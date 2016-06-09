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
using System.Reflection;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region DocObjectTypes
	public enum DocObjectType {
		PictureFloatingObject,
		TextBoxFloatingObject,
		InlineImage,
		ImageCollection,
		AutoNumberedFootnoteReference,
		NoteNumber,
		AnnotationReference,
		EndnoteReference,
		FieldBegin,
		FieldSeparator,
		FieldEnd,
		HyperlinkFieldData,
		TextRun,
		TableCell,
		TableRow,
		Paragraph,
		Section,
		ExpectedFieldBegin,
		ExpectedFieldSeparator,
		ExpectedFieldEnd,
		UnsupportedObject
	}
	#endregion
	public class DocObjectCollection : List<IDocObject> {
	}
	#region DocObjectInfo
	public class DocObjectInfo {
		#region Fields
		int position;
		string text;
		#endregion
		public DocObjectInfo(int position, string text) {
			this.position = position;
			this.text = text;
		}
		#region Properties
		public int Position {
			get { return this.position; }
			set { this.position = value; }
		}
		public string Text { get { return this.text; } }
		#endregion
	}
	#endregion
	#region IDocObject
	public interface IDocObject {
		DocObjectType DocObjectType { get; }
		DocPropertyContainer PropertyContainer { get; }
		int Position { get; }
		int Length { get; }
	}
	#endregion
	#region DocObjectBase (abstract class)
	public abstract class DocObjectBase : IDocObject {
		readonly int position;
		readonly DocPropertyContainer propertyContainer;
		protected DocObjectBase(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			this.position = objectInfo.Position;
			this.propertyContainer = propertyContainer;
		}
		#region IDocObject Members
		public abstract DocObjectType DocObjectType { get; }
		public DocPropertyContainer PropertyContainer { get { return this.propertyContainer; } }
		public int Position { get { return this.position; } }
		public virtual int Length { get { return 1; } }
		#endregion
	}
	#endregion
	#region DocObjectFactory
	public sealed class DocObjectFactory {
		#region static
		static Dictionary<DocObjectType, ConstructorInfo> objectTypes;
		static readonly DocObjectFactory instance = new DocObjectFactory();
		static DocObjectFactory() {
			objectTypes = new Dictionary<DocObjectType, ConstructorInfo>();
			RegisterObjectType(DocObjectType.InlineImage, typeof(DocImage));
			RegisterObjectType(DocObjectType.NoteNumber, typeof(DocNoteNumber));
			RegisterObjectType(DocObjectType.AutoNumberedFootnoteReference, typeof(DocAutoNumberedFootnoteReference));
			RegisterObjectType(DocObjectType.EndnoteReference, typeof(DocEndnoteReference));
			RegisterObjectType(DocObjectType.AnnotationReference, typeof(DocAnnotationReference));
			RegisterObjectType(DocObjectType.FieldBegin, typeof(DocFieldBegin));
			RegisterObjectType(DocObjectType.FieldSeparator, typeof(DocFieldSeparator));
			RegisterObjectType(DocObjectType.FieldEnd, typeof(DocFieldEnd));
			RegisterObjectType(DocObjectType.ExpectedFieldBegin, typeof(ExpectedFieldBegin));
			RegisterObjectType(DocObjectType.ExpectedFieldSeparator, typeof(ExpectedFieldSeparator));
			RegisterObjectType(DocObjectType.ExpectedFieldEnd, typeof(ExpectedFieldEnd));
			RegisterObjectType(DocObjectType.TextRun, typeof(DocTextRun));
			RegisterObjectType(DocObjectType.TableCell, typeof(DocTableCell));
			RegisterObjectType(DocObjectType.TableRow, typeof(DocTableRow));
			RegisterObjectType(DocObjectType.Paragraph, typeof(DocParagraph));
			RegisterObjectType(DocObjectType.Section, typeof(DocSection));
		}
		static void RegisterObjectType(DocObjectType objectType, Type type) {
			objectTypes.Add(objectType, type.GetConstructor(new Type[] { typeof(DocObjectInfo), typeof(DocPropertyContainer) }));
		}
		#endregion
		DocObjectFactory() { }
		public static DocObjectFactory Instance { get { return instance; } }
		public DocObjectBase CreateDocObject(DocObjectType objectType, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			ConstructorInfo constructor;
			if (!objectTypes.TryGetValue(objectType, out constructor))
				return null;
			return constructor.Invoke(new object[] { objectInfo, propertyContainer }) as DocObjectBase;
		}
	}
	#endregion
	#region DocTextRun
	public class DocTextRun : DocObjectBase {
		#region Fields
		string text;
		#endregion
		public DocTextRun(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) {
			this.text = objectInfo.Text;
		}
		#region Properties
		public override DocObjectType DocObjectType { get { return DocObjectType.TextRun; } }
		public override int Length { get { return text.Length; } }
		public string Text { get { return text; } }
		#endregion
	}
	#endregion
	#region DocNoteNumber
	public class DocNoteNumber : DocObjectBase {
		public DocNoteNumber(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) { }
		public override DocObjectType DocObjectType { get { return DocObjectType.NoteNumber; } }
	}
	#endregion
	#region DocAutoNumberedFootnoteReference
	public class DocAutoNumberedFootnoteReference : DocObjectBase {
		public DocAutoNumberedFootnoteReference(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) { }
		public override DocObjectType DocObjectType { get { return DocObjectType.AutoNumberedFootnoteReference; } }
	}
	#endregion
	#region DocEndnoteReference
	public class DocEndnoteReference : DocObjectBase {
		public DocEndnoteReference(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) { }
		public override DocObjectType DocObjectType { get { return DocObjectType.EndnoteReference; } }
	}
	#endregion
	#region DocAnnotationReference
	public class DocAnnotationReference : DocObjectBase {
		public DocAnnotationReference(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) { }
		public override DocObjectType DocObjectType { get { return DocObjectType.AnnotationReference; } }
	}
	#endregion
	#region DocParagraph
	public class DocParagraph : DocObjectBase {
		public DocParagraph(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) { }
		public override DocObjectType DocObjectType { get { return DocObjectType.Paragraph; } }
	}
	#endregion
	#region DocSection
	public class DocSection : DocObjectBase {
		public DocSection(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) { }
		public override DocObjectType DocObjectType { get { return DocObjectType.Section; } }
	}
	#endregion
	#region DocImage
	public class DocImage : DocObjectBase {
		#region Fields
		OfficeImage image;
		int scaleX;
		int scaleY;
		DocMetafileHeader metafileHeader;
		#endregion
		public DocImage(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) { }
		protected internal DocImage(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer, PictureDescriptor descriptor, int pictureIndex)
			: base(objectInfo, propertyContainer) {
			this.image = descriptor.Images[pictureIndex];
			this.scaleX = Math.Max(1, Convert.ToInt32((descriptor.Width * descriptor.HorizontalScaleFactor) / (Image.SizeInTwips.Width * 10.0)));
			this.scaleY = Math.Max(1, Convert.ToInt32((descriptor.Height * descriptor.VerticalScaleFactor) / (Image.SizeInTwips.Height * 10.0)));
			this.metafileHeader = descriptor.MetafileHeaders[pictureIndex];
		}
		#region Properties
		public override DocObjectType DocObjectType { get { return DocObjectType.InlineImage; } }
		public OfficeImage Image { get { return this.image; } }
		public int ScaleX { get { return this.scaleX; } }
		public int ScaleY { get { return this.scaleY; } }
		public DocMetafileHeader MetafileHeader { get { return this.metafileHeader; } }
		#endregion
	}
	#endregion
	#region DocImageCollection
	public class DocImageCollection : List<DocImage>, IDocObject {
		int position;
		DocPropertyContainer propertyContainer;
		public DocImageCollection(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer, PictureDescriptor descriptor) {
			this.position = objectInfo.Position;
			this.propertyContainer = propertyContainer;
			for (int i = 0; i < descriptor.Images.Count; i++) {
				if (descriptor.Images[i] == null)
					continue;
				DocImage item = new DocImage(objectInfo, propertyContainer, descriptor, i);
				this.Add(item);
			}
		}
		#region IDocObject Members
		public DocObjectType DocObjectType { get { return DocObjectType.ImageCollection; } }
		public DocPropertyContainer PropertyContainer { get { return this.propertyContainer; } }
		public int Position { get { return this.position; } }
		public int Length { get { return 1; } }
		#endregion
	}
	#endregion
	#region DocFloatingObjectBase (abstract class)
	public abstract class DocFloatingObjectBase : DocObjectBase {
		protected DocFloatingObjectBase(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) {
		}
		public FloatingObjectFormatting Formatting { get; protected internal set; }
		public int Rotation { get; protected internal set; }
		public int LineWidth { get; protected internal set; }
		public Color FillColor { get; protected set; }
		public Color LineColor { get; protected set; }
		public int WrapLeftDistance { get; protected set; }
		public int WrapRightDistance { get; protected set; }
		public int WrapTopDistance { get; protected set; }
		public int WrapBottomDistance { get; protected set; }
		protected internal virtual void ApplyFileShapeAddress(FileShapeAddress address) {
			Formatting.Locked = address.Locked;
			int x = PropertyContainer.UnitConverter.TwipsToModelUnits(address.Left);
			int y = PropertyContainer.UnitConverter.TwipsToModelUnits(address.Top);
			int width = PropertyContainer.UnitConverter.TwipsToModelUnits(address.Right) - x;
			int height = PropertyContainer.UnitConverter.TwipsToModelUnits(address.Bottom) - y;
			if (x < int.MinValue / 2)
				x = 0;
			if (y < int.MinValue / 2)
				y = 0;
			Formatting.Offset = new Point(x, y);
			Formatting.ActualSize = new Size(width, height);
			Formatting.HorizontalPositionType = address.HorisontalPositionType;
			Formatting.VerticalPositionType = address.VericalPositionType;
			Formatting.TextWrapSide = address.TextWrapSide;
			Formatting.TextWrapType = address.TextWrapType;
			if (address.UseIsBehindDoc)
				Formatting.IsBehindDoc = address.IsBehindDoc;
		}
		public void ApplyShapeProperties(Shape shape) {
			if (shape.Rotation != Rotation)
				shape.Rotation = Rotation;
			if (shape.OutlineWidth != LineWidth)
				shape.OutlineWidth = LineWidth;
			if (shape.OutlineColor != LineColor)
				shape.OutlineColor = LineColor;
			if (shape.FillColor != FillColor)
				shape.FillColor = FillColor;
		}
		protected internal virtual void SetOfficeArtProperties(OfficeArtProperties properties) {
			DocumentModelUnitConverter unitConverter = PropertyContainer.UnitConverter;
			SetOfficeArtPropertiesCore(properties, unitConverter);
		}
		protected internal virtual void SetOfficeArtTertiaryProperties(OfficeArtTertiaryProperties properties) {
			if (properties.UseRelativeWidth)
				Formatting.RelativeWidth = new FloatingObjectRelativeWidth(GetRelativeFrom(properties.SizeRelH), properties.PctHoriz * 100);
			if (properties.UseRelativeHeight)
				Formatting.RelativeHeight = new FloatingObjectRelativeHeight(GetRelativeFrom(properties.SizeRelV), properties.PctVert * 100);
			int actualHorizPos = 0;
			int actualVertPos = 0;
			if (properties.PctHorizPosValid || properties.PctVertPosValid) {
				actualHorizPos = properties.PctHorizPosValid ? properties.PctHorizPos * 100 : 0;
				actualVertPos = properties.PctVertPosValid ? properties.PctVertPos * 100 : 0;
			}
			if (properties.UsePosH) {
				Formatting.HorizontalPositionAlignment = GetHorizontalPositionAlignment(properties.PosH);
				Formatting.HorizontalPositionType = GetHorizontalPositionType(properties.PosRelH);
				Point pt = Formatting.PercentOffset;
				pt.X = actualHorizPos;
				Formatting.PercentOffset = pt;
			}
			if (properties.UsePosV) {
				Formatting.VerticalPositionAlignment = GetVerticalPositionAlignment(properties.PosV);
				Formatting.VerticalPositionType = GetVerticalPositionType(properties.PosRelV);
				Point pt = Formatting.PercentOffset;
				pt.Y = actualVertPos;
				Formatting.PercentOffset = pt;
			}
			if (properties.UseLayoutInCell)
				Formatting.LayoutInTableCell = properties.LayoutInCell;
			else
				Formatting.LayoutInTableCell = true;
		}
		FloatingObjectHorizontalPositionAlignment GetHorizontalPositionAlignment(DrawingGroupShapePosH.Msoph msoph) {
			switch (msoph) {
				case DrawingGroupShapePosH.Msoph.msophAbs:
					return FloatingObjectHorizontalPositionAlignment.None;
				case DrawingGroupShapePosH.Msoph.msophCenter:
					return FloatingObjectHorizontalPositionAlignment.Center;
				case DrawingGroupShapePosH.Msoph.msophInside:
					return FloatingObjectHorizontalPositionAlignment.Inside;
				case DrawingGroupShapePosH.Msoph.msophLeft:
					return FloatingObjectHorizontalPositionAlignment.Left;
				case DrawingGroupShapePosH.Msoph.msophOutside:
					return FloatingObjectHorizontalPositionAlignment.Outside;
				case DrawingGroupShapePosH.Msoph.msophRight:
					return FloatingObjectHorizontalPositionAlignment.Right;
				default:
					return FloatingObjectHorizontalPositionAlignment.None;
			}
		}
		FloatingObjectHorizontalPositionType GetHorizontalPositionType(DrawingGroupShapePosRelH.Msoprh msoprh) {
			switch (msoprh) {
				case DrawingGroupShapePosRelH.Msoprh.msoprhChar:
					return FloatingObjectHorizontalPositionType.Character;
				case DrawingGroupShapePosRelH.Msoprh.msoprhMargin:
					return FloatingObjectHorizontalPositionType.Margin;
				case DrawingGroupShapePosRelH.Msoprh.msoprhPage:
					return FloatingObjectHorizontalPositionType.Page;
				case DrawingGroupShapePosRelH.Msoprh.msoprhText:
					return FloatingObjectHorizontalPositionType.Column;
				default:
					return FloatingObjectHorizontalPositionType.Column;
			}
		}
		FloatingObjectVerticalPositionAlignment GetVerticalPositionAlignment(DrawingGroupShapePosV.Msopv msopv) {
			switch (msopv) {
				case DrawingGroupShapePosV.Msopv.msopvAbs:
					return FloatingObjectVerticalPositionAlignment.None;
				case DrawingGroupShapePosV.Msopv.msopvBottom:
					return FloatingObjectVerticalPositionAlignment.Bottom;
				case DrawingGroupShapePosV.Msopv.msopvCenter:
					return FloatingObjectVerticalPositionAlignment.Center;
				case DrawingGroupShapePosV.Msopv.msopvInside:
					return FloatingObjectVerticalPositionAlignment.Inside;
				case DrawingGroupShapePosV.Msopv.msopvOutside:
					return FloatingObjectVerticalPositionAlignment.Outside;
				case DrawingGroupShapePosV.Msopv.msopvTop:
					return FloatingObjectVerticalPositionAlignment.Top;
				default:
					return FloatingObjectVerticalPositionAlignment.None;
			}
		}
		FloatingObjectVerticalPositionType GetVerticalPositionType(DrawingGroupShapePosRelV.Msoprv msoprv) {
			switch (msoprv) {
				case DrawingGroupShapePosRelV.Msoprv.msoprvLine:
					return FloatingObjectVerticalPositionType.Line;
				case DrawingGroupShapePosRelV.Msoprv.msoprvMargin:
					return FloatingObjectVerticalPositionType.Margin;
				case DrawingGroupShapePosRelV.Msoprv.msoprvPage:
					return FloatingObjectVerticalPositionType.Page;
				case DrawingGroupShapePosRelV.Msoprv.msoprvText:
					return FloatingObjectVerticalPositionType.Paragraph;
				default:
					return FloatingObjectVerticalPositionType.Paragraph;
			}
		}
		FloatingObjectRelativeFromHorizontal GetRelativeFrom(DrawingGroupShape2SizeRelH.RelativeFrom relativeFrom) {
			return (FloatingObjectRelativeFromHorizontal)relativeFrom;
		}
		FloatingObjectRelativeFromVertical GetRelativeFrom(DrawingGroupShape2SizeRelV.RelativeFrom relativeFrom) {
			return (FloatingObjectRelativeFromVertical)relativeFrom;
		}
		protected internal virtual void SetOfficeArtPropertiesCore(OfficeArtProperties properties, DocumentModelUnitConverter unitConverter) {
			if (properties.ZOrder != 0)
				Formatting.ZOrder = properties.ZOrder;
			if (properties.UseIsBehindDoc)
				Formatting.IsBehindDoc = properties.IsBehindDoc;
			int rotation = unitConverter.DegreeToModelUnits((int)properties.Rotation);
			if (rotation != 0)
				Rotation = rotation;
			if (!properties.UseLine || (properties.UseLine && properties.Line)) {
				LineWidth = unitConverter.EmuToModelUnits(OfficeArtConstants.DefaultLineWidthInEmus);
				LineColor = DXColor.Black;
			}
			if (properties.UseFilled)
				FillColor = properties.Filled ? DXColor.White : DXColor.Empty;
			int lineWidth = unitConverter.EmuToModelUnits((int)properties.LineWidth);
			if (lineWidth != 0)
				LineWidth = lineWidth;
			Formatting.LeftDistance = unitConverter.EmuToModelUnits((int)(properties.UseWrapLeftDistance ? properties.WrapLeftDistance : DrawingWrapLeftDistance.DefaultValue));
			Formatting.RightDistance = unitConverter.EmuToModelUnits((int)(properties.UseWrapRightDistance ? properties.WrapRightDistance : DrawingWrapRightDistance.DefaultValue));
			Formatting.TopDistance = unitConverter.EmuToModelUnits((int)(properties.UseWrapTopDistance ? properties.WrapTopDistance : DrawingWrapTopDistance.DefaultValue));
			Formatting.BottomDistance = unitConverter.EmuToModelUnits((int)(properties.UseWrapBottomDistance ? properties.WrapBottomDistance : DrawingWrapBottomDistance.DefaultValue));
#if!SL
			if (!properties.LineColor.IsEmpty)
				LineColor = properties.LineColor;
			if (!properties.FillColor.IsEmpty && properties.UseFilled && properties.Filled)
				FillColor = properties.FillColor;
#else
			if (properties.LineColor.R != 0 && properties.LineColor.G != 0 && properties.LineColor.B != 0)
				LineColor = properties.LineColor;
			if (properties.FillColor.R != 0 && properties.FillColor.G != 0 && properties.FillColor.B != 0 && properties.UseFilled && properties.Filled)
				FillColor = properties.FillColor;
#endif
		}
	}
	#endregion
	#region DocPictureFloatingObject
	public class DocPictureFloatingObject : DocFloatingObjectBase {
		#region Fields
		readonly OfficeImage image;
		#endregion
		public DocPictureFloatingObject(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) {
		}
		public DocPictureFloatingObject(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer, BlipBase blip)
			: base(objectInfo, propertyContainer) {
			this.image = blip.Image;
		}
		#region Properties
		public override DocObjectType DocObjectType { get { return DocObjectType.PictureFloatingObject; } }
		public OfficeImage Image { get { return this.image; } }
		#endregion
	}
	#endregion
	#region DocTextBoxFloatingObject
	public class DocTextBoxFloatingObject : DocFloatingObjectBase {
		#region Fields
		int shapeId;
		#endregion
		public DocTextBoxFloatingObject(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer, int shapeId)
			: base(objectInfo, propertyContainer) {
			this.shapeId = shapeId;
		}
		#region Properties
		public override DocObjectType DocObjectType { get { return DocObjectType.TextBoxFloatingObject; } }
		public int ShapeId { get { return shapeId; } }
		public bool UseTextTop { get; protected internal set; }
		public bool UseTextBottom { get; protected internal set; }
		public bool UseTextRight { get; protected internal set; }
		public bool UseTextLeft { get; protected internal set; }
		public bool UseFitShapeToText { get; protected internal set; }
		public bool FitShapeToText { get; protected internal set; }
		public int TextTop { get; protected internal set; }
		public int TextBottom { get; protected internal set; }
		public int TextLeft { get; protected internal set; }
		public int TextRight { get; protected internal set; }
		#endregion
		protected internal override void SetOfficeArtPropertiesCore(OfficeArtProperties properties, DocumentModelUnitConverter unitConverter) {
			base.SetOfficeArtPropertiesCore(properties, unitConverter);
			if (properties.UseTextTop) {
				UseTextTop = true;
				TextTop = unitConverter.EmuToModelUnits(properties.TextTop);
			}
			if (properties.UseTextBottom) {
				UseTextBottom = true;
				TextBottom = unitConverter.EmuToModelUnits(properties.TextBottom);
			}
			if (properties.UseTextLeft) {
				UseTextLeft = true;
				TextLeft = unitConverter.EmuToModelUnits(properties.TextLeft);
			}
			if (properties.UseTextRight) {
				UseTextRight = true;
				TextRight = unitConverter.EmuToModelUnits(properties.TextRight);
			}
			if (properties.UseFitShapeToText) {
				UseFitShapeToText = true;
				FitShapeToText = properties.FitShapeToText;
			}
		}
		public void ApplyTextBoxProperties(TextBoxProperties properties) {
			if (UseTextTop)
				properties.TopMargin = TextTop;
			if (UseTextBottom)
				properties.BottomMargin = TextBottom;
			if (UseTextLeft)
				properties.LeftMargin = TextLeft;
			if (UseTextRight)
				properties.RightMargin = TextRight;
			if (UseFitShapeToText)
				properties.ResizeShapeToFitText = FitShapeToText;
		}
	}
	#endregion
	#region DocFieldBegin
	public class DocFieldBegin : DocObjectBase {
		public DocFieldBegin(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) { }
		public override DocObjectType DocObjectType { get { return DocObjectType.FieldBegin; } }
	}
	#endregion
	#region DocFieldSeparator
	public class DocFieldSeparator : DocObjectBase {
		#region Fields
		bool oleObject;
		#endregion
		public DocFieldSeparator(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) {
			if (propertyContainer.CharacterInfo != null)
				this.oleObject = propertyContainer.CharacterInfo.EmbeddedObject || propertyContainer.CharacterInfo.Ole2Object;
		}
		#region Properties
		public override DocObjectType DocObjectType { get { return DocObjectType.FieldSeparator; } }
		public bool OleObject { get { return this.oleObject; } }
		#endregion
	}
	#endregion
	#region DocFieldEnd
	public class DocFieldEnd : DocObjectBase {
		public DocFieldEnd(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) { }
		public override DocObjectType DocObjectType { get { return DocObjectType.FieldEnd; } }
	}
	#endregion
	public abstract class ExpectedDocObject : DocObjectBase {
		DocObjectType type;
		string text;
		protected ExpectedDocObject(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) {
			this.type = GetExpectedType();
			this.text = objectInfo.Text;
		}
		protected abstract DocObjectType ActualType { get; }
		protected abstract DocObjectType GetExpectedType();
		public override DocObjectType DocObjectType { get { return type; } }
		public string Text { get { return text; } }
		public void SetActualType() {
			this.type = ActualType;
		}
	}
	#region ExpectedFieldBegin
	public class ExpectedFieldBegin : ExpectedDocObject {
		public ExpectedFieldBegin(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) { }
		protected override DocObjectType ActualType { get { return DocObjectType.FieldBegin; } }
		protected override DocObjectType GetExpectedType() {
			return DocObjectType.ExpectedFieldBegin;
		}
	}
	#endregion
	#region ExpectedFieldSeparator
	public class ExpectedFieldSeparator : ExpectedDocObject {
		public ExpectedFieldSeparator(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) { }
		protected override DocObjectType ActualType { get { return DocObjectType.FieldSeparator; } }
		protected override DocObjectType GetExpectedType() {
			return DocObjectType.ExpectedFieldSeparator;
		}
	}
	#endregion
	#region ExpectedFieldEnd
	public class ExpectedFieldEnd : ExpectedDocObject {
		public ExpectedFieldEnd(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) { }
		protected override DocObjectType ActualType { get { return Doc.DocObjectType.FieldEnd; } }
		protected override DocObjectType GetExpectedType() {
			return DocObjectType.ExpectedFieldEnd;
		}
	}
	#endregion
	#region DocHyperlinkFieldData
	public class DocHyperlinkFieldData : DocObjectBase {
		#region Fields
		DocHyperlinkInfo hyperlinkInfo;
		#endregion
		public DocHyperlinkFieldData(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer, DocHyperlinkInfo hyperlinkInfo)
			: base(objectInfo, propertyContainer) {
			this.hyperlinkInfo = hyperlinkInfo;
		}
		#region Properties
		public override DocObjectType DocObjectType { get { return DocObjectType.HyperlinkFieldData; } }
		public DocHyperlinkInfo HyperlinkInfo { get { return this.hyperlinkInfo; } }
		#endregion
	}
	#endregion
	#region DocTableCell
	public class DocTableCell : DocObjectBase {
		public DocTableCell(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) { }
		public override DocObjectType DocObjectType {
			get { return DocObjectType.TableCell; }
		}
	}
	#endregion
	#region DocTableRow
	public class DocTableRow : DocObjectBase {
		public DocTableRow(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer)
			: base(objectInfo, propertyContainer) { }
		public override DocObjectType DocObjectType {
			get { return DocObjectType.TableRow; }
		}
	}
	#endregion
}
