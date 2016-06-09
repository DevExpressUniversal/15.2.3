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
using System.Xml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.WordML {
	#region ParagraphDestination
	public class ParagraphDestination : DevExpress.XtraRichEdit.Import.OpenXml.ParagraphDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("pPr", OnParagraphProperties);
			result.Add("r", OnRun);
			result.Add("pict", OnPicture);
			result.Add("fldSimple", OnFieldSimple);
			result.Add("hlink", OnHlink);
			result.Add("fldChar", OnComplexFieldMarker);
			result.Add("instrText", OnFieldInstruction); 
			result.Add("annotation", OnAnnotation);
			result.Add("permStart", OnRangePermissionStart);
			result.Add("permEnd", OnRangePermissionEnd);
			return result;
		}
		public ParagraphDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal override DevExpress.XtraRichEdit.Import.OpenXml.ParagraphPropertiesDestination CreateParagraphPropertiesDestination() {
			return new ParagraphPropertiesDestination(Importer, this, Importer.Position.ParagraphFormatting, Importer.Position.ParagraphTabs);
		}
		static Destination OnRun(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateRunDestination();
		}
		protected static Destination OnAnnotation(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AnnotationElementDestination(importer);
		}
		static Destination OnHlink(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new HyperlinkDestination(importer);
		}
	}
	#endregion
	#region ParagraphPropertiesDestination
	public class ParagraphPropertiesDestination : DevExpress.XtraRichEdit.Import.OpenXml.ParagraphPropertiesDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static new ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = ParagraphPropertiesBaseDestination.CreateElementHandlerTable();
			result.Add("pStyle", OnStyle);
			result.Add("sectPr", OnSection);
			result.Add("listPr", OnNumbering);
			return result;
		}
		public ParagraphPropertiesDestination(WordProcessingMLBaseImporter importer, ParagraphDestination paragraphDestination, ParagraphFormattingBase paragraphFormatting, TabFormattingInfo tabs)
			: base(importer, paragraphDestination, paragraphFormatting, tabs) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static ParagraphPropertiesDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (ParagraphPropertiesDestination)importer.PeekDestination();
		}
		protected internal override SectionDestinationBase CreateSectionDestination() {
			return new InnerSectionDestination(Importer);
		}
		static Destination OnNumbering(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ParagraphNumberingReferenceDestination(importer, GetThis(importer));
		}
	}
	#endregion
	#region InnerSectionDestination
	public class InnerSectionDestination : SectionDestination {
		public InnerSectionDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
	}
	#endregion
	public class ParagraphNumberingReferenceDestination : DevExpress.XtraRichEdit.Import.OpenXml.ParagraphNumberingReferenceDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("ilvl", OnLevel);
			result.Add("ilfo", OnNumberingId);
			return result;
		}
		public ParagraphNumberingReferenceDestination(WordProcessingMLBaseImporter importer, ParagraphPropertiesBaseDestination paragraphPropertiesDestination)
			: base(importer, paragraphPropertiesDestination) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
}
