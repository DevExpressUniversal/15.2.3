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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraLayout;
namespace DevExpress.Tutorials.Controls {
	[ToolboxItem(false)]
	public class AlignmentControl : DevExpress.XtraEditors.XtraUserControl {
		private System.ComponentModel.Container components = null;
		ContentAlignment fAlignment = ContentAlignment.MiddleCenter;
		public event EventHandler AlignmentChanged;
		public AlignmentControl() {
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer, true);
			this.TabStop = true;
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		[DefaultValue(ContentAlignment.MiddleCenter)]
		public ContentAlignment Alignment {
			get { return fAlignment; }
			set { fAlignment = value; 
				Info.CalcLocation();
			}
		}
		AlignmentControlViewInfo fInfo;
		public AlignmentControlViewInfo Info {
			get {
				if(fInfo == null) fInfo = new AlignmentControlViewInfo(this);
				return fInfo;
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			Info.Draw(new GraphicsCache(e), LookAndFeel);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			Info.CalcBounds();
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			Info.CalcHotTrack(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			Info.HotTrackIndex = -1;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			Info.CalcPressed(e);
		}
		public void RaiseAlignmentChanged(ContentAlignment alignment) {
			Alignment = alignment;
			if(AlignmentChanged != null)
				AlignmentChanged(this, EventArgs.Empty);
		}
		public class AlignmentControlViewInfo {
			AlignmentControl control;
			ArrayList args = new ArrayList();
			int fHotTrackIndex = -1;
			ContentAlignment[] alignments = new ContentAlignment[] { ContentAlignment.TopLeft, ContentAlignment.TopCenter, ContentAlignment.TopRight, ContentAlignment.MiddleLeft, ContentAlignment.MiddleCenter, ContentAlignment.MiddleRight, ContentAlignment.BottomLeft, ContentAlignment.BottomCenter, ContentAlignment.BottomRight};
			public AlignmentControlViewInfo(AlignmentControl control) {
				this.control = control;
				for(int i = 0; i < alignments.Length; i++) 
					args.Add(new StyleObjectInfoArgs());
				CalcBounds();
				CalcLocation();
			}
			StyleObjectInfoArgs ObjectInfoByID(int index) {
				return args[index] as StyleObjectInfoArgs;
			}
			public void Draw(GraphicsCache cache, UserLookAndFeel lookAndFeel) {
				for(int i = 0; i < args.Count; i++) {
					ObjectInfoByID(i).Cache = cache;
					lookAndFeel.Painter.Button.DrawObject(ObjectInfoByID(i));
				}
			}
			int dx = 8;
			int ElementWidht { get { return (control.Width - dx * 2) / 3; }}
			int ElementHeight { get { return (control.Height - dx * 2) / 3; }}
			public void CalcBounds() {
				for(int i = 0; i < 9; i++) 
					ObjectInfoByID(i).Bounds = new Rectangle(
						(ElementWidht + dx) * (i % 3), 
						(ElementHeight + dx) * (i / 3), 
						ElementWidht, ElementHeight);
				control.Refresh();
			}
			int LocationIndex {
				get {
					if(control == null) return -1;
					for(int i = 0; i < alignments.Length; i++)
						if(control.Alignment.Equals(alignments[i])) return i;
					return -1;
				}
			}
			public void CalcLocation() {
				int index = LocationIndex;
				for(int i = 0; i < args.Count; i++) 
					ObjectInfoByID(i).State = index == i ? ObjectState.Pressed : ObjectState.Normal; 
				if(HotTrackIndex > -1 && HotTrackIndex != index) ObjectInfoByID(HotTrackIndex).State = ObjectState.Hot;
				if(control == null)
					for(int i = 0; i < args.Count; i++) ObjectInfoByID(i).State = ObjectState.Disabled;
				control.Refresh();
			}
			public int HotTrackIndex {
				get { return fHotTrackIndex; }
				set {
					if(fHotTrackIndex == value) return;
					fHotTrackIndex = value;
					CalcLocation();
				}
			}
			int IndexByPoint(Point p) {
				for(int i = 0; i < args.Count; i++)
					if(ObjectInfoByID(i).Bounds.Contains(p)) return i;
				return -1;
			}
			public void CalcHotTrack(MouseEventArgs e) {
				Point p = new Point(e.X, e.Y);
				HotTrackIndex = IndexByPoint(p);
			}
			public void CalcPressed(MouseEventArgs e) {
				Point p = new Point(e.X, e.Y);
				int i = IndexByPoint(p);
				if(i > -1) {
					control.RaiseAlignmentChanged(alignments[i]);
					CalcLocation();
				}
			}
		} 
	}
	public class XtraFontDialog : XtraForm {
		private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
		private DevExpress.XtraEditors.SimpleButton sbCancel;
		private DevExpress.XtraEditors.SimpleButton sbOk;
		private DevExpress.XtraEditors.ComboBoxEdit cbeFont;
		private DevExpress.XtraEditors.ImageListBoxControl ilbcFont;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraEditors.CheckedListBoxControl clbStyle;
		private DevExpress.XtraEditors.TextEdit teFontStyle;
		private DevExpress.XtraEditors.LabelControl labelControl2;
		private System.ComponentModel.Container components = null;
		private DevExpress.XtraEditors.SpinEdit seFontSize;
		private DevExpress.XtraEditors.LabelControl labelControl3;
		private DevExpress.XtraEditors.ListBoxControl lbcFontSize;
		private DevExpress.XtraEditors.LabelControl labelControl4;
		private DevExpress.XtraEditors.LabelControl lcPreview;
		Font fCurrentFont = null;
		Font fResultFont = null;
		public XtraFontDialog(Font font) {
			InitializeComponent();
			InitFont();
			InitSize();
			if(LookAndFeel.GetTouchUI()) {
				Scale(new SizeF(1.4f + LookAndFeel.GetTouchScaleFactor() / 10.0f, 1.4f + LookAndFeel.GetTouchScaleFactor() / 10.0f));
			}
			ilbcFont.SelectedIndex = -1;
			CurrentFont = font;
			UpdatePreview();
			cbeFont.GotFocus += new EventHandler(cbeFont_GotFocus);
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					ResultFontDispose();
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		Font CurrentFont {
			get { return fCurrentFont; }
			set {
				fCurrentFont = value;
				InitFontValues();
			}
		}
		void InitFontValues() {
			ilbcFont.Enabled = cbeFont.Enabled = teFontStyle.Enabled = clbStyle.Enabled =
				seFontSize.Enabled = lbcFontSize.Enabled = CurrentFont != null;
			if(CurrentFont == null) return;
			ilbcFont.SelectedValue = CurrentFont.Name;
			foreach(CheckedListBoxItem item in clbStyle.Items) 
				item.CheckState = (CurrentFont.Style.ToString().IndexOf(item.Value.ToString()) > -1) ? CheckState.Checked : CheckState.Unchecked;
			InitStyleString();
			seFontSize.Value = Convert.ToInt32(CurrentFont.Size);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
			this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
			this.lcPreview = new DevExpress.XtraEditors.LabelControl();
			this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.lbcFontSize = new DevExpress.XtraEditors.ListBoxControl();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.seFontSize = new DevExpress.XtraEditors.SpinEdit();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.teFontStyle = new DevExpress.XtraEditors.TextEdit();
			this.clbStyle = new DevExpress.XtraEditors.CheckedListBoxControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.ilbcFont = new DevExpress.XtraEditors.ImageListBoxControl();
			this.cbeFont = new DevExpress.XtraEditors.ComboBoxEdit();
			this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
			this.sbOk = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
			this.xtraTabControl1.SuspendLayout();
			this.xtraTabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbcFontSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.seFontSize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.teFontStyle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clbStyle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ilbcFont)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbeFont.Properties)).BeginInit();
			this.SuspendLayout();
			this.xtraTabControl1.Location = new System.Drawing.Point(8, 9);
			this.xtraTabControl1.Name = "xtraTabControl1";
			this.xtraTabControl1.SelectedTabPage = this.xtraTabPage2;
			this.xtraTabControl1.Size = new System.Drawing.Size(376, 303);
			this.xtraTabControl1.TabIndex = 0;
			this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
																							this.xtraTabPage2});
			this.xtraTabControl1.TabStop = false;
			this.xtraTabControl1.Text = "xtcFont";
			this.xtraTabPage2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.lcPreview,
																					   this.labelControl4,
																					   this.lbcFontSize,
																					   this.labelControl3,
																					   this.seFontSize,
																					   this.labelControl2,
																					   this.teFontStyle,
																					   this.clbStyle,
																					   this.labelControl1,
																					   this.ilbcFont,
																					   this.cbeFont});
			this.xtraTabPage2.Name = "xtraTabPage2";
			this.xtraTabPage2.Size = new System.Drawing.Size(367, 273);
			this.xtraTabPage2.Text = "Font";
			this.lcPreview.Appearance.BackColor = System.Drawing.Color.White;
			this.lcPreview.Appearance.Options.UseBackColor = true;
			this.lcPreview.Appearance.Options.UseTextOptions = true;
			this.lcPreview.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.lcPreview.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.lcPreview.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.lcPreview.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.lcPreview.LineVisible = true;
			this.lcPreview.Location = new System.Drawing.Point(8, 216);
			this.lcPreview.Name = "lcPreview";
			this.lcPreview.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lcPreview.Size = new System.Drawing.Size(352, 40);
			this.lcPreview.TabIndex = 25;
			this.lcPreview.Text = "Font Preview Text";
			this.labelControl4.LineVisible = true;
			this.labelControl4.Location = new System.Drawing.Point(10, 192);
			this.labelControl4.Name = "labelControl4";
			this.labelControl4.Size = new System.Drawing.Size(350, 13);
			this.labelControl4.TabIndex = 24;
			this.labelControl4.Text = "Preview";
			this.lbcFontSize.Location = new System.Drawing.Point(280, 46);
			this.lbcFontSize.Name = "lbcFontSize";
			this.lbcFontSize.Size = new System.Drawing.Size(80, 130);
			this.lbcFontSize.TabIndex = 5;
			this.lbcFontSize.SelectedIndexChanged += new System.EventHandler(this.lbcFontSize_SelectedIndexChanged);
			this.labelControl3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
			this.labelControl3.Location = new System.Drawing.Point(282, 8);
			this.labelControl3.Name = "labelControl3";
			this.labelControl3.Size = new System.Drawing.Size(48, 13);
			this.labelControl3.TabIndex = 22;
			this.labelControl3.Text = "Font Size:";
			this.seFontSize.EditValue = new System.Decimal(new int[] {
																		 8,
																		 0,
																		 0,
																		 0});
			this.seFontSize.Location = new System.Drawing.Point(280, 24);
			this.seFontSize.Name = "seFontSize";
			this.seFontSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
																											   new DevExpress.XtraEditors.Controls.EditorButton()});
			this.seFontSize.Properties.IsFloatValue = false;
			this.seFontSize.Properties.Mask.EditMask = "N00";
			this.seFontSize.Properties.MaxValue = new System.Decimal(new int[] {
																				   100,
																				   0,
																				   0,
																				   0});
			this.seFontSize.Properties.MinValue = new System.Decimal(new int[] {
																				   6,
																				   0,
																				   0,
																				   0});
			this.seFontSize.Size = new System.Drawing.Size(80, 20);
			this.seFontSize.TabIndex = 4;
			this.seFontSize.EditValueChanged += new System.EventHandler(this.seFontSize_EditValueChanged);
			this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
			this.labelControl2.Location = new System.Drawing.Point(170, 8);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new System.Drawing.Size(53, 13);
			this.labelControl2.TabIndex = 20;
			this.labelControl2.Text = "Font Style:";
			this.teFontStyle.Location = new System.Drawing.Point(168, 24);
			this.teFontStyle.Name = "teFontStyle";
			this.teFontStyle.Properties.ReadOnly = true;
			this.teFontStyle.Size = new System.Drawing.Size(104, 20);
			this.teFontStyle.TabIndex = 2;
			this.teFontStyle.TabStop = false;
			this.clbStyle.CheckOnClick = true;
			this.clbStyle.Items.AddRange(new DevExpress.XtraEditors.Controls.CheckedListBoxItem[] {
																									  new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Bold"),
																									  new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Italic"),
																									  new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Strikeout"),
																									  new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Underline")});
			this.clbStyle.Location = new System.Drawing.Point(168, 46);
			this.clbStyle.Name = "clbStyle";
			this.clbStyle.Size = new System.Drawing.Size(104, 130);
			this.clbStyle.TabIndex = 3;
			this.clbStyle.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.clbStyle_ItemCheck);
			this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
			this.labelControl1.Location = new System.Drawing.Point(10, 8);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(26, 13);
			this.labelControl1.TabIndex = 2;
			this.labelControl1.Text = "Font:";
			this.ilbcFont.Location = new System.Drawing.Point(8, 46);
			this.ilbcFont.Name = "ilbcFont";
			this.ilbcFont.Size = new System.Drawing.Size(152, 130);
			this.ilbcFont.TabIndex = 1;
			this.ilbcFont.SelectedIndexChanged += new System.EventHandler(this.ilbcFont_SelectedIndexChanged);
			this.cbeFont.Location = new System.Drawing.Point(8, 24);
			this.cbeFont.Name = "cbeFont";
			this.cbeFont.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
			this.cbeFont.Size = new System.Drawing.Size(152, 20);
			this.cbeFont.TabIndex = 0;
			this.cbeFont.SelectedValueChanged += new System.EventHandler(this.cbeFont_SelectedValueChanged);
			this.sbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.sbCancel.Location = new System.Drawing.Point(296, 320);
			this.sbCancel.Name = "sbCancel";
			this.sbCancel.Size = new System.Drawing.Size(88, 24);
			this.sbCancel.TabIndex = 2;
			this.sbCancel.Text = "&Cancel";
			this.sbOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.sbOk.Location = new System.Drawing.Point(200, 320);
			this.sbOk.Name = "sbOk";
			this.sbOk.Size = new System.Drawing.Size(88, 24);
			this.sbOk.TabIndex = 1;
			this.sbOk.Text = "&OK";
			this.AcceptButton = this.sbOk;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.CancelButton = this.sbCancel;
			this.ClientSize = new System.Drawing.Size(394, 352);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.sbOk,
																		  this.sbCancel,
																		  this.xtraTabControl1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmFontDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Font";
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
			this.xtraTabControl1.ResumeLayout(false);
			this.xtraTabPage2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lbcFontSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.seFontSize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.teFontStyle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clbStyle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ilbcFont)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbeFont.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		void InitStyleString() {
			teFontStyle.Text = GetFontStyleByValues(clbStyle).ToString();
			UpdatePreview();
		}
		FontStyle GetFontStyleByValues(CheckedListBoxControl clb) {
			FontStyle ret = new FontStyle();
			if(clb.GetItemChecked(0)) ret |= FontStyle.Bold;
			if(clb.GetItemChecked(1)) ret |= FontStyle.Italic;
			if(clb.GetItemChecked(2)) ret |= FontStyle.Strikeout;
			if(clb.GetItemChecked(3)) ret |= FontStyle.Underline;
			return ret;
		}
		void InitFont() {
			DevExpress.Tutorials.TutorialHelper.InitFont(ilbcFont);
			cbeFont.Properties.Items.AddRange(ilbcFont.Items);
		}
		void InitSize() {
			for(int i = 8; i < 30; i+=2)
				lbcFontSize.Items.Add(i); 
			lbcFontSize.Items.Add(36);
			lbcFontSize.Items.Add(48);
			lbcFontSize.Items.Add(72);
		}
		private void ilbcFont_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(ilbcFont.SelectedItem == null) return;
			cbeFont.SelectedItem = ilbcFont.SelectedItem;
			UpdatePreview();
		}
		private void cbeFont_SelectedValueChanged(object sender, System.EventArgs e) {
			ilbcFont.SelectedItem = cbeFont.SelectedItem;
		}
		void cbeFont_GotFocus(object sender, EventArgs e) {
			cbeFont.SelectAll();
		}
		private void clbStyle_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			InitStyleString();
		}
		private void seFontSize_EditValueChanged(object sender, System.EventArgs e) {
			int val = (int)seFontSize.Value;
			if(lbcFontSize.Items.IndexOf(val) < 0) 
				lbcFontSize.SelectedIndex = -1;
			else
				lbcFontSize.SelectedValue = val;
			UpdatePreview();
		}
		private void lbcFontSize_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(lbcFontSize.SelectedIndex == -1) return;
			seFontSize.Value = (int)lbcFontSize.SelectedItem;
		}
		string ResultFontName {
			get {
				if(ilbcFont.SelectedItem != null) return ilbcFont.SelectedItem.ToString();
				return DevExpress.Utils.AppearanceObject.DefaultFont.Name;
			}
		}
		public Font ResultFont {
			get {
				if(fResultFont == null)
					fResultFont = new Font(ResultFontName, (int)seFontSize.Value, GetFontStyleByValues(clbStyle));
				return fResultFont;
			}
		}
		void UpdatePreview() {
			ResultFontDispose();
			lcPreview.Font = ResultFont;
		}
		void ResultFontDispose() {
			if(fResultFont != null) {
				fResultFont.Dispose();
				fResultFont = null;
			}
		}
	}
	public class frmAbout : XtraForm {
		private System.Windows.Forms.RichTextBox richTextBox1;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private DevExpress.XtraEditors.PanelControl panelControl2;
		private DevExpress.XtraEditors.SimpleButton simpleButton1;
		private System.ComponentModel.Container components = null;
		public frmAbout(string aboutFileName, string text, Icon icon) {
			InitializeComponent();
			if(text != null) this.Text = text;
			if(icon != null) this.Icon = icon;
			LoadAboutFile(aboutFileName);
		}
		void LoadAboutFile(string fileName) {
			try {
				richTextBox1.LoadFile(fileName);
			} catch {}
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
			this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			this.panelControl2.SuspendLayout();
			this.SuspendLayout();
			this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Location = new System.Drawing.Point(2, 2);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(564, 364);
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = "";
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.simpleButton1});
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelControl1.Location = new System.Drawing.Point(0, 368);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(568, 46);
			this.panelControl1.TabIndex = 1;
			this.panelControl1.Text = "&Close";
			this.panelControl2.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.richTextBox1});
			this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelControl2.Name = "panelControl2";
			this.panelControl2.Size = new System.Drawing.Size(568, 368);
			this.panelControl2.TabIndex = 2;
			this.panelControl2.Text = "panelControl2";
			this.simpleButton1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.simpleButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.simpleButton1.Location = new System.Drawing.Point(456, 8);
			this.simpleButton1.Name = "simpleButton1";
			this.simpleButton1.Size = new System.Drawing.Size(104, 30);
			this.simpleButton1.TabIndex = 0;
			this.simpleButton1.Text = "&Close";
			this.AcceptButton = this.simpleButton1;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.simpleButton1;
			this.ClientSize = new System.Drawing.Size(568, 414);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.panelControl2,
																		  this.panelControl1});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmAbout";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About";
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			this.panelControl2.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
	}
	public class RichTextBoxEx : RichTextBox, IMouseWheelSupport {
		int lockSmartMouse = 0;
		protected sealed override void OnMouseWheel(MouseEventArgs ev) {
			if(lockSmartMouse == 0 && XtraForm.IsAllowSmartMouseWheel(this, ev)) {
				return;
			}
			OnMouseWheelCore(ev);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			try {
				this.lockSmartMouse++;
				if(IsHandleCreated) {
					DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(Handle,
						DevExpress.Utils.Drawing.Helpers.MSG.WM_MOUSEWHEEL,
						new IntPtr((e.Delta << 16) + 0),
						new IntPtr(e.X + (e.Y << 16)));
				}
				else {
					OnMouseWheelCore(e);
				}
			}
			finally {
				this.lockSmartMouse--;
			}
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs e) {
			base.OnMouseWheel(e);
		}
	}
	[ToolboxItem(false)]
	public class OverviewLabel : LabelControl {
		protected override void OnPaint(PaintEventArgs e) {
			if(IsDesignMode) return;
			GraphicsCache gc = new GraphicsCache(e.Graphics);
			gc.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			GetFitAppearance(gc).DrawString(gc, this.Text, this.ClientRectangle);
		}
		LabelControlAppearanceObject GetFitAppearance(GraphicsCache gc) {
			LabelControlAppearanceObject obj = new LabelControlAppearanceObject();
			obj.Assign(ViewInfo.PaintAppearance);
			while(obj.Font.Size > 12) {
				SizeF s = obj.CalcTextSize(gc, this.Text, this.ClientRectangle.Width);
				if(s.Width < this.ClientRectangle.Width) break;
				obj.Font = new Font(obj.Font.FontFamily, obj.Font.Size - 1);
			}
			return obj;
		}
	}
	public class OverviewButton : DevExpress.Utils.Frames.PictureButton {
		public OverviewButton(Control parent, Image normal, Image hover, Image pressed, string processStartLink) : base(parent, normal, hover, pressed, processStartLink) { }
		protected override void SetActive(Image image, bool active) {
			this.Image = image;
			if(this.Parent == null) return;
			LayoutControl lc = this.Parent.Parent as LayoutControl;
			if(lc != null) {
				LayoutControlItem item = lc.GetItemByControl(this.Parent);
				if(item.TextVisible)
					item.AppearanceItemCaption.Font = new Font(item.AppearanceItemCaption.Font, active ? FontStyle.Underline : FontStyle.Regular);
			}
		}
	}
}
