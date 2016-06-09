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
	#region IDefinedName
	public interface IDefinedName {
		string Name { get; }
		string Scope { get; }
		bool IsHidden { get; }
		XlCellRange Range { get; }
		string RefersTo { get; }
		string Comment { get; }
	}
	#endregion
	#region IDefinedNamesCollection
	public interface IDefinedNamesCollection : IEnumerable<IDefinedName>, ICollection {
		IDefinedName this[int index] { get; }
		IDefinedName FindBy(string name, string scope);
	}
	#endregion
}
namespace DevExpress.SpreadsheetSource.Implementation {
	using DevExpress.Utils;
	#region DefinedName
	public class DefinedName : IDefinedName {
		public DefinedName(string name, string scope, bool isHidden, XlCellRange range, string refersTo) {
			Guard.ArgumentIsNotNullOrEmpty(name, "name");
			Guard.ArgumentNotNull(range, "range");
			Guard.ArgumentIsNotNullOrEmpty(refersTo, "refersTo");
			Name = name;
			Scope = scope;
			IsHidden = isHidden;
			Range = range;
			RefersTo = refersTo;
		}
		#region IDefinedName Members
		public string Name { get; private set; }
		public string Scope { get; private set; }
		public bool IsHidden { get; private set; }
		public XlCellRange Range { get; private set; }
		public string RefersTo { get; private set; }
		public string Comment { get; set; }
		#endregion
	}
	#endregion
	#region DefinedNamesCollection
	public class DefinedNamesCollection : IDefinedNamesCollection {
		readonly List<IDefinedName> innerList = new List<IDefinedName>();
		public DefinedNamesCollection() {
		}
		public void Add(IDefinedName item) {
			innerList.Add(item);
		}
		public void Clear() {
			innerList.Clear();
		}
		#region IDefinedNamesCollection Members
		public IDefinedName FindBy(string name, string scope) {
			foreach(IDefinedName item in innerList) {
				if(item.Name == name && (item.Scope == scope || (string.IsNullOrEmpty(item.Scope) && string.IsNullOrEmpty(scope))))
					return item;
			}
			return null;
		}
		public IDefinedName this[int index] {
			get { return innerList[index]; }
		}
		#endregion
		#region IEnumerable<IDefinedName> Members
		public IEnumerator<IDefinedName> GetEnumerator() {
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
