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
using DevExpress.Xpf.Editors.Controls;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.DateNavigator;
using DevExpress.Xpf.Editors.DateNavigator.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.DateNavigator.Internal;
using System.Windows;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Editors.DateNavigator {
	public abstract class NavigationStrategyBase : INavigationService {
		protected SelectionManager SelectionManager { get { return Navigator.SelectionManager; } }
		protected DateNavigator Navigator { get; private set; }
		INavigationCallbackService NavigationCallback { get { return Navigator.NavigationCallbackService; } }
		protected IOptionsProviderService OptionsProvider { get { return Navigator.OptionsProviderService; } }
		protected bool ScrollSelection { get { return OptionsProvider != null && OptionsProvider.ScrollSelection && Navigator.SelectedDates != null && Navigator.SelectedDates.Count > 0 && Navigator.CalendarView == DateNavigatorCalendarView.Month && ActiveContent != null; } }
		protected ViewSpecificNavigationLogic ViewSpecific { get; private set; }
		protected IDateNavigatorContent ActiveContent { get { return (IDateNavigatorContent)Navigator.ActiveContent; } }
		protected DateTime? MouseDownDate { get; private set; }
		protected DateNavigatorCalendarButtonKind MouseDownDateButtonKind { get; private set; }
		public NavigationStrategyBase(DateNavigator navigator) {
			Navigator = navigator;
			ViewSpecific = new MonthNavigationLogic();
		}
		#region INavigationService
		public virtual void ToView(DateNavigatorCalendarView navigationState) {
			if (navigationState == DateNavigatorCalendarView.Month)
				ViewSpecific = new MonthNavigationLogic();
			if (navigationState == DateNavigatorCalendarView.Year)
				ViewSpecific = new YearNavigationLogic();
			if (navigationState == DateNavigatorCalendarView.Years)
				ViewSpecific = new YearsNavigationLogic();
			if (navigationState == DateNavigatorCalendarView.YearsRange)
				ViewSpecific = new YearsGroupNavigationLogic();
			Navigator.CalendarView = navigationState;
		}
		public virtual void CheckSelectedDates() {
		}
		public virtual bool MoveLeft() {
			return true;
		}
		public virtual bool MoveRight() {
			return true;
		}
		public virtual bool MoveUp() {
			return true;
		}
		public virtual bool MoveDown() {
			return true;
		}
		public virtual bool Move(DateTime dateTime) {
			SelectionManager.SetFocusedDate(dateTime);
			BringToView();
			return true;
		}
		public virtual bool SelectLeft(bool clearSelection) {
			return true;
		}
		public virtual bool SelectRight(bool clearSelection) {
			return true;
		}
		public virtual bool SelectUp(bool clearSelection) {
			return true;
		}
		public virtual bool SelectDown(bool clearSelection) {
			return true;
		}
		public virtual bool Unselect(DateTime dateTime, bool clearSelection) {
			return true;
		}
		public virtual bool Select(DateTime dateTime, bool clearSelection) {
			return Select(new ObservableCollection<DateTime> { dateTime }, clearSelection);
		}
		public virtual bool Select(IList<DateTime> selectedDates, bool clearSelection) {
			SelectionManager.SetSelection(selectedDates, clearSelection);
			return true;
		}
		public virtual bool ScrollNext() {
			return ScrollTo(ViewSpecific.GetPage(ActiveContent.EndDate, 1), false);
		}
		public virtual bool ScrollPrevious() {
			return ScrollTo(ViewSpecific.GetPage(ActiveContent.StartDate, -1), false);
		}
		public virtual bool ScrollNextPage() {
			if (ScrollSelection) {
				DateTime prevFirstSelectedDate = Navigator.SelectedDates[0];
				NavigationCallback.Scroll(Navigator.SelectedDates[0].AddMonths(1) - Navigator.SelectedDates[0]);
				bool scroll;
				if (ActiveContent.GetCalendar(Navigator.SelectedDates[Navigator.SelectedDates.Count - 1], true) == null) {
					scroll = Navigator.SetActiveContentDateTime(Navigator.SelectedDates[0].AddMonths(ActiveContent.CalendarCount - 1), false);
				} else
					scroll = Navigator.SetActiveContentDateTime(Navigator.SelectedDates[0], false);
				Navigator.FocusedDate = Navigator.FocusedDate.AddDays((Navigator.SelectedDates[0] - prevFirstSelectedDate).Days);
				return scroll;
			} else
				return ScrollTo(ViewSpecific.NextPage(ActiveContent.EndDate, ActiveContent.CalendarCount), false);
		}
		public virtual bool ScrollPreviousPage() {
			if (ScrollSelection) {
				DateTime prevFirstSelectedDate = Navigator.SelectedDates[0];
				NavigationCallback.Scroll(Navigator.SelectedDates[0].AddMonths(-1) - Navigator.SelectedDates[0]);
				bool scroll = Navigator.SetActiveContentDateTime(Navigator.SelectedDates[0], true);
				Navigator.FocusedDate = Navigator.FocusedDate.AddDays((Navigator.SelectedDates[0] - prevFirstSelectedDate).Days);
				return scroll;
			} else
				return ScrollTo(ViewSpecific.PreviousPage(ActiveContent.StartDate, ActiveContent.CalendarCount), false);
		}
		public virtual bool ScrollTo(DateTime dateTime, bool scrollIfValueInactive) {
			bool scroll = Navigator.SetActiveContentDateTime(dateTime, scrollIfValueInactive);
			if (scroll)
				NavigationCallback.Scroll(new TimeSpan());
			return scroll;
		}
		public virtual void ProcessKeyDown(KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				ModifierKeys modifiers = ModifierKeysHelper.GetKeyboardModifiers(e);
				if (ModifierKeysHelper.IsCtrlPressed(modifiers))
					ToView(GetView(Navigator.CalendarView, 1));
				else
					ToView(GetView(Navigator.CalendarView, -1));
			}
		}
		public virtual void ProcessKeyUp(KeyEventArgs e) {
		}
		DateNavigatorCalendarView GetView(DateNavigatorCalendarView view, int offset) {
			int current = (int)view + offset;
			if (current < 0)
				return (DateNavigatorCalendarView)0;
			int maxLength = EnumHelper.GetEnumCount(typeof(DateNavigatorCalendarView)) - 1;
			if (current > maxLength)
				return (DateNavigatorCalendarView)maxLength;
			return (DateNavigatorCalendarView)current;
		}
		public virtual void BringToView() {
			ScrollTo(SelectionManager.FocusedDate, false);
		}
		public virtual void ProcessMouseDown(DateTime buttonDate, DateNavigatorCalendarButtonKind buttonKind) {
			if (Navigator.CaptureMouse()) {
				MouseDownDate = buttonDate;
				MouseDownDateButtonKind = buttonKind;
				if (Navigator.CalendarView != DateNavigatorCalendarView.Month) {
					Navigator.Dispatcher.BeginInvoke(new Action(() => {
					VisualStateManager.GoToState(DateNavigatorHelper.GetCalendarCellButton(Navigator, buttonDate), "Normal", true);
					SelectionManager.SetFocusedDate(buttonDate);
					}), null);
				}
				if (Navigator.CalendarView == DateNavigatorCalendarView.Month && buttonKind == DateNavigatorCalendarButtonKind.Date)
					Navigator.Dispatcher.BeginInvoke(new Action(() => {
						VisualStateManager.GoToState(DateNavigatorHelper.GetCalendarCellButton(Navigator, buttonDate), "Normal", true);
					}), null);
			}
		}
		public void ProcessMouseUp(DateTime? buttonDate, DateNavigatorCalendarButtonKind buttonKind) {
			if (MouseDownDate == null) return;
			try {
				ProcessMouseUpCore(buttonDate);
			} finally {
				MouseDownDate = null;
			}
		}
		protected virtual void ProcessMouseUpCore(DateTime? date) {
			if (date != null) {
				Move((DateTime)date);
				ToView(GetView(Navigator.CalendarView, -1));
			}
		}
		public virtual void ProcessMouseMove(DateTime? buttonDate, DateNavigatorCalendarButtonKind buttonKind) {
			if (Navigator.CalendarView != DateNavigatorCalendarView.Month && MouseDownDate != null) {
				if (buttonDate != null)
					SelectionManager.SetFocusedDate((DateTime)buttonDate);
				else {
					DateNavigatorCalendarCellButton button = DateNavigatorHelper.GetCalendarCellButton(Navigator, ((IDateNavigatorContent)Navigator.ActiveContent).FocusedDate);
					if (button != null)
						VisualStateManager.GoToState(button, "Normal", true);
				}
			}
		}
		protected bool MoveLeftRight(bool isLeft) {
			if (Navigator.FlowDirection == FlowDirection.RightToLeft)
				isLeft = !isLeft;
			return isLeft ? MoveLeft() : MoveRight();
		}
		protected bool SelectLeftRight(bool clearSelection, bool isLeft) {
			if (Navigator.FlowDirection == FlowDirection.RightToLeft)
				isLeft = !isLeft;
			return isLeft ? SelectLeft(clearSelection) : SelectRight(clearSelection);
		}
		#endregion
	}
	public static class SelectedDatesHelper {
		static readonly SkipTimeEqualityComparer ComparerInstance = new SkipTimeEqualityComparer();
		public static bool AreEquals(IList<DateTime> list1, IList<DateTime> list2) {
			if (list1 == null || list2 == null)
				return list1 == list2;
			return list1.SequenceEqual(list2, ComparerInstance);
		}
		public static bool AreEquals(DateTime dt1, DateTime dt2) {
			return ComparerInstance.Equals(dt1, dt2);
		}
		public static bool AreEquals(DateTime dt1, DateTime dt2, DateNavigatorCalendarView state) {
			return ComparerInstance.Equals(dt1, dt2, state);
		}
		public static IList<DateTime> GetSelection(DateTime dt1, DateTime dt2) {
			ObservableCollection<DateTime> result = new ObservableCollection<DateTime>();
			DateTime start = dt1.CompareTo(dt2) >= 0 ? dt2 : dt1;
			DateTime end = dt1.CompareTo(dt2) < 0 ? dt2 : dt1;
			for (int i = 0; i < (end - start).TotalDays + 1; i++) 
				result.Add(start.AddDays(i));
			return result;
		}
		public static IList<DateTime> Remove(IList<DateTime> selectedDates, DateTime date) {
			if (!Contains(selectedDates, date))
				return selectedDates;
			return new ObservableCollection<DateTime>(selectedDates.Except<DateTime>(new ObservableCollection<DateTime> { date }, ComparerInstance));
		}
		public static ObservableCollection<DateTime> Merge(IEnumerable<DateTime> selectedDates, DateTime newDate) {
			return Merge(selectedDates, new List<DateTime> { newDate });
		}
		public static ObservableCollection<DateTime> Merge(IEnumerable<DateTime> selectedDates, IEnumerable<DateTime> newDates) {
			List<DateTime> merged;
			if (selectedDates == null)
				merged = new List<DateTime>();
			else
				merged = new List<DateTime>(selectedDates);
			List<DateTime> union = new List<DateTime>(merged.Union<DateTime>(newDates, ComparerInstance));
			union.Sort();
			return new ObservableCollection<DateTime>(union);
		}
		public static bool Contains(IList<DateTime> selectedDates, DateTime date) {
			return (selectedDates != null) && selectedDates.Contains(date, ComparerInstance);
		}
		class SkipTimeEqualityComparer : IEqualityComparer<DateTime> {
			#region IEqualityComparer<DateTime> Members
			public bool Equals(DateTime x, DateTime y) {
				return x.Year == y.Year && x.Month == y.Month && x.Day == y.Day;
			}
			public bool Equals(DateTime x, DateTime y, DateNavigatorCalendarView state) {
				if (state == DateNavigatorCalendarView.Year)
					return x.Year == y.Year;
				if (state == DateNavigatorCalendarView.Month)
					return x.Year == y.Year && x.Month == y.Month;
				return Equals(x, y);
			}
			public int GetHashCode(DateTime obj) {
				return base.GetHashCode();
			}
			#endregion
		}
	}
}
