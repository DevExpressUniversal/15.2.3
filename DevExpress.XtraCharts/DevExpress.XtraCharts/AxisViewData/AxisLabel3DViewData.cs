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
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class AxisLabel3DViewData {
		readonly Axis3D axis;
		readonly AxisLabelItemList items;
		public AxisLabel3DViewData(Axis3D axis, TextMeasurer textMeasurer, XYDiagram3DCoordsCalculator coordsCalculator, AxisTextDataEx textData) {
			this.axis = axis;
			items = CalculateItems(textMeasurer, coordsCalculator, textData.PrimaryItems, 0);
			IList<AxisTextItem> staggeredItems = textData.StaggeredItems;
			if (staggeredItems.Count > 0) {
				int primaryExtent = 0;
				foreach (AxisLabel3DItem item in items) {
					int elementSize = axis.GetTextSize(item.RoundedBounds);
					if (elementSize > primaryExtent)
						primaryExtent = elementSize;
				}
				primaryExtent += 2;
				items.AddRange(CalculateItems(textMeasurer, coordsCalculator, staggeredItems, primaryExtent));
			}
		}
		AxisLabelItemList CalculateItems(TextMeasurer textMeasurer, XYDiagram3DCoordsCalculator coordsCalculator, IList<AxisTextItem> textItems, int extent) {
			AxisLabel label = axis.Label;
			AxisLabelItemList items = new AxisLabelItemList(textItems.Count);
			foreach (AxisTextItem textItem in textItems) {
				Point basePoint = (Point)coordsCalculator.Project(axis.GetLabelPoint(coordsCalculator, textItem.Value, -extent));
				items.Add(new AxisLabel3DItem(coordsCalculator, label, basePoint, textItem));
			}
			items.AdjustPropertiesAndCreatePainters(axis, textMeasurer);
			return items;
		}
		public void Render(IRenderer renderer) {
			items.Render(renderer, axis.Diagram.Chart.HitTestController);
		}
	}
}
