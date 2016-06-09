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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public class DiagramControlWithRenderLayer : DiagramControlEx {
		interface IDiagramItemNativeRegion {
			void Destroy();
		}
		class RenderLayerImpl : ILayer {
			class DiagramItemRegion : INativeImageRegion, IDiagramItemNativeRegion {
				readonly RenderLayerImpl layer;
				readonly INativeImageRegionRenderer renderer;
				public DiagramItemRegion(RenderLayerImpl layer, DiagramItem item, INativeImageRegionRenderer renderer) {
					this.renderer = renderer;
					this.layer = layer;
					Item = item;
				}
				void IDiagramItemNativeRegion.Destroy() {
					layer.Destroy(this);
				}
				public DiagramItem Item { get; private set; }
				Rect INativeImageRegion.LogicBounds { get { return Item.ActualDiagramBounds(); } }
				INativeImageRegionRenderer INativeImageRegion.Renderer { get { return renderer; } }
				bool INativeImageRegion.Visible { get { return XRDiagramItemBase.GetBandContentVisible(Item); } }
			}
			readonly DiagramControlWithRenderLayer diagram;
			readonly NativeImage nativeImage;
			readonly MultiRegionNativeImageRenderer nativeImageRenderer;
			public RenderLayerImpl(DiagramControlWithRenderLayer diagram) {
				this.diagram = diagram;
				nativeImage = new NativeImage();
				nativeImage.SetBinding(NativeImage.BackgroundProperty, new Binding() { Path = new PropertyPath(DiagramControl.BackgroundProperty), Mode = BindingMode.OneWay, Source = diagram });
				nativeImageRenderer = new MultiRegionNativeImageRenderer();
				nativeImage.Renderer = nativeImageRenderer;
			}
			public FrameworkElement Visual { get { return nativeImage; } }
			public IDiagramItemNativeRegion CreateRegion(DiagramItem item, INativeImageRegionRenderer renderer) {
				var region = new DiagramItemRegion(this, item, renderer);
				nativeImageRenderer.AddRegion(region);
				return region;
			}
			void Destroy(DiagramItemRegion region) {
				nativeImageRenderer.RemoveRegion(region);
			}
			void ILayer.Update(Point offset, Point location, Size viewport, double zoomFactor) {
				nativeImageRenderer.Update(offset, zoomFactor);
				UpdateNativeImageSize(zoomFactor);
			}
			public void UpdateNativeImageSize(double zoomFactor) {
				var parent = (FrameworkElement)nativeImage.Parent;
				if(parent == null) return;
				double leftPadding = diagram.LeftPadding * zoomFactor;
				Size baseSize = parent.RenderSize;
				nativeImage.SetSize(new Size(leftPadding + baseSize.Width, baseSize.Height));
				nativeImage.Margin = new Thickness(-leftPadding, 0.0, 0.0, 0.0);
			}
			public void InvalidateNativeImage() {
				nativeImage.Invalidate();
			}
		}
		public static readonly DependencyProperty ScrollGapProperty;
		public Thickness ScrollGap {
			get { return (Thickness)GetValue(ScrollGapProperty); }
			set { SetValue(ScrollGapProperty, value); }
		}
		public static readonly DependencyProperty LeftPaddingProperty;
		public double LeftPadding {
			get { return (double)GetValue(LeftPaddingProperty); }
			set { SetValue(LeftPaddingProperty, value); }
		}
		public static readonly DependencyProperty RightPaddingProperty;
		public double RightPadding {
			get { return (double)GetValue(RightPaddingProperty); }
			set { SetValue(RightPaddingProperty, value); }
		}
		public static readonly DependencyProperty RendererProperty;
		public static INativeImageRegionRenderer GetRenderer(DiagramItem d) { return (INativeImageRegionRenderer)d.GetValue(RendererProperty); }
		public static void SetRenderer(DiagramItem d, INativeImageRegionRenderer renderer) { d.SetValue(RendererProperty, renderer); }
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty RendererRegionProperty;
		static IDiagramItemNativeRegion GetRendererRegion(DiagramItem d) { return (IDiagramItemNativeRegion)d.GetValue(RendererRegionProperty); }
		static void SetRendererRegion(DiagramItem d, IDiagramItemNativeRegion region) { d.SetValue(RendererRegionProperty, region); }
		static DiagramControlWithRenderLayer() {
			DependencyPropertyRegistrator<DiagramControlWithRenderLayer>.New()
				.RegisterAttached((DiagramItem d) => GetRenderer(d), out RendererProperty, null, (d, e) => OnRendererChanged(d, e))
				.RegisterAttached((DiagramItem d) => GetRendererRegion(d), out RendererRegionProperty, null)
				.Register(d => d.LeftPadding, out LeftPaddingProperty, 0.0, d => d.OnLeftPaddingChanged())
				.Register(d => d.RightPadding, out RightPaddingProperty, 0.0)
				.Register(d => d.ScrollGap, out ScrollGapProperty, new Thickness(), d => d.UpdateScrollMargin())
			;
		}
		RenderLayerImpl renderLayer;
		RenderLayerImpl RenderLayer {
			get {
				if(renderLayer == null)
					renderLayer = new RenderLayerImpl(this);
				return renderLayer;
			}
		}
		public DiagramControlWithRenderLayer() {
			GridSize = new Size(12, 12);
			MeasureUnit = MeasureUnits.Inches;
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if(e.Property != SelectionModelProperty && e.Property != SelectionToolsModelProperty) return;
			var oldValue = (INotifyPropertyChanged)e.OldValue;
			var newValue = (INotifyPropertyChanged)e.NewValue;
			if(oldValue != null)
				oldValue.PropertyChanged -= OnSelectionPropertyChanged;
			if(newValue != null)
				newValue.PropertyChanged += OnSelectionPropertyChanged;
			InvalidateRenderLayer();
		}
		void OnSelectionPropertyChanged(object sender, PropertyChangedEventArgs e) {
			InvalidateRenderLayer();
		}
		public void InvalidateRenderLayer() {
			RenderLayer.InvalidateNativeImage();
		}
		protected override IEnumerable<LayerInfo> GetBackgroundLayers() {
			yield return new LayerInfo(RenderLayer, RenderLayer.Visual);
			foreach(var layer in base.GetBackgroundLayers())
				yield return layer;
		}
		static void OnRendererChanged(DiagramItem d, DependencyPropertyChangedEventArgs e) {
			var diagram = (DiagramControlWithRenderLayer)d.GetDiagram();
			if(diagram == null) return;
			diagram.DestroyRegion(d);
			diagram.CreateRegion(d);
		}
		protected void DestroyRegion(DiagramItem diagramItem) {
			var region = GetRendererRegion(diagramItem);
			if(region == null) return;
			region.Destroy();
			SetRendererRegion(diagramItem, null);
		}
		protected void CreateRegion(DiagramItem diagramItem) {
			var renderer = GetRenderer(diagramItem);
			if(renderer == null) return;
			SetRendererRegion(diagramItem, renderLayer.CreateRegion(diagramItem, renderer));
		}
		void OnLeftPaddingChanged() {
			UpdateScrollMargin();
			RenderLayer.UpdateNativeImageSize(ZoomFactor);
		}
		void UpdateScrollMargin() {
			SetBinding(ScrollMarginProperty, new Binding(ZoomFactorProperty.Name) { Source = this, Mode = BindingMode.OneWay, Converter = new ScrollMarginBindingConverter(this) });
		}
		public void BindRenderLayerRootAdornerElement(FrameworkElement adornerElement) {
			adornerElement.SetBinding(AdornerLayer.AdornerMarginProperty, new Binding(MarginProperty.Name) { Source = RenderLayer.Visual, Mode = BindingMode.OneWay });
		}
		class ScrollMarginBindingConverter : IValueConverter {
			readonly DiagramControlWithRenderLayer diagram;
			public ScrollMarginBindingConverter(DiagramControlWithRenderLayer diagram) {
				this.diagram = diagram;
			}
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
				double zoomFactor = (double)value;
				double margin = zoomFactor * diagram.LeftPadding;
				return new Thickness(diagram.ScrollGap.Left + margin, diagram.ScrollGap.Top, diagram.ScrollGap.Right, diagram.ScrollGap.Bottom);
			}
			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
				throw new NotSupportedException();
			}
		}
	}
}
