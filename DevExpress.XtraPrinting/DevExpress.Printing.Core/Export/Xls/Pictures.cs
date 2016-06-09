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
using System.Globalization;
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xls {
	using DevExpress.XtraExport.Implementation;
	using DevExpress.XtraExport.OfficeArt;
	using DevExpress.Utils.Crypt;
#if !SL
	using System.Drawing;
	using System.Drawing.Imaging;
	using DevExpress.Compatibility.System.Drawing.Imaging;
#endif
	#region XlsDataAwareExporter
	public partial class XlsDataAwareExporter {
		int shapeId = 0;
		readonly List<XlsPictureData> drawingGroup = new List<XlsPictureData>();
		XlPicture currentPicture = null;
		public IXlPicture BeginPicture() {
			currentPicture = new XlPicture(this);
			return currentPicture;
		}
		public void EndPicture() {
			if(currentSheet == null)
				throw new InvalidOperationException("No current sheet. Please use BeginPicture/EndPicture inside BeginSheet/EndSheet scope.");
			int blipId = AddToDrawingGroup(currentPicture);
			if(blipId > 0) {
				XlsPictureObject drawingObject = CreateDrawingObject(blipId, currentPicture);
				currentSheet.DrawingObjects.Add(drawingObject);
			}
			currentPicture = null;
		}
		int AddToDrawingGroup(XlPicture picture) {
			if(picture == null || picture.Image == null)
				return -1;
			ImageFormat actualFormat = ReplaceImageFormat(picture.Format);
			byte[] imageBytes;
			try {
				imageBytes = picture.GetImageBytes(actualFormat);
			}
			catch {
				actualFormat = ImageFormat.Png;
				imageBytes = picture.GetImageBytes(actualFormat);
			}
			MD4HashCalculator calculator = new MD4HashCalculator();
			uint[] hash = calculator.InitialCheckSumValue;
			calculator.UpdateCheckSum(hash, imageBytes, 0, imageBytes.Length);
			byte[] imageDigest = MD4HashConverter.ToArray(calculator.GetFinalCheckSum(hash));
			XlsPictureData pictureData;
			for(int i = 0; i < drawingGroup.Count; i++) {
				pictureData = drawingGroup[i];
				if(pictureData.EqualsDigest(imageDigest)) {
					pictureData.NumberOfReferences = pictureData.NumberOfReferences + 1;
					return i + 1;
				}
			}
			pictureData = new XlsPictureData(GetImageFormat(actualFormat), imageBytes, imageDigest);
			drawingGroup.Add(pictureData);
			return drawingGroup.Count;
		}
		XlsImageFormat GetImageFormat(ImageFormat format) {
			if(format == ImageFormat.Jpeg)
				return XlsImageFormat.Jpeg;
			if(format == ImageFormat.Tiff)
				return XlsImageFormat.Tiff;
			return XlsImageFormat.Png;
		}
		ImageFormat ReplaceImageFormat(ImageFormat format) {
			if(format == ImageFormat.Jpeg)
				return format;
			if(format == ImageFormat.Tiff)
				return format;
			return ImageFormat.Png;
		}
		XlsPictureObject CreateDrawingObject(int blipId, XlPicture picture) {
			XlsPictureObject result = new XlsPictureObject();
			result.PictureId = ++shapeId;
			result.BlipId = blipId;
			result.Name = string.IsNullOrEmpty(picture.Name) ? string.Format("Picture{0}", result.PictureId) : picture.Name;
			if(picture.AnchorType == XlAnchorType.Absolute) {
				result.AnchorType = XlAnchorType.TwoCell;
				result.AnchorBehavior = XlAnchorType.Absolute;
			}
			else if(picture.AnchorType == XlAnchorType.OneCell) {
				result.AnchorType = XlAnchorType.TwoCell;
				result.AnchorBehavior = XlAnchorType.OneCell;
			}
			else {
				result.AnchorType = picture.AnchorType;
				result.AnchorBehavior = picture.AnchorBehavior;
			}
			result.TopLeft = picture.TopLeft;
			result.BottomRight = picture.BottomRight;
			result.HyperlinkClick = picture.HyperlinkClick.Clone();
			return result;
		}
		XlAnchorPoint CalculateAnchorPoint(XlAnchorPoint source) {
			int column = source.Column;
			int row = source.Row;
			int columnOffsetInPixels = source.ColumnOffsetInPixels;
			int rowOffsetInPixels = source.RowOffsetInPixels;
			int cellWidth = GetCellWidth(column);
			int cellHeight = GetCellHeight(row);
			int maxColumnIndex = options.MaxColumnCount - 1;
			int maxRowIndex = options.MaxRowCount - 1;
			while(columnOffsetInPixels < 0 && column > 0) {
				column--;
				cellWidth = GetCellWidth(column);
				columnOffsetInPixels += cellWidth;
			}
			while((columnOffsetInPixels >= cellWidth) && (column < maxColumnIndex)) {
				columnOffsetInPixels -= cellWidth;
				column++;
				cellWidth = GetCellWidth(column);
			}
			while(rowOffsetInPixels < 0 && row > 0) {
				row--;
				cellHeight = GetCellHeight(row);
				rowOffsetInPixels += cellHeight;
			}
			while((rowOffsetInPixels >= cellHeight) && (row < maxRowIndex)) {
				rowOffsetInPixels -= cellHeight;
				row++;
				cellHeight = GetCellHeight(row);
			}
			return new XlAnchorPoint(column, row, columnOffsetInPixels, rowOffsetInPixels, cellWidth, cellHeight);
		}
		int GetCellHeight(int row) {
			int rowHeight = -1;
			if(currentSheet.RowHeights.ContainsKey(row))
				rowHeight = currentSheet.RowHeights[row];
			if(rowHeight < 0)
				rowHeight = (int)currentSheet.DefaultRowHeightInPixels; 
			return rowHeight;
		}
		int GetCellWidth(int column) {
			int columnWidth = -1;
			if(currentSheet.ColumnsTable.ContainsKey(column)) {
				IXlColumn item = currentSheet.ColumnsTable[column];
				columnWidth = item.WidthInPixels;
				if(item.IsHidden)
					columnWidth = 0;
			}
			if(columnWidth < 0)
				columnWidth = (int)currentSheet.DefaultColumnWidthInPixels; 
			return columnWidth;
		}
		#region MsoDrawingGroup
		void WriteDrawingGroup() {
			if(drawingGroup.Count == 0 && !ShouldExportShapes())
				return;
			XlsChunk firstChunk = new XlsChunk(XlsRecordType.MsoDrawingGroup);
			XlsChunk nextChunk = new XlsChunk(XlsRecordType.Continue);
			using(XlsChunkWriter chunkWriter = new XlsChunkWriter(writer, firstChunk, nextChunk)) {
				OfficeArtDrawingGroupContainer container = CreateMsoDrawingGroup();
				container.Write(chunkWriter);
			}
		}
		OfficeArtDrawingGroupContainer CreateMsoDrawingGroup() {
			OfficeArtDrawingGroupContainer result = new OfficeArtDrawingGroupContainer();
			result.Items.Add(CreateFileDrawingBlock());
			if(drawingGroup.Count > 0)
				result.Items.Add(CreateBlipStore());
			return result;
		}
		OfficeArtFileDrawingGroupRecord CreateFileDrawingBlock() {
			OfficeArtFileDrawingGroupRecord result = new OfficeArtFileDrawingGroupRecord();
			int maxShapedId = 0;
			int totalShapes = 0;
			int totalDrawings = 0;
			for(int i = 0; i < sheets.Count; i++) {
				XlsTableBasedDocumentSheet sheet = sheets[i];
				int count = sheet.DrawingObjects.Count + CountShapes(sheet as IXlShapeContainer);
				if(count > 0) {
					totalShapes += count + 1;
					totalDrawings++;
				}
				int clusterMaxShapeId = count + 1;
				maxShapedId = Math.Max(maxShapedId, GetTopmostShapeId(sheet.SheetId) + clusterMaxShapeId);
				OfficeArtFileIdCluster cluster = new OfficeArtFileIdCluster();
				cluster.ClusterId = sheet.SheetId;
				cluster.LargestShapeId = clusterMaxShapeId;
				result.Clusters.Add(cluster);
			}
			result.MaxShapeId = maxShapedId;
			result.TotalShapes = totalShapes;
			result.TotalDrawings = totalDrawings;
			return result;
		}
		OfficeArtBlipStoreContainer CreateBlipStore() {
			OfficeArtBlipStoreContainer result = new OfficeArtBlipStoreContainer();
			foreach(XlsPictureData pictureData in drawingGroup) {
				OfficeArtBlipImage blip = new OfficeArtBlipImage(pictureData);
				result.Items.Add(new OfficeArtBlipStoreFileBlock(blip));
			}
			return result;
		}
		#endregion
		#region MsoDrawing
		void WriteDrawingObjects() {
			if(currentSheet.DrawingObjects.Count == 0 && !ShouldExportShapes(currentSheet as IXlShapeContainer))
				return;
			CalculatePicturesAnchorPoints();
			CalculateShapesAnchorPoints();
			XlsChunk firstChunk = new XlsChunk(XlsRecordType.MsoDrawing);
			XlsChunk nextChunk = new XlsChunk(XlsRecordType.Continue);
			using(XlsChunkWriter chunkWriter = new XlsChunkWriter(writer, firstChunk, nextChunk)) {
				OfficeArtDrawingContainer drawingContainer = CreateMsoDrawing();
				drawingContainer.Write(chunkWriter);
			}
		}
		void CalculatePicturesAnchorPoints() {
			foreach(XlsPictureObject pictureObject in currentSheet.DrawingObjects) {
				pictureObject.TopLeft = CalculateAnchorPoint(pictureObject.TopLeft);
				pictureObject.BottomRight = CalculateAnchorPoint(pictureObject.BottomRight);
			}
		}
		void CalculateShapesAnchorPoints() {
			IXlShapeContainer container = currentSheet as IXlShapeContainer;
			if(container == null)
				return;
			foreach(XlShape shape in container.Shapes) {
				shape.TopLeft = CalculateAnchorPoint(shape.TopLeft);
				shape.BottomRight = CalculateAnchorPoint(shape.BottomRight);
			}
		}
		OfficeArtDrawingContainer CreateMsoDrawing() {
			OfficeArtDrawingContainer result = new OfficeArtDrawingContainer();
			result.Items.Add(CreateFileDrawingRecord());
			result.Items.Add(CreateShapeGroup());
			return result;
		}
		OfficeArtFileDrawingRecord CreateFileDrawingRecord() {
			int topmostShapeId = GetTopmostShapeId(currentSheet.SheetId);
			int count = currentSheet.DrawingObjects.Count + CountShapes(currentSheet as IXlShapeContainer);
			int lastShapeId = topmostShapeId + count;
			return new OfficeArtFileDrawingRecord(currentSheet.SheetId, count + 1, lastShapeId);
		}
		int GetTopmostShapeId(int sheetId) {
			return ((sheetId - 1) % 16 + 1) * 0x400;
		}
		OfficeArtShapeGroupContainer CreateShapeGroup() {
			OfficeArtShapeGroupContainer result = new OfficeArtShapeGroupContainer();
			result.Items.Add(CreateTopmostShape());
			foreach(XlsPictureObject pictureObject in currentSheet.DrawingObjects)
				result.Items.Add(CreateShape(pictureObject));
			IXlShapeContainer container = currentSheet as IXlShapeContainer;
			if(container != null) {
				foreach(XlShape shape in container.Shapes)
					if(shape.GeometryPreset == XlGeometryPreset.Line) {
						shape.Id = ++shapeId;
						if(string.IsNullOrEmpty(shape.Name))
							shape.Name = string.Format("Straight Connector {0}", shapeId);
						result.Items.Add(CreateShape(shape));
					}
			}
			return result;
		}
		OfficeArtShapeContainer CreateTopmostShape() {
			OfficeArtShapeContainer result = new OfficeArtShapeContainer();
			result.Items.Add(new OfficeArtShapeGroupCoordinateSystem());
			result.Items.Add(new OfficeArtShapeRecord(0, GetTopmostShapeId(currentSheet.SheetId), 0x0005));
			return result;
		}
		OfficeArtShapeContainer CreateShape(XlsPictureObject pictureObject) {
			OfficeArtShapeContainer result = new OfficeArtShapeContainer();
			result.Items.Add(new OfficeArtShapeRecord(0x004b, GetTopmostShapeId(currentSheet.SheetId) + pictureObject.PictureId, 0x0A00));
			result.Items.Add(CreateShapeProperties(pictureObject));
			OfficeArtTertiaryProperties tertiaryProperties = CreateTertiaryProperties(pictureObject);
			if(tertiaryProperties != null)
				result.Items.Add(tertiaryProperties);
			result.Items.Add(new OfficeArtClientAnchorSheet(pictureObject));
			result.Items.Add(new OfficeArtPictureClientData(pictureObject));
			return result;
		}
		OfficeArtProperties CreateShapeProperties(XlsPictureObject pictureObject) {
			OfficeArtProperties result = new OfficeArtProperties();
			result.Properties.Add(new OfficeArtIntProperty(0x007f, 0x01fb0080)); 
			result.Properties.Add(new OfficeArtIntProperty(0x4104, pictureObject.BlipId)); 
			result.Properties.Add(new OfficeArtIntProperty(0x013f, 0x00060000)); 
			result.Properties.Add(new OfficeArtIntProperty(0x01bf, 0x00100000)); 
			result.Properties.Add(new OfficeArtIntProperty(0x01ff, 0x00180000)); 
			result.Properties.Add(new OfficeArtIntProperty(0x033f, 0x00180010)); 
			result.Properties.Add(new OfficeArtStringProperty(0xc380, pictureObject.Name)); 
			byte[] hyperlinkData = GetPictureHyperlinkData(pictureObject);
			if (hyperlinkData != null)
				result.Properties.Add(new OfficeArtDataProperty(0xc382, hyperlinkData));
			result.Properties.Add(new OfficeArtIntProperty(0x03bf, 0x00200000)); 
			return result;
		}
		byte[] GetPictureHyperlinkData(XlsPictureObject picture) {
			if(picture.HyperlinkClick == null || string.IsNullOrEmpty(picture.HyperlinkClick.TargetUri))
				return null;
			XlsHyperlinkObject hyperlink = new XlsHyperlinkObject();
			string targetUri = picture.HyperlinkClick.TargetUri;
			if(picture.HyperlinkClick.IsExternal) {
				string location = string.Empty;
				int pos = targetUri.IndexOf('#');
				if(pos != -1) {
					location = targetUri.Substring(pos + 1);
					targetUri = targetUri.Substring(0, pos);
				}
				Uri uri;
				if(!Uri.TryCreate(targetUri, UriKind.RelativeOrAbsolute, out uri))
					return null;
				hyperlink.HasMoniker = true;
				hyperlink.IsAbsolute = uri.IsAbsoluteUri;
				if(uri.IsAbsoluteUri && uri.Scheme != "file") {
					XlsHyperlinkURLMoniker urlMoniker = new XlsHyperlinkURLMoniker();
					urlMoniker.Url = targetUri;
					urlMoniker.HasOptionalData = true;
					urlMoniker.AllowImplicitFileScheme = true;
					urlMoniker.AllowRelative = true;
					urlMoniker.Canonicalize = true;
					urlMoniker.CrackUnknownSchemes = true;
					urlMoniker.DecodeExtraInfo = true;
					urlMoniker.IESettings = true;
					urlMoniker.NoEncodeForbiddenChars = true;
					urlMoniker.PreProcessHtmlUri = true;
					hyperlink.OleMoniker = urlMoniker;
				}
				else {
					XlsHyperlinkFileMoniker fileMoniker = new XlsHyperlinkFileMoniker();
					fileMoniker.Path = targetUri;
					hyperlink.OleMoniker = fileMoniker;
				}
				if(!string.IsNullOrEmpty(location)) {
					hyperlink.Location = location;
					hyperlink.HasLocationString = true;
				}
			}
			else {
				hyperlink.Location = targetUri.TrimStart(new char[] { '#' });
				hyperlink.HasLocationString = true;
			}
			if(!string.IsNullOrEmpty(picture.HyperlinkClick.TargetFrame)) {
				hyperlink.FrameName = picture.HyperlinkClick.TargetFrame;
				hyperlink.HasFrameName = true;
			}
			return hyperlink.GetHyperlinkData();
		}
		OfficeArtTertiaryProperties CreateTertiaryProperties(XlsPictureObject picture) {
			if(picture.HyperlinkClick == null ||
				string.IsNullOrEmpty(picture.HyperlinkClick.TargetUri) ||
				string.IsNullOrEmpty(picture.HyperlinkClick.Tooltip))
				return null;
			OfficeArtTertiaryProperties result = new OfficeArtTertiaryProperties();
			result.Properties.Add(new OfficeArtStringProperty(0xc38d, picture.HyperlinkClick.Tooltip));
			return result;
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraExport.OfficeArt {
	using DevExpress.XtraExport.Xls;
	using DevExpress.XtraExport.Implementation;
	#region OfficeArtPartBase
	abstract class OfficeArtPartBase {
		#region Properties
		public abstract int HeaderVersion { get; }
		public abstract int HeaderInstanceInfo { get; }
		public abstract int HeaderTypeCode { get; }
		public virtual int Length { get { return GetSize(); } }
		#endregion
		public void Write(BinaryWriter writer) {
			if(!ShouldWrite())
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
	#region OfficeArtCompositePartBase
	abstract class OfficeArtCompositePartBase : OfficeArtPartBase {
		readonly List<OfficeArtPartBase> items = new List<OfficeArtPartBase>();
		public List<OfficeArtPartBase> Items { get { return items; } }
		protected internal override void WriteCore(BinaryWriter writer) {
			int count = Items.Count;
			for(int i = 0; i < count; i++) {
				Items[i].Write(writer);
			}
		}
		protected internal override int GetSize() {
			int size = 0;
			int count = Items.Count;
			for(int i = 0; i < count; i++) {
				if(Items[i].ShouldWrite()) {
					size += OfficeArtRecordHeader.Size;
					size += Items[i].GetSize();
				}
			}
			return size;
		}
	}
	#endregion
	#region OfficeArtRecordHeader
	class OfficeArtRecordHeader {
		public const int Size = 8;
		public int Version { get; set; }
		public int InstanceInfo { get; set; }
		public int TypeCode { get; set; }
		public int Length { get; set; }
		public void Write(BinaryWriter writer) {
			ushort flags = (ushort)(Version | (InstanceInfo << 4));
			writer.Write(flags);
			writer.Write((ushort)TypeCode);
			writer.Write(Length);
		}
	}
	#endregion
	#region OfficeArtDrawingGroupContainer (OfficeArtDggContainer)
	class OfficeArtDrawingGroupContainer : OfficeArtCompositePartBase {
		public override int HeaderInstanceInfo { get { return 0x0000; } }
		public override int HeaderTypeCode { get { return 0xf000; } }
		public override int HeaderVersion { get { return 0x000f; } }
	}
	#endregion
	#region OfficeArtFileDrawingGroupRecord (OfficeArtFDGGBlock)
	class OfficeArtFileIdCluster {
		public const int Size = 8;
		public int ClusterId { get; set; }
		public int LargestShapeId { get; set; }
		public void Write(BinaryWriter writer) {
			writer.Write(ClusterId);
			writer.Write(LargestShapeId);
		}
	}
	class OfficeArtFileDrawingGroupRecord : OfficeArtPartBase {
		readonly List<OfficeArtFileIdCluster> clusters = new List<OfficeArtFileIdCluster>();
		#region Properties
		public override int HeaderInstanceInfo { get { return 0x0000; } }
		public override int HeaderTypeCode { get { return 0xf006; } }
		public override int HeaderVersion { get { return 0x0000; } }
		public int MaxShapeId { get; set; }
		public int TotalShapes { get; set; }
		public int TotalDrawings { get; set; }
		public List<OfficeArtFileIdCluster> Clusters { get { return clusters; } }
		#endregion
		protected internal override void WriteCore(BinaryWriter writer) {
			writer.Write(MaxShapeId);
			int clustersCount = Clusters.Count;
			writer.Write(clustersCount + 1);
			writer.Write(TotalShapes);
			writer.Write(TotalDrawings);
			for(int i = 0; i < clustersCount; i++)
				Clusters[i].Write(writer);
		}
		protected internal override int GetSize() {
			return 0x10 + Clusters.Count * OfficeArtFileIdCluster.Size;
		}
	}
	#endregion
	#region OfficeArtBlipStoreContainer
	class OfficeArtBlipStoreContainer : OfficeArtCompositePartBase {
		#region Properties
		public override int HeaderInstanceInfo { get { return Items.Count; } }
		public override int HeaderTypeCode { get { return 0xf001; } }
		public override int HeaderVersion { get { return 0x000f; } }
		#endregion
	}
	#endregion
	#region OfficeArtBlipStoreFileBlock
	class OfficeArtBlipStoreFileBlock : OfficeArtPartBase {
		readonly OfficeArtBlipBase blip;
		public OfficeArtBlipStoreFileBlock(OfficeArtBlipBase blip)
			: base() {
			Guard.ArgumentNotNull(blip, "blip");
			this.blip = blip;
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return blip.BlipType; } }
		public override int HeaderTypeCode { get { return 0xf007; } }
		public override int HeaderVersion { get { return 0x0002; } }
		protected internal OfficeArtBlipBase Blip { get { return blip; } }
		#endregion
		protected internal override void WriteCore(BinaryWriter writer) {
			byte blipType = blip.BlipType;
			writer.Write(blipType); 
			writer.Write(blipType); 
			writer.Write(blip.Digest); 
			writer.Write((ushort)0x00ff); 
			writer.Write((int)(blip.GetSize() + OfficeArtRecordHeader.Size)); 
			writer.Write(blip.NumberOfReferences); 
			writer.Write((int)0); 
			writer.Write((int)0); 
			blip.Write(writer);
		}
		protected internal override int GetSize() {
			return 36 + blip.GetSize() + OfficeArtRecordHeader.Size;
		}
	}
	#endregion
	#region OfficeArtBlipBase
	abstract class OfficeArtBlipBase : OfficeArtPartBase {
		public abstract byte BlipType { get; }
		public abstract byte[] Digest { get; }
		public abstract int NumberOfReferences { get; }
	}
	#endregion
	#region OfficeArtBlipImage
	class OfficeArtBlipImage : OfficeArtBlipBase {
		readonly XlsPictureData pictureData;
		public OfficeArtBlipImage(XlsPictureData pictureData)
			: base() {
			this.pictureData = pictureData;
		}
		public override int HeaderInstanceInfo { 
			get {
				if(pictureData.ImageFormat == XlsImageFormat.Jpeg)
					return 0x46a;
				if(pictureData.ImageFormat == XlsImageFormat.Tiff)
					return 0x6e4;
				return 0x6e0; 
			} 
		}
		public override int HeaderTypeCode { 
			get { 
				if(pictureData.ImageFormat == XlsImageFormat.Jpeg)
					return 0xf01d;
				if(pictureData.ImageFormat == XlsImageFormat.Tiff)
					return 0xf029;
				return 0xf01e; 
			} 
		}
		public override int HeaderVersion { get { return 0x0000; } }
		public override byte BlipType { 
			get {
				if(pictureData.ImageFormat == XlsImageFormat.Jpeg)
					return 0x05;
				if(pictureData.ImageFormat == XlsImageFormat.Tiff)
					return 0x11;
				return 0x06; 
			} 
		}
		public override byte[] Digest { get { return pictureData.ImageDigest; } }
		public override int NumberOfReferences { get { return pictureData.NumberOfReferences; } }
		protected internal override void WriteCore(BinaryWriter writer) {
			writer.Write(pictureData.ImageDigest);
			writer.Write((byte)0xff); 
			writer.Write(pictureData.ImageBytes);
		}
		protected internal override int GetSize() {
			return pictureData.ImageBytes.Length + pictureData.ImageDigest.Length + 1;
		}
	}
	#endregion
	#region OfficeArtDrawingContainer (OfficeArtDgContainer)
	class OfficeArtDrawingContainer : OfficeArtCompositePartBase {
		public override int HeaderInstanceInfo { get { return 0x0000; } }
		public override int HeaderTypeCode { get { return 0xf002; } }
		public override int HeaderVersion { get { return 0x000f; } }
	}
	#endregion
	#region OfficeArtFileDrawingRecord (OfficeArtFDG)
	class OfficeArtFileDrawingRecord : OfficeArtPartBase {
		int drawingId;
		int numberOfShapes;
		int lastShapeId;
		public OfficeArtFileDrawingRecord(int drawingId, int numberOfShapes, int lastShapeId) {
			this.drawingId = drawingId;
			this.numberOfShapes = numberOfShapes;
			this.lastShapeId = lastShapeId;
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return drawingId; } }
		public override int HeaderTypeCode { get { return 0xf008; } }
		public override int HeaderVersion { get { return 0x0000; } }
		#endregion
		protected internal override void WriteCore(BinaryWriter writer) {
			writer.Write(numberOfShapes);
			writer.Write(lastShapeId);
		}
		protected internal override int GetSize() {
			return 8;
		}
	}
	#endregion
	#region OfficeArtShapeGroupContainer
	class OfficeArtShapeGroupContainer : OfficeArtCompositePartBase {
		public override int HeaderInstanceInfo { get { return 0x0000; } }
		public override int HeaderTypeCode { get { return 0xf003; } }
		public override int HeaderVersion { get { return 0x000f; } }
	}
	#endregion
	#region OfficeArtShapeContainer
	class OfficeArtShapeContainer : OfficeArtCompositePartBase {
		public override int HeaderInstanceInfo { get { return 0x0000; } }
		public override int HeaderTypeCode { get { return 0xf004; } }
		public override int HeaderVersion { get { return 0x000f; } }
	}
	#endregion
	#region OfficeArtShapeGroupCoordinateSystem (OfficeArtFSPGR)
	class OfficeArtShapeGroupCoordinateSystem : OfficeArtPartBase {
		#region Properties
		public override int HeaderInstanceInfo { get { return 0x0000; } }
		public override int HeaderTypeCode { get { return 0xf009; } }
		public override int HeaderVersion { get { return 0x0001; } }
		public int Left { get; set; }
		public int Top { get; set; }
		public int Right { get; set; }
		public int Bottom { get; set; }
		#endregion
		protected internal override void WriteCore(BinaryWriter writer) {
			writer.Write(Left);
			writer.Write(Top);
			writer.Write(Right);
			writer.Write(Bottom);
		}
		protected internal override int GetSize() {
			return 16;
		}
	}
	#endregion
	#region OfficeArtShapeRecord (OfficeArtFSP)
	class OfficeArtShapeRecord : OfficeArtPartBase {
		int shapeTypeCode;
		int shapeId;
		int flags { get; set; }
		public OfficeArtShapeRecord(int shapeTypeCode, int shapeId, int flags)
			: base() {
			this.shapeTypeCode = shapeTypeCode;
			this.shapeId = shapeId;
			this.flags = flags;
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return shapeTypeCode; } }
		public override int HeaderTypeCode { get { return 0xf00a; } }
		public override int HeaderVersion { get { return 0x0002; } }
		#endregion
		protected internal override void WriteCore(BinaryWriter writer) {
			writer.Write(shapeId);
			writer.Write(flags);
		}
		protected internal override int GetSize() {
			return 8;
		}
	}
	#endregion
	#region IOfficeArtProperty
	interface IOfficeArtProperty {
		int Size { get; }
		bool Complex { get; }
		byte[] ComplexData { get; }
		void Write(BinaryWriter writer);
	}
	#endregion
	#region OfficeArtIntProperty
	class OfficeArtIntProperty : IOfficeArtProperty {
		int typeCode;
		int value;
		public OfficeArtIntProperty(int typeCode, int value) {
			this.typeCode = typeCode;
			this.value = value;
		}
		#region IOfficeArtProperty Members
		public int Size { get { return 6; } }
		public bool Complex { get { return false; } }
		public byte[] ComplexData { get { return null; } }
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)typeCode);
			writer.Write(value);
		}
		#endregion
	}
	#endregion
	#region OfficeArtStringProperty
	class OfficeArtStringProperty : IOfficeArtProperty {
		int typeCode;
		string value;
		public OfficeArtStringProperty(int typeCode, string value) {
			this.typeCode = typeCode;
			if(value == null)
				this.value = string.Empty;
			else
				this.value = value;
		}
		#region IOfficeArtProperty Members
		public int Size { get { return 8 + this.value.Length * 2; } }
		public bool Complex { get { return true; } }
		public byte[] ComplexData { get { return XLStringEncoder.GetEncoding(true).GetBytes(this.value + "\0"); } }
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)typeCode);
			writer.Write((int)(this.value.Length * 2 + 2));
		}
		#endregion
	}
	#endregion
	#region OfficeArtDataProperty
	class OfficeArtDataProperty : IOfficeArtProperty {
		int typeCode;
		int value;
		byte[] complexData;
		public OfficeArtDataProperty(int typeCode, byte[] data) {
			this.typeCode = typeCode;
			this.complexData = data;
			this.value = data.Length;
		}
		#region IOfficeArtProperty Members
		public int Size { get { return 6 + complexData.Length; } }
		public bool Complex { get { return true; } }
		public byte[] ComplexData { get { return complexData; } }
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)typeCode);
			writer.Write(value);
		}
		#endregion
	}
	#endregion
	#region OfficeArtProperties (OfficeArtFOPT)
	class OfficeArtProperties : OfficeArtPartBase {
		readonly List<IOfficeArtProperty> properties = new List<IOfficeArtProperty>();
		public override int HeaderInstanceInfo { get { return Properties.Count; } }
		public override int HeaderTypeCode { get { return 0xf00b; } }
		public override int HeaderVersion { get { return 0x0003; } }
		protected internal List<IOfficeArtProperty> Properties { get { return properties; } }
		protected internal override void WriteCore(BinaryWriter writer) {
			foreach(IOfficeArtProperty property in Properties) {
				property.Write(writer);
			}
			foreach(IOfficeArtProperty property in Properties) {
				if(property.Complex)
					writer.Write(property.ComplexData);
			}
		}
		protected internal override int GetSize() {
			int totalSize = 0;
			foreach(IOfficeArtProperty property in Properties) {
				totalSize += property.Size;
			}
			return totalSize;
		}
	}
	#endregion
	#region OfficeArtTertiaryProperties
	class OfficeArtTertiaryProperties : OfficeArtProperties {
		public override int HeaderTypeCode { get { return 0xf122; } }
	}
	#endregion
	#region OfficeArtClientAnchorSheet
	class OfficeArtClientAnchorSheet : OfficeArtPartBase {
		XlDrawingObjectBase drawingObject;
		public OfficeArtClientAnchorSheet(XlDrawingObjectBase drawingObject)
			: base() {
			this.drawingObject = drawingObject;
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return 0x0000; } }
		public override int HeaderTypeCode { get { return 0xf010; } }
		public override int HeaderVersion { get { return 0x0000; } }
		public XlDrawingObjectBase DrawingObject { get { return drawingObject; } }
		#endregion
		protected internal override void WriteCore(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if(DrawingObject.AnchorBehavior == XlAnchorType.Absolute)
				bitwiseField |= 0x0001;
			if(DrawingObject.AnchorBehavior != XlAnchorType.TwoCell)
				bitwiseField |= 0x0002;
			writer.Write(bitwiseField);
			writer.Write((ushort)DrawingObject.TopLeft.Column);
			writer.Write(GetOffsetValue(DrawingObject.TopLeft.RelativeColumnOffset, 1024));
			writer.Write((ushort)DrawingObject.TopLeft.Row);
			writer.Write(GetOffsetValue(DrawingObject.TopLeft.RelativeRowOffset, 256));
			writer.Write((ushort)DrawingObject.BottomRight.Column);
			writer.Write(GetOffsetValue(DrawingObject.BottomRight.RelativeColumnOffset, 1024));
			writer.Write((ushort)DrawingObject.BottomRight.Row);
			writer.Write(GetOffsetValue(DrawingObject.BottomRight.RelativeRowOffset, 256));
		}
		protected internal override int GetSize() {
			return 18;
		}
		short GetOffsetValue(float value, int factor) {
			int result = (int)(value * factor);
			result = Math.Min(factor, result);
			result = Math.Max(-factor, result);
			return (short)result;
		}
	}
	#endregion
	#region OfficeArtClientData
	abstract class OfficeArtClientData : OfficeArtPartBase {
		#region Properties
		public override int HeaderInstanceInfo { get { return 0x0000; } }
		public override int HeaderTypeCode { get { return 0xf011; } }
		public override int HeaderVersion { get { return 0x0000; } }
		#endregion
		protected internal override void WriteCore(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null) {
				chunkWriter.Flush();
				chunkWriter.SetNextChunk(new XlsChunk(XlsRecordType.Obj));
				WriteObjData(chunkWriter);
				chunkWriter.Flush();
				chunkWriter.SetNextChunk(new XlsChunk(XlsRecordType.MsoDrawing));
			}
		}
		protected internal override int GetSize() {
			return 0;
		}
		protected abstract void WriteObjData(BinaryWriter writer);
	}
	#endregion
	#region OfficeArtPictureClientData
	class OfficeArtPictureClientData : OfficeArtClientData {
		XlsPictureObject pictureObject;
		public OfficeArtPictureClientData(XlsPictureObject pictureObject)
			: base() {
			this.pictureObject = pictureObject;
		}
		#region Properties
		public XlsPictureObject PictureObject { get { return pictureObject; } }
		#endregion
		protected override void WriteObjData(BinaryWriter writer) {
			writer.Write((ushort)0x15);
			writer.Write((ushort)0x12);
			writer.Write((ushort)0x08); 
			writer.Write((ushort)(PictureObject.PictureId)); 
			writer.Write((ushort)0x6011); 
			writer.Write((int)0); 
			writer.Write((int)0); 
			writer.Write((int)0); 
			writer.Write((ushort)0x07);
			writer.Write((ushort)0x02);
			writer.Write((ushort)0xffff);
			writer.Write((ushort)0x08);
			writer.Write((ushort)0x02);
			writer.Write((ushort)0x01);
			writer.Write((int)0); 
		}
	}
	#endregion
}
namespace DevExpress.XtraExport.Implementation {
	#region XlsImageFormat
	enum XlsImageFormat {
		Png,
		Jpeg,
		Tiff
	}
	#endregion
	#region XlsPictureData
	class XlsPictureData {
		public XlsPictureData(XlsImageFormat imageFormat, byte[] imageBytes, byte[] imageDigest) {
			Guard.ArgumentNotNull(imageBytes, "imageBytes");
			Guard.ArgumentNotNull(imageDigest, "imageDigest");
			ImageFormat = imageFormat;
			NumberOfReferences = 1;
			ImageBytes = imageBytes;
			ImageDigest = imageDigest;
		}
		public XlsImageFormat ImageFormat { get; private set; }
		public int NumberOfReferences { get; set; }
		public byte[] ImageBytes { get; private set; }
		public byte[] ImageDigest { get; private set; }
		public bool EqualsDigest(byte[] digest) {
			if(digest == null)
				return false;
			if(ImageDigest.Length != digest.Length)
				return false;
			for(int i = 0; i < ImageDigest.Length; i++) {
				if(ImageDigest[i] != digest[i])
					return false;
			}
			return true;
		}
	}
	#endregion
}
