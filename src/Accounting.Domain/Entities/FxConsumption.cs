using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Accounting.Domain.Entities
{
    [Table("FxConsumptions")]
    public class FxConsumption : BaseEntity
    {
        [Required]
        public int FxTransactionId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal ConsumedAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,6)")]
        public decimal ConsumedRate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ConsumedCost { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(50)]
        public string Company { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Reference { get; set; }

        // Navigation properties
        public virtual FxTransaction FxTransaction { get; set; } = null!;

        public FxConsumption()
        {
            Date = DateTime.UtcNow;
        }
    }
}