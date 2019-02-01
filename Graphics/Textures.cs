using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Splosion.Graphics
{
    public static class Textures
    {
        //public static Texture2D FontArialSmall;
        public static Texture2D ParticleFlowerBurst;
        public static Texture2D ScapesCity;
        public static Texture2D LogoLarge;
        public static Texture2D Selector;
        public static Texture2D TextAwesome;
        public static Texture2D TextBoo;
        public static Texture2D TextExit;
        public static Texture2D TextGo;
        public static Texture2D TextGood;
        public static Texture2D TextGreat;
        public static Texture2D TextNoGood;
        public static Texture2D TextOK;
        public static Texture2D TextRush;
        public static Texture2D TextPause;
        public static Texture2D TextReady;
        public static Texture2D TextKeepUp;
        public static Texture2D TextPlay;
        public static Texture2D TextFreePlay;
        public static Texture2D TextPerfect;
        public static Texture2D TextSad;
        public static Texture2D TextWatch;
        public static Texture2D TextSet;
        public static Texture2D TextSmile;
        public static Texture2D Hand;
        public static Texture2D Bar;
        public static Texture2D Tick;
        public static Texture2D Save;
        public static Texture2D Score;
        public static Texture2D HighScore;
        public static Texture2D Level;
        public static Texture2D Required;
        public static Texture2D Continue;
        public static Texture2D Saved;
        public static Texture2D N0;
        public static Texture2D N1;
        public static Texture2D N2;
        public static Texture2D N3;
        public static Texture2D N4;
        public static Texture2D N5;
        public static Texture2D N6;
        public static Texture2D N7;
        public static Texture2D N8;
        public static Texture2D N9;
        public static Texture2D Plus;
        public static Texture2D Period;
        public static Texture2D X1;
        public static Texture2D X2;
        public static Texture2D X3;
        public static Texture2D X4;
        public static Texture2D BestScore;

        //Fonts #FF8400 Frazzel
        public static void LoadContent(ContentManager content)
        {
            Plus = content.Load<Texture2D>("Plus.png");
            Period = content.Load<Texture2D>("Period.png");
            X1 = content.Load<Texture2D>("x1.png");
            X2 = content.Load<Texture2D>("x2.png");
            X3 = content.Load<Texture2D>("x3.png");
            X4 = content.Load<Texture2D>("x4.png");
            BestScore = content.Load<Texture2D>("BestScore.png");
            Save = content.Load<Texture2D>("Save.png");
            Saved = content.Load<Texture2D>("Saved.png");
            Score = content.Load<Texture2D>("Score.png");
            HighScore = content.Load<Texture2D>("HighScore.png");
            Required = content.Load<Texture2D>("Required.png");
            Level = content.Load<Texture2D>("Level.png");
            Continue = content.Load<Texture2D>("Continue.png");
            N0 = content.Load<Texture2D>(@"Number\0.png");
            N1 = content.Load<Texture2D>(@"Number\1.png");
            N2 = content.Load<Texture2D>(@"Number\2.png");
            N3 = content.Load<Texture2D>(@"Number\3.png");
            N4 = content.Load<Texture2D>(@"Number\4.png");
            N5 = content.Load<Texture2D>(@"Number\5.png");
            N6 = content.Load<Texture2D>(@"Number\6.png");
            N7 = content.Load<Texture2D>(@"Number\7.png");
            N8 = content.Load<Texture2D>(@"Number\8.png");
            N9 = content.Load<Texture2D>(@"Number\9.png");
            Tick = content.Load<Texture2D>("Tick.png");
            Bar = content.Load<Texture2D>("Bar.png");
            TextAwesome = content.Load<Texture2D>("Awesome.png");
            TextBoo = content.Load<Texture2D>("Boo.png");
            TextExit = content.Load<Texture2D>("Exit.png");
            TextGo = content.Load<Texture2D>("Go.png");
            TextGood = content.Load<Texture2D>("Good.png");
            TextGreat = content.Load<Texture2D>("Great.png");
            TextNoGood = content.Load<Texture2D>("No Good.png");
            TextOK = content.Load<Texture2D>("Ok.png");
            TextRush = content.Load<Texture2D>("Rush.png");
            TextPause = content.Load<Texture2D>("Pause.png");
            TextReady = content.Load<Texture2D>("Ready.png");
            TextKeepUp = content.Load<Texture2D>("KeepUp.png");
            TextPlay = content.Load<Texture2D>("Play.png");
            TextWatch = content.Load<Texture2D>("Watch.png");
            TextFreePlay = content.Load<Texture2D>("FreePlay.png");   
            TextSad = content.Load<Texture2D>("Sad.png");
            TextSet = content.Load<Texture2D>("Set.png");
            TextSmile = content.Load<Texture2D>("Smile.png");
            TextPerfect = content.Load<Texture2D>("Perfect.png");
            LogoLarge = content.Load<Texture2D>("Logo.png");            
            ScapesCity = content.Load<Texture2D>("CityScape.png");
            Selector = content.Load<Texture2D>("Selector.png");
            Hand = content.Load<Texture2D>("Hand.png");   
            ParticleFlowerBurst = content.Load<Texture2D>(@"Particle\FlowerBurst.png");
            //FontArialSmall = content.Load<Texture2D>("Fonts\\ArialSmall_0.png");            
        }
    }
}
