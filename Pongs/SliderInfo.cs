using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Pongs;
using PongsSBK;

public class SliderInfo
{
    public static double BALL_MAX_SPEED = 10.0;
    public static double PADDLE_MAX_SPEED = 10.0;

    
    public SliderInfo(SBKPongEngine engine)
	{
        Engine = engine;
        
    }
    public static double BallSpeed { get; set; }
    public static double BallSize { get; set; }
    public static double PaddleSpeed { get; set; }
    public static double PaddleSize { get; set; }
    public static double RoundsToWin { get; set; }
    public static Color BallColor { get; set; }
    public static Color BackgroundColor { get; set; }
    public static Color PaddleColor { get; set; }
    public static Color WallColor { get; set; }

    public SBKPongEngine Engine { get; private set; }
}
