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
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public class Stack<T> where T : class {
		internal T[] items;
		internal int count;
		internal int capacity;
		internal int growth;
		public Stack(int growth) {
			this.growth = growth;
		}
		public T this[int i] {
			get { return (i >= 0 && i < capacity) ? items[i] : null; }
			internal set { items[i] = value; }
		}
		public int Count {
			get { return count; }
		}
		public T TopItemUnsafe {
			get { return this.items[this.count - 1]; }
			set { this.items[this.count - 1] = value; }
		}
		public T Pop() {
			if(this.count <= 0)
				throw new InvalidOperationException("Stack is empty");
			return (--this.count > 0) ? this.items[this.count - 1] : null;
		}
		public T PushSlot() {
			if(this.count == this.capacity) {
				this.capacity += this.growth;
				T[] newItems = new T[this.capacity];
				if(this.items != null)
					Array.Copy(this.items, newItems, this.count);
				this.items = newItems;
			}
			return this.items[this.count++];
		}
		public void RemoveAt(int i) {
			if(i < 0 || i >= this.count)
				throw new InvalidOperationException();
			if(i < this.count - 1) {
				T itemToRemove = this.items[i];
				this.items[i] = null;
				Array.Copy(this.items, i + 1, this.items, i, this.count - i - 1);
				this.items[this.count - 1] = itemToRemove;
			}
			this.count--;
		}
		public void Reset() {
			this.count = 0;
		}
	}
	public struct EndTagTransformQuery {
		private string newTagName;
		private int depth;
		public EndTagTransformQuery(string newTagName, int depth) {
			this.newTagName = newTagName;
			this.depth = depth;
		}
		public string NewTagName {
			get { return newTagName; }
		}
		public int Depth {
			get { return depth; }
		}
		public override int GetHashCode() {
			return this.newTagName.GetHashCode() ^ this.depth;
		}
		public override bool Equals(object obj) {
			if(obj == null || !(obj is EndTagTransformQuery))
				return false;
			EndTagTransformQuery q = (EndTagTransformQuery)obj;
			return this.depth == q.depth && this.newTagName == q.newTagName;
		}
		public override string ToString() {
			return string.Format("{0} ({1})", this.newTagName, this.depth);
		}
	}
	public class RequiredAttributePresenceList {
		internal const int MinGrowth = 32;
		internal bool[] presenceList;
		public RequiredAttributePresenceList(int capacity) {
			this.presenceList = new bool[capacity];
		}
		public bool this[int i] {
			get { return (i >= 0 && i < this.presenceList.Length) ? this.presenceList[i] : false; }
			set {
				if(i >= this.presenceList.Length) {
					int newSize = Math.Max(i + 1, this.presenceList.Length + MinGrowth);
					bool[] newList = new bool[newSize];
					Array.Copy(this.presenceList, newList, this.presenceList.Length);
					this.presenceList = newList;
				}
				this.presenceList[i] = value;
			}
		}
		public void Clear() {
			Array.Clear(presenceList, 0, presenceList.Length);
		}
	}
}
