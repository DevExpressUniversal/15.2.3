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
	using System.Linq.Expressions;
	using System.Reflection;
	static class ExternalAndFluentAPIFilteringAttributes {
		internal static IEnumerable<Attribute> GetAttributes(Type componentType, string memberName) {
			return GetMetadataHelperType().@Get(t =>
						GetExternalAndFluentAPIFilteringAttributes(t, componentType, memberName) ??
						GetExternalAndFluentAPIAttributes(t, componentType, memberName));
		}
		static IDictionary<Type, IGetExternalAndFluentAPIAttributesProxy> getExternalAndFluentAPIAttributesCache = new Dictionary<Type, IGetExternalAndFluentAPIAttributesProxy>();
		static IEnumerable<Attribute> GetExternalAndFluentAPIAttributes(Type metadataHelperType, Type componentType, string memberName) {
			IGetExternalAndFluentAPIAttributesProxy proxy;
			if(!getExternalAndFluentAPIAttributesCache.TryGetValue(metadataHelperType, out proxy)) {
				var getMethod =
					metadataHelperType.GetMethod("GetExternalAndFluentAPIAttrbutes", new Type[] { typeof(Type), typeof(string) }) ?? 
					metadataHelperType.GetMethod("GetExtenalAndFluentAPIAttrbutes", new Type[] { typeof(Type), typeof(string) }) ??
					metadataHelperType.GetMethod("GetExtenalAndFluentAPIAttributes", new Type[] { typeof(Type), typeof(string) });
				if(getMethod != null) {
					proxy = new GetExternalAndFluentAPIAttributesProxy(getMethod, metadataHelperType);
					getExternalAndFluentAPIAttributesCache.Add(metadataHelperType, proxy);
				}
			}
			return (proxy != null) ? proxy.Get(componentType, memberName) : new Attribute[0];
		}
		static IDictionary<Type, IGetExternalAndFluentAPIAttributesProxy> getExternalAndFluentAPIFilteringAttributesCache = new Dictionary<Type, IGetExternalAndFluentAPIAttributesProxy>();
		static IEnumerable<Attribute> GetExternalAndFluentAPIFilteringAttributes(Type metadataHelperType, Type componentType, string memberName) {
			IGetExternalAndFluentAPIAttributesProxy proxy;
			if(!getExternalAndFluentAPIFilteringAttributesCache.TryGetValue(metadataHelperType, out proxy)) {
				var getMethod =
					metadataHelperType.GetMethod("GetExternalAndFluentAPIFilteringAttrbutes", new Type[] { typeof(Type), typeof(string) }) ?? 
					metadataHelperType.GetMethod("GetExternalAndFluentAPIFilteringAttributes", new Type[] { typeof(Type), typeof(string) });
				if(getMethod != null) {
					proxy = new GetExternalAndFluentAPIAttributesProxy(getMethod, metadataHelperType);
					getExternalAndFluentAPIFilteringAttributesCache.Add(metadataHelperType, proxy);
				}
			}
			return (proxy != null) ? proxy.Get(componentType, memberName) : new Attribute[0];
		}
		#region  GetExternalAndFluentAPIAttributes proxy
		interface IGetExternalAndFluentAPIAttributesProxy {
			IEnumerable<Attribute> Get(Type componentType, string memberName);
		}
		sealed class GetExternalAndFluentAPIAttributesProxy : IGetExternalAndFluentAPIAttributesProxy {
			Func<Type, string, IEnumerable<Attribute>> get;
			public GetExternalAndFluentAPIAttributesProxy(MethodInfo mInfo, Type metadataHelperType) {
				var componentType = Expression.Parameter(typeof(Type), "componentType");
				var memberName = Expression.Parameter(typeof(string), "memberName");
				get = Expression.Lambda<Func<Type, string, IEnumerable<Attribute>>>(
								Expression.Call(mInfo, componentType, memberName), componentType, memberName).Compile();
			}
			IEnumerable<Attribute> IGetExternalAndFluentAPIAttributesProxy.Get(Type componentType, string memberName) {
				return get(componentType, memberName);
			}
		}
		#endregion
		static Type metadataHelperType;
		static Type GetMetadataHelperType() {
			return DevExpress.Utils.MVVM.MVVMAssemblyProxy.GetMvvmType(ref metadataHelperType, "Native.MetadataHelper");
		}
		internal static void Reset() {
			metadataHelperType = null;
			getExternalAndFluentAPIAttributesCache.Clear();
			getExternalAndFluentAPIFilteringAttributesCache.Clear();
		}
	}
}
