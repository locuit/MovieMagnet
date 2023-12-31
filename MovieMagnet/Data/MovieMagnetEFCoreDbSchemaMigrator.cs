﻿using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;

namespace MovieMagnet.Data;

public class MovieMagnetEFCoreDbSchemaMigrator : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public MovieMagnetEFCoreDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the MovieMagnetDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<MovieMagnetDbContext>()
            .Database
            .MigrateAsync();
    }
}
