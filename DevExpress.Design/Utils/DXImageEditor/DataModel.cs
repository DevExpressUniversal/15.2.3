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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using DevExpress.Utils.Drawing;
namespace DevExpress.Utils.Design {
	public class DXImageGalleryDataModel {
		List<DXImageGalleryCategory> categories;
		List<DXImageGalleryItem> allItems;
		public DXImageGalleryDataModel() {
			this.categories = new List<DXImageGalleryCategory>();
			this.allItems = new List<DXImageGalleryItem>();
		}
		public void AddItem(DXImageGalleryItem galleryItem) {
			DXImageGalleryCategory category = GetTargetCategory(galleryItem.CategoryName);
			category.Items.Add(galleryItem);
			AllItems.Add(galleryItem);
		}
		public DXImageGalleryItem FindObjectByName(string name, DXImageGalleryItemType type) {
			foreach(DXImageGalleryItem item in AllItems) {
				if(type == item.ItemType && string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase) )
					return item;
			}
			return null;
		}
		public List<DXImageGalleryItem> ApplyFilter(string filterString, List<string> categories) {
			List<DXImageGalleryItem> filteredItems = new List<DXImageGalleryItem>();
			foreach(DXImageGalleryItem item in AllItems) {
				if(!categories.Contains(item.CategoryName)) continue;
				if(item.MatchFilter(filterString))
					filteredItems.Add(item);
			}
			return filteredItems;
		}
		protected virtual DXImageGalleryCategory GetTargetCategory(string categoryName) {
			foreach(DXImageGalleryCategory category in Categories) {
				if(string.Equals(category.CategoryName, categoryName, StringComparison.Ordinal))
					return category;
			}
			DXImageGalleryCategory newCategory = new DXImageGalleryCategory(categoryName);
			Categories.Add(newCategory);
			return newCategory;
		}
		public void Sort() {
			Categories.Sort(new DXImageGalleryGroupsComparer());
			foreach(DXImageGalleryCategory category in Categories) {
				category.Sort();
			}
		}
		#region DXImageGalleryGroupsComparer
		protected class DXImageGalleryGroupsComparer : IComparer<DXImageGalleryCategory> {
			public int Compare(DXImageGalleryCategory x, DXImageGalleryCategory y) {
				return x.CategoryName.CompareTo(y.CategoryName);
			}
		}
		#endregion
		public List<DXImageGalleryItem> AllItems { get { return allItems; } }
		public List<DXImageGalleryCategory> Categories { get { return categories; } }
	}
	public enum DXImageGalleryItemSize {
		Small,
		Large,
	}
	public enum DXImageGalleryItemType {
		Colored,
		GrayScaled,
		Office2013,
		DevAV
	}
	public enum DXImageGalleryImageSource {
		Default,
		Gallery
	}
	public enum DXImageGalleryResourceType {
		Project,
		Form
	}
	public class DXImageGalleryCategory {
		string categoryName;
		List<DXImageGalleryItem> items;
		public DXImageGalleryCategory(string categoryName) {
			this.categoryName = categoryName;
			this.items = new List<DXImageGalleryItem>();
		}
		public override bool Equals(object obj) {
			DXImageGalleryCategory sample = obj as DXImageGalleryCategory;
			if(sample == null)
				return false;
			return string.Equals(CategoryName, sample.CategoryName, StringComparison.Ordinal);
		}
		public override int GetHashCode() {
			return CategoryName.GetHashCode();
		}
		public override string ToString() {
			return CategoryName;
		}
		public void Sort() {
			Items.Sort(new GalleryItemsComparer());
		}
		#region 
		protected class GalleryItemsComparer : IComparer<DXImageGalleryItem> {
			public int Compare(DXImageGalleryItem x, DXImageGalleryItem y) {
				return x.FriendlyName.CompareTo(y.FriendlyName);
			}
		}
		#endregion
		public string CategoryName { get { return categoryName; } }
		public List<DXImageGalleryItem> Items { get { return items; } }
	}
	public class DXImageGalleryItem {
		string name;
		string uri;
		string[] tags;
		Image image;
		DXImageGalleryItemSize itemSize;
		DXImageGalleryItemType itemType;
		public DXImageGalleryItem(string uri, Image image, DXImageGalleryItemType itemType) {
			this.uri = uri;
			this.name = PrepareName(uri);
			this.image = image;
			this.itemSize = GetItemSize(image);
			this.itemType = itemType;
			this.tags = GetTags(image);
		}
		protected virtual string[] GetTags(Image image) {
			string rawTags = ImageMetadataHelper.LoadTags(image);
			return rawTags.Split(';', ',');
		}
		public static readonly Size SmallItemSize = new Size(16, 16);
		public static readonly Size LargeItemSize = new Size(32, 32);
		public static bool IsSmallSize(Size size) {
			return size == SmallItemSize;
		}
		public static bool IsLargeSize(Size size) {
			return size == LargeItemSize;
		}
		protected virtual DXImageGalleryItemSize GetItemSize(Image image) {
			if(IsSmallSize(image.Size)) {
				return DXImageGalleryItemSize.Small;
			}
			if(IsLargeSize(image.Size)) {
				return DXImageGalleryItemSize.Large;
			}
			throw new InvalidOperationException("Unsupported image size");
		}
		protected string PrepareName(string sourceName) {
			StringBuilder builder = new StringBuilder(sourceName);
			builder.Replace("%20", " ");
			return builder.ToString();
		}
		protected string[] GetNameParts() {
			string[] parts = Name.Split('\\', '/');
			return parts;
		}
		public override bool Equals(object obj) {
			DXImageGalleryItem sample = obj as DXImageGalleryItem;
			if(sample == null)
				return false;
			return string.Equals(Name, sample.Name, StringComparison.Ordinal);
		}
		public override int GetHashCode() {
			return Name.GetHashCode();
		}
		public override string ToString() {
			return Name;
		}
		public string Name { get { return name; } }
		public string Uri { get { return uri; } }
		public Image Image { get { return image; } }
		public DXImageGalleryItemSize ItemSize { get { return itemSize; } }
		public DXImageGalleryItemType ItemType { get { return itemType; } }
		public string[] Tags { get { return tags; } }
		public string FriendlyName {
			get {
				string res = string.Empty;
				string[] items = GetNameParts();
				if(items.Length > 0) {
					res = items[items.Length - 1];
				}
				return res;
			}
		}
		public string PropertyName {
			get {
				string res = FriendlyName;
				int pos = res.LastIndexOf('.');
				if(pos == -1)
					return res;
				return res.Substring(0, pos);
			}
		}
		public string CategoryName {
			get { return GetCategoryName(); }
		}
		protected string GetCategoryName() {
			string[] items = GetNameParts();
			if(items.Length < 2)
				return null;
			string categoryName = items[items.Length - 2];
			if(string.IsNullOrEmpty(categoryName)) return categoryName;
			StringBuilder builder = new StringBuilder(categoryName);
			for(int i = 0; i < builder.Length; i++) {
				if(i == 0 || (i > 0 && builder[i - 1] == ' ')) {
					if(char.IsLower(builder[i])) builder[i] = char.ToUpper(builder[i]);
				}
			}
			return builder.ToString();
		}
		public string RelatedName {
			get {
				if(ItemSize == DXImageGalleryItemSize.Small)
					return Name.Replace(SmallItemSizeTag, LargeItemSizeTag);
				return Name.Replace(LargeItemSizeTag, SmallItemSizeTag);
			}
		}
		public virtual bool MatchFilter(string filterString) {
			for(int i = 0; i < Tags.Length; i++) {
				string tag = Tags[i];
				if(tag.StartsWith(filterString, StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}
		public readonly string SmallItemSizeTag = "16x16";
		public readonly string LargeItemSizeTag = "32x32";
	}
	public class DXImageGalleryStorage {
		bool isLoaded;
		DXImageGalleryDataModel dataModel;
		protected DXImageGalleryStorage() {
			this.isLoaded = false;
			this.dataModel = null;
		}
		static DXImageGalleryStorage defaultCore = null;
		public static DXImageGalleryStorage Default {
			get {
				if(defaultCore == null) {
					defaultCore = new DXImageGalleryStorage();
				}
				return defaultCore;
			}
		}
		public void Load() {
			if(IsLoaded) return;
			this.dataModel = DXImageGalleryLoader.Load();
			this.isLoaded = true;
		}
		IAsyncResult ar = null;
		public void LoadAsync() {
			if(IsLoaded) return;
			Func<DXImageGalleryDataModel> callback = DXImageGalleryLoader.Load;
			this.ar = callback.BeginInvoke(null, callback);
		}
		public void CheckLoaded() {
			if(IsLoaded) return;
			if(this.ar == null) {
				throw new InvalidImageSizeException("You must call LoadAsync first");
			}
			Func<DXImageGalleryDataModel> callback = (Func<DXImageGalleryDataModel>)this.ar.AsyncState;
			dataModel = callback.EndInvoke(this.ar);
			this.isLoaded = true;
		}
		public bool IsLoaded { get { return isLoaded; } }
		public DXImageGalleryDataModel DataModel {
			get {
				CheckLoaded();
				return dataModel;
			}
		}
	}
	public static class DXImageGalleryLoader {
		public static DXImageGalleryDataModel Load() {
			DXImageGalleryDataModel dataModel = new DXImageGalleryDataModel();
			using(ResourceReader reader = GetResourceReader(DxImageAssemblyUtil.ImageAssembly)) {
				IDictionaryEnumerator dict = reader.GetEnumerator();
				while(dict.MoveNext()) {
					string key = (string)dict.Key as string;
					if(!DxImageAssemblyUtil.ImageProvider.IsBrowsable(key)) continue;
					if(IsImageBasedResource(key)) {
						Image image = GetImageFromStream((Stream)dict.Value);
						if(image != null) DoAddItem(dataModel, DxImageAssemblyUtil.ImageProvider, key, image);
					}
				}
			}
			dataModel.Sort();
			return dataModel;
		}
		static void DoAddItem(DXImageGalleryDataModel dataModel, IDXImagesProvider provider, string key, Image image) {
			DXImageGalleryItemType itemType = DXImageGalleryItemType.Colored;
			if(provider.IsGrayScaledImage(key)) itemType = DXImageGalleryItemType.GrayScaled;
			if(provider.IsOffice2013Image(key)) itemType = DXImageGalleryItemType.Office2013;
			if(provider.IsDevAVImage(key)) itemType = DXImageGalleryItemType.DevAV;
			dataModel.AddItem(new DXImageGalleryItem(key, image, itemType));
		}
		static bool IsImageBasedResource(string key) {
			return key.EndsWith(".png", StringComparison.Ordinal);
		}
		static ResourceReader GetResourceReader(Assembly imagesAssembly) {
			var resources = imagesAssembly.GetManifestResourceNames();
			var imageResources = Array.FindAll(resources, resourceName => resourceName.EndsWith(".resources"));
			if(imageResources.Length != 1) {
				throw new CannotFindImageResourceException();
			}
			return new ResourceReader(imagesAssembly.GetManifestResourceStream(imageResources[0]));
		}
		static Image GetImageFromStream(Stream stream) {
			Image res = null;
			try {
				res = Image.FromStream(stream);
			}
			catch { res = null; }
			return res;
		}
	}
	#region Exceptions
	public class InvalidImageSizeException : Exception {
		public InvalidImageSizeException(string msg)
			: base(msg) {
		}
	}
	public class CannotFindImageResourceException : Exception {
		public CannotFindImageResourceException() {
		}
	}
	#endregion
}
