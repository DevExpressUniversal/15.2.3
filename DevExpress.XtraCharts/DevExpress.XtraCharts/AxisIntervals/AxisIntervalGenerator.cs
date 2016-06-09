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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class AxisIntervalsGenerator {
		class Edge : IComparable<Edge> {
			public static Edge CreateMinEdge(double value) {
				return new Edge(value, false);
			}
			public static Edge CreateMaxEdge(double value) {
				return new Edge(value, true);
			}
			readonly double value;
			readonly bool isMaxEdge;
			public double Value { get { return value; } }
			Edge(double value, bool isMaxEdge) {
				this.value = value;
				this.isMaxEdge = isMaxEdge;
			}
			int IComparable<Edge>.CompareTo(Edge edge) {
				int result = Comparer<double>.Default.Compare(value, edge.value);
				if (result == 0) {
					if (isMaxEdge && !edge.isMaxEdge)
						result++;
					else if (!isMaxEdge && edge.isMaxEdge)
						result--;
				}
				return result;
			}
			public void UpdateCounter(ref int counter) {
				if(isMaxEdge)
					counter--;
				else
					counter++;
			}
		}
		readonly Axis axis;
		readonly double minLimit;
		readonly double maxLimit;
		public AxisIntervalsGenerator(Axis axis, double minLimit, double maxLimit) {
			this.axis = axis;
			this.minLimit = minLimit;
			this.maxLimit = maxLimit;
		}
		IList<IMinMaxValues> EdgesToIntervalLimits(IList<Edge> edges) {
			List<IMinMaxValues> intervalLimitsList = new List<IMinMaxValues>();
			int counter = 1;
			double? firstValue = null;
			foreach(Edge edge in edges) {
				edge.UpdateCounter(ref counter);
				if(counter == 0) {
					firstValue = edge.Value;
					continue;
				}
				if(counter == 1 && firstValue != null && firstValue < edge.Value)
					intervalLimitsList.Add(new MinMaxValues((double)firstValue, edge.Value));
				firstValue = null;
			}
			return intervalLimitsList;
		}
		IList<Edge> CreateEdges() {
			List<Edge> edges = new List<Edge>();
			foreach (IScaleDiapason scaleDiapason in axis.ScaleDiapasons) {
				if (!scaleDiapason.Visible)
					continue;
				double value1 = scaleDiapason.Edge1;
				double value2 = scaleDiapason.Edge2;
				double minValue = Math.Min(value1, value2);
				double maxValue = Math.Max(value1, value2);
				if (minValue <= minLimit && maxValue >= maxLimit)
					return null;
				if (minValue <= minLimit && maxValue <= minLimit)
					continue;
				if (minValue >= maxLimit && maxValue >= maxLimit)
					continue;
				if (minValue < minLimit)
					minValue = minLimit;
				if (maxValue > maxLimit)
					maxValue = maxLimit;
				edges.Add(Edge.CreateMinEdge(minValue));
				edges.Add(Edge.CreateMaxEdge(maxValue));
			}
			edges.Add(Edge.CreateMinEdge(maxLimit));
			edges.Add(Edge.CreateMaxEdge(minLimit));
			edges.Sort();
			return edges;
		}
		public IList<IMinMaxValues> GenerateIntervalLimits() {
			if(minLimit == maxLimit)
				return null;
			IList<Edge> edges = CreateEdges();
			return edges != null ? EdgesToIntervalLimits(edges) : null;
		}
		public IList<AxisInterval> GenerateIntervals() {
			if (axis.ScaleDiapasons.Count < 1)
				return null;
			IList<IMinMaxValues> intervalLimitsList = GenerateIntervalLimits();
			if(intervalLimitsList == null)
				return null;
			List<AxisInterval> intervals = new List<AxisInterval>();
			foreach(IMinMaxValues intervalLimits in intervalLimitsList)
				intervals.Add(new AxisInterval( intervalLimits, intervalLimits));
			return intervals;
		}
	}
}
