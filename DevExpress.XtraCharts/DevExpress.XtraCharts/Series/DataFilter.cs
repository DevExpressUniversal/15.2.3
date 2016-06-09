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
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using DevExpress.Charts.Native;
using DevExpress.Data.Browsing;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum DataFilterCondition {
		Equal = Condition.Equal,
		GreaterThan = Condition.GreaterThan,
		GreaterThanOrEqual = Condition.GreaterThanOrEqual,
		LessThan = Condition.LessThan,
		LessThanOrEqual = Condition.LessThanOrEqual,
		NotEqual = Condition.NotEqual
	}
	public enum ConjunctionTypes {
		And = Conjunction.And,
		Or = Conjunction.Or
	}
	[
	TypeConverter(typeof(DataFilter.DataFilterTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class DataFilter : ChartElement, IDataFilter, IObjectValueTypeProvider {
		#region Nested classes: DataFilterTypeConverter, ValueTypeConverter, DataTypeConverter
		internal class DataFilterTypeConverter : System.ComponentModel.TypeConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) ? true : base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor)) {
					ConstructorInfo ci = typeof(DataFilter).GetConstructor(
						new Type[] { typeof(string), typeof(string), typeof(DataFilterCondition), typeof(object) });
					return new InstanceDescriptor(ci, GetConstructorParams(value), true);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
			object[] GetConstructorParams(object value) {
				object[] result = new object[4];
				result[0] = ((DataFilter)value).ActualColumnName;
				result[1] = ((DataFilter)value).DataType == null ? null : ((DataFilter)value).DataType.ToString();
				result[2] = ((DataFilter)value).Condition;
				result[3] = ((DataFilter)value).Value;
				return result;
			}
		}
		#endregion
		static readonly Type defaultType = typeof(string);
		const string StringTypeName = "System.String";
		const DataFilterCondition DefaultCondition = DataFilterCondition.Equal;
		string columnName;
		Type dataType;
		DataFilterCondition condition = DefaultCondition;
		object val;
		string valueSerializable;
		bool required;
		string ChartDataMember {
			get {
				return ((ChartContainer != null) && (ChartContainer.Chart != null) && (ChartContainer.Chart.DataContainer != null)) ? ChartContainer.Chart.DataContainer.DataMember : String.Empty;
			}
		}
		internal string ActualColumnName {
			get { return columnName; }
			set {
				if (value != columnName) {
					if (!CheckColumnName(value))
						throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDataMember), value));
					SendNotification(new ElementWillChangeNotification(this));
					columnName = value;
					if (Owner != null) {
						Type newDataType = BindingHelperCore.GetMemberType(Owner.GetDataSource(),
							BindingProcedure.ConvertToActualDataMember(Owner.GetChartDataMember(), columnName));
						if (newDataType != null)
							DataType = newDataType;
					}
					RaiseControlChanged();
				}
			}
		}
		internal new SeriesBase Owner {
			get { return (SeriesBase)base.Owner; }
			set { base.Owner = value; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DataFilterColumnName"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DataFilter.ColumnName"),
		RefreshProperties(RefreshProperties.Repaint),
		Editor("DevExpress.XtraCharts.Design.DataMemberEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty
		]
		public string ColumnName {
			get {
				if (Owner != null && ChartContainer != null && ChartContainer.DataProvider != null && ChartContainer.DesignMode) {
					return BindingHelper.GetDataMemberName(ChartContainer.DataProvider.DataContext, Owner.GetDataSource(), ChartDataMember, columnName);
				}
				return columnName;
			}
			set { ActualColumnName = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string ColumnNameSerializable {
			get {
				if (this.columnName == null)
					return String.Empty;
				return this.columnName;
			}
			set {
				try {
					ColumnName = value;
				}
				catch {
					ColumnName = string.Empty;
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DataFilterDataType"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DataFilter.DataType"),
		RefreshProperties(RefreshProperties.Repaint),
		TypeConverter(typeof(DataTypeConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public Type DataType {
			get { return this.dataType; }
			set {
				if (value != dataType) {
					SendNotification(new ElementWillChangeNotification(this));
					this.dataType = value;
					if (!Loading && !CheckCondition(condition))
						condition = DataFilterCondition.Equal;
					try {
						SetValue(val);
					}
					catch {
						val = null;
					}
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string DataTypeSerializable {
			get {
				if (dataType == null)
					return String.Empty;
				return dataType.Assembly.GlobalAssemblyCache ? dataType.ToString() : dataType.AssemblyQualifiedName;
			}
			set {
				try {
					DataType = Type.GetType(value);
				}
				catch {
					DataType = null;
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DataFilterCondition"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DataFilter.Condition"),
		TypeConverter(typeof(DataFilterConditionTypeConverter)),
		XtraSerializableProperty
		]
		public DataFilterCondition Condition {
			get { return condition; }
			set {
				if (value != condition) {
					if (!CheckCondition(value))
						throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgInvalidFilterCondition),
							value.ToString(), dataType.ToString()));
					SendNotification(new ElementWillChangeNotification(this));
					condition = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DataFilterValue"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DataFilter.Value"),
		TypeConverter(typeof(ObjectValueTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public object Value {
			get { return val; }
			set {
				if (value != val) {
					SendNotification(new ElementWillChangeNotification(this));
					SetValue(value);
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public string InvariantValueSerializable {
			get { return val == null ? String.Empty : Convert.ToString(val, CultureInfo.InvariantCulture); }
			set {
				val = null;
				valueSerializable = value;
			}
		}
		[
		Obsolete("This property is now obsolete. Use the InvariantValueSerializable property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public string ValueSerializable {
			get { return val == null ? String.Empty : val.ToString(); }
			set {
				if (val == null || value != val.ToString()) {
					SendNotification(new ElementWillChangeNotification(this));
					SetValue(value);
					RaiseControlChanged();
				}
			}
		}
		internal DataFilter(string columnName, string dataTypeName, DataFilterCondition condition, object val, bool required) {
			this.columnName = columnName;
			try {
				dataType = Type.GetType(dataTypeName);
			}
			catch {
				dataType = null;
			}
			this.condition = condition;
			SetValue(val);
			this.required = required;
		}
		public DataFilter(string columnName, string dataTypeName, DataFilterCondition condition, object val) : this(columnName, dataTypeName, condition, val, false) {
		}
		public DataFilter() : this(String.Empty, StringTypeName, DataFilterCondition.Equal, null, false) {
		}
		#region IDataFilter interface implementation
		Condition IDataFilter.Condition { get { return (Condition)condition; } }
		bool IDataFilter.Required { get { return required; } }
		string IDataFilter.ColumnName { get { return ActualColumnName; } }
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeColumnName() {
			return !String.IsNullOrEmpty(columnName);
		}
		void ResetColumnName() {
			ColumnName = string.Empty;
		}
		bool ShouldSerializeDataType() {
			return !dataType.Equals(defaultType);
		}
		void ResetDataType() {
			DataType = defaultType;
		}
		bool ShouldSerializeCondition() {
			return condition != DefaultCondition;
		}
		void ResetCondition() {
			Condition = DefaultCondition;
		}
		bool ShouldSerializeValue() {
			return this.val != null;
		}
		void ResetValue() {
			Value = null;
		}
		bool ShouldSerializeInvariantValueSerializable() {
			return !String.IsNullOrEmpty(InvariantValueSerializable);
		}
		bool ShouldSerializeValueSerializable() {
			return false;
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		#endregion
		#region XtraSerialization
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "ColumnName":
					return false;
				case "DataTypeSerializable":
					return ShouldSerializeDataType();
				case "Condition":
					return ShouldSerializeCondition();
				case "InvariantValueSerializable":
					return ShouldSerializeInvariantValueSerializable();
				case "ValueSerializable":
					return ShouldSerializeValueSerializable();
				case "ColumnNameSerializable":
					return ShouldSerializeColumnName();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		void SetValue(object value) {
			val = BindingProcedure.ConvertToCompatibleValue(this, value);
			if (dataType == null && val != null)
				dataType = val.GetType();
		}
		bool CheckCondition(DataFilterCondition condition) {
			return condition == DataFilterCondition.Equal || condition == DataFilterCondition.NotEqual ||
				(dataType != null && typeof(IComparable).IsAssignableFrom(dataType));
		}
		protected override ChartElement CreateObjectForClone() {
			throw new NotSupportedException();
		}
		protected override bool ProcessChanged(ChartElement sender, ChartUpdateInfoBase changeInfo) {
			if (!Loading && Owner != null)
				Owner.BindingChanged();
			return false;
		}
		internal bool CheckColumnName(string columnName) {
			if (Owner == null || Owner.Loading)
				return true;
			ChartContainerAdapter adapter = Owner.ContainerAdapter;
			DataContext dataContext = adapter != null ? adapter.DataContext : null;
			return BindingHelper.CheckDataMember(dataContext, Owner.GetDataSource(), BindingProcedure.ConvertToActualDataMember(Owner.GetChartDataMember(), columnName));
		}
		internal void OnEndLoading() {
			if (val == null && !String.IsNullOrEmpty(valueSerializable)) {
				if (dataType == null)
					val = valueSerializable;
				else {
					TypeConverter converter = TypeDescriptor.GetConverter(dataType);
					if (converter == null)
						val = valueSerializable;
					else
						try {
							val = converter.ConvertTo(null, CultureInfo.InvariantCulture, valueSerializable, dataType);
						}
						catch {
							try {
								val = converter.ConvertFrom(null, CultureInfo.InvariantCulture, valueSerializable);
							}
							catch {
								val = valueSerializable;
							}
						}
				}
			}
			valueSerializable = null;
		}
		public override string ToString() {
			return (ColumnName != null && ColumnName.Length > 0) ? ColumnName :
				ChartLocalizer.GetString(ChartStringId.DefaultDataFilterName);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			DataFilter filter = obj as DataFilter;
			if (filter == null)
				return;
			this.columnName = filter.columnName;
			this.dataType = filter.dataType == null ? null : Type.GetType(filter.dataType.ToString());
			this.condition = filter.condition;
			this.val = filter.val;
			this.required = filter.required;
		}
		public override object Clone() {
			return new DataFilter(columnName, dataType == null ? null : dataType.ToString(), condition, val, required);
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class DataFilterCollection : ChartCollectionBase, IEnumerable<IDataFilter> {
		#region Nested class: DataFilterEnumerator
		class DataFilterEnumerator : IEnumerator<IDataFilter> {
			readonly DataFilterCollection collection;
			int currentIndex;
			public object Current { get { return collection[currentIndex]; } }
			IDataFilter IEnumerator<IDataFilter>.Current { get { return (IDataFilter)Current; } }
			public DataFilterEnumerator(DataFilterCollection collection) {
				this.collection = collection;
				Reset();
			}
			public void Dispose() { }
			public void Reset() {
				currentIndex = -1;
			}
			public bool MoveNext() {
				return ++currentIndex < collection.List.Count;
			}
		}
		#endregion
		internal const ConjunctionTypes DefaultConjunctionMode = ConjunctionTypes.And;
		ConjunctionTypes conjunctionMode = DefaultConjunctionMode;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("DataFilterCollectionConjunctionMode")]
#endif
		public ConjunctionTypes ConjunctionMode {
			get { return conjunctionMode; }
			set {
				if (value != conjunctionMode) {
					SendControlChanging();
					conjunctionMode = value;
					RaiseControlChanged();
				}
			}
		}
		public DataFilter this[int index] {
			get { return (DataFilter)List[index]; }
		}
		internal DataFilterCollection(SeriesBase series) : base() {
			base.Owner = series;
		}
		#region IEnumerable<IDataFilter> implementation
		IEnumerator<IDataFilter> IEnumerable<IDataFilter>.GetEnumerator() {
			return new DataFilterEnumerator(this);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeConjunctionMode() {
			return conjunctionMode != DefaultConjunctionMode;
		}
		void ResetConjunctionMode() {
			ConjunctionMode = DefaultConjunctionMode;
		}
		#endregion
		void AttachFilter(DataFilter filter) {
			filter.Owner = (SeriesBase)Owner;
			if (!filter.CheckColumnName(filter.ActualColumnName))
				throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDataMember), filter.ActualColumnName));
		}
		protected internal override void ProcessChanged(ChartUpdateInfoBase changeInfo) {
			((SeriesBase)Owner).BindingChanged(changeInfo);
		}
		public int Add(DataFilter filter) {
			AttachFilter(filter);
			SendControlChanging();
			int res = base.Add(filter);
			RaiseControlChanged();
			return res;
		}
		public int Add(string columnName, string dataTypeName, DataFilterCondition condition, object val) {
			return Add(new DataFilter(columnName, dataTypeName, condition, val));
		}
		public void Insert(int index, DataFilter filter) {
			AttachFilter(filter);
			SendControlChanging();
			base.Insert(index, filter);
			RaiseControlChanged();
		}
		public void ClearAndAddRange(DataFilter[] coll) {
			Clear();
			AddRange(coll);
		}
		public void AddRange(DataFilter[] coll) {
			SendControlChanging();
			foreach (DataFilter filter in coll) {
				filter.Owner = (SeriesBase)Owner;
				if (!filter.CheckColumnName(filter.ActualColumnName))
					throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDataMember), filter.ActualColumnName));
			}
			base.AddRange(coll);
			RaiseControlChanged();
		}
		public void Remove(DataFilter filter) {
			SendControlChanging();
			base.Remove(filter);
			RaiseControlChanged();
		}
	}
}
