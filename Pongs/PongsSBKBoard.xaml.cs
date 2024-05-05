using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using NLog;
using PongsSBK;


namespace Pongs
{
/// <summary>
/// This is pongs game that I developed during my spring break on April 2024
/// </summary>
    public partial class PongsGameSBK : Window
    {
        #region Private variables

        private Line Boundary;
        private SBKPongEngine sbkGameEngine; 
        private Logger log = LogManager.GetLogger("");
        private Settings settings;
        private SliderInfo sliderInfo;

        System.Windows.RoutedEventArgs a;

        #endregion

        #region Public variables
        
        public Ellipse Ball;
        public Rectangle paddle1, paddle2;
        public Rectangle TopWall, BottomWall;
       
        #endregion

        #region Public members
        public PongsGameSBK()
        {
            log.Info("PongsGameSBK Start");

            InitializeComponent();

            Board.Focus();

            sbkGameEngine = new SBKPongEngine(this);
            sliderInfo = new SliderInfo(sbkGameEngine);

            log.Info("Game Start");

            Thread RunBall = new Thread(Runner);
            RunBall.Start();

            Thread RunPaddle = new Thread(PRunner);
            RunPaddle.Start();

            WhoWon_.Visibility = Visibility.Hidden;
            RestartText.Visibility = Visibility.Hidden;

            InitBoard();
            ReDraw();

            log.Info("PongsGameSBK End");
        }
        public void DrawPaddle(Rectangle Paddle, double x, double y)
        {
            log.Info("DrawPaddle Start");
            Paddle.Width = 1.5 * SliderInfo.PaddleSize;
            Paddle.Height = 7.8 * SliderInfo.PaddleSize;
            Paddle.Fill = Brushes.Black;
            Paddle.Stroke = Brushes.White;
            Paddle.StrokeThickness = 4;

            Canvas.SetTop(Paddle, y);
            Canvas.SetLeft(Paddle, x);

            Board.Children.Remove(Paddle);
            Board.Children.Add(Paddle);
            log.Info("DrawPaddle End");
        }
        public void ReDraw()
        {
            log.Info("ReDraw Start");
            if (WindowState == WindowState.Maximized)
            {
                Window.Height = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
                Window.Width = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            }

            Board.Width = Window.Width;
            Board.Height = Window.Height;

            sbkGameEngine.y1 = Board.Height / 2 - (paddle1.Height / 2);
            sbkGameEngine.y2 = sbkGameEngine.y1;
            sbkGameEngine.x2 = Board.Width - 32;
            sbkGameEngine.x1 = 0;

            Ball.Width = 1.5 * SliderInfo.BallSize;
            Ball.Height = 1.5 * SliderInfo.BallSize;
            Ball.Fill = Brushes.White;
            Ball.Stroke = Brushes.White;
            Ball.StrokeThickness = 4;
            Canvas.SetTop(Ball, Board.Height / 2 - Ball.Height / 2);
            Canvas.SetLeft(Ball, Board.Width / 2 - Ball.Width / 2);

            Menu.Width = Board.Width;
            if (Board.Width - (SettingsMenu.Width + About.Width + Help.Width) - (1.5 * RestartButton.Width + PauseButton.Width) - 2 > 0)
            {
                Spacer.Width = Board.Width - (SettingsMenu.Width + About.Width + Help.Width) - (1.5 * RestartButton.Width + PauseButton.Width) - 2;
            }

            Board.Children.Remove(Ball);
            Board.Children.Add(Ball);

            P1Scoreboard.Text = "" + sbkGameEngine.P1Score;
            P2Scoreboard.Text = "" + sbkGameEngine.P2Score;

            sbkGameEngine.CanBallMove = false;
            ReDrawUnmoving();
            log.Info("ReDraw End");
        }
        public void PressedKeys()
        {
            log.Info("PressedKeys Start");
            DrawPaddle(paddle1, sbkGameEngine.x1, sbkGameEngine.y1);
            DrawPaddle(paddle2, sbkGameEngine.x2, sbkGameEngine.y2);
            log.Info("PressedKeys End");
        }
        public void BallMovement()
        {
            log.Info("BallMovement Start");
            if (sbkGameEngine.CanBallMove == true && sbkGameEngine.GamePlayable == true)
            {
                Canvas.SetTop(Ball, Canvas.GetTop(Ball) + sbkGameEngine.VMovement);
                Canvas.SetLeft(Ball, Canvas.GetLeft(Ball) + sbkGameEngine.HMovement);
            }
            if (sbkGameEngine.P1Wins == true)
            {
                WhoWon_.Text = "Player 1 Wins!";
                WhoWon_.Visibility = Visibility.Visible;
                RestartText.Visibility = Visibility.Visible;
                OnPause(Ball, a);
                sbkGameEngine.i = 2;
            }
            if (sbkGameEngine.P2Wins == true)
            {
                WhoWon_.Text = "Player 2 Wins!";
                WhoWon_.Visibility = Visibility.Visible;
                RestartText.Visibility = Visibility.Visible;
                OnPause(Ball, a);
                sbkGameEngine.i = 2;
            }
            log.Info("BallMovement End");
        }

        #endregion

        #region Private members
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            log.Info("OnKeyDown Start");
            sbkGameEngine.OnKeyDown(sender, e);
            log.Info("OnKeyDown End");
        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            log.Info("OnKeyUp Start");
            sbkGameEngine.OnKeyUp(sender, e);
            log.Info("OnKeyUp End");
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            log.Info("Window_Closing Start");
            sbkGameEngine.Start = false;
            log.Info("Game End");
            App.Current.Shutdown();
            log.Info("Window_Closing End");
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            log.Info("OnClick Start");

