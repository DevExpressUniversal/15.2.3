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
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using DevExpress.Utils.Design;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Design;
using DevExpress.Utils.Frames;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraTab;
using DevExpress.XtraGrid.Views.Printing;
using DevExpress.XtraGrid.Views;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.XtraGrid.Frames {
	[ToolboxItem(false)]
	public class LayoutViewPrintDesigner : DevExpress.XtraEditors.Designer.Utils.XtraFrame, IPrintDesigner {
		private CheckEdit printSelectedCardsOnly;
		private CheckEdit printCardCaption;
		private CheckEdit printFilterInfo;
		private CheckEdit usePrintStyles;
		private ComboBoxEdit printMode;
		private SpinEdit maximumCardColumns;
		private SpinEdit maximumCardRows;
		private PanelControl pnlGridPreview;
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
		private DevExpress.XtraLayout.SplitterItem splitterItem1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup4;
		private DevExpress.XtraLayout.SimpleLabelItem simpleLabelItem1;
		private DevExpress.XtraLayout.SimpleLabelItem simpleLabelItem2;
		private DevExpress.XtraLayout.SimpleLabelItem simpleLabelItem3;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraLayout.SimpleLabelItem simpleLabelItem4;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		bool autoApplyCore = true;
		public bool AutoApply {
			get { return autoApplyCore; }
			set { autoApplyCore = value; }
		}
		void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutViewPrintDesigner));
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.pnlGridPreview = new DevExpress.XtraEditors.PanelControl();
			this.maximumCardColumns = new DevExpress.XtraEditors.SpinEdit();
			this.maximumCardRows = new DevExpress.XtraEditors.SpinEdit();
			this.printMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.printFilterInfo = new DevExpress.XtraEditors.CheckEdit();
			this.usePrintStyles = new DevExpress.XtraEditors.CheckEdit();
			this.printSelectedCardsOnly = new DevExpress.XtraEditors.CheckEdit();
			this.printCardCaption = new DevExpress.XtraEditors.CheckEdit();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.simpleLabelItem1 = new DevExpress.XtraLayout.SimpleLabelItem();
			this.simpleLabelItem2 = new DevExpress.XtraLayout.SimpleLabelItem();
			this.simpleLabelItem3 = new DevExpress.XtraLayout.SimpleLabelItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.simpleLabelItem4 = new DevExpress.XtraLayout.SimpleLabelItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
			this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlGridPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.maximumCardColumns.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.maximumCardRows.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.printMode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.printFilterInfo.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.usePrintStyles.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.printSelectedCardsOnly.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.printCardCaption.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.pnlMain.Controls.Add(this.layoutControl1);
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.pnlGridPreview);
			this.layoutControl1.Controls.Add(this.maximumCardColumns);
			this.layoutControl1.Controls.Add(this.maximumCardRows);
			this.layoutControl1.Controls.Add(this.printMode);
			this.layoutControl1.Controls.Add(this.printFilterInfo);
			this.layoutControl1.Controls.Add(this.usePrintStyles);
			this.layoutControl1.Controls.Add(this.printSelectedCardsOnly);
			this.layoutControl1.Controls.Add(this.printCardCaption);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(533, 161, 450, 350);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.pnlGridPreview.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlGridPreview, "pnlGridPreview");
			this.pnlGridPreview.Name = "pnlGridPreview";
			resources.ApplyResources(this.maximumCardColumns, "maximumCardColumns");
			this.maximumCardColumns.Name = "maximumCardColumns";
			this.maximumCardColumns.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.maximumCardColumns.StyleController = this.layoutControl1;
			this.maximumCardColumns.EditValueChanged += new System.EventHandler(this.maximumCardColumns_EditValueChanged);
			resources.ApplyResources(this.maximumCardRows, "maximumCardRows");
			this.maximumCardRows.Name = "maximumCardRows";
			this.maximumCardRows.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.maximumCardRows.StyleController = this.layoutControl1;
			this.maximumCardRows.EditValueChanged += new System.EventHandler(this.maximumCardRows_EditValueChanged);
			resources.ApplyResources(this.printMode, "printMode");
			this.printMode.Name = "printMode";
			this.printMode.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.printMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("printMode.Properties.Buttons"))))});
			this.printMode.Properties.ImmediatePopup = true;
			this.printMode.Properties.Items.AddRange(new object[] {
			resources.GetString("printMode.Properties.Items"),
			resources.GetString("printMode.Properties.Items1"),
			resources.GetString("printMode.Properties.Items2"),
			resources.GetString("printMode.Properties.Items3"),
			resources.GetString("printMode.Properties.Items4")});
			this.printMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.printMode.StyleController = this.layoutControl1;
			this.printMode.SelectedIndexChanged += new System.EventHandler(this.printMode_SelectedIndexChanged);
			resources.ApplyResources(this.printFilterInfo, "printFilterInfo");
			this.printFilterInfo.Name = "printFilterInfo";
			this.printFilterInfo.Properties.Caption = resources.GetString("printFilterInfo.Properties.Caption");
			this.printFilterInfo.StyleController = this.layoutControl1;
			this.printFilterInfo.Tag = 0;
			this.printFilterInfo.CheckedChanged += new System.EventHandler(this.printFilterInfo_CheckedChanged);
			resources.ApplyResources(this.usePrintStyles, "usePrintStyles");
			this.usePrintStyles.Name = "usePrintStyles";
			this.usePrintStyles.Properties.Caption = resources.GetString("usePrintStyles.Properties.Caption");
			this.usePrintStyles.StyleController = this.layoutControl1;
			this.usePrintStyles.Tag = 3;
			this.usePrintStyles.CheckedChanged += new System.EventHandler(this.usePrintStyles_CheckedChanged);
			resources.ApplyResources(this.printSelectedCardsOnly, "printSelectedCardsOnly");
			this.printSelectedCardsOnly.Name = "printSelectedCardsOnly";
			this.printSelectedCardsOnly.Properties.Caption = resources.GetString("printSelectedCardsOnly.Properties.Caption");
			this.printSelectedCardsOnly.StyleController = this.layoutControl1;
			this.printSelectedCardsOnly.Tag = 2;
			this.printSelectedCardsOnly.CheckedChanged += new System.EventHandler(this.printSelectedCardsOnly_CheckedChanged);
			resources.ApplyResources(this.printCardCaption, "printCardCaption");
			this.printCardCaption.Name = "printCardCaption";
			this.printCardCaption.Properties.Caption = resources.GetString("printCardCaption.Properties.Caption");
			this.printCardCaption.StyleController = this.layoutControl1;
			this.printCardCaption.Tag = 1;
			this.printCardCaption.CheckedChanged += new System.EventHandler(this.printCardCaption_CheckedChanged);
			resources.ApplyResources(this.layoutControlGroup1, "layoutControlGroup1");
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlGroup4,
			this.splitterItem1,
			this.layoutControlGroup2});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Size = new System.Drawing.Size(493, 348);
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlGroup4.AppearanceItemCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("layoutControlGroup4.AppearanceItemCaption.BackColor")));
			this.layoutControlGroup4.AppearanceItemCaption.Options.UseBackColor = true;
			this.layoutControlGroup4.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlGroup4.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.layoutControlGroup4, "layoutControlGroup4");
			this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.simpleLabelItem1,
			this.simpleLabelItem2,
			this.simpleLabelItem3,
			this.layoutControlItem1,
			this.layoutControlItem3,
			this.layoutControlItem4,
			this.layoutControlItem6,
			this.layoutControlItem8,
			this.layoutControlItem7,
			this.emptySpaceItem2,
			this.emptySpaceItem3,
			this.emptySpaceItem1,
			this.simpleLabelItem4,
			this.layoutControlItem5});
			this.layoutControlGroup4.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup4.Name = "layoutControlGroup4";
			this.layoutControlGroup4.Size = new System.Drawing.Size(167, 328);
			this.simpleLabelItem1.AllowHotTrack = false;
			resources.ApplyResources(this.simpleLabelItem1, "simpleLabelItem1");
			this.simpleLabelItem1.Location = new System.Drawing.Point(0, 40);
			this.simpleLabelItem1.Name = "simpleLabelItem1";
			this.simpleLabelItem1.Size = new System.Drawing.Size(143, 17);
			this.simpleLabelItem1.TextSize = new System.Drawing.Size(113, 13);
			this.simpleLabelItem2.AllowHotTrack = false;
			resources.ApplyResources(this.simpleLabelItem2, "simpleLabelItem2");
			this.simpleLabelItem2.Location = new System.Drawing.Point(0, 103);
			this.simpleLabelItem2.Name = "simpleLabelItem2";
			this.simpleLabelItem2.Size = new System.Drawing.Size(143, 17);
			this.simpleLabelItem2.TextSize = new System.Drawing.Size(113, 13);
			this.simpleLabelItem3.AllowHotTrack = false;
			resources.ApplyResources(this.simpleLabelItem3, "simpleLabelItem3");
			this.simpleLabelItem3.Location = new System.Drawing.Point(0, 166);
			this.simpleLabelItem3.Name = "simpleLabelItem3";
			this.simpleLabelItem3.Size = new System.Drawing.Size(143, 17);
			this.simpleLabelItem3.TextSize = new System.Drawing.Size(113, 13);
			this.layoutControlItem1.Control = this.printCardCaption;
			resources.ApplyResources(this.layoutControlItem1, "layoutControlItem1");
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 57);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(143, 23);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextToControlDistance = 0;
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem3.Control = this.printSelectedCardsOnly;
			resources.ApplyResources(this.layoutControlItem3, "layoutControlItem3");
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 80);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(143, 23);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextToControlDistance = 0;
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem4.Control = this.printFilterInfo;
			resources.ApplyResources(this.layoutControlItem4, "layoutControlItem4");
			this.layoutControlItem4.Location = new System.Drawing.Point(0, 120);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Size = new System.Drawing.Size(143, 23);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextToControlDistance = 0;
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem6.Control = this.printMode;
			resources.ApplyResources(this.layoutControlItem6, "layoutControlItem6");
			this.layoutControlItem6.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Size = new System.Drawing.Size(143, 40);
			this.layoutControlItem6.TextLocation = DevExpress.Utils.Locations.Top;
			this.layoutControlItem6.TextSize = new System.Drawing.Size(113, 13);
			this.layoutControlItem8.Control = this.maximumCardRows;
			resources.ApplyResources(this.layoutControlItem8, "layoutControlItem8");
			this.layoutControlItem8.Location = new System.Drawing.Point(89, 224);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.Size = new System.Drawing.Size(54, 24);
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextToControlDistance = 0;
			this.layoutControlItem8.TextVisible = false;
			this.layoutControlItem7.Control = this.maximumCardColumns;
			resources.ApplyResources(this.layoutControlItem7, "layoutControlItem7");
			this.layoutControlItem7.Location = new System.Drawing.Point(89, 183);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.Size = new System.Drawing.Size(54, 24);
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextToControlDistance = 0;
			this.layoutControlItem7.TextVisible = false;
			this.emptySpaceItem2.AllowHotTrack = false;
			resources.ApplyResources(this.emptySpaceItem2, "emptySpaceItem2");
			this.emptySpaceItem2.Location = new System.Drawing.Point(0, 183);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(89, 24);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem3.AllowHotTrack = false;
			resources.ApplyResources(this.emptySpaceItem3, "emptySpaceItem3");
			this.emptySpaceItem3.Location = new System.Drawing.Point(0, 248);
			this.emptySpaceItem3.Name = "emptySpaceItem3";
			this.emptySpaceItem3.Size = new System.Drawing.Size(143, 36);
			this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem1.AllowHotTrack = false;
			resources.ApplyResources(this.emptySpaceItem1, "emptySpaceItem1");
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 224);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(89, 24);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.simpleLabelItem4.AllowHotTrack = false;
			resources.ApplyResources(this.simpleLabelItem4, "simpleLabelItem4");
			this.simpleLabelItem4.Location = new System.Drawing.Point(0, 207);
			this.simpleLabelItem4.Name = "simpleLabelItem4";
			this.simpleLabelItem4.Size = new System.Drawing.Size(143, 17);
			this.simpleLabelItem4.TextSize = new System.Drawing.Size(113, 13);
			this.layoutControlItem5.Control = this.usePrintStyles;
			resources.ApplyResources(this.layoutControlItem5, "layoutControlItem5");
			this.layoutControlItem5.Location = new System.Drawing.Point(0, 143);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Size = new System.Drawing.Size(143, 23);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextToControlDistance = 0;
			this.layoutControlItem5.TextVisible = false;
			this.splitterItem1.AllowHotTrack = true;
			resources.ApplyResources(this.splitterItem1, "splitterItem1");
			this.splitterItem1.Location = new System.Drawing.Point(167, 0);
			this.splitterItem1.Name = "splitterItem1";
			this.splitterItem1.Size = new System.Drawing.Size(5, 328);
			resources.ApplyResources(this.layoutControlGroup2, "layoutControlGroup2");
			this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem2});
			this.layoutControlGroup2.Location = new System.Drawing.Point(172, 0);
			this.layoutControlGroup2.Name = "layoutControlGroup2";
			this.layoutControlGroup2.Size = new System.Drawing.Size(301, 328);
			this.layoutControlItem2.Control = this.pnlGridPreview;
			resources.ApplyResources(this.layoutControlItem2, "layoutControlItem2");
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(277, 284);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextToControlDistance = 0;
			this.layoutControlItem2.TextVisible = false;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "LayoutViewPrintDesigner";
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlGridPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.maximumCardColumns.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.maximumCardRows.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.printMode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.printFilterInfo.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.usePrintStyles.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.printSelectedCardsOnly.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.printCardCaption.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			this.ResumeLayout(false);
		}
		protected DevExpress.XtraGrid.GridControl GridPreview;
		public LayoutViewPrintDesigner() : base(9) {
			InitializeComponent();
			layoutControl1.UseLocalBindingContext = true;
			GridPreview = CreatePreviewGridControl(pnlGridPreview);
		}
		protected virtual GridControl CreatePreviewGridControl(Control parent) {
			return new PreviewLayoutGrid(parent);
		}
		protected override string DescriptionText {
			get { return GridLocalizer.Active.GetLocalizedString(GridStringId.PrintDesignerDescription); }
		}
		protected virtual string CaptionText {
			get { return GridLocalizer.Active.GetLocalizedString(GridStringId.PrintDesignerLayoutView); }
		}
		public override void InitComponent() {
			InitPreviewData();
			InitPrintOptions();
			InitPrintStates();
			CurrentView.AppearancePrint.Assign(EditingView.AppearancePrint);
			CurrentView.OptionsPrint.Assign(EditingView.OptionsPrint);
			InitViewStyles(usePrintStyles.Checked);
			lbCaption.Text = CaptionText;
			CurrentView.PaintStyleName = "Flat";
		}
		DataView dv1, dv2;
		protected virtual void InitPreviewData() {
			dv1 = new DataView(((DataTable)GridPreview.DataSource));
			dv2 = new DataView(((DataTable)GridPreview.DataSource));
			dv2.RowFilter = "[Product Name] = 'Tofu'";
			dv1[1]["Discontinued"] = DBNull.Value;
		}
		public virtual LayoutView CurrentView {
			get { return GridPreview.MainView as LayoutView; }
		}
		public virtual LayoutView EditingView {
			get { return EditingObject as LayoutView; }
		}
		LayoutViewPrintAppearances GetPrintAppearance(LayoutView view) {
			MethodInfo method = view.GetType().GetMethod("CreatePrintInfoCore", BindingFlags.NonPublic | BindingFlags.Instance);
			BaseViewPrintInfo pi = method.Invoke(view, new object[] { new PrintInfoArgs(view) }) as BaseViewPrintInfo;
			return pi.AppearancePrint as LayoutViewPrintAppearances;
		}
		void InitViewStyles(bool IsPrintStyles) {
			CurrentView.BeginUpdate();
			try {
				CurrentView.Appearance.Reset();
				CurrentView.FocusedRowHandle = GridControl.InvalidRowHandle;
				if(IsPrintStyles) {
					LayoutViewPrintAppearances collection = GetPrintAppearance(CurrentView);
					if(collection != null) {
						CurrentView.Appearance.Assign(collection);
						CurrentView.Appearance.CardCaption.Assign(CurrentView.AppearancePrint.CardCaption);
						CurrentView.Appearance.FocusedCardCaption.Assign(CurrentView.AppearancePrint.CardCaption);
						CurrentView.Appearance.SelectedCardCaption.Assign(CurrentView.AppearancePrint.CardCaption);
						CurrentView.Appearance.HideSelectionCardCaption.Assign(CurrentView.AppearancePrint.CardCaption);
					}
				}
				else CurrentView.Appearance.Assign(EditingView.PaintAppearance);
				CurrentView.Appearance.FocusedCardCaption.Assign(CurrentView.PaintAppearance.CardCaption);
				CurrentView.Appearance.SelectedCardCaption.Assign(CurrentView.PaintAppearance.CardCaption);
				CurrentView.Appearance.HideSelectionCardCaption.Assign(CurrentView.PaintAppearance.CardCaption);
				CurrentView.Appearance.ViewBackground.BackColor = CurrentView.Appearance.ViewBackground.BackColor2 = Color.White;
			}
			finally { CurrentView.EndUpdate(); }
		}
		string[] printFlags = Enum.GetNames(typeof(LayoutViewPrintOptionsFlags));
		int lockApply = 0;
		void InitPrintOptions() {
			lockApply--;
			for(int i = 0; i < printFlags.Length; i++) {
				CheckEdit chb = CheckEditByIndex(i);
				if(chb != null) {
					chb.Checked = SetOptions.OptionValueByString(printFlags[i], EditingView.OptionsPrint);
					if(chb.Checked)
						chb.CheckState = CheckState.Checked;
				}
			}
			SetPrintMode(EditingView.OptionsPrint.PrintMode);
			maximumCardColumns.EditValue = EditingView.OptionsPrint.MaxCardColumns;
			maximumCardRows.EditValue = EditingView.OptionsPrint.MaxCardRows;
			lockApply++;
		}
		void InitPrintStates() {
			CurrentView.OptionsView.BeginUpdate();
			try {
				printSelectedCardsOnly_CheckedChanged(printSelectedCardsOnly, EventArgs.Empty);
				printCardCaption_CheckedChanged(printCardCaption, EventArgs.Empty);
				printFilterInfo_CheckedChanged(printFilterInfo, EventArgs.Empty);
				usePrintStyles_CheckedChanged(usePrintStyles, EventArgs.Empty);
				printMode_SelectedIndexChanged(printMode, EventArgs.Empty);
				maximumCardColumns_EditValueChanged(maximumCardColumns, EventArgs.Empty);
				maximumCardRows_EditValueChanged(maximumCardRows, EventArgs.Empty);
			}
			finally { CurrentView.OptionsView.EndUpdate(); }
		}
		#region Editing
		CheckEdit CheckEditByIndex(int index) {
			foreach(object o in layoutControl1.Controls)
				if(o is CheckEdit && (int)((CheckEdit)o).Tag == index)
					return o as CheckEdit;
			return null;
		}
		private void printCardCaption_CheckedChanged(object sender, EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			bool result = chb.Checked;
			CurrentView.OptionsView.ShowCardCaption = result;
			CurrentView.OptionsPrint.PrintCardCaption = result;
			ApplyOptions();
		}
		private void printSelectedCardsOnly_CheckedChanged(object sender, EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			GridPreview.DataSource = (chb.Checked ? dv2 : dv1);
			CurrentView.OptionsPrint.PrintSelectedCardsOnly = chb.Checked;
			ApplyOptions();
		}
		void SetPrintMode(LayoutViewPrintMode lvpm) { 
			switch(lvpm) {
				case LayoutViewPrintMode.Default: printMode.SelectedIndex = 0;
					break;
				case LayoutViewPrintMode.Column: printMode.SelectedIndex = 1;
					break;
				case LayoutViewPrintMode.Row: printMode.SelectedIndex = 2;
					break;
				case LayoutViewPrintMode.MultiColumn: printMode.SelectedIndex = 3;
					break;
				case LayoutViewPrintMode.MultiRow: printMode.SelectedIndex = 4;
					break;
			}
		}
		LayoutViewPrintMode GetLayoutViewPrintModeByIndex(int index) { 
			switch(index) {
				case 1: return LayoutViewPrintMode.Column;
				case 2: return LayoutViewPrintMode.Row;
				case 3: return LayoutViewPrintMode.MultiColumn;
				case 4: return LayoutViewPrintMode.MultiRow;
			}
			return LayoutViewPrintMode.Default;
		}
		private void printMode_SelectedIndexChanged(object sender, EventArgs e) {
			LayoutViewPrintMode lvpm = GetLayoutViewPrintModeByIndex(printMode.SelectedIndex);
			CurrentView.OptionsView.ViewMode = PrintModeHelper.FromViewPrintMode(lvpm);
			CurrentView.OptionsPrint.PrintMode = lvpm;
			maximumCardColumns.Enabled = false;
			maximumCardRows.Enabled = false;
			switch(lvpm) {
				case LayoutViewPrintMode.MultiColumn:
					maximumCardColumns.Enabled = true; break;
				case LayoutViewPrintMode.MultiRow:
					maximumCardRows.Enabled = true; break;
			}
			ApplyOptions();
		}
		private void printFilterInfo_CheckedChanged(object sender, EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			CurrentView.OptionsView.ShowFilterPanelMode = chb.Checked ? ShowFilterPanelMode.ShowAlways : ShowFilterPanelMode.Never;
			CurrentView.OptionsPrint.PrintFilterInfo = chb.Checked;
			ApplyOptions();
		}
		private void usePrintStyles_CheckedChanged(object sender, EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			ApplyOptions();
			InitViewStyles(chb.Checked);
		}
		private void maximumCardColumns_EditValueChanged(object sender, EventArgs e) {
			int result = Convert.ToInt32(maximumCardColumns.Value);
			if(result < 0) {
				maximumCardColumns.EditValue = "0";
				return;
			}
			CurrentView.OptionsMultiRecordMode.MaxCardColumns = result;
			CurrentView.OptionsPrint.MaxCardColumns = result;
			ApplyOptions();
		}
		private void maximumCardRows_EditValueChanged(object sender, EventArgs e) {
			int result = Convert.ToInt32(maximumCardRows.EditValue);
			if(result < 0) {
				maximumCardRows.EditValue = "0";
				return;
			}
			CurrentView.OptionsMultiRecordMode.MaxCardRows = result;
			CurrentView.OptionsPrint.MaxCardRows = result;
			ApplyOptions();
		}
		#endregion
		#region IPrintDesigner
		public void ApplyOptions(bool setOptions) {
			if(lockApply != 0) return;
			CheckEdit chb;
			for(int i = 0; i < printFlags.Length; i++) {
				chb = CheckEditByIndex(i);
				if(chb != null) {
					if(setOptions) SetOptions.SetOptionValueByString(printFlags[i], EditingView.OptionsPrint, chb.Checked);
					SetOptions.SetOptionValueByString(printFlags[i], CurrentView.OptionsPrint, chb.Checked);
				}
			}
			EditingView.OptionsPrint.MaxCardColumns = CurrentView.OptionsPrint.MaxCardColumns;
			EditingView.OptionsPrint.MaxCardRows = CurrentView.OptionsPrint.MaxCardRows;
			EditingView.OptionsPrint.PrintMode = CurrentView.OptionsPrint.PrintMode;
			EditingView.FireChanged();
		}
		private void ApplyOptions() {
			ApplyOptions(AutoApply);
		}
		public void HideCaption() {
			lbCaption.Visible = horzSplitter.Visible = false;
		}
		#endregion
		#region UserControlSize
		public Size UserControlSize {
			get { return DevExpress.Utils.ScaleUtils.GetScaleSize(new Size(560, 345)); }
		}
		#endregion
	}
	public class PrintModeHelper {
		public static LayoutViewMode FromViewPrintMode(LayoutViewPrintMode mode) {
			switch(mode) {
				case LayoutViewPrintMode.Row:
					return LayoutViewMode.Row;
				case LayoutViewPrintMode.Column:
					return LayoutViewMode.Column;
				case LayoutViewPrintMode.MultiRow:
					return LayoutViewMode.MultiRow;
				case LayoutViewPrintMode.MultiColumn:
					return LayoutViewMode.MultiColumn;
				default:
					return LayoutViewMode.MultiColumn;
			}
		}
		public static LayoutViewPrintMode FromViewMode(LayoutViewMode mode) {
			switch(mode) {
				case LayoutViewMode.Row:
					return LayoutViewPrintMode.Row;
				case LayoutViewMode.Column:
					return LayoutViewPrintMode.Column;
				case LayoutViewMode.MultiRow:
					return LayoutViewPrintMode.MultiRow;
				case LayoutViewMode.MultiColumn:
					return LayoutViewPrintMode.MultiColumn;
				default:
					return LayoutViewPrintMode.Default;
			}
		}
	}
}
