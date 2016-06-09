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
using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Utils;
using System.Globalization;
using System.Text.RegularExpressions;
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using System.Collections.Generic;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	#region CustomFilter
	public class CustomFilter {
		#region Static Members
		static char[] wildcards = new char[] { '?', '*' };
		internal static bool ContainsWildcardCharacters(string value) {
			return value.IndexOfAny(wildcards) > -1;
		}
		#endregion
		#region Fields
		FilterComparisonOperator filterOperator;
		string value;
		VariantValue numericValue;
		Regex wildcardRegex;
		#endregion
		#region Properties
		public FilterComparisonOperator FilterOperator { get { return filterOperator; } set { filterOperator = value; } }
		public string Value { get { return value; } set { this.value = value; } }
		public VariantValue NumericValue { get { return numericValue; } set { numericValue = value; } }
		public bool IsBlank { get { return String.IsNullOrEmpty(value) || value == " "; } }
		public bool IsDateTime { get; private set; }
		#endregion
		public void UpdateNumericValue(WorkbookDataContext dataContext, bool isDateTimeFilter) {
			UpdateNumericValueCore(dataContext, isDateTimeFilter);
			UpdateWildcard();
		}
		public void SetupFromInvariantValue(VariantValue value, bool isDateTimeFilter) {
			Debug.Assert(value.IsNumeric);
			Value = value.NumericValue.ToString(CultureInfo.InvariantCulture);
			NumericValue = value;
			IsDateTime = isDateTimeFilter;
		}
		void UpdateNumericValueCore(WorkbookDataContext dataContext, bool isDateTimeFilter) {
			if (String.IsNullOrEmpty(Value)) {
				numericValue = VariantValue.Empty;
				return;
			}
			if (isDateTimeFilter) {
				FormattedVariantValue result = dataContext.TryConvertStringToDateTimeValue(Value, false);
				if (!result.IsEmpty && result.Value.IsNumeric) {
					numericValue = result.Value;
					IsDateTime = true;
					return;
				}
			}
			double val;
			string stringValue = Value;
			bool isPercent = stringValue.EndsWith("%");
			if (isPercent)
				stringValue = stringValue.TrimEnd(new char[] { '%' });
			if (Double.TryParse(stringValue, NumberStyles.Float, dataContext.Culture, out val))
				numericValue = isPercent ? val / 100.0 : val;
			else
				numericValue = VariantValue.Empty;
		}
		void UpdateWildcard() {
			wildcardRegex = null;
			if (String.IsNullOrEmpty(Value))
				return;
			if (!ContainsWildcardCharacters(Value))
				return;
			wildcardRegex = WildcardComparer.CreateWildcardRegex(value);
		}
		public bool CalculatePredicate(IAutoFilterValue value, IFilteringBehaviour filteringBehaviour) {
			int sign;
			switch (FilterOperator) {
				default:
				case FilterComparisonOperator.Equal: {
						sign = CalculateSubtractionSign(value);
						if (sign == Int32.MinValue)
							return StringEquals(value, filteringBehaviour);
						return sign == 0;
					}
				case FilterComparisonOperator.NotEqual: {
						sign = CalculateSubtractionSign(value);
						if (sign == Int32.MinValue)
							return !StringEquals(value, filteringBehaviour);
						return sign != 0;
					}
				case FilterComparisonOperator.GreaterThan:
					return CalculateSubtractionSignWithStringComparison(value, filteringBehaviour) == 1;
				case FilterComparisonOperator.GreaterThanOrEqual:
					return CalculateSubtractionSignWithStringComparison(value, filteringBehaviour) >= 0;
				case FilterComparisonOperator.LessThan:
					return CalculateSubtractionSignWithStringComparison(value, filteringBehaviour) == -1;
				case FilterComparisonOperator.LessThanOrEqual:
					sign = CalculateSubtractionSignWithStringComparison(value, filteringBehaviour);
					return sign == 0 || sign == -1;
			}
		}
		int CalculateSubtractionSignWithStringComparison(IAutoFilterValue value, IFilteringBehaviour filteringBehaviour) {
			int sign = CalculateSubtractionSign(value);
			if (sign == Int32.MinValue && filteringBehaviour.AllowsStringComparison)
				sign = StringCompare(value);
			return sign;
		}
		int StringCompare(IAutoFilterValue autoFilterValue) {
			string text;
			if (autoFilterValue != null) {
				text = autoFilterValue.Text;
			}
			else
				text = String.Empty;
			return String.Compare(text, Value, StringComparison.CurrentCultureIgnoreCase);
		}
		bool StringEquals(IAutoFilterValue autoFilterValue, IFilteringBehaviour filteringBehaviour) {
			bool hasWildCards = wildcardRegex != null;
			string text;
			if (autoFilterValue != null) {
				if (autoFilterValue.Value.IsNumeric && hasWildCards && !filteringBehaviour.AllowsNumericAndWildcardComparison)
					return false;
				text = autoFilterValue.Text;
			}
			else
				text = String.Empty;
			if (hasWildCards)
				return WildcardComparer.Match(wildcardRegex, text);
			return String.IsNullOrEmpty(text) ? IsBlank : text == Value;
		}
		int CalculateSubtractionSign(IAutoFilterValue autoFilterValue) {
			if (autoFilterValue == null)
				return Int32.MinValue;
			if (!NumericValue.IsNumeric)
				return Int32.MinValue;
			VariantValue val = autoFilterValue.Value;
			if (!val.IsNumeric)
				return Int32.MinValue;
			return Math.Sign(val.NumericValue - NumericValue.NumericValue);
		}
	}
	#endregion
	#region CustomFilterCollection
	public class CustomFilterCollection : SimpleCollection<CustomFilter> {
		Worksheet sheet;
		bool criterionAnd;
		public CustomFilterCollection(Worksheet sheet) {
			this.sheet = sheet;
		}
		public Worksheet Sheet { get { return sheet; } }
		public bool CriterionAnd {
			get { return criterionAnd; }
			set {
				if (criterionAnd == value)
					return;
				DocumentHistory history = Sheet.Workbook.History;
				CustomFilterCriterionAndHistoryItem item = new CustomFilterCriterionAndHistoryItem(this, value, criterionAnd);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetCriterionAndCore(bool value) {
			criterionAnd = value;
		}
		public override int Add(CustomFilter item) {
			Guard.ArgumentNotNull(item, "item");
			Insert(InnerList.Count, item);
			return InnerList.Count - 1;
		}
		public override void Clear() {
			for (int i = Count - 1; i >= 0; --i)
				RemoveAt(i);
		}
		public override void Insert(int index, CustomFilter item) {
			Guard.ArgumentNotNull(item, "item");
			DocumentHistory history = Sheet.Workbook.History;
			CustomFilterInsertHistoryItem historyItem = new CustomFilterInsertHistoryItem(this, item, index);
			history.Add(historyItem);
			historyItem.Execute();
		}
		public override void RemoveAt(int index) {
			CustomFilter deletedFilter = InnerList[index];
			Remove(deletedFilter);
		}
		public override bool Remove(CustomFilter item) {
			Guard.ArgumentNotNull(item, "item");
			DocumentHistory history = Sheet.Workbook.History;
			CustomFilterRemoveHistoryItem historyItem = new CustomFilterRemoveHistoryItem(this, item);
			history.Add(historyItem);
			historyItem.Execute();
			return true;
		}
		protected internal virtual void RemoveCore(CustomFilter item) {
			Guard.ArgumentNotNull(item, "item");
			InnerList.Remove(item);
		}
		protected internal virtual void InsertCore(CustomFilter item, int index) {
			if (Count == 2)
				DevExpress.Office.Utils.Exceptions.ThrowInternalException();
			Guard.ArgumentNotNull(item, "item");
			InnerList.Insert(index, item);
		}
		public void CopyFrom(CustomFilterCollection sourceCollection) {
			int count = sourceCollection.Count;
			for (int i = 0; i < count; i++) {
				CustomFilter sourceCustomFilter = sourceCollection[i];
				CustomFilter targetCustomFilter = new CustomFilter();
				targetCustomFilter.FilterOperator = sourceCustomFilter.FilterOperator;
				targetCustomFilter.Value = sourceCustomFilter.Value;
				targetCustomFilter.NumericValue = sourceCustomFilter.NumericValue;
				this.Add(targetCustomFilter);
			}
			this.CriterionAnd = sourceCollection.CriterionAnd;
		}
		public bool IsCellVisible(IAutoFilterValue value, IFilteringBehaviour filteringBehaviour) {
			int count = Count;
			if (count <= 0)
				return true;
			if (CriterionAnd) {
				for (int i = 0; i < count; i++)
					if (!this[i].CalculatePredicate(value, filteringBehaviour))
						return false;
				return true;
			}
			else {
				for (int i = 0; i < count; i++)
					if (this[i].CalculatePredicate(value, filteringBehaviour))
						return true;
				return false;
			}
		}
	}
	#endregion
}
