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

using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Editors;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Charts.RangeControlClient {
	public class RangeControlClientLineView : RangeControlClientView {
		public static readonly DependencyProperty ShowMarkersProperty =
			DependencyProperty.Register("ShowMarkers", typeof(Boolean), typeof(RangeControlClientLineView),
			new PropertyMetadata(defaultShowMarkers, (d, e) => ((RangeControlClientLineView)d).ShowMarkersChanged((Boolean)e.NewValue)));
		public static readonly DependencyProperty MarkerSizeProperty =
			DependencyProperty.Register("MarkerSize", typeof(Int32), typeof(RangeControlClientLineView),
			new PropertyMetadata(defaultMarkerSize, (d, e) => ((RangeControlClientLineView)d).MarkerSizeChanged((Int32)e.NewValue)));
		public static readonly DependencyProperty MarkerBrushProperty =
			DependencyProperty.Register("MarkerBrush", typeof(SolidColorBrush), typeof(RangeControlClientLineView),
			new PropertyMetadata(null, (d, e) => ((RangeControlClientLineView)d).MarkerBrushChanged((SolidColorBrush)e.NewValue)));
		const bool defaultShowMarkers = false;
		const int defaultLineWidth = 1;
		const int defaultMarkerSize = 5;
		[Category(Categories.Appearance)]
		public SolidColorBrush MarkerBrush {
			get { return (SolidColorBrush)GetValue(MarkerBrushProperty); }
			set { SetValue(MarkerBrushProperty, value); }
		}
		[Category(Categories.Appearance)]
		public bool ShowMarkers {
			get { return (bool)GetValue(ShowMarkersProperty); }
			set { SetValue(ShowMarkersProperty, value); }
		}
		[Category(Categories.Appearance)]
		public int MarkerSize {
			get { return (int)GetValue(MarkerSizeProperty); }
			set { SetValue(MarkerSizeProperty, value); }
		}
		public RangeControlClientLineView()
			: base(new LineSparklineControl()) {
			DefaultStyleKey = typeof(RangeControlClientLineView);
		}
		protected RangeControlClientLineView(SparklineControl sparkline) : base(sparkline) { }
		void ShowMarkersChanged(bool value) {
			((LineSparklineControl)Sparkline).ShowMarkers = value;
		}
		void MarkerSizeChanged(int value) {
			((LineSparklineControl)Sparkline).MarkerSize = value;
		}
		void MarkerBrushChanged(SolidColorBrush value) {
			((LineSparklineControl)Sparkline).MarkerBrush = value;
		}
	}
}
