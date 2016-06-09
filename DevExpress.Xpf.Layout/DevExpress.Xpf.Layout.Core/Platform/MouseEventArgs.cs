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
namespace DevExpress.Xpf.Layout.Core.Platform {
	[Flags]
	public enum MouseButtons : long {
		None = 0,
		Left = 0x100000,
		Right = 0x200000,
		Middle = 0x400000,
		XButton1 = 0x800000,
		XButton2 = 0x1000000,
	}
	public enum KeyEventType { 
		KeyDown,
		KeyUp
	}
	public enum MouseEventType {
		MouseDown,
		MouseUp,
		MouseMove,
		MouseLeave
	}
	[System.Diagnostics.DebuggerStepThrough]
	public class MouseEventArgs : EventArgs {
		public MouseEventArgs(Point point, MouseButtons buttons)
			: this(point, buttons, MouseButtons.None) {
		}
		public MouseEventArgs(Point point, MouseButtons buttons, MouseButtons changed) {
			Point = point;
			Buttons = buttons;
			ChangedButtons = changed;
		}
		public Point Point { get; private set; }
		public MouseButtons Buttons { get; private set; }
		public MouseButtons ChangedButtons { get; private set; }
		public bool Handled { get; set; }
	}
}
