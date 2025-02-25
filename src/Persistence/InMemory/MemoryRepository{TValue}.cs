﻿// <copyright file="MemoryRepository{TValue}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

using System.Collections;

/// <summary>
/// A repository which lives on memory only.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public class MemoryRepository<TValue> : IRepository<TValue>, IMemoryRepository
    where TValue : class
{
    private readonly IDictionary<Guid, TValue> _values = new Dictionary<Guid, TValue>();

    /// <summary>
    /// Adds an item to the repository.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="obj">The item.</param>
    public void Add(Guid key, TValue obj)
    {
        this._values.Add(key, obj);
    }

    /// <inheritdoc />
    public void Add(Guid key, object obj)
    {
        if (obj is TValue value)
        {
            this._values[key] = value;
        }
        else
        {
            throw new ArgumentException($"Given object is not of type {typeof(TValue)}", nameof(obj));
        }
    }

    /// <inheritdoc />
    public async ValueTask RemoveAsync(Guid key)
    {
        await this.DeleteAsync(key).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask<TValue?> GetByIdAsync(Guid id)
    {
        this._values.TryGetValue(id, out var obj);
        return ValueTask.FromResult(obj);
    }

    /// <inheritdoc/>
    async ValueTask<object?> IRepository.GetByIdAsync(Guid id)
    {
        return await this.GetByIdAsync(id).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask<bool> DeleteAsync(object obj)
    {
        var key = this._values.Where(kvp => kvp.Value.Equals(obj)).Select(kvp => kvp.Key).FirstOrDefault();
        return ValueTask.FromResult(this._values.Remove(key));
    }

    /// <inheritdoc/>
    public ValueTask<bool> DeleteAsync(Guid id)
    {
        return ValueTask.FromResult(this._values.Remove(id));
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable<TValue>> GetAllAsync()
    {
        return ValueTask.FromResult(this._values.Values.Cast<TValue>());
    }

    /// <inheritdoc/>
    ValueTask<IEnumerable> IRepository.GetAllAsync()
    {
        return ValueTask.FromResult<IEnumerable>(this._values.Values);
    }
}