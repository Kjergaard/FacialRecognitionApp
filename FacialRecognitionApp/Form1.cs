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
        public VideoCapture Webcam { get; set; } // Our webacm
        public EigenFaceRecognizer FaceRecognition { get; set; } // Used to train and predict
        public CascadeClassifier FaceDetection { get; set; } // Class linked to the trained algorithm data that recognizes faces
        public CascadeClassifier EyeDetection { get; set; } // Class linked to the trained algorithm data that recognizes eyes

        public Mat Frame { get; set; } // the main Frame

        public List<Image<Gray, byte>> Faces { get; set; } // This gonna hold all the images taken of faces so we can predict whos face is who's
        public List<int> IDs { get; set; } // Holds the corresponding ID's for the faces

        public int ProcessedImageWidth { get; set; } = 128; // images scaled down for less stress on the system
        public int ProcessedImageHeight { get; set; } = 150; // images scaled down for less stress on the system

        public int TimerCounter { get; set; } = 0; // This counts the time towards our timeLimit
        public int TimeLimit { get; set; } = 30; // This is our limit set to 30 sec
        public int ScanCounter { get; set; } = 0; // this wil increment the time by 1 every time its though the loop

        public string YMLPath { get; set; } = @"../../Algo/TrainingData.yml"; // This is the place we are gonna store our trained data

        public Timer Timer { get; set; } 

        public bool FaceSquare { get; set; } = false; // Used to toggle the face detection ON and OFF
        public bool EyeSquare { get; set; } = false; // Used to toggle the eye detection ON and OFF



        public Form1()
        {
            InitializeComponent();

            // Initializes the needed variables
            FaceRecognition = new EigenFaceRecognizer(80, double.PositiveInfinity); 
            FaceDetection = new CascadeClassifier(System.IO.Path.GetFullPath(@"../../Algo/haarcascade_frontalface_default.xml")); // Path to our face Algorithym data
            EyeDetection = new CascadeClassifier(System.IO.Path.GetFullPath(@"../../Algo/haarcascade_eye.xml")); // Path to our eye Algorithym data
            Frame = new Mat(); 
            Faces = new List<Image<Gray, byte>>(); 
            IDs = new List<int>(); 
            BeginWebcam(); // Begins Webcam method
        }



        // This is where the webcam and frame logic lies
        #region Webcam
        private void BeginWebcam()
        {
            // Check if Webcam is null
            if (Webcam == null)
            {
                Webcam = new VideoCapture();
            }

            // Event is called everytime a image is grabbed from the webcam
            Webcam.ImageGrabbed += Webcam_ImageGrabbed;
            Webcam.Start();
            OutputBox.AppendText($"Webcam Started...{Environment.NewLine}");
        }

        // Event is called every frame
        private void Webcam_ImageGrabbed(object sender, EventArgs e)
        {
            Webcam.Retrieve(Frame); // After the image is grabbed its retrived in gray
            var imageFrame = Frame.ToImage<Bgr, byte>(); // This converts it from gray Mat to a Color image

            if (imageFrame != null)
            {
                var grayFrame = imageFrame.Convert<Gray, byte>(); // Converted to Gray image due to gray is less info processed in the image
                var faces = FaceDetection.DetectMultiScale(imageFrame, 1.3, 5); // This will detect every face in the gray frame
                var eyes = EyeDetection.DetectMultiScale(imageFrame, 1.3, 5); // This will detect every eye in the gray frame

                if (FaceSquare)                     // If the FaceSquare Button is ON 
                    foreach (var face in faces)     // Draw on every frame a Green rectancle that the DetectMultiScale found
                        imageFrame.Draw(face, new Bgr(Color.LimeGreen), 2);
                

                if (EyeSquare)                      // If the FaceSquare Button is ON 
                    foreach (var eye in eyes)       // Draw on every frame a Red rectancle that the DetectMultiScale found
                        imageFrame.Draw(eye, new Bgr(Color.Red), 2);

                WebcamBox.Image = imageFrame.ToBitmap(); // Converts the output to Bitmap so the PictureBox in the UI form can use it.
            }
        }
        #endregion

        // Toggle ON and OFF for the Face recognize button
        #region FaceRecognition_BTN_Toggle
        private void EyeButton_Click(object sender, EventArgs e)
        {
            if (EyeSquare)
                EyeButton.Text = "Eye Square: Off";
            else
                EyeButton.Text = "Eye Square: On";

            EyeSquare = !EyeSquare;
        }
        #endregion


        // Toggle ON and OFF for the Eye recognize button
        #region EyeRecognition_BTN_Toggle
        private void SquareButton_Click(object sender, EventArgs e)
        {
            if (FaceSquare)
                SquareButton.Text = "Face Square: Off";
            else
                SquareButton.Text = "Face Square: On";

            FaceSquare = !FaceSquare;
        }
        #endregion

        // This logic will take a picture of your face and based on your ID its gonna use the training data and predict your face
        #region Predict_Button

        private void PredictButton_Click(object sender, EventArgs e)
        {
            Webcam.Retrieve(Frame); // Retrive the frame from the webcam
            var imageFrame = Frame.ToImage<Gray, byte>(); // Converts from Mat to Gray Image cause of less processing

            if (imageFrame != null) // If not null do: ("Frame has to have value")
            {
                var faces = FaceDetection.DetectMultiScale(imageFrame, 1.3, 5); // This will detect every face in the gray frame

                if (faces.Count() != 0) // If more than 0 faces are found in the frame
                {
                    var processedImage = imageFrame.Copy(faces[0]).Resize(ProcessedImageWidth, ProcessedImageHeight, Emgu.CV.CvEnum.Inter.Cubic); // Resizes the faces found in the frame tó something more manageble
                    var result = FaceRecognition.Predict(processedImage); // The method .Predict. It will use the trained data to predict who is in the image

                    if (result.Label.ToString() == IDBox.Text) // if the result is equal to the ID you put in before scanning you trained your face
                        MessageBox.Show("Your Name"); // then the program will predict your name
                    else
                        MessageBox.Show("Your Friend"); // Else your friends name
                }
                else
                    MessageBox.Show("Face was not found - try again"); // If no faces was found in the frame write message
            }
            
        }
        #endregion


        // The logic in charge of creating the training for all the images you take. To be used by the EigenFaceRecognizer to predict what person is in the frame
        #region TrainData_Button

        private void TrainButton_Click(object sender, EventArgs e)
        {
            if (IDBox.Text != string.Empty)
            {
                IDBox.Enabled = !IDBox.Enabled; // Little trick to toggle a button ON and OFF

                Timer = new Timer();
                Timer.Interval = 500; // every 0.5 sec it will take a picture
                Timer.Tick += Timer_Tick; // every 0.5 sec this method will be called
                Timer.Start();
                TrainButton.Enabled = !TrainButton.Enabled;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Webcam.Retrieve(Frame); // Retrive the image after grab
            var imageFrame = Frame.ToImage<Gray, byte>(); // converts it from Mat to Gray Image

            if (TimerCounter < TimeLimit) // Check if the timer is less than the TimeLimit
            {
                TimerCounter++; // Increment timeCount

                if (imageFrame != null) // If not null do: ("Frame has to have value")
                {
                    var faces = FaceDetection.DetectMultiScale(imageFrame, 1.3, 5); // This will detect every face in the gray frame

                    if (faces.Count() > 0) // If more than 0 faces are found in the frame
                    {
                        var processedImage = imageFrame.Copy(faces[0]).Resize(ProcessedImageWidth, ProcessedImageHeight, Emgu.CV.CvEnum.Inter.Cubic); // Resizes the faces found in the frame tó something more manageble
                        Faces.Add(processedImage); // adds the processed image to the list of faces
                        IDs.Add(Convert.ToInt32(IDBox.Text)); // adds the ID's of the faces to ID list
                        ScanCounter++;
                        OutputBox.AppendText($"{ScanCounter} Successful Scans Taken...{Environment.NewLine}");
                        OutputBox.ScrollToCaret();
                    }
                }
            }
            else
            {
                FaceRecognition.Train(Faces.ToArray(), IDs.ToArray());
                FaceRecognition.Write(YMLPath); // writes the trained data from the line above to the path defined up in our varibles in the top
                Timer.Stop();
                TimerCounter = 0;
                TrainButton.Enabled = !TrainButton.Enabled;
                IDBox.Enabled = !IDBox.Enabled;
                OutputBox.AppendText($"Training Complete! {Environment.NewLine}");
                MessageBox.Show("Training Complete");
            }
            
        }
        #endregion
    }
}
