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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public abstract class PdfFilter {
		internal static PdfFilter Create(string name, PdfReaderDictionary parameters) {
			switch (name) {
				case PdfASCIIHexDecodeFilter.Name:
				case PdfASCIIHexDecodeFilter.ShortName:
					return new PdfASCIIHexDecodeFilter();
				case PdfASCII85DecodeFilter.Name:
				case PdfASCII85DecodeFilter.ShortName:
					return new PdfASCII85DecodeFilter();
				case PdfFlateDecodeFilter.Name:
				case PdfFlateDecodeFilter.ShortName:
					return new PdfFlateDecodeFilter(parameters);
				case PdfLZWDecodeFilter.Name:
				case PdfLZWDecodeFilter.ShortName:
					return new PdfLZWDecodeFilter(parameters);
				case PdfRunLengthDecodeFilter.Name:
				case PdfRunLengthDecodeFilter.ShortName:
					return new PdfRunLengthDecodeFilter();
				case PdfCCITTFaxDecodeFilter.Name:
				case PdfCCITTFaxDecodeFilter.ShortName:
					return new PdfCCITTFaxDecodeFilter(parameters);
				case PdfJBIG2DecodeFilter.Name:
					return new PdfJBIG2DecodeFilter(parameters);
				case PdfDCTDecodeFilter.Name:
				case PdfDCTDecodeFilter.ShortName:
					return new PdfDCTDecodeFilter(parameters);
				case PdfJPXDecodeFilter.Name:
					return new PdfJPXDecodeFilter();
				case PdfCryptFilter.Name:
					return new PdfCryptFilter();
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					return null;
			}
		}
		protected internal virtual PdfImageDataType ImageDataType { get { return PdfImageDataType.Raw; } }
		protected internal abstract string FilterName { get; }
		protected PdfFilter() {
		}
		protected internal virtual PdfDictionary Write(PdfObjectCollection objects) {
			return null;
		}
		protected internal abstract byte[] Decode(byte[] data);
	}
}
