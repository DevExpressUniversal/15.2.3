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
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Internal;
using System.Collections.ObjectModel;
using DevExpress.Utils;
using System.Windows.Documents;
using System.ComponentModel;
using DevExpress.Diagram.Core.Themes;
using DevExpress.Utils.Serializing;
using DevExpress.Diagram.Core.Serialization;
using System.IO;
using System.Globalization;
using DevExpress.Diagram.Core.TypeConverters;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Diagram.Core.Base;
namespace DevExpress.Diagram.Core {
	public class DiagramItemController {
		public static ReadOnlyCollection<T> GetEmptyItems<T>() where T : IDiagramItem {
			return new ReadOnlyCollection<T>(EmptyArray<T>.Instance);
		}
		public readonly IDiagramItem Item;
		public IDiagramItem Owner { get; private set; }
		protected internal virtual void SetOwner(IDiagramItem owner) {
			Owner = owner;
		}
		protected IDiagramControl Diagram { get { return Item.GetRootDiagram(); } }
		public Rect Bounds {
			get { return new Rect(Item.Position, Item.Size); }
			set {
				if(Item.Position != value.Location)
					Item.Position = value.Location;
				var newSize = value.Size;
				if(Item.Size != newSize)
					Item.Size = newSize;
			}
		}
		public void OnPositionChanged(Point oldValue) {
			UpdateBoundaryProperties(() => OnPositionChangedCore(oldValue));
		}
		protected virtual void OnPositionChangedCore(Point oldValue) {
			OnBoundsChangedCore();
		}
		public void OnSizeChanged(Size oldValue) {
			UpdateBoundaryProperties(() => OnSizeChangedCore(oldValue));
		}
		protected virtual void OnSizeChangedCore(Size oldValue) {
			OnBoundsChangedCore();
		}
		protected IDiagramRoot RootItem { get { return Diagram.RootItem(); } }
		public ISelectionLayer CoerceSelectionLayer(ISelectionLayer layer) {
			return layer ?? DefaultSelectionLayer.Instance;
		}
		public virtual void OnActualSizeChanged() {
			Item.NotifyChanged(ItemChangedKind.Bounds);
		}
		Locker boundaryLocker = new Locker();
		protected void UpdateBoundaryProperties(Action update) {
			boundaryLocker.DoLockedActionIfNotLocked(update);
		}
		TabOrderProvider tabOrderProvider;
		public TabOrderProvider TabOrderProvider { get { return tabOrderProvider ?? (tabOrderProvider = CreateTabOrderProvider()); } }
		PropertyDescriptor[] editableProperties;
		public PropertyDescriptor[] EditableProperties { get { return editableProperties ?? (editableProperties = Item.GetEditableProperties().ToArray()); } }
		protected virtual TabOrderProvider CreateTabOrderProvider() { return new TabOrderProvider(() => Item.NestedItems, x => x.GetIndexInOwnerCollection()); }
		public DiagramItemController(IDiagramItem item) {
			this.Item = item;
		}
		public virtual bool CanDeleteCore() {
			return TrueForItemAndChildren(x => x.CanDelete, x => x.CanDeleteCore());
		}
		public virtual bool CanCopyCore() {
			return TrueForItemAndChildren(x => x.CanCopy, x => x.CanCopyCore());
		}
		internal virtual bool ActualCanSelect { get { return Item.CanSelect; } }
		protected virtual bool CanHaveNoChildren { get { return true; } }
		protected internal virtual IEnumerable<IDiagramItem> GetRelatedItems() {
			var connectors = RootItem.NestedItems
				.OfType<IDiagramConnector>()
				.Where(x => x.BeginItem == Item || x.EndItem == Item);
			var parents = Item.GetParents().TakeWhile(x => !x.Controller.CanHaveNoChildren && x.NestedItems.Count == 1);
			return connectors.Concat(parents);
		}
		protected internal virtual void BeforeDelete(Transaction transaction) {
			GetRelatedItems().ToArray().ForEach(x => transaction.RemoveItem(x));
		}
		public virtual IInputElement CreateInputElement() {
			return new DiagramItemInputElement(() => Item);
		}
		bool TrueForItemAndChildren(Predicate<IDiagramItem> itemPredicate, Predicate<DiagramItemController> childrenPredicate) {
			return itemPredicate(Item) && Item.NestedItems.All(x => childrenPredicate(x.Controller));
		}
		protected internal virtual IEnumerable<IAdorner> CreateAdorners() {
			yield break;
		}
		protected virtual void OnBoundsChangedCore() {
			Item.IterateSelfAndChildren(x => x.NotifyChanged(ItemChangedKind.Bounds));
		}
		public IEnumerable<PropertyDescriptor> GetProxyDescriptors(Func<IDiagramItem, object> getRealComponent) {
			return Item.GetRootDiagram().Controller.GetProxyDescriptors(Item, getRealComponent);
		}
		public virtual IEnumerable<Point> GetConnectionPoints() {
			return Enumerable.Empty<Point>();
		}
		public IEnumerable<PropertyDescriptor> GetFontTraitsProperties() {
			if(GetFontTraits() == null)
				return Enumerable.Empty<PropertyDescriptor>();
			return GetProxyDescriptors(x => x.Controller.GetFontTraits());
		}
		public virtual IEnumerable<Point> GetIntersectionPoints(Point point1, Point point2) {
			var points = Item.RotatedDiagramBounds().GetPoints();
			int count = points.Count();
			for(int i = 0; i < count; i++) {
				Point? intersectionPoint = MathHelper.GetIntersectionPoint(points[i], points[(i + 1) % count], point1, point2);
				if(intersectionPoint.HasValue)
					yield return intersectionPoint.Value;
			}
		}
		public virtual PropertyDescriptor GetTextProperty() {
			return null;
		}
		public virtual void RestoreRelations(Func<DiagramItemFinderPath, IDiagramItem> getItem) {
		}
		public virtual void PrepareRelations(Func<IDiagramItem, DiagramItemFinderPath> getPath) {
		}
		#region Clone
		public Func<IDiagramItem> GetCloneInfo(IDiagramControl diagram) {
			var itemInfo = diagram.SerializeItems(Item.Yield().ToList(), StoreRelationsMode.RelativeToDiagram);
			return () => diagram.DeserializeItems(itemInfo, storeRelationsMode: null).Single();
		}
		public IEnumerable<PropertyDescriptor> GetCloneableProperties() {
			return GetItemProperties()
				.Where(x => x.ShouldSerializeValue(Item))
				;
		}
		public virtual IEnumerable<PropertyDescriptor> GetItemProperties() {
			var col = GetInterfaceProperties()
				.Concat(GetProxyProperties());
			return col;
		}
		IEnumerable<PropertyDescriptor> GetInterfaceProperties() {
			return Item.GetType().GetInterfaces()
				 .Where(x => typeof(IDiagramItem).IsAssignableFrom(x))
				 .SelectMany(x => TypeDescriptor.GetProperties(x).Cast<PropertyDescriptor>())
				 .Where(x => x.Attributes[typeof(XtraSerializableProperty)] != null)
				 .Select(x => new CombinedPropertyDescriptor(x, Item.GetRealProperty(x)))
				 ;
		}
		protected virtual IEnumerable<PropertyDescriptor> GetProxyProperties() {
			yield return ProxyPropertyDescriptor.Create(ExpressionHelper.GetProperty((DiagramItemController x) => x.AdditionalStyles), (IDiagramItem x) => x.Controller);
			yield return ProxyPropertyDescriptor.Create(ExpressionHelper.GetProperty((DiagramItemController x) => x.Children), (IDiagramItem x) => x.Controller);
			yield return ProxyPropertyDescriptor.Create(ExpressionHelper.GetProperty((DiagramItemController x) => x.ItemKind), (IDiagramItem x) => x.Controller);
		}
		#region XtraSerializer
		public static IDiagramItem CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			var diagram = ((DiagramDeserializingItemsContainer)e.RootObject).Diagram;
			XtraPropertyInfo xtraProp = e.Item;
			if(xtraProp != null && xtraProp.ChildProperties != null) {
				string itemKind = (string)xtraProp.ChildProperties[DiagramItemTypeRegistrator.ItemKindProperty].Value;
				if(diagram.IsRootItemKind(itemKind)) {
					return diagram.RootItem();
				}
				IDiagramItem item = diagram.ItemTypeRegistrator.Create(itemKind, e);
				return item;
			}
			return null;
		}
		public static void SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			XtraPropertyInfo xtraProp = e.Item;
			IDiagramItem item = e.Item.Value as IDiagramItem;
			if(item != null) {
				IList<IDiagramItem> childrenCol = (IList<IDiagramItem>)e.Collection;
				childrenCol.Add(item);
			}
		}
		#endregion
		public IEnumerable<PropertyDescriptor> GetColorProperties() {
			yield return ProxyPropertyDescriptor.Create(ExpressionHelper.GetProperty((DiagramItemController x) => x.BackgroundColor), (IDiagramItem x) => x.Controller);
			yield return ProxyPropertyDescriptor.Create(ExpressionHelper.GetProperty((DiagramItemController x) => x.ForegroundColor), (IDiagramItem x) => x.Controller);
			yield return ProxyPropertyDescriptor.Create(ExpressionHelper.GetProperty((DiagramItemController x) => x.StrokeColor), (IDiagramItem x) => x.Controller);
			yield return ProxyPropertyDescriptor.Create(ExpressionHelper.GetProperty((DiagramItemController x) => x.StyleInfo), (IDiagramItem x) => x.Controller);
		}
		#endregion
		public virtual IFontTraits GetFontTraits() {
			return null;
		}
		public virtual Rect GetInplaceEditAdornerBounds() {
			return Item.ActualDiagramBounds();
		}
		internal virtual void ChildChanged(IDiagramItem child, ItemChangedKind kind) {
		}
		internal static IUpdatableAdorner CreateUpdatableSelectionAdorner(IDiagramControl diagram, DefaultSelectionLayerHandler handler, double? multipleSelectionMargin) {
			return diagram.AdornerFactory().CreateSelectionAdorner(handler, multipleSelectionMargin)
				.AsUpdatableAdorner(model => {
					model.CanResize = diagram.GetResizableSelectedItems().Any();
					model.CanRotate = diagram.SelectedItems().Any(item => item.CanRotate);
					model.ShowFullUI = !diagram.SelectedItems().IsEmptyOrSingle() || diagram.ActiveTool.ShowFullSelectionUI;
				});
		}
		internal virtual Func<IDiagramControl, DefaultSelectionLayerHandler, IUpdatableAdorner> SingleSelectionAdornerFactory {
			get { return (diagram, handler) => CreateUpdatableSelectionAdorner(diagram, handler, null); }
		}
		internal virtual Func<IAdornerFactory, bool, IUpdatableAdorner> SelectionPartAdornerFactory {
			get { return (factory, isPrimarySelection) => factory.CreateSelectionPartAdorner(Item, isPrimarySelection).AsUpdatableAdorner(model => { }); }
		}
		protected internal virtual IAdorner CreateDragPreviewAdorner(IDiagramControl diagram) {
			return diagram.AdornerFactory().CreateItemDragPreview(Item);
		}
		public void OnPaddingChanged() {
		}
		public void OnAngleChanged() {
			Item.IterateSelfAndChildren(x => x.NotifyChanged(ItemChangedKind.Bounds));
		}
		#region Serialization
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public IList<IDiagramItem> Children {
			get { return Item.NestedItems; }
		}
		bool ShouldSerializeChildren() { return Children.Count > 0; }
		[XtraSerializableProperty]
		public string ItemKind { get { return DiagramItemTypeRegistrator.GetItemKind(Item); } }
		bool ShouldSerializeItemKind() { return true; }
		#endregion
		#region Themes
		AdditionalStyleList additionalStyles = new AdditionalStyleList();
		[XtraSerializableProperty]
		public AdditionalStyleList AdditionalStyles {
			get { return additionalStyles; }
			set {
				if(additionalStyles == value)
					return;
				additionalStyles = value;
				UpdateCustomStyle();
			}
		}
		bool ShouldSerializeAdditionalStyles() { return AdditionalStyles.Count > 0; }
		[Browsable(false)]
		public DiagramItemEditUnit BackgroundColor {
			get { return DiagramItemEditUnit.CreateEditUnit(Item, DiagramItemEditUnit.BackgroundProperties); }
			set { DiagramItemEditUnit.ApplyEditUnit(Item, value, DiagramItemEditUnit.BackgroundProperties); }
		}
		[Browsable(false)]
		public DiagramItemEditUnit ForegroundColor {
			get { return DiagramItemEditUnit.CreateEditUnit(Item, DiagramItemEditUnit.ForegroundProperties); }
			set { DiagramItemEditUnit.ApplyEditUnit(Item, value, DiagramItemEditUnit.ForegroundProperties); }
		}
		[Browsable(false)]
		public DiagramItemEditUnit StrokeColor {
			get { return DiagramItemEditUnit.CreateEditUnit(Item, DiagramItemEditUnit.StrokeProperties); }
			set { DiagramItemEditUnit.ApplyEditUnit(Item, value, DiagramItemEditUnit.StrokeProperties); }
		}
		[Browsable(false)]
		public DiagramItemEditUnit StyleInfo {
			get { return DiagramItemEditUnit.CreateStyleEditUnit(Item); }
			set { DiagramItemEditUnit.ApplyStyleEditUnit(Item, value); }
		}
		public void AddStyleId(object id) {
			AdditionalStyles.Add(id);
			UpdateCustomStyle();
		}
		public void RemoveStyleId(object id) {
			AdditionalStyles.Remove(id);
			UpdateCustomStyle();
		}
		public void UpdateCustomStyle() {
			Diagram.Do(x => x.Controller.UpdateItemCustomStyle(Item));
		}
		public virtual DiagramItemStyleId GetDefaultStyleId() {
			return null;
		}
		public virtual ReadOnlyCollection<DiagramItemStyleId> GetDiagramItemStylesId() {
			return null;
		}
		internal DiagramItemStyle CorrectThemeStyle(Themes.DiagramItemStyle style, DiagramColorPalette palette) {
			if(style == null)
				return null;
			var brush = DiagramItemStyleHelper.CorrectBrush(style.Brush, GetAllowLightForeground(), GetAllowLightStroke());
			brush = DiagramItemStyleHelper.CorrectBrush(palette, brush, Item.ForegroundId, Item.BackgroundId, Item.StrokeId);
			return style.ReplaceBrush(brush);
		}
		protected virtual bool GetAllowLightForeground() {
			return true;
		}
		protected virtual bool GetAllowLightStroke() {
			return true;
		}
		#endregion
		protected internal virtual AdjustBoundaryBehavior GetAdjustBoundaryBehavior(Direction direction) {
			return AdjustBoundaryBehavior.None;
		}
		protected internal virtual Rect GetMaxChildBounds(IDiagramItem child, Direction direction) {
			return Item.ClientDiagramBounds();
		}
		internal virtual Size GetMinResizingSize(IEnumerable<Direction> directions) {
			return AnchorsHelper.GetMinResizingSize(
				item: Item,
				getChildren: x => x.NestedItems,
				getRect: x => x.RotatedBounds().RotatedRect,
				getMinSize: x => x.MinSize,
				getAnchors: x => x.Anchors,
				shouldProtectDirection: (x, direction) => x.Controller.GetAdjustBoundaryBehavior(direction) != AdjustBoundaryBehavior.None,
				getChildMinResizingSize: (x, direction) => x.Controller.GetMinResizingSize(directions),
				directions: directions
			);
		}
		internal virtual AdjustmentBase CreateLayoutAdjustment(Item_Owner_Bounds moveInfo, Func<IDiagramItem, bool> isMovingItem) {
			return new BoundsOwnerAngleAdjustment(moveInfo.Item, moveInfo.DiagramBounds, moveInfo.Owner, moveInfo.Index, moveInfo.Angle);
		}
		#region keep relations
		Locker disconnectLocker = new Locker();
		internal void DetachFromConnector(IDiagramConnector connector) {
			disconnectLocker.DoLockedActionIfNotLocked(() => {
				if(connector.BeginItem == Item)
					connector.MakeFree(ConnectorPointType.Begin);
				if(connector.EndItem == Item)
					connector.MakeFree(ConnectorPointType.End);
			});
		}
		internal void MoveToOwner(IDiagramItem newOwner, int index) {
			disconnectLocker.DoLockedAction(() => {
				Item.OwnerCollection().RemoveAt(Item.GetIndexInOwnerCollection());
				newOwner.NestedItems.Insert(index, Item);
			});
		} 
		#endregion
	}
	public class DiagramContainerControllerBase : DiagramItemController {
		public DiagramContainerControllerBase(IDiagramItem item)
			: base(item) {
		}
	}
	public class DiagramContainerController : DiagramContainerControllerBase {
		new IDiagramContainer Item { get { return (IDiagramContainer)base.Item; } }
		public DiagramContainerController(IDiagramContainer item)
			: base(item) {
		}
		public override IInputElement CreateInputElement() {
			return new ContainerItemInputElement(Item);
		}
		public override DiagramItemStyleId GetDefaultStyleId() {
			return DefaultDiagramStyleId.Variant1;
		}
		protected internal override IEnumerable<IDiagramItem> GetRelatedItems() {
			return base.GetRelatedItems()
				.Concat(Item.GetChildren().SelectMany(x => x.Controller.GetRelatedItems()));
		}
		protected internal override AdjustBoundaryBehavior GetAdjustBoundaryBehavior(Direction direction) {
			return Item.AdjustBoundsBehavior;
		}
		protected internal override Rect GetMaxChildBounds(IDiagramItem child, Direction direction) {
			var clientBounds = base.GetMaxChildBounds(child, direction);
			var adjustBehavior = GetAdjustBoundaryBehavior(direction);
			if(adjustBehavior == AdjustBoundaryBehavior.DisableOutOfBounds)
				return direction.SetSide(MathHelper.InfiniteRect, direction.GetSide(clientBounds));
			return MathHelper.InfiniteRect;
		}
		public override ReadOnlyCollection<DiagramItemStyleId> GetDiagramItemStylesId() {
			return DiagramShapeStyleId.Styles;
		}
	}
	public class DiagramRootController : DiagramContainerController {
		internal override bool ActualCanSelect { get { return !Diagram.AllowEmptySelection || base.ActualCanSelect; } }
		public DiagramRootController(IDiagramRoot item)
			: base(item) {
		}
		public override IInputElement CreateInputElement() {
			return null;
		}
		protected internal override IEnumerable<IAdorner> CreateAdorners() {
			yield return Diagram.AdornerFactory().CreateBackground();
			yield return Diagram.AdornerFactory().CreateHRuler().MakeTopmostEx();
			yield return Diagram.AdornerFactory().CreateVRuler().MakeTopmostEx();
		}
		public override ReadOnlyCollection<DiagramItemStyleId> GetDiagramItemStylesId() {
			return null;
		}
		protected override IEnumerable<PropertyDescriptor> GetProxyProperties() {
			return base.GetProxyProperties().Concat(GetProxyDiagramProperties().Select(x => x.AddAttributes(new XtraSerializableProperty())));
		}
		public IEnumerable<PropertyDescriptor> GetProxyDiagramProperties() {
			yield return ProxyPropertyDescriptor.Create(ExpressionHelper.GetProperty((IDiagramControl x) => x.Theme), (IDiagramItem x) => x.GetRootDiagram());
			yield return ProxyPropertyDescriptor.Create(ExpressionHelper.GetProperty((IDiagramControl x) => x.PageSize), (IDiagramItem x) => x.GetRootDiagram());
			yield return ProxyPropertyDescriptor.Create(ExpressionHelper.GetProperty((IDiagramControl x) => x.CanvasSizeMode), (IDiagramItem x) => x.GetRootDiagram());
		}
	}
	[TypeConverter(typeof(AdditionalStyleListTypeConverter))]
	public class AdditionalStyleList : List<object> {
	}
	public class TabOrderProvider {
		public readonly Func<IList<IDiagramItem>> GetChildren;
		public readonly Func<IDiagramItem, int> IndexOf;
		public TabOrderProvider(Func<IList<IDiagramItem>> getChildren, Func<IDiagramItem, int> indexOf) {
			this.GetChildren = getChildren;
			this.IndexOf = indexOf;
		}
	}
}
