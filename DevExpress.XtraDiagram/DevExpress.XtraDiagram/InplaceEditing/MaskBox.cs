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
using System.Linq;
using System.Text;
using DevExpress.XtraEditors.Mask;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using System.Runtime.InteropServices;
using System.Security;
namespace DevExpress.XtraDiagram.InplaceEditing {
	public class DiagramMaskBox : MaskBox {
		DiagramControl diagram;
		Rectangle textRect;
		Rectangle clipRect;
		public DiagramMaskBox(DiagramControl diagram) {
			this.diagram = diagram;
			Initialize();
			this.textRect = this.clipRect = Rectangle.Empty;
		}
		protected virtual void Initialize() {
			BorderStyle = BorderStyle.None;
			TabIndex = 0;
			AutoSize = false;
			Multiline = true;
			TextAlign = HorizontalAlignment.Center;
		}
		public Rectangle ClipRect { get { return clipRect; } }
		public Rectangle TextRect { get { return textRect; } }
		public void SetTextRect(Rectangle rect) {
			if(TextRect == rect) return;
			SetTextRectCore(rect);
			this.textRect = rect;
		}
		[SecuritySafeCritical]
		protected void SetTextRectCore(Rectangle rect) {
			NativeMethods.RECT nrect = new NativeMethods.RECT(rect);
			SendMessage(Handle, 0x00B3, IntPtr.Zero, ref nrect);
		}
		protected override void OnFontChanged(EventArgs e) {
			base.OnFontChanged(e);
			ResetRects();
		}
		protected virtual void ResetRects() {
			this.clipRect = this.textRect = Rectangle.Empty;
		}
		[DllImport("User32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref NativeMethods.RECT rect);
		public void SetRegion(Rectangle clipRect) {
			if(ClipRect == clipRect) return;
			this.clipRect = clipRect;
			UpdateClipRegion();
		}
		protected void UpdateClipRegion() {
			Region = new Region(ClipRect);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Control && e.KeyCode == Keys.A) {
				MaskBoxSelectAll();
				e.SuppressKeyPress = true;
			}
		}
		protected Point GetPopupMenuPos(ref Message msg) {
			Point pt = new Point(msg.LParam.ToInt32());
			if(pt.X == -1 && pt.Y == -1) {
				pt = GetPositionFromCharIndex(MaskBoxSelectionStart + MaskBoxSelectionLength);
				pt.Offset(Bounds.X + 1, Bounds.Y + 16);
				return pt;
			}
			return Diagram.PointToClient(Control.MousePosition);
		}
		protected internal new void DestroyHandle() {
			base.DestroyHandle();
		}
		protected override void WndProc(ref Message msg) {
			bool handled = false;
			switch(msg.Msg) {
				case MSG.WM_CONTEXTMENU:
					if(Diagram != null && Diagram.ShowEditorMenu(GetPopupMenuPos(ref msg))) {
						return;
					}
					break;
			}
			if(handled) return;
			base.WndProc(ref msg);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.diagram = null;
			}
			base.Dispose(disposing);
		}
		public DiagramControl Diagram { get { return diagram; } }
	}
}
