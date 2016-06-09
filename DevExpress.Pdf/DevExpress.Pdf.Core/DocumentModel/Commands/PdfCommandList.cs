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

using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfCommandList : List<PdfCommand> {
		public PdfCommandList() : base() {
		}
		public PdfCommandList(IEnumerable<PdfCommand> commands) : base(commands) {
		}
		internal byte[] ToByteArray(PdfResources resources) {
			using (MemoryStream stream = new MemoryStream()) {
				PdfDocumentStream documentStream = new PdfDocumentStream(stream);
				foreach (PdfCommand command in this)
					command.Write(resources, documentStream);
				return stream.ToArray();
			}
		}
		internal PdfStream ToStream(PdfResources resources, PdfDictionary dictionary) {
			dictionary.Add(PdfReaderStream.FilterDictionaryKey, new PdfName(PdfFlateDecodeFilter.Name));
			using (PdfFlateEncoder encoder = new PdfFlateEncoder()) {
				PdfCommandsStream commandsStream;
				using (BufferedStream bufferedStream = new BufferedStream(encoder.DeflateStream)) {
					commandsStream = new PdfCommandsStream(bufferedStream);
					foreach (PdfCommand command in this)
						command.Write(resources, commandsStream);
				}
				encoder.Close();
				commandsStream.Adler32.Write(encoder.Stream);
				return new PdfStream(dictionary, encoder.GetData());
			}
		}
		internal PdfStream ToStream(PdfResources resources) {
			return ToStream(resources, new PdfDictionary());
		}
		internal PdfReaderStream ToReaderStream(PdfResources resources) {
			PdfReaderDictionary dictionary = new PdfReaderDictionary(resources.Objects, 0, 0);
			return new PdfReaderStream(dictionary, ToStream(resources, dictionary).RawData, null);
		}
	}
}
