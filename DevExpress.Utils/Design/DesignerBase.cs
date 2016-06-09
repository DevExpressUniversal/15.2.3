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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Data.Utils;
namespace DevExpress.Utils.Design {
	public class DesignerGroup : CollectionBase, IDisposable {
		string caption, description;
		bool defaultExpanded;
		Image image;
		object tag;
		public DesignerGroup(string caption, string description, Image image) : this(caption, description, image, false) { }
		public DesignerGroup(string caption, string description, Image image, bool defaultExpanded) {
			this.defaultExpanded = defaultExpanded;
			this.caption = caption;
			this.description = description;
			this.image = image;
			this.tag = null;
		}
		public virtual void Dispose() {
			image = null;
			this.Clear();
		}
		public object Tag { get { return tag; } set { tag = value; } }
		public bool DefaultExpanded { get { return defaultExpanded; } set { defaultExpanded = value; } }
		public string Caption { get { return caption; } set { caption = value; } }
		public string Description { get { return description; } set { description = value; } }
		public Image Image { get { return image; } set { image = value; } }
		public DesignerItem this[int index] { get { return List[index] as DesignerItem; } }
		public virtual int Add(DesignerItem item) {
			return List.Add(item);
		}
		public virtual void Insert(int index, DesignerItem item) {
			List.Insert(index, item);
		}
		public DesignerItem Add(string caption, string description, string frameTypeName, Image largeImage, Image smallImage) {
			DesignerItem item = new DesignerItem(caption, description, frameTypeName, largeImage, smallImage);
			Add(item);
			return item;
		}
		public DesignerItem Add(string caption, string description, Type frameType, Image largeImage, Image smallImage, object tag) {
			DesignerItem item = new DesignerItem(caption, description, frameType, largeImage, smallImage, tag);
			Add(item);
			return item;
		}
		public DesignerItem Add(string caption, string description, string frameTypeName, Image largeImage, Image smallImage, object tag) {
			DesignerItem item = new DesignerItem(caption, description, frameTypeName, largeImage, smallImage, tag);
			Add(item);
			return item;
		}
		public DesignerItem Add(string caption, string description, string frameTypeName, Image largeImage, Image smallImage, object tag, bool showInVerbs) {
			DesignerItem item = new DesignerItem(caption, description, frameTypeName, largeImage, smallImage, tag, showInVerbs);
			Add(item);
			return item;
		}
		public DesignerItem Insert(int index, string caption, string description, string frameTypeName, Image largeImage, Image smallImage, object tag) {
			DesignerItem item = new DesignerItem(caption, description, frameTypeName, largeImage, smallImage, tag);
			Insert(index, item);
			return item;
		}
		public int IndexOf(DesignerItem item) { return List.IndexOf(item); }
		public DesignerItem GetItemByCaption(string caption) {
			foreach(DesignerItem item in this) {
				if(item.Caption == caption) return item;
			}
			return null;
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n --) RemoveAt(n);
		}
		protected override void OnRemoveComplete(int index, object val) {
			DesignerItem item = val as DesignerItem;
			if(item != null) {
				item.Dispose();
				item.SetGroup(null);
			}
		}
		protected override void OnInsertComplete(int index, object val) {
			DesignerItem item = val as DesignerItem;
			if(item != null) item.SetGroup(this);
		}
	}
	public class DesignerGroupCollection : CollectionBase {
		public DesignerGroup this[int index] { get { return List[index] as DesignerGroup; } }
		public DesignerGroup this[string caption] { 
			get { 
				foreach(DesignerGroup group in this) {
					if(group.Caption == caption) return group;
				}
				return null;
			}
		}
		public virtual int Add(DesignerGroup group) {
			return List.Add(group);
		}
		public DesignerGroup Add(string caption, string description, Image image, bool defaultExpanded) {
			DesignerGroup group = new DesignerGroup(caption, description, image, defaultExpanded);
			Add(group);
			return group;
		}
		public DesignerGroup Add(string caption, string description, Image image) { return Add(caption, description, image, false); }
		public int IndexOf(DesignerGroup group) { return List.IndexOf(group); }
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n --) RemoveAt(n);
		}
		protected override void OnRemoveComplete(int index, object item) {
			DesignerGroup group = item as DesignerGroup;
			if(group != null) group.Dispose();
		}
	}
	public class DesignerItem : IDisposable {
		string frameTypeName;
		Type frameType;
		string caption, description;
		Image largeImage;
		Image smallImage;
		object tag;
		bool showInVerbs;
		EventHandler clickEvent = null;
		DesignerGroup group = null;
		public DesignerItem() : this("", "", "", null, null) { }
		public DesignerItem(string caption, string description, EventHandler clickEvent, Image largeImage, Image smallImage) :  this(caption, description, "", largeImage, smallImage, null) { 
			this.clickEvent = clickEvent;
		}
		public DesignerItem(string caption, string description, string frameType, Image largeImage, Image smallImage) :  this(caption, description, frameType, largeImage, smallImage, null) { }
		public DesignerItem(string caption, string description, string frameType, Image largeImage, Image smallImage, object tag) : this(caption, description, typeof(object), largeImage, smallImage, tag) {
			this.frameTypeName = frameType;
		}
		public DesignerItem(string caption, string description, Type frameType, Image largeImage, Image smallImage, object tag) : 
			this(caption, description, frameType, largeImage, smallImage, tag, true) { }
		public DesignerItem(string caption, string description, string frameTypeName, Image largeImage, Image smallImage, object tag, bool showInVerbs) :
			this(caption, description, typeof(object), largeImage, smallImage, tag, showInVerbs) { 
			this.frameTypeName = frameTypeName;
		}
		public DesignerItem(string caption, string description, Type frameType, Image largeImage, Image smallImage, object tag, bool showInVerbs) {
			this.showInVerbs = showInVerbs;
			this.caption = caption;
			this.description = description;
			this.largeImage = largeImage;
			this.smallImage = smallImage;
			this.frameTypeName = "";
			if(frameType != null && frameType.Equals(typeof(object))) frameType = null;
			this.frameType = frameType;
			this.tag = tag;
		}
		public virtual void Dispose() {
			this.smallImage = null;
			this.largeImage = null;
		}
		internal void SetGroup(DesignerGroup group) { this.group = group; } 
		public EventHandler ClickEvent { get { return clickEvent; } }
		public DesignerGroup Group { get { return group; } }
		public string Caption { get { return caption; } set { caption = value; } }
		public string Description { get { return description; } set { description = value; } }
		public Image LargeImage { get { return largeImage; } set { largeImage = value; } }
		public Image SmallImage { get { return smallImage; } set { smallImage = value; } }
		public bool ShowInVerbs { get { return showInVerbs; } set { showInVerbs = value; } }
		public string FrameTypeName { get { return frameTypeName; } set { frameTypeName = value; } }
		public Type FrameType { get { return frameType; } set { frameType = value; } }
		public object Tag { get { return tag; } set { tag = value; } }
	}
	public class BaseDesigner : IDisposable {
		DesignerGroupCollection groups;
		bool initialized = false;
		public static ImageList CreateImageListXXX(int x, int y, ColorDepth cd, string s) {
			ImageList iml = new ImageList();
			iml.ColorDepth = cd;
			iml.ImageSize = new System.Drawing.Size(x, y);
			try {
				System.IO.Stream str = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(s);
				iml.Images.AddStrip(ImageTool.ImageFromStream(str));
			} catch {}
			iml.TransparentColor = System.Drawing.Color.Magenta;
			return iml;
		}
		static BaseDesigner() {
		}
		public BaseDesigner() {
			this.groups = CreateGroupCollection();
		}
		void CheckInit() {
			if(this.initialized) return;
			this.initialized = true;
			Init();
		}
		public virtual void Init() {
			this.initialized = true;
			CreateGroups();
		}
		protected virtual object LargeImageList { get { return null; } }
		protected virtual object SmallImageList { get { return null; } }
		protected virtual object GroupImageList { get { return null; } }
		protected virtual DesignerGroupCollection CreateGroupCollection() {
			return new DesignerGroupCollection();
		}
		public virtual void Dispose() {
			if(groups != null) groups.Clear();
		}
		public DesignerGroupCollection Groups { 
			get { 
				CheckInit();
				return groups; 
			} 
		}
		protected virtual void CreateGroups() {
			Groups.Clear();
		}
		protected virtual Image GetDefaultLargeImage(int index) {
			return ImageCollection.GetImageListImage(LargeImageList, index);
		}
		protected virtual Image GetDefaultSmallImage(int index) {
			return ImageCollection.GetImageListImage(SmallImageList, index);
		}
		protected virtual Image GetDefaultGroupImage(int index) {
			return ImageCollection.GetImageListImage(GroupImageList, index);
		}
		public virtual DesignerGroup DefaultGroup { get { return Groups.Count > 0 ? Groups[0] : null; } }
		public virtual DesignerItem DefaultItem { get { return DefaultGroup == null || DefaultGroup.Count == 0 ? null : DefaultGroup[0]; } }
		public virtual DesignerItem ItemByName(string itemName) {
			foreach(DesignerGroup group in Groups) {
				foreach(DesignerItem item in group) {
					if(item.Caption == itemName) return item;
				}
			}
			return null;
		}
	}
}
