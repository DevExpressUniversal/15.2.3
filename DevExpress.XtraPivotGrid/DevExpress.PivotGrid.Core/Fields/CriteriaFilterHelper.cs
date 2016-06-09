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

using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.XtraPivotGrid;
using System.Linq;
using grouping = System.Linq.IGrouping<bool, DevExpress.XtraPivotGrid.PivotGroupFilterValue>;
using DevExpress.Utils;
namespace DevExpress.PivotGrid.QueryMode {
	static class CriteriaFilterHelper {
		public static CriteriaOperator CreateValuesCriteria(IEnumerable membersValues, CriteriaOperator columnCriteria, bool forceNullValue = false) {
			List<CriteriaOperator> values = new List<CriteriaOperator>();
			bool hasNull = false;
			foreach(object value in membersValues)
				if(!object.ReferenceEquals(null, value))
					values.Add(new OperandValue(value));
				else
					hasNull = true;
			if(hasNull)
				return CriteriaOperator.Or(GetValuesCriteria(columnCriteria, values), new NullOperator(columnCriteria));
			else
				return GetValuesCriteria(columnCriteria, values);
		}
		static CriteriaOperator GetValuesCriteria(CriteriaOperator columnCriteria, List<CriteriaOperator> values) {
			if(values.Count > 0)
				if(values.Count == 1)
					return new BinaryOperator(columnCriteria, values[0], BinaryOperatorType.Equal);
				else
					return new InOperator(columnCriteria, values);
			else
				return null;
		}
		public static CriteriaOperator CreateCriteria(PivotFilterType filterType, IEnumerable values, string fieldName, bool forceNullValue) {
			CriteriaOperator baseCriteria = CreateValuesCriteria(values, new OperandProperty(fieldName));
			if(filterType == PivotFilterType.Excluded) {
				baseCriteria = new NotOperator(baseCriteria);
				if(forceNullValue && IsNullable(values) && !HaveNull(values))
					return CriteriaOperator.Or(baseCriteria, new FunctionOperator(FunctionOperatorType.IsNull, new OperandProperty(fieldName)));
				else
					return baseCriteria;
			} else
				return baseCriteria;
		}
		static bool HaveNull(IEnumerable values) {
			foreach(object value in values)
				if(ReferenceEquals(null, value))
					return true;
			return false;
		}
		static bool IsNullable(IEnumerable values) {
			IEnumerator enumerator = values.GetEnumerator();
			bool result = false;
			while((result = enumerator.MoveNext()) == true && enumerator.Current == null)
				;
			object val = result ? enumerator.Current : null;
			if(!ReferenceEquals(null, val))
				return !val.GetType().IsValueType();
			return false;
		}
		public static CriteriaOperator CreateCriteria(PivotFilterType filterType, PivotGroupFilterValuesCollection collection, bool forceNullValue) {
			if(filterType == PivotFilterType.Included)
				return GetGroupCriteriaRecursive(collection, false);
			if(collection.Count == 0)
				return null;
			return new NotOperator(GetGroupCriteriaRecursive(collection, forceNullValue));
		}
		static CriteriaOperator GetGroupCriteriaRecursive(PivotGroupFilterValuesCollection values, bool forceNullValue) {
			CriteriaOperator criteria = null;
			if(values.Count == 0)
				return criteria;
			List<grouping> separate = values.GroupBy((gv) => gv.ChildValues == null || gv.ChildValues.Count == 0).OrderBy((p) => p.Key).ToList();
			if(separate[0].Key == false)
				criteria = CriteriaOperator.Or(
									 separate[0].Select((fv) =>
											CriteriaOperator.And(
													 GetValueCriteria(fv, forceNullValue),
													 GetGroupCriteriaRecursive(fv.ChildValues, forceNullValue)
								)));
			grouping withChildren = separate.Where((sk) => sk.Key).FirstOrDefault();
			if(withChildren != null) {
				string fieldName = values.Items[0].Field.Name;
				CriteriaOperator opCriteria = CreateCriteria(PivotFilterType.Included, withChildren.Select((gv) => gv.Value), fieldName, forceNullValue);
				if(forceNullValue && !withChildren.Any((fv) => fv.Value == null))
					opCriteria = CriteriaOperator.And(new NotOperator(new NullOperator(new OperandProperty(fieldName))), opCriteria);
				criteria = CriteriaOperator.Or(criteria, opCriteria);
			}
			return criteria;
		}
		static CriteriaOperator GetValueCriteria(PivotGroupFilterValue value, bool forceNullValue) {
			if(object.ReferenceEquals(null, value.Value))
				return new NullOperator(value.Field.Name);
			CriteriaOperator criteria = new BinaryOperator(
								   new OperandProperty(value.Field.Name),
								   new OperandValue(value.Value),
								   BinaryOperatorType.Equal);
			return forceNullValue ? CriteriaOperator.And(new NotOperator(new NullOperator(new OperandProperty(value.Field.Name))), criteria) : criteria;
		}
	}
}
