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
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Data;
namespace DevExpress.Snap.Core.Native {
	delegate bool FilterAction(CriteriaOperator criteriaOperator);
	public static class FilterStringHelper {
		#region inner classes
		class FilterStringVisitor : ICriteriaVisitor {
			readonly FilterAction action;
			public bool Result { get; private set; }
			public FilterStringVisitor(FilterAction action) {
				this.action = action;
			}
			public void Visit(BinaryOperator theOperator) {
				Result = action(theOperator.LeftOperand);
			}
			public void Visit(FunctionOperator theOperator) {
				Result = action(theOperator.Operands[0]);
			}
			public void Visit(BetweenOperator theOperator) {
				Result = action(theOperator.TestExpression);
			}
			public void Visit(UnaryOperator theOperator) {
				Result = action(theOperator.Operand);
			}
			public void Visit(InOperator theOperator) {
				Result = action(theOperator.LeftOperand);
			}
			public void Visit(GroupOperator theOperator) {
			}
			public void Visit(OperandValue theOperand) {
			}
		}
		#endregion
		static CriteriaOperator GetOrFilterString(string dataFieldName, IEnumerable<string> values) {
			if (values == null || !values.Any())
				return null;
			var escapedValues = new List<string>();
			foreach (string t in values) {
				escapedValues.Add(EscapeString(t));
			}
			if (escapedValues.Count <= 2) {
				var criteriaOperators = new List<CriteriaOperator>();
				escapedValues.ForEach(e => criteriaOperators.Add(new BinaryOperator(dataFieldName, e)));
				return new GroupOperator(GroupOperatorType.Or, criteriaOperators);
			}
			return new InOperator(dataFieldName, escapedValues);
		}
		static string EscapeString(string val) {
			return val.Replace("'", "''");
		}
		public static CriteriaOperator ModifyFilterString(CriteriaOperator filterCriteria, IEnumerable<string> values, string dataFieldName) {
			if (values == null)
				return filterCriteria;
			var newFilterString = GetOrFilterString(dataFieldName, values);
			return ModifyFilterStringCore(filterCriteria, newFilterString, dataFieldName);
		}
		public static CriteriaOperator ModifyFilterString(CriteriaOperator filterCriteria, CriteriaOperator newFilterCriteria, string dataFieldName) {
			return ModifyFilterStringCore(filterCriteria, newFilterCriteria, dataFieldName);
		}
		public static CriteriaOperator ModifyFilterString(string filterString, IEnumerable<string> values, string dataFieldName) {
			return ModifyFilterString(CriteriaOperator.Parse(filterString), values, dataFieldName);
		}
		public static CriteriaOperator ModifyFilterString(string filterString, CriteriaOperator newfilterCriteria, string dataFieldName) {
			return ModifyFilterString(CriteriaOperator.Parse(filterString), newfilterCriteria, dataFieldName);
		}
		static CriteriaOperator ModifyFilterStringCore(CriteriaOperator filterCriteria, CriteriaOperator newFilterCriteria, string dataFieldName) {
			var filterStringWithoutDataFieldName = GetFilterStringWithoutPropertyName(filterCriteria, dataFieldName);
			return ReferenceEquals(filterStringWithoutDataFieldName, null) ? newFilterCriteria
						: CriteriaOperator.And(filterStringWithoutDataFieldName, newFilterCriteria);
		}
		static CriteriaOperator GetFilterStringWithoutPropertyName(CriteriaOperator filterString, string propertyNameToIgnore) {
			return GetFilterStringCore(filterString, s => {
				FilterAction action = o => (o is OperandProperty && ((OperandProperty)o).PropertyName != propertyNameToIgnore);
				if (s is GroupOperator) {
					foreach (var op in ((GroupOperator)s).Operands) {
						if (op is GroupOperator)
							return true;
						var visitor = new FilterStringVisitor(action);
						op.Accept(visitor);
						if (visitor.Result) {
							return true;
						}
					}
					return false;
				}
				return action(s);
			});
		}
		public static CriteriaOperator GetFilterStringOnlyWithPropertyName(string filterString, string propertyName) {
			return GetFilterStringCore(CriteriaOperator.Parse(filterString), s => s is OperandProperty && ((OperandProperty)s).PropertyName == propertyName);
		}
		static CriteriaOperator GetFilterStringCore(CriteriaOperator criteriaOperator, FilterAction action) {
			if (!ReferenceEquals(criteriaOperator, null)) {
				var groupCriteriaOperator = criteriaOperator as GroupOperator;
				if (!ReferenceEquals(groupCriteriaOperator, null)) {
					var operatorType = groupCriteriaOperator.OperatorType;
					var criteriaOperators = new List<CriteriaOperator>();
					foreach (var criteria in groupCriteriaOperator.Operands) {
						if (criteria is GroupOperator && action(criteria))
							criteriaOperators.Add(criteria);
						else {
							var visitor = new FilterStringVisitor(action);
							criteria.Accept(visitor);
							if (visitor.Result) {
								if (operatorType == GroupOperatorType.Or)
									return groupCriteriaOperator;
								criteriaOperators.Add(criteria);
							}
						}
					}
					return operatorType == GroupOperatorType.And ? CriteriaOperator.And(criteriaOperators) : null;
				}
				var v = new FilterStringVisitor(action);
				criteriaOperator.Accept(v);
				if (v.Result)
					return criteriaOperator;
			}
			return null;
		}
	}
	public class FilterItem {
		public bool? IsChecked {
			get;
			set;
		}
		public object Value { get; set; }
		public override string ToString() {
			string result = GetDisplayValue();
			return !String.IsNullOrEmpty(result) ? result : base.ToString();
		}
		string GetDisplayValue() {
			if (Value == null)
				return String.Empty;
			char[] chars = Value.ToString().ToCharArray();
			StringBuilder displayValue = new StringBuilder();
			int length = chars.Length;
			for (int i = 0; i < length; i++) {
				if (chars[i] != '\n' || chars[i] == '\r')
					displayValue.Append(chars[i]);
			}
			return displayValue.ToString();
		}
	}
	public interface IUniqueFilteredValuesAccessor {
		IEnumerable<object> GetUniqueFilteredValues(string fieldName, int maxCount, bool includeFilteredOut, bool roundDataTime, OperationCompleted completed);
	}
	public static class FilteredValuesHelper {
		public static IEnumerable<FilterItem> GetUniqueFilteredValues(IUniqueFilteredValuesAccessor filteredValuesAccessor, string fieldName) {
			return GetUniqueFilteredValues(filteredValuesAccessor, fieldName, -1, false, null);
		}
		public static IEnumerable<FilterItem> GetUniqueFilteredValues(IUniqueFilteredValuesAccessor filteredValuesAccessor, string fieldName, int maxCount, bool roundDataTime, OperationCompleted completed) {
			var values = filteredValuesAccessor.GetUniqueFilteredValues(fieldName, maxCount, true, roundDataTime, completed);
			var filterItems = new List<FilterItem>();
			if (object.ReferenceEquals(values, null))
				return filterItems;
			foreach (object item in values) {
				filterItems.Add(new FilterItem { Value = item, IsChecked = false });
			}
			var filteredValues = filteredValuesAccessor.GetUniqueFilteredValues(fieldName, maxCount, false, roundDataTime, completed);
			if (filteredValues != null) {
				filterItems.ForEach(item => {
					if (filteredValues.Contains(item.Value))
						item.IsChecked = true;
				});
			}
			return filterItems;
		}
	}
}
