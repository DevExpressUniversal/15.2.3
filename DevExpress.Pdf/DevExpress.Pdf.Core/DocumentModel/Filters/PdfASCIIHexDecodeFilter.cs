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
	public class PdfASCIIHexDecodeFilter : PdfFilter {
		internal const string Name = "ASCIIHexDecode";
		internal const string ShortName = "AHx";
		const byte nullSymbol = 0x00;
		const byte horizontalTab = 0x09;
		const byte lineFeed = 0x0a;
		const byte formFeed = 0x0c;
		const byte carriageReturn = 0x0d;
		const byte space = 0x20;
		const byte zero = (byte)'0';
		const byte one = (byte)'1';
		const byte two = (byte)'2';
		const byte three = (byte)'3';
		const byte four = (byte)'4';
		const byte five = (byte)'5';
		const byte six = (byte)'6';
		const byte seven = (byte)'7';
		const byte eight = (byte)'8';
		const byte nine = (byte)'9';
		const byte a = (byte)'a';
		const byte b = (byte)'b';
		const byte c = (byte)'c';
		const byte d = (byte)'d';
		const byte e = (byte)'e';
		const byte f = (byte)'f';
		const byte capitalA = (byte)'A';
		const byte capitalB = (byte)'B';
		const byte capitalC = (byte)'C';
		const byte capitalD = (byte)'D';
		const byte capitalE = (byte)'E';
		const byte capitalF = (byte)'F';
		const byte eod = 0x3e;
		protected internal override string FilterName { get { return Name; } }
		internal PdfASCIIHexDecodeFilter() {
		}
		protected internal override byte[] Decode(byte[] data) {
			List<byte> result = new List<byte>(data.Length / 2);
			bool high = true;
			byte decoded = 0;
			foreach (byte element in data) {
				byte digit;
				switch (element) {
					case nullSymbol:
					case horizontalTab:
					case lineFeed:
					case formFeed:
					case carriageReturn:
					case space:
						continue;
					case zero:
					case one:
					case two:
					case three:
					case four:
					case five:
					case six:
					case seven:
					case eight:
					case nine:
						digit = (byte)(element - zero);
						break;
					case a:
					case b:
					case c:
					case d:
					case e:
					case f:
						digit = (byte)(element - a + 10);
						break;
					case capitalA:
					case capitalB:
					case capitalC:
					case capitalD:
					case capitalE:
					case capitalF:
						digit = (byte)(element - capitalA + 10);
						break;
					case eod:
						if (!high)
							result.Add(decoded);
						return result.ToArray();
					default:
						PdfDocumentReader.ThrowIncorrectDataException();
						digit = 0;
						break;
				}
				if (high)
					decoded = (byte)(digit << 4);
				else
					result.Add((byte)(decoded + digit));
				high = !high;
			}
			return result.ToArray();
		}
	}
}
