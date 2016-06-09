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
using System.Xml;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.OpenXml;
using System.Globalization;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Import.WordML {
	#region InlinePictureDestination
	public class InlinePictureDestination : DevExpress.XtraRichEdit.Import.OpenXml.InlinePictureDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("shape", OnShape);
			result.Add("binData", OnBinData);
			result.Add("shapetype", OnShapeType);
			return result;
		}
		static readonly Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> horizontalPositionTypeAttributeTable = CreateHorizontalPositionTypeAttributeTable();
		static readonly Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> verticalPositionTypeAttributeTable = CreateVerticalPositionTypeAttributeTable();
		#region Translation tables
		#region CreateHorizontalPositionTypeAttributeTable
		static Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> CreateHorizontalPositionTypeAttributeTable() {
			Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> result = new Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue>();
			Add(result, FloatingObjectHorizontalPositionType.Column, "text");
			Add(result, FloatingObjectHorizontalPositionType.Margin, "margin");
			Add(result, FloatingObjectHorizontalPositionType.Page, "page");
			Add(result, FloatingObjectHorizontalPositionType.Character, "char");
			Add(result, FloatingObjectHorizontalPositionType.LeftMargin, "page");
			Add(result, FloatingObjectHorizontalPositionType.RightMargin, "page");
			Add(result, FloatingObjectHorizontalPositionType.InsideMargin, "page");
			Add(result, FloatingObjectHorizontalPositionType.OutsideMargin, "page");
			return result;
		}
		#endregion
		#region CreateVerticalPositionTypeAttributeTable
		static Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> CreateVerticalPositionTypeAttributeTable() {
			Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> result = new Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue>();
			Add(result, FloatingObjectVerticalPositionType.Margin, "margin");
			Add(result, FloatingObjectVerticalPositionType.Page, "margin");
			Add(result, FloatingObjectVerticalPositionType.Line, "line");
			Add(result, FloatingObjectVerticalPositionType.Paragraph, "text");
			Add(result, FloatingObjectVerticalPositionType.TopMargin, "page");
			Add(result, FloatingObjectVerticalPositionType.BottomMargin, "page");
			Add(result, FloatingObjectVerticalPositionType.InsideMargin, "page");
			Add(result, FloatingObjectVerticalPositionType.OutsideMargin, "page");
			return result;
		}
		#endregion
		#endregion
		string pictureUrl = String.Empty;
		Dictionary<string, OfficeNativeImage> picturesTable;
		static InlinePictureDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (InlinePictureDestination)importer.PeekDestination();
		}	 
		public InlinePictureDestination(WordMLImporter importer)
			: base(importer) {
			this.picturesTable = importer.PicturesTable;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal Dictionary<string, OfficeNativeImage> PicturesTable { get { return picturesTable; } }
		protected internal string PictureUrl { get { return pictureUrl; } set { pictureUrl = value; } }
		protected override Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> HorizontalPositionTypeAttributeTable { get { return horizontalPositionTypeAttributeTable; } }
		protected override Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> VerticalPositionTypeAttributeTable { get { return verticalPositionTypeAttributeTable; } }
		internal static Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> WordMLHorizontalPositionTypeAttributeTable { get { return horizontalPositionTypeAttributeTable; } }
		internal static Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> WordMLVerticalPositionTypeAttributeTable { get { return verticalPositionTypeAttributeTable; } }
		static Destination OnShape(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new VMLShapeDestination(importer, GetThis(importer));
		}
		protected static Destination OnBinData(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new PictureDataDestination(importer, GetThis(importer));
		}
		protected static Destination OnShapeType(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ShapeTypeDestination(importer);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (!FloatingObjectImportInfo.IsTextBox && FloatingObjectImportInfo.TextBoxContent == null) {
				if (PicturesTable.ContainsKey(PictureUrl) && ShouldInsertPicture()) {
					Image = new OfficeReferenceImage(DocumentModel, PicturesTable[PictureUrl]);
				}
			}
			base.ProcessElementClose(reader);
		}
	}
	#endregion
	#region ShapeTypeDestination
	public class ShapeTypeDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		string id;
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("lock", OnLock);
			return result;
		}
		public ShapeTypeDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			id = reader.GetAttribute("id");
		}
		static ShapeTypeDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (ShapeTypeDestination)importer.PeekDestination();
		}
		static Destination OnLock(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ShapeTypeLockDestination(importer, GetThis(importer).id);
		}
	}
	#endregion
	#region PictureDataDestination
	public class PictureDataDestination : LeafElementDestination {
		readonly InlinePictureDestination inlinePictureDestination;
		string pictureName;
		public PictureDataDestination(WordProcessingMLBaseImporter importer, InlinePictureDestination inlinePictureDestination)
			: base(importer) {
			Guard.ArgumentNotNull(inlinePictureDestination, "inlinePictureDestination");
			this.inlinePictureDestination = inlinePictureDestination;
		}
		public new WordMLImporter Importer { get { return (WordMLImporter)base.Importer; } }
		public override void ProcessElementOpen(XmlReader reader) {
			string name = reader.GetAttribute("name", Importer.WordProcessingNamespaceConst);
			if (String.IsNullOrEmpty(name))
				return;
			this.pictureName = name;
		}
		public override bool ProcessText(XmlReader reader) {
			if (inlinePictureDestination.PicturesTable.ContainsKey(pictureName) || !inlinePictureDestination.ShouldInsertPicture())
				return true;
			if (reader.CanReadBinaryContent) {
				MemoryStream memoryStream = new MemoryStream();
				byte[] bytes = new byte[4096];
				for (; ; ) {
					int bytesRead = reader.ReadContentAsBase64(bytes, 0, bytes.Length);
					memoryStream.Write(bytes, 0, bytesRead);
					if (bytesRead < bytes.Length)
						break;
				}
				if (memoryStream.Length > 0) {
					memoryStream.Seek(0, SeekOrigin.Begin);
					inlinePictureDestination.PicturesTable.Add(pictureName, DocumentModel.CreateImage(memoryStream).NativeRootImage);
				}
				return false;
			}
			else {
				string encodedPicture = reader.Value;
				if (!String.IsNullOrEmpty(encodedPicture)) {
					inlinePictureDestination.PicturesTable.Add(pictureName, DocumentModel.CreateImage(new MemoryStream(Convert.FromBase64String(encodedPicture))).NativeRootImage);
				}
				return true;
			}
		}
	}
	#endregion
	#region WrapDestination
	public class WrapDestination : LeafElementDestination {
		readonly FloatingObjectProperties floatingObject;
		public WrapDestination(WordProcessingMLBaseImporter importer, FloatingObjectProperties floatingObjectProperties)
			: base(importer) {
			this.floatingObject = floatingObjectProperties;
		}
		void ImportTextWrapType(XmlReader reader) {
			string textWrapType = reader.GetAttribute("type");
			Dictionary<FloatingObjectTextWrapType, WordProcessingMLValue> table = WordProcessingMLBaseExporter.floatingObjectTextWrapTypeTable;
			foreach (FloatingObjectTextWrapType key in table.Keys) {
				if (textWrapType == table[key].WordMLValue)
					floatingObject.TextWrapType = key;
			}
		}
		void ImportTextWrapSide(XmlReader reader) {
			string textWrapSide = reader.GetAttribute("side");
			Dictionary<FloatingObjectTextWrapSide, WordProcessingMLValue> table = WordProcessingMLBaseExporter.floatingObjectTextWrapSideTable;
			foreach (FloatingObjectTextWrapSide key in table.Keys) {
				if (textWrapSide == table[key].WordMLValue)
					floatingObject.TextWrapSide = key;
			}
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ImportTextWrapType(reader);
			ImportTextWrapSide(reader);
		}
	}
	#endregion
	#region ShapeLockDestination
	public class ShapeLockDestination : LeafElementDestination {
		readonly FloatingObjectProperties floatingObjectProperties;
		public ShapeLockDestination(WordProcessingMLBaseImporter importer, FloatingObjectProperties floatingObjectProperties)
			: base(importer) {
			this.floatingObjectProperties = floatingObjectProperties;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string aspectRatioValue = reader.GetAttribute("aspectratio");
			if (!String.IsNullOrEmpty(aspectRatioValue))
				floatingObjectProperties.LockAspectRatio = (aspectRatioValue == "t");
		}
	}
	#endregion
	#region ShapeTypeLockDestination
	public class ShapeTypeLockDestination : LeafElementDestination {
		string shapeTypeId;
		public ShapeTypeLockDestination(WordProcessingMLBaseImporter importer, string shapeTypeId)
			: base(importer) {
			this.shapeTypeId = shapeTypeId;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string aspectRatioValue = reader.GetAttribute("aspectratio");
			((WordMLImporter)Importer).LockAspectRatioTableAddValue(shapeTypeId, aspectRatioValue);
		}
	}
	#endregion
	#region VMLShapeDestination
	public class VMLShapeDestination : DevExpress.XtraRichEdit.Import.OpenXml.VMLShapeDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		readonly InlinePictureDestination inlinePictureDestination;
		string pictureUrl;
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("imagedata", OnImageData);
			result.Add("wrap", OnWrap);
			result.Add("lock", OnLock);
			result.Add("textbox", OnTextBox);
			return result;
		}
		public VMLShapeDestination(WordProcessingMLBaseImporter importer, InlinePictureDestination inlinePictureDestination)
			: base(importer, inlinePictureDestination) {
			this.inlinePictureDestination = inlinePictureDestination;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public string PictureUrl { get { return pictureUrl; } set { pictureUrl = value; } }
		protected internal Dictionary<string, OfficeNativeImage> PicturesTable { get { return inlinePictureDestination.PicturesTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			inlinePictureDestination.PictureUrl = this.PictureUrl;
			base.ProcessElementClose(reader);
		}
		static VMLShapeDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (VMLShapeDestination)importer.PeekDestination();
		}
		static Destination OnImageData(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new VMLImageDataDestination(importer, GetThis(importer));
		}
		static Destination OnWrap(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WrapDestination(importer, (GetThis(importer)).FloatingObject);
		}
		static Destination OnLock(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ShapeLockDestination(importer, GetThis(importer).FloatingObject);
		}
		static Destination OnTextBox(WordProcessingMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).FloatingObjectImportInfo.IsTextBox = true;
			return new WordMLTextBoxDestination(importer, GetThis(importer).FloatingObjectImportInfo);
		}
	}
	#endregion
	#region WordMLTextBoxDestination
	public class WordMLTextBoxDestination : TextBoxDestination {
		public WordMLTextBoxDestination(WordProcessingMLBaseImporter importer, FloatingObjectImportInfo floatingObjectImportInfo)
			: base(importer, floatingObjectImportInfo) {
		}
	}
	#endregion
	#region VMLImageDataDestination
	public class VMLImageDataDestination : DevExpress.XtraRichEdit.Import.OpenXml.VMLImageDataDestination {
		readonly VMLShapeDestination shapeDestination;
		public VMLImageDataDestination(WordProcessingMLBaseImporter importer, VMLShapeDestination shapeDestination)
			: base(importer, shapeDestination.FloatingObjectImportInfo) {
			this.shapeDestination = shapeDestination;
		}
		public new WordMLImporter Importer { get { return (WordMLImporter)base.Importer; } }
		protected internal Dictionary<string, OfficeNativeImage> PicturesTable { get { return ShapeDestination.PicturesTable; } }
		public VMLShapeDestination ShapeDestination { get { return shapeDestination; } }
		public override void ProcessElementOpen(XmlReader reader) {
			string src = reader.GetAttribute("src");
			if (String.IsNullOrEmpty(src))
				return;
			ShapeDestination.PictureUrl = src;
		}
	}
	#endregion
}
