using Microsoft.ML;
using Microsoft.ML.Data;

// =============================================
// CKD MODEL TRAINER
// Reads kidney_disease.csv, trains FastTree,
// saves ckdModel.zip
// =============================================

var mlContext = new MLContext(seed: 42);

Console.WriteLine("=== CKD Model Trainer ===");
Console.WriteLine("Loading dataset...");

// --- STEP 1: Define raw data schema ---
var dataPath = "C:\\Users\\Vaishnavi Raut\\OneDrive\\Desktop\\WAD\\proj1\\CKDTrainer\\kidney_disease.csv";
if (!File.Exists(dataPath))
{
    Console.WriteLine($"ERROR: {dataPath} not found! Copy it to CKDTrainer folder.");
    return;
}

// Load raw CSV as text (we'll parse manually for flexibility)
var lines = File.ReadAllLines(dataPath).Skip(1).ToList(); // skip header

var records = new List<CKDRawData>();
int skipped = 0;

foreach (var line in lines)
{
    var parts = line.Split(',');
    if (parts.Length < 26) { skipped++; continue; }

    try
    {
        var r = new CKDRawData
        {
            Age = ParseFloat(parts[1], 44),
            Bp = ParseFloat(parts[2], 76),
            Sg = ParseFloat(parts[3], 1.018f),
            Al = ParseFloat(parts[4], 1),
            Su = ParseFloat(parts[5], 0),
            Rbc = EncodeNormal(parts[6]),
            Pc = EncodeNormal(parts[7]),
            Pcc = EncodePresent(parts[8]),
            Ba = EncodePresent(parts[9]),
            Bgr = ParseFloat(parts[10], 148),
            Bu = ParseFloat(parts[11], 53),
            Sc = ParseFloat(parts[12], 3.1f),
            Sod = ParseFloat(parts[13], 137),
            Pot = ParseFloat(parts[14], 4.6f),
            Hemo = ParseFloat(parts[15], 12.5f),
            Pcv = ParseFloat(parts[16], 38),
            Wc = ParseFloat(parts[17], 8400),
            Rc = ParseFloat(parts[18], 4.7f),
            Htn = EncodeYes(parts[19]),
            Dm = EncodeYes(parts[20]),
            Cad = EncodeYes(parts[21]),
            Appet = EncodeGood(parts[22]),
            Pe = EncodeYes(parts[23]),
            Ane = EncodeYes(parts[24]),
            Label = parts[25].Trim().ToLower().StartsWith("ckd") && !parts[25].Trim().ToLower().Equals("notckd")
        };
        records.Add(r);
    }
    catch { skipped++; }
}

Console.WriteLine($"Loaded {records.Count} records. Skipped {skipped}.");
Console.WriteLine($"CKD: {records.Count(r => r.Label)}, Non-CKD: {records.Count(r => !r.Label)}");

// --- STEP 2: Load into IDataView ---
var dataView = mlContext.Data.LoadFromEnumerable(records);

// --- STEP 3: Define pipeline ---
var pipeline = mlContext.Transforms.Concatenate("Features",
    nameof(CKDRawData.Age), nameof(CKDRawData.Bp), nameof(CKDRawData.Sg),
    nameof(CKDRawData.Al), nameof(CKDRawData.Su), nameof(CKDRawData.Rbc),
    nameof(CKDRawData.Pc), nameof(CKDRawData.Pcc), nameof(CKDRawData.Ba),
    nameof(CKDRawData.Bgr), nameof(CKDRawData.Bu), nameof(CKDRawData.Sc),
    nameof(CKDRawData.Sod), nameof(CKDRawData.Pot), nameof(CKDRawData.Hemo),
    nameof(CKDRawData.Pcv), nameof(CKDRawData.Wc), nameof(CKDRawData.Rc),
    nameof(CKDRawData.Htn), nameof(CKDRawData.Dm), nameof(CKDRawData.Cad),
    nameof(CKDRawData.Appet), nameof(CKDRawData.Pe), nameof(CKDRawData.Ane))
    .Append(mlContext.BinaryClassification.Trainers.FastTree(
        labelColumnName: "Label",
        featureColumnName: "Features",
        numberOfLeaves: 50,
        numberOfTrees: 150,
        minimumExampleCountPerLeaf: 5,
        learningRate: 0.2));

// --- STEP 4: Train/test split ---
var split = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

Console.WriteLine("Training model (FastTree)...");
var model = pipeline.Fit(split.TrainSet);

// --- STEP 5: Evaluate ---
var predictions = model.Transform(split.TestSet);
var metrics = mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");

Console.WriteLine($"\n=== Model Evaluation ===");
Console.WriteLine($"Accuracy:  {metrics.Accuracy:P2}");
Console.WriteLine($"AUC:       {metrics.AreaUnderRocCurve:P2}");
Console.WriteLine($"F1 Score:  {metrics.F1Score:P2}");
Console.WriteLine($"Precision: {metrics.PositivePrecision:P2}");
Console.WriteLine($"Recall:    {metrics.PositiveRecall:P2}");

// --- STEP 6: Save model ---
var outputPath = "ckdModel.zip";
mlContext.Model.Save(model, dataView.Schema, outputPath);
Console.WriteLine($"\nModel saved to: {Path.GetFullPath(outputPath)}");
Console.WriteLine("\nCopy ckdModel.zip to: CKDPrediction/MLModel/ckdModel.zip");
Console.WriteLine("\nDone! Press any key to exit.");
Console.ReadKey();

// =============================================
// HELPER FUNCTIONS
// =============================================

static float ParseFloat(string val, float defaultVal)
{
    val = val.Trim().Replace("\t", "");
    return float.TryParse(val, out float result) ? result : defaultVal;
}

static float EncodeNormal(string val)
{
    val = val.Trim().ToLower().Replace("\t", "");
    return val == "normal" ? 1f : 0f;
}

static float EncodePresent(string val)
{
    val = val.Trim().ToLower().Replace("\t", "");
    return val == "present" ? 1f : 0f;
}

static float EncodeYes(string val)
{
    val = val.Trim().ToLower().Replace("\t", "");
    return val == "yes" ? 1f : 0f;
}

static float EncodeGood(string val)
{
    val = val.Trim().ToLower().Replace("\t", "");
    return val == "good" ? 1f : 0f;
}

// =============================================
// DATA CLASSES
// =============================================

public class CKDRawData
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
    public bool Label { get; set; }
}

public class CKDPrediction
{
    [ColumnName("PredictedLabel")]
    public bool PredictedLabel { get; set; }
    public float Probability { get; set; }
    public float Score { get; set; }
}