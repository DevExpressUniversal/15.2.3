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
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf.Native {
	internal abstract class PdfDocumentStructureReader {
		public const byte LineFeed = (byte)'\x0a';
		public const byte CarriageReturn = (byte)'\x0d';
		public const byte Comment = (byte)'%';
		protected static readonly PdfTokenDescription EofToken = new PdfTokenDescription(new byte[] { PdfDocumentStructureReader.Comment, PdfDocumentStructureReader.Comment, (byte)'E', (byte)'O', (byte)'F' });
		protected static readonly PdfTokenDescription TrailerToken = new PdfTokenDescription(new byte[] { (byte)'t', (byte)'r', (byte)'a', (byte)'i', (byte)'l', (byte)'e', (byte)'r' });
		internal static void ThrowIncorrectDataException() {
			throw new ArgumentException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectPdfData));
		}
		readonly PdfObjectCollection objects;
		readonly PdfDocumentStream documentStream;
		int rootObjectNumber;
		protected PdfObjectCollection Objects { get { return objects; } }
		protected int RootObjectNumber { get { return rootObjectNumber; } }
		protected PdfDocumentStream DocumentStream { get { return documentStream; } }
		protected PdfDocumentStructureReader(PdfDocumentStream documentStream) {
			this.documentStream = documentStream;
			this.objects = new PdfObjectCollection(documentStream);
		}
		protected virtual void UpdateTrailer(PdfReaderDictionary trailerDictionary, PdfObjectCollection objects) {
			PdfObjectReference reference = trailerDictionary.GetObjectReference(PdfObjectCollection.TrailerRootKey);
			if (reference != null)
				rootObjectNumber = reference.Number;
		}
	}
}
