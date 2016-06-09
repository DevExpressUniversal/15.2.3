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

using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
namespace DevExpress.XtraBars {
	[
	DesignTimeVisible(false), ToolboxItem(false),
	SmartTagSupport(typeof(TabFormPageDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.UseComponentDesigner)
	]
	public class TabFormPage : Component, IDXImageUriClient {
		XtraScrollableControl contentContainer;
		string text, name;
		bool visible, enabled;
		Image image;
		DefaultBoolean allowGlyphSkinning, showCloseButton;
		DxImageUri imageUri;
		public TabFormPage() {
			this.text = string.Empty;
			this.visible = this.enabled = true;
			this.contentContainer = null;
			this.imageUri = CreateImageUriInstance();
			this.imageUri.Changed += ImageUriChanged;
			this.allowGlyphSkinning = this.showCloseButton = DefaultBoolean.Default;
		}
		[ Category(CategoryName.Appearance), DefaultValue(""), Localizable(true)]
		public string Text {
			get { return text; }
			set {
				if(Text == value)
					return;
				text = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(true)]
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value)
					return;
				visible = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(true)]
		public bool Enabled {
			get { return enabled; }
			set {
				if(Enabled == value)
					return;
				enabled = value;
				OnEnabledChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowCloseButton {
			get { return showCloseButton; }
			set {
				if(ShowCloseButton == value)
					return;
				showCloseButton = value;
				OnPropertiesChanged();
			}
		}
		public virtual void OnEnabledChanged() {
			if(ContentContainer != null)
				ContentContainer.Enabled = GetEnabled();
			OnPropertiesChanged();
		}
		protected internal bool ShouldShowCloseButton() {
			if(Owner != null && ShowCloseButton == DefaultBoolean.Default)
				return Owner.ShowTabCloseButtons;
			return ShowCloseButton == DefaultBoolean.True;
		}
		protected internal bool GetVisible() {
			if(Owner.IsDesignMode)
				return true;
			return Visible;
		}
		protected internal bool GetEnabled() {
			if(Owner == null || Owner.IsDesignMode)
				return true;
			return Enabled;
		}
		protected internal bool CanSelect() {
			return GetEnabled() && GetVisible();
		}
		protected internal int GetIndex() {
			return Owner.Pages.IndexOf(this);
		}
		[Browsable(false)]
		public string Name {
			get {
				if(Site != null) return Site.Name;
				return name;
			}
			set {
				if(value == null) return;
				name = value;
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Image {
			get { return image; }
			set {
				if(Image == value)
					return;
				image = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Data), DefaultValue(null),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag { get; set; }
		[ Category(CategoryName.Appearance), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				OnPropertiesChanged();
			}
		}
		protected internal virtual bool ShouldSerializeImageUri() {
			if(ImageUri == null)
				return false;
			return ImageUri.ShouldSerialize();
		}
		protected internal virtual void ResetImageUri() {
			ImageUri.Reset();
		}
		[ Category(CategoryName.Appearance), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual DxImageUri ImageUri {
			get { return imageUri; }
			set {
				if(value == null || ImageUri.Equals(value)) return;
				DxImageUri prev = ImageUri;
				this.imageUri = value;
				OnImageUriChanged(prev, value);
			}
		}
		private void OnImageUriChanged(DxImageUri prev, DxImageUri next) {
			if(prev != null) {
				prev.Changed -= ImageUriChanged;
			}
			if(next != null) {
				next.Changed += ImageUriChanged;
				next.SetClient(this);
			}
			OnPropertiesChanged();
		}
		protected void ImageUriChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		protected virtual DxImageUri CreateImageUriInstance() {
			return new DxImageUri();
		}
		protected internal bool GetAllowGlyphSkinning() {
			if(AllowGlyphSkinning == DefaultBoolean.True) return true;
			if(AllowGlyphSkinning == DefaultBoolean.False) return false;
			return Owner != null && Owner.AllowGlyphSkinning == DefaultBoolean.True;
		}
		protected internal Image GetImage() {
			if(ImageUri != null) {
				if(ImageUri.HasLargeImage) return ImageUri.GetLargeImage();
				if(ImageUri.HasImage) return ImageUri.GetImage();
			}
			if(Image != null) return Image;
			if(Images != null)
				return ImageCollection.GetImageListImage(Images, ImageIndex);
			return null;
		}
		[Browsable(false)]
		public object Images {
			get {
				if(Owner == null) return null;
				return Owner.Images;
			}
		}
		int imageIndex = -1;
		[ Category(CategoryName.Appearance), DefaultValue(-1), Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)), ImageList("Images")]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex == value)
					return;
				imageIndex = value;
				OnPropertiesChanged();
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(null)]
		public XtraScrollableControl ContentContainer {
			get { return contentContainer; }
			set {
				if(ContentContainer == value)
					return;
				contentContainer = value;
				OnPropertiesChanged();
			}
		}
		internal void CreateContentContainer() {
			if(ContentContainer == null) {
				ContentContainer = new XtraScrollableControl();
				ContentContainer.Dock = DockStyle.Fill;
				if(Owner != null && Owner.IsDesignMode) {
					Owner.DesignManager.DesignerHost.Container.Add(ContentContainer);
				}
			}
		}
		protected void OnPropertiesChanged() {
			if(Owner != null) Owner.LayoutChanged();
		}
		TabFormControlBase owner;
		[Browsable(false)]
		public TabFormControlBase Owner {
			get { return owner; }
		}
		public void SetOwner(TabFormControlBase owner) {
			this.owner = owner;
			CreateContentContainer();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Owner != null && !Owner.Disposing) {
					if(object.Equals(this, Owner.SelectedPage)) {
						Owner.Handler.UpdateSelectedPage(Owner.SelectedPage);
					}
				}
				if(ContentContainer != null) {
					if(Owner != null && Owner.IsDesignMode)
						Owner.Container.Remove(ContentContainer);
					ContentContainer.Dispose();
					ContentContainer = null;
				}
				if(Owner != null && !Owner.Disposing)
					Owner.Pages.Remove(this);
				if(this.imageUri != null) {
					this.imageUri.Changed -= ImageUriChanged;
					this.imageUri.Dispose();
				}
				this.imageUri = null;
			}
			base.Dispose(disposing);
		}
		bool IDXImageUriClient.IsDesignMode { get { return DesignMode; } }
		UserLookAndFeel IDXImageUriClient.LookAndFeel {
			get {
				if(Owner == null) return null;
				return Owner.LookAndFeel;
			}
		}
		void IDXImageUriClient.SetGlyphSkinningValue(bool value) {
			AllowGlyphSkinning = (value ? DefaultBoolean.True : DefaultBoolean.False);
		}
		bool IDXImageUriClient.SupportsGlyphSkinning { get { return true; } }
		bool IDXImageUriClient.SupportsLookAndFeel { get { return true; } }
	}
	[TypeConverter(typeof(UniversalCollectionTypeConverter))]
	public class TabFormPageCollection : CollectionBase {
		int lockUpdate;
		TabFormControlBase owner;
		public TabFormPageCollection(TabFormControlBase owner) {
			this.lockUpdate = 0;
			this.owner = owner;
		}
		public TabFormControlBase Owner { get { return owner; } }
		public virtual void Add(TabFormPage page) {
			List.Add(page);
			page.SetOwner(Owner);
		}
		public virtual TabFormPage this[int index] {
			get { return List[index] as TabFormPage; }
			set {
				if(value == null) return;
				List[index] = value;
			}
		}
		public virtual bool Remove(TabFormPage page) {
			if(!Contains(page)) return false;
			List.Remove(page);
			return true;
		}
		public virtual void Insert(int position, TabFormPage page) {
			if(Contains(page)) return;
			List.Insert(position, page);
		}
		public virtual void Move(int newIndex, TabFormPage page) {
			Owner.LockUpdateHeight();
			Remove(page);
			Owner.UnlockUpdateHeight();
			Insert(newIndex, page);
		}
		public virtual bool Contains(TabFormPage page) {
			return List.Contains(page);
		}
		public virtual int IndexOf(TabFormPage page) {
			return List.IndexOf(page);
		}
		public virtual void BeginUpdate() { lockUpdate++; }
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) {
				RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
		}
		protected override void OnInsert(int position, object value) {
			base.OnInsert(position, value);
		}
		protected override void OnInsertComplete(int position, object value) {
			base.OnInsertComplete(position, value);
			((TabFormPage)value).SetOwner(Owner);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, position));
		}
		protected override void OnRemoveComplete(int position, object value) {
			((TabFormPage)value).SetOwner(null);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, position));
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}
		protected virtual void RaiseListChanged(ListChangedEventArgs e) {
			if(lockUpdate != 0) return;
			if(ListChanged != null) ListChanged(this, e);
		}
		public event ListChangedEventHandler ListChanged;
		public void Dispose() {
			for(int i = 0; i < Count; i++) {
				this[i].Dispose();
			}
		}
	}
	public class TabFormPageViewInfo : ISupportXtraAnimation {
		Rectangle bounds;
		TabFormPage page;
		ObjectState closeButtonState;
		public TabFormPageViewInfo(TabFormPage page) {
			this.bounds = Rectangle.Empty;
			this.closeButtonState = ObjectState.Normal;
			this.page = page;
		}
		public TabFormPage Page { get { return page; } }
		public TabFormControlBase TabFormControl { get { return Page.Owner; } }
		internal Rectangle Bounds {
			get { return bounds; }
			set {
				CurrentBounds = value;
				bounds = value;
			}
		}
		internal int BestWidth { get; set; }
		internal Rectangle CurrentBounds { get; set; }
		internal Rectangle TextContentBounds { get; set; }
		internal Rectangle ImageContentBounds { get; set; }
		internal Rectangle CloseButtonBounds { get; set; }
		public ObjectState CloseButtonState {
			get {
				if(!Page.GetEnabled())
					return ObjectState.Disabled;
				return closeButtonState;
			}
			set {
				if(closeButtonState == value)
					return;
				closeButtonState = value;
				if(page.Owner != null && !page.Owner.IsInAnimation)
					Page.Owner.Invalidate();
			}
		}
		public Rectangle GetTextBounds() {
			Rectangle rect = TextContentBounds;
			rect.X += CurrentBounds.X;
			rect.Y += CurrentBounds.Y;
			return rect;
		}
		public Rectangle GetImageBounds() {
			Rectangle rect = ImageContentBounds;
			rect.X += CurrentBounds.X;
			rect.Y += CurrentBounds.Y;
			return rect;
		}
		public Rectangle GetCloseButtonBounds() {
			Rectangle rect = CloseButtonBounds;
			rect.X += CurrentBounds.X;
			rect.Y += CurrentBounds.Y;
			return rect;
		}
		internal AppearanceObject PaintAppearance { get; set; }
		internal void UpdateCurrentLeft(int x) {
			Rectangle rect = Bounds;
			if(TabFormControl.ViewInfo.PageInfos.Count > 1) {
				int minX, maxX;
				if(TabFormControl.IsRightToLeft) {
					minX = TabFormControl.ViewInfo.PageInfos[TabFormControl.ViewInfo.PageInfos.Count - 1].Bounds.X;
					maxX = TabFormControl.ViewInfo.PageInfos[0].Bounds.Right - Bounds.Width;
				}
				else {
					minX = TabFormControl.ViewInfo.PageInfos[0].Bounds.X;
					maxX = TabFormControl.ViewInfo.PageInfos[TabFormControl.ViewInfo.PageInfos.Count - 1].Bounds.Right - Bounds.Width;
				}
				rect.X = Math.Min(maxX, Math.Max(minX, x));
			}
			CurrentBounds = rect;
		}
		public bool CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return TabFormControl; }
		}
		public void ReverseRects() {
			TextContentBounds = ReverseRect(TextContentBounds);
			ImageContentBounds = ReverseRect(ImageContentBounds);
			CloseButtonBounds = ReverseRect(CloseButtonBounds);
		}
		Rectangle ReverseRect(Rectangle rect) {
			Rectangle res = rect;
			res.X = Bounds.Width - rect.Right;
			return res;
		}
	}
}
