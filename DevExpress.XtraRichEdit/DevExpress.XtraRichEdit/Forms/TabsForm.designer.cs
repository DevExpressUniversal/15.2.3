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

using DevExpress.XtraRichEdit.Design;
namespace DevExpress.XtraRichEdit.Forms {
	partial class TabsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TabsForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.rgAlignment = new DevExpress.XtraEditors.RadioGroup();
			this.lblAlignment = new DevExpress.XtraEditors.LabelControl();
			this.rgLeader = new DevExpress.XtraEditors.RadioGroup();
			this.lblLeader = new DevExpress.XtraEditors.LabelControl();
			this.btnSet = new DevExpress.XtraEditors.SimpleButton();
			this.btnClear = new DevExpress.XtraEditors.SimpleButton();
			this.btnClearAll = new DevExpress.XtraEditors.SimpleButton();
			this.lblDefaultTabStops = new DevExpress.XtraEditors.LabelControl();
			this.lblTabStopPosition = new DevExpress.XtraEditors.LabelControl();
			this.lblTabStopsToBeCleared = new DevExpress.XtraEditors.LabelControl();
			this.lblSeparator = new DevExpress.XtraEditors.LabelControl();
			this.lblRemoveTabStops = new DevExpress.XtraEditors.LabelControl();
			this.edtDefaultTabWidth = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.tabStopPositionEdit = new DevExpress.XtraRichEdit.Design.TabStopPositionEdit();
			((System.ComponentModel.ISupportInitialize)(this.rgAlignment.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rgLeader.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtDefaultTabWidth.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnBtnOkClick);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.rgAlignment, "rgAlignment");
			this.rgAlignment.Name = "rgAlignment";
			this.rgAlignment.Properties.AccessibleName = resources.GetString("rgAlignment.Properties.AccessibleName");
			this.rgAlignment.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgAlignment.Properties.Appearance.BackColor")));
			this.rgAlignment.Properties.Appearance.Options.UseBackColor = true;
			this.rgAlignment.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgAlignment.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgAlignment.Properties.Items"))), resources.GetString("rgAlignment.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgAlignment.Properties.Items2"))), resources.GetString("rgAlignment.Properties.Items3")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgAlignment.Properties.Items4"))), resources.GetString("rgAlignment.Properties.Items5")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgAlignment.Properties.Items6"))), resources.GetString("rgAlignment.Properties.Items7"))});
			resources.ApplyResources(this.lblAlignment, "lblAlignment");
			this.lblAlignment.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblAlignment.LineVisible = true;
			this.lblAlignment.Name = "lblAlignment";
			resources.ApplyResources(this.rgLeader, "rgLeader");
			this.rgLeader.Name = "rgLeader";
			this.rgLeader.Properties.AccessibleName = resources.GetString("rgLeader.Properties.AccessibleName");
			this.rgLeader.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgLeader.Properties.Appearance.BackColor")));
			this.rgLeader.Properties.Appearance.Options.UseBackColor = true;
			this.rgLeader.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgLeader.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgLeader.Properties.Items"))), resources.GetString("rgLeader.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgLeader.Properties.Items2"))), resources.GetString("rgLeader.Properties.Items3")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgLeader.Properties.Items4"))), resources.GetString("rgLeader.Properties.Items5")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgLeader.Properties.Items6"))), resources.GetString("rgLeader.Properties.Items7")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgLeader.Properties.Items8"))), resources.GetString("rgLeader.Properties.Items9")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgLeader.Properties.Items10"))), resources.GetString("rgLeader.Properties.Items11")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgLeader.Properties.Items12"))), resources.GetString("rgLeader.Properties.Items13"))});
			resources.ApplyResources(this.lblLeader, "lblLeader");
			this.lblLeader.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblLeader.LineVisible = true;
			this.lblLeader.Name = "lblLeader";
			resources.ApplyResources(this.btnSet, "btnSet");
			this.btnSet.Name = "btnSet";
			this.btnSet.Click += new System.EventHandler(this.OnBtnSetClick);
			resources.ApplyResources(this.btnClear, "btnClear");
			this.btnClear.Name = "btnClear";
			this.btnClear.Click += new System.EventHandler(this.OnBtnClearClick);
			resources.ApplyResources(this.btnClearAll, "btnClearAll");
			this.btnClearAll.Name = "btnClearAll";
			this.btnClearAll.Click += new System.EventHandler(this.OnBtnClearAllClick);
			resources.ApplyResources(this.lblDefaultTabStops, "lblDefaultTabStops");
			this.lblDefaultTabStops.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblDefaultTabStops.Name = "lblDefaultTabStops";
			resources.ApplyResources(this.lblTabStopPosition, "lblTabStopPosition");
			this.lblTabStopPosition.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblTabStopPosition.Name = "lblTabStopPosition";
			resources.ApplyResources(this.lblTabStopsToBeCleared, "lblTabStopsToBeCleared");
			this.lblTabStopsToBeCleared.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblTabStopsToBeCleared.Name = "lblTabStopsToBeCleared";
			resources.ApplyResources(this.lblSeparator, "lblSeparator");
			this.lblSeparator.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.lblSeparator.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.lblSeparator.LineVisible = true;
			this.lblSeparator.Name = "lblSeparator";
			resources.ApplyResources(this.lblRemoveTabStops, "lblRemoveTabStops");
			this.lblRemoveTabStops.Name = "lblRemoveTabStops";
			resources.ApplyResources(this.edtDefaultTabWidth, "edtDefaultTabWidth");
			this.edtDefaultTabWidth.Name = "edtDefaultTabWidth";
			this.edtDefaultTabWidth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtDefaultTabWidth.Properties.IsValueInPercent = false;
			this.tabStopPositionEdit.EditValue = "";
			resources.ApplyResources(this.tabStopPositionEdit, "tabStopPositionEdit");
			this.tabStopPositionEdit.Name = "tabStopPositionEdit";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.edtDefaultTabWidth);
			this.Controls.Add(this.tabStopPositionEdit);
			this.Controls.Add(this.lblRemoveTabStops);
			this.Controls.Add(this.lblSeparator);
			this.Controls.Add(this.lblTabStopsToBeCleared);
			this.Controls.Add(this.lblTabStopPosition);
			this.Controls.Add(this.lblDefaultTabStops);
			this.Controls.Add(this.btnClearAll);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.btnSet);
			this.Controls.Add(this.lblLeader);
			this.Controls.Add(this.rgLeader);
			this.Controls.Add(this.lblAlignment);
			this.Controls.Add(this.rgAlignment);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TabsForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.rgAlignment.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rgLeader.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtDefaultTabWidth.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.LabelControl lblAlignment;
		protected DevExpress.XtraEditors.RadioGroup rgLeader;
		protected DevExpress.XtraEditors.LabelControl lblLeader;
		protected DevExpress.XtraEditors.SimpleButton btnSet;
		protected DevExpress.XtraEditors.SimpleButton btnClear;
		protected DevExpress.XtraEditors.SimpleButton btnClearAll;
		protected DevExpress.XtraEditors.LabelControl lblDefaultTabStops;
		protected DevExpress.XtraEditors.LabelControl lblTabStopPosition;
		protected DevExpress.XtraEditors.LabelControl lblTabStopsToBeCleared;
		protected DevExpress.XtraEditors.LabelControl lblSeparator;
		protected DevExpress.XtraEditors.RadioGroup rgAlignment;
		protected DevExpress.XtraEditors.LabelControl lblRemoveTabStops;
		protected TabStopPositionEdit tabStopPositionEdit;
		protected RichTextIndentEdit edtDefaultTabWidth;
	}
}
