using FinanceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data
{
    public class FinanceAppContext : DbContext
    {
        public FinanceAppContext(DbContextOptions<FinanceAppContext> options):base(options) { } // Constructor to pass options, base(options) passes options to the parent DbContext class



        public DbSet<Expense> Expenses { get; set; } // DbSet for Expenses, this will create a table in the database named Expenses
    }
}
