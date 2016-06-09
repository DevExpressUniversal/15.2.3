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
using System.Drawing;
namespace DevExpress.XtraPrinting.Control {
	public class AreaPaintEventArgs : EventArgs {
		public RectangleF Area { get; private set; }
		public Graphics Graphics { get; private set; }
		internal AreaPaintEventArgs(Graphics graphics, RectangleF area) {
			Graphics = graphics;
			Area = area;
		}
	}
	internal class CommandExecuteEventArgs : EventArgs {
		bool handled;
		PrintingSystemCommand command;
		object[] args;
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
		public PrintingSystemCommand Command {
			get { return command; }
		}
		public object[] Args {
			get { return args; }
		}
		public CommandExecuteEventArgs(PrintingSystemCommand command, object[] args) {
			this.command = command;
			this.args = args;
		}
	}
	internal delegate void CommandExecuteEventHandler(object sender, CommandExecuteEventArgs e);
	public class BrickEventArgs : BrickEventArgsBase {
		public Rectangle BrickScreenBounds { private set; get; }
		public float X { private set; get; }
		public float Y { private set; get; }
		public EventArgs Args { private set; get; }
		public Page Page { private set; get; }
		public BrickEventArgs(EventArgs args, Brick brick, Page page, Rectangle brickScreenBounds, float x, float y)
			: base(brick) {
			Page = page;
			Args = args;
			BrickScreenBounds = brickScreenBounds;
			X = x;
			Y = y;
		}
		public BrickEventArgs(Brick brick)
			: this(EventArgs.Empty, brick, null, Rectangle.Empty, 0, 0) {
		}
	}
	public delegate void BrickEventHandler(object sender, BrickEventArgs e);
}
namespace DevExpress.XtraPrinting.Control {
	delegate void CommandEventHandler(object[] args, XtraPrinting.Control.PrintControl printControl);
}
