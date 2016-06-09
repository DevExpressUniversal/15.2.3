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

using System.Drawing;
using System.Collections.Generic;
using DevExpress.Pdf;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfPageCache {
		readonly Dictionary<PdfPage, Bitmap> dictionary = new Dictionary<PdfPage, Bitmap>();
		readonly List<PdfPage> lastUsedPages = new List<PdfPage>();
		public PdfPageCache() {
		}
		public void BeginDrawing() {
			lastUsedPages.Clear();
		}
		public void EndDrawing() {
			List<PdfPage> pages = new List<PdfPage>(dictionary.Keys);
			foreach (PdfPage page in lastUsedPages)
				pages.Remove(page);
			foreach (PdfPage page in pages) {
				dictionary[page].Dispose();
				dictionary.Remove(page);
			}
		}
		public Bitmap GetBitmap(PdfPage page) {
			Bitmap bitmap;
			if (!dictionary.TryGetValue(page, out bitmap))
				return null;
			lastUsedPages.Add(page);
			return bitmap;
		}
		public void AddBitmap(PdfPage page, Bitmap bitmap) {
			dictionary.Add(page, bitmap);
			lastUsedPages.Add(page);
		}
		public void Clear() {
			foreach (Bitmap bitmap in dictionary.Values)
				bitmap.Dispose();
			dictionary.Clear();
			lastUsedPages.Clear();
		}
	}
}
