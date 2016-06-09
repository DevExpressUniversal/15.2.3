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
using System.IO;
namespace DevExpress.Pdf.Native {
	public class JBIG2SegmentHeader {
		readonly List<int> referredToSegments = new List<int>();
		readonly int number;
		readonly byte flags;
		readonly int pageAssociation;
		readonly int dataLength;
		readonly JBIG2SegmentData data;
		public int Number { get { return number; } }
		public byte Flags { get { return flags; } }
		public int PageAssociation { get { return pageAssociation; } }
		public int DataLength { get { return dataLength; } }
		public JBIG2SegmentData Data { get { return data; } }
		public List<int> ReferredToSegments { get { return referredToSegments; } }
		public bool EndOfFile { get { return (Flags & 63) == 51; } }
		public JBIG2SegmentHeader(Stream stream, JBIG2Image image) {
			JBIG2StreamHelper streamHelper = new JBIG2StreamHelper(stream);
			number = streamHelper.ReadInt32();
			flags = streamHelper.ReadByte();
			byte rtscarf = streamHelper.ReadByte();
			int referredToSegmentsCount;
			if ((rtscarf & 224) == 224) {
				stream.Position--;
				referredToSegmentsCount = streamHelper.ReadInt32() & 0x1fffffff;
				stream.Position += (referredToSegmentsCount + 1) / 8;
			}
			else {
				referredToSegmentsCount = rtscarf >> 5;
			}
			int referredToSegmentSize = Number <= 256 ? 1 : Number <= 65536 ? 2 : 4;
			if (referredToSegmentsCount > 1000000)
				PdfDocumentReader.ThrowIncorrectDataException();
			for (int i = 0; i < referredToSegmentsCount; i++) {
				ReferredToSegments.Add(streamHelper.ReadInt(referredToSegmentSize));
			}
			int pageAssociationSize = (Flags & 64) == 1 ? 4 : 1;
			pageAssociation = streamHelper.ReadInt(pageAssociationSize);
			dataLength = streamHelper.ReadInt32();
			data = JBIG2SegmentData.Create(streamHelper, this, image);
		}
		public void Process() {
			data.Process();
		}
	}
}
