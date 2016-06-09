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
using System.Linq;
using System.Text;
using DevExpress.Office;
using DevExpress.Export.Xl;
namespace DevExpress.SpreadsheetSource {
	#region ICell
	public interface ICell {
		int FieldIndex { get; }
		XlVariantValue Value { get; }
	}
	#endregion
	#region ICellCollection
	public interface ICellCollection : IEnumerable<ICell>, ICollection {
		ICell this[int index] { get; }
	}
	#endregion
}
namespace DevExpress.SpreadsheetSource.Implementation {
	#region Cell
	public class Cell : ICell {
		internal Cell(int fieldIndex, XlVariantValue value) 
			: this(fieldIndex, value, fieldIndex, 0) {
		}
		public Cell(int fieldIndex, XlVariantValue value, int columnIndex, int formatIndex) {
			FieldIndex = fieldIndex;
			Value = value;
			ColumnIndex = columnIndex;
			FormatIndex = formatIndex;
		}
		#region ICell Members
		public int FieldIndex { get; private set; }
		public XlVariantValue Value { get; private set; }
		public int ColumnIndex { get; private set; }
		public int FormatIndex { get; private set; }
		#endregion
	}
	#endregion
	#region CellCollection
	public class CellCollection : ICellCollection {
		readonly List<ICell> innerList = new List<ICell>();
		public CellCollection() {
		}
		public void Add(ICell item) {
			innerList.Add(item);
		}
		public void Clear() {
			innerList.Clear();
		}
		#region ICellCollection Members
		public ICell this[int index] {
			get { return innerList[index]; }
		}
		#endregion
		#region IEnumerable<ICell> Members
		public IEnumerator<ICell> GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)innerList).GetEnumerator();
		}
		#endregion
		#region ICollection Members
		public void CopyTo(Array array, int index) {
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		public int Count {
			get { return innerList.Count; }
		}
		public bool IsSynchronized {
			get {
				ICollection collection = innerList;
				return collection.IsSynchronized;
			}
		}
		public object SyncRoot {
			get {
				ICollection collection = innerList;
				return collection.SyncRoot;
			}
		}
		#endregion
	}
	#endregion
}
