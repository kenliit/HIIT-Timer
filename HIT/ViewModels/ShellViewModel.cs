using Caliburn.Micro;
using HIT.Helper;
using HIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace HIT.ViewModels
{
    public class ShellViewModel : Screen
    {
        private ISettingsModel _settings;
        public ShellViewModel(ISettingsModel settings)
        {
            _settings = settings;
            ReadConfig();
            TimerMessage = "Start";
        }

        private void ReadConfig()
        {
            try
            {
                _settings.ReadConfig();
            }
            catch (Exception)
            {
                try
                {
                    _settings.DeleteConfig();
                    _settings.SaveConfig();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SaveConfig()
        {
            try
            {
                _settings.SaveConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Timer Control

        private bool isTimerRunning = false;
        DispatcherTimer timer;

        public void RunTimer()
        {
            if (isTimerRunning)
            {
                timer.Stop();
                timer = null;
                isTimerRunning = false;
                TimerMessage = "Start";
                NotifyOfPropertyChange(() => ShowSettings);
                NotifyOfPropertyChange(() => ShowAgainText);
            }
            else
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;
                timer.Start();
                isTimerRunning = true;
                NotifyOfPropertyChange(() => ShowSettings);
                NotifyOfPropertyChange(() => ShowAgainText);

                countdown = 3;
                mode = 1;
                round = 0;
            }
        }

        private int countdown = 3;  // pre-countdown period 3 seconds, countdown period from settings.
        private int mode = 1;    // 1: pre-countdown period.  2: time on period  3: time off period  4: break period
        private int round = 0;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (countdown < 3 && countdown > 0)
            {
                SystemBeep.ShortBeeps();
                //System.Media.SystemSounds.Beep.Play();  // Just another sound you can pick.
            }

            switch (mode)
            {
                case 1:
                    switch (countdown)
                    {
                        case 3:
                            TimerMessage = "Ready?";
                            break;
                        case 2:
                            TimerMessage = "Get Set";
                            break;
                        case 1:
                            TimerMessage = "Go!";
                            break;
                        default:
                            break;
                    }
                    ProgressPercent = Convert.ToInt32(countdown * 100 / 3);
                    if (countdown-- == 0)
                    {
                        SystemBeep.LongBeeps();
                        countdown = _settings.TimeOn;
                        mode = 2;
                    }
                    break;
                case 2:
                    ProgressPercent = Convert.ToInt32(countdown *100 / _settings.TimeOn);
                    TimerMessage = $"ROUND {round + 1}{Environment.NewLine}Time On{Environment.NewLine}{countdown}";
                    if (countdown-- == 0)
                    {
                        SystemBeep.LongBeeps();
                        if (round == _settings.Rounds - 1)
                        {
                            countdown = _settings.Break;
                            mode = 4;
                        }
                        else
                        {
                            countdown = _settings.TimeOff;
                            mode = 3;
                        }
                    }
                    break;
                case 3:
                    ProgressPercent = Convert.ToInt32(countdown * 100 / _settings.TimeOff);
                    TimerMessage = $"ROUND {round + 1}{Environment.NewLine}Time Off{Environment.NewLine}{countdown}";
                    if (countdown-- == 0)
                    {
                        SystemBeep.LongBeeps();
                        countdown = _settings.TimeOn;
                        round++;
                        mode = 2;
                    }
                    break;
                case 4:
                    ProgressPercent = Convert.ToInt32(countdown * 100 / _settings.Break);
                    TimerMessage = $"REST{Environment.NewLine}{countdown}";
                    if (countdown-- == 0)
                    {
                        SystemBeep.LongBeeps();
                        TimerMessage = "Try again!";
                        countdown = 3;
                        round = 0;
                        mode = 1;
                    }
                    break;
                default:
                    break;
            }
        }


        private int _progressPercent;

        public int ProgressPercent
        {
            get { return _progressPercent; }
            set 
            {
                _progressPercent = value;
                NotifyOfPropertyChange(() => ProgressPercent);
            }
        }


        /// <summary>
        /// The message displayed on the main button
        /// </summary>
        private string _timerMessage;
        public string TimerMessage
        {
            get { return _timerMessage; }
            set
            {
                _timerMessage = value;
                NotifyOfPropertyChange(() => TimerMessage);
            }
        }

        #endregion

        public bool ShowSettings
        {
            get { return !isTimerRunning; }
        }

        public bool ShowAgainText
        {
            get { return isTimerRunning; }
        }

        public ISettingsModel Settings
        {
            get { return _settings; }
            set 
            {
                _settings = value;
                NotifyOfPropertyChange(() => Settings);
            }
        }

        int newVal = 0;
        public void TextUpdated(object sender)
        {
            if (sender == null)
            {
                return;
            }

            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;

            if (tb.Text == "") return;

            if (int.TryParse(tb.Text, out newVal))
            {
                if (newVal <= 0)
                {
                    MessageBox.Show("Hey, we need an positive integer over there.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Hey, we need an integer over there.");
            }
        }

        public void SaveData()
        {
            if (newVal > 0 && _allowSave) // one of the settings has been changed
            {
                _settings.SaveConfig();
                newVal = 0;
            }
        }

        private bool _allowSave = true;

        public bool AllowSave
        {
            get { return _allowSave; }
            set 
            {
                _allowSave = value;
                NotifyOfPropertyChange(() => AllowSave);
            }
        }



        /// <summary>
        /// Drag and move the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == null || e == null)
            {
                return;
            }

            Window myWin = (Window)sender;

            if (e.ChangedButton == MouseButton.Left)
            {
                e.Handled = true;
                myWin.DragMove();
            }
        }

        /// <summary>
        /// Close this App
        /// </summary>
        public void Close()
        {
            Application.Current.Shutdown();
        }
    }
}
