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
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class XYDiagram3DSeriesLayout : SeriesLayout {
		XYDiagram3DCoordsCalculator coordsCalculator;
		XYDiagram3DWholeSeriesLayout wholeLayout;
		public new XYDiagram3DSeriesViewBase View { get { return (XYDiagram3DSeriesViewBase)SeriesData.Series.View; } }		
		public XYDiagram3DSeriesLayout(RefinedSeriesData seriesData, XYDiagram3DCoordsCalculator coordsCalculator) : base(seriesData) {
			this.coordsCalculator = coordsCalculator;
		}
		public void Calculate() {
			foreach (RefinedPointData pointData in SeriesData)
				Add(pointData.RefinedPoint.IsEmpty ? null : View.CalculateSeriesPointLayout(coordsCalculator, pointData));
			wholeLayout = View.CalculateWholeSeriesLayout(coordsCalculator, this);
		}
		public void FillPrimitivesContainer(PrimitivesContainer container) {
			View.FillPrimitivesContainer(wholeLayout, container);
			foreach(SeriesPointLayout pointLayout in this)
				View.FillPrimitivesContainer(pointLayout, container);
		}
		public override void Render(IRenderer renderer) {
		}
	}
	public class XYDiagram3DWholeSeriesLayout { }
	public class View3DSeriesPointLayout : SeriesPointLayout {
		IList<Line> lines = new List<Line>();
		IList<PlanePolygon> polygons = new List<PlanePolygon>();
		public IList<Line> Lines { get { return lines; } }
		public IList<PlanePolygon> Polygons { get { return polygons; } }
		public View3DSeriesPointLayout(RefinedPointData pointData) : base(pointData) {
		}
		public View3DSeriesPointLayout(RefinedPointData pointData, IList<Line> lines) : this(pointData) {
			if(lines != null)
				this.lines = lines;
		}
		public View3DSeriesPointLayout(RefinedPointData pointData, IList<PlanePolygon> polygons) : this(pointData) {
			if(polygons != null)
				this.polygons = polygons;
		}
	}
}
