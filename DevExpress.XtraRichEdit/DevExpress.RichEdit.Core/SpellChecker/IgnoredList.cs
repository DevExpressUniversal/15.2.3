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
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpellChecker;
using System.Collections;
namespace DevExpress.XtraRichEdit.SpellChecker {
	#region SortedIgnoreList
	public class SortedIgnoreList {
		DocumentLogPosition[] keys;
		string[] values;
		int size = 0;
		public SortedIgnoreList() {
			this.keys = new DocumentLogPosition[] { };
			this.values = new string[] { };
		}
		public int Count { get { return size; } }
		public virtual void Add(DocumentLogPosition pos, string word) {
			int index = BinarySearch(pos);
			if (index >= 0)
				return;
			Insert(~index, pos, word);
		}
		public void Insert(int index, DocumentLogPosition key, string value) {
			if (this.keys.Length == this.size) {
				DocumentLogPosition[] newKeys = new DocumentLogPosition[this.size + 1];
				string[] newValues = new string[this.size + 1];
				Array.Copy(this.keys, 0, newKeys, 0, this.size);
				Array.Copy(this.values, 0, newValues, 0, this.size);
				this.keys = newKeys;
				this.values = newValues;
			}
			if (index < this.size) {
				Array.Copy(this.keys, index, this.keys, index + 1, this.size - index);
				Array.Copy(this.values, index, this.values, index + 1, this.size - index);
			}
			this.keys[index] = key;
			this.values[index] = value;
			this.size++;
		}
		public virtual bool Contains(DocumentLogPosition pos) {
			return BinarySearch(pos) >= 0;
		}
		public virtual bool Contains(string word) {
			for (int i = 0; i < this.size; i++)
				if (this.values[i] == word)
					return true;
			return false;
		}
		protected internal int BinarySearch(DocumentLogPosition pos) {
			return Array.BinarySearch<DocumentLogPosition>(keys, 0, this.size, pos);
		}
		public void RemoveAt(int index) {
			this.size--;
			if (index < this.size) {
				Array.Copy(this.keys, index + 1, this.keys, index, this.size - index);
				Array.Copy(this.values, index + 1, this.values, index, this.size - index);
			}
			this.keys[this.size] = new DocumentLogPosition(-1);
			this.values[this.size] = null;
		}
		public virtual bool Remove(DocumentLogPosition pos) {
			int index = BinarySearch(pos);
			if (index >= 0) {
				this.RemoveAt(index);
				return true;
			}
			return false;
		}
		public virtual void Remove(string word) {
			for (int i = this.size - 1; i >= 0; i--) {
				if (this.values[i] == word)
					this.RemoveAt(i);
			}
		}
		public virtual void Remove(DocumentLogPosition start, DocumentLogPosition end) {
			int startIndex = BinarySearch(start);
			if (startIndex < 0)
				startIndex = ~startIndex;
			int endIndex = BinarySearch(end);
			if (endIndex < 0)
				endIndex = (~endIndex) - 1;
			for (int i = endIndex; i >= startIndex; i--)
				this.RemoveAt(i);
		}
		public virtual void Clear() {
			Array.Clear(this.keys, 0, this.size);
			Array.Clear(this.values, 0, this.size);
			this.size = 0;
		}
		public virtual SortedIgnoreList Clone() {
			SortedIgnoreList clone = new SortedIgnoreList();
			clone.CopyFrom(this);
			return clone;
		}
		public virtual void CopyFrom(SortedIgnoreList ignoreList) {
			Clear();
			int newSize = ignoreList.size;
			DocumentLogPosition[] newKeys = new DocumentLogPosition[newSize];
			Array.Copy(ignoreList.keys, newKeys, newSize);
			this.keys = newKeys;
			string[] newValues = new string[newSize];
			Array.Copy(ignoreList.values, newValues, newSize);
			this.values = newValues;
			this.size = newSize;
		}
	}
	#endregion
	#region IgnoreItem
	public struct IgnoreItem : IIgnoreItem {
		public Position End { get; internal set; }
		public Position Start { get; internal set; }
		public string Word { get; internal set; }
	} 
	#endregion
	#region IgnoreListManager
	public class IgnoreListManager : IIgnoreList {
		readonly DocumentModel documentModel;
		public IgnoreListManager(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		public PieceTable PieceTable { get { return DocumentModel.ActivePieceTable; } }
		public IgnoredList IgnoredList { get { return PieceTable.SpellCheckerManager.IgnoredList; } }
		public MisspelledIntervalCollection MisspelledIntervals { get { return PieceTable.SpellCheckerManager.MisspelledIntervals; } }
		public virtual void Add(Position start, Position end, string word) {
			DocumentPosition startPosition = start as DocumentPosition;
			DocumentPosition endPosition = end as DocumentPosition;
			if (startPosition == null || endPosition == null)
				return;
			AddCore(startPosition, endPosition);
		}
		protected internal virtual void AddCore(DocumentPosition startPosition, DocumentPosition endPosition) {
			MisspelledInterval interval = MisspelledIntervals.FindInerval(startPosition.LogPosition, endPosition.LogPosition);
			if (interval != null) {
				MisspelledIntervals.Remove(interval);
				IgnoredList.Add(interval);
			}
			else
				IgnoredList.Add(startPosition.Position, endPosition.Position);
		}
		public virtual bool Contains(Position start, Position end, string word) {
			DocumentPosition startPosition = start as DocumentPosition;
			DocumentPosition endPosition = end as DocumentPosition;
			if (startPosition == null || endPosition == null)
				return false;
			return IgnoredList.Contains(startPosition.LogPosition, endPosition.LogPosition, word);
		}
		public virtual void Remove(Position start, Position end, string word) {
			DocumentPosition startPosition = start as DocumentPosition;
			DocumentPosition endPosition = end as DocumentPosition;
			if (startPosition == null || endPosition == null)
				return;
			RemoveCore(startPosition, endPosition);
		}
		protected internal virtual void RemoveCore(DocumentPosition startPosition, DocumentPosition endPosition) {
			MisspelledInterval interval = IgnoredList.FindInerval(startPosition.LogPosition, endPosition.LogPosition);
			if (interval != null) {
				IgnoredList.Remove(interval);
				MisspelledIntervals.Add(interval);
			}
		}
		public virtual void Add(string word) {
			if (IgnoredList.Contains(word))
				return;
			IgnoredList.Add(word);
			PieceTable.SpellCheckerManager.InitializeUncheckedInerval();
		}
		public virtual bool Contains(string word) {
			return IgnoredList.Contains(word);
		}
		public virtual void Remove(string word) {
			if (!IgnoredList.Remove(word))
				return;
			PieceTable.SpellCheckerManager.InitializeUncheckedInerval();
		}
		public void Clear() {
			DocumentModel.ActivePieceTable.SpellCheckerManager.IgnoredList.Clear();
		}
		#region IEnumerable<IIgnoreItem> Members
		public IEnumerator<IIgnoreItem> GetEnumerator() {
			for (int i = 0; i < IgnoredList.Count; i++) {
				DocumentModelPosition start = IgnoredList[i].Interval.Start;
				DocumentModelPosition end = IgnoredList[i].Interval.End;
				string word = IgnoredList.PieceTable.GetPlainText(start, end);
				yield return new IgnoreItem() { 
					Start = new DocumentPosition(start),
					End = new DocumentPosition(end),
					Word = word
				};
			}
			for (int i = 0; i < IgnoredList.IgnoreAllList.Count; i++) {
				string word = IgnoredList.IgnoreAllList[i];
				yield return new IgnoreItem() {
					Start = Position.Undefined,
					End = Position.Undefined,
					Word = word
				};
			}
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
	}
	#endregion
}
