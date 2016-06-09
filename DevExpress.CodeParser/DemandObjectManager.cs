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
using System.Collections;
#if SL
using DevExpress.Utils;
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.Patterns
#else
namespace DevExpress.CodeParser.Patterns
#endif
{
	public interface IDemandObjectProxy
	{
		object Key { get; }
		DemandObjectManager Manager { get; set; }
	}
	public abstract class DemandObjectManager : IDisposable
	{
		Hashtable _ProxiesHash;
		Hashtable _Proxies;
		ArrayList _ObjectsToLive;
		~DemandObjectManager()
		{
			Dispose(false);
		}
		int GetLiveObjectsCount()
		{
			int lCount = 0;
			lock (this)
			{
				foreach (DictionaryEntry lEntry in Proxies)
				{
					object lObject = GetAliveObject((IDemandObjectProxy)lEntry.Key);
					if (lObject != null)
						lCount ++;
				}
			}
			return lCount;
		}
		protected virtual void Dispose(bool disposing)
		{
			lock (this)
			{
				foreach (DictionaryEntry lEntry in Proxies)
				{
					IDemandObjectProxy lProxy = (IDemandObjectProxy)lEntry.Key;
					if (lProxy != null)
						lProxy.Manager = null;
				}
				if (_Proxies != null)
				{
					_Proxies.Clear();
					_Proxies = null;
				}
				if (_ProxiesHash != null)
				{
					_ProxiesHash.Clear();
					_ProxiesHash = null;
				}
				if (_ObjectsToLive != null)
				{
					_ObjectsToLive.Clear();
					_ObjectsToLive = null;
				}
			}
		}
		protected void RegisterObject(IDemandObjectProxy proxy, object obj)
		{
			if (proxy == null)
				return;
			lock (this)
			{
				WeakReference lReference = null;
				if (obj != null)
					lReference = new WeakReference(obj);
				Proxies.Remove(proxy);
				Proxies.Add(proxy, lReference);
				if (obj != null)
					ObjectsToLive.Add(obj);
			}
		}
		protected abstract object CreateObjectForProxy(IDemandObjectProxy proxy);
		public void UpdateKey(object oldKey, object newKey)
		{
			if (oldKey == null || newKey == null)
				return;
			if (!ProxiesHash.Contains(oldKey))
				return;
			object lValue = ProxiesHash[oldKey];
			ProxiesHash.Remove(oldKey);
			ProxiesHash.Add(newKey, lValue);				
		}
		public void AddProxy(IDemandObjectProxy proxy)
		{
			if (proxy == null)
				return;
			lock (this)
			{
				proxy.Manager = this;
				Proxies.Add(proxy, null);
				ProxiesHash.Add(proxy.Key, proxy);
			}
		}
		public void RemoveProxy(IDemandObjectProxy proxy)
		{
			if (proxy == null)
				return;
			ReleaseObject(proxy);
			lock (this)
			{
				proxy.Manager = null;
				Proxies.Remove(proxy);
				ProxiesHash.Remove(proxy.Key);
			}
		}
		public object RequestObject(IDemandObjectProxy proxy)
		{
			return RequestObject(proxy, false);
		}
		public object RequestObject(IDemandObjectProxy proxy, bool forceCreate)
		{
			object lObject = GetAliveObject(proxy);
			if (lObject != null && !forceCreate)
				return lObject;
			lObject = CreateObjectForProxy(proxy);
			if (lObject == null)
				return null;
			RegisterObject(proxy, lObject);
			return lObject;
		}
		public void ReleaseObject(IDemandObjectProxy proxy)
		{
			object lObject = GetAliveObject(proxy);
			if (lObject != null)
			{
				lock (this)
					ObjectsToLive.Remove(lObject);
			}
		}
		public void ReleaseAllObjects()
		{
			lock (this)
			{
				foreach (DictionaryEntry lEntry in Proxies)
				{
					IDemandObjectProxy lProxy = lEntry.Key as IDemandObjectProxy;
					if (lProxy != null)
						ReleaseObject(lProxy);
				}
			}
		}
		public void ReplaceObject(IDemandObjectProxy proxy, object obj)
		{
			if (proxy == null)
				return;
			ReleaseObject(proxy);
			RegisterObject(proxy, obj);
		}
		public object GetAliveObject(IDemandObjectProxy proxy)
		{
			if (proxy == null)
				return null;
			lock (this)
			{
				WeakReference lReference = (WeakReference)Proxies[proxy];
				if (lReference == null)
					return null;
				if (lReference.IsAlive)
					return lReference.Target;
			}
			return null;
		}
		protected ArrayList ObjectsToLive
		{
			get
			{
				if (_ObjectsToLive == null)
					_ObjectsToLive = new ArrayList();
				return _ObjectsToLive;
			}
		}
		protected Hashtable Proxies
		{
			get
			{
				if (_Proxies == null)
					_Proxies = new Hashtable();
				return _Proxies;
			}
		}
		protected int ProxiesCount
		{
			get
			{
				if (_Proxies == null)
					return 0;
				return _Proxies.Count;
			}
		}
		protected Hashtable ProxiesHash
		{
			get
			{
				if (_ProxiesHash == null)
					_ProxiesHash = new Hashtable(StringComparer.CurrentCultureIgnoreCase);
				return _ProxiesHash;
			}
		}
		public int LiveObjectsCount
		{
			get
			{
				return GetLiveObjectsCount();
			}
		}
		#region Dispose
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
