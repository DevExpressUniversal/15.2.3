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
using System.Drawing;
using System.Text;
using System.IO;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Crypt;
using DevExpress.Utils.Zip;
using DevExpress.Compatibility.System.Drawing;
using System.IO.Compression;
using DevExpress.Office.PInvoke;
using System.Diagnostics;
namespace DevExpress.Office.Utils {
	#region BlipTypeCodes
	public static class BlipTypeCodes {
		public const int FileBlipStoreEntry = 0xf007;
		public const int BlipEmf = 0xf01a;
		public const int BlipWmf = 0xf01b;
		public const int BlipPict = 0xf01c;
		public const int BlipJpeg = 0xf01d;
		public const int BlipPng = 0xf01e;
		public const int BlipDib = 0xf01f;
		public const int BlipTiff = 0xf029;
		public const int BlipJpeg2 = 0xf02a;
	}
	#endregion
	#region BlipTypes
	public static class BlipTypes {
		public const int Error = 0x00;
		public const int Unknown = 0x01;
		public const int Emf = 0x02;
		public const int Wmf = 0x03;
		public const int MacintoshPict = 0x04;
		public const int Jpeg = 0x05;
		public const int Png = 0x06;
		public const int Dib = 0x07;
		public const int Tiff = 0x11;
		public const int CMYKJpeg = 0x12;
	}
	#endregion
	#region MessageDigestCodes
	public static class MessageDigestCodes {
		public const int EmfSingleMessage = 0x3d4;
		public const int EmfDoubleMessage = 0x3d5;
		public const int WmfSingleMessage = 0x216;
		public const int WmfDoubleMessage = 0x217;
		public const int PictSingleMessage = 0x542;
		public const int PictDoubleMessage = 0x543;
		public const int JpegRGBSingleMessage = 0x46a;
		public const int JpegCMYKSingleMessage = 0x6e2;
		public const int JpegRGBDoubleMessage = 0x46b;
		public const int JpegCMYKDoubleMessage = 0x6e3;
		public const int PngSingleMessage = 0x6e0;
		public const int PngDoubleMessage = 0x6e1;
		public const int DibSingleMessage = 0x7a8;
		public const int DibDoubleMessage = 0x7a9;
		public const int TiffSingleMessage = 0x6e4;
		public const int TiffDoubleMessage = 0x6e5;
	}
	#endregion
	#region BlipFactory
	public static class BlipFactory {
		static Dictionary<int, ConstructorInfo> typeCodeTranslationTable;
		static Dictionary<OfficeImageFormat, ConstructorInfo> imageFormatTranslationTable;
		static BlipFactory() {
			typeCodeTranslationTable = new Dictionary<int, ConstructorInfo>(9);
			typeCodeTranslationTable.Add(BlipTypeCodes.BlipEmf, GetConstructor(typeof(BlipEmf), new Type[] { typeof(BinaryReader), typeof(OfficeArtRecordHeader) }));
			typeCodeTranslationTable.Add(BlipTypeCodes.BlipWmf, GetConstructor(typeof(BlipWmf), new Type[] { typeof(BinaryReader), typeof(OfficeArtRecordHeader) }));
			typeCodeTranslationTable.Add(BlipTypeCodes.BlipPict, GetConstructor(typeof(BlipPict), new Type[] { typeof(BinaryReader), typeof(OfficeArtRecordHeader) }));
			typeCodeTranslationTable.Add(BlipTypeCodes.BlipJpeg, GetConstructor(typeof(BlipJpeg), new Type[] { typeof(BinaryReader), typeof(OfficeArtRecordHeader) }));
			typeCodeTranslationTable.Add(BlipTypeCodes.BlipPng, GetConstructor(typeof(BlipPng), new Type[] { typeof(BinaryReader), typeof(OfficeArtRecordHeader) }));
			typeCodeTranslationTable.Add(BlipTypeCodes.BlipDib, GetConstructor(typeof(BlipDib), new Type[] { typeof(BinaryReader), typeof(OfficeArtRecordHeader) }));
			typeCodeTranslationTable.Add(BlipTypeCodes.BlipTiff, GetConstructor(typeof(BlipTiff), new Type[] { typeof(BinaryReader), typeof(OfficeArtRecordHeader) }));
			typeCodeTranslationTable.Add(BlipTypeCodes.BlipJpeg2, GetConstructor(typeof(BlipJpeg), new Type[] { typeof(BinaryReader), typeof(OfficeArtRecordHeader) }));
			imageFormatTranslationTable = new Dictionary<OfficeImageFormat, ConstructorInfo>();
			imageFormatTranslationTable.Add(OfficeImageFormat.Bmp, GetConstructor(typeof(BlipDib), new Type[] { typeof(OfficeImage) }));
			imageFormatTranslationTable.Add(OfficeImageFormat.MemoryBmp, GetConstructor(typeof(BlipDib), new Type[] { typeof(OfficeImage) }));
			imageFormatTranslationTable.Add(OfficeImageFormat.Emf, GetConstructor(typeof(BlipEmf), new Type[] { typeof(OfficeImage) }));
			imageFormatTranslationTable.Add(OfficeImageFormat.Jpeg, GetConstructor(typeof(BlipJpeg), new Type[] { typeof(OfficeImage) }));
			imageFormatTranslationTable.Add(OfficeImageFormat.Png, GetConstructor(typeof(BlipPng), new Type[] { typeof(OfficeImage) }));
			imageFormatTranslationTable.Add(OfficeImageFormat.Gif, GetConstructor(typeof(BlipPng), new Type[] { typeof(OfficeImage) }));
			imageFormatTranslationTable.Add(OfficeImageFormat.Tiff, GetConstructor(typeof(BlipTiff), new Type[] { typeof(OfficeImage) }));
			imageFormatTranslationTable.Add(OfficeImageFormat.Wmf, GetConstructor(typeof(BlipWmf), new Type[] { typeof(OfficeImage) }));
		}
		public static ConstructorInfo GetConstructor(Type type, Type[] parameters) {
			return type.GetConstructor(parameters);
		}
		static bool CompareParameterTypes(ParameterInfo[] ciParameters, Type[] parameters) {
			if (ciParameters.Length != parameters.Length)
				return false;
			int count = ciParameters.Length;
			for (int i = 0; i < count; i++)
				if (ciParameters[i].ParameterType != parameters[i])
					return false;
			return true;
		}
	public static List<BlipBase> ReadAllBlips(BinaryReader reader, long endPosition) {
			return ReadAllBlips(reader, reader, endPosition);
		}
		public static List<BlipBase> ReadAllBlips(BinaryReader reader, BinaryReader embeddedReader, long endPosition) {
			List<BlipBase> result = new List<BlipBase>();
			while (reader.BaseStream.Position < endPosition) {
				OfficeArtRecordHeader blipHeader = OfficeArtRecordHeader.FromStream(reader);
				long expectedBlipEnd = reader.BaseStream.Position + blipHeader.Length;
				BlipBase blip = CreateBlipFromStream(reader, embeddedReader, blipHeader);
				if (blip != null)
					result.Add(blip);
				reader.BaseStream.Position = Math.Max(expectedBlipEnd, reader.BaseStream.Position);
			}
			Debug.Assert(reader.BaseStream.Position == endPosition);
			return result;
		}
		public static BlipBase CreateBlipFromStream(BinaryReader reader, OfficeArtRecordHeader header) {
			return CreateBlipFromStreamCore(reader, reader, header);
		}
		public static BlipBase CreateBlipFromStream(BinaryReader reader, BinaryReader embeddedReader, OfficeArtRecordHeader header) {
			return CreateBlipFromStreamCore(reader, embeddedReader, header);
		}
		static BlipBase CreateBlipFromStreamCore(BinaryReader reader, BinaryReader embeddedReader, OfficeArtRecordHeader header) {
			int typeCode = header.TypeCode;
			if (typeCode == BlipTypeCodes.FileBlipStoreEntry)
				return new FileBlipStoreEntry(reader, embeddedReader, header);
			ConstructorInfo constructor;
			if (typeCodeTranslationTable.TryGetValue(typeCode, out constructor))
				return constructor.Invoke(new object[] { reader, header }) as BlipBase;
			return null;
		}
		public static BlipBase CreateBlipFromImage(OfficeImage image) {
			ConstructorInfo constructor;
			if (imageFormatTranslationTable.TryGetValue(image.RawFormat, out constructor))
				return constructor.Invoke(new object[] { image }) as BlipBase;
			return new BlipDib(image);
		}
	}
#endregion
#region BlipBase (abstract class)
	public abstract class BlipBase : IDisposable {
#region Fields
		public const int MD4MessageSize = 0x10;
		public const int TagSize = 0x1;
		internal const int MetafileHeaderSize = 0x22;
		internal const byte DefaultTagValue = 0xff;
		OfficeImage image;
		Stream imageBytesStream;
		DocMetafileHeader metafileHeader;
#endregion
		protected BlipBase() {
			TagValue = DefaultTagValue;
		}
		protected BlipBase(BinaryReader reader, OfficeArtRecordHeader header) { 
			Guard.ArgumentNotNull(reader, "reader");
			TagValue = DefaultTagValue;
			Read(reader, header);
		}
		protected BlipBase(OfficeImage image) {
			TagValue = DefaultTagValue;
			SetImage(image);
		}
#region Properties
		public abstract int SingleMD4Message { get; }
		public abstract int DoubleMD4Message { get; }
		public byte TagValue { get; protected set; }
		public DocMetafileHeader MetafileHeader { get { return metafileHeader; } }
		public OfficeImage Image { get { return image; } set { SetImage(value); } }
		protected internal Stream ImageBytesStream {
			get {
				if(imageBytesStream == null)
					imageBytesStream = CreateImageBytesStream();
				return imageBytesStream;
			}
		}
		protected internal int ImageBytesLength {
			get {
				if(imageBytesStream == null)
					imageBytesStream = CreateImageBytesStream();
				return (imageBytesStream != null) ? (int)imageBytesStream.Length : 0; 
			} 
		}
		protected internal virtual OfficeImageFormat Format { get { return image == null ? OfficeImageFormat.None : image.RawFormat; } }
#endregion
		void SetImage(OfficeImage image) {
			if (image == null)
				return;
			this.image = image;
		}
		protected internal virtual void SetImage(BlipBase blip) {
			SetImage(blip.Image);
			this.metafileHeader = blip.metafileHeader;
			this.TagValue = blip.TagValue;
		}
		protected internal virtual void SetImage(DocMetafileReader reader) {
			SetImage(reader.Image);
			this.metafileHeader = reader.MetafileHeader;
		}
		protected virtual void Read(BinaryReader reader, OfficeArtRecordHeader header) {
			int offset = CalcMD4MessageOffset(header.InstanceInfo);
			reader.BaseStream.Seek(offset, SeekOrigin.Current); 
			int size = header.Length - offset;
			byte[] buffer = reader.ReadBytes(size);
			MemoryStream imageStream = new MemoryStream(buffer);
			LoadImageFromStream(imageStream);
		}
		public virtual MD4MessageDigest Write(BinaryWriter writer) {
			OfficeArtRecordHeader header = CreateRecordHeader();
			header.Write(writer);
			long position = writer.BaseStream.Position;
			writer.Seek(MD4MessageDigest.Size, SeekOrigin.Current);
			writer.Write(TagValue);
			MD4MessageDigest digest = SaveImageToStream(writer);
			long end = writer.BaseStream.Position;
			writer.Seek((int)position, SeekOrigin.Begin);
			writer.Write(digest.ToArray());
			writer.Seek((int)end, SeekOrigin.Begin);
			return digest;
		}
		protected virtual void LoadImageFromStream(MemoryStream imageStream) {
			Image = OfficeImage.CreateImage(imageStream);
		}
		protected virtual MD4MessageDigest SaveImageToStream(BinaryWriter writer) {
			Stream stream = ImageBytesStream;
			if (stream == null)
				return new MD4MessageDigest();
			byte[] buffer = new byte[4096];
			int bufferLength = buffer.Length;
			long length = ImageBytesLength;
			MD4HashCalculator calculator = new MD4HashCalculator();
			uint[] hash = calculator.InitialCheckSumValue;
			while (length >= bufferLength) {
				stream.Read(buffer, 0, bufferLength);
				length -= bufferLength;
				writer.Write(buffer);
				hash = calculator.UpdateCheckSum(hash, buffer, 0, bufferLength);
			}
			if (length > 0) {
				stream.Read(buffer, 0, (int)length);
				writer.Write(buffer, 0, (int)length);
				hash = calculator.UpdateCheckSum(hash, buffer, 0, (int)length);
			}
			return new MD4MessageDigest(calculator.GetFinalCheckSum(hash));
		}
		public virtual int GetSize() {
			return OfficeArtRecordHeader.Size + MD4MessageSize + TagSize + ImageBytesLength;
		}
		protected virtual DocMetafileHeader CreateMetafileHeader() {
			DocMetafileHeader result = new DocMetafileHeader();
			int imageSize = ImageBytesLength;
			int widthInPixels = Image.SizeInPixels.Width;
			int heightInPixels = Image.SizeInPixels.Height;
			result.Compressed = false;
			result.CompressedSize = imageSize;
			result.UncompressedSize = imageSize;
			result.Right = 0;
			result.Left = widthInPixels;
			result.Top = 0;
			result.Bottom = heightInPixels;
			result.HeightInEmus = heightInPixels;
			result.WidthInEmus = widthInPixels;
			return result;
		}
		protected internal virtual int CalcMD4MessageOffset(int uidInfo) {
			if (uidInfo == SingleMD4Message)
				return MD4MessageSize + TagSize;
			if (uidInfo == DoubleMD4Message)
				return (MD4MessageSize * 2) + TagSize;
			OfficeArtExceptions.ThrowInvalidContent();
			return 0;
		}
		public abstract OfficeArtRecordHeader CreateRecordHeader();
		protected virtual Stream CreateImageBytesStream() {
			if(image != null)
				return image.GetImageBytesStreamSafe(Format);
			return null;
		}
#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (imageBytesStream != null) {
					imageBytesStream.Dispose();
					imageBytesStream = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~BlipBase() {
			Dispose(false);
		}
#endregion
	}
#endregion
#region BlipMetaFile (abstract class)
	public abstract class BlipMetaFile : BlipBase {
#region Fields
		const int bufferSize = 0x4000;
		const int maxUncompressedSize = 0x8000;
		int uncompressedSize;
		MD4MessageDigest md4Digest;
#endregion
		protected BlipMetaFile(BinaryReader reader, OfficeArtRecordHeader header)
			: base(reader, header) {
		}
		protected BlipMetaFile(OfficeImage image)
			: base(image) {
		}
		protected override void Read(BinaryReader reader, OfficeArtRecordHeader header) {
			long end = reader.BaseStream.Position + header.Length;
			int uidOffset = CalcMD4MessageOffset(header.InstanceInfo);
			reader.BaseStream.Seek(uidOffset, SeekOrigin.Current); 
			DocMetafileReader metafileReader = new DocMetafileReader();
			metafileReader.Read(reader);
			if (reader.BaseStream.Position != end && reader.BaseStream.Length >= end)
				reader.BaseStream.Seek(end, SeekOrigin.Begin);
			SetImage(metafileReader);
		}
		public override MD4MessageDigest Write(BinaryWriter writer) {
			OfficeArtRecordHeader header = CreateRecordHeader();
			header.Write(writer);
			writer.Write(this.md4Digest.ToArray());
			DocMetafileHeader metafileHeader = CreateMetafileHeader();
			metafileHeader.Write(writer);
			byte[] buf = new byte[bufferSize];
			int bytesToWrite = ImageBytesLength;
			Stream stream = ImageBytesStream;
			while(bytesToWrite > 0) {
				int count = Math.Min(bytesToWrite, bufferSize);
				stream.Read(buf, 0, count);
				writer.Write(buf, 0, count);
				bytesToWrite -= count;
			}
			return this.md4Digest;
		}
		protected internal override int CalcMD4MessageOffset(int uidInfo) {
			if (uidInfo == SingleMD4Message)
				return MD4MessageSize;
			if (uidInfo == DoubleMD4Message)
				return 2 * MD4MessageSize;
			Exceptions.ThrowInternalException();
			return 0;
		}
		public override int GetSize() {
			return OfficeArtRecordHeader.Size + MD4MessageSize + MetafileHeaderSize + ImageBytesLength;
		}
		protected override DocMetafileHeader CreateMetafileHeader() {
			DocMetafileHeader result = new DocMetafileHeader();
			int imageSize = ImageBytesLength;
			int widthInPixels = Image.SizeInPixels.Width;
			int heightInPixels = Image.SizeInPixels.Height;
			result.Compressed = imageSize != uncompressedSize;
			result.CompressedSize = imageSize;
			result.UncompressedSize = uncompressedSize;
			result.Right = 0;
			result.Left = widthInPixels;
			result.Top = 0;
			result.Bottom = heightInPixels;
			result.HeightInEmus = heightInPixels;
			result.WidthInEmus = widthInPixels;
			return result;
		}
		protected override Stream CreateImageBytesStream() {
			Stream uncompressedStream = base.CreateImageBytesStream();
			uncompressedSize = (int)uncompressedStream.Length;
#if !SL
			if(uncompressedSize <= maxUncompressedSize) {
				CalcMD4Digest(uncompressedStream);
				return uncompressedStream;
			}
			ChunkedMemoryStream compressedStream = new ChunkedMemoryStream();
			compressedStream.WriteByte(0x78); 
			compressedStream.WriteByte(0x01); 
			Adler32CheckSumCalculator adler32CheckSumCalculator = new Adler32CheckSumCalculator();
			uint checkSum = adler32CheckSumCalculator.InitialCheckSumValue;
			MD4HashCalculator md4HashCalculator = new MD4HashCalculator();
			uint[] hash = md4HashCalculator.InitialCheckSumValue;
			using(DeflateStream compressionStream = new DeflateStream(compressedStream, CompressionMode.Compress, true)) {
				byte[] buf = new byte[bufferSize];
				int bytesToCompress = uncompressedSize;
				while(bytesToCompress > 0) {
					int count = Math.Min(bytesToCompress, bufferSize);
					uncompressedStream.Read(buf, 0, count);
					compressionStream.Write(buf, 0, count);
					checkSum = adler32CheckSumCalculator.UpdateCheckSum(checkSum, buf, 0, count);
					hash = md4HashCalculator.UpdateCheckSum(hash, buf, 0, count);
					bytesToCompress -= count;
				}
			}
			this.md4Digest = new MD4MessageDigest(md4HashCalculator.GetFinalCheckSum(hash));
			checkSum = adler32CheckSumCalculator.GetFinalCheckSum(checkSum);
			byte[] checkSumBytes = BitConverter.GetBytes(checkSum);
			compressedStream.Write(checkSumBytes, 0, checkSumBytes.Length);
			compressedStream.Flush();
			compressedStream.Position = 0;
			return compressedStream;
#else
			CalcMD4Digest(uncompressedStream);
			return uncompressedStream;
#endif
		}
		void CalcMD4Digest(Stream stream) {
			byte[] buf = new byte[bufferSize];
			MD4HashCalculator calculator = new MD4HashCalculator();
			uint[] hash = calculator.InitialCheckSumValue;
			int bytesToRead = (int)stream.Length;
			while(bytesToRead > 0) {
				int count = Math.Min(bytesToRead, bufferSize);
				stream.Read(buf, 0, count);
				hash = calculator.UpdateCheckSum(hash, buf, 0, count);
				bytesToRead -= count;
			}
			stream.Position = 0;
			this.md4Digest = new MD4MessageDigest(calculator.GetFinalCheckSum(hash));
		}
	}
#endregion
#region FileBlipStoreEntry
	public class FileBlipStoreEntry : BlipBase {
#region Fields
		const uint emptySlotOffset = 0xffffffff;
		const int headerVersion = 0x2;
		const int blipStoreHeaderSize = 0x24;
		const short tag = 0xff;
		const int defaultDelay = 0x44;
		const byte defaultNameLength = 0;
		const int unusedDataSize = 24;
		int refCount = 1;
		uint embeddedBlipOffset;
		byte blipType;
		string name;
		int embeddedBlipSize;
		bool hasDelayedStream;
		BinaryReader embeddedReader;
		BlipBase embeddedBlip;
#endregion
		public FileBlipStoreEntry(BinaryReader reader, BinaryReader embeddedReader, OfficeArtRecordHeader header) {
			this.embeddedReader = embeddedReader;
			Read(reader, header);
		}
		public FileBlipStoreEntry(OfficeImage image, bool hasDelayedStream) {
			this.embeddedBlip = BlipFactory.CreateBlipFromImage(image);
			SetImage(this.embeddedBlip);
			this.blipType = GetBlipType(image.RawFormat);
			this.embeddedBlipSize = this.embeddedBlip.GetSize();
			this.hasDelayedStream = hasDelayedStream;
		}
#region Properties
		public string Name { get { return name; } }
		public int ReferenceCount { get { return refCount; } set { refCount = value; } }
		protected internal BinaryReader EmbeddedReader { get { return embeddedReader; } }
		protected internal int EmbeddedBlipSize { get { return embeddedBlipSize; } }
		protected internal bool HasDelayedStream { get { return hasDelayedStream; } set { hasDelayedStream = value; } }
		public override int SingleMD4Message { get { return embeddedBlip.SingleMD4Message; } }
		public override int DoubleMD4Message { get { return embeddedBlip.DoubleMD4Message; } }
#endregion
		protected override void Read(BinaryReader reader, OfficeArtRecordHeader header) {
			reader.BaseStream.Seek(unusedDataSize, SeekOrigin.Current); 
			ReadCore(reader);
			ReadName(reader);
			ReadEmbeddedBlip(reader);
		}
		void ReadCore(BinaryReader reader) {
			this.refCount = reader.ReadInt32();
			this.embeddedBlipOffset = reader.ReadUInt32();
			reader.BaseStream.Seek(1, SeekOrigin.Current);
		}
		void ReadName(BinaryReader reader) {
			byte nameLength = reader.ReadByte(); 
			reader.BaseStream.Seek(2, SeekOrigin.Current);
			if (nameLength != 0) {
				byte[] bytes = reader.ReadBytes(nameLength - 2);
				this.name = Encoding.Unicode.GetString(bytes, 0, bytes.Length);
				reader.BaseStream.Seek(2, SeekOrigin.Current);
			}
		}
		void ReadEmbeddedBlip(BinaryReader reader) {
			if (this.embeddedBlipOffset == emptySlotOffset || ReferenceCount == 0)
				return;
			if (EmbeddedReader != reader)
				EmbeddedReader.BaseStream.Seek(this.embeddedBlipOffset, SeekOrigin.Begin);
			OfficeArtRecordHeader embeddeBlipHeader = OfficeArtRecordHeader.FromStream(embeddedReader);
			this.embeddedBlip = BlipFactory.CreateBlipFromStream(embeddedReader, embeddeBlipHeader);
			if (this.embeddedBlip != null && this.embeddedBlip.Image != null)
				SetImage(this.embeddedBlip);
			else
				Image = UriBasedOfficeImage.CreatePlaceHolder(null, 0, 0);
		}
		public override MD4MessageDigest Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			long position = WriteCore(writer);
			writer.Write(defaultDelay);
			WriteName(writer);
			MD4MessageDigest digest = this.embeddedBlip.Write(writer);
			long end = writer.BaseStream.Position;
			writer.Seek((int)position, SeekOrigin.Begin);
			writer.Write(digest.ToArray());
			writer.Seek((int)end, SeekOrigin.Begin);
			return digest;
		}
		public void Write(BinaryWriter writer, BinaryWriter embeddedWriter) {
			Guard.ArgumentNotNull(writer, "writer");
			Guard.ArgumentNotNull(embeddedWriter, "embeddedWriter");
			long position = WriteCore(writer);
			if(HasDelayedStream)
				writer.Write((int)embeddedWriter.BaseStream.Position);
			else
				writer.Write((int)0); 
			WriteName(writer);
			MD4MessageDigest digest = this.embeddedBlip.Write(embeddedWriter);
			long end = writer.BaseStream.Position;
			writer.Seek((int)position, SeekOrigin.Begin);
			writer.Write(digest.ToArray());
			writer.Seek((int)end, SeekOrigin.Begin);
		}
		long WriteCore(BinaryWriter writer) {
			OfficeArtRecordHeader header = CreateRecordHeader();
			header.Write(writer);
			writer.Write(this.blipType);
			writer.Write(this.blipType);
			long position = writer.BaseStream.Position;
			writer.Seek(MD4MessageDigest.Size, SeekOrigin.Current);
			writer.Write(tag);
			writer.Write(EmbeddedBlipSize);
			writer.Write(ReferenceCount);
			return position;
		}
		void WriteName(BinaryWriter writer) {
			writer.BaseStream.Seek(1, SeekOrigin.Current);
			writer.Write(defaultNameLength);
			writer.BaseStream.Seek(2, SeekOrigin.Current);
		}
		public override OfficeArtRecordHeader CreateRecordHeader() {
			OfficeArtRecordHeader result = new OfficeArtRecordHeader();
			result.Version = headerVersion;
			result.InstanceInfo = this.blipType;
			result.TypeCode = BlipTypeCodes.FileBlipStoreEntry;
			result.Length =  HasDelayedStream ? blipStoreHeaderSize : blipStoreHeaderSize + EmbeddedBlipSize;
			return result;
		}
		protected virtual byte GetBlipType(OfficeImageFormat format) {
			switch (format) {
				case OfficeImageFormat.Bmp: return BlipTypes.Png;
				case OfficeImageFormat.MemoryBmp: return BlipTypes.Png;
				case OfficeImageFormat.Gif: return BlipTypes.Png;
				case OfficeImageFormat.Emf: return BlipTypes.Emf;
				case OfficeImageFormat.Jpeg: return BlipTypes.Jpeg;
				case OfficeImageFormat.None: return BlipTypes.Unknown;
				case OfficeImageFormat.Png: return BlipTypes.Png;
				case OfficeImageFormat.Tiff: return BlipTypes.Tiff;
				case OfficeImageFormat.Wmf: return BlipTypes.Wmf;
				default: return BlipTypes.Unknown;
			}
		}
		public override int GetSize() {
			return HasDelayedStream ? OfficeArtRecordHeader.Size + blipStoreHeaderSize
				: OfficeArtRecordHeader.Size + blipStoreHeaderSize + EmbeddedBlipSize;
		}
	}
