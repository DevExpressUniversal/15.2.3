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
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using System.Globalization;
using DevExpress.Xpf.Core;
using System.Windows;
namespace DevExpress.Xpf.Scheduler.UI {
	public partial class TimeRulerForm : UserControl {
		DispatcherTimer timer;
		TimeRuler timeRuler;
		public TimeRulerForm(TimeRuler timeRuler) {
			this.timeRuler = timeRuler;
			InitializeComponent();
			InitializeTimer();
			MainElement.DataContext = this;
			Title = SchedulerControlLocalizer.GetString(SchedulerControlStringId.Caption_TimeRuler);
			UpdateCurrentTime();
		}
		public TimeRuler TimeRuler { get { return timeRuler; } }
		#region Title
		public String Title {
			get { return (String)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimeRulerForm, String>("Title", String.Empty);
		#endregion
		void InitializeTimer() {
			this.timer = new DispatcherTimer();
			this.timer.Interval = TimeSpan.FromMinutes(1);
			this.timer.Tick += new EventHandler(OnTimerTick);
			this.timer.Start();
		}
		void OnTimerTick(object sender, EventArgs e) {
			UpdateCurrentTime();
		}
		void UpdateCurrentTime() {
			if (TimeRuler == null)
				return;
			TimeZoneInfo tz = GetSelectedTimeZone();
			if (tz == null)
				return;
			DateTime now = DateTime.Now;
			if (chkDayLightSaving.IsChecked == null)
				chkDayLightSaving.IsChecked = false;
			DateTime convertedTime = TimeZoneInfo.ConvertTime(now, TimeZoneInfo.Local, tz);
			tbCurrentTime.Text = String.Format("{0} {1}", convertedTime.ToShortDateString(), DateTimeFormatHelper.DateToShortTimeString(convertedTime));
		}
		TimeZoneInfo GetSelectedTimeZone() {
			string tzId = this.cbTimeZoneEdit.TimeZoneId;
			TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
			if (tz == null)
				tz = TimeZoneInfo.FindSystemTimeZoneById(TimeRuler.TimeZoneId);
			return tz;
		}
		void CloseForm(bool dialogResult) {
			SchedulerFormBehavior.Close(this, dialogResult);
		}
		void OnOKButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			if (TimeRuler != null) {				
				timeRuler.Caption = edtCaption.Text;
				timeRuler.AdjustForDaylightSavingTime = (bool)this.chkDayLightSaving.IsChecked;
				timeRuler.TimeZoneId = cbTimeZoneEdit.TimeZoneId;
			}
			CloseForm(true);
		}
		private void OnCancelButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			CloseForm(false);
		}
		private void cbTimeZoneEdit_EditValueChanged(object sender, Editors.EditValueChangedEventArgs e) {
			UpdateCurrentTime();
		}
		private void chkDayLightSaving_Checked(object sender, System.Windows.RoutedEventArgs e) {
			UpdateCurrentTime();
		}
	}
}
