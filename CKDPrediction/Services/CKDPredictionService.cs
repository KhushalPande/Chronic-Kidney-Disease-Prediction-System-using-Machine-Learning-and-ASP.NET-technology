using CKDPrediction.Models;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace CKDPrediction.Services
{
    public class CKDPredictionService : ICKDPredictionService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly PredictionEngine<CKDModelInput, CKDModelOutput> _predEngine;

        public CKDPredictionService(IWebHostEnvironment env)
        {
            _mlContext = new MLContext(seed: 42);
            var modelPath = Path.Combine(env.ContentRootPath, "MLModel", "ckdModel.zip");

            if (!File.Exists(modelPath))
                throw new FileNotFoundException($"ML model not found at: {modelPath}. Please train the model first.");

            _model = _mlContext.Model.Load(modelPath, out _);
            _predEngine = _mlContext.Model.CreatePredictionEngine<CKDModelInput, CKDModelOutput>(_model);
        }

        public CKDPredictionResult Predict(CKDInput input)
        {
            var modelInput = new CKDModelInput
            {
                Age = input.Age,
                Bp = input.BloodPressure,
                Sg = input.SpecificGravity,
                Al = input.Albumin,
                Su = input.Sugar,
                Rbc = input.RedBloodCells,
                Pc = input.PusCell,
                Pcc = input.PusCellClumps,
                Ba = input.Bacteria,
                Bgr = input.BloodGlucoseRandom,
                Bu = input.BloodUrea,
                Sc = input.SerumCreatinine,
                Sod = input.Sodium,
                Pot = input.Potassium,
                Hemo = input.Haemoglobin,
                Pcv = input.PackedCellVolume,
                Wc = input.WhiteBloodCellCount,
                Rc = input.RedBloodCellCount,
                Htn = input.Hypertension,
                Dm = input.DiabetesMellitus,
                Cad = input.CoronaryArteryDisease,
                Appet = input.Appetite,
                Pe = input.PedalEdema,
                Ane = input.Anaemia
            };

            var output = _predEngine.Predict(modelInput);

            var result = new CKDPredictionResult
            {
                HasCKD = output.PredictedLabel,
                Probability = output.Probability,
                InputData = input
            };

            // AI Recommendation Engine
            result.Recommendations = GenerateRecommendations(input, output.PredictedLabel, output.Probability);

            return result;
        }

        private List<string> GenerateRecommendations(CKDInput input, bool hasCKD, float probability)
        {
            var recs = new List<string>();

            if (hasCKD || probability > 0.4f)
            {
                recs.Add("🏥 Consult a nephrologist (kidney specialist) immediately.");
                recs.Add("🩸 Schedule comprehensive kidney function blood tests (GFR, creatinine, BUN).");
            }

            if (input.BloodPressure > 90)
                recs.Add("💊 Monitor and control blood pressure — target below 130/80 mmHg.");

            if (input.BloodGlucoseRandom > 200)
                recs.Add("🍬 Control blood sugar levels — high glucose damages kidney filters.");

            if (input.SerumCreatinine > 1.5f)
                recs.Add("⚠️ Elevated creatinine detected — reduce protein intake and stay hydrated.");

            if (input.Haemoglobin < 11)
                recs.Add("🩺 Low haemoglobin — discuss anaemia treatment options with your doctor.");

            if (input.Sodium < 135)
                recs.Add("🧂 Low sodium levels — avoid excessive fluid intake and monitor electrolytes.");

            if (input.DiabetesMellitus == 1)
                recs.Add("💉 Manage diabetes carefully — it is the leading cause of kidney disease.");

            if (input.Hypertension == 1)
                recs.Add("🫀 Hypertension detected — take medications as prescribed and reduce salt intake.");

            recs.Add("💧 Drink 8-10 glasses of water daily unless advised otherwise by your doctor.");
            recs.Add("🥗 Follow a kidney-friendly diet: low sodium, low potassium, low phosphorus.");
            recs.Add("🚭 Avoid smoking and limit alcohol consumption.");
            recs.Add("🏃 Engage in light physical activity such as walking 30 minutes per day.");

            return recs;
        }
    }

    // ML.NET input class (matches trainer)
    public class CKDModelInput
    {
        public float Age { get; set; }
        public float Bp { get; set; }
        public float Sg { get; set; }
        public float Al { get; set; }
        public float Su { get; set; }
        public float Rbc { get; set; }
        public float Pc { get; set; }
        public float Pcc { get; set; }
        public float Ba { get; set; }
        public float Bgr { get; set; }
        public float Bu { get; set; }
        public float Sc { get; set; }
        public float Sod { get; set; }
        public float Pot { get; set; }
        public float Hemo { get; set; }
        public float Pcv { get; set; }
        public float Wc { get; set; }
        public float Rc { get; set; }
        public float Htn { get; set; }
        public float Dm { get; set; }
        public float Cad { get; set; }
        public float Appet { get; set; }
        public float Pe { get; set; }
        public float Ane { get; set; }
    }

    public class CKDModelOutput
    {
        [ColumnName("PredictedLabel")]
        public bool PredictedLabel { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}