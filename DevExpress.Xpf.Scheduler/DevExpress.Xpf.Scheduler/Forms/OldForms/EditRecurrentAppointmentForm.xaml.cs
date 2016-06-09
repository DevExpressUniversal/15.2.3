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
using System.Windows;
using System.Windows.Controls;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
namespace DevExpress.Xpf.Scheduler.UI {
	public partial class EditRecurrentAppointmentForm : UserControl {
		SchedulerControl control;
		Appointment appointment;
		bool readOnly;
		string message;
		RecurrentAppointmentAction action = RecurrentAppointmentAction.Occurrence;
		public EditRecurrentAppointmentForm(SchedulerControl control, Appointment appointment, bool readOnly) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(appointment, "appointment");
			this.control = control;
			this.appointment = appointment;
			this.readOnly = readOnly;
			this.message = String.Format(SchedulerControlLocalizer.GetString(SchedulerControlStringId.Form_EditRecurrentAppointmentFormMessage), appointment.Subject);
			Title = SchedulerControlLocalizer.GetString(SchedulerControlStringId.Caption_OpenRecurrentApt);
			InitializeComponent();
		}
		#region Title
		public String Title {
			get { return (String)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<EditRecurrentAppointmentForm, String>("Title", String.Empty);
		#endregion
		public RecurrentAppointmentAction Action { get { return action; } set { action = value; } }
		public string Message { get { return message; } }
		bool ReadOnly { get { return readOnly; } }
		Appointment Appointment { get { return appointment; } }
		SchedulerControl Control { get { return control; } }
		private void OnCancelButtonClick(object sender, RoutedEventArgs e) {
			CloseForm(false);
		}
		private void OnOKButtonClick(object sender, RoutedEventArgs e) {
			CloseForm(true);
			Appointment actualAppointment = Action == RecurrentAppointmentAction.Series ? Appointment.RecurrencePattern : Appointment;
			if (Action == RecurrentAppointmentAction.Series)
				control.ShowEditAppointmentForm(actualAppointment, ReadOnly, CommandSourceType.Unknown, true);
			else
				control.ShowEditAppointmentForm(actualAppointment, ReadOnly, CommandSourceType.Unknown);
		}
		void CloseForm(bool dialogResult) {
			SchedulerFormBehavior.Close(this, dialogResult);
		}		
	}
}
