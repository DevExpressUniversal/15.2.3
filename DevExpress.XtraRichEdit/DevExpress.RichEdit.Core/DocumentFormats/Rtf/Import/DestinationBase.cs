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
using System.Text;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region DestinationBase
	public delegate void TranslateControlCharHandler(RtfImporter importer, char ch);
	public delegate void TranslateKeywordHandler(RtfImporter importer, int parameterValue, bool hasParameter);
	public abstract class DestinationBase : IDisposable {
		#region EmptyDestination
		class EmptyDestination : DestinationBase {
			protected override DestinationBase CreateClone() {
				return new EmptyDestination();
			}
			protected override ControlCharTranslatorTable ControlCharHT {
				get { return new ControlCharTranslatorTable(); }
			}
			protected override KeywordTranslatorTable KeywordHT {
				get { return new KeywordTranslatorTable(); }
			}
		}
		#endregion
		static readonly DestinationBase empty = new EmptyDestination();
		public static DestinationBase Empty { get { return empty; } }
		#region CreateControlCharTable & CreateKeywordTable
		static ControlCharTranslatorTable defaultControlCharHT = CreateControlCharTable();
		static KeywordTranslatorTable defaultKeywordHT = CreateKeywordTable();
		const int macCodePage = 10000;
		const int pcCodePage = 437;
		const int pcaCodePage = 850;
		static ControlCharTranslatorTable CreateControlCharTable() {
			ControlCharTranslatorTable table = new ControlCharTranslatorTable();
			table.Add('\'', OnSwitchToHexChar);
			table.Add('*', OnOptionalGroupChar);
			return table;
		}
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("bin", OnBinKeyword);
			table.Add("colortbl", OnColorTableKeyword);
			table.Add("fonttbl", OnFontTableKeyword);
			table.Add("protusertbl", OnUserTableKeyword);
			table.Add("ansi", OnAnsiKeyword);
			table.Add("mac", OnMacKeyword);
			table.Add("pc", OnPcKeyword);
			table.Add("pca", OnPcaKeyword);
			table.Add("ansicpg", OnAnsiCodePageKeyword);
			table.Add("hyphauto", OnHyphAutoKeyword);
			return table;
		}
		#endregion
		#region processing control char and keyword
		protected static void OnSwitchToHexChar(RtfImporter importer, char ch) {
			importer.StateManager.ParsingState = RtfParsingState.HexData;
		}
		protected static void OnEscapedChar(RtfImporter importer, char ch) {
			importer.FlushDecoder();
			importer.Destination.ProcessChar(ch);
		}
		protected static void OnUnicodeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			char ch = (char)(parameterValue & 0xFFFF);
			if ((int)ch >= 0xF020 && (int)ch <= 0xF0FF)
				ch = (char)((int)ch - 0xF000);
			importer.ParseUnicodeChar(ch);
		}
		static void OnOptionalGroupChar(RtfImporter importer, char ch) {
			if (importer.Destination.NonEmpty)
				return;
			importer.FlushDecoder();
			importer.OptionalGroupLevel = importer.StateManager.SavedStates.Count;
		}
		internal static void OnBinKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter && parameterValue != 0) {
				importer.BinCharCount = parameterValue;
				importer.StateManager.ParsingState = RtfParsingState.BinData;
			}
			else {
				importer.DecreaseSkipCount();
			}
		}
		static void OnColorTableKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new ColorTableDestination(importer);
		}
		static void OnFontTableKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new FontTableDestination(importer);
		}
		static void OnUserTableKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new UserTableDestination(importer);
		}
		static void OnAnsiKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.SetCodePage(DXEncoding.GetEncodingCodePage(RtfImporter.DefaultEncoding));
		}
		static void OnMacKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultCodePage = macCodePage;
			importer.SetCodePage(macCodePage);
		}
		static void OnPcKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultCodePage = pcCodePage;
			importer.SetCodePage(pcCodePage);
		}
		static void OnPcaKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultCodePage = pcaCodePage;
			importer.SetCodePage(pcaCodePage);
		}
		static void OnAnsiCodePageKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter) {
				importer.DocumentProperties.DefaultCodePage = parameterValue;
				importer.SetCodePage(parameterValue);
			}
		}
		static void OnHyphAutoKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 1;
			importer.DocumentModel.DocumentProperties.HyphenateDocument = parameterValue != 0;
		}
		#endregion
		#region Fields
		readonly RtfImporter importer;
		readonly PieceTable pieceTable;
		bool nonEmpty;
		#endregion
		DestinationBase() {
		}
		protected DestinationBase(RtfImporter importer) {
			this.importer = importer;
			this.pieceTable = importer.PieceTable;
			RtfFormattingInfo rtfFormattingInfo = CreateRtfFormattingInfo();
			rtfFormattingInfo.CopyFrom(importer.Position.RtfFormattingInfo);
			Importer.Position.RtfFormattingInfo = rtfFormattingInfo;
		}
		#region Properties
		protected RtfImporter Importer { get { return importer; } }
		protected internal virtual PieceTable PieceTable { get { return pieceTable; } }
		protected internal virtual bool CanAppendText { get { return false; } }
		public bool NonEmpty { get { return nonEmpty; } }
		#endregion
		protected internal virtual RtfFormattingInfo CreateRtfFormattingInfo() {
			return new RtfFormattingInfo();
		}
		public void ProcessControlChar(char ch) {
			ProcessControlCharCore(ch);
			nonEmpty = true;
		}
		public bool ProcessKeyword(string keyword, int parameterValue, bool hasParameter) {
			bool result = ProcessKeywordCore(keyword, parameterValue, hasParameter);
			nonEmpty = true;
			return result;
		}
		public void ProcessChar(char ch) {
			ProcessCharCore(ch);
			nonEmpty = true;
		}
		public void ProcessText(string text) {
			ProcessTextCore(text);
			nonEmpty = true;
		}
		public void ProcessBinChar(char ch) {
			ProcessBinCharCore(ch);
			nonEmpty = true;
		}
		protected virtual void ProcessControlCharCore(char ch) {
			TranslateControlCharHandler translator = null;
			if (ControlCharHT != null)
				ControlCharHT.TryGetValue(ch, out translator);
			if (translator == null)
				defaultControlCharHT.TryGetValue(ch, out translator);
			if (translator != null)
				translator(Importer, ch);
		}
		protected virtual bool ProcessKeywordCore(string keyword, int parameterValue, bool hasParameter) {
			TranslateKeywordHandler translator = null;
			if (KeywordHT != null)
				KeywordHT.TryGetValue(keyword, out translator);
			if (translator == null)
				defaultKeywordHT.TryGetValue(keyword, out translator);
			if (translator != null) {
				translator(Importer, parameterValue, hasParameter);
				return true;
			}
			return false;
		}
		protected virtual void ProcessCharCore(char ch) {
		}
		protected virtual void ProcessTextCore(string text) {
			Debug.Assert(CanAppendText);
		}
		protected virtual void ProcessBinCharCore(char ch) {
		}
		public virtual void BeforePopRtfState() {
		}
		public virtual void AfterPopRtfState() {
		}
		public virtual void IncreaseGroupLevel() {
		}
		public void BeforeNestedGroupFinished(DestinationBase nestedDestination) {
			BeforeNestedGroupFinishedCore(nestedDestination);
			nonEmpty = true;
		}
		protected virtual void BeforeNestedGroupFinishedCore(DestinationBase nestedDestination) {
		}
		public virtual void NestedGroupFinished(DestinationBase nestedDestination) {
		}
		public virtual void AfterNestedGroupFinished(DestinationBase nestedDestination) {
		}
		protected abstract DestinationBase CreateClone();
		public virtual DestinationBase Clone() {
			DestinationBase clone = CreateClone();
			return clone;
		}
		protected abstract ControlCharTranslatorTable ControlCharHT { get; }
		protected abstract KeywordTranslatorTable KeywordHT { get; }
		public bool Equals(DestinationBase destination) {
			return destination.GetType() == this.GetType();
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
}
