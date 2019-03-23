using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
namespace Picture_Execute
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // ;
        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        public static bool CompareBitmapsFast(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1 == null || bmp2 == null)
                return false;
            if (object.Equals(bmp1, bmp2))
                return true;
            if (!bmp1.Size.Equals(bmp2.Size) || !bmp1.PixelFormat.Equals(bmp2.PixelFormat))
                return false;

            int bytes = bmp1.Width * bmp1.Height * (Image.GetPixelFormatSize(bmp1.PixelFormat) / 8);

            bool result = true;
            byte[] b1bytes = new byte[bytes];
            byte[] b2bytes = new byte[bytes];

            BitmapData bitmapData1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bitmapData2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, bmp2.PixelFormat);

            Marshal.Copy(bitmapData1.Scan0, b1bytes, 0, bytes);
            Marshal.Copy(bitmapData2.Scan0, b2bytes, 0, bytes);

            for (int n = 0; n <= bytes - 1; n++)
            {
                if (b1bytes[n] != b2bytes[n])
                {
                    result = false;
                    break;
                }
            }

            bmp1.UnlockBits(bitmapData1);
            bmp2.UnlockBits(bitmapData2);

            return result;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        static Bitmap[] arrBmp = new Bitmap[576];
        int k = 0;
        
        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap(pictureBox1.Image);
            int width = bitmap.Width;
            int high = bitmap.Height;

            for (int y = 0; y < bitmap.Height; y += 32)
            {
                for (int x = 0; x < bitmap.Width; x += 32)
                {
                    RectangleF cloneRect = new RectangleF(x, y, 32, 32);
                    PixelFormat format = bitmap.PixelFormat;
                    Bitmap cloneBitmap = bitmap.Clone(cloneRect, format);
                    arrBmp[k++] = cloneBitmap;
                }
            }
            Result1 rs1 = new Result1();

            for (k = 0; k < (bitmap.Height*bitmap.Width)/(32*32); k++)
            {
                PictureBox picture = new PictureBox();
                picture.Image = arrBmp[k];
                picture.SizeMode = PictureBoxSizeMode.AutoSize;
                rs1.flowLayoutPanel1.Controls.Add(picture);
            }
            rs1.Show();
            //MessageBox.Show(rs1.flowLayoutPanel1.Controls.Count.ToString());
        }
        ArrayList arlPictureRef = new ArrayList();
        private void button2_Click(object sender, EventArgs e)
        {        
          
            bool isDuplicate = false;

            foreach (Bitmap bmp in arrBmp)
            {
                Picture_refs pcr = new Picture_refs();
                pcr.bmp_ref = bmp;
                pcr.num_ref = 0;
                for (int i = 0; i < arlPictureRef.Count; i++)
                    if (CompareBitmapsFast((Bitmap)arlPictureRef[i], bmp) == true)
                        isDuplicate = true;
                if (isDuplicate == false)
                    arlPictureRef.Add(bmp);
                else
                    isDuplicate = false;
            }

            Result rs = new Result();
            for (int i = 0; i < arlPictureRef.Count; i++)
            {

                PictureBox picture = new PictureBox();
                picture.Image = (Bitmap)arlPictureRef[i];
                picture.SizeMode = PictureBoxSizeMode.AutoSize;
                rs.flowLayoutPanel1.Controls.Add (picture);          
            }

            rs.Show();
            //arlPictureRef: arrayList của các ảnh Bitmap đã rút gọn, kèm theo ID của nó 

            
            ArrayList arlPictureRef2 = new ArrayList();
            foreach(Bitmap bmp in arrBmp)
            {
             
                Picture_refs pcr = new Picture_refs();
                pcr.bmp_ref = bmp;
               
                for ( int i=0; i<arlPictureRef.Count; i++)
                {
                    Bitmap nBmp = (Bitmap)arlPictureRef[i];
                    int id = i;
                    if (CompareBitmapsFast(bmp,nBmp) == true)
                    {
                        pcr.num_ref = id;
                    }
                                
                } 
                
                arlPictureRef2.Add(pcr);
            }

            using (StreamWriter sw = new StreamWriter("matrixMatrix.txt"))
            {
                Bitmap bitmap = new Bitmap(pictureBox1.Image);
                int c = 0;
                sw.Write(arlPictureRef.Count + " " + bitmap.Height / 32 + " " + bitmap.Width / 32);
                sw.WriteLine();

                for (int i = 0; i < arlPictureRef2.Count; i++)
                {
                    Picture_refs pcr = (Picture_refs)arlPictureRef2[i];
                    sw.Write(pcr.num_ref + " ");
                    c++;
                    if (c %(bitmap.Width/32) ==0)
                        sw.WriteLine();
                }           
            }
            //MessageBox.Show(rs.flowLayoutPanel1.Controls.Count.ToString());
        }


        public void CombineImage(ArrayList arl )
        {
           
            int width = arl.Count*32;
            int height = 32;
            Bitmap newBitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(newBitmap);
            g.Clear(SystemColors.AppWorkspace);
            int nIndex = 0;
            foreach(object obj in arl)
            {
                Bitmap nBM = (Bitmap)obj;
                if (nIndex == 0)
                {
                    g.DrawImage(nBM, new Point(0, 0));
                    nIndex++;
                    width = nBM.Width;
                }
                else
                {
                    g.DrawImage(nBM, new Point(width, 0));
                    width += nBM.Width;
                }
                nBM.Dispose();
            }

            newBitmap.Save("myImage.bmp");
            newBitmap.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
                CombineImage(arlPictureRef);
        }
    }
}
