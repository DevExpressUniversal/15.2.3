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
using System.Collections.Generic;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.Native {
	public interface IObjectContainer {
		int Count { get; }
		void Clear();
	}
	public class ObjectContainer<T> : IObjectContainer, IEnumerable<T> where T : IDisposable {
		readonly Dictionary<object, T> items;
		public int Count {
			get { return items.Count; }
		}
		protected IDictionary<object, T> Items {
			get { return items; }
		}
		protected virtual IEqualityComparer<object> EqualityComparer {
			get { return DefaultEqualityComparer.Instance; }
		}
		public ObjectContainer() {
			items = new Dictionary<object, T>(EqualityComparer);
		}
		public virtual void Clear() {
			foreach(IDisposable entry in items.Values)
				entry.Dispose();
			items.Clear();
		}
		protected T GetObject(object key, T obj) {
			Guard.ArgumentNotNull(obj, "obj");
			Guard.ArgumentNotNull(key, "key");
			T entry;
			if(items.TryGetValue(key, out entry)) {
				if(!object.ReferenceEquals(entry, obj))
					obj.Dispose();
				return entry;
			}
			items.Add(key, obj);
			return obj;
		}
		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator() {
			return items.Values.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
	}
}
