using System.ComponentModel.DataAnnotations;

namespace CKDPrediction.Models
{
    public class CKDInput
    {
        [Required]
        [Range(1, 120)]
        public float Age { get; set; }

        [Required]
        [Range(50, 180)]
        [Display(Name = "Blood Pressure (mm/Hg)")]
        public float BloodPressure { get; set; }

        [Required]
        [Display(Name = "Specific Gravity")]
        public float SpecificGravity { get; set; }  // 1.005, 1.01, 1.015, 1.02, 1.025

        [Required]
        [Display(Name = "Albumin (0-5)")]
        public float Albumin { get; set; }

        [Required]
        [Display(Name = "Sugar (0-5)")]
        public float Sugar { get; set; }

        [Required]
        [Display(Name = "Red Blood Cells")]
        public float RedBloodCells { get; set; }  // 0=abnormal, 1=normal

        [Required]
        [Display(Name = "Pus Cell")]
        public float PusCell { get; set; }  // 0=abnormal, 1=normal

        [Required]
        [Display(Name = "Pus Cell Clumps")]
        public float PusCellClumps { get; set; }  // 0=notpresent, 1=present

        [Required]
        [Display(Name = "Bacteria")]
        public float Bacteria { get; set; }  // 0=notpresent, 1=present

        [Required]
        [Display(Name = "Blood Glucose Random (mgs/dl)")]
        public float BloodGlucoseRandom { get; set; }

        [Required]
        [Display(Name = "Blood Urea (mgs/dl)")]
        public float BloodUrea { get; set; }

        [Required]
        [Display(Name = "Serum Creatinine (mgs/dl)")]
        public float SerumCreatinine { get; set; }

        [Required]
        [Display(Name = "Sodium (mEq/L)")]
        public float Sodium { get; set; }

        [Required]
        [Display(Name = "Potassium (mEq/L)")]
        public float Potassium { get; set; }

        [Required]
        [Display(Name = "Haemoglobin (gms)")]
        public float Haemoglobin { get; set; }

        [Required]
        [Display(Name = "Packed Cell Volume")]
        public float PackedCellVolume { get; set; }

        [Required]
        [Display(Name = "White Blood Cell Count (cells/cumm)")]
        public float WhiteBloodCellCount { get; set; }

        [Required]
        [Display(Name = "Red Blood Cell Count (millions/cmm)")]
        public float RedBloodCellCount { get; set; }

        [Required]
        [Display(Name = "Hypertension")]
        public float Hypertension { get; set; }  // 0=no, 1=yes

        [Required]
        [Display(Name = "Diabetes Mellitus")]
        public float DiabetesMellitus { get; set; }  // 0=no, 1=yes

        [Required]
        [Display(Name = "Coronary Artery Disease")]
        public float CoronaryArteryDisease { get; set; }  // 0=no, 1=yes

        [Required]
        [Display(Name = "Appetite")]
        public float Appetite { get; set; }  // 0=poor, 1=good

        [Required]
        [Display(Name = "Pedal Edema")]
        public float PedalEdema { get; set; }  // 0=no, 1=yes

        [Required]
        [Display(Name = "Anaemia")]
        public float Anaemia { get; set; }  // 0=no, 1=yes
    }
}