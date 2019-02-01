using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Splosion.Input
{
    public class KeyboardInput
    {
        private const string D = "D";
        private const string DecimalStr = "Decimal";
        private const string DivideStr = "Divide";
        private const string Zero = "0";
        private const string One = "1";
        private const string Two = "2";
        private const string Three = "3";
        private const string Four = "4";
        private const string Five = "5";
        private const string Six = "6";
        private const string Seven = "7";
        private const string Eight = "8";
        private const string Nine = "9";
        private const string ZeroShift = "}";
        private const string OneShift = "!";
        private const string TwoShift = "@";
        private const string ThreeShift = "#";
        private const string FourShift = "$";
        private const string FiveShift = "%";
        private const string SixShift = "^";
        private const string SevenShift = "&";
        private const string EightShift = "*";
        private const string NineShift = "(";
        private const string NumPad = "NumPad";
        private const string Dot = ".";
        private const string OemCommaStr = "OemComma";
        private const string OemComma = ",";
        private const string OemCommaShift = "<";
        private const string OemPeriodStr = "OemPeriod";
        private const string OemPeriod = ".";
        private const string OemPeriodShift = ">";
        private const string OemQuestionStr = "OemQuestion";
        private const string OemQuestion = "/";
        private const string OemQuestionShift = "?";
        private const string OemSemicolonStr = "OemSemicolon";
        private const string OemSemicolon = ";";
        private const string OemSemicolonShift = ":";
        private const string OemQuotesStr = "OemQuotes";
        private const string OemQuotes = "'";
        private const string OemQuotesShift = "\"";
        private const string OemOpenBracketsStr = "OemOpenBrackets";
        private const string OemOpenBrackets = "[";
        private const string OemOpenBracketsShift = "{";
        private const string OemCloseBracketsStr = "OemCloseBrackets";
        private const string OemCloseBrackets = "]";
        private const string OemCloseBracketsShift = "}";
        private const string OemPipeStr = "OemPipe";
        private const string OemPipe = "\\";
        private const string OemPipeShift = "|";
        private const string OemPlusStr = "OemPlus";
        private const string OemPlus = "=";
        private const string OemPlusShift = "+";
        private const string OemMinusStr = "OemMinus";
        private const string OemMinus = "-";
        private const string OemMinusShift = "_";
        private const string TabStr = "Tab";
        private const string Tab = "     ";
        private const string MultiplyStr = "Multiply";
        private const string Multiply = "*";
        private const string Divide = "/";
        private const string SubtractStr = "Subtract";
        private const string Subtract = "-";
        private const string AddStr = "Add";
        private const string Add = "+";
        private const string Space = " ";

        private readonly Keys[] _alphaKeys =
            {
                Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I,
                Keys.J, Keys.K, Keys.L, Keys.M,
                Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V,
                Keys.W, Keys.X, Keys.Y, Keys.Z
            };

        private readonly List<Procedure<KeyboardInput>> _keyboardListeners;

        private readonly Keys[] _keys =
            {
                Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I,
                Keys.J, Keys.K, Keys.L, Keys.M,
                Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V,
                Keys.W, Keys.X, Keys.Y, Keys.Z,
                Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7,
                Keys.D8, Keys.D9,
                Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4,
                Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9,
                Keys.OemComma, Keys.OemPeriod, Keys.OemQuestion, Keys.OemSemicolon,
                Keys.OemQuotes, Keys.OemOpenBrackets, Keys.OemCloseBrackets, Keys.OemPipe,
                Keys.OemPlus, Keys.OemMinus,
                Keys.Tab, Keys.Divide, Keys.Multiply, Keys.Subtract, Keys.Add, Keys.Decimal
            };

        private readonly Keys[] _numericKeys =
            {
                Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6,
                Keys.D7,
                Keys.D8, Keys.D9
            };

        private KeyboardState _currKey;
        private string _currentString = string.Empty;
        private KeyboardState _prevKey;
        private string _previousString = string.Empty;

        public KeyboardInput()
        {
            _keyboardListeners = new List<Procedure<KeyboardInput>>();
            _prevKey = Keyboard.GetState();
        }

        public string CurrentLine
        {
            get { return _currentString; }
        }

        public string CurrentChar
        {
            get
            {
                return _currentString == string.Empty ? string.Empty : _currentString.ToCharArray()[_currentString.Length - 1].ToString();
            }
        }

        public string PreviousLine
        {
            get { return _previousString; }
        }

        public string Typed
        {
            get
            {
                return (string.IsNullOrEmpty(_currentString))
                           ? string.Empty
                           : _currentString.ToCharArray()[_currentString.Length].ToString();
            }
        }

        public void Update(GameTime gameTime)
        {
            _prevKey = _currKey;
            _currKey = Keyboard.GetState();

            if (_prevKey == _currKey) return;
            foreach (var t in _keys)
            {
                if (!_currKey.IsKeyDown(t) || !_prevKey.IsKeyUp(t)) continue;
                var l = t.ToString();
                if (l.Length == 1)
                {
                    if (_currKey.IsKeyUp(Keys.LeftShift) && _currKey.IsKeyUp(Keys.RightShift))
                        l = l.ToLower();
                }
                else
                {
                    #region Name Numbers

                    if (l.StartsWith(D) & !l.Equals(DecimalStr) &
                        !l.Equals(Divide))
                    {
                        l = l.Substring(1);
                        if (_currKey.IsKeyDown(Keys.LeftShift) || _currKey.IsKeyDown(Keys.RightShift))
                        {
                            switch (l)
                            {
                                case Zero:
                                {
                                    l = ZeroShift;
                                    break;
                                }
                                case One:
                                {
                                    l = OneShift;
                                    break;
                                }
                                case Two:
                                {
                                    l = TwoShift;
                                    break;
                                }
                                case Three:
                                {
                                    l = ThreeShift;
                                    break;
                                }
                                case Four:
                                {
                                    l = FourShift;
                                    break;
                                }
                                case Five:
                                {
                                    l = FiveShift;
                                    break;
                                }
                                case Six:
                                {
                                    l = SixShift;
                                    break;
                                }
                                case Seven:
                                {
                                    l = SevenShift;
                                    break;
                                }
                                case Eight:
                                {
                                    l = EightShift;
                                    break;
                                }
                                case Nine:
                                {
                                    l = NineShift;
                                    break;
                                }
                            }
                        }
                    }
                        #endregion

                    else if (l.StartsWith(NumPad))
                    {
                        l = l.Substring(6);
                    }
                    else
                    {
                        switch (l)
                        {
                            case OemCommaStr:
                            {
                                l = (_currKey.IsKeyDown(Keys.LeftShift) || _currKey.IsKeyDown(Keys.RightShift))
                                    ? OemCommaShift
                                    : OemComma;
                                break;
                            }
                            case OemPeriodStr:
                            {
                                l = (_currKey.IsKeyDown(Keys.LeftShift) || _currKey.IsKeyDown(Keys.RightShift))
                                    ? OemPeriodShift
                                    : OemPeriod;
                                break;
                            }
                            case OemQuestionStr:
                            {
                                l = (_currKey.IsKeyDown(Keys.LeftShift) || _currKey.IsKeyDown(Keys.RightShift))
                                    ? OemQuestionShift
                                    : OemQuestion;
                                break;
                            }
                            case OemSemicolonStr:
                            {
                                l = (_currKey.IsKeyDown(Keys.LeftShift) || _currKey.IsKeyDown(Keys.RightShift))
                                    ? OemSemicolonShift
                                    : OemSemicolon;
                                break;
                            }
                            case OemQuotesStr:
                            {
                                l = (_currKey.IsKeyDown(Keys.LeftShift) || _currKey.IsKeyDown(Keys.RightShift))
                                    ? OemQuotesShift
                                    : OemQuotes;
                                break;
                            }
                            case OemOpenBracketsStr:
                            {
                                l = (_currKey.IsKeyDown(Keys.LeftShift) || _currKey.IsKeyDown(Keys.RightShift))
                                    ? OemOpenBracketsShift
                                    : OemOpenBrackets;
                                break;
                            }
                            case OemCloseBracketsStr:
                            {
                                l = (_currKey.IsKeyDown(Keys.LeftShift) || _currKey.IsKeyDown(Keys.RightShift))
                                    ? OemCloseBracketsShift
                                    : OemCloseBrackets;
                                break;
                            }
                            case OemPipeStr:
                            {
                                l = (_currKey.IsKeyDown(Keys.LeftShift) || _currKey.IsKeyDown(Keys.RightShift))
                                    ? OemPipeShift
                                    : OemPipe;
                                break;
                            }
                            case OemPlusStr:
                            {
                                l = (_currKey.IsKeyDown(Keys.LeftShift) || _currKey.IsKeyDown(Keys.RightShift))
                                    ? OemPlusShift
                                    : OemPlus;
                                break;
                            }
                            case OemMinusStr:
                            {
                                l = (_currKey.IsKeyDown(Keys.LeftShift) || _currKey.IsKeyDown(Keys.RightShift))
                                    ? OemMinusShift
                                    : OemMinus;
                                break;
                            }
                            case TabStr:
                            {
                                l = Tab;
                                break;
                            }
                            case MultiplyStr:
                            {
                                l = Multiply;
                                break;
                            }
                            case DivideStr:
                            {
                                l = Divide;
                                break;
                            }
                            case SubtractStr:
                            {
                                l = Subtract;
                                break;
                            }
                            case AddStr:
                            {
                                l = Add;
                                break;
                            }
                            case DecimalStr:
                            {
                                l = Dot;
                                break;
                            }
                            default:
                            {
                                break;
                            }
                        }
                    }
                }

                _currentString += l;
            }

            // Check input for spacebar
            if (_currKey.IsKeyDown(Keys.Space) && _prevKey.IsKeyUp(Keys.Space) && _currentString != string.Empty &&
                _currentString[_currentString.Length - 1].ToString() != Space)
            {
                _currentString += Space;
            }


            // Check input for backspace
            if (_currKey.IsKeyDown(Keys.Back) && _prevKey.IsKeyUp(Keys.Back) && _currentString != string.Empty)
            {
                _currentString = _currentString.Remove(_currentString.Length - 1, 1);
            }

            // Check input for enter
            if (_currKey.IsKeyDown(Keys.Enter) && _prevKey.IsKeyUp(Keys.Enter) && _currentString != string.Empty)
            {
                _previousString = _currentString;
                _currentString = string.Empty;
            }

            /*if (_currKey.IsKeyDown(Keys.Up) | _currKey.IsKeyDown(Keys.Down))
                    _currentString = string.Empty;*/

            var remove = new List<Procedure<KeyboardInput>>();
            foreach (var keyboardListener in _keyboardListeners)
                try
                {
                    keyboardListener(this);
                }
                catch
                {
                    remove.Add(keyboardListener);
                }
            _keyboardListeners.RemoveAll(remove.Contains);
        }

        public void AddKeyboardListener(Procedure<KeyboardInput> listener)
        {
            _keyboardListeners.Add(listener);
        }

        public bool TypedKey(Keys k)
        {
            return _currKey.GetPressedKeys().Contains(k) && !(_prevKey.GetPressedKeys().Contains(k));
        }

        public bool Pressed(Keys k)
        {
            return _currKey.GetPressedKeys().Contains(k);
        }

        public bool Released(Keys k)
        {
            return (_prevKey.GetPressedKeys().Contains(k));
        }

        public bool Any()
        {
            return _currKey.GetPressedKeys().Any(k => !_prevKey.GetPressedKeys().Contains(k));
        }

        public bool AnyAlpha()
        {
            return _currKey.GetPressedKeys().Any(k => _alphaKeys.Contains(k) && !_prevKey.GetPressedKeys().Contains(k));
        }

        public bool AnyNumeric()
        {
            return _currKey.GetPressedKeys().Any(k => _numericKeys.Contains(k) && !_prevKey.GetPressedKeys().Contains(k));
        }

        public bool AnyAlphaNumeric()
        {
            return AnyNumeric() || AnyAlpha();
        }
    }
}
