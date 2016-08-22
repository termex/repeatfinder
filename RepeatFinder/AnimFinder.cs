using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RepeatFinder
{
    public partial class AnimFinder : Form
    {
        int curtext = 0;
        bool quit = false;

        string[] text_anim =
        {
            "Поиск совпадений в тексте |",
            "Поиск совпадений в тексте /",
            "Поиск совпадений в тексте —",
            "Поиск совпадений в тексте \\",
        };

        public void Quit()
        {
            BeginInvoke(new Action(() =>
            {
                quit = true;
                Close();
            }));
        }

        public AnimFinder()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void AnimFinder_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !quit)
                e.Cancel = true;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Text = text_anim[curtext];
            ++curtext;
            curtext %= 4;
        }
    }
}
