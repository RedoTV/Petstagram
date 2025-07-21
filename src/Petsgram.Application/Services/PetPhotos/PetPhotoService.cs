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

    private readonly string _photoPhysicalPath;
    private readonly string _photoPublicPath;

    public PetPhotoService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<StorageSettings> storageOptions)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;

        _photoPhysicalPath = storageOptions.Value.PhotoPhysicalPath;
        _photoPublicPath = storageOptions.Value.PhotoPublicPath;

        if (string.IsNullOrEmpty(_photoPhysicalPath) || string.IsNullOrEmpty(_photoPublicPath))
        {
            throw new ArgumentException("Storage paths are not configured correctly.");
        }
    }

    public async Task<IEnumerable<PetPhotoResponse>> GetAllByPetIdAsync(int petId)
    {
        var photos = await _unitOfWork.PetPhotos.GetAllAsync(petId);
        return photos.Select(p => _mapper.Map<PetPhotoResponse>(p));
    }

    public async Task<PetPhotoResponse> GetByIdAsync(int id)
    {
        var photo = await _unitOfWork.PetPhotos.FindAsync(id);
        if (photo == null)
            throw new NullReferenceException($"PetPhoto with id:{id} not found");

        return _mapper.Map<PetPhotoResponse>(photo);
    }

    public async Task AddPhotoAsync(int petId, int userId, Stream fileStream, string fileName)
    {
        var userFolder = $"user_{userId}";
        var ext = Path.GetExtension(fileName);
        var uniqueName = $"photo_{Guid.NewGuid()}{ext}";

        var storagePath = Path.Combine(_photoPhysicalPath, userFolder);
        Directory.CreateDirectory(storagePath);

        var filePath = Path.Combine(storagePath, uniqueName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        var publicUrl = $"{_photoPublicPath}/{userFolder}/{uniqueName}";
        var photo = new PetPhoto
        {
            PetId = petId,
            Path = filePath,
            PublicUrl = publicUrl
        };

        await _unitOfWork.PetPhotos.AddAsync(photo);
        await _unitOfWork.CompleteAsync();
    }

    public async Task RemovePhotoAsync(int id)
    {
        var photo = await _unitOfWork.PetPhotos.FindAsync(id);
        if (photo == null)
            throw new NullReferenceException($"Photo (id:{id}) not found");

        await Task.Run(() => File.Delete(photo.Path));

        await _unitOfWork.PetPhotos.RemoveAsync(id);
        await _unitOfWork.CompleteAsync();
    }
}