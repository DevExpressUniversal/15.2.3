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
using DevExpress.Xpf.Charts.Native;
using System;
namespace DevExpress.Xpf.Charts {
	public class BarSideBySideSeries3D : BarSeries3D, ISideBySideBarSeriesView {
		protected override Type PointInterfaceType { get { return typeof(IValuePoint); } }
		protected override bool NeedSeriesGroupsInteraction { get { return true; } }
		public BarSideBySideSeries3D() {
			DefaultStyleKey = typeof(BarSideBySideSeries3D);
		}
		#region ISideBySideBarSeriesView
		double ISideBySideBarSeriesView.BarDistance {
			get { return Diagram is XYDiagram3D ? ((XYDiagram3D)Diagram).BarDistance : 0; }
			set {
				if (Diagram is XYDiagram3D) {
					((XYDiagram3D)Diagram).BarDistance = value;
				}
			}
		}
		int ISideBySideBarSeriesView.BarDistanceFixed {
			get { return Diagram is XYDiagram3D ? ((XYDiagram3D)Diagram).BarDistanceFixed : 0; }
			set {
				if (Diagram is XYDiagram3D) {
					((XYDiagram3D)Diagram).BarDistanceFixed = value;
				}
			}
		}
		bool ISideBySideBarSeriesView.EqualBarWidth {
			get { return Diagram is XYDiagram3D ? ((XYDiagram3D)Diagram).EqualBarWidth : true; }
			set {
				if (Diagram is XYDiagram3D) {
					((XYDiagram3D)Diagram).EqualBarWidth = value;
				}
			}
		}
		#endregion
		protected internal override SeriesData CreateSeriesData() {
			SeriesData = new BarSideBySideSeries3DData(this);
			return SeriesData;
		}
		protected override Series CreateObjectForClone() {
			return new BarSideBySideSeries3D();
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			return ((IValuePoint)point).Value;
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			return ((IValuePoint)point).Value;
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			return Math.Abs(((IValuePoint)point).Value);
		}
	}
}
