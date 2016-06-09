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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using DevExpress.XtraPrinting.Export.Pdf;
using System.IO;
using System.Drawing.Drawing2D;
namespace DevExpress.Printing.Core.PdfExport.Metafile {
	public class WmfPenObject {
		public Pen Pen { get; set; }
		public void Read(MetaReader reader) {
			PenStyle Style = (PenStyle)reader.ReadUInt16();
			int Width = reader.ReadPointXY().X;
			Color Color = reader.ReadColorRGB();
			Pen = new Pen(Color, Width);
			switch(Style) {
				case PenStyle.PS_SOLID:
					Pen.DashStyle = DashStyle.Solid;
					break;
				case PenStyle.PS_DASH:
					Pen.DashStyle = DashStyle.Dash;
					break;
				case PenStyle.PS_DOT:
					Pen.DashStyle = DashStyle.Dot;
					break;
				case PenStyle.PS_DASHDOT:
					Pen.DashStyle = DashStyle.DashDot;
					break;
				case PenStyle.PS_DASHDOTDOT:
					Pen.DashStyle = DashStyle.DashDotDot;
					break;
				case PenStyle.PS_NULL:
					Pen.Color = Color.Transparent;
					break;
				case PenStyle.PS_INSIDEFRAME:
					break;
				case PenStyle.PS_USERSTYLE:
					break;
				case PenStyle.PS_ALTERNATE:
					break;
				case PenStyle.PS_ENDCAP_SQUARE:
					break;
				case PenStyle.PS_ENDCAP_FLAT:
					break;
				case PenStyle.PS_JOIN_BEVEL:
					break;
				case PenStyle.PS_JOIN_MITER:
					break;
				default:
					break;
			}
		}
	}
	public enum PenStyle {
		PS_COSMETIC = 0x0000,
		PS_ENDCAP_ROUND = 0x0000,
		PS_JOIN_ROUND = 0x0000,
		PS_SOLID = 0x0000,
		PS_DASH = 0x0001,
		PS_DOT = 0x0002,
		PS_DASHDOT = 0x0003,
		PS_DASHDOTDOT = 0x0004,
		PS_NULL = 0x0005,
		PS_INSIDEFRAME = 0x0006,
		PS_USERSTYLE = 0x0007,
		PS_ALTERNATE = 0x0008,
		PS_ENDCAP_SQUARE = 0x0100,
		PS_ENDCAP_FLAT = 0x0200,
		PS_JOIN_BEVEL = 0x1000,
		PS_JOIN_MITER = 0x2000
	}
}
