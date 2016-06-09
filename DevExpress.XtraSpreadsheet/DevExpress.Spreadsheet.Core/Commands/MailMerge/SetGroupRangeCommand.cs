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
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region SetGroupRangeCommand
	public abstract class SetGroupRangeCommand :SetRangeCommand {
		#region fields
		string groupIndex;
		Dictionary<string, string> definedNames;
		#endregion
		protected SetGroupRangeCommand(ISpreadsheetControl control)
			: base(control) {
			groupIndex = string.Empty;
		}
		#region Properties
		protected string GroupIndex { get { return groupIndex; } }
		protected abstract bool UseHeaderCommand { get; }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<Dictionary<string, string>> valueBasedState = state as IValueBasedCommandUIState<Dictionary<string, string>>;
				if (valueBasedState == null || valueBasedState.Value == null || valueBasedState.Value.Count < 1)
					return;
				if (InnerControl.AllowShowingForms) {
					GroupRangeEditorViewModel viewModel = new GroupRangeEditorViewModel(Control, valueBasedState.Value, UseHeaderCommand);
					Control.ShowGroupRangeEditorForm(viewModel);
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		public void ApplyChanges(string groupDefianedName) {
			if (string.IsNullOrEmpty(groupDefianedName)) {
				groupIndex = null;
				return;
			}
			groupIndex = groupDefianedName.Substring(MailMergeDefinedNames.GroupName.Length);
			ExecuteCore();
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<Dictionary<string, string>> state = new DefaultValueBasedCommandUIState<Dictionary<string, string>>();
			state.Value = GetGroupNames();
			return state;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = GetGroupNames().Count > 0;
			if(!state.Enabled)
				return;
			CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			MailMergeOptions options = new MailMergeOptions(DocumentModel);
			List<GroupInfo> groupInfo = options.GetGroupInfoFromPosition(selectedRange.TopLeft);
			foreach (GroupInfo info in groupInfo) {
				if (info.Header != null && info.Header.Intersects(selectedRange)) {
					state.Enabled = false;
					break;
				}
				if (info.Footer != null && info.Footer.Intersects(selectedRange)) {
					state.Enabled = false;
					break;
				}
			}
		}
		Dictionary<string, string> GetGroupNames() {
			CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			MailMergeOptions options = new MailMergeOptions(DocumentModel);
			List<GroupInfo> groupInfo = options.GetGroupInfoFromPosition(selectedRange.TopLeft);
			definedNames = new Dictionary<string, string>();
			if (groupInfo != null)
				foreach (GroupInfo info in groupInfo)
					if (!GroupHasRange(info)) {
						string key = string.Format("{0}({1})", this.DocumentModel.GetMailMergeDisplayName(info.FieldName, false), info.SortOrder);
						if (definedNames.ContainsKey(key))
							continue;
						definedNames.Add(key, info.DefinedName);
					}
			return definedNames;
		}
		protected override void SetFunctionalComment(DefinedName definedName) {
			if (definedName != null)
				definedName.Comment = MailMergeDefinedNames.GroupName + groupIndex;
		}
		protected abstract bool GroupHasRange(GroupInfo groupInfo);
	}
	#endregion
}
