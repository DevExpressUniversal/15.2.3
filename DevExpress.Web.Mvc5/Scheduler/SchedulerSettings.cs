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
using DevExpress.Web;
using DevExpress.Web.Localization;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.Mvc.Internal;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Web.Mvc {
	public class SchedulerSettings : SettingsBase {
		static SchedulerSettings() {
			SchedulerLocalizer.GetString(SchedulerStringId.Msg_InternalError);
			if (SchedulerLocalizer.GetActiveLocalizerProvider() == null) {
				var provider = new ASPxActiveLocalizerProvider<SchedulerStringId>(MVCxSchedulerCoreResLocalizer.CreateResLocalizerInstance);
				SchedulerLocalizer.SetActiveLocalizerProvider(provider);
			}
		}
		SchedulerViewType? activeViewType;
		MVCxSchedulerStorage storage;
		MVCxSchedulerViewRepository views;
		ASPxSchedulerOptionsBehavior optionsBehavior;
		SchedulerOptionsCustomization optionsCustomization;
		ASPxSchedulerOptionsCookies optionsCookies;
		MVCxSchedulerOptionsForms optionsForms;
		MVCxSchedulerOptionsToolTips optionsToolTips;
		ASPxSchedulerOptionsMenu optionsMenu;
		ASPxSchedulerOptionsView optionsView;
		SettingsLoadingPanel optionsLoadingPanel;
		SchedulerGroupType groupType;
		MVCxSchedulerExportSettings settingsExport;
		MVCxSchedulerImportSettings settingsImport;
		DateNavigatorSettings dateNavigatorSettings;
		WorkDaysCollection workDays;
		public SchedulerSettings() {
			this.storage = new MVCxSchedulerStorage();
			this.optionsBehavior = new ASPxSchedulerOptionsBehavior();
			this.optionsCustomization = new SchedulerOptionsCustomization();
			this.optionsCookies = new ASPxSchedulerOptionsCookies();
			this.optionsForms = new MVCxSchedulerOptionsForms();
			this.optionsToolTips = new MVCxSchedulerOptionsToolTips();
			this.optionsMenu = new ASPxSchedulerOptionsMenu();
			this.optionsView = new ASPxSchedulerOptionsView();
			this.optionsLoadingPanel = new SettingsLoadingPanel(null);
			this.settingsExport = new MVCxSchedulerExportSettings();
			this.settingsImport = new MVCxSchedulerImportSettings();
			this.dateNavigatorSettings = new DateNavigatorSettings();
			this.workDays = DevExpress.XtraScheduler.Native.InnerSchedulerControl.CreateDefaultWorkDaysCollection();
			this.views = new MVCxSchedulerViewRepository();
			this.views.PrepareViews();
			EnableChangeVisibleIntervalGestures = AutoBoolean.Auto;
			LimitInterval = NotificationTimeInterval.FullRange;
			Start = DateTime.Today;
			EnablePagingGestures = AutoBoolean.Auto;
			this.groupType = SchedulerGroupType.None;
		}
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public object EditAppointmentRouteValues { get; set; }
		public bool EnableCallbackAnimation { get; set; }
		public bool EnableChangeVisibleIntervalCallbackAnimation { get; set; }
		public AutoBoolean EnableChangeVisibleIntervalGestures { get; set; }
		public ASPxSchedulerImages Images { get { return (ASPxSchedulerImages)ImagesInternal; } }
		public ASPxSchedulerStyles Styles { get { return (ASPxSchedulerStyles)StylesInternal; } }
		public MVCxSchedulerClientSideEvents ClientSideEvents { get { return (MVCxSchedulerClientSideEvents)ClientSideEventsInternal; } }
		public TimeInterval LimitInterval { get; set; }
		public AutoBoolean EnablePagingGestures { get; set; }
		public MVCxSchedulerStorage Storage { get { return storage; } }
		public DateTime Start { get; set; }
		public SchedulerViewType ActiveViewType {
			get { return activeViewType.HasValue ? activeViewType.Value : SchedulerViewType.Day; }
			set { activeViewType = value; } 
		}
		protected internal bool IsActiveViewTypeAssigned { get { return activeViewType.HasValue; } }
		public SchedulerGroupType GroupType {
			get { return groupType; }
			set {
				if (groupType != value) {
					groupType = value;
					this.views.SetGroupType(value);
				}
			}
		}
		public MVCxSchedulerViewRepository Views { get { return views; } }
		public ASPxSchedulerOptionsBehavior OptionsBehavior { get { return optionsBehavior; } }
		public SchedulerOptionsCustomization OptionsCustomization { get { return optionsCustomization; } }
		public ASPxSchedulerOptionsCookies OptionsCookies { get { return optionsCookies; } }
		public MVCxSchedulerOptionsForms OptionsForms { get { return optionsForms; } }
		public MVCxSchedulerOptionsToolTips OptionsToolTips { get { return optionsToolTips; } }
		public ASPxSchedulerOptionsMenu OptionsMenu { get { return optionsMenu; } }
		public ASPxSchedulerOptionsView OptionsView { get { return optionsView; } }
		public SettingsLoadingPanel OptionsLoadingPanel { get { return optionsLoadingPanel; } }
		public MVCxSchedulerExportSettings SettingsExport { get { return settingsExport; } }
		public MVCxSchedulerImportSettings SettingsImport { get { return settingsImport; } }
		public DateNavigatorSettings DateNavigatorExtensionSettings { get { return dateNavigatorSettings; } }
		public WorkDaysCollection WorkDays { get { return workDays; } }
		public SchedulerCallbackCommandEventHandler AfterExecuteCallbackCommand { get; set; }
		public SchedulerCallbackCommandEventHandler BeforeExecuteCallbackCommand { get; set; }
		public EventHandler BeforeGetCallbackResult { get; set; }
		public AppointmentOperationEventHandler AllowAppointmentDelete { get; set; }
		public AppointmentOperationEventHandler AllowAppointmentEdit { get; set; }
		public AppointmentOperationEventHandler AllowAppointmentResize { get; set; }
		public AppointmentOperationEventHandler AllowInplaceEditor { get; set; }
		public AppointmentViewInfoCustomizingEventHandler AppointmentViewInfoCustomizing { get; set; }
		public ASPxSchedulerCustomErrorTextEventHandler CustomErrorText { get; set; }
		public CustomizeElementStyleEventHandler CustomizeElementStyle { get; set; }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		[Obsolete("This property is now obsolete. Use the SchedulerExtension.Bind method instead.")]
		public FetchAppointmentsEventHandler FetchAppointments { get; set; }
		[Obsolete("This property is now obsolete. Use the SchedulerExtension.Bind method instead.")]
		public PersistentObjectCancelEventHandler FilterAppointment  { get; set; }
		[Obsolete("This property is now obsolete. Use the SchedulerExtension.Bind method instead.")]
		public PersistentObjectCancelEventHandler FilterResource { get { return FilterResourceInternal; } set { FilterResourceInternal = value; } }
		internal PersistentObjectCancelEventHandler FilterResourceInternal { get; set; }
		public ASPxSchedulerTimeCellPreparedEventHandler HtmlTimeCellPrepared { get; set; }
		public AppointmentDisplayTextEventHandler InitAppointmentDisplayText { get; set; }
		public AppointmentImagesEventHandler InitAppointmentImages { get; set; }
		public InitClientAppointmentHandler InitClientAppointment { get; set; }
		public ASPxSchedulerPrepareFormPopupContainerHandler PrepareAppointmentFormPopupContainer { get; set; }
		public ASPxSchedulerPrepareFormPopupContainerHandler PrepareAppointmentInplaceEditorPopupContainer { get; set; }
		public ASPxSchedulerPrepareFormPopupContainerHandler PrepareGotoDateFormPopupContainer { get; set; }
		public PopupMenuShowingEventHandler PopupMenuShowing { get; set; }
		public EventHandler ResourceCollectionCleared { get; set; }
		public EventHandler ResourceCollectionLoaded { get; set; }
		public QueryWorkTimeEventHandler QueryWorkTime { get; set; }
		public AppointmentFormEventHandler AppointmentFormShowing { get; set; }
		protected internal string HorizontalResourceHeaderTemplateContent { get; set; }
		protected internal Action<ResourceHeaderTemplateContainer> HorizontalResourceHeaderTemplateContentMethod { get; set; }
		protected internal string DateHeaderTemplateContent { get; set; }
		protected internal Action<DateHeaderTemplateContainer> DateHeaderTemplateContentMethod { get; set; }
		protected internal string DayOfWeekHeaderTemplateContent { get; set; }
		protected internal Action<DayOfWeekHeaderTemplateContainer> DayOfWeekHeaderTemplateContentMethod { get; set; }
		protected internal string VerticalResourceHeaderTemplateContent { get; set; }
		protected internal Action<ResourceHeaderTemplateContainer> VerticalResourceHeaderTemplateContentMethod { get; set; }
		protected internal string HorizontalAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> HorizontalAppointmentTemplateContentMethod { get; set; }
		protected internal string HorizontalSameDayAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> HorizontalSameDayAppointmentTemplateContentMethod { get; set; }
		protected internal string ToolbarViewSelectorTemplateContent { get; set; }
		protected internal Action<ToolbarViewSelectorTemplateContainer> ToolbarViewSelectorTemplateContentMethod { get; set; }
		protected internal string ToolbarViewNavigatorTemplateContent { get; set; }
		protected internal Action<ToolbarViewNavigatorTemplateContainer> ToolbarViewNavigatorTemplateContentMethod { get; set; }
		protected internal string ToolbarViewVisibleIntervalTemplateContent { get; set; }
		protected internal Action<ToolbarViewVisibleIntervalTemplateContainer> ToolbarViewVisibleIntervalTemplateContentMethod { get; set; }
		public void SetHorizontalResourceHeaderTemplateContent(string content) {
			HorizontalResourceHeaderTemplateContent = content;
		}
		public void SetHorizontalResourceHeaderTemplateContent(Action<ResourceHeaderTemplateContainer> contentMethod) {
			HorizontalResourceHeaderTemplateContentMethod = contentMethod;
		}
		public void SetDateHeaderTemplateContent(string content) {
			DateHeaderTemplateContent = content;
		}
		public void SetDateHeaderTemplateContent(Action<DateHeaderTemplateContainer> contentMethod) {
			DateHeaderTemplateContentMethod = contentMethod;
		}
		public void SetDayOfWeekHeaderTemplateContent(string content) {
			DayOfWeekHeaderTemplateContent = content;
		}
		public void SetDayOfWeekHeaderTemplateContent(Action<DayOfWeekHeaderTemplateContainer> contentMethod) {
			DayOfWeekHeaderTemplateContentMethod = contentMethod;
		}
		public void SetVerticalResourceHeaderTemplateContent(string content) {
			VerticalResourceHeaderTemplateContent = content;
		}
		public void SetVerticalResourceHeaderTemplateContent(Action<ResourceHeaderTemplateContainer> contentMethod) {
			VerticalResourceHeaderTemplateContentMethod = contentMethod;
		}
		public void SetHorizontalAppointmentTemplateContent(string content) {
			HorizontalAppointmentTemplateContent = content;
		}
		public void SetHorizontalAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			HorizontalAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetHorizontalSameDayAppointmentTemplateContent(string content) {
			HorizontalSameDayAppointmentTemplateContent = content;
		}
		public void SetHorizontalSameDayAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			HorizontalSameDayAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewSelectorTemplateContent(string content) {
			ToolbarViewSelectorTemplateContent = content;
		}
		public void SetToolbarViewSelectorTemplateContent(Action<ToolbarViewSelectorTemplateContainer> contentMethod) {
			ToolbarViewSelectorTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewNavigatorTemplateContent(string content) {
			ToolbarViewNavigatorTemplateContent = content;
		}
		public void SetToolbarViewNavigatorTemplateContent(Action<ToolbarViewNavigatorTemplateContainer> contentMethod) {
			ToolbarViewNavigatorTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewVisibleIntervalTemplateContent(string content) {
			ToolbarViewVisibleIntervalTemplateContent = content;
		}
		public void SetToolbarViewVisibleIntervalTemplateContent(Action<ToolbarViewVisibleIntervalTemplateContainer> contentMethod) {
			ToolbarViewVisibleIntervalTemplateContentMethod = contentMethod;
		}
		protected override ImagesBase CreateImages() {
			return new ASPxSchedulerImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new ASPxSchedulerStyles(null);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new MVCxSchedulerClientSideEvents();
		}
	}
}
