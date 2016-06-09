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
	public class RadarAxisYLabelViewData {
		readonly RadarAxisYViewData axisViewData;
		readonly AxisLabelItemList items;
		RadarAxisY Axis { get { return axisViewData.Axis; } }
		public AxisLabelItemList Items { get { return items; } }
		public RadarAxisYLabelViewData(TextMeasurer textMeasurer, RadarAxisYViewData axisViewData, RadarAxisYMapping mapping, GridAndTextDataEx gridAndTextData) {
			this.axisViewData = axisViewData;
			float angle;
			NearTextPosition nearPosition;
			CalculateAngleAndNearPosition(mapping, out angle, out nearPosition);
			IList<AxisTextItem> primaryItems = gridAndTextData.TextData.PrimaryItems;
			int labelsCount = primaryItems.Count;
			items = new AxisLabelItemList(labelsCount);
			for (int i = 0; i < labelsCount; i++) {
				AxisTextItem item = primaryItems[i];
				Point basePoint = (Point)mapping.GetDiagramPoint(item.Value, 0, (int)axisViewData.LabelOffset);
				items.Add(new RadarAxisYLabelItem(Axis.Label, basePoint, item, angle, nearPosition));
			}
			items.AdjustPropertiesAndCreatePainters(Axis, textMeasurer);
		}
		void CalculateAngleAndNearPosition(RadarAxisYMapping mapping, out float angle, out NearTextPosition nearPosition) {
			angle = (float)((RadarDiagram)Axis.Diagram).StartAngleInDegrees;
			if (mapping.DiagramMapping.RevertAngle) {
				angle = -angle;
				nearPosition = NearTextPosition.Left;
			}
			else
				nearPosition = NearTextPosition.Right;
		}
		public void UpdateCorrection(RectangleCorrection correction) {
			items.UpdateCorrection(correction);
		}
		public void Render(IRenderer renderer) {
			items.Render(renderer, Axis.Diagram.Chart.HitTestController);
		}
	}
}
