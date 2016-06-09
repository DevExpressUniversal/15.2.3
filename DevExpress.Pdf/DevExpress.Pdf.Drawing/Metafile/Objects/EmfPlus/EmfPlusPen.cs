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
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.Pdf.Drawing {
	public class EmfPlusPen {
		readonly PdfPen pen;
		readonly GraphicsUnit unitType;
		public GraphicsUnit UnitType { get { return unitType; } }
		public EmfPlusPen(EmfPlusReader reader) {
			reader.BaseStream.Position += 8;
			EmfPlusPenDataFlags flags = (EmfPlusPenDataFlags)reader.ReadInt32();
			unitType = (GraphicsUnit)reader.ReadInt32();
			pen = new PdfPen(Color.Black, reader.ReadSingle());
			if (flags.HasFlag(EmfPlusPenDataFlags.PenDataTransform))
				pen.Transform = reader.ReadTransformMatrix();
			if (flags.HasFlag(EmfPlusPenDataFlags.PenDataStartCap))
				pen.StartCap = (LineCap)reader.ReadInt32();
			if (flags.HasFlag(EmfPlusPenDataFlags.PenDataEndCap))
				pen.EndCap = (LineCap)reader.ReadInt32();
			if (flags.HasFlag(EmfPlusPenDataFlags.PenDataJoin))
				pen.LineJoin = (LineJoin)reader.ReadInt32();
			if (flags.HasFlag(EmfPlusPenDataFlags.PenDataMiterLimit))
				pen.MiterLimit = reader.ReadSingle();
			if (flags.HasFlag(EmfPlusPenDataFlags.PenDataLineStyle))
				pen.DashStyle = (DashStyle)reader.ReadInt32();
			if (flags.HasFlag(EmfPlusPenDataFlags.PenDataDashedLineCap))
				pen.DashCap = (DashCap)reader.ReadInt32();
			if (flags.HasFlag(EmfPlusPenDataFlags.PenDataDashedLineOffset))
				pen.DashOffset = reader.ReadSingle();
			if (flags.HasFlag(EmfPlusPenDataFlags.PenDataDashedLine))
				pen.DashPattern = ReadArray(reader.ReadInt32(), reader);
			if (flags.HasFlag(EmfPlusPenDataFlags.PenDataNonCenter)) {
				int alignment = reader.ReadInt32();
				switch (alignment) {
					case 2:
						pen.Alignment = PenAlignment.Left;
						break;
					case 3:
						pen.Alignment = PenAlignment.Outset;
						break;
					default:
						pen.Alignment = (PenAlignment)alignment;
						break;
				}
			}
			if (flags.HasFlag(EmfPlusPenDataFlags.PenDataCompoundLine))
				reader.ReadBytes(reader.ReadInt32() * 4);
			if (flags.HasFlag(EmfPlusPenDataFlags.PenDataCustomStartCap))
				reader.ReadBytes(reader.ReadInt32());
			if (flags.HasFlag(EmfPlusPenDataFlags.PenDataCustomEndCap))
				reader.ReadBytes(reader.ReadInt32());
			pen.Brush = EmfPlusBrush.Create(reader).BrushContainer;
		}
		public PdfPen GetPen() {
			return pen;
		}
		float[] ReadArray(int size, BinaryReader reader) {
			float[] array = new float[size];
			for (int i = 0; i < size; i++)
				array[i] = reader.ReadSingle();
			return array;
		}
	}
}
