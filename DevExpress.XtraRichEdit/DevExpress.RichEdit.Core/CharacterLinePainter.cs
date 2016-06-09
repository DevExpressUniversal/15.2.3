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
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Drawing {
#region RichEditPatternLinePainterParameters
	public class RichEditPatternLinePainterParameters : PatternLinePainterParameters {
		readonly DocumentLayoutUnitConverter unitConverter;
		float[] sectionStartPattern;
		float[] pageBreakPattern;
		float[] columnBreakPattern;
		public RichEditPatternLinePainterParameters(DocumentLayoutUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
		}
		public float[] SectionStartPattern { get { return sectionStartPattern; } }
		public float[] PageBreakPattern { get { return pageBreakPattern; } }
		public float[] ColumnBreakPattern { get { return columnBreakPattern; } }
		public override void Initialize(float dpiX) {
			sectionStartPattern = CreatePattern(new float[] { 10, 10 }, dpiX);
			pageBreakPattern = CreatePattern(new float[] { 10, 10 }, dpiX);
			columnBreakPattern = CreatePattern(new float[] { 10, 20 }, dpiX);
			base.Initialize(dpiX);
		}
		protected override float PixelsToUnits(float value, float dpi) {
			return unitConverter.PixelsToLayoutUnitsF(value, dpi);
		}
	}
#endregion
#region RichEditPatternLinePainter (abstract class)
	public abstract class RichEditPatternLinePainter : PatternLinePainter, ICharacterLinePainter {
#region Static Fields
		readonly static Graphics pixelGraphics = CreatePixelGraphics();		
		PatternLinePainterParameters parameters;
#endregion
		protected RichEditPatternLinePainter(IPatternLinePaintingSupport painter, DocumentLayoutUnitConverter unitConverter)
			: base(painter, unitConverter) {
			PrepareParameters();
		}
#region Properties
		protected override PatternLinePainterParameters Parameters { get { return parameters; } }
		protected internal float[] ColumnBreakPattern {
			get {
				RichEditPatternLinePainterParameters richEditParameters = parameters as RichEditPatternLinePainterParameters;
				if (richEditParameters != null)
					return richEditParameters.ColumnBreakPattern;
				else
					return richEditParameters.DotPattern;
			}
		}
		protected internal float[] PageBreakPattern {
			get {
				RichEditPatternLinePainterParameters richEditParameters = parameters as RichEditPatternLinePainterParameters;
				if (richEditParameters != null)
					return richEditParameters.PageBreakPattern;
				else
					return richEditParameters.DotPattern;
			}
		}
		protected internal float[] SectionStartPattern {
			get {
				RichEditPatternLinePainterParameters richEditParameters = parameters as RichEditPatternLinePainterParameters;
				if (richEditParameters != null)
					return richEditParameters.SectionStartPattern;
				else
					return richEditParameters.DotPattern;
			}
		}
		protected abstract PatternLinePainterParametersTable ParametersTable { get; }
		protected override Graphics PixelGraphics { get { return pixelGraphics; } }
#endregion
		protected internal virtual void PrepareParameters() {
			lock (ParametersTable) {
				if (ParametersTable.TryGetValue(UnitConverter.GetType(), out this.parameters))
					return;
				RichEditPatternLinePainterParameters richEditParameters = new RichEditPatternLinePainterParameters(UnitConverter);
				this.parameters = richEditParameters;
				InitializeParameters(richEditParameters);
				ParametersTable.Add(UnitConverter.GetType(), this.parameters);
			}
		}
		protected abstract void InitializeParameters(PatternLinePainterParameters parameters);
		protected static Graphics CreatePixelGraphics() {
#if !SL && !DXPORTABLE
			return DevExpress.XtraPrinting.Native.GraphicsHelper.CreateGraphicsWithoutAspCheck();
#else
			return Graphics.FromHwnd(IntPtr.Zero);
#endif
		}
#region IUnderlinePainter Members
		public void DrawUnderline(UnderlineSingle underline, RectangleF bounds, Color color) {
			DrawSolidLine(bounds, color);
		}
		public void DrawUnderline(UnderlineDotted underline, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, DotPattern);
		}
		public void DrawUnderline(UnderlineDashed underline, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, DashPattern);
		}
		public void DrawUnderline(UnderlineDashSmallGap underline, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, DashSmallGapPattern);
		}
		public void DrawUnderline(UnderlineDashDotted underline, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, DashDotPattern);
		}
		public void DrawUnderline(UnderlineDashDotDotted underline, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, DashDotDotPattern);
		}
		public void DrawUnderline(UnderlineDouble underline, RectangleF bounds, Color color) {
			DrawDoubleSolidLine(bounds, color);
		}
		public void DrawUnderline(UnderlineHeavyWave underline, RectangleF bounds, Color color) {
			bounds = MakeBoundsAtLeast2PixelsHigh(bounds);
			DrawWaveUnderline(bounds, color, 2 * PixelPenWidth);
		}
		public void DrawUnderline(UnderlineLongDashed underline, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, LongDashPattern);
		}
		public void DrawUnderline(UnderlineThickSingle underline, RectangleF bounds, Color color) {
			DrawSolidLine(bounds, color);
		}
		public void DrawUnderline(UnderlineThickDotted underline, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, DotPattern);
		}
		public void DrawUnderline(UnderlineThickDashed underline, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, DashPattern);
		}
		public void DrawUnderline(UnderlineThickDashDotted underline, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, DashDotPattern);
		}
		public void DrawUnderline(UnderlineThickDashDotDotted underline, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, DashDotDotPattern);
		}
		public void DrawUnderline(UnderlineThickLongDashed underline, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, LongDashPattern);
		}
		public void DrawUnderline(UnderlineDoubleWave underline, RectangleF bounds, Color color) {
			bounds = RotateBounds(bounds);
			float thickness = RoundToPixels(bounds.Height / 2, PixelGraphics.DpiY);
			if (thickness <= PixelsToUnits(1, PixelGraphics.DpiY))
				thickness = PixelsToUnits(1, PixelGraphics.DpiY);
			RectangleF topBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, thickness);
			topBounds = MakeBoundsAtLeast2PixelsHigh(topBounds);
			RectangleF bottomBounds = new RectangleF(bounds.X, topBounds.Bottom, bounds.Width, thickness);
			bottomBounds = MakeBoundsAtLeast2PixelsHigh(bottomBounds);
			DrawWaveUnderline(RotateBounds(topBounds), color, 0);
			DrawWaveUnderline(RotateBounds(bottomBounds), color, 0);
		}
		public void DrawUnderline(UnderlineWave underline, RectangleF bounds, Color color) {
			bounds = MakeBoundsAtLeast2PixelsHigh(RotateBounds(bounds));
			DrawWaveUnderline(RotateBounds(bounds), color, 0);
		}
