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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.Templates;
using DevExpress.XtraSplashScreen;
namespace DevExpress.XtraReports.Native.Templates {
	[ToolboxItem(false)]
	public class TemplateForm : XtraForm {
		private SimpleButton bntCancel;
		private GroupControl gcPreviewContainer;
		private GroupControl groupControl1;
		private LabelControl labelControl2;
		private RatingControl lbRating;
		private FramedPictureBox pictureEdit1;
		private LabelControl labelControl5;
		private TextEdit teCreatedBy;
		private System.Windows.Forms.Timer timer;
		private GroupControl groupControl2;
		private IContainer components;
		private GalleryUserControl galleryUserControl1;
		private LabelControl lbDescr;
		private SimpleButton btnLoad;
		private GroupControl groupControl4;
		ITemplateProvider templateProvider;
		public TemplateForm() {
			InitializeComponent();
			DataSource = new System.ComponentModel.BindingList<Template>();
			DataSource.ListChanged += new ListChangedEventHandler(DataSource_ListChanged);
			SetupControlsBindings();
			SubscribeEvents();
			ActiveControl = this.galleryUserControl1.galleryControl1;
		}
		protected override void OnFormClosed(System.Windows.Forms.FormClosedEventArgs e) {
			base.OnFormClosed(e);
			timer.Enabled = false;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			TemplateProvider.GetTemplates(string.Empty, GetTemplates);
		}
		public ITemplateProvider TemplateProvider {
			get {
				if(templateProvider == null)
					templateProvider = DXTemplateProvider.CreateReportTemplateProvider();
				return templateProvider;
			}
			set {
				templateProvider = value;
			}
		}
		void DataSource_ListChanged(object sender, ListChangedEventArgs e) {
			UpdateControlsProperties(DataSource.Count > 0);
		}
		#region InitializeComponent
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateForm));
			this.bntCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnLoad = new DevExpress.XtraEditors.SimpleButton();
			this.gcPreviewContainer = new DevExpress.XtraEditors.GroupControl();
			this.galleryUserControl1 = new DevExpress.XtraReports.Native.Templates.GalleryUserControl();
			this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
			this.groupControl4 = new DevExpress.XtraEditors.GroupControl();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.pictureEdit1 = new DevExpress.XtraReports.Native.Templates.FramedPictureBox();
			this.lbDescr = new DevExpress.XtraEditors.LabelControl();
			this.lbRating = new DevExpress.XtraReports.Native.Templates.RatingControl();
			this.teCreatedBy = new DevExpress.XtraEditors.TextEdit();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
			this.timer = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.gcPreviewContainer)).BeginInit();
			this.gcPreviewContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
			this.groupControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl4)).BeginInit();
			this.groupControl4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.teCreatedBy.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.bntCancel, "bntCancel");
			this.bntCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bntCancel.Name = "bntCancel";
			resources.ApplyResources(this.btnLoad, "btnLoad");
			this.btnLoad.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			this.gcPreviewContainer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.gcPreviewContainer.Controls.Add(this.galleryUserControl1);
			this.gcPreviewContainer.Controls.Add(this.btnLoad);
			this.gcPreviewContainer.Controls.Add(this.bntCancel);
			this.gcPreviewContainer.Controls.Add(this.groupControl2);
			resources.ApplyResources(this.gcPreviewContainer, "gcPreviewContainer");
			this.gcPreviewContainer.Name = "gcPreviewContainer";
			this.gcPreviewContainer.ShowCaption = false;
			resources.ApplyResources(this.galleryUserControl1, "galleryUserControl1");
			this.galleryUserControl1.BackColor = System.Drawing.Color.Transparent;
			this.galleryUserControl1.Name = "galleryUserControl1";
			resources.ApplyResources(this.groupControl2, "groupControl2");
			this.groupControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.groupControl2.Controls.Add(this.groupControl4);
			this.groupControl2.Name = "groupControl2";
			this.groupControl2.ShowCaption = false;
			this.groupControl4.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("groupControl4.Appearance.BackColor")));
			this.groupControl4.Appearance.Options.UseBackColor = true;
			this.groupControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.groupControl4.Controls.Add(this.groupControl1);
			resources.ApplyResources(this.groupControl4, "groupControl4");
			this.groupControl4.Name = "groupControl4";
			this.groupControl4.ShowCaption = false;
			this.groupControl1.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("groupControl1.Appearance.BackColor")));
			this.groupControl1.Appearance.Options.UseBackColor = true;
			this.groupControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.groupControl1.Controls.Add(this.pictureEdit1);
			this.groupControl1.Controls.Add(this.lbDescr);
			this.groupControl1.Controls.Add(this.lbRating);
			this.groupControl1.Controls.Add(this.teCreatedBy);
			this.groupControl1.Controls.Add(this.labelControl2);
			this.groupControl1.Controls.Add(this.labelControl5);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.ShowCaption = false;
			resources.ApplyResources(this.pictureEdit1, "pictureEdit1");
			this.pictureEdit1.EditValue = null;
			this.pictureEdit1.Name = "pictureEdit1";
			this.pictureEdit1.TabStop = false;
			resources.ApplyResources(this.lbDescr, "lbDescr");
			this.lbDescr.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.lbDescr.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
			this.lbDescr.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.lbDescr.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.lbDescr.Name = "lbDescr";
			resources.ApplyResources(this.lbRating, "lbRating");
			this.lbRating.Name = "lbRating";
			this.lbRating.RatingValue = 0F;
			this.lbRating.StarsColor = System.Drawing.Color.Goldenrod;
			this.lbRating.StarsCount = 4;
			this.lbRating.TabStop = false;
			resources.ApplyResources(this.teCreatedBy, "teCreatedBy");
			this.teCreatedBy.Name = "teCreatedBy";
			this.teCreatedBy.Properties.AppearanceDisabled.BackColor = ((System.Drawing.Color)(resources.GetObject("teCreatedBy.Properties.AppearanceDisabled.BackColor")));
			this.teCreatedBy.Properties.AppearanceDisabled.ForeColor = ((System.Drawing.Color)(resources.GetObject("teCreatedBy.Properties.AppearanceDisabled.ForeColor")));
			this.teCreatedBy.Properties.AppearanceDisabled.Options.UseBackColor = true;
			this.teCreatedBy.Properties.AppearanceDisabled.Options.UseForeColor = true;
			this.teCreatedBy.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.teCreatedBy.TabStop = false;
			this.labelControl2.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelControl2.Appearance.Font")));
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			this.labelControl5.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelControl5.Appearance.Font")));
			resources.ApplyResources(this.labelControl5, "labelControl5");
			this.labelControl5.Name = "labelControl5";
			this.timer.Interval = 500;
			this.timer.Tick += new System.EventHandler(this.timer1_Tick);
			this.AcceptButton = this.btnLoad;
			this.CancelButton = this.bntCancel;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.gcPreviewContainer);
			this.MinimizeBox = false;
			this.Name = "TemplateForm";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(this.gcPreviewContainer)).EndInit();
			this.gcPreviewContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
			this.groupControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl4)).EndInit();
			this.groupControl4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.groupControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.teCreatedBy.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		Thread thread;
		BindingList<Template> DataSource { get; set; }
		string ExceptionMessage { get; set; }
		GalleryControlGallery Gallery {
			get {
				return this.galleryUserControl1.galleryControl1.Gallery;
			}
		}
		public byte[] LayoutStream { get; set; }
		void GetTemplates(TemplatesInfo templatesInfo) {
			if(IsHandleCreated)
				BeginInvoke(new Action<TemplatesInfo>(UpdateDataSource), templatesInfo);
		}
		void UpdateDataSource(TemplatesInfo templatesInfo) {
			DataSource.RaiseListChangedEvents = false;
			DataSource.Clear();
			if(templatesInfo.Templates != null)
				templatesInfo.Templates.ForEach(item => DataSource.Add(item));
			DataSource.RaiseListChangedEvents = true;
			FillGallary();
			DataSource.ResetBindings();
			UpdateTemplateImages();
		}
		void UpdateTemplateImages() {
			if(thread != null)
				thread.Abort();
			thread = new Thread(new ThreadStart(delegate() {
				if(DataSource.Count > 0)
					TemplateProvider.GetPreviewImageAsync(DataSource[0].ID,GetPreviewImage);
				for(int i = 0; i < DataSource.Count; i++) {
					Template template = DataSource[i];
					template.IconBytes = TemplateProvider.GetIconImage(template.ID);
					if(this.IsHandleCreated)
						BeginInvoke(new Action<int>(SetGallaryItemImage), i);
				}
				thread.Abort();
				thread = null;
			}));
			thread.Name = "UpdateTemplateImages";
			thread.Start();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
				if(thread != null) {
					thread.Abort();
					thread = null;
				}
			}
			base.Dispose(disposing);
		}
		void FillGallary() {
			Gallery.Groups[0].Items.Clear();
			for(int i = 0; i < DataSource.Count; i++) {
				GalleryItem item = new GalleryItem() { Checked = i == 0 };
				item.Caption = DataSource[i].Name;
				item.Image = SetImageBorder(DataSource[i].Preview, Gallery.GalleryControl.Gallery.ImageSize);
				item.Tag = i;
				Gallery.Groups[0].Items.Add(item);
			}
		}
		void SetGallaryItemImage(int index) {
			DataSource.ResetItem(index);
			if(index >= 0 && index < DataSource.Count && DataSource[index].Icon != null)
				Gallery.Groups[0].Items[index].Image = SetImageBorder(DataSource[index].Icon, Gallery.GalleryControl.Gallery.ImageSize);
		}
		Image SetImageBorder(Image source, Size size) {
			Bitmap bitmap = FormTemplateHelper.CreateImage(source, size, Color.Transparent);
			using(Graphics graphics = Graphics.FromImage(bitmap)) {
				using(Pen pen = new Pen(Color.FromArgb(184, 184, 184))) {
					RectangleF rect = FormTemplateHelper.InscribeSize(source.Size, size);
					rect.Width--;
					rect.Height--;
					graphics.DrawRectangle(pen, Rectangle.Ceiling(rect));
				}
			}
			return bitmap;
		}
		void SetupControlsBindings() {
			lbRating.DataBindings.Add("RatingValue", DataSource, "Rating");
			teCreatedBy.DataBindings.Add("EditValue", DataSource, "Author.Name");
			lbDescr.DataBindings.Add("Text", DataSource, "Description");
			lbRating.DataBindings.Add("Text", DataSource, "Rating");
			pictureEdit1.DataBindings.Add("EditValue", DataSource, "Preview");
		}
		void SubscribeEvents() {
			Gallery.ItemCheckedChanged += new GalleryItemEventHandler(galleryControlGallery1_ItemCheckedChanged);
			galleryUserControl1.teSearch.TextChanged += new EventHandler(textEdit1_EditValueChanged);
		}
		private void btnLoad_Click(object sender, EventArgs e) {
			using(SplashScreenManager splashManager = new SplashScreenManager(this, typeof(DevExpress.XtraWaitForm.DemoWaitForm), false, false)) {
				LayoutStream = DataSource[SelectedTemplateIndex].LayoutBytes;
				if(LayoutStream == null) {
					splashManager.ShowWaitForm();
					splashManager.SetWaitFormDescription("Loading...");
					try {
						LayoutStream = TemplateProvider.GetTemplateLayout(DataSource[SelectedTemplateIndex].ID);
						if(LayoutStream == null)
							throw new Exception("Report layout is empty");
					} catch(Exception ex) {
						splashManager.CloseWaitForm();
						XtraMessageBox.Show(string.Format("Unable to download the report template. Error message: {0}", ex.Message));
					} finally {
						if(splashManager.IsSplashFormVisible)
							splashManager.CloseWaitForm();
					}
				}
			}
		}
		void UpdateControlsProperties(bool isCountGreaterThenZero) {
			galleryUserControl1.gcNoData.lbNoItemsFound.Text = string.IsNullOrEmpty(ExceptionMessage) ? "No items found" : ExceptionMessage;
			btnLoad.Enabled = SelectedTemplateIndex != -1;
			galleryUserControl1.teSearch.Enabled = true;
			groupControl1.Visible = isCountGreaterThenZero;
			galleryUserControl1.gcProgress.Visible = false;
			galleryUserControl1.galleryControl1.Visible = isCountGreaterThenZero;
		}
		int SelectedTemplateIndex {
			get {
				var checkedItems = Gallery.Groups[0].GetCheckedItems();
				var checkedItem = checkedItems != null && checkedItems.Count > 0 ? checkedItems[0] : null;
				return checkedItem != null ? (int)checkedItem.Tag : -1;
			}
		}
		private void textEdit1_EditValueChanged(object sender, EventArgs e) {
			timer.Enabled = false;
			timer.Enabled = true;
		}
		private void timer1_Tick(object sender, EventArgs e) {
			timer.Enabled = false;
			TemplateProvider.GetTemplates((string)galleryUserControl1.teSearch.EditValue, GetTemplates);
			btnLoad.Enabled = false;
			groupControl1.Visible = false;
			galleryUserControl1.gcProgress.Visible = true;
		}
		private void galleryControlGallery1_ItemCheckedChanged(object sender, GalleryItemEventArgs e) {
			int position = (int)e.Item.Tag;
			TemplateProvider.GetPreviewImageAsync(DataSource[position].ID, GetPreviewImage);
			galleryUserControl1.galleryControl1.BindingContext[DataSource].Position = position;
		}
		void GetPreviewImage(Guid id, byte[] data) {
			if(IsHandleCreated) {
				BeginInvoke(new Action(() => {
					Template template = DataSource.Where(item => item.ID == id).Single();
					template.PreviewBytes = data;
					DataSource.ResetItem(DataSource.IndexOf(template));
				}));
			}
		}
	}
}
