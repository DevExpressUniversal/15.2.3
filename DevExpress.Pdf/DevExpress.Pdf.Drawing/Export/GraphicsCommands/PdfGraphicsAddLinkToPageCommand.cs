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

using DevExpress.Pdf.Native;
using System;
using System.Drawing;
namespace DevExpress.Pdf.Drawing {
	class PdfGraphicsAddLinkToPageCommand : PdfGraphicsAddLinkCommand {
		readonly int pageNumber;
		readonly float? x;
		readonly float? y;
		readonly float? zoom;
		readonly string destinationName;
		public PdfGraphicsAddLinkToPageCommand(RectangleF linkArea, int pageNumber, float? x, float? y, float? zoom)
			: base(linkArea) {
			this.pageNumber = pageNumber;
			this.x = x;
			this.y = y;
			this.zoom = zoom;
			PdfXYZDestination.ValidateZoomValue(zoom);
		}
		public PdfGraphicsAddLinkToPageCommand(RectangleF linkArea, string destinationName) : base (linkArea) {
			this.destinationName = destinationName;
		}
		protected override PdfLinkAnnotation CreateLinkAnnotation(PdfPage page, PdfRectangle rect, PdfGraphicsPageContentsCommandConstructor constructor) {
			string destinationName = this.destinationName;
			if (String.IsNullOrEmpty(destinationName)) {
				double? left = x.HasValue ? (double?)constructor.TransformX(x.Value) : null;
				double? top = y.HasValue ? (double?)constructor.TransformY(y.Value) : null;
				PdfDocumentCatalog catalog = page.DocumentCatalog;
				destinationName = catalog.Names.AddDestination(new PdfXYZDestination(catalog, pageNumber - 1, left, top, zoom));
			}
			return new PdfLinkAnnotation(page, rect, destinationName);
		}
	}
}
