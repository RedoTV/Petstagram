using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Petsgram.Application.DTOs.PetTypes;
using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Application.Interfaces.UnitOfWork;
using Petsgram.Application.Services.PetTypes;
using Petsgram.Domain.Entities;

namespace Petsgram.UnitTests.ServiceTests;

public class PetTypeServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IPetTypeRepository> _mockPetTypeRepository;
    private readonly PetTypeService _petTypeService;

    public PetTypeServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockPetTypeRepository = new Mock<IPetTypeRepository>();

        _mockUnitOfWork.Setup(u => u.PetTypes).Returns(_mockPetTypeRepository.Object);

        _petTypeService = new PetTypeService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnNotEmpty_EnumerableOf_PetTypeResponse()
    {
        //Arrange
        var petTypes = new List<PetType>
        {
            new PetType() { Id = 1, Name = "Cat" },
            new PetType() { Id = 2, Name = "Dog" }
        };

        _mockPetTypeRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(petTypes);

        _mockMapper
            .Setup(mapper => mapper.Map<PetTypeResponse>(It.IsAny<PetType>()))
            .Returns<PetType>(pt => new PetTypeResponse
            {
                Id = pt.Id,
                Name = pt.Name
            });

        //Act
        var result = await _petTypeService.GetAllAsync();
        var resultList = result.ToList();

        //Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(2, resultList.Count());
        Assert.Equal(1, resultList[0].Id);
        Assert.Equal("Cat", resultList[0].Name);
        Assert.Equal(2, resultList[1].Id);
        Assert.Equal("Dog", resultList[1].Name);

        _mockPetTypeRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoPetTypes()
    {
        //Arrange
        _mockPetTypeRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<PetType>());

        //Act
        var result = await _petTypeService.GetAllAsync();

        //Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockPetTypeRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GetByIdAsync_ReturnNotEmpty_PetTypeResponse(int id)
    {
        //Arrange
        var petTypes = new List<PetType>
        {
            new PetType() { Id = 1, Name = "Cat" },
            new PetType() { Id = 2, Name = "Dog" }
        };

        var expectedPetType = petTypes.First(pt => pt.Id == id);

        _mockPetTypeRepository
            .Setup(r => r.FindAsync(id))
            .ReturnsAsync(expectedPetType);

        _mockMapper
            .Setup(mapper => mapper.Map<PetTypeResponse>(It.IsAny<PetType>()))
            .Returns<PetType>(pt => new PetTypeResponse
            {
                Id = pt.Id,
                Name = pt.Name
            });

        //Act
        var result = await _petTypeService.GetByIdAsync(id);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPetType.Id, result.Id);
        Assert.Equal(expectedPetType.Name, result.Name);

        _mockPetTypeRepository.Verify(r => r.FindAsync(id), Times.Once);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(10)]
    public async Task GetByIdAsync_ThrowsArgumentException_WhenPetTypeNotFound(int id)
    {
        //Arrange
        _mockPetTypeRepository
            .Setup(r => r.FindAsync(id))
            .ReturnsAsync((PetType?)null);

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _petTypeService.GetByIdAsync(id));

        //Assert
        Assert.Equal($"PetType with id:{id} not found", exception.Message);

        _mockPetTypeRepository.Verify(r => r.FindAsync(id), Times.Once);
    }

    [Fact]
    public async Task AddTypeAsync_AddsNewPetType_AndCompletesUnitOfWork()
    {
        //Arrange
        var petTypeName = "Fish";

        _mockPetTypeRepository
            .Setup(r => r.AddAsync(It.IsAny<PetType>()))
            .Returns<PetType>(pt => Task.FromResult(pt));

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .ReturnsAsync(1);

        //Act
        await _petTypeService.AddTypeAsync(petTypeName);

        //Assert
        _mockPetTypeRepository.Verify(r =>
            r.AddAsync(It.Is<PetType>(pt => pt.Name == petTypeName)),
            Times.Once
        );
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AddTypeAsync_ThrowsArgumentException_WhenNameIsInvalid(string name)
    {
        //Arrange

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _petTypeService.AddTypeAsync(name));

        //Assert
        Assert.Contains("name", exception.Message.ToLower());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    public async Task RemoveTypeAsync_RemovesPetType_AndCompletesUnitOfWork(int id)
    {
        //Arrange
        _mockPetTypeRepository
            .Setup(r => r.RemoveAsync(id))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .ReturnsAsync(1);

        //Act
        await _petTypeService.RemoveTypeAsync(id);

        //Assert
        _mockPetTypeRepository.Verify(r => r.RemoveAsync(id), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Theory]
    [InlineData(1, "Dog")]
    [InlineData(2, "Monkey")]
    public async Task UpdateTypeAsync_UpdatesExistingPetType_AndCompletesUnitOfWork(int id, string newName)
    {
        //Arrange
        var existingType = new PetType { Id = id, Name = "OldName" };

        _mockPetTypeRepository
            .Setup(r => r.FindAsync(id))
            .ReturnsAsync(existingType);

        _mockPetTypeRepository
            .Setup(r => r.UpdateAsync(It.IsAny<PetType>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .ReturnsAsync(1);

        //Act
        await _petTypeService.UpdateTypeAsync(id, newName);

        //Assert
        Assert.Equal(newName, existingType.Name);

        _mockPetTypeRepository.Verify(r => r.FindAsync(id), Times.Once);
        _mockPetTypeRepository.Verify(r => r.UpdateAsync(existingType), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(134)]
    public async Task UpdateTypeAsync_ThrowsArgumentException_WhenPetTypeNotFound(int id)
    {
        //Arrange
        _mockPetTypeRepository
            .Setup(r => r.FindAsync(id))
            .ReturnsAsync((PetType?)null);

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _petTypeService.UpdateTypeAsync(id, "AnyName"));

        //Assert
        Assert.Equal($"PetType with id:{id} not found", exception.Message);
        _mockPetTypeRepository.Verify(r => r.FindAsync(id), Times.Once);
        _mockPetTypeRepository.Verify(r => r.UpdateAsync(It.IsAny<PetType>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
    }

    [Theory]
    [InlineData(1, null)]
    [InlineData(1, "")]
    [InlineData(1, "   ")]
    public async Task UpdateTypeAsync_ThrowsArgumentException_WhenNameIsInvalid(int id, string name)
    {
        //Arrange
        var existingType = new PetType { Id = id, Name = "OldName" };

        _mockPetTypeRepository
            .Setup(r => r.FindAsync(id))
            .ReturnsAsync(existingType);

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _petTypeService.UpdateTypeAsync(id, name));

        //Assert
        Assert.Contains("name", exception.Message.ToLower());
    }
}
