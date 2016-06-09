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
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Scheduler.UI;
using DevExpress.XtraScheduler;
using DevExpress.Utils.Commands;
using System.Windows.Controls;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Scheduler.Menu;
using DevExpress.Xpf.Scheduler.Native;
#if !SL
using System.Collections.Generic;
#else
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.Scheduler {
	delegate void DragLeaveDelegate(System.Windows.DragEventArgs e);
	public delegate void SchedulerMenuEventHandler(object sender, SchedulerMenuEventArgs e);
	public class SchedulerMenuEventArgs : RoutedEventArgs {
		readonly SchedulerPopupMenu innerMenu;
		public SchedulerMenuEventArgs(SchedulerPopupMenu menu) {
			Guard.ArgumentNotNull(menu, "menu");
			this.innerMenu = menu;
		}
		public PopupMenu Menu { get { return innerMenu; } }
		public BarManagerActionCollection Customizations { get { return this.innerMenu.Customizations; } }
	}
	#region RemindersFormEventHandler
	public delegate void RemindersFormEventHandler(object sender, RemindersFormEventArgs e);
	#endregion
	#region RemindersFormEventArgs
	public class RemindersFormEventArgs : FormShowingEventArgs {
		ReminderAlertNotificationCollection alerts;
		public RemindersFormEventArgs(ReminderAlertNotificationCollection alerts) {
			Guard.ArgumentNotNull(alerts, "alerts");
			this.alerts = alerts;
			AllowResize = false;
		}
		public new RemindersFormBase Form { get { return (RemindersFormBase)base.Form; } set { base.Form = value; } }
		public ReminderAlertNotificationCollection AlertNotifications { get { return alerts; } }
	}
	#endregion
	#region InplaceEditorEventHandler
	public delegate void InplaceEditorEventHandler(object sender, InplaceEditorEventArgs e);
	#endregion
	#region InplaceEditorEventArgs
	public class InplaceEditorEventArgs : AppointmentEventArgs {
		Rect bounds;
		ISchedulerInplaceEditorEx editor;
		public InplaceEditorEventArgs(Appointment apt)
			: base(apt) {
		}
		public Rect Bounds { get { return bounds; } set { bounds = value; } }
		public ISchedulerInplaceEditorEx InplaceEditor { get { return editor; } set { editor = value; } }
	}
	#endregion
	#region EditRecurrentAppointmentFormEventHandler
	public delegate void EditRecurrentAppointmentFormEventHandler(object sender, EditAppointmentFormEventArgs e);
	#endregion
	#region EditRecurrentAppointmentFormEventArgs
	public class EditRecurrentAppointmentFormEventArgs : EditAppointmentFormEventArgs {
		public EditRecurrentAppointmentFormEventArgs(Appointment apt, bool readOnly)
			: base(apt, readOnly) {
			AllowResize = false;
		}
		public EditRecurrentAppointmentFormEventArgs(Appointment apt, bool readOnly, CommandSourceType commandSourceType)
			: base(apt, readOnly, commandSourceType) {
		}
	}
	#endregion
	#region CustomizeTimeRulerFormEventHandler
	public delegate void CustomizeTimeRulerFormEventHandler(object sender, CustomizeTimeRulerFormEventArgs e);
	#endregion
	#region CustomizeTimeRulerFormEventArgs
	public class CustomizeTimeRulerFormEventArgs : FormShowingEventArgs {
		TimeRuler timeRuler;
		public CustomizeTimeRulerFormEventArgs(TimeRuler timeRuler)
			: base() {
			this.timeRuler = timeRuler;
		}
		public TimeRuler TimeRuler { get { return timeRuler; } }
	}
	#endregion
	#region AppointmentFormEventHandler
	public delegate void AppointmentFormEventHandler(object sender, EditAppointmentFormEventArgs e);
	#endregion
	public abstract class EditFormShowingEventArgs : FormShowingEventArgs {
		#region Fields
		bool readOnly;
		CommandSourceType commandSourceType;
		#endregion
		protected EditFormShowingEventArgs(bool readOnly)
			: this(readOnly, CommandSourceType.Unknown) {
		}
		protected internal EditFormShowingEventArgs(bool readOnly, CommandSourceType commandSourceType) {
			this.readOnly = readOnly;
			this.commandSourceType = commandSourceType;
		}
		#region Properties
		public bool ReadOnly { get { return readOnly; } }
		public CommandSourceType CommandSourceType { get { return commandSourceType; } }
		#endregion
	}
	#region EditAppointmentFormEventArgs
	public class EditAppointmentFormEventArgs : EditFormShowingEventArgs {
		#region Fields
		Appointment apt;
		#endregion
		public EditAppointmentFormEventArgs(Appointment apt, bool readOnly)
			: this(apt, readOnly, false, CommandSourceType.Unknown) {
		}
		public EditAppointmentFormEventArgs(Appointment apt, bool readOnly, bool openRecurrenceDialog)
			: this(apt, readOnly, openRecurrenceDialog, CommandSourceType.Unknown) {
		}
		public EditAppointmentFormEventArgs(Appointment apt, bool readOnly, CommandSourceType commandSourceType)
			: this(apt, readOnly, false, commandSourceType) {
		}
		protected internal EditAppointmentFormEventArgs(Appointment apt, bool readOnly, bool openRecurrenceDialog, CommandSourceType commandSourceType)
			: base(readOnly, commandSourceType) {
			Guard.ArgumentNotNull(apt, "apt");
			this.apt = apt;
			this.OpenRecurrenceDialog = openRecurrenceDialog;
		}
		#region Properties
		public Appointment Appointment { get { return apt; } }
		public bool OpenRecurrenceDialog { get; private set; }
		#endregion
	}
	#endregion
	#region RecurrenceFormEventHandler
	public delegate void RecurrenceFormEventHandler(object sender, RecurrenceFormEventArgs e);
	#endregion
	#region RecurrenceFormEventArgs
	public class RecurrenceFormEventArgs : EditFormShowingEventArgs {
		UserControl parentForm;
		AppointmentFormController controller;
		public AppointmentFormViewModel AppointmentFormViewModel { get; protected set; }
		public RecurrenceFormEventArgs(UserControl parentForm, bool readOnly)
			: base(readOnly) {
			this.parentForm = parentForm;
		}
		public RecurrenceFormEventArgs(AppointmentFormViewModel parentViewModel, bool readOnly)
			: base(readOnly) {
			this.AppointmentFormViewModel = parentViewModel;
		}
		public UserControl ParentForm { get { return parentForm; } }
		public AppointmentFormController Controller {
			get { return controller; }
			set {
				if (controller == value)
					return;
				controller = value;
			}
		}
	}
	#endregion
	#region GotoDateFormEventHandler
	public delegate void GotoDateFormEventHandler(object sender, GotoDateFormEventArgs e);
	#endregion
	#region GotoDateFormEventArgs
	public class GotoDateFormEventArgs : FormShowingEventArgs {
		#region Fields
		DateTime date;
		SchedulerViewType viewType;
		SchedulerViewRepository views;
		#endregion
		public GotoDateFormEventArgs(SchedulerViewRepository views, DateTime date, SchedulerViewType viewType) {
			this.views = views;
			this.date = date;
			this.viewType = viewType;
			AllowResize = false;
		}
		#region Properties
		public DateTime Date { get { return date; } }
		public SchedulerViewType SchedulerViewType { get { return viewType; } }
		public SchedulerViewRepository Views { get { return views; } }
		#endregion
	}
	#endregion
	#region DeleteRecurrentAppointmentFormEventHandler
	public delegate void DeleteRecurrentAppointmentFormEventHandler(object sender, DeleteRecurrentAppointmentFormEventArgs e);
	#endregion
	#region DeleteRecurrentAppointmentFormEventArgs
	public class DeleteRecurrentAppointmentFormEventArgs : FormShowingEventArgs {
		#region Fields
		AppointmentBaseCollection appointments;
		#endregion
		public DeleteRecurrentAppointmentFormEventArgs(AppointmentBaseCollection appointments) {
			this.appointments = appointments;
			AllowResize = false;
		}
		#region Properties
		public AppointmentBaseCollection Appointments { get { return appointments; } }
		#endregion
	}
	#endregion
	#region ShowFormEventArgs
	public abstract class FormShowingEventArgs : EventArgs {
		bool cancel = false;
		UserControl form;
		bool allowResize = true;
		#region Properties
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		public UserControl Form { get { return form; } set { form = value; } }
		public bool AllowResize { get { return allowResize; } set { allowResize = value; } }
		public object ViewModel { get; set; }
#if !SL
		[Obsolete(ObsoleteText.SRFormShowingEventArgsSizeToContent)]
		public SizeToContent SizeToContent { get { return SizeToContent.WidthAndHeight; } set { } }
#endif
		#endregion
	}
	#endregion
	#region AppointmentViewInfoCustomizingEventHandler
	public delegate void AppointmentViewInfoCustomizingEventHandler(object sender, AppointmentViewInfoCustomizingEventArgs e);
	#endregion
	#region AppointmentViewInfoCustomizingEventArgs
	public class AppointmentViewInfoCustomizingEventArgs : EventArgs {
		AppointmentViewInfo viewInfo;
		public AppointmentViewInfoCustomizingEventArgs(AppointmentViewInfo viewInfo) {
			if (viewInfo == null)
				Exceptions.ThrowArgumentException("viewInfo", viewInfo);
			this.viewInfo = viewInfo;
		}
		public AppointmentViewInfo ViewInfo { get { return viewInfo; } }
	}
	#endregion
	#region ActiveViewChangingEventHandler
	public delegate void ActiveViewChangingEventHandler(object sender, ActiveViewChangingEventArgs e);
	#endregion
	#region ActiveViewChangingEventArgs
	public class ActiveViewChangingEventArgs : EventArgs {
		#region Fields
		bool cancel;
		SchedulerViewBase oldView;
		SchedulerViewBase newView;
		#endregion
		public ActiveViewChangingEventArgs(SchedulerViewBase oldView, SchedulerViewBase newView) {
			if (oldView == null)
				Exceptions.ThrowArgumentException("oldView", oldView);
			if (newView == null)
				Exceptions.ThrowArgumentException("newView", newView);
			this.oldView = oldView;
			this.newView = newView;
		}
		#region Properties
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		public SchedulerViewBase OldView { get { return oldView; } }
		public SchedulerViewBase NewView { get { return newView; } }
		#endregion
	}
	#endregion
	public delegate void CustomizeVisualViewInfoEventHandler(object sender, CustomizeVisualViewInfoEventArgs e);
	public class CustomizeVisualViewInfoEventArgs : EventArgs {
		VisualViewInfoBase visualViewInfo;
		public CustomizeVisualViewInfoEventArgs(VisualViewInfoBase visualViewInfo) {
			Guard.ArgumentNotNull(visualViewInfo, "visualViewInfo");
			this.visualViewInfo = visualViewInfo;
		}
		public VisualViewInfoBase VisualViewInfo { get { return visualViewInfo; } }
	}
#if !SL
	public class CustomizeSpecialDatesEventArgs : EventArgs {
		public CustomizeSpecialDatesEventArgs(IList<DateTime> specialDates) {
			SpecialDates = specialDates;
		}
		public IList<DateTime> SpecialDates { get; set; }
	}
#endif
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class LastSelectedCellLayoutChangedEventArgs : EventArgs {
		int cellIndex;
		SelectionPresenter selectionPresenter;
		public LastSelectedCellLayoutChangedEventArgs(int cellIndex, SelectionPresenter source) {
			this.selectionPresenter = source;
			this.cellIndex = cellIndex;
		}
		protected virtual int CellIndex { get { return cellIndex; } }
		protected SelectionPresenter SelectionPresenter { get { return selectionPresenter; } }
		public virtual Rect GetCellBounds(UIElement relativeTo) {
			if (SelectionPresenter == null)
				return Rect.Empty;
			return SelectionPresenter.GetCellBounds(relativeTo, cellIndex);
		}
	}
	public delegate void LastSelectedCellLayoutChangedEventHandler(object sender, LastSelectedCellLayoutChangedEventArgs e);
}
namespace DevExpress.Xpf.Scheduler.Internal {
	public class LayoutChangedEventArgs : EventArgs {
		AppointmentsPanelChangeActions changeActions;
		public LayoutChangedEventArgs(AppointmentsPanelChangeActions changeActions) {
			this.changeActions = changeActions;
		}
		public AppointmentsPanelChangeActions ChangeActions { get { return changeActions; } }
	}
	public class CopyFromCompletedEventArgs : EventArgs {
		public CopyFromCompletedEventArgs(bool isStateChanged) {
			IsStateChanged = isStateChanged;
		}
		public bool IsStateChanged { get; private set; }
	}
	public class QueryPositionByTimeEventArgs : EventArgs {
		public QueryPositionByTimeEventArgs(DateTime dateTime) {
			DateTime = dateTime;
			Position = -1;
		}
		public DateTime DateTime { get; private set; }
		public double Position { get; set; }
	}
}
