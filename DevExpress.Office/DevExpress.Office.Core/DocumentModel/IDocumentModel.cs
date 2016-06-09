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
using System.Drawing;
using System.IO;
using DevExpress.Office.Drawing;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Office.Model;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Office {
#region IDocumentModel
	public interface IDocumentModel : IServiceProvider {
		UriBasedImageReplaceQueue UriBasedImageReplaceQueue { get; }
		DocumentModelUnitConverter UnitConverter { get; }
		DocumentHistory History { get; }
		IDocumentModelPart MainPart { get; }
		FontCache FontCache { get; }
		IDrawingCache DrawingCache { get; }
		IOfficeTheme OfficeTheme { get; set; }
		void BeginUpdate();
		void EndUpdate();
		T GetService<T>() where T : class;
		bool IsDisposed { get; }
		OfficeReferenceImage CreateImage(Stream stream);
#if !SL
		OfficeImage GetImageById(IUniqueImageId id);
		OfficeReferenceImage CreateImage(Image nativeImage);
		OfficeReferenceImage CreateImage(MemoryStreamBasedImage image);
#endif
	}
	#endregion
	public static class DocumentModelDpi {
		public static readonly float DpiX = DevExpress.XtraPrinting.GraphicsDpi.Pixel;
		public static readonly float DpiY = DevExpress.XtraPrinting.GraphicsDpi.Pixel;
		public static readonly float Dpi = DevExpress.XtraPrinting.GraphicsDpi.Pixel;
	}
}
