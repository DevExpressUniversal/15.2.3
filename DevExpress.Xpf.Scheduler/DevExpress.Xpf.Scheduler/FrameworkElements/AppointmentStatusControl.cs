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
using System.Windows.Controls;
using System.Windows;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler.Drawing;
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Scheduler.UI {
	public class AppointmentStatusControl : XPFContentControl {
		#region Properties
		#region Orientation
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public static readonly DependencyProperty OrientationProperty = CreateOrientationProperty();
		static DependencyProperty CreateOrientationProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentStatusControl, Orientation>("Orientation", Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure);
		}
		#endregion
		#region ViewInfo
		public VisualAppointmentViewInfo ViewInfo {
			get { return (VisualAppointmentViewInfo)GetValue(ViewInfoProperty); }
			set { SetValue(ViewInfoProperty, value); }
		}
		public static readonly DependencyProperty ViewInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentStatusControl, VisualAppointmentViewInfo>("ViewInfo", null, (d, e) => d.OnViewInfoChanged(e.OldValue, e.NewValue), null);
		void OnViewInfoChanged(VisualAppointmentViewInfo oldValue, VisualAppointmentViewInfo newValue) {
			if (oldValue != null)
				UnsubsribeAppointmentViewInfoEvents(oldValue);
			if (newValue != null)
				SubscribeAppointmentViewInfoEvents(newValue);
			InvalidateMeasure();
		}
		#endregion
		DateTime ViewInfoStart { get { return ViewInfo.IntervalStart; } }
		DateTime ViewInfoEnd { get { return ViewInfo.IntervalEnd; } }
		DateTime AppointmentStart { get { return ViewInfo.AppointmentStart; } }
		DateTime AppointmentEnd { get { return ViewInfo.AppointmentEnd; } }
		AppointmentStatusDisplayType StatusDisplayType { get { return ViewInfo.Options.StatusDisplayType; } }
		#endregion
		void UnsubsribeAppointmentViewInfoEvents(VisualAppointmentViewInfo viewInfo) {
			viewInfo.PropertiesChanged -= new EventHandler(OnAppointmentViewInfoPropertiesChanged);
		}
		void SubscribeAppointmentViewInfoEvents(VisualAppointmentViewInfo viewInfo) {
			viewInfo.PropertiesChanged += new EventHandler(OnAppointmentViewInfoPropertiesChanged);
		}
		void OnAppointmentViewInfoPropertiesChanged(object sender, EventArgs e) {
			InvalidateMeasure();
		}
		protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint) {
			if (double.IsInfinity(constraint.Width))
				constraint.Width = 0;
			if (double.IsInfinity(constraint.Height))
				constraint.Height = 0;
#if !SL
			if(VisualChildrenCount != 1)
				return constraint;
#else
#endif
			FrameworkElement content = System.Windows.Media.VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
			Rect contentBounds = CalculateContentBounds(constraint);
			base.MeasureOverride(RectHelper.Size(contentBounds));
			content.Measure(RectHelper.Size(contentBounds));
			return content.DesiredSize;
		}
		protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize) {
#if !SL
			FrameworkElement content = System.Windows.Media.VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
#else
			FrameworkElement content = System.Windows.Media.VisualTreeHelper.GetChild(System.Windows.Media.VisualTreeHelper.GetChild(this, 0), 0) as FrameworkElement;
#endif
			Rect contentBounds = CalculateContentBounds(finalSize);
			content.Arrange(contentBounds);
			return finalSize;
		}
		Rect CalculateContentBounds(Size size) {
			SizeHelperBase sizeHelper = SizeHelperBase.GetDefineSizeHelper(Orientation);
			Rect result = RectHelper.New(size);
			if (StatusDisplayType == AppointmentStatusDisplayType.Bounds)
				return result;
			if (ViewInfo == null)
				return result;
			TimeSpan statusStart = ((AppointmentStart > ViewInfoStart) ? AppointmentStart : ViewInfoStart) - ViewInfoStart;
			TimeSpan statusEnd = ((AppointmentEnd < ViewInfoEnd) ? AppointmentEnd : ViewInfoEnd) - ViewInfoStart;
			TimeSpan viewInfoDuration = ViewInfoEnd - ViewInfoStart;
			double defineSize = sizeHelper.GetDefineSize(size);
			if (defineSize <= 0)
				return RectHelper.New(size);
			double location = 0;
			double factor = viewInfoDuration.Ticks / defineSize;
			if (factor > 0) {
				location = statusStart.Ticks / factor;
				defineSize = Math.Max(0, (statusEnd.Ticks - statusStart.Ticks) / factor);
			}
			Point resultLocation = sizeHelper.CreatePoint(location, 0);
			Size resultSize = sizeHelper.CreateSize(defineSize, sizeHelper.GetSecondarySize(size));
			return new Rect(resultLocation, resultSize);
		}
	}
}
