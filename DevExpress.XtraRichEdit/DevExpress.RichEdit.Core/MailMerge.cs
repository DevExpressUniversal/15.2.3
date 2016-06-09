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
using DevExpress.Data;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Internal;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region MailMergeDocumentModel
	public class MailMergeDocumentManager : DocumentModel {
		public MailMergeDocumentManager()
			: base(RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies()) {
		}
		MailMergeFieldInfoCollection fieldInfoCollection = new MailMergeFieldInfoCollection();
		public MailMergeFieldInfoCollection FieldInfoCollection { get { return fieldInfoCollection; } }
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region MailMergeRtfImporter
	public class MailMergeRtfImporter : RtfImporter {
		MailMergeFieldInfo fieldInfo;
		int level;
		public MailMergeRtfImporter(MailMergeDocumentManager documentModel)
			: base(documentModel, new RtfDocumentImporterOptions()) {
		}
		protected virtual MailMergeFieldInfo FieldInfo { get { return fieldInfo; } set { fieldInfo = value; } }
		protected override RtfParserStateManager CreateParserStateManager() {
			return new MailMergeRtfParserStateManager(this);
		}
		public virtual void StartMailMergeField(char ch) {
			if (level > 0) {
				Debug.Assert(FieldInfo != null);
				FieldInfo.FieldName += ch;
			}
			else {
				FieldInfo = new MailMergeFieldInfo();
				FieldInfo.StartPosition = (int)RtfStreamPosition - 1;
			}
			level++;
		}
		public virtual void EndMailMergeField(char ch) {
			level--;
			if (level > 0) {
				FieldInfo.FieldName += ch;
				return;
			}
			FieldInfo.EndPosition = (int)RtfStreamPosition - 1;
			FieldInfo.FieldName = fieldInfo.FieldName.Trim();
			if (fieldInfo.FieldName.Length > 0) {
				MailMergeDocumentManager documentModel = (MailMergeDocumentManager)DocumentModel;
				documentModel.FieldInfoCollection.Add(fieldInfo);
			}
			FieldInfo = null;
		}
		public virtual void ProcessMailMergeDefaultDestinationChar(char ch) {
			if (fieldInfo != null)
				FieldInfo.FieldName += ch;
		}
	}
	#endregion
	#region MailMergeRtfParserStateManager
	public class MailMergeRtfParserStateManager : RtfParserStateManager {
		public MailMergeRtfParserStateManager(MailMergeRtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		protected internal override DestinationBase CreateDefaultDestination() {
			return new MailMergeDefaultDestination(Importer, Importer.PieceTable);
		}
	}
	#endregion
	#region MailMergeDefaultDestination
	public class MailMergeDefaultDestination : DefaultDestination {
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("bin", OnBinKeyword);
			return table;
		}
		#endregion
		public MailMergeDefaultDestination(RtfImporter importer, PieceTable targetPieceTable)
			: base(importer, targetPieceTable) {
		}
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected internal override bool CanAppendText { get { return false; } }
		protected override DestinationBase CreateClone() {
			return new MailMergeDefaultDestination(Importer, PieceTable);
		}
		protected internal override RtfFormattingInfo CreateRtfFormattingInfo() {
			return new MailMergeRtfCharacterFormattingInfo();
		}
		protected override void ProcessCharCore(char ch) {
			MailMergeRtfImporter importer = (MailMergeRtfImporter)Importer;
			if (ch == '[')
				importer.StartMailMergeField(ch);
			else if (ch == ']')
				importer.EndMailMergeField(ch);
			else
				importer.ProcessMailMergeDefaultDestinationChar(ch);
		}
	}
	#endregion
	#region XRMailMergeRtfImporter
	public class XRMailMergeRtfImporter : MailMergeRtfImporter {
		public XRMailMergeRtfImporter(MailMergeDocumentManager documentModel)
			: base(documentModel) {
		}
		public override void StartMailMergeField(char ch) {
			FieldInfo = new MailMergeFieldInfo();
			FieldInfo.StartPosition = (int)RtfStreamPosition - 1;
		}
		public override void EndMailMergeField(char ch) {
			if (FieldInfo != null) {
				FieldInfo.EndPosition = (int)(RtfStreamPosition - 1);
				MailMergeDocumentManager documentModel = (MailMergeDocumentManager)DocumentModel;
				documentModel.FieldInfoCollection.Add(FieldInfo);
				FieldInfo = null;
			}
		}
		protected override RtfParserStateManager CreateParserStateManager() {
			return new XRMailMergeRtfParserStateManager(this);
		}
		public override void ProcessMailMergeDefaultDestinationChar(char ch) {
			if(!Position.CharacterFormatting.Hidden)
				base.ProcessMailMergeDefaultDestinationChar(ch);
		}
	}
	#endregion
	#region XRMailMergeRtfParserStateManager
	public class XRMailMergeRtfParserStateManager : RtfParserStateManager {
		public XRMailMergeRtfParserStateManager(XRMailMergeRtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		protected internal override DestinationBase CreateDefaultDestination() {
			return new XRMailMergeDefaultDestination(Importer, Importer.PieceTable);
		}
	}
	#endregion
	#region XRMailMergeDefaultDestination
	public class XRMailMergeDefaultDestination : DefaultDestination {
		public XRMailMergeDefaultDestination(RtfImporter importer, PieceTable targetPieceTable)
			: base(importer, targetPieceTable) {
		}
		protected internal override bool CanAppendText { get { return false; } }
		protected override DestinationBase CreateClone() {
			return new XRMailMergeDefaultDestination(Importer, PieceTable);
		}
		protected internal override RtfFormattingInfo CreateRtfFormattingInfo() {
			return new MailMergeRtfCharacterFormattingInfo();
		}
		protected override void ProcessCharCore(char ch) {
			XRMailMergeRtfImporter importer = (XRMailMergeRtfImporter)Importer;
			if (ch == '[') {
				importer.StartMailMergeField(ch);
			}
			else if (ch == ']') {
				importer.EndMailMergeField(ch);
			}
			else
				importer.ProcessMailMergeDefaultDestinationChar(ch);
			base.ProcessCharCore(ch);
		}
	}
	#endregion
	#region MailMergeRtfCharacterFormattingInfo
	public class MailMergeRtfCharacterFormattingInfo : RtfFormattingInfo {
		protected internal override CodePageCharacterDecoder ChooseDecoder() {
			return new MailMergeCodePageCharacterDecoder(CodePage);
		}
		protected internal override RtfFormattingInfo CreateEmptyClone() {
			return new MailMergeRtfCharacterFormattingInfo();
		}
	}
	#endregion
	#region MailMergeCodePageCharacterDecoder
	public class MailMergeCodePageCharacterDecoder : CodePageCharacterDecoder {
		public MailMergeCodePageCharacterDecoder(int codePage)
			: base(codePage) {
		}
		public override void ProcessChar(RtfImporter importer, char ch) {
			base.ProcessChar(importer, ch);
			if (ch == '[' || ch == ']')
				Flush(importer);
		}
	}
	#endregion
}
