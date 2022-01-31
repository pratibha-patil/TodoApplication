using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TodoApplication.Models
{
    public class Todo
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        public bool IsCompleted { get; set; }

        public int UserId { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
        
    }
}
