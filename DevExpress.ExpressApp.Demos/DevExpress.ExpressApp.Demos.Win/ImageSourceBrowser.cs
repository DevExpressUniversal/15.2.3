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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.Demos.Win {
	[NavigationItem(false)]
	[DomainComponent]
	[System.ComponentModel.DisplayName("Category")]
	public class CategoryString {
		private ImageSourceBrowserBase owner;
		private BindingList<ImagePreviewObject> images;
		public CategoryString(ImageSourceBrowserBase owner, string name) {
			Name = name;
			this.owner = owner;
		}
		[ReadOnly(true)]
		public string Name { get; set; }
		public IList<ImagePreviewObject> Images {
			get {
				if(images == null) {
					images = new BindingList<ImagePreviewObject>();
					if(owner != null) {
						foreach(string imageName in owner.AllCategories[Name]) {
							ImagePreviewObject imagePreview = new ImagePreviewObject(owner, imageName);
							images.Add(imagePreview);
						}
					}
				}
				return images;
			}
		}
	}
	[NavigationItem("Images"), System.ComponentModel.DisplayName("Image Source Browser")]
	public abstract class ImageSourceBrowserBase : BaseObject {
		private IDictionary<string, IList<string>> allCategories;
		private ImageSource imageSource = null;
		private string imageSourceName;
		private List<string> imageNames;
		private BindingList<CategoryString> categories = new BindingList<CategoryString>();
		private BindingList<ImageBrowserCategory> treeCategories = new BindingList<ImageBrowserCategory>();
		private bool IsImageNameWithSuffix(string imageName) {
			return imageName.EndsWith(ImageLoader.SmallImageSuffix)
							|| imageName.EndsWith(ImageLoader.LargeImageSuffix)
							|| imageName.EndsWith(ImageLoader.DialogImageSuffix)
							|| imageName.EndsWith("_Large")
							|| imageName.EndsWith("_32x32");
		}
		internal void BuildTreeNodes() {
			treeCategories.Clear();
			allCategories = new Dictionary<string, IList<string>>();
			List<string> rootCategories = new List<string>();
			foreach(string imageName in ImageNames) {
				foreach(CategoryString category in Categories) {
					string categoryName = category.Name;
					IList<string> categoryContent;
					if(!allCategories.TryGetValue(categoryName, out categoryContent)) {
						categoryContent = new List<string>();
						allCategories.Add(categoryName, categoryContent);
					}
					if(("_" + imageName + "_").Contains("_" + categoryName + "_")) {
						categoryContent.Add(imageName);
					}
					if(imageName.StartsWith(categoryName) && !rootCategories.Contains(categoryName)) {
						rootCategories.Add(categoryName);
					}
				}
			}
			List<string> simpleCategories = new List<string>();
			foreach(KeyValuePair<string, IList<string>> category in allCategories) {
				if(category.Value.Count < 2) {
					simpleCategories.Add(category.Key);
				}
			}
			foreach(string category in simpleCategories) {
				CategoryString categoryString = null;
				foreach(CategoryString cat in Categories) {
					if(cat.Name == category) {
						categoryString = cat;
						break;
					}
				}
				Categories.Remove(categoryString);
				allCategories.Remove(category);
				if(rootCategories.Contains(category)) {
					rootCategories.Remove(category);
				}
			}
			ImagesGroupNode rootCategory = new ImagesGroupNode(this, null, "");
			rootCategory.FillImageCategoriesTree(allCategories, ImageNames, rootCategories);
			ImageCategories.Add(rootCategory);
		}
		internal void AddImageCategories(List<string> imageNames) {
			List<string> rootCategories = new List<string>();
			foreach(string imageName in imageNames) {
				foreach(string category in imageName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)) {
					if(!rootCategories.Contains(category)) {
						rootCategories.Add(category);
						CategoryString categoryString = null;
						foreach(CategoryString cat in Categories) {
							if(cat.Name == category) {
								categoryString = cat;
								break;
							}
						}
						if(categoryString == null) {
							categoryString = new CategoryString(this, category);
							Categories.Add(categoryString);
						}
					}
				}
			}
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			if(!string.IsNullOrEmpty(ImageSourceName)) {
				AddImageCategories(ImageSource.GetImageNames());
				BuildTreeNodes();
			}
		}
		protected abstract ImageSource CreateImageSource(string imageSource);
		[Browsable(false)]
		public ImageSource ImageSource {
			get {
				if(imageSource == null) {
					imageSource = CreateImageSource(ImageSourceName);
				}
				return imageSource;
			}
		}
		[Browsable(false)]
		public List<string> ImageNames {
			get {
				if(imageNames == null) {
					imageNames = new List<string>();
					foreach(string imageName in ImageSource.GetImageNames()) {
						if(!IsImageNameWithSuffix(imageName)) {
							imageNames.Add(imageName);
						}
					}
				}
				return imageNames;
			}
		}
		public ImageSourceBrowserBase(Session session) : base(session) { }
		public ImageSourceBrowserBase(Session session, string imageSourceName)
			: base(session) {
			this.imageSourceName = imageSourceName;
		}
		public string ImageSourceName {
			get { return imageSourceName; }
			set {
				SetPropertyValue("ImageSourceName", ref imageSourceName, value);
				imageSource = null;
				if(imageNames != null) {
					imageNames.Clear();
				}
				imageNames = null;
			}
		}
		public BindingList<ImageBrowserCategory> ImageCategories {
			get { return treeCategories; }
		}
		public BindingList<CategoryString> Categories {
			get { return categories; }
		}
		[Browsable(false)]
		public IDictionary<string, IList<string>> AllCategories {
			get { return allCategories; }
		}
	}
	public class FileImageSourceBrowser : ImageSourceBrowserBase {
		protected override ImageSource CreateImageSource(string imageSource) {
			return new FileImageSource(imageSource);
		}
		public FileImageSourceBrowser(Session session) : base(session) { }
		public FileImageSourceBrowser(Session session, string imageSourceName) : base(session, imageSourceName) { }
	}
	public class AssemblyImageSourceBrowser : ImageSourceBrowserBase {
		protected override ImageSource CreateImageSource(string imageSource) {
			return new AssemblyResourceImageSource(imageSource);
		}
		public AssemblyImageSourceBrowser(Session session) : base(session) { }
		public AssemblyImageSourceBrowser(Session session, string imageSourceName) : base(session, imageSourceName) { }
	}
}
