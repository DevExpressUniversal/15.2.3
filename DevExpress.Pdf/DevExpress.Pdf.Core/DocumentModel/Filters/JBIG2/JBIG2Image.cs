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
using System.IO;
namespace DevExpress.Pdf.Native {
	public class JBIG2Image {
		public static byte[] Decode(byte[] data, Dictionary<int, JBIG2SegmentHeader> globalSegments) {
			JBIG2Image image = new JBIG2Image();
			if (globalSegments != null && globalSegments.Count != 0) {
				foreach (KeyValuePair<int, JBIG2SegmentHeader> segment in globalSegments)
					image.GlobalSegments.Add(segment.Key, segment.Value);
			}
			using (MemoryStream stream = new MemoryStream(data)) {
				JBIG2SegmentHeader segment;
				do {
					segment = new JBIG2SegmentHeader(stream, image);
					image.Segments.Add(segment.Number, segment);
				} while (!segment.EndOfFile && stream.Position <= stream.Length - 1);
				foreach (JBIG2SegmentHeader s in image.Segments.Values)
					s.Process();
			}
			for (int i = 0; i < image.Data.Length; i++)
				image.Data[i] = (byte)(255 - image.Data[i]);
			return image.Data;
		}
		static Func<byte, byte, byte> CreateComposeOperatorByte(int value) {
			switch (value) {
				case 0: return (a, b) => (byte)(a | b);
				case 1: return (a, b) => (byte)(a & b);
				case 2: return (a, b) => (byte)(a ^ b);
				case 3: return (a, b) => (byte)~(a ^ b);
				case 4: return (a, b) => a;
			}
			PdfDocumentReader.ThrowIncorrectDataException();
			return null;
		}
		static Func<bool, bool, bool> CreateComposeOperator(int value) {
			switch (value) {
				case 0: return (a, b) => a | b;
				case 1: return (a, b) => a & b;
				case 2: return (a, b) => a ^ b;
				case 3: return (a, b) => a == b;
				case 4: return (a, b) => a;
			}
			PdfDocumentReader.ThrowIncorrectDataException();
			return null;
		}
		int width;
		int height;
		int stride;
		byte[] data;
		readonly Dictionary<int, JBIG2SegmentHeader> segments;
		readonly Dictionary<int, JBIG2SegmentHeader> globalSegments;
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public int Stride { get { return stride; } }
		public Dictionary<int, JBIG2SegmentHeader> Segments { get { return segments; } }
		public Dictionary<int, JBIG2SegmentHeader> GlobalSegments { get { return globalSegments; } }
		public byte[] Data { get { return data; } }
		public JBIG2Image(int w, int h)
			: this() {
			width = w;
			height = h;
			stride = ((width - 1) >> 3) + 1;
			data = new byte[stride * height];
		}
		public JBIG2Image() {
			segments = new Dictionary<int, JBIG2SegmentHeader>();
			globalSegments = new Dictionary<int, JBIG2SegmentHeader>();
		}
		public void SetDimensions(int width, int height) {
			this.width = width;
			this.height = height;
			this.stride = ((width - 1) >> 3) + 1;
			this.data = new byte[stride * height];
		}
		public void SetPixel(int x, int y, bool value) {
			if ((x < 0) || (x >= width)) return;
			if ((y < 0) || (y >= height)) return;
			int b = (x >> 3) + y * stride;
			int bit = 7 - (x & 7);
			int mask = (1 << bit) ^ 0xff;
			int scratch = data[b] & mask;
			data[b] = (byte)(scratch | ((value ? 1 : 0) << bit));
		}
		public int GetPixelInt(int x, int y) {
			return GetPixel(x, y) ? 1 : 0;
		}
		public bool GetPixel(int x, int y) {
			int b = (x >> 3) + y * stride;
			int bit = 7 - (x & 7);
			if ((x < 0) || (x >= width) ||
				(y < 0) || (y >= height))
				return false;
			return ((data[b] >> bit) & 1) != 0;
		}
		internal void Composite(JBIG2Image image, int x, int y, int composeOperator) {
			if (x == 0 && y == 0 && image.width == width && image.height == height)
				CompositeFast(image, composeOperator);
			else if (composeOperator != 0) {
				CompositeGeneral(image, x, y, composeOperator);
			}
			else
				CompositeOrFast(image, x, y);
		}
		void CompositeFast(JBIG2Image image, int composeOperator) {
			Func<byte, byte, byte> co = CreateComposeOperatorByte(composeOperator);
			for (int i = 0; i < data.Length; i++)
				data[i] = co(data[i], image.data[i]);
		}
		void CompositeOrFast(JBIG2Image image, int x, int y) {
			int mask;
			int rightmask;
			int w = image.width;
			int h = image.height;
			int ss = 0;
			int s = ss;
			if (x < 0) { w += x; x = 0; }
			if (y < 0) { h += y; y = 0; }
			w = (x + w < width) ? w : width - x;
			h = (y + h < height) ? h : height - y;
			if ((w <= 0) || (h <= 0))
				return;
			int leftbyte = x >> 3;
			int rightbyte = (x + w - 1) >> 3;
			int shift = x & 7;
			int dd = y * stride + leftbyte;
			int d = dd;
			if (d < 0 || leftbyte > stride || h * stride < 0 ||
				d - leftbyte + h * stride > height * stride)
				PdfDocumentReader.ThrowIncorrectDataException();
			if (leftbyte == rightbyte) {
				mask = 0x100 - (0x100 >> w);
				for (int j = 0; j < h; j++) {
					data[d] |= (byte)((image.data[s] & mask) >> shift);
					d += stride;
					s += image.stride;
				}
			}
			else if (shift == 0) {
				rightmask = (w & 7) != 0 ? 0x100 - (1 << (8 - (w & 7))) : 0xFF;
				for (int j = 0; j < h; j++) {
					for (int i = leftbyte; i < rightbyte; i++)
						data[d++] |= image.data[s++];
					data[d] |= (byte)(image.data[s] & rightmask);
					dd += stride;
					d = dd;
					ss += image.stride;
					s = ss;
				}
			}
			else {
				bool overlap = (((w + 7) >> 3) < ((x + w + 7) >> 3) - (x >> 3));
				mask = 0x100 - (1 << shift);
				if (overlap)
					rightmask = (0x100 - (0x100 >> ((x + w) & 7))) >> (8 - shift);
				else
					rightmask = 0x100 - (0x100 >> (w & 7));
				for (int j = 0; j < h; j++) {
					data[d++] |= (byte)((image.data[s] & mask) >> shift);
					for (int i = leftbyte; i < rightbyte - 1; i++) {
						data[d] |= (byte)((image.data[s++] & ~mask) << (8 - shift));
						data[d++] |= (byte)((image.data[s] & mask) >> shift);
					}
					if (overlap)
						data[d] |= (byte)((image.data[s] & rightmask) << (8 - shift));
					else
						data[d] |= (byte)(((image.data[s] & ~mask) << (8 - shift)) |
							((image.data[s + 1] & rightmask) >> shift));
					dd += stride;
					d = dd;
					ss += image.stride;
					s = ss;
				}
			}
		}
		void CompositeGeneral(JBIG2Image image, int x, int y, int composeOperator) {
			int sw = image.width;
			int sh = image.height;
			int sx = 0;
			int sy = 0;
			if (x < 0) { sx += -x; sw -= -x; x = 0; }
			if (y < 0) { sy += -y; sh -= -y; y = 0; }
			if (x + sw >= width) sw = width - x;
			if (y + sh >= height) sh = height - y;
			Func<bool, bool, bool> co = CreateComposeOperator(composeOperator);
			for (int j = 0; j < sh; j++)
				for (int i = 0; i < sw; i++)
					SetPixel(i + x, j + y, co(image.GetPixel(i + sx, j + sy), GetPixel(i + x, j + y)));
		}
		internal void Clear(bool color) {
			for (int i = 0; i < data.Length; i++)
				data[i] = (byte)(color ? 0xFF : 0x00);
		}
	}
}