#endregion
#region BlipEmf
	public class BlipEmf : BlipMetaFile {
		public BlipEmf(BinaryReader reader, OfficeArtRecordHeader header)
			: base(reader, header) { }
		public BlipEmf(OfficeImage image)
			: base(image) { }
		public override int SingleMD4Message { get { return MessageDigestCodes.EmfSingleMessage; } }
		public override int DoubleMD4Message { get { return MessageDigestCodes.EmfDoubleMessage; } }
		public override OfficeArtRecordHeader CreateRecordHeader() {
			OfficeArtRecordHeader result = new OfficeArtRecordHeader();
			result.Version = 0;
			result.InstanceInfo = MessageDigestCodes.EmfSingleMessage;
			result.TypeCode = BlipTypeCodes.BlipEmf;
			result.Length = MD4MessageSize + MetafileHeaderSize + ImageBytesLength;
			return result;
		}
	}
#endregion
#region BlipWmf
	public class BlipWmf : BlipMetaFile {
		public BlipWmf(BinaryReader reader, OfficeArtRecordHeader header)
			: base(reader, header) { }
		public BlipWmf(OfficeImage image)
			: base(image) { }
		public override int SingleMD4Message { get { return MessageDigestCodes.WmfSingleMessage; } }
		public override int DoubleMD4Message { get { return MessageDigestCodes.WmfDoubleMessage; } }
		public override OfficeArtRecordHeader CreateRecordHeader() {
			OfficeArtRecordHeader result = new OfficeArtRecordHeader();
			result.Version = 0;
			result.InstanceInfo = MessageDigestCodes.WmfSingleMessage;
			result.TypeCode = BlipTypeCodes.BlipWmf;
			result.Length = MD4MessageSize + MetafileHeaderSize + ImageBytesLength;
			return result;
		}
	}
