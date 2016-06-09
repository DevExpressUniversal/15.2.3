#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class ImageEditorController {
		private int changeModeFlag = 17;
		private int defaultImageSize = 32;
		private ModelEditorGalleryForm galeryForm;
		private string selectedImageName = null;
		public ImageEditorController(ModelEditorGalleryForm galeryForm) {
			this.galeryForm = galeryForm;
			if(galeryForm != null) {
				InitGalery();
			}
		}
		public void ShowEditor(string selectedImageName) {
			this.selectedImageName = selectedImageName;
			galeryForm.ShowDialog();
			galeryForm.Dispose();
		}
		public string SelectedImageName {
			get {
				return selectedImageName;
			}
		}
		private void InitGalery() {
			galeryForm.Gallery.Gallery.ImageSize = new Size(defaultImageSize, defaultImageSize);
			galeryForm.Gallery.Gallery.ShowItemText = true;
			galeryForm.Gallery.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.ZoomInside;
			galeryForm.Gallery.Gallery.ScrollMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryScrollMode.Smooth;
			galeryForm.Gallery.Gallery.Appearance.ItemCaptionAppearance.Normal.TextOptions.HAlignment = HorzAlignment.Center;
			galeryForm.Gallery.Gallery.Appearance.ItemCaptionAppearance.Normal.TextOptions.WordWrap = WordWrap.Wrap;
			galeryForm.Gallery.Gallery.Appearance.ItemCaptionAppearance.Normal.TextOptions.VAlignment = VertAlignment.Center;
			galeryForm.Gallery.Gallery.Appearance.ItemCaptionAppearance.Normal.Options.UseTextOptions = true;
			galeryForm.Gallery.Gallery.ShowGroupCaption = false;
			galeryForm.ZoomBar.Properties.AllowFocused = false;
			galeryForm.ZoomBar.TabStop = false;
			galeryForm.ZoomBar.Value = 5;
			galeryForm.ZoomBar.Properties.Minimum = 1;
			galeryForm.ZoomBar.Properties.Maximum = defaultImageSize * 2;
			galeryForm.ZoomBar.Value = defaultImageSize;
			SubscribeEvents();
			FillGallery(galeryForm.Gallery);
		}
		private void SubscribeEvents() {
			UnSubscribeEvents();
			if(galeryForm != null) {
				galeryForm.ZoomBar.ValueChanged += new EventHandler(ZoomTrackBar_ValueChanged);
				galeryForm.TextEdit.EditValueChanged += new EventHandler(TextEdit_EditValueChanged);
				galeryForm.Gallery.Gallery.ItemDoubleClick += new GalleryItemClickEventHandler(Gallery_ItemDoubleClick);
				galeryForm.Gallery.Gallery.CustomDrawItemText += new GalleryItemCustomDrawEventHandler(Gallery_CustomDrawItemText);
				galeryForm.TextEdit.KeyDown += new KeyEventHandler(control_KeyDown);
				galeryForm.Gallery.KeyDown += new KeyEventHandler(control_KeyDown);
				galeryForm.Gallery.KeyUp += new KeyEventHandler(control_KeyUp);
				galeryForm.TextEdit.KeyUp += new KeyEventHandler(control_KeyUp);
				galeryForm.Shown += new EventHandler(galeryForm_Shown);
				galeryForm.Disposed += new EventHandler(galeryForm_Disposed);
			}
		}
		private void UnSubscribeEvents() {
			if(galeryForm != null) {
				galeryForm.ZoomBar.ValueChanged -= new EventHandler(ZoomTrackBar_ValueChanged);
				galeryForm.TextEdit.EditValueChanged -= new EventHandler(TextEdit_EditValueChanged);
				galeryForm.Gallery.Gallery.ItemDoubleClick -= new GalleryItemClickEventHandler(Gallery_ItemDoubleClick);
				galeryForm.Gallery.Gallery.CustomDrawItemText -= new GalleryItemCustomDrawEventHandler(Gallery_CustomDrawItemText);
				galeryForm.TextEdit.KeyDown -= new KeyEventHandler(control_KeyDown);
				galeryForm.Gallery.KeyDown -= new KeyEventHandler(control_KeyDown);
				galeryForm.Gallery.KeyUp -= new KeyEventHandler(control_KeyUp);
				galeryForm.TextEdit.KeyUp -= new KeyEventHandler(control_KeyUp);
				galeryForm.Shown -= new EventHandler(galeryForm_Shown);
				galeryForm.Disposed -= new EventHandler(galeryForm_Disposed);
				galeryForm.Gallery.MouseWheel -= new MouseEventHandler(Gallery_MouseWheel);
			}
		}
		private void Gallery_ItemDoubleClick(object sender, GalleryItemClickEventArgs e) {
			CloseForm();
		}
		private void galeryForm_Disposed(object sender, EventArgs e) {
			UnSubscribeEvents();
			if(galeryForm != null) {
				galeryForm = null;
			}
		}
		private void FillGallery(GalleryControl control) {
			GalleryItemGroup group = new GalleryItemGroup();
			control.Gallery.Groups.Add(group);
			group.Items.AddRange(CollectImages().ToArray());
		}
		private bool IsImageNameWithSuffix(string imageName) {
			return imageName.EndsWith(ImageLoader.SmallImageSuffix)
							|| imageName.EndsWith(ImageLoader.DialogImageSuffix)
							|| imageName.EndsWith("_Large")
							|| imageName.EndsWith("_72x72");
		}
		private void galeryForm_Shown(object sender, EventArgs e) {
			galeryForm.Gallery.Focus();
			galeryForm.TextEdit.Focus();
			ZoomTrackBar_ValueChanged(galeryForm.ZoomBar, EventArgs.Empty);
		}
		private void Gallery_CustomDrawItemText(object sender, GalleryItemCustomDrawEventArgs e) {
			GalleryItemViewInfo itemInfo = e.ItemInfo as GalleryItemViewInfo;
			itemInfo.PaintAppearance.ItemDescriptionAppearance.Normal.DrawString(e.Cache,
			  e.Item.Description, itemInfo.DescriptionBounds);
			AppearanceObject app = itemInfo.PaintAppearance.ItemCaptionAppearance.Normal.Clone()
			  as AppearanceObject;
			app.Font = (Font)e.Item.Tag;
			app.DrawString(e.Cache, e.Item.Caption, itemInfo.CaptionBounds);
			e.Handled = true;
		}
		private void ZoomTrackBar_ValueChanged(object sender, EventArgs e) {
			try {
				int imageSize = galeryForm.ZoomBar.Value;
				canTrimming = galeryForm.ZoomBar.Value > changeModeFlag;
				galeryForm.Gallery.Gallery.BeginUpdate();
				if (canTrimming) {
					galeryForm.Gallery.Gallery.Appearance.ItemCaptionAppearance.Normal.TextOptions.HAlignment = HorzAlignment.Center;
					galeryForm.Gallery.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Top;
				} else {
					galeryForm.Gallery.Gallery.Appearance.ItemCaptionAppearance.Normal.TextOptions.HAlignment = HorzAlignment.Near;
					galeryForm.Gallery.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
				}
				galeryForm.Gallery.Gallery.Orientation = canTrimming ? System.Windows.Forms.Orientation.Vertical : System.Windows.Forms.Orientation.Horizontal;
				galeryForm.Gallery.Gallery.ImageSize = new Size(imageSize, imageSize);
			} finally {
				galeryForm.Gallery.Gallery.EndUpdate();
			}
		}
		private static string WildcardToRegex(string pattern) {
			return "^" + Regex.Escape(pattern).
			Replace("\\*", ".*").
			Replace("\\\\.*", "\\*").
			Replace("\\?", ".").
			Replace("\\\\.", "\\?") + "$";
		}
		private void TextEdit_EditValueChanged(object sender, EventArgs e) {
			galeryForm.Gallery.Gallery.BeginUpdate();
			try {
				string filterValue = galeryForm.TextEdit.EditValue as string;
				Regex regex = null;
				if (!string.IsNullOrEmpty(filterValue)) {
					if (!filterValue.Contains("*") && !filterValue.Contains("?")) {
						filterValue = "*" + filterValue + "*";
					}
					string paramValueRegexPattern = WildcardToRegex(filterValue.ToLower());
					regex = new Regex(paramValueRegexPattern);
				}
				foreach (GalleryItem item in galeryForm.Gallery.Gallery.GetAllItems()) {
					bool itemVisible = regex == null || regex.Match(item.Caption.ToLower()).Success;
					item.Visible = itemVisible;
				}
			} finally { galeryForm.Gallery.Gallery.EndUpdate(); }
		}
		private void control_KeyDown(object sender, KeyEventArgs e) {
			if ((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab) && sender is TextEdit) {
				galeryForm.Gallery.Focus();
			}
			if (e.KeyCode == Keys.Escape || (sender is ModelEditorGalleryControl && e.KeyCode == Keys.Enter)) {
				CloseForm();
			}
			if(sender is ModelEditorGalleryControl && e.KeyCode == Keys.ControlKey) {
				galeryForm.Gallery.MouseWheel -= new MouseEventHandler(Gallery_MouseWheel);
				galeryForm.Gallery.MouseWheel += new MouseEventHandler(Gallery_MouseWheel);
			}
		}
		private void control_KeyUp(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.ControlKey) {
				galeryForm.Gallery.MouseWheel -= new MouseEventHandler(Gallery_MouseWheel);
			}
		}
		private void Gallery_MouseWheel(object sender, MouseEventArgs e) {
			DXMouseEventArgs dxMouseEventArgs = e as DXMouseEventArgs;
			if(dxMouseEventArgs != null) {
				dxMouseEventArgs.Handled = true;
				if(e.Delta > 0) {
					galeryForm.ZoomBar.Value++;
				}
				else {
					galeryForm.ZoomBar.Value--;
				}
			}
		}
		private List<GalleryItem> CollectImages() {
			List<GalleryItem> result = new List<GalleryItem>();
			Dictionary<string, ImageDate> cache = new Dictionary<string, ImageDate>();
			foreach(ImageSource imageSource in ImageLoader.Instance.ImageSources) {
				List<string> imageNames = imageSource.GetImageNames();
				if(imageNames.Count > 0) {
					List<string> imagesForRepleysForImageSource = new List<string>();
					foreach(string imageName in imageNames) {
						if(!IsImageNameWithSuffix(imageName)) {
							bool large = imageName.EndsWith(ImageLoader.LargeImageSuffix);
							string imageCaption = imageName;
							if(large) {
								imageCaption = imageName.Replace(ImageLoader.LargeImageSuffix, "");
								if(!imagesForRepleysForImageSource.Contains(imageCaption) && cache.ContainsKey(imageCaption)) {
									continue;
								}
								if(cache.ContainsKey(imageCaption)) {
									cache.Remove(imageCaption);
								}
							}
							if(!cache.ContainsKey(imageCaption)) {
								imagesForRepleysForImageSource.Add(imageCaption);
								cache.Add(imageCaption, new ImageDate(imageSource, imageName));
							}
						}
					}
				}
			}
			foreach(KeyValuePair<string, ImageDate> source in cache) {
				string imageCaption = source.Key;
				DevExpress.ExpressApp.Utils.ImageInfo imageInfo = DevExpress.ExpressApp.Utils.ImageInfo.Empty;
				try {
					imageInfo = source.Value.imageSource.FindImageInfo(source.Value.imageName, true);
				}
				catch(ArgumentException) { }
				catch(ImageLoadException) { }
				if(imageInfo != DevExpress.ExpressApp.Utils.ImageInfo.Empty) {
					GalleryItem item = new GalleryItem(imageInfo.Image, imageCaption, "");
					item.Hint = imageCaption;
					result.Add(item);
				}
			}
			result.Sort(new GalleryItemComparer());
			return result;
		}
		public void CloseForm() {
			selectedImageName = galeryForm.Gallery.SelectedImageName;
			galeryForm.Close();
		}
		private static bool canTrimming = false;
		public static bool CanTrimming {
			get {
				return canTrimming;
			}
		}
		private class ImageDate {
			public ImageSource imageSource;
			public string imageName;
			public ImageDate(ImageSource imageSource, string imageName) {
				this.imageSource = imageSource;
				this.imageName = imageName;
			}
		}
		private class GalleryItemComparer : IComparer<GalleryItem> {
			#region IComparer<GalleryItem> Members
			public int Compare(GalleryItem x, GalleryItem y) {
				return Comparer<string>.Default.Compare(x.Caption, y.Caption);
			}
			#endregion
		}
