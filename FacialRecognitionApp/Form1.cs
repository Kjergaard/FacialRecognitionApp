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
        private FaceRecognizer.PredictionResult result { get; set; } 


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
        public bool isPredicted { get; set; } = false;
        public bool isUnknown { get; set; } = true;
        public int userId { get; set; } = 0;
        



        public Form1()
        {
            InitializeComponent();

            FaceRecognition = new EigenFaceRecognizer(80, 2500);
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

            Webcam.ImageGrabbed += Webcam_ImageGrabbed; // will call this every time an image is grabbed from the webcam.
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
                var faces = FaceDetection.DetectMultiScale(grayFrame, 1.3, 5); // Et array af firkanter, som holder alle ansigter den finder.
                // var eyes = EyeDetection.DetectMultiScale(grayFrame, 1.3, 5); // Samme med øjne

                foreach (var face in faces)
                {
                    ImageFrame.Draw(face, new Bgr(Color.LimeGreen), 3);

                    if (result.Label != -1 || isPredicted && doneTraining)
                    {
                        try
                        {
                            Graphics graphicImage1 = Graphics.FromImage(ImageFrame.Bitmap);
                            graphicImage1.DrawString(listOfNames[result.Label - 1],
                                new Font("Arial",
                                    15, FontStyle.Bold),
                                new SolidBrush(Color.LimeGreen),
                                new Point(face.X, face.Y));
                        }
                        catch (Exception exception)
                        {
                            //No action as the error is useless, it is simply an error in
                            //no data being there to process and this occurss sporadically
                        }

                    }

                    if (result.Label == 0 || result.Label == -1)
                    {
                        Graphics graphicImage2 = Graphics.FromImage(ImageFrame.Bitmap);
                        graphicImage2.DrawString("Unknown",
                            new Font("Arial",
                                15, FontStyle.Bold),
                            new SolidBrush(Color.Red),
                            new Point(face.X, face.Y));
                    }

                }

                // ID checker
                Graphics graphicImage = Graphics.FromImage(ImageFrame.Bitmap);
                graphicImage.DrawString($"Face ID: " + result.Label.ToString(),
                    new Font("Arial",
                    15, FontStyle.Bold),
                    new SolidBrush(Color.LimeGreen),
                    new Point(0, 50));



                WebcamBox.Image = ImageFrame.ToBitmap(); // shows frames in the UI.
            }
        }

        private void TrainButton_Click(object sender, EventArgs e)
        {
            //IDBox.Text != string.Empty &&
            if (nameBox.Text != string.Empty)
            {
                userId++;
                isPredicted = false;
                isUnknown = true;

                //IDBox.Enabled = !IDBox.Enabled;
                nameBox.Enabled = !nameBox.Enabled;

                listOfNames.Add(nameBox.Text); // Adds Name from UI to the list of user Names
                listOfIds.Add(userId); // Adds the ID from the UI to the list of User ID's

                Timer = new Timer();
                Timer.Interval = 500; // ticks every 0.5 sec
                Timer.Tick += Timer_Tick; // this method gets called every time the timer fires.
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
                        var processedImage = imageFrame.Copy(faces[0]).Resize(ProcessedImageWidth, ProcessedImageHeight, Inter.Cubic); // Will zoom into the rectangle it finds to only see that.
                        Faces.Add(processedImage.Mat);
                        IDs.Add(userId);
                        ScanCounter++;
                        OutputBox.AppendText($"{ScanCounter} Successful Scans Taken...{Environment.NewLine}");
                        OutputBox.ScrollToCaret();
                    }
                }
            }
            else
            {
                FaceRecognition.Train(new VectorOfMat(Faces.ToArray()), new VectorOfInt(IDs.ToArray())); // Here we finally train on face and ID collection we just captures and is written to the pathfile "YMLPath"
                FaceRecognition.Write(YMLPath);

                Timer.Stop();
                TimerCounter = 0;
                
                //IDBox.Clear();
                nameBox.Clear();

                TrainButton.Enabled = !TrainButton.Enabled;
                //IDBox.Enabled = !IDBox.Enabled;
                nameBox.Enabled = !nameBox.Enabled;

                OutputBox.AppendText($"Training Complete! {Environment.NewLine}");
                //MessageBox.Show("Training Complete");

                doneTraining = true;

                Timer = new Timer();
                Timer.Interval = 500; // ticks every 0.5 sec
                Timer.Tick += Timer_Tick1; // this method gets called every time the timer fires.
                Timer.Start();


            }

        }

        private void Timer_Tick1(object sender, EventArgs e)
        {
            Webcam.Retrieve(Frame);
            var imageFrame = Frame.ToImage<Gray, byte>();


            if (imageFrame != null)
            {
                var faces = FaceDetection.DetectMultiScale(imageFrame, 1.3, 5);
                
                if (faces.Count() != 0)
                {

                    Image<Gray, byte> processedImage = imageFrame.Copy(faces[0]).Resize(ProcessedImageWidth, ProcessedImageHeight, Inter.Cubic);
                    result = FaceRecognition.Predict(processedImage);
                    
                    foreach (var id in listOfIds)
                    {
                        if (result.Label == id)
                        {
                            isUnknown = false;
                            isPredicted = true;
                        }
                        else
                            isUnknown = true;
                    }
                    
                }
            }
        }
    }
}