#endregion
#region BlipPict
	public class BlipPict : BlipMetaFile {
		public BlipPict(BinaryReader reader, OfficeArtRecordHeader header)
			: base(reader, header) { }
		public override int SingleMD4Message { get { return MessageDigestCodes.PictSingleMessage; } }
		public override int DoubleMD4Message { get { return MessageDigestCodes.PictDoubleMessage; } }
		protected override void Read(BinaryReader reader, OfficeArtRecordHeader header) {
			int uidOffset = CalcMD4MessageOffset(header.InstanceInfo);
			reader.BaseStream.Seek(uidOffset, SeekOrigin.Current); 
			DocMetafileHeader metafileHeader = DocMetafileHeader.FromStream(reader);
			if (metafileHeader.Compressed)
				reader.BaseStream.Seek(metafileHeader.CompressedSize, SeekOrigin.Current);
			else
				reader.BaseStream.Seek(metafileHeader.UncompressedSize, SeekOrigin.Current);
		}
		public override OfficeArtRecordHeader CreateRecordHeader() {
			OfficeArtRecordHeader result = new OfficeArtRecordHeader();
			result.Version = 0;
			result.InstanceInfo = MessageDigestCodes.PictSingleMessage;
			result.TypeCode = BlipTypeCodes.BlipPict;
			result.Length = MD4MessageSize + MetafileHeaderSize + ImageBytesLength;
			return result;
		}
	}
