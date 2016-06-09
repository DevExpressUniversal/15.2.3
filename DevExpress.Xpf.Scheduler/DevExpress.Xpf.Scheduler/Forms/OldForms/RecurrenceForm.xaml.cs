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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using System.Reflection;
namespace DevExpress.Xpf.Scheduler.UI {
	public partial class RecurrenceForm : UserControl {
		readonly StandaloneRecurrenceVisualController recurrenceVisualController;
		bool readOnly;
		readonly SchedulerStorage storage;
		readonly SchedulerControl control;
		public RecurrenceForm(SchedulerControl control, AppointmentFormController controller, bool readOnly) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(control.Storage, "control.Storage");
			this.recurrenceVisualController = new StandaloneRecurrenceVisualController(controller);
			this.control = control;
			this.storage = control.Storage;
			this.readOnly = readOnly;
			InitializeComponent();
			Title = SchedulerControlLocalizer.GetString(SchedulerControlStringId.Form_Recurrence);
			FirstDayOfWeek = GetActualFirtDayOfWeek(this.control);
		}
		#region Properties
		public StandaloneRecurrenceVisualController RecurrenceVisualController { get { return recurrenceVisualController; } }
		#region Title
		public String Title {
			get { return (String)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RecurrenceForm, String>("Title", String.Empty);
		#endregion
		public TimeZoneHelper TimeZoneHelper { get { return RecurrenceVisualController.Controller.TimeZoneHelper; } }
		public bool ReadOnly { get { return readOnly; } }
		#region FirstDayOfWeek
		public DayOfWeek FirstDayOfWeek {
			get { return (DayOfWeek)GetValue(FirstDayOfWeekProperty); }
			set { SetValue(FirstDayOfWeekProperty, value); }
		}
		public static readonly DependencyProperty FirstDayOfWeekProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RecurrenceForm, DayOfWeek>("FirstDayOfWeek", DayOfWeek.Saturday, null);
		#endregion
		#endregion        
		void CloseForm(bool dialogResult) {
			SchedulerFormBehavior.Close(this, dialogResult);
		}
		void OnOKButtonClick(object sender, RoutedEventArgs e) {
			if (RecurrenceVisualController.Controller.AreExceptionsPresent()) {
				MessageBoxResult result = MessageBox.Show(SchedulerLocalizer.GetString(SchedulerStringId.Msg_RecurrenceExceptionsWillBeLost), Assembly.GetEntryAssembly().GetName().Name, MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
				if (result == MessageBoxResult.No)
					return;
			}
			RecurrenceVisualController.ApplyRecurrence();
			CloseForm(true);
		}
		void OnCancelButtonClick(object sender, RoutedEventArgs e) {
			CloseForm(false);
		}
		void OnDeleteButtonClick(object sender, RoutedEventArgs e) {
			RecurrenceVisualController.Controller.RemoveRecurrence();
			CloseForm(false);
		}
		DayOfWeek GetActualFirtDayOfWeek(SchedulerControl schedulerControl) {
			if (schedulerControl.OptionsView.FirstDayOfWeek == XtraScheduler.FirstDayOfWeek.System)
				return DevExpress.XtraScheduler.Native.DateTimeHelper.FirstDayOfWeek;
			return DevExpress.XtraScheduler.Native.DateTimeHelper.ConvertFirstDayOfWeek(schedulerControl.OptionsView.FirstDayOfWeek);
		}
	}
}
