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
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region OfficeArtClientAnchorBase
	public abstract class OfficeArtClientAnchorBase : OfficeDrawingPartBase {
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.ClientAnchor; } }
		public override int HeaderVersion { get { return OfficeArtVersions.EmptyHeaderVersion; } }
		#endregion
	}
	#endregion
	#region OfficeArtClientAnchorSheet
	public class OfficeArtClientAnchorSheet : OfficeArtClientAnchorBase {
		public static OfficeArtClientAnchorSheet FromStream(BinaryReader reader) {
			OfficeArtClientAnchorSheet result = new OfficeArtClientAnchorSheet();
			result.Read(reader);
			return result;
		}
		#region Properties
		public bool KeepOnMove { get; set; }
		public bool KeepOnResize { get; set; }
		public CellPosition TopLeft { get; set; }
		public CellPosition BottomRight { get; set; }
		public int DeltaXLeft { get; set; }
		public int DeltaYTop { get; set; }
		public int DeltaXRight { get; set; }
		public int DeltaYBottom { get; set; }
		#endregion
		protected internal void Read(BinaryReader reader) {
			ushort bitwiseField = reader.ReadUInt16();
			KeepOnMove = Convert.ToBoolean(bitwiseField & 0x0001);
			KeepOnResize = Convert.ToBoolean(bitwiseField & 0x0002);
			int leftColumn = reader.ReadUInt16();
			DeltaXLeft = reader.ReadInt16();
			int topRow = reader.ReadUInt16();
			DeltaYTop = reader.ReadInt16();
			int rightColumn = reader.ReadUInt16();
			DeltaXRight = reader.ReadInt16();
			int bottomRow = reader.ReadUInt16();
			DeltaYBottom = reader.ReadInt16();
			TopLeft = new CellPosition(leftColumn, topRow);
			BottomRight = new CellPosition(rightColumn, bottomRow);
		}
		protected override void WriteCore(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if(KeepOnMove)
				bitwiseField |= 0x0001;
			if(KeepOnResize)
				bitwiseField |= 0x0002;
			writer.Write(bitwiseField);
			writer.Write((ushort)TopLeft.Column);
			writer.Write((short)DeltaXLeft);
			writer.Write((ushort)TopLeft.Row);
			writer.Write((short)DeltaYTop);
			writer.Write((ushort)BottomRight.Column);
			writer.Write((short)DeltaXRight);
			writer.Write((ushort)BottomRight.Row);
			writer.Write((short)DeltaYBottom);
		}
		protected override int GetSize() {
			return 18;
		}
		protected internal bool IsValid() {
			return TopLeft.Column <= BottomRight.Column && TopLeft.Row <= BottomRight.Row;
		}
	}
	#endregion
	#region OfficeArtClientData
	public class OfficeArtClientData : OfficeDrawingPartBase {
		XlsObjData data = new XlsObjData();
		public static OfficeArtClientData FromStream(BinaryReader reader, XlsContentBuilder contentBuilder) {
			OfficeArtClientData result = new OfficeArtClientData();
			result.Data.Read(reader, contentBuilder); 
			return result;
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.ClientData; } }
		public override int HeaderVersion { get { return OfficeArtVersions.EmptyHeaderVersion; } }
		public XlsObjData Data { get { return data; } }
		#endregion
		protected override void WriteCore(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null) {
				chunkWriter.Flush();
				chunkWriter.SetNextChunk(new XlsCommandObject());
				this.data.Write(chunkWriter);
				chunkWriter.Flush();
				chunkWriter.SetNextChunk(new XlsCommandMsoDrawing());
			}
		}
		protected override int GetSize() {
			return 0;
		}
	}
	#endregion
	#region OfficeArtClientAnchorChart
	public class OfficeArtClientAnchorChart : OfficeArtClientAnchorBase {
		public static OfficeArtClientAnchorChart FromStream(BinaryReader reader) {
			OfficeArtClientAnchorChart result = new OfficeArtClientAnchorChart();
			result.Read(reader);
			return result;
		}
		#region Properties
		public bool KeepOnResize { get; set; }
		public int DeltaXLeft { get; set; }
		public int DeltaYTop { get; set; }
		public int DeltaXRight { get; set; }
		public int DeltaYBottom { get; set; }
		#endregion
		protected internal void Read(BinaryReader reader) {
			ushort bitwiseField = reader.ReadUInt16();
			KeepOnResize = Convert.ToBoolean(bitwiseField & 0x0002);
			DeltaXLeft = reader.ReadInt32();
			DeltaYTop = reader.ReadInt32();
			DeltaXRight = reader.ReadInt32();
			DeltaYBottom = reader.ReadInt32();
		}
		protected override void WriteCore(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if(KeepOnResize)
				bitwiseField |= 0x0002;
			writer.Write(bitwiseField);
			writer.Write(DeltaXLeft);
			writer.Write(DeltaYTop);
			writer.Write(DeltaXRight);
			writer.Write(DeltaYBottom);
		}
		protected override int GetSize() {
			return 18;
		}
	}
	#endregion
	#region OfficeArtClientAnchorHeaderFooter
	public class OfficeArtClientAnchorHeaderFooter : OfficeArtClientAnchorBase {
		public static OfficeArtClientAnchorHeaderFooter FromStream(BinaryReader reader) {
			OfficeArtClientAnchorHeaderFooter result = new OfficeArtClientAnchorHeaderFooter();
			result.Read(reader);
			return result;
		}
		int width;
		int height;
		#region Properties
		public int Width {
			get { return width; }
			set {
				Guard.ArgumentPositive(value, "Width");
				width = value;
			}
		}
		public int Height {
			get { return height; }
			set {
				Guard.ArgumentPositive(value, "Height");
				height = value;
			}
		}
		#endregion
		protected internal void Read(BinaryReader reader) {
			Width = reader.ReadInt32();
			Height = reader.ReadInt32();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(Width);
			writer.Write(Height);
		}
		protected override int GetSize() {
			return 8;
		}
	}
	#endregion
	#region OfficeArtChildAnchor
	public class OfficeArtChildAnchor : OfficeDrawingPartBase {
		#region Fields
		const int recordLength = 0x10;
		int left;
		int top;
		int right;
		int bottom;
		#endregion
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.ChildAnchor; } }
		public override int HeaderVersion { get { return OfficeArtVersions.EmptyHeaderVersion; } }
		public int Left { get { return left; } set { left = value; } }
		public int Top { get { return top; } set { top = value; } }
		public int Right { get { return right; } set { right = value; } }
		public int Bottom { get { return bottom; } set { bottom = value; } }
		#endregion
		public static OfficeArtChildAnchor FromStream(BinaryReader reader) {
			OfficeArtChildAnchor result = new OfficeArtChildAnchor();
			result.Read(reader);
			return result;
		}
		protected internal void Read(BinaryReader reader) {
			left = reader.ReadInt32();
			top = reader.ReadInt32();
			right = reader.ReadInt32();
			bottom = reader.ReadInt32();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(Left);
			writer.Write(Top);
			writer.Write(Right);
			writer.Write(Bottom);
		}
		protected override int GetSize() {
			return recordLength;
		}
	}
	#endregion
	#region OfficeArtClientTextbox
	public class OfficeArtClientTextbox : OfficeDrawingPartBase {
		XlsTextObjData data = new XlsTextObjData();
		public static OfficeArtClientTextbox FromStream(BinaryReader reader, XlsContentBuilder contentBuilder) {
			OfficeArtClientTextbox result = new OfficeArtClientTextbox();
			result.Data.Read(reader, contentBuilder); 
			return result;
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.ClientTextbox; } }
		public override int HeaderVersion { get { return OfficeArtVersions.EmptyHeaderVersion; } }
		public XlsTextObjData Data { get { return data; } }
		#endregion
		protected override void WriteCore(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null) {
				chunkWriter.Flush();
				chunkWriter.SetNextChunk(new XlsCommandTextObject());
				this.data.Write(chunkWriter);
				chunkWriter.Flush();
				chunkWriter.SetNextChunk(new XlsCommandMsoDrawing());
			}
		}
		protected override int GetSize() {
			return 0;
		}
	}
	#endregion
	#region OfficeArtBlipStoreContainerEx
	public class OfficeArtBlipStoreContainerEx : OfficeArtBlipStoreContainer {
		const int blipHeaderSize = OfficeArtRecordHeader.Size + 66;
		const int blipStoreEntryHeaderSize = OfficeArtRecordHeader.Size + 36 + blipHeaderSize;
		protected override void WriteCore(BinaryWriter writer, BinaryWriter embeddedWriter) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null) {
				int maxSize = (int)(GetMaxBlipSize() + chunkWriter.BaseStream.Length);
				int newCapacity = (maxSize / XlsDefs.MaxRecordDataSize + 1) * XlsDefs.MaxRecordDataSize;
				if(newCapacity > chunkWriter.Capacity)
					chunkWriter.Capacity = newCapacity;
			}
			int count = Blips.Count;
			for(int i = 0; i < count; i++) {
				FileBlipStoreEntry fileBlipStore = Blips[i] as FileBlipStoreEntry;
				if(fileBlipStore != null) {
					AdjustChunkWriter(chunkWriter, blipStoreEntryHeaderSize, true);
					fileBlipStore.Write(writer, embeddedWriter);
				}
				else {
					AdjustChunkWriter(chunkWriter, blipHeaderSize, true);
					Blips[i].Write(writer);
				}
				AdjustChunkWriter(chunkWriter, 0, false);
			}
		}
		void AdjustChunkWriter(XlsChunkWriter chunkWriter, int requiredSpace, bool suppressAutoFlush) {
			if(chunkWriter != null) {
				if(requiredSpace > 0)
					chunkWriter.BeginRecord(requiredSpace);
				chunkWriter.SuppressAutoFlush = suppressAutoFlush;
			}
		}
		int GetMaxBlipSize() {
			int result = 0;
			int count = Blips.Count;
			for(int i = 0; i < count; i++)
				result = Math.Max(result, Blips[i].GetSize());
			return result;
		}
	}
	#endregion
	#region OfficeArtDrawingGroupContainer
	public class OfficeArtDrawingGroupContainer : CompositeOfficeDrawingPartBase {
		#region static
		public static OfficeArtDrawingGroupContainer FromStream(BinaryReader reader) {
			OfficeArtDrawingGroupContainer result = new OfficeArtDrawingGroupContainer();
			result.Read(reader);
			return result;
		}
		#endregion
		#region Fields
		OfficeArtFileDrawingGroupRecordEx fileDrawingBlock;
		OfficeArtBlipStoreContainer blipContainer;
		OfficeArtProperties artProperties;
		#endregion
		public OfficeArtDrawingGroupContainer() {
			this.fileDrawingBlock = new OfficeArtFileDrawingGroupRecordEx();
			this.blipContainer = new OfficeArtBlipStoreContainerEx();
			this.artProperties = new OfficeArtProperties();
			Items.Add(this.fileDrawingBlock);
			Items.Add(this.blipContainer);
			Items.Add(this.artProperties);
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.DrawingContainer; } }
		public override int HeaderVersion { get { return OfficeArtVersions.DefaultHeaderVersion; } }
		public OfficeArtFileDrawingGroupRecordEx FileDrawingBlock { get { return fileDrawingBlock; } }
		public OfficeArtBlipStoreContainer BlipContainer { get { return blipContainer; } }
		public OfficeArtProperties ArtProperties { get { return artProperties; } }
		#endregion
		protected internal void Read(BinaryReader reader) {
			OfficeArtRecordHeader header = OfficeArtRecordHeader.FromStream(reader);
			CheckHeader(header);
			long endPosition = reader.BaseStream.Position + header.Length;
			while(reader.BaseStream.Position < endPosition) {
				ParseHeader(reader);
			}
		}
		void CheckHeader(OfficeArtRecordHeader header) {
			if(header.Version != OfficeArtVersions.DefaultHeaderVersion ||
				header.InstanceInfo != OfficeArtHeaderInfos.EmptyHeaderInfo ||
				header.TypeCode != OfficeArtTypeCodes.DrawingContainer)
				OfficeArtExceptions.ThrowInvalidContent();
		}
		public void Write(BinaryWriter writer, BinaryWriter embeddedWriter) {
			WriteHeader(writer);
			FileDrawingBlock.Write(writer);
			BlipContainer.Write(writer, embeddedWriter);
			ArtProperties.Write(writer);
		}
		void ParseHeader(BinaryReader reader) {
			OfficeArtRecordHeader header = OfficeArtRecordHeader.FromStream(reader);
			if(header.TypeCode == OfficeArtTypeCodes.FileDrawingGroupRecord) {
				this.fileDrawingBlock = OfficeArtFileDrawingGroupRecordEx.FromStream(reader, header);
				return;
			}
			if(header.TypeCode == OfficeArtTypeCodes.BlipStoreContainer) {
				this.blipContainer = OfficeArtBlipStoreContainerEx.FromStream(reader, reader, header);
				return;
			}
			if(header.TypeCode == OfficeArtTypeCodes.PropertiesTable) {
				this.artProperties = OfficeArtProperties.FromStream(reader, header);
				return;
			}
			reader.BaseStream.Seek(header.Length, SeekOrigin.Current);
		}
	}
	#endregion
	#region OfficeArtFileDrawingGroupRecordEx
	public class OfficeArtFileIdCluster {
		public const int Size = 8;
		public int ClusterId { get; set; }
		public int LargestShapeId { get; set; }
		public static OfficeArtFileIdCluster FromStream(BinaryReader reader) {
			OfficeArtFileIdCluster result = new OfficeArtFileIdCluster();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			ClusterId = reader.ReadInt32();
			LargestShapeId = reader.ReadInt32();
		}
		public void Write(BinaryWriter writer) {
			writer.Write(ClusterId);
			writer.Write(LargestShapeId);
		}
	}
	public class OfficeArtFileDrawingGroupRecordEx : OfficeDrawingPartBase {
		public static OfficeArtFileDrawingGroupRecordEx FromStream(BinaryReader reader, OfficeArtRecordHeader header) {
			OfficeArtFileDrawingGroupRecordEx result = new OfficeArtFileDrawingGroupRecordEx();
			result.Read(reader, header);
			return result;
		}
		#region Fields
		const int headerVersion = 0x0;
		const int headerInstanceInfo = 0x000;
		const int headerTypeCode = 0xf006;
		const int basePartSize = 0x10;
		int maxShapeId;
		int totalShapes;
		int totalDrawings;
		readonly List<OfficeArtFileIdCluster> clusters = new List<OfficeArtFileIdCluster>();
		#endregion
		#region Properties
		public override int HeaderInstanceInfo { get { return headerInstanceInfo; } }
		public override int HeaderTypeCode { get { return headerTypeCode; } }
		public override int HeaderVersion { get { return headerVersion; } }
		public int MaxShapeId { 
			get { return maxShapeId; }
			set {
				ValueChecker.CheckValue(value, 0, 0x03ffd7ff, "Max shape id");
				maxShapeId = value;
			}
		}
		public int TotalShapes {
			get { return totalShapes; }
			set {
				ValueChecker.CheckValue(value, 0, int.MaxValue, "TotalShapes");
				totalShapes = value;
			}
		}
		public int TotalDrawings {
			get { return totalDrawings; }
			set {
				ValueChecker.CheckValue(value, 0, int.MaxValue, "TotalDrawings");
				totalDrawings = value;
			}
		}
		public List<OfficeArtFileIdCluster> Clusters { get { return clusters; } }
		#endregion
		protected internal void Read(BinaryReader reader, OfficeArtRecordHeader header) {
			MaxShapeId = reader.ReadInt32();
			int clustersCount = reader.ReadInt32() - 1;
			if(header.Length != (clustersCount * OfficeArtFileIdCluster.Size) + basePartSize)
				OfficeArtExceptions.ThrowInvalidContent();
			TotalShapes = reader.ReadInt32();
			TotalDrawings = reader.ReadInt32();
			for(int i = 0; i < clustersCount; i++)
				Clusters.Add(OfficeArtFileIdCluster.FromStream(reader));
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(MaxShapeId);
			int clustersCount = Clusters.Count;
			writer.Write(clustersCount + 1);
			writer.Write(TotalShapes);
			writer.Write(TotalDrawings);
			for(int i = 0; i < clustersCount; i++)
				Clusters[i].Write(writer);
		}
		protected override int GetSize() {
			return basePartSize + Clusters.Count * OfficeArtFileIdCluster.Size;
		}
	}
	#endregion
	#region IOfficeArtRecordFactory
	public interface IOfficeArtRecordFactory {
		OfficeDrawingPartBase Create(BinaryReader reader, OfficeArtRecordHeader header, XlsContentBuilder contentBuilder);
	}
	#endregion
	#region OfficeArtRecordFactoryBase
	public abstract class OfficeArtRecordFactoryBase : IOfficeArtRecordFactory {
		readonly Dictionary<int, Type> products;
		protected OfficeArtRecordFactoryBase() {
			products = new Dictionary<int, Type>();
			Initialize();
		}
		protected virtual void Initialize() {
			AddProduct(OfficeArtTypeCodes.ShapeGroupCoordinateSystem, typeof(OfficeArtShapeGroupCoordinateSystemEx));
			AddProduct(OfficeArtTypeCodes.FileShape, typeof(OfficeArtShapeRecord));
			AddProduct(OfficeArtTypeCodes.PropertiesTable, typeof(OfficeArtProperties));
			AddProduct(OfficeArtTypeCodes.TertiaryPropertiesTable, typeof(OfficeArtTertiaryProperties));
			AddProduct(OfficeArtTypeCodes.ClientData, typeof(OfficeArtClientData));
			AddProduct(OfficeArtTypeCodes.ClientTextbox, typeof(OfficeArtClientTextbox));
			AddProduct(OfficeArtTypeCodes.ChildAnchor, typeof(OfficeArtChildAnchor));
		}
		protected void AddProduct(int typeCode, Type prodType) {
			products.Add(typeCode, prodType);
		}
		#region IOfficeArtRecordFactory Members
		public OfficeDrawingPartBase Create(BinaryReader reader, OfficeArtRecordHeader header, XlsContentBuilder contentBuilder) {
			Guard.ArgumentNotNull(reader, "reader");
			Guard.ArgumentNotNull(header, "header");
			try {
				if(!products.ContainsKey(header.TypeCode))
					return null;
				Type prodType = products[header.TypeCode];
				MethodInfo method = prodType.GetMethod("FromStream", new Type[] { typeof(BinaryReader), typeof(OfficeArtRecordHeader) });
				if(method != null && method.IsStatic)
					return method.Invoke(null, new object[] { reader, header }) as OfficeDrawingPartBase;
				method = prodType.GetMethod("FromStream", new Type[] { typeof(BinaryReader), typeof(XlsContentBuilder) });
				if(method != null && method.IsStatic)
					return method.Invoke(null, new object[] { reader, contentBuilder }) as OfficeDrawingPartBase;
				method = prodType.GetMethod("FromStream", new Type[] { typeof(BinaryReader) });
				if(method != null && method.IsStatic)
					return method.Invoke(null, new object[] { reader }) as OfficeDrawingPartBase;
			}
			catch { }
			return null;
		}
		#endregion
	}
	#endregion
	#region OfficeArtRecordFactorySheet
	public class OfficeArtRecordFactorySheet : OfficeArtRecordFactoryBase {
		protected override void Initialize() {
			base.Initialize();
			AddProduct(OfficeArtTypeCodes.ClientAnchor, typeof(OfficeArtClientAnchorSheet));
		}
	}
	#endregion
	#region OfficeArtRecordFactoryChart
	public class OfficeArtRecordFactoryChart : OfficeArtRecordFactoryBase {
		protected override void Initialize() {
			base.Initialize();
			AddProduct(OfficeArtTypeCodes.ClientAnchor, typeof(OfficeArtClientAnchorChart));
		}
	}
	#endregion
	#region OfficeArtRecordFactoryHeaderFooter
	public class OfficeArtRecordFactoryHeaderFooter : OfficeArtRecordFactoryBase {
		protected override void Initialize() {
			base.Initialize();
			AddProduct(OfficeArtTypeCodes.ClientAnchor, typeof(OfficeArtClientAnchorHeaderFooter));
		}
	}
	#endregion
	#region OfficeArtShapeCoordinateSystemEx
	public class OfficeArtShapeGroupCoordinateSystemEx : OfficeArtShapeGroupCoordinateSystem {
		public static OfficeArtShapeGroupCoordinateSystem FromStream(BinaryReader reader) {
			OfficeArtShapeGroupCoordinateSystemEx result = new OfficeArtShapeGroupCoordinateSystemEx();
			result.Read(reader);
			return result;
		}
		protected internal void Read(BinaryReader reader) {
			Left = reader.ReadInt32();
			Top = reader.ReadInt32();
			Right = reader.ReadInt32();
			Bottom = reader.ReadInt32();
		}
	}
	#endregion
	#region OfficeArtDrawingObjectsContainer
	public class OfficeArtDrawingObjectsContainer : CompositeOfficeDrawingPartBase {
		#region Fields
		IOfficeArtRecordFactory factory;
		XlsContentBuilder contentBuilder;
		OfficeArtFileDrawingRecord drawingData;
		OfficeArtShapeGroupContainer shapeGroup;
		int contentLength;
		int contentPosition;
		#endregion
		public static OfficeArtDrawingObjectsContainer FromStream(BinaryReader reader, IOfficeArtRecordFactory factory, XlsContentBuilder contentBuilder) {
			OfficeArtDrawingObjectsContainer result = new OfficeArtDrawingObjectsContainer(factory, contentBuilder);
			result.Read(reader);
			return result;
		}
		protected OfficeArtDrawingObjectsContainer(IOfficeArtRecordFactory factory, XlsContentBuilder contentBuilder) {
			this.factory = factory;
			this.contentBuilder = contentBuilder;
		}
		public OfficeArtDrawingObjectsContainer(int drawingId) {
			this.drawingData = new OfficeArtFileDrawingRecord(drawingId);
			this.shapeGroup = new OfficeArtShapeGroupContainer();
			Items.Add(this.drawingData);
			Items.Add(this.shapeGroup);
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.DrawingObjectsContainer; } }
		public override int HeaderVersion { get { return OfficeArtVersions.DefaultHeaderVersion; } }
		public OfficeArtFileDrawingRecord DrawingData { get { return drawingData; } }
		public OfficeArtShapeGroupContainer ShapeGroup { get { return shapeGroup; } }
		#endregion
		protected internal void Read(BinaryReader reader) {
			OfficeArtRecordHeader header = OfficeArtRecordHeader.FromStream(reader);
			CheckHeader(header);
			this.contentPosition = 0;
			this.contentLength = header.Length;
			while(this.contentPosition < this.contentLength) {
				ParseHeader(reader);
			}
		}
		void CheckHeader(OfficeArtRecordHeader header) {
			if((header.Version != 0 && header.Version != OfficeArtVersions.DefaultHeaderVersion) ||
				header.InstanceInfo != OfficeArtHeaderInfos.EmptyHeaderInfo ||
				header.TypeCode != OfficeArtTypeCodes.DrawingObjectsContainer)
				OfficeArtExceptions.ThrowInvalidContent();
		}
		void ParseHeader(BinaryReader reader) {
			OfficeArtRecordHeader header = OfficeArtRecordHeader.FromStream(reader);
			if(header.TypeCode == OfficeArtTypeCodes.FileDrawingGroupRecord)
				this.drawingData = OfficeArtFileDrawingRecord.FromStream(reader, header);
			else if(header.TypeCode == OfficeArtTypeCodes.ShapeGroupContainer)
				this.shapeGroup = OfficeArtShapeGroupContainer.FromStream(reader, header, this.factory, this.contentBuilder);
			else
				reader.BaseStream.Seek(header.Length, SeekOrigin.Current);
			this.contentPosition += header.Length + 8;
		}
	}
	#endregion
	#region OfficeArtShapeGroupContainer
	public class OfficeArtShapeGroupContainer : CompositeOfficeDrawingPartBase {
		IOfficeArtRecordFactory factory;
		XlsContentBuilder contentBuilder;
		OfficeArtTopmostShapeContainer topmostShape;
		int contentLength;
		int contentPosition;
		public static OfficeArtShapeGroupContainer FromStream(BinaryReader reader, OfficeArtRecordHeader header, IOfficeArtRecordFactory factory, XlsContentBuilder contentBuilder) {
			OfficeArtShapeGroupContainer result = new OfficeArtShapeGroupContainer(factory, contentBuilder);
			result.Read(reader, header);
			return result;
		}
		protected OfficeArtShapeGroupContainer(IOfficeArtRecordFactory factory, XlsContentBuilder contentBuilder) {
			this.factory = factory;
			this.contentBuilder = contentBuilder;
		}
		public OfficeArtShapeGroupContainer() {
			this.topmostShape = new OfficeArtTopmostShapeContainer();
			Items.Add(this.topmostShape);
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.ShapeGroupContainer; } }
		public override int HeaderVersion { get { return OfficeArtVersions.DefaultHeaderVersion; } }
		public OfficeArtTopmostShapeContainer TopmostShape { get { return topmostShape; } }
		#endregion
		protected internal void Read(BinaryReader reader, OfficeArtRecordHeader header) {
			this.contentPosition = 0;
			this.contentLength = header.Length;
			while(this.contentPosition < this.contentLength) {
				Items.Add(ReadShapeContainer(reader));
			}
		}
		OfficeDrawingPartBase ReadShapeContainer(BinaryReader reader) {
			OfficeArtRecordHeader header = OfficeArtRecordHeader.FromStream(reader);
			this.contentPosition += header.Length + 8;
			int typeCode = header.TypeCode;
			if(typeCode == OfficeArtTypeCodes.ShapeGroupContainer)
				return OfficeArtShapeGroupContainer.FromStream(reader, header, this.factory, this.contentBuilder);
			if(typeCode == OfficeArtTypeCodes.ShapeContainer)
				return OfficeArtShapeContainer.FromStream(reader, header, this.factory, this.contentBuilder);
			OfficeArtExceptions.ThrowInvalidContent();
			return null;
		}
	}
	#endregion
	#region OfficeArtShapeContainer
	public class OfficeArtShapeContainer : CompositeOfficeDrawingPartBase {
		#region Fields
		XlsContentBuilder contentBuilder;
		IOfficeArtRecordFactory factory;
		int contentLength;
		int contentPosition;
		#endregion
		public static OfficeArtShapeContainer FromStream(BinaryReader reader, OfficeArtRecordHeader header, IOfficeArtRecordFactory factory, XlsContentBuilder contentBuilder) {
			OfficeArtShapeContainer result = new OfficeArtShapeContainer(factory, contentBuilder);
			result.Read(reader, header);
			return result;
		}
		protected OfficeArtShapeContainer(IOfficeArtRecordFactory factory, XlsContentBuilder contentBuilder) {
			this.factory = factory;
			this.contentBuilder = contentBuilder;
		}
		public OfficeArtShapeContainer() {
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.ShapeContainer; } }
		public override int HeaderVersion { get { return OfficeArtVersions.DefaultHeaderVersion; } }
		public OfficeArtShapeGroupCoordinateSystem CoordinateSystem { 
			get { return FindPart(OfficeArtTypeCodes.ShapeGroupCoordinateSystem) as OfficeArtShapeGroupCoordinateSystem; }
		}
		public OfficeArtShapeRecord ShapeRecord { 
			get { return FindPart(OfficeArtTypeCodes.FileShape) as OfficeArtShapeRecord; }
		}
		public OfficeArtProperties ArtProperties { 
			get { return FindPart(OfficeArtTypeCodes.PropertiesTable) as OfficeArtProperties; }
		}
		public OfficeArtTertiaryProperties TertiaryArtProperties {
			get { return FindPart(OfficeArtTypeCodes.TertiaryPropertiesTable) as OfficeArtTertiaryProperties; }
		}
		public OfficeArtClientAnchorBase ClientAnchor {
			get { return FindPart(OfficeArtTypeCodes.ClientAnchor) as OfficeArtClientAnchorBase; }
		}
		public OfficeArtChildAnchor ChildAnchor {
			get { return FindPart(OfficeArtTypeCodes.ChildAnchor) as OfficeArtChildAnchor; }
		}
		public OfficeArtClientData ClientData {
			get { return FindPart(OfficeArtTypeCodes.ClientData) as OfficeArtClientData; }
		}
		public OfficeArtClientTextbox ClientTextbox {
			get { return FindPart(OfficeArtTypeCodes.ClientTextbox) as OfficeArtClientTextbox; }
		}
		public bool IsDeleted {
			get {
				OfficeArtShapeRecord shapeRecord = ShapeRecord;
				if(shapeRecord != null)
					return shapeRecord.IsDeleted;
				return false;
			}
		}
		public int ShapeId {
			get {
				OfficeArtShapeRecord shapeRecord = ShapeRecord;
				if(shapeRecord != null)
					return shapeRecord.ShapeIdentifier;
				return 0;
			}
		}
		public string ShapeName {
			get {
				OfficeArtProperties artProperties = ArtProperties;
				if(artProperties != null)
					return artProperties.ShapeName;
				return string.Empty;
			}
		}
		public int BlipIndex {
			get {
				OfficeArtProperties artProperties = ArtProperties;
				if(artProperties != null)
					return artProperties.BlipIndex;
				return 0;
			}
		}
		#endregion
		protected internal void Read(BinaryReader reader, OfficeArtRecordHeader header) {
			this.contentPosition = 0;
			this.contentLength = header.Length;
			while(this.contentPosition < this.contentLength) {
				ParseHeader(reader);
			}
		}
		void ParseHeader(BinaryReader reader) {
			OfficeArtRecordHeader header = OfficeArtRecordHeader.FromStream(reader);
			this.contentPosition += header.Length + 8;
			OfficeDrawingPartBase item = this.factory.Create(reader, header, this.contentBuilder);
			if(item != null)
				Items.Add(item);
			else
				reader.BaseStream.Seek(header.Length, SeekOrigin.Current);
		}
		OfficeDrawingPartBase FindPart(int headerTypeCode) {
			foreach(OfficeDrawingPartBase item in Items) {
				if(item.HeaderTypeCode == headerTypeCode)
					return item;
			}
			return null;
		}
	}
	#endregion
	#region OfficeArtTopmostShapeContainer
	public class OfficeArtTopmostShapeContainer : OfficeDrawingPartBase {
		#region Fields
		const int recordLength = 0x28;
		const int topmostShapeId = 0x400;
		const int topmostShapeFlags = 0x05;
		OfficeArtShapeGroupCoordinateSystem coordinateSystem;
		OfficeArtShapeRecord shapeRecord;
		#endregion
		public OfficeArtTopmostShapeContainer() {
			this.coordinateSystem = new OfficeArtShapeGroupCoordinateSystem();
			InitializeShapeRecord();
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.ShapeContainer; } }
		public override int HeaderVersion { get { return OfficeArtVersions.DefaultHeaderVersion; } }
		public OfficeArtShapeGroupCoordinateSystem CoordinateSystem { get { return coordinateSystem; } }
		public OfficeArtShapeRecord ShapeRecord { get { return shapeRecord; } }
		#endregion
		void InitializeShapeRecord() {
			this.shapeRecord = new OfficeArtShapeRecord(OfficeArtHeaderInfos.NotPrimitiveShape);
			this.shapeRecord.ShapeIdentifier = topmostShapeId;
			this.shapeRecord.Flags = topmostShapeFlags;
		}
		protected override void WriteCore(BinaryWriter writer) {
			CoordinateSystem.Write(writer);
			ShapeRecord.Write(writer);
		}
		protected override int GetSize() {
			return recordLength;
		}
	}
	#endregion
	#region OfficeArtProperties
	public class OfficeArtProperties : OfficeArtPropertiesBase, IOfficeArtProperties {
		public const int DefaultLineWidthInEmu = 0x00002535;
		public static OfficeArtProperties FromStream(BinaryReader reader, OfficeArtRecordHeader header) {
			OfficeArtProperties result = new OfficeArtProperties();
			result.Read(reader, header);
			return result;
		}
		#region Properties
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.PropertiesTable; } }
		#region IOfficeArtProperties members
		public int BlipIndex { get; set; }
		public int TextIndex { get; set; }
		public int ZOrder { get; set; }
		public bool UseTextTop { get; set; }
		public bool UseTextBottom { get; set; }
		public bool UseTextRight { get; set; }
		public bool UseTextLeft { get; set; }
		public int TextTop { get; set; }
		public int TextBottom { get; set; }
		public int TextRight { get; set; }
		public int TextLeft { get; set; }
		public int WrapLeftDistance { get; set; }
		public int WrapRightDistance { get; set; }
		public int WrapTopDistance { get; set; }
		public int WrapBottomDistance { get; set; }
		public bool UseWrapLeftDistance { get; set; }
		public bool UseWrapRightDistance { get; set; }
		public bool UseWrapTopDistance { get; set; }
		public bool UseWrapBottomDistance { get; set; }
		public double CropFromTop { get; set; }
		public double CropFromBottom { get; set; }
		public double CropFromLeft { get; set; }
		public double CropFromRight { get; set; }
		public double Rotation { get; set; }
		public bool IsBehindDoc { get; set; }
		public bool UseIsBehindDoc { get; set; }
		public bool Line { get; set; }
		public bool UseLine { get; set; }
		public bool LayoutInCell { get; set; }
		public bool UseLayoutInCell { get; set; }
		public bool Filled { get; set; }
		public bool UseFilled { get; set; }
		public bool FitShapeToText { get; set; }
		public bool UseFitShapeToText { get; set; }
		public double LineWidth { get; set; }
		public Color LineColor { get; set; }
		public Color FillColor { get; set; }
		#endregion
		public string ShapeName { 
			get { return GetStringPropertyValue(typeof(DrawingShapeName)); } 
		}
		public string ShapeDescription {
			get { return GetStringPropertyValue(typeof(DrawingShapeDescription)); }
		}
		public string BlipName {
			get { return GetStringPropertyValue(typeof(DrawingBlipName)); }
		}
		public LineJoinStyle LineJoinStyle {
			get {
				DrawingLineJoinStyle prop = GetPropertyByType(typeof(DrawingLineJoinStyle)) as DrawingLineJoinStyle;
				if(prop != null)
					return prop.Style;
				return LineJoinStyle.Miter;
			}
		}
		public double LineMiterLimit {
			get {
				DrawingLineMiterLimit prop = GetPropertyByType(typeof(DrawingLineMiterLimit)) as DrawingLineMiterLimit;
				if(prop != null)
					return prop.Value;
				return DrawingLineMiterLimit.DefaultValue;
			}
		}
		public int LineWidthInEMUs {
			get {
				DrawingLineWidth prop = GetPropertyByType(typeof(DrawingLineWidth)) as DrawingLineWidth;
				if(prop != null)
					return prop.Value;
				return DefaultLineWidthInEmu;
			}
		}
		public OutlineCompoundType LineCompoundType {
			get {
				DrawingLineCompoundType prop = GetPropertyByType(typeof(DrawingLineCompoundType)) as DrawingLineCompoundType;
				if(prop != null)
					return prop.CompoundType;
				return OutlineCompoundType.Single;
			}
		}
		public OutlineEndCapStyle LineCapStyle {
			get {
				DrawingLineCapStyle prop = GetPropertyByType(typeof(DrawingLineCapStyle)) as DrawingLineCapStyle;
				if(prop != null)
					return prop.Style;
				return OutlineEndCapStyle.Flat;
			}
		}
		public OutlineDashing LineDashing {
			get {
				DrawingLineDashing prop = GetPropertyByType(typeof(DrawingLineDashing)) as DrawingLineDashing;
				if(prop != null)
					return prop.Dashing;
				return OutlineDashing.Solid;
			}
		}
		public ShapePathType ShapePathType {
			get {
				DrawingGeometryShapePath prop = GetPropertyByType(typeof(DrawingGeometryShapePath)) as DrawingGeometryShapePath;
				return prop != null ? prop.ShapePath : ShapePathType.LinesClosed;
			}
		}
		#endregion
		public override void CreateProperties() {
		}
		public BlackWhiteMode GetBlackAndWhiteMode() {
			DrawingBlackWhiteMode prop = GetPropertyByType(typeof(DrawingBlackWhiteMode)) as DrawingBlackWhiteMode;
			if(prop != null)
				return prop.Mode;
			return BlackWhiteMode.Automatic;
		}
		public DrawingBooleanProtectionProperties GetProtectionProperties() {
			return GetPropertyByType(typeof(DrawingBooleanProtectionProperties)) as DrawingBooleanProtectionProperties;
		}
		public DrawingGroupShapeBooleanProperties GetGroupShapeBooleanProperties() {
			return GetPropertyByType(typeof(DrawingGroupShapeBooleanProperties)) as DrawingGroupShapeBooleanProperties;
		}
		public DrawingShapeBooleanProperties GetShapeBooleanProperties() {
			return GetPropertyByType(typeof(DrawingShapeBooleanProperties)) as DrawingShapeBooleanProperties;
		}
		public DrawingBlipFlags GetBlipFlagsProperties() {
			return GetPropertyByType(typeof(DrawingBlipFlags)) as DrawingBlipFlags;
		}
		public DrawingLineColor GetLineColorProperties() {
			return GetPropertyByType(typeof(DrawingLineColor)) as DrawingLineColor;
		}
		public DrawingFillColor GetFillColorProperties() {
			return GetPropertyByType(typeof(DrawingFillColor)) as DrawingFillColor;
		}
		public DrawingFillOpacity GetFillOpacityProperties() {
			return GetPropertyByType(typeof(DrawingFillOpacity)) as DrawingFillOpacity;
		}
		public DrawingFillBackColor GetFillBackColorProperties() {
			return GetPropertyByType(typeof(DrawingFillBackColor)) as DrawingFillBackColor;
		}
		public DrawingShadowColor GetShadowColorProperties() {
			return GetPropertyByType(typeof(DrawingShadowColor)) as DrawingShadowColor;
		}
		public DrawingShadowStyleBooleanProperties GetShadowStyleBooleanProperties() {
			return GetPropertyByType(typeof(DrawingShadowStyleBooleanProperties)) as DrawingShadowStyleBooleanProperties;
		}
		public DrawingConnectionPointsType GetConnectionPointsType() {
			return GetPropertyByType(typeof(DrawingConnectionPointsType)) as DrawingConnectionPointsType;
		}
		public DrawingTextBooleanProperties GetTextBooleanProperties() {
			return GetPropertyByType(typeof(DrawingTextBooleanProperties)) as DrawingTextBooleanProperties;
		}
		public DrawingTextDirection GetTextDirection() {
			return GetPropertyByType(typeof(DrawingTextDirection)) as DrawingTextDirection;
		}
		public DrawingShapeHyperlink GetHyperlinkProperties() {
			return GetPropertyByType(typeof(DrawingShapeHyperlink)) as DrawingShapeHyperlink;
		}
		public DrawingGeometryVertices GetGeometryVertices() {
			return GetPropertyByType(typeof(DrawingGeometryVertices)) as DrawingGeometryVertices;
		}
		public DrawingGeometrySegmentInfo GetGeometrySegmentInfo() {
			return GetPropertyByType(typeof(DrawingGeometrySegmentInfo)) as DrawingGeometrySegmentInfo;
		}
		public void Merge(OfficeArtProperties commonProperties) {
			if(commonProperties == null) return;
			for(int i = 0; i < commonProperties.Properties.Count; i++) {
				IOfficeDrawingProperty commonProp = commonProperties.Properties[i];
				IOfficeDrawingProperty prop = GetPropertyByType(commonProp.GetType());
				if(prop != null)
					prop.Merge(commonProp);
				else
					this.Properties.Add(commonProp);
			}
			for(int i = 0; i < this.Properties.Count; i++)
				this.Properties[i].Execute(this);
		}
		#region Internals
		string GetStringPropertyValue(Type propertyType) {
			OfficeDrawingStringPropertyValueBase prop = GetPropertyByType(propertyType) as OfficeDrawingStringPropertyValueBase;
			if(prop != null)
				return prop.Data.TrimEnd(new char[] { '\0' });
			return string.Empty;
		}
		protected internal IOfficeDrawingProperty GetPropertyByType(Type propertyType) {
			return OfficeArtPropertiesHelper.GetPropertyByType(this, propertyType);
		}
		#endregion
	}
	#endregion
	#region ShapeTypeCode
	public static class ShapeTypeCode {
		public const int NotPrimitive = 0x00000000;
		public const int Rectangle = 0x00000001;
		public const int RoundRect = 0x00000002;
		public const int Ellipse = 0x00000003;
		public const int Diamond = 0x00000004;
		public const int IsocelesTriangle = 0x00000005;
		public const int RightTriangle = 0x00000006;
		public const int Parallelogram = 0x00000007;
		public const int Trapezoid = 0x00000008;
		public const int Hexagon = 0x00000009;
		public const int Octagon = 0x0000000A;
		public const int Plus = 0x0000000B;
		public const int Star = 0x0000000C;
		public const int Arrow = 0x0000000D;
		public const int ThickArrow = 0x0000000E;
		public const int HomePlate = 0x0000000F;
		public const int Cube = 0x00000010;
		public const int Balloon = 0x00000011;
		public const int Seal = 0x00000012;
		public const int Arc = 0x00000013;
		public const int Line = 0x00000014;
		public const int Plaque = 0x00000015;
		public const int Can = 0x00000016;
		public const int Donut = 0x00000017;
		public const int TextSimple = 0x00000018;
		public const int TextOctagon = 0x00000019;
		public const int TextHexagon = 0x0000001A;
		public const int TextCurve = 0x0000001B;
		public const int TextWave = 0x0000001C;
		public const int TextRing = 0x0000001D;
		public const int TextOnCurve = 0x0000001E;
		public const int TextOnRing = 0x0000001F;
		public const int StraightConnector1 = 0x00000020;
		public const int BentConnector2 = 0x00000021;
		public const int BentConnector3 = 0x00000022;
		public const int BentConnector4 = 0x00000023;
		public const int BentConnector5 = 0x00000024;
		public const int CurvedConnector2 = 0x00000025;
		public const int CurvedConnector3 = 0x00000026;
		public const int CurvedConnector4 = 0x00000027;
		public const int CurvedConnector5 = 0x00000028;
		public const int Callout1 = 0x00000029;
		public const int Callout2 = 0x0000002A;
		public const int Callout3 = 0x0000002B;
		public const int AccentCallout1 = 0x0000002C;
		public const int AccentCallout2 = 0x0000002D;
		public const int AccentCallout3 = 0x0000002E;
		public const int BorderCallout1 = 0x0000002F;
		public const int BorderCallout2 = 0x00000030;
		public const int BorderCallout3 = 0x00000031;
		public const int AccentBorderCallout1 = 0x00000032;
		public const int AccentBorderCallout2 = 0x00000033;
		public const int AccentBorderCallout3 = 0x00000034;
		public const int Ribbon = 0x00000035;
		public const int Ribbon2 = 0x00000036;
		public const int Chevron = 0x00000037;
		public const int Pentagon = 0x00000038;
		public const int NoSmoking = 0x00000039;
		public const int Seal8 = 0x0000003A;
		public const int Seal16 = 0x0000003B;
		public const int Seal32 = 0x0000003C;
		public const int WedgeRectCallout = 0x0000003D;
		public const int WedgeRRectCallout = 0x0000003E;
		public const int WedgeEllipseCallout = 0x0000003F;
		public const int Wave = 0x00000040;
		public const int FoldedCorner = 0x00000041;
		public const int LeftArrow = 0x00000042;
		public const int DownArrow = 0x00000043;
		public const int UpArrow = 0x00000044;
		public const int LeftRightArrow = 0x00000045;
		public const int UpDownArrow = 0x00000046;
		public const int IrregularSeal1 = 0x00000047;
		public const int IrregularSeal2 = 0x00000048;
		public const int LightningBolt = 0x00000049;
		public const int Heart = 0x0000004A;
		public const int PictureFrame = 0x0000004B;
		public const int QuadArrow = 0x0000004C;
		public const int LeftArrowCallout = 0x0000004D;
		public const int RightArrowCallout = 0x0000004E;
		public const int UpArrowCallout = 0x0000004F;
		public const int DownArrowCallout = 0x00000050;
		public const int LeftRightArrowCallout = 0x00000051;
		public const int UpDownArrowCallout = 0x00000052;
		public const int QuadArrowCallout = 0x00000053;
		public const int Bevel = 0x00000054;
		public const int LeftBracket = 0x00000055;
		public const int RightBracket = 0x00000056;
		public const int LeftBrace = 0x00000057;
		public const int RightBrace = 0x00000058;
		public const int LeftUpArrow = 0x00000059;
		public const int BentUpArrow = 0x0000005A;
		public const int BentArrow = 0x0000005B;
		public const int Seal24 = 0x0000005C;
		public const int StripedRightArrow = 0x0000005D;
		public const int NotchedRightArrow = 0x0000005E;
		public const int BlockArc = 0x0000005F;
		public const int SmileyFace = 0x00000060;
		public const int VerticalScroll = 0x00000061;
		public const int HorizontalScroll = 0x00000062;
		public const int CircularArrow = 0x00000063;
		public const int NotchedCircularArrow = 0x00000064;
		public const int UturnArrow = 0x00000065;
		public const int CurvedRightArrow = 0x00000066;
		public const int CurvedLeftArrow = 0x00000067;
		public const int CurvedUpArrow = 0x00000068;
		public const int CurvedDownArrow = 0x00000069;
		public const int CloudCallout = 0x0000006A;
		public const int EllipseRibbon = 0x0000006B;
		public const int EllipseRibbon2 = 0x0000006C;
		public const int FlowChartProcess = 0x0000006D;
		public const int FlowChartDecision = 0x0000006E;
		public const int FlowChartInputOutput = 0x0000006F;
		public const int FlowChartPredefinedProcess = 0x00000070;
		public const int FlowChartInternalStorage = 0x00000071;
		public const int FlowChartDocument = 0x00000072;
		public const int FlowChartMultidocument = 0x00000073;
		public const int FlowChartTerminator = 0x00000074;
		public const int FlowChartPreparation = 0x00000075;
		public const int FlowChartManualInput = 0x00000076;
		public const int FlowChartManualOperation = 0x00000077;
		public const int FlowChartConnector = 0x00000078;
		public const int FlowChartPunchedCard = 0x00000079;
		public const int FlowChartPunchedTape = 0x0000007A;
		public const int FlowChartSummingJunction = 0x0000007B;
		public const int FlowChartOr = 0x0000007C;
		public const int FlowChartCollate = 0x0000007D;
		public const int FlowChartSort = 0x0000007E;
		public const int FlowChartExtract = 0x0000007F;
		public const int FlowChartMerge = 0x00000080;
		public const int FlowChartOfflineStorage = 0x00000081;
		public const int FlowChartOnlineStorage = 0x00000082;
		public const int FlowChartMagneticTape = 0x00000083;
		public const int FlowChartMagneticDisk = 0x00000084;
		public const int FlowChartMagneticDrum = 0x00000085;
		public const int FlowChartDisplay = 0x00000086;
		public const int FlowChartDelay = 0x00000087;
		public const int TextPlainText = 0x00000088;
		public const int TextStop = 0x00000089;
		public const int TextTriangle = 0x0000008A;
		public const int TextTriangleInverted = 0x0000008B;
		public const int TextChevron = 0x0000008C;
		public const int TextChevronInverted = 0x0000008D;
		public const int TextRingInside = 0x0000008E;
		public const int TextRingOutside = 0x0000008F;
		public const int TextArchUpCurve = 0x00000090;
		public const int TextArchDownCurve = 0x00000091;
		public const int TextCircleCurve = 0x00000092;
		public const int TextButtonCurve = 0x00000093;
		public const int TextArchUpPour = 0x00000094;
		public const int TextArchDownPour = 0x00000095;
		public const int TextCirclePour = 0x00000096;
		public const int TextButtonPour = 0x00000097;
		public const int TextCurveUp = 0x00000098;
		public const int TextCurveDown = 0x00000099;
		public const int TextCascadeUp = 0x0000009A;
		public const int TextCascadeDown = 0x0000009B;
		public const int TextWave1 = 0x0000009C;
		public const int TextWave2 = 0x0000009D;
		public const int TextWave3 = 0x0000009E;
		public const int TextWave4 = 0x0000009F;
		public const int TextInflate = 0x000000A0;
		public const int TextDeflate = 0x000000A1;
		public const int TextInflateBottom = 0x000000A2;
		public const int TextDeflateBottom = 0x000000A3;
		public const int TextInflateTop = 0x000000A4;
		public const int TextDeflateTop = 0x000000A5;
		public const int TextDeflateInflate = 0x000000A6;
		public const int TextDeflateInflateDeflate = 0x000000A7;
		public const int TextFadeRight = 0x000000A8;
		public const int TextFadeLeft = 0x000000A9;
		public const int TextFadeUp = 0x000000AA;
		public const int TextFadeDown = 0x000000AB;
		public const int TextSlantUp = 0x000000AC;
		public const int TextSlantDown = 0x000000AD;
		public const int TextCanUp = 0x000000AE;
		public const int TextCanDown = 0x000000AF;
		public const int FlowChartAlternateProcess = 0x000000B0;
		public const int FlowChartOffpageConnector = 0x000000B1;
		public const int Callout90 = 0x000000B2;
		public const int AccentCallout90 = 0x000000B3;
		public const int BorderCallout90 = 0x000000B4;
		public const int AccentBorderCallout90 = 0x000000B5;
		public const int LeftRightUpArrow = 0x000000B6;
		public const int Sun = 0x000000B7;
		public const int Moon = 0x000000B8;
		public const int BracketPair = 0x000000B9;
		public const int BracePair = 0x000000BA;
		public const int Seal4 = 0x000000BB;
		public const int DoubleWave = 0x000000BC;
		public const int ActionButtonBlank = 0x000000BD;
		public const int ActionButtonHome = 0x000000BE;
		public const int ActionButtonHelp = 0x000000BF;
		public const int ActionButtonInformation = 0x000000C0;
		public const int ActionButtonForwardNext = 0x000000C1;
		public const int ActionButtonBackPrevious = 0x000000C2;
		public const int ActionButtonEnd = 0x000000C3;
		public const int ActionButtonBeginning = 0x000000C4;
		public const int ActionButtonReturn = 0x000000C5;
		public const int ActionButtonDocument = 0x000000C6;
		public const int ActionButtonSound = 0x000000C7;
		public const int ActionButtonMovie = 0x000000C8;
		public const int HostControl = 0x000000C9;
		public const int Textbox = 0x000000CA;
	}
	#endregion
	#region OfficeArtPropertiesHelper
	static class OfficeArtPropertiesHelper {
		public static IOfficeDrawingProperty GetPropertyByType(OfficeArtPropertiesBase prop, Type propertyType) {
			for (int i = 0; i < prop.Properties.Count; i++) {
				if (propertyType.IsInstanceOfType(prop.Properties[i]))
					return prop.Properties[i];
			}
			return null;
		}
	}
	#endregion
}