#endregion
#region BlipJpeg
	public class BlipJpeg : BlipBase {
		public BlipJpeg(BinaryReader reader, OfficeArtRecordHeader header)
			: base(reader, header) { }
		public BlipJpeg(OfficeImage image)
			: base(image) { }
		public override int SingleMD4Message { get { return MessageDigestCodes.JpegRGBSingleMessage; } }
		public override int DoubleMD4Message { get { return MessageDigestCodes.JpegRGBDoubleMessage; } }
		protected internal override int CalcMD4MessageOffset(int uidInfo) {
			if (uidInfo == MessageDigestCodes.JpegRGBSingleMessage || uidInfo == MessageDigestCodes.JpegCMYKSingleMessage)
				return MD4MessageSize + TagSize;
			if (uidInfo == MessageDigestCodes.JpegRGBDoubleMessage || uidInfo == MessageDigestCodes.JpegCMYKDoubleMessage)
				return (MD4MessageSize * 2) + TagSize;
			OfficeArtExceptions.ThrowInvalidContent();
			return 0;
		}
		public override OfficeArtRecordHeader CreateRecordHeader() {
			OfficeArtRecordHeader result = new OfficeArtRecordHeader();
			result.Version = 0;
			result.InstanceInfo = MessageDigestCodes.JpegRGBSingleMessage;
			result.TypeCode = BlipTypeCodes.BlipJpeg;
			result.Length = MD4MessageSize + TagSize + ImageBytesLength;
			return result;
		}
	}
