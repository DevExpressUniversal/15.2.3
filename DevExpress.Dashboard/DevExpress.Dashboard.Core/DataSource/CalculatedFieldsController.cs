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
using System.Xml.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.DataAccess;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.PivotGrid.ServerMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon.Native {
	public class CalculatedFieldsController : ICalculatedFieldsController, IDisposable {
		public static bool PrepareCalculatedField(IDashboardDataSource dataSource, string dataSetName, string dataMember, PivotGridFieldBase pivotField, IActualParametersProvider provider) {
			if(dataSource == null || dataSource.CalculatedFields == null)
				return false;
			CalculatedField field = dataSource.CalculatedFields.FindFirst((f) => f.Name == dataMember);
			if(field == null)
				return false;
			string expression = dataSource.GetExpandedCalculatedFieldExpression(field, dataSetName, provider, true);
			bool mixedType = false;
			if(!dataSource.IsSqlServerMode(dataSetName) && DevExpress.PivotGrid.CriteriaVisitors.HasAggregateCriteriaChecker.Check(CriteriaOperator.Parse(expression)))
				mixedType = true;
			pivotField.DrillDownColumnName = pivotField.FieldName;
			pivotField.FieldName = string.Empty;
			pivotField.UnboundType = field.DataType.ToUnboundColumnType();
			pivotField.UnboundExpression = expression;
			pivotField.UnboundExpressionMode = mixedType ? UnboundExpressionMode.UseAggregateFunctions : UnboundExpressionMode.DataSource;
			return true;
		}
		public const string ParameterFormatString = "Parameters.{0}";
		public const string ParameterExpressionFormatString = "[" + ParameterFormatString + "]";
		const string xmlCalculatedFields = "CalculatedFields";
		const string xmlCalculatedField = "CalculatedField";
		readonly CalculatedFieldCollection calculatedFields = new CalculatedFieldCollection();
		readonly Dictionary<CalculatedField, bool> isCalculatedFieldValidCache = new Dictionary<CalculatedField, bool>();
		readonly NullableDictionary<string, NullableDictionary<string, Type>> calculatedFieldTypeCache = new NullableDictionary<string, NullableDictionary<string, Type>>();
		readonly CollectionPrefixNameGenerator<CalculatedField> calculatedFieldNameGenerator;
		readonly IDashboardDataSource dataSource;
		public event EventHandler<CalculatedFieldChangedEventArgs> CalculatedFieldChanged;
		public event EventHandler<NotifyingCollectionChangedEventArgs<CalculatedField>> CalculatedFieldsCorrupted;
		public event EventHandler<NotifyingCollectionChangedEventArgs<CalculatedField>> CalculatedFieldCollectionChanged;
		internal event EventHandler<EventArgs> ConstructTree;
		internal event EventHandler<EventArgs> CalculatedFieldsChanged;
		public CalculatedFieldCollection CalculatedFields { get { return calculatedFields; } }
		IDashboardDataSource ICalculatedFieldsController.DataSource { get { return dataSource; } }
		static CalculatedFieldsController() {
			CriteriaOperator.RegisterCustomFunctions(new ICustomFunctionOperator[] {
					new DashboardDistinctCountFunction(),
					new FunctionOperatorAddFunction(FunctionOperatorTypeAdd.GetQuarter),
					new FunctionOperatorAddFunction(FunctionOperatorTypeAdd.GetDateMonthYear),
					new FunctionOperatorAddFunction(FunctionOperatorTypeAdd.GetDateQuarterYear),
					new FunctionOperatorAddFunction(FunctionOperatorTypeAdd.GetDateHour),
					new FunctionOperatorAddFunction(FunctionOperatorTypeAdd.GetDateHourMinute),
					new FunctionOperatorAddFunction(FunctionOperatorTypeAdd.GetDateHourMinuteSecond),
					new FunctionOperatorAddFunction(FunctionOperatorTypeAdd.GetWeekOfYear)
				});
		}
		public CalculatedFieldsController(IDashboardDataSource dataSource) {
			calculatedFieldNameGenerator = new CollectionPrefixNameGenerator<CalculatedField>(calculatedFields);
			calculatedFields.CollectionChanged += CalculatedFieldsCollectionChanged;
			this.dataSource = dataSource;
		}
		public void Dispose() {
			calculatedFields.Clear();
			calculatedFieldNameGenerator.Dispose();
		}
		public CriteriaOperator GetExpandedCalculatedFieldExpressionOperator(string expression, string calculatedFieldName, bool throwOnError, IActualParametersProvider provider, bool actualValues) {
			if(string.IsNullOrEmpty(expression))
				return null;
			CriteriaOperator criteria;
			try {
				criteria = CriteriaOperator.Parse(expression);
			} catch {
				if(throwOnError)
					throw;
				else
					return null;
			}
			if(ReferenceEquals(criteria, null))
				return criteria;
			if(calculatedFields != null && calculatedFields.Count() > 0)
				criteria = new CalculatedFieldsExpressionExpander(calculatedFields, calculatedFieldName, throwOnError).Process(criteria);
			IEnumerable<IParameter> parameters = actualValues ? provider.GetActualParameters() : provider.GetParameters();
			if(ReferenceEquals(criteria, null) || parameters == null)
				return criteria;
			List<IParameter> parametersList = parameters.ToList();
			if(parametersList.Count == 0)
				return criteria;
			return new ParametersToValuesCriteriaPatcher(parametersList, false).Process(criteria);
		}
		public string GetExpandedCalculatedFieldExpression(CalculatedField field, IActualParametersProvider provider, bool actualValues) {
			CriteriaOperator criteria = GetExpandedCalculatedFieldExpressionOperator(field.Expression, field.Name, false, provider, actualValues);
			return ReferenceEquals(criteria, null) ? string.Empty : criteria.ToString();
		}
		public void LoadFromXml(XElement xElement) {
			CalculatedFields.Clear();
			XElement calculatedFieldElement = xElement.Element(xmlCalculatedFields);
			if(calculatedFieldElement != null) {
				foreach(XElement field in calculatedFieldElement.Elements()) {
					CalculatedField newField = new CalculatedField();
					newField.LoadFromXml(field);
					calculatedFields.Add(newField);
				}
			}
		}
		public void SaveToXml(XElement element) {
			if(calculatedFields.Count > 0) {
				XElement calculatedFieldElement = new XElement(xmlCalculatedFields);
				foreach(CalculatedField field in CalculatedFields) {
					XElement fieldElement = new XElement(xmlCalculatedField);
					field.SaveToXml(fieldElement);
					calculatedFieldElement.Add(fieldElement);
				}
				element.Add(calculatedFieldElement);
			}
		}
		public bool IsCalculatedFieldNameValid(string name) {
			IEnumerable<string> dataSets = dataSource.GetDataSets();
			bool hasCoincidence = false;
			if(dataSets.Count() > 0) {
				foreach(string dataSet in dataSets) {
					hasCoincidence = dataSource.GetField(name, dataSet) != null;
					if(hasCoincidence)
						break;
				}
			} else
				hasCoincidence = dataSource.GetField(name, string.Empty) != null;
			return calculatedFieldNameGenerator.ValidateName(name) == null && !hasCoincidence;
		}
		void CalculatedFieldsCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<CalculatedField> e) {
			foreach(CalculatedField field in e.AddedItems)
				field.Changed += OnCalculatedFieldChanged;
			foreach(CalculatedField field in e.RemovedItems) {
				field.Changed -= OnCalculatedFieldChanged;
				isCalculatedFieldValidCache.Remove(field);
			}
			RaiseConstructTree();
			RaiseCalculatedFieldsChanged();
			if(CalculatedFieldCollectionChanged != null)
				CalculatedFieldCollectionChanged(this, e);
		}
		void OnCalculatedFieldChanged(object sender, CalculatedFieldChangedEventArgs e) {
			if(e.PropertyName == "Expression" || e.PropertyName == "DataType") {
				isCalculatedFieldValidCache.Remove(e.Field);
			}
			RaiseConstructTree();
			RaiseCalculatedFieldsChanged();
			if(CalculatedFieldChanged != null)
				CalculatedFieldChanged(this, e);
		}
		void RaiseConstructTree() {
			if(ConstructTree != null)
				ConstructTree(this, EventArgs.Empty);
		}
		public void OnCalculatedFieldParametersChanged(IList<CalculatedField> fields) {
			foreach(CalculatedField field in fields)
				isCalculatedFieldValidCache.Remove(field);
			RaiseCalculatedFieldsChanged();
			if(CalculatedFieldsCorrupted != null)
				CalculatedFieldsCorrupted(this, new NotifyingCollectionChangedEventArgs<CalculatedField>(fields, null));
		}
		void RaiseCalculatedFieldsChanged() {
			if(CalculatedFieldsChanged != null)
				CalculatedFieldsChanged(this, EventArgs.Empty);
		}
		public void SetCalculatedFieldIsEvaluable(CalculatedField field, bool value) {
			isCalculatedFieldValidCache[field] = value;
		}
		public List<CalculatedField> GetFailingCalculatedFields(IActualParametersProvider provider) {
			List<CalculatedField> failedFields = new List<CalculatedField>();
			foreach(CalculatedField field in CalculatedFields) {
				if(!CheckIsCalculatedFieldValid(field, provider))
					failedFields.Add(field);
			}
			return failedFields;
		}
		public bool CheckIsCalculatedFieldValid(CalculatedField field, IActualParametersProvider provider) {
			bool isValid;
			if(GetIsCalculatedFieldValidFromCache(field, out isValid))
				return isValid;
			bool check = true;
			CriteriaOperator criteria = null;
			try {
				criteria = GetExpandedCalculatedFieldExpressionOperator(field.Expression, field.Name, true, provider, false);
			} catch {
				isValid = false;
				check = false;
			}
			if(check)
				isValid = IsValid(dataSource.GetListSource(field.DataMember), dataSource.GetPivotDataSource(field.DataMember), criteria, field.DataType.ToUnboundColumnType());
			SetCalculatedFieldIsEvaluable(field, isValid);
			return isValid;
		}
		public bool GetIsCalculatedFieldValidFromCache(CalculatedField field, out bool isEvaluable) {
			isEvaluable = false;
			if(isCalculatedFieldValidCache.TryGetValue(field, out isEvaluable))
				return true;
			return false;
		}
		public Type GetCalculatedFieldDesiredType(DataNode root, string expression, string dataMember, string name, bool useCache, IActualParametersProvider provider) {
			Type type = null;
			if(useCache) {
				NullableDictionary<string, Type> c1;
				if(calculatedFieldTypeCache.TryGetValue(dataMember, out c1)) {
					if(c1.TryGetValue(expression, out type))
						return type;
				}
			}
			type = GetCalculatedFieldDesiredTypeCore(root, expression, dataMember, name, provider);
			if(type != null) {
				NullableDictionary<string, Type> c1;
				if(!calculatedFieldTypeCache.TryGetValue(dataMember, out c1)) {
					c1 = new NullableDictionary<string, Type>();
					calculatedFieldTypeCache.Add(dataMember, c1);
				}
				c1[expression] = type;
			}
			return type;
		}
		public void ClearCache() {
			isCalculatedFieldValidCache.Clear();
			calculatedFieldTypeCache.Clear();
		}
		Type GetCalculatedFieldDesiredTypeCore(DataNode root, string expression, string dataMember, string name, IActualParametersProvider provider) {
			Type type = null;
			Func<PropertyDescriptorCollection> properties = () => new PropertyDescriptorCollection(GetPropertiesForValidation(root, provider).ToArray());
			CriteriaOperator criteria = GetExpandedCalculatedFieldExpressionOperator(expression, name, true, provider, false);
			if(!ReferenceEquals(criteria, null)) {
				if(DevExpress.PivotGrid.CriteriaVisitors.HasAggregateCriteriaChecker.Check(criteria)) {
					return new DevExpress.PivotGrid.CriteriaVisitors.SummaryLevelCriteriaValidator().Validate(criteria, properties());
				} else {
					type = CriteriaCompiler.ToLambda(criteria, new BaseRowStub.DataControllerCriteriaCompilerDescriptor(properties)).ReturnType;
					if(!dataSource.IsSqlServerMode(dataMember))
						return type;
				}
				type = dataSource.ServerGetUnboundExpressionType(criteria.ToString(), dataMember);
				if(type == null)
					throw new CalculatedFieldExpressionEditorException("The expression cannot be executed in server mode.");
				else
					return type;
			} else
				throw new ArgumentException("The specified criteria is empty.");
		}
		IEnumerable<PropertyDescriptor> GetPropertiesForValidation(DataNode root, IActualParametersProvider provider) {
			if(root != null)
				foreach(DataField dataField in root.GetAllDataFields())
					yield return new ExpressionValidationPropertyDescriptor(dataField.DataMember, dataField.SourceType);
			foreach(CalculatedField calculatedField in calculatedFields)
				yield return new ExpressionValidationPropertyDescriptor(calculatedField.Name, calculatedField.DataType.ToType());
			foreach(IParameter parameter in provider.GetParameters())
				yield return new ExpressionValidationPropertyDescriptor(string.Format(CalculatedFieldsController.ParameterFormatString, parameter.Name), parameter.Type);
		}
		bool IsValid(IList listSource, IPivotGridDataSource pivotSource, CriteriaOperator criteria, UnboundColumnType type) {
			if(pivotSource != null) {
				PivotGridFieldBase pivotField = new PivotGridFieldBase();
				pivotField.UnboundType = type;
				pivotField.UnboundExpression = CriteriaOperator.ToString(criteria);
				pivotField.UnboundExpressionMode = UnboundExpressionMode.DataSource;
				return pivotSource.IsUnboundExpressionValid(pivotField);
			} else {
				if(listSource != null) {
					ListSourceDataController dataController = new ListSourceDataController();
					dataController.ListSource = listSource;
					dataController.PopulateColumns();
					if(dataController.Columns.Count == 0)
						return true;
					try {
						if(HasAggregateCriteriaChecker.Check(criteria))
							new DevExpress.PivotGrid.CriteriaVisitors.SummaryLevelCriteriaValidator().Validate(criteria, dataController.Helper.DescriptorCollection);
						else
							dataController.ValidateExpression(criteria);
					} catch {
						return false;
					}
					return true;
				} else
					return true;
			}
		}
	}
	public class CalculatedFieldExpressionEditorException : Exception {
		public CalculatedFieldExpressionEditorException(string message)
			: base(message) { }
	}
}
