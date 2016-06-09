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
using DevExpress.Office.Drawing;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Native.LayoutUI;
using DevExpress.XtraRichEdit.Layout;
namespace DevExpress.Snap.Core.Native {
	public class BoxHotZonePainter : SnapHotZonePainter {
		const float TransparencyCoeff = 0.55f;
		const float TailToHeadWidthsRatio = 0.56f;
		const int MinRadius = 5;
		protected const int DefaultFontSize = 10;
		public static readonly string UIString_SecondLine = SnapLocalizer.GetString(SnapStringId.HotZonePainter_SecondLine);
		readonly Color backgroundColor;
		readonly Brush backgroundBrush;
		readonly Brush foregroundBrush = Brushes.White;
		public BoxHotZonePainter(Painter painter)
			: base(painter) {
			this.backgroundColor = Color.FromArgb((int)(256 * TransparencyCoeff), BackgroundHoverColor);
			this.backgroundBrush = new SolidBrush(backgroundColor);
		}
		protected Brush BackgroundBrush { get { return backgroundBrush; } }
		protected Brush ForegroundBrush { get { return foregroundBrush; } }
		public override void DrawHotZone(HotZone hotZone) {
			DrawHotZone((DropFieldRoundHotZone)hotZone, String.Empty);
		}
		public void DrawHotZone(DropFieldRoundHotZone hotZone, string hint) {
			if(hotZone.Radius < MinRadius)
				return;
			Painter.PushSmoothingMode(true);
			try {
				DrawHotZoneCore(hotZone, hint);
			}
			finally {
				Painter.PopSmoothingMode();
			}
		}
		protected internal void DrawHotZoneCore(DropFieldRoundHotZone hotZone, string hint) {
			RectangleF arrowBounds = GetPolygonBounds(hotZone);
			RectangleF ellipseBounds = GetEllipseBounds(arrowBounds);
			FillEllipse(hotZone.IsHovered ? BackgroundHoverBrush : BackgroundBrush, ellipseBounds);
			WithHighQualityPixelOffsetMode(Painter, () =>
				FillPolygon(ForegroundBrush, GetPolygon(arrowBounds, hotZone.ArrowSegment))
			);
			if(!String.IsNullOrEmpty(hint))
				DrawText(hotZone.Bounds, hint, UIString_SecondLine);
		}
		protected internal virtual PointF[] GetPolygon(RectangleF bounds, float segment) {
			return GetArrowPolygon(bounds, segment);
		}
		PointF[] GetArrowPolygon(RectangleF bounds, float arrowSegment) {
			int halfHeadWidth = (int)(bounds.Width * arrowSegment / 2);
			int headHeight = halfHeadWidth;
			int halfTailWidth = (int)(halfHeadWidth * TailToHeadWidthsRatio);
			int tailWidth = halfTailWidth * 2 + 1;
			int tailHeight = tailWidth;
			PointF headMiddleCenter = GetHeadMiddleCenter(bounds, arrowSegment);
			headMiddleCenter.X = ((int)(headMiddleCenter.X + 0.5)) + 0.5f;
			headMiddleCenter.Y = ((int)(headMiddleCenter.Y + 0.5));
			PointF headTopLeft = new PointF(headMiddleCenter.X - halfHeadWidth - 0.5f, headMiddleCenter.Y - headHeight);
			PointF headTopRight = new PointF(headMiddleCenter.X + halfHeadWidth + 0.5f, headMiddleCenter.Y - headHeight);
			PointF tailBottomLeft = new PointF(headMiddleCenter.X - halfTailWidth - 0.5f, headTopLeft.Y);
			PointF tailBottomRight = new PointF(headMiddleCenter.X + halfTailWidth + 0.5f, headTopRight.Y);
			PointF tailTopLeft = new PointF(tailBottomLeft.X, tailBottomLeft.Y - tailHeight);
			PointF tailTopRight = new PointF(tailBottomRight.X, tailBottomRight.Y - tailHeight);
			PointF[] polygon = new PointF[] { tailTopLeft, tailTopRight, tailBottomRight, headTopRight, headMiddleCenter, headTopLeft, tailBottomLeft };
			return polygon;
		}
		protected virtual PointF GetHeadMiddleCenter(RectangleF bounds, float arrowSegment) {
			return new PointF(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
		}
		protected virtual RectangleF GetPolygonBounds(DropFieldRoundHotZone hotZone) {
			RectangleF bounds = hotZone.Bounds;
			if(bounds.Width % 2 == 0) {
				bounds.Width++;
				bounds.Height++;
			}
			bounds.X -= 0.5f;
			bounds.Y -= 0.5f;
			return bounds;
		}
		RectangleF GetEllipseBounds(RectangleF arrowBounds) {
			RectangleF result = arrowBounds;
			result.Width++;
			result.Height++;
			return result;
		}
		protected virtual string GetLongestString() { return string.Empty; }
		float GetFontSize(float width) {
			string longestString = GetLongestString();
			int fontSize = DefaultFontSize;
			SizeF lineSize;
			do {
				lineSize = GetLineSize(longestString, fontSize++);
			} while(lineSize.Width < width);
			do {
				lineSize = GetLineSize(longestString, fontSize--);
			} while(lineSize.Width > width && fontSize > 1);
			return fontSize;
		}
		SizeF GetLineSize(string str, int fontSize) {
			using(Font font = new Font(SystemFonts.DefaultFont.FontFamily, fontSize))
				return Painter.MeasureString(str, font);
		}
		void FillEllipse(Brush brush, RectangleF boundsF) {
			Rectangle bounds = new Rectangle((int)boundsF.X, (int)boundsF.Y, (int)boundsF.Width, (int)boundsF.Height);
			Painter.FillEllipse(brush, bounds);
		}
		void FillPolygon(Brush brush, PointF[] points) {
			Painter.FillPolygon(brush, points);
		}
		void DrawText(Rectangle bounds, string line1, string line2) {
			float fontSize = GetFontSize(bounds.Width * 0.9f);
			using(Font font = new Font(SystemFonts.DefaultFont.FontFamily, fontSize)) {
				SizeF lineSize = MeasureString(line1, font);
				float textTop = bounds.Y + bounds.Height / 2;
				DrawString(line1, ForegroundBrush, font, bounds.X + (bounds.Width - lineSize.Width) / 2, textTop);
				SizeF line2Size = MeasureString(line2, font);
				DrawString(line2, ForegroundBrush, font, bounds.X + (bounds.Width - line2Size.Width) / 2, textTop + line2Size.Height);
			}
		}
		protected SizeF MeasureString(string text, Font font) {
			return Painter.MeasureString(text, font);
		}
		void DrawString(string text, Brush brush, Font font, float x, float y) {
			Painter.DrawString(text, brush, font, x, y);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing)
				BackgroundBrush.Dispose();
		}
	}
}
