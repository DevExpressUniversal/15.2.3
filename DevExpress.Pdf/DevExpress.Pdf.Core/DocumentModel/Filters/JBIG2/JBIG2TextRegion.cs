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
	public class JBIG2TextRegion : JBIG2SegmentData {
		readonly bool sbrefine;
		readonly int logsbstrips;
		readonly int sbstrips;
		readonly JBIG2Corner refcorner;
		readonly bool transposed;
		readonly int sbcombop;
		readonly bool sbdefpixel;
		readonly int sbdsoffset;
		readonly int sbrtemplate;
		readonly int[] sbrat;
		readonly int sbnuminstances;
		readonly List<JBIG2Image> glyphs = new List<JBIG2Image>();
		readonly JBIG2RegionSegmentInfo regionInfo;
		readonly JBIG2Decoder decoder;
		protected override bool CacheData { get { return false; } }
		public JBIG2TextRegion(JBIG2StreamHelper streamHelper, JBIG2SegmentHeader header, JBIG2Image image)
			: base(streamHelper, header, image) {
			DoCacheData(); 
			regionInfo = new JBIG2RegionSegmentInfo(StreamHelper);
			int flags = StreamHelper.ReadInt16();
			if ((flags & 0x0001) != 0) 
				PdfDocumentReader.ThrowIncorrectDataException();
			sbrefine = (flags & 0x0002) != 0;
			logsbstrips = (flags & 0x000c) >> 2;
			sbstrips = 1 << logsbstrips;
			refcorner = (JBIG2Corner)((flags & 0x0030) >> 4);
			transposed = (flags & 0x0040) != 0;
			sbcombop = (flags & 0x0180) >> 7;
			sbdefpixel = (flags & 0x0200) != 0;
			sbdsoffset = (flags & 0x7C00) >> 10;
			if (sbdsoffset > 0x0f) sbdsoffset -= 0x20;
			sbrtemplate = (flags & 0x8000) >> 15;
			if ((sbrefine) && (sbrtemplate == 0)) {
				sbrat = StreamHelper.ReadAdaptiveTemplate(4);
			}
			sbnuminstances = StreamHelper.ReadInt32();
			foreach (JBIG2SymbolDictionary sd in GetSDReferred())
				glyphs.AddRange(sd.Glyphs);
			decoder = JBIG2Decoder.Create(StreamHelper, glyphs.Count);
		}
		internal JBIG2TextRegion(int refaggninst, int sdrtemplate, int[] at, List<JBIG2Image> glyphs, JBIG2Decoder decoder, JBIG2Image image) :
			base(image) {
			this.sbrefine = true;
			this.sbnuminstances = refaggninst;
			this.sbstrips = 1;
			this.glyphs = glyphs;
			this.sbdefpixel = false;
			this.sbcombop = 0;
			this.transposed = false;
			this.refcorner = JBIG2Corner.TopLeft;
			this.sbdsoffset = 0;
			this.sbrtemplate = sdrtemplate;
			this.sbrat = at;
			this.regionInfo = new JBIG2RegionSegmentInfo(image.Width, image.Height);
			this.decoder = decoder;
		}
		public override void Process() {
			base.Process();
			if (glyphs.Count == 0)
				return;
			JBIG2Image resultImage = new JBIG2Image(regionInfo.Width, regionInfo.Height);
			int curs = 0;
			resultImage.Clear(sbdefpixel);
			int stript = decoder.DecodeDT() * -sbstrips;
			int firsts = 0;
			int ninstances = 0;
			while (ninstances < sbnuminstances) {
				int dt = decoder.DecodeDT() * sbstrips;
				stript += dt;
				firsts += decoder.DecodeFS();
				curs = firsts;
				while(!decoder.LastCode){
					int curt = sbstrips == 1 ? 0 : decoder.DecodeIT();
					int T = stript + curt;
					int id = decoder.DecodeID();
					JBIG2Image ib = glyphs[id];
					if (sbrefine && decoder.DecodeRI() > 0) {
						ib = RefinementDecode(ib);
					}
					if ((!transposed) && ((int)refcorner > 1)) {
						curs += ib.Width - 1;
					}
					else if ((transposed) && ((int)refcorner & 1) == 0) {
						curs += ib.Height - 1;
					}
					int x = 0, y = 0;
					int ss = transposed ? T : curs;
					int tt = transposed ? curs : T;
					switch (refcorner) {
						case JBIG2Corner.TopLeft: x = ss; y = tt; break;
						case JBIG2Corner.TopRight: x = ss - ib.Width + 1; y = tt; break;
						case JBIG2Corner.BottomLeft: x = ss; y = tt - ib.Height + 1; break;
						case JBIG2Corner.BottomRight: x = ss - ib.Width + 1; y = tt - ib.Height + 1; break;
					}
					resultImage.Composite(ib, x, y, sbcombop);
					if ((!transposed) && ((int)refcorner < 2)) {
						curs += ib.Width - 1;
					}
					else if ((transposed) && ((int)refcorner & 1) != 0) {
						curs += ib.Height - 1;
					}
					ninstances++;
					if (ninstances > sbnuminstances)
						break;
					int ids = decoder.DecodeDS();
					curs += ids + sbdsoffset;
				}
			}
			if (Header == null || (Header.Flags & 63) != 4)
				Image.Composite(resultImage, regionInfo.X, regionInfo.Y, regionInfo.ComposeOperator);
		}
		JBIG2Image RefinementDecode(JBIG2Image ib) {
			int rdw = decoder.DecodeRDW();
			int rdh = decoder.DecodeRDH();
			int rdx = decoder.DecodeRDX();
			int rdy = decoder.DecodeRDY();
			JBIG2Image refImage = new JBIG2Image(ib.Width + rdw, ib.Height + rdh);
			int dx = (rdw >> 1) + rdx;
			int dy = (rdh >> 1) + rdy;
			JBIG2RefinementRegion refinement = new JBIG2RefinementRegion(ib, dx, dy, sbrtemplate, sbrat, decoder, refImage);
			refinement.Process();
			return refImage;
		}
	}
}
