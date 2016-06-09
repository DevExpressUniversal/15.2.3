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
using DevExpress.Utils;
using DevExpress.Office.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotItemVisibilityCommandBase
	public abstract class PivotItemVisibilityCommandBase : PivotTableTransactedCommand {
		readonly PivotField pivotField;
		protected PivotItemVisibilityCommandBase(PivotField pivotField, IErrorHandler errorHandler)
			: base(pivotField.PivotTable, errorHandler) {
			Guard.ArgumentNotNull(pivotField, "pivotField");
			this.pivotField = pivotField;
		}
		#region Properties
		protected PivotField PivotField { get { return pivotField; } }
		protected PivotItemCollection Items { get { return pivotField.Items; } }
		protected bool IsPageField { get { return pivotField.Axis == PivotTableAxis.Page; } }
		#endregion
		protected internal override void ExecuteCore() {
			SetVisibility();
			if (IsPageField) {
				PivotPageField pageField = FindPageField();
				Guard.ArgumentNotNull(pageField, "pageField");
				ProcessPageField(pageField);
			}
			PivotTable.CalculationInfo.InvalidateLayout();
		}
		protected virtual void SetVisibility() {
		}
		protected virtual void ProcessPageField(PivotPageField pageField) {
		}
		protected PivotPageField FindPageField() {
			foreach (PivotPageField pageField in PivotTable.PageFields)
				if (Object.ReferenceEquals(PivotTable.Fields[pageField.FieldIndex], pivotField))
					return pageField;
			return null;
		}
		protected PivotItem ValidateItem(int itemIndex) {
			ValueChecker.CheckValue(itemIndex, 0, Items.Count - 1);
			PivotItem pivotItem = Items[itemIndex];
			if (!pivotItem.IsDataItem)
				throw new ArgumentException("Pivot item must be of data type.");
			return pivotItem;
		}
	}
	#endregion
	#region TogglePivotItemVisibilityCommand
	public class TogglePivotItemVisibilityCommand : PivotItemVisibilityCommandBase {
		#region Fields
		readonly PivotItem pivotItem;
		readonly List<int> visibleItemIndexes;
		readonly bool setToVisible;
		readonly int itemIndex;
		#endregion
		public TogglePivotItemVisibilityCommand(PivotField pivotField, int itemIndex, IErrorHandler errorHandler)
			: base(pivotField, errorHandler) {
			this.pivotItem = ValidateItem(itemIndex);
			this.itemIndex = itemIndex;
			this.setToVisible = pivotItem.IsHidden;
			this.visibleItemIndexes = Items.GetVisibleItemIndexes();
		}
		#region Properties
		bool HidingLastVisible { get { return !setToVisible && visibleItemIndexes.Count < 2; } }
		bool UnhidingLastHidden { get { return setToVisible && visibleItemIndexes.Count == Items.DataItemsCount - 1; } }
		#endregion
		protected internal override bool Validate() {
			if (HidingLastVisible)
				return HandleError(new ModelErrorInfo(ModelErrorType.PivotCannotHideLastVisibleItem));
			return true;
		}
		protected override void SetVisibility() {
			pivotItem.IsHidden = !setToVisible;
		}
		protected override void ProcessPageField(PivotPageField pageField) {
			if (pageField.ItemIndex >= 0) {
				if (!setToVisible && this.itemIndex == pageField.ItemIndex)
					pageField.ItemIndex = GetIndexOfNextVisible(pageField.ItemIndex);
			}
			else
				PivotField.MultipleItemSelectionAllowed = !UnhidingLastHidden;
		}
		int GetIndexOfNextVisible(int currentVisible) {
			int indexInList = visibleItemIndexes.IndexOf(currentVisible);
			bool lastInList = indexInList == visibleItemIndexes.Count - 1;
			return lastInList ? visibleItemIndexes[0] : visibleItemIndexes[indexInList + 1];
		}
	}
	#endregion
	#region TurnOffShowSingleItemCommand
	public class TurnOffShowSingleItemCommand : PivotItemVisibilityCommandBase {
		PivotPageField pageField;
		public TurnOffShowSingleItemCommand(PivotField pivotField, IErrorHandler errorHandler)
			: base(pivotField, errorHandler) {
		}
		protected internal override bool Validate() {
			if (PivotField.Axis != PivotTableAxis.Page)
				return false;
			pageField = FindPageField();
			Guard.ArgumentNotNull(pageField, "pageField");
			return pageField.ItemIndex >= 0;
		}
		protected internal override void ExecuteCore() {
			pageField.ItemIndex = -1;
			PivotTable.CalculationInfo.InvalidateLayout();
		}
	}
	#endregion
	#region ShowAllPivotItemsCommand
	public class ShowAllPivotItemsCommand : PivotItemVisibilityCommandBase {
		public ShowAllPivotItemsCommand(PivotField pivotField, IErrorHandler errorHandler)
			: base(pivotField, errorHandler) {
		}
		protected override void SetVisibility() {
			Items.UnhideAll();
		}
		protected override void ProcessPageField(PivotPageField pageField) {
			pageField.ItemIndex = -1;
		}
	}
	#endregion
	#region ShowSinglePivotItemCommand
	public class ShowSinglePivotItemCommand : PivotItemVisibilityCommandBase {
		readonly int itemIndex;
		public ShowSinglePivotItemCommand(PivotField pivotField, int itemIndex, IErrorHandler errorHandler)
			: base(pivotField, errorHandler) {
			this.itemIndex = itemIndex;
			ValidateItem(itemIndex);
		}
		protected override void SetVisibility() {
			for (int i = 0; i < Items.DataItemsCount; i++)
				Items[i].IsHidden = i != itemIndex;
		}
		protected override void ProcessPageField(PivotPageField pageField) {
			pageField.ItemIndex = itemIndex;
			PivotField.MultipleItemSelectionAllowed = false;
		}
	}
	#endregion
}
