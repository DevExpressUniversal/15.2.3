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

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotAggregateCommand
	public class PivotAggregateCommand : PivotTableTransactionCommand {
		public PivotAggregateCommand(IPivotTableTransaction transaction)
			: base(transaction) {
		}
		protected internal override bool Validate() {
			if (PivotTable.Cache.ContainsGrouppedOrCalculatedFields()) {
				ModelErrorInfo errorInfo = new ModelErrorInfo(ModelErrorType.PivotTableGrouppedAndCalculatedFieldsAreNotSupportedNow);
				if (!HandleError(errorInfo))
					return false;
			}
			return true;
		}
		protected internal override void ExecuteCore() {
			Debug.Assert(!PivotTable.Cache.RefreshNeeded);
			PivotGenerateItemsCommand generateItemsCommand = new PivotGenerateItemsCommand(PivotTable);
			generateItemsCommand.Execute();
			Transaction.InitPageFieldsVisibilityData();
			AggregatedCacheCalculator cacheCalculator = new AggregatedCacheCalculator(Transaction);
			PivotCalculatedCache calculatedCache = cacheCalculator.Calculate();
			PivotTable.CalculationInfo.SetCalculatedCacheCore(calculatedCache);
		}
	}
	#endregion
	#region PivotGenerateItemsCommand
	public class PivotGenerateItemsCommand : SpreadsheetModelCommand {
		readonly PivotTable pivotTable;
		public PivotGenerateItemsCommand(PivotTable pivotTable)
			: base(pivotTable.Worksheet) {
			this.pivotTable = pivotTable;
		}
		protected internal override void ExecuteCore() {
			GenerateItems();
		}
		#region Prepare pivot field items
		void GenerateItems() {
			for (int i = 0; i < pivotTable.Fields.Count; ++i)
				GenerateFieldItems(i);
		}
		public void GenerateItemsAfterImport() {
#if !DXPORTABLE
			Parallel.For(0, pivotTable.Fields.Count, GenerateFieldItemsAfterImport);
#else
			for (int i = 0; i < pivotTable.Fields.Count; i++)
				GenerateFieldItemsAfterImport(i);
#endif
		}
		void GenerateFieldItemsAfterImport(int fieldIndex) {
			if (pivotTable.Fields[fieldIndex].Items.Count == 0) {
				IPivotCacheField cacheField = pivotTable.Cache.CacheFields[fieldIndex];
				if (cacheField.DatabaseField && !cacheField.FieldGroup.Initialized)
					GenerateFieldItems(fieldIndex);
			}
		}
		void GenerateFieldItems(int fieldIndex) {
			PivotField field = pivotTable.Fields[fieldIndex];
			int fieldExistingItemsCount = field.Items.Count;
			IPivotCacheField cacheField = pivotTable.Cache.CacheFields[fieldIndex];
			if (cacheField.FieldGroup.GroupItems.Count > 0) {
				field.Items.ClearCore();
				for (int i = 0; i < cacheField.FieldGroup.GroupItems.Count; ++i) {
					PivotItem pivotItem = new PivotItem(pivotTable); 
					pivotItem.SetItemIndexCore(i);
					field.Items.AddCore(pivotItem);
				}
			}
			else
				if (fieldExistingItemsCount <= 0) {
					int cacheFieldSharedItemsCount = cacheField.SharedItems.Count;
					if (cacheFieldSharedItemsCount <= 0)
						return;
					bool hideNewItems = !field.IncludeNewItemsInFilter && pivotTable.FieldHasHiddenItems(fieldIndex);
					for (int i = 0; i < cacheFieldSharedItemsCount; i++) {
						PivotItem pivotItem = pivotTable.CalculationInfo.CreatePivotItem(fieldIndex, i, cacheField, hideNewItems);
						field.Items.AddCore(pivotItem);
					}
				}
				else
					for (int i = field.Items.Count - 1; i >= 0; i--) {
						if (!field.Items[i].IsDataItem)
							field.Items.RemoveAtCore(i);
						else
							break;
					}
			SortFieldItems(field, cacheField.SharedItems, fieldExistingItemsCount);
			AddSubtotalItems(field);
		}
		void SortFieldItems(PivotField field, PivotCacheSharedItemsCollection sharedItemsCollection, int fieldExistingItemsCount) {
			if (field.SortType == PivotTableSortTypeField.Manual && fieldExistingItemsCount > 0)
				return;
			bool ascending = field.SortType != PivotTableSortTypeField.Descending;
			field.Items.Sort(0, field.Items.Count, new PivotFieldItemComparer(ascending, sharedItemsCollection));
		}
		void AddSubtotalItems(PivotField field) {
			if (field.SumSubtotal)
				field.Items.AddCore(CreatePivotSubtotalItem(field, PivotFieldItemType.Sum));
			if (field.CountSubtotal)
				field.Items.AddCore(CreatePivotSubtotalItem(field, PivotFieldItemType.Count));
			if (field.AvgSubtotal)
				field.Items.AddCore(CreatePivotSubtotalItem(field, PivotFieldItemType.Avg));
			if (field.MaxSubtotal)
				field.Items.AddCore(CreatePivotSubtotalItem(field, PivotFieldItemType.Max));
			if (field.MinSubtotal)
				field.Items.AddCore(CreatePivotSubtotalItem(field, PivotFieldItemType.Min));
			if (field.ProductSubtotal)
				field.Items.AddCore(CreatePivotSubtotalItem(field, PivotFieldItemType.Product));
			if (field.CountASubtotal)
				field.Items.AddCore(CreatePivotSubtotalItem(field, PivotFieldItemType.CountA));
			if (field.StdDevSubtotal)
				field.Items.AddCore(CreatePivotSubtotalItem(field, PivotFieldItemType.StdDev));
			if (field.StdDevPSubtotal)
				field.Items.AddCore(CreatePivotSubtotalItem(field, PivotFieldItemType.StdDevP));
			if (field.VarSubtotal)
				field.Items.AddCore(CreatePivotSubtotalItem(field, PivotFieldItemType.Var));
			if (field.VarPSubtotal)
				field.Items.AddCore(CreatePivotSubtotalItem(field, PivotFieldItemType.VarP));
			if (field.DefaultSubtotal)
				field.Items.AddCore(CreatePivotSubtotalItem(field, PivotFieldItemType.DefaultValue));
		}
		PivotItem CreatePivotSubtotalItem(PivotField field, PivotFieldItemType itemType) {
			PivotItem pivotItem = new PivotItem(pivotTable);
			pivotItem.SetItemTypeCore(itemType);
			pivotItem.SetItemIndexCore(-1);
			return pivotItem;
		}
		#endregion
	}
	#endregion
}
