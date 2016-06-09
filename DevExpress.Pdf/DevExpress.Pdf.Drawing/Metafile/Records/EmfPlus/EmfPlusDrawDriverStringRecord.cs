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

using System.Drawing;
namespace DevExpress.Pdf.Drawing {
	public class EmfPlusDrawDriverStringRecord : EmfPlusDrawRecord {
		readonly int fontId;
		readonly int brushId;
		readonly Color? brushColor;
		readonly char[] glyphs;
		readonly PointF[] positions;
		readonly PdfTransformationMatrix transform;
		readonly EmfPlusDriverStringOptions stringOptions;
		public EmfPlusDrawDriverStringRecord(short flags, byte[] content)
			: base(flags, content) {
			if ((Flags & 0x8000) != 0)
				brushColor = ContentStream.ReadColor();
			else
				brushId = ContentStream.ReadInt32();
			fontId = Flags & emfPlusObjectIdMask;
			stringOptions = (EmfPlusDriverStringOptions)ContentStream.ReadInt32();
			bool isMatrixPresent = ContentStream.ReadInt32() != 0;
			int glyphCount = ContentStream.ReadInt32();
			glyphs = new char[glyphCount];
			for (int i = 0; i < glyphCount; i++)
				glyphs[i] = (char)ContentStream.ReadUInt16();
			positions = new PointF[glyphCount];
			for (int i = 0; i < glyphCount; i++)
				positions[i] = ContentStream.ReadPointF(false);
			if (isMatrixPresent)
				transform = ContentStream.ReadTransformMatrix();
		}
		protected override void Draw(EmfMetafileGraphics context) {
			if (stringOptions.HasFlag(EmfPlusDriverStringOptions.DriverStringOptionsCMapLookup)) 
				context.DrawUnicodeString(glyphs, positions, fontId, brushId, brushColor, transform);
		}
	}
}
