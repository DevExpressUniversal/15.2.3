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
	public class EmfPlusBrush {
		public Brush Brush { get; set; }
		public EmfPlusBrush(MetaReader reader) {
			new EmfPlusGraphicsVersion(reader);
			BrushType type = (BrushType)reader.ReadUInt32();
			switch(type) {
				case BrushType.BrushTypeSolidColor:
					Brush = new SolidBrush(reader.ReadEmfPlusARGB());
					break;
				case BrushType.BrushTypeHatchFill:
					Brush = new HatchBrush((System.Drawing.Drawing2D.HatchStyle)reader.ReadUInt32(), reader.ReadEmfPlusARGB(), reader.ReadEmfPlusARGB());
					break;
				case BrushType.BrushTypePathGradient:
					Brush = new EmfPlusPathGradientBrushData(reader).Brush;
					break;
				case BrushType.BrushTypeLinearGradient:
					Brush = new EmfPlusLinearGradientBrushData(reader).Brush;
					break;
				default:
					throw new NotSupportedException();
			}
		}
	}
	public class EmfPlusLinearGradientBrushData {
		public LinearGradientBrush Brush { get; set; }
		public EmfPlusLinearGradientBrushData(MetaReader reader) {
			BrushDataFlags flags = (BrushDataFlags)reader.ReadUInt32();
			WrapMode wrapMode = (WrapMode)reader.ReadInt32();
			RectangleF rectF = reader.ReadRectF();
			Color startColor = reader.ReadEmfPlusARGB();
			Color endColor = reader.ReadEmfPlusARGB();
			reader.ReadInt32();
			reader.ReadInt32();
			Brush = new LinearGradientBrush(rectF, startColor, endColor, 0f);
			Brush.WrapMode = wrapMode;
			new EmfPlusLinearGradientBrushOptionalData(reader, Brush, flags);
		}
	}
	public class EmfPlusLinearGradientBrushOptionalData {
		public EmfPlusLinearGradientBrushOptionalData(MetaReader reader, LinearGradientBrush brush, BrushDataFlags flags) {
			if(flags.HasFlag(BrushDataFlags.BrushDataTransform)) {
				brush.Transform = reader.ReadMatrix();
			}
		}
	}
	public class EmfPlusPathGradientBrushData {
		public Brush Brush { get; set; }
		public EmfPlusPathGradientBrushData(MetaReader reader) {
			BrushDataFlags flags = (BrushDataFlags)reader.ReadUInt32();
			WrapMode wrapMode = (WrapMode)reader.ReadInt32();
			Color centerColor = reader.ReadEmfPlusARGB();
			Brush = new SolidBrush(centerColor);
		}
	}
	[Flags]
	public enum BrushDataFlags {
		BrushDataPath = 0x00000001,
		BrushDataTransform = 0x00000002,
		BrushDataPresetColors = 0x00000004,
		BrushDataBlendFactors = 0x00000008,
		BrushDataFocusScales = 0x00000040,
		BrushDataIsGammaCorrected = 0x00000080,
		BrushDataDoNotTransform = 0x00000100
	}
	public enum BrushType {
		BrushTypeSolidColor = 0x00000000,
		BrushTypeHatchFill = 0x00000001,
		BrushTypeTextureFill = 0x00000002,
		BrushTypePathGradient = 0x00000003,
		BrushTypeLinearGradient = 0x00000004
	}
}
