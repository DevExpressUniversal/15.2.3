#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfPageDataCache : PdfCache<int, PdfPageData> {
		public const long DefaultLimit = 65;
		readonly IList<PdfPage> documentPages;
		readonly bool recognizeAnnotationsData;
		public IList<PdfPage> DocumentPages { get { return documentPages; } }
		public PdfPageDataCache(IList<PdfPage> documentPages, bool recognizeAnnotationsData) : base(DefaultLimit) {
			this.documentPages = documentPages;
			this.recognizeAnnotationsData = recognizeAnnotationsData;
		}
		public IList<PdfTextLine> GetPageLines(int pageIndex) {
			return this[pageIndex].TextData;
		}
		public IList<PdfPageImageData> GetImageData(int pageIndex) {
			return this[pageIndex].ImageData;
		}
		protected override PdfPageData GetData(int key) {
			try {
				return PdfDataRecognizer.Recognize(documentPages[key], recognizeAnnotationsData);
			}
			catch {
				return new PdfPageData(new List<PdfTextLine>(), new List<PdfPageImageData>());
			}
		}
		protected override long GetSizeOfValue(PdfPageData value) {
			return 1;
		}
	}
}
