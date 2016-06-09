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

using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Xpf.Core {
	public static class DeferredResourceLoader {
		static readonly Dictionary<object, List<DeferredThemePartResourceDictionary>> ResourcesCache = new Dictionary<object, List<DeferredThemePartResourceDictionary>>();
		static readonly List<object> LoadingList = new List<object>();
		static readonly Dictionary<object, object> LoadedCache = new Dictionary<object, object>();
		public static void Register(object groupingKey, DeferredThemePartResourceDictionary dictionary) {
			List<DeferredThemePartResourceDictionary> list = GetOrCreateContainer(groupingKey);
			list.Add(dictionary);
			FlushLoadingList();
		}
		static List<DeferredThemePartResourceDictionary> GetOrCreateContainer(object groupingKey) {
			List<DeferredThemePartResourceDictionary> result;
			if (!ResourcesCache.TryGetValue(groupingKey, out result)) {
				result = new List<DeferredThemePartResourceDictionary>();
				ResourcesCache.Add(groupingKey, result);
			}
			return result;
		}
		public static void Load(object groupingKey) {
			if (LoadedCache.ContainsKey(groupingKey))
				return;
			if (ResourcesCache.ContainsKey(groupingKey))
				PerformLoad(groupingKey);
			else
				LoadingList.Add(groupingKey);
		}
		static void PerformLoad(object groupingKey) {
			List<DeferredThemePartResourceDictionary> list = ResourcesCache[groupingKey];
			foreach (DeferredThemePartResourceDictionary dictionary in list)
				ThemePartResourceDictionary.EnableSource(dictionary);
		}
		static void FlushLoadingList() {
			foreach (object key in LoadingList) {
				if (ResourcesCache.ContainsKey(key))
					PerformLoad(key);
			}
		}
	}
}
