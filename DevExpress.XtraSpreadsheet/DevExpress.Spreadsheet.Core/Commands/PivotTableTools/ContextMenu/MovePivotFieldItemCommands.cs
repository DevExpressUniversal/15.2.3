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
	#region MovePivotFieldItemCommandBase
	public abstract class MovePivotFieldItemCommandBase : PivotTablePopupMenuCommandBase {
		readonly int itemIndex;
		protected MovePivotFieldItemCommandBase(ISpreadsheetControl control)
			: base(control) {
			this.itemIndex = GetItemIndex();
		}
		#region Properties
		public override string MenuCaption { get { return GetFormattedCaption(base.MenuCaption); } }
		public override string Description { get { return GetFormattedCaption(base.Description); } }
		protected int ItemIndex { get { return itemIndex; } }
		protected abstract PivotItemMoveType MoveType { get; }
		protected bool IsValidAxis { get { return Axis == PivotTableAxis.Column || Axis == PivotTableAxis.Row; } }
		#endregion
		protected internal override void ExecuteCore() {
			MovePivotFieldItemCommand command = new MovePivotFieldItemCommand(Field, itemIndex, MoveType, InnerControl.ErrorHandler);
			command.Execute();
		}
		string GetFormattedCaption(string caption) {
			if (itemIndex >= 0) {
				string itemName = PivotTableFieldsFilterItemsCommandBase.GetItemName(PivotTable, FieldIndex, itemIndex);
				return String.Format(caption, itemName);
			}
			return caption;
		}
		protected override bool IsVisible() {
			return IsValidAxis && itemIndex >= 0;
		}
		int GetItemIndex() {
			if (!IsValidAxis || FieldReferenceIndex != 0)
				return -1; 
			PivotZone zone = PivotTable.CalculationInfo.GetPivotZoneByCellPosition(ActiveCell);
			IPivotLayoutItem layoutItem;
			PivotLayoutItems collection;
			if (Axis == PivotTableAxis.Row) {
				layoutItem = zone.RowAxisInfo.LayoutItem;
				collection = PivotTable.CalculationInfo.RowItems;
			}
			else {
				layoutItem = zone.ColumnAxisInfo.LayoutItem;
				collection = PivotTable.CalculationInfo.ColumnItems;
			}
			if (layoutItem.Type != PivotFieldItemType.Data)
				return -1;
			if (FieldReferenceIndex < layoutItem.RepeatedItemsCount)
				layoutItem = FindLayoutItem(collection, layoutItem);
			return layoutItem.PivotFieldItemIndices[FieldReferenceIndex - layoutItem.RepeatedItemsCount];
		}
		IPivotLayoutItem FindLayoutItem(PivotLayoutItems collection, IPivotLayoutItem layoutItem) {
			int layoutItemIndex = collection.IndexOf(layoutItem);
			for (int i = layoutItemIndex - 1; i >= 0; i--) {
				layoutItem = collection[i];
				if (FieldReferenceIndex >= layoutItem.RepeatedItemsCount)
					break;
			}
			return layoutItem;
		}
	}
	#endregion
	#region MovePivotFieldItemUpCommand
	public class MovePivotFieldItemUpCommand : MovePivotFieldItemCommandBase {
		public MovePivotFieldItemUpCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MovePivotFieldItemUp; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceUp; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceUp; } }
		protected override PivotItemMoveType MoveType { get { return PivotItemMoveType.Up; } }
		#endregion
		protected override bool IsEnabled() {
			return IsValidAxis && ItemIndex > 0;
		}
	}
	#endregion
	#region MovePivotFieldItemDownCommand
	public class MovePivotFieldItemDownCommand : MovePivotFieldItemCommandBase {
		public MovePivotFieldItemDownCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MovePivotFieldItemDown; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceDown; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceDown; } }
		protected override PivotItemMoveType MoveType { get { return PivotItemMoveType.Down; } }
		#endregion
		protected override bool IsEnabled() {
			return IsValidAxis && ItemIndex < Field.Items.DataItemsCount - 1;
		}
	}
	#endregion
	#region MovePivotFieldItemToBeginningCommand
	public class MovePivotFieldItemToBeginningCommand : MovePivotFieldItemUpCommand {
		public MovePivotFieldItemToBeginningCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MovePivotFieldItemToBeginning; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToBeginning; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToBeginning; } }
		protected override PivotItemMoveType MoveType { get { return PivotItemMoveType.ToBeginning; } }
		#endregion
	}
	#endregion
	#region MovePivotFieldItemToEndCommand
	public class MovePivotFieldItemToEndCommand : MovePivotFieldItemDownCommand {
		public MovePivotFieldItemToEndCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MovePivotFieldItemToEnd; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToEnd; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MovePivotFieldReferenceToEnd; } }
		protected override PivotItemMoveType MoveType { get { return PivotItemMoveType.ToEnd; } }
		#endregion
	}
	#endregion
}
