namespace UserManagmentSystem.Models
{
    public class ExceptionsLog
    {
            public int Id { get; set; }
            public DateTime Timestamp { get; set; }
            public string Message { get; set; }
            public string StackTrace { get; set; }
            public string ExceptionType { get; set; }
        }
}
