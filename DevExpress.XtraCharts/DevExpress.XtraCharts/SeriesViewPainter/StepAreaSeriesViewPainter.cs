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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class StepAreaSeriesViewPainter : AreaSeriesViewPainter {
		protected override bool CanOptimizePolygons { get { return false; } }
		public static RangeStrip CreateStepAreaMarkerStrip(IGeometryStripCreator creator, Rectangle bounds, bool invertedStep) {
			RangeStrip strip = creator.CreateStrip() as RangeStrip;
			if (strip != null) {
				LineStrip topStrip = StripsUtils.CalcLegendStepLinePoints(bounds, invertedStep);
				LineStrip bottomStrip = new LineStrip();
				foreach (GRealPoint2D point in topStrip)
					bottomStrip.Add(new GRealPoint2D(point.X, bounds.Bottom));
				strip.TopStrip = topStrip;
				strip.BottomStrip = bottomStrip;
			}
			return strip;
		}
		StepAreaSeriesView StepAreaView { get { return (StepAreaSeriesView)View; } }
		public StepAreaSeriesViewPainter(StepAreaSeriesView view) : base(view) { }
		protected override Rectangle CalculateBoundsForLegendMarker(Rectangle bounds) {
			return GraphicUtils.InflateRect(bounds, -2, -2);
		}
		protected override RangeStrip CreateLegendMarkerStrip(Rectangle bounds, bool pointMarkerVisible) {
			return CreateStepAreaMarkerStrip(View, bounds, StepAreaView.InvertedStep);
		}
	}
}
