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
using System.Drawing;
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Utils.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
namespace DevExpress.XtraScheduler {
	#region PopupMenuShowingEventHandler
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
	#endregion
	#region PopupMenuShowingEventArgs
	public class PopupMenuShowingEventArgs : EventArgs {
		SchedulerPopupMenu menu;
		public PopupMenuShowingEventArgs(SchedulerPopupMenu menu) {
			this.menu = menu;
		}
		public SchedulerPopupMenu Menu {
			get { return menu; }
			set { menu = value; }
		}
	}
	#endregion
	#region PreparePopupMenuEventHandler
	[Obsolete("You should use the 'PopupMenuShowingEventHandler' instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void PreparePopupMenuEventHandler(object sender, PreparePopupMenuEventArgs e);
	#endregion
	#region PreparePopupMenuEventArgs
	[Obsolete("You should use the 'PopupMenuShowingEventArgs' instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
	public class PreparePopupMenuEventArgs : PopupMenuShowingEventArgs {
		public PreparePopupMenuEventArgs(SchedulerPopupMenu menu)
			: base(menu) {
		}
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
	#region PrepareContextMenuEventHandler
	[Obsolete("You should use the 'PopupMenuShowingEventHandler' instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void PrepareContextMenuEventHandler(object sender, PrepareContextMenuEventArgs e);
	#endregion
	#region PrepareContextMenuEventArgs
	[Obsolete("You should use the 'PopupMenuShowingEventArgs' instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
	public class PrepareContextMenuEventArgs : PreparePopupMenuEventArgs {
		public PrepareContextMenuEventArgs(SchedulerPopupMenu menu)
			: base(menu) {
		}
	}
	#endregion
	#region GotoDateFormEventHandler
	public delegate void GotoDateFormEventHandler(object sender, GotoDateFormEventArgs e);
	#endregion
	#region GotoDateFormEventArgs
	public class GotoDateFormEventArgs : ShowFormEventArgs {
	#region Fields
		DateTime date;
		SchedulerViewType viewType = SchedulerViewType.Day;
	#endregion
	#region Properties
		public DateTime Date { get { return date; } set { date = value; } }
		public SchedulerViewType SchedulerViewType { get { return viewType; } set { viewType = value; } }
	#endregion
	}
	#endregion
	#region ShowFormEventArgs
	public class ShowFormEventArgs : EventArgs {
		#region Fields
		DialogResult dialogResult = DialogResult.None;
		bool handled;
		IWin32Window parent;
		#endregion
		#region Properties
		public bool Handled { get { return handled; } set { handled = value; } }
		public DialogResult DialogResult { get { return dialogResult; } set { dialogResult = value; } }
		public IWin32Window Parent { get { return parent; } set { parent = value; } }
		#endregion
	}
	#endregion
	#region AppointmentFormEventHandler
	public delegate void AppointmentFormEventHandler(object sender, AppointmentFormEventArgs e);
	#endregion
	#region AppointmentFormEventArgs
	public class AppointmentFormEventArgs : ShowFormEventArgs {
		#region Fields
		Appointment apt;
		bool readOnly;
		bool openRecurrenceForm;
		CommandSourceType commandSourceType;
		#endregion
		public AppointmentFormEventArgs(Appointment apt)
			: this(apt, CommandSourceType.Unknown, false, false) {
		}
		protected internal AppointmentFormEventArgs(Appointment apt, CommandSourceType commandSourceType, bool readOnly, bool openRecurrenceForm) {
			if(apt == null)
				Exceptions.ThrowArgumentException("apt", apt);
			this.apt = apt;
			this.commandSourceType = commandSourceType;
			this.readOnly = readOnly;
			this.openRecurrenceForm = openRecurrenceForm;
		}
		#region Properties
		public Appointment Appointment { get { return apt; } }
		public CommandSourceType CommandSourceType { get { return commandSourceType; } }
		public bool ReadOnly { get { return readOnly; } }
		public bool OpenRecurrenceForm { get { return openRecurrenceForm; } }
		#endregion
	}
	#endregion
	#region AppointmentImagesEventHandler
	public delegate void AppointmentImagesEventHandler(object sender, AppointmentImagesEventArgs e);
	#endregion
	#region AppointmentImagesEventArgs
	public class AppointmentImagesEventArgs : EventArgs {
		IAppointmentViewInfo viewInfo;
		AppointmentImageInfoCollection imageInfos;
		public AppointmentImagesEventArgs(IAppointmentViewInfo viewInfo, AppointmentImageInfoCollection imageInfos)
			: base() {
			Guard.ArgumentNotNull(imageInfos, "imageInfos");
			Guard.ArgumentNotNull(viewInfo, "viewInfo");
			this.viewInfo = viewInfo;
			this.imageInfos = imageInfos;
		}
		public AppointmentImageInfoCollection ImageInfoList { get { return imageInfos; } }
		public Appointment Appointment { get { return ViewInfo.Appointment; } }
		public IAppointmentViewInfo ViewInfo { get { return viewInfo; } }
	}
	#endregion
	#region RecurrentAppointmentActionFormEventArgs
	public class RecurrentAppointmentActionFormEventArgs : AppointmentFormEventArgs {
		RecurrentAppointmentAction queryResult;
		public RecurrentAppointmentActionFormEventArgs(Appointment apt)
			: base(apt) {
		}
		public RecurrentAppointmentActionFormEventArgs(Appointment apt, CommandSourceType commandSourceType, bool readOnly, bool openRecurrenceForm): base(apt, commandSourceType, readOnly, openRecurrenceForm) {
		}
		public RecurrentAppointmentAction QueryResult { get { return queryResult; } set { queryResult = value; } }
	}
	#endregion
	#region DeleteRecurrentAppointmentFormEventHandler
	public delegate void DeleteRecurrentAppointmentFormEventHandler(object sender, DeleteRecurrentAppointmentFormEventArgs e);
	#endregion
	#region DeleteRecurrentAppointmentFormEventArgs
	public class DeleteRecurrentAppointmentFormEventArgs : RecurrentAppointmentActionFormEventArgs {
		public DeleteRecurrentAppointmentFormEventArgs(Appointment apt)
			: base(apt) {
		}
		[Obsolete("You should use the 'QueryResult' instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool DeleteSeries {
			get {
				return QueryResult == RecurrentAppointmentAction.Series;
			}
			set {
				QueryResult = value ? RecurrentAppointmentAction.Series : RecurrentAppointmentAction.Occurrence;
			}
		}
	}
	#endregion
	#region EditRecurrentAppointmentFormEventHandler
	public delegate void EditRecurrentAppointmentFormEventHandler(object sender, EditRecurrentAppointmentFormEventArgs e);
	#endregion
	#region EditRecurrentAppointmentFormEventArgs
	public class EditRecurrentAppointmentFormEventArgs : RecurrentAppointmentActionFormEventArgs {
		public EditRecurrentAppointmentFormEventArgs(Appointment apt)
			: base(apt) {
		}
		public EditRecurrentAppointmentFormEventArgs(Appointment apt, CommandSourceType commandSourceType, bool readOnly, bool openRecurrenceForm): base(apt, commandSourceType, readOnly, openRecurrenceForm) {
		}
	}
	#endregion
	#region RemindersFormEventHandler
	public delegate void RemindersFormEventHandler(object sender, RemindersFormEventArgs e);
	#endregion
	#region RemindersFormEventArgs
	public class RemindersFormEventArgs : ShowFormEventArgs {
		ReminderAlertNotificationCollection alerts;
		public RemindersFormEventArgs(ReminderAlertNotificationCollection alerts) {
			if (alerts == null)
				Exceptions.ThrowArgumentException("alerts", alerts);
			this.alerts = alerts;
		}
		public ReminderAlertNotificationCollection AlertNotifications { get { return alerts; } }
	}
	#endregion
	#region DefaultDrawDelegate
	public delegate void DefaultDrawDelegate();
	#endregion
	#region CustomDrawObjectEventHandler
	public delegate void CustomDrawObjectEventHandler(object sender, CustomDrawObjectEventArgs e);
	#endregion
	#region CustomDrawObjectEventArgs
	public class CustomDrawObjectEventArgs : EventArgs {
	#region Fields
		ObjectInfoArgs objectInfo;
		Rectangle bounds;
		DefaultDrawDelegate defaultDrawDelegate;
		bool handled;
	#endregion
		public CustomDrawObjectEventArgs(ObjectInfoArgs objectInfo, Rectangle bounds)
			: this(objectInfo, bounds, null) {
		}
		public CustomDrawObjectEventArgs(ObjectInfoArgs objectInfo, Rectangle bounds, DefaultDrawDelegate defaultDrawDelegate) {
			if (objectInfo == null)
				Exceptions.ThrowArgumentException("objectInfo", objectInfo);
			this.objectInfo = objectInfo;
			this.bounds = bounds;
			this.defaultDrawDelegate = defaultDrawDelegate;
		}
	#region Properties
		public Rectangle Bounds { get { return bounds; } }
		public ObjectInfoArgs ObjectInfo { get { return objectInfo; } }
		public virtual bool Handled { get { return handled; } set { handled = value; } }
		public virtual Graphics Graphics { get { return ObjectInfo.Graphics; } }
		public virtual GraphicsCache Cache { get { return ObjectInfo.Cache; } }
		internal DefaultDrawDelegate DefaultDrawDelegate { get { return defaultDrawDelegate; } }
	#endregion
		public void DrawDefault() {
			if (defaultDrawDelegate != null)
				defaultDrawDelegate();
		}
	}
	#endregion
	#region InplaceEditorEventHandler
	public delegate void InplaceEditorEventHandler(object sender, InplaceEditorEventArgs e);
	#endregion
	#region InplaceEditorEventArgs
	public class InplaceEditorEventArgs : AppointmentEventArgs {
		ISchedulerInplaceEditor inplaceEditor;
		ISchedulerInplaceEditorEx inplaceEditorEx;
		SchedulerInplaceEditorEventArgs schedulerInplaceEditorEventArgs;
		public InplaceEditorEventArgs(Appointment apt)
			: base(apt) {
		}
		public ISchedulerInplaceEditor InplaceEditor {
			get { return inplaceEditor; }
			set {
				if (inplaceEditor == value)
					return;
				if (inplaceEditor != null)
					inplaceEditor.Dispose();
				inplaceEditor = value;
			}
		}
		public ISchedulerInplaceEditorEx InplaceEditorEx {
			get { return inplaceEditorEx; }
			set {
				if (inplaceEditorEx == value)
					return;
				if (inplaceEditorEx != null)
					inplaceEditorEx.Dispose();
				inplaceEditorEx = value;
			}
		}
		public SchedulerInplaceEditorEventArgs SchedulerInplaceEditorEventArgs {
			get { return schedulerInplaceEditorEventArgs; }
			set {
				schedulerInplaceEditorEventArgs = value;
			}
		}
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
	#region PersistentObjectEventHandler
	public delegate void PrepareFilterColumnEventHandler(object sender, PrepareFilterColumnEventArgs e);
	#endregion
	#region PrepareFilterColumnEventArgs
	public class PrepareFilterColumnEventArgs : CancelEventArgs {
		FilterColumn filterColumn;
		public PrepareFilterColumnEventArgs(FilterColumn filterColumn) {
			this.filterColumn = filterColumn;
		}
		public FilterColumn FilterColumn { get { return filterColumn; } set { filterColumn = value; } }
	}
	#endregion
	#region AppointmentDependencyFormEventHandler
	public delegate void AppointmentDependencyFormEventHandler(object sender, AppointmentDependencyFormEventArgs e);
	#endregion
	#region AppointmentDependencyFormEventArgs
	public class AppointmentDependencyFormEventArgs : ShowFormEventArgs {
		#region Fields
		AppointmentDependency dep;
		bool readOnly;
		CommandSourceType commandSourceType;
		#endregion
		public AppointmentDependencyFormEventArgs(AppointmentDependency dep)
			: this(dep, CommandSourceType.Unknown, false) {
		}
		protected internal AppointmentDependencyFormEventArgs(AppointmentDependency dep, CommandSourceType commandSourceType, bool readOnly) {
			if (dep == null)
				Exceptions.ThrowArgumentException("dep", dep);
			this.dep = dep;
			this.commandSourceType = commandSourceType;
			this.readOnly = readOnly;
		}
		#region Properties
		public AppointmentDependency AppointmentDependency { get { return dep; } }
		public CommandSourceType CommandSourceType { get { return commandSourceType; } }
		public bool ReadOnly { get { return readOnly; } }
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region SchedulerViewEventHandler
	public delegate void SchedulerViewEventHandler(object sender, SchedulerViewEventArgs e);
	#endregion
	#region SchedulerViewEventArgs
	public class SchedulerViewEventArgs : EventArgs {
		SchedulerViewBase view;
		public SchedulerViewEventArgs(SchedulerViewBase view) {
			if(view == null)
				Exceptions.ThrowArgumentException("view", view);
			this.view = view;
		}
		public SchedulerViewBase View { get { return view; } }
	}
	#endregion
	#region SchedulerViewCancelEventHandler
	public delegate void SchedulerViewCancelEventHandler(object sender, SchedulerViewCancelEventArgs e);
	#endregion
	#region SchedulerViewCancelEventArgs
	public class SchedulerViewCancelEventArgs : SchedulerViewEventArgs {
		bool cancel;
		public SchedulerViewCancelEventArgs(SchedulerViewBase view)
			: base(view) {
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	#endregion
}
