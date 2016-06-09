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
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
namespace DevExpress.Spreadsheet {
	public interface PageBreaksCollection : ISimpleCollection<int> {
		void Add(int position);
		int IndexOf(int position);
		bool Contains(int position);
		void Remove(int position);
		void RemoveAt(int index);
		void Clear();
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using System.Collections;
	using System.Collections.Generic;
	partial class NativePageBreaksCollection : NativeObjectBase, PageBreaksCollection {
		readonly Model.PageBreakCollection modelCollection;
		readonly int maxPosition;
		public NativePageBreaksCollection(Model.PageBreakCollection modelCollection, int maxPosition) {
			Guard.ArgumentNotNull(modelCollection, "modelCollection");
			this.modelCollection = modelCollection;
			this.maxPosition = maxPosition;
		}
		public int this[int index] {
			get {
				CheckValid();
				return modelCollection[index];
			}
		}
		public int Count { 
			get {
				CheckValid();
				return modelCollection.Count; 
			} 
		}
		#region PageBreaksCollection Members
		public void Add(int position) {
			CheckValid();
			CheckPosition(position);
			modelCollection.TryInsert(position);
		}
		public int IndexOf(int position) {
			CheckValid();
			return modelCollection.IndexOf(position);
		}
		public bool Contains(int position) {
			CheckValid();
			return modelCollection.Contains(position);
		}
		public void Remove(int position) {
			CheckValid();
			modelCollection.Remove(position);
		}
		public void RemoveAt(int index) {
			CheckValid();
			if (index < 0 || index >= modelCollection.Count)
				throw new ArgumentOutOfRangeException("index");
			modelCollection.Remove(modelCollection[index]);
		}
		public void Clear() {
			CheckValid();
			modelCollection.Clear();
		}
		#endregion
		#region IEnumerable<int> Members
		public IEnumerator<int> GetEnumerator() {
			CheckValid();
			return new EnumeratorAdapter<int, int>(modelCollection.InnerList.GetEnumerator());
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			CheckValid();
			return modelCollection.InnerList.GetEnumerator();
		}
		#endregion
		#region ICollection Members
		bool ICollection.IsSynchronized { 
			get {
				CheckValid();
				return ((ICollection)modelCollection.InnerList).IsSynchronized; 
			} 
		}
		object ICollection.SyncRoot { 
			get {
				CheckValid();
				return ((ICollection)modelCollection.InnerList).SyncRoot; 
			} 
		}
		void ICollection.CopyTo(Array array, int index) {
			CheckValid();
			Array.Copy(modelCollection.InnerList.ToArray(), 0, array, index, modelCollection.InnerList.Count);
		}
		#endregion
		void CheckPosition(int position) {
			if (position <= 0 || position > maxPosition)
				throw new ArgumentOutOfRangeException("position");
		}
	}
}
