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
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.Xpf.Editors.DateNavigator.Controls;
namespace DevExpress.Xpf.Editors.DateNavigator {
	public interface INavigationService {
		void ToView(DateNavigatorCalendarView navigationState);
		void BringToView();
		void CheckSelectedDates();
		void ProcessKeyDown(KeyEventArgs e);
		void ProcessKeyUp(KeyEventArgs e);
		void ProcessMouseDown(DateTime buttonDate, DateNavigatorCalendarButtonKind buttonKind);
		void ProcessMouseUp(DateTime? buttonDate, DateNavigatorCalendarButtonKind buttonKind);
		void ProcessMouseMove(DateTime? buttonDate, DateNavigatorCalendarButtonKind buttonKind);
		bool Move(DateTime date);
		bool MoveLeft();
		bool MoveRight();
		bool MoveUp();
		bool MoveDown();
		bool Unselect(DateTime dateTime, bool clearSelection);
		bool Select(DateTime dateTime, bool clearSelection);
		bool Select(IList<DateTime> range, bool clearSelection);
		bool SelectLeft(bool clearSelection);
		bool SelectRight(bool clearSelection);
		bool SelectUp(bool clearSelection);
		bool SelectDown(bool clearSelection);
		bool ScrollNext();
		bool ScrollPrevious();
		bool ScrollNextPage();
		bool ScrollPreviousPage();
		bool ScrollTo(DateTime dateTime, bool scrollIfValueInactive);
	}
	public interface INavigationCallbackService {
		void Move(DateTime dateTime);
		void Select(IList<DateTime> selectedDates);
		void Scroll(TimeSpan offset);
		void ChangeView(DateNavigatorCalendarView state);
		void VisibleDateRangeChanged(bool isScrolling);
	}
	public interface IValueValidatingService {
		IList<DateTime> Validate(IList<DateTime> selectedDates);
	}
	public interface IValueEditingService {
		DateTime StartDate { get; }
		DateTime EndDate { get; }
		DateTime FocusedDate { get; }
		IList<DateTime> SelectedDates { get; }
		void SetSelectedDates(ObservableCollection<DateTime> selectedDates, bool clearSelection);
		void SetFocusedDate(DateTime date);
	}
	public interface IDateCalculationService {
		bool IsWorkday(DateTime date);
	}
	public interface IOptionsProviderService {
		DayOfWeek FirstDayOfWeek { get; }
		bool HighlightSpecialDates { set; }
		IList<DateTime> ExactWorkdays { get; }
		IList<DateTime> Holidays { get; }
		bool ScrollSelection { get; }
		IList<DayOfWeek> Workdays { get; }
		void Start();
		void Stop();
		event EventHandler OptionsChanged;
	}
}
