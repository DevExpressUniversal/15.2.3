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
using System.Diagnostics;
using System.Xml;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Import.OpenDocument;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	public class SectionDestination : TextDestination {
		ImportRangePermissionInfo protectionRange;
		public SectionDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		public ImportRangePermissionInfo ProtectionRange {
			get {
				if (protectionRange == null)
					protectionRange = new ImportRangePermissionInfo();
				return protectionRange;
			}
		}
		protected bool IsSectionProtected { get { return protectionRange != null; } }
		public override void ProcessElementOpen(XmlReader reader) {
			SectionInfo info = Importer.InputPosition.CurrentSectionInfo;
			info.SectionTagOpened = DocumentModel.DocumentCapabilities.SectionsAllowed;
			info.SectionTagClosed = false;
			info.StyleName = ImportHelper.GetTextStringAttribute(reader, "style-name");
			info.Name = ImportHelper.GetTextStringAttribute(reader, "name");
			bool isProtected = ImportHelper.GetTextBoolAttribute(reader, "protected", false);
			if (isProtected) {
				ProtectionRange.Start = Importer.InputPosition.LogPosition;
				DocumentProtectionProperties properties = Importer.DocumentModel.ProtectionProperties;	
				properties.EnforceProtection = true;
				string protectionKey = ImportHelper.GetTextStringAttribute(reader, "protection-key");
				if (!String.IsNullOrEmpty(protectionKey)) {
					properties.ProtectionType = DocumentProtectionType.ReadOnly;
					properties.OpenOfficePasswordHash = Convert.FromBase64String(protectionKey);
				}
			}
			TryToInsertSectionAferTableEnd(info);
		}
		void TryToInsertSectionAferTableEnd(SectionInfo info) {
			ParagraphIndex lastParagraphIndex = Importer.InputPosition.ParagraphIndex;
			if (lastParagraphIndex == ParagraphIndex.Zero)
				return;
			ParagraphIndex previousParagraph = lastParagraphIndex - 1;
			bool isNoParagraphAfterTable = Importer.PieceTable.Paragraphs[previousParagraph].GetCell() != null;
			if (isNoParagraphAfterTable) {
				Importer.InsertParagraph();
				Importer.InsertSection();
				info.SectionTagOpened = false;
				info.SectionTagClosed = false;
			}
		}
		public override void ProcessElementClose(XmlReader reader) {		  
			if (DocumentModel.DocumentCapabilities.SectionsAllowed) {
				Importer.InputPosition.CurrentSectionInfo.IsFirstAtDocument = false;
				Importer.InputPosition.CurrentParagraphInfo.IsFirstAtDocument = false;
				Importer.InputPosition.CurrentSectionInfo.SectionTagClosed = true;
				if (IsSectionProtected) {
					ProtectionRange.End = Importer.InputPosition.LogPosition;
					RegisterProtectionRange(ProtectionRange);
				}
			}
		}
		protected virtual void RegisterProtectionRange(ImportRangePermissionInfo range) {
		   string id = Importer.InputPosition.CurrentSectionInfo.Name;
		   Debug.Assert(!string.IsNullOrEmpty(id));
			if (!Importer.RangePermissions.ContainsKey(id)) {
				Importer.RangePermissions.Add(id, range);
			}
		}		
	}
}
