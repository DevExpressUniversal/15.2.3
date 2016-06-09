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
using System.Windows.Media;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class Slice {
		readonly double startAngle;
		readonly double finishAngle;
		public double StartAngle { get { return startAngle; } }
		public double FinishAngle { get { return finishAngle; } }
		public double MedianAngle { get { return 0.5 * (startAngle + finishAngle); } }
		public bool IsEmpty { get; private set; }
		public Slice(double startAngle, double finishAngle, bool isEmpty) {
			this.startAngle = startAngle;
			this.finishAngle = finishAngle;
			this.IsEmpty = isEmpty;
		}
	}
	public class Pie {
		readonly Dictionary<ISeriesPoint, Slice> pointSlices = new Dictionary<ISeriesPoint, Slice>();
		public Slice this[ISeriesPoint point] {
			get {
				Slice slice;
				return pointSlices.TryGetValue(point, out slice) ? slice : null;
			}
		}
		public Pie(IRefinedSeries series, PieSweepDirection sweepDirection)
			: this(series, sweepDirection, 360) {
		}
		public Pie(IRefinedSeries series, PieSweepDirection sweepDirection, double sweepAngle) {
			if (series.Points.Count > 0) {
				double startAngle = sweepDirection == PieSweepDirection.Clockwise ? 0 : 360;
				for (int i = 0; i < series.Points.Count; i++) {
					double normalizedValue = ((IPiePoint)series.Points[i]).NormalizedValue;
					double finishAngle;
					if (i == series.Points.Count - 1)
						finishAngle = sweepDirection == PieSweepDirection.Clockwise ? sweepAngle : 360 - sweepAngle;
					else
						finishAngle = sweepDirection == PieSweepDirection.Clockwise ? startAngle + sweepAngle * normalizedValue : startAngle - sweepAngle * normalizedValue;
					pointSlices.Add(series.Points[i].SeriesPoint, new Slice(startAngle, finishAngle, normalizedValue == 0));
					startAngle = finishAngle;
				}
			}
		}
	}
}
