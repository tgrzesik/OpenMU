﻿// <copyright file="CachingGenericRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

/// <summary>
/// A generic repository which wraps the access to the dbset of the <see cref="EntityDataContext"/>.
/// Entities are getting eagerly (=completely) loaded automatically.
/// </summary>
/// <typeparam name="T">The type which this repository should manage.</typeparam>
internal class CachingGenericRepository<T> : GenericRepositoryBase<T>
    where T : class
{
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingGenericRepository{T}" /> class.
    /// </summary>
    /// <param name="repositoryManager">The repository manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public CachingGenericRepository(RepositoryManager repositoryManager, ILoggerFactory loggerFactory)
        : base(repositoryManager, loggerFactory.CreateLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(CachingGenericRepository<T>)))
    {
        this._loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Gets a context to work with. If no context is currently registered at the repository manager, a new one is getting created.
    /// </summary>
    /// <returns>The context.</returns>
    protected override EntityFrameworkContextBase GetContext()
    {
        var context = this.RepositoryManager.ContextStack.GetCurrentContext() as CachingEntityFrameworkContext;

        return new CachingEntityFrameworkContext(context?.Context ?? new EntityDataContext(), this.RepositoryManager, context is null, this._loggerFactory.CreateLogger<CachingEntityFrameworkContext>());
    }

    /// <inheritdoc/>
    protected override IEnumerable<INavigation> GetNavigations(EntityEntry entityEntry)
    {
        return this.FullEntityType.GetNavigations();
    }
}