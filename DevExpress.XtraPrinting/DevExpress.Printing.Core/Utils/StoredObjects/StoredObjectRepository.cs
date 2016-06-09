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
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Data.Utils;
namespace DevExpress.Utils.StoredObjects {
	public class InvalidRestoreException : Exception {
	}
	public class InvalidStoreException : Exception {
	}
	interface ITypeProvider {
		long GetTypeId(Type type);
		bool TryGetType(long typeId, out Type type);
		object CreateInstance(Type type);
	}
	class RepositoryObjectChangedWeakEventHandler<T> : WeakEventHandler<T, StoredObjectEventArgs, EventHandler<StoredObjectEventArgs>> where T : class {
		static Action<WeakEventHandler<T, StoredObjectEventArgs, EventHandler<StoredObjectEventArgs>>, object> onDetachAction = (h, o) => {
			((IObjectRepository<T>)o).ObjectChanged -= h.Handler;
		};
		static Func<WeakEventHandler<T, StoredObjectEventArgs, EventHandler<StoredObjectEventArgs>>, EventHandler<StoredObjectEventArgs>> createHandlerFunction = h => h.OnEvent;
		public RepositoryObjectChangedWeakEventHandler(T owner, Action<T, object, StoredObjectEventArgs> onEventAction)
			: base(owner, onEventAction, onDetachAction, createHandlerFunction) {
		}
	}
	abstract class ObjectRepositoryBase<T> : IObjectRepository<T>, IDisposable where T : IStoredObject {
		SortedDictionary<long, byte[]> buffer = new SortedDictionary<long, byte[]>();
		public event EventHandler<StoredObjectEventArgs> ObjectChanged;
		void RaiseObjectChanged(StoredObjectEventArgs e) {
			if(ObjectChanged != null)
				ObjectChanged(this, e);
		}
		public abstract void Dispose();
		public long StoreObject(IRepositoryProvider provider, T obj) {
			if(obj == null) 
				throw new ArgumentNullException();
			if(obj.Id == StoredObjectExtentions.UndefinedId) {
				ITypeProvider typeProvider = provider.GetService(typeof(ITypeProvider)) as ITypeProvider;
				byte[] store = obj.Store(provider);
				long storeId = AddStore(store);
				obj.Id = (storeId << 8) | typeProvider.GetTypeId(obj.GetType());
			} else {
				SetStore(obj.Id >> 8, obj.Store(provider));
				RaiseObjectChanged(new StoredObjectEventArgs(obj.Id));
			}
			return obj.Id;
		}
		void ClearBuffer() {
			foreach(KeyValuePair<long, byte[]> pair in buffer) {
				long storedId = AddStore(pair.Value);
				if(storedId != pair.Key)
					throw new InvalidOperationException();
			}
			buffer.Clear();
		}
		public virtual bool TryRestoreObject<TObj>(IRepositoryProvider provider, long id, out TObj obj) where TObj : T {
			Type type;
			byte[] store;
			long typeId = 0x00ff & id;
			ITypeProvider typeProvider = provider.GetService(typeof(ITypeProvider)) as ITypeProvider;
			if(!typeProvider.TryGetType(typeId, out type) || !TryGetStore(id >> 8, out store)) {
				obj = default(TObj);
				return false;
			}
			obj = (TObj)typeProvider.CreateInstance(type);
			obj.Restore(provider, store);
			obj.Id = id;
			return true;
		}
		protected abstract void SetStore(long id, byte[] store);
		protected abstract long PredictId(long count);
		protected abstract long AddStore(byte[] store);
		protected abstract bool TryGetStore(long id, out byte[] store);
	}
	class TypeProvider : ITypeProvider, IDisposable {
		long currentTypeId = 0;
		Dictionary<long, Type> objectTypes = new Dictionary<long, Type>();
		Dictionary<Type, long> objectTypeIds = new Dictionary<Type, long>();
		long ITypeProvider.GetTypeId(Type type) {
			long typeId;
			if(!objectTypeIds.TryGetValue(type, out typeId)) {
				typeId = ++currentTypeId;
				objectTypeIds[type] = typeId;
				objectTypes[typeId] = type;
			}
			return typeId;
		}
		bool ITypeProvider.TryGetType(long typeId, out Type type) {
			return objectTypes.TryGetValue(typeId, out type);
		}
		object ITypeProvider.CreateInstance(Type type) {
			return Activator.CreateInstance(type);
		}
		void IDisposable.Dispose() {
			objectTypes.Clear();
			objectTypeIds.Clear();
		}
	}
	class MemoryObjectRepository<T> : ObjectRepositoryBase<T> where T : IStoredObject {
		long currentId = 0;
		Dictionary<long, byte[]> objectStore = new Dictionary<long, byte[]>();
		public override void Dispose() {
			objectStore.Clear();
		}
		protected override void SetStore(long id, byte[] store) {
			objectStore[id] = store;
		}
		protected override long PredictId(long count) {
			return currentId + count;
		}
		protected override long AddStore(byte[] store) {
			try {
				objectStore[currentId] = store;
				return currentId;
			} finally {
				currentId++;
			}
		}
		protected override bool TryGetStore(long id, out byte[] store) {
			return objectStore.TryGetValue(id, out store);
		}
	}
}
