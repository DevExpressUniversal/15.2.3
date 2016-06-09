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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Printing;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.MemoPrintStyleOptionsControl.chkBreakPageAfterEachItem")]
#endregion
namespace DevExpress.XtraScheduler.Design.PrintStyleControls {
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class MemoPrintStyleOptionsControl : PrintStyleOptionsControlBase {
		protected DevExpress.XtraEditors.CheckEdit chkBreakPageAfterEachItem;
		System.ComponentModel.IContainer components = null;
		public MemoPrintStyleOptionsControl() {
			InitializeComponent();
		}
		protected internal new MemoPrintStyle PrintStyle { get { return (MemoPrintStyle)base.PrintStyle; } }
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MemoPrintStyleOptionsControl));
			this.chkBreakPageAfterEachItem = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.chkBreakPageAfterEachItem.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chkBreakPageAfterEachItem, "chkBreakPageAfterEachItem");
			this.chkBreakPageAfterEachItem.Name = "chkBreakPageAfterEachItem";
			this.chkBreakPageAfterEachItem.AutoSizeInLayoutControl = true;
			this.chkBreakPageAfterEachItem.Properties.AccessibleName = resources.GetString("chkBreakPageAfterEachItem.Properties.AccessibleName");
			this.chkBreakPageAfterEachItem.Properties.AutoWidth = true;
			this.chkBreakPageAfterEachItem.Properties.Caption = resources.GetString("chkBreakPageAfterEachItem.Properties.Caption");
			this.chkBreakPageAfterEachItem.CheckedChanged += new System.EventHandler(this.BreakPageAfterEachItemCheckedChanged);
			this.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.Appearance.Options.UseBackColor = true;
			this.Controls.Add(this.chkBreakPageAfterEachItem);
			this.Name = "MemoPrintStyleOptionsControl";
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			((System.ComponentModel.ISupportInitialize)(this.chkBreakPageAfterEachItem.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected internal override bool IsValidPrintStyle(SchedulerPrintStyle style) {
			return style is MemoPrintStyle;
		}
		protected internal override void SubscribeEvents() {
			base.SubscribeEvents();
			chkBreakPageAfterEachItem.CheckedChanged += new EventHandler(BreakPageAfterEachItemCheckedChanged);
		}
		protected internal override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			chkBreakPageAfterEachItem.CheckedChanged -= new EventHandler(BreakPageAfterEachItemCheckedChanged);
		}
		protected internal override void RefreshDataCore(SchedulerPrintStyle printStyle) {
			base.RefreshDataCore(printStyle);
			MemoPrintStyle memoPrintStyle = (MemoPrintStyle)printStyle;
			chkBreakPageAfterEachItem.Checked = memoPrintStyle.BreakPageAfterEachItem;
		}
		protected internal virtual void BreakPageAfterEachItemCheckedChanged(object sender, System.EventArgs e) {
			PrintStyle.BreakPageAfterEachItem = chkBreakPageAfterEachItem.Checked;
			OnPrintStyleChanged();
		}
		protected internal override SchedulerPrintStyle CreateDefaultPrintStyle() {
			return new MemoPrintStyle();
		}
	}
}
