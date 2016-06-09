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
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPrinting;
using System.Reflection;
using DevExpress.XtraScheduler.Printing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Design.PrintStyleControls;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.grpPaperSizes")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.grpPageSize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.grpMargins")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.lblTop")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.lblBottom")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.lblLeft")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.lblRight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.edtTop")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.edtBottom")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.edtLeft")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.edtRight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.cbPaperSource")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.lbPageSize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.edtPageWidth")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.edtPageHeight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.lblType")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.lblPaperSource")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.lblSize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.lblPageDimensions")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.lblPageWidth")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.lblPageHeight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.pctPreview")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.grpPageOrientation")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.lbPaperSize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.chkApplySettingsToAllStyles")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.chkLandscape")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupPaperTabControl.chkPortrait")]
#endregion
namespace DevExpress.XtraScheduler.Design {
	#region PageSetupPaperTabControl
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class PageSetupPaperTabControl : DevExpress.XtraEditors.XtraUserControl {
		#region Fields
		protected GroupControl grpPaperSizes;
		protected GroupControl grpPageSize;
		protected GroupControl grpMargins;
		protected DevExpress.XtraEditors.LabelControl lblTop;
		protected DevExpress.XtraEditors.LabelControl lblBottom;
		protected DevExpress.XtraEditors.LabelControl lblLeft;
		protected DevExpress.XtraEditors.LabelControl lblRight;
		protected TextEdit edtTop;
		protected TextEdit edtBottom;
		protected TextEdit edtLeft;
		protected TextEdit edtRight;
		protected ComboBoxEdit cbPaperSource;
		protected ListBoxControl lbPageSize;
		protected TextEdit edtPageWidth;
		protected TextEdit edtPageHeight;
		protected DevExpress.XtraEditors.LabelControl lblType;
		protected DevExpress.XtraEditors.LabelControl lblPaperSource;
		protected DevExpress.XtraEditors.LabelControl lblSize;
		protected DevExpress.XtraEditors.LabelControl lblPageDimensions;
		protected DevExpress.XtraEditors.LabelControl lblPageWidth;
		protected DevExpress.XtraEditors.LabelControl lblPageHeight;
		protected PicturePreviewControl pctPreview;
		protected GroupControl grpPageOrientation;
		IContainer components = null;
		PaperSize customPaperSize;
		protected ListBoxControl lbPaperSize;
		protected CheckEdit chkApplySettingsToAllStyles;
		protected CheckEdit chkLandscape;
		protected CheckEdit chkPortrait;
		PageSettings pageSettings;
		#endregion
		public PageSetupPaperTabControl() {
			InitializeComponent();
			SubscribeEvents();
		}
		#region Properties
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		protected internal virtual PrinterSettings PrinterSettings {
			get {
				if (pageSettings == null)
					return null;
				return pageSettings.PrinterSettings;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public PageSettings PageSettings {
			get { return pageSettings; }
			set {
				if (value == null)
					Exceptions.ThrowArgumentNullException("PageSettings");
				bool printersNotFound = (GetInstalledPrintersCount() < 1);
				if (printersNotFound) {
					this.Enabled = false;
					pageSettings = null;
					return;
				}
				if (pageSettings == value)
					return;
				bool requireUpdatePrinterSpecificData = (pageSettings == null);
				pageSettings = value;
				this.Enabled = true;
				UpdatePrinterData(requireUpdatePrinterSpecificData);
			}
		}
		public bool ApplySettingsToAllStyles {
			get { return chkApplySettingsToAllStyles.Checked; }
			set {
				if (chkApplySettingsToAllStyles.Checked == value)
					return;
				chkApplySettingsToAllStyles.Checked = value;
			}
		}
		#endregion
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageSetupPaperTabControl));
			this.grpPaperSizes = new DevExpress.XtraEditors.GroupControl();
			this.lbPaperSize = new DevExpress.XtraEditors.ListBoxControl();
			this.cbPaperSource = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblType = new DevExpress.XtraEditors.LabelControl();
			this.lblPaperSource = new DevExpress.XtraEditors.LabelControl();
			this.grpPageSize = new DevExpress.XtraEditors.GroupControl();
			this.edtPageHeight = new DevExpress.XtraEditors.TextEdit();
			this.lblPageHeight = new DevExpress.XtraEditors.LabelControl();
			this.lblPageWidth = new DevExpress.XtraEditors.LabelControl();
			this.edtPageWidth = new DevExpress.XtraEditors.TextEdit();
			this.lbPageSize = new DevExpress.XtraEditors.ListBoxControl();
			this.lblPageDimensions = new DevExpress.XtraEditors.LabelControl();
			this.lblSize = new DevExpress.XtraEditors.LabelControl();
			this.grpMargins = new DevExpress.XtraEditors.GroupControl();
			this.edtRight = new DevExpress.XtraEditors.TextEdit();
			this.edtLeft = new DevExpress.XtraEditors.TextEdit();
			this.edtBottom = new DevExpress.XtraEditors.TextEdit();
			this.edtTop = new DevExpress.XtraEditors.TextEdit();
			this.lblRight = new DevExpress.XtraEditors.LabelControl();
			this.lblLeft = new DevExpress.XtraEditors.LabelControl();
			this.lblBottom = new DevExpress.XtraEditors.LabelControl();
			this.lblTop = new DevExpress.XtraEditors.LabelControl();
			this.grpPageOrientation = new DevExpress.XtraEditors.GroupControl();
			this.chkLandscape = new DevExpress.XtraEditors.CheckEdit();
			this.chkPortrait = new DevExpress.XtraEditors.CheckEdit();
			this.pctPreview = new DevExpress.XtraScheduler.Design.PrintStyleControls.PicturePreviewControl();
			this.chkApplySettingsToAllStyles = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.grpPaperSizes)).BeginInit();
			this.grpPaperSizes.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbPaperSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPaperSource.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpPageSize)).BeginInit();
			this.grpPageSize.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtPageHeight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPageWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbPageSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpMargins)).BeginInit();
			this.grpMargins.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtRight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtLeft.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtBottom.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtTop.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpPageOrientation)).BeginInit();
			this.grpPageOrientation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkLandscape.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkPortrait.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkApplySettingsToAllStyles.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grpPaperSizes, "grpPaperSizes");
			this.grpPaperSizes.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.grpPaperSizes.Controls.Add(this.lbPaperSize);
			this.grpPaperSizes.Controls.Add(this.cbPaperSource);
			this.grpPaperSizes.Controls.Add(this.lblType);
			this.grpPaperSizes.Controls.Add(this.lblPaperSource);
			this.grpPaperSizes.Name = "grpPaperSizes";
			resources.ApplyResources(this.lbPaperSize, "lbPaperSize");
			this.lbPaperSize.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lbPaperSize.ItemHeight = 16;
			this.lbPaperSize.Name = "lbPaperSize";
			resources.ApplyResources(this.cbPaperSource, "cbPaperSource");
			this.cbPaperSource.Name = "cbPaperSource";
			this.cbPaperSource.Properties.AccessibleName = resources.GetString("cbPaperSource.Properties.AccessibleName");
			this.cbPaperSource.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbPaperSource.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPaperSource.Properties.Buttons"))))});
			this.cbPaperSource.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.lblType, "lblType");
			this.lblType.Name = "lblType";
			resources.ApplyResources(this.lblPaperSource, "lblPaperSource");
			this.lblPaperSource.Name = "lblPaperSource";
			resources.ApplyResources(this.grpPageSize, "grpPageSize");
			this.grpPageSize.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.grpPageSize.Controls.Add(this.edtPageHeight);
			this.grpPageSize.Controls.Add(this.lblPageHeight);
			this.grpPageSize.Controls.Add(this.lblPageWidth);
			this.grpPageSize.Controls.Add(this.edtPageWidth);
			this.grpPageSize.Controls.Add(this.lbPageSize);
			this.grpPageSize.Controls.Add(this.lblPageDimensions);
			this.grpPageSize.Controls.Add(this.lblSize);
			this.grpPageSize.Name = "grpPageSize";
			resources.ApplyResources(this.edtPageHeight, "edtPageHeight");
			this.edtPageHeight.Name = "edtPageHeight";
			this.edtPageHeight.Properties.AccessibleName = resources.GetString("edtPageHeight.Properties.AccessibleName");
			this.edtPageHeight.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			resources.ApplyResources(this.lblPageHeight, "lblPageHeight");
			this.lblPageHeight.Name = "lblPageHeight";
			resources.ApplyResources(this.lblPageWidth, "lblPageWidth");
			this.lblPageWidth.Name = "lblPageWidth";
			resources.ApplyResources(this.edtPageWidth, "edtPageWidth");
			this.edtPageWidth.Name = "edtPageWidth";
			this.edtPageWidth.Properties.AccessibleName = resources.GetString("edtPageWidth.Properties.AccessibleName");
			this.edtPageWidth.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			resources.ApplyResources(this.lbPageSize, "lbPageSize");
			this.lbPageSize.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lbPageSize.ItemHeight = 16;
			this.lbPageSize.Name = "lbPageSize";
			resources.ApplyResources(this.lblPageDimensions, "lblPageDimensions");
			this.lblPageDimensions.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPageDimensions.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.lblPageDimensions.Appearance.Options.UseBackColor = true;
			this.lblPageDimensions.Name = "lblPageDimensions";
			resources.ApplyResources(this.lblSize, "lblSize");
			this.lblSize.Name = "lblSize";
			resources.ApplyResources(this.grpMargins, "grpMargins");
			this.grpMargins.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.grpMargins.Controls.Add(this.edtRight);
			this.grpMargins.Controls.Add(this.edtLeft);
			this.grpMargins.Controls.Add(this.edtBottom);
			this.grpMargins.Controls.Add(this.edtTop);
			this.grpMargins.Controls.Add(this.lblRight);
			this.grpMargins.Controls.Add(this.lblLeft);
			this.grpMargins.Controls.Add(this.lblBottom);
			this.grpMargins.Controls.Add(this.lblTop);
			this.grpMargins.Name = "grpMargins";
			resources.ApplyResources(this.edtRight, "edtRight");
			this.edtRight.Name = "edtRight";
			this.edtRight.Properties.AccessibleName = resources.GetString("edtRight.Properties.AccessibleName");
			this.edtRight.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			resources.ApplyResources(this.edtLeft, "edtLeft");
			this.edtLeft.Name = "edtLeft";
			this.edtLeft.Properties.AccessibleName = resources.GetString("edtLeft.Properties.AccessibleName");
			this.edtLeft.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.edtBottom, "edtBottom");
			this.edtBottom.Name = "edtBottom";
			this.edtBottom.Properties.AccessibleName = resources.GetString("edtBottom.Properties.AccessibleName");
			this.edtBottom.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			resources.ApplyResources(this.edtTop, "edtTop");
			this.edtTop.Name = "edtTop";
			this.edtTop.Properties.AccessibleName = resources.GetString("edtTop.Properties.AccessibleName");
			this.edtTop.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			resources.ApplyResources(this.lblRight, "lblRight");
			this.lblRight.Name = "lblRight";
			resources.ApplyResources(this.lblLeft, "lblLeft");
			this.lblLeft.Name = "lblLeft";
			resources.ApplyResources(this.lblBottom, "lblBottom");
			this.lblBottom.Name = "lblBottom";
			resources.ApplyResources(this.lblTop, "lblTop");
			this.lblTop.Name = "lblTop";
			resources.ApplyResources(this.grpPageOrientation, "grpPageOrientation");
			this.grpPageOrientation.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.grpPageOrientation.Controls.Add(this.chkLandscape);
			this.grpPageOrientation.Controls.Add(this.chkPortrait);
			this.grpPageOrientation.Controls.Add(this.pctPreview);
			this.grpPageOrientation.Name = "grpPageOrientation";
			resources.ApplyResources(this.chkLandscape, "chkLandscape");
			this.chkLandscape.Name = "chkLandscape";
			this.chkLandscape.Properties.AccessibleName = resources.GetString("chkLandscape.Properties.AccessibleName");
			this.chkLandscape.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkLandscape.Properties.AutoWidth = true;
			this.chkLandscape.Properties.Caption = resources.GetString("chkLandscape.Properties.Caption");
			this.chkLandscape.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkLandscape.Properties.RadioGroupIndex = 1;
			this.chkLandscape.TabStop = false;
			resources.ApplyResources(this.chkPortrait, "chkPortrait");
			this.chkPortrait.Name = "chkPortrait";
			this.chkPortrait.Properties.AccessibleName = resources.GetString("chkPortrait.Properties.AccessibleName");
			this.chkPortrait.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkPortrait.Properties.AutoWidth = true;
			this.chkPortrait.Properties.Caption = resources.GetString("chkPortrait.Properties.Caption");
			this.chkPortrait.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkPortrait.Properties.RadioGroupIndex = 1;
			this.chkPortrait.TabStop = false;
			this.pctPreview.AccessibleRole = System.Windows.Forms.AccessibleRole.Client;
			this.pctPreview.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.pctPreview.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.pctPreview, "pctPreview");
			this.pctPreview.Name = "pctPreview";
			this.pctPreview.PageHeight = 0;
			this.pctPreview.PageWidth = 0;
			this.pctPreview.PaperHeight = 0;
			this.pctPreview.PaperWidth = 0;
			this.pctPreview.TabStop = false;
			resources.ApplyResources(this.chkApplySettingsToAllStyles, "chkApplySettingsToAllStyles");
			this.chkApplySettingsToAllStyles.Name = "chkApplySettingsToAllStyles";
			this.chkApplySettingsToAllStyles.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.chkApplySettingsToAllStyles.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
			this.chkApplySettingsToAllStyles.Properties.Appearance.Options.UseBackColor = true;
			this.chkApplySettingsToAllStyles.Properties.Appearance.Options.UseFont = true;
			this.chkApplySettingsToAllStyles.Properties.AutoWidth = true;
			this.chkApplySettingsToAllStyles.Properties.Caption = resources.GetString("chkApplySettingsToAllStyles.Properties.Caption");
			this.Controls.Add(this.chkApplySettingsToAllStyles);
			this.Controls.Add(this.grpPageOrientation);
			this.Controls.Add(this.grpMargins);
			this.Controls.Add(this.grpPageSize);
			this.Controls.Add(this.grpPaperSizes);
			this.Name = "PageSetupPaperTabControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.grpPaperSizes)).EndInit();
			this.grpPaperSizes.ResumeLayout(false);
			this.grpPaperSizes.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbPaperSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPaperSource.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpPageSize)).EndInit();
			this.grpPageSize.ResumeLayout(false);
			this.grpPageSize.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtPageHeight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPageWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbPageSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpMargins)).EndInit();
			this.grpMargins.ResumeLayout(false);
			this.grpMargins.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtRight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtLeft.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtBottom.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtTop.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpPageOrientation)).EndInit();
			this.grpPageOrientation.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chkLandscape.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkPortrait.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkApplySettingsToAllStyles.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		#region UpdateData
		protected internal virtual void UpdatePrinterData(bool requireUpdatePrinterSpecificData) {
			XtraSchedulerDebug.Assert(PageSettings != null);
			if (requireUpdatePrinterSpecificData)
				UpdatePrinterSpecificData();
			UpdateChangeableData();
		}
		protected internal virtual void UpdatePrinterSpecificData() {
			UnsubscribeEvents();
			UpdatePaperGroup();
			SubscribeEvents();
		}
		protected internal virtual void UpdateChangeableData() {
			XtraSchedulerDebug.Assert(PageSettings != null);
			if (!IsPaperInfoExist()) {
				this.Enabled = false;
				return;
			}
			UnsubscribeEvents();
			UpdatePageGroup();
			UpdatePageOrientationGroup();
			UpdatePaperMarginsGroup();
			SetCurrentPaperSizePosition();
			SetCurrentPaperSourcesPosition();
			SubscribeEvents();
		}
		protected internal virtual bool IsPaperInfoExist() {
			return PrinterSettings.PaperSizes.Count != 0;
		}
		protected internal virtual void SubscribeEvents() {
			lbPaperSize.SelectedIndexChanged += new EventHandler(OnPaperSizeSelectedChanged);
			cbPaperSource.SelectedIndexChanged += new EventHandler(OnPaperSourceSelectedChanged);
			lbPageSize.SelectedIndexChanged += new EventHandler(OnPageSizeSelectedChanged);
			edtRight.Validating += new CancelEventHandler(OnEdtSizeValidating);
			edtRight.Validated += new EventHandler(OnEdtMarginsValidated);
			edtRight.InvalidValue += new InvalidValueExceptionEventHandler(OnEdtSizeInvalidValue);
			edtLeft.Validating += new CancelEventHandler(OnEdtSizeValidating);
			edtLeft.Validated += new EventHandler(OnEdtMarginsValidated);
			edtLeft.InvalidValue += new InvalidValueExceptionEventHandler(OnEdtSizeInvalidValue);
			edtBottom.Validating += new CancelEventHandler(OnEdtSizeValidating);
			edtBottom.Validated += new EventHandler(OnEdtMarginsValidated);
			edtBottom.InvalidValue += new InvalidValueExceptionEventHandler(OnEdtSizeInvalidValue);
			edtTop.Validating += new CancelEventHandler(OnEdtSizeValidating);
			edtTop.Validated += new EventHandler(OnEdtMarginsValidated);
			edtTop.InvalidValue += new InvalidValueExceptionEventHandler(OnEdtSizeInvalidValue);
			edtPageWidth.Validating += new CancelEventHandler(OnEdtSizeValidating);
			edtPageWidth.Validated += new EventHandler(OnEdtPageSizeValidated);
			edtPageWidth.InvalidValue += new InvalidValueExceptionEventHandler(OnEdtSizeInvalidValue);
			edtPageHeight.Validating += new CancelEventHandler(OnEdtSizeValidating);
			edtPageHeight.Validated += new EventHandler(OnEdtPageSizeValidated);
			edtPageHeight.InvalidValue += new InvalidValueExceptionEventHandler(OnEdtSizeInvalidValue);
			chkPortrait.CheckedChanged += new EventHandler(OnOrientationChanged);
			chkLandscape.CheckedChanged += new EventHandler(OnOrientationChanged);
		}
		protected internal virtual void UnsubscribeEvents() {
			lbPaperSize.SelectedIndexChanged -= new EventHandler(OnPaperSizeSelectedChanged);
			cbPaperSource.SelectedIndexChanged -= new EventHandler(OnPaperSourceSelectedChanged);
			lbPageSize.SelectedIndexChanged -= new EventHandler(OnPageSizeSelectedChanged);
			edtRight.Validating -= new CancelEventHandler(OnEdtSizeValidating);
			edtRight.Validated -= new EventHandler(OnEdtMarginsValidated);
			edtRight.InvalidValue -= new InvalidValueExceptionEventHandler(OnEdtSizeInvalidValue);
			edtLeft.Validating -= new CancelEventHandler(OnEdtSizeValidating);
			edtLeft.Validated -= new EventHandler(OnEdtMarginsValidated);
			edtLeft.InvalidValue -= new InvalidValueExceptionEventHandler(OnEdtSizeInvalidValue);
			edtBottom.Validating -= new CancelEventHandler(OnEdtSizeValidating);
			edtBottom.Validated -= new EventHandler(OnEdtMarginsValidated);
			edtBottom.InvalidValue -= new InvalidValueExceptionEventHandler(OnEdtSizeInvalidValue);
			edtTop.Validating -= new CancelEventHandler(OnEdtSizeValidating);
			edtTop.Validated -= new EventHandler(OnEdtMarginsValidated);
			edtTop.InvalidValue -= new InvalidValueExceptionEventHandler(OnEdtSizeInvalidValue);
			edtPageWidth.Validating -= new CancelEventHandler(OnEdtSizeValidating);
			edtPageWidth.Validated -= new EventHandler(OnEdtPageSizeValidated);
			edtPageWidth.InvalidValue -= new InvalidValueExceptionEventHandler(OnEdtSizeInvalidValue);
			edtPageHeight.Validating -= new CancelEventHandler(OnEdtSizeValidating);
			edtPageHeight.Validated -= new EventHandler(OnEdtPageSizeValidated);
			edtPageHeight.InvalidValue -= new InvalidValueExceptionEventHandler(OnEdtSizeInvalidValue);
			chkPortrait.CheckedChanged -= new EventHandler(OnOrientationChanged);
			chkLandscape.CheckedChanged -= new EventHandler(OnOrientationChanged);
		}
		#endregion
		#region UpdatePaperGroup
		protected internal virtual void UpdatePaperGroup() {
			FillPaperSizes();
			SetCurrentPaperSizePosition();
			FillPaperSources();
			SetCurrentPaperSourcesPosition();
		}
		protected internal virtual void FillPaperSizes() {
			lbPaperSize.BeginUpdate();
			lbPaperSize.Items.Clear();
			PrinterSettings.PaperSizeCollection paperSizes = GetPaperSizes();
			XtraSchedulerDebug.Assert(paperSizes != null);
			int count = paperSizes.Count;
			for (int i = 0; i < count; i++) {
				PaperSize size = paperSizes[i];
				if (size.Kind == PaperKind.Custom)
					customPaperSize = size;
				lbPaperSize.Items.Add(new ObjectWrapper(size, size.PaperName));
			}
			if (customPaperSize != null)
				EnableCustomPaperSize();
			else
				DisableCustomPaperSize();
			lbPaperSize.EndUpdate();
		}
		protected internal virtual void EnableCustomPaperSize() {
			edtPageWidth.Enabled = true;
			edtPageHeight.Enabled = true;
		}
		protected internal virtual void DisableCustomPaperSize() {
			edtPageWidth.Enabled = false;
			edtPageHeight.Enabled = false;
		}
		protected internal virtual PrinterSettings.PaperSizeCollection GetPaperSizes() {
			XtraSchedulerDebug.Assert(PrinterSettings != null);
			PrinterSettings.PaperSizeCollection paperSizes = PrinterSettings.PaperSizes;
			return paperSizes;
		}
		protected internal virtual void SetCurrentPaperSizePosition() {
			if (GetInstalledPrintersCount() <= 0)
				return;
			XtraSchedulerDebug.Assert(PageSettings != null);
			XtraSchedulerDebug.Assert(PageSettings.PaperSize != null);
			PaperSize selectedPaperSize = PageSettings.PaperSize;
			int paperSizeIndex = FindPaperSizeIndex(selectedPaperSize);
			if (paperSizeIndex >= 0) {
				lbPaperSize.SelectedIndex = paperSizeIndex;
			}
			else {
				lbPaperSize.SelectedIndex = 0;
				ObjectWrapper item = (ObjectWrapper)lbPaperSize.SelectedItem;
				if (item != null)
					PageSettings.PaperSize = (PaperSize)item.Object;
			}
			selectedPaperSize = PageSettings.PaperSize;
		}
		protected internal virtual int FindPaperSizeIndex(PaperSize paperSize) {
			ListBoxItemCollection items = lbPaperSize.Items;
			int count = items.Count;
			int kind = paperSize.RawKind;
			for (int i = 0; i < count; i++) {
				ObjectWrapper item = (ObjectWrapper)items[i];
				PaperSize size = (PaperSize)item.Object;
				if (kind == size.RawKind)
					return i;
			}
			return -1;
		}
		protected internal virtual void FillPaperSources() {
			ComboBoxItemCollection items = cbPaperSource.Properties.Items;
			items.Clear();
			PrinterSettings.PaperSourceCollection paperSources = GetPaperSources();
			int count = paperSources.Count;
			for (int i = 0; i < count; i++) {
				PaperSource source = paperSources[i];
				items.Add(new ObjectWrapper(source, source.SourceName));
			}
		}
		protected internal virtual PrinterSettings.PaperSourceCollection GetPaperSources() {
			XtraSchedulerDebug.Assert(PrinterSettings != null);
			XtraSchedulerDebug.Assert(PrinterSettings.PaperSources != null);
			PrinterSettings.PaperSourceCollection paperSources = PrinterSettings.PaperSources;
			return paperSources;
		}
		protected internal virtual void SetCurrentPaperSourcesPosition() {
			if (GetInstalledPrintersCount() <= 0)
				return;
			PaperSourceKind paperSourceKind = PageSettings.PaperSource.Kind;
			int paperSourceKindIndex = FindPaperKindIndex(paperSourceKind);
			if (paperSourceKindIndex >= 0)
				cbPaperSource.SelectedIndex = paperSourceKindIndex;
			else {
				cbPaperSource.SelectedIndex = 0;
				if (cbPaperSource.SelectedItem != null)
					PageSettings.PaperSource = ((ObjectWrapper)cbPaperSource.SelectedItem).Object as PaperSource;
			}
		}
		protected internal virtual int GetInstalledPrintersCount() {
			return PrinterSettings.InstalledPrinters.Count;
		}
		protected internal virtual int FindPaperKindIndex(PaperSourceKind paperSourceKind) {
			ComboBoxItemCollection items = cbPaperSource.Properties.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				ObjectWrapper item = (ObjectWrapper)items[i];
				PaperSource itemPaperSource = (PaperSource)item.Object;
				if (paperSourceKind == (PaperSourceKind)itemPaperSource.RawKind)
					return i;
			}
			return -1;
		}
		#endregion
		#region UpdatePageOrientationGroup
		protected internal virtual void UpdatePageOrientationGroup() {
			UpdatePreviewImage();
			if (PageSettings.Landscape)
				chkLandscape.Checked = true;
			else
				chkPortrait.Checked = true;
		}
		protected internal virtual void UpdatePreviewImage() {
			PageSize pageSize = lbPageSize.SelectedItem as PageSize;
			if (pageSize == null)
				return;
			PaperSize paperSize = PageSettings.PaperSize;
			int paperWidth = pageSize.RotatePaper ? paperSize.Height : paperSize.Width;
			int paperHeight = pageSize.RotatePaper ? paperSize.Width : paperSize.Height;
			pctPreview.PageWidth = pageSize.Width;
			pctPreview.PageHeight = pageSize.Height;
			pctPreview.PaperWidth = paperWidth;
			pctPreview.PaperHeight = paperHeight;
			pctPreview.Invalidate();
		}
		#endregion
		#region UpdatePageGroup
		protected internal virtual void UpdatePageGroup() {
			UpdatePageSize();
			UpdateDimensions();
		}
		protected internal virtual void UpdateDimensions() {
			PageSize pageSize = lbPageSize.SelectedItem as PageSize;
			if (pageSize == null) {
				edtPageWidth.Text = edtPageHeight.Text = String.Empty;
				return;
			}
			edtPageWidth.Text = PaperSizeConverter.ToString(pageSize.Width);
			edtPageHeight.Text = PaperSizeConverter.ToString(pageSize.Height);
		}
		protected internal virtual void UpdatePageSize() {
			lbPageSize.BeginUpdate();
			try {
				lbPageSize.Items.Clear();
				if (lbPaperSize.SelectedItem != null) {
					PaperSize size = PageSettings.PaperSize;
					PageSizeCollection pageSizes = new PageSizeCollection(size, PageSettings.Landscape);
					int count = pageSizes.Count;
					for (int i = 0; i < count; i++)
						lbPageSize.Items.Add(pageSizes[i]);
				}
			}
			finally {
				lbPageSize.EndUpdate();
			}
		}
		#endregion
		#region UpdatePaperMarginsGroup
		protected internal virtual void UpdatePaperMarginsGroup() {
			XtraSchedulerDebug.Assert(PageSettings.Margins != null);
			Margins margins = PageSettings.Margins;
			edtTop.Text = PaperSizeConverter.ToString(margins.Top);
			edtLeft.Text = PaperSizeConverter.ToString(margins.Left);
			edtRight.Text = PaperSizeConverter.ToString(margins.Right);
			edtBottom.Text = PaperSizeConverter.ToString(margins.Bottom);
		}
		#endregion
		protected internal virtual void OnPaperSizeSelectedChanged(object sender, System.EventArgs e) {
			if (lbPaperSize.SelectedItem as ObjectWrapper == null)
				return;
			PaperSize size = (PaperSize)((ObjectWrapper)lbPaperSize.SelectedItem).Object;
			if (size.Kind == PaperKind.Custom) {
				size = new PaperSize();
				size.PaperName = customPaperSize.PaperName;
				size.RawKind = customPaperSize.RawKind;
				if (size.Width == 0)
					size.Width = PageSettings.PaperSize.Width;
				if (size.Height == 0)
					size.Height = PageSettings.PaperSize.Height;
			}
			PageSettings.PaperSize = size;
			UpdateChangeableData();
		}
		protected internal virtual void OnPageSizeSelectedChanged(object sender, System.EventArgs e) {
			UpdateChangeableData();
		}
		protected internal virtual void OnEdtSizeValidating(object sender, System.ComponentModel.CancelEventArgs e) {
			TextEdit textEdit = (TextEdit)sender;
			if (PaperSizeConverter.FromString(textEdit.Text) < 0)
				e.Cancel = true;
		}
		protected internal virtual void OnEdtSizeInvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e) {
			e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidSize);
		}
		protected internal virtual void OnEdtMarginsValidated(object sender, System.EventArgs e) {
			XtraSchedulerDebug.Assert(PageSettings != null);
			Margins margins = PageSettings.Margins;
			int width = PaperSizeConverter.FromString(edtPageWidth.Text);
			int height = PaperSizeConverter.FromString(edtPageHeight.Text);
			int left = PaperSizeConverter.FromString(edtLeft.Text);
			int right = PaperSizeConverter.FromString(edtRight.Text);
			int top = PaperSizeConverter.FromString(edtTop.Text);
			int bottom = PaperSizeConverter.FromString(edtBottom.Text);
			if (left + right < width && top + bottom < height) {
				margins.Left = left;
				margins.Right = right;
				margins.Top = top;
				margins.Bottom = bottom;
				UpdateChangeableData();
			}
		}
		protected internal virtual void OnEdtPageSizeValidated(object sender, System.EventArgs e) {
			XtraSchedulerDebug.Assert(PageSettings != null);
			int width = PaperSizeConverter.FromString(edtPageWidth.Text);
			int height = PaperSizeConverter.FromString(edtPageHeight.Text);
			PaperSize newSize = new PaperSize();
			newSize.PaperName = customPaperSize.PaperName;
			newSize.Width = width;
			newSize.Height = height;
			newSize.RawKind = customPaperSize.RawKind;
			PageSettings.PaperSize = newSize;
			Margins margins = PageSettings.Margins;
			int left = PaperSizeConverter.FromString(edtLeft.Text);
			int right = PaperSizeConverter.FromString(edtRight.Text);
			int top = PaperSizeConverter.FromString(edtTop.Text);
			int bottom = PaperSizeConverter.FromString(edtBottom.Text);
			if (left + right >= width) {
				margins.Left = 0;
				margins.Right = 0;
			}
			if (top + bottom >= height) {
				margins.Top = 0;
				margins.Bottom = 0;
			}
			UpdateChangeableData();
		}
		protected internal virtual void OnOrientationChanged(object sender, System.EventArgs e) {
			XtraSchedulerDebug.Assert(PageSettings != null);
			PageSettings.Landscape = IsLandscapeSelected();
			UpdateChangeableData();
		}
		protected internal virtual bool IsLandscapeSelected() {
			return chkLandscape.Checked;
		}
		protected internal virtual void OnPaperSourceSelectedChanged(object sender, System.EventArgs e) {
			XtraSchedulerDebug.Assert(PageSettings != null);
			PageSettings.PaperSource = ((ObjectWrapper)cbPaperSource.EditValue).Object as PaperSource;
			UpdateChangeableData();
		}
	}
	#endregion
	#region ObjectWrapper
	public class ObjectWrapper {
		object contentObject;
		string displayName;
		public ObjectWrapper(object contentObject, string displayName) {
			this.contentObject = contentObject;
			this.displayName = displayName;
		}
		public object Object { get { return contentObject; } }
		public override string ToString() {
			return displayName;
		}
	}
	#endregion
	#region PageSize
	public class PageSize {
		#region Fields
		string displayName;
		int width;
		int height;
		bool rotatePaper;
		#endregion
		public PageSize(string name, int width, int height, bool rotatePaper) {
			this.displayName = name;
			this.width = width;
			this.height = height;
			this.rotatePaper = rotatePaper;
		}
		#region Properties
		public bool RotatePaper { get { return rotatePaper; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		#endregion
		public override string ToString() {
			return displayName;
		}
	}
	#endregion
	#region PageSizeCollection
	public class PageSizeCollection : List<PageSize> {
		PaperSize paperSize;
		bool landscape;
		public PageSizeCollection(PaperSize paperSize, bool landscape) {
			this.paperSize = paperSize;
			this.landscape = landscape;
			if (paperSize.RawKind == (int)PaperKind.Custom) {
				PageSize pageSize = new PageSize(paperSize.PaperName, paperSize.Width, paperSize.Height, false);
				this.Add(pageSize);
				return;
			}
			AddWithCheck(paperSize.PaperName, paperSize.Width, paperSize.Height);
			AddHalfFormat();
			AddWithCheck("Billfold", 375, 675);
			AddWithCheck("Pocket", 300, 500);
			AddWithCheck("1/2 sheet booklet", paperSize.Height / 2, paperSize.Width);
			AddWithCheck("1/4 sheet booklet", paperSize.Width / 2, paperSize.Height / 2);
			AddWithCheck("1/8 sheet booklet", paperSize.Height / 4, paperSize.Width / 2);
			AddWithCheck("Day-Timer Senior Desk", 850, 1100);
			AddWithCheck("Day-Timer Senior Pocket", 375, 675);
			AddWithCheck("Day-Timer Junior Desk", 550, 850);
			AddWithCheck("Day-Timer Junior Pocket", 300, 500);
			AddWithCheck("Day Runner Entrepreneur", 850, 1100);
			AddWithCheck("Day Runner Classic", 550, 850);
			AddWithCheck("Day Runner Running Mate", 375, 675);
			AddWithCheck("Franklin Day Planner Monarch", 850, 1100);
			AddWithCheck("Franklin Day Planner Classic", 550, 850);
		}
		protected internal virtual void AddHalfFormat() {
			int firstSize = Math.Max(paperSize.Width, paperSize.Height) / 2;
			int secondSize = Math.Min(paperSize.Width, paperSize.Height);
			int minSize = Math.Min(firstSize, secondSize);
			int maxSize = Math.Max(firstSize, secondSize);
			String name = String.Format("{0} Half", paperSize.PaperName);
			AddWithCheck(name, minSize, maxSize);
		}
		protected internal virtual void AddWithCheck(string name, int width, int height) {
			if (width == 0 || height == 0)
				return;
			if (landscape) {
				int t = width; width = height; height = t;
			}
			int defaultOrientationPages = ((int)(paperSize.Width / width)) * ((int)(paperSize.Height / height));
			int rotatedOrientationPages = ((int)(paperSize.Width / height)) * ((int)(paperSize.Height / width));
			PageSize pageSize;
			if (defaultOrientationPages == 0 && rotatedOrientationPages == 0)
				return;
			if (rotatedOrientationPages > defaultOrientationPages)
				pageSize = new PageSize(name, width, height, true);
			else
				pageSize = new PageSize(name, width, height, false);
			this.Add(pageSize);
		}
	}
	#endregion
}
