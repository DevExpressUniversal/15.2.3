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
namespace DevExpress.XtraSpreadsheet.Commands {
	#region  MovePivotFieldReferenceCommandGroup
	public class MovePivotFieldReferenceCommandGroup : PivotTablePopupMenuCommandGroupBase {
		public MovePivotFieldReferenceCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MovePivotFieldReferenceCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReference; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReference; } }
		#endregion
		protected override bool IsVisible() {
			return Axis != PivotTableAxis.None && Axis != PivotTableAxis.Value;
		}
		protected override bool IsEnabled() {
			return FieldReferenceIndex > 0;
		}
	}
	#endregion
	#region MovePivotFieldReferenceCommandBase
	public abstract class MovePivotFieldReferenceCommandBase : PivotTablePopupMenuCommandBase {
		protected MovePivotFieldReferenceCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected abstract int TargetFieldRefernceIndex { get; }
		protected virtual PivotTableAxis TargetAxis { get { return Axis; } }
		protected int Count { get { return GetCount(); } }
		protected bool IsRowAxis { get { return Axis == PivotTableAxis.Row; } }
		public override string MenuCaption { get { return GetFormattedCaption(base.MenuCaption); } }
		public override string Description { get { return GetFormattedCaption(base.Description); } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable.MoveKeyField(Axis, FieldReferenceIndex, TargetAxis, TargetFieldRefernceIndex, Control.InnerControl.ErrorHandler);
		}
		int GetCount() {
			if (TargetAxis == PivotTableAxis.Column)
				return PivotTable.ColumnFields.Count;
			if (TargetAxis == PivotTableAxis.Row)
				return PivotTable.RowFields.Count;
			return PivotTable.PageFields.Count;
		}
		string GetFormattedCaption(string caption) {
			if (Info == null || FieldIndex < 0)
				return caption;
			return String.Format(caption, FieldName);
		}
	}
	#endregion
	#region MovePivotFieldReferenceUpCommand
	public class MovePivotFieldReferenceUpCommand : MovePivotFieldReferenceCommandBase {
		public MovePivotFieldReferenceUpCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MovePivotFieldReferenceUp; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return GetCaptionId(); } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return GetCaptionId(); } }
		protected override int TargetFieldRefernceIndex { get { return FieldReferenceIndex - 1; } }
		#endregion
		protected override bool IsEnabled() {
			return FieldReferenceIndex > 0;
		}
		XtraSpreadsheetStringId GetCaptionId() {
			if (Info == null || IsRowAxis)
				return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToLeft;
			return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceUp;
		}
	}
	#endregion
	#region MovePivotFieldReferenceDownCommand
	public class MovePivotFieldReferenceDownCommand : MovePivotFieldReferenceCommandBase {
		public MovePivotFieldReferenceDownCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MovePivotFieldReferenceDown; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return GetCaptionId(); } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return GetCaptionId(); } }
		protected override int TargetFieldRefernceIndex { get { return FieldReferenceIndex + 2; } }
		#endregion
		protected override bool IsEnabled() {
			return FieldReferenceIndex < Count - 1;
		}
		XtraSpreadsheetStringId GetCaptionId() {
			if (Info == null || IsRowAxis)
				return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToRight;
			return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceDown;
		}
	}
	#endregion
	#region MovePivotFieldReferenceToBeginningCommand
	public class MovePivotFieldReferenceToBeginningCommand : MovePivotFieldReferenceUpCommand {
		public MovePivotFieldReferenceToBeginningCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MovePivotFieldReferenceToBeginning; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToBeginning; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToBeginning; } }
		protected override int TargetFieldRefernceIndex { get { return 0; } }
		#endregion
	}
	#endregion
	#region MovePivotFieldReferenceToEndCommand
	public class MovePivotFieldReferenceToEndCommand : MovePivotFieldReferenceDownCommand {
		public MovePivotFieldReferenceToEndCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MovePivotFieldReferenceToEnd; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToEnd; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToEnd; } }
		protected override int TargetFieldRefernceIndex { get { return Count; } }
		#endregion
	}
	#endregion
	#region MovePivotFieldReferenceToDifferentAxisCommand
	public class MovePivotFieldReferenceToDifferentAxisCommand : MovePivotFieldReferenceCommandBase {
		public MovePivotFieldReferenceToDifferentAxisCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MovePivotFieldReferenceToDifferentAxis; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return GetCaptionId(); } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return GetCaptionId(); } }
		protected override PivotTableAxis TargetAxis { get { return IsRowAxis ? PivotTableAxis.Column : PivotTableAxis.Row; } }
		protected override int TargetFieldRefernceIndex { get { return Count; } }
		#endregion
		protected override bool IsVisible() {
			return Axis != PivotTableAxis.Page;
		}
		XtraSpreadsheetStringId GetCaptionId() {
			if (Info == null || IsRowAxis)
				return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToColumns;
			return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToRows;
		}
	}
	#endregion
}
