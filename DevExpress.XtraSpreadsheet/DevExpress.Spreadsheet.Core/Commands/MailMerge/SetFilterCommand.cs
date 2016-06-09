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

using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region SetFilterCommand
	public class SetFilterCommand :SetDetailLevelCommand {
		#region fields
		string dataMember;
		#endregion
		public SetFilterCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override bool AllowFullSelectRange { get { return true; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MailMergeSetFilter; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeSetFilterCommandDescription; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeSetFilterCommand; } }
		public override string ImageName { get { return "EditFilter"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
				if (valueBasedState == null)
					return;
				if (InnerControl.AllowShowingForms) {
					FilterEditorViewModel viewModel = new FilterEditorViewModel(Control, valueBasedState.Value, dataMember, DocumentModel.MailMergeParameters.InnerList);
					Control.ShowFilterEditorForm(viewModel);
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		public void ApplyChanges(string newExpression) {
			DocumentModel.BeginUpdateFromUI();
			try {
				FilterInfo info = GetFilterInfoBySelectedPosition(null);
				info.Expression = newExpression;
				SetSheetDefinedNameValue(info.DefinedName, info.ToString());
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<string> state = new DefaultValueBasedCommandUIState<string>();
			MailMergeOptions options = new MailMergeOptions(DocumentModel);
			FilterInfo info = GetFilterInfoBySelectedPosition(options);
			state.Value = info.Expression;
			dataMember = options.GetFullDataMemberFromPosition(ActiveSheet.Selection.SelectedRanges[0].TopLeft);
			return state;
		}
		FilterInfo GetFilterInfoBySelectedPosition(MailMergeOptions options) {
			CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			if(options == null)
				options = new MailMergeOptions(DocumentModel);
			string filterDataMember = options.GetDataMemberFromPosition(selectedRange.TopLeft);
			if (string.IsNullOrEmpty(filterDataMember) && !string.IsNullOrEmpty(DocumentModel.MailMergeDataMember))
				filterDataMember = DocumentModel.MailMergeDataMember;
			FilterInfo info = options.GetFilterInfoByDataMember(filterDataMember);
			if (info == null)
				info = options.CreateFilterInfo(filterDataMember, string.Empty);
			return info;
		}
		protected internal override void ExecuteCore() {
			throw new System.InvalidOperationException();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled = state.Enabled && DocumentModel.MailMergeDataSource != null;
		}
	}
	#endregion
}
