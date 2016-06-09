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
using DevExpress.Export.Xl;
namespace DevExpress.SpreadsheetSource {
	#region IWorksheet
	public interface IWorksheet {
		string Name { get; }
		XlSheetVisibleState VisibleState { get; }
	}
	#endregion
	#region IWorksheetCollection
	public interface IWorksheetCollection : IEnumerable<IWorksheet>, ICollection {
		IWorksheet this[int index] { get; }
		IWorksheet this[string name] { get; }
	}
	#endregion
}
namespace DevExpress.SpreadsheetSource.Implementation {
	#region Worksheet
	public class Worksheet : IWorksheet {
		public Worksheet(string name, XlSheetVisibleState visibleState) {
			Name = name;
			VisibleState = visibleState;
		}
		#region IWorksheet Members
		public string Name { get; private set; }
		public XlSheetVisibleState VisibleState { get; private set; }
		#endregion
	}
	#endregion
	#region WorksheetCollection
	public class WorksheetCollection : IWorksheetCollection {
		readonly List<IWorksheet> innerList = new List<IWorksheet>();
		public WorksheetCollection() {
		}
		public void Add(IWorksheet sheet) {
			innerList.Add(sheet);
		}
		public void Clear() {
			innerList.Clear();
		}
		public void RemoveAt(int index) {
			innerList.RemoveAt(index);
		}
		#region IWorksheetCollection Members
		public IWorksheet this[string name] {
			get {
				foreach(IWorksheet sheet in innerList) {
					if(sheet.Name == name)
						return sheet;
				}
				return null; 
			}
		}
		public IWorksheet this[int index] {
			get { return innerList[index]; }
		}
		#endregion
		#region IEnumerable<IWorksheet> Members
		public IEnumerator<IWorksheet> GetEnumerator() {
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
