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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Web.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxLookupPropertyEditor : ASPxObjectPropertyEditorBase, IDependentPropertyEditor, ITestable, ISupportViewShowing, IFrameContainer {
		private static int windowWidth = 800;
		private static int windowHeight = 480;
		private PopupWindowShowAction showFindSelectWindowAction;
		private PopupWindowShowAction newObjectWindowAction;
		private WebLookupEditorHelper helper;
		private object newObject;
		private IObjectSpace newObjectSpace;
		private List<IObjectSpace> createdObjectSpaces = new List<IObjectSpace>();
		internal NestedFrame frame;
		private ListView listView;
		private NewObjectViewController newObjectViewController;
		private ASPxLookupDropDownEdit dropDownEdit;
		private ASPxLookupFindEdit findEdit;
		private string editorId;
		private WebControl ActiveControl {
			get {
				if(dropDownEdit != null && dropDownEdit.Visible) {
					return dropDownEdit;
				}
				else {
					return findEdit;
				}
			}
		}
		private WebControl InactiveControl {
			get {
				if(dropDownEdit != null && !dropDownEdit.Visible) {
					return dropDownEdit;
				}
				else {
					return findEdit;
				}
			}
		}
		private Boolean GetAllowClear() {
			return Model.AllowClear;
		}
		private ASPxLookupFindEdit CreateFindLookupControl() {
			if(showFindSelectWindowAction == null) {
				showFindSelectWindowAction = new PopupWindowShowAction(null, MemberInfo.Name + "_ASPxLookupEditor_ShowFindWindow", PredefinedCategory.Unspecified);
				showFindSelectWindowAction.Execute += new PopupWindowShowActionExecuteEventHandler(actionFind_OnExecute);
				showFindSelectWindowAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(action_FindWindowParamsCustomizing);
				showFindSelectWindowAction.Application = application;
				showFindSelectWindowAction.AcceptButtonCaption = "";
			}
			ASPxLookupFindEdit result = new ASPxLookupFindEdit();
			result.Width = Unit.Percentage(100);
			result.ReadOnly = !AllowEdit;
			result.ClearButton.Visible = GetAllowClear();
			result.ValueChanged += new EventHandler(findLookup_ValueChanged);
			result.Init += new EventHandler(findLookup_Init);
			result.Callback += new EventHandler<CallbackEventArgs>(findLookup_Callback);
			return result;
		}
		string SetImmediatePostDataScript_SelectedIndexChangedKey = Guid.NewGuid().ToString();
		string SetImmediatePostDataScript_TextChangedKey = Guid.NewGuid().ToString();
		protected override void SetImmediatePostDataScript(string script) {
			ClientSideEventsHelper.AssignClientHandlerSafe(dropDownEdit.DropDown, "SelectedIndexChanged", script, SetImmediatePostDataScript_SelectedIndexChangedKey);
			ClientSideEventsHelper.AssignClientHandlerSafe(findEdit.Editor, "TextChanged", script, SetImmediatePostDataScript_TextChangedKey);
		}
		string SetImmediatePostDataCompanionScriptKey = Guid.NewGuid().ToString();
		protected override void SetImmediatePostDataCompanionScript(string script) {
			ClientSideEventsHelper.AssignClientHandlerSafe(dropDownEdit.DropDown, "GotFocus", script, SetImmediatePostDataCompanionScriptKey);
		}
		protected override void SetProcessValueChangedScript(string script) {
			if(dropDownEdit != null) {
				ClientSideEventsHelper.AssignClientHandlerSafe(dropDownEdit.DropDown, "SelectedIndexChanged", script, Guid.NewGuid().ToString());
			}
		}
		private void findLookup_Init(object sender, EventArgs e) {
			ASPxLookupFindEdit editor = ((ASPxLookupFindEdit)sender);
			editor.Init -= new EventHandler(findLookup_Init);
			if(application != null) {
				UpdateFindButtonScript(editor, application.PopupWindowManager);
			}
		}
		private void UpdateFindButtonScript(ASPxLookupFindEdit findEdit, PopupWindowManager popupWindowManager) {
			if(findEdit != null && popupWindowManager != null) {
				ICallbackManagerHolder callbackManagerHolder = WebWindow.CurrentRequestPage as ICallbackManagerHolder;
				string immediatePostDataScript = Model.ImmediatePostData && callbackManagerHolder != null ? callbackManagerHolder.CallbackManager.GetScript().Replace("'", @"\\\\\\""") : "";
				string processFindResultFunc = "xafFindLookupProcessFindObject('" + findEdit.UniqueID + "', '" + findEdit.Hidden.ClientID + "', window.findLookupResult, '" + immediatePostDataScript + "');";
				findEdit.SetFindButtonClientScript(popupWindowManager.GetShowPopupWindowScript(showFindSelectWindowAction, HttpUtility.JavaScriptStringEncode(processFindResultFunc), findEdit.ClientID, false, true, true, true, "function() { window.buttonEditAlreadyClicked = false; document.isMenuClicked = false;}"));
			}
		}
		private void UpdateDropDownLookupControlAddButton(ASPxLookupDropDownEdit control) {
			control.AddingEnabled = false;
			if(CurrentObject != null) {
				string diagnosticInfo = "";
				RecreateListView(true);
				control.AddingEnabled = AllowEdit && DataManipulationRight.CanCreate(listView, helper.LookupObjectType, listView.CollectionSource, out diagnosticInfo);
				if(control.AddingEnabled) {
					if(newObjectViewController != null) {
						control.AddingEnabled = newObjectViewController.NewObjectAction.Active && newObjectViewController.NewObjectAction.Enabled;
					}
				}
			}
		}
		string CreateDropDownLookupControlKey = Guid.NewGuid().ToString();
		private ASPxLookupDropDownEdit CreateDropDownLookupControl() {
			ASPxLookupDropDownEdit result = new ASPxLookupDropDownEdit();
			result.Width = Unit.Percentage(100);
			ClientSideEventsHelper.AssignClientHandlerSafe(result.DropDown, "SelectedIndexChanged", "function(sender, args) {{ args.processOnServer = false; }}", CreateDropDownLookupControlKey);
			result.DropDown.SelectedIndexChanged += new EventHandler(dropDownLookup_SelectedIndexChanged);
			UpdateDropDownLookup(result);
			result.ReadOnly = !AllowEdit;
			result.ClearingEnabled = GetAllowClear();
			result.Init += new EventHandler(dropDownLookup_Init);
			result.PreRender += new EventHandler(dropDownLookup_PreRender);
			result.Callback += new EventHandler<CallbackEventArgs>(dropDownLookup_Callback);
			return result;
		}
		private void UpdateDropDownLookup(ASPxLookupDropDownEdit editor) {
			if(!UseFindEdit()) {
				if(newObjectViewController != null) {
					editor.NewActionCaption = newObjectViewController.NewObjectAction.Caption;
				}
				UpdateDropDownLookupControlAddButton(editor);
				if(application != null) {
					editor.SetClientNewButtonScript(application.PopupWindowManager.GetShowPopupWindowScript(newObjectWindowAction, HttpUtility.JavaScriptStringEncode(editor.GetProcessNewObjFunction()), editor.ClientID, false, newObjectWindowAction.IsSizeable, false, false, "function() { window.buttonEditAlreadyClicked = false; document.isMenuClicked = false;}"));
				}
			}
		}
		private void findLookup_Callback(object sender, CallbackEventArgs e) {
			if(e.Argument == "clear") {
				findEdit.Text = helper.GetDisplayText(null, EmptyValue, DisplayFormat);
			}
			else if(e.Argument.StartsWith("found")) {
				findEdit.Text = helper.GetDisplayText(GetObjectByKey(e.Argument.Substring(5)), EmptyValue, DisplayFormat);
			}
		}
		private void findLookup_ValueChanged(object sender, EventArgs e) {
			EditValueChangedHandler(sender, EventArgs.Empty);
		}
		private void dropDownLookup_Callback(object sender, CallbackEventArgs e) {
			if(!string.IsNullOrEmpty(e.Argument)) {
				FillEditorValues(GetObjectByKey(e.Argument), dropDownEdit.DropDown);
			}
		}
		private void dropDownLookup_SelectedIndexChanged(object source, EventArgs e) {
			if(!((ASPxComboBox)source).IsCallback) {
				EditValueChangedHandler(source, EventArgs.Empty);
			}
		}
		private void dropDownLookup_Init(object sender, EventArgs e) {
			UpdateDropDownLookup((ASPxLookupDropDownEdit)sender);
		}
		private void dropDownLookup_PreRender(object sender, EventArgs e) {
			UpdateDropDownLookup((ASPxLookupDropDownEdit)sender);
		}
		private object GetObjectByKey(string key) {
			return helper.GetObjectByKey(CurrentObject, key);
		}
		private void FillEditorValues(object currentSelectedObject, ASPxComboBox control) {
			ArrayList list = GetObjectsList(currentSelectedObject);
			CreateControlItems(currentSelectedObject, list, control, helper);
		}
		protected virtual ArrayList GetObjectsList(object currentSelectedObject) {
			ArrayList result = new ArrayList();
			if(CurrentObject != null) {
				IList sourceList = GetDataSourceList();
				if(sourceList != null) {
					for(int i = 0; i < sourceList.Count; i++) {
						result.Add(sourceList[i]);
					}
				}
				if(currentSelectedObject != null && !result.Contains(currentSelectedObject)) {
					result.Add(currentSelectedObject);
				}
				ApplySorting(result);
			}
			return result;
		}
		protected virtual IList GetDataSourceList() {
			EnsureDataSource();
			return DataSource.List;
		}
		private void EnsureDataSource() {
			if(DataSource != null) {
				helper.ReloadCollectionSource(DataSource, CurrentObject);
			}
			RecreateListView(true);
			if(DataSource == null) {
				throw new ArgumentNullException("DataSource");
			}
		}
		protected virtual void CreateControlItems(object currentSelectedObject, ArrayList list, ASPxComboBox control, WebLookupEditorHelper helper) {
			control.Items.Clear();
			int shift = 0;
			if(GetAllowClear()) {
				control.Items.Add(HttpUtility.HtmlEncode(WebPropertyEditor.EmptyValue), null);
				shift = 1;
			}
			foreach(object obj in list) {
				control.Items.Add(helper.GetEscapedDisplayText(obj, EmptyValue, DisplayFormat, objectSpace), helper.GetObjectKey(obj));
			}
			int selectedIndex = 0;
			int indexInList = list.IndexOf(currentSelectedObject);
			if(indexInList != -1) {
				selectedIndex = indexInList + shift;
			}
			control.SelectedIndex = selectedIndex;
		}
		protected virtual void ApplySorting(ArrayList list) {
			if(helper.LookupListViewModel.Sorting.Count == 0) {
				list.Sort(new DisplayValueComparer(helper, EmptyValue, objectSpace));
			}
		}
		private string EscapeObjectKey(string key) {
			return key.Replace("'", "\\'");
		}
		private void actionFind_OnExecute(Object sender, PopupWindowShowActionExecuteEventArgs args) {
			string objectKey = helper.GetObjectKey(((ListView)args.PopupWindow.View).CurrentObject);
			((PopupWindow)args.PopupWindow).ClosureScript = "if(window.dialogOpener) window.dialogOpener.findLookupResult = '" + EscapeObjectKey(objectKey) + "';";
		}
		private void action_FindWindowParamsCustomizing(Object sender, CustomizePopupWindowParamsEventArgs args) {
			OnViewShowingNotification();
			args.View = helper.CreateListView(CurrentObject);
			FindLookupDialogController controller = helper.Application.CreateController<FindLookupDialogController>();
			controller.Initialize(helper);
			args.DialogController = controller;
		}
		private void newObjectViewController_ObjectCreated(object sender, DevExpress.ExpressApp.SystemModule.ObjectCreatedEventArgs e) {
			newObject = e.CreatedObject;
			newObjectSpace = e.ObjectSpace;
			createdObjectSpaces.Add(newObjectSpace);
		}
		private void newObjectViewController_ObjectCreating(object sender, ObjectCreatingEventArgs e) {
			e.ShowDetailView = false;
			if(e.ObjectSpace is INestedObjectSpace) {
				e.ObjectSpace = application.CreateObjectSpace(e.ObjectType);
			}
		}
		private void newObjectWindowAction_OnCustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs args) {
			if(!this.DataSource.AllowAdd) {
				throw new InvalidOperationException();
			}
			if(newObjectViewController != null) {
				OnViewShowingNotification();
				newObjectViewController.NewObjectAction.DoExecute(newObjectViewController.NewObjectAction.Items[0]);
				args.View = application.CreateDetailView(newObjectSpace, newObject, listView);
			}
		}
		private void newObjectWindowAction_OnExecute(Object sender, PopupWindowShowActionExecuteEventArgs args) {
			if(!this.DataSource.AllowAdd) {
				throw new InvalidOperationException();
			}
			if(objectSpace != args.PopupWindow.View.ObjectSpace) {
				args.PopupWindow.View.ObjectSpace.CommitChanges();
			}
			DataSource.Add(helper.ObjectSpace.GetObject(((DetailView)args.PopupWindow.View).CurrentObject));
			((PopupWindow)args.PopupWindow).ClosureScript = "if(window.dialogOpener) window.dialogOpener.ddLookupResult = '" + helper.GetObjectKey(((DetailView)args.PopupWindow.View).CurrentObject) + "';";
		}
		private void RecreateListView(bool ifNotCreatedOnly) {
			if(ViewEditMode == ViewEditMode.Edit && (!ifNotCreatedOnly || listView == null)) {
				listView = null;
				if(CurrentObject != null) {
					listView = helper.CreateListView(CurrentObject);
				}
				Frame.SetView(listView);
			}
		}
		private void OnViewShowingNotification() {
			if(viewShowingNotification != null) {
				viewShowingNotification(this, EventArgs.Empty);
			}
		}
		private string GetPropertyDisplayValueForObject(object obj) {
			return helper.GetDisplayText(obj, EmptyValue, DisplayFormat);
		}
		protected override void ApplyReadOnly() {
			if(findEdit != null) {
				findEdit.ReadOnly = !AllowEdit;
			}
			if(dropDownEdit != null) {
				dropDownEdit.ReadOnly = !AllowEdit;
			}
		}
		protected override WebControl CreateEditModeControlCore() {
			if(newObjectWindowAction == null) {
				newObjectWindowAction = new PopupWindowShowAction(null, "New", PredefinedCategory.Unspecified.ToString());
				newObjectWindowAction.Execute += new PopupWindowShowActionExecuteEventHandler(newObjectWindowAction_OnExecute);
				newObjectWindowAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(newObjectWindowAction_OnCustomizePopupWindowParams);
				newObjectWindowAction.Application = helper.Application;
			}
			Panel panel = new Panel(); 
			dropDownEdit = CreateDropDownLookupControl();
			panel.Controls.Add(dropDownEdit);
			findEdit = CreateFindLookupControl();
			panel.Controls.Add(findEdit);
			UpdateControlsVisibility();
			return panel;
		}
		private void UpdateControlsVisibility() {
			if(dropDownEdit != null || findEdit != null) {
				bool useFindEdit = UseFindEdit();
				if(dropDownEdit != null) {
					dropDownEdit.Visible = !useFindEdit;
				}
				if(findEdit != null) {
					findEdit.Visible = useFindEdit;
				}
			}
			UpdateControlId();
			if(application != null) {
				UpdateFindButtonScript(findEdit, application.PopupWindowManager);
			}
		}
		public virtual bool UseFindEdit() {
			bool useSearchMode = helper.IsSearchEditorMode();
			if(!useSearchMode) {
				useSearchMode = helper.CanFilterDataSource(DataSource, CurrentObject);
			}
			return useSearchMode;
		}
		protected override object GetControlValueCore() {
			if(ViewEditMode == ViewEditMode.Edit && Editor != null) {
				if(!UseFindEdit()) {
					ASPxComboBox dropDownControl = dropDownEdit.DropDown;
					if(dropDownControl.SelectedIndex != -1 && dropDownControl.Value != null) {
						return GetObjectByKey(dropDownControl.Value.ToString());
					}
					return null;
				}
				else {
					return GetObjectByKey(findEdit.Value);
				}
			}
			return PropertyValue;
		}
		protected override void OnCurrentObjectChanged() {
			if(Editor != null) {
				RecreateListView(false);
				UpdateControlsVisibility();
			}
			base.OnCurrentObjectChanged();
			if(dropDownEdit != null) {
				UpdateDropDownLookup(dropDownEdit);
			}
			if(helper != null) {
				helper.SetDataType(CurrentObject);
			}
		}
		protected override string GetPropertyDisplayValue() {
			return GetPropertyDisplayValueForObject(PropertyValue);
		}
		public override void Refresh() {
			UpdateControlsVisibility();
			base.Refresh();
		}
		protected override void ReadEditModeValueCore() {
			UpdateControlsVisibility(); 
			object propertyValue = PropertyValue;
			if(dropDownEdit != null && dropDownEdit.Visible) {
				FillEditorValues(propertyValue, dropDownEdit.DropDown);
			}
			if(findEdit != null) {
				findEdit.Value = helper.GetObjectKey(propertyValue);
				findEdit.Text = GetPropertyDisplayValueForObject(propertyValue);
			}
		}
		public void SetValueToControl(object obj) {
			if(dropDownEdit != null) {
				ASPxComboBox Control = dropDownEdit.DropDown;
				foreach(ListEditItem item in Control.Items) {
					string val = item.Value as string;
					if(val == helper.GetObjectKey(obj)) {
						Control.SelectedIndex = item.Index;
						break;
					}
				}
			}
		}
		protected internal override IJScriptTestControl GetEditorTestControlImpl() {
			if(findEdit != null && UseFindEdit()) {
				return new JSASPxLookupProperytEditorTestControl();
			}
			else {
				return new JSASPxSimpleLookupTestControl();
			}
		}
		protected internal override IJScriptTestControl GetInplaceViewModeEditorTestControlImpl() {
			return new JSButtonTestControl();
		}
		protected override WebControl GetActiveControl() {
			if(ActiveControl is ASPxLookupFindEdit) {
				return ((ASPxLookupFindEdit)ActiveControl).Editor;
			}
			if(ActiveControl is ASPxLookupDropDownEdit) {
				return ((ASPxLookupDropDownEdit)ActiveControl).DropDown;
			}
			return base.GetActiveControl();
		}
		protected override string GetEditorClientId() {
			if(!UseFindEdit()) {
				return dropDownEdit.ClientID;
			}
			else {
				return findEdit.ClientID;
			}
		}
		private void UpdateControlId() {
			if(ActiveControl != null) {
				ActiveControl.ID = editorId;
			}
			if(InactiveControl != null) {
				InactiveControl.ID = editorId + "_inactive";
			}
		}
		protected override void SetEditorId(string controlId) {
			this.editorId = controlId;
			UpdateControlId();
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(showFindSelectWindowAction != null) {
						showFindSelectWindowAction.Execute -= new PopupWindowShowActionExecuteEventHandler(actionFind_OnExecute);
						showFindSelectWindowAction.CustomizePopupWindowParams -= new CustomizePopupWindowParamsEventHandler(action_FindWindowParamsCustomizing);
						DisposeAction(showFindSelectWindowAction);
						showFindSelectWindowAction = null;
					}
					if(newObjectWindowAction != null) {
						newObjectWindowAction.Execute -= new PopupWindowShowActionExecuteEventHandler(newObjectWindowAction_OnExecute);
						newObjectWindowAction.CustomizePopupWindowParams -= new CustomizePopupWindowParamsEventHandler(newObjectWindowAction_OnCustomizePopupWindowParams);
						DisposeAction(newObjectWindowAction);
						newObjectWindowAction = null;
					}
					if(newObjectViewController != null) {
						newObjectViewController.ObjectCreating -= new EventHandler<ObjectCreatingEventArgs>(newObjectViewController_ObjectCreating);
						newObjectViewController.ObjectCreated -= new EventHandler<ObjectCreatedEventArgs>(newObjectViewController_ObjectCreated);
						newObjectViewController = null;
					}
					if(frame != null) {
						frame.SetView(null);
						frame.Dispose();
						frame = null;
					}
					if(listView != null) {
						listView.Dispose();
						listView = null;
					}
					foreach(IObjectSpace createdObjectSpace in createdObjectSpaces) {
						if(!createdObjectSpace.IsDisposed) {
							createdObjectSpace.Dispose();
						}
					}
					createdObjectSpaces.Clear();
					newObject = null;
					newObjectSpace = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public WebLookupEditorHelper WebLookupEditorHelper {
			get { return helper; }
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(findEdit != null) {
				findEdit.ValueChanged -= new EventHandler(findLookup_ValueChanged);
				findEdit.Callback -= new EventHandler<CallbackEventArgs>(findLookup_Callback);
			}
			if(dropDownEdit != null) {
				dropDownEdit.DropDown.SelectedIndexChanged -= new EventHandler(dropDownLookup_SelectedIndexChanged);
				dropDownEdit.Callback -= new EventHandler<CallbackEventArgs>(dropDownLookup_Callback);
				dropDownEdit.Init -= new EventHandler(dropDownLookup_Init);
				dropDownEdit.PreRender -= new EventHandler(dropDownLookup_PreRender);
			}
			if(!unwireEventsOnly) {
				findEdit = null;
				dropDownEdit = null;
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public ASPxLookupPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			skipEditModeDataBind = true;
		}
		public void SetControlValue(object val) {
			object selectedObject = GetControlValueCore();
			if(((selectedObject == null && val == null) || (selectedObject != val)) && (CurrentObject != null)) {
				OnValueStoring(helper.GetDisplayText(val, EmptyValue, DisplayFormat));
				PropertyValue = helper.ObjectSpace.GetObject(val);
				OnValueStored();
				ReadValue();
			}
		}
		public override void Setup(IObjectSpace objectSpace, XafApplication application) {
			base.Setup(objectSpace, application);
			helper = new WebLookupEditorHelper(application, objectSpace, MemberInfo.MemberTypeInfo, Model);
		}
		IList<string> IDependentPropertyEditor.MasterProperties {
			get { return helper.MasterProperties; }
		}
		protected virtual CollectionSourceBase DataSource {
			get {
				if(listView != null) {
					return listView.CollectionSource;
				}
				return null;
			}
		}
		public static int WindowWidth {
			get { return windowWidth; }
			set { windowWidth = value; }
		}
		public static int WindowHeight {
			get { return windowHeight; }
			set { windowHeight = value; }
		}
		public ASPxLookupDropDownEdit DropDownEdit {
			get { return dropDownEdit; }
		}
		public ASPxLookupFindEdit FindEdit {
			get { return findEdit; }
		}
		internal LookupEditorHelper Helper {
			get { return helper; }
		}
		#region ISupportViewShowing Members
		private event EventHandler<EventArgs> viewShowingNotification;
		event EventHandler<EventArgs> ISupportViewShowing.ViewShowingNotification {
			add { viewShowingNotification += value; }
			remove { viewShowingNotification -= value; }
		}
		#endregion
		internal string GetSearchActionName() {
			return Frame.GetController<FilterController>().FullTextFilterAction.Caption;
		}
		#region IFrameContainer Members
		public Frame Frame {
			get {
				InitializeFrame();
				return frame;
			}
		}
		public void InitializeFrame() {
			if(frame == null) {
				frame = helper.Application.CreateNestedFrame(this, TemplateContext.LookupControl);
				newObjectViewController = frame.GetController<NewObjectViewController>();
				if(newObjectViewController != null) {
					newObjectViewController.ObjectCreating += new EventHandler<ObjectCreatingEventArgs>(newObjectViewController_ObjectCreating);
					newObjectViewController.ObjectCreated += new EventHandler<ObjectCreatedEventArgs>(newObjectViewController_ObjectCreated);
				}
			}
		}
		#endregion
#if DebugTest
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public NestedFrame DebugTest_Frame {
			get { return frame; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public LookupEditorHelper DebugTest_Helper {
			get { return Helper; }
		}
		public void DebugTest_FillEditorValues(object currentSelectedObject, ASPxComboBox control) {
			FillEditorValues(currentSelectedObject, control);
		}
		public IList DebugTest_GetObjectsList(object currentSelectedObject) {
			return GetObjectsList(currentSelectedObject);
		}
		public void DebugTest_CreateControlItems(object currentSelectedObject, ArrayList list, ASPxComboBox control, WebLookupEditorHelper helper) {
			CreateControlItems(currentSelectedObject, list, control, helper);
		}
		public void DebugTest_SetDropDownEdit(ASPxLookupDropDownEdit edit) {
			dropDownEdit = edit;
		}
		public void DebugTest_SetFindEdit(ASPxLookupFindEdit edit) {
			findEdit = edit;
		}
		public void DebugTest_UpdateFindButtonScript(ASPxLookupFindEdit edit, PopupWindowManager manager) {
			UpdateFindButtonScript(edit, manager);
		}
		public void DebugTest_SetFindAction(PopupWindowShowAction action) {
			this.showFindSelectWindowAction = action;
		}
#endif
	}
	public class CallbackEventArgs : EventArgs {
		public CallbackEventArgs(string argument) {
			Argument = argument;
		}
		public string Argument;
	}
	[ToolboxItem(false)]
	public class ASPxLookupDropDownEdit : WebControl, ICallbackEventHandler, INamingContainer { 
		private bool addingEnabled;
		private bool clearingEnabled = true;
		private EditButton newButton;
		private EditButton clearButton;
		private ASPxComboBox dropDown;
		private bool isPrerendered = false;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public const string ClearButtonClickScriptKey = "cpClearButtonClickScript";
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public const string NewButtonClickScriptKey = "cpNewButtonClickScript";
		private void SetButtonVisibility(EditButton button, bool isVisible) {
			if(button != null) {
				button.Enabled = isVisible;
				button.Visible = isVisible;
			}
		}
		protected override void Render(HtmlTextWriter writer) {
			if(!isPrerendered) {
				OnPreRender(EventArgs.Empty);
			}
			Page.ClientScript.RegisterForEventValidation(dropDown.ClientID);
			base.Render(writer);
		}
		protected override void OnPreRender(EventArgs e) {
			isPrerendered = true;
			if(!ReadOnly) {
				newButton.Enabled = addingEnabled;
				clearButton.Enabled = clearingEnabled;
				if(clearingEnabled) {
					string clearButtonClickScript =
					@"  var processOnServer = false;
						var dropDownControl = ASPx.GetControlCollection().Get('" + dropDown.ClientID + @"');
						if(dropDownControl.GetSelectedIndex() != 0) {
							dropDownControl.SetSelectedIndex(0);
							processOnServer = dropDownControl.RaiseValueChangedEvent();
						}
						e.processOnServer = processOnServer;";
					dropDown.JSProperties[ClearButtonClickScriptKey] = clearButtonClickScript;
				}
			}
			else {
				clearButton.Visible = false;
				newButton.Visible = false;
			}
			base.OnPreRender(e);
		}
		public ASPxLookupDropDownEdit() : base( HtmlTextWriterTag.Div) {
			dropDown = RenderHelper.CreateASPxComboBox();
			dropDown.CustomButtonsPosition = CustomButtonsPosition.Far;
			dropDown.ID = "DD";
			dropDown.Width = Unit.Percentage(100);
			dropDown.CssClass = "xafLookupEditor";
			dropDown.ClientSideEvents.ButtonClick = string.Format("function(s,e) {{ if(e.buttonIndex == 0) {{ if(!window.buttonEditAlreadyClicked) {{ window.buttonEditAlreadyClicked = true; eval(s.{0}); }} }} if(e.buttonIndex == 1) {{ eval(s.{1}); }} }}", NewButtonClickScriptKey, ClearButtonClickScriptKey);
			newButton = DropDown.Buttons.Add();
			ASPxImageHelper.SetImageProperties(newButton.Image, "Action_New_12x12");
			clearButton = dropDown.Buttons.Add();
			clearButton.ToolTip = CaptionHelper.GetLocalizedText("DialogButtons", "Clear");
			ASPxImageHelper.SetImageProperties(clearButton.Image, "Editor_Clear");
			if(clearButton.Image.IsEmpty) {
				clearButton.Text = CaptionHelper.GetLocalizedText("DialogButtons", "Clear");
			}
			Controls.Add(dropDown);
		}
		public string GetProcessNewObjFunction() {
			return "xafDropDownLookupProcessNewObject('" + UniqueID + "')";
		}
		public void SetClientNewButtonScript(string value) {
			string newButtonClickScript = "document.isMenuClicked= true; e.handled = true; e.processOnServer = false; " + value;
			dropDown.JSProperties[NewButtonClickScriptKey] = newButtonClickScript;
		}
		public ASPxComboBox DropDown {
			get { return dropDown; }
		}
		public EditButton ClearButton {
			get { return clearButton; }
		}
		public EditButton NewButton {
			get { return newButton; }
		}
		public string SelectedItem {
			get {
				if(dropDown.SelectedItem == null) {
					return null;
				}
				else {
					return dropDown.SelectedItem.Text;
				}
			}
		}
		public bool ReadOnly {
			get { return dropDown.ReadOnly; }
			set {
				dropDown.ReadOnly = value;
				dropDown.ClientEnabled = !value;
			}
		}
		public bool AddingEnabled {
			get { return addingEnabled; }
			set {
				addingEnabled = value;
				SetButtonVisibility(newButton, addingEnabled);
			}
		}
		public bool ClearingEnabled {
			get { return clearingEnabled; }
			set {
				clearingEnabled = value;
				SetButtonVisibility(clearButton, clearingEnabled);
			}
		}
		public string NewActionCaption {
			get { return newButton.Text; }
			set {
				newButton.ToolTip = value;
				if(newButton.Image.IsEmpty) {
					newButton.Text = value;
				}
				dropDown.JSProperties["cpNewButtonCaption"] = value;
			}
		}
		public event EventHandler<CallbackEventArgs> Callback;
		#region ICallbackEventHandler Members
		public string GetCallbackResult() {
			StringBuilder result = new StringBuilder();
			foreach(ListEditItem item in dropDown.Items) {
				result.AppendFormat("{0}<{1}{2}|", HttpUtility.HtmlAttributeEncode(item.Text), item.Value, dropDown.SelectedItem == item ? "<" : "");
			}
			if(result.Length > 0) {
				result.Remove(result.Length - 1, 1);
			}
			return string.Format("{0}><{1}", dropDown.ClientID, result.ToString());
		}
		public void RaiseCallbackEvent(string eventArgument) {
			if(Callback != null) {
				Callback(this, new CallbackEventArgs(eventArgument));
			}
		}
		#endregion
	}
	[ToolboxItem(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class LookupHiddenField : HiddenField {
		bool isPostDataLoaded = false;
		protected override bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection) {
			isPostDataLoaded = isPostDataLoaded || base.LoadPostData(postDataKey, postCollection);
			return isPostDataLoaded;
		}
	}
	[ToolboxItem(false)]
	public class ASPxLookupFindEdit : WebControl, ICallbackEventHandler, INamingContainer { 
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public const string FindButtonClickScriptKey = "cpFindButtonClickScript";
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public const string ClearButtonClickScriptKey = "cpClearButtonClickScript";
		private EditButton findButton;
		private EditButton clearButton;
		private ASPxButtonEdit edit;
		private HiddenField hidden;
		private bool isPrerendered = false;
		private bool readOnly;
		private void hidden_ValueChanged(object sender, EventArgs e) {
			if(ValueChanged != null)
				ValueChanged(this, EventArgs.Empty);
		}
		private void SetClearButtonClientScript() {
			string clearButtonClickScript =
			   @"document.getElementById('" + hidden.ClientID + @"').value = '';
				var textBox = ASPx.GetControlCollection().Get('" + Editor.ClientID + @"');
                textBox.SetValue('" + CaptionHelper.NullValueText + @"');
                textBox.RaiseValueChangedEvent();
				e.processOnServer = false;";
			this.edit.JSProperties["cpClearButtonClickScript"] = clearButtonClickScript;
		}
		protected override void Render(HtmlTextWriter writer) {
			if(!isPrerendered) {
				OnPreRender(EventArgs.Empty);
			}
			base.Render(writer);
		}
		protected override void OnPreRender(EventArgs e) {
			isPrerendered = true;
			if(!ReadOnly) {
				SetClearButtonClientScript();
			}
			else {
				findButton.Visible = false;
				clearButton.Visible = false;
			}
			base.OnPreRender(e);
		}
		public Control Hidden {
			get {
				return hidden;
			}
		}
		public override void Dispose() {
			try {
				if(hidden != null) {
					hidden.ValueChanged -= new EventHandler(hidden_ValueChanged);
				}
			}
			finally {
				base.Dispose();
			}
		}
		public void SetFindButtonClientScript(string value) {
			if(!string.IsNullOrEmpty(value)) {
				this.edit.JSProperties[FindButtonClickScriptKey] = "document.isMenuClicked= true; e.processOnServer = false;" + value;
			}
			else {
				findButton.Visible = false;
			}
		}
		public ASPxLookupFindEdit()
			: base(HtmlTextWriterTag.Div) {
			edit = new ASPxButtonEdit();
			edit.EnableViewState = false;
			edit.EnableClientSideAPI = true;
			edit.ID = "Edit";
			edit.ReadOnly = true;
			edit.Width = new Unit("100%");
			edit.ClientSideEvents.ButtonClick = string.Format("function(s,e) {{ if(e.buttonIndex == 0) {{ if(!window.buttonEditAlreadyClicked) {{ window.buttonEditAlreadyClicked = true; eval(s.{0}); }} }} if(e.buttonIndex == 1) {{ eval(s.{1}); }} }}", FindButtonClickScriptKey, ClearButtonClickScriptKey);
			findButton = edit.Buttons.Add();
			string localizedCaption = CaptionHelper.GetLocalizedText("DialogButtons", "Find");
			findButton.ToolTip = localizedCaption;
			ASPxImageHelper.SetImageProperties(findButton.Image, "Editor_Search");
			if(findButton.Image.IsEmpty) {
				findButton.Text = localizedCaption;
			}
			clearButton = edit.Buttons.Add();
			clearButton.ToolTip = CaptionHelper.GetLocalizedText("DialogButtons", "Clear");
			ASPxImageHelper.SetImageProperties(clearButton.Image, "Editor_Clear");
			if(clearButton.Image.IsEmpty) {
				clearButton.Text = CaptionHelper.GetLocalizedText("DialogButtons", "Clear");
			}
			edit.JSProperties["cpFindButtonCaption"] = localizedCaption;
			hidden = new LookupHiddenField();
			hidden.ID = "HDN";
			hidden.ValueChanged += new EventHandler(hidden_ValueChanged);
			Controls.Add(edit);
			Controls.Add(hidden); 
		}
		public string Text {
			get { return edit.Text; }
			set { edit.Text = value; }
		}
		public string Value {
			get { return hidden.Value; }
			set { hidden.Value = value; }
		}
		public bool ReadOnly {
			get { return readOnly; }
			set {
				readOnly = value;
				if(edit != null) {
					edit.ClientEnabled = !readOnly;
				}
			}
		}
		public ASPxButtonEdit Editor {
			get { return edit; }
		}
		public EditButton ClearButton {
			get { return clearButton; }
		}
		public EditButton FindButton {
			get { return findButton; }
		}
		public event EventHandler<CallbackEventArgs> Callback;
		public event EventHandler ValueChanged;
		#region ICallbackEventHandler Members
		public string GetCallbackResult() {
			return edit.ClientID + "><" + HttpUtility.HtmlAttributeEncode(Text);
		}
		public void RaiseCallbackEvent(string eventArgument) {
			if(Callback != null) {
				Callback(this, new CallbackEventArgs(eventArgument));
			}
		}
		#endregion
		#region Obsolete 15.1
		[Obsolete("Use the Editor property instead", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ASPxTextBox TextBox {
			get { return null; }
		}
		#endregion
	}
}
