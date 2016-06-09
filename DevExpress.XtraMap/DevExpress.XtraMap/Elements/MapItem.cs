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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Utils;
namespace DevExpress.XtraMap {
	public abstract class MapItem : IOwnedElement, IHitTestableElement, IRenderItem, IInteractiveElement, IMapStyleOwner, IMapItemCore, IMapDataItem, ISupportToolTip,
									IMapItemAttributeOwner, ILockableObject {
		const bool DefaultHitTestVisible = true;
		const bool DefaultVisible = true;
		internal const int UndefinedRowIndex = -1;
		protected internal const double RenderScaleFactor = 10000;
		readonly MapItemAttributeCollection attributes;
		readonly NotificationCollectionChangedListener<MapItemAttribute> attributeListener;
		MapItemStyle style;
		MapItemStyle highlightedStyle;
		MapItemStyle selectedStyle;
		readonly MapItemStyle actualStyle = new MapItemStyle();
		int lockUpdate = 0;
		int rowIndex = UndefinedRowIndex;
		int[]  listSourceRowIndices = new int[0];
		bool visible = DefaultVisible;
		bool isHighlighted = false;
		bool isSelected = false;
		bool isHitTestVisible = DefaultHitTestVisible;
		string toolTipPattern = String.Empty;
		object owner;
		IMapItemGeometry geometry;
		IHitTestGeometry hitTestGeometry;
		IRenderItemResourceHolder resourceHolder;
		MapRect bounds = MapRect.Empty;
		CoordBounds nativeBounds = CoordBounds.Empty;
		RegionRange regionRange = RegionRange.Empty;
		object updateLocker = new object();
		bool forceUpdateResourceHolder = false;
		HitTestKey hitTestKey;
		StyleMergerBase styleMerger;
		MapItemUpdateType updateType;
		Color colorizerColor = Color.Empty;
#if DEBUGTEST
		protected internal virtual MapItemType ItemType { get { return MapItemType.Unknown; } }
#endif
		protected object Owner { get { return ((IOwnedElement)this).Owner; } }
		protected IMapView ViewportSupport { get { return Layer != null ? Layer.View : null; } }
		protected MapRect RenderBounds { get { return ViewportSupport != null ? ViewportSupport.RenderBounds : MapRect.Empty; } }
		protected internal MapUnitConverter UnitConverter {
			get {
				return Layer != null ? Layer.UnitConverter : EmptyUnitConverter.Instance;
			}
		}
		protected virtual bool AllowUseAntiAliasing { get { return true; } }
		protected internal object UpdateLocker { get { return updateLocker; } }
		protected internal abstract GeometryType GeometryType { get; }
		protected internal bool IsSelected { get { return isSelected; } }
		protected internal bool IsHighlighted { get { return isHighlighted; } }
		protected internal MapItemUpdateType UpdateType { get { return updateType; } }
		protected Color ColorizerColor {
			get { return colorizerColor; }
			set {
				if (colorizerColor == value)
					return;
				colorizerColor = value;
				ColorizerColorChanged(colorizerColor);
			}
		}
		protected internal virtual bool ShouldUseColorizerColor { get { return !MapUtils.IsColorEmpty(ColorizerColor); } }
		protected internal IMapItemStyleProvider DefaultStyleProvider {
			get {
				IMapItemStyleProvider provider = Layer != null ? Layer.DefaultItemStyleProvider : null;
				return provider != null ? provider : MapItemStyleProvider.Default;
			}
		}
		protected internal MapItemStyle Style { get { return style; } }
		protected internal MapItemStyle HighlightedStyle { get { return highlightedStyle; } }
		protected internal MapItemStyle SelectedStyle { get { return selectedStyle; } }
		protected internal bool IsStyleEmpty { get { return style == null; } }
		protected internal bool IsSelectedStyleEmpty { get { return selectedStyle == null; } }
		protected internal bool IsHighlightedStyleEmpty { get { return highlightedStyle == null; } }
		protected StyleMergerBase StyleMerger {
			get {
				if(styleMerger == null)
					this.styleMerger = CreateStyleMerger();
				return styleMerger;
			}
		}
		protected virtual StyleMergerBase CreateStyleMerger() { 
			return new MapItemStyleMerger(this); 
		}
		protected void EnsureStyle() { 
			if(IsStyleEmpty) { 
				style = new MapItemStyle();
				MapUtils.SetOwner(style, this);
			} 
		}
		protected void EnsureHighlightedStyle() {
			if(IsHighlightedStyleEmpty) {
				highlightedStyle = new MapItemStyle();
				MapUtils.SetOwner(highlightedStyle, this);
			}
		}
		protected void EnsureSelectedStyle() {
			if(IsSelectedStyleEmpty) {
				selectedStyle = new MapItemStyle();
				MapUtils.SetOwner(selectedStyle, this);
			}
		}
		#region Style properties
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemFill"),
#endif
		Category(SRCategoryNames.Appearance)]
		public Color Fill {
			get { return IsStyleEmpty ? Color.Empty : Style.Fill; }
			set {
				EnsureStyle();
				Style.Fill = value; 
			}
		}
		void ResetFill() { if(!IsStyleEmpty) Fill = Color.Empty; }
		protected bool ShouldSerializeFill() { return !IsStyleEmpty && Fill != Color.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemStroke"),
#endif
		Category(SRCategoryNames.Appearance)]
		public virtual Color Stroke {
			get { return IsStyleEmpty ? Color.Empty : Style.Stroke; }
			set {
				EnsureStyle();
				Style.Stroke = value; 
			}
		}
		void ResetStroke() { if(!IsStyleEmpty) Stroke = Color.Empty; }
		protected bool ShouldSerializeStroke() { return !IsStyleEmpty && Stroke != Color.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemStrokeWidth"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(MapItemStyle.EmptyStrokeWidth)]
		public virtual int StrokeWidth {
			get { return IsStyleEmpty ? MapItemStyle.EmptyStrokeWidth : Style.StrokeWidth; }
			set {
				EnsureStyle();
				if(Style.StrokeWidth == value)
					return;
				Style.StrokeWidth = value;
				OnStrokeWidthChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemHighlightedFill"),
#endif
		Category(SRCategoryNames.Appearance)]
		public Color HighlightedFill {
			get { return IsHighlightedStyleEmpty ? Color.Empty : HighlightedStyle.Fill; }
			set {
				EnsureHighlightedStyle();
				HighlightedStyle.Fill = value; }
		}
		void ResetHighlightedFill() { if(!IsHighlightedStyleEmpty) HighlightedFill = Color.Empty; }
		protected bool ShouldSerializeHighlightedFill() { return !IsHighlightedStyleEmpty && HighlightedFill != Color.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemHighlightedStroke"),
#endif
		Category(SRCategoryNames.Appearance)]
		public Color HighlightedStroke {
			get { return IsHighlightedStyleEmpty ? Color.Empty : HighlightedStyle.Stroke; }
			set {
				EnsureHighlightedStyle();
				HighlightedStyle.Stroke = value; 
			}
		}
		void ResetHighlightedStroke() { if(!IsHighlightedStyleEmpty) HighlightedStroke = Color.Empty; }
		protected bool ShouldSerializeHighlightedStroke() { return !IsHighlightedStyleEmpty && HighlightedStroke != Color.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemHighlightedStrokeWidth"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(MapItemStyle.EmptyStrokeWidth)]
		public int HighlightedStrokeWidth {
			get { return IsHighlightedStyleEmpty ? MapItemStyle.EmptyStrokeWidth : HighlightedStyle.StrokeWidth; }
			set {
				EnsureHighlightedStyle();
				if(HighlightedStyle.StrokeWidth == value)
					return;
				HighlightedStyle.StrokeWidth = value;
				OnStrokeWidthChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemSelectedFill"),
#endif
		Category(SRCategoryNames.Appearance)]
		public Color SelectedFill {
			get { return IsSelectedStyleEmpty ? Color.Empty : SelectedStyle.Fill; }
			set {
				EnsureSelectedStyle();
				SelectedStyle.Fill = value; 
			}
		}
		void ResetSelectedFill() { if(!IsSelectedStyleEmpty) SelectedFill = Color.Empty; }
		protected bool ShouldSerializeSelectedFill() { return !IsSelectedStyleEmpty && SelectedFill != Color.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemSelectedStroke"),
#endif
		Category(SRCategoryNames.Appearance)]
		public Color SelectedStroke {
			get { return IsSelectedStyleEmpty ? Color.Empty : SelectedStyle.Stroke; }
			set {
				EnsureSelectedStyle();
				SelectedStyle.Stroke = value; 
			}
		}
		void ResetSelectedStroke() { if(!IsSelectedStyleEmpty) SelectedStroke = Color.Empty; }
		protected bool ShouldSerializeSelectedStroke() { return !IsSelectedStyleEmpty && SelectedStroke != Color.Empty; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemSelectedStrokeWidth"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(MapItemStyle.EmptyStrokeWidth)]
		public int SelectedStrokeWidth {
			get { return IsSelectedStyleEmpty ? MapItemStyle.EmptyStrokeWidth : SelectedStyle.StrokeWidth; }
			set {
				EnsureSelectedStyle();
				if(SelectedStyle.StrokeWidth == value)
					return;
				SelectedStyle.StrokeWidth = value;
				OnStrokeWidthChanged();
			}
		}
		#endregion
		protected virtual HitTestKey HitTestKey { get { return hitTestKey; } set { hitTestKey = value; } }
		protected internal virtual IMapItemGeometry Geometry { get { return geometry; } }
		protected IHitTestableRegistrator HitTestableRegistrator { get { return Layer as IHitTestableRegistrator; } }
		protected internal virtual bool ShouldRecreateHitTestGeometry {
			get {
				if(hitTestGeometry != null && Layer.Map != null && Layer.Map.Capture)
					return false;
				return hitTestGeometry == null;
			}
		}
		protected virtual bool EnableHighlighting { get { return Layer != null ? Layer.EnableHighlighting : false; } }
		protected virtual bool EnableSelection { get { return true; } }
		protected internal virtual void ApplyTitleOptions() {
		}
		protected internal int RowIndex { get { return rowIndex; } set { rowIndex = value; } }
		protected int[]  ListSourceRowIndices { get { return listSourceRowIndices; } 
			set { 
				int[] indices = value != null ? value : new int[0];
				listSourceRowIndices = indices;
			}
		}
		protected internal string ActualToolTipPattern { get { return (!string.IsNullOrEmpty(ToolTipPattern) || Layer == null) ? ToolTipPattern : Layer.ToolTipPattern; } }
		internal MapItemStyle ActualStyle { get { return actualStyle; } }
		internal bool GeometryIsValid { get { return UpdateType == MapItemUpdateType.None; } }
		protected virtual bool NeedRecreateGeometry { get { return true; } }
		protected internal virtual bool ActualVisible { get { return Visible && Layer != null && Layer.CheckVisibility(); } }
		[Browsable(false)]
		public MapItemsLayerBase Layer { get { return GetLayer(); } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemAttributes"),
#endif
		Category(SRCategoryNames.Data)]
		public MapItemAttributeCollection Attributes { get { return attributes; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemToolTipPattern"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(""), Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public string ToolTipPattern {
			get { return toolTipPattern; }
			set {
				if (toolTipPattern == value)
					return;
				toolTipPattern = value;
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemIsHitTestVisible"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue(DefaultHitTestVisible)]
		public bool IsHitTestVisible {
			get { return isHitTestVisible; }
			set {
				if (isHitTestVisible == value)
					return;
				isHitTestVisible = value;
				if(isHitTestVisible)
					RegisterHitTestableItem();
				else
					UnregisterHitTestableItem();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemVisible"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(DefaultVisible)]
		public bool Visible {
			get { return visible; }
			set {
				if (visible == value) return;
				visible = value;
				InvalidateRender();
			}
		}
		protected internal IHitTestGeometry HitTestGeometry {
			get {
				if (ShouldRecreateHitTestGeometry) {
					hitTestGeometry = CreateHitTestGeometry();
					UpdateHitGeometryInPool(this);
				}
				return hitTestGeometry;
			}
		}
		protected internal virtual MapRect Bounds { get { return bounds; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool IsUpdateLocked { get { return this.lockUpdate != 0; } }
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList<MapItem> ClusteredItems {
			get {
				IClusterItem item = this as IClusterItem;
				return item != null ? item.ClusteredItems.OfType<MapItem>().ToList() : null;
			}
			set {
				IClusterItem item = this as IClusterItem;
				if(item != null)
					item.ClusteredItems = value.OfType<IClusterable>().ToList();
			}
		}
		protected MapItem() {
			this.updateType = MapItemUpdateType.All;
			this.attributes = new MapItemAttributeCollection();
			this.attributeListener = new NotificationCollectionChangedListener<MapItemAttribute>(this.attributes);
		}
		#region IOwnedElement
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				SetOwner(value);
				ApplyAppearance();
				UpdateBoundingRect();
				StyleChanged(MapItemUpdateType.Layout);
			}
		}
		#endregion
		#region IHitTestableElement
		IHitTestableOwner IHitTestableElement.Owner { get { return null; } }
		IEnumerable<IHitTestableElement> IHitTestableElement.HitTest(MapUnit unit, Point point) {
			IHitTestableElement hitTestableElement = GetElementHitTest(unit, point);
			List<IHitTestableElement> hitTestableList = new List<IHitTestableElement>();
			if(hitTestableElement != null) {
				hitTestableList.AddRange(HitTestInnerItems(unit, point));
				hitTestableList.Add(hitTestableElement);
			}
			return hitTestableList;
		}
		MapRect IHitTestableElement.UnitBounds { get { return GetHitTestUnitBounds(); } }
		RegionRange IHitTestableElement.RegionRange { get { return regionRange; } set { regionRange = value; } }
		bool IHitTestableElement.IsHitTestVisible { get { return isHitTestVisible && ActualVisible; } }
		GeometryType IHitTestableElement.GeometryType { get { return GeometryType; } }
		HitTestKey IHitTestableElement.Key { get { return HitTestKey; } set { HitTestKey = value; } }
		object IHitTestableElement.Locker { get { return this; } }
		#endregion
		#region IInteractiveElement
		bool IInteractiveElement.EnableHighlighting {
			get { return EnableHighlighting; }
		}
		bool IInteractiveElement.EnableSelection {
			get { return EnableSelection; }
		}
		bool IInteractiveElement.IsHighlighted {
			get { return isHighlighted; }
			set {
				if(isHighlighted == value)
					return;
				isHighlighted = value;
				OnHighlightedChanged();
			}
		}
		bool IInteractiveElement.IsSelected {
			get { return isSelected; }
			set {
				if(isSelected == value)
					return;
				isSelected = value;
				OnSelectedChanged();
			}
		}
		#endregion
		#region IRenderItem
		IRenderShapeTitle IRenderItem.Title { get { return GetTitle(); } }
		IMapItemGeometry IRenderItem.Geometry { get { return GetGeometry(); } }
		IRenderItemStyle IRenderItem.Style { get { return GetStyle(); } }
		object IRenderItem.UpdateLocker { get { return updateLocker; } }
		IRenderItemResourceHolder IRenderItem.ResourceHolder {
			get { return resourceHolder != null ? resourceHolder : RenderItemResourceHolder.Empty; }
		}
		bool IRenderItem.Visible { get { return ActualVisible && CalculateBoundsVisibility(); } }
		void IRenderItem.OnRender() {
			OnRender();
		}
		protected internal void OnRender() {
			if(Layer != null) {
				LockUpdate();
				try {
					IRenderItemStyle style = Layer.RaiseDrawMapItem(this);
					if(style != null) AfterDrawMapItemEvent(style);
				}
				finally {
					UnlockUpdate();
				}
			}
		}
		bool IRenderItem.CanExport() {
			return CanExport();
		}
		bool IRenderItem.ForceUpdateResourceHolder { get { return forceUpdateResourceHolder; } set { forceUpdateResourceHolder = value; } }
		bool IRenderItem.UseAntiAliasing { get { return AllowUseAntiAliasing; } }
		void IRenderItem.SetResourceHolder(IRenderer renderer, IRenderItemProvider provider) {
			SetResourceHolder(renderer, provider);
		}
		void IRenderItem.PrepareGeometry() {
			PrepareGeometry();
		}
		protected virtual void SetOwner(object value) {
			UnregisterHitTestableItem();
			UnSubscribeAttributeListenerEvents();
			if(value != null)
				SubscribeAttributeListenerEvents();
			lock(this) {
				owner = value;
			}
			if(owner == null)
				ReleaseResourcesInternal();
		}
		protected virtual void SetResourceHolder(IRenderer renderer, IRenderItemProvider provider) {
			lock(updateLocker) {
				ReleaseResourcesInternal();
				this.resourceHolder = renderer.CreateResourceHolder(provider, this);
				this.forceUpdateResourceHolder = true;
			}
		}
		protected virtual void AfterDrawMapItemEvent(IRenderItemStyle style) {
		}
		#endregion
		#region IStyleOwner
		void IMapStyleOwner.OnStyleChanged() {
			StyleChanged(MapItemUpdateType.Style);
		}
		#endregion
		#region IMapItemCore
		string IMapItemCore.Text { get { return GetTextCore(); } }
		Color IMapItemCore.TextColor { get { return GetTextColorCore(); } }
		int IMapItemCore.AttributesCount { get { return Attributes.Count; } }
		void IMapItemCore.AddAttribute(IMapItemAttribute attribute) {
			Attributes.Add(MapItemAttribute.Create(attribute));
		}
		IMapItemAttribute IMapItemCore.GetAttribute(int index) {
			return Attributes[index];
		}
		IMapItemAttribute IMapItemCore.GetAttribute(string name) {
			return Attributes[name];
		}
		#endregion
		#region IMapDataItem
		int IMapDataItem.RowIndex { get { return this.rowIndex; } set { this.rowIndex = value; } }
		int[] IMapDataItem.ListSourceRowIndices { get { return ListSourceRowIndices; } set { ListSourceRowIndices = value; } }
		void IMapDataItem.AddAttribute(IMapItemAttribute attribute) {
			AddAttribute(MapItemAttribute.Create(attribute));
		}
		#endregion
		#region IToolTipSupportElement
		MapItem ISupportToolTip.ActiveObject { get { return this; } }
		string ISupportToolTip.CalculateToolTipText() {
			return CalculateToolTipText();
		}
		#endregion
		#region IMapItemAttributeOwner
		IMapItemAttribute IMapItemAttributeOwner.GetAttribute(string name) {
			return Attributes[name];
		}
		#endregion
		#region ILockableObject implementation
		object ILockableObject.UpdateLocker { get { return updateLocker; } }
		#endregion
		IHitTestableElement GetElementHitTest(MapUnit unit, Point point) {
			IHitTestGeometry geometry = HitTestGeometry;
			if(geometry == null) 
				return null;
			IUnitHitTestGeometry unitGeometry = geometry as IUnitHitTestGeometry;
			if(unitGeometry != null && unitGeometry.HitTest(unit))
				return this;
			IScreenHitTestGeometry screenGeometry = geometry as IScreenHitTestGeometry;
			if(screenGeometry != null && screenGeometry.HitTest(point))
				return this;
			return null;
		}
		void SubscribeAttributeListenerEvents() {
			attributeListener.Changed += OnAttributeCollectionChanged;
		}
		void UnSubscribeAttributeListenerEvents() {
			attributeListener.Changed -= OnAttributeCollectionChanged;
		}
		void OnAttributeCollectionChanged(object sender, EventArgs e) {
			if(Layer != null)
				ResetColorizerColor();
		}
		void OnHighlightedChanged() {
			ResetStyle();
		}
		void OnSelectedChanged() {
			ResetStyle();
		}
		void PrepareGeometry() {
			if(!GeometryIsValid)
				EnsureGeometry();
		}
		protected virtual string GetTextCore() {
			return string.Empty;
		}
		protected virtual Color GetTextColorCore() {
			return Color.Empty;
		}
		protected internal virtual bool CalculateBoundsVisibility() {
			return Layer != null && Layer.View is MiniMap ? true : MapRect.IsIntersected(RenderBounds, bounds);
		}
		protected void UpdateBoundingRect() {
			if (Layer != null)
				Layer.UpdateBoundingRect();
		}
		protected void AddAttribute(MapItemAttribute attr) {
			if (attr == null) return;
			MapItemAttribute founded = Attributes[attr.Name];
			if(founded != null)
				Attributes.Remove(founded);
			AddAttributeCore(attr);
		}
		protected virtual void AddAttributeCore(MapItemAttribute attr) {
			Attributes.Add(attr);
		}
		protected internal virtual MapElementStyleBase GetDefaultItemStyle() {
			return DefaultStyleProvider.ShapeStyle;
		}
		protected internal virtual MapElementStyleBase GetDefaultSelectedItemStyle() {
			return DefaultStyleProvider.SelectedShapeStyle;
		}
		protected internal virtual MapElementStyleBase GetDefaultHighlightedItemStyle() {
			return DefaultStyleProvider.HighlightedShapeStyle;
		}
		protected void UnregisterHitTestableItem() {
			if(HitTestableRegistrator != null) HitTestableRegistrator.UnregisterItem(this);
		}
		protected virtual void RegisterHitTestableItem() {
			if (HitTestableRegistrator != null && IsHitTestVisible) HitTestableRegistrator.RegisterItem(this);
		}
		protected virtual IEnumerable<IHitTestableElement> HitTestInnerItems(MapUnit unit, Point point) {
			return new IHitTestableElement[0];
		}
		protected internal void UpdateHitGeometryInPool(IHitTestableElement element) {
			if(HitTestableRegistrator != null && IsHitTestVisible)
				HitTestableRegistrator.UpdateItem(element);
		}
		protected internal void LockUpdate() {
			this.lockUpdate++;
		}
		protected internal void UnlockUpdate() {
			this.lockUpdate--;
		}
		protected abstract IHitTestGeometry CreateHitTestGeometry();
		protected abstract IRenderItemStyle GetStyle();
		protected abstract MapRect CalculateBounds();
		protected abstract CoordBounds CalculateNativeBounds();
		protected abstract IMapItemGeometry CreateGeometry();
		MapItemsLayerBase GetLayer() {
			IMapDataAdapter data = owner as IMapDataAdapter;
			return data != null ? data.GetLayer() : null;
		}
		protected virtual IRenderShapeTitle GetTitle() {
			return null;
		}
		protected virtual IMapItemGeometry GetGeometry() {
			PrepareGeometry();
			return Geometry;
		}
		protected abstract void PrepareImageGeometry();
		protected void OnGeometryChanged() {
			this.forceUpdateResourceHolder = true;
		}
		protected internal virtual void ReleaseHitTestGeometry() {
			hitTestGeometry = null;
		}
		protected internal bool CanExport() {
			if(Layer != null) {
				LockUpdate();
				try {
					return Layer.RaiseExportMapItem(this);
				} finally {
					UnlockUpdate();
				}
			}
			return false;
		}
		protected internal virtual void RegisterUpdate(MapItemUpdateType updateType) {
			this.updateType |= updateType;
		}
		protected internal void EnsureGeometry() {
			lock(updateLocker) {
				ApplyUpdates();
				if(NeedRecreateGeometry) {
					this.geometry = CreateGeometry();
					PrepareImageGeometry();
				}
				OnGeometryChanged();
				ResetUpdateType();
			}
		}
		protected internal void UpdateNativeBounds() {
			this.nativeBounds = CalculateNativeBounds();
			ResetUnitLocation();
			VectorItemsLayer itemsLayer = Layer as VectorItemsLayer;
			if (itemsLayer != null)
				itemsLayer.AppendBoundingRect(nativeBounds);
		}
		protected internal void ResetUnitLocation() {
			ILocatableRenderItem locatableItem = this as ILocatableRenderItem;
			if (locatableItem != null)
				locatableItem.ResetLocation();
		}
		protected virtual void UpdateBounds() {
			this.bounds = CalculateBounds();
			ResetUnitLocation();
		}
		protected virtual MapRect GetHitTestUnitBounds() {
			return bounds;
		}
		protected internal virtual DrawMapItemEventArgs CreateDrawEventArgs() {
			return new DrawMapItemEventArgs(this);
		}
		protected internal virtual void ResetStyle() {
			if(Layer == null)
				return;
			lock(updateLocker) {
				MergeStyles();
			}
			OnStyleReset();
		}
		protected internal void ResetUpdateType() {
			this.updateType = MapItemUpdateType.None;
		}
		protected internal void UpdateItem(MapItemUpdateType updateType) {
			if(Layer == null)
				return;
			RegisterUpdate(updateType);
			InvalidateRender();
		}
		protected virtual void OnStrokeWidthChanged() {
		}
		protected void ColorizerColorChanged(Color color) {
			ResetStyle();
		}
		protected void InvalidateRender() {
			if(Layer != null) Layer.InvalidateRender();
		}
		protected virtual void OnStyleReset() {
			InvalidateRender();
		}
		protected virtual void ReleaseResourcesInternal() {
			if(resourceHolder != null) {
				resourceHolder.Dispose();
				resourceHolder = null;
			}
		}
		protected internal void StyleChanged(MapItemUpdateType updateType) {
			ResetStyle();
			UpdateItem(updateType);
		}
		protected internal virtual void ApplyAppearance() {
		}
		internal void RegisterLayoutUpdate() {
			RegisterUpdate(MapItemUpdateType.Layout);
		}
		void EnsureLayout() {
			if(Layer != null) {
				lock(Layer.UpdateLocker) {
					UpdateBounds();
					RegisterHitTestableItem();
				}
			}
		}
		protected internal void ApplyUpdates() {
			if(UpdateType.HasFlag(MapItemUpdateType.Layout) || UpdateType.HasFlag(MapItemUpdateType.Location))
				EnsureLayout();
		}
		protected internal void MergeStyles() {
			StyleMerger.Merge(ActualStyle);
		}
		protected virtual internal void OnLayerInitialized(MapItemsLayerBase itemsLayer) { }
		protected internal string CalculateToolTipText() {
			ToolTipPatternParser parser = CreateToolTipPatternParser(ActualToolTipPattern);
			parser.AddContextRange(Attributes);
			return parser.GetText();
		}
		protected internal virtual ToolTipPatternParser CreateToolTipPatternParser(string pattern) {
			return new ToolTipPatternParser(pattern);
		}
		protected internal virtual void HandleMapItemMouseMove(MouseEventArgs e) {
		}
		protected internal virtual void ResetColorizerColor() {
		}
		protected internal abstract IList<Map.CoordPoint> GetItemPoints();
		protected internal virtual IList<MapUnit> GetUnitPoints() {
			return Geometry != null ? Geometry.GetPoints() : new List<MapUnit>();
		}
		protected internal virtual IList<IList<MapUnit>> GetSegmentGeometries(IList<MapUnit> unitPoints) {
			return new List<IList<MapUnit>>() { unitPoints };
		}
	}
}
