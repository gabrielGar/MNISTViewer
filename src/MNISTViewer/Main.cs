using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MNISTViewer
{
    public partial class Main : Form
    {
        private InkCanvas _inkCanvas;
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
            if (string.IsNullOrEmpty(textUrl.Text) || string.IsNullOrWhiteSpace(textUrl.Text))
            {
                MessageBox.Show("Invalid Scoring Endpoint!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(new { image = string.Join(",", digit) }),
                    Encoding.UTF8,
                    "application/json");

                var response = client.PostAsync(textUrl.Text, content).Result;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Reponse Code: {response.StatusCode}");
                if (response.Content != null)
                {
                    var scoreJson = response.Content.ReadAsStringAsync().Result;
                    var score = JsonConvert.DeserializeObject<Score>(
                        JsonConvert.DeserializeObject(scoreJson).ToString());
                                        
                    sb.AppendLine("Scores:");
                    int i = 0;
                    foreach (var s in score.Scores)
                        sb.AppendLine($"\t{i++}: {s:P}");

                    sb.AppendLine($"Prediction: {score.Prediction}");
                    sb.AppendLine($"Time: {score.Time}");

                    labelPrediction.Text = score.Prediction.ToString();
                }

                textResponse.Text = "";
                textResponse.Text = sb.ToString();
            }
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

    }

    public class Score
    {
        [JsonProperty("prediction")]
        public int Prediction { get; set; }

        [JsonProperty("time")]
        public double Time { get; set; }

        [JsonProperty("scores")]
        public List<double> Scores { get; set; }
    }
}
