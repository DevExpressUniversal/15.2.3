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
using DevExpress.Office;
using DevExpress.Utils;
namespace DevExpress.Spreadsheet {
#if DXPORTABLE
	public
#else
	internal
#endif
		interface MergedCellsCollection : ISimpleCollection<Range> {
		void Clear();
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Office.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.Spreadsheet;
	using System.Collections;
#region NativeMergedCellsCollection
	partial class NativeMergedCellsCollection : MergedCellsCollection {
		List<NativeRange> innerList;
		NativeWorksheet worksheet;
		public NativeMergedCellsCollection(NativeWorksheet worksheet) {
			this.worksheet = worksheet;
			this.innerList = new List<NativeRange>();
			PopulateMergedCells();
		}
		Model.Worksheet ModelWorksheet { get { return worksheet.ModelWorksheet; } }
		public int Count { get { return ModelWorksheet.MergedCells.Count; } }
		public Range this[int index] {
			get { return innerList[index]; }
		}
		List<NativeRange> InnerList {
			get {
				return innerList; 
			}
		}
		void PopulateMergedCells() {
			innerList.Clear();
			ModelWorksheet.MergedCells.ForEach(RegisterMergedCellRange);
		}
		void RegisterMergedCellRange(Model.CellRange modelMergedRange) {
			NativeRange mergedRange = new NativeRange(modelMergedRange, worksheet);
			innerList.Add(mergedRange);
		}
		public void Clear() {
			ModelWorksheet.MergedCells.Clear();
		}
		public void OnAdd(Model.CellRange modelMergedRange) {
			innerList.Add(new NativeRange(modelMergedRange, worksheet));
		}
		public void OnRemove(Model.CellRange mergedRange) {
			foreach(NativeRange item in InnerList) {
				if(Object.ReferenceEquals(item.ModelRange, mergedRange)) {
					InnerList.Remove(item);
					return;
				}
			}
		}
		public void OnClear() {
			InnerList.Clear();
		}
#region ISimpleCollection members
#region GetEnumerator
		public IEnumerator<Range> GetEnumerator() {
			return new EnumeratorAdapter<Range, NativeRange>(innerList.GetEnumerator());
		}
#endregion
#region IEnumerable.GetEnumerator
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
#endregion
#region ICollection members
		object ICollection.SyncRoot {
			get {
				ICollection collection = innerList;
				return collection.SyncRoot;
			}
		}
		bool ICollection.IsSynchronized {
			get {
				ICollection collection = innerList;
				return collection.IsSynchronized;
			}
		}
		void ICollection.CopyTo(Array array, int index) {
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
#endregion
#endregion
	}
#endregion
}
