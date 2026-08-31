using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Forno.Data;

public sealed class FornoDbContextFactory : IDesignTimeDbContextFactory<FornoDbContext>
{
    public FornoDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<FornoDbContext>()
            .UseSqlite("Data Source=Data/forno.db")
            .Options;

        return new FornoDbContext(options);
    }
}
