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
using System.Collections.Specialized;
using DevExpress.XtraScheduler.Native;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.XtraScheduler;
using DevExpress.Web;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Drawing;
using System.Collections.Generic;
namespace DevExpress.Web.ASPxScheduler {
	#region SchedulerFormEventArgs
	public abstract class SchedulerFormEventArgs : EventArgs {
		SchedulerFormTemplateContainer container;
		string formTemplateUrl = string.Empty;
		bool cancel;
		protected SchedulerFormEventArgs(SchedulerFormTemplateContainer container) {
			if (container == null)
				Exceptions.ThrowArgumentNullException("container");
			this.container = container;
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		public string FormTemplateUrl { get { return formTemplateUrl; } set { formTemplateUrl = value; } }
		public SchedulerFormTemplateContainer Container { get { return container; } set { container = value; } }
	}
	#endregion
	public enum SchedulerFormAction { Create, Edit };
	#region AppointmentFormBaseEventArgs
	public abstract class AppointmentFormBaseEventArgs : SchedulerFormEventArgs {
		readonly Appointment appointment;
		readonly SchedulerFormAction action = SchedulerFormAction.Create;
		protected AppointmentFormBaseEventArgs(SchedulerFormTemplateContainer container, Appointment appointment, SchedulerFormAction action)
			: base(container) {
			if (appointment == null)
				Exceptions.ThrowArgumentNullException("appointment");
			this.appointment = appointment;
			this.action = action;
		}
		public Appointment Appointment { get { return appointment; } }
		public SchedulerFormAction Action { get { return action; } }
	}
	#endregion
	#region AppointmentFormEventHandler
	public delegate void AppointmentFormEventHandler(object sender, AppointmentFormEventArgs e);
	#endregion
	#region AppointmentFormEventArgs
	public class AppointmentFormEventArgs : AppointmentFormBaseEventArgs {
		public AppointmentFormEventArgs(AppointmentFormTemplateContainer container, Appointment appointment, SchedulerFormAction action)
			: base(container, appointment, action) {
		}
		public new AppointmentFormTemplateContainer Container { get { return (AppointmentFormTemplateContainer)base.Container; } set { base.Container = value; } }
	}
	#endregion
	#region AppointmentInplaceEditorEventHandler
	public delegate void AppointmentInplaceEditorEventHandler(object sender, AppointmentInplaceEditorEventArgs e);
	#endregion
	#region AppointmentInplaceEditorEventArgs
	public class AppointmentInplaceEditorEventArgs : AppointmentFormBaseEventArgs {
		public AppointmentInplaceEditorEventArgs(AppointmentInplaceEditorTemplateContainer container, Appointment appointment, SchedulerFormAction action)
			: base(container, appointment, action) {
		}
		public new AppointmentInplaceEditorTemplateContainer Container { get { return (AppointmentInplaceEditorTemplateContainer)base.Container; } set { base.Container = value; } }
	}
	#endregion
	#region SchedulerCallbackCommandEventHandler
	public delegate void SchedulerCallbackCommandEventHandler(object sender, SchedulerCallbackCommandEventArgs e);
	#endregion
	#region SchedulerCallbackCommandEventArgs
	public class SchedulerCallbackCommandEventArgs {
		string commandId;
		SchedulerCallbackCommand command;
		public SchedulerCallbackCommandEventArgs(string commandId, SchedulerCallbackCommand command) {
			this.commandId = commandId;
			this.command = command;
		}
		public SchedulerCallbackCommand Command { get { return command; } set { command = value; } }
		public string CommandId { get { return commandId; } }
	}
	#endregion
	#region GotoDateFormEventHandler
	public delegate void GotoDateFormEventHandler(object sender, GotoDateFormEventArgs e);
	#endregion
	#region GotoDateFormEventArgs
	public class GotoDateFormEventArgs : SchedulerFormEventArgs {
		public GotoDateFormEventArgs(GotoDateFormTemplateContainer container)
			: base(container) {
		}
		public new GotoDateFormTemplateContainer Container { get { return (GotoDateFormTemplateContainer)base.Container; } set { base.Container = value; } }
	}
	#endregion
	#region RecurrentAppointmentDeleteFormEventHandler
	public delegate void RecurrentAppointmentDeleteFormEventHandler(object sender, RecurrentAppointmentDeleteFormEventArgs e);
	#endregion
	#region RecurrentAppointmentDeleteFormEventArgs
	public class RecurrentAppointmentDeleteFormEventArgs : SchedulerFormEventArgs {
		public RecurrentAppointmentDeleteFormEventArgs(RecurrentAppointmentDeleteFormTemplateContainer container)
			: base(container) {
		}
		public new RecurrentAppointmentDeleteFormTemplateContainer Container { get { return (RecurrentAppointmentDeleteFormTemplateContainer)base.Container; } set { base.Container = value; } }
	}
	#endregion
	#region RecurrentAppointmentEditFormEventHandler
	public delegate void RecurrentAppointmentEditFormEventHandler(object sender, RecurrentAppointmentEditFormEventArgs e);
	#endregion
	#region RecurrentAppointmentEditFormEventArgs
	public class RecurrentAppointmentEditFormEventArgs : SchedulerFormEventArgs {
		public RecurrentAppointmentEditFormEventArgs(RecurrentAppointmentEditFormTemplateContainer container)
			: base(container) {
		}
		public new RecurrentAppointmentEditFormTemplateContainer Container { get { return (RecurrentAppointmentEditFormTemplateContainer)base.Container; } set { base.Container = value; } }
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
	#region RemindersFormEventArgs
	public class RemindersFormEventArgs : SchedulerFormEventArgs {
		public RemindersFormEventArgs(RemindersFormTemplateContainer container)
			: base(container) {
		}
		public new RemindersFormTemplateContainer Container { get { return (RemindersFormTemplateContainer)base.Container; } set { base.Container = value; } }
	}
	#endregion
	#region RemindersFormEventHandler
	public delegate void RemindersFormEventHandler(object sender, RemindersFormEventArgs e);
	#endregion
	#region CustomizeElementStyleEventHandler
	public delegate void CustomizeElementStyleEventHandler(object sender, CustomizeElementStyleEventArgs e);
	#endregion
	#region CustomizeElementStyleEventArgs
	public class CustomizeElementStyleEventArgs : EventArgs {
		WebElementType elementType;
		ITimeCell cell;
		AppearanceStyleBase style;
		bool isAlternate;
		public CustomizeElementStyleEventArgs(AppearanceStyleBase style, WebElementType type, ITimeCell cell, bool isAlternate) {
			this.elementType = type;
			this.cell = cell;
			this.style = style;
			this.isAlternate = isAlternate;
		}
		public WebElementType ElementType { get { return elementType; } }
		public ITimeCell Cell { get { return cell; } }
		public Style Style { get { return style; } }
		public bool IsAlternate { get { return isAlternate; } }
	}
	#endregion
	#region AppointmentImagesEventHandler
	public delegate void AppointmentImagesEventHandler(object sender, AppointmentImagesEventArgs e);
	#endregion
	#region AppointmentImagesEventArgs
	public class AppointmentImagesEventArgs : EventArgs {
		AppointmentImageInfoCollection imageInfos;
		IAppointmentViewInfo viewInfo;
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
	#region ASPxSchedulerDataInsertingEventHandler
	public delegate void ASPxSchedulerDataInsertingEventHandler(object sender, ASPxSchedulerDataInsertingEventArgs e);
	#endregion
	#region ASPxSchedulerDataInsertingEventArgs
	public class ASPxSchedulerDataInsertingEventArgs : CancelEventArgs {
		OrderedDictionary newValues;
		public ASPxSchedulerDataInsertingEventArgs(OrderedDictionary values) {
			if (values == null)
				Exceptions.ThrowArgumentNullException("values");
			this.newValues = values;
		}
		public OrderedDictionary NewValues { get { return newValues; } }
	}
	#endregion
	#region ASPxSchedulerDataInsertedEventHandler
	public delegate void ASPxSchedulerDataInsertedEventHandler(object sender, ASPxSchedulerDataInsertedEventArgs e);
	#endregion
	#region ASPxSchedulerDataInsertedEventArgs
	public class ASPxSchedulerDataInsertedEventArgs : ASPxSchedulerDataBaseUpdatedEventArgs {
		#region Fields
		OrderedDictionary newValues;
		object keyFieldValue;
		#endregion
		public ASPxSchedulerDataInsertedEventArgs(int affectedRecords, Exception exception, OrderedDictionary values)
			: base(affectedRecords, exception) {
			if (values == null)
				Exceptions.ThrowArgumentNullException("values");
			this.newValues = values;
		}
		#region Properties
		public object KeyFieldValue { get { return keyFieldValue; } set { keyFieldValue = value; } }
		public OrderedDictionary NewValues { get { return newValues; } }
		#endregion
	}
	#endregion
	#region ASPxSchedulerDataUpdatingEventHandler
	public delegate void ASPxSchedulerDataUpdatingEventHandler(object sender, ASPxSchedulerDataUpdatingEventArgs e);
	#endregion
	#region ASPxSchedulerDataUpdatingEventArgs
	public class ASPxSchedulerDataUpdatingEventArgs : CancelEventArgs {
		#region Fields
		OrderedDictionary keys;
		OrderedDictionary oldValues;
		OrderedDictionary newValues;
		#endregion
		public ASPxSchedulerDataUpdatingEventArgs(OrderedDictionary keys, OrderedDictionary oldValues, OrderedDictionary newValues) {
			if (keys == null)
				Exceptions.ThrowArgumentNullException("keys");
			if (oldValues == null)
				Exceptions.ThrowArgumentNullException("oldValues");
			if (newValues == null)
				Exceptions.ThrowArgumentNullException("newValues");
			this.keys = keys;
			this.oldValues = oldValues;
			this.newValues = newValues;
		}
		#region Properties
		public OrderedDictionary Keys { get { return keys; } }
		public OrderedDictionary NewValues { get { return newValues; } }
		public OrderedDictionary OldValues { get { return oldValues; } }
		#endregion
	}
	#endregion
	#region ASPxSchedulerDataBaseUpdatedEventArgs (abstract class)
	public abstract class ASPxSchedulerDataBaseUpdatedEventArgs : EventArgs {
		#region Fields
		readonly int affectedRecords;
		readonly Exception exception;
		bool exceptionHandled;
		#endregion
		protected ASPxSchedulerDataBaseUpdatedEventArgs(int affectedRecords, Exception exception) {
			this.exception = exception;
			this.affectedRecords = affectedRecords;
			this.exceptionHandled = (exception == null);
		}
		#region Properties
		public int AffectedRecords { get { return affectedRecords; } }
		public Exception Exception { get { return exception; } }
		public bool ExceptionHandled { get { return exceptionHandled; } set { exceptionHandled = value; } }
		#endregion
	}
	#endregion
	#region ASPxSchedulerDataUpdatedEventHandler
	public delegate void ASPxSchedulerDataUpdatedEventHandler(object sender, ASPxSchedulerDataUpdatedEventArgs e);
	#endregion
	#region ASPxSchedulerDataUpdatedEventArgs
	public class ASPxSchedulerDataUpdatedEventArgs : ASPxSchedulerDataBaseUpdatedEventArgs {
		#region Fields
		OrderedDictionary keys;
		OrderedDictionary oldValues;
		OrderedDictionary newValues;
		#endregion
		public ASPxSchedulerDataUpdatedEventArgs(int affectedRecords, Exception exception, OrderedDictionary keys, OrderedDictionary oldValues, OrderedDictionary newValues)
			: base(affectedRecords, exception) {
			if (keys == null)
				Exceptions.ThrowArgumentNullException("keys");
			if (oldValues == null)
				Exceptions.ThrowArgumentNullException("oldValues");
			if (newValues == null)
				Exceptions.ThrowArgumentNullException("newValues");
			this.keys = keys;
			this.oldValues = oldValues;
			this.newValues = newValues;
		}
		#region Properties
		public OrderedDictionary Keys { get { return keys; } }
		public OrderedDictionary NewValues { get { return newValues; } }
		public OrderedDictionary OldValues { get { return oldValues; } }
		#endregion
	}
	#endregion
	#region ASPxSchedulerDataDeletingEventHandler
	public delegate void ASPxSchedulerDataDeletingEventHandler(object sender, ASPxSchedulerDataDeletingEventArgs e);
	#endregion
	#region ASPxSchedulerDataDeletingEventArgs
	public class ASPxSchedulerDataDeletingEventArgs : CancelEventArgs {
		#region Fields
		OrderedDictionary keys;
		OrderedDictionary values;
		#endregion
		public ASPxSchedulerDataDeletingEventArgs(OrderedDictionary keys, OrderedDictionary values) {
			if (keys == null)
				Exceptions.ThrowArgumentNullException("keys");
			if (values == null)
				Exceptions.ThrowArgumentNullException("values");
			this.keys = keys;
			this.values = values;
		}
		#region Properties
		public OrderedDictionary Keys { get { return keys; } }
		public OrderedDictionary Values { get { return values; } }
		#endregion
	}
	#endregion
	#region ASPxSchedulerDataDeletedEventHandler
	public delegate void ASPxSchedulerDataDeletedEventHandler(object sender, ASPxSchedulerDataDeletedEventArgs e);
	#endregion
	#region ASPxSchedulerDataDeletedEventArgs
	public class ASPxSchedulerDataDeletedEventArgs : ASPxSchedulerDataBaseUpdatedEventArgs {
		#region Fields
		OrderedDictionary keys;
		OrderedDictionary values;
		#endregion
		public ASPxSchedulerDataDeletedEventArgs(int affectedRecords, Exception exception, OrderedDictionary keys, OrderedDictionary values)
			: base(affectedRecords, exception) {
			if (keys == null)
				Exceptions.ThrowArgumentNullException("keys");
			if (values == null)
				Exceptions.ThrowArgumentNullException("values");
			this.keys = keys;
			this.values = values;
		}
		#region Properties
		public OrderedDictionary Keys { get { return keys; } }
		public OrderedDictionary Values { get { return values; } }
		#endregion
	}
	#endregion
	#region ActiveViewChangingEventHandler
	public delegate void ActiveViewChangingEventHandler(object sender, ActiveViewChangingEventArgs e);
	#endregion
	#region ASPxSchedulerActiveViewChangingEventArgs
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
	#region PopupMenuShowingEventHandler
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
	#endregion
	#region PopupMenuShowingEventArgs
	public class PopupMenuShowingEventArgs : EventArgs {
		readonly ASPxSchedulerPopupMenu menu;
		public PopupMenuShowingEventArgs(ASPxSchedulerPopupMenu menu) {
			if (menu == null)
				Exceptions.ThrowArgumentNullException("menu");
			this.menu = menu;
		}
		public ASPxSchedulerPopupMenu Menu { get { return menu; } }
	}
	#endregion
	#region PreparePopupMenuEventHandler
	[Obsolete("You should use the 'PopupMenuShowingEventHandler' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void PreparePopupMenuEventHandler(object sender, PreparePopupMenuEventArgs e);
	#endregion
	#region PreparePopupMenuEventArgs
	[Obsolete("You should use the 'PopupMenuShowingEventArgs' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public class PreparePopupMenuEventArgs : PopupMenuShowingEventArgs {
		public PreparePopupMenuEventArgs(ASPxSchedulerPopupMenu menu)
			: base(menu) {
		}
	}
	#endregion
	#region ASPxSchedulerTimeCellPreparedEventHandler
	public delegate void ASPxSchedulerTimeCellPreparedEventHandler(object handler, ASPxSchedulerTimeCellPreparedEventArgs e);
	#endregion
	#region ASPxSchedulerTimeCellPreparedEventArgs
	public class ASPxSchedulerTimeCellPreparedEventArgs : EventArgs {
		#region Fields
		TableCell cell;
		TimeInterval interval;
		XtraScheduler.Resource resource;
		SchedulerViewBase view;
		#endregion
		public ASPxSchedulerTimeCellPreparedEventArgs(TableCell cell, IWebTimeCell timeCell, SchedulerViewBase view) {
			Guard.ArgumentNotNull(cell, "cell");
			Guard.ArgumentNotNull(timeCell, "timeCell");
			Guard.ArgumentNotNull(view, "view");
			this.cell = cell;
			this.view = view;
			this.interval = timeCell.Interval.Clone();
			this.resource = timeCell.Resource;
		}
		#region Properties
		public TableCell Cell { get { return cell; } }
		public TimeInterval Interval { get { return interval; } }
		public Resource Resource { get { return resource; } }
		public SchedulerViewBase View { get { return view; } }
		#endregion
	}
	#endregion
	#region ASPxSchedulerCustomErrorTextEventHandler
	public delegate void ASPxSchedulerCustomErrorTextEventHandler(object handler, ASPxSchedulerCustomErrorTextEventArgs e);
	#endregion
	#region ASPxSchedulerCustomErrorTextEventArgs
	public class ASPxSchedulerCustomErrorTextEventArgs : EventArgs {
		#region Fields
		string errorText;
		Exception exception;
		#endregion
		public ASPxSchedulerCustomErrorTextEventArgs(Exception exception, string errorText) {
			Guard.ArgumentNotNull(exception, "exception");
			this.ErrorText = errorText;
			this.exception = exception;
		}
		#region Properties
		public string ErrorText {
			get { return errorText; }
			set {
				if (value == null)
					value = String.Empty;
				errorText = value;
			}
		}
		public Exception Exception { get { return exception; } }
		#endregion
	}
	#endregion
	#region PrepareFormPopupContainerHandler
	public delegate void ASPxSchedulerPrepareFormPopupContainerHandler(object sender, ASPxSchedulerPrepareFormPopupContainerEventArgs e);
	#endregion
	#region ASPxSchedulerPrepareFormPopupContainerEventArgs
	public class ASPxSchedulerPrepareFormPopupContainerEventArgs : EventArgs {
		DevExpress.Web.ASPxPopupControl popup;
		public ASPxSchedulerPrepareFormPopupContainerEventArgs(DevExpress.Web.ASPxPopupControl popup) {
			this.popup = popup;
		}
		public DevExpress.Web.ASPxPopupControl Popup { get { return popup; } }
	}
	#endregion
	#region InitClientAppointmentHandler
	public delegate void InitClientAppointmentHandler(object sender, InitClientAppointmentEventArgs args);
	#endregion
	#region InitClientAppointmentEventArgs
	public class InitClientAppointmentEventArgs : EventArgs {
		Dictionary<string, object> properties;
		Appointment appointment;
		public InitClientAppointmentEventArgs(Appointment appointment, Dictionary<string, object> properties) {
			this.appointment = appointment;
			this.properties = properties;
		}
		public Dictionary<string, object> Properties { get { return properties; } }
		public Appointment Appointment { get { return appointment; } }
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region ASPxSchedulerCreatedEventHandler
	public delegate void ASPxSchedulerCreatedEventHandler(object sender, ASPxSchedulerCreatedEventArgs e);
	#endregion
	#region ASPxSchedulerCreatedEventArgs
	public class ASPxSchedulerCreatedEventArgs : EventArgs {
		ASPxScheduler control;
		public ASPxSchedulerCreatedEventArgs(ASPxScheduler control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
		}
		public ASPxScheduler Control { get { return control; } }
	}
	#endregion
}
