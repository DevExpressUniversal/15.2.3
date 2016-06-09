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
using System.Windows;
namespace DevExpress.Data {
	public enum MouseButtons {
		None = 0,
		Left = 1048576,
		Right = 2097152,
		Middle = 4194304,
		XButton1 = 8388608,
		XButton2 = 16777216,
	}
	public class MouseEventArgs : EventArgs {
		int x = 0;
		int y = 0;
		int clicks = 0;
		int delta = 0;
		System.Windows.Input.MouseEventArgs originalEventArgs;
		public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
			: this(button, clicks, x, y, delta, false, false) {
		}
		public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta, bool shift, bool ctrl)
			: this(null, button, clicks, x, y, delta, shift, ctrl) {
		}
		public MouseEventArgs(object source, MouseButtons button, int clicks, int x, int y, int delta, bool shift, bool ctrl)
			: this(source, button, clicks, x, y, delta, shift, ctrl, null) {
		}
		public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta, bool shift, bool ctrl, System.Windows.Input.MouseEventArgs originalEventArgs)
			: this(null, button, clicks, x, y, delta, shift, ctrl, originalEventArgs) {
		}
		public MouseEventArgs(object source, MouseButtons button, int clicks, int x, int y, int delta, bool shift, bool ctrl, System.Windows.Input.MouseEventArgs originalEventArgs) {
			this.Source = source;
			this.x = x;
			this.y = y;
			this.clicks = clicks;
			this.delta = delta;
			this.Button = button;
			Shift = shift;
			Ctrl = ctrl;
			this.originalEventArgs = originalEventArgs;
		}
		public object Source { get; private set; }
		public bool Shift { get; private set; }
		public bool Ctrl { get; private set; }
		public MouseButtons Button { get; private set; }
		public int Clicks { get { return clicks; } }
		public int Delta { get { return delta; } }
		public Point Location { get { return new Point(x, y); } }
		public int X { get { return x; } }
		public int Y { get { return y; } }
		public System.Windows.Input.MouseEventArgs OriginalEventArgs { get { return originalEventArgs; } }
	}
}
