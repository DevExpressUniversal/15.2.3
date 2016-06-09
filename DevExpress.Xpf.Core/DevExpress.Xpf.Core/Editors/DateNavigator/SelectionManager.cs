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
using System.Collections.ObjectModel;
using System.Linq;
namespace DevExpress.Xpf.Editors.DateNavigator {
	public class SelectionManager {
		DateTime selectionStart;
		IList<DateTime> currentSelection;
		DateTime focusedDate;
		IList<DateTime> initialSelection;
		public DateTime SelectionStart { get { return IsInContinuousSelection ? selectionStart : FocusedDate; } }
		public DateTime SelectionEnd { get { return SelectedDates.Count > 0 ? SelectedDates.Last<DateTime>() : SelectionStart; } }
		public IList<DateTime> SelectedDates { get { return IsInContinuousSelection ? currentSelection : Navigator.SelectedDates; } }
		public DateTime FocusedDate { get { return IsInContinuousSelection ? focusedDate : Navigator.FocusedDate; } }
		DateNavigator Navigator { get; set; }
		public bool IsInContinuousSelection { get; private set; }
		IValueValidatingService ValueValidating { get { return Navigator.ValueValidatingService; } }
		public SelectionManager(DateNavigator navigator) {
			Navigator = navigator;
			Clear();
		}
		public virtual void Snapshot() {
			if (IsInContinuousSelection)
				return;
			IsInContinuousSelection = true;
			selectionStart = Navigator.FocusedDate;
			Clear();
		}
		public virtual void Flush() {
			if (!IsInContinuousSelection)
				return;
			selectionStart = FocusedDate;
			initialSelection = currentSelection;
			InvalidateSelection();
		}
		public virtual void Post() {
			Navigator.FocusedDate = FocusedDate;
			if (!SelectedDatesHelper.AreEquals(SelectedDates, Navigator.SelectedDates))
				Navigator.SetSelectedDates(new ObservableCollection<DateTime>(Navigator.ValueValidatingService.Validate(SelectedDates)));
			Reset();
			InvalidateSelection();
		}
		IList<DateTime> Select(IList<DateTime> selection, bool clearSelection) {
			IList<DateTime> result = clearSelection ? selection : SelectedDatesHelper.Merge(initialSelection, selection);
			return ValueValidating.Validate(result);
		}
		public void SetFocusedDate(DateTime date) {
			if (IsInContinuousSelection)
				focusedDate = date;
			else 
				Navigator.FocusedDate = date;
			InvalidateFocused();
		}
		public void SetSelection(IList<DateTime> selection, bool clearSelection) {
			if (IsInContinuousSelection) {
				currentSelection = new ObservableCollection<DateTime>(Select(selection, clearSelection));
				if (clearSelection)
					initialSelection = currentSelection;
			} else
				Navigator.SetSelectedDates(new ObservableCollection<DateTime>(Select(selection, clearSelection)));
			InvalidateSelection();
		}
		protected virtual void Clear() {
			currentSelection = Navigator.SelectedDates;
			focusedDate = Navigator.FocusedDate;
			initialSelection = Navigator.SelectedDates;
		}
		protected virtual void InvalidateFocused() {
			Navigator.InvalidateFocusedDate();
		}
		protected virtual void InvalidateSelection() {
			Navigator.InvalidateSelection();
		}
		protected virtual void Reset() {
			IsInContinuousSelection = false;
			selectionStart = Navigator.FocusedDate;
			Clear();
		}
	}
}
