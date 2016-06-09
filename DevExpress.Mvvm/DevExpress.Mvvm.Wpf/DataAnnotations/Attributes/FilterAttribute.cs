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

#if MVVM
namespace DevExpress.Mvvm.Native {
	using System;
	using System.ComponentModel;
	using System.Linq.Expressions;
	public enum FilterRangeUIEditorType { Default, Range, Text, Spin }
	public enum FilterDateTimeRangeUIEditorType { Default, Picker, Range, Calendar }
	public enum FilterLookupUIEditorType { Default, List, DropDown, TokenBox }
	public enum FilterBooleanUIEditorType { Default, Check, Toggle, List, DropDown }
	public static class FilterMetadataTypeAttributeHelper {
		public const string FilteringMetadataTypeName = "FilterMetadataTypeAttribute";
		public const string FilteringMetadataTypeNamespace = "DevExpress.Utils.Filtering";
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public abstract class FilterAttributeProxy : Attribute, IAttributeProxy {
		const string FilterRangeAttributeName = "DevExpress.Utils.Filtering.FilterRangeAttribute";
		const string FilterDateTimeRangeAttributeName = "DevExpress.Utils.Filtering.FilterDateTimeRangeAttribute";
		const string FilterLookupAttributeName = "DevExpress.Utils.Filtering.FilterLookupAttribute";
		const string FilterBooleanChoiceAttributeName = "DevExpress.Utils.Filtering.FilterBooleanChoiceAttribute";
		const string FilterEnumChoiceAttributeName = "DevExpress.Utils.Filtering.FilterEnumChoiceAttribute";
#region Range Initializer
		protected delegate Attribute RangeAttributeCtor(object minOrMinMember, object maxOrMaxMember, object avgOrAvgMember);
		protected static RangeAttributeCtor RangeInitializer {
			get { return rangeInitializer ?? (rangeInitializer = GetRangeInitializer()); }
		}
		static RangeAttributeCtor rangeInitializer;
		static RangeAttributeCtor GetRangeInitializer() {
			Type attrType = DataAssemblyHelper.DataAssembly.GetType(FilterRangeAttributeName);
			var pMinOrMinMember = Expression.Parameter(typeof(object), "minOrMinMember");
			var pMaxOrMaxMember = Expression.Parameter(typeof(object), "maxOrMaxMember");
			var pAvgOrAvgMember = Expression.Parameter(typeof(object), "avgOrAvgMember");
			var ctorExpression = Expression.New(
				attrType.GetConstructor(new Type[] { typeof(object), typeof(object), typeof(object) }),
				pMinOrMinMember, pMaxOrMaxMember, pAvgOrAvgMember);
			return Expression.Lambda<RangeAttributeCtor>(ctorExpression, 
				pMinOrMinMember, pMaxOrMaxMember, pAvgOrAvgMember).Compile();
		}
#endregion
#region DateTimeRange Initializer
		protected delegate Attribute DateTimeRangeAttributeEmptyCtor();
		protected delegate Attribute DateTimeRangeAttributeCtor(string minOrMinMember, string maxOrMaxMember);
		protected static DateTimeRangeAttributeEmptyCtor DateTimeRangeEmptyInitializer {
			get { return dateTimeRangeEmptyInitializer ?? (dateTimeRangeEmptyInitializer = GetDateTimeRangeEmptyInitializer()); }
		}
		protected static DateTimeRangeAttributeCtor DateTimeRangeInitializer {
			get { return dateTimeRangeInitializer ?? (dateTimeRangeInitializer = GetDateTimeRangeInitializer()); }
		}
		static DateTimeRangeAttributeEmptyCtor dateTimeRangeEmptyInitializer;
		static DateTimeRangeAttributeCtor dateTimeRangeInitializer;
		static DateTimeRangeAttributeEmptyCtor GetDateTimeRangeEmptyInitializer() {
			Type attrType = DataAssemblyHelper.DataAssembly.GetType(FilterDateTimeRangeAttributeName);
			var ctorExpression = Expression.New(
				attrType.GetConstructor(new Type[] { }));
			return Expression.Lambda<DateTimeRangeAttributeEmptyCtor>(ctorExpression).Compile();
		}
		static DateTimeRangeAttributeCtor GetDateTimeRangeInitializer() {
			Type attrType = DataAssemblyHelper.DataAssembly.GetType(FilterDateTimeRangeAttributeName);
			var pMinOrMinMember = Expression.Parameter(typeof(string), "minOrMinMember");
			var pMaxOrMaxMember = Expression.Parameter(typeof(string), "maxOrMaxMember");
			var ctorExpression = Expression.New(
				attrType.GetConstructor(new Type[] { typeof(string), typeof(string) }),
				pMinOrMinMember, pMaxOrMaxMember);
			return Expression.Lambda<DateTimeRangeAttributeCtor>(ctorExpression, 
				pMinOrMinMember, pMaxOrMaxMember).Compile();
		}
#endregion
#region Lookup Initializer
		protected delegate Attribute LookupAttributeCtor(object dataSourceOrDataSourceMember, int top, int maxCount);
		protected static LookupAttributeCtor LookupInitializer {
			get { return lookupInitializer ?? (lookupInitializer = GetLookupInitializer()); }
		}
		static LookupAttributeCtor lookupInitializer;
		static LookupAttributeCtor GetLookupInitializer() {
			Type attrType = DataAssemblyHelper.DataAssembly.GetType(FilterLookupAttributeName);
			var pDataSourceOrDataSourceMember = Expression.Parameter(typeof(object), "dataSourceOrDataSourceMember");
			var pTop = Expression.Parameter(typeof(int), "top");
			var pMaxCount = Expression.Parameter(typeof(int), "maxCount");
			var ctorExpression = Expression.New(
				attrType.GetConstructor(new Type[] { typeof(object), typeof(int), typeof(int) }),
				pDataSourceOrDataSourceMember, pTop, pMaxCount);
			return Expression.Lambda<LookupAttributeCtor>(ctorExpression,
				pDataSourceOrDataSourceMember, pTop, pMaxCount).Compile();
		}
#endregion
#region BooleanChoice Initializer
		protected delegate Attribute BooleanChoiceAttributeCtor();
		protected delegate Attribute BooleanChoiceWithDefaultValueAttributeCtor(bool defaultValue);
		protected static BooleanChoiceAttributeCtor BooleanChoiceInitializer {
			get { return booleanChoiceInitializer ?? (booleanChoiceInitializer = GetBooleanChoiceInitializer()); }
		}
		protected static BooleanChoiceWithDefaultValueAttributeCtor BooleanChoiceWithDefaultValueInitializer {
			get { return booleanChoiceWithDefaultValueInitializer ?? (booleanChoiceWithDefaultValueInitializer = GetBooleanChoiceWithDefaultValueInitializer()); }
		}
		static BooleanChoiceAttributeCtor booleanChoiceInitializer;
		static BooleanChoiceWithDefaultValueAttributeCtor booleanChoiceWithDefaultValueInitializer;
		static BooleanChoiceAttributeCtor GetBooleanChoiceInitializer() {
			Type attrType = DataAssemblyHelper.DataAssembly.GetType(FilterBooleanChoiceAttributeName);
			var ctorExpression = Expression.New(
				attrType.GetConstructor(new Type[] {  }));
			return Expression.Lambda<BooleanChoiceAttributeCtor>(ctorExpression).Compile();
		}
		static BooleanChoiceWithDefaultValueAttributeCtor GetBooleanChoiceWithDefaultValueInitializer() {
			Type attrType = DataAssemblyHelper.DataAssembly.GetType(FilterBooleanChoiceAttributeName);
			var pDefaultValue = Expression.Parameter(typeof(bool), "defaultValue");
			var ctorExpression = Expression.New(attrType.GetConstructor(new Type[] { typeof(bool) }), pDefaultValue);
			return Expression.Lambda<BooleanChoiceWithDefaultValueAttributeCtor>(ctorExpression, pDefaultValue).Compile();
		}
		#endregion
		#region EnumChoice Initializer
		protected delegate Attribute EnumChoiceAttributeEmptyCtor();
		protected delegate Attribute EnumChoiceAttributeCtor(bool useFlags);
		protected static EnumChoiceAttributeEmptyCtor EnumChoiceEmptyInitializer {
			get { return enumChoiceEmptyInitializer ?? (enumChoiceEmptyInitializer = GetEnumChoiceEmptyInitializer()); }
		}
		protected static EnumChoiceAttributeCtor EnumChoiceInitializer {
			get { return enumChoiceInitializer ?? (enumChoiceInitializer = GetEnumChoiceInitializer()); }
		}
		static EnumChoiceAttributeEmptyCtor enumChoiceEmptyInitializer;
		static EnumChoiceAttributeCtor enumChoiceInitializer;
		static EnumChoiceAttributeEmptyCtor GetEnumChoiceEmptyInitializer() {
			Type attrType = DataAssemblyHelper.DataAssembly.GetType(FilterEnumChoiceAttributeName);
			var ctorExpression = Expression.New(
				attrType.GetConstructor(new Type[] { }));
			return Expression.Lambda<EnumChoiceAttributeEmptyCtor>(ctorExpression).Compile();
		}
		static EnumChoiceAttributeCtor GetEnumChoiceInitializer() {
			Type attrType = DataAssemblyHelper.DataAssembly.GetType(FilterEnumChoiceAttributeName);
			var pUseFlags = Expression.Parameter(typeof(bool), "useFlags");
			var ctorExpression = Expression.New(
				attrType.GetConstructor(new Type[] { typeof(bool) }),
				pUseFlags);
			return Expression.Lambda<EnumChoiceAttributeCtor>(ctorExpression,
				pUseFlags).Compile();
		}
#endregion
		Attribute IAttributeProxy.CreateRealAttribute() {
			return CreateRealAttribute();
		}
		protected abstract Attribute CreateRealAttribute();
		protected void SetProperty<T>(Attribute attr, Expression<Func<T>> property, T value) {
			TypeDescriptor.GetProperties(attr)[BindableBase.GetPropertyName(property)].SetValue(attr, value);
		}
		protected void SetProperty<T>(Attribute attr, string property, T value) {
			TypeDescriptor.GetProperties(attr)[property].SetValue(attr, value);
		}
	}
	public abstract class BaseFilterRangeAttributeProxy : FilterAttributeProxy {
		public object MinOrMinMember { get; set; }
		public object MaxOrMaxMember { get; set; }
		public string FromName { get; set; }
		public string ToName { get; set; }
	}
	public class FilterRangeAttributeProxy : BaseFilterRangeAttributeProxy {
		public FilterRangeUIEditorType EditorType { get; set; }
		protected override Attribute CreateRealAttribute() {
			var attr = RangeInitializer(MinOrMinMember, MaxOrMaxMember, null);
			SetProperty(attr, () => FromName, FromName);
			SetProperty(attr, () => ToName, ToName);
			SetProperty(attr, () => EditorType, EditorType);
			return attr;
		}
	}
	public class FilterDateTimeRangeAttributeProxy : BaseFilterRangeAttributeProxy {
		public FilterDateTimeRangeUIEditorType EditorType { get; set; }
		protected override Attribute CreateRealAttribute() {
			Attribute attr;
			if(MinOrMinMember is string && MaxOrMaxMember is string)
				attr = DateTimeRangeInitializer((string)MinOrMinMember, (string)MaxOrMaxMember);
			else {
				attr = DateTimeRangeEmptyInitializer();
				SetProperty(attr, "Minimum", MinOrMinMember);
				SetProperty(attr, "Maximum", MaxOrMaxMember);
			}
			SetProperty(attr, () => FromName, FromName);
			SetProperty(attr, () => ToName, ToName);
			SetProperty(attr, () => EditorType, EditorType);
			return attr;
		}
	}
	public abstract class BaseFilterLookupAttributeProxy : FilterAttributeProxy {
		public FilterLookupUIEditorType EditorType { get; set; }
		public bool? UseFlags { get; set; }
		public bool? UseSelectAll { get; set; }
		public string SelectAllName { get; set; }
	}
	public class FilterLookupAttributeProxy : BaseFilterLookupAttributeProxy {
		public object DataSourceOrDataSourceMember { get; set; }
		public string ValueMember { get; set; }
		public string DisplayMember { get; set; }
		public object TopOrTopMember { get; set; }
		public object MaxCountOrMaxCountMember { get; set; }
		protected override Attribute CreateRealAttribute() {
			Attribute attr;
			if(TopOrTopMember is int && MaxCountOrMaxCountMember is int)
				attr = LookupInitializer(DataSourceOrDataSourceMember, (int)TopOrTopMember, (int)MaxCountOrMaxCountMember);
			else {
				attr = LookupInitializer(DataSourceOrDataSourceMember, 0, 0);
				if(TopOrTopMember is string) SetProperty(attr, "TopMember", TopOrTopMember);
				if(MaxCountOrMaxCountMember is string) SetProperty(attr, "MaxCountMember", MaxCountOrMaxCountMember);
			}
			SetProperty(attr, () => EditorType, EditorType);
			SetProperty(attr, () => ValueMember, ValueMember);
			SetProperty(attr, () => DisplayMember, DisplayMember);
			SetProperty(attr, () => SelectAllName, SelectAllName);
			if(UseFlags.HasValue) SetProperty(attr, () => UseFlags, UseFlags.Value);
			if(UseSelectAll.HasValue) SetProperty(attr, () => UseSelectAll, UseSelectAll.Value);
			return attr;
		}
	}
	public class FilterEnumChoiceAttributeProxy : BaseFilterLookupAttributeProxy {
		protected override Attribute CreateRealAttribute() {
			Attribute attr;
			if(UseFlags.HasValue)
				attr = EnumChoiceInitializer(UseFlags.Value);
			else attr = EnumChoiceEmptyInitializer();
			SetProperty(attr, () => EditorType, EditorType);
			SetProperty(attr, () => SelectAllName, SelectAllName);
			if(UseSelectAll.HasValue) SetProperty(attr, () => UseSelectAll, UseSelectAll.Value);
			return attr;
		}
	}
	public class FilterBooleanChoiceAttributeProxy : FilterAttributeProxy {
		public FilterBooleanUIEditorType EditorType { get; set; }
		public string TrueName { get; set; }
		public string FalseName { get; set; }
		public string DefaultName { get; set; }
		public bool? DefaultValue { get; set; }
		public string DefaultValueMember { get; set; }
		protected override Attribute CreateRealAttribute() {
			Attribute attr;
			if(DefaultValue == null)
				attr = BooleanChoiceInitializer();
			else attr = BooleanChoiceWithDefaultValueInitializer(DefaultValue.Value);
			SetProperty(attr, () => EditorType, EditorType);
			SetProperty(attr, () => TrueName, TrueName);
			SetProperty(attr, () => FalseName, FalseName);
			SetProperty(attr, () => DefaultName, DefaultName);
			SetProperty(attr, () => DefaultValueMember, DefaultValueMember);
			return attr;
		}
	}
}
#else
namespace DevExpress.Utils.Filtering {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Globalization;
	using System.Linq.Expressions;
	using Internal;
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class FilterMetadataTypeAttribute : Attribute {
		public FilterMetadataTypeAttribute(Type metadataClassType) {
			this.MetadataClassType = metadataClassType;
		}
		public Type MetadataClassType { get; private set; }
	}
	public enum RangeUIEditorType {
		Default,
		Range,
		Text,
		Spin
	}
	public enum DateTimeRangeUIEditorType {
		Default,
		Picker,
		Range,
		Calendar
	}
	public enum LookupUIEditorType {
		Default,
		List,
		DropDown,
		TokenBox
	}
	public enum BooleanUIEditorType {
		Default,
		Check,
		Toggle,
		List,
		DropDown
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public abstract class FilterAttribute : Attribute {
		internal readonly static object[] EmptyValues = new object[0];
		internal readonly static string[] EmptyMembers = new string[0];
		protected internal virtual string[] GetMembers() { return EmptyMembers; }
	}
	public abstract class FilterAttributeLocalizable : FilterAttribute {
		readonly Dictionary<string, LocalizableString> localizableStrings = new Dictionary<string, LocalizableString>();
		protected FilterAttributeLocalizable() {
			foreach(Expression<Func<string>> propertySelector in GetLocalizableProperties())
				RegisterLocalizableProperty(propertySelector);
		}
		protected abstract IEnumerable<Expression<Func<string>>> GetLocalizableProperties();
		void RegisterLocalizableProperty(Expression<Func<string>> propertySelector) {
			localizableStrings.Add(ExpressionHelper.GetPropertyName(propertySelector), new LocalizableString(propertySelector));
		}
		Type resourceTypeCore;
		public Type ResourceType {
			get { return resourceTypeCore; }
			set {
				if(resourceTypeCore == value) return;
				resourceTypeCore = value;
				OnResourceTypeChanged(value);
			}
		}
		void OnResourceTypeChanged(Type value) {
			foreach(LocalizableString locString in localizableStrings.Values)
				locString.ResourceType = value;
		}
		protected string GetLocalizableValue(Expression<Func<string>> propertySelector) {
			return localizableStrings[ExpressionHelper.GetPropertyName(propertySelector)].GetLocalizableValue();
		}
		protected string GetLocalizablePropertyValue(Expression<Func<string>> propertySelector) {
			return localizableStrings[ExpressionHelper.GetPropertyName(propertySelector)].Value;
		}
		protected void SetLocalizablePropertyValue(Expression<Func<string>> propertySelector, string value) {
			localizableStrings[ExpressionHelper.GetPropertyName(propertySelector)].Value = value;
		}
	}
	public abstract class BaseFilterRangeAttribute : FilterAttributeLocalizable {
		protected BaseFilterRangeAttribute() : this(null, null) { }
		protected BaseFilterRangeAttribute(object minOrMinMember = null, object maxOrMaxMember = null)
			: base() {
			if(minOrMinMember is string && !TryParse((string)minOrMinMember, out minOrMinMember))
				this.MinimumMember = (string)minOrMinMember;
			else
				this.Minimum = minOrMinMember;
			if(maxOrMaxMember is string && !TryParse((string)maxOrMaxMember, out maxOrMaxMember))
				this.MaximumMember = (string)maxOrMaxMember;
			else
				this.Maximum = maxOrMaxMember;
		}
		protected virtual bool TryParse(string str, out object value) {
			value = str;
			return false;
		}
		public string FromName {
			get { return GetLocalizablePropertyValue(() => FromName); }
			set { SetLocalizablePropertyValue(() => FromName, value); }
		}
		public string ToName {
			get { return GetLocalizablePropertyValue(() => ToName); }
			set { SetLocalizablePropertyValue(() => ToName, value); }
		}
		public string GetFromName() {
			return GetLocalizableValue(() => FromName);
		}
		public string GetToName() {
			return GetLocalizableValue(() => ToName);
		}
		protected override IEnumerable<Expression<Func<string>>> GetLocalizableProperties() {
			return new Expression<Func<string>>[] {
						() => FromName,
						() => ToName
					};
		}
		public object Minimum { get; protected set; }
		public object Maximum { get; protected set; }
		public string MinimumMember { get; set; }
		public string MaximumMember { get; set; }
		protected internal override string[] GetMembers() {
			return new string[] { MinimumMember, MaximumMember, null };
		}
	}
	public sealed class FilterRangeAttribute : BaseFilterRangeAttribute {
		public FilterRangeAttribute()
			: this(null, null, null) {
		}
		public FilterRangeAttribute(object minOrMinMember, object maxOrMaxMember)
			: this(minOrMinMember, maxOrMaxMember, null) {
		}
		public FilterRangeAttribute(object minOrMinMember, object maxOrMaxMember, object avgOrAvgMember)
			: base(minOrMinMember, maxOrMaxMember) {
			if(avgOrAvgMember is string && !TryParse((string)avgOrAvgMember, out avgOrAvgMember))
				this.AverageMember = (string)avgOrAvgMember;
			else
				this.Average = avgOrAvgMember;
		}
		public RangeUIEditorType EditorType { get; set; }
		public object Average { get; private set; }
		public string AverageMember { get; set; }
		protected internal override string[] GetMembers() {
			return new string[] { MinimumMember, MaximumMember, AverageMember };
		}
	}
	public sealed class FilterDateTimeRangeAttribute : BaseFilterRangeAttribute {
		public FilterDateTimeRangeAttribute()
			: this(null, null) {
		}
		public FilterDateTimeRangeAttribute(string minOrMinMember, string maxOrMaxMember)
			: base(minOrMinMember, maxOrMaxMember) {
		}
		protected override bool TryParse(string str, out object value) {
			DateTime resultDateTime;
			if(DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out resultDateTime)) {
				value = resultDateTime;
				return true;
			}
			TimeSpan resultTimeSpan;
			if(TimeSpan.TryParse(str, CultureInfo.InvariantCulture, out resultTimeSpan)) {
				value = resultTimeSpan;
				return true;
			}
			return base.TryParse(str, out value);
		}
		public DateTimeRangeUIEditorType EditorType { get; set; }
		public new object Minimum {
			get { return base.Minimum; }
			set { base.Minimum = value; }
		}
		public new object Maximum {
			get { return base.Maximum; }
			set { base.Maximum = value; }
		}
	}
	public abstract class BaseFilterLookupAttribute : FilterAttributeLocalizable {
		public LookupUIEditorType EditorType {
			get;
			set;
		}
		bool? useFlags;
		protected bool UseFlagsCore {
			get { return useFlags ?? true; }
			set { useFlags = value; }
		}
		protected internal bool? ActualUseFlags {
			get { return useFlags; }
		}
		bool? useSelectAll;
		public bool UseSelectAll {
			get { return useSelectAll ?? true; }
			set { useSelectAll = value; }
		}
		protected internal bool? ActualUseSelectAll {
			get { return useSelectAll; }
		}
		public string SelectAllName {
			get { return GetLocalizablePropertyValue(() => SelectAllName); }
			set { SetLocalizablePropertyValue(() => SelectAllName, value); }
		}
		public string GetSelectAllName() {
			return GetLocalizableValue(() => SelectAllName);
		}
		public string NullName {
			get { return GetLocalizablePropertyValue(() => NullName); }
			set { SetLocalizablePropertyValue(() => NullName, value); }
		}
		public string GetNullName() {
			return GetLocalizableValue(() => NullName);
		}
		protected override IEnumerable<Expression<Func<string>>> GetLocalizableProperties() {
			return new Expression<Func<string>>[] {
						() => SelectAllName,
						() => NullName
					};
		}
	}
	public sealed class FilterLookupAttribute : BaseFilterLookupAttribute {
		public FilterLookupAttribute()
			: this(null, 0, 0) {
		}
		public FilterLookupAttribute(int top, int maxCount = 0)
			: this(null, top, maxCount) {
		}
		public FilterLookupAttribute(string topOrTopMember, string maxCountOrMaxCountMember) {
			int topValue;
			if(!int.TryParse(topOrTopMember, NumberStyles.Integer, CultureInfo.InvariantCulture, out topValue))
				this.TopMember = topOrTopMember;
			else
				this.Top = topValue;
			int maxCountValue;
			if(!int.TryParse(maxCountOrMaxCountMember, NumberStyles.Integer, CultureInfo.InvariantCulture, out maxCountValue))
				this.MaxCountMember = maxCountOrMaxCountMember;
			else
				this.MaxCount = maxCountValue;
		}
		public FilterLookupAttribute(object dataSourceOrDataSourceMember, int top = 0, int maxCount = 0) {
			if(dataSourceOrDataSourceMember is string)
				this.DataSourceMember = (string)dataSourceOrDataSourceMember;
			else
				this.DataSource = dataSourceOrDataSourceMember;
			this.Top = (top == 0) ? null : (maxCount > 0) ? new Nullable<int>(Math.Min(top, maxCount)) : top;
			this.MaxCount = (maxCount == 0) ? null : new Nullable<int>(Math.Max(top, maxCount));
		}
		[Browsable(false)]
		public object DataSource { get; private set; }
		public int? Top { get; private set; }
		public int? MaxCount { get; private set; }
		public bool UseFlags {
			get { return UseFlagsCore; }
			set { UseFlagsCore = value; }
		}
		public string DataSourceMember { get; set; }
		public string ValueMember { get; set; }
		public string DisplayMember { get; set; }
		public string TopMember { get; set; }
		public string MaxCountMember { get; set; }
		protected internal override string[] GetMembers() {
			return new string[] { DataSourceMember, TopMember, MaxCountMember };
		}
	}
	public sealed class FilterBooleanChoiceAttribute : FilterAttributeLocalizable {
		public FilterBooleanChoiceAttribute() { }
		public FilterBooleanChoiceAttribute(bool defaultValue) {
			this.DefaultValue = defaultValue;
		}
		public FilterBooleanChoiceAttribute(string defaultValueOrDefaultValueMember) {
			bool value;
			if(!bool.TryParse((string)defaultValueOrDefaultValueMember, out value))
				this.DefaultValueMember = defaultValueOrDefaultValueMember;
			else
				this.DefaultValue = value;
		}
		public BooleanUIEditorType EditorType { get; set; }
		public bool? DefaultValue {
			get;
			private set;
		}
		public string DefaultValueMember { get; set; }
		public string TrueName {
			get { return GetLocalizablePropertyValue(() => TrueName); }
			set { SetLocalizablePropertyValue(() => TrueName, value); }
		}
		public string FalseName {
			get { return GetLocalizablePropertyValue(() => FalseName); }
			set { SetLocalizablePropertyValue(() => FalseName, value); }
		}
		public string DefaultName {
			get { return GetLocalizablePropertyValue(() => DefaultName); }
			set { SetLocalizablePropertyValue(() => DefaultName, value); }
		}
		public string GetTrueName() {
			return GetLocalizableValue(() => TrueName);
		}
		public string GetFalseName() {
			return GetLocalizableValue(() => FalseName);
		}
		public string GetDefaultName() {
			return GetLocalizableValue(() => DefaultName);
		}
		protected override IEnumerable<Expression<Func<string>>> GetLocalizableProperties() {
			return new Expression<Func<string>>[] {
						() => TrueName,
						() => FalseName,
						() => DefaultName
					};
		}
		protected internal override string[] GetMembers() {
			return new string[] { DefaultValueMember };
		}
	}
	public sealed class FilterEnumChoiceAttribute : BaseFilterLookupAttribute {
		public FilterEnumChoiceAttribute() { }
		public FilterEnumChoiceAttribute(bool useFlags) {
			this.UseFlagsCore = useFlags;
		}
	}
}
#endif
