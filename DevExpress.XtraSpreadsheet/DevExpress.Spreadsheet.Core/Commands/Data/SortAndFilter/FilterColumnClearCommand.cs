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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.Utils.Commands;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FilterColumnClearCommand
	public class FilterColumnClearCommand : ShowCustomFilterFormCommandBase {
		public FilterColumnClearCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterColumnClear; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterColumnClear; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterColumnClearDescription; } }
		protected override GenericFilterOperator FilterOperator { get { return GenericFilterOperator.None; } }
		public override string MenuCaption { get { return String.Format(base.MenuCaption, CalculateColumnCaption()); } }
		public override string ImageName { get { return "ClearFilter"; } }
		#endregion
		string CalculateColumnCaption() {
			if (Accessor == null)
				return String.Empty;
			CellRange range = Accessor.GetSortOrFilterRange();
			if (range == null)
				return String.Empty;
			CellPosition position = Selection.ActiveCell;
			ICell cell = ActiveSheet.TryGetCell(position.Column, position.Row);
			if (cell == null)
				return (position.Column - Accessor.Filter.Range.TopLeft.Column + 1).ToString();
			else
				return cell.Text;
		}
		protected internal override void ExecuteCore() {
			ApplyChanges(CreateViewModel());
		}
		protected override void ModifyFilter(CellRange range, GenericFilterViewModel viewModel) {
			AutoFilterColumn filterColumn = GetFilterColumn(Accessor.Filter, range);
			filterColumn.Clear();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Enabled)
				state.Enabled = ColumnIsNonDefault();
		}
		bool ColumnIsNonDefault() {
			if (Accessor == null)
				return false;
			AutoFilterBase filter = Accessor.GetFilter();
			if (filter == null || !filter.Enabled)
				return false;
			AutoFilterColumn filterColumn = GetFilterColumn(filter, filter.Range);
			return filterColumn.IsNonDefault;
		}
	}
	#endregion
}
