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
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class SliceDataQueryValidator {
		readonly SliceDataQuery query;
		readonly Stack<string> inValidate = new Stack<string>();
		readonly HashSet<string> fieldList = new HashSet<string>();
		string stateText;
		public SliceDataQueryValidator(SliceDataQuery query, DataSourceModel dataSource) {
			this.query = query;
			try {
				IStorage storage = dataSource.Storage;
				IPivotGridDataSource pivotSource = dataSource.PivotDataSource;
				if(storage != null)
					fieldList = new HashSet<string>(storage.Columns);
				else if(pivotSource != null)
					fieldList = new HashSet<string>(pivotSource.GetFieldList());
#if !DXPORTABLE
				else if(dataSource.ListSource is ITypedList)
					fieldList = new HashSet<string>(((ITypedList)dataSource.ListSource).GetItemProperties(null).Cast<PropertyDescriptor>().Select(pd => pd.Name));
#endif
				else
					fieldList = new HashSet<string>();
			} catch {
				fieldList = new HashSet<string>();
			}
		}
		public bool Validate() {
			return ValidateCore() == DataQueryValidationState.Success;
		}
		DataQueryValidationState ValidateCore() {
			if(query == null || query.DataSlices.Count == 0 || fieldList.Count == 0)
				return DataQueryValidationState.NothingToCalculate;
			foreach(SliceModel slice in query.DataSlices) {
				DataQueryValidationState state = ValidateCore(slice);
				if(state != DataQueryValidationState.Success)
					return state;
			}
			return DataQueryValidationState.Success;
		}
		DataQueryValidationState ValidateCore(SliceModel slice) {
			List<DimensionModel> allDimensions = slice.FilterDimensions.Concat(slice.Dimensions).Concat(slice.SummaryAggregations.Where(s => s.Dimension != null).Select(s => s.Dimension)).ToList();
			List<MeasureModel> allMeasures = slice.Measures.Concat(slice.SummaryAggregations.Where(s => s.Measure != null).Select(s => s.Measure)).Distinct().ToList();
			DataQueryValidationState state = ValidateDataItemModels(allDimensions, false);
			if(state != DataQueryValidationState.Success)
				return state;
			state = ValidateDataItemModels(allMeasures, null);
			if(state != DataQueryValidationState.Success)
				return state;
			if(!ReferenceEquals(null, slice.FilterCriteria)) {
				state = ValidateDataSourceExpression(CriteriaOperator.ToString(new CriteriaParameterReplacer().Process(slice.FilterCriteria)), "", allDimensions);
				if(state != DataQueryValidationState.Success)
					return state;
			}
			return DataQueryValidationState.Success;
		}
		DataQueryValidationState ValidateDataItemModels<TDataItemModel>(List<TDataItemModel> dataItemModels, bool? isSummaryOnly) where TDataItemModel : DataItemModel<TDataItemModel> {
			foreach(TDataItemModel dimension in dataItemModels) {
				DataQueryValidationState state = ValidateDataItemModel(dimension, isSummaryOnly, dataItemModels);
				if(state != DataQueryValidationState.Success) {
					if(state == DataQueryValidationState.NoSuchDataMember) {
						try {
							throw new SliceDataQueryValidationNoSuchDataMemberException(stateText);
						} catch { }
					}
					return state;
				}
			}
			return DataQueryValidationState.Success;
		}
		DataQueryValidationState ValidateDataItemModel<TDataItemModel>(TDataItemModel dataItemModel, bool? isSummaryOnly, List<TDataItemModel> allItems) where TDataItemModel : DataItemModel<TDataItemModel> {
			if(string.IsNullOrEmpty(dataItemModel.UnboundExpression)) {
				if(!TryResolveDataMember(dataItemModel.DataMember)) {
					return DataQueryValidationState.NoSuchDataMember;					
				}
			} else {
				DataQueryValidationState state;
				switch(dataItemModel.UnboundMode) {
					case ExpressionMode.DataSourceLevel:
						state = ValidateDataSourceExpression(CriteriaOperator.Parse(dataItemModel.UnboundExpression), dataItemModel.Name, allItems);
						if(state != DataQueryValidationState.Success)
							return state;
						break;
					case ExpressionMode.SummaryLevel:
					case ExpressionMode.AggregateFunction:
						if(isSummaryOnly == false)
							ThrowInvalidUnboundLevel<TDataItemModel>(dataItemModel);
						state = ValidateSummaryExpression(dataItemModel.UnboundExpression, dataItemModel.Name, allItems);
						if(state != DataQueryValidationState.Success)
							return state;
						break;
					default:
						throw new NotSupportedException(dataItemModel.UnboundMode.ToString());
				}
			}
			return DataQueryValidationState.Success;
		}
		DataQueryValidationState ValidateSummaryExpression<TDataItemModel>(string expression, string name, List<TDataItemModel> allItems) where TDataItemModel : DataItemModel<TDataItemModel> {
			if(inValidate.Contains(name)) {
				stateText = string.Join(", ", inValidate);
				return DataQueryValidationState.Recursive;
			}
			inValidate.Push(name);
			SummaryLevelCriteriaValidator validator = new SummaryLevelCriteriaValidator();
			CriteriaOperator.Parse(expression).Accept(validator);
			foreach(var a in validator.SummaryLevel) {
				DataQueryValidationState state = ValidateDataSourceExpression(a.Key, a.Value.DataSourceLevelName, allItems);
				if(state != DataQueryValidationState.Success)
					return state;
			}
			foreach(string sname in validator.UsedNames) {
				DataQueryValidationState state = ValidateDataItemModel(allItems.Where(item => item.Name == sname).Single(), true, allItems);
				if(state != DataQueryValidationState.Success)
					return state;
			}
			inValidate.Pop();
			return DataQueryValidationState.Success;
		}
		public void ValidateSummaryExpressionByName(string name) {
			throw new NotSupportedException();
		}
		DataQueryValidationState ValidateDataSourceExpression<TDataItemModel>(CriteriaOperator expression, string name, List<TDataItemModel> allItems) where TDataItemModel : DataItemModel<TDataItemModel> {
			if(inValidate.Contains(name)) {
				stateText = string.Join(", ", inValidate);
				return DataQueryValidationState.Recursive;
			}
			inValidate.Push(name);
			ColumnNamesCriteriaVisitor visitor = new ColumnNamesCriteriaVisitor(false);
			expression.Accept(visitor);
			foreach(string newField in visitor.ColumnNames) {
				TDataItemModel dataItemModel = allItems.Where(item => item.Name == newField).FirstOrDefault();
				if(dataItemModel == null) {
					if(!TryResolveDataMember(newField)) {
						stateText = name + " (" + newField + ")";
						return DataQueryValidationState.NoSuchDataMember;
					}
				} else {
					if(dataItemModel.UnboundMode == ExpressionMode.AggregateFunction || dataItemModel.UnboundMode == ExpressionMode.SummaryLevel)
						ThrowInvalidUnboundLevel<TDataItemModel>(dataItemModel);
					DataQueryValidationState state = ValidateDataItemModel(dataItemModel, false, allItems);
					if(state != DataQueryValidationState.Success)
						return state;
				}
			}
			inValidate.Pop();
			return DataQueryValidationState.Success;
		}
		void ThrowInvalidUnboundLevel<TDataItemModel>(TDataItemModel dataItemModel) where TDataItemModel : DataItemModel<TDataItemModel> {
			throw new ArgumentException("Invalid unbound mode: " + dataItemModel.Name);
		}
		bool TryResolveDataMember(string dataMember) {
			return fieldList.Contains(dataMember);
		}
		class SummaryLevelCriteriaValidator : MixedSummaryLevelCriteriaVisitor, IUniqueFieldNameGenerator {
			int counter;
			readonly List<string> usedNames = new List<string>();
			public List<string> UsedNames { get { return usedNames; } }
			public SummaryLevelCriteriaValidator()
				: base(null) {
				SetUniqueFieldNameGenerator(this);
			}
			string IUniqueFieldNameGenerator.Generate(string prefix) {
				return prefix + counter++;
			}
			public override CriteriaOperator Visit(OperandProperty theOperand) {
				usedNames.Add(theOperand.PropertyName);
				return base.Visit(theOperand);
			}
		}
	}
	public class SliceDataQueryValidationNoSuchDataMemberException : Exception {
		public SliceDataQueryValidationNoSuchDataMemberException(string message)
			: base(message) {
		}
	}
	public enum DataQueryValidationState {
		Success,
		NothingToCalculate,
		NoSuchDataMember,
		Recursive,
	}
}
