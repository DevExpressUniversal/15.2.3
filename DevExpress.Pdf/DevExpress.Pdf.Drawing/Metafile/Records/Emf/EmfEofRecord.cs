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
	public class EmfEofRecord : EmfRecord {
		readonly EmfLogPaletteEntry[] paletteEntries;
		public EmfLogPaletteEntry[] PaletteEntries { get { return paletteEntries; } }
		public EmfEofRecord(byte[] content)
			: base(content) {
			BinaryReader contentStream = ContentStream;
			int entriesCount = contentStream.ReadInt32();
			int entriesOffset = contentStream.ReadInt32();
			if (entriesCount != 0) {
				paletteEntries = new EmfLogPaletteEntry[entriesCount];
				contentStream.BaseStream.Position += entriesOffset - 4 * 4;
				for (int i = 0; i < entriesCount; i++)
					paletteEntries[i] = new EmfLogPaletteEntry(contentStream);
			}
		}
		public override void Execute(EmfMetafileGraphics context) {
			context.RestoreGraphicsState(-1);
		}
	}
}
