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
using DevExpress.Charts.Native;
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Native {
	public class CrosshairSeriesLabel : IAnnotationShapeOwner, ISupportTextAntialiasing {
		#region Nested struct: LabelSizeCache
		struct LabelElementSizes {
			readonly double lineHeight;
			readonly SizeF elementSize;
			readonly SizeF headerSize;
			readonly SizeF footerSize;
			public double LineHeight { get { return lineHeight; } }
			public SizeF ElementSize { get { return elementSize; } }
			public SizeF HeaderSize { get { return headerSize; } }
			public SizeF FooterSize { get { return footerSize; } }
			public LabelElementSizes(double lineHeight, SizeF textSize, SizeF headerSize, SizeF footerSize) {
				this.lineHeight = lineHeight;
				this.elementSize = textSize;
				this.headerSize = headerSize;
				this.footerSize = footerSize;
			}
		}
		#endregion
		const int DefatilTailHeight = 20;
		static readonly RectangleIndents LabelIndents = new RectangleIndents(null) { Left = 12, Top = 8, Right = 12, Bottom = 8 };
		static readonly RectangleIndents MarkerIndents = new RectangleIndents(null) { Left = 0, Top = 1, Right = 2, Bottom = 1 };
		static readonly RectangleIndents ItemIndents = new RectangleIndents(null) { Left = 0, Top = 3, Right = 0, Bottom = 3 };
		readonly Color borderColor;
		readonly Color backColor;
		readonly RectangleFillStyle fillStyle;
		readonly RoundedRectangleAnnotationShape shape;
		readonly TextAnnotationAppearance textAppearance;
		readonly bool showTail;
		internal TextAnnotationAppearance TextAppearence { get { return textAppearance; } }
		int TailHeight { get { return showTail ? DefatilTailHeight : 0; } }
		public bool BorderVisible { get { return true; } }
		public int BorderThickness { get { return 1; } }
		public Color BorderColor { get { return borderColor; } }
		public RectangleFillStyle FillStyle { get { return fillStyle; } }
		public Color BackColor { get { return backColor; } }
		public int ShapeFillet { get { return 3; } }
		public Shadow Shadow { get { return null; } }
		public AnnotationConnectorStyle ConnectorStyle { get { return showTail ? AnnotationConnectorStyle.Tail : AnnotationConnectorStyle.None; } }
		public int Angle { get { return 0; } }
		public CrosshairSeriesLabel(TextAnnotationAppearance textAppearance, bool showTile) {
			this.textAppearance = textAppearance;
			this.borderColor = textAppearance.BorderColor;
			this.backColor = textAppearance.BackColor;
			this.fillStyle = textAppearance.FillStyle;
			this.showTail = showTile;
			this.shape = new RoundedRectangleAnnotationShape(this);
		}
		#region IHitTest implemetation
		object IHitTest.Object { get { return null; } }
		HitTestState IHitTest.State { get { return new HitTestState(); } }
		#endregion
		#region ISupportTextAntialiasing implementation
		DefaultBoolean ISupportTextAntialiasing.EnableAntialiasing { get { return DefaultBoolean.Default; } }
		bool ISupportTextAntialiasing.DefaultAntialiasing { get { return false; } }
		Color ISupportTextAntialiasing.TextBackColor { get { return backColor; } }
		RectangleFillStyle ISupportTextAntialiasing.TextBackFillStyle { get { return fillStyle; } }
		bool ISupportTextAntialiasing.Rotated { get { return false; } }
		ChartElement ISupportTextAntialiasing.BackElement { get { return null; } }
		#endregion
		void RenderHeaderOrFooterText(IRenderer renderer, RectangleF headerOrFooterBounds, string text, Font font, Color textColor) {
			if (!String.IsNullOrEmpty(text)) 
				using (NativeFont nativeFont = new NativeFont(font))
					renderer.DrawBoundedText(text, nativeFont, textColor, this, headerOrFooterBounds, StringAlignment.Near, StringAlignment.Center);
		}
		GRealSize2D CalcWholeLabelSizes(List<CrosshairElementGroup> elementGroups, CrosshairLabelInfoEx seriesLabelInfo, TextMeasurer textMeasurer, Dictionary<CrosshairLabelElement, LabelElementSizes> labelElementSizesCache, Dictionary<CrosshairGroupHeaderElement, SizeF> headerSizeCache) {
			GRealSize2D size = new GRealSize2D(0, 0);
			foreach (CrosshairElementGroup elementGroup in elementGroups) {
				if (elementGroup.Label != seriesLabelInfo)
					continue;
				bool isGroupHeaderVisible = elementGroup.HeaderElement.ActualVisibility;
				foreach (CrosshairElement element in elementGroup.CrosshairElements) { 
					CrosshairLabelElement labelElement = element.LabelElement;		 
					if (!element.Visible || !labelElement.Visible)
						continue;
					LabelElementSizes labelElementSizes = CalcLabelElementSizes(textMeasurer, labelElement);
					labelElementSizesCache.Add(labelElement, labelElementSizes);
					SizeF labelElementSize = labelElementSizes.ElementSize;
					size.Height += labelElementSize.Height;
					size.Height += labelElementSizes.HeaderSize.Height;
					size.Height += labelElementSizes.FooterSize.Height;
					size.Width = Math.Max(labelElementSize.Width, size.Width);
				}
				if (isGroupHeaderVisible) {
					CrosshairGroupHeaderElement groupHeaderElement = elementGroup.HeaderElement;
					SizeF groupHeaderTextSize = textMeasurer.MeasureString(groupHeaderElement.Text, groupHeaderElement.Font);
					groupHeaderTextSize = ItemIndents.IncreaseSizeF(groupHeaderTextSize);
					size.Height += groupHeaderTextSize.Height;
					size.Width = Math.Max(groupHeaderTextSize.Width, size.Width);
					headerSizeCache.Add(groupHeaderElement, groupHeaderTextSize);
				}
			}
			size = LabelIndents.IncreaseSizeF(size);
			size.Height += TailHeight;
			return size;
		}
		bool IsVisible(CrosshairLabelInfoEx label, List<CrosshairElementGroup> elementGroups) {
			foreach (CrosshairElementGroup elementGroup in elementGroups) {
				if (elementGroup.Label == label)
					foreach (CrosshairElement element in elementGroup.CrosshairElements)
						if (element.Visible && !element.LabelElement.Empty)
							return true;
			}
			return false;
		}
		Rectangle CalculateMarkerBounds(LabelElementSizes labelElementSizes, RectangleF bounds, Size markerSize, bool isRightToLeft) {
			int x;
			if (!isRightToLeft)
				x = MathUtils.StrongRound(bounds.X) + MarkerIndents.Left;
			else
				x = MathUtils.StrongRound(bounds.X + bounds.Width - markerSize.Width) - MarkerIndents.Left;
			int y = MathUtils.StrongRound(bounds.Y + labelElementSizes.LineHeight * 0.5 - markerSize.Height * 0.5 + ItemIndents.Top);
			Rectangle markerBounds = new Rectangle(x, y, markerSize.Width, markerSize.Height);
			return markerBounds;
		}
		SeriesViewBase GetView(IRefinedSeries refinedSeries) {
			ISeries series = refinedSeries.Series;
			return series != null ? series.SeriesView as SeriesViewBase : null;
		}
		RectangleF CalcWholeLabelBounds(AnnotationLocation location, GRealRect2D bounds) {
			RectangleF labelBounds = new RectangleF((float)bounds.Left, (float)bounds.Top, (float)bounds.Width, (float)bounds.Height);
			if (location == AnnotationLocation.TopLeft || location == AnnotationLocation.TopRight)
				labelBounds = new RectangleF((float)bounds.Left, (float)bounds.Top, (float)bounds.Width, (float)Math.Max(bounds.Height - TailHeight, 0.0));
			if (location == AnnotationLocation.BottomLeft || location == AnnotationLocation.BottomRight)
				labelBounds = new RectangleF((float)bounds.Left, (float)(bounds.Top + TailHeight), (float)bounds.Width, (float)Math.Max(bounds.Height - TailHeight, 0));
			return labelBounds;
		}
		LabelElementSizes CalcLabelElementSizes(TextMeasurer textMeasurer, CrosshairLabelElement labelElement) {
			SizeF textSize;
			SizeF headerSize;
			SizeF footerSize;
			double firstLineHeight;
			if (labelElement.TextVisible) {
				string text = labelElement.Text;
				textSize = textMeasurer.MeasureString(text, labelElement.Font);
				firstLineHeight = GetTextHeight(textMeasurer, text, labelElement.Font, textSize.Height);
			}
			else {
				textSize = new SizeF(0, 0);
				firstLineHeight = 0;
			}
			if (!String.IsNullOrEmpty(labelElement.HeaderText))
				headerSize = textMeasurer.MeasureString(labelElement.HeaderText, labelElement.Font);
			else
				headerSize = new SizeF(0, 0);
			if (!String.IsNullOrEmpty(labelElement.FooterText))
				footerSize = textMeasurer.MeasureString(labelElement.FooterText, labelElement.Font);
			else
				footerSize = new SizeF(0, 0);
			Size markerSize = MarkerIndents.IncreaseSize(labelElement.MarkerSize);
			firstLineHeight = Math.Max(firstLineHeight, markerSize.Height);
			textSize.Height = Math.Max(textSize.Height, markerSize.Height);
			textSize.Width += markerSize.Width;
			float maxTextWidth = Math.Max(textSize.Width, headerSize.Width);
			maxTextWidth = Math.Max(maxTextWidth, footerSize.Width);
			textSize.Width = headerSize.Width = footerSize.Width = maxTextWidth;
			textSize = ItemIndents.IncreaseSizeF(textSize);
			if (!String.IsNullOrEmpty(labelElement.HeaderText))
				headerSize = ItemIndents.IncreaseSizeF(headerSize);
			if (!String.IsNullOrEmpty(labelElement.FooterText))
				footerSize = ItemIndents.IncreaseSizeF(footerSize); 
			return new LabelElementSizes(firstLineHeight, textSize, headerSize, footerSize);
		}
		double GetTextHeight(TextMeasurer textMeasurer, string text, Font font, float defaultHeight) {
			string[] lines = text.Split('\n');
			return lines.Length > 0 ? textMeasurer.MeasureString(lines[0], font).Height : defaultHeight;
		}
		void RenderMarkerAndText(CrosshairLabelElement labelElement, IRenderer renderer, IRefinedSeries refinedSeries, RefinedPoint refinedPoint, LabelElementSizes labelElementSizes, RectangleF elementBounds) {			
			SeriesViewBase view = GetView(refinedSeries);
			RectangleF textBounds;
			if (view != null) {
				Rectangle markerBounds = CalculateMarkerBounds(labelElementSizes, elementBounds, labelElement.MarkerSize, renderer.IsRightToLeft);
				if (labelElement.MarkerVisible) {
					if (labelElement.MarkerImage != null)
						renderer.DrawImage(labelElement.MarkerImage, markerBounds, labelElement.MarkerImageSizeMode);
					else {
						view.RenderCrosshairMarker(renderer, markerBounds, refinedSeries, refinedPoint, labelElement.MarkerColor);
					}
				}
				markerBounds = MarkerIndents.IncreaseRectangle(markerBounds);
				float x;
				if (!renderer.IsRightToLeft)
					x = elementBounds.Left + markerBounds.Width;
				else
					x = elementBounds.Left;
				textBounds = new RectangleF(x, elementBounds.Y, Math.Max(elementBounds.Width - markerBounds.Width, 0), elementBounds.Height);
			}
			else
				textBounds = elementBounds;
			if (labelElement.TextVisible)
				using (NativeFont nativeFont = new NativeFont(labelElement.Font))
					renderer.DrawBoundedText(labelElement.Text, nativeFont, labelElement.TextColor, this, textBounds, StringAlignment.Near, StringAlignment.Center);
		}
		void RenderCrosshairLabelElement(CrosshairLabelElement labelElement, IRenderer renderer, IRefinedSeries refinedSeries, RefinedPoint refinedPoint, LabelElementSizes labelElementSizes, RectangleF wholeLabelBounds, float offsetY) {
			float headerHeight = labelElementSizes.HeaderSize.Height;
			RectangleF headerBounds = new RectangleF(wholeLabelBounds.X, wholeLabelBounds.Y + offsetY, wholeLabelBounds.Width, headerHeight);
			RenderHeaderOrFooterText(renderer, headerBounds, labelElement.HeaderText, labelElement.Font, labelElement.TextColor);
			offsetY += headerHeight;
			SizeF markerAndTextSize = labelElementSizes.ElementSize; 
			float x;
			if (renderer.IsRightToLeft)
				x = wholeLabelBounds.Right - markerAndTextSize.Width;
			else
				x = wholeLabelBounds.Left;
			RectangleF markerAndTextBounds = new RectangleF(x, wholeLabelBounds.Y + offsetY, markerAndTextSize.Width, markerAndTextSize.Height);
			RenderMarkerAndText(labelElement, renderer, refinedSeries, refinedPoint, labelElementSizes, markerAndTextBounds);
			offsetY += markerAndTextSize.Height;
			float footerHeight = labelElementSizes.FooterSize.Height;
			RectangleF footerBounds = new RectangleF(wholeLabelBounds.X, wholeLabelBounds.Y + offsetY, wholeLabelBounds.Width, footerHeight);
			RenderHeaderOrFooterText(renderer, footerBounds, labelElement.FooterText, labelElement.Font, labelElement.TextColor);
		}
		public void Render(IRenderer renderer, CrosshairPaneInfoEx crosshairPaneInfo, TextMeasurer textMeasurer, List<CrosshairElementGroup> elementGroups) {
			var labelSizeCache  = new Dictionary<CrosshairLabelElement, LabelElementSizes>();
			var headerSizeCache = new Dictionary<CrosshairGroupHeaderElement, SizeF>();
			foreach (CrosshairLabelInfoEx seriesLabelInfo in crosshairPaneInfo.LabelsInfo)
				seriesLabelInfo.Size = CalcWholeLabelSizes(elementGroups, seriesLabelInfo, textMeasurer, labelSizeCache, headerSizeCache);
			crosshairPaneInfo.CompleteLabelsLayout();
			foreach (CrosshairLabelInfoEx seriesLabelInfo in crosshairPaneInfo.LabelsInfo) {
				if (!IsVisible(seriesLabelInfo, elementGroups))
					continue;
				RectangleF wholeLabelBounds = CalcWholeLabelBounds(seriesLabelInfo.Location, seriesLabelInfo.Bounds);
				shape.Render(renderer, (ZPlaneRectangle)wholeLabelBounds, new DiagramPoint(seriesLabelInfo.AnchorPoint.X, seriesLabelInfo.AnchorPoint.Y));
				wholeLabelBounds = LabelIndents.DecreaseRectangle(wholeLabelBounds);
				float offsetY = 0;
				foreach (CrosshairElementGroup elementGroup in elementGroups) {
					if (elementGroup.Label != seriesLabelInfo)
						continue;
					CrosshairGroupHeaderElement groupHeaderElement = elementGroup.HeaderElement;
					if (groupHeaderElement.ActualVisibility) {
						float headerHeight = headerSizeCache[groupHeaderElement].Height;
						RectangleF headerBounds = new RectangleF(wholeLabelBounds.X, wholeLabelBounds.Y + offsetY, wholeLabelBounds.Width, headerHeight);
						RenderHeaderOrFooterText(renderer, headerBounds, groupHeaderElement.Text, groupHeaderElement.Font, groupHeaderElement.TextColor);
						offsetY += headerHeight;
					}
					foreach (CrosshairElement element in elementGroup.CrosshairElements) {
						CrosshairLabelElement labelElement = element.LabelElement;
						if (!element.Visible || !labelElement.Visible)
							continue;
						LabelElementSizes labelElementSizes = labelSizeCache[labelElement];
						RenderCrosshairLabelElement(labelElement, renderer, element.RefinedSeries, element.RefinedPoint, labelElementSizes, wholeLabelBounds, offsetY);
						offsetY += labelElementSizes.HeaderSize.Height + labelElementSizes.ElementSize.Height + labelElementSizes.FooterSize.Height;
					}
				}
			}
		}
	}
}
