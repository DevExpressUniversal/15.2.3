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

using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Editors.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Editors {
	public class LineSparklineControl : SparklineControl, ISupportNegativePointsControl {
		#region Dependency Properties
		public static readonly DependencyProperty LineWidthProperty = DependencyProperty.Register("LineWidth", typeof(Int32), typeof(LineSparklineControl),
			new FrameworkPropertyMetadata(defaultLineWidth, (d, e) => ((LineSparklineControl)d).OnLineWidthChanged((Int32)e.NewValue)));
		public static readonly DependencyProperty HighlightNegativePointsProperty = DependencyProperty.Register("HighlightNegativePoints", typeof(Boolean), typeof(LineSparklineControl),
			new FrameworkPropertyMetadata(highlightShowNegativePoints, (d, e) => ((LineSparklineControl)d).OnHighlightNegativePointsChanged((Boolean)e.NewValue)));
		public static readonly DependencyProperty ShowMarkersProperty = DependencyProperty.Register("ShowMarkers", typeof(Boolean), typeof(LineSparklineControl),
			new FrameworkPropertyMetadata(defaultShowMarkers, (d, e) => ((LineSparklineControl)d).OnShowMarkersChanged((Boolean)e.NewValue)));
		public static readonly DependencyProperty MarkerSizeProperty = DependencyProperty.Register("MarkerSize", typeof(Int32), typeof(LineSparklineControl),
			new FrameworkPropertyMetadata(defaultMarkerSize, (d, e) => ((LineSparklineControl)d).OnMarkerSizeChanged((Int32)e.NewValue)));
		public static readonly DependencyProperty MaxPointMarkerSizeProperty = DependencyProperty.Register("MaxPointMarkerSize", typeof(Int32), typeof(LineSparklineControl),
			new FrameworkPropertyMetadata(defaultMaxPointMarkerSize, (d, e) => ((LineSparklineControl)d).OnMaxPointMarkerSizeChanged((Int32)e.NewValue)));
		public static readonly DependencyProperty MinPointMarkerSizeProperty = DependencyProperty.Register("MinPointMarkerSize", typeof(Int32), typeof(LineSparklineControl),
			new FrameworkPropertyMetadata(defaultMinPointMarkerSize, (d, e) => ((LineSparklineControl)d).OnMinPointMarkerSizeChanged((Int32)e.NewValue)));
		public static readonly DependencyProperty StartPointMarkerSizeProperty = DependencyProperty.Register("StartPointMarkerSize", typeof(Int32), typeof(LineSparklineControl),
			new FrameworkPropertyMetadata(defaultStartPointMarkerSize, (d, e) => ((LineSparklineControl)d).OnStartPointMarkerSizeChanged((Int32)e.NewValue)));
		public static readonly DependencyProperty EndPointMarkerSizeProperty = DependencyProperty.Register("EndPointMarkerSize", typeof(Int32), typeof(LineSparklineControl),
			new FrameworkPropertyMetadata(defaultEndPointMarkerSize, (d, e) => ((LineSparklineControl)d).OnEndPointMarkerSizeChanged((Int32)e.NewValue)));
		public static readonly DependencyProperty NegativePointMarkerSizeProperty = DependencyProperty.Register("NegativePointMarkerSize", typeof(Int32), typeof(LineSparklineControl),
			new FrameworkPropertyMetadata(defaultNegativePointMarkerSize, (d, e) => ((LineSparklineControl)d).OnNegativePointMarkerSizeChanged((Int32)e.NewValue)));
		public static readonly DependencyProperty MarkerBrushProperty = DependencyProperty.Register("MarkerBrush", typeof(SolidColorBrush), typeof(LineSparklineControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((LineSparklineControl)d).OnMarkerBrushChanged((SolidColorBrush)e.NewValue)));
		#endregion
		const bool highlightShowNegativePoints = false;
		const bool defaultShowMarkers = false;
		const int defaultLineWidth = 1;
		const int defaultMarkerSize = 5;
		const int defaultMaxPointMarkerSize = 5;
		const int defaultMinPointMarkerSize = 5;
		const int defaultStartPointMarkerSize = 5;
		const int defaultEndPointMarkerSize = 5;
		const int defaultNegativePointMarkerSize = 5;
		bool highlightNegativePoints = highlightShowNegativePoints;
		bool showMarkers = defaultShowMarkers;
		int lineWidth = defaultLineWidth;
		int markerSize = defaultMarkerSize;
		int maxPointMarkerSize = defaultMaxPointMarkerSize;
		int minPointMarkerSize = defaultMinPointMarkerSize;
		int startPointMarkerSize = defaultStartPointMarkerSize;
		int endPointMarkerSize = defaultEndPointMarkerSize;
		int negativePointMarkerSize = defaultNegativePointMarkerSize;
		SolidColorBrush markerBrush = new SolidColorBrush(Colors.Black);
		protected internal override bool ActualShowNegativePoint { get { return ActualHighlightNegativePoints; } }
		internal SolidColorBrush ActualMarkerBrush { get { return markerBrush; } }
		internal int ActualLineWidth { get { return lineWidth; } }
		internal bool ActualHighlightNegativePoints { get { return highlightNegativePoints; } }
		internal bool ActualShowMarkers { get { return showMarkers; } }
		internal int ActualMarkerSize { get { return markerSize; } }
		internal int ActualMaxPointMarkerSize { get { return maxPointMarkerSize; } }
		internal int ActualMinPointMarkerSize { get { return minPointMarkerSize; } }
		internal int ActualStartPointMarkerSize { get { return startPointMarkerSize; } }
		internal int ActualEndPointMarkerSize { get { return endPointMarkerSize; } }
		internal int ActualNegativePointMarkerSize { get { return negativePointMarkerSize; } }
		public bool HighlightNegativePoints {
			get { return (bool)GetValue(HighlightNegativePointsProperty); }
			set { SetValue(HighlightNegativePointsProperty, value); }
		}
		public bool ShowMarkers {
			get { return (bool)GetValue(ShowMarkersProperty); }
			set { SetValue(ShowMarkersProperty, value); }
		}
		public int LineWidth {
			get { return (int)GetValue(LineWidthProperty); }
			set { SetValue(LineWidthProperty, value); }
		}
		public int MarkerSize {
			get { return (int)GetValue(MarkerSizeProperty); }
			set { SetValue(MarkerSizeProperty, value); }
		}
		public int MaxPointMarkerSize {
			get { return (int)GetValue(MaxPointMarkerSizeProperty); }
			set { SetValue(MaxPointMarkerSizeProperty, value); }
		}
		public int MinPointMarkerSize {
			get { return (int)GetValue(MinPointMarkerSizeProperty); }
			set { SetValue(MinPointMarkerSizeProperty, value); }
		}
		public int StartPointMarkerSize {
			get { return (int)GetValue(StartPointMarkerSizeProperty); }
			set { SetValue(StartPointMarkerSizeProperty, value); }
		}
		public int EndPointMarkerSize {
			get { return (int)GetValue(EndPointMarkerSizeProperty); }
			set { SetValue(EndPointMarkerSizeProperty, value); }
		}
		public int NegativePointMarkerSize {
			get { return (int)GetValue(NegativePointMarkerSizeProperty); }
			set { SetValue(NegativePointMarkerSizeProperty, value); }
		}
		public SolidColorBrush MarkerBrush {
			get { return (SolidColorBrush)GetValue(MarkerBrushProperty); }
			set { SetValue(MarkerBrushProperty, value); }
		}
		public override SparklineViewType Type {
			get { return SparklineViewType.Line; }
		}
		public LineSparklineControl()
			: base() {
			DefaultStyleKey = typeof(LineSparklineControl);
		}
		protected internal override BaseSparklinePainter CreatePainter() {
			return new LineSparklinePainter();
		}
		protected override System.Windows.Forms.Padding GetMarkersPadding() {
			double maxMarkerWidth = ActualLineWidth;
			if (ActualHighlightStartPoint)
				maxMarkerWidth = Math.Max(maxMarkerWidth, ActualStartPointMarkerSize);
			if (ActualHighlightEndPoint)
				maxMarkerWidth = Math.Max(maxMarkerWidth, ActualEndPointMarkerSize);
			if (ActualHighlightMaxPoint)
				maxMarkerWidth = Math.Max(maxMarkerWidth, ActualMaxPointMarkerSize);
			if (ActualHighlightMinPoint)
				maxMarkerWidth = Math.Max(maxMarkerWidth, ActualMinPointMarkerSize);
			if (ActualHighlightNegativePoints)
				maxMarkerWidth = Math.Max(maxMarkerWidth, ActualNegativePointMarkerSize);
			if (ActualShowMarkers)
				maxMarkerWidth = Math.Max(maxMarkerWidth, ActualMarkerSize);
			return new System.Windows.Forms.Padding((int)Math.Ceiling(0.5 * maxMarkerWidth));
		}
		void OnLineWidthChanged(int lineWidth) {
			this.lineWidth = lineWidth;
			PropertyChanged();
		}
		void OnHighlightNegativePointsChanged(bool highlightNegativePoints) {
			this.highlightNegativePoints = highlightNegativePoints;
			PropertyChanged();
		}
		void OnShowMarkersChanged(bool showMarkers) {
			this.showMarkers = showMarkers;
			PropertyChanged();
		}
		void OnMarkerSizeChanged(int markerSize) {
			this.markerSize = markerSize;
			PropertyChanged();
		}
		void OnMaxPointMarkerSizeChanged(int maxPointMarkerSize) {
			this.maxPointMarkerSize = maxPointMarkerSize;
			PropertyChanged();
		}
		void OnMinPointMarkerSizeChanged(int minPointMarkerSize) {
			this.minPointMarkerSize = minPointMarkerSize;
			PropertyChanged();
		}
		void OnStartPointMarkerSizeChanged(int startPointMarkerSize) {
			this.startPointMarkerSize = startPointMarkerSize;
			PropertyChanged();
		}
		void OnEndPointMarkerSizeChanged(int endPointMarkerSize) {
			this.endPointMarkerSize = endPointMarkerSize;
			PropertyChanged();
		}
		void OnNegativePointMarkerSizeChanged(int negativePointMarkerSize) {
			this.negativePointMarkerSize = negativePointMarkerSize;
			PropertyChanged();
		}
		void OnMarkerBrushChanged(SolidColorBrush markerBrush) {
			this.markerBrush = markerBrush;
			PropertyChanged();
		}
		protected override string GetViewName() {
			return EditorLocalizer.GetString(EditorStringId.SparklineViewLine);
		}
		public void SetSizeForAllMarkers(int markerSize) {
			MarkerSize = markerSize;
			MaxPointMarkerSize = markerSize;
			MinPointMarkerSize = markerSize;
			StartPointMarkerSize = markerSize;
			EndPointMarkerSize = markerSize;
			NegativePointMarkerSize = markerSize;
		}
		public override void Assign(SparklineControl view) {
			base.Assign(view);
			LineSparklineControl lineView = view as LineSparklineControl;
			if (lineView != null) {
				showMarkers = lineView.showMarkers;
				lineWidth = lineView.lineWidth;
				markerSize = lineView.markerSize;
				maxPointMarkerSize = lineView.maxPointMarkerSize;
				minPointMarkerSize = lineView.minPointMarkerSize;
				startPointMarkerSize = lineView.startPointMarkerSize;
				endPointMarkerSize = lineView.endPointMarkerSize;
				negativePointMarkerSize = lineView.negativePointMarkerSize;
				markerBrush = lineView.markerBrush;
			}
			ISupportNegativePointsControl negativePointsView = view as ISupportNegativePointsControl;
			if (negativePointsView != null) {
				highlightNegativePoints = negativePointsView.HighlightNegativePoints;
			}
		}
	}
}
