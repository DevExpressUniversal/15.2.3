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
using System.ComponentModel.DataAnnotations;
using System.Linq;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Data.Utils {
	public class AnnotationAttributes {
		#region static
		readonly static Func<PropertyDescriptor, AnnotationAttributesProvider> propertyDescriptorAnnotationAttributesProviderInitializer;
		readonly static Func<AttributeCollection, AnnotationAttributesProvider> attributeCollectionAnnotationAttributesProviderInitializer;
		static AnnotationAttributes() {
			AllowValidation = DevExpress.Utils.DefaultBoolean.Default;
			try {
				CheckDataAnnotations_ConditionallyAPTCAIssue<DisplayAttribute>();
				CheckDataAnnotations_ConditionallyAPTCAIssue<DisplayFormatAttribute>();
				propertyDescriptorAnnotationAttributesProviderInitializer = (property) => new AnnotationAttributesProviderReal(property);
				attributeCollectionAnnotationAttributesProviderInitializer = (attributes) => new AnnotationAttributesProviderReal(attributes);
			}
			catch(MethodAccessException) {
				IsConditionallyAPTCAIssueThreat = true;
				propertyDescriptorAnnotationAttributesProviderInitializer = (property) => AnnotationAttributesProviderEmpty.Instance;
				attributeCollectionAnnotationAttributesProviderInitializer = (attributes) => AnnotationAttributesProviderEmpty.Instance;
			}
		}
		public static DevExpress.Utils.DefaultBoolean AllowValidation { get; set; }
		#endregion
		#region Conditionally APTCA Issue Threat
		readonly static bool IsConditionallyAPTCAIssueThreat;
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
		static void CheckDataAnnotations_ConditionallyAPTCAIssue<TAttrbute>() {
		}
		#endregion
		readonly AnnotationAttributesProvider provider;
		AnnotationAttributes(AnnotationAttributesProvider provider) {
			this.provider = provider;
		}
		public AnnotationAttributes(PropertyDescriptor property) {
			provider = propertyDescriptorAnnotationAttributesProviderInitializer(property);
		}
		readonly object dataSourceNullValue;
		public AnnotationAttributes(PropertyDescriptor property, object dataSourceNullValue)
			: this(property) {
			this.dataSourceNullValue = dataSourceNullValue;
		}
		public AnnotationAttributes(IEnumerable<Attribute> attributes) {
			var attributeCollection = new AttributeCollection(attributes.ToArray());
			provider = attributeCollectionAnnotationAttributesProviderInitializer(attributeCollection);
		}
		public AnnotationAttributes(AttributeCollection attributes) {
			provider = attributeCollectionAnnotationAttributesProviderInitializer(attributes);
		}
		public bool Any() {
			return provider.Any();
		}
		protected internal AttributeCollection GetAttributes() {
			return provider.GetAttributes();
		}
		public AnnotationAttributes Merge(AnnotationAttributes attributes) {
			if(provider == null || provider == AnnotationAttributesProviderEmpty.Instance)
				return attributes;
			if(attributes == null || attributes.provider == null || attributes.provider == AnnotationAttributesProviderEmpty.Instance)
				return this;
			return new AnnotationAttributes(new AnnotationAttributesProviderMerged(provider, attributes.provider));
		}
		#region Validation
		public bool TryValidateValue(object value, out string errorMessage) {
			return provider.TryValidateValue(CheckDataSourceNullValue(value), out errorMessage);
		}
		public bool TryValidateValue(object value, object row, out string errorMessage) {
			return provider.TryValidateValue(CheckDataSourceNullValue(value), row, out errorMessage);
		}
		object CheckDataSourceNullValue(object value) {
			return !object.Equals(value, null) && object.Equals(value, dataSourceNullValue) ? null : value;
		}
		#endregion Validation
		public bool? CheckAddEnumeratorIntegerValues() {
			return provider.CheckAddEnumeratorIntegerValues();
		}
		public DataType? GetActualDataType() {
			return provider.GetActualDataType();
		}
		#region Display
		public bool HasDisplayAttribute {
			get { return provider.HasDisplayAttribute; }
		}
		public string Name {
			get { return provider.Name; }
		}
		public string Description {
			get { return provider.Description; }
		}
		public string ShortName {
			get { return provider.ShortName; }
		}
		public string GroupName {
			get { return provider.GroupName; }
		}
		public string Prompt {
			get { return provider.Prompt; }
		}
		public int? Order {
			get { return provider.Order; }
		}
		public bool? AutoGenerateField {
			get { return provider.AutoGenerateField; }
		}
		public bool? AutoGenerateFilter {
			get { return provider.AutoGenerateFilter; }
		}
		#endregion
		#region Display Format
		public bool HasDisplayFormatAttribute {
			get { return provider.HasDisplayFormatAttribute; }
		}
		public bool ApplyFormatInEditMode {
			get { return provider.ApplyFormatInEditMode; }
		}
		public bool ConvertEmptyStringToNull {
			get { return provider.ConvertEmptyStringToNull; }
		}
		public string DataFormatString {
			get { return provider.DataFormatString; }
		}
		public string NullDisplayText {
			get { return provider.NullDisplayText; }
		}
		#endregion
		public bool? IsRequired {
			get { return provider.IsRequired; }
		}
		public bool? AllowEdit {
			get { return provider.AllowEdit; }
		}
		public bool? IsReadOnly {
			get { return provider.IsReadOnly; }
		}
		public DataType? DataType {
			get { return provider.DataType; }
		}
		public Type EnumType {
			get { return provider.EnumType; }
		}
		public bool IsKey {
			get { return provider.IsKey; }
		}
		public string FieldName {
			get { return provider.FieldName; }
		}
		public object Key {
			get { return provider.Key; }
		}
		protected bool IsShortNameEmpty {
			get { return provider.IsShortNameEmpty; }
		}
		protected bool HasShortName {
			get { return provider.HasShortName; }
		}
		abstract class AnnotationAttributesProvider {
			AttributeCollection attributes;
			Lazy<bool> isKey;
			protected AnnotationAttributesProvider(AttributeCollection attributes) {
				this.attributes = attributes;
				this.isKey = new Lazy<bool>(() => GetIsKey());
			}
			public virtual bool Any() { return false; }
			protected virtual bool GetIsKey() {
				return (attributes != null) && attributes.OfType<Attribute>()
					.Any(a => a.GetType().Name == "KeyAttribute");
			}
			public virtual bool TryValidateValue(object value, out string errorMessage) {
				errorMessage = null;
				return true;
			}
			public virtual bool TryValidateValue(object value, object row, out string errorMessage) {
				errorMessage = null;
				return true;
			}
			public virtual DataType? GetActualDataType() { return null; }
			#region Display
			public virtual bool HasDisplayAttribute { get { return false; } }
			public virtual string Name { get { return null; } }
			public virtual string Description { get { return null; } }
			public virtual bool IsShortNameEmpty { get { return false; } }
			public virtual bool HasShortName { get { return false; } }
			public virtual string ShortName { get { return null; } }
			public virtual string GroupName { get { return null; } }
			public virtual string Prompt { get { return null; } }
			public virtual int? Order { get { return null; } }
			public virtual bool? AutoGenerateField { get { return null; } }
			public virtual bool? AutoGenerateFilter { get { return null; } }
			#endregion
			#region Display Format
			public virtual bool HasDisplayFormatAttribute { get { return false; } }
			public virtual bool ApplyFormatInEditMode { get { return false; } }
			public virtual bool ConvertEmptyStringToNull { get { return false; } }
			public virtual string DataFormatString { get { return null; } }
			public virtual string NullDisplayText { get { return null; } }
			#endregion
			public virtual bool? IsRequired { get { return null; } }
			public virtual bool? AllowEdit { get { return null; } }
			public virtual bool? IsReadOnly { get { return null; } }
			public virtual DataType? DataType { get { return null; } }
			public virtual Type EnumType { get { return null; } }
			public virtual string FieldName { get { return null; } }
			public virtual object Key { get { return null; } }
			public bool IsKey { get { return isKey.Value; } }
			public virtual bool? CheckAddEnumeratorIntegerValues() { return null; }
			public virtual AttributeCollection GetAttributes() { return attributes; }
		}
		class AnnotationAttributesProviderEmpty : AnnotationAttributesProvider {
			internal static AnnotationAttributesProvider Instance = new AnnotationAttributesProviderEmpty();
			AnnotationAttributesProviderEmpty() : base(null) { }
		}
		class AnnotationAttributesProviderReal : AnnotationAttributesProvider {
			readonly AttributeCollection attributes;
			readonly Lazy<DisplayAttribute> displayAttributeValue;
			readonly Lazy<DisplayNameAttribute> displayNameAttributeValue;
			readonly Lazy<DescriptionAttribute> descriptionAttributeValue;
			readonly Lazy<DisplayFormatAttribute> displayFormatAttributeValue;
			readonly Lazy<bool?> requiredValue;
			readonly Lazy<bool?> isReadOnlyValue;
			readonly Lazy<bool?> allowEditValue;
			readonly Lazy<DataType?> dataTypeValue;
			readonly Lazy<Type> enumTypeValue;
			readonly Lazy<bool?> enumDataColumnValue;
			public AnnotationAttributesProviderReal(PropertyDescriptor property) :
				this(new MetadataAttributesProvider(property).Attributes) {
				this.fieldNameCore = GetFieldName(property);
				this.keyCore = GetKey(property);
				this.enumDataColumnValue = new Lazy<bool?>(() => DataColumnPropertyDescriptorHelper.IsEnumDataColumn(property));
			}
			public AnnotationAttributesProviderReal(AttributeCollection attributes)
				: base(attributes) {
				this.attributes = attributes;
				this.displayAttributeValue = Read<DisplayAttribute>();
				this.displayNameAttributeValue = Read<DisplayNameAttribute>();
				this.descriptionAttributeValue = Read<DescriptionAttribute>();
				this.displayFormatAttributeValue = Read<DisplayFormatAttribute>();
				this.requiredValue = Read<RequiredAttribute, bool?>(x => true);
				this.isReadOnlyValue = Read<ReadOnlyAttribute, bool?>(x => x.IsReadOnly);
				this.allowEditValue = Read<EditableAttribute, bool?>(x => x.AllowEdit);
				this.dataTypeValue = Read<DataTypeAttribute, DataType?>(x => x.DataType);
				this.enumTypeValue = Read<EnumDataTypeAttribute, Type>(x => x.EnumType);
			}
			static Type[] supportedAttributes = new Type[] { 
				typeof(DisplayAttribute),
				typeof(DisplayFormatAttribute),
				typeof(DisplayNameAttribute),
				typeof(DescriptionAttribute),
				typeof(ReadOnlyAttribute),
				typeof(EditableAttribute),
			};
			public override bool Any() {
				return attributes.OfType<Attribute>()
					.Any(a => a is ValidationAttribute || Array.IndexOf(supportedAttributes, a.GetType()) != -1);
			}
			public override AttributeCollection GetAttributes() {
				return attributes;
			}
			public override bool TryValidateValue(object value, out string errorMessage) {
				errorMessage = null;
				if(AllowValidation == DevExpress.Utils.DefaultBoolean.False)
					return true;
				var validationAttributes = attributes.OfType<Attribute>()
					.Where(a => a is ValidationAttribute).OfType<ValidationAttribute>()
					.Where(a => !ValidationContextHelper.RequiresValidationContext(a));
				string name = (HasDisplayAttribute && !string.IsNullOrEmpty(Name)) ? Name : FieldName;
				foreach(ValidationAttribute va in validationAttributes) {
					if(EnterpriseLibraryHelper.IsValidatorWithEmptyOrNullRuleset(va))
						if(AllowValidation == DevExpress.Utils.DefaultBoolean.Default)
							continue;
					if(SafeCheck(() => va.IsValid(value)))
						continue;
					if(!string.IsNullOrEmpty(name))
						errorMessage = va.FormatErrorMessage(name);
					return false;
				}
				return true;
			}
			public override bool TryValidateValue(object value, object row, out string errorMessage) {
				errorMessage = null;
				if(AllowValidation == DevExpress.Utils.DefaultBoolean.False)
					return true;
				var validationAttributes = attributes.OfType<Attribute>()
					.Where(a => a is ValidationAttribute).OfType<ValidationAttribute>()
					.Where(a => ValidationContextHelper.RequiresValidationContext(a));
				ValidationContext context = new ValidationContext(row, null, null) { MemberName = FieldName };
				List<ValidationResult> validationResults = new List<ValidationResult>();
				if(AllowValidation == DevExpress.Utils.DefaultBoolean.Default)
					validationAttributes = validationAttributes.Where(a => !EnterpriseLibraryHelper.IsValidatorWithEmptyOrNullRuleset(a));
				if(!SafeCheck(() => Validator.TryValidateValue(value, context, validationResults, validationAttributes))) {
					errorMessage = string.Join(Environment.NewLine, validationResults.Select(r => r.ErrorMessage));
					return false;
				}
				return true;
			}
			public override DataType? GetActualDataType() {
				return dataTypeValue.Value;
			}
			public override bool? CheckAddEnumeratorIntegerValues() {
				return enumDataColumnValue.Value;
			}
			string fieldNameCore;
			public override string FieldName { get { return fieldNameCore; } }
			object keyCore;
			public override object Key { get { return keyCore; } }
			static string GetFieldName(PropertyDescriptor descriptor) {
				return descriptor.Name;
			}
			static string GetKey(PropertyDescriptor descriptor) {
				return descriptor.ComponentType.FullName + "." + descriptor.Name;
			}
			class MetadataAttributesProvider {
				AttributeCollection attributes;
				public AttributeCollection Attributes {
					get { return attributes; }
				}
				static IDictionary<string, AttributeCollection> attributesCache = new Dictionary<string, AttributeCollection>();
				public MetadataAttributesProvider(PropertyDescriptor descriptor) {
					string key = GetKey(descriptor);
					lock(((System.Collections.ICollection)attributesCache).SyncRoot) {
						if(!attributesCache.TryGetValue(key, out attributes)) {
							attributes = descriptor.Attributes;
#if !SILVERLIGHT
							EnsureMetadataAttributes(descriptor);
#endif
							attributesCache.Add(key, attributes);
						}
					}
				}
#if !SILVERLIGHT
				void EnsureMetadataAttributes(PropertyDescriptor descriptor) {
					if(descriptor.ComponentType != null) {
						var typeAttributes = descriptor.ComponentType.GetCustomAttributes(true).OfType<Attribute>();
						var metadataClassTypeAttribute = typeAttributes.FirstOrDefault(a => a is MetadataTypeAttribute);
						if(metadataClassTypeAttribute != null) {
							var metadataClassType = ((MetadataTypeAttribute)metadataClassTypeAttribute).MetadataClassType;
							if(metadataClassType != null) {
								var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public;
								var memberInfo =
									(System.Reflection.MemberInfo)metadataClassType.GetProperty(descriptor.Name, flags) ??
									(System.Reflection.MemberInfo)metadataClassType.GetField(descriptor.Name, flags) ??
									(System.Reflection.MemberInfo)metadataClassType.GetMethod(descriptor.Name, flags);
								if(memberInfo != null) {
									var memberAttributes = memberInfo.GetCustomAttributes(true).OfType<Attribute>();
									attributes = AttributeCollection.FromExisting(attributes, memberAttributes.ToArray());
								}
							}
						}
					}
				}
#endif
			}
			#region Display
			public override bool HasDisplayAttribute {
				get { return displayAttributeValue.Value != null; }
			}
			public override string Name {
				get { return Read(displayAttributeValue, x => x.GetName()) ?? Read(displayNameAttributeValue, x => GetNonDefaultDisplayName(x)); }
			}
			public override string Description {
				get { return Read(displayAttributeValue, x => x.GetDescription()) ?? Read(descriptionAttributeValue, x => GetNonDefaultDescription(x)); }
			}
			public override string ShortName {
				get { return Read(displayAttributeValue, x => x.GetShortName()); }
			}
			public override bool IsShortNameEmpty {
				get { return Read(displayAttributeValue, x => x.ShortName == string.Empty); }
			}
			public override bool HasShortName {
				get { return Read(displayAttributeValue, x => x.ShortName != null); }
			}
			public override string GroupName {
				get { return Read(displayAttributeValue, x => x.GetGroupName()); }
			}
			public override string Prompt {
				get { return Read(displayAttributeValue, x => x.GetPrompt()); }
			}
			public override int? Order {
				get { return Read(displayAttributeValue, x => x.GetOrder()); }
			}
			public override bool? AutoGenerateField {
				get { return Read(displayAttributeValue, x => x.GetAutoGenerateField()); }
			}
			public override bool? AutoGenerateFilter {
				get { return Read(displayAttributeValue, x => x.GetAutoGenerateFilter()); }
			}
			#endregion
			#region Display Format
			public override bool HasDisplayFormatAttribute {
				get { return displayFormatAttributeValue.Value != null; }
			}
			public override bool ApplyFormatInEditMode {
				get { return Read(displayFormatAttributeValue, x => x.ApplyFormatInEditMode); }
			}
			public override bool ConvertEmptyStringToNull {
				get { return Read(displayFormatAttributeValue, x => x.ConvertEmptyStringToNull, true); }
			}
			public override string DataFormatString {
				get { return Read(displayFormatAttributeValue, x => x.DataFormatString); }
			}
			public override string NullDisplayText {
				get { return Read(displayFormatAttributeValue, x => x.NullDisplayText); }
			}
			#endregion
			public override bool? IsRequired {
				get { return requiredValue.Value; }
			}
			public override bool? AllowEdit {
				get { return allowEditValue.Value; }
			}
			public override bool? IsReadOnly {
				get { return isReadOnlyValue.Value; }
			}
			public override DataType? DataType {
				get { return dataTypeValue.Value; }
			}
			public override Type EnumType {
				get { return enumTypeValue.Value; }
			}
			#region Read API
			TValue Read<TAttribute, TValue>(Lazy<TAttribute> lazyAttributeValue, Func<TAttribute, TValue> read, TValue defaultValue = default(TValue))
				where TAttribute : Attribute {
				if(lazyAttributeValue.Value != null)
					return read(lazyAttributeValue.Value);
				return defaultValue;
			}
			Lazy<TAttribute> Read<TAttribute>() where TAttribute : Attribute {
				return Read<TAttribute, TAttribute>(typeof(TAttribute), x => x);
			}
			Lazy<TValue> Read<TAttribute, TValue>(Func<TAttribute, TValue> reader, TValue defaultValue = default(TValue))
				where TAttribute : Attribute {
				return Read<TAttribute, TValue>(typeof(TAttribute), reader, defaultValue);
			}
			Lazy<TValue> Read<TAttribute, TValue>(Type attributeType, Func<TAttribute, TValue> reader, TValue defaultValue = default(TValue))
				where TAttribute : Attribute {
				return new Lazy<TValue>(() => Reader.Read<TAttribute, TValue>(attributeType, attributes, reader, defaultValue));
			}
			#endregion
			#region Check Default attributes
			static string GetNonDefaultDisplayName(DisplayNameAttribute attribute) {
				return GetNonDefaultValue<DisplayNameAttribute>(attribute, a => a.DisplayName, DisplayNameAttribute.Default);
			}
			static string GetNonDefaultDescription(DescriptionAttribute attribute) {
				return GetNonDefaultValue<DescriptionAttribute>(attribute, a => a.Description, DescriptionAttribute.Default);
			}
			static string GetNonDefaultValue<TAttribute>(TAttribute attribute, Func<TAttribute, string> accessor, TAttribute defaultAttribute)
				where TAttribute : Attribute {
				string value = accessor(attribute);
				if(object.ReferenceEquals(defaultAttribute, attribute) && object.Equals(value, string.Empty))
					value = null;
				return value;
			}
			#endregion
			#region Helpers
			static bool SafeCheck(Func<bool> check, bool defaultResult = true) {
				try { return check(); }
				catch { return defaultResult; }
			}
			static class EnterpriseLibraryHelper {
				const string EnterpriseLibraryValidatorTypeName = "Microsoft.Practices.EnterpriseLibrary.Validation.Validators.BaseValidationAttribute";
				static Type EnterpriseLibraryValidatorType;
				internal static bool IsValidatorWithEmptyOrNullRuleset(Attribute attribute) {
					var type = attribute.GetType();
					while(type != null && (type != typeof(ValidationAttribute))) {
						if(EnterpriseLibraryValidatorType != null) {
							if(type == EnterpriseLibraryValidatorType)
								return IsNullOrEmptyRuleset(attribute, type);
						}
						else {
							if(type.FullName == EnterpriseLibraryValidatorTypeName) {
								EnterpriseLibraryValidatorType = type;
								return IsNullOrEmptyRuleset(attribute, type);
							}
						}
						type = type.BaseType;
					}
					return false;
				}
				static Func<Attribute, string> getRulesetRoutine;
				static bool IsNullOrEmptyRuleset(Attribute attribute, Type attributeType) {
					if(getRulesetRoutine == null) {
						var pInfo = EnterpriseLibraryValidatorType.GetProperty("Ruleset");
						var a = System.Linq.Expressions.Expression.Parameter(typeof(Attribute), "a");
						var accessor = System.Linq.Expressions.Expression.MakeMemberAccess(
							System.Linq.Expressions.Expression.Convert(a, attributeType), pInfo);
						getRulesetRoutine = System.Linq.Expressions.Expression.Lambda<Func<Attribute, string>>(
							accessor, a).Compile();
					}
					return string.IsNullOrEmpty(getRulesetRoutine(attribute));
				}
			}
			static class DataColumnPropertyDescriptorHelper {
				internal static bool? IsEnumDataColumn(PropertyDescriptor property) {
					if(IsDataColumnPropertyDescriptor(property.GetType())) {
						var dataColumn = GetDataColumn(property);
						if(dataColumn != null && dataColumn.DataType != null && dataColumn.DataType.IsEnum)
							return true;
					}
					return null;
				}
				static Type DataColumnPropertyDescriptorType;
				static bool IsDataColumnPropertyDescriptor(Type descriptorType) {
					if(DataColumnPropertyDescriptorType == null) {
						if(descriptorType.FullName == "System.Data.DataColumnPropertyDescriptor")
							DataColumnPropertyDescriptorType = descriptorType;
					}
					return (DataColumnPropertyDescriptorType != null) && DataColumnPropertyDescriptorType == descriptorType;
				}
				static System.Reflection.FieldInfo dataColumnFieldInfo;
				static System.Data.DataColumn GetDataColumn(PropertyDescriptor property) {
					if(dataColumnFieldInfo == null)
						dataColumnFieldInfo = DataColumnPropertyDescriptorType.GetField("column", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					if(dataColumnFieldInfo != null)
						return dataColumnFieldInfo.GetValue(property) as System.Data.DataColumn;
					return null;
				}
			}
			static class ValidationContextHelper {
				static Func<ValidationAttribute, bool> requiresValidationContextRoutine;
				internal static bool RequiresValidationContext(ValidationAttribute attribute) { 
					CustomValidationAttribute customValidationAttribute = attribute as CustomValidationAttribute;
					return (customValidationAttribute != null) ?
						CustomRequiresValidationContextCore(customValidationAttribute) :
						RequiresValidationContextCore(attribute);
				}
				internal static bool RequiresValidationContextCore(ValidationAttribute attribute) { 
					if(requiresValidationContextRoutine == null) {
						var pInfo = typeof(ValidationAttribute).GetProperty("RequiresValidationContext");
						if(pInfo != null) {
							var a = System.Linq.Expressions.Expression.Parameter(typeof(ValidationAttribute));
							requiresValidationContextRoutine = System.Linq.Expressions.Expression.Lambda<Func<ValidationAttribute, bool>>(
								System.Linq.Expressions.Expression.MakeMemberAccess(a, pInfo), a).Compile();
						}
						else requiresValidationContextRoutine = (va) => false;
					}
					return requiresValidationContextRoutine(attribute);
				}
				internal static bool CustomRequiresValidationContextCore(CustomValidationAttribute attribute) { 
					if(attribute.ValidatorType == null || string.IsNullOrEmpty(attribute.Method))
						return false;
					try {
						var method = attribute.ValidatorType.GetMethod(attribute.Method, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
						return (method != null) && method.GetParameters().Length == 2;
					}
					catch { return false; }
				}
			}
			#endregion
		}
		class AnnotationAttributesProviderMerged : AnnotationAttributesProvider {
			readonly IEnumerable<AnnotationAttributesProvider> providers;
			internal AnnotationAttributesProviderMerged(params AnnotationAttributesProvider[] providers)
				: base(null) {
				this.providers = providers ?? new AnnotationAttributesProvider[] { };
			}
			TValue Merge<TValue>(Func<AnnotationAttributesProvider, TValue> accessor, TValue defaultValue = default(TValue)) {
				TValue result = defaultValue;
				foreach(var provider in providers) {
					result = accessor(provider);
					if(!object.Equals(result, defaultValue))
						return result;
				}
				return result;
			}
			public override bool TryValidateValue(object value, out string errorMessage) {
				errorMessage = null;
				foreach(var provider in providers) {
					if(!provider.TryValidateValue(value, out errorMessage))
						return false;
				}
				return true;
			}
			public override bool TryValidateValue(object value, object row, out string errorMessage) {
				errorMessage = null;
				foreach(var provider in providers) {
					if(!provider.TryValidateValue(value, row, out errorMessage))
						return false;
				}
				return true;
			}
			public override bool Any() { return Merge(x => x.Any()); }
			public override DataType? GetActualDataType() { return Merge(x => x.GetActualDataType()); }
			#region Display
			public override bool HasDisplayAttribute { get { return Merge(x => x.HasDisplayAttribute); } }
			public override string Name { get { return Merge(x => x.Name); } }
			public override string Description { get { return Merge(x => x.Description); } }
			public override string ShortName { get { return Merge(x => x.ShortName); } }
			public override bool IsShortNameEmpty { get { return Merge(x => x.IsShortNameEmpty); } }
			public override bool HasShortName { get { return Merge(x => x.HasShortName); } }
			public override string GroupName { get { return Merge(x => x.GroupName); } }
			public override string Prompt { get { return Merge(x => x.Prompt); } }
			public override int? Order { get { return Merge(x => x.Order); } }
			public override bool? AutoGenerateField { get { return Merge(x => x.AutoGenerateField); } }
			public override bool? AutoGenerateFilter { get { return Merge(x => x.AutoGenerateFilter); } }
			#endregion
			#region Display Format
			public override bool HasDisplayFormatAttribute { get { return Merge(x => x.HasDisplayFormatAttribute); } }
			public override bool ApplyFormatInEditMode { get { return Merge(x => x.ApplyFormatInEditMode); } }
			public override bool ConvertEmptyStringToNull { get { return Merge(x => x.ConvertEmptyStringToNull); } }
			public override string DataFormatString { get { return Merge(x => x.DataFormatString); } }
			public override string NullDisplayText { get { return Merge(x => x.NullDisplayText); } }
			#endregion
			public override bool? IsRequired { get { return Merge(x => x.IsRequired); } }
			public override bool? AllowEdit { get { return Merge(x => x.AllowEdit); } }
			public override bool? IsReadOnly { get { return Merge(x => x.IsReadOnly); } }
			public override DataType? DataType { get { return Merge(x => x.DataType); } }
			public override Type EnumType { get { return Merge(x => x.EnumType); } }
			public override string FieldName { get { return Merge(x => x.FieldName); } }
			public override object Key { get { return Merge(x => x.Key); } }
			public override bool? CheckAddEnumeratorIntegerValues() { return Merge(x => x.CheckAddEnumeratorIntegerValues()); }
			public override AttributeCollection GetAttributes() { return Merge(x => x.GetAttributes()); }
		}
		#region Column Settings
		public static ColumnOptions GetColumnOptions(PropertyDescriptor columnDescriptor, int columnIndex, bool readOnly) {
			return new ColumnOptions(readOnly, columnIndex).Calculate(columnDescriptor);
		}
		public static bool ShouldHideFieldLabel(AnnotationAttributes annotationAttributes) {
			if(annotationAttributes == null || !annotationAttributes.HasDisplayAttribute)
				return false;
			return annotationAttributes.IsShortNameEmpty;
		}
		public static string GetColumnCaption(AnnotationAttributes annotationAttributes) {
			if(annotationAttributes == null || !annotationAttributes.HasDisplayAttribute)
				return null;
			return annotationAttributes.Name ?? annotationAttributes.ShortName;
		}
		public static string GetColumnDescription(AnnotationAttributes annotationAttributes) {
			if(annotationAttributes == null || !annotationAttributes.HasDisplayAttribute)
				return null;
			return annotationAttributes.Description;
		}
		public static string GetName(AnnotationAttributes annotationAttributes) {
			if(annotationAttributes == null || !annotationAttributes.HasDisplayAttribute)
				return null;
			return annotationAttributes.Name ?? (annotationAttributes.IsShortNameEmpty ? null : annotationAttributes.ShortName);
		}
		public static string GetShortName(AnnotationAttributes annotationAttributes) {
			if(annotationAttributes == null || !annotationAttributes.HasDisplayAttribute)
				return null;
			return annotationAttributes.HasShortName ? annotationAttributes.ShortName : null;
		}
		public static string GetDescription(AnnotationAttributes annotationAttributes) {
			if(annotationAttributes == null || !annotationAttributes.HasDisplayAttribute)
				return null;
			return annotationAttributes.Description;
		}
		public static string GetGroupName(AnnotationAttributes annotationAttributes) {
			if(annotationAttributes == null || !annotationAttributes.HasDisplayAttribute)
				return null;
			return annotationAttributes.GroupName;
		}
		public static int GetColumnIndex(AnnotationAttributes annotationAttributes, int columnIndex = 0) {
			if(annotationAttributes == null || !annotationAttributes.HasDisplayAttribute)
				return columnIndex;
			return annotationAttributes.Order.GetValueOrDefault(columnIndex);
		}
		public static bool GetAutoGenerateColumnOrFilter(Data.Utils.AnnotationAttributes attributes) {
			return (attributes != null) &&
				AnnotationAttributes.GetAutoGenerateColumn(attributes) &&
				AnnotationAttributes.GetAutoGenerateFilter(attributes);
		}
		public static bool GetAutoGenerateColumn(AnnotationAttributes annotationAttributes) {
			if(annotationAttributes == null || !annotationAttributes.HasDisplayAttribute)
				return true;
			return annotationAttributes.AutoGenerateField.GetValueOrDefault(true);
		}
		public static bool GetAutoGenerateFilter(AnnotationAttributes annotationAttributes) {
			if(annotationAttributes == null || !annotationAttributes.HasDisplayAttribute)
				return true;
			return annotationAttributes.AutoGenerateFilter.GetValueOrDefault(true);
		}
		public static string GetDataFormatString(AnnotationAttributes annotationAttributes) {
			if(annotationAttributes == null || !annotationAttributes.HasDisplayFormatAttribute)
				return null;
			return annotationAttributes.DataFormatString;
		}
		public static bool? CheckAddEnumeratorIntegerValues(AnnotationAttributes annotationAttributes) {
			return (annotationAttributes != null) ? annotationAttributes.CheckAddEnumeratorIntegerValues() : null;
		}
		public static DataType? GetDataType(AnnotationAttributes annotationAttributes) {
			return (annotationAttributes != null) ? annotationAttributes.GetActualDataType() : null;
		}
		#region ColumnOptions
		public sealed class ColumnOptions {
			internal ColumnOptions(bool isReadOnly, int columnIndex) {
				ReadOnly = isReadOnly;
				AutoGenerateField = true;
				AllowEdit = true;
				AllowFilter = true;
				ColumnIndex = columnIndex;
			}
			public bool AutoGenerateField { get; private set; }
			public bool AllowEdit { get; private set; }
			public bool AllowFilter { get; private set; }
			public bool ReadOnly { get; private set; }
			public bool IsFarAlignedByDefault { get; private set; }
			public int ColumnIndex { get; private set; }
			public AnnotationAttributes Attributes { get; private set; }
			public ColumnOptions Calculate(PropertyDescriptor columnDescriptor) {
				if(columnDescriptor != null) {
					Attributes = new AnnotationAttributes(columnDescriptor);
					if(Attributes.HasDisplayAttribute) {
						if(!Attributes.AutoGenerateField.GetValueOrDefault(true)) {
							AutoGenerateField = false;
							return this;
						}
						if(Attributes.Order.HasValue)
							ColumnIndex = Attributes.Order.Value;
						AllowFilter = Attributes.AutoGenerateFilter.GetValueOrDefault(AllowFilter);
					}
					IsFarAlignedByDefault = CalculateDefaultAlignment(columnDescriptor);
					ReadOnly = Attributes.IsReadOnly.GetValueOrDefault(ReadOnly);
					AllowEdit = Attributes.AllowEdit.GetValueOrDefault(AllowEdit);
				}
				return this;
			}
			bool CalculateDefaultAlignment(PropertyDescriptor columnDescriptor) {
				bool farAlignedByDefault = DevExpress.Data.Helpers.DefaultColumnAlignmentHelper.IsColumnFarAlignedByDefault(columnDescriptor.PropertyType);
				if(!IsConditionallyAPTCAIssueThreat)
					CheckDefaultAlignmentByDataType(ref farAlignedByDefault);
				return farAlignedByDefault;
			}
			void CheckDefaultAlignmentByDataType(ref bool farAlignedByDefault) {
				var dataType = Attributes.GetActualDataType();
				if(dataType.HasValue)
					farAlignedByDefault = (dataType.Value == System.ComponentModel.DataAnnotations.DataType.Currency);
			}
		}
		#endregion ColumnOptions
		#endregion
		#region Reader
		internal static class Reader {
			public static TValue Read<TAttribute, TValue>(Type type, Func<TAttribute, TValue> read, TValue defaultValue = default(TValue))
				where TAttribute : Attribute {
				return Read<TAttribute, TValue>(typeof(TAttribute), TypeDescriptor.GetAttributes(type), read, defaultValue);
			}
			public static TValue Read<TAttribute, TValue>(Type attributeType, AttributeCollection attributes, Func<TAttribute, TValue> read, TValue defaultValue)
				where TAttribute : Attribute {
				try {
					if(attributeType != null) {
						TAttribute attribute = attributes[attributeType] as TAttribute;
						if(attribute != null)
							return read(attribute);
					}
				}
				catch(System.Security.SecurityException) { } 
				catch(TypeAccessException) { } 
				return defaultValue;
			}
			public static TValue[] Read<TAttribute, TValue>(AttributeCollection attributes, Func<TAttribute, TValue> read)
				where TAttribute : Attribute {
				try {
					return attributes.Cast<Attribute>().Where(x => typeof(TAttribute).IsAssignableFrom(x.GetType())).Cast<TAttribute>().Select(x => read(x)).ToArray();
				}
				catch(System.Security.SecurityException) { } 
				catch(TypeAccessException) { } 
				return new TValue[0];
			}
		}
		#endregion Reader
	}
}
#if DEBUGTEST
namespace DevExpress.Data.Utils.Tests {
#if !SILVERLIGHT
	using NUnit.Framework;
#else
	using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
	#region Test Classes
	public class AnnotationTestClass_01 {
		public int Property1 { get; set; }
		[Display(Name = "Property2 Display.Name", Description = "Property2 Display.Description")]
		public int Property2 { get; set; }
		[DisplayName("Property3 DisplayName")]
		public int Property3 { get; set; }
		[DisplayFormat(DataFormatString = "p0", ApplyFormatInEditMode = true)]
		public int Property4 { get; set; }
		[Required]
		public int PropertyRequired { get; set; }
		[Editable(true)]
		public int PropertyEditable { get; set; }
		[ReadOnly(true)]
		public int PropertyReadOnly { get; set; }
		[DataType(System.ComponentModel.DataAnnotations.DataType.PhoneNumber)]
		public string PropertyPhoneNumber { get; set; }
	}
	class Obj_For_ColumnAndFieldOptionsTests {
		#region ShortName
		public string PropertyWithoutShortName { get; set; }
		[Display(ShortName = "")]
		public string PropertyWithEmptyShortName { get; set; }
		[Display(ShortName = "AAA")]
		public string PropertyWithNonEmptyShortName { get; set; }
		#endregion ShortName
	}
	#endregion
	[TestFixture]
	public class AnnotationTests01 {
		protected virtual PropertyDescriptorCollection GetProperties() {
			return TypeDescriptor.GetProperties(typeof(AnnotationTestClass_01));
		}
		[Test]
		public void Test00_NoAtributes() {
			var properties = GetProperties();
			var p1Attributes = new AnnotationAttributes(properties["Property1"]);
			Assert.IsFalse(p1Attributes.HasDisplayAttribute);
			Assert.IsFalse(p1Attributes.HasDisplayFormatAttribute);
			Assert.IsFalse(p1Attributes.Any());
		}
		[Test]
		public void Test01_Display() {
			var properties = GetProperties();
			var p2Attributes = new AnnotationAttributes(properties["Property2"]);
			Assert.IsTrue(p2Attributes.HasDisplayAttribute);
			Assert.AreEqual("Property2 Display.Name", p2Attributes.Name);
			Assert.AreEqual("Property2 Display.Description", p2Attributes.Description);
		}
		[Test]
		public void Test02_DisplayName() {
			var properties = GetProperties();
			var p3Attributes = new AnnotationAttributes(properties["Property3"]);
			Assert.IsFalse(p3Attributes.HasDisplayAttribute);
			Assert.AreEqual("Property3 DisplayName", p3Attributes.Name);
		}
		[Test]
		public void Test03_DisplayFormat() {
			var properties = GetProperties();
			var p4Attributes = new AnnotationAttributes(properties["Property4"]);
			Assert.IsTrue(p4Attributes.HasDisplayFormatAttribute);
			Assert.AreEqual("p0", p4Attributes.DataFormatString);
			Assert.IsTrue(p4Attributes.ApplyFormatInEditMode);
		}
		[Test]
		public void Test04_Required() {
			var properties = GetProperties();
			var pAttributes = new AnnotationAttributes(properties["PropertyRequired"]);
			Assert.IsTrue(pAttributes.IsRequired.HasValue && pAttributes.IsRequired.Value);
		}
		[Test]
		public void Test05_Editable() {
			var properties = GetProperties();
			var pAttributes = new AnnotationAttributes(properties["PropertyEditable"]);
			Assert.IsTrue(pAttributes.AllowEdit.HasValue && pAttributes.AllowEdit.Value);
		}
		[Test]
		public void Test06_ReadOnly() {
			var properties = GetProperties();
			var pAttributes = new AnnotationAttributes(properties["PropertyReadOnly"]);
			Assert.IsTrue(pAttributes.IsReadOnly.HasValue && pAttributes.IsReadOnly.Value);
		}
		[Test]
		public void Test07_DataType() {
			var properties = GetProperties();
			var pAttributes = new AnnotationAttributes(properties["PropertyPhoneNumber"]);
			Assert.IsTrue(pAttributes.DataType.HasValue && pAttributes.DataType.Value == DataType.PhoneNumber);
			Assert.AreEqual(DataType.PhoneNumber, pAttributes.GetActualDataType().Value);
		}
	}
#if !SILVERLIGHT
	#region Test Classes
	[MetadataType(typeof(AnotationTestClass_Metadata))]
	public class AnnotationTestClass_02 {
		public int Property1 { get; set; }
		public int Property2 { get; set; }
		public int Property3 { get; set; }
		public int Property4 { get; set; }
		public int PropertyRequired { get; set; }
		public int PropertyEditable { get; set; }
		public int PropertyReadOnly { get; set; }
		public string PropertyPhoneNumber { get; set; }
	}
	public class AnotationTestClass_Metadata {
		[Display(Name = "Property2 Display.Name", Description = "Property2 Display.Description")]
		public int Property2 { get; set; } 
		[DisplayName("Property3 DisplayName")]
		public void Property3() { } 
		[DisplayFormat(DataFormatString = "p0", ApplyFormatInEditMode = true)]
		public object Property4; 
		[Required]
		public object PropertyRequired;
		[Editable(true)]
		public object PropertyEditable;
		[ReadOnly(true)]
		public object PropertyReadOnly;
		[DataType(System.ComponentModel.DataAnnotations.DataType.PhoneNumber)]
		public object PropertyPhoneNumber;
	}
	#endregion Test Classes
	[TestFixture]
	public class AnnotationTests02_Metadata : AnnotationTests01 {
		protected override PropertyDescriptorCollection GetProperties() {
			return TypeDescriptor.GetProperties(typeof(AnnotationTestClass_02));
		}
	}
#endif
	[TestFixture]
	public class AnnotationTests03_ColumnAndFieldOptions {
		protected virtual PropertyDescriptorCollection GetProperties() {
			return TypeDescriptor.GetProperties(typeof(Obj_For_ColumnAndFieldOptionsTests));
		}
		[Test]
		public void Test00_ShouldHideFieldLabel() {
			var properties = GetProperties();
			Assert.IsFalse(AnnotationAttributes.ShouldHideFieldLabel(new AnnotationAttributes(properties["PropertyWithoutShortName"])));
			Assert.IsTrue(AnnotationAttributes.ShouldHideFieldLabel(new AnnotationAttributes(properties["PropertyWithEmptyShortName"])));
			Assert.IsFalse(AnnotationAttributes.ShouldHideFieldLabel(new AnnotationAttributes(properties["PropertyWithNonEmptyShortName"])));
		}
		[Test]
		public void Test00_GetShortName() {
			var properties = GetProperties();
			Assert.IsNull(AnnotationAttributes.GetShortName(new AnnotationAttributes(properties["PropertyWithoutShortName"])));
			Assert.IsEmpty(AnnotationAttributes.GetShortName(new AnnotationAttributes(properties["PropertyWithEmptyShortName"])));
			Assert.AreEqual("AAA", AnnotationAttributes.GetShortName(new AnnotationAttributes(properties["PropertyWithNonEmptyShortName"])));
		}
	}
}
#endif
