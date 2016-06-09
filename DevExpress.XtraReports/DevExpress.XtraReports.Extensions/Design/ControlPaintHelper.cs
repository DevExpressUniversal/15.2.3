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
using System.Text;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Design {
	public static class ControlPaintHelper {
		public static readonly Brush WarningBrush;
		public static readonly Brush WarningBorderBrush;
		public static readonly Brush NormalSelectionBrush;
		public static readonly Brush WarningSelectionBrush;
		public static readonly Color NormalColor = Color.FromArgb(0x4f79ad);
		public static readonly Color WarningColor = Color.FromArgb(0xd60d3f);
		static ControlPaintHelper() {
			const int alphaSelected = 191;
			const int alphaUnselected = 127;
			const int alphaWarning = 38;
			WarningBorderBrush = new SolidBrush(Color.FromArgb(alphaUnselected, WarningColor));
			WarningBrush = new SolidBrush(Color.FromArgb(alphaWarning, WarningColor));
			NormalSelectionBrush = new SolidBrush(Color.FromArgb(alphaSelected, NormalColor));
			WarningSelectionBrush = new SolidBrush(Color.FromArgb(alphaSelected, WarningColor));
		}
		static Brush GetSelectionBrush(bool hasWarning) {
			if(hasWarning)
				return WarningSelectionBrush;
			else
				return NormalSelectionBrush;
		}
		public static void FillRectangle(Graphics gr, Brush brush, RectangleF rect, RectangleF clipBounds) {
			gr.FillRectangle(brush, RectangleF.Intersect(rect, clipBounds));
		}
		public static void DrawControlPrintingWarning(Graphics gr, RectangleF rect, RectangleF clipBounds, float clientRightBound) {
			float x = Math.Max(clientRightBound, rect.X);
			RectangleF intersectRect = new RectangleF(x, rect.Y, rect.Right - x, rect.Height);
			FillRectangle(gr, WarningBrush, intersectRect, clipBounds);
		}
		public static void DrawControlSelection(Graphics gr, RectangleF rect, BorderSide borders, float borderWidth, bool hasWarning) {
			Brush borderBrush = GetSelectionBrush(hasWarning);
			DrawRectangle(gr, borderBrush, borderWidth, rect, borders);
		}
		public static void DrawContainerGrabHandle(Graphics gr, RectangleF rect, float borderWidth) {
			DrawGrabRectangle(gr, rect, borderWidth, Brushes.White, Brushes.Black);
			rect.Inflate(-borderWidth * 2, -borderWidth * 2);
			float x = rect.X + (rect.Width - borderWidth) / 2;
			float y = rect.Y + (rect.Height - borderWidth) / 2;
			gr.FillRectangle(Brushes.Black, x, rect.Y, borderWidth, rect.Height);
			gr.FillRectangle(Brushes.Black, rect.X, y, rect.Width, borderWidth);
			gr.FillRectangle(Brushes.Black, x - borderWidth, rect.Y + borderWidth, borderWidth * 3, borderWidth);
			gr.FillRectangle(Brushes.Black, x - borderWidth, rect.Bottom - borderWidth * 2, borderWidth * 3, borderWidth);
			gr.FillRectangle(Brushes.Black, rect.X + borderWidth, y - borderWidth, borderWidth, borderWidth * 3);
			gr.FillRectangle(Brushes.Black, rect.Right - borderWidth * 2, y - borderWidth, borderWidth, borderWidth * 3);
		}
		public static void DrawGrabHandle(Graphics gr, RectangleF rect, float borderWidth, bool primary, bool hasWarning) {
			Brush borderBrush = GetSelectionBrush(hasWarning);
			DrawGrabRectangle(gr, rect, borderWidth, primary ? borderBrush : Brushes.White, borderBrush);
		}
		static void DrawGrabRectangle(Graphics gr, RectangleF rect, float borderWidth, Brush backgrBrush, Brush borderBrush) {
			RectangleF borderRect = rect;
			borderRect.Inflate(-borderWidth, -borderWidth);
			gr.FillRectangle(backgrBrush, borderRect);
			DrawRectangle(gr, borderBrush, borderWidth, rect);
		}
		public static void DrawRectangle(Graphics gr, Brush brush, float borderWidth, RectangleF rect) {
			DrawRectangle(gr, brush, borderWidth, rect, BorderSide.All);
		}
		public static void DrawRectangle(Graphics gr, Brush brush, float borderWidth, RectangleF rect, BorderSide borders) {
			if((borders & BorderSide.Left) > 0) {
				RectangleF r = new RectangleF(rect.X, rect.Y, borderWidth, rect.Height);
				if((borders & BorderSide.Top) > 0)
					r = RectHelper.DeflateRect(r, 0, borderWidth, 0, 0);
				if((borders & BorderSide.Bottom) > 0)
					r = RectHelper.DeflateRect(r, 0, 0, 0, borderWidth);
				gr.FillRectangle(brush, r.X, r.Y, r.Width, r.Height);
			}
			if((borders & BorderSide.Top) > 0) {
				gr.FillRectangle(brush, rect.X, rect.Y, rect.Width, borderWidth);
			}
			if((borders & BorderSide.Right) > 0) {
				RectangleF r = new RectangleF(rect.Right - borderWidth, rect.Y, borderWidth, rect.Height);
				if((borders & BorderSide.Top) > 0)
					r = RectHelper.DeflateRect(r, 0, borderWidth, 0, 0);
				if((borders & BorderSide.Bottom) > 0)
					r = RectHelper.DeflateRect(r, 0, 0, 0, borderWidth);
				gr.FillRectangle(brush, r.X, r.Y, r.Width, r.Height);
			}
			if((borders & BorderSide.Bottom) > 0) {
				gr.FillRectangle(brush, rect.X, rect.Bottom - borderWidth, rect.Width, borderWidth);
			}
		}
	}
}
