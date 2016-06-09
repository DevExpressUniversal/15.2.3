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
using System.Drawing.Drawing2D;
using DevExpress.Utils;
using DevExpress.Office.PInvoke;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
namespace DevExpress.XtraRichEdit.Drawing {
	#region Caret
	public class Caret {
		#region Fields
		bool hidden = true;
		Rectangle bounds;
		#endregion
		#region Properties
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public bool IsHidden { get { return hidden; } set { hidden = value; } }
		#endregion
		[System.Security.SecuritySafeCritical]
		public virtual void Draw(Graphics gr) {
			IntPtr hdc = gr.GetHdc();
			try {
				DrawCore(hdc);
			}
			finally {
				gr.ReleaseHdc(hdc);
			}
		}
		protected internal virtual void DrawCore(IntPtr hdc) {
			IntPtr brush = Win32.GetStockObject(Win32.StockObject.WHITE_BRUSH);
			IntPtr oldBrush = Win32.SelectObject(hdc, brush);
			try {
				Win32.PatBlt(hdc, bounds.X, bounds.Y, bounds.Width, bounds.Height, Win32.TernaryRasterOperation.PATINVERT);
			}
			finally {
				Win32.SelectObject(hdc, oldBrush);
			}
		}
		public virtual bool ShouldDrawCaret(DocumentModel documentModel) {
			return documentModel.Selection.Length <= 0;
		}
		public virtual CaretPosition GetCaretPosition(RichEditView view) {
			return view.CaretPosition;
		}
	}
	#endregion
	#region DragCaret
	public class DragCaret : Caret {
		DragCaretPosition position;
		public DragCaret(RichEditView view) {
			Guard.ArgumentNotNull(view, "view");
			this.position = view.CaretPosition.CreateDragCaretPosition();
		}
		public override bool ShouldDrawCaret(DocumentModel documentModel) {
			return true;
		}
		public override CaretPosition GetCaretPosition(RichEditView view) {
			return position;
		}
		internal void SetLogPosition(DocumentLogPosition logPosition) {
			position.SetLogPosition(logPosition);
		}
		protected internal override void DrawCore(IntPtr hdc) {
			IntPtr brush = Win32.GetStockObject(Win32.StockObject.WHITE_BRUSH);
			IntPtr oldBrush = Win32.SelectObject(hdc, brush);
			try {
				Rectangle bounds = Bounds;
				int width = bounds.Width * 2;
				int dashHeight = Math.Max((int)(width / 2), 1);
				for (int y = bounds.Top; y < bounds.Bottom; y += dashHeight * 2)
					Win32.PatBlt(hdc, bounds.X, y, width, dashHeight, Win32.TernaryRasterOperation.PATINVERT);
			}
			finally {
				Win32.SelectObject(hdc, oldBrush);
			}
		}
	}
	#endregion
}
