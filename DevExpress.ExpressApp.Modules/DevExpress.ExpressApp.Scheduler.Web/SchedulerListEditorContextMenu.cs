#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.Internal;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Compatibility;
namespace DevExpress.ExpressApp.Scheduler.Web {
	public class ASPxSchedulerContextMenu : IContextMenuTemplate, IXafCallbackHandler {
		public const string XafMenuCommandPrefix = "Xaf_";
		public const string SelectionChangedCommandId = "SELCHNG";
		private List<WebContextMenuActionContainer> containers = new List<WebContextMenuActionContainer>();
		private ASPxSchedulerListEditor editor;
		private ActionBase processSelectedItemAction;
		private ASPxSchedulerPopupMenu appointmentMenu;
		private string appointmentMenuClientID;
		protected static bool IsActionVisibleInAppointmentMenu(ActionBase action) {
			return action != null && action.Active && (action.SelectionDependencyType == SelectionDependencyType.RequireSingleObject || action.SelectionDependencyType == SelectionDependencyType.RequireMultipleObjects) && action.Id != ListViewController.InlineEditActionId;
		}
		protected static bool IsActionVisibleInDefaultMenu(ActionBase action) {
			return action != null && action.Active && action.Enabled && action.SelectionDependencyType == SelectionDependencyType.Independent && action.Id != ListViewController.InlineEditActionId;
		}
		private void editor_ControlsCreated(object sender, EventArgs e) {
			ASPxScheduler scheduler = (ASPxScheduler)editor.SchedulerControl;
			scheduler.PopupMenuShowing += new PopupMenuShowingEventHandler(Scheduler_PopupMenuShowing);
			scheduler.Load += new EventHandler(scheduler_Load);
		}
		private void scheduler_Load(object sender, EventArgs e) {
			ASPxScheduler scheduler = (ASPxScheduler)sender;
			scheduler.Load -= new EventHandler(scheduler_Load);
			if(CallbackManager != null) {
				CallbackManager.RegisterHandler(scheduler.UniqueID, this);
			}
		}
		private void Scheduler_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
			ASPxSchedulerPopupMenu menu = e.Menu;
			if(menu.MenuId.Equals(SchedulerMenuItemId.DefaultMenu)) {
				PrepareDefaultMenu(menu);
			}
			else if(menu.MenuId.Equals(SchedulerMenuItemId.AppointmentMenu)) {
				PrepareAppointmentMenu(menu);
			}
		}
		private void PrepareDefaultMenu(ASPxSchedulerPopupMenu menu) {
			ClearUnusedDefaultMenuItems(menu);
			IList<IList<ActionBase>> groupedActions = GetGroupedContextActions(IsActionVisibleInDefaultMenu);
			AddActionMenuItems(menu, groupedActions);
			InitilalizeClientScripts(menu);
		}
		private void PrepareAppointmentMenu(ASPxSchedulerPopupMenu menu) {
			ASPxScheduler scheduler = editor.SchedulerControl;
			appointmentMenu = menu;
			appointmentMenuClientID = menu.ClientID; 
			ClearUnusedAppointmentMenuItems(menu);
			IList<IList<ActionBase>> groupedActions = GetGroupedContextActions(IsActionVisibleInAppointmentMenu);
			AddActionMenuItems(menu, groupedActions);
			InitilalizeClientScripts(menu);
		}
		private IList<IList<ActionBase>> GetGroupedContextActions(Predicate<ActionBase> filter) {
			List<IList<ActionBase>> result = new List<IList<ActionBase>>();
			foreach(WebContextMenuActionContainer container in containers) {
				List<ActionBase> actions = null;
				foreach(ActionBase action in container.Actions) {
					if(filter(action)) {
						if(actions == null) {
							actions = new List<ActionBase>();
							result.Add(actions);
						}
						actions.Add(action);
					}
				}
			}
			return result;
		}
		private static void AddActionMenuItems(ASPxSchedulerPopupMenu menu, IList<IList<ActionBase>> groupedActions) {
			for(int i = groupedActions.Count - 1; i >= 0; i--) {
				IList<ActionBase> actions = groupedActions[i];
				MenuItem menuItem;
				for(int j = actions.Count - 1; j > 0; j--) {
					menuItem = CreateMenuItem(actions[j]);
					menu.Items.Insert(0, menuItem);
				}
				menuItem = CreateMenuItem(actions[0]);
				menuItem.BeginGroup = true;
				menu.Items.Insert(0, menuItem);
			}
		}
		private static MenuItem CreateMenuItem(ActionBase action) {
			MenuItem menuItem = new MenuItem(action.Caption, GetMenuItemName(action));
			ASPxImageHelper.SetImageProperties(menuItem.Image, action.ImageName);
			menuItem.ClientEnabled = action.Enabled;
			return menuItem;
		}
		private static string GetMenuItemName(ActionBase action) {
			return XafMenuCommandPrefix + action.Id;
		}
		private void InitilalizeClientScripts(ASPxSchedulerPopupMenu menu) {
			menu.JSProperties["cpConfirmationMessages"] = GetConfirmationMessages(menu);
			menu.JSProperties["cpUsePostBackFlags"] = GetUsePostBackFlags(menu);
			menu.ClientSideEvents.ItemClick = GetItemClickScript(menu);
		}
		private IDictionary<string, string> GetConfirmationMessages(ASPxSchedulerPopupMenu menu) {
			Dictionary<string, string> result = new Dictionary<string, string>();
			foreach(MenuItem menuItem in menu.Items) {
				ActionBase action = FindActionByMenuItem(menuItem);
				if(action != null) {
					string message = action.GetFormattedConfirmationMessage();
					if(!string.IsNullOrEmpty(message)) {
						result.Add(menuItem.Name, message);
					}
				}
			}
			return result;
		}
		private IDictionary<string, bool> GetUsePostBackFlags(ASPxSchedulerPopupMenu menu) {
			Dictionary<string, bool> result = new Dictionary<string, bool>();
			foreach(MenuItem menuItem in menu.Items) {
				ActionBase action = FindActionByMenuItem(menuItem);
				if(action != null && GetPostBackRequired(action)) {
					result.Add(menuItem.Name, true);
				}
			}
			return result;
		}
		private bool GetPostBackRequired(ActionBase action) {
			return action.Model.GetValue<bool>("IsPostBackRequired");
		}
		private string GetItemClickScript(ASPxSchedulerPopupMenu menu) {
			ASPxScheduler scheduler = editor.SchedulerControl;
			string schedulerId = GetSchedulerId(scheduler);
			string formatString = HttpUtility.JavaScriptStringEncode(CultureInfo.CurrentCulture.DateTimeFormat.FullDateTimePattern);
			string raiseCallbackString = CallbackManager.GetScript(scheduler.UniqueID, "parameters", "confirmation", "usePostBack");
			string defaultItemClickScript = menu.ClientSideEvents.ItemClick;
			string script = string.Format(
@"function(s, e) {{
    var scheduler = {0};
    var itemName = e.item.name;
    if(e.item.GetItemCount() <= 0 && itemName.indexOf('{1}') == 0) {{
        var parameters = itemName + ':';
        if(itemName == '{1}{2}') {{
            var start = scheduler.selection.interval.GetStart();
            var end = scheduler.selection.interval.GetEnd();
            var formatter = new ASPx.DateFormatter();
            formatter.SetFormatString('{3}');
            parameters += formatter.Format(start) + '#' + formatter.Format(end) + '#' + scheduler.selection.resource;
        }}
        else if(scheduler.appointmentSelection.selectedAppointmentIds.length > 0) {{
            parameters += scheduler.appointmentSelection.selectedAppointmentIds[0];
        }}
        var confirmation = s.cpConfirmationMessages[itemName] || '';
        var usePostBack = s.cpUsePostBackFlags[itemName] || false;
        {4}
    }}
    else {{
        {5}(s, e);
    }}
}}", schedulerId, XafMenuCommandPrefix, NewObjectViewController.NewActionId, formatString, raiseCallbackString, defaultItemClickScript);
			return script;
		}
		private string GetSchedulerId(ASPxScheduler scheduler) {
			return string.IsNullOrEmpty(scheduler.ClientInstanceName) ? scheduler.ClientID : scheduler.ClientInstanceName;
		}
		private ActionBase FindActionByMenuItem(MenuItem menuItem) {
			foreach(WebContextMenuActionContainer container in containers) {
				foreach(ActionBase action in container.Actions) {
					if(GetMenuItemName(action) == menuItem.Name) {
						return action;
					}
				}
			}
			return null;
		}
		private ActionBase FindActionById(string actionId) {
			foreach(WebContextMenuActionContainer container in containers) {
				foreach(ActionBase action in container.Actions) {
					if(action.Id == actionId) {
						return action;
					}
				}
			}
			return null;
		}
		private void ProcessAction(ActionBase action) {
			if(action == null) {
				throw new ArgumentNullException("action");
			}
			if(action is SimpleAction) {
				ExecuteAction((SimpleAction)action);
			}
			else if(action is SingleChoiceAction) {
				ExecuteAction((SingleChoiceAction)action);
			}
			else if(action is PopupWindowShowAction && editor.SchedulerControl != null) {
				WebApplication.Instance.PopupWindowManager.ShowPopup((PopupWindowShowAction)action, editor.SchedulerControl.ClientID);
			}
		}
		private XafCallbackManager CallbackManager {
			get {
				Page page = editor != null && editor.SchedulerControl != null ? editor.SchedulerControl.Page : null;
				if(page != null) {
					Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), page.GetType(), "Page");
					return ((ICallbackManagerHolder)page).CallbackManager;
				}
				return null;
			}
		}
		private void CallbackManager_PartialRendering(object sender, PartialRenderingEventArgs e) {
			if(RenderUtils.GetReferentControl(e.Control, editor.SchedulerControl.ID, null) != null) {
				e.Cancel = true;
			}
		}
		protected void OnBoundItemCreating() {
			if(BoundItemCreating != null) {
				BoundItemCreating(this, new BoundItemCreatingEventArgs(null, null));
			}
		}
		protected void ClearUnusedDefaultMenuItems(ASPxSchedulerPopupMenu menu) {
			RemoveMenuItem(menu, "NewAppointment");
			RemoveMenuItem(menu, "NewAllDayEvent");
			RemoveMenuItem(menu, "NewRecurringAppointment");
			RemoveMenuItem(menu, "NewRecurringEvent");
		}
		protected void ClearUnusedAppointmentMenuItems(ASPxSchedulerPopupMenu menu) {
			RemoveMenuItem(menu, "OpenAppointment");
			RemoveMenuItem(menu, "EditSeries");
			RemoveMenuItem(menu, "RestoreOccurrence");
			RemoveMenuItem(menu, "DeleteAppointment");
		}
		protected void RemoveMenuItem(ASPxSchedulerPopupMenu menu, string menuItemName) {
			DevExpress.Web.MenuItem item = menu.Items.FindByName(menuItemName);
			if(item != null) {
				menu.Items.Remove(item);
			}
		}
		public ASPxSchedulerContextMenu(ASPxSchedulerListEditor editor) {
			this.editor = editor;
			editor.ControlsCreated += new EventHandler(editor_ControlsCreated);
		}
		public void CreateActionItems(IFrameTemplate parent, ListView context, ICollection<IActionContainer> contextContainers) {
			foreach(WebContextMenuActionContainer container in containers) {
				container.Dispose();
			}
			containers.Clear();
			foreach(IActionContainer sourceContainer in contextContainers) {
				WebContextMenuActionContainer container = new WebContextMenuActionContainer(sourceContainer.ContainerId);
				foreach(ActionBase action in sourceContainer.Actions) {
					if(action.Id == ListViewProcessCurrentObjectController.ListViewShowObjectActionId) {
						processSelectedItemAction = action;
					}
					container.Register(action);
				}
				containers.Add(container);
			}
		}
		public event EventHandler<BoundItemCreatingEventArgs> BoundItemCreating;
		public void ProcessAction(string actionId) {
			if(editor != null && processSelectedItemAction != null && actionId == processSelectedItemAction.Id) {
				ProcessAction(processSelectedItemAction);
			}
			else {
				ActionBase action = FindActionById(actionId);
				ProcessAction(action);
			}
		}
		public void ExecuteAction(SimpleAction action) {
			action.DoExecute();
		}
		public void ExecuteAction(SingleChoiceAction action) {
			action.DoExecute(action.Items.FirstActiveItem);
		}
		public void Dispose() {
			if(CallbackManager != null) {
				CallbackManager.PartialRendering -= new EventHandler<PartialRenderingEventArgs>(CallbackManager_PartialRendering);
			}
			if(editor != null) {
				editor.ControlsCreated -= new EventHandler(editor_ControlsCreated);
				editor = null;
			}
			foreach(WebContextMenuActionContainer container in containers) {
				container.Dispose();
			}
		}
		public string GetMenuUpdateScript() {
			if(appointmentMenu.Items.Count == 0) {
				return "";
			}
			StringBuilder result = new StringBuilder();
			foreach(MenuItem menuItem in appointmentMenu.Items) {
				ActionBase action = FindActionByMenuItem(menuItem);
				if(action != null) {
					result.AppendLine(JSUpdateScriptHelper.GetASPxMenuItemUpdateScript(appointmentMenuClientID, menuItem.Name, action.Enabled, action.Active));
				}
			}
			IDictionary<string, string> confirmationMessages = GetConfirmationMessages(appointmentMenu);
			result.AppendFormat("SetMenuProperty({0}, 'cpConfirmationMessages', {1});", appointmentMenuClientID, HtmlConvertor.ToJSON(confirmationMessages));
			return result.ToString();
		}
		#region IXafCallbackHandler Members
		void IXafCallbackHandler.ProcessAction(string parameter) {
			if(parameter == SelectionChangedCommandId) {
				editor.RaiseSelectionChanged();
				if(CallbackManager != null) {
					CallbackManager.PartialRendering += new EventHandler<PartialRenderingEventArgs>(CallbackManager_PartialRendering);
				}
			}
			else {
				parameter = parameter.Substring(XafMenuCommandPrefix.Length);
				string[] parts = parameter.Split(new char[] { ':' }, 2);
				if(parts[0] == NewObjectViewController.NewActionId) {
					string[] arguments = parts[1].Split('#');
					DateTime startDate = DateTime.ParseExact(arguments[0], CultureInfo.CurrentCulture.DateTimeFormat.FullDateTimePattern, null);
					DateTime endDate = DateTime.ParseExact(arguments[1], CultureInfo.CurrentCulture.DateTimeFormat.FullDateTimePattern, null);
					Appointment newAppointment = StaticAppointmentFactory.CreateAppointment(AppointmentType.Normal, startDate, endDate);
					string resourceId = arguments[2];
					Type resourceType = editor.GetResourceType();
					if(!string.IsNullOrEmpty(resourceId) && resourceId != "null" && resourceType != null) {
						Type keyType = editor.ObjectSpace.GetKeyPropertyType(resourceType);
						TypeConverter typeConverter = TypeDescriptor.GetConverter(keyType);
						newAppointment.ResourceIds.Add(typeConverter.ConvertFromString(resourceId));
					}
					editor.RaiseNewAction(newAppointment);
				}
				else {
					ProcessAction(parts[0]);
				}
			}
		}
		#endregion
#if DebugTest  //T158414
		public ASPxSchedulerPopupMenu DebugTest_AppointmentMenu {
			get { return appointmentMenu; }
			set { appointmentMenu = value; }
		}
#endif
	}
}
