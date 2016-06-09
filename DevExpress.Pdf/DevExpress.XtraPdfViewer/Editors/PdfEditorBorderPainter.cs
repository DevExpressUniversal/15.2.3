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
using System.Collections.Generic;
using DevExpress.Utils.Drawing;
using DevExpress.Pdf.Drawing;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfEditorBorderPainter : BorderPainter {
		readonly PdfEditorBorder border;
		readonly PdfEditorController controller;
		float BorderWidth { get { return (float)(border.BorderWidth * controller.Scale); } }
		Pen SolidPen { get { return new Pen(new SolidBrush(border.Color), BorderWidth) { Alignment = PenAlignment.Inset }; } }
		Pen DashedPen {
			get {
				Pen pen = SolidPen;
				double[] dashPattern = border.DashPattern;
				if (dashPattern != null) {
					pen.DashStyle = DashStyle.Custom;
					int patternLength = dashPattern.Length;
					List<float> pattern = new List<float>();
					for (int i = 0; i < patternLength; i++) {
						float v = (float)(dashPattern[i] / border.BorderWidth);
						if (v != 0) {
							pattern.Add(v);
						}
						if (pattern.Count == 1)
							pattern.Add(pattern[0]);
						pen.DashPattern = pattern.ToArray();
						pen.DashOffset = (float)border.DashPhase;
					}
				}
				return pen;
			}
		}
		public PdfEditorBorderPainter(PdfEditorController controller) {
			this.controller = controller;
			this.border = controller.Settings.Border;
		}
		public Rectangle GetScaledContentRect(Rectangle bounds) {
			Rectangle b = bounds;
			int reduce = (int)Math.Floor(BorderWidth);
			b.Inflate(new Size(-reduce, -reduce));
			return b;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { return e.Bounds; }
		public override void DrawObject(ObjectInfoArgs e) {
			Rectangle bounds = e.Bounds;
			switch (border.BorderStyle) {
				case PdfEditorBorderStyle.Beveled:
					e.Cache.DrawRectangle(SolidPen, bounds);
					DrawStrokes(e.Cache.Graphics, bounds, Color.White, Color.Gray);
					break;
				case PdfEditorBorderStyle.Inset:
					e.Cache.DrawRectangle(SolidPen, bounds);
					DrawStrokes(e.Cache.Graphics, bounds, Color.Gray, Color.LightGray);
					break;
				case PdfEditorBorderStyle.Solid:
					e.Cache.DrawRectangle(SolidPen, bounds);
					break;
				case PdfEditorBorderStyle.Underline:
					int bw = bounds.Bottom - (int)Math.Floor(BorderWidth / 2);
					e.Cache.Graphics.DrawLine(SolidPen, new Point(bounds.Left, bw), new Point(bounds.Right, bw));
					break;
				case PdfEditorBorderStyle.Dashed:
					e.Cache.DrawRectangle(DashedPen, bounds);
					break;
			}
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { return client; }
		void DrawStroke(Graphics g, Color color, PointF[] strokePoints) {
			int length = strokePoints.Length;
			if (strokePoints != null && length > 0) {
				byte[] types = new byte[length];
				types[0] = 0;
				for (int i = 1; i < length; i++)
					types[i] = 1;
				using (GraphicsPath path = new GraphicsPath(strokePoints, types))
				using (SolidBrush brush = new SolidBrush(color))
					g.FillPath(brush, path);
			}
		}
		PointF[] GetUpperLeftStrokePoints(Rectangle bounds) {
			float borderWidth = BorderWidth;
			return new PointF[] {
				new PointF(bounds.Left,  bounds.Bottom),
				new PointF(bounds.Left, bounds.Top),
				new PointF(bounds.Right, bounds.Top),
				new PointF(bounds.Right - borderWidth, bounds.Top + borderWidth),
				new PointF(bounds.Left + borderWidth, bounds.Top + borderWidth),
				new PointF(bounds.Left + borderWidth, bounds.Bottom - borderWidth),
				new PointF(bounds.Left, bounds.Bottom)
			};
		}
		PointF[] GetBottomRightStrokePoints(Rectangle bounds) {
			float borderWidth = BorderWidth;
			return new PointF[] {
				new PointF(bounds.Right,  bounds.Top),
				new PointF(bounds.Right, bounds.Bottom),
				new PointF(bounds.Left, bounds.Bottom),
				new PointF(bounds.Left + borderWidth, bounds.Bottom - borderWidth),
				new PointF(bounds.Right - borderWidth, bounds.Bottom - borderWidth),
				new PointF(bounds.Right - borderWidth, bounds.Top + borderWidth),
				new PointF(bounds.Right, bounds.Top)
			};
		}
		void DrawStrokes(Graphics g, Rectangle bounds, Color upperLeftColor, Color bottomRightColor) {
			int reduce = -(int)Math.Ceiling(BorderWidth);
			bounds.Inflate(new Size(reduce, reduce));
			DrawStroke(g, upperLeftColor, GetUpperLeftStrokePoints(bounds));
			DrawStroke(g, bottomRightColor, GetBottomRightStrokePoints(bounds));
		}
	}
}
