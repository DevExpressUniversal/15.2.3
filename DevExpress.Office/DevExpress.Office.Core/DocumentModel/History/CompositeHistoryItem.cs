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
using DevExpress.Office.Utils;
using System.Diagnostics;
namespace DevExpress.Office.History {
	#region CompositeHistoryItem
	public class CompositeHistoryItem : HistoryItem {
		readonly List<HistoryItem> items;
		public CompositeHistoryItem(IDocumentModelPart part)
			: base(part) {
			items = new List<HistoryItem>();
		}
		public override bool ChangeModified {
			get {
				for (int i = 0; i < Count; i++) {
					if (items[i].ChangeModified)
						return true;
				}
				return false;
			}
		}
		public HistoryItem this[int index] { get { return items[index]; } }
		public int Count { get { return items.Count; } }
		public List<HistoryItem> Items { get { return items; } }
		public void AddItem(HistoryItem item) {
			items.Add(item);
		}
		protected override void UndoCore() {
			for (int i = items.Count - 1; i >= 0; i--) {
#if DEBUG
				int currentIndexBefore = DocumentModel.History.CurrentIndex;
				int countBefore = Count;
#endif
				this[i].Undo();
#if DEBUG
				System.Diagnostics.Debug.Assert(countBefore == Count);
				System.Diagnostics.Debug.Assert(currentIndexBefore == DocumentModel.History.CurrentIndex);
#endif
			}
		}
		protected internal virtual void Clear() {
			items.Clear();
		}
		protected override void RedoCore() {
			int count = items.Count;
			for (int i = 0; i < count; i++) {
#if DEBUG
				int currentIndexBefore = DocumentModel.History.CurrentIndex;
				int countBefore = Count;
#endif
				this[i].Redo();
#if DEBUG
				System.Diagnostics.Debug.Assert(countBefore == Count);
				System.Diagnostics.Debug.Assert(currentIndexBefore == DocumentModel.History.CurrentIndex);
#endif
			}
		}
		public void Rollback() {
			Undo();
			Clear();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				int count = items.Count;
				for (int i = 0; i < count; i++)
					this[i].Dispose();
			}
		}
	}
	#endregion
}
