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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Sparkline.Core;
using DevExpress.Sparkline.Localization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Sparkline {
	public class LineSparklineView : SparklineViewBase, ISupportNegativePointsControl {
		const bool highlightShowNegativePoints = false;
		const bool defaultShowMarkers = false;
		const bool defaultEnableAntialiasing = true;
		const int defaultLineWidth = 1;
		const int defaultMarkerSize = 5;
		const int defaultMaxPointMarkerSize = 5;
		const int defaultMinPointMarkerSize = 5;
		const int defaultStartPointMarkerSize = 5;
		const int defaultEndPointMarkerSize = 5;
		const int defaultNegativePointMarkerSize = 5;
		bool highlightNegativePoints = highlightShowNegativePoints;
		bool showMarkers = defaultShowMarkers;
		bool enableAntialiasing = defaultEnableAntialiasing;
		int lineWidth = defaultLineWidth;
		int markerSize = defaultMarkerSize;
		int maxPointMarkerSize = defaultMaxPointMarkerSize;
		int minPointMarkerSize = defaultMinPointMarkerSize;
		int startPointMarkerSize = defaultStartPointMarkerSize;
		int endPointMarkerSize = defaultEndPointMarkerSize;
		int negativePointMarkerSize = defaultNegativePointMarkerSize;
		Color markerColor;
		[Browsable(false)]
		public Color ActualMarkerColor { get { return markerColor.IsEmpty ? AppearanceProvider != null ? AppearanceProvider.MarkerColor : Color.Empty : markerColor; } }
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("LineSparklineViewLineWidth"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.LineSparklineView.LineWidth")
		]
		public int LineWidth {
			get { return lineWidth; }
			set {
				if (value < 1)
					throw new ArgumentException();
				if (lineWidth != value) {
					lineWidth = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("LineSparklineViewHighlightNegativePoints"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.LineSparklineView.HighlightNegativePoints"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool HighlightNegativePoints {
			get { return highlightNegativePoints; }
			set {
				if (highlightNegativePoints != value) {
					highlightNegativePoints = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("LineSparklineViewShowMarkers"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.LineSparklineView.ShowMarkers"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowMarkers {
			get { return showMarkers; }
			set {
				if (showMarkers != value) {
					showMarkers = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("LineSparklineViewMarkerSize"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.LineSparklineView.MarkerSize")
		]
		public int MarkerSize {
			get { return markerSize; }
			set {
				if (markerSize != value) {
					markerSize = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("LineSparklineViewMaxPointMarkerSize"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.LineSparklineView.MaxPointMarkerSize")
		]
		public int MaxPointMarkerSize {
			get { return maxPointMarkerSize; }
			set {
				if (maxPointMarkerSize != value) {
					maxPointMarkerSize = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("LineSparklineViewMinPointMarkerSize"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.LineSparklineView.MinPointMarkerSize")
		]
		public int MinPointMarkerSize {
			get { return minPointMarkerSize; }
			set {
				if (minPointMarkerSize != value) {
					minPointMarkerSize = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("LineSparklineViewStartPointMarkerSize"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.LineSparklineView.StartPointMarkerSize")
		]
		public int StartPointMarkerSize {
			get { return startPointMarkerSize; }
			set {
				if (startPointMarkerSize != value) {
					startPointMarkerSize = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("LineSparklineViewEndPointMarkerSize"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.LineSparklineView.EndPointMarkerSize")
		]
		public int EndPointMarkerSize {
			get { return endPointMarkerSize; }
			set {
				if (endPointMarkerSize != value) {
					endPointMarkerSize = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("LineSparklineViewNegativePointMarkerSize"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.LineSparklineView.NegativePointMarkerSize")
		]
		public int NegativePointMarkerSize {
			get { return negativePointMarkerSize; }
			set {
				if (negativePointMarkerSize != value) {
					negativePointMarkerSize = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("LineSparklineViewMarkerColor"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.LineSparklineView.MarkerColor")
		]
		public Color MarkerColor {
			get { return markerColor; }
			set {
				if (markerColor != value) {
					markerColor = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("LineSparklineViewEnableAntialiasing"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.LineSparklineView.EnableAntialiasing"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool EnableAntialiasing {
			get { return enableAntialiasing; }
			set {
				if (enableAntialiasing != value) {
					enableAntialiasing = value;
					OnPropertiesChanged();
				}
			}
		}
#if !SL
	[DevExpressSparklineCoreLocalizedDescription("LineSparklineViewType")]
#endif
		public override SparklineViewType Type {
			get { return SparklineViewType.Line; }
		}
		protected internal override bool ActualShowNegativePoint {
			get { return HighlightNegativePoints; }
		}
		public LineSparklineView() {
		}
		#region ShouldSerialize
		bool ShouldSerializeLineWidth() {
			return lineWidth != defaultLineWidth;
		}
		bool ShouldSerializeHighlightNegativePoints() {
			return highlightNegativePoints != highlightShowNegativePoints;
		}
		bool ShouldSerializeShowMarkers() {
			return showMarkers != defaultShowMarkers;
		}
		bool ShouldSerializeMarkerSize() {
			return markerSize != defaultMarkerSize;
		}
		bool ShouldSerializeMaxPointMarkerSize() {
			return maxPointMarkerSize != defaultMaxPointMarkerSize;
		}
		bool ShouldSerializeMinPointMarkerSize() {
			return minPointMarkerSize != defaultMinPointMarkerSize;
		}
		bool ShouldSerializeStartPointMarkerSize() {
			return startPointMarkerSize != defaultStartPointMarkerSize;
		}
		bool ShouldSerializeEndPointMarkerSize() {
			return endPointMarkerSize != defaultEndPointMarkerSize;
		}
		bool ShouldSerializeNegativePointMarkerSize() {
			return negativePointMarkerSize != defaultNegativePointMarkerSize;
		}
		bool ShouldSerializeMarkerColor() {
			return !markerColor.IsEmpty;
		}
		bool ShouldSerializeEnableAntialiasing() {
			return enableAntialiasing != defaultEnableAntialiasing;
		}
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "LineWidth":
					return ShouldSerializeLineWidth();
				case "HighlightNegativePoints":
					return ShouldSerializeHighlightNegativePoints();
				case "ShowMarkers":
					return ShouldSerializeShowMarkers();
				case "MarkerSize":
					return ShouldSerializeMarkerSize();
				case "MaxPointMarkerSize":
					return ShouldSerializeMaxPointMarkerSize();
				case "MinPointMarkerSize":
					return ShouldSerializeMinPointMarkerSize();
				case "StartPointMarkerSize":
					return ShouldSerializeStartPointMarkerSize();
				case "EndPointMarkerSize":
					return ShouldSerializeEndPointMarkerSize();
				case "NegativePointMarkerSize":
					return ShouldSerializeNegativePointMarkerSize();
				case "MarkerColor":
					return ShouldSerializeMarkerColor();
				case "EnableAntialiasing":
					return ShouldSerializeEnableAntialiasing();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region Reset
		void ResetLineWidth() {
			lineWidth = defaultLineWidth;
		}
		void ResetHighlightNegativePoints() {
			highlightNegativePoints = highlightShowNegativePoints;
		}
		void ResetShowMarkers() {
			showMarkers = defaultShowMarkers;
		}
		void ResetMarkerSize() {
			markerSize = defaultMarkerSize;
		}
		void ResetMaxPointMarkerSize() {
			maxPointMarkerSize = defaultMaxPointMarkerSize;
		}
		void ResetMinPointMarkerSize() {
			minPointMarkerSize = defaultMinPointMarkerSize;
		}
		void ResetStartPointMarkerSize() {
			startPointMarkerSize = defaultStartPointMarkerSize;
		}
		void ResetEndPointMarkerSize() {
			endPointMarkerSize = defaultEndPointMarkerSize;
		}
		void ResetNegativePointMarkerSize() {
			negativePointMarkerSize = defaultNegativePointMarkerSize;
		}
		void ResetMarkerColor() {
			markerColor = Color.Empty;
		}
		void ResetEnableAntialiasing() {
			enableAntialiasing = defaultEnableAntialiasing;
		}
		#endregion
		protected override string GetViewName() {
			return SparklineLocalizer.GetString(SparklineStringId.viewLine);
		}
		public void SetSizeForAllMarkers(int markerSize) {
			MarkerSize = markerSize;
			MaxPointMarkerSize = markerSize;
			MinPointMarkerSize = markerSize;
			StartPointMarkerSize = markerSize;
			EndPointMarkerSize = markerSize;
			NegativePointMarkerSize = markerSize;
		}
		public override void Assign(SparklineViewBase view) {
			base.Assign(view);
			LineSparklineView lineView = view as LineSparklineView;
			if (lineView != null) {
				enableAntialiasing = lineView.enableAntialiasing;
				showMarkers = lineView.showMarkers;
				lineWidth = lineView.lineWidth;
				markerSize = lineView.markerSize;
				maxPointMarkerSize = lineView.maxPointMarkerSize;
				minPointMarkerSize = lineView.minPointMarkerSize;
				startPointMarkerSize = lineView.startPointMarkerSize;
				endPointMarkerSize = lineView.endPointMarkerSize;
				negativePointMarkerSize = lineView.negativePointMarkerSize;
				markerColor = lineView.markerColor;
			}
			ISupportNegativePointsControl negativePointsView = view as ISupportNegativePointsControl;
			if (negativePointsView != null) {
				highlightNegativePoints = negativePointsView.HighlightNegativePoints;
			}
		}
		public override void Visit(ISparklineViewVisitor visitor) {
			visitor.Visit(this);
		}
	}
}
