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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Windows.Forms;
using DevExpress.Data.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Design;
namespace DevExpress.Utils {
	[Editor("DevExpress.Utils.Design.ImageInfoImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
	public class ImageInfo {
		string name;
		Image image;
		byte[] imdata;
		internal ImageInfo(byte[] imdata) {
			this.name = String.Empty;
			this.imdata = imdata;
		}
		public ImageInfo(string name, Image image) {
			this.name = name;
			this.image = image;
		}
		internal void Assign(ImageInfo imageInfo) {
			imdata = imageInfo.imdata;
			image = imageInfo.image;
		}
		public string Name { get { return name; } set { name = value; } }
		[Browsable(false)]
		public Image Image {
			get {
				if(imdata != null) {
					MemoryStream temp = new MemoryStream(imdata);
					image = ImageTool.ImageFromStream(temp);
					imdata = null;
				}
				return image;
			}
			set { image = value; imdata = null; }
		}
		public float HorizontalResolution { get { return Image.HorizontalResolution; } }
		public Size PhysicalDimension { get { return Image.Size; } }
		public PixelFormat PixelFormat { get { return Image.PixelFormat; } }
		public ImageFormat RawFormat { get { return Image.RawFormat; } }
		public Size Size { get { return Image.Size; } }
		public float VerticalResolution { get { return Image.VerticalResolution; } }
		[Browsable(false)]
		public virtual bool ShouldSerialize { get { return true; } }
	}
	public class ProjectImageInfo : ImageInfo {
		string originalResName;
		public ProjectImageInfo(string name, Image image, Type parentType)
			: this(name, image, parentType, null) {
		}
		public ProjectImageInfo(string name, Image image, Type parentType, string resName)
			: base(name, image) {
			ParentType = parentType;
			this.originalResName = resName;
		}
		public void SetResName(string resName) { this.originalResName = resName; }
		public string ResourceName {
			get {
				if(string.IsNullOrEmpty(this.originalResName))
					return Name;
				return this.originalResName;
			}
		}
		new public string Name { get { return base.Name; } set { base.Name = value; } }
		[Browsable(false)]
		public Type ParentType { get; private set; }
		[Browsable(false)]
		public override bool ShouldSerialize { get { return false; } }
		#region Equals & GetHashCode
		public override bool Equals(object obj) {
			ProjectImageInfo p = obj as ProjectImageInfo;
			if(p == null) return false;
			return p.ParentType == ParentType && p.originalResName == originalResName;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#endregion
	}
	public class DXGalleryImageInfo : ImageInfo {
		string uri;
		public DXGalleryImageInfo(string name, string uri, Image image)
			: base(name, image) {
			this.uri = uri;
		}
		public string Uri { get { return uri; } }
		[Browsable(false)]
		public override bool ShouldSerialize { get { return false; } }
	}
	public class SolutionImageInfo : ImageInfo {
		string assemblyName;
		string path;
		public SolutionImageInfo(string name, Image image, string assemblyName, string path) : base(name, image) {
			this.assemblyName = assemblyName;
			this.path = path;
		}
		public string Path { get { return path; } }
		public string AssemblyName { get { return assemblyName; } }
		[Browsable(false)]
		public override bool ShouldSerialize { get { return false; } }
		#region Equals & GetHashCode
		public override bool Equals(object obj) {
			SolutionImageInfo s = obj as SolutionImageInfo;
			if(s == null) return false; 
			return s.AssemblyName == AssemblyName && s.Path == Path;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#endregion
	}
	public class InnerImagesList : List<ImageInfo>, IList {
		Images owner;
		public InnerImagesList(Images owner) {
			this.owner = owner;
		}
		public void SetKeyName(int index, string name) {
			if(index < 0 || index >= Count) {
				return;
			}
			this[index].Name = name;
		}
		int IList.Add(object obj) {
			ImageInfo info = obj as ImageInfo;
			info.Image = owner.Resize(info.Image);
			Add(obj as ImageInfo);
			return Count;
		}
	}
	[ListBindable(false), Editor("DevExpress.Utils.Design.ImageCollectionEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class Images : IList {
		int lockUpdate;
		ImageCollection owner;
		InnerImagesList innerImages;
		bool allowModifyImages;
		public Images(ImageCollection owner) : this(owner, true) {
		}
		public Images(ImageCollection owner, bool allowModifyImages) {
			this.innerImages = new InnerImagesList(this);
			this.lockUpdate = 0;
			this.owner = owner;
			this.allowModifyImages = allowModifyImages;
			this.ForceMakeBitmapTransparent = false;
		}
		protected internal bool AllowModifyImages { get { return allowModifyImages; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public InnerImagesList InnerImages { get { return innerImages; } }
		internal bool ReCreateImages() {
			if(Count > 0)
				if(DialogResult.No == MessageBox.Show("The collection will be cleared. Do you want to continue?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
					return false;
			InnerImages.Clear();
			return true;
		}
		protected ImageCollection Owner { get { return owner; } }
		protected virtual void BeginUpdate() {
			lockUpdate++;
		}
		protected virtual void EndUpdate() {
			if(--lockUpdate == 0)
				OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		public virtual Image this[int index] { 
			get { return InnerImages[index].Image; }
			set {
				if(value == null) throw new ArgumentNullException();
				InnerImages[index].Image = value;
				OnInsertComplete(index, value);
			}
		}
		public Image this[string name] {
			get {
				ImageInfo ii = Find(name);
				if(ii != null) return ii.Image;
				return null;
			}
			set {
				if(value == null) throw new ArgumentNullException();
				ImageInfo ii = Find(name);
				if(ii == null) return;
				ii.Image = value;
				OnInsertComplete(InnerImages.IndexOf(ii), value);
			}
		}
		ImageInfo Find(string name) {
			for(int n = 0; n < InnerImages.Count; n++) {
				ImageInfo ii = InnerImages[n];
				if(string.Compare(name, ii.Name, true, CultureInfo.InvariantCulture) == 0) return ii;
			}
			return null;
		}
		public Image GetImageFromCollection(Image iml, int width, int height, int wIndex, int hIndex) {
			Bitmap res = new Bitmap(width, height);
			res.SetResolution(iml.HorizontalResolution, iml.VerticalResolution);
			Graphics g = Graphics.FromImage(res);
			g.DrawImage(iml, 0, 0, new Rectangle(width * wIndex, height * hIndex, width, height), GraphicsUnit.Pixel);
			g.Dispose();
			MakeTransparent(res);
			return res;
		}
		protected virtual void AddImageStrip(Image sourceImage, bool vertical) {
			BeginUpdate();
			try {
				Size size = Owner.ImageSize;
				int index = 0;
				for(int i = vertical ? sourceImage.Height : sourceImage.Width; i > 0; i -= vertical ? size.Height : size.Width) {
					InnerImages.Add(new ImageInfo("", GetImageFromCollection(sourceImage, size.Width, size.Height, vertical ? 0 : index, vertical ? index : 0)));
					index++;
				}
			}
			finally {
				EndUpdate();
			}
		}
		public void AddImageStrip(Image sourceImage) {
			AddImageStrip(sourceImage, false);
		}
		public void AddImageStripVertical(Image sourceImage) {
			AddImageStrip(sourceImage, true);
		}
		public void SetKeyName(int index, string name) {
			if(index < 0 || index >= Count)
				throw new IndexOutOfRangeException();
			InnerImages[index].Name = name;
		}
		internal void Assign(ImageCollectionStreamer stream) {
			if(stream == null || stream.Collection == null || stream.Collection.Images.Count == 0) {
				InnerImages.Clear();
				return;
			}
			InnerImages.Clear();
			for(int n = 0; n < stream.Collection.Images.Count; n++) {
				if(n >= InnerImages.Count) InnerImages.Add(new ImageInfo("", null));
				InnerImages[n].Assign(stream.Collection.Images.InnerImages[n]);
			}
		}
		internal void AddCore(byte[] imdata) {
			if(imdata == null) return;
			InnerImages.Add(new ImageInfo(imdata));
		}
		public void AddRange(object[] images) {
			foreach(object image in images) {
				if(image is Image)
					Add(image as Image);
			}
		}
		public void AddRange(IList images) {
			foreach(object image in images) {
				if(image is Image)
					Add(image as Image);
			}
		}
		public virtual int Add(Image image) {
			return Add(image, string.Empty);
		}
		public virtual int Add(Image image, string name) {
			if(image == null || image.Size.IsEmpty) return -1;
			InnerImages.Add(new ImageInfo(name, image));
			int res = InnerImages.Count - 1;
			OnInsertComplete(res, image);
			return res;
		}
		protected virtual void OnInsertComplete(int index, object item) {
			Image image = item as Image;
			if(image == null) throw new ArgumentException("image");
			ImageInfo i = InnerImages[index];
			if(allowModifyImages)
				i.Image = Resize(image);
			else {
				if(AllowMakeBitmapTransparent) MakeTransparent(i.Image);
			}
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected internal virtual Image Resize(Image image) {
			if(allowModifyImages) {
				if(image.Size != Owner.ImageSize) {
					image = new Bitmap(image, Owner.ImageSize);
				}
			}
			if(AllowMakeBitmapTransparent) MakeTransparent(image);
			return image;
		}
		protected internal virtual bool ForceMakeBitmapTransparent { get; set; }
		protected bool AllowMakeBitmapTransparent {
			get { return allowModifyImages || ForceMakeBitmapTransparent; }
		}
		protected virtual void OnRemoveComplete(int index, object item) {
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}
		protected void MakeTransparent(Image image) {
			if(Owner.TransparentColor == Color.Empty) return;
			Bitmap bitmap = image as Bitmap;
			if(bitmap != null) bitmap.MakeTransparent(Owner.TransparentColor);
		}
		protected virtual void OnClear() {
			if(Count == 0) return;
			BeginUpdate();
			try {
				InnerImages.Clear();
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual void OnCollectionChanged(CollectionChangeEventArgs e) {
			if(this.lockUpdate != 0) return;
			Owner.OnChanged();
		}
		#region IList Members
		int IList.Add(object obj) {
			ImageInfo info = obj as ImageInfo;
			if(info == null)
				return Add(obj as Image);
			ProjectImageInfo pInfo = info as ProjectImageInfo;
			if(pInfo != null) {
				return Insert(pInfo.Image, pInfo.Name, pInfo.ParentType, -1);
			}
			SolutionImageInfo sImg = obj as SolutionImageInfo;
			if(sImg != null) {
				return Insert(sImg.AssemblyName, sImg.Name, sImg.Path, -1);
			}
			return Add(info.Image, info.Name);
		}
		object IList.this[int index] { get { return this[index]; } set { } }
		public void Clear() {
			OnClear();
		}
		public bool Contains(object value) { return IndexOf(value) != -1; }
		public int IndexOf(object value) {
			for(int n = 0; n < InnerImages.Count; n++) {
				if(object.ReferenceEquals(InnerImages[n].Image, value)) return n;
			}
			return -1;
		}
		public void Insert(int index, object value) {
			Insert(index, value, "");
		}
		public void Insert(int index,object value,string name) {
			Image image = value as Image;
			if(image == null) return;
			InnerImages.Insert(index, new ImageInfo(name, image));
			OnInsertComplete(index, image);
		}
		public virtual int Insert(Image image, string name, Type resourceType, int index) {
			return Insert(image, name, resourceType, index, name);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int Insert(Image image, string name, Type resourceType, int index, string resName) {
			if(image == null || image.Size.IsEmpty) return -1;
			ProjectImageInfo imageInfo = CreateProjectImageInfo(name, resName, image, resourceType);
			imageInfo.SetResName(resName);
			if(index == -1 || index == InnerImages.Count) {
				InnerImages.Add(imageInfo);
				index = InnerImages.Count - 1;
			}
			else InnerImages.Insert(index, imageInfo);
			OnInsertComplete(index, image);
			return index;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void InsertGalleryImage(string name, string uri, Image image, int index) {
			if(image == null || image.Size.IsEmpty) return;
			DXGalleryImageInfo imageInfo = new DXGalleryImageInfo(name, uri, image);
			if(index == -1 || index == InnerImages.Count) {
				InnerImages.Add(imageInfo);
				index = InnerImages.Count - 1;
			}
			else InnerImages.Insert(index, imageInfo);
			OnInsertComplete(index, image);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int Insert(string assemblyName, string name, string path, int index) {
			Image image = ImageCollectionImageCache.Cache.GetImage(assemblyName, path, Owner.IsDesignMode);
			if(image == null || image.Size.IsEmpty) return -1;
			SolutionImageInfo imageInfo = new SolutionImageInfo(name, image, assemblyName, path);
			if(index == -1 || index == InnerImages.Count) {
				InnerImages.Add(imageInfo);
				index = InnerImages.Count - 1;
			}
			else InnerImages.Insert(index, imageInfo);
			OnInsertComplete(index, image);
			return index;
		}
		protected virtual ProjectImageInfo CreateProjectImageInfo(string name, string resName, Image defImg, Type resource) {
			Image projImg = defImg;
			if(Owner != null && !Owner.IsDesignMode && Owner.AllowDPIAware()) {
				Image img = GetDPIDependentImage(resName, resource);
				if(img != null) projImg = img;
			}
			return new ProjectImageInfo(name, projImg, resource);
		}
		protected virtual Image GetDPIDependentImage(string resName, Type resource) {
			Image img = Owner.ImageResolver.ResolveImage(resName, resource);
			if(img == null)
				img = RaiseResolveImage(resName);
			return img;
		}
		protected virtual Image RaiseResolveImage(string resName) {
			ImageCollectionResolveImageEventArgs e = new ImageCollectionResolveImageEventArgs(resName, ScaleUtils.GetSystemDPI());
			Owner.RaiseResolveImage(e);
			return e.Image;
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ImagesIsFixedSize")]
#endif
public bool IsFixedSize { get { return false; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ImagesIsReadOnly")]
#endif
public bool IsReadOnly { get { return false; } }
		public void Remove(object value) {
			RemoveAt(IndexOf(value));
		}
		public void RemoveByName(string name) {
			ImageInfo ii = Find(name);
			RemoveAt(InnerImages.IndexOf(ii));
		}
		public void RemoveAt(int index) {
			if(index < 0) throw new IndexOutOfRangeException();
			ImageInfo i = InnerImages[index];
			InnerImages.RemoveAt(index);
			OnRemoveComplete(index, i.Image);
		}
		public void CopyTo(Array array, int index) {
			GetImageArray().CopyTo(array, index);
		}
		public Image[] ToArray() {
			return (Image[])GetImageArray().ToArray(typeof(Image));
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ImagesCount")]
#endif
public int Count { get { return InnerImages.Count; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ImagesIsSynchronized")]
#endif
public bool IsSynchronized { get { return true; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ImagesSyncRoot")]
#endif
public object SyncRoot { get { return this; } }
		public IEnumerator GetEnumerator() {
			return GetImageArray().GetEnumerator();
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ImagesKeys")]
#endif
public StringCollection Keys {
			get {
				StringCollection strings = new StringCollection();
				for(int i = 0; i < InnerImages.Count; i++) {
					ImageInfo info = InnerImages[i];
					strings.Add(info.Name);
				}
				return strings;
			}
		}
		ArrayList GetImageArray() {
			ArrayList list = new ArrayList();
			for(int n = 0; n < InnerImages.Count; n++) {
				list.Add(InnerImages[n].Image);
			}
			return list;
		}
		#endregion
		internal static bool IsEquals(Images lCol, Images rCol) {
			if(lCol == null || rCol == null)
				return false;
			if(lCol.Count != rCol.Count)
				return false;
			bool equals = true;
			try {
				for(int i = 0; i < lCol.Count; i++) {
					if(lCol[i].Size != rCol[i].Size || lCol.Keys[i] != rCol.Keys[i]) {
						equals = false;
						break;
					}
				}
			}
			catch { equals = false; }
			return equals;
		}
	}
	public interface IImageCollectionHelper {
		Control OwnerControl { get; }
	}
	[Designer("DevExpress.Utils.Design.ImageCollectionDesigner, " + AssemblyInfo.SRAssemblyDesign, typeof(IDesigner)),
	 Description("Represents a collection of System.Drawing.Image objects and supports alpha channels in images."),
   DesignerSerializer("DevExpress.Utils.Design.ImageCollectionCodeDomSerializer, " + AssemblyInfo.SRAssemblyDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design"),
	 ToolboxTabName(AssemblyInfo.DXTabComponents), DXToolboxItem(DXToolboxItemKind.Free),
	ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "ImageCollection")
	]
	public class ImageCollection : Component, ISupportInitialize, ICloneable {
		Size imageSize;
		Color transparentColor = Color.Empty;
		Images images;
		DefaultBoolean isDpiAware;
		IImageResolver imageResolver;
		private static readonly object changed = new object();
		private static readonly object getImageSize = new object();
		private static readonly object resolveImage = new object();
		static ImageCollection() {
			DevExpress.Utils.Design.DXAssemblyResolverEx.Init();
		}
		public ImageCollection(IContainer container) : this() {
			container.Add(this);
		}
		public ImageCollection() : this(true) {
		}
		public ImageCollection(bool allowModifyImages) {
			this.images = CreateImages(allowModifyImages);
			this.imageSize = DefaultSize;
			this.isDpiAware = DefaultBoolean.Default;
			this.imageResolver = CreateDpiAwareImageResolver();
			ResetTransparentColor();
		}
		protected virtual IImageResolver CreateDpiAwareImageResolver() {
			return new ImageCollectionDpiAwareImageResolver(this);
		}
		object ICloneable.Clone() {
			ImageCollection res = new ImageCollection();
			res.imageSize = ImageSize;
			res.isDpiAware = IsDpiAware;
			res.transparentColor = TransparentColor;
			foreach(Image image in Images) {
				res.Images.Add(image.Clone() as Image);
			}
			return res;
		}
		internal static Size DefaultSize = new Size(16, 16);
		void ResetImageSize() { ImageSize = Size.Empty;	}
		bool ShouldSerializeImageSize() {
			if(AllowDPIAware()) return true;
			return ImageSize != DefaultSize;
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ImageCollectionImageSize"),
#endif
 Localizable(true), RefreshProperties(RefreshProperties.All)]
		public Size ImageSize {
			get { return imageSize; }
			set {
				if(value == Size.Empty) value = DefaultSize;
				if(ImageSize == value) return;
				if(this._lockInit == 0 && DesignMode)
					if(!Images.ReCreateImages()) return;
				imageSize = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ImageCollectionIsDpiAware"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean IsDpiAware {
			get { return isDpiAware; }
			set {
				if(IsDpiAware == value)
					return;
				if(!EnsureAllowModifyImages(value))
					return;
				isDpiAware = value;
				OnIsDPIAwareChanged();
			}
		}
		protected virtual void OnIsDPIAwareChanged() {
			if(AllowDPIAware()) CheckDPIAwareImageSize();
			OnChanged();
		}
		protected internal IImageResolver ImageResolver { get { return imageResolver; } }
		protected virtual bool EnsureAllowModifyImages(DefaultBoolean isDpiAware) {
			bool allowDpiAware = AllowDPIAwareCore(isDpiAware);
			if((allowDpiAware && !Images.AllowModifyImages) || !allowDpiAware && Images.AllowModifyImages)
				return true;
			if(this._lockInit == 0 && DesignMode) {
				if(!Images.ReCreateImages()) return false;
			}
			this.images = CreateImages(!allowDpiAware);
			return true;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool AllowDPIAware() { return AllowDPIAwareCore(IsDpiAware); }
		protected virtual bool AllowDPIAwareCore(DefaultBoolean isDpiAware) {
			return isDpiAware == DefaultBoolean.True;
		}
		protected internal bool IsDesignMode { get { return DesignMode; } }
		void ResetTransparentColor() {	TransparentColor = Color.Empty; }
		bool ShouldSerializeTransparentColor() { return TransparentColor != Color.Empty;}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ImageCollectionTransparentColor"),
#endif
 Localizable(true)]
		public Color TransparentColor {
			get { return transparentColor; }
			set {
				if(TransparentColor == value) return;
				transparentColor = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ImageCollectionImages"),
#endif
 RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Images Images { get { return images; } }
		[Browsable(false)]
		public ImageCollectionStreamer ImageStream {
			get { return new ImageCollectionStreamer(this); }
			set {
				Images.Assign(value);
				OnChanged();
			}
		}
		int _lockInit = 0;
		public void BeginInit() {
			_lockInit ++;
		}
		public void EndInit() {
			_lockInit = 0;
			CheckDPIAwareImageSize();
		}
		bool sizeChecked = false;
		protected void CheckDPIAwareImageSize() {
			if(this.sizeChecked) return;
			if(!DesignMode && AllowDPIAware()) {
				this.imageSize = CheckImageSize(ImageSize);
				this.sizeChecked = true;
			}
		}
		protected virtual Size CheckImageSize(Size value) {
			if(DesignMode || !AllowDPIAware()) return value;
			ImageCollectionGetImageSizeEventArgs e = new ImageCollectionGetImageSizeEventArgs(ScaleUtils.GetSystemDPI(), ScaleUtils.GetScaleFactor());
			RaiseGetImageSize(e);
			return new Size((int)(value.Width * e.ScaleFactor.Width), (int)(value.Height * e.ScaleFactor.Height));
		}
		public void AddImageStrip(Image image) {
			Images.AddImageStrip(image);
		}
		public void AddImageStripVertical(Image image) {
			Images.AddImageStripVertical(image);
		}
		public void AddImage(Image image) {
			AddImage(image, string.Empty);
		}
		public void AddImage(Image image, string name) {
			Images.Add(image, name);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void InsertImage(Image image, string name, Type resourceType, int index) {
			Images.Insert(image, name, resourceType, index);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void InsertImage(Image image, string name, Type resourceType, int index, string resName) {
			Images.Insert(image, name, resourceType, index, resName);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void InsertImage(int asmId, string name, string path, int index) {
			Images.Insert(RegisteredAssemblies.GetAssemblyName(asmId), name, path, index);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void InsertGalleryImage(string name, string uri, Image image, int index) {
			Images.InsertGalleryImage(name, uri, image, index);
		}
		ImageCollectionRegisteredAssemblies registeredAssemblies = null;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageCollectionRegisteredAssemblies RegisteredAssemblies {
			get {
				if(this.registeredAssemblies == null) {
					this.registeredAssemblies = new ImageCollectionRegisteredAssemblies();
				}
				return this.registeredAssemblies;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RegisterAssemblies(string[] assemblies) {
			for(int i = 0; i < assemblies.Length; i++) {
				RegisteredAssemblies.Register(assemblies[i]);
			}
		}
		public void Clear() {
			Images.Clear();
		}
		protected internal virtual void OnChanged() {
			if(this._lockInit != 0) return;
			FireChanged();
			RaiseChanged(EventArgs.Empty);
		}
		protected void FireChanged() {
			if(!DesignMode || this._lockInit != 0) return;
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv == null) return;
			srv.OnComponentChanged(this, null, null, null);
		}
		protected internal virtual void RaiseChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[changed];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseGetImageSize(ImageCollectionGetImageSizeEventArgs e) {
			ImageCollectionGetImageSizeEventHandler handler = (ImageCollectionGetImageSizeEventHandler)this.Events[getImageSize];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseResolveImage(ImageCollectionResolveImageEventArgs e) {
			ImageCollectionResolveImageEventHandler handler = (ImageCollectionResolveImageEventHandler)this.Events[resolveImage];
			if(handler != null) handler(this, e);
		}
		protected virtual Images CreateImages(bool allowModifyImages) {
			return new Images(this, allowModifyImages);
		}
		[Category("Events")]
		public event EventHandler Changed {
			add { this.Events.AddHandler(changed, value); }
			remove { this.Events.RemoveHandler(changed, value); }
		}
		[Category("Events")]
		public event ImageCollectionGetImageSizeEventHandler GetImageSize {
			add { this.Events.AddHandler(getImageSize, value); }
			remove { this.Events.RemoveHandler(getImageSize, value); }
		}
		[Category("Events")]
		public event ImageCollectionResolveImageEventHandler ResolveImage {
			add { this.Events.AddHandler(resolveImage, value); }
			remove { this.Events.RemoveHandler(resolveImage, value); }
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.imageResolver != null) {
					this.imageResolver.Dispose();
				}
				this.imageResolver = null;
				if(this.registeredAssemblies != null) {
					this.registeredAssemblies.Dispose();
				}
				this.registeredAssemblies = null;
			}
			this.imageResolver = null;
			base.Dispose(disposing);
		}
		static ImageCollection TryGetImageCollection(object images) {
			if(images is ImageCollection)
				return (ImageCollection)images;
			if(images is SharedImageCollection)
				return ((SharedImageCollection)images).ImageSource;
			return null;
		}
		public static Image ImageFromFile(string fileName) {
			if(!File.Exists(fileName)) return null;
			FileInfo fInfo = new FileInfo(fileName);
			if(fInfo.Extension.ToLower().Equals(".ico")) {
				try {
				using(Icon icon = new Icon(fileName))
					return icon.ToBitmap(); 
				} catch {}
			}
			try {
				return Image.FromFile(fileName); 
			} catch {}
			return null;
		}
		public static void AddImage(object images, Image image) {
			if(images == null || image == null) return;
			ImageList list = images as ImageList;
			if(list != null) {
				list.Images.Add(image);
				return;
			}
			ImageCollection coll = TryGetImageCollection(images);
			if(coll != null) coll.AddImage(image);
		}
		public static void DrawImageListImage(GraphicsCache cache, object images, int index, Rectangle bounds) {
			if(images == null) return;
			ImageList list = images as ImageList;
			if(list != null) {
				bounds = cache.CalcRectangle(bounds);
				list.Draw(cache.Graphics, bounds.X, bounds.Y, bounds.Width, bounds.Height, index);
				return;
			}
			Image image = GetImageListImage(images, index);
			if(image == null) return;
			cache.Paint.DrawImage(cache.Graphics, image, bounds);
		}
		public static void DrawImageListImage(GraphicsCache cache, Image image, object images, int index, Rectangle bounds, bool enabled) {
			if(image != null) {
				cache.Paint.DrawImage(cache.Graphics, image, bounds, new Rectangle(Point.Empty, image.Size), enabled);
				return;
			}
			if(IsImageExists(null, images, index))
				DrawImageListImage(cache, images, index, bounds, enabled);
		}
		public static void DrawImageListImage(GraphicsCache cache, Image image, object images, int index, Rectangle bounds, ImageAttributes attributes) {
			if(image != null) {
				cache.Paint.DrawImage(cache.Graphics, image, bounds, new Rectangle(Point.Empty, image.Size), attributes);
				return;
			}
			if(IsImageExists(null, images, index))
				DrawImageListImage(cache, images, index, bounds, attributes);
		}
		public static void DrawImageListImage(GraphicsCache cache, object images, int index, Rectangle bounds, ImageAttributes attributes) {
			if(images == null) return;
			ImageList list = images as ImageList;
			if(list != null) {
				cache.Paint.DrawImage(cache.Graphics, list.Images[index], bounds, new Rectangle(Point.Empty, list.Images[index].Size), attributes);
				return;
			}
			Image image = GetImageListImage(images, index);
			if(image == null) return;
			cache.Paint.DrawImage(cache.Graphics, image, bounds, new Rectangle(Point.Empty, image.Size), attributes);
		}
		public static void DrawImageListImage(GraphicsCache cache, object images, int index, Rectangle bounds, bool enabled) {
			if(enabled) {
				DrawImageListImage(cache, images, index, bounds);
				return;
			}
			if(images == null) return;
			ImageList list = images as ImageList;
			if(list != null) {
				cache.Paint.DrawImage(cache, list, index, bounds, false);
				return;
			}
			Image image = GetImageListImage(images, index);
			if(image == null) return;
			cache.Paint.DrawImage(cache.Graphics, image, bounds, new Rectangle(Point.Empty, image.Size), false);
		}
		public static void DrawImageListImage(GraphicsInfoArgs info, object images, int index, Rectangle bounds, bool enabled) {
			DrawImageListImage(info.Cache, images, index, bounds, enabled);
		}
		public static Size GetImageListSize(Image image, object images, int index) {
			if(image != null) return image.Size;
			if(IsImageListImageExists(images, index)) return GetImageListSize(images);
			return Size.Empty;
		}
		public static Size GetImageListSize(object images) {
			if(images == null) return Size.Empty;
			ImageList list = images as ImageList;
			if(list != null) return list.ImageSize;
			ImageCollection ic = TryGetImageCollection(images);
			if(ic != null) return ic.ImageSize;
			return Size.Empty;
		}
		public static bool IsImageExists(Image image, object images, int index) {
			return image != null || IsImageListImageExists(images, index);
		}
		public static bool IsImageListImageExists(object images, int index) {
			if(images == null || index < 0) return false;
			ImageList list = images as ImageList;
			if(list != null) return index < list.Images.Count;
			ImageCollection ic = TryGetImageCollection(images);
			if(ic != null) return index < ic.Images.Count;
			return false;
		}
		public static int GetImageListImageCount(object images) {
			if(images == null) return 0;
			ImageList list = images as ImageList;
			if(list != null) return list.Images.Count;
			ImageCollection ic = TryGetImageCollection(images);
			if(ic != null) return ic.Images.Count;
			return 0;
		}
		public static Image GetImageListImage(object images, int index) {
			if(images == null || index < 0) return null;
			ImageList list = images as ImageList;
			if(list != null) return index < list.Images.Count ? list.Images[index] : null;
			ImageCollection ic = TryGetImageCollection(images);
			if(ic != null) return index < ic.Images.Count ? ic.Images[index] : null;
			return null;
		}
		public static Image GetImageListImage(object images, string id) {
			if(images == null || string.IsNullOrEmpty(id)) return null;
			Image result = null;
			ImageList list = images as ImageList;
			if(list != null)
				result = list.Images[id];
			if(result != null) return result;
			ImageCollection ic = TryGetImageCollection(images);
			if(ic != null) return ic.Images[id];
			return null;
		}
		public void ExportToFile(string fileName) {
			if(this.Images.Count == 0) return;
			try {
				Bitmap images = new Bitmap(this.ImageSize.Width * this.Images.Count, this.ImageSize.Height);
				for(int i = 0; i < this.Images.Count; i++) {
					Graphics g = Graphics.FromImage(images);
					g.DrawImage(this.Images[i], new Point(i * this.ImageSize.Width, 0));
					g.Dispose();
				}
				if(System.IO.File.Exists(fileName)) {
					System.IO.FileInfo fi = new FileInfo(fileName);
					fi.Delete();
				}
				images.Save(fileName, this.Images[0].RawFormat);
			}
			catch(Exception e) {
				MessageBox.Show(e.Message, e.Source);
			}
		}
	}
	public class ImageCollectionImageCache {
		Dictionary<string, Dictionary<string, ImageValueHolder>> dict;
		protected ImageCollectionImageCache() {
			this.dict = new Dictionary<string, Dictionary<string, ImageValueHolder>>();
		}
		static ImageCollectionImageCache cache = null;
		public static ImageCollectionImageCache Cache {
			get {
				if(cache == null) cache = new ImageCollectionImageCache();
				return cache;
			}
		}
		public static bool IsCacheCreated { get { return cache != null; } }
		public static void Reset() {
			if(!IsCacheCreated) return;
			Cache.dict.Clear();
		}
		public Image GetImage(string assemblyName, string key, bool designMode) {
			return GetImageHolder(assemblyName, key, designMode).Image;
		}
		protected ImageValueHolder GetImageHolder(string assemblyName, string key, bool designMode) {
			Dictionary<string, ImageValueHolder> dictCore;
			if(!this.dict.TryGetValue(assemblyName, out dictCore)) {
				dictCore = LoadImages(assemblyName, designMode);
				this.dict[assemblyName] = dictCore;
			}
			if(!dictCore.ContainsKey(key)) return null;
			return dictCore[key];
		}
		[System.Security.SecuritySafeCritical]
		Dictionary<string, ImageValueHolder> LoadImages(string assemblyName, bool designMode) {
			Dictionary<string, ImageValueHolder> res = new Dictionary<string, ImageValueHolder>();
			Assembly asm = DesignTimeTools.LoadAssembly(assemblyName, designMode);
			foreach(string resourceName in ImageCollectionUtils.GetImageResourceNames(asm)) {
				res[resourceName] = new ImageValueHolder(new ImageValueLoader(asm, resourceName));
			}
			return res;
		}
	}
	public class ImageCollectionRegisteredAssemblies : IEnumerable<string>, IDisposable {
		Dictionary<int, string> assemblies;
		public ImageCollectionRegisteredAssemblies() {
			this.assemblies = new Dictionary<int, string>();
		}
		public void Register(string assemblyName) {
			if(this.assemblies.ContainsValue(assemblyName)) return;
			this.assemblies.Add(GetNextId(), assemblyName);
		}
		public string GetAssemblyName(int asmId) {
			if(!this.assemblies.ContainsKey(asmId)) {
				throw new ArgumentException(string.Format("ImageCollection: '{0}' assembly identifier isn't registered", asmId.ToString()));
			}
			return this.assemblies[asmId];
		}
		public bool IsEmpty { get { return this.assemblies.Count == 0; } }
		public int GetAssemblyId(string assemblyName) {
			foreach(KeyValuePair<int, string> pair in this.assemblies) {
				if(string.Equals(pair.Value, assemblyName, StringComparison.OrdinalIgnoreCase)) return pair.Key;
			}
			throw new ArgumentException(string.Format("ImageCollection: '{0}' assembly isn't registered", assemblyName));
		}
		public static ImageCollectionRegisteredAssemblies Create(ICollection<ImageInfo> images) {
			if(images == null || images.Count == 0) return null;
			ImageCollectionRegisteredAssemblies col = new ImageCollectionRegisteredAssemblies();
			foreach(ImageInfo obj in images) {
				SolutionImageInfo si = obj as SolutionImageInfo;
				if(si != null) col.Register(si.AssemblyName);
			}
			return col;
		}
		protected int GetNextId() {
			return this.assemblies.Count + 1;
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(this.assemblies != null) {
					this.assemblies.Clear();
				}
				this.assemblies = null;
			}
		}
		#region IEnumerable
		IEnumerator<string> IEnumerable<string>.GetEnumerator() {
			return GetAssemblies();	
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetAssemblies();
		}
		protected IEnumerator<string> GetAssemblies() {
			foreach(KeyValuePair<int, string> pair in this.assemblies) {
				yield return pair.Value;
			}
		}
		#endregion
	}
	public class ImageValueHolder : ValueHolder {
		public ImageValueHolder(IValueLoader loader)
			: base(loader) {
		}
		public Image Image { get { return (Image)base.Value; } }
	}
	public class ImageValueLoader : IValueLoader {
		string path;
		Assembly asm;
		public ImageValueLoader(Assembly asm, string path) {
			this.path = path;
			this.asm = asm;
		}
		public object Load() {
			return ImageCollectionUtils.GetImage(asm, path);
		}
	}
	public class ImageCollectionGetImageSizeEventArgs : EventArgs {
		Size dpi;
		SizeF scaleFactor;
		public ImageCollectionGetImageSizeEventArgs(Size dpi, SizeF scaleFactor) {
			this.dpi = dpi;
			this.scaleFactor = scaleFactor;
		}
		public void SetScaleFactor(float scaleFactorX, float scaleFactorY) {
			this.scaleFactor.Width = scaleFactorX;
			this.scaleFactor.Height = scaleFactorY;
		}
		public Size Dpi { get { return dpi; } }
		public SizeF ScaleFactor { get { return scaleFactor; } }
	}
	public delegate void ImageCollectionGetImageSizeEventHandler(object sender, ImageCollectionGetImageSizeEventArgs e);
	public class ImageCollectionResolveImageEventArgs : EventArgs {
		Size dpi;
		Image image;
		string resourceName;
		public ImageCollectionResolveImageEventArgs(string resourceName, Size dpi) {
			this.image = null;
			this.dpi = dpi;
			this.resourceName = resourceName;
		}
		public string ResourceName { get { return resourceName; } }
		public Size Dpi { get { return dpi; } }
		public Image Image { get { return image; } set { image = value; } }
	}
	public delegate void ImageCollectionResolveImageEventHandler(object sender, ImageCollectionResolveImageEventArgs e);
	public interface IImageResolver : IDisposable {
		Image ResolveImage(string name, Type resource);
	}
	public class ImageCollectionDpiAwareImageResolver : IImageResolver {
		ImageCollection owner;
		public ImageCollectionDpiAwareImageResolver(ImageCollection owner) {
			this.owner = owner;
		}
		public virtual Image ResolveImage(string name, Type resource) {
			List<PropertyInfo> list = new List<PropertyInfo>();
			foreach(PropertyInfo prop in resource.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
				if(AcceptProperty(name, GetFilter(), prop)) list.Add(prop);
			}
			if(list.Count != 1) return null;
			return (Image)list[0].GetValue(null, null);
		}
		public void Dispose() { Dispose(true); }
		protected virtual bool AcceptProperty(string baseName, string filter, PropertyInfo prop) {
			if(!ImageType.IsAssignableFrom(prop.PropertyType) || string.Equals(baseName, prop.Name, StringComparison.Ordinal)) return false;
			if(!prop.Name.StartsWith(baseName)) return false;
			string suffix = prop.Name.Substring(baseName.Length);
			if(string.IsNullOrEmpty(suffix) || IsSuffixHasIllegalChapters(suffix, filter)) return false;
			return suffix.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
		}
		protected virtual bool IsSuffixHasIllegalChapters(string suffix, string filter) {
			string pure = suffix.Replace(filter, string.Empty);
			for(int i = 0; i < pure.Length; i++) {
				if(char.IsLetterOrDigit(pure[i])) return true;
			}
			return false;
		}
		static readonly Type ImageType = typeof(Image);
		protected virtual string GetFilter() {
			return ScaleUtils.GetSystemDPI().Width.ToString() + "DPI";
		}
		protected virtual void Dispose(bool disposing) {
			this.owner = null;
		}
		protected ImageCollection Owner { get { return owner; } }
	}
	[ToolboxTabName(AssemblyInfo.DXTabComponents), DXToolboxItem(DXToolboxItemKind.Free)]
	[DesignerSerializer("DevExpress.Utils.Design.SharedImageCollectionCodeDomSerializer, " + AssemblyInfo.SRAssemblyDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	[Designer("DevExpress.Utils.Design.SharedImageCollectionDesigner, " + AssemblyInfo.SRAssemblyDesign, typeof(IDesigner))]
	[Description("An image collection that allows you to share images between controls within multiple forms.")]
	[ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "SharedImageCollection")]
	public class SharedImageCollection : Component, ISupportInitialize {
		[ThreadStatic]
		static int instanceCounter;
		[ThreadStatic]
		static ImageCollection instance;
		static object syncObject = new object();
		static SharedImageCollection() {
			AllowModifyingResources = true;
		}
		Control parentControl;
		ImageCollection localCollection;
		public SharedImageCollection() {
			this.parentControl = null;
			this.InitMode = false;
			lock(syncObject) {
				instanceCounter++;
				if(instance == null) instance = CreateInternalCollection();
			}
			this.localCollection = CreateInternalCollection();
			PreserveActualImageSize();
		}
		public SharedImageCollection(IContainer container) : this() {
			container.Add(this);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool HasChanges() {
			if(this.localCollection != null && instance != null) {
				if(this.localCollection.ImageSize != instance.ImageSize)
					return true;
				return !Images.IsEquals(instance.Images, this.localCollection.Images);
			}
			return true;
		}
		static Size actualImageSize = Size.Empty;
		protected virtual void PreserveActualImageSize() {
			if(InstanceCount < 2) return;
			actualImageSize = instance.ImageSize;
		}
		protected virtual void RestoreActualImageSize() {
			if(InstanceCount < 2) return;
			instance.ImageSize = actualImageSize;
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SharedImageCollectionImageSource"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageCollection ImageSource {
			get {
				if(IsSkipUpdateInternalCollection) return this.localCollection;
				return instance;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int InstanceCount {
			get {
				lock(syncObject) {
					return instanceCounter;
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Control ParentControl {
			get {
				if(DesignMode && Site != null) {
					IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
					return host != null ? host.RootComponent as Control : null;
				}
				return parentControl;
			}
			set { parentControl = value; }
		}
		protected virtual ImageCollection CreateInternalCollection() {
			ImageCollection res = new ImageCollection(false);
			res.Images.ForceMakeBitmapTransparent = true;
			return res;
		}
		protected bool InitMode {
			get;
			private set;
		}
		bool IsSkipUpdateInternalCollection {
			get {
				lock(syncObject) {
					return InitMode && instanceCounter > 1;
				}
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool AllowModifyingResources { get; set; }
		#region ISupportInitialize
		public void BeginInit() {
			InitMode = true;
		}
		public void EndInit() {
			if(DesignMode) RestoreActualImageSize(); 
			TryLoadActualParameters();
			InitMode = false;
		}
		#endregion
		#region
		public static readonly string TimestampResourceName = "SharedImageCollection.Timestamp";
		public static readonly string ImageSizeResourceName = "SharedImageCollection.ImageSize";
		protected virtual void TryLoadActualParameters() {
			if(DesignMode || ParentControl == null) return;
			SharedImageCollectionHashInfoCollection infos = new SharedImageCollectionHashInfoCollection();
			foreach(string resourceName in BinaryResources) {
				ResourceManager rm = new ResourceManager(resourceName, CurrentAssembly);
				if(SharedImageCollectionHashInfo.IsResourceValid(rm)) infos.Add(new SharedImageCollectionHashInfo(rm));
			}
			if(infos.Count < 2) return;
			infos.Sort();
			ApplyParameters(infos[0]);
		}
		protected virtual void ApplyParameters(SharedImageCollectionHashInfo info) {
			ImageSource.ImageSize = info.ImageSize;
		}
		protected IEnumerable<string> BinaryResources {
			get {
				const string suffix = ".resources";
				List<string> result = new List<string>();
				foreach(string resourceName in CurrentAssembly.GetManifestResourceNames()) {
					if(resourceName.EndsWith(suffix)) {
						string targetName = resourceName.Substring(0, resourceName.Length - suffix.Length);
						result.Add(targetName);
					}
				}
				return result;
			}
		}
		protected Assembly CurrentAssembly {
			get { return ParentControl.GetType().Assembly; }
		}
		#endregion
		#region SharedImageCollectionHashInfo
		protected class SharedImageCollectionHashInfo : IComparable {
			Size imageSize;
			DateTime timestamp;
			public SharedImageCollectionHashInfo(ResourceManager rm) {
				Initialize(rm);
			}
			protected virtual void Initialize(ResourceManager rm) {
				this.timestamp = (DateTime)rm.GetObject(SharedImageCollection.TimestampResourceName);
				imageSize = (Size)rm.GetObject(SharedImageCollection.ImageSizeResourceName);
			}
			public static bool IsResourceValid(ResourceManager rm) {
				if(!TryGetResource(rm, SharedImageCollection.TimestampResourceName)) return false;
				if(!TryGetResource(rm, SharedImageCollection.ImageSizeResourceName)) return false;
				return true;
			}
			static bool TryGetResource(ResourceManager rm, string element) {
				bool res;
				try {
					res = (rm.GetObject(element, CultureInfo.InvariantCulture) != null);
				}
				catch {
					res = false;
				}
				return res;
			}
			#region IComparable
			public int CompareTo(object obj) {
				SharedImageCollectionHashInfo info = obj as SharedImageCollectionHashInfo;
				if(info == null) throw new ArgumentException();
				return -1 * Timestamp.CompareTo(info.Timestamp);
			}
			#endregion
			public Size ImageSize { get { return imageSize; } }
			public DateTime Timestamp { get { return timestamp; } }
		}
		protected class SharedImageCollectionHashInfoCollection : Collection<SharedImageCollectionHashInfo> {
			public void Sort() {
				ListCore.Sort();
			}
			protected List<SharedImageCollectionHashInfo> ListCore { get { return base.Items as List<SharedImageCollectionHashInfo>; } }
		}
		#endregion
		#region Disposing
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.localCollection != null)
					this.localCollection.Dispose();
				this.localCollection = null;
				lock(syncObject) {
					instanceCounter--;
					if(instanceCounter == 0) {
						instance.Dispose();
						instance = null;
					}
				}
			}
			base.Dispose(disposing);
		}
		#endregion
	}
	#region Enum for natigation controls
	public enum SharedImageCollectionImageSizeMode { UseCollectionImageSize, UseImageSize }
	#endregion
	[Serializable, TypeConverter(typeof(DevExpress.Utils.Design.BinaryTypeConverter))]
	public class ImageCollectionStreamer : ISerializable {
		ImageCollection collection = null;
		static ImageCollectionStreamer() {
			DevExpress.Utils.Design.DXAssemblyResolverEx.Init();
		}
		public ImageCollectionStreamer(ImageCollection collection) {
			this.collection = collection;
		}
		ImageCollectionStreamer(SerializationInfo si, StreamingContext context) {
			if(si.MemberCount == 0) return;
			byte[] data = si.GetValue("Data", typeof(object)) as byte[];
			if(data == null) return;
			Size size = (Size)si.GetValue("ImageSize", typeof(Size));
			this.collection = new ImageCollection();
			this.collection.ImageSize = size;
			int position = 0;
			while(position < data.Length) {
				int imSize = GetSize(data, position);
				position += 4;
				if(imSize == 0) break;
				byte[] imdata = new byte[imSize];
				Array.Copy(data, position, imdata, 0, imSize);
				this.collection.Images.AddCore(imdata);
				position += imSize;
			}
		}
		static ImageFormat GetFormat(Image image) {
			ImageFormat res = image.RawFormat;
			if(res == ImageFormat.Jpeg || res == ImageFormat.MemoryBmp)
				res = ImageFormat.Png;
			return res;
		}
		static ImageCodecInfo FindCodec(ImageFormat format) {
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			foreach(ImageCodecInfo codec in codecs) {
				if(codec.FormatID.Equals(format.Guid))
					return codec;
			}
			return null;
		}
		public static ImageCodecInfo GetCodec(Image image) {
			ImageFormat format = GetFormat(image);
			ImageCodecInfo codec = FindCodec(format);
			if(codec == null) codec = FindCodec(ImageFormat.Png);
			return codec;
		}
		public ImageCollection Collection { get { return collection; } }
		[System.Security.SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context) {
			if(collection == null || collection.Images.Count == 0) return;
			using(MemoryStream stream = GetSerializationStream()) {
				si.AddValue("ImageSize", collection.ImageSize);
				si.AddValue("Data", stream.ToArray());
			}
		}
		MemoryStream GetSerializationStream() {
			MemoryStream stream = new MemoryStream();
			for(int i = 0; i < collection.Images.Count; i++) {
				ImageInfo imageInfo = collection.Images.InnerImages[i];
				if(imageInfo.ShouldSerialize) {
					Image image = collection.Images[i];
					MemoryStream tempStream = new MemoryStream();
					image.Save(tempStream, GetCodec(image), null);
					tempStream.Seek(0, SeekOrigin.Begin);
					byte[] size = GetSize((int)tempStream.Length);
					stream.Write(size, 0, size.Length);
					tempStream.WriteTo(stream);
					tempStream.Close();
				}
			}
			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}
		byte[] GetSize(int size) {
			byte[] res = new byte[4];
			res[0] = (byte)(size & 0xff);
			res[1] = (byte)((size >> 8) & 0xff);
			res[2] = (byte)((size >> 16) & 0xff);
			res[3] = (byte)((size >> 24) & 0xff);
			return res;
		}
		int GetSize(byte[] size, int position) {
			return size[position + 0] | (size[position + 1] << 8) | (size[position + 2] << 16) | (size[position + 3] << 24);
		}
	}
}
