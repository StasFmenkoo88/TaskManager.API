namespace TaskManager.API.DTOs
{
    public class TaskFilterDto
    {
        public string? Priority { get; set; }
        public string? Title { get; set; }

        public string? SortBy { get; set; }

        public bool Descending { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public bool? IsCompleted { get; set; }
    }
}
