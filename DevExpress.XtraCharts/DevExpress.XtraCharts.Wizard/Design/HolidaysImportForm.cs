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
using System.Windows.Forms;
using DevExpress.Schedule;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Design {
	public partial class HolidaysImportForm : XtraForm {
		readonly string allHolidaysString;
		readonly SortedList<string, List<KnownDate>> holidaysList = new SortedList<string,List<KnownDate>>();
		KnownDate[] importedHolidays;
		bool locked;
		public KnownDate[] ImportedHolidays { get { return importedHolidays; } }
		public HolidaysImportForm(HolidayBaseCollection holidays) {
			InitializeComponent();
			foreach (Holiday holiday in holidays) {
				string location = holiday.Location;
				List<KnownDate> list;
				if (!holidaysList.TryGetValue(location, out list)) {
					list = new List<KnownDate>();
					holidaysList.Add(location, list);
				}
				list.Add(new KnownDate(holiday));
			}
			allHolidaysString = ChartLocalizer.GetString(ChartStringId.AllHolidays);
			clbLocations.Items.Add(allHolidaysString, true);
			foreach (string location in holidaysList.Keys)
				clbLocations.Items.Add(location, true);
			UpdateHolidaysList();
		}
		void UpdateHolidaysList() {
			List<KnownDate> list = new List<KnownDate>();
			foreach (DevExpress.XtraEditors.Controls.CheckedListBoxItem item in clbLocations.Items)
				if (item.CheckState == CheckState.Checked) {
					string location = item.Value as string;
					if (!String.IsNullOrEmpty(location)) {
						List<KnownDate> holidays;
						if (holidaysList.TryGetValue(location, out holidays))
							list.AddRange(holidays.ToArray());
					}
				}
			importedHolidays = list.ToArray();
			lbHolidays.Items.BeginUpdate();
			try {
				lbHolidays.Items.Clear();
				lbHolidays.Items.AddRange(importedHolidays);
			}
			finally {
				lbHolidays.Items.EndUpdate();
			}
		}
		void clbLocations_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			if (!locked) {
				locked = true;
				try {
					if (e.Index == 0)
						if (e.State == CheckState.Checked)
							clbLocations.CheckAll();
						else
							clbLocations.UnCheckAll();
					else
						clbLocations.Items[0].CheckState = CheckState.Indeterminate;
					UpdateHolidaysList();
				}
				finally {
					locked = false;
				}
			}
		}
	}
}
