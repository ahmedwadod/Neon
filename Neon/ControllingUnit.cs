using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Speech.Recognition;
using System.Windows.Forms;
using NKH.MindSqualls;

namespace Neon
{
    /// <summary>
    /// This is Neon Project:
    /// A voice-controlled NXT car.
    /// Project started in October 8th 2015.
    /// Version 2.0.0.0 in October 9th 2015.
    /// Version 2.0.0.3 in October 16th 2015.
    /// Version 3.0.0.0 in October 21st 2015.
    /// Version 3.0.0.1 in November 16th 2015.
    /// Version 3.0.0.2 in December 11th 2015.
    /// 
    /// Second Edition:
    /// Added new voice orders: "Start engine" & "Stop engine" & "Go ahead".
    /// Added engine reanimation motion (Starting and Stopping).
    /// Changed the instructions.
    /// Made Neon more User-Friendly.
    /// Added HD Icon.
    /// 
    /// Revision 2.0.0.3:
    /// Edited Data.
    /// 
    /// Version 3.0.0.0:
    /// Added Battery Level.
    /// Changed the design.
    /// Changed the instructions.
    /// Changed the icon.
    /// Changed StartPosition.
    /// Edited Data.
    /// 
    /// Version 3.0.0.1:
    /// Stopped throwing battery's Exceptions.
    /// 
    /// Version 3.0.0.2:
    /// Added Log system.
    /// Edited Data.
    /// 
    /// Version 3.0.0.3:
    /// Changing port bug fixed.
    /// Edited data.
    /// 
    /// Developer: Ahmed Abd Al-Wadood Abd Allah Al-Khalifa Al-Mobark.
    /// (Ahmed A.Khalifa)
    /// 
    /// October 9th 2015
    /// October 16th 2015.
    /// October 21st 2015.
    /// November 16th 2015.
    /// December 11th 2015.
    /// April 10th 2016.
    /// </summary>
    public partial class ControllingUnit : Form
    {
        #region Controlling_Section
        // Define variables for the SpeechRecognitionEngine

        // Define the orders
        readonly private Choices _neonChoices =
            new Choices(new[]
            {
                "Turn on engine","Start engine", "Turn off engine","Stop engine", "Turn engine on", "Turn engine off", "Go forward", "Go backward",
                "Go back", "Stop", "Turn right", "Turn left", "Don't turn", "Go straight","Go ahead"
            });

        // Define the SpeechRecognitionEngine
        private readonly SpeechRecognitionEngine _defaultEngine = new SpeechRecognitionEngine(new CultureInfo("en-US"));

        // _engine is defined for the status of the engine (On or Off)
        private bool _engine;

        #endregion

        #region Robot_Section
        // Define NXT Brick's needs

        // Define the NXT Brick
        private NxtBrick _nxt;

        // Define NXT Brick's Motors
        private readonly NxtMotor _a = new NxtMotor();
        private readonly NxtMotor _ar = new NxtMotor(true);
        private readonly NxtMotor _c = new NxtMotor();
        private readonly NxtMotor _cr = new NxtMotor(true);

        // Define Motors power
        private sbyte _apower = 60;
        private sbyte _cpower = 60;

        // _isMoving is defined for Changing Direction
        private bool _isMoving;

        // EngineReanimation function is defined for the engine starting
        void EngineReanimation()
        {
            _nxt.MotorA = _ar;
            _nxt.MotorC = _c;

            _nxt.MotorA.Run(100, 5);
            _nxt.MotorC.Run(100, 5);
        }

        #endregion

        #region Log
        // Define Log's needs

        // Main Log path
        private const string Logpath = @"D:\Neon_Log.txt";

        // WriteToLog Function
        void WriteToLog(string txt)
        {
            var fullText = DateTime.Today.ToString(CultureInfo.InvariantCulture) + @" : " + txt;

            if (!File.Exists(Logpath))
            {
                File.Create(Logpath).Close();
            }

            var alreadyText = File.ReadAllText(Logpath);
            File.WriteAllLines(Logpath, new string[] {alreadyText, fullText});
        }

        #endregion

