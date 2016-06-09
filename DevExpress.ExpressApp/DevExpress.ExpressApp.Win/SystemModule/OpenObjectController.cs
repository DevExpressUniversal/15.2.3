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
using System.Drawing;
using System.Security;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class OpenObjectController : ViewController {
		public abstract class ViewAdapter {
			private readonly OpenObjectController controller;
			protected Boolean CanOpen(Type type) {
				return Controller.CanOpen(type);
			}
			protected Object FindLastObject(Object sourceObject, IMemberInfo memberInfo) {
				return Controller.FindLastObject(sourceObject, memberInfo);
			}
			protected Boolean CanOpen(Object obj) {
				return Controller.CanOpen(obj);
			}
			protected void SetObjectToOpen(Object objectToOpen) {
				Controller.SetObjectToOpen(objectToOpen);
			}
			protected void OpenObject(Object objectToOpen) {
				SetObjectToOpen(objectToOpen);
				SimpleAction openObjectAction = Controller.OpenObjectAction;
				if(openObjectAction.Active && openObjectAction.Enabled) {
					openObjectAction.DoExecute();
				}
			}
			public ViewAdapter(OpenObjectController controller) {
				Guard.ArgumentNotNull(controller, "controller");
				this.controller = controller;
			}
			public abstract Boolean ContainsObjectEditor();
			public virtual void Activate() { }
			public virtual void Deactivate() { }
			protected OpenObjectController Controller {
				get { return controller; }
			}
		}
		public sealed class EmptyViewAdapter : ViewAdapter {
			public EmptyViewAdapter(OpenObjectController controller) : base(controller) { }
			public override Boolean ContainsObjectEditor() {
				return false;
			}
		}
		public sealed class DetailViewAdapter : ViewAdapter {
			private readonly DetailView detailView;
			private readonly Dictionary<Control, WinPropertyEditor> controlToEditorMap;
			private readonly Dictionary<WinPropertyEditor, ControlCursorHelper> cursorHelpers;
			private void detailView_ControlsCreated(Object sender, EventArgs e) {
				foreach(WinPropertyEditor winPropertyEditor in detailView.GetItems<WinPropertyEditor>()) {
					if(winPropertyEditor is LookupPropertyEditor) {
						((LookupPropertyEditor)winPropertyEditor).QueryPopUp += new CancelEventHandler(PropertyEditor_QueryPopUp);
					}
					if(winPropertyEditor.Control == null) {
						winPropertyEditor.ControlCreated += new EventHandler<EventArgs>(winPropertyEditor_ControlCreated);
					}
					else {
						SubscribeToWinPropertyEditor(winPropertyEditor);
					}
				}
			}
			private void PropertyEditor_QueryPopUp(Object sender, CancelEventArgs e) {
				e.Cancel = NativeMethods.IsCtrlShiftPressed();
			}
			private void winPropertyEditor_ControlCreated(Object sender, EventArgs e) {
				SubscribeToWinPropertyEditor((WinPropertyEditor)sender);
			}
			private void SubscribeToWinPropertyEditor(WinPropertyEditor winPropertyEditor) {
				if(winPropertyEditor.Control != null) {
					controlToEditorMap.Add(winPropertyEditor.Control, winPropertyEditor);
					cursorHelpers.Add(winPropertyEditor, new ControlCursorHelper(winPropertyEditor.Control));
					winPropertyEditor.Control.KeyDown += new KeyEventHandler(Control_KeyDown);
					winPropertyEditor.Control.MouseDown += new MouseEventHandler(Control_MouseDown);
					winPropertyEditor.Control.MouseMove += new MouseEventHandler(Control_MouseMove);
					winPropertyEditor.Control.Enter += new EventHandler(Control_Focused);
					winPropertyEditor.ControlValueChanged += new EventHandler(PropertyEditor_ControlValueChanged);
					winPropertyEditor.ValueRead += new EventHandler(PropertyEditor_ValueRead);
				}
			}
			private void UnsubscribeFromWinPropertyEditor(WinPropertyEditor winPropertyEditor) {
				if(winPropertyEditor.Control != null) {
					winPropertyEditor.Control.KeyDown -= new KeyEventHandler(Control_KeyDown);
					winPropertyEditor.Control.MouseDown -= new MouseEventHandler(Control_MouseDown);
					winPropertyEditor.Control.MouseMove -= new MouseEventHandler(Control_MouseMove);
					winPropertyEditor.Control.Enter -= new EventHandler(Control_Focused);
					winPropertyEditor.ControlValueChanged -= new EventHandler(PropertyEditor_ControlValueChanged);
					winPropertyEditor.ValueRead -= new EventHandler(PropertyEditor_ValueRead);
				}
			}
			private void Control_KeyDown(Object sender, KeyEventArgs e) {
				if(e.KeyCode == Keys.Enter && e.Control && e.Shift && !e.Alt) {
					WinPropertyEditor propertyEditor;
					if(controlToEditorMap.TryGetValue((Control)sender, out propertyEditor)) {
						OpenObject(FindObjectToOpen(propertyEditor));
					}
				}
			}
			private void Control_MouseDown(Object sender, MouseEventArgs e) {
				if(e.Button == MouseButtons.Left && NativeMethods.IsCtrlShiftPressed()) {
					WinPropertyEditor propertyEditor;
					if(controlToEditorMap.TryGetValue((Control)sender, out propertyEditor)) {
						OpenObject(FindObjectToOpen(propertyEditor));
					}
				}
			}
			private void Control_MouseMove(Object sender, MouseEventArgs e) {
				WinPropertyEditor propertyEditor;
				if(controlToEditorMap.TryGetValue((Control)sender, out propertyEditor)) {
					ControlCursorHelper cursorHelper = cursorHelpers[propertyEditor];
					if(NativeMethods.IsCtrlShiftPressed() && CanOpen(FindObjectToOpen(propertyEditor))) {
						cursorHelper.ChangeControlCursor(Cursors.Hand);
					}
					else {
						cursorHelper.Restore();
					}
				}
			}
			private void Control_Focused(Object sender, EventArgs e) {
				WinPropertyEditor propertyEditor;
				if(controlToEditorMap.TryGetValue((Control)sender, out propertyEditor)) {
					SetObjectToOpen(FindObjectToOpen(propertyEditor));
				}
			}
			private void PropertyEditor_ControlValueChanged(Object sender, EventArgs e) {
				SetObjectToOpen(FindObjectToOpen(sender as WinPropertyEditor));
			}
			private void PropertyEditor_ValueRead(Object sender, EventArgs e) {
				WinPropertyEditor editor = sender as WinPropertyEditor;
				if(((Control)editor.Control).Focused) {
					SetObjectToOpen(FindObjectToOpen(editor));
				}
			}
			private Object FindObjectToOpen(PropertyEditor propertyEditor) {
				Object result = null;
				if(propertyEditor != null) {
					if(propertyEditor.MemberInfo != null && SimpleTypes.IsSimpleType(propertyEditor.MemberInfo.MemberType)) {
						result = FindLastObject(propertyEditor.CurrentObject, propertyEditor.MemberInfo);
					}
					else {
						result = propertyEditor.ControlValue;
					}
				}
				return result;
			}
			public DetailViewAdapter(OpenObjectController controller)
				: base(controller) {
				detailView = (DetailView)Controller.View;
				controlToEditorMap = new Dictionary<Control, WinPropertyEditor>();
				cursorHelpers = new Dictionary<WinPropertyEditor, ControlCursorHelper>();
			}
			public override void Activate() {
				base.Activate();
				detailView.ControlsCreated += new EventHandler(detailView_ControlsCreated);
			}
			public override void Deactivate() {
				detailView.ControlsCreated -= new EventHandler(detailView_ControlsCreated);
				cursorHelpers.Clear();
				controlToEditorMap.Clear();
				foreach(WinPropertyEditor winPropertyEditor in detailView.GetItems<WinPropertyEditor>()) {
					if(winPropertyEditor is LookupPropertyEditor) {
						((LookupPropertyEditor)winPropertyEditor).QueryPopUp -= new CancelEventHandler(PropertyEditor_QueryPopUp);
					}
					UnsubscribeFromWinPropertyEditor(winPropertyEditor);
					winPropertyEditor.ControlCreated -= new EventHandler<EventArgs>(winPropertyEditor_ControlCreated);
				}
				base.Deactivate();
			}
			public override Boolean ContainsObjectEditor() {
				foreach(PropertyEditor propertyEditor in detailView.GetItems<PropertyEditor>()) {
					Type objectType = propertyEditor.MemberInfo.MemberType;
					if(CanOpen(objectType)) {
						return true;
					}
				}
				return false;
			}
		}
		public sealed class ListViewAdapter : ViewAdapter {
			private readonly ListView listView;
			private GridControl grid;
			private GridView gridView;
			private ControlCursorHelper cursorHelper;
			private GridListEditor GridListEditor {
				get { return listView.Editor as GridListEditor; }
			}
			private void listView_ControlsCreated(Object sender, EventArgs e) {
				UnsubscribeFromGrid();
				if(GridListEditor != null) {
					cursorHelper = new ControlCursorHelper(GridListEditor.Grid);
					grid = GridListEditor.Grid;
					gridView = GridListEditor.GridView;
					SubscribeToGrid();
				}
			}
			private void SubscribeToGrid() {
				grid.MouseDown += new MouseEventHandler(grid_MouseDown);
				grid.MouseMove += new MouseEventHandler(grid_MouseMove);
				gridView.FocusedRowObjectChanged += new FocusedRowObjectChangedEventHandler(gridView_FocusedRowObjectChanged);
				gridView.FocusedColumnChanged += new FocusedColumnChangedEventHandler(gridView_FocusedColumnChanged);
				gridView.ShownEditor += new EventHandler(gridView_ShownEditor);
				gridView.DataSourceChanged += new EventHandler(gridView_DataSourceChanged);
			}
			private void UnsubscribeFromGrid() {
				if(grid != null) {
					grid.MouseDown -= new MouseEventHandler(grid_MouseDown);
					grid.MouseMove -= new MouseEventHandler(grid_MouseMove);
					gridView.FocusedRowObjectChanged -= new FocusedRowObjectChangedEventHandler(gridView_FocusedRowObjectChanged);
					gridView.FocusedColumnChanged -= new FocusedColumnChangedEventHandler(gridView_FocusedColumnChanged);
					gridView.ShownEditor -= new EventHandler(gridView_ShownEditor);
					gridView.DataSourceChanged -= new EventHandler(gridView_DataSourceChanged);
				}
			}
			private void grid_MouseDown(Object sender, MouseEventArgs eventArgs) {
				if(eventArgs.Button == MouseButtons.Left && NativeMethods.IsCtrlShiftPressed()) {
					Object objectToOpen = FindObjectToOpen(eventArgs.X, eventArgs.Y);
					if(CanOpen(objectToOpen)) {
						DevExpress.Utils.DXMouseEventArgs dxArgs = eventArgs as DevExpress.Utils.DXMouseEventArgs;
						if(dxArgs != null) {
							dxArgs.Handled = true;
						}
						OpenObject(objectToOpen);
					}
				}
			}
			private void grid_MouseMove(Object sender, MouseEventArgs eventArgs) {
				if(NativeMethods.IsCtrlShiftPressed() && CanOpen(FindObjectToOpen(eventArgs.X, eventArgs.Y))) {
					cursorHelper.ChangeControlCursor(Cursors.Hand);
				}
				else {
					cursorHelper.Restore();
				}
			}
			private void gridView_FocusedRowObjectChanged(Object sender, FocusedRowObjectChangedEventArgs e) {
				SetObjectToOpen(FindObjectToOpen(GridListEditor.GridView.FocusedColumn, GridListEditor.GridView.FocusedRowHandle));
			}
			private void gridView_FocusedColumnChanged(Object sender, FocusedColumnChangedEventArgs e) {
				SetObjectToOpen(FindObjectToOpen(GridListEditor.GridView.FocusedColumn, GridListEditor.GridView.FocusedRowHandle));
			}
			private void gridView_ShownEditor(Object sender, EventArgs e) {
				GridListEditor.GridView.ActiveEditor.EditValueChanged += new EventHandler(ActiveEditor_EditValueChanged);
				GridListEditor.GridView.ActiveEditor.VisibleChanged += new EventHandler(ActiveEditor_VisibleChanged);
			}
			private void ActiveEditor_VisibleChanged(Object sender, EventArgs e) {
				BaseEdit editor = sender as BaseEdit;
				if(!editor.Visible) {
					editor.EditValueChanged -= new EventHandler(ActiveEditor_EditValueChanged);
				}
			}
			private void ActiveEditor_EditValueChanged(Object sender, EventArgs e) {
				SetObjectToOpen(FindObjectToOpen(GridListEditor.GridView.FocusedColumn, GridListEditor.GridView.FocusedRowHandle));
			}
			private void gridView_DataSourceChanged(Object sender, EventArgs e) {
				SetObjectToOpen(FindObjectToOpen(GridListEditor.GridView.FocusedColumn, GridListEditor.GridView.FocusedRowHandle));
			}
			private Object FindObjectToOpen(int mouseX, int mouseY) {
				if(GridListEditor != null && GridListEditor.GridView != null) {
					GridHitInfo gridHitInfo = GridListEditor.GridView.CalcHitInfo(new Point(mouseX, mouseY));
					return FindObjectToOpen(gridHitInfo.Column, gridHitInfo.RowHandle);
				}
				else {
					return null;
				}
			}
			private Object FindObjectToOpen(GridColumn column, int rowHandle) {
				Object result = null;
				if(column != null && GridListEditor != null && GridListEditor.GridView != null) {
					if(GridListEditor.GridView.ActiveEditor != null) {
						result = GridListEditor.GridView.ActiveEditor.EditValue;
					}
					else {
						Object currObject = XtraGridUtils.GetRow(GridListEditor.GridView, rowHandle);
						if(currObject != null) {
							ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(currObject.GetType());
							IMemberInfo memberInfo = typeInfo.FindMember(column.FieldName);
							if(memberInfo != null) {
								result = FindLastObject(currObject, memberInfo);
							}
						}
					}
				}
				return result;
			}
			public ListViewAdapter(OpenObjectController controller)
				: base(controller) {
				listView = (ListView)Controller.View;
			}
			public override void Activate() {
				base.Activate();
				listView.ControlsCreated += new EventHandler(listView_ControlsCreated);
			}
			public override void Deactivate() {
				listView.ControlsCreated -= new EventHandler(listView_ControlsCreated);
				UnsubscribeFromGrid();
				cursorHelper = null;
				grid = null;
				gridView = null;
				base.Deactivate();
			}
			public override Boolean ContainsObjectEditor() {
				if(listView.Model != null) {
					foreach(IModelColumn columnInfo in listView.Model.Columns) {
						IMemberInfo findMember = listView.ObjectTypeInfo.FindMember(columnInfo.PropertyName);
						if(findMember != null && CanOpen(findMember.MemberType)) {
							return true;
						}
					}
				}
				return false;
			}
		}
		public const string ActiveKeyHasReadPermissionToTargetType = "HasReadPermissionToTargetType";
		private const string ActiveKeyViewContainsObjectEditor = "HasObjectEditor";
		private const string EnabledKeyCanOpenObject = "CanOpenObject";
		private const string DataViewMode = "DataViewMode";
		private readonly SimpleAction openObjectAction;
		private ViewAdapter adapter;
		private Object objectToOpen;
		private void openObjectAction_Execute(Object sender, SimpleActionExecuteEventArgs e) {
			OpenObject(objectToOpen, e);
		}
		public OpenObjectController() {
			TypeOfView = typeof(ObjectView);
			openObjectAction = new SimpleAction(this, "OpenObject", PredefinedCategory.OpenObject);
			openObjectAction.Caption = "Open Related Record";
			openObjectAction.ToolTip = "Open the record associated with the focused reference property editor (SHIFT+CTRL+left click)";
			openObjectAction.ImageName = "MenuBar_OpenObject";
			openObjectAction.Shortcut = "CtrlShiftO";
			openObjectAction.Execute += new SimpleActionExecuteEventHandler(openObjectAction_Execute);
		}
		protected Boolean CanOpen(Type type) {
			return type != null && IsDetailViewExists(type) && IsReadGrantedForType(type);
		}
		private Boolean IsDetailViewExists(Type type) {
			return !String.IsNullOrEmpty(Application.FindDetailViewId(type));
		}
		private Boolean IsReadGrantedForType(Type type) {
			Guard.ArgumentNotNull(type, "type");
			return DataManipulationRight.CanRead(type, null, null, null, ObjectSpace);
		}
		private Boolean IsListViewInDataViewMode {
			get {
				return View is ListView && View.Model != null && ((IModelListView)View.Model).DataAccessMode == CollectionSourceDataAccessMode.DataView;
			}
		}
		protected Object FindLastObject(Object sourceObject, IMemberInfo memberInfo) {
			Object result = null;
			IList<IMemberInfo> path = memberInfo.GetPath();
			if(!SimpleTypes.IsSimpleType(path[0].MemberType)) {
				result = path[0].GetValue(sourceObject);
				for(int i = 1; i < path.Count; i++) {
					if(SimpleTypes.IsSimpleType(path[i].MemberType)) {
						break;
					}
					result = path[i].GetValue(result);
				}
			}
			return result;
		}
		protected Boolean CanOpen(Object obj) {
			return obj != null && IsDetailViewExists(obj) && !IsNewObject(obj) && IsReadGrantedForObject(obj);
		}
		private Boolean IsDetailViewExists(Object obj) {
			return IsDetailViewExists(obj.GetType());
		}
		private Boolean IsNewObject(Object obj) {
			return ObjectSpace != null && ObjectSpace.IsNewObject(obj);
		}
		private Boolean IsReadGrantedForObject(Object obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return DataManipulationRight.CanRead(obj.GetType(), null, obj, null, ObjectSpace);
		}
		protected override void OnActivated() {
			base.OnActivated();
			bool isReadGrantedForType = IsReadGrantedForType(View.ObjectTypeInfo.Type);
			OpenObjectAction.Active[ActiveKeyHasReadPermissionToTargetType] = isReadGrantedForType;
			OpenObjectAction.Active[DataViewMode] = !IsListViewInDataViewMode;
			if(isReadGrantedForType && !IsListViewInDataViewMode) {
				ViewAdapter localAdapter = CreateViewAdapter();
				bool isContainsObjectEditor = localAdapter.ContainsObjectEditor();
				OpenObjectAction.Active[ActiveKeyViewContainsObjectEditor] = isContainsObjectEditor;
				if(isContainsObjectEditor) {
					adapter = localAdapter;
					adapter.Activate();
					UpdateActionState(objectToOpen);
				}
			}
		}
		protected virtual ViewAdapter CreateViewAdapter() {
			if(View is DetailView) {
				return new DetailViewAdapter(this);
			}
			else if(View is ListView) {
				return new ListViewAdapter(this);
			}
			else {
				return new EmptyViewAdapter(this);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(adapter != null) {
				adapter.Deactivate();
				adapter = null;
			}
			OpenObjectAction.Active.RemoveItem(ActiveKeyHasReadPermissionToTargetType);
			OpenObjectAction.Active.RemoveItem(ActiveKeyViewContainsObjectEditor);
			OpenObjectAction.Enabled.RemoveItem(EnabledKeyCanOpenObject);
		}
		protected void SetObjectToOpen(Object objectToOpen) {
			this.objectToOpen = objectToOpen;
			UpdateActionState(objectToOpen);
		}
		protected virtual void UpdateActionState(Object objectToOpen) {
			OpenObjectAction.Enabled[EnabledKeyCanOpenObject] = CanOpen(objectToOpen);
		}
		protected virtual void OpenObject(Object objectToOpen, SimpleActionExecuteEventArgs e) {
			Guard.ArgumentNotNull(objectToOpen, "objectToOpen");
			if(!IsReadGrantedForObject(objectToOpen)) {
				throw new SecurityException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.PermissionIsDenied));
			}
			CustomOpenObjectEventArgs customOpenObjectEventArgs = new CustomOpenObjectEventArgs(objectToOpen, e);
			OnCustomOpenObject(customOpenObjectEventArgs);
			if(!customOpenObjectEventArgs.Handled) {
				DetailViewLinkController detailViewLinkController = Application.CreateController<DetailViewLinkController>();
				detailViewLinkController.SkipCurrentObjectMoving = true;
				e.ShowViewParameters.Controllers.Add(detailViewLinkController);
				ListViewProcessCurrentObjectController.ShowObject(objectToOpen, e.ShowViewParameters, Application, Frame, View);
			}
		}
		protected virtual void OnCustomOpenObject(CustomOpenObjectEventArgs e) {
			if(CustomOpenObject != null) {
				CustomOpenObject(this, e);
			}
		}
		public SimpleAction OpenObjectAction {
			get { return openObjectAction; }
		}
		public event EventHandler<CustomOpenObjectEventArgs> CustomOpenObject;
	}
	public class CustomOpenObjectEventArgs : HandledEventArgs {
		private readonly Object objectToOpen;
		private readonly SimpleActionExecuteEventArgs innerArgs;
		public CustomOpenObjectEventArgs(Object objectToOpen, SimpleActionExecuteEventArgs innerArgs) {
			this.objectToOpen = objectToOpen;
			this.innerArgs = innerArgs;
		}
		public Object ObjectToOpen {
			get { return objectToOpen; }
		}
		public SimpleActionExecuteEventArgs InnerArgs {
			get { return innerArgs; }
		}
	}
}
