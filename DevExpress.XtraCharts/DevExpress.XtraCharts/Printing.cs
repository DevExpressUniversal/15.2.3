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
using System.Drawing.Imaging;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraCharts.Native;
using System.Drawing.Printing;
namespace DevExpress.XtraCharts.Printing {
	public enum PrintSizeMode {
		None,
		Stretch,
		Zoom
	}
	public enum PrintImageFormat {
		Bitmap,
		Metafile
	}
	public class ChartPrinter : IChartPrinter, IDisposable, ICloneable {
		readonly IChartContainer control;
		IPrintingSystem printingSystem;
		ILink link;
		IDisposable componentPrinter;
		PrintSizeMode sizeMode;
		PrintImageFormat imageFormat;
		ChartPageSettings pageSettings;
		IChartRenderProvider RenderProvider { get { return control.RenderProvider; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("ChartPrinterSizeMode")]
#endif
		public PrintSizeMode SizeMode { 
			get { return sizeMode; } 
			set { sizeMode = value; } 
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("ChartPrinterImageFormat")]
#endif
		public PrintImageFormat ImageFormat { 
			get { return imageFormat; } 
			set { imageFormat = value; } 
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("ChartPrinterComponentPrinter")]
#endif
		public ComponentExporter ComponentPrinter {
			get {
				if (componentPrinter == null)
					componentPrinter = CreateComponentPrinter();
				return (ComponentExporter)componentPrinter;
			}
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("ChartPrinterIsPrintingAvailable")]
#endif
		public bool IsPrintingAvailable { get { return RenderProvider != null ? RenderProvider.IsPrintingAvailable : false; } }
		public ChartPrinter(IChartContainer control) {
			this.control = control;
		}
		#region IChartPrinter implementation
		void IChartPrinter.SetPageSettings(ChartPageSettings settings) {
			this.pageSettings = settings;
		}
		#endregion
		public void Dispose() {
			Release();
			if (componentPrinter != null) {
				componentPrinter.Dispose();
				componentPrinter = null;
			}
		}
		public void Assign(ChartPrinter printer) {
			sizeMode = printer.sizeMode;
			imageFormat = printer.imageFormat;
		}
		public void PerformPrintingAction(Action0 action, PrintSizeMode sizeMode) {
			this.sizeMode = sizeMode;
			PerformPrintingAction(action);
		}
		public virtual ComponentExporter CreateComponentPrinter() {
			IChartRenderProvider renderProvider = RenderProvider;
			ComponentExporter exporter = renderProvider != null ? renderProvider.CreateComponentPrinter(renderProvider.Printable) : null;
			if (exporter == null)
				exporter = renderProvider != null ? new ComponentExporter(renderProvider.Printable) : new ComponentExporter(null);
			return exporter;
		}
		public virtual void Initialize(IPrintingSystem ps, ILink lnk) {
			SetPS(ps);
			link = lnk as LinkBase;
			if (pageSettings != null)
				pageSettings.Apply(ps as PrintingSystemBase);
		}
		public virtual void Release() {
			SetPS(null);
			link = null;
		}
		public virtual void PerformPrintingAction(Action0 action) {
			ComponentPrinter.ClearDocument();
			action();
		}
		public virtual void CreateArea(string areaName, IBrickGraphics graph) {
			if (printingSystem != null && areaName == SR.Detail && RenderProvider != null)
				CreateDetail(graph);
		}
		public virtual void CreateDetail(IBrickGraphics graph) {
			Chart chart = control.Chart;
			Rectangle bounds;
			switch (sizeMode) {
				case PrintSizeMode.Stretch:
					bounds = new Rectangle(Point.Empty, Size.Truncate(GetPSSize()));
					break;
				case PrintSizeMode.Zoom: {
					Rectangle displayBounds = RenderProvider.DisplayBounds;
	 				float controlRatio = (float)displayBounds.Width / displayBounds.Height;
					SizeF size = GetPSSize();
					if (size.IsEmpty)
						bounds = Rectangle.Empty;
					else {
						if (controlRatio > size.Width / size.Height)
							size.Height = size.Width / controlRatio;
						else
							size.Width = size.Height * controlRatio;
						bounds = new Rectangle(Point.Empty, Size.Truncate(size));
					}
					break;
				}
				default:
					bounds = RenderProvider.DisplayBounds;
					break;
			}
			BrickGraphics brickGraph = graph as BrickGraphics;
			if (brickGraph != null && bounds.Width > 0 && bounds.Height > 0) {
				GraphicsUnit oldUnits = brickGraph.PageUnit;
				Image image = null;
				try {
					brickGraph.PageUnit = GraphicsUnit.Pixel;
					chart.ResetGraphicsCache();
					chart.LockCrosshairForExport = true;
					image = imageFormat == PrintImageFormat.Metafile ? 
						(Image)chart.CreateMetafile(bounds.Size, MetafileFrameUnit.Pixel) : (Image)chart.CreateBitmap(bounds.Size);
					ImageBrick brick = printingSystem.CreateBrick("ImageBrick") as ImageBrick;
					if (brick != null) {
						chart.LockCrosshairForExport = false;
						brick.Image = image;
						brick.Sides = BorderSide.None;
						brick.DisposeImage = true;
						graph.DrawBrick(brick, bounds);
						image = null;
					}
				}
				finally {
					chart.ResetGraphicsCache();
					chart.ClearCache();
					brickGraph.PageUnit = oldUnits;
					if (image != null)
						image.Dispose();
				}
			}
		}
		public virtual object Clone() {
			ChartPrinter printer = new ChartPrinter(control);
			printer.Assign(this);
			return printer;
		}
		public override bool Equals(object obj) {
			return obj.GetType().Equals(GetType());
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		void OnPrintingSystem_AfterChange(object sender, ChangeEventArgs e) {
			PrintingSystemBase printingSystemBase = printingSystem as PrintingSystemBase;
			LinkBase linkBase = link as LinkBase;
			if (printingSystemBase != null && linkBase != null && (e.EventName == SR.PageSettingsChanged || e.EventName == SR.AfterMarginsChange)) {
				linkBase.Margins = printingSystemBase.PageMargins;
				linkBase.CreateDocument();
			}
		}
		void SetPS(IPrintingSystem ps) {
			if (printingSystem != null)
				printingSystem.AfterChange -= new ChangeEventHandler(OnPrintingSystem_AfterChange);
			printingSystem = ps;
			if (printingSystem != null) {
				printingSystem.SetCommandVisibility(PrintingSystemCommand.ExportGraphic, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.ExportHtm, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.ExportMht, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.ExportPdf, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.ExportRtf, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.ExportXls, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.ExportXlsx, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.SendGraphic, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.SendMht, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.SendPdf, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.SendRtf, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.SendXls, true);
				printingSystem.SetCommandVisibility(PrintingSystemCommand.SendXlsx, true);
				printingSystem.AfterChange += new ChangeEventHandler(OnPrintingSystem_AfterChange);
			}
		}
		SizeF GetPSSize() {
			PrintingSystemBase printingSystemBase = printingSystem as PrintingSystemBase;
			if (printingSystemBase == null)
				return SizeF.Empty;
			Rectangle pageBounds = printingSystemBase.PageBounds;
			System.Drawing.Printing.Margins margins = printingSystemBase.PageMargins;
			float width = GraphicsUnitConverter.Convert(pageBounds.Width - margins.Left - margins.Right - 1,
				GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.UnitToDpi(GraphicsUnit.Pixel));
			float height = GraphicsUnitConverter.Convert(pageBounds.Height - margins.Top - margins.Bottom - 1,
				GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.UnitToDpi(GraphicsUnit.Pixel));
			return new SizeF(width, height);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class ChartPageSettings {
		System.Drawing.Printing.Margins margins = XtraPageSettingsBase.DefaultMargins;
		bool landscape = false;
		PaperKind paperKind = XtraPageSettingsBase.DefaultPaperKind;
		Size customPaperSize = Size.Empty;
		string customPaperName = string.Empty;
		public System.Drawing.Printing.Margins Margins { get { return margins; } set { margins = value; } }
		public bool Landscape { get { return landscape; } set { landscape = value; } }
		public PaperKind PaperKind { get { return paperKind; } set { paperKind = value; } }
		public Size CustomPaperSize { get { return customPaperSize; } set { customPaperSize = value; } }
		public string CustomPaperName { get { return customPaperName; } set { customPaperName = value; } }
		public void Apply(PrintingSystemBase ps) {
			if (ps == null)
				return;
			XtraPageSettingsBase.ApplyPageSettings(ps.PageSettings, paperKind, customPaperSize, margins, new System.Drawing.Printing.Margins(0, 0, 0, 0), landscape, customPaperName);
		}
	}
	public interface IChartPrinter {
		void SetPageSettings(ChartPageSettings settings);
	}
}
