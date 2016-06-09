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
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region IgnoredErrorType
	[Flags]
	public enum IgnoredErrorType {
		None = 0x000,
		InconsistentColumnFormula = 0x001,
		InconsistentFormula = 0x002,
		FormulaRange = 0x004,
		TextDate = 0x008,
		EmptyCellReferences = 0x010,
		ListDataValidation = 0x020,
		EvaluateToError = 0x040,
		NumberAsText = 0x080,
		UnlockedFormula = 0x100
	}
	#endregion
	#region IgnoredErrorInfo
	public class IgnoredErrorInfo : ICloneable<IgnoredErrorInfo>, ISupportsCopyFrom<IgnoredErrorInfo>, ISupportsSizeOf {
		#region Fields
		const uint MaskInconsistentColumnFormula = 0x00000001; 
		const uint MaskEmptyCellReferences = 0x00000002; 
		const uint MaskEvaluateToError = 0x00000004; 
		const uint MaskInconsistentFormula = 0x00000008; 
		const uint MaskFormulaRangeError = 0x00000010; 
		const uint MaskListDataValidation = 0x00000020; 
		const uint MaskNumberAsText = 0x00000040; 
		const uint MaskTwoDidgitTextYear = 0x00000080; 
		const uint MaskUnlockedFormula = 0x00000100; 
		uint packedValues;
		#endregion
		#region Properties
		public bool InconsistentColumnFormula {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskInconsistentColumnFormula); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskInconsistentColumnFormula, value); }
		}
		public bool EmptyCellReferences {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskEmptyCellReferences); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskEmptyCellReferences, value); }
		}
		public bool EvaluateToError {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskEvaluateToError); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskEvaluateToError, value); }
		}
		public bool InconsistentFormula {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskInconsistentFormula); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskInconsistentFormula, value); }
		}
		public bool FormulaRangeError {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskFormulaRangeError); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskFormulaRangeError, value); }
		}
		public bool ListDataValidation {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskListDataValidation); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskListDataValidation, value); }
		}
		public bool NumberAsText {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskNumberAsText); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskNumberAsText, value); }
		}
		public bool TwoDidgitTextYear {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskTwoDidgitTextYear); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskTwoDidgitTextYear, value); }
		}
		public bool UnlockedFormula {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskUnlockedFormula); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskUnlockedFormula, value); }
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		#region ICloneable<SparklineGroupInfo> Members
		public IgnoredErrorInfo Clone() {
			IgnoredErrorInfo result = new IgnoredErrorInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<SparklineGroupInfo> Members
		public void CopyFrom(IgnoredErrorInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			IgnoredErrorInfo info = obj as IgnoredErrorInfo;
			if (info == null)
				return false;
			return packedValues == info.packedValues;
		}
		public override int GetHashCode() {
			return packedValues.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region IgnoredErrorInfoCache
	public class IgnoredErrorInfoCache : UniqueItemsCache<IgnoredErrorInfo> {
		public const int DefaultItemIndex = 0;
		public IgnoredErrorInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override IgnoredErrorInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new IgnoredErrorInfo();
		}
	}
	#endregion
	#region IgnoredErrorCollection
	public class IgnoredErrorCollection : UndoableCollection<IgnoredError> {
		public IgnoredErrorCollection(Worksheet sheet) : base(sheet) { }
		public void Clear(CellRangeBase range) {
			for (int i = Count - 1; i >= 0; i--)
				InnerList[i].TryClearRange(range);
		}
		#region Notifications
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			for (int i = Count - 1; i >= 0; i--)
				InnerList[i].OnRangeRemoving(context.Range, context.Mode);
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			for (int i = Count - 1; i >= 0; i--)
				InnerList[i].OnRangeInserting(context.Range, context.Mode);
		}
		#endregion
	}
	#endregion
	#region IgnoredError
	public class IgnoredError : SpreadsheetUndoableIndexBasedObject<IgnoredErrorInfo>, ISupportsCopyFrom<IgnoredError> {
		#region Static Members
		public static IgnoredError Create(CellRangeBase range, IgnoredErrorType errorType) {
			Worksheet sheet = range.Worksheet as Worksheet;
			IgnoredError result = new IgnoredError(sheet, range);
			sheet.Workbook.BeginUpdate();
			try {
				if ((errorType & IgnoredErrorType.EmptyCellReferences) != 0)
					result.EmptyCellReferences = true;
				if ((errorType & IgnoredErrorType.EvaluateToError) != 0)
					result.EvaluateToError = true;
				if ((errorType & IgnoredErrorType.FormulaRange) != 0)
					result.FormulaRangeError = true;
				if ((errorType & IgnoredErrorType.InconsistentColumnFormula) != 0)
					result.InconsistentColumnFormula = true;
				if ((errorType & IgnoredErrorType.InconsistentFormula) != 0)
					result.InconsistentFormula = true;
				if ((errorType & IgnoredErrorType.ListDataValidation) != 0)
					result.ListDataValidation = true;
				if ((errorType & IgnoredErrorType.NumberAsText) != 0)
					result.NumberAsText = true;
				if ((errorType & IgnoredErrorType.TextDate) != 0)
					result.TwoDidgitTextYear = true;
				if ((errorType & IgnoredErrorType.UnlockedFormula) != 0)
					result.UnlockedFormula = true;
			}
			finally {
				sheet.Workbook.EndUpdate();
			}
			return result;
		}
		#endregion
		CellRangeBase range;
		public IgnoredError(Worksheet sheet, CellRangeBase range)
			: base(sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			CheckErrorRange(sheet, range);
			this.range = range;
		}
		#region Properties
		public Worksheet Sheet { get { return (Worksheet)DocumentModelPart; } }
		#region Range
		public CellRangeBase Range {
			get { return range; }
			set {
				CheckErrorRange(Sheet, value);
				if (range.Equals(value))
					return;
				IgnoredErrorRangeChangedHistoryItem item = new IgnoredErrorRangeChangedHistoryItem(this, range, value);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		void CheckErrorRange(Worksheet sheet, CellRangeBase range) {
			Guard.ArgumentNotNull(range, "range");
			if (!Object.ReferenceEquals(Sheet, range.Worksheet))
				throw new NullReferenceException("CellRange from different worksheet!");
		}
		protected internal void SetRangeCore(CellRangeBase range) {
			this.range = range;
		}
		#endregion
		#region InconsistentColumnFormula
		public bool InconsistentColumnFormula {
			get { return Info.InconsistentColumnFormula; }
			set {
				if (InconsistentColumnFormula == value)
					return;
				SetPropertyValue(SetInconsistentColumnFormulaCore, value);
			}
		}
		DocumentModelChangeActions SetInconsistentColumnFormulaCore(IgnoredErrorInfo info, bool value) {
			info.InconsistentColumnFormula = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region InconsistentFormula
		public bool InconsistentFormula {
			get { return Info.InconsistentFormula; }
			set {
				if (InconsistentFormula == value)
					return;
				SetPropertyValue(SetInconsistentFormulaCore, value);
			}
		}
		DocumentModelChangeActions SetInconsistentFormulaCore(IgnoredErrorInfo info, bool value) {
			info.InconsistentFormula = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region EmptyCellReferences
		public bool EmptyCellReferences {
			get { return Info.EmptyCellReferences; }
			set {
				if (EmptyCellReferences == value)
					return;
				SetPropertyValue(SetEmptyCellReferencesCore, value);
			}
		}
		DocumentModelChangeActions SetEmptyCellReferencesCore(IgnoredErrorInfo info, bool value) {
			info.EmptyCellReferences = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region EvaluateToError
		public bool EvaluateToError {
			get { return Info.EvaluateToError; }
			set {
				if (EvaluateToError == value)
					return;
				SetPropertyValue(SetEvaluateToErrorCore, value);
			}
		}
		DocumentModelChangeActions SetEvaluateToErrorCore(IgnoredErrorInfo info, bool value) {
			info.EvaluateToError = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FormulaRangeError
		public bool FormulaRangeError {
			get { return Info.FormulaRangeError; }
			set {
				if (FormulaRangeError == value)
					return;
				SetPropertyValue(SetFormulaRangeErrorCore, value);
			}
		}
		DocumentModelChangeActions SetFormulaRangeErrorCore(IgnoredErrorInfo info, bool value) {
			info.FormulaRangeError = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ListDataValidation
		public bool ListDataValidation {
			get { return Info.ListDataValidation; }
			set {
				if (ListDataValidation == value)
					return;
				SetPropertyValue(SetListDataValidationCore, value);
			}
		}
		DocumentModelChangeActions SetListDataValidationCore(IgnoredErrorInfo info, bool value) {
			info.ListDataValidation = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region NumberAsText
		public bool NumberAsText {
			get { return Info.NumberAsText; }
			set {
				if (NumberAsText == value)
					return;
				SetPropertyValue(SetNumberAsTextCore, value);
			}
		}
		DocumentModelChangeActions SetNumberAsTextCore(IgnoredErrorInfo info, bool value) {
			info.NumberAsText = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region TwoDidgitTextYear
		public bool TwoDidgitTextYear {
			get { return Info.TwoDidgitTextYear; }
			set {
				if (TwoDidgitTextYear == value)
					return;
				SetPropertyValue(SetTwoDidgitTextYearCore, value);
			}
		}
		DocumentModelChangeActions SetTwoDidgitTextYearCore(IgnoredErrorInfo info, bool value) {
			info.TwoDidgitTextYear = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region UnlockedFormula
		public bool UnlockedFormula {
			get { return Info.UnlockedFormula; }
			set {
				if (UnlockedFormula == value)
					return;
				SetPropertyValue(SetUnlockedFormulaCore, value);
			}
		}
		DocumentModelChangeActions SetUnlockedFormulaCore(IgnoredErrorInfo info, bool value) {
			info.UnlockedFormula = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		#region ISupportsCopyFrom<IgnoredError> Members
		public void CopyFrom(IgnoredError other) {
			Guard.ArgumentNotNull(other, "other");
			DocumentModel.BeginUpdate();
			try {
				base.CopyFrom(other);
				this.range = other.Range.Clone(Sheet);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region SpreadsheetUndoableIndexBasedObject Members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<IgnoredErrorInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.IgnoredErrorInfoCache;
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			IgnoredError other = obj as IgnoredError;
			if (other == null)
				return false;
			return Info.Equals(other.Info) && range.Equals(other.Range);
		}
		public override int GetHashCode() {
			return Info.GetHashCode() ^ range.GetHashCode();
		}
		#endregion
		#region Internal
		internal IgnoredErrorType GetErrorType() {
			IgnoredErrorType result = IgnoredErrorType.None;
			if (EmptyCellReferences)
				result |= IgnoredErrorType.EmptyCellReferences;
			if (EvaluateToError)
				result |= IgnoredErrorType.EvaluateToError;
			if (FormulaRangeError)
				result |= IgnoredErrorType.FormulaRange;
			if (InconsistentColumnFormula)
				result |= IgnoredErrorType.InconsistentColumnFormula;
			if (InconsistentFormula)
				result |= IgnoredErrorType.InconsistentFormula;
			if (ListDataValidation)
				result |= IgnoredErrorType.ListDataValidation;
			if (NumberAsText)
				result |= IgnoredErrorType.NumberAsText;
			if (TwoDidgitTextYear)
				result |= IgnoredErrorType.TextDate;
			if (UnlockedFormula)
				result |= IgnoredErrorType.UnlockedFormula;
			return result;
		}
		#endregion
		#region Notifications
		public void OnRangeRemoving(CellRange removingRange, RemoveCellMode mode) {
			Guard.ArgumentNotNull(removingRange, "removingRange");
			if (!TryClearRange(removingRange)) {
				if (mode == RemoveCellMode.ShiftCellsLeft)
					range = range.GetForceShifted(removingRange, true, false);
				else if (mode == RemoveCellMode.ShiftCellsUp)
					range = range.GetForceShifted(removingRange, false, false);
			}
		}
		public void OnRangeInserting(CellRange insertingRange, InsertCellMode mode) {
			Guard.ArgumentNotNull(insertingRange, "insertingRange");
			range = range.GetForceShifted(insertingRange, mode == InsertCellMode.ShiftCellsRight, true);
		}
		internal bool TryClearRange(CellRangeBase removingRange) {
			if (!range.Intersects(removingRange))
				return false;
			CellRangeBase remainingRange = range.ExcludeRange(removingRange);
			if (remainingRange != null) {
				Range = remainingRange;
				return false;
			}
			Sheet.IgnoredErrors.Remove(this);
			return true;
		}
		#endregion
	}
	#endregion
}
