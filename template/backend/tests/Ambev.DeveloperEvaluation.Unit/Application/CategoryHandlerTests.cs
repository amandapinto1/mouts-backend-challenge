using Ambev.DeveloperEvaluation.Application.Categories.CreateCategory;
using Ambev.DeveloperEvaluation.Application.Categories.GetCategory;
using Ambev.DeveloperEvaluation.Application.Categories.UpdateCategory;
using Ambev.DeveloperEvaluation.Application.Categories.DeleteCategory;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CategoryHandlerTests
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly Faker _faker = new();

    public CategoryHandlerTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _mapper = Substitute.For<IMapper>();
    }

    [Fact(DisplayName = "Given valid category command When creating Then returns category result")]
    public async Task CreateCategory_ValidCommand_ReturnsResult()
    {
        // Arrange
        var handler = new CreateCategoryHandler(_categoryRepository, _mapper);
        var command = new CreateCategoryCommand
        {
            Name = _faker.Commerce.Categories(1)[0],
            Description = _faker.Lorem.Sentence()
        };

        var category = new Category { Id = Guid.NewGuid(), Name = command.Name };
        var expectedResult = new CreateCategoryResult { Id = category.Id, Name = category.Name };

        _mapper.Map<Category>(command).Returns(category);
        _categoryRepository.CreateAsync(category, Arg.Any<CancellationToken>()).Returns(category);
        _mapper.Map<CreateCategoryResult>(category).Returns(expectedResult);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(category.Id);
        await _categoryRepository.Received(1).CreateAsync(category, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given empty name When creating category Then throws ValidationException")]
    public async Task CreateCategory_EmptyName_ThrowsValidationException()
    {
        // Arrange
        var handler = new CreateCategoryHandler(_categoryRepository, _mapper);
        var command = new CreateCategoryCommand { Name = "", Description = "Desc" };

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given existing category When getting by Id Then returns category")]
    public async Task GetCategory_ExistingId_ReturnsCategory()
    {
        // Arrange
        var handler = new GetCategoryHandler(_categoryRepository, _mapper);
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        var expectedResult = new GetCategoryResult { Id = categoryId, Name = "Electronics" };

        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(category);
        _mapper.Map<GetCategoryResult>(category).Returns(expectedResult);

        // Act
        var result = await handler.Handle(new GetCategoryCommand { Id = categoryId }, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(categoryId);
    }

    [Fact(DisplayName = "Given non-existent category When getting by Id Then throws KeyNotFoundException")]
    public async Task GetCategory_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var handler = new GetCategoryHandler(_categoryRepository, _mapper);
        _categoryRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Category?)null);

        // Act
        var act = async () => await handler.Handle(new GetCategoryCommand { Id = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given existing category When updating Then returns updated category")]
    public async Task UpdateCategory_ValidCommand_ReturnsUpdatedCategory()
    {
        // Arrange
        var handler = new UpdateCategoryHandler(_categoryRepository, _mapper);
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Old Name" };

        var command = new UpdateCategoryCommand
        {
            Id = categoryId,
            Name = "New Name",
            Description = "Updated Desc",
            IsActive = true
        };

        var expectedResult = new UpdateCategoryResult { Id = categoryId, Name = "New Name" };

        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(category);
        _categoryRepository.UpdateAsync(category, Arg.Any<CancellationToken>()).Returns(category);
        _mapper.Map<UpdateCategoryResult>(category).Returns(expectedResult);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        category.Name.Should().Be("New Name");
        category.Description.Should().Be("Updated Desc");
    }

    [Fact(DisplayName = "Given non-existent category When updating Then throws KeyNotFoundException")]
    public async Task UpdateCategory_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var handler = new UpdateCategoryHandler(_categoryRepository, _mapper);
        _categoryRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Category?)null);

        var command = new UpdateCategoryCommand { Id = Guid.NewGuid(), Name = "Test", Description = "Desc" };

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given existing category When deleting Then returns success")]
    public async Task DeleteCategory_ExistingId_ReturnsSuccess()
    {
        // Arrange
        var handler = new DeleteCategoryHandler(_categoryRepository);
        var categoryId = Guid.NewGuid();
        _categoryRepository.DeleteAsync(categoryId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await handler.Handle(new DeleteCategoryCommand { Id = categoryId }, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Contain("deleted");
    }

    [Fact(DisplayName = "Given non-existent category When deleting Then throws KeyNotFoundException")]
    public async Task DeleteCategory_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var handler = new DeleteCategoryHandler(_categoryRepository);
        _categoryRepository.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var act = async () => await handler.Handle(new DeleteCategoryCommand { Id = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
