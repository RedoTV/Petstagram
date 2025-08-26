using Petsgram.Application.DTOs.Pets;
using Petsgram.Application.Interfaces.Pets;
using Petsgram.Application.Interfaces.UnitOfWork;
using Petsgram.Application.Interfaces.Auth;
using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Application.Interfaces.ExchangeRate;
using Petsgram.Application.Settings;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Petsgram.Domain.Entities;
using Petsgram.Domain.Exceptions.Pet;

namespace Petsgram.Application.Services.Pets;

public class PetService : IPetService
{
    private readonly IPetRepository _petRepository;
    private readonly IPetTypeRepository _petTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IExchangeRateService _exchangeRateService;
    private readonly ExchangeRateSettings _exchangeSettings;
    private readonly IMapper _mapper;
    private readonly ILogger<PetService> _logger;

    public PetService(
        IPetRepository petRepository,
        IPetTypeRepository petTypeRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<PetService> logger,
        IExchangeRateService exchangeRateService,
        IOptions<ExchangeRateSettings> exchangeSettings)
    {
        _petRepository = petRepository;
        _petTypeRepository = petTypeRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
        _exchangeRateService = exchangeRateService;
        _exchangeSettings = exchangeSettings.Value;
    }

    public async Task<List<PetResponse>> GetCurrentUserPetsAsync(
        string? targetCurrency = null,
        CancellationToken cancellationToken = default)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null)
        {
            _logger.LogWarning("User not authenticated when trying to get current user pets");
            throw new PetUnauthorizedException("User not authenticated");
        }

        var pets = await _petRepository.GetAllAsync(currentUser.Id, cancellationToken);
        var responses = _mapper.Map<List<PetResponse>>(pets);
        
        if (!string.IsNullOrEmpty(targetCurrency))
        {
            await ConvertPetPricesToCurrency(responses, targetCurrency);
        }

        _logger.LogInformation("Returned {Count} pets for current user {UserId}", responses.Count, currentUser.Id);
        return responses;
    }

    public async Task<List<PetResponse>> GetUserPetsAsync(
        int userId, 
        string? targetCurrency = null,
        CancellationToken cancellationToken = default)
    {
        var pets = await _petRepository.GetAllAsync(userId, cancellationToken);
        var responses = _mapper.Map<List<PetResponse>>(pets);
        
        if (!string.IsNullOrEmpty(targetCurrency))
        {
            await ConvertPetPricesToCurrency(responses, targetCurrency);
        }

        _logger.LogInformation("Returned {Count} pets for user {UserId}", responses.Count, userId);
        return responses;
    }

    public async Task<PetResponse> GetPetByIdAsync(
        int petId, 
        string? targetCurrency = null,
        CancellationToken cancellationToken = default)
    {
        var pet = await _petRepository.FindAsync(petId, cancellationToken);
        if (pet == null)
        {
            _logger.LogWarning("Pet with id {PetId} not found", petId);
            throw new PetNotFoundException(petId);
        }

        var response = _mapper.Map<PetResponse>(pet);
        
        if (!string.IsNullOrEmpty(targetCurrency) && response.Currency != targetCurrency)
        {
            response.Price = await _exchangeRateService.ConvertAsync(
                response.Price, response.Currency, targetCurrency);
            response.Currency = targetCurrency;
        }

        _logger.LogInformation("Returned pet with id {PetId}", petId);
        return response;
    }

    public async Task<PetResponse> AddPetToCurrentUserAsync(CreatePetRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.PetName))
            throw new PetValidationException("Pet name cannot be empty");

        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null)
        {
            _logger.LogWarning("User not authenticated when trying to create pet");
            throw new PetUnauthorizedException("User not authenticated");
        }

        var petType = await _petTypeRepository.GetByNameAsync(request.PetType, cancellationToken);
        if (petType == null)
        {
            _logger.LogWarning("Pet type '{PetType}' not found when creating pet", request.PetType);
            throw new PetValidationException($"Pet type '{request.PetType}' not found");
        }
        
        var baseCurrency = _exchangeSettings.DefaultBaseCurrency;
        var priceInBaseCurrency = await _exchangeRateService
            .ConvertAsync(request.Amount, request.CurrencyCode, baseCurrency);

        var pet = _mapper.Map<Pet>(request);
        pet.UserId = currentUser.Id;
        pet.PetTypeId = petType.Id;
        pet.Price = priceInBaseCurrency;
        pet.Currency = baseCurrency;

        await _petRepository.AddAsync(pet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pet created: {PetName} for user {UserId}, price: {Price} {Currency}", 
            request.PetName, currentUser.Id, priceInBaseCurrency, baseCurrency);
        
        return _mapper.Map<PetResponse>(pet);
    }
    
    public async Task<PetResponse> UpdatePetAsync(int petId, UpdatePetRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.PetName))
            throw new PetValidationException("Pet name cannot be empty");

        var pet = await _petRepository.FindAsync(petId, cancellationToken);
        if (pet == null)
        {
            _logger.LogWarning("Pet with id {PetId} not found for update", petId);
            throw new PetNotFoundException(petId);
        }

        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null || pet.UserId != currentUser.Id)
        {
            _logger.LogWarning("User {UserId} not authorized to update pet {PetId}", currentUser?.Id, petId);
            throw new PetUnauthorizedException("Not authorized to update this pet");
        }

        var petType = await _petTypeRepository.GetByNameAsync(request.PetType, cancellationToken);
        if (petType == null)
        {
            _logger.LogWarning("Pet type '{PetType}' not found when updating pet {PetId}", request.PetType, petId);
            throw new PetValidationException($"Pet type '{request.PetType}' not found");
        }
        
        pet.PetName = request.PetName;
        pet.PetTypeId = petType.Id;
        
        var baseCurrency = _exchangeSettings.DefaultBaseCurrency;
        pet.Price = await _exchangeRateService.ConvertAsync(request.Amount, request.CurrencyCode, baseCurrency);
        pet.Currency = baseCurrency;

        await _petRepository.UpdateAsync(pet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pet updated: id={PetId}, name={PetName}, price={Price} {Currency}", petId, request.PetName, pet.Price, pet.Currency);
        return _mapper.Map<PetResponse>(pet);
    }
    
    public async Task<PetResponse> RemovePetAsync(int petId, CancellationToken cancellationToken = default)
    {
        var pet = await _petRepository.FindAsync(petId, cancellationToken);
        if (pet == null)
        {
            _logger.LogWarning("Pet with id {PetId} not found for deletion", petId);
            throw new PetNotFoundException(petId);
        }

        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null || pet.UserId != currentUser.Id)
        {
            _logger.LogWarning("User {UserId} not authorized to delete pet {PetId}", currentUser?.Id, petId);
            throw new PetUnauthorizedException("Not authorized to delete this pet");
        }

        await _petRepository.RemoveAsync(petId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pet deleted: id={PetId}", petId);
        return _mapper.Map<PetResponse>(pet);
    }
    
    private async Task ConvertPetPricesToCurrency(List<PetResponse> pets, string targetCurrency)
    {
        var conversionTasks = pets
            .Where(pet => pet.Currency != targetCurrency)
            .Select(async pet =>
            {
                pet.Price = await _exchangeRateService.ConvertAsync(pet.Price, pet.Currency, targetCurrency);
                pet.Currency = targetCurrency;
            });

        await Task.WhenAll(conversionTasks);
    }
}
