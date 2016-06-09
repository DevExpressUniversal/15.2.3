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
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DataValidationCollection
	public class DataValidationCollection : UndoableCollection<DataValidation> {
		public DataValidationCollection(IDocumentModel documentModel)
			: base(documentModel.MainPart) {
		}
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			List<DataValidation> splittedParts = new List<DataValidation>();
			for (int i = Count - 1; i >= 0; i--)
				splittedParts.AddRange(InnerList[i].OnRangeInserting(notificationContext));
			AddSplittedParts(splittedParts);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			List<DataValidation> splittedParts = new List<DataValidation>();
			for (int i = Count - 1; i >= 0; i--) {
				List<DataValidation> currentParts = InnerList[i].OnRangeRemoving(notificationContext);
				if (currentParts != null)
					splittedParts.AddRange(currentParts);
			}
			AddSplittedParts(splittedParts);
		}
		public void ShiftReferences(ShiftReferencesFromSourceRangeToTargetAfterPasteCutRangeWalker walker) {
			List<DataValidation> splittedParts = new List<DataValidation>();
			for (int i = Count - 1; i >= 0; i--)
				splittedParts.AddRange(InnerList[i].ShiftReferences(walker));
			AddSplittedParts(splittedParts);
		}
		void AddSplittedParts(List<DataValidation> splittedParts) {
			foreach (DataValidation item in splittedParts)
				AddOrMergeWithExisting(item);
		}
		public void CheckIntegrity(CheckIntegrityFlags flags) {
			for (int i = 0; i < Count; i++) {
				this[i].CheckIntegrity(flags);
			}
		}
		public List<DataValidation> FindAll(Predicate<DataValidation> match) {
			return this.InnerList.FindAll(match);
		}
		public List<DataValidation> LookupDataValidations(CellPosition position) {
			return this.FindAll((item) => item.CellRange.ContainsCell(position.Column, position.Row));
		}
		public List<DataValidation> LookupDataValidations(CellRange range) {
			return this.FindAll((item) => item.CellRange.Intersects(range));
		}
		public void ClearRange(CellRangeBase range) {
			ClearRange(range, new List<DataValidation>());
		}
		public void ClearRange(CellRangeBase range, List<DataValidation> itemsToSkip) {
			for (int i = Count - 1; i >= 0; i--) {
				DataValidation item = InnerList[i];
				CellRangeBase itemRange = item.CellRange;
				if (itemRange.Intersects(range) && !ShouldSkipClearing(itemsToSkip, i)) {
					CellRangeBase newRange = itemRange.ExcludeRange(range);
					if (newRange != null)
						item.CellRange = newRange;
					else
						RemoveAt(i);
				}
			}
		}
		bool ShouldSkipClearing(List<DataValidation> itemsToSkip, int current) {
			int count = itemsToSkip.Count;
			for (int i = 0; i < count; i++)
				if (itemsToSkip[i].Index == current + 1)
					return true;
			return false;
		}
		DataValidation FindEqual(DataValidation item) {
			foreach (DataValidation existingItem in this)
				if (!Object.ReferenceEquals(item, existingItem) && item.EqualsWithoutRange(existingItem))
					return existingItem;
			return null;
		}
		public bool HasEqual(DataValidation item) {
			return FindEqual(item) != null;
		}
		public void AddOrMergeWithExisting(DataValidation newItem) {
			DataValidation existingItem = FindEqual(newItem);
			if (existingItem != null)
				existingItem.CellRange = existingItem.CellRange.MergeWithRange(newItem.CellRange);
			else
				Add(newItem);
		}
	}
	#endregion
	#region DataValidation
	public class DataValidation : SpreadsheetUndoableIndexBasedObject<DataValidationInfo> {
		#region Fields
		CellRangeBase cellRange;
		ParsedExpression expression1;
		ParsedExpression expression2;
		DataValidator validator;
		#endregion
		public DataValidation(CellRangeBase cellRange, Worksheet sheet)
			: base(sheet) {
			this.cellRange = cellRange;
			this.expression1 = new ParsedExpression();
			this.expression2 = new ParsedExpression();
		}
		#region Properties
		public Worksheet Sheet { get { return (Worksheet)DocumentModelPart; } }
		public CellRangeBase CellRange {
			get { return cellRange; }
			set {
				if(cellRange.Equals(value))
					return;
				DocumentHistory history = DocumentModel.History;
				ChangeDataValidationRangeHistoryItem item = new ChangeDataValidationRangeHistoryItem(this, cellRange, value);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetRangeCore(CellRangeBase range) {
			this.cellRange = range;
		}
		WorkbookDataContext DataContext { get { return DocumentModel.DataContext; } }
		protected override void OnIndexChanged() {
			base.OnIndexChanged();
			validator = null;
		}
		#region Type
		public DataValidationType Type {
			get { return Info.Type; }
			set {
				if (Type == value)
					return;
				SetPropertyValue(SetTypeCore, value);
			}
		}
		DocumentModelChangeActions SetTypeCore(DataValidationInfo info, DataValidationType value) {
			info.Type = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ErrorStyle
		public DataValidationErrorStyle ErrorStyle {
			get { return Info.ErrorStyle; }
			set {
				if (ErrorStyle == value)
					return;
				SetPropertyValue(SetErrorStyleCore, value);
			}
		}
		DocumentModelChangeActions SetErrorStyleCore(DataValidationInfo info, DataValidationErrorStyle value) {
			info.ErrorStyle = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ImeMode
		public DataValidationImeMode ImeMode {
			get { return Info.ImeMode; }
			set {
				if (ImeMode == value)
					return;
				SetPropertyValue(SetImeModeCore, value);
			}
		}
		DocumentModelChangeActions SetImeModeCore(DataValidationInfo info, DataValidationImeMode value) {
			info.ImeMode = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ValidationOperator
		public DataValidationOperator ValidationOperator {
			get { return Info.ValidationOperator; }
			set {
				if (ValidationOperator == value)
					return;
				SetPropertyValue(SetValidationOperatorCore, value);
			}
		}
		DocumentModelChangeActions SetValidationOperatorCore(DataValidationInfo info, DataValidationOperator value) {
			info.ValidationOperator = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region AllowBlank
		public bool AllowBlank {
			get { return Info.AllowBlank; }
			set {
				if (AllowBlank == value)
					return;
				SetPropertyValue(SetAllowBlankCore, value);
			}
		}
		DocumentModelChangeActions SetAllowBlankCore(DataValidationInfo info, bool value) {
			info.AllowBlank = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SuppressDropDown
		public bool SuppressDropDown {
			get { return Info.SuppressDropDown; }
			set {
				if (SuppressDropDown == value)
					return;
				SetPropertyValue(SetSuppressDropDownCore, value);
			}
		}
		DocumentModelChangeActions SetSuppressDropDownCore(DataValidationInfo info, bool value) {
			info.SuppressDropDown = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowInputMessage
		public bool ShowInputMessage {
			get { return Info.ShowInputMessage; }
			set {
				if (ShowInputMessage == value)
					return;
				SetPropertyValue(SetShowInputMessageCore, value);
			}
		}
		DocumentModelChangeActions SetShowInputMessageCore(DataValidationInfo info, bool value) {
			info.ShowInputMessage = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowErrorMessage
		public bool ShowErrorMessage {
			get { return Info.ShowErrorMessage; }
			set {
				if (ShowErrorMessage == value)
					return;
				SetPropertyValue(SetShowErrorMessageCore, value);
			}
		}
		DocumentModelChangeActions SetShowErrorMessageCore(DataValidationInfo info, bool value) {
			info.ShowErrorMessage = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ErrorTitle
		public string ErrorTitle {
			get { return Info.ErrorTitle; }
			set {
				if (ErrorTitle == value)
					return;
				SetPropertyValue(SetErrorTitleCore, value);
			}
		}
		DocumentModelChangeActions SetErrorTitleCore(DataValidationInfo info, string value) {
			info.ErrorTitle = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Error
		public string Error {
			get { return Info.Error; }
			set {
				if (Error == value)
					return;
				SetPropertyValue(SetErrorCore, value);
			}
		}
		DocumentModelChangeActions SetErrorCore(DataValidationInfo info, string value) {
			info.Error = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PromptTitle
		public string PromptTitle {
			get { return Info.PromptTitle; }
			set {
				if (PromptTitle == value)
					return;
				SetPropertyValue(SetPromptTitleCore, value);
			}
		}
		DocumentModelChangeActions SetPromptTitleCore(DataValidationInfo info, string value) {
			info.PromptTitle = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Prompt
		public string Prompt {
			get { return Info.Prompt; }
			set {
				if (Prompt == value)
					return;
				SetPropertyValue(SetPromptCore, value);
			}
		}
		DocumentModelChangeActions SetPromptCore(DataValidationInfo info, string value) {
			info.Prompt = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Formula1
		public string Formula1 {
			get { return GetReference(CellPosition.InvalidValue, expression1, string.Empty); }
			set { Expression1 = PrepareExpression(value, CellPosition.InvalidValue); }
		}
		#endregion
		#region Formula2
		public string Formula2 {
			get { return GetReference(CellPosition.InvalidValue, expression2, string.Empty); }
			set { Expression2 = PrepareExpression(value, CellPosition.InvalidValue); }
		}
		#endregion
		#region Expression1
		public ParsedExpression Expression1 {
			get {
				return expression1;
			}
			set {
				if (value == null)
					value = new ParsedExpression();
				if (this.expression1.Equals(value))
					return;
				DocumentHistory history = DocumentModel.History;
				ChangeDataValidationExpressionHistoryItem item = new ChangeDataValidationExpressionHistoryItem(this, this.expression1, value, true);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetExpression1Core(ParsedExpression expression) {
			this.expression1 = expression;
			this.validator = null;
		}
		#endregion
		#region Expression2
		public ParsedExpression Expression2 {
			get {
				return expression2;
			}
			set {
				if (value == null)
					value = new ParsedExpression();
				if (this.expression2.Equals(value))
					return;
				DocumentHistory history = DocumentModel.History;
				ChangeDataValidationExpressionHistoryItem item = new ChangeDataValidationExpressionHistoryItem(this, this.expression2, value, false);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetExpression2Core(ParsedExpression expression) {
			this.expression2 = expression;
			this.validator = null;
		}
		#endregion
		DataValidationCollection Collection { get { return Sheet.DataValidations; } }
		#endregion
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<DataValidationInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.DataValidationInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			Sheet.ApplyChanges(changeActions);
		}
		#region GetFormula
		public string GetFormula1(ICell relativeToCell) {
			return GetReference(new CellPosition(relativeToCell.ColumnIndex, relativeToCell.RowIndex), expression1, string.Empty);
		}
		public string GetFormula1(CellPosition relativeToPosition) {
			return GetReference(relativeToPosition, expression1, string.Empty);
		}
		public string GetFormula2(CellPosition relativeToPosition) {
			return GetReference(relativeToPosition, expression2, string.Empty);
		}
		public string GetFormula2(ICell relativeToCell) {
			return GetReference(new CellPosition(relativeToCell.ColumnIndex, relativeToCell.RowIndex), expression2, string.Empty);
		}
		string GetReference(CellPosition relativeToPosition, ParsedExpression expression, string formula) {
			if (expression == null || expression.Count < 1)
				return formula;
			else {
				ValueParsedThing valueExpression = expression[0] as ValueParsedThing;
				if (valueExpression != null)
					return valueExpression.GetValue().ToText(DataContext).InlineTextValue;
				WorkbookDataContext dataContext = DataContext;
				dataContext.PushRelativeToCurrentCell(true);
				if (relativeToPosition.IsValid)
					dataContext.PushCurrentCell(relativeToPosition);
				else
					dataContext.PushCurrentCell(CellRange.TopLeft);
				try {
					return "=" + expression.BuildExpressionString(dataContext);
				}
				finally {
					dataContext.PopRelativeToCurrentCell();
					dataContext.PopCurrentCell();
				}
			}
		}
		#endregion
		#region SetFormula
		public void SetFormula1(string value, CellPosition relativeToPosition) {
			Expression1 = PrepareExpression(value, relativeToPosition);
		}
		public void SetFormula2(string value, CellPosition relativeToPosition) {
			Expression2 = PrepareExpression(value, relativeToPosition);
		}
		public void SetFormula1(ParsedExpression expression) {
			Expression1 = expression;
		}
		public void SetFormula2(ParsedExpression expression) {
			Expression2 = expression;
		}
		ParsedExpression PrepareExpression(string formula, CellPosition relativeToPosition) {
			validator = null;
			if (string.IsNullOrEmpty(formula))
				return null;
			WorkbookDataContext dataContext = DataContext;
			ParsedExpression expression = new ParsedExpression();
			if (!formula.StartsWith("=", StringComparison.Ordinal)) {
				if (Type == DataValidationType.List)
					expression.Add(ValueParsedThing.CreateInstance(formula, DataContext));
				else
					expression.Add(ValueParsedThing.CreateInstance(DataContext.ConvertTextToVariantValueWithCaching(formula), DataContext));
			}
			else {
				dataContext.PushRelativeToCurrentCell(true);
				if (relativeToPosition.IsValid)
					dataContext.PushCurrentCell(relativeToPosition);
				else
					dataContext.PushCurrentCell(CellRange.TopLeft);
				try {
					if (Type == DataValidationType.List)
						expression = dataContext.ParseExpression(formula, OperandDataType.Reference, true);
					else
						expression = dataContext.ParseExpression(formula, OperandDataType.Value, true);
					if (expression == null)
						Exceptions.ThrowArgumentException("Error in formula", formula);
				}
				finally {
					dataContext.PopRelativeToCurrentCell();
					dataContext.PopCurrentCell();
				}
			}
			return expression;
		}
		#endregion
		protected internal void SetFormulas(string formula1, string formula2) {
			if (Type != DataValidationType.None) {
				if (!string.IsNullOrEmpty(formula1))
					Formula1 = formula1;
				if (!string.IsNullOrEmpty(formula2))
					if (Type != DataValidationType.List && Type != DataValidationType.Custom)
						if (ValidationOperator == DataValidationOperator.Between || ValidationOperator == DataValidationOperator.NotBetween)
							Formula2 = formula2;
			}
		}
		public void CheckIntegrity(CheckIntegrityFlags flags) {
			if (Type == DataValidationType.None)
				return;
			if (validator == null)
				validator = PrepareValidator(Type);
			validator.CheckIntegrity(ValidationOperator);
		}
		#region ValidateValue
		public bool ValidateValue(VariantValue value, CellPosition currentCellPosition) {
			if (Type == DataValidationType.None || (value.IsEmpty && AllowBlank))
				return true;
			if (validator == null)
				validator = PrepareValidator(Type);
			return validator.Validate(Sheet, value, currentCellPosition, ValidationOperator, AllowBlank);
		}
		DataValidator PrepareValidator(DataValidationType validationType) {
			DataValidator result;
			switch (validationType) {
				case DataValidationType.Date:
					result = new DataValidatorDate(DocumentModel);
					break;
				case DataValidationType.Decimal:
					result = new DataValidatorDecimal(DocumentModel);
					break;
				case DataValidationType.List:
					result = new DataValidatorList(DocumentModel);
					break;
				case DataValidationType.TextLength:
					result = new DataValidatorTextLength(DocumentModel);
					break;
				case DataValidationType.Time:
					result = new DataValidatorTime(DocumentModel);
					break;
				case DataValidationType.Whole:
					result = new DataValidatorWhole(DocumentModel);
					break;
				case DataValidationType.Custom:
					result = new DataValidatorCustom(DocumentModel);
					break;
				default: throw new ArgumentException("Unknown validation type:" + Type);
			}
			result.SetExpression1(expression1);
			result.SetExpression2(expression2);
			return result;
		}
		#endregion
		public bool UpdateInvalidDataCircles() {
			HashSet<CellKey> invalidDataCircles = Sheet.InvalidDataCircles;
			foreach (CellKey key in cellRange.GetAllCellKeysEnumerable()) {
				ICell cell = Sheet.TryGetCell(key.ColumnIndex, key.RowIndex);
				VariantValue value = cell == null ? VariantValue.Empty : cell.Value;
				if (!ValidateValue(value, new CellPosition(key.ColumnIndex, key.RowIndex))) {
					if (invalidDataCircles.Count == 255)
						return false;
					invalidDataCircles.Add(key);
				}
				else
					invalidDataCircles.Remove(key);
			}
			return true;
		}
		#region Notifications
		public List<DataValidation> OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			RemoveCellMode mode = notificationContext.Mode;
			if (mode == RemoveCellMode.Default)
				return null;
			if (mode == RemoveCellMode.NoShiftOrRangeToPasteCutRange && notificationContext.SuppressDataValidationSplit) {
				ProcessCopiedRange(null, notificationContext.Range);
				return null;
			}
			DataValidationNotificationsProcessorBase processor = DataValidationNotificationsProcessorBase.GetProcessor(CreateNotificationInfo(), notificationContext);
			return ProcessSplittedParts(processor);
		}
		public List<DataValidation> OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			DataValidationNotificationsProcessorBase processor = DataValidationNotificationsProcessorBase.GetProcessor(CreateNotificationInfo(), notificationContext);
			return ProcessSplittedParts(processor);
		}
		public List<DataValidation> ShiftReferences(ShiftReferencesFromSourceRangeToTargetAfterPasteCutRangeWalker walker) {
			DataValidationProcessorForCutOperation processor = new DataValidationProcessorForCutOperation(CreateNotificationInfo(), walker);
			return ProcessSplittedParts(processor);
		}
		DataValidationNotificationInfo CreateNotificationInfo() {
			return new DataValidationNotificationInfo(cellRange, Expression1, Expression2);
		}
		List<DataValidation> ProcessSplittedParts(DataValidationSplitterBase procesor) {
			List<DataValidation> result = new List<DataValidation>();
			List<DataValidationNotificationInfo> splittedParts = procesor.Process();
			if (splittedParts.Count == 1) {
				bool validationChanged = TrySetupFromNotificationInfo(splittedParts[0]);
				if (!validationChanged || !Collection.HasEqual(this))
					return result;
			}
			Collection.Remove(this);
			foreach (DataValidationNotificationInfo info in splittedParts)
				result.Add(CreateFromNotificationInfo(info));
			return result;
		}
		bool TrySetupFromNotificationInfo(DataValidationNotificationInfo info) {
			ParsedExpression newExpression1 = info.Expression1;
			ParsedExpression newExpression2 = info.Expression2;
			CellRangeBase newRange = info.Range;
			bool expression1Equals = Expression1.Equals(newExpression1);
			bool expression2Equals = Expression2.Equals(newExpression2);
			bool rangeEquals = CellRange.EqualsPosition(newRange) && CellRange.CellCount == newRange.CellCount;
			if (expression1Equals && expression2Equals && rangeEquals)
				return false;
			if (!expression1Equals)
				Expression1 = newExpression1;
			if (!expression2Equals)
				Expression2 = newExpression2;
			if (!rangeEquals)
				CellRange = newRange;
			return true;
		}
		DataValidation CreateFromNotificationInfo(DataValidationNotificationInfo info) {
			DataValidation result = new DataValidation(info.Range, Sheet);
			result.CopyFrom(this);
			result.Expression1 = info.Expression1;
			result.Expression2 = info.Expression2;
			return result;
		}
		#endregion
		internal void ProcessCopiedRange(CellRangeBase rangeToAdd, CellRangeBase rangeToClear) {
			CellRangeBase resultRange = CellRange.Clone();
			if (rangeToAdd != null)
				resultRange = resultRange.MergeWithRange(rangeToAdd);
			if (rangeToClear != null) {
				resultRange = resultRange.ExcludeRange(rangeToClear);
				if (resultRange == null) {
					Sheet.DataValidations.Remove(this);
					return;
				}
			}
			CellRange = PrepareRange(resultRange);
		}
		CellRangeBase PrepareRange(CellRangeBase range) {
			if (range.RangeType == CellRangeType.UnionRange) {
				CellRangeBase merged = range.MergeWithRange(range.GetFirstInnerCellRange().Clone());
				if (merged.RangeType != CellRangeType.UnionRange)
					range = merged;
			}
			return range;
		}
		#region CloneTo & CopyFrom
		public DataValidation CloneTo(CellRangeBase cellRange) {
			DataValidation result = new DataValidation(cellRange, (Worksheet)cellRange.Worksheet);
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(DataValidation dataValidation) {
			Guard.ArgumentNotNull(dataValidation, "dataValidation");
			DocumentModel.BeginUpdate();
			try {
				base.CopyFrom(dataValidation);
				Expression1 = dataValidation.Expression1.Clone();
				Expression2 = dataValidation.Expression2.Clone();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public bool EqualsWithoutRange(DataValidation other) {
			return Info.Equals(other.Info) && Expression1.Equals(other.Expression1) && Expression2.Equals(other.Expression2);
		}
		#endregion
	}
	#endregion
	#region DataValidationInfo
	public class DataValidationInfo : ICloneable<DataValidationInfo>, ISupportsCopyFrom<DataValidationInfo>, ISupportsSizeOf {
		#region Constants
		const uint DataValidationTypeMask = 0x00000007;		   
		const short DataValidationTypeShift = 0;
		const uint DataValidationErrorStyleMask = 0x00000018;	 
		const short DataValidationErrorStyleShift = 3;
		const uint DataValidationImeModeMask = 0x000001E0;		
		const short DataValidationImeModeShift = 5;
		const uint DataValidationOperatorMask = 0x00000E00;	   
		const short DataValidationOperatorShift = 9;
		const uint AllowBlankMask = 0x00001000;				   
		const uint SuppressDropDownMask = 0x00002000;				 
		const uint ShowInputMessageMask = 0x00004000;			 
		const uint ShowErrorMessageMask = 0x00008000;			 
		#endregion
		#region Fields
		uint packedValues;
		string errorTitle;
		string error;
		string promptTitle;
		string prompt;
		#endregion
		#region Properties
		public string ErrorTitle { get { return errorTitle == null ? String.Empty : errorTitle; } set { errorTitle = value; } }
		public string Error { get { return error == null ? String.Empty : error; } set { error = value; } }
		public string PromptTitle { get { return promptTitle == null ? String.Empty : promptTitle; } set { promptTitle = value; } }
		public string Prompt { get { return prompt == null ? String.Empty : prompt; } set { prompt = value; } }
		public bool AllowBlank { get { return GetBooleanVal(AllowBlankMask); } set { SetBooleanVal(AllowBlankMask, value); } }
		public bool SuppressDropDown { get { return GetBooleanVal(SuppressDropDownMask); } set { SetBooleanVal(SuppressDropDownMask, value); } }
		public bool ShowInputMessage { get { return GetBooleanVal(ShowInputMessageMask); } set { SetBooleanVal(ShowInputMessageMask, value); } }
		public bool ShowErrorMessage { get { return GetBooleanVal(ShowErrorMessageMask); } set { SetBooleanVal(ShowErrorMessageMask, value); } }
		public DataValidationType Type { get { return (DataValidationType)GetUInt(DataValidationTypeMask, DataValidationTypeShift); } set { SetUInt((uint)value, DataValidationTypeMask, DataValidationTypeShift); } }
		public DataValidationErrorStyle ErrorStyle { get { return (DataValidationErrorStyle)GetUInt(DataValidationErrorStyleMask, DataValidationErrorStyleShift); } set { SetUInt((uint)value, DataValidationErrorStyleMask, DataValidationErrorStyleShift); } }
		public DataValidationImeMode ImeMode { get { return (DataValidationImeMode)GetUInt(DataValidationImeModeMask, DataValidationImeModeShift); } set { SetUInt((uint)value, DataValidationImeModeMask, DataValidationImeModeShift); } }
		public DataValidationOperator ValidationOperator { get { return (DataValidationOperator)GetUInt(DataValidationOperatorMask, DataValidationOperatorShift); } set { SetUInt((uint)value, DataValidationOperatorMask, DataValidationOperatorShift); } }
		#region GetVal/SetVal helpers
		void SetBooleanVal(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanVal(uint mask) {
			return (packedValues & mask) != 0;
		}
		uint GetUInt(uint mask, int shift) {
			return (packedValues & mask) >> shift;
		}
		void SetUInt(uint value, uint mask, int shift) {
			packedValues &= ~mask;
			packedValues |= (value << shift) & mask;
		}
		#endregion
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		#region ICloneable<WorksheetProtectionInfo> Members
		public DataValidationInfo Clone() {
			DataValidationInfo result = new DataValidationInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<WorksheetProtectionInfo> Members
		public void CopyFrom(DataValidationInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
			this.errorTitle = value.errorTitle;
			this.error = value.error;
			this.promptTitle = value.promptTitle;
			this.prompt = value.prompt;
		}
		#endregion
		public override bool Equals(object obj) {
			DataValidationInfo info = obj as DataValidationInfo;
			if (info == null)
				return false;
			return this.packedValues == info.packedValues &&
						errorTitle == info.errorTitle &&
						error == info.error &&
						promptTitle == info.promptTitle &&
						prompt == info.prompt; 
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32((int)packedValues, errorTitle.GetHashCode(), error.GetHashCode(), promptTitle.GetHashCode(), prompt.GetHashCode());
		}
	}
	#endregion
	#region DataValidationInfoCache
	public class DataValidationInfoCache : UniqueItemsCache<DataValidationInfo> {
		public DataValidationInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override DataValidationInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			DataValidationInfo item = new DataValidationInfo();
			item.AllowBlank = true;
			item.ShowInputMessage = true;
			item.ShowErrorMessage = true;
			item.SuppressDropDown = true;
			return item;
		}
	}
	#endregion
	#region Validators
	#region DataValidator(abstract class)
	abstract class DataValidator {
		#region Fields
		protected DocumentModel workbook;
		protected ParsedExpression calculatedExpression;
		protected ParsedThingVariantValue constantExpression;
		protected ParsedExpression expression1;
		protected ParsedExpression expression2;
		#endregion
		protected DataValidator(DocumentModel workbook) {
			this.workbook = workbook;
		}
		#region Properties
		protected WorkbookDataContext DataContext { get { return workbook.DataContext; } }
		protected bool ForceValidate { get; set; }
		#endregion
		public void SetExpression1(ParsedExpression expression) {
			this.expression1 = expression;
			calculatedExpression = null;
		}
		public void SetExpression2(ParsedExpression expression) {
			this.expression2 = expression;
			calculatedExpression = null;
		}
		internal protected bool Validate(Worksheet sheet, VariantValue value, CellPosition activeCellPosition, DataValidationOperator validationOperator, bool allowBlank) {
			if (activeCellPosition.IsValid)
				DataContext.PushCurrentCell(activeCellPosition);
			else
				DataContext.PushCurrentCell(null);
			DataContext.PushCurrentWorksheet(sheet);
			try {
				if (value.IsCellRange)
					value = DataContext.DereferenceValue(value, true);
				DataContext.PushRelativeToCurrentCell(true);
				if (!CheckTypeRestrictions(value))
					return false;
				ProcessDefinedNames(allowBlank);
				if (ForceValidate)
					return true;
				return ValidateCore(value, validationOperator);
			}
			finally {
				DataContext.PopCurrentCell();
				DataContext.PopRelativeToCurrentCell();
				DataContext.PopCurrentWorksheet();
			}
		}
		internal protected virtual bool ValidateCore(VariantValue value, DataValidationOperator validationOperator) {
			if (calculatedExpression == null)
				calculatedExpression = GetExpression(value, validationOperator);
			if (constantExpression != null)
				constantExpression.Value = value;
			VariantValue result = calculatedExpression.Evaluate(DataContext);
			if (!result.IsBoolean)
				return false;
			return result.BooleanValue;
		}
		protected virtual void ProcessDefinedNames(bool allowBlank) {
			VariantValue definedNameValue1 = TryGetValueFromNameExpression(expression1);
			if (!definedNameValue1.IsError) {
				CellRangeBase range = definedNameValue1.CellRangeValue;
				VariantValue cellValue = GetTopLeftCellValue(range);
				CellRangeType rangeType = range.RangeType;
				ForceValidate = rangeType == CellRangeType.SingleRange ? allowBlank && cellValue.IsEmpty : allowBlank;
				if (ForceValidate || rangeType == CellRangeType.UnionRange)
					return;
				expression1 = new ParsedExpression();
				expression1.Add(ValueParsedThing.CreateInstance(cellValue.NumericValue, DataContext));
			}
			VariantValue definedNameValue2 = TryGetValueFromNameExpression(expression2);
			if (!definedNameValue2.IsError) {
				CellRangeBase range = definedNameValue2.CellRangeValue;
				VariantValue cellValue = GetTopLeftCellValue(range);
				CellRangeType rangeType = range.RangeType;
				ForceValidate = rangeType == CellRangeType.SingleRange ? allowBlank && cellValue.IsEmpty : allowBlank;
				if (ForceValidate || rangeType == CellRangeType.UnionRange)
					return;
				expression2 = new ParsedExpression();
				expression2.Add(ValueParsedThing.CreateInstance(cellValue.NumericValue, DataContext));
			}
		}
		protected VariantValue GetTopLeftCellValue(CellRangeBase range) {
			ICellBase topLeftCell = range.Worksheet.TryGetCell(range.TopLeft.Column, range.TopLeft.Row);
			return topLeftCell == null ? VariantValue.Empty : topLeftCell.Value;
		}
		protected VariantValue TryGetValueFromNameExpression(ParsedExpression expression) {
			if (expression.Count != 1)
				return VariantValue.ErrorValueNotAvailable;
			ParsedThingName parsedThingName = expression[0] as ParsedThingName;
			if (parsedThingName == null)
				return VariantValue.ErrorValueNotAvailable;
			parsedThingName.DataType = OperandDataType.Default;
			VariantValue definedNameValue = parsedThingName.EvaluateToValue(DataContext);
			if (definedNameValue.Type != VariantValueType.CellRange)
				return VariantValue.ErrorValueNotAvailable;
			return definedNameValue;
		}
		protected abstract bool CheckTypeRestrictions(VariantValue value);
		void AddBinaryParsedThing(ParsedExpression expression, ParsedThingBase leftExpression, ParsedExpression rightExpression, BinaryParsedThing operatorThing) {
			expression.Add(leftExpression);
			expression.AddRange(rightExpression);
			expression.Add(operatorThing);
		}
		protected virtual ParsedExpression GetExpression(VariantValue value, DataValidationOperator validationOperator) {
			ParsedExpression resultExpression = new ParsedExpression();
			constantExpression = new ParsedThingVariantValue(value);
			switch (validationOperator) {
				case DataValidationOperator.Equal:
					AddBinaryParsedThing(resultExpression, constantExpression, expression1, ParsedThingEqual.Instance);
					break;
				case DataValidationOperator.NotEqual:
					AddBinaryParsedThing(resultExpression, constantExpression, expression1, ParsedThingNotEqual.Instance);
					break;
				case DataValidationOperator.GreaterThan:
					AddBinaryParsedThing(resultExpression, constantExpression, expression1, ParsedThingGreater.Instance);
					break;
				case DataValidationOperator.GreaterThanOrEqual:
					AddBinaryParsedThing(resultExpression, constantExpression, expression1, ParsedThingGreaterEqual.Instance);
					break;
				case DataValidationOperator.LessThan:
					AddBinaryParsedThing(resultExpression, constantExpression, expression1, ParsedThingLess.Instance);
					break;
				case DataValidationOperator.LessThanOrEqual:
					AddBinaryParsedThing(resultExpression, constantExpression, expression1, ParsedThingLessEqual.Instance);
					break;
				case DataValidationOperator.Between:
					AddBinaryParsedThing(resultExpression, constantExpression, expression1, ParsedThingGreaterEqual.Instance);
					AddBinaryParsedThing(resultExpression, constantExpression, expression2, ParsedThingLessEqual.Instance);
					resultExpression.Add(new ParsedThingFuncVar(FormulaCalculator.FuncAndCode, 2));
					break;
				case DataValidationOperator.NotBetween:
					AddBinaryParsedThing(resultExpression, constantExpression, expression1, ParsedThingLess.Instance);
					AddBinaryParsedThing(resultExpression, constantExpression, expression2, ParsedThingGreater.Instance);
					resultExpression.Add(new ParsedThingFuncVar(FormulaCalculator.FuncOrCode, 2));
					break;
				default: throw new ArgumentException("Unknown DataValidationOperator:" + validationOperator);
			}
			return resultExpression;
		}
		public virtual void CheckIntegrity(DataValidationOperator validationOperator) {
			switch (validationOperator) {
				case DataValidationOperator.Between:
				case DataValidationOperator.NotBetween:
					if (expression2 == null)
						IntegrityChecks.Fail("Operator " + validationOperator + " requires second formula."); break;
			}
			if (expression1 == null)
				IntegrityChecks.Fail("Operator " + validationOperator + " requires first formula.");
		}
	}
	#endregion
	#region DataValidatorWhole
	class DataValidatorWhole : DataValidator {
		public DataValidatorWhole(DocumentModel workbook)
			: base(workbook) {
		}
		protected override bool CheckTypeRestrictions(VariantValue value) {
			return !value.IsError && !value.IsText && value.NumericValue == Math.Truncate(value.NumericValue);
		}
	}
	#endregion
	#region DataValidatorTextLength
	class DataValidatorTextLength : DataValidator {
		public DataValidatorTextLength(DocumentModel workbook)
			: base(workbook) {
		}
		internal protected override bool ValidateCore(VariantValue value, DataValidationOperator validationOperator) {
			if (calculatedExpression == null)
				calculatedExpression = GetExpression(value, validationOperator);
			if (constantExpression != null)
				constantExpression.Value = value.ToText(DataContext).InlineTextValue.Length;
			VariantValue result = calculatedExpression.Evaluate(DataContext);
			if (!result.IsBoolean)
				return false;
			return result.BooleanValue;
		}
		protected override bool CheckTypeRestrictions(VariantValue value) {
			return !value.IsError;
		}
	}
	#endregion
	#region DataValidatorCustom
	class DataValidatorCustom : DataValidator {
		public DataValidatorCustom(DocumentModel workbook)
			: base(workbook) {
		}
		internal protected override bool ValidateCore(VariantValue value, DataValidationOperator validationOperator) {
			VariantValue result = expression1.Evaluate(DataContext);
			return result.ToBoolean(DataContext).BooleanValue;
		}
		protected override bool CheckTypeRestrictions(VariantValue value) {
			return true;
		}
		public override void CheckIntegrity(DataValidationOperator validationOperator) {
			if (expression1 == null)
				IntegrityChecks.Fail("Operator " + validationOperator + " needs first formula.");
		}
		protected override void ProcessDefinedNames(bool allowBlank) {
			ForceValidate = allowBlank && !TryGetValueFromNameExpression(expression1).IsError;
		}
	}
	#endregion
	#region DataValidatorDate
	class DataValidatorDate : DataValidator {
		public DataValidatorDate(DocumentModel workbook)
			: base(workbook) {
		}
		protected override bool CheckTypeRestrictions(VariantValue value) {
			return !value.IsError && !value.IsText && !value.IsBoolean;
		}
	}
	#endregion
	#region DataValidatorTime
	class DataValidatorTime : DataValidator {
		public DataValidatorTime(DocumentModel workbook)
			: base(workbook) {
		}
		protected override bool CheckTypeRestrictions(VariantValue value) {
			return !value.IsError && !value.IsText;
		}
	}
	#endregion
	#region DataValidatorDecimal
	class DataValidatorDecimal : DataValidator {
		public DataValidatorDecimal(DocumentModel workbook)
			: base(workbook) {
		}
		protected override bool CheckTypeRestrictions(VariantValue value) {
			return !value.IsError && !value.IsText;
		}
	}
	#endregion
	#region DataValidatorList
	class DataValidatorList : DataValidator {
		public DataValidatorList(DocumentModel workbook)
			: base(workbook) {
		}
		protected internal override bool ValidateCore(VariantValue value, DataValidationOperator validationOperator) {
			VariantValue allowedValuesExpression = expression1.Evaluate(DataContext);
			if (allowedValuesExpression.IsText) {
				string textValue = value.IsError ? CellErrorFactory.GetErrorName(value.ErrorValue, DataContext) : value.ToText(DataContext).InlineTextValue.Trim();
				string textAllowedValues = allowedValuesExpression.GetTextValue(workbook.SharedStringTable);
				char separator = DataContext.GetListSeparator();
				string[] textAllowedValuesArray = textAllowedValues.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < textAllowedValuesArray.Length; i++)
					textAllowedValuesArray[i] = textAllowedValuesArray[i].Trim();
				for (int i = 0; i < textAllowedValuesArray.Length; i++) {
					if (string.Compare(textAllowedValuesArray[i], textValue) == 0)
						return true;
				}
				return false;
			}
			else if (allowedValuesExpression.IsCellRange) {
				CellRangeBase cellRange = allowedValuesExpression.CellRangeValue;
				foreach (ICellBase cell in cellRange.GetExistingCellsEnumerable()) {
					if (cell.Value.IsEqual(value, StringComparison.CurrentCultureIgnoreCase, workbook.SharedStringTable))
						return true;
				}
				return false;
			}
			return false;
		}
		protected override void ProcessDefinedNames(bool allowBlank) {
			VariantValue definedNameValue = TryGetValueFromNameExpression(expression1);
			if (definedNameValue.IsError)
				return;
			CellRangeBase range = definedNameValue.CellRangeValue;
			CellRangeType rangeType = range.RangeType;
			ForceValidate = allowBlank && (rangeType == CellRangeType.IntervalRange || 
						   (rangeType == CellRangeType.SingleRange && GetTopLeftCellValue(range) == VariantValue.Empty));
		}
		protected override bool CheckTypeRestrictions(VariantValue value) {
			return true;
		}
		public override void CheckIntegrity(DataValidationOperator validationOperator) {
			if (expression1 == null)
				IntegrityChecks.Fail("Operator " + validationOperator + " needs first formula.");
		}
	}
	#endregion
	#endregion
	#region DataValidationOperator(enum)
	public enum DataValidationOperator {
		Between = 0,
		NotBetween = 1,
		Equal = 2,
		NotEqual = 3,
		LessThan = 5,
		LessThanOrEqual = 7,
		GreaterThan = 4,
		GreaterThanOrEqual = 6
	}
	#endregion
	#region DataValidationImeMode(enum)
	public enum DataValidationImeMode {
		NoControl = 0,
		On = 1,
		Off = 2,
		Disabled = 3,
		Hiragana = 4,
		FullKatakana = 5,
		HalfKatakana = 6,
		FullAlpha = 7,
		HalfAlpha = 8,
		FullHangul = 9,
		HalfHangul = 10,
	}
	#endregion
	#region DataValidationErrorStyle(enum)
	public enum DataValidationErrorStyle {
		Stop = 0,
		Warning = 1,
		Information = 2,
	}
	#endregion
	#region DataValidationType(enum)
	public enum DataValidationType {
		None = 0,
		Whole = 1,
		Decimal = 2,
		List = 3,
		Date = 4,
		Time = 5,
		TextLength = 6,
		Custom = 7,
	}
	#endregion
}
