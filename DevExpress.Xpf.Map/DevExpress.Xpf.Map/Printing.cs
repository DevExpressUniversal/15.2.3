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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPrinting.DataNodes;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
namespace DevExpress.Xpf.Map {
	public enum MapPrintSizeMode {
		Normal = 0,
		Zoom = 1,
		Stretch = 2
	}
	public class MapPrintOptions : MapDependencyObject {
		public static readonly DependencyProperty SizeModeProperty = DependencyPropertyManager.Register("SizeMode",
			typeof(MapPrintSizeMode), typeof(MapPrintOptions), new PropertyMetadata(MapPrintSizeMode.Normal));
		public static readonly DependencyProperty PrintMiniMapProperty = DependencyPropertyManager.Register("PrintMiniMap",
			typeof(bool), typeof(MapPrintOptions), new PropertyMetadata(false));
		[Category(Categories.Behavior)]
		public MapPrintSizeMode SizeMode {
			get { return (MapPrintSizeMode)GetValue(SizeModeProperty); }
			set { SetValue(SizeModeProperty, value); }
		}
		[Category(Categories.Behavior)]
		public bool PrintMiniMap {
			get { return (bool)GetValue(PrintMiniMapProperty); }
			set { SetValue(PrintMiniMapProperty, value); }
		}
		protected override MapDependencyObject CreateObject() {
			return new MapPrintOptions();
		}
	}
	public class PrintContainer : Control {
		readonly MapControl map;
		ItemsControl legendsContainer;
		ItemsControl layersContainer;
		MapOverlaysPanel MiniMapPanel { get; set; }
		bool CanPrintMiniMap { get { return Map.MiniMap != null && Map.PrintOptions != null && Map.PrintOptions.PrintMiniMap; } }
		internal ItemsControl LayersContainer {
			get { return layersContainer; }
			set {
				if (layersContainer == value)
					return;
				layersContainer = value;
				OnLayersContainerChanged();
			}
		}
		internal ItemsControl LegendsContainer {
			get { return legendsContainer; }
			set {
				if (legendsContainer == value)
					return;
				legendsContainer = value;
				OnLegendContainerChanged();
			}
		}
		public MapControl Map { get { return map; } }
		public PrintContainer(MapControl map) {
			this.map = map;
			this.DataContext = map.DataContext;
			DefaultStyleKey = typeof(PrintContainer);
		}
		void ApplyMiniMap(MapOverlaysPanel miniMapPanel) {
			if (miniMapPanel != null && CanPrintMiniMap)
				miniMapPanel.Children.Add(Map.MiniMap);
		}
		protected virtual void OnLayersContainerChanged() {
			if (layersContainer != null) {
				layersContainer.ItemsSource = Map.Layers;
			}
		}
		protected virtual void OnLegendContainerChanged() {
			if (legendsContainer != null) {
				legendsContainer.ItemsSource = Map.Legends;
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			LayersContainer = GetTemplateChild("PART_PrintLayersContainer") as ItemsControl;
			LegendsContainer = GetTemplateChild("PART_PrintLegendsContainer") as ItemsControl;
			MiniMapPanel = GetTemplateChild("PART_MiniMap") as MapOverlaysPanel;
			ApplyMiniMap(MiniMapPanel); 
		}
		public void ClearVisualTree() {
			if (LayersContainer != null)
				LayersContainer.ItemsSource = null;
			if (LegendsContainer != null)
				LegendsContainer.ItemsSource = null;
			if (MiniMapPanel != null)
				MiniMapPanel.Children.Clear();
		}
	}
}
namespace DevExpress.Xpf.Map.Printing {
	public class MapRootDataNode : IRootDataNode, IDisposable {
		readonly MapControlPrinter printer;
		readonly MapVisualDataNode visualChild;
		public MapControlPrinter Printer { get { return printer; } }
		public MapRootDataNode(MapControl map, Size usablePageSize) {
			this.printer = CreatePrinter(map);
			this.visualChild = new MapVisualDataNode(this, usablePageSize);
		}
		#region IRootDataNode implementation
		int IRootDataNode.GetTotalDetailCount() {
			return 1;
		}
		#endregion
		#region IDataNode implementation
		int IDataNode.Index { get { return -1; } }
		bool IDataNode.IsDetailContainer { get { return true; } }
		bool IDataNode.PageBreakAfter { get { return false; } }
		bool IDataNode.PageBreakBefore { get { return false; } }
		IDataNode IDataNode.Parent { get { return null; } }
		bool IDataNode.CanGetChild(int index) {
			return IsValidChildIndex(index);
		}
		IDataNode IDataNode.GetChild(int index) {
			return IsValidChildIndex(index) ? visualChild : null;
		}
		#endregion
		#region IDisposable implementation
		bool isDisposed;
		void Dispose(bool disposing) {
			if (disposing && !isDisposed) {
				isDisposed = true;
				printer.OnCreateDocumentFinished();
			}
		}
		~MapRootDataNode() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
		protected virtual MapControlPrinter CreatePrinter(MapControl map) {
			return new MapControlPrinter(map);
		}
		bool IsValidChildIndex(int childIndex) {
			return childIndex == 0;
		}
	}
	public class MapVisualDataNode : IVisualDetailNode {
		const string printingTemplateString = @"<Border xmlns:dxp=""http://schemas.devexpress.com/winfx/2008/xaml/printing"">
                                              <Image Source=""{Binding Content}"" dxp:ExportSettings.TargetType=""Image""/>
                                              </Border>";
		readonly MapRootDataNode root;
		readonly Size usablePageSize;
		protected MapControlPrinter Printer { get { return root.Printer; } }
		protected MapControl Map { get { return Printer != null ? Printer.Map : null; } }
		public MapVisualDataNode(MapRootDataNode root, Size usablePageSize) {
			this.root = root;
			this.usablePageSize = usablePageSize;
		}
		#region IVisualDetailNode Members
		RowViewInfo IVisualDetailNode.GetDetail(bool allowContentReuse) {
			DataTemplate template = XamlHelper.GetTemplate(printingTemplateString);
			return new RowViewInfo(template, CreateImageSource());
		}
		#endregion
		#region IDataNode Members
		int IDataNode.Index { get { return 0; } }
		bool IDataNode.IsDetailContainer { get { return false; } }
		bool IDataNode.PageBreakAfter { get { return false; } }
		bool IDataNode.PageBreakBefore { get { return false; } }
		IDataNode IDataNode.Parent { get { return root; } }
		bool IDataNode.CanGetChild(int index) {
			return false;
		}
		IDataNode IDataNode.GetChild(int index) {
			return null;
		}
		#endregion
		object CreateImageSource() {
			return Printer.RenderToImage(usablePageSize);
		}
	}
	public class MapControlPrinter {
		const double dpiX = 96;
		const double dpiY = 96;
		const int DefaultTimeout = 10000;
		WeakReference mapReference;
		PrintContainer container;
		MapPrintSizeMode sizeMode = MapPrintSizeMode.Normal;
		delegate void EmptyDelegate();
		public MapControl Map { get { return mapReference.IsAlive ? mapReference.Target as MapControl : null; } }
		public MapPrintSizeMode PrintSizeMode { get { return sizeMode; } }
		public MapControlPrinter(MapControl map) {
			this.mapReference = new WeakReference(map);
			this.sizeMode = GetSizeMode(Map.PrintOptions);
			OnCreateDocumentStarted();
		}
		MapPrintSizeMode GetSizeMode(MapPrintOptions options) {
			return options != null ? options.SizeMode : MapPrintSizeMode.Normal;
		}
		public MapControlPrinter(MapControl map, MapPrintSizeMode printSizeMode) {
			this.mapReference = new WeakReference(map);
			this.sizeMode = printSizeMode;
		}
		public void OnCreateDocumentStarted() {
			Map.ClearVisualTreeOnPrint();
		}
		public void OnCreateDocumentFinished() {
			container.ClearVisualTree();
			container = null;
			Map.RestoreVisualTreeOnPrint();
		}
		public object RenderToImage(Size usablePageSize) {
			Size initialSize = CalculateActualMapSize(Map);
			Size renderSize = CalculatePrintAreaSize(initialSize, usablePageSize, PrintSizeMode);
			if (renderSize.Height == 0 || renderSize.Width == 0)
				return null;
			RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderSize.Width, (int)renderSize.Height, dpiX, dpiY, PixelFormats.Pbgra32);
			RenderPrintingVisualTree(Map, renderSize, renderTarget);
			return renderTarget;
		}
		Size CalculateActualMapSize(MapControl control) {
			double width = double.IsNaN(control.Width) ? control.ActualWidth : control.Width;
			double height = double.IsNaN(control.Height) ? control.ActualHeight : control.Height;
			return new Size(width, height);
		}
		void RenderPrintingVisualTree(MapControl map, Size renderSize, RenderTargetBitmap renderTarget) {
			Rect bounds = new Rect(new Point(), renderSize);
			this.container = new PrintContainer(map);
			container.Measure(renderSize);
			container.Arrange(bounds);
			container.UpdateLayout();
			WaitDataLoaded(map.Layers);
			renderTarget.Render(container);
		}
		static void DoEvents() {
			Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new EmptyDelegate(delegate { }));
		}
		void WaitDataLoaded(LayerCollection layerCollection) {
			int start = System.Environment.TickCount;
			while (!AreLayersDataReady(layerCollection)) {
				DoEvents();
				if (System.Environment.TickCount - start > DefaultTimeout)
					break;
			}
		}
		bool AreLayersDataReady(LayerCollection layerCollection) {
			foreach (LayerBase layer in layerCollection) {
				if (!layer.IsDataReady)
					return false;
			}
			return true;
		}
		protected Size CalculatePrintAreaSize(Size imageSize, Size usablePageSize, MapPrintSizeMode sizeMode) {
			switch (sizeMode) {
				case MapPrintSizeMode.Zoom:
					return ZoomInto(usablePageSize, imageSize);
				case MapPrintSizeMode.Stretch:
					return usablePageSize;
			}
			return imageSize;
		}
		public static Size ZoomInto(Size outer, Size inner) {
			Size result = new Size();
			double innerRatio = inner.Width / inner.Height;
			double outerRatio = outer.Width / outer.Height;
			if (innerRatio < outerRatio) {
				result.Height = outer.Height;
				result.Width = outer.Height * innerRatio;
			}
			else {
				result.Width = outer.Width;
				result.Height = outer.Width / innerRatio;
			}
			return result;
		}
	}
}
