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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxStringPropertyEditor : ASPxPropertyEditor, ITestable {
		private int rowCount;
		private string predefinedValues;
		protected virtual void SetupTextBox(ASPxTextBox textBox) {
			textBox.Password = IsPassword;
			if(!string.IsNullOrEmpty(EditMask) && EditMaskType != EditMaskType.RegEx) {
				textBox.MaskSettings.Mask = EditMask;
				textBox.MaskSettings.ShowHints = true;
				textBox.MaskSettings.ErrorText = UserVisibleExceptionLocalizer.GetExceptionMessage(UserVisibleExceptionId.MaskValidationErrorMessage);
			}
			textBox.MaxLength = MaxLength;
		}
		protected virtual void SetupComboBox(ASPxComboBox comboBox) {
			comboBox.DropDownStyle = DropDownStyle.DropDown;
			comboBox.MaxLength = MaxLength;
			foreach(string item in PredefinedValuesEditorHelper.CreatePredefinedValuesFromString(predefinedValues)) {
				comboBox.Items.Add(item);
			}
		}
		protected override void ApplyReadOnly() {
			base.ApplyReadOnly();
			if(Editor is ASPxMemo) {
				((ASPxMemo)Editor).Enabled = true;
			}
		}
		protected internal override IJScriptTestControl GetEditorTestControlImpl() {
			if(Editor is ASPxMemo) {
				return new JSASPxMemoTestControl();
			}
			else if(Editor is ASPxTextBox) {
				return new JSASPxTextBoxTestControl();
			}
			else {
				return new JSASPxComboBoxTestControl();
			}
		}
		protected override WebControl CreateEditModeControlCore() {
			ASPxTextEdit result = null;
			if(!string.IsNullOrEmpty(predefinedValues)) {
				ASPxComboBox comboBox = RenderHelper.CreateASPxComboBox();
				SetupComboBox(comboBox);
				result = comboBox;
			}
			else if(rowCount <= 1) {
				ASPxTextBox textBox = RenderHelper.CreateASPxTextBox();
				SetupTextBox(textBox);
				result = textBox;
			}
			else {
				ASPxMemo memo = RenderHelper.CreateASPxMemo();
				SetupASPxMemo(memo);
				result = memo;
			}
			result.TextChanged += new EventHandler(EditValueChangedHandler);
			return result;
		}
		protected override string GetPropertyDisplayValue() {
			return GetFormattedValue();
		}
		protected virtual void SetupASPxMemo(ASPxMemo memo) {
			memo.Rows = RowCount;
			memo.MaxLength = MaxLength;
		}
		public ASPxStringPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			rowCount = Model.RowCount;
			predefinedValues = model.PredefinedValues;
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(Editor != null) {
				Editor.TextChanged -= new EventHandler(EditValueChangedHandler);
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public new ASPxTextEdit Editor {
			get { return (ASPxTextEdit)base.Editor; }
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
		public int RowCount {
			get { return rowCount; }
			set { rowCount = value; }
		}
	}
}
