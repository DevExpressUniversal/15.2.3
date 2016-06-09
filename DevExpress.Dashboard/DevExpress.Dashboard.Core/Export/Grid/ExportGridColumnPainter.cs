#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting;
namespace DevExpress.DashboardExport {
	public static class ExportBarPainter {
		const int BrickHorizontalPadding = 2;
		const int HOffset = 1;
		const int VOffset = 1;
		public static List<VisualBrick> CreateBrick(Rectangle cellBounds, decimal normalizedValue, decimal zeroValue) {
			List<VisualBrick> bricks = new List<VisualBrick>();
			Rectangle targetBarBounds = new Rectangle(HOffset, VOffset, cellBounds.Width - HOffset * 2, cellBounds.Height - VOffset * 2);
			Rectangle barBounds = BarBoundsCalculator.CalculateBounds(targetBarBounds, normalizedValue, zeroValue, true);
			bricks.Add(new PanelBrick() {
				Rect = barBounds,
				BackColor = DeltaColorsGetter.DefaultBarColor,
				Sides = BorderSide.None
			});
			return bricks;
		}
		public static List<VisualBrick> CreateBrick(Rectangle cellBounds, decimal normalizedValue, decimal zeroValue, BarStyleSettingsInfo barInfo, bool isPositiveNumber) {
			List<VisualBrick> bricks = new List<VisualBrick>();
			Rectangle targetBarBounds = new Rectangle(HOffset, VOffset, cellBounds.Width - HOffset * 2, cellBounds.Height - VOffset * 2);
			Rectangle barBounds = BarBoundsCalculator.CalculateBounds(targetBarBounds, normalizedValue, zeroValue, barInfo.AllowNegativeAxis);
			if(!barInfo.Color.IsEmpty) {
				if(barInfo.DrawAxis && isPositiveNumber && barBounds.X > 0)
					barBounds = new Rectangle(barBounds.X - 1, barBounds.Y, barBounds.Width + 1, barBounds.Height);
				bricks.Add(new PanelBrick() {
					Rect = barBounds,
					BackColor = barInfo.Color,
					Sides = BorderSide.None
				});
			}
			return bricks;
		}
		public static int GetBestFitWidth(string caption, float charWidth, int defaultBestCharacterCount) {
			int captionTextWidth = ExportStringMeasurer.GetTextWidth(caption);
			int barImageWidth = Convert.ToInt32(defaultBestCharacterCount * charWidth);
			return Math.Max(captionTextWidth + BrickHorizontalPadding * 2, barImageWidth + HOffset * 2);
		}
	}
	public static class ExportDeltaPainter {
		const int BrickHorizontalPadding = 2;
		public static List<VisualBrick> CreateBrick(string displayText, Rectangle bounds, GridColumnViewModel column, IndicatorType? indicatorType, bool? deltaIsGood, object value, PrintAppearanceStyleSettingsInfo styleSettings, GridDeltaInfo deltaInfo) {
			List<VisualBrick> bricks = new List<VisualBrick>();
			int imageBrickWidth = GridDeltaInfo.ImageWidth + BrickHorizontalPadding * 2;
			int textWidth = ExportStringMeasurer.GetTextWidth(displayText);
			bool contentWidthLessThanCellWidth = bounds.Width > textWidth + GridDeltaInfo.ImageWidth + BrickHorizontalPadding;
			int textBrickWidht = bounds.Width;
			if(!column.IgnoreDeltaIndication && contentWidthLessThanCellWidth)
				textBrickWidht -= imageBrickWidth;
			textBrickWidht = Math.Max(textBrickWidht, 0);
			TextBrick textBrick = new TextBrick() {
				Text = displayText,
				TextValue = value,
				Font = styleSettings.Font,
				ForeColor = styleSettings.CustomForeColor ? styleSettings.ForeColor : deltaInfo.GetTextColor(indicatorType, deltaIsGood == true, column.IgnoreDeltaColor, null, ObjectState.Normal),
				Rect = new RectangleF(0, 0, textBrickWidht, bounds.Height),
				Sides = BorderSide.None,
				BackColor = Color.Empty,
				StringFormat = BrickStringFormat.Create(TextAlignment.MiddleRight, false),
				Padding = new PaddingInfo(BrickHorizontalPadding, BrickHorizontalPadding, 0, 0),
			};
			textBrick.Style.StringFormat = BrickStringFormat.Create(textBrick.Style.TextAlignment, false, StringTrimming.EllipsisCharacter);
			bricks.Add(textBrick);
			if(!column.IgnoreDeltaIndication && contentWidthLessThanCellWidth)
				bricks.Add(new ImageBrick() {
					Image = deltaInfo.GetImage(indicatorType, deltaIsGood == true),
					Rect = new RectangleF(textBrickWidht, 0, imageBrickWidth, bounds.Height),
					Padding = new PaddingInfo(0, BrickHorizontalPadding, 0, 0),
					Sides = BorderSide.None,
					BackColor = Color.Empty,
					SizeMode = XtraPrinting.ImageSizeMode.CenterImage
				});
			return bricks;
		}
		public static int GetBestFitWidth(string caption, bool ignoreDeltaIndication) {
			int captionTextWidth = ExportStringMeasurer.GetTextWidth(caption);
			int maxContentWidth = 0;
			if(!ignoreDeltaIndication)
				maxContentWidth += GridDeltaInfo.ImageWidth + BrickHorizontalPadding;
			return Math.Max(captionTextWidth, maxContentWidth) + BrickHorizontalPadding * 2;
		}
	}
	public static class ExportStringMeasurer {
		static PrintingSystemBase ps = new PrintingSystemBase();
		public static int GetTextWidth(string text) {
			SizeF size = ps.Graph.MeasureString(text);
			return Math.Max(Convert.ToInt32(size.Width), 0);
		}
	}
}
