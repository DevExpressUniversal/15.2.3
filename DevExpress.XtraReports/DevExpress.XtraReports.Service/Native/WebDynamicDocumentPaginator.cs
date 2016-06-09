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
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using DevExpress.Utils;
using DevExpress.XtraReports.Service.Native.Services.Transient;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Service.Native {
	public class WebDynamicDocumentPaginator : DynamicDocumentPaginator, IDocumentPaginatorSource {
		readonly IXamlPagesExporter xamlPagesExporter;
		readonly Document document;
		public WebDynamicDocumentPaginator(IXamlPagesExporter xamlPagesExporter, Document document) {
			Guard.ArgumentNotNull(xamlPagesExporter, "xamlPagesExporter");
			Guard.ArgumentNotNull(document, "document");
			this.xamlPagesExporter = xamlPagesExporter;
			this.document = document;
			xamlPagesExporter.Compatibility = DevExpress.XtraPrinting.XamlExport.XamlCompatibility.WPF;
		}
		public override DocumentPage GetPage(int pageNumber) {
			using(var stream = new MemoryStream()) {
				xamlPagesExporter.Export(document, pageNumber, stream);
				stream.Position = 0;
				var content = (FrameworkElement)XamlReader.Load(stream);
				var pageSize = new Size(content.Width, content.Height);
				var finalRect = new Rect(pageSize);
				content.Arrange(finalRect);
				return new DocumentPage(content, pageSize, finalRect, finalRect);
			}
		}
		public override bool IsPageCountValid {
			get { return true; }
		}
		public override int PageCount {
			get { return document.PageCount; }
		}
		public override IDocumentPaginatorSource Source {
			get { return this; }
		}
		#region Unnecessary
		public override ContentPosition GetObjectPosition(object value) {
			throw new NotSupportedException();
		}
		public override int GetPageNumber(ContentPosition contentPosition) {
			throw new NotSupportedException();
		}
		public override ContentPosition GetPagePosition(DocumentPage page) {
			throw new NotSupportedException();
		}
		public override Size PageSize {
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}
		#endregion
		#region IDocumentPaginatorSource Members
		DocumentPaginator IDocumentPaginatorSource.DocumentPaginator {
			get { return this; }
		}
		#endregion
	}
}
