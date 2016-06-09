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
	public class WmfLogBrush {
		public Brush Brush { get; set; }
		public void Read(MetaReader reader) {
			BrushStyle style = (BrushStyle)reader.ReadUInt16();
			Color color = reader.ReadColorRGB();
			HatchStyle brushHatch = (HatchStyle)reader.ReadInt16();
			switch(style) {
				case BrushStyle.BS_SOLID:
					Brush = new SolidBrush(color);
					break;
				case BrushStyle.BS_NULL:
					Brush = new NullBrush();
					break;
				case BrushStyle.BS_HATCHED:
					Brush = new HatchBrush(brushHatch, color);
					break;
				case BrushStyle.BS_PATTERN:
					break;
				case BrushStyle.BS_INDEXED:
					break;
				case BrushStyle.BS_DIBPATTERN:
					break;
				case BrushStyle.BS_DIBPATTERNPT:
					break;
				case BrushStyle.BS_PATTERN8X8:
					break;
				case BrushStyle.BS_DIBPATTERN8X8:
					break;
				case BrushStyle.BS_MONOPATTERN:
					break;
				default:
					break;
			}
		}
	}
	public class NullBrush : Brush{
		public NullBrush() {
		}
		public override object Clone() {
			return new NullBrush();
		}
	}
	public enum BrushStyle {
		BS_SOLID = 0x0000,
		BS_NULL = 0x0001,
		BS_HATCHED = 0x0002,
		BS_PATTERN = 0x0003,
		BS_INDEXED = 0x0004,
		BS_DIBPATTERN = 0x0005,
		BS_DIBPATTERNPT = 0x0006,
		BS_PATTERN8X8 = 0x0007,
		BS_DIBPATTERN8X8 = 0x0008,
		BS_MONOPATTERN = 0x0009
	}
}
