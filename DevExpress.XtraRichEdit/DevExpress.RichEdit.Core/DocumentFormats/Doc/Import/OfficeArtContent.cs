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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Doc {
	public class BlipsWithProperties {
		readonly Dictionary<int, BlipBase> blips;
		readonly Dictionary<int, OfficeArtProperties> shapeArtProperties;
		readonly Dictionary<int, OfficeArtTertiaryProperties> shapeArtTertiaryProperties;
		public BlipsWithProperties() {
			blips = new Dictionary<int, BlipBase>();
			shapeArtProperties = new Dictionary<int, OfficeArtProperties>();
			shapeArtTertiaryProperties = new Dictionary<int, OfficeArtTertiaryProperties>();
		}
		public Dictionary<int, OfficeArtProperties> ShapeArtProperties { get { return shapeArtProperties; } }
		public Dictionary<int, OfficeArtTertiaryProperties> ShapeArtTertiaryProperties { get { return shapeArtTertiaryProperties; } }
		public Dictionary<int, BlipBase> Blips { get { return blips; } }
	}
	#region OfficeArtContent
	public class OfficeArtContent {
		#region OfficeArtContentHelper
		internal class OfficeArtContentHelper {
			#region Fields
			BlipsWithProperties activeShapes;
			OfficeArtContent content;
			#endregion
			public OfficeArtContentHelper(OfficeArtContent content) {
				this.content = content;
			}
			public void Traverse() {
				if (this.content.DrawingContainer.BlipContainer == null)
					return;
				int count = this.content.Drawings.Count;
				for (int i = 0; i < count; i++) {
					OfficeArtWordDrawing drawing = this.content.Drawings[i];
					this.activeShapes = (drawing.Location == 0) ? this.content.MainDocumentBlips : this.content.HeadersBlips;
					Process(drawing.DrawingObjectsContainer.ShapeGroup.Items);
				}
			}
			void Process(List<OfficeDrawingPartBase> shapes) {
				int count = shapes.Count;
				for (int i = 0; i < count; i++) {
					if (shapes[i] is OfficeArtShapeContainer) {
						OfficeArtShapeContainer shape = shapes[i] as OfficeArtShapeContainer;
						List<BlipBase> blips = content.DrawingContainer.BlipContainer.Blips;
						int shapeBlipIndex = shape.ArtProperties.BlipIndex;
						if (shapeBlipIndex > 0 && shapeBlipIndex <= blips.Count) {
							activeShapes.Blips.Add(shape.ShapeRecord.ShapeIdentifier, blips[shapeBlipIndex - 1]);
						}
						activeShapes.ShapeArtProperties.Add(shape.ShapeRecord.ShapeIdentifier, shape.ArtProperties);
						activeShapes.ShapeArtTertiaryProperties.Add(shape.ShapeRecord.ShapeIdentifier, shape.ArtTertiaryProperties);
						continue;
					}
					if (shapes[i] is OfficeArtShapeGroupContainer) {
						OfficeArtShapeGroupContainer shapeGroup = shapes[i] as OfficeArtShapeGroupContainer;
						Process(shapeGroup.Items);
					}
				}
			}
		}
		#endregion
		#region static
		public static OfficeArtContent FromStream(BinaryReader reader, BinaryReader embeddedReader, int offset, int size) {
			OfficeArtContent result = new OfficeArtContent();
			result.Read(reader, embeddedReader, offset, size);
			return result;
		}
		#endregion
		#region Fields
		const int mainDocumentDrawingLocation = 0;
		const int headerDocumentDrawingLocation = 1;
		OfficeArtContentHelper contentHelper;
		OfficeArtDrawingContainer drawingContainer;
		List<OfficeArtWordDrawing> drawings;
		OfficeArtWordDrawing mainDocumentDrawing;
		OfficeArtWordDrawing headerDrawing;
		BlipsWithProperties mainDocumentBlips;
		BlipsWithProperties headersBlips;
		#endregion
		public OfficeArtContent() {
			this.drawingContainer = new OfficeArtDrawingContainer();
			this.mainDocumentBlips = new BlipsWithProperties();
			this.headersBlips = new BlipsWithProperties();
			this.contentHelper = new OfficeArtContentHelper(this);
			this.drawings = new List<OfficeArtWordDrawing>();
		}
		#region Properties
		protected internal OfficeArtDrawingContainer DrawingContainer { get { return this.drawingContainer; } }
		protected internal List<OfficeArtWordDrawing> Drawings { get { return this.drawings; } }
		protected internal OfficeArtWordDrawing MainDocumentDrawing {
			get {
				if (mainDocumentDrawing == null)
					InitMainDocumentDrawing();
				return mainDocumentDrawing;
			}
		}
		protected internal OfficeArtWordDrawing HeaderDrawing {
			get {
				if (mainDocumentDrawing == null)
					InitMainDocumentDrawing();
				if (headerDrawing == null)
					InitHeaderDrawing();
				return headerDrawing;
			}
		}
		public BlipsWithProperties MainDocumentBlips { get { return this.mainDocumentBlips; } }
		public BlipsWithProperties HeadersBlips { get { return this.headersBlips; } }
		#endregion
		protected internal void InitMainDocumentDrawing() {
			this.mainDocumentDrawing = new OfficeArtWordDrawing(mainDocumentDrawingLocation, Drawings.Count + 1, OfficeArtConstants.DefaultMainDocumentShapeIdentifier);
			Drawings.Add(this.mainDocumentDrawing);
		}
		protected internal void InitHeaderDrawing() {
			this.headerDrawing = new OfficeArtWordDrawing(headerDocumentDrawingLocation, Drawings.Count + 1, OfficeArtConstants.DefaultHeaderShapeIdentifier);
			Drawings.Add(this.headerDrawing);
		}
		public void InsertPictureFloatingObject(OfficeImage image, DocContentState state, int shapeIdentifier, FloatingObjectAnchorRun run, DocumentModelUnitConverter unitConverter) {
			if (state != DocContentState.MainDocument && state != DocContentState.HeadersFootersStory)
				return;
			InsertPictureFloatingObjectCore(image, state, shapeIdentifier, run, unitConverter);
		}
		public void InsertTextBoxFloatingObject(DocContentState state, int shapeIdentifier, int textIdentifier, FloatingObjectAnchorRun run, DocumentModelUnitConverter unitConverter) {
			if (state != DocContentState.MainDocument && state != DocContentState.HeadersFootersStory)
				return;
			InsertTextBoxFloatingObjectCore(state, shapeIdentifier, textIdentifier, run, unitConverter);
		}
		void InsertPictureFloatingObjectCore(OfficeImage image, DocContentState state, int shapeIdentifier, FloatingObjectAnchorRun run, DocumentModelUnitConverter unitConverter) {
			OfficeArtFloatingShapeContainer shapeContainer = CreateShapeContainer(state, shapeIdentifier);
			DrawingContainer.BlipContainer.Blips.Add(new FileBlipStoreEntry(image, true));
			shapeContainer.ArtProperties.BlipIndex = DrawingContainer.BlipContainer.Blips.Count;
			InitializeArtProperties(shapeContainer.ArtProperties, run, unitConverter);
			ApplyFloatingObjectDistancesProperties(shapeContainer.ArtProperties, shapeContainer.ArtTertiaryProperties, run.FloatingObjectProperties);
			shapeContainer.ArtProperties.CreateProperties();
			shapeContainer.ArtTertiaryProperties.CreateProperties();
		}
		void InitializeArtProperties(OfficeArtProperties artProperties, FloatingObjectAnchorRun run, DocumentModelUnitConverter unitConverter) {
			artProperties.Shape = run.Shape;
			artProperties.FloatingObjectProperties = run.FloatingObjectProperties;
			artProperties.UnitConverter = unitConverter;
		}
		void InsertTextBoxFloatingObjectCore(DocContentState state, int shapeIdentifier, int textIdentifier, FloatingObjectAnchorRun run, DocumentModelUnitConverter unitConverter) {
			OfficeArtFloatingShapeContainer shapeContainer = CreateShapeContainer(state, shapeIdentifier);
			shapeContainer.ArtProperties.TextIndex = textIdentifier;
			InitializeArtProperties(shapeContainer.ArtProperties, run, unitConverter);
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			ApplyTextBoxProperties(shapeContainer.ArtProperties, textBoxContent.TextBoxProperties);
			ApplyFloatingObjectDistancesProperties(shapeContainer.ArtProperties, shapeContainer.ArtTertiaryProperties, run.FloatingObjectProperties);
			shapeContainer.ArtProperties.CreateProperties();
			shapeContainer.ArtTertiaryProperties.CreateProperties();
		}
		void ApplyFloatingObjectDistancesProperties(OfficeArtProperties artProperties, OfficeArtTertiaryProperties artTertiaryProperties, FloatingObjectProperties floatingObjectProperties) {
			artProperties.FloatingObjectProperties = floatingObjectProperties;
			if (floatingObjectProperties.UseRelativeWidth)
				ApplyRelativeWidth(artTertiaryProperties, floatingObjectProperties);
			if (floatingObjectProperties.UseRelativeHeight)
				ApplyRelativeHeight(artTertiaryProperties, floatingObjectProperties);
			if (floatingObjectProperties.UseHorizontalPositionAlignment)
				ApplyHorizontalPositionAlignment(artTertiaryProperties, floatingObjectProperties);
			if (floatingObjectProperties.UseVerticalPositionAlignment)
				ApplyVerticalPositionAlignment(artTertiaryProperties, floatingObjectProperties);
			if (floatingObjectProperties.UsePercentOffset)
				ApplyPercentOffset(artTertiaryProperties, floatingObjectProperties);
		}
		void ApplyRelativeWidth(OfficeArtTertiaryProperties artTertiaryProperties, FloatingObjectProperties floatingObjectProperties) {
			artTertiaryProperties.UseRelativeWidth = true;
			FloatingObjectRelativeWidth relativeWidth = floatingObjectProperties.RelativeWidth;
			artTertiaryProperties.SizeRelH = (DrawingGroupShape2SizeRelH.RelativeFrom)relativeWidth.From;
			artTertiaryProperties.PctHoriz = relativeWidth.Width / 100;
		}
		void ApplyRelativeHeight(OfficeArtTertiaryProperties artTertiaryProperties, FloatingObjectProperties floatingObjectProperties) {
			artTertiaryProperties.UseRelativeHeight = true;
			FloatingObjectRelativeHeight relativeHeight = floatingObjectProperties.RelativeHeight;
			artTertiaryProperties.SizeRelV = (DrawingGroupShape2SizeRelV.RelativeFrom)relativeHeight.From;
			artTertiaryProperties.PctVert = relativeHeight.Height / 100;
		}
		void ApplyHorizontalPositionAlignment(OfficeArtTertiaryProperties artTertiaryProperties, FloatingObjectProperties floatingObjectProperties) {
			artTertiaryProperties.PosRelH = GetPosRelH(floatingObjectProperties.HorizontalPositionType);
			artTertiaryProperties.PosH = GetPosH(floatingObjectProperties.HorizontalPositionAlignment);
			artTertiaryProperties.UsePosH = true;
		}
		void ApplyVerticalPositionAlignment(OfficeArtTertiaryProperties artTertiaryProperties, FloatingObjectProperties floatingObjectProperties) {
			artTertiaryProperties.PosRelV = GetPosRelV(floatingObjectProperties.VerticalPositionType);
			artTertiaryProperties.PosV = GetPosV(floatingObjectProperties.VerticalPositionAlignment);
			artTertiaryProperties.UsePosV = true;
		}
		void ApplyPercentOffset(OfficeArtTertiaryProperties artTertiaryProperties, FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.PercentOffsetX != 0 && (!floatingObjectProperties.UseHorizontalPositionAlignment || floatingObjectProperties.HorizontalPositionAlignment == FloatingObjectHorizontalPositionAlignment.None)) {
				artTertiaryProperties.PctHorizPos = floatingObjectProperties.PercentOffsetX / 100;
				artTertiaryProperties.PosRelH = GetPosRelH(floatingObjectProperties.HorizontalPositionType);
				artTertiaryProperties.PosH = DrawingGroupShapePosH.Msoph.msophAbs;
				artTertiaryProperties.UsePosH = true;
			}
			if (floatingObjectProperties.PercentOffsetY != 0 && (!floatingObjectProperties.UseVerticalPositionAlignment || floatingObjectProperties.VerticalPositionAlignment == FloatingObjectVerticalPositionAlignment.None)) {
				artTertiaryProperties.PctVertPos = floatingObjectProperties.PercentOffsetY / 100;
				artTertiaryProperties.PosRelV = GetPosRelV(floatingObjectProperties.VerticalPositionType);
				artTertiaryProperties.PosV = DrawingGroupShapePosV.Msopv.msopvAbs;
				artTertiaryProperties.UsePosV = true;
			}
		}
		DrawingGroupShapePosRelH.Msoprh GetPosRelH(FloatingObjectHorizontalPositionType type) {
			switch (type) {
				case FloatingObjectHorizontalPositionType.Character:
					return DrawingGroupShapePosRelH.Msoprh.msoprhChar;
				case FloatingObjectHorizontalPositionType.Margin:
					return DrawingGroupShapePosRelH.Msoprh.msoprhMargin;
				case FloatingObjectHorizontalPositionType.Page:
					return DrawingGroupShapePosRelH.Msoprh.msoprhPage;
				case FloatingObjectHorizontalPositionType.Column:
					return DrawingGroupShapePosRelH.Msoprh.msoprhText;
				default:
					return DrawingGroupShapePosRelH.Msoprh.msoprhText;
			}
		}
		DrawingGroupShapePosRelV.Msoprv GetPosRelV(FloatingObjectVerticalPositionType type) {
			switch (type) {
				case FloatingObjectVerticalPositionType.Line:
					return DrawingGroupShapePosRelV.Msoprv.msoprvLine;
				case FloatingObjectVerticalPositionType.Margin:
					return DrawingGroupShapePosRelV.Msoprv.msoprvMargin;
				case FloatingObjectVerticalPositionType.Page:
					return DrawingGroupShapePosRelV.Msoprv.msoprvPage;
				case FloatingObjectVerticalPositionType.Paragraph:
					return DrawingGroupShapePosRelV.Msoprv.msoprvText;
				default:
					return DrawingGroupShapePosRelV.Msoprv.msoprvText;
			}
		}
		DrawingGroupShapePosH.Msoph GetPosH(FloatingObjectHorizontalPositionAlignment alignment) {
			switch (alignment) {
				case FloatingObjectHorizontalPositionAlignment.None:
					return DrawingGroupShapePosH.Msoph.msophAbs;
				case FloatingObjectHorizontalPositionAlignment.Center:
					return DrawingGroupShapePosH.Msoph.msophCenter;
				case FloatingObjectHorizontalPositionAlignment.Inside:
					return DrawingGroupShapePosH.Msoph.msophInside;
				case FloatingObjectHorizontalPositionAlignment.Left:
					return DrawingGroupShapePosH.Msoph.msophLeft;
				case FloatingObjectHorizontalPositionAlignment.Outside:
					return DrawingGroupShapePosH.Msoph.msophOutside;
				case FloatingObjectHorizontalPositionAlignment.Right:
					return DrawingGroupShapePosH.Msoph.msophRight;
				default:
					return DrawingGroupShapePosH.Msoph.msophAbs;
			}
		}
		DrawingGroupShapePosV.Msopv GetPosV(FloatingObjectVerticalPositionAlignment alignment) {
			switch (alignment) {
				case FloatingObjectVerticalPositionAlignment.None:
					return DrawingGroupShapePosV.Msopv.msopvAbs;
				case FloatingObjectVerticalPositionAlignment.Bottom:
					return DrawingGroupShapePosV.Msopv.msopvBottom;
				case FloatingObjectVerticalPositionAlignment.Center:
					return DrawingGroupShapePosV.Msopv.msopvCenter;
				case FloatingObjectVerticalPositionAlignment.Inside:
					return DrawingGroupShapePosV.Msopv.msopvInside;
				case FloatingObjectVerticalPositionAlignment.Outside:
					return DrawingGroupShapePosV.Msopv.msopvOutside;
				case FloatingObjectVerticalPositionAlignment.Top:
					return DrawingGroupShapePosV.Msopv.msopvTop;
				default:
					return DrawingGroupShapePosV.Msopv.msopvAbs;
			}
		}
		void ApplyTextBoxProperties(OfficeArtProperties artProperties, TextBoxProperties textBoxProperties) {
			artProperties.TextBoxProperties = textBoxProperties;
		}
		OfficeArtFloatingShapeContainer CreateShapeContainer(DocContentState state, int shapeIdentifier) {
			OfficeArtWordDrawing drawing = GetDrawingByState(state);
			OfficeArtFloatingShapeContainer shapeContainer = new OfficeArtFloatingShapeContainer();
			List<OfficeDrawingPartBase> shapes = drawing.DrawingObjectsContainer.ShapeGroup.Items;
			shapes.Add(shapeContainer);
			shapeContainer.ShapeRecord.ShapeIdentifier = shapeIdentifier;
			OfficeArtFileDrawingRecord drawingData = drawing.DrawingObjectsContainer.DrawingData;
			drawingData.LastShapeIdentifier = shapeIdentifier;
			drawingData.NumberOfShapes = shapes.Count;
			return shapeContainer;
		}
		OfficeArtWordDrawing GetDrawingByState(DocContentState state) {
			if (state == DocContentState.MainDocument)
				return MainDocumentDrawing;
			if (state == DocContentState.HeadersFootersStory)
				return HeaderDrawing;
			Exceptions.ThrowInternalException();
			return null;
		}
		protected internal void Read(BinaryReader reader, BinaryReader embeddedReader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			Guard.ArgumentNotNull(embeddedReader, "embeddedReader");
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			long endPosition = reader.BaseStream.Position + size;
			this.drawingContainer = OfficeArtDrawingContainer.FromStream(reader, embeddedReader);
			while (reader.BaseStream.Position < endPosition) {
				Drawings.Add(OfficeArtWordDrawing.FromStream(reader));
			}
			this.contentHelper.Traverse();
		}
		public void Write(BinaryWriter writer, BinaryWriter embeddedWriter) {
			Guard.ArgumentNotNull(writer, "writer");
			Guard.ArgumentNotNull(embeddedWriter, "embeddedWriter");
			DrawingContainer.Write(writer, embeddedWriter);
			int count = Drawings.Count;
			for (int i = 0; i < count; i++) {
				Drawings[i].Write(writer);
			}
		}
	}
	#endregion
	#region OfficeArtWordDrawing
	public class OfficeArtWordDrawing {
		#region static
		public static OfficeArtWordDrawing FromStream(BinaryReader reader) {
			OfficeArtWordDrawing result = new OfficeArtWordDrawing();
			result.Read(reader);
			return result;
		}
		#endregion
		#region Fields
		byte location;
		OfficeArtDrawingObjectsContainer drawingContainer;
		#endregion
		OfficeArtWordDrawing() { }
		public OfficeArtWordDrawing(byte location, int drawingId, int shapeId) {
			this.location = location;
			this.drawingContainer = new OfficeArtDrawingObjectsContainer(location, drawingId, shapeId);
			this.drawingContainer.BackgroundShape.Items.Add(new OfficeClientData());
		}
		#region Properties
		public byte Location { get { return location; } }
		public OfficeArtDrawingObjectsContainer DrawingObjectsContainer { get { return drawingContainer; } }
		#endregion
		protected internal void Read(BinaryReader reader) {
			this.location = reader.ReadByte();
			this.drawingContainer = OfficeArtDrawingObjectsContainer.FromStream(reader);
		}
		public void Write(BinaryWriter writer) {
			writer.Write(Location);
			DrawingObjectsContainer.Write(writer);
		}
	}
	#endregion
	#region OfficeArtDrawingObjectsContainer
	public class OfficeArtDrawingObjectsContainer : CompositeOfficeDrawingPartBase {
		#region static
		public static OfficeArtDrawingObjectsContainer FromStream(BinaryReader reader) {
			OfficeArtDrawingObjectsContainer result = new OfficeArtDrawingObjectsContainer();
			result.Read(reader);
			return result;
		}
		#endregion
		#region Fields
		OfficeArtFileDrawingRecord drawingData;
		OfficeArtShapeGroupContainer shapeGroup;
		DrawingFillStyleBooleanProperties drawingFillStyleBooleanProperties;
		OfficeArtShapeContainer backgroundShape;
		#endregion
		OfficeArtDrawingObjectsContainer() { }
		public OfficeArtDrawingObjectsContainer(byte location, int drawingId, int shapeIdentifier) {
			this.drawingData = new OfficeArtFileDrawingRecord(drawingId);
			this.shapeGroup = new OfficeArtShapeGroupContainer(location);
			this.backgroundShape = new OfficeArtShapeContainer();
			Items.Add(this.drawingData);
			Items.Add(this.shapeGroup);
			Items.Add(this.backgroundShape);
			BackgroundShape.ShapeRecord.ShapeIdentifier = shapeIdentifier;
			SetShapeContainerProperties();
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.DrawingObjectsContainer; } }
		public override int HeaderVersion { get { return OfficeArtVersions.DefaultHeaderVersion; } }
		public OfficeArtFileDrawingRecord DrawingData { get { return drawingData; } }
		public OfficeArtShapeGroupContainer ShapeGroup { get { return shapeGroup; } }
		protected internal OfficeArtShapeContainer BackgroundShape { get { return backgroundShape; } }
		public DrawingFillStyleBooleanProperties DrawingFillStyleBooleanProperties { get { return drawingFillStyleBooleanProperties; } }
		#endregion
		void SetShapeContainerProperties() {
			List<IOfficeDrawingProperty> properties = BackgroundShape.ArtProperties.Properties;
			drawingFillStyleBooleanProperties = new DrawingFillStyleBooleanProperties();
			properties.Add(drawingFillStyleBooleanProperties);
			properties.Add(new DrawingLineWidth());
			properties.Add(new DrawingLineStyleBooleanProperties());
			properties.Add(new DrawingBlackWhiteMode(BlackWhiteMode.White));
			properties.Add(new DrawingShapeBooleanProperties());
		}
		protected internal void Read(BinaryReader reader) {
			OfficeArtRecordHeader header = OfficeArtRecordHeader.FromStream(reader);
			CheckHeader(header);
			long endPosition = reader.BaseStream.Position + header.Length;
			while (reader.BaseStream.Position < endPosition) {
				ParseHeader(reader);
			}
		}
		void CheckHeader(OfficeArtRecordHeader header) {
			if (header.Version != OfficeArtVersions.DefaultHeaderVersion ||
				header.InstanceInfo != OfficeArtHeaderInfos.EmptyHeaderInfo ||
				header.TypeCode != OfficeArtTypeCodes.DrawingObjectsContainer)
				OfficeArtExceptions.ThrowInvalidContent();
		}
		void ParseHeader(BinaryReader reader) {
			OfficeArtRecordHeader header = OfficeArtRecordHeader.FromStream(reader);
			if (header.TypeCode == OfficeArtTypeCodes.FileDrawingGroupRecord) {
				this.drawingData = OfficeArtFileDrawingRecord.FromStream(reader, header);
				return;
			}
			if (header.TypeCode == OfficeArtTypeCodes.ShapeGroupContainer) {
				this.shapeGroup = OfficeArtShapeGroupContainer.FromStream(reader, header);
				return;
			}
			if (header.TypeCode == OfficeArtTypeCodes.ShapeContainer) {
				this.backgroundShape = OfficeArtShapeContainer.FromStream(reader, header);
				return;
			}
			reader.BaseStream.Seek(header.Length, SeekOrigin.Current);
		}
	}
	#endregion
	#region OfficeArtFloatingShapeContainer
	public class OfficeArtFloatingShapeContainer : OfficeArtShapeContainer {
		public OfficeArtFloatingShapeContainer() {
			Items.Add(new OfficeClientAnchor());
			Items.Add(new OfficeClientData());
		}
	}
	#endregion
	#region OfficeArtShapeGroupContainer
	public class OfficeArtShapeGroupContainer : CompositeOfficeDrawingPartBase {
		public static OfficeArtShapeGroupContainer FromStream(BinaryReader reader, OfficeArtRecordHeader header) {
			OfficeArtShapeGroupContainer result = new OfficeArtShapeGroupContainer();
			result.Read(reader, header);
			return result;
		}
		protected OfficeArtShapeGroupContainer() { }
		public OfficeArtShapeGroupContainer(byte location) {
			Items.Add(new OfficeArtTopmostShapeContainer(location));
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.ShapeGroupContainer; } }
		public override int HeaderVersion { get { return OfficeArtVersions.DefaultHeaderVersion; } }
		#endregion
		protected internal void Read(BinaryReader reader, OfficeArtRecordHeader header) {
			long endPosition = reader.BaseStream.Position + header.Length;
			while (reader.BaseStream.Position < endPosition) {
				Items.Add(CreateShapeContainer(reader));
			}
		}
		OfficeDrawingPartBase CreateShapeContainer(BinaryReader reader) {
			OfficeArtRecordHeader shapeHeader = OfficeArtRecordHeader.FromStream(reader);
			int typeCode = shapeHeader.TypeCode;
			if (typeCode == OfficeArtTypeCodes.ShapeGroupContainer)
				return OfficeArtShapeGroupContainer.FromStream(reader, shapeHeader);
			if (typeCode == OfficeArtTypeCodes.ShapeContainer)
				return OfficeArtShapeContainer.FromStream(reader, shapeHeader);
			OfficeArtExceptions.ThrowInvalidContent();
			return null;
		}
	}
	#endregion
	#region OfficeArtTopmostShapeContainer
	public class OfficeArtTopmostShapeContainer : OfficeDrawingPartBase {
		#region Fields
		const int recordLength = 0x28;
		const int mainTopmostShapeId = 0x400;
		const int headerTopmostShapeId = 0x800;
		const int topmostShapeFlags = 0x05;
		OfficeArtShapeGroupCoordinateSystem coordinateSystem;
		OfficeArtShapeRecord shapeRecord;
		#endregion
		public OfficeArtTopmostShapeContainer(byte location) {
			this.coordinateSystem = new OfficeArtShapeGroupCoordinateSystem();
			InitializeShapeRecord(location);
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.ShapeContainer; } }
		public override int HeaderVersion { get { return OfficeArtVersions.DefaultHeaderVersion; } }
		public OfficeArtShapeGroupCoordinateSystem CoordinateSystem { get { return coordinateSystem; } }
		public OfficeArtShapeRecord ShapeRecord { get { return shapeRecord; } }
		#endregion
		void InitializeShapeRecord(byte location) {
			this.shapeRecord = new OfficeArtShapeRecord(OfficeArtHeaderInfos.NotPrimitiveShape);
			this.shapeRecord.ShapeIdentifier = (location == 0) ? mainTopmostShapeId : headerTopmostShapeId;
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
	#region OfficeArtShapeContainer
	public class OfficeArtShapeContainer : CompositeOfficeDrawingPartBase {
		#region static
		public static OfficeArtShapeContainer FromStream(BinaryReader reader, OfficeArtRecordHeader header) {
			OfficeArtShapeContainer result = new OfficeArtShapeContainer();
			result.Read(reader, header);
			return result;
		}
		#endregion
		#region Fields
		OfficeArtShapeRecord shapeRecord;
		OfficeArtProperties artProperties;
		OfficeArtTertiaryProperties artTertiaryProperties;
		#endregion
		public OfficeArtShapeContainer() {
			this.shapeRecord = new OfficeArtShapeRecord(OfficeArtHeaderInfos.PictureFrameShape);
			this.artProperties = new OfficeArtProperties();
			this.artTertiaryProperties = new OfficeArtTertiaryProperties();
			Items.Add(this.shapeRecord);
			Items.Add(this.artProperties);
			Items.Add(this.artTertiaryProperties);
		}
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.ShapeContainer; } }
		public override int HeaderVersion { get { return OfficeArtVersions.DefaultHeaderVersion; } }
		public OfficeArtShapeRecord ShapeRecord { get { return shapeRecord; } }
		public OfficeArtProperties ArtProperties { get { return artProperties; } }
		public OfficeArtTertiaryProperties ArtTertiaryProperties { get { return artTertiaryProperties; } }
		#endregion
		protected internal void Read(BinaryReader reader, OfficeArtRecordHeader header) {
			long endPosition = reader.BaseStream.Position + header.Length;
			if (endPosition > reader.BaseStream.Length)
				return;
			while (reader.BaseStream.Position < endPosition) {
				ParseHeader(reader);
			}
		}
		void ParseHeader(BinaryReader reader) {
			OfficeArtRecordHeader header = OfficeArtRecordHeader.FromStream(reader);
			if (header.TypeCode == OfficeArtTypeCodes.FileShape) {
				this.shapeRecord = OfficeArtShapeRecord.FromStream(reader);
				return;
			}
			if (header.TypeCode == OfficeArtTypeCodes.PropertiesTable) {
				this.artProperties = OfficeArtProperties.FromStream(reader, header);
				return;
			}
			if (header.TypeCode == OfficeArtTypeCodes.TertiaryPropertiesTable) {
				this.artTertiaryProperties = OfficeArtTertiaryProperties.FromStream(reader, header);
				return;
			}
			reader.BaseStream.Seek(header.Length, SeekOrigin.Current);
		}
	}
	#endregion
	#region OfficeArtInlineShapeContainer
	public class OfficeArtInlineShapeContainer {
		public static OfficeArtInlineShapeContainer FromStream(BinaryReader reader, int size) {
			OfficeArtInlineShapeContainer result = new OfficeArtInlineShapeContainer();
			result.Read(reader, size);
			return result;
		}
		#region Fields
		OfficeArtShapeContainer shapeContainer;
		List<BlipBase> blips;
		#endregion
		public OfficeArtInlineShapeContainer() {
			this.shapeContainer = new OfficeArtShapeContainer();
			this.blips = new List<BlipBase>();
		}
		#region Properties
		public OfficeArtShapeContainer ShapeContainer { get { return this.shapeContainer; } }
		public List<BlipBase> Blips { get { return this.blips; } }
		#endregion
		protected internal void Read(BinaryReader reader, int size) {
			long endPosition = reader.BaseStream.Position + size;
			OfficeArtRecordHeader header = OfficeArtRecordHeader.FromStream(reader);
			this.shapeContainer = OfficeArtShapeContainer.FromStream(reader, header);
			this.blips = BlipFactory.ReadAllBlips(reader, endPosition);
		}
		public void Write(BinaryWriter writer) {
			this.shapeContainer.Write(writer);
			int count = this.blips.Count;
			for (int i = 0; i < count; i++) {
				this.blips[i].Write(writer);
			}
		}
	}
	#endregion
	#region OfficeClientAnchor
	public class OfficeClientAnchor : OfficeDrawingPartBase {
		#region Fields
		const int recordLength = 4;
		const int data = 0x0000;
		#endregion
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.ClientAnchor; } }
		public override int HeaderVersion { get { return OfficeArtVersions.EmptyHeaderVersion; } }
		#endregion
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(data);
		}
		protected override int GetSize() {
			return recordLength;
		}
	}
	#endregion
	#region OfficeClientData
	public class OfficeClientData : OfficeDrawingPartBase {
		#region Fields
		const int recordLength = 4;
		const int data = 0x0001;
		#endregion
		#region Properties
		public override int HeaderInstanceInfo { get { return OfficeArtHeaderInfos.EmptyHeaderInfo; } }
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.ClientData; } }
		public override int HeaderVersion { get { return OfficeArtVersions.EmptyHeaderVersion; } }
		#endregion
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(data);
		}
		protected override int GetSize() {
			return recordLength;
		}
	}
	#endregion
	#region OfficeArtProperties
	public class OfficeArtProperties : OfficeArtPropertiesBase, IOfficeArtProperties {
		#region static
		public static OfficeArtProperties FromStream(BinaryReader reader, OfficeArtRecordHeader header) {
			OfficeArtProperties result = new OfficeArtProperties();
			result.Read(reader, header);
			return result;
		}
		#endregion
		#region Properties
		public override int HeaderTypeCode { get { return OfficeArtTypeCodes.PropertiesTable; } }
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
		public bool FitShapeToText { get; set; }
		public bool UseFitShapeToText { get; set; }
		public bool Filled { get; set; }
		public bool UseFilled { get; set; }
		public double LineWidth { get; set; }
		public Color LineColor { get; set; }
		public Color FillColor { get; set; }
		public Shape Shape { get; set; }
		public TextBoxProperties TextBoxProperties { get; set; }
		public FloatingObjectProperties FloatingObjectProperties { get; set; }
		public DocumentModelUnitConverter UnitConverter { get; set; }
		#endregion
		public override void CreateProperties() {
			SetRotationProperties();
			SetBooleanProtectionProperties();
			SetBlipIndexProperty();
			SetTextIndexProperty();
			SetTextBoxProperties();
			SetFillColorProperties();
			SetBlipBooleanProperties();
			SetDrawingTextBooleanProperties();
			SetFillStyleBooleanProperties();
			SetLineProrerties();
			SetLineStyleBooleanProperties();
			SetShapeBooleanProperties();
			SetShapeNameProperty();
			SetShapeDescriptionProperty();
			SetFloatingObjectDistancesProperties();
			SetShapeGroupBooleanProperties();
		}
		void SetFloatingObjectDistancesProperties() {
		}
		void SetTextBoxProperties() {
			if (Object.ReferenceEquals(TextBoxProperties, null))
				return;
			if (TextBoxProperties.UseLeftMargin)
				Properties.Add(new DrawingTextLeft(UnitConverter.ModelUnitsToEmu(TextBoxProperties.LeftMargin)));
			if (TextBoxProperties.UseTopMargin)
				Properties.Add(new DrawingTextTop(UnitConverter.ModelUnitsToEmu(TextBoxProperties.TopMargin)));
			if (TextBoxProperties.UseRightMargin)
				Properties.Add(new DrawingTextRight(UnitConverter.ModelUnitsToEmu(TextBoxProperties.RightMargin)));
			if (TextBoxProperties.UseBottomMargin)
				Properties.Add(new DrawingTextBottom(UnitConverter.ModelUnitsToEmu(TextBoxProperties.BottomMargin)));
		}
		void SetRotationProperties() {
			if (!Object.ReferenceEquals(Shape, null) && Shape.UseRotation)
				Properties.Add(new DrawingRotation(UnitConverter.ModelUnitsToDegree((int)Shape.Rotation)));
		}
		void SetFillColorProperties() {
			if (!Object.ReferenceEquals(Shape, null) && Shape.UseFillColor && !DXColor.IsTransparentOrEmpty(Shape.FillColor))
				Properties.Add(new DrawingFillColor(Shape.FillColor));
		}
		void SetLineProrerties() {
			if (Object.ReferenceEquals(Shape, null))
				return;
			if (Shape.UseOutlineColor)
				Properties.Add(new DrawingLineColor(Shape.OutlineColor));
			if (Shape.UseOutlineWidth)
				Properties.Add(new DrawingLineWidth(UnitConverter.ModelUnitsToEmu((int)Shape.OutlineWidth)));
		}
		void SetBooleanProtectionProperties() {
			Properties.Add(new DrawingBooleanProtectionProperties());
		}
		void SetBlipIndexProperty() {
			if (BlipIndex == 0)
				return;
			DrawingBlipIdentifier drawingBlipIdentifier = new DrawingBlipIdentifier();
			drawingBlipIdentifier.Value = BlipIndex;
			Properties.Add(drawingBlipIdentifier);
		}
		void SetTextIndexProperty() {
			if (TextIndex == 0)
				return;
			DrawingTextIdentifier drawingTextIdentifier = new DrawingTextIdentifier();
			drawingTextIdentifier.Value = TextIndex;
			Properties.Add(drawingTextIdentifier);
		}
		void SetBlipBooleanProperties() {
			if (BlipIndex != 0)
				Properties.Add(new DrawingBlipBooleanProperties());
		}
		void SetFillStyleBooleanProperties() {
			DrawingFillStyleBooleanProperties property = new DrawingFillStyleBooleanProperties();
			if (!Object.ReferenceEquals(Shape, null) && Shape.UseFillColor) {
				property.Filled = true;
				property.UseFilled = true;
			}
			Properties.Add(property);
		}
		void SetLineStyleBooleanProperties() {
			DrawingLineStyleBooleanProperties lineStyleBooleanProperties = new DrawingLineStyleBooleanProperties();
			if (Shape != null && Shape.UseOutlineColor && Shape.UseOutlineWidth) {
				lineStyleBooleanProperties.Line = true;
				lineStyleBooleanProperties.UseLine = true;
			}
			Properties.Add(lineStyleBooleanProperties);
		}
		void SetShapeBooleanProperties() {
			DrawingShapeBooleanProperties property = new DrawingShapeBooleanProperties();
			property.UseLockShapeType = true;
			property.LockShapeType = false;
			Properties.Add(property);
		}
		void SetShapeNameProperty() {
			if (BlipIndex == 0)
				return;
			DrawingShapeName property = new DrawingShapeName();
			property.Data = String.Format("Picture {0}\0", BlipIndex);
			Properties.Add(property);
		}
		protected virtual void SetShapeDescriptionProperty() {
		}
		void SetShapeGroupBooleanProperties() {
			DrawingGroupShapeBooleanProperties property = new DrawingGroupShapeBooleanProperties();
			if (FloatingObjectProperties != null) {
				property.IsBehindDoc = FloatingObjectProperties.IsBehindDoc;
				property.LayoutInCell = FloatingObjectProperties.LayoutInTableCell;
				property.UseLayoutInCell = FloatingObjectProperties.UseLayoutInTableCell;
			}
			Properties.Add(property);
		}
		void SetDrawingTextBooleanProperties() {
			DrawingTextBooleanProperties property = new DrawingTextBooleanProperties();
			if (TextBoxProperties != null && TextBoxProperties.UseResizeShapeToFitText) {
				property.FitShapeToText = TextBoxProperties.ResizeShapeToFitText;
				property.UseFitShapeToText = true;
			}
			Properties.Add(property);
		}
	}
	#endregion
}
