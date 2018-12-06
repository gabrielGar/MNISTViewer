using Microsoft.ML.OnnxRuntime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Numerics.Tensors;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;

namespace MNISTViewer
{
    public partial class Main : Form
    {
        private InkCanvas _inkCanvas;
        private InferenceSession _session = null;
        public Main()
        {
            InitializeComponent();
            textUrl.Text = Properties.Settings.Default.Uri;
            textUrl.HideSelection = true;
            _inkCanvas = new InkCanvas();
            _inkCanvas.DefaultDrawingAttributes = new DrawingAttributes()
            {
                FitToCurve = true,
                Height = 20,
                Width = 20
            };
            hostCanvas.Child = _inkCanvas;
            labelPrediction.Text = "";
            labelVersion.Text = $"Version {Application.ProductVersion}";
            linkUpdates.Visible = false;

            // begin update check
            new Thread(CheckUpdate).Start();
        }

        private async void CheckUpdate()
        {
            var available = await Updater.CheckUpdates();
            if (linkUpdates.InvokeRequired)
                linkUpdates.Invoke((Action)(() => linkUpdates.Visible = available));
            else
                linkUpdates.Visible = available;
        }

        private async void linkUpdates_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            var restart = await Updater.ApplyUpdates();
            if (restart)
            {
                Cursor = Cursors.Default;
                Updater.Restart();
            }

            Cursor = Cursors.Default;
        }

        private float[] GetWrittenDigit(int size)
        {
            RenderTargetBitmap b = new RenderTargetBitmap(
                (int)_inkCanvas.ActualWidth, (int)_inkCanvas.ActualHeight,
                96d, 96d, PixelFormats.Default);

            b.Render(_inkCanvas);
            var bitmap = new WriteableBitmap(b)
                            .Resize(size, size, WriteableBitmapExtensions.Interpolation.NearestNeighbor);

            float[] data = new float[size * size];
            for (int x = 0; x < bitmap.PixelWidth; x++)
            {
                for (int y = 0; y < bitmap.PixelHeight; y++)
                {
                    var color = bitmap.GetPixel(x, y);
                    data[y * bitmap.PixelWidth + x] = 255 - ((color.R + color.G + color.B) / 3);
                }
            }

            return data;
        }

        private void Predict(float[] digit)
        {
            Score score;
            if (_session == null)
                score = PredictWeb(textUrl.Text, digit);
            else
                score = PredictLocal(digit);


            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Reponse Code: {score.Status}");

            if (!score.Empty)
            {
                
                sb.AppendLine("Scores:");
                int i = 0;
                foreach (var s in score.Scores)
                    sb.AppendLine($"\t{i++}: {s:P}");

                sb.AppendLine($"Prediction: {score.Prediction}");
                sb.AppendLine($"Time: {score.Time}");
                labelPrediction.Text = score.Prediction.ToString();
                
            }
            else
            {
                labelPrediction.Text = "";
            }

            textResponse.Text = "";
            textResponse.Text = sb.ToString();
        }

        private Score PredictWeb(string uri, float[] digit)
        {
            if (string.IsNullOrEmpty(uri) || string.IsNullOrWhiteSpace(uri))
            {
                MessageBox.Show("Invalid Scoring Endpoint!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new Score { Status = "Invalid Endpoint" };
            }
                    
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(new { image = string.Join(",", digit) }),
                    Encoding.UTF8,
                    "application/json");

                var response = client.PostAsync(uri, content).Result;
                
                if (response.Content != null)
                {
                    var scoreJson = response.Content.ReadAsStringAsync().Result;
                    var score = JsonConvert.DeserializeObject<Score>(scoreJson);
                    score.Empty = false;
                    score.Status = response.StatusCode.ToString();
                    return score;
                }
                else
                    return new Score { Status = response.StatusCode.ToString() };
            }
        }

        private Score PredictLocal(float[] digit)
        {
            var now = DateTime.Now;
            Tensor<float> x = new DenseTensor<float>(digit.Length);

            // normalize
            for (int i = 0; i < digit.Length; i++) x[i] = digit[i] / 255;

            var input = new List<NamedOnnxValue>() {
                NamedOnnxValue.CreateFromTensor("0", x)
            };

            var prediction = _session.Run(input)
                                     .First()
                                     .AsTensor<float>()
                                     .ToArray();
            return new Score
            {
                Status = "OK",
                Empty = false,
                Prediction = Array.IndexOf(prediction, prediction.Max()),
                Scores = prediction.Select(i => (double)i).ToList(),
                Time = (DateTime.Now - now).TotalSeconds
            };
        }

        private void buttonRecognize_Click(object sender, EventArgs e)
        {
            var digit = GetWrittenDigit(28);
            Predict(digit);
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            _inkCanvas.Strokes.Clear();
            textResponse.Clear();
            labelPrediction.Text = "";
        }

        private void textUrl_TextChanged(object sender, EventArgs e)
        {
            if (textUrl.Text != Properties.Settings.Default.Uri)
            {
                Properties.Settings.Default.Uri = textUrl.Text;
                Properties.Settings.Default.Save();
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if(openFileOnnx.ShowDialog() == DialogResult.OK &&
                File.Exists(openFileOnnx.FileName))
            {
                var file = openFileOnnx.FileName;
                var options = SessionOptions.Default;
                options.AppendExecutionProvider(ExecutionProvider.Cpu);
                _session = new InferenceSession(file, options);
                textUrl.Enabled = false;
            }
        }

        private void textUrl_DoubleClick(object sender, EventArgs e)
        {
            if(!textUrl.Enabled && _session != null)
            {
                textUrl.Enabled = true;
                _session = null;
            }
        }
    }

    public class Score
    {
        [JsonProperty("prediction")]
        public int Prediction { get; set; }

        [JsonProperty("time")]
        public double Time { get; set; }

        [JsonProperty("scores")]
        public List<double> Scores { get; set; }

        public string Status { get; set; }

        public bool Empty { get; set; } = true;
    }
}
