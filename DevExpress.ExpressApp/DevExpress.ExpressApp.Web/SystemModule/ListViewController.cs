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
using System.Web.UI;
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Localization;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class ListViewController : ObjectViewController {
		public const string EditActionId = "Edit";
		public const string InlineEditActionId = "InlineEdit";
		private System.ComponentModel.IContainer components;
		private DevExpress.ExpressApp.Actions.SimpleAction editAction;
		private DevExpress.ExpressApp.Actions.SimpleAction inlineEditAction;
		private void SubscribeToContextMenuTemplate() {
			ListView listView = View as ListView;
			if(listView != null) {
				DevExpress.ExpressApp.Editors.ListEditor listEditor = listView.Editor;
				if(listEditor != null) {
					IContextMenuTemplate contextMenuTemplate = listEditor.ContextMenuTemplate;
					if(contextMenuTemplate != null) {
						contextMenuTemplate.BoundItemCreating += new EventHandler<BoundItemCreatingEventArgs>(contextMenuTemplate_BoundItemCreating);
					}
				}
			}
		}
		private void UnsubscribeFromContextMenuTemplate() {
			ListView listView = View as ListView;
			if(listView != null) {
				DevExpress.ExpressApp.Editors.ListEditor listEditor = listView.Editor;
				if(listEditor != null) {
					IContextMenuTemplate contextMenuTemplate = listEditor.ContextMenuTemplate;
					if(contextMenuTemplate != null) {
						contextMenuTemplate.BoundItemCreating -= new EventHandler<BoundItemCreatingEventArgs>(contextMenuTemplate_BoundItemCreating);
					}
				}
			}
		}
		private void contextMenuTemplate_BoundItemCreating(object sender, BoundItemCreatingEventArgs args) {
			UpdateInlineActionState(args);
		}
		protected virtual void UpdateInlineActionState(BoundItemCreatingEventArgs args) {
			if(args.Action.Id == editAction.Id || args.Action.Id == inlineEditAction.Id) {
				args.Enabled = args.Enabled && DataManipulationRight.CanEdit(args.Object != null ? args.Object.GetType() : View.ObjectTypeInfo.Type, "", args.Object, View is ListView ? ((ListView)View).CollectionSource : null, View.ObjectSpace);
			}
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.editAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
			this.inlineEditAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
			this.editAction.Caption = "Edit";
			this.editAction.Category = PredefinedCategory.Edit.ToString();
			this.editAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
			this.editAction.Id = EditActionId;
			if(WebApplicationStyleManager.IsNewStyle) {
				this.editAction.ImageName = "GridView_Edit";
			}
			else {
				this.editAction.ImageName = "MenuBar_Edit";
			}
			this.editAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.editAction_OnExecute);
			this.inlineEditAction.Category = "ListView";
			this.inlineEditAction.Id = InlineEditActionId;
			this.inlineEditAction.Caption = "Inline Edit";
			this.TargetViewNesting = DevExpress.ExpressApp.Nesting.Any;
			this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
		}
		private void editAction_OnExecute(Object sender, SimpleActionExecuteEventArgs args) {
			HandledEventArgs eventArgs = new HandledEventArgs(false);
			OnCustomExecuteEdit(eventArgs);
			if(!eventArgs.Handled) {
				ExecuteEdit(args);
			}
		}
		private void View_ControlsCreating(object sender, EventArgs e) {
			UnsubscribeFromContextMenuTemplate();
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			ListView listView = (ListView)sender;
			Control control = (Control)(listView.Editor).Control;
			if(string.IsNullOrEmpty(control.ID)) {
				control.ID = "EC_" + WebIdHelper.GetCorrectedId(listView.Id);
			}
			SubscribeToContextMenuTemplate();
		}
		private void View_AllowEditChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void View_SelectionChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void PropertyEditor_AllowEditChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		protected virtual void UpdateActionState() {
			String detailViewId = ((ListView)View).DetailViewId;
			editAction.Active.SetItemValue("DetailView ID is specified", !string.IsNullOrEmpty(detailViewId));
			if(!String.IsNullOrEmpty(detailViewId) && Application.Model != null) {
				IModelDetailView detailView = Application.Model.Views[detailViewId] as IModelDetailView;
				if(detailView != null && !detailView.AllowEdit) {
					editAction.Active.SetItemValue("DetailView is ReadOnly", false);
				}
				else {
					editAction.Active.RemoveItem("DetailView is ReadOnly");
				}
			}
			else {
				editAction.Active.RemoveItem("DetailView is ReadOnly");
			}
			if(Frame is NestedFrame && ((NestedFrame)Frame).ViewItem is PropertyEditor) {
				editAction.Active[PropertyEditor.PropertyEditorAllowEdit] = ((PropertyEditor)((NestedFrame)Frame).ViewItem).AllowEdit;
			}
			editAction.Enabled.SetItemValue("SecurityAllowEdit", DataManipulationRight.CanEdit(View.CurrentObject != null ? View.CurrentObject.GetType() : View.ObjectTypeInfo.Type, "", View.CurrentObject, View is ListView ? ((ListView)View).CollectionSource : null, View.ObjectSpace));
		}
		protected virtual void OnCustomExecuteEdit(HandledEventArgs e) {
			if(CustomExecuteEdit != null) {
				CustomExecuteEdit(this, e);
			}
		}
		protected virtual void ExecuteEdit(SimpleActionExecuteEventArgs args) {
			IObjectSpace objectSpace = Application.GetObjectSpaceToShowDetailViewFrom(Frame, ObjectSpace.GetObjectType(args.CurrentObject));
			if (ObjectSpace.IsNewObject(args.CurrentObject) && !(objectSpace is INestedObjectSpace)) {
				throw new UserFriendlyException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.AnUnsavedObjectCannotBeShown));
			}
			Object currentObj = objectSpace.GetObject(args.CurrentObject);
			args.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, currentObj, View);
			((DetailView)args.ShowViewParameters.CreatedView).ViewEditMode = ViewEditMode.Edit;
			objectSpace.SetModified(currentObj);
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(Frame is NestedFrame && ((NestedFrame)Frame).ViewItem is PropertyEditor) {
				((PropertyEditor)((NestedFrame)Frame).ViewItem).AllowEditChanged += new EventHandler(PropertyEditor_AllowEditChanged);
			}
			View.ControlsCreating += new EventHandler(View_ControlsCreating);
			View.ControlsCreated += new EventHandler(View_ControlsCreated);
			View.AllowEditChanged += new EventHandler(View_AllowEditChanged);
			View.SelectionChanged += new EventHandler(View_SelectionChanged);
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			SubscribeToContextMenuTemplate();
			UpdateActionState();
		}
		protected override void OnDeactivated() {
			UnsubscribeFromContextMenuTemplate();
			View.ControlsCreating -= new EventHandler(View_ControlsCreating);
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			View.AllowEditChanged -= new EventHandler(View_AllowEditChanged);
			View.SelectionChanged -= new EventHandler(View_SelectionChanged);
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			if(Frame is NestedFrame && ((NestedFrame)Frame).ViewItem is PropertyEditor) {
				((PropertyEditor)((NestedFrame)Frame).ViewItem).AllowEditChanged -= new EventHandler(PropertyEditor_AllowEditChanged);
			}
			base.OnDeactivated();
		}
		public ListViewController()
			: base() {
			InitializeComponent();
			RegisterActions(components);
		}
		public DevExpress.ExpressApp.Actions.SimpleAction EditAction {
			get { return editAction; }
		}
		public DevExpress.ExpressApp.Actions.SimpleAction InlineEditAction {
			get { return inlineEditAction; }
		}
		public event HandledEventHandler CustomExecuteEdit;
	}
}
