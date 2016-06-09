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

namespace DevExpress.Pdf.Native {
	public abstract class PdfType1FontEexecCipher : PdfType1FontCipher {
		public static PdfType1FontEexecCipher Create(byte[] data, int startPosition, int dataLength) {
			const int kindBytesCount = 4;
			if (dataLength < kindBytesCount)
				return null;
			bool isASCII;
			if (PdfObjectParser.IsHexadecimalDigitSymbol(data[startPosition])) {
				isASCII = true;
				for (int i = 1, index = startPosition + 1; i < kindBytesCount; i++) {
					byte c = data[index++];
					if (!PdfObjectParser.IsSpaceSymbol(c) && !PdfObjectParser.IsHexadecimalDigitSymbol(c)) {
						isASCII = false;
						break;
					}
				}
			}
			else 
				isASCII = false;
			return isASCII ? (PdfType1FontEexecCipher)new PdfType1FontEexecASCIICipher(data, startPosition, dataLength) : 
							 (PdfType1FontEexecCipher)new PdfType1FontEexecBinaryCipher(data, startPosition, dataLength);
		}
		protected override int SkipBytesCount { get { return 4; } }
		protected override int R { get { return 55665; } }
		protected PdfType1FontEexecCipher(byte[] data, int startPosition, int dataLength) : base(data, startPosition, dataLength) {
		}
	}
}
