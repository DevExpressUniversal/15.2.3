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
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
namespace DevExpress.Xpf.Printing {
	public class LegacyPrintableComponentLink : LinkBase {
		readonly IPrintable printableComponent;
		readonly PrintableComponentLinkBase legacyLink;
		public IPrintable PrintableComponent { get { return printableComponent; } }
		public LegacyPrintableComponentLink(IPrintable printableComponent, string documentName)
			: base(documentName) {
			Guard.ArgumentNotNull(printableComponent, "printableComponent");
			this.printableComponent = printableComponent;
			legacyLink = new PrintableComponentLinkBase(PrintingSystem) { Component = printableComponent };
			PrintingSystem.DocumentFactory = new PSLinkDocumentFactory();
		}
		public LegacyPrintableComponentLink(IPrintable printableComponent)
			: this(printableComponent, string.Empty) {
		}
		protected override void CreateDocumentCore(bool buildPagesInBackground, bool applyPageSettings) {
			if(applyPageSettings) {
				legacyLink.PaperKind = PaperKind;
				legacyLink.CustomPaperSize = CustomPaperSize;
				legacyLink.Margins = Margins;
				legacyLink.MinMargins = MinMargins;
				legacyLink.Landscape = Landscape;
			}
			PrintingSystem.Document.Name = DocumentName;
			legacyLink.VerticalContentSplitting = VerticalContentSplitting;
			legacyLink.CreateDocument(buildPagesInBackground);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				legacyLink.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
