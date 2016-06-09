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
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(SeriesPointFilter.TypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SeriesPointFilter : ChartElement, ISupportInitialize, IObjectValueTypeProvider, IXtraSerializable {
		#region Nested class: TypeConverter
		class TypeConverter : System.ComponentModel.TypeConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) ? true :
					base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor)) {
					ConstructorInfo ci = typeof(SeriesPointFilter).GetConstructor(
						new Type[] { typeof(SeriesPointKey), typeof(DataFilterCondition), typeof(object) });
					return new InstanceDescriptor(ci, GetConstructorParams(value), true);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
			object[] GetConstructorParams(object value) {
				SeriesPointFilter filter = (SeriesPointFilter)value;
				object[] result = new object[3];
				result[0] = filter.Key;
				result[1] = filter.condition;
				result[2] = filter.Value;
				return result;
			}
		}
		#endregion
		internal const SeriesPointKey DefaultKey = SeriesPointKey.Argument;
		const DataFilterCondition DefaultCondition = DataFilterCondition.Equal;
		static SeriesBase GetOwnedSeriesBase(ChartElement chartElement) {
			while (chartElement != null && !(chartElement is SeriesBase))
				chartElement = chartElement.Owner;
			return chartElement as SeriesBase;
		}
		static Series GetOwnedSeries(ChartElement chartElement) {
			return GetOwnedSeriesBase(chartElement) as Series;
		}
		static Type ScaleTypeToType(ScaleType scaleType) {
			switch (scaleType) {
				case ScaleType.Numerical:
					return typeof(double);
				case ScaleType.Qualitative:
					return typeof(string);
				case ScaleType.DateTime:
					return typeof(DateTime);
				default:
					throw new DefaultSwitchException();
			}
		}
		SeriesPointKey key = DefaultKey;
		DataFilterCondition condition = DefaultCondition;
		object value;
		bool loading;
		protected internal override bool Loading { get { return loading || base.Loading; } }
		internal SeriesBase Series {
			get {
				SeriesViewBase view = Owner as SeriesViewBase;
				return view == null ? null : view.Owner as SeriesBase;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointFilterKey"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPointFilter.Key"),
		RefreshProperties(RefreshProperties.Repaint),
		TypeConverter(typeof(SeriesPointKeyConverter)),
		XtraSerializableProperty
		]
		public SeriesPointKey Key {
			get { return key; }
			set {
				if (value != key) {
					SendNotification(new ElementWillChangeNotification(this));
					key = value;
					if (!Loading)
						UpdateValue();
					SeriesPointFiltersChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointFilterCondition"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPointFilter.Condition"),
		XtraSerializableProperty
		]
		public DataFilterCondition Condition {
			get { return condition; }
			set {
				if (value != condition) {
					SendNotification(new ElementWillChangeNotification(this));
					condition = value;
					SeriesPointFiltersChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointFilterValue"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPointFilter.Value"),
		TypeConverter(typeof(ObjectValueTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public object Value {
			get { return value; }
			set {
				if (this.value != value) {
					SendNotification(new ElementWillChangeNotification(this));
					this.value = value;
					SeriesPointFiltersChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string StringValueSerializable {
			get { return value == null ? String.Empty : (string)value; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				this.value = value;
				SeriesPointFiltersChanged();
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public double DoubleValueSerializable {
			get { return value == null ? 0.0 : (double)value; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				this.value = value;
				SeriesPointFiltersChanged();
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public DateTime DateTimeValueSerializable {
			get { return (DateTime)value; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				this.value = value;
				SeriesPointFiltersChanged();
			}
		}
		public SeriesPointFilter() : base() {
			Initialize(DefaultKey, DefaultCondition, null);
		}
		public SeriesPointFilter(SeriesPointKey key, DataFilterCondition condition, double value) : base() {
			Initialize(key, condition, value);
		}
		public SeriesPointFilter(SeriesPointKey key, DataFilterCondition condition, string value) : base() {
			Initialize(key, condition, value);
		}
		public SeriesPointFilter(SeriesPointKey key, DataFilterCondition condition, DateTime value) : base() {
			Initialize(key, condition, value);
		}
		public SeriesPointFilter(SeriesPointKey key, DataFilterCondition condition, object value) : base() {
			Initialize(key, condition, value);
		}
		void Initialize(SeriesPointKey key, DataFilterCondition condition, object value) {
			this.key = key;
			this.condition = condition;
			this.value = value;
		}
		#region IObjectValueTypeProvider implementation
		Type IObjectValueTypeProvider.DataType { get { return GetDataType(); } }
		#endregion
		#region ISupportInitialize implementation
		void ISupportInitialize.BeginInit() {
			loading = true;
		}
		void ISupportInitialize.EndInit() {
			loading = false;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Key")
				return ShouldSerializeKey();
			if (propertyName == "Condition")
				return ShouldSerializeCondition();
			if (propertyName == "StringValueSerializable")
				return ShouldSerializeStringValueSerializable();
			if (propertyName == "DoubleValueSerializable")
				return ShouldSerializeDoubleValueSerializable();
			if (propertyName == "DateTimeValueSerializable")
				return ShouldSerializeDateTimeValueSerializable();
			return base.XtraShouldSerialize(propertyName);
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			((ISupportInitialize)this).BeginInit();
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			((ISupportInitialize)this).EndInit();
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeKey() {
			return key != DefaultKey;
		}
		void ResetKey() {
			Key = DefaultKey;
		}
		bool ShouldSerializeCondition() {
			return condition != DefaultCondition;
		}
		void ResetCondition() {
			Condition = DefaultCondition;
		}
		bool ShouldSerializeValue() {
			return Value != null;
		}
		void ResetValue() {
			Value = null;
		}
		bool ShouldSerializeStringValueSerializable() {
			return value != null && (value is string);
		}
		bool ShouldSerializeDoubleValueSerializable() {
			return value != null && (value is double);
		}
		bool ShouldSerializeDateTimeValueSerializable() {
			return value != null && (value is DateTime);
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		#endregion
		void UpdateValue() {
			if (value == null)
				return;
			Type dataType = GetDataType();
			if (dataType == null) {
				value = null;
				return;
			}
			System.ComponentModel.TypeConverter converter = TypeDescriptor.GetConverter(dataType);
			if (converter != null) {
				try {
					value = converter.ConvertTo(value, dataType);
				}
				catch {
					value = null;
				}
			}
		}
		void SeriesPointFiltersChanged() {
			RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.SeriesPointFiltersChanged));
		}
		object GetFilterValue(SeriesPoint point) {
			if (key == SeriesPointKey.Argument) {
				Series series = GetOwnedSeries(this);
				if (series == null)
					throw new InternalException("SeriesPointFilter error");
				switch (series.ActualArgumentScaleType) {
					case ScaleType.Numerical:
						return point.NumericalArgument;
					case ScaleType.DateTime:
						return point.DateTimeArgument;
					default:
						return point.Argument;
				}
			}
			int valueIndex = (int)key - 1;
			return valueIndex < point.Values.Length ? point.GetValueObject(valueIndex) : null;
		}
		object GetConvertedValue(ScaleType scaleType) {
			if (value == null)
				return null;
			try {
				switch (scaleType) {
					case ScaleType.Qualitative:
						return value.ToString();
					case ScaleType.DateTime:
						return Convert.ToDateTime(value);
					case ScaleType.Numerical:
						return Convert.ToDouble(value);
				}
			}
			catch {
			}
			return value;
		}
		protected override ChartElement CreateObjectForClone() {
			return new SeriesPointFilter();
		}
		internal bool CanConvert(ScaleType scaleType) {
			if (value != null) {
				object result = GetConvertedValue(scaleType);
				switch (scaleType) {
					case ScaleType.Qualitative:
						return (result is string);
					case ScaleType.Numerical:
						return (result is double);
					case ScaleType.DateTime:
						return (result is DateTime);
				}
			}
			return true;
		}
		internal void ConvertBasedOnScaleType(ScaleType scaleType) {
			value = GetConvertedValue(scaleType);
		}
		internal Type GetDataType() {
			SeriesBase seriesBase = GetOwnedSeriesBase(this);
			if (seriesBase != null)
				return
					key == SeriesPointKey.Argument ?
					ScaleTypeToType(seriesBase.ActualArgumentScaleType) :
					ScaleTypeToType(seriesBase.ValueScaleType);
			else
				return null;
		}
		internal bool CheckSeriesPoint(SeriesPoint point) {
			if (value == null)
				throw new InternalException("SeriesPointFilter error");
			object filterValue = GetFilterValue(point);
			if (filterValue == null)
				return false;
			switch (condition) {
				case DataFilterCondition.Equal:
					return filterValue.Equals(value);
				case DataFilterCondition.NotEqual:
					return !filterValue.Equals(value);
			}
			IComparable comparable = filterValue as IComparable;
			if (comparable != null) {
				int compareResult = comparable.CompareTo(value);
				try {
					switch (condition) {
						case DataFilterCondition.GreaterThan:
							return compareResult > 0;
						case DataFilterCondition.GreaterThanOrEqual:
							return compareResult >= 0;
						case DataFilterCondition.LessThan:
							return compareResult < 0;
						case DataFilterCondition.LessThanOrEqual:
							return compareResult <= 0;
					}
				} catch {
				}
			}
			return false;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SeriesPointFilter filter = obj as SeriesPointFilter;
			if (filter == null)
				return;
			key = filter.key;
			condition = filter.condition;
			value = filter.value;
		}
		public override bool Equals(object obj) {
			SeriesPointFilter filter = (SeriesPointFilter)obj;
			if (key != filter.key || condition != filter.condition)
				return false;
			if (value == null)
				return filter.value == null;
			IComparable comparable = value as IComparable;
			if (comparable == null)
				return false;
			return comparable.CompareTo(filter.value) == 0;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return ChartLocalizer.GetString(ChartStringId.DefaultSeriesPointFilterName);
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SeriesPointFilterCollection : ChartCollectionBase {
		internal const ConjunctionTypes DefaultConjunctionMode = ConjunctionTypes.And;
		static bool CheckSeriesPointByAnd(SeriesPoint point, List<SeriesPointFilter> filters) {
			foreach (SeriesPointFilter filter in filters)
				if (!filter.CheckSeriesPoint(point))
					return false;
			return true;
		}
		static bool CheckSeriesPointByOr(SeriesPoint point, List<SeriesPointFilter> filters) {
			foreach (SeriesPointFilter filter in filters)
				if (filter.CheckSeriesPoint(point))
					return true;
			return false;
		}
		ConjunctionTypes conjunctionMode = DefaultConjunctionMode;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("SeriesPointFilterCollectionConjunctionMode")]
#endif
		public ConjunctionTypes ConjunctionMode {
			get { return conjunctionMode; }
			set {
				if (value != conjunctionMode) {
					SendControlChanging();
					conjunctionMode = value;
					RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.SeriesPointFiltersChanged));
				}
			}
		}
		public SeriesPointFilter this[int index] { get { return (SeriesPointFilter)List[index]; } }
		internal SeriesPointFilterCollection(ChartElement chartElement) : base(chartElement) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeConjunctionMode() {
			return conjunctionMode != DefaultConjunctionMode;
		}
		void ResetConjunctionMode() {
			ConjunctionMode = DefaultConjunctionMode;
		}
		#endregion
		List<SeriesPointFilter> GetActualFilters() {
			List<SeriesPointFilter> filters = new List<SeriesPointFilter>();
			foreach (SeriesPointFilter filter in this)
				if (filter.Value != null)
					filters.Add(filter);
			return filters;
		}
		internal bool CheckSeriesPoint(SeriesPoint point) {
			if (Count == 0)
				return false;
			List<SeriesPointFilter> actualFilters = GetActualFilters();
			if (actualFilters.Count == 0)
				return false;
			switch (conjunctionMode) {
				case ConjunctionTypes.And:
					return CheckSeriesPointByAnd(point, actualFilters);
				case ConjunctionTypes.Or:
					return CheckSeriesPointByOr(point, actualFilters);
				default:
					throw new DefaultSwitchException();
			}
		}
		public int Add(SeriesPointFilter filter) {
			return base.Add(filter);
		}
		public void Insert(int index, SeriesPointFilter filter) {
			base.Insert(index, filter);
		}
		public void AddRange(SeriesPointFilter[] coll) {
			base.AddRange(coll);
		}
		public void Remove(SeriesPointFilter filter) {
			base.Remove(filter);
		}
		public override bool Equals(object obj) {
			SeriesPointFilterCollection collection = (SeriesPointFilterCollection)obj;
			if (Count != collection.Count)
				return false;
			for (int i = 0; i < Count; i++)
				if (!this[i].Equals(collection[i]))
					return false;
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override void Assign(ChartCollectionBase collection) {
			base.Assign(collection);
			SeriesPointFilterCollection filters = collection as SeriesPointFilterCollection;
			if (filters == null)
				return;
			this.conjunctionMode = filters.conjunctionMode;
		}
	}
}
