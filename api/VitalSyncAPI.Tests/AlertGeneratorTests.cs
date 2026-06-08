using VitalSyncAPI.Domain.Entities;
using VitalSyncAPI.Domain.Enums;
using VitalSyncAPI.Domain.Services;

namespace VitalSyncAPI.Tests;

public class AlertGeneratorTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    private static HealthRecord MakeRecord(decimal value, int metricTypeId = 1) => new()
    {
        Id = Guid.NewGuid(),
        UserId = UserId,
        MetricTypeId = metricTypeId,
        Value = value,
        MeasuredAt = DateTime.UtcNow,
        MetricType = MakeMetricType(metricTypeId),
    };

    private static MetricType MakeMetricType(int id = 1) => new()
    {
        Id = id,
        Name = "Pressão Sistólica",
        Unit = "mmHg",
    };

    // ── Casos que NÃO geram alerta ─────────────────────────────────────────

    [Fact]
    public void Generate_QuandoAmbosLimitesNulos_RetornaNull()
    {
        var alert = AlertGenerator.Generate(MakeRecord(120), MakeMetricType(), null, null);
        Assert.Null(alert);
    }

    [Fact]
    public void Generate_QuandoValorDentroDoIntervalo_RetornaNull()
    {
        var alert = AlertGenerator.Generate(MakeRecord(100), MakeMetricType(), 90, 120);
        Assert.Null(alert);
    }

    [Fact]
    public void Generate_QuandoValorIgualAoMinimo_RetornaNull()
    {
        var alert = AlertGenerator.Generate(MakeRecord(90), MakeMetricType(), 90, 120);
        Assert.Null(alert);
    }

    [Fact]
    public void Generate_QuandoValorIgualAoMaximo_RetornaNull()
    {
        var alert = AlertGenerator.Generate(MakeRecord(120), MakeMetricType(), 90, 120);
        Assert.Null(alert);
    }

    [Fact]
    public void Generate_QuandoMaxNormalEhZero_NaoDispararAboveMax()
    {
        // max=0 representa ausência de limite superior (bug fix: 6000 passos > 0 não deve gerar alerta)
        var alert = AlertGenerator.Generate(MakeRecord(6000), MakeMetricType(), 5000, 0);
        Assert.Null(alert);
    }

    [Fact]
    public void Generate_QuandoAcimaDaMetrica11_RetornaNull()
    {
        // Métrica 11 (ex: passos) não gera alerta por estar acima do máximo
        var record = MakeRecord(200, metricTypeId: 11);
        var metric = MakeMetricType(11);
        var alert = AlertGenerator.Generate(record, metric, 60, 100);
        Assert.Null(alert);
    }

    [Fact]
    public void Generate_ComSomenteMinDefinido_ValorAcimaDoMin_RetornaNull()
    {
        var alert = AlertGenerator.Generate(MakeRecord(7000), MakeMetricType(), 5000, null);
        Assert.Null(alert);
    }

    [Fact]
    public void Generate_ComSomenteMaxDefinido_ValorAbaixoDoMax_RetornaNull()
    {
        var alert = AlertGenerator.Generate(MakeRecord(80), MakeMetricType(), null, 120);
        Assert.Null(alert);
    }

    // ── Casos que geram alerta ─────────────────────────────────────────────

    [Fact]
    public void Generate_QuandoValorAcimaDoMaximo_RetornaAlerta()
    {
        var record = MakeRecord(130);
        var alert = AlertGenerator.Generate(record, MakeMetricType(), 90, 120);

        Assert.NotNull(alert);
        Assert.Equal(UserId, alert.UserId);
        Assert.Equal(record.Id, alert.HealthRecordId);
        Assert.Equal(1, alert.MetricTypeId);
    }

    [Fact]
    public void Generate_QuandoValorAbaixoDoMinimo_RetornaAlerta()
    {
        var record = MakeRecord(80);
        var alert = AlertGenerator.Generate(record, MakeMetricType(), 90, 120);

        Assert.NotNull(alert);
        Assert.Equal(AlertSeverity.Warning, alert.Severity);
    }

    [Fact]
    public void Generate_QuandoValorAcimaComDesvioAbaixoDe20Pct_RetornaWarning()
    {
        // 125 está 4.2% acima de 120 → Warning
        var alert = AlertGenerator.Generate(MakeRecord(125), MakeMetricType(), 90, 120);

        Assert.NotNull(alert);
        Assert.Equal(AlertSeverity.Warning, alert.Severity);
    }

    [Fact]
    public void Generate_QuandoValorAcimaComDesvioAcimaDe20Pct_RetornaCritical()
    {
        // 150 está 25% acima de 120 → Critical
        var alert = AlertGenerator.Generate(MakeRecord(150), MakeMetricType(), 90, 120);

        Assert.NotNull(alert);
        Assert.Equal(AlertSeverity.Critical, alert.Severity);
    }

    [Fact]
    public void Generate_QuandoValorAbaixoComDesvioAcimaDe20Pct_RetornaCritical()
    {
        // 60 está 33% abaixo de 90 → Critical
        var alert = AlertGenerator.Generate(MakeRecord(60), MakeMetricType(), 90, 120);

        Assert.NotNull(alert);
        Assert.Equal(AlertSeverity.Critical, alert.Severity);
    }

    [Fact]
    public void Generate_QuandoComSomenteMinDefinido_ValorAbaixo_RetornaAlerta()
    {
        var alert = AlertGenerator.Generate(MakeRecord(3000), MakeMetricType(), 5000, null);

        Assert.NotNull(alert);
    }

    [Fact]
    public void Generate_MensagemContemNomeEUnidade()
    {
        var alert = AlertGenerator.Generate(MakeRecord(150), MakeMetricType(), 90, 120);

        Assert.NotNull(alert);
        Assert.Contains("Pressão Sistólica", alert.Message);
        Assert.Contains("mmHg", alert.Message);
        Assert.Contains("150", alert.Message);
    }

    // ── Limiar exato de 20% ────────────────────────────────────────────────

    [Theory]
    [InlineData(143, AlertSeverity.Warning)]   // 19.2% acima de 120
    [InlineData(144, AlertSeverity.Critical)]  // 20.0% acima de 120
    [InlineData(145, AlertSeverity.Critical)]  // 20.8% acima de 120
    public void Generate_LimiarDe20Pct_SeveridadeCorreta(decimal value, AlertSeverity expected)
    {
        var alert = AlertGenerator.Generate(MakeRecord(value), MakeMetricType(), 90, 120);

        Assert.NotNull(alert);
        Assert.Equal(expected, alert.Severity);
    }
}
