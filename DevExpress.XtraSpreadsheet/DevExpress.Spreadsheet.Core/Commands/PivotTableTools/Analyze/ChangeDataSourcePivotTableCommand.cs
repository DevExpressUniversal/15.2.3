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

using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ChangeDataSourcePivotTableCommand
	public class ChangeDataSourcePivotTableCommand : PivotTableCommandBase {
		public ChangeDataSourcePivotTableCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChangeDataSourcePivotTable; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChangeDataSourcePivotTable; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChangeDataSourcePivotTableDescription; } }
		public override string ImageName { get { return "ChangeDataSourcePivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			if (table.Cache.Source.Type != PivotCacheType.Worksheet)
				return;
			if (InnerControl.AllowShowingForms)
				Control.ShowChangeDataSourcePivotTableForm(CreateViewModel(table));
		}
		ChangeDataSourcePivotTableViewModel CreateViewModel(PivotTable table) {
			string reference = table.Cache.Source.GetExpressionString(DocumentModel.DataContext);
			ChangeDataSourcePivotTableViewModel viewModel = new ChangeDataSourcePivotTableViewModel(Control);
			viewModel.PivotTable = table;
			viewModel.Source = reference;
			viewModel.NewWorksheet = false;
			viewModel.Location = null;
			return viewModel;
		}
		protected internal bool Validate(ChangeDataSourcePivotTableViewModel viewModel) {
			if (!ValidateReference(viewModel))
				return false;
			if (!CreatePivotChangeDataSourceCommand(viewModel).Validate())
				return false;
			return true;
		}
		bool ValidateReference(ChangeDataSourcePivotTableViewModel viewModel) {
			WorkbookDataContext dataContext = Control.Document.Model.DocumentModel.DataContext;
			viewModel.PivotSource = PivotCacheSourceWorksheet.CreateInstance(viewModel.Source, dataContext);
			if ((viewModel.PivotSource == null || viewModel.PivotSource.GetRange(dataContext) == null))
				if (!HandleError(new ModelErrorInfo(ModelErrorType.PivotTableDataSourceReferenceNotValid)))
					return false;
			return true;
		}
		protected internal void ApplyChanges(ChangeDataSourcePivotTableViewModel viewModel) {
			viewModel.PivotTable.ChangeDataSourceCore(viewModel.PivotSource, ErrorHandler);
			InnerControl.ResetDocumentLayout();
		}
		PivotChangeDataSourceCommand CreatePivotChangeDataSourceCommand(ChangeDataSourcePivotTableViewModel viewModel) {
			return new PivotChangeDataSourceCommand(viewModel.PivotTable, viewModel.PivotSource, ErrorHandler);
		}
	}
	#endregion
}
