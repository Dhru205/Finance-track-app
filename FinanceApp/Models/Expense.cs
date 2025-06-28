
using System.ComponentModel.DataAnnotations;

namespace FinanceApp.Models
{
    public class Expense 
    {
        // Properties

        public int Id { get; set; }
        [Required] // Data Annotations
        public string Description { get; set; } = null!; // Cannot set null
        [Required]
        [Range(1, int.MaxValue, ErrorMessage ="Please enter a valid integer greater than 0")]
        public double Amount { get; set; }
        [Required]
        public string Category { get; set; } = null!;
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
