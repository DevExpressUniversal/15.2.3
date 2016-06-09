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

using System.IO;
using DevExpress.Printing.Core.HtmlExport;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Service.Native.Services.Transient {
	public class HtmlPagesExporter : PagesExporterBase {
		public HtmlPagesExporter(ISerializationService serializationService)
			: base(serializationService) {
		}
		public override bool ExclusivelyDocumentUsing {
			get { return true; }
		}
		protected override string ExportCore(Document document, int pageIndex) {
			var htmlBuilder = new HtmlDocumentBuilderBase();
			var ps = document.PrintingSystem;
			using(var imageRepository = new CssImageRepository())
			using(var htmlControl = new SinglePageWebControl(document, imageRepository, pageIndex, false))
			using(var stringWriter = new StringWriter())
			using(var htmlWriter = new DXHtmlTextWriter(stringWriter)) {
				ps.ReplaceService<IBrickPublisher>(new DefaultBrickPublisher());
				try {
					htmlBuilder.CreateDocumentCore(htmlWriter, htmlControl, imageRepository);
				} finally {
					ps.RemoveService<IBrickPublisher>();
				}
				htmlWriter.Flush();
				return stringWriter.ToString();
			}
		}
	}
}
