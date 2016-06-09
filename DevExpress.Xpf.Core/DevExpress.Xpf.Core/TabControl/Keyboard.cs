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

using DevExpress.Xpf.Utils;
using System.Windows.Input;
namespace DevExpress.Xpf.Core.Native {
	public static class TabControlKeyboardController {
		public static void OnTabItemKeyDown(DXTabItem tabItem, KeyEventArgs e) {
			if(!tabItem.IsFocused || !CanNavigate(tabItem.Owner)) return;
			e.Handled = CheckArrowKeyNavigation(tabItem.Owner, e);
		}
		public static void OnTabControlKeyDown(DXTabControl tabControl, KeyEventArgs e) {
			if(e.Key != Key.Tab || !CanNavigate(tabControl)) return;
			e.Handled = KeyboardHelper.IsControlPressed;
		}
		public static void OnTabControlKeyUp(DXTabControl tabControl, KeyEventArgs e) {
			if(e.Key != Key.Tab || !CanNavigate(tabControl)) return;
			if(KeyboardHelper.IsControlPressed && KeyboardHelper.IsShiftPressed && tabControl.CanSelectPrevItem(true)) {
				tabControl.SelectPrevItem(true);
				e.Handled = true;
				return;
			}
			if(KeyboardHelper.IsControlPressed && tabControl.CanSelectNextItem(true)) {
				tabControl.SelectNextItem(true);
				e.Handled = true;
				return;
			}
		}
		static bool CanNavigate(DXTabControl tabControl) {
			if(tabControl == null) return false;
			if(tabControl.View.AllowKeyboardNavigation.HasValue)
				return tabControl.View.AllowKeyboardNavigation.Value;
			return tabControl.View.HeaderLocation != HeaderLocation.None;
		}
		static bool CheckArrowKeyNavigation(DXTabControl tabControl, KeyEventArgs e) {
			if(tabControl == null) return false;
			switch(e.Key) {
				case Key.Right:
				case Key.Down:
					tabControl.SelectNext();
					return true;
				case Key.Left:
				case Key.Up:
					tabControl.SelectPrev();
					return true;
				case Key.Home:
					tabControl.SelectFirst();
					return true;
				case Key.End:
					tabControl.SelectLast();
					return true;
				default: return false;
			}
		}
	}
}
