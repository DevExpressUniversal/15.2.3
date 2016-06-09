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

using DevExpress.Charts.Native;
using System.Collections.Generic;
namespace DevExpress.XtraCharts.Native {
	public class AxisIntervalLayoutCache : Dictionary<XYDiagramPaneBase, List<AxisIntervalLayout>> {
		public void SetAxisIntervalLayout(XYDiagramPaneBase pane, IEnumerable<AxisIntervalLayout> intervalLayouts) {
			if (pane != null) {
				List<AxisIntervalLayout> intervals;
				if (ContainsKey(pane)) {
					intervals = this[pane];
					intervals.Clear();
				}
				else {
					intervals = new List<AxisIntervalLayout>();
					Add(pane, intervals);
				}
				intervals.AddRange(intervalLayouts);				
			}
		}
		public double? GetAxisValue(XYDiagramPaneBase pane, ITransformation transformation, int coordinate) {
			AxisIntervalLayout axisIntervalLayout = GetIntervalBounds(pane, coordinate);
			if (axisIntervalLayout == null)
				return null;
			AxisInterval interval = axisIntervalLayout.Interval;
			Interval bounds = axisIntervalLayout.Bounds;
			double ratio = (double)(coordinate - bounds.Start) / bounds.Length;
			return interval.GetInternalValue(ratio, transformation);
		}
		AxisIntervalLayout GetIntervalBounds(XYDiagramPaneBase pane, int location) {
			if (ContainsKey(pane)) {
				List<AxisIntervalLayout> axisIntervalLayouts = this[pane];
				for (int index = 0; index < axisIntervalLayouts.Count; index++) {
					AxisIntervalLayout layout = axisIntervalLayouts[index];
					if (layout.Bounds.IsIn(location))
						return layout;
				}
			}
			return null;
		}
	}
}
