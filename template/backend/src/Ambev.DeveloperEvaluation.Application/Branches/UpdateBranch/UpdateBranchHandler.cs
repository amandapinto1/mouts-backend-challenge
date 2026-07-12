using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Branches.UpdateBranch;

public class UpdateBranchHandler : IRequestHandler<UpdateBranchCommand, UpdateBranchResult>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public UpdateBranchHandler(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<UpdateBranchResult> Handle(UpdateBranchCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateBranchCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var branch = await _branchRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Branch with Id {command.Id} not found");

        branch.Name = command.Name;
        branch.Address = command.Address;
        branch.City = command.City;
        branch.State = command.State;
        branch.IsActive = command.IsActive;

        var updated = await _branchRepository.UpdateAsync(branch, cancellationToken);
        return _mapper.Map<UpdateBranchResult>(updated);
    }
}
