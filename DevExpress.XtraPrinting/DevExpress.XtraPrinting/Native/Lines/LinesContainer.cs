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
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Design;
namespace DevExpress.XtraPrinting.Native.Lines {
	[ToolboxItem(false),]
	public class LinesContainer : System.Windows.Forms.Control {
		#region static
		const int padding = 2;
		static int GetLineWidth(BaseLine[] lines) {
			int width = 0;
			foreach (BaseLine line in lines)
				width = Math.Max(width, line.GetLineSize().Width);
			return width;
		}
		#endregion
		BaseLine[] lines = new BaseLine[0];
		internal BaseLine[] Lines { get { return lines; } }
		public event EventHandler<RefreshLinesEventArgs> LinesRefreshed;
		public LinesContainer() {
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.Selectable, false);
			BackColor = Color.Transparent;
			ForeColor = Color.Transparent;
		}
		public void FillWithLines(BaseLine[] lines, UserLookAndFeel lookAndFeel, int minWidth, int innerPadding, int outerPadding) {
			this.lines = lines;
			Point point = new Point(outerPadding, outerPadding);
			foreach(BaseLine line in lines)
				line.Init(lookAndFeel);
			int lineWidth = Math.Max(GetLineWidth(lines), minWidth);
			foreach(BaseLine line in lines) {
				line.Location = point;
				line.Width = lineWidth;
				Controls.Add(line);
				line.LinesContainer = this;
				point.Offset(0, line.Height + innerPadding);
			}
			Size = new Size(lineWidth + outerPadding * 2, point.Y + (outerPadding - innerPadding));
		}
		public void RefreshLines(BaseLine initiatingLine) {
			foreach(BaseLine line in lines)
				line.RefreshProperty();
			OnLinesRefreshed(new RefreshLinesEventArgs(initiatingLine));
		}
		protected void OnLinesRefreshed(RefreshLinesEventArgs e) {
			if(LinesRefreshed != null) LinesRefreshed(this, e);
		}
	}
	public class RefreshLinesEventArgs : EventArgs {
		BaseLine initiatingLine;
		public BaseLine InitiatingLine {
			get { return initiatingLine; }
		}
		internal RefreshLinesEventArgs(BaseLine initiatingLine) {
			this.initiatingLine = initiatingLine;
		}
	}
}
