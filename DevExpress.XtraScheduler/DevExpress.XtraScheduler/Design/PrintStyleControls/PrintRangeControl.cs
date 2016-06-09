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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler.Native;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintRangeControl.edtEnd")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintRangeControl.edtStart")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintRangeControl.lblEnd")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintRangeControl.lblStart")]
#endregion
namespace DevExpress.XtraScheduler.Design {
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class PrintRangeControl : DevExpress.XtraEditors.XtraUserControl, IBatchUpdateable, IBatchUpdateHandler {
		protected DateEdit edtEnd;
		protected DateEdit edtStart;
		protected DevExpress.XtraEditors.LabelControl lblEnd;
		protected DevExpress.XtraEditors.LabelControl lblStart;
		private System.ComponentModel.IContainer components = null;
		BatchUpdateHelper batchUpdateHelper;
		bool deferredRaiseChanged;
		public PrintRangeControl() {
			InitializeComponent();
			batchUpdateHelper = new BatchUpdateHelper(this);
		}
		public DateTime StartDate {
			get { return edtStart.DateTime; }
		}
		public DateTime EndDate {
			get { return edtEnd.DateTime; }
		}
		#region Dispose
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
					components = null;
				}
			}
			base.Dispose(disposing);
		}
		#endregion
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintRangeControl));
			this.edtEnd = new DevExpress.XtraEditors.DateEdit();
			this.edtStart = new DevExpress.XtraEditors.DateEdit();
			this.lblEnd = new DevExpress.XtraEditors.LabelControl();
			this.lblStart = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.edtEnd.Properties.VistaTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEnd.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStart.Properties.VistaTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStart.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.edtEnd, "edtEnd");
			this.edtEnd.Name = "edtEnd";
			this.edtEnd.Properties.AccessibleName = resources.GetString("edtEnd.Properties.AccessibleName");
			this.edtEnd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtEnd.Properties.Buttons"))))});
			this.edtEnd.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtEnd.EditValueChanged += new System.EventHandler(this.EndEditValueChanged);
			resources.ApplyResources(this.edtStart, "edtStart");
			this.edtStart.Name = "edtStart";
			this.edtStart.Properties.AccessibleName = resources.GetString("edtStart.Properties.AccessibleName");
			this.edtStart.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtStart.Properties.Buttons"))))});
			this.edtStart.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtStart.EditValueChanged += new System.EventHandler(this.StartEditValueChanged);
			resources.ApplyResources(this.lblEnd, "lblEnd");
			this.lblEnd.Name = "lblEnd";
			resources.ApplyResources(this.lblStart, "lblStart");
			this.lblStart.Name = "lblStart";
			this.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.Appearance.Options.UseBackColor = true;
			this.Controls.Add(this.edtEnd);
			this.Controls.Add(this.edtStart);
			this.Controls.Add(this.lblEnd);
			this.Controls.Add(this.lblStart);
			this.Name = "PrintRangeControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.edtEnd.Properties.VistaTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEnd.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStart.Properties.VistaTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStart.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		#region DateRangeChanged
		internal static readonly object onDateRangeChanged = new object();
		public event EventHandler DateRangeChanged {
			add { Events.AddHandler(onDateRangeChanged, value); }
			remove { Events.RemoveHandler(onDateRangeChanged, value); }
		}
		protected internal virtual void RaiseDateRangeChangedEvent() {
			EventHandler handler = (EventHandler)Events[onDateRangeChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		protected internal virtual void OnDateRangeChanged() {
			if (IsUpdateLocked)
				deferredRaiseChanged = true;
			else
				RaiseDateRangeChangedEvent();
		}
		#region Validating code
		protected internal virtual bool IsValidInterval(DateTime start, DateTime end) {
			return start <= end;
		}
		#endregion
		#region SetRange
		public void SetRange(DateTime start, DateTime end) {
			start = start.Date;
			end = end.Date;
			if (!IsValidInterval(start, end))
				Exceptions.ThrowArgumentException("end", end);
			if (StartDate == start && EndDate == end)
				return;
			UnsubscribeEvents();
			edtStart.DateTime = start;
			edtEnd.DateTime = end;
			SubscribeEvents();
			OnDateRangeChanged();
		}
		protected internal virtual void UnsubscribeEvents() {
			edtStart.EditValueChanged -= new EventHandler(StartEditValueChanged);
			edtEnd.EditValueChanged -= new EventHandler(EndEditValueChanged);
		}
		protected internal virtual void SubscribeEvents() {
			edtStart.EditValueChanged += new EventHandler(StartEditValueChanged);
			edtEnd.EditValueChanged += new EventHandler(EndEditValueChanged);
		}
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			deferredRaiseChanged = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (deferredRaiseChanged)
				RaiseDateRangeChangedEvent();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
		protected internal virtual void StartEditValueChanged(object sender, EventArgs e) {
			UnsubscribeEvents();
			if (!IsValidInterval(StartDate, EndDate))
				edtEnd.EditValue = StartDate;
			SubscribeEvents();
			OnDateRangeChanged();
		}
		protected internal virtual void EndEditValueChanged(object sender, EventArgs e) {
			UnsubscribeEvents();
			if (!IsValidInterval(StartDate, EndDate))
				edtStart.EditValue = EndDate;
			SubscribeEvents();
			OnDateRangeChanged();
		}
	}
}
