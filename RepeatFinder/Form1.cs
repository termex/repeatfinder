using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RepeatFinder
{
    public partial class MainForm : Form
    {
        Loader loader;
        string Python;
        string WorkDir;

        public MainForm()
        {

           loader = new Loader();
           loader.ShowDialog();

           InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Python = loader.Python;
            WorkDir = loader.WorkDir;
            if (loader.Exit) Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Word Documents|*.docx";

            if(ofd.ShowDialog()==DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
            }
        }

        string wd;
        string temp_doc;
        string n_temp_doc = "~temp~doc~";

        private void LaunchScript()
        {
            try
            {
                wd = Path.GetDirectoryName(textBox1.Text);
                string bat = Path.Combine(wd, "launchscript.bat");
                string percent = textBox2.Text + "%";
                string minlength = textBox3.Text + "#";
                temp_doc = Path.Combine(wd, n_temp_doc);

                File.Copy(textBox1.Text, temp_doc, true);
                File.Copy(Path.Combine(WorkDir, "repeatfinder.py"), Path.Combine(wd, "repeatfinder.py"), true);

                StreamWriter sw = new StreamWriter(bat);
                sw.WriteLine("\"" + Python + "\" repeatfinder.py " + "\"" + n_temp_doc + "\" " + 
                    percent + " " + minlength + " > log.txt");
                sw.Close();

                Process proc = new Process();
                proc.StartInfo.FileName = bat;
                proc.StartInfo.WorkingDirectory = wd;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.EnableRaisingEvents = true;
                proc.Exited += Proc_Exited;
                proc.Start();
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnableButtons()
        {
            BeginInvoke(new Action(() =>
            {
                button1.Enabled = true;
                button2.Enabled = true;
            }));

        }

        private void Proc_Exited(object sender, EventArgs e)
        {           
            try
            {
                anim.Quit();
            }
            catch { }

            MessageBox.Show("Процесс поиска завершён. Вся информация сохранена в файлы full_match.txt(полные совпадения) и в part_match.txt(частичные совпадения)", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Process.Start(wd);
            EnableButtons();

            try
            {
                string temp_doc = Path.Combine(wd, n_temp_doc);
                if (File.Exists(temp_doc))
                    File.Delete(temp_doc);
            }
            catch { }
        }

        AnimFinder anim;

        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox1.Text))
            {
                button1.Enabled = false;
                button2.Enabled = false;

                anim = new AnimFinder();
                anim.Show();

                LaunchScript();
            }
        }
    }
}
