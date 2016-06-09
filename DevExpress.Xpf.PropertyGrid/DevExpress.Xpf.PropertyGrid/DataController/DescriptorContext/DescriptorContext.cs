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

#if SL
using DevExpress.Data.Browsing;
#else
using System.ComponentModel;
using System.Drawing.Design;
#endif
using System;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Xpf.PropertyGrid;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.PropertyGrid.Internal;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using System.Collections.Specialized;
using DevExpress.Xpf.Core.Mvvm.UI.ViewGenerator.Metadata;
using DevExpress.Entity.Model;
using DataController = DevExpress.Xpf.PropertyGrid.Internal.DataController;
using DevExpress.Xpf.Core;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.XtraVerticalGrid.Data {
	public abstract class DescriptorContext : System.ComponentModel.ITypeDescriptorContext {
		public static bool DefaultIsExpanded = false;
		object multiInstance;
		object value;
		bool hasValue = false;
		RowHandle rowHandle;
		AttributeCollection propertyAttributes;
		AttributeCollection typeAttributes;
		Dictionary<object, PropertyDescriptorCollection> properties;
		PropertyDescriptor propertyDescriptor;
		HybridDictionary childHandles;
		IEnumerable<RowHandle> visibleHandles;
		IList<RowHandle> categoryHandles;
		bool? isGetPropertiesSupported;
		bool? shouldSerializeValue;
		bool? getCreateInstanceSupported;
		bool? isExpanded;
		TypeConverter converter;
		TypeConverter baseConverter;
		bool isValid = true;
		ICollection standardValues;
		bool? isStandardValuesSupported;
		bool? isStandardValuesExclusive;
		string displayName;
		Lazy<IEdmPropertyInfo> edmPropertyInfo;
		Lazy<string> categoryName;
		Lazy<IEnumerable<string>> validationError;
		Lazy<bool> supportDataAnnotation;
		Lazy<IDataErrorInfo> dataErrorInfo;
		CollectionHelper collectionHelper;
		Lazy<IInstanceInitializer> instanceInitializer;
		Lazy<PropertyValidator> propertyValidator;
		public DescriptorContext(IServiceProvider serviceProvider, bool isMultiSource) {
			Reset();
			this.edmPropertyInfo = new Lazy<IEdmPropertyInfo>(() => GetEdmPropertyInfo());
			IsMultiSource = isMultiSource;
			ServiceProvider = serviceProvider;
		}
		~DescriptorContext() {
			if (handler == null || PropertyDescriptor == null)
				return;
			PropertyDescriptor.RemoveValueChanged(MultiInstance, handler.Handler);
		}
		PropertyDescriptorValueChangedEventHandler handler;
		internal void SubscribeValueChanged() {
			if (handler == null)
				handler = new PropertyDescriptorValueChangedEventHandler(this, (d, o, e) => {
					d.DataController.Invalidate(d.RowHandle);
				});
			PropertyDescriptor.AddValueChanged(MultiInstance, handler.Handler);
		}
		DescriptorContext parentContext;
		public DescriptorContext ParentContext {
			get { return parentContext; }
			internal set {
				parentContext = value;
				ParentContextChanged();
			}
		}
		protected virtual void ParentContextChanged() {
			ParentContext.Do(x => x.AddChildHandle(this));
		}
		protected void AddChildHandle(DescriptorContext context) {
			this.visibleHandlesLocker.DoLockedActionIfNotLocked(() => {
				RowHandle handle = context.RowHandle;
				if (handle.IsInvalid || ForcedChildHandles.Contains(handle))
					return;
				this.childHandles.Add(handle, null);
			});
		}
		public AttributeCollection PropertyAttributes {
			get {
				if (propertyAttributes == null) {
					propertyAttributes = PropertyDescriptor.Return(x => x.Attributes, null);
				}
				return propertyAttributes;
			}
		}
		public AttributeCollection TypeAttributes {
			get {
				if (typeAttributes == null) {
					typeAttributes = TypeDescriptor.GetAttributes(PropertyType);
				}
				return typeAttributes;
			}
		}
		public virtual object MultiInstance {
			get { return multiInstance; }
			internal set {
				if (multiInstance != null)
					throw new Exception();
				multiInstance = value;
				MultiInstanceChanged();
			}
		}
		public virtual object Instance { get { return IsMultiSource ? GetInstanceFromMultiInstance() : MultiInstance; } }
		protected internal virtual void ContextAdded() {
		}
		protected virtual void MultiInstanceChanged() {
		}
		object GetInstanceFromMultiInstance() {
			if (MultiInstance == null)
				return null;
			object[] obj = MultiInstance as object[];
			if (obj == null)
				return null;
			return obj[0];
		}
		public CollectionHelper CollectionHelper {
			get {
				if (collectionHelper == null) {
					collectionHelper = GetCollectionHelper();
				}
				return collectionHelper;
			}
		}
		CollectionHelper GetCollectionHelper() {
			if (Converter is ListConverter)
				return new CollectionHelper(this);
			return NullCollectionHelper.Instance;
		}
		public virtual PropertyDescriptor PropertyDescriptor {
			get { return propertyDescriptor; }
			internal set {
				if (propertyDescriptor != null)
					throw new ArgumentException("Try to change PropertyDescriptor");
				propertyDescriptor = value;
				PropertyDescriptorChanged();
			}
		}
		public bool IsExpanded {
			get {
				if (isExpanded == null) {
					isExpanded = DataController == null ? false : DataController.VisualClient.IsExpanded(RowHandle);
				}
				return isExpanded.Value;
			}
		}
		public virtual bool CanExpand {
			get {
				if (Value == null)
					return false;
				if (!IsGetPropertiesSupported)
					return false;
				return  ChildHandles.Any();
			}
		}
		public RowHandle RowHandle {
			get { return rowHandle; }
			internal set {
				if(rowHandle == value)
					return;
				rowHandle = value;
			}
		}
		internal TypeInfo NewListItemValue { get; set; }
		public string FieldName { get; set; }
		public virtual string CategoryName { get { return categoryName.Value; } }
		public virtual PropertyValidator EdmValidator { get { return propertyValidator.Value; } }
		public IEnumerable<string> ValidationError { get { return validationError.Value; } }
		public virtual string Name { get { return PropertyDescriptor.Return(x => x.Name, () => PropertyHelper.RootPropertyName); } }
		public virtual string DisplayName {
			get {
				if (displayName == null)
					displayName = GetDisplayName();
				return displayName;
			}
		}
		public IInstanceInitializer InstanceInitializer { get { return instanceInitializer.Value; } }
		public virtual bool IsLoaded { get { return visibleHandles != null || IsExpanded; } }
		public IEnumerable<RowHandle> ChildHandles {
			get {
				if (visibleHandles == null) {
					visibleHandles = CreateVisibleHandles();
					VisibleHandlesCreated();
				}
				return visibleHandles;
			}
		}
		HybridDictionary ForcedChildHandles {
			get {
				if (childHandles == null) {
					childHandles = new HybridDictionary();
				}
				return childHandles;
			}
		}
		public IEnumerable<RowHandle> AllChildHandles {
			get { return ChildHandles.Union(ForcedChildHandles.Keys.Cast<RowHandle>()); }
		}
		protected virtual void VisibleHandlesCreated() {
			if (!isExpanded.HasValue) {
				this.isExpanded = DefaultIsExpanded;
			}
		}
		public IList<RowHandle> CategoryHandles {
			get {
				if (categoryHandles == null) {
					categoryHandles = CreateCategoryHandles();
				}
				return categoryHandles;
			}
		}
		public bool IsValid { get { return isValid; } }
		public bool IsStandardValuesExclusive {
			get {
				if (isStandardValuesExclusive == null)
					isStandardValuesExclusive = GetStandardValuesExclusive();
				return isStandardValuesExclusive.Value;
			}
		}
		public bool IsStandardValuesSupported {
			get {
				if (isStandardValuesSupported == null)
					isStandardValuesSupported = GetStandardValuesSupported();
				return isStandardValuesSupported.Value;
			}
		}
		public ICollection StandardValues {
			get {
				if (standardValues == null)
					standardValues = GetStandardValues();
				return standardValues;
			}
		}
		internal protected IServiceProvider ServiceProvider { get; set; }
		internal protected bool IsMultiSource { get; set; }
		internal DataController DataController { get { return ServiceProvider as DataController; } }
		internal protected IEdmPropertyInfo AssignedEdmPropertyInfo { get; set; }
		public IEdmPropertyInfo EdmPropertyInfo { get { return edmPropertyInfo.Value; } }
		bool IsDefaultTypeConverter { get { return BaseConverter == null || (BaseConverter == Converter && BaseConverter.GetType() == typeof(TypeConverter)); } }
		internal protected virtual void SetIsExpandedInternal(bool? isExpanded) {
			this.isExpanded = isExpanded;
		}
		public bool SetIsExpanded(bool isExpanded) {
			bool needUpdateChildren = IsExpanded != isExpanded;
			if (!CanExpand)
				return false;
			SetIsExpandedInternal(isExpanded);
			return needUpdateChildren;
		}
		protected virtual string GetCategoryName() {
			string name = EdmPropertyInfo.Return(x => x.Attributes.GroupName, () => null);
			if (!string.IsNullOrEmpty(name))
				return name;
			return PropertyDescriptor.Return(x => x.Category, () => null);
		}
		protected virtual bool GetStandardValuesExclusive() {
			return Converter.Return(x => x.GetStandardValuesExclusive(this), () => false);
		}
		protected virtual bool GetStandardValuesSupported() {
			return Converter.Return(x => x.GetStandardValuesSupported(this), () => false);
		}
		protected virtual ICollection GetStandardValues() {
			if (IsStandardValuesSupported)
				return Converter.Return(x => x.GetStandardValues(this), () => new List<object>() as ICollection);
			return new List<object>();
		}
		protected virtual IList<RowHandle> CreateCategoryHandles() {
			if (!PropertyHelper.IsRoot(FieldName))
				return new List<RowHandle>();
			IList<string> categoryNames = new List<string>();
			foreach (RowHandle handle in ChildHandles) {
				string categoryName = GetContext(handle).CategoryName;
				if (!categoryNames.Contains(categoryName))
					categoryNames.Add(categoryName);
			}
			IList<RowHandle> handles = new List<RowHandle>();
			foreach (string categoryName in categoryNames) {
				RowHandle handle = CreateCategory(categoryName);
				DescriptorContext context = CreateGroupDescriptorContext(MultiInstance, categoryName, FieldName);
				context.RowHandle = handle;
				context.ParentContext = this;
				DataController.ContextCache[IsMultiSource, context.FieldName] = context;
				DataController.ContextCache[IsMultiSource, handle] = context;
				context.ContextAdded();
				handles.Add(handle);
			}
			return handles;
		}
		protected virtual DescriptorContext CreateGroupDescriptorContext(object component, string categoryName, string fieldName) {
			return new GroupDescriptorContext(component, categoryName, ServiceProvider, IsMultiSource, fieldName);
		}
		void PropertyDescriptorChanged() {
			if (PropertyDescriptor == null || !PropertyDescriptor.SupportsChangeEvents)
				return;
			SubscribeValueChanged();
		}
		RowHandle CreateCategory(string categoryName) {
			return new RowHandle();
		}
		Locker visibleHandlesLocker = new Locker();
		protected virtual IEnumerable<RowHandle> CreateVisibleHandles() {
			if (DataController == null)
				return EmptyEnumerable<RowHandle>.Instance;
			List<RowHandle> visibleHandles = new List<RowHandle>();
			visibleHandlesLocker.DoLockedAction(() => {
				PropertyDescriptorCollection properties = GetProperties(DataController.BrowsableAttributes);
				if (properties == null || Value == null)
					return;
				IEntityProperties entityProperties = new ReflectionEntityProperties(properties.Cast<PropertyDescriptor>(), Value.GetType(), true);
				IEnumerable<IEdmPropertyInfo> edmProperties = EditorsGeneratorBase.GetFilteredAndSortedProperties(entityProperties.AllProperties, false, true, DevExpress.Mvvm.Native.LayoutType.Default);
				foreach (IEdmPropertyInfo edmPropertyInfo in edmProperties) {
					RowHandle handle = AddProperty(edmPropertyInfo);
					if (handle != null)
						visibleHandles.Add(handle);
				}
			});
			return visibleHandles;
		}
		RowHandle AddProperty(IEdmPropertyInfo edmPropertyInfo) {
			PropertyDescriptor descriptor = (PropertyDescriptor)edmPropertyInfo.ContextObject;
#if !SL
			if (!DataController.ShowAttachedProperties && IsAttachedProperty(descriptor) || edmPropertyInfo.Attributes.Hidden())
				return null;
#endif
			if (IsMultiSource && !DataController.ShouldIncludeMultiProperty(this, descriptor))
				return null;
			DescriptorContext context = DataController.GetDescriptorContext(this, descriptor);
			if (context == null)
				return null;
			context.AssignedEdmPropertyInfo = edmPropertyInfo;
			return context.RowHandle;
		}
		internal protected virtual string GetFieldNameForChild() {
			return FieldName;
		}
		internal static bool IsAttachedProperty(PropertyDescriptor descriptor) {
			if (descriptor==null || descriptor.GetType().Name != "DependencyObjectPropertyDescriptor")
				return false;
			return DependencyPropertyDescriptor.FromProperty(descriptor).IsAttached;
		}
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
			return GetProperties(Value, attributes);
		}
		public virtual AllowExpandingMode AllowExpandingMode {
			get {
				return DataController.Return(x => x.VisualClient.GetAllowExpandingMode(RowHandle), () => AllowExpandingMode.Default);
			}
		}
		public virtual bool IsGetPropertiesSupported {
			get {
				if(isGetPropertiesSupported == null) {
					isGetPropertiesSupported = GetIsGetPropertiesSupported();
				}
				return isGetPropertiesSupported.Value;
			}
		}
		public object Value {
			get {
				if(!this.hasValue) {
					this.hasValue = true;
					value = GetValue();
					SubscribeNotifications();
				}
				return value;
			}
		}
		public TypeConverter Converter {
			get {
				if(converter == null) {
					converter = GetConverter();
				}
				return converter;
			}
		}
		public TypeConverter BaseConverter {
			get {
				if (baseConverter == null) {
					baseConverter = GetBaseConverter();
				}
				return baseConverter;
			}
		}
		public bool GetCreateInstanceSupported {
			get {
				if (getCreateInstanceSupported == null)
					getCreateInstanceSupported = GetGetCreateInstanceSupported();
				return getCreateInstanceSupported.Value;
			}
		}
		public virtual IInstanceInitializer GetInstanceInitializer() {
			IInstanceInitializer initializer = EdmPropertyInfo.Return(e => e.Attributes.InstanceInitializer(), () => null);
			if (initializer != null)
				return initializer;
			return DataController.VisualClient.GetInstanceInitializer(RowHandle);
		}
		public RowHandle SetValue(object value) {
			object settingValue = TryConvertFromDifferentType(value);
			DoValidate(settingValue);
			BeforeSetValue(value, settingValue);
			return SetValueInternal(settingValue);
		}
		protected virtual void DoValidate(object settingValue) {
			if (DataController.VisualClient.AllowCommitOnValidationAttributeError() || !SupportDataAnnotations)
				return;
			string errorText = EdmValidator.GetErrorText(settingValue, Instance);
			if (!string.IsNullOrEmpty(errorText))
				throw new Exception(errorText);
		}
		protected virtual void BeforeSetValue(object originalValue, object convertedValue) {
		}
		internal protected virtual RowHandle SetValueInternal(object value) {
			PropertyDescriptor.Do(x => x.SetValue(MultiInstance, value));
			return RowHandle;
		}
		object TryConvertFromDifferentType(object value) {
			if (value == null)
				return value;
			object result = value;
			Type valueType = value.GetType();
			if (valueType != PropertyType) {
				if (Converter.CanConvertFrom(this, valueType))
					result = Converter.ConvertFrom(this, System.Globalization.CultureInfo.CurrentCulture, value);
				else {
					if (valueType.IsPrimitive || value is IConvertible) {
						Type conversionType = PropertyType;
						Type underlyingType = GetUnderlyingType(conversionType);
						result = Convert.ChangeType(value, underlyingType != null ? underlyingType : conversionType, null);
					}
				}
			}
			return result;
		}
		public virtual bool CanResetValue() {
			return PropertyDescriptor.Return(x => x.CanResetValue(MultiInstance), () => false);
		}
		public virtual RowHandle ResetValue() {
			PropertyDescriptor.Do(x => x.ResetValue(MultiInstance));
			return RowHandle;
		}
		TypeConverter GetBaseConverter() {
			TypeConverter baseConverter = ExtractConverter();
			if (Instance != null)
				return MetadataHelper.GetExternalAndFluentAPIAttrbutes(Instance.GetType(), Name).OfType<TypeConverterWrapperAttribute>().SingleOrDefault().WrapTypeConverter(baseConverter);
			return baseConverter;
		}
		TypeConverter ExtractConverter() {
			if (PropertyDescriptor != null)
				return PropertyDescriptor.Converter;
			if (Value != null)
				return TypeDescriptor.GetConverter(Value);
			return new TypeConverter();
		}
		public TypeConverter GetConverter() {
			TypeConverter converter = GetConverterInternal();
			if (converter != null)
				return converter;
			return BaseConverter;
		}
		protected virtual TypeConverter GetConverterInternal() {
			if (DataController == null)
				return null;
			bool useNewInstanceInitializer = CanUseInstanceInitializer() && DataController.VisualClient.AllowInstanceInitializer(RowHandle);
			if (CanUseCollectionEditor() && DataController.VisualClient.AllowCollectionEditor(RowHandle)) {
				return new ListConverter(useNewInstanceInitializer);
			}
			if (useNewInstanceInitializer)
				return DataController.NewInstanceConverter;
			return null;
		}
		public bool CanUseCollectionEditor() {
			if (ListConverter.IsNewItemProperty(PropertyDescriptor))
				return false;
			return Value is IList ||
				(Value == null && PropertyType != null && typeof(IList).IsInstanceOfType(PropertyType));
		}
		bool CanUseInstanceInitializer() {
			if (ListConverter.IsItemProperty(PropertyDescriptor) || IsPropertyReadOnly)
				return false;
			return DataController.NewInstanceConverter.GetStandardValues(this).If(x => x.Count > 0).ReturnSuccess();
		}
		protected virtual PropertyDescriptorCollection GetPropertiesInternal(object source, Attribute[] attributes) {
			if (!IsGetPropertiesSupported)
				return null;
			try {
				var properties = Converter.GetProperties(this, source, attributes);
				AllowExpandingMode mode = AllowExpandingMode;
				if ((mode == AllowExpandingMode.Force || mode == AllowExpandingMode.ForceIfNoTypeConverter) &&
					(properties == null || properties.Count == 0) && CollectionHelper == NullCollectionHelper.Instance) {
					properties = TypeDescriptor.GetProperties(source, attributes, false);
				}
				return properties;
			}
			catch {
				return null;
			}
		}
		public PropertyDescriptor GetProperty(object source, Attribute[] attributes, string propertyName, bool onlyBrowsable = false) {
			PropertyDescriptorCollection propCollection = GetProperties(source, attributes);
			PropertyDescriptor propertyDescriptor = null;
			if (propCollection != null) {
				propertyDescriptor = propCollection.Find(propertyName, false);
			}
			if (propertyDescriptor != null) {
				return propertyDescriptor;
			}
			if (onlyBrowsable)
				return null;
			return TypeDescriptor.GetProperties(source, null, true).Return(x => x.Find(propertyName, false), () => null);
		}
		public PropertyDescriptorCollection GetProperties(object source, Attribute[] attributes) {
			if (source == null)
				return null;
			PropertyDescriptorCollection properties = null;
			if (!Properties.TryGetValue(source, out properties)) {
				properties = GetPropertiesInternal(source, attributes);
				Properties[source] = properties;
			}
			return properties;
		}
		public bool ShouldSerializeValue {
			get {
				if (shouldSerializeValue == null) {
					shouldSerializeValue = GetShouldSerializeValue();
				}
				return shouldSerializeValue.Value;
			}
		}
		private Dictionary<object, PropertyDescriptorCollection> Properties {
			get {
				if (properties == null) {
					properties = new Dictionary<object, PropertyDescriptorCollection>();
				}
				return properties;
			}
		}
		bool IsSerializeContentsProp {
			get {
				if (PropertyDescriptor == null)
					return false;
				return PropertyDescriptor.SerializationVisibility == DesignerSerializationVisibility.Content;
			}
		}
		public virtual bool ShouldRenderReadOnly {
			get {
				if (ForceReadOnly) {
					return true;
				}
				if (IsPropertyReadOnly && ReadOnlyEditable) {
					if (PropertyType != null && (PropertyType.IsArray || PropertyType.IsPrimitive || PropertyType.IsValueType)) {
						return true;
					}
				}
				return ShouldRenderReadOnlyCore && !IsSerializeContentsProp && !NeedsCustomEditorButton;
			}
		}
		internal bool NeedsDropDownButton {
			get {
				if (PropertyDescriptor == null)
					return false;
				if (GetEditStyle(this) != System.Drawing.Design.UITypeEditorEditStyle.DropDown)
					return false;
				return true;
			}
		}
		bool NeedsCustomEditorButtonCore {
			get {
				if (PropertyDescriptor == null)
					return false;
				return GetEditStyle(this) == System.Drawing.Design.UITypeEditorEditStyle.Modal;
			}
		}
		internal bool NeedsCustomEditorButton {
			get {
				if (!NeedsCustomEditorButtonCore)
					return false;
				if (!IsValueEditable) {
					return !ReadOnlyEditable;
				}
				else {
					return true;
				}
			}
		}
		bool ShouldRenderReadOnlyCore {
			get {
				if (ForceReadOnly)
					return true;
				if (!IsValueEditable)
					return !ReadOnlyEditable;
				else
					return false;
			}
		}
		protected virtual bool IsPropertyReadOnly {
			get {
				if (PropertyDescriptor == null)
					return true;
				return PropertyDescriptor.IsReadOnly;
			}
		}
		public bool IsValueEditable {
			get {
				if (!IsPropertyReadOnly)
					return IsValueEditableCore;
				else
					return false;
			}
		}
		bool IsValueEditableCore {
			get {
				if (!ForceReadOnly)
					return IsTextEditableCore || Enumerable || NeedsCustomEditorButtonCore || NeedsDropDownButton;
				else
					return false;
			}
		}
		bool IsTextEditable {
			get {
				if (IsValueEditable)
					return IsTextEditableCore;
				else {
					return false;
				}
			}
		}
		bool IsTextEditableCore {
			get {
				return Converter.CanConvertFrom(this, typeof(string)) && !Converter.GetStandardValuesExclusive(this) || PropertyType.IsAssignableFrom(typeof(string));
			}
		}
		bool EnumerableCore {
			get { return Converter.GetStandardValuesSupported(this); }
		}
		bool Enumerable {
			get {
				if (EnumerableCore)
					return !IsPropertyReadOnly;
				else
					return false;
			}
		}
		bool IsImmutableObject {
			get {
				return TypeAttributes[typeof(ImmutableObjectAttribute)] == ImmutableObjectAttribute.Yes;
			}
		}
		bool ForceReadOnly {
			get {
				if (PropertyDescriptor == null)
					return true;
				if (MultiInstance == null)
					return false;
				if (EdmPropertyInfo != null && EdmPropertyInfo.Attributes.IsReadOnly.HasValue)
					return EdmPropertyInfo.Attributes.IsReadOnly.Value;
				ReadOnlyAttribute readOnlyAttribute = (ReadOnlyAttribute)TypeDescriptor.GetAttributes(MultiInstance)[typeof(ReadOnlyAttribute)];
				bool forceReadOnly = readOnlyAttribute != null && !readOnlyAttribute.IsDefaultAttribute();
				return forceReadOnly;
			}
		}
		bool ReadOnlyEditable {
			get {
				if (PropertyDescriptor == null)
					return false;
				if (!CanExpand)
					return false;
				return !ForceReadOnly && !IsTextEditable && !IsImmutableObject;
			}
		}
		public Type PropertyType { get { return PropertyDescriptor.Return(x => x.PropertyType, null); } }
		UITypeEditorEditStyle GetEditStyle(DescriptorContext context) {
			return UITypeEditorEditStyle.None;
		}
		protected virtual bool GetShouldSerializeValue() {
			if (PropertyDescriptor == null || MultiInstance == null)
				return true;
			return PropertyDescriptor.ShouldSerializeValue(MultiInstance);
		}
		string GetDisplayName() {
			if (PropertyDescriptor == null)
				return PropertyHelper.RootPropertyName;
			string displayName = PropertyDescriptor.DisplayName;
			if (NeedParenthesize(PropertyDescriptor))
				return ParenthesizeCaption(displayName);
			else
				return displayName;
		}
		bool NeedParenthesize(PropertyDescriptor propertyDescriptor) {
			return ((ParenthesizePropertyNameAttribute)propertyDescriptor.Attributes[typeof(ParenthesizePropertyNameAttribute)]).NeedParenthesis;
		}
		string ParenthesizeCaption(string caption) {
			return string.Format("({0})", caption);
		}
		INotifyPropertyChangedEventHandler iNotifyPropertyChangedHandler;
		INotifyCollectionChangedEventHandler iNotifyCollectionChangedHandler;
		void SubscribeNotificationsInternal(object value) {
			INotifyPropertyChanged iNotifyPropertyChanged = value as INotifyPropertyChanged;
			if (iNotifyPropertyChanged != null && iNotifyPropertyChangedHandler == null) {
				iNotifyPropertyChangedHandler = new INotifyPropertyChangedEventHandler(this,
					(d, o, e) => {
						if (!string.IsNullOrEmpty(e.PropertyName))
							return;
						d.DataController.Invalidate(d.RowHandle);
					}
				);
				iNotifyPropertyChanged.PropertyChanged += iNotifyPropertyChangedHandler.Handler;
			}
			INotifyCollectionChanged iNotifyCollectionChanged = value as INotifyCollectionChanged;
			if (iNotifyCollectionChanged != null && IsGetPropertiesSupported && iNotifyCollectionChangedHandler == null) {
				iNotifyCollectionChangedHandler = new INotifyCollectionChangedEventHandler(this,
					(d, o, e) => {
						d.DataController.Invalidate(d.RowHandle);
					}
				);
				iNotifyCollectionChanged.CollectionChanged += iNotifyCollectionChangedHandler.Handler;
			}
		}
		protected virtual void SubscribeNotifications() {
			if (IsMultiSource) {
				var multiInstance = MultiInstance as object[];
				if (multiInstance == null)
					return;
				IEnumerable<object> values = PropertyDescriptor != null ? ((MultiObjectPropertyDescriptor)PropertyDescriptor).GetValues(multiInstance) : multiInstance;
				if (values == null)
					return;
				values.ForEach(value => SubscribeNotificationsInternal(value));
			}
			else {
				SubscribeNotificationsInternal(Value);
			}
		}
		protected virtual object GetValue() {
			object value = PropertyHelper.GetValue(PropertyDescriptor, MultiInstance);
			return value;
		}
		protected bool GetIsGetPropertiesSupported() {
			if (DataController != null) {
				AllowExpandingMode mode = AllowExpandingMode;
				if (mode == AllowExpandingMode.Force)
					return true;
				if (mode == AllowExpandingMode.Never)
					return false;
				if (mode == AllowExpandingMode.ForceIfNoTypeConverter && IsDefaultTypeConverter)
					return true;
			}
			return Converter.Return(x => x.GetPropertiesSupported(this), () => false);
		}
		protected bool GetGetCreateInstanceSupported() {
			return Converter.Return(x => x.GetCreateInstanceSupported(this), () => false);
		}
		internal void InvalidateChildren() {
			this.isValid = false;
		}
		DescriptorContext GetContext(RowHandle rowhandle) {
			return DataController.ContextCache[IsMultiSource, rowhandle];
		}
		#region ITypeDescriptorContext Members
		public System.ComponentModel.IContainer Container { get { return null; } }
		public void OnComponentChanged() {
		}
		public bool OnComponentChanging() {
			return false;
		}
#if !SL
		System.ComponentModel.PropertyDescriptor System.ComponentModel.ITypeDescriptorContext.PropertyDescriptor {
			get { return PropertyDescriptor; }
		}
#else
		System.ComponentModel.PropertyDescriptor System.ComponentModel.ITypeDescriptorContext.PropertyDescriptor {
			get { return null; }
		}
#endif
		#endregion
		#region IServiceProvider Members
		T GetService<T>() {
			return (T)GetService(typeof(T));
		}
		public object GetService(Type serviceType) {
			return ServiceProvider.GetService(serviceType);
		}
		#endregion
		internal void Reset() {
			this.hasValue = false;
			this.properties = null;
			this.categoryHandles = null;
			this.childHandles = null;
			this.visibleHandles = null;
			this.isGetPropertiesSupported = null;
			this.converter = null;
			this.baseConverter = null;
			this.getCreateInstanceSupported = null;
			this.shouldSerializeValue = null;
			this.isValid = true;
			this.standardValues = null;
			this.isStandardValuesSupported = null;
			this.isStandardValuesExclusive = null;
			this.propertyAttributes = null;
			this.typeAttributes = null;
			this.displayName = null;
			this.collectionHelper = null;
			this.categoryName = new Lazy<string>(() => GetCategoryName());
			this.validationError = new Lazy<IEnumerable<string>>(() => GetValidationError());
			this.supportDataAnnotation = new Lazy<bool>(() => GetSupportDataAnnotation());
			this.dataErrorInfo = new Lazy<IDataErrorInfo>(() => GetIDataErrorInfo());
			this.instanceInitializer = new Lazy<IInstanceInitializer>(() => GetInstanceInitializer());
			this.propertyValidator = new Lazy<PropertyValidator>(() => GetEdmValidator());
			this.properties = null;
		}
		protected virtual PropertyValidator GetEdmValidator() {
			if (PropertyDescriptor == null)
				return null;
			return DataColumnAttributesExtensions.CreatePropertyValidator(PropertyDescriptor, Instance.With(x => x.GetType()));
		}
		static Type GetUnderlyingType(Type nullableType) {
			if (!nullableType.IsGenericType) {
				return null;
			}
			return nullableType.GetGenericArguments()[0];
		}
		bool SupportDataAnnotations { get { return supportDataAnnotation.Value; } }
		IDataErrorInfo DataErrorInfo { get { return dataErrorInfo.Value; } }
		IDataErrorInfo GetIDataErrorInfo() {
			return Instance as IDataErrorInfo;
		}
		bool GetSupportDataAnnotation() {
			return EdmValidator != null;
		}
		protected virtual IEdmPropertyInfo GetEdmPropertyInfo() {
			if (PropertyDescriptor == null)
				return null;
			if (AssignedEdmPropertyInfo != null)
				return AssignedEdmPropertyInfo;
			return new EdmPropertyInfo(PropertyDescriptor, DataColumnAttributesProvider.GetAttributes(PropertyDescriptor), false);
		}
		protected virtual IEnumerable<string> GetValidationError() {
			try {
				if (SupportDataAnnotations) {
					return EdmValidator.GetErrors(Value, Instance);
				}
				string error = GetIDataErrorInfoError();
				if (!string.IsNullOrEmpty(error))
					return new List<string> { error };
			}
			catch(Exception e) {
				return new List<string> { PropertyHelper.GetUnwindedException(e).Message };
			}
			return null;
		}
		string GetIDataErrorInfoError() {
			return DataErrorInfo.Return(x => x[Name], () => null);
		}
		public virtual object GetValues() {
			if (PropertyDescriptor == null)
				return MultiInstance;
			return IsMultiSource ? ((MultiObjectPropertyDescriptor)PropertyDescriptor).GetValues((object[])MultiInstance) : Value;
		}
	}
	public class CollectionHelper {
		Lazy<bool> canRemoveCollectionItem;
		Lazy<bool> canAddNewItem;
		DataController DataController { get { return CollectionContext.DataController; } }
		DescriptorContext CollectionContext { get; set; }
		DescriptorContext NewItemContext { get; set; }
		public bool CanRemoveCollectionItem { get { return canRemoveCollectionItem.Value; } }
		public bool CanAddNewItem { get { return canAddNewItem.Value; } }
		public virtual object SelectedItem { get { return NewItemContext.Value; } }
		public virtual IEnumerable NewItemValues {
			get {
				if (!CanAddNewItem)
					return null;
				return NewItemContext.StandardValues;
			}
		}
		protected CollectionHelper() {
			this.canRemoveCollectionItem = new Lazy<bool>(() => GetCanRemoveCollectionItem());
			this.canAddNewItem = new Lazy<bool>(() => GetCanAddNewItem());
		}
		public CollectionHelper(DescriptorContext context) : this() {
			Guard.ArgumentNotNull(context, "context");
			this.CollectionContext = context;
			this.NewItemContext = new NewListItemDescriptorContext(context);
		}
		public virtual void AddNewItem(object item) {
			if (NewItemContext == null)
				return;
			NewItemContext.SetValue(item);
			DataController.Invalidate(CollectionContext.RowHandle);
		}
		protected virtual bool GetCanRemoveCollectionItem() {
			IList list = CollectionContext.Value as IList;
			if (list == null || list.IsFixedSize)
				return false;
			return true;
		}
		protected virtual bool GetCanAddNewItem() {
			IList list = CollectionContext.Value as IList;
			if (list == null || list.IsFixedSize)
				return false;
			Type elementType = ListConverter.GetElementType(list.GetType());
			if (CanUseListItemInitializer(elementType, NewItemContext.InstanceInitializer) &&
				DataController.VisualClient.AllowListItemInitializer(CollectionContext.RowHandle))
				return true;
			return false;
		}
		bool CanUseListItemInitializer(Type elementType, IInstanceInitializer initializer) {
			if (initializer != null && initializer.Types != null)
				return true;
			return NewInstanceConverter.CanUseDefaultNewItemInitializer(elementType);
		}
		public virtual void RemoveCollectionItem(RowHandle handle) {
			IList list = CollectionContext.Value as IList;
			if (list == null)
				return;
			ListItemDescriptorContext itemContext = DataController.GetDescriptorContext(handle) as ListItemDescriptorContext;
			if (itemContext == null)
				return;
			list.RemoveAt(itemContext.Index);
			DataController.Invalidate(CollectionContext.RowHandle);
		}
	}
	public class NullCollectionHelper : CollectionHelper {
		public static CollectionHelper Instance {
			get;
			protected set;
		}
		static NullCollectionHelper() {
			Instance = new NullCollectionHelper();
		}
		protected NullCollectionHelper() { }
		public override IEnumerable NewItemValues { get { return null; } }
		public override void AddNewItem(object item) { }
		public override void RemoveCollectionItem(RowHandle handle) { }
		protected override bool GetCanRemoveCollectionItem() { return false; }
		protected override bool GetCanAddNewItem() { return false; }
	}
}
