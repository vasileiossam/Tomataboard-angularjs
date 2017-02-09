using System.Collections.Generic;

namespace Tomataboard.Models
{
    public class TodoDto
    {
        public int Category { get; set;}
        public string[] Categories { get; set; }
        public List<TodoItemDto> Todos { get; set; }
    }
}
