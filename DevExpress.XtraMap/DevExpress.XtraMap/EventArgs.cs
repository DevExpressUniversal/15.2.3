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
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraMap {
	public delegate void ViewportChangedEventHandler(object sender, ViewportChangedEventArgs e);
	public class ViewportChangedEventArgs : EventArgs {
		readonly CoordPoint topLeft;
		readonly CoordPoint bottomRight;
		readonly double zoomLevel;
		readonly bool isAnimated;
		public CoordPoint TopLeft { get { return topLeft; } }
		public CoordPoint BottomRight { get { return bottomRight; } }
		public double ZoomLevel { get { return zoomLevel; } }
		public bool IsAnimated { get { return isAnimated; } }
		internal ViewportChangedEventArgs(CoordPoint topLeft, CoordPoint bottomRight, double zoomLevel, bool isAnimated) {
			this.topLeft = topLeft;
			this.bottomRight = bottomRight;
			this.zoomLevel = zoomLevel;
			this.isAnimated = isAnimated;
		}
	}
	public class MapItemEventArgs : EventArgs {
		readonly MapItem item;
		public MapItemEventArgs(MapItem item) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		public MapItem Item { get { return item; } }
	}
	public delegate void ExportMapItemEventHandler(object sender, ExportMapItemEventArgs e);
	public class ExportMapItemEventArgs : MapItemEventArgs {
		public bool Cancel { get; set; }
		public bool IsSelected {
			get {
				IInteractiveElement interactive = Item as IInteractiveElement;
				return (interactive != null) ? interactive.IsSelected : false;
			}
		}
		public ExportMapItemEventArgs(MapItem item) : base(item) { 
		}
	}
	public delegate void DrawMapItemEventHandler(object sender, DrawMapItemEventArgs e);
	public class DrawMapItemEventArgs : MapItemEventArgs, IRenderItemStyle {
		public DrawMapItemEventArgs(MapItem item) : base(item) {
		}
		protected MapItemStyle ActualStyle { get { return Item.ActualStyle; } }
		public MapItemsLayerBase Layer { get { return Item.Layer; } }
		public bool IsHighlighted {
			get {
				IInteractiveElement interactive = Item as IInteractiveElement;
				return (interactive != null) ? interactive.IsHighlighted : false;
			}
		}
		public bool IsSelected {
			get {
				IInteractiveElement interactive = Item as IInteractiveElement;
				return (interactive != null) ? interactive.IsSelected : false;
			}
		}
		public Color Fill {
			get { return ActualStyle.Fill; }
			set {
				if(ActualStyle.Fill == value)
					return;
				ActualStyle.Fill = value;
				OnPropertyChanged();
			}
		}
		public Color Stroke {
			get { return ActualStyle.Stroke; }
			set {
				if(ActualStyle.Stroke == value)
					return;
				ActualStyle.Stroke = value;
				OnPropertyChanged();
			}
		}
		public int StrokeWidth {
			get { return ActualStyle.StrokeWidth; }
			set {
				if(ActualStyle.StrokeWidth == value)
					return;
				ActualStyle.StrokeWidth = value;
				OnPropertyChanged();
			}
		}
		protected internal bool HasUpdate { get; set; }
		protected void OnPropertyChanged(){
			HasUpdate = true;
		}
	}
	public class DrawMapPointerEventArgs : DrawMapItemEventArgs, IPointRenderItemStyle {
		Image image;
		string text;
		byte transparency;
		bool disposeImage;
		public Image Image {
			get { return image; }
			set {
				if(image == value)
					return;
				image = value;
				OnPropertyChanged();
			}
		}
		public bool DisposeImage {
			get { return disposeImage; }
			set { disposeImage = value; }
		}
		public string Text {
			get { return text; }
			set {
				if(text == value)
					return;
				text = value;
				OnPropertyChanged();
			}
		}
		public Color TextColor {
			get { return ActualStyle.TextColor; }
			set {
				if (ActualStyle.TextColor == value)
					return;
				ActualStyle.TextColor = value;
				OnPropertyChanged();
			}
		}
		public byte Transparency {
			get { return transparency; }
			set {
				if (transparency == value)
					return;
				transparency = value;
				OnPropertyChanged();
			}
		}
		public DrawMapPointerEventArgs(MapPointer item) : base(item) {
		}
	}
	public class DrawMapShapeEventArgs : DrawMapItemEventArgs, IShapeRenderItemStyle {
		Color titleColor;
		Color titleGlowColor;
		string actualTitle;
		public Color TitleColor {
			get { return titleColor; }
			set {
				if(titleColor == value)
					return;
				titleColor = value;
				OnPropertyChanged();
			}
		}
		public Color TitleGlowColor {
			get { return titleGlowColor; }
			set {
				if(titleGlowColor == value)
					return;
				titleGlowColor = value;
				OnPropertyChanged();
			}
		}
		public string ActualTitle {
		get { return actualTitle; }
			set {
				if(actualTitle == value)
					return;
				actualTitle = value;
				OnPropertyChanged();
			}
		}
		public DrawMapShapeEventArgs(MapShape item) : base(item) {
		}
	}
	public class DrawMapSegmentEventArgs : EventArgs, IRenderItemStyle {
		MapSegmentBase segment;
		DrawMapSegmentableItemEventArgs owner;
		bool fillSet = false;
		bool strokeSet = false;
		bool strokeWidthSet = false;
		public DrawMapSegmentEventArgs(DrawMapSegmentableItemEventArgs owner, MapSegmentBase segment) {
			this.owner = owner;
			Guard.ArgumentNotNull(segment, "segment");
			this.segment = segment;
		}
		protected MapItemStyle ActualStyle { get { return segment.ActualStyle; } }
		public Color Fill {
			get { return fillSet ? ActualStyle.Fill : owner.Fill; }
			set {
				this.fillSet = true;
				if(ActualStyle.Fill == value)
					return;
				ActualStyle.Fill = value;
				OnPropertyChanged();
			}
		}
		public Color Stroke {
			get { return strokeSet ? ActualStyle.Stroke : owner.Stroke; }
			set {
				this.strokeSet = true;
				if(ActualStyle.Stroke == value)
					return;
				ActualStyle.Stroke = value;
				OnPropertyChanged();
			}
		}
		public int StrokeWidth {
			get { return strokeWidthSet ? ActualStyle.StrokeWidth : owner.StrokeWidth; }
			set {
				this.strokeWidthSet = true;
				if(ActualStyle.StrokeWidth == value)
					return;
				ActualStyle.StrokeWidth = value;
				OnPropertyChanged();
			}
		}
		protected void OnPropertyChanged() {
			owner.HasUpdate = true;
		}
	}
	public class DrawMapSegmentableItemEventArgs : DrawMapShapeEventArgs, ICompositeRenderItemStyle {
		DrawMapSegmentEventArgs[] segments;
		public DrawMapSegmentEventArgs[] Segments { get { return segments; } }
		protected internal DrawMapSegmentableItemEventArgs(ISupportSegments item) : base((MapShape)item) {
			this.segments = CreateSegments(item);
		}
		#region ICompositeRenderItemStyle implementation
		IRenderItemStyle[] ICompositeRenderItemStyle.Parts { get { return segments; } }
		#endregion
		DrawMapSegmentEventArgs[] CreateSegments(ISupportSegments item) {
			DrawMapSegmentEventArgs[] result = new DrawMapSegmentEventArgs[item.Segments.Length];
			for(int i = 0; i < item.Segments.Length; i++)
				result[i] = new DrawMapSegmentEventArgs(this, item.Segments[i]);
			return result;
		}
	}
	public delegate void MapItemClickEventHandler(object sender, MapItemClickEventArgs e);
	public class MapItemClickEventArgs : MapItemEventArgs {
		MouseEventArgs mouseArgs;
		public bool Handled { get; set; }
		public MouseEventArgs MouseArgs { get { return mouseArgs; } }
		public MapItemClickEventArgs(MapItem item, MouseEventArgs args) : base(item) {
			this.mouseArgs = args;
		}
	}
	public delegate void MapItemHighlightingEventHandler(object sender, MapItemHighlightingEventArgs e);
	public class MapItemHighlightingEventArgs : MapItemEventArgs {
		public bool Cancel { get; set; }
		public MapItemHighlightingEventArgs(MapItem item) : base(item) {
		}
	}
	public abstract class MapSelectionEventArgs : EventArgs {
		readonly List<object> selection;
		public IList<object> Selection { get { return selection; } }
		protected MapSelectionEventArgs(IEnumerable<object> selection) {
			this.selection = new List<object>(selection);
		}
	}
	public delegate void MapSelectionChangedEventHandler(object sender, MapSelectionChangedEventArgs e);
	public class MapSelectionChangedEventArgs : MapSelectionEventArgs {
		public MapSelectionChangedEventArgs(IEnumerable<object> selection)
			: base(selection) {
		}
	}
	public delegate void MapSelectionChangingEventHandler(object sender, MapSelectionChangingEventArgs e);
	public class MapSelectionChangingEventArgs : MapSelectionEventArgs {
		public bool Cancel { get; set; }
		public MapSelectionChangingEventArgs(IEnumerable<object> selection)
			: base(selection) {
		}
	}
	public delegate void DataLoadedEventHandler(object sender, DataLoadedEventArgs e);
	public class DataLoadedEventArgs : EventArgs { 
	}
	public delegate void LayerVisibleChangedEventHandler(object sender, LayerVisibleChangedEventArgs e);
	public class LayerVisibleChangedEventArgs : EventArgs {
		readonly bool visible;
		public bool Visible { get { return visible; } }
		public LayerVisibleChangedEventArgs(bool visible) {
			this.visible = visible;
		}
	}
	public class MapItemsLoadedEventArgs : DataLoadedEventArgs {
		readonly IEnumerable<MapItem> items;
		public IEnumerable<MapItem> Items { get { return items; } }
		public MapItemsLoadedEventArgs(IEnumerable<MapItem> items) {
			this.items = items;
		}
	}
	public class ItemsLoadedEventArgs : EventArgs {
		readonly IList<MapItem> items;
		public IList<MapItem> Items { get { return items; } }
		internal ItemsLoadedEventArgs(IList<MapItem> items) {
			this.items = items;
		}
	}
	public class ClusteredEventArgs : EventArgs {
		readonly IEnumerable<MapItem> items;
		public IEnumerable<MapItem> Items { get { return items; } }
		internal ClusteredEventArgs(IEnumerable<MapItem> items) {
			this.items = items;
		}
	}
	public delegate void ItemsLoadedEventHandler(object sender, ItemsLoadedEventArgs e);
	public delegate void ClusteredEventHandler(object sender, ClusteredEventArgs e);
	public delegate void ClusteringEventHandler(object sender, EventArgs e);
	public delegate void LegendItemCreatingEventHandler(object sender, LegendItemCreatingEventArgs e);
	public class LegendItemCreatingEventArgs : EventArgs {
		readonly MapLegendBase legend;
		readonly MapLegendItemBase item;
		int index;
		public MapLegendBase Legend { get { return legend; } }
		public MapLegendItemBase Item { get { return item; } }
		public int Index { get { return index; } }
		public LegendItemCreatingEventArgs(MapLegendBase legend, int index, MapLegendItemBase item) {
			Guard.ArgumentNotNull(legend, "legend");
			Guard.ArgumentNotNull(item, "item");
			this.legend = legend;
			this.item = item;
			this.index = index;
		}
	}
	public delegate void DataAdapterChangedEventHandler(object sender, DataAdapterChangedEventArgs e);
	public class DataAdapterChangedEventArgs : EventArgs {
		MapUpdateType updateType;
		public MapUpdateType UpdateType { get { return updateType; } }
		public DataAdapterChangedEventArgs(MapUpdateType updateType) {
			this.updateType = updateType;
		}
	}
	public delegate void OverlaysArrangedEventHandler(object sender, OverlaysArrangedEventArgs e);
	public class OverlaysArrangedEventArgs : EventArgs {
		readonly OverlayArrangement[] overlayArrangements;
		public OverlayArrangement[] OverlayArrangements { get { return overlayArrangements; } }
		public OverlaysArrangedEventArgs(OverlayArrangement[] overlayArrangements) {
			this.overlayArrangements = overlayArrangements;
		}
	}
	public class OverlayArrangement {
		readonly Rectangle[] itemLayouts;
		Rectangle overlayLayout;
		public Rectangle[] ItemLayouts { get { return itemLayouts; } }
		public Rectangle OverlayLayout {
			get { return overlayLayout; }
			set { overlayLayout = value; }
		}
		void ResetOverlayLayout() {
			overlayLayout = Rectangle.Empty;
		}
		bool ShouldSerializeOverlayLayout() {
			return overlayLayout != Rectangle.Empty;
		}
		internal OverlayArrangement(Rectangle[] itemsBounds) {
			this.itemLayouts = itemsBounds;
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public abstract class PropertyChangedEventArgs<T> : EventArgs {
		T oldValue;
		T newValue;
		protected PropertyChangedEventArgs(T newValue, T oldValue) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected T OldValue { get { return oldValue; } }
		protected T NewValue { get { return newValue; } }
	}
	public delegate void NameChangedEventHandler(object sender, NameChangedEventArgs e);
	public class NameChangedEventArgs : PropertyChangedEventArgs<string> {
		public NameChangedEventArgs(string name, string oldName) : base(name, oldName) {
		}
		public string Name { get { return base.NewValue; } }
		public string OldName { get { return OldValue; } }
	}
	public delegate void KeyChangedEventHandler(object sender, KeyChangedEventArgs e);
	public class KeyChangedEventArgs : PropertyChangedEventArgs<object> {
		public KeyChangedEventArgs(object key, object oldKey)
			: base(key, oldKey) {
		}
		public object Key { get { return base.NewValue; } }
		public object OldKey { get { return OldValue; } }
	}
}
