using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISaleRepository, SaleRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<ICartRepository, CartRepository>();
        builder.Services.AddScoped<IBranchRepository, BranchRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

        // MongoDB
        var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB");
        var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"] ?? "developer_evaluation_events";
        builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoConnectionString));
        builder.Services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName));
        builder.Services.AddScoped<ISaleEventRepository, SaleEventRepository>();
    }
}