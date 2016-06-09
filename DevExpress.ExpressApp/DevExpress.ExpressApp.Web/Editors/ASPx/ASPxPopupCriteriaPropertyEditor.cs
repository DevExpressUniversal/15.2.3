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
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxPopupCriteriaPropertyEditor : ASPxPropertyEditor, IComplexViewItem, IDependentPropertyEditor, ITestable {
		private CriteriaPropertyEditorHelper helper;
		private XafApplication application;
		private PopupWindowShowAction showFilterEditorAction;
		private bool buttonEditTextEnabled = true;
		private void showFilterEditorAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
			CriteriaProvider providerObject = new CriteriaProvider(helper.GetCriteriaObjectType(CurrentObject), (string)PropertyValue);
			IObjectSpace dialogObjectSpace = application.CreateObjectSpace(typeof(CriteriaProvider));
			e.View = application.CreateDetailView(dialogObjectSpace, providerObject);
			e.View.Caption = CaptionHelper.GetLocalizedText("Captions", "EditCriteria");
			((DetailView)e.View).ViewEditMode = ViewEditMode.Edit;
		}
		private void showFilterEditorAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
			IList<ASPxCriteriaPropertyEditor> items = ((DetailView)e.PopupWindow.View).GetItems<ASPxCriteriaPropertyEditor>();
			ASPxCriteriaPropertyEditor editor = (items.Count > 0) ? items[0] : null;
			Guard.ArgumentNotNull(editor, "editor");
			editor.WriteValue();
			PropertyValue = ((CriteriaProvider)((DetailView)e.PopupWindow.View).CurrentObject).FilterString;
			string result = (string)PropertyValue;
			((PopupWindow)e.PopupWindow).ClosureScript = "if(window.dialogOpener) window.dialogOpener.resultObject = '" + (result != null ? result.Replace("'", "\\'") : "") + "';";
		}
		private void buttonEdit_Load(object sender, EventArgs e) {
			ASPxButtonEdit editor = ((ASPxButtonEdit)sender);
			editor.Load -= new EventHandler(buttonEdit_Load);
			EditorWithButtonEditHelper.AssignButtonClickScript(editor, application as WebApplication, showFilterEditorAction);
		}
		private void SetButtonEditTextEnabled(ASPxButtonEdit buttonEdit, bool value) {
			if(buttonEditTextEnabled != value) {
				buttonEditTextEnabled = value;
				if(value) {
					buttonEdit.CssClass.Replace(" dxDisabled", "");
				}
				else {
					buttonEdit.CssClass += " dxDisabled";
				}
			}
		}
		protected internal override IJScriptTestControl GetEditorTestControlImpl() {
			return new JSASPxPopupCriteriaPropertyEditorTestControl();
		}
		protected override WebControl CreateEditModeControlCore() {
			if(showFilterEditorAction == null) {
				showFilterEditorAction = new PopupWindowShowAction(null, "ShowFilterEditor", PredefinedCategory.Unspecified.ToString());
				showFilterEditorAction.Execute += new PopupWindowShowActionExecuteEventHandler(showFilterEditorAction_Execute);
				showFilterEditorAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(showFilterEditorAction_CustomizePopupWindowParams);
				showFilterEditorAction.Application = application;
			}
			ASPxButtonEdit buttonEdit = new ASPxButtonEdit();
			SetButtonEditTextEnabled(buttonEdit, false);
			buttonEdit.EnableClientSideAPI = true;
			buttonEdit.Load += new EventHandler(buttonEdit_Load);
			buttonEdit.Buttons.Clear();
			buttonEdit.Buttons.Add("");
			ASPxImageHelper.SetImageProperties(buttonEdit.Buttons[0].Image, "Editor_Edit");
			if(buttonEdit.Buttons[0].Image.IsEmpty) {
				buttonEdit.Buttons[0].Text = "Edit";
			}
			buttonEdit.Enabled = true;
			buttonEdit.ReadOnly = true;
			return buttonEdit;
		}
		protected override void ApplyReadOnly() {
			if(Editor != null) {
				SetButtonEditTextEnabled(Editor, AllowEdit);
				Editor.Buttons[0].Enabled = AllowEdit;
			}
		}
		protected override void Dispose(bool disposing) {
			if(showFilterEditorAction != null) {
				showFilterEditorAction.Execute -= new PopupWindowShowActionExecuteEventHandler(showFilterEditorAction_Execute);
				showFilterEditorAction.CustomizePopupWindowParams -= new CustomizePopupWindowParamsEventHandler(showFilterEditorAction_CustomizePopupWindowParams);
				showFilterEditorAction.Dispose();
				showFilterEditorAction = null;
			}
			helper = null;
			base.Dispose(disposing);
		}
		protected override void ReadEditModeValueCore() {
			base.ReadEditModeValueCore();
			AllowEdit.SetItemValue(TheDataTypeIsDefined, helper.GetCriteriaObjectType(CurrentObject) != null);
		}
		public ASPxPopupCriteriaPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			this.application = application;
			this.helper = new CriteriaPropertyEditorHelper(MemberInfo);
		}
		public IList<string> MasterProperties {
			get { return helper.MasterProperties; }
		}
		public new ASPxButtonEdit Editor {
			get { return (ASPxButtonEdit)base.Editor; }
		}
	}
}
