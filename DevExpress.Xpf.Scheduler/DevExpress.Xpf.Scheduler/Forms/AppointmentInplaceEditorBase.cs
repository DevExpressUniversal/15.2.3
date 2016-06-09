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
using System.Windows.Input;
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows.Controls;
#if SL
using DevExpress.Xpf.Scheduler.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Scheduler.UI {
	public class AppointmentInplaceEditorBase : UserControl, ISchedulerInplaceEditorEx {
		readonly SchedulerControl control;
		readonly Appointment appointment;
		public AppointmentInplaceEditorBase(SchedulerControl control, Appointment apt) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(apt, "apt");
			this.control = control;
			this.appointment = apt;
			InitializeProperties(apt);
#if !SL
			Focusable = true;
#endif
			Dispatcher.BeginInvoke(new Action(() => this.Focus()));
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AppointmentInplaceEditorBase() {
		}
		#region Properties
		#region Subject
		public static readonly DependencyProperty SubjectProperty = CreateSubjectProperty();
		static DependencyProperty CreateSubjectProperty() {
			return DependencyPropertyManager.Register("Subject", typeof(string), typeof(AppointmentInplaceEditorBase), new PropertyMetadata(null));
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentInplaceEditorBaseSubject")]
#endif
		public string Subject {
			get { return (string)GetValue(SubjectProperty); }
			set { SetValue(SubjectProperty, value); }
		}
		#endregion
		#region Description
		public static readonly DependencyProperty DescriptionProperty = CreateDescriptionProperty();
		static DependencyProperty CreateDescriptionProperty() {
			return DependencyPropertyManager.Register("Description", typeof(string), typeof(AppointmentInplaceEditorBase), new PropertyMetadata(null));
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentInplaceEditorBaseDescription")]
#endif
		public string Description {
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		#endregion
		#region Location
		public static readonly DependencyProperty LocationProperty = CreateLocationProperty();
		static DependencyProperty CreateLocationProperty() {
			return DependencyPropertyManager.Register("Location", typeof(string), typeof(AppointmentInplaceEditorBase), new PropertyMetadata(null));
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentInplaceEditorBaseLocation")]
#endif
		public string Location {
			get { return (string)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		#endregion
		#region Label
		public static readonly DependencyProperty LabelProperty = CreateLabelProperty();
		static DependencyProperty CreateLabelProperty() {
			return DependencyPropertyManager.Register("Label", typeof(IAppointmentLabel), typeof(AppointmentInplaceEditorBase), new PropertyMetadata(null));
		}
		public IAppointmentLabel Label {
			get { return (AppointmentLabel)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}
		#endregion
		#region Status
		public static readonly DependencyProperty StatusProperty = CreateStatusProperty();
		static DependencyProperty CreateStatusProperty() {
			return DependencyPropertyManager.Register("Status", typeof(IAppointmentStatus), typeof(AppointmentInplaceEditorBase), new PropertyMetadata(null));
		}
		public IAppointmentStatus Status {
			get { return (IAppointmentStatus)GetValue(StatusProperty); }
			set { SetValue(StatusProperty, value); }
		}
		#endregion
		#region Storage
		static readonly DependencyPropertyKey StoragePropertyKey = CreateStorageKey();
		static DependencyPropertyKey CreateStorageKey() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<AppointmentInplaceEditorBase, SchedulerStorage>("Storage", null);
		}
		public static readonly DependencyProperty StorageProperty = StoragePropertyKey.DependencyProperty;
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentInplaceEditorBaseStorage")]
#endif
		public SchedulerStorage Storage {
			get { return (SchedulerStorage)GetValue(StorageProperty); }
			private set { this.SetValue(StoragePropertyKey, value); }
		}
		#endregion
		public Appointment Appointment { get { return appointment; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentInplaceEditorBaseIsNewAppointment")]
#endif
		public bool IsNewAppointment { get { return Control.Storage.AppointmentStorage.IsNewAppointment(Appointment); } }
		protected internal SchedulerControl Control { get { return control; } }
		#endregion
		#region Events
		#region CommitChanges
		public event EventHandler CommitChanges;
		protected internal virtual void RaiseCommitChanges() {
			if (CommitChanges != null)
				CommitChanges(this, EventArgs.Empty);
		}
		#endregion
		#region RollbackChanges
		public event EventHandler RollbackChanges;
		protected internal virtual void RaiseRollbackChanges() {
			if (RollbackChanges != null)
				RollbackChanges(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected virtual void InitializeProperties(Appointment apt) {
			Subject = apt.Subject;
			Description = apt.Description;
			Location = apt.Location;
			Storage = Control.Storage;
			Label = Storage.GetLabel(apt.LabelKey);
			Status = Storage.GetStatus(apt.StatusKey);
		}
		protected virtual AppointmentFormController CreateController(SchedulerControl control, Appointment apt) {
			return new AppointmentFormController(control, apt);
		}
#if !SL
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			if (!(bool)e.NewValue)
				OnCommitChanges();
		}
#else
		protected override void OnLostFocus(RoutedEventArgs e) {
			base.OnLostFocus(e);
			if (this.GetIsKeyboardFocusWithin())
				return;
			OnCommitChanges();
		}
#endif
		protected internal virtual void OnRollbackChanges() {
			RaiseRollbackChanges();
		}
		protected internal virtual void OnCommitChanges() {
			RaiseCommitChanges();
		}
#if !SILVERLIGHT
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
#else
		protected virtual void OnPreviewKeyDown(object sender, KeyEventArgs e) {
#endif
			if (e.Key == Key.Enter) {
				OnCommitChanges();
				e.Handled = true;
			}
			if (e.Key == Key.Escape) {
				OnRollbackChanges();
				e.Handled = true;
			}
		}
#if SL
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if (e.Key == Key.Enter) {
				OnCommitChanges();
				Control.Focus();
				e.Handled = true;
			}
		}
#endif
		#region ISchedulerInplaceEditorEx Members
		public virtual void Activate() {
		}
		public virtual void Deactivate() {
		}
		public virtual void ApplyChanges() {
			Appointment.Subject = Subject;
			Appointment.Description = Description;
			Appointment.Location = Location;
			Appointment.LabelKey = Label.Id;
			Appointment.StatusKey = Status.Id;
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
	}
}
