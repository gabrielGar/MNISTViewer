namespace MNISTViewer
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.hostCanvas = new System.Windows.Forms.Integration.ElementHost();
            this.textUrl = new System.Windows.Forms.TextBox();
            this.buttonRecognize = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.labelPrediction = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textResponse = new System.Windows.Forms.RichTextBox();
            this.labelVersion = new System.Windows.Forms.Label();
            this.linkUpdates = new System.Windows.Forms.LinkLabel();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.openFileOnnx = new System.Windows.Forms.OpenFileDialog();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // hostCanvas
            // 
            this.hostCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hostCanvas.Location = new System.Drawing.Point(0, 0);
            this.hostCanvas.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.hostCanvas.Name = "hostCanvas";
            this.hostCanvas.Size = new System.Drawing.Size(516, 375);
            this.hostCanvas.TabIndex = 0;
            this.hostCanvas.Child = null;
            // 
            // textUrl
            // 
            this.textUrl.Font = new System.Drawing.Font("Segoe UI Light", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textUrl.Location = new System.Drawing.Point(16, 14);
            this.textUrl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textUrl.Name = "textUrl";
            this.textUrl.Size = new System.Drawing.Size(516, 49);
            this.textUrl.TabIndex = 1;
            this.textUrl.TextChanged += new System.EventHandler(this.textUrl_TextChanged);
            // 
            // buttonRecognize
            // 
            this.buttonRecognize.Location = new System.Drawing.Point(111, 458);
            this.buttonRecognize.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonRecognize.Name = "buttonRecognize";
            this.buttonRecognize.Size = new System.Drawing.Size(105, 52);
            this.buttonRecognize.TabIndex = 3;
            this.buttonRecognize.Text = "Recognize";
            this.buttonRecognize.UseVisualStyleBackColor = true;
            this.buttonRecognize.Click += new System.EventHandler(this.buttonRecognize_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(223, 458);
            this.buttonClear.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(105, 52);
            this.buttonClear.TabIndex = 4;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // labelPrediction
            // 
            this.labelPrediction.Font = new System.Drawing.Font("Segoe UI Light", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPrediction.ForeColor = System.Drawing.Color.Maroon;
            this.labelPrediction.Location = new System.Drawing.Point(14, 512);
            this.labelPrediction.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPrediction.Name = "labelPrediction";
            this.labelPrediction.Size = new System.Drawing.Size(519, 121);
            this.labelPrediction.TabIndex = 5;
            this.labelPrediction.Text = "0";
            this.labelPrediction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.hostCanvas);
            this.panel1.Location = new System.Drawing.Point(16, 69);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(518, 377);
            this.panel1.TabIndex = 6;
            // 
            // textResponse
            // 
            this.textResponse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textResponse.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textResponse.Font = new System.Drawing.Font("Segoe UI Light", 15.75F);
            this.textResponse.Location = new System.Drawing.Point(540, 14);
            this.textResponse.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textResponse.Name = "textResponse";
            this.textResponse.Size = new System.Drawing.Size(703, 712);
            this.textResponse.TabIndex = 7;
            this.textResponse.Text = "";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(12, 708);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(51, 20);
            this.labelVersion.TabIndex = 8;
            this.labelVersion.Text = "label1";
            // 
            // linkUpdates
            // 
            this.linkUpdates.AutoSize = true;
            this.linkUpdates.Location = new System.Drawing.Point(404, 708);
            this.linkUpdates.Name = "linkUpdates";
            this.linkUpdates.Size = new System.Drawing.Size(129, 20);
            this.linkUpdates.TabIndex = 9;
            this.linkUpdates.TabStop = true;
            this.linkUpdates.Text = "Update Available";
            this.linkUpdates.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkUpdates_LinkClicked);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(334, 458);
            this.buttonLoad.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(105, 52);
            this.buttonLoad.TabIndex = 10;
            this.buttonLoad.Text = "Load Model";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // openFileOnnx
            // 
            this.openFileOnnx.FileName = "model.onnx";
            this.openFileOnnx.Filter = "ONNX Models|*.onnx|All Files|*.*";
            this.openFileOnnx.InitialDirectory = ".";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1256, 738);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.linkUpdates);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.textResponse);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelPrediction);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonRecognize);
            this.Controls.Add(this.textUrl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(964, 545);
            this.Name = "Main";
            this.Text = "MNIST Viewer";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost hostCanvas;
        private System.Windows.Forms.TextBox textUrl;
        private System.Windows.Forms.Button buttonRecognize;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Label labelPrediction;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox textResponse;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.LinkLabel linkUpdates;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.OpenFileDialog openFileOnnx;
    }
}

