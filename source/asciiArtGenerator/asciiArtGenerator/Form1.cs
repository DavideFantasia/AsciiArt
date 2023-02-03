using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Efundies;

namespace asciiArtGenerator
{
    public partial class Form1 : Form
    {
        //grey scale of character from the darkest to the brightest
        public const string charScale = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\" ^`. ";
        //getting by the expressione charScale.lenght / 256
        public double scaleFactor = 0.2705882353;
        public int contrastTrashold = 25;
        //file size scaler
        //needs to be scalable based on the size of file
        public int scale = 3;

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog.FileName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                textBox1.Text = "please choose a photo first. Click Browse to search a photo";
            }else{
                Bitmap img = new Bitmap((Bitmap)pictureBox1.Image);
                SetContrast(img,contrastTrashold);
                if(img.Width > 200 || img.Height > 200)
                {
                    img = new Bitmap(img, new Size(img.Width / scale, img.Height / scale));
                }
                string asciiArt = processImage(img);
                textBox1.Font = new Font(textBox1.Font.FontFamily, 2.5f );
                textBox1.Text = asciiArt;
            }
        }

        public int rangeMap(int x)
        {
            //mapping the hue from [0,255] to the range of [0,charScale.lenght-1]
            return (int)((double)x * scaleFactor);
        }

        //using bitlocks to better performance
        public string processImage(Bitmap bmp)
        {
            string asciiCanvas = "";
            var lockedBitmap = new LockBitmap(bmp);
            lockedBitmap.LockBits();
            //reading all the pixel of the image and switching the corrisponding HUE with the character from the gradient
            for (int y=0; y< lockedBitmap.Height; y++)
            {
                for(int x=0; x< lockedBitmap.Width; x++)
                {
                    Color pixel = lockedBitmap.GetPixel(x, y);
                    int hue = (byte)(.299*pixel.R + .587*pixel.G + .114*pixel.B);
                    //concat of the new char from the char gradient
                    asciiCanvas += charScale[rangeMap(hue)];
                }
                //going to the new line
                asciiCanvas += Environment.NewLine;
            }
            lockedBitmap.UnlockBits();
            return asciiCanvas;
        }
        private static void SetContrast(Bitmap bmp, int threshold)
        {
            var lockedBitmap = new LockBitmap(bmp);
            lockedBitmap.LockBits();

            var contrast = Math.Pow((100.0 + threshold) / 100.0, 2);

            for (int y = 0; y < lockedBitmap.Height; y++)
            {
                for (int x = 0; x < lockedBitmap.Width; x++)
                {
                    var oldColor = lockedBitmap.GetPixel(x, y);
                    var red = ((((oldColor.R / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                    var green = ((((oldColor.G / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                    var blue = ((((oldColor.B / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                    if (red > 255)
                        red = 255;
                    if (red < 0)
                        red = 0;
                    if (green > 255)
                        green = 255;
                    if (green < 0)
                        green = 0;
                    if (blue > 255)
                        blue = 255;
                    if (blue < 0)
                        blue = 0;

                    var newColor = Color.FromArgb(oldColor.A, (int)red, (int)green, (int)blue);
                    lockedBitmap.SetPixel(x, y, newColor);
                }
            }
            lockedBitmap.UnlockBits();
        }
    }
}
