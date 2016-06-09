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
using System.IO;
using System.Linq;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Service.Native.Services.Factories;
namespace DevExpress.XtraReports.Service.Native.Services.Transient {
	class ExclusivelyDocumentPagesExporterWrapper : IPagesExporter {
		readonly IPagesExporter originalExporter;
		readonly XtraSerializer printingSystemSerializer;
		readonly IPrintingSystemFactory printingSystemFactory;
		public ExclusivelyDocumentPagesExporterWrapper(IPagesExporter originalExporter, XtraSerializer printingSystemSerializer, IPrintingSystemFactory printingSystemFactory) {
			Guard.ArgumentNotNull(originalExporter, "originalExporter");
			Guard.ArgumentNotNull(printingSystemSerializer, "printingSystemSerializer");
			Guard.ArgumentNotNull(printingSystemFactory, "printingSystemFactory");
			this.originalExporter = originalExporter;
			this.printingSystemSerializer = printingSystemSerializer;
			this.printingSystemFactory = printingSystemFactory;
		}
		#region IExportWrapper Members
		public bool ExclusivelyDocumentUsing {
			get { return false; }
		}
		public string Export(Document document, int pageIndex) {
			var pageIndexes = new[] { pageIndex };
			return ActWithCopiedDocument(document, pageIndexes, d => originalExporter.Export(d, pageIndex));
		}
		public byte[] Export(Document document, int[] pageIndexes) {
			return ActWithCopiedDocument(document, pageIndexes, d => originalExporter.Export(d, pageIndexes));
		}
		#endregion
		T ActWithCopiedDocument<T>(Document document, int[] pageIndexes, Func<Document, T> func) {
			Guard.ArgumentNotNull(func, "func");
			var pages = pageIndexes.Select(i => document.Pages[i]).ToArray();
			using(var printingSystem = printingSystemFactory.CreatePrintingSystem())
			using(var stream = new MemoryStream()) {
				document.SerializeCore(stream, printingSystemSerializer, ContinuousExportInfo.Empty, pages);
				printingSystem.LoadDocument(stream);
				return func(printingSystem.Document);
			}
		}
	}
}
