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

using DevExpress.DataAccess.Native;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region MailMergeSelectDataSourceCommand
	public class MailMergeSelectDataSourceCommand : SpreadsheetMenuItemSimpleCommand {
		public MailMergeSelectDataSourceCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MailMergeSelectDataSource; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeSelectDataSourceCommandDescription; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeSelectDataSourceCommand; } }
		public override string ImageName { get { return "SelectDataSource"; } }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdateFromUI();
			try {
				SelectDataSourceViewModel viewModel = new SelectDataSourceViewModel(Control);
				if(InnerControl.AllowShowingForms)
					Control.ShowSelectDataSourceForm(viewModel);
			} finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
		public void ApplyChanges(SelectDataSourceViewModel viewModel) {
			int index = viewModel.SelectedItemIndex;
			if(index < 0)
				return;
			SelectDataSourceHistoryItem historyItem = new SelectDataSourceHistoryItem(DocumentModel, index);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SelectApplicationDataSource() {
			SelectDataSourceHistoryItem historyItem = new SelectDataSourceHistoryItem(DocumentModel, -1);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
	}
	#endregion
}
