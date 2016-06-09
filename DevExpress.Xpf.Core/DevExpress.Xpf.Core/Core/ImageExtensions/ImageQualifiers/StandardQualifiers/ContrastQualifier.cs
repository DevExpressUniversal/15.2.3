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

using DevExpress.Xpf.Editors.Helpers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
namespace DevExpress.Xpf.Core.Native {
	public class ContrastQualifier : IBindableUriQualifier {
		const string nameValue = "contrast";
		const string blackValue = "black";
		const string whiteValue = "white";
		public string Name { get { return nameValue; } }
		public Binding GetBinding(DependencyObject source) { return new Binding() { Path = new PropertyPath(ThemeManager.TreeWalkerProperty), Source = source }; }
		public int GetAltitude(DependencyObject context, string value, IEnumerable<string> values, out int maxAltitude) {
			maxAltitude = 1;
			bool isBlackTheme = ThemeHelper.IsBlackTheme(context);
			bool isBlackString = IsBlackString(value);
			bool isWhiteString = IsWhiteString(value);
			if (isBlackTheme && isBlackString || !isBlackTheme && isWhiteString)
				return 1;
			if (isBlackTheme && isWhiteString || !isBlackTheme && isBlackString)
				return -1;
			return 0;
		}
		bool IsBlackString(string value) {
			return string.Equals(blackValue, value, StringComparison.InvariantCultureIgnoreCase);
		}
		bool IsWhiteString(string value) {
			return string.Equals(whiteValue, value, StringComparison.InvariantCultureIgnoreCase);
		}
		public bool IsValidValue(string value) {
			return IsBlackString(value) || IsWhiteString(value);
		}
	}
}
