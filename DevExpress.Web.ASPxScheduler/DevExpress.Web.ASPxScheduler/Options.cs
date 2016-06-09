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
using DevExpress.XtraScheduler;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Native;
using DevExpress.Web;
using DevExpress.Web.Localization;
using DevExpress.Web.Internal;
using System.Web.UI;
using System.Collections.Generic;
using DevExpress.Web.ASPxScheduler.Internal;
namespace DevExpress.Web.ASPxScheduler {
	#region ASPxSchedulerOptionsBehavior
	public class ASPxSchedulerOptionsBehavior : SchedulerOptionsBehaviorBase {
		#region Fields
		bool showViewSelector;
		bool showViewNavigator;
		bool showViewVisibleInterval;
		bool showDetailedErrorInfo;
		bool showViewNavigatorGotoDateButton;
		#endregion
		#region Properties
		#region ShowViewSelector
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsBehaviorShowViewSelector"),
#endif
DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public bool ShowViewSelector {
			get { return showViewSelector; }
			set {
				if (showViewSelector == value)
					return;
				showViewSelector = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowViewSelector", !value, value));
			}
		}
		#endregion
		#region ShowViewNavigator
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsBehaviorShowViewNavigator"),
#endif
DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public bool ShowViewNavigator {
			get { return showViewNavigator; }
			set {
				if (showViewNavigator == value)
					return;
				showViewNavigator = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowViewNavigator", !value, value));
			}
		}
		#endregion
		#region ShowViewNavigatorGotoDateButton
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsBehaviorShowViewNavigatorGotoDateButton"),
#endif
DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public bool ShowViewNavigatorGotoDateButton {
			get { return showViewNavigatorGotoDateButton; }
			set {
				if (showViewNavigatorGotoDateButton == value)
					return;
				showViewNavigatorGotoDateButton = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowViewNavigatorGotoDateButton", !value, value));
			}
		}
		#endregion
		#region ShowViewVisibleInterval
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsBehaviorShowViewVisibleInterval"),
#endif
DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public bool ShowViewVisibleInterval {
			get { return showViewVisibleInterval; }
			set {
				if (showViewVisibleInterval == value)
					return;
				showViewVisibleInterval = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowViewVisibleInterval", !value, value));
			}
		}
		#endregion
		#region ShowDetailedErrorInfo
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsBehaviorShowDetailedErrorInfo"),
#endif
DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public bool ShowDetailedErrorInfo {
			get {
				return showDetailedErrorInfo;
			}
			set {
				showDetailedErrorInfo = value;
			}
		}
		#endregion
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool SelectOnRightClick { get { return base.SelectOnRightClick; } set { base.SelectOnRightClick = value; } }
		#endregion
		#region ResetCore
		protected internal override void ResetCore() {
			base.ResetCore();
			ShowViewSelector = true;
			ShowViewNavigator = true;
			ShowViewVisibleInterval = true;
			showDetailedErrorInfo = true;
			ShowViewNavigatorGotoDateButton = true;
		}
		#endregion
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			ASPxSchedulerOptionsBehavior optionsBehaviour = options as ASPxSchedulerOptionsBehavior;
			if (optionsBehaviour != null) {
				BeginUpdate();
				try {
					ShowViewSelector = optionsBehaviour.ShowViewSelector;
					ShowViewNavigator = optionsBehaviour.ShowViewNavigator;
					ShowViewNavigatorGotoDateButton = optionsBehaviour.ShowViewNavigatorGotoDateButton;
					ShowViewVisibleInterval = optionsBehaviour.ShowViewVisibleInterval;
					ShowDetailedErrorInfo = optionsBehaviour.ShowDetailedErrorInfo;
					SelectOnRightClick = optionsBehaviour.SelectOnRightClick;
				}
				finally {
					EndUpdate();
				}
			}
		}
	}
	#endregion
	public enum ToolTipCornerType { None, Rounded, Square };
	#region ASPxSchedulerOptionsMenu
	public class ASPxSchedulerOptionsMenu : SchedulerNotificationOptions {
		#region Fields
		bool enableMenuScrolling;
		#endregion
		#region EnableMenuScrolling
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsMenuEnableMenuScrolling"),
#endif
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public bool EnableMenuScrolling
		{
			get { return enableMenuScrolling; }
			set
			{
				bool oldValue = EnableMenuScrolling;
				if (oldValue == value)
					return;
				enableMenuScrolling = value;
				OnChanged(new BaseOptionChangedEventArgs("EnableMenuScrolling", oldValue, value));
			}
		}
		#endregion
		protected internal override void ResetCore() {
			EnableMenuScrolling = false;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			ASPxSchedulerOptionsMenu optionsMenu = options as ASPxSchedulerOptionsMenu;
			if (optionsMenu != null) {
				BeginUpdate();
				try {
					EnableMenuScrolling = optionsMenu.EnableMenuScrolling;
				}
				finally {
					EndUpdate();
				}
			}
		}
	}
	#endregion
	#region ASPxSchedulerOptionsToolTips
	public class ASPxSchedulerOptionsToolTips : SchedulerNotificationOptions {
		#region Fields
		StringValuePersistentStorage stringStorage = new StringValuePersistentStorage();
		ToolTipCornerType selectionToolTipCornerType = ToolTipCornerType.Square;
		ToolTipCornerType appointmentToolTipCornerType = ToolTipCornerType.Square;
		ToolTipCornerType appointmentDragToolTipCornerType = ToolTipCornerType.Rounded;
		bool showSelectionToolTip = true;
		bool showAppointmentToolTip = true;
		bool showAppointmentDragToolTip = true;
		#endregion
		#region SelectionToolTipUrl
		StringValuePersistentStorage StringStorage { get { return stringStorage; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsToolTipsSelectionToolTipUrl"),
#endif
DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string SelectionToolTipUrl {
			get { return StringStorage.GetProperty(SchedulerFormNames.SelectionToolTip); }
			set {
				string oldValue = SelectionToolTipUrl;
				if (oldValue == value)
					return;
				StringStorage.SetProperty(SchedulerFormNames.SelectionToolTip, value);
				OnChanged(new BaseOptionChangedEventArgs("SelectionToolTipUrl", oldValue, value));
			}
		}
		#endregion
		#region AppointmentToolTipUrl
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsToolTipsAppointmentToolTipUrl"),
#endif
DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string AppointmentToolTipUrl {
			get { return StringStorage.GetProperty(SchedulerFormNames.AppointmentToolTip); }
			set {
				string oldValue = AppointmentToolTipUrl;
				if (oldValue == value)
					return;
				StringStorage.SetProperty(SchedulerFormNames.AppointmentToolTip, value);
				OnChanged(new BaseOptionChangedEventArgs("AppointmentToolTipUrl", oldValue, value));
			}
		}
		#endregion
		#region AppointmentDragToolTipUrl
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsToolTipsAppointmentDragToolTipUrl"),
#endif
DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string AppointmentDragToolTipUrl {
			get { return StringStorage.GetProperty(SchedulerFormNames.AppointmentDragToolTip); }
			set {
				string oldValue = AppointmentDragToolTipUrl;
				if (oldValue == value)
					return;
				StringStorage.SetProperty(SchedulerFormNames.AppointmentDragToolTip, value);
				OnChanged(new BaseOptionChangedEventArgs("AppointmentDragToolTipUrl", oldValue, value));
			}
		}
		#endregion
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsToolTipsSelectionToolTipCornerType"),
#endif
DefaultValue(ToolTipCornerType.Square), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public ToolTipCornerType SelectionToolTipCornerType {
			get { return selectionToolTipCornerType; }
			set {
				ToolTipCornerType oldValue = selectionToolTipCornerType;
				if (oldValue == value)
					return;
				selectionToolTipCornerType = value;
				OnChanged(new BaseOptionChangedEventArgs("SelectionToolTipCornerType", oldValue, value));
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsToolTipsAppointmentToolTipCornerType"),
#endif
DefaultValue(ToolTipCornerType.Square), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public ToolTipCornerType AppointmentToolTipCornerType {
			get { return appointmentToolTipCornerType; }
			set {
				ToolTipCornerType oldValue = appointmentToolTipCornerType;
				if (oldValue == value)
					return;
				appointmentToolTipCornerType = value;
				OnChanged(new BaseOptionChangedEventArgs("AppointmentToolTipCornerType", oldValue, value));
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsToolTipsAppointmentDragToolTipCornerType"),
#endif
DefaultValue(ToolTipCornerType.Rounded), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public ToolTipCornerType AppointmentDragToolTipCornerType {
			get { return appointmentDragToolTipCornerType; }
			set {
				ToolTipCornerType oldValue = appointmentDragToolTipCornerType;
				if (oldValue == value)
					return;
				appointmentDragToolTipCornerType = value;
				OnChanged(new BaseOptionChangedEventArgs("AppointmentDragToolTipCornerType", oldValue, value));
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsToolTipsShowSelectionToolTip"),
#endif
DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public bool ShowSelectionToolTip {
			get { return showSelectionToolTip; }
			set {
				bool oldValue = ShowSelectionToolTip;
				if (oldValue == value)
					return;
				showSelectionToolTip = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowSelectionToolTip", oldValue, value));
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsToolTipsShowAppointmentToolTip"),
#endif
DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public bool ShowAppointmentToolTip {
			get { return showAppointmentToolTip; }
			set {
				bool oldValue = ShowAppointmentToolTip;
				if (oldValue == value)
					return;
				showAppointmentToolTip = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowAppointmentToolTip", oldValue, value));
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsToolTipsShowAppointmentDragToolTip"),
#endif
DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public bool ShowAppointmentDragToolTip {
			get { return showAppointmentDragToolTip; }
			set {
				bool oldValue = ShowAppointmentDragToolTip;
				if (oldValue == value)
					return;
				showAppointmentDragToolTip = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowAppointmentDragToolTip", oldValue, value));
			}
		}
		#region ResetCore
		protected internal override void ResetCore() {
			AppointmentToolTipUrl = String.Empty;
			SelectionToolTipUrl = String.Empty;
			AppointmentDragToolTipUrl = String.Empty;
			ShowSelectionToolTip = true;
			ShowAppointmentToolTip = true;
			ShowAppointmentDragToolTip = true;
			AppointmentToolTipCornerType = ToolTipCornerType.Square;
			AppointmentDragToolTipCornerType = ToolTipCornerType.Rounded;
			SelectionToolTipCornerType = ToolTipCornerType.Square;
		}
		#endregion
		protected internal string GetFormPath(string formName) {
			return StringStorage.GetProperty(formName);
		}
		protected internal void SetFormPath(string formName, string value) {
			string oldValue = GetFormPath(formName);
			StringStorage.SetProperty(formName, value);
			OnChanged(new BaseOptionChangedEventArgs(String.Format("{0}Url", formName), oldValue, value));
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			ASPxSchedulerOptionsToolTips optionsToolTips = options as ASPxSchedulerOptionsToolTips;
			if (optionsToolTips != null) {
				BeginUpdate();
				try {
					SelectionToolTipUrl = optionsToolTips.SelectionToolTipUrl;
					AppointmentToolTipUrl = optionsToolTips.AppointmentToolTipUrl;
					AppointmentDragToolTipUrl = optionsToolTips.AppointmentDragToolTipUrl;
					SelectionToolTipCornerType = optionsToolTips.SelectionToolTipCornerType;
					AppointmentToolTipCornerType = optionsToolTips.AppointmentToolTipCornerType;
					AppointmentDragToolTipCornerType = optionsToolTips.AppointmentDragToolTipCornerType;
					ShowSelectionToolTip = optionsToolTips.ShowSelectionToolTip;
					ShowAppointmentToolTip = optionsToolTips.ShowAppointmentToolTip;
					ShowAppointmentDragToolTip = optionsToolTips.ShowAppointmentDragToolTip;
				}
				finally {
					EndUpdate();
				}
			}
		}
	}
	#endregion
	#region ASPxSchedulerOptionsForms
	public class ASPxSchedulerOptionsForms : SchedulerNotificationOptions {
		#region Fields
		StringValuePersistentStorage stringStorage = new StringValuePersistentStorage();
		SchedulerFormVisibility appointmentFormVisibility;
		SchedulerFormVisibility gotoDateFormVisibility;
		SchedulerFormVisibility recurrentAppointmentDeleteFormVisibility;
		SchedulerFormVisibility recurrentAppointmentEditFormVisibility;
		#endregion
		StringValuePersistentStorage StringStorage { get { return stringStorage; } }
		#region AppointmentFormVisibility
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsFormsAppointmentFormVisibility"),
#endif
		DefaultValue(SchedulerFormVisibility.PopupWindow), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public SchedulerFormVisibility AppointmentFormVisibility {
			get { return appointmentFormVisibility; }
			set {
				SchedulerFormVisibility oldValue = appointmentFormVisibility;
				if (oldValue == value)
					return;
				appointmentFormVisibility = value;
				OnChanged(new BaseOptionChangedEventArgs("AppointmentFormVisibility", oldValue, value));
			}
		}
		#endregion
		#region GotoDateFormVisibility
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsFormsGotoDateFormVisibility"),
#endif
		DefaultValue(SchedulerFormVisibility.PopupWindow), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public SchedulerFormVisibility GotoDateFormVisibility {
			get { return gotoDateFormVisibility; }
			set {
				SchedulerFormVisibility oldValue = gotoDateFormVisibility;
				if (oldValue == value)
					return;
				gotoDateFormVisibility = value;
				OnChanged(new BaseOptionChangedEventArgs("GotoDateFormVisibility", oldValue, value));
			}
		}
		#endregion
		#region RecurrentAppointmentDeleteFormVisibility
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsFormsRecurrentAppointmentDeleteFormVisibility"),
#endif
DefaultValue(SchedulerFormVisibility.PopupWindow), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public SchedulerFormVisibility RecurrentAppointmentDeleteFormVisibility {
			get { return recurrentAppointmentDeleteFormVisibility; }
			set {
				SchedulerFormVisibility oldValue = recurrentAppointmentDeleteFormVisibility;
				if (oldValue == value)
					return;
				recurrentAppointmentDeleteFormVisibility = value;
				OnChanged(new BaseOptionChangedEventArgs("RecurrentAppointmentDeleteFormVisibility", oldValue, value));
			}
		}
		#endregion
		#region RecurrentAppointmentEditFormVisibility
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsFormsRecurrentAppointmentEditFormVisibility"),
#endif
DefaultValue(SchedulerFormVisibility.PopupWindow), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public SchedulerFormVisibility RecurrentAppointmentEditFormVisibility {
			get { return recurrentAppointmentEditFormVisibility; }
			set {
				SchedulerFormVisibility oldValue = recurrentAppointmentEditFormVisibility;
				if (oldValue == value)
					return;
				recurrentAppointmentEditFormVisibility = value;
				OnChanged(new BaseOptionChangedEventArgs("RecurrentAppointmentEditFormVisibility", oldValue, value));
			}
		}
		#endregion
		#region AppointmentFormTemplateUrl
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsFormsAppointmentFormTemplateUrl"),
#endif
		DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty] 
		public string AppointmentFormTemplateUrl {
			get { return StringStorage.GetProperty(SchedulerFormNames.AppointmentForm); }
			set {
				string oldValue = AppointmentFormTemplateUrl;
				if (oldValue == value)
					return;
				StringStorage.SetProperty(SchedulerFormNames.AppointmentForm, value);
				OnChanged(new BaseOptionChangedEventArgs("AppointmentFormTemplateUrl", oldValue, value));
			}
		}
		#endregion
		#region RemindersFormTemplateUrl
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsFormsRemindersFormTemplateUrl"),
#endif
		DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty] 
		public string RemindersFormTemplateUrl {
			get { return StringStorage.GetProperty(SchedulerFormNames.RemindersForm); }
			set {
				string oldValue = RemindersFormTemplateUrl;
				if (oldValue == value)
					return;
				StringStorage.SetProperty(SchedulerFormNames.RemindersForm, value);
				OnChanged(new BaseOptionChangedEventArgs("RemindersFormTemplateUrl", oldValue, value));
			}
		}
		#endregion
		#region GotoDateFormTemplateUrl
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsFormsGotoDateFormTemplateUrl"),
#endif
		DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty] 
		public string GotoDateFormTemplateUrl {
			get { return StringStorage.GetProperty(SchedulerFormNames.GotoDateForm); }
			set {
				string oldValue = GotoDateFormTemplateUrl;
				if (oldValue == value)
					return;
				StringStorage.SetProperty(SchedulerFormNames.GotoDateForm, value);
				OnChanged(new BaseOptionChangedEventArgs("GotoDateFormTemplateUrl", oldValue, value));
			}
		}
		#endregion
		#region RecurrentAppointmentDeleteFormTemplateUrl
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsFormsRecurrentAppointmentDeleteFormTemplateUrl"),
#endif
		DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty] 
		public string RecurrentAppointmentDeleteFormTemplateUrl {
			get { return StringStorage.GetProperty(SchedulerFormNames.RecurrentAppointmentDeleteForm); }
			set {
				string oldValue = RecurrentAppointmentDeleteFormTemplateUrl;
				if (oldValue == value)
					return;
				StringStorage.SetProperty(SchedulerFormNames.RecurrentAppointmentDeleteForm, value);
				OnChanged(new BaseOptionChangedEventArgs("RecurrentAppointmentDeleteFormTemplateUrl", oldValue, value));
			}
		}
		#endregion
		#region RecurrentAppointmentEditFormTemplateUrl
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsFormsRecurrentAppointmentEditFormTemplateUrl"),
#endif
		DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty] 
		public string RecurrentAppointmentEditFormTemplateUrl {
			get { return StringStorage.GetProperty(SchedulerFormNames.RecurrentAppointmentEditForm); }
			set {
				string oldValue = RecurrentAppointmentEditFormTemplateUrl;
				if (oldValue == value)
					return;
				StringStorage.SetProperty(SchedulerFormNames.RecurrentAppointmentEditForm, value);
				OnChanged(new BaseOptionChangedEventArgs("RecurrentAppointmentEditFormTemplateUrl", oldValue, value));
			}
		}
		#endregion
		#region AppointmentInplaceEditorFormTemplateUrl
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsFormsAppointmentInplaceEditorFormTemplateUrl"),
#endif
		DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty] 
		public string AppointmentInplaceEditorFormTemplateUrl {
			get { return StringStorage.GetProperty(SchedulerFormNames.InplaceEditorForm); }
			set {
				string oldValue = AppointmentInplaceEditorFormTemplateUrl;
				if (oldValue == value)
					return;
				StringStorage.SetProperty(SchedulerFormNames.InplaceEditorForm, value);
				OnChanged(new BaseOptionChangedEventArgs("AppointmentInplaceEditorFormTemplateUrl", oldValue, value));
			}
		}
		#endregion
		#region MessageBoxTemplateUrl
		[DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable, Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string MessageBoxTemplateUrl {
			get {return StringStorage.GetProperty(SchedulerFormNames.MessageBox);}
			set {
				string oldValue = MessageBoxTemplateUrl;
				if (oldValue == value)
					return;
				StringStorage.SetProperty(SchedulerFormNames.MessageBox, value);
				OnChanged(new BaseOptionChangedEventArgs("MessageBoxTemplateUrl", oldValue, value));
			}
		}
		#endregion
		#region ResetCore
		protected internal override void ResetCore() {
			AppointmentFormVisibility = SchedulerFormVisibility.PopupWindow;
			AppointmentFormTemplateUrl = String.Empty;
			GotoDateFormVisibility = SchedulerFormVisibility.PopupWindow;
			GotoDateFormTemplateUrl = String.Empty;
			AppointmentInplaceEditorFormTemplateUrl = String.Empty;
			RecurrentAppointmentDeleteFormVisibility = SchedulerFormVisibility.PopupWindow;
			RecurrentAppointmentDeleteFormTemplateUrl = String.Empty;
			RecurrentAppointmentEditFormVisibility = SchedulerFormVisibility.PopupWindow;
			RecurrentAppointmentEditFormTemplateUrl = String.Empty;
			RemindersFormTemplateUrl = String.Empty;
		}
		#endregion
		protected internal string GetFormPath(string formName) {
			return StringStorage.GetProperty(formName);
		}
		protected internal void SetFormPath(string formName, string value) {
			string oldValue = GetFormPath(formName);
			StringStorage.SetProperty(formName, value);
			OnChanged(new BaseOptionChangedEventArgs(String.Format("{0}TemplateUrl", formName), oldValue, value));
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			ASPxSchedulerOptionsForms optionsForms = options as ASPxSchedulerOptionsForms;
			if (optionsForms != null) {
				BeginUpdate();
				try {
					AppointmentFormVisibility = optionsForms.AppointmentFormVisibility;
					GotoDateFormVisibility = optionsForms.GotoDateFormVisibility;
					RecurrentAppointmentDeleteFormVisibility = optionsForms.RecurrentAppointmentDeleteFormVisibility;
					RecurrentAppointmentEditFormVisibility = optionsForms.RecurrentAppointmentEditFormVisibility;
					AppointmentFormTemplateUrl = optionsForms.AppointmentFormTemplateUrl;
					RemindersFormTemplateUrl = optionsForms.RemindersFormTemplateUrl;
					GotoDateFormTemplateUrl = optionsForms.GotoDateFormTemplateUrl;
					RecurrentAppointmentDeleteFormTemplateUrl = optionsForms.RecurrentAppointmentDeleteFormTemplateUrl;
					RecurrentAppointmentEditFormTemplateUrl = optionsForms.RecurrentAppointmentEditFormTemplateUrl;
					AppointmentInplaceEditorFormTemplateUrl = optionsForms.AppointmentInplaceEditorFormTemplateUrl;
					MessageBoxTemplateUrl = optionsForms.MessageBoxTemplateUrl;
				}
				finally {
					EndUpdate();
				}
			}
		}
	}
	#endregion
	#region ASPxSchedulerOptionsView
	public class ASPxSchedulerOptionsView : SchedulerOptionsViewBase {
	}
	#endregion
	#region ASPxSchedulerOptionsLoadingPanel
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ASPxSchedulerOptionsLoadingPanel : StateManager {
		public ASPxSchedulerOptionsLoadingPanel()
			: base() {
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsLoadingPanelText"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.LoadingPanelText), AutoFormatEnable, Localizable(true)]
		public string Text {
			get { return GetStringProperty("Text", ASPxperienceLocalizer.GetString(ASPxperienceStringId.Loading)); }
			set { SetStringProperty("Text", ASPxperienceLocalizer.GetString(ASPxperienceStringId.Loading), value); }
		}
	}
	#endregion
	#region ASPxSchedulerOptionsCellAutoHeight
	public class ASPxSchedulerOptionsCellAutoHeight : SchedulerNotificationOptions {
		internal const int MinDefaultHeight = 5;
		AutoHeightMode mode;
		int maxHeight;
		int minHeight;
		#region Mode
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCellAutoHeightMode"),
#endif
		DefaultValue(AutoHeightMode.None), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public AutoHeightMode Mode
		{
			get { return mode; }
			set
			{
				AutoHeightMode oldValue = Mode;
				if (oldValue == value)
					return;
				mode = value;
				OnChanged(new BaseOptionChangedEventArgs("Mode", oldValue, value));
			}
		}
		#endregion
		#region MaxHeight
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCellAutoHeightMaxHeight"),
#endif
		DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public int MaxHeight
		{
			get { return maxHeight; }
			set
			{
				int oldValue = MaxHeight;
				if (oldValue == value)
					return;
				maxHeight = value;
				OnChanged(new BaseOptionChangedEventArgs("MaxHeight", oldValue, value));
			}
		}
		#endregion
		#region MinHeight
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCellAutoHeightMinHeight"),
#endif
		DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public int MinHeight
		{
			get { return minHeight; }
			set
			{
				int oldValue = MinHeight;
				if (oldValue == value)
					return;
				minHeight = value;
				OnChanged(new BaseOptionChangedEventArgs("MinHeight", oldValue, value));
			}
		}
		#endregion
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			ASPxSchedulerOptionsCellAutoHeight optionsCellAutoHeight = options as ASPxSchedulerOptionsCellAutoHeight;
			if (optionsCellAutoHeight != null) {
				BeginUpdate();
				try {
					Mode = optionsCellAutoHeight.Mode;
					MaxHeight = optionsCellAutoHeight.MaxHeight;
					MinHeight = optionsCellAutoHeight.MinHeight;
				}
				finally {
					EndUpdate();
				}
			}
		}
		protected internal override void ResetCore() {
			Mode = AutoHeightMode.None;
			MinHeight = 0;
			MaxHeight = 0;
		}
	}
	#endregion
	public enum AutoHeightMode { None, FitToContent, LimitHeight }
	#region SchedulerDeferredScrollingOption
	public class ASPxSchedulerDeferredScrollingOption : SchedulerNotificationOptions, ISchedulerDeferredScrollingOption {
		public bool Allow {
			get {
				return false;
			}
			set {
			}
		}
		event BaseOptionChangedEventHandler ISchedulerDeferredScrollingOption.Changed { add { } remove { } }
		protected internal override void ResetCore() {
		}
	}
	#endregion
	#region TimeIndicatorDisplayOptions
	public class TimeIndicatorDisplayOptions : TimeIndicatorDisplayOptionsBase {
	}
	#endregion
}
