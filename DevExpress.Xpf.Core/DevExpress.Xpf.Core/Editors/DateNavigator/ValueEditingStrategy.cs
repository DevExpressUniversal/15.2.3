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
using System.Text;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Controls;
using System.Windows;
using DevExpress.Xpf.Editors.DateNavigator.Controls;
using DevExpress.Xpf.Editors.DateNavigator.Internal;
namespace DevExpress.Xpf.Editors.DateNavigator {
	public class ValueEditingStrategy : IValueEditingService {
		SelectionManager SelectionManager { get { return Navigator.SelectionManager; } }
		Locker PropertyChangedLocker { get; set; }
		Locker InternalSyncLocker { get; set; }
		protected DateNavigator Navigator { get; private set; }
		protected IDateNavigatorContent ActiveContent { get { return (IDateNavigatorContent)Navigator.ActiveContent; } }
		IValueEditingService Instance { get { return this as IValueEditingService; } }
		IValueValidatingService ValueValidating { get { return Navigator.ValueValidatingService; } }
		public ValueEditingStrategy(DateNavigator navigator) {
			Navigator = navigator;
			PropertyChangedLocker = new Locker();
			InternalSyncLocker = new Locker();
		}
		public virtual void DateTimeChanged(DateTime? oldValue, DateTime? newValue) {
			PropertyChangedLocker.DoLockedActionIfNotLocked(() => {
				Instance.SetSelectedDates(newValue != null ? new ObservableCollection<DateTime> { (DateTime)newValue } : null, true);
				if (newValue != null)
					Instance.SetFocusedDate(newValue.Value);
			});
		}
		public virtual void FocusedDateChanged(DateTime oldValue, DateTime newValue) {
		}
		public virtual bool IsWorkday(DateTime date) {
			return true;
		}
		public virtual void SelectedDatesChanged(IList<DateTime> selectedDates) {
			UpdateSelectedState();
		}
		public virtual void UpdateSelectedState() {
			if (ActiveContent == null)
				return;
			ActiveContent.UpdateCalendarsSelectedDates();
		}
		public void ResetFocusedCellButtonFocusedState(DateNavigatorCalendarView view) {
			DateNavigatorCalendarCellButton button = GetCalendarCellButton(view, Instance.FocusedDate);
			if(button != null)
				DateNavigatorCalendar.SetCellFocused(button, false);
		}
		public virtual void ResetFocusedCellButtonMouseOverState() {
			DateNavigatorCalendarCellButton button = GetCalendarCellButton(Instance.FocusedDate);
			if(button != null)
				VisualStateManager.GoToState(button, "Normal", true);
		}
		public virtual DateNavigatorCalendarCellButton GetCalendarCellButton(DateTime dt) {
			return DateNavigatorHelper.GetCalendarCellButton(Navigator, dt);
		}
		public virtual DateNavigatorCalendarCellButton GetCalendarCellButton(DateNavigatorCalendarView view, DateTime dt) {
			DateNavigatorContent content = ((IDateNavigatorContentContainer)Navigator).GetContent(view);
			if (content == null)
				return null;
			DateNavigatorCalendar calendar = ((IDateNavigatorContent)content).GetCalendar(dt);
			return (calendar != null) ? calendar.GetCellButton(dt) : null;
		}
		public virtual void SetFocusedCellButtonFocusedState() {
			DateNavigatorCalendarCellButton button = GetCalendarCellButton(Instance.FocusedDate);
			if(button != null)
				DateNavigatorCalendar.SetCellFocused(button, true);
		}
		public virtual void SetFocusedCellButtonMouseOverState() {
			DateNavigatorCalendarCellButton button = GetCalendarCellButton(Instance.FocusedDate);
			if(button != null)
				VisualStateManager.GoToState(button, "MouseOver", true);
		}
		public virtual void HolidaysChanged() {
			if (ActiveContent != null)
				ActiveContent.UpdateCalendarsHolidays();
		}
		public virtual void UpdateSelection() {
		}
		#region IValueEditingService Members
		DateTime IValueEditingService.StartDate { get { return ActiveContent != null ? ActiveContent.StartDate : Instance.FocusedDate; } }
		DateTime IValueEditingService.EndDate { get { return ActiveContent != null ? ActiveContent.EndDate : Instance.FocusedDate; } }
		DateTime IValueEditingService.FocusedDate { get { return SelectionManager.FocusedDate; } }
		IList<DateTime> IValueEditingService.SelectedDates { get { return SelectionManager.SelectedDates; } }
		void IValueEditingService.SetSelectedDates(ObservableCollection<DateTime> selectedDates, bool clearSelection) {
			ObservableCollection<DateTime> result = clearSelection ? selectedDates : SelectedDatesHelper.Merge(Navigator.SelectedDates, selectedDates);
			if (SelectedDatesHelper.AreEquals(Navigator.SelectedDates, result))
				return;
			Navigator.SetSelectedDates(result);
		}
		void IValueEditingService.SetFocusedDate(DateTime date) {
			if (SelectedDatesHelper.AreEquals(Navigator.FocusedDate, date))
				return;
			Navigator.FocusedDate = date;
		}
		#endregion
	}
}
