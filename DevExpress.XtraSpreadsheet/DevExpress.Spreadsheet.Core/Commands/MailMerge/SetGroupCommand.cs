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
	#region SetGroupCommand
	public class SetGroupCommand :SetDetailLevelCommand {
		#region static
		static List<string> GetGroupDefinedNames(List<EditGroupInfo> newGroupInfo) {
			List<string> result = new List<string>();
			foreach (EditGroupInfo info in newGroupInfo)
				if (!result.Contains(info.DefinedName))
					result.Add(info.DefinedName);
			return result;
		}
		#endregion
		#region fields
		string dataMember;
		List<EditGroupInfo> editableGroupInfo;
		MailMergeOptions options;
		#endregion 
		public SetGroupCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override bool AllowFullSelectRange { get { return true; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MailMergeSetGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeSetGroupCommandDescription; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeSetGroupCommand; } }
		public override string ImageName { get { return "SortFields"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<List<EditGroupInfo>> valueBasedState = state as IValueBasedCommandUIState<List<EditGroupInfo>>;
				if (valueBasedState == null)
					return;
				if (InnerControl.AllowShowingForms) {
					GroupEditorViewModel viewModel = new GroupEditorViewModel(Control, valueBasedState.Value, dataMember);
					Control.ShowGroupEditorForm(viewModel);
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		public void ApplyChanges(List<EditGroupInfo> newGroupInfo) {
			DocumentModel.BeginUpdateFromUI();
			try {
				CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
				options = new MailMergeOptions(DocumentModel);
				List<GroupInfo> oldGroupInfo = options.GetGroupInfoFromPosition(selectedRange.TopLeft);
				bool changeOrder = ChangeGroupNames(newGroupInfo, oldGroupInfo);
				if (changeOrder) {
					DefinedName definedName = ActiveSheet.DefinedNames[options.GetRangeNameFromPosition(selectedRange.TopLeft)] as DefinedName;
					string comment = string.Empty;
					foreach (string name in GetGroupDefinedNames(newGroupInfo))
						comment += name + ";";
					definedName.Comment = comment;
				}
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			options = new MailMergeOptions(DocumentModel);
			DefaultValueBasedCommandUIState<List<EditGroupInfo>> state = new DefaultValueBasedCommandUIState<List<EditGroupInfo>>();
			dataMember = options.GetFullDataMemberFromPosition(selectedRange.TopLeft);
			editableGroupInfo = options.GetEditableGroupInfo(selectedRange.TopLeft);
			state.Value = editableGroupInfo;
			return state;
		}
		bool ChangeGroupNames(List<EditGroupInfo> newGroupInfo, List<GroupInfo> oldGroupInfo) {
			bool orderChanged = false;
			bool created = false;
			bool deleted = false;
			for (int i = 0; i < newGroupInfo.Count; i++) {
				EditGroupInfo info = newGroupInfo[i];
				if (info.Index != i && !orderChanged)
					orderChanged = true;
				if (info.Index == -1) {
					created = true;
					CreateNewGroupName(info);
				}
				else
					ChangeGroupName(info, oldGroupInfo[info.Index]);
			}
			deleted = oldGroupInfo.Count > newGroupInfo.Count;
			if (!deleted && created && newGroupInfo.Count <= oldGroupInfo.Count)
				deleted = true;
			if (deleted)
				RemoveDeletedGroupNames(newGroupInfo, oldGroupInfo);
			return orderChanged || deleted || created;
		}
		void ChangeGroupName(EditGroupInfo newInfo, GroupInfo oldInfo) {
			DefinedName definedName = ActiveSheet.DefinedNames[oldInfo.DefinedName] as DefinedName;
			if (newInfo.FieldName != oldInfo.FieldName)
				definedName.SetReference(string.Format("\"{0}\"", newInfo.FieldName));
			if (newInfo.SortOrder != oldInfo.SortOrder)
				definedName.Comment = newInfo.SortOrder.ToString();
		}
		void CreateNewGroupName(EditGroupInfo info) {
			GroupInfo groupInfo = options.CreateGroupInfo(string.Format("\"{0}\"", info.FieldName), null, null, info.SortOrder);
			SetSheetDefinedNameValue(groupInfo.DefinedName, groupInfo.FieldName).Comment = info.SortOrder.ToString();
			info.DefinedName = groupInfo.DefinedName;
		}
		void RemoveDeletedGroupNames(List<EditGroupInfo> newGroupInfo, List<GroupInfo> oldGroupInfo) {
			List<string> lostDefinedNames = GetGroupDefinedNames(newGroupInfo);
			foreach (GroupInfo info in oldGroupInfo)
				if (!lostDefinedNames.Contains(info.DefinedName)) {
					ActiveSheet.RemoveDefinedName(info.DefinedName);
					string stringIndex = info.DefinedName.Substring(MailMergeDefinedNames.GroupName.Length);
					ActiveSheet.RemoveDefinedName(MailMergeDefinedNames.GroupHeader + stringIndex);
					ActiveSheet.RemoveDefinedName(MailMergeDefinedNames.GroupFooter + stringIndex);
				}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled = state.Enabled && DocumentModel.MailMergeDataSource != null;
		}
	}
	#endregion
}
