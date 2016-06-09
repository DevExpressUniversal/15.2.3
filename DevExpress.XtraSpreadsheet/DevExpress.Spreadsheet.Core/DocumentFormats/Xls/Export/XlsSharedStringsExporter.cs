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
using System.IO;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	#region XlsSharedStringsExporterBase
	public abstract class XlsSharedStringsExporterBase : XlsExporterBase {
		const int recordHeaderSize = 4;
		protected XlsSharedStringsExporterBase(BinaryWriter streamWriter, DocumentModel documentModel, ExportXlsStyleSheet exportStyleSheet)
			: base(streamWriter, documentModel, exportStyleSheet) {
		}
		public override void WriteContent() {
			XlsCommandSharedStrings sstCommand = new XlsCommandSharedStrings();
			XlsCommandContinue continueCommand = new XlsCommandContinue();
			using(XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, sstCommand, continueCommand)) {
				writer.Write(GetSharedStringsTotalRefCount()); 
				int count = GetSharedStringsCount(); 
				writer.Write(count);
				for(int i = 0; i < count; i++) {
					XLUnicodeRichExtendedString item = GetSharedStringsItem(i);
					BeforeStringWrite(i, StreamWriter.BaseStream.Position, (int)(writer.BaseStream.Position + recordHeaderSize));
					item.Write(writer);
				}
			}
		}
		protected abstract int GetSharedStringsTotalRefCount();
		protected abstract int GetSharedStringsCount();
		protected abstract XLUnicodeRichExtendedString GetSharedStringsItem(int index);
		protected internal virtual void BeforeStringWrite(int index, long streamPosition, int recordPosition) {
		}
	}
	#endregion
	#region XlsSharedStringsExporter
	public class XlsSharedStringsExporter : XlsSharedStringsExporterBase {
		int stringsInBucket;
		List<XlsSSTInfo> extendedSSTItems = new List<XlsSSTInfo>();
		public XlsSharedStringsExporter(BinaryWriter streamWriter, DocumentModel documentModel, ExportXlsStyleSheet exportStyleSheet)
			: base(streamWriter, documentModel, exportStyleSheet) {
		}
		public int StringsInBucket { get { return stringsInBucket; } }
		public IList<XlsSSTInfo> ExtendedSSTItems { get { return extendedSSTItems; } }
		public override void WriteContent() {
			this.stringsInBucket = (GetSharedStringsCount() / 128) + 1;
			if(this.stringsInBucket < XlsDefs.MinStringsInBucket)
				this.stringsInBucket = XlsDefs.MinStringsInBucket;
			this.extendedSSTItems.Clear();
			base.WriteContent();
		}
		protected override int GetSharedStringsTotalRefCount() {
			return ExportStyleSheet.SharedStringsTotalRefCount;
		}
		protected override int GetSharedStringsCount() {
			return ExportStyleSheet.SharedStringsIndicies.Count;
		}
		protected override XLUnicodeRichExtendedString GetSharedStringsItem(int index) {
			XLUnicodeRichExtendedString result = new XLUnicodeRichExtendedString();
			int sstIndex = ExportStyleSheet.SharedStringsIndicies[index];
			ISharedStringItem item = DocumentModel.SharedStringTable[sstIndex];
			PlainTextStringItem plainString = item as PlainTextStringItem;
			if(plainString != null) {
				result.Value = plainString.Content;
			}
			else {
				FormattedStringItem formattedString = item as FormattedStringItem;
				if(formattedString == null)
					throw new Exception("Unknown shared strings item");
				result.Value = formattedString.GetPlainText();
				int charIndex = 0;
				foreach(FormattedStringItemPart part in formattedString.Items) {
					XlsFormatRun formatRun = new XlsFormatRun();
					formatRun.CharIndex = charIndex;
					int fontIndex = DocumentModel.Cache.FontInfoCache.GetItemIndex(part.Font);
					int runFontIndex = ExportStyleSheet.GetFontIndex(fontIndex);
					if(runFontIndex >= XlsDefs.UnusedFontIndex)
						runFontIndex++;
					formatRun.FontIndex = runFontIndex;
					if(!formatRun.IsDefault())
						result.FormatRuns.Add(formatRun);
					charIndex += part.Content.Length;
				}
			}
			return result;
		}
		protected internal override void BeforeStringWrite(int index, long streamPosition, int recordPosition) {
			if(FirstStringInBucket(index)) {
				XlsSSTInfo item = new XlsSSTInfo();
				item.StreamPosition = (int)(streamPosition + recordPosition);
				item.Offset = recordPosition;
				this.extendedSSTItems.Add(item);
			}
		}
		bool FirstStringInBucket(int index) {
			return (index % this.stringsInBucket) == 0;
		}
	}
	#endregion
}
