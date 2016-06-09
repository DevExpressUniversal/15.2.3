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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfIdentityEncoding : PdfCompositeFontEncoding {
		internal const string HorizontalIdentityName = "Identity-H";
		internal const string VerticalIdentityName = "Identity-V";
		static PdfIdentityEncoding horizontalIdentity = new PdfIdentityEncoding(false);
		static PdfIdentityEncoding verticalIdentity = new PdfIdentityEncoding(true);
		public static PdfIdentityEncoding HorizontalIdentity { get { return horizontalIdentity; } }
		public static PdfIdentityEncoding VerticalIdentity { get { return verticalIdentity; } }
		readonly bool isVertical;
		public override bool IsVertical { get { return isVertical; } }
		PdfIdentityEncoding(bool isVertical) {
			this.isVertical = isVertical;
		}
		protected internal override object Write(PdfObjectCollection objects) {
			return new PdfName(Object.ReferenceEquals(this, PdfIdentityEncoding.VerticalIdentity) ? VerticalIdentityName : HorizontalIdentityName);
		}
		protected internal override PdfStringData GetStringData(byte[] bytes, double[] glyphOffsets) {
			int length = bytes.Length / 2;
			short[] chars = new short[length];
			byte[][] charCodes = new byte[length][];
			double[] offsets = new double[length + 1];
			if (glyphOffsets == null)
				for (int i = 0, byteIndex = 0; i < length; i++) {
					byte highByte = bytes[byteIndex++];
					byte lowByte = bytes[byteIndex++];
					int value = highByte << 8;
					chars[i] = (short)(value + lowByte);
					charCodes[i] = new byte[] { highByte, lowByte };
				}
			else 
				for (int i = 0, byteIndex = 0; i < length; i++) {
					offsets[i] = glyphOffsets[byteIndex];
					byte highByte = bytes[byteIndex++];
					byte lowByte = bytes[byteIndex++];
					int value = highByte << 8;
					chars[i] = (short)(value + lowByte);
					charCodes[i] = new byte[] { highByte, lowByte };
				}
			return new PdfStringData(charCodes, chars, offsets);
		}
	}
}
