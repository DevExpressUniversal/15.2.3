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
using DevExpress.XtraCharts.Native;
using System.ComponentModel;
namespace DevExpress.XtraCharts {
	public class LineDrawOptions : PointDrawOptions {
		LineStyle lineStyle;
		bool markerVisible;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("LineDrawOptionsMarker")]
#endif
		public new Marker Marker { get { return (Marker)base.Marker; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("LineDrawOptionsLineStyle")]
#endif
		public LineStyle LineStyle { get { return this.lineStyle; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("LineDrawOptionsMarkerVisible")]
#endif
		public bool MarkerVisible {
			get { return markerVisible; }
			set { markerVisible = value; }
		}
		protected internal override bool ActualMarkerVisible { get { return MarkerVisible; } }
		internal LineDrawOptions(LineSeriesView view) : base(view) {
			lineStyle = (LineStyle)view.LineStyle.Clone();
		}
		internal LineDrawOptions(RadarLineSeriesView view) : base(view) {
			lineStyle = (LineStyle)view.LineStyle.Clone();
		}
		protected LineDrawOptions() {
		}
		protected override DrawOptions CreateInstance() {
			return new LineDrawOptions();
		}
		protected override void DeepClone(object obj) {
			base.DeepClone(obj);
			LineDrawOptions drawOptions = obj as LineDrawOptions;
			if(drawOptions != null)
				lineStyle = (LineStyle)drawOptions.lineStyle.Clone();
		}
		protected internal override void InitializeSeriesPointDrawOptions(SeriesViewBase view, IRefinedSeries seriesInfo, int pointIndex) {
			base.InitializeSeriesPointDrawOptions(view, seriesInfo, pointIndex);
			ILineSeriesView lineSeriesView = view as ILineSeriesView;
			if (lineSeriesView != null)
				MarkerVisible = lineSeriesView.MarkerVisible;
		}
	}
}
