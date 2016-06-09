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
	#region CollapseExpandGroupCommandBase
	public abstract class CollapseExpandGroupCommandBase : SpreadsheetMenuItemSimpleCommand {
		#region fields
		GroupItem group;
		#endregion
		protected CollapseExpandGroupCommandBase(ISpreadsheetControl control, GroupItem group)
			: base(control) {
			this.group = group;
		}
		#region Properties
		protected abstract DocumentCapability BehaviourOption { get; }
		protected abstract bool Collapse { get; }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			if (state.Enabled) {
				ApplyCommandRestrictionOnEditableControl(state, BehaviourOption);
				if (state.Enabled) {
					ApplyActiveSheetProtection(state);
					if (state.Enabled)
						base.ForceExecute(state);
					else
						Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_GroupingNeedUnprotectSheet));
				}
			}
		}
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdateFromUI();
			try {
				CollapseExpandGroupModelCommandBase command = CollapseExpandGroupModelCommandBase.CreateInstance(group.RowGroup, ActiveSheet, group.Start, group.End, group.ButtonPosition);
				command.Collapse = Collapse;
				command.Execute();
				ScrollToGroupStartPosition(group.RowGroup, group.Start);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		void ScrollToGroupStartPosition(bool isRowGroup, int startGroupIndex) {
			if (Collapse || startGroupIndex >= GetFirstVisibleIndex(isRowGroup))
				return;
			if (isRowGroup)
				ActiveSheet.ScrollToRow(startGroupIndex);
			else
				ActiveSheet.ScrollToColumn(startGroupIndex);
		}
		int GetFirstVisibleIndex(bool isRowGroup) {
			CellPosition unfrozenTopLeftPositon = ActiveSheet.ActiveView.ScrolledTopLeftCell;
			return isRowGroup ? unfrozenTopLeftPositon.Row : unfrozenTopLeftPositon.Column;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
		protected internal override void ApplyActiveSheetProtection(ICommandUIState state) {
			if (Options.InnerBehavior.Group.CollapseExpandOnProtectedSheetAllowed)
				return;
			base.ApplyActiveSheetProtection(state);
		}
	}
	#endregion
	#region CollapseGroupCommand
	public class CollapseGroupCommand : CollapseExpandGroupCommandBase {
		public CollapseGroupCommand(ISpreadsheetControl control, GroupItem group)
			: base(control, group) {
		}
		#region Properties
		protected override DocumentCapability BehaviourOption { get { return Options.InnerBehavior.Group.Collapse; } }
		protected override bool Collapse { get { return true; } }
		#endregion
	}
	#endregion
	#region ExpandGroupCommand
	public class ExpandGroupCommand : CollapseExpandGroupCommandBase {
		public ExpandGroupCommand(ISpreadsheetControl control, GroupItem group)
			: base(control, group) {
		}
		#region Properties
		protected override DocumentCapability BehaviourOption { get { return Options.InnerBehavior.Group.Expand; } }
		protected override bool Collapse { get { return false; } }
		#endregion
	}
	#endregion
	#region OutlineLevelCommand
	public class OutlineLevelCommand : SpreadsheetMenuItemSimpleCommand {
		#region fields
		int targetLevel;
		bool isRow;
		#endregion
		public OutlineLevelCommand(ISpreadsheetControl control, int targetLevel, bool isRow)
			: base(control) {
			this.targetLevel = targetLevel;
			this.isRow = isRow;
		}
		public override void ForceExecute(ICommandUIState state) {
			if (state.Enabled) {
				ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Group.Collapse);
				if (!state.Enabled)
					return;
				ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Group.Expand);
				if (!state.Enabled)
					return;
				ApplyActiveSheetProtection(state);
				if (state.Enabled)
					base.ForceExecute(state);
				else
					Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_GroupingNeedUnprotectSheet));
			}
		}
		protected internal override void ExecuteCore() {
			OutlineLevelCommandBase command = OutlineLevelCommandBase.CreateInstance(isRow, this.DocumentModel.ActiveSheet, targetLevel);
			command.Execute();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) { }
	}
	#endregion
}
