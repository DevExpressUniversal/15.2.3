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
using System.Text;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandSharedStringsBase
	public abstract class XlsCommandSharedStringsBase : XlsCommandDataCollectorBase {
		#region Fields
		int count;
		int stringsToRead;
		bool readingStrings;
		XLUnicodeRichExtendedString sharedString = new XLUnicodeRichExtendedString();
		#endregion
		protected override void ReadData(XlsReader reader, XlsContentBuilder contentBuilder) {
			if(!this.readingStrings) {
				reader.ReadInt32(); 
				this.stringsToRead = reader.ReadInt32(); 
				this.readingStrings = true;
			}
			while(this.count < this.stringsToRead) {
				if (!ReadSharedString(reader)) return;
				AddSharedString(contentBuilder, this.sharedString);
				count++;
			}
		}
		protected override bool GetCompleted() {
			return this.count == this.stringsToRead;
		}
		bool ReadSharedString(XlsReader reader) {
			if(reader.Position == reader.StreamLength) 
				return false;
			this.sharedString.ReadData(reader);
			return this.sharedString.Complete;
		}
		protected abstract void AddSharedString(XlsContentBuilder builder, XLUnicodeRichExtendedString sharedString);
	}
	#endregion
	#region XlsCommandSharedStrings
	public class XlsCommandSharedStrings : XlsCommandSharedStringsBase {
		protected override void AddSharedString(XlsContentBuilder contentBuilder, XLUnicodeRichExtendedString sharedString) {
			SharedStringTable sharedStringTable = contentBuilder.DocumentModel.SharedStringTable;
			if(sharedString.IsRichString) {
				FormattedStringItem richTextItem = new FormattedStringItem(contentBuilder.DocumentModel);
				ExtractRuns(richTextItem, sharedString, contentBuilder);
				sharedStringTable.Add(richTextItem);
			}
			else {
				PlainTextStringItem plainTextItem = new PlainTextStringItem();
				plainTextItem.Content = sharedString.Value;
				sharedStringTable.Add(plainTextItem);
			}
		}
		void ExtractRuns(FormattedStringItem richTextItem, XLUnicodeRichExtendedString sharedString, XlsContentBuilder contentBuilder) {
			int lastCharIndex = 0;
			int lastFontIndex = 0;
			string str = sharedString.Value;
			int length = str.Length;
			for(int i = 0; i < sharedString.FormatRuns.Count; i++) {
				XlsFormatRun formatRun = sharedString.FormatRuns[i];
				if(formatRun.CharIndex >= length) break; 
				int runLength = formatRun.CharIndex - lastCharIndex;
				if(runLength > 0) {
					AddFormattedStringItemPart(richTextItem, str.Substring(lastCharIndex, runLength),
						contentBuilder.StyleSheet.GetFontInfoIndex(lastFontIndex));
				}
				lastCharIndex = formatRun.CharIndex;
				lastFontIndex = formatRun.FontIndex;
				if(lastFontIndex == XlsDefs.UnusedFontIndex)
					contentBuilder.ThrowInvalidFile("Invalid font index in rich shared string");
				else if(lastFontIndex > XlsDefs.UnusedFontIndex)
					lastFontIndex--;
			}
			if((length - lastCharIndex) > 0) {
				AddFormattedStringItemPart(richTextItem, 
					str.Substring(lastCharIndex, length - lastCharIndex),
					contentBuilder.StyleSheet.GetFontInfoIndex(lastFontIndex));
			}
		}
		void AddFormattedStringItemPart(FormattedStringItem richTextItem, string partContent, int fontIndex) {
			FormattedStringItemPart item = richTextItem.AddNewFormattedStringItemPart();
			item.Content = partContent;
			RunFontInfo newFontInfo = new RunFontInfo();
			newFontInfo.CopyFrom(richTextItem.DocumentModel.Cache.FontInfoCache[fontIndex]);
			item.ReplaceInfo(newFontInfo, DocumentModelChangeActions.None);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandSharedStrings();
		}
	}
	#endregion
}
