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
using DevExpress.XtraEditors;
using DevExpress.Utils.Design;
using System.Windows.Forms;
namespace DevExpress.XtraPivotGrid.Frames {
	[ToolboxItem(false)]
	public class GroupsDesigner : DevExpress.XtraEditors.Designer.Utils.XtraFrame {
		Panel panel1;
		Panel panel2;
		Panel pnlButtonTopLeft;
		Panel pnlButtonTopRight;
		Panel pnlGroup;
		Panel pnlFields;
		SimpleButton btnAddGroup;
		SimpleButton btnRemoveGroup;
		SplitterControl splitter;
		ListBoxControl lboxGroups;
		TextEdit edGroupCaption;
		GroupControl pnlGroupedFields;
		GroupControl pnlUngroupedFields;
		Panel pnlGroupedFieldsButtons;
		ListBoxControl lboxGroupedFields;
		ListBoxControl lboxUngroupedFields;
		SimpleButton btnAddField;
		SimpleButton btnRemoveField;
		SimpleButton btnFieldUp;
		SimpleButton btnFieldDown;
		private Panel panel3;
		private Label lblGroupCaption;
		public GroupsDesigner()	{
			InitializeComponent();
		}
		protected override void InitImages() {
			base.InitImages();
			btnAddGroup.Image = DesignerImages16.Images[btnAddGroup.ImageIndex];
			btnRemoveGroup.Image = DesignerImages16.Images[btnRemoveGroup.ImageIndex];
			btnAddField.Image = DesignerImages16.Images[btnAddField.ImageIndex];
			btnRemoveField.Image = DesignerImages16.Images[btnRemoveField.ImageIndex];
			btnFieldUp.Image = DesignerImages16.Images[btnFieldUp.ImageIndex];
			btnFieldDown.Image = DesignerImages16.Images[btnFieldDown.ImageIndex];
		}
		public PivotGridControl PivotGrid { get { return EditingObject as PivotGridControl; } }
		public PivotGridFieldCollection Fields { get { return PivotGrid.Fields; } }
		public PivotGridGroupCollection Groups { get { return PivotGrid.Groups; } }
		protected PivotGridGroup ActiveGroup { get {	return lboxGroups.SelectedItem != null ? lboxGroups.SelectedItem as PivotGridGroup : null; } }
		protected override void Dispose(bool disposing){
			if(disposing){
			}
			base.Dispose( disposing );
		}
		public override void  DoInitFrame() {
 			base.DoInitFrame();
			FillGroupList();
			UpdateGroupControls();
			FillUngroupedFields();
			UpdateGroupedControls();
		}
		private void InitializeComponent() {
			this.panel1 = new System.Windows.Forms.Panel();
			this.pnlGroup = new System.Windows.Forms.Panel();
			this.lboxGroups = new DevExpress.XtraEditors.ListBoxControl();
			this.edGroupCaption = new DevExpress.XtraEditors.TextEdit();
			this.lblGroupCaption = new System.Windows.Forms.Label();
			this.pnlButtonTopLeft = new System.Windows.Forms.Panel();
			this.btnAddGroup = new DevExpress.XtraEditors.SimpleButton();
			this.btnRemoveGroup = new DevExpress.XtraEditors.SimpleButton();
			this.panel2 = new System.Windows.Forms.Panel();
			this.pnlFields = new System.Windows.Forms.Panel();
			this.pnlUngroupedFields = new DevExpress.XtraEditors.GroupControl();
			this.lboxUngroupedFields = new DevExpress.XtraEditors.ListBoxControl();
			this.pnlGroupedFieldsButtons = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.btnRemoveField = new DevExpress.XtraEditors.SimpleButton();
			this.btnAddField = new DevExpress.XtraEditors.SimpleButton();
			this.pnlGroupedFields = new DevExpress.XtraEditors.GroupControl();
			this.lboxGroupedFields = new DevExpress.XtraEditors.ListBoxControl();
			this.pnlButtonTopRight = new System.Windows.Forms.Panel();
			this.btnFieldUp = new DevExpress.XtraEditors.SimpleButton();
			this.btnFieldDown = new DevExpress.XtraEditors.SimpleButton();
			this.splitter = new DevExpress.XtraEditors.SplitterControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.panel1.SuspendLayout();
			this.pnlGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lboxGroups)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edGroupCaption.Properties)).BeginInit();
			this.pnlButtonTopLeft.SuspendLayout();
			this.panel2.SuspendLayout();
			this.pnlFields.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlUngroupedFields)).BeginInit();
			this.pnlUngroupedFields.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lboxUngroupedFields)).BeginInit();
			this.pnlGroupedFieldsButtons.SuspendLayout();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlGroupedFields)).BeginInit();
			this.pnlGroupedFields.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lboxGroupedFields)).BeginInit();
			this.pnlButtonTopRight.SuspendLayout();
			this.SuspendLayout();
			this.lbCaption.Size = new System.Drawing.Size(743, 38);
			this.pnlMain.Controls.Add(this.panel1);
			this.pnlMain.Controls.Add(this.splitter);
			this.pnlMain.Controls.Add(this.panel2);
			this.pnlMain.Location = new System.Drawing.Point(0, 46);
			this.pnlMain.Size = new System.Drawing.Size(743, 395);
			this.horzSplitter.Location = new System.Drawing.Point(0, 42);
			this.horzSplitter.Size = new System.Drawing.Size(743, 4);
			this.panel1.Controls.Add(this.pnlGroup);
			this.panel1.Controls.Add(this.pnlButtonTopLeft);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(438, 395);
			this.panel1.TabIndex = 0;
			this.pnlGroup.Controls.Add(this.lboxGroups);
			this.pnlGroup.Controls.Add(this.edGroupCaption);
			this.pnlGroup.Controls.Add(this.lblGroupCaption);
			this.pnlGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlGroup.Location = new System.Drawing.Point(0, 54);
			this.pnlGroup.Name = "pnlGroup";
			this.pnlGroup.Size = new System.Drawing.Size(438, 341);
			this.pnlGroup.TabIndex = 1;
			this.lboxGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lboxGroups.Location = new System.Drawing.Point(0, 26);
			this.lboxGroups.Name = "lboxGroups";
			this.lboxGroups.Size = new System.Drawing.Size(438, 315);
			this.lboxGroups.TabIndex = 2;
			this.edGroupCaption.Dock = System.Windows.Forms.DockStyle.Fill;
			this.edGroupCaption.Location = new System.Drawing.Point(51, 0);
			this.edGroupCaption.Name = "edGroupCaption";
			this.edGroupCaption.Size = new System.Drawing.Size(387, 20);
			this.edGroupCaption.TabIndex = 1;
			this.edGroupCaption.EditValueChanged += new System.EventHandler(this.OnGroupCaptionChanged);
			this.lblGroupCaption.AutoSize = true;
			this.lblGroupCaption.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblGroupCaption.Location = new System.Drawing.Point(0, 0);
			this.lblGroupCaption.Margin = new System.Windows.Forms.Padding(3, 4, 3, 0);
			this.lblGroupCaption.Name = "lblGroupCaption";
			this.lblGroupCaption.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this.lblGroupCaption.Size = new System.Drawing.Size(51, 17);
			this.lblGroupCaption.TabIndex = 0;
			this.lblGroupCaption.Text = "Caption: ";
			this.pnlButtonTopLeft.Controls.Add(this.btnAddGroup);
			this.pnlButtonTopLeft.Controls.Add(this.btnRemoveGroup);
			this.pnlButtonTopLeft.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlButtonTopLeft.Location = new System.Drawing.Point(0, 0);
			this.pnlButtonTopLeft.Name = "pnlButtonTopLeft";
			this.pnlButtonTopLeft.Size = new System.Drawing.Size(438, 54);
			this.pnlButtonTopLeft.TabIndex = 0;
			this.btnAddGroup.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnAddGroup.ImageIndex = 6;
			this.btnAddGroup.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnAddGroup.Location = new System.Drawing.Point(0, 4);
			this.btnAddGroup.Name = "btnAddGroup";
			this.btnAddGroup.Size = new System.Drawing.Size(30, 30);
			this.btnAddGroup.TabIndex = 0;
			this.btnAddGroup.ToolTip = "Add Group";
			this.btnAddGroup.Click += new System.EventHandler(this.OnAddGroup);
			this.btnRemoveGroup.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnRemoveGroup.ImageIndex = 7;
			this.btnRemoveGroup.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnRemoveGroup.Location = new System.Drawing.Point(36, 4);
			this.btnRemoveGroup.Name = "btnRemoveGroup";
			this.btnRemoveGroup.Size = new System.Drawing.Size(30, 30);
			this.btnRemoveGroup.TabIndex = 1;
			this.btnRemoveGroup.ToolTip = "Remove Group";
			this.btnRemoveGroup.Click += new System.EventHandler(this.OnRemoveGroup);
			this.panel2.Controls.Add(this.pnlFields);
			this.panel2.Controls.Add(this.pnlButtonTopRight);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel2.Location = new System.Drawing.Point(443, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(300, 395);
			this.panel2.TabIndex = 1;
			this.pnlFields.Controls.Add(this.pnlUngroupedFields);
			this.pnlFields.Controls.Add(this.pnlGroupedFieldsButtons);
			this.pnlFields.Controls.Add(this.pnlGroupedFields);
			this.pnlFields.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlFields.Location = new System.Drawing.Point(0, 54);
			this.pnlFields.Name = "pnlFields";
			this.pnlFields.Size = new System.Drawing.Size(300, 341);
			this.pnlFields.TabIndex = 2;
			this.pnlUngroupedFields.Controls.Add(this.lboxUngroupedFields);
			this.pnlUngroupedFields.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlUngroupedFields.Location = new System.Drawing.Point(171, 0);
			this.pnlUngroupedFields.Name = "pnlUngroupedFields";
			this.pnlUngroupedFields.Padding = new System.Windows.Forms.Padding(2);
			this.pnlUngroupedFields.Size = new System.Drawing.Size(129, 341);
			this.pnlUngroupedFields.TabIndex = 2;
			this.pnlUngroupedFields.Text = "Ungrouped fields";
			this.lboxUngroupedFields.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.lboxUngroupedFields.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lboxUngroupedFields.Location = new System.Drawing.Point(4, 23);
			this.lboxUngroupedFields.Name = "lboxUngroupedFields";
			this.lboxUngroupedFields.Size = new System.Drawing.Size(121, 314);
			this.lboxUngroupedFields.TabIndex = 1;
			this.pnlGroupedFieldsButtons.Controls.Add(this.panel3);
			this.pnlGroupedFieldsButtons.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlGroupedFieldsButtons.Location = new System.Drawing.Point(129, 0);
			this.pnlGroupedFieldsButtons.Name = "pnlGroupedFieldsButtons";
			this.pnlGroupedFieldsButtons.Size = new System.Drawing.Size(42, 341);
			this.pnlGroupedFieldsButtons.TabIndex = 1;
			this.pnlGroupedFieldsButtons.SizeChanged += new System.EventHandler(this.pnlGroupedFieldsButtons_SizeChanged);
			this.panel3.Controls.Add(this.btnRemoveField);
			this.panel3.Controls.Add(this.btnAddField);
			this.panel3.Location = new System.Drawing.Point(0, 106);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(42, 66);
			this.panel3.TabIndex = 2;
			this.btnRemoveField.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnRemoveField.ImageIndex = 11;
			this.btnRemoveField.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnRemoveField.Location = new System.Drawing.Point(6, 0);
			this.btnRemoveField.Name = "btnRemoveField";
			this.btnRemoveField.Size = new System.Drawing.Size(30, 30);
			this.btnRemoveField.TabIndex = 1;
			this.btnRemoveField.ToolTip = "Remove field";
			this.btnRemoveField.Click += new System.EventHandler(this.OnRemoveField);
			this.btnAddField.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnAddField.ImageIndex = 10;
			this.btnAddField.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnAddField.Location = new System.Drawing.Point(6, 36);
			this.btnAddField.Name = "btnAddField";
			this.btnAddField.Size = new System.Drawing.Size(30, 30);
			this.btnAddField.TabIndex = 0;
			this.btnAddField.ToolTip = "Add field";
			this.btnAddField.Click += new System.EventHandler(this.OnAddField);
			this.pnlGroupedFields.Controls.Add(this.lboxGroupedFields);
			this.pnlGroupedFields.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlGroupedFields.Location = new System.Drawing.Point(0, 0);
			this.pnlGroupedFields.Name = "pnlGroupedFields";
			this.pnlGroupedFields.Padding = new System.Windows.Forms.Padding(2);
			this.pnlGroupedFields.Size = new System.Drawing.Size(129, 341);
			this.pnlGroupedFields.TabIndex = 0;
			this.pnlGroupedFields.Text = "Grouped fields";
			this.lboxGroupedFields.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.lboxGroupedFields.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lboxGroupedFields.Location = new System.Drawing.Point(4, 23);
			this.lboxGroupedFields.Name = "lboxGroupedFields";
			this.lboxGroupedFields.Size = new System.Drawing.Size(121, 314);
			this.lboxGroupedFields.TabIndex = 0;
			this.pnlButtonTopRight.Controls.Add(this.btnFieldUp);
			this.pnlButtonTopRight.Controls.Add(this.btnFieldDown);
			this.pnlButtonTopRight.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlButtonTopRight.Location = new System.Drawing.Point(0, 0);
			this.pnlButtonTopRight.Name = "pnlButtonTopRight";
			this.pnlButtonTopRight.Size = new System.Drawing.Size(300, 54);
			this.pnlButtonTopRight.TabIndex = 0;
			this.btnFieldUp.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnFieldUp.ImageIndex = 12;
			this.btnFieldUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnFieldUp.Location = new System.Drawing.Point(0, 4);
			this.btnFieldUp.Name = "btnFieldUp";
			this.btnFieldUp.Size = new System.Drawing.Size(30, 30);
			this.btnFieldUp.TabIndex = 2;
			this.btnFieldUp.ToolTip = "Up";
			this.btnFieldUp.Click += new System.EventHandler(this.OnFieldUp);
			this.btnFieldDown.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnFieldDown.ImageIndex = 13;
			this.btnFieldDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnFieldDown.Location = new System.Drawing.Point(36, 4);
			this.btnFieldDown.Name = "btnFieldDown";
			this.btnFieldDown.Size = new System.Drawing.Size(30, 30);
			this.btnFieldDown.TabIndex = 3;
			this.btnFieldDown.ToolTip = "Down";
			this.btnFieldDown.Click += new System.EventHandler(this.OnFieldDown);
			this.splitter.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter.Location = new System.Drawing.Point(438, 0);
			this.splitter.Name = "splitter";
			this.splitter.Size = new System.Drawing.Size(5, 395);
			this.splitter.TabIndex = 3;
			this.splitter.TabStop = false;
			this.Name = "GroupsDesigner";
			this.Size = new System.Drawing.Size(743, 441);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.panel1.ResumeLayout(false);
			this.pnlGroup.ResumeLayout(false);
			this.pnlGroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.lboxGroups)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edGroupCaption.Properties)).EndInit();
			this.pnlButtonTopLeft.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.pnlFields.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlUngroupedFields)).EndInit();
			this.pnlUngroupedFields.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lboxUngroupedFields)).EndInit();
			this.pnlGroupedFieldsButtons.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlGroupedFields)).EndInit();
			this.pnlGroupedFields.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lboxGroupedFields)).EndInit();
			this.pnlButtonTopRight.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		protected override string DescriptionText { 
			get { return "You can add and delete PivotGrid groups and modify their settings."; }
		}
		protected void CreateButtonTopPanel() {
		}
		protected void CreateGroupPanel() {
			pnlGroup = new Panel();
			pnlGroup.Width = 150;
			pnlGroup.Dock = DockStyle.Fill;
			pnlGroup.Parent = panel1;
			pnlGroup.BringToFront();
			Label lblGroupCaption = new Label();
			lblGroupCaption.Text = "Caption: ";
			lblGroupCaption.Location = new Point(2, 4);
			lblGroupCaption.Parent = pnlGroup;
			lblGroupCaption.AutoSize = true;
			edGroupCaption = new TextEdit();
			edGroupCaption.Parent = pnlGroup;
			edGroupCaption.Bounds = new Rectangle(lblGroupCaption.Right + 2, 2, pnlGroup.Width - lblGroupCaption.Right - 4, 20);
			edGroupCaption.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			edGroupCaption.EditValueChanged += new EventHandler(OnGroupCaptionChanged);
			lboxGroups = new ListBoxControl();
			lboxGroups.Parent = pnlGroup;
			lboxGroups.Bounds = new Rectangle(2, 4 + edGroupCaption.Bottom, pnlGroup.Width - 4, pnlGroup.Height - edGroupCaption.Bottom - 4);
			lboxGroups.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
		}
		protected void CreateFieldsPanel() {
   		}
		protected void SetSelectedIndex(ListBoxControl listBox, int oldSelectedIndex) {
			if(oldSelectedIndex >= listBox.Items.Count)
				oldSelectedIndex = listBox.Items.Count - 1;
			if(oldSelectedIndex > -1) {
				listBox.SelectedIndex = oldSelectedIndex;
			} else listBox.SelectedIndex = 0;
		}
		protected void FillGroupList() {
			lboxGroups.SelectedIndexChanged -= new EventHandler(GroupsSelectedIndexChanged);
			int oldSelectedIndex = lboxGroups.SelectedIndex;
			lboxGroups.Items.Clear();
			for(int i = 0; i < Groups.Count; i ++) {
				lboxGroups.Items.Add(Groups[i]);
			}
			SetSelectedIndex(lboxGroups, oldSelectedIndex);
			UpdateGroupControls();
			lboxGroups.SelectedIndexChanged += new EventHandler(GroupsSelectedIndexChanged);
		}
		protected void UpdateGroupControls() {
			edGroupCaption.Enabled = ActiveGroup != null;
			btnRemoveGroup.Enabled = ActiveGroup != null;
			edGroupCaption.Text = ActiveGroup != null ? ActiveGroup.Caption : string.Empty;
			FillGroupedFields();
		}
		protected void FillUngroupedFields() {
			int oldSelectedIndex = lboxUngroupedFields.SelectedIndex;
			lboxUngroupedFields.SelectedIndexChanged -= new EventHandler(OnUngroupedFieldsSelectedIndexChanged);
			lboxUngroupedFields.Items.Clear();
			for(int i = 0; i < Fields.Count; i ++) {
				if(Fields[i].Group == null) {
					lboxUngroupedFields.Items.Add(Fields[i]);
				}
			}
			SetSelectedIndex(lboxUngroupedFields, oldSelectedIndex);
			UpdateUngroupedControls();
			lboxUngroupedFields.SelectedIndexChanged += new EventHandler(OnUngroupedFieldsSelectedIndexChanged);
		}
		protected void FillGroupedFields() {
			lboxGroupedFields.SelectedIndexChanged -= new EventHandler(OnGroupedFieldsSelectedIndexChanged);
			int oldSelectedIndex = lboxGroupedFields.SelectedIndex;
			lboxGroupedFields.Items.Clear();
			if(ActiveGroup == null) return;
			for(int i = 0; i < ActiveGroup.Count; i ++) {
				lboxGroupedFields.Items.Add(ActiveGroup[i]);
			}
			SetSelectedIndex(lboxGroupedFields, oldSelectedIndex);
			UpdateGroupedControls();
			lboxGroupedFields.SelectedIndexChanged += new EventHandler(OnGroupedFieldsSelectedIndexChanged);
		}
		void GroupsSelectedIndexChanged(object sender, EventArgs e) {
			UpdateGroupControls();
		}
		void OnGroupCaptionChanged(object sender, EventArgs e) {
			if(ActiveGroup == null) return;
			ActiveGroup.Caption = edGroupCaption.Text;
			lboxGroups.Refresh();
		}
		void UpdateUngroupedControls() {
			btnAddField.Enabled = lboxUngroupedFields.SelectedItem != null;
			if(lboxGroups.SelectedItem == null) 
				btnAddField.Enabled = false;
			if(PivotGrid != null && PivotGrid.IsOLAPDataSource)
				DisableButtons();
		}
		void DisableButtons() {
			btnAddField.Enabled = false;
			btnRemoveField.Enabled = false;
			btnAddGroup.Enabled = false;
			btnRemoveGroup.Enabled = false;
			btnFieldUp.Enabled = false;
			btnFieldDown.Enabled = false;
		}
		void OnUngroupedFieldsSelectedIndexChanged(object sender, EventArgs e) {
			UpdateUngroupedControls();
		}
		void UpdateGroupedControls() {
			btnRemoveField.Enabled = lboxGroupedFields.SelectedItem != null;
			btnFieldUp.Enabled = btnRemoveField.Enabled && lboxGroupedFields.SelectedIndex > 0;
			btnFieldDown.Enabled = btnRemoveField.Enabled && lboxGroupedFields.SelectedIndex < lboxGroupedFields.ItemCount - 1;
			if(lboxGroups.SelectedItem == null)
				btnRemoveField.Enabled = btnFieldUp.Enabled = btnFieldDown.Enabled = false;
			if(PivotGrid != null && PivotGrid.IsOLAPDataSource)
				DisableButtons();
		}
		void OnGroupedFieldsSelectedIndexChanged(object sender, EventArgs e) {
			UpdateGroupedControls();
		}
		void OnAddGroup(object sender, EventArgs e) {
			Groups.Add();
			FillGroupList();
			lboxGroups.SelectedIndex = Groups.Count - 1;
			UpdateUngroupedControls();
		}
		void OnRemoveGroup(object sender, EventArgs e) {
			if(ActiveGroup == null) return;
			Groups.Remove(ActiveGroup);
			FillGroupList();
			FillUngroupedFields();
			UpdateGroupedControls();
		}
		void OnAddField(object sender, EventArgs e) {
			if(ActiveGroup == null) return;
			PivotGridField field = lboxUngroupedFields.SelectedItem as PivotGridField;
			if(field == null) return;
			ActiveGroup.Add(field);
			FillUngroupedFields();
			FillGroupedFields();
			lboxGroups.Refresh();
		}
		void OnRemoveField(object sender, EventArgs e) {
			if(ActiveGroup == null) return;
			PivotGridField field = lboxGroupedFields.SelectedItem as PivotGridField;
			if(field == null) return;
			ActiveGroup.Remove(field);
			FillUngroupedFields();
			FillGroupedFields();
			lboxGroups.Refresh();
		}
		void OnFieldUp(object sender, EventArgs e) {
			ChangeFieldIndex(-1);
		}
		void OnFieldDown(object sender, EventArgs e) {
			ChangeFieldIndex(1);
		}
		void ChangeFieldIndex(int dx) {
			if(ActiveGroup == null) return;
			PivotGridField field = lboxGroupedFields.SelectedItem as PivotGridField;
			if(field == null) return;
			int newIndex = lboxGroupedFields.SelectedIndex + dx;
			ActiveGroup.ChangeFieldIndex(field, newIndex);
			FillGroupedFields();
			lboxGroupedFields.SelectedIndex = newIndex;
		}
		private void pnlGroupedFieldsButtons_SizeChanged(object sender, EventArgs e) {
			panel3.Top = (pnlGroupedFieldsButtons.Height - panel3.Height) / 2;	
		}
	}
}
