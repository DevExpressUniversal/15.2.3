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
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Sparkline;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Native;
namespace DevExpress.XtraEditors {
	public class LineChartRangeControlClientView : ChartRangeControlClientView {
		const int defaultMarkerSize = 5;
		const int defaultLineWidth = 1;
		const bool defaultShowMarkers = false;
		const bool defaultEnableAntialiasing = true;
		internal static Color DefaultMarkerColor = Color.Empty;
		internal new LineSparklineView SparklineView {
			get { return (LineSparklineView)base.SparklineView; }
		}
		[
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LineChartRangeControlClientView.MarkerColor"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LineChartRangeControlClientViewMarkerColor"),
#endif
		Editor(typeof(ChartRangeControlClientTemplatedColorTypeEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(ChartRangeControlClientTemplatedColorTypeConverter))
		]
		public Color MarkerColor {
			get { return SparklineView.MarkerColor; }
			set {
				if (SparklineView.MarkerColor != value) {
					SparklineView.MarkerColor = value;
					RaiseChanged();
				}
			}
		}
		[
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LineChartRangeControlClientView.ShowMarkers"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LineChartRangeControlClientViewShowMarkers")
#else
	Description("")
#endif
		]
		public bool ShowMarkers {
			get { return SparklineView.ShowMarkers; }
			set {
				if (SparklineView.ShowMarkers != value) {
					SparklineView.ShowMarkers = value;
					RaiseChanged();
				}
			}
		}
		[
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LineChartRangeControlClientView.MarkerSize"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LineChartRangeControlClientViewMarkerSize")
#else
	Description("")
#endif
		]
		public int MarkerSize {
			get { return SparklineView.MarkerSize; }
			set {
				if (SparklineView.MarkerSize != value) {
					SparklineView.MarkerSize = value;
					RaiseChanged();
				}
			}
		}
		[
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LineChartRangeControlClientView.LineWidth"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LineChartRangeControlClientViewLineWidth")
#else
	Description("")
#endif
		]
		public int LineWidth {
			get { return SparklineView.LineWidth; }
			set {
				if (SparklineView.LineWidth != value) {
					SparklineView.LineWidth = value;
					RaiseChanged();
				}
			}
		}
		[
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.LineChartRangeControlClientView.EnableAnitaliasing"),
		]
		public bool EnableAntialiasing {
			get { return SparklineView.EnableAntialiasing; }
			set {
				if (SparklineView.EnableAntialiasing != value) {
					SparklineView.EnableAntialiasing = value;
					RaiseChanged();
				}
			}
		}
		public LineChartRangeControlClientView() : this(new LineSparklineView()) { }
		protected LineChartRangeControlClientView(LineSparklineView view) : base(view) {
			LineSparklineView sparklineView = SparklineView;
			sparklineView.EndPointMarkerSize = 0;
			sparklineView.HighlightNegativePoints = false;
			sparklineView.MaxPointMarkerSize = 0;
			sparklineView.MinPointMarkerSize = 0;
			sparklineView.NegativePointMarkerSize = 0;
			sparklineView.StartPointMarkerSize = 0;
			sparklineView.LineWidth = defaultLineWidth;
			sparklineView.MarkerColor = DefaultMarkerColor;
			sparklineView.MarkerSize = defaultMarkerSize;
			sparklineView.ShowMarkers = defaultShowMarkers;
			sparklineView.EnableAntialiasing = defaultEnableAntialiasing;
		}
		#region ShouldSerialize & Reset
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() ||
				ShouldSerializeMarkerColor() ||
				ShouldSerializeShowMarkers() ||
				ShouldSerializeMarkerSize() ||
				ShouldSerializeEnableAntialiasing() ||
				ShouldSerializeLineWidth();
		}
		bool ShouldSerializeMarkerColor() {
			return MarkerColor != DefaultMarkerColor;
		}
		bool ShouldSerializeShowMarkers() {
			return ShowMarkers != defaultShowMarkers;
		}
		bool ShouldSerializeMarkerSize() {
			return MarkerSize != defaultMarkerSize;
		}
		bool ShouldSerializeLineWidth() {
			return LineWidth != defaultLineWidth;
		}
		bool ShouldSerializeEnableAntialiasing() {
			return EnableAntialiasing != defaultEnableAntialiasing;
		}
		void ResetMarkerColor() {
			MarkerColor = DefaultMarkerColor;
		}
		void ResetShowMarkers() {
			ShowMarkers = defaultShowMarkers;
		}
		void ResetMarkerSize() {
			MarkerSize = defaultMarkerSize;
		}
		void ResetLineWidth() {
			LineWidth = defaultLineWidth;
		}
		void ResetEnableAntialiasing() {
			EnableAntialiasing = defaultEnableAntialiasing;
		}
		#endregion
		protected override void Assign(ChartRangeControlClientView clone) {
			base.Assign(clone);
			LineChartRangeControlClientView cloneView = (LineChartRangeControlClientView)clone;
			cloneView.LineWidth = LineWidth;
			cloneView.MarkerSize = MarkerSize;
			cloneView.MarkerColor = MarkerColor;
			cloneView.ShowMarkers = ShowMarkers;
			cloneView.EnableAntialiasing = EnableAntialiasing;
		}
		protected override void ApplyPaletteCore(ChartRangeControlClientPaletteEntry paletteEntry) {
			base.ApplyPaletteCore(paletteEntry);
			if (SparklineView.MarkerColor == DefaultMarkerColor)
				SparklineView.MarkerColor = paletteEntry.Color;
		}
		protected override ChartRangeControlClientView CreateInstance() {
			return new LineChartRangeControlClientView();
		}
	}
}
