using CKDPrediction.Models;

namespace CKDPrediction.Services
{
    public interface ICKDPredictionService
    {
        CKDPredictionResult Predict(CKDInput input);
    }
}