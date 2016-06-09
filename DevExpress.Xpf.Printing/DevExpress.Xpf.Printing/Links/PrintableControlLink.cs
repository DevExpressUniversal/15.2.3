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

using DevExpress.Utils;
using DevExpress.Xpf.Printing.BrickCollection;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.DataNodes;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Printing {
	public class PrintableControlLink : TemplatedLink {
		readonly IPrintableControl printableControl;
		bool disposed;
#if DEBUGTEST
		internal PrintableControlLink(IRootDataNode rootNode, string documentName)
			: base(documentName) {
			Guard.ArgumentNotNull(rootNode, "rootNode");
			RootNode = rootNode;
		}
		internal PrintableControlLink(IRootDataNode rootNode)
			: this(rootNode, string.Empty) {
		}
#endif
		public PrintableControlLink(IPrintableControl printableControl, string documentName)
			: base(documentName) {
			Guard.ArgumentNotNull(printableControl, "printableControl");
			this.printableControl = printableControl;
			PrintingSystem.PageInsertComplete += PrintingSystem_PageInsertComplete;
		}
		public PrintableControlLink(IPrintableControl printableControl)
			: this(printableControl, string.Empty) {
		}
		protected override void Build(bool buildPagesInBackground) {
			if(printableControl == null || !buildPagesInBackground || !printableControl.CanCreateRootNodeAsync)
				base.Build(buildPagesInBackground);
			else {
				printableControl.CreateRootNodeCompleted += printableControl_CreateRootNodeCompleted;
				CreateRootNodeAsync();
			}
		}
		protected override void BuildCore() {
			if(printableControl != null) {
				IVisualTreeWalker walker = printableControl.GetCustomVisualTreeWalker();
				if(walker != null)
					BrickCollector.VisualTreeWalker = walker;
			}
			base.BuildCore();
		}
		protected override void Dispose(bool disposing) {
			if(!disposed) {
				if(disposing) {
					if(printableControl != null && printableControl.CanCreateRootNodeAsync)
						printableControl.CreateRootNodeCompleted -= printableControl_CreateRootNodeCompleted;
					if(PrintingSystem != null) {
						PrintingSystem.PageInsertComplete -= PrintingSystem_PageInsertComplete;
					}
				}
				disposed = true;
			}
			base.Dispose(disposing);
		}
		protected override IRootDataNode CreateRootNode() {
			if(printableControl == null)
				return null;
			DocumentBand band = PrintingSystem.PrintingDocument.Root;
			IRootDataNode rootNode = printableControl.CreateRootNode(
				DrawingConverter.ToSize(PrintingSystem.PageSettings.UsablePageSizeInPixels),
				GetBoundsSize(band.GetBand(DocumentBandKind.ReportHeader)),
				GetBoundsSize(band.GetBand(DocumentBandKind.ReportFooter)),
				GetBoundsSize(band.GetBand(DocumentBandKind.PageHeader)),
				GetBoundsSize(band.GetBand(DocumentBandKind.PageFooter)));
			return rootNode;
		}
		void CreateRootNodeAsync() {
			DocumentBand band = PrintingSystem.PrintingDocument.Root;
			printableControl.CreateRootNodeAsync(
				DrawingConverter.ToSize(PrintingSystem.PageSettings.UsablePageSizeInPixels),
				GetBoundsSize(band.GetBand(DocumentBandKind.ReportHeader)),
				GetBoundsSize(band.GetBand(DocumentBandKind.ReportFooter)),
				GetBoundsSize(band.GetBand(DocumentBandKind.PageHeader)),
				GetBoundsSize(band.GetBand(DocumentBandKind.PageFooter)));
		}
		void printableControl_CreateRootNodeCompleted(object sender, Data.Utils.ServiceModel.ScalarOperationCompletedEventArgs<IRootDataNode> e) {
			printableControl.CreateRootNodeCompleted -= printableControl_CreateRootNodeCompleted;
			if(disposed)
				return;
			RootNode = e.Result;
			BuildCore();
			RaiseDocumentBuildingCompleted();
		}
		void PrintingSystem_PageInsertComplete(object sender, PageInsertCompleteEventArgs e) {
			if(printableControl != null) {
				BrickEnumerator pageBrickEnumerator = new BrickEnumerator(PrintingSystem.Pages[e.PageIndex]);
				printableControl.PagePrintedCallback(pageBrickEnumerator, BrickCollector.BrickUpdaters);
			}
		}
		System.Windows.Size GetBoundsSize(DocumentBand band) {
			System.Drawing.SizeF size = new System.Drawing.SizeF(band.BrickBounds.Size.Width, band.SelfHeight);
			return DrawingConverter.ToSize(GraphicsUnitConverter2.DocToPixel(size));
		}
	}
}
