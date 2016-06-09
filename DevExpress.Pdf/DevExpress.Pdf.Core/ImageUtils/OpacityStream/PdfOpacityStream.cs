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
	public class PdfOpacityStream : IOpacityStream {
		readonly int stride;
		readonly byte paintValue;
		readonly byte maskValue;
		readonly byte[] data;
		int position = -1;
		byte currentBit = 0;
		byte currentByte = 0;
		int stridePosition;
		public PdfOpacityStream(int width, PdfRange decode, byte[] data) {
			this.data = data;
			if (decode.Min <= decode.Max) {
				paintValue = 255;
				maskValue = 0;
			}
			else {
				paintValue = 0;
				maskValue = 255;
			}
			stride = width;
		}
		byte IOpacityStream.GetNextValue() {
			if (stridePosition >= stride) { 
				stridePosition = 0;
				currentBit = 0;
			}
			currentBit >>= 1;
			if (currentBit == 0) {
				currentByte = data[++position];
				currentBit = 0x80;
			}
			stridePosition++;
			return (currentByte & currentBit) == 0 ? paintValue : maskValue;
		}
	}
}
