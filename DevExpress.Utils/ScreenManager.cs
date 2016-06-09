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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.Utils {
	public class ScreenManager {
		Screen[] screens;
		Screen currentScreen;
		Point nextScreenPoint;
		Point prevScreenPoint;
		Control control;
		public ScreenManager(Control control) {
			this.screens = Screen.AllScreens;
			this.currentScreen = Screen.FromControl(control);
			this.control = control;
		}
		int delta = 50;
		public bool HasNextScreen() {
			nextScreenPoint = new Point(CurrentScreen.Bounds.Right + delta, CurrentScreen.Bounds.Y);
			if(nextScreenPoint.X <= GetMaxScreenWidth())
				return true;
			return false;
		}
		public bool HasPrevScreen() {
			prevScreenPoint = new Point(CurrentScreen.Bounds.Left - delta, CurrentScreen.Bounds.Y);
			if(prevScreenPoint.X > 0)
				return true;
			return false;
		}
		public void NextScreen() {
			if(HasNextScreen()) {
				currentScreen = Screen.FromPoint(nextScreenPoint);
				return;
			}
			currentScreen = Screen.FromPoint(Point.Empty);
		}
		public void PrevScreen() {
			if(HasPrevScreen()) {
				currentScreen = Screen.FromPoint(prevScreenPoint);
				return;
			}
			currentScreen = Screen.FromPoint(new Point(GetMaxScreenWidth(), 0));
		}
		public void RefreshScreen() {
			currentScreen = Screen.FromControl(control);
		}
		int GetMaxScreenWidth() {
			int maxWidth = 0;
			foreach(var screen in screens) {
				if(maxWidth < screen.Bounds.Right)
					maxWidth = screen.Bounds.Right;
			}
			return maxWidth;
		}
		public Screen CurrentScreen { get { return currentScreen; } }
	}
}
