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
using System.ComponentModel;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.XtraScheduler.Localization;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Commands;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentDependencyForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentDependencyForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentDependencyForm.lblFrom")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentDependencyForm.lblTask1Description")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentDependencyForm.lblTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentDependencyForm.lblTask2Description")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentDependencyForm.lblType")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentDependencyForm.cbTypeEdit")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentDependencyForm.btnDelete")]
#endregion
namespace DevExpress.XtraScheduler.UI {
	[System.Runtime.InteropServices.ComVisible(false)]
	public partial class AppointmentDependencyForm : DevExpress.XtraEditors.XtraForm {
		#region Fields
		bool readOnly;
		readonly AppointmentDependencyFormController controller;
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AppointmentDependencyForm() {
			InitializeComponent();
		}
		public AppointmentDependencyForm(SchedulerControl control, AppointmentDependency dep) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(control.Storage, "control.Storage");
			Guard.ArgumentNotNull(dep, "dep");
			controller = CreateController(control, dep);
			InitializeComponent();
			UpdateForm();
		}
		#region Properties
		protected internal AppointmentDependencyFormController Controller { get { return controller; } }
		#region ReadOnly
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentDependencyFormReadOnly")]
#endif
		public bool ReadOnly {
			get { return readOnly; }
			set {
				if (readOnly == value)
					return;
				readOnly = value;
				UpdateForm();
			}
		}
		#endregion
		#endregion
		protected internal virtual AppointmentDependencyFormController CreateController(SchedulerControl control, AppointmentDependency dependency) {
			return new AppointmentDependencyFormController(control, dependency);
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void SubscribeControlsEvents() {
			cbTypeEdit.EditValueChanged += new EventHandler(cbTypeEdit_SelectedIndexChanged);
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			cbTypeEdit.EditValueChanged -= new EventHandler(cbTypeEdit_SelectedIndexChanged);
		}
		protected internal virtual void UpdateFormCore() {
			MakeControlsReadOnly(false);
			cbTypeEdit.Type = controller.DependencyType;
			lblTask1Description.Text = controller.ParentTaskDescription;
			lblTask2Description.Text = controller.DependentTaskDescription;
			if (readOnly)
				MakeControlsReadOnly(readOnly);
		}
		void MakeControlsReadOnly(bool readOnly) {
			cbTypeEdit.Enabled = !readOnly;
			btnOk.Enabled = !readOnly;
		}
		protected internal virtual void OnOkButton() {
			controller.ApplyChanges();
		}
		void cbTypeEdit_SelectedIndexChanged(object sender, EventArgs e) {
			controller.DependencyType = cbTypeEdit.Type;
		}
		void btnOk_Click(object sender, EventArgs e) {
			OnOkButton();
		}
		private void btnDelete_Click(object sender, EventArgs e) {
			OnDeleteButton();
		}
		protected internal virtual void OnDeleteButton() {
			controller.DeleteDependency();
			DialogResult = DialogResult.Abort;
			Close();
		}
		private void AppointmentDependencyForm_Load(object sender, EventArgs e) {
		}
	}
	#region AppointmentDependencyFormController
	public class AppointmentDependencyFormController : INotifyPropertyChanged {
		InnerSchedulerControl innerControl;
		AppointmentDependency sourceDependency;
		AppointmentDependencyType editedType;
		string parentTaskDescription;
		string dependentTaskDescription;
		public AppointmentDependencyFormController(SchedulerControl control, AppointmentDependency dependency) {
			if (control == null)
				Exceptions.ThrowArgumentException("innerControl", innerControl);
			if (dependency == null)
				Exceptions.ThrowArgumentException("dependency", dependency);
			ISchedulerStorageBase storage = control.Storage;
			if (storage == null)
				Exceptions.ThrowArgumentException("control.Storage", storage);
			this.innerControl = control.InnerControl;
			this.sourceDependency = dependency;
			this.editedType = dependency.Type;
			this.parentTaskDescription = GetTaskDescription(sourceDependency.ParentId, storage);
			this.dependentTaskDescription = GetTaskDescription(sourceDependency.DependentId, storage);
		}
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentDependencyFormControllerDependencyType")]
#endif
		public AppointmentDependencyType DependencyType {
			get { return editedType; }
			set {
				if (DependencyType == value)
					return;
				editedType = value;
				NotifyPropertyChanged("DependencyType");
			}
		}
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentDependencyFormControllerParentTaskDescription")]
#endif
		public string ParentTaskDescription { get { return parentTaskDescription; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentDependencyFormControllerDependentTaskDescription")]
#endif
		public string DependentTaskDescription { get { return dependentTaskDescription; } }
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler propertyChanged;
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentDependencyFormControllerPropertyChanged")]
#endif
		public event PropertyChangedEventHandler PropertyChanged { add { propertyChanged += value; } remove { propertyChanged -= value; } }
		protected void NotifyPropertyChanged(String info) {
			if (propertyChanged != null) {
				propertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
		#endregion
		public virtual void ApplyChanges() {
			sourceDependency.BeginUpdate();
			try {
				sourceDependency.Type = editedType;
			}
			finally {
				sourceDependency.EndUpdate();
			}
		}
		string GetTaskDescription(object id, ISchedulerStorageBase storage) {
			string result = String.Empty;
			Appointment app = storage.Appointments.GetAppointmentById(id);
			if (!String.IsNullOrEmpty(app.Subject))
				result = app.Subject;
			return result;
		}
		protected internal virtual void DeleteDependency() {
			DeleteAppointmentDependenciesCommand command = new DeleteAppointmentDependenciesCommand(innerControl, sourceDependency);
			command.Execute();
		}
	}
	#endregion
}
