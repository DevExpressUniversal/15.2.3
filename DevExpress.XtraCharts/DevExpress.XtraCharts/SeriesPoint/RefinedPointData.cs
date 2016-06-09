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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public sealed class RefinedPointData : ILegendItemData {
		readonly RefinedSeriesData seriesData;
		readonly RefinedPoint refinedPoint;
		readonly DrawOptions drawOptions;
		readonly DrawOptions legendDrawOptions;
		readonly bool markerVisible;
		readonly Size markerSize;
		readonly ChartImageSizeMode markerImageSizeMode;
		readonly bool textVisible;
		readonly Color textColor;
		readonly SeriesLabelViewData[] labelViewData;
		readonly string legendText = string.Empty;
		readonly SelectionState selectionState;
		bool disposeFont;
		bool disposeMarkerImage;
		BarData barData;
		Font font;
		Image markerImage;
		DiagramPoint? toolTipRelativePosition = null;
		Legend Legend { get { return Series.View.Chart.Legend; } }
		internal RefinedSeriesData SeriesData { get { return seriesData; } }
		public Series Series { get { return seriesData.Series; } }
		public bool DisposeFont { get { return disposeFont; } set { disposeFont = value; } }
		public bool DisposeMarkerImage { get { return disposeMarkerImage; } set { disposeMarkerImage = value; } }
		public bool IsValidLegendItem { get { return !refinedPoint.IsEmpty && !string.IsNullOrEmpty(legendText); } }
		public bool MarkerVisible { get { return markerVisible; } }
		public bool TextVisible { get { return textVisible; } }
		public Color TextColor { get { return textColor; } }
		public DrawOptions DrawOptions { get { return drawOptions; } }
		public DrawOptions LegendDrawOptions { get { return legendDrawOptions; } }
		public Font Font { get { return font; } }
		public Image MarkerImage { get { return markerImage; } }
		public ChartImageSizeMode MarkerImageSizeMode { get { return markerImageSizeMode; } }
		public ISeriesPoint SeriesPoint { get { return refinedPoint.SeriesPoint; } }
		public SeriesLabelViewData[] LabelViewData { get { return labelViewData; } }
		public Size MarkerSize { get { return markerSize; } }
		public string LegendText { get { return legendText; } }
		public RefinedPoint RefinedPoint { get { return refinedPoint; } }
		public DiagramPoint? ToolTipRelativePosition { get { return toolTipRelativePosition; } set { toolTipRelativePosition = value; } }
		public SelectionState SelectionState { get { return selectionState; } }
		public RefinedPointData(RefinedSeriesData seriesData, RefinedPoint refinedPoint, DrawOptions drawOptions, DrawOptions legendDrawOptions,
			bool markerVisible, Size markerSize, ChartImageSizeMode markerImageSizeMode, Image markerImage, bool disposeMarkerImage, bool textVisible, Color textColor,
			SeriesLabelViewData[] labelViewData, string legendText, Font legendFont, bool disposeLegendFont, SelectionState selectionState) {
			this.seriesData = seriesData;
			this.refinedPoint = refinedPoint;
			this.drawOptions = drawOptions; 
			this.legendDrawOptions = legendDrawOptions;
			Color color = seriesData.Series.ColorProvider.GetColor(refinedPoint.SeriesPoint);
			ColorProvider.SetupDrawOptions(color, this.drawOptions);
			ColorProvider.SetupDrawOptions(color, this.legendDrawOptions);
			this.markerVisible = markerVisible;
			this.markerSize = markerSize;
			this.markerImageSizeMode = markerImageSizeMode;
			this.markerImage = markerImage;
			this.disposeMarkerImage = disposeMarkerImage;
			this.textVisible = textVisible;
			this.textColor = textColor;
			this.labelViewData = labelViewData;
			this.legendText = legendText;
			this.font = legendFont;
			this.disposeFont = disposeLegendFont;
			this.selectionState = selectionState;
		}
		#region ILegendItem Implementation
		string ILegendItemData.Text { get { return legendText; } }
		object ILegendItemData.RepresentedObject { get { return refinedPoint; } }
		void ILegendItemData.RenderMarker(IRenderer renderer, Rectangle bounds) {
			Series.RenderLegendItem(renderer, bounds, legendDrawOptions, seriesData.LegendDrawOptions, null, this); 
		}
		#endregion
		public void DisposeCustomFont() {
			if (disposeFont && font != null && font != Legend.Font) {
				font.Dispose();
				font = null;
			}
		}
		public void DisposeCustomMarkerImage() {
			if (disposeMarkerImage && markerImage != null) {
				markerImage.Dispose();
				markerImage = null;
			}
		}
		public BarData GetBarData(BarSeriesView view) {
			if (barData == null)
				barData = view.CreateBarData(RefinedPoint);
			return barData;
		}
	}
}
