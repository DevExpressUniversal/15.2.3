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

using DevExpress.Pdf.Native;
using System.Collections.Generic;
using System.IO;
namespace DevExpress.Pdf {
	public class PdfJBIG2GlobalSegments : PdfObject {
		internal static PdfJBIG2GlobalSegments Parse(PdfObjectCollection objects, object value) {
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference != null)
				return objects.ResolveObject<PdfJBIG2GlobalSegments>(reference.Number, () => Parse(objects, objects.GetObjectData(reference.Number)));
			PdfReaderStream stream = value as PdfReaderStream;
			if (stream == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return new PdfJBIG2GlobalSegments(stream.GetData(true));
		}
		readonly byte[] data;
		Dictionary<int, JBIG2SegmentHeader> segments;
		public byte[] Data { get { return data; } }
		internal Dictionary<int, JBIG2SegmentHeader> Segments {
			get {
				if (segments == null) {
					try {
						JBIG2Image image = new JBIG2Image();
						if (data.Length != 0)
							using (MemoryStream stream = new MemoryStream(data)) {
								JBIG2SegmentHeader segment;
								do {
									segment = new JBIG2SegmentHeader(stream, image);
									image.GlobalSegments.Add(segment.Number, segment);
								} while (!segment.EndOfFile && stream.Position <= stream.Length - 1);
								foreach (JBIG2SegmentHeader s in image.GlobalSegments.Values)
									s.Process();
							}
						segments = image.GlobalSegments;
					}
					catch {
						segments = new Dictionary<int, JBIG2SegmentHeader>();
					}
				}
				return segments;
			}
		}
		PdfJBIG2GlobalSegments(byte[] data) {
			this.data = data;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return new PdfCompressedStream(data);
		}
	}
}
