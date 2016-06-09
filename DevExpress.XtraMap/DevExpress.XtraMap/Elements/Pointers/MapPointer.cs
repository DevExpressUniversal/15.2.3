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

using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Utils;
using DevExpress.Utils.Text.Internal;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
namespace DevExpress.XtraMap {
	[Flags]
	public enum ElementState {
		None = 0,
		Normal = 1,
		Highlighted = 2,
		Selected = 4,
		All = Normal | Highlighted | Selected
	}
	public enum TextAlignment {
		TopLeft = 1,
		TopCenter = 2,
		TopRight = 4,
		MiddleLeft = 16,
		MiddleRight = 64,
		BottomLeft = 256,
		BottomCenter = 512,
		BottomRight = 1024
	}
	public abstract class MapPointer : MapItem, IImageContainer, ILocatableRenderItem, ISupportCoordLocation, IImageUriLoaderClient, IMapAnimatableItem, ISupportImagePainter, IClusterable, IClusterItem, IPointCore, IMapPointerStyleCore {
		const bool DefaultEnableAnimation = true;
		public const TextAlignment DefaultTextAlignment = TextAlignment.MiddleRight;
		public const int DefaultTextPadding = 6;
		internal const int DefaultImageIndex = -1;
		IList<IClusterable> clusteredItems = new List<IClusterable>();
		CoordPoint location;
		MapPoint renderOrigin = TextImageItemPainterBase.DefaultRenderOrigin;
		Uri imageUri;
		Image image;
		Image loadedImage;
		int imageIndex = DefaultImageIndex;
		Size imageSize = Size.Empty;
		object actualImageList;
		IImageTransform imageTransform;
		bool enableAnimation = DefaultEnableAnimation;
		MapAnimation mapAnimation = null;
		CoordPoint anchorLocation;
		string text = string.Empty;
		bool allowUseAntiAliasing = true;
		MapPoint imageOrigin;
		ElementState skinBackgroundVisibility = ElementState.All;
		TextAlignment textAlignment = DefaultTextAlignment;
		int textPadding = DefaultTextPadding;
		DevExpress.Utils.Text.StringInfo htmlStringInfo;
		TextImageItemPainterBase currentPainter = null;
		byte transparency = ImageGeometry.DefaultTransparency;
		MapUnit? unitLocation = null;
		protected virtual internal bool AllowHtmlTextCore { get { return false; } }
		protected internal DevExpress.Utils.Text.StringInfo HtmlStringInfo { get { return htmlStringInfo; } }
		protected bool HasHyperlink { get { return HtmlStringInfo != null ? HtmlStringInfo.HasHyperlink : false; } }
		protected TextImageItemPainterBase Painter { get { return currentPainter; } }
		protected override bool AllowUseAntiAliasing { get { return allowUseAntiAliasing; } }
		protected virtual ElementState DefaultSkinBackgroundState { get { return ElementState.All; } }
		internal LayerColoredSkinElementCache LayerColoredSkinElementCache {
			get {
				if(Layer != null)
					return Layer.ColoredSkinElementCache;
				return null;
			}
		}
		internal IImageTransform ImageTransform { get { return imageTransform; } set { imageTransform = value; } }
		protected internal override GeometryType GeometryType { get { return GeometryType.Screen; } }
		protected internal Image LoadedImage { get { return loadedImage; } }
		protected internal MapAnimation MapAnimation {
			get {
				if(mapAnimation == null)
					mapAnimation = new MapAnimation(this);
				return mapAnimation;
			}
		}
		protected internal CoordPoint CurrentLocation {
			get {
				return EnableAnimation ? CalculateActualLocation(Location) : Location;
			}
		}
		protected internal ElementState SkinBackgroundVisibility {
			get { return skinBackgroundVisibility; }
			set {
				if(skinBackgroundVisibility == value)
					return;
				skinBackgroundVisibility = value;
				OnBackgroundVisibilityChanged();
			}
		}
		protected object ActualImageList { get { return actualImageList; } }
		protected int ActualImageIndex { get { return Layer != null && imageIndex == DefaultImageIndex ? Layer.ItemImageIndex : imageIndex; } }
		protected internal MapPoint ImageOrigin { get { return imageOrigin; } set { imageOrigin = value; } }
		protected internal new ImageGeometry Geometry { get { return base.Geometry as ImageGeometry; } }
		protected Size ImageSize { get { return imageSize; } set { imageSize = value; } }
		protected internal bool EnableAnimation {
			get {
				return Layer != null && Layer.Map != null && UseAnimation;
			}
		}
		protected internal MapPoint RenderOrigin {
			get { return renderOrigin; }
			set {
				if(renderOrigin == value)
					return;
				renderOrigin = value;
				OnRenderOriginChagned();
			}
		}
		protected internal Color TextGlowColor {
			get { return IsStyleEmpty ? Color.Empty : Style.TextGlowColor; }
			set {
				EnsureStyle();
				Style.TextGlowColor = value;
			}
		}
		protected override bool NeedRecreateGeometry { get { return UpdateType != MapItemUpdateType.Location && UpdateType != MapItemUpdateType.None; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPointerTextAlignment"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(DefaultTextAlignment),
		Editor("DevExpress.XtraMap.Design.LegendAlignmentEditor," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign, typeof(UITypeEditor))
		]
		public TextAlignment TextAlignment {
			get { return textAlignment; }
			set {
				if(textAlignment == value)
					return;
				textAlignment = value;
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPointerTextPadding"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(DefaultTextPadding)]
		public int TextPadding {
			get { return textPadding; }
			set {
				if(textPadding == value)
					return;
				textPadding = value < 0 ? MapPointer.DefaultTextPadding : value;
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPointerLocation"),
#endif
		Category(SRCategoryNames.Data),
		TypeConverter("DevExpress.XtraMap.Design.CoordPointTypeConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign)
		]
		public CoordPoint Location {
			get { return location; }
			set {
				SetLocationInternal(value, EnableAnimation);
			}
		}
		void ResetLocation() { Location = UnitConverter.PointFactory.CreatePoint(0, 0); }
		bool ShouldSerializeLocation() { return Location != UnitConverter.PointFactory.CreatePoint(0, 0); }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPointerText"),
#endif
		Category(SRCategoryNames.Data), DefaultValue(""), Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public string Text {
			get { return text; }
			set {
				if(text == value)
					return;
				text = value;
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPointerFont"),
#endif
		Category(SRCategoryNames.Appearance)]
		public Font Font {
			get { return IsStyleEmpty ? MapItemStyleProvider.DefaultFont : Style.Font; }
			set {
				EnsureStyle();
				Style.Font = value;
			}
		}
		void ResetFont() { if(!IsStyleEmpty) Font = MapItemStyleProvider.DefaultFont; }
		bool ShouldSerializeFont() { return !IsStyleEmpty && Font != MapItemStyleProvider.DefaultFont; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPointerTextColor"),
#endif
		Category(SRCategoryNames.Appearance)]
		public Color TextColor {
			get { return IsStyleEmpty ? Color.Empty : Style.TextColor; }
			set {
				EnsureStyle();
				Style.TextColor = value;
			}
		}
		bool ShouldSerializeTextColor() { return !IsStyleEmpty && TextColor != Color.Empty; }
		void ResetTextColor() { if(!IsStyleEmpty) TextColor = Color.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPointerUseAnimation"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(DefaultEnableAnimation)]
		public bool UseAnimation {
			get { return enableAnimation; }
			set {
				if(enableAnimation == value)
					return;
				enableAnimation = value;
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPointerImageUri"),
#endif
		Category(SRCategoryNames.Data), DefaultValue(null)]
		public Uri ImageUri {
			get { return imageUri; }
			set {
				if(imageUri == value)
					return;
				imageUri = value;
				OnImageUriChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPointerImage"),
#endif
		Category(SRCategoryNames.Data), DefaultValue(null)]
		public Image Image {
			get { return image; }
			set {
				if(Object.Equals(image, value))
					return;
				image = value;
				OnImagePropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapPointerImageIndex"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(DefaultImageIndex),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)), ImageList("ImageList")]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(imageIndex == value)
					return;
				imageIndex = value;
				OnImagePropertiesChanged();
			}
		}
		[Category(SRCategoryNames.Appearance), DefaultValue(ImageGeometry.DefaultTransparency)]
		public byte Transparency {
			get { return transparency; }
			set {
				if(transparency == value)
					return;
				transparency = value;
				InvalidateRender();
			}
		}
		protected MapPointer() {
			this.location = UnitConverter.PointFactory.CreatePoint(0, 0);
			this.anchorLocation = this.location;
			this.skinBackgroundVisibility = DefaultSkinBackgroundState;
		}
		protected Point CalculateLeftTopInPixels() {
			Point pixelLocation = UnitConverter.CoordPointToScreenPoint(CurrentLocation).ToPoint();
			Point offset = GetLeftTopPointOffset();
			return new Point(pixelLocation.X - offset.X, pixelLocation.Y - ImageSize.Height + offset.Y);
		}
		protected virtual Point GetLeftTopPointOffset() {
			return new Point(ImageSize.Width / 2, ImageSize.Height / 2);
		}
		protected override string GetTextCore() {
			return Text;
		}
		protected override Color GetTextColorCore() {
			return TextColor;
		}
		void OnImageUriChanged() {
			if(ImageUri == null) {
				this.loadedImage = null; 
			}
			else
				ImageCacheContainer.QueryImage(ImageUri, this);
		}
		protected internal override void OnLayerInitialized(MapItemsLayerBase itemsLayer) {
			base.OnLayerInitialized(itemsLayer);
			if(Layer.Map == null)
				return;
			MapUtils.UpdateImageContainer(this, Layer.Map.ImageList);
		}
		void IImageContainer.UpdateImage(object imageList) {
			this.actualImageList = imageList;
			UpdateItem(MapItemUpdateType.Layout);
		}
		void IImageUriLoaderClient.OnImageLoaded(Image image) {
			OnImageLoaded(image);
		}
		void OnImageLoaded(Image image) {
			this.loadedImage = image;
			if(ImageTransform != null && image != null) {
				double originX = 0; double originY = 0;
				Size imageSize = ImageSafeAccess.GetSize(image);
				ImageTransform.CalcImageOrigin(imageSize.Width, imageSize.Height, out originX, out originY); ;
				RenderOrigin = new MapPoint(originX, originY);
			}
			OnImagePropertiesChanged();
		}
		void SetLocationInternal(CoordPoint value, bool enableAnimation) {
			if(location == value)
				return;
			if(!CoordinateSystemHelper.IsNumericCoordPoint(value))
				Exceptions.ThrowIncorrectCoordPointException();
			anchorLocation = CalculateActualLocation(Location);
			if(enableAnimation)
				MapAnimation.Start();
			else
				anchorLocation = value;
			location = value;
			OnLocationChanged();
		}
		protected virtual void OnLocationChanged() {
			UpdateBoundingRect();
			UpdateItem(MapItemUpdateType.Location);
		}
		CoordPoint CalculateActualLocation(CoordPoint location) {
			double xOffset = (location.GetX() - this.anchorLocation.GetX()) * MapAnimation.Progress;
			double yOffset = (location.GetY() - this.anchorLocation.GetY()) * MapAnimation.Progress;
			return UnitConverter.PointFactory.CreatePoint(anchorLocation.GetX() + xOffset, anchorLocation.GetY() + yOffset);
		}
		Image GetActualImageCore() {
			Image defaultImage = GetDefaultImage();
			if(loadedImage != null) return loadedImage;
			if(Image != null) return Image;
			if(ActualImageIndex == DefaultImageIndex || actualImageList == null)
				return defaultImage;
			return ImageCollection.GetImageListImage(actualImageList, ActualImageIndex);
		}
		protected virtual Image GetDefaultImage() {
			return null;
		}
		protected virtual void OnBackgroundVisibilityChanged() {
			RegisterLayoutUpdate();
			InvalidateRender();
		}
		protected void OnImagePropertiesChanged() {
			UpdateItem(MapItemUpdateType.Layout);
		}
		protected internal override void ApplyAppearance() {
			base.ApplyAppearance();
			if(Layer != null)
				Layer.UpdateImageHolder(this);
		}
		protected virtual void OnRenderOriginChagned() {
			UpdateImageOrigin();
			InvalidateRender();
		}
		protected virtual void UpdateImageOrigin() {
		}
		protected virtual void OnTextChanged() {
		}
		protected override void OnStyleReset() {
			RegisterLayoutUpdate();
			base.OnStyleReset();
		}
		protected override IHitTestGeometry CreateHitTestGeometry() {
			MapPoint pixelLocation = UnitConverter.CoordPointToScreenPoint(Location);
			return new RectangleScreenHitTestGeometry(pixelLocation, ImageSize, ImageOrigin);
		}
		protected override IRenderItemStyle GetStyle() {
			return MapItemStyleCache.ReadOnlyPointerStyle;
		}
		protected override void AfterDrawMapItemEvent(IRenderItemStyle style) {
			DrawMapPointerEventArgs args = style as DrawMapPointerEventArgs;
			if(args != null)
				CustomizeGeometry(args);
		}
		protected void CustomizeGeometry(DrawMapPointerEventArgs args) {
			CustomizeGeometryCore(args.Image, args.Text, args.Transparency, args.DisposeImage);
		}
		protected override MapRect GetHitTestUnitBounds() {
			return Bounds;
		}
		protected override IMapItemGeometry CreateGeometry() {
			return new ImageGeometry();
		}
		void CustomizeGeometryCore(Image image, string text, byte transparency, bool disposeImage) {
			lock(UpdateLocker) {
				ResetPainter();
				PrepareImageGeometry(Geometry, image, text, transparency, false, disposeImage);
				RegisterHitTestableItem();
			}
		}
		protected void ResetPainter() {
			if(this.currentPainter != null)
				this.currentPainter.ResetFinalImage();
			this.currentPainter = null;
		}
		protected override void PrepareImageGeometry() {
			PrepareImageGeometry(Geometry, GetActualImage(), Text, Transparency, true, false);
		}
		bool ShouldRecreateImagePainter(MapItemUpdateType updateType) {
			return updateType != MapItemUpdateType.None && updateType != MapItemUpdateType.Location;
		}
		protected void PrepareImageGeometry(ImageGeometry geometry, Image image, string text, byte transparency, bool storingInPool, bool disposeImage) {
			PrepareGeometryCore(geometry, image, text, transparency, storingInPool);
			if(this.currentPainter != null && disposeImage)
				this.currentPainter.ClearSourceImage();
		}
		void PrepareGeometryCore(ImageGeometry geometry, Image image, string text, byte transparency, bool storingInPool) {
			PreparePainterIfNecessary(image, text);
			InitializeGeometry(geometry, transparency, storingInPool);
		}
		void InitializeGeometry(ImageGeometry geometry, byte transparency, bool storingInPool) {
			this.allowUseAntiAliasing = this.currentPainter.AllowUseAntiAliasing;
			geometry.SetImage(this.currentPainter.FinalImage, this.currentPainter.IsExternalImage);
			geometry.ImageRect = this.currentPainter.FinalImageRect;
			geometry.StoringInPool = storingInPool;
			geometry.Transparency = transparency;
			ImageSize = this.currentPainter.FinalImageRect.Size;
		}
		void PreparePainterIfNecessary(Image image, string text) {
			if(this.currentPainter == null || ShouldRecreateImagePainter(UpdateType)) {
				this.currentPainter = CreatePainter();
				this.currentPainter.SourceImage = image;
				this.currentPainter.SourceText = text;
				this.currentPainter.Draw(Layer.Map);
			}
		}
		protected override MapRect CalculateBounds() {
			MapUnit locationUnit = (Layer != null) ? UnitConverter.CoordPointToMapUnit(Location, false) : new MapUnit();
			return new MapRect(locationUnit.X, locationUnit.Y, 0, 0);
		}
		protected override CoordBounds CalculateNativeBounds() {
			return new CoordBounds(Location, Location);
		}
		protected override void SetOwner(object value) {
			base.SetOwner(value);
			if(value == null) {
				ImageGeometry geometry = Geometry as ImageGeometry;
				if(geometry != null) {
					geometry.Dispose();
					geometry = null;
				}
			}
		}
		protected internal Image GetActualImage() {
			return GetActualImageCore();
		}
		protected abstract TextImageItemPainterBase CreatePainter();
		protected abstract IClusterItem CreateInstance();
		#region ILocatableRenderItem Members
		MapUnit ILocatableRenderItem.Location {
			get { return UnitConverter.CoordPointToMapUnit(this.CurrentLocation, true) * MapShape.RenderScaleFactor; }
		}
		Size ILocatableRenderItem.SizeInPixels {
			get { return imageSize; }
		}
		MapPoint ILocatableRenderItem.Origin {
			get { return ImageOrigin; }
		}
		MapPoint ILocatableRenderItem.StretchFactor {
			get { return new MapPoint(1.0, 1.0); }
		}
		void ILocatableRenderItem.ResetLocation() {
			this.unitLocation = null;
		}
		MapUnit GetUnitLocation() {
			if(!this.unitLocation.HasValue)
				this.unitLocation = UnitConverter.CoordPointToMapUnit(location, true);
			return this.unitLocation.Value;
		}
		#endregion
		#region IMapAnimatableItem Members
		bool IMapAnimatableItem.EnableAnimation {
			get { return EnableAnimation; }
		}
		void IMapAnimatableItem.FrameChanged(object sender, AnimationAction action, double progress) {
			if(action == AnimationAction.Complete)
				anchorLocation = Location;
			InvalidateRender();
		}
		#endregion
		#region ISupportImagePainter members
		MapItemStyle ISupportImagePainter.ActualStyle { get { return ActualStyle; } }
		ImageGeometry ISupportImagePainter.Geometry { get { return Geometry; } }
		#endregion
		#region IClusterItemCore members
		CoordPoint IClusterItemCore.Location {
			get { return this.location; }
			set {  SetLocationInternal(value, false);
			}
		}
		IMapUnit IClusterItemCore.GetUnitLocation() {
			return GetUnitLocation();
		}
		object IClusterItemCore.Owner {
			get { return Owner; }
			set {
				if (Owner == value)
					return;
				MapUtils.SetOwner(this, value);
			}
		}
	#endregion
		#region IClusterable members
		IClusterItem IClusterable.CreateInstance() { return CreateInstance(); }
		#endregion
		#region IClusterItem members
		IList<IClusterable> IClusterItem.ClusteredItems { get { return clusteredItems; } set { clusteredItems = value; } }
		void IClusterItem.ApplySize(double size) { }
		#endregion
		#region IMapPointerStyleCore members
		Image IMapPointerStyleCore.Image {
			get { return GetActualImage(); }
		}
		#endregion
		protected internal override DrawMapItemEventArgs CreateDrawEventArgs() {
			return new DrawMapPointerEventArgs(this) { Image = GetActualImage(), Text = Text, Transparency = Transparency };
		}
		protected override void RegisterHitTestableItem() {
			base.RegisterHitTestableItem();
			ReleaseHitTestGeometry();
		}
		internal void UpdateHtmlStringInfo() {
			if(!AllowHtmlTextCore)
				return;
			using(AppearanceObject app = (AppearanceObject)AppearanceObject.EmptyAppearance.Clone()) {
				app.ForeColor = ActualStyle.TextColor;
				app.BackColor = ActualStyle.Fill;
				this.htmlStringInfo = MapUtils.CalcHtmlStringInfo(Text, app);
			}
		}
		StringBlock FindHyperlinkBlockByMousePosition(Point point) {
			Point leftTop = CalculateLeftTopInPixels();
			Point relativePos = new Point(point.X - leftTop.X, point.Y - leftTop.Y);
			return HtmlStringInfo.GetLinkByPoint(relativePos);
		}
		protected internal HyperlinkClickEventArgs TryClickHyperlink(MouseEventArgs e) {
			if(!HasHyperlink || Control.ModifierKeys != Keys.Control)
				return null;
			StringBlock block = FindHyperlinkBlockByMousePosition(e.Location);
			return block != null ? new HyperlinkClickEventArgs() { Text = block.Text, Link = block.Link } : null;
		}
		protected Cursor PrevCursor { get; set; }
		protected internal override void HandleMapItemMouseMove(MouseEventArgs e) {
			if(Layer == null || Layer.Map == null)
				return;
			if(HasHyperlink && Control.ModifierKeys == Keys.Control) {
				StringBlock block = FindHyperlinkBlockByMousePosition(e.Location);
				if(block != null) {
					if(PrevCursor == null) PrevCursor = Layer.Map.Cursor;
					UpdateMapCursor(Cursors.Hand);
				}
				else {
					if(PrevCursor != null)
						RestoreMapCursor();
				}
			}
			else
				RestoreMapCursor();
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			return new CoordPoint[] { Location };
		}
		void RestoreMapCursor() {
			if(PrevCursor != null)
				UpdateMapCursor(PrevCursor);
			PrevCursor = null;
		}
		void UpdateMapCursor(Cursor cursor) {
			Layer.Map.Cursor = cursor;
		}
	}
}
