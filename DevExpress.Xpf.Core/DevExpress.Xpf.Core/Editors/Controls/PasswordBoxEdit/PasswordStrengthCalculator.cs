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

#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Collections;
using System.Windows.Controls;
using System.Globalization;
#if !SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Popups;
using System.Windows.Markup;
using DevExpress.Xpf.Editors.Settings.Extension;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
#endregion
namespace DevExpress.Xpf.Editors.Native {
	public static class PasswordStrengthCalculator {
		static readonly int magicNumber = 26;
		static readonly int punctuationSymbolsCount = 52;
		static readonly int unicodeSymbolsCount = 7662;
		public static PasswordStrength Calculate(string password) {
			double strength = CalcStrength(password);
			if(strength < 20)
				return PasswordStrength.Weak;
			if(strength < 32)
				return PasswordStrength.Fair;
			if(strength < 64)
				return PasswordStrength.Good;
			return PasswordStrength.Strong;
		}
		private static double CalcStrength(string password) {
			int length = password != null ? password.Length : 0;
			if(length == 0)
				return 0;
			int strength = 0;
			if(HasDigit(password))
				strength += 10;
			if(HasLowercaseLetter(password))
				strength += magicNumber;
			if(HasUpperCaseLetter(password))
				strength += magicNumber;
			if(HasPunctuationLetter(password))
				strength += punctuationSymbolsCount;
			if(HasAnotherLetter(password))
				strength += unicodeSymbolsCount;
			return password.Length * Math.Log(strength, 2);
		}
		static bool HasDigit(string password) {
			for(int i = 0; i < password.Length; i++) {
				if(CharUnicodeInfo.GetUnicodeCategory(password, i) == UnicodeCategory.DecimalDigitNumber)
					return true;
			}
			return false;
		}
		static bool HasLowercaseLetter(string password) {
			for(int i = 0; i < password.Length; i++) {
				if(CharUnicodeInfo.GetUnicodeCategory(password, i) == UnicodeCategory.LowercaseLetter)
					return true;
			}
			return false;
		}
		static bool HasUpperCaseLetter(string password) {
			for(int i = 0; i < password.Length; i++) {
				if(CharUnicodeInfo.GetUnicodeCategory(password, i) == UnicodeCategory.UppercaseLetter)
					return true;
			}
			return false;
		}
		static bool HasPunctuationLetter(string password) {
			for(int i = 0; i < password.Length; i++) {
				if(CharUnicodeInfo.GetUnicodeCategory(password, i) == UnicodeCategory.ClosePunctuation ||
				   CharUnicodeInfo.GetUnicodeCategory(password, i) == UnicodeCategory.ConnectorPunctuation ||
				   CharUnicodeInfo.GetUnicodeCategory(password, i) == UnicodeCategory.DashPunctuation ||
				   CharUnicodeInfo.GetUnicodeCategory(password, i) == UnicodeCategory.FinalQuotePunctuation ||
				   CharUnicodeInfo.GetUnicodeCategory(password, i) == UnicodeCategory.InitialQuotePunctuation ||
				   CharUnicodeInfo.GetUnicodeCategory(password, i) == UnicodeCategory.ModifierLetter ||
				   CharUnicodeInfo.GetUnicodeCategory(password, i) == UnicodeCategory.ModifierSymbol ||
				   CharUnicodeInfo.GetUnicodeCategory(password, i) == UnicodeCategory.OpenPunctuation ||
				   CharUnicodeInfo.GetUnicodeCategory(password, i) == UnicodeCategory.OtherPunctuation ||
				   CharUnicodeInfo.GetUnicodeCategory(password, i) == UnicodeCategory.SpaceSeparator ||
				   CharUnicodeInfo.GetUnicodeCategory(password, i) == UnicodeCategory.MathSymbol)
					return true;
			}
			return false;
		}
		static bool HasAnotherLetter(string password) {
			foreach(char str in password) {
				string current = new string(str, 1);
				if (!HasLowercaseLetter(current) && !HasUpperCaseLetter(current) && !HasPunctuationLetter(current) && !HasDigit(current))
					return true;
			}
			return false;
		}
	}
}
