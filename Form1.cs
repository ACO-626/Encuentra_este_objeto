using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Encuentra_este_objeto
{
    public partial class Form1 : Form
    {
        Image<Bgr, byte> img;

        Dictionary<string, Image<Bgr,byte>> imgList;
        Rectangle rect;
        Point StartROI, EndROI;
        bool Selecting;
        bool MouseDown;

        public Form1()
        {
            InitializeComponent();
            Selecting = false;
            rect = Rectangle.Empty;
            imgList = new Dictionary<string, Image<Bgr, byte>>();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            imgList.Clear();
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                img = new Image<Bgr, byte>(ofd.FileName);
                pictureBox1.Image = img.ToBitmap();
                AddImage(img, "Input");
            }
        }


        private void AddImage(Image<Bgr,byte> img, string keyname)
        {
             if(!imgList.ContainsKey(keyname))
            {
                imgList.Add(keyname, img);
            }
            else
            {
                imgList[keyname] = img;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(Selecting)
            {
                MouseDown = true;
                StartROI = e.Location;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if(Selecting)
            {
                int width = Math.Max(StartROI.X, e.X) - Math.Min(StartROI.X, e.X);
                int Height = Math.Max(StartROI.Y, e.Y) - Math.Min(StartROI.Y, e.Y);
                rect = new Rectangle(Math.Min(StartROI.X,e.X),Math.Min(StartROI.Y,e.Y),width,Height);
                Refresh();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if(MouseDown)
            {
                using (Pen pen = new Pen(Color.Red))
                {
                    e.Graphics.DrawRectangle(pen,rect);
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if(Selecting)
            {
                Selecting = false;
                MouseDown = false;
            }

            ObtenerImagen();
        }


        private void ObtenerImagen()
        {
            try
            {
                if(pictureBox1.Image == null)
                {
                    return;
                }
                if(rect==Rectangle.Empty)
                {
                    return;
                }
                var img = new Image<Bgr, byte>(new Bitmap(pictureBox1.Image));

                img.ROI = rect;

                var imgROI = img.Copy();
                img.ROI = Rectangle.Empty;

                pictureBox1.Image = imgROI.ToBitmap();

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if(pictureBox1.Image==null || rect ==null)
                {
                    return;
                }

                var imgScene = new Image<Bgr, byte>(new Bitmap(pictureBox2.Image));
                var template = new Image<Bgr,byte>( new Bitmap(pictureBox1.Image));

                Mat imgOut = new Mat();

                CvInvoke.MatchTemplate(imgScene,template,imgOut,Emgu.CV.CvEnum.TemplateMatchingType.Sqdiff);
                double minVal = 0.0;
                double maxVal = 0.0;
                Point minLoc = new Point();
                Point maxLoc = new Point();
                CvInvoke.MinMaxLoc(imgOut, ref minVal, ref maxVal, ref minLoc, ref maxLoc);
                Rectangle r = new Rectangle(minLoc, template.Size);
                CvInvoke.Rectangle(imgScene, r, new MCvScalar(255, 0, 0), 3);
                pictureBox1.Image = imgScene.ToBitmap();


            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImportaer2_Click(object sender, EventArgs e)
        {
            //imgList.Clear();
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                img = new Image<Bgr, byte>(ofd.FileName);
                pictureBox2.Image = img.ToBitmap();
                //AddImage(img, "Input");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Selecting = true;
            
        }
    }
}
