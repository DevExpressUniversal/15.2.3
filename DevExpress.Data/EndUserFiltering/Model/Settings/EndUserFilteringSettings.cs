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
	using System.Linq;
	using DevExpress.Data.Utils;
	using DevExpress.Utils.IoC;
	interface IMetadataStorage {
		void SetEnabled(string path, bool enabled);
		void SetOrder(string path, int? order);
		void SetAttributes(string path, AnnotationAttributes attributes);
		void SetAttributes(string path, FilterAttributes attributes);
		bool TryGetValue(string path, out int order);
		bool TryGetValue(string path, out AnnotationAttributes attributes);
		bool TryGetValue(string path, out FilterAttributes attributes);
	}
	interface IEndUserFilteringMetadataProvider :
		IEnumerable<IEndUserFilteringMetric> {
		void SetMetadataStorage(IMetadataStorage metadataStorage);
	}
	interface IMetricAttributesCache {
		Type GetValueOrCache(string path, Func<Type> create);
		IMetricAttributes GetValueOrCache(string path, Func<IMetricAttributes> create);
		void Reset();
	}
	class EndUserFilteringSettings : IEndUserFilteringSettings, IServiceProvider,
		IMetadataStorage, IMetricAttributesCache,
		IEqualityComparer<IEndUserFilteringMetric> {
		IOrderedStorage<IEndUserFilteringMetric> storage;
		IEndUserFilteringMetadataProvider customMetadataProvider;
		public EndUserFilteringSettings(IEndUserFilteringMetadataProvider typeMetadataProvider, IEndUserFilteringMetadataProvider customMetadataProvider) {
			typeMetadataProvider.SetMetadataStorage(this);
			customMetadataProvider.SetMetadataStorage(this);
			this.customMetadataProvider = customMetadataProvider;
			var metrics = typeMetadataProvider.Concat(customMetadataProvider).Distinct(this);
			this.storage = new Storage<IEndUserFilteringMetric>(metrics, m => m.Order);
			RegisterServices();
		}
		#region IEndUserFilteringSettings
		IEndUserFilteringMetric IEndUserFilteringSettings.this[string path] {
			get { return storage[path, m => m.Path]; }
		}
		internal static readonly IEnumerable<IEndUserFilteringMetricAttributes> EmptyAttributes = Enumerable.Empty<IEndUserFilteringMetricAttributes>();
		IEnumerable<IEndUserFilteringMetricAttributes> IEndUserFilteringSettings.CustomAttributes {
			get {
				return (customMetadataProvider as IEndUserFilteringMetricAttributesProvider)
					.@Get(provider => provider.Attributes, EmptyAttributes);
			}
		}
		IEnumerable<KeyValuePair<string, TValue>> IEndUserFilteringSettings.GetPairs<TValue>(Func<IEndUserFilteringMetric, TValue> accessor) {
			return storage.GetPairs(m => m.Path, accessor);
		}
		#endregion
		#region IEnumerable
		IEnumerator<IEndUserFilteringMetric> IEnumerable<IEndUserFilteringMetric>.GetEnumerator() {
			return storage.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)storage).GetEnumerator();
		}
		#endregion
		#region Services
		IntegrityContainer serviceContainer = new IntegrityContainer();
		object IServiceProvider.GetService(Type serviceType) {
			return serviceContainer.Resolve(serviceType);
		}
		void RegisterServices() {
			serviceContainer.RegisterInstance<IEndUserFilteringSettings>(this);
			serviceContainer.RegisterInstance<IBehaviorProvider>(new BehaviorProvider(orders, disabledItems));
			serviceContainer.RegisterInstance<IMetadataProvider>(new MetadataProvider(this));
		}
		#endregion Services
		#region Metadata Storage
		Hashtable disabledItems = new Hashtable();
		void IMetadataStorage.SetEnabled(string path, bool value) {
			SetEnabledCore(path, value);
		}
		void SetEnabledCore(string path, bool value) {
			if(value)
				disabledItems.Remove(path);
			else
				disabledItems.Add(path, null);
		}
		IDictionary<string, int> orders = new Dictionary<string, int>();
		void IMetadataStorage.SetOrder(string path, int? value) {
			SetOrderCore(path, value);
		}
		bool IMetadataStorage.TryGetValue(string path, out int value) {
			return orders.TryGetValue(path, out value);
		}
		void SetOrderCore(string path, int? value) {
			if(!value.HasValue)
				orders.Remove(path);
			else
				orders.Add(path, value.Value);
		}
		void ResetOrder() {
			storage.ResetOrder(m => m.Order);
		}
		IDictionary<string, AnnotationAttributes> attributes = new Dictionary<string, AnnotationAttributes>();
		void IMetadataStorage.SetAttributes(string path, AnnotationAttributes value) {
			SetAttributesCore(path, value);
		}
		bool IMetadataStorage.TryGetValue(string path, out AnnotationAttributes value) {
			return attributes.TryGetValue(path, out value);
		}
		void SetAttributesCore(string path, AnnotationAttributes value) {
			if(value == null)
				attributes.Remove(path);
			else {
				AnnotationAttributes existing;
				if(!attributes.TryGetValue(path, out existing))
					attributes.Add(path, value);
				else {
					if(existing != value)
						attributes[path] = value;
				}
			}
		}
		IDictionary<string, FilterAttributes> filterAttributes = new Dictionary<string, FilterAttributes>();
		void IMetadataStorage.SetAttributes(string path, FilterAttributes value) {
			SetFilterAttributesCore(path, value);
		}
		bool IMetadataStorage.TryGetValue(string path, out FilterAttributes value) {
			return filterAttributes.TryGetValue(path, out value);
		}
		void SetFilterAttributesCore(string path, FilterAttributes value) {
			if(value == null)
				filterAttributes.Remove(path);
			else {
				FilterAttributes existing;
				if(!filterAttributes.TryGetValue(path, out existing))
					filterAttributes.Add(path, value);
				else {
					if(existing != value)
						filterAttributes[path] = value;
				}
			}
		}
		#endregion Metadata Storage
		#region Metrics Cache
		IDictionary<string, Type> typeDefinitionsCache = new Dictionary<string, Type>();
		Type IMetricAttributesCache.GetValueOrCache(string path, Func<Type> create) {
			return GetValueOrCache(path, create, typeDefinitionsCache);
		}
		IDictionary<string, IMetricAttributes> metricsCache = new Dictionary<string, IMetricAttributes>();
		IMetricAttributes IMetricAttributesCache.GetValueOrCache(string path, Func<IMetricAttributes> create) {
			return GetValueOrCache(path, create, metricsCache);
		}
		static T GetValueOrCache<T>(string path, Func<T> create, IDictionary<string, T> cache) {
			T result = default(T);
			if(!cache.TryGetValue(path, out result)) {
				result = create();
				cache.Add(path, result);
			}
			return result;
		}
		void IMetricAttributesCache.Reset() {
			typeDefinitionsCache.Clear();
			metricsCache.Clear();
		}
		#endregion
		#region IEqualityComparer<IEndUserFilteringMetricAttributes> Members
		bool IEqualityComparer<IEndUserFilteringMetric>.Equals(IEndUserFilteringMetric x, IEndUserFilteringMetric y) {
			return x.Path == y.Path;
		}
		int IEqualityComparer<IEndUserFilteringMetric>.GetHashCode(IEndUserFilteringMetric m) {
			return m.Path.GetHashCode();
		}
		#endregion
	}
}
