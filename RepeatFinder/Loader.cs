using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net;

namespace RepeatFinder
{
    public partial class Loader : Form
    {
        public bool Exit { set; get; }
        public string Python { set; get; }
        public string WorkDir { set; get; }

        public Loader()
        {
            InitializeComponent();
            Exit = true;
            Python = @"C:\Python27\python.exe";
            WorkDir = Environment.CurrentDirectory;
        }

        private void Print(string text, Color color)
        {
            Application.DoEvents();
            BeginInvoke(new Action(() =>
            {
                outPut.SelectionColor = color;
                outPut.AppendText(text + "\r\n");
                outPut.ScrollToCaret();
            }));
        }

        private bool CheckPython()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = Python;

            try
            {
                proc.Start();
                proc.Kill();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SetVisibleButs(bool visible)
        {
            Application.DoEvents();
            BeginInvoke(new Action(() =>
            {
                groupBox1.Enabled = visible;
            }));
        }

        private void GenerateTestScript()
        {
            StreamWriter sw = new StreamWriter(Path.Combine(WorkDir, "test.py"));
            sw.WriteLine(@"from docx import Document");
            sw.Close();
        }

        private bool CheckDocx()
        {
            try
            {
                GenerateTestScript();
                Process proc = new Process();
                proc.StartInfo.FileName = Python;
                proc.StartInfo.WorkingDirectory = WorkDir;
                proc.StartInfo.Arguments = "test.py";
                proc.Start();
                proc.WaitForExit();
                return proc.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
        //http://www.lfd.uci.edu/~gohlke/pythonlibs/zpcorkgj/lxml-3.6.4-cp27-cp27m-win32.whl
        private string GenerateLxmlBat()
        {
           string lxmlwhl = "lxml-3.6.4-cp27-cp27m-win32.whl";
           string bat = Path.Combine(WorkDir, "lxmlinstall.bat");
           string pydir = Path.GetDirectoryName(Python);
           StreamWriter sw = new StreamWriter(bat);
           sw.WriteLine("\"" + Path.Combine(pydir, "Scripts", "pip2.exe") + "\" install " + "\"" + lxmlwhl + "\" > installs_lxml.txt");
           //sw.WriteLine("pause");
           sw.Close();
           return bat;           
        }


        private string GenerateDocxBat()
        {
            string bat = Path.Combine(WorkDir, "docxinstall.bat");
            string pydir = Path.GetDirectoryName(Python);
            StreamWriter sw = new StreamWriter(bat);
            sw.WriteLine("\"" + Path.Combine(pydir, "Scripts", "pip2.exe") + "\"" + " install python-docx > installs_docx.txt");
            //sw.WriteLine("pause");
            sw.Close();
            return bat;
        }

        private bool InstallDocx()
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = GenerateLxmlBat();
                proc.Start();
                proc.WaitForExit();

                proc.StartInfo.FileName = GenerateDocxBat();
                proc.Start();
                proc.WaitForExit();

                return true;
            }
            catch(Exception exc)
            {
                Print(exc.Message, Color.Red);
                return false;
            }
        }

        private void Quit()
        {
            BeginInvoke(new Action(() =>
            {
                Close();
            }));
        }

        private void LoadPython()
        {
            try
            {
                Print("Загрузка пути к Python", Color.Green);
                string pypath = Path.Combine(WorkDir, "pypath");
                if (File.Exists(pypath))
                {
                    StreamReader sr = new StreamReader(pypath);
                    Python = sr.ReadLine();
                    sr.Close();
                }
                
                Print("Проверка Python", Color.Green);

                if (!CheckPython())
                {
                    Print("В вашей системе не обнаружен Python...", Color.Red);
                    SetVisibleButs(true);
                }
                else
                {
                    Print("Проверка библиотеки docx", Color.Green);

                    if (!CheckDocx())
                    {
                        Print("У вас не установлена библиотека docx, установка...", Color.Green);
                        Application.DoEvents();
                        if (InstallDocx())
                        {
                            if (CheckDocx())
                            {
                                Print("Не смог установить библиотеку docx", Color.Green);
                                Exit = false;
                                Application.DoEvents();
                                Thread.Sleep(1000);
                                Quit();
                            }
                            else
                            {
                                Print("Не смог установить библиотеку docx", Color.Red);
                            }
                        }
                    }
                    else
                    {
                        Exit = false;
                        Quit();
                    }
                }
            }
            catch(Exception exc)
            {
                Print(exc.ToString(), Color.Red);
            }
        }

        private void Loader_Load(object sender, EventArgs e)
        {
            Thread thr = new Thread(LoadPython);
            thr.Start();
        }

        private void butExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void WritePath(string path = @"C:\Python27\Python.exe")
        {
            Python = path;
            StreamWriter sw = new StreamWriter(Path.Combine(WorkDir, "pypath"));
            sw.WriteLine(Python);
            sw.Close();
        }

        private void LoadPythonFromWeb()
        {
            try
            {
                WebClient Client = new WebClient();
                Client.DownloadFile("https://www.python.org/ftp/python/2.7.12/python-2.7.12.msi",
                   Path.Combine(WorkDir, "Python.msi"));
            }
            catch (Exception exc)
            {
                Print(exc.Message, Color.Red);
                SetVisibleButs(true);
            }

            try
            {
                string python_msi = Path.Combine(WorkDir, "Python.msi");

                Process proc = new Process();
                proc.StartInfo.FileName = python_msi;
                proc.Start();
                proc.WaitForExit();
                WritePath();

                if (File.Exists(python_msi))
                    File.Delete(python_msi);

                LoadPython();

            }
            catch (Exception exc)
            {
                Print(exc.Message, Color.Red);
                SetVisibleButs(true);
            }
        }

        private void butLoadPython_Click(object sender, EventArgs e)
        {
            SetVisibleButs(false);
            Print("Произвожу загрузку Python, ждите...", Color.Green);
            Application.DoEvents();
            Thread thr = new Thread(LoadPythonFromWeb);
            thr.Start();
        }

        private void butPathPython_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Executable files|*.exe";

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                WritePath(ofd.FileName);
                LoadPython();
            }
        }
    }
}
