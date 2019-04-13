using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;

namespace FacialRecognitionApp
{
    public partial class Form1 : Form
    {
        public VideoCapture Webcam { get; set; }
        public EigenFaceRecognizer FaceRecognition { get; set; }
        public CascadeClassifier FaceDetection { get; set; }
        public CascadeClassifier EyeDetection { get; set; }

        public Mat Frame { get; set; }

        public List<Image<Gray, byte>> Faces { get; set; }
        public List<int> IDs { get; set; }

        public int ProcessedImageWidth { get; set; } = 128;
        public int ProcessedImageHeight { get; set; } = 150;

        public int TimerCounter { get; set; } = 0;
        public int TimeLimit { get; set; } = 30;
        public int ScanCounter { get; set; } = 0;

        public string YMLPath { get; set; } = @"../../Algo/TrainingData.yml";

        public Timer Timer { get; set; }

        public bool FaceSquare { get; set; } = false;
        public bool EyeSquare { get; set; } = false;

        public Form1()
        {
            InitializeComponent();

            FaceRecognition = new EigenFaceRecognizer(80, double.PositiveInfinity);
            FaceDetection = new CascadeClassifier(System.IO.Path.GetFullPath(@"../../Algo/haarcascade_frontalface_default.xml"));
            EyeDetection = new CascadeClassifier(System.IO.Path.GetFullPath(@"../../Algo/haarcascade_eye.xml"));
            Frame = new Mat();
            Faces = new List<Image<Gray, byte>>();
            IDs = new List<int>();
            BeginWebcam();
        }

        private void BeginWebcam()
        {
            if (Webcam == null)
            {
                Webcam = new VideoCapture();
            }

            Webcam.ImageGrabbed += Webcam_ImageGrabbed;
            Webcam.Start();
            OutputBox.AppendText($"Webcam Started...{Environment.NewLine}");
        }

        private void Webcam_ImageGrabbed(object sender, EventArgs e)
        {
            Webcam.Retrieve(Frame);
            var ImageFrame = Frame.ToImage<Bgr, byte>();

            if (ImageFrame != null)
            {
                var grayFrame = ImageFrame.Convert<Gray, byte>();
                var faces = FaceDetection.DetectMultiScale(grayFrame, 1.3, 5);
                var eyes = EyeDetection.DetectMultiScale(grayFrame, 1.3, 5);

                if (FaceSquare)
                    foreach (var face in faces)
                        ImageFrame.Draw(face, new Bgr(Color.BurlyWood), 3);
                

                if (EyeSquare)
                    foreach (var eye in eyes)
                        ImageFrame.Draw(eye, new Bgr(Color.Yellow), 3);

                WebcamBox.Image = ImageFrame.ToBitmap();
            }
        }

        private void EyeButton_Click(object sender, EventArgs e)
        {
            if (EyeSquare)
                EyeButton.Text = "Eye Square: Off";
            else
                EyeButton.Text = "Eye Square: On";

            FaceSquare = !FaceSquare;
        }

        private void SquareButton_Click(object sender, EventArgs e)
        {
            if (FaceSquare)
                SquareButton.Text = "Face Square: Off";
            else
                SquareButton.Text = "Face Square: On";

            FaceSquare = !FaceSquare;
        }

        private void PredictButton_Click(object sender, EventArgs e)
        {
            Webcam.Retrieve(Frame);
            var imageFrame = Frame.ToImage<Gray, byte>();

            if (imageFrame != null)
            {
                var faces = FaceDetection.DetectMultiScale(imageFrame, 1.3, 5);

                if (faces.Count() != 0)
                {
                    var processedImage = imageFrame.Copy(faces[0]).Resize(ProcessedImageWidth, ProcessedImageHeight,
                        Emgu.CV.CvEnum.Inter.Cubic);
                    var result = FaceRecognition.Predict(processedImage);

                    if (result.Label.ToString() == "15")
                        MessageBox.Show("Your Name");
                    else
                        MessageBox.Show("Your Friend");
                }
                else
                    MessageBox.Show("Face was not found - try again");
            }
            
        }

        private void TrainButton_Click(object sender, EventArgs e)
        {
            if (IDBox.Text != string.Empty)
            {
                IDBox.Enabled = !IDBox.Enabled;

                Timer = new Timer();
                Timer.Interval = 500;
                Timer.Tick += Timer_Tick;
                Timer.Start();
                TrainButton.Enabled = !TrainButton.Enabled;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Webcam.Retrieve(Frame);
            var imageFrame = Frame.ToImage<Gray, byte>();

            if (TimerCounter < TimeLimit)
            {
                TimerCounter++;

                if (imageFrame != null)
                {
                    var faces = FaceDetection.DetectMultiScale(imageFrame, 1.3, 5);

                    if (faces.Count() > 0)
                    {
                        var processedImage = imageFrame.Copy(faces[0]).Resize(ProcessedImageWidth, ProcessedImageHeight, Emgu.CV.CvEnum.Inter.Cubic);
                        Faces.Add(processedImage);
                        IDs.Add(Convert.ToInt32(IDBox.Text));
                        ScanCounter++;
                        OutputBox.AppendText($"{ScanCounter} Successful Scans Taken...{Environment.NewLine}");
                        OutputBox.ScrollToCaret();
                    }
                }
            }
            else
            {
                FaceRecognition.Train(Faces.ToArray(), IDs.ToArray());
                FaceRecognition.Write(YMLPath);
                Timer.Stop();
                TimerCounter = 0;
                TrainButton.Enabled = !TrainButton.Enabled;
                IDBox.Enabled = !IDBox.Enabled;
                OutputBox.AppendText($"Training Complete! {Environment.NewLine}");
                MessageBox.Show("Training Complete");
            }
            
        }
    }
}
