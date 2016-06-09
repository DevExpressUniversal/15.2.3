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

using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Export.Xl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ConditionalFormattingRuleType
	public enum ConditionalFormattingRuleType {
		AboveOrBelowAverage,	  
		BeginsWithText,		   
		CompareWithFormulaResult, 
		ColorScale,			   
		CellIsBlank,			  
		ContainsErrors,		   
		ContainsText,			 
		DataBar,				  
		DuplicateValues,		  
		EndsWithText,			 
		ExpressionIsTrue,		 
		IconSet,				  
		CellIsNotBlank,		   
		NotContainsErrors,		
		NotContainsText,		  
		InsideDatePeriod,		 
		TopOrBottomValue,		 
		UniqueValue,			  
		Unknown = -1
	}
	#endregion
	#region ConditionalFormattingOperator
	public enum ConditionalFormattingOperator {
		BeginsWith,
		Between,
		ContainsText,
		EndsWith,
		Equal,
		GreaterThan,
		GreaterThanOrEqual,
		LessThan,
		LessThanOrEqual,
		NotBetween,
		NotContains,
		NotEqual,
		Unknown = -1
	}
	#endregion
	#region ConditionalFormattingCreatorData
	public abstract class ConditionalFormattingCreatorData {
		Worksheet sheet;
		List<ConditionalFormattingValueObject> valueObjects;
		protected ConditionalFormattingCreatorData() {
		}
		protected ConditionalFormattingCreatorData(Worksheet worksheet, CellRangeBase cellRange, bool isPivot) {
			sheet = worksheet;
			CellRange = cellRange;
			IsPivot = isPivot;
			ActivePresent = null;
			StopIfTrue = false;
			ShowValue = false;
			Priority = Int32.MaxValue;
		}
		#region Properties
		public Worksheet Sheet { get { return sheet; } set { sheet = value; } }
		public bool IsPivot { get; set; }
		public CellRangeBase CellRange { get; set; }
		public virtual ConditionalFormattingType Type { get { return ConditionalFormattingType.Unknown; } }
		public bool ShowValue { get; set; }
		public bool IsActivePresent { get; set; }
		public string ActivePresent { get; set; }
		public bool StopIfTrue { get; set; }
		public int Priority { get; set; }
		public int DxfId { get; set; }
		public ConditionalFormattingRuleType RuleType { get; set; }
		public List<ConditionalFormattingValueObject> ValueObjects { get { return valueObjects; } }
		public string ExtId { get; set; }
		#endregion
		protected void InitializeValueObjects() {
			valueObjects = new List<ConditionalFormattingValueObject>();
		}
		static int OpenXmlImportComparer(ConditionalFormattingCreatorData a, ConditionalFormattingCreatorData b) {
			if (a.Priority < b.Priority)
				return -1;
			if (a.Priority == b.Priority)
				return 0;
			return 1;
		}
		public static void ApplyCollection(List<ConditionalFormattingCreatorData> collection) {
			collection.Sort(OpenXmlImportComparer);
			for (int i = 0; i < collection.Count;) {
				ConditionalFormattingCreatorData item = collection[i];
				item.Priority = ++i;
				ConditionalFormatting newCondFmt = item.CreateConditionalFormatting();
				if (newCondFmt != null)
					item.Sheet.ConditionalFormattings.Add(newCondFmt);
			}
		}
		public abstract ConditionalFormatting CreateConditionalFormatting();
	}
	public class ConditionalFormattingColorScaleCreatorData : ConditionalFormattingCreatorData {
		#region Fields
		List<ColorModelInfo> color;
		#endregion
		public ConditionalFormattingColorScaleCreatorData(Worksheet worksheet)
			: this(worksheet, null, false) {
		}
		public ConditionalFormattingColorScaleCreatorData(Worksheet worksheet, CellRangeBase cellRange, bool isPivot)
			: base(worksheet, cellRange, isPivot) {
			InitializeValueObjects();
			color = new List<ColorModelInfo>();
		}
		public override ConditionalFormattingType Type { get { return ConditionalFormattingType.ColorScale; } }
		public List<ColorModelInfo> Colors { get { return color; } }
		public override ConditionalFormatting CreateConditionalFormatting() {
			ColorScaleConditionalFormatting result = new ColorScaleConditionalFormatting(Sheet, CellRange, ValueObjects.ToArray(), Colors.ToArray());
			result.BeginInit();
			try {
				result.IsPivot = IsPivot;
				result.SetPriority(Priority);
				result.StopIfTrue = StopIfTrue;
			}
			finally {
				result.EndInit();
			}
			return result;
		}
	}
	public class ConditionalFormattingDataBarCreatorData : ConditionalFormattingCreatorData {
		#region Fields
		List<ColorModelInfo> color;
		#endregion
		public ConditionalFormattingDataBarCreatorData(Worksheet worksheet)
			: this(worksheet, null, false) {
		}
		public ConditionalFormattingDataBarCreatorData(Worksheet worksheet, CellRangeBase cellRange, bool isPivot)
			: base(worksheet, cellRange, isPivot) {
			InitializeValueObjects();
			color = new List<ColorModelInfo>();
		}
		public override ConditionalFormattingType Type { get { return ConditionalFormattingType.DataBar; } }
		public int MinLength { get; set; }
		public int MaxLength { get; set; }
		public bool HaveBorder { get; set; }
		public ColorModelInfo BorderColor { get; set; }
		public ColorModelInfo AxisColor { get; set; }
		public ColorModelInfo NegativeBorderColor { get; set; }
		public ColorModelInfo NegativeFillColor { get; set; }
		public bool GradientFill { get; set; }
		public ConditionalFormattingDataBarDirection Direction { get; set; }
		public ConditionalFormattingDataBarAxisPosition AxisPosition { get; set; }
		public bool NegativeUseSameColor { get; set; }
		public bool NegativeUseSameBorderColor { get; set; }
		public List<ColorModelInfo> Colors { get { return color; } }
		public override ConditionalFormatting CreateConditionalFormatting() {
			DataBarConditionalFormatting result = new DataBarConditionalFormatting(Sheet, CellRange, ValueObjects[0], ValueObjects[1], Colors[0]);
			result.BeginInit();
			try {
				result.AxisPosition = AxisPosition;
				result.Direction = Direction;
				result.GradientFill = GradientFill;
				result.ShowValue = ShowValue;
				result.MinLength = MinLength;
				result.MaxLength = MaxLength;
				result.SetPriority(Priority);
			}
			finally {
				result.EndInit();
			}
			result.NegativeValueColorIndex = NegativeUseSameColor ? result.ColorIndex : ColorModelInfo.GetColorIndex(Sheet.Workbook.Cache.ColorModelInfoCache, NegativeFillColor);
			if (AxisPosition != ConditionalFormattingDataBarAxisPosition.None)
				result.AxisColorIndex = ColorModelInfo.GetColorIndex(Sheet.Workbook.Cache.ColorModelInfoCache, AxisColor);
			if (HaveBorder) {
				int colorIndex = ColorModelInfo.GetColorIndex(Sheet.Workbook.Cache.ColorModelInfoCache, BorderColor);
				result.BorderColorIndex = colorIndex;
				result.NegativeValueBorderColorIndex = NegativeUseSameBorderColor ? colorIndex : ColorModelInfo.GetColorIndex(Sheet.Workbook.Cache.ColorModelInfoCache, NegativeBorderColor);
			}
			return result;
		}
	}
	public class ConditionalFormattingFormulaCreatorData : ConditionalFormattingCreatorData {
		List<string> formulas;
		public ConditionalFormattingFormulaCreatorData() {
			formulas = new List<string>();
		}
		public ConditionalFormattingFormulaCreatorData(Worksheet worksheet)
			: this(worksheet, null, false) {
		}
		public ConditionalFormattingFormulaCreatorData(Worksheet worksheet, CellRangeBase cellRange, bool isPivot)
			: base(worksheet, cellRange, isPivot) {
			formulas = new List<string>();
		}
		#region Properties
		public override ConditionalFormattingType Type { get { return ConditionalFormattingType.Formula; } }
		public bool AboveAverage { get; set; }
		public bool Bottom { get; set; }
		public bool EqualAverage { get; set; }
		public ConditionalFormattingOperator CfOperator { get; set; }
		public bool Percent { get; set; }
		public int Rank { get; set; }
		public int StdDev { get; set; }
		public string Text { get; set; }
		public ConditionalFormattingTimePeriod TimePeriod { get; set; }
		public List<string> Formulas { get { return formulas; } }
		#endregion
		public void ApplyFormulas() {
			string text = string.Empty;
			if (Formulas.Count >= 2 &&
				(RuleType == ConditionalFormattingRuleType.ContainsText || RuleType == ConditionalFormattingRuleType.BeginsWithText ||
				RuleType == ConditionalFormattingRuleType.EndsWithText || RuleType == ConditionalFormattingRuleType.NotContainsText))
				text = Formulas[1];
			else if (Formulas.Count >= 1 && RuleType == ConditionalFormattingRuleType.ExpressionIsTrue)
				text = Formulas[0];
			if (!string.IsNullOrEmpty(text)) {
				if (text[0] == '=')
					this.Text = text;
				else
					this.Text = '=' + text;
			}
		}
		public override ConditionalFormatting CreateConditionalFormatting() {
			ConditionalFormatting result = CreateFromRuleType();
			if (result != null) {
				result.IsPivot = IsPivot;
				result.StopIfTrue = StopIfTrue;
				result.SetPriority(Priority);
				if (result.SupportsDifferentialFormat)
					result.DifferentialFormatIndex = DxfId;
			}
			return result;
		}
		FormulaConditionalFormatting CreateFromRuleType() {
			switch (RuleType) {
				case ConditionalFormattingRuleType.AboveOrBelowAverage:
					return new AverageFormulaConditionalFormatting(Sheet, CellRange, ConvertToAverageCondition(AboveAverage, EqualAverage), StdDev);
				case ConditionalFormattingRuleType.BeginsWithText:
					return new TextFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingTextCondition.BeginsWith, Text);
				case ConditionalFormattingRuleType.ContainsText:
					return new TextFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingTextCondition.Contains, Text);
				case ConditionalFormattingRuleType.EndsWithText:
					return new TextFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingTextCondition.EndsWith, Text);
				case ConditionalFormattingRuleType.NotContainsText:
					return new TextFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingTextCondition.NotContains, Text);
				case ConditionalFormattingRuleType.CellIsBlank:
					return new SpecialFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingSpecialCondition.ContainBlanks);
				case ConditionalFormattingRuleType.CellIsNotBlank:
					return new SpecialFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingSpecialCondition.ContainNonBlanks);
				case ConditionalFormattingRuleType.CompareWithFormulaResult:
					return CreateFromOperator();
				case ConditionalFormattingRuleType.ContainsErrors:
					return new SpecialFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingSpecialCondition.ContainError);
				case ConditionalFormattingRuleType.DuplicateValues:
					return new SpecialFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingSpecialCondition.ContainDuplicateValue);
				case ConditionalFormattingRuleType.ExpressionIsTrue:
					if (Formulas.Count > 0)
						return new ExpressionFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingExpressionCondition.ExpressionIsTrue, CorrectFormula(Formulas[0]));
					break;
				case ConditionalFormattingRuleType.InsideDatePeriod:
					return new TimePeriodFormulaConditionalFormatting(Sheet, CellRange, TimePeriod);
				case ConditionalFormattingRuleType.NotContainsErrors:
					return new SpecialFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingSpecialCondition.NotContainError);
				case ConditionalFormattingRuleType.TopOrBottomValue:
					return new RankFormulaConditionalFormatting(Sheet, CellRange, ConvertToRankCondition(!Bottom, Percent), Rank);
				case ConditionalFormattingRuleType.UniqueValue:
					return new SpecialFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingSpecialCondition.ContainUniqueValue);
			}
			return null;
		}
		FormulaConditionalFormatting CreateFromOperator() {
			int argumentsCount = Formulas.Count;
			if (argumentsCount < 1) {
				Exceptions.ThrowInvalidOperationException("Too few arguments");
				return null;
			}
			string firstArgument = CorrectFormula(Formulas[0]);
			switch (CfOperator) {
				case ConditionalFormattingOperator.BeginsWith:
					return new TextFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingTextCondition.BeginsWith, firstArgument);
				case ConditionalFormattingOperator.Between:
					if (argumentsCount < 2)
						Exceptions.ThrowInvalidOperationException("Too few arguments");
					else
						return new RangeFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingRangeCondition.Inside, firstArgument, CorrectFormula(Formulas[1]));
					break;
				case ConditionalFormattingOperator.ContainsText:
					return new TextFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingTextCondition.Contains, firstArgument);
				case ConditionalFormattingOperator.EndsWith:
					return new TextFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingTextCondition.EndsWith, firstArgument);
				case ConditionalFormattingOperator.Equal:
					return new ExpressionFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingExpressionCondition.EqualTo, firstArgument);
				case ConditionalFormattingOperator.GreaterThan:
					return new ExpressionFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingExpressionCondition.GreaterThan, firstArgument);
				case ConditionalFormattingOperator.GreaterThanOrEqual:
					return new ExpressionFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingExpressionCondition.GreaterThanOrEqual, firstArgument);
				case ConditionalFormattingOperator.LessThan:
					return new ExpressionFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingExpressionCondition.LessThan, firstArgument);
				case ConditionalFormattingOperator.LessThanOrEqual:
					return new ExpressionFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingExpressionCondition.LessThanOrEqual, firstArgument);
				case ConditionalFormattingOperator.NotBetween:
					if (argumentsCount < 2)
						Exceptions.ThrowInvalidOperationException("Too few arguments");
					else
						return new RangeFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingRangeCondition.Outside, firstArgument, CorrectFormula(Formulas[1]));
					break;
				case ConditionalFormattingOperator.NotContains:
					return new TextFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingTextCondition.NotContains, firstArgument);
				case ConditionalFormattingOperator.NotEqual:
					return new ExpressionFormulaConditionalFormatting(Sheet, CellRange, ConditionalFormattingExpressionCondition.InequalTo, firstArgument);
			}
			Exceptions.ThrowInvalidOperationException("Invalid value of conditional formatting operator");
			return null;
		}
		string CorrectFormula(string formula) {
			if (!string.IsNullOrEmpty(formula) && formula[0] != '=')
				return "=" + formula;
			return formula;
		}
		internal static ConditionalFormattingAverageCondition ConvertToAverageCondition(bool isAbove, bool isEqual) {
			if (isAbove)
				return isEqual ? ConditionalFormattingAverageCondition.AboveOrEqual : ConditionalFormattingAverageCondition.Above;
			return isEqual ? ConditionalFormattingAverageCondition.BelowOrEqual : ConditionalFormattingAverageCondition.Below;
		}
		internal static ConditionalFormattingRankCondition ConvertToRankCondition(bool isTop, bool isPercent) {
			if (isTop)
				return isPercent ? ConditionalFormattingRankCondition.TopByPercent : ConditionalFormattingRankCondition.TopByRank;
			return isPercent ? ConditionalFormattingRankCondition.BottomByPercent : ConditionalFormattingRankCondition.BottomByRank;
		}
	}
	public class ConditionalFormattingIconSetCreatorData : ConditionalFormattingCreatorData {
		List<ConditionalFormattingCustomIcon> customIcons;
		public ConditionalFormattingIconSetCreatorData(Worksheet worksheet)
			: this(worksheet, null, false) {
		}
		public ConditionalFormattingIconSetCreatorData(Worksheet worksheet, CellRangeBase cellRange, bool isPivot)
			: base(worksheet, cellRange, isPivot) {
			customIcons = new List<ConditionalFormattingCustomIcon>();
			InitializeValueObjects();
		}
		public override ConditionalFormattingType Type { get { return ConditionalFormattingType.IconSet; } }
		public bool CustomIconSet { get; set; } 
		public IconSetType IconSet { get; set; }
		public bool Percent { get; set; }
		public List<ConditionalFormattingCustomIcon> CustomIcons { get { return customIcons; } }
		public bool Reverse { get; set; }
		public override ConditionalFormatting CreateConditionalFormatting() {
			IconSetConditionalFormatting result = new IconSetConditionalFormatting(Sheet, CellRange, IconSet, ValueObjects.ToArray());
			result.BeginInit();
			try {
				result.IsPivot = IsPivot;
				result.SetPriority(Priority);
				result.StopIfTrue = StopIfTrue;
				result.Reversed = Reverse;
				result.ShowValue = ShowValue;
				if (CustomIconSet) {
					result.IsCustom = true;
					result.SetCustomIcons(CustomIcons.ToArray());
				}
			}
			finally {
				result.EndInit();
			}
			return result;
		}
	}
	#endregion
	#region SpreadsheetLightIndexBasedObject
	public abstract class SpreadsheetLightIndexBasedObject<T> where T : ICloneable<T>, ISupportsCopyFrom<T>, ISupportsSizeOf {
		protected internal delegate DocumentModelChangeActions SetPropertyValueDelegate<U>(T info, U newValue);
		int index;
		Worksheet worksheet;
		protected internal int Index { get { return index; } set { index = value; } }
		public Worksheet Worksheet { get { return worksheet; } protected set { worksheet = value; } }
		public DocumentModel DocumentModel { get { return Worksheet.Workbook; } }
		protected SpreadsheetLightIndexBasedObject(Worksheet worksheet, int index) {
			Worksheet = worksheet;
			Index = index;
		}
		protected SpreadsheetLightIndexBasedObject(Worksheet worksheet)
			: this(worksheet, 0) {
		}
		protected internal virtual UniqueItemsCache<T> InfoCache { get { return GetCache(DocumentModel); } }
		protected internal T Info { get { return InfoCache[Index]; } }
		protected void BeginUpdate() {
		}
		protected void EndUpdate() {
		}
		protected void CancelUpdate() {
		}
		protected int ApplyInfo(T info) {
			return InfoCache.AddItem(info);
		}
		protected abstract T GetInfoForModification();
		protected void ReplaceInfo(T newInfo) {
			int newIndex = ApplyInfo(newInfo);
			if (newIndex != Index) {
				int oldIndex = Index;
				Index = newIndex;
				OnIndexChanged(oldIndex);
			}
		}
		protected void SetPropertyValue<U>(SetPropertyValueDelegate<U> setter, U value) {
			T newInfo = GetInfoForModification();
			setter(newInfo, value);
			ReplaceInfo(newInfo);
		}
		protected internal virtual void OnIndexChanged(int oldIndex) { }
		protected void SetIndexInitial(int startIndex) {
			index = startIndex;
		}
		protected internal abstract UniqueItemsCache<T> GetCache(IDocumentModel documentModel);
		protected internal abstract void ApplyChanges(DocumentModelChangeActions changeActions);
		protected internal virtual DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
	}
	#endregion
	#region ConditionalFormattingValueObjectType
	public enum ConditionalFormattingValueObjectType {
		Unknown = 0,
		Formula,
		Max,
		Min,
		Num,
		Percent,
		Percentile,
		AutoMin,
		AutoMax
	}
	#endregion
	#region ConditionalFormattingValueInfo
	public class ConditionalFormattingValueInfo : ICloneable<ConditionalFormattingValueInfo>,
												  ISupportsCopyFrom<ConditionalFormattingValueInfo>,
												  ISupportsSizeOf {
		#region Fields
		#region packed data masks
		const uint packedEmpty = 0;
		const uint packedValueTypeMask = 15;   
		const uint packedIsGteMask = 16;	   
		#endregion
		uint packedData;
		byte[] valueFormula = null;
		#endregion
		#region Properties
		public ConditionalFormattingValueObjectType ValueType { get { return GetValueType(); } set { SetValueType(value); } }
		public bool IsGreaterOrEqual { get { return GetBooleanValue(packedIsGteMask); } set { SetBooleanValue(packedIsGteMask, value); } }
		internal uint PackedData { get { return packedData; } set { packedData = value; } }
		public byte[] Value { get { return valueFormula; } set { valueFormula = (value != null) ? (byte[])value.Clone() : null; } }
		#endregion
		#region Packed values helpers
		bool GetBooleanValue(uint mask) {
			return (packedData & mask) != 0;
		}
		void SetBooleanValue(uint mask, bool value) {
			if (value)
				packedData |= mask;
			else
				packedData &= ~mask;
		}
		uint GetUIntValue(uint shifter, uint mask) {
			return (uint)((int)packedData >> (int)shifter) & mask;
		}
		void SetUIntValue(uint shifter, uint mask, uint value) {
			packedData &= ~(uint)((int)mask << (int)shifter);
			packedData |= (uint)((int)(value & mask) << (int)shifter);
		}
		#endregion
		#region property helpers
		ConditionalFormattingValueObjectType GetValueType() {
			return (ConditionalFormattingValueObjectType)GetUIntValue(0, packedValueTypeMask);
		}
		void SetValueType(ConditionalFormattingValueObjectType value) {
			SetUIntValue(0, packedValueTypeMask, (uint)value);
		}
		#endregion
		#region ICloneable<ConditionalFormattingValueInfo> Members
		public ConditionalFormattingValueInfo Clone() {
			ConditionalFormattingValueInfo result = new ConditionalFormattingValueInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ConditionalFormattingValueInfo> Members
		public void CopyFrom(ConditionalFormattingValueInfo value) {
			Guard.ArgumentNotNull(value, "value");
			if (object.ReferenceEquals(this, value))
				return;
			PackedData = value.PackedData;
			Value = value.Value;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public static ConditionalFormattingValueInfo CreateDefault() {
			ConditionalFormattingValueInfo result = new ConditionalFormattingValueInfo();
			result.PackedData = (uint)ConditionalFormattingValueObjectType.Min | packedIsGteMask; 
			return result;
		}
		static internal bool IsFormulaEqual(byte[] arg1, byte[] arg2) {
			bool isArg1Null = arg1 == null;
			if (isArg1Null ^ (arg2 == null))
				return false;
			if (isArg1Null)
				return true;
			int length = arg1.Length;
			if (length != arg2.Length)
				return false;
			for (int i = 0; i < length; i++)
				if (arg1[i] != arg2[i])
					return false;
			return true;
		}
		public override bool Equals(object obj) {
			ConditionalFormattingValueInfo info = obj as ConditionalFormattingValueInfo;
			if (info == null)
				return false;
			if (object.ReferenceEquals(this, obj))
				return true;
			return PackedData == info.PackedData &&
				   IsFormulaEqual(Value, info.Value);
		}
		int GetValueHash() {
			int result = 0;
			if (Value != null)
				foreach (byte item in Value)
					result ^= (int)item;
			return result;
		}
		public override int GetHashCode() {
			return (int)(PackedData) ^ GetValueHash();
		}
		#region Creation helpers
		internal static ConditionalFormattingValueInfo Create(Worksheet worksheet, ConditionalFormattingValueObjectType valueType, bool isGte, double value) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			ConditionalFormattingValueInfo result = new ConditionalFormattingValueInfo();
			result.ValueType = valueType;
			result.IsGreaterOrEqual = isGte;
			switch (valueType) {
				case ConditionalFormattingValueObjectType.Formula:
				case ConditionalFormattingValueObjectType.Num:
				case ConditionalFormattingValueObjectType.Percent:
				case ConditionalFormattingValueObjectType.Percentile:
					result.Value = ConditionalFormattingValueObject.GeneratePackedValue(worksheet, value);
					break;
				default:
					result.Value = null;
					break;
			}
			return result;
		}
		internal static ConditionalFormattingValueInfo Create(Worksheet worksheet, ConditionalFormattingValueObjectType valueType, bool isGte, string value) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			ConditionalFormattingValueInfo result = new ConditionalFormattingValueInfo();
			result.ValueType = valueType;
			result.IsGreaterOrEqual = isGte;
			switch (valueType) {
				case ConditionalFormattingValueObjectType.Formula:
				case ConditionalFormattingValueObjectType.Num:
				case ConditionalFormattingValueObjectType.Percent:
				case ConditionalFormattingValueObjectType.Percentile:
					result.Value = ConditionalFormattingValueObject.GeneratePackedValue(worksheet, value);
					break;
				default:
					result.Value = null;
					break;
			}
			return result;
		}
		#endregion
	}
	#endregion
	#region ConditionalFormattingValueInfoCache
	public class ConditionalFormattingValueInfoCache : UniqueItemsCache<ConditionalFormattingValueInfo> {
		internal const int DefaultItemIndex = 0;
		public ConditionalFormattingValueInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override ConditionalFormattingValueInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return ConditionalFormattingValueInfo.CreateDefault();
		}
	}
	#endregion
	#region ConditionalFormattingValueObject
	#region IConditionalFormattingValueObject
	public interface IConditionalFormattingValueObject {
		ConditionalFormattingValueObjectType ValueType { get; set; }
		bool IsGreaterOrEqual { get; set; }
		void SetValue(double value);
		void SetValue(string value);
		VariantValue Evaluate();
		bool Check(double value);
	}
	#endregion
	#region ConditionalFormattingValueObject
	public class ConditionalFormattingValueObject : SpreadsheetLightIndexBasedObject<ConditionalFormattingValueInfo>,
													IConditionalFormattingValueObject,
													ISupportsCopyFrom<ConditionalFormattingValueObject>,
													ICloneable<ConditionalFormattingValueObject> {
		#region Fields
		static readonly int functionMax = FormulaCalculator.GetFunctionByInvariantName("MAX").Code;
		static readonly int functionMin = FormulaCalculator.GetFunctionByInvariantName("MIN").Code;
		static readonly int functionPercentile = FormulaCalculator.GetFunctionByInvariantName("PERCENTILE").Code;
		ConditionalFormatting owner;
		ParsedExpression expression;
		int cachedContentVersion = -1;
		VariantValue cachedCalculationResult;
		Formula valueExpression;
		#endregion
		protected internal ConditionalFormattingValueObject(Worksheet sheet, int infoIndex)
			: base(sheet) {
			SetIndexInitial(infoIndex);
			this.owner = null;
			this.valueExpression = new Formula(null);
			GenerateValueFromInfo();
		}
		internal ConditionalFormattingValueObject(Worksheet sheet, ConditionalFormattingValueObjectType objectType, double value, bool gte)
			: base(sheet) {
			ConditionalFormattingValueInfo info = ConditionalFormattingValueInfo.Create(sheet, objectType, gte, value);
			int index = GetCache(DocumentModel).AddItem(info);
			SetIndexInitial(index);
			this.owner = null;
			this.valueExpression = new Formula(null);
			GenerateValueFromInfo();
		}
		internal ConditionalFormattingValueObject(Worksheet sheet, ConditionalFormattingValueObjectType objectType, double value)
			: this(sheet, objectType, value, true) {
		}
		internal ConditionalFormattingValueObject(Worksheet sheet, ConditionalFormattingValueObjectType objectType, string value, bool gte)
			: base(sheet) {
			ConditionalFormattingValueInfo info = ConditionalFormattingValueInfo.Create(sheet, objectType, gte, value);
			int index = GetCache(DocumentModel).AddItem(info);
			SetIndexInitial(index);
			this.valueExpression = new Formula(null);
			GenerateValueFromInfo();
		}
		internal ConditionalFormattingValueObject(Worksheet sheet, ConditionalFormattingValueObjectType objectType, string value)
			: this(sheet, objectType, value, true) {
		}
		public ConditionalFormattingValueObject(Worksheet sheet)
			: this(sheet, ConditionalFormattingValueInfoCache.DefaultItemIndex) {
		}
		internal ParsedExpression Expression {
			get { return expression; }
			set {
				expression = value;
				ResetCachedContentVersion();
			}
		}
		internal ParsedExpression ValueExpression { get { return valueExpression.Expression; } }
		public string Value { get { return GetValue(CultureInfo.InvariantCulture); } set { SetValue(value); } }
		#region IConditionalFormattingValueObject Members
		#region ValueType
		public ConditionalFormattingValueObjectType ValueType {
			get {
				return Info.ValueType;
			}
			set {
				if (ValueType == value)
					return;
				SetPropertyValue(SetValueTypeCore, value);
			}
		}
		DocumentModelChangeActions SetValueTypeCore(ConditionalFormattingValueInfo info, ConditionalFormattingValueObjectType value) {
			info.ValueType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IsGreaterOrEqual
		public bool IsGreaterOrEqual {
			get {
				return Info.IsGreaterOrEqual;
			}
			set {
				if (IsGreaterOrEqual == value)
					return;
				SetPropertyValue(SetGteCore, value);
			}
		}
		DocumentModelChangeActions SetGteCore(ConditionalFormattingValueInfo info, bool value) {
			info.IsGreaterOrEqual = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		string ExpressionToString(ParsedExpression expression, WorkbookDataContext context) {
			string result = String.Empty;
			IParsedThing part1 = expression[0];
			if ((expression.Count == 1) && ((part1 is ParsedThingNumeric) || (part1 is ParsedThingInteger))) {
				result = expression.BuildExpressionString(context);
			}
			else
				result = "=" + expression.BuildExpressionString(context);
			return result;
		}
		protected string GetValue(WorkbookDataContext context) {
			string result = String.Empty;
			if (valueExpression != null) {
				ParsedExpression expr = valueExpression.Expression;
				if ((expr != null) && (expr.Count > 0)) {
					result = ExpressionToString(expr, context);
				}
			}
			return result;
		}
		protected string GetValue(CultureInfo culture) {
			string result = String.Empty;
			{
				WorkbookDataContext context = Worksheet.DataContext;
				context.PushCulture(culture);
				try {
					result = GetValue(context);
				}
				finally {
					context.PopCulture();
				}
			}
			return result;
		}
		protected string GetValue() {
			return GetValue(Worksheet.DataContext);
		}
		protected void SetValue(byte[] value) {
			if (ConditionalFormattingValueInfo.IsFormulaEqual(Info.Value, value))
				return;
			SetPropertyValue(SetValueCore, value);
			GenerateValueFromInfo();
			if ((owner != null) && (owner.CellRangeInternalNoHistory != null))
				expression = CreateExpression();
		}
		DocumentModelChangeActions SetValueCore(ConditionalFormattingValueInfo info, byte[] value) {
			info.Value = value;
			return DocumentModelChangeActions.None;
		}
		internal static byte[] GeneratePackedValue(Worksheet worksheet, double value) {
			ParsedExpression expr = new ParsedExpression();
			expr.Add(new ParsedThingNumeric() { Value = value });
			Formula newValue = new Formula(null, expr);
			return newValue.GetBinary(worksheet.DataContext);
		}
		public void SetValue(double value) {
			SetValue(GeneratePackedValue(Worksheet, value));
		}
		internal static byte[] GeneratePackedValue(Worksheet worksheet, string value) {
			WorkbookDataContext context = worksheet.DataContext;
			context.PushCulture(CultureInfo.InvariantCulture);
			try {
				ParsedExpression expr = context.ParseExpression(value, OperandDataType.Value, false);
				if (expr == null)
					return null;
				Formula newValue = new Formula(null, expr);
				return newValue.GetBinary(context);
			}
			finally {
				context.PopCulture();
			}
		}
		public void SetValue(string value) {
			SetValue(GeneratePackedValue(Worksheet, value));
		}
		public IModelErrorInfo CanSetValue(string value) {
			ParsedExpression expression;
			WorkbookDataContext context = Worksheet.DataContext;
			context.PushCulture(CultureInfo.InvariantCulture);
			try {
				expression = context.ParseExpression(value, OperandDataType.Value, false);
			}
			finally {
				context.PopCulture();
			}
			ConditionalFormattingFormulaInspector inspector = new ConditionalFormattingFormulaInspector();
			return inspector.Process(expression, false);
		}
		public VariantValue Evaluate() {
			if (Expression == null)
				return VariantValue.Empty;
			return Evaluate(DocumentModel.DataContext);
		}
		#region Check
		public bool Check(double value) {
			VariantValue exprValue = Evaluate();
			if (exprValue.IsNumeric) {
				double referenceValue = exprValue.NumericValue;
				return IsGreaterOrEqual ? (value >= referenceValue) : (value > referenceValue);
			}
			return false;
		}
		#endregion
		#endregion
		void GenerateValueFromInfo() {
			byte[] newValue = Info.Value;
			if ((newValue == null) || (newValue.Length < 1)) {
				if (valueExpression.Expression == null)
					valueExpression = new Formula(null, new ParsedExpression());
				else
					valueExpression.Expression.Clear();
			}
			else {
				ParsedExpression newExpr = Worksheet.DataContext.RPNContext.BinaryToExpression(newValue, 1);
				valueExpression = new Formula(null, newExpr);
			}
		}
		protected internal override void OnIndexChanged(int oldIndex) {
			ResetCachedContentVersion();
			GenerateValueFromInfo();
			base.OnIndexChanged(oldIndex);
		}
		void ResetCachedContentVersion() {
			cachedContentVersion = -1;
		}
		protected internal VariantValue Evaluate(WorkbookDataContext context) {
			int contentVersion = context.Workbook.ContentVersion;
			if (contentVersion == cachedContentVersion)
				return cachedCalculationResult;
			this.cachedContentVersion = contentVersion;
			this.cachedCalculationResult = Expression.Evaluate(context).ToNumeric(context);
			return cachedCalculationResult;
		}
		protected internal override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None; 
		}
		protected internal override UniqueItemsCache<ConditionalFormattingValueInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.ConditionalFormattingValueCache;
		}
		protected internal override void ApplyChanges(DocumentModelChangeActions changeActions) {
			Worksheet.ApplyChanges(changeActions);
		}
		protected internal void SetOwner(ConditionalFormatting value) {
			Guard.ArgumentNotNull(value, "value");
			SetOwnerCore(value);
		}
		protected internal void SetOwnerCore(ConditionalFormatting value) {
			owner = value;
			Expression = CreateExpression();
		}
		#region CreateExpression
		void AppendValueFuncExpression(ParsedExpression target, ParsedThingVariantValue rangeExpr, int funcCode) {
			target.Add(rangeExpr);
			target.Add(new ParsedThingFuncVar() { DataType = OperandDataType.Value, FuncCode = funcCode, ParamCount = 1 });
		}
		protected ParsedExpression CreateExpression() {
			ParsedExpression value;
			ParsedThingVariantValue rangeExpr = new ParsedThingVariantValue(new VariantValue() { CellRangeValue = owner.CellRangeInternalNoHistory });
			ParsedExpression result = new ParsedExpression();
			switch (Info.ValueType) {
				case ConditionalFormattingValueObjectType.Max:
					AppendValueFuncExpression(result, rangeExpr, functionMax);
					break;
				case ConditionalFormattingValueObjectType.Min:
					AppendValueFuncExpression(result, rangeExpr, functionMin);
					break;
				case ConditionalFormattingValueObjectType.Formula:
				case ConditionalFormattingValueObjectType.Num:
					value = valueExpression.Expression;
					if ((value == null) || (value.Count < 1))
						return null;
					result.AddRange(value);
					break;
				case ConditionalFormattingValueObjectType.Percent:
					value = valueExpression.Expression;
					if ((value == null) || (value.Count < 1))
						return null;
					AppendValueFuncExpression(result, rangeExpr, functionMax);
					AppendValueFuncExpression(result, rangeExpr, functionMin);
					result.Add(ParsedThingSubtract.Instance);
					result.AddRange(value);
					result.Add(new ParsedThingNumeric() { Value = 0.01 });
					result.Add(ParsedThingMultiply.Instance);
					result.Add(ParsedThingMultiply.Instance);
					AppendValueFuncExpression(result, rangeExpr, functionMin);
					result.Add(ParsedThingAdd.Instance);
					break;
				case ConditionalFormattingValueObjectType.Percentile:
					value = valueExpression.Expression;
					if ((value == null) || (value.Count < 1))
						return null;
					result.Add(rangeExpr);
					result.AddRange(value);
					result.Add(new ParsedThingNumeric() { Value = 0.01 });
					result.Add(ParsedThingMultiply.Instance);
					result.Add(new ParsedThingFuncVar() { DataType = OperandDataType.Value, FuncCode = functionPercentile, ParamCount = 2 });
					break;
				case ConditionalFormattingValueObjectType.AutoMin:
					result.Add(new ParsedThingNumeric() { Value = 0 });
					AppendValueFuncExpression(result, rangeExpr, functionMin);
					result.Add(new ParsedThingFuncVar() { DataType = OperandDataType.Value, FuncCode = functionMin, ParamCount = 2 });
					break;
				case ConditionalFormattingValueObjectType.AutoMax:
					result.Add(new ParsedThingNumeric() { Value = 0 });
					AppendValueFuncExpression(result, rangeExpr, functionMax);
					result.Add(new ParsedThingFuncVar() { DataType = OperandDataType.Value, FuncCode = functionMax, ParamCount = 2 });
					break;
				default:
					Exceptions.ThrowInvalidOperationException("cfvo@type invalid value");
					result = null;
					break;
			}
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ConditionalFormattingValueObject> Members
		public void CopyFrom(ConditionalFormattingValueObject value) {
			Index = value.Index;
			this.owner = value.owner;
		}
		#endregion
		#region ICloneable<ConditionalFormattingValueObject> Members
		public ConditionalFormattingValueObject Clone() {
			ConditionalFormattingValueObject result = new ConditionalFormattingValueObject(Worksheet, Index);
			result.owner = this.owner;
			return result;
		}
		#endregion
		#region Clone to particular sheet
		protected internal ConditionalFormattingValueObject Clone(Worksheet targetSheet) {
			int index;
			if (Worksheet.Workbook != targetSheet.Workbook) {
				string formula = ToString(CultureInfo.InvariantCulture);
				ConditionalFormattingValueInfo info = ConditionalFormattingValueInfo.Create(targetSheet, Info.ValueType, Info.IsGreaterOrEqual, formula);
				index = targetSheet.Workbook.Cache.ConditionalFormattingValueCache.AddItem(info);
			}
			else
				index = Index;
			ConditionalFormattingValueObject result = new ConditionalFormattingValueObject(targetSheet, index);
			return result;
		}
		#endregion
		#region ToString
		public override string ToString() {
			return GetValue();
		}
		internal string ToString(CultureInfo culture) {
			return GetValue(culture);
		}
		#endregion
		protected override ConditionalFormattingValueInfo GetInfoForModification() {
			return Info.Clone();
		}
		public override bool Equals(object obj) {
			ConditionalFormattingValueObject value = obj as ConditionalFormattingValueObject;
			if (value == null)
				return false;
			if (!object.ReferenceEquals(owner, value.owner))
				return false;
			return Index == value.Index;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), Index);
		}
	}
	#endregion
	#endregion
	#region ConditionalFormattingInfo
	public class ConditionalFormattingInfo : ICloneable<ConditionalFormattingInfo>, ISupportsCopyFrom<ConditionalFormattingInfo>, ISupportsSizeOf {
		#region PackedDataStorage
		[Flags]
		enum PackedDataMask {
			None = 0,
			IsPivot = 1,
			StopIfTrue = 2
		}
		#endregion
		readonly static int HashUniqueModifier = typeof(ConditionalFormattingInfo).GetHashCode();
		public const bool DefaultIsPivotValue = false;
		public const bool DefaultStopIfTrueValue = false;
		int packedData;
		protected int PackedData { get { return packedData; } set { packedData = value; } }
		public bool StopIfTrue { get { return GetBooleanValue((int)PackedDataMask.StopIfTrue); } set { SetBooleanValue((int)PackedDataMask.StopIfTrue, value); } }
		public bool IsPivot { get { return GetBooleanValue((int)PackedDataMask.IsPivot); } set { SetBooleanValue((int)PackedDataMask.IsPivot, value); } }
		#region Packed data helpers
		protected static bool GetBooleanValue(int packedStorage, int mask) {
			return (packedStorage & mask) != 0;
		}
		protected bool GetBooleanValue(int mask) {
			return GetBooleanValue(PackedData, mask);
		}
		protected static int PackBooleanValue(int packedStorage, int mask, bool value) {
			if (value)
				return packedStorage | mask;
			return packedStorage & ~mask;
		}
		protected void SetBooleanValue(int mask, bool value) {
			PackedData = PackBooleanValue(PackedData, mask, value);
		}
		protected int GetPackedIntValue(int startPos, int mask) {
			return (PackedData >> startPos) & mask;
		}
		protected void SetPackedIntValue(int startPos, int mask, int value) {
			int cleanValue = PackedData & ~(mask << startPos);
			PackedData = cleanValue | ((value & mask) << startPos);
		}
		#endregion
		#region ICloneable<ConditionalFormattingInfo> Members
		protected virtual ConditionalFormattingInfo CloneCore() {
			ConditionalFormattingInfo result = new ConditionalFormattingInfo();
			result.CopyFrom(this);
			return result;
		}
		public ConditionalFormattingInfo Clone() {
			return CloneCore();
		}
		#endregion
		#region ISupportsCopyFrom<ConditionalFormattingInfo> Members
		protected virtual void CopyFromCore(ConditionalFormattingInfo value) {
			Guard.ArgumentNotNull(value, "value");
			PackedData = value.PackedData;
		}
		public void CopyFrom(ConditionalFormattingInfo value) {
			CopyFromCore(value);
		}
		#endregion
		#region ISupportsSizeOf Members
		protected virtual int SizeOfCore() {
			return DXMarshal.SizeOf(GetType());
		}
		public int SizeOf() {
			return SizeOfCore();
		}
		#endregion
		public override bool Equals(object obj) {
			ConditionalFormattingInfo info = obj as ConditionalFormattingInfo;
			if (info == null)
				return false;
			if (object.ReferenceEquals(this, obj))
				return true;
			return PackedData == info.PackedData;
		}
		public override int GetHashCode() {
			return HashUniqueModifier ^ PackedData;
		}
		public static ConditionalFormattingInfo CreateDefault(IDocumentModelUnitConverter unitConverter) {
			return new ConditionalFormattingInfo();
		}
	}
	#endregion
	#region DataBarConditionalFormattingInfo
	public class DataBarConditionalFormattingInfo : ConditionalFormattingInfo,
													ICloneable<DataBarConditionalFormattingInfo>,
													ISupportsCopyFrom<DataBarConditionalFormattingInfo>,
													ISupportsSizeOf {
		#region Fields
		const int showValueMask = 4;
		const int gradientValueMask = 8;
		readonly static int HashUniqueModifier = typeof(DataBarConditionalFormattingInfo).GetHashCode();
		int minWidth;
		int maxWidth;
		public const int DefaultMinWidth = 10;
		public const int DefaultMaxWidth = 90;
		public const bool DefaultGradientFillValue = true;
		public const bool DefaultShowValue = true;
		public static ConditionalFormattingDataBarAxisPosition DefaultAxisPosition = ConditionalFormattingDataBarAxisPosition.Automatic;
		public static ConditionalFormattingDataBarDirection DefaultDirection = ConditionalFormattingDataBarDirection.Context;
		const int axisPositionMask = 3;
		const int axisPositionPos = 4;
		const int directionPosition = 6;
		const int directionMask = 3;
		#endregion
		#region Properties
		public int MinWidth { get { return minWidth; } set { minWidth = value; } }
		public int MaxWidth { get { return maxWidth; } set { maxWidth = value; } }
		public ConditionalFormattingDataBarAxisPosition AxisPosition {
			get { return (ConditionalFormattingDataBarAxisPosition)GetPackedIntValue(axisPositionPos, axisPositionMask); }
			set { SetPackedIntValue(axisPositionPos, axisPositionMask, (int)value); }
		}
		public ConditionalFormattingDataBarDirection Direction {
			get { return (ConditionalFormattingDataBarDirection)GetPackedIntValue(directionPosition, directionMask); }
			set { SetPackedIntValue(directionPosition, directionMask, (int)value); }
		}
		public bool GradientFill { get { return GetBooleanValue(gradientValueMask); } set { SetBooleanValue(gradientValueMask, value); } }
		public bool ShowValue { get { return GetBooleanValue(showValueMask); } set { SetBooleanValue(showValueMask, value); } }
		#endregion
		#region ConditionalFormattingInfo members
		protected override ConditionalFormattingInfo CloneCore() {
			DataBarConditionalFormattingInfo result = new DataBarConditionalFormattingInfo();
			result.CopyFrom(this);
			return result;
		}
		protected void CopyFromCore(DataBarConditionalFormattingInfo value) {
			base.CopyFromCore(value);
			MinWidth = value.MinWidth;
			MaxWidth = value.MaxWidth;
		}
		protected override void CopyFromCore(ConditionalFormattingInfo value) {
			DataBarConditionalFormattingInfo detail = value as DataBarConditionalFormattingInfo;
			if (detail != null)
				CopyFromCore(detail);
			else
				Exceptions.ThrowInvalidOperationException("Can't copy from " + value.ToString() + " to " + this.ToString());
		}
		protected override int SizeOfCore() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		#region ICloneable<DataBarConditionalFormattingInfo> Members
		public new DataBarConditionalFormattingInfo Clone() {
			return (DataBarConditionalFormattingInfo)CloneCore();
		}
		#endregion
		#region ISupportsCopyFrom<DataBarConditionalFormattingInfo> Members
		public void CopyFrom(DataBarConditionalFormattingInfo value) {
			CopyFromCore(value);
		}
		#endregion
		#region ISupportsSizeOf Members
		int ISupportsSizeOf.SizeOf() {
			return SizeOfCore();
		}
		#endregion
		#region Equals(object) - object comparing support
		public override bool Equals(object obj) {
			DataBarConditionalFormattingInfo info = obj as DataBarConditionalFormattingInfo;
			if (info == null)
				return false;
			if (object.ReferenceEquals(this, obj))
				return true;
			return PackedData == info.PackedData &&
				   MinWidth == info.MinWidth &&
				   MaxWidth == info.MaxWidth;
		}
		#endregion
		#region GetHashCode() - hash tables support
		public override int GetHashCode() {
			return HashUniqueModifier ^ PackedData ^ MinWidth ^ MaxWidth;
		}
		#endregion
		public static new DataBarConditionalFormattingInfo CreateDefault(IDocumentModelUnitConverter unitConverter) {
			DataBarConditionalFormattingInfo result = new DataBarConditionalFormattingInfo();
			result.minWidth = DefaultMinWidth;
			result.maxWidth = DefaultMaxWidth;
			result.AxisPosition = DefaultAxisPosition;
			result.GradientFill = DefaultGradientFillValue;
			result.ShowValue = DefaultShowValue;
			return result;
		}
	}
	#endregion
	#region IconSetConditionalFormattingInfo
	public class IconSetConditionalFormattingInfo : ConditionalFormattingInfo,
													ICloneable<IconSetConditionalFormattingInfo>,
													ISupportsCopyFrom<IconSetConditionalFormattingInfo>,
													ISupportsSizeOf {
		#region Fields
		const int showValueMask = 4;	   
		const int percentValueMask = 8;	   
		const int reversedValueMask = 16;	  
		const int customIconSetValueMask = 32; 
		const int iconSetStartPos = 6;
		const int iconSetBitLength = 5;
		const int iconSetValueMask = (1 << iconSetBitLength) - 1; 
		const int iconIdBitLength = 3;
		const int iconIdValueMask = (1 << iconIdBitLength) - 1;
		const int customIconBitLength = iconSetBitLength + iconIdBitLength;
		const int customIconValueMask = (1 << customIconBitLength) - 1;
		long packedData2;
		readonly static int HashUniqueModifier = typeof(IconSetConditionalFormattingInfo).GetHashCode();
		public static IconSetType DefaultIconSetValue = IconSetType.TrafficLights13;
		public static bool DefaultPercentValue = true;
		public static bool DefaultReversedValue = false;
		public static bool DefaultShowValue = true;
		public static bool DefaultCustomIconSetValue = false;
		#endregion
		#region Properties
		protected long PackedData2 { get { return packedData2; } set { packedData2 = value; } }
		public bool Percent { get { return GetBooleanValue(percentValueMask); } set { SetBooleanValue(percentValueMask, value); } }
		public bool Reversed { get { return GetBooleanValue(reversedValueMask); } set { SetReversed(value); } }
		public bool ShowValue { get { return GetBooleanValue(showValueMask); } set { SetBooleanValue(showValueMask, value); } }
		public IconSetType IconSet { get { return (IconSetType)GetPackedIntValue(iconSetStartPos, iconSetValueMask); } set { SetPredefinedIconSet(value, Reversed); } }
		public bool CustomIconSet { get { return GetBooleanValue(customIconSetValueMask); } set { SetBooleanValue(customIconSetValueMask, value); } }
		#endregion
		#region ConditionalFormattingInfo members
		protected override ConditionalFormattingInfo CloneCore() {
			IconSetConditionalFormattingInfo result = new IconSetConditionalFormattingInfo();
			result.CopyFrom(this);
			return result;
		}
		protected void CopyFromCore(IconSetConditionalFormattingInfo value) {
			base.CopyFromCore(value);
			PackedData2 = value.PackedData2;
		}
		protected override void CopyFromCore(ConditionalFormattingInfo value) {
			base.CopyFromCore(value);
			IconSetConditionalFormattingInfo detail = value as IconSetConditionalFormattingInfo;
			if (detail != null)
				CopyFromCore(detail);
			else
				Exceptions.ThrowInvalidOperationException("Can't copy from " + value.ToString() + " to " + this.ToString());
		}
		protected override int SizeOfCore() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		#region ICloneable<ColorScaleConditionalFormattingInfo> Members
		public new IconSetConditionalFormattingInfo Clone() {
			return (IconSetConditionalFormattingInfo)CloneCore();
		}
		#endregion
		#region ISupportsCopyFrom<ColorScaleConditionalFormattingInfo> Members
		public void CopyFrom(IconSetConditionalFormattingInfo value) {
			CopyFromCore(value);
		}
		#endregion
		#region ISupportsSizeOf Members
		int ISupportsSizeOf.SizeOf() {
			return SizeOfCore();
		}
		#endregion
		#region Equals(object) - object comparing support
		public override bool Equals(object obj) {
			IconSetConditionalFormattingInfo info = obj as IconSetConditionalFormattingInfo;
			if (info == null)
				return false;
			if (object.ReferenceEquals(this, obj))
				return true;
			return PackedData == info.PackedData &&
				   PackedData2 == info.PackedData2;
		}
		#endregion
		#region GetHashCode() - hash tables support
		public override int GetHashCode() {
			return HashUniqueModifier ^ PackedData ^ PackedData2.GetHashCode();
		}
		#endregion
		void SetReversed(bool value) {
			SetBooleanValue(reversedValueMask, value);
			ApplyIconSequence(IconSet, value);
		}
		static int PackCustomIcon(IconSetType iconSet, int iconIndex) {
			return ((int)iconSet) & iconSetValueMask | ((iconIndex & iconIdValueMask) << iconSetBitLength);
		}
		static int PackCustomIcon(ConditionalFormattingCustomIcon value) {
			return PackCustomIcon(value.IconSet, value.IconIndex);
		}
		protected internal long ModifiedIconValue(int index, ConditionalFormattingCustomIcon value) {
			long result = PackedData2;
			int startPos = index * customIconBitLength;
			result &= ~(customIconValueMask << startPos);
			long newValue = PackCustomIcon(value);
			result |= newValue << startPos;
			return result;
		}
		protected internal void ApplyModifiedIconValue(long newValue) {
			PackedData2 = newValue;
		}
		public void SetCustomIcon(int index, ConditionalFormattingCustomIcon value) {
			ApplyModifiedIconValue(ModifiedIconValue(index, value));
		}
		internal static bool IconSetContainIcon(IconSetType iconSet, int iconId) {
			if (iconId < 0)
				return false;
			int numOfIcons;
			if ((iconSet == IconSetType.None) && (iconId == 0))
				return true;
			if (!IconSetConditionalFormatting.expectedPointsNumberTable.TryGetValue(iconSet, out numOfIcons))
				return false;
			return iconId < numOfIcons;
		}
		protected internal long ModifiedIconValue(ConditionalFormattingCustomIcon[] values) {
			long result = 0;
			int numOfIcons;
			if (IconSetConditionalFormatting.expectedPointsNumberTable.TryGetValue(IconSet, out numOfIcons) && (values.Length <= numOfIcons)) {
				int shifter = 0;
				for (int i = 0; i < values.Length; ++i) {
					IconSetType iconSet = values[i].IconSet;
					int iconIndex = values[i].IconIndex;
					if (IconSetContainIcon(iconSet, iconIndex)) {
						result |= ((long)PackCustomIcon(iconSet, iconIndex)) << shifter;
						shifter += customIconBitLength;
					}
				}
			}
			return result;
		}
		protected internal bool IsSameIconSequence(long value) {
			return PackedData2 == value;
		}
		public void SetCustomIcons(ConditionalFormattingCustomIcon[] values) {
			ApplyModifiedIconValue(ModifiedIconValue(values));
		}
		public ConditionalFormattingCustomIcon GetIcon(int index) {
			int startPos = index * customIconBitLength;
			long normalizedData = PackedData2 >> startPos;
			return new ConditionalFormattingCustomIcon((IconSetType)(normalizedData & iconSetValueMask), (int)(normalizedData >> iconSetBitLength) & iconIdValueMask);
		}
		void ApplyIconSequence(IconSetType iconSet, bool reversed) {
			int numOfIcons;
			if (IconSetConditionalFormatting.expectedPointsNumberTable.TryGetValue(iconSet, out numOfIcons)) {
				long newValue = 0;
				int iconId = reversed ? 0 : (numOfIcons - 1);
				int incValue = reversed ? 1 : -1;
				for (; numOfIcons > 0; --numOfIcons) {
					newValue <<= customIconBitLength;
					newValue |= (uint)PackCustomIcon(iconSet, iconId);
					iconId += incValue;
				}
				PackedData2 = newValue;
			}
			else {
				PackedData2 = 0;
			}
		}
		public void SetPredefinedIconSet(IconSetType iconSet, bool reversed) {
			SetPackedIntValue(iconSetStartPos, iconSetValueMask, (int)iconSet);
			ApplyIconSequence(iconSet, reversed);
		}
		public static new IconSetConditionalFormattingInfo CreateDefault(IDocumentModelUnitConverter unitConverter) {
			IconSetConditionalFormattingInfo result = new IconSetConditionalFormattingInfo();
			result.ShowValue = DefaultShowValue;
			result.Percent = DefaultPercentValue;
			result.SetPredefinedIconSet(DefaultIconSetValue, DefaultReversedValue);
			return result;
		}
	}
	#endregion
	#region ConditionalFormattingInfoCache
	public class ConditionalFormattingInfoCache : UniqueItemsCache<ConditionalFormattingInfo> {
		internal const int DefaultItemIndex = 0;
		internal const int DefaultDataBarItemIndex = 1;
		internal const int DefaultIconSetItemIndex = 2;
		public ConditionalFormattingInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override ConditionalFormattingInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return null; 
		}
		protected override void InitItems(IDocumentModelUnitConverter unitConverter) {
			AppendItem(ConditionalFormattingInfo.CreateDefault(unitConverter));
			AppendItem(DataBarConditionalFormattingInfo.CreateDefault(unitConverter));
			AppendItem(IconSetConditionalFormattingInfo.CreateDefault(unitConverter));
		}
	}
	#endregion
	public class ConditionalFormattingFormulaInspector : ParsedThingVisitor {
		bool haveArrays;
		bool haveErrors;
		bool haveRelativeReferences;
		public ModelErrorInfo Process(ParsedExpression expression, bool mayContainErrors) {
			if (expression == null)
				return new ModelErrorInfo(ModelErrorType.CondFmtInvalidExpression);
			if (expression.Count == 1) {
				IParsedThing thing = expression[0];
				if (thing is ParsedThingArea || thing is ParsedThingAreaN)
					return new ModelErrorInfo(ModelErrorType.CondFmtExpressionCantBeRange);
			}
			Process(expression);
			if (haveErrors && !mayContainErrors)
				return new ModelErrorInfo(ModelErrorType.CondFmtExpressionCantContainErrorValues);
			if (haveRelativeReferences)
				return new ModelErrorInfo(ModelErrorType.CondFmtExpressionCantUseRelativeRefs);
			if (haveArrays)
				return new ModelErrorInfo(ModelErrorType.CondFmtExpressionCantBeArray);
			return null;
		}
		#region Range helpers
		void CheckPositionItem(PositionType posType) {
			if (posType == PositionType.Relative)
				haveRelativeReferences = true;
		}
		void CheckPosition(CellPosition position) {
			CheckPositionItem(position.ColumnType);
			CheckPositionItem(position.RowType);
		}
		void CheckRange(CellRangeBase range) {
			switch (range.RangeType) {
				case CellRangeType.SingleRange:
					CheckPosition(range.TopLeft);
					CheckPosition(range.BottomRight);
					break;
				case CellRangeType.IntervalRange:
					CellIntervalRange interval = range as CellIntervalRange;
					if (interval.IsColumnInterval) {
						CheckPositionItem(range.TopLeft.ColumnType);
						CheckPositionItem(range.BottomRight.ColumnType);
					}
					else {
						CheckPositionItem(range.TopLeft.RowType);
						CheckPositionItem(range.BottomRight.RowType);
					}
					break;
				case CellRangeType.UnionRange:
					foreach (CellRangeBase item in (range as CellUnion).InnerCellRanges)
						CheckRange(item);
					break;
			}
		}
		#endregion
		#region IParsedThingVisitor Members
		public override void Visit(ParsedThingArray thing) {
			haveArrays = true;
		}
		public override void Visit(ParsedThingError thing) {
			haveErrors = true;
		}
		public override void Visit(ParsedThingUnknownFunc thing) {
			haveErrors = true;
		}
		public override void Visit(ParsedThingUnknownFuncExt thing) {
			haveErrors = true;
		}
		public override void Visit(ParsedThingMemErr thing) {
			haveErrors = true;
		}
		public override void Visit(ParsedThingAreaErr thing) {
			haveErrors = true;
		}
		public override void Visit(ParsedThingAreaErr3d thing) {
			haveErrors = true;
		}
		public override void Visit(ParsedThingRefErr thing) {
			haveErrors = true;
		}
		public override void Visit(ParsedThingErr3d thing) {
			haveErrors = true;
		}
		public override void Visit(ParsedThingRef thing) {
			CheckPosition(thing.Position);
		}
		public override void Visit(ParsedThingRef3d thing) {
			CheckPosition(thing.Position);
		}
		public override void Visit(ParsedThingArea thing) {
			CheckRange(thing.CellRange);
		}
		public override void Visit(ParsedThingArea3d thing) {
			CheckRange(thing.CellRange);
		}
		public override void Visit(ParsedThingVariantValue thing) {
			VariantValueType thingType = thing.Value.Type;
			switch (thingType) {
				case VariantValueType.CellRange:
					CheckRange(thing.Value.CellRangeValue);
					break;
				case VariantValueType.Error:
					haveErrors = true;
					break;
			}
		}
		#endregion
	}
}
