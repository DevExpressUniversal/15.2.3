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
using System.Linq;
using System.Collections.Generic;
namespace DevExpress.Entity.ProjectModel {
	public class HasTypesCacheBase : IHasTypesCache {
		Dictionary<string, IDXTypeInfo> cache;
		public void ClearCache() {
			if(cache != null)
				cache = null;
		}
		protected Dictionary<string, IDXTypeInfo> Cache {
			get {
				if(cache == null)
					cache = new Dictionary<string, IDXTypeInfo>();
				return cache;
			}
		}
		protected bool IsCacheEmpty { get { return cache == null || cache.Count == 0; } }
		public void AddRange(IEnumerable<IDXTypeInfo> typesInfo) {
			if(typesInfo == null)
				return;
			foreach(IDXTypeInfo typeInfo in typesInfo)
				Add(typeInfo);
		}
		public virtual void Add(IDXTypeInfo typeInfo) {
			if(typeInfo == null || string.IsNullOrEmpty(typeInfo.FullName))
				return;			
			Cache[typeInfo.FullName] = typeInfo;
		}
		public IEnumerable<IDXTypeInfo> TypesInfo {
			get {
				if(IsCacheEmpty)
					return new IDXTypeInfo[0];
				return Cache.Values;
			}
		}
		public bool Contains(IDXTypeInfo typeInfo) {
			if(typeInfo == null || string.IsNullOrEmpty(typeInfo.FullName))
				return false;
			return GetTypeInfo(typeInfo.FullName) != null;
		}
		public IDXTypeInfo GetTypeInfo(string fullName) {
			if(string.IsNullOrEmpty(fullName))
				return null;
			IDXTypeInfo result;
			if(Cache.TryGetValue(fullName, out result))
				return result;
			return null;
		}
		public void Remove(IDXTypeInfo typeInfo) {
			if(typeInfo == null || IsCacheEmpty)
				return;
			if(Cache.ContainsKey(typeInfo.FullName))
				Cache.Remove(typeInfo.FullName);
		}
	}
}
