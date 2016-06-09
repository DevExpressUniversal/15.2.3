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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraCharts.Design {
	internal class StyleEditPainter : PictureEditPainter, IDisposable {
		protected Pen highlightPen;
		protected virtual int PenWidth { get { return 3; } }
		protected virtual int Fraction { get { return 1; } }
		public StyleEditPainter(UserLookAndFeel lookAndFeel) {
			Skin skin = CommonSkins.GetSkin(lookAndFeel);
			Color focusColor = skin.Colors["HighlightAlternate"];
			if (focusColor.IsEmpty)
				focusColor = skin.Colors["Highlight"];
			Color actualColor = focusColor;
			if (skin.Name != "High Contrast")
				actualColor = GetFocusColor(focusColor);
			highlightPen = new Pen(actualColor, PenWidth);
		}
		public void Dispose() {
			if (highlightPen != null) {
				highlightPen.Dispose();
				highlightPen = null;
			}
		}
		protected virtual Color GetFocusColor(Color baseColor) {
			return Color.FromArgb(128, baseColor);
		}
		protected override void DrawFocusRect(ControlGraphicsInfoArgs info) {
			BaseEditViewInfo vi = info.ViewInfo as BaseEditViewInfo;
			if (vi != null && vi.DrawFocusRect)
				DrawCustomBorder(info);
			else
				base.DrawFocusRect(info);
		}
		protected void DrawCustomBorder(ControlGraphicsInfoArgs info) {
			SmoothingMode smoothing = info.Graphics.SmoothingMode;
			try {
				info.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				using (GraphicsPath path = new GraphicsPath()) {
					int halfWidth = -MathUtils.Ceiling(highlightPen.Width / 2.0);
					Rectangle bounds = Rectangle.Inflate(info.Bounds, halfWidth, halfWidth);
					int fraction = Fraction;
					int right = bounds.Right - fraction - 1;
					int bottom = bounds.Bottom - fraction - 1;
					path.AddArc(bounds.X, bounds.Y, fraction, fraction, 180, 90);
					path.AddArc(right, bounds.Y, fraction, fraction, 270, 90);
					path.AddArc(right, bottom, fraction, fraction, 0, 90);
					path.AddArc(bounds.Left, bottom, fraction, fraction, 90, 90);
					path.CloseFigure();
					info.Graphics.DrawPath(highlightPen, path);
				}
			}
			finally {
				info.Graphics.SmoothingMode = smoothing;
			}
		}
	}
	internal class StyleEdit : PictureEdit {
		StyleEditPainter painter;
		protected override BaseControlPainter Painter { 
			get { 
				if (painter == null) 
					painter = new StyleEditPainter(LookAndFeel);
				return painter; 
			}
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing && painter != null) {
				painter.Dispose();
				painter = null;
			}
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			StylesContainerControl container = Parent as StylesContainerControl;
			if (container != null)
				container.OnFocusChanged();
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			if (e.Button == MouseButtons.Left)
				CloseContainer();
			else
				base.OnMouseDoubleClick(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if (e.KeyCode == Keys.Return)
				CloseContainer();
			else
				base.OnKeyDown(e);
		}
		void CloseContainer() {
			StylesContainerControl container = Parent as StylesContainerControl;
			if (container != null)
				container.RaiseNeedClose();
		}
	}
}
