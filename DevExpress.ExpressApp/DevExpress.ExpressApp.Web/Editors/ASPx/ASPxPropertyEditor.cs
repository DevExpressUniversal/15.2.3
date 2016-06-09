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
using System.ComponentModel;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Web.Utils;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public abstract class ASPxPropertyEditor : WebPropertyEditor, ITestable {
		string SetImmediatePostDataCompanionScriptKey = Guid.NewGuid().ToString();
		string SetImmediatePostDataScriptKey = Guid.NewGuid().ToString();
		private void Editor_Load(object sender, EventArgs e) {
			ASPxEditor.Load -= new EventHandler(Editor_Load);
			ASPxEditor.DataBind();
		}
		protected ASPxEditBase ASPxEditor {
			get { return base.Editor as ASPxEditBase; }
		}
		protected override void SetImmediatePostDataCompanionScript(string script) {
			base.SetImmediatePostDataCompanionScript(script);
			if(ASPxEditor != null) { 
				ClientSideEventsHelper.AssignClientHandlerSafe(ASPxEditor, "GotFocus", script, SetImmediatePostDataCompanionScriptKey);
			}
		}
		protected override void SetImmediatePostDataScript(string script) {
			base.SetImmediatePostDataScript(script);
			if(ASPxEditor != null) {
				ClientSideEventsHelper.AssignClientHandlerSafe(ASPxEditor, "ValueChanged", script, SetImmediatePostDataScriptKey);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected override void SetProcessValueChangedScript(string script) {
			if(ASPxEditor != null) {
				ClientSideEventsHelper.AssignClientHandlerSafe(ASPxEditor, "ValueChanged", script, Guid.NewGuid().ToString());
			}
		}
		protected override void WriteValueCore() {
			if(AllowEdit) {
				base.WriteValueCore();
			}
			else {
			}
		}
		protected override object GetControlValueCore() {
			if(ASPxEditor != null) {
				return ASPxEditor.Value;
			}
			else {
				return null;
			}
		}
		protected override object CreateControlCore() {
			object result = base.CreateControlCore();
			if(ASPxEditor != null) {
				ASPxEditor.Load += new EventHandler(Editor_Load);
				if(ViewEditMode == DevExpress.ExpressApp.Editors.ViewEditMode.Edit) {
					ASPxEdit aspxEdit = ASPxEditor as ASPxEdit;
					if(aspxEdit != null && EditMaskType == DevExpress.ExpressApp.Editors.EditMaskType.RegEx && !string.IsNullOrEmpty(EditMask)) {
						SetupValidationSettings(aspxEdit);
					}
				}
			}
			return result;
		}
		protected virtual void SetupValidationSettings(ASPxEdit aspxEdit) {
			aspxEdit.ValidationSettings.RegularExpression.ValidationExpression = EditMask;
			aspxEdit.ValidationSettings.RegularExpression.ErrorText = UserVisibleExceptionLocalizer.GetExceptionMessage(UserVisibleExceptionId.MaskValidationErrorMessage);
		}
		protected override void ReadEditModeValueCore() {
			if(ASPxEditor != null) {
				ASPxEditor.Value = PropertyValue;
			}
		}
		protected override void ApplyReadOnly() {
			if(ASPxEditor != null) {
				ASPxEditor.ReadOnly = !AllowEdit;
				ASPxEditor.ClientEnabled = AllowEdit;
			}
		}
		protected internal override IJScriptTestControl GetEditorTestControlImpl() {
			return new JSASPxTextBoxTestControl();
		}
		[Obsolete("Use the EditValueChangedHandler method instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected void ExtendedEditValueChangedHandler(object source, EventArgs e) {
			EditValueChangedHandler(source, e);
		}		
		protected ASPxPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
	}
}
