public class Expense
{
    public int Id {get; set;}
    public required string ProjectId { get; set; }

    public int BudgetLineId { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateTime ExpenseDate { get; set; }

    public int RecordedByUserId { get; set; }
}