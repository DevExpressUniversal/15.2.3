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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region PivotTablePopupMenuCommandBase
	public abstract class PivotTablePopupMenuCommandBase : SpreadsheetMenuItemSimpleCommand {
		protected PivotTablePopupMenuCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected PivotTableStaticInfo Info { get { return ActiveSheet.PivotTableStaticInfo; } }
		protected PivotTable PivotTable { get { return ActiveSheet.PivotTables[Info.TableIndex]; } }
		protected PivotTableAxis Axis { get { return Info == null ? PivotTableAxis.None : Info.Axis; } }
		protected PivotField Field { get { return PivotTable.Fields[FieldIndex]; } }
		protected PivotDataField DataField { get { return PivotTable.DataFields[FieldReferenceIndex]; } }
		protected virtual int FieldIndex { get { return Info.FieldIndex; } }
		protected int FieldReferenceIndex { get { return Info.FieldReferenceIndex; } }
		protected string FieldName { get { return PivotTable.GetFieldCaption(FieldIndex); } }
		protected CellPosition ActiveCell { get { return ActiveSheet.Selection.ActiveCell; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive && !ActiveSheet.ReadOnly);
			ApplyActiveSheetProtection(state, !Protection.PivotTablesLocked);
			state.Visible = IsVisible();
			state.Checked = IsChecked();
			state.Enabled = IsEnabled();
		}
		protected virtual bool IsVisible() {
			return true;
		}
		protected virtual bool IsChecked() {
			return false;
		}
		protected virtual bool IsEnabled() {
			return true;
		}
	}
	#endregion
	#region PivotTablePopupMenuCommandGroupBase
	public abstract class PivotTablePopupMenuCommandGroupBase : SpreadsheetCommandGroup {
		protected PivotTablePopupMenuCommandGroupBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected PivotTableStaticInfo Info { get { return ActiveSheet.PivotTableStaticInfo; } }
		protected PivotTable PivotTable { get { return ActiveSheet.PivotTables[Info.TableIndex]; } }
		protected PivotTableAxis Axis { get { return Info.Axis; } }
		protected PivotField Field { get { return PivotTable.Fields[FieldIndex]; } }
		protected virtual int FieldIndex { get { return Info.FieldIndex; } }
		protected int FieldReferenceIndex { get { return Info.FieldReferenceIndex; } }
		protected bool ContainsNonDate { get { return PivotTable.Cache.CacheFields[FieldIndex].SharedItems.ContainsNonDate; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive && !ActiveSheet.ReadOnly);
			ApplyActiveSheetProtection(state, !Protection.PivotTablesLocked);
			state.Visible = IsVisible();
			state.Checked = IsChecked();
			state.Enabled = IsEnabled();
		}
		protected virtual bool IsVisible() {
			return true;
		}
		protected virtual bool IsChecked() {
			return false;
		}
		protected virtual bool IsEnabled() {
			return true;
		}
	}
	#endregion
}
