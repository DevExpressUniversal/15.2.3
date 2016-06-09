#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Native {
	class HierarchyColumnAggregator {
		readonly IFieldsTypeMap fieldsTypeMap;
		ValuesColumn column;
		int numericSpread = 0;
		int shuffleParam = 0;
		public int SubcategoryGroupCount = 3;
		public int IndependentGroupCount = 5;
		public bool EnableSpread = false;
		public bool NumericPercentMode = false;
		public HierarchyColumnAggregator(IFieldsTypeMap fieldsTypeMap) {
			this.fieldsTypeMap = fieldsTypeMap;
		}
		public void Aggregate(DataItem item, int uniqueCount, FakeDataAggregateType type, int groupCount = -1) {
			if (item != null)
				Aggregate(item.DataMember, uniqueCount, type, groupCount);
		}
		public void Aggregate(string fieldName, int uniqueCount, FakeDataAggregateType type, int groupCount = -1) {
			if (!CanAggregateField(fieldName))
				return;
			ValuesColumn second = CreateValuesColumn(fieldName, uniqueCount);
			Aggregate(second, type, groupCount);
		}
		public void Aggregate(ValuesColumn second, FakeDataAggregateType type, int groupCount = -1) {
			if (IsSkipOperation(second))
				return;
			switch (type) {
				case FakeDataAggregateType.Independent:
					column = column.ProductIndependent(second);
					break;
				case FakeDataAggregateType.Subcategory:
					Guard.ArgumentNonNegative(groupCount, "must be defined");
					column = column.ProductSubcategory(second, groupCount);
					break;
				case FakeDataAggregateType.Cartesian:
					column = column.ProductCartesian(second);
					break;
			}
		}
		public void AggregateSubcategoryIndependent(IEnumerable<DataItem> items, int subcategoryCount) {
			var subcategoryItems = items.Take(subcategoryCount);
			var independentItems = items.Skip(subcategoryCount);
			Func<int, int> getUniqueCount = x => x == 0 ? IndependentGroupCount : x;
			int uniqueCount = SubcategoryGroupCount;
			foreach (DataItem item in subcategoryItems) {
				Aggregate(item, uniqueCount, FakeDataAggregateType.Subcategory, SubcategoryGroupCount);
				uniqueCount = getUniqueCount(GetCurrentCount() * SubcategoryGroupCount);
			}
			uniqueCount = getUniqueCount(GetCurrentCount());
			foreach (DataItem item in independentItems)
				Aggregate(item, uniqueCount, FakeDataAggregateType.Independent);
		}
		public void DuplicateRows(int maxDuplicateCount) {
			if (column != null)
				column = column.DuplicateRows(1, maxDuplicateCount);
		}
		public Dictionary<string, IList<object>> GetValues() {
			Dictionary<string, IList<object>> dict = new Dictionary<string, IList<object>>();
			if (column != null) {
				Action<ValuesContainer, bool> addRow = (row, needAdd) => {
					foreach (string name in row.FieldNames) {
						if (needAdd)
							dict.Add(name, new List<object>());
						dict[name].Add(row[name]);
					}
				};
				bool isFirst = true;
				foreach (ValuesContainer row in column.Rows) {
					addRow(row, isFirst);
					isFirst = false;
				}
			}
			return dict;
		}
		public bool CanAggregateField(string fieldName) {
			if (string.IsNullOrEmpty(fieldName))
				return false;
			return fieldsTypeMap.ContainsField(fieldName) && (column == null || !column.Rows.Any(x => x.Contains(fieldName)));
		}
		ValuesColumn CreateValuesColumn(string fieldName, int uniqueCount) {
			FakeDataGeneratorBase generator;
			List<Type> numericFloatTypes = new List<Type>() { typeof(double), typeof(decimal), typeof(float) };
			if (NumericPercentMode && numericFloatTypes.Contains(fieldsTypeMap[fieldName]))
				generator = new RangeDataGenerator(0, 1, uniqueCount, fieldsTypeMap[fieldName], fieldName);
			else
				generator = FakeDataGeneratorBase.MakeGenerator(fieldsTypeMap[fieldName], uniqueCount, fieldName);
			generator.SpreadParam = numericSpread;
			generator.ShuffleParam = shuffleParam;
			shuffleParam++;
			if (EnableSpread)
				numericSpread += 1; 
			return new ValuesColumnWithGenerator(generator);
		}
		int GetCurrentCount() {
			if (column == null)
				return 0;
			else
				return column.Rows.Count();
		}
		bool IsSkipOperation(ValuesColumn c) {
			if (column == null) {
				column = c;
				return true;
			}
			return false;
		}
	}
}
