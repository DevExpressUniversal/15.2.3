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
	public abstract class JBIG2SegmentData {
		public static JBIG2SegmentData Create(JBIG2StreamHelper streamHelper, JBIG2SegmentHeader header, JBIG2Image image) {
			switch (header.Flags & 63) {
				case 0:
					return new JBIG2SymbolDictionary(streamHelper, header, image);
				case 4: 
				case 6: 
				case 7: 
					return new JBIG2TextRegion(streamHelper, header, image);
				case 38: 
				case 39: 
					return new JBIG2GenericRegion(streamHelper, header, image);
				case 40: 
				case 42: 
				case 43: 
					return new JBIG2RefinementRegion(streamHelper, header, image);
				case 48:
					return new JBIG2PageInfo(streamHelper, header, image);
				case 20: 
				case 22: 
				case 23: 
				case 49:
				case 50:
				case 53: 
				case 62:
				case 36:
				case 16:
				case 51:
				case 52:
				default:
					return new JBIG2UnknownSegment(streamHelper, header, image);
			}
		}
		readonly JBIG2SegmentHeader header;
		readonly JBIG2Image image;
		JBIG2StreamHelper streamHelper;
		public JBIG2SegmentHeader Header { get { return header; } }
		public JBIG2Image Image { get { return image; } }
		protected JBIG2StreamHelper StreamHelper { get { return streamHelper; } }
		protected virtual bool CacheData { get { return true; } }
		protected JBIG2SegmentData(JBIG2Image image) {
			this.image = image;
		}
		protected JBIG2SegmentData(JBIG2StreamHelper streamHelper, JBIG2SegmentHeader header, JBIG2Image image)
			: this(image) {
			this.header = header;
			this.streamHelper = streamHelper;
			if (CacheData)
				DoCacheData();
		}
		protected virtual void DoCacheData() {
			this.streamHelper = streamHelper.ReadData(header.DataLength);
		}
		public virtual void Process() {
		}
		protected List<JBIG2SymbolDictionary> GetSDReferred() {
			List<JBIG2SymbolDictionary> result = new List<JBIG2SymbolDictionary>();		   
			for (int index = 0; index < Header.ReferredToSegments.Count; index++) {
				JBIG2SegmentHeader rsegment;
				if (!Image.Segments.TryGetValue(Header.ReferredToSegments[index], out rsegment)) {
					Image.GlobalSegments.TryGetValue(Header.ReferredToSegments[index], out rsegment);
				}
				if (rsegment != null) {
					JBIG2SymbolDictionary rdata = rsegment.Data as JBIG2SymbolDictionary;
					if (rdata != null)
						result.Add(rdata);
				}
			}
			return result;
		}
	}
}
