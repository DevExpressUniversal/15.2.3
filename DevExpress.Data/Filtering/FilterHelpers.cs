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
using System.Collections;
using System.Text;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Helpers;
using System.Linq;
using DevExpress.Data.Summary;
using System.Threading;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data.Filtering {
	public enum FilterCondition {
		Contains,
		StartsWith,
		Like,
		Default,
		Equals
	}
	public interface IFilteredComponentBase {
		event EventHandler RowFilterChanged;
		event EventHandler PropertiesChanged;
		CriteriaOperator RowCriteria { get; set; }
	}
	public static class DxFtsContainsHelperAlt {
		public static CriteriaOperator Create(FindSearchParserResults parseResult, FilterCondition defaultCondition, bool isServerMode) {
			CriteriaOperator rv = null;
			rv = CreateFilter(parseResult.SearchTexts, parseResult.ColumnNames, defaultCondition, isServerMode);
			foreach(FindSearchField f in parseResult.Fields) {
				CriteriaOperator columnFilter = null;
				columnFilter = CreateFilter(f.Values, new FindColumnInfo[] { f.Column } , defaultCondition, isServerMode);
				rv &= columnFilter;
			}
			return rv;
		}
		static CriteriaOperator CreateFilter(string[] values, FindColumnInfo[] properties, FilterCondition filterCondition, bool isServerMode) {
			CriteriaOperator stAnd = null, stOr = null;
			foreach(string stext in values) {
				if(stext.StartsWith("+")) {
					stAnd &= DoFilterCondition(stext.Substring(1), properties, filterCondition, isServerMode);
					continue;
				}
				if(stext.StartsWith("-")) {
					stAnd &= !DoFilterCondition(stext.Substring(1), properties, filterCondition, isServerMode);
					continue;
				}
				stOr |= DoFilterCondition(stext, properties, filterCondition, isServerMode);
			}
			return stAnd & stOr;
		}
		static CriteriaOperator DoFilterCondition(string originalValue, FindColumnInfo[] columns, FilterCondition defaultCondition, bool isServerMode) {
			CriteriaOperator rv = null;
			CriteriaOperator op;
			foreach(FindColumnInfo column in columns) {
				FilterCondition filterCondition = defaultCondition;
				object value = originalValue;
				if(isServerMode) {
					if(!AllowColumn(column, ref value, ref filterCondition)) continue;
				}
				OperandProperty property = new OperandProperty(column.PropertyName);
				switch(filterCondition) {
					case FilterCondition.StartsWith:
						op = new FunctionOperator(FunctionOperatorType.StartsWith, property, new OperandValue(value));
						break;
					case FilterCondition.Contains:
						op = new FunctionOperator(FunctionOperatorType.Contains, property, new OperandValue(value));
						break;
					case FilterCondition.Equals:
						op = FilterHelper.CalcColumnFilterCriteriaByValue(column.PropertyName, column.Column.FieldType, value, true, CultureInfo.CurrentCulture);
						break;
					default:
						op = value.ToString().Contains("%") ?
							(CriteriaOperator)LikeCustomFunction.Create(property, new OperandValue(value)) :
							new FunctionOperator(FunctionOperatorType.Contains, property, new OperandValue(value));
						break;
				}
				rv |= op;
			}
			return rv;
		}
		static bool AllowColumn(FindColumnInfo column, ref object value, ref FilterCondition filterCondition) {
			if(column.Column == null) return false;
			Type type = column.Column.FieldType;
			if(Nullable.GetUnderlyingType(type) != null) type = Nullable.GetUnderlyingType(type);
			string val = value == null ? null : value.ToString();
			if(type.IsEnum()) return false; 
			if(SummaryItemTypeHelper.IsNumericalType(type)) {
				filterCondition = FilterCondition.Equals;
				object numVal;
				try {
#if !SL
					numVal = Convert.ChangeType(val, type);
#else
					numVal = Convert.ChangeType(val, type, CultureInfo.CurrentCulture);
#endif
				}
				catch {
					return false;
				}
				value = numVal;
				return true;
			}
			if(SummaryItemTypeHelper.IsDateTime(type)) {
				filterCondition = FilterCondition.Equals;
				object date;
				try {
#if !SL
					date = Convert.ChangeType(val, type);
#else
					date = Convert.ChangeType(val, type, CultureInfo.CurrentCulture);
#endif
				}
				catch {
					return false;
				}
				value = date;
				return true;
			}
			if(SummaryItemTypeHelper.IsBool(type)) {
				filterCondition = FilterCondition.Equals;
				bool res;
				if(!bool.TryParse(val, out res)) return false;
				value = res;
				return true;
			} 
#if SL
			if(type.Equals(typeof(TimeSpan)) || type.Equals(typeof(TimeSpan?))) {
				filterCondition = FilterCondition.Equals;
				TimeSpan res;
				if(!TimeSpan.TryParse(val, out res)) return false;
				value = res;
				return true;
			}
#endif
			return true;
		}
	}
	public static class DxFtsContainsHelper {
		public const string DxFtsPropertyPrefix = DevExpress.Data.Access.DisplayTextPropertyDescriptor.DxFtsPropertyPrefix;
		public const string DxFtsContainsCustomFunctionName = "DxFtsContains";
		public const string DxFtsLikeCustomFunctionName = "DxFtsLike";
		public static CriteriaOperator Expand(CriteriaOperator criteria, CriteriaOperator[] columns) {
			return DxFtsContainsProcessor.Convert(criteria, columns);
		}
		public static CriteriaOperator BuildContains(CriteriaOperator value) {
			return new FunctionOperator(FunctionOperatorType.Custom, new OperandValue(DxFtsContainsCustomFunctionName), value);
		}
		public static CriteriaOperator BuildLike(CriteriaOperator value) {
			return new FunctionOperator(FunctionOperatorType.Custom, new OperandValue(DxFtsLikeCustomFunctionName), value);
		}
		public static CriteriaOperator Create(string[] columns, FindSearchParserResults parseResult) {
			return DxFtsContainsHelper.Create(columns, parseResult, FilterCondition.Contains);
		}
		public static CriteriaOperator Create(string[] columns, FindSearchParserResults parseResult, FilterCondition filterCondition) {
			CriteriaOperator rv = null;
			OperandProperty[] properties = new OperandProperty[columns.Length];
			for(int n = 0; n < columns.Length; n++) properties[n] = new OperandProperty(columns[n]);
			rv = CreateFilter(parseResult.SearchTexts, properties, filterCondition);
			foreach(FindSearchField f in parseResult.Fields) {
				CriteriaOperator columnFilter = null;
				CriteriaOperator[] prop = new CriteriaOperator[] { new OperandProperty(f.Name) };
				columnFilter = CreateFilter(f.Values, prop, filterCondition);
				rv &= columnFilter;
			}
			return rv;
		}
		static CriteriaOperator CreateFilter(string[] values, CriteriaOperator[] properties, FilterCondition filterCondition) {
			CriteriaOperator stAnd = null, stOr = null;
			foreach(string stext in values) {
				if(stext.StartsWith("+")) {
					stAnd &= DoFilterCondition(new OperandValue(stext.Substring(1)), properties, filterCondition);
					continue;
				}
				if(stext.StartsWith("-")) {
					stAnd &= !DoFilterCondition(new OperandValue(stext.Substring(1)), properties, filterCondition);
					continue;
				}
				stOr |= DoFilterCondition(new OperandValue(stext), properties, filterCondition);
			}
			return stAnd & stOr;
		}
		static CriteriaOperator DoFilterCondition(CriteriaOperator value, CriteriaOperator[] columns, FilterCondition filterCondition) {
			CriteriaOperator rv = null;
			CriteriaOperator op;
			foreach(CriteriaOperator column in columns) {
				switch(filterCondition) {
					case FilterCondition.StartsWith:
						op = new FunctionOperator(FunctionOperatorType.StartsWith, column, value);
						break;
					case FilterCondition.Contains:
						op = new FunctionOperator(FunctionOperatorType.Contains, column, value);
						break;
					case FilterCondition.Equals:
						op = new BinaryOperator(column, value, BinaryOperatorType.Equal);
						break;
					default:
						op = value.ToString().Contains("%") ?
							(CriteriaOperator)LikeCustomFunction.Create(column, value) :
							new FunctionOperator(FunctionOperatorType.Contains, column, value);
						break;
				}
				rv |= op;
			}
			return rv;
		}
	}
	class DxFtsContainsProcessor: IClientCriteriaVisitor<CriteriaOperator> {
		readonly CriteriaOperator[] Columns;
		public DxFtsContainsProcessor(params CriteriaOperator[] columns) {
			if(columns == null)
				throw new ArgumentNullException("columns");
			if(columns.Length == 0)
				throw new ArgumentException("columns.Length == 0");
			this.Columns = columns;
		}
		public CriteriaOperator Visit(AggregateOperand theOperand) {
			CriteriaOperator aggregated = Process(theOperand.AggregatedExpression);
			CriteriaOperator condition = Process(theOperand.Condition);
			if(ReferenceEquals(null, aggregated) && ReferenceEquals(null, condition))
				return null;
			else
				return new AggregateOperand(theOperand.CollectionProperty, aggregated ?? theOperand.AggregatedExpression, theOperand.AggregateType, condition ?? theOperand.Condition);
		}
		public CriteriaOperator Visit(OperandProperty theOperand) {
			return null;
		}
		public CriteriaOperator Visit(JoinOperand theOperand) {
			CriteriaOperator aggregated = Process(theOperand.AggregatedExpression);
			CriteriaOperator condition = Process(theOperand.Condition);
			if(ReferenceEquals(null, aggregated) && ReferenceEquals(null, condition))
				return null;
			else
				return new JoinOperand(theOperand.JoinTypeName, condition ?? theOperand.Condition, theOperand.AggregateType, aggregated ?? theOperand.AggregatedExpression);
		}
		public CriteriaOperator Visit(BetweenOperator theOperator) {
			CriteriaOperator t = Process(theOperator.TestExpression);
			CriteriaOperator b = Process(theOperator.BeginExpression);
			CriteriaOperator e = Process(theOperator.EndExpression);
			if(ReferenceEquals(null, t) && ReferenceEquals(null, b) && ReferenceEquals(null, e))
				return null;
			else
				return new BetweenOperator(t ?? theOperator.TestExpression, b ?? theOperator.BeginExpression, e ?? theOperator.EndExpression);
		}
		public CriteriaOperator Visit(BinaryOperator theOperator) {
			CriteriaOperator l = Process(theOperator.LeftOperand);
			CriteriaOperator r = Process(theOperator.RightOperand);
			if(ReferenceEquals(null, l) && ReferenceEquals(null, r))
				return null;
			else
				return new BinaryOperator(l ?? theOperator.LeftOperand, r ?? theOperator.RightOperand, theOperator.OperatorType);
		}
		public CriteriaOperator Visit(UnaryOperator theOperator) {
			CriteriaOperator op = Process(theOperator.Operand);
			if(ReferenceEquals(null, op))
				return null;
			else
				return new UnaryOperator(theOperator.OperatorType, op);
		}
		public CriteriaOperator Visit(InOperator theOperator) {
			CriteriaOperator l = Process(theOperator.LeftOperand);
			IList<CriteriaOperator> ops = Process(theOperator.Operands);
			if(ReferenceEquals(null, l) && ReferenceEquals(null, ops))
				return null;
			else
				return new InOperator(l ?? theOperator.LeftOperand, ops ?? theOperator.Operands);
		}
		public CriteriaOperator Visit(GroupOperator theOperator) {
			IList<CriteriaOperator> ops = Process(theOperator.Operands);
			if(ops == null)
				return null;
			else
				return new GroupOperator(theOperator.OperatorType, ops);
		}
		public CriteriaOperator Visit(OperandValue theOperand) {
			return null;
		}
		public CriteriaOperator Visit(FunctionOperator theOperator) {
			if(theOperator.OperatorType == FunctionOperatorType.Custom) {
				if(theOperator.Operands.Count == 2) {
					OperandValue customFunctionName = theOperator.Operands[0] as OperandValue;
					if(!ReferenceEquals(null, customFunctionName)) {
						switch(customFunctionName.Value as string) {
							case DxFtsContainsHelper.DxFtsContainsCustomFunctionName:
								return DoContains(theOperator.Operands[1]);
							case DxFtsContainsHelper.DxFtsLikeCustomFunctionName:
								return DoLike(theOperator.Operands[1]);
						}
					}
				}
			}
			IList<CriteriaOperator> ops = Process(theOperator.Operands);
			if(ops == null)
				return theOperator;
			else
				return new FunctionOperator(theOperator.OperatorType, ops);
		}
		CriteriaOperator DoLike(CriteriaOperator value) {
			CriteriaOperator processed = Process(value) ?? value;
			CriteriaOperator rv = null;
			foreach(CriteriaOperator column in this.Columns) {
				rv |= LikeCustomFunction.Create(column, processed);
			}
			return rv;
		}
		CriteriaOperator DoContains(CriteriaOperator value) {
			CriteriaOperator processed = Process(value) ?? value;
			CriteriaOperator rv = null;
			foreach(CriteriaOperator column in this.Columns) {
				rv |= new FunctionOperator(FunctionOperatorType.Contains, column, processed);
			}
			return rv;
		}
		CriteriaOperator Process(CriteriaOperator op) {
			if(ReferenceEquals(null, op))
				return null;
			else
				return op.Accept(this);
		}
		IList<CriteriaOperator> Process(IEnumerable<CriteriaOperator> ops) {
			List<CriteriaOperator> results = new List<CriteriaOperator>();
			bool changes = false;
			foreach(CriteriaOperator op in ops) {
				CriteriaOperator processed = Process(op);
				if(!ReferenceEquals(null, processed)) {
					changes = true;
					results.Add(processed);
				}
				else {
					results.Add(op);
				}
			}
			if(changes)
				return results;
			else
				return null;
		}
		public static CriteriaOperator Convert(CriteriaOperator criteria, CriteriaOperator[] ftsColumns) {
			return new DxFtsContainsProcessor(ftsColumns).Process(criteria) ?? criteria;
		}
	}
#if !SL
	public class BindingListFilterProxyBase : IFilteredComponentBase {
		IBindingList dataSource = null;
		public BindingListFilterProxyBase(IBindingList dataSource) {
			this.dataSource = dataSource;
		}
		public IBindingList DataSource {
			get { return dataSource; }
		}
		void DS_ListChanged(object sender, ListChangedEventArgs e) {
			switch(e.ListChangedType) {
				case ListChangedType.Reset:
					OnFilterChanged();
					break;
				case ListChangedType.PropertyDescriptorAdded:
				case ListChangedType.PropertyDescriptorChanged:
				case ListChangedType.PropertyDescriptorDeleted:
					OnPropertiesChanged();
					break;
			}
		}
		void OnFilterChanged() {
			if(filterChanged != null)
				filterChanged(this, EventArgs.Empty);
		}
		void OnPropertiesChanged() {
			if(propertiesChanged != null)
				propertiesChanged(this, EventArgs.Empty);
		}
		EventHandler filterChanged, propertiesChanged;
		void BeforeAddEvent() {
			if(DataSource == null)
				return;
			if(filterChanged == null && propertiesChanged == null)
				DataSource.ListChanged += new ListChangedEventHandler(DS_ListChanged);
		}
		void AfterRemoveEvent() {
			if(DataSource == null)
				return;
			if(filterChanged == null && propertiesChanged == null)
				DataSource.ListChanged -= new ListChangedEventHandler(DS_ListChanged);
		}
		event EventHandler IFilteredComponentBase.RowFilterChanged {
			add {
				BeforeAddEvent();
				filterChanged += value;
			}
			remove {
				filterChanged -= value;
				AfterRemoveEvent();
			}
		}
		event EventHandler IFilteredComponentBase.PropertiesChanged {
			add {
				BeforeAddEvent();
				propertiesChanged += value;
			}
			remove {
				propertiesChanged -= value;
				AfterRemoveEvent();
			}
		}
		CriteriaOperator IFilteredComponentBase.RowCriteria {
			get {
				if(DataSource is IFilteredXtraBindingList) {
					return ((IFilteredXtraBindingList)DataSource).Filter;
				}
				else if(DataSource is IBindingListView) {
					return CriteriaOperator.TryParse(((IBindingListView)DataSource).Filter);
				}
				else
					return null;
			}
			set {
				if(DataSource is IFilteredXtraBindingList) {
					((IFilteredXtraBindingList)DataSource).Filter = CriteriaOperator.Clone(value);
				}
				else if(DataSource is IBindingListView) {
					((IBindingListView)DataSource).Filter = CriteriaToWhereClauseHelper.GetDataSetWhere(value);
				}
				else {
				}
			}
		}
	}
#endif
}
namespace DevExpress.Data.Filtering.Helpers {
	public class CollectFirstLevelPropertiesHelper: IClientCriteriaVisitor {
		public static IEnumerable<OperandProperty> Collect(CriteriaOperator opa) {
			if(ReferenceEquals(null, opa))
				return EmptyEnumerable<OperandProperty>.Instance;
			CollectFirstLevelPropertiesHelper helper = new CollectFirstLevelPropertiesHelper();
			helper.Process(opa);
			if(helper._Dictionary == null)
				return EmptyEnumerable<OperandProperty>.Instance;
			else
				return helper._Dictionary.Keys;
		}
		public static IEnumerable<OperandProperty> Collect(IEnumerable<CriteriaOperator> ops) {
			CollectFirstLevelPropertiesHelper helper = new CollectFirstLevelPropertiesHelper();
			foreach(CriteriaOperator opa in ops)
				helper.Process(opa);
			if(helper._Dictionary == null)
				return EmptyEnumerable<OperandProperty>.Instance;
			else
				return helper._Dictionary.Keys;
		}
		public static IEnumerable<OperandProperty> Collect(params CriteriaOperator[] ops) {
			return Collect((IEnumerable<CriteriaOperator>)ops);
		}
		Dictionary<OperandProperty, object> _Dictionary;
		CollectFirstLevelPropertiesHelper() { }
		void IClientCriteriaVisitor.Visit(AggregateOperand theOperand) {
			Process(theOperand.CollectionProperty);
		}
		void IClientCriteriaVisitor.Visit(OperandProperty theOperand) {
			if(_Dictionary == null)
				_Dictionary = new Dictionary<OperandProperty, object>();
			_Dictionary[theOperand] = theOperand;
		}
		void IClientCriteriaVisitor.Visit(JoinOperand theOperand) {
		}
		void ICriteriaVisitor.Visit(BetweenOperator theOperator) {
			Process(theOperator.BeginExpression);
			Process(theOperator.EndExpression);
			Process(theOperator.TestExpression);
		}
		void ICriteriaVisitor.Visit(BinaryOperator theOperator) {
			Process(theOperator.LeftOperand);
			Process(theOperator.RightOperand);
		}
		void ICriteriaVisitor.Visit(UnaryOperator theOperator) {
			Process(theOperator.Operand);
		}
		void ICriteriaVisitor.Visit(InOperator theOperator) {
			Process(theOperator.LeftOperand);
			Process(theOperator.Operands);
		}
		void ICriteriaVisitor.Visit(GroupOperator theOperator) {
			Process(theOperator.Operands);
		}
		void ICriteriaVisitor.Visit(OperandValue theOperand) {
		}
		void ICriteriaVisitor.Visit(FunctionOperator theOperator) {
			Process(theOperator.Operands);
		}
		void Process(CriteriaOperator opa) {
			if(!ReferenceEquals(null, opa))
				opa.Accept(this);
		}
		void Process(IEnumerable<CriteriaOperator> ops) {
			foreach(CriteriaOperator opa in ops)
				Process(opa);
		}
	}
	public class EmptyEnumerable<T>: IEnumerable<T>, IEnumerator<T> {
		public static readonly EmptyEnumerable<T> Instance = new EmptyEnumerable<T>();
		EmptyEnumerable() { }
		public IEnumerator<T> GetEnumerator() {
			return this;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return this;
		}
		public T Current {
			get { return default(T); }
		}
		public void Dispose() {
		}
		object IEnumerator.Current {
			get { return Current; }
		}
		public bool MoveNext() {
			return false;
		}
		public void Reset() {
		}
	}
	public class IsNullOrEmptyEliminator: IClientCriteriaVisitor<CriteriaOperator> {
		bool found;
		readonly string FieldName;
		IsNullOrEmptyEliminator(string fieldName) {
			this.FieldName = fieldName;
		}
		public CriteriaOperator Visit(AggregateOperand theOperand) {
			return theOperand;
		}
		public CriteriaOperator Visit(OperandProperty theOperand) {
			return theOperand;
		}
		public CriteriaOperator Visit(JoinOperand theOperand) {
			return theOperand;
		}
		public CriteriaOperator Visit(BetweenOperator theOperator) {
			return theOperator;
		}
		public CriteriaOperator Visit(BinaryOperator theOperator) {
			return theOperator;
		}
		public CriteriaOperator Visit(UnaryOperator theOperator) {
			return new UnaryOperator(theOperator.OperatorType, Process(theOperator.Operand));
		}
		public CriteriaOperator Visit(InOperator theOperator) {
			return theOperator;
		}
		public CriteriaOperator Visit(GroupOperator theOperator) {
			return GroupOperator.Combine(theOperator.OperatorType, Process(theOperator.Operands));
		}
		public CriteriaOperator Visit(OperandValue theOperand) {
			return theOperand;
		}
		public CriteriaOperator Visit(FunctionOperator theOperator) {
			if(theOperator.OperatorType == FunctionOperatorType.IsNullOrEmpty && theOperator.Operands.Count == 1) {
				OperandProperty op = theOperator.Operands[0] as OperandProperty;
				if(!ReferenceEquals(op, null)) {
					if(op.PropertyName == FieldName) {
						found = true;
						return null;
					}
				}
			}
			return theOperator;
		}
		public static bool Eliminate(CriteriaOperator op, string fieldName, out CriteriaOperator patched) {
			IsNullOrEmptyEliminator e = new IsNullOrEmptyEliminator(fieldName);
			var rv = e.Process(op);
			if(e.found) {
				patched = rv;
				return true;
			} else {
				patched = op;
				return false;
			}
		}
		protected CriteriaOperator Process(CriteriaOperator op) {
			if(ReferenceEquals(op, null))
				return null;
			return op.Accept(this);
		}
		protected IEnumerable<CriteriaOperator> Process(IEnumerable<CriteriaOperator> ops) {
			return ops.Select(op => Process(op)).Where(p => !ReferenceEquals(p, null));
		}
	}
}
