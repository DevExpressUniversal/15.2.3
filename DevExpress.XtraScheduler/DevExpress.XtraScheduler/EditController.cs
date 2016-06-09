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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Services.Internal;
using System.Linq;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler {
	#region ISchedulerInplaceEditor
	public interface ISchedulerInplaceEditor : IDisposable {
		Rectangle Bounds { get; set; }
		Control Parent { get; set; }
		Color BackColor { get; set; }
		Color ForeColor { get; set; }
		Font Font { get; set; }
		string Text { get; set; }
		bool Visible { get; set; }
		bool Focus();
		void SelectAll();
		void SetPositionToTheEndOfText();
		event EventHandler CommitChanges;
		event EventHandler RollbackChanges;
		event EventHandler LostFocus;
	}
	#endregion
	#region SchedulerInplaceEditorEventArgs
	public class SchedulerInplaceEditorEventArgs : EventArgs {
		#region Fields
		Rectangle bounds;
		SchedulerControl control;
		Color backColor;
		Color foreColor;
		Font font;
		AppointmentViewInfo viewInfo;
		bool handled;
		#endregion
		#region Properties
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public SchedulerControl Control { get { return control; } set { control = value; } }
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		public Color ForeColor { get { return foreColor; } set { foreColor = value; } }
		public Font Font { get { return font; } set { font = value; } }
		public AppointmentViewInfo ViewInfo { get { return viewInfo; } set { viewInfo = value; } }
		public bool Handled { get { return handled; } set { handled = value; } }
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region TransparentBackColorReadyTextBox
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class TransparentBackColorReadyTextBox : TextBox {
		public TransparentBackColorReadyTextBox() {
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}
	}
	#endregion
	#region SchedulerInplaceEditorEx
	public class SchedulerInplaceEditorEx : ISchedulerInplaceEditorEx {
		#region Fields
		TextBox textBox;
		Font font;
		Appointment appointment;
		SchedulerControl control;
		#endregion
		public SchedulerInplaceEditorEx(SchedulerInplaceEditorEventArgs inplaceEditorArgs) {
			Guard.ArgumentNotNull(inplaceEditorArgs.Control, "inplaceEditorArgs.Control");
			Guard.ArgumentNotNull(inplaceEditorArgs.ViewInfo, "inplaceEditorArgs.ViewInfo");
			Guard.ArgumentNotNull(inplaceEditorArgs.Font, "inplaceEditorArgs.Font");
			this.textBox = new TransparentBackColorReadyTextBox();
			this.control = inplaceEditorArgs.Control;
			this.appointment = inplaceEditorArgs.ViewInfo.Appointment;
			this.font = (Font)inplaceEditorArgs.Font.Clone();
			CreateTextBox(inplaceEditorArgs);
		}
		#region Properties
		protected internal TextBox TextBox { get { return textBox; } }
		protected Font Font { get { return font; } }
		protected Appointment Appointment { get { return appointment; } }
		protected SchedulerControl Control { get { return control; } }
		#endregion
		#region Events
		public event EventHandler CommitChanges;
		public event EventHandler RollbackChanges;
		#endregion
		protected virtual void CreateTextBox(SchedulerInplaceEditorEventArgs inplaceEditorArgs) {
			TextBox.Visible = false;
			TextBox.BorderStyle = BorderStyle.None;
			TextBox.Multiline = true;
			TextBox.HideSelection = false;
			TextBox.Parent = Control;
			TextBox.BackColor = inplaceEditorArgs.BackColor;
			TextBox.ForeColor = inplaceEditorArgs.ForeColor;
			TextBox.Font = Font;
			TextBox.Bounds = inplaceEditorArgs.Bounds;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SchedulerInplaceEditorEx() {
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (TextBox != null) {
					TextBox.Dispose();
					this.textBox = null;
				}
				if (Font != null) {
					Font.Dispose();
					this.font = null;
				}
				this.appointment = null;
			}
		}
		public virtual void Activate() {
			TextBox.Text = Appointment.Subject;
			SubscribeTextBoxEvents();
			if (Control.DataStorage.Appointments.IsNewAppointment(appointment)) {
				TextBox.SelectionLength = 0;
				TextBox.SelectionStart = TextBox.Text.Length;
			} else
				TextBox.SelectAll();
			TextBox.Visible = true;
			TextBox.Focus();
		}
		public virtual void Deactivate() {
			UnsubscribeTextBoxEvents();
			TextBox.Visible = false;
		}
		public virtual void ApplyChanges() {
			Appointment.Subject = TextBox.Text;
		}
		protected internal virtual void SubscribeTextBoxEvents() {
			TextBox.LostFocus += new EventHandler(TextBox_LostFocus);
			TextBox.KeyDown += new KeyEventHandler(TextBox_KeyDown);
		}
		protected internal virtual void UnsubscribeTextBoxEvents() {
			TextBox.LostFocus -= new EventHandler(TextBox_LostFocus);
			TextBox.KeyDown -= new KeyEventHandler(TextBox_KeyDown);
		}
		protected internal virtual void TextBox_LostFocus(object sender, EventArgs e) {
			OnCommitChanges();
		}
		protected internal virtual void TextBox_KeyDown(object sender, KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Escape:
					OnRollbackChanges();
					e.Handled = true;
					break;
				case Keys.Return:
					OnCommitChanges();
					e.Handled = true;
					break;
			}
		}
		protected internal virtual void OnRollbackChanges() {
			if (RollbackChanges != null)
				RollbackChanges(this, EventArgs.Empty);
		}
		protected internal virtual void OnCommitChanges() {
			RaiseCommitChanges();
		}
		protected internal virtual void RaiseCommitChanges() {
			if (CommitChanges != null)
				CommitChanges(this, EventArgs.Empty);
		}
	}
	#endregion
	#region SchedulerInplaceEditor
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class SchedulerInplaceEditor : TextBox, ISchedulerInplaceEditor {
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
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			switch (e.KeyCode) {
				case Keys.Escape:
					RaiseRollbackChanges();
					break;
				case Keys.Return:
					RaiseCommitChanges();
					break;
			}
		}
		#region ISchedulerInplaceEditor explicit implementation
		Rectangle ISchedulerInplaceEditor.Bounds { get { return base.Bounds; } set { base.Bounds = value; } }
		Control ISchedulerInplaceEditor.Parent { get { return base.Parent; } set { base.Parent = value; } }
		Color ISchedulerInplaceEditor.BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
		Color ISchedulerInplaceEditor.ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
		Font ISchedulerInplaceEditor.Font { get { return base.Font; } set { base.Font = value; } }
		string ISchedulerInplaceEditor.Text { get { return base.Text; } set { base.Text = value; } }
		bool ISchedulerInplaceEditor.Visible { get { return base.Visible; } set { base.Visible = value; } }
		bool ISchedulerInplaceEditor.Focus() { return base.Focus(); }
		void ISchedulerInplaceEditor.SelectAll() { base.SelectAll(); }
		void ISchedulerInplaceEditor.SetPositionToTheEndOfText() {
			SelectionLength = 0;
			SelectionStart = Text.Length;
		}
		#endregion
	}
	#endregion
	#region SchedulerInplaceEditorWrapper
	class SchedulerInplaceEditorWrapper : ISchedulerInplaceEditorEx {
		ISchedulerInplaceEditor editor;
		Appointment appointment;
		bool isDisposed;
		SchedulerControl control;
		public SchedulerInplaceEditorWrapper(ISchedulerInplaceEditor editor, SchedulerInplaceEditorEventArgs e) {
			this.editor = editor;
			this.appointment = e.ViewInfo.Appointment;
			this.control = e.Control;
		}
		protected internal ISchedulerInplaceEditor Editor { get { return editor; } }
		SchedulerControl Control { get { return control; } }
		internal bool IsDisposed { get { return isDisposed; } }
		public event EventHandler CommitChanges;
		public event EventHandler RollbackChanges;
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (editor != null) {
					Editor.Dispose();
					this.editor = null;
				}
				this.control = null;
				this.isDisposed = true;
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SchedulerInplaceEditorWrapper() {
			Dispose(false);
		}
		#endregion
		public void Activate() {
			Editor.LostFocus += new EventHandler(Editor_LostFocus);
			Editor.CommitChanges += new EventHandler(Editor_CommitChanges);
			Editor.RollbackChanges += new EventHandler(Editor_RollbackChanges);
			Editor.Visible = true;
			if (Control.DataStorage.Appointments.IsNewAppointment(appointment))
				Editor.SetPositionToTheEndOfText();
			else
				Editor.SelectAll();
			Editor.Focus();
		}
		public void Deactivate() {
			editor.CommitChanges -= new EventHandler(Editor_CommitChanges);
			editor.RollbackChanges -= new EventHandler(Editor_RollbackChanges);
			editor.LostFocus -= new EventHandler(Editor_LostFocus);
		}
		void Editor_RollbackChanges(object sender, EventArgs e) {
			OnRollbackChanges(e);
		}
		public void ApplyChanges() {
			appointment.Subject = Editor.Text;
		}
		private void OnRollbackChanges(EventArgs e) {
			if (RollbackChanges != null)
				RollbackChanges(this, e);
		}
		void Editor_CommitChanges(object sender, EventArgs e) {
			OnCommitChanges();
		}
		private void OnCommitChanges() {
			if (CommitChanges != null)
				CommitChanges(this, EventArgs.Empty);
		}
		void Editor_LostFocus(object sender, EventArgs e) {
			OnCommitChanges();
		}
	}
	#endregion
	#region SchedulerInplaceEditControllerEx
	public class SchedulerInplaceEditControllerEx : SchedulerInplaceEditController {
		#region Fields
		DayView dayView;
		#endregion
		public SchedulerInplaceEditControllerEx(InnerSchedulerControl control)
			: base(control) {
		}
		#region Properties
		SchedulerControl Control { get { return (SchedulerControl)InnerControl.Owner; } }
		#endregion
		protected internal override void BeforeEditorActivate() {
		}
		protected internal override void AfterEditorDeactivate() {
			Control.Focus();
		}
		void SubscribeControlEvents() {
			Control.VisibleIntervalChanged += OnVisibleIntervalChanged;
			this.dayView = Control.ActiveView as DayView;
			if (dayView != null)
				dayView.TopRowTimeChanged += OnViewTopRowTimeChanged;
			Control.SizeChanged += OnControlSizeChanged;
		}
		void UnsubscribeControlEvents() {
			Control.VisibleIntervalChanged -= OnVisibleIntervalChanged;
			if (dayView != null)
				dayView.TopRowTimeChanged -= OnViewTopRowTimeChanged;
			Control.SizeChanged -= OnControlSizeChanged;
		}
		void OnViewTopRowTimeChanged(object sender, ChangeEventArgs e) {
			OnVisibleIntervalChanged(this, EventArgs.Empty);
		}
		void OnControlSizeChanged(object sender, EventArgs e) {
			ISetSchedulerStateService setStateService = InnerControl.GetService<ISetSchedulerStateService>();
			if (setStateService != null && setStateService.IsInplaceEditorOpened == true)
				OnRollbackChanges();
		}
		void OnVisibleIntervalChanged(object sender, EventArgs e) {
			if (Editor == null || EditedAppointment == null)
				return;
			AppointmentViewInfo vi = FindViewInfo(EditedAppointment);
			if (vi != null) {
				Rectangle bounds = Control.ActiveView.ViewInfo.GetInplaceEditorBounds(vi);
				SchedulerInplaceEditorWrapper wrapper = Editor as SchedulerInplaceEditorWrapper;
				if (wrapper != null)
					wrapper.Editor.Bounds = bounds;
				SchedulerInplaceEditorEx editorEx = Editor as SchedulerInplaceEditorEx;
				if (editorEx != null)
					editorEx.TextBox.Bounds = bounds;
			} else {
				try {
					DoCommit();
				} catch {
					DoRollback();
				}
			}
		}
		AppointmentViewInfo FindViewInfo(Appointment appointment) {
			SchedulerViewInfoBase schedulerViewInfo = Control.ActiveView.ViewInfo;
			if (schedulerViewInfo == null)
				return null;
			Control.ActiveView.ThreadManager.WaitForAllThreads();
			IEnumerable<AppointmentViewInfo> appointmentViewInfoCollection = schedulerViewInfo.GetAllAppointmentViewInfos();
			foreach (AppointmentViewInfo aptViewInfo in appointmentViewInfoCollection) {
				bool isVisible = aptViewInfo.Visibility.Visible;
				if (SchedulerUtils.IsAppointmentsEquals(aptViewInfo.Appointment, EditedAppointment) && isVisible) {
					if (aptViewInfo.Resource.Id != Control.Selection.Resource.Id && aptViewInfo.Resource.Id != EmptyResourceId.Id)
						continue;
					if (aptViewInfo.Bounds.Width == 0 || aptViewInfo.Bounds.Height == 0)
						continue;
					return aptViewInfo;
				}
			}
			return null;
		}
		protected internal virtual void OnSchedulerStorageChanged() {
			Deactivate();
			RaiseRollbackChanges();
		}
		public override void Activate() {
			base.Activate();
			if (Editor != null)
				SubscribeControlEvents();
		}
		protected internal override void Deactivate() {
			UnsubscribeControlEvents();
			base.Deactivate();
		}
		protected internal override void OnCommitChanges() {
			base.OnCommitChanges();
			ResetForceSyncMode();
		}
		protected override void OnRollbackChanges() {
			base.OnRollbackChanges();
			ResetForceSyncMode();
		}
		protected void ResetForceSyncMode() {
			IViewAsyncSupport viewAsyncSupport = Control.ActiveView as IViewAsyncSupport;
			if (viewAsyncSupport == null)
				return;
			Control.ActiveView.ThreadManager.WaitForAllThreads();
			viewAsyncSupport.ResetForceSyncMode();
		}
		protected internal override ISchedulerInplaceEditorEx CreateAppointmentInplaceEditor(Appointment apt) {
			AppointmentViewInfo vi = FindViewInfo(EditedAppointment);
			if (vi == null)
				return null;
			SchedulerInplaceEditorEventArgs e = new SchedulerInplaceEditorEventArgs();
			e.Bounds = Control.ActiveView.ViewInfo.GetInplaceEditorBounds(vi);
			e.BackColor = vi.Appearance.GetBackColor();
			e.Font = vi.Appearance.GetFont();
			e.ForeColor = vi.Appearance.GetForeColor();
			e.Control = Control;
			e.ViewInfo = vi;
			return CreateEditor(vi.Appointment, e);
		}
		protected internal virtual ISchedulerInplaceEditorEx CreateEditor(Appointment apt, SchedulerInplaceEditorEventArgs e) {
			ISchedulerInplaceEditor defaultInplaceEditor = CreateOldDefaultInplaceEditor();
			ISchedulerInplaceEditorEx defaultInplaceEditorEx = new SchedulerInplaceEditorEx(e);
			InplaceEditorEventArgs args = new InplaceEditorEventArgs(apt);
			args.InplaceEditor = defaultInplaceEditor;
			args.InplaceEditorEx = defaultInplaceEditorEx;
			args.SchedulerInplaceEditorEventArgs = e;
			Control.RaiseInplaceEditorShowing(args);
			if (!Object.ReferenceEquals(defaultInplaceEditor, args.InplaceEditor)) {
				defaultInplaceEditor.Dispose();
				return new SchedulerInplaceEditorWrapper(args.InplaceEditor, e);
			}
			defaultInplaceEditor.Dispose();
			if (!Object.ReferenceEquals(defaultInplaceEditorEx, args.InplaceEditorEx)) {
				defaultInplaceEditorEx.Dispose();
			}
			return args.InplaceEditorEx;
		}
		protected internal virtual ISchedulerInplaceEditor CreateOldDefaultInplaceEditor() {
			SchedulerInplaceEditor result = new SchedulerInplaceEditor();
			result.Visible = false;
			result.Parent = Control;
			result.BorderStyle = BorderStyle.None;
			result.Multiline = true;
			result.HideSelection = false;
			return result;
		}
		public void DoCommit() {
			if (Editor == null)
				return;
			Editor.ApplyChanges();
			Deactivate();
			RaiseCommitChanges();
		}
		public void DoRollback() {
			if (Editor == null)
				return;
			Deactivate();
			RaiseRollbackChanges();
		}
	}
	#endregion
}
