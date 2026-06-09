using VitalSyncAPI.Application.DTOs.Request;
using VitalSyncAPI.Application.UseCases;
using SysValidation = System.ComponentModel.DataAnnotations.ValidationException;

namespace VitalSyncAPI.Tests;

public class ConditionMedicationUseCaseTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    // ── AddUserConditionUseCase ────────────────────────────────────────────

    [Fact]
    public async Task AddConditions_SubstituiListaEAdicionaNovas()
    {
        var conditionRepo = Substitute.For<IUserConditionRepository>();
        var unitOfWork    = Substitute.For<IUnitOfWork>();

        conditionRepo.GetByUserIdAndConditionId(UserId, Arg.Any<int>())
            .Returns((UserCondition?)null);

        var useCase  = new AddUserConditionUseCase(conditionRepo, unitOfWork);
        var request  = new List<UserConditionRequest>
        {
            new(1),
            new(2),
        };

        await useCase.ExecuteAsync(UserId, request);

        await conditionRepo.Received(1).RemoveAllByUserId(UserId);
        await conditionRepo.Received(2).AddCondition(Arg.Any<UserCondition>());
        await unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task AddConditions_ListaVazia_ApenasRemoveExistentes()
    {
        var conditionRepo = Substitute.For<IUserConditionRepository>();
        var unitOfWork    = Substitute.For<IUnitOfWork>();

        var useCase = new AddUserConditionUseCase(conditionRepo, unitOfWork);
        await useCase.ExecuteAsync(UserId, new List<UserConditionRequest>());

        await conditionRepo.Received(1).RemoveAllByUserId(UserId);
        await conditionRepo.DidNotReceive().AddCondition(Arg.Any<UserCondition>());
        await unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task AddConditions_CondicaoDuplicadaNoRequest_LancaValidationException()
    {
        var conditionRepo = Substitute.For<IUserConditionRepository>();
        var unitOfWork    = Substitute.For<IUnitOfWork>();

        // Primeira chamada retorna null (add), segunda retorna objeto (duplicata detectada)
        conditionRepo.GetByUserIdAndConditionId(UserId, 1)
            .Returns((UserCondition?)null,
                     new UserCondition { UserId = UserId, ConditionId = 1 });

        var useCase  = new AddUserConditionUseCase(conditionRepo, unitOfWork);
        var request  = new List<UserConditionRequest> { new(1), new(1) };

        await Assert.ThrowsAsync<SysValidation>(
            () => useCase.ExecuteAsync(UserId, request));
    }

    // ── AddUserMedicationUseCase ───────────────────────────────────────────

    [Fact]
    public async Task AddMedications_SubstituiListaEAdicionaNovas()
    {
        var medicationRepo = Substitute.For<IUserMedicationRepository>();
        var unitOfWork     = Substitute.For<IUnitOfWork>();

        var useCase  = new AddUserMedicationUseCase(medicationRepo, unitOfWork);
        var request  = new List<UserMedicationRequest>
        {
            new(MedicationClass.BetaBlockers, null),
            new(MedicationClass.Diuretics,    "1x ao dia"),
        };

        await useCase.ExecuteAsync(UserId, request);

        await medicationRepo.Received(1).RemoveAllByUserId(UserId);
        await medicationRepo.Received(2).AddMedication(Arg.Any<UserMedication>());
        await unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task AddMedications_ListaVazia_ApenasRemoveExistentes()
    {
        var medicationRepo = Substitute.For<IUserMedicationRepository>();
        var unitOfWork     = Substitute.For<IUnitOfWork>();

        var useCase = new AddUserMedicationUseCase(medicationRepo, unitOfWork);
        await useCase.ExecuteAsync(UserId, new List<UserMedicationRequest>());

        await medicationRepo.Received(1).RemoveAllByUserId(UserId);
        await medicationRepo.DidNotReceive().AddMedication(Arg.Any<UserMedication>());
    }

    [Fact]
    public async Task AddMedications_MedicamentoDuplicadoNoRequest_LancaValidationException()
    {
        var medicationRepo = Substitute.For<IUserMedicationRepository>();
        var unitOfWork     = Substitute.For<IUnitOfWork>();

        var useCase  = new AddUserMedicationUseCase(medicationRepo, unitOfWork);
        var request  = new List<UserMedicationRequest>
        {
            new(MedicationClass.BetaBlockers, null),
            new(MedicationClass.BetaBlockers, "duplicata"),
        };

        await Assert.ThrowsAsync<SysValidation>(
            () => useCase.ExecuteAsync(UserId, request));

        await medicationRepo.DidNotReceive().RemoveAllByUserId(UserId);
    }
}
