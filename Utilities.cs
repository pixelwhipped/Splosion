using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Splosion.Graphics;

namespace Splosion
{
    public static class Utilities
    {
        public static bool DoesFileExistAsync(StorageFolder folder, string fileName)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var f = await folder.GetFileAsync(fileName);
                    return f != null;
                }
                catch
                {
                    return false;
                }
            }).Result;
        }
        public static T RandomEnum<T>()
        {
            return Enum
                .GetValues(typeof(T))
                .Cast<T>()
                .OrderBy(x => Splosion.Random.Next())
                .FirstOrDefault();
        }

        public static Vector2 Center(this Texture2D t)
        {
            return new Vector2(t.Width/2f, t.Height/2f);
        }
        public static float NextFloat(this Random r)
        {
            return (float) r.NextDouble();
        }

        public static Vector2 RandomPoint(this Rectangle rect)
        {
            return new Vector2((Splosion.Random.NextFloat() * rect.Width) + rect.Left,
                (Splosion.Random.NextFloat() * rect.Height) + rect.Top);
        }

        public static Texture2D GetNum(char c)
        {
            if (c == '.') return Textures.Period;
            if (c == '+') return Textures.Plus;
            if (c == '1') return Textures.N1;
            if (c == '2') return Textures.N2;
            if (c == '3') return Textures.N3;
            if (c == '4') return Textures.N4;
            if (c == '5') return Textures.N5;
            if (c == '6') return Textures.N6;
            if (c == '7') return Textures.N7;
            if (c == '8') return Textures.N8;

            return c == '9' ? Textures.N9 : Textures.N0;
        }

        public static Texture2D[] GetNumTextures(int val)
        {
            var a = val.ToString().ToCharArray();
            return (from c in a where Char.IsNumber(c) select GetNum(c)).ToArray();
        }

        public static int GetNumWidth(int val)
        {
            return GetNumTextures(val).Sum(c => c.Width);
        }

        public static void DrawNumbers(SpriteBatch batch, int val, Vector2 posistion, Color color)
        {
            foreach (var c in GetNumTextures(val))
            {
                batch.Draw(c, posistion, color);
                posistion = new Vector2(posistion.X + c.Width, posistion.Y);
            }
        }

        public static Texture2D[] GetNumTexturesPlus(float val)
        {
            var a = ("+" + val.ToString()).ToCharArray();
            return (from c in a where (Char.IsNumber(c) || c=='.' || c=='+')select GetNum(c)).ToArray();
        }

        public static int GetNumWidthPlus(float val)
        {
            return GetNumTexturesPlus(val).Sum(c => c.Width);
        }

        public static void DrawNumbersPlus(SpriteBatch batch, float val, Vector2 posistion, Color color)
        {
            foreach (var c in GetNumTexturesPlus(val))
            {
                batch.Draw(c, posistion, color);
                posistion = new Vector2(posistion.X + c.Width, posistion.Y);
            }
        }

        public static void DrawRectangle(SpriteBatch batch, Rectangle rect)
        {
            batch.Draw(Splosion.Pixel, new Rectangle(rect.X, rect.Y, rect.Width, rect.Height), null, Color.White*.25f);
        }
    }
}
