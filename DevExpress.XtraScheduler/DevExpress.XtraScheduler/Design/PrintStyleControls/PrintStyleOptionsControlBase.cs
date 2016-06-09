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
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Printing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Design.PrintStyleControls {
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class PrintStyleOptionsControlBase : DevExpress.XtraEditors.XtraUserControl, IBatchUpdateable, IBatchUpdateHandler {
		public static PrintStyleOptionsControlBase CreateOptionsControl(SchedulerPrintStyleKind styleKind) {
			switch(styleKind) {
				case SchedulerPrintStyleKind.Daily :
					return new DailyPrintStyleOptionsControl();
				case SchedulerPrintStyleKind.Weekly:
					return new WeeklyPrintStyleOptionsControl();
				case SchedulerPrintStyleKind.Monthly:
					return new MonthlyPrintStyleOptionsControl();
				case SchedulerPrintStyleKind.TriFold:
					return new TriFoldPrintStyleOptionsControl();
				case SchedulerPrintStyleKind.CalendarDetails:
					return new CalendarDetailsPrintStyleOptionsControl();
				case SchedulerPrintStyleKind.Memo:
					return new MemoPrintStyleOptionsControl();
				default:
					XtraSchedulerDebug.Assert(false);
					return null;
			}	
		}
		#region Field
		IContainer components = null;
		SchedulerPrintStyle printStyle;
		public event EventHandler PrintStyleChanged;
		BatchUpdateHelper batchUpdateHelper;
		bool defferedPrintStyleChanged;
		#endregion
		public PrintStyleOptionsControlBase() 
		{
			batchUpdateHelper = new BatchUpdateHelper(this);
			InitializeComponent();
			printStyle = CreateDefaultPrintStyle();
		}
		#region Properties
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerPrintStyle PrintStyle {
			get { return printStyle; }
			set {
				if(value == null)
					Exceptions.ThrowArgumentNullException("PrintStyle");
				if(!IsValidPrintStyle(value))
					Exceptions.ThrowArgumentException("PrintStyle.Kind", value.Kind);
				printStyle = value;
				RefreshData();
			}
		}
		#endregion
		protected internal virtual SchedulerPrintStyle CreateDefaultPrintStyle() {
			return null;
		}
		protected internal virtual void SetPosition(PrintStyleOptionsControlBase pattern) {
			XtraSchedulerDebug.Assert(pattern != null);
			this.Dock = pattern.Dock;;
			this.Location = pattern.Location;
			this.Size = pattern.Size;
			this.TabIndex = pattern.TabIndex;
		}
		protected internal virtual bool IsValidPrintStyle(SchedulerPrintStyle style) {
			XtraSchedulerDebug.Assert(false);
			return false;
		}
		protected internal virtual void RefreshData() {
			if(PrintStyle == null)
				return;
			UnsubscribeEvents();
			RefreshDataCore(PrintStyle);
			SubscribeEvents();
		}
		protected internal virtual void UnsubscribeEvents() {
		}
		protected internal virtual void RefreshDataCore(SchedulerPrintStyle style) {
			if(style == null)
				Exceptions.ThrowArgumentNullException("style");
		}
		protected internal virtual void SubscribeEvents() {
		}
		#region Dispose
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
					components = null;
				}
			}
			base.Dispose(disposing);
		}
		#endregion
		#region Designer generated code
		private void InitializeComponent() {
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PrintStyleOptionsControlBase));
			this.AccessibleDescription = ((string)(resources.GetObject("$this.AccessibleDescription")));
			this.AccessibleName = ((string)(resources.GetObject("$this.AccessibleName")));
			this.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("$this.Anchor")));
			this.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.Appearance.Options.UseBackColor = true;
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("$this.Dock")));
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.Name = "PrintStyleOptionsControlBase";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.Size = ((System.Drawing.Size)(resources.GetObject("$this.Size")));
			this.TabIndex = ((int)(resources.GetObject("$this.TabIndex")));
			this.Visible = ((bool)(resources.GetObject("$this.Visible")));
		}
		#endregion
		protected internal virtual void RaisePrintStyleChangedEvent() {
			if(PrintStyleChanged != null)
				PrintStyleChanged(this, new EventArgs());
		}
		protected internal virtual void OnPrintStyleChanged() {
			RefreshData();
			if(IsUpdateLocked)
				defferedPrintStyleChanged = true;
			else
				RaisePrintStyleChangedEvent();
		}
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
			OnBeginUpdateCore();
			defferedPrintStyleChanged = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnEndUpdateCore();
			if (defferedPrintStyleChanged)
				RaisePrintStyleChangedEvent();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
		protected internal virtual void OnBeginUpdateCore() {
		}
		protected internal virtual void OnEndUpdateCore() { 
		}
	}
}
