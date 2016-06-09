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
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	sealed class DefaultEndUserFilteringSettingsFactory : IEndUserFilteringSettingsFactory {
		internal static readonly IEndUserFilteringSettingsFactory Instance = new DefaultEndUserFilteringSettingsFactory();
		DefaultEndUserFilteringSettingsFactory() { }
		public IEndUserFilteringSettings Create(Type type, IEnumerable<IEndUserFilteringMetricAttributes> customAttributes) {
			var typeMetadataMetricsProvider = new TypeMetadataMetricsProvider(type);
			var customMetricsProvider = new CustomMetricsProvider(customAttributes);
			return new EndUserFilteringSettings(typeMetadataMetricsProvider, customMetricsProvider);
		}
		static bool SkipFiltering(Type type) {
			return 
				type == typeof(byte[]) ||
				type == typeof(System.Drawing.Image);
		}
		class TypeMetadataMetricsProvider : IEndUserFilteringMetadataProvider {
			HashSet<Type> typesHash = new HashSet<Type>();
			Stack<KeyValuePair<string, PropertyDescriptor>> propertiesStack;
			IMetadataStorage metadataStorage;
			public TypeMetadataMetricsProvider(Type type, string rootPath = null) {
				propertiesStack = new Stack<KeyValuePair<string, PropertyDescriptor>>();
				if(type != null)
					PopulateLevel(rootPath, TypeDescriptor.GetProperties(type), type);
			}
			void PopulateLevel(string rootPath, PropertyDescriptorCollection properties, Type propertyType = null) {
				if(propertyType != null) {
					if(typesHash.Contains(propertyType))
						return;
					typesHash.Add(propertyType);
				}
				for(int i = 0; i < properties.Count; i++) {
					PropertyDescriptor pd = properties[(properties.Count - 1) - i];
					propertiesStack.Push(new KeyValuePair<string, PropertyDescriptor>(rootPath, pd));
				}
			}
			static string GetPath(string rootPath, PropertyDescriptor pd) {
				return (string.IsNullOrEmpty(rootPath)) ? pd.Name : rootPath + "." + pd.Name;
			}
			IEnumerator<IEndUserFilteringMetric> IEnumerable<IEndUserFilteringMetric>.GetEnumerator() {
				return GetEnumeratorCore();
			}
			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumeratorCore();
			}
			protected virtual IEnumerator<IEndUserFilteringMetric> GetEnumeratorCore() {
				while(propertiesStack.Count > 0) {
					var current = propertiesStack.Pop();
					PropertyDescriptor pd = current.Value;
					string rootPath = current.Key;
					var path = GetPath(rootPath, pd);
					if(SkipFiltering(pd.PropertyType))
						continue;
					if(!TypeHelper.IsExpandableProperty(pd)) {
						var metadataAttributes = FilterAttributes.GetMetadataAttributes(pd);
						var attributes = new Data.Utils.AnnotationAttributes(metadataAttributes);
						attributes = attributes.Merge(new DevExpress.Data.Utils.AnnotationAttributes(pd));
						if(Data.Utils.AnnotationAttributes.GetAutoGenerateColumnOrFilter(attributes)) {
							metadataStorage.SetAttributes(path, attributes);
							var filterAttributes = new FilterAttributes(pd);
							metadataStorage.SetAttributes(path, filterAttributes);
							if(filterAttributes.HasFilterPropertyAttribute && !filterAttributes.IsFilterProperty)
								continue;
							yield return new EndUserFilteringMetric(GetServiceProvider, path);
						}
						else metadataStorage.SetAttributes(path, attributes);
					}
					else PopulateLevel(path, pd.GetChildProperties(), pd.PropertyType);
				}
			}
			void IEndUserFilteringMetadataProvider.SetMetadataStorage(IMetadataStorage metadataStorage) {
				this.metadataStorage = metadataStorage;
			}
			IServiceProvider GetServiceProvider() {
				return metadataStorage as IServiceProvider;
			}
		}
		class CustomMetricsProvider : IEndUserFilteringMetadataProvider, IEndUserFilteringMetricAttributesProvider,
			IEqualityComparer<IEndUserFilteringMetricAttributes> {
			readonly IEnumerable<IEndUserFilteringMetricAttributes> customAttributes;
			IMetadataStorage metadataStorage;
			public CustomMetricsProvider(IEnumerable<IEndUserFilteringMetricAttributes> customAttributes) {
				this.customAttributes = customAttributes
					.@Get(a => a.Distinct(this), EndUserFilteringSettings.EmptyAttributes);
			}
			IEnumerator<IEndUserFilteringMetric> IEnumerable<IEndUserFilteringMetric>.GetEnumerator() {
				return GetEnumeratorCore();
			}
			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumeratorCore();
			}
			protected virtual IEnumerator<IEndUserFilteringMetric> GetEnumeratorCore() {
				foreach(var a in customAttributes) {
					if(SkipFiltering(a.Type))
						continue;
					if(!TypeHelper.IsExpandableType(a.Type)) {
						var annotationAttributes = new DevExpress.Data.Utils.AnnotationAttributes(a);
						Data.Utils.AnnotationAttributes existing = null;
						if(AllowMerging(a) && metadataStorage.TryGetValue(a.Path, out existing))
							annotationAttributes = annotationAttributes.Merge(existing);
						if(Data.Utils.AnnotationAttributes.GetAutoGenerateColumnOrFilter(annotationAttributes)) {
							metadataStorage.SetAttributes(a.Path, annotationAttributes);
							FilterAttributes filterAttributes = new FilterAttributes(a, a.Type);
							FilterAttributes existingFilterAttributes;
							if(AllowMerging(a) && metadataStorage.TryGetValue(a.Path, out existingFilterAttributes))
								filterAttributes = filterAttributes.Merge(existingFilterAttributes);
							metadataStorage.SetAttributes(a.Path, filterAttributes);
							if(filterAttributes.HasFilterPropertyAttribute && !filterAttributes.IsFilterProperty)
								continue;
							yield return new EndUserFilteringMetric(GetServiceProvider, a.Path);
						}
						else {
							if(Data.Utils.AnnotationAttributes.GetAutoGenerateColumnOrFilter(existing))
								metadataStorage.SetAttributes(a.Path, annotationAttributes);
						}
					}
					else {
						IEndUserFilteringMetadataProvider metadataMetricsProvider = new TypeMetadataMetricsProvider(a.Type, a.Path);
						metadataMetricsProvider.SetMetadataStorage(metadataStorage);
						foreach(var metric in metadataMetricsProvider)
							yield return metric;
					}
				}
			}
			bool AllowMerging(IEndUserFilteringMetricAttributes a) {
				return a.MergeMode != AttributesMergeMode.Replace;
			}
			IServiceProvider GetServiceProvider() {
				return metadataStorage as IServiceProvider;
			}
			#region IEndUserFilteringMetricAttributesProvider
			IEnumerable<IEndUserFilteringMetricAttributes> IEndUserFilteringMetricAttributesProvider.Attributes {
				get { return customAttributes; }
			}
			void IEndUserFilteringMetadataProvider.SetMetadataStorage(IMetadataStorage metadataStorage) {
				this.metadataStorage = metadataStorage;
			}
			#endregion
			#region IEqualityComparer<IEndUserFilteringMetricAttributes> Members
			bool IEqualityComparer<IEndUserFilteringMetricAttributes>.Equals(IEndUserFilteringMetricAttributes x, IEndUserFilteringMetricAttributes y) {
				return x.Path == y.Path;
			}
			int IEqualityComparer<IEndUserFilteringMetricAttributes>.GetHashCode(IEndUserFilteringMetricAttributes a) {
				return a.Path.GetHashCode();
			}
			#endregion
		}
	}
}
