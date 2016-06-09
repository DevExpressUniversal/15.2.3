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
using System.Windows;
using DevExpress.Xpf.Printing.BrickCollection;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.DataNodes;
using DevExpress.XtraPrinting.Native;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Printing {
	public abstract class TemplatedLink : LinkBase {
		#region Fields & Properties
		const float buildPagesRange = 75;
		const float postBuildPagesRange = 25;
		DocumentBandInitializer bandInitializer;
		IRootDataNode rootNode;
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkPrintReportFooterAtBottom")]
#endif
		public bool PrintReportFooterAtBottom { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkColumnWidth")]
#endif
		public float ColumnWidth { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkColumnLayout")]
#endif
		public ColumnLayout ColumnLayout { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkTopMarginTemplate")]
#endif
		public DataTemplate TopMarginTemplate { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkBottomMarginTemplate")]
#endif
		public DataTemplate BottomMarginTemplate { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkPageHeaderTemplate")]
#endif
		public DataTemplate PageHeaderTemplate { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkPageFooterTemplate")]
#endif
		public DataTemplate PageFooterTemplate { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkReportHeaderTemplate")]
#endif
		public DataTemplate ReportHeaderTemplate { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkReportFooterTemplate")]
#endif
		public DataTemplate ReportFooterTemplate { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkReportHeaderData")]
#endif
		public object ReportHeaderData {
			get { return (object)GetValue(ReportHeaderDataProperty); }
			set { SetValue(ReportHeaderDataProperty, value); }
		}
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkReportFooterData")]
#endif
		public object ReportFooterData {
			get { return (object)GetValue(ReportFooterDataProperty); }
			set { SetValue(ReportFooterDataProperty, value); }
		}
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkPageHeaderData")]
#endif
		public object PageHeaderData {
			get { return (object)GetValue(PageHeaderDataProperty); }
			set { SetValue(PageHeaderDataProperty, value); }
		}
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkPageFooterData")]
#endif
		public object PageFooterData {
			get { return (object)GetValue(PageFooterDataProperty); }
			set { SetValue(PageFooterDataProperty, value); }
		}
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkTopMarginData")]
#endif
		public object TopMarginData {
			get { return (object)GetValue(TopMarginDataProperty); }
			set { SetValue(TopMarginDataProperty, value); }
		}
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("TemplatedLinkBottomMarginData")]
#endif
		public object BottomMarginData {
			get { return (object)GetValue(BottomMarginDataProperty); }
			set { SetValue(BottomMarginDataProperty, value); }
		}
		public static readonly DependencyProperty ReportHeaderDataProperty =
			DependencyPropertyManager.Register("ReportHeaderData", typeof(object), typeof(TemplatedLink), new PropertyMetadata(null));
		public static readonly DependencyProperty ReportFooterDataProperty =
			DependencyPropertyManager.Register("ReportFooterData", typeof(object), typeof(TemplatedLink), new PropertyMetadata(null));
		public static readonly DependencyProperty PageHeaderDataProperty =
			DependencyPropertyManager.Register("PageHeaderData", typeof(object), typeof(TemplatedLink), new PropertyMetadata(null));
		public static readonly DependencyProperty PageFooterDataProperty =
			DependencyPropertyManager.Register("PageFooterData", typeof(object), typeof(TemplatedLink), new PropertyMetadata(null));
		public static readonly DependencyProperty TopMarginDataProperty =
			DependencyPropertyManager.Register("TopMarginData", typeof(object), typeof(TemplatedLink), new PropertyMetadata(null));
		public static readonly DependencyProperty BottomMarginDataProperty =
			DependencyPropertyManager.Register("BottomMarginData", typeof(object), typeof(TemplatedLink), new PropertyMetadata(null));
		protected DocumentBandInitializer BandInitializer { get { return bandInitializer; } }
		protected BrickCollector BrickCollector { get; private set; }
		protected internal IRootDataNode RootNode {
			get { return rootNode; }
			set {
				if(value == null || rootNode == value)
					return;
				rootNode = value;
			}
		}
		#endregion
		#region Events
		event EventHandler DocumentBuildingCompleted;
		#endregion
		#region Constructors
		protected TemplatedLink(PrintingSystem ps, string documentName)
			: base(ps, documentName) {
			BrickCollector = new BrickCollector(PrintingSystem);
		}
		protected TemplatedLink(PrintingSystem ps)
			: this(ps, string.Empty) {
		}
		protected TemplatedLink(string documentName)
			: this(null, documentName) {
		}
		protected TemplatedLink()
			: this(null, string.Empty) {
		}
		#endregion
		#region Methods
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(bandInitializer != null) {
					bandInitializer.Clear();
					bandInitializer = null;
				}
				if(BrickCollector != null) {
					BrickCollector.Clear();
					BrickCollector = null;
				}
			}
			base.Dispose(disposing);
		}
		internal RowViewInfo GetTopMarginRowViewInfo(bool allowContentReuse) {
			return TopMarginTemplate == null ? null : new RowViewInfo(TopMarginTemplate, TopMarginData);
		}
		internal RowViewInfo GetBottomMarginRowViewInfo(bool allowContentReuse) {
			return BottomMarginTemplate == null ? null : new RowViewInfo(BottomMarginTemplate, BottomMarginData);
		}
		internal RowViewInfo GetReportHeaderRowViewInfo(bool allowContentReuse) {
			return ReportHeaderTemplate == null ? null : new RowViewInfo(ReportHeaderTemplate, ReportHeaderData);
		}
		internal RowViewInfo GetReportFooterRowViewInfo(bool allowContentReuse) {
			return ReportFooterTemplate == null ? null : new RowViewInfo(ReportFooterTemplate, ReportFooterData);
		}
		internal RowViewInfo GetPageHeaderRowViewInfo(bool allowContentReuse) {
			return PageHeaderTemplate == null ? null : new RowViewInfo(PageHeaderTemplate, PageHeaderData);
		}
		internal RowViewInfo GetPageFooterRowViewInfo(bool allowContentReuse) {
			return PageFooterTemplate == null ? null : new RowViewInfo(PageFooterTemplate, PageFooterData);
		}
		protected override void CreateDocumentCore(bool buildPagesInBackground, bool applyPageSettings) {
			PrintingSystem.Unlock();
			PrintingSystem.Graph.PageUnit = GraphicsUnit.Document;
			PrintingSystem.ClearContent();
			if(bandInitializer != null)
				bandInitializer.Clear();
#if !SL
			ImageRepository.Clear(RepositoryImageHelper.GetDocumentId(PrintingSystem));
#endif
			PrintingSystem.Begin();
			if(applyPageSettings) {
				XtraPageSettingsBase.ApplyPageSettings(PrintingSystem.PageSettings, PaperKind, CustomPaperSize, Margins, MinMargins, Landscape);
			}
			PrintingSystem.ProgressReflector.SetProgressRanges(new float[] { buildPagesRange, postBuildPagesRange });
			PrintingSystem.Document.Name = DocumentName;
			PrintingSystem.PrintingDocument.VerticalContentSplitting = VerticalContentSplitting;
			System.Windows.Size usablePageSize = DrawingConverter.ToSize(PrintingSystem.PageSettings.UsablePageSizeInPixels);
			bandInitializer = new DocumentBandInitializer(BrickCollector, usablePageSize);
			BuildHeaderFooterBands();
			if(buildPagesInBackground) {
				DocumentBuildingCompleted += TemplatedLink_DocumentBuildingCompleted;
				Build(buildPagesInBackground);
			} else {
				Build(buildPagesInBackground);
				PrintingSystem.End(buildPagesInBackground);
			}
		}
		void TemplatedLink_DocumentBuildingCompleted(object sender, EventArgs e) {
			DocumentBuildingCompleted -= TemplatedLink_DocumentBuildingCompleted;
			PrintingSystem.End(BuildPagesInBackground);
		}
		void BuildHeaderFooterBands() {
			BrickCollector.VisualTreeWalker = new VisualTreeWalker();
			DocumentBand band = PrintingSystem.PrintingDocument.Root;
			band.Bands.Add(CreateDocumentBand(DocumentBandKind.TopMargin, GetTopMarginRowViewInfo));
			band.Bands.Add(CreateDocumentBand(DocumentBandKind.ReportHeader, GetReportHeaderRowViewInfo));
			band.Bands.Add(CreateDocumentBand(DocumentBandKind.PageHeader, GetPageHeaderRowViewInfo));
			band.Bands.Add(CreateDocumentBand(DocumentBandKind.PageFooter, GetPageFooterRowViewInfo));
			var reportFooterBand = CreateDocumentBand(DocumentBandKind.ReportFooter, GetReportFooterRowViewInfo);
			reportFooterBand.PrintAtBottom = PrintReportFooterAtBottom;
			band.Bands.Add(reportFooterBand);
			band.Bands.Add(CreateDocumentBand(DocumentBandKind.BottomMargin, GetBottomMarginRowViewInfo));
		}
		protected virtual void Build(bool buildPagesInBackground) {
			RootNode = CreateRootNode();
			BuildCore();
			if(buildPagesInBackground)
				RaiseDocumentBuildingCompleted();
		}
		protected abstract IRootDataNode CreateRootNode();
		protected virtual void BuildCore() {
			VisualDataNodeBandManager manager = new VisualDataNodeBandManager(RootNode, BandInitializer);
			manager.Initialize(PrintingSystem.PrintingDocument.Root);
			if(manager.totalDetailCount == -1)
				PrintingSystem.ProgressReflector.SetProgressRanges(new float[] { float.NaN, float.NaN } );
			if(ColumnWidth > 0) {
				float columnWidth = GraphicsUnitConverter.Convert(ColumnWidth, GraphicsDpi.DeviceIndependentPixel, GraphicsDpi.Document);
				if(columnWidth > 0) {
					int columnCount = GetColumnCount(columnWidth);
					if(columnCount > 0)
						PrintingSystem.PrintingDocument.Root.MultiColumn = new MultiColumn(columnCount, columnWidth, ColumnLayout);
				}
			}
			PrintingSystem.PrintingDocument.Root.BandManager = manager;
		}
		protected override void AfterBuildPages() {
			PrintingSystem.Lock();
			BrickCollector.Clear();
			new DocumentMapBuilder().Build(PrintingSystem.Document);
			BandInitializer.Clear();
			if(RootNode is IDisposable)
				((IDisposable)RootNode).Dispose();
			base.AfterBuildPages();
		}
		protected void RaiseDocumentBuildingCompleted() {
			if(DocumentBuildingCompleted != null)
				DocumentBuildingCompleted(this, EventArgs.Empty);
		}
		DocumentBand CreateDocumentBand(DocumentBandKind bandKind, Func<bool, RowViewInfo> getRowViewInfo) {
			DocumentBand band = new DocumentBand(bandKind);
			BandInitializer.Initialize(band, getRowViewInfo);
			return band;
		}
		int GetColumnCount(float columnWidth) {
			float usablePageWidth = GraphicsUnitConverter.Convert(PrintingSystem.PageSettings.UsablePageSize.Width, GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.Document);
			int result = (int)(usablePageWidth / columnWidth);
			return result > 0 ? result : 1;
		}
		#endregion
	}
}
