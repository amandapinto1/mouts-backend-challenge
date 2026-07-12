using Ambev.DeveloperEvaluation.Application.Branches.CreateBranch;
using Ambev.DeveloperEvaluation.Application.Branches.GetBranch;
using Ambev.DeveloperEvaluation.Application.Branches.UpdateBranch;
using Ambev.DeveloperEvaluation.Application.Branches.DeleteBranch;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class BranchHandlerTests
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;
    private readonly Faker _faker = new();

    public BranchHandlerTests()
    {
        _branchRepository = Substitute.For<IBranchRepository>();
        _mapper = Substitute.For<IMapper>();
    }

    [Fact(DisplayName = "Given valid branch command When creating Then returns branch result")]
    public async Task CreateBranch_ValidCommand_ReturnsResult()
    {
        // Arrange
        var handler = new CreateBranchHandler(_branchRepository, _mapper);
        var command = new CreateBranchCommand
        {
            Name = _faker.Company.CompanyName(),
            Address = _faker.Address.StreetAddress(),
            City = _faker.Address.City(),
            State = _faker.Address.State()
        };

        var branch = new Branch { Id = Guid.NewGuid(), Name = command.Name };
        var expectedResult = new CreateBranchResult { Id = branch.Id, Name = branch.Name };

        _mapper.Map<Branch>(command).Returns(branch);
        _branchRepository.CreateAsync(branch, Arg.Any<CancellationToken>()).Returns(branch);
        _mapper.Map<CreateBranchResult>(branch).Returns(expectedResult);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(branch.Id);
        await _branchRepository.Received(1).CreateAsync(branch, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given empty name When creating branch Then throws ValidationException")]
    public async Task CreateBranch_EmptyName_ThrowsValidationException()
    {
        // Arrange
        var handler = new CreateBranchHandler(_branchRepository, _mapper);
        var command = new CreateBranchCommand { Name = "", Address = "Addr", City = "City", State = "ST" };

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given existing branch When getting by Id Then returns branch")]
    public async Task GetBranch_ExistingId_ReturnsBranch()
    {
        // Arrange
        var handler = new GetBranchHandler(_branchRepository, _mapper);
        var branchId = Guid.NewGuid();
        var branch = new Branch { Id = branchId, Name = "Test Branch" };
        var expectedResult = new GetBranchResult { Id = branchId, Name = "Test Branch" };

        _branchRepository.GetByIdAsync(branchId, Arg.Any<CancellationToken>()).Returns(branch);
        _mapper.Map<GetBranchResult>(branch).Returns(expectedResult);

        // Act
        var result = await handler.Handle(new GetBranchCommand { Id = branchId }, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(branchId);
    }

    [Fact(DisplayName = "Given non-existent branch When getting by Id Then throws KeyNotFoundException")]
    public async Task GetBranch_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var handler = new GetBranchHandler(_branchRepository, _mapper);
        _branchRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Branch?)null);

        // Act
        var act = async () => await handler.Handle(new GetBranchCommand { Id = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given existing branch When updating Then returns updated branch")]
    public async Task UpdateBranch_ValidCommand_ReturnsUpdatedBranch()
    {
        // Arrange
        var handler = new UpdateBranchHandler(_branchRepository, _mapper);
        var branchId = Guid.NewGuid();
        var branch = new Branch { Id = branchId, Name = "Old Name" };

        var command = new UpdateBranchCommand
        {
            Id = branchId,
            Name = "New Name",
            Address = "New Address",
            City = "New City",
            State = "NS",
            IsActive = true
        };

        var expectedResult = new UpdateBranchResult { Id = branchId, Name = "New Name" };

        _branchRepository.GetByIdAsync(branchId, Arg.Any<CancellationToken>()).Returns(branch);
        _branchRepository.UpdateAsync(branch, Arg.Any<CancellationToken>()).Returns(branch);
        _mapper.Map<UpdateBranchResult>(branch).Returns(expectedResult);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        branch.Name.Should().Be("New Name");
        branch.Address.Should().Be("New Address");
    }

    [Fact(DisplayName = "Given non-existent branch When updating Then throws KeyNotFoundException")]
    public async Task UpdateBranch_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var handler = new UpdateBranchHandler(_branchRepository, _mapper);
        _branchRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Branch?)null);

        var command = new UpdateBranchCommand { Id = Guid.NewGuid(), Name = "Test", Address = "Addr", City = "City", State = "ST" };

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given existing branch When deleting Then returns success")]
    public async Task DeleteBranch_ExistingId_ReturnsSuccess()
    {
        // Arrange
        var handler = new DeleteBranchHandler(_branchRepository);
        var branchId = Guid.NewGuid();
        _branchRepository.DeleteAsync(branchId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await handler.Handle(new DeleteBranchCommand { Id = branchId }, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Contain("deleted");
    }

    [Fact(DisplayName = "Given non-existent branch When deleting Then throws KeyNotFoundException")]
    public async Task DeleteBranch_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var handler = new DeleteBranchHandler(_branchRepository);
        _branchRepository.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var act = async () => await handler.Handle(new DeleteBranchCommand { Id = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
