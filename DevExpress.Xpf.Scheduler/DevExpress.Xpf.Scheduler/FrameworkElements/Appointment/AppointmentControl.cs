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

using System.Windows;
using DevExpress.XtraScheduler;
using System.Windows.Controls;
using System;
using DevExpress.Xpf.Core.Native;
using System.Windows.Data;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Xpf.Scheduler.Automation;
#if SL
using DevExpress.Xpf.Scheduler.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class HorizontalAppointmentStyleSelectorProperties : SchedulerSealableObject {
		#region HorizontalAppointmentSameDay
		public Style HorizontalAppointmentSameDay {
			get { return (Style)GetValue(HorizontalAppointmentSameDayProperty); }
			set { SetValue(HorizontalAppointmentSameDayProperty, value); }
		}
		public static readonly DependencyProperty HorizontalAppointmentSameDayProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<HorizontalAppointmentStyleSelectorProperties, Style>("HorizontalAppointmentSameDay", null, (s, e) => s.OnHorizontalAppointmentSameDayChanged(e.OldValue, e.NewValue));
		void OnHorizontalAppointmentSameDayChanged(Style oldValue, Style newValue) {
			SealHelper.SealIfSealable(newValue);
		}
		#endregion
		#region HorizontalAppointmentLongerThanADay
		public Style HorizontalAppointmentLongerThanADay {
			get { return (Style)GetValue(HorizontalAppointmentLongerThanADayProperty); }
			set { SetValue(HorizontalAppointmentLongerThanADayProperty, value); }
		}
		public static readonly DependencyProperty HorizontalAppointmentLongerThanADayProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<HorizontalAppointmentStyleSelectorProperties, Style>("HorizontalAppointmentLongerThanADay", null, (s, e) => s.OnHorizontalAppointmentLongerThanADayChanged(e.OldValue, e.NewValue));
		void OnHorizontalAppointmentLongerThanADayChanged(Style oldValue, Style newValue) {
			SealHelper.SealIfSealable(newValue);
		}
		#endregion
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new HorizontalAppointmentStyleSelectorProperties();
		}
#endif
	}
	public class HorizontalAppointmentStyleSelector : StyleSelector {
		HorizontalAppointmentStyleSelectorProperties properties;
		public HorizontalAppointmentStyleSelector() {
			this.properties = new HorizontalAppointmentStyleSelectorProperties();
		}
		public HorizontalAppointmentStyleSelectorProperties Properties {
			get { return properties; }
			set {
				properties = value;
				SealHelper.SealIfSealable(value);
			}
		}
		public override Style SelectStyle(object item, DependencyObject container) {
			VisualAppointmentControl appointmentControl = (VisualAppointmentControl)item;
			bool isLongTime = appointmentControl.ViewInfo.IsLongTime();
			if (isLongTime && Properties.HorizontalAppointmentLongerThanADay != null)
				return Properties.HorizontalAppointmentLongerThanADay;
			return Properties.HorizontalAppointmentSameDay;
		}
	}
	public class AppointmentStyleSelectorProperties : SchedulerSealableObject {
		#region AppointmentStyle
		public Style AppointmentStyle {
			get { return (Style)GetValue(AppointmentStyleProperty); }
			set { SetValue(AppointmentStyleProperty, value); }
		}
		public static readonly DependencyProperty AppointmentStyleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentStyleSelectorProperties, Style>("AppointmentStyle", null, (s, e) => s.OnAppointmentStyleChanged(e.OldValue, e.NewValue));
		void OnAppointmentStyleChanged(Style oldValue, Style newValue) {
			SealHelper.SealIfSealable(newValue);
		}
		#endregion
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new AppointmentStyleSelectorProperties();
		}
