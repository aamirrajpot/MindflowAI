﻿using Volo.Abp.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace MindflowAI.Data;

public class MindflowAIDbSchemaMigrator : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public MindflowAIDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        
        /* We intentionally resolving the MindflowAIDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<MindflowAIDbContext>()
            .Database
            .MigrateAsync();

    }
}
