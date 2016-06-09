#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
namespace DevExpress.Pdf.Drawing {
	public abstract class EmfBitmapHeader {
		const int BitmapCoreHeaderSize = 0x0000000C;
		readonly int width;
		readonly int height;
		readonly EmfBitCount bitCount;
		protected int Width { get { return width; } }
		protected int Height { get { return height; } }
		protected EmfBitCount BitCount { get { return bitCount; } }
		protected EmfBitmapHeader(int width, int height, BinaryReader reader) {
			this.width = width;
			this.height = height;
			reader.ReadInt16();
			this.bitCount = (EmfBitCount)reader.ReadInt16();
		}
		public static EmfBitmapHeader Create(byte[] content) {
			using (BinaryReader reader = new BinaryReader(new MemoryStream(content))) {
				int size = reader.ReadInt32();
				if (size == BitmapCoreHeaderSize)
					return new EmfBitmapCoreHeader(reader);
				return new EmfBitmapInfoHeader(reader);
			}
		}
	}
}
