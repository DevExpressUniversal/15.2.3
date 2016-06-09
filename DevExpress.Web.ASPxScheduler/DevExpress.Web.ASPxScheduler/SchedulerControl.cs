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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design.WebControls;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.About;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Web.ASPxScheduler.Commands;
using DevExpress.Web.ASPxScheduler.Controls;
using DevExpress.Web.ASPxScheduler.Cookies;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Localization;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.Web.ASPxScheduler.Services;
using DevExpress.Web.ASPxScheduler.Services.Implementation;
using DevExpress.Web.ASPxScheduler.Services.Internal;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Implementation;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.Web.ASPxScheduler {
	#region ASPxScheduler
	[DXWebToolboxItem(true),
	Designer("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerDesigner, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
	ControlBuilder(typeof(SchedulerControlBuilder)),
	ToolboxData(@"<{0}:ASPxScheduler runat=""server"">
        <Views>
            <WeekView Enabled=""false""></WeekView>
            <FullWeekView Enabled=""true"">
            </FullWeekView>
        </Views>

    </{0}:ASPxScheduler>"),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxScheduler.bmp"),
	DevExpress.Utils.Design.DXClientDocumentationProvider("#AspNet/DevExpressWebASPxSchedulerScripts")
]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724")]
	public class ASPxScheduler : ASPxDataWebControl, IInnerSchedulerControlOwner, IBatchUpdateable, IBatchUpdateHandler, ISupportInitialize, ISupportAppointmentEdit, ISupportAppointmentDependencyEdit, IScriptBlockOwner, IMasterControl, IXtraSupportShouldSerialize, IXtraSupportDeserializeCollectionItem, IRequiresLoadPostDataControl, IServiceContainer, ISchedulerCommandTarget, IInnerSchedulerCommandTarget {
		protected internal const string SchedulerFireDefaultButtonHandlerName = FireDefaultButtonHandlerName;
		#region Fields
		public static string[] FormNames = new string[] { SchedulerFormNames.AppointmentForm, SchedulerFormNames.AppointmentFormEx, SchedulerFormNames.GotoDateForm, SchedulerFormNames.InplaceEditorForm, SchedulerFormNames.RecurrentAppointmentDeleteForm, SchedulerFormNames.RecurrentAppointmentEditForm, SchedulerFormNames.HorizontalAppointmentTemplate, SchedulerFormNames.HorizontalSameDayAppointmentTemplate, SchedulerFormNames.VerticalAppointmentTemplate, SchedulerFormNames.RemindersForm };
		public static string[] ToolTipsFormNames = new string[] { SchedulerFormNames.AppointmentToolTip, SchedulerFormNames.AppointmentDragToolTip, SchedulerFormNames.SelectionToolTip };
		protected internal const string SchedulerScriptCommonResourceName = "Scripts.Common.js";
		protected internal const string SchedulerScriptResourceName = "Scripts.Scheduler.js";
		protected internal const string SchedulerScriptSelectionResourceName = "Scripts.Selection.js";
		protected internal const string SchedulerScriptViewInfosResourceName = "Scripts.ViewInfos.js";
		protected internal const string SchedulerScriptClientAppointmentResourceName = "Scripts.ClientAppointment.js";
		protected internal const string SchedulerScriptGlobalFunctionsResourceName = "Scripts.GlobalFunctions.js";
		protected internal const string SchedulerScriptAPIResourceName = "Scripts.SchedulerAPI.js";
		protected internal const string SchedulerScriptMouseUtilsResourceName = "Scripts.MouseUtils.js";
		protected internal const string SchedulerScriptRecurrenceControlsResourceName = "Scripts.RecurrenceControls.js";
		protected internal const string SchedulerScriptRecurrenceTypeEditResourceName = "Scripts.RecurrenceTypeEdit.js";
		protected internal const string SchedulerScriptMouseHandlerResourceName = "Scripts.MouseHandler.js";
		protected internal const string SchedulerScriptKeyboardHandlerResourceName = "Scripts.KeyboardHandler.js";
		protected internal const string SchedulerDefaultCssResourceName = "Css.Default.css";
		protected internal const string SchedulerSpriteCssResourceName = "Css.Sprite.css";
		protected internal const string SchedulerSystemCssResourceName = "Css.System.css";
		protected internal const string AppointmentsDataHelperName = "Appointments";
		protected internal const string ResourcesDataHelperName = "Resources";
		bool isDisposed;
		BatchUpdateHelper batchUpdateHelper;
		ASPxInnerSchedulerControl innerControl;
		SchedulerEditableAppointmentInfo editableAppointment;
		ISchedulerWebViewInfoBase viewInfo;
		ASPxSchedulerStorage storage;
		StateBlock stateBlock;
		FormBlock formBlock;
		InplaceEditorFormBlock inplaceEditorFormBlock;
		MenuControlBlockBase appointmentMenuBlock;
		MenuControlBlockBase viewMenuBlock;
		ASPxSchedulerControlScriptBlock scriptBlock;
		AppointmentsBlock appointmentsBlock;
		NavigationButtonsBlock navigationButtonsBlock;
		ResourceNavigatorBlock resourceNavigatorBlock;
		ViewSelectorBlock viewSelectorBlock;
		ViewNavigatorBlock viewNavigatorBlock;
		ViewVisibleIntervalBlock viewVisibleIntervalBlock;
		CommonControlsBlock commonControlsBlock;
		ToolTipControlBlock toolTipControlBlock;
		SchedulerStatusInfoManagerBlock statusInfoManagerBlock;
		MessageBoxBlock messageBoxBlock;
		SeparatorBlock separatorBlock;
		ASPxSchedulerContainerControl containerControl;
		SchedulerCallbackCommandManager callbackCommandManager;
		SchedulerCallbackCommand currentCallbackCommand;
		ControlBlockCollection activeBlocks;
		Table mainTable;
		TableCell mainCell;
		TableCell resourceNavigatorCell;
		TableCell statusInfoCell;
		TableCell statusInfoManagerCell;
		TableCell viewSelectorCell;
		TableCell viewNavigatorCell;
		TableCell viewVisibleIntervalCell;
		TableCell separatorCell;
		TableRow separatorContainerRow;
		TableRow toolbarContainerRow;
		Table toolbarTable;
		TableCell toolbarContainerCell;
		TableRow resourceNavigatorRow;
		SchedulerTemplates templates;
		SchedulerFormType activeFormType;
		AppointmentDataProvider appointmentDataProvider;
		ASPxSchedulerOptionsCookies optionsCookies;
		ASPxSchedulerOptionsForms optionsForms;
		ASPxSchedulerOptionsToolTips optionsToolTips;
		ASPxSchedulerOptionsMenu optionsMenu;
		MasterControlDefaultImplementation masterControlDefaultImplementation;
		ResourceNavigator resourceNavigatorSettings;
		ASPxSchedulerChangeAction userChangeActions = ASPxSchedulerChangeAction.None;
		ASPxSchedulerChangeAction controlChangeActions = ASPxSchedulerChangeAction.All;
		Hashtable schedulerControlChangeTypeTable;
		string scrollState;
		Dictionary<string, HashValueBase> blockCheckSums;
		XtraSupportShouldSerializeHelper shouldSerializeHelper = new XtraSupportShouldSerializeHelper();
		bool isFormRecreated;
		InternalSchedulerCellUniqueIdProvider cellIdProvider;
		ASPxSchedulerServices serviceAccessor;
		bool ignoreFormPostData;
		ReminderState reminderState;
		bool isRenderStage;
		bool isSnapShotCreating = false;
		AppointmentSelectionKeeper selectionKeeper;
		SchedulerDataBoundLocker dataBoundLocker;
		#endregion
		#region Events
		#region CustomizeElementStyle
		static readonly object onCustomizeElementStyle = new object();
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerCustomizeElementStyle")]
#endif
		public event CustomizeElementStyleEventHandler CustomizeElementStyle {
			add { Events.AddHandler(onCustomizeElementStyle, value); }
			remove { Events.RemoveHandler(onCustomizeElementStyle, value); }
		}
		internal void RaiseCustomizeElementStyle(AppearanceStyleBase style, WebElementType type, ITimeCell timeCell, bool isAlternate) {
			CustomizeElementStyleEventHandler handler = Events[onCustomizeElementStyle] as CustomizeElementStyleEventHandler;
			if (handler == null)
				return;
			CustomizeElementStyleEventArgs args = new CustomizeElementStyleEventArgs(style, type, timeCell, isAlternate);
			handler(this, args);
		}
		#endregion
		#region InitAppointmentImages
		static readonly object onInitAppointmentImages = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerInitAppointmentImages"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event AppointmentImagesEventHandler InitAppointmentImages {
			add { Events.AddHandler(onInitAppointmentImages, value); }
			remove { Events.RemoveHandler(onInitAppointmentImages, value); }
		}
		protected internal virtual void RaiseInitAppointmentImages(AppointmentImagesEventArgs args) {
			AppointmentImagesEventHandler handler = (AppointmentImagesEventHandler)Events[onInitAppointmentImages];
			if (handler != null) handler(this, args);
		}
		#endregion
		#region InitAppointmentDisplayText
		static readonly object onInitAppointmentDisplayText = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerInitAppointmentDisplayText"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event AppointmentDisplayTextEventHandler InitAppointmentDisplayText {
			add { Events.AddHandler(onInitAppointmentDisplayText, value); }
			remove { Events.RemoveHandler(onInitAppointmentDisplayText, value); }
		}
		protected internal virtual void RaiseInitAppointmentDisplayText(AppointmentDisplayTextEventArgs e) {
			AppointmentDisplayTextEventHandler handler = (AppointmentDisplayTextEventHandler)Events[onInitAppointmentDisplayText];
			if (handler != null) handler(this, e);
		}
		#endregion
		#region AppointmentViewInfoCustomizing
		static readonly object onAppointmentViewInfoCustomizing = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentViewInfoCustomizing"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event AppointmentViewInfoCustomizingEventHandler AppointmentViewInfoCustomizing {
			add { Events.AddHandler(onAppointmentViewInfoCustomizing, value); }
			remove { Events.RemoveHandler(onAppointmentViewInfoCustomizing, value); }
		}
		protected internal virtual void RaiseAppointmentViewInfoCustomizing(AppointmentViewInfoCustomizingEventArgs e) {
			AppointmentViewInfoCustomizingEventHandler handler = (AppointmentViewInfoCustomizingEventHandler)Events[onAppointmentViewInfoCustomizing];
			if (handler != null) handler(this, e);
		}
		#endregion
		#region InitNewAppointment
		static readonly object onInitNewAppointment = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerInitNewAppointment"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event AppointmentEventHandler InitNewAppointment {
			add { Events.AddHandler(onInitNewAppointment, value); }
			remove { Events.RemoveHandler(onInitNewAppointment, value); }
		}
		protected internal virtual void RaiseInitNewAppointment(AppointmentEventArgs args) {
			AppointmentEventHandler handler = (AppointmentEventHandler)Events[onInitNewAppointment];
			if (handler != null) handler(this, args);
		}
		#endregion
		#region InitClientAppointment
		static readonly object onInitClientAppointment = new object();
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerInitClientAppointment")]
#endif
		public event InitClientAppointmentHandler InitClientAppointment {
			add { Events.AddHandler(onInitClientAppointment, value); }
			remove { Events.RemoveHandler(onInitClientAppointment, value); }
		}
		protected internal void RaiseInitClientAppointment(Appointment appointment, Dictionary<string, object> properties) {
			InitClientAppointmentEventArgs args = new InitClientAppointmentEventArgs(appointment, properties);
			InitClientAppointmentHandler handler = (InitClientAppointmentHandler)Events[onInitClientAppointment];
			if (handler != null) handler(this, args);
		}
		#endregion
		#region IsFormRecreated
		internal bool IsFormRecreated { get { return isFormRecreated; } set { isFormRecreated = value; } }
		#endregion
		#region AppointmentRowInserting
		static readonly object onAppointmentRowInserting = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentRowInserting"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event ASPxSchedulerDataInsertingEventHandler AppointmentRowInserting {
			add { Events.AddHandler(onAppointmentRowInserting, value); }
			remove { Events.RemoveHandler(onAppointmentRowInserting, value); }
		}
		protected internal virtual void RaiseAppointmentRowInserting(ASPxSchedulerDataInsertingEventArgs e) {
			ASPxSchedulerDataInsertingEventHandler handler = (ASPxSchedulerDataInsertingEventHandler)Events[onAppointmentRowInserting];
			if (handler != null) handler(this, e);
		}
		#endregion
		#region AppointmentRowInserted
		static readonly object onAppointmentRowInserted = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentRowInserted"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event ASPxSchedulerDataInsertedEventHandler AppointmentRowInserted {
			add { Events.AddHandler(onAppointmentRowInserted, value); }
			remove { Events.RemoveHandler(onAppointmentRowInserted, value); }
		}
		protected internal virtual void RaiseAppointmentRowInserted(ASPxSchedulerDataInsertedEventArgs e) {
			ASPxSchedulerDataInsertedEventHandler handler = (ASPxSchedulerDataInsertedEventHandler)Events[onAppointmentRowInserted];
			if (handler != null) handler(this, e);
		}
		#endregion
		#region AppointmentRowUpdating
		static readonly object onAppointmentRowUpdating = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentRowUpdating"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event ASPxSchedulerDataUpdatingEventHandler AppointmentRowUpdating {
			add { Events.AddHandler(onAppointmentRowUpdating, value); }
			remove { Events.RemoveHandler(onAppointmentRowUpdating, value); }
		}
		protected internal virtual void RaiseAppointmentRowUpdating(ASPxSchedulerDataUpdatingEventArgs e) {
			ASPxSchedulerDataUpdatingEventHandler handler = (ASPxSchedulerDataUpdatingEventHandler)Events[onAppointmentRowUpdating];
			if (handler != null) handler(this, e);
		}
		#endregion
		#region AppointmentRowUpdated
		static readonly object onAppointmentRowUpdated = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentRowUpdated"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event ASPxSchedulerDataUpdatedEventHandler AppointmentRowUpdated {
			add { Events.AddHandler(onAppointmentRowUpdated, value); }
			remove { Events.RemoveHandler(onAppointmentRowUpdated, value); }
		}
		protected internal virtual void RaiseAppointmentRowUpdated(ASPxSchedulerDataUpdatedEventArgs e) {
			ASPxSchedulerDataUpdatedEventHandler handler = (ASPxSchedulerDataUpdatedEventHandler)Events[onAppointmentRowUpdated];
			if (handler != null) handler(this, e);
		}
		#endregion
		#region AppointmentRowDeleting
		static readonly object onAppointmentRowDeleting = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentRowDeleting"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event ASPxSchedulerDataDeletingEventHandler AppointmentRowDeleting {
			add { Events.AddHandler(onAppointmentRowDeleting, value); }
			remove { Events.RemoveHandler(onAppointmentRowDeleting, value); }
		}
		protected internal virtual void RaiseAppointmentRowDeleting(ASPxSchedulerDataDeletingEventArgs e) {
			ASPxSchedulerDataDeletingEventHandler handler = (ASPxSchedulerDataDeletingEventHandler)Events[onAppointmentRowDeleting];
			if (handler != null) handler(this, e);
		}
		#endregion
		#region AppointmentRowDeleted
		static readonly object onAppointmentRowDeleted = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentRowDeleted"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event ASPxSchedulerDataDeletedEventHandler AppointmentRowDeleted {
			add { Events.AddHandler(onAppointmentRowDeleted, value); }
			remove { Events.RemoveHandler(onAppointmentRowDeleted, value); }
		}
		protected internal virtual void RaiseAppointmentRowDeleted(ASPxSchedulerDataDeletedEventArgs e) {
			ASPxSchedulerDataDeletedEventHandler handler = (ASPxSchedulerDataDeletedEventHandler)Events[onAppointmentRowDeleted];
			if (handler != null) handler(this, e);
		}
		#endregion
		#region AppointmentFormShowing
		static readonly object onAppointmentFormShowing = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentFormShowing"),
#endif
		Category(SRCategoryNames.Forms)]
		public event AppointmentFormEventHandler AppointmentFormShowing {
			add { Events.AddHandler(onAppointmentFormShowing, value); }
			remove { Events.RemoveHandler(onAppointmentFormShowing, value); }
		}
		protected internal virtual void RaiseAppointmentFormShowing(AppointmentFormEventArgs e) {
			AppointmentFormEventHandler handler = (AppointmentFormEventHandler)Events[onAppointmentFormShowing];
			if (handler != null) handler(this, e);
		}
		#endregion
		#region AppointmentInplaceEditorShowing
		static readonly object onAppointmentInplaceEditorShowing = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentInplaceEditorShowing"),
