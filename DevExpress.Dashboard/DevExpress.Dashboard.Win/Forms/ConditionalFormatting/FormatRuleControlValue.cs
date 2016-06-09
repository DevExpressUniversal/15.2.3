#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.ComponentModel;
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class FormatRuleControlValue : FormatRuleControlStyleBase, IFormatRuleControlValueView {
		BaseEdit valueEditor;
		BaseEdit value2Editor;
		DataFieldType dataType;
		protected override bool IsValid {
			get {
				if(ValueEditor.EditValue == null ||
					object.Equals(ValueEditor.EditValue, string.Empty) ||
					(Value2Editor != null && (Value2Editor.EditValue == null || object.Equals(Value2Editor.EditValue, string.Empty))))
					return false;
				object obj = null;
				try {
					obj = ConvertToType(ValueEditor.EditValue, true);
				} catch {
					return false;
				}
				if(Value2Editor == null)
					return obj != null;
				try {
					obj = ConvertToType(Value2Editor.EditValue, true);
				} catch {
					return false;
				}
				return obj != null;
			} 
		}
		protected BaseEdit ValueEditor { get { return valueEditor; } set { valueEditor = value; } }
		protected BaseEdit Value2Editor { get { return value2Editor; } set { value2Editor = value; } }
		protected DataFieldType DataType { get { return dataType; } set { dataType = value; } }
		public FormatRuleControlValue()
			: base() {
			InitializeComponent();
		}
		protected override void Initialize(IFormatRuleControlViewInitializationContext initializationContext) {
			base.Initialize(initializationContext);
			FillValuePanel(initializationContext);
		}
		protected virtual void FillValuePanel(IFormatRuleControlViewInitializationContext initializationContext) {
			IFormatRuleViewValueContext context = (IFormatRuleViewValueContext)initializationContext;
			dataType = context.DataType;
			ValueEditor = CreateValueEditor("valueEditor", dataType, context.DateTimeGroupInterval);
			LayoutControlItem lciValue = ValuePanelGroup.AddItem(string.Empty, ValueEditor);
			lciValue.Name = "lciValue";
			lciValue.TextVisible = false;
			lciValue.Padding = new XtraLayout.Utils.Padding(0);
			if(context.IsValue2Required) {
				lciValue.TextVisible = false;
				Value2Editor = CreateValueEditor("value2Editor", dataType, context.DateTimeGroupInterval);
				LayoutControlItem lciValue2 = ValuePanelGroup.AddItem(DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionBetweenAndCaption), Value2Editor, lciValue, XtraLayout.Utils.InsertType.Right);
				lciValue2.Name = "lciValue2";
				lciValue2.TextToControlDistance = 6;
				lciValue2.TextAlignMode = TextAlignModeItem.AutoSize;
				lciValue2.Padding = new XtraLayout.Utils.Padding(lciValue2.TextToControlDistance, 0, 0, 0);
			}
		}
		protected BaseEdit CreateValueEditor(string name, DataFieldType dataType, DateTimeGroupInterval groupInterval) {
			BaseEdit editor = null;
			if(dataType == DataFieldType.DateTime) {
				DateEdit dateEdit = new DateEdit();
				dateEdit.Properties.VistaEditTime = DateTimeHelper.GetEditTime(groupInterval);
				dateEdit.Properties.VistaCalendarViewStyle = DateTimeHelper.GetCalendarStyle(groupInterval);
				editor = dateEdit;
			} else {
				TextEdit textEdit = new TextEdit();
				textEdit.Properties.NullValuePromptShowForEmptyValue = true;
				textEdit.Properties.ShowNullValuePromptWhenFocused = true;				
				textEdit.Properties.NullValuePrompt = DashboardWinLocalizer.GetString(DashboardWinStringId.EditorEmptyEnter);
				editor = textEdit;
			}
			editor.Name = name;
			editor.MinimumSize = new Size(112, 0);
			editor.EditValueChanged += OnStateChanged;
			return editor;
		}
		object ConvertToType(object value, bool throwException) {
			return DashboardCommon.Native.Helper.ConvertToType(value, dataType, throwException);
		}
		#region IFormatRuleControlValueView Members       
		object IFormatRuleControlValueView.Value {
			get { return ConvertToType(ValueEditor.EditValue, false); }
			set { ValueEditor.EditValue = value; }
		}
		object IFormatRuleControlValueView.Value2 {
			get { return Value2Editor != null ? ConvertToType(Value2Editor.EditValue, false) : null; }
			set {
				if(Value2Editor != null)
					Value2Editor.EditValue = value;
			}
		}
		#endregion
	}
}
