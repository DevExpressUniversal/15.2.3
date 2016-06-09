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

namespace DevExpress.Pdf.Native {
	public class JBIG2GenericRegion : JBIG2SegmentData {
		readonly JBIG2RegionSegmentInfo info;
		readonly byte flags;
		readonly int[] gbat;
		readonly bool mmr;
		readonly int gbtemplate;
		readonly bool tpgdon;
		readonly JBIG2Decoder decoder;
		protected override bool CacheData { get { return false; } }
		internal JBIG2GenericRegion(JBIG2SegmentHeader header, int[] gbat, int gbtemplate, JBIG2Decoder decoder)
			: base(null, header, null) {
			this.gbat = gbat;
			this.mmr = false;
			this.gbtemplate = gbtemplate;
			this.tpgdon = false;
			this.decoder = decoder;
		}
		public JBIG2GenericRegion(JBIG2StreamHelper streamHelper, JBIG2SegmentHeader header, JBIG2Image image)
			: base(streamHelper, header, image) {
			info = new JBIG2RegionSegmentInfo(StreamHelper);
			flags = StreamHelper.ReadByte();
			if ((flags & 1) == 0) {
				int length = (flags & 6) > 0 ? 2 : 8;
				gbat = StreamHelper.ReadAdaptiveTemplate(length);
			}
			mmr = (flags & 1) > 0;
			gbtemplate = (flags & 6) >> 1;
			tpgdon = (flags & 8) >> 3 > 0;
			DoCacheData();
			decoder = JBIG2Decoder.Create(StreamHelper, 0);
		}
		public override void Process() {
			base.Process();
			JBIG2Image i = new JBIG2Image(info.Width, info.Height);
			CreateDecoder(i).Decode();
			Image.Composite(i, info.X, info.Y, info.ComposeOperator);
		}
		internal JBIG2GenericRegionDecoder CreateDecoder(JBIG2Image i) {
			if (mmr)
				PdfDocumentReader.ThrowIncorrectDataException();
			if (tpgdon)
				return JBIG2TPGDONDecoder.Create(i, decoder, gbat, gbtemplate);
			else
				switch (gbtemplate) {
					case 0:
						if (gbat[0] == +3 && gbat[1] == -1 &&
							gbat[2] == -3 && gbat[3] == -1 &&
							gbat[4] == +2 && gbat[5] == -2 &&
							gbat[6] == -2 && gbat[7] == -2)
							return new JBIG2Template0Decoder(i, decoder);
						break;
					case 1:
						return new JBIG2Template1Decoder(i, decoder);
					case 2:
						if (gbat[0] == 3 && gbat[1] == -1)
							return new JBIG2Template2aDecoder(i, decoder);
						else
							return new JBIG2Template2Decoder(i, decoder);
					case 3:
						return new JBIG2Template3Decoder(i, decoder, gbat[0], gbat[1]);
				}
			PdfDocumentReader.ThrowIncorrectDataException();
			return null;
		}
	}
}
