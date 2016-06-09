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
using System.Text;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfUriAction : PdfAction {
		internal const string Name = "URI";
		const string uriDictionaryKey = "URI";
		const string isMapDictuinaryKey = "IsMap";
		readonly string uri;
		readonly bool isMap;
		public string Uri { get { return uri; } }
		public bool IsMap { get { return isMap; } }
		protected override string ActionType { get { return Name; } }
		internal PdfUriAction(PdfDocumentCatalog documentCatalog, Uri uri) : base(documentCatalog) {
			this.uri = uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString;
		}
		internal PdfUriAction(PdfReaderDictionary dictionary) : base(dictionary) {
			byte[] uri = dictionary.GetBytes(uriDictionaryKey);
			this.uri = uri == null ? String.Empty : Encoding.ASCII.GetString(uri, 0, uri.Length);
			isMap = dictionary.GetBoolean(isMapDictuinaryKey) ?? false;
		}
		protected internal override void Execute(IPdfInteractiveOperationController interactiveOperationController, IList<PdfPage> pages) {
			interactiveOperationController.OpenUri(uri);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.Add(uriDictionaryKey, Encoding.ASCII.GetBytes(uri));
			dictionary.Add(isMapDictuinaryKey, isMap, false);
			return dictionary;
		}
	}
}
