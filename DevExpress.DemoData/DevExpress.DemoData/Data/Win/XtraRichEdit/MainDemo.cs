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
		static List<Module> Create_XtraRichEdit_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraRichEdit %MarketingVersion%",
					group: "About",
					type: "DevExpress.XtraRichEdit.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "CharacterFormattingModule",
					displayName: @"Character Formatting",
					group: "Editing Features",
					type: "DevExpress.XtraRichEdit.Demos.CharacterFormattingModule",
					description: @"The XtraRichEdit ships everything you’d expect from a powerhouse word processor…including a rich set of character formatting options. Explore this demo by applying the character settings and font effects available to you via the Home tab of the Ribbon.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\CharacterFormatting.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\CharacterFormatting.vb"
					}
				),
				new SimpleModule(demo,
					name: "ParagraphFormattingModule",
					displayName: @"Paragraph Formatting",
					group: "Editing Features",
					type: "DevExpress.XtraRichEdit.Demos.ParagraphFormattingModule",
					description: @"This demo helps illustrate the different paragraph formatting options (alignment, spacing, indentation) available to you when using the XtraRichEdit. Give it a try and see why readers of Visual Studio Magazine votes the XtraRichEdit Suite best in class.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\ParagraphFormatting.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\ParagraphFormatting.vb"
					}
				),
				new SimpleModule(demo,
					name: "ConditionalFormattingModule",
					displayName: @"Conditional Formatting",
					group: "Editing Features",
					type: "DevExpress.XtraRichEdit.Demos.ConditionalFormattingModule",
					description: @"The XtraRichEdit includes conditional formatting support so you can maintain total control over the manner in which information is presented to end-users. In this demo, conditional styles are applied to character, paragraph and table elements.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\ConditionalFormatting.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\ConditionalFormatting.vb"
					}
				),
				new SimpleModule(demo,
					name: "StylesModule",
					displayName: @"Styles",
					group: "Editing Features",
					type: "DevExpress.XtraRichEdit.Demos.StylesModule",
					description: @"The DevExpress WinForms Rich Edit Control supports both paragraph and character based styles, along with multiple style inheritance. A built-in Style Gallery allows you to preview formatting before it is applied to the contents of the Rich Edit. Give it a try and see how it works…select a block of text within the editor, navigate to the Home Tab of the Ribbon and select the desired style from the Style Gallery.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\Styles.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\Styles.vb"
					}
				),
				new SimpleModule(demo,
					name: "BulletsAndNumberingModule",
					displayName: @"Bullets and Numbering",
					group: "Editing Features",
					type: "DevExpress.XtraRichEdit.Demos.BulletsAndNumberingModule",
					description: @"This straightforward demo illustrates how the XtraRichEdit displays numbered lists within a document and how you can control the auto-number feature. Use the corresponding buttons on the Home Tab of the Ribbon to make the desired changes.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\BulletsAndNumbering.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\BulletsAndNumbering.vb"
					}
				),
				new SimpleModule(demo,
					name: "HyperlinksModule",
					displayName: @"Hyperlinks and Bookmarks",
					group: "Editing Features",
					type: "DevExpress.XtraRichEdit.Demos.HyperlinksModule",
					description: @"This demo helps illustrate built-in hyperlink and bookmark support available within the XtraRichEdit. Once you browse the document, you’ll notice hyperlinks to external locations and bookmarks to content within the document. To create a bookmark, select a position within the document or document range and sue the Bookmark command button on the Insert Tab of the Ribbon. Once a bookmark is generated, hyperlinks allow you to navigate to it.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\Hyperlinks.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\Hyperlinks.vb"
					}
				),
				new SimpleModule(demo,
					name: "HyperlinkClickHandlingModule",
					displayName: @"Hyperlink Click Handling",
					group: "Editing Features",
					type: "DevExpress.XtraRichEdit.Demos.HyperlinkClickHandlingModule",
					description: @"Yet another must have feature that ships inside the XtraRichEdit Suite is its support of document headers and footers. By clicking the corresponding regions of the document, you can enable the Rich Edit’s Header and Footer command set, which includes all necessary options to edit the contents of the document’s header and footer.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\HyperlinkClickHandling.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\HyperlinkClickHandling.vb"
					}
				),
				new SimpleModule(demo,
					name: "TablesModule",
					displayName: @"Tables",
					group: "Editing Features",
					type: "DevExpress.XtraRichEdit.Demos.TablesModule",
					description: @"As you might expect, the DevExpress WinForms Rich Edit Control ships with integrated table support and offers you many of the features you’ve come to expect from word processors such as Microsoft Word. In this demo, we illustrate a few of the capabilities available to you, include AutoFit. Give it a try and see how easy it is to create, manage and modify tables within the XtraRichEdit.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\Tables.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\Tables.vb"
					}
				),
				new SimpleModule(demo,
					name: "HeadersFootersModule",
					displayName: @"Headers and Footers",
					group: "Editing Features",
					type: "DevExpress.XtraRichEdit.Demos.HeadersFootersModule",
					description: @"This demo illustrates the how the document's headers and footers are displayed by the XtraRichEdit. By clicking the corresponding areas of the document you can enable the Headers and Footers Tools command set. It contains commands used to modify options specific for headers and footers.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\HeadersFootes.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\HeadersFootes.vb"
					}
				),
				new SimpleModule(demo,
					name: "ViewsModule",
					displayName: @"Document Views and Layouts",
					group: "Layout & Navigation",
					type: "DevExpress.XtraRichEdit.Demos.ViewsModule",
					description: @"Much like Microsoft Word, the XtraRichEdit Suite ships with 3 unique View options: Simple, Draft and Print Layout. Use the commands on the View tab of the Ribbon to switch between any of these views. Simple is suitable for typing and spell checking; Drag can be used for text formatting; And Print Layout is best used to prepare the document for printing.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\Views.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\Views.vb"
					}
				),
				new SimpleModule(demo,
					name: "SectionsModule",
					displayName: @"Multi-Column Content",
					group: "Layout & Navigation",
					type: "DevExpress.XtraRichEdit.Demos.SectionsModule",
					description: @"This demo helps illustrates the ways in which a document can be divided into sections with different page settings for each. You’ll find it extremely easy to specify the number of columns, page orientation and margins for each section.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\Sections.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\Sections.vb"
					}
				),
				new SimpleModule(demo,
					name: "ZoomModule",
					displayName: @"Zooming",
					group: "Layout & Navigation",
					type: "DevExpress.XtraRichEdit.Demos.ZoomModule",
					description: @"In this demo, you can explore the document zooming capabilities we’ve built into the XtraRichEdit. Much like Microsoft Word, you can either use the zoom bar to control zoom level or by pressing the CTRL key while scrolling your mouse wheel.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\Zoom.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\Zoom.vb"
					}
				),
				new SimpleModule(demo,
					name: "SearchCapabilitiesModule",
					displayName: @"Find and Replace",
					group: "Layout & Navigation",
					type: "DevExpress.XtraRichEdit.Demos.SearchCapabilitiesModule",
					description: @"This demo allows you to find and replace characters or text strings within a document. You can specify search direction and numerous other search/replace options available in today's most popular word processing programs.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\SearchCapabilities.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\SearchCapabilities.vb"
					}
				),
				new SimpleModule(demo,
					name: "LineNumberingModule",
					displayName: @"Line Numbering",
					group: "Layout & Navigation",
					type: "DevExpress.XtraRichEdit.Demos.LineNumberingModule",
					description: @"This demo helps illustrate the line numbering capabilities of the XtraRichEdit, a feature often used by the legal profession. As you can see, line numbers can be inserted into document margins and can run continuously throughout the document, restarted on each page/section or suppressed for a specific paragraph.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\LineNumbering.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\LineNumbering.vb"
					}
				),
				new SimpleModule(demo,
					name: "DocumentRestrictionsModule",
					displayName: @"Document Restrictions",
					group: "Restrictions",
					type: "DevExpress.XtraRichEdit.Demos.DocumentRestrictionsModule",
					description: @"In this demo, we illustrate the level of control the XtraRichEdit provides to you and ways in which you can disable formatting actions within business-sensitive documents. Changes can be restricted and confined to specific characteristics of document objects. When restrictions are applied, changes that have been restricted will not be applied to the document and modifications will be discarded.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\DocumentRestrictions.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\DocumentRestrictions.vb"
					}
				),
				new SimpleModule(demo,
					name: "OperationRestrictionsModule",
					displayName: @"Operation Restrictions",
					group: "Restrictions",
					type: "DevExpress.XtraRichEdit.Demos.OperationRestrictionsModule",
					description: @"This demo illustrates how you can restrict operations that can be performed within a document. You can also specify whether disabled command bars associated with restricted commands should be visible. To reinforce the protective effect, you can prevent the popup menu from being displayed.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\OperationRestrictions.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\OperationRestrictions.vb"
					}
				),
				new SimpleModule(demo,
					name: "SimpleDataMergeModule",
					displayName: @"Mail Merge: Runtime Data",
					group: "Mail Merge",
					type: "DevExpress.XtraRichEdit.Demos.SimpleDataMergeModule",
					description: @"This demo illustrates an older approach for creating mail merge documents with the XtraRichEdit control by embedding data fields into the static document content. 
To learn about a newer and more convenient approach to mail merge, check out the WYSIWYG Report Writer demo in the WinForms section of this Demo Center.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\SimpleDataMerge.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\SimpleDataMerge.vb"
					}
				),
				new SimpleModule(demo,
					name: "MergeDatabaseRecordsModule",
					displayName: @"Mail Merge: Database",
					group: "Mail Merge",
					type: "DevExpress.XtraRichEdit.Demos.MergeDatabaseRecordsModule",
					description: @"This demo illustrates an older approach for creating mail merge documents with the XtraRichEdit control by embedding data fields into the static document content. 
To learn about a newer and more convenient approach to mail merge, check out the WYSIWYG Report Writer demo in the WinForms section of this Demo Center.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\MergeDatabaseRecords.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\MergeDatabaseRecords.vb"
					}
				),
				new SimpleModule(demo,
					name: "TableOfContentsModule",
					displayName: @"Table of Contents",
					group: "Mail Merge",
					type: "DevExpress.XtraRichEdit.Demos.TableOfContentsModule",
					description: @"This demo illustrates an older approach for creating mail merge documents with the XtraRichEdit control by embedding data fields into the static document content. 
To learn about a newer and more convenient approach to mail merge, check out the WYSIWYG Report Writer demo in the WinForms section of this Demo Center.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\TableOfContents.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\TableOfContents.vb"
					}
				),
				new SimpleModule(demo,
					name: "MasterDetailMailMergeModule",
					displayName: @"Master-Detail Mail Merge",
					group: "Mail Merge",
					type: "DevExpress.XtraRichEdit.Demos.MasterDetailMailMergeModule",
					description: @"This demo illustrates an older approach for creating mail merge documents with the XtraRichEdit control by embedding data fields into the static document content. 
To learn about a newer and more convenient approach to mail merge, check out the WYSIWYG Report Writer demo in the WinForms section of this Demo Center.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\MasterDetailMailMerge.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\MasterDetailMailMerge.vb"
					}
				),
				new SimpleModule(demo,
					name: "FileOperationsModule",
					displayName: @"Load/Save RTF",
					group: "Document Management",
					type: "DevExpress.XtraRichEdit.Demos.FileOperationsModule",
					description: @"As you would expect, the XtraRichEdit is able to load/save RTF and text files.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\FileOperations.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\FileOperations.vb"
					}
				),
				new SimpleModule(demo,
					name: "DocLoadingModule",
					displayName: @"Load/Save DOC",
					group: "Document Management",
					type: "DevExpress.XtraRichEdit.Demos.DocLoadingModule",
					description: @"The RichEditControl can open and save documents in most popular third-party formats, such as Word 97 or Word 2007.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 8,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\FileOperations.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\FileOperations.vb"
					}
				),
				new SimpleModule(demo,
					name: "HtmlLoadingModule",
					displayName: @"Load/Save HTML",
					group: "Document Management",
					type: "DevExpress.XtraRichEdit.Demos.HtmlLoadingModule",
					description: @"This demo illustrates the XtraRichEdit’s ability to load and save HTML content. Give it a try and see for yourself. Load an HTML file and see how it is displayed within the Rich Edit vs. the standard Web Browser control. Refer to the page markup pane to see how the Rich Edit interprets HTML tags.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\HtmlLoading.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\HtmlLoading.vb"
					}
				),
				new SimpleModule(demo,
					name: "DocumentProtectionModule",
					displayName: @"Document Protection",
					group: "Document Management",
					type: "DevExpress.XtraRichEdit.Demos.DocumentProtectionModule",
					description: @"Much like Microsoft Word, the XtraRichEdit allows you to apply password protection to your documents and to place editing restrictions on a per users basis. The editable regions in this sample document are highlighted in yellow and differ from one user to the other. The password for this document is ‘123’.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 7,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\DocumentProtection.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\DocumentProtection.vb"
					}
				),
				new SimpleModule(demo,
					name: "SpellCheckerModule",
					displayName: @"Spell Checking",
					group: "Editing Features",
					type: "DevExpress.XtraRichEdit.Demos.SpellCheckerModule",
					description: @"The XtraRichEdit Suite includes built in spell checking support, including type as you go error detection.
To learn more about the capabilities of the DevExpress WinForms Spell Checker, refer to the XtraSpellChecker demo.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 3,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\SpellChecker.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\SpellChecker.vb"
					}
				),
				new SimpleModule(demo,
					name: "RibbonModule",
					displayName: @"Ribbon UI",
					group: "Layout & Navigation",
					type: "DevExpress.XtraRichEdit.Demos.RibbonModule",
					description: @"So how easy is it to replicate the Office UI with the DevExpress WinForms Rich Edit? Incredibly easy. In this demo, we demonstrate the integration of the DevExpress Ribbon with the Rich Edit Control. And when you’re ready to integrate the Rich Edit in your WinForms project, you’ll find a Visual Studio app template that will generate  this exact user experience automatically – you will not have to write a single line of code to enable all the capabilities you see here.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 1,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\Ribbon.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\Ribbon.vb"
					}
				),
				new SimpleModule(demo,
					name: "FloatingObjectsModule",
					displayName: @"Floating Objects",
					group: "Editing Features",
					type: "DevExpress.XtraRichEdit.Demos.FloatingObjectsModule",
					description: @"To help you replicate the Microsoft Word user experience, the XtraRichEdit ships with floating object support. Whether you want to insert pictures or text boxes into a document, the Rich Edit Control allows you to freely position, scale and rotate all floating objects. You can also modify object characteristics using command bars, the DevExpress Ribbon or use the built-in layout dialog which can be activated via the context menu. Give it a try and see how easy it is to drag and resize an object using its outline and rotate it by using its rotate handle.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 6,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\FloatingObjects.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\FloatingObjects.vb"
					}
				),
				new SimpleModule(demo,
					name: "AutoCorrectModule",
					displayName: @"AutoCorrect",
					group: "Editing Features",
					type: "DevExpress.XtraRichEdit.Demos.AutoCorrectModule",
					description: @"Among the many high-impact features you’ll find in the XtraRichEdit is its support for auto correction. AutoCorrect provides automatic text substitution when you are entering text. Whether it’s inserting frequently used text fragments, correcting capitalization errors or auto correction of typing errors, you are in total control with the Rich Edit. You can even implement custom substitution algorithms by handling the AutoCorrect event.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 4,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\AutoCorrect.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\AutoCorrect.vb"
					}
				),
				new SimpleModule(demo,
					name: "SyntaxHighlightModule",
					displayName: @"Syntax Highlighting",
					group: "Layout & Navigation",
					type: "DevExpress.XtraRichEdit.Demos.SyntaxHighlightModule",
					description: @"Though you may not need to build a code editor, this demo helps illustrate the flexibility of the XtraRichEdit control. We parse text into tokens, then specify token formatting and apply it to the document. We’ve made our code parser library available to you should you ever wish to experiment with this capability or create solutions that require similar functionality in your industry.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 5,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\SyntaxHighlight.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\SyntaxHighlight.vb"
					}
				),
				new SimpleModule(demo,
					name: "PDFExporterModule",
					displayName: @"Generate PDF",
					group: "Document Management",
					type: "DevExpress.XtraRichEdit.Demos.PDFExporterModule",
					description: @"The DevExpress WinForms Rich Edit Control allows you to introduce Microsoft Word inspired capabilities into your WinForms applications. In this demo, we illustrate the ease with which you can generate PDF files directly from the contents of the XtraRichEdit Control. Make any changes you’d like in the document, then press the Generate PDF button on the File Tab of the Ribbon Control to view PDF output.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\PDFExporter.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\PDFExporter.vb"
					}
				),
				new SimpleModule(demo,
					name: "ThirdPartyCommentsModule",
					displayName: @"Comments",
					group: "Editing Features",
					type: "DevExpress.XtraRichEdit.Demos.ThirdPartyCommentsModule",
					description: @"This demo illustrates use of annotations in a document. XtraRichEdit displays the comments in the Reviewing pane or in the margin of the document. You can add new comments, edit them, hide or delete comments if they are no longer needed. Before adding a comment, use the combo box to specify a default author name.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152,
					featuredPriority: 9,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\ThirdPartyCommentsModule.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\ThirdPartyCommentsModule.vb"
					}
				),
				new SimpleModule(demo,
					name: "LayoutAPI",
					displayName: @"Layout API",
					group: "Layout & Navigation",
					type: "DevExpress.XtraRichEdit.Demos.LayoutAPI",
					description: @"This demo illustrates how to use the Layout API to create a tree-like representation of the document layout. Click a node to select the range that relates to the layout element. Selection is restricted to ranges contained within the main document (headers, footers and text boxes are excluded). The bottom-right corner of the window displays the coordinates of the layout box encompassing the selected element (X, Y, width, height) and the start position and length of the document range.",
					addedIn: KnownDXVersion.V151,
					featuredPriority: 9,
					isFeatured: true,
					updatedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\RichEditMainDemo\Modules\LayoutAPI.cs",
						@"\WinForms\VB\RichEditMainDemo\Modules\LayoutAPI.vb"
					}
				)
			};
		}
	}
}
