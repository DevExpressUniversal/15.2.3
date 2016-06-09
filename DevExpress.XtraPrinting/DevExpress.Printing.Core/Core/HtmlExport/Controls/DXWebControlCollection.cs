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
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	public class DXWebControlCollection : ICollection, IEnumerable {
		List<DXWebControlBase> controls = new List<DXWebControlBase>(5);
		int growthFactor;
		DXWebControlBase owner;
		string readOnlyErrorMsg;
		int version;
		public virtual int Count {
			get { return controls.Count; }
		}
		public bool IsReadOnly {
			get { return readOnlyErrorMsg != null; }
		}
		public bool IsSynchronized {
			get { return false; }
		}
		public virtual DXWebControlBase this[int index] {
			get {
				if(index < 0 || index >= controls.Count)
					throw new ArgumentOutOfRangeException("index");
				return controls[index];
			}
		}
		protected DXWebControlBase Owner {
			get { return owner; }
		}
		public object SyncRoot {
			get { return this; }
		}
		public DXWebControlCollection(DXWebControlBase owner) {
			growthFactor = 4;
			if(owner == null)
				throw new ArgumentNullException("owner");
			this.owner = owner;
		}
		internal DXWebControlCollection(DXWebControlBase owner, int growthFactor) {
			Guard.ArgumentNotNull(owner, "owner");
			growthFactor = 4;
			this.owner = owner;
			this.growthFactor = growthFactor;
		}
		public virtual void Add(DXWebControlBase child) {
			Guard.ArgumentNotNull(child, "child");
			if(readOnlyErrorMsg != null)
				throw new Exception(readOnlyErrorMsg);
			controls.Add(child);
			version++;
			owner.AddedControl(child, controls.Count - 1);
		}
		public virtual void AddAt(int index, DXWebControlBase child) {
			if(index == -1) {
				Add(child);
				return;
			}
			Guard.ArgumentNotNull(child, "child");
			if(index < 0 || index > controls.Count)
				throw new ArgumentOutOfRangeException("index");
			if(readOnlyErrorMsg != null)
				throw new Exception(readOnlyErrorMsg);
			controls.Insert(index, child);
			version++;
			owner.AddedControl(child, index);
		}
		public virtual void Clear() {
			if(controls != null)
				for(int i = controls.Count - 1; i >= 0; i--)
					RemoveAt(i);
		}
		public virtual bool Contains(DXWebControlBase c) {
			if(c != null)
				return controls.Contains(c);
			return false;
		}
		public virtual void CopyTo(Array array, int index) {
			if(!(array is DXWebControlBase[]) || array.Rank != 1)
				throw new Exception("InvalidArgumentValue");
			Array.Copy(controls.ToArray(), 0, array, index, controls.Count);
		}
		public virtual IEnumerator GetEnumerator() {
			for(int i = 0; i < Count; i++)
				yield return this[i];
		}
		public virtual int IndexOf(DXWebControlBase value) {
			return controls.IndexOf(value);
		}
		public virtual void Remove(DXWebControlBase value) {
			controls.Remove(value);
		}
		public virtual void RemoveAt(int index) {
			if(readOnlyErrorMsg != null)
				throw new Exception(readOnlyErrorMsg);
			DXWebControlBase control = this[index];
			controls.RemoveAt(index);
			version++;
			owner.RemovedControl(control);
		}
		internal string SetCollectionReadOnly(string errorMsg) {
			string str = readOnlyErrorMsg;
			readOnlyErrorMsg = errorMsg;
			return str;
		}
	}
}
