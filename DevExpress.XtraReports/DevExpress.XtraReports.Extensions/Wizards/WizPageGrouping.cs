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
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Wizards;
namespace DevExpress.XtraReports.Design {
	[ToolboxItem(false)]
	public class WizPageGrouping : DevExpress.Utils.InteriorWizardPage {
		#region static
		static string GetFieldsString(ObjectNameCollection fields) {
			int count = fields.Count;
			if(count <= 0)
				return String.Empty;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(fields[0].DisplayName);
			for(int i = 1; i < count; i++) {
				sb.Append(", ");
				sb.Append(fields[i].DisplayName);
			}
			return sb.ToString();
		}
		#endregion
		#region inner classes
		internal class WizGroupingPreviewPainter : DevExpress.XtraPrinting.Native.PagePreviewPainterBase {
			WizPageGrouping page;
			Brush groupingBrush = new SolidBrush(Color.Blue);
			Brush selectedBrush = new SolidBrush(Color.LightYellow);
			Font defaultFont = new Font(FontFamily.GenericSansSerif, 10);
			Font selectedFont = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
			StringFormat stringFormat = StringFormat.GenericDefault.Clone() as StringFormat;
			public WizGroupingPreviewPainter(WizPageGrouping page) {
				this.page = page;
			}
			int DrawFields(Graphics gr, bool selected, string str, int x, int y, int w, int h) {
				Font font = selected ? selectedFont : defaultFont;
				int width = w - x - 2 * padding;
				int height = (int)gr.MeasureString(str, font, width, stringFormat).Height;
				Rectangle rect = new Rectangle(x, y, width, h < 0 ? height : h);
				if (selected)
					gr.FillRectangle(selectedBrush, rect);
				gr.DrawString(str, font, h < 0 ? groupingBrush : blackBrush, rect, stringFormat);
				gr.DrawRectangle(borderPen, rect);
				if (selected) {
					rect.Inflate(1, 1);
					gr.DrawRectangle(borderPen, rect);
				}
				return height;
			}
			protected override void DrawImage(Graphics gr, int w, int h) {
				base.DrawImage(gr, w, h);
				page.groupingFieldsRects.Clear();
				int hOffset = 2 * padding;
				int vOffset = 2 * padding;
				Region oldClip = gr.Clip;
				try {
					Rectangle clipRect = new Rectangle(padding, padding, w - 2 * padding, h - 2 * padding);
					gr.Clip = new Region(clipRect);
					int count = page.groupingFieldsSet.Count;
					for (int i = 0; i < count; i++) {
						Rectangle rect = new Rectangle(hOffset, vOffset, w - hOffset - 2 * padding, 0);
						string groupFieldsString = WizPageGrouping.GetFieldsString(page.groupingFieldsSet[i]);
						int height = DrawFields(gr, i == page.selectedGroupFieldIndex, groupFieldsString, hOffset, vOffset, w, -1);
						rect.Height = height;
						page.groupingFieldsRects.Add(rect);
						vOffset += height + shadowWidth;
						hOffset += padding;
					}
					DrawFields(gr, false, page.remainingFieldsString, hOffset, vOffset, w, h - vOffset - 2 * padding);
				}
				finally {
					gr.Clip = oldClip;
					oldClip.Dispose();
				}
			}
			public override void Dispose() {
				base.Dispose();
				groupingBrush.Dispose();
				selectedBrush.Dispose();
				defaultFont.Dispose();
				selectedFont.Dispose();
				stringFormat.Dispose();
			}
		}
		#endregion
		private System.Windows.Forms.PictureBox picPreview;
		private System.Windows.Forms.ListView lvAvailableFields;
		private DevExpress.XtraEditors.SimpleButton btnRemove;
		private DevExpress.XtraEditors.SimpleButton btnAdd;
		private System.Windows.Forms.Label lblPriority;
		private DevExpress.XtraEditors.SimpleButton btnIncreasePriority;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.ComponentModel.IContainer components = null;
		WizGroupingPreviewPainter previewPainter;
		ObjectNameCollectionsSet groupingFieldsSet = new ObjectNameCollectionsSet();
		ObjectNameCollection remainingFields = new ObjectNameCollection();
		ArrayList groupingFieldsRects = new ArrayList();
		string remainingFieldsString;
		private System.Windows.Forms.ImageList imageList;
		private DevExpress.XtraEditors.SimpleButton btnAppend;
		int selectedGroupFieldIndex;
		private DevExpress.XtraEditors.SimpleButton btnDecreasePriority;
		StandardReportWizard wizard;
		public WizPageGrouping(XRWizardRunnerBase runner)
			: this() {
			this.wizard = (StandardReportWizard)runner.Wizard;
		}
		WizPageGrouping() {
			this.previewPainter = new WizGroupingPreviewPainter(this);
			InitializeComponent();
			btnRemove.Image = ResourceImageHelper.CreateBitmapFromResources("Images.MoveLeft.gif", typeof(LocalResFinder));
			btnAdd.Image = ResourceImageHelper.CreateBitmapFromResources("Images.MoveRight.gif", typeof(LocalResFinder));
			btnAppend.Image = ResourceImageHelper.CreateBitmapFromResources("Images.AddToRight.gif", typeof(LocalResFinder));
			btnIncreasePriority.Image = ResourceImageHelper.CreateBitmapFromResources("Images.MoveUp.gif", typeof(LocalResFinder));
			btnDecreasePriority.Image = ResourceImageHelper.CreateBitmapFromResources("Images.MoveDown.gif", typeof(LocalResFinder));
			PickManager.FillDataSourceImageList(imageList);
			headerPicture.Image = ResourceImageHelper.CreateBitmapFromResources("Images.WizTopGrouping.gif", typeof(LocalResFinder));
			picPreview.AllowDrop = true;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) 
					components.Dispose();
				previewPainter.Dispose();
			}
			base.Dispose(disposing);
		}
		void UpdateRemainingFieldsString() {
			remainingFieldsString = GetFieldsString(remainingFields);
		}
		void UpdateButtons() {
			btnAdd.Enabled = lvAvailableFields.Items.Count > 0;
			btnRemove.Enabled = groupingFieldsSet.Count > 0;
			btnIncreasePriority.Enabled = groupingFieldsSet.Count > 0 && selectedGroupFieldIndex > 0;
			btnDecreasePriority.Enabled = groupingFieldsSet.Count > 0 && selectedGroupFieldIndex + 1 < groupingFieldsSet.Count;
			btnAppend.Enabled = lvAvailableFields.Items.Count > 0 && groupingFieldsSet.Count > 0;
		}
		void UpdateImage() {
			UpdateRemainingFieldsString();
			previewPainter.GenerateImage(picPreview);
			UpdateButtons();
		}
		#region Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageGrouping));
			this.picPreview = new System.Windows.Forms.PictureBox();
			this.lvAvailableFields = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.lblPriority = new System.Windows.Forms.Label();
			this.btnIncreasePriority = new DevExpress.XtraEditors.SimpleButton();
			this.btnAppend = new DevExpress.XtraEditors.SimpleButton();
			this.btnDecreasePriority = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.titleLabel, "titleLabel");
			resources.ApplyResources(this.subtitleLabel, "subtitleLabel");
			this.picPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			resources.ApplyResources(this.picPreview, "picPreview");
			this.picPreview.Name = "picPreview";
			this.picPreview.TabStop = false;
			this.picPreview.DoubleClick += new System.EventHandler(this.picPreview_DoubleClick);
			this.picPreview.DragOver += new System.Windows.Forms.DragEventHandler(this.picPreview_DragOver);
			this.picPreview.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picPreview_MouseMove);
			this.picPreview.DragDrop += new System.Windows.Forms.DragEventHandler(this.picPreview_DragDrop);
			this.picPreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picPreview_MouseDown);
			this.picPreview.DragEnter += new System.Windows.Forms.DragEventHandler(this.picPreview_DragEnter);
			this.lvAvailableFields.AllowDrop = true;
			this.lvAvailableFields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.columnHeader1});
			this.lvAvailableFields.FullRowSelect = true;
			this.lvAvailableFields.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvAvailableFields.HideSelection = false;
			resources.ApplyResources(this.lvAvailableFields, "lvAvailableFields");
			this.lvAvailableFields.Name = "lvAvailableFields";
			this.lvAvailableFields.SmallImageList = this.imageList;
			this.lvAvailableFields.UseCompatibleStateImageBehavior = false;
			this.lvAvailableFields.View = System.Windows.Forms.View.Details;
			this.lvAvailableFields.Resize += new System.EventHandler(this.lvAvailableFields_Resize);
			this.lvAvailableFields.DoubleClick += new System.EventHandler(this.lvAvailableFields_DoubleClick);
			this.lvAvailableFields.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvAvailableFields_DragDrop);
			this.lvAvailableFields.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvAvailableFields_DragEnter);
			this.lvAvailableFields.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvAvailableFields_ItemDrag);
			this.lvAvailableFields.DragOver += new System.Windows.Forms.DragEventHandler(this.lvAvailableFields_DragOver);
			resources.ApplyResources(this.columnHeader1, "columnHeader1");
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			resources.ApplyResources(this.imageList, "imageList");
			this.imageList.TransparentColor = System.Drawing.Color.Lime;
			this.btnRemove.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnRemove, "btnRemove");
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			this.btnAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			resources.ApplyResources(this.lblPriority, "lblPriority");
			this.lblPriority.Name = "lblPriority";
			this.btnIncreasePriority.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnIncreasePriority, "btnIncreasePriority");
			this.btnIncreasePriority.Name = "btnIncreasePriority";
			this.btnIncreasePriority.Click += new System.EventHandler(this.btnIncreasePriority_Click);
			this.btnAppend.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnAppend, "btnAppend");
			this.btnAppend.Name = "btnAppend";
			this.btnAppend.Click += new System.EventHandler(this.btnAppend_Click);
			this.btnDecreasePriority.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnDecreasePriority, "btnDecreasePriority");
			this.btnDecreasePriority.Name = "btnDecreasePriority";
			this.btnDecreasePriority.Click += new System.EventHandler(this.btnDecreasePriority_Click);
			this.Controls.Add(this.btnAppend);
			this.Controls.Add(this.btnDecreasePriority);
			this.Controls.Add(this.btnIncreasePriority);
			this.Controls.Add(this.lblPriority);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.lvAvailableFields);
			this.Controls.Add(this.picPreview);
			this.Name = "WizPageGrouping";
			this.Controls.SetChildIndex(this.picPreview, 0);
			this.Controls.SetChildIndex(this.headerPanel, 0);
			this.Controls.SetChildIndex(this.headerSeparator, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.subtitleLabel, 0);
			this.Controls.SetChildIndex(this.headerPicture, 0);
			this.Controls.SetChildIndex(this.lvAvailableFields, 0);
			this.Controls.SetChildIndex(this.btnAdd, 0);
			this.Controls.SetChildIndex(this.btnRemove, 0);
			this.Controls.SetChildIndex(this.lblPriority, 0);
			this.Controls.SetChildIndex(this.btnIncreasePriority, 0);
			this.Controls.SetChildIndex(this.btnDecreasePriority, 0);
			this.Controls.SetChildIndex(this.btnAppend, 0);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		static void UpdateListViewColumnWidth(ListView lv) {
			lv.Columns[0].Width = lv.ClientRectangle.Width;
		}
		private void lvAvailableFields_Resize(object sender, System.EventArgs e) {
			UpdateListViewColumnWidth(lvAvailableFields);
		}
		private void picPreview_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			int count = groupingFieldsRects.Count;
			for (int i = 0; i < count; i++) {
				Rectangle rect = (Rectangle)groupingFieldsRects[i];
				if (rect.Contains(e.X, e.Y)) {
					if (selectedGroupFieldIndex != i) {
						selectedGroupFieldIndex = i;
						UpdateImage();
					}
					break;
				}
			}
		}
		void ChangePriority(int val) {
			ObjectNameCollection obj1 = groupingFieldsSet[selectedGroupFieldIndex];
			groupingFieldsSet[selectedGroupFieldIndex] = groupingFieldsSet[selectedGroupFieldIndex + val];
			selectedGroupFieldIndex += val;
			groupingFieldsSet[selectedGroupFieldIndex] = obj1;
			UpdateImage();
		}
		private void btnIncreasePriority_Click(object sender, System.EventArgs e) {
			ChangePriority(-1);
		}
		private void btnDecreasePriority_Click(object sender, System.EventArgs e) {
			ChangePriority(1);
		}
		void RemoveFromRemainingFields(ObjectName val) {
			int count = remainingFields.Count;
			for (int i = 0; i < count; i++) {
				if (remainingFields[i] == val) {
					remainingFields.RemoveAt(i);
					break;
				}
			}
		}
		void AddToGroupingFields(bool append) {
			ArrayList items = new ArrayList();
			items.AddRange(lvAvailableFields.SelectedItems);
			if (items.Count <= 0)
				return;
			int index = int.MaxValue;
			foreach (ListViewItem item in items) {
				if (item.Index < index)
					index = item.Index;
				lvAvailableFields.Items.Remove(item);
				RemoveFromRemainingFields(item.Tag as ObjectName);
				if (append && selectedGroupFieldIndex < groupingFieldsSet.Count) {
					ObjectNameCollection groupFields = groupingFieldsSet[selectedGroupFieldIndex];
					groupFields.Add(item.Tag as ObjectName);
				}
				else {
					ObjectNameCollection groupFields = new ObjectNameCollection();
					groupFields.Add(item.Tag as ObjectName);
					groupingFieldsSet.Add(groupFields);
				}
			}
			if (index + 1 > lvAvailableFields.Items.Count)
				index = lvAvailableFields.Items.Count - 1;
			if (index >= 0)
				lvAvailableFields.Items[index].Selected = true;
			if (!append)
				selectedGroupFieldIndex = groupingFieldsSet.Count - 1;
			UpdateImage();
			lvAvailableFields.Focus();
		}
		void RemoveFromGroupingFields() {
			if (groupingFieldsSet.Count <= 0)
				return;
			groupingFieldsSet.RemoveAt(selectedGroupFieldIndex);
			RecreateRemainingFields(groupingFieldsSet);
			if (selectedGroupFieldIndex >= groupingFieldsSet.Count)
				selectedGroupFieldIndex = groupingFieldsSet.Count - 1;
			UpdateImage();
		}
		private void btnAdd_Click(object sender, System.EventArgs e) {
			AddToGroupingFields(false);
		}
		private void btnAppend_Click(object sender, System.EventArgs e) {
			AddToGroupingFields(true);
		}
		private void lvAvailableFields_DoubleClick(object sender, System.EventArgs e) {
			AddToGroupingFields(Control.ModifierKeys == Keys.Control);
		}
		private void btnRemove_Click(object sender, System.EventArgs e) {
			RemoveFromGroupingFields();
		}
		void HandlePreviewDragOver(DragEventArgs e) {
			if (lvAvailableFields.Equals(e.Data.GetData(typeof(ListView))))
				e.Effect = e.AllowedEffect;
			else
				e.Effect = DragDropEffects.None;
		}
		void HandleFieldListDragOver(DragEventArgs e) {
			if (picPreview.Equals(e.Data.GetData(typeof(PictureBox))))
				e.Effect = e.AllowedEffect;
			else
				e.Effect = DragDropEffects.None;
		}
		private void picPreview_DragEnter(object sender, System.Windows.Forms.DragEventArgs e) {
			HandlePreviewDragOver(e);
		}
		private void picPreview_DragOver(object sender, System.Windows.Forms.DragEventArgs e) {
			HandlePreviewDragOver(e);
		}
		private void picPreview_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
			AddToGroupingFields(Control.ModifierKeys == Keys.Control);
		}
		private void lvAvailableFields_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e) {
			if(!e.Button.IsLeft())
				return;
			DoDragDrop(new DataObject(lvAvailableFields), DragDropEffects.Move);
		}
		private void picPreview_DoubleClick(object sender, System.EventArgs e) {
			if (groupingFieldsRects.Count <= 0)
				return;
			Point pt = picPreview.PointToClient(Control.MousePosition);
			Rectangle rect = (Rectangle)groupingFieldsRects[selectedGroupFieldIndex];
			if (rect.Contains(pt))
				RemoveFromGroupingFields();
		}
		private void picPreview_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (groupingFieldsRects.Count <= 0)
				return;
			if(!e.Button.IsLeft())
				return;
			Rectangle rect = (Rectangle)groupingFieldsRects[selectedGroupFieldIndex];
			if (rect.Contains(e.X, e.Y))
				DoDragDrop(new DataObject(picPreview), DragDropEffects.Move);
		}
		private void lvAvailableFields_DragEnter(object sender, System.Windows.Forms.DragEventArgs e) {
			HandleFieldListDragOver(e);
		}
		private void lvAvailableFields_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
			RemoveFromGroupingFields();
		}
		private void lvAvailableFields_DragOver(object sender, System.Windows.Forms.DragEventArgs e) {
			HandleFieldListDragOver(e);
		}
		void RecreateRemainingFields(ObjectNameCollectionsSet groupFieldsSet) {
			remainingFields.Clear();
			lvAvailableFields.Items.Clear();
			ObjectNameCollection selectedFields = wizard.SelectedFields;
			int count = selectedFields.Count;
			for (int i = 0; i < count; i++) {
				ObjectName f = selectedFields[i];
				if (!ReportBuilder.FoundInGroupingFields(f, groupFieldsSet)) {
					ListViewItem item = new ListViewItem(f.DisplayName, 1);
					item.Tag = f;
					lvAvailableFields.Items.Add(item);
					remainingFields.Add(f);
				}
			}
		}
		void FillFieldsLists() {
			groupingFieldsSet.Clear();
			RecreateRemainingFields(wizard.GroupingFieldsSet);
			groupingFieldsSet.AddRange(wizard.GroupingFieldsSet);
		}
		protected override bool OnSetActive() {
			FillFieldsLists();
			UpdateListViewColumnWidth(lvAvailableFields);
			UpdateImage();
			if (lvAvailableFields.Items.Count > 0)
				lvAvailableFields.Items[0].Selected = true;
			lvAvailableFields.Focus();
			return true;
		}
		protected override void UpdateWizardButtons() {
			Wizard.WizardButtons = WizardButton.Back | WizardButton.Next | WizardButton.Finish;
		}
		void RollbackChanges() {
			wizard.GroupingFieldsSet.Clear();
		}
		void ApplyChanges() {
			wizard.GroupingFieldsSet.Clear();
			int count = groupingFieldsSet.Count;
			for (int i = 0; i < count; i++)
				wizard.GroupingFieldsSet.Add(groupingFieldsSet[i]);
		}
		protected override string OnWizardBack() {
			RollbackChanges();
			return WizardForm.NextPage;
		}
		protected override string OnWizardNext() {
			ApplyChanges();
			if (groupingFieldsSet.Count <= 0)
				return "WizPageUngroupedLayout";
			return wizard.GetFieldsForSummary().Count > 0 ? "WizPageSummary" : "WizPageGroupedLayout";
		}
		protected override bool OnWizardFinish() {
			ApplyChanges();
			return true;
		}
	}
}
