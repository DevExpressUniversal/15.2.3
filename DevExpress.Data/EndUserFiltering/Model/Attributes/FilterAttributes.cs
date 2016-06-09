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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	public class FilterAttributes {
		#region static
		readonly static Func<PropertyDescriptor, AttributeCollection> metadataAttributesProvider;
		readonly static Func<PropertyDescriptor, FilterAttributesProvider> propertyDescriptorAttributesProviderInitializer;
		readonly static Func<AttributeCollection, FilterAttributesProvider> attributeCollectionAttributesProviderInitializer;
		static FilterAttributes() {
			try {
				CheckDataAnnotations_ConditionallyAPTCAIssue<MetadataTypeAttribute>();
				metadataAttributesProvider = (property) => new FilterAttributesProviderReal.MetadataAttributesProvider(property).Attributes;
				propertyDescriptorAttributesProviderInitializer = (property) => new FilterAttributesProviderReal(property);
				attributeCollectionAttributesProviderInitializer = (attributes) => new FilterAttributesProviderReal(attributes);
			}
			catch(MethodAccessException) {
				metadataAttributesProvider = (property) => property.Attributes;
				propertyDescriptorAttributesProviderInitializer = (property) => FilterAttributesProviderEmpty.Instance;
				attributeCollectionAttributesProviderInitializer = (attributes) => FilterAttributesProviderEmpty.Instance;
			}
		}
		#endregion
		#region Conditionally APTCA Issue Threat
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
		static void CheckDataAnnotations_ConditionallyAPTCAIssue<TAttrbute>() {
		}
		#endregion
		readonly FilterAttributesProvider provider;
		readonly Type propertyType;
		readonly Type enumDataType;
		FilterAttributes(FilterAttributesProvider provider, Type propertyType) {
			this.provider = provider;
			this.propertyType = propertyType;
			this.enumDataType = provider.GetEnumDataType(propertyType);
		}
		public FilterAttributes(PropertyDescriptor property) {
			this.provider = propertyDescriptorAttributesProviderInitializer(property);
			this.propertyType = property.PropertyType;
			this.enumDataType = provider.GetEnumDataType(propertyType);
		}
		public FilterAttributes(AttributeCollection attributes, Type propertyType) {
			this.provider = attributeCollectionAttributesProviderInitializer(attributes);
			this.propertyType = propertyType;
			this.enumDataType = provider.GetEnumDataType(propertyType);
		}
		public FilterAttributes(IEnumerable<Attribute> attributes, Type propertyType)
			: this(new AttributeCollection(attributes.ToArray()), propertyType) {
		}
		public FilterAttributes(DevExpress.Data.Utils.AnnotationAttributes annotationAttributes, Type propertyType)
			: this(annotationAttributes.GetAttributes(), propertyType) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Type PropertyType {
			get { return propertyType; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Type EnumDataType {
			get { return enumDataType; }
		}
		public bool Any() {
			return provider.Any();
		}
		#region FilterEditor
		public bool HasEditorAttribute {
			get { return provider.HasEditorAttribute; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Internal.FilterRangeEditorSettings GetRangeEditorSettings() {
			return provider.GetRangeEditorSettings();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Internal.FilterLookupEditorSettings GetLookupEditorSettings() {
			return provider.GetLookupEditorSettings();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Internal.FilterBooleanEditorSettings GetBooleanEditorSettings() {
			return provider.GetBooleanEditorSettings();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Internal.FilterEnumEditorSettings GetEnumEditorSettings() {
			return provider.GetEnumEditorSettings();
		}
		#endregion FilterEditor
		#region FilterProperty
		public bool HasFilterPropertyAttribute {
			get { return provider.HasFilterPropertyAttribute; }
		}
		public bool IsFilterProperty {
			get { return provider.IsFilterProperty; }
		}
		#endregion FilterProperty
		public FilterAttributes Merge(FilterAttributes attributes) {
			if(provider == null || provider == FilterAttributesProviderEmpty.Instance)
				return attributes;
			if(attributes == null || attributes.provider == null || attributes.provider == FilterAttributesProviderEmpty.Instance)
				return this;
			return new FilterAttributes(new FilterAttributesProviderMerged(provider, attributes.provider), PropertyType);
		}
		internal static AttributeCollection GetMetadataAttributes(PropertyDescriptor descriptor) {
			return metadataAttributesProvider(descriptor);
		}
		Type metricAttributesTypeDefinition;
		public Type GetMetricAttributesTypeDefinition() {
			if(metricAttributesTypeDefinition == null) {
				if(provider.Any()) {
					if(provider.HasRangeAttribute || provider.HasDateTimeRangeAttribute)
						metricAttributesTypeDefinition = typeof(IRangeMetricAttributes<>);
					if((metricAttributes == null) && provider.HasEnumChoiceAttribute)
						metricAttributesTypeDefinition = typeof(IEnumChoiceMetricAttributes<>);
					if((metricAttributes == null) && provider.HasBooleanChoiceAttribute)
						metricAttributesTypeDefinition = typeof(IChoiceMetricAttributes<>);
					if((metricAttributes == null) && provider.HasLookupAttribute)
						metricAttributesTypeDefinition = typeof(ILookupMetricAttributes<>);
				}
			}
			return metricAttributesTypeDefinition;
		}
		IMetricAttributes metricAttributes;
		public IMetricAttributes GetMetricAttributes() {
			if(metricAttributes == null) {
				if(provider.Any() && PropertyType != null) {
					if(provider.HasRangeAttribute)
						metricAttributes = provider.GetRangeAttributes(PropertyType);
					if(provider.HasDateTimeRangeAttribute)
						metricAttributes = provider.GetDateTimeRangeAttributes(PropertyType);
					if((metricAttributes == null) && provider.HasEnumChoiceAttribute)
						metricAttributes = provider.GetEnumChoiceAttributes(PropertyType);
					if((metricAttributes == null) && provider.HasBooleanChoiceAttribute)
						metricAttributes = provider.GetBooleanChoiceAttributes(PropertyType);
					if((metricAttributes == null) && provider.HasLookupAttribute)
						metricAttributes = provider.GetLookupAttributes(PropertyType);
				}
			}
			return metricAttributes;
		}
		#region Providers
		abstract class FilterAttributesProvider {
			protected FilterAttributesProvider(AttributeCollection attributes) { }
			public virtual bool Any() { return false; }
			public virtual Type GetEnumDataType(Type type) { return type; }
			public virtual bool HasRangeAttribute { get { return false; } }
			public virtual IMetricAttributes GetRangeAttributes(Type type) { return null; }
			public virtual bool HasDateTimeRangeAttribute { get { return false; } }
			public virtual IMetricAttributes GetDateTimeRangeAttributes(Type type) { return null; }
			public virtual bool HasLookupAttribute { get { return false; } }
			public virtual IMetricAttributes GetLookupAttributes(Type type) { return null; }
			public virtual bool HasBooleanChoiceAttribute { get { return false; } }
			public virtual IMetricAttributes GetBooleanChoiceAttributes(Type type) { return null; }
			public virtual bool HasEnumChoiceAttribute { get { return false; } }
			public virtual IMetricAttributes GetEnumChoiceAttributes(Type type) { return null; }
			public virtual bool HasEditorAttribute { get { return false; } }
			public virtual bool HasFilterPropertyAttribute { get { return false; } }
			public virtual bool IsFilterProperty { get { return false; } }
			public virtual Internal.FilterRangeEditorSettings GetRangeEditorSettings() { return null; }
			public virtual Internal.FilterLookupEditorSettings GetLookupEditorSettings() { return null; }
			public virtual Internal.FilterBooleanEditorSettings GetBooleanEditorSettings() { return null; }
			public virtual Internal.FilterEnumEditorSettings GetEnumEditorSettings() { return null; }
		}
		class FilterAttributesProviderEmpty : FilterAttributesProvider {
			internal static FilterAttributesProviderEmpty Instance = new FilterAttributesProviderEmpty();
			FilterAttributesProviderEmpty() : base(null) { }
		}
		class FilterAttributesProviderReal : FilterAttributesProvider {
			readonly AttributeCollection attributes;
			readonly Lazy<FilterRangeAttribute> rangeAttributeValue;
			readonly Lazy<FilterDateTimeRangeAttribute> dateTimeRangeAttributeValue;
			readonly Lazy<FilterLookupAttribute> lookupAttributeValue;
			readonly Lazy<FilterBooleanChoiceAttribute> booleanChoiceAttributeValue;
			readonly Lazy<FilterEnumChoiceAttribute> enumChoiceAttributeValue;
			readonly Lazy<Internal.FilterEditorAttribute> filterEditorAttributeValue;
			readonly Lazy<Internal.FilterPropertyAttribute> filterPropertyAttributeValue;
			readonly Lazy<EnumDataTypeAttribute> enumDataTypeAttributeValue;
			public FilterAttributesProviderReal(PropertyDescriptor property) :
				this(new MetadataAttributesProvider(property).Attributes) {
			}
			public FilterAttributesProviderReal(AttributeCollection attributes)
				: base(attributes) {
				this.attributes = attributes;
				this.rangeAttributeValue = Read<FilterRangeAttribute>();
				this.dateTimeRangeAttributeValue = Read<FilterDateTimeRangeAttribute>();
				this.lookupAttributeValue = Read<FilterLookupAttribute>();
				this.booleanChoiceAttributeValue = Read<FilterBooleanChoiceAttribute>();
				this.enumChoiceAttributeValue = Read<FilterEnumChoiceAttribute>();
				this.filterEditorAttributeValue = Read<Internal.FilterEditorAttribute>();
				this.filterPropertyAttributeValue = Read<Internal.FilterPropertyAttribute>();
				this.enumDataTypeAttributeValue = Read<EnumDataTypeAttribute>();
			}
			public override bool Any() {
				return attributes.OfType<Attribute>()
					.Any(a => a is FilterAttribute);
			}
			public override Type GetEnumDataType(Type type) {
				return Read(enumDataTypeAttributeValue, x => x.EnumType, type);
			}
			#region Range
			public override bool HasRangeAttribute {
				get { return rangeAttributeValue.Value != null; }
			}
			public override IMetricAttributes GetRangeAttributes(Type type) {
				return Read(rangeAttributeValue, x => MetricAttributes.CreateRange(type, x.Minimum, x.Maximum, x.Average, x.GetFromName(), x.GetToName(), x.EditorType, x.GetMembers()));
			}
			public override bool HasDateTimeRangeAttribute {
				get { return dateTimeRangeAttributeValue.Value != null; }
			}
			public override IMetricAttributes GetDateTimeRangeAttributes(Type type) {
				return Read(dateTimeRangeAttributeValue, x => MetricAttributes.CreateRange(type, x.Minimum, x.Maximum, x.GetFromName(), x.GetToName(), x.EditorType, x.GetMembers()));
			}
			#endregion Range
			#region Lookup
			public override bool HasLookupAttribute {
				get { return lookupAttributeValue.Value != null; }
			}
			public override IMetricAttributes GetLookupAttributes(Type type) {
				return Read(lookupAttributeValue, x => MetricAttributes.CreateLookup(type, x.DataSource, x.ValueMember, x.DisplayMember, x.Top, x.MaxCount, x.EditorType, x.ActualUseFlags, x.ActualUseSelectAll, x.GetSelectAllName(), x.GetNullName(), x.GetMembers()));
			}
			#endregion Lookup
			#region BooleanChoice
			public override bool HasBooleanChoiceAttribute {
				get { return booleanChoiceAttributeValue.Value != null; }
			}
			public override IMetricAttributes GetBooleanChoiceAttributes(Type type) {
				return Read(booleanChoiceAttributeValue, x => MetricAttributes.CreateBooleanChoice(type, x.DefaultValue, x.GetTrueName(), x.GetFalseName(), x.GetDefaultName(), x.EditorType, x.GetMembers()));
			}
			#endregion
			#region EnumChoice
			public override bool HasEnumChoiceAttribute {
				get { return enumChoiceAttributeValue.Value != null; }
			}
			public override IMetricAttributes GetEnumChoiceAttributes(Type type) {
				return Read(enumChoiceAttributeValue, x => MetricAttributes.CreateEnumChoice(type, GetEnumDataType(type), x.EditorType, x.ActualUseFlags, x.ActualUseSelectAll, x.GetSelectAllName(), x.GetNullName(), x.GetMembers()));
			}
			#endregion
			#region FilterEditor
			public override bool HasEditorAttribute {
				get { return filterEditorAttributeValue.Value != null; }
			}
			public override Internal.FilterRangeEditorSettings GetRangeEditorSettings() {
				return Read(filterEditorAttributeValue, x => x.RangeEditorSettings);
			}
			public override Internal.FilterLookupEditorSettings GetLookupEditorSettings() {
				return Read(filterEditorAttributeValue, x => x.LookupEditorSettings);
			}
			public override Internal.FilterBooleanEditorSettings GetBooleanEditorSettings() {
				return Read(filterEditorAttributeValue, x => x.BooleanEditorSettings);
			}
			public override Internal.FilterEnumEditorSettings GetEnumEditorSettings() {
				return Read(filterEditorAttributeValue, x => x.EnumEditorSettings);
			}
			#endregion
			#region FilterEditor
			public override bool HasFilterPropertyAttribute {
				get { return filterPropertyAttributeValue.Value != null; }
			}
			public override bool IsFilterProperty {
				get { return Read(filterPropertyAttributeValue, x => x.IsFilterProperty); }
			}
			#endregion
			#region Read API
			TValue Read<TAttribute, TValue>(Lazy<TAttribute> lazyAttributeValue, Func<TAttribute, TValue> read, TValue defaultValue = default(TValue))
				where TAttribute : Attribute {
				return (lazyAttributeValue.Value != null) ? read(lazyAttributeValue.Value) : defaultValue;
			}
			Lazy<TAttribute> Read<TAttribute>() where TAttribute : Attribute {
				return Read<TAttribute, TAttribute>(typeof(TAttribute), x => x);
			}
			Lazy<TValue> Read<TAttribute, TValue>(Type attributeType, Func<TAttribute, TValue> reader, TValue defaultValue = default(TValue))
				where TAttribute : Attribute {
				return new Lazy<TValue>(() => DevExpress.Data.Utils.AnnotationAttributes.Reader.Read<TAttribute, TValue>(attributeType, attributes, reader, defaultValue));
			}
			#endregion
			#region MedataType
			internal class MetadataAttributesProvider {
				AttributeCollection attributes;
				public AttributeCollection Attributes {
					get { return attributes; }
				}
				static string GetKey(PropertyDescriptor descriptor) {
					return descriptor.ComponentType.FullName + "." + descriptor.Name;
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
						var metadataClassTypeAttribute =
							typeAttributes.FirstOrDefault(a => a is FilterMetadataTypeAttribute) ??
							typeAttributes.FirstOrDefault(a => a is MetadataTypeAttribute);
						if(metadataClassTypeAttribute != null) {
							var metadataClassType =
								(metadataClassTypeAttribute as FilterMetadataTypeAttribute).@Get(a => a.MetadataClassType) ??
								(metadataClassTypeAttribute as MetadataTypeAttribute).@Get(a => a.MetadataClassType);
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
						else {
							var externalAndFluentAttributes = ExternalAndFluentAPIFilteringAttributes.GetAttributes(descriptor.ComponentType, descriptor.Name);
							if(externalAndFluentAttributes != null)
								attributes = AttributeCollection.FromExisting(attributes, externalAndFluentAttributes.ToArray());
						}
					}
				}
#endif
			}
			#endregion
		}
		class FilterAttributesProviderMerged : FilterAttributesProvider {
			readonly IEnumerable<FilterAttributesProvider> providers;
			internal FilterAttributesProviderMerged(params FilterAttributesProvider[] providers)
				: base(null) {
				this.providers = providers ?? new FilterAttributesProvider[] { };
			}
			TValue Merge<TValue>(Func<FilterAttributesProvider, TValue> accessor, TValue defaultValue = default(TValue)) {
				TValue result = defaultValue;
				foreach(var provider in providers) {
					result = accessor(provider);
					if(!object.Equals(result, defaultValue))
						return result;
				}
				return result;
			}
			public override bool Any() { return Merge(x => x.Any()); }
			public override Type GetEnumDataType(Type type) { return Merge(x => x.GetEnumDataType(type), type); }
			public override bool HasRangeAttribute { get { return Merge(x => x.HasRangeAttribute); } }
			public override IMetricAttributes GetRangeAttributes(Type type) { return Merge(x => x.GetRangeAttributes(type)); }
			public override bool HasDateTimeRangeAttribute { get { return Merge(x => x.HasDateTimeRangeAttribute); } }
			public override IMetricAttributes GetDateTimeRangeAttributes(Type type) { return Merge(x => x.GetDateTimeRangeAttributes(type)); }
			public override bool HasLookupAttribute { get { return Merge(x => x.HasLookupAttribute); } }
			public override IMetricAttributes GetLookupAttributes(Type type) { return Merge(x => x.GetLookupAttributes(type)); }
			public override bool HasBooleanChoiceAttribute { get { return Merge(x => x.HasBooleanChoiceAttribute); } }
			public override IMetricAttributes GetBooleanChoiceAttributes(Type type) { return Merge(x => x.GetBooleanChoiceAttributes(type)); }
			public override bool HasEnumChoiceAttribute { get { return Merge(x => x.HasEnumChoiceAttribute); } }
			public override IMetricAttributes GetEnumChoiceAttributes(Type type) { return Merge(x => x.GetEnumChoiceAttributes(type)); }
			public override bool HasEditorAttribute { get { return Merge(x => x.HasEditorAttribute); } }
			public override Internal.FilterRangeEditorSettings GetRangeEditorSettings() { return Merge(x => x.GetRangeEditorSettings()); }
			public override Internal.FilterLookupEditorSettings GetLookupEditorSettings() { return Merge(x => x.GetLookupEditorSettings()); }
			public override Internal.FilterBooleanEditorSettings GetBooleanEditorSettings() { return Merge(x => x.GetBooleanEditorSettings()); }
			public override Internal.FilterEnumEditorSettings GetEnumEditorSettings() { return Merge(x => x.GetEnumEditorSettings()); }
			public override bool HasFilterPropertyAttribute { get { return Merge(x => x.HasFilterPropertyAttribute); } }
			public override bool IsFilterProperty { get { return Merge(x => x.IsFilterProperty); } }
		}
		#endregion Providers
	}
}
