using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using NLog;
using Pongs;

namespace PongsSBK
{
    public class SBKPongEngine
    {
        #region Private variables

        private PongsGameSBK mGuiReference;

        HashSet<Key> AllowedKeys = new HashSet<Key>() { Key.Up, Key.Down, Key.Left, Key.Right, Key.W, Key.A, Key.S, Key.D };
        HashSet<Key> KeysPressed = new HashSet<Key>();

        private Logger log = LogManager.GetLogger("");

        #endregion

        #region Public variables

        public bool CanBallMove = false;
        public bool GamePlayable = true;
        public int i = 0;

        public double x1 = 0;
        public double x2 = 157;
        public double y1 = 0;
        public double y2 = 0;

        public bool Start = true;

        public bool P1Wins = false;
        public bool P2Wins = false;

        public int HMovement = 1;
        public int VMovement = 1;

        public int P1Score = 0;
        public int P2Score = 0;

        #endregion

        #region Public members
        public SBKPongEngine(PongsGameSBK guiReference)
        {
            log.Info("PongsGameSBK Start");

            mGuiReference = guiReference;
            Thread RunBall = new Thread(Runner);
            RunBall.Start();

            Thread RunPaddle = new Thread(PRunner);
            RunPaddle.Start();

            log.Info("PongsGameSBK End");
        }

