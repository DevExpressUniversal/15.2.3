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

using DevExpress.Office.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region RefreshPivotTableCommand
	public class RefreshPivotTableCommand : LayoutPivotTableCommandBase {
		public RefreshPivotTableCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.RefreshPivotTable; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableRefreshCommandAndCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableRefreshDescription; } }
		public override string ImageName { get { return "RefreshPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			PivotCacheRefreshCommand command = new PivotCacheRefreshCommand(ErrorHandler, table.Cache);
			command.Execute();
		}
		protected override bool GetEnabled(PivotTable table) {
			if (!base.GetEnabled(table))
				return false;
			if (!IsCurrentPivotCacheCanUpdate(table))
				return false;
			return true;
		}
		public static bool IsCurrentPivotCacheCanUpdate(PivotTable table) {
			return table.Cache.EnableRefresh;
		}
	}
	#endregion
	#region RefreshAllPivotTableCommand
	public class RefreshAllPivotTableCommand : LayoutPivotTableCommandBase {
		public RefreshAllPivotTableCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.RefreshAllPivotTable; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableRefreshAllCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableRefreshDescription; } }
		public override string ImageName { get { return "RefreshAllPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			PivotCacheRefreshAllCommand command = new PivotCacheRefreshAllCommand(table.DocumentModel.PivotCaches, ErrorHandler);
			command.Execute();
		}
		protected override bool GetEnabled(PivotTable table) {
			if (!base.GetEnabled(table))
				return false;
			if (!IsAnyPivotCacheCanUpdate(table))
				return false;
			return true;
		}
		public static bool IsAnyPivotCacheCanUpdate(PivotTable table) {
			PivotCacheCollection pivotCaches = table.DocumentModel.PivotCaches;
			foreach (PivotCache pivotCache in pivotCaches)
				if (pivotCache.EnableRefresh)
					return true;
			return false;
		}
	}
	#endregion
}
