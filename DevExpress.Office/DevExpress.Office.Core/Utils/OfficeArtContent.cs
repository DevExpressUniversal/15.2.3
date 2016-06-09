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
using System.IO;
using System.Text;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.Office;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Office.Utils {
	#region OfficeArtConstants
	public static class OfficeArtConstants {
		public const int DefaultMainDocumentShapeIdentifier = 0x0401;
		public const int DefaultHeaderShapeIdentifier = 0x0800;
		public const int DefaultLineWidthInEmus = 9525;
	}
	#endregion
	#region OfficeArtExceptions
	public static class OfficeArtExceptions {
		public static void ThrowInvalidContent() {
			throw new ArgumentException("Invalid OfficeArt content!");
		}
	}
	#endregion
	#region OfficeArtTypeCodes
	public static class OfficeArtTypeCodes {
		public const int DrawingContainer = 0xf000;
		public const int BlipStoreContainer = 0xf001;
		public const int DrawingObjectsContainer = 0xf002;
		public const int ShapeGroupContainer = 0xf003;
		public const int ShapeContainer = 0xf004;
		public const int FileDrawingGroupRecord = 0xf006;
		public const int FileDrawingRecord = 0xf008;
		public const int ShapeGroupCoordinateSystem = 0xf009;
		public const int FileShape = 0xf00a;
		public const int PropertiesTable = 0xf00b;
		public const int ClientTextbox = 0xf00d;
		public const int ChildAnchor = 0xf00f;
		public const int ClientAnchor = 0xf010;
		public const int ClientData = 0xf011;
		public const int SplitMenuColorContainer = 0xf11e;
		public const int TertiaryPropertiesTable = 0xf122;
	}
	#endregion
	#region OfficeArtVersions
	public static class OfficeArtVersions {
		public const int DefaultHeaderVersion = 0xf;
		public const int EmptyHeaderVersion = 0x0;
		public const int ShapeGroupCoordinateSystemVersion = 0x1;
		public const int ShapeRecordVersion = 0x2;
		public const int PropertiesVersion = 0x3;
	}
	#endregion
	#region OfficeArtHeaderInfos
	public static class OfficeArtHeaderInfos {
		public const int EmptyHeaderInfo = 0x000;
		public const int SplitMenuColorContainerInfo = 0x004;
		public const int FileDrawingRecordInfo = 0xffe;
		public const int NotPrimitiveShape = 0x000;
		public const int PictureFrameShape = 0x04b;
	}
	#endregion
	#region OfficeDrawingPartBase
	public abstract class OfficeDrawingPartBase {
		#region Properties
		public abstract int HeaderVersion { get; }
		public abstract int HeaderInstanceInfo { get; }
		public abstract int HeaderTypeCode { get; }
		public virtual int Length { get { return GetSize(); } }
		#endregion
		public void Write(BinaryWriter writer) {
			if (!ShouldWrite())
				return;
			WriteHeader(writer);
			WriteCore(writer);
		}
		protected internal void WriteHeader(BinaryWriter writer) {
			OfficeArtRecordHeader header = new OfficeArtRecordHeader();
			header.InstanceInfo = HeaderInstanceInfo;
			header.Length = Length;
			header.TypeCode = HeaderTypeCode;
			header.Version = HeaderVersion;
			header.Write(writer);
		}
		protected internal virtual void WriteCore(BinaryWriter writer) {
		}
		protected internal virtual bool ShouldWrite() {
			return true;
		}
		protected internal abstract int GetSize();
	}
	#endregion
	#region CompositeOfficeDrawingPartBase
	public abstract class CompositeOfficeDrawingPartBase : OfficeDrawingPartBase {
		readonly List<OfficeDrawingPartBase> items;
		protected CompositeOfficeDrawingPartBase() {
			this.items = new List<OfficeDrawingPartBase>();
		}
		public List<OfficeDrawingPartBase> Items { get { return items; } }
		protected internal override void WriteCore(BinaryWriter writer) {
			int count = Items.Count;
			for (int i = 0; i < count; i++) {
				Items[i].Write(writer);
			}
		}
		protected internal override int GetSize() {
			int size = 0;
			int count = Items.Count;
			for (int i = 0; i < count; i++) {
				if (Items[i].ShouldWrite()) {
					size += OfficeArtRecordHeader.Size;
					size += Items[i].GetSize();
				}
			}
			return size;
		}
	}
	#endregion
	#region OfficeArtRecordHeader
	public class OfficeArtRecordHeader {
		public static OfficeArtRecordHeader FromStream(BinaryReader reader) {
			OfficeArtRecordHeader result = new OfficeArtRecordHeader();
			result.Read(reader);
			return result;
		}
		#region Fields
		public const int Size = 8;
		int version;
		int instanceInfo;
		int typeCode;
		int length;
		#endregion
		#region Properties
		public int Version { get { return version; } protected internal set { version = value; } }
		public int InstanceInfo { get { return instanceInfo; } protected internal set { instanceInfo = value; } }
		public int TypeCode { get { return typeCode; } protected internal set { typeCode = value; } }
		public int Length { get { return length; } protected internal set { length = value; } }
		#endregion
		protected internal void Read(BinaryReader reader) {
			ushort flags = reader.ReadUInt16();
			this.version = (flags & 0x000f);
			this.instanceInfo = (flags & 0xfff0) >> 4;
			this.typeCode = (int)reader.ReadUInt16();
			this.length = reader.ReadInt32();
		}
		public void Write(BinaryWriter writer) {
			ushort flags = (ushort)(this.version | (this.instanceInfo << 4));
			writer.Write(flags);
			writer.Write((ushort)this.typeCode);
			writer.Write(this.length);
		}
	}
	#endregion
	#region OfficeArtDrawingContainer
	public class OfficeArtDrawingContainer : CompositeOfficeDrawingPartBase {
		#region static
		public static OfficeArtDrawingContainer FromStream(BinaryReader reader, BinaryReader embeddedReader) {
			OfficeArtDrawingContainer result = new OfficeArtDrawingContainer();
			result.Read(reader, embeddedReader);
			return result;
		}
		#endregion
		#region Fields
		OfficeArtFileDrawingGroupRecord fileDrawingBlock;
		OfficeArtBlipStoreContainer blipContainer;
		OfficeArtSplitMenuColorContainer splitMenuColorContainer;
		#endregion
		public OfficeArtDrawingContainer() {
			this.fileDrawingBlock = new OfficeArtFileDrawingGroupRecord();
			this.blipContainer = new OfficeArtBlipStoreContainer();
			this.splitMenuColorContainer = new OfficeArtSplitMenuColorContainer();
			Items.Add(this.fileDrawingBlock);
			Items.Add(this.blipContainer);
			Items.Add(this.splitMenuColorContainer);
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.DrawingContainer; } }
		public override int HeaderVersion { get { return OfficeArtVersions.DefaultHeaderVersion; } }
		public OfficeArtFileDrawingGroupRecord FileDrawingBlock { get { return fileDrawingBlock; } }
		public OfficeArtBlipStoreContainer BlipContainer { get { return blipContainer; } }
		public OfficeArtSplitMenuColorContainer SplitMenuColorContainer { get { return splitMenuColorContainer; } }
		#endregion
		protected internal void Read(BinaryReader reader, BinaryReader embeddedReader) {
			OfficeArtRecordHeader header = OfficeArtRecordHeader.FromStream(reader);
			CheckHeader(header);
			long endPosition = reader.BaseStream.Position + header.Length;
			while (reader.BaseStream.Position < endPosition) {
				ParseHeader(reader, embeddedReader);
			}
		}
		void CheckHeader(OfficeArtRecordHeader header) {
			if (header.Version != OfficeArtVersions.DefaultHeaderVersion || 
				header.InstanceInfo != OfficeArtHeaderInfos.EmptyHeaderInfo ||
				header.TypeCode != OfficeArtTypeCodes.DrawingContainer)
				OfficeArtExceptions.ThrowInvalidContent();
		}
		public void Write(BinaryWriter writer, BinaryWriter embeddedWriter) {
			WriteHeader(writer);
			FileDrawingBlock.Write(writer);
			BlipContainer.Write(writer, embeddedWriter);
			SplitMenuColorContainer.Write(writer);
		}
		void ParseHeader(BinaryReader reader, BinaryReader embeddedReader) {
			OfficeArtRecordHeader header = OfficeArtRecordHeader.FromStream(reader);
			if (header.TypeCode == OfficeArtTypeCodes.FileDrawingGroupRecord) {
				this.fileDrawingBlock = OfficeArtFileDrawingGroupRecord.FromStream(reader, header);
				return;
			}
			if (header.TypeCode == OfficeArtTypeCodes.BlipStoreContainer) {
				this.blipContainer = OfficeArtBlipStoreContainer.FromStream(reader, embeddedReader, header);
				return;
			}
			if (header.TypeCode == OfficeArtTypeCodes.SplitMenuColorContainer) {
				this.splitMenuColorContainer = OfficeArtSplitMenuColorContainer.FromStream(reader, header);
				return;
			}
			reader.BaseStream.Seek(header.Length, SeekOrigin.Current);
		}
	}
	#endregion
	#region OfficeArtFileDrawingGroupRecord
	public class OfficeArtFileDrawingGroupRecord : OfficeDrawingPartBase {
		public static OfficeArtFileDrawingGroupRecord FromStream(BinaryReader reader, OfficeArtRecordHeader header) {
			OfficeArtFileDrawingGroupRecord result = new OfficeArtFileDrawingGroupRecord();
			result.Read(reader, header);
			return result;
		}
		#region Fields
		const int mainDocumentClusterId = 1;
		const int headerClusterId = 2;
		const int headerVersion = 0x0;
		const int headerInstanceInfo = 0x000;
		const int headerTypeCode = 0xf006;
		const int currentMaximumShapeID = 0x03ffd7ff;
		const int idClusterSize = 0x8;
		const int basePartSize = 0x10;
		int mainDocumentPicturesCount;
		int headerPicturesCount;
		#endregion
		#region Properties
		public override int HeaderInstanceInfo { get { return headerInstanceInfo; } }
		public override int HeaderTypeCode { get { return headerTypeCode; } }
		public override int HeaderVersion { get { return headerVersion; } }
		public int MainDocumentFloatingObjectsCount { get { return mainDocumentPicturesCount; } set { mainDocumentPicturesCount = value; } }
		public int HeaderFloatingObjectsCount { get { return headerPicturesCount; } set { headerPicturesCount = value; } }
		internal bool HasPicturesInHeader { get { return HeaderFloatingObjectsCount > 0; } }
		#endregion
		protected internal void Read(BinaryReader reader, OfficeArtRecordHeader header) {
			int maxShapeIdentifier = reader.ReadInt32();
			if (maxShapeIdentifier >= currentMaximumShapeID)
				OfficeArtExceptions.ThrowInvalidContent();
			int idClustersCount = reader.ReadInt32() - 1;
			if (header.Length != (idClustersCount * idClusterSize) + basePartSize)
				OfficeArtExceptions.ThrowInvalidContent();
			reader.ReadInt32(); 
			reader.ReadInt32(); 
			reader.BaseStream.Seek(idClustersCount * idClusterSize, SeekOrigin.Current); 
		}
		protected internal override void WriteCore(BinaryWriter writer) {
			writer.Write(CalcMaxShapeIdentifier());
			writer.Write(CalcClustersCount() + 1);
			writer.Write(CalcShapesCount());
			writer.Write(CalcDrawingsCount());
			WriteClustersInfo(writer);
		}
		void WriteClustersInfo(BinaryWriter writer) {
			writer.Write(mainDocumentClusterId);
			writer.Write(MainDocumentFloatingObjectsCount + 2);
			if (HasPicturesInHeader) {
				writer.Write(headerClusterId);
				writer.Write(HeaderFloatingObjectsCount + 1);
			}
		}
		int CalcMaxShapeIdentifier() {
			return HasPicturesInHeader ? OfficeArtConstants.DefaultHeaderShapeIdentifier + HeaderFloatingObjectsCount + 1
				: OfficeArtConstants.DefaultMainDocumentShapeIdentifier + MainDocumentFloatingObjectsCount + 1;
		}
		int CalcClustersCount() {
			return HasPicturesInHeader ? 2 : 1;
		}
		int CalcShapesCount() {
			int result = MainDocumentFloatingObjectsCount + 1;
			if (HeaderFloatingObjectsCount > 0)
				result += HeaderFloatingObjectsCount + 1;
			return result;
		}
		int CalcDrawingsCount() {
			return HasPicturesInHeader ? 2 : 1;
		}
		protected internal override int GetSize() {
			return (CalcClustersCount() * idClusterSize) + basePartSize;
		}
	}
	#endregion
	#region OfficeArtBlipStoreContainer
	public class OfficeArtBlipStoreContainer : OfficeDrawingPartBase {
		public static OfficeArtBlipStoreContainer FromStream(BinaryReader reader, BinaryReader embeddedReader, OfficeArtRecordHeader header) {
			OfficeArtBlipStoreContainer result = new OfficeArtBlipStoreContainer();
			result.Read(reader, embeddedReader, header);
			return result;
		}
		#region Fields
		List<BlipBase> blips;
		#endregion
		public OfficeArtBlipStoreContainer() {
			this.blips = new List<BlipBase>();
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return Blips.Count; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.BlipStoreContainer; } }
		public override int HeaderVersion { get { return OfficeArtVersions.DefaultHeaderVersion; } }
		public List<BlipBase> Blips { get { return this.blips; } }
		#endregion
		protected internal void Read(BinaryReader reader, BinaryReader embeddedReader, OfficeArtRecordHeader header) {
			long end = reader.BaseStream.Position + header.Length;
			this.blips = BlipFactory.ReadAllBlips(reader, embeddedReader, end);
		}
		public void Write(BinaryWriter writer, BinaryWriter embeddedWriter) {
			if (!ShouldWrite())
				return;
			WriteHeader(writer);
			WriteCore(writer, embeddedWriter);
		}
		protected internal virtual void WriteCore(BinaryWriter writer, BinaryWriter embeddedWriter) {
			int count = Blips.Count;
			for (int i = 0; i < count; i++) {
				FileBlipStoreEntry fileBlipStore = Blips[i] as FileBlipStoreEntry;
				if (fileBlipStore != null)
					fileBlipStore.Write(writer, embeddedWriter);
				else
					Blips[i].Write(writer);
			}
		}
		protected internal override bool ShouldWrite() {
			return Blips.Count > 0;
		}
		protected internal override int GetSize() {
			int total = 0;
			int count = Blips.Count;
			for (int i = 0; i < count; i++) {
				total += Blips[i].GetSize();
			}
			return total;
		}
	}
	#endregion
	#region OfficeArtSplitMenuColorContainer
	public class OfficeArtSplitMenuColorContainer : OfficeDrawingPartBase {
		#region static
		public static OfficeArtSplitMenuColorContainer FromStream(BinaryReader reader, OfficeArtRecordHeader header) {
			OfficeArtSplitMenuColorContainer result = new OfficeArtSplitMenuColorContainer();
			result.Read(reader, header);
			return result;
		}
		#endregion
		#region Fields
		const int recordLength = 0x10;
		OfficeColorRecord fillColor;
		OfficeColorRecord lineColor;
		OfficeColorRecord shapeColor;
		OfficeColorRecord color3D;
		#endregion
		public OfficeArtSplitMenuColorContainer() {
			this.color3D = new OfficeColorRecord(DXColor.Empty);
			this.fillColor = new OfficeColorRecord(DXColor.Empty);
			this.lineColor = new OfficeColorRecord(DXColor.Empty);
			this.shapeColor = new OfficeColorRecord(DXColor.Empty);
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.SplitMenuColorContainerInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.SplitMenuColorContainer; } }
		public override int HeaderVersion { get { return OfficeArtVersions.EmptyHeaderVersion; } }
		public override int Length { get { return recordLength; } }
		public OfficeColorRecord FillColor { get { return this.fillColor; } }
		public OfficeColorRecord LineColor { get { return this.lineColor; } }
		public OfficeColorRecord ShapeColor { get { return this.shapeColor; } }
		public OfficeColorRecord Color3D { get { return this.color3D; } }
		#endregion
		protected internal void Read(BinaryReader reader, OfficeArtRecordHeader header) {
			if (header.Length != recordLength)
				OfficeArtExceptions.ThrowInvalidContent();
			this.fillColor = OfficeColorRecord.FromStream(reader);
			this.lineColor = OfficeColorRecord.FromStream(reader);
			this.shapeColor = OfficeColorRecord.FromStream(reader);
			this.color3D = OfficeColorRecord.FromStream(reader);
		}
		protected internal override void WriteCore(BinaryWriter writer) {
			FillColor.Write(writer);
			LineColor.Write(writer);
			ShapeColor.Write(writer);
			Color3D.Write(writer);
		}
		protected internal override int GetSize() {
			return recordLength;
		}
	}
	#endregion
	#region OfficeColorUse
	public enum OfficeColorUse {
		None = 0x0000,
		UseFillColor = 0x00f0,
		UseLineOrFillColor = 0x00f1,
		UseLineColor = 0x00f2,
		UseShadowColor = 0x00f3,
		UseCurrentColor = 0x00f4,
		UseFillBackColor = 0x00f5,
		UseLineBackColor = 0x00f6,
		UseFillOrLineColor = 0x00f7
	}
	#endregion
	#region OfficeColorTransform
	public enum OfficeColorTransform {
		None = 0x0000,
		Darken = 0x0100,
		Lighten = 0x0200,
		AddGray = 0x0300,
		SubtractGray = 0x0400,
		ReverseSubtractGray = 0x0500,
		Threshold = 0x0600,
		Invert = 0x2000,
		ToggleHighBit = 0x4000,
		ConvertToGrayscale = 0x8000
	}
	#endregion
	#region OfficeColorRecord
	public class OfficeColorRecord {
		const uint maskDefault = 0xff000000;
		const uint maskColorScheme = 0x08000000;
		const uint maskSystemColor = 0x10000000;
		const uint maskSchemeIndex = 0x000000ff;
		const uint maskSystemIndex = 0x0000ffff;
		const uint maskColorUse = 0x000000ff;
		const uint maskTransform = 0x0000ff00;
		const uint maskTransformValue = 0x00ff0000;
		uint data;
		public static OfficeColorRecord FromStream(BinaryReader reader) {
			OfficeColorRecord result = new OfficeColorRecord();
			result.Read(reader);
			return result;
		}
		public OfficeColorRecord() {
			this.data = 0xffffffff;
		}
		public OfficeColorRecord(Color color) {
			Color = color;
		}
		public OfficeColorRecord(int colorIndex) {
			ColorSchemeIndex = (byte)colorIndex;
		}
		public OfficeColorRecord(OfficeColorUse colorUse, OfficeColorTransform transform, byte transformValue) {
			if(colorUse == OfficeColorUse.None)
				throw new ArgumentException("colorUse");
			if(transform == OfficeColorTransform.None)
				throw new ArgumentException("transform");
			this.data = (uint)(maskSystemColor | (uint)transformValue << 16 | (uint)transform << 8 | (uint)colorUse);
		}
		#region Properties
		public bool IsDefault { get { return (this.data & maskDefault) == maskDefault; } }
		public bool SystemColorUsed { get { return !IsDefault && ((this.data & maskSystemColor) == maskSystemColor); } }
		public bool ColorSchemeUsed { get { return !IsDefault && ((this.data & maskColorScheme) == maskColorScheme); } }
		public Color Color {
			get { return DXColor.FromArgb((int)(this.data & 0x0000ff), (int)(this.data & 0x00ff00) >> 8, (int)(this.data & 0x00ff0000) >> 16); }
			set { this.data = (uint)((int)value.R | (int)value.G << 8 | (int)value.B << 16); }
		}
		public byte ColorSchemeIndex {
			get { return (byte)(this.data & maskSchemeIndex); }
			set { this.data = maskColorScheme | value; } 
		}
		public int SystemColorIndex {
			get { return (int)(this.data & maskSystemIndex); }
			set { this.data = (uint)(maskSystemColor | (value & maskSystemIndex)); } 
		}
		public OfficeColorUse ColorUse {
			get {
				if(!SystemColorUsed)
					return OfficeColorUse.None;
				uint value = this.data & maskColorUse;
				if(value < 0x00f0)
					return OfficeColorUse.None;
				return (OfficeColorUse)value;
			}
		}
		public OfficeColorTransform Transform {
			get {
				if(!SystemColorUsed)
					return OfficeColorTransform.None;
				return (OfficeColorTransform)(this.data & maskTransform);
			}
		}
		public byte TransformValue {
			get {
				if(!SystemColorUsed)
					return 0;
				return (byte)((this.data & maskTransformValue) >> 16);
			}
		}
		#endregion
		protected internal void Read(BinaryReader reader) {
			this.data = reader.ReadUInt32();
		}
		public void Write(BinaryWriter writer) {
			writer.Write(this.data);
		}
		public byte[] GetBytes() {
			return BitConverter.GetBytes(this.data);
		}
	}
	#endregion
	#region OfficeArtFileDrawingRecord
	public class OfficeArtFileDrawingRecord : OfficeDrawingPartBase {
		#region static
		public static OfficeArtFileDrawingRecord FromStream(BinaryReader reader, OfficeArtRecordHeader header) {
			OfficeArtFileDrawingRecord result = new OfficeArtFileDrawingRecord(header.InstanceInfo);
			result.Read(reader, header);
			return result;
		}
		#endregion
		#region Fields
		const int recordLength = 0x0008;
		int drawingId;
		int numberOfShapes;
		int lastShapeId;
		#endregion
		public OfficeArtFileDrawingRecord(int drawingId) {
			this.drawingId = drawingId;
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return drawingId; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.FileDrawingRecord; } }
		public override int HeaderVersion { get { return OfficeArtVersions.EmptyHeaderVersion; } }
		public override int Length { get { return recordLength; } }
		public int NumberOfShapes { get { return numberOfShapes; } set { numberOfShapes = value; } }
		public int LastShapeIdentifier { get { return lastShapeId; } set { lastShapeId = value; } }
		#endregion
		protected internal void Read(BinaryReader reader, OfficeArtRecordHeader header) {
			if (header.Length != recordLength)
				OfficeArtExceptions.ThrowInvalidContent();
			this.numberOfShapes = reader.ReadInt32();
			this.lastShapeId = reader.ReadInt32();
		}
		protected internal override void WriteCore(BinaryWriter writer) {
			writer.Write(NumberOfShapes);
			writer.Write(LastShapeIdentifier);
		}
		protected internal override int GetSize() {
			return recordLength;
		}
	}
	#endregion
	#region OfficeArtShapeRecord
	public class OfficeArtShapeRecord : OfficeDrawingPartBase {
		#region static
		public static OfficeArtShapeRecord FromStream(BinaryReader reader) {
			OfficeArtShapeRecord result = new OfficeArtShapeRecord();
			result.Read(reader);
			return result;
		}
		public static OfficeArtShapeRecord FromStream(BinaryReader reader, OfficeArtRecordHeader header) {
			OfficeArtShapeRecord result = new OfficeArtShapeRecord(header.InstanceInfo);
			result.Read(reader);
			return result;
		}
		#endregion
		#region Fields
		const int defaultFlags = 0x0a00;
		const int recordLength = 8;
		int shapeTypeCode;
		int shapeIdentifier;
		int flags;
		#endregion
		OfficeArtShapeRecord() { }
		public OfficeArtShapeRecord(int shapeTypeCode) {
			this.shapeTypeCode = shapeTypeCode;
			this.flags = defaultFlags;
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return shapeTypeCode; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.FileShape; } }
		public override int HeaderVersion { get { return OfficeArtVersions.ShapeRecordVersion; } }
		public override int Length { get { return recordLength; } }
		public int ShapeIdentifier { get { return shapeIdentifier; } set { shapeIdentifier = value; } }
		public int Flags { get { return flags; } set { flags = value; } }
		public bool IsGroup { get { return GetBoolValue(maskGroup); } set { SetBoolValue(value, maskGroup); } }
		public bool IsChild { get { return GetBoolValue(maskChild); } set { SetBoolValue(value, maskChild); } }
		public bool IsPatriarch { get { return GetBoolValue(maskPatriarch); } set { SetBoolValue(value, maskPatriarch); } }
		public bool IsDeleted { get { return GetBoolValue(maskDeleted); } set { SetBoolValue(value, maskDeleted); } }
		public bool HaveMaster { get { return GetBoolValue(maskHaveMaster); } set { SetBoolValue(value, maskHaveMaster); } }
		public bool FlipH { get { return GetBoolValue(maskFlipH); } set { SetBoolValue(value, maskFlipH); } }
		public bool FlipV { get { return GetBoolValue(maskFlipV); } set { SetBoolValue(value, maskFlipV); } }
		public bool IsConnector { get { return GetBoolValue(maskConnector); } set { SetBoolValue(value, maskConnector); } }
		public bool HaveAnchor { get { return GetBoolValue(maskHaveAnchor); } set { SetBoolValue(value, maskHaveAnchor); } }
		public bool IsBackground { get { return GetBoolValue(maskBackground); } set { SetBoolValue(value, maskBackground); } }
		public bool HaveShapeType { get { return GetBoolValue(maskHaveShapeType); } set { SetBoolValue(value, maskHaveShapeType); } }
		#endregion
		protected internal void Read(BinaryReader reader) {
			this.shapeIdentifier = reader.ReadInt32();
			this.flags = reader.ReadInt32();
		}
		protected internal override void WriteCore(BinaryWriter writer) {
			writer.Write(ShapeIdentifier);
			writer.Write(Flags);
		}
		protected internal override int GetSize() {
			return recordLength;
		}
		#region Internals
		#region Masks
		const int maskGroup = 0x0001;
		const int maskChild = 0x0002;
		const int maskPatriarch = 0x0004;
		const int maskDeleted = 0x0008;
		const int maskOleShape = 0x0010;
		const int maskHaveMaster = 0x0020;
		const int maskFlipH = 0x0040;
		const int maskFlipV = 0x0080;
		const int maskConnector = 0x0100;
		const int maskHaveAnchor = 0x0200;
		const int maskBackground = 0x0400;
		const int maskHaveShapeType = 0x0800;
		#endregion
		bool GetBoolValue(int mask) {
			return (flags & mask) != 0;
		}
		void SetBoolValue(bool value, int mask) {
			if(value)
				flags |= mask;
			else
				flags &= ~mask;
		}
		#endregion
	}
	#endregion
	#region OfficeArtShapeGroupCoordinateSystem
	public class OfficeArtShapeGroupCoordinateSystem : OfficeDrawingPartBase {
		#region Fields
		const int recordLength = 0x10;
		int left;
		int top;
		int right;
		int bottom;
		#endregion
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.ShapeGroupCoordinateSystem; } }
		public override int HeaderVersion { get { return OfficeArtVersions.ShapeGroupCoordinateSystemVersion; } }
		public int Left { get { return left; } set { left = value; } }
		public int Top { get { return top; } set { top = value; } }
		public int Right { get { return right; } set { right = value; } }
		public int Bottom { get { return bottom; } set { bottom = value; } }
		#endregion
		protected internal override void WriteCore(BinaryWriter writer) {
			writer.Write(Left);
			writer.Write(Top);
			writer.Write(Right);
			writer.Write(Bottom);
		}
		protected internal override int GetSize() {
			return recordLength;
		}
	}
	#endregion
	#region OfficeArtPropertiesBase (abstract class)
	public abstract class OfficeArtPropertiesBase : OfficeDrawingPartBase {
		readonly List<IOfficeDrawingProperty> properties;
		protected OfficeArtPropertiesBase() {
			this.properties = new List<IOfficeDrawingProperty>();
		}
		public List<IOfficeDrawingProperty> Properties { get { return properties; } }
		public override int HeaderInstanceInfo { get { return Properties.Count; } }
		public override int HeaderVersion { get { return OfficeArtVersions.PropertiesVersion; } }
		protected internal void Read(BinaryReader reader, OfficeArtRecordHeader header) {
			long start = reader.BaseStream.Position;
			int count = header.InstanceInfo;
			for (int i = 0; i < count; i++)
				Properties.Add(OfficePropertiesFactory.CreateProperty(reader));
			for (int i = 0; i < count; i++) {
				IOfficeDrawingProperty property = Properties[i];
				if (property.Complex) {
					OfficeDrawingIntPropertyBase intProperty = property as OfficeDrawingIntPropertyBase;
					if(intProperty != null)
						intProperty.SetComplexData(reader.ReadBytes(intProperty.Value));
					else {
						OfficeDrawingMsoArrayPropertyBase msoArrayProperty = property as OfficeDrawingMsoArrayPropertyBase;
						if(msoArrayProperty != null) {
							if(msoArrayProperty.Value > 6) {
								byte[] msoArrayHeader = reader.ReadBytes(6);
								if(BitConverter.ToUInt16(msoArrayHeader, 4) == 0xFFF0) {
									msoArrayProperty.Value += 6;
								}
								byte[] msoArrayData = reader.ReadBytes(msoArrayProperty.Value - 6);
								byte[] complexData = new byte[msoArrayProperty.Value];
								Array.Copy(msoArrayHeader, complexData, 6);
								Array.Copy(msoArrayData, 0, complexData, 6, msoArrayData.Length);
								msoArrayProperty.SetComplexData(complexData);
							}
							else {
								msoArrayProperty.SetComplexData(reader.ReadBytes(msoArrayProperty.Value));
							}
						}
					}
				}
				property.Execute(this);
			}
			reader.BaseStream.Seek(start + header.Length, SeekOrigin.Begin);
		}
		protected internal override void WriteCore(BinaryWriter writer) {
			List<byte[]> complexData = new List<byte[]>();
			int count = Properties.Count;
			for (int i = 0; i < count; i++) {
				IOfficeDrawingProperty current = Properties[i];
				if (current.Complex)
					complexData.Add(current.ComplexData);
				current.Write(writer);
			}
			count = complexData.Count;
			for (int i = 0; i < count; i++) {
				writer.Write(complexData[i]);
			}
		}
		protected internal override int GetSize() {
			int count = this.properties.Count;
			int totalSize = 0;
			for (int i = 0; i < count; i++) {
				totalSize += properties[i].Size;
			}
			return totalSize;
		}
		public abstract void CreateProperties();
	}
	#endregion
	#region IOfficeArtPropertiesBase
	public interface IOfficeArtPropertiesBase {
		bool IsBehindDoc { get; set; }
		bool UseIsBehindDoc { get; set; }
		bool Filled { get; set; }
		bool UseFilled { get; set; }
		bool LayoutInCell { get; set; }
		bool UseLayoutInCell { get; set; }
	}
	#endregion
	#region IOfficeArtProperties
	public interface IOfficeArtProperties : IOfficeArtPropertiesBase {
		int BlipIndex { get; set; }
		int TextIndex { get; set; }
		int ZOrder { get; set; }
		bool UseTextTop { get; set; }
		bool UseTextBottom { get; set; }
		bool UseTextRight { get; set; }
		bool UseTextLeft { get; set; }
		bool UseFitShapeToText { get; set; }
		bool FitShapeToText { get; set; }
		int TextTop { get; set; }
		int TextBottom { get; set; }
		int TextRight { get; set; }
		int TextLeft { get; set; }
		int WrapLeftDistance { get; set; }
		bool UseWrapLeftDistance { get; set; }
		int WrapRightDistance { get; set; }
		bool UseWrapRightDistance { get; set; }
		int WrapTopDistance { get; set; }
		bool UseWrapTopDistance { get; set; }
		int WrapBottomDistance { get; set; }
		bool UseWrapBottomDistance { get; set; }
		double CropFromTop { get; set; }
		double CropFromBottom { get; set; }
		double CropFromLeft { get; set; }
		double CropFromRight { get; set; }
		double Rotation { get; set; }	   
		bool Line { get; set; }
		bool UseLine { get; set; }
		double LineWidth { get; set; }
		Color LineColor { get; set; }
		Color FillColor { get; set; }
	}
	#endregion
	#region IOfficeArtTertiaryProperties
	public interface IOfficeArtTertiaryProperties : IOfficeArtPropertiesBase {
		bool UseRelativeWidth { get; set; }
		bool UseRelativeHeight { get; set; }
		bool UsePosH { get; set; }
		bool UsePosV { get; set; }
		int PctHoriz { get; set; }
		int PctVert { get; set; }
		int PctHorizPos { get; set; }
		int PctVertPos { get; set; }
		DrawingGroupShapePosH.Msoph PosH { get; set; }
		DrawingGroupShapePosV.Msopv PosV { get; set; }
		DrawingGroupShapePosRelH.Msoprh PosRelH { get; set; }
		DrawingGroupShapePosRelV.Msoprv PosRelV { get; set; }
		DrawingGroupShape2SizeRelH.RelativeFrom SizeRelH { get; set; }
		DrawingGroupShape2SizeRelV.RelativeFrom SizeRelV { get; set; }
	}
	#endregion
	#region OfficeArtTertiaryProperties
	public class OfficeArtTertiaryProperties : OfficeArtPropertiesBase, IOfficeArtTertiaryProperties {
		const int defaultFlags = 0x0000;
		#region static
		public static OfficeArtTertiaryProperties FromStream(BinaryReader reader, OfficeArtRecordHeader header) {
			OfficeArtTertiaryProperties result = new OfficeArtTertiaryProperties();
			result.Read(reader, header);
			return result;
		}
		#endregion
		public OfficeArtTertiaryProperties() {
			PosRelH = DrawingGroupShapePosRelH.Msoprh.msoprhText;
			PosRelV = DrawingGroupShapePosRelV.Msoprv.msoprvText;
		}
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.TertiaryPropertiesTable; } }
		public bool IsBehindDoc { get; set; }
		public bool UseIsBehindDoc { get; set; }
		public bool UseRelativeWidth { get; set; }
		public bool UseRelativeHeight { get; set; }
		public bool UsePosH { get; set; }
		public bool UsePosV { get; set; }		
		public bool Filled { get; set; }
		public bool UseFilled { get; set; }
		public bool LayoutInCell { get; set; }
		public bool UseLayoutInCell { get; set; }
		public int PctHoriz { get; set; }
		public int PctVert { get; set; }
		public int PctHorizPos { get; set; }
		public int PctVertPos { get; set; }
		public DrawingGroupShape2SizeRelH.RelativeFrom SizeRelH { get; set; }
		public DrawingGroupShape2SizeRelV.RelativeFrom SizeRelV { get; set; }
		public DrawingGroupShapePosH.Msoph PosH { get; set; }
		public DrawingGroupShapePosV.Msopv PosV { get; set; }
		public DrawingGroupShapePosRelH.Msoprh PosRelH { get; set; }
		public DrawingGroupShapePosRelV.Msoprv PosRelV { get; set; }
		public bool PctHorizPosValid { get { return PctHorizPos != (unchecked((int)0xFFFFD8EF)); } }
		public bool PctVertPosValid { get { return PctVertPos != (unchecked((int)0xFFFFD8EF)); } }
		public override void CreateProperties() {
			SetGroupShapePosHProperties();
			SetGroupShapePosRelHProperties();
			SetGroupShapePosVProperties();
			SetGroupShapePosRelVProperties();
			SetGroupShape2PctHorizProperties();
			SetGroupShape2PctVertProperties();
			SetGroupShape2SizeRelHProperties();
			SetGroupShape2SizeRelVProperties();
			SetGroupShapePctPosProperties();
			SetGroupShapeBooleanProperties();
			SetDiagramBooleanProperties();
		}
		void SetGroupShapePctPosProperties() {
			if (PctHorizPos != 0 || PctVertPos != 0) {
				if (PctHorizPos != 0)
					Properties.Add(new DrawingGroupShape2PctHorizPos(PctHorizPos));
				else
					Properties.Add(new DrawingGroupShape2PctHorizPos());
				if (PctVertPos != 0)
					Properties.Add(new DrawingGroupShape2PctVertPos(PctVertPos));
				else
					Properties.Add(new DrawingGroupShape2PctVertPos());
			}
		}
		void SetGroupShape2PctHorizProperties() {
			if (UseRelativeWidth)
				Properties.Add(new DrawingGroupShape2PctHoriz(PctHoriz));
		}
		void SetGroupShape2PctVertProperties() {
			if (UseRelativeHeight)
				Properties.Add(new DrawingGroupShape2PctVert(PctVert));
		}
		void SetGroupShape2SizeRelHProperties() {
			if (UseRelativeWidth)
				Properties.Add(new DrawingGroupShape2SizeRelH(SizeRelH));
		}
		void SetGroupShape2SizeRelVProperties() {
			if (UseRelativeHeight)
				Properties.Add(new DrawingGroupShape2SizeRelV(SizeRelV));
		}
		void SetGroupShapeBooleanProperties() {
			DrawingGroupShapeBooleanProperties properties = new DrawingGroupShapeBooleanProperties();
			properties.Value = defaultFlags;
			Properties.Add(properties);
		}
		void SetGroupShapePosHProperties() {
			if(UsePosH)
				Properties.Add(new DrawingGroupShapePosH(PosH));
		}
		void SetGroupShapePosRelHProperties() {
			if (UsePosH)
				Properties.Add(new DrawingGroupShapePosRelH(PosRelH));
		}
		void SetGroupShapePosVProperties() {
			if (UsePosV)
				Properties.Add(new DrawingGroupShapePosV(PosV));
		}
		void SetGroupShapePosRelVProperties() {
			if (UsePosV)
				Properties.Add(new DrawingGroupShapePosRelV(PosRelV));
		}
		void SetDiagramBooleanProperties() {
			Properties.Add(new DiagramBooleanProperties());
		}
	}
	#endregion
	#region IOfficeDrawingProperty
	public interface IOfficeDrawingProperty {
		int Size { get; }
		bool Complex { get; }
		byte[] ComplexData { get; }
		void Read(BinaryReader reader);
		void Execute(OfficeArtPropertiesBase owner);
		void Write(BinaryWriter writer);
		void Merge(IOfficeDrawingProperty other);
	}
	#endregion
	#region OfficeDrawingPropertyInfo
	public class OfficeDrawingPropertyInfo {
		#region Fields
		short identifier;
		Type type;
		#endregion
		public OfficeDrawingPropertyInfo(short identifier, Type type) {
			this.identifier = identifier;
			this.type = type;
		}
		public short Identifier { get { return identifier; } }
		public Type Type { get { return type; } }
	}
	#endregion
	#region OfficePropertiesFactory
	public static class OfficePropertiesFactory {
		static readonly ConstructorInfo Empty = BlipFactory.GetConstructor(typeof(DrawingEmpty), new Type[] { });
		static List<OfficeDrawingPropertyInfo> infos;
		static Dictionary<short, ConstructorInfo> propertyTypes;
		static Dictionary<Type, short> propertyIdentifiers;
		static OfficePropertiesFactory() {
			infos = new List<OfficeDrawingPropertyInfo>();
			propertyIdentifiers = new Dictionary<Type, short>();
			propertyTypes = new Dictionary<short, ConstructorInfo>();
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x007f), typeof(DrawingBooleanProtectionProperties)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0100), typeof(DrawingCropFromTop)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0101), typeof(DrawingCropFromBottom)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0102), typeof(DrawingCropFromLeft)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0103), typeof(DrawingCropFromRight)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0xc105), typeof(DrawingBlipName)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0106), typeof(DrawingBlipFlags)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x013f), typeof(DrawingBlipBooleanProperties)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01bf), typeof(DrawingFillStyleBooleanProperties)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01cb), typeof(DrawingLineWidth)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01cc), typeof(DrawingLineMiterLimit)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01cd), typeof(DrawingLineCompoundType)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01ce), typeof(DrawingLineDashing)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01d6), typeof(DrawingLineJoinStyle)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01d7), typeof(DrawingLineCapStyle)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01ff), typeof(DrawingLineStyleBooleanProperties)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0304), typeof(DrawingBlackWhiteMode)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x033f), typeof(DrawingShapeBooleanProperties)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x4104), typeof(DrawingBlipIdentifier)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0080), typeof(DrawingTextIdentifier)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0xc380), typeof(DrawingShapeName)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0xc381), typeof(DrawingShapeDescription)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0xc382), typeof(DrawingShapeHyperlink)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0xc38d), typeof(DrawingShapeTooltip)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x03bf), typeof(DrawingGroupShapeBooleanProperties)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x053f), typeof(DiagramBooleanProperties)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x038F), typeof(DrawingGroupShapePosH)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0390), typeof(DrawingGroupShapePosRelH)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0391), typeof(DrawingGroupShapePosV)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0392), typeof(DrawingGroupShapePosRelV)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x07c0), typeof(DrawingGroupShape2PctHoriz)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x07c1), typeof(DrawingGroupShape2PctVert)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x07c2), typeof(DrawingGroupShape2PctHorizPos)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x07c3), typeof(DrawingGroupShape2PctVertPos)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x07c4), typeof(DrawingGroupShape2SizeRelH)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x07c5), typeof(DrawingGroupShape2SizeRelV)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0004), typeof(DrawingRotation)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01c0), typeof(DrawingLineColor)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0081), typeof(DrawingTextLeft)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0082), typeof(DrawingTextTop)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0083), typeof(DrawingTextRight)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0084), typeof(DrawingTextBottom)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0384), typeof(DrawingWrapLeftDistance)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0385), typeof(DrawingWrapTopDistance)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0386), typeof(DrawingWrapRightDistance)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0387), typeof(DrawingWrapBottomDistance)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x4186), typeof(DrawingFillBlipIdentifier)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0201), typeof(DrawingShadowColor)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x023f), typeof(DrawingShadowStyleBooleanProperties)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0158), typeof(DrawingConnectionPointsType)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x00bf), typeof(DrawingTextBooleanProperties)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x008b), typeof(DrawingTextDirection)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0180), typeof(OfficeDrawingFillType)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0181), typeof(DrawingFillColor)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0182), typeof(DrawingFillOpacity)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0183), typeof(DrawingFillBackColor)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0184), typeof(DrawingFillBackOpacity)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0185), typeof(DrawingFillBWColor)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0xc186), typeof(DrawingFillBlip)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0xc187), typeof(DrawingFillBlipName)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0188), typeof(DrawingFillBlipFlags)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0189), typeof(DrawingFillWidth)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x018a), typeof(DrawingFillHeight)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x018b), typeof(DrawingFillAngle)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x018c), typeof(DrawingFillFocus)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x018d), typeof(DrawingFillToLeft)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x018e), typeof(DrawingFillToTop)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x018f), typeof(DrawingFillToRight)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0190), typeof(DrawingFillToBottom)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0191), typeof(DrawingFillRectLeft)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0192), typeof(DrawingFillRectTop)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0193), typeof(DrawingFillRectRight)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0194), typeof(DrawingFillRectBottom)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0195), typeof(DrawingFillDzType)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0196), typeof(DrawingFillShadePreset)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0xC197), typeof(DrawingFillShadeColors)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0198), typeof(DrawingFillOriginX)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x0199), typeof(DrawingFillOriginY)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x019a), typeof(DrawingFillShapeOriginX)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x019b), typeof(DrawingFillShapeOriginY)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x019c), typeof(DrawingFillShadeType)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x019e), typeof(DrawingFillColorExt)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x019f), typeof(DrawingFillReserved415)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01a0), typeof(DrawingFillTintShade)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0xc1a1), typeof(DrawingFillReserved417)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01a2), typeof(DrawingFillBackColorExt)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01a3), typeof(DrawingFillReserved419)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01a4), typeof(DrawingFillBackTintShade)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0xc1a5), typeof(DrawingFillReserved421)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01a6), typeof(DrawingFillReserved422)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short)0x01a7), typeof(DrawingFillReserved423)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0xC145), typeof(DrawingGeometryVertices)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0xC151), typeof(DrawingGeometryConnectionSites)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x0140), typeof(DrawingGeometryLeft)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x0141), typeof(DrawingGeometryTop)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x0142), typeof(DrawingGeometryRight)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x0143), typeof(DrawingGeometryBottom)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x0144), typeof(DrawingGeometryShapePath)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0xC146), typeof(DrawingGeometrySegmentInfo)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x0147), typeof(DrawingGeometryAdjustValue1)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x0148), typeof(DrawingGeometryAdjustValue2)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x0149), typeof(DrawingGeometryAdjustValue3)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x014A), typeof(DrawingGeometryAdjustValue4)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x014B), typeof(DrawingGeometryAdjustValue5)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x014C), typeof(DrawingGeometryAdjustValue6)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x014D), typeof(DrawingGeometryAdjustValue7)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x014E), typeof(DrawingGeometryAdjustValue8)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0xC152), typeof(DrawingGeometryConnectionSitesDir)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x017F), typeof(DrawingGeometryBooleanProperties)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0xC3A9), typeof(DrawingMetroBlob)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x0085), typeof(DrawingShapeWrapText)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x0087), typeof(DrawingShapeAnchorText)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x0088), typeof(DrawingShapeTextFlow)));
			infos.Add(new OfficeDrawingPropertyInfo(unchecked((short) 0x0089), typeof(DrawingShapeFontDirection)));
			int count = infos.Count;
			for (int i = 0; i < count; i++) {
				propertyTypes.Add(infos[i].Identifier, BlipFactory.GetConstructor(infos[i].Type, new Type[] { }));
				propertyIdentifiers.Add(infos[i].Type, infos[i].Identifier);
			}
		}
		public static IOfficeDrawingProperty CreateProperty(BinaryReader reader) {
			ushort bitwiseField = reader.ReadUInt16();
			short opcode = (short)bitwiseField;
			ConstructorInfo propertyConstructor;
			if(!propertyTypes.TryGetValue(opcode, out propertyConstructor)) {
				if(!propertyTypes.TryGetValue((short) (opcode | 0x4000), out propertyConstructor))
					propertyConstructor = Empty;
			}
			OfficeDrawingPropertyBase property = propertyConstructor.Invoke(new object[] { }) as OfficeDrawingPropertyBase;
			bool complex = (bitwiseField & 0x8000) != 0;
			property.SetComplex(complex);
			property.Read(reader);
			return property;
		}
		public static short GetOpcodeByType(Type propertyType) {
			short opcode;
			if (!propertyIdentifiers.TryGetValue(propertyType, out opcode))
				opcode = 0x0000;
			return opcode;
		}
	}
	#endregion
	#region OfficeDrawingPropertyBase (abstract class)
	public abstract class OfficeDrawingPropertyBase : IOfficeDrawingProperty {
		const int operationCodeSize = 2;
		const int operandSize = 4;
		bool complex;
		byte[] complexData;
		protected OfficeDrawingPropertyBase() {
			this.complexData = new byte[] { };
		}
		#region IOfficeDrawingProperty Members
		public virtual bool Complex { get { return complex; } }
		public byte[] ComplexData { get { return complexData; } }
		public int Size {
			get {
				return Complex ? operationCodeSize + operandSize + ComplexData.Length : operationCodeSize + operandSize;
			}
		}
		public abstract void Read(BinaryReader reader);
		public abstract void Execute(OfficeArtPropertiesBase owner);
		public abstract void Write(BinaryWriter writer);
		public virtual void Merge(IOfficeDrawingProperty other) {
		}
		#endregion
		protected internal void SetComplexData(byte[] data) {
			this.complexData = data;
		}
		protected internal void SetComplex(bool complex) {
			this.complex = complex;
		}
	}
	#endregion
	#region OfficeDrawingIntPropertyBase
	public abstract class OfficeDrawingIntPropertyBase : OfficeDrawingPropertyBase {
		int value;
		public int Value { get { return this.value; } set { this.value = value; } }
		public override void Read(BinaryReader reader) {
			this.value = reader.ReadInt32();
		}
		public override void Execute(OfficeArtPropertiesBase owner) { }
		public override void Write(BinaryWriter writer) {
			writer.Write(OfficePropertiesFactory.GetOpcodeByType(GetType()));
			writer.Write(Value);
		}
		protected bool GetFlag(int mask) {
			return (Value & mask) == mask;
		}
		protected void SetFlag(int mask, bool flag) {
			if(flag)
				Value |= mask;
			else
				Value &= ~mask;
		}
	}
	#endregion
	#region OfficeDrawingStringPropertyValueBase
	public abstract class OfficeDrawingStringPropertyValueBase : OfficeDrawingIntPropertyBase {
		string data;
		public override bool Complex { get { return true; } }
		public string Data { get { return GetStringData(); } set { SetStringData(value); } }
		string GetStringData() {
			if (String.IsNullOrEmpty(this.data))
				this.data = Encoding.Unicode.GetString(ComplexData, 0, ComplexData.Length);
			return this.data;
		}
		void SetStringData(string value) {
			if (this.data == value)
				return;
			this.data = value;
			SetComplexData(Encoding.Unicode.GetBytes(value));
			Value = ComplexData.Length;
		}
	}
	#endregion
	#region OfficeDrawingFixedPointPropertyBase
	public class OfficeDrawingFixedPointPropertyBase : OfficeDrawingPropertyBase {
		double value;
		public double Value { get { return this.value; } set { this.value = value; } }
		public override bool Complex { get { return true; } } 
		public override void Read(BinaryReader reader) {
			this.value = FixedPoint.FromStream(reader).Value;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(OfficePropertiesFactory.GetOpcodeByType(GetType()));
			FixedPoint fixedPoint = new FixedPoint();
			fixedPoint.Value = Value;
			fixedPoint.Write(writer);
		}
	}
	#endregion
	#region OfficeDrawingColorPropertyBase
	public abstract class OfficeDrawingColorPropertyBase : OfficeDrawingPropertyBase {
		#region Fields
		OfficeColorRecord colorRecord;
		#endregion
		protected OfficeDrawingColorPropertyBase() {
			this.colorRecord = new OfficeColorRecord(DXColor.Empty);
		}
		#region Properties
		public OfficeColorRecord ColorRecord { get { return colorRecord; }
			set { colorRecord = value; } }
		#endregion
		public override void Read(BinaryReader reader) {
			ColorRecord = OfficeColorRecord.FromStream(reader);
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(OfficePropertiesFactory.GetOpcodeByType(GetType()));
			ColorRecord.Write(writer);
		}
	}
	#endregion
	#region OfficeDrawingBooleanPropertyBase
	public abstract class OfficeDrawingBooleanPropertyBase : OfficeDrawingIntPropertyBase {
		const uint useMask = 0xffff0000;
		public override void Merge(IOfficeDrawingProperty other) {
			OfficeDrawingBooleanPropertyBase otherProp = other as OfficeDrawingBooleanPropertyBase;
			if(otherProp != null) {
				uint otherValue = (uint)otherProp.Value;
				uint thisValue = (uint)this.Value;
				otherValue &= ~(thisValue & useMask);
				uint otherValueMask = (otherValue & useMask) >> 16;
				otherValue &= useMask | otherValueMask;
				thisValue &= ~otherValueMask;
				thisValue |= otherValue;
				this.Value = (int)thisValue;
			}
		}
	}
	#endregion
	#region DrawingEmpty
	public class DrawingEmpty : OfficeDrawingIntPropertyBase {
	}
	#endregion
	#region DrawingBooleanProtectionProperties
	public class DrawingBooleanProtectionProperties : OfficeDrawingBooleanPropertyBase {
		const int lockGroup = 0x0001;
		const int lockAdjustHandles = 0x0002;
		const int lockText = 0x0004;
		const int lockVertices = 0x0008;
		const int lockCropping = 0x0010;
		const int lockSelect = 0x0020;
		const int lockPosition = 0x0040;
		const int lockAspectRatio = 0x0080;
		const int lockRotation = 0x0100;
		const int lockUngroup = 0x0200;
		const int useLockGroup = 0x00010000;
		const int useLockAdjustHandles = 0x00020000;
		const int useLockText = 0x00040000;
		const int useLockVertices = 0x00080000;
		const int useLockCropping = 0x00100000;
		const int useLockSelect = 0x00200000;
		const int useLockPosition = 0x00400000;
		const int useLockAspectRatio = 0x00800000;
		const int useLockRotation = 0x01000000;
		const int useLockUngroup = 0x02000000;
		const int defaultFlags = 0x00e10080;
		public DrawingBooleanProtectionProperties() {
			Value = defaultFlags;
		}
		#region Properties
		public override bool Complex { get { return false; } }
		public bool LockGroup { get { return GetFlag(lockGroup); } set { SetFlag(lockGroup, value); } }
		public bool LockAdjustHandles { get { return GetFlag(lockAdjustHandles); } set { SetFlag(lockAdjustHandles, value); } }
		public bool LockText { get { return GetFlag(lockText); } set { SetFlag(lockText, value); } }
		public bool LockVertices { get { return GetFlag(lockVertices); } set { SetFlag(lockVertices, value); } }
		public bool LockCropping { get { return GetFlag(lockCropping); } set { SetFlag(lockCropping, value); } }
		public bool LockSelect { get { return GetFlag(lockSelect); } set { SetFlag(lockSelect, value); } }
		public bool LockPosition { get { return GetFlag(lockPosition); } set { SetFlag(lockPosition, value); } }
		public bool LockAspectRatio { get { return GetFlag(lockAspectRatio); } set { SetFlag(lockAspectRatio, value); } }
		public bool LockRotation { get { return GetFlag(lockRotation); } set { SetFlag(lockRotation, value); } }
		public bool LockUngroup { get { return GetFlag(lockUngroup); } set { SetFlag(lockUngroup, value); } }
		public bool UseLockGroup { get { return GetFlag(useLockGroup); } set { SetFlag(useLockGroup, value); } }
		public bool UseLockAdjustHandles { get { return GetFlag(useLockAdjustHandles); } set { SetFlag(useLockAdjustHandles, value); } }
		public bool UseLockText { get { return GetFlag(useLockText); } set { SetFlag(useLockText, value); } }
		public bool UseLockVertices { get { return GetFlag(useLockVertices); } set { SetFlag(useLockVertices, value); } }
		public bool UseLockCropping { get { return GetFlag(useLockCropping); } set { SetFlag(useLockCropping, value); } }
		public bool UseLockSelect { get { return GetFlag(useLockSelect); } set { SetFlag(useLockSelect, value); } }
		public bool UseLockPosition { get { return GetFlag(useLockPosition); } set { SetFlag(useLockPosition, value); } }
		public bool UseLockAspectRatio { get { return GetFlag(useLockAspectRatio); } set { SetFlag(useLockAspectRatio, value); } }
		public bool UseLockRotation { get { return GetFlag(useLockRotation); } set { SetFlag(useLockRotation, value); } }
		public bool UseLockUngroup { get { return GetFlag(useLockUngroup); } set { SetFlag(useLockUngroup, value); } }
		#endregion
	}
	#endregion
	#region DrawingBlipBooleanProperties
	public class DrawingBlipBooleanProperties : OfficeDrawingBooleanPropertyBase {
		const int defaultFlags = 0x00060000;
		public override bool Complex { get { return false; } }
		public DrawingBlipBooleanProperties() {
			Value = defaultFlags;
		}
	}
	#endregion
	#region DrawingFillStyleBooleanProperties
	public class DrawingFillStyleBooleanProperties : OfficeDrawingBooleanPropertyBase {
		[Flags]
		public enum FillStyle {
			NoFillHitTest = 0x01,
			FillUseRect = 0x02,
			FillShape = 0x04,
			HitTestFill = 0x08,
			Filled = 0x10,
			ShapeAnchor = 0x20,
			RecolorFillAsPicture = 0x40,
			UseNoFillHitTest = 0x010000,
			UseFillUseRect = 0x020000,
			UseFillShape = 0x040000,
			UseHitTestFill = 0x080000,
			UseFilled = 0x100000,
			UseShapeAnchor = 0x200000,
			UseRecolorFillAsPicture = 0x400000
		}
		FillStyle fillStyle;
		#region NoFillHitTest
		public bool NoFillHitTest {
			get { return (fillStyle & FillStyle.NoFillHitTest) == FillStyle.NoFillHitTest; }
			set {
				if(value)
					fillStyle |= FillStyle.NoFillHitTest;
				else
					fillStyle &= ~FillStyle.NoFillHitTest;
			}
		}
		public bool UseNoFillHitTest {
			get { return (fillStyle & FillStyle.UseNoFillHitTest) == FillStyle.UseNoFillHitTest; }
			set {
				if(value)
					fillStyle |= FillStyle.UseNoFillHitTest;
				else
					fillStyle &= ~FillStyle.UseNoFillHitTest;
			}
		}
		#endregion
		#region FillUseRect
		public bool FillUseRect {
			get { return (fillStyle & FillStyle.FillUseRect) == FillStyle.FillUseRect; }
			set {
				if(value)
					fillStyle |= FillStyle.FillUseRect;
				else
					fillStyle &= ~FillStyle.FillUseRect;
			}
		}
		public bool UseFillUseRect {
			get { return (fillStyle & FillStyle.UseFillUseRect) == FillStyle.UseFillUseRect; }
			set {
				if(value)
					fillStyle |= FillStyle.UseFillUseRect;
				else
					fillStyle &= ~FillStyle.UseFillUseRect;
			}
		}
		#endregion
		#region FillShape
		public bool FillShape {
			get { return (fillStyle & FillStyle.FillShape) == FillStyle.FillShape; }
			set {
				if(value)
					fillStyle |= FillStyle.FillShape;
				else
					fillStyle &= ~FillStyle.FillShape;
			}
		}
		public bool UseFillShape {
			get { return (fillStyle & FillStyle.UseFillShape) == FillStyle.UseFillShape; }
			set {
				if(value)
					fillStyle |= FillStyle.UseFillShape;
				else
					fillStyle &= ~FillStyle.UseFillShape;
			}
		}
		#endregion
		#region HitTestFill
		public bool HitTestFill {
			get { return (fillStyle & FillStyle.HitTestFill) == FillStyle.HitTestFill; }
			set {
				if(value)
					fillStyle |= FillStyle.HitTestFill;
				else
					fillStyle &= ~FillStyle.HitTestFill;
			}
		}
		public bool UseHitTestFill {
			get { return (fillStyle & FillStyle.UseHitTestFill) == FillStyle.UseHitTestFill; }
			set {
				if(value)
					fillStyle |= FillStyle.UseHitTestFill;
				else
					fillStyle &= ~FillStyle.UseHitTestFill;
			}
		}
		#endregion
		#region Filled
		public bool Filled {
			get { return (fillStyle & FillStyle.Filled) == FillStyle.Filled; }
			set {
				if (value)
					fillStyle |= FillStyle.Filled;
				else
					fillStyle &= ~FillStyle.Filled;
			}
		}
		public bool UseFilled {
			get { return (fillStyle & FillStyle.UseFilled) == FillStyle.UseFilled; }
			set {
				if (value)
					fillStyle |= FillStyle.UseFilled;
				else
					fillStyle &= ~FillStyle.UseFilled;
			}
		}
		#endregion
		#region ShapeAnchor
		public bool ShapeAnchor {
			get { return (fillStyle & FillStyle.ShapeAnchor) == FillStyle.ShapeAnchor; }
			set {
				if(value)
					fillStyle |= FillStyle.ShapeAnchor;
				else
					fillStyle &= ~FillStyle.ShapeAnchor;
			}
		}
		public bool UseShapeAnchor {
			get { return (fillStyle & FillStyle.UseShapeAnchor) == FillStyle.UseShapeAnchor; }
			set {
				if(value)
					fillStyle |= FillStyle.UseShapeAnchor;
				else
					fillStyle &= ~FillStyle.UseShapeAnchor;
			}
		}
		#endregion
		#region RecolorFillAsPicture
		public bool RecolorFillAsPicture {
			get { return (fillStyle & FillStyle.RecolorFillAsPicture) == FillStyle.RecolorFillAsPicture; }
			set {
				if(value)
					fillStyle |= FillStyle.RecolorFillAsPicture;
				else
					fillStyle &= ~FillStyle.RecolorFillAsPicture;
			}
		}
		public bool UseRecolorFillAsPicture {
			get { return (fillStyle & FillStyle.UseRecolorFillAsPicture) == FillStyle.UseRecolorFillAsPicture; }
			set {
				if(value)
					fillStyle |= FillStyle.UseRecolorFillAsPicture;
				else
					fillStyle &= ~FillStyle.UseRecolorFillAsPicture;
			}
		}
		#endregion
		public override bool Complex { get { return false; } }
		public DrawingFillStyleBooleanProperties() {
			fillStyle = FillStyle.Filled & FillStyle.UseFilled;
		}
		public override void Read(BinaryReader reader) {
			base.Read(reader);
			fillStyle = (FillStyle)Value;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtPropertiesBase artProperties = (IOfficeArtPropertiesBase)owner;
			artProperties.Filled = Filled;
			artProperties.UseFilled = UseFilled;
		}
		public override void Write(BinaryWriter writer) {
			Value = (int)this.fillStyle;
			base.Write(writer);
		}
	}
	#endregion
	#region DrawingLineStyleBooleanProperties
	public class DrawingLineStyleBooleanProperties : OfficeDrawingBooleanPropertyBase {
		[Flags]
		public enum DrawingLineStyle {
			Line = 0x8,
			ArrowheadOK = 0x10,
			UseLine = 0x80000,
			UseArrowheadOK = 0x100000
		}
		DrawingLineStyle lineStyle;
		public override bool Complex { get { return false; } }
		public DrawingLineStyleBooleanProperties() {
			lineStyle = DrawingLineStyle.UseLine;
		}
		public bool Line {
			get { return (lineStyle & DrawingLineStyle.Line) == DrawingLineStyle.Line; }
			set {
				if (value)
					lineStyle |= DrawingLineStyle.Line;
				else
					lineStyle &= ~DrawingLineStyle.Line;
			}
		}
		public bool UseLine {
			get { return (lineStyle & DrawingLineStyle.UseLine) == DrawingLineStyle.UseLine; }
			set {
				if (value)
					lineStyle |= DrawingLineStyle.UseLine;
				else
					lineStyle &= ~DrawingLineStyle.UseLine;
			}
		}
		public override void Read(BinaryReader reader) {
			base.Read(reader);
			lineStyle = (DrawingLineStyle)Value;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties artProperties = owner as IOfficeArtProperties;
			if (artProperties == null)
				return;
			artProperties.Line = Line;
			artProperties.UseLine = UseLine;
		}
		public override void Write(BinaryWriter writer) {
			Value = (int)this.lineStyle;
			base.Write(writer);
		}
	}
	#endregion
	#region DrawingGroupShapeBooleanProperties
	public class DrawingGroupShapeBooleanProperties : OfficeDrawingBooleanPropertyBase {
		const int print = 0x0001;
		const int hidden = 0x0002;
		const int behindDocument = 0x0020;
		const int layoutInCell = 0x00008000;
		const int usePrint = 0x00010000;
		const int useHidden = 0x00020000;
		const int useBehindDocument = 0x00200000;
		const int useLayoutInCell = Int32.MinValue; 
		public override bool Complex { get { return false; } }
		public bool Print { get { return GetFlag(print); } set { SetFlag(print, value); } }
		public bool Hidden { get { return GetFlag(hidden); } set { SetFlag(hidden, value); } }
		public bool IsBehindDoc { get { return GetFlag(behindDocument); } set { SetFlag(behindDocument, value); } }
		public bool LayoutInCell { get { return GetFlag(layoutInCell); } set { SetFlag(layoutInCell, value); } }
		public bool UsePrint { get { return GetFlag(usePrint); } set { SetFlag(usePrint, value); } }
		public bool UseHidden { get { return GetFlag(useHidden); } set { SetFlag(useHidden, value); } }
		public bool UseBehindDocument { get { return GetFlag(useBehindDocument); } set { SetFlag(useBehindDocument, value); } }
		public bool UseLayoutInCell { get { return GetFlag(useLayoutInCell); } set { SetFlag(useLayoutInCell, value); } }
		public DrawingGroupShapeBooleanProperties() {
			Value = useHidden | useBehindDocument;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtPropertiesBase artProperties = (IOfficeArtPropertiesBase)owner;
			artProperties.IsBehindDoc = IsBehindDoc;
			artProperties.UseIsBehindDoc = IsBehindDoc;
			artProperties.LayoutInCell = LayoutInCell;
			artProperties.UseLayoutInCell = UseLayoutInCell;
		}
	}
	#endregion
	#region DrawingGroupShape2PctHoriz
	public class DrawingGroupShape2PctHorizPos : OfficeDrawingIntPropertyBase {
		public DrawingGroupShape2PctHorizPos() {
			Value = (unchecked((int)0xFFFFD8EF));
		}
		public DrawingGroupShape2PctHorizPos(int val) {
			Value = val;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			base.Execute(owner);
			IOfficeArtTertiaryProperties artProperties = owner as IOfficeArtTertiaryProperties;
			if (artProperties == null)
				return;
			artProperties.PctHorizPos = Value;
		}
	}
	#endregion
	#region DrawingGroupShape2PctVert
	public class DrawingGroupShape2PctVertPos : OfficeDrawingIntPropertyBase {
		public DrawingGroupShape2PctVertPos() {
			Value = (unchecked((int)0xFFFFD8EF));
		}
		public DrawingGroupShape2PctVertPos(int val) {
			Value = val;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			base.Execute(owner);
			IOfficeArtTertiaryProperties artProperties = owner as IOfficeArtTertiaryProperties;
			if (artProperties == null)
				return;
			artProperties.PctVertPos = Value;
		}
	}
	#endregion
	#region DrawingGroupShapePosH
	public class DrawingGroupShapePosH : OfficeDrawingIntPropertyBase {
		public enum Msoph {
			msophAbs = 0,
			msophLeft = 1,
			msophCenter = 2,
			msophRight = 3,
			msophInside = 4,
			msophOutside = 5
		}
		public DrawingGroupShapePosH() {
		}
		public DrawingGroupShapePosH(Msoph msoPosH) {
			this.MsoPosH = msoPosH;
		}
		public Msoph MsoPosH { get { return (Msoph)Value; } set { Value = (int)value; } }
		public override void Execute(OfficeArtPropertiesBase owner) {
			base.Execute(owner);
			IOfficeArtTertiaryProperties artProperties = owner as IOfficeArtTertiaryProperties;
			if (artProperties == null)
				return;			
			artProperties.PosH = MsoPosH;
			artProperties.UsePosH = true;
		}
	}
	#endregion
	#region DrawingGroupShapePosRelH
	public class DrawingGroupShapePosRelH : OfficeDrawingIntPropertyBase {
		public enum Msoprh {
			msoprhMargin = 0,
			msoprhPage = 1,
			msoprhText = 2,
			msoprhChar = 3,			
		}
		public DrawingGroupShapePosRelH() : this(Msoprh.msoprhText) {			
		}
		public DrawingGroupShapePosRelH(Msoprh msoPosRelH) {
			this.MsoPosRelH = msoPosRelH;
		}
		public Msoprh MsoPosRelH { get { return (Msoprh)Value; } set { Value = (int)value; } }
		public override void Execute(OfficeArtPropertiesBase owner) {
			base.Execute(owner);
			IOfficeArtTertiaryProperties artProperties = owner as IOfficeArtTertiaryProperties;
			if (artProperties == null)
				return;
			artProperties.PosRelH = MsoPosRelH;
			artProperties.UsePosH = true;
		}
	}
	#endregion
	#region DrawingGroupShapePosRelV
	public class DrawingGroupShapePosRelV : OfficeDrawingIntPropertyBase {
		public enum Msoprv {
			msoprvMargin = 0,
			msoprvPage = 1,
			msoprvText = 2,
			msoprvLine = 3,
		}
		public DrawingGroupShapePosRelV()
			: this(Msoprv.msoprvText) {
		}
		public DrawingGroupShapePosRelV(Msoprv msoPosRelV) {
			this.MsoPosRelV = msoPosRelV;
		}
		public Msoprv MsoPosRelV { get { return (Msoprv)Value; } set { Value = (int)value; } }
		public override void Execute(OfficeArtPropertiesBase owner) {
			base.Execute(owner);
			IOfficeArtTertiaryProperties artProperties = owner as IOfficeArtTertiaryProperties;
			if (artProperties == null)
				return;
			artProperties.PosRelV = MsoPosRelV;
			artProperties.UsePosV = true;
		}
	}
	#endregion
	#region DrawingGroupShapePosV
	public class DrawingGroupShapePosV : OfficeDrawingIntPropertyBase {
		public enum Msopv {
			msopvAbs = 0,
			msopvTop = 1,
			msopvCenter = 2,
			msopvBottom = 3,
			msopvInside = 4,
			msopvOutside = 5
		}
		public DrawingGroupShapePosV() {
		}
		public DrawingGroupShapePosV(Msopv msoPosV) {
			this.MsoPosV = msoPosV;
		}
		public Msopv MsoPosV { get { return (Msopv)Value; } set { Value = (int)value; } }
		public override void Execute(OfficeArtPropertiesBase owner) {
			base.Execute(owner);
			IOfficeArtTertiaryProperties artProperties = owner as IOfficeArtTertiaryProperties;
			if (artProperties == null)
				return;			
			artProperties.PosV = MsoPosV;
			artProperties.UsePosV = true;
		}
	}
	#endregion
	#region DrawingGroupShape2PctHoriz
	public class DrawingGroupShape2PctHoriz : OfficeDrawingIntPropertyBase {
		public DrawingGroupShape2PctHoriz() {
		}
		public DrawingGroupShape2PctHoriz(int val) {
			Value = val;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			base.Execute(owner);
			IOfficeArtTertiaryProperties artProperties = owner as IOfficeArtTertiaryProperties;
			if (artProperties == null)
				return;
			if (!artProperties.UseRelativeWidth)
				artProperties.SizeRelH = DrawingGroupShape2SizeRelH.RelativeFrom.Page;
			artProperties.UseRelativeWidth = true;
			artProperties.PctHoriz = Value;			
		}
	}
	#endregion
	#region DrawingGroupShape2PctVert
	public class DrawingGroupShape2PctVert : OfficeDrawingIntPropertyBase {
		public DrawingGroupShape2PctVert() {
		}
		public DrawingGroupShape2PctVert(int val) {
			Value = val;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			base.Execute(owner);
			IOfficeArtTertiaryProperties artProperties = owner as IOfficeArtTertiaryProperties;
			if (artProperties == null)
				return;
			if (!artProperties.UseRelativeHeight)
				artProperties.SizeRelV = DrawingGroupShape2SizeRelV.RelativeFrom.Page;
			artProperties.UseRelativeHeight = true;
			artProperties.PctVert = Value;
		}
	}
	#endregion
	#region DrawingGroupShape2SizeRelH
	public class DrawingGroupShape2SizeRelH : OfficeDrawingIntPropertyBase {
		public DrawingGroupShape2SizeRelH() {
			From = RelativeFrom.Page;
		}
		public DrawingGroupShape2SizeRelH(RelativeFrom from) {
			From = from;
		}
		public enum RelativeFrom {
			Margin = 0x00,
			Page = 0x01,
			LeftMargin = 0x02,
			RightMargin = 0x03,
			InsideMargin = 0x04,
			OutsideMargin = 0x05,
		}
		public RelativeFrom From { get { return (RelativeFrom)Value; } set { Value = (int)value; } }
		public override void Execute(OfficeArtPropertiesBase owner) {
			base.Execute(owner);
			IOfficeArtTertiaryProperties artProperties = owner as IOfficeArtTertiaryProperties;
			if (artProperties == null)
				return;
			artProperties.UseRelativeWidth = true;
			artProperties.SizeRelH = From;
		}
	}
	#endregion
	#region DrawingGroupShape2SizeRelV
	public class DrawingGroupShape2SizeRelV : OfficeDrawingIntPropertyBase {
		public DrawingGroupShape2SizeRelV() {
		}
		public DrawingGroupShape2SizeRelV(RelativeFrom from) {
			From = from;
		}
		public enum RelativeFrom {
			Margin = 0x00,
			Page = 0x01,
			TopMargin = 0x02,
			BottomMargin = 0x03,
			InsideMargin = 0x04,
			OutsideMargin = 0x05,
		}
		public RelativeFrom From { get { return (RelativeFrom)Value; } set { Value = (int)value; } }
		public override void Execute(OfficeArtPropertiesBase owner) {
			base.Execute(owner);
			IOfficeArtTertiaryProperties artProperties = owner as IOfficeArtTertiaryProperties;
			if (artProperties == null)
				return;
			artProperties.UseRelativeHeight = true;
			artProperties.SizeRelV = From;
		}
	}
	#endregion
	#region DrawingShapeBooleanProperties
	public class DrawingShapeBooleanProperties : OfficeDrawingBooleanPropertyBase {
		const int background = 0x0001;
		const int lockShapeType = 0x0008;
		const int preferRelativeResize = 0x0010;
		const int flipVOverride = 0x0040;
		const int flipHOverride = 0x0080;
		const int useBackground = 0x00010000;
		const int useLockShapeType = 0x00080000;
		const int usePreferRelativeResize = 0x00100000;
		const int useFlipVOverride = 0x00400000;
		const int useFlipHOverride = 0x00800000;
		public DrawingShapeBooleanProperties() {
			Value = background | useBackground;
		}
		public override bool Complex { get { return false; } }
		public bool IsBackground { get { return GetFlag(background); } set { SetFlag(background, value); } }
		public bool LockShapeType { get { return GetFlag(lockShapeType); } set { SetFlag(lockShapeType, value); } }
		public bool PreferRelativeResize { get { return GetFlag(preferRelativeResize); } set { SetFlag(preferRelativeResize, value); } }
		public bool FlipVOverride { get { return GetFlag(flipVOverride); } set { SetFlag(flipVOverride, value); } }
		public bool FlipHOverride { get { return GetFlag(flipHOverride); } set { SetFlag(flipHOverride, value); } }
		public bool UseBackground { get { return GetFlag(useBackground); } set { SetFlag(useBackground, value); } }
		public bool UseLockShapeType { get { return GetFlag(useLockShapeType); } set { SetFlag(useLockShapeType, value); } }
		public bool UsePreferRelativeResize { get { return GetFlag(usePreferRelativeResize); } set { SetFlag(usePreferRelativeResize, value); } }
		public bool UseFlipVOverride { get { return GetFlag(useFlipVOverride); } set { SetFlag(useFlipVOverride, value); } }
		public bool UseFlipHOverride { get { return GetFlag(useFlipHOverride); } set { SetFlag(useFlipHOverride, value); } }
	}
	#endregion
	#region DrawingBlipIdentifier
	public class DrawingBlipIdentifier : OfficeDrawingIntPropertyBase {
		public override bool Complex { get { return false; } }
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties artProperties = owner as IOfficeArtProperties;
			if (artProperties == null)
				return;
			artProperties.BlipIndex = Value;
			artProperties.ZOrder = Value;
		}
	}
	#endregion
	#region DrawingTextIdentifier
	public class DrawingTextIdentifier : OfficeDrawingIntPropertyBase {
		const int coeff = 0x10000;
		public override bool Complex { get { return false; } }
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties artProperties = owner as IOfficeArtProperties;
			if (artProperties == null)
				return;
			int index = Value / coeff;
			artProperties.TextIndex = index;
			artProperties.ZOrder = index;
		}
		public override void Write(BinaryWriter writer) {
			Value *= coeff;
			base.Write(writer);
		}
	}
	#endregion
	#region DrawingLineWidth
	public class DrawingLineWidth : OfficeDrawingIntPropertyBase {
		public override bool Complex { get { return false; } }
		public DrawingLineWidth() {
			Value = OfficeArtConstants.DefaultLineWidthInEmus;
		}
		public DrawingLineWidth(int width) {
			Value = width;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties properties = owner as IOfficeArtProperties;
			if (properties != null)
				properties.LineWidth = Value;
		}
	}
	#endregion
	#region DrawingTextTop
	public class DrawingTextTop : OfficeDrawingIntPropertyBase {
		const int defaultValue = 0x0000B298;
		public override bool Complex { get { return false; } }
		public DrawingTextTop() {
			Value = defaultValue;
		}
		public DrawingTextTop(int value) {
			Value = value;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties properties = owner as IOfficeArtProperties;
			if (properties == null)
				return;
			properties.TextTop = Value;
			properties.UseTextTop = true;
		}
	}
	#endregion
	#region DrawingTextBottom
	public class DrawingTextBottom : OfficeDrawingIntPropertyBase {
		const int defaultValue = 0x0000B298;
		public override bool Complex { get { return false; } }
		public DrawingTextBottom() {
			Value = defaultValue;
		}
		public DrawingTextBottom(int value) {
			Value = value;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties properties = owner as IOfficeArtProperties;
			if (properties == null)
				return;
			properties.TextBottom = Value;
			properties.UseTextBottom = true;
		}
	}
	#endregion
	#region DrawingTextLeft
	public class DrawingTextLeft : OfficeDrawingIntPropertyBase {
		const int defaultValue = 0x00016530;
		public override bool Complex { get { return false; } }
		public DrawingTextLeft() {
			Value = defaultValue;
		}
		public DrawingTextLeft(int value) {
			Value = value;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties properties = owner as IOfficeArtProperties;
			if (properties == null)
				return;
			properties.TextLeft = Value;
			properties.UseTextLeft = true;
		}
	}
	#endregion
	#region DrawingTextRight
	public class DrawingTextRight : OfficeDrawingIntPropertyBase {
		const int defaultValue = 0x00016530;
		public override bool Complex { get { return false; } }
		public DrawingTextRight() {
			Value = defaultValue;
		}
		public DrawingTextRight(int value) {
			Value = value;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties properties = owner as IOfficeArtProperties;
			if (properties == null)
				return;
			properties.TextRight = Value;
			properties.UseTextRight = true;
		}
	}
	#endregion
	#region DrawingWrapTopDistance
	public class DrawingWrapTopDistance : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = 0x0;
		public override bool Complex { get { return false; } }
		public DrawingWrapTopDistance() {
			Value = DefaultValue;
		}
		public DrawingWrapTopDistance(int value) {
			Value = value;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties artProperties = owner as IOfficeArtProperties;
			if (artProperties != null) {
				artProperties.UseWrapTopDistance = true;
				artProperties.WrapTopDistance = Value;
			}
		}
	}
	#endregion
	#region DrawingWrapBottomDistance
	public class DrawingWrapBottomDistance : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = 0x0;
		public override bool Complex { get { return false; } }
		public DrawingWrapBottomDistance() {
			Value = DefaultValue;
		}
		public DrawingWrapBottomDistance(int value) {
			Value = value;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties artProperties = owner as IOfficeArtProperties;
			if (artProperties != null) {
				artProperties.WrapBottomDistance = Value;
				artProperties.UseWrapBottomDistance = true;
			}
		}
	}
	#endregion
	#region DrawingWrapLeftDistance
	public class DrawingWrapLeftDistance : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = 0x0001BE7C;
		public override bool Complex { get { return false; } }
		public DrawingWrapLeftDistance() {
			Value = DefaultValue;
		}
		public DrawingWrapLeftDistance(int value) {
			Value = value;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties artProperties = owner as IOfficeArtProperties;
			if (artProperties != null) {
				artProperties.WrapLeftDistance = Value;
				artProperties.UseWrapLeftDistance = true;
			}
		}
	}
	#endregion
	#region DrawingWrapRightDistance
	public class DrawingWrapRightDistance : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = 0x0001BE7C;
		public override bool Complex { get { return false; } }
		public DrawingWrapRightDistance() {
			Value = DefaultValue;
		}
		public DrawingWrapRightDistance(int value) {
			Value = value;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties artProperties = owner as IOfficeArtProperties;
			if (artProperties != null) {
				artProperties.WrapRightDistance = Value;
				artProperties.UseWrapRightDistance = true;
			}
		}
	}
	#endregion
	#region DrawingCropFromTop
	public class DrawingCropFromTop : OfficeDrawingFixedPointPropertyBase {
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties properties = owner as IOfficeArtProperties;
			if(properties != null)
				properties.CropFromTop = Value;
		}
	}
	#endregion
	#region DrawingRotation
	public class DrawingRotation : OfficeDrawingFixedPointPropertyBase {
		public DrawingRotation() { 
		}
		public DrawingRotation(int rotation) {
			Value = rotation;
		}
		public override bool Complex { get { return false; } }
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties artProperties = owner as IOfficeArtProperties;
			if(artProperties != null)
				artProperties.Rotation = Value;
		}
	}
	#endregion
	#region DrawingCropFromBottom
	public class DrawingCropFromBottom : OfficeDrawingFixedPointPropertyBase {
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties properties = owner as IOfficeArtProperties;
			if (properties != null)
				properties.CropFromBottom = Value;
		}
	}
	#endregion
	#region DrawingCropFromLeft
	public class DrawingCropFromLeft : OfficeDrawingFixedPointPropertyBase {
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties properties = owner as IOfficeArtProperties;
			if (properties != null)
				properties.CropFromLeft = Value;
		}
	}
	#endregion
	#region DrawingCropFromRight
	public class DrawingCropFromRight : OfficeDrawingFixedPointPropertyBase {
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties properties = owner as IOfficeArtProperties;
			if (properties != null)
				properties.CropFromRight = Value;
		}
	}
	#endregion
	#region DrawingPictureName
	public class DrawingShapeName : OfficeDrawingStringPropertyValueBase {
	}
	#endregion
	#region DrawingPictureDescription
	public class DrawingShapeDescription : OfficeDrawingStringPropertyValueBase {
	}
	#endregion
	#region DrawingShapeHyperlink
	public class DrawingShapeHyperlink : OfficeDrawingIntPropertyBase {
		public override bool Complex { get { return true; } }
		public byte[] HyperlinkData { 
			get { return ComplexData; } 
			set { SetHyperlinkData(value); } 
		}
		void SetHyperlinkData(byte[] value) {
			SetComplexData(value);
			Value = ComplexData.Length;
		}
	}
	#endregion
	#region DrawingShapeTooltip
	public class DrawingShapeTooltip : OfficeDrawingStringPropertyValueBase {
	}
	#endregion
	#region DrawingFillColor
	public class DrawingFillColor : OfficeDrawingColorPropertyBase {
		public DrawingFillColor() { 
		}
		public DrawingFillColor(Color color) {
			ColorRecord = new OfficeColorRecord(color);
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties artProperties = owner as IOfficeArtProperties;
			if (artProperties != null)
				artProperties.FillColor = ColorRecord.Color;
		}
	}
	#endregion
	#region DrawingFillBackColor
	public class DrawingFillBackColor : OfficeDrawingColorPropertyBase {
		public DrawingFillBackColor() {
		}
		public DrawingFillBackColor(Color color) {
			ColorRecord = new OfficeColorRecord(color);
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
		}
	}
	#endregion
	#region DrawingLineColor
	public class DrawingLineColor : OfficeDrawingColorPropertyBase {
		public DrawingLineColor() { 
		}
		public DrawingLineColor(Color color) {
			ColorRecord = new OfficeColorRecord(color);
		}
		public DrawingLineColor(int colorIndex) {
			ColorRecord = new OfficeColorRecord(colorIndex);
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties artProperties = owner as IOfficeArtProperties;
			if (artProperties != null)
				artProperties.LineColor = ColorRecord.Color;
		}
	}
	#endregion
	#region BlackWhiteMode
	public enum BlackWhiteMode {
		Normal = 0x0000,
		Automatic = 0x0001,
		GrayScale = 0x0002,
		LightGrayScale = 0x0003,
		InverseGray = 0x0004,
		GrayOutline = 0x0005,
		BlackTextLine = 0x0006,
		HighContrast = 0x0007,
		Black = 0x0008,
		White = 0x0009,
		DontShow = 0x000a
	}
	#endregion
	#region DrawingBlackWhiteMode
	public class DrawingBlackWhiteMode : OfficeDrawingIntPropertyBase {
		public DrawingBlackWhiteMode() {
			Value = (int)BlackWhiteMode.Automatic;
		}
		public DrawingBlackWhiteMode(BlackWhiteMode mode) {
			Value = (int)mode;
		}
		public override bool Complex { get { return false; } }
		public BlackWhiteMode Mode { get { return (BlackWhiteMode)Value; } set { Value = (int)value; } }
	}
	#endregion
	#region LineJoinStyle
	public enum LineJoinStyle {
		Bevel = 0x0000,
		Miter = 0x0001,
		Round = 0x0002
	}
	#endregion
	#region DrawingLineJoinStyle
	public class DrawingLineJoinStyle : OfficeDrawingIntPropertyBase {
		public DrawingLineJoinStyle() {
			Value = (int)LineJoinStyle.Miter; 
		}
		public DrawingLineJoinStyle(LineJoinStyle joinStyle) {
			Value = (int)joinStyle;
		}
		public override bool Complex { get { return false; } }
		public LineJoinStyle Style { get { return (LineJoinStyle)Value; } set { Value = (int)value; } }
	}
	#endregion
	#region DrawingLineMiterLimit
	public class DrawingLineMiterLimit : OfficeDrawingFixedPointPropertyBase {
		public const double DefaultValue = 8.0;
		public DrawingLineMiterLimit() {
			Value = DefaultValue;
		}
		public DrawingLineMiterLimit(int miterLimit) {
			Value = miterLimit;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region OutlineCompoundType
	public enum OutlineCompoundType {
		Single = 0x00,
		Double = 0x01,
		ThickThin = 0x02,
		ThinThick = 0x03,
		Triple = 0x04
	}
	#endregion
	#region DrawingLineCompoundType
	public class DrawingLineCompoundType : OfficeDrawingIntPropertyBase {
		public override bool Complex { get { return false; } }
		public OutlineCompoundType CompoundType {
			get { return (OutlineCompoundType)Value; }
			set { Value = (int)value; }
		}
	}
	#endregion
	#region OutlineEndCapStyle
	public enum OutlineEndCapStyle {
		Round = 0x00,
		Square = 0x01,
		Flat = 0x02
	}
	#endregion
	#region DrawingLineCapStyle
	public class DrawingLineCapStyle : OfficeDrawingIntPropertyBase {
		public DrawingLineCapStyle() {
			Value = (int)OutlineEndCapStyle.Flat;
		}
		public DrawingLineCapStyle(OutlineEndCapStyle capStyle) {
			Value = (int)capStyle;
		}
		public override bool Complex { get { return false; } }
		public OutlineEndCapStyle Style { get { return (OutlineEndCapStyle)Value; } set { Value = (int)value; } }
	}
	#endregion
	#region OutlineDashing
	public enum OutlineDashing {
		Solid = 0x00,
		Dash = 0x06,
		DashDot = 0x08,
		Dot = 0x05,
		LongDash = 0x07,
		LongDashDot = 0x09,
		LongDashDotDot = 0x0a,
		SystemDash = 0x01,
		SystemDashDot = 0x03,
		SystemDashDotDot = 0x04,
		SystemDot = 0x02
	}
	#endregion
	#region DrawingLineDashing
	public class DrawingLineDashing : OfficeDrawingIntPropertyBase {
		public DrawingLineDashing() {
			Value = (int)OutlineDashing.Solid;
		}
		public DrawingLineDashing(OutlineDashing dashing) {
			Value = (int)dashing;
		}
		public override bool Complex { get { return false; } }
		public OutlineDashing Dashing { get { return (OutlineDashing)Value; } set { Value = (int)value; } }
	}
	#endregion
	#region DrawingBlipName
	public class DrawingBlipName : OfficeDrawingStringPropertyValueBase {
	}
	#endregion
	#region DrawingBlipFlagsBase
	public abstract class DrawingBlipFlagsBase : OfficeDrawingIntPropertyBase {
		const int flagFile = 0x01;
		const int flagUrl = 0x02;
		const int flagDontSave = 0x04;
		const int flagLinkToFile = 0x08;
		public override bool Complex { get { return false; } }
		public bool IsComment { get { return Value == 0; } }
		public bool IsFile { get { return GetFlag(flagFile); } set { SetFlag(flagFile, value); } }
		public bool IsUrl { get { return GetFlag(flagUrl); } set { SetFlag(flagUrl, value); } }
		public bool DontSave { get { return GetFlag(flagDontSave); } set { SetFlag(flagDontSave, value); } }
		public bool LinkToFile { get { return GetFlag(flagLinkToFile); } set { SetFlag(flagLinkToFile, value); } }
	}
	#endregion
	#region DrawingBlipFlags
	public class DrawingBlipFlags : DrawingBlipFlagsBase {
	}
	#endregion
	#region DrawingFillBlipName
	public class DrawingFillBlipName : OfficeDrawingStringPropertyValueBase {
	}
	#endregion
	#region DrawingFillBlipFlags
	public class DrawingFillBlipFlags : DrawingBlipFlagsBase {
	}
	#endregion
	#region DrawingFillBlipIdentifier
	public class DrawingFillBlipIdentifier : OfficeDrawingIntPropertyBase {
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingShadowColor
	public class DrawingShadowColor : OfficeDrawingColorPropertyBase {
		public DrawingShadowColor() {
		}
		public DrawingShadowColor(Color color) {
			ColorRecord = new OfficeColorRecord(color);
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
		}
	}
	#endregion
	#region DrawingShadowStyleBooleanProperties
	public class DrawingShadowStyleBooleanProperties : OfficeDrawingBooleanPropertyBase {
		const int shadowObscured = 0x0001;
		const int shadow = 0x0002;
		const int useShadowObscured = 0x00010000;
		const int useShadow = 0x00020000;
		public override bool Complex { get { return false; } }
		public bool ShadowObscured { get { return GetFlag(shadowObscured); } set { SetFlag(shadowObscured, value); } }
		public bool Shadow { get { return GetFlag(shadow); } set { SetFlag(shadow, value); } }
		public bool UseShadowObscured { get { return GetFlag(useShadowObscured); } set { SetFlag(useShadowObscured, value); } }
		public bool UseShadow { get { return GetFlag(useShadow); } set { SetFlag(useShadow, value); } }
		public DrawingShadowStyleBooleanProperties() {
			Value = 0;
		}
	}
	#endregion
	#region DrawingConnectionPoints
	public enum ConnectionPointsType {
		None = 0x00000000,
		Segments = 0x00000001,
		Custom = 0x00000002,
		Rect = 0x00000003
	}
	public class DrawingConnectionPointsType : OfficeDrawingIntPropertyBase {
		public DrawingConnectionPointsType() {
			Value = (int)ConnectionPointsType.Segments;
		}
		public DrawingConnectionPointsType(ConnectionPointsType type) {
			Value = (int)type;
		}
		public override bool Complex { get { return false; } }
		public ConnectionPointsType PointsType { get { return (ConnectionPointsType)Value; } set { Value = (int)value; } }
	}
	#endregion
	#region DrawingTextBooleanProperties
	public class DrawingTextBooleanProperties : OfficeDrawingBooleanPropertyBase {
		const int fitShapeToText = 0x0002;
		const int autoTextMargins = 0x0008;
		const int selectText = 0x0010;
		const int useFitShapeToText = 0x00020000;
		const int useAutoTextMargins = 0x00080000;
		const int useSelectText = 0x00100000;
		public override bool Complex { get { return false; } }
		public bool FitShapeToText { get { return GetFlag(fitShapeToText); } set { SetFlag(fitShapeToText, value); } }
		public bool AutoTextMargins { get { return GetFlag(autoTextMargins); } set { SetFlag(autoTextMargins, value); } }
		public bool SelectText { get { return GetFlag(selectText); } set { SetFlag(selectText, value); } }
		public bool UseFitShapeToText { get { return GetFlag(useFitShapeToText); } set { SetFlag(useFitShapeToText, value); } }
		public bool UseAutoTextMargins { get { return GetFlag(useAutoTextMargins); } set { SetFlag(useAutoTextMargins, value); } }
		public bool UseSelectText { get { return GetFlag(useSelectText); } set { SetFlag(useSelectText, value); } }
		public DrawingTextBooleanProperties() {
			Value = autoTextMargins | useAutoTextMargins;
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
			IOfficeArtProperties artProperties = owner as IOfficeArtProperties;
			if (artProperties == null)
				return;
			artProperties.UseFitShapeToText = UseFitShapeToText;
			artProperties.FitShapeToText = FitShapeToText;
		}
	}
	#endregion
	#region DrawingTextDirection
	public enum OfficeTextReadingOrder {
		LeftToRight = 0,
		RightToLeft = 1,
		Context = 2
	}
	public class DrawingTextDirection : OfficeDrawingIntPropertyBase {
		public DrawingTextDirection() {
			Value = (int)OfficeTextReadingOrder.LeftToRight;
		}
		public DrawingTextDirection(OfficeTextReadingOrder direction) {
			Value = (int)direction;
		}
		public override bool Complex { get { return false; } }
		public OfficeTextReadingOrder Direction { get { return (OfficeTextReadingOrder)Value; } set { Value = (int)value; } }
	}
	#endregion
	#region DrawingFillOpacity
	public class DrawingFillOpacity : OfficeDrawingFixedPointPropertyBase {
		public const double DefaultValue = 1.0;
		public DrawingFillOpacity() {
			Value = DefaultValue;
		}
		public DrawingFillOpacity(double opacity) {
			Value = opacity;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DiagramBooleanProperties
	public class DiagramBooleanProperties : OfficeDrawingIntPropertyBase {
		const int defaultFlags = 0x00010000;
		public DiagramBooleanProperties() {
			this.Value = defaultFlags;
		}
	}
	#endregion
	#region FixedPoint
	public class FixedPoint {
		const double fractionalCoeff = 65536.0;
		double value;
		public static FixedPoint FromStream(BinaryReader reader) {
			FixedPoint result = new FixedPoint();
			result.Read(reader);
			return result;
		}
		public static FixedPoint FromBytes(byte[] data, int offset) {
			FixedPoint result = new FixedPoint();
			ushort fractional = BitConverter.ToUInt16(data, offset);
			short integral = BitConverter.ToInt16(data, offset + 2);
			result.value = integral + (fractional / fractionalCoeff);
			return result;
		}
		public double Value { get { return value; } set { this.value = value; } }
		protected void Read(BinaryReader reader) {
			ushort fractional = reader.ReadUInt16();
			short integral = reader.ReadInt16();
			this.value = integral + (fractional / fractionalCoeff); 
		}
		public void Write(BinaryWriter writer) {
			short integral = (short)Value;
			if((Value - integral) != 0.0 && integral < 0)
				integral--;
			ushort fractional = (ushort)((Value - integral) * fractionalCoeff);
			writer.Write(fractional);
			writer.Write(integral);
		}
		public byte[] GetBytes() {
			short integral = (short)Value;
			if((Value - integral) != 0.0 && integral < 0)
				integral--;
			ushort fractional = (ushort)((Value - integral) * fractionalCoeff);
			uint data = ((uint)integral << 16) | fractional;
			return BitConverter.GetBytes(data);
		}
	}
	#endregion
	#region OfficeDrawingFillType
	public enum OfficeFillType {
		Solid = 0,
		Pattern = 1,
		Texture = 2,
		Picture = 3,
		Shade = 4,
		ShadeCenter = 5,
		ShadeShape = 6,
		ShadeScale = 7,
		ShadeTile = 8,
		Background = 9
	}
	public class OfficeDrawingFillType : OfficeDrawingIntPropertyBase {
		public OfficeDrawingFillType() {
			Value = (int)OfficeFillType.Solid;
		}
		public override bool Complex { get { return false; } }
		public OfficeFillType FillType { get { return (OfficeFillType)Value; } set { Value = (int)value; } }
	}
	#endregion
	#region DrawingFillBackOpacity
	public class DrawingFillBackOpacity : OfficeDrawingFixedPointPropertyBase {
		public const double DefaultValue = 1.0;
		public DrawingFillBackOpacity() {
			Value = DefaultValue;
		}
		public DrawingFillBackOpacity(double opacity) {
			Value = opacity;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillBWColor
	public class DrawingFillBWColor : OfficeDrawingColorPropertyBase {
		public DrawingFillBWColor() {
		}
		public DrawingFillBWColor(Color color) {
			ColorRecord = new OfficeColorRecord(color);
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
		}
	}
	#endregion
	#region DrawingFillBlip
	public class DrawingFillBlip : OfficeDrawingIntPropertyBase {
		public override bool Complex { get { return true; } }
	}
	#endregion
	#region DrawingFillWidth
	public class DrawingFillWidth : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = 0;
		public DrawingFillWidth() {
			Value = DefaultValue;
		}
		public DrawingFillWidth(int width) {
			Value = width;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillHeight
	public class DrawingFillHeight : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = 0;
		public DrawingFillHeight() {
			Value = DefaultValue;
		}
		public DrawingFillHeight(int width) {
			Value = width;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillAngle
	public class DrawingFillAngle : OfficeDrawingFixedPointPropertyBase {
		public const double DefaultValue = 0.0;
		public DrawingFillAngle() {
			Value = DefaultValue;
		}
		public DrawingFillAngle(double angle) {
			Value = angle;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillFocus
	public class DrawingFillFocus : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = 0;
		public DrawingFillFocus() {
			Value = DefaultValue;
		}
		public DrawingFillFocus(int focus) {
			if(focus < -100 || focus > 100)
				throw new ArgumentOutOfRangeException("Focus out of range -100..100!");
			Value = focus;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillToLeft
	public class DrawingFillToLeft : OfficeDrawingFixedPointPropertyBase {
		public const double DefaultValue = 0.0;
		public DrawingFillToLeft() {
			Value = DefaultValue;
		}
		public DrawingFillToLeft(double value) {
			Value = value;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillToTop
	public class DrawingFillToTop : OfficeDrawingFixedPointPropertyBase {
		public const double DefaultValue = 0.0;
		public DrawingFillToTop() {
			Value = DefaultValue;
		}
		public DrawingFillToTop(double value) {
			Value = value;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillToRight
	public class DrawingFillToRight : OfficeDrawingFixedPointPropertyBase {
		public const double DefaultValue = 0.0;
		public DrawingFillToRight() {
			Value = DefaultValue;
		}
		public DrawingFillToRight(double value) {
			Value = value;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillToBottom
	public class DrawingFillToBottom : OfficeDrawingFixedPointPropertyBase {
		public const double DefaultValue = 0.0;
		public DrawingFillToBottom() {
			Value = DefaultValue;
		}
		public DrawingFillToBottom(double value) {
			Value = value;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillRectLeft
	public class DrawingFillRectLeft : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = 0;
		public DrawingFillRectLeft() {
			Value = DefaultValue;
		}
		public DrawingFillRectLeft(int value) {
			Value = value;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillRectTop
	public class DrawingFillRectTop : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = 0;
		public DrawingFillRectTop() {
			Value = DefaultValue;
		}
		public DrawingFillRectTop(int value) {
			Value = value;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillRectRight
	public class DrawingFillRectRight : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = 0;
		public DrawingFillRectRight() {
			Value = DefaultValue;
		}
		public DrawingFillRectRight(int value) {
			Value = value;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillRectBottom
	public class DrawingFillRectBottom : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = 0;
		public DrawingFillRectBottom() {
			Value = DefaultValue;
		}
		public DrawingFillRectBottom(int value) {
			Value = value;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillDzType
	public enum OfficeDzType {
		Default = 0x0000,
		EMU = 0x0001,
		Pixels = 0x0002,
		Shape = 0x0003,
		FixedAspect = 0x0004,
		EMUFixed = 0x0005,
		PixelsFixed = 0x0006,
		ShapeFixed = 0x0007,
		FixedAspectEnlarge = 0x0008,
		EMUFixedBig = 0x0009,
		PixelsFixedBig = 0x000a,
		ShapeFixedBig = 0x000b
	}
	public class DrawingFillDzType : OfficeDrawingIntPropertyBase {
		public DrawingFillDzType() {
			Value = (int)OfficeDzType.Default;
		}
		public DrawingFillDzType(OfficeDzType dzType) {
			Value = (int)dzType;
		}
		public override bool Complex { get { return false; } }
		public OfficeDzType DzType { get { return (OfficeDzType)Value; } set { Value = (int)value; } }
	}
	#endregion
	#region DrawingFillShadePreset
	public class DrawingFillShadePreset : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = 0;
		public DrawingFillShadePreset() {
			Value = DefaultValue;
		}
		public DrawingFillShadePreset(int value) {
			Value = value;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillShadeColors
	public class DrawingFillShadeColors : OfficeDrawingIntPropertyBase {
		public override bool Complex { get { return true; } }
	}
	#endregion
	#region DrawingFillOriginX
	public class DrawingFillOriginX : OfficeDrawingFixedPointPropertyBase {
		public const double DefaultValue = 0.0;
		public DrawingFillOriginX() {
			Value = DefaultValue;
		}
		public DrawingFillOriginX(double origin) {
			if(origin < -1.5 || origin > 0.5)
				throw new ArgumentOutOfRangeException("Origin out of range -1.5..0.5!");
			Value = origin;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillOriginY
	public class DrawingFillOriginY : OfficeDrawingFixedPointPropertyBase {
		public const double DefaultValue = 0.0;
		public DrawingFillOriginY() {
			Value = DefaultValue;
		}
		public DrawingFillOriginY(double origin) {
			if(origin < -1.5 || origin > 0.5)
				throw new ArgumentOutOfRangeException("Origin out of range -1.5..0.5!");
			Value = origin;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillShapeOriginX
	public class DrawingFillShapeOriginX : OfficeDrawingFixedPointPropertyBase {
		public const double DefaultValue = 0.0;
		public DrawingFillShapeOriginX() {
			Value = DefaultValue;
		}
		public DrawingFillShapeOriginX(double origin) {
			if(origin < -0.5 || origin > 0.5)
				throw new ArgumentOutOfRangeException("Origin out of range -0.5..0.5!");
			Value = origin;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillShapeOriginY
	public class DrawingFillShapeOriginY : OfficeDrawingFixedPointPropertyBase {
		public const double DefaultValue = 0.0;
		public DrawingFillShapeOriginY() {
			Value = DefaultValue;
		}
		public DrawingFillShapeOriginY(double origin) {
			if(origin < -0.5 || origin > 0.5)
				throw new ArgumentOutOfRangeException("Origin out of range -0.5..0.5!");
			Value = origin;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillShadeType
	public class DrawingFillShadeType : OfficeDrawingIntPropertyBase {
		const int color = 0x0001;
		const int gamma = 0x0002;
		const int sigma = 0x0004;
		const int band = 0x0008;
		const int oneColor = 0x0010;
		public DrawingFillShadeType() {
			Value = 0x03;
		}
		public override bool Complex { get { return false; } }
		public bool Color { get { return GetFlag(color); } set { SetFlag(color, value); } }
		public bool Gamma { get { return GetFlag(gamma); } set { SetFlag(gamma, value); } }
		public bool Sigma { get { return GetFlag(sigma); } set { SetFlag(sigma, value); } }
		public bool Band { get { return GetFlag(band); } set { SetFlag(band, value); } }
		public bool OneColor { get { return GetFlag(oneColor); } set { SetFlag(oneColor, value); } }
	}
	#endregion
	#region DrawingFillColorExt
	public class DrawingFillColorExt : OfficeDrawingColorPropertyBase {
		public DrawingFillColorExt() {
			ColorRecord = new OfficeColorRecord();
		}
		public DrawingFillColorExt(Color color) {
			ColorRecord = new OfficeColorRecord(color);
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
		}
	}
	#endregion
	#region DrawingFillReserved415
	public class DrawingFillReserved415 : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = -1;
		public DrawingFillReserved415() {
			Value = DefaultValue;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region OfficeTintShade
	public enum OfficeTintShade {
		Undefined = 0x0000,
		Shade = 0x0001,
		Tint = 0x0002
	}
	#endregion
	#region DrawingFillTintShade
	public class DrawingFillTintShade : OfficeDrawingIntPropertyBase {
		public DrawingFillTintShade() {
			Value = (int)OfficeTintShade.Tint;
		}
		public override bool Complex { get { return false; } }
		public OfficeTintShade TineShade { get { return (OfficeTintShade)Value; } set { Value = (int)value; } }
	}
	#endregion
	#region DrawingFillReserved417
	public class DrawingFillReserved417 : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = 0;
		public DrawingFillReserved417() {
			Value = DefaultValue;
		}
		public override bool Complex { get { return true; } }
	}
	#endregion
	#region DrawingFillBackColorExt
	public class DrawingFillBackColorExt : OfficeDrawingColorPropertyBase {
		public DrawingFillBackColorExt() {
			ColorRecord = new OfficeColorRecord();
		}
		public DrawingFillBackColorExt(Color color) {
			ColorRecord = new OfficeColorRecord(color);
		}
		public override void Execute(OfficeArtPropertiesBase owner) {
		}
	}
	#endregion
	#region DrawingFillReserved419
	public class DrawingFillReserved419 : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = -1;
		public DrawingFillReserved419() {
			Value = DefaultValue;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillBackTintShade
	public class DrawingFillBackTintShade : OfficeDrawingIntPropertyBase {
		public DrawingFillBackTintShade() {
			Value = (int)OfficeTintShade.Tint;
		}
		public override bool Complex { get { return false; } }
		public OfficeTintShade TineShade { get { return (OfficeTintShade)Value; } set { Value = (int)value; } }
	}
	#endregion
	#region DrawingFillReserved421
	public class DrawingFillReserved421 : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = 0;
		public DrawingFillReserved421() {
			Value = DefaultValue;
		}
		public override bool Complex { get { return true; } }
	}
	#endregion
	#region DrawingFillReserved422
	public class DrawingFillReserved422 : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = -1;
		public DrawingFillReserved422() {
			Value = DefaultValue;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region DrawingFillReserved423
	public class DrawingFillReserved423 : OfficeDrawingIntPropertyBase {
		public const int DefaultValue = -1;
		public DrawingFillReserved423() {
			Value = DefaultValue;
		}
		public override bool Complex { get { return false; } }
	}
	#endregion
	#region OfficeDrawingTypedMsoArrayPropertyBase
	public abstract class OfficeDrawingTypedMsoArrayPropertyBase<T> : OfficeDrawingMsoArrayPropertyBase {
		const int MsoArrayHeaderSize = 6;
		public T[] GetElements() {
			if(ComplexData.Length < MsoArrayHeaderSize)
				return new T[0];
			int count = BitConverter.ToUInt16(ComplexData, 0);
			int elementSize = BitConverter.ToUInt16(ComplexData, 4);
			if(elementSize == 0xFFF0)
				elementSize = 4;
			if(ComplexData.Length != elementSize * count + MsoArrayHeaderSize)
				return new T[0];
			T[] result = new T[count];
			int offset = MsoArrayHeaderSize;
			for(int i = 0; i < count; i++) {
				result[i] = ReadElement(offset, elementSize);
				offset += elementSize;
			}
			return result;
		}
		public void SetElements(T[] elements) {
			if(elements.Length == 0)
				return;
			ushort elementSize = (ushort) ElementSize;
			byte[] length = BitConverter.GetBytes((ushort)elements.Length);
			List<byte> data = new List<byte>(elements.Length * elementSize + MsoArrayHeaderSize);
			data.AddRange(length);
			data.AddRange(length);
			data.AddRange(BitConverter.GetBytes(elementSize));
			foreach(T element in elements) {
				WriteElement(element, data);
			}
			SetComplexData(data.ToArray());
		}
		public abstract T ReadElement(int offset, int size);
		public abstract void WriteElement(T element, List<byte> data);
		public abstract int ElementSize { get; }
	}
	#endregion
	#region OfficeDrawingMsoArrayPropertyBase
	public abstract class OfficeDrawingMsoArrayPropertyBase : OfficeDrawingPropertyBase {
		public override bool Complex { get { return true; } }
		public int Value { get; set; }
		public override void Read(BinaryReader reader) {
			Value = reader.ReadInt32();
		}
		public override void Execute(OfficeArtPropertiesBase owner) {}
		public override void Write(BinaryWriter writer) {
		}
	}
	#endregion
	#region DrawingGeometryPointArray
	public class DrawingGeometryPointArray : OfficeDrawingTypedMsoArrayPropertyBase<Point> {
		#region Overrides of OfficeDrawingMsoArrayPropertyBase<Point>
		public override Point ReadElement(int offset, int size) {
			switch(size) {
				case 4:
					return new Point(BitConverter.ToInt16(ComplexData, offset), BitConverter.ToInt16(ComplexData, offset + 2));
				case 8:
					return new Point(BitConverter.ToInt32(ComplexData, offset), BitConverter.ToInt32(ComplexData, offset + 4));
				default:
					return new Point();
			}
		}
		public override void WriteElement(Point element, List<byte> data) {
			data.AddRange(BitConverter.GetBytes(element.X));
			data.AddRange(BitConverter.GetBytes(element.Y));
		}
		public override int ElementSize { get { return 8; } }
		#endregion
	}
	#endregion
	#region DrawingGeometryVertices
	public class DrawingGeometryVertices : DrawingGeometryPointArray {}
	#endregion
	#region DrawingGeometryConnectionSites
	public class DrawingGeometryConnectionSites : DrawingGeometryPointArray { }
	#endregion
	#region DrawingGeometryBooleanProperties
	public class DrawingGeometryBooleanProperties : OfficeDrawingBooleanPropertyBase {
		const int FillFlag = 0x0001;					
		const int FillShapeShapeFlag = 0x0002;		  
		const int GTextFlag = 0x0004;				   
		const int LineFlag = 0x0008;					
		const int F3DFlag = 0x0010;					 
		const int ShadowFlag = 0x0020;				  
		const int SoftEdgeFlag = 0x0080;				
		const int GlowFlag = 0x00100;				   
		const int ReflectionFlag = 0x00200;			 
		const int UseFillFlag = 0x00010000;			 
		const int UseFillShapeShapeFlag = 0x00020000;   
		const int UseGTextFlag = 0x00040000;			
		const int UseLineFlag = 0x00080000;			 
		const int UseF3DFlag = 0x00100000;			  
		const int UseShadowFlag = 0x00200000;		   
		const int UseSoftEdgeFlag = 0x00800000;		 
		const int UseGlowFlag = 0x001000000;			
		const int UseReflectionFlag = 0x002000000;	  
		public override bool Complex { get { return false; } }
		public bool Fill { get { return GetFlag(FillFlag); } set { SetFlag(FillFlag, value); } }
		public bool FillShadeShape { get { return GetFlag(FillShapeShapeFlag); } set { SetFlag(FillShapeShapeFlag, value); } }
		public bool GText { get { return GetFlag(GTextFlag); } set { SetFlag(GTextFlag, value); } }
		public bool Line { get { return GetFlag(LineFlag); } set { SetFlag(LineFlag, value); } }
		public bool F3D { get { return GetFlag(F3DFlag); } set { SetFlag(F3DFlag, value); } }
		public bool Shadow { get { return GetFlag(ShadowFlag); } set { SetFlag(ShadowFlag, value); } }
		public bool SoftEdge { get { return GetFlag(SoftEdgeFlag); } set { SetFlag(SoftEdgeFlag, value); } }
		public bool Glow { get { return GetFlag(GlowFlag); } set { SetFlag(GlowFlag, value); } }
		public bool Reflection { get { return GetFlag(ReflectionFlag); } set { SetFlag(ReflectionFlag, value); } }
		public bool UseFill { get { return GetFlag(UseFillFlag); } set { SetFlag(UseFillFlag, value); } }
		public bool UseFillShadeShape { get { return GetFlag(UseFillShapeShapeFlag); } set { SetFlag(UseFillShapeShapeFlag, value); } }
		public bool UseGText { get { return GetFlag(UseGTextFlag); } set { SetFlag(UseGTextFlag, value); } }
		public bool UseLine { get { return GetFlag(UseLineFlag); } set { SetFlag(UseLineFlag, value); } }
		public bool UseF3D { get { return GetFlag(UseF3DFlag); } set { SetFlag(UseF3DFlag, value); } }
		public bool UseShadow { get { return GetFlag(UseShadowFlag); } set { SetFlag(UseShadowFlag, value); } }
		public bool UseSoftEdge { get { return GetFlag(UseSoftEdgeFlag); } set { SetFlag(UseSoftEdgeFlag, value); } }
		public bool UseGlow { get { return GetFlag(UseGlowFlag); } set { SetFlag(UseGlowFlag, value); } }
		public bool UseReflection { get { return GetFlag(UseReflectionFlag); } set { SetFlag(UseReflectionFlag, value); } }
	}
	#endregion
	#region DrawingGeometryShapePath
	public enum ShapePathType {
		Lines,
		LinesClosed,
		Curves,
		CurvesClosed,
		Complex
	}
	public class DrawingGeometryShapePath : OfficeDrawingIntPropertyBase {
		#region Overrides of OfficeDrawingPropertyBase
		public override bool Complex { get { return false; } }
		#endregion
		public ShapePathType ShapePath { get { return (ShapePathType)Value; } set { Value = (int)value; } }
	}
	#endregion
	#region DrawingGeometrySegmentInfo
	public enum MsoPathType {
		LineTo,
		CurveTo,
		MoveTo,
		Close,
		End,
		Escape,
		ClientEscape
	}
	public enum MsoPathEscape {
		Extension,
		AngleEllipseTo,
		AngleEllipse,
		ArcTo,
		Arc,
		ClockwiseArcTo,
		ClockwiseArc,
		EllipticalQuadrantX,
		EllipticalQuadrantY,
		QuadraticBezier,
		NoFill,
		NoLine,
		AutoLine,
		AutoCurve,
		CornerLine,
		CornerCurve,
		SmoothLine,
		SmoothCurve,
		SymmetricLine,
		SymmetricCurve,
		Freeform,
		FillColor,
		LineColor
	}
	public class MsoPathInfo {
		public MsoPathType PathType { get; set; }
		public MsoPathEscape PathEscape { get; set; }
		public int Segments { get; set; }
		public MsoPathInfo(MsoPathType pathType, int segments) {
			PathType = pathType;
			Segments = segments;
		}
		public MsoPathInfo(MsoPathEscape escapeType, int segments) {
			PathType = MsoPathType.Escape;
			PathEscape = escapeType;
			Segments = segments;
		}
	}
	public class DrawingGeometrySegmentInfo : OfficeDrawingTypedMsoArrayPropertyBase<MsoPathInfo> {
		const int PathEscapeMask = 0x1F;
		const int PathTypeMask = 0xE000;
		#region Overrides of OfficeDrawingTypedMsoArrayPropertyBase<MsoPathInfo>
		public override MsoPathInfo ReadElement(int offset, int size) {
			if(size != 2)
				return new MsoPathInfo(MsoPathType.End, 0);
			MsoPathInfo result;
			MsoPathType pathType = (MsoPathType) (ComplexData[offset + 1] >> 5);
			int segments;
			if(pathType == MsoPathType.ClientEscape || pathType == MsoPathType.Escape) {
				MsoPathEscape pathEscape = (MsoPathEscape) (ComplexData[offset + 1] & PathEscapeMask);
				segments = ComplexData[offset];
				result = new MsoPathInfo(pathEscape, segments);
			}
			else {
				segments = BitConverter.ToInt16(ComplexData, offset) & (~PathTypeMask);
				result = new MsoPathInfo(pathType, segments);
			}
			return result;
		}
		public override void WriteElement(MsoPathInfo element, List<byte> data) {
			ushort result = (ushort) ((ushort) element.PathType << 13);
			if(element.PathType == MsoPathType.ClientEscape || element.PathType == MsoPathType.Escape) {
				result |= (ushort) ((int) element.PathEscape << 8);
			}
			result |= (ushort) element.Segments;
			data.AddRange(BitConverter.GetBytes(result));
		}
		public override int ElementSize { get { return 2; } }
		#endregion
	}
	#endregion
	#region DrawingGeometryConnectionSitesDir
	public class DrawingGeometryConnectionSitesDir : OfficeDrawingTypedMsoArrayPropertyBase<FixedPoint> {
		#region Overrides of OfficeDrawingTypedMsoArrayPropertyBase<FixedPoint>
		public override FixedPoint ReadElement(int offset, int size) {
			if(size != 4)
				return new FixedPoint();
			return FixedPoint.FromBytes(ComplexData, offset);
		}
		public override void WriteElement(FixedPoint element, List<byte> data) {
			data.AddRange(element.GetBytes());
		}
		public override int ElementSize { get { return 4; } }
		#endregion
	}
	#endregion
	#region DrawingGeometryAdjustValue
	public interface IDrawingGeometryAdjustValue {
		int Index { get; }
	}
	public class DrawingGeometryAdjustValue : OfficeDrawingIntPropertyBase {
		#region Overrides of OfficeDrawingPropertyBase
		public override bool Complex { get { return false; } }
		#endregion
	}
	public class DrawingGeometryAdjustValue1 : DrawingGeometryAdjustValue, IDrawingGeometryAdjustValue {
		#region Implementation of IDrawingGeometryAdjustValue
		public int Index { get { return 0; } }
		#endregion
	}
	public class DrawingGeometryAdjustValue2 : DrawingGeometryAdjustValue, IDrawingGeometryAdjustValue {
		#region Implementation of IDrawingGeometryAdjustValue
		public int Index { get { return 1; } }
		#endregion
	}
	public class DrawingGeometryAdjustValue3 : DrawingGeometryAdjustValue, IDrawingGeometryAdjustValue {
		#region Implementation of IDrawingGeometryAdjustValue
		public int Index { get { return 2; } }
		#endregion
	}
	public class DrawingGeometryAdjustValue4 : DrawingGeometryAdjustValue, IDrawingGeometryAdjustValue {
		#region Implementation of IDrawingGeometryAdjustValue
		public int Index { get { return 3; } }
		#endregion
	}
	public class DrawingGeometryAdjustValue5 : DrawingGeometryAdjustValue, IDrawingGeometryAdjustValue {
		#region Implementation of IDrawingGeometryAdjustValue
		public int Index { get { return 4; } }
		#endregion
	}
	public class DrawingGeometryAdjustValue6 : DrawingGeometryAdjustValue, IDrawingGeometryAdjustValue {
		#region Implementation of IDrawingGeometryAdjustValue
		public int Index { get { return 5; } }
		#endregion
	}
	public class DrawingGeometryAdjustValue7 : DrawingGeometryAdjustValue, IDrawingGeometryAdjustValue {
		#region Implementation of IDrawingGeometryAdjustValue
		public int Index { get { return 6; } }
		#endregion
	}
	public class DrawingGeometryAdjustValue8 : DrawingGeometryAdjustValue, IDrawingGeometryAdjustValue {
		#region Implementation of IDrawingGeometryAdjustValue
		public int Index { get { return 7; } }
		#endregion
	}
	#endregion
	#region DrawingGeometryLeft
	public class DrawingGeometryLeft : OfficeDrawingIntPropertyBase {
		#region Overrides of OfficeDrawingPropertyBase
		public override bool Complex { get { return false; } }
		#endregion
	}
	#endregion
	#region DrawingGeometryRight
	public class DrawingGeometryRight : OfficeDrawingIntPropertyBase {
		#region Overrides of OfficeDrawingPropertyBase
		public override bool Complex { get { return false; } }
		#endregion
	}
	#endregion
	#region DrawingGeometryTop
	public class DrawingGeometryTop : OfficeDrawingIntPropertyBase {
		#region Overrides of OfficeDrawingPropertyBase
		public override bool Complex { get { return false; } }
		#endregion
	}
	#endregion
	#region DrawingGeometryBottom
	public class DrawingGeometryBottom : OfficeDrawingIntPropertyBase {
		#region Overrides of OfficeDrawingPropertyBase
		public override bool Complex { get { return false; } }
		#endregion
	}
	#endregion
	#region DrawingMetroBlob
	public class DrawingMetroBlob : OfficeDrawingIntPropertyBase {
		#region Overrides of OfficeDrawingPropertyBase
		public override bool Complex { get { return true; } }
		#endregion
	}
	#endregion
	#region MsoWrapMode
	public enum MsoWrapMode {
		WrapSquare,
		WrapByPoints,
		WrapNone,
		WrapTopBottom,
		WrapThrough
	}
	#endregion
	#region DrawingShapeWrapText
	public class DrawingShapeWrapText : OfficeDrawingIntPropertyBase {
		#region Overrides of OfficeDrawingPropertyBase
		public override bool Complex { get { return false; } }
		#endregion
		public MsoWrapMode WrapMode { get { return (MsoWrapMode) Value; } set { Value = (int) value; } }
	}
	#endregion
	#region DrawingShapeAnchorText
	public enum MsoAnchor {
		Top,
		Middle,
		Bottom,
		TopCentered,
		MiddleCentered,
		BottomCentered,
		TopBaseline,
		BottomBaseline,
		TopCenteredBaseline,
		BottomCenteredBaseline
	}
	public class DrawingShapeAnchorText : OfficeDrawingIntPropertyBase {
		#region Overrides of OfficeDrawingPropertyBase
		public override bool Complex { get { return false; } }
		#endregion
		public MsoAnchor Anchor { get { return (MsoAnchor) Value; } set { Value = (int) value; } }
	}
	#endregion
	#region DrawingShapeTextFlow
	public enum MsoTextFlow {
		Horizontal,
		RightA,
		Left,
		RightN,
		HorizontalA,
		Vertical
	}
	public class DrawingShapeTextFlow : OfficeDrawingIntPropertyBase {
		#region Overrides of OfficeDrawingPropertyBase
		public override bool Complex { get { return false; } }
		#endregion
		public MsoTextFlow TextFlow { get { return (MsoTextFlow) Value; } set { Value = (int) value; } }
	}
	#endregion
	#region DrawingShapeFontDirection
	public enum FontDirection {
		Angle0,
		Angle90,
		Angle180,
		Angle270
	}
	public class DrawingShapeFontDirection : OfficeDrawingIntPropertyBase {
		#region Overrides of OfficeDrawingPropertyBase
		public override bool Complex { get { return false; } }
		#endregion
		public FontDirection Direction { get { return (FontDirection) Value; } set { Value = (int) value; } }
	}
	#endregion
	#region OfficeShadeColor
	public class OfficeShadeColor {
		#region Fields
		public const int Size = 8;
		OfficeColorRecord colorRecord;
		double position;
		#endregion
		#region Properties
		public OfficeColorRecord ColorRecord {
			get { return colorRecord; }
			set {
				if(value == null)
					value = new OfficeColorRecord();
				colorRecord = value;
			}
		}
		public double Position {
			get { return position; }
			set {
				if(value < 0.0 || value > 1.0)
					throw new ArgumentOutOfRangeException("Position out of range 0.0...1.0!");
				position = value;
			}
		}
		#endregion
		public OfficeShadeColor() {
			this.colorRecord = new OfficeColorRecord();
			this.position = 0.0;
		}
		public OfficeShadeColor(Color color, double position) {
			this.colorRecord = new OfficeColorRecord(color);
			Position = position;
		}
		public static OfficeShadeColor FromStream(BinaryReader reader) {
			OfficeShadeColor result = new OfficeShadeColor();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			this.colorRecord = OfficeColorRecord.FromStream(reader);
			this.position = FixedPoint.FromStream(reader).Value;
		}
		public void Write(BinaryWriter writer) {
			this.colorRecord.Write(writer);
			FixedPoint fixedPoint = new FixedPoint();
			fixedPoint.Value = this.position;
			fixedPoint.Write(writer);
		}
	}
	#endregion
}
