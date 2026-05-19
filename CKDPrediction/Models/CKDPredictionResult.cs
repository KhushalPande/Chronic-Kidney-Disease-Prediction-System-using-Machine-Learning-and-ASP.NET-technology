namespace CKDPrediction.Models
{
    public class CKDPredictionResult
    {
        public bool HasCKD { get; set; }
        public float Probability { get; set; }
        public float Confidence => HasCKD ? Probability * 100 : (1 - Probability) * 100;
        public string RiskLevel => Probability > 0.75f ? "High" : Probability > 0.45f ? "Medium" : "Low";
        public string RiskColor => RiskLevel == "High" ? "#ef4444" : RiskLevel == "Medium" ? "#f59e0b" : "#10b981";
        public List<string> Recommendations { get; set; } = new();
        public CKDInput? InputData { get; set; }
    }
}