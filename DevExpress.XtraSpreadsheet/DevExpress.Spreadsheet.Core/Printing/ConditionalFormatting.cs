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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraPrinting;
using System.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.Printing;
using DevExpress.Office.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Utils;
using DevExpress.Export.Xl;
#if !SL
using System.Drawing.Printing;
using PrintMargins = System.Drawing.Printing.Margins;
using DevExpress.XtraPrinting.BrickExporters;
using System.Drawing.Drawing2D;
using System.Collections;
#else
using System.Windows.Media;
using DevExpress.Xpf.Drawing;
using PrintMargins = DevExpress.Xpf.Drawing.Printing.Margins;
using DevExpress.Xpf.Drawing.Printing;
#endif
namespace DevExpress.XtraSpreadsheet.Printing {
	public class ConditionalFormattingPrinter : IConditionalFormattingVisitor {
		readonly PanelBrick clipBrick;
		readonly Rectangle bounds;
		readonly ICell cell;
		public ConditionalFormattingPrinter(ICell cell, PanelBrick clipBrick, Rectangle bounds) {
			this.cell = cell;
			this.clipBrick = clipBrick;
			this.bounds = bounds;
		}
		public bool Visit(IconSetConditionalFormatting formatting) {
#if !SL
			if (formatting == null)
				return false;
			IconSetType iconSet = formatting.IconSet;
			if (iconSet == IconSetType.None)
				return false; 
			int iconIndex = formatting.EvaluateIconIndex(cell);
			if (iconIndex < 0)
				return false;
			if (formatting.IsCustom) {
				ConditionalFormattingCustomIcon customIcon = formatting.GetIcon(iconIndex);
				iconSet = customIcon.IconSet;
				if (iconSet == IconSetType.None)
					return false;  
				iconIndex = customIcon.IconIndex;
			}
			Image image = ConditionalFormattingIconSet.GetImage(iconSet, iconIndex);
			if (image == null)
				return false;
			DocumentLayoutUnitConverter unitConverter = cell.Worksheet.Workbook.LayoutUnitConverter;
			OfficeImageBrick brick = new OfficeImageBrick(unitConverter);
			Rectangle imageBounds = bounds;
			imageBounds = AlignConditionalFormattingImageBounds(cell, imageBounds, Units.PixelsToDocumentsRound(image.Size, DocumentModel.DpiX, DocumentModel.DpiY), !formatting.ShowValue);
			RectangleF brickBounds = imageBounds;
			VisualBrickHelper.InitializeBrick(brick, clipBrick.PrintingSystem, brickBounds);
			brick.Image = PrintingDocumentExporter.GetImage(image);
			brick.BackColor = DXColor.Transparent;
			brick.SizeMode = ImageSizeMode.StretchImage;
			clipBrick.Bricks.Add(brick);
#endif
			return true;
		}
		public static Rectangle AlignConditionalFormattingImageBounds(ICell cell, Rectangle imageBounds, Size imageSize, bool checkHorizontal) {
			int verticalOffset = 2;
			int horizontalOffset = 1;
			Size availableSize = imageBounds.Size;
			Rectangle result;
			if (checkHorizontal) {
				switch (cell.ActualAlignment.Horizontal) {
					case XlHorizontalAlignment.Center:
					case XlHorizontalAlignment.CenterContinuous:
						horizontalOffset += (availableSize.Width - imageSize.Width) / 2;
						break;
					case XlHorizontalAlignment.Right:
						horizontalOffset += availableSize.Width - imageSize.Width;
						break;
					default:
						break;
				}
			}
			switch (cell.ActualAlignment.Vertical) {
				default:
				case XlVerticalAlignment.Top:
					result = new Rectangle(imageBounds.Left + horizontalOffset, imageBounds.Top + verticalOffset, imageSize.Width, imageSize.Height);
					break;
				case XlVerticalAlignment.Bottom:
					result = new Rectangle(imageBounds.Left + horizontalOffset, imageBounds.Top + Math.Max(0, availableSize.Height - imageSize.Height), imageSize.Width, imageSize.Height);
					break;
				case XlVerticalAlignment.Distributed:
				case XlVerticalAlignment.Justify:
				case XlVerticalAlignment.Center:
					result = new Rectangle(imageBounds.Left + horizontalOffset, imageBounds.Top + Math.Max(0, (availableSize.Height - imageSize.Height) / 2), imageSize.Width, imageSize.Height);
					break;
			}
			return result;
		}
#if !SL
		OfficeImageBrick GetGradientBrick(Rectangle barBounds, Color fillColor, DocumentLayoutUnitConverter unitConverter, bool gradientFromLeftToRight) {
			OfficeImageBrick brick = new OfficeImageBrick(unitConverter);
			VisualBrickHelper.InitializeBrick(brick, clipBrick.PrintingSystem, barBounds);
			Bitmap brickImage = new Bitmap(barBounds.Width, barBounds.Height);
			brickImage.SetResolution(300, 300);
			PrintingSystemBase printSystem = clipBrick.PrintingSystem;
			using (GdiGraphics gBmp = new ImageGraphics(brickImage, printSystem)) {
				IGradientColorCalculator gradientColors = DataBarGradientColorCalculatorBase.GetCalculator(gradientFromLeftToRight);
				gradientColors.Prepare(fillColor);
				Rectangle brushRect = new Rectangle(0, 0, barBounds.Width + 1, barBounds.Height + 1);
				LinearGradientBrush brush = new LinearGradientBrush(brushRect, gradientColors.StartColor, gradientColors.EndColor, LinearGradientMode.Horizontal);
				gBmp.FillRectangle(brush, brushRect);
				brick.SizeMode = ImageSizeMode.StretchImage;
				brick.Image = brickImage;
			}
			return brick;
		}
#endif
		public bool Visit(DataBarConditionalFormatting formatting) {
#if !SL
			if (formatting == null)
				return false;
			ConditionalFormattingDataBarEvaluationResult value = formatting.Evaluate(cell);
			if (value.Length < 0)
				return false;
			ICondFmtDataBarPainterCalculator calculator = CondFmtDataBarPainterCalculator.GetPainterCalculator(formatting, value, bounds, 3);
			if (calculator == null)
				return false;
			calculator.Process();
			Rectangle barBounds = calculator.Bar;
			Point axisTopPos = calculator.AxisTop;
			bool isPositive = value.Value >= 0;
			VisualBrick barBrick = null;
			LineBrick axisBrick = null;
			if (barBounds.Width > 0 && barBounds.Height > 0) {
				Color fillColor = (isPositive || formatting.IsNegativeColorSameAsPositive) ? formatting.Color : formatting.NegativeValueColor;
				if (!DXColor.IsEmpty(fillColor)) {
					DocumentLayoutUnitConverter unitConverter = cell.Worksheet.Workbook.LayoutUnitConverter;
					if (formatting.GradientFill) {
						bool gradientFromLeftToRight = (formatting.AxisPosition == ConditionalFormattingDataBarAxisPosition.None) || !(calculator.IsLeftToRight ^ isPositive);
						barBrick = GetGradientBrick(barBounds, fillColor, unitConverter, gradientFromLeftToRight);
					}
					else {
						barBrick = new OfficeRectBrick(unitConverter);
						VisualBrickHelper.InitializeBrick(barBrick, clipBrick.PrintingSystem, barBounds);
						barBrick.BackColor = fillColor;
					}
				}
				if (formatting.IsBorderColorAssigned) {
					Color borderColor = isPositive ? formatting.BorderColor : formatting.NegativeValueBorderColor;
					if (!DXColor.IsEmpty(borderColor)) {
						BorderSide visibleBorders = BorderSide.Top | BorderSide.Bottom | BorderSide.Left | BorderSide.Right;
						barBrick.Sides = visibleBorders;
						barBrick.BorderDashStyle = BorderDashStyle.Solid;
						barBrick.BorderColor = borderColor;
						barBrick.BorderWidth = 1;
					}
				}
			}
			if (!axisTopPos.IsEmpty && !DXColor.IsEmpty(formatting.AxisColor)) {
				if (barBrick != null)
					barBrick.Sides &= axisTopPos.X > barBounds.X ? ~BorderSide.Right : ~BorderSide.Left;
				axisBrick = new LineBrick();
				RectangleF axisRectangle = new RectangleF(axisTopPos.X, axisTopPos.Y, 1, bounds.Height);
				VisualBrickHelper.InitializeBrick(axisBrick, clipBrick.PrintingSystem, axisRectangle);
				axisBrick.ForeColor = formatting.AxisColor;
				axisBrick.LineDirection = XtraReports.UI.LineDirection.Vertical;
				axisBrick.LineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
				axisBrick.LineWidth = 1;
			}
			if (barBrick != null)
				clipBrick.Bricks.Add(barBrick);
			if (axisBrick != null)
				clipBrick.Bricks.Add(axisBrick);
#endif
			return true;
		}
		public bool Visit(ColorScaleConditionalFormatting formatting) {
			return true;
		}
		public bool Visit(FormulaConditionalFormatting formatting) {
			return true;
		}
	}
}
namespace DevExpress.XtraSpreadsheet.Internal {
	#region ICondFmtDataBarPainterCalculator
	public interface ICondFmtDataBarPainterCalculator {
		void Process();
		Rectangle Bar { get; }
		Point AxisTop { get; }
		bool IsLeftToRight { get; }
	}
	#endregion
	#region CondFmtDataBarPainterCalculator
	public abstract class CondFmtDataBarPainterCalculator : ICondFmtDataBarPainterCalculator {
		Rectangle bar;
		Point axisTop;
		protected CondFmtDataBarPainterCalculator(ConditionalFormattingDataBarEvaluationResult value, Rectangle cellBounds, int inflateScaleFactor) {
			SetInput(value, cellBounds, inflateScaleFactor);
			axisTop = new Point();
		}
		protected Rectangle CellBounds { get; set; }
		protected Rectangle PaintArea { get; set; }
		protected ConditionalFormattingDataBarEvaluationResult Value { get; set; }
		public Rectangle Bar { get { return bar; } }
		public Point AxisTop { get { return axisTop; } }
		public abstract bool IsLeftToRight { get; }
		public abstract void Process();
		protected void SetBarWidth(int value) {
			bar.Width = value;
		}
		protected void ShiftBar(int shiftValue) {
			bar.X += shiftValue;
		}
		protected void SetAxisTop(int x, int y) {
			axisTop.X = x;
			axisTop.Y = y;
		}
		protected void SetInput(ConditionalFormattingDataBarEvaluationResult value, Rectangle cellBounds, int factor) {
			Value = value;
			CellBounds = cellBounds;
			PaintArea = Rectangle.Inflate(CellBounds, -factor + 1, -2 * factor);
			bar = PaintArea;
		}
		static bool CheckLeftToRight(DataBarConditionalFormatting formatting) {
			switch (formatting.Direction) {
				case ConditionalFormattingDataBarDirection.LeftToRight:
					return true;
				case ConditionalFormattingDataBarDirection.RightToLeft:
					return false;
			}
#if SL
			return true;
#else
			return !formatting.Sheet.DataContext.Culture.TextInfo.IsRightToLeft; 
#endif
		}
		public static ICondFmtDataBarPainterCalculator GetPainterCalculator(DataBarConditionalFormatting formatting, ConditionalFormattingDataBarEvaluationResult value, Rectangle cellBounds, int inflateScaleFactor) {
			bool isLeftToRigth = CheckLeftToRight(formatting);
			bool isBipolar = (value.MinValue < 0) && (value.MaxValue > 0);
			if (!isBipolar) {
				if (isLeftToRigth)
					return new CondFmtDataBarUnipolarLtrCalculator(value, cellBounds, inflateScaleFactor);
				else
					return new CondFmtDataBarUnipolarRtlCalculator(value, cellBounds, inflateScaleFactor);
			}
			else {
				switch (formatting.AxisPosition) {
					case ConditionalFormattingDataBarAxisPosition.Automatic:
						if (isLeftToRigth)
							return new CondFmtDataBarAutoBipolarLtrCalculator(value, cellBounds, inflateScaleFactor);
						else
							return new CondFmtDataBarAutoBipolarRtlCalculator(value, cellBounds, inflateScaleFactor);
					case ConditionalFormattingDataBarAxisPosition.Middle:
						if (isLeftToRigth)
							return new CondFmtDataBarMiddleBipolarLtrCalculator(value, cellBounds, inflateScaleFactor);
						else
							return new CondFmtDataBarMiddleBipolarRtlCalculator(value, cellBounds, inflateScaleFactor);
					case ConditionalFormattingDataBarAxisPosition.None:
						if (isLeftToRigth)
							return new CondFmtDataBarNoneBipolarLtrCalculator(value, cellBounds, inflateScaleFactor);
						else
							return new CondFmtDataBarNoneBipolarRtlCalculator(value, cellBounds, inflateScaleFactor);
				}
			}
			return null;
		}
	}
	#endregion
	#region CondFmtDataBarAutoBipolarLtrCalculator - axis=automatic, bipolar values, direction=leftToRight
	public class CondFmtDataBarAutoBipolarLtrCalculator : CondFmtDataBarPainterCalculator {
		public CondFmtDataBarAutoBipolarLtrCalculator(ConditionalFormattingDataBarEvaluationResult value, Rectangle cellBounds, int inflateScaleFactor)
			: base(value, cellBounds, inflateScaleFactor) {
		}
		#region ICondFmtDataBarCalculator Members
		public override bool IsLeftToRight { get { return true; } }
		public override void Process() {
			double minValue = Value.MinValue;
			double maxValue = Value.MaxValue;
			double minBarRatio = Value.MinLength * 0.5;
			if (maxValue > -minValue) {
				double minLimit = -minBarRatio * maxValue;
				if (minValue > minLimit)
					minValue = minLimit;
			}
			else
				if (maxValue < -minValue) {
					double maxLimit = -minBarRatio * minValue;
					if (maxValue < maxLimit)
						maxValue = maxLimit;
				}
			double valueBias = maxValue - minValue;
			double maxLength = Value.LengthBias + Value.MinLength;
			double axisRatio = -minValue / valueBias; 
			int axisOffset = (int)Math.Round(PaintArea.Width * axisRatio);
			double barRatio = Math.Abs(Value.Value) * maxLength / valueBias;
			if (barRatio < minBarRatio)
				barRatio = minBarRatio;
			SetBarWidth((int)Math.Round(Bar.Width * barRatio));
			SetAxisTop(PaintArea.X + axisOffset, CellBounds.Y);
			if (Value.Value >= 0)
				ShiftBar(axisOffset + 1); 
			else
				ShiftBar(axisOffset - Bar.Width);
		}
		#endregion
	}
	#endregion
	#region CondFmtDataBarAutoBipolarRtlCalculator - axis=automatic, bipolar values, direction=rigthToLeft
	public class CondFmtDataBarAutoBipolarRtlCalculator : CondFmtDataBarPainterCalculator {
		public CondFmtDataBarAutoBipolarRtlCalculator(ConditionalFormattingDataBarEvaluationResult value, Rectangle cellBounds, int inflateScaleFactor)
			: base(value, cellBounds, inflateScaleFactor) {
		}
		#region ICondFmtDataBarCalculator Members
		public override bool IsLeftToRight { get { return false; } }
		public override void Process() {
			double minValue = Value.MinValue;
			double maxValue = Value.MaxValue;
			double minBarRatio = Value.MinLength * 0.5;
			if (maxValue > -minValue) {
				double minLimit = -minBarRatio * maxValue;
				if (minValue > minLimit)
					minValue = minLimit;
			}
			else
				if (maxValue < -minValue) {
					double maxLimit = -minBarRatio * minValue;
					if (maxValue < maxLimit)
						maxValue = maxLimit;
				}
			double valueBias = maxValue - minValue;
			double maxLength = Value.LengthBias + Value.MinLength;
			double axisRatio = -minValue / valueBias; 
			int axisOffset = PaintArea.Width - (int)Math.Round(PaintArea.Width * axisRatio);
			double barRatio = Math.Abs(Value.Value) * maxLength / valueBias;
			if (barRatio < minBarRatio)
				barRatio = minBarRatio;
			SetBarWidth((int)Math.Round(Bar.Width * barRatio));
			SetAxisTop(PaintArea.X + axisOffset, CellBounds.Y);
			if (Value.Value <= 0)
				ShiftBar(axisOffset + 1); 
			else
				ShiftBar(axisOffset - Bar.Width);
		}
		#endregion
	}
	#endregion
	#region CondFmtDataBarUnipolarCalculator (abstract class)
	public abstract class CondFmtDataBarUnipolarCalculator : CondFmtDataBarPainterCalculator {
		protected CondFmtDataBarUnipolarCalculator(ConditionalFormattingDataBarEvaluationResult value, Rectangle cellBounds, int inflateScaleFactor)
			: base(value, cellBounds, inflateScaleFactor) {
		}
		#region ICondFmtDataBarCalculator Members
		public override void Process() {
			double minBound = Math.Min(Math.Abs(Value.MinValue), Math.Abs(Value.MaxValue));
			double maxBound = Math.Max(Math.Abs(Value.MinValue), Math.Abs(Value.MaxValue));
			if (maxBound == minBound) {
				SetBarWidth(0);
				SetAxisTop(PaintArea.X + PaintArea.Width / 2, CellBounds.Y);
			}
			else {
				double ratio = ((Math.Abs(Value.Value) - minBound) / (maxBound - minBound)) * Value.LengthBias + Value.MinLength;
				SetBarWidth((int)Math.Round(Bar.Width * ratio));
			}
			TryShiftBar(PaintArea.Width - Bar.Width);
		}
		#endregion
		protected abstract void TryShiftBar(int offset);
	}
	#endregion
	#region CondFmtDataBarUnipolarLtrCalculator - unipolar values, direction=leftToRight
	public class CondFmtDataBarUnipolarLtrCalculator : CondFmtDataBarUnipolarCalculator {
		public CondFmtDataBarUnipolarLtrCalculator(ConditionalFormattingDataBarEvaluationResult value, Rectangle cellBounds, int inflateScaleFactor)
			: base(value, cellBounds, inflateScaleFactor) {
		}
		#region ICondFmtDataBarCalculator Members
		public override bool IsLeftToRight { get { return true; } }
		#endregion
		protected override void TryShiftBar(int offset) {
			if (Value.Value < 0)
				ShiftBar(offset);
		}
	}
	#endregion
	#region CondFmtDataBarUnipolarRtlCalculator - unipolar values, direction=rigthToLeft
	public class CondFmtDataBarUnipolarRtlCalculator : CondFmtDataBarUnipolarCalculator {
		public CondFmtDataBarUnipolarRtlCalculator(ConditionalFormattingDataBarEvaluationResult value, Rectangle cellBounds, int inflateScaleFactor)
			: base(value, cellBounds, inflateScaleFactor) {
		}
		#region ICondFmtDataBarCalculator Members
		public override bool IsLeftToRight { get { return false; } }
		#endregion
		protected override void TryShiftBar(int offset) {
			if (Value.Value > 0)
				ShiftBar(offset);
		}
	}
	#endregion
	#region CondFmtDataBarMiddleBipolarLtrCalculator - axis=middle, bipolar values, direction=leftToRight
	public class CondFmtDataBarMiddleBipolarLtrCalculator : CondFmtDataBarPainterCalculator {
		public CondFmtDataBarMiddleBipolarLtrCalculator(ConditionalFormattingDataBarEvaluationResult value, Rectangle cellBounds, int inflateScaleFactor)
			: base(value, cellBounds, inflateScaleFactor) {
		}
		#region ICondFmtDataBarCalculator Members
		public override bool IsLeftToRight { get { return true; } }
		public override void Process() {
			double maxBound = Math.Max(-Value.MinValue, Value.MaxValue);
			double ratio = (0.5 * Math.Abs(Value.Value) / maxBound) * Value.LengthBias + Value.MinLength;
			SetBarWidth((int)Math.Round(Bar.Width * ratio));
			int axisOffset = PaintArea.Width / 2;
			SetAxisTop(PaintArea.X + axisOffset, CellBounds.Y);
			if (Value.Value >= 0)
				ShiftBar(axisOffset + 1); 
			else
				ShiftBar(axisOffset - Bar.Width);
		}
		#endregion
	}
	#endregion
	#region CondFmtDataBarMiddleBipolarRtlCalculator - axis=middle, bipolar values, direction=rigthToLeft
	public class CondFmtDataBarMiddleBipolarRtlCalculator : CondFmtDataBarPainterCalculator {
		public CondFmtDataBarMiddleBipolarRtlCalculator(ConditionalFormattingDataBarEvaluationResult value, Rectangle cellBounds, int inflateScaleFactor)
			: base(value, cellBounds, inflateScaleFactor) {
		}
		#region ICondFmtDataBarCalculator Members
		public override bool IsLeftToRight { get { return false; } }
		public override void Process() {
			double maxBound = Math.Max(-Value.MinValue, Value.MaxValue);
			double ratio = (0.5 * Math.Abs(Value.Value) / maxBound) * Value.LengthBias + Value.MinLength;
			SetBarWidth((int)Math.Round(Bar.Width * ratio));
			int axisOffset = PaintArea.Width / 2;
			SetAxisTop(PaintArea.Right - axisOffset, CellBounds.Y);
			if (Value.Value >= 0)
				ShiftBar(axisOffset + 1); 
			else
				ShiftBar(axisOffset - Bar.Width);
		}
		#endregion
	}
	#endregion
	#region CondFmtDataBarNoneBipolarLtrCalculator - axis=none, bipolar values, direction=leftToRight
	public class CondFmtDataBarNoneBipolarLtrCalculator : CondFmtDataBarPainterCalculator {
		public CondFmtDataBarNoneBipolarLtrCalculator(ConditionalFormattingDataBarEvaluationResult value, Rectangle cellBounds, int inflateScaleFactor)
			: base(value, cellBounds, inflateScaleFactor) {
		}
		#region ICondFmtDataBarCalculator Members
		public override bool IsLeftToRight { get { return true; } }
		public override void Process() {
			double windowWidth = Value.MaxValue - Value.MinValue;
			double ratio = ((Value.Value - Value.MinValue) / windowWidth) * Value.LengthBias + Value.MinLength;
			SetBarWidth((int)Math.Round(Bar.Width * ratio));
		}
		#endregion
	}
	#endregion
	#region CondFmtDataBarNoneBipolarRtlCalculator - axis=none, bipolar values, direction=leftToRight
	public class CondFmtDataBarNoneBipolarRtlCalculator : CondFmtDataBarPainterCalculator {
		public CondFmtDataBarNoneBipolarRtlCalculator(ConditionalFormattingDataBarEvaluationResult value, Rectangle cellBounds, int inflateScaleFactor)
			: base(value, cellBounds, inflateScaleFactor) {
		}
		#region ICondFmtDataBarCalculator Members
		public override bool IsLeftToRight { get { return false; } }
		public override void Process() {
			double windowWidth = Value.MaxValue - Value.MinValue;
			double ratio = ((Value.Value - Value.MinValue) / windowWidth) * Value.LengthBias + Value.MinLength;
			SetBarWidth((int)Math.Round(Bar.Width * ratio));
			ShiftBar(PaintArea.Width - Bar.Width);
		}
		#endregion
	}
	#endregion
	public interface IGradientColorCalculator {
		Color StartColor { get; }
		Color EndColor { get; }
		void Prepare(Color colorValue);
	}
	public abstract class DataBarGradientColorCalculatorBase {
		Color definedColor;
		protected DataBarGradientColorCalculatorBase() { }
		protected Color DefinedColor { get { return definedColor; } set { definedColor = value; } }
		public static IGradientColorCalculator GetCalculator(bool leftToRight) {
			return leftToRight ? (IGradientColorCalculator)new DataBarGradientColorLtrCalculator() : (IGradientColorCalculator)new DataBarGradientColorRtlCalculator();
		}
	}
	public class DataBarGradientColorLtrCalculator : DataBarGradientColorCalculatorBase, IGradientColorCalculator {
		#region IGradientColorCalculator Members
		public Color StartColor { get { return DefinedColor; } }
		public Color EndColor { get { return DXColor.White; } }
		public void Prepare(Color colorValue) {
			DefinedColor = colorValue;
		}
		#endregion
	}
	public class DataBarGradientColorRtlCalculator : DataBarGradientColorCalculatorBase, IGradientColorCalculator {
		#region IGradientColorCalculator Members
		public Color StartColor { get { return DXColor.White; } }
		public Color EndColor { get { return DefinedColor; } }
		public void Prepare(Color colorValue) {
			DefinedColor = colorValue;
		}
		#endregion
	}
}
