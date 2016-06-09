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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfOptionsFormFieldOption {
		readonly string text;
		readonly string exportText;
		public string Text { get { return text; } }
		public string ExportText { get { return exportText; } }
		internal PdfOptionsFormFieldOption(PdfReaderDictionary dictionary, object value) {
			byte[] bytes = value as byte[];
			if (bytes == null) {
				IList<object> array = value as IList<object>;
				if (array == null || array.Count != 2)
					PdfDocumentReader.ThrowIncorrectDataException();
				byte[] textBytes = array[0] as byte[];
				byte[] exportTextBytes = array[1] as byte[];
				if (textBytes == null || exportTextBytes == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				text = PdfDocumentReader.ConvertToString(textBytes);
				exportText = PdfDocumentReader.ConvertToString(exportTextBytes);
			}
			else {
				text = PdfDocumentReader.ConvertToString(bytes);
				exportText = text;
			}
		}
		internal object Write() {
			return text == exportText ? (object)text : new object[] { text, exportText };
		}
	}
}