#endregion
#region BlipPng
	public class BlipPng : BlipBase {
		public BlipPng(OfficeImage image)
			: base(image) { }
		public BlipPng(BinaryReader reader, OfficeArtRecordHeader header)
			: base(reader, header) { }
		public override int SingleMD4Message { get { return MessageDigestCodes.PngSingleMessage; } }
		public override int DoubleMD4Message { get { return MessageDigestCodes.PngDoubleMessage; } }
		protected internal override OfficeImageFormat Format { get { return OfficeImageFormat.Png; } }
		public override OfficeArtRecordHeader CreateRecordHeader() {
			OfficeArtRecordHeader result = new OfficeArtRecordHeader();
			result.Version = 0;
			result.InstanceInfo = MessageDigestCodes.PngSingleMessage;
			result.TypeCode = BlipTypeCodes.BlipPng;
			result.Length = MD4MessageSize + TagSize + ImageBytesLength;
			return result;
		}
	}
#endregion
#region BlipDib
	public class BlipDib : BlipBase {
		const int widthPosition = 4;
		const int heightPosition = 8;
		const int bitCountPosition = 14;
		const int dwordSize = 32;
		const int headerInfoSize = 16;
		public BlipDib(OfficeImage image)
			: base(image) { }
		public BlipDib(BinaryReader reader, OfficeArtRecordHeader header)
			: base(reader, header) { }
		public override int SingleMD4Message { get { return MessageDigestCodes.DibSingleMessage; } }
		public override int DoubleMD4Message { get { return MessageDigestCodes.DibDoubleMessage; } }
		protected internal override OfficeImageFormat Format { get { return OfficeImageFormat.Png; } }
		protected override void Read(BinaryReader reader, OfficeArtRecordHeader header) {
			int offset = CalcMD4MessageOffset(header.InstanceInfo);
			reader.BaseStream.Seek(MD4MessageSize, SeekOrigin.Current); 
			this.TagValue = reader.ReadByte();
			if(header.InstanceInfo == DoubleMD4Message)
				reader.BaseStream.Seek(MD4MessageSize, SeekOrigin.Current); 
			int size = header.Length - offset;
			byte[] buffer = reader.ReadBytes(size);
			MemoryStream imageStream = new MemoryStream(buffer);
			LoadImageFromStream(imageStream);
		}
		protected override void LoadImageFromStream(MemoryStream imageStream) {
			BinaryReader reader = new BinaryReader(imageStream);
			DibHeader header = DibHeader.FromStream(reader);
			int bytesInLine = header.BitCount * header.Width;
			if (bytesInLine % dwordSize == 0)
				bytesInLine /= 8;
			else {
				bytesInLine /= 32;
				bytesInLine++;
				bytesInLine *= 4;
			}
			imageStream.Seek(0, SeekOrigin.Begin);
#if !SL
			Image = OfficeImage.CreateImage(DibHelper.CreateDib(imageStream, header.Width, header.Height, bytesInLine));
#else
			OfficeImageSL image = new OfficeImageSL(null);
			image.LoadDibFromStream(imageStream, header.Width, header.Height, bytesInLine);
			Image = image;
#endif
		}
		public override int GetSize() {
			return OfficeArtRecordHeader.Size + MD4MessageSize + TagSize + ImageBytesLength;
		}
		public override OfficeArtRecordHeader CreateRecordHeader() {
			OfficeArtRecordHeader result = new OfficeArtRecordHeader();
			result.Version = 0;
			result.InstanceInfo = MessageDigestCodes.PngSingleMessage;
			result.TypeCode = BlipTypeCodes.BlipPng;
			result.Length = MD4MessageSize + TagSize + ImageBytesLength;
			return result;
		}
	}
