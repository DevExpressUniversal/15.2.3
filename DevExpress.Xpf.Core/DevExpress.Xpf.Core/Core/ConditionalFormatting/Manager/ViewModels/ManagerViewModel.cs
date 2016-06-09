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
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Data;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public class ManagerViewModel : ManagerViewModelBase {
		ManagerController controller = null;
		public static Func<IDialogContext, ManagerViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new ManagerViewModel(x)); } }
		protected ManagerViewModel(IDialogContext context)
			: base(context) {
			controller = new ManagerController(context);
			FieldNames = FieldNameWrapper.Create(context.ColumnInfo);
			Items = new ManagerItemsCollection();
			FilterFieldName = new FieldNameWrapper(null, GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FilterAll));
			FilterFieldNames = new List<FieldNameWrapper>(FieldNames);
			List<FieldNameWrapper> pivotSpecialFieldNames = new List<FieldNameWrapper>();
			if(context.PivotSpecialFieldNames != null)
				pivotSpecialFieldNames.AddRange(context.PivotSpecialFieldNames);
			pivotSpecialFieldNames.AddRange(FieldNames);
			PivotSpecialFieldNames = pivotSpecialFieldNames;
			FilterFieldNames.Insert(0, FilterFieldName);
			IsPivot = context.IsPivot;
			ApplyToFieldNameCaption = context.ApplyToFieldNameCaption;
			ApplyToPivotColumnCaption = context.ApplyToPivotColumnCaption;
			ApplyToPivotRowCaption = context.ApplyToPivotRowCaption;
			UpdateItems();
		}
		[BindableProperty(OnPropertyChangedMethodName = "UpdateItems")]
		public virtual FieldNameWrapper FilterFieldName { get; set; }
		public ManagerItemsCollection Items { get; private set; }
		public virtual ManagerItemViewModel SelectedItem { get; set; }
		public virtual IList<FieldNameWrapper> FieldNames { get; protected set; }
		public virtual IList<FieldNameWrapper> PivotSpecialFieldNames { get; protected set; }
		public virtual IList<FieldNameWrapper> FilterFieldNames { get; protected set; }
		public virtual IDialogService DialogService { get { return null; } }
		public bool CanApply { get; internal set; }
		public bool IsPivot { get; private set; }
		public string ApplyToFieldNameCaption { get; private set; } 
		public string ApplyToPivotRowCaption { get; private set; } 
		public string ApplyToPivotColumnCaption { get; private set; }
		[Command(CanExecuteMethodName = "HasSelectedItem")]
		public void RemoveFormatCondition(ManagerItemViewModel item) {
			controller.Remove(item);
			UpdateItems();
			CanApply = true;
		}
		[Command(CanExecuteMethodName = "HasSelectedItem")]
		public void ShowEditDialog(ManagerItemViewModel item) {
			IDialogContext actualContext = Context;
			string name = item.EditUnit.FieldName;
			if(!string.IsNullOrEmpty(name))
				actualContext = Context.Find(name) ?? Context;
			IConditionEditor viewModel = AddConditionViewModel.Factory(actualContext);
			viewModel.Init(item.EditUnit);
			if(ShowDialogCore(viewModel)) {
				controller.Edit(item, viewModel.Edit());
				UpdateItems();
				CanApply = true;
			}
		}
		public void ShowAddDialog() {
			IConditionEditor viewModel = AddConditionViewModel.Factory(Context);
			if(ShowDialogCore(viewModel)) {
				controller.Add(viewModel.Edit());
				UpdateItems();
				SelectedItem = Items.Last();
				CanApply = true;
			}
		}
		bool ShowDialogCore(IConditionEditor vm) {
			return ManagerHelper.ShowDialog(vm, vm.Description, DialogService, x => x.Cancel = !vm.Validate());
		}
		public bool HasSelectedItem(ManagerItemViewModel item) {
			return item != null && Items.Count > 0;
		}
		public void ApplyChanges() {
			controller.ApplyChanges();
			UpdateItems();
			CanApply = false;
		}
		protected void UpdateItems() {
			Items.Assign(controller.GetDisplayItems(FilterFieldName.FieldName).Select(x => x.Init(this)).ToList());
		}
		public void MoveUp(ManagerItemViewModel item) {
			MoveCore(item, -1);
		}
		public void MoveDown(ManagerItemViewModel item) {
			MoveCore(item, 1);
		}
		void MoveCore(ManagerItemViewModel item, int delta) {
			controller.Swap(item, Items[Items.IndexOf(item) + delta]);
			UpdateItems();
			SelectedItem = item;
			CanApply = true;
		}
		public bool CanMoveUp(ManagerItemViewModel item) {
			return HasSelectedItem(item) && item != Items[0];
		}
		public bool CanMoveDown(ManagerItemViewModel item) {
			return HasSelectedItem(item) && item != Items[Items.Count - 1];
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Title); } }
	}
	public class ManagerItemViewModel {
		public static Func<ISupportManager, ManagerItemViewModel> Factory { get { return ViewModelSource.Factory((ISupportManager x) => new ManagerItemViewModel(x)); } }
		ManagerViewModel parentVM;
		public ISupportManager Value { get; private set; }
		public BaseEditUnit EditUnit { get; private set; }
		public virtual string Rule { get; protected set; }
		public virtual Freezable PreviewFormat { get; protected set; }
		public virtual string AppliesTo { get; set; }
		public virtual bool ApplyToRow { get; set; }
		public virtual bool CanApplyToRow { get; protected set; }
		public virtual string ColumnName { get; set; }
		public virtual string RowName { get; set; }
		protected ManagerItemViewModel(ISupportManager condition) {
			condition.Do(x => InitFromCondition(condition));
		}
		void InitFromCondition(ISupportManager condition) {
			Value = condition;
			EditUnit = condition.CreateEditUnit();
			UpdateConditon();
		}
		internal void SetOwner(ManagerViewModel vm) {
			parentVM = vm;
		}
		void UpdateConditon() {
			Rule = EditUnit.GetDescription();
			AppliesTo = EditUnit.FieldName;
			PreviewFormat = CreatePreviewWrapper(EditUnit.GetFormat());
			CanApplyToRow = EditUnit.CanApplyToRow;
			ApplyToRow = CanApplyToRow ? EditUnit.ApplyToRow : false;
			RowName = EditUnit.RowName;
			ColumnName = EditUnit.ColumnName;
			if(PreviewFormat != null && PreviewFormat.CanFreeze)
				PreviewFormat.Freeze();
		}
		bool AreEditUnitsCompatible(BaseEditUnit firstUnit, BaseEditUnit secondUnit) {
			return firstUnit != null && secondUnit != null && firstUnit.GetType() == secondUnit.GetType();
		}
		public void SetEditUnit(BaseEditUnit unit) {
			if(AreEditUnitsCompatible(EditUnit, unit))
				EditUnit.Populate(unit);
			else {
				unit.Restore(EditUnit);
				EditUnit = unit;
				Value = null;
			}
			UpdateConditon();
		}
		protected void OnAppliesToChanged() {
			if(EditUnit.FieldName != AppliesTo) {
				EditUnit.FieldName = AppliesTo;
				if(parentVM != null)
					parentVM.CanApply = true;
			}
		}
		protected void OnApplyToRowChanged() {
			if(EditUnit.ApplyToRow != ApplyToRow) {
				EditUnit.ApplyToRow = ApplyToRow;
				if(parentVM != null)
					parentVM.CanApply = true;
			}
		}
		protected void OnRowNameChanged() {
			if(EditUnit.RowName != RowName) {
				EditUnit.RowName = RowName;
				if(parentVM != null)
					parentVM.CanApply = true;
			}
		}
		protected void OnColumnNameChanged() {
			if(EditUnit.ColumnName != ColumnName) {
				EditUnit.ColumnName = ColumnName;
				if(parentVM != null)
					parentVM.CanApply = true;
			}
		}
		static Freezable CreatePreviewWrapper(Freezable format) {
			return format is IconSetFormat ? new IconFormatStyle((IconSetFormat)format, string.Empty) : format;
		}
	}
	public class ManagerItemsCollection : ObservableCollectionCore<ManagerItemViewModel> { }
	public class FieldNameWrapper {
		public string FieldName { get; private set; }
		public string Caption { get; private set; }
		public FieldNameWrapper(DevExpress.Data.IDataColumnInfo info) : this(info.FieldName, info.Caption) { }
		public FieldNameWrapper(string fieldName, string caption) {
			FieldName = fieldName;
			Caption = caption;
		}
		public static IList<FieldNameWrapper> Create(DevExpress.Data.IDataColumnInfo info) {
			return info.Columns.Select(x => new FieldNameWrapper(x)).ToList();
		}
		public override string ToString() {
			return Caption;
		}
	}
}
