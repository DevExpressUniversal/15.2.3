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

namespace DevExpress.XtraMap.Printing {
	partial class PrintingOptionsEditor {
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintingOptionsEditor));
			this.lcItemsTitle = new DevExpress.XtraEditors.LabelControl();
			this.cheNavPanel = new DevExpress.XtraEditors.CheckEdit();
			this.cheZoom = new DevExpress.XtraEditors.CheckEdit();
			this.cheStretch = new DevExpress.XtraEditors.CheckEdit();
			this.cheNone = new DevExpress.XtraEditors.CheckEdit();
			this.cheMiniMap = new DevExpress.XtraEditors.CheckEdit();
			this.lcSizeModeTitle = new DevExpress.XtraEditors.LabelControl();
			this.cheAuto = new DevExpress.XtraEditors.CheckEdit();
			this.cheSqueeze = new DevExpress.XtraEditors.CheckEdit();
			this.cheCenter = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cheNavPanel.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cheZoom.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cheStretch.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cheNone.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cheMiniMap.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cheAuto.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cheSqueeze.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cheCenter.Properties)).BeginInit();
			this.SuspendLayout();
			this.lbCaption.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lbCaption.Appearance.Font")));
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.pnlMain.Controls.Add(this.lcItemsTitle);
			this.pnlMain.Controls.Add(this.cheNavPanel);
			this.pnlMain.Controls.Add(this.cheZoom);
			this.pnlMain.Controls.Add(this.cheStretch);
			this.pnlMain.Controls.Add(this.cheNone);
			this.pnlMain.Controls.Add(this.cheMiniMap);
			this.pnlMain.Controls.Add(this.lcSizeModeTitle);
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			resources.ApplyResources(this.lcItemsTitle, "lcItemsTitle");
			this.lcItemsTitle.Name = "lcItemsTitle";
			resources.ApplyResources(this.cheNavPanel, "cheNavPanel");
			this.cheNavPanel.Name = "cheNavPanel";
			this.cheNavPanel.Properties.Appearance.Options.UseTextOptions = true;
			this.cheNavPanel.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.cheNavPanel.Properties.Caption = resources.GetString("cheNavPanel.Properties.Caption");
			this.cheNavPanel.TabStop = false;
			this.cheNavPanel.CheckedChanged += new System.EventHandler(this.ItemsCheckedChanged);
			resources.ApplyResources(this.cheZoom, "cheZoom");
			this.cheZoom.Name = "cheZoom";
			this.cheZoom.Properties.Appearance.Options.UseTextOptions = true;
			this.cheZoom.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.cheZoom.Properties.Caption = resources.GetString("cheZoom.Properties.Caption");
			this.cheZoom.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.cheZoom.Properties.RadioGroupIndex = 0;
			this.cheZoom.TabStop = false;
			this.cheZoom.CheckedChanged += new System.EventHandler(this.SizeModeCheckedChanged);
			resources.ApplyResources(this.cheStretch, "cheStretch");
			this.cheStretch.Name = "cheStretch";
			this.cheStretch.Properties.Appearance.Options.UseTextOptions = true;
			this.cheStretch.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.cheStretch.Properties.Caption = resources.GetString("cheStretch.Properties.Caption");
			this.cheStretch.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.cheStretch.Properties.RadioGroupIndex = 0;
			this.cheStretch.TabStop = false;
			this.cheStretch.CheckedChanged += new System.EventHandler(this.SizeModeCheckedChanged);
			resources.ApplyResources(this.cheNone, "cheNone");
			this.cheNone.Name = "cheNone";
			this.cheNone.Properties.Appearance.Options.UseTextOptions = true;
			this.cheNone.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.cheNone.Properties.Caption = resources.GetString("cheNone.Properties.Caption");
			this.cheNone.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.cheNone.Properties.RadioGroupIndex = 0;
			this.cheNone.TabStop = false;
			this.cheNone.CheckedChanged += new System.EventHandler(this.SizeModeCheckedChanged);
			resources.ApplyResources(this.cheMiniMap, "cheMiniMap");
			this.cheMiniMap.Name = "cheMiniMap";
			this.cheMiniMap.Properties.Appearance.Options.UseTextOptions = true;
			this.cheMiniMap.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.cheMiniMap.Properties.Caption = resources.GetString("cheMiniMap.Properties.Caption");
			this.cheMiniMap.TabStop = false;
			this.cheMiniMap.CheckedChanged += new System.EventHandler(this.ItemsCheckedChanged);
			resources.ApplyResources(this.lcSizeModeTitle, "lcSizeModeTitle");
			this.lcSizeModeTitle.Name = "lcSizeModeTitle";
			resources.ApplyResources(this.cheAuto, "cheAuto");
			this.cheAuto.Name = "cheAuto";
			this.cheAuto.Properties.Appearance.Options.UseTextOptions = true;
			this.cheAuto.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.cheAuto.Properties.Caption = resources.GetString("cheAuto.Properties.Caption");
			this.cheAuto.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.cheAuto.Properties.RadioGroupIndex = 0;
			this.cheAuto.TabStop = false;
			this.cheAuto.CheckedChanged += new System.EventHandler(this.SizeModeCheckedChanged);
			resources.ApplyResources(this.cheSqueeze, "cheSqueeze");
			this.cheSqueeze.Name = "cheSqueeze";
			this.cheSqueeze.Properties.Appearance.Options.UseTextOptions = true;
			this.cheSqueeze.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.cheSqueeze.Properties.Caption = resources.GetString("cheSqueeze.Properties.Caption");
			this.cheSqueeze.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.cheSqueeze.Properties.RadioGroupIndex = 0;
			this.cheSqueeze.TabStop = false;
			this.cheSqueeze.CheckedChanged += new System.EventHandler(this.SizeModeCheckedChanged);
			resources.ApplyResources(this.cheCenter, "cheCenter");
			this.cheCenter.Name = "cheCenter";
			this.cheCenter.Properties.Appearance.Options.UseTextOptions = true;
			this.cheCenter.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.cheCenter.Properties.Caption = resources.GetString("cheCenter.Properties.Caption");
			this.cheCenter.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.cheCenter.Properties.RadioGroupIndex = 0;
			this.cheCenter.TabStop = false;
			this.cheCenter.CheckedChanged += new System.EventHandler(this.SizeModeCheckedChanged);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Name = "PrintingOptionsEditor";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			this.pnlMain.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cheNavPanel.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cheZoom.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cheStretch.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cheNone.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cheMiniMap.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cheAuto.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cheSqueeze.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cheCenter.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.CheckEdit cheNone;
		private DevExpress.XtraEditors.LabelControl lcSizeModeTitle;
		private DevExpress.XtraEditors.LabelControl lcItemsTitle;
		private DevExpress.XtraEditors.CheckEdit cheZoom;
		private DevExpress.XtraEditors.CheckEdit cheStretch;
		private DevExpress.XtraEditors.CheckEdit cheMiniMap;
		private DevExpress.XtraEditors.CheckEdit cheNavPanel;
		private DevExpress.XtraEditors.CheckEdit cheCenter;
		private DevExpress.XtraEditors.CheckEdit cheSqueeze;
		private DevExpress.XtraEditors.CheckEdit cheAuto;
	}
}
