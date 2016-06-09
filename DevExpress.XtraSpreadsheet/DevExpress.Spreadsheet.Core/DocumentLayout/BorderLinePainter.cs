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
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Drawing {
	#region SpreadsheetPatternLinePainterParameters
	public class SpreadsheetPatternLinePainterParameters : PatternLinePainterParameters {
		readonly DocumentLayoutUnitConverter unitConverter;
		float[] spreadsheetHairlinePattern;
		float[] spreadsheetDashPattern;
		float[] spreadsheetDashDotPattern;
		float[] spreadsheetDashDotDotPattern;
		float[] spreadsheetMediumDashPattern;
		public SpreadsheetPatternLinePainterParameters(DocumentLayoutUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
		}
		public float[] SpreadsheetHairlinePattern { get { return spreadsheetHairlinePattern; } }
		public float[] SpreadsheetDashPattern { get { return spreadsheetDashPattern; } }
		public float[] SpreadsheetDashDotPattern { get { return spreadsheetDashDotPattern; } }
		public float[] SpreadsheetDashDotDotPattern { get { return spreadsheetDashDotDotPattern; } }
		public float[] SpreadsheetMediumDashPattern { get { return spreadsheetMediumDashPattern; } }
		public override void Initialize(float dpiX) {
			base.Initialize(dpiX);
			spreadsheetHairlinePattern = CreatePattern(new float[] { 5, 5 }, dpiX);
			spreadsheetDashPattern = CreatePattern(new float[] { 15, 5 }, dpiX);
			spreadsheetDashDotPattern = CreatePattern(new float[] { 45, 15, 15, 15 }, dpiX);
			spreadsheetDashDotDotPattern = CreatePattern(new float[] { 45, 15, 15, 15, 15, 15 }, dpiX);
			spreadsheetMediumDashPattern = CreatePattern(new float[] { 45, 15 }, dpiX);
		}
		protected override float PixelsToUnits(float value, float dpi) {
			return unitConverter.PixelsToLayoutUnitsF(value, dpi);
		}
	}
	#endregion
	#region SpreadsheetPatternLinePainter (abstract class)
	public abstract class SpreadsheetPatternLinePainter : PatternLinePainter, IBorderLinePainter {
		#region Fields
		readonly static Graphics pixelGraphics = CreatePixelGraphics();
		PatternLinePainterParameters parameters;
		#endregion
		protected SpreadsheetPatternLinePainter(IPatternLinePaintingSupport painter, DocumentLayoutUnitConverter unitConverter)
			: base(painter, unitConverter) {
			PrepareParameters();
		}
		#region Properties
		protected override PatternLinePainterParameters Parameters { get { return parameters; } }
		protected internal SpreadsheetPatternLinePainterParameters SpreadsheetParameters { get { return (SpreadsheetPatternLinePainterParameters)parameters; } }
		protected abstract PatternLinePainterParametersTable ParametersTable { get; }
		protected override Graphics PixelGraphics { get { return pixelGraphics; } }
		float[] SpreadsheetHairlinePattern { get { return SpreadsheetParameters.SpreadsheetHairlinePattern; } }
		float[] SpreadsheetDashPattern { get { return SpreadsheetParameters.SpreadsheetDashPattern; } }
		float[] SpreadsheetDashDotPattern { get { return SpreadsheetParameters.SpreadsheetDashDotPattern; } }
		float[] SpreadsheetDashDotDotPattern { get { return SpreadsheetParameters.SpreadsheetDashDotDotPattern; } }
		float[] SpreadsheetMediumDashPattern { get { return SpreadsheetParameters.SpreadsheetMediumDashPattern; } }
		#endregion
		protected internal virtual void PrepareParameters() {
			lock (ParametersTable) {
				if (ParametersTable.TryGetValue(UnitConverter.GetType(), out this.parameters))
					return;
				SpreadsheetPatternLinePainterParameters spreadsheetParameters = new SpreadsheetPatternLinePainterParameters(UnitConverter);
				this.parameters = spreadsheetParameters;
				InitializeParameters(spreadsheetParameters);
				ParametersTable.Add(UnitConverter.GetType(), this.parameters);
			}
		}
		protected abstract void InitializeParameters(PatternLinePainterParameters parameters);
		protected static Graphics CreatePixelGraphics() {
#if !SL
			return DevExpress.XtraPrinting.Native.GraphicsHelper.CreateGraphics();
#else
			return Graphics.FromHwnd(IntPtr.Zero);
#endif
		}
		#region IBorderLinePainter Members
		public void DrawBorderLine(BorderLineThin border, RectangleF bounds, Color color) {
			DrawSolidLine(bounds, color);
		}
		public void DrawBorderLine(BorderLineMedium border, RectangleF bounds, Color color) {
			DrawSolidLine(bounds, color);
		}
		public void DrawBorderLine(BorderLineDashed border, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, SpreadsheetDashPattern);
		}
		public void DrawBorderLine(BorderLineDotted border, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, DotPattern);
		}
		public void DrawBorderLine(BorderLineThick border, RectangleF bounds, Color color) {
			DrawSolidLine(bounds, color);
		}
		public void DrawBorderLine(BorderLineDouble border, RectangleF bounds, Color color) {
			DrawDoubleSolidLine(bounds, color);
		}
		public void DrawBorderLine(BorderLineHair border, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, SpreadsheetHairlinePattern);
		}
		public void DrawBorderLine(BorderLineMediumDashed border, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, SpreadsheetMediumDashPattern);
		}
		public void DrawBorderLine(BorderLineDashDot border, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, SpreadsheetDashDotPattern);
		}
		public void DrawBorderLine(BorderLineMediumDashDot border, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, SpreadsheetDashDotPattern);
		}
		public void DrawBorderLine(BorderLineDashDotDot border, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, SpreadsheetDashDotDotPattern);
		}
		public void DrawBorderLine(BorderLineMediumDashDotDot border, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, SpreadsheetDashDotDotPattern);
		}
		public void DrawBorderLine(BorderLineSlantDashDot border, RectangleF bounds, Color color) {
			DrawPatternLine(bounds, color, SpreadsheetDashDotPattern);
		}
		public void DrawBorderLine(BorderLineThin border, PointF from, PointF to, Color color, float thickness) {
			DrawSolidLine(from, to, color, thickness);
		}
		public void DrawBorderLine(BorderLineMedium border, PointF from, PointF to, Color color, float thickness) {
			DrawSolidLine(from, to, color, thickness);
		}
		public void DrawBorderLine(BorderLineDashed border, PointF from, PointF to, Color color, float thickness) {
			DrawPatternLine(from, to, color, thickness, SpreadsheetDashPattern);
		}
		public void DrawBorderLine(BorderLineDotted border, PointF from, PointF to, Color color, float thickness) {
			DrawPatternLine(from, to, color, thickness, DotPattern);
		}
		public void DrawBorderLine(BorderLineThick border, PointF from, PointF to, Color color, float thickness) {
			DrawSolidLine(from, to, color, thickness);
		}
		public void DrawBorderLine(BorderLineDouble border, PointF from, PointF to, Color color, float thickness) {
			DrawDoubleSolidLine(from, to, color, thickness);
		}
		public void DrawBorderLine(BorderLineHair border, PointF from, PointF to, Color color, float thickness) {
			DrawPatternLine(from, to, color, thickness, SpreadsheetHairlinePattern);
		}
		public void DrawBorderLine(BorderLineMediumDashed border, PointF from, PointF to, Color color, float thickness) {
			DrawPatternLine(from, to, color, thickness, SpreadsheetMediumDashPattern);
		}
		public void DrawBorderLine(BorderLineDashDot border, PointF from, PointF to, Color color, float thickness) {
			DrawPatternLine(from, to, color, thickness, SpreadsheetDashDotPattern);
		}
		public void DrawBorderLine(BorderLineMediumDashDot border, PointF from, PointF to, Color color, float thickness) {
			DrawPatternLine(from, to, color, thickness, SpreadsheetDashDotPattern);
		}
		public void DrawBorderLine(BorderLineDashDotDot border, PointF from, PointF to, Color color, float thickness) {
			DrawPatternLine(from, to, color, thickness, SpreadsheetDashDotDotPattern);
		}
		public void DrawBorderLine(BorderLineMediumDashDotDot border, PointF from, PointF to, Color color, float thickness) {
			DrawPatternLine(from, to, color, thickness, SpreadsheetDashDotDotPattern);
		}
		public void DrawBorderLine(BorderLineSlantDashDot border, PointF from, PointF to, Color color, float thickness) {
			DrawPatternLine(from, to, color, thickness, SpreadsheetDashDotPattern);
		}
		#endregion
	}
	#endregion
	#region SpreadsheetHorizontalPatternLinePainter
	public class SpreadsheetHorizontalPatternLinePainter : SpreadsheetPatternLinePainter {
		readonly static PatternLinePainterParametersTable parametersTable = new PatternLinePainterParametersTable();
		public SpreadsheetHorizontalPatternLinePainter(IPatternLinePaintingSupport painter, DocumentLayoutUnitConverter unitConverter)
			: base(painter, unitConverter) {
		}
		protected override PatternLinePainterParametersTable ParametersTable { get { return parametersTable; } }
		protected override void InitializeParameters(PatternLinePainterParameters parameters) {
			parameters.Initialize(PixelGraphics.DpiX);
		}
		protected override void DrawLine(Pen pen, RectangleF bounds) {
			float y = bounds.Y;
			Painter.DrawLine(pen, bounds.X, y, bounds.Right, y);
		}
	}
	#endregion
	#region SpreadsheetVerticalPatternLinePainter
	public class SpreadsheetVerticalPatternLinePainter : SpreadsheetPatternLinePainter {
		readonly static PatternLinePainterParametersTable parametersTable = new PatternLinePainterParametersTable();
		public SpreadsheetVerticalPatternLinePainter(IPatternLinePaintingSupport painter, DocumentLayoutUnitConverter unitConverter)
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
			float x = bounds.X;
			Painter.DrawLine(pen, x, bounds.Y, x, bounds.Bottom);
		}
	}
	#endregion
}
