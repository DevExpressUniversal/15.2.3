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
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class JBIG2SymbolDictionary : JBIG2SegmentData {
		readonly int flags;
		readonly bool sdrefagg;
		readonly int sdtemplate;
		readonly int sdrtemplate;
		readonly int sdnumexsyms;
		readonly int sdnumnewsyms;
		readonly List<JBIG2Image> syms = new List<JBIG2Image>();
		readonly int[] sdat;
		readonly int[] sdrat;
		readonly List<JBIG2Image> glyphs = new List<JBIG2Image>();
		public List<JBIG2Image> Glyphs { get { return glyphs; } }
		public JBIG2SymbolDictionary(JBIG2Image image)
			: base(image) {
		}
		public JBIG2SymbolDictionary(JBIG2StreamHelper streamHelper, JBIG2SegmentHeader header, JBIG2Image image)
			: base(streamHelper, header, image) {
			flags = StreamHelper.ReadInt16();
			if ((flags & 1) != 0) 
				PdfDocumentReader.ThrowIncorrectDataException();
			sdrefagg = ((flags >> 1) & 1) != 0;
			sdtemplate = (flags >> 10) & 3;
			sdrtemplate = (flags >> 12) & 1;
			if ((flags & 0x000c) != 0 || (flags & 0x0030) != 0 || (flags & 0x0080) != 0 || (flags & 0x0100) != 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			int sdatLength = sdtemplate == 0 ? 8 : 2;
			sdat = StreamHelper.ReadAdaptiveTemplate(sdatLength);
			if (sdrefagg && sdrtemplate == 0)
				sdrat = StreamHelper.ReadAdaptiveTemplate(4);
			sdnumexsyms = StreamHelper.ReadInt32();
			sdnumnewsyms = StreamHelper.ReadInt32();
			ProcessImage();
		}
		void ProcessImage() {
			foreach (JBIG2SymbolDictionary sd in GetSDReferred())
				syms.AddRange(sd.glyphs);
			int limit = syms.Count + sdnumnewsyms;
			int symwidth;
			int hcheight = 0;
			JBIG2Decoder decoder = JBIG2Decoder.Create(StreamHelper, limit);
			while (syms.Count < limit) {
				int hcdh = decoder.DecodeDH();
				hcheight += hcdh;
				symwidth = 0;
				if (hcheight < 0)
					PdfDocumentReader.ThrowIncorrectDataException();
				int dw = decoder.DecodeDW();
				while (!decoder.LastCode) {
					if (syms.Count >= limit)
						PdfDocumentReader.ThrowIncorrectDataException();
					symwidth += dw;
					if (symwidth < 0)
						PdfDocumentReader.ThrowIncorrectDataException();
					JBIG2Image glyph = new JBIG2Image(symwidth, hcheight);
					if (!sdrefagg) {
						JBIG2GenericRegion region = new JBIG2GenericRegion(Header, sdat, sdtemplate, decoder);
						region.CreateDecoder(glyph).Decode();
					}
					else {
						int refaggninst = decoder.DecodeAI();
						if (decoder.LastCode || refaggninst <= 0)
							PdfDocumentReader.ThrowIncorrectDataException();
						if (refaggninst > 1) {
							JBIG2TextRegion textRegion = new JBIG2TextRegion(refaggninst, sdrtemplate, sdrat, syms, decoder, glyph);
							textRegion.Process();
						}
						else {
							int id = decoder.DecodeID();
							int rdx = decoder.DecodeRDX();
							int rdy = decoder.DecodeRDY();
							if (id >= syms.Count)
								PdfDocumentReader.ThrowIncorrectDataException();
							JBIG2RefinementRegion refinement = new JBIG2RefinementRegion(syms[id], rdx, rdy, sdrtemplate, sdrat, decoder, glyph);
							refinement.Process();
						}
					}
					syms.Add(glyph);
					dw = decoder.DecodeDW();
				}
			}
			int ii = 0;
			bool exflag = false;
			int zerolength = 0;
			while (ii < limit) {
				int exrunlength = decoder.DecodeEX();
				zerolength = exrunlength > 0 ? 0 : zerolength + 1;
				if (decoder.LastCode || (exrunlength > limit - ii) || (exrunlength < 0) || (zerolength > 4) || (exflag && (exrunlength > sdnumexsyms - glyphs.Count)))
					PdfDocumentReader.ThrowIncorrectDataException();
				for (int k = 0; k < exrunlength; k++) {
					if (exflag) {
						this.glyphs.Add(syms[ii]);
					}
					ii++;
				}
				exflag = !exflag;
			}
		}
	}
}
