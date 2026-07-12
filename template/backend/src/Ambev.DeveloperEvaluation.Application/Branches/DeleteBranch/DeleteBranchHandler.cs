using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Branches.DeleteBranch;

public class DeleteBranchHandler : IRequestHandler<DeleteBranchCommand, DeleteBranchResult>
{
    private readonly IBranchRepository _branchRepository;

    public DeleteBranchHandler(IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository;
    }

    public async Task<DeleteBranchResult> Handle(DeleteBranchCommand command, CancellationToken cancellationToken)
    {
        var validator = new DeleteBranchCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var deleted = await _branchRepository.DeleteAsync(command.Id, cancellationToken);
        if (!deleted)
            throw new KeyNotFoundException($"Branch with Id {command.Id} not found");

        return new DeleteBranchResult { Message = "Branch deleted successfully" };
    }
}
