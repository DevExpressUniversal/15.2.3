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
using System.Drawing.Printing;
using System.Windows;
using DevExpress.Xpf.Printing.Exports;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.XamlExport;
namespace DevExpress.Xpf.Printing {
	public abstract partial class LinkBase : DependencyObject, ILink2, IDisposable {
		#region Fields & Properties
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("LinkBasePrintingSystem")]
#endif
		public PrintingSystem PrintingSystem { get; private set; }
		IPrintingSystem ILink.PrintingSystem { get { return PrintingSystem; } }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("LinkBaseSuppressAutoRebuildOnPageSettingsChange")]
#endif
		public bool SuppressAutoRebuildOnPageSettingsChange { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("LinkBaseVerticalContentSplitting")]
#endif
		public VerticalContentSplitting VerticalContentSplitting { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("LinkBaseLandscape")]
#endif
		public bool Landscape { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("LinkBasePaperKind")]
#endif
		public PaperKind PaperKind { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("LinkBaseCustomPaperSize")]
#endif
		public System.Drawing.Size CustomPaperSize { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("LinkBaseMargins")]
#endif
		public Margins Margins { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("LinkBaseMinMargins")]
#endif
		public Margins MinMargins { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("LinkBaseDocumentName")]
#endif
		public string DocumentName { get; set; }
		internal bool IsPrintingSystemOwner { get; private set; }
		internal bool BuildPagesInBackground { get; private set; }
		#endregion
		#region Events
		public event EventHandler<EventArgs> CreateDocumentStarted;
		public event EventHandler<EventArgs> CreateDocumentFinished;
		#endregion
		#region ctor
		protected LinkBase(PrintingSystem ps, string documentName) {
			if(ps != null) {
				PrintingSystem = ps;
			} else {
				PrintingSystem = new PrintingSystem();
				IsPrintingSystemOwner = true;
			}
			PrintingSystem.PageSettingsChanged += ps_PageSettingsChanged;
			PrintingSystem.AfterBuildPages += PrintingSystem_AfterBuildPages;
			PrintingSystem.ReplaceService(typeof(XpsExportServiceBase), new XpsExportService(CreatePaginator()));
			DocumentName = documentName;
			PaperKind = XtraPageSettingsBase.DefaultPaperKind;
			Margins = XtraPageSettingsBase.DefaultMargins;
			MinMargins = XtraPageSettingsBase.DefaultMinMargins;
		}
		protected LinkBase(PrintingSystem ps)
			: this(ps, string.Empty) {
		}
		protected LinkBase(string documentName)
			: this(null, documentName) {
		}
		protected LinkBase()
			: this(null, string.Empty) {
		}
		#endregion
		#region Methods
		public void CreateDocument() {
			CreateDocument(false);
		}
		public void CreateDocument(bool buildPagesInBackground) {
			BuildPagesInBackground = buildPagesInBackground;
			CreateDocument(buildPagesInBackground, true);
		}
		public void StopPageBuilding() {
			if(PrintingSystem != null)
				PrintingSystem.Document.StopPageBuilding();
		}
		internal void CreateIfEmpty(bool buildPagesInBackground) {
			if(PrintingSystem.Document.PageCount == 0)
				CreateDocument(buildPagesInBackground);
		}
		void CreateDocument(bool buildPagesInBackground, bool applyPageSettings) {
			OnCreateDocumentStarted();
			CreateDocumentCore(buildPagesInBackground, applyPageSettings);
		}
		protected abstract void CreateDocumentCore(bool buildPagesInBackground, bool applyPageSettings);
		protected virtual void AfterBuildPages() {
			OnCreateDocumentFinished();
		}
		protected internal FrameworkElement VisualizePage(int pageIndex) {
			PageVisualizer strategy = CreatePageVisualizer();
			FrameworkElement page;
			try {
				page = strategy.Visualize((PSPage)PrintingSystem.Pages[pageIndex], pageIndex, PrintingSystem.Pages.Count);
			} catch(System.Windows.Markup.XamlParseException) {
				page = new PageWithRedCross();
			}
			return page;
		}
		protected virtual PageVisualizer CreatePageVisualizer() {
			return new BrickPageVisualizer(TextMeasurementSystem.NativeXpf);
		}
		void ps_PageSettingsChanged(object sender, EventArgs e) {
			if(!SuppressAutoRebuildOnPageSettingsChange)
				CreateDocument(BuildPagesInBackground, false);
		}
		void PrintingSystem_AfterBuildPages(object sender, EventArgs e) {
			AfterBuildPages();
		}
		void OnCreateDocumentStarted() {
			if(CreateDocumentStarted != null)
				CreateDocumentStarted(this, EventArgs.Empty);
		}
		void OnCreateDocumentFinished() {
			if(CreateDocumentFinished != null)
				CreateDocumentFinished(this, EventArgs.Empty);
		}
		void ILink2.AddSubreport(PrintingSystemBase ps, DocumentBand band, System.Drawing.PointF offset) {
			throw new NotImplementedException();
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(PrintingSystem != null) {
					PrintingSystem.PageSettingsChanged -= ps_PageSettingsChanged;
					PrintingSystem.AfterBuildPages -= PrintingSystem_AfterBuildPages;
				}
				if(PrintingSystem != null) {
					ImageRepository.Clear(RepositoryImageHelper.GetDocumentId(PrintingSystem));
				}
				if(IsPrintingSystemOwner && PrintingSystem != null) {
					PrintingSystem.Dispose();
					PrintingSystem = null;
				}
			}
		}
		#endregion
	}
}
