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
using System.Windows;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
namespace DevExpress.Xpf.Scheduler.Native {
	#region SchedulerViewPropertySyncManager
	public abstract class SchedulerViewPropertySyncManager<T> : DependencyPropertySyncManager where T : SchedulerViewBase {
		readonly T view;
		protected SchedulerViewPropertySyncManager(T view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
		}
		public T View { get { return view; } }
		public override void Register() {
			PropertyMapperTable.RegisterPropertyMapper(SchedulerViewBase.ResourcesPerPageProperty, new ResourcesPerPagePropertyMapper(SchedulerViewBase.ResourcesPerPageProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerViewBase.ShowMoreButtonsProperty, new ViewShowMoreButtonsPropertyMapper(SchedulerViewBase.ShowMoreButtonsProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerViewBase.NavigationButtonVisibilityProperty, new ViewNavigationButtonVisibilityPropertyMapper(SchedulerViewBase.NavigationButtonVisibilityProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerViewBase.NavigationButtonAppointmentSearchIntervalProperty, new ViewNavigationButtonAppointmentSearchIntervalPropertyMapper(SchedulerViewBase.NavigationButtonAppointmentSearchIntervalProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerViewBase.DisplayNameProperty, new DisplayNamePropertyMapper(SchedulerViewBase.DisplayNameProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(DayView.MenuCaptionProperty, new MenuCaptionPropertyMapper(DayView.MenuCaptionProperty, View));
		}
	}
	#endregion
	#region View mappers
	public abstract class SchedulerViewPropertyMapperBase<T, U> : DependencyPropertyMapperBase
		where T : SchedulerViewBase
		where U : InnerSchedulerViewBase {
		protected SchedulerViewPropertyMapperBase(DependencyProperty property, T view)
			: base(property, view) {
		}
		public T View { get { return (T)Owner; } }
		public U InnerView { get { return (U)View.InnerView; } }
		protected abstract bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType);
		protected override void SubscribeEvents() {
			InnerView.Changed += OnActiveViewChanged;
		}
		protected override void UnsubscribeEvents() {
			InnerView.Changed -= OnActiveViewChanged;
		}
		void OnActiveViewChanged(object sender, SchedulerControlStateChangedEventArgs e) {
			if (CanUpdateOwnerProperty(e.Change))
				UpdateOwnerPropertyValue();
		}
	}
	public abstract class SchedulerViewPropertyMapper : SchedulerViewPropertyMapperBase<SchedulerViewBase, InnerSchedulerViewBase> {
		protected SchedulerViewPropertyMapper(DependencyProperty property, SchedulerViewBase view)
			: base(property, view) {
		}
	}
	public class ResourcesPerPagePropertyMapper : SchedulerViewPropertyMapper {
		public ResourcesPerPagePropertyMapper(DependencyProperty property, SchedulerViewBase view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.ResourcesPerPageChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.ResourcesPerPage;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.ResourcesPerPage = Convert.ToInt32(newValue);
		}
	}
	public class ViewShowMoreButtonsPropertyMapper : SchedulerViewPropertyMapper {
		public ViewShowMoreButtonsPropertyMapper(DependencyProperty property, SchedulerViewBase view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.ShowMoreButtonsChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.ShowMoreButtons;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.ShowMoreButtons = (bool)newValue;
		}
	}
	public class ViewNavigationButtonVisibilityPropertyMapper : SchedulerViewPropertyMapper {
		public ViewNavigationButtonVisibilityPropertyMapper(DependencyProperty property, SchedulerViewBase view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.NavigationButtonVisibilityChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.NavigationButtonVisibility;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.NavigationButtonVisibility = (NavigationButtonVisibility)newValue;
		}
	}
	public class ViewNavigationButtonAppointmentSearchIntervalPropertyMapper : SchedulerViewPropertyMapper {
		public ViewNavigationButtonAppointmentSearchIntervalPropertyMapper(DependencyProperty property, SchedulerViewBase view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.NavigationButtonAppointmentSearchIntervalChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.NavigationButtonAppointmentSearchInterval;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.NavigationButtonAppointmentSearchInterval = (TimeSpan)newValue;
		}
	}
	public class DisplayNamePropertyMapper : SchedulerViewPropertyMapper {
		public DisplayNamePropertyMapper(DependencyProperty property, SchedulerViewBase view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return true;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.DisplayName;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.DisplayName = (string)newValue;
		}
	}
	public class MenuCaptionPropertyMapper : SchedulerViewPropertyMapper {
		public MenuCaptionPropertyMapper(DependencyProperty property, SchedulerViewBase view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return true;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.MenuCaption;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.MenuCaption = (string)newValue;
		}
	}
	#endregion
	public abstract class ViewBaseInnerObjectContainerPropertyMapper<U, T> : InnerObjectContainerPropertyMapperBase<U, T>
		where U : SchedulerViewBase
		where T : BaseOptions {
		protected ViewBaseInnerObjectContainerPropertyMapper(DependencyProperty property, SchedulerViewBase owner)
			: base(property, owner) {
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			PropertyOwner.InnerView.Changed += OnInnerViewChanged;
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			PropertyOwner.InnerView.Changed -= OnInnerViewChanged;
		}
		void OnInnerViewChanged(object sender, SchedulerControlStateChangedEventArgs e) {
			UpdateOwnerPropertyValue();
		}
	}
	public abstract class AppointmentDisplayOptionsPropertyMapperBase<U, T> : ViewBaseInnerObjectContainerPropertyMapper<U, T>
		where U : SchedulerViewBase
		where T : BaseOptions {
		protected AppointmentDisplayOptionsPropertyMapperBase(DependencyProperty property, SchedulerViewBase owner)
			: base(property, owner) {
		}
		public override object GetInnerPropertyValue() {
			return PropertyOwner.InnerView.AppointmentDisplayOptions;
		}
	}
}
