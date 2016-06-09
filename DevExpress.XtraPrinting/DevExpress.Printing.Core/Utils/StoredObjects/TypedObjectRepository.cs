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
namespace DevExpress.Utils.StoredObjects {
	abstract class TypedRepositoryBase<T> : IObjectRepository<T>, IDisposable {
		IPersistentDictionary store;
		event EventHandler<StoredObjectEventArgs> IObjectRepository<T>.ObjectChanged {
			add { throw new NotSupportedException(); }
			remove { throw new NotSupportedException(); }
		}
		protected TypedRepositoryBase(IPersistentDictionary store) {
			this.store = store;
		}
		public long StoreObject(IRepositoryProvider provider, T obj) {
			return store.AddValue(SerializeObject(obj));
		}
		public bool TryRestoreObject<TObj>(IRepositoryProvider provider, long id, out TObj obj) where TObj : T {
			byte[] value;
			if(store.TryGetValue(id, out value)) {
				obj = (TObj)DeserializeObject(value);
				return true;
			}
			obj = default(TObj);
			return false;
		}
		protected abstract byte[] SerializeObject(T obj);
		protected abstract T DeserializeObject(byte[] value);
		public virtual void Dispose() {
			store.Dispose();
		}
	}
	class StringRepository : TypedRepositoryBase<string> {
		public StringRepository(IPersistentDictionary store) : base(store) { 
		}
		protected override byte[] SerializeObject(string obj) {
			return Encoding.UTF8.GetBytes(obj);
		}
		protected override string DeserializeObject(byte[] value) {
			return Encoding.UTF8.GetString(value);
		}
	}
}
