using Core.Application.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _entities;

    public Repository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _entities = context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        var query = _entities.AsQueryable();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.ToListAsync();
    }


    public async Task<T> GetByIdAsync(Guid id)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return await _entities.FindAsync(id);
#pragma warning restore CS8603 // Possible null reference return.
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _entities.Where(predicate).ToListAsync();
    }

    public async Task<bool> CreateAsync(T entity)
    {
        try
        {
            _entities.Add(entity);
            int affectedRows = await _context.SaveChangesAsync();

            // Check if any rows were affected during the save operation
            return affectedRows > 0;
        }
        catch (DbUpdateException ex)
        {
            // Log the exception or handle it accordingly
            Console.Error.WriteLine($"Error saving changes to the database: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Handle other exceptions, log them, and return false
            Console.Error.WriteLine($"An unexpected error occurred: {ex.Message}");
            return false;
        }
    }


    public async Task<bool> UpdateAsync(T entity)
    {
        try
        {
            _context.Entry(entity).State = EntityState.Modified;
            int affectedRows = await _context.SaveChangesAsync();

            // Check if any rows were affected during the save operation
            return affectedRows > 0;
        }
        catch (DbUpdateException ex)
        {
            // Log the exception or handle it accordingly
            Console.Error.WriteLine($"Error saving changes to the database: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Handle other exceptions, log them, and return false
            Console.Error.WriteLine($"An unexpected error occurred: {ex.Message}");
            return false;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _entities.FindAsync(id);
        if (entity != null)
        {
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

}