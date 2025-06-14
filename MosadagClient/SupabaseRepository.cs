using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Supabase.Postgrest.Interfaces; 
using Supabase.Postgrest.Models;

using static Supabase.Postgrest.Constants;

public class SupabaseRepository<T> where T : BaseModel, IBaseModel, new()
{
    private readonly Supabase.Client _client;

    public SupabaseRepository(Supabase.Client client)
    {
        _client = client;
    }

    private IPostgrestTable<T> Table => _client.From<T>();

    public async Task<T?> GetByIdAsync(long id)
    {
        return await Table
            .Where(x => x.Id == id)
            .Single();
    }

    public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate)
    {
        return await Table
            .Where(predicate)
            .Single();
    }

    public async Task<IEnumerable<T>?> GetAllAsync()
    {

        return (await Table.Get()).Models;

    }

    public async Task<IEnumerable<T>?> FindAsync(Expression<Func<T, bool>> predicate)
    {

        return (await Table
            .Where(predicate)
            .Get())?.Models;


    }

    public async Task<PaginatedResult<T>?> GetPaginatedAsync(PaginationOptions options)
    {
        var query = Table;

        // Apply filters
        foreach (var filter in options.Filters)
        {
            query = query.Filter(filter.Key, Operator.Equals, filter.Value);
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(options.SortBy))
        {
            query = options.SortDescending
                ? query.Order(options.SortBy, Ordering.Descending)
                : query.Order(options.SortBy, Ordering.Ascending);
        }

        // Get count
        var countTask = Table.Count(CountType.Exact);

        // Apply pagination
        query = query
            .Range((options.PageNumber - 1) * options.PageSize, options.PageNumber * options.PageSize - 1);

        var itemsTask = query.Get();

        await Task.WhenAll(countTask, itemsTask);

        return new PaginatedResult<T>
        {
            Items = itemsTask.Result.Models,
            TotalCount = countTask.Result,
            PageNumber = options.PageNumber,
            PageSize = options.PageSize,
            SortBy = options.SortBy,
            SortDescending = options.SortDescending,
            PaginationOptions = options
        };

    }

    public async Task<long> CountAsync(Expression<Func<T, bool>> predicate = null)
    {

        var query = Table;
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        return await query.Count(CountType.Exact);

    }

    public async Task<T> AddAsync(T entity)
    {
        var response = await Table.Insert(entity);
        return response.Models.First();

    }

    public async Task<T> UpdateAsync(T entity)
    {

        var response = await Table.Update(entity, new Supabase.Postgrest.QueryOptions() { Returning = Supabase.Postgrest.QueryOptions.ReturnType.Representation });
        return response.Models.First();

    }

    public async Task<bool> DeleteAsync(long id)
    {

        await Table.Where(x => x.Id == id).Delete();
        return true;

    }

    public async Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate)
    {

        await Table.Where(predicate).Delete();
        return true;

    }
}