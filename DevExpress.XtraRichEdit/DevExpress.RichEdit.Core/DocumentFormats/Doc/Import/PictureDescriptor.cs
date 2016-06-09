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
using System.Text;
using System.IO;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public enum PictureDataFormat {
		TiffFile = 0x0062,
		BmpFile = 0x0063,
		ShapeObject = 0x0064,
		ShapeFile = 0x0066
	}
	public class PictureDescriptor {
		#region static
		public static PictureDescriptor FromStream(BinaryReader reader, int offset) {
			PictureDescriptor result = new PictureDescriptor();
			result.Read(reader, offset);
			return result;
		}
		#endregion
		#region Fields
		const int innerHeaderSize = 0x14;
		const int obsoletePropertiesSize = 0x6;
		const int reservedDataSize = 0xa;
		const short picturePropertiesSize = 0x44;
		const int maxImageSize = 0x7bc0;
		PictureDataFormat dataFormat;
		ushort width;
		ushort height;
		ushort horizontalScaleFactor;
		ushort verticalScaleFactor;
		OfficeArtInlineShapeContainer inlineShapeContainer;
		BorderDescriptor97 top;
		BorderDescriptor97 left;
		BorderDescriptor97 bottom;
		BorderDescriptor97 right;
		string pictureName;
		List<DocMetafileHeader> metafileHeaders;
		List<OfficeImage> images;
		#endregion
		#region Constructors
		public PictureDescriptor() {
			this.metafileHeaders = new List<DocMetafileHeader>();
			this.images = new List<OfficeImage>();
		}
		public PictureDescriptor(InlinePictureRun run, int shapeId, int blipIndex) {
			this.horizontalScaleFactor = Convert.ToUInt16(Math.Min(run.ScaleX * 10, Int16.MaxValue));
			this.verticalScaleFactor = Convert.ToUInt16(Math.Min(run.ScaleY * 10, Int16.MaxValue));
			this.inlineShapeContainer = new OfficeArtInlineShapeContainer();
			OfficeArtShapeContainer shapeContainer = this.inlineShapeContainer.ShapeContainer;
			shapeContainer.ShapeRecord.ShapeIdentifier = shapeId;
			shapeContainer.ArtProperties.BlipIndex = blipIndex;
			shapeContainer.ArtProperties.ZOrder = blipIndex;
			shapeContainer.ArtProperties.CreateProperties();
			this.inlineShapeContainer.Blips.Add(new FileBlipStoreEntry(run.Image, false));
			CalculateImageSize(run);
		}
		#endregion
		#region Properties
		PictureDataFormat DataFormat { get { return dataFormat; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public int HorizontalScaleFactor { get { return horizontalScaleFactor; } }
		public int VerticalScaleFactor { get { return verticalScaleFactor; } }
		public BorderDescriptor97 Top { get { return top; } }
		public BorderDescriptor97 Left { get { return left; } }
		public BorderDescriptor97 Bottom { get { return bottom; } }
		public BorderDescriptor97 Right { get { return right; } }
		public string PictureName { get { return pictureName; } }
		public List<DocMetafileHeader> MetafileHeaders { get { return metafileHeaders; } }
		public OfficeArtProperties Properties { get { return inlineShapeContainer != null ? inlineShapeContainer.ShapeContainer.ArtProperties : null; } }
		public List<OfficeImage> Images { get { return images; } }
		#endregion
		void CalculateImageSize(InlinePictureRun run) {
			OfficeImage image = run.Image;
			if (image.SizeInTwips.Width <= maxImageSize)
				this.width = (ushort)image.SizeInTwips.Width;
			else {
				this.width = maxImageSize;
				this.horizontalScaleFactor = Convert.ToUInt16(run.ScaleX * ((double)image.SizeInTwips.Width / maxImageSize) * 10);
			}
			if (run.Image.SizeInTwips.Height <= maxImageSize)
				this.height = (ushort)image.SizeInTwips.Height;
			else {
				this.height = maxImageSize;
				this.verticalScaleFactor = Convert.ToUInt16(run.ScaleY * ((double)image.SizeInTwips.Height / maxImageSize) * 10);
			}
		}
		protected internal void Read(BinaryReader reader, int offset) {
			if (!CheckReader(reader, offset))
				return;
			int size = reader.ReadInt32();
			if (reader.ReadInt16() != picturePropertiesSize) 
				return; 
			this.dataFormat = (PictureDataFormat)reader.ReadInt16();
			ReadCore(reader, size);
		}
		bool CheckReader(BinaryReader reader, int offset) {
			Guard.ArgumentNotNull(reader, "reader");
			if (reader.BaseStream.Length <= offset)
				return false;
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			return true;
		}
		void ReadCore(BinaryReader reader, int size) {
			reader.BaseStream.Seek(innerHeaderSize, SeekOrigin.Current); 
			ReadImageSize(reader);
			if (Width == 0 && Height == 0)
				return;
			reader.BaseStream.Seek(reservedDataSize, SeekOrigin.Current);
			InitializeBorders(reader);
			reader.BaseStream.Seek(obsoletePropertiesSize, SeekOrigin.Current);
			size -= picturePropertiesSize;
			if (DataFormat == PictureDataFormat.ShapeFile)
				size -= ReadPictureName(reader);
			if (this.dataFormat == PictureDataFormat.ShapeFile || this.dataFormat == PictureDataFormat.ShapeObject)
				InitializeShapeContainer(reader, size);
		}
		void ReadImageSize(BinaryReader reader) {
			this.width = reader.ReadUInt16();
			this.height = reader.ReadUInt16();
			this.horizontalScaleFactor = reader.ReadUInt16();
			this.verticalScaleFactor = reader.ReadUInt16();
		}
		void InitializeBorders(BinaryReader reader) {
			this.top = BorderDescriptor97.FromStream(reader);
			this.left = BorderDescriptor97.FromStream(reader);
			this.bottom = BorderDescriptor97.FromStream(reader);
			this.right = BorderDescriptor97.FromStream(reader);
		}
		int ReadPictureName(BinaryReader reader) {
			int total = 0;
			byte pictureNameLength = reader.ReadByte();
			total += 1;
			byte[] bytes = reader.ReadBytes(pictureNameLength);
			this.pictureName = DXEncoding.ASCII.GetString(bytes, 0, bytes.Length);
			total += pictureNameLength;
			return total;
		}
		void InitializeShapeContainer(BinaryReader reader, int size) {
			this.inlineShapeContainer = OfficeArtInlineShapeContainer.FromStream(reader, size);
			if (this.inlineShapeContainer.Blips != null) {
				List<BlipBase> blips = this.inlineShapeContainer.Blips;
				int count = blips.Count;
				for (int i = 0; i < count; i++) {
					this.images.Add(blips[i].Image);
					this.metafileHeaders.Add(blips[i].MetafileHeader);
				}
			}
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			long start = writer.BaseStream.Position;
			WriteDefaults(writer);
			WriteImageSize(writer);
			writer.BaseStream.Seek(reservedDataSize, SeekOrigin.Current);
			WriteEmptyBorders(writer);
			writer.BaseStream.Seek(obsoletePropertiesSize, SeekOrigin.Current);
			this.inlineShapeContainer.Write(writer);
			WritePictureDescriptorSize(writer, start);
		}
		void WriteDefaults(BinaryWriter writer) {
			writer.BaseStream.Seek(4, SeekOrigin.Current);
			writer.Write(picturePropertiesSize);
			writer.Write((short)PictureDataFormat.ShapeObject);
			writer.BaseStream.Seek(innerHeaderSize, SeekOrigin.Current);
		}
		void WriteImageSize(BinaryWriter writer) {
			writer.Write(this.width);
			writer.Write(this.height);
			writer.Write(this.horizontalScaleFactor);
			writer.Write(this.verticalScaleFactor);
		}
		void WritePictureDescriptorSize(BinaryWriter writer, long start) {
			int size = (int)(writer.BaseStream.Position - start);
			writer.BaseStream.Seek(start, SeekOrigin.Begin);
			writer.Write(size);
			writer.BaseStream.Seek(start + size, SeekOrigin.Begin);
		}
		void WriteEmptyBorders(BinaryWriter writer) {
			BorderDescriptor97 emptyBorder = new BorderDescriptor97();
			for (int i = 0; i < 4; i++) {
				emptyBorder.Write(writer);
			}
		}
	}
}
