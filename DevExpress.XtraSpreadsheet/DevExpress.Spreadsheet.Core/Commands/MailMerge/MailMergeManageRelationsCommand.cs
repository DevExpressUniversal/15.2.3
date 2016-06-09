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

using System.Collections.Generic;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region MailMergeManageQueriesCommand
	public class MailMergeManageQueriesCommand : SpreadsheetCommand {
		public MailMergeManageQueriesCommand(ISpreadsheetControl control) : base(control) {}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeManageQueriesCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeManageQueriesCommand; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MailMergeManageQueriesCommand; } }
		public override string ImageName { get { return "ManageQueries"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			DocumentModel.BeginUpdateFromUI();
			try {
				if(InnerControl.AllowShowingForms) {
					Control.ShowManageQueriesForm();
				}
			} finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = DocumentModel.MailMergeDataSource is SqlDataSource;
		}
		public void ApplyChanges(List<SqlQuery> oldQueries) {
			SqlDataSource sqlDataSource = DocumentModel.MailMergeDataSource as SqlDataSource;
			if(sqlDataSource == null)
				return;
			ManageQueriesHistoryItem historyItem = new ManageQueriesHistoryItem(DocumentModel, oldQueries);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
	}
	#endregion
	#region MailMergeManageRelationsCommand
	public class MailMergeManageRelationsCommand : SpreadsheetCommand {
		public MailMergeManageRelationsCommand(ISpreadsheetControl control) : base(control) {}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeManageRelationsCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeManageRelationsCommand; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MailMergeManageRelationsCommand; } }
		public override string ImageName { get { return "ManageRelations"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			DocumentModel.BeginUpdateFromUI();
			try {
				if(InnerControl.AllowShowingForms) {
					Control.ShowManageRelationsForm();
				}
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = DocumentModel.MailMergeDataSource is SqlDataSource;
		}
		public void ApplyChanges(List<MasterDetailInfo> oldValues) {
			SqlDataSource sqlDataSource = DocumentModel.MailMergeDataSource as SqlDataSource;
			if(sqlDataSource == null)
				return;
			ManageRelationsHistoryItem historyItem = new ManageRelationsHistoryItem(DocumentModel, oldValues);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
			DocumentModel.DataComponentInfos.UpdateActiveDataSource(sqlDataSource);
			DocumentModel.RaiseMailMergeRelationsChanged();
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	internal class MailMergeManageRelationsCommandGroup : SpreadsheetCommandGroup {
		public MailMergeManageRelationsCommandGroup(ISpreadsheetControl control) : base(control) {}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeManageRelationsCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeManageRelationsCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MailMergeManageRelationsCommandGroup; } }
		public override string ImageName { get { return "ManageRelations"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
			Control.InnerControl.RaiseUpdateUI();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = DocumentModel.MailMergeDataSource is SqlDataSource;
		}
	}
}
