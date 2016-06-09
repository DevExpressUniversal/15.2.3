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

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraCharts.Native {
	public abstract class IndicatorLayout {
		readonly Indicator indicator;
		protected Color Color { get { return GraphicUtils.CorrectColorByHitTestState(Indicator.IndicatorBehavior.Color, ((IHitTest)Indicator).State); } }
		protected int Thickness { get { return GraphicUtils.CorrectThicknessByHitTestState(Indicator.LineStyle.Thickness, ((IHitTest)Indicator).State, 1); } }
		public Indicator Indicator { get { return indicator; } }
		public XYDiagramMappingBase DiagramMapping { get; set; }
		protected IndicatorLayout(Indicator indicator) {
			this.indicator = indicator;
		}
		public abstract void Render(IRenderer renderer);
		public abstract GraphicsPath CalculateHitTestGraphicsPath();
	}
	public class IndicatorLayoutList : List<IndicatorLayout> {
		readonly XYDiagramMappingList mappingList;
		public XYDiagramMappingList MappingList { get { return mappingList; } }
		public IndicatorLayoutList(XYDiagramMappingList mappingList) {
			this.mappingList = mappingList;
		}
		public void Initialize(Indicator indicator, TextMeasurer textMeasurer) {
			XYDiagram2DSeriesViewBase seriesView = ((XYDiagram2DSeriesViewBase)indicator.OwningSeries.View);
			Axis2D axisX = seriesView.ActualAxisX;
			Axis2D axisY;
			SeparatePaneIndicator separatePaneIndicator = indicator as SeparatePaneIndicator;
			if (separatePaneIndicator != null)
				axisY = separatePaneIndicator.ActualAxisY;
			else
				axisY = seriesView.ActualAxisY;
			XYDiagramMappingContainer mappingContainer = mappingList.GetMappingContainer(axisX, axisY);
			if (mappingContainer != null) {
				foreach (XYDiagramMappingBase mapping in mappingContainer) {
					IndicatorLayout indicatorLayout = indicator.IndicatorBehavior.CreateLayout(mapping, textMeasurer);
					if (indicatorLayout != null) {
						indicatorLayout.DiagramMapping = mapping;
						Add(indicatorLayout);
					}
				}
			}
		}
	}
}
