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
using DevExpress.Accessibility;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.ExpressApp.Win.Editors {
	public class PopupCriteriaPropertyEditor : DXPropertyEditor, IComplexViewItem, IDependentPropertyEditor {
		private CriteriaPropertyEditorHelper helper;
		private XafApplication application;
		protected override void OnControlCreated() {
			base.OnControlCreated();
			RefreshControlDatasource();
		}
		protected override void OnCurrentObjectChanged() {
			base.OnCurrentObjectChanged();
			RefreshControlDatasource();
		}
		private void RefreshControlDatasource() {
			if(Control != null) {
				Control.GridEditingObject = CurrentObject;
			}
		}
		protected override object CreateControlCore() {
			return new PopupCriteriaEdit();
		}
		protected override void SetupRepositoryItem(RepositoryItem item) {
			base.SetupRepositoryItem(item);
			((RepositoryItemPopupCriteriaEdit)item).Init(helper, application);
		}
		protected override RepositoryItem CreateRepositoryItem() {
			return new RepositoryItemPopupCriteriaEdit();
		}
		public PopupCriteriaPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			this.application = application;
			helper = new CriteriaPropertyEditorHelper(MemberInfo);
		}
		public new PopupCriteriaEdit Control {
			get { return (PopupCriteriaEdit)base.Control; }
		}
		public IList<string> MasterProperties {
			get { return helper.MasterProperties; }
		}
	}
	public class RepositoryItemPopupCriteriaEdit : RepositoryItemButtonEdit {
		private CriteriaPropertyEditorHelper helper;
		private XafApplication application;
		protected internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(PopupCriteriaEdit),
					typeof(RepositoryItemPopupCriteriaEdit), typeof(ButtonEditViewInfo),
					new ButtonEditPainter(), true, EditImageIndexes.ButtonEdit, typeof(ButtonEditAccessible)));
			}
		}
		protected internal static string EditorName {
			get { return typeof(PopupCriteriaEdit).Name; }
		}
		protected override bool IsButtonEnabled(EditorButton button) {
			if(ReadOnly) {
				return false;
			}
			return base.IsButtonEnabled(button);
		}
		static RepositoryItemPopupCriteriaEdit() {
			RepositoryItemPopupCriteriaEdit.Register();
		}
		public RepositoryItemPopupCriteriaEdit() {
			TextEditStyle = TextEditStyles.DisableTextEditor;
		}
		public void Init(CriteriaPropertyEditorHelper helper, XafApplication application) {
			Guard.ArgumentNotNull(helper, "helper");
			Guard.ArgumentNotNull(application, "application");
			this.helper = helper;
			this.application = application;
		}
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			RepositoryItemPopupCriteriaEdit sourceItem = (RepositoryItemPopupCriteriaEdit)item;
			Init(sourceItem.helper, sourceItem.application);
		}
		public override string EditorTypeName {
			get { return EditorName; }
		}
		public CriteriaPropertyEditorHelper Helper {
			get { return helper; }
		}
		public XafApplication Application { 
			get { return application; } 
		}
	}
	[ToolboxItem(false)]
	public class PopupCriteriaEdit : ButtonEdit, IGridInplaceEdit {
		protected override void OnClickButton(EditorButtonObjectInfoArgs buttonInfo) {
			base.OnClickButton(buttonInfo);
			PopupWindowShowAction showFilterAction = new PopupWindowShowAction(null, "FilterPopup", string.Empty);
			showFilterAction.Application = Properties.Application;
			showFilterAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(showFilterAction_CustomizePopupWindowParams);
			PopupWindowShowActionHelper helper = new PopupWindowShowActionHelper(showFilterAction);
			helper.ShowPopupWindow();
		}
		private void showFilterAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
			CriteriaProvider providerObject = new CriteriaProvider(Properties.Helper.GetCriteriaObjectType(GridEditingObject), (string)EditValue);
			IObjectSpace dialogObjectSpace = Properties.Application.CreateObjectSpace(providerObject.FilteredObjectsType);
			e.View = Properties.Application.CreateDetailView(dialogObjectSpace, providerObject);
			e.View.Caption = CaptionHelper.GetLocalizedText("Captions", "EditCriteria");
			e.DialogController.Accepting += new EventHandler<DialogControllerAcceptingEventArgs>(DialogController_Accepting);
		}
		private void DialogController_Accepting(object sender, DialogControllerAcceptingEventArgs e) {
			EditValue = ((CriteriaProvider)e.AcceptActionArgs.CurrentObject).FilterString;
		}
		static PopupCriteriaEdit() { 
			RepositoryItemPopupCriteriaEdit.Register(); 
		}
		public override string EditorTypeName {
			get { return RepositoryItemPopupCriteriaEdit.EditorName; }
		}
		public new RepositoryItemPopupCriteriaEdit Properties {
			get { return (RepositoryItemPopupCriteriaEdit)base.Properties; }
		}
		public object GridEditingObject { get; set; }
	}
}
