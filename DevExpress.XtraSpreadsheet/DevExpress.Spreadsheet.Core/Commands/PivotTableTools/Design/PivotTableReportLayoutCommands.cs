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
	#region PivotTableShowCompactFormCommand
	public class PivotTableShowCompactFormCommand : LayoutPivotTableCommandBase {
		public PivotTableShowCompactFormCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowCompactForm; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableShowCompactForm; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override string ImageName { get { return "ShowCompactFormPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			table.BeginTransaction(ErrorHandler);
			try {
				table.SetCompactForm(ErrorHandler);
			}
			finally {
				table.EndTransaction();
			}
		}
	}
	#endregion
	#region PivotTableShowOutlineFormCommand
	public class PivotTableShowOutlineFormCommand : LayoutPivotTableCommandBase {
		public PivotTableShowOutlineFormCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowOutlineForm; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableShowOutlineForm; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override string ImageName { get { return "ShowOutlineFormPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			table.BeginTransaction(ErrorHandler);
			try {
				table.SetOutlineForm(ErrorHandler);
			}
			finally {
				table.EndTransaction();
			}
		}
	}
	#endregion
	#region PivotTableShowTabularFormCommand
	public class PivotTableShowTabularFormCommand : LayoutPivotTableCommandBase {
		public PivotTableShowTabularFormCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowTabularForm; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableShowTabularForm; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override string ImageName { get { return "ShowTabularFormPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			table.BeginTransaction(ErrorHandler);
			try {
				table.SetTabularForm(ErrorHandler);
			}
			finally {
				table.EndTransaction();
			}
		}
	}
	#endregion
	#region PivotTableRepeatAllItemLabelsCommand
	public class PivotTableRepeatAllItemLabelsCommand : LayoutPivotTableCommandBase {
		public PivotTableRepeatAllItemLabelsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableRepeatAllItemLabels; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableRepeatAllItemLabels; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override string ImageName { get { return "RepeatAllItemLabelsPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			table.BeginTransaction(ErrorHandler);
			try {
				table.RepeatAllItemLabels(true, ErrorHandler);
			}
			finally {
				table.EndTransaction();
			}
		}
	}
	#endregion
	#region PivotTableDoNotRepeatItemLabelsCommand
	public class PivotTableDoNotRepeatItemLabelsCommand : LayoutPivotTableCommandBase {
		public PivotTableDoNotRepeatItemLabelsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDoNotRepeatItemLabels; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableDoNotRepeatItemLabels; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override string ImageName { get { return "DoNotRepeatItemLabelsPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			table.BeginTransaction(ErrorHandler);
			try {
				table.RepeatAllItemLabels(false, ErrorHandler);
			}
			finally {
				table.EndTransaction();
			}
		}
	}
	#endregion
}