#endif
	}
	public class AppointmentStyleSelector : StyleSelector {
		AppointmentStyleSelectorProperties properties;
		public AppointmentStyleSelector() {
			this.properties = new AppointmentStyleSelectorProperties();
		}
		public AppointmentStyleSelectorProperties Properties {
			get { return properties; }
			set {
				properties = value;
				SealHelper.SealIfSealable(value);
			}
		}
		public override Style SelectStyle(object item, DependencyObject container) {
			return Properties.AppointmentStyle;
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region VisualAppointmentControl
	public abstract class VisualAppointmentControl : SchedulerContentControl, ISupportCopyFrom<AppointmentControl> {
		Appointment appointment;
		Rect cachedRect;
		bool lastDraggedVisualState;
		protected VisualAppointmentControl() {
			IsRecreated  = false;
		}
		#region Properties
		bool isLayoutValid = false;
		internal bool IsLayoutValid { 
			get {return isLayoutValid;} 
			set { isLayoutValid = value;} }
		internal bool IsRecreated {
			get; set;
		}
		internal double Version { get { return ViewInfo != null ? ViewInfo.Version : double.NaN; } }
		#region StyleSelector
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualAppointmentControlStyleSelector")]
#endif
		public StyleSelector StyleSelector {
			get { return (StyleSelector)GetValue(StyleSelectorProperty); }
			set { SetValue(StyleSelectorProperty, value); }
		}
		public static readonly DependencyProperty StyleSelectorProperty = CreateStyleSelectorProperty();
		static DependencyProperty CreateStyleSelectorProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentControl, StyleSelector>("StyleSelector", null, (d, e) => d.OnStyleSelectorChanged(e.OldValue, e.NewValue), null);
		}
		private void OnStyleSelectorChanged(StyleSelector oldValue, StyleSelector newValue) {
			UpdateAppointmentControl(ViewInfo);
		}
		#endregion
		#region ViewInfo
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualAppointmentControlViewInfo")]
#endif
		public VisualAppointmentViewInfo ViewInfo {
			get { return (VisualAppointmentViewInfo)GetValue(ViewInfoProperty); }
			set { SetValue(ViewInfoProperty, value); }
		}
		public static readonly DependencyProperty ViewInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentControl, VisualAppointmentViewInfo>("ViewInfo", null, (d, e) => d.OnViewInfoChanged(e.OldValue, e.NewValue), null);
		void OnViewInfoChanged(VisualAppointmentViewInfo oldValue, VisualAppointmentViewInfo newValue) {
			if (oldValue != null)
				oldValue.PropertiesChanged -= new EventHandler(OnViewInfoPropertiesChanged);
			if (newValue != null) {
				newValue.PropertiesChanged += new EventHandler(OnViewInfoPropertiesChanged);
				SetStyleSelectorBinding(newValue.View);
				UpdateAppointmentControl(newValue);
			}
		}
		#endregion
		protected internal Rect CachedRectangle {
			get { return cachedRect; }
			set { cachedRect = value; }
		}
		#region LayoutViewInfo
		public static readonly DependencyProperty LayoutViewInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentControl, VisualLayoutViewInfo>("LayoutViewInfo", null);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualAppointmentControlLayoutViewInfo")]
#endif
		public VisualLayoutViewInfo LayoutViewInfo { get { return (VisualLayoutViewInfo)GetValue(LayoutViewInfoProperty); } set { SetValue(LayoutViewInfoProperty, value); } }
		#endregion
		#region StatusDisplayType
		AppointmentStatusDisplayType oldStatusDisplayType = AppointmentStatusDisplayType.Never;
		public AppointmentStatusDisplayType StatusDisplayType {
			get { return (AppointmentStatusDisplayType)GetValue(StatusDisplayTypeProperty); }
			set {
				if (oldStatusDisplayType == value)
					return;
				oldStatusDisplayType = value;
				SetValue(StatusDisplayTypeProperty, value);
			}
		}
		public static readonly DependencyProperty StatusDisplayTypeProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentControl, AppointmentStatusDisplayType>("StatusDisplayType", AppointmentStatusDisplayType.Never);
		#endregion
		internal object IntermediateLayoutViewInfo { get; set; }
		#endregion
		protected internal virtual void InvalidateLayout() {
			IsLayoutValid = false;
			InvalidateMeasure();
		}
		public Appointment GetAppointment() {
			return appointment;
		}
		protected abstract void SetStyleSelectorBinding(SchedulerViewBase view);
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateAppointmentControl(ViewInfo);
		}
		#region ISupportCopyFrom<AppointmentControl> Members
		void ISupportCopyFrom<AppointmentControl>.CopyFrom(AppointmentControl source) {
#if !SL
			if (!source.ShouldCopyFrom)
				return;
#endif
			source.ShouldCopyFrom = false;
			this.appointment = source.Appointment;
			VisualAppointmentViewInfo viewInfo = ViewInfo ?? new VisualAppointmentViewInfo();
			viewInfo.CopyFrom(source);
			if (ViewInfo == null)
				ViewInfo = viewInfo;
			if (LayoutViewInfo == null)
				LayoutViewInfo = new VisualLayoutViewInfo();
			LayoutViewInfo.CopyFrom(source.IntermediateViewInfo);
			StatusDisplayType = source.ViewInfo.Options.StatusDisplayType;
		}
		void OnViewInfoPropertiesChanged(object sender, EventArgs e) {
			VisualAppointmentViewInfo viewInfo = sender as VisualAppointmentViewInfo;
			if (viewInfo == null)
				return;
			UpdateAppointmentControl(viewInfo);
		}
		#endregion
		protected virtual void UpdateAppointmentControl(VisualAppointmentViewInfo viewInfo) {
			if (StyleSelector != null) {
				Style style = StyleSelector.SelectStyle(this, this);
				if (Style == null || !Object.ReferenceEquals(Style, style))
					Style = style;
			}
			if (viewInfo == null)
				return;
			ApplyAppointmentTemplate(viewInfo);
			if (ViewInfo.Selected && ViewInfo.View.Control.IsKeyboardFocusWithin)
				Focus();
		}
		protected abstract void ApplyAppointmentTemplate(VisualAppointmentViewInfo viewInfo);
		protected internal virtual void SetDraggedVisualState(bool dragged) {
			if (lastDraggedVisualState == dragged)
				return;
			if (dragged)
				VisualStateManagerHelper.GoToState(this, "Dragged", true);
			else
				VisualStateManagerHelper.GoToState(this, "NotDragged", true);
			this.lastDraggedVisualState = dragged;
		}
		public virtual VisualAppointmentControl CreateCloneForDrag() {
			VisualAppointmentControl result = CreateNewControl();
			result.CopyFrom(this);
			return result;
		}
		protected internal DependencyObject GetToolTipContainer() {
			return GetTemplateChild("PART_ToolTipContainer");
		}
		protected abstract VisualAppointmentControl CreateNewControl();
		protected virtual void CopyFrom(VisualAppointmentControl source) {
			SetBinding(source, XPFContentControl.TemplateProperty, "Template");
			SetBinding(source, XPFContentControl.ContentProperty, "Content");
			SetBinding(source, XPFContentControl.ContentTemplateProperty, "ContentTemplate");
			SetBinding(source, XPFContentControl.ContentTemplateSelectorProperty, "ContentTemplateSelector");
			SetBinding(source, XPFContentControl.StyleProperty, "Style");
			SetBinding(source, VisualAppointmentControl.BackgroundProperty, "Background");
			SetBinding(source, VisualAppointmentControl.ViewInfoProperty, "ViewInfo");
			SetBinding(source, VisualAppointmentControl.LayoutViewInfoProperty, "LayoutViewInfo");
		}
		protected virtual void SetBinding(VisualAppointmentControl source, DependencyProperty dependencyProperty, string propertyName) {
			Binding binding = new Binding();
			binding.Source = source;
			binding.Mode = BindingMode.OneWay;
			binding.Path = new PropertyPath(propertyName);
			binding.BindsDirectlyToSource = true;
			this.SetBinding(dependencyProperty, binding);
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new VisualAppointmentControlAutomationPeer(this);
		}
	}
	#endregion
	#region VisualDraggedAppointmentControl
	public class VisualDraggedAppointmentControl : VisualAppointmentControl {
		protected override void ApplyAppointmentTemplate(VisualAppointmentViewInfo viewInfo) {
		}
		protected override VisualAppointmentControl CreateNewControl() {
			return new VisualDraggedAppointmentControl();
		}
		protected override void SetStyleSelectorBinding(SchedulerViewBase view) {
		}
	}
	#endregion
}
