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

using DevExpress.Compatibility.System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraPivotGrid.Selection {
		public class ReadOnlyCells : IEnumerable<Point> {
		readonly SelectionStorage store;
		internal ReadOnlyCells(SelectionStorage store) {
			this.store = store;
		}
		public ReadOnlyCells(List<Point> cells) {
			this.store = new SelectionStorage();
			foreach(Point cell in cells)
				store.Add(cell);
		}
		public ReadOnlyCells(Point cell)
			: this(new SelectionStorage()) {
			store.Add(cell);
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("ReadOnlyCellsIsEmpty")]
#endif
		public bool IsEmpty { get { return store.Count == 0; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("ReadOnlyCellsCount")]
#endif
		public int Count { get { return store.Count; } }
		public Point this[int index] { get { return store[index]; } }
		public virtual bool Contains(Point point) { return store.Contains(point); }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("ReadOnlyCellsRectangle")]
#endif
		public Rectangle Rectangle {
			get {
				if(IsEmpty)
					return Rectangle.Empty;
				return store.GetRect();
			}
		}
		public void CopyTo(List<Point> cells) {
			foreach(Point point in this)
				cells.Add(point);
		}
		public static ReadOnlyCells operator -(ReadOnlyCells c1, ReadOnlyCells c2) {
			SelectionStorage store = c1.store.Clone();
			store.Substract(c2.store);
			return new ReadOnlyCells(store);
		}
		#region IEnumerable<Point> Members
		public IEnumerator<Point> GetEnumerator() {
			return store.GetPointEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return store.GetPointEnumerator();
		}
		#endregion
		public static ReadOnlyCells Empty = new ReadOnlyCells(new SelectionStorage());
		internal IEnumerable<SelectionStorage.RegionSelection> GetRectEnumerable() {
			return store.GetRectEnumerable();
		}
		internal Dictionary<int, int> GetColumns() {
			return store.GetColumns();
		}
		internal Dictionary<int, int> GetRows() {
			return store.GetRows();
		}
	}
}
