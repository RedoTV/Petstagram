using Petsgram.Application.DTOs.PetPhotos;
using Petsgram.Application.Interfaces.PetPhotos;
using Petsgram.Application.Interfaces.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Petsgram.Domain.Entities;
using Microsoft.Extensions.Options;
using Petsgram.Application.Interfaces.Auth;
using Petsgram.Application.Interfaces.Pets;
using Petsgram.Application.Settings;
using Petsgram.Domain.Exceptions.PetPhoto;

namespace Petsgram.Application.Services.PetPhotos;

public class PetPhotoService : IPetPhotoService
{
    private readonly IPetPhotoRepository _petPhotoRepository;
    private readonly IPetRepository _petRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<PetPhotoService> _logger;
    private readonly string _photoPhysicalPath;
    private readonly string _photoPublicPath;

    public PetPhotoService(
        IPetPhotoRepository petPhotoRepository,
        IPetRepository petRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<PetPhotoService> logger,
        IOptions<StorageSettings> storageOptions)
    {
        _petPhotoRepository = petPhotoRepository;
        _petRepository = petRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;

        _photoPhysicalPath = storageOptions.Value.PhotoPhysicalPath;
        _photoPublicPath = storageOptions.Value.PhotoPublicPath;

        if (string.IsNullOrEmpty(_photoPhysicalPath) || string.IsNullOrEmpty(_photoPublicPath))
            throw new PetPhotoValidationException("Storage paths are not configured correctly");
    }

    public async Task<List<PetPhotoResponse>> GetAllByPetIdAsync(int petId, CancellationToken cancellationToken = default)
    {
        var photos = await _petPhotoRepository.GetAllAsync(petId, cancellationToken);
        _logger.LogInformation("Returned {Count} photos for pet {PetId}", photos.Count, petId);
        return _mapper.Map<List<PetPhotoResponse>>(photos);
    }

    public async Task<PetPhotoResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var photo = await _petPhotoRepository.FindAsync(id, cancellationToken);
        if (photo == null)
        {
            _logger.LogWarning("Pet photo with id {PhotoId} not found", id);
            throw new PetPhotoNotFoundException(id);
        }

        _logger.LogInformation("Returned pet photo with id {PhotoId}", id);
        return _mapper.Map<PetPhotoResponse>(photo);
    }

    public async Task<PetPhotoResponse> AddPhotoAsync(UploadPhotoRequest request, CancellationToken cancellationToken = default)
    {
        if (request.File == null || request.File.Length == 0)
            throw new PetPhotoValidationException("No file uploaded");

        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null)
        {
            _logger.LogWarning("User not authenticated when trying to upload photo");
            throw new PetPhotoUnauthorizedException("User not authenticated");
        }

        var pet = await _petRepository.FindAsync(request.PetId, cancellationToken);
        if (pet == null)
        {
            _logger.LogWarning("Pet with id {PetId} not found when uploading photo", request.PetId);
            throw new PetPhotoValidationException($"Pet with id {request.PetId} not found");
        }

        if (pet.UserId != currentUser.Id)
        {
            _logger.LogWarning("User {UserId} not authorized to upload photo for pet {PetId}", currentUser.Id, request.PetId);
            throw new PetPhotoUnauthorizedException("Not authorized to upload photo for this pet");
        }

        var userFolder = $"user_{currentUser.Id}";
        var ext = Path.GetExtension(request.File.FileName);
        var uniqueName = $"photo_{Guid.NewGuid()}{ext}";

        var storagePath = Path.Combine(_photoPhysicalPath, userFolder);
        Directory.CreateDirectory(storagePath);

        var filePath = Path.Combine(storagePath, uniqueName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        var publicUrl = $"{_photoPublicPath}/{userFolder}/{uniqueName}";
        var photo = new PetPhoto
        {
            PetId = request.PetId,
            Path = filePath,
            PublicUrl = publicUrl
        };

        await _petPhotoRepository.AddAsync(photo, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Photo uploaded for pet {PetId} by user {UserId}", request.PetId, currentUser.Id);
        return _mapper.Map<PetPhotoResponse>(photo);
    }

    public async Task<PetPhotoResponse> RemovePhotoAsync(int id, CancellationToken cancellationToken = default)
    {
        var photo = await _petPhotoRepository.FindAsync(id, cancellationToken);
        if (photo == null)
        {
            _logger.LogWarning("Pet photo with id {PhotoId} not found for deletion", id);
            throw new PetPhotoNotFoundException(id);
        }

        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null)
        {
            _logger.LogWarning("User not authenticated when trying to delete photo {PhotoId}", id);
            throw new PetPhotoUnauthorizedException("User not authenticated");
        }

        var pet = await _petRepository.FindAsync(photo.PetId, cancellationToken);
        if (pet?.UserId != currentUser.Id)
        {
            _logger.LogWarning("User {UserId} not authorized to delete photo {PhotoId}", currentUser.Id, id);
            throw new PetPhotoUnauthorizedException("Not authorized to delete this photo");
        }

        if (File.Exists(photo.Path))
        {
            File.Delete(photo.Path);
        }

        await _petPhotoRepository.RemoveAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pet photo deleted: id={PhotoId}", id);
        return _mapper.Map<PetPhotoResponse>(photo);
    }
}