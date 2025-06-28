using FinanceApp.Data;
using FinanceApp.Data.Service;
using FinanceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering; // Added for SelectList
using System.Text; 


namespace FinanceApp.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly IExpensesService _expensesService;  // _expensesService is an instance of IExpensesService, which is used to interact with the expenses data.
        public ExpensesController(IExpensesService expensesService)
        {
            _expensesService = expensesService;
        }

    public async Task<IActionResult> Index(string selectedMonth)
    {
        var expenses = await _expensesService.GetAll();

        // Build dropdown values: list of months with expenses
        var months = expenses
            .Select(e => new DateTime(e.Date.Year, e.Date.Month, 1))
            .Distinct()
            .OrderByDescending(d => d)
            .ToList();

        ViewBag.Months = months.Select(m => new SelectListItem
        {
            Value = m.ToString("yyyy-MM"),
            Text = m.ToString("MMMM yyyy"),
            Selected = selectedMonth == m.ToString("yyyy-MM")
        }).ToList();

        // Determine which month to filter
        DateTime selectedDate = string.IsNullOrEmpty(selectedMonth)
            ? DateTime.Now
            : DateTime.ParseExact(selectedMonth, "yyyy-MM", null);

        // Filter expenses by selected month
        var filteredExpenses = expenses
            .Where(e => e.Date.Month == selectedDate.Month && e.Date.Year == selectedDate.Year)
            .ToList();

        ViewBag.MonthlyTotal = filteredExpenses.Sum(e => e.Amount);
        ViewBag.SelectedMonth = selectedDate.ToString("MMMM yyyy");

        return View(filteredExpenses);
    }

        public async Task<IActionResult> ExportToCsv(string selectedMonth)
        {
            var expenses = await _expensesService.GetAll();

            if (!string.IsNullOrEmpty(selectedMonth) && DateTime.TryParse($"{selectedMonth}-01", out var parsedDate))
            {
                expenses = expenses
                    .Where(e => e.Date.Month == parsedDate.Month && e.Date.Year == parsedDate.Year)
                    .ToList();
            }

            var csv = new StringBuilder();
            csv.AppendLine("Description,Amount,Category,Date");

            double total = 0;

            foreach (var e in expenses)
            {
                csv.AppendLine($"\"{e.Description}\",{e.Amount},\"{e.Category}\",{e.Date:yyyy-MM-dd}");
                total += e.Amount;
            }

            csv.AppendLine();
            csv.AppendLine($"Total Expenses:,{total}");

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"expenses_{selectedMonth ?? "all"}.csv");
        }



        public IActionResult Create()
        {

            var existingCategories = _expensesService
                .GetAll()
                .Result
                .Select(e => e.Category)
                .Distinct()
                .ToList();

            ViewBag.Categories = new SelectList(existingCategories);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Expense expense)
        {
            if (ModelState.IsValid)
            {
                await _expensesService.Add(expense);
                return RedirectToAction("Index");
            }

            // Re-populate dropdown when form is re-displayed
            var expenses = await _expensesService.GetAll();
            var categories = expenses
                .Select(e => e.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .ToList();

            ViewBag.Categories = new SelectList(categories);

            return View(expense);
        }




        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _expensesService.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetChart(int month, int year)
        {
            var data = _expensesService.GetAll()
                .Result
                .Where(e => e.Date.Month == month && e.Date.Year == year)
                .GroupBy(e => e.Category)
                .Select(g => new {
                    category = g.Key,
                    total = g.Sum(e => e.Amount)
                });

            return Json(data);
        }

    }
}
