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
using System.Collections.Generic;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FilterToggleCommand
	public class FilterToggleCommand : SpreadsheetMenuItemSimpleCommand {
		readonly DataSortOrFilterAccessor accessor;
		bool clearFilter;
		public FilterToggleCommand(ISpreadsheetControl control)
			: base(control) {
			this.accessor = new DataSortOrFilterAccessor(DocumentModel);
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterToggle; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterToggle; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterToggleDescription; } }
		public override string ImageName { get { return "Filter"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			this.clearFilter = state.Checked;
			base.ForceExecute(state);
		}
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				CellRange range = accessor.GetSortOrFilterRange();
				if (clearFilter) {
					DisableFilter();
					DocumentModel.ApplyChanges(DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.RaiseModifiedChanged | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetAllLayout | DocumentModelChangeActions.ResetCachedContentVersions | DocumentModelChangeActions.ResetCachedTransactionVersions);
				}
				else
					CreateFilter(range);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void DisableFilter() {
			AutoFilterBase filter = accessor.Filter;
			if (filter != null)
				filter.Disable();
		}
		void CreateFilter(CellRange range) {
			if (range != null) {
				AutoFilterBase filter = accessor.Filter;
				if (filter == null)
					filter = ActiveSheet.AutoFilter;
				if (filter is TableAutoFilter) {
					filter.Range = accessor.EntireRange.Clone();
					Table table = ActiveSheet.Tables.TryGetItem(filter.Range.TopLeft);
					table.ChangeHeaders(true, true, true, ErrorHandler);
				}
				else
					filter.Range = range.Clone();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive && !ActiveSheet.ReadOnly);
			ApplyActiveSheetProtection(state, !Protection.AutoFiltersLocked);
			if (state.Enabled)
				state.Enabled = CanToggle();
			state.Checked = state.Enabled ? accessor.GetFilter().Enabled : false;
		}
		bool CanToggle() {
			if (accessor == null)
				return false;
			SheetViewSelection selection = accessor.Selection;
			if (selection.TryGetActivePivotTable() != null)
				return false;
			IList<Table> tables = selection.GetActiveTables();
			if (tables.Count == 0)
				return true;
			if (tables.Count != 1)
				return false;
			return tables[0].Range.ContainsRange(selection.ActiveRange);
		}
	}
	#endregion
}
