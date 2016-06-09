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
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	public sealed class DXHtmlTableRowCollection : ICollection, IEnumerable {
		DXHtmlTable owner;
		public int Count {
			get {
				if(owner.HasControls())
					return owner.Controls.Count;
				return 0;
			}
		}
		public DXHtmlTableRow this[int index] {
			get { return (DXHtmlTableRow)owner.Controls[index]; }
		}
		public object SyncRoot {
			get { return this; }
		}
		public bool IsSynchronized {
			get { return false; }
		}
		internal DXHtmlTableRowCollection(DXHtmlTable owner) {
			this.owner = owner;
		}
		public void Add(DXHtmlTableRow row) {
			Insert(-1, row);
		}
		public void Clear() {
			if(owner.HasChildren)
				owner.Controls.Clear();
		}
		public void CopyTo(Array array, int index) {
			IEnumerator enumerator = GetEnumerator();
			while(enumerator.MoveNext())
				array.SetValue(enumerator.Current, index++);
		}
		public IEnumerator GetEnumerator() {
			return owner.Controls.GetEnumerator();
		}
		public void Insert(int index, DXHtmlTableRow row) {
			owner.Controls.AddAt(index, row);
		}
		public void Remove(DXHtmlTableRow row) {
			owner.Controls.Remove(row);
		}
		public void RemoveAt(int index) {
			owner.Controls.RemoveAt(index);
		}
	}
}
