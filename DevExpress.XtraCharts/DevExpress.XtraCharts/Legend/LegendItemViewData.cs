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

using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraCharts.Native {
	public class LegendItemViewData : IHitTest {
		const int CheckBoxBorderThickness = 1;
		const int CheckBoxCheckThickness = 2;
		readonly Color colorForDisabledItem = Color.LightGray;
		public static readonly Size DefaultMarkerSize = new Size(20, 16);
		int rightIndent;
		bool textVisible;
		bool markerVisible;
		Legend legend;
		ILegendItemData item;
		Size size;
		Size textSize;
		Size markerSize;
		Point offset;
		LegendItemMarkerView markerView;
		HitTestController HitTestController {
			get { return legend.Chart.HitTestController; }
		}
		ICheckableLegendItemData CheckableItem {
			get { return item as ICheckableLegendItemData; }
		}
		public int RightIndent {
			get { return rightIndent; }
			set { rightIndent = value; }
		}
		public string Text {
			get { return item.Text; }
		}
		public ILegendItemData Item {
			get { return item; }
		}
		public Point Offset {
			get { return offset; }
			set { offset = value; }
		}
		public Size Size {
			get { return size; }
		}
		public Size TextSize {
			get { return textSize; }
		}
		public Size MarkerSize {
			get { return markerSize; }
		}
		public LegendItemMarkerView MarkerView {
			get { return markerView; }
		}
		public LegendItemViewData(Legend legend, ILegendItemData item, TextMeasurer textMeasurer) {
			this.legend = legend;
			this.item = item;
			textSize = textMeasurer.MeasureStringRounded(item.Text, item.Font);
			markerSize = item.MarkerImage != null && item.MarkerImageSizeMode == ChartImageSizeMode.AutoSize ? item.MarkerImage.Size : item.MarkerSize;
			textVisible = item.TextVisible || legend.TextVisible;
			markerVisible = item.MarkerVisible || legend.MarkerVisible;
			size = textVisible ? textSize : new Size();
			if (markerVisible) {
				size.Width += markerSize.Width;
				if (markerSize.Height > size.Height)
					size.Height = markerSize.Height;
			}
			else if (CheckableItem != null && CheckableItem.UseCheckBox) {
				size.Width += DefaultMarkerSize.Width;
				if (DefaultMarkerSize.Height > size.Height)
					size.Height = DefaultMarkerSize.Height;
			}
			if (textVisible && (markerVisible || (CheckableItem != null && CheckableItem.UseCheckBox)))
				size.Width += legend.TextOffset;
			if (legend.UseCheckBoxes && CheckableItem != null && CheckableItem.UseCheckBox && item.MarkerImage == null)
				markerView = LegendItemMarkerView.Checkbox;
			else if ((!legend.UseCheckBoxes && markerVisible) || (legend.UseCheckBoxes && CheckableItem != null && !CheckableItem.UseCheckBox && markerVisible) || (CheckableItem == null && markerVisible))
				markerView = LegendItemMarkerView.Marker;
			else if (legend.UseCheckBoxes && CheckableItem != null && CheckableItem.UseCheckBox && item.MarkerImage != null)
				markerView = LegendItemMarkerView.MarkerImageAsCheckbox;
			else
				markerView = LegendItemMarkerView.None;
		}
		#region IHitTest implementation
		HitTestState hitTestState = new HitTestState();
		object IHitTest.Object { get { return this; } }
		HitTestState IHitTest.State { get { return hitTestState; } }
		#endregion
		void RenderCheckBox(IRenderer renderer, Rectangle checkBoxAreaBounds) {
			renderer.ProcessHitTestRegion(HitTestController, legend, new ChartFocusedArea(this), new HitRegion(checkBoxAreaBounds), true);
			Color color = CheckableItem.Disabled ? this.colorForDisabledItem : CheckableItem.MainColor;
			Rectangle checkBoxBounds = checkBoxAreaBounds;
			checkBoxBounds.Inflate(-3, -1);
			renderer.DrawRectangle(checkBoxBounds, color, CheckBoxBorderThickness);
			if (CheckableItem.CheckedInLegend) {
				Point[] checkPoints = {
					Point.Add(checkBoxBounds.Location, new Size(3, 6)),
					Point.Add(checkBoxBounds.Location, new Size(6, 10)),
					Point.Add(checkBoxBounds.Location, new Size(11, 3))
				};
				renderer.EnableAntialiasing(true);
				renderer.DrawLine(checkPoints[0], checkPoints[1], color, CheckBoxCheckThickness, new LineStyle(null), LineCap.Triangle);
				renderer.DrawLine(checkPoints[1], checkPoints[2], color, CheckBoxCheckThickness, new LineStyle(null), LineCap.Triangle);
				renderer.RestoreAntialiasing();
			}
		}
		void RenderLegendItemText(IRenderer renderer, Point textLocation) {
			bool shouldDispose = item.DisposeFont && !ReferenceEquals(item.Font, legend.Font);
			NativeFont nativeFont = shouldDispose ? new NativeFontDisposable(item.Font) : new NativeFont(item.Font);
			Color actualTextColor = GetActualLegendItemTextColor();
			renderer.ProcessHitTestRegion(HitTestController, legend, item.RepresentedObject, new HitRegion(new RectangleF(textLocation.X, textLocation.Y, textSize.Width, textSize.Height)));
			renderer.DrawText(item.Text, nativeFont, actualTextColor, legend, textLocation);
			if (shouldDispose)
				nativeFont.Dispose();
		}
		Color GetActualLegendItemTextColor() {
			if (item is ICheckableLegendItemData)
				return ((ICheckableLegendItemData)item).Disabled && ((ICheckableLegendItemData)item).UseCheckBox ? this.colorForDisabledItem : item.TextColor;
			else
				return item.TextColor;
		}
		Rectangle CalculateMarkerBounds(Point firstLegendItemLocation, bool isRightToLeft) {
			if (legend.MarkerVisible && !item.MarkerVisible)
				return Rectangle.Empty;
			int halfHeightDiff = (textSize.Height - markerSize.Height) / 2;
			int markerVerticalOffsetFromItemLocation = halfHeightDiff < 0 ? 0 : halfHeightDiff;
			int markerY = firstLegendItemLocation.Y + Offset.Y + markerVerticalOffsetFromItemLocation;
			int markerX;
			if (!isRightToLeft)
				markerX = firstLegendItemLocation.X + Offset.X;
			else
				markerX = firstLegendItemLocation.X + Offset.X + Size.Width + RightIndent - MarkerSize.Width;
			Point markerLocation = new Point(markerX, markerY);
			return new Rectangle(markerLocation, MarkerSize);
		}
		Rectangle ClaculateCheckBoxAreaBounds(Point firstLegendItemLocation, bool isRightToLeft) {
			Size checkBoxAreaSize = DefaultMarkerSize;
			int halfHeightDiff = (textSize.Height - checkBoxAreaSize.Height) / 2;
			int checkBoxVerticalOffsetFromItemLocation = halfHeightDiff < 0 ? 0 : halfHeightDiff;
			int checkBoxY = firstLegendItemLocation.Y + Offset.Y + checkBoxVerticalOffsetFromItemLocation;
			int checkBoxX;
			if (!isRightToLeft)
				checkBoxX = firstLegendItemLocation.X + Offset.X;
			else
				checkBoxX = firstLegendItemLocation.X + Offset.X + Size.Width + RightIndent - MarkerSize.Width;
			Point checkBoxLocation = new Point(checkBoxX, checkBoxY);
			return new Rectangle(checkBoxLocation, checkBoxAreaSize);
		}
		Point CalculateTextLocation(Point firstLegendItemLocation, Size markerOrCheckSize, int textOffset, bool isRightToLeft) {
			int halfHeightDiff = (textSize.Height - markerOrCheckSize.Height) / 2;
			int textVerticalOffsetFromItemLocation = halfHeightDiff < 0 ? -halfHeightDiff : 0;
			int textY = firstLegendItemLocation.Y + Offset.Y + textVerticalOffsetFromItemLocation;
			int textX;
			if (!isRightToLeft)
				textX = firstLegendItemLocation.X + Offset.X + markerOrCheckSize.Width + textOffset;
			else
				textX = firstLegendItemLocation.X + Offset.X + RightIndent;
			return new Point(textX, textY);
		}
		public void ChangeAppropriateObjectCheckedInLegendState() {
			var checkableLegendItem = (ICheckableLegendItemData)this.item;
			checkableLegendItem.CheckedInLegend = !checkableLegendItem.CheckedInLegend;
		}
		public void Render(IRenderer renderer, Point firstLegendItemLocation) {
			Size markerOrCheckSize = new Size();
			int textOffset = legend.TextOffset; 
			switch (MarkerView) {
				case LegendItemMarkerView.Marker:
					Rectangle markerBounds = CalculateMarkerBounds(firstLegendItemLocation, renderer.IsRightToLeft);
					item.RenderMarker(renderer, markerBounds);
					markerOrCheckSize = markerBounds.Size;
					break;
				case LegendItemMarkerView.Checkbox:
					Rectangle checkBoxAreaBounds = ClaculateCheckBoxAreaBounds(firstLegendItemLocation, renderer.IsRightToLeft);
					RenderCheckBox(renderer, checkBoxAreaBounds);
					markerOrCheckSize = checkBoxAreaBounds.Size;
					break;
				case LegendItemMarkerView.MarkerImageAsCheckbox:
					Rectangle imageBounds = CalculateMarkerBounds(firstLegendItemLocation, renderer.IsRightToLeft);
					renderer.ProcessHitTestRegion(HitTestController, legend, new ChartFocusedArea(this), new HitRegion(imageBounds), true);
					item.RenderMarker(renderer, imageBounds);
					markerOrCheckSize = imageBounds.Size;
					break;
				default:
					textOffset = 0;
					break;
			}
			if (item.TextVisible) {
				Point textLocation = CalculateTextLocation(firstLegendItemLocation, markerOrCheckSize, textOffset, renderer.IsRightToLeft);
				RenderLegendItemText(renderer, textLocation);
			}
		}
	}
	public enum LegendItemMarkerView {
		None,
		Marker,
		Checkbox,
		MarkerImageAsCheckbox 
	}
}
