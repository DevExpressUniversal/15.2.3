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
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region FontTableDestination
	public class FontTableDestination : DestinationBase {
		RtfFontInfo fontInfo;
		readonly bool nestedState;
		bool emptyFontInfo;
		public FontTableDestination(RtfImporter rtfImporter) : this(rtfImporter, false) {
		}
		public FontTableDestination(RtfImporter rtfImporter, bool nestedState)
			: base(rtfImporter) {
			this.nestedState = nestedState;
			fontInfo = new RtfFontInfo();
			emptyFontInfo = true;
		}
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return null; } }
		protected bool NestedState { get { return nestedState; } }
		protected override bool ProcessKeywordCore(string keyword, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 0;
			emptyFontInfo = false;
			switch (keyword) {
				case "f":
					fontInfo.Id = parameterValue;
					break;
				case "fcharset":
					OnFontCharset(parameterValue);
					break;
				case "bin":
					return base.ProcessKeywordCore(keyword, parameterValue, hasParameter);
				default:
					return false;
			}
			return true;
		}
		protected override DestinationBase CreateClone() {
			return new FontTableDestination(Importer, true);
		}		
		void AddFontInfo() {
			if (nestedState && emptyFontInfo)
				return;
			fontInfo.Name = StringHelper.RemoveSpecialSymbols(fontInfo.Name);
			if (fontInfo.Name.Length == 0)
				fontInfo.Name = Importer.DocumentModel.Cache.CharacterFormattingInfoCache.DefaultItem.FontName;
			Importer.DocumentProperties.Fonts.Add(fontInfo);
			fontInfo = new RtfFontInfo();
			emptyFontInfo = true;
		}
		protected override void ProcessCharCore(char ch) {
			if (ch == ';') {
				AddFontInfo();
				Importer.SetCodePage(Importer.DocumentProperties.DefaultCodePage);
			}
			else {
				fontInfo.Name += ch;
				emptyFontInfo = false;
			}
		}
		protected internal void OnFontCharset(int parameterValue) {
			fontInfo.Charset = parameterValue;
			if (fontInfo.Charset >= 0)
				Importer.SetCodePage(DXEncoding.CodePageFromCharset(fontInfo.Charset));
		}
		public override void BeforePopRtfState() {
			if (nestedState && !emptyFontInfo)
				AddFontInfo();
			base.BeforePopRtfState();
		}
		public override void AfterPopRtfState() {
			RtfDocumentProperties props = Importer.DocumentProperties;
			RtfFontInfo fontInfo = props.Fonts.GetRtfFontInfoById(props.DefaultFontNumber);
			Importer.Position.CharacterFormatting.FontName = fontInfo.Name;
			if (fontInfo != props.Fonts.defaultRtfFontInfo && fontInfo.Charset >= 0) {
				Importer.DocumentProperties.DefaultCodePage = DXEncoding.CodePageFromCharset(fontInfo.Charset);
			}
			Importer.SetCodePage(Importer.DocumentProperties.DefaultCodePage);
		}
	}
	#endregion
}
