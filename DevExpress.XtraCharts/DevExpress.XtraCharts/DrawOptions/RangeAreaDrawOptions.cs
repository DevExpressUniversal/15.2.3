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
using System.ComponentModel;
namespace DevExpress.XtraCharts {
	public class RangeAreaDrawOptions : AreaDrawOptions {
		Marker marker1;
		Marker marker2;
		CustomBorder border1;
		CustomBorder border2;
		bool marker1Visible;
		bool marker2Visible;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("RangeAreaDrawOptionsMarker1")]
#endif
		public Marker Marker1 { get { return marker1; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("RangeAreaDrawOptionsMarker2")]
#endif
		public Marker Marker2 { get { return marker2; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("RangeAreaDrawOptionsBorder1")]
#endif
		public CustomBorder Border1 { get { return border1; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("RangeAreaDrawOptionsBorder2")]
#endif
		public CustomBorder Border2 { get { return border2; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("RangeAreaDrawOptionsMarker1Visible")]
#endif
		public bool Marker1Visible {
			get { return marker1Visible; }
			set { marker1Visible = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("RangeAreaDrawOptionsMarker2Visible")]
#endif
		public bool Marker2Visible {
			get { return marker2Visible; }
			set { marker2Visible = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new Marker Marker { get { return null; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new CustomBorder Border { get { return null; } }
		internal RangeAreaDrawOptions(RangeAreaSeriesView view) : base(view) {
			this.marker1 = (Marker)CreateMarkerFromPattern(view.Marker1);
			this.marker2 = (Marker)CreateMarkerFromPattern(view.Marker2);
			if (view.Border1 != null)
				border1 = (CustomBorder)view.Border1.Clone();
			if (view.Border2 != null)
				border2 = (CustomBorder)view.Border2.Clone();
		}
		protected RangeAreaDrawOptions() {
		}
		protected override DrawOptions CreateInstance() {
			return new RangeAreaDrawOptions();
		}
		protected override void DeepClone(object obj) {
			base.DeepClone(obj);
			RangeAreaDrawOptions drawOptions = obj as RangeAreaDrawOptions;
			if (drawOptions != null) {
				this.marker1 = (Marker)drawOptions.marker1.Clone();
				this.marker2 = (Marker)drawOptions.marker2.Clone();
				if (drawOptions.Border1 != null)
					this.border1 = (CustomBorder)drawOptions.border1.Clone();
				if (drawOptions.Border2 != null)
					this.border2 = (CustomBorder)drawOptions.border2.Clone();
			}
		}
		protected internal override void InitializeSeriesPointDrawOptions(SeriesViewBase view, IRefinedSeries seriesInfo, int pointIndex) {
			base.InitializeSeriesPointDrawOptions(view, seriesInfo, pointIndex);
			RangeAreaSeriesView areaView = view as RangeAreaSeriesView;
			if (areaView != null) {
				marker1Visible = areaView.ActualMarker1Visible;
				marker2Visible = areaView.ActualMarker2Visible;
			}
		}		
	}
}
