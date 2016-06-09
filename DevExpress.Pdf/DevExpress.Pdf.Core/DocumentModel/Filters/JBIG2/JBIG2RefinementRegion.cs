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
namespace DevExpress.Pdf.Native {
	public class JBIG2RefinementRegion : JBIG2SegmentData {
		readonly JBIG2Image referenceGlyph;
		readonly int template;
		readonly int dx;
		readonly int dy;
		readonly int[] at;
		readonly JBIG2Decoder decoder;
		public JBIG2RefinementRegion(JBIG2StreamHelper streamHelper, JBIG2SegmentHeader header, JBIG2Image image) : base(streamHelper, header, image) { }
		internal JBIG2RefinementRegion(JBIG2Image referenceGlyph, int dx, int dy, int template, int[] at, JBIG2Decoder decoder, JBIG2Image image)
			: base(image) {
			this.referenceGlyph = referenceGlyph;
			this.dx = dx;
			this.dy = dy;
			this.template = template;
			this.at = at;
			this.decoder = decoder;
		}
		public override void Process() {
			base.Process();
			int w = Image.Width;
			int h = Image.Height;
			int context = 0;
			Func<int, int, int> contextCalculator = null;
			switch (template) {
				case 0: contextCalculator = Template0Context;
					break;
				case 1: contextCalculator = Template1Context;
					break;
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					break;
			}
			for (int y = 0; y < h; y++) {
				for (int x = 0; x < w; x++) {
					context = contextCalculator(x, y);
					Image.SetPixel(x, y, decoder.DecodeGR(context));
				}
			}
		}
		int Template1Context(int x, int y) {
			int context = 0;
			context |= Image.GetPixelInt(x - 1, y + 0);
			context |= Image.GetPixelInt(x + 1, y - 1) << 1;
			context |= Image.GetPixelInt(x + 0, y - 1) << 2;
			context |= Image.GetPixelInt(x - 1, y - 1) << 3;
			context |= referenceGlyph.GetPixelInt(x - dx + 1, y - dy + 1) << 4;
			context |= referenceGlyph.GetPixelInt(x - dx + 0, y - dy + 1) << 5;
			context |= referenceGlyph.GetPixelInt(x - dx + 1, y - dy + 0) << 6;
			context |= referenceGlyph.GetPixelInt(x - dx + 0, y - dy + 0) << 7;
			context |= referenceGlyph.GetPixelInt(x - dx - 1, y - dy + 0) << 8;
			context |= referenceGlyph.GetPixelInt(x - dx + 0, y - dy - 1) << 9;
			return context;
		}
		int Template0Context(int x, int y) {
			int context = 0;
			context |= Image.GetPixelInt(x - 1, y + 0);
			context |= Image.GetPixelInt(x + 1, y - 1) << 1;
			context |= Image.GetPixelInt(x + 0, y - 1) << 2;
			context |= Image.GetPixelInt(x + at[0], y + at[1]) << 3;
			context |= referenceGlyph.GetPixelInt(x - dx + 1, y - dy + 1) << 4;
			context |= referenceGlyph.GetPixelInt(x - dx + 0, y - dy + 1) << 5;
			context |= referenceGlyph.GetPixelInt(x - dx - 1, y - dy + 1) << 6;
			context |= referenceGlyph.GetPixelInt(x - dx + 1, y - dy + 0) << 7;
			context |= referenceGlyph.GetPixelInt(x - dx + 0, y - dy + 0) << 8;
			context |= referenceGlyph.GetPixelInt(x - dx - 1, y - dy + 0) << 9;
			context |= referenceGlyph.GetPixelInt(x - dx + 1, y - dy - 1) << 10;
			context |= referenceGlyph.GetPixelInt(x - dx + 0, y - dy - 1) << 11;
			context |= referenceGlyph.GetPixelInt(x - dx + at[2], y - dy + at[3]) << 12;
			return context;
		}
	}
}
