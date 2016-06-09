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
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.SystemModule;
namespace DevExpress.ExpressApp.Web.Editors {
	public class WebListEditorCommand {
		public const string XafCommandPrefix = "XafAction";
		public const string XafCommandTemplate = XafCommandPrefix + ":{0}:{1}";
		private string commandString;
		private string commandId;
		private string objectId;
		private bool encodeHtml = true;
		private void ParseCommandString() {
			if(commandString.StartsWith(XafCommandPrefix)) {
				string arguments = commandString.Substring(XafCommandPrefix.Length + 1);
				string[] argsList = arguments.Split(':');
				commandId = System.Web.HttpUtility.UrlDecode(argsList[0]);
				objectId = System.Web.HttpUtility.UrlDecode(argsList[1]);
			}
			else {
				commandId = string.Empty;
				objectId = string.Empty;
			}
		}
		private void UpdateCommandString() {
			if(encodeHtml) {
				commandString = string.Format(XafCommandTemplate, System.Web.HttpUtility.UrlEncode(commandId), System.Web.HttpUtility.UrlEncode(objectId));
			}
			else {
				commandString = string.Format(XafCommandTemplate, commandId, objectId);
			}
		}
		public WebListEditorCommand(string commandString) {
			this.commandString = commandString;
			ParseCommandString();
		}
		public WebListEditorCommand(string commandId, string objectId)
			: this(commandId, objectId, true) {
		}
		public WebListEditorCommand(string commandId, string objectId, bool encodeHtml) {
			this.commandId = commandId;
			this.objectId = objectId;
			this.encodeHtml = encodeHtml;
			UpdateCommandString();
		}
		public string CommandString {
			get { return commandString; }
			set {
				commandId = value;
				ParseCommandString();
			}
		}
		public string CommandId {
			get { return commandId; }
			set { 
				commandId = value;
				UpdateCommandString();
			}
		}
		public string ObjectId {
			get { return objectId; }
			set {
				objectId = value;
				UpdateCommandString();
			}
		}
		public bool IsEmpty {
			get {
				return string.IsNullOrEmpty(commandId) && string.IsNullOrEmpty(objectId);
			}
		}
	}
	public abstract class ListEditorContextMenuBase<PopupMenuContainerType> : IContextMenuTemplate 
		where PopupMenuContainerType : IActionContainer, new() {
		private static string[] contextRequiredKeys = new string[] { ActionBase.RequireSingleObjectContext, ActionBase.RequireMultipleObjectsContext, ActionsCriteriaViewController.EnabledByCriteriaKey };
		private List<PopupMenuContainerType> containers = new List<PopupMenuContainerType>();
		private ListEditor editor;
		private ActionBase processSelectedItemAction;
		protected bool IsFalseByContextRequiredOrCriteria(BoolList checkedList) {
			bool result = !checkedList;
			if(result) {
				foreach(string activeKey in checkedList.GetKeys()) {
					if(!checkedList[activeKey] && (Array.IndexOf<string>(contextRequiredKeys, activeKey) == -1)) {
						result = false;
						break;
					}
				}
			}
			return result;
		}
		protected virtual void ProcessAction(ActionBase action) {
			if(action == null) {
				throw new ArgumentNullException("action");
			}
			if(action is SimpleAction) {
				ExecuteAction((SimpleAction)action);
			}
			else if(action is PopupWindowShowAction) {
				WebApplication.Instance.PopupWindowManager.ShowPopup((PopupWindowShowAction)action, ((WebControl)Editor.Control).ClientID);
			}
		}
		private void ExecuteAction(SimpleAction action) {
			action.DoExecute();
		}
		private ActionBase FindActionById(string actionId) {
			foreach(IActionContainer container in Containers) {
				foreach(ActionBase action in container.Actions) {
					if(action.Id == actionId) {
						return action;
					}
				}
			}
			return null;
		}
		protected IEnumerable<PopupMenuContainerType> Containers {
			get {
				return containers;
			}
		}
		protected bool IsActionVisibleInContextMenu(ActionBase action) {
			return IsActionVisibleInContextMenu(ProcessSelectedItemAction, action);
		}		
		protected ActionBase FindInlineEditAction() {
			return FindActionById(ListViewController.InlineEditActionId);
		}
		protected void ProcessAction(string actionId) {
			ActionBase action = FindActionById(actionId);
			ProcessAction(action);
		}
		protected virtual bool IsActionVisibleInContextMenu(ActionBase defaultAction, ActionBase action) {
			if((action != null) &&
				(action.Active || IsFalseByContextRequiredOrCriteria(action.Active)) &&
				defaultAction != action &&
				(
				action.SelectionDependencyType == SelectionDependencyType.RequireSingleObject ||
				(action.SelectionDependencyType == SelectionDependencyType.RequireMultipleObjects && (editor.SelectionType & SelectionType.TemporarySelection) == SelectionType.TemporarySelection))
				) {
				return true;
			}
			return false;
		}
		protected virtual bool GetRowActionActivity(ActionBase action, object rowObject) {
			bool isActive = false;
			if(action != null) {
				BoundItemCreatingEventArgs args = new BoundItemCreatingEventArgs(rowObject, action);
				args.Enabled = (action.Active || IsFalseByContextRequiredOrCriteria(action.Active)) && (action.Enabled || IsFalseByContextRequiredOrCriteria(action.Enabled));
				if(BoundItemCreating != null) {
					BoundItemCreating(this, args);
				}
				isActive = args.Enabled;
			}
			return isActive;
		}
		protected virtual void RecreateControls() {
			if((Editor != null) && (Editor.Control != null)) {
				CreateControls();
			}
		}
		public void CreateControls() {
			CreateControlsCore();
			OnControlsCreated();
		}
		protected virtual void OnControlsCreated() {
			if(ControlsCreated != null) {
				ControlsCreated(this, EventArgs.Empty);
			}
		}
		public abstract void CreateControlsCore();
		protected ListEditor Editor {
			get { return editor; }
		}
		public ListEditorContextMenuBase(ListEditor editor) {
			this.editor = editor;
		}
		public virtual void CreateActionItems(IFrameTemplate parent, ListView context, ICollection<IActionContainer> contextContainers) {
			foreach(PopupMenuContainerType container in containers) {
				container.Dispose();
			}
			containers.Clear();
			foreach(IActionContainer sourceContainer in contextContainers) {
				PopupMenuContainerType container = new PopupMenuContainerType();
				container.ContainerId = sourceContainer.ContainerId;
				foreach(ActionBase action in sourceContainer.Actions) {
					if(action.Id == ListViewProcessCurrentObjectController.ListViewShowObjectActionId && processSelectedItemAction == null) {
						processSelectedItemAction = action;
					}
					ContextMenuCustomRegisterActionEventArgs args = new ContextMenuCustomRegisterActionEventArgs(action, container);
					if(CustomRegisterAction != null) {
						CustomRegisterAction(this, args);
					}
					if(!args.Handled) {
						container.Register(action);
					}
				}
				containers.Add(container);
			}
			RecreateControls();
		}
		public ActionBase ProcessSelectedItemAction {
			get { return processSelectedItemAction; }
			set { processSelectedItemAction = value; }
		}
		public virtual void Dispose() {
			editor = null;
		}
		public virtual void BreakLinksToControls() { }
		public event EventHandler<BoundItemCreatingEventArgs> BoundItemCreating;
		public event EventHandler ControlsCreated;
		public event EventHandler<ContextMenuCustomRegisterActionEventArgs> CustomRegisterAction; 
	}
	public class ContextMenuCustomRegisterActionEventArgs : HandledEventArgs {
		public ContextMenuCustomRegisterActionEventArgs(ActionBase action, IActionContainer container) {
			this.Action = action;
			this.Container = container;
		}
		public ActionBase Action { get; private set; }
		public IActionContainer Container { get; private set; }
	}
}
