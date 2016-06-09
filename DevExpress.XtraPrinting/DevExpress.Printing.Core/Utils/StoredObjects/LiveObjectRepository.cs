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
using System.Linq;
using System.Text;
namespace DevExpress.Utils.StoredObjects {
	class LiveObjectRepository<T> : IObjectRepository<T>, IDisposable {
		List<object> objectStore = new List<object>();
		event EventHandler<StoredObjectEventArgs> IObjectRepository<T>.ObjectChanged {
			add { throw new NotSupportedException(); }
			remove { throw new NotSupportedException(); }
		}
		public long StoreObject(IRepositoryProvider provider, T obj) {
			if(obj is IStoredObject)
				return StoreObjectCore((IStoredObject)obj);
			int index = objectStore.IndexOf(obj);
			if(index >= 0) return index;
			objectStore.Add(obj);
			return objectStore.Count - 1;
		}
		long StoreObjectCore(IStoredObject obj) {
			if(!obj.HasId()) {
				objectStore.Add(obj);
				obj.Id = objectStore.Count - 1;
			} else
				System.Diagnostics.Debug.Assert(ReferenceEquals(obj, objectStore[(int)obj.Id]));
			return obj.Id;
		}
		public bool TryRestoreObject<TObj>(IRepositoryProvider provider, long id, out TObj obj) where TObj : T {
			if(id >= 0 && id < objectStore.Count) {
				obj = (TObj)objectStore[(int)id];
				return true;
			}
			obj = default(TObj);
			return false;
		}
		public void Dispose() {
			objectStore.Clear();
		}
	}
}
