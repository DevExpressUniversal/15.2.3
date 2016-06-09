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
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxObjectPropertyEditor : ASPxObjectPropertyEditorBase, ITestable, ISupportViewShowing, IObjectPropertyEditor {
		private ObjectEditorHelper helper;
		private PopupWindowShowAction objectWindowAction;
		private ASPxButtonEdit control;
		private bool buttonEditTextEnabled = true;
		private void objectWindowAction_OnCustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs args) {
			string viewId = Model.View != null ? Model.View.Id : "";
			args.View = ObjectEditorHelper.CreateDetailView(application, objectSpace, PropertyValue, MemberInfo.MemberType, AllowEdit, viewId);
			args.View.ObjectSpace.SetModified(((DetailView)args.View).CurrentObject);
			OnViewShowingNotification();
		}
		private void OnViewShowingNotification() {
			if(viewShowingNotification != null) {
				viewShowingNotification(this, EventArgs.Empty);
			}
		}
		private void objectWindowAction_OnExecute(Object sender, PopupWindowShowActionExecuteEventArgs args) {
			if(args.PopupWindow.View.AllowEdit) {
				args.PopupWindow.View.ObjectSpace.CommitChanges();
				PropertyValue = objectSpace.GetObject(((ObjectView)args.PopupWindow.View).CurrentObject);
			}
			string displayValue = GetPropertyDisplayValue();
			((PopupWindow)args.PopupWindow).ClosureScript = "if(window.dialogOpener) window.dialogOpener.resultObject = '" + (displayValue != null ? displayValue.Replace("'", "\\'") : "") + "';";
		}
		private void SetButtonEditTextEnabled(bool value) {
			if(buttonEditTextEnabled != value) {
				buttonEditTextEnabled = value;
				if(value) {
					control.CssClass.Replace(" dxDisabled", "");
				}
				else {
					control.CssClass += " dxDisabled";
				}
			}
		}
		private void buttonEdit_Load(object sender, EventArgs e) {
			ASPxButtonEdit editor = (ASPxButtonEdit)sender;
			editor.Load -= new EventHandler(buttonEdit_Load);
			EditorWithButtonEditHelper.AssignButtonClickScript(editor, application, objectWindowAction);
		}
		protected override object GetControlValueCore() {
			return MemberInfo.GetValue(CurrentObject);
		}
		protected override bool IsMemberSetterRequired() {
			return false;
		}
		protected override WebControl CreateEditModeControlCore() {
			if(objectWindowAction == null) {
				objectWindowAction = new PopupWindowShowAction(null, "ShowObjectDetailViewPopup", PredefinedCategory.Unspecified.ToString());
				objectWindowAction.Execute += new PopupWindowShowActionExecuteEventHandler(objectWindowAction_OnExecute);
				objectWindowAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(objectWindowAction_OnCustomizePopupWindowParams);
				objectWindowAction.Application = application;
			}
			control = new ASPxButtonEdit();
			control.EnableClientSideAPI = true;
			control.Load += new EventHandler(buttonEdit_Load);
			control.Buttons.Clear();
			control.Buttons.Add("");
			ASPxImageHelper.SetImageProperties(control.Buttons[0].Image, "Editor_Edit");
			if(control.Buttons[0].Image.IsEmpty) {
				control.Buttons[0].Text = "Edit";
			}
			control.Enabled = true;
			control.ReadOnly = true;
			return control;
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(objectWindowAction != null) {
						objectWindowAction.Execute -= new PopupWindowShowActionExecuteEventHandler(objectWindowAction_OnExecute);
						objectWindowAction.CustomizePopupWindowParams -= new CustomizePopupWindowParamsEventHandler(objectWindowAction_OnCustomizePopupWindowParams);
						objectWindowAction.Dispose();
						objectWindowAction = null;
					}
					if(control != null) {
						control.Buttons.Clear();
						control.Dispose();
						control = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal override IJScriptTestControl GetEditorTestControlImpl() {
			return new JSASPxButtonEditTestControl();
		}
		protected override void ApplyReadOnly() {
			if(Editor != null && Editor.Controls.Count > 0) {
				SetButtonEditTextEnabled(AllowEdit);
				control.Buttons[0].Enabled = PropertyValue != null || AllowEdit;
			}
		}
		protected override void ReadEditModeValueCore() {
			control.Value = GetPropertyDisplayValue();
		}
		protected override string GetPropertyDisplayValue() {
			return helper.GetDisplayText(MemberInfo.GetValue(CurrentObject), EmptyValue, DisplayFormat);
		}
		public ASPxObjectPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			skipEditModeDataBind = true;
		}
		public override void Setup(IObjectSpace objectSpace, XafApplication application) {
			base.Setup(objectSpace, application);
			if(helper == null) {
				helper = new ObjectEditorHelper(MemberInfo.MemberTypeInfo, Model);
			}
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(!unwireEventsOnly) {
				control = null;
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		#region ISupportViewShowing Members
		private event EventHandler<EventArgs> viewShowingNotification;
		event EventHandler<EventArgs> ISupportViewShowing.ViewShowingNotification {
			add { viewShowingNotification += value; }
			remove { viewShowingNotification -= value; }
		}
		#endregion
	}
}
