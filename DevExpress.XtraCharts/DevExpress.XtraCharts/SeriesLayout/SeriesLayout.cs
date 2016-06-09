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
	public abstract class SeriesLayout : List<SeriesPointLayout> {
		readonly RefinedSeriesData seriesData;
		protected virtual HitTestController HitTestController { get { return View.HitTestController; } }
		public RefinedSeriesData SeriesData { get { return seriesData; } }
		public SeriesViewBase View { get { return seriesData.Series.View; } }
		public virtual WholeSeriesLayout WholeLayout { get { return null; } }
		public virtual WholeSeriesViewData WholeViewData { get { return seriesData.WholeViewData; } }
		public virtual DrawOptions DrawOptions { get { return seriesData.DrawOptions; } }
		public SeriesLayout(RefinedSeriesData seriesData) {
			this.seriesData = seriesData;
		}
		protected virtual void RenderShadow(IRenderer renderer, Rectangle mappingBounds) {
			if (WholeLayout != null)
				View.RenderWholeSeriesShadow(renderer, mappingBounds, WholeLayout);
			foreach (SeriesPointLayout pointLayout in this)
				View.RenderShadow(renderer, mappingBounds, pointLayout, pointLayout.PointData.DrawOptions);
		}
		public abstract void Render(IRenderer renderer);
	}
	public abstract class WholeSeriesLayout {
		SeriesLayout seriesLayout;
		public SeriesLayout SeriesLayout { get { return seriesLayout; } }
		public WholeSeriesLayout(SeriesLayout seriesLayout) {
			this.seriesLayout = seriesLayout;
		}
		public virtual HitRegionContainer CalculateHitRegion(DrawOptions drawOptions) {
			return new HitRegionContainer();
		}
	}
	public abstract class SeriesPointLayout {
		readonly RefinedPointData pointData;
		public RefinedPointData PointData { get { return pointData; } }
		public virtual ISeriesPoint SeriesPoint { get { return pointData.RefinedPoint.SeriesPoint; } }
		public virtual DrawOptions DrawOptions { get { return pointData.DrawOptions; } }
		public virtual bool IsEmpty { get { return pointData.RefinedPoint.IsEmpty; } }
		public virtual SeriesLabelViewData[] LabelViewData { get { return pointData.LabelViewData; } }
		public virtual DiagramPoint? ToolTipRelativePosition { get { return pointData.ToolTipRelativePosition; } }
		public SeriesPointLayout(RefinedPointData pointData) {
			this.pointData = pointData;
		}
		public virtual HitRegionContainer CalculateHitRegion() {
			return new HitRegionContainer();
		}
	}
}
