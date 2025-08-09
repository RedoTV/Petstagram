using Petsgram.Application.DTOs.PetPhotos;
using Petsgram.Application.Interfaces.PetPhotos;
using Petsgram.Application.Interfaces.UnitOfWork;
using AutoMapper;
using Petsgram.Domain.Entities;
using Microsoft.Extensions.Options;
using Petsgram.Application.Settings;

namespace Petsgram.Application.Services.PetPhotos;

public class PetPhotoService : IPetPhotoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPetPhotoRepository _petPhotoRepository;

    private readonly string _photoPhysicalPath;
    private readonly string _photoPublicPath;

    public PetPhotoService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPetPhotoRepository petPhotoRepository,
        IOptions<StorageSettings> storageOptions)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _petPhotoRepository = petPhotoRepository;

        _photoPhysicalPath = storageOptions.Value.PhotoPhysicalPath;
        _photoPublicPath = storageOptions.Value.PhotoPublicPath;

        if (string.IsNullOrEmpty(_photoPhysicalPath) || string.IsNullOrEmpty(_photoPublicPath))
        {
            throw new ArgumentException("Storage paths are not configured correctly.");
        }
    }

    public async Task<List<PetPhotoResponse>> GetAllByPetIdAsync(int petId, CancellationToken cancellationToken = default)
    {
        var photos = await _petPhotoRepository.GetAllAsync(petId, cancellationToken);
        return photos.Select(p => _mapper.Map<PetPhotoResponse>(p)).ToList();
    }

    public async Task<PetPhotoResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var photo = await _petPhotoRepository.FindAsync(id, cancellationToken);
        if (photo == null)
            throw new ArgumentException($"PetPhoto with id:{id} not found");

        return _mapper.Map<PetPhotoResponse>(photo);
    }

    public async Task AddPhotoAsync(int petId, int userId, Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var userFolder = $"user_{userId}";
        var ext = Path.GetExtension(fileName);
        var uniqueName = $"photo_{Guid.NewGuid()}{ext}";

        var storagePath = Path.Combine(_photoPhysicalPath, userFolder);
        Directory.CreateDirectory(storagePath);

        var filePath = Path.Combine(storagePath, uniqueName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream, cancellationToken);
        }

        var publicUrl = $"{_photoPublicPath}/{userFolder}/{uniqueName}";
        var photo = new PetPhoto
        {
            PetId = petId,
            Path = filePath,
            PublicUrl = publicUrl
        };

        await _petPhotoRepository.AddAsync(photo, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemovePhotoAsync(int id, CancellationToken cancellationToken = default)
    {
        var photo = await _petPhotoRepository.FindAsync(id, cancellationToken);
        if (photo == null)
            throw new ArgumentException($"Photo (id:{id}) not found");

        var removedFilePath = Path.Combine(Directory.GetCurrentDirectory(), photo.Path);
        await Task.Run(() => File.Delete(removedFilePath), cancellationToken);

        await _petPhotoRepository.RemoveAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
