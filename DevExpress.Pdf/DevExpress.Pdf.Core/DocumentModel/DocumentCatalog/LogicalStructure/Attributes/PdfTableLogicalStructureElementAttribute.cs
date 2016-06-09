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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public enum PdfTableLogicalStructureElementAttributeScope { Row, Column, Both }
	public class PdfTableLogicalStructureElementAttribute : PdfLogicalStructureElementAttribute {
		internal const string Owner = "Table";
		const string rowSpanKey = "RowSpan";
		const string colSpanKey = "ColSpan";
		const string headersKey = "Headers";
		const string scopeKey = "Scope";
		const string summaryKey = "Summary";
		readonly int rowSpan = 1;
		readonly int colSpan = 1;
		readonly IList<string> headers;
		readonly PdfTableLogicalStructureElementAttributeScope? scope;
		readonly string summary;
		public int RowSpan { get { return rowSpan; } }
		public int ColSpan { get { return colSpan; } }
		public IList<string> Headers { get { return headers; } }
		public PdfTableLogicalStructureElementAttributeScope? Scope { get { return scope; } }
		public string Summary { get { return summary; } }
		internal PdfTableLogicalStructureElementAttribute(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			rowSpan = dictionary.GetInteger(rowSpanKey) ?? 1;
			colSpan = dictionary.GetInteger(colSpanKey) ?? 1;
			headers = dictionary.GetArray<string>(headersKey, o => {
				byte[] header = o as byte[];
				if (header == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				return PdfDocumentReader.ConvertToString(header);
			});
			string str = dictionary.GetName(scopeKey);
			if (!String.IsNullOrEmpty(str))
				scope = PdfEnumToStringConverter.Parse<PdfTableLogicalStructureElementAttributeScope>(str);
			summary = dictionary.GetString(summaryKey);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.AddName(OwnerKey, Owner);
			dictionary.Add(rowSpanKey, rowSpan, 1);
			dictionary.Add(colSpanKey, colSpan, 1);
			if (headers != null)
				dictionary.Add(headersKey, new PdfWritableArray(headers));
			if (scope.HasValue)
				dictionary.AddEnumName(scopeKey, scope.Value);
			dictionary.AddNotNullOrEmptyString(summaryKey, summary);
			return dictionary;
		}
	}
}
