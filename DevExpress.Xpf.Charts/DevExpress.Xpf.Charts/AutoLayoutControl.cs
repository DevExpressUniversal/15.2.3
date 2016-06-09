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
using System.Windows;
using DevExpress.Charts.Native;
using System.ComponentModel;
namespace DevExpress.Xpf.Charts.Native {
	public enum InvalidateMeasureFlags : int {
		MeasureDiagram = 1,
		Legend = 2,
		Titles = 4,
		ArrangeDiagram = 8
	}
	public class AutoLayoutController {
		GRealSize2D minSize = new GRealSize2D(200, 100);
		ChartControl chart;
		public void CalculateLayout(ChartControl chartControl, Size constraint) {
			this.chart = chartControl;
			if (chart.Diagram == null)
				return;
			constraint = RemovePaddings(constraint);
			List<ISupportVisibilityControlElement> chartElements = GetElementsForAutoLayout();
			List<VisibilityLayoutRegion> regions = chart.Diagram.GetElementsForAutoLayout(constraint);
			List<ISupportVisibilityControlElement> changed = new List<ISupportVisibilityControlElement>();
			if (!chart.ActualAutoLayout || DesignerProperties.GetIsInDesignMode(chart))
				SetDefaulteVisible(chartElements, regions);
			else {
				VisibilityCalculator calculator = new VisibilityCalculator(minSize);
				changed.AddRange(calculator.CalculateLayout(chartElements, regions));
			}
			if (changed.Count > 0) {
				chart.AddInvalidate(InvalidateMeasureFlags.MeasureDiagram | InvalidateMeasureFlags.Legend | InvalidateMeasureFlags.Titles);
				List<Title> titles = changed.ConvertAll((x) => { return x as Title; });
				if (titles != null && titles.Count > 0)
					ChartElementHelper.UpdateWithClearDiagramCache(chart);
			}
		}
		void SetDefaulteVisible(List<ISupportVisibilityControlElement> chartElements, List<VisibilityLayoutRegion> models) {
			List<ISupportVisibilityControlElement> elements = new List<ISupportVisibilityControlElement>();
			elements.AddRange(chartElements);
			foreach (VisibilityLayoutRegion model in models)
				elements.AddRange(model.Elements);
			foreach (ISupportVisibilityControlElement element in elements)
				element.Visible = true;
		}
		Size RemovePaddings(Size constraint) {
			double width = constraint.Width - chart.Padding.Left - chart.Padding.Right;
			if (width <= 0)
				width = 0;
			double height = constraint.Height - chart.Padding.Top - chart.Padding.Bottom;
			if (height <= 0)
				height = 0;
			constraint = new Size(width, height);
			return constraint;
		}
		List<ISupportVisibilityControlElement> GetElementsForAutoLayout() {
			List<ISupportVisibilityControlElement> elements = new List<ISupportVisibilityControlElement>();
			if (chart.Legend != null && !chart.Legend.Visible.HasValue &&
				(chart.Legend.HorizontalPosition == HorizontalPosition.RightOutside || chart.Legend.HorizontalPosition == HorizontalPosition.LeftOutside ||
				chart.Legend.VerticalPosition == VerticalPosition.BottomOutside || chart.Legend.VerticalPosition == VerticalPosition.TopOutside))
				elements.Add(chart.Legend);
			foreach (Title title in chart.Titles)
				if (!title.Visible.HasValue)
					elements.Add(title);
			return elements;
		}
	}
}
