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
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region PivotTableFieldsFiltersCommandGroup
	public class PivotTableFieldsFiltersCommandGroup : PivotTablePopupMenuCommandGroupBase {
		public PivotTableFieldsFiltersCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableFieldsFiltersCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableFieldsFilters; } }
		public override string MenuCaption { 
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_PivotTableFieldsFilters) + " (" +PivotTable.GetFieldCaption(Info.FieldIndex) + ")";
			}
		}
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableFieldsFiltersDescription; } }
		#endregion
		protected override bool IsVisible() {
			return ActiveSheet.PivotTableStaticInfo.IsContextMenuFieldGroupActive;
		}
	}
	#endregion
	#region PivotTableSelectFieldFiltersCommand
	public class PivotTableSelectFieldFiltersCommand : PivotTablePopupMenuCommandBase {
		int fieldIndex ;
		public PivotTableSelectFieldFiltersCommand(ISpreadsheetControl control, int fieldIndex)
			: base(control) {
			this.fieldIndex = fieldIndex;
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override string MenuCaption { get { return PivotTable.GetFieldCaption(fieldIndex); } }
		#endregion
		protected internal override void ExecuteCore() {
			Info.FieldIndex = fieldIndex;
			PivotTable pivotTable = PivotTable;
			pivotTable.Filters.SetupPivotInfo(Info, pivotTable.MultipleFieldFilters);
			Control.ShowPivotTableAutoFilterForm();
		}
		protected override bool IsChecked() {
			return FieldIndex == fieldIndex;
		}
	}
	#endregion
}
