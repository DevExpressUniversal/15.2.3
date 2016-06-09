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
using System.IO;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Doc {
	public enum DocBorderLineStyle {
		None = 0x00,
		Single = 0x01,
		Double = 0x03,
		ThinSingle = 0x05,
		Dotted = 0x06,
		Dashed = 0x07,
		DotDash = 0x08,
		DotDotDash = 0x09,
		Triple = 0x0a,
		ThinThickSmallGap = 0x0b,
		ThickThinSmallGap = 0x0c,
		ThinThickThinSmallGap = 0x0d,
		ThinThickMediumGap = 0x0e,
		ThickThinMediumGap = 0x0f,
		ThinThickThinMediumGap = 0x10,
		ThinThickLargeGap = 0x11,
		ThickThinLargeGap = 0x12,
		ThinThickThinLargeGap = 0x13,
		Wave = 0x14,
		DoubleWave = 0x15
	}
	#region BorderDescriptor97
	public class BorderDescriptor97 {
		#region static
		public static BorderDescriptor97 FromStream(BinaryReader reader) {
			BorderDescriptor97 result = new BorderDescriptor97();
			result.Read(reader);
			return result;
		}
		public static BorderDescriptor97 FromByteArray(byte[] data, int startIndex) {
			BorderDescriptor97 result = new BorderDescriptor97();
			result.Read(data, startIndex);
			return result;
		}
		#endregion
		#region Fields
		const byte size = 4;
		const uint emptyBorder = 0xffffffff;
		byte width; 
		DocBorderLineStyle style;
		Color borderColor;
		int distance;
		#endregion
		#region Properties
		public byte Width { get { return this.width; } }
		public DocBorderLineStyle Style { get { return this.style; } }
		public Color BorderColor { get { return this.borderColor; } }
		public int Distance { get { return this.distance; } }
		#endregion
		protected void Read(BinaryReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			byte[] data = reader.ReadBytes(size);
			Read(data, 0);
		}
		protected void Read(byte[] data, int startIndex) {
			if (BitConverter.ToUInt32(data, startIndex) == emptyBorder)
				return;
			this.width = data[startIndex];
			this.style = (DocBorderLineStyle)data[startIndex + 1];
			byte colorIndex = data[startIndex + 2];
			if (colorIndex < DocConstants.DefaultMSWordColor.Length)
				this.borderColor = DocConstants.DefaultMSWordColor[colorIndex];
			this.distance = data[startIndex + 3] & 0xe0;
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			if (this.style == DocBorderLineStyle.None && this.width == 0) {
				writer.Write(emptyBorder);
				return;
			}
			writer.Write((byte)this.width);
			writer.Write((byte)this.style);
			writer.Write((byte)Array.IndexOf(DocConstants.DefaultMSWordColor, this.borderColor));
			writer.Write((byte)this.distance);
		}
		public BorderDescriptor97 Clone() {
			BorderDescriptor97 result = new BorderDescriptor97();
			result.borderColor = this.borderColor;
			result.style = this.Style;
			result.distance = this.distance;
			result.width = this.width;
			return result;
		}
	}
	#endregion
	#region BorderDescriptor
	public class BorderDescriptor {
		#region static
		public static BorderDescriptor FromStream(BinaryReader reader) {
			BorderDescriptor result = new BorderDescriptor();
			result.Read(reader);
			return result;
		}
		public static BorderDescriptor FromByteArray(byte[] data, int startIndex) {
			BorderDescriptor result = new BorderDescriptor();
			result.Read(data, startIndex);
			return result;
		}
		#endregion
		#region Fields
		const uint emptyBorder = 0xffffffff;
		public const int BorderDescriptorSize = 8;
		const int returnOffset = -8;
		DocColorReference colorReference;
		bool frame;
		byte offset; 
		bool shadow;
		DocBorderLineStyle style;
		byte width; 
		#endregion
		public BorderDescriptor() {
			this.colorReference = new DocColorReference();
		}
		#region Properties
		public Color Color { get { return colorReference.Color; } set { colorReference.Color = value; } }
		public bool Frame { get { return frame; } set { frame = value; } }
		public int Offset { get { return this.offset; } set { this.offset = (byte)value; } }
		public bool Shadow { get { return shadow; } set { shadow = value; } }
		public DocBorderLineStyle Style { get { return style; } set { style = value; } }
		public int Width { get { return width; } set { width = (byte)value; } }
		#endregion
		protected void Read(BinaryReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			if (BitConverter.ToUInt32(reader.ReadBytes(BorderDescriptorSize), 0) == emptyBorder)
				return;
			reader.BaseStream.Seek(returnOffset, SeekOrigin.Current);
			this.colorReference = DocColorReference.FromByteArray(reader.ReadBytes(DocColorReference.ColorReferenceSize), 0);
			this.width = reader.ReadByte();
			this.style = (DocBorderLineStyle)reader.ReadByte();
			byte bitwiseField = reader.ReadByte();
			this.offset = (byte)(bitwiseField & 0x1f);
			this.shadow = (bitwiseField & 0x20) != 0;
			this.frame = (bitwiseField & 0x40) != 0;
			reader.ReadByte();
		}
		protected void Read(byte[] data, int startIndex) {
			this.colorReference = DocColorReference.FromByteArray(data, startIndex);
			if (BitConverter.ToUInt32(data, startIndex) == emptyBorder ||
				BitConverter.ToUInt32(data, startIndex + 4) == emptyBorder)
				return;
			this.width = data[startIndex + 4];
			this.style = (DocBorderLineStyle)data[startIndex + 5];
			byte bitwiseField = data[startIndex + 6];
			this.offset = (byte)(bitwiseField & 0x1f);
			this.shadow = (bitwiseField & 0x20) != 0;
			this.frame = (bitwiseField & 0x40) != 0;
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(this.colorReference.GetBytes());
			if (this.style == DocBorderLineStyle.None && this.width == 0) {
				writer.Write(emptyBorder);
				return;
			}
			writer.Write(this.width);
			writer.Write((byte)this.style);
			byte bitwiseField = this.offset;
			if (this.shadow)
				bitwiseField |= 0x20;
			if (this.frame)
				bitwiseField |= 0x40;
			writer.Write(bitwiseField);
			writer.BaseStream.WriteByte(0);
		}
		public void ConvertFromBorderInfo(BorderInfo info, DocumentModelUnitConverter unitConverter) {
			Color = info.Color;
			this.frame = info.Frame;
			this.offset = (byte)info.Offset;
			this.shadow = info.Shadow;
			this.style = DocBorderCalculator.MapToDocBorderLineStyle(info.Style);
			this.width = (byte)(unitConverter.ModelUnitsToTwips(info.Width) / 2.5);
		}
		public void ApplyProperties(BorderBase destination, DocumentModelUnitConverter unitConverter) {
			destination.Color = Color;
			destination.Frame = this.frame;
			destination.Offset = this.offset;
			destination.Shadow = this.shadow;
			destination.Style = DocBorderCalculator.MapToBorderLineStyle(this.style);
			destination.Width = unitConverter.TwipsToModelUnits((int)(this.width * 2.5));
		}
		public BorderDescriptor Clone() {
			BorderDescriptor result = new BorderDescriptor();
			result.Color = this.Color;
			result.frame = this.frame;
			result.offset = this.offset;
			result.shadow = this.shadow;
			result.style = this.style;
			result.width = this.width;
			return result;
		}
	}
	#endregion
	#region DocBorderCalculator
	public static class DocBorderCalculator {
		public static DocBorderLineStyle MapToDocBorderLineStyle(BorderLineStyle lineStyleType) {
			switch (lineStyleType) {
				case BorderLineStyle.None: return DocBorderLineStyle.None;
				case BorderLineStyle.Single: return DocBorderLineStyle.Single;
				case BorderLineStyle.Double: return DocBorderLineStyle.Double;
				case BorderLineStyle.Dotted: return DocBorderLineStyle.Dotted;
				case BorderLineStyle.Dashed: return DocBorderLineStyle.Dashed;
				case BorderLineStyle.DotDash: return DocBorderLineStyle.DotDash;
				case BorderLineStyle.DotDotDash: return DocBorderLineStyle.DotDotDash;
				case BorderLineStyle.Triple: return DocBorderLineStyle.Triple;
				case BorderLineStyle.ThinThickSmallGap: return DocBorderLineStyle.ThinThickSmallGap;
				case BorderLineStyle.ThickThinSmallGap: return DocBorderLineStyle.ThickThinSmallGap;
				case BorderLineStyle.ThinThickThinSmallGap: return DocBorderLineStyle.ThinThickThinSmallGap;
				case BorderLineStyle.ThinThickMediumGap: return DocBorderLineStyle.ThinThickMediumGap;
				case BorderLineStyle.ThickThinMediumGap: return DocBorderLineStyle.ThickThinMediumGap;
				case BorderLineStyle.ThinThickThinMediumGap: return DocBorderLineStyle.ThinThickThinMediumGap;
				case BorderLineStyle.ThinThickLargeGap: return DocBorderLineStyle.ThinThickLargeGap;
				case BorderLineStyle.ThickThinLargeGap: return DocBorderLineStyle.ThickThinLargeGap;
				case BorderLineStyle.ThinThickThinLargeGap: return DocBorderLineStyle.ThinThickThinLargeGap;
				case BorderLineStyle.Wave: return DocBorderLineStyle.Wave;
				case BorderLineStyle.DoubleWave: return DocBorderLineStyle.DoubleWave;
				default: return DocBorderLineStyle.None;
			}
		}
		public static BorderLineStyle MapToBorderLineStyle(DocBorderLineStyle lineStyleType) {
			switch (lineStyleType) {
				case DocBorderLineStyle.None: return BorderLineStyle.None;
				case DocBorderLineStyle.Single: return BorderLineStyle.Single;
				case DocBorderLineStyle.Double: return BorderLineStyle.Double;
				case DocBorderLineStyle.Dotted: return BorderLineStyle.Dotted;
				case DocBorderLineStyle.Dashed: return BorderLineStyle.Dashed;
				case DocBorderLineStyle.DotDash: return BorderLineStyle.DotDash;
				case DocBorderLineStyle.DotDotDash: return BorderLineStyle.DotDotDash;
				case DocBorderLineStyle.Triple: return BorderLineStyle.Triple;
				case DocBorderLineStyle.ThinThickSmallGap: return BorderLineStyle.ThinThickSmallGap;
				case DocBorderLineStyle.ThickThinSmallGap: return BorderLineStyle.ThickThinSmallGap;
				case DocBorderLineStyle.ThinThickThinSmallGap: return BorderLineStyle.ThinThickThinSmallGap;
				case DocBorderLineStyle.ThinThickMediumGap: return BorderLineStyle.ThinThickMediumGap;
				case DocBorderLineStyle.ThickThinMediumGap: return BorderLineStyle.ThickThinMediumGap;
				case DocBorderLineStyle.ThinThickThinMediumGap: return BorderLineStyle.ThinThickThinMediumGap;
				case DocBorderLineStyle.ThinThickLargeGap: return BorderLineStyle.ThinThickLargeGap;
				case DocBorderLineStyle.ThickThinLargeGap: return BorderLineStyle.ThickThinLargeGap;
				case DocBorderLineStyle.ThinThickThinLargeGap: return BorderLineStyle.ThinThickThinLargeGap;
				case DocBorderLineStyle.Wave: return BorderLineStyle.Wave;
				case DocBorderLineStyle.DoubleWave: return BorderLineStyle.DoubleWave;
				default: return BorderLineStyle.Single;
			}
		}
	}
	#endregion
}
