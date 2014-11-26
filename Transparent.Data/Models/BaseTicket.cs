using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class BaseTicket
    {
        public BaseTicket()
        {

        }

        public BaseTicket(int id, int rank)
        {
            Id = id;
            Rank = rank;
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        // Must be indexed
        public int Rank { get; set; }

        [MaxLength(100)]
        [Required]
        public string Heading { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(10000)]
        [Required]
        public string Body { get; set; }
    }
}