#endif
		Category(SRCategoryNames.Forms)]
		public event AppointmentInplaceEditorEventHandler AppointmentInplaceEditorShowing {
			add { Events.AddHandler(onAppointmentInplaceEditorShowing, value); }
			remove { Events.RemoveHandler(onAppointmentInplaceEditorShowing, value); }
		}
		protected internal virtual void RaiseAppointmentInplaceEditorFormShowing(AppointmentInplaceEditorEventArgs e) {
			AppointmentInplaceEditorEventHandler handler = (AppointmentInplaceEditorEventHandler)Events[onAppointmentInplaceEditorShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region GotoDateFormShowing
		static readonly object onGotoDateFormShowing = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerGotoDateFormShowing"),
#endif
		Category(SRCategoryNames.Forms)]
		public event GotoDateFormEventHandler GotoDateFormShowing {
			add { Events.AddHandler(onGotoDateFormShowing, value); }
			remove { Events.RemoveHandler(onGotoDateFormShowing, value); }
		}
		protected internal virtual void RaiseGotoDateFormShowing(GotoDateFormEventArgs e) {
			GotoDateFormEventHandler handler = (GotoDateFormEventHandler)Events[onGotoDateFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region RecurrentAppointmentDeleteFormShowing
		static readonly object onRecurrentAppointmentDeleteFormShowing = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerRecurrentAppointmentDeleteFormShowing"),
#endif
		Category(SRCategoryNames.Forms)]
		public event RecurrentAppointmentDeleteFormEventHandler RecurrentAppointmentDeleteFormShowing {
			add { Events.AddHandler(onRecurrentAppointmentDeleteFormShowing, value); }
			remove { Events.RemoveHandler(onRecurrentAppointmentDeleteFormShowing, value); }
		}
		protected internal virtual void RaiseRecurrentAppointmentDeleteFormShowing(RecurrentAppointmentDeleteFormEventArgs e) {
			RecurrentAppointmentDeleteFormEventHandler handler = (RecurrentAppointmentDeleteFormEventHandler)Events[onRecurrentAppointmentDeleteFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region RecurrentAppointmentEditFormShowing
		static readonly object onRecurrentAppointmentEditFormShowing = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerRecurrentAppointmentEditFormShowing"),
#endif
		Category(SRCategoryNames.Forms)]
		public event RecurrentAppointmentEditFormEventHandler RecurrentAppointmentEditFormShowing {
			add { Events.AddHandler(onRecurrentAppointmentEditFormShowing, value); }
			remove { Events.RemoveHandler(onRecurrentAppointmentEditFormShowing, value); }
		}
		protected internal virtual void RaiseRecurrentAppointmentEditFormShowing(RecurrentAppointmentEditFormEventArgs e) {
			RecurrentAppointmentEditFormEventHandler handler = (RecurrentAppointmentEditFormEventHandler)Events[onRecurrentAppointmentEditFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region RemindersFormShowing
		static readonly object onRemindersFormShowing = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerRemindersFormShowing"),
#endif
		Category(SRCategoryNames.Forms)]
		public event RemindersFormEventHandler RemindersFormShowing {
			add { Events.AddHandler(onRemindersFormShowing, value); }
			remove { Events.RemoveHandler(onRemindersFormShowing, value); }
		}
		protected internal virtual void RaiseReminderFormShowing(RemindersFormEventArgs e) {
			RemindersFormEventHandler handler = (RemindersFormEventHandler)Events[onRemindersFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region VisibleIntervalChanged
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerVisibleIntervalChanged"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event EventHandler VisibleIntervalChanged {
			add {
				if (innerControl != null)
					innerControl.VisibleIntervalChanged += value;
			}
			remove {
				if (innerControl != null)
					innerControl.VisibleIntervalChanged -= value;
			}
		}
		#endregion
		#region ActiveViewChanging
		static readonly object onActiveViewChanging = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerActiveViewChanging"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event ActiveViewChangingEventHandler ActiveViewChanging {
			add { Events.AddHandler(onActiveViewChanging, value); }
			remove { Events.RemoveHandler(onActiveViewChanging, value); }
		}
		protected internal virtual bool RaiseActiveViewChanging(SchedulerViewBase newView) {
			ActiveViewChangingEventHandler handler = (ActiveViewChangingEventHandler)Events[onActiveViewChanging];
			if (handler != null) {
				ActiveViewChangingEventArgs args = new ActiveViewChangingEventArgs(ActiveView, newView);
				handler(this, args);
				return !args.Cancel;
			}
			return true;
		}
		#endregion
		#region ActiveViewChanged
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerActiveViewChanged"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event EventHandler ActiveViewChanged {
			add {
				if (innerControl != null)
					innerControl.ActiveViewChanged += value;
			}
			remove {
				if (innerControl != null)
					innerControl.ActiveViewChanged -= value;
			}
		}
		#endregion
		#region QueryWorkTime
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerQueryWorkTime"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event QueryWorkTimeEventHandler QueryWorkTime {
			add {
				if (innerControl != null)
					innerControl.QueryWorkTime += value;
			}
			remove {
				if (innerControl != null)
					innerControl.QueryWorkTime -= value;
			}
		}
		#endregion
		#region MoreButtonClicked
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerMoreButtonClicked"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event MoreButtonClickedEventHandler MoreButtonClicked {
			add {
				if (innerControl != null)
					innerControl.MoreButtonClicked += value;
			}
			remove {
				if (innerControl != null)
					innerControl.MoreButtonClicked -= value;
			}
		}
		#endregion
		#region BeforeExecuteCallbackCommand
		static readonly object onBeforeExecuteCallbackCommand = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerBeforeExecuteCallbackCommand"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event SchedulerCallbackCommandEventHandler BeforeExecuteCallbackCommand {
			add { Events.AddHandler(onBeforeExecuteCallbackCommand, value); }
			remove { Events.RemoveHandler(onBeforeExecuteCallbackCommand, value); }
		}
		protected internal virtual void RaiseBeforeExecuteCallbackCommand(SchedulerCallbackCommandEventArgs args) {
			SchedulerCallbackCommandEventHandler handler = (SchedulerCallbackCommandEventHandler)Events[onBeforeExecuteCallbackCommand];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region AfterExecuteCallbackCommand
		static readonly object onAfterExecuteCallbackCommand = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAfterExecuteCallbackCommand"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event SchedulerCallbackCommandEventHandler AfterExecuteCallbackCommand {
			add { Events.AddHandler(onAfterExecuteCallbackCommand, value); }
			remove { Events.RemoveHandler(onAfterExecuteCallbackCommand, value); }
		}
		protected internal virtual void RaiseAfterExecuteCallbackCommand(SchedulerCallbackCommandEventArgs args) {
			SchedulerCallbackCommandEventHandler handler = (SchedulerCallbackCommandEventHandler)Events[onAfterExecuteCallbackCommand];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region CustomCallback
		static readonly object onCustomCallback = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerCustomCallback"),
#endif
		Category("Action")]
		public event CallbackEventHandlerBase CustomCallback {
			add { Events.AddHandler(onCustomCallback, value); }
			remove { Events.RemoveHandler(onCustomCallback, value); }
		}
		protected internal virtual void RaiseCustomCallback(string eventArgument) {
			CallbackEventArgsBase args = new CallbackEventArgsBase(eventArgument);
			CallbackEventHandlerBase handler = (CallbackEventHandlerBase)Events[onCustomCallback];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region HtmlTimeCellPrepared
		static readonly object onHtmlTimeCellPrepared = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerHtmlTimeCellPrepared"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event ASPxSchedulerTimeCellPreparedEventHandler HtmlTimeCellPrepared {
			add { Events.AddHandler(onHtmlTimeCellPrepared, value); }
			remove { Events.RemoveHandler(onHtmlTimeCellPrepared, value); }
		}
		protected internal virtual void RaiseHtmlTimeCellPrepared(TableCell cell, IWebTimeCell timeCell, SchedulerViewBase view) {
			ASPxSchedulerTimeCellPreparedEventHandler handler = (ASPxSchedulerTimeCellPreparedEventHandler)Events[onHtmlTimeCellPrepared];
			if (handler == null)
				return;
			ASPxSchedulerTimeCellPreparedEventArgs e = new ASPxSchedulerTimeCellPreparedEventArgs(cell, timeCell, view);
			handler(this, e);
		}
		#endregion
		#region PrepareRemindersFormPopupContainer
		static readonly object onPrepareRemindersFormPopupContainer = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrepareRemindersFormPopupContainer"),
#endif
		Category(SRCategoryNames.Forms)]
		public event ASPxSchedulerPrepareFormPopupContainerHandler PrepareRemindersFormPopupContainer {
			add { Events.AddHandler(onPrepareRemindersFormPopupContainer, value); }
			remove { Events.RemoveHandler(onPrepareRemindersFormPopupContainer, value); }
		}
		protected internal virtual void RaisePrepareRemindersFormPopupContainer(DevExpress.Web.ASPxPopupControl popupControl) {
			ASPxSchedulerPrepareFormPopupContainerHandler handler = (ASPxSchedulerPrepareFormPopupContainerHandler)Events[onPrepareRemindersFormPopupContainer];
			if (handler == null)
				return;
			ASPxSchedulerPrepareFormPopupContainerEventArgs e = new ASPxSchedulerPrepareFormPopupContainerEventArgs(popupControl);
			handler(this, e);
		}
		#endregion
		#region PrepareAppointmentFormPopupContainer
		static readonly object onPrepareAppointmentFormPopupContainer = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrepareAppointmentFormPopupContainer"),
#endif
		Category(SRCategoryNames.Forms)]
		public event ASPxSchedulerPrepareFormPopupContainerHandler PrepareAppointmentFormPopupContainer {
			add { Events.AddHandler(onPrepareAppointmentFormPopupContainer, value); }
			remove { Events.RemoveHandler(onPrepareAppointmentFormPopupContainer, value); }
		}
		protected internal virtual void RaisePrepareAppointmentFormPopupContainer(DevExpress.Web.ASPxPopupControl popupControl) {
			ASPxSchedulerPrepareFormPopupContainerHandler handler = (ASPxSchedulerPrepareFormPopupContainerHandler)Events[onPrepareAppointmentFormPopupContainer];
			if (handler == null)
				return;
			ASPxSchedulerPrepareFormPopupContainerEventArgs e = new ASPxSchedulerPrepareFormPopupContainerEventArgs(popupControl);
			handler(this, e);
		}
		#endregion
		#region PrepareAppointmentInplaceEditorPopupContainer
		static readonly object onPrepareAppointmentInplaceEditorPopupContainer = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrepareAppointmentInplaceEditorPopupContainer"),
#endif
		Category(SRCategoryNames.Forms)]
		public event ASPxSchedulerPrepareFormPopupContainerHandler PrepareAppointmentInplaceEditorPopupContainer {
			add { Events.AddHandler(onPrepareAppointmentInplaceEditorPopupContainer, value); }
			remove { Events.RemoveHandler(onPrepareAppointmentInplaceEditorPopupContainer, value); }
		}
		protected internal virtual void RaisePrepareAppointmentInplaceEditorPopupContainer(DevExpress.Web.ASPxPopupControl popupControl) {
			ASPxSchedulerPrepareFormPopupContainerHandler handler = (ASPxSchedulerPrepareFormPopupContainerHandler)Events[onPrepareAppointmentInplaceEditorPopupContainer];
			if (handler == null)
				return;
			ASPxSchedulerPrepareFormPopupContainerEventArgs e = new ASPxSchedulerPrepareFormPopupContainerEventArgs(popupControl);
			handler(this, e);
		}
		#endregion
		#region PrepareGotoDateFormPopupContainer
		static readonly object onPrepareGotoDateFormPopupContainer = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrepareGotoDateFormPopupContainer"),
#endif
		Category(SRCategoryNames.Forms)]
		public event ASPxSchedulerPrepareFormPopupContainerHandler PrepareGotoDateFormPopupContainer {
			add { Events.AddHandler(onPrepareGotoDateFormPopupContainer, value); }
			remove { Events.RemoveHandler(onPrepareGotoDateFormPopupContainer, value); }
		}
		protected internal virtual void RaisePrepareGotoDateFormPopupContainer(DevExpress.Web.ASPxPopupControl popupControl) {
			ASPxSchedulerPrepareFormPopupContainerHandler handler = (ASPxSchedulerPrepareFormPopupContainerHandler)Events[onPrepareGotoDateFormPopupContainer];
			if (handler == null)
				return;
			ASPxSchedulerPrepareFormPopupContainerEventArgs e = new ASPxSchedulerPrepareFormPopupContainerEventArgs(popupControl);
			handler(this, e);
		}
		#endregion
		#region PrepareRecurrenceAppointmentDeleteFormPopupContainer
		static readonly object onPrepareRecurrenceAppointmentDeleteFormPopupContainer = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrepareRecurrenceAppointmentDeleteFormPopupContainer"),
#endif
		Category(SRCategoryNames.Forms)]
		public event ASPxSchedulerPrepareFormPopupContainerHandler PrepareRecurrenceAppointmentDeleteFormPopupContainer {
			add { Events.AddHandler(onPrepareRecurrenceAppointmentDeleteFormPopupContainer, value); }
			remove { Events.RemoveHandler(onPrepareRecurrenceAppointmentDeleteFormPopupContainer, value); }
		}
		protected internal virtual void RaisePrepareRecurrenceAppointmentDeleteFormPopupContainer(DevExpress.Web.ASPxPopupControl popupControl) {
			ASPxSchedulerPrepareFormPopupContainerHandler handler = (ASPxSchedulerPrepareFormPopupContainerHandler)Events[onPrepareRecurrenceAppointmentDeleteFormPopupContainer];
			if (handler == null)
				return;
			ASPxSchedulerPrepareFormPopupContainerEventArgs e = new ASPxSchedulerPrepareFormPopupContainerEventArgs(popupControl);
			handler(this, e);
		}
		#endregion
		#region PrepareRecurrenceAppointmentEditFormPopupContainer
		static readonly object onPrepareRecurrenceAppointmentEditFormPopupContainer = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPrepareRecurrenceAppointmentEditFormPopupContainer"),
