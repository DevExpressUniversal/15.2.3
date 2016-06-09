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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ConditionalFormatting;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public class ManagerController {
		IDialogContext context;
		ManagerItemsCollection items;
		IConditionModelItemsBuilder builder;
		IModelProperty Conditions { get { return context.Conditions; } }
		public ManagerController(IDialogContext context) {
			this.context = context;
			builder = context.Builder;
			items = new ManagerItemsCollection();
			AssignItems();
		}
		void AssignItems() {
			var formatConditions = Conditions.Collection;
			items.Assign(formatConditions.Select(x => ManagerItemViewModel.Factory((ISupportManager)x.GetCurrentValue())).ToList());
		}
		public ManagerItemViewModel Add(BaseEditUnit editUnit) {
			ManagerItemViewModel item = ManagerItemViewModel.Factory(null);
			item.SetEditUnit(editUnit);
			items.Add(item);
			return item;
		}
		public void Remove(ManagerItemViewModel item) {
			items.Remove(item);
		}
		public void Swap(ManagerItemViewModel first, ManagerItemViewModel second) {
			items.Move(items.IndexOf(first), items.IndexOf(second));
		}
		public void Edit(ManagerItemViewModel item, BaseEditUnit unit) {
			item.SetEditUnit(unit);
		}
		public IList<ManagerItemViewModel> GetDisplayItems(string fieldName) {
			var displayItems = items.AsEnumerable();
			if(!string.IsNullOrEmpty(fieldName))
				displayItems = items.Where(x => x.EditUnit.FieldName == fieldName);
			return displayItems.ToList();
		}
		public void ApplyChanges() {
			IModelItem dataControlModel = context.GetRootModelItem();
			ILockable conditions = (ILockable)Conditions.ComputedValue;
			conditions.BeginUpdate();
			try {
				using(IModelEditingScope scope = dataControlModel.BeginEdit("Update format conditions from manager")) {
					UpdateModelItems(dataControlModel);
					scope.Complete();
				}
			}
			finally {
				conditions.EndUpdate();
			}
			AssignItems();
		}
		void UpdateModelItems(IModelItem dataControl) {
			IModelItemCollection modelItems = Conditions.Collection;
			IndexedItem[] indexedItems = items.Select((x, i) => new IndexedItem(x, i)).ToArray();
			RemoveItems(modelItems, indexedItems);
			AddNewItems(modelItems, indexedItems);
			ReplaceModelItems(modelItems, indexedItems);
			EditModelItems(modelItems, indexedItems);
		}
		void RemoveItems(IModelItemCollection modelItems, IndexedItem[] indexedItems) {
			var oldItems = indexedItems.Where(x => !x.IsNew).Select(y => y.Value).ToArray();
			var modelsToRemove = new List<IModelItem>();
			foreach(IModelItem modelItem in modelItems) {
				var condition = modelItem.GetCurrentValue() as ISupportManager;
				if(condition == null || !oldItems.Contains(condition))
					modelsToRemove.Add(modelItem);
			}
			foreach(IModelItem item in modelsToRemove)
				modelItems.Remove(item);
		}
		void AddNewItems(IModelItemCollection modelItems, IndexedItem[] indexedItems) {
			var indexedNewItems = indexedItems.Where(x => x.IsNew).ToArray();
			foreach(var indexedItem in indexedNewItems)
				modelItems.Insert(indexedItem.Index, indexedItem.Unit.BuildCondition(builder));
		}
		void ReplaceModelItems(IModelItemCollection modelItems, IndexedItem[] indexedItems) {
			var indexedOldItems = indexedItems.Where(x => !x.IsNew).ToArray();
			foreach(var indexedItem in indexedOldItems) {
				IModelItem modelItem = modelItems[indexedItem.Index];
				ISupportManager condition = indexedItem.Value;
				if(condition != modelItem.GetCurrentValue()) {
					modelItems.Remove(condition);
					modelItems.Insert(indexedItem.Index, condition);
				}
			}
		}
		void EditModelItems(IModelItemCollection modelItems, IndexedItem[] indexedItems) {
			var indexedOldItems = indexedItems.Where(x => !x.IsNew).ToArray();
			foreach(var indexedItem in indexedOldItems)
				indexedItem.Unit.BuildCondition(builder, modelItems[indexedItem.Index]);
		}
		struct IndexedItem {
			readonly int index;
			readonly BaseEditUnit unit;
			readonly ISupportManager value;
			public int Index { get { return index; } }
			public BaseEditUnit Unit { get { return unit; } }
			public ISupportManager Value { get { return value; } }
			public IndexedItem(ManagerItemViewModel vm, int index) {
				this.index = index;
				this.unit = vm.EditUnit;
				this.value = vm.Value;
			}
			public bool IsNew { get { return Value == null; } }
		}
	}
}
