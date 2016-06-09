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
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet {
	#region PivotDataConsolidationFunction
	public enum PivotDataConsolidationFunction {
		Average = DevExpress.XtraSpreadsheet.Model.PivotDataConsolidateFunction.Average,
		Count = DevExpress.XtraSpreadsheet.Model.PivotDataConsolidateFunction.Count,
		CountNumbers = DevExpress.XtraSpreadsheet.Model.PivotDataConsolidateFunction.CountNums,
		Max = DevExpress.XtraSpreadsheet.Model.PivotDataConsolidateFunction.Max,
		Min = DevExpress.XtraSpreadsheet.Model.PivotDataConsolidateFunction.Min,
		Product = DevExpress.XtraSpreadsheet.Model.PivotDataConsolidateFunction.Product,
		StdDev = DevExpress.XtraSpreadsheet.Model.PivotDataConsolidateFunction.StdDev,
		StdDevp = DevExpress.XtraSpreadsheet.Model.PivotDataConsolidateFunction.StdDevp,
		Sum = DevExpress.XtraSpreadsheet.Model.PivotDataConsolidateFunction.Sum,
		Var = DevExpress.XtraSpreadsheet.Model.PivotDataConsolidateFunction.Var,
		Varp = DevExpress.XtraSpreadsheet.Model.PivotDataConsolidateFunction.Varp,
	}
	#endregion
	#region PivotShowValuesAsType
	public enum PivotShowValuesAsType {
		NoCalculation = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.Normal,
		Difference = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.Difference,
		Percent = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.Percent,
		PercentDifference = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.PercentDifference,
		RunningTotal = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.RunningTotal,
		PercentOfRow = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.PercentOfRow,
		PercentOfColumn = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.PercentOfColumn,
		PercentOfTotal = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.PercentOfTotal,
		Index = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.Index,
		RankAscending = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.RankAscending,
		RankDescending = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.RankDescending,
		PercentOfRunningTotal = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.PercentOfRunningTotal,
		PercentOfParent = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.PercentOfParent,
		PercentOfParentRow = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.PercentOfParentRow,
		PercentOfParentColumn = DevExpress.XtraSpreadsheet.Model.PivotShowDataAs.PercentOfParentColumn
	}
	#endregion
	#region PivotBaseItemType
	public enum PivotBaseItemType {
		Specified,
		Previous,
		Next
	}
	#endregion
	#region PivotFieldReferenceBase
	public interface PivotFieldReferenceBase {
		PivotField Field { get; }
		void MoveUp();
		void MoveDown();
		void MoveToBeginning();
		void MoveToEnd();
	}
	#endregion
	#region PivotFieldReference
	public interface PivotFieldReference : PivotFieldReferenceBase {
		bool IsValuesReference { get; }
	}
	#endregion
	#region PivotPageField
	public interface PivotPageField : PivotFieldReferenceBase {
	}
	#endregion
	#region PivotDataField
	public interface PivotDataField : PivotFieldReferenceBase {
		string Name { get; set; }
		string NumberFormat { get; set; }
		PivotDataConsolidationFunction SummarizeValuesBy { get; set; }
		PivotShowValuesAsType ShowValuesAs { get; }
		PivotField BaseField { get; }
		PivotItem BaseItem { get; }
		PivotBaseItemType BaseItemType { get; }
		void ShowValuesWithCalculation(PivotShowValuesAsType calculationType);
		void ShowValuesWithCalculation(PivotShowValuesAsType calculationType, PivotField baseField);
		void ShowValuesWithCalculation(PivotShowValuesAsType calculationType, PivotField baseField, PivotItem baseItem);
		void ShowValuesWithCalculation(PivotShowValuesAsType calculationType, PivotField baseField, PivotBaseItemType baseItemType);
		void ShowValuesWithoutCalculation();
		CellValue GetPivotData(params FieldItemPair[] indices);
	}
	#endregion
	public class FieldItemPair {
		public FieldItemPair(int fieldIndex, int itemIndex) {
			this.FieldIndex = fieldIndex;
			this.ItemIndex = itemIndex;
		}
		public int FieldIndex { get; set; }
		public int ItemIndex { get; set; }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	#region NativePivotFieldReferenceBase
	abstract partial class NativePivotFieldReferenceBase<T, N, M> : NativeObjectBase, PivotFieldReferenceBase
		where T : PivotFieldReferenceBase
		where N : NativePivotFieldReferenceBase<T, N, M>, T
		where M : Model.IPivotFieldReference {
		#region Fields
		readonly NativePivotFieldReferenceCollectionBase<T, N, M> nativeCollection;
		readonly Model.PivotFieldReference modelReference;
		int referenceIndex;
		#endregion
		protected NativePivotFieldReferenceBase(NativePivotFieldReferenceCollectionBase<T, N, M> nativeCollection, Model.PivotFieldReference modelReference) {
			Guard.ArgumentNotNull(nativeCollection, "nativeCollection");
			Guard.ArgumentNotNull(modelReference, "modelReference");
			this.nativeCollection = nativeCollection;
			this.modelReference = modelReference;
		}
		#region Properties
		protected internal NativePivotTable ParentTable { get { return nativeCollection.Parent; } }
		protected internal int FieldIndex { get { return modelReference.FieldIndex; } }
		protected internal Model.PivotFieldReference ModelItem { get { return modelReference; } }
		protected internal int ReferenceIndex { get { return referenceIndex; } set { referenceIndex = value; } } 
		protected Model.PivotFieldReferenceCollection<M> ModelCollection { get { return nativeCollection.ModelCollection; } }
		protected ApiErrorHandler ErrorHandler { get { return ApiErrorHandler.Instance; } }
		#endregion
		#region PivotFieldReferenceBase members
		public PivotField Field {
			get {
				CheckValid();
				return modelReference.IsValuesReference ? null : ParentTable.Fields[FieldIndex];
			}
		}
		public void MoveUp() {
			MoveCore(Model.PivotItemMoveType.Up);
		}
		public void MoveDown() {
			MoveCore(Model.PivotItemMoveType.Down);
		}
		public void MoveToBeginning() {
			MoveCore(Model.PivotItemMoveType.ToBeginning);
		}
		public void MoveToEnd() {
			MoveCore(Model.PivotItemMoveType.ToEnd);
		}
		void MoveCore(Model.PivotItemMoveType moveType) {
			CheckValid();
			Model.MovePivotFieldItemCommandBase<M> command = CreateMoveCommand(moveType);
			command.Execute();
		}
		protected abstract Model.MovePivotFieldItemCommandBase<M> CreateMoveCommand(Model.PivotItemMoveType moveType);
		#endregion
	}
	#endregion
	#region NativePivotFieldReference
	partial class NativePivotFieldReference : NativePivotFieldReferenceBase<PivotFieldReference, NativePivotFieldReference, Model.PivotFieldReference>, PivotFieldReference {
		public NativePivotFieldReference(NativePivotFieldReferenceCollection nativeCollection, Model.PivotFieldReference modelReference)
			: base(nativeCollection, modelReference) {
		}
		public bool IsValuesReference {
			get {
				CheckValid();
				return ModelItem.IsValuesReference;
			}
		}
		protected override Model.MovePivotFieldItemCommandBase<Model.PivotFieldReference> CreateMoveCommand(Model.PivotItemMoveType moveType) {
			return new Model.MovePivotFieldReferenceCommand(ParentTable.ModelItem, ModelCollection, ReferenceIndex, moveType, ErrorHandler);
		}
	}
	#endregion
	#region NativePivotPageField
	partial class NativePivotPageField : NativePivotFieldReferenceBase<PivotPageField, NativePivotPageField, Model.PivotPageField>, PivotPageField {
		public NativePivotPageField(NativePivotPageFieldCollection nativeCollection, Model.PivotPageField modelPageField)
			: base(nativeCollection, modelPageField) {
		}
		protected override Model.MovePivotFieldItemCommandBase<Model.PivotPageField> CreateMoveCommand(Model.PivotItemMoveType moveType) {
			return new Model.MovePivotPageFieldCommand(ParentTable.ModelItem, ModelCollection, ReferenceIndex, moveType, ErrorHandler);
		}
	}
	#endregion
	#region NativePivotDataField
	partial class NativePivotDataField : NativePivotFieldReferenceBase<PivotDataField, NativePivotDataField, Model.PivotDataField>, PivotDataField {
		public NativePivotDataField(NativePivotDataFieldCollection nativeCollection, Model.PivotDataField modelDataField)
			: base(nativeCollection, modelDataField) {
		}
		protected internal new Model.PivotDataField ModelItem { get { return (Model.PivotDataField)base.ModelItem; } }
		#region Name
		public string Name {
			get {
				CheckValid();
				return ModelItem.Name;
			}
			set {
				CheckValid();
				Model.PivotDataFieldRenameCommand command = new Model.PivotDataFieldRenameCommand(ModelItem, value, ErrorHandler);
				command.Execute();
			}
		}
		#endregion
		#region NumberFormat
		public string NumberFormat {
			get {
				CheckValid();
				return ModelItem.FormatString;
			}
			set {
				CheckValid();
				ModelItem.SetFormatString(value, ErrorHandler);
			}
		}
		#endregion
		#region SummarizeValuesBy
		public PivotDataConsolidationFunction SummarizeValuesBy {
			get {
				CheckValid();
				return (PivotDataConsolidationFunction)ModelItem.Subtotal;
			}
			set {
				CheckValid();
				ModelItem.SetSubtotal((Model.PivotDataConsolidateFunction)value, ErrorHandler);
			}
		}
		#endregion
		#region ShowValuesAs
		public PivotShowValuesAsType ShowValuesAs {
			get {
				CheckValid();
				return (PivotShowValuesAsType)ModelItem.ShowDataAs;
			}
		}
		#endregion
		#region BaseField
		public PivotField BaseField {
			get {
				CheckValid();
				if (!ModelItem.HasBaseField)
					return null;
				return ParentTable.Fields[ModelItem.BaseField];
			}
		}
		#endregion
		#region BaseItemType
		public PivotBaseItemType BaseItemType {
			get {
				CheckValid();
				int baseItem = ModelItem.BaseItem;
				if (baseItem == Model.PivotTableLayoutCalculator.NextItem)
					return PivotBaseItemType.Next;
				else if (baseItem == Model.PivotTableLayoutCalculator.PreviousItem)
					return PivotBaseItemType.Previous;
				return PivotBaseItemType.Specified;
			}
		}
		#endregion
		#region BaseItem
		public PivotItem BaseItem {
			get {
				CheckValid();
				if (!ModelItem.HasBaseItem || BaseItemType != PivotBaseItemType.Specified)
					return null;
				return BaseField.Items[ModelItem.BaseItem];
			}
		}
		#endregion
		#region ShowValuesWithCalculation
		public void ShowValuesWithCalculation(PivotShowValuesAsType calculationType) {
			CheckValid();
			PivotReferenceShowValuesHelper helper = new PivotReferenceShowValuesHelper(calculationType);
			helper.Execute(this);
		}
		public void ShowValuesWithCalculation(PivotShowValuesAsType calculationType, PivotField baseField) {
			CheckValid();
			PivotReferenceShowValuesHelper helper = new PivotReferenceShowValuesHelper(calculationType, baseField);
			helper.Execute(this);
		}
		public void ShowValuesWithCalculation(PivotShowValuesAsType calculationType, PivotField baseField, PivotItem baseItem) {
			CheckValid();
			PivotReferenceShowValuesHelper helper = new PivotReferenceShowValuesHelper(calculationType, baseField, baseItem);
			helper.Execute(this);
		}
		public void ShowValuesWithCalculation(PivotShowValuesAsType calculationType, PivotField baseField, PivotBaseItemType baseItemType) {
			CheckValid();
			PivotReferenceShowValuesHelper helper = new PivotReferenceShowValuesHelper(calculationType, baseField, baseItemType);
			helper.Execute(this);
		}
		public void ShowValuesWithoutCalculation() {
			CheckValid();
			ModelItem.SetShowValuesAs(Model.PivotShowDataAs.Normal, ErrorHandler);
		}
		#endregion
		protected override Model.MovePivotFieldItemCommandBase<Model.PivotDataField> CreateMoveCommand(Model.PivotItemMoveType moveType) {
			return new Model.MovePivotDataFieldCommand(ParentTable.ModelItem, ModelCollection, ReferenceIndex, moveType, ErrorHandler);
		}
		public CellValue GetPivotData(params FieldItemPair[] indices) {
			CheckValid();
			Model.VariantValue value = Model.GetPivotDataCalculator.GetPivotData(ModelItem.PivotTable, ReferenceIndex, indices);
			Model.DocumentModel modelBook = ModelItem.PivotTable.Worksheet.Workbook;
			CellValue result = new CellValue(value, modelBook.DataContext);
			bool isDateTime = modelBook.Cache.NumberFormatCache[ModelItem.NumberFormatIndex].IsDateTime;
			result.ChangeModelValue(value, isDateTime);
			return result;
		}
	}
	#endregion
	#region PivotReferenceShowValuesHelper
	partial class PivotReferenceShowValuesHelper {
		#region Fields
		readonly Model.PivotShowDataAs calculationType;
		readonly PivotField baseField;
		readonly PivotItem baseItem;
		readonly PivotBaseItemType? baseItemType;
		#endregion
		#region Constructors
		public PivotReferenceShowValuesHelper(PivotShowValuesAsType calculationType) {
			this.calculationType = (Model.PivotShowDataAs)calculationType;
		}
		public PivotReferenceShowValuesHelper(PivotShowValuesAsType calculationType, PivotField baseField)
			: this(calculationType) {
			this.baseField = baseField;
		}
		public PivotReferenceShowValuesHelper(PivotShowValuesAsType calculationType, PivotField baseField, PivotItem baseItem)
			: this(calculationType, baseField) {
			this.baseItem = baseItem;
		}
		public PivotReferenceShowValuesHelper(PivotShowValuesAsType calculationType, PivotField baseField, PivotBaseItemType baseItemType)
			: this(calculationType, baseField) {
			this.baseItemType = baseItemType;
		}
		#endregion
		ApiErrorHandler ErrorHandler { get { return ApiErrorHandler.Instance; } }
		public void Execute(NativePivotDataField dataField) {
			Guard.ArgumentNotNull(dataField, "dataField");
			PivotTable parentTable = dataField.ParentTable;
			Model.PivotDataField modelDataField = dataField.ModelItem;
			bool shouldHaveBaseField = modelDataField.ShouldHaveBaseField(calculationType);
			bool shouldHaveBaseItem = modelDataField.ShouldHaveBaseItem(calculationType);
			if (!shouldHaveBaseField && !shouldHaveBaseItem)
				modelDataField.SetShowValuesAs(calculationType, ErrorHandler);
			else if (shouldHaveBaseField && !shouldHaveBaseItem)
				modelDataField.SetShowValuesAs(calculationType, GetFieldIndex(parentTable), ErrorHandler);
			else
				modelDataField.SetShowValuesAs(calculationType, GetFieldIndex(parentTable), GetItemIndex(), ErrorHandler);
		}
		int GetFieldIndex(PivotTable table) {
			if (baseField == null)
				ErrorHandler.HandleError(Model.ModelErrorType.PivotCalculationRequiresField);
			NativePivotField nativeBaseField = (NativePivotField)baseField;
			if (!Object.ReferenceEquals(table, nativeBaseField.ParentTable))
				ErrorHandler.HandleError(Model.ModelErrorType.UsingInvalidObject);
			return nativeBaseField.FieldIndex;
		}
		int GetItemIndex() {
			if (baseItem == null && !baseItemType.HasValue)
				ErrorHandler.HandleError(Model.ModelErrorType.PivotCalculationRequiresItem);
			if (baseItem != null) {
				NativePivotItem nativeBaseItem = (NativePivotItem)baseItem;
				if (!Object.ReferenceEquals(baseField, nativeBaseItem.Parent))
					ErrorHandler.HandleError(Model.ModelErrorType.UsingInvalidObject);
				return nativeBaseItem.ItemIndex;
			}
			PivotBaseItemType baseItemTypeValue = baseItemType.Value;
			if (baseItemTypeValue == PivotBaseItemType.Specified)
				ErrorHandler.HandleError(Model.ModelErrorType.PivotCalculationRequiresItem);
			return baseItemTypeValue == PivotBaseItemType.Next ? Model.PivotTableLayoutCalculator.NextItem : Model.PivotTableLayoutCalculator.PreviousItem;
		}
	}
	#endregion
}
