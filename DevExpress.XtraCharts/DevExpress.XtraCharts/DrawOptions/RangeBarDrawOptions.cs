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

using System.ComponentModel;
using DevExpress.XtraCharts.Native;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	public class RangeBarDrawOptions : BarDrawOptions {
		Marker minValueMarker;
		Marker maxValueMarker;
		bool minValueMarkerVisible;
		bool maxValueMarkerVisible;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("RangeBarDrawOptionsMinValueMarker")]
#endif
		public Marker MinValueMarker { get { return this.minValueMarker; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("RangeBarDrawOptionsMaxValueMarker")]
#endif
		public Marker MaxValueMarker { get { return this.maxValueMarker; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("RangeBarDrawOptionsMinValueMarkerVisible")]
#endif
		public bool MinValueMarkerVisible {
			get { return minValueMarkerVisible; }
			set { minValueMarkerVisible = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("RangeBarDrawOptionsMaxValueMarkerVisible")]
#endif
		public bool MaxValueMarkerVisible {
			get { return maxValueMarkerVisible; }
			set { maxValueMarkerVisible = value; }
		}
		internal RangeBarDrawOptions(RangeBarSeriesView view)
			: base(view) {
			this.minValueMarker = CreateMarkerFromPattern(view.MinValueMarker);
			this.maxValueMarker = CreateMarkerFromPattern(view.MaxValueMarker);
		}
		protected RangeBarDrawOptions() {
		}
		Marker CreateMarkerFromPattern(Marker patternMarker) {
			Marker marker = (Marker)patternMarker.Clone();
			if (patternMarker.FillStyle.FillMode == FillMode.Empty)
				marker.FillStyle.Assign(CommonUtils.GetActualAppearance(patternMarker).MarkerAppearance.FillStyle);
			return marker;
		}
		protected override DrawOptions CreateInstance() {
			return new RangeBarDrawOptions();
		}
		protected override void DeepClone(object obj) {
			base.DeepClone(obj);
			RangeBarDrawOptions drawOptions = obj as RangeBarDrawOptions;
			if(drawOptions != null) {
				this.minValueMarker = (Marker)drawOptions.minValueMarker.Clone();
				this.maxValueMarker = (Marker)drawOptions.maxValueMarker.Clone();
			}
		}
		protected internal override void InitializeSeriesPointDrawOptions(SeriesViewBase view, Charts.Native.IRefinedSeries seriesInfo, int pointIndex) {
			base.InitializeSeriesPointDrawOptions(view, seriesInfo, pointIndex);
			RangeBarSeriesView barView = view as RangeBarSeriesView;
			if (barView != null) {
				minValueMarkerVisible = barView.ActualMinValueMarkerVisible;
				maxValueMarkerVisible = barView.ActualMaxValueMarkerVisible;
			}
		}		
	}
}