#endregion
#region BlipTiff
	public class BlipTiff : BlipBase {
		public BlipTiff(BinaryReader reader, OfficeArtRecordHeader header)
			: base(reader, header) { }
		public BlipTiff(OfficeImage image)
			: base(image) { }
		public override int SingleMD4Message { get { return MessageDigestCodes.TiffSingleMessage; } }
		public override int DoubleMD4Message { get { return MessageDigestCodes.TiffDoubleMessage; } }
		public override OfficeArtRecordHeader CreateRecordHeader() {
			OfficeArtRecordHeader result = new OfficeArtRecordHeader();
			result.Version = 0;
			result.InstanceInfo = MessageDigestCodes.TiffSingleMessage;
			result.TypeCode = BlipTypeCodes.BlipTiff;
			result.Length = MD4MessageSize + TagSize + ImageBytesLength;
			return result;
		}
	}
#endregion
#region MetafileHeader
	public class DocMetafileHeader {
		public static DocMetafileHeader FromStream(BinaryReader reader) {
			DocMetafileHeader result = new DocMetafileHeader();
			result.Read(reader);
			return result;
		}
#region Fields
		const int emusPerHundredthsOfMillimeter = 360;
		const byte deflateCode = 0x00;
		const byte uncompressedCode = 0xfe;
		const byte filterCode = 0xfe;
		int uncompressedSize;
		int left;
		int top;
		int right;
		int bottom;
		int widthInEmus;
		int heightInEmus;
		int compressedSize;
		bool compressed;
#endregion
#region Properties
		public int UncompressedSize { get { return uncompressedSize; } protected internal set { uncompressedSize = value; } }
		public int Left { get { return left; } protected internal set { left = value; } }
		public int Top { get { return top; } protected internal set { top = value; } }
		public int Right { get { return right; } protected internal set { right = value; } }
		public int Bottom { get { return bottom; } protected internal set { bottom = value; } }
		public int WidthInEmus { get { return widthInEmus; } protected internal set { widthInEmus = value; } }
		public int WidthInHundredthsOfMillimeter { get { return widthInEmus / emusPerHundredthsOfMillimeter; } }
		public int HeightInEmus { get { return heightInEmus; } protected internal set { heightInEmus = value; } }
		public int HeightInHundredthsOfMillimeter { get { return heightInEmus / emusPerHundredthsOfMillimeter; } }
		public int CompressedSize { get { return compressedSize; } protected internal set { compressedSize = value; } }
		public bool Compressed { get { return compressed; } protected internal set { compressed = value; } }
#endregion
		protected void Read(BinaryReader reader) {
			this.uncompressedSize = reader.ReadInt32();
			this.left = reader.ReadInt32();
			this.top = reader.ReadInt32();
			this.right = reader.ReadInt32();
			this.bottom = reader.ReadInt32();
			this.widthInEmus = reader.ReadInt32();
			this.heightInEmus = reader.ReadInt32();
			this.compressedSize = reader.ReadInt32();
			byte compressionCode = reader.ReadByte();
			if (compressionCode == deflateCode)
				this.compressed = true;
			if (reader.ReadByte() != 0xfe)
				OfficeArtExceptions.ThrowInvalidContent();
		}
		public void Write(BinaryWriter writer) {
			writer.Write(this.uncompressedSize);
			writer.Write(this.left);
			writer.Write(this.top);
			writer.Write(this.right);
			writer.Write(this.bottom);
			writer.Write(this.widthInEmus);
			writer.Write(this.heightInEmus);
			writer.Write(this.compressedSize);
			writer.Write(this.compressed ? deflateCode : uncompressedCode);
			writer.Write(filterCode);
		}
	}
