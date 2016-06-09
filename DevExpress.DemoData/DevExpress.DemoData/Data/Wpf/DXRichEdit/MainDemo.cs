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
namespace DevExpress.DemoData.Model {
	public static partial class Repository {
		static List<Module> Create_DXRichEdit_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "FloatingObjects",
					displayName: @"Floating Objects",
					group: "Editing Features",
					type: "RichEditDemo.FloatingObjects",
					shortDescription: @"Explore the capabilities to display and manage floating objects.",
					description: @"
                        <Paragraph>
                        Floating objects (pictures and text boxes) can be inserted into the document, freely positioned, scaled or rotated. You can modify object characteristics using command bars or Ribbon UI and use Layout dialog available via context menu. You can drag and resize an object by its outline box and rotate it with the rotate handle.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "AutoCorrect",
					displayName: @"AutoCorrect",
					group: "Editing Features",
					type: "RichEditDemo.AutoCorrect",
					shortDescription: @"Try automatic text substitution while you are typing text.",
					description: @"
                        <Paragraph>
                        The AutoCorrect feature provides automatic text substitution while you are typing text. You can automate the process of inserting frequently used text and graphics, auto-correct typing errors and incorrect capitalization. It is also possible to implement a custom substitution algorithm by handling the AutoCorrect event.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "CharacterFormatting",
					displayName: @"Character Formatting",
					group: "Editing Features",
					type: "RichEditDemo.CharacterFormatting",
					shortDescription: @"Demonstrates character formatting features.",
					description: @"
                        <Paragraph>
                        This demo illustrates the character formatting available. There are various font, font size, and character style settings used for text – bold, italics, underlined, strike-through style, and different colors for the background and foreground. Use the bar buttons to explore the capabilities of the control.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ParagraphFormatting",
					displayName: @"Paragraph Formatting",
					group: "Editing Features",
					type: "RichEditDemo.ParagraphFormatting",
					shortDescription: @"Demonstrates paragraph formatting features.",
					description: @"
                        <Paragraph>
                        This demo illustrates different formatting applied to paragraphs. You can see what various alignment, spacing and indentation look like. The paragraph's content is just a dummy text, known as “Lorem Ipsum”, which is widely used in the printing and typesetting industry.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Styles",
					displayName: @"Styles",
					group: "Editing Features",
					type: "RichEditDemo.Styles",
					shortDescription: @"Demonstrates various document styles.",
					description: @"
                        <Paragraph>
                        This demo illustrates the use of document styles. DXRichEdit supports both paragraph and character based styles, along with multiple style inheritance. The Style Gallery shows a preview of text formatted with a particular style.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BulletsAndNumbering",
					displayName: @"Bullets and Numbering",
					group: "Editing Features",
					type: "RichEditDemo.BulletsAndNumbering",
					shortDescription: @"Demonstrates different types of bulleted and numbered lists.",
					description: @"
                        <Paragraph>
                        This demo illustrates bulleted and numbered lists in a document and the list auto-number feature.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "HyperlinksAndBookmarks",
					displayName: @"Hyperlinks and Bookmarks",
					group: "Editing Features",
					type: "RichEditDemo.HyperlinksAndBookmarks",
					shortDescription: @"Browse the document to see bookmarks and hyperlinks in action.",
					description: @"
                        <Paragraph>
                        This demo features the document bookmarks and hyperlinks. Browse through the document to see hyperlinks pointing to external locations or associated with bookmarks in the same document. To create a bookmark, select a position in the document or select a document range, and use the Bookmark command button on the toolbar. Once created, the bookmark can be referred to via a hyperlink.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "HyperlinkClickHandling",
					displayName: @"Hyperlink Click Handling",
					group: "Editing Features",
					type: "RichEditDemo.HyperlinkClickHandling",
					shortDescription: @"Click a hyperlink to fill in the fields.",
					description: @"
                        <Paragraph>
                        This demo illustrates the capability to handle a hyperlink click to invoke a custom form. User input via this form replaces the hyperlink content, allowing smart modifications of the document. In this demo, you can select an item in the drop-down list, select a date in the calendar or insert a value calculated with the help of the built-in calculator.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "HeadersAndFooters",
					displayName: @"Headers and Footers",
					group: "Editing Features",
					type: "RichEditDemo.HeadersAndFooters",
					shortDescription: @"Double-click to edit document's header or footer.",
					description: @"
                        <Paragraph>
                        This demo features document headers and footers. By clicking the corresponding areas of the document you can enable the Headers and Footers Tools command set.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "MasterDetailMailMerge",
					displayName: @"Master-Detail Mail Merge",
					group: "Mail Merge",
					type: "RichEditDemo.MasterDetailMailMerge",
					shortDescription: @"See how easy it is to create a Master-Detail report using mail merge and a document field event.",
					description: @"
                        <Paragraph>
                        This demo illustrates how the DOCVARIABVLE field functionality enables you to create Master-Detail report during mail merge. Master and Detail templates can be nested within the basic merge template to create a master-detail report. This technique makes use of RichEditDocumentServer that performs mail merge operations at master and detail levels, providing a high degree of configurability and flexibility.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "SimpleDataMerge",
					displayName: @"Mail Merge: Runtime Data",
					group: "Mail Merge",
					type: "RichEditDemo.SimpleDataMerge",
					shortDescription: @"Mail merge using document fields and an IList data source.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to implement simple mail merge using document fields. The control is bound to List data, created and populated at runtime. The built-in Mail Merge toolbar allows inserting data fields from the bound source and switching between field view modes to preview the results. The resulting document can be sent to a file or a new RichEdit control instance for editing.
                        </Paragraph>
                        <Paragraph>
                        You can provide descriptive field names to display in the Insert Merge Field popup menu and selection form. Click the Customize Merge Field button to explore this feature.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "MergeDatabaseRecords",
					displayName: @"Mail Merge: Database",
					group: "Mail Merge",
					type: "RichEditDemo.MergeDatabaseRecords",
					shortDescription: @"See how various data from the database can be merged with the document template.",
					description: @"
                        <Paragraph>
                        This demo illustrates the Mail Merge feature of the RichEditControl that allows inserting any kind of data from the database: texts, dates, and even pictures. A sample Access database representing the Northwind Trading Company is queried for information on managers and customers served by them. Data is grouped in the grid, allowing you to select records for the mail merge. The built-in Mail Merge toolbar allows inserting data fields from the bound source and switching between field view modes to preview the results. The resulting document can be sent to a file or a new RichEdit control instance for editing.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "LoadSaveDoc",
					displayName: @"Load/Save DOC",
					group: "Document Management",
					type: "RichEditDemo.LoadSaveDoc",
					shortDescription: @"Demonstrates the capabilities to load and save files in DOC (Word 97) format.",
					description: @"
                        <Paragraph>
                        The DXRichEdit control enables you to load and save files in DOC (Word 97) format.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "LoadSaveRtf",
					displayName: @"Load/Save RTF",
					group: "Document Management",
					type: "RichEditDemo.LoadSaveRtf",
					shortDescription: @"Demonstrates the capabilities to load and save files in RTF format.",
					description: @"
                        <Paragraph>
                        The DXRichEdit control enables you to load and save files in RTF format.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "LoadSaveHtml",
					displayName: @"Load/Save HTML",
					group: "Document Management",
					type: "RichEditDemo.LoadSaveHtml",
					shortDescription: @"Demonstrates the capabilities to load and save HTML content.",
					description: @"
                        <Paragraph>
                        This demo illustrates the capabilities to load and save HTML content. Load an HTML file and look at how it is displayed in the RichEdit control and in the standard Web browser. The page markup is shown at the bottom, so you can observe how HTML tags are interpreted.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "TableOfContents",
					displayName: @"Table of Contents",
					group: "Layout and Navigation",
					type: "RichEditDemo.TableOfContents",
					shortDescription: @"Use command buttons to experiment with the table of contents and create a table of figures.",
					description: @"
                        <Paragraph>
                        This demo shows how you can programmatically insert an automatic table of contents in a document that was created using a mail merge. You can optionally modify the mail merge template shown in the first tab. Click the Merge to New Document button to perform a mail merge. The resulting document is displaying the table of contents. Use CTRL+Click to navigate table entries.
                        </Paragraph>
                        <Paragraph>
                        You are encouraged to insert pictures, captions and create automatic Table of Figures yourself.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "LineNumbering",
					displayName: @"Line Numbering",
					group: "Layout and Navigation",
					type: "RichEditDemo.LineNumbering",
					shortDescription: @"See how line numbering can be used for certain legal documents.",
					description: @"
                        <Paragraph>
                        This demo illustrates the line numbering feature that allows you to add line numbering to the document margins as required for certain types of legal documents. Line numbers can run continuously through the document, restarted on each page or section or suppressed for a particular paragraph.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "Tables",
					displayName: @"Tables",
					group: "Layout and Navigation",
					type: "RichEditDemo.Tables",
					shortDescription: @"Modify document tables via command bars or popup menus.",
					description: @"
                        <Paragraph>
                        Explore the DXRichEdit capability to display, edit and create tables in the document. Try the new AutoFit feature.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "SyntaxHighlighting",
					displayName: @"Syntax Highlighting",
					group: "Layout and Navigation",
					type: "RichEditDemo.SyntaxHighlighting",
					shortDescription: @"Demonstrates its code highlighted using DevExpress CodeParser library.",
					description: @"
                        <Paragraph>
                        This demo shows how RichEditControl can be used for syntax highlighting. The text is parsed into tokens, token formatting is specified and applied to the loaded document. You can use DevExpress CodeParser library or implement your own custom parser.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "Views",
					displayName: @"Document Views and Layouts",
					group: "Layout and Navigation",
					type: "RichEditDemo.Views",
					shortDescription: @"Explore three basic views available for the document.",
					description: @"
                        <Paragraph>
                        This demo illustrates built-in document views. The control ships with 3 views – Simple, which is suitable for typing and spell checking, Draft – for text formatting, and Print Layout, which is the most appropriate for finishing the document before printing.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Sections",
					displayName: @"Multi-Column Content",
					group: "Layout and Navigation",
					type: "RichEditDemo.Sections",
					shortDescription: @"Demonstrates a document divided into multi-column sections with different settings.",
					description: @"
                        <Paragraph>
                        This demo illustrates the ways in which a document can be divided into multiple sections with different page settings. You can specify the number of columns, page orientation and margins for each section.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Zooming",
					displayName: @"Zooming",
					group: "Layout and Navigation",
					type: "RichEditDemo.Zooming",
					shortDescription: @"Zoom in and out and see all functionality retained at any zoom level.",
					description: @"
                        <Paragraph>
                        This demo allows you to explore the zooming capabilities of the control. Hold the CTRL key and use your mouse wheel, or simply drag the slider to magnify or reduce zoom level.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "FindAndReplace",
					displayName: @"Find and Replace",
					group: "Layout and Navigation",
					type: "RichEditDemo.FindAndReplace",
					shortDescription: @"Find and replace characters or text strings within a document, use regular expressions if necessary.",
					description: @"
                        <Paragraph>
                        This demo shows the capability to find and replace characters or text strings within a document. You can enhance the search and replace operations, by using regular expressions.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DocumentProtection",
					displayName: @"Document Protection",
					group: "Restrictions",
					type: "RichEditDemo.DocumentProtection",
					shortDescription: @"Select identity (email) to edit ranges available to you. Unprotect password is '123'.",
					description: @"
                        <Paragraph>
                        This demo illustrates how the document can be protected and editing restrictions can be applied on a per-user basis. Log in as different users, disable document protection and enforce it to observe different editable ranges.
                        </Paragraph>
                        <Paragraph>
                        Password to unprotect a document is '123'.""
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DocumentRestrictions",
					displayName: @"Document Restrictions",
					group: "Restrictions",
					type: "RichEditDemo.DocumentRestrictions",
					shortDescription: @"Apply different end-user document restrictions.",
					description: @"
                        <Paragraph>
                        This demo illustrates how you can restrict operations that can be performed within a document.
                        </Paragraph>
                        <Paragraph>
                        You can also specify whether disabled command bars associated with restricted commands should be visible. To reinforce the protective effect, you can prevent the popup menu from being displayed.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "OperationRestrictions",
					displayName: @"Operation Restrictions",
					group: "Restrictions",
					type: "RichEditDemo.OperationRestrictions",
					shortDescription: @"Apply different end-user operation restrictions.",
					description: @"
                        <Paragraph>
                        This demo illustrates how you can protect the document content and layout. Here, you can decide what kind of operations can be performed on the document. To reinforce the protective effect, the popup menu is not displayed.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "SpellChecking",
					displayName: @"Spell Checking",
					group: "Editing Features",
					type: "RichEditDemo.SpellChecking",
					shortDescription: @"Check spelling explicitly or as you type.",
					description: @"
                        <Paragraph>
                        The DXRichEdit control has built-in support for spell checking.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "RibbonUI",
					displayName: @"Ribbon UI",
					group: "Layout and Navigation",
					type: "RichEditDemo.RibbonUI",
					shortDescription: @"Explore the Ribbon-like UI created using the DXBars Suite.",
					description: @"
                        <Paragraph>
                        This demo illustrates how you can create a Ribbon-like UI using DXBars Suite.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ThirdPartyCommentsModule",
					displayName: @"Comments",
					group: "Editing Features",
					type: "RichEditDemo.ThirdPartyComments",
					shortDescription: @"Use comments to annotate a document.",
					description: @"
                        <Paragraph>
                        This demo illustrates use of annotations in a document. The DXRichEdit control displays the comments in the Reviewing pane and in the margin of the document. You can add new comments, edit them, hide or delete comments if they are no longer needed. Before adding a comment, use the combo box to specify a default author name.
                        </Paragraph>",
					addedIn: KnownDXVersion.V151,
					updatedIn: KnownDXVersion.V152,
					allowRtl: false,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "LayoutAPI",
					displayName: "Layout API",
					group: "Layout and Navigation",
					type: "RichEditDemo.LayoutAPI",
					shortDescription: @"Explore the document layout structure using the tree view.",
					description: @"
                    <Paragraph>
                    This demo illustrates the use of Layout API to create a tree-like representation of the document layout. Click a node to select the range that relates to the layout element. Selection is restricted to ranges contained within the main document (headers, footers and text boxes are excluded).
                    </Paragraph>
                    <Paragraph>
                    The bottom-right corner of the window displays the coordinates of the layout box encompassing the selected element (X, Y, width, height). Most layout elements are associated with individual ranges in the document. The start position and length of the corresponding range is displayed when appropriate.
                    </Paragraph>", 
					addedIn: KnownDXVersion.V151,
					updatedIn: KnownDXVersion.V152,
					allowRtl: false,
					isFeatured: true
					)
			};
		}
	}
}
