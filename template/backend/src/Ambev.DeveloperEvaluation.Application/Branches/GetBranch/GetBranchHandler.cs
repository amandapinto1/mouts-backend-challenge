using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Branches.GetBranch;

public class GetBranchHandler : IRequestHandler<GetBranchCommand, GetBranchResult>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public GetBranchHandler(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<GetBranchResult> Handle(GetBranchCommand command, CancellationToken cancellationToken)
    {
        var validator = new GetBranchCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var branch = await _branchRepository.GetByIdAsync(command.Id, cancellationToken);
        return branch == null
            ? throw new KeyNotFoundException($"Branch with Id {command.Id} not found")
            : _mapper.Map<GetBranchResult>(branch);
    }
}
