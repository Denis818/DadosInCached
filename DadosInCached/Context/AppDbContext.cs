using DadosInCached.Models;
using Microsoft.EntityFrameworkCore;

namespace DadosInCached.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
    }

}