#endif
		Category(SRCategoryNames.Forms)]
		public event ASPxSchedulerPrepareFormPopupContainerHandler PrepareRecurrenceAppointmentEditFormPopupContainer {
			add { Events.AddHandler(onPrepareRecurrenceAppointmentEditFormPopupContainer, value); }
			remove { Events.RemoveHandler(onPrepareRecurrenceAppointmentEditFormPopupContainer, value); }
		}
		protected internal virtual void RaisePrepareRecurrenceAppointmentEditFormPopupContainer(DevExpress.Web.ASPxPopupControl popupControl) {
			ASPxSchedulerPrepareFormPopupContainerHandler handler = (ASPxSchedulerPrepareFormPopupContainerHandler)Events[onPrepareRecurrenceAppointmentEditFormPopupContainer];
			if (handler == null)
				return;
			ASPxSchedulerPrepareFormPopupContainerEventArgs e = new ASPxSchedulerPrepareFormPopupContainerEventArgs(popupControl);
			handler(this, e);
		}
		#endregion
		#region AppointmentCollectionLoaded
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentCollectionLoaded")]
#endif
		public event EventHandler AppointmentCollectionLoaded { add { Storage.AppointmentCollectionLoaded += value; } remove { Storage.AppointmentCollectionLoaded -= value; } }
		#endregion
		#region AppointmentCollectionCleared
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentCollectionCleared")]
#endif
		public event EventHandler AppointmentCollectionCleared { add { Storage.AppointmentCollectionCleared += value; } remove { Storage.AppointmentCollectionCleared -= value; } }
		#endregion
		#region AppointmentCollectionAutoReloading
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentCollectionAutoReloading")]
#endif
		public event CancelListChangedEventHandler AppointmentCollectionAutoReloading { add { Storage.AppointmentCollectionAutoReloading += value; } remove { Storage.AppointmentCollectionAutoReloading -= value; } }
		#endregion
		#region AppointmentInserting
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentInserting")]
#endif
		public event PersistentObjectCancelEventHandler AppointmentInserting { add { Storage.AppointmentInserting += value; } remove { Storage.AppointmentInserting -= value; } }
		#endregion
		#region AppointmentsInserted
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentsInserted")]
#endif
		public event PersistentObjectsEventHandler AppointmentsInserted { add { Storage.AppointmentsInserted += value; } remove { Storage.AppointmentsInserted -= value; } }
		#endregion
		#region AppointmentChanging
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentChanging")]
#endif
		public event PersistentObjectCancelEventHandler AppointmentChanging { add { Storage.AppointmentChanging += value; } remove { Storage.AppointmentChanging -= value; } }
		#endregion
		#region AppointmentsChanged
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentsChanged")]
#endif
		public event PersistentObjectsEventHandler AppointmentsChanged { add { Storage.AppointmentsChanged += value; } remove { Storage.AppointmentsChanged -= value; } }
		#endregion
		#region AppointmentDeleting
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentDeleting")]
#endif
		public event PersistentObjectCancelEventHandler AppointmentDeleting { add { Storage.AppointmentDeleting += value; } remove { Storage.AppointmentDeleting -= value; } }
		#endregion
		#region AppointmentsDeleted
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentsDeleted")]
#endif
		public event PersistentObjectsEventHandler AppointmentsDeleted { add { Storage.AppointmentsDeleted += value; } remove { Storage.AppointmentsDeleted -= value; } }
		#endregion
		#region FilterAppointment
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerFilterAppointment")]
#endif
		public event PersistentObjectCancelEventHandler FilterAppointment { add { Storage.FilterAppointment += value; } remove { Storage.FilterAppointment -= value; } }
		#endregion
		public event EventHandler<ReminderCancelEventArgs> FilterReminderAlert { add { Storage.FilterReminderAlert += value; } remove { Storage.FilterReminderAlert -= value; } }
		#region ResourceCollectionLoaded
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerResourceCollectionLoaded")]
#endif
		public event EventHandler ResourceCollectionLoaded { add { Storage.ResourceCollectionLoaded += value; } remove { Storage.ResourceCollectionLoaded -= value; } }
		#endregion
		#region ResourceCollectionCleared
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerResourceCollectionCleared")]
#endif
		public event EventHandler ResourceCollectionCleared { add { Storage.ResourceCollectionCleared += value; } remove { Storage.ResourceCollectionCleared -= value; } }
		#endregion
		#region ResourceCollectionAutoReloading
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerResourceCollectionAutoReloading")]
#endif
		public event CancelListChangedEventHandler ResourceCollectionAutoReloading { add { Storage.ResourceCollectionAutoReloading += value; } remove { Storage.ResourceCollectionAutoReloading -= value; } }
		#endregion
		#region ResourceInserting
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerResourceInserting")]
#endif
		public event PersistentObjectCancelEventHandler ResourceInserting { add { Storage.ResourceInserting += value; } remove { Storage.ResourceInserting -= value; } }
		#endregion
		#region ResourcesInserted
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerResourcesInserted")]
#endif
		public event PersistentObjectsEventHandler ResourcesInserted { add { Storage.ResourcesInserted += value; } remove { Storage.ResourcesInserted -= value; } }
		#endregion
		#region ResourceChanging
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerResourceChanging")]
#endif
		public event PersistentObjectCancelEventHandler ResourceChanging { add { Storage.ResourceChanging += value; } remove { Storage.ResourceChanging -= value; } }
		#endregion
		#region ResourcesChanged
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerResourcesChanged")]
#endif
		public event PersistentObjectsEventHandler ResourcesChanged { add { Storage.ResourcesChanged += value; } remove { Storage.ResourcesChanged -= value; } }
		#endregion
		#region ResourceDeleting
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerResourceDeleting")]
#endif
		public event PersistentObjectCancelEventHandler ResourceDeleting { add { Storage.ResourceDeleting += value; } remove { Storage.ResourceDeleting -= value; } }
		#endregion
		#region ResourcesDeleted
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerResourcesDeleted")]
#endif
		public event PersistentObjectsEventHandler ResourcesDeleted { add { Storage.ResourcesDeleted += value; } remove { Storage.ResourcesDeleted -= value; } }
		#endregion
		#region FilterResource
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerFilterResource")]
#endif
		public event PersistentObjectCancelEventHandler FilterResource { add { Storage.FilterResource += value; } remove { Storage.FilterResource -= value; } }
		#endregion
		#region ReminderAlert
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerReminderAlert")]
#endif
		public event ReminderEventHandler ReminderAlert { add { Storage.ReminderAlert += value; } remove { Storage.ReminderAlert -= value; } }
		#endregion
		#region FetchAppointments
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerFetchAppointments")]
#endif
		public event FetchAppointmentsEventHandler FetchAppointments { add { Storage.FetchAppointments += value; } remove { Storage.FetchAppointments -= value; } }
		#endregion
		#region AllowAppointmentDrag
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAllowAppointmentDrag"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentDrag {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentDrag += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentDrag -= value;
			}
		}
		#endregion
		#region AllowAppointmentDragBetweenResources
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAllowAppointmentDragBetweenResources"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentDragBetweenResources {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentDragBetweenResources += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentDragBetweenResources -= value;
			}
		}
		#endregion
		#region AllowAppointmentResize
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAllowAppointmentResize"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentResize {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentResize += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentResize -= value;
			}
		}
		#endregion
		#region AllowAppointmentCopy
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAllowAppointmentCopy"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentCopy {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentCopy += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentCopy -= value;
			}
		}
		#endregion
		#region AllowAppointmentDelete
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAllowAppointmentDelete"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentDelete {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentDelete += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentDelete -= value;
			}
		}
		#endregion
		#region AllowAppointmentCreate
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAllowAppointmentCreate"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentCreate {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentCreate += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentCreate -= value;
			}
		}
		#endregion
		#region AllowAppointmentEdit
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAllowAppointmentEdit"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentEdit {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentEdit += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentEdit -= value;
			}
		}
		#endregion
		#region AllowInplaceEditor
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAllowInplaceEditor"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowInplaceEditor {
			add {
				if (innerControl != null)
					innerControl.AllowInplaceEditor += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowInplaceEditor -= value;
			}
		}
		#endregion
		#region AllowAppointmentConflicts
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAllowAppointmentConflicts"),
#endif
		Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentConflictEventHandler AllowAppointmentConflicts {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentConflicts += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentConflicts -= value;
			}
		}
		#endregion
		#region PreparePopupMenu
		internal static readonly object onPreparePopupMenu = new object();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Category(SRCategoryNames.Scheduler), Obsolete("You should use the 'PopupMenuShowing' instead", false)]
		public event PreparePopupMenuEventHandler PreparePopupMenu {
			add { Events.AddHandler(onPreparePopupMenu, value); }
			remove { Events.RemoveHandler(onPreparePopupMenu, value); }
		}
		[Obsolete("Please use the RaisePopupMenuShowing instead.")]
		protected internal virtual void RaisePreparePopupMenu(PreparePopupMenuEventArgs args) {
			PreparePopupMenuEventHandler handler = (PreparePopupMenuEventHandler)this.Events[onPreparePopupMenu];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region PopupMenuShowing
		internal static readonly object onPopupMenuShowing = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPopupMenuShowing"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event PopupMenuShowingEventHandler PopupMenuShowing {
			add { Events.AddHandler(onPopupMenuShowing, value); }
			remove { Events.RemoveHandler(onPopupMenuShowing, value); }
		}
		protected internal virtual void RaisePopupMenuShowing(PopupMenuShowingEventArgs args) {
			PopupMenuShowingEventHandler handler = (PopupMenuShowingEventHandler)this.Events[onPopupMenuShowing];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region RemindersFormDefaultAction
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerRemindersFormDefaultAction"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event RemindersFormDefaultActionEventHandler RemindersFormDefaultAction {
			add {
				if (innerControl != null)
					innerControl.RemindersFormDefaultAction += value;
			}
			remove {
				if (innerControl != null)
					innerControl.RemindersFormDefaultAction -= value;
			}
		}
		#endregion
		#region CustomErrorText
		static readonly object onCustomErrorText = new object();
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerCustomErrorText"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event ASPxSchedulerCustomErrorTextEventHandler CustomErrorText {
			add { Events.AddHandler(onCustomErrorText, value); }
			remove { Events.RemoveHandler(onCustomErrorText, value); }
		}
		protected internal virtual void RaiseCustomErrorText(ASPxSchedulerCustomErrorTextEventArgs args) {
			ASPxSchedulerCustomErrorTextEventHandler handler = (ASPxSchedulerCustomErrorTextEventHandler)Events[onCustomErrorText];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region CustomJSProperties
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		#endregion
		#region BeforeGetCallbackResult
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerBeforeGetCallbackResult")]
#endif
		public event EventHandler BeforeGetCallbackResult {
			add { Events.AddHandler(EventBeforeGetCallbackResult, value); }
			remove { Events.RemoveHandler(EventBeforeGetCallbackResult, value); }
		}
		#endregion
		#region ClientLayout
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerClientLayout")]
#endif
		public event ASPxClientLayoutHandler ClientLayout {
			add { Events.AddHandler(EventClientLayout, value); }
			remove { Events.RemoveHandler(EventClientLayout, value); }
		}
		#endregion
		#region DateNavigatorQueryActiveViewType
		internal static readonly object onDateNavigatorQueryActiveViewType = new object();
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerDateNavigatorQueryActiveViewType")]
#endif
		public event DateNavigatorQueryActiveViewTypeHandler DateNavigatorQueryActiveViewType {
			add { Events.AddHandler(onDateNavigatorQueryActiveViewType, value); }
			remove { Events.RemoveHandler(onDateNavigatorQueryActiveViewType, value); }
		}
		protected internal virtual SchedulerViewType RaiseDateNavigatorQueryActiveViewType(SchedulerViewType oldViewType, SchedulerViewType newViewType, DayIntervalCollection selectedDays) {
			DateNavigatorQueryActiveViewTypeHandler handler = Events[onDateNavigatorQueryActiveViewType] as DateNavigatorQueryActiveViewTypeHandler;
			if (handler == null)
				return newViewType;
			DateNavigatorQueryActiveViewTypeEventArgs e = new DateNavigatorQueryActiveViewTypeEventArgs(oldViewType, newViewType, selectedDays);
			handler(this, e);
			return e.NewViewType;
		}
		#endregion
		#region UnhandledException
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerUnhandledException")]
#endif
		public event SchedulerUnhandledExceptionEventHandler UnhandledException {
			add { if (InnerControl != null) InnerControl.UnhandledException += value; }
			remove { if (InnerControl != null) InnerControl.UnhandledException -= value; }
		}
		#endregion
		internal ASPxSchedulerPropertyChangeTracker PropertyChangeTracker { get; set; }
		#endregion
		static ASPxScheduler() {
			SchedulerLocalizer.GetString(SchedulerStringId.Msg_InternalError); 
			if (SchedulerLocalizer.GetActiveLocalizerProvider() == null) {
				ActiveLocalizerProvider<SchedulerStringId> provider = new ASPxActiveLocalizerProvider<SchedulerStringId>(ASPxSchedulerCoreResLocalizer.CreateResLocalizerInstance);
				SchedulerLocalizer.SetActiveLocalizerProvider(provider);
			}
		}
		internal static void EnsureLocalizer() {
		}
		public ASPxScheduler() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
			this.dataBoundLocker = new SchedulerDataBoundLocker(this);
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.editableAppointment = new SchedulerEditableAppointmentInfo();
			this.schedulerControlChangeTypeTable = CreateSchedulerControlChangeTypeTable();
			this.resourceNavigatorSettings = new ResourceNavigator(this);
			this.masterControlDefaultImplementation = new SchedulerControlMasterControlDefaultImplementation(this);
			this.activeBlocks = new ControlBlockCollection();
			this.stateBlock = CreateStateBlock();
			this.blockCheckSums = new Dictionary<string, HashValueBase>();
			this.activeBlocks.Add(stateBlock);
			this.appointmentDataProvider = new AppointmentDataProvider(this);
			this.optionsCookies = CreateOptionsCookies();
			this.optionsForms = CreateOptionsForms();
			this.optionsToolTips = CreateOptionsToolTips();
			this.optionsMenu = CreateOptionsMenu();
			this.callbackCommandManager = CreateCallbackCommandManager();
			this.EnableCallBacks = true;
			this.EnableClientSideAPIInternal = true;
			this.storage = CreateSchedulerStorage();
			this.innerControl = CreateInnerControl();
			this.innerControl.Initialize();
			this.innerControl.Storage = storage;
			this.selectionKeeper = new AppointmentSelectionKeeper(this.innerControl);
			SubscribeInnerControlEvents();
			AddServices();
			this.serviceAccessor = new ASPxSchedulerServices(this.innerControl);
			this.templates = new SchedulerTemplates();
			this.cellIdProvider = new InternalSchedulerCellUniqueIdProvider();
			SubscribeTemplatesEvents();
			SubscribeViewsTemplatesEvents();
			SubscribeOptionsFormsEvents();
			SubscribeStorageEvents();
			SchedulerControlCreationInterceptor.Instance.RaiseControlCreated(this);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("LimitInterval", XtraShouldSerializeLimitInterval);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("ResourceColorSchemas", XtraShouldSerializeResourceColorSchemas);
			this.reminderState = CreateReminderState();
			PropertyChangeTracker = new ASPxSchedulerPropertyChangeTracker(this);
		}
		#region Properties
		bool IsPostDataLoaded { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override bool EncodeHtml { get { return false; } set { } }
		SchedulerDataBoundLocker DataBoundLocker { get { return dataBoundLocker; } }
		internal bool IsSnapShotCreating { get { return isSnapShotCreating; } set { isSnapShotCreating = value; } }
		internal ReminderState ReminderState { get { return reminderState; } }
		internal bool IgnoreFormPostData { get { return ignoreFormPostData; } set { ignoreFormPostData = value; } }
		protected internal Dictionary<string, HashValueBase> BlockCheckSums { get { return blockCheckSums; } }
		protected internal Table MainTable { get { return mainTable; } }
		protected internal TableCell MainCell { get { return mainCell; } }
		internal bool IsDisposed { get { return isDisposed; } }
		protected internal InnerSchedulerControl InnerControl { get { return innerControl; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerStorage"),
#endif
		Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable]
		public ASPxSchedulerStorage Storage { get { return storage; } }
		protected internal SchedulerEditableAppointmentInfo EditableAppointment { get { return editableAppointment; } }
		protected internal virtual Appointment GetEditedAppointment() {
			return EditableAppointment.GetAppointment(this);
		}
		protected internal virtual IDefaultUserData GetDefaultUserData() {
			return EditableAppointment.GetDefaultUserData(this);
		}
		internal AppointmentSelectionController AppointmentSelectionController {
			get {
				if (innerControl != null)
					return innerControl.AppointmentSelectionController;
				else
					return null;
			}
		}
		internal SchedulerViewSelection Selection {
			get {
				if (innerControl != null)
					return innerControl.Selection;
				else
					return null;
			}
			set {
				if (innerControl != null)
					innerControl.Selection = value;
			}
		}
		protected internal ISchedulerWebViewInfoBase ViewInfo { get { return viewInfo; } }
		protected internal StateBlock StateBlock { get { return stateBlock; } }
		protected internal FormBlock FormBlock { get { return formBlock; } }
		protected internal MenuControlBlockBase AppointmentMenuBlock { get { return appointmentMenuBlock; } }
		protected internal MenuControlBlockBase ViewMenuBlock { get { return viewMenuBlock; } }
		protected internal ASPxSchedulerControlScriptBlock ScriptBlock { get { return scriptBlock; } }
		protected internal AppointmentsBlock AppointmentsBlock { get { return appointmentsBlock; } }
		protected internal NavigationButtonsBlock NavigationButtonsBlock { get { return navigationButtonsBlock; } }
		protected internal InplaceEditorFormBlock InplaceEditorFormBlock { get { return inplaceEditorFormBlock; } }
		protected internal ResourceNavigatorBlock ResourceNavigatorBlock { get { return resourceNavigatorBlock; } }
		protected internal ViewSelectorBlock ViewSelectorBlock { get { return viewSelectorBlock; } }
		protected internal ViewNavigatorBlock ViewNavigatorBlock { get { return viewNavigatorBlock; } }
		protected internal ViewVisibleIntervalBlock ViewVisibleIntervalBlock { get { return viewVisibleIntervalBlock; } }
		protected internal ToolTipControlBlock ToolTipControlBlock { get { return toolTipControlBlock; } }
		protected internal CommonControlsBlock CommonControlsBlock { get { return commonControlsBlock; } }
		protected internal ASPxSchedulerContainerControl ContainerControl { get { return containerControl; } }
		protected internal SchedulerStatusInfoManagerBlock StatusInfoManagerBlock { get { return statusInfoManagerBlock; } }
		protected internal MessageBoxBlock MessageBoxBlock { get { return messageBoxBlock; } }
		protected internal ControlBlockCollection ActiveBlocks { get { return activeBlocks; } }
		protected internal SchedulerCallbackCommandManager CallbackCommandManager { get { return callbackCommandManager; } }
		protected internal SchedulerCallbackCommand CurrentCallbackCommand { get { return currentCallbackCommand; } }
		protected internal InternalSchedulerCellUniqueIdProvider CellIdProvider { get { return cellIdProvider; } }
		protected internal string ScrollState { get { return scrollState; } set { scrollState = value; } }
		#region ResourceColorSchemas
		[ Category(SRCategoryNames.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true), AutoFormatEnable()]
		public SchedulerColorSchemaCollection ResourceColorSchemas {
			get {
				if (innerControl != null)
					return (SchedulerColorSchemaCollection)innerControl.ResourceColorSchemas;
				else
					return null;
			}
		}
		internal bool ShouldSerializeResourceColorSchemas() {
			if (innerControl != null)
				return innerControl.ShouldSerializeResourceColorSchemas();
			else
				return false;
		}
		internal bool XtraShouldSerializeResourceColorSchemas() {
			return ShouldSerializeResourceColorSchemas();
		}
		internal void ResetResourceColorSchemas() {
			if (innerControl != null)
				innerControl.ResetResourceColorSchemas();
		}
		internal object XtraCreateResourceColorSchemasItem(XtraItemEventArgs e) {
			if (innerControl != null)
				return innerControl.XtraCreateResourceColorSchemasItem(e);
			else
				return null;
		}
		internal void XtraSetIndexResourceColorSchemasItem(XtraSetItemIndexEventArgs e) {
			if (innerControl != null)
				innerControl.XtraSetIndexResourceColorSchemasItem(e);
		}
		#endregion
		#region HideDisabledSupport
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override DisabledStyle DisabledStyle { get { return base.DisabledStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(true), AutoFormatDisable]
		public override bool Enabled { get { return base.Enabled; } set { ; } }
		#endregion
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WorkDaysCollection WorkDays {
			get {
				if (innerControl != null)
					return innerControl.WorkDays;
				else
					return null;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DayOfWeek FirstDayOfWeek {
			get {
				if (innerControl != null)
					return innerControl.FirstDayOfWeek;
				else
					return DateTimeHelper.FirstDayOfWeek;
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerViews"),
#endif
		Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable]
		public SchedulerViewRepository Views {
			get {
				if (innerControl != null)
					return (SchedulerViewRepository)innerControl.Views;
				else
					return null;
			}
		}
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public DayView DayView { get { return Views.DayView; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public WorkWeekView WorkWeekView { get { return Views.WorkWeekView; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public WeekView WeekView { get { return Views.WeekView; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public MonthView MonthView { get { return Views.MonthView; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public TimelineView TimelineView { get { return Views.TimelineView; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public FullWeekView FullWeekView { get { return Views.FullWeekView; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerViewBase ActiveView {
			get {
				if (innerControl != null)
					return (SchedulerViewBase)innerControl.ActiveView.Owner;
				else
					return null;
			}
		}
		#region Start
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerStart"),
#endif
		Category(SRCategoryNames.Scheduler), XtraSerializableProperty(999), AutoFormatDisable()]
		public DateTime Start {
			get {
				if (innerControl != null)
					return innerControl.Start;
				else
					return DateTime.MinValue;
			}
			set {
				if (innerControl != null)
					innerControl.Start = value;
			}
		}
		internal bool ShouldSerializeStart() {
			return !isSnapShotCreating;
		}
		#endregion
		#region LimitInterval
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerLimitInterval"),
#endif
Category(SRCategoryNames.Scheduler), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable()]
		public TimeInterval LimitInterval {
			get {
				if (innerControl != null)
					return innerControl.LimitInterval;
				else
					return null;
			}
			set {
				if (innerControl != null) {
					NotificationTimeInterval newInterval = (value == null) ? NotificationTimeInterval.FullRange :
						new NotificationTimeInterval(value.Start, value.Duration);
					innerControl.LimitInterval = newInterval;
				}
			}
		}
		internal bool ShouldSerializeLimitInterval() {
			return !NotificationTimeInterval.FullRange.Equals(LimitInterval);
		}
		protected internal void ResetLimitInterval() {
			this.LimitInterval = NotificationTimeInterval.FullRange;
		}
		internal bool XtraShouldSerializeLimitInterval() {
			return ShouldSerializeLimitInterval();
		}
		#endregion
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerActiveViewType"),
#endif
		DefaultValue(SchedulerViewType.Day),
		Category(SRCategoryNames.View),
		XtraSerializableProperty(XtraSerializationFlags.DefaultValue),
		AutoFormatDisable()
		]
		public SchedulerViewType ActiveViewType {
			get {
				SchedulerViewBase activeView = ActiveView;
				if (activeView != null)
					return activeView.Type;
				else
					return SchedulerViewType.Day;
			}
			set {
				if (ActiveViewType == value)
					return;
				InnerSchedulerViewBase view = Views.GetInnerView(value);
				if (view != null && !view.Enabled)
					view.Enabled = true;
				if (innerControl != null)
					innerControl.ActiveViewType = value;
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerGroupType"),
#endif
		DefaultValue(SchedulerGroupType.None), Category(SRCategoryNames.View),
		XtraSerializableProperty(XtraSerializationFlags.DefaultValue), AutoFormatDisable()]
		public SchedulerGroupType GroupType {
			get {
				SchedulerViewBase activeView = this.ActiveView;
				if (activeView != null)
					return activeView.GroupType;
				else
					return SchedulerGroupType.None;
			}
			set {
				if (Views != null)
					Views.SetGroupType(value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppointmentBaseCollection SelectedAppointments { get { return InnerControl != null ? InnerControl.SelectedAppointments : new AppointmentBaseCollection(); } }
		#region SelectedInterval
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeInterval SelectedInterval { get { return Selection != null ? InnerControl.TimeZoneHelper.FromClientTime(Selection.Interval) : TimeInterval.Empty; } }
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Resource SelectedResource { get { return Selection != null ? Selection.Resource : ResourceBase.Empty; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible { get { return base.ClientVisibleInternal; } set { base.ClientVisibleInternal = value; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerEnableCallBacks"),
#endif
		DefaultValue(true), Category("Behavior"), AutoFormatDisable]
		public bool EnableCallBacks {
			get { return EnableCallBacksInternal; }
			set { EnableCallBacksInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerEnableChangeVisibleIntervalCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableSlideCallbackAnimation), AutoFormatDisable]
		public bool EnableChangeVisibleIntervalCallbackAnimation {
			get { return EnableSlideCallbackAnimationInternal; }
			set { EnableSlideCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerEnableChangeVisibleIntervalGestures"),
#endif
		Category("Behavior"), DefaultValue(AutoBoolean.Auto), AutoFormatDisable]
		public AutoBoolean EnableChangeVisibleIntervalGestures {
			get { return EnableSwipeGesturesInternal; }
			set { EnableSwipeGesturesInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerClientSideEvents"),
#endif
		Category("Client-Side"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SchedulerClientSideEvents ClientSideEvents { get { return (SchedulerClientSideEvents)base.ClientSideEventsInternal; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerStyles"),
#endif
		Category(SRCategoryNames.Appearance), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxSchedulerStyles Styles { get { return (ASPxSchedulerStyles)StylesInternal; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxSchedulerImages Images { get { return (ASPxSchedulerImages)ImagesInternal; } }
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SchedulerTemplates Templates { get { return templates; } }
		#region Options
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCustomization"),
#endif
		Category(SRCategoryNames.Options), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public SchedulerOptionsCustomization OptionsCustomization {
			get {
				if (innerControl != null)
					return innerControl.OptionsCustomization;
				else
					return null;
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsBehavior"),
#endif
		Category(SRCategoryNames.Options), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public ASPxSchedulerOptionsBehavior OptionsBehavior {
			get {
				if (innerControl != null)
					return (ASPxSchedulerOptionsBehavior)innerControl.OptionsBehavior;
				else
					return null;
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCookies"),
#endif
		Category(SRCategoryNames.Options), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public ASPxSchedulerOptionsCookies OptionsCookies { get { return optionsCookies; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsForms"),
#endif
		Category(SRCategoryNames.Options), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public ASPxSchedulerOptionsForms OptionsForms { get { return optionsForms; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsToolTips"),
#endif
Category(SRCategoryNames.Options), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public ASPxSchedulerOptionsToolTips OptionsToolTips { get { return optionsToolTips; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsMenu"),
#endif
Category(SRCategoryNames.Options), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public ASPxSchedulerOptionsMenu OptionsMenu { get { return optionsMenu; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsView"),
#endif
		Category(SRCategoryNames.Options), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public ASPxSchedulerOptionsView OptionsView {
			get {
				if (innerControl != null)
					return (ASPxSchedulerOptionsView)innerControl.OptionsView;
				else
					return null;
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsLoadingPanel"),
#endif
Category(SRCategoryNames.Options), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SettingsLoadingPanel OptionsLoadingPanel { get { return  base.SettingsLoadingPanel; } }
		#endregion
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerResourceNavigator"),
#endif
		Category(SRCategoryNames.Behavior), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public ResourceNavigator ResourceNavigator { get { return resourceNavigatorSettings; } }
		protected internal SchedulerFormType ActiveFormType {
			get { return activeFormType; }
			set {
				if (activeFormType == value)
					return;
				activeFormType = value;
				LayoutChanged();
			}
		}
		protected internal AppointmentDataProvider AppointmentDataProvider { get { return appointmentDataProvider; } }
		#region DataSource properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string DataMember { get { return base.DataMember; } set { base.DataMember = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override object DataSource { get { return base.DataSource; } set { base.DataSource = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string DataSourceID { get { return base.DataSourceID; } set { base.DataSourceID = value; } }
		[Bindable(false), Browsable(false), Category("Data"), Themeable(false), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public virtual object AppointmentDataSource {
			get { return DataContainer[AppointmentsDataHelperName].DataSource; }
			set { DataContainer[AppointmentsDataHelperName].DataSource = value; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerAppointmentDataSourceID"),
#endif
		DefaultValue(""), Category("Data"), AutoFormatDisable, Themeable(false), Localizable(false),
		IDReferenceProperty(typeof(DataSourceControl)), TypeConverter(typeof(DataSourceIDConverter))]
		public virtual string AppointmentDataSourceID {
			get { return DataContainer[AppointmentsDataHelperName].DataSourceID; }
			set { DataContainer[AppointmentsDataHelperName].DataSourceID = value; }
		}
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Themeable(false), Category("Data"), AutoFormatDisable, Localizable(false)]
		public virtual string AppointmentDataMember {
			get { return DataContainer[AppointmentsDataHelperName].DataMember; }
			set { DataContainer[AppointmentsDataHelperName].DataMember = value; }
		}
		[Bindable(false), Browsable(false), Category("Data"), Themeable(false), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public virtual object ResourceDataSource {
			get { return DataContainer[ResourcesDataHelperName].DataSource; }
			set { DataContainer[ResourcesDataHelperName].DataSource = value; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerResourceDataSourceID"),
#endif
DefaultValue(""), Category("Data"), AutoFormatDisable, Themeable(false), Localizable(false),
		IDReferenceProperty(typeof(DataSourceControl)), TypeConverter(typeof(DataSourceIDConverter))]
		public virtual string ResourceDataSourceID {
			get { return DataContainer[ResourcesDataHelperName].DataSourceID; }
			set { DataContainer[ResourcesDataHelperName].DataSourceID = value; }
		}
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Themeable(false), Category("Data"), AutoFormatDisable, Localizable(false)]
		public virtual string ResourceDataMember {
			get { return DataContainer[ResourcesDataHelperName].DataMember; }
			set { DataContainer[ResourcesDataHelperName].DataMember = value; }
		}
		#endregion
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height { get { return base.Height; } set { } }
		internal MasterControlDefaultImplementation MasterControlDefaultImplementation { get { return masterControlDefaultImplementation; } }
		internal ASPxSchedulerChangeAction ControlChangeActions { get { return controlChangeActions; } }
		internal ASPxSchedulerChangeAction UserChangeActions { get { return userChangeActions; } }
		internal Hashtable SchedulerControlChangeTypeTable { get { return schedulerControlChangeTypeTable; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssFilePath { get { return base.CssFilePath; } set { base.CssFilePath = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssPostfix { get { return base.CssPostfix; } set { base.CssPostfix = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxSchedulerServices Services { get { return serviceAccessor; } }
		internal bool IsRenderStage { get { return isRenderStage; } }
		internal char ProtectedIdSeparator { get { return IdSeparator; } }
		internal SeparatorBlock SeparatorBlock { get { return separatorBlock; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		public TimeZoneHelper TimeZoneHelper { get { return InnerControl.TimeZoneHelper; } }
		[
		Category("Behavior"), DefaultValue(AutoBoolean.Auto), AutoFormatDisable]
		public AutoBoolean EnablePagingGestures {
			get { return EnableSwipeGesturesInternal; }
			set { EnableSwipeGesturesInternal = value; }
		}
		#endregion
		#region IDisposable implementation
		public override void Dispose() {
			try {
				this.serviceAccessor = null;
				if (innerControl != null) {
					UnsubscribeInnerControlEvents();
					innerControl.Dispose();
					innerControl = null;
				}
				if (storage != null) {
					UnsubscribeStorageEvents();
					storage.Dispose();
					storage = null;
				}
				if (templates != null) {
					UnsubscribeTemplatesEvents();
					templates = null;
				}
				if (Views != null) {
					UnsubscribeViewsTemplatesEvents();
				}
				this.viewInfo = null;
				this.stateBlock = null;
				this.formBlock = null;
				this.appointmentMenuBlock = null;
				this.viewMenuBlock = null;
				this.scriptBlock = null;
				this.appointmentsBlock = null;
				this.navigationButtonsBlock = null;
				this.inplaceEditorFormBlock = null;
				this.resourceNavigatorBlock = null;
				this.viewSelectorBlock = null;
				this.viewNavigatorBlock = null;
				this.viewVisibleIntervalBlock = null;
				this.commonControlsBlock = null;
				this.containerControl = null;
				if (activeBlocks != null) {
					this.activeBlocks.Clear();
					this.activeBlocks = null;
				}
				this.optionsCookies = null;
				if (optionsForms != null) {
					UnsubscribeOptionsFormsEvents();
					optionsForms = null;
				}
				if (this.editableAppointment != null) {
					this.editableAppointment.Dispose();
					this.editableAppointment = null;
				}
				this.callbackCommandManager = null;
				this.currentCallbackCommand = null;
				this.masterControlDefaultImplementation = null;
				this.batchUpdateHelper = null;
				if (ASPxScheduler.ActiveControl != null && ASPxScheduler.ActiveControl.ClientID == ClientID)
					ASPxScheduler.ActiveControl = null;
			} finally {
				base.Dispose();
				isDisposed = true;
			}
		}
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			innerControl.BeginUpdate();
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			innerControl.EndUpdate();
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			innerControl.CancelUpdate();
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return innerControl.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
		protected internal virtual bool HandleException(Exception e) {
			return HandleException(e, String.Empty);
		}
		protected internal virtual bool HandleException(Exception e, string subject) {
			if (InnerControl != null) {
				bool result = InnerControl.RaiseUnhandledException(e);
				if (result)
					SchedulerStatusInfoHelper.AddError(this, subject, DevExpress.Web.ASPxScheduler.Internal.ExceptionHelper.PrepareDetailedExceptionMessageAsHtml(this, e, this.OptionsBehavior.ShowDetailedErrorInfo));
				return result;
			} else
				return false;
		}
		protected internal virtual ASPxSchedulerOptionsCookies CreateOptionsCookies() {
			return new ASPxSchedulerOptionsCookies();
		}
		protected internal virtual ASPxSchedulerOptionsForms CreateOptionsForms() {
			return new ASPxSchedulerOptionsForms();
		}
		protected internal virtual ASPxSchedulerOptionsToolTips CreateOptionsToolTips() {
			return new ASPxSchedulerOptionsToolTips();
		}
		protected internal virtual ASPxSchedulerOptionsMenu CreateOptionsMenu() {
			return new ASPxSchedulerOptionsMenu();
		}
		protected override bool IsCallBacksEnabled() {
			return EnableCallBacks || IsClientSideAPIEnabled();
		}
		protected internal virtual void CreateBlocks() {
			this.formBlock = CreateFormBlock();
			this.appointmentMenuBlock = CreateAppointmentMenuBlock();
			this.viewMenuBlock = CreateViewMenuBlock();
			this.containerControl = CreateContainerControl();
			this.appointmentsBlock = CreateAppointmentsBlock();
			this.navigationButtonsBlock = CreateNavigationButtonsBlock();
			this.inplaceEditorFormBlock = CreateInplaceEditorFormBlock();
			this.resourceNavigatorBlock = CreateResourceNavigatorBlock();
			this.viewSelectorBlock = CreateViewSelectorBlock();
			this.viewNavigatorBlock = CreateViewNavigatorBlock();
			this.viewVisibleIntervalBlock = CreateViewVisibleIntervalBlock();
			this.commonControlsBlock = CreateCommonControlsBlock();
			this.scriptBlock = CreateScriptBlock();
			this.statusInfoManagerBlock = CreateStatusInfoManagerBlock();
			this.messageBoxBlock = CreateMessageBoxBlock();
			this.separatorBlock = CreateSeparatorBlock();
			this.toolTipControlBlock = CreateToolTipControlBlock();
			UpdateActiveBlocks();
		}
		protected internal virtual void UpdateActiveBlocks() {
			ActiveBlocks.Clear();
			ActiveBlocks.Add(StateBlock);
			MainCell.Controls.Add(StateBlock);
			if ((ControlChangeActions & ASPxSchedulerChangeAction.NotifyResourceIntervalChanged) != 0) {
				ActiveBlocks.Add(ResourceNavigatorBlock);
				resourceNavigatorCell.Controls.Add(ResourceNavigatorBlock);
			}
			if ((ControlChangeActions & ASPxSchedulerChangeAction.NotifyActiveViewChanged) != 0) {
				ActiveBlocks.Add(ViewSelectorBlock);
				viewSelectorCell.Controls.Add(ViewSelectorBlock);
				if (this.separatorCell != null) { 
					ActiveBlocks.Add(SeparatorBlock);
					this.separatorCell.Controls.Add(SeparatorBlock);
				}
			}
			if ((ControlChangeActions & ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged) != 0) {
				ActiveBlocks.Add(ViewNavigatorBlock);
				viewNavigatorCell.Controls.Add(ViewNavigatorBlock);
				ActiveBlocks.Add(ViewVisibleIntervalBlock);
				viewVisibleIntervalCell.Controls.Add(ViewVisibleIntervalBlock);
			}
			ActiveBlocks.Add(FormBlock);
			MainCell.Controls.Add(FormBlock);
			ActiveBlocks.Add(ContainerControl);
			MainCell.Controls.Add(ContainerControl);
			if (!FormMode) {
				if ((ControlChangeActions & ASPxSchedulerChangeAction.RenderAppointmentMenu) != 0) {
					ActiveBlocks.Add(AppointmentMenuBlock);
					MainCell.Controls.Add(AppointmentMenuBlock);
				}
				if ((ControlChangeActions & ASPxSchedulerChangeAction.RenderViewMenu) != 0) {
					ActiveBlocks.Add(ViewMenuBlock);
					MainCell.Controls.Add(ViewMenuBlock);
				}
				ActiveBlocks.Add(AppointmentsBlock);
				MainCell.Controls.Add(AppointmentsBlock);
				if (!DesignMode) {
					ActiveBlocks.Add(NavigationButtonsBlock);
					MainCell.Controls.Add(NavigationButtonsBlock);
				}
			}
			if (!DesignMode) {
				ActiveBlocks.Add(StatusInfoManagerBlock);
				statusInfoManagerCell.Controls.Add(StatusInfoManagerBlock);
				activeBlocks.Add(MessageBoxBlock);
				MainCell.Controls.Add(messageBoxBlock);
			}
			MainCell.Controls.Add(ScriptBlock);
			if (!DesignMode && (ControlChangeActions & ASPxSchedulerChangeAction.RenderCommonControls) != 0) {
				ActiveBlocks.Add(ToolTipControlBlock);
				ActiveBlocks.Add(CommonControlsBlock);
				MainCell.Controls.Add(CommonControlsBlock);
				MainCell.Controls.Add(ToolTipControlBlock);
			}
			ActiveBlocks.Add(ScriptBlock);
		}
		protected internal virtual void CalculateBlocksVisibility() {
			bool formMode = FormMode;
			ContainerControl.IsBlockVisible = !formMode;
			ResourceNavigatorBlock.IsBlockVisible = !formMode && innerControl.CalculateResourceNavigatorVisibility(ResourceNavigator.Visibility);
			ViewSelectorBlock.IsBlockVisible = !formMode && OptionsBehavior.ShowViewSelector;
			ViewNavigatorBlock.IsBlockVisible = !formMode && OptionsBehavior.ShowViewNavigator;
			ViewVisibleIntervalBlock.IsBlockVisible = !formMode && OptionsBehavior.ShowViewVisibleInterval;
			SeparatorBlock.IsBlockVisible = false;
		}
		protected internal virtual StateBlock CreateStateBlock() {
			return new StateBlock(this);
		}
		protected internal virtual FormBlock CreateFormBlock() {
			return new FormBlock(this);
		}
		protected internal virtual InplaceEditorFormBlock CreateInplaceEditorFormBlock() {
			return new InplaceEditorFormBlock(this);
		}
		protected internal virtual ResourceNavigatorBlock CreateResourceNavigatorBlock() {
			return new ResourceNavigatorBlock(this);
		}
		protected internal virtual ViewSelectorBlock CreateViewSelectorBlock() {
			return new ViewSelectorBlock(this);
		}
		protected internal virtual ViewNavigatorBlock CreateViewNavigatorBlock() {
			return new ViewNavigatorBlock(this);
		}
		protected internal virtual ViewVisibleIntervalBlock CreateViewVisibleIntervalBlock() {
			return new ViewVisibleIntervalBlock(this);
		}
		protected internal virtual ToolTipControlBlock CreateToolTipControlBlock() {
			return new ToolTipControlBlock(this);
		}
		protected internal virtual CommonControlsBlock CreateCommonControlsBlock() {
			return new CommonControlsBlock(this);
		}
		protected internal virtual MenuControlBlockBase CreateAppointmentMenuBlock() {
			return new AppointmentMenuBlock(this);
		}
		protected internal virtual MenuControlBlockBase CreateViewMenuBlock() {
			return new ViewMenuBlock(this);
		}
		protected internal virtual ASPxSchedulerControlScriptBlock CreateScriptBlock() {
			return new ASPxSchedulerControlScriptBlock(this, this);
		}
		protected internal virtual AppointmentsBlock CreateAppointmentsBlock() {
			return ActiveView.FactoryHelper.CreateAppointmentsBlock(this);
		}
		protected internal virtual NavigationButtonsBlock CreateNavigationButtonsBlock() {
			return new NavigationButtonsBlock(this);
		}
		protected internal virtual SchedulerStatusInfoManagerBlock CreateStatusInfoManagerBlock() {
			return new SchedulerStatusInfoManagerBlock(this);
		}
		protected internal virtual MessageBoxBlock CreateMessageBoxBlock() {
			return new MessageBoxBlock(this);
		}
		protected internal virtual SeparatorBlock CreateSeparatorBlock() {
			return new SeparatorBlock(this);
		}
		protected internal virtual ASPxSchedulerContainerControl CreateContainerControl() {
			return new ASPxSchedulerContainerControl(this);
		}
		protected internal virtual void SuspendEvents() {
			innerControl.SuspendEvents();
		}
		protected internal virtual void ResumeEvents() {
			innerControl.ResumeEvents();
		}
		protected internal virtual bool IsEventsSuspended { get { return innerControl.IsEventsSuspended; } }
		#region Active control support
		[ThreadStatic()]
		static ASPxScheduler activeControl;
#if DEBUGTEST
		protected internal static ASPxScheduler ActiveControl { get { return activeControl; } set { activeControl = value; } }
#else
		static object SchedulerContextKey = new object();
		protected internal static ASPxScheduler ActiveControl {
			get {
				if (System.Web.HttpContext.Current == null)
					return activeControl;
				return System.Web.HttpContext.Current.Items[SchedulerContextKey] as ASPxScheduler;
			}
			set {
				if (System.Web.HttpContext.Current == null)
					activeControl = value;
				else
					System.Web.HttpContext.Current.Items[SchedulerContextKey] = value;
			}
		}
#endif
		#endregion
		#region ISupportAppointmentEdit implementation
		void ISupportAppointmentEdit.SelectNewAppointment(Appointment apt) {
		}
		void ISupportAppointmentEdit.ShowAppointmentForm(Appointment apt, bool openRecurrenceForm, bool readOnly, CommandSourceType commandSourceType) {
			SetEditableAppointment(apt);
			ActiveFormType = SchedulerFormType.Appointment;
			EnsureProcessToolbarBlocks();
			LayoutChanged();
		}
		void ISupportAppointmentEdit.BeginEditNewAppointment(Appointment apt) {
			NonpermanentAppointmentIdHelper idHelper = new NonpermanentAppointmentIdHelper();
			apt.SetId(idHelper.GetNextId());
			SetEditableAppointment(apt);
			ActiveFormType = SchedulerFormType.AppointmentInplace;
			EnsureProcessToolbarBlocks();
			LayoutChanged();
		}
		void ISupportAppointmentEdit.RaiseInitNewAppointmentEvent(Appointment apt) {
			AppointmentEventArgs args = new AppointmentEventArgs(apt);
			RaiseInitNewAppointment(args);
		}
		#endregion
		void ISupportAppointmentDependencyEdit.ShowAppointmentDependencyForm(AppointmentDependency dep, bool readOnly, CommandSourceType commandSourceType) {
		}
		#region SetEditableAppointment
		protected internal virtual void SetEditableAppointment(Appointment apt) {
			if (Storage.Appointments.IsNewAppointment(apt))
				EditableAppointment.SetNewInstance(apt);
			else
				EditableAppointment.SetExistingReference(apt);
		}
		protected internal virtual void SetEditableAppointment(Appointment apt, IDefaultUserData data) {
			EditableAppointment.SetExistingReference(apt, data);
		}
		#endregion
		#region ShowForm
		protected internal virtual void ShowForm(SchedulerFormType formType) {
			ActiveFormType = formType;
			EnsureProcessToolbarBlocks();
			LayoutChanged();
		}
		#endregion
		#region CreateAppointment
		public void CreateAppointment() {
			WebNewAppointmentCommand newAppointmentCommand = new WebNewAppointmentCommand(this);
			newAppointmentCommand.Execute();
		}
		#endregion
		protected internal virtual void EnsureProcessToolbarBlocks() {
			this.controlChangeActions |= GetToolbarBlocksChangeAction();
		}
		protected internal virtual ASPxSchedulerChangeAction GetToolbarBlocksChangeAction() {
			return ASPxSchedulerChangeAction.NotifyActiveViewChanged | ASPxSchedulerChangeAction.NotifyResourceIntervalChanged | ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged;
		}
		protected internal virtual ASPxSchedulerStorage CreateSchedulerStorage() {
			ASPxScheduler oldActiveControl = ASPxScheduler.ActiveControl;
			ASPxScheduler.ActiveControl = this;
			try {
				return CreateSchedulerStorageCore();
			} finally {
				ASPxScheduler.ActiveControl = oldActiveControl;
			}
		}
		protected internal virtual ASPxSchedulerStorage CreateSchedulerStorageCore() {
			return new ASPxSchedulerStorage(this);
		}
		#region SubscribeInnerControlEvents
		protected internal virtual void SubscribeInnerControlEvents() {
			innerControl.AppointmentsSelectionChanged += new EventHandler(OnAppointmentsSelectionChanged);
			innerControl.ReminderAlert += new ReminderEventHandler(OnReminderAlert);
			innerControl.ActiveViewChanging += new InnerActiveViewChangingEventHandler(OnActiveViewChanging);
			innerControl.AfterApplyChanges += new AfterApplyChangesEventHandler(OnAfterApplyChanges);
			innerControl.BeforeApplyChanges += new AfterApplyChangesEventHandler(OnBeforeApplyChanges);
		}
		#endregion
		#region UnsubscribeInnerControlEvents
		protected internal virtual void UnsubscribeInnerControlEvents() {
			innerControl.AppointmentsSelectionChanged -= new EventHandler(OnAppointmentsSelectionChanged);
			innerControl.ReminderAlert -= new ReminderEventHandler(OnReminderAlert);
			innerControl.ActiveViewChanging -= new InnerActiveViewChangingEventHandler(OnActiveViewChanging);
			innerControl.BeforeApplyChanges -= new AfterApplyChangesEventHandler(OnBeforeApplyChanges);
			innerControl.AfterApplyChanges -= new AfterApplyChangesEventHandler(OnAfterApplyChanges);
		}
		#endregion
		protected internal virtual void SubscribeStorageEvents() {
			((IInternalSchedulerStorageBase)Storage).AfterFetchAppointments += new EventHandler(OnAfterFetchAppointments);
		}
		protected internal virtual void UnsubscribeStorageEvents() {
			((IInternalSchedulerStorageBase)Storage).AfterFetchAppointments -= new EventHandler(OnAfterFetchAppointments);
		}
		protected internal virtual void OnAfterFetchAppointments(object sender, EventArgs e) {
			PerformSelect(AppointmentsDataHelperName);
			InvalidateAppointmentCache();
		}
		void InvalidateAppointmentCache() {
			((IInternalSchedulerStorageBase)Storage).AppointmentCache.OnAppointmentCollectionLoaded();
		}
		#region SubscribeAppointmentContentLayoutCalculatorEvents
		protected internal virtual void SubscribeAppointmentContentLayoutCalculatorEvents(AppointmentContentLayoutCalculator calculator) {
			if (onInitAppointmentImages != null)
				calculator.InitAppointmentImages += new AppointmentImagesEventHandler(this.OnInitAppointmentImages);
			if (onInitAppointmentDisplayText != null)
				calculator.InitAppointmentDisplayText += new AppointmentDisplayTextEventHandler(this.OnInitAppointmentDisplayText);
			if (onAppointmentViewInfoCustomizing != null)
				calculator.AppointmentViewInfoCustomizing += new AppointmentViewInfoCustomizingEventHandler(this.OnAppointmentViewInfoCustomizing);
		}
		#endregion
		#region UnsubscribeAppointmentContentLayoutCalculatorEvents
		protected internal virtual void UnsubscribeAppointmentContentLayoutCalculatorEvents(AppointmentContentLayoutCalculator calculator) {
			calculator.InitAppointmentImages -= new AppointmentImagesEventHandler(this.OnInitAppointmentImages);
			calculator.InitAppointmentDisplayText -= new AppointmentDisplayTextEventHandler(this.OnInitAppointmentDisplayText);
			calculator.AppointmentViewInfoCustomizing -= new AppointmentViewInfoCustomizingEventHandler(this.OnAppointmentViewInfoCustomizing);
		}
		#endregion
		protected internal virtual void OnAppointmentsSelectionChanged(object sender, EventArgs e) {
		}
		protected internal virtual void OnActiveViewChanging(object sender, InnerActiveViewChangingEventArgs e) {
			SchedulerViewBase newView = Views[e.NewView.Type];
			e.Cancel = !RaiseActiveViewChanging(newView);
		}
		protected internal virtual void OnBeforeApplyChanges(object sender, AfterApplyChangesEventArgs e) {
			this.selectionKeeper.SaveSelection();
		}
		protected internal virtual void OnAfterApplyChanges(object sender, AfterApplyChangesEventArgs e) {
			List<SchedulerControlChangeType> changeTypes = e.ChangeTypes;
			int count = changeTypes.Count;
			for (int i = 0; i < count; i++) {
				ASPxSchedulerChangeAction action = TranslateSchedulerControlChangeType(changeTypes[i]);
				this.controlChangeActions |= action;
				if (IsPostDataLoaded)
					this.userChangeActions |= action;
			}
			if ((e.Actions & ChangeActions.RaiseVisibleIntervalChanged) != 0) {
				if (this.selectionKeeper.IsSaved)
					this.selectionKeeper.RestoreSelection();
				else if (stateBlock != null)
					stateBlock.ApplyAppointmentSelectionState();
			}
			this.selectionKeeper.Reset();
			LayoutChanged();
		}
		#region OnReminderAlert
		protected internal virtual void OnReminderAlert(object sender, ReminderEventArgs e) {
			if (OptionsBehavior.ShowRemindersForm) {
				ReminderState.Reset();
				ReminderAlertNotificationCollection alertNotifications = e.AlertNotifications;
				int count = alertNotifications.Count;
				for (int i = 0; i < count; i++) {
					ReminderAlertNotification alertNotification = alertNotifications[i];
					if (alertNotification.Ignore == true && alertNotification.Handled == true)
						ReminderState.AddIgnoredReminder(alertNotification.Reminder);
					else
						ReminderState.AddReminder(alertNotification.Reminder);
					alertNotification.Handled = true;
				}
			}
		}
		#endregion
		protected virtual ReminderState CreateReminderState() {
			return new ReminderState(this);
		}
		protected internal virtual ASPxSchedulerChangeAction TranslateSchedulerControlChangeType(SchedulerControlChangeType changeType) {
			return (ASPxSchedulerChangeAction)schedulerControlChangeTypeTable[changeType];
		}
		protected internal virtual Hashtable CreateSchedulerControlChangeTypeTable() {
			Hashtable result = new Hashtable();
			result[SchedulerControlChangeType.None] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.ActiveViewChanged] = ASPxSchedulerChangeAction.NotifyActiveViewChanged | ASPxSchedulerChangeAction.NotifyResourceIntervalChanged | ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.DataBindResources | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderViewMenu;
			result[SchedulerControlChangeType.AppearanceChanged] = ASPxSchedulerChangeAction.RenderAppointmentMenu | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderViewMenu | ASPxSchedulerChangeAction.RenderCommonControls;
			result[SchedulerControlChangeType.AppointmentDisplayOptionsChanged] = ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderState;
			result[SchedulerControlChangeType.BorderStyleChanged] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.BoundsChanged] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.CompressWeekendChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderAppointments;
			result[SchedulerControlChangeType.ControlStartChanged] = ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.DateTimeScroll] = ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.DayCountChanged] = ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.ExternalAppointmentImages] = ASPxSchedulerChangeAction.RenderAppointments;
			result[SchedulerControlChangeType.FirstVisibleResourceIndexChanged] = ASPxSchedulerChangeAction.NotifyResourceIntervalChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.DataBindResources | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.GroupTypeChanged] = ASPxSchedulerChangeAction.NotifyResourceIntervalChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.DataBindResources | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.LimitIntervalChanged] = ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.LookAndFeelChanged] = ASPxSchedulerChangeAction.RenderAppointmentMenu | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderViewMenu | ASPxSchedulerChangeAction.RenderCommonControls;
			result[SchedulerControlChangeType.NavigationButtonVisibilityChanged] = ASPxSchedulerChangeAction.RenderNavigationButtons;
			result[SchedulerControlChangeType.NavigationButtonAppointmentSearchIntervalChanged] = ASPxSchedulerChangeAction.RenderNavigationButtons;
			result[SchedulerControlChangeType.PerformViewLayoutChanged] = ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderNavigationButtons;
			result[SchedulerControlChangeType.ResourceColorSchemasChanged] = ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.ResourceScroll] = ASPxSchedulerChangeAction.NotifyResourceIntervalChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.DataBindResources | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.ResourcesPerPageChanged] = ASPxSchedulerChangeAction.NotifyResourceIntervalChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.DataBindResources | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.ScrollStartTimeChanged] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.SelectionBarOptionsChanged] = ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.SelectionChanged] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.ShowAllDayAreaChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderAppointments;
			result[SchedulerControlChangeType.ShowAllAppointmentsAtTimeCellsChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderAppointments;
			result[SchedulerControlChangeType.ShowDayViewDayHeadersChanged] = ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.ShowFullWeekChanged] = ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.ShowMoreButtonsChanged] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.CellsAutoHeightChanged] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.ShowMoreButtonsOnEachColumnChanged] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.ShowWeekendChanged] = ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.ShowWorkTimeOnlyChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderAppointments;
			result[SchedulerControlChangeType.StatusLineWidthChanged] = ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.StorageChanged] = ASPxSchedulerChangeAction.NotifyResourceIntervalChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.DataBindResources | ASPxSchedulerChangeAction.RenderAppointmentMenu | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.TimelineScalesChanged] = ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.TimeRulersChanged] = ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.TimeScaleChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderAppointments;
			result[SchedulerControlChangeType.VisibleIntervalsChanged] = ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.VisibleTimeChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderAppointments;
			result[SchedulerControlChangeType.VisibleTimeSnapModeChanged] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.ScrollStartTimeChangedSystemScroll] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.WeekCountChanged] = ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.WorkTimeChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderAppointments;
			result[SchedulerControlChangeType.DateTimeScrollbarVisibilityChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderState;
			result[SchedulerControlChangeType.RowHeightChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderState;
			result[SchedulerControlChangeType.WorkDaysChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons;
			result[SchedulerControlChangeType.StorageAppointmentsChanged] = ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderNavigationButtons;
			result[SchedulerControlChangeType.DayViewActiveStorageAppointmentsChanged] = result[SchedulerControlChangeType.StorageAppointmentsChanged];
			result[SchedulerControlChangeType.StorageAppointmentMappingsChanged] = ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderViewMenu | ASPxSchedulerChangeAction.RenderState;
			result[SchedulerControlChangeType.StorageUIObjectsChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderAppointmentMenu | ASPxSchedulerChangeAction.RenderState;
			result[SchedulerControlChangeType.StorageResourcesChanged] = ASPxSchedulerChangeAction.DataBindResources | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.NotifyResourceIntervalChanged;
			result[SchedulerControlChangeType.StorageResourceMappingsChanged] = ASPxSchedulerChangeAction.DataBindResources | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.NotifyResourceIntervalChanged;
			result[SchedulerControlChangeType.StorageDeferredNotifications] = ASPxSchedulerChangeAction.RenderAppointmentMenu | ASPxSchedulerChangeAction.RenderViewMenu | ASPxSchedulerChangeAction.DataBindResources | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.NotifyResourceIntervalChanged;
			result[SchedulerControlChangeType.UserPreferenceChanged] = ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.UserPreferenceChangedTimeline] = ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.SystemTimeChanged] = ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.EndInit] = ASPxSchedulerChangeAction.NotifyActiveViewChanged | ASPxSchedulerChangeAction.RenderAppointmentMenu | ASPxSchedulerChangeAction.RenderViewMenu | ASPxSchedulerChangeAction.DataBindResources | ASPxSchedulerChangeAction.DataBindAppointments | ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.NotifyResourceIntervalChanged;
			result[SchedulerControlChangeType.ResourceNavigatorVisibilityChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderState;
			result[SchedulerControlChangeType.OptionsViewChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged | ASPxSchedulerChangeAction.RenderNavigationButtons;
			result[SchedulerControlChangeType.ViewEnabledChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.NotifyActiveViewChanged;
			result[SchedulerControlChangeType.OptionsCustomizationChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderViewMenu;
			result[SchedulerControlChangeType.OptionsBehaviorChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.NotifyTimeZoneChanged;
			result[SchedulerControlChangeType.TimelineIntervalCountChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged;
			result[SchedulerControlChangeType.FormatStringsChanged] = ASPxSchedulerChangeAction.RenderView | ASPxSchedulerChangeAction.RenderState | ASPxSchedulerChangeAction.RenderAppointments | ASPxSchedulerChangeAction.RenderNavigationButtons | ASPxSchedulerChangeAction.RenderCommonControls | ASPxSchedulerChangeAction.StringFormatsChanged;
			result[SchedulerControlChangeType.CellContentScroll] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.AppointmentDragging] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.ScrollBarVisibilityChanged] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.ResourceExpandedChanged] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.StorageAppointmentDependenciesChanged] = ASPxSchedulerChangeAction.None;
			result[SchedulerControlChangeType.ShowResourceHeadersChanged] = ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.TimeIndicatorDisplayOptionsChanged] = ASPxSchedulerChangeAction.RenderView;
			result[SchedulerControlChangeType.TimeMarkerVisibilityChanged] = ASPxSchedulerChangeAction.RenderView;
			return result;
		}
		protected internal virtual void OnInitAppointmentImages(object sender, AppointmentImagesEventArgs e) {
			RaiseInitAppointmentImages(e);
		}
		protected internal virtual void OnInitAppointmentDisplayText(object sender, AppointmentDisplayTextEventArgs e) {
			RaiseInitAppointmentDisplayText(e);
		}
		protected internal virtual void OnAppointmentViewInfoCustomizing(object sender, AppointmentViewInfoCustomizingEventArgs e) {
			RaiseAppointmentViewInfoCustomizing(e);
		}
		#region ISupportInitialize implementation
		public void BeginInit() {
			UnsubscribeInnerControlEvents();
			SuspendEvents();
			innerControl.BeginUpdate();
			((ISupportInitialize)storage).BeginInit();
			innerControl.BeginInit();
		}
		public void EndInit() {
			if (!innerControl.IsUpdateLocked)
				return;
			if (DayView.TimeRulers.Count <= 0)
				DayView.TimeRulers.Add(new TimeRuler());
			if (WorkWeekView.TimeRulers.Count <= 0)
				WorkWeekView.TimeRulers.Add(new TimeRuler());
			if (FullWeekView.TimeRulers.Count <= 0)
				FullWeekView.TimeRulers.Add(new TimeRuler());
			innerControl.EndInit();
			((ISupportInitialize)storage).EndInit();
			ResetSeletionToDefault();
			StateBlock.ApplyAppointmentSelectionState();
			StateBlock.ApplyRemindersState();
			innerControl.EndUpdate();
			ResumeEvents();
			SubscribeInnerControlEvents();
		}
		protected virtual void ResetSeletionToDefault() {
			if (InnerControl.ActiveViewType == SchedulerViewType.Timeline)
				InnerControl.ActiveView.InitializeSelection(InnerControl.Selection);
		}
		#endregion
		protected override string GetSkinControlName() { return "Scheduler"; } 
		protected override string[] GetChildControlNames() {
			return new string[] { "Web", "Editors" };
		}
		public override void ApplyStyleSheetSkin(Page page) {
			base.ApplyStyleSheetSkin(page);
			if (IsForceBeginInitAfterApplyingSkin())
				BeginInit();
		}
		protected virtual bool IsForceBeginInitAfterApplyingSkin() {
			return true;
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			EndInit();
		}
		#region IInnerSchedulerControlOwner implementation
		ISupportAppointmentEdit IInnerSchedulerControlOwner.SupportAppointmentEdit { get { return this; } }
		ISupportAppointmentDependencyEdit IInnerSchedulerControlOwner.SupportAppointmentDependencyEdit { get { return this; } }
		AppointmentChangeHelper IInnerSchedulerControlOwner.CreateAppointmentChangeHelper(InnerSchedulerControl innerControl) {
			return new AppointmentChangeHelper(innerControl);
		}
		void IInnerSchedulerControlOwner.RecalcDraggingAppointmentPosition() {
		}
		void IInnerSchedulerControlOwner.ShowPrintPreview() {
		}
		SchedulerMouseHandler IInnerSchedulerControlOwner.CreateMouseHandler(InnerSchedulerControl control) {
			return new ASPxSchedulerMouseHandler(control);
		}
		NormalKeyboardHandlerBase IInnerSchedulerControlOwner.CreateKeyboardHandler() {
			return new ASPxNormalKeyboardHandler();
		}
		bool IInnerSchedulerControlOwner.ChangeDateTimeScrollBarOrientationIfNeeded() {
			return false;
		}
		bool IInnerSchedulerControlOwner.ChangeDateTimeScrollBarVisibilityIfNeeded() {
			return false;
		}
		bool IInnerSchedulerControlOwner.ChangeResourceScrollBarOrientationIfNeeded() {
			return false;
		}
		bool IInnerSchedulerControlOwner.ChangeResourceScrollBarVisibilityIfNeeded() {
			return false;
		}
		SchedulerViewRepositoryBase IInnerSchedulerControlOwner.CreateViewRepository() {
			return new SchedulerViewRepository();
		}
		ISchedulerInplaceEditController IInnerSchedulerControlOwner.CreateInplaceEditController() {
			return this.CreateInplaceEditController();
		}
		AppointmentSelectionController IInnerSchedulerControlOwner.CreateAppointmentSelectionController() {
			return this.CreateAppointmentSelectionController();
		}
		SchedulerOptionsBehaviorBase IInnerSchedulerControlOwner.CreateOptionsBehavior() {
			return new ASPxSchedulerOptionsBehavior();
		}
		SchedulerOptionsViewBase IInnerSchedulerControlOwner.CreateOptionsView() {
			return new ASPxSchedulerOptionsView();
		}
		bool IInnerSchedulerControlOwner.IsDateTimeScrollbarVisibilityDependsOnClientSize() {
			return false;
		}
		bool IInnerSchedulerControlOwner.ObtainDateTimeScrollBarVisibility() {
			return false;
		}
		void IInnerSchedulerControlOwner.RecalcClientBounds() {
		}
		void IInnerSchedulerControlOwner.RecalcFinalLayout() {
		}
		void IInnerSchedulerControlOwner.RecalcScrollBarVisibility() {
		}
		void IInnerSchedulerControlOwner.RecalcPreliminaryLayout() {
		}
		void IInnerSchedulerControlOwner.ClearPreliminaryAppointmentsAndCellContainers() {
		}
		void IInnerSchedulerControlOwner.RecreateViewInfo() {
		}
		ChangeActions IInnerSchedulerControlOwner.PrepareChangeActions() {
			return ChangeActions.None;
		}
		void IInnerSchedulerControlOwner.RecalcAppointmentsLayout() {
		}
		void IInnerSchedulerControlOwner.RecalcViewBounds() {
		}
		void IInnerSchedulerControlOwner.UpdateDateTimeScrollBarValue() {
		}
		void IInnerSchedulerControlOwner.UpdatePaintStyle() {
		}
		void IInnerSchedulerControlOwner.UpdateResourceScrollBarValue() {
		}
		void IInnerSchedulerControlOwner.UpdateScrollBarsPosition() {
		}
		void IInnerSchedulerControlOwner.EnsureCalculationsAreFinished() {
		}
		void IInnerSchedulerControlOwner.RepaintView() {
		}
		void IInnerSchedulerControlOwner.UpdateScrollMoreButtonsVisibility() {
		}
		RecurrentAppointmentAction IInnerSchedulerControlOwner.ShowEditRecurrentAppointmentForm(Appointment apt, bool readOnly, CommandSourceType commandSourceType) {
			return RecurrentAppointmentAction.Cancel;
		}
		void IInnerSchedulerControlOwner.ShowGotoDateForm(DateTime date) {
		}
		RecurrentAppointmentAction IInnerSchedulerControlOwner.ShowDeleteRecurrentAppointmentForm(Appointment apt) {
			return RecurrentAppointmentAction.Cancel;
		}
		RecurrentAppointmentAction IInnerSchedulerControlOwner.ShowDeleteRecurrentAppointmentsForm(AppointmentBaseCollection apts) {
			return OptionsBehavior.RecurrentAppointmentDeleteAction;
		}
		bool IInnerSchedulerControlOwner.QueryDeleteForEachRecurringAppointment { get { return true; } }
		ISchedulerStorageBase IInnerSchedulerControlOwner.Storage {
			get { return this.Storage; }
		}
		event EventHandler IInnerSchedulerControlOwner.StorageChanged { add { ; } remove { ; } }
		event EventHandler ICommandAwareControl<SchedulerCommandId>.BeforeDispose { add { ;} remove { ; } }
		#endregion
		#region IPostBackEventHandler overridables
		protected override void RaisePostBackEvent(string eventArgument) {
			try {
				ExecuteCallback(eventArgument);
			} catch (Exception ex) {
				SchedulerStatusInfoHelper.AddError(this, DevExpress.Web.ASPxScheduler.Internal.ExceptionHelper.PrepareDetailedExceptionMessageAsHtml(this, ex, this.OptionsBehavior.ShowDetailedErrorInfo));
			}
		}
		#endregion
		#region ICallbackEventHandler overridables
		protected override object GetCallbackResult() {
			try {
				ASPxScheduler oldActiveControl = ASPxScheduler.ActiveControl;
				ASPxScheduler.ActiveControl = this;
				try {
					Hashtable result = new Hashtable();
					result[CallbackResultProperties.Result] = GetCallbackResultCore();
					result[CallbackResultProperties.StateObject] = GetClientObjectState();
					System.Diagnostics.Debug.WriteLine(String.Format("GetCallbackResult: callback result is {0}, chars long", ((string)result[CallbackResultProperties.Result]).Length));
					return result;
				} finally {
					ASPxScheduler.ActiveControl = oldActiveControl;
				}
			} catch (Exception ex) {
				if (!HandleException(ex))
					throw;
				return GetErrorCallbackResult();
			}
		}
		protected virtual string GetCallbackResultCore() {
			SchedulerFunctionalCallbackCommand command = CurrentCallbackCommand as SchedulerFunctionalCallbackCommand;
			if (command != null)
				try {
					return command.GetCallbackResult();
				} finally {
					this.currentCallbackCommand = null;
				}
			EnsureChildControls();
			StringBuilder result = new StringBuilder();
			IMasterControl masterControl = this;
			BeginRendering();
			try {
				result.Append(activeBlocks.GetHtmlCallbackResult(this));
				if (!ScriptBlock.IsExceptionHandled)
					result.Append(masterControl.CalcRelatedControlsCallbackResult());
			} finally {
				EndRendering();
			}
			return result.ToString();
		}
		string GetErrorCallbackResult() {
			SetHandledException();
			return activeBlocks.GetHtmlCallbackResult(this);
		}
		protected internal string GetCallbackErrorMessage(Exception ex) {
			return base.OnCallbackException(ex);
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			System.Diagnostics.Debug.WriteLine("RaiseCallbackEvent: " + eventArgument);
			try {
				this.controlChangeActions = CalculateCallbackControlChangeActions();
				ExecuteCallback(eventArgument);
			} catch (Exception ex) {
				if (!HandleException(ex))
					throw;
				else
					SetHandledException();
			}
		}
		protected override string OnCallbackException(Exception e) {
			return DevExpress.Web.ASPxScheduler.Internal.ExceptionHelper.GetDetailedExceptionMessageAsHtml(this, e, this.OptionsBehavior.ShowDetailedErrorInfo);
		}
		void SetHandledException() {
			activeBlocks.Clear();
			ScriptBlock.IsExceptionHandled = true;
			activeBlocks.Add(FormBlock);
			activeBlocks.Add(StatusInfoManagerBlock);
			activeBlocks.Add(ScriptBlock);
		}
		protected internal virtual ASPxSchedulerChangeAction CalculateCallbackControlChangeActions() {
			ASPxSchedulerChangeAction result = this.controlChangeActions;
			result &= ~GetCallbackInitiallyDisabledActions();
			result |= userChangeActions;
			if (Events[onPreparePopupMenu] != null)
				result |= ASPxSchedulerChangeAction.RenderViewMenu | ASPxSchedulerChangeAction.RenderAppointmentMenu;
			return result;
		}
		protected internal virtual ASPxSchedulerChangeAction GetCallbackInitiallyDisabledActions() {
			ASPxSchedulerChangeAction result = ASPxSchedulerChangeAction.None;
			result |= ASPxSchedulerChangeAction.RenderViewMenu;
			result |= ASPxSchedulerChangeAction.RenderAppointmentMenu;
			result |= ASPxSchedulerChangeAction.NotifyActiveViewChanged;
			result |= ASPxSchedulerChangeAction.NotifyResourceIntervalChanged;
			result |= ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged;
			result |= ASPxSchedulerChangeAction.NotifyTimeZoneChanged;
			result |= ASPxSchedulerChangeAction.RenderCommonControls;
			return result;
		}
		#endregion
		#region Callback commands implementation
		protected internal virtual void ExecuteCallback(string eventArgument) {
			CallbackCommandInfo commandInfo = CreateCommandInfo(eventArgument);
			if (commandInfo == null)
				return;
			ExecuteCallbackCommand(commandInfo);
		}
		protected internal virtual CallbackCommandInfo CreateCommandInfo(string callbackArgument) {
			if (String.IsNullOrEmpty(callbackArgument))
				return null;
			string[] parameters = StringArgumentsHelper.SplitMasterSlaveArgumentPair(callbackArgument);
			string commandId = parameters[0];
			string commandArguments = parameters[1];
			if (String.IsNullOrEmpty(commandId))
				return null;
			return new CallbackCommandInfo(commandId, commandArguments);
		}
		protected internal virtual SchedulerCallbackCommandManager CreateCallbackCommandManager() {
			return new SchedulerCallbackCommandManager(this);
		}
		protected internal virtual void ExecuteCallbackCommand(CallbackCommandInfo commandInfo) {
			if (CallbackCommandManager.IsCustomCommand(commandInfo)) {
				EnsureChildControls();
				RaiseCustomCallback(commandInfo.Parameters);
				return;
			}
			SchedulerCallbackCommand command = CallbackCommandManager.GetCallbackCommand(commandInfo);
			SchedulerCallbackCommandEventArgs args = new SchedulerCallbackCommandEventArgs(commandInfo.Id, command);
			RaiseBeforeExecuteCallbackCommand(args);
			command = args.Command;
			if (command != null) {
				this.currentCallbackCommand = command;
				if (command.RequiresControlHierarchy)
					EnsureChildControls();
				command.Execute(commandInfo.Parameters);
				if (AppointmentDataProvider.NeedToRebindAfterInsert && Storage.Appointments.AutoRetrieveId)
					DataBind();
				RaiseAfterExecuteCallbackCommand(args);
			}
		}
		protected internal virtual NameValueCollection GetPageRequestParams() {
			return Request.Params;
		}
		#endregion
		public override void RenderControl(HtmlTextWriter writer) {
			this.isRenderStage = true;
			base.RenderControl(writer);
		}
		protected override void ResetControlHierarchy() {
			base.ResetControlHierarchy();
			this.masterControlDefaultImplementation.RemoveInnerRelatedControls();
		}
		protected internal virtual ISchedulerInplaceEditController CreateInplaceEditController() {
			return new ASPxSchedulerInplaceEditController();
		}
		protected internal virtual AppointmentSelectionController CreateAppointmentSelectionController() {
			return new AppointmentSelectionController(OptionsCustomization.AllowAppointmentMultiSelect);
		}
		protected internal virtual void ProtectedResetControlHierarchy() {
			this.ResetControlHierarchy();
		}
		protected internal virtual string ProtectedGetClientInstanceName() {
			return this.GetClientInstanceName();
		}
		protected override bool CanLoadPostDataOnCreateControls() {
			return false;
		}
		internal NameValueCollection postCollection;
		protected override bool LoadPostData(NameValueCollection postCollection) {
			this.postCollection = postCollection;
			Storage.BeginUpdate();
			try {
				LoadStorageData();
				StateBlock.CreateInitialStateSnapshot();
				stateBlock.LoadPreviousStatePostData();
			} finally {
#if DEBUGTEST
				EnsureFilteredAppointmentsAreNotDisposed();
#endif
				Storage.EndUpdate();
			}
#if DEBUGTEST
			EnsureFilteredAppointmentsAreNotDisposed();
#endif
			bool loadResult = stateBlock.LoadUserActionChangesPostData();
			PropertyChangeTracker.BeginTrack();
			IsPostDataLoaded = true;
			return loadResult;
		}
#if DEBUGTEST
		protected void EnsureFilteredAppointmentsAreNotDisposed() {
			AppointmentBaseCollection appointments = ActiveView.FilteredAppointments;
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				XtraSchedulerDebug.Assert(!appointments[i].IsDisposed);
			}
		}
#endif
		protected override void InitInternal() {
			if (Page != null && !Page.IsPostBack)
				StateBlock.CreateInitialStateSnapshot();
			base.InitInternal();
		}
		protected internal virtual bool FormMode {
			get {
				if (ActiveFormType == SchedulerFormType.Appointment && OptionsForms.AppointmentFormVisibility == SchedulerFormVisibility.FillControlArea)
					return true;
				if (ActiveFormType == SchedulerFormType.GotoDate && OptionsForms.GotoDateFormVisibility == SchedulerFormVisibility.FillControlArea)
					return true;
				return false;
			}
		}
		protected internal virtual Resource LookupResourceByIdString(string resourceIdString) {
			if (String.IsNullOrEmpty(resourceIdString))
				return ResourceBase.Empty;
			ResourceBaseCollection resources = ActiveView.FilteredResources;
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				XtraScheduler.Resource resource = resources[i];
				string resourceIdAsString = resource.Id.ToString();
				if (resourceIdString == resourceIdAsString)
					return resource;
			}
			return ResourceBase.Empty;
		}
		protected override void CreateControlHierarchy() {
			CellIdProvider.Reset();
			CreateMainTable();
			this.viewInfo = CreateViewInfo();
			CreateBlocks();
			CalculateBlocksVisibility();
			base.CreateControlHierarchy();
		}
		protected override bool HasLoadingPanel() {
			return IsCallBacksEnabled();
		}
		protected override bool HasLoadingDiv() {
			return IsCallBacksEnabled();
		}
		protected override bool CanAppendDefaultLoadingPanelCssClass() {
			return false;
		}
		protected internal virtual ISchedulerWebViewInfoBase CreateViewInfo() {
			return ActiveView.FactoryHelper.CreateWebViewInfo(ActiveView);
		}
		protected internal virtual void CreateMainTable() {
			this.mainTable = RenderUtils.CreateTable();
			Controls.Add(mainTable);
			CreateToolbar();
			CreateResourceNavigator();
			CreateSeparatorContainerCell();
			if (!DesignMode) {
				CreateStatusInformationControl();
				CreateStatusInfoManagerControl();
			}
			TableRow row;
			row = RenderUtils.CreateTableRow();
			mainTable.Rows.Add(row);
			this.mainCell = RenderUtils.CreateTableCell();
			row.Cells.Add(mainCell);
		}
		protected void CreateSeparatorContainerCell() {
			bool isSeparatorCellRequired = (RenderUtils.Browser.IsIE) || (RenderUtils.Browser.Family.IsWebKit);
			if (!isSeparatorCellRequired)
				return;
			this.separatorContainerRow = RenderUtils.CreateTableRow();
			this.separatorContainerRow.ID = "separatorContainerRow";
			this.separatorCell = RenderUtils.CreateTableCell();
			this.separatorContainerRow.Cells.Add(this.separatorCell);
			MainTable.Rows.Add(this.separatorContainerRow);
		}
		protected void CreateToolbar() {
			TableRow containerRow = RenderUtils.CreateTableRow();
			TableCell containerCell = RenderUtils.CreateTableCell();
			Table layoutTable = RenderUtils.CreateTable();
			TableRow innerRow = RenderUtils.CreateTableRow();
			this.viewNavigatorCell = RenderUtils.CreateTableCell();
			this.viewVisibleIntervalCell = RenderUtils.CreateTableCell();
			this.viewSelectorCell = RenderUtils.CreateTableCell();
			innerRow.Cells.Add(this.viewNavigatorCell);
			innerRow.Cells.Add(this.viewVisibleIntervalCell);
			innerRow.Cells.Add(this.viewSelectorCell);
			layoutTable.Rows.Add(innerRow);
			containerCell.Controls.Add(layoutTable);
			containerRow.Cells.Add(containerCell);
			MainTable.Rows.Add(containerRow);
			this.toolbarContainerRow = containerRow;
			this.toolbarTable = layoutTable;
			this.toolbarContainerCell = containerCell;
		}
		protected void PrepareToolbar() {
			this.toolbarTable.Width = Unit.Percentage(100);
			AppearanceStyle toolbarStyle = Styles.GetToolbarStyle();
			AppearanceStyle toolbarContainerStyle = Styles.GetToolbarContainerStyle();
			if (this.toolbarContainerCell != null && this.toolbarContainerCell.Visible) {
				toolbarContainerStyle.AssignToControl(this.toolbarContainerCell);
				if (Browser.IsIE || Browser.Family.IsNetscape) {
					this.toolbarContainerCell.Style.Add("border-bottom-width", "0");
					this.toolbarContainerCell.Style.Add("border-collapse", "separate");
				}
			}
			if (this.viewNavigatorCell != null) {
				toolbarStyle.AssignToControl(this.viewNavigatorCell);
				RenderUtils.SetStringAttribute(this.viewNavigatorCell, "align", "left");
			}
			if (this.viewVisibleIntervalCell != null) {
				toolbarStyle.AssignToControl(this.viewVisibleIntervalCell);
				RenderUtils.SetStringAttribute(this.viewVisibleIntervalCell, "align", "center");
				this.viewVisibleIntervalCell.Width = Unit.Percentage(100);
			}
			if (this.viewSelectorCell != null) {
				toolbarStyle.AssignToControl(this.viewSelectorCell);
				this.viewSelectorCell.Style.Add(HtmlTextWriterStyle.Padding, "0");
				RenderUtils.SetStringAttribute(this.viewSelectorCell, "align", "right");
			}
		}
		protected bool IsToolbarVisible() {
			return ViewNavigatorBlock.IsBlockVisible || ViewVisibleIntervalBlock.IsBlockVisible || ViewSelectorBlock.IsBlockVisible;
		}
		protected internal virtual void CreateResourceNavigator() {
			TableRow row = RenderUtils.CreateTableRow();
			row.ID = "resourceNavigatorRow";
			mainTable.Rows.Add(row);
			this.resourceNavigatorCell = RenderUtils.CreateTableCell();
			row.Cells.Add(resourceNavigatorCell);
			this.resourceNavigatorRow = row;
		}
		protected internal virtual void PrepareResourceNavigator() {
			if (!ResourceNavigatorBlock.IsBlockVisible)
				RenderUtils.SetStyleStringAttribute(resourceNavigatorRow, "display", "none");
		}
		protected internal virtual void PrepareSeparatorContainer() {
			if (!SeparatorBlock.IsBlockVisible && separatorContainerRow != null)
				RenderUtils.SetStyleStringAttribute(separatorContainerRow, "display", "none");
		}
		protected internal virtual void CreateStatusInformationControl() {
			TableRow row = RenderUtils.CreateTableRow();
			mainTable.Rows.Add(row);
			this.statusInfoCell = RenderUtils.CreateTableCell();
			row.Cells.Add(statusInfoCell);
			statusInfoCell.ID = "statusInfo";
			statusInfoCell.Controls.Add(new ASPxSchedulerStatusInfo(this));
		}
		protected internal virtual void CreateStatusInfoManagerControl() {
			TableRow row = RenderUtils.CreateTableRow();
			mainTable.Rows.Add(row);
			this.statusInfoManagerCell = RenderUtils.CreateTableCell();
			row.Cells.Add(statusInfoManagerCell);
			statusInfoManagerCell.ID = "statusInfoManagerCell";
		}
		protected internal virtual void PrepareStatusInformationControl() {
			Styles.GetErrorInformationStyle().AssignToControl(this.statusInfoCell, true);
			RenderUtils.SetStyleStringAttribute((WebControl)statusInfoCell.Parent, "display", "none");
		}
		protected internal virtual void PrepareStatusInfoManagerControl() {
			Styles.GetErrorInformationStyle().AssignToControl(this.statusInfoManagerCell, true);
			RenderUtils.SetStyleStringAttribute((WebControl)statusInfoManagerCell.Parent, "display", "none");
		}
		protected override void CreateChildControls() {
			ASPxScheduler oldActiveControl = ASPxScheduler.ActiveControl;
			ASPxScheduler.ActiveControl = this;
			try {
				base.CreateChildControls();
			} finally {
				ASPxScheduler.ActiveControl = oldActiveControl;
			}
		}
		protected override void OnPreRender(EventArgs e) {
			ASPxScheduler.ActiveControl = this;
			try {
				base.OnPreRender(e);
			} finally {
			}
		}
		protected override void PrepareControlHierarchy() {
			ASPxScheduler.ActiveControl = this;
			try {
				PrepareControlHierarchyCore();
			} finally {
			}
			base.PrepareControlHierarchy();
		}
		protected internal virtual void PrepareControlHierarchyCore() {
			RenderUtils.AssignAttributes(this, MainTable);
			RenderUtils.SetVisibility(mainTable, IsClientVisible(), true);
			SchedulerWebEventHelper.AddMouseDownEvent(mainTable, ASPxSchedulerScripts.GetMainDivMouseDownFunction(ClientID));
			SchedulerWebEventHelper.AddMouseUpEvent(mainTable, ASPxSchedulerScripts.GetMainDivMouseUpFunction(ClientID));
			SchedulerWebEventHelper.AddClickEvent(mainTable, ASPxSchedulerScripts.GetMainDivMouseClickFunction(ClientID));
			SchedulerWebEventHelper.AddDoubleClickEvent(mainTable, ASPxSchedulerScripts.GetMainDivMouseDoubleClickFunction(ClientID));
			SchedulerWebUtils.SetPreventSelectionAttribute(mainTable);
			PrepareMainTable();
			bool visible = IsToolbarVisible();
			this.toolbarContainerRow.Visible = visible;
			if (visible)
				PrepareToolbar();
		}
		void PrepareMainTable() {
			RenderUtils.AssignAttributes(this, MainTable);
			StylesHelper.GetControlStyle(this).AssignToControl(MainTable);
			if (this.Width.IsEmpty)
				mainTable.Width = Unit.Percentage(100);
			else
				mainTable.Width = this.Width;
			if (IsSchedulerControlTopBorderWidthCorrectionRequery())
				RenderUtils.SetStyleStringAttribute(MainTable, "border-top-width", "0");
			PrepareResourceNavigator();
			PrepareSeparatorContainer();
			if (!DesignMode) {
				PrepareStatusInformationControl();
				PrepareStatusInfoManagerControl();
			}
		}
		protected internal virtual bool IsSchedulerControlTopBorderWidthCorrectionRequery() {
			return (Browser.IsIE && !IsToolbarVisible());
		}
		#region Client scripts support
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterDragAndDropUtilsScripts();
			RegisterDateFormatterScript();
			RegisterControlResizeManagerScripts();
			RegisterIncludeScript(typeof(ASPxScheduler), SchedulerScriptCommonResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), SchedulerScriptMouseHandlerResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), SchedulerScriptKeyboardHandlerResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), SchedulerScriptResourceName);			
			RegisterIncludeScript(typeof(ASPxScheduler), SchedulerScriptSelectionResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), SchedulerScriptViewInfosResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), SchedulerScriptClientAppointmentResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), SchedulerScriptGlobalFunctionsResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), SchedulerScriptMouseUtilsResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), SchedulerScriptAPIResourceName);
			if (FormBlock != null && FormBlock.IsBlockVisible)
				RegisterIncludeScript(typeof(ASPxScheduler), SchedulerScriptRecurrenceControlsResourceName);
		}
		protected override void RegisterScriptBlocks() {
			base.RegisterScriptBlocks();
			RegisterCultureInfoScript();
		}
		protected override void GetCreateClientObjectScript(StringBuilder sb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(sb, localVarName, clientName);
			AppointmentsBlock.EnsureAppointments();
			sb.AppendFormat("{0}.AddContextMenuEvent(\"{1}\",\"{2}\");\n", localVarName, ClientID, ASPxSchedulerScripts.GetViewContextMenuFunction(ClientID));
			sb.AppendFormat("{0}.isCallbackMode = {1};\n", localVarName, this.EnableCallBacks ? "true" : "false");
			ScriptBlock.RenderPostbackScript(sb, localVarName, clientName);
		}
		protected override Hashtable GetClientObjectState() {
			return StateBlock.GetClientObjectState();
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientScheduler";
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new SchedulerClientSideEvents();
		}
		#endregion
		protected internal virtual ASPxInnerSchedulerControl CreateInnerControl() {
			return new ASPxInnerSchedulerControl(this);
		}
		protected internal virtual ResourceBaseCollection GetFilteredResources() {
			return innerControl.GetFilteredResources();
		}
		protected internal virtual AppointmentBaseCollection GetFilteredAppointments(TimeInterval interval, ResourceBaseCollection resources) {
			bool reloaded;
			return innerControl.GetFilteredAppointments(interval, resources, out reloaded);
		}
		protected internal virtual AppointmentBaseCollection GetNonFilteredAppointments() {
			return innerControl.GetNonFilteredAppointments();
		}
		#region Styles
		protected override Style CreateControlStyle() {
			return new AppearanceStyleBase();
		}
		protected override StylesBase CreateStyles() {
			return new ASPxSchedulerStyles(this);
		}
		protected override ImagesBase CreateImages() {
			return new ASPxSchedulerImages(this);
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ASPxScheduler), SchedulerDefaultCssResourceName);
		}
		protected override void RegisterSystemCssFile() {
			base.RegisterSystemCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ASPxScheduler), SchedulerSystemCssResourceName);
		}
		#endregion
		protected override void RegisterDataHelpers() {
			base.RegisterDataHelpers();
			DataContainer.RegisterDataHelper(CreateDataHelper(ResourcesDataHelperName));
			DataContainer.RegisterDataHelper(CreateDataHelper(AppointmentsDataHelperName));
		}
		internal virtual object GetLastInsertedId() {
			AppointmentsDataHelper dataHelper = DataContainer[AppointmentsDataHelperName] as AppointmentsDataHelper;
			if (dataHelper == null)
				return null;
			return dataHelper.GetLastInsertedId();
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			if (name == AppointmentsDataHelperName) {
				return new AppointmentsDataHelper(this, name);
			}
			return base.CreateDataHelper(name);
		}
		protected class AppointmentsDataHelper : DevExpress.Web.Internal.DataHelper {
			object lastInsertedId;
			public AppointmentsDataHelper(ASPxScheduler control, string name)
				: base(control, name) {
			}
			ASPxScheduler Scheduler { get { return (ASPxScheduler)Control; } }
			protected override DataSourceSelectArguments CreateDataSourceSelectArguments() {
				return base.CreateDataSourceSelectArguments();
			}
			protected override void ConnectToData() {
				base.ConnectToData();
				if (Scheduler.Storage != null && Scheduler.Storage.Appointments != null && !Scheduler.Storage.Appointments.AutoRetrieveId)
					return;
				IDataSource dataSource = GetActualDataSource();
				SqlDataSource sqlDataSource = dataSource as SqlDataSource;
				if (sqlDataSource != null)
					sqlDataSource.Inserted += OnSqlDataSourceInserted;
				ObjectDataSource objectDataSource = dataSource as ObjectDataSource;
				if (objectDataSource != null)
					objectDataSource.Inserted += OnObjectDataSourceInserted;
			}
			IDataSource GetActualDataSource() {
				IDataSource dataSource = GetDataSource();
				if (dataSource is ReadOnlyDataSource)
					return DataSource as IDataSource;
				return dataSource;
			}
			void OnObjectDataSourceInserted(object sender, ObjectDataSourceStatusEventArgs e) {
				this.lastInsertedId = e.ReturnValue;
			}
			void OnSqlDataSourceInserted(object sender, SqlDataSourceStatusEventArgs e) {
				DbConnection connection = e.Command.Connection;
				DbCommand command = connection.CreateCommand();
				command.CommandText = "SELECT @@IDENTITY";
				this.lastInsertedId = command.ExecuteScalar();
			}
			public object GetLastInsertedId() {
				return this.lastInsertedId;
			}
		}
		protected override void DataBindCore() {
			UnsubscribeInnerControlEvents();
			try {
				Storage.BeginUpdate();
				try {
					base.DataBindCore();
				} finally {
					Storage.EndUpdate();
				}
			} finally {
				SubscribeInnerControlEvents();
			}
			stateBlock.ApplyRemindersState();
			stateBlock.ApplyAppointmentSelectionState();
			stateBlock.ApplySelectionState();
			ResetControlHierarchy();
		}
		public override void DataBind() {
			DataBoundLocker.BeginUpdate();
			try {
				base.DataBind();
			} finally {
				DataBoundLocker.EndUpdate();
			}
		}
		protected override void OnDataBound(EventArgs e) {
			if (DataBoundLocker.Locked) {
				DataBoundLocker.SetOnDataBoundRaised();
				return;
			}
			base.OnDataBound(e);
		}
		protected internal virtual void ProtectedRaiseDataBound() {
			RaiseDataBound();
		}
		protected internal virtual void LoadStorageData() {
			UnsubscribeInnerControlEvents();
			try {
				DataBoundLocker.BeginUpdate();
				Storage.BeginUpdate();
				try {
					PerformSelect(ResourcesDataHelperName);
					PerformSelect(AppointmentsDataHelperName);
				} finally {
					Storage.EndUpdate();
					DataBoundLocker.EndUpdate();
				}
				StateBlock.ApplyRemindersState();
				stateBlock.ApplyAppointmentSelectionState();
				stateBlock.ApplySelectionState();
				ResetControlHierarchy();
			} finally {
				SubscribeInnerControlEvents();
			}
		}
		protected override void PerformDataBinding(string dataHelperName, IEnumerable data) {
			if (DesignMode)
				return;
			if (dataHelperName == ResourcesDataHelperName) {
				Storage.Resources.DataSource = null;
				Storage.Resources.DataSource = GetActualDataSource(data, Storage.Resources as ISchedulerMappingFieldChecker);
			} else if (dataHelperName == AppointmentsDataHelperName) {
				Storage.Appointments.DataSource = null;
				Storage.Appointments.DataSource = GetActualDataSource(data, Storage.Appointments as ISchedulerMappingFieldChecker);
			}
		}
		protected internal virtual object GetActualDataSource(IEnumerable data, ISchedulerMappingFieldChecker fieldProvider) {
			if (data != null) {
				FakeDataTableCreator creator = CreateFakeDataTableCreator(fieldProvider);
				return creator.CreateFakeDataTableIfNeeded(data);
			}
			return data;
		}
		protected virtual FakeDataTableCreator CreateFakeDataTableCreator(ISchedulerMappingFieldChecker fieldProvider) {
			return new FakeDataTableCreator(this, fieldProvider);
		}
		#region Templates
		protected internal virtual void SubscribeTemplatesEvents() {
			Templates.Changed += new EventHandler(OnTemplatesChanged);
		}
		protected internal virtual void SubscribeViewsTemplatesEvents() {
			int count = Views.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewBase view = Views[i];
				view.TemplatesChanged += new EventHandler(OnTemplatesChanged);
			}
		}
		protected internal virtual void UnsubscribeTemplatesEvents() {
			Templates.Changed -= new EventHandler(OnTemplatesChanged);
		}
		protected internal virtual void UnsubscribeViewsTemplatesEvents() {
			int count = Views.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewBase view = Views[i];
				view.TemplatesChanged -= new EventHandler(OnTemplatesChanged);
			}
		}
		protected internal virtual void OnTemplatesChanged(object sender, EventArgs e) {
			TemplatesChanged();
		}
		protected internal virtual bool IsTemplatesAssigned() {
			if (IsTemplatesGroupAssigned(Templates))
				return true;
			int count = Views.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewBase view = Views[i];
				if (IsTemplatesGroupAssigned(view.InnerTemplates))
					return true;
			}
			return false;
		}
		protected internal virtual bool IsTemplatesGroupAssigned(SchedulerTemplates templates) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(templates);
			int count = properties.Count;
			for (int i = 0; i < count; i++) {
				PropertyDescriptor property = properties[i];
				if (property.GetValue(templates) != null)
					return true;
			}
			return false;
		}
		protected internal virtual bool ProtectedOnBubbleEvent(object source, EventArgs args) {
			return OnBubbleEvent(source, args);
		}
		protected override bool OnBubbleEvent(object source, EventArgs args) {
			if (args is CommandEventArgs) {
				RaiseBubbleEvent(this, args);
				return true;
			}
			return base.OnBubbleEvent(source, args);
		}
		#endregion
		protected internal virtual void SubscribeOptionsFormsEvents() {
			OptionsForms.Changed += new BaseOptionChangedEventHandler(OnOptionsFormsChanged);
		}
		protected internal virtual void UnsubscribeOptionsFormsEvents() {
			OptionsForms.Changed -= new BaseOptionChangedEventHandler(OnOptionsFormsChanged);
		}
		protected internal virtual void OnOptionsFormsChanged(object sender, BaseOptionChangedEventArgs e) {
			if (e.Name.Contains("TemplateUrl")) {
				TemplatesChanged();
				return;
			}
			ValidateFormVisibility(OptionsForms, ActiveFormType);
			LayoutChanged();
		}
		protected internal virtual void ValidateFormVisibility(ASPxSchedulerOptionsForms optionsForms, SchedulerFormType activeForm) {
			if ((optionsForms.AppointmentFormVisibility == SchedulerFormVisibility.None && activeForm == SchedulerFormType.Appointment) ||
				(optionsForms.GotoDateFormVisibility == SchedulerFormVisibility.None && activeForm == SchedulerFormType.GotoDate)) {
				ActiveFormType = SchedulerFormType.None;
				ApplyChanges(GetToolbarBlocksChangeAction());
			}
		}
		public string GetAppointmentClientId(Appointment appointment) {
			return AppointmentSearchHelper.GetAppointmentClientId(appointment);
		}
		protected internal virtual Appointment LookupVisibleAppointmentByIdString(string appointmentIdString) {
			AppointmentSearchHelper helper = new AppointmentSearchHelper(ActiveView, null);
			return helper.FindAppointmentByClientId(appointmentIdString);
		}
		public virtual Appointment LookupAppointmentByIdString(string appointmentIdString) {
			AppointmentSearchHelper helper = new AppointmentSearchHelper(ActiveView, Storage.Appointments);
			return helper.FindAppointmentByClientId(appointmentIdString);
		}
		protected internal virtual Appointment LookupAppointmentByServerIdString(string appointmentIdString) {
			AppointmentSearchHelper helper = new AppointmentSearchHelper(ActiveView, Storage.Appointments);
			return helper.FindAppointmentByServerId(appointmentIdString);
		}
		protected internal virtual DataSourceView GetAppointmentData() {
			return GetData(AppointmentsDataHelperName);
		}
		#region ISchedulerScriptBuilderOwner Members
		List<IControlBlock> IScriptBlockOwner.GetScriptBuilderOrderedList() {
			return GetScriptBuilderOrderedListCore();
		}
		protected internal ControlBlockCollection GetScriptBuilderOrderedListCore() {
			ControlBlockCollection result = new ControlBlockCollection();
			result.AddRange(activeBlocks);
			result.Remove(scriptBlock);
			return result;
		}
		#endregion
		#region Cookies
		protected override bool IsStateSavedToCookies { get { return !DesignMode && OptionsCookies.Enabled; } }
		protected override string GetStateCookieName() {
			return base.GetStateCookieName(OptionsCookies.CookiesID);
		}
		protected override string SaveClientState() {
			return new ASPxSchedulerCookies(this).SaveState();
		}
		protected override void LoadClientState(string state) {
			ASPxSchedulerCookies cookies = new ASPxSchedulerCookies(this);
			cookies.LoadState(state);
		}
		protected override bool NeedLoadClientState() {
			return String.IsNullOrEmpty(Request.Form[GetClientObjectStateInputID()]);
		}
		internal string GetClientObjectStateValueStringInternal(string key) {
			return GetClientObjectStateValueString(key);
		}
		internal T GetClientObjectStateValueInternal<T>(string key) {
			return GetClientObjectStateValue<T>(key);
		}
		#endregion
		#region IMasterControl implementation
		void IMasterControl.RegisterRelatedControl(IRelatedControl control) {
			masterControlDefaultImplementation.RegisterRelatedControl(control);
		}
		string IMasterControl.CalcRelatedControlsCallbackResult() {
			UnsubscribeInnerControlEvents();
			try {
				return masterControlDefaultImplementation.CalcRelatedControlsCallbackResult();
			} finally {
				SubscribeInnerControlEvents();
			}
		}
		#endregion
		public virtual void ApplyChanges(ASPxSchedulerChangeAction actions) {
			this.userChangeActions |= actions;
			this.controlChangeActions |= actions;
		}
		#region IXtraSupportShouldSerialize Members
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			return shouldSerializeHelper.ShouldSerialize(propertyName);
		}
		#endregion
		#region IXtraSupportDeserializeCollectionItem Members
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			switch (propertyName) {
				case "ResourceColorSchemas":
					return XtraCreateResourceColorSchemasItem(e);
				default:
					return null;
			}
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			switch (propertyName) {
				case "ResourceColorSchemas":
					XtraSetIndexResourceColorSchemasItem(e);
					break;
				default:
					break;
			}
		}
		#endregion
		public virtual void GoToToday() {
			IDateTimeNavigationService service = (IDateTimeNavigationService)GetService(typeof(IDateTimeNavigationService));
			if (service != null)
				service.GoToToday();
		}
		public virtual void GoToDate(DateTime date) {
			IDateTimeNavigationService service = (IDateTimeNavigationService)GetService(typeof(IDateTimeNavigationService));
			if (service != null)
				service.GoToDate(date);
		}
		public virtual void GoToDate(DateTime date, SchedulerViewType viewType) {
			IDateTimeNavigationService service = (IDateTimeNavigationService)GetService(typeof(IDateTimeNavigationService));
			if (service != null)
				service.GoToDate(date, viewType);
		}
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			if (innerControl != null)
				innerControl.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			if (innerControl != null)
				innerControl.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			if (innerControl != null)
				innerControl.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			if (innerControl != null)
				innerControl.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			if (innerControl != null)
				innerControl.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			if (innerControl != null)
				innerControl.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		public virtual object GetService(Type serviceType) {
			if (innerControl != null)
				return innerControl.GetService(serviceType);
			else
				return null;
		}
		#endregion
		protected internal virtual void AddServices() {
			AddService(typeof(DevExpress.XtraScheduler.Services.ISelectionService), new SelectionService(InnerControl));
			AddService(typeof(IHeaderCaptionService), new HeaderCaptionService());
			AddService(typeof(IHeaderToolTipService), new HeaderToolTipService());
			AddService(typeof(ISupportCollectionDataSourceService), new DefaultSupportXpCollectionService());
		}
		#region IInnerSchedulerCommandTarget Members
		InnerSchedulerControl IInnerSchedulerCommandTarget.InnerSchedulerControl { get { return InnerControl; } }
		#endregion
		internal bool ShouldRenderAppointmentEarly() {
			if (IsRenderStage)
				return true;
			SchedulerTemplates templates = ActiveView.InnerTemplates;
			if (templates.HorizontalAppointmentTemplate != null || templates.HorizontalSameDayAppointmentTemplate != null)
				return true;
			DayViewTemplates dayViewTemplate = ActiveView.InnerTemplates as DayViewTemplates;
			if (dayViewTemplate != null && dayViewTemplate.VerticalAppointmentTemplate != null)
				return true;
			return false;
		}
		protected internal virtual IAppointmentStatus GetStatus(object statusId) {
			if (Storage != null)
				return Storage.Appointments.Statuses.GetById(statusId);
			else
				return AppointmentStatus.Empty;
		}
		protected internal virtual Color GetLabelColor(object labelId) {
			if (Storage != null)
				return Storage.Appointments.Labels.GetById(labelId).GetColor();
			else
				return Color.White;
		}
		#region ICommandAwareControl
		void ICommandAwareControl<SchedulerCommandId>.CommitImeContent() {
		}
		Command ICommandAwareControl<SchedulerCommandId>.CreateCommand(SchedulerCommandId id) {
			if (InnerControl != null)
				return InnerControl.CreateCommand(id);
			else
				return null;
		}
		void ICommandAwareControl<SchedulerCommandId>.Focus() {
		}
		bool ICommandAwareControl<SchedulerCommandId>.HandleException(Exception e) {
			return false;
		}
		Utils.KeyboardHandler.CommandBasedKeyboardHandler<SchedulerCommandId> ICommandAwareControl<SchedulerCommandId>.KeyboardHandler {
			get { return null; }
		}
		event EventHandler ICommandAwareControl<SchedulerCommandId>.UpdateUI {
			add { ; }
			remove { ; }
		}
		#endregion
		protected override bool IsSwipeGesturesEnabled() {
			return base.IsSwipeGesturesEnabled();
		}
	}
	#endregion
	#region ASPxSchedulerChangeAction
	[Flags]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217")]
	public enum ASPxSchedulerChangeAction {
		None = 0x00000000,
		NotifyActiveViewChanged = 0x00000001, 
		NotifyVisibleIntervalsChanged = 0x00000002,  
		NotifyResourceIntervalChanged = 0x00000004, 
		RenderView = 0x00000008, 
		RenderAppointments = 0x00000010, 
		RenderNavigationButtons = 0x00000020, 
		RenderViewMenu = 0x00000040, 
		RenderAppointmentMenu = 0x00000080, 
		RenderActiveForm = 0x00000100, 
		RenderState = 0x00000200, 
		DataBindResources = 0x00000400,
		DataBindAppointments = 0x00000800,
		NotifyTimeZoneChanged = 0x00001000,
		RenderCommonControls = 0x00002000,
		StringFormatsChanged = 0x00004000,
		Any = 0x40000000,
		All = 0x7FFFFFFF
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region AppointmentDataProvider
	public class AppointmentDataProvider : IWebSchedulerDataProvider {
		ASPxScheduler control;
		public AppointmentDataProvider(ASPxScheduler control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
		}
		public ASPxScheduler Control { get { return control; } }
		#region IWebSchedulerDataProvider Members
		public bool NeedToRebindAfterInsert { get; set; }
		public DataSourceView GetData() {
			return control.GetAppointmentData();
		}
		public object GetLastInsertedId() {
			return control.GetLastInsertedId();
		}
		public void RaiseRowInserting(ASPxSchedulerDataInsertingEventArgs e) {
			control.RaiseAppointmentRowInserting(e);
		}
		public void RaiseRowInserted(ASPxSchedulerDataInsertedEventArgs e) {
			control.RaiseAppointmentRowInserted(e);
		}
		public void RaiseRowUpdating(ASPxSchedulerDataUpdatingEventArgs e) {
			control.RaiseAppointmentRowUpdating(e);
		}
		public void RaiseRowUpdated(ASPxSchedulerDataUpdatedEventArgs e) {
			control.RaiseAppointmentRowUpdated(e);
		}
		public void RaiseRowDeleting(ASPxSchedulerDataDeletingEventArgs e) {
			control.RaiseAppointmentRowDeleting(e);
		}
		public void RaiseRowDeleted(ASPxSchedulerDataDeletedEventArgs e) {
			control.RaiseAppointmentRowDeleted(e);
		}
		#endregion
	}
	#endregion
	public class SchedulerControlMasterControlDefaultImplementation : MasterControlDefaultImplementation {
		ASPxScheduler schedulerControl;
		public SchedulerControlMasterControlDefaultImplementation(ASPxScheduler control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.schedulerControl = control;
		}
		public ASPxScheduler Control { get { return schedulerControl; } }
		protected override bool ShouldCalcCallbackResult(ISupportsCallbackResult control) {
			ISchedulerRelatedControl relatedControl = control as ISchedulerRelatedControl;
			if (relatedControl != null)
				return ((schedulerControl.ControlChangeActions & relatedControl.RenderActions) != 0);
			else
				return base.ShouldCalcCallbackResult(control);
		}
	}
	#region ASPxInnerSchedulerControl
	public class ASPxInnerSchedulerControl : InnerSchedulerControl {
		public ASPxInnerSchedulerControl(ASPxScheduler owner)
			: base(owner) {
		}
		[Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SchedulerColorSchemaCollection ResourceColorSchemas { get { return (SchedulerColorSchemaCollection)base.ResourceColorSchemas; } }
		protected internal override void OnStorageDeferredNotificationsCore() {
			List<SchedulerControlChangeType> changes = new List<SchedulerControlChangeType>();
			AnalyzeStorageDeferredChanges(((IInternalSchedulerStorageBase)Storage).DeferredChanges, changes);
			ChangeActions actions = ChangeActionsCalculator.CalculateChangeActions(SchedulerControlChangeType.StorageDeferredNotifications);
			ApplyChangesCore(changes, actions);
		}
		protected internal override void OnAppointmentMappingsChanged(object sender, EventArgs e) {
			ApplyChanges(SchedulerControlChangeType.StorageAppointmentMappingsChanged);
		}
		protected internal override void OnResourceMappingsChanged(object sender, EventArgs e) {
			ApplyChanges(SchedulerControlChangeType.StorageResourceMappingsChanged);
		}
		protected internal override void OnAppointmentUIObjectsChanged(object sender, EventArgs e) {
			ApplyChanges(SchedulerControlChangeType.StorageUIObjectsChanged);
		}
		protected internal virtual void AnalyzeStorageDeferredChanges(SchedulerStorageDeferredChanges deferredChanges, List<SchedulerControlChangeType> changes) {
			bool resourcesWereChanged = AreResourcesChanged(deferredChanges);
			bool appointmentsWereChanged = resourcesWereChanged || AreAppointmentsChanged(deferredChanges);
			if (resourcesWereChanged)
				changes.Add(SchedulerControlChangeType.StorageResourcesChanged);
			if (appointmentsWereChanged)
				changes.Add(SchedulerControlChangeType.StorageAppointmentsChanged);
			if (deferredChanges.AppointmentMappingsChanged)
				changes.Add(SchedulerControlChangeType.StorageAppointmentMappingsChanged);
			if (deferredChanges.ResourceMappingsChanged)
				changes.Add(SchedulerControlChangeType.StorageResourceMappingsChanged);
			if (deferredChanges.AppointmentUIObjectsChanged)
				changes.Add(SchedulerControlChangeType.StorageUIObjectsChanged);
		}
		protected internal virtual bool AreResourcesChanged(SchedulerStorageDeferredChanges deferredChanges) {
			if (deferredChanges.ClearResources)
				return true;
			if (deferredChanges.LoadResources)
				return true;
			if (deferredChanges.RaiseResourcesLoaded)
				return true;
			if (deferredChanges.InsertedResources.Count > 0)
				return true;
			if (deferredChanges.ChangedResources.Count > 0)
				return true;
			if (deferredChanges.DeletedResources.Count > 0)
				return true;
			if (deferredChanges.ResourceVisibilityChanged)
				return true;
			return false;
		}
		protected internal virtual bool AreAppointmentsChanged(SchedulerStorageDeferredChanges deferredChanges) {
			if (deferredChanges.ClearAppointments)
				return true;
			if (deferredChanges.LoadAppointments)
				return true;
			if (deferredChanges.RaiseAppointmentsLoaded)
				return true;
			if (deferredChanges.InsertedAppointments.Count > 0)
				return true;
			if (deferredChanges.ChangedAppointments.Count > 0)
				return true;
			if (deferredChanges.DeletedAppointments.Count > 0)
				return true;
			return false;
		}
		protected internal override void InitializeKeyboardHandlerDefaults(NormalKeyboardHandlerBase keyboardHandler) {
		}
		protected internal override object XtraCreateResourceColorSchemasItem(XtraItemEventArgs e) {
			return new SchedulerColorSchema();
		}
		protected override ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> CreateResourceColorSchemaCollection() {
			return new SchedulerColorSchemaCollection();
		}
		protected override ICollectionChangedListener CreateResourceColorSchemaChangedListener() {
			return new ResourceColorSchemasChangedListener(ResourceColorSchemas);
		}
#pragma warning disable 618
		protected internal override void UpdateTimeMarkerVisibilityFromOptionBehavior() {
			base.UpdateTimeMarkerVisibilityFromOptionBehavior();
			ASPxScheduler control = Owner as ASPxScheduler;
			if (control == null)
				return;
			TimeIndicatorVisibility timeIndicatorVisibility = ConvertToTimeIndicatorVisibility(OptionsBehavior.ShowCurrentTime);
			UpdateTimeIndicatorVisibility(control.DayView, timeIndicatorVisibility);
			UpdateTimeIndicatorVisibility(control.WorkWeekView, timeIndicatorVisibility);
			UpdateTimeIndicatorVisibility(control.FullWeekView, timeIndicatorVisibility);
			UpdateTimeIndicatorVisibility(control.TimelineView, timeIndicatorVisibility);
		}
		TimeIndicatorVisibility ConvertToTimeIndicatorVisibility(CurrentTimeVisibility visibility) {
			switch (visibility) {
				case CurrentTimeVisibility.Always:
					return TimeIndicatorVisibility.Always;
				case CurrentTimeVisibility.Never:
					return TimeIndicatorVisibility.Never;
				default:
					return TimeIndicatorVisibility.TodayView;
			}
		}
		void UpdateTimeIndicatorVisibility(SchedulerViewBase view, TimeIndicatorVisibility timeIndicatorVisibility) {
			if (view == null)
				return;
			DayView dayView = view as DayView;
			if (dayView != null && dayView.TimeIndicatorDisplayOptions.Visibility != timeIndicatorVisibility) {
				dayView.TimeIndicatorDisplayOptions.Visibility = timeIndicatorVisibility;
				return;
			}
			TimelineView timelineView = view as TimelineView;
			if (timelineView != null && timelineView.TimeIndicatorDisplayOptions.Visibility != timeIndicatorVisibility)
				timelineView.TimeIndicatorDisplayOptions.Visibility = timeIndicatorVisibility;
		}
#pragma warning restore 618
	}
	#endregion
	public class ResourceColorSchemasChangedListener : NotificationCollectionChangedListener<SchedulerColorSchema>, ICollectionChangedListener {
		public ResourceColorSchemasChangedListener(NotificationCollection<SchedulerColorSchema> collection)
			: base(collection) {
		}
	}
	#region ASPxNormalKeyboardHandler
	class ASPxNormalKeyboardHandler : NormalKeyboardHandlerBase {
		protected internal ASPxScheduler Control { get { return ((SchedulerViewBase)InnerView.Owner).Control; } }
		protected internal override ISchedulerCommandTarget ISchedulerCommandTarget { get { return Control; } }
		protected internal override InnerSchedulerControl InnerControl { get { return Control.InnerControl; } }		
	}
	#endregion
	#region EmptySelectableIntervalViewInfo
	class EmptySelectableIntervalViewInfo : ISelectableIntervalViewInfo {
		#region ISelectableIntervalViewInfo Members
		SchedulerHitTest ISelectableIntervalViewInfo.HitTestType { get { return SchedulerHitTest.None; } }
		TimeInterval ISelectableIntervalViewInfo.Interval { get { return TimeInterval.Empty; } }
		Resource ISelectableIntervalViewInfo.Resource { get { return ResourceBase.Empty; } }
		bool ISelectableIntervalViewInfo.Selected { get { return false; } }
		#endregion
	}
	#endregion
	#region ASPxSchedulerMouseHandler
	class ASPxSchedulerMouseHandler : SchedulerMouseHandler {
		readonly EmptySelectableIntervalViewInfo selectableIntervalInfo = new EmptySelectableIntervalViewInfo();
		readonly ASPxSchedulerMenuBuilderUIFactory uiFactory;
		public ASPxSchedulerMouseHandler(InnerSchedulerControl control)
			: base(control) {
			this.uiFactory = new ASPxSchedulerMenuBuilderUIFactory((ASPxScheduler)control.Owner);
		}
		protected internal override ISelectableIntervalViewInfo EmptySelectableIntervalViewInfo { get { return selectableIntervalInfo; } }
		protected internal override IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> UiFactory { get { return uiFactory; } }
		protected internal override MouseHandlerState CreateDefaultState() {
			return new ASPxSchedulerMouseHandlerState(this);
		}
		protected internal override MouseHandlerState CreateAppointmentDragState(Appointment appointment, ISchedulerHitInfo layoutHitInfo, ISchedulerHitInfo prevLayoutHitInfo) {
			return new ASPxSchedulerMouseHandlerState(this);
		}
		public override SchedulerPopupMenuBuilder CreateDefaultPopupMenuBuilder(ISchedulerHitInfo hitInfo) {
			return new ASPxSchedulerDefaultPopupMenuBuilder(UiFactory, (ASPxScheduler)Control.Owner, hitInfo.HitTest);
		}
		protected internal override EditAppointmentCommand CreateEditAppointmentCommand() {
			return new WebEditAppointmentCommand((ASPxScheduler)Control.Owner);
		}
		public override bool OnPopupMenu(Point pt) {
			return true;
		}
		protected internal override void Redraw() {
		}
		protected internal override void ScrollBackward(System.Windows.Forms.MouseEventArgs e) {
		}
		protected internal override void ScrollForward(System.Windows.Forms.MouseEventArgs e) {
		}
		protected internal override void SetCurrentCursor(System.Windows.Forms.Cursor cursor) {
		}
		protected internal override void UpdateActiveViewSelection(SchedulerViewSelection selection) {
		}
		protected internal override void UpdateHotTrack(ISelectableIntervalViewInfo viewInfo) {
		}
		protected override void CalculateAndSaveHitInfo(System.Windows.Forms.MouseEventArgs e) {
		}
		protected override IOfficeScroller CreateOfficeScroller() {
			return null;
		}
		protected override void StartOfficeScroller(Point clientPoint) {
		}
	}
	#endregion
	#region ASPxSchedulerMouseHandlerState
	class ASPxSchedulerMouseHandlerState : SchedulerMouseHandlerState {
		public ASPxSchedulerMouseHandlerState(SchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
	}
	#endregion
	#region ASPxSchedulerInplaceEditController
	class ASPxSchedulerInplaceEditController : ISchedulerInplaceEditController {
		Appointment appointment;
		#region ISchedulerInplaceEditController Members
		public void Activate() {
		}
		event EventHandler ISchedulerInplaceEditController.CommitChanges { add { } remove { } }
		event EventHandler ISchedulerInplaceEditController.RollbackChanges { add { } remove { } }
		public Appointment EditedAppointment { get { return appointment; } }
		public void ResetEditedAppointment() {
			this.appointment = null;
		}
		public void SetEditedAppointment(Appointment appointment) {
			this.appointment = appointment;
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
	}
	#endregion
	#region FakeDataTableCreator
	public class FakeDataTableCreator {
		ASPxScheduler scheduler;
		ISchedulerMappingFieldChecker fieldProvider;
		public FakeDataTableCreator(ASPxScheduler scheduler, ISchedulerMappingFieldChecker fieldProvider) {
			Guard.ArgumentNotNull(scheduler, "scheduler");
			this.scheduler = scheduler;
			this.fieldProvider = fieldProvider;
		}
		public virtual object CreateFakeDataTableIfNeeded(IEnumerable data) {
			if (data == null)
				return null;
			ListSourceDataController controller = CreateDataControllerForWrappedDataSource(data);
			if (controller != null)
				return PrepareFakeDataTable(controller);
			else
				return data;
		}
		protected internal virtual ListSourceDataController CreateDataControllerForWrappedDataSource(IEnumerable data) {
			ISupportCollectionDataSourceService xpCollectionService = this.scheduler.GetService(typeof(ISupportCollectionDataSourceService)) as ISupportCollectionDataSourceService;
			if (xpCollectionService == null)
				return null;
			ListSourceDataController controller = new ListSourceDataController();
			controller.SetListSource(null, data, String.Empty);
			if (xpCollectionService.IsSupported(data))
				return controller;
			else {
				if (!controller.AllowNew || !controller.AllowRemove || !controller.AllowEdit)
					return controller;
				else
					return null;
			}
		}
		protected internal virtual DataTable PrepareFakeDataTable(ListSourceDataController controller) {
			DataTable table = CreateFakeDataTable(controller);
			PopulateFakeDataTable(controller, table);
			return table;
		}
		protected internal virtual DataTable CreateFakeDataTable(ListSourceDataController controller) {
			DataTable table = new DataTable();
			table.Locale = CultureInfo.InvariantCulture;
			table.BeginInit();
			try {
				DataColumnCollection columns = table.Columns;
				DataColumnInfoCollection controllerColumns = controller.Columns;
				int count = controllerColumns.Count;
				bool needToCheckFields = this.fieldProvider != null && this.fieldProvider.HasFields();
				for (int i = 0; i < count; i++) {
					DataColumnInfo column = controllerColumns[i];
					if (needToCheckFields && !this.fieldProvider.Contains(column.Name))
						continue;
					Type columnType = GetColumnType(column);
					columns.Add(column.Name, columnType);
				}
			} finally {
				table.EndInit();
			}
			return table;
		}
		protected internal virtual Type GetColumnType(DataColumnInfo column) {
			Type columnType = column.Type;
			if (!columnType.IsGenericType)
				return columnType;
			else
				if (columnType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
					Type[] genericArgs = columnType.GetGenericArguments();
					XtraSchedulerDebug.Assert(genericArgs.Length == 1);
					return genericArgs[0];
				}
			return columnType;
		}
		protected internal virtual void PopulateFakeDataTable(ListSourceDataController controller, DataTable table) {
			int count = controller.ListSourceRowCount;
			DataColumnCollection columns = table.Columns;
			for (int i = 0; i < count; i++) {
				DataRow row = table.NewRow();
				PopulateFakeDataRow(controller, i, row, columns);
				table.Rows.Add(row);
			}
		}
		protected internal virtual void PopulateFakeDataRow(ListSourceDataController controller, int rowIndex, DataRow row, DataColumnCollection columns) {
			row.BeginEdit();
			try {
				int count = columns.Count;
				for (int i = 0; i < count; i++) {
					DataColumn column = columns[i];
					string columnName = column.ColumnName;
					object value = controller.GetRowValue(rowIndex, columnName);
					if (value == null)
						value = DBNull.Value;
					row[columnName] = value;
				}
			} finally {
				row.EndEdit();
			}
		}
	}
	#endregion
}
