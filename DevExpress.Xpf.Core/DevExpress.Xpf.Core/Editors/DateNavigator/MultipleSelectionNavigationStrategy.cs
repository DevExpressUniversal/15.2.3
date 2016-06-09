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
using System.Linq;
using System.Text;
using DevExpress.Xpf.Editors;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Helpers;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.DateNavigator;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.DateNavigator.Controls;
namespace DevExpress.Xpf.Editors.DateNavigator {
	public class MultipleSelectionNavigationStrategy : NavigationStrategyBase {
		public MultipleSelectionNavigationStrategy(DateNavigator dateNavigator)
			: base(dateNavigator) {
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			base.ProcessKeyDown(e);
			if (e.Handled)
				return;
			if (KeyboardHelper.IsShiftKey(e.Key)) {
				SelectionManager.Snapshot();
				return;
			}
			if (KeyboardHelper.IsControlKey(e.Key) && !Keyboard2.IsShiftPressed) {
				SelectionManager.Snapshot();
				return;
			}
			ModifierKeys modifiers = ModifierKeysHelper.GetKeyboardModifiers(e);
			if (ModifierKeysHelper.NoModifiers(modifiers))
				e.Handled = PerformNoModifiersNavigation(e);
			if (ModifierKeysHelper.IsCtrlPressed(modifiers) || ModifierKeysHelper.IsShiftPressed(modifiers))
				e.Handled = PerformModifiersNavigation(e);
		}
		public override void ProcessKeyUp(KeyEventArgs e) {
			base.ProcessKeyUp(e);
			if (e.Handled)
				return;
			if (KeyboardHelper.IsShiftKey(e.Key))
				SelectionManager.Post();
			if (KeyboardHelper.IsControlKey(e.Key) && !Keyboard2.IsShiftPressed)
				SelectionManager.Post();
			if (e.Key == Key.Space)
				e.Handled = PerformSpace(e);
		}
		protected virtual bool PerformSpace(KeyEventArgs e) {
			if (e.Handled)
				return true;
			bool shouldClearSelection = !ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e));
			bool isSelected = SelectedDatesHelper.Contains(SelectionManager.SelectedDates, SelectionManager.FocusedDate);
			if (isSelected) {
				Unselect(SelectionManager.FocusedDate, shouldClearSelection);
				SelectionManager.Flush();
				return true;
			}
			return Select(SelectionManager.FocusedDate, shouldClearSelection);
		}
		protected virtual bool PerformModifiersNavigation(KeyEventArgs e) {
			if (e.Handled)
				return e.Handled;
			ModifierKeys modifiers = ModifierKeysHelper.GetKeyboardModifiers(e);
			bool shouldClearContent = !ModifierKeysHelper.IsShiftPressed(modifiers) && !ModifierKeysHelper.IsCtrlPressed(modifiers);
			bool shouldNavigate = ModifierKeysHelper.IsCtrlPressed(modifiers) && !ModifierKeysHelper.IsShiftPressed(modifiers);
			switch (e.Key) {
				case Key.Left:
					return shouldNavigate ? MoveLeftRight(true) : SelectLeftRight(shouldClearContent, true);
				case Key.Right:
					return shouldNavigate ? MoveLeftRight(false) : SelectLeftRight(shouldClearContent, false);
				case Key.Up:
					return shouldNavigate ? MoveUp() : SelectUp(shouldClearContent);
				case Key.Down:
					return shouldNavigate ? MoveDown() : SelectDown(shouldClearContent);
			}
			return false;
		}
		protected virtual bool PerformNoModifiersNavigation(KeyEventArgs e) {
			if (e.Handled)
				return true;
			bool shouldSelect = false;
			switch (e.Key) {
				case Key.Left:
					if (MoveLeftRight(true))
						shouldSelect = Select(SelectionManager.FocusedDate, true);
					break;
				case Key.Right:
					if (MoveLeftRight(false))
						shouldSelect = Select(SelectionManager.FocusedDate, true);
					break;
				case Key.Up:
					if (MoveUp())
						shouldSelect = Select(SelectionManager.FocusedDate, true);
					break;
				case Key.Down:
					if (MoveDown())
						shouldSelect = Select(SelectionManager.FocusedDate, true);
					break;
			}
			return shouldSelect;
		}
		public override bool MoveLeft() {
			return Move(ViewSpecific.MoveLeft(SelectionManager.FocusedDate));
		}
		public override bool MoveRight() {
			return Move(ViewSpecific.MoveRight(SelectionManager.FocusedDate));
		}
		public override bool MoveDown() {
			return Move(ViewSpecific.MoveDown(SelectionManager.FocusedDate));
		}
		public override bool MoveUp() {
			return Move(ViewSpecific.MoveUp(SelectionManager.FocusedDate));
		}
		public override bool SelectLeft(bool clearSelection) {
			if (!MoveLeft())
				return false;
			bool result = Select(SelectedDatesHelper.GetSelection(SelectionManager.FocusedDate, SelectionManager.SelectionStart), clearSelection);
			base.SelectLeft(clearSelection);
			return result;
		}
		public override bool SelectRight(bool clearSelection) {
			if (!MoveRight())
				return false;
			bool result = Select(SelectedDatesHelper.GetSelection(SelectionManager.FocusedDate, SelectionManager.SelectionStart), clearSelection);
			base.SelectRight(clearSelection);
			return result;
		}
		public override bool SelectDown(bool clearSelection) {
			if (!MoveDown())
				return false;
			bool result = Select(SelectedDatesHelper.GetSelection(SelectionManager.FocusedDate, SelectionManager.SelectionStart), clearSelection);
			base.SelectDown(clearSelection);
			return result;
		}
		public override bool SelectUp(bool clearSelection) {
			if (!MoveUp())
				return false;
			bool result = Select(SelectedDatesHelper.GetSelection(SelectionManager.FocusedDate, SelectionManager.SelectionStart), clearSelection);
			base.SelectUp(clearSelection);
			return result;
		}
		public override bool Unselect(DateTime dateTime, bool clearSelection) {
			IList<DateTime> baseSelection = clearSelection ? new ObservableCollection<DateTime>() : SelectedDatesHelper.Remove(SelectionManager.SelectedDates, dateTime);
			SelectionManager.SetSelection(baseSelection, true);
			base.Unselect(dateTime, clearSelection);
			return true;
		}
		public override void ProcessMouseDown(DateTime buttonDate, DateNavigatorCalendarButtonKind buttonKind) {
			if (buttonKind == DateNavigatorCalendarButtonKind.WeekNumber && Keyboard2.IsControlPressed && !Keyboard2.IsShiftPressed) return;
			base.ProcessMouseDown(buttonDate, buttonKind);
			if (Navigator.CalendarView == DateNavigatorCalendarView.Month)
				MonthCalendarMouseDown(buttonDate, buttonKind);
		}
		protected virtual void MonthCalendarMouseDown(DateTime buttonDate, DateNavigatorCalendarButtonKind buttonKind) {
			bool shouldClearContent = !Keyboard2.IsShiftPressed && !Keyboard2.IsControlPressed;
			bool shouldAddSelection = Keyboard2.IsShiftPressed;
			bool shouldChangeSelection = Keyboard2.IsControlPressed && !Keyboard2.IsShiftPressed;
			if (shouldClearContent) {
				SelectionManager.Snapshot();
				Move(buttonDate);
				SelectionManager.Flush();
				SelectRange(buttonDate, buttonKind == DateNavigatorCalendarButtonKind.WeekNumber ? 7 : 1, true);
			}
			else if (shouldAddSelection) {
				Move(buttonDate);
				SelectionManager.Flush();
				SelectRange(buttonDate, buttonKind == DateNavigatorCalendarButtonKind.WeekNumber ? 7 : 1, false);
			}
			else if (shouldChangeSelection) {
				SelectionManager.Snapshot();
				if (SelectedDatesHelper.Contains(SelectionManager.SelectedDates, buttonDate)) {
					Move(buttonDate);
					Unselect(buttonDate, false);
					SelectionManager.Flush();
				}
				else {
					Move(buttonDate);
					SelectionManager.Flush();
					Select(buttonDate, false);
				}
			}
		}
		public override void ProcessMouseMove(DateTime? buttonDate, DateNavigatorCalendarButtonKind buttonKind) {
			base.ProcessMouseMove(buttonDate, buttonKind);
			if (Navigator.CalendarView != DateNavigatorCalendarView.Month || MouseDownDate == null)
				return;
			bool isInChangeSelection = Keyboard2.IsControlPressed && !Keyboard2.IsShiftPressed;
			if (isInChangeSelection)
				return;
			SelectionManager.Snapshot();
			if (buttonDate != null) {
				Move((DateTime)buttonDate);
				if (MouseDownDateButtonKind == DateNavigatorCalendarButtonKind.Date)
					Select(SelectedDatesHelper.GetSelection(SelectionManager.SelectionStart, (DateTime)buttonDate), false);
				else {
					DateTime buttonDateWeekFirstDate = ActiveContent.GetWeekFirstDateByDate((DateTime)buttonDate);
					DateTime startDate = (buttonDate >= MouseDownDate) ? (DateTime)MouseDownDate : buttonDateWeekFirstDate;
					int dayCount = 7 + ((buttonDate >= MouseDownDate) ? (buttonDateWeekFirstDate - startDate).Days : ((DateTime)MouseDownDate - startDate).Days);
					SelectRange(startDate, dayCount, false);
				}
			}
		}
		protected override void ProcessMouseUpCore(DateTime? date) {
			if (Navigator.CalendarView != DateNavigatorCalendarView.Month) {
				base.ProcessMouseUpCore(date);
				return;
			}
			if (Keyboard2.IsControlPressed)
				return;
			if (!Keyboard2.IsShiftPressed)
				SelectionManager.Post();
			else
				SelectionManager.Flush();
		}
		protected bool SelectRange(DateTime startDate, int dayCount, bool clearSelection) {
			List<DateTime> selectedDates = new List<DateTime>();
			for (int i = 0; i < dayCount; i++)
				selectedDates.Add(startDate.AddDays(i));
			return Select(selectedDates, clearSelection);
		}
	}
}
