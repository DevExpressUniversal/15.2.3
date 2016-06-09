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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.VisualEffects;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Native;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors {
	public enum TileItemContentAlignment { Default, TopLeft, TopCenter, TopRight, MiddleLeft, MiddleCenter, MiddleRight, BottomLeft, BottomCenter, BottomRight, Manual }
	[
	ToolboxItem(false), DesignTimeVisible(false),
	SmartTagSupport(typeof(TileGroupDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto),
	SmartTagAction(typeof(TileGroupDesignTimeActionsProvider), "AddGroup", "Add Group", SmartTagActionType.RefreshAfterExecute),
	SmartTagAction(typeof(TileGroupDesignTimeActionsProvider), "RemoveGroup", "Remove Group", SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(TileGroupDesignTimeActionsProvider), "AddMediumItem", "Add Medium Item", SmartTagActionType.RefreshAfterExecute),
	SmartTagAction(typeof(TileGroupDesignTimeActionsProvider), "AddSmallItem", "Add Small Item", SmartTagActionType.RefreshAfterExecute),
	SmartTagAction(typeof(TileGroupDesignTimeActionsProvider), "AddWideItem", "Add Wide Item", SmartTagActionType.RefreshAfterExecute),
	SmartTagAction(typeof(TileGroupDesignTimeActionsProvider), "AddLargeItem", "Add Large Item", SmartTagActionType.RefreshAfterExecute),
	Designer("DevExpress.XtraBars.Design.TileGroupDesigner, " + AssemblyInfo.SRAssemblyBarsDesign)
	]
	public class TileGroup : Component {
		protected internal TileGroupCollection Groups { get; set; }
		internal List<TileItem> OriginItems {
			get;
			set;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TileGroupViewInfo GroupInfo {
			get;
			set;
		}
		TileItemCollection items;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Collection, false, true, false)]
		public TileItemCollection Items {
			get {
				if(items == null)
					items = CreateItems();
				return items;
			}
		}
		[
		DefaultValue(null), Category(CategoryName.Data), SmartTagProperty("Tag", "", 0),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public object Tag {
			get;
			set;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ISite Site {
			get { return base.Site; }
			set {
				if(base.Site != value) {
					base.Site = value;
					if(value == null || string.IsNullOrEmpty(name) || DesignMode) return;
					try {
						Site.Name = name;
					}
					catch { }
				}
			}
		}
		string name = String.Empty;
		[ Browsable(false), XtraSerializableProperty]
		public virtual string Name {
			get {
				string n = GetNameCore();
				return string.IsNullOrEmpty(n) ? String.Empty : n;
			}
			set {
				if(Site != null) {
					Site.Name = value;
					name = Site.Name;
				}
				else {
					name = value;
				}
			}
		}
		string GetNameCore() {
			if(Site == null) return name;
			return Site.Name;
		}
		protected virtual TileItemCollection CreateItems() {
			return new TileItemCollection(this);
		}
		[Browsable(false)]
		public ITileControl Control { get { return Groups == null ? null : Groups.Owner; } }
		protected internal virtual TileGroupViewInfo CreateViewInfo() {
			return new TileGroupViewInfo(this);
		}
		internal TileItem XtraFindItemsItem(XtraItemEventArgs e) {
			int id = int.Parse(e.Item.ChildProperties["Id"].Value.ToString());
			foreach(TileItem item in OriginItems) {
				if(item.Id == id) {
					Items.Add(item);
					AddToDeserializedItems(item);
					return item;
				}
			}
			return null;
		}
		void AddToDeserializedItems(TileItem item) {
			if(this.Groups == null || (this.Groups.Owner as TileControl) == null)
				return;
			((TileControl)this.Groups.Owner).DeserializedItems.Add(item);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				List<TileItem> itemsCore = new List<TileItem>();
				foreach(TileItem item in Items) {
					itemsCore.Add(item);
				}
				Items.Clear();
				foreach(TileItem item in itemsCore) {
					item.Dispose();
				}
				Items.Dispose();
				if(OriginItems != null)
					OriginItems.Clear();
				OriginItems = null;
			}
			base.Dispose(disposing);
		}
		string text = String.Empty;
		[DefaultValue(""), XtraSerializableProperty, SmartTagProperty("Text", "Appearance", 0, SmartTagActionType.RefreshAfterExecute)]
		public string Text {
			get { return text; }
			set {
				if(Text == value)
					return;
				text = value;
				if(Control != null)
					Control.OnPropertiesChanged();
			}
		}
		bool visible = true;
		[DefaultValue(true), SmartTagProperty("Visible", "Appearance", 10), XtraSerializableProperty]
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value)
					return;
				visible = value;
				OnVisibilityChanged();
				if(Control != null)
					Control.OnPropertiesChanged();
			}
		}
		void OnVisibilityChanged() {
			if(GroupInfo == null) return;
			GroupInfo.OnVisibilityChanged();
		}
		public TileItem GetTileItemByText(string str) {
			foreach(TileItem item in Items)
				if(item.Text == str) return item;
			return null;
		}
		public TileItem GetTileItemByName(string str) {
			foreach(TileItem item in Items)
				if(item.Name == str) return item;
			return null;
		}
	}
	public class TileItemEventArgs : EventArgs {
		public TileItem Item { get; set; }
	}
	public delegate void TileItemClickEventHandler(object sender, TileItemEventArgs e);
	public class TileItemDragEventArgs : CancelEventArgs {
		public TileItem Item { get; set; }
		public TileGroup TargetGroup { get; protected internal set; }
	}
	public delegate void TileItemDragEventHandler(object sender, TileItemDragEventArgs e);
	public class TileItemFrame : INotifyElementPropertiesChanged {
		public TileItemFrame() {
			AnimateBackgroundImage = true;
			AnimateText = true;
			AnimateImage = true;
			UseBackgroundImage = true;
			UseText = true;
			UseImage = true;
			Animation = TileItemContentAnimationType.Default;
		}
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		AppearanceObject appearance;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public AppearanceObject Appearance {
			get {
				if(appearance == null) {
					appearance = new AppearanceObject();
				}
				return appearance;
			}
		}
		protected internal ITileItem Owner { get; set; }
		protected internal int Index { get; set; }
		[DefaultValue(TileItemContentAnimationType.Default), XtraSerializableProperty]
		public TileItemContentAnimationType Animation { get; set; }
		TileItemElementCollection items;
		void ResetItems() { Elements.Clear(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Collection, false, true, false)]
		public TileItemElementCollection Elements {
			get {
				if(items == null)
					items = CreateElementCollection();
				return items;
			}
			set {
				if(items == value)
					return;
				TileItemElementCollection prev = Elements;
				items = value;
				OnElementsCollectionChanged(prev);
			}
		}
		internal TileItemElement XtraFindElementsItem(XtraItemEventArgs e) {
			int index = int.Parse(e.Item.Name.Replace("Item", ""));
			if(Elements.Count >= index) {
				return Elements[index - 1];
			}
			return null;
		}
		protected virtual TileItemElementCollection CreateElementCollection() {
			return new TileItemElementCollection(this);
		}
		protected internal TileItemFrameCollection Collection { get; set; }
		void OnElementsCollectionChanged(TileItemElementCollection prev) {
			if(prev != null)
				prev.Owner = null;
			Elements.Owner = this;
			INotifyElementPropertiesChanged reference = (INotifyElementPropertiesChanged)this;
			foreach(TileItemElement element in Elements)
				reference.OnElementPropertiesChanged(element);
		}
		void CreateElements(int count) {
			if(Elements.Count < count) {
				for(int i = Elements.Count; i < count; i++) {
					Elements.Add(CreateTileItemElement());
				}
			}
		}
		protected virtual TileItemElement CreateTileItemElement() {
			return new TileItemElement();
		}
		[DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image BackgroundImage { get; set; }
		[DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Image {
			get {
				if(Elements.Count < 1)
					return null;
				return Elements[0].Image;
			}
			set {
				if(Image == value)
					return;
				CreateElements(1);
				Elements[0].Image = value;
			}
		}
		[DefaultValue(null), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Text {
			get {
				if(Elements.Count < 1)
					return null;
				return Elements[0].Text;
			}
			set {
				CreateElements(1);
				Elements[0].Text = value;
			}
		}
		[DefaultValue(null), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Text2 {
			get {
				if(Elements.Count < 2)
					return null;
				return Elements[1].Text;
			}
			set {
				CreateElements(2);
				Elements[1].Text = value;
			}
		}
		[DefaultValue(null), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Text3 {
			get {
				if(Elements.Count < 3)
					return null;
				return Elements[2].Text;
			}
			set {
				CreateElements(3);
				Elements[2].Text = value;
			}
		}
		[DefaultValue(null), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Text4 {
			get {
				if(Elements.Count < 4)
					return null;
				return Elements[3].Text;
			}
			set {
				CreateElements(4);
				Elements[3].Text = value;
			}
		}
		[DefaultValue(true)]
		public bool AnimateBackgroundImage { get; set; }
		[DefaultValue(true)]
		public bool AnimateText { get; set; }
		[DefaultValue(true)]
		public bool AnimateImage { get; set; }
		[DefaultValue(0), XtraSerializableProperty]
		public int Interval { get; set; }
		[DefaultValue(true)]
		public bool UseBackgroundImage { get; set; }
		[DefaultValue(true)]
		public bool UseImage { get; set; }
		[DefaultValue(true)]
		public bool UseText { get; set; }
		[DefaultValue(null),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get;
			set;
		}
		#region INotifyTextElementPropertiesChanged Members
		void INotifyElementPropertiesChanged.OnElementPropertiesChanged(TileItemElement element) {
			if(Owner == null || Owner.CurrentFrameIndex != this.Index) return;
			Owner.SetContent(this, false);
		}
		#endregion
		#region INotifyElementPropertiesChanged Members
		ITileControl INotifyElementPropertiesChanged.TileControl {
			get {
				if(Collection == null || Collection.Owner == null)
					return null;
				return Collection.Owner.Control;
			}
		}
		#endregion
	}
	[Editor("DevExpress.XtraBars.Design.TileItemFrameEditor, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class TileItemFrameCollection : CollectionBase, IEnumerable<TileItemFrame> {
		public ITileItem Owner { get; private set; }
		public TileItemFrameCollection(ITileItem item) {
			Owner = item;
		}
		public int Add(TileItemFrame item) {
			item.Index = List.Count;
			item.Owner = this.Owner;
			return List.Add(item);
		}
		public void Insert(int index, TileItemFrame item) { List.Insert(index, item); }
		public void Remove(TileItemFrame item) { List.Remove(item); }
		public int IndexOf(TileItemFrame item) { return List.IndexOf(item); }
		public bool Contains(TileItemFrame item) { return List.Contains(item); }
		public TileItemFrame this[int index] { get { return (TileItemFrame)List[index]; } set { List[index] = value; } }
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			Owner.OnInfoChanged();
			((TileItemFrame)value).Collection = this;
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			if(IsNotTileContainer(Owner.Control))
				Owner.Control.OnPropertiesChanged();
			Owner.OnInfoChanged();
			((TileItemFrame)value).Collection = null;
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			if(IsNotTileContainer(Owner.Control))
				Owner.Control.OnPropertiesChanged();
			Owner.OnInfoChanged();
			((TileItemFrame)oldValue).Collection = null;
			((TileItemFrame)newValue).Collection = this;
		}
		int lockUpdate = 0;
		internal bool LockUpdate { get { return lockUpdate > 0; } }
		public void Assign(TileItemFrameCollection collection) {
			lockUpdate++;
			this.List.Clear();
			foreach(var item in collection.List)
				this.List.Add(item);
			if(IsNotTileContainer(Owner.Control))
				Owner.Control.OnPropertiesChanged();
			lockUpdate--;
			Owner.OnInfoChanged();
		}
		bool IsNotTileContainer(ITileControl control) {
			if(control == null) return false;
			return !control.GetType().Name.Equals("TileContainerInfo");
		}
		IEnumerator<TileItemFrame> IEnumerable<TileItemFrame>.GetEnumerator() {
			foreach(TileItemFrame frame in List)
				yield return frame;
		}
	}
	public enum TileItemImageScaleMode { Default, NoScale, Stretch, StretchVertical, StretchHorizontal, ZoomInside, ZoomOutside, Squeeze }
	public enum TileItemCheckMode { None, Single, Multiple }
	public enum TileItemElementImageBorderMode { Default, None, SingleBorder }
	public enum TileItemBorderVisibility { Auto, Always, Never }
	public class TileItemElement : ICloneable, IAppearanceOwner, IAnimatedItem, ISupportXtraAnimation, IDXImageUriClient {
		internal static Point DefaultLocation = new Point(0, 0);
		internal static Size DefaultImageSize = Size.Empty;
		public TileItemElement() {
			this.textLocation = DefaultLocation;
			this.imageLocation = DefaultLocation;
			this.imageSize = DefaultImageSize;
			this.AnchorElementIndex = -1;
			this.AnchorChildsList = new List<TileItemElement>();
			this.imageUri = CreateImageUriInstance();
			this.imageUri.Changed += ImageUriChanged;
		}
		int width = 0;
		[DefaultValue(0), Category(CategoryName.Appearance)]
		public int Width {
			get { return width; }
			set {
				if(Width == value)
					return;
				width = value;
				OnPropertiesChanged();
			}
		}
		int height = 0;
		[DefaultValue(0), Category(CategoryName.Appearance)]
		public int Height {
			get { return height; }
			set {
				if(Height == value)
					return;
				height = value;
				OnPropertiesChanged();
			}
		}
		bool stretchHorizontal = false;
		[DefaultValue(false), Category(CategoryName.Appearance)]
		public bool StretchHorizontal {
			get { return stretchHorizontal; }
			set {
				if(StretchHorizontal == value)
					return;
				stretchHorizontal = value;
				OnPropertiesChanged();
			}
		}
		bool stretchVertical = false;
		[DefaultValue(false), Category(CategoryName.Appearance)]
		public bool StretchVertical {
			get { return stretchVertical; }
			set {
				if(StretchVertical == value)
					return;
				stretchVertical = value;
				OnPropertiesChanged();
			}
		}
		TileItemElementImageBorderMode imgBorder = TileItemElementImageBorderMode.Default;
		[DefaultValue(TileItemElementImageBorderMode.Default), Category(CategoryName.Appearance)]
		public TileItemElementImageBorderMode ImageBorder {
			get { return imgBorder; }
			set {
				if(imgBorder == value) return;
				imgBorder = value;
				OnPropertiesChanged();
			}
		}
		Color imgBorderColor = Color.Empty;
		[Category(CategoryName.Appearance)]
		public Color ImageBorderColor {
			get { return imgBorderColor; }
			set {
				if(imgBorderColor == value) return;
				imgBorderColor = value;
				OnPropertiesChanged();
			}
		}
		void ResetImageBorderColor() { ImageBorderColor = Color.Empty; }
		bool ShouldSerializeImageBorderColor() { return !ImageBorderColor.IsEmpty; }
		bool useTextInTransition = true;
		[DefaultValue(true), Category(CategoryName.Appearance)]
		public bool UseTextInTransition {
			get { return useTextInTransition; }
			set {
				if(UseTextInTransition == value)
					return;
				useTextInTransition = value;
				OnPropertiesChanged();
			}
		}
		bool useImageInTransition = true;
		[DefaultValue(true), Category(CategoryName.Appearance)]
		public bool UseImageInTransition {
			get { return useImageInTransition; }
			set {
				if(UseImageInTransition == value)
					return;
				useImageInTransition = value;
				OnPropertiesChanged();
			}
		}
		DefaultBoolean animateTransition = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default), Category(CategoryName.Appearance)]
		public DefaultBoolean AnimateTransition {
			get { return animateTransition; }
			set {
				if(AnimateTransition == value)
					return;
				animateTransition = value;
				OnPropertiesChanged();
			}
		}
		TileItemContentAlignment imageAlignmentCore;
		[DefaultValue(TileItemContentAlignment.Default), Category(CategoryName.Appearance)]
		public TileItemContentAlignment ImageAlignment {
			get { return imageAlignmentCore; }
			set {
				if(ImageAlignment == value)
					return;
				imageAlignmentCore = value;
				OnPropertiesChanged();
			}
		}
		int imageToTextIndent = 6;
		[DefaultValue(6), Category(CategoryName.Appearance)]
		public int ImageToTextIndent {
			get { return imageToTextIndent; }
			set {
				if(ImageToTextIndent == value)
					return;
				imageToTextIndent = value;
				OnPropertiesChanged();
			}
		}
		TileControlImageToTextAlignment imageToTextAlignment = TileControlImageToTextAlignment.Default;
		[DefaultValue(TileControlImageToTextAlignment.Default), Category(CategoryName.Appearance)]
		public TileControlImageToTextAlignment ImageToTextAlignment {
			get { return imageToTextAlignment; }
			set {
				if(ImageToTextAlignment == value)
					return;
				imageToTextAlignment = value;
				OnPropertiesChanged();
			}
		}
		TileItemImageScaleMode imageScaleMode = TileItemImageScaleMode.Default;
		[DefaultValue(TileItemImageScaleMode.Default), Category(CategoryName.Appearance)]
		public TileItemImageScaleMode ImageScaleMode {
			get { return imageScaleMode; }
			set {
				if(ImageScaleMode == value)
					return;
				imageScaleMode = value;
				SetOptimizedImageDirty();
				OnPropertiesChanged();
			}
		}
		[Editor("DevExpress.XtraBars.Design.TileItemElementsSelectorEditor, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.Drawing.Design.UITypeEditor))]
		[DefaultValue(null), Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TileItemElement AnchorElement {
			get {
				if(AnchorElementIndex == -1 ||
					Collection == null ||
					AnchorElementIndex > Collection.Count - 1)
					return null;
				return Collection[AnchorElementIndex];
			}
			set {
				if(AnchorElement == value || Collection == null)
					return;
				CheckCircularReference(value);
				NotifyAnchorElement(AnchorElement, value);
				if(value == null || !Collection.Contains(value))
					AnchorElementIndex = -1;
				else
					AnchorElementIndex = Collection.IndexOf(value);
				OnPropertiesChanged();
			}
		}
		protected internal void NotifyAnchorElement(TileItemElement oldAnchor, TileItemElement newAnchor) {
			if(oldAnchor != null)
				oldAnchor.AnchorChildRemove(this);
			if(newAnchor != null)
				newAnchor.AnchorChildAdd(this);
		}
		void AnchorChildAdd(TileItemElement element) {
			if(!AnchorChildsList.Contains(element))
				AnchorChildsList.Add(element);
		}
		void AnchorChildRemove(TileItemElement element) {
			if(AnchorChildsList.Contains(element))
				AnchorChildsList.Remove(element);
		}
		int anchorElementIndexCore = -1;
		[DefaultValue(-1), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public int AnchorElementIndex {
			get { return anchorElementIndexCore; }
			set {
				if(anchorElementIndexCore == value)
					return;
				TileItemElement anchorElement = null;
				if(value > -1 && Collection != null && Collection.Count - 1 < value)
					anchorElement = Collection[value];
				NotifyAnchorElement(AnchorElement, anchorElement);
				CheckCircularReference(anchorElement);
				anchorElementIndexCore = value;
			}
		}
		protected internal List<TileItemElement> AnchorChildsList { get; set; }
		private void CheckCircularReference(TileItemElement value) {
			TileItemElement current = value;
			while(current != null) {
				current = current.AnchorElement;
				if(current != null && current == this)
					throw new ArgumentException("Circular anchor reference");
			}
		}
		AnchorAlignment anchorAlignment = AnchorAlignment.Default;
		[DefaultValue(AnchorAlignment.Default), Category(CategoryName.Appearance)]
		public AnchorAlignment AnchorAlignment {
			get { return anchorAlignment; }
			set {
				if(AnchorAlignment == value)
					return;
				anchorAlignment = value;
				OnPropertiesChanged();
			}
		}
		int anchorIndent = 6;
		[DefaultValue(6), Category(CategoryName.Appearance)]
		public int AnchorIndent {
			get { return anchorIndent; }
			set {
				if(AnchorIndent == value)
					return;
				anchorIndent = value;
				OnPropertiesChanged();
			}
		}
		void ResetAnchorOffset() { AnchorOffset = DefaultLocation; }
		bool ShouldSerializeAnchorOffset() { return !AnchorOffset.Equals(DefaultLocation); }
		Point anchorOffset = Point.Empty;
		[Category(CategoryName.Appearance)]
		public Point AnchorOffset {
			get { return anchorOffset; }
			set {
				if(AnchorOffset == value)
					return;
				anchorOffset = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public object Images {
			get {
				if(Collection == null || Collection.Owner == null || Collection.Owner.TileControl == null)
					return null;
				return Collection.Owner.TileControl.Images;
			}
		}
		int imageIndex = -1;
		[DefaultValue(-1),
		Category(CategoryName.Appearance),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)),
		DevExpress.Utils.ImageList("Images")]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex == value)
					return;
				imageIndex = value;
				OnPropertiesChanged();
			}
		}
		void ResetImageSize() { ImageSize = DefaultImageSize; }
		bool ShouldSerializeImageSize() { return !ImageSize.Equals(DefaultImageSize); }
		Size imageSize;
		[Category(CategoryName.Appearance)]
		public Size ImageSize {
			get { return imageSize; }
			set {
				if(ImageSize == value)
					return;
				imageSize = value;
				SetOptimizedImageDirty();
				OnPropertiesChanged();
			}
		}
		Image image;
		[DefaultValue(null), Category(CategoryName.Appearance), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Image {
			get { return image; }
			set {
				if(Image == value)
					return;
				if(value == null && image != null)
					StopGifAnimation();
				image = value;
				SetOptimizedImageDirty();
				UpdateGifHelper();
				OnPropertiesChanged();
			}
		}
		Image optimizedImage;
		protected internal Image OptimizedImage {
			get { return optimizedImage; }
			set {
				if(value == optimizedImage) return;
				if(value == null && optimizedImage != null)
					optimizedImage.Dispose();
				optimizedImage = value;
			}
		}
		bool needUpdateOptimizedImage;
		void SetOptimizedImageDirty() {
			needUpdateOptimizedImage = true;
		}
		protected virtual void UpdateOptimizedImage() {
			OptimizedImage = null;
			var itemInfo = GetItemInfo();
			if(Image == null || GifHelper.IsAnimated ||
			itemInfo == null || itemInfo.ContentBounds.IsEmpty)
				return;
			if(ImageScaleMode == TileItemImageScaleMode.Default ||
			ImageScaleMode == TileItemImageScaleMode.NoScale)
				return;
			if(!ImageSize.IsEmpty && (ImageSize.Width <= 0 || ImageSize.Height <= 0))
				return;
			Size boundsSize = ImageSize != Size.Empty ? ImageSize : itemInfo.ContentBounds.Size;
			Size fullsize = Size.Empty;
			Size resultSize = TileItemImageScaleModeHelper.ScaleImage(boundsSize, Image.Size, ImageScaleMode, out fullsize);
			if(!fullsize.IsEmpty)
				resultSize = fullsize;
			resultSize = GetScaledSize(resultSize);
			Bitmap bmp = new Bitmap(resultSize.Width, resultSize.Height);
			using(Graphics g = Graphics.FromImage((Image)bmp)) {
				g.DrawImage(Image, 0, 0, resultSize.Width, resultSize.Height);
			}
			OptimizedImage = (Image)bmp;
		}
		string text = null;
		[DefaultValue(null), Category(CategoryName.Appearance), XtraSerializableProperty, Localizable(true)]
		public string Text {
			get { return text; }
			set {
				if(Text == value)
					return;
				text = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public TileItemElementCollection Collection {
			get;
			protected internal set;
		}
		protected virtual void OnPropertiesChanged() {
			if(Collection != null)
				Collection.OnItemPropertiesChanged(this);
		}
		TileItemContentAlignment alignment = TileItemContentAlignment.Default;
		[DefaultValue(TileItemContentAlignment.Default), Category(CategoryName.Appearance)]
		public TileItemContentAlignment TextAlignment {
			get { return alignment; }
			set {
				if(TextAlignment == value)
					return;
				alignment = value;
				OnPropertiesChanged();
			}
		}
		int maxWidth = -1;
		[DefaultValue(-1), Category(CategoryName.Appearance)]
		public int MaxWidth {
			get { return maxWidth; }
			set {
				if(MaxWidth == value)
					return;
				maxWidth = value;
				OnPropertiesChanged();
			}
		}
		TileItemAppearances appearance;
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		void ResetAppearance() { Appearance.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public TileItemAppearances Appearance {
			get {
				if(appearance == null) {
					appearance = new TileItemAppearances(this);
					appearance.Changed += new EventHandler(OnAppearanceChanged);
				}
				return appearance;
			}
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		void ResetTextLocation() { TextLocation = DefaultLocation; }
		bool ShouldSerializeTextLocation() { return !TextLocation.Equals(DefaultLocation); }
		void ResetImageLocation() { ImageLocation = DefaultLocation; }
		bool ShouldSerializeImageLocation() { return !ImageLocation.Equals(DefaultLocation); }
		Point textLocation, imageLocation;
		[Category(CategoryName.Appearance)]
		public Point TextLocation {
			get { return textLocation; }
			set {
				if(TextLocation == value)
					return;
				textLocation = value;
				OnPropertiesChanged();
			}
		}
		[Category(CategoryName.Appearance)]
		public Point ImageLocation {
			get { return imageLocation; }
			set {
				if(ImageLocation == value)
					return;
				imageLocation = value;
				OnPropertiesChanged();
			}
		}
		public virtual void Assign(TileItemElement src) {
			Text = src.Text;
			TextAlignment = src.TextAlignment;
			TextLocation = src.TextLocation;
			MaxWidth = src.MaxWidth;
			Image = src.Image;
			ImageIndex = src.ImageIndex;
			ImageScaleMode = src.ImageScaleMode;
			ImageToTextAlignment = src.ImageToTextAlignment;
			ImageToTextIndent = src.ImageToTextIndent;
			ImageAlignment = src.ImageAlignment;
			ImageLocation = src.ImageLocation;
			ImageSize = src.ImageSize;
			ImageBorder = src.ImageBorder;
			ImageBorderColor = src.ImageBorderColor;
			ImageUri = src.ImageUri.Clone();
			AnchorElementIndex = src.AnchorElementIndex;
			AnchorAlignment = src.AnchorAlignment;
			AnchorIndent = src.AnchorIndent;
			AnchorOffset = src.AnchorOffset;
			AnimateTransition = src.AnimateTransition;
			UseTextInTransition = src.UseTextInTransition;
			UseImageInTransition = src.UseImageInTransition;
			Width = src.Width;
			Height = src.Height;
			StretchHorizontal = src.StretchHorizontal;
			StretchVertical = src.StretchVertical;
			Appearance.Assign(src.Appearance);
		}
		public virtual void AssignWithoutDefault(TileItemElement src, bool assignText, bool assignImage) {
			AnimateTransition = src.AnimateTransition;
			UseTextInTransition = src.UseTextInTransition;
			UseImageInTransition = src.UseImageInTransition;
			AnchorElementIndex = src.AnchorElementIndex;
			AnchorAlignment = src.AnchorAlignment;
			AnchorIndent = src.AnchorIndent;
			AnchorOffset = src.AnchorOffset;
			Width = src.Width;
			Height = src.Height;
			StretchHorizontal = src.StretchHorizontal;
			StretchVertical = src.StretchVertical;
			if(assignText) {
				Text = src.Text;
			}
			if(src.TextAlignment != TileItemContentAlignment.Default)
				TextAlignment = src.TextAlignment;
			TextLocation = src.TextLocation;
			MaxWidth = src.MaxWidth;
			if(assignImage) {
				Image = src.Image;
				ImageSize = src.ImageSize;
				ImageAlignment = src.ImageAlignment;
				ImageIndex = src.ImageIndex;
				ImageScaleMode = src.ImageScaleMode;
				ImageToTextAlignment = src.ImageToTextAlignment;
				ImageLocation = src.ImageLocation;
				ImageToTextIndent = src.ImageToTextIndent;
				ImageBorder = src.ImageBorder;
				ImageBorderColor = src.ImageBorderColor;
				ImageUri = src.ImageUri.Clone();
			}
			Appearance.Assign(src.Appearance);
		}
		#region ICloneable Members
		public object Clone() {
			TileItemElement res = CreateTileItemElement();
			res.Assign(this);
			return res;
		}
		#endregion
		protected internal object CloneWithoutDefault() {
			TileItemElement res = CreateTileItemElement();
			res.AssignWithoutDefault(this, true, true);
			return res;
		}
		protected virtual TileItemElement CreateTileItemElement() {
			return new TileItemElement();
		}
		#region IAppearanceOwner Members
		bool IAppearanceOwner.IsLoading {
			get { return false; }
		}
		#endregion
		public override string ToString() {
			if(IsTileItemElementTextEmpty(text))
				return Image == null ? "<empty>" : "<image>";
			return ProcessNewLineSimbols(Text);
		}
		protected bool IsTileItemElementTextEmpty(string text) {
			if(string.IsNullOrEmpty(text))
				return true;
			for(int i = 0; i < text.Length; i++) {
				if(!char.IsWhiteSpace(text[i]))
					return false;
			}
			return true;
		}
		protected string ProcessNewLineSimbols(string text) {
			return Text.Replace((char)0xA, ' ');
		}
		protected internal void SetImageAlignment(TileItemContentAlignment value) {
			this.imageAlignmentCore = value;
		}
		protected virtual Rectangle GetItemRectangle() {
			var itemInfo = GetItemInfo();
			if(itemInfo != null)
				return itemInfo.Bounds;
			return Rectangle.Empty;
		}
		TileItemViewInfo GetItemInfo() {
			var item = GetOwnerItem();
			if(item != null) {
				var owner = GetOwnerControl();
				if(owner == null)
					return null;
				var vi = owner.ViewInfo;
				if(vi != null && vi.ItemCacheContains(item))
					return vi.GetItemFromCache(item);
			}
			return null;
		}
		protected virtual INotifyElementPropertiesChanged GetOwner() {
			if(Collection == null) return null;
			return Collection.Owner;
		}
		protected virtual ITileControl GetOwnerControl() {
			var owner = GetOwner();
			return owner == null ? null : owner.TileControl;
		}
		protected TileItem GetOwnerItem() {
			var owner = GetOwner();
			return owner == null ? null : owner as TileItem;
		}
		protected TileItemFrame GetOwnerFrame() {
			var owner = GetOwner();
			return owner == null ? null : owner as TileItemFrame;
		}
		Rectangle IAnimatedItem.AnimationBounds {
			get { return GetItemRectangle(); }
		}
		int IAnimatedItem.AnimationInterval {
			get { return GifHelper.AnimationInterval; }
		}
		int[] IAnimatedItem.AnimationIntervals {
			get { return GifHelper.AnimationIntervals; }
		}
		DevExpress.Utils.Drawing.Animation.AnimationType IAnimatedItem.AnimationType {
			get { return GifHelper.AnimationType; }
		}
		int IAnimatedItem.FramesCount {
			get { return GifHelper.FramesCount; }
		}
		int IAnimatedItem.GetAnimationInterval(int frameIndex) {
			return GifHelper.GetAnimationInterval(frameIndex);
		}
		bool IAnimatedItem.IsAnimated {
			get { return GifHelper.IsAnimated; }
		}
		void IAnimatedItem.OnStart() { }
		void IAnimatedItem.OnStop() { }
		object IAnimatedItem.Owner {
			get { return this; }
		}
		void IAnimatedItem.UpdateAnimation(BaseAnimationInfo info) {
			GifHelper.UpdateAnimation(info);
		}
		AnimatedImageHelper gifHelper;
		protected AnimatedImageHelper GifHelper {
			get {
				if(gifHelper == null)
					gifHelper = CreateGifHelper(Image);
				return gifHelper;
			}
		}
		protected virtual AnimatedImageHelper CreateGifHelper(Image image) {
			return new AnimatedImageHelper(image);
		}
		public virtual void StopGifAnimation() {
			XtraAnimator.RemoveObject(this);
		}
		public virtual void StartGifAnimation() {
			if(CanStartGifAnimation) {
				XtraAnimator.Current.AddEditorAnimation(null, this, this, new CustomAnimationInvoker(OnImageAnimation));
			}
		}
		protected bool CanStartGifAnimation {
			get {
				if(ControlInDesignMode)
					return false;
				return Image != null && (this as IAnimatedItem).FramesCount > 1 && GetOwnerFrame() == null;
			}
		}
		protected virtual void OnImageAnimation(BaseAnimationInfo animInfo) {
			IAnimatedItem animItem = this;
			EditorAnimationInfo info = animInfo as EditorAnimationInfo;
			var owner = GetOwnerControl();
			if(Image == null || info == null || owner == null) return;
			if(GifAnimationIsLegal) {
				if(!animInfo.IsFinalFrame) {
					Image.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Time, info.CurrentFrame);
					owner.Invalidate(animItem.AnimationBounds);
				}
				else {
					StopGifAnimation();
					StartGifAnimation();
				}
			}
			else {
				StopGifAnimation();
			}
		}
		bool ISupportXtraAnimation.CanAnimate {
			get { return GifHelper.FramesCount > 1; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get {
				var itilecontrol = GetOwnerControl();
				return itilecontrol == null ? null : itilecontrol.Control;
			}
		}
		protected internal virtual void UpdateGifHelper() {
			StopGifAnimation();
			GifHelper.Image = Image;
			StartGifAnimation();
		}
		bool ControlInDesignMode {
			get {
				var control = GetOwnerControl();
				return control == null ? false : control.IsDesignMode;
			}
		}
		protected virtual bool GifAnimationIsLegal {
			get {
				return !ControlInDesignMode && GetOwnerItem() != null;
			}
		}
		protected internal void OnIndexChanged() {
			if(Collection == null || !Collection.Contains(this)) return;
			foreach(TileItemElement childElement in AnchorChildsList) {
				childElement.AnchorElementIndex = Collection.IndexOf(this);
			}
		}
		protected internal virtual string GetDisplayText() {
			return Text;
		}
		protected internal virtual Image GetDisplayImage() {
			EnsureOptimizedImageScale();
			if(needUpdateOptimizedImage) {
				UpdateOptimizedImage();
				optimizedImgScaleFactor = GetScaleFactor();
				optimizedImgItemSize = GetItemRectangle().Size;
				needUpdateOptimizedImage = false;
			}
			return Image;
		}
		SizeF optimizedImgScaleFactor = new SizeF(1.0f, 1.0f);
		Size optimizedImgItemSize = new Size();
		void EnsureOptimizedImageScale() {
			if(Image == null) return;
			Size currentItemSize = GetItemRectangle().Size;
			SizeF currentFactor = GetScaleFactor();
			if(optimizedImgScaleFactor != currentFactor || optimizedImgItemSize != currentItemSize)
				SetOptimizedImageDirty();
		}
		protected internal SizeF GetScaleFactor() {
			TileItem item = GetOwnerItem();
			if(item != null && item.ItemInfo != null && item.Control != null && item.Control.ScaleFactor != new SizeF(1, 1)) {
				int itemSizeScaled = item.ItemInfo.ControlInfo.ItemSize;
				int itemSize = item.ItemInfo.ControlInfo.Owner.Properties.ItemSize;
				float scaleY = (float)itemSizeScaled / itemSize;
				float scaleX = (float)((itemSizeScaled * 2) + item.ItemInfo.ControlInfo.IndentBetweenItems) / ((itemSize * 2) + item.ItemInfo.ControlInfo.Owner.Properties.IndentBetweenItems);
				return new SizeF(scaleX, scaleY);
			}
			return new SizeF(1, 1);
		}
		protected internal Size GetScaledSize(Size input) {
			if(input != Size.Empty) {
				SizeF scale = GetScaleFactor();
				return new Size((int)(input.Width * scale.Width), (int)(input.Height * scale.Height));
			}
			return input;
		}
		bool IDXImageUriClient.IsDesignMode {
			get { 
				var control = GetOwnerControl();
				if(control == null) return false;
				return control.IsDesignMode;
			}
		}
		UserLookAndFeel IDXImageUriClient.LookAndFeel {
			get {
				var control = GetOwnerControl();
				return control == null ? null : control.LookAndFeel;
			}
		}
		void IDXImageUriClient.SetGlyphSkinningValue(bool value) {
			var item = GetOwnerItem();
			if(item != null) item.AllowGlyphSkinning = value ? DefaultBoolean.True : DefaultBoolean.Default;
		}
		bool IDXImageUriClient.SupportsGlyphSkinning {
			get { return true; }
		}
		bool IDXImageUriClient.SupportsLookAndFeel {
			get { return true; }
		}
		protected virtual DxImageUri CreateImageUriInstance() {
			return new DxImageUri();
		}
		bool ShouldSerializeImageUri() { return ImageUri.ShouldSerialize(); }
		void ResetImageUri() { ImageUri.Reset(); }
		DxImageUri imageUri;
		[Category("Appearance"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DxImageUri ImageUri {
			get { return imageUri; }
			set {
				if(ImageUri.Equals(value)) return;
				var oldUri = ImageUri;
				imageUri = value;
				OnImageUriChanged(oldUri, ImageUri);
			}
		}
		void OnImageUriChanged(DxImageUri oldUri, DxImageUri newUri) {
			if(oldUri != null) {
				oldUri.Changed -= ImageUriChanged;
			}
			if(newUri != null) {
				newUri.Changed += ImageUriChanged;
				newUri.SetClient(this);
			}
			OnPropertiesChanged();
		}
		void ImageUriChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		protected internal void ReleaseImageUri() {
			if(this.imageUri != null) {
				this.imageUri.Changed -= ImageUriChanged;
				this.imageUri.Dispose();
			}
			this.imageUri = null;
		}
	}
	[Editor("DevExpress.XtraBars.Design.TileItemElementsEditor, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class TileItemElementCollection : CollectionBase, IEnumerable<TileItemElement> {
		public TileItemElementCollection(INotifyElementPropertiesChanged owner) {
			Owner = owner;
		}
		public INotifyElementPropertiesChanged Owner { get; internal set; }
		public int Add(TileItemElement element) { return List.Add(element); }
		public void Insert(int index, TileItemElement element) { List.Insert(index, element); }
		public void Remove(TileItemElement element) { List.Remove(element); }
		public TileItemElement this[int index] { get { return (TileItemElement)List[index]; } set { List[index] = value; } }
		int UpdateCount { get; set; }
		public void BeginUpdate() {
			UpdateCount++;
		}
		public void EndUpdate() {
			if(UpdateCount == 0)
				return;
			UpdateCount--;
			if(UpdateCount == 0)
				OnItemPropertiesChanged(null);
		}
		public void CancelUpdate() {
			if(UpdateCount == 0)
				return;
			UpdateCount--;
		}
		public bool IsLockUpdate { get { return UpdateCount > 0; } }
		protected internal virtual void OnItemPropertiesChanged(TileItemElement element) {
			if(!IsLockUpdate)
				Owner.OnElementPropertiesChanged(element);
		}
		protected override void OnClear() {
			foreach(TileItemElement element in List)
				SetCollection(element, null);
			base.OnClear();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			DefferedNotifyElements.Clear();
			if(!IsLockUpdate)
				Owner.OnElementPropertiesChanged(null);
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			SetCollection(value, this);
			for(int i = index; i < List.Count; i++)
				((TileItemElement)List[i]).OnIndexChanged();
			if(!IsLockUpdate)
				Owner.OnElementPropertiesChanged((TileItemElement)value);
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			if(oldValue != null)
				SetCollection(oldValue, null);
			if(newValue != null)
				SetCollection(newValue, this);
			if(!IsLockUpdate)
				Owner.OnElementPropertiesChanged((TileItemElement)newValue);
		}
		public void Assign(TileItemElementCollection src) {
			Clear();
			foreach(TileItemElement elem in src)
				Add((TileItemElement)elem.Clone());
		}
		protected internal void AssignWithoutDefault(TileItemElementCollection src, bool assignText, bool assignImage) {
			Clear();
			for(int i = 0; i < src.Count; i++) {
				TileItemElement elem = new TileItemElement();
				Add(elem);
				elem.AssignWithoutDefault(src[i], assignText, assignImage);
			}
		}
		public int IndexOf(TileItemElement element) {
			return List.IndexOf(element);
		}
		public bool Contains(TileItemElement element) {
			return List.Contains(element);
		}
		IEnumerator<TileItemElement> IEnumerable<TileItemElement>.GetEnumerator() {
			foreach(TileItemElement element in List)
				yield return element;
		}
		protected override void OnRemove(int index, object value) {
			var element = (value as TileItemElement);
			if(element.AnchorElement != null)
				element.NotifyAnchorElement(element.AnchorElement, null);
			while(element.AnchorChildsList.Count > 0)
				element.AnchorChildsList[0].AnchorElement = null;
			base.OnRemove(index, value);
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			for(int i = index; i < List.Count; i++)
				((TileItemElement)List[i]).OnIndexChanged();
			SetCollection(value, null);
			if(!IsLockUpdate)
				Owner.OnElementPropertiesChanged(null);
		}
		protected virtual void SetCollection(object value, TileItemElementCollection collection) {
			var element = value as TileItemElement;
			element.Collection = collection;
			if(collection != null) {
				if(element.AnchorElementIndex > -1) {
					if(element.AnchorElementIndex < collection.Count)
						element.NotifyAnchorElement(null, element.AnchorElement);
					else
						DefferedNotifyElements.Add(element, element.AnchorElementIndex);
				}
				int index = collection.IndexOf(element);
				while(DefferedNotifyElements.ContainsValue(index)) {
					var pairsToRemove = new List<KeyValuePair<TileItemElement, int>>();
					foreach(KeyValuePair<TileItemElement, int> pair in DefferedNotifyElements) {
						if(pair.Value != index) continue;
						pair.Key.NotifyAnchorElement(null, element);
						pairsToRemove.Add(pair);
					}
					foreach(KeyValuePair<TileItemElement, int> pair in pairsToRemove)
						if(DefferedNotifyElements.ContainsKey(pair.Key))
							DefferedNotifyElements.Remove(pair.Key);
				}
			}
		}
		Dictionary<TileItemElement, int> DefferedNotifyElements = new Dictionary<TileItemElement, int>();
	}
	public interface INotifyElementPropertiesChanged {
		void OnElementPropertiesChanged(TileItemElement element);
		ITileControl TileControl { get; }
	}
	public interface ITileItem {
		ITileControl Control { get; }
		Padding Padding { get; set; }
		Image BackgroundImage { get; set; }
		bool Checked { get; set; }
		bool Visible { get; set; }
		bool Enabled { get; set; }
		object Tag { get; set; }
		event TileItemClickEventHandler CheckedChanged;
		TileItemElementCollection Elements { get; }
		TileItemFrameCollection Frames { get; }
		TileItemAppearances Appearances { get; }
		ITileItemProperties Properties { get; }
		int CurrentFrameIndex { get; }
		void OnInfoChanged();
		void SetContent(TileItemFrame frame, bool animated);
	}
	public interface ITileItemProperties {
		bool IsLarge { get; set; }
		TileItemSize ItemSize { get; set; }
		int RowCount { get; set; }
		bool AllowGlyphSkinning { get; set; }
		TileItemImageScaleMode BackgroundImageScaleMode { get; set; }
		TileItemContentAlignment BackgroundImageAlignment { get; set; }
		bool AllowHtmlDraw { get; set; }
		TileItemContentShowMode TextShowMode { get; set; }
		int CurrentFrameIndex { get; set; }
		int FrameAnimationInterval { get; set; }
		TileItemContentAnimationType ContentAnimation { get; set; }
		TileItemBorderVisibility BorderVisibility { get; set; }
		void Assign(ITileItemProperties source);
	}
	public enum TileItemSize { Default, Small, Medium, Wide, Large }
	[
	ToolboxItem(false), DesignTimeVisible(false), DefaultEvent("ItemClick"),
	SmartTagSupport(typeof(TileItemDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto),
	SmartTagAction(typeof(TileItemDesignTimeActionsProvider), "AddGroup", "Add Group", SmartTagActionType.RefreshAfterExecute),
	SmartTagAction(typeof(TileItemDesignTimeActionsProvider), "RemoveGroup", "Remove Group", SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(TileItemDesignTimeActionsProvider), "AddMediumItem", "Add Medium Item", SmartTagActionType.RefreshAfterExecute),
	SmartTagAction(typeof(TileItemDesignTimeActionsProvider), "AddSmallItem", "Add Small Item", SmartTagActionType.RefreshAfterExecute),
	SmartTagAction(typeof(TileItemDesignTimeActionsProvider), "AddWideItem", "Add Wide Item", SmartTagActionType.RefreshAfterExecute),
	SmartTagAction(typeof(TileItemDesignTimeActionsProvider), "AddLargeItem", "Add Large Item", SmartTagActionType.RefreshAfterExecute),
	SmartTagAction(typeof(TileItemDesignTimeActionsProvider), "RemoveItem", "Remove Item", SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(TileItemDesignTimeActionsProvider), "EditFrames", "Edit Animation Frames", SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(TileItemDesignTimeActionsProvider), "EditElements", "Edit TileItem Elements", SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(TileItemDesignTimeActionsProvider), "SelectTemplate", "Select TileItem Template", SmartTagActionType.CloseAfterExecute),
	Designer("DevExpress.XtraBars.Design.TileItemDesigner, " + AssemblyInfo.SRAssemblyBarsDesign)
	]
	public class TileItem : Component, ITileItem, ITileItemProperties, IAppearanceOwner, INotifyElementPropertiesChanged, DevExpress.Utils.MVVM.ISupportCommandBinding, ISupportAdornerElement {
		private static readonly object itemClick = new object();
		private static readonly object itemDoubleClick = new object();
		private static readonly object rightItemClick = new object();
		private static readonly object itemPress = new object();
		private static readonly object checkedChanged = new object();
		private static readonly object targetChanged = new object();
		public TileItem() {
			this.allowHtmlText = DefaultBoolean.Default;
			this.padding = DefaultPadding;
			FrameAnimationInterval = DefaultFrameAnimationInterval;
			ContentAnimation = TileItemContentAnimationType.Default;
			allowAnimation = visible = enabled = true;
		}
		SuperToolTip superTip;
		[ Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(UITypeEditor)), SmartTagProperty("Super Tip", "Appearance", 7), Category("Appearance")]
		public virtual SuperToolTip SuperTip {
			get { return superTip; }
			set { superTip = value; }
		}
		protected virtual bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		public virtual void ResetSuperTip() { SuperTip = null; }
		DefaultBoolean allowGlyphSkinning = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(allowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				OnPropertiesChanged();
			}
		}
		bool enabled;
		[DefaultValue(true), SmartTagProperty("Enabled", "Appearance", 60)]
		public virtual bool Enabled {
			get { return enabled; }
			set {
				if(Enabled == value) return;
				enabled = value;
				if(value) Timer.Enabled = true;
				else Timer.Enabled = false;
				if(Control != null)
					Control.OnPropertiesChanged();
			}
		}
		bool visible;
		[DefaultValue(true)]
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				OnVisibilityChanged();
			}
		}
		protected virtual void OnVisibilityChanged() {
			if(Control != null)
				Control.OnPropertiesChanged();
		}
		int id = -1;
		[DefaultValue(-1), XtraSerializableProperty]
		public int Id { get { return id; } set { id = value; } }
		public void StartAnimation() {
			if(Frames.Count == 0 || Control == null)
				return;
			CurrentFrameIndex = 0;
			SetContent(CurrentFrame, false);
			allowFrameAnimation = true;
			Timer.Stop();
			Timer.Interval = GetInterval(CurrentFrame);
			TimerStartCore();
		}
		int rowCount = 1;
		[DefaultValue(1)]
		public int RowCount {
			get { return rowCount; }
			set {
				value = Math.Max(1, value);
				if(RowCount == value)
					return;
				rowCount = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(TileItemContentAnimationType.Default), SmartTagProperty("Content Animation Type", "Behavior")]
		public TileItemContentAnimationType ContentAnimation {
			get;
			set;
		}
		TileItemElementCollection items;
		void ResetItems() { Elements.Clear(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Collection, false, true, false)]
		public TileItemElementCollection Elements {
			get {
				if(items == null)
					items = CreateElementCollection();
				return items;
			}
			set {
				if(items == value)
					return;
				TileItemElementCollection prev = Elements;
				items = value;
				OnElementsCollectionChanged(prev);
			}
		}
		internal TileItemElement XtraFindElementsItem(XtraItemEventArgs e) {
			int index = int.Parse(e.Item.Name.Replace("Item", ""));
			if(Elements.Count >= index) {
				return Elements[index - 1];
			}
			return null;
		}
		protected virtual TileItemElementCollection CreateElementCollection() {
			return new TileItemElementCollection(this);
		}
		ITileItemProperties ITileItem.Properties {
			[System.Diagnostics.DebuggerStepThrough]
			get { return this; }
		}
		TileItemAppearances ITileItem.Appearances {
			get {
				if(appearances == null)
					appearances = new TileItemAppearances(this);
				return appearances;
			}
		}
		int bufferedCurrentFrameIndex = 0;
		void ITileItemProperties.Assign(ITileItemProperties source) {
			this.allowHtmlText = source.AllowHtmlDraw ? DefaultBoolean.Default : DefaultBoolean.False;
			this.backgroundImageAlignment = source.BackgroundImageAlignment;
			this.backgroundImageScaleMode = source.BackgroundImageScaleMode;
			this.AllowGlyphSkinning = source.AllowGlyphSkinning ? DefaultBoolean.True : DefaultBoolean.Default;
			ContentAnimation = source.ContentAnimation;
			if(bufferedCurrentFrameIndex != source.CurrentFrameIndex)
				CurrentFrameIndex = source.CurrentFrameIndex;
			bufferedCurrentFrameIndex = source.CurrentFrameIndex;
			FrameAnimationInterval = source.FrameAnimationInterval;
			this.itemSize = source.ItemSize;
			this.rowCount = source.RowCount;
			this.textShowMode = source.TextShowMode;
			this.BorderVisibility = source.BorderVisibility;
		}
		ITileControl ITileItem.Control { get { return Control; } }
		void ITileItem.OnInfoChanged() {
			if(!Frames.LockUpdate)
				OnInfoChanged();
		}
		bool ITileItemProperties.AllowGlyphSkinning {
			get { return AllowGlyphSkinning == DefaultBoolean.True; }
			set { AllowHtmlText = value ? DefaultBoolean.True : DefaultBoolean.Default; }
		}
		bool ITileItemProperties.AllowHtmlDraw {
			get { return AllowHtmlText != DefaultBoolean.False; }
			set { AllowHtmlText = value ? DefaultBoolean.Default : DefaultBoolean.False; }
		}
		int ITileItem.CurrentFrameIndex {
			get { return this.CurrentFrameIndex; }
		}
		void OnElementsCollectionChanged(TileItemElementCollection prev) {
			if(prev != null)
				prev.Owner = null;
			Elements.Owner = this;
			OnPropertiesChanged();
		}
		void CreateContentElements(int count) {
			if(Elements.Count < count) {
				for(int i = Elements.Count; i < count; i++) {
					Elements.Add(CreateTileItemElement());
				}
			}
		}
		protected virtual TileItemElement CreateTileItemElement() {
			return new TileItemElement();
		}
		object hoverAnimationId = new object();
		internal object HoverAnimationId { get { return hoverAnimationId; } }
		TileItemContentShowMode textShowMode = TileItemContentShowMode.Default;
		[DefaultValue(TileItemContentShowMode.Default), Category(CategoryName.Appearance), SmartTagProperty("Text Show Mode", "Appearance", 120)]
		public TileItemContentShowMode TextShowMode {
			get { return textShowMode; }
			set {
				if(TextShowMode == value)
					return;
				textShowMode = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(6), Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ImageToTextIndent {
			get {
				if(Elements.Count < 1)
					return 6;
				return Elements[0].ImageToTextIndent;
			}
			set {
				if(ImageToTextIndent == value)
					return;
				CreateContentElements(1);
				Elements[0].ImageToTextIndent = value;
				OnPropertiesChanged();
			}
		}
		TileControlImageToTextAlignment imageToTextAlignment = TileControlImageToTextAlignment.Default;
		[DefaultValue(TileControlImageToTextAlignment.Default), Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), SmartTagProperty("Image To Text Alignment", "Appearance", 40)]
		public TileControlImageToTextAlignment ImageToTextAlignment {
			get {
				if(imageToTextAlignment != TileControlImageToTextAlignment.Default) {
					CreateContentElements(1);
					Elements[0].ImageToTextAlignment = imageToTextAlignment;
				}
				return imageToTextAlignment;
			}
			set {
				if(ImageToTextAlignment == value)
					return;
				CreateContentElements(1);
				Elements[0].ImageToTextAlignment = value;
				imageToTextAlignment = value;
				OnPropertiesChanged();
			}
		}
		Padding padding;
		internal static Padding DefaultPadding { get { return new Padding(-1); } }
		bool ShouldSerializePadding() { return Padding != DefaultPadding; }
		void ResetPadding() { Padding = DefaultPadding; }
		[Category(CategoryName.Appearance)]
		public Padding Padding {
			get { return padding; }
			set {
				if(Padding == value)
					return;
				padding = value;
				OnPropertiesChanged();
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void StartContentAnimation() {
			if(Frames == null || Frames.Count == 0)
				return;
			allowFrameAnimation = true;
			OnInfoChanged();
		}
		bool checkedCore = false;
		[DefaultValue(false), Category(CategoryName.Behavior), SmartTagProperty("Checked", "Appearance", 55)]
		public bool Checked {
			get { return checkedCore; }
			set {
				if(Checked == value)
					return;
				checkedCore = value;
				OnCheckedChanged();
			}
		}
		protected virtual void OnCheckedChanged() {
			if(Control == null)
				return;
			if(Control.Properties.ItemCheckMode == TileItemCheckMode.Single)
				ClearItemsCheckState(this);
			if(ItemInfo == null)
				Control.Invalidate(Control.ClientRectangle);
			else Control.Invalidate(ItemInfo.Bounds);
			OnItemCheckedChanged();
		}
		protected virtual void ClearItemsCheckState(TileItem tileItem) {
			if(Control == null)
				return;
			foreach(TileGroup group in Control.Groups) {
				foreach(TileItem item in group.Items) {
					if(item != tileItem) {
						item.ClearCheckCore();
					}
				}
			}
			Control.Invalidate(Control.ClientRectangle);
		}
		protected virtual void ClearCheckCore() {
			if(Checked == false)
				return;
			this.checkedCore = false;
			RaiseCheckedChanged();
		}
		[Category(CategoryName.Behavior)]
		public event TileItemClickEventHandler CheckedChanged {
			add { Events.AddHandler(checkedChanged, value); }
			remove { Events.RemoveHandler(checkedChanged, value); }
		}
		[DefaultValue(TileItemImageScaleMode.Default), Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TileItemImageScaleMode ImageScaleMode {
			get {
				if(Elements.Count < 1)
					return TileItemImageScaleMode.Default;
				return Elements[0].ImageScaleMode;
			}
			set {
				if(ImageScaleMode == value)
					return;
				CreateContentElements(1);
				Elements[0].ImageScaleMode = value;
				OnPropertiesChanged();
			}
		}
		TileItemImageScaleMode backgroundImageScaleMode = TileItemImageScaleMode.Default;
		[DefaultValue(TileItemImageScaleMode.Default), Category(CategoryName.Appearance)]
		public TileItemImageScaleMode BackgroundImageScaleMode {
			get { return backgroundImageScaleMode; }
			set {
				if(BackgroundImageScaleMode == value)
					return;
				backgroundImageScaleMode = value;
				SetOptimizedBackgroundImageDirty();
				OnPropertiesChanged();
			}
		}
		TileItemBorderVisibility borderMode = TileItemBorderVisibility.Auto;
		[DefaultValue(TileItemBorderVisibility.Auto), Category(CategoryName.Appearance)]
		public TileItemBorderVisibility BorderVisibility {
			get { return borderMode; }
			set {
				if(borderMode == value) return;
				borderMode = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TileItemViewInfo ItemInfo {
			get;
			internal set;
		}
		[DefaultValue(null), Category(CategoryName.Data), SmartTagProperty("Tag", ""),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get;
			set;
		}
		protected internal void SavePrevText() {
			ItemInfo.PrevItems = new List<TileItemElementViewInfo>();
			foreach(TileItemElementViewInfo info in ItemInfo.Elements) {
				ItemInfo.PrevItems.Add(info);
			}
		}
		[Category(CategoryName.Behavior)]
		public event TileItemClickEventHandler ItemClick {
			add { Events.AddHandler(itemClick, value); }
			remove { Events.RemoveHandler(itemClick, value); }
		}
		[Category(CategoryName.Behavior)]
		public event TileItemClickEventHandler ItemDoubleClick {
			add { Events.AddHandler(itemDoubleClick, value); }
			remove { Events.RemoveHandler(itemDoubleClick, value); }
		}
		[Category(CategoryName.Behavior)]
		public event TileItemClickEventHandler RightItemClick {
			add { Events.AddHandler(rightItemClick, value); }
			remove { Events.RemoveHandler(rightItemClick, value); }
		}
		[Category(CategoryName.Behavior)]
		public event TileItemClickEventHandler ItemPress {
			add { Events.AddHandler(itemPress, value); }
			remove { Events.RemoveHandler(itemPress, value); }
		}
		protected internal void RaiseCheckedChanged() {
			TileItemEventArgs e = new TileItemEventArgs() { Item = this };
			TileItemClickEventHandler handler = Events[checkedChanged] as TileItemClickEventHandler;
			if(handler != null)
				handler.Invoke(this, e);
		}
		protected internal void RaiseItemClick() {
			TileItemEventArgs e = new TileItemEventArgs() { Item = this };
			TileItemClickEventHandler handler = Events[itemClick] as TileItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseItemDoubleClick() {
			TileItemEventArgs e = new TileItemEventArgs() { Item = this };
			TileItemClickEventHandler handler = Events[itemDoubleClick] as TileItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseRightItemClick() {
			TileItemEventArgs e = new TileItemEventArgs() { Item = this };
			TileItemClickEventHandler handler = Events[rightItemClick] as TileItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseItemPress() {
			TileItemEventArgs e = new TileItemEventArgs() { Item = this };
			TileItemClickEventHandler handler = Events[itemPress] as TileItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal TileItemCollection Collection { get; set; }
		[Browsable(false)]
		public TileGroup Group { get { return GetGroup(); } }
		protected virtual TileGroup GetGroup() { return Collection == null ? null : Collection.Owner; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ITileControl Control { get { return Group == null ? null : Group.Control; } }
		TileItemFrameCollection info;
		[Browsable(false), DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Collection, false, true, false)]
		public TileItemFrameCollection Frames {
			get {
				if(info == null)
					info = new TileItemFrameCollection(this);
				return info;
			}
		}
		internal TileItemFrame XtraFindFramesItem(XtraItemEventArgs e) {
			int index = int.Parse(e.Item.Name.Replace("Item", ""));
			if(Frames.Count >= index) {
				return Frames[index - 1];
			}
			return null;
		}
		static int DefaultFrameAnimationInterval = 3000;
		[DefaultValue(3000), Category(CategoryName.Behavior), XtraSerializableProperty]
		public int FrameAnimationInterval {
			get;
			set;
		}
		Timer timer;
		protected internal Timer Timer {
			get {
				if(timer == null) {
					timer = new Timer();
					timer.Tick += OnTimerTick;
				}
				return timer;
			}
		}
		[DefaultValue(0), Category(CategoryName.Properties), XtraSerializableProperty]
		public int CurrentFrameIndex {
			get;
			set;
		}
		[Browsable(false)]
		public TileItemFrame CurrentFrame {
			get {
				if(CurrentFrameIndex >= Frames.Count)
					return null;
				return Frames[CurrentFrameIndex];
			}
		}
		protected virtual void OnTimerTick(object sender, EventArgs e) {
			if(!allowFrameAnimation || ItemInfo == null || ItemInfo.IsPressed || !Enabled) return;
			CurrentFrameIndex++;
			if(CurrentFrameIndex >= Frames.Count)
				CurrentFrameIndex = 0;
			SetItemInfo(CurrentFrame);
		}
		protected virtual bool IsHoverAnimationActive {
			get {
				if(Control == null)
					return false;
				return XtraAnimator.Current.Get(Control.AnimationHost, HoverAnimationId) != null;
			}
		}
		protected virtual void SetItemInfo(TileItemFrame frame) {
			if(frame == null || IsHoverAnimationActive)
				return;
			Timer.Stop();
			if(Frames.Count == 0)
				return;
			SetContent(frame, true);
			Timer.Interval = GetInterval(frame);
			TimerStartCore();
		}
		protected virtual void SetContentNonAnimated(TileItemFrame frame) {
			if(Control != null)
				Control.BeginUpdate();
			try {
				if(frame.UseBackgroundImage)
					BackgroundImage = frame.BackgroundImage;
				Elements.BeginUpdate();
				try {
					Elements.AssignWithoutDefault(frame.Elements, frame.UseText, frame.UseImage);
				}
				finally {
					Elements.EndUpdate();
				}
			}
			finally {
				if(Control != null)
					Control.EndUpdate();
			}
			return;
		}
		void UpdateCurrentFrameIndex(TileItemFrame frame) {
			if(ItemInfo != null && Control != null && !ItemInfo.Bounds.IsEmpty && ItemInfo.Item.Frames.Contains(frame))
				ItemInfo.Item.CurrentFrameIndex = ItemInfo.Item.Frames.IndexOf(frame);
		}
		public virtual void SetContent(TileItemFrame frame, bool animated) {
			UpdateCurrentFrameIndex(frame);
			if(Control == null || ItemInfo == null || !animated || ItemInfo.Bounds.IsEmpty) {
				SetContentNonAnimated(frame);
				return;
			}
			ItemInfo.IsChangingContent = false;
			ItemInfo.PrepareForContentChanging(frame);
			Elements.BeginUpdate();
			try {
				Elements.AssignWithoutDefault(frame.Elements, frame.UseText, frame.UseImage);
			}
			finally {
				Elements.CancelUpdate();
			}
			if(frame.UseBackgroundImage) {
				this.backgroundImage = frame.BackgroundImage;
				this.SetOptimizedBackgroundImageDirty();
			}
			if(!frame.AnimateBackgroundImage && !frame.AnimateImage && !frame.AnimateText) {
				ItemInfo.LayoutItem(ItemInfo.Bounds);
				Control.Invalidate(ItemInfo.Bounds);
				return;
			}
			Control.ViewInfo.AddChangeItemContentAnimation(ItemInfo, frame);
		}
		int GetInterval(TileItemFrame info) {
			return (info.Interval == 0 ? FrameAnimationInterval : info.Interval) + TileControlViewInfo.ItemContentAnimationLength;
		}
		protected internal virtual void OnInfoChanged() {
			if(CurrentFrameIndex >= Frames.Count)
				CurrentFrameIndex = 0;
			Timer.Stop();
			if(Frames == null || Frames.Count == 0 || Control == null || !Control.AllowItemContentAnimation)
				return;
			SetContent(CurrentFrame, false);
			if(Frames.Count == 1) {
				Timer.Stop();
				return;
			}
			Timer.Interval = GetInterval(CurrentFrame);
			if(Control != null && Control.IsAnimationSuspended)
				return;
			TimerStartCore();
		}
		DefaultBoolean allowHtmlText;
		[DefaultValue(DefaultBoolean.Default), Category(CategoryName.Appearance)]
		public DefaultBoolean AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(AllowHtmlText == value)
					return;
				allowHtmlText = value;
				OnPropertiesChanged();
			}
		}
		Image renderImage;
		internal Image RenderImage {
			get { return renderImage; }
			set {
				if(RenderImage == value)
					return;
				if(RenderImage != null) RenderImage.Dispose();
				renderImage = value;
			}
		}
		Image prevRenderImage;
		internal Image PrevRenderImage {
			get { return prevRenderImage; }
			set {
				if(PrevRenderImage == value)
					return;
				if(PrevRenderImage != null) PrevRenderImage.Dispose();
				prevRenderImage = value;
			}
		}
		bool disposed = false;
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				RenderImage = null;
				PrevRenderImage = null;
				OptimizedBackgroundImage = null;
				ReleaseElementsImageUri();
				FreeContentAnimationTimer();
				if(appearances != null) {
					appearances.Changed -= new EventHandler(OnAppearanceChanged);
					appearances = null;
				}
				if(ItemInfo != null && ItemInfo.ControlInfo != null)
					ItemInfo.ControlInfo.RemoveAnimation(ItemInfo);
			}
			this.disposed = true;
		}
		protected internal bool IsDisposed { get { return disposed; } }
		protected internal void FreeContentAnimationTimer() {
			if(timer == null) return;
			timer.Tick -= OnTimerTick;
			timer.Dispose();
			timer = null;
		}
		void ReleaseElementsImageUri() {
			foreach(TileItemElement element in Elements)
				element.ReleaseImageUri();
		}
		Image backgroundImage;
		[DefaultValue(null), Category(CategoryName.Appearance), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image BackgroundImage {
			get { return backgroundImage; }
			set {
				if(BackgroundImage == value)
					return;
				backgroundImage = value;
				SetOptimizedBackgroundImageDirty();
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public object Images { get { return Control == null ? null : Control.Images; } }
		[DefaultValue(-1), Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)), ImageList("Images"), SmartTagProperty("Image Index", "Appearance", 20)]
		public int ImageIndex {
			get {
				if(Elements.Count < 1)
					return -1;
				return Elements[0].ImageIndex;
			}
			set {
				if(ImageIndex == value)
					return;
				CreateContentElements(1);
				Elements[0].ImageIndex = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(null), Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), SmartTagProperty("Image", "Appearance", 10), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Image {
			get {
				if(Elements.Count < 1)
					return null;
				return Elements[0].Image;
			}
			set {
				if(Image == value)
					return;
				CreateContentElements(1);
				Elements[0].Image = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(TileItemContentAlignment.Default), Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), SmartTagProperty("Text Alignment", "Appearance", 1)]
		public TileItemContentAlignment TextAlignment {
			get {
				if(Elements.Count == 0)
					return TileItemContentAlignment.Default;
				return Elements[0].TextAlignment;
			}
			set {
				CreateContentElements(1);
				if(Elements[0].TextAlignment == value)
					return;
				Elements[0].TextAlignment = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(TileItemContentAlignment.Default), Category(CategoryName.Appearance), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TileItemContentAlignment Text2Alignment {
			get {
				if(Elements.Count < 2)
					return TileItemContentAlignment.Default;
				return Elements[1].TextAlignment;
			}
			set {
				CreateContentElements(2);
				if(Elements[1].TextAlignment == value)
					return;
				Elements[1].TextAlignment = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(TileItemContentAlignment.Default), Category(CategoryName.Appearance), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TileItemContentAlignment Text3Alignment {
			get {
				if(Elements.Count < 3)
					return TileItemContentAlignment.Default;
				return Elements[2].TextAlignment;
			}
			set {
				CreateContentElements(3);
				if(Elements[2].TextAlignment == value)
					return;
				Elements[2].TextAlignment = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(TileItemContentAlignment.Default), Category(CategoryName.Appearance), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TileItemContentAlignment Text4Alignment {
			get {
				if(Elements.Count < 4)
					return TileItemContentAlignment.Default;
				return Elements[3].TextAlignment;
			}
			set {
				CreateContentElements(4);
				if(Elements[3].TextAlignment == value)
					return;
				Elements[3].TextAlignment = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(null), Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), SmartTagProperty("Text", "Appearance", 0)]
		public string Text {
			get {
				if(Elements.Count < 1)
					return null;
				return Elements[0].Text;
			}
			set {
				CreateContentElements(1);
				if(Elements[0].Text == value)
					return;
				Elements[0].Text = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(null), Category(CategoryName.Appearance), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Text2 {
			get {
				if(Elements.Count < 2)
					return null;
				return Elements[1].Text;
			}
			set {
				CreateContentElements(2);
				if(Elements[1].Text == value)
					return;
				Elements[1].Text = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(null), Category(CategoryName.Appearance), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Text3 {
			get {
				if(Elements.Count < 3)
					return null;
				return Elements[2].Text;
			}
			set {
				CreateContentElements(3);
				if(Elements[2].Text == value)
					return;
				Elements[2].Text = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(null), Category(CategoryName.Appearance), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Text4 {
			get {
				if(Elements.Count < 4)
					return null;
				return Elements[3].Text;
			}
			set {
				CreateContentElements(4);
				if(Elements[3].Text == value)
					return;
				Elements[3].Text = value;
				OnPropertiesChanged();
			}
		}
		void ResetAppearanceItem() { AppearanceItem.Reset(); }
		bool ShouldSerializeAppearanceItem() { return AppearanceItem.ShouldSerialize(); }
		TileItemAppearances appearances;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public TileItemAppearances AppearanceItem {
			get {
				if(appearances == null) {
					appearances = CreateAppearanceItem();
					appearances.Changed += new EventHandler(OnAppearanceChanged);
				}
				return appearances;
			}
		}
		protected virtual TileItemAppearances CreateAppearanceItem() {
			return new TileItemAppearances(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(CategoryName.Appearance)]
		public AppearanceObject Appearance {
			get { return AppearanceItem.Normal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(CategoryName.Appearance)]
		public AppearanceObject AppearanceHover {
			get { return AppearanceItem.Hovered; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(CategoryName.Appearance)]
		public AppearanceObject AppearanceSelected {
			get { return AppearanceItem.Selected; }
		}
		TileItemSize itemSize;
		[DefaultValue(TileItemSize.Default), Category(CategoryName.Appearance), SmartTagProperty("Item Size", "Appearance", 70, SmartTagActionType.RefreshAfterExecute), XtraSerializableProperty]
		public TileItemSize ItemSize {
			get { return itemSize; }
			set {
				if(ItemSize == value)
					return;
				itemSize = value;
				SetOptimizedBackgroundImageDirty();
				OnPropertiesChanged();
			}
		}
		protected internal virtual bool GetIsSmall() {
			return ItemSize == TileItemSize.Small;
		}
		protected internal virtual bool GetIsLarge() {
			return (ItemSize == TileItemSize.Large) || (ItemSize == TileItemSize.Wide);
		}
		protected internal virtual bool GetIsMedium() {
			return (ItemSize == TileItemSize.Default) || (ItemSize == TileItemSize.Medium);
		}
		[DefaultValue(false), Category(CategoryName.Appearance), Obsolete("Use 'ItemSize' instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsLarge {
			get { return GetIsLarge(); }
			set {
				if(GetIsLarge() == value) return;
				ItemSize = value ? TileItemSize.Wide : TileItemSize.Default;
			}
		}
		TileItemContentAlignment imageAlignment = TileItemContentAlignment.Default;
		[DefaultValue(TileItemContentAlignment.Default), Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), SmartTagProperty("Image Alignment", "Appearance", 30)]
		public TileItemContentAlignment ImageAlignment {
			get {
				if(imageAlignment != TileItemContentAlignment.Default) {
					CreateContentElements(1);
					Elements[0].SetImageAlignment(imageAlignment);
				}
				return imageAlignment;
			}
			set {
				if(ImageAlignment == value)
					return;
				CreateContentElements(1);
				Elements[0].ImageAlignment = value;
				imageAlignment = value;
				OnPropertiesChanged();
			}
		}
		TileItemContentAlignment backgroundImageAlignment = TileItemContentAlignment.Default;
		[DefaultValue(TileItemContentAlignment.Default), Category(CategoryName.Appearance)]
		public TileItemContentAlignment BackgroundImageAlignment {
			get { return backgroundImageAlignment; }
			set {
				if(BackgroundImageAlignment == value)
					return;
				backgroundImageAlignment = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ISite Site {
			get { return base.Site; }
			set {
				if(base.Site != value) {
					base.Site = value;
					if(value == null || string.IsNullOrEmpty(name) || DesignMode) return;
					try {
						Site.Name = name;
					}
					catch { }
				}
			}
		}
		string name = "";
		[DefaultValue(""), Browsable(false), XtraSerializableProperty]
		public virtual string Name {
			get {
				if(Site == null) return name;
				return Site.Name;
			}
			set {
				if(Site != null) {
					Site.Name = value;
					name = Site.Name;
				}
				else {
					name = value;
				}
			}
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			if(ItemInfo != null && ItemInfo.UseRenderImage) {
				ItemInfo.NeedUpdateAppearance = true;
				return;
			}
			OnPropertiesChanged();
		}
		protected internal virtual void OnPropertiesChanged() {
			if(Control != null)
				Control.OnPropertiesChanged();
		}
		protected internal virtual TileItemViewInfo CreateViewInfo() {
			return new TileItemViewInfo(this);
		}
		protected internal virtual void OnItemCheckedChanged() {
			RaiseCheckedChanged();
			if(Control != null)
				Control.OnItemCheckedChanged(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void OnItemPress() {
			RaiseItemPress();
			if(Control != null)
				Control.OnItemPress(this);
		}
		public void PerformItemClick() {
			OnItemClick();
		}
		public void PerformRightItemClick() {
			OnRightItemClick();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void OnItemClick() {
			RaiseItemClick();
			if(Control != null)
				Control.OnItemClick(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void OnItemPreDoubleClick() {
			if(EnableItemDoubleClickCore) OnItemDoubleClick();
			else OnItemClick();
		}
		protected internal virtual void OnItemDoubleClick() {
			RaiseItemDoubleClick();
			if(Control != null)
				Control.OnItemDoubleClick(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void OnRightItemClick() {
			RaiseRightItemClick();
			if(Control != null)
				Control.OnRightItemClick(this);
		}
		DefaultBoolean enableItemDoubleClick = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default), Category(CategoryName.Behavior)]
		public DefaultBoolean EnableItemDoubleClickEvent {
			get { return enableItemDoubleClick; }
			set { enableItemDoubleClick = value; }
		}
		bool EnableItemDoubleClickCore {
			get {
				if(EnableItemDoubleClickEvent != DefaultBoolean.Default) return EnableItemDoubleClickEvent.ToBoolean(false);
				return Control != null ? Control.EnableItemDoubleClickEvent : false;
			}
		}
		#region IAppearanceOwner Members
		bool IAppearanceOwner.IsLoading {
			get { return false; }
		}
		#endregion
		#region INotifyTextElementPropertiesChanged Members
		void INotifyElementPropertiesChanged.OnElementPropertiesChanged(TileItemElement element) {
			OnPropertiesChanged();
		}
		#endregion
		#region INotifyElementPropertiesChanged Members
		ITileControl INotifyElementPropertiesChanged.TileControl {
			get {
				return Control;
			}
		}
		#endregion
		protected internal virtual void TimerStartCore() {
			if(AllowAnimation && Frames != null && Frames.Count > 1)
				Timer.Start();
		}
		bool allowFrameAnimation = true;
		public void StopAnimation() {
			allowFrameAnimation = false;
			Timer.Stop();
		}
		public void NextFrame() {
			if(AllowAnimation)
				throw new InvalidOperationException("You can't use this method with AllowAnimation set to true");
			if(ItemInfo.IsChangingContent) return;
			CurrentFrameIndex++;
			if(CurrentFrameIndex >= Frames.Count)
				CurrentFrameIndex = 0;
			SetItemInfo(CurrentFrame);
		}
		bool allowAnimation = true;
		[DefaultValue(true), SmartTagProperty("Allow Animation", "Behavior")]
		public bool AllowAnimation {
			get { return allowAnimation; }
			set {
				if(AllowAnimation == value) return;
				if(!value) StopAnimation();
				allowAnimation = value;
			}
		}
		#region Commands
		public virtual IDisposable BindCommand(object command, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(tileItem, execute) => tileItem.ItemClick += (s, e) => execute(),
				(tileItem, canExecute) => tileItem.Enabled = canExecute(),
				command, queryCommandParameter);
		}
		public virtual IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(tileItem, execute) => tileItem.ItemClick += (s, e) => execute(),
				(tileItem, canExecute) => tileItem.Enabled = canExecute(),
				commandSelector, source, queryCommandParameter);
		}
		public virtual IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter = null) {
			 return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(tileItem, execute) => tileItem.ItemClick += (s, e) => execute(),
				(tileItem, canExecute) => tileItem.Enabled = canExecute(),
				commandSelector, source, () => (queryCommandParameter != null) ? queryCommandParameter() : default(T));
		}
		#endregion Commands
		bool allowSelectAnimation = true;
		[DefaultValue(true)]
		public bool AllowSelectAnimation {
			get { return allowSelectAnimation; }
			set {
				if(AllowSelectAnimation == value) return;
				if(!value) StopAnimation();
				allowSelectAnimation = value;
			}
		}
		internal bool GetAllowSelectAnimation() { return !IsDisposed && AllowSelectAnimation; }
		internal bool GetAllowAnimation() { return !IsDisposed && AllowAnimation; }
		protected internal virtual Image GetDisplayBackgroundImage() {
			EnsureOptimizedImageScale();
			if(needUpdateOptimizedBackgroundImage) {
				UpdateOptimizedBackgroundImage();
				needUpdateOptimizedBackgroundImage = false;
			}
			return BackgroundImage;
		}
		Size optimizedBackImgItemSize = new Size();
		void EnsureOptimizedImageScale() {
			if(BackgroundImage == null || ItemInfo == null)
				return;
			Size currentItemSize = ItemInfo.Bounds.Size;
			if(optimizedBackImgItemSize != currentItemSize) {
				SetOptimizedBackgroundImageDirty();
				optimizedBackImgItemSize = currentItemSize;
			}
		}
		void UpdateOptimizedBackgroundImage() {
			OptimizedBackgroundImage = null;
			if(BackgroundImage == null || ItemInfo == null || ItemInfo.Bounds.IsEmpty ||
			ItemInfo.BackgroundImageScaleMode == TileItemImageScaleMode.Default ||
			ItemInfo.BackgroundImageScaleMode == TileItemImageScaleMode.NoScale)
				return;
			Size boundsSize = ItemInfo.Bounds.Size;
			Size resultSize = TileItemImageScaleModeHelper.ScaleImage(boundsSize, BackgroundImage.Size, ItemInfo.BackgroundImageScaleMode);
			Bitmap bmp = new Bitmap(resultSize.Width, resultSize.Height);
			using(Graphics g = Graphics.FromImage((Image)bmp)) {
				g.DrawImage(BackgroundImage, 0, 0, resultSize.Width, resultSize.Height);
			}
			OptimizedBackgroundImage = (Image)bmp;
		}
		bool needUpdateOptimizedBackgroundImage;
		Image optimizedBackgroundImage;
		protected internal Image OptimizedBackgroundImage {
			get { return optimizedBackgroundImage; }
			set {
				if(value == optimizedBackgroundImage) return;
				if(value == null && optimizedBackgroundImage != null)
					optimizedBackgroundImage.Dispose();
				optimizedBackgroundImage = value;
			}
		}
		void SetOptimizedBackgroundImageDirty() {
			this.needUpdateOptimizedBackgroundImage = true;
		}
		#region ISupportAdornerElement Members
		Rectangle ISupportAdornerElement.Bounds {
			get {
				if(ItemInfo == null) return Rectangle.Empty;
				return ItemInfo.GetVisualEffectBounds();
			}
		}
		bool ISupportAdornerElement.IsVisible {
			get {
				return
					Visible &&
					Control != null &&
					Control.Handler.State != TileControlHandlerState.DragMode &&
					ItemInfo != null &&
					!ItemInfo.IsInTransition;
			}
		}
		event UpdateActionEventHandler ISupportAdornerElement.Changed {
			add { Events.AddHandler(targetChanged, value); }
			remove { Events.RemoveHandler(targetChanged, value); }
		}
		ISupportAdornerUIManager ISupportAdornerElement.Owner {
			get { return Control == null ? null : Control.ViewInfo as ISupportAdornerUIManager; }
		}
		protected internal void UpdateVisualEffects(UpdateAction action) {
			UpdateActionEventHandler handler = Events[targetChanged] as UpdateActionEventHandler;
			if(handler == null) return;
			UpdateActionEvendArgs e = new UpdateActionEvendArgs(action);
			handler(this, e);
		}
		#endregion
	}
	public class TileItemCollection : CollectionBase, IEnumerable<ITileItem>, IDisposable {
		public TileGroup Owner { get; private set; }
		public TileItemCollection(TileGroup group) {
			Owner = group;
		}
		protected override void OnClear() {
			foreach(TileItem item in List) {
				RemoveFromCache(item);
			}
			base.OnClear();
		}
		protected override void OnRemove(int index, object value) {
			RemoveFromCache((TileItem)List[index]);
			base.OnRemove(index, value);
		}
		public int Add(ITileItem item) {
			if(((TileItem)item).Collection != null)
				((TileItem)item).Collection.Remove((TileItem)item);
			return List.Add(item);
		}
		public void Insert(int index, TileItem item) {
			if(((TileItem)item).Collection != null)
				((TileItem)item).Collection.Remove((TileItem)item);
			List.Insert(index, item);
		}
		public void Remove(TileItem item) {
			if(!List.Contains(item)) return;
			List.Remove(item);
			ResetSelectedItem(item);
		}
		void ResetSelectedItem(TileItem removedItem) {
			if(removedItem == null || removedItem.ItemInfo == null || removedItem.ItemInfo.ControlInfo == null) return;
			var cInfo = removedItem.ItemInfo.ControlInfo;
			if(cInfo.Owner != null && cInfo.Owner.SelectedItem == removedItem && cInfo.ShouldResetSelectedItem) {
				cInfo.Owner.SelectedItem = null;
			}
			cInfo.ShouldResetSelectedItem = true;
		}
		protected internal void RemoveFromCache(TileItem item) {
			if(item.ItemInfo != null && item.ItemInfo.ShouldRemoveFromCache) {
				if(item.Control != null && item.Control.ViewInfo != null)
					item.Control.ViewInfo.RemoveItemFromCache(item);
			}
			if(item != null && item.ItemInfo != null)
				item.ItemInfo.ShouldRemoveFromCache = true;
		}
		public int IndexOf(TileItem item) { return List.IndexOf(item); }
		public bool Contains(TileItem item) { return List.Contains(item); }
		public virtual TileItem this[int index] { get { return (TileItem)List[index]; } set { List[index] = value; } }
		public TileItem this[string index] {
			get {
				foreach(TileItem item in List) {
					if(item.Name == index) return item;
				}
				return null;
			}
			set {
				int itemIndex = 0;
				foreach(TileItem item in List) {
					if(item.Name == index) List[itemIndex] = value;
					itemIndex++;
				}
			}
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			SetCollection((TileItem)value);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			OnCollectionChanged();
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			ClearCollection((TileItem)value);
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			ClearCollection((TileItem)oldValue);
			SetCollection((TileItem)newValue);
		}
		void SetCollection(TileItem item) {
			if(item != null)
				item.Collection = this;
			OnCollectionChanged();
		}
		void ClearCollection(TileItem item) {
			if(item.Control != null && item.Control.ViewInfo.PressedInfo.ItemInfo == item.ItemInfo)
				item.Control.ViewInfo.SetPressedInfoCore(null);
			if(item != null)
				item.Collection = null;
			OnCollectionChanged();
		}
		protected virtual void OnCollectionChanged() {
			if(Owner == null || Owner.Control == null) return;
			Owner.Control.OnPropertiesChanged();
		}
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			Owner = null;
		}
		public new IEnumerator<ITileItem> GetEnumerator() {
			return GetEnumeratorCore();
		}
		protected virtual IEnumerator<ITileItem> GetEnumeratorCore() {
			foreach(var item in List)
				yield return (ITileItem)item;
		}
	}
	public class TileGroupCollection : CollectionBase, IEnumerable<TileGroup> {
		public ITileControl Owner { get; private set; }
		public TileGroupCollection(ITileControl control) {
			Owner = control;
		}
		bool shouldRemoveItemsFromCache = true;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShouldRemoveItemsFromCache {
			get { return shouldRemoveItemsFromCache; }
			set { shouldRemoveItemsFromCache = value; }
		}
		public int Add(TileGroup group) {
			if(group.Groups != null && group.Groups.Contains(group))
				group.Groups.Remove(group);
			return List.Add(group);
		}
		public void Insert(int index, TileGroup group) {
			if(index >= Count)
				Add(group);
			else
				List.Insert(index, group);
		}
		public void Remove(TileGroup group) { List.Remove(group); }
		public int IndexOf(TileGroup group) { return List.IndexOf(group); }
		public bool Contains(TileGroup group) { return List.Contains(group); }
		public TileGroup this[int index] { get { return (TileGroup)List[index]; } set { List[index] = value; } }
		public TileGroup this[string index] {
			get {
				foreach(TileGroup group in List) {
					if(group.Name == index) return group;
				}
				return null;
			}
			set {
				int groupIndex = 0;
				foreach(TileGroup group in List) {
					if(group.Name == index) {
						List[groupIndex] = value;
						break;
					}
					groupIndex++;
				}
			}
		}
		protected override void OnClear() {
			if(ShouldRemoveItemsFromCache) {
				foreach(TileGroup group in List) {
					foreach(TileItem item in group.Items) {
						if(item.ItemInfo != null)
							item.ItemInfo.ShouldRemoveFromCache = true;
						group.Items.RemoveFromCache(item);
					}
				}
			}
			ShouldRemoveItemsFromCache = true;
			base.OnClear();
		}
		protected override void OnRemove(int index, object value) {
			if(ShouldRemoveItemsFromCache) {
				foreach(TileItem item in ((TileGroup)value).Items) {
					if(item.ItemInfo != null)
						item.ItemInfo.ShouldRemoveFromCache = true;
					((TileGroup)value).Items.RemoveFromCache(item);
				}
			}
			ShouldRemoveItemsFromCache = true;
			base.OnRemove(index, value);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			OnCollectionChanged();
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			SetCollection((TileGroup)value);
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			ClearCollection((TileGroup)value);
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			ClearCollection((TileGroup)oldValue);
			SetCollection((TileGroup)newValue);
		}
		void SetCollection(TileGroup item) {
			if(item != null)
				item.Groups = this;
			OnCollectionChanged();
		}
		void ClearCollection(TileGroup item) {
			if(item != null)
				item.Groups = null;
			OnCollectionChanged();
		}
		protected virtual void OnCollectionChanged() {
			if(Owner != null)
				Owner.OnPropertiesChanged();
		}
		IEnumerator<TileGroup> IEnumerable<TileGroup>.GetEnumerator() {
			foreach(TileGroup group in List)
				yield return group;
		}
	}
	public enum TileControlScrollMode { Default, ScrollBar, ScrollButtons, None }
	public enum TileControlImageToTextAlignment { Default, None, Top, Left, Right, Bottom }
	public enum TileItemContentShowMode { Default, Always, Hover }
	public enum TileItemContentAnimationType { Default, ScrollTop, ScrollDown, ScrollLeft, ScrollRight, Fade, SegmentedFade, RandomSegmentedFade }
	public interface ITileControlProperties {
		int RowCount { get; set; }
		int ColumnCount { get; set; }
		int ItemSize { get; set; }
		int LargeItemWidth { get; set; }
		int IndentBetweenItems { get; set; }
		int IndentBetweenGroups { get; set; }
		Orientation Orientation { get; set; }
		HorzAlignment HorizontalContentAlignment { get; set; }
		VertAlignment VerticalContentAlignment { get; set; }
		Padding Padding { get; set; }
		Padding ItemPadding { get; set; }
		ImageLayout BackgroundImageLayout { get; set; }
		TileItemContentAlignment ItemImageAlignment { get; set; }
		TileItemContentAlignment ItemBackgroundImageAlignment { get; set; }
		TileItemImageScaleMode ItemImageScaleMode { get; set; }
		TileItemImageScaleMode ItemBackgroundImageScaleMode { get; set; }
		TileItemContentAnimationType ItemContentAnimation { get; set; }
		TileItemContentShowMode ItemTextShowMode { get; set; }
		TileItemBorderVisibility ItemBorderVisibility { get; set; }
		TileItemCheckMode ItemCheckMode { get; set; }
		bool AllowHtmlDraw { get; set; }
		bool AllowItemHover { get; set; }
		bool AllowSelectedItem { get; set; }
		bool AllowDrag { get; set; }
		bool AllowGroupHighlighting { get; set; }
		GroupHighlightingProperties AppearanceGroupHighlighting { get; set; }
		bool ShowText { get; set; }
		bool ShowGroupText { get; set; }
		void Assign(ITileControlProperties source);
	}
	public interface ITileControl {
		bool Capture { get; set; }
		TileGroupCollection Groups { get; }
		void OnPropertiesChanged();
		TileControlViewInfo ViewInfo { get; }
		TileControlPainter SourcePainter { get; }
		TileControlNavigator Navigator { get; }
		TileControlHandler Handler { get; }
		void Invalidate(Rectangle rect);
		ISupportXtraAnimation AnimationHost { get; }
		Color BackColor { get; }
		Color SelectionColor { get; }
		Rectangle Bounds { get; }
		Rectangle ClientRectangle { get; }
		bool AnimateArrival { get; set; }
		Image BackgroundImage { get; set; }
		ScrollBarBase ScrollBar { get; set; }
		void AddControl(Control control);
		bool ContainsControl(Control control);
		bool EnableItemDoubleClickEvent { get; set; }
		int Position { get; set; }
		void BeginUpdate();
		void EndUpdate();
		bool IsLockUpdate { get; }
		Point PointToClient(Point pt);
		Point PointToScreen(Point pt);
		SizeF ScaleFactor { get; }
		Size DragSize { get; set; }
		void OnItemPress(TileItem tileItem);
		void OnItemClick(TileItem tileItem);
		void OnItemDoubleClick(TileItem tileItem);
		void OnRightItemClick(TileItem tileItem);
		void OnItemCheckedChanged(TileItem tileItem);
		event TileItemClickEventHandler ItemClick;
		event TileItemClickEventHandler ItemDoubleClick;
		event TileItemClickEventHandler RightItemClick;
		event TileItemClickEventHandler ItemPress;
		bool Focus();
		bool AllowDrag { get; }
		bool AllowGlyphSkinning { get; set; }
		bool AllowDragTilesBetweenGroups { get; set; }
		event TileItemClickEventHandler ItemCheckedChanged;
		TileControlScrollMode ScrollMode { get; set; }
		void SuspendAnimation();
		void ResumeAnimation();
		bool IsAnimationSuspended { get; }
		TileItem SelectedItem { get; set; }
		event TileItemClickEventHandler SelectedItemChanged;
		event TileItemDragEventHandler StartItemDragging;
		event TileItemDragEventHandler EndItemDragging;
		bool IsDesignMode { get; }
		ISite Site { get; }
		IContainer Container { get; }
		Control Control { get; }
		TileItemDragEventArgs RaiseStartItemDragging(TileItem item);
		TileItemDragEventArgs RaiseEndItemDragging(TileItem item, TileGroup targetGroup);
		TileItemAppearances AppearanceItem { get; }
		AppearanceObject AppearanceText { get; }
		AppearanceObject AppearanceGroupText { get; }
		BorderStyles BorderStyle { get; set; }
		UserLookAndFeel LookAndFeel { get; }
		bool IsHandleCreated { get; }
		IntPtr Handle { get; }
		object Images { get; set; }
		int ScrollButtonFadeAnimationTime { get; set; }
		TileControlHitInfo CalcHitInfo(Point pt);
		string Text { get; set; }
		bool Enabled { get; set; }
		bool AutoSelectFocusedItem { get; set; }
		bool AllowItemContentAnimation { get; }
		bool AllowSelectedItem { get; }
		bool AllowDisabledStateIndication { get; set; }
		ContextItemCollection ContextButtons { get; }
		ContextItemCollectionOptions ContextButtonOptions { get; }
		void RaiseContextItemClick(ContextItemClickEventArgs e);
		void RaiseContextButtonCustomize(ITileItem tileItem, ContextItem item);
		void RaiseCustomContextButtonToolTip(ITileItem tileItem, ContextButtonToolTipEventArgs e);
		ITileControlProperties Properties { get; }
		bool DebuggingState { get; }
		void UpdateSmartTag();
	}
	public class TileControlSmartTagEventArgs {
		public ITileControl TileControl { get; set; }
		public object Info { get; set; }
	}
	public delegate void SmartTagUpdateEventHandler(object sender, TileControlSmartTagEventArgs e);
	public interface ITileControlUpdateSmartTag {
		event SmartTagUpdateEventHandler SmartTagUpdate;
	}
	[DXToolboxItem(true),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "TileControl"),
	Description("Allows you to build Windows UI inspired interfaces."),
	Designer("DevExpress.XtraBars.Design.TileControlDesigner, " + AssemblyInfo.SRAssemblyBarsDesign),
	ToolboxTabName(AssemblyInfo.DXTabNavigation), Docking(DockingBehavior.Ask)
]
	public class TileControl : Control, ITileControl, ITileControlUpdateSmartTag, ITileControlProperties, ISupportXtraAnimation, ISupportLookAndFeel, IAppearanceOwner, IXtraSerializable,
			IMouseWheelScrollClient, IMouseWheelSupport, IToolTipControlClient, IContextItemCollectionOptionsOwner, IContextItemCollectionOwner {
		private static readonly object itemClick = new object();
		private static readonly object itemDoubleClick = new object();
		private static readonly object rightItemClick = new object();
		private static readonly object itemPress = new object();
		private static readonly object itemCheckedChanged = new object();
		private static readonly object selectedItemChanged = new object();
		private static readonly object startDragging = new object();
		private static readonly object endDragging = new object();
		private static readonly object contextButtonCustomize = new object();
		private static readonly object contextButtonClick = new object();
		private static readonly object customContextButtonToolTip = new object();
		static Color DefaultSelectionColor = Color.White;
		public TileControl() {
			((ITileControl)this).ScrollBar = ViewInfoCore.CreateScrollBarCore(new HScrollBar());
			this.selectionColor = DefaultSelectionColor;
			this.allowHtmlText = DefaultBoolean.Default;
			this.rowCount = 5;
			this.itemImageAlignment = TileItemContentAlignment.Default;
			this.indentBetweenItems = 8;
			this.indentBetweenGroups = 56;
			this.itemPadding = DefaultItemPadding;
			this.animateArrival = true;
			this.lookAndFeel = new UserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.Selectable | ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.BackColor = Color.Empty;
			this.appearanceGroupHighlighting = new GroupHighlightingProperties();
			this.MaxId = 0;
			ItemContentAnimation = TileItemContentAnimationType.Default;
			AllowDisabledStateIndication = true;
			IsFirstPaintingComplete = false;
			UseParentAutoScaleFactor = true;
			AllowDragTilesBetweenGroups = true;
			ShowToolTips = true;
			ToolTipController.DefaultController.AddClientControl(this);
		}
		ToolTipController toolTipController;
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseControlToolTipController"),
#endif
 DefaultValue(null)]
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(ToolTipController == value) return;
				if(ToolTipController != null)
					ToolTipController.RemoveClientControl(this);
				toolTipController = value;
				if(ToolTipController != null) {
					ToolTipController.DefaultController.RemoveClientControl(this);
					ToolTipController.AddClientControl(this);
				}
				else ToolTipController.DefaultController.AddClientControl(this);
			}
		}
		[DefaultValue(true), Category(CategoryName.Behavior)]
		public bool ShowToolTips { get; set; }
		[DefaultValue(true), Category(CategoryName.Behavior)]
		public bool AllowDragTilesBetweenGroups { get; set; }
		bool allowGlyphSkinning = false;
		[DefaultValue(false), Category(CategoryName.Appearance)]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(allowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				OnPropertiesChanged();
			}
		}
		[Category(CategoryName.Behavior)]
		public Size DragSize { get; set; }
		bool enableItemDoubleClick = true;
		[DefaultValue(true), Category(CategoryName.Behavior)]
		public bool EnableItemDoubleClickEvent {
			get { return enableItemDoubleClick; }
			set { enableItemDoubleClick = value; }
		}
		Form ParentForm { get; set; }
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			FormUnsubscribe(ParentForm);
			Form f = FindForm();
			if(f == null) return;
			FormSubscribe(f);
			ParentForm = f;
		}
		protected virtual void FormSubscribe(Form f) {
			if(f == null) return;
			f.Deactivate += ParentFormDeactivated;
		}
		protected virtual void FormUnsubscribe(Form f) {
			if(f == null) return;
			f.Deactivate -= ParentFormDeactivated;
		}
		void ParentFormDeactivated(object sender, EventArgs e) {
			ViewInfoCore.canStartPressAnimation = false;
		}
		static readonly Size TileControlDefaultSize = new Size(240, 150);
		protected override Size DefaultSize {
			get { return TileControlDefaultSize; }
		}
		ITileControlProperties ITileControl.Properties { get { return this; } }
		bool ITileControlProperties.AllowHtmlDraw {
			get { return AllowHtmlText != DefaultBoolean.False; }
			set { AllowHtmlText = value ? DefaultBoolean.Default : DefaultBoolean.False; }
		}
		void ITileControlProperties.Assign(ITileControlProperties source) {
			BeginUpdate();
			this.allowDrag = source.AllowDrag;
			this.allowGroupHighlighting = source.AllowGroupHighlighting;
			this.appearanceGroupHighlighting = source.AppearanceGroupHighlighting.Clone() as GroupHighlightingProperties;
			this.allowHtmlText = (source.AllowHtmlDraw ? DefaultBoolean.Default : DefaultBoolean.False);
			this.allowItemHover = source.AllowItemHover;
			this.allowSelectedItem = source.AllowSelectedItem;
			BackgroundImageLayout = source.BackgroundImageLayout;
			this.horizontalContentAlignment = source.HorizontalContentAlignment;
			this.indentBetweenGroups = source.IndentBetweenGroups;
			this.indentBetweenItems = source.IndentBetweenItems;
			this.itemBackgroundImageAlignment = source.ItemBackgroundImageAlignment;
			this.itemBackgroundImageScaleMode = source.ItemBackgroundImageScaleMode;
			this.itemCheckMode = source.ItemCheckMode;
			this.ItemContentAnimation = source.ItemContentAnimation;
			this.itemImageAlignment = source.ItemImageAlignment;
			this.itemImageScaleMode = source.ItemImageScaleMode;
			this.itemPadding = source.ItemPadding;
			this.itemSize = source.ItemSize;
			this.itemTextShowMode = source.ItemTextShowMode;
			this.itemBorderMode = source.ItemBorderVisibility;
			this.largeItemWidth = source.LargeItemWidth;
			this.orientation = source.Orientation;
			Padding = source.Padding;
			this.rowCount = source.RowCount;
			this.columnCount = source.ColumnCount;
			this.showGroupText = source.ShowGroupText;
			this.showText = source.ShowText;
			this.verticalContentAlignment = source.VerticalContentAlignment;
			EndUpdate();
		}
		ITileControlDesigner Designer {
			get {
				IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host != null) return host.GetDesigner(this) as ITileControlDesigner;
				return null;
			}
		}
		bool ITileControl.DebuggingState {
			get {
				if(Designer == null)
					return false;
				return Designer.DebuggingState;
			}
		}
		[DefaultValue(0), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int MaxId { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int GetNextId() {
			return MaxId++;
		}
		void OnLookAndFeelChanged(object sender, EventArgs e) {
			ViewInfoCore.ClearBorderPainter();
			ResetItemsDefaultAppearances();
			OnPropertiesChanged();
		}
		void ResetItemsDefaultAppearances() {
			foreach(TileGroupViewInfo grInfo in ViewInfoCore.Groups)
				foreach(TileItemViewInfo itemInfo in grInfo.Items)
					itemInfo.ResetDefaultAppearance();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AnimateArrival {
			get { return animateArrival; }
			set { animateArrival = value; }
		}
		bool allowDrag = true;
		[DefaultValue(true), Category(CategoryName.Behavior)]
		public bool AllowDrag {
			get { return allowDrag; }
			set { allowDrag = value; }
		}
		bool allowGroupHighlighting = false;
		[DefaultValue(false), Category(CategoryName.Appearance)]
		public bool AllowGroupHighlighting {
			get { return allowGroupHighlighting; }
			set { allowGroupHighlighting = value; }
		}
		GroupHighlightingProperties appearanceGroupHighlighting;
		[DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GroupHighlightingProperties AppearanceGroupHighlighting {
			get { return appearanceGroupHighlighting; }
			set { appearanceGroupHighlighting = value; }
		}
		bool ITileControl.IsDesignMode { get { return DesignMode; } }
		Control ITileControl.Control { get { return this; } }
		[Category(CategoryName.Behavior)]
		public event TileItemDragEventHandler StartItemDragging {
			add { Events.AddHandler(startDragging, value); }
			remove { Events.RemoveHandler(startDragging, value); }
		}
		[Category(CategoryName.Behavior)]
		public event TileItemDragEventHandler EndItemDragging {
			add { Events.AddHandler(endDragging, value); }
			remove { Events.RemoveHandler(endDragging, value); }
		}
		TileItemDragEventArgs ITileControl.RaiseStartItemDragging(TileItem item) {
			TileItemDragEventHandler handler = Events[startDragging] as TileItemDragEventHandler;
			TileItemDragEventArgs e = new TileItemDragEventArgs() { Item = item };
			if(handler != null)
				handler(this, e);
			return e;
		}
		TileItemDragEventArgs ITileControl.RaiseEndItemDragging(TileItem item, TileGroup targetGroup) {
			TileItemDragEventHandler handler = Events[endDragging] as TileItemDragEventHandler;
			TileItemDragEventArgs e = new TileItemDragEventArgs() { Item = item, TargetGroup = targetGroup };
			if(handler != null)
				handler(this, e);
			return e;
		}
		[Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public override Image BackgroundImage {
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}
		protected override void OnBackgroundImageLayoutChanged(EventArgs e) {
			ViewInfoCore.ResetBackgroundImage();
			base.OnBackgroundImageLayoutChanged(e);
		}
		protected override void OnBackgroundImageChanged(EventArgs e) {
			ViewInfoCore.ResetBackgroundImage();
			base.OnBackgroundImageChanged(e);
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			Invalidate();
		}
		UserLookAndFeel lookAndFeel;
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public UserLookAndFeel LookAndFeel {
			get { return lookAndFeel; }
		}
		bool allowSelectedItem = false;
		[DefaultValue(false), Category(CategoryName.Behavior)]
		public bool AllowSelectedItem {
			get { return allowSelectedItem; }
			set {
				if(AllowSelectedItem == value)
					return;
				allowSelectedItem = value;
				OnAllowSelectedItemChanged();
			}
		}
		bool autoSelectFocusedItem = true;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AutoSelectFocusedItem {
			get { return autoSelectFocusedItem; }
			set {
				if(AutoSelectFocusedItem == value)
					return;
				autoSelectFocusedItem = value;
				OnAutoSelectFocusedItemChanged();
			}
		}
		protected virtual void OnAllowSelectedItemChanged() {
			if(!AllowSelectedItem)
				this.selectedItem = null;
			OnPropertiesChanged();
		}
		protected virtual void OnAutoSelectFocusedItemChanged() {
			OnPropertiesChanged();
		}
		[DefaultValue(TileItemContentAnimationType.Default), Category(CategoryName.Appearance)]
		public TileItemContentAnimationType ItemContentAnimation {
			get;
			set;
		}
		TileItem selectedItem = null;
		[DefaultValue(null), Category(CategoryName.Properties)]
		public TileItem SelectedItem {
			get { return selectedItem; }
			set {
				if(SelectedItem == value)
					return;
				TileItem oldItem = SelectedItem;
				selectedItem = value;
				OnSelectedItemChanged(oldItem, value);
			}
		}
		void OnSelectedItemChanged(TileItem oldItem, TileItem newItem) {
			RaiseSelectedItemChanged(newItem);
			if(oldItem != null && oldItem.ItemInfo != null && oldItem.Visible) {
				oldItem.ItemInfo.ForceUpdateAppearanceColors();
				ViewInfoCore.RemoveHoverAnimation(oldItem.ItemInfo, false);
			}
			if(newItem != null && newItem.ItemInfo != null) {
				newItem.ItemInfo.ForceUpdateAppearanceColors();
				ViewInfoCore.RemoveHoverAnimation(newItem.ItemInfo, false);
				if(!newItem.ItemInfo.IsFullyVisible) {
					ViewInfoCore.MakeVisible(newItem.ItemInfo);
				}
			}
			Invalidate(ClientRectangle);
		}
		BorderStyles borderStyle = BorderStyles.NoBorder;
		[DefaultValue(BorderStyles.NoBorder), Category(CategoryName.Appearance)]
		public BorderStyles BorderStyle {
			get { return borderStyle; }
			set {
				if(BorderStyle == value)
					return;
				borderStyle = value;
				OnBorderStyleChanged();
			}
		}
		protected virtual void OnBorderStyleChanged() {
			ViewInfoCore.ClearBorderPainter();
			OnPropertiesChanged();
		}
		bool showText;
		[DefaultValue(false), Category(CategoryName.Appearance)]
		public bool ShowText {
			get { return showText; }
			set {
				if(ShowText == value)
					return;
				showText = value;
				OnPropertiesChanged();
			}
		}
		bool showGroupText;
		[DefaultValue(false), Category(CategoryName.Appearance)]
		public bool ShowGroupText {
			get { return showGroupText; }
			set {
				if(ShowGroupText == value)
					return;
				showGroupText = value;
				OnPropertiesChanged();
			}
		}
		bool allowItemHover = false;
		[DefaultValue(false), Category(CategoryName.Behavior)]
		public bool AllowItemHover {
			get { return allowItemHover; }
			set {
				if(AllowItemHover == value)
					return;
				allowItemHover = value;
				OnPropertiesChanged();
			}
		}
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			OnPropertiesChanged();
		}
		const int defaultScrollBtnFadeAnimationTime = 600;
		int scrollBtnFadeAnimationTime = defaultScrollBtnFadeAnimationTime;
		[DefaultValue(defaultScrollBtnFadeAnimationTime), Category(CategoryName.Appearance)]
		public int ScrollButtonFadeAnimationTime {
			get { return scrollBtnFadeAnimationTime; }
			set {
				if(scrollBtnFadeAnimationTime == value)
					return;
				if(value < 0)
					throw new ArgumentException();
				if(value == 0) value = 1;
				scrollBtnFadeAnimationTime = value;
			}
		}
		TileItemContentShowMode itemTextShowMode = TileItemContentShowMode.Default;
		[DefaultValue(TileItemContentShowMode.Default), Category(CategoryName.Appearance)]
		public TileItemContentShowMode ItemTextShowMode {
			get { return itemTextShowMode; }
			set {
				if(ItemTextShowMode == value)
					return;
				itemTextShowMode = value;
				OnPropertiesChanged();
			}
		}
		int AnimationSuspendCount { get; set; }
		[Browsable(false)]
		public bool IsAnimationSuspended {
			get { return AnimationSuspendCount > 0; }
		}
		public void SuspendAnimation() {
			if(AnimationSuspendCount == 0)
				SuspendAnimationCore();
			AnimationSuspendCount++;
		}
		protected virtual void SuspendAnimationCore() {
			foreach(TileGroup group in Groups) {
				foreach(TileItem item in group.Items) {
					if(item.Timer.Enabled) {
						item.Timer.Stop();
						if(item.CurrentFrame != null)
							item.SetContent(item.CurrentFrame, false);
					}
				}
			}
		}
		public void ResumeAnimation() {
			if(AnimationSuspendCount > 0)
				AnimationSuspendCount--;
			if(AnimationSuspendCount == 0)
				ResumeAnimationCore();
		}
		protected virtual void ResumeAnimationCore() {
			foreach(TileGroup group in Groups) {
				foreach(TileItem item in group.Items) {
					item.TimerStartCore();
				}
			}
		}
		int largeItemWidth = -1;
		[DefaultValue(-1), Category(CategoryName.Appearance)]
		int ITileControlProperties.LargeItemWidth {
			get { return largeItemWidth; }
			set {
				if(largeItemWidth == value)
					return;
				this.largeItemWidth = value;
				OnPropertiesChanged();
			}
		}
		TileItemBorderVisibility itemBorderMode = TileItemBorderVisibility.Auto;
		[DefaultValue(TileItemBorderVisibility.Auto), Category(CategoryName.Appearance)]
		public TileItemBorderVisibility ItemBorderVisibility {
			get { return itemBorderMode; }
			set {
				if(itemBorderMode == value)
					return;
				itemBorderMode = value;
				OnPropertiesChanged();
			}
		}
		TileItemCheckMode itemCheckMode = TileItemCheckMode.None;
		[DefaultValue(TileItemCheckMode.None), Category(CategoryName.Behavior)]
		public TileItemCheckMode ItemCheckMode {
			get { return itemCheckMode; }
			set {
				if(ItemCheckMode == value)
					return;
				itemCheckMode = value;
				OnCheckModeChanged();
			}
		}
		TileControlScrollMode scrollMode = TileControlScrollMode.Default;
		[DefaultValue(TileControlScrollMode.Default), Category(CategoryName.Behavior)]
		public TileControlScrollMode ScrollMode {
			get { return scrollMode; }
			set {
				if(ScrollMode == value)
					return;
				scrollMode = value;
				OnPropertiesChanged();
			}
		}
		protected virtual void OnCheckModeChanged() {
			bool setFirstChecked = false;
			if(ItemCheckMode == TileItemCheckMode.Multiple)
				return;
			foreach(TileGroup group in Groups) {
				foreach(TileItem item in group.Items) {
					if(item.Checked) {
						if(ItemCheckMode == TileItemCheckMode.Single && !setFirstChecked) {
							setFirstChecked = true;
						}
						item.Checked = false;
					}
				}
			}
			Invalidate(ClientRectangle);
		}
		TileItemImageScaleMode itemImageScaleMode;
		[DefaultValue(TileItemImageScaleMode.Default), Category(CategoryName.Appearance)]
		public TileItemImageScaleMode ItemImageScaleMode {
			get { return itemImageScaleMode; }
			set {
				if(ItemImageScaleMode == value)
					return;
				itemImageScaleMode = value;
				OnPropertiesChanged();
			}
		}
		TileItemImageScaleMode itemBackgroundImageScaleMode;
		[DefaultValue(TileItemImageScaleMode.Default), Category(CategoryName.Appearance)]
		public TileItemImageScaleMode ItemBackgroundImageScaleMode {
			get { return itemBackgroundImageScaleMode; }
			set {
				if(ItemBackgroundImageScaleMode == value)
					return;
				itemBackgroundImageScaleMode = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public bool AllowItemContentAnimation { get { return IsHandleCreated; } }
		Orientation orientation;
		[DefaultValue(Orientation.Horizontal), Category(CategoryName.Appearance)]
		public Orientation Orientation {
			get { return orientation; }
			set {
				if(Orientation == value)
					return;
				orientation = value;
				OnOrientationChanged();
			}
		}
		protected virtual void OnOrientationChanged() {
			if(Orientation == Orientation.Horizontal)
				((ITileControl)this).ScrollBar = ViewInfoCore.CreateScrollBarCore(new HScrollBar());
			else
				((ITileControl)this).ScrollBar = ViewInfoCore.CreateScrollBarCore(new VScrollBar());
			OnPropertiesChanged();
		}
		HorzAlignment horizontalContentAlignment = HorzAlignment.Default;
		[DefaultValue(HorzAlignment.Default), Category(CategoryName.Appearance)]
		public HorzAlignment HorizontalContentAlignment {
			get { return horizontalContentAlignment; }
			set {
				if(HorizontalContentAlignment == value)
					return;
				horizontalContentAlignment = value;
				OnPropertiesChanged();
			}
		}
		VertAlignment verticalContentAlignment = VertAlignment.Default;
		[DefaultValue(VertAlignment.Default), Category(CategoryName.Appearance)]
		public VertAlignment VerticalContentAlignment {
			get { return verticalContentAlignment; }
			set {
				if(VerticalContentAlignment == value)
					return;
				verticalContentAlignment = value;
				OnPropertiesChanged();
			}
		}
		TileItemContentAlignment itemImageAlignment;
		[DefaultValue(TileItemContentAlignment.Default), Category(CategoryName.Appearance)]
		public TileItemContentAlignment ItemImageAlignment {
			get { return itemImageAlignment; }
			set {
				if(ItemImageAlignment == value)
					return;
				itemImageAlignment = value;
				OnPropertiesChanged();
			}
		}
		TileItemContentAlignment itemBackgroundImageAlignment = TileItemContentAlignment.Default;
		[DefaultValue(TileItemContentAlignment.Default), Category(CategoryName.Appearance)]
		public TileItemContentAlignment ItemBackgroundImageAlignment {
			get { return itemBackgroundImageAlignment; }
			set {
				if(ItemBackgroundImageAlignment == value)
					return;
				itemBackgroundImageAlignment = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		TileGroupCollection groups;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Collection, false, true, false)]
		public TileGroupCollection Groups {
			get {
				if(groups == null)
					groups = CreateGroups();
				return groups;
			}
		}
		protected override Padding DefaultPadding {
			get {
				return new Padding(18);
			}
		}
		protected virtual TileGroupCollection CreateGroups() {
			return new TileGroupCollection(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TileControlViewInfo ViewInfoCore { get { return ((ITileControl)this).ViewInfo; } }
		TileControlNavigator ITileControl.Navigator {
			get { return NavigatorCore; }
		}
		TileControlNavigator navigator;
		protected TileControlNavigator NavigatorCore {
			get {
				if(navigator == null)
					navigator = CreateNavigator();
				return navigator;
			}
		}
		protected virtual TileControlNavigator CreateNavigator() {
			return new TileControlNavigator(this);
		}
		public void OnPropertiesChanged() {
			if(IsLockUpdate)
				return;
			ViewInfoCore.IsReady = false;
			Invalidate();
		}
		TileControlViewInfo viewInfo;
		TileControlViewInfo ITileControl.ViewInfo {
			get {
				if(viewInfo == null)
					viewInfo = CreateViewInfo();
				return viewInfo;
			}
		}
		TileControlPainter ITileControl.SourcePainter {
			get { return Painter; }
		}
		bool ITileControl.ContainsControl(Control control) {
			return Controls.Contains(control);
		}
		void ITileControl.AddControl(Control control) {
			Controls.Add(control);
		}
		protected virtual TileControlViewInfo CreateViewInfo() {
			return new TileControlViewInfo(this);
		}
		void ResetAppearanceText() { AppearanceText.Reset(); }
		bool ShouldSerializeAppearanceText() { return AppearanceText.ShouldSerialize(); }
		AppearanceObject appearanceText;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public AppearanceObject AppearanceText {
			get {
				if(appearanceText == null) {
					appearanceText = new AppearanceObject();
					appearanceText.Changed += new EventHandler(OnAppearanceChanged);
				}
				return appearanceText;
			}
		}
		void ResetAppearanceGroupText() { AppearanceGroupText.Reset(); }
		bool ShouldSerializeAppearanceGroupText() { return AppearanceGroupText.ShouldSerialize(); }
		AppearanceObject appearanceGroupText;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public AppearanceObject AppearanceGroupText {
			get {
				if(appearanceGroupText == null) {
					appearanceGroupText = new AppearanceObject();
					appearanceGroupText.Changed += new EventHandler(OnAppearanceChanged);
				}
				return appearanceGroupText;
			}
		}
		void ResetAppearanceItem() { AppearanceItem.Reset(); }
		bool ShouldSerializeAppearanceItem() { return AppearanceItem.ShouldSerialize(); }
		TileItemAppearances appearances;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public TileItemAppearances AppearanceItem {
			get {
				if(appearances == null) {
					appearances = new TileItemAppearances(this);
					appearances.Changed += new EventHandler(OnAppearanceChanged);
				}
				return appearances;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(CategoryName.Appearance)]
		public AppearanceObject ItemAppearance {
			get { return AppearanceItem.Normal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(CategoryName.Appearance)]
		public AppearanceObject ItemAppearanceHover {
			get { return AppearanceItem.Hovered; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(CategoryName.Appearance)]
		public AppearanceObject ItemAppearanceSelected {
			get { return AppearanceItem.Selected; }
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		int UpdateCount { get; set; }
		public void BeginUpdate() {
			UpdateCount++;
		}
		public void EndUpdate() {
			if(UpdateCount == 0)
				return;
			UpdateCount--;
			if(UpdateCount == 0)
				OnPropertiesChanged();
		}
		[Browsable(false)]
		public bool IsLockUpdate { get { return UpdateCount > 0; } }
		bool ShouldSerializeSelectionColor() { return SelectionColor != DefaultSelectionColor; }
		void ResetSelectionColor() { SelectionColor = DefaultSelectionColor; }
		Color selectionColor;
		[Category(CategoryName.Appearance)]
		public Color SelectionColor {
			get { return selectionColor; }
			set {
				if(SelectionColor == value)
					return;
				selectionColor = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("Use the SelectionColor property instead")]
		public Color FocusRectColor {
			get { return SelectionColor; }
			set { SelectionColor = value; }
		}
		DefaultBoolean allowHtmlText;
		[DefaultValue(DefaultBoolean.Default), Category(CategoryName.Appearance)]
		public DefaultBoolean AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(AllowHtmlText == value)
					return;
				allowHtmlText = value;
				OnPropertiesChanged();
			}
		}
		bool animateArrival = true;
		[DefaultValue(true)]
		bool ITileControl.AnimateArrival {
			get { return animateArrival; }
			set { animateArrival = value; }
		}
		object images;
		[DefaultValue(null),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter)),
		Category(CategoryName.Appearance)]
		public object Images {
			get { return images; }
			set {
				if(Images == value)
					return;
				images = value;
				OnPropertiesChanged();
			}
		}
		internal protected const int itemSizeDefault = 120;
		int itemSize = itemSizeDefault;
		[DefaultValue(itemSizeDefault), Category(CategoryName.Appearance)]
		public int ItemSize {
			get { return itemSize; }
			set {
				if(ItemSize == value)
					return;
				itemSize = value;
				OnPropertiesChanged();
			}
		}
		int indentBetweenItems;
		[DefaultValue(8), Category(CategoryName.Appearance)]
		public int IndentBetweenItems {
			get { return indentBetweenItems; }
			set {
				if(IndentBetweenItems == value)
					return;
				indentBetweenItems = value;
				OnPropertiesChanged();
			}
		}
		int indentBetweenGroups;
		[DefaultValue(56), Category(CategoryName.Appearance)]
		public int IndentBetweenGroups {
			get { return indentBetweenGroups; }
			set {
				if(IndentBetweenGroups == value)
					return;
				indentBetweenGroups = value;
				OnPropertiesChanged();
			}
		}
		public static Padding DefaultItemPadding { get { return new Padding(12, 8, 12, 8); } }
		void ResetItemPadding() { ItemPadding = DefaultItemPadding; }
		bool ShouldSerializeItemPadding() { return ItemPadding != DefaultItemPadding; }
		Padding itemPadding;
		[Category(CategoryName.Appearance)]
		public Padding ItemPadding {
			get { return itemPadding; }
			set {
				if(ItemPadding == value)
					return;
				itemPadding = value;
				OnPropertiesChanged();
			}
		}
		int position;
		[DefaultValue(0), Category(CategoryName.Properties)]
		public int Position {
			get { return position; }
			set {
				value = ViewInfoCore.ConstraintOffset(value);
				if(Position == value)
					return;
				position = value;
				ViewInfoCore.Offset = position;
				if(((ITileControl)this).ScrollBar != null && ScrollMode == TileControlScrollMode.ScrollBar)
					((ITileControl)this).ScrollBar.Value = value;
				RaiseSmartTagUpdate();
			}
		}
		int rowCount;
		[DefaultValue(5), Category(CategoryName.Properties)]
		public int RowCount {
			get { return rowCount; }
			set {
				if(RowCount == value)
					return;
				rowCount = value;
				OnPropertiesChanged();
			}
		}
		int columnCount;
		[DefaultValue(0), Category(CategoryName.Properties)]
		public int ColumnCount {
			get { return columnCount; }
			set {
				if(ColumnCount == value)
					return;
				columnCount = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(true), Category(CategoryName.Behavior)]
		public bool AllowDisabledStateIndication {
			get;
			set;
		}
		[Category(CategoryName.Behavior)]
		public event TileItemClickEventHandler SelectedItemChanged {
			add { Events.AddHandler(selectedItemChanged, value); }
			remove { Events.RemoveHandler(selectedItemChanged, value); }
		}
		[Category(CategoryName.Behavior)]
		public event TileItemClickEventHandler ItemClick {
			add { Events.AddHandler(itemClick, value); }
			remove { Events.RemoveHandler(itemClick, value); }
		}
		[Category(CategoryName.Behavior)]
		public event TileItemClickEventHandler ItemDoubleClick {
			add { Events.AddHandler(itemDoubleClick, value); }
			remove { Events.RemoveHandler(itemDoubleClick, value); }
		}
		[Category(CategoryName.Behavior)]
		public event TileItemClickEventHandler RightItemClick {
			add { Events.AddHandler(rightItemClick, value); }
			remove { Events.RemoveHandler(rightItemClick, value); }
		}
		[Category(CategoryName.Behavior)]
		public event TileItemClickEventHandler ItemPress {
			add { Events.AddHandler(itemPress, value); }
			remove { Events.RemoveHandler(itemPress, value); }
		}
		protected void RaiseSelectedItemChanged(TileItem item) {
			TileItemEventArgs e = new TileItemEventArgs() { Item = item };
			TileItemClickEventHandler handler = Events[selectedItemChanged] as TileItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected void RaiseItemClick(TileItem item) {
			TileItemEventArgs e = new TileItemEventArgs() { Item = item };
			TileItemClickEventHandler handler = Events[itemClick] as TileItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected void RaiseItemDoubleClick(TileItem item) {
			TileItemEventArgs e = new TileItemEventArgs() { Item = item };
			TileItemClickEventHandler handler = Events[itemDoubleClick] as TileItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected void RaiseRightItemClick(TileItem item) {
			TileItemEventArgs e = new TileItemEventArgs() { Item = item };
			TileItemClickEventHandler handler = Events[rightItemClick] as TileItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected void RaiseItemPress(TileItem item) {
			TileItemEventArgs e = new TileItemEventArgs() { Item = item };
			TileItemClickEventHandler handler = Events[itemPress] as TileItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected void RaiseItemCheckedChanged(TileItem item) {
			TileItemEventArgs e = new TileItemEventArgs() { Item = item };
			TileItemClickEventHandler handler = Events[itemCheckedChanged] as TileItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		void ITileControl.OnItemCheckedChanged(TileItem item) {
			OnItemCheckedChangedCore(item);
		}
		protected virtual void OnItemCheckedChangedCore(TileItem item) {
			RaiseItemCheckedChanged(item);
		}
		void ITileControl.OnItemClick(TileItem item) {
			OnItemClickCore(item);
		}
		void ITileControl.OnItemDoubleClick(TileItem item) {
			OnItemDoubleClickCore(item);
		}
		void ITileControl.OnRightItemClick(TileItem item) {
			OnRightItemClickCore(item);
		}
		protected virtual void OnItemClickCore(TileItem item) {
			RaiseItemClick(item);
		}
		protected virtual void OnItemDoubleClickCore(TileItem item) {
			RaiseItemDoubleClick(item);
		}
		protected virtual void OnRightItemClickCore(TileItem item) {
			RaiseRightItemClick(item);
		}
		void ITileControl.OnItemPress(TileItem item) {
			OnItemPressCore(item);
		}
		protected virtual void OnItemPressCore(TileItem item) {
			RaiseItemPress(item);
		}
		protected override void OnPaintBackground(PaintEventArgs pevent) {
			if(!ShouldDrawBackground)
				return;
			base.OnPaintBackground(pevent);
		}
		protected virtual bool ShouldDrawBackground {
			get {
				if(BackgroundImage == null)
					return true;
				return BackgroundImageLayout == ImageLayout.Center || BackgroundImageLayout == ImageLayout.None;
			}
		}
		protected bool IsFirstPaintingComplete { get; set; }
		protected override void OnPaint(PaintEventArgs e) {
			DevExpress.Utils.Mdi.ControlState.CheckPaintError(this);
			CheckParentColors();
			CheckViewInfo();
			if(!IsFirstPaintingComplete)
				StartItemContentAnimation();
			if(((ITileControl)this).AnimateArrival && !DesignMode) {
				CheckViewInfo();
				((ITileControl)this).AnimateArrival = false;
				((ITileControl)this).ViewInfo.AnimateAppearance();
				Invalidate();
				return;
			}
			using(GraphicsCache cache = new GraphicsCache(e)) {
				Painter.Draw(new TileControlInfoArgs(cache, ViewInfoCore));
			}
			RaisePaintEvent(this, e);
			IsFirstPaintingComplete = true;
		}
		TileControlPainter painterCore;
		protected internal TileControlPainter Painter {
			get {
				if(painterCore == null) painterCore = CreatePainter();
				return painterCore;
			}
		}
		protected virtual TileControlPainter CreatePainter() {
			return new TileControlPainter();
		}
		protected virtual void CheckParentColors() {
			ViewInfoCore.CheckParentColors();
		}
		protected virtual void StartItemContentAnimation() {
			foreach(TileGroup group in Groups) {
				foreach(TileItem item in group.Items) {
					if(item.Frames != null && item.Frames.Count > 0 && item.Enabled)
						item.StartContentAnimation();
				}
			}
		}
		protected virtual void CheckViewInfo() {
			if(!ViewInfoCore.IsReady)
				ViewInfoCore.CalcViewInfo(ClientRectangle);
		}
		protected override void OnLocationChanged(EventArgs e) {
			base.OnLocationChanged(e);
			ViewInfoCore.UpdateVisualEffects(UpdateAction.Update);
		}
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			OnPropertiesChanged();
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			ViewInfoCore.ResetBackgroundImage();
			OnPropertiesChanged();
		}
		TileControlHandler handler;
		[Browsable(false)]
		public TileControlHandler Handler {
			get {
				if(handler == null)
					handler = CreateHandler();
				return handler;
			}
		}
		ScrollBarBase scrollbar;
		ScrollBarBase ITileControl.ScrollBar {
			get { return scrollbar; }
			set {
				if(scrollbar == value)
					return;
				ScrollBarBase prev = scrollbar;
				scrollbar = value;
				OnScrollBarChanged(prev);
			}
		}
		protected virtual void OnScrollBarChanged(ScrollBarBase prev) {
			if(prev != null) {
				Controls.Remove(prev);
				prev.Dispose();
			}
			if(((ITileControl)this).ScrollBar != null)
				((ITileControl)this).ScrollBar.ValueChanged += new EventHandler(Handler.OnScrollBarValueChanged);
		}
		protected virtual TileControlHandler CreateHandler() {
			return new TileControlHandler(this);
		}
		MouseWheelScrollHelper wheelScrollHelper;
		MouseWheelScrollHelper WheelScrollHelper {
			get {
				if(wheelScrollHelper == null)
					wheelScrollHelper = new MouseWheelScrollHelper(this);
				return wheelScrollHelper;
			}
		}
		protected sealed override void OnMouseWheel(MouseEventArgs ev) {
			if(XtraForm.ProcessSmartMouseWheel(this, ev)) return;
			OnMouseWheel2(ev);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheel2(e);
		}
		void OnMouseWheel2(MouseEventArgs e) {
			WheelScrollHelper.OnMouseWheel(e);
		}
		void IMouseWheelScrollClient.OnMouseWheel(MouseWheelScrollClientArgs e) {
			OnMouseWheelCore(e);
		}
		bool IMouseWheelScrollClient.PixelModeHorz { get { return true; } }
		bool IMouseWheelScrollClient.PixelModeVert { get { return true; } }
		protected virtual void OnMouseWheelCore(MouseWheelScrollClientArgs e) {
			CheckViewInfo();
			Handler.OnMouseWheel(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			CheckViewInfo();
			Handler.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			CheckViewInfo();
			Handler.OnMouseUp(e);
		}
		const int CURSOR_SHOWING = 0x00000001;
		EditorsNativeMethods.CURSORINFO info = new EditorsNativeMethods.CURSORINFO();
		bool CheckCursorIsVisibile() {
			info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
			if(EditorsNativeMethods.GetCursorInfo(out info)) {
				if(info.flags != CURSOR_SHOWING)
					return false;
			}
			return true;
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(((ITileControl)this).ScrollBar != null) (((ITileControl)this).ScrollBar).OnAction(ScrollNotifyAction.MouseMove);
			CheckViewInfo();
			if(!CheckCursorIsVisibile()) {
				ViewInfoCore.HoverInfo = ViewInfoCore.CalcHitInfo(TileControlHandler.InvalidHitPoint);
				return;
			}
			Handler.OnMouseMove(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			CheckViewInfo();
			Handler.OnMouseLeave(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			CheckViewInfo();
			Handler.OnMouseEnter(e);
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			CheckViewInfo();
			if(Handler.ProcessCmdKey(keyData))
				return true;
			return base.ProcessCmdKey(ref msg, keyData);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			CheckViewInfo();
			Handler.OnKeyDown(e);
		}
		SizeF scaleFactor = new SizeF(1f, 1f);
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SizeF ScaleFactor {
			get { return UseParentAutoScaleFactor ? scaleFactor : new SizeF(1f, 1f); }
		}
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
			scaleFactor = factor;
			base.ScaleControl(factor, specified);
		}
		[DefaultValue(true), Browsable(false)]
		public bool UseParentAutoScaleFactor { get; set; }
		public event TileItemClickEventHandler ItemCheckedChanged {
			add { Events.AddHandler(itemCheckedChanged, value); }
			remove { Events.RemoveHandler(itemCheckedChanged, value); }
		}
		ISupportXtraAnimation ITileControl.AnimationHost { get { return this; } }
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate {
			get {
				if(!IsFirstPaintingComplete) return true;
				return Enabled;
			}
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return this; }
		}
		#endregion
		protected override void WndProc(ref Message m) {
			if(Handler.GestureHelper.WndProc(ref m))
				return;
			base.WndProc(ref m);
		}
		#region ISupportLookAndFeel Members
		bool ISupportLookAndFeel.IgnoreChildren {
			get { return true; }
		}
		UserLookAndFeel ISupportLookAndFeel.LookAndFeel {
			get { return LookAndFeel; }
		}
		#endregion
		#region IAppearanceOwner Members
		bool IAppearanceOwner.IsLoading {
			get { return false; }
		}
		#endregion
		public virtual void SaveLayoutToXml(string xmlFile) {
			SaveLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile) {
			RestoreLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual bool SaveLayoutToRegistry(string path) {
			return SaveLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void RestoreLayoutFromRegistry(string path) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void SaveLayoutToStream(System.IO.Stream stream) {
			SaveLayoutCore(new XmlXtraSerializer(), stream);
		}
		public virtual void RestoreLayoutFromStream(System.IO.Stream stream) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream);
		}
		protected virtual bool SaveLayoutCore(XtraSerializer serializer, object path) {
			System.IO.Stream stream = path as System.IO.Stream;
			if(stream != null)
				return serializer.SerializeObjects(
					new XtraObjectInfo[] { new XtraObjectInfo("TileControl", this) },
											 stream, this.GetType().Name);
			else
				return serializer.SerializeObjects(
					new XtraObjectInfo[] { new XtraObjectInfo("TileControl", this) },
											path.ToString(), this.GetType().Name);
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path) {
			System.IO.Stream stream = path as System.IO.Stream;
			BeginUpdate();
			try {
				if(stream != null)
					serializer.DeserializeObjects(new XtraObjectInfo[] { new XtraObjectInfo("TileControl", this) },
						stream, this.GetType().Name);
				else
					serializer.DeserializeObjects(new XtraObjectInfo[] { new XtraObjectInfo("TileControl", this) },
						path.ToString(), this.GetType().Name);
			}
			finally {
				RestoreFrames();
				EndUpdate();
			}
		}
		void RestoreFrames() {
			foreach(TileGroup group in Groups) {
				foreach(TileItem item in group.Items) {
					if(item.Frames.Count > 0 && item.Frames.Count > item.CurrentFrameIndex)
						item.SetContent(item.Frames[item.CurrentFrameIndex], false);
				}
			}
		}
		public Size GetItemSize(TileItemSize itemSize) {
			TileItem newItem = new TileItem() { ItemSize = itemSize };
			return ViewInfoCore.GetItemSize(new TileItemViewInfo(newItem));
		}
		public Size GetItemSize(TileItem item) {
			if(item == null) return Size.Empty;
			return ViewInfoCore.GetItemSize(new TileItemViewInfo(item));
		}
		#region IXtraSerializable Members
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			RemoveUnusedItems();
			DeserializedItems.Clear();
			DeserializedItems = null;
			OriginItems.Clear();
			OriginItems = null;
			OriginGroups.Clear();
			OriginGroups = null;
			foreach(TileGroup group in Groups) {
				if(group.OriginItems == null) continue;
				group.OriginItems.Clear();
				group.OriginItems = null;
			}
			RemoveEmptyGroups();
			((ITileControl)this).AnimateArrival = true;
		}
		void RemoveUnusedItems() {
			if(DeserializedItems == null ||
				DeserializedItems.Count <= 0 ||
				OriginItems == null ||
				OriginItems.Count <= 0) return;
			foreach(TileItem originItem in OriginItems) {
				if(DeserializedItems.Contains(originItem)) continue;
				if(originItem.Group != null)
					originItem.Group.Items.Remove(originItem);
			}
		}
		void RemoveEmptyGroups() {
			List<TileGroup> emptyGroups = new List<TileGroup>();
			foreach(TileGroup group in Groups) {
				if(group.Items.Count == 0)
					emptyGroups.Add(group);
			}
			foreach(TileGroup emptyGroup in emptyGroups) {
				Groups.Remove(emptyGroup);
				emptyGroup.Dispose();
			}
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		List<TileGroup> OriginGroups { get; set; }
		List<TileItem> OriginItems { get; set; }
		protected internal List<TileItem> DeserializedItems { get; set; }
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			OriginItems = new List<TileItem>();
			DeserializedItems = new List<TileItem>();
			OriginGroups = new List<TileGroup>();
			foreach(TileGroup group in Groups) {
				OriginGroups.Add(group);
				foreach(TileItem item in group.Items) {
					OriginItems.Add(item);
				}
			}
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		internal TileGroup XtraCreateGroupsItem(XtraItemEventArgs e) {
			TileGroup res = new TileGroup();
			res.OriginItems = OriginItems;
			Groups.Add(res);
			return res;
		}
		internal TileGroup XtraFindGroupsItem(XtraItemEventArgs e) {
			string name = e.Item.ChildProperties["Name"].Value as String;
			foreach(TileGroup group in OriginGroups) {
				if(group.Name.Equals(name)) {
					group.OriginItems = OriginItems;
					Groups.Add(group);
					return group;
				}
			}
			return XtraCreateGroupsItem(e);
		}
		#endregion
		public TileControlHitInfo CalcHitInfo(Point pt) {
			return ViewInfoCore.CalcHitInfo(pt);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				RemoveTooltipControllers();
				FormUnsubscribe(ParentForm);
				if(this.viewInfo != null)
					this.viewInfo.StopItemContentAnimations();
				List<TileGroup> groupsCore = new List<TileGroup>();
				foreach(TileGroup group in Groups) {
					groupsCore.Add(group);
				}
				foreach(TileGroup group in groupsCore) {
					group.Dispose();
				}
				OriginGroups = null;
				if(DeserializedItems != null)
					DeserializedItems.Clear();
				DeserializedItems = null;
			}
			base.Dispose(disposing);
		}
		void RemoveTooltipControllers() {
			ToolTipController.DefaultController.RemoveClientControl(this);
			ToolTipController = null;
		}
		public TileGroup GetTileGroupByText(string str) {
			foreach(TileGroup group in Groups)
				if(group.Text == str) return group;
			return null;
		}
		public TileGroup GetTileGroupByName(string str) {
			foreach(TileGroup group in Groups)
				if(group.Name == str) return group;
			return null;
		}
		public List<TileItem> GetCheckedItems() {
			List<TileItem> checkedItems = new List<TileItem>();
			foreach(TileGroup group in Groups)
				foreach(TileItem item in group.Items)
					if(item.Checked)
						checkedItems.Add(item);
			return checkedItems;
		}
		#region ITileControlProperties Members
		GroupHighlightingProperties ITileControlProperties.AppearanceGroupHighlighting {
			get { return appearanceGroupHighlighting; }
			set { appearanceGroupHighlighting = value; }
		}
		#endregion
		#region ITileControlUpdateSmartTag Members
		event SmartTagUpdateEventHandler smartTagUpdate;
		event SmartTagUpdateEventHandler ITileControlUpdateSmartTag.SmartTagUpdate {
			add { smartTagUpdate += value; }
			remove { smartTagUpdate -= value; }
		}
		void ITileControl.UpdateSmartTag() {
			RaiseSmartTagUpdate();
		}
		void RaiseSmartTagUpdate() {
			if(smartTagUpdate == null) return;
			smartTagUpdate(this, new TileControlSmartTagEventArgs() { Info = this.viewInfo, TileControl = this });
		}
		#endregion
		#region ToolTips_implementation
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return GetToolTipInfo(point);
		}
		protected virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			if(CanShowToolTip) {
				object obj = this;
				SuperToolTip superTip = null;
				ToolTipControlInfo res = new ToolTipControlInfo();
				var hitInfo = ViewInfoCore.CalcHitInfo(point);
				if(hitInfo.InItem && hitInfo.ItemInfo != null && hitInfo.ItemInfo.Item != null) {
					var contextToolTipInfo = hitInfo.ItemInfo.ContextButtonsViewInfo.GetToolTipInfo(point);
					if(contextToolTipInfo != null)
						return contextToolTipInfo;
					obj = hitInfo.ItemInfo.Item;
					superTip = hitInfo.ItemInfo.Item.SuperTip;
				}
				res.Object = obj;
				res.SuperTip = superTip;
				return res;
			}
			return null;
		}
		protected virtual bool CanShowToolTip {
			get {
				return !(this as ITileControl).IsDesignMode && ShowToolTips &&
					Handler.State == TileControlHandlerState.Normal;
			}
		}
		#endregion
		ContextAnimationType IContextItemCollectionOptionsOwner.AnimationType {
			get { return ContextAnimationType.OpacityAnimation; }
		}
		public void OnOptionsChanged(string propertyName, object oldValue, object newValue) {
			OnPropertiesChanged();
		}
		bool IContextItemCollectionOwner.IsDesignMode {
			get { return DesignMode; }
		}
		public void OnCollectionChanged() {
			OnPropertiesChanged();
		}
		public void OnItemChanged(ContextItem item, string propertyName, object oldValue, object newValue) {
			if(propertyName == "Visibility") {
				Invalidate();
				Update();
				return;
			}
			OnPropertiesChanged();
		}
		ContextItemCollection contextButtons;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ContextItemCollection ContextButtons {
			get {
				if(contextButtons == null) {
					contextButtons = CreateContextButtons();
					contextButtons.Options = ContextButtonOptions;
				}
				return contextButtons;
			}
		}
		protected virtual ContextItemCollection CreateContextButtons() {
			return new ContextItemCollection(this);
		}
		ContextItemCollectionOptions contextButtonOptions;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public ContextItemCollectionOptions ContextButtonOptions {
			get {
				if(contextButtonOptions == null)
					contextButtonOptions = CreateContextButtonOptions();
				return contextButtonOptions;
			}
		}
		protected virtual ContextItemCollectionOptions CreateContextButtonOptions() {
			return new ContextItemCollectionOptions(this);
		}
		public event TileContextButtonCustomizeEventHandler ContextButtonCustomize {
			add { Events.AddHandler(contextButtonCustomize, value); }
			remove { Events.RemoveHandler(contextButtonCustomize, value); }
		}
		public event ContextItemClickEventHandler ContextButtonClick {
			add { Events.AddHandler(contextButtonClick, value); }
			remove { Events.RemoveHandler(contextButtonClick, value); }
		}
		public event TileContextButtonToolTipEventHandler CustomContextButtonToolTip {
			add { Events.AddHandler(customContextButtonToolTip, value); }
			remove { Events.RemoveHandler(customContextButtonToolTip, value); }
		}
		void ITileControl.RaiseContextItemClick(ContextItemClickEventArgs e) {
			ContextItemClickEventHandler handler = Events[contextButtonClick] as ContextItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		void ITileControl.RaiseContextButtonCustomize(ITileItem tileItem, ContextItem contextItem) {
			TileContextButtonCustomizeEventHandler handler = Events[contextButtonCustomize] as TileContextButtonCustomizeEventHandler;
			if(handler != null)
				handler(this, new TileContextButtonCustomizeEventArgs() { Item = contextItem, TileItem = tileItem });
		}
		void ITileControl.RaiseCustomContextButtonToolTip(ITileItem tileItem, ContextButtonToolTipEventArgs e) {
			TileContextButtonToolTipEventHandler handler = Events[customContextButtonToolTip] as TileContextButtonToolTipEventHandler;
			if(handler != null) {
				handler(this, new TileContextButtonToolTipEventArgs(tileItem, e));
			}
		}
		bool isRightToLeft = false;
		[Browsable(false)]
		public bool IsRightToLeft { get { return isRightToLeft; } }
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			bool newRightToLeft = WindowsFormsSettings.GetIsRightToLeft(this);
			if(newRightToLeft == this.isRightToLeft) return;
			this.isRightToLeft = newRightToLeft;
			OnPropertiesChanged();
		}
	}
	public delegate void TileContextButtonCustomizeEventHandler(object sender, TileContextButtonCustomizeEventArgs e);
	public delegate void TileContextButtonToolTipEventHandler(object sender, TileContextButtonToolTipEventArgs e);
	public class TileContextButtonCustomizeEventArgs : EventArgs {
		public ITileItem TileItem { get; internal set; }
		public ContextItem Item { get; internal set; }
	}
	public class TileContextButtonToolTipEventArgs : EventArgs {
		ContextButtonToolTipEventArgs contextToolTipArgs;
		public TileContextButtonToolTipEventArgs(ITileItem tileItem, ContextButtonToolTipEventArgs toolTipArgs) {
			this.TileItem = tileItem;
			this.contextToolTipArgs = toolTipArgs;
		}
		public ITileItem TileItem { get; internal set; }
		public ContextItem Item { get { return contextToolTipArgs.Item; } }
		public object Value { get { return contextToolTipArgs.Value; } }
		public string Text {
			get { return contextToolTipArgs.Text; }
			set { contextToolTipArgs.Text = value; }
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GroupHighlightingProperties : ICloneable, IXtraSerializable, IXtraSerializableLayoutEx {
		public const int DefaultMaskOpacity = 50;
		public const int DefaultHoveredMaskOpacity = 80;
		public static Color DefaultMaskColor = Color.White;
		public static Color DefaultHoveredMaskColor = Color.White;
		int maskOpacity;
		int hoveredMaskOpacity;
		public GroupHighlightingProperties() {
			this.MaskOpacity = DefaultMaskOpacity;
			this.MaskColor = DefaultMaskColor;
			this.HoveredMaskOpacity = DefaultHoveredMaskOpacity;
			this.HoveredMaskColor = DefaultHoveredMaskColor;
		}
		[DefaultValue(typeof(Color), "White"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public Color MaskColor { get; set; }
		[DefaultValue(DefaultMaskOpacity), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int MaskOpacity {
			get { return maskOpacity; }
			set {
				if(value >= 0 && value <= 255)
					maskOpacity = value;
				else throw new Exception("Incorrect value (0 - 255)");
			}
		}
		[DefaultValue(typeof(Color), "White"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public Color HoveredMaskColor { get; set; }
		[DefaultValue(DefaultHoveredMaskOpacity), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int HoveredMaskOpacity {
			get { return hoveredMaskOpacity; }
			set {
				if(value >= 0 && value <= 255)
					hoveredMaskOpacity = value;
				else throw new Exception("Incorrect value (0 - 255)");
			}
		}
		[Obsolete("Use StandardColorGroup"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Color StandartColorGroup {
			get { return StandardColorGroup; }
		}
		[Browsable(false)]
		public Color StandardColorGroup {
			get { return Color.FromArgb(MaskOpacity, MaskColor); }
		}
		[Browsable(false)]
		public Color HoverColorGroup {
			get { return Color.FromArgb(HoveredMaskOpacity, HoveredMaskColor); }
		}
		public override string ToString() { return "AppearanceGroupHighlighting"; }
		#region ICloneable Members
		public object Clone() {
			return CloneCore();
		}
		#endregion
		#region Infrastructure
		protected virtual GroupHighlightingProperties CloneCore() {
			GroupHighlightingProperties clone = new GroupHighlightingProperties();
			clone.Assign(this);
			return clone;
		}
		public void Assign(GroupHighlightingProperties properties) {
			if(properties == null) return;
			this.maskOpacity = properties.maskOpacity;
			this.MaskColor = properties.MaskColor;
			this.hoveredMaskOpacity = properties.hoveredMaskOpacity;
			this.HoveredMaskColor = properties.HoveredMaskColor;
		}
		public void Combine(GroupHighlightingProperties properties) {
			if(properties == null) return;
			if(this.maskOpacity == DefaultMaskOpacity)
				this.maskOpacity = properties.maskOpacity;
			if(this.MaskColor == DefaultMaskColor)
				this.MaskColor = properties.MaskColor;
			if(this.hoveredMaskOpacity == DefaultHoveredMaskOpacity)
				this.hoveredMaskOpacity = properties.hoveredMaskOpacity;
			if(this.HoveredMaskColor == DefaultHoveredMaskColor)
				this.HoveredMaskColor = properties.HoveredMaskColor;
		}
		public bool ShouldSerialize() {
			return
				(maskOpacity != DefaultMaskOpacity) ||
				(MaskColor != DefaultMaskColor) ||
				(hoveredMaskOpacity != DefaultHoveredMaskOpacity) ||
				(HoveredMaskColor != DefaultHoveredMaskColor);
		}
		public void Reset() {
			this.maskOpacity = DefaultMaskOpacity;
			this.MaskColor = DefaultMaskColor;
			this.hoveredMaskOpacity = DefaultHoveredMaskOpacity;
			this.HoveredMaskColor = DefaultHoveredMaskColor;
		}
		#endregion Infrastructure
		#region IXtraSerializable Members
		int xtraSerializableState;
		bool IsSerializing {
			get { return xtraSerializableState > 0; }
		}
		bool IsDeserializing {
			get { return xtraSerializableState < 0; }
		}
		void IXtraSerializable.OnStartSerializing() {
			xtraSerializableState++;
		}
		void IXtraSerializable.OnEndSerializing() {
			xtraSerializableState--;
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			xtraSerializableState--;
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			xtraSerializableState++;
		}
		#endregion
		#region IXtraSerializableLayoutEx Members
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			if(IsSerializing) {
				switch(propertyName) {
					case "MaskOpacity":
						return maskOpacity != DefaultMaskOpacity;
					case "MaskColor":
						return MaskColor != DefaultMaskColor;
					case "HoveredMaskOpacity":
						return hoveredMaskOpacity != DefaultHoveredMaskOpacity;
					case "HoveredMaskColor":
						return HoveredMaskColor != DefaultHoveredMaskColor;
					default:
						return false;
				}
			}
			return true;
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			if(IsDeserializing) Reset();
		}
		#endregion
	}
	public class TileControlDesignTimeManagerBase : BaseDesignTimeManager, ITileControlDesignManager {
		public BaseDesignTimeManager GetBase() { return this; }
		public object GetGroup() { return this.TileGroup; }
		public object GetItem() { return this.TileItem; }
		public TileControlDesignTimeManagerBase(IComponent component, ITileControl tileControl)
			: base(component, tileControl.Site) {
			TileControl = tileControl;
			Component = component;
			ComponentChangeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
		}
		public void ComponentChanged(IComponent comp) {
			if(ComponentChangeService != null && comp != null)
				ComponentChangeService.OnComponentChanged(comp, null, null, null);
		}
		void OnComponentRemoved(object sender, ComponentEventArgs e) {
			TileControl tileControl = e.Component as TileControl;
			if(tileControl != null) {
				while(tileControl.Groups.Count > 0)
					RemoveGroup(tileControl.Groups[0]);
			}
			else if(e.Component is TileItem)
				RemoveItem((TileItem)e.Component, false);
			else if(e.Component is TileGroup)
				RemoveGroup((TileGroup)e.Component, false);
		}
		public ITileControl TileControl { get; private set; }
		public IComponent Component { get; private set; }
		public TileGroup TileGroup {
			get {
				if(Component is TileGroup)
					return (TileGroup)Component;
				if(TileItem != null)
					return TileItem.Group;
				ICollection coll = SelectionService.GetSelectedComponents();
				foreach(IComponent comp in coll) {
					if(comp is TileGroup)
						return (TileGroup)comp;
				}
				return null;
			}
		}
		public TileItem TileItem {
			get {
				if(Component is TileItem)
					return (TileItem)Component;
				ICollection coll = SelectionService.GetSelectedComponents();
				foreach(IComponent comp in coll) {
					if(IsWinUIBaseTile(comp)) {
						return GetTileItemByBaseTile(comp);
					}
					if(comp is TileItem)
						return (TileItem)comp;
				}
				return null;
			}
		}
		protected virtual bool IsWinUIBaseTile(IComponent component) {
			if(component == null) return false;
			bool r1 = component.GetType().Name.Equals("BaseTile");
			bool r2 = component.GetType().BaseType.Name.Equals("BaseTile");
			return r2 || r1;
		}
		protected virtual TileItem GetTileItemByBaseTile(object baseTile) {
			if(TileControl is ITileItemProvider)
				return (TileControl as ITileItemProvider).GetTileItem(baseTile);
			return null;
		}
		INameCreationService nameService;
		bool isSelectedCore;
		bool IsSelected { get { return isSelectedCore; } }
		void OnSelectChange(bool value) {
			if(IsSelected && !value)
				TileControl.Invalidate(TileControl.ClientRectangle);
			isSelectedCore = value;
		}
		public INameCreationService NameService {
			get {
				if(nameService == null)
					nameService = TileControl.Site.GetService(typeof(INameCreationService)) as INameCreationService;
				return nameService;
			}
		}
		IComponentChangeService componentChangeService;
		public IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null)
					componentChangeService = TileControl.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				return componentChangeService;
			}
		}
		public virtual void OnAddTileGroupClick() {
			if(TileControl.DebuggingState)
				return;
			TileGroup group = new TileGroup();
			TileControl.Groups.Add(group);
			if(TileControl.Container != null)
				TileControl.Container.Add(group);
			group.Name = NameService.CreateName(TileControl.Container, typeof(TileGroup));
			ComponentChangeService.OnComponentChanged(TileControl, null, null, null);
		}
		public void OnRemoveTileGroupClick(object group) {
			if(group is TileGroup) OnRemoveTileGroupClick(group as TileGroup);
		}
		public virtual void OnRemoveTileGroupClick(TileGroup group) {
			RemoveGroup(group);
		}
		public virtual void OnAddTileItemClick() {
			OnAddTileItemClick(false);
		}
		public virtual void OnAddLargeTileItemClick() {
			OnAddTileItemClick(true);
		}
		public void OnAddTileItemClick(object islarge) {
			if(islarge is Boolean) OnAddTileItemClick((bool)islarge);
			else
				if(islarge is TileItemSize) OnAddTileItemClick((TileItemSize)islarge);
		}
		public virtual void OnAddTileItemClick(bool isLarge) {
			OnAddTileItemClick(isLarge ? TileItemSize.Wide : TileItemSize.Medium);
		}
		public virtual void OnAddTileItemClick(TileItemSize itemType) {
			if(TileGroup == null || TileControl.DebuggingState)
				return;
			TileItem item = CreateTileItem(itemType);
			item.Id = ((TileControl)TileGroup.Control).GetNextId();
			item.Name = NameService.CreateName(TileControl.Container, typeof(TileItem));
			item.Text = item.Name;
			TileGroup.Items.Add(item);
			if(TileGroup.Container != null)
				TileGroup.Container.Add(item);
			ComponentChangeService.OnComponentChanged(TileGroup, null, null, null);
		}
		public static TileItem CreateTileItem(TileItemSize itemSize) {
			TileItem item = new TileItem();
			item.ItemSize = itemSize;
			return item;
		}
		protected override void OnDesignTimeSelectionChanged(object sender, EventArgs e) {
			base.OnDesignTimeSelectionChanged(sender, e);
			OnSelectChange(SelectionService.GetComponentSelected(TileControl) || TileItem != null || TileGroup != null);
		}
		public virtual void RemoveGroup(TileGroup group) { RemoveGroup(group, true); }
		public virtual void RemoveGroup(TileGroup group, bool removeFromContainer) {
			if(TileControl.DebuggingState)
				return;
			while(group.Items.Count > 0) {
				RemoveItem(group.Items[0], true);
			}
			if(removeFromContainer && group.Control != null)
				group.Control.Container.Remove(group);
			if(group.Control != null)
				group.Control.Groups.Remove(group);
			ComponentChangeService.OnComponentChanged(TileControl, null, null, null);
		}
		public virtual void RemoveItem(TileItem item) { RemoveItem(item, true); }
		public virtual void RemoveItem(TileItem item, bool removeFromContainer) {
			if(TileControl.DebuggingState)
				return;
			TileGroup group = item.Group;
			if(removeFromContainer && item.Group != null && item.Group.Control != null)
				item.Group.Control.Container.Remove(item);
			if(item.Group != null)
				item.Group.Items.Remove(item);
			if(group != null)
				ComponentChangeService.OnComponentChanged(group, null, null, null);
			else
				ComponentChangeService.OnComponentChanged(TileControl, null, null, null);
		}
		public void OnRemoveTileItemClick(object item) {
			if(item is TileItem) OnRemoveTileItemClick(item as TileItem);
		}
		public virtual void OnRemoveTileItemClick(TileItem item) {
			if(item == null)
				return;
			RemoveItem(item);
		}
		protected virtual void OnAddTileGroupClick(object sender, EventArgs e) {
			OnAddTileGroupClick();
		}
		protected virtual void OnRemoveTileGroupClick(object sender, EventArgs e) {
			OnRemoveTileGroupClick(((TileGroupViewInfo)((DXMenuItem)sender).Tag).Group);
		}
		protected virtual void OnAddMediumTileItemClick(object sender, EventArgs e) {
			OnAddTileItemClick();
		}
		protected virtual void OnAddWideTileItemClick(object sender, EventArgs e) {
			OnAddLargeTileItemClick();
		}
		protected virtual void OnAddSmallTileItemClick(object sender, EventArgs e) {
			OnAddTileItemClick(TileItemSize.Small);
		}
		protected virtual void OnAddLargeTileItemClick(object sender, EventArgs e) {
			OnAddTileItemClick(TileItemSize.Large);
		}
		protected virtual void OnRemoveTileItemClick(object sender, EventArgs e) {
			OnRemoveTileItemClick(((TileItemViewInfo)((DXMenuItem)sender).Tag).Item);
		}
		public virtual void ShowGroupMenu(TileControlHitInfo hitInfo) {
			DXPopupMenu popupMenu = new DXPopupMenu();
			FillDesignTimePopupMenu(popupMenu, true, hitInfo);
			MenuManagerHelper.Standard.ShowPopupMenu(popupMenu, TileControl.Control, hitInfo.HitPoint);
		}
		public virtual void ShowItemMenu(TileControlHitInfo hitInfo) {
			DXPopupMenu popupMenu = new DXPopupMenu();
			FillDesignTimePopupMenu(popupMenu, false, hitInfo);
			MenuManagerHelper.Standard.ShowPopupMenu(popupMenu, TileControl.Control, hitInfo.HitPoint);
		}
		protected virtual void FillDesignTimePopupMenu(DXPopupMenu popupMenu, bool isGroupMenu, TileControlHitInfo hitInfo) {
			DXMenuItem addGroup = new DXMenuItem() { Caption = "Add Group", Tag = hitInfo.GroupInfo };
			addGroup.Click += new EventHandler(OnAddTileGroupClick);
			addGroup.Enabled = !TileControl.DebuggingState;
			DXMenuItem removeGroup = new DXMenuItem() { Caption = "Remove Group", Tag = hitInfo.GroupInfo };
			removeGroup.Click += new EventHandler(OnRemoveTileGroupClick);
			removeGroup.Enabled = !TileControl.DebuggingState;
			DXMenuItem addMediumItem = new DXMenuItem() { Caption = "Add Medium Item", Tag = hitInfo.ItemInfo };
			addMediumItem.Click += new EventHandler(OnAddMediumTileItemClick);
			addMediumItem.Enabled = !TileControl.DebuggingState;
			DXMenuItem addWideItem = new DXMenuItem() { Caption = "Add Wide Item", Tag = hitInfo.ItemInfo };
			addWideItem.Click += new EventHandler(OnAddWideTileItemClick);
			addWideItem.Enabled = !TileControl.DebuggingState;
			DXMenuItem addSmallItem = new DXMenuItem() { Caption = "Add Small Item", Tag = hitInfo.ItemInfo };
			addSmallItem.Click += new EventHandler(OnAddSmallTileItemClick);
			addSmallItem.Enabled = !TileControl.DebuggingState;
			DXMenuItem addLargeItem = new DXMenuItem() { Caption = "Add Large Item", Tag = hitInfo.ItemInfo };
			addLargeItem.Click += new EventHandler(OnAddLargeTileItemClick);
			addLargeItem.Enabled = !TileControl.DebuggingState;
			DXMenuItem removeItem = new DXMenuItem() { Caption = "Remove Item", Tag = hitInfo.ItemInfo };
			removeItem.Click += new EventHandler(OnRemoveTileItemClick);
			removeItem.Enabled = !TileControl.DebuggingState;
			popupMenu.Items.Add(addGroup);
			popupMenu.Items.Add(removeGroup);
			popupMenu.Items.Add(addMediumItem);
			popupMenu.Items.Add(addWideItem);
			popupMenu.Items.Add(addSmallItem);
			popupMenu.Items.Add(addLargeItem);
			if(!isGroupMenu) {
				popupMenu.Items.Add(removeItem);
			}
		}
		public void OnEditFramesCollectionClickCore(object item) {
			if(item is TileItem) OnEditFramesCollectionClickCore(item as TileItem);
		}
		public virtual void OnEditFramesCollectionClickCore(TileItem item) { }
		public void OnEditElementsCollectionClickCore(object item) {
			if(item is TileItem) OnEditElementsCollectionClickCore(item as TileItem);
		}
		public virtual void OnEditElementsCollectionClickCore(TileItem item) { }
		public void OnEditTileTemplateClickCore(object item) {
			if(item is TileItem) OnEditTileTemplateClickCore(item as TileItem);
		}
		public virtual void OnEditTileTemplateClickCore(TileItem item) { }
		public void DrawSelectionBounds(GraphicsCache cache, Rectangle bounds, Color color) {
			using(Pen pen = new Pen(Color.Red)) {
				pen.DashPattern = new float[] { 5.0f, 5.0f };
				cache.Graphics.DrawRectangle(pen, bounds);
			}
		}
		public virtual void FireChanged() {
			ComponentChangeService.OnComponentChanged(TileControl, null, null, null);
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TileItemAppearances : BaseOwnerAppearance {
		AppearanceObject appearance;
		AppearanceObject appearanceHovered;
		AppearanceObject appearanceSelected;
		AppearanceObject appearancePressed;
		public TileItemAppearances(AppearanceDefault defaultApp)
			: this((IAppearanceOwner)null) {
			Normal.Assign(defaultApp);
			Hovered.Assign(defaultApp);
			Selected.Assign(defaultApp);
			Pressed.Assign(defaultApp);
		}
		public TileItemAppearances(AppearanceObject obj)
			: this((IAppearanceOwner)null) {
			Normal.Assign(obj);
			Hovered.Assign(obj);
			Selected.Assign(obj);
			Pressed.Assign(obj);
		}
		public TileItemAppearances() : this((IAppearanceOwner)null) { }
		public TileItemAppearances(IAppearanceOwner owner)
			: base(owner) {
			this.appearance = CreateAppearance();
			this.appearanceHovered = CreateAppearance();
			this.appearanceSelected = CreateAppearance();
			this.appearancePressed = CreateAppearance();
		}
		protected override AppearanceObject CreateAppearance() {
			AppearanceObject res = CreateAppearanceCore();
			res.Changed += new EventHandler(OnApperanceChanged);
			return res;
		}
		public virtual void Assign(TileItemAppearances app) {
			Normal.Assign(app.Normal);
			Hovered.Assign(app.Hovered);
			Selected.Assign(app.Selected);
			Pressed.Assign(app.Pressed);
		}
		public virtual void Assign(AppearanceDefault app) {
			Normal.Assign(app);
			Hovered.Assign(app);
			Selected.Assign(app);
			Pressed.Assign(app);
		}
		public virtual void Assign(AppearanceObject app) {
			Normal.Assign(app);
			Hovered.Assign(app);
			Selected.Assign(app);
			Pressed.Assign(app);
		}
		protected virtual AppearanceObject CreateAppearanceCore() {
			return new AppearanceObject(this, true);
		}
		internal void ResetNormal() {
			Normal.Reset();
		}
		internal bool ShouldSerializeNormal() {
			return this.appearance != null && Normal.ShouldSerialize();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TileItemAppearancesNormal"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Normal {
			get { return appearance; }
		}
		internal void ResetHovered() {
			Hovered.Reset();
		}
		internal bool ShouldSerializeHovered() {
			return this.appearanceHovered != null && Hovered.ShouldSerialize();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TileItemAppearancesHovered"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Hovered {
			get { return appearanceHovered; }
		}
		internal void ResetSelected() {
			Selected.Reset();
		}
		internal bool ShouldSerializeSelected() {
			return this.appearanceSelected != null && Selected.ShouldSerialize();
		}
		[ Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Selected {
			get { return appearanceSelected; }
		}
		internal void ResetPressed() {
			Pressed.Reset();
		}
		internal bool ShouldSerializePressed() {
			return this.appearancePressed != null && Pressed.ShouldSerialize();
		}
		[ Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Pressed {
			get { return appearancePressed; }
		}
		protected override void OnResetCore() {
			Normal.Reset();
			Hovered.Reset();
			Selected.Reset();
			Pressed.Reset();
		}
		public override void Dispose() {
			base.Dispose();
			DestroyAppearance(Normal);
			DestroyAppearance(Hovered);
			DestroyAppearance(Selected);
			DestroyAppearance(Pressed);
		}
	}
	public class TileControlObjectDescriptor : ICustomTypeDescriptor {
		IComponent parentCore;
		object itemCore;
		public IComponent Parent { get { return parentCore; } }
		public object Item { get { return itemCore; } }
		public TileControlObjectDescriptor(object item, IComponent parent) {
			itemCore = item;
			parentCore = parent;
		}
		AttributeCollection ICustomTypeDescriptor.GetAttributes() { return TypeDescriptor.GetAttributes(Item, true); }
		string ICustomTypeDescriptor.GetClassName() { return TypeDescriptor.GetClassName(Item, true); }
		string ICustomTypeDescriptor.GetComponentName() { return ""; }
		TypeConverter ICustomTypeDescriptor.GetConverter() { return TypeDescriptor.GetConverter(Item, true); }
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(Item, true); }
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(Item, true); }
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(Item, editorBaseType, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return EventDescriptorCollection.Empty; }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(System.Attribute[] attributes) { return EventDescriptorCollection.Empty; }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() { return TypeDescriptor.GetProperties(Item, true); }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(System.Attribute[] attributes) { return TypeDescriptor.GetProperties(Item, attributes, true); }
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor p) { return Item; }
	}
	public interface ITileControlDesigner {
		bool DebuggingState { get; }
	}
	public interface ITileItemProvider {
		TileItem GetTileItem(object obj);
	}
	public class TileItemDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			TileItem item = (TileItem)component;
			TileControlViewInfo vi = item.Control.ViewInfo;
			foreach(TileGroupViewInfo groupViewInfo in vi.Groups) {
				if(object.ReferenceEquals(groupViewInfo.Group, item.Group)) {
					foreach(TileItemViewInfo itemViewInfo in groupViewInfo.Items) {
						if(object.ReferenceEquals(itemViewInfo.Item, item))
							return itemViewInfo.Bounds;
					}
				}
			}
			return Rectangle.Empty;
		}
		public Control GetOwnerControl(IComponent component) {
			TileItem item = (TileItem)component;
			return item.Control as TileControl;
		}
	}
	public class TileGroupDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			TileGroup group = (TileGroup)component;
			TileControlViewInfo vi = group.Control.ViewInfo;
			foreach(TileGroupViewInfo groupViewInfo in vi.Groups) {
				if(object.ReferenceEquals(groupViewInfo.Group, group)) {
					return groupViewInfo.DesignTimeBounds;
				}
			}
			return Rectangle.Empty;
		}
		public Control GetOwnerControl(IComponent component) {
			TileGroup group = (TileGroup)component;
			return group.Control as TileControl;
		}
	}
	public class TileGroupDesignTimeActionsProvider {
		public void AddGroup(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnAddTileGroupClick();
		}
		public void RemoveGroup(IComponent component) {
			TileGroup group = (TileGroup)component;
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnRemoveTileGroupClick(group);
		}
		public void AddItem(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnAddTileItemClick();
		}
		public void AddMediumItem(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnAddTileItemClick(TileItemSize.Medium);
		}
		public void AddSmallItem(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnAddTileItemClick(TileItemSize.Small);
		}
		public void AddWideItem(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnAddTileItemClick(TileItemSize.Wide);
		}
		public void AddLargeItem(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnAddTileItemClick(TileItemSize.Large);
		}
		protected ITileControlDesignManager GetDesignTimeManager(IComponent component) {
			TileGroup group = (TileGroup)component;
			return group.Control.ViewInfo.DesignTimeManager;
		}
	}
	public class TileItemDesignTimeActionsProvider {
		public void AddGroup(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnAddTileGroupClick();
		}
		public void RemoveGroup(IComponent component) {
			TileItem item = (TileItem)component;
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnRemoveTileGroupClick(item.Group);
		}
		public void AddItem(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnAddTileItemClick();
		}
		public void AddSmallItem(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnAddTileItemClick(TileItemSize.Small);
		}
		public void AddMediumItem(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnAddTileItemClick(TileItemSize.Medium);
		}
		public void AddWideItem(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnAddTileItemClick(TileItemSize.Wide);
		}
		public void AddLargeItem(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnAddTileItemClick(TileItemSize.Large);
		}
		public void RemoveItem(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnRemoveTileItemClick((TileItem)component);
		}
		public void EditElements(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnEditElementsCollectionClickCore((TileItem)component);
		}
		public void EditFrames(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnEditFramesCollectionClickCore((TileItem)component);
		}
		public void SelectTemplate(IComponent component) {
			ITileControlDesignManager designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnEditTileTemplateClickCore((TileItem)component);
		}
		protected ITileControlDesignManager GetDesignTimeManager(IComponent component) {
			TileItem item = (TileItem)component;
			return item.Control.ViewInfo.DesignTimeManager;
		}
	}
}
