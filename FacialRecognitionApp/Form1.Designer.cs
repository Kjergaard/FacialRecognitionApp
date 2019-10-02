namespace FacialRecognitionApp
{
    partial class Form1
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
            this.WebcamBox = new System.Windows.Forms.PictureBox();
            this.IDBox = new System.Windows.Forms.TextBox();
            this.TrainButton = new System.Windows.Forms.Button();
            this.OutputBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_ID = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SquareButton = new System.Windows.Forms.Button();
            this.EyeButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.nameBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.WebcamBox)).BeginInit();
            this.SuspendLayout();
            // 
            // WebcamBox
            // 
            this.WebcamBox.Location = new System.Drawing.Point(12, 31);
            this.WebcamBox.Name = "WebcamBox";
            this.WebcamBox.Size = new System.Drawing.Size(674, 493);
            this.WebcamBox.TabIndex = 0;
            this.WebcamBox.TabStop = false;
            // 
            // IDBox
            // 
            this.IDBox.Location = new System.Drawing.Point(710, 46);
            this.IDBox.Name = "IDBox";
            this.IDBox.Size = new System.Drawing.Size(222, 20);
            this.IDBox.TabIndex = 1;
            // 
            // TrainButton
            // 
            this.TrainButton.Location = new System.Drawing.Point(710, 126);
            this.TrainButton.Name = "TrainButton";
            this.TrainButton.Size = new System.Drawing.Size(222, 34);
            this.TrainButton.TabIndex = 2;
            this.TrainButton.Text = "Begin Training";
            this.TrainButton.UseVisualStyleBackColor = true;
            this.TrainButton.Click += new System.EventHandler(this.TrainButton_Click);
            // 
            // OutputBox
            // 
            this.OutputBox.Location = new System.Drawing.Point(710, 229);
            this.OutputBox.Name = "OutputBox";
            this.OutputBox.Size = new System.Drawing.Size(222, 254);
            this.OutputBox.TabIndex = 3;
            this.OutputBox.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Webcam";
            // 
            // lbl_ID
            // 
            this.lbl_ID.AutoSize = true;
            this.lbl_ID.Location = new System.Drawing.Point(710, 30);
            this.lbl_ID.Name = "lbl_ID";
            this.lbl_ID.Size = new System.Drawing.Size(71, 13);
            this.lbl_ID.TabIndex = 5;
            this.lbl_ID.Text = "Enter Your ID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(707, 213);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Output";
            // 
            // SquareButton
            // 
            this.SquareButton.Location = new System.Drawing.Point(710, 489);
            this.SquareButton.Name = "SquareButton";
            this.SquareButton.Size = new System.Drawing.Size(107, 33);
            this.SquareButton.TabIndex = 7;
            this.SquareButton.Text = "Face Squares OFF";
            this.SquareButton.UseVisualStyleBackColor = true;
            this.SquareButton.Click += new System.EventHandler(this.SquareButton_Click);
            // 
            // EyeButton
            // 
            this.EyeButton.Location = new System.Drawing.Point(825, 489);
            this.EyeButton.Name = "EyeButton";
            this.EyeButton.Size = new System.Drawing.Size(107, 33);
            this.EyeButton.TabIndex = 8;
            this.EyeButton.Text = "Eye Squares OFF";
            this.EyeButton.UseVisualStyleBackColor = true;
            this.EyeButton.Click += new System.EventHandler(this.EyeButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(710, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Enter Your Name";
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(710, 85);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(222, 20);
            this.nameBox.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 536);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.EyeButton);
            this.Controls.Add(this.SquareButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbl_ID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OutputBox);
            this.Controls.Add(this.TrainButton);
            this.Controls.Add(this.IDBox);
            this.Controls.Add(this.WebcamBox);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.WebcamBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox WebcamBox;
        private System.Windows.Forms.TextBox IDBox;
        private System.Windows.Forms.Button TrainButton;
        private System.Windows.Forms.RichTextBox OutputBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_ID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button SquareButton;
        private System.Windows.Forms.Button EyeButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nameBox;
    }
}