#endregion
#region IStrikeoutPainter Members
		public void DrawStrikeout(StrikeoutSingle Strikeout, RectangleF bounds, Color color) {
			DrawSolidLine(bounds, color);
		}
		public void DrawStrikeout(StrikeoutDouble Strikeout, RectangleF bounds, Color color) {
			DrawDoubleSolidLine(bounds, color);
		}
#endregion
	}
#endregion
#region RichEditHorizontalPatternLinePainter
	public class RichEditHorizontalPatternLinePainter : RichEditPatternLinePainter {
		readonly static PatternLinePainterParametersTable parametersTable = new PatternLinePainterParametersTable();
		public RichEditHorizontalPatternLinePainter(IPatternLinePaintingSupport painter, DocumentLayoutUnitConverter unitConverter)
			: base(painter, unitConverter) {
		}
		protected override PatternLinePainterParametersTable ParametersTable { get { return parametersTable; } }
		protected override void InitializeParameters(PatternLinePainterParameters parameters) {
			parameters.Initialize(PixelGraphics.DpiX);
		}
	}
#endregion
#region RichEditVerticalPatternLinePainter
	public class RichEditVerticalPatternLinePainter : RichEditPatternLinePainter {
		readonly static PatternLinePainterParametersTable parametersTable = new PatternLinePainterParametersTable();
		public RichEditVerticalPatternLinePainter(IPatternLinePaintingSupport painter, DocumentLayoutUnitConverter unitConverter)
			: base(painter, unitConverter) {
		}
		protected override PatternLinePainterParametersTable ParametersTable { get { return parametersTable; } }
		protected override void InitializeParameters(PatternLinePainterParameters parameters) {
			parameters.Initialize(PixelGraphics.DpiY);
		}
		protected override RectangleF RotateBounds(RectangleF bounds) {
			return new RectangleF(bounds.Y, bounds.X, bounds.Height, bounds.Width);
		}
		protected override PointF RotatePoint(PointF pointF) {
			return new PointF(pointF.Y, pointF.X);
		}
		protected override void DrawLine(Pen pen, RectangleF bounds) {
			Painter.DrawLine(pen, bounds.X, bounds.Y, bounds.X, bounds.Bottom);
		}
	}
#endregion
}
