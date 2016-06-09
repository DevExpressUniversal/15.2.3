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
	public class PdfXFAForm {
		readonly string content;
		public string Content { get { return content; } }
		internal PdfXFAForm(byte[] data) {
			content = Encoding.UTF8.GetString(data, 0, data.Length);
		}
		internal PdfXFAForm(PdfObjectCollection collection, IList<object> array) {
			int count = array.Count;
			if (count % 2 != 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			count /= 2;
			content = String.Empty;
			for (int i = 0, index = 0; i < count; i++) {
				byte[] name = array[index++] as byte[];
				if (name == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				PdfObjectReference reference = array[index++] as PdfObjectReference;
				if (reference == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				PdfReaderStream stream = collection.GetStream(reference.Number);
				if (stream == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				if (i > 0)
					content = content + "\n";
				byte[] data = stream.GetData(true);
				content = content + Encoding.UTF8.GetString(data, 0, data.Length);
			}
		}
		internal PdfCompressedStream Write() {
			return new PdfCompressedStream(new PdfDictionary(), content);
		}
	}
}
