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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region CommentRunCollection
	public class CommentRunCollection {
		#region Fields
		readonly List<CommentRun> innerList;
		readonly Comment comment;
		#endregion
		public CommentRunCollection(Comment comment) {
			Guard.ArgumentNotNull(comment, "comment");
			this.comment = comment;
			this.innerList = new List<CommentRun>();
		}
		#region Properties
		public DocumentModel Workbook { get { return comment.Workbook; } }
		public Worksheet Worksheet { get { return comment.Worksheet; } }
		public int Count { get { return innerList.Count; } }
		public CommentRun this[int index] { get { return innerList[index]; } }
		protected internal List<CommentRun> InnerList { get { return innerList; } }
		#endregion
		#region Add
		public void Add(CommentRun run) {
			DocumentHistory history = Workbook.History;
			CommentRunAddHistoryItem historyItem = new CommentRunAddHistoryItem(this, run);
			history.Add(historyItem);
			historyItem.Execute();
		}
		#endregion
		#region Add()
		public CommentRun Add() {
			CommentRun run = new CommentRun(Worksheet);
			Add(run);
			return run;
		}
		#endregion
		#region RemoveAt
		public void RemoveAt(int index) {
			if(index < 0 || index >= Count)
				Exceptions.ThrowArgumentException("index", index);
			DocumentHistory history = Workbook.History;
			CommentRunRemoveAtHistoryItem historyItem = new CommentRunRemoveAtHistoryItem(this, index);
			history.Add(historyItem);
			historyItem.Execute();
		}
		#endregion
		#region Remove
		public void Remove(CommentRun run) {
			int index = IndexOf(run);
			RemoveAt(index);
		}
		#endregion
		#region RemoveAtCore
		public void RemoveAtCore(int index) {
			innerList.RemoveAt(index);
		}
		#endregion
		#region IndexOf
		public int IndexOf(CommentRun item) {
			return innerList.IndexOf(item);
		}
		#endregion
		#region Contains
		public bool Contains(CommentRun item) {
			return this.innerList.Contains(item);
		}
		#endregion
		#region ForEach
		public void ForEach(Action<CommentRun> action) {
			innerList.ForEach(action);
		}
		#endregion
		#region AddCore
		public int AddCore(CommentRun item) {
			Guard.ArgumentNotNull(item, "Item");
			this.innerList.Add(item);
			return Count - 1;
		}
		#endregion
		#region AddCore
		public void AddCore(List<CommentRun> runs) {
			this.innerList.AddRange(runs);
		}
		#endregion
		#region GetEnumerator
		public IEnumerator<CommentRun> GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		#region Clear
		public void Clear() {
			DocumentHistory history = Workbook.History;
			CommentRunClearHistoryItem historyItem = new CommentRunClearHistoryItem(this);
			history.Add(historyItem);
			historyItem.Execute();
		}
		#endregion
		#region ClearCore
		public void ClearCore() {
			InnerList.Clear();
		}
		#endregion
		#region InsertCore
		public void InsertCore(int index, CommentRun item) {
			innerList.Insert(index, item);
		}
		#endregion
		#region Insert
		public void Insert(int index, CommentRun item) {
			DocumentHistory history = Workbook.History;
			CommentRunInsertHistoryItem historyItem = new CommentRunInsertHistoryItem(this, item, index);
			history.Add(historyItem);
			historyItem.Execute();
		}
		#endregion
		public void CopyFrom(CommentRunCollection source) {
			Workbook.BeginUpdate();
			try {
				innerList.Clear();
				foreach (CommentRun sourceRun in source.InnerList) {
					CommentRun run = new CommentRun(Worksheet);
					run.CopyFrom(sourceRun);
					innerList.Add(run);
				}
			}
			finally {
				Workbook.EndUpdate();
			}
		}
	}
	#endregion
}
