﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ExerciseAssistantApplication.Common;
using ExerciseAssistantApplication.Modell;
using System.Windows.Controls;
using System.Windows;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ExerciseAssistantApplication.ViewModell
{
    public class MenuViewModel : ViewModelBase
    {
        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private Process process;
        private IntPtr unityHWND = IntPtr.Zero;

        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);



        private string visibility;
        public RelayCommand Start { get; set; }
        public RelayCommand MyExercise { get; set; }
        public RelayCommand NewExercise { get; set; }
        public MenuViewModel(bool isAdmin)
        {
            this.Start = new RelayCommand(StartClick,StartCancel);
            this.MyExercise = new RelayCommand(MyExerciseClick, MyExerciseCancel);
            this.NewExercise = new RelayCommand(NewExerciseClick, NewExerciseCancel);
            if (isAdmin)
            {
                //NewExercise button visible
                Visibility = "Visible";
            }
            else
            {
                //NewExercise button hide
                Visibility = "Hidden";
            }

        }

        public string Visibility
        {
            get { return visibility; }
            set { visibility = value; }
        }
        public void StartClick()
        {
            //StartViewModel auvm = new StartViewModel();
            //ViewService.ShowDialog(auvm);

            try
            {
                process = new Process();
                process.StartInfo.FileName = "Skeleton3D.exe";
                
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                process.WaitForInputIdle();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ".\nCheck if Container.exe is placed next to UnityGame.exe.");
            }


        }
        public bool StartCancel()
        {
            return true;
        }
        public void MyExerciseClick()
        {
            //SearchViewModel svm = new SearchViewModel();
            //ViewService.ShowDialog(svm);
        }
        public bool MyExerciseCancel()
        {
            return true;
        }
        public void NewExerciseClick()
        {
            //NewExerciseViewModel nevm = new NewExerciseViewModel();
            //ViewService.ShowDialog(nevm);

            ReferenceDataCollection.MainWindow m = new ReferenceDataCollection.MainWindow();
            m.Show();
        }
        public bool NewExerciseCancel()
        {
            return true;
        }

    }
}
