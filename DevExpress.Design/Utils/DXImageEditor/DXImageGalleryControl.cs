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
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Skins;
namespace DevExpress.Utils.Design {
	[ToolboxItem(false)]
	public partial class DXImageGalleryControl : XtraUserControl {
		IServiceProvider serviceProvider;
		IDefaultResourcePickerServiceProvider defaultResourcePickerSvcProvider;
		DXImageGalleryControlOptions options;
		ITopFormOptionsProvider optionsProvider;
		public DXImageGalleryControl() {
			this.options = CreateOptions();
			InitializeComponent();
		}
		protected virtual DXImageGalleryControlOptions CreateOptions() {
			return new DXImageGalleryControlOptions();
		}
		public void InitServices(IServiceProvider serviceProvider, IDefaultResourcePickerServiceProvider defaultResPickerSvcProvider, ITopFormOptionsProvider optionsProvider) {
			this.serviceProvider = serviceProvider;
			this.defaultResourcePickerSvcProvider = defaultResPickerSvcProvider;
			this.optionsProvider = optionsProvider;
		}
		#region SelectedItemChanged
		public event GallerySelectedItemChanged SelectedItemChanged;
		protected void RaiseSelectedItemChanged(DXImageGalleryItem item) {
			if(SelectedItemChanged != null)
				SelectedItemChanged(this, item);
		}
		#endregion
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(!OptionsProvider.IsAsync) DoLoad();
		}
		protected internal virtual void DoLoad() {
			if(!DesignMode && !this.isInitialized) InitializeSearchBox();
			Initialize();
		}
		protected internal SplitContainerControl SplitContainerControl {
			get { return this.splitContainerControl; }
		}
		bool isInitialized = false;
		protected internal void Initialize() {
			if(DesignMode || this.isInitialized) return;
			InitializeCategories();
			InitializeSizes();
			InitializeCollections();
			InitializeGallery();
			this.isInitialized = true;
		}
		Image SearchIcon { get { return imgCol.Images[0]; } }
		Image SearchCancelIcon { get { return imgCol.Images[1]; } }
		protected internal void InitializeSearchBox() {
			UpdateButtonEditSearchStateImage(SearchIcon);
		}
		void InitializeSizes() {
			if(OptionsProvider.DesiredImageSize != null) {
				Size desiredSize = (Size)OptionsProvider.DesiredImageSize;
				if(DXImageGalleryItem.IsSmallSize(desiredSize)) {
					Item16x16.CheckState = CheckState.Checked;
					Item32x32.CheckState = CheckState.Unchecked;
					return;
				}
				if(DXImageGalleryItem.IsLargeSize(desiredSize)) {
					Item16x16.CheckState = CheckState.Unchecked;
					Item32x32.CheckState = CheckState.Checked;
					return;
				}
			}
			Item16x16.CheckState = (Options.UseSmallImages ? CheckState.Checked : CheckState.Unchecked);
			Item32x32.CheckState = (Options.UseLargeImages ? CheckState.Checked : CheckState.Unchecked);
		}
		CheckedListBoxItem CheckAllItem { get { return  lbCategories.Items[0]; } }
		void InitializeCategories() {
			lbCategories.Items.Clear();
			lbCategories.Items.Add(new CheckedListBoxItem() { CheckState = CheckState.Checked, Description = "Select All", Value = 0 }, true);
			foreach(DXImageGalleryCategory category in DataModelCore.Categories) {
				lbCategories.Items.Add(category, true);
			}
		}
		void InitializeCollections() {
			ColoredItem.CheckState = (Options.UseColoredImages ? CheckState.Checked : CheckState.Unchecked);
			GrayScaledItem.CheckState = (Options.UseGrayScaledImages ? CheckState.Checked : CheckState.Unchecked);
			Office2013Item.CheckState = (Options.UseOffice2013Images ? CheckState.Checked : CheckState.Unchecked);
			DevAVItem.CheckState = (Options.UseDevAVImages ? CheckState.Checked : CheckState.Unchecked);
		}
		void InitializeGallery() {
			Gallery.Gallery.BeginUpdate();
			try {
				Gallery.Gallery.ItemCheckMode = OptionsProvider.AllowMultiSelect ? ItemCheckMode.Multiple : ItemCheckMode.SingleRadio; ;
				foreach(DXImageGalleryCategory category in DataModelCore.Categories) {
					GalleryItemGroup group = new GalleryItemGroup() { Caption = category.CategoryName, Tag = category };
					Gallery.Gallery.Groups.Add(group);
					foreach(DXImageGalleryItem item in category.Items) {
						GalleryItem galleryItem = CreateGalleryItem(item);
						group.Items.Add(galleryItem);
					}
				}
				UpdateGalleryItemsVisibility();
				CheckEmptyGroups();
			}
			finally {
				Gallery.Gallery.EndUpdate();
			}
		}
		protected virtual GalleryItem CreateGalleryItem(DXImageGalleryItem item) {
			return new GalleryItem() { Caption = item.Name, Image = item.Image, Hint = item.FriendlyName, Tag = item };
		}
		void UpdateGalleryGroupsVisibility() {
			Gallery.Gallery.BeginUpdate();
			try {
				foreach(CheckedListBoxItem item in lbCategories.Items) {
					DXImageGalleryCategory category = item.Value as DXImageGalleryCategory;
					UpdateGalleryGroupVisibility(category, item.CheckState);
				}
			}
			finally {
				Gallery.Gallery.EndUpdate();
			}
		}
		void UpdateGalleryGroupVisibility(DXImageGalleryCategory category, CheckState checkState) {
			Gallery.Gallery.BeginUpdate();
			try {
				GalleryItemGroup group = FindGroupByCategory(category);
				if(group != null) {
					group.Visible = (checkState == CheckState.Checked);
				}
			}
			finally {
				Gallery.Gallery.EndUpdate();
			}
		}
		void CheckEmptyGroups() {
			Gallery.Gallery.BeginUpdate();
			try {
				foreach(GalleryItemGroup group in Gallery.Gallery.Groups) {
					if(!group.HasVisibleItems()) {
						group.Visible = false;
					}
				}
			}
			finally {
				Gallery.Gallery.EndUpdate();
			}
		}
		void UpdateGalleryItemsVisibility() {
			Gallery.Gallery.BeginUpdate();
			try {
				EnsureImageSizeCore();
				foreach(GalleryItemGroup group in Gallery.Gallery.Groups) {
					UpdateGalleryItemsVisibilityCore(group);
				}
			}
			finally {
				Gallery.Gallery.EndUpdate();
			}
		}
		void EnsureImageSizeCore() {
			Gallery.Gallery.ImageSize = GetGalleryItemSize();
		}
		Size GetGalleryItemSize() {
			if(Options.UseSmallImages && !Options.UseLargeImages)
				return new Size(16, 16);
			return new Size(32, 32);
		}
		void UpdateGalleryItemsVisibilityCore(GalleryItemGroup group) {
			foreach(GalleryItem item in group.Items) {
				DXImageGalleryItem gi = (DXImageGalleryItem)item.Tag;
				item.Visible = CheckItemVisibility(gi);
			}
		}
		bool CheckItemVisibility(DXImageGalleryItem gi) {
			return IsMatchBySize(gi) && IsMatchByType(gi);
		}
		bool IsMatchBySize(DXImageGalleryItem gi) {
			return gi.ItemSize == DXImageGalleryItemSize.Small ? Options.UseSmallImages : Options.UseLargeImages;
		}
		bool IsMatchByType(DXImageGalleryItem gi) {
			switch(gi.ItemType) {
				case DXImageGalleryItemType.GrayScaled:
					return Options.UseGrayScaledImages;
				case DXImageGalleryItemType.Office2013:
					return Options.UseOffice2013Images;
				case DXImageGalleryItemType.DevAV:
					return Options.UseDevAVImages;
				default:
					return Options.UseColoredImages;
			}
		}
		GalleryItemGroup FindGroupByCategory(DXImageGalleryCategory category) {
			foreach(GalleryItemGroup group in Gallery.Gallery.Groups) {
				if(object.ReferenceEquals(group.Tag, category))
					return group;
			}
			return null;
		}
		CheckedListBoxItem FindItemByValue(CheckedListBoxControl listBox, object value) {
			foreach(CheckedListBoxItem item in listBox.Items) {
				if(item.Value.Equals(value))
					return item;
			}
			return null;
		}
		CheckedListBoxItem Item16x16 {
			get { return FindItemByValue(lbSizes, (int)DXImageGalleryItemSize.Small); }
		}
		CheckedListBoxItem Item32x32 {
			get { return FindItemByValue(lbSizes, (int)DXImageGalleryItemSize.Large); }
		}
		CheckedListBoxItem ColoredItem {
			get { return FindItemByValue(lbCollections, (int)DXImageGalleryItemType.Colored); }
		}
		CheckedListBoxItem GrayScaledItem {
			get { return FindItemByValue(lbCollections, (int)DXImageGalleryItemType.GrayScaled); }
		}
		CheckedListBoxItem Office2013Item {
			get { return FindItemByValue(lbCollections, (int)DXImageGalleryItemType.Office2013); }
		}
		CheckedListBoxItem DevAVItem {
			get { return FindItemByValue(lbCollections, (int)DXImageGalleryItemType.DevAV); }
		}
		public virtual object EditValue {
			get {
				DXImageGalleryItem item = Options.SelectedItem;
				if(item == null)
					return null;
				if(OptionsProvider.ResourceType == DXImageGalleryResourceType.Form) {
					return item.Image;
				}
				return DefaultResourcePickerSvcProvider.AddItemToProject(item);
			}
		}
		public IEnumerable<DXImageGalleryItem> GetValues() {
			var items = Gallery.Gallery.GetCheckedItems();
			if(items.Count == 0 && Options.SelectedItem != null)
				yield return (DXImageGalleryItem)Options.SelectedItem;
			foreach(GalleryItem item in items) {
				if(item.Visible)
					yield return (DXImageGalleryItem)item.Tag;
			}
		}
		void OnCategoryItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			if(IsSearchMode || e.Index == 0) {
				if(e.Index == 0) UpdateCheckStateCategories(e.State);
				if(IsSearchMode) {
					UpdateSearchResult(searchResultGroup, beSearchBox.Text);
					UpdateCheckAllItem();
				}
				return;
			}
			CheckedListBoxControl listBox = sender as CheckedListBoxControl;
			object data = ((CheckedListBoxItem)listBox.Items[e.Index]).Value;
			UpdateGalleryGroupVisibility((DXImageGalleryCategory)data, e.State);
			CheckEmptyGroups();
			UpdateCheckAllItem();
		}
		void UpdateCheckAllItem() {
			List<object> categories = lbCategories.Items.GetCheckedValues();
			if(categories.Count == 0) {
				CheckAllItem.CheckState = CheckState.Unchecked;
				return;
			}
			if(categories[0] is CheckedListBoxItem) categories.RemoveAt(0);
			CheckAllItem.CheckState = categories.Count == lbCategories.Items.Count - 1 ? CheckState.Checked : CheckState.Indeterminate;
		}
		void UpdateCheckStateCategories(CheckState state) {
			Gallery.Gallery.BeginUpdate();
			try {
				if(state == CheckState.Indeterminate) return;
				if(state == CheckState.Checked) lbCategories.CheckAll();
				else lbCategories.UnCheckAll();
			}
			finally {
				Gallery.Gallery.EndUpdate();
			}
		}
		void OnSizeListItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			Gallery.Gallery.BeginUpdate();
			try {
				Options.UseSmallImages = (Item16x16.CheckState == CheckState.Checked);
				Options.UseLargeImages = (Item32x32.CheckState == CheckState.Checked);
				if(searchMode) {
					UpdateSearchResult(searchResultGroup, beSearchBox.Text);
					return;
				}
				UpdateGalleryItemsVisibility();
				UpdateGalleryGroupsVisibility();
				CheckEmptyGroups();
			}
			finally {
				Gallery.Gallery.EndUpdate();
			}
		}
		void OnCollectionListItemCheck(object sender, XtraEditors.Controls.ItemCheckEventArgs e) {
			Gallery.Gallery.BeginUpdate();
			try {
				Options.UseColoredImages = (ColoredItem.CheckState == CheckState.Checked);
				Options.UseGrayScaledImages = (GrayScaledItem.CheckState == CheckState.Checked);
				Options.UseOffice2013Images = (Office2013Item.CheckState == CheckState.Checked);
				Options.UseDevAVImages = (DevAVItem.CheckState == CheckState.Checked);
				if(searchMode) {
					UpdateSearchResult(searchResultGroup, beSearchBox.Text);
					return;
				}
				UpdateGalleryGroupsVisibility();
				UpdateGalleryItemsVisibility();
				CheckEmptyGroups();
			}
			finally {
				Gallery.Gallery.EndUpdate();
			}
		}
		bool searchMode = false;
		void OnSearchBoxEditValueChanged(object sender, EventArgs e) {
			ButtonEdit buttonEdit = sender as ButtonEdit;
			string searchString = (string)buttonEdit.EditValue;
			if(ShouldShowSearchResult(searchString)) {
				this.searchMode = true;
				ShowSearchResultGroup(searchString);
			}
			else if(ShouldHideSearchResult(searchString)) {
				this.searchMode = false;
				HideSearchResultGroup();
			}
			else if(ShouldUpdateSearchResult(searchString)) {
				UpdateSearchResult(this.searchResultGroup, searchString);
			}
		}
		void OnSearchBoxButtonClick(object sender, ButtonPressedEventArgs e) {
			if(e.Button.Index != 0) return;
			this.beSearchBox.Text = string.Empty;
		}
		void OnGalleryControlItemDoubleClick(object sender, GalleryItemClickEventArgs e) {
			Form frm = FindForm();
			if(frm != null) {
				DXImageEditorUtils.PostponedCall(state => ((Form)state).DialogResult = DialogResult.OK, frm); 
			}
		}
		void OnGalleryControlGalleryItemCheckedChanged(object sender, GalleryItemEventArgs e) {
			DXImageGalleryItem curr = e.Item.Tag as DXImageGalleryItem;
			if(curr == null) {
				throw new InvalidControlSettingsException("Wrong initialized gallery item");
			}
			Options.SelectedItem = curr;
			RaiseSelectedItemChanged(curr);
		}
		bool IsSearchMode {
			get { return this.searchMode; }
		}
		GalleryItemGroup searchResultGroup = null;
		void ShowSearchResultGroup(string searchString) {
			Gallery.Gallery.BeginUpdate();
			try {
				UpdateButtonEditSearchStateImage(SearchCancelIcon);
				foreach(GalleryItemGroup group in Gallery.Gallery.Groups) {
					group.Visible = false;
				}
				this.searchResultGroup = CreateSearchResultGroup(searchString);
				Gallery.Gallery.Groups.Add(this.searchResultGroup);
			}
			finally {
				Gallery.Gallery.EndUpdate();
			}
		}
		void HideSearchResultGroup() {
			Gallery.Gallery.BeginUpdate();
			try {
				UpdateButtonEditSearchStateImage(SearchIcon);
				Gallery.Gallery.Groups.Remove(this.searchResultGroup);
				UpdateGalleryGroupsVisibility();
				UpdateGalleryItemsVisibility();
				this.searchResultGroup = null;
				CheckEmptyGroups();
			}
			finally {
				Gallery.Gallery.EndUpdate();
			}
		}
		GalleryItemGroup UpdateSearchResult(GalleryItemGroup group, string searchString) {
			EnsureImageSizeCore();
			Gallery.Gallery.BeginUpdate();
			try {
				group.Items.Clear();
				var filteredItems = DataModelCore.ApplyFilter(searchString, GetCheckedCategoriesList());
				foreach(DXImageGalleryItem item in filteredItems) {
					if(IsMatchPostFilter(item))
						group.Items.Add(CreateGalleryItem(item));
				}
			}
			finally {
				Gallery.Gallery.EndUpdate();
			}
			return group;
		}
		List<string> GetCheckedCategoriesList() {
			var checkedValues = lbCategories.Items.GetCheckedValues();
			return (from obj in checkedValues where obj is DXImageGalleryCategory select obj.ToString()).ToList();
		}
		bool IsMatchPostFilter(DXImageGalleryItem item) {
			return IsMatchByType(item) && IsMatchBySize(item);
		}
		void UpdateButtonEditSearchStateImage(Image image) {
			EditorButtonCollection buttons = beSearchBox.Properties.Buttons;
			if(buttons.Count > 0) {
				beSearchBox.Properties.Buttons[0].Image = image;
			}
		}
		GalleryItemGroup CreateSearchResultGroup(string searchString) {
			GalleryItemGroup group = new GalleryItemGroup();
			group.Caption = "Search Result";
			return UpdateSearchResult(group, searchString);
		}
		bool ShouldShowSearchResult(string searchString) {
			if(this.searchMode)
				return false;
			return !string.IsNullOrEmpty(searchString);
		}
		bool ShouldHideSearchResult(string searchString) {
			if(!this.searchMode)
				return false;
			return string.IsNullOrEmpty(searchString);
		}
		bool ShouldUpdateSearchResult(string searchString) {
			return this.searchMode && !string.IsNullOrEmpty(searchString);
		}
		#region Disposing
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#endregion
		public DXImageGalleryDataModel DataModelCore {
			get { return DXImageGalleryStorage.Default.DataModel; }
		}
		protected DXImageGalleryControlOptions Options { get { return options; } }
		ITopFormOptionsProvider OptionsProvider { get { return optionsProvider; } }
		GalleryControl Gallery { get { return galleryControl; } }
		IDefaultResourcePickerServiceProvider DefaultResourcePickerSvcProvider { get { return defaultResourcePickerSvcProvider; } }
	}
	public delegate void GallerySelectedItemChanged(object sender, DXImageGalleryItem item);
	public delegate void GalleryResourceTypeSelectorChanged(object sender, DXImageGalleryResourceType type);
	public class DXImageGalleryControlOptions {
		public DXImageGalleryControlOptions() {
			this.UseSmallImages = false;
			this.UseLargeImages = true;
			this.UseColoredImages = true;
			this.UseGrayScaledImages = false;
			this.UseOffice2013Images = false;
			this.UseDevAVImages = false;
			this.SelectedItem = null;
		}
		public bool UseSmallImages { get; set; }
		public bool UseLargeImages { get; set; }
		public bool UseColoredImages { get; set; }
		public bool UseGrayScaledImages { get; set; }
		public bool UseOffice2013Images { get; set; }
		public bool UseDevAVImages { get; set; }
		public DXImageGalleryItem SelectedItem { get; set; }
	}
	public interface ITopFormOptionsProvider {
		bool AllowMultiSelect { get; }
		Size? DesiredImageSize { get; }
		DXImageGalleryResourceType ResourceType { get; }
		bool IsAsync { get; }
	}
	public class InvalidControlSettingsException : Exception {
		public InvalidControlSettingsException(string msg)
			: base(msg) {
		}
	}
}
