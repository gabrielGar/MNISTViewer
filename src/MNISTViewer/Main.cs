using Microsoft.ML.OnnxRuntime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Numerics.Tensors;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using NLog;

namespace MNISTViewer
{
    public partial class Main : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
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

            //openFileOnnx.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);

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
            Cursor = Cursors.WaitCursor;
            Score score;
            if (_session == null)
                score = PredictWeb(textUrl.Text, digit);
            else
                score = PredictLocal(digit);


            StringBuilder sb = new StringBuilder();
            sb.AppendLine(score.Status);

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
            Cursor = Cursors.Default;
        }

        private Score PredictWeb(string uri, float[] digit)
        {
            if (string.IsNullOrEmpty(uri) || string.IsNullOrWhiteSpace(uri))
                return new Score { Status = "Invalid Endpoint" };
                    
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(new { image = string.Join(",", digit) }),
                    Encoding.UTF8,
                    "application/json");

                try
                {
                    var response = client.PostAsync(uri, content).Result;
                    if (response.Content != null)
                    {
                        var scoreJson = response.Content.ReadAsStringAsync().Result;
                        var score = JsonConvert.DeserializeObject<Score>(scoreJson);
                        score.Empty = false;
                        score.Status = $"Response Code: {response.StatusCode}";
                        return score;
                    }
                    else
                        return new Score { Status = $"Response Code: {response.StatusCode}" };
                }
                catch(Exception e)
                {
                    logger.Error(e, "PredictWeb Exception");
                    StringBuilder sb = new StringBuilder();
                    do
                    {
                        sb.AppendLine($"{e.GetType().ToString().Split('.').LastOrDefault()}: {e.Message}\n");
                        e = e.InnerException;
                    } while (e != null);

                    return new Score { Status = sb.ToString() };
                }
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

            try
            {
                var prediction = _session.Run(input)
                                         .First()
                                         .AsTensor<float>()
                                         .ToArray();

                return new Score
                {
                    Status = $"Local Mode: {_session}",
                    Empty = false,
                    Prediction = Array.IndexOf(prediction, prediction.Max()),
                    Scores = prediction.Select(i => (double)i).ToList(),
                    Time = (DateTime.Now - now).TotalSeconds
                };
            }
            catch (Exception e)
            {
                logger.Error(e, "PredictLocal Exception");
                StringBuilder sb = new StringBuilder();
                do
                {
                    sb.AppendLine($"Error: {e.GetType()}, {e.Message}");
                    e = e.InnerException;
                } while (e != null);

                return new Score { Status = sb.ToString() };
            }
        }

        private void buttonRecognize_Click(object sender, EventArgs e)
        {
            var digit = GetWrittenDigit(28);
            Predict(digit);
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear(bool clearink = true)
        {
            if(clearink)
                _inkCanvas.Strokes.Clear();
            textResponse.Clear();
            labelPrediction.Text = "";
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (textUrl.Enabled)
            {
                if (openFileOnnx.ShowDialog() == DialogResult.OK &&
                    File.Exists(openFileOnnx.FileName))
                {
                    try
                    {
                        var file = openFileOnnx.FileName;
                        _session = new InferenceSession(file);
                        textUrl.Text = $"Local Mode: {Path.GetFileName(file)}";
                        textUrl.Enabled = false;
                        buttonLoad.Text = "Use Service";
                        Clear(false);
                    }
                    catch (Exception error)
                    {
                        logger.Error(error, "Load Model Exception");
                        StringBuilder sb = new StringBuilder();
                        do
                        {
                            sb.AppendLine($"Error: {error.GetType()}, {error.Message}");
                            error = error.InnerException;
                        } while (error != null);

                        MessageBox.Show("Error", sb.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                textUrl.Text = Properties.Settings.Default.Uri;
                textUrl.Enabled = true;
                _session = null;
                buttonLoad.Text = "Load Model";
                Clear(false);
            }
        }

        private void textUrl_TextChanged(object sender, EventArgs e)
        {
            if (textUrl.Text != Properties.Settings.Default.Uri && 
                textUrl.Text.StartsWith("http"))
            {
                Properties.Settings.Default.Uri = textUrl.Text;
                Properties.Settings.Default.Save();
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
