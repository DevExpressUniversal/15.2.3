#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Linq;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	public class WebActionContainersCollection : IList<WebActionContainer>, ICollection<WebActionContainer>, IEnumerable<WebActionContainer>, IList, ICollection, IEnumerable {
		private List<WebActionContainer> items = new List<WebActionContainer>();
		private bool isReadOnly = false;
		private ActionContainerHolder owner;
		public WebActionContainersCollection() { }
		private void CheckReadOnly() {
			if(isReadOnly) {
				throw new InvalidOperationException("Unable to modify ReadOnly collection.");
			}
		}
		public WebActionContainer this[int index] {
			get { return items[index]; }
			set {
				CheckReadOnly();
				items[index] = value;
			}
		}
		public void Add(WebActionContainer item) {
			CheckReadOnly();
			items.Add(item);
			item.Owner = Owner;
		}
		public bool Contains(WebActionContainer item) {
			return items.Contains(item);
		}
		public int IndexOf(WebActionContainer item) {
			return items.IndexOf(item);
		}
		public void Insert(int index, WebActionContainer item) {
			CheckReadOnly();
			items.Insert(index, item);
			item.Owner = Owner;
		}
		public bool Remove(WebActionContainer item) {
			CheckReadOnly();
			return items.Remove(item);
		}
		public void Clear() {
			CheckReadOnly();
			items.Clear();
		}
		public void RemoveAt(int index) {
			CheckReadOnly();
			items.RemoveAt(index);
		}
		public int Count {
			get { return items.Count; }
		}
		public void CopyTo(WebActionContainer[] array, int arrayIndex) {
			items.CopyTo(array, arrayIndex);
		}
		public void SetReadOnly(bool isReadOnly) {
			this.isReadOnly = isReadOnly;
		}
		public bool IsReadOnly { get { return isReadOnly; } }
		public IEnumerator<WebActionContainer> GetEnumerator() {
			return items.GetEnumerator();
		}
		internal ActionContainerHolder Owner {
			get { return owner; }
			set { owner = value; }
		}
		#region ICollection Members
		void ICollection.CopyTo(Array array, int arrayIndex) {
			items.CopyTo(array.Cast<WebActionContainer>().ToArray(), arrayIndex);
		}
		int ICollection.Count { get { return this.Count; } }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return null; } }
		#endregion
		#region IEnumerator Members
		IEnumerator IEnumerable.GetEnumerator() {
			return items.GetEnumerator();
		}
		#endregion
		#region IList Members
		object IList.this[int index] {
			get { return this[index]; }
			set {
				CheckReadOnly();
				if(value is WebActionContainer) this[index] = (WebActionContainer)value;
			}
		}
		int IList.Add(object item) {
			CheckReadOnly();
			if(item is WebActionContainer) {
				Add((WebActionContainer)item);
				return items.Count - 1;
			};
			return -1;
		}
		bool IList.Contains(object item) {
			return (item is WebActionContainer) ? Contains((WebActionContainer)item) : false;
		}
		int IList.IndexOf(object item) {
			return (item is WebActionContainer) ? IndexOf((WebActionContainer)item) : -1;
		}
		void IList.Insert(int index, object item) {
			CheckReadOnly();
			if(item is WebActionContainer) {
				Insert(index, (WebActionContainer)item);
			}
		}
		void IList.Remove(object item) {
			CheckReadOnly();
			if(item is WebActionContainer) Remove((WebActionContainer)item);
		}
		bool IList.IsFixedSize { get { return false; } }
		#endregion
	}
}
