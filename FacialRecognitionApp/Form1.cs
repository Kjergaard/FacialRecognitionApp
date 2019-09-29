using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace FacialRecognitionApp
{
    public partial class Form1 : Form
    {
        public VideoCapture Webcam { get; set; }
        public EigenFaceRecognizer FaceRecognition { get; set; }
        public CascadeClassifier FaceDetection { get; set; }
        public CascadeClassifier EyeDetection { get; set; }
        public FaceRecognizer.PredictionResult result { get; set; }


        public Mat Frame { get; set; }

        public List<Mat> Faces { get; set; }
        public List<int> IDs { get; set; }
        public List<string> listOfNames { get; set; }
        public List<int> listOfIds { get; set; }

        public int ProcessedImageWidth { get; set; } = 128;
        public int ProcessedImageHeight { get; set; } = 150;

        public int TimerCounter { get; set; } = 0;
        public int TimeLimit { get; set; } = 30;
        public int ScanCounter { get; set; } = 0;

        public string YMLPath { get; set; } = @"../../Algo/TrainingData.yml";

        public Timer Timer { get; set; }

        public bool FaceSquare { get; set; } = false;
        public bool EyeSquare { get; set; } = false;
        public bool doneTraining { get; set; } = false;

        

        public Form1()
        {
            InitializeComponent();

            FaceRecognition = new EigenFaceRecognizer(80, double.PositiveInfinity);
            FaceDetection = new CascadeClassifier(System.IO.Path.GetFullPath(@"../../Algo/haarcascade_frontalface_default.xml"));
            EyeDetection = new CascadeClassifier(System.IO.Path.GetFullPath(@"../../Algo/haarcascade_eye.xml"));
            Frame = new Mat();
            Faces = new List<Mat>();
            IDs = new List<int>();
            listOfNames = new List<string>();
            listOfIds = new List<int>();
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

        public void Webcam_ImageGrabbed(object sender, EventArgs e)
        {
            Webcam.Retrieve(Frame);
            var ImageFrame = Frame.ToImage<Bgr, byte>();

            if (ImageFrame != null)
            {
                var grayFrame = ImageFrame.Convert<Gray, byte>();
                var faces = FaceDetection.DetectMultiScale(grayFrame, 1.3, 5);
                var eyes = EyeDetection.DetectMultiScale(grayFrame, 1.3, 5);


                if (!TrainButton.Enabled)
                    foreach (var face in faces)
                        ImageFrame.Draw(face, new Bgr(Color.LimeGreen), 3);



                if (FaceSquare)
                    foreach (var face in faces)
                    { 
                        ImageFrame.Draw(face, new Bgr(Color.LimeGreen), 3);
                        
                        
                        //TODO draw decicion ID on rectancle   
                        Graphics graphicImage = Graphics.FromImage(ImageFrame.Bitmap);
                        graphicImage.DrawString(result.Label.ToString(),
                                                new Font("Arial", 
                                                12, FontStyle.Bold),
                                                SystemBrushes.WindowText,
                                                new Point(face.X,face.Y));
                    }

                if (EyeSquare)
                    foreach (var eye in eyes)
                        ImageFrame.Draw(eye, new Bgr(Color.Red), 3);
               

                WebcamBox.Image = ImageFrame.ToBitmap();
            }
        }

        public string NamePrediction(string id)
        {
            switch (id)
            {
                case "":
                    return listOfNames[0];
                    break;


            }

            return id.ToString();
        }

        private void EyeButton_Click(object sender, EventArgs e)
        {
            if (EyeSquare)
                EyeButton.Text = "Eye Square: OFF";
            else
                EyeButton.Text = "Eye Square: ON";

            EyeSquare = !EyeSquare;
        }

        private void SquareButton_Click(object sender, EventArgs e)
        {
            if (FaceSquare)
                SquareButton.Text = "Face Square: OFF";
            else
                SquareButton.Text = "Face Square: ON";

            FaceSquare = !FaceSquare;
        }

        public void PredictButton_Click(object sender, EventArgs e)
        {
            Webcam.Retrieve(Frame);
            var imageFrame = Frame.ToImage<Gray, byte>();
            

            if (imageFrame != null)
            {
                var faces = FaceDetection.DetectMultiScale(imageFrame, 1.3, 5);
                
                if (faces.Any())
                {
                    
                    Image<Gray, byte> processedImage = imageFrame.Copy(faces[0]).Resize(ProcessedImageWidth, ProcessedImageHeight, Emgu.CV.CvEnum.Inter.Cubic);
                    result = FaceRecognition.Predict(processedImage);
                    
                    
                    
                    if (result.Label == listOfIds.IndexOf(result.Label)+1)
                    {
                        MessageBox.Show($"This is, " + listOfNames[result.Label-1]);
                        
                    }
                    //TODO - else virker ikke.
                    else
                        MessageBox.Show($"I dont know this person");


                }
                else
                    MessageBox.Show("Face was not found - try again");
            }
        }

        private void TrainButton_Click(object sender, EventArgs e)
        {
            if (IDBox.Text != string.Empty && nameBox.Text != string.Empty)
            {
                IDBox.Enabled = !IDBox.Enabled;
                nameBox.Enabled = !nameBox.Enabled;

                listOfNames.Add(nameBox.Text);
                listOfIds.Add(Convert.ToInt32(IDBox.Text));

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
                    Rectangle[] faces = FaceDetection.DetectMultiScale(imageFrame, 1.3, 5);

                    if (faces.Any())
                    {
                        var processedImage = imageFrame.Copy(faces[0]).Resize(ProcessedImageWidth, ProcessedImageHeight, Emgu.CV.CvEnum.Inter.Cubic);
                        Faces.Add(processedImage.Mat);
                        IDs.Add(Convert.ToInt32(IDBox.Text));
                        ScanCounter++;
                        OutputBox.AppendText($"{ScanCounter} Successful Scans Taken...{Environment.NewLine}");
                        OutputBox.ScrollToCaret();
                    }
                }
            }
            else
            {
                FaceRecognition.Train(new VectorOfMat(Faces.ToArray()), new VectorOfInt(IDs.ToArray()));
                FaceRecognition.Write(YMLPath);
                
                Timer.Stop();
                TimerCounter = 0;
                IDBox.Clear();
                nameBox.Clear();
                TrainButton.Enabled = !TrainButton.Enabled;
                IDBox.Enabled = !IDBox.Enabled;
                nameBox.Enabled = !nameBox.Enabled;
                OutputBox.AppendText($"Training Complete! {Environment.NewLine}");
                MessageBox.Show("Training Complete");
                doneTraining = true;
            }
            
        }
    }
}
