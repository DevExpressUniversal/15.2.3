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
using System.Collections.Concurrent;
using System.Linq;
using DevExpress.Utils.Zip;
namespace DevExpress.XtraReports.Service.Native.Services {
	public class InMemoryResourceProviderService : IResourceProviderService {
		class Resource {
			public string Key { get; set; }
			public object Context { get; set; }
			public object Object { get; set; }
			public byte[] Data { get; set; }
		}
		static readonly byte[] EmptyData = new byte[0];
		static string GenerateId(byte[] data) {
			return Adler32.CalculateChecksum(data).ToString();
		}
		readonly ConcurrentDictionary<object, BlockingCollection<Resource>> resourcesByContext = new ConcurrentDictionary<object, BlockingCollection<Resource>>();
		readonly ConcurrentDictionary<object, Resource> resourcesByObject = new ConcurrentDictionary<object, Resource>();
		readonly ConcurrentDictionary<string, Resource> resourcesByKey = new ConcurrentDictionary<string, Resource>();
		#region Add
		string AddCore<T>(object context, T obj, Func<T, byte[]> serializeAction) {
			Resource resource;
			if(resourcesByObject.TryGetValue(obj, out resource)) {
				return resource.Key;
			}
			var collection = GetCollection(context);
			var data = serializeAction(obj);
			var key = GenerateId(data);
			resource = collection.FirstOrDefault(r => r.Key == key);
			if(resource != null) {
				return resource.Key;
			}
			resource = new Resource { Key = key, Context = context, Object = obj, Data = data };
			AddResourceToDictionaries(collection, resource);
			return key;
		}
		BlockingCollection<Resource> GetCollection(object context) {
			BlockingCollection<Resource> collection;
			if(!resourcesByContext.TryGetValue(context, out collection)) {
				collection = new BlockingCollection<Resource>();
				resourcesByContext[context] = collection;
			}
			return collection;
		}
		void AddResourceToDictionaries(BlockingCollection<Resource> resources, Resource resource) {
			resources.Add(resource);
			resourcesByObject[resource.Object] = resource;
			resourcesByKey[resource.Key] = resource;
		}
		#endregion
		#region Remove
		void RemoveFromDictionariesByContext(object context) {
			Resource fakeResource;
			BlockingCollection<Resource> resources;
			if(!resourcesByContext.TryRemove(context, out resources)) {
				return;
			}
			foreach(var resource in resources) {
				resourcesByObject.TryRemove(resource.Object, out fakeResource);
				resourcesByKey.TryRemove(resource.Key, out fakeResource);
			}
			resources.Dispose();
		}
		#endregion
		#region IResourceProviderService
		public virtual string Add<T>(object context, T obj, Func<T, byte[]> serializeAction) {
			lock(context) {
				return AddCore(context, obj, serializeAction);
			}
		}
		public virtual byte[] Get(string key) {
			Resource resource;
			if(resourcesByKey.TryGetValue(key, out resource)) {
				return resource.Data;
			}
			return EmptyData;
		}
		public virtual void Clear(object context) {
			lock(context) {
				RemoveFromDictionariesByContext(context);
			}
		}
		#endregion
	}
}
