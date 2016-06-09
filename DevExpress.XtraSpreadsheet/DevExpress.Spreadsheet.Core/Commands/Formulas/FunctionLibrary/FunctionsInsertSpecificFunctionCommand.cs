#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FunctionsInsertSpecificFunctionCommand
	public class FunctionsInsertSpecificFunctionCommand : SpreadsheetMenuItemSimpleCommand {
		string functionNameInvariant;
		string functionName;
		public FunctionsInsertSpecificFunctionCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public FunctionsInsertSpecificFunctionCommand(ISpreadsheetControl control, string functionName)
			: base(control) {
			this.FunctionNameInvariant = functionName;
		}
		#region Properties
		public string FunctionName { get { return functionName; } }
		public string FunctionNameInvariant {
			get { return functionNameInvariant; }
			set {
				this.functionNameInvariant = value;
				this.functionName = ObtainFunctionName(functionNameInvariant);
			}
		}
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FunctionsInsertSpecificFunction; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FunctionsInsertSpecificFunctionDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FunctionsInsertSpecificFunction; } }
		#endregion
		string ObtainFunctionName(string invariantFunctionName) {
			return DevExpress.XtraSpreadsheet.Model.FormulaCalculator.GetFunctionName(invariantFunctionName, DocumentModel.DataContext);
		}
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
			if (valueBasedState != null)
				FunctionNameInvariant = valueBasedState.Value;
			if (!String.IsNullOrEmpty(FunctionNameInvariant))
				base.ForceExecute(state);
		}
		protected internal override void ExecuteCore() {
			CheckExecutedAtUIThread();
			if (!InnerControl.TryEditActiveCellContent())
				return;
			InsertFunctionFormulaAndOpenInplaceEditor();
		}
		protected void InsertFunctionFormulaAndOpenInplaceEditor() {
			string value = "=" + FunctionName + "()";
			InnerControl.ActivateCellInplaceEditor(ActiveSheet.Selection.ActiveCell, value, value.Length - 1, CellEditorMode.Edit);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = !ActiveSheet.Selection.IsDrawingSelected && IsContentEditable && !InnerControl.IsCommentInplaceEditorActive;
			state.Visible = true;
			state.Checked = false;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<string> result = new DefaultValueBasedCommandUIState<string>();
			result.Value = FunctionName;
			return result;
		}
	}
	#endregion
}