        public ControllingUnit()
        {
            InitializeComponent();

            // Load Grammer and setup SpeechRecognitionEngine
            var neonGrammar = new Grammar(new GrammarBuilder(_neonChoices));
            _defaultEngine.LoadGrammar(neonGrammar);
            _defaultEngine.SpeechRecognized += defaultEngine_SpeechRecognized;
            _defaultEngine.SetInputToDefaultAudioDevice();

            // Writes to the Log
            WriteToLog(@"Neon Started.");
        }

        #region RecognizingOrders
        private void defaultEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "Turn on engine" || e.Result.Text == "Turn engine on" || e.Result.Text == "Start engine")
            {
                // Start recognizing orders
                _engine = true;
                Engine_status.Text = @"ON";

                // Reanimation engine strting
                EngineReanimation();

            }
            else if (e.Result.Text == "Turn off engine" || e.Result.Text == "Turn engine off" || e.Result.Text == "Stop engine")
            {
                // Stop recognizing orders
                _engine = false;
                Engine_status.Text = @"OFF";

                // Reanimation engine stoping
                _nxt.MotorA.Coast();
                _nxt.MotorC.Coast();
            }

            if (_engine)
            {
                switch (e.Result.Text)
                {
                        // Define motion
                    case "Go forward":
                        _nxt.MotorA = _ar;
                        _nxt.MotorC = _cr;

                        _nxt.MotorA.Run(_apower, 0);
                        _nxt.MotorC.Run(_cpower, 0);

                        _isMoving = true;
                        break;
                    case "Go backward":
                        _nxt.MotorA = _a;
                        _nxt.MotorC = _c;

                        _nxt.MotorA.Run(_apower, 0);
                        _nxt.MotorC.Run(_cpower, 0);

                        _isMoving = true;
                        break;
                    case "Go back":
                        _nxt.MotorA = _a;
                        _nxt.MotorC = _c;

                        _nxt.MotorA.Run(_apower, 0);
                        _nxt.MotorC.Run(_cpower, 0);

                        _isMoving = true;
                        break;
                    case "Stop":
                        _nxt.MotorA.Brake();
                        _nxt.MotorC.Brake();

                        _isMoving = false;
                        break;

                        // Define direction
                    case "Turn right":
                        Direction_bar.Value = 2;
                        break;
                    case "Turn left":
                        Direction_bar.Value = 0;
                        break;
                    case "Go straight":
                        Direction_bar.Value = 1;
                        break;
                    case "Go ahead":
                        Direction_bar.Value = 1;
                        break;
                }
            }


        }
        #endregion

        private void Connect_btn_Click(object sender, EventArgs e)
        {
                // Try to connect then check connection and throw error if found
            try
            {
                // Setup the NXT Brick
                _nxt = new NxtBrick(NxtCommLinkType.Bluetooth, Convert.ToByte(Port_txt.Text)) { MotorA = _ar, MotorC = _cr };

                _nxt.Connect();
                if (_nxt.IsConnected)
                {
                    _defaultEngine.RecognizeAsync(RecognizeMode.Multiple);
                    Connected_lbl.Visible = true;
                    Port_txt.BackColor = Color.Green;
                    Connect_btn.Enabled = false;
                    Battery_checker.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Connected_lbl.Visible = false;
                Port_txt.BackColor = Color.Red;

                // Writes the Exception to the Log
                WriteToLog(ex.Message);
            }

        }

        private void Direction_bar_ValueChanged(object sender, EventArgs e)
        {
            // Change direction
            switch (Direction_bar.Value)
            {
                case 0:
                    _apower = 50;
                    _cpower = 100;
                    break;
                case 1:
                    _cpower = 60;
                    _apower = 60;
                    break;
                case 2:
                    _cpower = 50;
                    _apower = 100;
                    break;
            }

            if (_isMoving)
            {
                _nxt.MotorA.Run(_apower, 0);
                _nxt.MotorC.Run(_cpower, 0);
            }
            
        }

        #region HelpAndAbout
        // Show Help & About windows

        // Show about window
        private void button1_Click(object sender, EventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        // Show help window
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var help = new HelpingUnit();
            help.ShowDialog();
        }
        #endregion

        private void Battary_checker_Tick(object sender, EventArgs e)
        {
            // Get battery level
            try
            {
                Battery.Value = _nxt.BatteryLevel;
            }
            catch (Exception ex)
            {
                // Writes the Exception to the Log
                WriteToLog(ex.Message);
            }
        }
    }
}
// Ahmed A.Khalifa