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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region PivotTableClearFieldFiltersCommandBase
	public abstract class PivotTableClearFieldFiltersCommandBase : PivotTablePopupMenuCommandBase {
		protected PivotTableClearFieldFiltersCommandBase(ISpreadsheetControl control) 
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotClearFieldFilter; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotClearFieldFilterDescription; } }
		public override string ImageName { get { return "ClearFilter"; } }
		protected abstract PivotFilterClearType ClearType { get; }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable.ClearFieldFilters(FieldIndex, ClearType, Control.InnerControl.ErrorHandler);
		}
	}
	#endregion
	#region PivotTableClearFieldFiltersCommand
	public class PivotTableClearFieldFiltersCommand : PivotTableClearFieldFiltersCommandBase {
		public PivotTableClearFieldFiltersCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableClearFieldFilters; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterColumnClear; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterColumnClearDescription; } }
		public override string MenuCaption { get { return GetFormattedCaption(base.MenuCaption); } }
		public override string Description { get { return GetFormattedCaption(base.Description); } }
		protected override PivotFilterClearType ClearType { get { return PivotFilterClearType.All; } }
		#endregion
		protected override bool IsEnabled() {
			return PivotTable.FieldHasHiddenItemsCore(FieldIndex) || Info.LabelFilter != null || Info.MeasureFilter != null;
		}
		string GetFormattedCaption(string baseCaption) {
			return String.Format(baseCaption, PivotTable.GetFieldCaption(FieldIndex));
		}
	}
	#endregion
	#region PivotTableClearFieldLabelFilterCommand
	public class PivotTableClearFieldLabelFilterCommand : PivotTableClearFieldFiltersCommandBase {
		public PivotTableClearFieldLabelFilterCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableClearFieldLabelFilter; } }
		protected override PivotFilterClearType ClearType { get { return PivotFilterClearType.Label; } }
		#endregion
		protected override bool IsEnabled() {
			return Info.LabelFilter != null;
		}
	}
	#endregion
	#region PivotTableClearFieldValueFilterCommand
	public class PivotTableClearFieldValueFilterCommand : PivotTableClearFieldFiltersCommandBase {
		public PivotTableClearFieldValueFilterCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableClearFieldValueFilter; } }
		protected override PivotFilterClearType ClearType { get { return PivotFilterClearType.Value; } }
		#endregion
		protected override bool IsEnabled() {
			return Info.MeasureFilter != null;
		}
	}
	#endregion
}
