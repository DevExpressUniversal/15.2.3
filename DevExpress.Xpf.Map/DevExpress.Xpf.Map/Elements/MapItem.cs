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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
namespace DevExpress.Xpf.Map {
	public abstract class MapItem : MapDependencyObject, IOwnedElement, IMapItem, IMapItemCore, IMapDataItem, IMapItemAttributeOwner {
		internal static readonly DependencyPropertyKey AttributesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Attributes",
			typeof(MapItemAttributeCollection), typeof(MapItem), new PropertyMetadata());
		public static readonly DependencyProperty AttributesProperty = AttributesPropertyKey.DependencyProperty;
		public static readonly DependencyProperty IsHitTestVisibleProperty = DependencyPropertyManager.Register("IsHitTestVisible",
		   typeof(bool), typeof(MapItem), new PropertyMetadata(true));
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible",
			typeof(bool), typeof(MapItem), new PropertyMetadata(true, VisiblePropertyChanged));
		public static readonly DependencyProperty ToolTipPatternProperty = DependencyPropertyManager.Register("ToolTipPattern",
			typeof(string), typeof(MapItem), new PropertyMetadata(null, ToolTipPatternPropertyChanged));
		public static readonly DependencyProperty TagProperty = DependencyPropertyManager.Register("Tag",
			typeof(object), typeof(MapItem), new PropertyMetadata(null));
		public static readonly DependencyProperty ZIndexProperty = DependencyPropertyManager.Register("ZIndex",
			typeof(int), typeof(MapItem), new PropertyMetadata(0, ZIndexChanged));
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList<MapItem> ClusteredItems {
			get {
				IClusterItem item = this as IClusterItem;
				return item != null ? item.ClusteredItems.OfType<MapItem>().ToList() : null;
			}
		}
		[Category(Categories.Data)]
		public MapItemAttributeCollection Attributes {
			get { return (MapItemAttributeCollection)GetValue(AttributesProperty); }
		}
		[Category(Categories.Behavior)]
		public bool IsHitTestVisible {
			get { return (bool)GetValue(IsHitTestVisibleProperty); }
			set { SetValue(IsHitTestVisibleProperty, value); }
		}
		[Category(Categories.Appearance)]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NonTestableProperty]
		public object Tag {
			get { return GetValue(TagProperty); }
			set { SetValue(TagProperty, value); }
		}
		[NonCategorized]
		public string ToolTipPattern {
			get { return (string)GetValue(ToolTipPatternProperty); }
			set { SetValue(ToolTipPatternProperty, value); }
		}
		[Category(Categories.Layout)]
		public int ZIndex {
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
		public Brush Brush {
			get { return GetActualBrush(); } }
		protected static void LayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapItem item = d as MapItem;
			if (item != null)
				item.UpdateLayout();
		}
		protected static void VisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapItem item = d as MapItem;
			if (item != null)
				item.UpdateVisibility();
		}
		protected static void ToolTipPatternPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapItem item = d as MapItem;
			if (item != null)
				item.UpdateToolTipPattern();
		}
		protected static void ZIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapItem item = d as MapItem;
			if(item != null)
				item.UpdateZIndex();
		}
		internal const int UndefinedRowIndex = -1;
		static long hashStore;
		MapItemInfo itemInfo;
		readonly MapItemLayout layout = new MapItemLayout();
		UIElement container = null;
		object owner;
		long hashCode;
		int rowIndex = UndefinedRowIndex;
		int[] listSourceRowIndices = new int[0];
		Color colorizerColor = MapColorizer.DefaultColor;
		public event MouseEventHandler MouseEnter;
		public event MouseEventHandler MouseLeave;
		public event MouseButtonEventHandler MouseLeftButtonDown;
		public event MouseButtonEventHandler MouseLeftButtonUp;
		public event MouseEventHandler MouseMove;
		public event MouseButtonEventHandler MouseRightButtonDown;
		public event MouseButtonEventHandler MouseRightButtonUp;
		protected abstract bool IsVisualElement { get; }
		protected int[] ListSourceRowIndices {
			get { return listSourceRowIndices; }
			set {
				int[] indices = value != null ? value : new int[0];
				listSourceRowIndices = indices;
			}
		}
		protected internal ProjectionBase Projection {
			get {
				if (Layer == null)
					return MapControl.DefaultMapProjection;
				return Layer.ActualProjection;
			}
		}
		protected internal MapCoordinateSystem CoordinateSystem { get { return Layer != null ? Layer.ActualCoordinateSystem : MapControl.DefaultCoordinateSystem; } }
		protected internal virtual MapItemAttributeCollection ActualAttributes  { get { return Attributes; } }
		protected internal MapItemLayout Layout { get { return layout; } }
		protected internal Color ColorizerColor {
			get { return colorizerColor; }
			 set {
				colorizerColor = value;
				OnColorizerColorChanged();
				NotifyPropertyChanged(this, new PropertyChangedEventArgs("Brush"));
			}
		}
		internal UIElement Container {
			get { return container; }
			set {
				UnsubscribeFromMouseEvents(container);
				container = value;
				SubscribeToMouseEvents(container);
			}
		}
		internal VectorLayerBase Layer { get { return owner as VectorLayerBase; } }
		internal MapItemInfo Info {
			get {
				return itemInfo;
			}
			set {
				if (itemInfo != value) {
					itemInfo = value;
					OnItemInfoChanged();
					NotifyPropertyChanged("Info");
				}
			}
		}
		internal long HashCode {
			get {
				return hashCode;
			}
		}
		protected internal int RowIndex { get { return rowIndex; } set { rowIndex = value; } }
		protected internal virtual object Source { get { return Tag != null ? Tag : this; } }
		protected internal abstract ControlTemplate ItemTemplate { get; }
		protected internal string ActualToolTipPattern {
			get {
				if (!string.IsNullOrEmpty(ToolTipPattern))
					return ToolTipPattern;
				else if (Layer != null && !string.IsNullOrEmpty(Layer.ToolTipPattern))
					return Layer.ToolTipPattern;
				return string.Empty;
			}
		}
		public MapItem() {
			this.SetValue(AttributesPropertyKey, new MapItemAttributeCollection(this));
			hashCode = hashStore++;
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if(owner == value)
					return;
				SetOwnerInternal(value);
				OnOwnerChanged();
			}
		}
		protected void SetOwnerInternal(object value) {
			this.owner = value;
		}
		#endregion
		#region IMapElement implementation
		void IMapItem.CompleteLayout(UIElement element) {
			CompleteLayout(element);
		}
		void IMapItem.CalculateLayout() {
			CalculateLayout();
		}
		Point IMapItem.Location { get { return layout != null ? layout.LocationInPixels : new Point(0, 0); } }
		Size IMapItem.Size { get { return layout != null ? layout.SizeInPixels : new Size(0, 0); } }
		bool IMapItem.Visible { get { return layout != null; } }
		#endregion
		#region IMapItemCore
		string IMapItemCore.Text { get { return GetTextCore(); } }
		System.Drawing.Color IMapItemCore.TextColor { get { return System.Drawing.Color.Empty; } }
		int IMapItemCore.AttributesCount { get { return Attributes.Count; } }
		void IMapItemCore.AddAttribute(IMapItemAttribute attribute) {
			Attributes.Add(new MapItemAttribute(attribute));
		}
		IMapItemAttribute IMapItemCore.GetAttribute(int index) {
			return Attributes[index];
		}
		IMapItemAttribute IMapItemCore.GetAttribute(string name) {
			return Attributes[name];
		}
		#endregion
		#region IMapDataItem Members
		int IMapDataItem.RowIndex {
			get {
				return RowIndex;
			}
			set {
				RowIndex = value;
			}
		}
		int[] IMapDataItem.ListSourceRowIndices {
			get {
				return ListSourceRowIndices;
			}
			set {
				ListSourceRowIndices = value;
			}
		}
		void IMapDataItem.AddAttribute(IMapItemAttribute attr) {
			AddAttribute(new MapItemAttribute(attr));
		}
		#endregion
		#region IMapItemAttributeOwner Members
		IMapItemAttribute IMapItemAttributeOwner.GetAttribute(string name) {
			return Attributes[name];
		}
		#endregion
		protected void AddAttribute(MapItemAttribute attribute) {
			if(attribute == null) return;
			MapItemAttribute founded = Attributes[attribute.Name];
			if(founded != null)
				Attributes.Remove(founded);
			Attributes.Add(attribute);
		}
		void SubscribeToMouseEvents(UIElement element) {
			if (element != null) {
				element.MouseEnter += new MouseEventHandler(OnMouseEnter);
				element.MouseLeave += new MouseEventHandler(OnMouseLeave);
				element.MouseMove += new MouseEventHandler(OnMouseMove);
				element.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
				element.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
				element.MouseRightButtonDown += new MouseButtonEventHandler(OnMouseRightButtonDown);
				element.MouseRightButtonUp += new MouseButtonEventHandler(OnMouseRightButtonUp);
			}
		}
		void UnsubscribeFromMouseEvents(UIElement element) {
			if (element != null) {
				element.MouseEnter -= new MouseEventHandler(OnMouseEnter);
				element.MouseLeave -= new MouseEventHandler(OnMouseLeave);
				element.MouseMove -= new MouseEventHandler(OnMouseMove);
				element.MouseLeftButtonDown -= new MouseButtonEventHandler(OnMouseLeftButtonDown);
				element.MouseLeftButtonUp -= new MouseButtonEventHandler(OnMouseLeftButtonUp);
				element.MouseRightButtonDown -= new MouseButtonEventHandler(OnMouseRightButtonDown);
				element.MouseRightButtonUp -= new MouseButtonEventHandler(OnMouseRightButtonUp);
			}
		}
		protected Brush GetActualBrush() {
			if(ColorizerColor != MapColorizer.DefaultColor)
				return GetColorizerBrush(ColorizerColor);
			return GetFill();
		}
		protected Brush GetColorizerBrush(Color color) {
			if(Layer != null && Layer.Colorizer != null)
				return Layer.Colorizer.GetBrush(color);
			return null;
		}
		protected internal virtual void ApplyAppearance() {
		}
		protected virtual Brush GetFill() { return null; }
		protected virtual void OnColorizerColorChanged() {
			ApplyAppearance();
		}
		protected virtual void OnItemInfoChanged() {
		}
		protected virtual void ColorizeCore(MapColorizer colorizer) {
			ColorizerColor = colorizer.GetItemColor(this as IColorizerElement);
		}
		protected virtual void OnOwnerChanged() {
			UpdateLayout();
			ApplyAppearance();
		}
		protected virtual string GetTextCore() {
			return string.Empty;
		}
		protected internal virtual void OnMouseEnter(object sender, MouseEventArgs e) {
			if (MouseEnter != null && IsHitTestVisible)
				MouseEnter(this, e);
		}
		protected internal virtual void OnMouseLeave(object sender, MouseEventArgs e) {
			if (MouseLeave != null && IsHitTestVisible)
				MouseLeave(this, e);
		}
		protected internal virtual void OnMouseMove(object sender, MouseEventArgs e) {
			if (MouseMove != null && IsHitTestVisible)
				MouseMove(this, e);
		}
		protected internal virtual void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			if (MouseRightButtonDown != null && IsHitTestVisible)
				MouseRightButtonDown(this, e);
		}
		protected internal virtual void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e) {
			if (MouseRightButtonUp != null && IsHitTestVisible)
				MouseRightButtonUp(this, e);
		}
		protected internal virtual void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			if (MouseLeftButtonUp != null && IsHitTestVisible)
				MouseLeftButtonUp(this, e);
		}
		protected internal virtual void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			if (MouseLeftButtonDown != null && IsHitTestVisible)
				MouseLeftButtonDown(this, e);
		}
		protected internal virtual MapItemInfo CreateInfo() {
			return new MapItemInfo(this);
		}
		protected internal virtual void SetIsHighlighted(bool value) {
			if (Info != null)
				Info.IsHighlighting = value;
		}
		protected internal virtual void SetIsSelected(bool value) {
			if (Info != null)
				Info.IsSelected = value;
		}
		protected internal abstract CoordBounds CalculateBounds();
		protected abstract void CalculateLayoutInMapUnits();
		protected abstract void CalculateLayout();
		protected virtual void CompleteLayout(UIElement element) {
			if (IsVisualElement)
				Layout.SizeInPixels = element.DesiredSize;
			else
				Layout.SizeInPixels = new Size(Math.Max(Layout.SizeInPixels.Width, element.DesiredSize.Width),
											   Math.Max(Layout.SizeInPixels.Height, element.DesiredSize.Height));
		}
		protected virtual void UpdateVisibility() {
			if (Layer != null)
				Layer.HideToolTip();
		}
		protected virtual void UpdateToolTipPattern() {
		}
		protected virtual void UpdateZIndex() {
		}
		protected internal abstract IList<CoordPoint> GetItemPoints();
		protected internal void Colorize(MapColorizer colorizer) {
			ColorizeCore(colorizer);
		}
		protected internal virtual void ResetColor() {
			ColorizerColor = MapColorizer.DefaultColor;
		}
		protected internal virtual ToolTipPatternParser CreateToolTipPatternParser() {
			return new ToolTipPatternParser(ActualToolTipPattern);
		}
		internal void UpdateLayout() {
			if (Layer != null) {
				MapUnit oldLocation = Layout.LocationInMapUnits;
				Size oldSize = Layout.SizeInMapUnits;
				CalculateLayoutInMapUnits();
				CalculateLayout();
				Layer.Invalidate();
			}
		}
	}
	public class MapItemPresenter : Control, IMapItem, IHitTestableElement {
		internal MapItemInfo Info { get; set; }
		MapItem Item { get { return Info != null ? Info.MapItem : null; } }
		IMapItem Element { get { return Item; } }
		internal Shape Shape { get; private set; }
		#region IMapItem
		void IMapItem.CompleteLayout(UIElement element) {
			if (Element != null)
				Element.CompleteLayout(element);
		}
		void IMapItem.CalculateLayout() {
			if (Element != null)
				Element.CalculateLayout();
		}
		Point IMapItem.Location { get { return Element != null ? Element.Location : new Point(0, 0); } }
		Size IMapItem.Size { get { return Element != null ? Element.Size : new Size(0, 0); } }
		bool IMapItem.Visible { get { return Element != null ? Element.Visible : false; } }
		#endregion
		#region IHitTestableElement
		Object IHitTestableElement.Element { get { return Item; } }
		bool IHitTestableElement.IsHitTestVisible { get { return Item != null ? Item.IsHitTestVisible && Item.Layer != null && Item.Layer.ActualVisibility == Visibility.Visible : false; } }
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Shape = (Shape)GetTemplateChild("PART_Shape");
			if (Info != null)
				Info.ProcessApplyTempate(this);
		}
	}
}
namespace DevExpress.Xpf.Map.Native {
	[NonCategorized]
	public class MapItemInfo : IOwnedElement, INotifyPropertyChanged {
		const int SelectedItemZIndexOffset = 2;
		const int HighlightedItemZIndexOffset = 1;
		const int ItemZIndexRangeLength = SelectedItemZIndexOffset + HighlightedItemZIndexOffset + 1;
		static int collectionLength;
		public static void UpdateCollectionLength(int value) {
			collectionLength = value;
		}
		public event PropertyChangedEventHandler PropertyChanged;
		WeakReference mapItem;
		MapItemPresenter container;
		Shape shape;
		int indexInCollection;
		bool isSelected;
		bool isHighlighting;
		protected internal bool IsFree { get { return mapItem == null; } }
		public MapItem MapItem {
			get {
				if (mapItem != null)
					return (MapItem)mapItem.Target;
				return null;
			}
			set {
				if (value != null) {
					if ((mapItem == null) || (mapItem.Target != value)) {
						mapItem = new WeakReference(value);
						if(mapItem.Target is MapShapeBase)
							((MapShapeBase)mapItem.Target).OnItemChanged(Shape);
						NotifyPropertyChanged("MapItem");
						NotifyPropertyChanged("Visibility");
					}
				} else {
					if (mapItem != null) {
						if(mapItem.Target is MapShapeBase)
							((MapShapeBase)mapItem.Target).OnItemChanged(Shape);
						mapItem = null;
						NotifyPropertyChanged("MapItem");
						NotifyPropertyChanged("Visibility");
					}
				}
			}
		}
		public bool IsSelected {
			get { return isSelected; }
			internal set {
				if (isSelected == value)
					return;
				isSelected = value;
				SetContainerZIndex();
				MapItem.ApplyAppearance();
				MapItem.UpdateLayout();
				NotifyPropertyChanged("IsSelected");
			}
		}
		public bool IsHighlighting {
			get { return isHighlighting; }
			internal set {
				if (isHighlighting == value)
					return;
				isHighlighting = value;
				MapItem.UpdateLayout();
				SetContainerZIndex();
			}
		}
		public object Source { get { return MapItem.Source; } }
		public Visibility Visibility { get { return (mapItem != null) ? Visibility.Visible : Visibility.Collapsed; } }
		public int IndexInCollection {
			get {
				return indexInCollection;
			}
			set {
				if (indexInCollection != value) {
					indexInCollection = value;
					SetContainerZIndex();
				}
			}
		}
		public MapItemInfo(MapItem item) {
			MapItem = item;
		}
		object IOwnedElement.Owner {
			get { return ((IOwnedElement)mapItem.Target).Owner; }
			set { ((IOwnedElement)mapItem.Target).Owner = value; }
		}
		protected void NotifyPropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		void SetContainerZIndex() {
			if(Container == null)
				return;
			int index = CalculateActualZIndex();
			Canvas.SetZIndex(Container, index);
		}
		protected internal int CalculateActualZIndex() {
			int index = ItemZIndexRangeLength * (IndexInCollection + collectionLength * (MapItem != null ? MapItem.ZIndex : 0));
			if(IsSelected)
				index += SelectedItemZIndexOffset;
			if(IsHighlighting)
				index += HighlightedItemZIndexOffset;
			return index;
		}
		public Shape Shape {
			get {
				return shape;
			}
			private set {
				if (shape != value) {
					shape = value;
					if((MapItem != null) && (MapItem is MapShapeBase))
						((MapShapeBase)MapItem).OnItemChanged(Shape);
					NotifyPropertyChanged("Shape");
				}
			}
		}
		public MapItemPresenter Container {
			get {
				return container;
			}
			private set {
				if (container != value) {
					container = value;
					if (MapItem != null)
						MapItem.Container = container;
					SetContainerZIndex();
					NotifyPropertyChanged("Container");
				}
			}
		}
		internal void ProcessApplyTempate(MapItemPresenter container) {
			Container = container;
			Shape = container.Shape;
		}
	}
	public class MapItemLayout {
		CoordPoint location;
		MapUnit locationInMapUnits;
		Point locationInPixels;
		Size size;
		Size sizeInMapUnits;
		Size sizeInPixels;
		bool visible = true;
		public CoordPoint Location { get { return location; } set { location = value; } }
		public MapUnit LocationInMapUnits { get { return locationInMapUnits; } set { locationInMapUnits = value; } }
		public Point LocationInPixels { get { return locationInPixels; } set { locationInPixels = value; } }
		public Size Size { get { return size; } set { size = value; } }
		public Size SizeInMapUnits { get { return sizeInMapUnits; } set { sizeInMapUnits = value; } }
		public Size SizeInPixels { get { return sizeInPixels; } set { sizeInPixels = value; } }
		public bool Visible { get { return visible; } set { visible = value; } }
	}
}
