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
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public enum MapZoomTrackbarOrientation {
		Vertical,
		Horizontal
	}
	public class ZoomTrackbarOptions : NavigationElementOptions {
		public static readonly DependencyProperty OrientationProperty = DependencyPropertyManager.Register("Orientation",
			typeof(MapZoomTrackbarOrientation), typeof(ZoomTrackbarOptions), new PropertyMetadata(MapZoomTrackbarOrientation.Vertical, NotifyPropertyChanged));
		public static readonly DependencyProperty ZoomingStepProperty = DependencyPropertyManager.Register("ZoomingStep",
			typeof(double), typeof(ZoomTrackbarOptions), new PropertyMetadata(1.0, NotifyPropertyChanged));
		[Category(Categories.Layout)]
		public MapZoomTrackbarOrientation Orientation {
			get { return (MapZoomTrackbarOrientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		[Category(Categories.Layout)]
		public double ZoomingStep {
			get { return (double)GetValue(ZoomingStepProperty); }
			set { SetValue(ZoomingStepProperty, value); }
		}
		public ZoomTrackbarOptions() {
			this.SetValue(NavigationElementOptions.MarginProperty, new Thickness(37, 16, 37, 16));
		}
		protected override MapDependencyObject CreateObject() {
			return new ZoomTrackbarOptions();
		}
	}
	public class TrackbarOrientationToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Visibility)) {
				MapZoomTrackbarOrientation orientation = (MapZoomTrackbarOrientation)value;
				string panelSegment = (string)parameter;
				if ((panelSegment == "H" && orientation == MapZoomTrackbarOrientation.Horizontal) ||
					(panelSegment == "V" && orientation == MapZoomTrackbarOrientation.Vertical))
					return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
