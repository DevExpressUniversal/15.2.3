#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Helpers;
#if !SL
using System.Media;
#endif
using StandardButtonType = DevExpress.Xpf.Editors.CalculatorStandardView.ButtonType;
namespace DevExpress.Xpf.Editors {
	public class CalculatorStandardView : CalculatorViewBase {
		public enum ButtonType {
			MC, MR, MS, MAdd, MSub,
			Back, Cancel, Clear,
			Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine,
			Decimal, Sign,
			Add, Sub, Mul, Div,
			Fract, Percent, Sqrt,
			Equal, None
		};
		public static readonly DependencyProperty ButtonTypeProperty;
		public static readonly string DisplayFormat;
		static CalculatorStandardView() {
			string s = new string('#', 29) + "0." + new string('#', 30);
			DisplayFormat = s + ";-" + s;
			ButtonTypeProperty = DependencyProperty.RegisterAttached("ButtonType", typeof(ButtonType), typeof(CalculatorStandardView),
				new PropertyMetadata(ButtonType.None, PropertyChangedButtonType));
		}
		public static ButtonType GetButtonType(DependencyObject obj) {
			return (ButtonType)obj.GetValue(ButtonTypeProperty);
		}
		public static void SetButtonType(DependencyObject obj, ButtonType value) {
			obj.SetValue(ButtonTypeProperty, value);
		}
		static void PropertyChangedButtonType(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ButtonBase button = d as ButtonBase;
			StandardButtonType buttonType = (StandardButtonType)e.NewValue;
			if (button != null && buttonType != StandardButtonType.None) {
				button.CommandParameter = buttonType;
				button.SetBinding(ButtonBase.CommandProperty, new Binding("ButtonClickCommand"));
				if (DependencyPropertyHelper.GetValueSource(button, ContentControl.ContentProperty).BaseValueSource == BaseValueSource.Default)
					if (buttonType != StandardButtonType.Decimal)
						button.Content = EditorLocalizer.GetString((EditorStringId)Enum.Parse(typeof(EditorStringId), "CalculatorButton" + buttonType.ToString(), false));
					else
						button.Content = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
			}
		}
		public CalculatorStandardView(ICalculatorViewOwner owner)
			: base(owner) {
		}
		public object LastOperationButtonID { get; set; }
		protected override CalculatorStrategyBase CreateStrategy() {
			return new CalculatorStandardStrategy(this);
		}
		protected override object GetButtonIDByKey(KeyEventArgs e) {
			switch (e.Key) {
				case Key.Back:
					return StandardButtonType.Back;
				case Key.Enter:
					if (ModifierKeysHelper.GetKeyboardModifiers(e) == ModifierKeys.None)
						return StandardButtonType.Equal;
					break;
				case Key.Decimal:
#if !SL
				case Key.OemPeriod:
				case Key.OemComma:
#endif
					return StandardButtonType.Decimal;
				case Key.Escape:
					return StandardButtonType.Clear;
			}
			return null;
		}
		protected override object GetButtonIDByTextInput(TextCompositionEventArgs e) {
			switch (e.Text) {
				case "0": return StandardButtonType.Zero;
				case "1": return StandardButtonType.One;
				case "2": return StandardButtonType.Two;
				case "3": return StandardButtonType.Three;
				case "4": return StandardButtonType.Four;
				case "5": return StandardButtonType.Five;
				case "6": return StandardButtonType.Six;
				case "7": return StandardButtonType.Seven;
				case "8": return StandardButtonType.Eight;
				case "9": return StandardButtonType.Nine;
				case "%": return StandardButtonType.Percent;
				case "+": return StandardButtonType.Add;
				case "-": return StandardButtonType.Sub;
				case "*": return StandardButtonType.Mul;
				case "/":
				case ":":
					return StandardButtonType.Div;
				case " ":
				case "=": return StandardButtonType.Equal;
			}
			return null;
		}
	}
	public class CalculatorStandardStrategy : CalculatorStrategyBase {
		public CalculatorStandardStrategy(CalculatorStandardView view)
			: base(view) {
		}
		protected object LastOperationButtonID {
			get { return View.LastOperationButtonID; }
			set { View.LastOperationButtonID = value; }
		}
		protected new CalculatorStandardView View { get { return (CalculatorStandardView)base.View; } }
		string DecimalSeparator {
			get { return Culture.NumberFormat.NumberDecimalSeparator; }
		}
		public override void Init(decimal value, bool resetMemory) {
			base.Init(value, resetMemory);
			HistoryString = "";
			LastOperationButtonID = StandardButtonType.Equal;
			DisplayValue = Result = value; 
			if (resetMemory)
				Memory = Decimal.Zero;
			Status = CalcStatus.Ok;
			SetDisplayText(DisplayValue, null, true);
		}
		public override void SetDisplayText(string text) {
			decimal value;
			if (Decimal.TryParse(text, NumberStyles.Number, Culture, out value))
				SetDisplayText(value, null, true);
			else
				Error(EditorLocalizer.GetString(EditorStringId.CalculatorInvalidInputError));
		}
		public override void UpdateFormatting() {
			base.UpdateFormatting();
			if (Status == CalcStatus.OkEnteringDigits) {
				string s = DisplayText;
				s = UpdateDecimalSeparator(s);
				s = CheckFractionalPartLength(s);
				SetDisplayText(ToDecimal(s), s, false);
			} else {
				SetDisplayText(DisplayValue, null, true);
				if (IsModified)
					SynchronizeValue();
			}
		}
		protected override void Error(string message) {
			HistoryString = "";
			LastOperationButtonID = StandardButtonType.Equal;
			base.Error(message);
		}
		protected virtual string FormatForHistory(decimal value) {
			string result = CheckFractionalPartLength(value).ToString(CalculatorStandardView.DisplayFormat, Culture);
			if (value < 0)
				result = "(" + result + ")";
			return result;
		}
		protected override void ProcessButtonClickInternal(object buttonID) {
			if (!(buttonID is StandardButtonType)) return;
			StandardButtonType buttonType = (StandardButtonType)buttonID;
			if (Status == CalcStatus.Error && buttonType != StandardButtonType.Clear) return;
			int digit;
			if (IsDigitButton(buttonType, out digit)) {
				EnterDigit(digit);
				return;
			}
			switch (buttonType) {
				case StandardButtonType.Decimal:
					EnterDecimal();
					break;
				case StandardButtonType.Percent:
					if (Status == CalcStatus.OkEnteringDigits) {
						Status = CalcStatus.Ok;
						SetDisplayText(DisplayValue * Result / 100.0m, null, true);
					}
					break;
				case StandardButtonType.Equal:
					if (LastOperationButtonID is StandardButtonType) {
						string operationString = GetOperationString((StandardButtonType)LastOperationButtonID);
						string stringForHistory = null;
						if (!String.IsNullOrEmpty(operationString) && !(operationString == "/" && DisplayValue == decimal.Zero))
							stringForHistory = String.Format("{0} {1} {2}", (HistoryString == "") ? FormatForHistory(Result) : HistoryString, operationString, FormatForHistory(DisplayValue));
						switch ((StandardButtonType)LastOperationButtonID) {
							case StandardButtonType.Add:
								Result += DisplayValue;
								break;
							case StandardButtonType.Sub: Result -= DisplayValue; break;
							case StandardButtonType.Mul: Result *= DisplayValue; break;
							case StandardButtonType.Div: if (DisplayValue == decimal.Zero) { Error(EditorLocalizer.GetString(EditorStringId.CalculatorDivisionByZeroError)); return; } else Result /= DisplayValue; break;
							case StandardButtonType.Equal:
								Result = DisplayValue;
								break;
						}
						if (stringForHistory != null) {
							AddToHistory(String.Format("{0} = {1}", stringForHistory, CheckFractionalPartLength(Result).ToString(CalculatorStandardView.DisplayFormat, Culture)));
							HistoryString = "";
						}
					}
					LastOperationButtonID = StandardButtonType.Equal;
					Status = CalcStatus.Ok;
					SetDisplayText(Result, null, true);
					break;
				case StandardButtonType.Add:
				case StandardButtonType.Sub:
				case StandardButtonType.Mul:
				case StandardButtonType.Div:
					Status = CalcStatus.Ok;
					StandardButtonType prevButtonType = (PrevButtonID != null) ? (StandardButtonType)PrevButtonID : StandardButtonType.None;
					if ((LastOperationButtonID is StandardButtonType) && prevButtonType != StandardButtonType.Add && prevButtonType != StandardButtonType.Sub && prevButtonType != StandardButtonType.Mul && prevButtonType != StandardButtonType.Div) {
						string operationString = GetOperationString((StandardButtonType)LastOperationButtonID);
						if (!String.IsNullOrEmpty(operationString))
							HistoryString = String.Format("{0} {1} {2}", (HistoryString == "") ? FormatForHistory(Result) : HistoryString, operationString, FormatForHistory(DisplayValue));
						switch ((StandardButtonType)LastOperationButtonID) {
							case StandardButtonType.Add:
								SetDisplayText(Result + DisplayValue, null, true);
								break;
							case StandardButtonType.Sub:
								SetDisplayText(Result - DisplayValue, null, true);
								break;
							case StandardButtonType.Mul:
								SetDisplayText(Result * DisplayValue, null, true);
								break;
							case StandardButtonType.Div:
								if (DisplayValue == decimal.Zero)
									Error(EditorLocalizer.GetString(EditorStringId.CalculatorDivisionByZeroError));
								else
									SetDisplayText(Result / DisplayValue, null, true);
								break;
						}
					}
					Result = DisplayValue;
					LastOperationButtonID = buttonID;
					break;
				case StandardButtonType.Sqrt:
					if (DisplayValue < decimal.Zero) Error(EditorLocalizer.GetString(EditorStringId.CalculatorInvalidInputError));
					else {
						Status = CalcStatus.Ok;
						SetDisplayText(Sqrt(DisplayValue), null, true);
						if (LastOperationButtonID.Equals(StandardButtonType.Equal))
							Result = DisplayValue;
					}
					break;
				case StandardButtonType.Fract:
					if (DisplayValue == decimal.Zero)
						Error(EditorLocalizer.GetString(EditorStringId.CalculatorDivisionByZeroError));
					else {
						Status = CalcStatus.Ok;
						SetDisplayText(1 / DisplayValue, null, true);
						if (LastOperationButtonID.Equals(StandardButtonType.Equal))
							Result = DisplayValue;
					}
					break;
				case StandardButtonType.Sign:
					if (DisplayText == "0" + DecimalSeparator)
						SetDisplayText(0, "-" + DisplayText, false);
					else if (DisplayText == "-0" + DecimalSeparator)
						SetDisplayText(0, "0" + DecimalSeparator, false);
					else
						SetDisplayText(-DisplayValue, null, true);
					if (Status == CalcStatus.Ok)
						Result = DisplayValue;
					break;
				case StandardButtonType.Back:
					ProcessBackKey();
					break;
				case StandardButtonType.Cancel:
					Status = CalcStatus.Ok;
					SetDisplayText(decimal.Zero, null, true);
					break;
				case StandardButtonType.Clear:
					LastOperationButtonID = StandardButtonType.Equal;
					Status = CalcStatus.Ok;
					Result = decimal.Zero;
					SetDisplayText(Result, null, true);
					break;
				case StandardButtonType.MC:
					Memory = decimal.Zero;
					Status = CalcStatus.Ok;
					break;
				case StandardButtonType.MR:
					Status = CalcStatus.Ok;
					SetDisplayText(Memory, null, true);
					break;
				case StandardButtonType.MS:
					Memory = DisplayValue;
					Status = CalcStatus.Ok;
					break;
				case StandardButtonType.MAdd:
					Memory += DisplayValue;
					Status = CalcStatus.Ok;
					break;
				case StandardButtonType.MSub:
					Memory -= DisplayValue;
					Status = CalcStatus.Ok;
					break;
			}
		}
		protected virtual void SetDisplayText(decimal v, string displayText, bool convert) {
			DisplayValue = v;
			v = CheckFractionalPartLength(v);
			DisplayText = convert ? v.ToString(CalculatorStandardView.DisplayFormat, Culture) : displayText;
		}
		decimal CheckFractionalPartLength(decimal num) {
			return decimal.Round(num, Precision);
		}
		string CheckFractionalPartLength(string value) {
			int separatorIndex = value.IndexOf(DecimalSeparator);
			if (separatorIndex == -1) return value;
			if (value.Length - separatorIndex > Precision)
				return value.Remove(separatorIndex + Precision + 1, value.Length - (separatorIndex + Precision + 1));
			return value;
		}
		void EnterDecimal() {
			string s = DisplayText;
			if (StartEnteringDigits()) s = "0";
			if (s.IndexOf(DecimalSeparator) == -1) {
				s += DecimalSeparator;
				SetDisplayText(ToDecimal(s), s, false);
			}
		}
		void EnterDigit(int digit) {
			string s = StartEnteringDigits() ? string.Empty : DisplayText;
			s += digit.ToString();
			int i = 0;
			if (s[0] == '-') i++;
			if (s.Length > i + 1 && s[i] == '0' && Char.IsDigit(s[i + 1]))
				s = s.Remove(i, 1);
			s = CheckFractionalPartLength(s);
			try {
				SetDisplayText(ToDecimal(s), s, false);
			} catch {
#if !SL
				SystemSounds.Beep.Play();
#endif
			}
		}
		string ExtractDecimalNumber(string val) {
			string num = string.Empty;
			bool rightSymb = false;
			int sepCount = 0;
			for (int i = 0; i < val.Length; i++) {
				rightSymb = false;
				char ch = val[i];
				rightSymb = char.IsDigit(ch);
				if (num.Length == 0 && ch == '-') rightSymb = true;
				if (sepCount == 0 && val.IndexOf(DecimalSeparator, i, DecimalSeparator.Length) == i) {
					sepCount++;
					i += Math.Max(0, DecimalSeparator.Length - 1);
					num += DecimalSeparator;
				}
				if (rightSymb) num += ch;
			}
			return num;
		}
		string GetOperationString(StandardButtonType buttonType) {
			switch (buttonType) {
				case StandardButtonType.Add:
					return "+";
				case StandardButtonType.Sub:
					return "-";
				case StandardButtonType.Mul:
					return "*";
				case StandardButtonType.Div:
					return "/";
			}
			return "";
		}
		bool IsDigitButton(StandardButtonType buttonType, out int digit) {
			switch (buttonType) {
				case StandardButtonType.Zero:
					digit = 0;
					return true;
				case StandardButtonType.One:
					digit = 1;
					return true;
				case StandardButtonType.Two:
					digit = 2;
					return true;
				case StandardButtonType.Three:
					digit = 3;
					return true;
				case StandardButtonType.Four:
					digit = 4;
					return true;
				case StandardButtonType.Five:
					digit = 5;
					return true;
				case StandardButtonType.Six:
					digit = 6;
					return true;
				case StandardButtonType.Seven:
					digit = 7;
					return true;
				case StandardButtonType.Eight:
					digit = 8;
					return true;
				case StandardButtonType.Nine:
					digit = 9;
					return true;
			}
			digit = 0;
			return false;
		}
		void ProcessBackKey() {
			StartEnteringDigits();
			decimal displayValue = decimal.Zero;
			string displayText;
			if (DisplayText.Length > 1 && !(DisplayText.Length == 2 && DisplayText[0] == '-')) {
				displayText = DisplayText.Remove(DisplayText.Length - 1, 1);
				displayValue = ToDecimal(displayText);
			} else {
				displayText = "0";
				Status = CalcStatus.Ok;
			}
			SetDisplayText(displayValue, displayText, false);
			if (LastOperationButtonID.Equals(StandardButtonType.Equal))
				Result = DisplayValue;
		}
		bool StartEnteringDigits() {
			if (Status != CalcStatus.Ok) return false;
			Status = CalcStatus.OkEnteringDigits;
			View.ResetDisplayValue();
			return true;
		}
		string UpdateDecimalSeparator(string s) {
			int decimalSeparatorPos = -1, decimalSeparatorLength = 0;
			for (int i = 0; i < s.Length; i++) {
				if (!Char.IsDigit(s[i]) && s[i] != '-')
					if (decimalSeparatorPos == -1) {
						decimalSeparatorPos = i;
						decimalSeparatorLength = 1;
					} else
						decimalSeparatorLength++;
			}
			return s.Substring(0, decimalSeparatorPos) + DecimalSeparator + s.Substring(decimalSeparatorPos + decimalSeparatorLength);
		}
		decimal ToDecimal(string value) {
			if (string.IsNullOrEmpty(value))
				return Decimal.Zero;
			decimal v;
			if (decimal.TryParse(value, NumberStyles.Number, Culture, out v))
				return v;
			value = ExtractDecimalNumber(value);
			if (value.IndexOf(DecimalSeparator) == value.Length - DecimalSeparator.Length)
				value = value.Remove(value.Length - DecimalSeparator.Length, DecimalSeparator.Length);
			return Convert.ToDecimal(value);
		}
		decimal Sqrt(decimal value) {
			return Convert.ToDecimal(Math.Sqrt(Convert.ToDouble(value)));
		}
	}
}