#endregion
#region DibHeader
	public class DibHeader {
		public static DibHeader FromStream(BinaryReader reader) {
			DibHeader result = new DibHeader();
			result.Read(reader);
			return result;
		}
#region Fields
		public const int DibHeaderSize = 0x28;
		const int dibPlanes = 0x1;
		int headerSize;
		int width;
		int height;
		short planes;
		short bitCount;
		int compression;
		int imageSize;
		int horizontalPixelsPerMeter;
		int verticalPixelsPerMeter;
		int usedColors;
		int importantColors;
#endregion
		public DibHeader() {
			this.headerSize = DibHeaderSize;
			this.planes = dibPlanes;
		}
#region Properties
		protected internal int HeaderSize { get { return headerSize; } }
		public int Width { get { return width; } set { width = value; } }
		public int Height { get { return height; } set { height = value; } }
		protected internal short Planes { get { return planes; } }
		public short BitCount { get { return bitCount; } set { bitCount = value; } }
		protected internal int Compression { get { return compression; } set { compression = value; } }
		protected internal int ImageSize { get { return imageSize; } set { imageSize = value; } }
		protected internal int HorizontalPixelsPerMeter { get { return horizontalPixelsPerMeter; } set { horizontalPixelsPerMeter = value; } }
		protected internal int VerticalPixelsPerMeter { get { return verticalPixelsPerMeter; } set { verticalPixelsPerMeter = value; } }
		protected internal int UsedColors { get { return usedColors; } set { usedColors = value; } }
		protected internal int ImportantColors { get { return importantColors; } set { importantColors = value; } }
#endregion
		protected internal void Read(BinaryReader reader) {
			this.headerSize = reader.ReadInt32();
			if (headerSize != DibHeaderSize)
				OfficeArtExceptions.ThrowInvalidContent();
			this.width = reader.ReadInt32();
			this.height = reader.ReadInt32();
			this.planes = reader.ReadInt16();
			this.bitCount = reader.ReadInt16();
			this.compression = reader.ReadInt32();
			this.imageSize = reader.ReadInt32();
			this.horizontalPixelsPerMeter = reader.ReadInt32();
			this.verticalPixelsPerMeter = reader.ReadInt32();
			this.usedColors = reader.ReadInt32();
			this.importantColors = reader.ReadInt32();
		}
		public void Write(BinaryWriter writer) {
			writer.Write(this.headerSize);
			writer.Write(this.width);
			writer.Write(this.height);
			writer.Write(this.planes);
			writer.Write(this.bitCount);
			writer.Write(this.compression);
			writer.Write(this.imageSize);
			writer.Write(this.horizontalPixelsPerMeter);
			writer.Write(this.verticalPixelsPerMeter);
			writer.Write(this.usedColors);
			writer.Write(this.importantColors);
		}
	}
