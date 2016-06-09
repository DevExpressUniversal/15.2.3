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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.ExpressApp.Win.Editors {
	public class ObjectPropertyEditor : DXPropertyEditor, IComplexViewItem, ISupportViewShowing, IObjectPropertyEditor {
		private ObjectEditorHelper helper;
		private XafApplication application;
		private IObjectSpace objectSpace;
		private void editor_ShowPopupWindow(object sender, EventArgs e) {
			OnViewShowingNotification();
		}
		private void OnViewShowingNotification() {
			if(viewShowingNotification != null) {
				viewShowingNotification(this, EventArgs.Empty);
			}
		}
		protected override object CreateControlCore() {
			ObjectEdit editor = new ObjectEdit();
			editor.ShowPopupWindow += new EventHandler(editor_ShowPopupWindow);
			return editor;
		}
		protected override void SetupRepositoryItem(RepositoryItem item) {
			base.SetupRepositoryItem(item);
			RepositoryItemObjectEdit objectEditItem = (RepositoryItemObjectEdit)item;
			string viewId = Model.View != null ? Model.View.Id : "";
			objectEditItem.Init(application, MemberInfo.MemberTypeInfo, helper.DisplayMember, objectSpace, helper, viewId);
		}
		protected override RepositoryItem CreateRepositoryItem() {
			return new RepositoryItemObjectEdit();
		}
		protected override void ReadValueCore() {
			base.ReadValueCore();
			Control.UpdateText();
		}
		protected override bool IsMemberSetterRequired() {
			return false;
		}
		public ObjectPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) {
			ImmediatePostData = true;
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			if(helper == null) {
				helper = new ObjectEditorHelper(MemberInfo.MemberTypeInfo, Model);
			}
			this.application = application;
			this.objectSpace = objectSpace;
		}
		public new ObjectEdit Control {
			get { return (ObjectEdit)base.Control; }
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
		#region ISupportViewShowing Members
		private event EventHandler<EventArgs> viewShowingNotification;
		event EventHandler<EventArgs> ISupportViewShowing.ViewShowingNotification {
			add { viewShowingNotification += value; }
			remove { viewShowingNotification -= value; }
		}
		#endregion
	}
	public class RepositoryItemObjectEdit : RepositoryItemButtonEdit {
		private ObjectEditorHelper helper;
		private XafApplication application;
		private ITypeInfo editValueTypeInfo;
		private IMemberInfo defaultMember;
		private IObjectSpace objectSpace;
		private string viewId;
		protected internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(ObjectEdit),
					typeof(RepositoryItemObjectEdit), typeof(ButtonEditViewInfo),
					new ButtonEditPainter(), true, EditImageIndexes.ButtonEdit, typeof(DevExpress.Accessibility.ButtonEditAccessible)));
			}
		}
		protected internal static string EditorName {
			get { return typeof(ObjectEdit).Name; }
		}
		protected override bool AllowParseEditValue {
			get { return false; }
		}
		static RepositoryItemObjectEdit() {
			RepositoryItemObjectEdit.Register();
		}
		public RepositoryItemObjectEdit()
			: base() {
			ReadOnly = false;
			TextEditStyle = TextEditStyles.DisableTextEditor;
		}
		public override string GetDisplayText(DevExpress.Utils.FormatInfo format, object editValue) {
			string result = NullText;
			if(helper != null) {
				result = helper.GetDisplayText(editValue, NullText, format.FormatString);
			}
			return result;
		}
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			RepositoryItemObjectEdit sourceItem = (RepositoryItemObjectEdit)item;
			Init(sourceItem.application, sourceItem.EditValueTypeInfo, sourceItem.DefaultMember, sourceItem.objectSpace, sourceItem.helper);
		}
		public void Init(XafApplication application, Type editValueType, string defaultMemberName, IObjectSpace objectSpace, ObjectEditorHelper helper) {
			ITypeInfo editValueTypeInfo = XafTypesInfo.Instance.FindTypeInfo(editValueType);
			Init(application, editValueTypeInfo, editValueTypeInfo.FindMember(defaultMemberName), objectSpace, helper, "");
		}
		public void Init(XafApplication application, ITypeInfo editValueTypeInfo, IMemberInfo defaultMember, IObjectSpace objectSpace, ObjectEditorHelper helper) {
			Init(application, editValueTypeInfo, defaultMember, objectSpace, helper, "");
		}
		public void Init(XafApplication application, ITypeInfo editValueTypeInfo, IMemberInfo defaultMember, IObjectSpace objectSpace, ObjectEditorHelper helper, string viewId) {
			this.application = application;
			this.editValueTypeInfo = editValueTypeInfo;
			this.defaultMember = defaultMember;
			this.objectSpace = objectSpace;
			this.helper = helper;
			this.viewId = viewId;
		}
		public override string EditorTypeName {
			get { return EditorName; }
		}
		public XafApplication Application {
			get { return application; }
		}
		public ITypeInfo EditValueTypeInfo {
			get { return editValueTypeInfo; }
		}
		public IMemberInfo DefaultMember {
			get { return defaultMember; }
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		public string ViewId {
			get { return viewId; }
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class ObjectEdit : ButtonEdit, IGridInplaceEdit {
		private object gridEditingObject;
		private DetailView detailView;
		public event EventHandler ShowPopupWindow;
		private void ObjectSpace_Committed(object sender, EventArgs e) {
			EditValue = null;
			EditValue = Properties.ObjectSpace.GetObject(detailView.CurrentObject);
			IsModified = true;
			UpdateMaskBoxDisplayText();
			RaiseEditValueChanged();
		}
		private void showObjectAction_CustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs args) {
			args.DialogController.Cancelling += new EventHandler(DialogController_Cancelling);
			detailView = ObjectEditorHelper.CreateDetailView(Properties.Application, Properties.ObjectSpace, EditValue, Properties.EditValueTypeInfo.Type, !Properties.ReadOnly, Properties.ViewId);
			args.View = detailView;
			detailView.ObjectSpace.Committed += new EventHandler(ObjectSpace_Committed);
			OnShowPopupWindow();
		}
		private void DialogController_Cancelling(object sender, EventArgs e) {
			DialogController controller = ((DialogController)sender);
			controller.Window.GetController<WinModificationsController>().ModificationsHandlingMode = ModificationsHandlingMode.AutoRollback;
			controller.CanCloseWindow = true;
		}
		private void showObjectAction_Execute(Object sender, PopupWindowShowActionExecuteEventArgs args) {
			args.PopupWindow.View.ObjectSpace.CommitChanges();
		}
		private void ShowObject() {
			PopupWindowShowAction showObjectAction = new PopupWindowShowAction(null, "ShowObject", "");
			try {
				showObjectAction.Application = Properties.Application;
				showObjectAction.IsModal = true;
				showObjectAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(showObjectAction_CustomizePopupWindowParams);
				showObjectAction.Execute += new PopupWindowShowActionExecuteEventHandler(showObjectAction_Execute);
				PopupWindowShowActionHelper helper = new PopupWindowShowActionHelper(showObjectAction);
				helper.ShowPopupWindow();
			}
			finally {
				showObjectAction.Dispose();
			}
		}
		protected internal void UpdateText() {
			UpdateDisplayText();
			Invalidate();
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			base.OnEditorKeyDown(e);
			if(e.KeyData == Keys.Space) {
				ShowObject();
			}
		}
		protected override void OnClickButton(EditorButtonObjectInfoArgs buttonInfo) {
			base.OnClickButton(buttonInfo);
			if(buttonInfo.Button.IsDefaultButton) {
				ShowObject();
			}
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			EditHitInfo info = this.ViewInfo.CalcHitInfo(new Point(e.X, e.Y));
			if(info.HitTest == EditHitTest.None || info.HitTest == EditHitTest.MaskBox){
				ShowObject();
			}
		}
		protected virtual void OnShowPopupWindow() {
			if (ShowPopupWindow != null) {
				ShowPopupWindow(this, EventArgs.Empty);
			}
		}
		static ObjectEdit() {
			RepositoryItemObjectEdit.Register();
		}
		public ObjectEdit() : base() { }
		public override string EditorTypeName {
			get { return RepositoryItemObjectEdit.EditorName; }
		}
		public override object EditValue {
			get { return base.EditValue; }
			set {
				if((value != null) && (value != DBNull.Value) && !Properties.EditValueTypeInfo.Type.IsAssignableFrom(value.GetType())) {
					throw new InvalidCastException(string.Format(
						SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToCast),
						value.GetType(), Properties.EditValueTypeInfo.Type));
				}
				base.EditValue = value;
			}
		}
		public new RepositoryItemObjectEdit Properties {
			get { return (RepositoryItemObjectEdit)base.Properties; }
		}
		public object EditingObject {
			get { return BindingHelper.GetEditingObject(this); }
		}
		object IGridInplaceEdit.GridEditingObject {
			get { return gridEditingObject; }
			set { gridEditingObject = value; }
		}
	}
}
