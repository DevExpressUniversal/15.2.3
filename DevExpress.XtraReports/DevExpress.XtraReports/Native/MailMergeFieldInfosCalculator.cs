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

using DevExpress.Data;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting.Native.RichText;
using DevExpress.XtraRichEdit.Model;
using System.Collections;
namespace DevExpress.XtraReports.Native {
	public abstract class MailMergeFieldInfosCalculator {
		public MailMergeFieldInfoCollection CalcMailMergeFieldInfos(string source) {
			if(string.IsNullOrEmpty(source) || source.IndexOf(MailMergeFieldInfo.OpeningBracket) < 0 || source.IndexOf(MailMergeFieldInfo.ClosingBracket) < 0)
				return new MailMergeFieldInfoCollection();
			return CalcMailMergeFieldInfosCore(source);
		}
		public abstract string PrepareString(string source);
		protected abstract MailMergeFieldInfoCollection CalcMailMergeFieldInfosCore(string source);
	}
	public class PlainTextMailMergeFieldInfosCalculator : MailMergeFieldInfosCalculator {
		public static readonly PlainTextMailMergeFieldInfosCalculator Instance = new PlainTextMailMergeFieldInfosCalculator();
		#region Methods
		public override string PrepareString(string source) {
			return source;
		}
		protected override MailMergeFieldInfoCollection CalcMailMergeFieldInfosCore(string controlText) {
			MailMergeFieldInfoCollection result = new MailMergeFieldInfoCollection();
			int[] openingBracketIndexes = GetAllCharIndexes(controlText, MailMergeFieldInfo.OpeningBracket);
			int[] closingBracketIndexes = GetAllCharIndexes(controlText, MailMergeFieldInfo.ClosingBracket);
			int openingBracketIndex = 0;
			int closingBracketIndex = 0;
			MailMergeFieldInfo mailMergeFieldInfo;
			while(openingBracketIndex < openingBracketIndexes.Length && closingBracketIndex < closingBracketIndexes.Length) {
				closingBracketIndex = GetMinIndexMoreVal(closingBracketIndexes, closingBracketIndex,
					openingBracketIndexes[openingBracketIndex]);
				if(closingBracketIndex < 0)
					break;
				openingBracketIndex = GetMaxIndexLessVal(openingBracketIndexes, openingBracketIndex,
					closingBracketIndexes[closingBracketIndex]);
				if(openingBracketIndex < 0)
					break;
				mailMergeFieldInfo = new MailMergeFieldInfo();
				mailMergeFieldInfo.StartPosition = openingBracketIndexes[openingBracketIndex];
				mailMergeFieldInfo.EndPosition = closingBracketIndexes[closingBracketIndex];
				mailMergeFieldInfo.FieldName = controlText.Substring(mailMergeFieldInfo.StartPosition + 1, mailMergeFieldInfo.EndPosition - mailMergeFieldInfo.StartPosition - 1);
				result.Add(mailMergeFieldInfo);
				openingBracketIndex++;
				closingBracketIndex++;
			}
			return result;
		}
		static int[] GetAllCharIndexes(string source, char ch) {
			ArrayList result = new ArrayList();
			int index = source.IndexOf(ch, 0);
			while(index >= 0) {
				result.Add(index);
				index = source.IndexOf(ch, index + 1);
			}
			return (int[])result.ToArray(typeof(int));
		}
		static int GetMinIndexMoreVal(int[] source, int startIndex, int val) {
			for(int i = startIndex; i < source.Length; i++) {
				if(source[i] > val) {
					return i;
				}
			}
			return -1;
		}
		static int GetMaxIndexLessVal(int[] source, int startIndex, int val) {
			for(int i = source.Length - 1; i >= startIndex; i--) {
				if(source[i] < val) {
					return i;
				}
			}
			return -1;
		}
		#endregion
	}
	public class RichTextMailMergeFieldInfosCalculator : MailMergeFieldInfosCalculator {
		public static readonly RichTextMailMergeFieldInfosCalculator Instance = new RichTextMailMergeFieldInfosCalculator();
		public override string PrepareString(string source) {
			string rtfString = (string)source;
			RtfExportProvider.MakeStringUnicodeCompatible(ref rtfString);
			return rtfString;
		}
		protected override MailMergeFieldInfoCollection CalcMailMergeFieldInfosCore(string controlText) {
			using(MailMergeDocumentManager docManager = new MailMergeDocumentManager()) {
				XtraRichTextEditHelper.ImportRtfTextToMailMergeDocManager(controlText, docManager);
				return docManager.FieldInfoCollection;
			}
		}
	}
}
