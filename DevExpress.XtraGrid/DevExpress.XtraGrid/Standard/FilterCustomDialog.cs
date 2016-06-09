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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Filter.Parser;
using DevExpress.XtraGrid.Localization;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.XtraGrid.Filter {
	public class FilterCustomDialog : XtraForm {
		enum AccessType {All, StringOnly, Calculable, AllArray};
		GridColumn column;
		internal BaseEdit e1, e2;
		CriteriaOperator _OldFilter = null;
		internal object[,] operators = new object[,] {{
			ExpressionOperators.Equals, ExpressionOperators.NotEquals, 
			ExpressionOperators.Like,ExpressionOperators.NotLike,
			ExpressionOperators.Greater, ExpressionOperators.GreaterOrEqual,
			ExpressionOperators.Less, ExpressionOperators.LessOrEqual,
			ExpressionOperators.Null, ExpressionOperators.NotNull,
			ExpressionOperators.NullOrEmpty, ExpressionOperators.NonNullOrEmpty}, { 
			Localizer.Active.GetLocalizedString(StringId.FilterClauseEquals),
			Localizer.Active.GetLocalizedString(StringId.FilterClauseDoesNotEqual),
			Localizer.Active.GetLocalizedString(StringId.FilterClauseLike),
			Localizer.Active.GetLocalizedString(StringId.FilterClauseNotLike),
			Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater),
			Localizer.Active.GetLocalizedString(StringId.FilterClauseGreaterOrEqual),
			Localizer.Active.GetLocalizedString(StringId.FilterClauseLess),
			Localizer.Active.GetLocalizedString(StringId.FilterClauseLessOrEqual),
			Localizer.Active.GetLocalizedString(StringId.FilterClauseIsNull),
			Localizer.Active.GetLocalizedString(StringId.FilterClauseIsNotNull),
			Localizer.Active.GetLocalizedString(StringId.FilterClauseIsNullOrEmpty),
			Localizer.Active.GetLocalizedString(StringId.FilterClauseIsNotNullOrEmpty)},
			{AccessType.All, AccessType.All, AccessType.StringOnly, AccessType.StringOnly, AccessType.Calculable, AccessType.Calculable, AccessType.Calculable, AccessType.Calculable, AccessType.AllArray, AccessType.AllArray, AccessType.StringOnly, AccessType.StringOnly}};
		protected DevExpress.XtraLayout.LayoutControl layoutControlMain;
		private DevExpress.XtraLayout.LayoutControlGroup Root;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnOK;
		private System.Windows.Forms.Label label1;
		private DevExpress.XtraLayout.LayoutControlItem liLabel;
		private DevExpress.XtraLayout.LayoutControlItem liCancel;
		private DevExpress.XtraLayout.LayoutControlItem liOk;
		private DevExpress.XtraEditors.PictureEdit peInfo;
		private DevExpress.XtraLayout.LayoutControlItem lciInfo;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
		private XtraLayout.LayoutControlGroup layoutControlGroup2;
		private XtraLayout.LayoutControlGroup layoutControlGroup3;
		protected ComboBoxEdit piSecond;
		protected ComboBoxEdit piFirst;
		protected CheckEdit rbAnd;
		protected CheckEdit rbOr;
		private TextEdit textEdit4;
		private TextEdit textEdit3;
		private XtraLayout.LayoutControlGroup Root1;
		protected XtraLayout.EmptySpaceItem emptySpaceItem5;
		protected XtraLayout.LayoutControlItem layoutControlItem8;
		protected XtraLayout.EmptySpaceItem emptySpaceItem6;
		protected XtraLayout.LayoutControlItem layoutControlItem9;
		private XtraLayout.EmptySpaceItem emptySpaceItem7;
		private XtraLayout.LayoutControlItem layoutControlItem10;
		private XtraLayout.LayoutControlItem layoutControlItem11;
		protected XtraLayout.LayoutControlItem layoutControlItem6;
		protected XtraLayout.LayoutControlItem layoutControlItem7;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
		private XtraLayout.EmptySpaceItem emptySpaceItem3;
		private System.ComponentModel.Container components = null;
		protected override void Dispose( bool disposing ) {
			if(disposing) {
				if(e1 != null) e1.InplaceType = InplaceType.Grid;
				if(e2 != null) e2.InplaceType = InplaceType.Grid;
				if(components != null) components.Dispose();
				if(peInfo.ToolTipController != null)
					peInfo.ToolTipController.Dispose();
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.layoutControlMain = new DevExpress.XtraLayout.LayoutControl();
			this.peInfo = new DevExpress.XtraEditors.PictureEdit();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.label1 = new System.Windows.Forms.Label();
			this.piSecond = new DevExpress.XtraEditors.ComboBoxEdit();
			this.piFirst = new DevExpress.XtraEditors.ComboBoxEdit();
			this.rbAnd = new DevExpress.XtraEditors.CheckEdit();
			this.rbOr = new DevExpress.XtraEditors.CheckEdit();
			this.textEdit4 = new DevExpress.XtraEditors.TextEdit();
			this.textEdit3 = new DevExpress.XtraEditors.TextEdit();
			this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
			this.liLabel = new DevExpress.XtraLayout.LayoutControlItem();
			this.liCancel = new DevExpress.XtraLayout.LayoutControlItem();
			this.liOk = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciInfo = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.Root1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem7 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlMain)).BeginInit();
			this.layoutControlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.peInfo.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.piSecond.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.piFirst.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbAnd.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbOr.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.textEdit4.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.textEdit3.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.liLabel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.liCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.liOk)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciInfo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Root1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
			this.SuspendLayout();
			this.layoutControlMain.Controls.Add(this.peInfo);
			this.layoutControlMain.Controls.Add(this.btnCancel);
			this.layoutControlMain.Controls.Add(this.btnOK);
			this.layoutControlMain.Controls.Add(this.label1);
			this.layoutControlMain.Controls.Add(this.piSecond);
			this.layoutControlMain.Controls.Add(this.piFirst);
			this.layoutControlMain.Controls.Add(this.rbAnd);
			this.layoutControlMain.Controls.Add(this.rbOr);
			this.layoutControlMain.Controls.Add(this.textEdit4);
			this.layoutControlMain.Controls.Add(this.textEdit3);
			this.layoutControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlMain.Location = new System.Drawing.Point(0, 0);
			this.layoutControlMain.Name = "layoutControlMain";
			this.layoutControlMain.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(402, 357, 803, 350);
			this.layoutControlMain.OptionsView.AutoSizeInLayoutControl = DevExpress.XtraLayout.AutoSizeModes.ResizeToMinSize;
			this.layoutControlMain.Root = this.Root;
			this.layoutControlMain.Size = new System.Drawing.Size(386, 170);
			this.layoutControlMain.TabIndex = 0;
			this.layoutControlMain.Text = "layoutControl1";
			this.peInfo.Location = new System.Drawing.Point(10, 141);
			this.peInfo.Name = "peInfo";
			this.peInfo.Properties.AllowFocused = false;
			this.peInfo.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.peInfo.Properties.Appearance.Options.UseBackColor = true;
			this.peInfo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.peInfo.Properties.NullText = " ";
			this.peInfo.Properties.PictureAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.peInfo.Properties.ShowMenu = false;
			this.peInfo.Size = new System.Drawing.Size(23, 22);
			this.peInfo.StyleController = this.layoutControlMain;
			this.peInfo.TabIndex = 2;
			this.peInfo.Click += new System.EventHandler(this.peInfo_Click);
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.AutoWidthInLayoutControl = true;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(309, 141);
			this.btnCancel.MinimumSize = new System.Drawing.Size(69, 0);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(69, 22);
			this.btnCancel.StyleController = this.layoutControlMain;
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "&Cancel";
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.AutoWidthInLayoutControl = true;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(234, 141);
			this.btnOK.MinimumSize = new System.Drawing.Size(69, 0);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(69, 22);
			this.btnOK.StyleController = this.layoutControlMain;
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "&OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.label1.Location = new System.Drawing.Point(8, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(368, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Show rows where:";
			this.piSecond.EditValue = "";
			this.piSecond.Location = new System.Drawing.Point(21, 99);
			this.piSecond.Name = "piSecond";
			this.piSecond.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.piSecond.Properties.DropDownRows = 14;
			this.piSecond.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.piSecond.Size = new System.Drawing.Size(162, 20);
			this.piSecond.StyleController = this.layoutControlMain;
			this.piSecond.TabIndex = 25;
			this.piFirst.EditValue = "";
			this.piFirst.Location = new System.Drawing.Point(21, 52);
			this.piFirst.Name = "piFirst";
			this.piFirst.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.piFirst.Properties.DropDownRows = 14;
			this.piFirst.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.piFirst.Size = new System.Drawing.Size(162, 20);
			this.piFirst.StyleController = this.layoutControlMain;
			this.piFirst.TabIndex = 2;
			this.rbAnd.Location = new System.Drawing.Point(49, 76);
			this.rbAnd.Name = "rbAnd";
			this.rbAnd.Properties.Caption = "&And";
			this.rbAnd.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbAnd.Properties.RadioGroupIndex = 0;
			this.rbAnd.Size = new System.Drawing.Size(52, 19);
			this.rbAnd.StyleController = this.layoutControlMain;
			this.rbAnd.TabIndex = 13;
			this.rbAnd.TabStop = false;
			this.rbOr.Location = new System.Drawing.Point(105, 76);
			this.rbOr.Name = "rbOr";
			this.rbOr.Properties.Caption = "&Or";
			this.rbOr.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbOr.Properties.RadioGroupIndex = 0;
			this.rbOr.Size = new System.Drawing.Size(47, 19);
			this.rbOr.StyleController = this.layoutControlMain;
			this.rbOr.TabIndex = 14;
			this.rbOr.TabStop = false;
			this.textEdit4.Location = new System.Drawing.Point(205, 52);
			this.textEdit4.Name = "textEdit4";
			this.textEdit4.Size = new System.Drawing.Size(160, 20);
			this.textEdit4.StyleController = this.layoutControlMain;
			this.textEdit4.TabIndex = 26;
			this.textEdit3.Location = new System.Drawing.Point(205, 99);
			this.textEdit3.Name = "textEdit3";
			this.textEdit3.Size = new System.Drawing.Size(160, 20);
			this.textEdit3.StyleController = this.layoutControlMain;
			this.textEdit3.TabIndex = 27;
			this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.Root.GroupBordersVisible = false;
			this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.liLabel,
			this.liCancel,
			this.liOk,
			this.lciInfo,
			this.emptySpaceItem2,
			this.Root1,
			this.emptySpaceItem3});
			this.Root.Location = new System.Drawing.Point(0, 0);
			this.Root.Name = "Root";
			this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(8, 8, 5, 5);
			this.Root.Size = new System.Drawing.Size(386, 170);
			this.Root.TextVisible = false;
			this.liLabel.Control = this.label1;
			this.liLabel.Location = new System.Drawing.Point(0, 0);
			this.liLabel.MaxSize = new System.Drawing.Size(0, 20);
			this.liLabel.MinSize = new System.Drawing.Size(24, 20);
			this.liLabel.Name = "liLabel";
			this.liLabel.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
			this.liLabel.Size = new System.Drawing.Size(370, 20);
			this.liLabel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.liLabel.TextSize = new System.Drawing.Size(0, 0);
			this.liLabel.TextVisible = false;
			this.liCancel.Control = this.btnCancel;
			this.liCancel.FillControlToClientArea = false;
			this.liCancel.Location = new System.Drawing.Point(298, 134);
			this.liCancel.Name = "liCancel";
			this.liCancel.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 0, 2, 2);
			this.liCancel.Size = new System.Drawing.Size(72, 26);
			this.liCancel.TextSize = new System.Drawing.Size(0, 0);
			this.liCancel.TextVisible = false;
			this.liOk.Control = this.btnOK;
			this.liOk.FillControlToClientArea = false;
			this.liOk.Location = new System.Drawing.Point(224, 134);
			this.liOk.Name = "liOk";
			this.liOk.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 3, 2, 2);
			this.liOk.Size = new System.Drawing.Size(74, 26);
			this.liOk.TextSize = new System.Drawing.Size(0, 0);
			this.liOk.TextVisible = false;
			this.lciInfo.Control = this.peInfo;
			this.lciInfo.Location = new System.Drawing.Point(0, 134);
			this.lciInfo.Name = "lciInfo";
			this.lciInfo.Size = new System.Drawing.Size(27, 26);
			this.lciInfo.TextSize = new System.Drawing.Size(0, 0);
			this.lciInfo.TextVisible = false;
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(27, 134);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(197, 26);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.Root1.CustomizationFormText = "Root";
			this.Root1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.Root1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.emptySpaceItem5,
			this.layoutControlItem8,
			this.emptySpaceItem6,
			this.layoutControlItem9,
			this.emptySpaceItem7,
			this.layoutControlItem10,
			this.layoutControlItem11,
			this.layoutControlItem6,
			this.layoutControlItem7,
			this.emptySpaceItem1});
			this.Root1.Location = new System.Drawing.Point(0, 20);
			this.Root1.Name = "Root1";
			this.Root1.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 5, 10);
			this.Root1.Size = new System.Drawing.Size(370, 107);
			this.Root1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.Root1.Text = "Root";
			this.emptySpaceItem5.AllowHotTrack = false;
			this.emptySpaceItem5.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem5.Location = new System.Drawing.Point(166, 47);
			this.emptySpaceItem5.Name = "emptySpaceItem5";
			this.emptySpaceItem5.Size = new System.Drawing.Size(18, 24);
			this.emptySpaceItem5.Text = "emptySpaceItem1";
			this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.Control = this.piSecond;
			this.layoutControlItem8.CustomizationFormText = "layoutControlItem4";
			this.layoutControlItem8.Location = new System.Drawing.Point(0, 47);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.Size = new System.Drawing.Size(166, 24);
			this.layoutControlItem8.Text = "layoutControlItem4";
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextVisible = false;
			this.emptySpaceItem6.AllowHotTrack = false;
			this.emptySpaceItem6.CustomizationFormText = "emptySpaceItem3";
			this.emptySpaceItem6.Location = new System.Drawing.Point(166, 0);
			this.emptySpaceItem6.Name = "emptySpaceItem6";
			this.emptySpaceItem6.Size = new System.Drawing.Size(18, 24);
			this.emptySpaceItem6.Text = "emptySpaceItem3";
			this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem9.Control = this.piFirst;
			this.layoutControlItem9.CustomizationFormText = "layoutControlItem1";
			this.layoutControlItem9.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem9.Name = "layoutControlItem9";
			this.layoutControlItem9.Size = new System.Drawing.Size(166, 24);
			this.layoutControlItem9.Text = "layoutControlItem1";
			this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem9.TextVisible = false;
			this.emptySpaceItem7.AllowHotTrack = false;
			this.emptySpaceItem7.CustomizationFormText = "emptySpaceItem4";
			this.emptySpaceItem7.Location = new System.Drawing.Point(135, 24);
			this.emptySpaceItem7.Name = "emptySpaceItem7";
			this.emptySpaceItem7.Size = new System.Drawing.Size(213, 23);
			this.emptySpaceItem7.Text = "emptySpaceItem4";
			this.emptySpaceItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem10.Control = this.rbAnd;
			this.layoutControlItem10.CustomizationFormText = "layoutControlItem2";
			this.layoutControlItem10.Location = new System.Drawing.Point(28, 24);
			this.layoutControlItem10.Name = "layoutControlItem10";
			this.layoutControlItem10.Size = new System.Drawing.Size(56, 23);
			this.layoutControlItem10.Text = "layoutControlItem2";
			this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem10.TextVisible = false;
			this.layoutControlItem11.Control = this.rbOr;
			this.layoutControlItem11.CustomizationFormText = "layoutControlItem3";
			this.layoutControlItem11.Location = new System.Drawing.Point(84, 24);
			this.layoutControlItem11.Name = "layoutControlItem11";
			this.layoutControlItem11.Size = new System.Drawing.Size(51, 23);
			this.layoutControlItem11.Text = "layoutControlItem3";
			this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem11.TextVisible = false;
			this.layoutControlItem6.Control = this.textEdit4;
			this.layoutControlItem6.CustomizationFormText = "layoutControlItem6";
			this.layoutControlItem6.Location = new System.Drawing.Point(184, 0);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Size = new System.Drawing.Size(164, 24);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem7.Control = this.textEdit3;
			this.layoutControlItem7.CustomizationFormText = "layoutControlItem7";
			this.layoutControlItem7.Location = new System.Drawing.Point(184, 47);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.Size = new System.Drawing.Size(164, 24);
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextVisible = false;
			this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup2.GroupBordersVisible = false;
			this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup2.Name = "layoutControlGroup2";
			this.layoutControlGroup2.Size = new System.Drawing.Size(180, 120);
			this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup3.GroupBordersVisible = false;
			this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup3.Name = "layoutControlGroup3";
			this.layoutControlGroup3.Size = new System.Drawing.Size(180, 120);
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 24);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(28, 23);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem3.AllowHotTrack = false;
			this.emptySpaceItem3.Location = new System.Drawing.Point(0, 127);
			this.emptySpaceItem3.MinSize = new System.Drawing.Size(104, 5);
			this.emptySpaceItem3.Name = "emptySpaceItem3";
			this.emptySpaceItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.emptySpaceItem3.Size = new System.Drawing.Size(370, 7);
			this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(386, 170);
			this.Controls.Add(this.layoutControlMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FilterCustomDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Custom AutoFilter";
			this.Load += new System.EventHandler(this.FilterCustomDialog_Load);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlMain)).EndInit();
			this.layoutControlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.peInfo.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.piSecond.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.piFirst.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbAnd.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbOr.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.textEdit4.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.textEdit3.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.liLabel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.liCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.liOk)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciInfo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Root1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		#region Events
		internal void ChangeValues() {
			bool clear = (piFirst.SelectedItem.Equals(DBNull.Value) && piSecond.SelectedItem.Equals(DBNull.Value) && !ReferenceEquals(_OldFilter, null));
			string caption = (clear ? GridLocalizer.Active.GetLocalizedString(GridStringId.CustomFilterDialogClearFilter) : GridLocalizer.Active.GetLocalizedString(GridStringId.CustomFilterDialogOkButton));
			btnOK.Enabled = IsFilterExist(piFirst, e1) || clear;
			if(btnOK.Text != caption) btnOK.Text = caption;
		}
		void SetColor(object sender) { 
		}
		bool IsImmediatePopup(object sender) { 
			if(sender is LookUpEditBase && ((LookUpEditBase)sender).Properties.ImmediatePopup) return true;
			return false;
		}
		private void OnValidated(object sender, EventArgs e) {
			SetColor(sender);
		}
		private void EditValueChanged(object sender, EventArgs e) {
			if(IsImmediatePopup(sender)) return;
			SetColor(sender);
			ChangeValues();
		}
		private void EditorKeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete && e.Control)  
				((BaseEdit)sender).EditValue = null;
		}
		bool LikeSelected(ComboBoxEdit pi) {
			return pi.SelectedItem.Equals(operators[1, 2]) || pi.SelectedItem.Equals(operators[1, 3]); 
		}
		void UpdateInfo() {
			lciInfo.Visibility = LikeSelected(piFirst) || LikeSelected(piSecond) ? XtraLayout.Utils.LayoutVisibility.Always : XtraLayout.Utils.LayoutVisibility.Never;
		}
		internal void comboBox_SelectedValueChanged(object sender, System.EventArgs e) {
			ComboBoxEdit edit = sender as ComboBoxEdit;
			UpdateInfo();
			ChangeValues();
		}
		Color GetEnabledColor() {
			return LookAndFeelHelper.GetSystemColor(this.CurrentColumn.View.ElementsLookAndFeel, SystemColors.Window);
		}
		Color GetDisabledColor() {
			if(this.CurrentColumn.View.ElementsLookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) return DevExpress.Utils.ColorUtils.FlatTabBackColor;
			return LookAndFeelHelper.GetSystemColor(this.CurrentColumn.View.ElementsLookAndFeel, SystemColors.Control);
		}
		#endregion
		#region Creators
		protected internal GridColumn CurrentColumn { 
			get {
				if(column == null || column.View == null) return null;
				return column.View.GetColumnFieldNameSortGroup(column);
			} 
		}
		protected bool IsLookUpEditors() {
			return (CurrentColumn.ColumnEdit is RepositoryItemImageComboBox || CurrentColumn.ColumnEdit is RepositoryItemLookUpEditBase || CurrentColumn.ColumnEdit is RepositoryItemPopupContainerEdit || CurrentColumn.ColumnEdit is RepositoryItemRadioGroup);
		}
		protected bool IsTextEditors() {
			return !IsDisplayTextFilter() && (CurrentColumn.RealColumnEdit is RepositoryItemTextEdit || CurrentColumn.RealColumnEdit is RepositoryItemMemoEdit || CurrentColumn.RealColumnEdit is RepositoryItemMemoExEdit);
		}
		protected virtual BaseEdit CreateEditorEx(BaseEdit e) {
			BaseEdit ret;
			if(CurrentColumn.RealColumnEdit is DevExpress.XtraEditors.Repository.RepositoryItemProgressBar)
				ret = new SpinEdit();
			else if(CurrentColumn.RealColumnEdit is DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit ||
				DevExpress.XtraGrid.FilterEditor.GridCriteriaHelper.IsRichTextEditItem(CurrentColumn.RealColumnEdit)) {
				ret = new MemoExEdit();
				((MemoExEdit)ret).Properties.ShowIcon = false;
			}
			else {
				if( IsDisplayTextFilter()) {
					RepositoryItem ri = DevExpress.XtraGrid.FilterEditor.GridCriteriaHelper.CreateDisplayTextEditor(CurrentColumn);
					ret = ri.CreateEditor();
					ret.Properties.Assign(ri);
				}
				else {
					ret = CurrentColumn.RealColumnEdit.CreateEditor();
					ret.Properties.Assign(CurrentColumn.RealColumnEdit);
					ret.Properties.ResetEvents();
				}
			}
			if(ret is ComboBoxEdit)
				((ComboBoxEdit)ret).Properties.UseCtrlScroll = false;
			if(ret is ImageComboBoxEdit)
				((ImageComboBoxEdit)ret).Properties.UseCtrlScroll = false;
			if(ret is LookUpEditBase)
				((LookUpEditBase)ret).Properties.ImmediatePopup = false;
			TextEdit edit = ret as TextEdit;
			if(edit != null)
				edit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
			return ret;
		} 
		private void CreateEditor(ref BaseEdit e, ComboBoxEdit pi, int index) {
			e = CreateEditorEx(e);
			e.Name = "ValueEditor" + index.ToString();
			e.Properties.BorderStyle = BorderStyles.Default;
			e.Properties.AllowFocused = true;
			e.Properties.AutoHeight = false;
			e.Properties.ReadOnly = false;
			e.Enabled = true;
			e.Location = new System.Drawing.Point(pi.Left + pi.Width + 20, pi.Top);
			e.Size = new System.Drawing.Size(Root1.Width - e.Left - pi.Left, piFirst.Height);
			e.TabIndex = index == 1 ? 3 : 26;
			e.EditValue = null;
			e.EditValueChanged += new EventHandler(EditValueChanged);
			pi.SelectedValueChanged += new System.EventHandler(comboBox_SelectedValueChanged);
			e.Validated += new EventHandler(OnValidated);
			pi.Tag = index;
			e.KeyDown += new KeyEventHandler(EditorKeyDown);
			SetColor(e);
			comboBox_SelectedValueChanged(pi, null);
			e.StyleController = layoutControlMain;
			layoutControlMain.BeginUpdate();
			layoutControlMain.Controls.Add(e);
			if(index == 1) {
				layoutControlMain.Controls.Remove(layoutControlItem6.Control);
				layoutControlItem6.Control = e;
			}
			if(index == 2) {
				layoutControlMain.Controls.Remove(layoutControlItem7.Control);
				layoutControlItem7.Control = e;
			}
			layoutControlMain.EndUpdate();
			if(e is ButtonEdit)
				((ButtonEdit)e).Properties.TextEditStyle = TextEditStyles.Standard;
			if(e is TextEdit)
				((TextEdit)e).Properties.NullValuePrompt = GridLocalizer.Active.GetLocalizedString(GridStringId.CustomFilterDialogEmptyValue);
		}
		protected Type GetColumnType() {
			if(IsDisplayTextFilter()) return typeof(string);
			return CurrentColumn.ColumnType;
		}
		protected bool IsDisplayTextFilter() {
			return CurrentColumn.GetFilterMode() == ColumnFilterMode.DisplayText;
		}
		bool IsLikeType { get { return GetColumnType().Equals(typeof(System.String)); } } 
		bool IsObject { get { return GetColumnType().Equals(typeof(System.Boolean)) || GetColumnType().Equals(typeof(System.Object)); } }
		bool IsArray { get { return GetColumnType().Equals(typeof(System.Byte[])); } }
		bool IsArrayColumnEdit { get { return CurrentColumn.ColumnEditIsSparkLine; } }
		private void AddPickItems(ComboBoxEdit pi) {
			pi.Properties.Items.Clear();
			pi.Properties.Items.Add(DBNull.Value);
			bool skip = false;
			string description;
			for(int i = 0; i <= operators.GetUpperBound(1); i++) {
				AccessType accessType = (AccessType)operators[2, i];
				if(!IsLikeType) skip = accessType == AccessType.StringOnly;
				if(IsObject) skip = accessType != AccessType.All && accessType != AccessType.AllArray;
				if(IsArray || IsArrayColumnEdit) skip = accessType != AccessType.AllArray;
				if(!skip) {
					description = operators[1, i].ToString();
					object pii = description; 
					pi.Properties.Items.Add(pii);
				}
			}
		}
		#endregion
		#region Init
		public FilterCustomDialog(GridColumn col) : this(col, true) {}
		public FilterCustomDialog(GridColumn col, bool setFilterValue) {
			column = col;
			SetRightToLeft(col != null && col.View != null && col.View.IsRightToLeft);
			InitializeComponent();
			btnOK.Enabled = false;
			this.Icon = null;
			_OldFilter = CriteriaOperator.Clone(column.FilterInfo.FilterCriteria);
			AddPickItems(piFirst);
			piFirst.SelectedIndex = IsLikeType && IsTextEditors() ? 3 : 1;
			rbAnd.Checked = true; 
			AddPickItems(piSecond);
			piSecond.SelectedItem = DBNull.Value;
			CreateEditor(ref e1, piFirst, 1);
			CreateEditor(ref e2, piSecond, 2);
			Root1.Text = GetSpaceString(column.GetTextCaption());
			if(setFilterValue) SetFilter();
			SetLookAndFeel(CurrentColumn.View.ElementsLookAndFeel);
			this.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.CustomFilterDialogFormCaption);
			this.label1.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.CustomFilterDialogCaption);
			this.rbOr.Properties.Caption = GridLocalizer.Active.GetLocalizedString(GridStringId.CustomFilterDialogRadioOr);
			this.rbAnd.Properties.Caption = GridLocalizer.Active.GetLocalizedString(GridStringId.CustomFilterDialogRadioAnd);
			this.btnOK.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.CustomFilterDialogOkButton);
			this.btnCancel.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.CustomFilterDialogCancelButton);
			piFirst.Properties.NullValuePrompt = piSecond.Properties.NullValuePrompt = GridLocalizer.Active.GetLocalizedString(GridStringId.CustomFilterDialogEmptyOperator);
			peInfo.Image = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraGrid.Images.info-16x16.png", typeof(FilterCustomDialog).Assembly);
			peInfo.ToolTipController = new ToolTipController();
			peInfo.ToolTipController.ToolTipType = ToolTipType.SuperTip;
			string toolTip = GridLocalizer.Active.GetLocalizedString(GridStringId.CustomFilterDialogHint);
			toolTip = toolTip.Replace("#", "\r\n");
			peInfo.ToolTip = toolTip;
			UpdateInfo();
			UpdateEditorMenuManager(Controls);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			this.Height += layoutControlMain.GetPreferredSize(layoutControlMain.Size).Height - layoutControlMain.Height + 2;
		}
		protected internal void UpdateEditorMenuManager(Control.ControlCollection controls) {
			foreach(Control c in controls) {
				BaseEdit edit = c as BaseEdit;
				if(edit != null) edit.MenuManager = CurrentColumn.View.GridControl.MenuManager;
				else UpdateEditorMenuManager(c.Controls);
			}
		}
		string GetSpaceString(string ret) {
			ret = ret.Replace("\n", " ");
			ret = ret.Replace("\r", "");
			ret = ret.Replace("\t", " ");
			return ret;
		}
		internal void SetLookAndFeel(DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel) {
			if(column.View.GridControl != null && column.View.GridControl.FormsUseDefaultLookAndFeel) return;
			this.LookAndFeel.Assign(lookAndFeel);
			layoutControlMain.LookAndFeel.Assign(lookAndFeel);
			foreach(Control c in this.Controls) SetLookAndFeelControl(c, lookAndFeel);
			foreach(Control c in this.layoutControlMain.Controls) SetLookAndFeelControl(c, lookAndFeel); 
		}
		private void SetLookAndFeelControl(Control c, DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel) {
			BaseControl bc = c as BaseControl;
			if(bc != null) bc.LookAndFeel.Assign(lookAndFeel);
		}
		internal void SetFilter() {
			CriteriaOperator opa = _OldFilter;
			if(ReferenceEquals(opa, null))
				return;
			GroupOperator gropa = opa as GroupOperator;
			IEnumerator<CriteriaOperator> ops;
			if(ReferenceEquals(gropa, null)){
				ops = Enumerable.Repeat(opa, 1).GetEnumerator();
			}else{
				rbAnd.Checked = (gropa.OperatorType == GroupOperatorType.And);
				rbOr.Checked = (gropa.OperatorType == GroupOperatorType.Or);
				ops = gropa.Operands.GetEnumerator();
			}
			if(AssignFirstFilter(ops))
				AssignSecondFilter(ops);
		}
		bool SetValue(IEnumerator<CriteriaOperator> ops, ComboBoxEdit pi, BaseEdit e) {
			while(ops.MoveNext()) {
				if(SetValue(ops.Current, pi, e))
					return true;
			}
			return false;
		}
		bool AssignFirstFilter(IEnumerator<CriteriaOperator> ops) {
			return SetValue(ops, piFirst, e1);
		}
		bool AssignSecondFilter(IEnumerator<CriteriaOperator> ops) {
			return SetValue(ops, piSecond, e2);
		}
		string GetLikeOperandForFunctionOperator(string val, FunctionOperator fnOpa) {
			OperandValue ov = fnOpa.Operands[1] as OperandValue;
			if(ReferenceEquals(null, ov)) return string.Empty;
			return string.Format(val, ov.Value);	
		}
		bool ExtractOpa(CriteriaOperator op, out ExpressionOperators eopa, out CriteriaOperator operand) {
			UnaryOperator notOpa = op as UnaryOperator;
			bool isNot = (!ReferenceEquals(notOpa, null)) && notOpa.OperatorType == UnaryOperatorType.Not;
			if(isNot)
				op = notOpa.Operand;
			UnaryOperator unOpa = op as UnaryOperator;
			if(!ReferenceEquals(unOpa, null)) {
				operand = null;
				switch(unOpa.OperatorType) {
					case UnaryOperatorType.IsNull:
						eopa = isNot ? ExpressionOperators.NotNull : ExpressionOperators.Null;
						return true;
				}
			}
			FunctionOperator fnOpa = op as FunctionOperator;
			if(!ReferenceEquals(fnOpa, null)) {
				switch(fnOpa.OperatorType) {
					case FunctionOperatorType.IsNullOrEmpty:
						operand = null;
						eopa = isNot ? ExpressionOperators.NonNullOrEmpty : ExpressionOperators.NullOrEmpty;
						return true;
					case FunctionOperatorType.Custom:
						if(LikeCustomFunction.IsBinaryCompatibleLikeFunction(fnOpa)) {
							operand = fnOpa.Operands[2];
							eopa = isNot ? ExpressionOperators.NotLike : ExpressionOperators.Like;
							return true;
						}
						break;
					case FunctionOperatorType.StartsWith:
						operand = GetLikeOperandForFunctionOperator("{0}%", fnOpa);
						eopa = ExpressionOperators.Like;
						return true;
					case FunctionOperatorType.EndsWith:
						operand = GetLikeOperandForFunctionOperator("%{0}", fnOpa);
						eopa = ExpressionOperators.Like;
						return true;
					case FunctionOperatorType.Contains:
						operand = GetLikeOperandForFunctionOperator("%{0}%", fnOpa);
						eopa = isNot ? ExpressionOperators.NotLike : ExpressionOperators.Like;
						return true;
				}
			}
			BinaryOperator binOpa = op as BinaryOperator;
			if(!ReferenceEquals(binOpa, null)) {
				operand = binOpa.RightOperand;
				if(isNot) {
					switch(binOpa.OperatorType) {
						case BinaryOperatorType.Equal:
							eopa = ExpressionOperators.NotEquals;
							return true;
						case BinaryOperatorType.NotEqual:
							eopa = ExpressionOperators.Equals;
							return true;
						case BinaryOperatorType.Less:
							eopa = ExpressionOperators.GreaterOrEqual;
							return true;
						case BinaryOperatorType.LessOrEqual:
							eopa = ExpressionOperators.Greater;
							return true;
						case BinaryOperatorType.Greater:
							eopa = ExpressionOperators.LessOrEqual;
							return true;
						case BinaryOperatorType.GreaterOrEqual:
							eopa = ExpressionOperators.Less;
							return true;
#pragma warning disable 618
						case BinaryOperatorType.Like:
#pragma warning restore 618
							eopa = ExpressionOperators.NotLike;
							return true;
					}
				} else {
					switch(binOpa.OperatorType) {
						case BinaryOperatorType.Equal:
							eopa = ExpressionOperators.Equals;
							return true;
						case BinaryOperatorType.NotEqual:
							eopa = ExpressionOperators.NotEquals;
							return true;
						case BinaryOperatorType.Less:
							eopa = ExpressionOperators.Less;
							return true;
						case BinaryOperatorType.LessOrEqual:
							eopa = ExpressionOperators.LessOrEqual;
							return true;
						case BinaryOperatorType.Greater:
							eopa = ExpressionOperators.Greater;
							return true;
						case BinaryOperatorType.GreaterOrEqual:
							eopa = ExpressionOperators.GreaterOrEqual;
							return true;
#pragma warning disable 618
						case BinaryOperatorType.Like:
#pragma warning restore 618
							eopa = ExpressionOperators.Like;
							return true;
					}
				}
			}
			operand = null;
			eopa = ExpressionOperators.Null;
			return false;
		}
		bool SetValue(CriteriaOperator opa, ComboBoxEdit pi, BaseEdit e) {
			ExpressionOperators eopa;
			CriteriaOperator operand;
			if(!ExtractOpa(opa, out eopa, out operand))
				return false;
			pi.SelectedItem = DescriptionByExOperator(eopa);
			return SetEditorValue(pi, e, operand);
		}
		protected virtual bool SetEditorValue(ComboBoxEdit pi, BaseEdit e, CriteriaOperator operand) {
			OperandValue val = operand as OperandValue;
			if(BlankSelected(pi)) return true;
			if(!ReferenceEquals(val, null)) {
				e.EditValue = val.Value;
				return true;
			}
			return false;
		}
		#endregion
		#region CreateFilter
		private ExpressionOperators ExOperatorByDescription(object o) {
			string s = o.ToString();
			int i = 0;
			for(i = 0; i <= operators.GetUpperBound(1); i++) {
				if(operators[1, i].ToString() == s) 
					break;
			}
			return (ExpressionOperators)operators[0, i];
		}
		internal string DescriptionByExOperator(ExpressionOperators o) {
			int i = 0;
			for(i = 0; i <= operators.GetUpperBound(1); i++) {
				if(operators[0, i].Equals(o)) 
					break;
			}
			return operators[1, i].ToString();
		}
		private int OperationNumber(object exo) {
			int result = -1;
			for(int i = 0; i <= operators.GetUpperBound(1); i++) {
				if(operators[1, i].Equals(exo)) {
					result = i;
					break;
				} 
			}
			return result;
		}
		protected bool BlankSelected(ComboBoxEdit pi) {
			return pi.SelectedItem.Equals(operators[1, 8]) || pi.SelectedItem.Equals(operators[1, 9]) ||
				pi.SelectedItem.Equals(operators[1, 10]) || pi.SelectedItem.Equals(operators[1, 11]); 
		}
		protected virtual bool IsFilterExist(ComboBoxEdit pi, BaseEdit e) {
			bool result = true;
			if(CurrentColumn == null) return false;
			if(pi.SelectedItem == null || pi.SelectedItem.Equals(DBNull.Value) ||
				CurrentColumn.FieldName == "" || (!BlankSelected(pi) && e.EditValue == null)) 
				result = false;
			return result;
		}
		protected CriteriaOperator GetFilterCriterion(ComboBoxEdit pi, BaseEdit e) {
			return GetFilterCriterion(pi, e, false, string.Empty);
		}
		CriteriaOperator GetRightOperand(BaseEdit e, bool IsField, string FieldName) {
			if(IsField) {
				return new OperandProperty(FieldName);
			} else {
				Type columnType = GetColumnType();
				object value = DevExpress.Data.Helpers.FilterHelper.CorrectFilterValueType(columnType, e.EditValue);
				return new OperandValue(value);
			}
		}
		protected CriteriaOperator GetFilterCriterion(ComboBoxEdit pi, BaseEdit e, bool IsField, string FieldName) {
			if(!IsFilterExist(pi, e))
				return null;
			OperandProperty leftOperand = new OperandProperty(CurrentColumn.FieldName);
			int OpNum = OperationNumber(pi.SelectedItem);
			if(OpNum < 0)
				return null;
			ExpressionOperators opa = (ExpressionOperators)(operators[0, OpNum]);
			switch(opa) {
				case ExpressionOperators.Null:
					return leftOperand.IsNull();
				case ExpressionOperators.Equals:
					return leftOperand == GetRightOperand(e, IsField, FieldName);
				case ExpressionOperators.Greater:
					return leftOperand > GetRightOperand(e, IsField, FieldName);
				case ExpressionOperators.GreaterOrEqual:
					return leftOperand >= GetRightOperand(e, IsField, FieldName);
				case ExpressionOperators.Less:
					return leftOperand < GetRightOperand(e, IsField, FieldName);
				case ExpressionOperators.LessOrEqual:
					return leftOperand <= GetRightOperand(e, IsField, FieldName);
				case ExpressionOperators.Like:
					return LikeCustomFunction.Create(leftOperand, PatchLikeOperand(GetRightOperand(e, IsField, FieldName)));
				case ExpressionOperators.NotNull:
					return leftOperand.IsNotNull();
				case ExpressionOperators.NotEquals:
					return leftOperand != GetRightOperand(e, IsField, FieldName);
				case ExpressionOperators.NotLike:
					return !LikeCustomFunction.Create(leftOperand, PatchLikeOperand(GetRightOperand(e, IsField, FieldName)));
				case ExpressionOperators.NullOrEmpty:
					return new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, leftOperand);
				case ExpressionOperators.NonNullOrEmpty:
					return !new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, leftOperand);
				default:
					return null;
			}
		}
		bool useAsteriskAsWildcard = true;
		public virtual bool UseAsteriskAsWildcard {
			get { return useAsteriskAsWildcard; }
			set { useAsteriskAsWildcard = value; }
		}
		CriteriaOperator PatchLikeOperand(CriteriaOperator op) {
			if(UseAsteriskAsWildcard) {
				OperandValue ov = op as OperandValue;
				if(!ReferenceEquals(ov, null)) {
					if(ov.Value != null) {
						string msk = ov.Value.ToString();
						msk = msk.Replace('*', '%');
						msk = msk.Replace('?', '_');
						return new OperandValue(msk);
					}
				}
			}
			return op;
		}
		internal GroupOperatorType GroupOperatorType {
			get {
				return rbAnd.Checked ? GroupOperatorType.And : GroupOperatorType.Or;
			}
		}
		protected virtual CriteriaOperator GetFiltersCriterion() {
			return GroupOperator.Combine(GroupOperatorType,
				GetFilterCriterion(piFirst, e1),
				GetFilterCriterion(piSecond, e2));
		}
		[Obsolete("Use GetFiltersCriterion instead")]
		protected virtual string FilterConditions() {
			CriteriaOperator op = GetFiltersCriterion();
			if(ReferenceEquals(op, null))
				return string.Empty;
			return op.ToString();
		}
		private void CreateFilter(CriteriaOperator filterCriteria) {
			try {
				if(!ReferenceEquals(filterCriteria, null)) {
					ColumnFilterInfo info = new ColumnFilterInfo(filterCriteria);
					info.SetFilterKind(ColumnFilterKind.Predefined);
					column.FilterInfo = info;
				}
				else
					column.FilterInfo = new ColumnFilterInfo();
			} 
			catch (Exception ex) {
				XtraMessageBox.Show(this.CurrentColumn.View.ElementsLookAndFeel, FindForm(), ex.Message, GridLocalizer.Active.GetLocalizedString(GridStringId.WindowErrorCaption));
				CreateFilter(_OldFilter);
			}
		}
		private void btnOK_Click(object sender, System.EventArgs e) {
			CriteriaOperator filter = GetFiltersCriterion();
			CreateFilter(filter);
		}
		#endregion
		private void peInfo_Click(object sender, EventArgs e) {
			ToolTipControllerShowEventArgs args = peInfo.ToolTipController.CreateShowArgs();
			args.ToolTip = peInfo.ToolTip;
			peInfo.ToolTipController.ShowHint(args);
		}
		private void FilterCustomDialog_Load(object sender, EventArgs e) {
			if(WindowsFormsSettings.TouchUIMode == TouchUIMode.True) this.Size = new Size(Root.Size.Width + 30, Root.Size.Height + 40);
			else return;
		}
		internal void SetRightToLeft(bool isRightToLeft) {
			RightToLeft = isRightToLeft ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
		}
	}
}
