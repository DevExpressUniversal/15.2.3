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
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraMap.Drawing {
	[Flags]
	public enum ViewInfoUpdateType {
		None = 0,
		NavigationPanel = 1,
		Legend = 2,
		ErrorPanel = 4,
		SearchPanel = 8,
		CustomOverlay = 16,
		All = NavigationPanel | Legend | ErrorPanel | SearchPanel | CustomOverlay
	}
	public abstract class MapViewInfoBase : MapDisposableObject {
		readonly InnerMap map;
		Rectangle bounds;
		Rectangle clientBounds;
		public InnerMap Map { get { return map; } }
		public Rectangle Bounds {
			get { return bounds; }
			protected set {
				if(bounds == value)
					return;
				bounds = value;
			}
		}
		public Rectangle ClientBounds {
			get { return clientBounds; }
			protected set {
				if(clientBounds == value)
					return;
				clientBounds = value;
				OnClientBoundsChanged();
			}
		}
		protected MapViewInfoBase(InnerMap map) {
			Guard.ArgumentNotNull(map, "map");
			this.map = map;
		}
		protected virtual void OnClientBoundsChanged() { }
		protected void ResetBounds() {
			this.bounds = Rectangle.Empty;
			this.clientBounds = Rectangle.Empty;
		}
	}
	public abstract class SelfpaintingViewinfo : MapViewInfoBase {
		readonly MapStyleCollection paintAppearance;
		readonly OverlayViewInfoPainter painter;
		protected abstract MapStyleCollection DefaultAppearance { get; }
		protected virtual MapStyleCollection UserAppearance { get { return DefaultAppearance; } }
		public MapStyleCollection PaintAppearance { get { return paintAppearance; } }
		public OverlayViewInfoPainter Painter { get { return painter; } }
		protected SelfpaintingViewinfo(InnerMap map, MapStyleCollection viewInfoPaintAppearance, OverlayViewInfoPainter painter)
			: base(map) {
			this.painter = painter;
			this.paintAppearance = viewInfoPaintAppearance;
		}
		protected virtual void CalculateNestedPaintAppearance() {
		}
		protected internal void CalculatePaintAppearance() {
			MapStyleCollection defaultAppearances = DefaultAppearance;
			MapStyleCollection userAppearances = UserAppearance;
			foreach(KeyValuePair<string, MapElementStyleBase> pair in defaultAppearances) {
				MapElementStyleBase defaultStyle = pair.Value;
				string appearanceName = pair.Key;
				MapElementStyleBase paintStyle = PaintAppearance[appearanceName];
				MapElementStyleBase userStyle = userAppearances[appearanceName];
				MapElementStyleBase.MergeStyles(userStyle, defaultStyle, paintStyle);
			}
			CalculateNestedPaintAppearance();
		}
	}
	public abstract class OverlayViewInfoBase : SelfpaintingViewinfo {
		Point mousePosition;
		protected Point MousePosition { get { return mousePosition; } }
		protected abstract int KeyInternal { get; }
		public int CreateKey() {
			ISupportIndexOverlay indexOverlay = this as ISupportIndexOverlay;
			int index = indexOverlay != null ? indexOverlay.Index : 0;
			return KeyInternal + index;
		}
		public virtual bool Printable { get { return false; } }
		public virtual bool ShowInDesign { get { return false; } }
		public virtual bool CanStore { get { return false; } }
		public virtual ViewInfoUpdateType SupportedUpdateType { get { return ViewInfoUpdateType.None; } }
		protected OverlayViewInfoBase(InnerMap map, MapStyleCollection viewInfoPaintAppearance, OverlayViewInfoPainter painter)
			: base(map, viewInfoPaintAppearance, painter) {
		}
		public void Calculate(Graphics gr, Rectangle controlBounds, Point mousePosition, bool shouldLayout) {
			this.mousePosition = mousePosition;
			CalculatePaintAppearance();
			if(shouldLayout)
				CalculateLayout(controlBounds);
			CalculateOverlay(gr, controlBounds);
			ValidateOverlay();
		}
		protected internal virtual void CalculateLayout(Rectangle controlBounds) { }
		protected internal virtual void CalculateOverlay(Graphics gr, Rectangle controlBounds) { }
		public virtual void SetCoordData(CoordPoint activePoint, double kilometersScale) { }
		protected virtual void ValidateOverlay() {
		}
	}
	public class MapViewInfo : MapDisposableObject {
		readonly InnerMap map;
		bool isReady;
		MapViewInfoCollection viewInfos;
		protected InnerMap Map { get { return map; } }
		public MapViewInfoCollection ViewInfos { get { return viewInfos; } }
		public bool IsReady { get { return isReady; } }
		public GeoPoint ActiveGeoPoint { get; set; }
		public double KilometersScale { get; set; }
		public MapViewInfo(InnerMap map) {
			this.map = map;
			this.viewInfos = new MapViewInfoCollection();
		}
		protected override void DisposeOverride() {
			base.DisposeOverride();
			ReleaseViewInfos();
		}
		void FillViewInfoCollection(MapUIElementsPainter painter) {
			ReleaseViewInfos();
			AddViewInfoItem(CreateNavigationPanelViewInfo(painter.NavigationPanelPainter));
			AddLegendViewInfoRange(painter.SizeLegendPainter, painter.ColorLegendPainter);
			AddViewInfoItems(CreateErrorPanelViewInfos(painter.ErrorPanelPainter));
			AddViewInfoItem(CreateSearchPanelViewInfo(painter.SearchPanelPainter));
			AddViewInfoItems(CreateCustomOverlayViewinfos(painter));
		}
		void AddLegendViewInfoRange(SizeLegendPainter sizePainter, ColorLegendPainter colorPainter) {
			int count = Map.Legends.Count;
			for(int i = 0; i < count; i++) {
				MapLegendBase legend = Map.Legends[i];
				if(Map.OperationHelper.CanShowLegend(legend))
					AddViewInfoItem(CreateLegendViewInfo(legend, sizePainter, colorPainter, i));
			}
		}
		void AddViewInfoItems(IEnumerable<MapViewInfoBase> viewInfos) {
			foreach (MapViewInfoBase viewInfo in viewInfos)
				AddViewInfoItem(viewInfo);
		}
		void AddViewInfoItem(MapViewInfoBase viewInfo) {
			if(viewInfo != null) viewInfos.Add(viewInfo);
		}
		void ReleaseViewInfos() {
			lock(ViewInfos) {
				foreach(MapViewInfoBase mapViewInfo in viewInfos)
					mapViewInfo.Dispose();
				viewInfos.Clear();
			}
		}
		IEnumerable<OverlayViewInfoBase> CreateErrorPanelViewInfos(ErrorPanelPainter painter) {
			IList<OverlayViewInfoBase> result = new List<OverlayViewInfoBase>();
			if (Map.OperationHelper.CanShowErrorPanel())
				result.Add(new ErrorPanelViewInfo(Map, painter) { ErrorPanelIndex = 0 });
			if(Map.OperationHelper.CanShowMiniMapErrorPanel())
				result.Add(new ErrorPanelViewInfo(Map, Map.MiniMap.Bounds, painter) { ErrorPanelIndex = 1 });
			return result;
		}
		IEnumerable<CustomOverlayViewInfo> CreateCustomOverlayViewinfos(MapUIElementsPainter painter) {
			for(int i = 0; i < Map.Overlays.Count; i++) {
				MapOverlay overlay = Map.Overlays[i];
				if(Map.OperationHelper.CanShowOverlay(overlay))
					yield return new CustomOverlayViewInfo(Map, overlay, painter) { OverlayIndex = i };
			}
		}
		OverlayViewInfoBase CreateSearchPanelViewInfo(SearchPanelPainter painter) {
			return Map.OperationHelper.IsSearchActive() ? new SearchPanelViewInfo(Map, painter) : null;
		}
		OverlayViewInfoBase CreateLegendViewInfo(MapLegendBase legend, SizeLegendPainter sizePainter, ColorLegendPainter colorPainter, int legendIndex) {
			LegendViewInfo info = LegendViewInfo.CreateViewInfo(legend, map, sizePainter, colorPainter);
			if(info != null) {
				info.Initialize(legendIndex);
			}
			return info;
		}
		OverlayViewInfoBase CreateNavigationPanelViewInfo(NavigationPanelPainter painter) {
			if(!Map.OperationHelper.CanShowNavigationPanel())
				return null;
			return new NavigationPanelViewInfo(Map, Map.RenderController.ZoomTrackBarController, Map.RenderController.ScrollButtonsController, painter);
		}
		internal IMapUiHitInfo CalcHitInfo(Point point) {
			IMapUiHitInfo hitInfo = null;
			foreach(MapViewInfoBase viewInfo in ViewInfos) {
				IHitTestableViewinfo hitTestable = viewInfo as IHitTestableViewinfo;
				if(hitTestable != null) {
					IMapUiHitInfo uiHitInfo = hitTestable.CalcHitInfo(point);
					if(uiHitInfo != null)
						hitInfo = uiHitInfo;
				}
			}
			return hitInfo ?? new MapUiHitInfo(Point.Empty, MapHitUiElementType.None);
		}
		internal void UpdateVisualState(MapHitUiElementType hitTestType) {
			foreach(MapViewInfoBase viewInfo in ViewInfos) {
				ISupportPressedStatePanel panel = viewInfo as ISupportPressedStatePanel;
				if(panel != null)
					panel.UpdatePressedState(hitTestType);
			}
		}
		public void Initialize(MapUIElementsPainter painter) {
			FillViewInfoCollection(painter);
			ResetIsReady();
		}
		public void ResetIsReady() {
			this.isReady = false;
		}
		public void SetIsReady() {
			this.isReady = true;
		}
	}
	public class MapViewInfoCollection : List<MapViewInfoBase> { }
	public struct ViewInfoUpdateParams {
		public CoordPoint CoordPoint { get; set; }
		public Point MousePosition { get; set; }
		public double KilometersScale { get; set; }
		public Rectangle Bounds { get; set; }
		public ViewInfoUpdateType UpdateType { get; set; }
		public bool ShouldRearrange { get; set; }
	}
	public abstract class ViewInfoBuilderBase : MapDisposableObject {
		readonly RenderController controller;
		protected RenderController Controller { get { return controller; } }
		protected ViewInfoBuilderBase(RenderController controller) {
			Guard.ArgumentNotNull(controller, "controller");
			this.controller = controller;
		}
		public void Update(MapViewInfo viewInfo, Graphics graphics, ViewInfoUpdateParams parameters) {
			UpdateViewInfoItems(viewInfo.ViewInfos, graphics, parameters);
			viewInfo.SetIsReady();
			OnViewInfoUpdated();
		}
		void UpdateViewInfoItems(MapViewInfoCollection viewInfos, Graphics graphics, ViewInfoUpdateParams parameters) {
			List<OverlayViewInfoBase> overlays = new List<OverlayViewInfoBase>();
			foreach(MapViewInfoBase item in viewInfos) {
				OverlayViewInfoBase overlayItem = item as OverlayViewInfoBase;
				if(overlayItem != null && parameters.UpdateType.HasFlag(overlayItem.SupportedUpdateType)) {
					UpdateViewInfoItem(overlayItem, graphics, parameters);
					overlays.Add(overlayItem);
				}
			}
			if(parameters.ShouldRearrange) {
				RearrangeViewinfos(parameters.Bounds, overlays);
				RaiseOverlaysArrangedEventArgs(viewInfos, parameters.UpdateType);
			}
			foreach(OverlayViewInfoBase item in overlays)
				controller.UpdateOverlayCache(item);
		}
		void RearrangeViewinfos(Rectangle controlBounds, List<OverlayViewInfoBase> overlays) {
			ViewInfoLayoutCalculator calc = new ViewInfoLayoutCalculator();
			foreach(OverlayViewInfoBase item in overlays) {
				IViewInfoSupportAlignment supportAlignmentItem = item as IViewInfoSupportAlignment;
				if(supportAlignmentItem != null)
					calc.Add(supportAlignmentItem);
			}
			if(calc.CanAlign) {
				int navPanelHeight = MapUtils.GetActualNavigationPanelHeight(Controller.Map);
				calc.Align(new Rectangle(controlBounds.X, controlBounds.Y, controlBounds.Width, controlBounds.Height - navPanelHeight));
			}
		}
		void RaiseOverlaysArrangedEventArgs(MapViewInfoCollection viewInfos, ViewInfoUpdateType updateType) {
			OverlaysArrangedEventArgs args = CreateOverlaysArrangedEventArgs(viewInfos, updateType);
			if(args == null)
				return;
			Controller.RaiseOverlayLayoutCalculated(args);
			ApplyOverlayLayout(viewInfos, args);
		}
		OverlaysArrangedEventArgs CreateOverlaysArrangedEventArgs(MapViewInfoCollection viewinfos, ViewInfoUpdateType updateType) {
			List<OverlayArrangement> overlaysLayout = new List<OverlayArrangement>();
			foreach(MapViewInfoBase viewinfo in viewinfos) {
				ISupportViewinfoLayout layout = viewinfo as ISupportViewinfoLayout;
				if(layout != null) {
					OverlayArrangement overlayLayout = layout.CreateOverlayLayout(updateType);
					overlaysLayout.Add(overlayLayout);
				}
			}
			return overlaysLayout.Count > 0 ? new OverlaysArrangedEventArgs(overlaysLayout.ToArray()) : null;
		}
		void ApplyOverlayLayout(MapViewInfoCollection viewinfos, OverlaysArrangedEventArgs args) {
			int applyingLayoutIndex = 0;
			foreach(MapViewInfoBase viewinfo in viewinfos) {
				ISupportViewinfoLayout layout = viewinfo as ISupportViewinfoLayout; 
				if(layout != null)
					layout.ApplyLayout(args.OverlayArrangements[applyingLayoutIndex++]);
			}
		}
		protected void UpdateViewInfoItem(OverlayViewInfoBase item, Graphics graphics, ViewInfoUpdateParams parameters) {
			item.SetCoordData(parameters.CoordPoint, parameters.KilometersScale);
			item.Calculate(graphics, parameters.Bounds, parameters.MousePosition, parameters.ShouldRearrange);
		}
		protected virtual void OnViewInfoUpdated() {
		}
	}
	public class DefaultViewInfoBuilder : ViewInfoBuilderBase {
		public DefaultViewInfoBuilder(RenderController controller)
			: base(controller) {
		}
		protected override void OnViewInfoUpdated() {
			base.OnViewInfoUpdated();
			Controller.OnViewInfoUpdated();
		}
	}
	public class MapSkinBorderPainter : SkinTextBorderPainter {
		public SkinElement BorderElement { get; set; }
		public MapSkinBorderPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			return new SkinElementInfo(BorderElement);
		}
	}
	public interface IViewInfoSupportAlignment {
		Rectangle LayoutRect { get; set; }
		ContentAlignment Alignment { get; }
		Orientation JoiningOrientation { get; }
	}
	public class ViewInfoLayoutCalculator {
		Dictionary<ContentAlignment, List<IViewInfoSupportAlignment>> viewInfosList = new Dictionary<ContentAlignment, List<IViewInfoSupportAlignment>>();
		Size viewinfosSize;
		public bool CanAlign { get; private set; }
		public Size ViewinfosSize { get { return viewinfosSize; } }
		public ViewInfoLayoutCalculator() {
			CanAlign = false;
		}
		void AlignCore(List<IViewInfoSupportAlignment> items, Rectangle availableBounds, ContentAlignment alignment) {
			if(items == null || items.Count < 1)
				return;
			Rectangle contentRect = CalculateContentRect(items, true, alignment);
			contentRect = RectUtils.AlignRectangle(contentRect, availableBounds, alignment);
			foreach(IViewInfoSupportAlignment item in items) {
				item.LayoutRect = RectUtils.AlignInsideRectWithInversion(item.LayoutRect, contentRect, alignment);
			}
		}
		int CalculateItemOriginX(IViewInfoSupportAlignment item, ContentAlignment alignment, Rectangle contentRect) {
			if(ContentAlignmentUtils.RightAligned(alignment))
				return contentRect.Right;
			if(ContentAlignmentUtils.CenterAligned(alignment) && item.JoiningOrientation == Orientation.Vertical)
				return RectUtils.GetHorizontalCenter(contentRect) - item.LayoutRect.Width / 2;
			return contentRect.Left;
		}
		int CalculateItemOriginY(IViewInfoSupportAlignment item, ContentAlignment alignment, Rectangle contentRect) {
			if(ContentAlignmentUtils.BottomAligned(alignment))
				return contentRect.Bottom;
			if(ContentAlignmentUtils.MiddleAligned(alignment) && item.JoiningOrientation == Orientation.Horizontal)
				return RectUtils.GetVerticalCenter(contentRect) - item.LayoutRect.Height / 2;
			return contentRect.Top;
		}
		Point GetOrientalLocation(List<Rectangle> orientalRects, Orientation orientation) {
			if(orientation == Orientation.Horizontal)
				return new Point(orientalRects[orientalRects.Count - 1].Right, 0);
			return new Point(0, orientalRects[orientalRects.Count - 1].Bottom);
		}
		Rectangle ProcessRect(List<Rectangle> orientalRects, List<Rectangle> oppositeRects, Rectangle layoutRect, Orientation orientation) {
			Rectangle rect = new Rectangle(GetOrientalLocation(orientalRects, orientation), layoutRect.Size);
			foreach(Rectangle oppositeRect in oppositeRects)
				if(RectUtils.IntersectsExcludeBounds(rect, oppositeRect)) {
					if(orientation == Orientation.Horizontal)
						rect.X = oppositeRect.Right;
					else
						rect.Y = oppositeRect.Bottom;
				}
			orientalRects.Add(rect);
			return rect;
		}
		int GetLowestBound(Rectangle verticalRect, List<Rectangle> horizontalRects) {
			int max = verticalRect.Bottom;
			foreach(Rectangle rect in horizontalRects)
				if(rect.Bottom > max)
					max = rect.Bottom;
			return max;
		}
		int GetRightestBound(Rectangle horizontalRect, List<Rectangle> verticalRects) {
			int max = horizontalRect.Right;
			foreach(Rectangle rect in verticalRects)
				if(rect.Right > max)
					max = rect.Right;
			return max;
		}
		void RecalculateLayoutRects(List<IViewInfoSupportAlignment> items, ContentAlignment alignment, List<Rectangle> horizontalRects, List<Rectangle> verticalRects, Size result) {
			if(ContentAlignmentUtils.CenterAligned(alignment)) {
				int horizontalSize = horizontalRects[horizontalRects.Count - 1].Right;
				for(int i = 0; i < items.Count; i++) {
					IViewInfoSupportAlignment item = items[i];
					if(item.JoiningOrientation == Orientation.Horizontal || i == 0)
						item.LayoutRect = new Rectangle(item.LayoutRect.X + Math.Max(result.Width - horizontalSize, 0) / 2, item.LayoutRect.Y, item.LayoutRect.Width, item.LayoutRect.Height);
					else if(horizontalSize != result.Width)
						item.LayoutRect = new Rectangle(item.LayoutRect.X + (result.Width - item.LayoutRect.Width) / 2, item.LayoutRect.Y, item.LayoutRect.Width, item.LayoutRect.Height);
				}
			}
			if(ContentAlignmentUtils.MiddleAligned(alignment)) {
				int verticalSize = verticalRects[verticalRects.Count - 1].Bottom;
				for(int i = 0; i < items.Count; i++) {
					IViewInfoSupportAlignment item = items[i];
					if(item.JoiningOrientation == Orientation.Vertical || i == 0)
						item.LayoutRect = new Rectangle(item.LayoutRect.X, item.LayoutRect.Y + Math.Max(result.Height - verticalSize, 0) / 2, item.LayoutRect.Width, item.LayoutRect.Height);
					else if(verticalSize != result.Height)
						item.LayoutRect = new Rectangle(item.LayoutRect.X, item.LayoutRect.Y + (result.Height - item.LayoutRect.Height) / 2, item.LayoutRect.Width, item.LayoutRect.Height);
				}
			}
		}
		Size CalculateContentSizeCore(List<IViewInfoSupportAlignment> items, bool needUpdateLayoutRect, ContentAlignment alignment) {
			if(items.Count == 0)
				return new Size();
			List<Rectangle> horizontalRects = new List<Rectangle>() { new Rectangle(Point.Empty, items[0].LayoutRect.Size) };
			List<Rectangle> verticalRects = new List<Rectangle>() { new Rectangle(Point.Empty, items[0].LayoutRect.Size) };
			for(int i = 1; i < items.Count; i++) {
				IViewInfoSupportAlignment item = items[i];
				Rectangle newLayoutRect;
				if(item.JoiningOrientation == Orientation.Horizontal)
					newLayoutRect = ProcessRect(horizontalRects, verticalRects, item.LayoutRect, Orientation.Horizontal);
				else
					newLayoutRect = ProcessRect(verticalRects, horizontalRects, item.LayoutRect, Orientation.Vertical);
				if(needUpdateLayoutRect)
					item.LayoutRect = newLayoutRect;
			}
			Size result = new Size(GetRightestBound(horizontalRects[horizontalRects.Count - 1], verticalRects), GetLowestBound(verticalRects[verticalRects.Count - 1], horizontalRects));
			if(needUpdateLayoutRect) {
				RecalculateLayoutRects(items, alignment, horizontalRects, verticalRects, result);
			}
			return result;
		}
		Size CalculateMergedSize(Dictionary<ContentAlignment, Size> sizes) {
			Size result = new Size();
			int topLineSize = Math.Max(sizes[ContentAlignment.TopLeft].Height, Math.Max(sizes[ContentAlignment.TopCenter].Height, sizes[ContentAlignment.TopRight].Height));
			int middleLineSize = Math.Max(sizes[ContentAlignment.MiddleLeft].Height, Math.Max(sizes[ContentAlignment.MiddleCenter].Height, sizes[ContentAlignment.MiddleRight].Height));
			int bottomLineSize = Math.Max(sizes[ContentAlignment.BottomLeft].Height, Math.Max(sizes[ContentAlignment.BottomCenter].Height, sizes[ContentAlignment.BottomRight].Height));
			int leftLineSize = Math.Max(sizes[ContentAlignment.TopLeft].Width, Math.Max(sizes[ContentAlignment.MiddleLeft].Width, sizes[ContentAlignment.BottomLeft].Width));
			int centerLineSize = Math.Max(sizes[ContentAlignment.TopCenter].Width, Math.Max(sizes[ContentAlignment.MiddleCenter].Width, sizes[ContentAlignment.BottomCenter].Width));
			int rightLineSize = Math.Max(sizes[ContentAlignment.TopRight].Width, Math.Max(sizes[ContentAlignment.MiddleRight].Width, sizes[ContentAlignment.BottomRight].Width));
			result.Height = middleLineSize != 0 ? 2 * Math.Max(topLineSize, bottomLineSize) + middleLineSize : topLineSize + bottomLineSize;
			result.Width = centerLineSize != 0 ? 2 * Math.Max(leftLineSize, rightLineSize) + centerLineSize : leftLineSize + rightLineSize;
			return result;
		}
		Size CalculateOverallSize() {
			Dictionary<ContentAlignment, Size> sizesDictionary = new Dictionary<ContentAlignment,Size>();
			foreach(ContentAlignment alignment in Enum.GetValues(typeof(ContentAlignment)))
				sizesDictionary.Add(alignment, new Size());
			foreach(KeyValuePair<ContentAlignment, List<IViewInfoSupportAlignment>> item in viewInfosList) {
				Rectangle contentRect = CalculateContentRect(item.Value, false, item.Key);
				sizesDictionary[item.Key] = contentRect.Size;
			}
			return CalculateMergedSize(sizesDictionary);
		}
		internal Rectangle CalculateContentRect(List<IViewInfoSupportAlignment> items, bool needUpdateLayoutRect, ContentAlignment alignment) {
			List<IViewInfoSupportAlignment> itemsToLayout = new List<IViewInfoSupportAlignment>();
			List<IViewInfoSupportAlignment> layoutedItems = new List<IViewInfoSupportAlignment>();
			foreach(IViewInfoSupportAlignment item in items)
				if(item.LayoutRect.Location == MapOverlayItemBase.DefaultLocation)
					itemsToLayout.Add(item);
				else
					layoutedItems.Add(item);
			Size layoutingItemsSize = CalculateContentSizeCore(itemsToLayout, needUpdateLayoutRect, alignment);
			Rectangle result = new Rectangle(Point.Empty, layoutingItemsSize);
			foreach(IViewInfoSupportAlignment layoutedItem in layoutedItems)
				result = RectUtils.UnionNonEmpty(result, layoutedItem.LayoutRect);
			return result;
		}
		internal Point CalculateItemOrigin(IViewInfoSupportAlignment item, ContentAlignment alignment, Rectangle contentRect) {
			Point result = new Point();
			result.X = CalculateItemOriginX(item, alignment, contentRect);
			result.Y = CalculateItemOriginY(item, alignment, contentRect);
			return result;
		}
		internal Rectangle CalculateItemBounds(Point origin, Size size, ContentAlignment alignment) {
			Rectangle result = new Rectangle(Point.Empty, size);
			result.X = ContentAlignmentUtils.RightAligned(alignment) ? origin.X - size.Width : origin.X;
			result.Y = ContentAlignmentUtils.BottomAligned(alignment) ? origin.Y - size.Height : origin.Y;
			return result;
		}
		internal Rectangle CutContentRectangle(Rectangle contentRectangle, Rectangle itemBounds, ContentAlignment alignment, Orientation orientation) {
			Rectangle result = new Rectangle();
			result.X = orientation == Orientation.Vertical || ContentAlignmentUtils.RightAligned(alignment) ? contentRectangle.X : itemBounds.Right;
			result.Y = orientation == Orientation.Horizontal || ContentAlignmentUtils.BottomAligned(alignment) ? contentRectangle.Y : itemBounds.Bottom;
			result.Width = orientation == Orientation.Vertical ? contentRectangle.Width :
				ContentAlignmentUtils.RightAligned(alignment) ? itemBounds.X - contentRectangle.X : contentRectangle.Right - itemBounds.Right;
			result.Height = orientation == Orientation.Horizontal ? contentRectangle.Height :
				ContentAlignmentUtils.BottomAligned(alignment) ? itemBounds.Y - contentRectangle.Y : contentRectangle.Bottom - itemBounds.Bottom;
			return result;
		}
		public void Clear() {
			viewInfosList.Clear();
			CanAlign = false;
		}
		public void Add(IViewInfoSupportAlignment item) {
			if(item == null) return;
			List<IViewInfoSupportAlignment> list;
			viewInfosList.TryGetValue(item.Alignment, out list);
			if(list == null) {
				viewInfosList[item.Alignment] = new List<IViewInfoSupportAlignment>();
				viewInfosList[item.Alignment].Add(item);
			}
			else list.Add(item);
			CanAlign = true;
		}
		public void Align() {
			this.viewinfosSize = CalculateOverallSize();
			Align(new Rectangle(Point.Empty, viewinfosSize));
		}
		public void Align(Rectangle controlBounds) {
			foreach(KeyValuePair<ContentAlignment, List<IViewInfoSupportAlignment>> item in viewInfosList)
				AlignCore(item.Value, controlBounds, item.Key);
			CanAlign = false;
		}
	}
}
