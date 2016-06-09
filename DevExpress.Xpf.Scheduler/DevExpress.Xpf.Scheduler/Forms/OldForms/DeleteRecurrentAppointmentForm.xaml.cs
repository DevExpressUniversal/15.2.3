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
using System.Windows;
using System.Windows.Controls;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Core;
using System.Windows.Markup;
using System.Windows.Data;
using DevExpress.Xpf.Scheduler;
namespace DevExpress.Xpf.Scheduler.UI {
	public partial class DeleteRecurrentAppointmentForm : UserControl {
		readonly SchedulerStorage storage;
		readonly List<Appointment> appointments;
		public DeleteRecurrentAppointmentForm(SchedulerStorage storage, AppointmentBaseCollection appointments) {
			this.storage = storage;
			this.appointments = new List<Appointment>();
			this.appointments.AddRange(appointments);
			Title = SchedulerControlLocalizer.GetString(SchedulerControlStringId.Caption_DeleteRecurrentApt);
			InitializeComponent();
			FetchAppointmentForProcessing();
		}
		protected SchedulerStorage Storage { get { return storage; } }
		#region Title
		public String Title {
			get { return (String)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DeleteRecurrentAppointmentForm, String>("Title", String.Empty);
		#endregion
		#region Appointments
		public List<Appointment> Appointments { get { return appointments; } }
		#endregion
		#region Message
		public string Message {
			get { return (string)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty = CreateMessageProperty();
		static DependencyProperty CreateMessageProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DeleteRecurrentAppointmentForm, string>("Message", String.Empty);
		}
		#endregion
		#region CurrentAppointment
		public Appointment CurrentAppointment {
			get { return (Appointment)GetValue(CurrentAppointmentProperty); }
			set { SetValue(CurrentAppointmentProperty, value); }
		}
		public static readonly DependencyProperty CurrentAppointmentProperty = CreateCurrentAppointmentProperty();
		static DependencyProperty CreateCurrentAppointmentProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DeleteRecurrentAppointmentForm, Appointment>("CurrentAppointment", null, (d, e) => d.OnCurrentAppointmentChanged(e.OldValue, e.NewValue), null);
		}
		void OnCurrentAppointmentChanged(Appointment oldValue, Appointment newValue) {
			if (newValue != null)
				Message = String.Format(SchedulerControlLocalizer.GetString(SchedulerControlStringId.Form_DeleteRecurrentAppointmentFormMessage), newValue.Subject);
		}
		#endregion
		#region Action
		public RecurrentAppointmentAction Action {
			get { return (RecurrentAppointmentAction)GetValue(ActionProperty); }
			set { SetValue(ActionProperty, value); }
		}
		public static readonly DependencyProperty ActionProperty = CreateActionProperty();
		static DependencyProperty CreateActionProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DeleteRecurrentAppointmentForm, RecurrentAppointmentAction>("Action", RecurrentAppointmentAction.Occurrence);
		}
		#endregion
		bool FetchAppointmentForProcessing() {
			while (Appointments.Count > 0) {
				Appointment appointment = Appointments[0];
				Appointments.Remove(appointment);
				if (appointment.RecurrencePattern != null) {
					CurrentAppointment = appointment;
					return true;
				}
			}
			return false;
		}
		void OnCancelButtonClick(object sender, RoutedEventArgs e) {
			if(!FetchAppointmentForProcessing())
				CloseForm(false);
		}
		void OnOKButtonClick(object sender, RoutedEventArgs e) {
			if (!Storage.IsUpdateLocked)
				Storage.BeginUpdate();
			DeleteAppointments();
			if (!FetchAppointmentForProcessing()) {
				if (Storage.IsUpdateLocked)
					Storage.EndUpdate();
				CloseForm(false);
			}
		}
		private void DeleteAppointments() {
			if (Action == RecurrentAppointmentAction.Occurrence)
				CurrentAppointment.Delete();
			else if (Action == RecurrentAppointmentAction.Series) {
				Appointment pattern = CurrentAppointment.RecurrencePattern;
				pattern.Delete();
				RemoveAppointmentsRelatedToPattern(pattern);
			}
		}
		protected internal virtual void RemoveAppointmentsRelatedToPattern(Appointment pattern) {
			int count = Appointments.Count;
			for(int i = count - 1; i >= 0; i--) {
				Appointment apt = Appointments[i];
				if(apt.RecurrencePattern == pattern)
					appointments.RemoveAt(i);
			}
		}
		void CloseForm(bool dialogResult) {
			SchedulerFormBehavior.Close(this, dialogResult);
		}		
	}
}
