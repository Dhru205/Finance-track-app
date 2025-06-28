using FinanceApp.Models;

namespace FinanceApp.Data.Service
{
    public interface IExpensesService
    {
        Task<IEnumerable<Expense>> GetAll();
        Task Add(Expense expense);

        Task Delete(int id); // Added by me

        IQueryable GetChartData();
    }
}
