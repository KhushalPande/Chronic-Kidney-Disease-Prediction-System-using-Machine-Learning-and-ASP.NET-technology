using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKDPrediction.Models
{
    public class PredictionHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public float Age { get; set; }
        public float BloodPressure { get; set; }
        public float SerumCreatinine { get; set; }
        public float Haemoglobin { get; set; }
        public float BloodGlucoseRandom { get; set; }
        public float BloodUrea { get; set; }
        public float Sodium { get; set; }
        public float Potassium { get; set; }
        public bool HasCKD { get; set; }
        public float Probability { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public DateTime PredictedAt { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
    }
}