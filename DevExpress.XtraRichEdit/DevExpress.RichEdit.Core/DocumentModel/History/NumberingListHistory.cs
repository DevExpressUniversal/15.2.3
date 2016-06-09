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
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model.History {
	public class AddAbstractNumberingListHistoryItem : RichEditHistoryItem {
		AbstractNumberingList abstractList;
		public AddAbstractNumberingListHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public AbstractNumberingList AbstractList { get { return abstractList; } set { abstractList = value; } }
		protected override void UndoCore() {
			AbstractNumberingListCollection lists = DocumentModel.AbstractNumberingLists;
			AbstractNumberingListIndex index = lists.IndexOf(abstractList);
			Debug.Assert(index == new AbstractNumberingListIndex(lists.Count - 1));
			abstractList.Deleted = true;
			lists.RemoveAt(index);
		}
		protected override void RedoCore() {
			abstractList.Deleted = false;
			DocumentModel.AbstractNumberingLists.Add(abstractList);
		}
	}
	public class AddNumberingListHistoryItem : RichEditHistoryItem {
		NumberingList numberingList;
		public AddNumberingListHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public NumberingList NumberingList { get { return numberingList; } set { numberingList = value; } }
		protected override void UndoCore() {
			NumberingListCollection lists = DocumentModel.NumberingLists;
			NumberingListIndex index = lists.IndexOf(numberingList);
			Debug.Assert(index == new NumberingListIndex(lists.Count - 1));
			lists.RemoveAt(index);
		}
		protected override void RedoCore() {
			DocumentModel.NumberingLists.Add(numberingList);
		}
	}
	public class ChangeListLevelHistoryItem<T> : RichEditHistoryItem where T : IListLevel {
		int indexLevel;
		T newLevel;
		T oldLevel;
		ListLevelCollection<T> listLevels;
		public ChangeListLevelHistoryItem(PieceTable pieceTable, ListLevelCollection<T> listLevels)
			: base(pieceTable) {
			this.listLevels = listLevels;
		}
		public int IndexLevel { get { return indexLevel; } set { indexLevel = value; } }
		public T NewLevel { get { return newLevel; } set { newLevel = value; } }
		public T OldLevel { get { return oldLevel; } set { oldLevel = value; } }
		protected override void UndoCore() {
			listLevels[IndexLevel] = OldLevel;
		}
		protected override void RedoCore() {
			listLevels[IndexLevel] = NewLevel;
		}
	} 
}
