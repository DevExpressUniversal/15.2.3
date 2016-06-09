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

using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region MailMergeManageDataSourcesCommand
	public class MailMergeManageDataSourcesCommand : SpreadsheetMenuItemSimpleCommand {
		public MailMergeManageDataSourcesCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MailMergeManageDataSourcesCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeManageDataSourcesCommand; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeManageDataSourcesCommand; } }
		public override string ImageName { get { return "ManageDatasource"; } }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdateFromUI();
			try {
				if(InnerControl.AllowShowingForms) {
					ManageDataSourcesViewModel viewModel = new ManageDataSourcesViewModel(this.Control);
					Control.ShowManageDataSourcesForm(viewModel);
				}
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
		public void ApplyChanges(ManageDataSourcesViewModel viewModel) {
			if(!viewModel.WasChanges)
				return;			
			ManageDataSourcesHistoryItem historyItem = new ManageDataSourcesHistoryItem(DocumentModel, viewModel.DataInfos, viewModel.OldParameters, viewModel.NewDefault);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	internal class MailMergeManageDataSourceCommandGroup : SpreadsheetCommandGroup {
		public MailMergeManageDataSourceCommandGroup(ISpreadsheetControl control) : base(control) {}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeManageDataSourceCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeManageDataSourceCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MailMergeManageDataSourceCommandGroup; } }
		public override string ImageName { get { return "ManageDatasource"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
			Control.InnerControl.RaiseUpdateUI();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {			
		}
	}
}
