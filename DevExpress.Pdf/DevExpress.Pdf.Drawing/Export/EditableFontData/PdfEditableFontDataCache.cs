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
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.Pdf.Drawing {
	public class PdfEditableFontDataCache : PdfDisposableObject {
		readonly IDictionary<Tuple<string, FontStyle>, PdfEditableFontData> fontCache = new Dictionary<Tuple<string, FontStyle>, PdfEditableFontData>();
		readonly PdfDocumentCatalog documentCatalog;
		internal Guid ObjectsId { get { return documentCatalog == null ? default(Guid) : documentCatalog.Objects.Id; } }
		PdfCreationOptions CreationOptions { get { return documentCatalog == null ? null : documentCatalog.CreationOptions; } }
		public PdfEditableFontDataCache(PdfObjectCollection objects) {
			if (objects != null)
				this.documentCatalog = objects.DocumentCatalog;
		}
		public void UpdateFonts() {
			foreach (PdfEditableFontData fontData in fontCache.Values)
				fontData.UpdateFont();
		}
		public PdfEditableFontData GetEditableFontData(FontStyle fontStyle, string fontFamily) {
			PdfCreationOptions creationOptions = CreationOptions;
			PdfEditableFontData editableFontData;
			Tuple<string, FontStyle> fontId = new Tuple<string, FontStyle>(fontFamily, fontStyle);
			if (!fontCache.TryGetValue(fontId, out editableFontData)) {
				editableFontData = PdfEditableFontData.Create(fontStyle, fontFamily, creationOptions == null || creationOptions.EmbedFont(fontFamily));
				fontCache.Add(fontId, editableFontData);
			}
			return editableFontData;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				foreach (PdfEditableFontData fontData in fontCache.Values)
					fontData.Dispose();
				fontCache.Clear();
			}
		}
	}
}
