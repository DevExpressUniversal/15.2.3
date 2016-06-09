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
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
	public class XRLayersHost : LayersHost {
		static XRLayersHost() {
			DependencyPropertyRegistrator<XRLayersHost>.New()
				.OverrideDefaultStyleKey()
			;
		}
	}
	public class XRVerticalRuler : VerticalRuler {
		double baseLeft = 0.0;
		double borderThickness = 0.0;
		public XRVerticalRuler(XRDiagramControl diagram) : base(diagram) { }
		protected override Rect GetBounds(Adorner adorner) {
			var width = ((XRDiagramControl)diagram).RulerWidth;
			var bounds = GetBoundsDefault(adorner);
			var background = adorner.AdornerElement as XRVerticalRulerScaleBackground;
			double rightMargin = 0.0;
			if(background != null)
				borderThickness = background.BorderThickness.Right;
			else if(adorner.AdornerElement is XRVerticalRulerScale)
				baseLeft = bounds.Left;
			else
				rightMargin = borderThickness;
			bounds = Orientation.Horizontal.SetSize(bounds, width - rightMargin);
			bounds = Orientation.Horizontal.SetLocation(bounds, baseLeft - width);
			return bounds;
		}
	}
	public class XRHorizontalRuler : HorizontalRuler {
		double baseTop = 0.0;
		public XRHorizontalRuler(XRDiagramControl diagram) : base(diagram) { }
		protected override Rect GetBounds(Adorner adorner) {
			var height = ((XRDiagramControl)diagram).RulerWidth;
			var bounds = GetBoundsDefault(adorner);
			if(adorner.AdornerElement is XRHorizontalRulerScale)
				baseTop = bounds.Top;
			bounds = Orientation.Vertical.SetSize(bounds, height);
			bounds = Orientation.Vertical.SetLocation(bounds, baseTop - height);
			return bounds;
		}
	}
	public class XRDiagramControl : DiagramControlWithRenderLayer, ILogicalOwner {
		public void SetHasChanges(bool hasChanges) {
			HasChanges = hasChanges;
		}
		protected override VerticalRuler CreateVerticalRuler() {
			return new XRVerticalRuler(this);
		}
		protected override HorizontalRuler CreateHorizontalRuler() {
			return new XRHorizontalRuler(this);
		}
		protected override void SetLayers(LayerInfo[] layers, HorizontalRuler hRuler, VerticalRuler vRuler) {
			base.SetLayers(layers.Concat(new LayerInfo(vRuler).Yield()).Concat(new LayerInfo(hRuler).Yield()).ToArray(), null, null);
		}
		class XRDiagramController : DiagramControllerEx {
			XRDiagramControl XRDiagram { get { return (XRDiagramControl)Diagram; } }
			readonly ImmediateSingleActionManager singleActionManager;
			public XRDiagramController(XRDiagramControl diagram)
				: base(diagram) {
				singleActionManager = new ImmediateSingleActionManager(() => {
					Diagram.SelectionModel.Do(x => x.RaisePropertyChanged());
				});
			}
			protected sealed override IDiagramItem CoerceInsertTarget(IDiagramItem suggestedTarget, IEnumerable<IDiagramItem> items) {
				var finalTarget = CoerceInsertTargetCore((DiagramContainerBase)suggestedTarget, items.Cast<DiagramItem>());
				if(items.Cast<DiagramItem>().Any(x => !XRDiagram.CanAddItem(finalTarget, x)))
					return null;
				return finalTarget;
			}
			DiagramContainerBase CoerceInsertTargetCore(DiagramContainerBase suggestedTarget, IEnumerable<DiagramItem> items) {
				if(items.FirstOrDefault() is XRCrossBandControlDiagramItem)
					return XRDiagram.RootItem;
				if(suggestedTarget is XRDiagramRoot)
					return XRDiagram.Items.OfType<BandDiagramItem>().FirstOrDefault(x => x.BandKind == BandKind.Detail);
				if(suggestedTarget is XRTableRowDiagramItem)
					return (XRTableCellDiagramItem)suggestedTarget.Items.FirstOrDefault();
				if(suggestedTarget is XRTableDiagramItem)
					return (XRTableCellDiagramItem)((XRTableRowDiagramItem)suggestedTarget.Items.FirstOrDefault()).With(x => x.Items.FirstOrDefault());
				return suggestedTarget;
			}
			public override IEnumerable<PropertyDescriptor> GetProxyDescriptors<T>(T item, Func<T, object> getRealComponent, ITypeDescriptorContext realComponentContext = null, TypeConverter realComponentOwnerPropertyConverter = null, Attribute[] attributes = null) {
				return XRProxyPropertyDescriptor.GetXRProxyDescriptors(XRDiagram.RootItem, item, getRealComponent, realComponentContext, realComponentOwnerPropertyConverter, attributes);
			}
			protected override void OnItemAdded(IDiagramItem item) {
				base.OnItemAdded(item);
				var diagramItem = (DiagramItem)item;
				XRDiagramItemBase.GetOnAttachItemCallback(diagramItem).Do(x => x());
				XRDiagram.CreateRegion(diagramItem);
				XRDiagramItemBase.SetDiagram(diagramItem, XRDiagram);
				XRDiagram.UpdateBandNestingLevelsOnItemAdd(diagramItem);
				(item.Owner() as BandDiagramItem).Do(x => x.InvalidateChild(diagramItem));
			}
			protected override void OnItemRemoved(IDiagramItem item) {
				base.OnItemRemoved(item);
				var diagramItem = (DiagramItem)item;
				XRDiagramItemBase.GetOnDetachItemCallback(diagramItem).Do(x => x());
				XRDiagram.DestroyRegion(diagramItem);
				XRDiagramItemBase.SetDiagram(diagramItem, null);
				XRDiagram.UpdateBandNestingLevelsOnItemRemove(diagramItem);
			}
			static readonly ZoomHelper zoomHelper = new ZoomHelper(1, 30);
			int accumulatedDelta = 0;
			protected override double ModifyZoomFactor(double delta) {
				accumulatedDelta += (int)delta;
				int accumulatedDeltaFullCycles = accumulatedDelta < 0 ? -(-accumulatedDelta / 120) : accumulatedDelta / 120;
				accumulatedDelta -= accumulatedDeltaFullCycles * 120;
				if(accumulatedDeltaFullCycles != 0)
					return zoomHelper.ModZoomFactor(Diagram.ZoomFactor, accumulatedDeltaFullCycles);
				return Diagram.ZoomFactor;
			}
			protected override void RaiseSelectionModelPropertiesChanged() {
				singleActionManager.RaiseAction();
			}
			protected override void SerializeItems(IList<IDiagramItem> items, Stream stream, StoreRelationsMode storeRelationsMode) {
				if(items.Count == 1 && items[0] == RootItem)
					throw new NotSupportedException();
				var serializeCallback = XRDiagram.SerializeCallback;
				if(serializeCallback == null)
					throw new InvalidOperationException();
				serializeCallback(items.Cast<DiagramItem>(), stream);
			}
			protected override IList<IDiagramItem> DeserializeItems(Stream stream, StoreRelationsMode? storeRelationsMode) {
				var deserializeCallback = XRDiagram.DeserializeCallback;
				if(deserializeCallback == null)
					throw new InvalidOperationException();
				return deserializeCallback(stream).Cast<IDiagramItem>().ToList();
			}
		}
		public static readonly DependencyProperty XRCommandsProperty;
		public static readonly DependencyProperty MaxBandNestingLevelProperty;
		public static readonly DependencyProperty BandHeaderLevelWidthProperty;
		public static readonly DependencyProperty RulerWidthProperty;
		static XRDiagramControl() {
			AssemblyInitializer.Init();
			DependencyPropertyRegistrator<XRDiagramControl>.New()
				.Register(d => d.RulerWidth, out RulerWidthProperty, 20.0, d => d.UpdateScrollGap())
				.Register(d => d.XRCommands, out XRCommandsProperty, null)
				.Register(d => d.MaxBandNestingLevel, out MaxBandNestingLevelProperty, 0)
				.Register(d => d.BandHeaderLevelWidth, out BandHeaderLevelWidthProperty, 30.0, d => d.UpdateScrollGap())
				.OverrideMetadata(ResizingModeProperty, ResizingMode.Preview)
				.OverrideMetadata(AllowEmptySelectionProperty, false)
				.OverrideMetadata(CanvasSizeModeProperty, CanvasSizeMode.None)
				.OverrideDefaultStyleKey()
			;
		}
		public Action<IEnumerable<DiagramItem>, Stream> SerializeCallback { get; set; }
		public Func<Stream, IEnumerable<DiagramItem>> DeserializeCallback { get; set; }
		public XRDiagramControl() {
			Controller.KeepVerticalOffset = true;
			bandHeaderAdornerLayer = CreateAdornerLayer();
			Theme = null;
		}
		readonly AdornerLayer bandHeaderAdornerLayer;
		DiagramItem selectedItem;
		protected override void RaiseSelectionChanged() {
			base.RaiseSelectionChanged();
			var newSelectedItem = PrimarySelection;
			if(newSelectedItem == selectedItem) return;
			var oldSelectedItem = selectedItem;
			selectedItem = newSelectedItem;
			oldSelectedItem.With(x => XRDiagramItemBase.GetOnIsSelectedChangedAction(x)).Do(x => x());
			newSelectedItem.With(x => XRDiagramItemBase.GetOnIsSelectedChangedAction(x)).Do(x => x());
		}
		public BaseXRCommands XRCommands {
			get { return (BaseXRCommands)GetValue(XRCommandsProperty); }
			set { SetValue(XRCommandsProperty, value); }
		}
		public IAdorner CreateBandHeaderAdorner(FrameworkElement diagramItem) {
			return bandHeaderAdornerLayer.CreateAdorner(diagramItem);
		}
		protected override DiagramControllerEx CreateControllerEx() {
			return new XRDiagramController(this);
		}
		public Func<IComponent, DiagramItem> ItemFactory { get; set; }
		protected override DiagramRoot CreateRootItem() {
			return new XRDiagramRoot(this);
		}
		public new XRDiagramRoot RootItem { get { return (XRDiagramRoot)base.RootItem; } }
		public double RulerWidth {
			get { return (double)GetValue(RulerWidthProperty); }
			set { SetValue(RulerWidthProperty, value); }
		}
		public int MaxBandNestingLevel {
			get { return (int)GetValue(MaxBandNestingLevelProperty); }
			set { SetValue(MaxBandNestingLevelProperty, value); }
		}
		public double BandHeaderLevelWidth {
			get { return (double)GetValue(BandHeaderLevelWidthProperty); }
			set { SetValue(BandHeaderLevelWidthProperty, value); }
		}
		List<int> bandsCountByBandNestingLevel = new List<int>();
		void UpdateBandNestingLevelsOnItemAdd(DiagramItem item) {
			var band = item as BandDiagramItem;
			if(band == null) return;
			var parentBand = band.Owner() as BandDiagramItem;
			band.BandNestingLevel = parentBand == null ? 0 : parentBand.BandNestingLevel + 1;
			for(int i = 0; i < band.BandNestingLevel - bandsCountByBandNestingLevel.Count + 1; ++i)
				bandsCountByBandNestingLevel.Add(0);
			++bandsCountByBandNestingLevel[band.BandNestingLevel];
			UpdateMaxBandNestingLevel();
		}
		void UpdateBandNestingLevelsOnItemRemove(DiagramItem item) {
			var band = item as BandDiagramItem;
			if(band == null) return;
			--bandsCountByBandNestingLevel[band.BandNestingLevel];
			while(bandsCountByBandNestingLevel.Count != 0 && bandsCountByBandNestingLevel[bandsCountByBandNestingLevel.Count - 1] == 0)
				bandsCountByBandNestingLevel.RemoveAt(bandsCountByBandNestingLevel.Count - 1);
			band.BandNestingLevel = -1;
			UpdateMaxBandNestingLevel();
		}
		void UpdateMaxBandNestingLevel() {
			MaxBandNestingLevel = bandsCountByBandNestingLevel.Count - 1;
			UpdateScrollGap();
		}
		void UpdateScrollGap() {
			ScrollGap = new Thickness((MaxBandNestingLevel + 1) * BandHeaderLevelWidth + RulerWidth, RulerWidth, 0.0, 0.0);
		}
		bool CanAddItem(DiagramContainerBase container, DiagramItem item) {
			var xrDiagramContainer = container as XRDiagramContainer;
			return xrDiagramContainer == null || xrDiagramContainer.CanAddItem(item);
		}
		IEnumerable<BarItem> contextMenu;
		protected override IEnumerable<IBarManagerControllerAction> CreateContextMenu() {
			Func<DiagramItem, IEnumerable<BarItem>> getItems = item => XRDiagramItemBase.GetContextMenu(item).With(x => x.Items.Cast<BarItem>().ToList()) ?? Enumerable.Empty<BarItem>();
			var primaryMenu = getItems(PrimarySelection);
			var menus = SelectedItems.Select(getItems).Select(menu => menu.Select(x => x.Name).Where(x => x != null).GroupBy(x => x).ToDictionary(x => x.Key, x => true)).ToList();
			var dataContext = new XRDiagramContextMenuItemsData(this);
			contextMenu = primaryMenu
				.Where(item => (item.Name == null || menus.All(x => x.ContainsKey(item.Name))))
				.SkipWhile(x => x is BarItemSeparator)
				.Reverse()
				.SkipWhile(x => x is BarItemSeparator)
				.Reverse()
				.ToList();
			foreach(var item in contextMenu) {
				if(item.Name == XRDiagramControlBarItemNames.SmartTag) {
					SetDataContext(item, ContextMenuViewModel.Create(dataContext));
					continue;
				}
				SetDataContext(item, dataContext);
				if(item is BarSubItem) {
					var subItem = (BarSubItem)item;
					foreach(BarItem subMenuItem in subItem.Items)
						SetDataContext(subMenuItem, dataContext);
				}
			}
			return contextMenu.Select(x => x.CreateLink());
		}
		void SetDataContext(BarItem item, object dataContext) {
			if(item == null) return;
			item.DataContext = dataContext;
		}
		protected override void DestroyContextMenu() {
			if(contextMenu == null) return;
			foreach(var item in contextMenu) {
				if(item.Name == XRDiagramControlBarItemNames.SmartTag) {
					var dataContext = (ContextMenuViewModel)item.DataContext;
					dataContext.DataContext = null;
					SetDataContext(item, null);
				}
				SetDataContext(item, null);
				if(item is BarSubItem) {
					var subItem = (BarSubItem)item;
					foreach(BarItem subMenuItem in subItem.Items)
						SetDataContext(subMenuItem, null);
				}
			}
			contextMenu = null;
		}
		protected override IEnumerable<LayerInfo> GetAdornerLayers() {
			var layerInfo = new LayerInfo(bandHeaderAdornerLayer);
			return base.GetAdornerLayers().Concat(layerInfo.Yield());
		}
		protected override MenuController CreateMenuController() {
			return new XRMenuController(this);
		}
		protected class XRMenuController : MenuController {
			public XRMenuController(DiagramControl diagram) : base(diagram) { }
			protected override void OnShowMenu(DiagramMenuPlacement placement) {
				Menu.PlacementTarget = this.Diagram.PrimarySelection ?? (DiagramItem)this.Diagram.Controller.RootItem;
				Menu.Placement = PlacementMode.Mouse;
				Menu.VerticalOffset = 0;
				Menu.ShowPopup(Menu.PlacementTarget);
			}
		}
		#region ILogicalOwner
		readonly List<object> logicalChildren = new List<object>();
		void ILogicalOwner.AddChild(object child) {
			logicalChildren.Add(child);
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			RemoveLogicalChild(child);
			logicalChildren.Remove(child);
		}
		protected override IEnumerator LogicalChildren { get { return new MergedEnumerator(base.LogicalChildren, logicalChildren.GetEnumerator()); } }
		#endregion
	}
}
