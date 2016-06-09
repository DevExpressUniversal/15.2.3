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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting.DataNodes;
namespace DevExpress.Xpf.Charts.Native {
	public class ChartVisualDataNode : IVisualDetailNode {
		const double dpiX = 96;
		const double dpiY = 96;
		const string chartPrintingTemplateString = @"<Border xmlns:dxp=""http://schemas.devexpress.com/winfx/2008/xaml/printing"">
                                                        <Image Source=""{Binding Content.BitmapSource}"" dxp:ExportSettings.TargetType=""Image""
                                                                                            dxp:ImageExportSettings.ImageSource=""{Binding RelativeSource={RelativeSource Mode=Self}}""
                                                                                            dxp:ImageExportSettings.ImageRenderMode=""UseImageSource""
                                                                                            Width=""{Binding Content.ImageWidth}""
                                                                                            Height=""{Binding Content.ImageHeight}""/>
                                                     </Border>";
		readonly ChartRootDataNode parent;
		readonly Size usablePageSize;
		public ChartVisualDataNode(ChartRootDataNode parent, Size usablePageSize) {
			this.parent = parent;
			this.usablePageSize = usablePageSize;
		}
		#region IVisualDetailNode Members
		RowViewInfo IVisualDetailNode.GetDetail(bool allowContentReuse) {
			return new RowViewInfo(GetChartPrintingTemplate(), CreateRenderContext());
		}
		#endregion
		#region IDataNode Members
		int IDataNode.Index { get { return 0; } }
		bool IDataNode.IsDetailContainer { get { return false; } }
		bool IDataNode.PageBreakAfter { get { return false; } }
		bool IDataNode.PageBreakBefore { get { return false; } }
		IDataNode IDataNode.Parent { get { return parent; } }
		bool IDataNode.CanGetChild(int index) {
			return false;
		}
		IDataNode IDataNode.GetChild(int index) {
			return null;
		}
		#endregion
		ChartRenderContext CreateRenderContext() {
			ChartControl chart = parent.Chart;
			TextFormattingMode initialTextFormattingMode = TextOptions.GetTextFormattingMode(chart);
			TextOptions.SetTextFormattingMode(chart, TextFormattingMode.Display);
			ChartPrintLayoutCalculatorBase layoutCalculator = CreatePrintLayoutCalculator(chart.ActualPrintSizeMode);
			Size chartInitialSize = layoutCalculator.CalculateActualChartSize(chart);
			Size chartRenderSize = layoutCalculator.CalculateChartRenderSize(chartInitialSize, usablePageSize);
			Size renderAreaSize = layoutCalculator.CalculateChartImageSize(chartInitialSize, usablePageSize);
			chartRenderSize = new Size((int)chartRenderSize.Width, (int)chartRenderSize.Height);
			renderAreaSize = RoundSize(renderAreaSize);
			Control bufferControl = new Control();
			SaveChartProperties(chart, bufferControl);
			PrepareChartForPrinting(chart);
			if (chartRenderSize == chartInitialSize)
				PrepareChartLayout(chart, new Size(1, 1));
			PrepareChartLayout(chart, chartRenderSize);
			chartRenderSize = RoundSize(chartRenderSize);
			RenderTargetBitmap bitmapSource = new RenderTargetBitmap((int)chartRenderSize.Width, (int)chartRenderSize.Height, dpiX, dpiY, PixelFormats.Pbgra32);
			bitmapSource.Render(chart);
			TextOptions.SetTextFormattingMode(chart, initialTextFormattingMode);
			RestoreChartLayout(chart, chartInitialSize);
			chart.SelectionController.UpdateElementsSelection();
			RestoreChartProperties(chart, bufferControl);
			return new ChartRenderContext(bitmapSource, renderAreaSize.Width, renderAreaSize.Height);
		}
		void PrepareChartForPrinting(ChartControl chart) {
			chart.SelectionController.HideSelection();
			HideChartCrosshair(chart);
			chart.ActualToolTipController.UpdateToolTip(new Point(), chart, ToolTipNavigationAction.MouseLeave);
		}
		void HideChartCrosshair(ChartControl chart) {
			Diagram diagram = chart.Diagram;
			if (diagram != null)
				diagram.UpdateCrosshairLocation(null);
		}
		ChartPrintLayoutCalculatorBase CreatePrintLayoutCalculator(PrintSizeMode sizeMode) {
			switch (sizeMode) {
				case PrintSizeMode.None:
					return new DefaultChartPrintLayoutCalculator();
				case PrintSizeMode.Stretch:
					return new StretchChartPrintLayoutCalculator();
				case PrintSizeMode.NonProportionalZoom:
					return new NonProportionalZoomChartPrintLayoutCalculator();
				case PrintSizeMode.ProportionalZoom:
					return new ProportionalZoomChartPrintLayoutCalculator();
				default:
					ChartDebug.Fail("Unknown PrintSizeMode");
					return new DefaultChartPrintLayoutCalculator();
			}
		}
		void ResizeChart(ChartControl chart, Size newSize) {
			chart.Width = newSize.Width;
			chart.Height = newSize.Height;
		}
		void MeasureAndArrangeChart(ChartControl chart, Size newSize) {
			chart.Measure(newSize);
			chart.Arrange(new Rect(new Point(0, 0), newSize));
		}
		void PrepareChartLayout(ChartControl chart, Size newSize) {
			ResizeChart(chart, newSize);
			chart.Margin = new Thickness(0);
			chart.UpdateLayout();
			MeasureAndArrangeChart(chart, newSize);
		}
		void RestoreChartLayout(ChartControl chart, Size newSize) {
			ResizeChart(chart, newSize);
			MeasureAndArrangeChart(chart, newSize);
			chart.UpdateLayout();
		}
		void SaveChartProperties(ChartControl chart, Control bufferControl) {
			CopyProperties(chart, bufferControl);
		}
		void RestoreChartProperties(ChartControl chart, Control bufferControl) {
			CopyProperties(bufferControl, chart);
		}
		void CopyProperty(Control source, Control target, DependencyProperty dp) {
			target.ClearValue(dp);
			CopyPropertyValueHelper.CopyPropertyValue(target, source, dp);
			source.ClearValue(dp);
		}
		void CopyProperties(Control source, Control target) {
			CopyProperty(source, target, Control.WidthProperty);
			CopyProperty(source, target, Control.HeightProperty);
			CopyProperty(source, target, Control.MarginProperty);
		}
		Size RoundSize(Size size) {
			return new Size(Math.Max((int)size.Width, 1), Math.Max((int)size.Height, 1));
		}
		DataTemplate GetChartPrintingTemplate() {
			return XamlHelper.GetTemplate(chartPrintingTemplateString);
		}
	}
	public class ChartRenderContext {
		readonly RenderTargetBitmap bitmapSource;
		readonly double imageWidth;
		readonly double imageHeight;
		public RenderTargetBitmap BitmapSource { get { return bitmapSource; } }
		public double ImageWidth { get { return imageWidth; } }
		public double ImageHeight { get { return imageHeight; } }
		public ChartRenderContext(RenderTargetBitmap bitmapSource, double imageWidth, double imageHeight) {
			this.bitmapSource = bitmapSource;
			this.imageWidth = imageWidth;
			this.imageHeight = imageHeight;
		}
	}
}
