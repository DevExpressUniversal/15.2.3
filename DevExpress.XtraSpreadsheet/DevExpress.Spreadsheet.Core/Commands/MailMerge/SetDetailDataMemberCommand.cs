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
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region SetDetailDataMemberCommand
	public class SetDetailDataMemberCommand :SpreadsheetCommand {
		public SetDetailDataMemberCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MailMergeSetDetailDataMember; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeSetDetailDataMemberCommandDescription; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeSetDetailDataMemberCommand; } }
		public override string ImageName { get { return "SetDetailDataMember"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<List<string>> valueBasedState = state as IValueBasedCommandUIState<List<string>>;
				if (valueBasedState == null)
					return;
				DataMemberEditorViewModel viewModel = new DataMemberEditorViewModel(Control, valueBasedState.Value);
				if (InnerControl.AllowShowingForms)
					Control.ShowDataMemberEditorForm(viewModel);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		public void ApplyChanges(List<string> newDataMembers) {
			DocumentModel.BeginUpdate();
			try {
				for (int i = 0; i < newDataMembers.Count; i++)
					SetDetailMember(MailMergeDefinedNames.DetailDataMember + i.ToString(), newDataMembers[i]);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<List<string>> state = new DefaultValueBasedCommandUIState<List<string>>();
			MailMergeOptions options = new MailMergeOptions(DocumentModel);
			state.Value = options.DataMembers;
			return state;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			MailMergeOptions options = new MailMergeOptions(DocumentModel);
			state.Enabled = options.DataMembers.Count > 0;
		}
		void SetDetailMember(string definedName, string dataMember) {
			ActiveSheet.DefinedNames[definedName].SetReference(string.Format("\"{0}\"", dataMember));
		}
	}
	#endregion
}
