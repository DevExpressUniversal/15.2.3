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
using DevExpress.Xpf.Grid.Filtering;
namespace DevExpress.Xpf.Grid {
	public class DateAltColumnFilterInfo : DateCompactColumnFilterInfo {
		readonly DatesCache selectedDatesBeforePopupLastClose = new DatesCache();
		public DateAltColumnFilterInfo(ColumnBase column) : base(column) { }
		protected override FilterDateType[] Filters {
			get {
				return new[] {
					FilterDateType.Beyond,
					FilterDateType.MonthAfter2,
					FilterDateType.MonthAfter1,
					FilterDateType.NextWeek,
					FilterDateType.Today,
					FilterDateType.ThisWeek,
					FilterDateType.ThisMonth,
					FilterDateType.MonthAgo1,
					FilterDateType.MonthAgo2,
					FilterDateType.MonthAgo3,
					FilterDateType.MonthAgo4,
					FilterDateType.MonthAgo5,
					FilterDateType.MonthAgo6,
					FilterDateType.Earlier,
				};
			}
		}
		protected override void ClearPopupData(Editors.PopupBaseEdit popup) {
			SaveSelectedDates();
			base.ClearPopupData(popup);
		}
		void SaveSelectedDates() {
			if(!CalendarFilterExists) return;
			this.selectedDatesBeforePopupLastClose.Store(CalendarFilter.SelectedDates);
		}
		protected override void UpdateFilterBySpecificDateCheckState() {
			if(!CalendarFilterExists) return;
			if(!IsFiltered || !this.selectedDatesBeforePopupLastClose.HasData) {
				CalendarFilter.UncheckFilterBySpecificDate();
				return;
			}
			CalendarFilter.CheckFilterBySpecificDate();
		}
		protected override void UpdateSelectedDates() {
			if(!CalendarFilterExists) return;
			if(!IsFiltered) {
				CalendarFilter.SelectedDates = null;
				return;
			}
			var dates = this.selectedDatesBeforePopupLastClose.Data;
			CalendarFilter.SelectedDates = dates;
		}
		protected override void UpdateFiltersCheckState() {
			if(!CalendarFilterExists || !IsFiltered) return;
			if(this.selectedDatesBeforePopupLastClose.HasData) {
				CalendarFilter.UncheckAllFilters();
				return;
			}
			base.UpdateFiltersCheckState();
		}
		class DatesCache {
			DateTime[] data;
			public bool HasData {
				get { return data != null && data.Length > 0; }
			}
			public IList<DateTime> Data {
				get { return new ReadOnlyCollection<DateTime>(this.data ?? new DateTime[] { }); }
			}
			public void Store(IList<DateTime> dates) {
				if(dates == null || dates.Count == 0) {
					Dirty();
					return;
				}
				this.data = new DateTime[dates.Count];
				dates.CopyTo(this.data, 0);
			}
			public void Dirty() {
				this.data = null;
			}
		}
	}
}
