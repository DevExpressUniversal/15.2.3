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
using DevExpress.Utils.Drawing;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Drawing {
	#region ConditionalFormattingPainter
	public class ConditionalFormattingPainter : IConditionalFormattingVisitor {
		readonly GraphicsCache cache;
		readonly Rectangle bounds;
		readonly ICell cell;
		public ConditionalFormattingPainter(ICell cell, GraphicsCache cache, Rectangle bounds) {
			this.cell = cell;
			this.cache = cache;
			this.bounds = bounds;
		}
		public bool Visit(IconSetConditionalFormatting formatting) {
			if (formatting == null)
				return false;
			IconSetType iconSet = formatting.IconSet;
			if (iconSet == IconSetType.None)
				return false; 
			int iconIndex = formatting.EvaluateIconIndex(cell);
			if (iconIndex < 0)
				return false;
			if(formatting.IsCustom) {
				ConditionalFormattingCustomIcon customIcon = formatting.GetIcon(iconIndex);
				iconSet = customIcon.IconSet;
				if(iconSet == IconSetType.None)
					return false;  
				iconIndex = customIcon.IconIndex;
			}
			Image image = ConditionalFormattingIconSet.GetImage(iconSet, iconIndex);
			if (image == null)
				return false;
			Rectangle imageBounds = bounds;
			imageBounds = AlignConditionalFormattingImageBounds(cell, imageBounds, image.Size, !formatting.ShowValue);
			cache.Graphics.DrawImage(image, imageBounds.Location);
			return true;
		}
		static Rectangle AlignConditionalFormattingImageBounds(ICell cell, Rectangle imageBounds, Size imageSize, bool withoutValues) {
			return DevExpress.XtraSpreadsheet.Printing.ConditionalFormattingPrinter.AlignConditionalFormattingImageBounds(cell, imageBounds, imageSize, withoutValues);
		}
		void DrawAxis(Pen pen, Point axisTopPosition) {
			Point bottomPos = new Point(axisTopPosition.X, bounds.Bottom);
			cache.Paint.DrawLine(cache.Graphics, pen, axisTopPosition, bottomPos);
		}
		void DrawGradientFill(Rectangle barBounds, Color fillColor, bool isLeftToRight) {
			IGradientColorCalculator gradientColors = DataBarGradientColorCalculatorBase.GetCalculator(isLeftToRight);
			gradientColors.Prepare(fillColor);
			Brush brush = cache.GetGradientBrush(barBounds, gradientColors.StartColor, gradientColors.EndColor, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
			cache.FillRectangle(brush, barBounds);
		}
		public bool Visit(DataBarConditionalFormatting formatting) {
			if (formatting == null)
				return false;
			ConditionalFormattingDataBarEvaluationResult value = formatting.Evaluate(cell);
			if(value.Length < 0)
				return false;
			ICondFmtDataBarPainterCalculator calculator = CondFmtDataBarPainterCalculator.GetPainterCalculator(formatting, value, bounds, 1);
			if(calculator == null)
				return false;
			calculator.Process();
			Rectangle barBounds = calculator.Bar;
			Point axisTopPos = calculator.AxisTop;
			bool isPositive = value.Value >= 0;
			Color fillColor = (isPositive || formatting.IsNegativeColorSameAsPositive) ? formatting.Color : formatting.NegativeValueColor;
			if(barBounds.Width > 0 && barBounds.Height > 0 && !DXColor.IsEmpty(fillColor)) {
				if(formatting.GradientFill) {
					bool gradientFromLeftToRight = (formatting.AxisPosition == ConditionalFormattingDataBarAxisPosition.None) || !(calculator.IsLeftToRight ^ isPositive);
					DrawGradientFill(barBounds, fillColor, gradientFromLeftToRight);
				}
				else
					cache.FillRectangle(fillColor, barBounds);
			}
			if (barBounds.Width > 0 && barBounds.Height > 0 && formatting.IsBorderColorAssigned) {
				Color borderColor = isPositive ? formatting.BorderColor : formatting.NegativeValueBorderColor;
				if (!DXColor.IsEmpty(borderColor)) {
					Pen pen = cache.GetPen(GetRgbColor(borderColor), 1);
					pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
					cache.DrawRectangle(pen, barBounds);
				}
			}
			if(!axisTopPos.IsEmpty && !DXColor.IsEmpty(formatting.AxisColor)) {
				Pen pen = cache.GetPen(GetRgbColor(formatting.AxisColor), 1);
				pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
				DrawAxis(pen, axisTopPos);
			}
			return true;
		}
		Color GetRgbColor(Color color) {
			return DXColor.FromArgb(color.A, color.R, color.G, color.B);
		}
		public bool Visit(ColorScaleConditionalFormatting formatting) {
			return false;
		}
		public bool Visit(FormulaConditionalFormatting formatting) {
			return false;
		}
	}
	#endregion
}
