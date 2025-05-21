using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Bildbearbeitungsprogramm
{
    public partial class Form1 : Form
    {
        Bitmap originalPicture;
        Bitmap backupPicture;
        Bitmap editedPicture;
        public Form1()
        {
            InitializeComponent();
            change_button.Text = "load picture";
            apply_button.Hide(); 
            
            trackBarBrightness.Hide();
            trackBarBrightness.Maximum = 100;
            trackBarBrightness.Minimum = -100;
            trackBarBrightness.Value = 0;
            trackBarBrightness.TickFrequency = 10;
        }

        private void Change_picture(object sender, EventArgs e)                 //Bild wird geladen oder verändert
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Bilder|*.jpg;*.png;*.bmp";                           

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                originalPicture = new Bitmap(ofd.FileName);
                backupPicture = new Bitmap(originalPicture);
                pictureBox1.Image = originalPicture;
                trackBarBrightness.Value = 0;
                             
            }
            change_button.Text = "change picture";                              //nachdem Bild einmal geladen wurde, wird der text zu "change"
        }

        private void Rotate_picture(object sender, EventArgs e)                 //Bild wird gedreht
        {
            if (originalPicture == null)
            {
                MessageBox.Show("Bitte erst ein Bild laden!");                  //Falls kein Bild vorhanden ist -> wird ein Error angezeigt
                return;
            }
            else
            {               
                originalPicture.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox1.Image = originalPicture;              
            }
        }

        private void Flip_picture(object sender, EventArgs e)                   //Bild wird einmal horizontal gekippt (gespiegelt)
        {
            if (originalPicture == null)
            {
                MessageBox.Show("Bitte erst ein Bild laden!");                  //Falls kein Bild vorhanden ist -> wird ein Error angezeigt
                return;
            }
            else
            {
                originalPicture.RotateFlip(RotateFlipType.RotateNoneFlipX);
                pictureBox1.Image = originalPicture;
            }
        }

        private void Grey_colour(object sender, EventArgs e)                    //Bild bekommt Schwarz/weiss stufe
        {
            if (originalPicture == null)
            {
                MessageBox.Show("Bitte erst ein Bild laden!");                  //Falls kein Bild vorhanden ist -> wird ein Error angezeigt
                return;
            }
            Bitmap graustufenBild = new Bitmap(originalPicture.Width, originalPicture.Height);

            for (int x = 0; x < originalPicture.Width; x++)
            {
                for (int y = 0; y < originalPicture.Height; y++)
                {
                    Color originalFarbe = originalPicture.GetPixel(x, y);
                    int grau = (int)(0.3 * originalFarbe.R + 0.59 * originalFarbe.G + 0.11 * originalFarbe.B);
                    Color grauFarbe = Color.FromArgb(grau, grau, grau);
                    graustufenBild.SetPixel(x, y, grauFarbe);
                }
            }

            pictureBox1.Image = graustufenBild;
            originalPicture = graustufenBild;
           
            apply_button.Show();
        }
        
        
        private void Reset_Click(object sender, EventArgs e)                    //Bild wird auf das originalBild durch eine neue Bitmap zurückgesetzt
        {
            if (originalPicture == null)
            {
                MessageBox.Show("Bitte erst ein Bild laden!");                  //Falls kein Bild vorhanden ist -> wird ein Error angezeigt
                return;
            }
            originalPicture = new Bitmap(backupPicture);
            pictureBox1.Image = originalPicture;
        }

        private void Brightness_Click(object sender, EventArgs e)               //wenn der Button "brightness" gedrückt wird -> erscheint regler für Helligkeit
        {
            if (originalPicture == null)
            {
                MessageBox.Show("Bitte erst ein Bild laden!");                  //Falls kein Bild vorhanden ist -> wird ein Error angezeigt
                return;
            }
            trackBarBrightness.Show();;
        }
        private int Clamp(int value, int min, int max)                          //Clamp setzt einen Mindest-/ und Maximalwert fest sodass keine Fehler entstehen (begrenzen/einschränken)                
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private void TrackBar1_brightness(object sender, EventArgs e)           //Regler für die Helligkeit
                            
        {
            if (originalPicture == null)
            {
                MessageBox.Show("Bitte erst ein Bild laden!");                  //Falls kein Bild vorhanden ist -> wird ein Error angezeigt
                return;
            }

            int brightness = trackBarBrightness.Value;
            Bitmap neu = new Bitmap(originalPicture.Width, originalPicture.Height);

            for (int y = 0; y < originalPicture.Height; y++)
            {
                for (int x = 0; x < originalPicture.Width; x++)
                {
                    Color pixel = originalPicture.GetPixel(x, y);

                    int r = Clamp(pixel.R + brightness, 0, 255);
                    int g = Clamp(pixel.G + brightness, 0, 255);
                    int b = Clamp(pixel.B + brightness, 0, 255);

                    neu.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            editedPicture = neu;
            pictureBox1.Image = editedPicture;
            
            apply_button.Show();
        }

        private void apply_button_Click(object sender, EventArgs e)             //speichert den Wert der Helligkeit auf das originalbild ab
        {
            if (originalPicture == null)
            {
                MessageBox.Show("Bitte erst ein Bild laden!");                  //Falls kein Bild vorhanden ist -> wird ein Error angezeigt
                return;
            }
            originalPicture = editedPicture;
            trackBarBrightness.Hide();
            apply_button.Hide();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void save_button_Click(object sender, EventArgs e)
        {
            editedPicture = new Bitmap (pictureBox1.Image);

            // Speicherdialog öffnen
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG-Bild|*.png|JPEG-Bild|*.jpg|Bitmap|*.bmp";
                sfd.Title = "Bild speichern";
                sfd.FileName = "edited_picture.png";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // Bildformat bestimmen
                    System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
                    if (sfd.FilterIndex == 2)
                        format = System.Drawing.Imaging.ImageFormat.Jpeg;
                    else if (sfd.FilterIndex == 3)
                        format = System.Drawing.Imaging.ImageFormat.Bmp;

                    // Bild speichern
                    editedPicture.Save(sfd.FileName, format);
                    MessageBox.Show("Bild erfolgreich gespeichert!");
                }
            }
        }
    }
}
