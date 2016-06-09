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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
namespace DevExpress.XtraDiagram.ViewInfo {
	public class DiagramItems {
		Hashtable items;
		DiagramItemInfoCollection orderedList;
		public DiagramItems() {
			this.items = new Hashtable();
			this.orderedList = new DiagramItemInfoCollection();
		}
		public void Add(DiagramItem item, DiagramItemInfo diagramItemInfo) {
			this.items.Add(item, diagramItemInfo);
			this.orderedList.Add(diagramItemInfo);
		}
		public int Count { get { return this.items.Count; } }
		public DiagramItemInfo this[DiagramItem item] {
			get { return this.items[item] as DiagramItemInfo; }
		}
		public void ClearItems() {
			if(Count == 0) return;
			foreach(DiagramItemInfo itemInfo in VisibleItems) {
				itemInfo.Dispose();
			}
			this.items.Clear();
			this.orderedList.Clear();
		}
		public IList VisibleItems { get { return this.orderedList; } }
	}
	public class DiagramItemInfoCollection : Collection<DiagramItemInfo> {
		public DiagramItemInfoCollection() {
		}
	}
}
