using FFS.Application.Data;
using FFS.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FFS.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFfsApplication(this IServiceCollection services)
    {
        services.AddSingleton<IFinancialDataProvider, InMemoryFinancialDataProvider>();
        services.AddSingleton<ReportingService>();
        services.AddSingleton<TransactionQueryService>();
        services.AddSingleton<BudgetService>();
        services.AddSingleton<GoalService>();
        services.AddSingleton<SimpleProjectionService>();
        services.AddSingleton<DashboardService>();
        services.AddSingleton<CsvExportService>();
        return services;
    }
}