            settings = new Settings(sliderInfo);

            settings.Show();
            sbkGameEngine.CanBallMove = false;
            sbkGameEngine.GamePlayable = false;
            log.Info("OnClick End");
        }
        private void OnPause(object sender, RoutedEventArgs e)
        {
            log.Info("OnPause Start");
            if (sbkGameEngine.i == 0)
            {
                sbkGameEngine.CanBallMove = false;
                sbkGameEngine.i++;
                PauseButton.Header = "►";
            }
            else if (sbkGameEngine.i == 1)
            {
                sbkGameEngine.CanBallMove = true;
                sbkGameEngine.i--;
                PauseButton.Header = "❙❙";
            }
            log.Info("OnPause End");
        }
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            log.Info("OnSizeChanged Start");
            sbkGameEngine.OnSizeChanged(sender, e);
            log.Info("OnSizeChanged End");
        }
        private void OnRestart(object sender, RoutedEventArgs e)
        {
            log.Info("OnRestart Start");
            sbkGameEngine.P1Score = 0; sbkGameEngine.P2Score = 0;
            log.Info("Restart was pressed");
            ReDraw();
            WhoWon_.Visibility = Visibility.Hidden;
            RestartText.Visibility = Visibility.Hidden;
            sbkGameEngine.i = 0;
            log.Info("OnRestart End");
            sbkGameEngine.P1Wins = false;
            sbkGameEngine.P2Wins = false;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            log.Info("About_Click Start");
            About about = new About();
            log.Info("Help was clicked");
            about.Show();
            sbkGameEngine.CanBallMove = false;
            sbkGameEngine.GamePlayable = false;
            log.Info("About_Click End");
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Help_Click Start");
            Help help = new Help();
            log.Info("About was clicked");
            help.Show();
            sbkGameEngine.CanBallMove = false;
            sbkGameEngine.GamePlayable = false;
            log.Info("Help_Click End");
        }
        private void PRunner(object? obj)
        {
            log.Info("PRunner Start");
            while (sbkGameEngine.Start == true)
            {
                if (sbkGameEngine.GamePlayable == true)
                {
                    Dispatcher.Invoke(() =>
                    PressedKeys()
                    );
                    Thread.SpinWait((int)(SliderInfo.PADDLE_MAX_SPEED - SliderInfo.PaddleSpeed) * 2 * 30000
                        );
                }
            }
            log.Info("PRunner End");
        }
        private void Runner(object? obj)
        {
            log.Info("Runner Start");
            while (sbkGameEngine.Start == true)
            {
                if (sbkGameEngine.GamePlayable == true)
                {
                    Dispatcher.Invoke(() =>
                        BallMovement()
                    );
                    Thread.SpinWait((int)(SliderInfo.BALL_MAX_SPEED - SliderInfo.BallSpeed) * 30000);
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    ReDraw()
                    );
                    Dispatcher.Invoke(() =>
                    DrawPaddle(paddle1, sbkGameEngine.x1, sbkGameEngine.y1)
                    );
                    Dispatcher.Invoke(() =>
                    DrawPaddle(paddle2, sbkGameEngine.x2, sbkGameEngine.y2)
                    );
                    System.Threading.Thread.SpinWait(1000000);
                }
                if (sbkGameEngine.CanBallMove == false)
                {
                    Dispatcher.Invoke(() =>
                    Window.PauseButton.Header = "►"
                    );
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    Window.PauseButton.Header = "❙❙"
                    );
                }
            }
            log.Info("Runner End");
        }
        private void InitBoard()
        {
            log.Info("InitBoard Start");
            SliderInfo.BallSpeed = 4;
            SliderInfo.BallSize = 10;
            SliderInfo.PaddleSpeed = 4;
            SliderInfo.PaddleSize = 10;
            SliderInfo.RoundsToWin = 5;

            paddle1 = new Rectangle();
            paddle2 = new Rectangle();
            TopWall = new Rectangle();
            BottomWall = new Rectangle();
            Boundary = new Line();
            Ball = new Ellipse();
            Board.Children.Add(paddle1);
            Board.Children.Add(paddle2);

            DrawPaddle(paddle1, sbkGameEngine.x1, sbkGameEngine.y1);
            DrawPaddle(paddle2, sbkGameEngine.x2, sbkGameEngine.y2);
            log.Info("InitBoard End");
        }
        private void ReDrawUnmoving()
        {
            log.Info("ReDrawUnmoving Start");
            Boundary.Stroke = Brushes.Gray;
            Boundary.StrokeThickness = 3;
            Boundary.X1 = Board.Width / 2;
            Boundary.X2 = Board.Width / 2;
            Boundary.Y1 = 0;
            Boundary.Y2 = Board.Height;

            BottomWall.Width = Board.Width;
            BottomWall.Height = 24;
            BottomWall.Fill = Brushes.White;
            BottomWall.Stroke = Brushes.White;
            BottomWall.StrokeThickness = 4;
            Canvas.SetTop(BottomWall, Board.Height - 63);
            Canvas.SetLeft(BottomWall, 0);

            Board.Children.Remove(Boundary);
            Board.Children.Add(Boundary);
            Board.Children.Remove(TopWall);
            Board.Children.Add(TopWall);
            Board.Children.Remove(BottomWall);
            Board.Children.Add(BottomWall);
            log.Info("ReDrawUnmoving End");
        }
        #endregion

    }
}