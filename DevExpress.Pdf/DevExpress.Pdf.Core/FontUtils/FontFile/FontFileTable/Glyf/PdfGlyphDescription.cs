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
namespace DevExpress.Pdf.Native {
	public class PdfGlyphDescription {
		internal const int HeaderSize = 10;
		const ushort ARG_1_AND_2_ARE_WORDS = 1;
		const ushort WE_HAVE_A_SCALE = 8;
		const ushort MORE_COMPONENTS = 32;
		const ushort WE_HAVE_AN_X_AND_Y_SCALE = 64;
		const ushort WE_HAVE_A_TWO_BY_TWO = 128;
		readonly short numberOfContours;
		readonly byte[] data;
		readonly List<int> glyphIndexList = new List<int>();
		public bool IsEmpty { get { return numberOfContours == 0; } }
		public int Size { get { return data.Length + 2; } }
		public IEnumerable<int> AdditionalGlyphIndices { get { return glyphIndexList; } }
		public PdfGlyphDescription(PdfBinaryStream stream, int glyphDataSize) {
			long glyphStart = stream.Position;
			numberOfContours = stream.ReadShort();
			data = stream.ReadArray(glyphDataSize - 2);
			if (numberOfContours < 0) {
				stream.Position = glyphStart + HeaderSize;
				ushort flags = 0;
				do {
					flags = (ushort)stream.ReadUshort();
					glyphIndexList.Add(stream.ReadUshort());
					if ((flags & ARG_1_AND_2_ARE_WORDS) != 0)
						stream.Position += 4;
					else
						stream.Position += 2;
					if ((flags & WE_HAVE_A_SCALE) != 0)
						stream.Position += 2;
					else if ((flags & WE_HAVE_AN_X_AND_Y_SCALE) != 0)
						stream.Position += 4;
					else if ((flags & WE_HAVE_A_TWO_BY_TWO) != 0)
						stream.Position += 8;
				} while ((flags & MORE_COMPONENTS) != 0);
			}
		}
		public void Write(PdfBinaryStream stream) {
			stream.WriteShort(numberOfContours);
			stream.WriteArray(data);
		}
	}
}
