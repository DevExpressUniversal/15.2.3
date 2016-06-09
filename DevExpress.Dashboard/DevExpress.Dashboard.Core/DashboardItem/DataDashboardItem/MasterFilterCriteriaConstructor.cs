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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
namespace DevExpress.DashboardCommon.Native {
	class MasterFilterCriteriaGenerator {
		static object PatchToDay(object obj) {
			if(obj != null) {
				DateTime value = (DateTime)obj;
				return new DateTime(value.Year, value.Month, value.Day);
			} else
				return obj;
		}
		internal static CriteriaOperator CreateEquals(string property, object value) {
			if(ReferenceEquals(null, value))
				return new UnaryOperator(UnaryOperatorType.IsNull, property);
			else
				return new BinaryOperator(property, value, BinaryOperatorType.Equal);
		}
		readonly IDataSourceSchema dataSchema;
		readonly Func<Dimension, string> getDimensionNameFunc;
		public MasterFilterCriteriaGenerator(IDataSourceSchema dataSchema, Func<Dimension, string> getDimensionNameFunc) {
			this.dataSchema = dataSchema;
			this.getDimensionNameFunc = getDimensionNameFunc;
		}
		public CriteriaOperator GetParametersCriteria(IMasterFilterParameters parameters) {
			CriteriaOperator resultCriteria = CriteriaOperator.Parse(String.Empty);
			if (parameters.EmptyCriteria)
				return resultCriteria;
			if(parameters.IsExcludingAllFilter)
				return Helper.GetExcludingAllFilterCriteria();
			if(parameters.Ranges != null) {
				Dictionary<Dimension, MasterFilterRange> ranges = parameters.Ranges;
				foreach(Dimension dimension in ranges.Keys) {
					if(dataSchema != null && dataSchema.ContainsField(dimension.DataMember)) {
						object rangeLeft = ranges[dimension].Left;
						object rangeRight = ranges[dimension].Right;
						string rightSign = DiscreteScaleEmulator.Emulate ? "<" : "<=";
						string dimensionFilterName = getDimensionNameFunc(dimension);
						if(dimension.DateTimeGroupInterval == DateTimeGroupInterval.MonthYear || 
							dimension.DateTimeGroupInterval == DateTimeGroupInterval.QuarterYear ||
							dimension.DateTimeGroupInterval == DateTimeGroupInterval.DayMonthYear) {
							rangeLeft = PatchToDay(rangeLeft);
							rangeRight = PatchToDay(rangeRight);
						}
						resultCriteria = CriteriaOperator.And(resultCriteria,
							rangeLeft == null ? null : CriteriaOperator.Parse(String.Format("[{0}] >= ?", dimensionFilterName), rangeLeft),
							rangeRight == null ? null : CriteriaOperator.Parse(String.Format("[{0}] <= ?", dimensionFilterName), rangeRight));
					}
				}
			}
			if(parameters.Values != null) {
				DimensionValueSet masterFilterValues = parameters.Values;
				IEnumerable<Dimension> masterFilterDimensions = masterFilterValues.Dimensions.Where(dimension =>
					dataSchema != null && dataSchema.ContainsField(dimension.DataMember));
				MasterFilterNode rootNode = new MasterFilterNode(null);
				MasterFilterNode currentNode = rootNode;
				for(int i = 0; i < masterFilterValues.Count; i++) {
					foreach(Dimension dimension in masterFilterDimensions) {
						object value = DashboardSpecialValuesInternal.FromSpecialValue(masterFilterValues.GetValue(dimension, i));
						if(DashboardSpecialValues.IsOlapNullValue(value))
							break;
						currentNode = currentNode.GetChildNode(value);
					}
					currentNode = rootNode;
				}
				resultCriteria = CriteriaOperator.And(resultCriteria, GenerateCriteria(rootNode, masterFilterDimensions.ToList(), 0));
			}
			return resultCriteria;
		}
		CriteriaOperator GenerateCriteria(MasterFilterNode node, IList<Dimension> masterFilterDimensions, int currentIndex) {
			CriteriaOperator criteria = CriteriaOperator.Parse(String.Empty);
			int newIndex = currentIndex + 1;
			if(currentIndex == masterFilterDimensions.Count - 1)
				criteria = GenerateLastLevelCriteria(node, getDimensionNameFunc(masterFilterDimensions[currentIndex]));
			else
				foreach(MasterFilterNode childNode in node.ChildNodes)
					criteria = CriteriaOperator.Or(criteria, GenerateCriteria(childNode, masterFilterDimensions, newIndex));
			if(currentIndex > 0)
				criteria = CriteriaOperator.And(CreateEquals(getDimensionNameFunc(masterFilterDimensions[currentIndex - 1]), node.Value), criteria);
			return criteria;
		}
		CriteriaOperator GenerateLastLevelCriteria(MasterFilterNode node, string propertyName) {
			List<object> values = new List<object>();
			bool haveNull = false;
			foreach(MasterFilterNode childNode in node.ChildNodes) {
				if(object.ReferenceEquals(null, childNode.Value))
					haveNull = true;
				else
					values.Add(childNode.Value);
			}
			CriteriaOperator op = null;
			if(values.Count == 1)
				op = CreateEquals(propertyName, values[0]);
			else
				if(values.Count > 1)
					op = new InOperator(propertyName, values);
			if(haveNull)
				op = CriteriaOperator.Or(op, CreateEquals(propertyName, null));
			return op;
		}
	}
}
