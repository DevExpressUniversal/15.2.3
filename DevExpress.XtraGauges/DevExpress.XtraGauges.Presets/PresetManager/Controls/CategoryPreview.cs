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
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Presets;
namespace DevExpress.XtraGauges.Presets.PresetManager {
	[System.ComponentModel.ToolboxItem(false)]
	public class CategoryPreview : Control, ISupportInitialize {
		List<GaugePreset> dataSourceCore;
		BaseGaugePreset targetPresetCore;
		CategoryPreviewViewInfo viewInfoCore;
		GalleryControl galleryCore;
		SimpleButton loadButtonCore;
		Brush BackgroundBrush;
		static string applyPresetWarning = "Do you want to load layout from a preset\r\n and override the current layout?";
		public CategoryPreview() {
			SetStyle(
					ControlStyles.SupportsTransparentBackColor |
					ControlConstants.DoubleBuffer |
					ControlStyles.ResizeRedraw |
					ControlStyles.AllPaintingInWmPaint |
					ControlStyles.ResizeRedraw |
					ControlStyles.UserMouse |
					ControlStyles.UserPaint,
					true
				);
			OnCreate();
		}
		protected void OnCreate() {
			this.loadButtonCore = new SimpleButton();
			this.galleryCore = new GalleryControl();
			this.viewInfoCore = CreateViewInfo();
			LoadButton.Parent = this;
			Gallery.Parent = this;
			Gallery.ItemDoubleClick += OnPresetItemDoubleClick;
			LoadButton.Text = "Load";
			LoadButton.Click += OnLoadPresetClick;
			Color bgColor = LookAndFeelHelper.GetSystemColor(UserLookAndFeel.Default, SystemColors.Control);
			this.BackgroundBrush = new SolidBrush(bgColor);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(ViewInfo != null) {
					ViewInfo.Dispose();
					viewInfoCore = null;
				}
				if(Gallery != null) {
					Gallery.ItemDoubleClick -= OnPresetItemDoubleClick;
					Gallery.Parent = null;
					Gallery.Dispose();
					galleryCore = null;
				}
				if(LoadButton != null) {
					LoadButton.Click -= OnLoadPresetClick;
					LoadButton.Parent = null;
					LoadButton.Dispose();
					loadButtonCore = null;
				}
				this.dataSourceCore = null;
			}
			base.Dispose(disposing);
		}
		protected virtual CategoryPreviewViewInfo CreateViewInfo() {
			return new CategoryPreviewViewInfo(this);
		}
		protected CategoryPreviewViewInfo ViewInfo {
			get { return viewInfoCore; }
		}
		protected GalleryControl Gallery {
			get { return galleryCore; }
		}
		protected SimpleButton LoadButton {
			get { return loadButtonCore; }
		}
		void ISupportInitialize.BeginInit() {
			((ISupportInitialize)Gallery).BeginInit();
		}
		void ISupportInitialize.EndInit() {
			((ISupportInitialize)Gallery).EndInit();
		}
		protected override void OnPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				GraphicsInfoArgs ea = new GraphicsInfoArgs(cache, e.ClipRectangle);
				DrawBackground(ea);
			}
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			LayoutChanged();
		}
		public void SetDataSource(List<GaugePreset> source, Func<byte[], Image> creator) {
			this.dataSourceCore = source;
			GalleryItemCollection items = new GalleryItemCollection();
			for(int i = 0; i < source.Count; i++) {
				byte[] data = source[i].LayoutInfo;
				if(data != null)
					items.Add(new GalleryItem(source[i].Name, () => creator(data)));
			}
			Gallery.Items = items;
			Gallery.SelectedIndex = items.Count > 0 ? 0 : -1;
		}
		public void LayoutChanged() {
			ViewInfo.SetDirty();
			ViewInfo.CalcInfo(null, new Rectangle(Point.Empty, Size));
			if(Gallery.Bounds != ViewInfo.Rects.Gallery) Gallery.Bounds = ViewInfo.Rects.Gallery;
			if(LoadButton.Bounds != ViewInfo.Rects.LoadButton) LoadButton.Bounds = ViewInfo.Rects.LoadButton;
		}
		protected void DrawBackground(GraphicsInfoArgs e) {
			e.Graphics.FillRectangle(BackgroundBrush, ViewInfo.Rects.Bounds);
		}
		public BaseGaugePreset TargetPreset {
			get { return targetPresetCore; }
			set { targetPresetCore = value; }
		}
		void OnPresetItemDoubleClick(object sender, EventArgs e) {
			LoadSelectedPreset();
		}
		void OnLoadPresetClick(object sender, EventArgs e) {
			LoadSelectedPreset();
		}
		void LoadSelectedPreset() {
			if(dataSourceCore != null) {
				if(Gallery.SelectedIndex >= 0) {
					TargetPreset = dataSourceCore[Gallery.SelectedIndex];
				}
			}
			PresetManagerForm presetManagerForm = FindForm() as PresetManagerForm;
			bool fNeedApply = (presetManagerForm.GaugeContainer.Gauges.Count == 0) || XtraMessageBox.Show(this, applyPresetWarning, "Preset Manager",
					MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
			if(fNeedApply && presetManagerForm != null) presetManagerForm.DialogResult = DialogResult.OK;
		}
	}
	public class CategoryPreviewViewInfo : BaseViewInfo {
		CategoryPreview ownerCore;
		CategoryPreviewRects viewRectsCore;
		public CategoryPreviewViewInfo(CategoryPreview owner) {
			this.ownerCore = owner;
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.viewRectsCore = new CategoryPreviewRects();
		}
		protected override void OnDispose() {
			this.viewRectsCore = null;
			this.ownerCore = null;
			base.OnDispose();
		}
		protected CategoryPreview Owner {
			get { return ownerCore; }
		}
		public CategoryPreviewRects Rects {
			get { return viewRectsCore; }
		}
		protected override void CalcViewRects(Rectangle bounds) {
			Rects.Clear();
			Size btnSise = new Size(80, 20);
			Rectangle loadBtn = (bounds.Height < (btnSise.Height + 8)) ? Rectangle.Empty :
				new Rectangle(bounds.Right - btnSise.Width - 4, bounds.Bottom - btnSise.Height - 4, btnSise.Width, btnSise.Height);
			Rectangle gallery = new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height - loadBtn.Height - 8);
			Rects.Bounds = bounds;
			Rects.LoadButton = loadBtn;
			Rects.Gallery = gallery;
		}
		protected override void CalcViewStates() { }
	}
	public class CategoryPreviewRects {
		public Rectangle Bounds;
		public Rectangle Gallery;
		public Rectangle LoadButton;
		public CategoryPreviewRects() {
			Clear();
		}
		public void Clear() {
			Bounds = Gallery = LoadButton = Rectangle.Empty;
		}
	}
}
