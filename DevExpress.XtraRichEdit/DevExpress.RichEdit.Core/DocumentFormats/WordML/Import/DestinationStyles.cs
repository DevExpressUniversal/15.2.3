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
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.WordML {
	#region StylesDestination
	public class StylesDestination : DevExpress.XtraRichEdit.Import.OpenXml.StylesDestination {
		public StylesDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementClose(XmlReader reader) {
			Importer.CreateStylesHierarchy();
			Importer.LinkStyles();
			base.ProcessElementClose(reader);
		}
	}
	#endregion
	public class StyleParagraphPropertiesDestination : DevExpress.XtraRichEdit.Import.OpenXml.StyleParagraphPropertiesDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static new ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = ParagraphPropertiesBaseDestination.CreateElementHandlerTable();
			result.Add("listPr", OnNumbering);
			return result;
		}
		public StyleParagraphPropertiesDestination(WordProcessingMLBaseImporter importer, StyleDestinationBase styleDestination, ParagraphFormattingBase paragraphFormatting, TabFormattingInfo tabs)
			: base(importer, styleDestination, paragraphFormatting, tabs) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static StyleParagraphPropertiesDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (StyleParagraphPropertiesDestination)importer.PeekDestination();
		}
		static Destination OnNumbering(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ParagraphNumberingReferenceDestination(importer, GetThis(importer));
		}
	}
}
