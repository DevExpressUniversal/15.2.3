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
using DevExpress.XtraRichEdit;
using System.ComponentModel;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Commands {
	#region ToggleTableAutoFitCommandBase (abstract class)
	public abstract class ToggleTableAutoFitCommandBase : RichEditMenuItemSimpleCommand {
		protected ToggleTableAutoFitCommandBase(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal virtual Table Table {
			get { 
				TableCell cell = DocumentModel.Selection.SelectedCells.FirstSelectedCell;
				if (cell == null && InnerControl.ActiveView.CaretPosition.LayoutPosition != null && InnerControl.ActiveView.CaretPosition.LayoutPosition.TableCell != null)
					cell =  InnerControl.ActiveView.CaretPosition.LayoutPosition.TableCell.Cell;
				return cell.Table; 
			}
		}
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				ApplyRealWidths();
				ApplyTableProperties();
				ApplyTableCellProperties();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void ApplyRealWidths() {
			TableViewInfo tableViewInfo = GetTableViewInfo(InnerControl.ActiveView.CaretPosition.LayoutPosition);
			if(object.ReferenceEquals(tableViewInfo, null)) {
				tableViewInfo = GetTableViewInfo(InnerControl.ActiveView.SelectionLayout.StartLayoutPosition);
				if(object.ReferenceEquals(tableViewInfo, null)) {
					tableViewInfo = GetTableViewInfo(InnerControl.ActiveView.SelectionLayout.EndLayoutPosition);
					if(object.ReferenceEquals(tableViewInfo, null))
						Exceptions.ThrowInternalException();
				}
			}
			FromAutoToRealWidthsTableCalculator.ApplyRealWidths(tableViewInfo);
		}
		protected TableViewInfo GetTableViewInfo(DocumentLayoutPosition position) {
			position.Update(InnerControl.ActiveView.DocumentLayout.Pages, DocumentLayoutDetailsLevel.TableCell);
			TableCellViewInfo tableCell = position.TableCell;
			if(position.IsValid(DocumentLayoutDetailsLevel.TableCell) && tableCell != null)
				return tableCell.TableViewInfo;
			else 
				return null;
		}
		protected internal virtual void ApplyTableProperties() {
			WidthUnitInfo widthInfo = GetTableWidthInfo();
			TableProperties tableProperties = Table.TableProperties;
			tableProperties.PreferredWidth.CopyFrom(widthInfo);
			tableProperties.TableLayout = TableLayoutType.Autofit;
		}
		protected internal virtual void ApplyTableCellProperties() {
			TableRowCollection rows = Table.Rows;
			int rowWidth = CalculateRowWidth(rows.First);
			int rowCount = rows.Count;
			for (int i = 0; i < rowCount; i++) {
				TableCellCollection cells = rows[i].Cells;
				int cellCount = cells.Count;
				for (int j = 0; j < cellCount; j++) {
					TableCell currentCell = cells[j];
					WidthUnitInfo widthInfo = GetTableCellWidthInfo(currentCell, rowWidth);
					currentCell.Properties.PreferredWidth.CopyFrom(widthInfo);
				}
			}
		}
		protected abstract WidthUnitInfo GetTableWidthInfo();
		protected abstract WidthUnitInfo GetTableCellWidthInfo(TableCell cell, double rowWidth);
		protected abstract int CalculateRowWidth(TableRow row);
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = DocumentModel.Selection.IsWholeSelectionInOneTable();
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Tables, state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region ToggleTableAutoFitPlaceholderCommand
	public class ToggleTableAutoFitPlaceholderCommand : RichEditMenuItemSimpleCommand, IPlaceholderCommand {
		public ToggleTableAutoFitPlaceholderCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTableAutoFitPlaceholder; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableAutoFitPlaceholder; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableAutoFitPlaceholderDescription; } }
		public override string ImageName { get { return "TableAutoFitContents"; } }
		#endregion
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = DocumentModel.Selection.IsWholeSelectionInOneTable();
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Tables, state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
	#region ToggleTableAutoFitPlaceholderMenuCommand
	public class ToggleTableAutoFitPlaceholderMenuCommand : ToggleTableAutoFitPlaceholderCommand {
		public ToggleTableAutoFitPlaceholderMenuCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleTableAutoFitMenuPlaceholder; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableAutoFitPlaceholder; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleTableAutoFitPlaceholderDescription; } }
		public override string ImageName { get { return String.Empty; } }
		#endregion
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Visible = state.Enabled;
		}
	}
	#endregion
}
