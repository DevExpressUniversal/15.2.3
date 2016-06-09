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
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.Office.History;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model.History;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	public enum StyleType {
		ParagraphStyle,
		CharacterStyle,
		TableStyle,
		NumberingListStyle,
		TableCellStyle
	}
	public interface IStyle {
		string StyleName { get; }
		StyleType Type { get; }
		bool Deleted { get; }
		bool Hidden { get; }
		bool Semihidden { get; }
		void ResetCachedIndices(ResetFormattingCacheType resetFormattingCacheType);
	}
	#region KnownStyleNames
	public static class KnownStyleNames {
		public static readonly string AnnotationText = "annotation text";
		public static readonly string AnnotationReference = "annotation reference";
		public static readonly string AnnotationSubject = "annotation subject";
		public static readonly string ArticleSection = "Article / Section";
		public static readonly string BalloonText = "Balloon Text";
		public static readonly string BlockText = "Block Text";
		public static readonly string BodyText = "Body Text";
		public static readonly string BodyText2 = "Body Text 2";
		public static readonly string BodyText3 = "Body Text 3";
		public static readonly string BodyTextFirstIndent = "Body Text First Indent";
		public static readonly string BodyTextFirstIndent2 = "Body Text First Indent 2";
		public static readonly string BodyTextIndent = "Body Text Indent";
		public static readonly string BodyTextIndent2 = "Body Text Indent 2";
		public static readonly string BodyTextIndent3 = "Body Text Indent 3";
		public static readonly string Caption = "Caption";
		public static readonly string Closing = "Closing";
		public static readonly string CommentReference = "Comment Reference";
		public static readonly string CommentSubject = "Comment Subject";
		public static readonly string CommentText = "Comment Text";
		public static readonly string Date = "Date";
		public static readonly string DocumentMap = "Document Map";
		public static readonly string EmailSignature = "E-mail Signature";
		public static readonly string EndnoteReference = "Endnote Reference";
		public static readonly string EndnoteText = "Endnote Text";
		public static readonly string EnvelopeAddress = "Envelope Address";
		public static readonly string EnvelopeReturn = "Envelope Return";
		public static readonly string Footer = "Footer";
		public static readonly string FootnoteText = "Footnote Text";
		public static readonly string FootnoteReference = "Footnote Reference";
		public static readonly string Header = "Header";
		public static readonly string Heading1 = "Heading 1";
		public static readonly string Heading2 = "Heading 2";
		public static readonly string Heading3 = "Heading 3";
		public static readonly string Heading4 = "Heading 4";
		public static readonly string Heading5 = "Heading 5";
		public static readonly string Heading6 = "Heading 6";
		public static readonly string Heading7 = "Heading 7";
		public static readonly string Heading8 = "Heading 8";
		public static readonly string Heading9 = "Heading 9";
		public static readonly string HTMLAcronym = "HTML Acronym";
		public static readonly string HTMLAddress = "HTML Address";
		public static readonly string HTMLBottomOfForm = "HTML Bottom of Form";
		public static readonly string HTMLCite = "HTML Cite";
		public static readonly string HTMLCode = "HTML Code";
		public static readonly string HTMLDefinition = "HTML Definition";
		public static readonly string HTMLKeyboard = "HTML Keyboard";
		public static readonly string HTMLPreformatted = "HTML Preformatted";
		public static readonly string HTMLSample = "HTML Sample";
		public static readonly string HTMLTopOfForm = "HTML Top of Form";
		public static readonly string HTMLTypewriter = "HTML Typewriter";
		public static readonly string HTMLVariable = "HTML Variable";
		public static readonly string HyperlinkFollowed = "HyperlinkFollowed";
		public static readonly string HyperlinkStrongEmphasis = "HyperlinkStrongEmphasis";
		public static readonly string Emphasis = "Emphasis";
		public static readonly string FollowedHyperlink = "FollowedHyperlink";
		public static readonly string Index1 = "Index 1";
		public static readonly string Index2 = "Index 2";
		public static readonly string Index3 = "Index 3";
		public static readonly string Index4 = "Index 4";
		public static readonly string Index5 = "Index 5";
		public static readonly string Index6 = "Index 6";
		public static readonly string Index7 = "Index 7";
		public static readonly string Index8 = "Index 8";
		public static readonly string Index9 = "Index 9";
		public static readonly string IndexHeading = "Index Heading";
		public static readonly string List = "List";
		public static readonly string List2 = "List 2";
		public static readonly string List3 = "List 3";
		public static readonly string List4 = "List 4";
		public static readonly string List5 = "List 5";
		public static readonly string ListBullet = "List Bullet";
		public static readonly string ListBullet2 = "List Bullet 2";
		public static readonly string ListBullet3 = "List Bullet 3";
		public static readonly string ListBullet4 = "List Bullet 4";
		public static readonly string ListBullet5 = "List Bullet 5";
		public static readonly string ListContinue = "List Continue";
		public static readonly string ListContinue2 = "List Continue 2";
		public static readonly string ListContinue3 = "List Continue 3";
		public static readonly string ListContinue4 = "List Continue 4";
		public static readonly string ListContinue5 = "List Continue 5";
		public static readonly string ListNumber = "List Number";
		public static readonly string ListNumber2 = "List Number 2";
		public static readonly string ListNumber3 = "List Number 3";
		public static readonly string ListNumber4 = "List Number 4";
		public static readonly string ListNumber5 = "List Number 5";
		public static readonly string MacroText = "Macro Text";
		public static readonly string MacroToAHeading = "macro to a heading";
		public static readonly string MessageHeader = "Message Header";
		public static readonly string NoList = "No List";
		public static readonly string NormalIndent = "Normal Indent";
		public static readonly string NormalWeb = "Normal (Web)";
		public static readonly string NoteHeading = "Note Heading";
		public static readonly string NormalTable = "Normal Table";
		public static readonly string OutlineList1 = "Outline List 1";
		public static readonly string OutlineList2 = "Outline List 2";
		public static readonly string OutlineList3 = "Outline List 3";
		public static readonly string PageNumber = "Page Number";
		public static readonly string PlainText = "Plain Text";
		public static readonly string Salutation = "Salutation";
		public static readonly string Signature = "Signature";
		public static readonly string Strong = "Strong";
		public static readonly string Subtitle = "Subtitle";
		public static readonly string TableOfAuthorities = "Table of Authorities";
		public static readonly string TableOfFigures = "Table of Figures";
		public static readonly string Table3DEffects1 = "Table 3D effects 1";
		public static readonly string Table3DEffects2 = "Table 3D effects 2";
		public static readonly string Table3DEffects3 = "Table 3D effects 3";
		public static readonly string TableClassic1 = "Table Classic 1";
		public static readonly string TableClassic2 = "Table Classic 2";
		public static readonly string TableClassic3 = "Table Classic 3";
		public static readonly string TableClassic4 = "Table Classic 4";
		public static readonly string TableColorful1 = "Table Colorful 1";
		public static readonly string TableColorful2 ="Table Colorful 2";
		public static readonly string TableColorful3 ="Table Colorful 3";
		public static readonly string TableColumns1 = "Table Columns 1";
		public static readonly string TableColumns2 = "Table Columns 2";
		public static readonly string TableColumns3 = "Table Columns 3";
		public static readonly string TableColumns4 = "Table Columns 4";
		public static readonly string TableColumns5 = "Table Columns 5";
		public static readonly string TableContemporary = "Table Contemporary";
		public static readonly string TableElegant = "Table Elegant";
		public static readonly string TableGrid = "Table Grid";
		public static readonly string TableGrid1 = "Table Grid 1";
		public static readonly string TableGrid2 = "Table Grid 2";
		public static readonly string TableGrid3 = "Table Grid 3";
		public static readonly string TableGrid4 = "Table Grid 4";
		public static readonly string TableGrid5 = "Table Grid 5";
		public static readonly string TableGrid6 = "Table Grid 6";
		public static readonly string TableGrid7 = "Table Grid 7";
		public static readonly string TableGrid8 = "Table Grid 8";
		public static readonly string TableList1 = "Table List 1";
		public static readonly string TableList2 = "Table List 2";
		public static readonly string TableList3 = "Table List 3";
		public static readonly string TableList4 = "Table List 4";
		public static readonly string TableList5 = "Table List 5";
		public static readonly string TableList6 = "Table List 6";
		public static readonly string TableList7 = "Table List 7";
		public static readonly string TableList8 = "Table List 8";
		public static readonly string TableNormal = "Table Normal";
		public static readonly string TableProfessional = "Table Professional";
		public static readonly string TableSimple1 = "Table Simple 1";
		public static readonly string TableSimple2 = "Table Simple 2";
		public static readonly string TableSimple3 = "Table Simple 3";
		public static readonly string TableSubtle1 = "Table Subtle 1";
		public static readonly string TableSubtle2 = "Table Subtle 2";
		public static readonly string TableTheme = "Table Theme";
		public static readonly string TableWeb1 = "Table Web 1";
		public static readonly string TableWeb2 = "Table Web 2";
		public static readonly string TableWeb3 = "Table Web 3";
		public static readonly string Title = "Title";
		public static readonly string TOAHeading = "TOA Heading";
		public static readonly string TOC1 = "TOC 1";
		public static readonly string TOC2 = "TOC 2";
		public static readonly string TOC3 = "TOC 3";
		public static readonly string TOC4 = "TOC 4";
		public static readonly string TOC5 = "TOC 5";
		public static readonly string TOC6 = "TOC 6";
		public static readonly string TOC7 = "TOC 7";
		public static readonly string TOC8 = "TOC 8";
		public static readonly string TOC9 = "TOC 9";
		static Dictionary<string, XtraRichEditStringId> localizedStyleNames = CreateLocalizedStyleNamesDictionary();
		static Dictionary<string, XtraRichEditStringId> CreateLocalizedStyleNamesDictionary() {
			Dictionary<string, XtraRichEditStringId> result = new Dictionary<string, XtraRichEditStringId>();
			result.Add(ParagraphStyleCollection.DefaultParagraphStyleName, XtraRichEditStringId.LocalizedStyleName_Normal);
			result.Add(CharacterStyleCollection.HyperlinkStyleName, XtraRichEditStringId.LocalizedStyleName_Hyperlink);
			result.Add(CharacterStyleCollection.DefaultCharacterStyleName, XtraRichEditStringId.DefaultStyleName_DefaultParagraphFont);
			result.Add(CharacterStyleCollection.LineNumberingStyleName, XtraRichEditStringId.DefaultStyleName_linenumber);
			result.Add(ArticleSection, XtraRichEditStringId.DefaultStyleName_ArticleSection);
			result.Add(AnnotationText, XtraRichEditStringId.DefaultStyleName_annotationtext);
			result.Add(AnnotationReference, XtraRichEditStringId.DefaultStyleName_annotationreference);
			result.Add(AnnotationSubject, XtraRichEditStringId.DefaultStyleName_annotationsubject);
			result.Add(BalloonText, XtraRichEditStringId.DefaultStyleName_BalloonText);
			result.Add(BlockText, XtraRichEditStringId.DefaultStyleName_BlockText);
			result.Add(BodyText, XtraRichEditStringId.DefaultStyleName_BodyText);
			result.Add(BodyText2, XtraRichEditStringId.DefaultStyleName_BodyText2);
			result.Add(BodyText3, XtraRichEditStringId.DefaultStyleName_BodyText3);
			result.Add(BodyTextFirstIndent, XtraRichEditStringId.DefaultStyleName_BodyTextFirstIndent);
			result.Add(BodyTextFirstIndent2, XtraRichEditStringId.DefaultStyleName_BodyTextFirstIndent2);
			result.Add(BodyTextIndent, XtraRichEditStringId.DefaultStyleName_BodyTextIndent);
			result.Add(BodyTextIndent2, XtraRichEditStringId.DefaultStyleName_BodyTextIndent2);
			result.Add(BodyTextIndent3, XtraRichEditStringId.DefaultStyleName_BodyTextIndent3);
			result.Add(Caption, XtraRichEditStringId.DefaultStyleName_caption);
			result.Add(Closing, XtraRichEditStringId.DefaultStyleName_Closing);
			result.Add(CommentReference, XtraRichEditStringId.DefaultStyleName_CommentReference);
			result.Add(CommentSubject, XtraRichEditStringId.DefaultStyleName_CommentSubject);
			result.Add(CommentText, XtraRichEditStringId.DefaultStyleName_CommentText);
			result.Add(Date, XtraRichEditStringId.DefaultStyleName_Date);
			result.Add(DocumentMap, XtraRichEditStringId.DefaultStyleName_DocumentMap);
			result.Add(EmailSignature, XtraRichEditStringId.DefaultStyleName_EmailSignature);
			result.Add(EndnoteReference, XtraRichEditStringId.DefaultStyleName_endnotereference);
			result.Add(EndnoteText, XtraRichEditStringId.DefaultStyleName_endnotetext);
			result.Add(EnvelopeAddress, XtraRichEditStringId.DefaultStyleName_envelopeaddress);
			result.Add(EnvelopeReturn, XtraRichEditStringId.DefaultStyleName_envelopereturn);
			result.Add(Footer, XtraRichEditStringId.DefaultStyleName_footer);
			result.Add(FootnoteText, XtraRichEditStringId.DefaultStyleName_footnotetext);
			result.Add(FootnoteReference, XtraRichEditStringId.DefaultStyleName_footnotereference);
			result.Add(Header, XtraRichEditStringId.DefaultStyleName_header);
			result.Add(Heading1, XtraRichEditStringId.DefaultStyleName_heading1);
			result.Add(Heading2, XtraRichEditStringId.DefaultStyleName_heading2);
			result.Add(Heading3, XtraRichEditStringId.DefaultStyleName_heading3);
			result.Add(Heading4, XtraRichEditStringId.DefaultStyleName_heading4);
			result.Add(Heading5, XtraRichEditStringId.DefaultStyleName_heading5);
			result.Add(Heading6, XtraRichEditStringId.DefaultStyleName_heading6);
			result.Add(Heading7, XtraRichEditStringId.DefaultStyleName_heading7);
			result.Add(Heading8, XtraRichEditStringId.DefaultStyleName_heading8);
			result.Add(Heading9, XtraRichEditStringId.DefaultStyleName_heading9);
			result.Add(HTMLAcronym, XtraRichEditStringId.DefaultStyleName_HTMLAcronym);
			result.Add(HTMLAddress, XtraRichEditStringId.DefaultStyleName_HTMLAddress);
			result.Add(HTMLBottomOfForm, XtraRichEditStringId.DefaultStyleName_HTMLBottomofForm);
			result.Add(HTMLCite, XtraRichEditStringId.DefaultStyleName_HTMLCite);
			result.Add(HTMLCode, XtraRichEditStringId.DefaultStyleName_HTMLCode);
			result.Add(HTMLDefinition, XtraRichEditStringId.DefaultStyleName_HTMLDefinition);
			result.Add(HTMLKeyboard, XtraRichEditStringId.DefaultStyleName_HTMLKeyboard);
			result.Add(HTMLPreformatted, XtraRichEditStringId.DefaultStyleName_HTMLPreformatted);
			result.Add(HTMLSample, XtraRichEditStringId.DefaultStyleName_HTMLSample);
			result.Add(HTMLTopOfForm, XtraRichEditStringId.DefaultStyleName_HTMLTopofForm);
			result.Add(HTMLTypewriter, XtraRichEditStringId.DefaultStyleName_HTMLTypewriter);
			result.Add(HTMLVariable, XtraRichEditStringId.DefaultStyleName_HTMLVariable);
			result.Add(HyperlinkFollowed, XtraRichEditStringId.DefaultStyleName_HyperlinkFollowed);
			result.Add(HyperlinkStrongEmphasis, XtraRichEditStringId.DefaultStyleName_HyperlinkStrongEmphasis);
			result.Add(Emphasis, XtraRichEditStringId.DefaultStyleName_Emphasis);
			result.Add(FollowedHyperlink, XtraRichEditStringId.DefaultStyleName_FollowedHyperlink);
			result.Add(Index1, XtraRichEditStringId.DefaultStyleName_index1);
			result.Add(Index2, XtraRichEditStringId.DefaultStyleName_index2);
			result.Add(Index3, XtraRichEditStringId.DefaultStyleName_index3);
			result.Add(Index4, XtraRichEditStringId.DefaultStyleName_index4);
			result.Add(Index5, XtraRichEditStringId.DefaultStyleName_index5);
			result.Add(Index6, XtraRichEditStringId.DefaultStyleName_index6);
			result.Add(Index7, XtraRichEditStringId.DefaultStyleName_index7);
			result.Add(Index8, XtraRichEditStringId.DefaultStyleName_index8);
			result.Add(Index9, XtraRichEditStringId.DefaultStyleName_index9);
			result.Add(IndexHeading, XtraRichEditStringId.DefaultStyleName_indexheading);
			result.Add(List, XtraRichEditStringId.DefaultStyleName_List);
			result.Add(List2, XtraRichEditStringId.DefaultStyleName_List2);
			result.Add(List3, XtraRichEditStringId.DefaultStyleName_List3);
			result.Add(List4, XtraRichEditStringId.DefaultStyleName_List4);
			result.Add(List5, XtraRichEditStringId.DefaultStyleName_List5);
			result.Add(ListBullet, XtraRichEditStringId.DefaultStyleName_ListBullet);
			result.Add(ListBullet2, XtraRichEditStringId.DefaultStyleName_ListBullet2);
			result.Add(ListBullet3, XtraRichEditStringId.DefaultStyleName_ListBullet3);
			result.Add(ListBullet4, XtraRichEditStringId.DefaultStyleName_ListBullet4);
			result.Add(ListBullet5, XtraRichEditStringId.DefaultStyleName_ListBullet5);
			result.Add(ListContinue, XtraRichEditStringId.DefaultStyleName_ListContinue);
			result.Add(ListContinue2, XtraRichEditStringId.DefaultStyleName_ListContinue2);
			result.Add(ListContinue3, XtraRichEditStringId.DefaultStyleName_ListContinue3);
			result.Add(ListContinue4, XtraRichEditStringId.DefaultStyleName_ListContinue4);
			result.Add(ListContinue5, XtraRichEditStringId.DefaultStyleName_ListContinue5);
			result.Add(ListNumber, XtraRichEditStringId.DefaultStyleName_ListNumber);
			result.Add(ListNumber2, XtraRichEditStringId.DefaultStyleName_ListNumber2);
			result.Add(ListNumber3, XtraRichEditStringId.DefaultStyleName_ListNumber3);
			result.Add(ListNumber4, XtraRichEditStringId.DefaultStyleName_ListNumber4);
			result.Add(ListNumber5, XtraRichEditStringId.DefaultStyleName_ListNumber5);
			result.Add(MacroText, XtraRichEditStringId.DefaultStyleName_MacroText);
			result.Add(MacroToAHeading, XtraRichEditStringId.DefaultStyleName_macrotoaheading);
			result.Add(MessageHeader, XtraRichEditStringId.DefaultStyleName_MessageHeader);
			result.Add(NoList, XtraRichEditStringId.DefaultStyleName_NoList);
			result.Add(NormalIndent, XtraRichEditStringId.DefaultStyleName_NormalIndent);
			result.Add(NormalTable, XtraRichEditStringId.DefaultStyleName_NormalTable);
			result.Add(NormalWeb, XtraRichEditStringId.DefaultStyleName_NormalWeb);
			result.Add(NoteHeading, XtraRichEditStringId.DefaultStyleName_NoteHeading);
			result.Add(OutlineList1, XtraRichEditStringId.DefaultStyleName_OutlineList1);
			result.Add(OutlineList2, XtraRichEditStringId.DefaultStyleName_OutlineList2);
			result.Add(OutlineList3, XtraRichEditStringId.DefaultStyleName_OutlineList3);
			result.Add(PageNumber, XtraRichEditStringId.DefaultStyleName_pagenumber);
			result.Add(PlainText, XtraRichEditStringId.DefaultStyleName_PlainText);
			result.Add(Salutation, XtraRichEditStringId.DefaultStyleName_Salutation);
			result.Add(Signature, XtraRichEditStringId.DefaultStyleName_Signature);
			result.Add(Strong, XtraRichEditStringId.DefaultStyleName_Strong);
			result.Add(Subtitle, XtraRichEditStringId.DefaultStyleName_Subtitle);
			result.Add(TableOfAuthorities, XtraRichEditStringId.DefaultStyleName_tableofauthorities);
			result.Add(TableOfFigures, XtraRichEditStringId.DefaultStyleName_tableoffigures);
			result.Add(Table3DEffects1, XtraRichEditStringId.DefaultStyleName_Table3Deffects1);
			result.Add(Table3DEffects2, XtraRichEditStringId.DefaultStyleName_Table3Deffects2);
			result.Add(Table3DEffects3, XtraRichEditStringId.DefaultStyleName_Table3Deffects3);
			result.Add(TableClassic1, XtraRichEditStringId.DefaultStyleName_TableClassic1);
			result.Add(TableClassic2, XtraRichEditStringId.DefaultStyleName_TableClassic2);
			result.Add(TableClassic3, XtraRichEditStringId.DefaultStyleName_TableClassic3);
			result.Add(TableClassic4, XtraRichEditStringId.DefaultStyleName_TableClassic4);
			result.Add(TableColorful1, XtraRichEditStringId.DefaultStyleName_TableColorful1);
			result.Add(TableColorful2, XtraRichEditStringId.DefaultStyleName_TableColorful2);
			result.Add(TableColorful3, XtraRichEditStringId.DefaultStyleName_TableColorful3);
			result.Add(TableColumns1, XtraRichEditStringId.DefaultStyleName_TableColumns1);
			result.Add(TableColumns2, XtraRichEditStringId.DefaultStyleName_TableColumns2);
			result.Add(TableColumns3, XtraRichEditStringId.DefaultStyleName_TableColumns3);
			result.Add(TableColumns4, XtraRichEditStringId.DefaultStyleName_TableColumns4);
			result.Add(TableColumns5, XtraRichEditStringId.DefaultStyleName_TableColumns5);
			result.Add(TableContemporary, XtraRichEditStringId.DefaultStyleName_TableContemporary);
			result.Add(TableElegant, XtraRichEditStringId.DefaultStyleName_TableElegant);
			result.Add(TableGrid, XtraRichEditStringId.DefaultStyleName_TableGrid);
			result.Add(TableGrid1, XtraRichEditStringId.DefaultStyleName_TableGrid1);
			result.Add(TableGrid2, XtraRichEditStringId.DefaultStyleName_TableGrid2);
			result.Add(TableGrid3, XtraRichEditStringId.DefaultStyleName_TableGrid3);
			result.Add(TableGrid4, XtraRichEditStringId.DefaultStyleName_TableGrid4);
			result.Add(TableGrid5, XtraRichEditStringId.DefaultStyleName_TableGrid5);
			result.Add(TableGrid6, XtraRichEditStringId.DefaultStyleName_TableGrid6);
			result.Add(TableGrid7, XtraRichEditStringId.DefaultStyleName_TableGrid7);
			result.Add(TableGrid8, XtraRichEditStringId.DefaultStyleName_TableGrid8);
			result.Add(TableList1, XtraRichEditStringId.DefaultStyleName_TableList1);
			result.Add(TableList2, XtraRichEditStringId.DefaultStyleName_TableList2);
			result.Add(TableList3, XtraRichEditStringId.DefaultStyleName_TableList3);
			result.Add(TableList4, XtraRichEditStringId.DefaultStyleName_TableList4);
			result.Add(TableList5, XtraRichEditStringId.DefaultStyleName_TableList5);
			result.Add(TableList6, XtraRichEditStringId.DefaultStyleName_TableList6);
			result.Add(TableList7, XtraRichEditStringId.DefaultStyleName_TableList7);
			result.Add(TableList8, XtraRichEditStringId.DefaultStyleName_TableList8);
			result.Add(TableWeb1, XtraRichEditStringId.DefaultStyleName_TableWeb1);
			result.Add(TableWeb2, XtraRichEditStringId.DefaultStyleName_TableWeb2);
			result.Add(TableWeb3, XtraRichEditStringId.DefaultStyleName_TableWeb3);
			result.Add(TableNormal, XtraRichEditStringId.DefaultStyleName_TableNormal);
			result.Add(TableProfessional, XtraRichEditStringId.DefaultStyleName_TableProfessional);
			result.Add(TableSimple1, XtraRichEditStringId.DefaultStyleName_TableSimple1);
			result.Add(TableSimple2, XtraRichEditStringId.DefaultStyleName_TableSimple2);
			result.Add(TableSimple3, XtraRichEditStringId.DefaultStyleName_TableSimple3);
			result.Add(TableSubtle1, XtraRichEditStringId.DefaultStyleName_TableSubtle1);
			result.Add(TableSubtle2, XtraRichEditStringId.DefaultStyleName_TableSubtle2);
			result.Add(TableTheme, XtraRichEditStringId.DefaultStyleName_TableTheme);
			result.Add(Title, XtraRichEditStringId.DefaultStyleName_Title);
			result.Add(TOAHeading, XtraRichEditStringId.DefaultStyleName_toaheading);
			result.Add(TOC1, XtraRichEditStringId.DefaultStyleName_toc1);
			result.Add(TOC2, XtraRichEditStringId.DefaultStyleName_toc2);
			result.Add(TOC3, XtraRichEditStringId.DefaultStyleName_toc3);
			result.Add(TOC4, XtraRichEditStringId.DefaultStyleName_toc4);
			result.Add(TOC5, XtraRichEditStringId.DefaultStyleName_toc5);
			result.Add(TOC6, XtraRichEditStringId.DefaultStyleName_toc6);
			result.Add(TOC7, XtraRichEditStringId.DefaultStyleName_toc7);
			result.Add(TOC8, XtraRichEditStringId.DefaultStyleName_toc8);
			result.Add(TOC9, XtraRichEditStringId.DefaultStyleName_toc9);
			return result;
		}
		public static Dictionary<string, XtraRichEditStringId> LocalizedStyleNames { get { return localizedStyleNames; } }
	}
	#endregion
	#region StyleBase<T> (abstract class)
	public abstract class StyleBase<T> : IStyle, IBatchUpdateable, IBatchUpdateHandler where T : StyleBase<T> {
		#region Fields
		BatchUpdateHelper batchUpdateHelper;
		T parentStyle;
		DocumentModel documentModel;
		bool deleted;
		string styleName;
		bool hidden;
		bool semihidden;
		string localizedStyleName;
		bool primary;
		Guid id;
		#endregion
		protected StyleBase(DocumentModel documentModel, T parentStyle, string styleName) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			if (!IsParentValid(parentStyle))
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_InvalidParentStyle);
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.parentStyle = parentStyle;
			this.documentModel = documentModel;
			SetStyleNameCore(styleName);
			this.id = Guid.NewGuid();
		}
		#region Properties
		public bool Deleted { get { return deleted; } }
		public bool Hidden { get { return HiddenCore; } }
		public bool Semihidden { get { return SemihiddenCore; } }
		public bool Primary { get { return primary; } set { primary = value; } }
		protected internal bool DeletedCore { get { return deleted; } set { deleted = value; } }
		protected internal bool HiddenCore { get { return hidden; } set { hidden = value; } }
		protected internal bool SemihiddenCore { get { return semihidden; } set { semihidden = value; } }
		protected internal string LocalizedStyleName { get { return localizedStyleName; } }
		protected internal Guid Id { get { return id; } }
		public DocumentModel DocumentModel { get { return documentModel; } }
		public PieceTable PieceTable { get { return documentModel.MainPieceTable; } }
		#region Parent
		public virtual T Parent {
			get { return parentStyle; }
			set { SetParentStyle(value); }
		}
		#endregion
		#region StyleName
		public string StyleName {
			get { return styleName; }
			set {
				if (styleName == value)
					return;
				ChangeStyleNameHistoryItem<T> item = new ChangeStyleNameHistoryItem<T>(this, styleName, value);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		#endregion
		public abstract StyleType Type { get; }
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			DocumentModel.BeginUpdate();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdateCore();
			DocumentModel.EndUpdate();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastEndUpdateCore();
			DocumentModel.EndUpdate();
		}
		protected internal virtual void OnLastEndUpdateCore() {
			NotifyStyleChangedCore();
		}
		#endregion
		protected void NotifyStyleChanged() {
			if (!IsUpdateLocked)
				NotifyStyleChangedCore();
		}
		protected void NotifyStyleChangedCore() {
			DocumentModel.ResetDocumentFormattingCaches(ResetFormattingCacheType.All);
			DocumentModelChangeActions actions = ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.ParagraphStyle);
			DocumentModel.ApplyChangesCore(PieceTable, actions, RunIndex.Zero, new RunIndex(PieceTable.Runs.Count - 1));
		}
		internal bool IsParentValid(T parent) {
			T currentStyle = parent;
			while (currentStyle != null) {
				if (currentStyle == this)
					return false;
				currentStyle = currentStyle.Parent;
			}
			return true;
		}
		internal void SetParentStyle(T value) {
			if (Parent == value)
				return;
			if (!IsParentValid(value)) {
				if (DocumentModel.DeferredChanges == null || !DocumentModel.DeferredChanges.IsSetContentMode)
					RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_InvalidParentStyle);
				else
					return;
			}
			ChangeParentStyleHistoryItem<T> item = new ChangeParentStyleHistoryItem<T>(this, Parent, value);
			DocumentModel.History.Add(item);
			item.Execute();
		}
		internal void SetParentStyleCore(T newStyle) {
			if (IsUpdateLocked) {
				this.parentStyle = newStyle;
				return;
			}
			DocumentModel.BeginUpdate();
			try {
				this.parentStyle = newStyle;
				NotifyStyleChangedCore();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		internal void SetStyleNameCore(string newStyleName) {
			this.styleName = newStyleName;
			this.localizedStyleName = CalculateLocalizedName(newStyleName);
		}
		internal void SetId(Guid id) {
			this.id = id;
		}
		string CalculateLocalizedName(string styleName) {
			XtraRichEditStringId id;
			if (KnownStyleNames.LocalizedStyleNames.TryGetValue(styleName, out id))
				return XtraRichEditLocalizer.GetString(id);
			return styleName;
		}
		protected internal virtual void OnParentDeleting() {
			if (Parent == null)
				Exceptions.ThrowInternalException();
			BeginUpdate();
			try {
				MergePropertiesWithParent();
				Parent = Parent.Parent;
			}
			finally {
				EndUpdate();
			}
		}
		protected abstract void MergePropertiesWithParent();
		protected internal abstract void CopyProperties(T source);
		public abstract int Copy(DocumentModel targetModel);
		public override string ToString() {
			return LocalizedStyleName;
		}
		public abstract void ResetCachedIndices(ResetFormattingCacheType resetFromattingCacheType);
	}
	#endregion
	public class StyleTopologicalComparer<T> : IComparer<T> where T : StyleBase<T> {
		#region IComparer<T> Members
		public int Compare(T x, T y) {
			if (Object.ReferenceEquals(x.Parent, y))
				return 1;
			if (Object.ReferenceEquals(y.Parent, x))
				return -1;
			return 0;
		}
		#endregion
	}
	public class StyleLinkManager {
		#region Fields
		readonly Dictionary<ParagraphStyle, CharacterStyle> paragraphStyleToCharacterStyleLinks;
		readonly Dictionary<CharacterStyle, ParagraphStyle> characterStyleToParagraphStyleLinks;
		readonly DocumentModel documentModel;
		#endregion
		public StyleLinkManager(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.paragraphStyleToCharacterStyleLinks = new Dictionary<ParagraphStyle, CharacterStyle>();
			this.characterStyleToParagraphStyleLinks = new Dictionary<CharacterStyle, ParagraphStyle>();
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		public ParagraphStyle GetLinkedStyle(CharacterStyle characterStyle) {
			Guard.ArgumentNotNull(characterStyle, "characterStyle");
			ParagraphStyle paragraphStyle;
			if (!characterStyleToParagraphStyleLinks.TryGetValue(characterStyle, out paragraphStyle))
				return null;
			else
				return paragraphStyle;
		}
		public CharacterStyle GetLinkedStyle(ParagraphStyle paragraphStyle) {
			Guard.ArgumentNotNull(paragraphStyle, "paragraphStyle");
			CharacterStyle characterStyle;
			if (!paragraphStyleToCharacterStyleLinks.TryGetValue(paragraphStyle, out characterStyle))
				return null;
			else
				return characterStyle;
		}
		public bool HasLinkedStyle(CharacterStyle characterStyle) {
			return characterStyleToParagraphStyleLinks.ContainsKey(characterStyle);
		}
		public bool HasLinkedStyle(ParagraphStyle paragraphStyle) {
			return paragraphStyleToCharacterStyleLinks.ContainsKey(paragraphStyle);
		}
		public void CreateLink(ParagraphStyle paragraphStyle, CharacterStyle characterStyle) {
			if (paragraphStyle.HasLinkedStyle || characterStyle.HasLinkedStyle)
				return; 
			if (paragraphStyle.Deleted || characterStyle.Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_ErrorLinkDeletedStyle);
			DocumentModel.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
					characterStyle.CharacterProperties.CopyFrom(paragraphStyle.CharacterProperties);
					CreateStyleLinkHistoryItem item = new CreateStyleLinkHistoryItem(paragraphStyle, characterStyle);
					DocumentModel.History.Add(item);
					item.Execute();
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
			DocumentModel.CheckIntegrity();
		}
		public void DeleteLink(ParagraphStyle paragraphStyle) {
			if (!paragraphStyle.HasLinkedStyle)
				Exceptions.ThrowArgumentException("paragraphStyle", paragraphStyle);
		}
		public void DeleteLink(CharacterStyle characterStyle) {
			if (!characterStyle.HasLinkedStyle)
				Exceptions.ThrowArgumentException("characterStyle", characterStyle);
			DeleteLink(characterStyle.LinkedStyle);
		}
		protected internal virtual void CreateLinkCore(ParagraphStyle paragraphStyle, CharacterStyle characterStyle) {
			this.paragraphStyleToCharacterStyleLinks[paragraphStyle] = characterStyle;
			this.characterStyleToParagraphStyleLinks[characterStyle] = paragraphStyle;
		}
		protected internal virtual void DeleteLinkCore(ParagraphStyle paragraphStyle, CharacterStyle characterStyle) {
			this.paragraphStyleToCharacterStyleLinks.Remove(paragraphStyle);
			this.characterStyleToParagraphStyleLinks.Remove(characterStyle);
		}
	}
	#region StyleCollectionBase<T> (abstract class)
	public abstract class StyleCollectionBase<TStyle> : IList<TStyle> where TStyle : StyleBase<TStyle> {
		#region Fields
		readonly List<TStyle> items;
		readonly DocumentModel documentModel;
		#endregion
		protected StyleCollectionBase(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.items = new List<TStyle>();
			this.documentModel = documentModel;
			TStyle defaultItem = CreateDefaultItem();
			if(defaultItem != null)
				Items.Add(defaultItem);
		}
		#region Properties
		public TStyle this[int index] { get { return Items[index]; } }
		public int Count { get { return Items.Count; } }
		public int DefaultItemIndex { get { return 0; } }
		public TStyle DefaultItem { get { return Items[DefaultItemIndex]; } }
		public DocumentModel DocumentModel { get { return documentModel; } }
		protected List<TStyle> Items { get { return items; } }
		#endregion
		#region Events
		EventHandler onCollectionChanged;
		public event EventHandler CollectionChanged {
			add { onCollectionChanged += value; }
			remove { onCollectionChanged -= value; }
		}
		protected internal virtual void RaiseCollectionChanged() {
			if (onCollectionChanged != null)
				onCollectionChanged(this, EventArgs.Empty);
		}
		#endregion
		protected internal abstract TStyle CreateDefaultItem();
		public int Add(TStyle item) {
			Guard.ArgumentNotNull(item, "item");
			if (item.DocumentModel != DocumentModel)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_InvalidDocumentModel);
			DocumentModel.History.BeginTransaction();
			try {
				if (MustAddParent(item))
					Add(item.Parent);
				int styleIndex = Items.IndexOf(item);
				if (styleIndex >= 0) {
					if (item.Deleted)
						AddDeletedStyle(item);
					return styleIndex;
				}
				else
					return AddNewStyle(item);
			}
			finally {
				DocumentModel.History.EndTransaction();
			}
		}
		bool MustAddParent(TStyle style) {
			TStyle parent = style.Parent;
			if (parent == null)
				return false;
			else
				return !Items.Contains(parent) || parent.Deleted;
		}
		protected internal int AddNewStyle(TStyle style) {
			AddStyleHistoryItem<TStyle> historyItem = new AddStyleHistoryItem<TStyle>(this, style);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
			return Items.Count - 1;
		}
		void AddDeletedStyle(TStyle style) {
			AddDeletedStyleHistoryItem<TStyle> historyItem = new AddDeletedStyleHistoryItem<TStyle>(this, style);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public int GetStyleIndexByName(string styleName) {
			return GetStyleIndexByName(styleName, false);
		}
		internal int GetStyleIndexByName(string styleName, bool ignoreCase) {
			StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			int count = this.Count;
			for (int i = 0; i < count; i++) {
				TStyle style = this[i];
				if (String.Equals(style.StyleName, styleName, comparison))
					return i;
			}
			return -1;
		}
		public TStyle GetStyleByName(string styleName) {
			int index = GetStyleIndexByName(styleName);
			return index >= 0 ? this[index] : null;
		}
		internal int GetStyleIndexById(Guid id) {
			int count = this.Count;
			for (int i = 0; i < count; i++) {
				TStyle style = this[i];
				if (style.Id == id)
					return i;
			}
			return -1;
		}
		internal TStyle GetStyleById(Guid id) {
			int index = GetStyleIndexById(id);
			return index >= 0 ? this[index] : null;
		}
		internal void AddCore(TStyle item) {
			Debug.Assert(!Items.Contains(item));
			Items.Add(item);
			RaiseCollectionChanged();
		}
		internal void AddDeletedStyleCore(TStyle item) {
			item.DeletedCore = false;
			RaiseCollectionChanged();
		}
		internal void RemoveLastStyle() {
			Debug.Assert(Items.Count > 0);
			Items.RemoveAt(Items.Count - 1);
			RaiseCollectionChanged();
		}
		public void Delete(TStyle style) {
			Guard.ArgumentNotNull(style, "item");
			int styleIndex = Items.IndexOf(style);
			if (styleIndex < 0)
				return;
			DocumentModel.History.BeginTransaction();
			try {
				DeleteStyleHistoryItem<TStyle> item = new DeleteStyleHistoryItem<TStyle>(this, style);
				DocumentModel.History.Add(item);
				item.Execute();
			}
			finally {
				DocumentModel.History.EndTransaction();
			}
		}
		protected internal virtual void DeleteCore(TStyle style) {
			if (!CanDeleteStyle(style))
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_CantDeleteDefaultStyle);
			NotifyChildrenParentDeleting(style);
			NotifyDocumentStyleDeleting(style);
			style.DeletedCore = true;
			RaiseCollectionChanged();
		}
		protected virtual bool CanDeleteStyle(TStyle style) {
			return style != DefaultItem;
		}
		protected virtual void NotifyDocumentStyleDeleting(TStyle style) {
			List<PieceTable> pieceTables = DocumentModel.GetPieceTables(true);
			int count = pieceTables.Count;
			for(int i = 0; i < count; i++)
				NotifyPieceTableStyleDeleting(pieceTables[i], style);
		}
		protected abstract void NotifyPieceTableStyleDeleting(PieceTable pieceTable, TStyle style);
		public int IndexOf(TStyle item) {
			return Items.IndexOf(item);
		}
		public bool Contains(TStyle item) {
			return Items.Contains(item);
		}
		protected virtual void NotifyChildrenParentDeleting(TStyle parent) {
			int count = Count;
			for (int i = 0; i < count; i++) {
				TStyle style = Items[i];
				if (style.Parent == parent)
					style.OnParentDeleting();
			}
		}
		#region IList<TStyle> Members
		void IList<TStyle>.Insert(int index, TStyle item) {
			Items.Insert(index, item);
		}
		void IList<TStyle>.RemoveAt(int index) {
			Items.RemoveAt(index);
		}
		TStyle IList<TStyle>.this[int index] { get { return Items[index]; } set { Items[index] = value; } }
		#endregion
		#region ICollection<TStyle> Members
		void ICollection<TStyle>.Add(TStyle item) {
			this.Add(item);
		}
		void ICollection<TStyle>.Clear() {
			Items.Clear();
		}
		void ICollection<TStyle>.CopyTo(TStyle[] array, int arrayIndex) {
			Items.CopyTo(array, arrayIndex);
		}
		bool ICollection<TStyle>.IsReadOnly { get { return false; } }
		bool ICollection<TStyle>.Remove(TStyle item) {
			return Items.Remove(item);
		}
		#endregion
		#region IEnumerable<TStyle> Members
		IEnumerator<TStyle> IEnumerable<TStyle>.GetEnumerator() {
			IList<TStyle> itemsList = Items;
			return itemsList.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return Items.GetEnumerator();
		}
		#endregion
		protected internal void ResetCachedIndices(ResetFormattingCacheType resetFromattingCacheType) {
			Items.ForEach(style => ResetItemCachedIndices(style, resetFromattingCacheType));
		}
		void ResetItemCachedIndices(TStyle style, ResetFormattingCacheType resetFromattingCacheType) {
			style.ResetCachedIndices(resetFromattingCacheType);
		}
	}
	#endregion
}
