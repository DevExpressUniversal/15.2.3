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

using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Ribbon.Internal {
	public static class RibbonSpacingModeHelper {
		const string TouchThemeNameSuffix = @";Touch";
		public static bool GetIsTouchSupported(ThemeTreeWalker themeTreeWalker) {
			if (themeTreeWalker == null)
				return false;
			string touchThemeName = themeTreeWalker.ThemeName + TouchThemeNameSuffix;
			return themeTreeWalker.IsTouch || Theme.Themes.Any(theme => theme.Name.Equals(touchThemeName));
		}
		public static void UpdateThemeName(ThemeTreeWalker themeTreeWalker, SpacingMode spacingMode) {
			if (themeTreeWalker == null)
				return;
			var currentThemeName = GetActualThemeName(themeTreeWalker);
			string themeName = GetActualSpaceModeThemeName(currentThemeName, spacingMode);
			if (currentThemeName.Equals(themeName))
				return;
			var root = LayoutHelper.GetTopLevelVisual(themeTreeWalker.Owner);
			if (GlobalThemeHelper.GetIsGlobalThemeName(root))
				ThemeManager.ApplicationThemeName = themeName;
			else {
				ThemeManager.SetThemeName(root, themeName);
			}
		}
		static string GetActualSpaceModeThemeName(string currentThemeName, SpacingMode spacingMode) {
			if (string.IsNullOrEmpty(currentThemeName))
				return currentThemeName;
			return spacingMode == SpacingMode.Touch ? GetTouchThemeName(currentThemeName) : GetThemeName(currentThemeName);
		}
		static string GetActualThemeName(ThemeTreeWalker themeTreeWalker) {
			var currentThemeName = themeTreeWalker.ThemeName;
			if (themeTreeWalker.IsTouch) {
				currentThemeName += TouchThemeNameSuffix;
			}
			return currentThemeName;
		}
		static string GetThemeName(string currentThemeName) {
			return currentThemeName.Replace(TouchThemeNameSuffix, "");
		}
		static string GetTouchThemeName(string currentThemeName) {
			if (currentThemeName.EndsWith(TouchThemeNameSuffix))
				return currentThemeName;
			return currentThemeName + TouchThemeNameSuffix;
		}
	}
}