#if DebugTest
		public List<GalleryItem> DebugTest_CollectImages() {
			return CollectImages();
		}
#endif
	}
	[ToolboxItem(false)]
	public class ModelEditorGalleryControl : GalleryControl {
		private string selectedImageName;
		public ModelEditorGalleryControl() {
			this.Gallery.ItemCheckMode = DevExpress.XtraBars.Ribbon.Gallery.ItemCheckMode.SingleCheck;
		}
		public string SelectedImageName {
			get {
				return selectedImageName;
			}
			set {
				if(selectedImageName == null) {
					foreach(GalleryItem galleryItem in this.Gallery.GetAllItems()) {
						galleryItem.Checked = galleryItem.Caption == value;
						if(galleryItem.Checked) {
							selectedImageName = value;
							break;
						}
					}
				}
				else {
					selectedImageName = value;
				}
			}
		}
		protected override GalleryControlGallery CreateGallery() {
			return new ModelEditorGalleryControlGallery(this);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter && KeyboardSelectedItem != null) {
				SelectedImageName = KeyboardSelectedItem.Caption;
			}
			base.OnKeyDown(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			GalleryItem item = KeyboardSelectedItem;
			base.OnMouseLeave(e);
			if(KeyboardSelectedItem == null) {
				KeyboardSelectedItem = item;
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			GalleryItem item = KeyboardSelectedItem;
			base.OnMouseMove(e);
			if(KeyboardSelectedItem == null) {
				KeyboardSelectedItem = item;
			}
		}
		private bool lockCustomGotFocus = false;
		protected override void OnGotFocus(EventArgs e) {
			if(!lockCustomGotFocus) {
				List<GalleryItem> checkedItems = Gallery.GetCheckedItems();
				if(checkedItems.Count > 0) {
					KeyboardSelectedItem = checkedItems[0];
				}
				else {
					List<GalleryItem> visibleItems = Gallery.GetVisibleItems();
					if(visibleItems.Count > 0) {
						KeyboardSelectedItem = visibleItems[0];
					}
				}
				lockCustomGotFocus = true;
			}
			base.OnGotFocus(e);
		}
	}
	[ToolboxItem(false)]
	public class ModelEditorGalleryControlGallery : GalleryControlGallery {
		public ModelEditorGalleryControlGallery(GalleryControl galleryControl) : base(galleryControl) { }
		protected override BaseGalleryViewInfo CreateViewInfo() {
			return new ModelEditorGalleryControlGalleryViewInfo(this);
		}
		protected override void OnCheckedChanged(GalleryItem item) {
			ModelEditorGalleryControl control = this.GalleryControl as ModelEditorGalleryControl;
			if (control != null && item.Checked) {
				control.SelectedImageName = item.Caption;
			}
			base.OnCheckedChanged(item);
		}
	}
	public class ModelEditorGalleryControlGalleryViewInfo : GalleryControlGalleryViewInfo {
		public ModelEditorGalleryControlGalleryViewInfo(GalleryControlGallery gallery) : base(gallery) { }
		protected override GalleryItemGroupViewInfo CreateGroupInfo(BaseGalleryViewInfo galleryInfo, GalleryItemGroup group) {
			return new ModelEditorGalleryItemGroupViewInfo(galleryInfo, group);
		}
	}
	public class ModelEditorGalleryItemGroupViewInfo : GalleryItemGroupViewInfo {
		public ModelEditorGalleryItemGroupViewInfo(BaseGalleryViewInfo galleryInfo, GalleryItemGroup group)
			: base(galleryInfo, group) { }
		protected override GalleryItemViewInfo CreateItemViewInfo(GalleryItemGroupViewInfo groupInfo, GalleryItem item) {
			return new ModelEditorGalleryItemViewInfo(groupInfo, item);
		}
	}
	public class ModelEditorGalleryItemViewInfo : GalleryItemViewInfo {
		public ModelEditorGalleryItemViewInfo(GalleryItemGroupViewInfo groupInfo, GalleryItem item) : base(groupInfo, item) { }
		protected override Size CalcCaptionSize(DevExpress.Utils.Drawing.GraphicsInfo info) {
			 Size old = base.CalcCaptionSize(info);
			 if (ImageEditorController.CanTrimming) {
				 return new Size(64, old.Height);
			 } else {
				 return old;
			 }
		}
		protected override Size CalcTextSize() {
			return base.CalcTextSize();
		}
	}
}
