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

using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Services;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotDataField
	public class PivotDataField : PivotFieldReference {
		#region Fields
		readonly PivotTable pivotTable;
		string name;
		int baseField;
		int baseItem;
		int numberFormatIndex;
		PivotShowDataAs showDataAs;
		PivotDataConsolidateFunction subtotal;
		public const int DefaultBaseField = 0;
		public const int DefaultBaseItem = 0;
		#endregion
		public PivotDataField(PivotTable pivotTable, int fieldIndex)
			: base(fieldIndex) {
			Initialize();
			this.pivotTable = pivotTable;
		}
		void Initialize() {
			baseField = DefaultBaseField;
			baseItem = DefaultBaseItem;
			subtotal = PivotDataConsolidateFunction.Sum;
			showDataAs = PivotShowDataAs.Normal;
		}
		#region Properties
		public DocumentModel DocumentModel { get { return pivotTable.DocumentModel; } }
		public PivotTable PivotTable { get { return pivotTable; } }
		public bool HasBaseField { get { return ShouldHaveBaseField(showDataAs); } }
		public bool HasBaseItem { get { return ShouldHaveBaseItem(showDataAs); } }
		public int BaseField {
			get { return baseField; }
			set {
				if (BaseField != value) {
					pivotTable.CheckActiveTransaction();
					SetBaseField(value);
				}
			}
		}
		public int BaseItem {
			get { return baseItem; }
			set {
				if (BaseItem != value) {
					pivotTable.CheckActiveTransaction();
					SetBaseItem(value);
				}
			}
		}
		public string Name {
			get { return name; }
			set {
				if (Name != value) {
					pivotTable.CheckActiveTransaction();
					SetName(value);
				}
			}
		}
		public int NumberFormatIndex {
			get { return numberFormatIndex; }
			set {
				if (NumberFormatIndex != value) {
					pivotTable.CheckActiveTransaction();
					SetNumberFormatIndex(value);
				}
			}
		}
		public PivotShowDataAs ShowDataAs {
			get { return showDataAs; }
			set {
				if (ShowDataAs != value) {
					pivotTable.CheckActiveTransaction();
					SetShowDataAs(value);
				}
			}
		}
		public PivotDataConsolidateFunction Subtotal {
			get { return subtotal; }
			set {
				if (Subtotal != value) {
					pivotTable.CheckActiveTransaction();
					SetSubtotal(value);
				}
			}
		}
		#region FormatString
		public string FormatString { get { return DocumentModel.Cache.NumberFormatCache[NumberFormatIndex].FormatCode; } }
		public void SetFormatString(string newValue, IErrorHandler errorHandler) {
			if (StringExtensions.CompareInvariantCultureIgnoreCase(FormatString, newValue) == 0)
				return;
			pivotTable.BeginTransaction(errorHandler);
			try {
				NumberFormatIndex = NumberFormatHelper.GetNumberFormatIndex(newValue, DocumentModel);
			}
			finally {
				pivotTable.EndTransaction();
			}
		}
		#endregion
		#endregion
		public void SetBaseField(int value) {
			HistoryHelper.SetValue(DocumentModel, baseField, value, SetBaseFieldCore);
		}
		protected internal void SetBaseFieldCore(int value) {
			baseField = value;
			pivotTable.CalculationInfo.InvalidateWorksheetData();
		}
		protected internal void SetBaseItem(int value) {
			HistoryHelper.SetValue(DocumentModel, baseItem, value, SetBaseItemCore);
		}
		protected internal void SetBaseItemCore(int value) {
			baseItem = value;
			pivotTable.CalculationInfo.InvalidateWorksheetData();
		}
		protected internal void SetName(string value) {
			HistoryHelper.SetValue(DocumentModel, name, value, StringExtensions.ComparerInvariantCultureIgnoreCase, SetNameCore);
		}
		protected internal void SetNameCore(string value) {
			name = value;
			pivotTable.CalculationInfo.InvalidateWorksheetData();
		}
		protected internal void SetNumberFormatIndex(int value) {
			HistoryHelper.SetValue(DocumentModel, numberFormatIndex, value, SetNumberFormatIndexCore);
		}
		protected internal void SetNumberFormatIndexCore(int value) {
			numberFormatIndex = value;
			pivotTable.CalculationInfo.InvalidateWorksheetData();
		}
		protected internal void SetShowDataAs(PivotShowDataAs value) {
			HistoryHelper.SetValue(DocumentModel, showDataAs, value, SetShowDataAsCore);
		}
		protected internal void SetShowDataAsCore(PivotShowDataAs value) {
			showDataAs = value;
			pivotTable.CalculationInfo.InvalidateWorksheetData();
		}
		protected internal void SetSubtotal(PivotDataConsolidateFunction value) {
			HistoryHelper.SetValue(DocumentModel, subtotal, value, SetSubtotalCore);
		}
		protected internal void SetSubtotalCore(PivotDataConsolidateFunction value) {
			subtotal = value;
			pivotTable.CalculationInfo.InvalidateWorksheetData();
		}
		public void SetSubtotal(PivotDataConsolidateFunction value, IErrorHandler errorHandler) {
			if (subtotal == value)
				return;
			PivotTable.BeginTransaction(errorHandler);
			try {
				SetSubtotal(value);
				SetName(GenerateUniqueDefaultName());
			}
			finally {
				PivotTable.EndTransaction();
			}
		}
		public void SetShowValuesAs(PivotShowDataAs value, IErrorHandler errorHandler) {
			if (showDataAs == value)
				return;
			pivotTable.BeginTransaction(errorHandler);
			try {
				ShowDataAs = value;
			}
			finally {
				pivotTable.EndTransaction();
			}
		}
		public void SetShowValuesAs(PivotShowDataAs value, int baseField, IErrorHandler errorHandler) {
			if (showDataAs == value && this.baseField == baseField)
				return;
			pivotTable.BeginTransaction(errorHandler);
			try {
				ShowDataAs = value;
				BaseField = baseField;
			}
			finally {
				pivotTable.EndTransaction();
			}
		}
		public void SetShowValuesAs(PivotShowDataAs value, int baseField, int baseItem, IErrorHandler errorHandler) {
			if (showDataAs == value && this.baseField == baseField && this.baseItem == baseItem)
				return;
			pivotTable.BeginTransaction(errorHandler);
			try {
				ShowDataAs = value;
				BaseField = baseField;
				BaseItem = baseItem;
			}
			finally {
				pivotTable.EndTransaction();
			}
		}
		internal bool ShouldHaveBaseField(PivotShowDataAs value) {
			return ShouldHaveBaseItem(value) || value == PivotShowDataAs.PercentOfParent || value == PivotShowDataAs.RunningTotal ||
				   value == PivotShowDataAs.PercentOfRunningTotal || value == PivotShowDataAs.RankAscending || value == PivotShowDataAs.RankDescending;
		}
		internal bool ShouldHaveBaseItem(PivotShowDataAs value) {
			return value == PivotShowDataAs.Percent || value == PivotShowDataAs.Difference || value == PivotShowDataAs.PercentDifference;
		}
		#region GenerateUniqueName
		public string GetFullName() {
			if (!string.IsNullOrEmpty(name))
				return name;
			return GenerateDefaultName();
		}
		string GenerateUniqueDefaultName() {
			return GetUniqueName(string.Empty);
		}
		public void GenerateUniqueName(string name) {
			SetNameCore(GetUniqueName(name));
		}
		string GetUniqueName(string name) {
			if (string.IsNullOrEmpty(name))
				name = GenerateDefaultName();
			IPivotDataFieldNameCreationService service = DocumentModel.GetService<IPivotDataFieldNameCreationService>();
			if (service == null)
				Exceptions.ThrowInvalidOperationException("Name for the PivotDataField can not be assigned: service is missing.");
			return service.GetUniqueName(name, GetExistingNames());
		}
		string[] GetExistingNames() {
			int dataFieldsCount = PivotTable.DataFields.Count;
			int fieldsCount = PivotTable.Fields.Count;
			string[] names = new string[dataFieldsCount + fieldsCount];
			for (int i = 0; i < fieldsCount; ++i)
				names[i] = PivotTable.GetFieldCaption(i);
			for (int i = 0; i < dataFieldsCount; ++i)
				names[i + fieldsCount] = PivotTable.DataFields[i].Name;
			return names;
		}
		string GenerateDefaultName() {
			string subtotalCaption = XtraSpreadsheetLocalizer.GetString(PivotRefreshDataOnWorksheetCommand.GetSubtotalCaptionId((PivotFieldItemType)subtotal));
			string of = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_PartOfCustomName);
			string fieldName = PivotTable.GetFieldCaption(FieldIndex);
			return subtotalCaption + of + fieldName;
		}
		#endregion
		public override bool Equals(object obj) {
			PivotDataField other = obj as PivotDataField;
			if (other == null)
				return false;
			return FieldIndex == other.FieldIndex && name == other.Name && baseField == other.BaseField && baseItem == other.BaseItem &&
				   numberFormatIndex == other.NumberFormatIndex && showDataAs == other.ShowDataAs && subtotal == other.Subtotal;
		}
		public override int GetHashCode() {
			CombinedHashCode hashCode = new CombinedHashCode();
			hashCode.AddInt(FieldIndex);
			hashCode.AddInt(baseField);
			hashCode.AddInt(baseItem);
			hashCode.AddInt(numberFormatIndex);
			hashCode.AddInt((int)showDataAs);
			hashCode.AddInt((int)subtotal);
			hashCode.AddInt(name.GetHashCode());
			return hashCode.CombinedHash32;
		}
		public void CopyFromNoHistory(PivotDataField source) {
			System.Diagnostics.Debug.Assert(FieldIndex == source.FieldIndex);
			baseField = source.baseField;
			baseItem = source.baseItem;
			numberFormatIndex = NumberFormatHelper.GetNumberFormatIndex(source.FormatString, DocumentModel);
			showDataAs = source.ShowDataAs;
			subtotal = source.Subtotal;
			name = source.name;
		}
	}
	#endregion
	#region PivotDataFieldCollection
	public class PivotDataFieldCollection : PivotFieldReferenceCollection<PivotDataField> {
		public PivotDataFieldCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		public void CopyFromNoHistory(PivotTable newPivot, PivotDataFieldCollection source) {
			ClearCore();
			Capacity = source.Count;
			foreach (PivotDataField item in source) {
				PivotDataField newItem = new PivotDataField(newPivot, item.FieldIndex);
				newItem.CopyFromNoHistory(item);
				AddCore(newItem);
			}
		}
	}
	#endregion
	#region PivotDataConsolidateFunction
	public enum PivotDataConsolidateFunction {
		Average = PivotFieldItemType.Avg,
		Count = PivotFieldItemType.CountA,
		CountNums = PivotFieldItemType.Count,
		Max = PivotFieldItemType.Max,
		Min = PivotFieldItemType.Min,
		Product = PivotFieldItemType.Product,
		StdDev = PivotFieldItemType.StdDev,
		StdDevp = PivotFieldItemType.StdDevP,
		Sum = PivotFieldItemType.Sum,
		Var = PivotFieldItemType.Var,
		Varp = PivotFieldItemType.VarP,
	}
	#endregion
	#region PivotShowDataAs
	public enum PivotShowDataAs {
		Normal,
		Difference,
		Percent,
		PercentDifference,
		RunningTotal,
		PercentOfRow,
		PercentOfColumn,
		PercentOfTotal,
		Index,
		RankAscending,
		RankDescending,
		PercentOfRunningTotal,
		PercentOfParent,
		PercentOfParentRow,
		PercentOfParentColumn
	}
	#endregion
}
