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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using DevExpress.ExpressApp.Web.Utils;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.SystemModule {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ActionConfirmUnsavedChangesController : ProcessActionContainerHolderController {
		private List<string> menuItemsScriptsToProcess = new List<string>();
		protected override bool NeedSubscribeToHolderMenuItemsCreated(WebActionContainer webActionContainer) {
			return true;
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			if(Frame != null) {
				Active.SetItemValue(WebConfirmUnsavedChangesDetailViewController.ConfirmUnsavedChanges, ((IModelOptionsWeb)Frame.Application.Model.Options).ConfirmUnsavedChanges);
			}
		}
		private bool IsViewObjectSpaceOwner {
			get {
				return ObjectSpace.Owner == View;
			}
		}
		protected override void OnActionContainerHolderActionItemCreated(WebActionBaseItem item) {
			base.OnActionContainerHolderActionItemCreated(item);
			RegisterAction(item);
		}
		protected override void OnProcessActionContainer(WebActionContainer webActionContainer) {
			base.OnProcessActionContainer(webActionContainer);
			webActionContainer.Owner.MenuItemsCreated += ActionContainerHolder_MenuItemsCreated;
		}
		private void RegisterAction(WebActionBaseItem actionItem) {
			if(IsConfirmationRequired(actionItem)) {
				ASPxMenuBase menu = ((MenuActionItemBase)actionItem).MenuItem.Menu;
				string script = GetMenuItemScript(menu, actionItem, GetActionId(menu, actionItem));
				if(!string.IsNullOrEmpty(script)) {
					menuItemsScriptsToProcess.Add(script);
				}
			}
		}
		private void ActionContainerHolder_MenuItemsCreated(object sender, EventArgs e) {
			RegisterItemsScript(((ActionContainerHolder)sender).Menu);
		}
		protected void RegisterItemsScript(ASPxMenuBase menu) {
			if(menuItemsScriptsToProcess.Count > 0) {
				string script = @"function(s,e) {{" + string.Concat(menuItemsScriptsToProcess) + "}}"; ;
				if(!string.IsNullOrEmpty(menu.ClientSideEvents.Init)) {
					script = string.Format("function(s,e) {{ ({0})(s,e); ({1})(s,e);}}", menu.ClientSideEvents.Init, script);
				}
				menu.ClientSideEvents.Init = script;
				menuItemsScriptsToProcess.Clear();
			}
		}
		protected virtual bool IsConfirmationRequired(WebActionBaseItem actionItem) {
			bool result = IsViewObjectSpaceOwner ? actionItem.Action.Model.GetValue<bool>("ConfirmUnsavedChanges") : false;
			if(ConfirmationRequired != null) {
				ActionConfirmUnsavedChangesEventArgs args = new ActionConfirmUnsavedChangesEventArgs(actionItem.Action, result);
				ConfirmationRequired(this, args);
				result = args.ConfirmUnsavedChanges;
			}
			return result;
		}
		protected virtual string GetMenuItemScript(ASPxMenuBase menu, WebActionBaseItem actionItem, string itemId) {
			string script = null;
			if(!menu.ClientSideEvents.Init.Contains(string.Format("'{0}'", itemId))) {
				script = string.Format("xaf.ConfirmUnsavedChangedController.RegisterAction('{0}');", itemId);
			}
			return script;
		}
		protected virtual string GetActionId(ASPxMenuBase menu, WebActionBaseItem actionItem) {
			return menu.ClientID + "_" + actionItem.Action.Id;
		}
		public event EventHandler<ActionConfirmUnsavedChangesEventArgs> ConfirmationRequired;
#if DebugTest
		public void ForTests_RegisterAction(WebActionBaseItem actionItem) {
			RegisterAction(actionItem);
		}
#endif
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ASPxGridListEditorConfirmUnsavedChangesController : WebConfirmUnsavedChangesController<ListView> {
		private HashSet<string> confirmActions = new HashSet<string>();
		private ASPxGridListEditor listEditor;
		protected override void OnActivated() {
			base.OnActivated();
			ObjectSpace.ObjectReloaded += ObjectSpace_ObjectReloaded;
			Frame.ProcessActionContainer += new EventHandler<ProcessActionContainerEventArgs>(Frame_ProcessActionContainer);
		}
		protected override void OnDeactivated() {
			RegisterRemoveContainerScript();
			ObjectSpace.ObjectReloaded -= ObjectSpace_ObjectReloaded;
			Frame.ProcessActionContainer -= new EventHandler<ProcessActionContainerEventArgs>(Frame_ProcessActionContainer);
			if(listEditor != null && listEditor.Grid != null) {
				listEditor.Grid.Init -= gridView_Init;
				listEditor.Grid.BeforeGetCallbackResult -= gridView_BeforeGetCallbackResult;
				if(!IsViewObjectSpaceOwner) {
					listEditor.Grid.RowUpdated -= gridView_RowUpdated;
					listEditor.Grid.RowInserting -= Grid_RowInserting;
				}
			}
			confirmActions.Clear();
			base.OnDeactivated();
		}
		private void RegisterRemoveContainerScript() {
			RegisterClientScript(ViewGuid + "RemoveHelper", string.Format(@"
                            var viewContainersMap = xaf.ConfirmUnsavedChangedController.ViewContainersMap;
                            if(viewContainersMap['{0}'] != undefined){{
                                for (var iterator = 0; iterator < viewContainersMap['{0}'].length; iterator++) {{
                                    var containerId = viewContainersMap['{0}'][iterator];
                                    delete ControlUpdateWatcher.getInstance().helpers['xaf_' + containerId];
                                }}
                            }}   
                             ", ViewGuid));
			RegisterClientScript(ViewGuid + "RemoveContainer", string.Format("xaf.ConfirmUnsavedChangedController.RemoveContainer('{0}');", ViewGuid));
		}
		protected override void FrameTemplateChanged() {
			confirmActions.Clear();
		}
		private void Frame_ProcessActionContainer(object sender, ProcessActionContainerEventArgs e) {
			ContextActionsMenuContainer contextActionsMenuContainer = e.ActionContainer as ContextActionsMenuContainer;
			if(contextActionsMenuContainer != null) {
				RegisterActions(contextActionsMenuContainer);
			}
		}
		private void RegisterActions(ContextActionsMenuContainer contextActionsMenuContainer) {
			foreach(ActionBase action in contextActionsMenuContainer.Actions) {
				if(IsConfirmationRequired(action)) {
					confirmActions.Add(action.Category + "_" + action.Id);
				}
			};
		}
		protected virtual bool IsConfirmationRequired(ActionBase action) {
			bool result = action.Model.GetValue<bool>("ConfirmUnsavedChanges");
			if(ConfirmationRequired != null) {
				ActionConfirmUnsavedChangesEventArgs args = new ActionConfirmUnsavedChangesEventArgs(action, result);
				ConfirmationRequired(this, args);
				result = args.ConfirmUnsavedChanges;
			}
			return result;
		}
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			if(View.Editor is ISupportInplaceEdit) {
				listEditor = View.Editor as ASPxGridListEditor;
				if(listEditor != null && listEditor.Grid != null) {
					listEditor.Grid.Init += gridView_Init;
					listEditor.Grid.BeforeGetCallbackResult += gridView_BeforeGetCallbackResult;
					listEditor.Grid.CustomJSProperties += gridView_CustomJSProperties;
					if(!IsViewObjectSpaceOwner) {
						listEditor.Grid.RowUpdated += gridView_RowUpdated;
						listEditor.Grid.RowInserting += Grid_RowInserting;
					}
				}
			}
		}
		void Grid_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e) {
			SetModified(false);
			((ASPxGridView)sender).JSProperties["cpIsModifiedMainContainer"] = true;
			RegisterModifiedScript();		   
		}
		void gridView_RowUpdated(object sender, DevExpress.Web.Data.ASPxDataUpdatedEventArgs e) {
			SetModified(false);
			((ASPxGridView)sender).JSProperties["cpIsModifiedMainContainer"] = true;
			RegisterModifiedScript();
		}
		void gridView_Init(object sender, EventArgs e) {
			ASPxGridView gridView = sender as ASPxGridView;
			bool isBatchEditMode = gridView.SettingsEditing.Mode == GridViewEditingMode.Batch;
			string batchEditGridRegistrationScript = "";
			if(isBatchEditMode) {
				batchEditGridRegistrationScript = "xaf.ConfirmUnsavedChangedController.RegisterBatchEditGrid(s);";
			}
			gridView.ClientSideEvents.Init = string.Format(@"
                            function(s, e){{
                                var confirmActions = s.cpConfirmActions;
                                s.XafGridViewUpdateWatcherHelper = xaf.ConfirmUnsavedChangedController.XafGridViewUpdateWatcherHelper(s, confirmActions, {0});
                                xaf.ConfirmUnsavedChangedController.AddContainer('{2}', s.name); 
                                {1}
                            }}", ClientSideEventsHelper.ToJSBoolean(isBatchEditMode), batchEditGridRegistrationScript, ViewGuid);
			if(!string.IsNullOrEmpty((string)gridView.JSProperties[ASPxGridListEditor.EndCallbackHandlers])) {
				gridView.JSProperties[ASPxGridListEditor.EndCallbackHandlers] += ";cpEndCallbackHandlerConfirmScript";
			}
		}
		void gridView_CustomJSProperties(object sender, ASPxGridViewClientJSPropertiesEventArgs e) {
			ASPxGridView gridView = ((ASPxGridView)sender);
			gridView.JSProperties.Add("cpEndCallbackHandlerConfirmScript", string.Format(@"     
                                if(s.XafGridViewUpdateWatcherHelper.isBatchEdit){{
                                    s.batchEditClientState.confirmUpdate = xaf.ConfirmUnsavedChangedController.questionMessage;
                                }}            
                                if(s.cpIsModified != undefined && !s.XafGridViewUpdateWatcherHelper.isBatchEdit){{
                                    xaf.ConfirmUnsavedChangedController.SetModified(s.cpIsModified, '{0}');
                                    s.cpIsModified = undefined;                                    
                                }}
                                if(s.cpIsModifiedMainContainer != undefined){{
                                    xaf.ConfirmUnsavedChangedController.SetModified(s.cpIsModifiedMainContainer);
                                    s.cpIsModifiedMainContainer = undefined;                                    
                                }}
                                ClientParams.Set('CancelEditGrid', false);", gridView.ClientID));
			gridView.JSProperties["cpConfirmActions"] = confirmActions;
			gridView.CustomJSProperties -= gridView_CustomJSProperties;
		}
		protected override void RegisterModifiedScript() {
			if(View != null) {
				if(View.Editor is ISupportInplaceEdit && listEditor != null && listEditor.Grid != null) {
					listEditor.Grid.JSProperties["cpIsModified"] = IsModified;
				}
				base.RegisterModifiedScript();
			}
		}
		private void gridView_BeforeGetCallbackResult(object sender, EventArgs e) {
			bool? endEdit = WebApplication.Instance.ClientInfo.GetValue<bool?>("CancelEditGrid");
			if(endEdit.HasValue && endEdit.Value) {
				((ASPxGridView)sender).CancelEdit();
			}
		}
		protected override void ObjectSpaceCommitted() {
			base.ObjectSpaceCommitted();
			RegisterModifiedScript();
		}
		private void ObjectSpace_ObjectReloaded(object sender, ObjectManipulatingEventArgs e) {
			SetModified(false);
			RegisterModifiedScript();
		}
		public event EventHandler<ActionConfirmUnsavedChangesEventArgs> ConfirmationRequired;
#if DebugTest
		public ReadOnlyCollection<string> ConfirmActions {
			get {
				return confirmActions.ToList().AsReadOnly();
			}
		}
		public ASPxGridListEditor ForTests_ListEditor {
			get {
				return listEditor;
			}
			set {
				listEditor = value;
			}
		}
#endif
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class WebConfirmUnsavedChangesDetailViewController : WebConfirmUnsavedChangesController<DetailView> {
		protected override void OnDeactivated() {
			base.OnDeactivated();
			RegisterRemoveMainContainerScript();
		}
		protected override void OnViewChanged() {
			base.OnViewChanged();
			if(View != null && View.ViewEditMode == ViewEditMode.View) {
				UnsubscribeChangesEvents();
				View.ViewEditModeChanged += View_ViewEditModeChanged;
			}
		}
		protected override void OnViewChanging(View view) {
			base.OnViewChanging(view);
			if(View != null) {
				View.ViewEditModeChanged -= View_ViewEditModeChanged;
			}
		}
		private void View_ViewEditModeChanged(object sender, EventArgs e) {
			if(View.ViewEditMode == ViewEditMode.Edit && Active) {
				UnsubscribeChangesEvents();
				SubscribeChangesEvents();
			}
			else {
				UnsubscribeChangesEvents();
			}
		}
		protected override void SubscribeChangesEvents() {
			if(View.ViewEditMode == ViewEditMode.Edit) {
				base.SubscribeChangesEvents();
			}
		}
		protected override void RegisterScripts() {
			if(View.ViewEditMode == ViewEditMode.Edit) {
				RegisterAddMainContainerScript();
			}
			else {
				RegisterRemoveMainContainerScript();
			}
			base.RegisterScripts();
		}
		protected override void RegisterModifiedScript() {
			if(View.ViewEditMode == ViewEditMode.Edit) {
				base.RegisterModifiedScript();
			}
		}
		private void RegisterAddMainContainerScript() {
			RegisterClientScript("AddMainContainer", string.Format("xaf.ConfirmUnsavedChangedController.AddContainer('{0}', xaf.ConfirmUnsavedChangedController.mainContainerId);", ViewGuid));
		}
		private void RegisterRemoveMainContainerScript() {
			RegisterClientScript("RemoveMainContainer", string.Format("xaf.ConfirmUnsavedChangedController.RemoveContainer('{0}');", ViewGuid));
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class WebConfirmUnsavedChangesController<ViewType> : ViewController<ViewType> where ViewType : View {
		public const string ConfirmUnsavedChanges = "ConfirmUnsavedChanges";
		protected string ViewGuid = Guid.NewGuid().ToString();
		private bool needRegisterDropModifiedScript = false;
		private bool isModified = false;
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			if(Frame != null) {
				Active.SetItemValue(ConfirmUnsavedChanges, ((IModelOptionsWeb)Frame.Application.Model.Options).ConfirmUnsavedChanges);
			}
		}
		protected override void OnViewChanged() {
			base.OnViewChanged();
			if(View != null && IsViewObjectSpaceOwner) {
				needRegisterDropModifiedScript = true;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			OnActivatedCore();
		}
		private void OnActivatedCore() {
			Frame.TemplateChanged += Frame_TemplateChanged;
			SubscribePagePreRender();
			SubscribeCallBackManagerPreRender();
			if(IsViewObjectSpaceOwner) {
				SubscribeChangesEvents();
			}
		}
		void CallbackManager_ProcessPreRender(object sender, EventArgs e) {
			if(Frame.Template != null) {
				RegisterScripts();
			}
		}
		void ProcessPreRenderCompleted(object sender, EventArgs e) {
			RegisterScripts();
		}
		protected virtual void RegisterScripts() {
			if(IsViewObjectSpaceOwner) {
				RegisterActivateScript(Active);
				RegisterModifiedScript();			
			}
		}
		protected override void OnDeactivated() {
			Frame.TemplateChanged -= Frame_TemplateChanged;
			UnsubscribePagePreRender();
			UnsubscribeCallBackManagerPreRender();
			UnsubscribeChangesEvents();
			base.OnDeactivated();
		}
		protected virtual void SubscribeChangesEvents() {
			ObjectSpace.Committed += ObjectSpace_Committed;
			ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
			ObjectSpace.Reloaded += ObjectSpace_Reloaded;
		}
		protected virtual void UnsubscribeChangesEvents() {
			ObjectSpace.Committed -= ObjectSpace_Committed;
			ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
			ObjectSpace.Reloaded -= ObjectSpace_Reloaded;
		}
		private void SubscribeCallBackManagerPreRender() {
			Page page = WebWindow.CurrentRequestPage;
			if(page != null) {
				ICallbackManagerHolder holder = page as ICallbackManagerHolder;
				if(holder != null) {
					holder.CallbackManager.ProcessPreRender += CallbackManager_ProcessPreRender;
				}
			}
		}
		private void SubscribePagePreRender() {
			WebWindow window = this.Frame as WebWindow;
			if(window != null) {
				window.ProcessPreRenderCompleted += ProcessPreRenderCompleted;
			}
		}
		private void UnsubscribeCallBackManagerPreRender() {
			Page page = WebWindow.CurrentRequestPage;
			if(page != null) {
				ICallbackManagerHolder holder = page as ICallbackManagerHolder;
				if(holder != null) {
					holder.CallbackManager.ProcessPreRender -= CallbackManager_ProcessPreRender;
				}
			}
		}
		private void UnsubscribePagePreRender() {
			WebWindow window = this.Frame as WebWindow;
			if(window != null) {
				window.ProcessPreRenderCompleted -= ProcessPreRenderCompleted;
			}
		}
		private void Frame_TemplateChanged(object sender, EventArgs e) {
			FrameTemplateChanged();
		}
		protected virtual void FrameTemplateChanged() {
			if(Frame.Template != null) {
				UnsubscribePagePreRender();
				UnsubscribeCallBackManagerPreRender();
				SubscribePagePreRender();
				SubscribeCallBackManagerPreRender();
			}
		}
		private void ObjectSpace_Reloaded(object sender, EventArgs e) {
			needRegisterDropModifiedScript = true;
			SetModified(false);
		}
		private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			SetModified(true);
		}
		private void ObjectSpace_Committed(object sender, EventArgs e) {
			ObjectSpaceCommitted();
		}
		protected virtual void ObjectSpaceCommitted() {
			SetModified(false);
		}
		protected virtual void RegisterActivateScript(bool clientControllerActivate) {
			if(IsViewObjectSpaceOwner) {
				RegisterClientScript("ActiveStateConfirmMessage", string.Format("xaf.ConfirmUnsavedChangedController.SetActive({0});", ClientSideEventsHelper.ToJSBoolean(clientControllerActivate)));
				if(clientControllerActivate) {
					RegisterClientScript("SetConfirmMessage", string.Format("xaf.ConfirmUnsavedChangedController.SetMessage('{0}');", CaptionHelper.GetLocalizedText(XafApplication.Confirmations, "Cancel")));
				}
				if(needRegisterDropModifiedScript) {
					needRegisterDropModifiedScript = false;
					SetModified(false);
				}
			}
		}
		protected virtual void RegisterModifiedScript() {
			RegisterClientScript(ViewGuid + "SetModified", string.Format("xaf.ConfirmUnsavedChangedController.SetModified({0}, xaf.ConfirmUnsavedChangedController.mainContainerId);", ClientSideEventsHelper.ToJSBoolean(isModified)));
		}
		protected virtual void SetModified(bool onClientModified) {
			isModified = onClientModified;
		}
		public bool IsModified {
			get {
				return isModified;
			}
		}
		protected bool IsViewObjectSpaceOwner {
			get {
				return ObjectSpace.Owner == View;
			}
		}
		protected virtual void RegisterClientScript(string id, string script) {
			Page page = WebWindow.CurrentRequestPage;
			if(page != null) {
				if(page.IsCallback) {
					ICallbackManagerHolder holder = page as ICallbackManagerHolder;
					if(holder != null) {
						holder.CallbackManager.RegisterClientScript(id, script, true);
					}
				}
				else {
					page.ClientScript.RegisterClientScriptBlock(GetType(), id, script, true);
				}
			}
		}
#if DebugTest
		public bool ForTests_GetNeedRegisterDropModifiedScript() {
			return needRegisterDropModifiedScript;
		}
		public string ForTests_ViewGuID {
			get {
				return ViewGuid;
			}
		}
#endif
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ActionConfirmUnsavedChangesEventArgs : EventArgs {
		private ActionBase action;
		private bool confirmUnsavedChanges;
		public ActionConfirmUnsavedChangesEventArgs(ActionBase action, bool confirmUnsavedChanges) {
			this.action = action;
			this.confirmUnsavedChanges = confirmUnsavedChanges;
		}
		public ActionBase Action {
			get { return action; }
		}
		public bool ConfirmUnsavedChanges {
			get { return confirmUnsavedChanges; }
			set { confirmUnsavedChanges = value; }
		}
	}
}
