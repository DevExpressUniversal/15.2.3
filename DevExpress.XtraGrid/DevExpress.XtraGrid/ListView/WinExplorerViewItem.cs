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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraGrid.WinExplorer {
	public class WinExplorerViewItem : INotifyPropertyChanged {
		WinExplorerViewItemGroup group;
		int smallImageIndex = -1;
		public WinExplorerViewItem() {
			ImageIndex = -1;
			ExtraLargeImageIndex = -1;
			LargeImageIndex = -1;
			IsEnabled = true;
		}
		public WinExplorerViewItem(string text) :
			this(text, -1) { }
		public WinExplorerViewItem(string text, Image image) :
			this(text, (string)null, image) { }
		public WinExplorerViewItem(string text, int imageIndex) :
			this(text, (string)null, imageIndex) { }
		public WinExplorerViewItem(string text, string description) :
			this(text, description, false) { }
		public WinExplorerViewItem(string text, string description, bool isChecked) :
			this(text, description, isChecked, null) { }
		public WinExplorerViewItem(string text, string description, Image image) :
			this(text, description, false, image) { }
		public WinExplorerViewItem(string text, string description, int imageIndex) :
			this(text, description, false, imageIndex) { }
		public WinExplorerViewItem(string text, string description, bool isChecked, Image image, Image extraLargeImage, Image largeImage, Image smallImage) : 
			this(text, description, isChecked, image, extraLargeImage, largeImage, smallImage, -1, -1, -1, -1) { }
		public WinExplorerViewItem(string text, string description, bool isChecked, int imageIndex, int extraLargeImageIndex, int largeImageIndex, int smallImageIndex) :
			this(text, description, isChecked, null, null, null, null, imageIndex, extraLargeImageIndex, largeImageIndex, smallImageIndex) { }
		public WinExplorerViewItem(string text, string description, bool isChecked, Image image) :
			this(text, description, false, image, null, null, null) { }
		public WinExplorerViewItem(string text, string description, bool isChecked, int imageIndex) :
			this(text, description, false, imageIndex, -1, -1, -1) { }
		public WinExplorerViewItem(string text, string description, bool isChecked, Image image, Image extraLargeImage, Image largeImage, Image smallImage, int imageIndex, int extraLargeImageIndex, int largeImageIndex, int smallImageIndex) {
			Text = text;
			Description = description;
			IsChecked = isChecked;
			Image = image;
			ExtraLargeImage = largeImage;
			LargeImage = largeImage;
			SmallImage = smallImage;
			ImageIndex = ImageIndex;
			ExtraLargeImageIndex = extraLargeImageIndex;
			LargeImageIndex = largeImageIndex;
			this.smallImageIndex = smallImageIndex;
		}
		internal const string
			TextColumn = "Text",
			GroupColumn = "Group",
			DescriptionColumn = "Description",
			CheckColumn = "IsChecked",
			EnabledColumn = "IsEnabled",
			ImageColumn = "Image",
			ExtraLargeImageColumn = "ExtraLargeImage",
			LargeImageColumn = "LargeImage",
			SmallImageColumn = "SmallImage",
			ImageIndexColumn = "ImageIndex",
			ExtraLargeImageIndexColumn = "ExtraLargeImageIndex",
			LargeImageIndexColumn = "LargeImageIndex",
			SmallImageIndexColumn = "smallImageIndex";
		string text = null;
		[DefaultValue(null)]
		public string Text {
			get { return text; }
			set {
				if(Text == value)
					return;
				text = value;
				OnPropertyChanged("Text");
			}
		}
		string description = null;
		[DefaultValue(null)]
		public string Description {
			get { return description; }
			set {
				if(Description == value)
					return;
				description = value;
				OnPropertyChanged("Description");
			}
		}
		bool isChecked;
		[DefaultValue(false)]
		public bool IsChecked {
			get { return isChecked; }
			set {
				if(IsChecked == value)
					return;
				isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
		bool isEnabled;
		[DefaultValue(true)]
		public bool IsEnabled {
			get { return isEnabled; }
			set {
				if(IsEnabled == value)
					return;
				isEnabled = value;
				OnPropertyChanged("IsEnabled");
			}
		}
		Image image;
		[DefaultValue(null)]
		public Image Image {
			get { return image; }
			set {
				if(Image == value)
					return;
				image = value;
				OnPropertyChanged("Image");
			}
		}
		Image extraLargeImage;
		[DefaultValue(null)]
		public Image ExtraLargeImage {
			get { return extraLargeImage; }
			set {
				if(ExtraLargeImage == value)
					return;
				extraLargeImage = value;
				OnPropertyChanged("ExtraLargeImage");
			}
		}
		Image largeImage;
		[DefaultValue(null)]
		public Image LargeImage {
			get { return largeImage; }
			set {
				if(LargeImage == value)
					return;
				largeImage = value;
				OnPropertyChanged("LargeImage");
			}
		}
		Image smallImage;
		[DefaultValue(null)]
		public Image SmallImage {
			get { return smallImage; }
			set {
				if(SmallImage == value)
					return;
				smallImage = value;
				OnPropertyChanged("SmallImage");
			}
		}
		int imageIndex = -1;
		[DefaultValue(-1)]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(value < 0) value = -1;
				if(ImageIndex == value)
					return;
				imageIndex = value;
				OnPropertyChanged("ImageIndex");
			}
		}
		int extraLargeImageIndex = -1;
		[DefaultValue(-1)]
		public int ExtraLargeImageIndex {
			get { return extraLargeImageIndex; }
			set {
				if(value < 0) value = -1;
				if(ExtraLargeImageIndex == value)
					return;
				extraLargeImageIndex = value;
				OnPropertyChanged("ExtraLargeImageIndex");
			}
		}
		int largeImageIndex = -1;
		[DefaultValue(-1)]
		public int LargeImageIndex {
			get { return largeImageIndex; }
			set {
				if(value < 0) value = -1;
				if(LargeImageIndex == value)
					return;
				largeImageIndex = value;
				OnPropertyChanged("LargeImageIndex");
			}
		}
		[DefaultValue(-1)]
		public int SmallImageIndex {
			get { return smallImageIndex; }
			set {
				if(value < 0) value = -1;
				if(SmallImageIndex == value) return;
				smallImageIndex = value;
				OnPropertyChanged("SmallImageIndex");
			}
		}
		public WinExplorerViewItemGroup Group {
			get { return group; }
			set {
				if(group == value) return;
				if(group != null) group.OnRemoveItem(this);
				group = value;
				if(group != null) group.OnAddItem(this);
				OnPropertyChanged("Group");
			}
		}
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler propertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { propertyChanged += value; }
			remove { propertyChanged -= value; }
		}
		void OnPropertyChanged(string name) {
			if(propertyChanged != null) propertyChanged(this, new PropertyChangedEventArgs(name));
		}
		#endregion
	}
	public class WinExplorerViewItemCollection : ReadOnlyCollection<WinExplorerViewItem> {
		public WinExplorerViewItemCollection() : base(new BindingList<WinExplorerViewItem>()) {
		}
		protected internal BindingList<WinExplorerViewItem> ItemsList { get { return (BindingList<WinExplorerViewItem>)Items; } }
	}
	public class WinExplorerViewItemGroup : INotifyPropertyChanged, IComparable {
		WinExplorerViewItemCollection items;
		string name;
		internal void OnAddItem(WinExplorerViewItem item) {
			if(!Items.Contains(item))  Items.ItemsList.Add(item);
		}
		internal void OnRemoveItem(WinExplorerViewItem item) {
			if(Items.Contains(item)) Items.ItemsList.Remove(item);
		}
		public WinExplorerViewItemCollection Items {
			get {
				if(items == null) items = new WinExplorerViewItemCollection();
				return items;
			}
		}
		public string Name {
			get { return name; }
			set {
				if(value == null) value = "";
				if(value == Name) return;
				name = value;
				OnPropertyChanged("Name");
			}
		}
		public override string ToString() {
			return Name;
		}
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler propertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { propertyChanged += value; }
			remove { propertyChanged -= value; }
		}
		void OnPropertyChanged(string name) {
			if(propertyChanged != null) propertyChanged(this, new PropertyChangedEventArgs(name));
		}
		#endregion
		#region IComparable Members
		int IComparable.CompareTo(object obj) {
			return Comparer.Default.Compare(Name, ((WinExplorerViewItemGroup)obj).Name);
		}
		#endregion
	}
	public class WinExplorerViewDataSource : IListSource {
		BindingList<WinExplorerViewItem> items;
		BindingList<WinExplorerViewItemGroup> groups;
		public WinExplorerViewDataSource() {
			this.items = new BindingList<WinExplorerViewItem>();
			this.groups = new BindingList<WinExplorerViewItemGroup>();
		}
		bool IListSource.ContainsListCollection { get { return true; } }
		System.Collections.IList IListSource.GetList() { return Items; }
		public BindingList<WinExplorerViewItem> Items { get { return items; } }
		public BindingList<WinExplorerViewItemGroup> Groups { get { return groups; } } 
	}
}
