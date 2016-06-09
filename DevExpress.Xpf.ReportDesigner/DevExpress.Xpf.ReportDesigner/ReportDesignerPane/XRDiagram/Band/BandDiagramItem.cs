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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.XtraReports.UI;
using IInputElement = DevExpress.Diagram.Core.IInputElement;
using ResizeMode = DevExpress.Diagram.Core.ResizeMode;
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
	public class BandDiagramItem : XRDiagramContainer {
		protected class BandDiagramItemController : XRDiagramContainerController {
			BandDiagramItem BandItem { get { return (BandDiagramItem)Item; } }
			public BandDiagramItemController(IDiagramContainer item) : base(item) {
			}
			public override bool CanDeleteCore() {
				return BandItem.BandKind == BandKind.DetailReport || base.CanDeleteCore();
			}
			public override IInputElement CreateInputElement() {
				return new BandDiagramItemInputElement(BandItem);
			}
			protected override IEnumerable<IAdorner> CreateAdorners() {
				var diagram = BandItem.GetXRDiagram();
				var headerAdorner = new BandHeaderAdorner(BandItem) { Padding = BandItem.BandKind == BandKind.BottomMargin ? new Thickness() : new Thickness(0.0, 0.0, 0.0, -1.0) };
				headerAdorner.BandKind = BandItem.BandKind;
				headerAdorner.BandName = BandItem.BandName;
				headerAdorner.SetBinding(BandHeaderAdorner.BandPanelGapProperty, new Binding() { Path = new PropertyPath(XRDiagramControl.RulerWidthProperty), Source = diagram, Mode = BindingMode.OneWay });
				headerAdorner.SetBinding(BandHeaderAdorner.BandNameProperty, new Binding(BandNameProperty.Name) { Source = Item, Mode = BindingMode.OneWay });
				headerAdorner.SetBinding(BandHeaderAdorner.BandNestingLevelProperty, new Binding(BandNestingLevelProperty.Name) { Source = Item, Mode = BindingMode.OneWay });
				headerAdorner.SetBinding(BandHeaderAdorner.MaxBandNestingLevelProperty, new Binding(XRDiagramControl.MaxBandNestingLevelProperty.Name) { Source = diagram, Mode = BindingMode.OneWay });
				headerAdorner.SetBinding(BandHeaderAdorner.HeaderLevelWidthProperty, new Binding(XRDiagramControl.BandHeaderLevelWidthProperty.Name) { Source = diagram, Mode = BindingMode.OneWay });
				var result = base.CreateAdorners().Concat(new[] {
					diagram.CreateAdorner(new BandMarginAdorner(BandItem)),
					diagram.CreateBandHeaderAdorner(headerAdorner)
				});
				if(BandItem.BandKind != BandKind.BottomMargin)
					result = result.Concat(diagram.CreateAdorner(new BandBorderAdorner(BandItem)).Yield());
				if(BandItem.BandKind != BandKind.DetailReport && BandItem.BandKind != BandKind.SubBand) {
					result = result.Concat(diagram.CreateVRulerAdorner(new XRVerticalRulerScale(diagram)).MakeTopmostEx().Yield());
					result = result.Concat(diagram.CreateVRulerAdorner(new XRVerticalRulerScaleBackground(diagram)).Yield());
				}
				return result;
			}
			protected override AdjustBoundaryBehavior GetAdjustBoundaryBehavior(Direction direction) {
				if(direction == Direction.Down)
					return AdjustBoundaryBehavior.AutoAdjust;
				if(direction == Direction.Right)
					return AdjustBoundaryBehavior.None;
				return AdjustBoundaryBehavior.DisableOutOfBounds;
			}
		}
		public static readonly DependencyProperty BandKindProperty;
		public static readonly DependencyProperty BandNameProperty;
		public static readonly DependencyProperty BandNestingLevelProperty;
		class BandSelectionLayer : SelectionPartsLayerBase<BandSelectionAdorner> {
			protected override BandSelectionAdorner CreateAdornerElement(DiagramControl diagram, IDiagramItem item, bool isPrimarySelection) {
				return new BandSelectionAdorner((XRDiagramControl)diagram);
			}
		}
		static BandDiagramItem() {
			DependencyPropertyRegistrator<BandDiagramItem>.New()
				.Register(d => d.BandKind, out BandKindProperty, BandKind.Detail)
				.Register(d => d.BandName, out BandNameProperty, "")
				.Register(d => d.BandNestingLevel, out BandNestingLevelProperty, -1)
				.OverrideMetadata(CanMoveProperty, false)
				.OverrideMetadata(IsSnapScopeProperty, true)
				.OverrideMetadata(SelectionLayerProperty, new BandSelectionLayer())
				.OverrideDefaultStyleKey()
				.OverrideMetadata(CanCopyProperty, false)
			;
		}
		public BandKind BandKind {
			get { return (BandKind)GetValue(BandKindProperty); }
			set { SetValue(BandKindProperty, value); }
		}
		public string BandName {
			get { return (string)GetValue(BandNameProperty); }
			set { SetValue(BandNameProperty, value); }
		}
		public int BandNestingLevel {
			get { return (int)GetValue(BandNestingLevelProperty); }
			set { SetValue(BandNestingLevelProperty, value); }
		}
		public bool Collapsed {
			get { return XRDiagramItemBase.GetBandCollapsed(this); }
			set { XRDiagramItemBase.SetBandCollapsed(this, value); }
		}
		public void InvalidateChild(DiagramItem diagramItem) {
			XRDiagramItemBase.InvalidateChildDiagramItem(this, diagramItem);
		}
		protected internal override bool CanAddItem(DiagramItem item) {
			if(item is XRTableOfContentsDiagramItem)
				return this.Owner() is XRDiagramRoot && (BandKind == BandKind.ReportHeader || BandKind == BandKind.ReportFooter) && !Items.Any(x => x is XRTableOfContentsDiagramItem);
			return true;
		}
		protected override XRDiagramContainerControllerBase CreateXRContainerController() {
			return new BandDiagramItemController(this);
		}
		#region InputElement
		protected internal class BandDiagramItemInputElement : ContainerItemInputElement {
			readonly BandDiagramItem item;
			public BandDiagramItemInputElement(BandDiagramItem item) : base(item) {
				this.item = item;
			}
			protected override InputState CreatePointerToolMousePressedState(IDiagramControl diagram, IMouseButtonArgs mouseArgs) {
				if(IsResizeArea(mouseArgs.Position)) {
					var adorner = ((DiagramControl)diagram).CreateAdorner(new BandResizeAdorner((XRDiagramControl)diagram));
					adorner.Bounds = item.ActualDiagramBounds();
					return ResizeState.CreateResizePreviewState(diagram, ResizeMode.Bottom, mouseArgs, new[] { SizeInfo.Create(adorner, item, ResizeMode.Bottom) }, new[] { item }, () => { }, () => adorner.Destroy(), x => SnapInfo.Empty, mouseArgs.ChangedButton, 0);
				}
				return base.CreatePointerToolMousePressedState(diagram, mouseArgs);
			}
			protected override DiagramCursor GetPointerToolCursor(IMouseArgs args) {
				if(IsResizeArea(args.Position))
					return DiagramCursor.SizeNS;
				return base.GetPointerToolCursor(args);
			}
			bool IsResizeArea(Point diagramPoint) {
				return item.CanResize && diagramPoint.Y > item.ActualDiagramBounds().Bottom - 5;
			}
		}
		#endregion
	}
	public abstract class XRRulerScaleBase : RulerScaleBase {
		public XRRulerScaleBase(Orientation orientation, DiagramControl diagram) : base(orientation, diagram) {
		}
	}
	public class XRVerticalRulerScale : XRRulerScaleBase {
		static XRVerticalRulerScale() {
			DependencyPropertyRegistrator<XRVerticalRulerScale>.New()
				.OverrideDefaultStyleKey()
			;
		}
		public XRVerticalRulerScale(XRDiagramControl diagram) : base(Orientation.Vertical, diagram) {
			diagram.BindRenderLayerRootAdornerElement(this);
			AdornerLayer.SetForceBoundsRounding(this, true);
		}
	}
	public class XRVerticalRulerScaleBackground : Border {
		static XRVerticalRulerScaleBackground() {
			DependencyPropertyRegistrator<XRVerticalRulerScaleBackground>.New()
				.OverrideDefaultStyleKey()
			;
		}
		public XRVerticalRulerScaleBackground(XRDiagramControl diagram) {
			diagram.BindRenderLayerRootAdornerElement(this);
			AdornerLayer.SetForceBoundsRounding(this, true);
		}
	}
	public class XRHorizontalRulerScaleBackground : Border {
		static XRHorizontalRulerScaleBackground() {
			DependencyPropertyRegistrator<XRHorizontalRulerScaleBackground>.New()
				.OverrideDefaultStyleKey()
			;
		}
		public XRHorizontalRulerScaleBackground(XRDiagramControl diagram) {
			diagram.BindRenderLayerRootAdornerElement(this);
			AdornerLayer.SetForceBoundsRounding(this, true);
		}
	}
	public class XRHorizontalRulerScale : XRRulerScaleBase {
		static XRHorizontalRulerScale() {
			DependencyPropertyRegistrator<XRHorizontalRulerScale>.New()
				.OverrideDefaultStyleKey()
			;
		}
		protected override double GetNearPadding() {
			return AdornerLayer.GetAdornerMargin(this).Left;
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if(e.Property == AdornerLayer.AdornerMarginProperty)
				UpdatePadding();
		}
		public XRHorizontalRulerScale(XRDiagramControl diagram)
			: base(Orientation.Horizontal, diagram) {
			diagram.BindRenderLayerRootAdornerElement(this);
			AdornerLayer.SetForceBoundsRounding(this, true);
		}
	}
}
