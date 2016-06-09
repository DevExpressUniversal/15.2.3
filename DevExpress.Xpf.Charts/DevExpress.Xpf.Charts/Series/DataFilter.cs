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
using System.ComponentModel;
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Design;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum DataFilterCondition {
		Equal = DevExpress.Charts.Native.Condition.Equal,
		GreaterThan = DevExpress.Charts.Native.Condition.GreaterThan,
		GreaterThanOrEqual = DevExpress.Charts.Native.Condition.GreaterThanOrEqual,
		LessThan = DevExpress.Charts.Native.Condition.LessThan,
		LessThanOrEqual = DevExpress.Charts.Native.Condition.LessThanOrEqual,
		NotEqual = DevExpress.Charts.Native.Condition.NotEqual
	}
	public enum ConjunctionTypes {
		And = Conjunction.And,
		Or = Conjunction.Or
	}
	public class DataFilter : ChartNonVisualElement, IDataFilter {
		public static readonly DependencyProperty ColumnNameProperty;
		public static readonly DependencyProperty DataTypeProperty;
		public static readonly DependencyProperty ConditionProperty;
		public static readonly DependencyProperty ValueProperty;
		static DataFilter() {
			Type ownerType = typeof(DataFilter);
			ColumnNameProperty = DependencyPropertyManager.Register("ColumnName", typeof(string), ownerType, new PropertyMetadata(String.Empty, ColumnNameChanged));
			DataTypeProperty = DependencyPropertyManager.Register("DataType", typeof(Type), ownerType, new PropertyMetadata(DataTypeChanged));
			ConditionProperty = DependencyPropertyManager.Register("Condition", typeof(DataFilterCondition), ownerType, 
				new PropertyMetadata(DataFilterCondition.Equal, ConditionChanged));
			ValueProperty = DependencyPropertyManager.Register("Value", typeof(object), ownerType, new PropertyMetadata(null, ValueChanged));
		}
		static void ColumnNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DataFilter dataFilter = d as DataFilter;
			if (dataFilter != null && dataFilter.Series != null && dataFilter.DataType == null) {
				dataFilter.actualDataType = BindingHelperCore.GetMemberType(dataFilter.Series.GetDataSource(), dataFilter.ColumnName);
				dataFilter.actualValue = BindingProcedure.ConvertToCompatibleValue(dataFilter, dataFilter.Value);
			}
			ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateSeriesBinding);
		}
		static void DataTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DataFilter dataFilter = d as DataFilter;
			if (dataFilter != null) {
				dataFilter.actualDataType = (Type)e.NewValue;
				dataFilter.actualValue = BindingProcedure.ConvertToCompatibleValue(dataFilter, dataFilter.Value);
			}
			ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateSeriesBinding);
		}
		static void ConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateSeriesBinding);
		}
		static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DataFilter dataFilter = d as DataFilter;
			if (dataFilter != null)
				dataFilter.actualValue = BindingProcedure.ConvertToCompatibleValue(dataFilter, e.NewValue);
			ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateSeriesBinding);
		}
		bool required;
		Type actualDataType;
		object actualValue;
		Series Series { get { return (Series)((IChartElement)this).Owner; } }
		internal Type ActualType { get { return actualDataType; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("DataFilterColumnName"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public string ColumnName {
			get { return (string)GetValue(ColumnNameProperty); }
			set { SetValue(ColumnNameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("DataFilterDataType"),
#endif
		Category(Categories.Common),
		TypeConverter(typeof(DataTypeTypeConverter)),
		XtraSerializableProperty
		]
		public Type DataType {
			get { return (Type)GetValue(DataTypeProperty); }
			set { SetValue(DataTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("DataFilterCondition"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public DataFilterCondition Condition {
			get { return (DataFilterCondition)GetValue(ConditionProperty); }
			set { SetValue(ConditionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("DataFilterValue"),
#endif
		Category(Categories.Common),
		TypeConverter(typeof(ValueTypeConverter)),
		XtraSerializableProperty
		]
		public object Value {
			get { return (object)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		#region IDataFilter interface implementation
		DevExpress.Charts.Native.Condition IDataFilter.Condition { get { return (DevExpress.Charts.Native.Condition)Condition; } }
		Type IDataFilter.DataType { get { return actualDataType; } }
		bool IDataFilter.Required { get { return required; } }
		object IDataFilter.Value { get { return actualValue; } }
		#endregion
		internal DataFilter(string columnName, string dataTypeName, DataFilterCondition condition, object val, bool required) {
			ColumnName = columnName;
			if (dataTypeName != null)
				DataType = Type.GetType(dataTypeName, false);
			Condition = condition;
			Value = BindingProcedure.ConvertToCompatibleValue(this, val);
			this.required = required;
		}
		public DataFilter(string columnName, string dataTypeName, DataFilterCondition condition, object val) : this(columnName, dataTypeName, condition, val, false) {
		}
		public DataFilter() {
		}
		internal void UpdateActualParams() {
			actualDataType = BindingHelperCore.GetMemberType(Series.GetDataSource(), ColumnName);
			actualValue = BindingProcedure.ConvertToCompatibleValue(this, Value);
		}
		internal DataFilter CloneForBinding() {
			DataFilter dataFilter = new DataFilter(ColumnName, DataType == null ? null : DataType.ToString(), Condition, Value, required);
			dataFilter.actualDataType = actualDataType;
			dataFilter.actualValue = actualValue;
			return dataFilter;
		}
	}
	public class DataFilterCollection : ChartElementCollection<DataFilter>, IEnumerable<IDataFilter> {
		protected override ChartElementChange Change { get { return ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateSeriesBinding; } }
		IEnumerator<IDataFilter> IEnumerable<IDataFilter>.GetEnumerator() {
			foreach(IDataFilter dataFilter in this)
				yield return dataFilter;
		}
	}
}