#endregion
#region DocMetafileReader
	public class DocMetafileReader {
#region Fields
		DocMetafileHeader metafileHeader;
		OfficeImage image;
#endregion
#region Properties
		public DocMetafileHeader MetafileHeader { get { return this.metafileHeader; } }
		public OfficeImage Image { get { return this.image; } }
#endregion
		public void Read(BinaryReader reader) {
			this.metafileHeader = DocMetafileHeader.FromStream(reader);
			if (!this.metafileHeader.Compressed) {
				byte[] buffer = reader.ReadBytes(this.metafileHeader.UncompressedSize);
				using (MemoryStream metafileStream = new MemoryStream(buffer)) {
					this.image = OfficeImage.CreateImage(metafileStream);
				}
			}
			else {
				CheckCompressedData(reader);
				byte[] buffer = reader.ReadBytes(this.metafileHeader.CompressedSize - 2);
#if !SL && !DXPORTABLE
				using (MemoryStream compressedStream = new MemoryStream(buffer)) {
					DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress);
					buffer = new byte[metafileHeader.UncompressedSize];
					deflateStream.Read(buffer, 0, metafileHeader.UncompressedSize); 
					using (MemoryStream uncompressedStream = new MemoryStream()) {
						uncompressedStream.Write(buffer, 0, buffer.Length);
						int clipWidth = MetafileHeader.Right - MetafileHeader.Left;
						int clipHeight = MetafileHeader.Bottom - MetafileHeader.Top;
						Image nativeMetafileImage = MetafileHelper.CreateMetafile(uncompressedStream, Win32.MapMode.Anisotropic, clipWidth, clipHeight);
						OfficeMetafileImageWin metafileImage = (OfficeMetafileImageWin)OfficeImage.CreateImage(new MemoryStreamBasedImage(nativeMetafileImage, null));
						metafileImage.MetafileSizeInHundredthsOfMillimeter = new Size(MetafileHeader.WidthInHundredthsOfMillimeter, MetafileHeader.HeightInHundredthsOfMillimeter);
						this.image = metafileImage;
					}
				}
#endif
			}
		}
		void CheckCompressedData(BinaryReader reader) {
			byte data = reader.ReadByte();
			byte compressionMethod = (byte)(data & 0x0f);
			data = reader.ReadByte();
			byte presetDictionary = (byte)(data & 0x20);
			if ((compressionMethod != 8) | (presetDictionary != 0))
				OfficeArtExceptions.ThrowInvalidContent();
		}
	}
	#endregion
	#region MD4MessageDigest
	public struct MD4MessageDigest {
		uint[] digest;
		public const int Size = 16;
		internal MD4MessageDigest(uint[] digest) {
			this.digest = digest;
		}
		public byte[] ToArray() {
			byte[] result = new byte[Size];
			for(int i = 0; i < 4; i++) {
				Pack(result, i * 4, digest[i]);
			}
			return result;
		}
		void Pack(byte[] destination, int offset, uint value) {
			for(int i = 0; i < 4; i++) {
				destination[offset + i] = (byte)((value >> 8 * i) & 0xff);
			}
		}
	}
#endregion
}
