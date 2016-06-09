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
using DevExpress.Xpf.WindowsUI.Base;
namespace DevExpress.Xpf.WindowsUI.Navigation {
	public class JournalEntry {
		public JournalEntry(object source, string name, object parameter = null) {
			Source = source;
			Name = name;
			NavigationParameter = parameter;
		}
		public JournalEntry(object source, object parameter = null)
			: this(source, string.Empty, parameter) {
		}
		public JournalEntry() {
		}
		public string Name { get; private set; }
		public object Source { get; private set; }
		object content;
		public object Content {
			get { return content; }
			private set {
				content = value;
				if(ContentChanged != null)
					ContentChanged(this, EventArgs.Empty);
			}
		}
		public event EventHandler ContentChanged;
		public object NavigationParameter { get; private set; }
		public bool KeepAlive { get; set; }
		internal void ClearContent() {
			if(this.Content != null && !this.KeepAlive) this.Content = null;
		}
		internal protected virtual void SetContent(object content) {
			this.Content = content;
		}
	}
	internal class JournalEntryStackCountChangedEventArgs : EventArgs {
		public JournalEntryStackCountChangedEventArgs(int oldValue) {
			this.oldValue = oldValue;
		}
		readonly int oldValue;
		public int OldValue { get { return oldValue; } }
	}
	internal class JournalEntryStack : IEnumerable<JournalEntry>, ICollection {
		readonly Stack<JournalEntry> innerStack = new Stack<JournalEntry>();
		public event EventHandler<JournalEntryStackCountChangedEventArgs> CountChanged;
		public int Count { get { return innerStack.Count; } }
		public void Clear() { innerStack.Clear(); }
		public bool Contains(JournalEntry item) { return innerStack.Contains(item); }
		public void CopyTo(JournalEntry[] array, int arrayIndex) { innerStack.CopyTo(array, arrayIndex); }
		public IEnumerator<JournalEntry> GetEnumerator() { return innerStack.GetEnumerator(); }
		public JournalEntry Peek() { return innerStack.Peek(); }
		public JournalEntry Pop() {
			var entry = innerStack.Pop();
			if(CountChanged != null)
				CountChanged(this, new JournalEntryStackCountChangedEventArgs(Count + 1));
			return entry;
		}
		public void Push(JournalEntry item) {
			innerStack.Push(item);
			if(CountChanged != null)
				CountChanged(this, new JournalEntryStackCountChangedEventArgs(Count - 1));
		}
		public JournalEntry[] ToArray() { return innerStack.ToArray(); }
		public void TrimExcess() { innerStack.TrimExcess(); }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot {
			get {
				ICollection innerCollection = innerStack;
				return innerCollection.SyncRoot;
			}
		}
		void ICollection.CopyTo(Array array, int index) {
			CopyTo((JournalEntry[])array, index);
		}
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
	}
}
