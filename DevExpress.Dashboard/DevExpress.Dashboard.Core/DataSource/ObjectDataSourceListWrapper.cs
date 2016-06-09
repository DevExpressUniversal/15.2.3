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
using DevExpress.Data;
using DevExpress.Data.Access;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.Xpo;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon.Native {
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class ObjectDataSourceListWrapper : IList, ITypedList, IDataControllerData, IActualParametersProvider {
		readonly ListSourceDataController dataController = new ListSourceDataController();
		readonly IList list;
		readonly IDashboardDataSource dataSource;
		readonly string dataMember;
		IEnumerable<IParameter> parameters;
		int cachedCount = 0;
		public IList List { get { return list; } }
		public IEnumerable<PropertyDescriptor> Properties { get { return dataController.Columns.Select(c => c.PropertyDescriptor); } }
		public ObjectDataSourceListWrapper(IList list, IDashboardDataSource dataSource, IEnumerable<IParameter> parameters)
			: this(null, list, dataSource, parameters) {
		}
		public ObjectDataSourceListWrapper(string dataMember, IList list, IDashboardDataSource dataSource, IEnumerable<IParameter> parameters) {
			this.list = list;
			this.dataSource = dataSource;
			this.parameters = parameters;
			this.dataMember = dataMember;
			dataController.VisibleRowCountChanged += UpdateCachedCount;
			dataController.DataClient = this;
			dataController.ListSource = list;
#if !DXPORTABLE
			if(dataSource.DataProvider == null) {
				dataController.FilterCriteria = dataSource.GetExpandedCalculatedFieldExpressionOperator(dataSource.Filter, null, false, dataMember, this, true);
			}
#endif
			dataController.PopulateColumns();
		}
		public void RePopulateColumns(IEnumerable<IParameter> parameters) {
			this.parameters = parameters;
			RePopulate();
		}
		void RePopulate() {
#if !DXPORTABLE
			if(dataSource.DataProvider == null) {
				dataController.BeginUpdate();
				dataController.FilterCriteria = dataSource.GetExpandedCalculatedFieldExpressionOperator(dataSource.Filter, null, false, dataMember, this, true);
				dataController.CancelUpdate();
			}
#endif
			dataController.RePopulateColumns();
			if(parameters == null || parameters.Count() == 0)
				dataController.DoRefresh();
		}
		void UpdateCachedCount(object obj, EventArgs e) {
			cachedCount = dataController.VisibleCount;
		}
#region ITypedList
		class WrappedUnboundPropertyDescriptor : UnboundPropertyDescriptor {
			Func<int, int> action;
			protected internal WrappedUnboundPropertyDescriptor(Func<int, int> action, DataController controller, UnboundPropertyDescriptor descr)
				: base(controller, descr.UnboundInfo) {
				this.action = action;
			}
			protected override object GetEvaluatorValue(int row) {
				return base.GetEvaluatorValue(action(row));
			}
		}
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			ITypedList typedList = list as ITypedList;
			if(typedList != null) {
				PropertyDescriptorCollection propertyCollection = typedList.GetItemProperties(null);
				ExpandablePropertyDescriptorCollection newPropertyDescriptorCollection = new ExpandablePropertyDescriptorCollection(propertyCollection);
				foreach(PropertyDescriptor descr in dataController.Helper.DescriptorCollection)
					newPropertyDescriptorCollection.Add(PatchPropertyDescriptor(descr));
				return newPropertyDescriptorCollection;
			} else {
				List<PropertyDescriptor> propertyCollection = new List<PropertyDescriptor>();
				foreach(PropertyDescriptor property in dataController.Helper.DescriptorCollection)
					propertyCollection.Add(PatchPropertyDescriptor(property));
				return new PropertyDescriptorCollection(propertyCollection.ToArray());
			}
		}
		PropertyDescriptor PatchPropertyDescriptor(PropertyDescriptor property) {
			UnboundPropertyDescriptor unboundProperty = property as UnboundPropertyDescriptor;
			if(unboundProperty == null)
				return property;
			else
				return new WrappedUnboundPropertyDescriptor((index) => dataController.FilterHelper.VisibleListSourceRows.GetListSourceRow(index), dataController, unboundProperty);
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return string.Empty;
		}
#endregion
#region IList
		int IList.Add(object value) {
			return -1;
		}
		void IList.Clear() {
		}
		bool IList.Contains(object value) {
			int index = list.IndexOf(value);
			return index >= 0 && dataController.FilterHelper.VisibleListSourceRows.Contains(index);
		}
		int IList.IndexOf(object value) {
			int index = list.IndexOf(value);
			int? dataControllerIndex = dataController.FilterHelper.VisibleListSourceRows.GetVisibleRow(index);
			return dataControllerIndex ?? -1;
		}
		void IList.Insert(int index, object value) {
		}
		bool IList.IsFixedSize {
			get { return true; }
		}
		bool IList.IsReadOnly {
			get { return true; }
		}
		void IList.Remove(object value) {
		}
		void IList.RemoveAt(int index) {
		}
		object IList.this[int index] {
			get {
				return list[dataController.FilterHelper.VisibleListSourceRows.GetListSourceRow(index)];
			}
			set {
			}
		}
		void ICollection.CopyTo(Array array, int index) {
			IList source = this;
			for(int i = 0; i < source.Count; i++)
				array.SetValue(source[i], index + i);
		}
		int ICollection.Count {
			get { return cachedCount; }
		}
		public bool IsSynchronized {
			get { return list.IsSynchronized; }
		}
		public object SyncRoot {
			get { return null; }
		}
		public IEnumerator GetEnumerator() {
			return new ListSourceWrapperEnumerator(this);
		}
		class ListSourceWrapperEnumerator : IEnumerator {
			readonly IList wrapper;
			int index = -1;
			public ListSourceWrapperEnumerator(IList wrapper) {
				this.wrapper = wrapper;
			}
			object IEnumerator.Current {
				get { return wrapper[index]; }
			}
			bool IEnumerator.MoveNext() {
				index++;
				return index < wrapper.Count;
			}
			void IEnumerator.Reset() {
				index = -1;
			}
		}
#endregion
#region IDataControllerData
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() {
			UnboundColumnInfoCollection collection = new UnboundColumnInfoCollection();
			if(parameters != null)
				foreach(IParameter parameter in parameters)
					collection.Add(new UnboundColumnInfo(string.Format(CalculatedFieldsController.ParameterFormatString, parameter.Name), UnboundColumnType.Object, true, new OperandValue(parameter.Value).ToString()));
			return collection;
		}
		object IDataControllerData.GetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
			return value;
		}
		void IDataControllerData.SetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
		}
#endregion
		#region IActualParametersProvider
		IEnumerable<IParameter> IActualParametersProvider.GetParameters() {
			return parameters;
		}
		IEnumerable<IParameter> IActualParametersProvider.GetActualParameters() {
			return parameters;
		}
		#endregion
	}
	public class ExpandablePropertyDescriptorCollection : PropertyDescriptorCollection {
		PropertyDescriptorCollection baseCollection;
		public ExpandablePropertyDescriptorCollection(PropertyDescriptorCollection baseCollection)
			: base(null) {
			this.baseCollection = baseCollection;
		}
		public override PropertyDescriptor Find(string name, bool ignoreCase) {
			PropertyDescriptor pd = base.Find(name, ignoreCase);
			if(pd == null) {
				pd = baseCollection.Find(name, ignoreCase);
				if(pd != null)
					Add(pd);
			}
			return pd;
		}
	}
}
