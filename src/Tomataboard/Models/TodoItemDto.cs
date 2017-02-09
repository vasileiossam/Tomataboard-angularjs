using System;

namespace Tomataboard.Models
{
    public class TodoItemDto
    {
        public int Category { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public bool Done { get; set; }
    }
}
