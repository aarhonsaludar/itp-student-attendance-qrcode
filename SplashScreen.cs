using System;
using System.Drawing;
using System.Windows.Forms;
using System.Media;

namespace ITP104_FINAL_PROJECT
{
    public partial class SplashScreen : Form
    {
        private int progressValue = 0;

        public SplashScreen()
        {
            InitializeComponent();
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            PlayGif();
            PlayStartupSound();
            guna2ProgressBar1.Value = 0;
            splashTimer.Start();
        }

        private void PlayGif()
        {
            // Reload GIF from resources to restart animation
            pictureBoxLoading.Image = null;
            pictureBoxLoading.Image = Properties.Resources.loading_gif;
        }
        private void PlayStartupSound()
        {
            try
            {
                SoundPlayer player = new SoundPlayer(Properties.Resources.startup);
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error playing sound: " + ex.Message);
            }
        }

        private void splashTimer_Tick(object sender, EventArgs e)
        {
            progressValue += 1;
            guna2ProgressBar1.Value = progressValue;

            // Restart animation every time GIF finishes its natural cycle
            if (progressValue % 40 == 0)  // Adjust value depending on GIF duration
            {
                PlayGif();
            }

            if (progressValue >= 100)
            {
                splashTimer.Stop();

                this.Hide();
                LoginScreen loginForm = new LoginScreen();
                loginForm.Show();
                loginForm.FormClosed += (s, args) => this.Close();
            }
        }
    }
}
