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

using DevExpress.Xpf.Core;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Scheduler.Native;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.Xpf.Utils;
#if SL
using DevExpress.Xpf.Scheduler.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class AppointmentsItemControl : XPFItemsControl {
		#region View
		public SchedulerViewBase View {
			get { return (SchedulerViewBase)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		public static readonly DependencyProperty ViewProperty = CreateViewProperty();
		static DependencyProperty CreateViewProperty() {
			return DependencyPropertyHelper.RegisterProperty<AppointmentsItemControl, SchedulerViewBase>("View", null, OnViewChanged);
		}
		static void OnViewChanged(AppointmentsItemControl panel, DependencyPropertyChangedEventArgs<SchedulerViewBase> e) {
		}
		#endregion
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
		}
	}
	public class HorizontalAppointmentsItemControl : AppointmentsItemControl {
		#region ScrollOffset
		public static readonly DependencyProperty ScrollOffsetProperty = DependencyPropertyManager.RegisterAttached("ScrollOffset", typeof(double), typeof(HorizontalAppointmentsItemControl));
		public static void SetScrollOffset(DependencyObject element, double value) {
			element.SetValue(ScrollOffsetProperty, value);
		}
		public static double GetScrollOffset(DependencyObject element) {
			return (double)element.GetValue(ScrollOffsetProperty);
		}
		#endregion
		public HorizontalAppointmentsItemControl() {
			this.Loaded += new RoutedEventHandler(HorizontalAppointmentsItemControl_Loaded);
			this.Unloaded += new RoutedEventHandler(HorizontalAppointmentsItemControl_Unloaded);
		}
		void HorizontalAppointmentsItemControl_Unloaded(object sender, RoutedEventArgs e) {
			ClearValue(ScrollOffsetProperty);
		}
		void HorizontalAppointmentsItemControl_Loaded(object sender, RoutedEventArgs e) {
			ScrollContentPresenter scp = VisualTreeHelper.GetParent(this) as ScrollContentPresenter;
			if((scp != null) && (scp.ScrollOwner != null))
				SetBinding(ScrollOffsetProperty, InnerBindingHelper.CreateOneWayPropertyBinding(scp.ScrollOwner, "VerticalOffset"));
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new VisualHorizontalAppointmentControl();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			VisualHorizontalAppointmentControl aptControl = item as VisualHorizontalAppointmentControl;
			if(View == null)
				return;
			StyleSelector styleSelector = View.HorizontalAppointmentStyleSelector;
			if(styleSelector == null)
				return;
			aptControl.Style = styleSelector.SelectStyle(aptControl, aptControl);
		}
	}
}
