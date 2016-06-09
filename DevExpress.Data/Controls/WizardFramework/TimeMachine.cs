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
using System.Diagnostics;
namespace DevExpress.Data.WizardFramework {
	public class TimeMachine<T> where T : ICloneable {
		class HistoryItem {
			T currentValue;
			public T CurrentValue {
				get {
					return currentValue;
				}
				set {
					if(value == null)
						throw new ArgumentNullException("value");
					currentValue = value;
				}
			}
			T OriginalValue { get; set; }
			public bool IsDirty { get { return !CurrentValue.Equals(OriginalValue); } }
			public HistoryItem(T value) {
				OriginalValue = value;
				CurrentValue = (T)value.Clone();
			}
			public void Commit() {
				OriginalValue = (T)CurrentValue.Clone();
			}
		}
		readonly List<HistoryItem> historyItems = new List<HistoryItem>();
		int currentIndex;
		bool AtEndOfHistory {
			get {
				return currentIndex == historyItems.Count - 1;
			}
		}
		public T CurrentValue {
			get{
				return historyItems[currentIndex].CurrentValue;
			}
			set {
				historyItems[currentIndex].CurrentValue = value;
			}
		}
		public TimeMachine(T initialValue) {
			if(initialValue == null)
				throw new ArgumentNullException("initialValue");
			historyItems.Add(new HistoryItem(initialValue));
		}
		public void MoveToThePast() {
			if(currentIndex == 0)
				throw new InvalidOperationException();
			currentIndex--;
		}
		public void MoveToTheFuture() {
			Debug.Assert(historyItems.Count > 0);
			CommitCurrentValue();
			if(AtEndOfHistory) {
				HistoryItem historyItem = new HistoryItem(CurrentValue);
				historyItems.Add(historyItem);
			}
			currentIndex++;
		}
		public void MoveToTheEndOfHistory() {
			CommitCurrentValue();
			currentIndex = historyItems.Count - 1;
		}
		void CommitCurrentValue() {
			if(!AtEndOfHistory && historyItems[currentIndex].IsDirty) {
				historyItems.RemoveRange(currentIndex + 1, historyItems.Count - (currentIndex + 1));
			}
			if(historyItems[currentIndex].IsDirty) {
				historyItems[currentIndex].Commit();
			}
		}
	}
}
