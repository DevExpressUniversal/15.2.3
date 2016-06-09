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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.WordML {
	#region RunDestination
	public class RunDestination : DevExpress.XtraRichEdit.Import.OpenXml.RunDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("rPr", OnRunProperties);
			result.Add("t", OnText);
			result.Add("instrText", OnText); 
			result.Add("cr", OnCarriageReturn);
			result.Add("br", OnBreak);
			result.Add("tab", OnTab);
			result.Add("pict", OnPicture);
			result.Add("object", OnObject);
			result.Add("fldChar", OnComplexFieldMarker);
			result.Add("drawing", OnDrawing);
			result.Add("footnote", OnFootNote);
			result.Add("endnote", OnEndNote);
			result.Add("footnoteRef", OnFootNoteSelfReference);
			result.Add("endnoteRef", OnEndNoteSelfReference);
			result.Add("annotation", OnAnnotation);
			return result;
		}
		public RunDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnPicture(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new InlinePictureDestination((WordMLImporter)importer);
		}
		static Destination OnFootNote(WordProcessingMLBaseImporter importer, XmlReader reader) {
			if (!importer.DocumentModel.DocumentCapabilities.FootNotesAllowed)
				return null;
			return new FootNoteDestination(importer);
		}
		static Destination OnEndNote(WordProcessingMLBaseImporter importer, XmlReader reader) {
			if (!importer.DocumentModel.DocumentCapabilities.EndNotesAllowed)
				return null;
			return new EndNoteDestination(importer);
		}
		static Destination OnAnnotation(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AnnotationCommentDestination(importer);
		}
	}
	#endregion
}
