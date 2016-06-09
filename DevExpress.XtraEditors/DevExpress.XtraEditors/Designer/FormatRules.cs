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
using DevExpress.Utils.Frames;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraGrid;
using System.Linq;
using System.Collections.Generic;
using DevExpress.XtraEditors.Helpers;
using DevExpress.Utils;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public abstract class FormatRulesBaseFrame : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		[ToolboxItem(false)]
		public class PreviewGroupControl : GroupControl {
			FormatConditionRuleBase rule;
			public PreviewGroupControl() {
				Text = "Preview";
			}
			public void SetAppearance(FormatConditionRuleBase rule) {
				this.rule = rule;
				Invalidate();
			}
			protected override void OnPaint(PaintEventArgs e) {
				base.OnPaint(e);
				IFormatConditionDrawPreview preview = rule;
				if(preview == null) return;
				Rectangle rect = ViewInfo.ClientBounds;
				rect.Inflate(-10, -10);
				var args = new FormatConditionDrawPreviewArgs(e.Graphics, rect, ViewInfo.Appearance, "Text");
				preview.Draw(args);
			}
			public int GetBestHeight() {
				return ViewInfo.CaptionBounds.Height + 50;
			}
		}
		public class FormatRuleUI {
			FormatRuleBase rule;
			public FormatRuleUI(FormatRuleBase rule) { this.rule = rule; }
			public override string ToString() {
				string displayText = rule.ToString();
				int index = displayText.IndexOf(':');
				if(index > 0 && index < displayText.Length - 2)
					displayText = "<b>" + displayText.Insert(index, "</b>");
				return displayText;
			}
		}
		private DevExpress.XtraEditors.ListBoxControl FormatItemList;
		protected DevExpress.XtraEditors.SimpleButton btAdd;
		protected DevExpress.XtraEditors.SimpleButton btRemove;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		public SimpleButton btnUp;
		public SimpleButton btnDown;
		private PanelControl pnlConvert;
		private PanelControl pnlLabel;
		private LabelControl lbConvert;
		private PanelControl pnlButton;
		private SimpleButton sbConvert;
		private System.ComponentModel.Container components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormatRulesBaseFrame));
			this.btAdd = new DevExpress.XtraEditors.SimpleButton();
			this.FormatItemList = new DevExpress.XtraEditors.ListBoxControl();
			this.btRemove = new DevExpress.XtraEditors.SimpleButton();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.btnUp = new DevExpress.XtraEditors.SimpleButton();
			this.btnDown = new DevExpress.XtraEditors.SimpleButton();
			this.pnlConvert = new DevExpress.XtraEditors.PanelControl();
			this.pnlLabel = new DevExpress.XtraEditors.PanelControl();
			this.pnlButton = new DevExpress.XtraEditors.PanelControl();
			this.sbConvert = new DevExpress.XtraEditors.SimpleButton();
			this.lbConvert = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.FormatItemList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlConvert)).BeginInit();
			this.pnlConvert.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlLabel)).BeginInit();
			this.pnlLabel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlButton)).BeginInit();
			this.pnlButton.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.splMain, "splMain");
			resources.ApplyResources(this.pgMain, "pgMain");
			this.pnlControl.Controls.Add(this.btnUp);
			this.pnlControl.Controls.Add(this.btRemove);
			this.pnlControl.Controls.Add(this.btnDown);
			this.pnlControl.Controls.Add(this.btAdd);
			resources.ApplyResources(this.pnlControl, "pnlControl");
			this.lbCaption.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lbCaption.Appearance.Font")));
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.pnlMain.Controls.Add(this.groupControl1);
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			this.btAdd.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btAdd, "btAdd");
			this.btAdd.Name = "btAdd";
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			this.FormatItemList.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
			this.FormatItemList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.FormatItemList, "FormatItemList");
			this.FormatItemList.ItemHeight = 16;
			this.FormatItemList.Name = "FormatItemList";
			this.FormatItemList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.FormatItemList.SelectedIndexChanged += new System.EventHandler(this.FormatItemList_SelectedIndexChanged);
			this.btRemove.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btRemove.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btRemove, "btRemove");
			this.btRemove.Name = "btRemove";
			this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
			this.groupControl1.Controls.Add(this.FormatItemList);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Name = "groupControl1";
			this.btnUp.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.btnUp, "btnUp");
			this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnUp.Name = "btnUp";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			this.btnDown.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.btnDown, "btnDown");
			this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnDown.Name = "btnDown";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			this.pnlConvert.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlConvert.Controls.Add(this.pnlLabel);
			resources.ApplyResources(this.pnlConvert, "pnlConvert");
			this.pnlConvert.Name = "pnlConvert";
			this.pnlConvert.Resize += new System.EventHandler(this.pnlConvert_Resize);
			this.pnlLabel.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("pnlLabel.Appearance.BackColor")));
			this.pnlLabel.Appearance.Options.UseBackColor = true;
			this.pnlLabel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlLabel.Controls.Add(this.pnlButton);
			this.pnlLabel.Controls.Add(this.lbConvert);
			resources.ApplyResources(this.pnlLabel, "pnlLabel");
			this.pnlLabel.Name = "pnlLabel";
			this.pnlButton.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlButton.Controls.Add(this.sbConvert);
			resources.ApplyResources(this.pnlButton, "pnlButton");
			this.pnlButton.Name = "pnlButton";
			resources.ApplyResources(this.sbConvert, "sbConvert");
			this.sbConvert.Name = "sbConvert";
			this.sbConvert.Click += new System.EventHandler(this.sbConvert_Click);
			resources.ApplyResources(this.lbConvert, "lbConvert");
			this.lbConvert.Name = "lbConvert";
			this.Controls.Add(this.pnlConvert);
			this.Name = "FormatRulesBaseFrame";
			resources.ApplyResources(this, "$this");
			this.Controls.SetChildIndex(this.pnlConvert, 0);
			this.Controls.SetChildIndex(this.lbCaption, 0);
			this.Controls.SetChildIndex(this.horzSplitter, 0);
			this.Controls.SetChildIndex(this.pnlControl, 0);
			this.Controls.SetChildIndex(this.pnlMain, 0);
			this.Controls.SetChildIndex(this.splMain, 0);
			this.Controls.SetChildIndex(this.pgMain, 0);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.FormatItemList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlConvert)).EndInit();
			this.pnlConvert.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlLabel)).EndInit();
			this.pnlLabel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlButton)).EndInit();
			this.pnlButton.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		#region Init & Ctor
		XtraTabControl tc;
		public FormatRulesBaseFrame() : base(7) {
			InitializeComponent();
			lbConvert.AutoSizeMode = LabelAutoSizeMode.Vertical;
			pgMain.BringToFront();
		}
		protected abstract IFormatRuleCollection FormatConditions { get; }
		protected abstract FormatRuleBase CreateFormatCondition();
		protected virtual IFormatRuleCollection SampleFormatConditions { get { return null; } }
		PreviewGroupControl preview = null;
		DevExpress.XtraEditors.SplitterControl spHorz = null;
		public override void InitComponent() {
			FillData();
			CreateTabControl();
			lbCaption.Text = Properties.Resources.StyleFormatRulesCaption;
			btnDown.Visible = btnUp.Visible = ArrowsVisible;
			if(AllowConvert)
				StartConvert();
		}
		protected virtual bool AllowConvert { get { return false; } }
		protected virtual bool ArrowsVisible { get { return false; } }
		protected override void InitImages() {
			base.InitImages();
			btAdd.Image = DesignerImages16.Images[DesignerImages16PlusIndex];
			btRemove.Image = DesignerImages16.Images[DesignerImages16MinusIndex];
			btnUp.Image = DesignerImages16.Images[DesignerImages16UpIndex];
			btnDown.Image = DesignerImages16.Images[DesignerImages16DownIndex];
		}
		protected virtual void CreateTabControl() {
			tc = FramesUtils.CreateTabProperty(this, new Control[] {pgMain, null}, new string[] {Properties.Resources.PropertiesCaption, Properties.Resources.RuleCaption});
			spHorz = new DevExpress.XtraEditors.SplitterControl();
			preview = new PreviewGroupControl();
			spHorz.Dock = DockStyle.Bottom;
			spHorz.MinSize = preview.GetBestHeight();
			preview.Height = spHorz.MinSize + 5;
			preview.Dock = DockStyle.Bottom;
			spHorz.Height = 4;
			tc.TabPages[1].Controls.Add(spHorz);
			tc.TabPages[1].Controls.Add(preview);
			tc.SelectedPageChanged += new TabPageChangedEventHandler(changeTabPage);
		}
		protected override void OnPropertyGridPropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			base.OnPropertyGridPropertyValueChanged(sender, e);
			if(e.ChangedItem == null) return;
			FormatItemList.Refresh();
		}
		private void FillData() {
			FillData(0);		
		}
		bool lockUpdate = false;
		private void FillData(int indx) {
			FormatItemList.BeginUpdate();
			lockUpdate = true;
			try {
				FormatItemList.Items.Clear();
				if(FormatConditions == null) return;
				for(int i = 0; i < FormatConditions.Count; i++) {
					FormatItemList.Items.Add(new FormatRuleUI(FormatConditions.GetRule(i))); 
				}
				FramesUtils.SelectItem(FormatItemList, indx);
			} finally {
				FormatItemList.EndUpdate();
				lockUpdate = false;
				EnableArrowButtons();
			}
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion
		#region Editing
		int selectedTabPage = 0;
		private void changeTabPage(object sender, TabPageChangedEventArgs e) {
			XtraTabControl tc = sender as XtraTabControl;
			selectedTabPage = tc.SelectedTabPageIndex;
			RefreshPropertyGrid();
			e.Page.Controls.Add(pgMain);
			pgMain.BringToFront();
			pgMain.Refresh();
		}
		protected override void RefreshPropertyGrid() {
			base.RefreshPropertyGrid();
			if(preview == null || selectedTabPage == 0) return;
			if(SelectedObjects == null || SelectedObjects.Length > 1) {
				ShowPreviewAppearance(false);
				preview.SetAppearance(null);
				return;
			}
			FormatConditionRuleBase rule = GetNestedPropertyGridObject(SelectedObjects[0]) as FormatConditionRuleBase;
			if(rule != null) {
				preview.SetAppearance(rule);
				ShowPreviewAppearance(true);
			}
		}
		void ShowPreviewAppearance(bool show) {
			if(preview != null && spHorz != null)
				preview.Visible = spHorz.Visible = show;
		}
		protected override object[] SelectedObjects {
			get {
				ArrayList al = new ArrayList();
				if(FormatItemList.SelectedIndex < 0 || FormatConditions == null) return null;
				for(int i = 0; i < FormatItemList.SelectedIndices.Count; i++) 
					al.Add(FormatConditions.GetRule(FormatItemList.SelectedIndices[i]));
				return al.ToArray();
			}
		}
		protected override object[] GetPropertyGridSampleObjects(object obj) {
			FormatRuleBase formatCondition = obj as FormatRuleBase;
			if(SampleObjects == null || formatCondition == null) return null;
			int index = FormatConditions.IndexOfRule(formatCondition);
			if(index < 0) return null;
			object[] samples = new object[SampleObjects.Length];
			for(int i = 0; i < samples.Length; i ++)
				samples[i] = FormatConditions.GetRule(index);
			return samples;
		}
		protected override object GetNestedPropertyGridObject(object obj) {
			FormatRuleBase formatCondition = obj as FormatRuleBase;
			if(formatCondition != null && selectedTabPage > 0) 
				return formatCondition.Rule;
			return obj; 
		}
		private void FormatItemList_SelectedIndexChanged(object sender, System.EventArgs e) {
			RefreshPropertyGrid();
			EnableArrowButtons();
		}
		protected virtual void AddFormatCondition() {
			FormatConditions.AddRule(CreateFormatCondition());
			if(SampleFormatConditions != null)
				FormatConditions.AddRule(CreateFormatCondition());
		}
		protected virtual void RemoveFormatCondition(int index) {
			FormatConditions.RemoveAt(index);
			if(SampleFormatConditions != null)
				SampleFormatConditions.RemoveAt(index);
		}
		private void btAdd_Click(object sender, System.EventArgs e) {
			if(FormatConditions == null) return;
			FormatItemList.BeginUpdate();
			try {
				AddFormatCondition();
				int n = FormatItemList.Items.Count;
				int i = FormatItemList.Items.Add(new FormatRuleUI(FormatConditions.GetRule(n)));
				FormatItemList.SelectedIndex = i;
			} finally {
				FormatItemList.EndUpdate();
			}
		}
		private void btRemove_Click(object sender, System.EventArgs e) {
			if(FormatConditions == null) return;
			int j = 0;
			for(int i = FormatItemList.SelectedIndices.Count - 1; i >= 0 ; i--) {
				j = FormatItemList.SelectedIndices[i];
				RemoveFormatCondition(j);
			}
			FillData();
			FramesUtils.SelectItem(FormatItemList, j);
		}
		protected override void pgMain_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e) {
			if(preview != null)
				preview.Invalidate();
		}
		#endregion
		bool AllowMoveCollectionItems {
			get {
				if(SelectedObjects == null) return false;
				return SelectedObjects.Length == 1;
			}
		}
		protected virtual void EnableArrowButtons() {
			if(lockUpdate) return;
			btRemove.Enabled = (SelectedObjects != null && SelectedObjects.Length > 0);
			btnDown.Enabled = AllowMoveCollectionItems && !FormatItemList.SelectedIndices.Contains(FormatItemList.ItemCount - 1);
			btnUp.Enabled = AllowMoveCollectionItems && !FormatItemList.SelectedIndices.Contains(0);
			groupControl1.Text = string.Format(Properties.Resources.FormatRulesCaption, FormatItemList.Items.Count);
		}
		void MoveRecord(int j) {
			if(FormatItemList.SelectedIndices.Count != 1) return;
			int indx = FormatItemList.SelectedIndex;
			ArrayList collection = new ArrayList();
			for(int i = 0; i < FormatConditions.Count; i++) {
				int pos = i;
				if(i == indx + j) pos = i - j;
				if(i == indx) pos = i + j;
				collection.Add(FormatConditions.GetRule(pos));
			}
			FormatConditions.Clear();
			for(int i = 0; i < collection.Count; i++)
				FormatConditions.AddRule((FormatRuleBase)collection[i]);
			FillData(indx + j);
		}
		private void btnUp_Click(object sender, EventArgs e) {
			MoveRecord(-1);
		}
		private void btnDown_Click(object sender, EventArgs e) {
			MoveRecord(1);
		}
		private void sbConvert_Click(object sender, EventArgs e) {
			DoConvert();
			SetConvertEnabled(false);
		}
		protected virtual void DoConvert() {
			FillData();
			btnDown.Visible = btnUp.Visible = ArrowsVisible;
		}
		private void pnlConvert_Resize(object sender, EventArgs e) {
			pnlConvert.Height = lbConvert.Height + pnlLabel.Padding.Bottom * 3 + pnlButton.Height;
		}
		void SetConvertEnabled(bool allowConvert) {
			pnlConvert.Visible = allowConvert;
			pnlControl.Enabled = groupControl1.Enabled = tc.Enabled = splMain.Enabled = !allowConvert;
		}
		protected void StartConvert() {
			SetConvertEnabled(true);
		}
	}
}