        public void Runner(object? obj)
        {
            log.Info("Runner Start");
            while (Start == true)
            {
                if (GamePlayable == true)
                {
                    mGuiReference.Dispatcher.Invoke(() =>
                    BallMovement()
                    );

                    Thread.SpinWait((int)(SliderInfo.BALL_MAX_SPEED - SliderInfo.BallSpeed) * 30000);
                }
            }
            log.Info("Runner End");
        }
        public void BallMovement()
        {
            log.Info("BallMovement Start");
            if (GamePlayable == true)
            {
                int P1Top = (int)Canvas.GetTop(mGuiReference.paddle1);
                int P1Bottom = (int)(Canvas.GetTop(mGuiReference.paddle1) + mGuiReference.paddle1.Height);
                int P1Left = (int)Canvas.GetLeft(mGuiReference.paddle1);
                int P1Right = (int)(Canvas.GetLeft(mGuiReference.paddle1) + mGuiReference.paddle1.Width);
                int P2Top = (int)Canvas.GetTop(mGuiReference.paddle2);
                int P2Bottom = (int)(Canvas.GetTop(mGuiReference.paddle2) + mGuiReference.paddle2.Height);
                int P2Left = (int)Canvas.GetLeft(mGuiReference.paddle2);
                int P2Right = (int)(Canvas.GetLeft(mGuiReference.paddle2) + mGuiReference.paddle2.Width);
                int BallTop = (int)Canvas.GetTop(mGuiReference.Ball);
                int BallBottom = (int)(Canvas.GetTop(mGuiReference.Ball) + mGuiReference.Ball.Height);
                int BallLeft = (int)Canvas.GetLeft(mGuiReference.Ball);
                int BallRight = (int)(Canvas.GetLeft(mGuiReference.Ball) + mGuiReference.Ball.Width);

                if ((P2Bottom > BallTop && P2Top < BallBottom && BallLeft < P2Right && BallRight > P2Left && HMovement == 1) || (P1Bottom > BallTop && P1Top < BallBottom && BallLeft < P1Right && BallRight > P1Left && HMovement == -1))
                {
                    HMovement *= -1;
                    Console.Beep(37, 10);
                }
                if (BoundaryCheck(mGuiReference.Ball, 25, (int)(mGuiReference.Board.Height - (mGuiReference.BottomWall.Height * 2) - 5), 0, 0, true, true, false, false) == false)
                {
                    VMovement *= -1;
                    Console.Beep(70, 5);
                }
                if (Canvas.GetLeft(mGuiReference.Ball) >= mGuiReference.Board.Width)
                {
                    P1Score++;
                    mGuiReference.ReDraw();
                    log.Info("Player 1 scored!");
                    if (P1Score == SliderInfo.RoundsToWin)
                    {
                        P1Wins = true;
                    }
                }
                if (Canvas.GetLeft(mGuiReference.Ball) + 15 <= 0)
                {
                    P2Score++;
                    mGuiReference.ReDraw();
                    log.Info("Player 2 scored!");
                    if (P2Score == SliderInfo.RoundsToWin)
                    {
                        P1Wins = true;
                    }
                }
            }
            log.Info("BallMovement End");
        }
        public bool BoundaryCheck(System.Windows.UIElement shape, int BoundUpY, int BoundDownY, int BoundLeftX, int BoundRightX, bool UpBound, bool DownBound, bool LeftBound, bool RightBound)
        {
            log.Info("BoundaryCheck Start");
            int UpPoint = (int)Canvas.GetTop(shape);
            int DownPoint = 0;
            int LeftPoint = (int)Canvas.GetLeft(shape);
            int RightPoint = 0;

            if (shape is Rectangle)
            {
                DownPoint = UpPoint + (int)(7.8 * SliderInfo.PaddleSize) - 15;
                RightPoint = LeftPoint + (int)(1.5 * SliderInfo.PaddleSize);
            }
            if (shape is Ellipse)
            {
                DownPoint = UpPoint + (int)(1.5 * SliderInfo.BallSize);
                RightPoint = LeftPoint + (int)(1.5 * SliderInfo.BallSize);
            }

            if (UpBound == true && UpPoint <= BoundUpY)
            {
                return false;
            }
            if (DownBound == true && DownPoint >= BoundDownY)
            {
                return false;
            }
            if (LeftBound == true && LeftPoint <= BoundLeftX)
            {
                return false;
            }
            if (RightBound == true && RightPoint >= BoundRightX)
            {
                return false;
            }

            log.Info("BoundaryCheck End");
            return true;
        }
        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            log.Info("OnKeyDown Start");
            if (AllowedKeys.Contains(e.Key))
            {
                if (i == 0)
                {
                    KeysPressed.Add(e.Key);
                    CanBallMove = true;
                }
            }
            log.Info("OnKeyDown End");
        }
        public void PressedKeys(/*object? sender, EventArgs e*/)
        {
            log.Info("PressedKeys Start");
            if (GamePlayable == true)
            {
                if (KeysPressed.Contains(Key.Up) && BoundaryCheck(mGuiReference.paddle2, 25, (int)mGuiReference.Board.Height - 78, (int)mGuiReference.Board.Width / 2, (int)mGuiReference.Board.Width, true, false, false, false) == true)
                {
                    y2 -= 2;
                }
                if (KeysPressed.Contains(Key.W) && BoundaryCheck(mGuiReference.paddle1, 25, (int)mGuiReference.Board.Height - 78, (int)mGuiReference.Board.Width / 2, (int)mGuiReference.Board.Width, true, false, false, false) == true)
                {
                    y1 -= 2;
                }

                if (KeysPressed.Contains(Key.Down) && BoundaryCheck(mGuiReference.paddle2, 25, (int)mGuiReference.Board.Height - 78, (int)mGuiReference.Board.Width / 2, (int)mGuiReference.Board.Width, false, true, false, false) == true)
                {
                    y2 += 2;
                }
                if (KeysPressed.Contains(Key.S) && BoundaryCheck(mGuiReference.paddle1, 25, (int)mGuiReference.Board.Height - 78, (int)mGuiReference.Board.Width / 2, (int)mGuiReference.Board.Width, false, true, false, false) == true)
                {
                    y1 += 2;
                }

                if (KeysPressed.Contains(Key.Left) && BoundaryCheck(mGuiReference.paddle2, 25, (int)mGuiReference.Board.Height - 78, (int)mGuiReference.Board.Width / 2, (int)mGuiReference.Board.Width / 2, false, false, true, false) == true)
                {
                    x2 -= 2;
                }
                if (KeysPressed.Contains(Key.A) && BoundaryCheck(mGuiReference.paddle1, 25, (int)mGuiReference.Board.Height - 78, 0, (int)mGuiReference.Board.Width / 2, false, false, true, false) == true)
                {
                    x1 -= 2;
                }

                if (KeysPressed.Contains(Key.Right) && BoundaryCheck(mGuiReference.paddle2, 25, (int)mGuiReference.Board.Height - 78, 0, (int)mGuiReference.Board.Width - 20, false, false, false, true) == true)
                {
                    x2 += 2;
                }
                if (KeysPressed.Contains(Key.D) && BoundaryCheck(mGuiReference.paddle1, 25, (int)mGuiReference.Board.Height - 78, 0, (int)mGuiReference.Board.Width / 2, false, false, false, true) == true)
                {
                    x1 += 2;
                }
            }
            log.Info("PressedKeys End");
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            log.Info("OnKeyUp Start");
            if (KeysPressed.Contains(e.Key))
            {
                KeysPressed.Remove(e.Key);
            }
            log.Info("OnKeyUp End");
        }
        public void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            log.Info("OnSizeChanged Start");
            mGuiReference.ReDraw();
            mGuiReference.DrawPaddle(mGuiReference.paddle1, x1, y1);
            mGuiReference.DrawPaddle(mGuiReference.paddle2, x2, y2);
            log.Info("OnSizeChanged End");
        }
        #endregion

        #region Private members
        private void PRunner(object? obj)
        {
            log.Info("PRunner Start");
            while (Start == true)
            {
                if (GamePlayable == true)
                {
                    mGuiReference.Dispatcher.Invoke(() =>
                    PressedKeys()
                    );
                    Thread.SpinWait((int)(SliderInfo.PADDLE_MAX_SPEED - SliderInfo.PaddleSpeed) * 2 * 30000
                        );
                }
            }
            log.Info("PRunner End");
        }

            #endregion
    }
}
