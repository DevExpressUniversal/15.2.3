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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public abstract class IndicatorBehavior : ICheckableLegendItemData {
		readonly Indicator indicator;
		bool visible;
		Color color;
		Legend Legend {
			get { return indicator.ChartContainer.Chart.Legend; }
		}
		public Indicator Indicator { 
			get { return indicator; } 
		}
		public bool Visible {
			get { return visible; }
			protected set { visible = value; }
		}
		public Color Color { 
			get { return color; } 
		}
		public virtual Color LegendColor { 
			get { return color; } 
		}
		public virtual LineStyle LegendLineStyle { 
			get { return indicator.LineStyle; } 
		}
		#region ICheckableLegendItem implementation
		bool ILegendItemData.DisposeFont { get { return false; } }
		bool ILegendItemData.DisposeMarkerImage { get { return false; } }
		bool ILegendItemData.MarkerVisible { get { return Legend.MarkerVisible; } }
		bool ILegendItemData.TextVisible { get { return Legend.TextVisible; } }
		string ILegendItemData.Text { get { return indicator.Name; } }
		Color ILegendItemData.TextColor { get { return Legend.ActualTextColor; } }
		Color ICheckableLegendItemData.MainColor { get { return this.color; } }
		Image ILegendItemData.MarkerImage { get { return null; } }
		ChartImageSizeMode ILegendItemData.MarkerImageSizeMode { get { return ChartImageSizeMode.AutoSize; } }
		Size ILegendItemData.MarkerSize { get { return Legend.MarkerSize; } }
		Font ILegendItemData.Font { get { return Legend.Font; } }
		object ILegendItemData.RepresentedObject { get { return indicator; } }
		void ILegendItemData.RenderMarker(IRenderer renderer, Rectangle bounds) {
			int lineLevel = bounds.Top + bounds.Height / 2;
			Point p1 = new Point(bounds.Left + 1, lineLevel);
			Point p2 = new Point(bounds.Right - 1, lineLevel);
			Color color = GraphicUtils.CorrectColorByHitTestState(LegendColor, ((IHitTest)indicator).State);
			renderer.EnableAntialiasing(LegendLineStyle.AntiAlias);
			renderer.DrawLine(p1, p2, color, 2, LegendLineStyle, LineCap.Flat);
			renderer.RestoreAntialiasing();
			renderer.ProcessHitTestRegion(indicator.ChartContainer.Chart.HitTestController, indicator, null, new HitRegion(bounds));
		}
		bool ICheckableLegendItemData.CheckedInLegend {
			get { return indicator.CheckedInLegend; }
			set { indicator.CheckedInLegend = value; }
		}
		bool ICheckableLegendItemData.Disabled {
			get { return !indicator.View.Series.CheckedInLegend; }
		}
		bool ICheckableLegendItemData.UseCheckBox {
			get { return Legend.UseCheckBoxes && indicator.CheckableInLegend; }
		}
		#endregion
		protected abstract void Calculate(IRefinedSeries seriesInfo);
		protected IndicatorBehavior(Indicator indicator) {
			this.indicator = indicator;
		}
		protected internal virtual void UpdateColor(Color generatedColor) {
			color = indicator.Color;
			if (color.IsEmpty)
				color = generatedColor;
		}
		public void Calculate(IRefinedSeries seriesInfo, IndicatorColorGenerator colorGenerator) {
			Calculate(seriesInfo);
			if (visible)
				UpdateColor(colorGenerator.GenerateColor());
		}
		public abstract IndicatorLayout CreateLayout(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer);
	}
}
