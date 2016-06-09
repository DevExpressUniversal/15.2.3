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
		static List<Module> Create_DocumentServerForWin_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress Document Server %MarketingVersion%",
					group: "About",
					type: "DevExpress.Docs.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "SpreadsheetAPI",
					displayName: @"Spreadsheet API",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.SpreadsheetAPIModule",
					description: @"This demo introduces objects and methods available via Spreadsheet API. The TreeView on the right lists the names of actions illustrating API functionality. Click the action and you will see a code snippet and a preview of what the resulting spreadsheet looks like. The parent node indicates the code file containing this method so you can easily locate it within the demo module.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetAPI.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetAPI.vb"
					}
				),
				new SimpleModule(demo,
					name: "ChartAPI",
					displayName: @"Chart API",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.ChartsAPIModule",
					description: @"This demo introduces objects and methods available via Spreadsheet Chart API. The TreeView on the right lists the names of actions illustrating API functionality. Click the action and you will see a code snippet and a preview of what the resulting spreadsheet looks like. The parent node indicates the code file containing this method so you can easily locate it within the demo module.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetChartsAPI.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetChartsAPI.vb"
					}
				),
				new SimpleModule(demo,
					name: "PivotAPI",
					displayName: @"Pivot Table API",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.PivotAPIModule",
					description: @"This demo introduces objects and methods available via Spreadsheet Pivot Table API. The TreeView on the right lists the names of actions illustrating API functionality. Click the action and you will see a code snippet and a preview of what the resulting spreadsheet looks like. The parent node indicates the code file containing this method so you can easily locate it within the demo module.",
					addedIn: KnownDXVersion.V152,
					newUpdatedPriority: 1,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetPivotAPI.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetPivotAPI.vb"
					}
				),
				new SimpleModule(demo,
					name: "XLExportAPI",
					displayName: @"XL Export API",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.XLExportAPIModule",
					description: @"This demo introduces objects and methods available via XL Export API. The TreeView on the right lists the names of actions illustrating API functionality. Click the action and you will see a code snippet. The parent node indicates the code file containing this method so you can easily locate it within the demo module.",
					addedIn: KnownDXVersion.V151,
					updatedIn: KnownDXVersion.V152,
					newUpdatedPriority: 2,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\XLExportAPIModule.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\XLExportAPIModule.vb"
					}
				),
				new SimpleModule(demo,
					name: "HeaderFooter",
					displayName: @"Headers and Footers",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.HeaderFooter",
					description: @"This demo shows the printout of a sales report with a header and footer inserted at the top and bottom of the page. Click the Save As... button to save a worksheet to a file in one of the supported spreadsheet formats.",
					addedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetHeaderFooter.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetHeaderFooter.vb"
					}
				),
				new SimpleModule(demo,
					name: "BreakevenAnalysis",
					displayName: @"Breakeven Analysis",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.BreakevenAnalysis",
					description: @"The Spreadsheet Document Server allows you to embed charts in a worksheet. In this demo, we use both a line chart and a pie chart to visualize breakeven points.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetBreakevenAnalysis.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetBreakevenAnalysis.vb"
					}
				),
				new SimpleModule(demo,
					name: "SimpleLoanCalculator",
					displayName: @"Simple Loan Calculator",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.SpreadsheetSimpleLoanCalculatorModule",
					description: @"This demo illustrates how to use Spreadsheet API to create a Simple Loan Calculator spreadsheet template that automatically performs specific calculations. In the fields at the top, you can change the loan amount, duration, interest value and the start date. Data is automatically processed when you enter new values and results of calculations are displayed in the preview window.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetSimpleLoanCalculator.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetSimpleLoanCalculator.vb"
					}
				),
				new SimpleModule(demo,
					name: "MailMerge",
					displayName: @"Mail Merge Template",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.SpreadsheetMailMergeModule",
					description: @"This demo illustrates how to implement a simple data substitution (mail merge) in a spreadsheet. Fill in the fields at the top and click the Save As... button. The resulting worksheet will contain specified data placed in corresponding cells.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetMailMerge.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetMailMerge.vb"
					}
				),
				new SimpleModule(demo,
					name: "EmployeeInformation",
					displayName: @"Employee Information",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.SpreadsheetEmployeeInformationModule",
					description: @"This demo illustrates how to use the Spreadsheet Document Server functionality to create an Individual Paystubs spreadsheet template that automatically performs specific calculations.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetEmployeeInformation.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetEmployeeInformation.vb"
					}
				),
				new SimpleModule(demo,
					name: "ShiftSchedule",
					displayName: @"Shift Schedule",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.SpreadsheetShiftScheduleModule",
					description: @"This demo illustrates how to create a Shift Schedule spreadsheet template. The TOTAL column includes array formulas to automatically calculate the total number of work hours per shift by employees.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetShiftSchedule.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetShiftSchedule.vb"
					}
				),
				new SimpleModule(demo,
					name: "SportResults",
					displayName: @"Sport Results",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.SpreadsheetSportResultsModule",
					description: @"This demo illustrates how to use the built-in functions (INDEX, LARGE, SMALL, ROW, IF, COUNTIF, LOOKUP), array formulas and 3D references to implement a sample application for processing sport results.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetSportResults.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetSportResults.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomFunction",
					displayName: @"Custom Function",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.SpreadsheetCustomFunctionModule",
					description: @"This demo illustrates how to implement a custom function for use in a spreadsheet. The number is automatically written in words in different cultures. To create a custom function, implement the CustomFunction interface and add a function to the Custom Functions collection.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetCustomFunction.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetCustomFunction.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomView",
					displayName: @"Custom View",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.SpreadsheetCustomViewModule",
					description: @"This demo guides you through the customization of a worksheet view. You can view the result when you save a modified worksheet to a file and open it in the spreadsheet application. Choose the view type. Then specify the visibility of worksheet gridlines and headings, followed by whether to switch the R1C1 style on or off, whether to set a worksheet tab color, zoom a worksheet, and set the paper size, margins and orientation for worksheet pages. Click the Save As… button to save a worksheet.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetCustomView.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetCustomView.vb"
					}
				),
				new SimpleModule(demo,
					name: "FormulaCalculator",
					displayName: @"Formula Calculator",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.SpreadsheetFormulaCalculatorModule",
					description: @"This demo illustrates the operation of the formula calculation engine. Select a cell where you wish to insert a formula and type the “=” (equal sign), followed by the formula text in the text editor located above the grid. You can also enter the formula directly into the selected cell. Press ENTER to complete the formula entry. The result of the calculation will be automatically displayed in the cell.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetFormulaCalculator.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetFormulaCalculator.vb"
					}
				),
				new SimpleModule(demo,
					name: "MultiplicationTable",
					displayName: @"Pythagorean Table",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.SpreadsheetMultiplicationTableModule",
					description: @"This demo shows how to automatically generate the Pythagorean Table using the formula calculation engine. Specify the number of columns and rows in a table, set the options which allow creating heading cells that indicate multiplication numbers and forcing the table to fit in a single page. Click the Save As… button to save the modified table to a file in one of the supported spreadsheet formats.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetMultiplicationTable.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetMultiplicationTable.vb"
					}
				),
				new SimpleModule(demo,
					name: "SimplifiedMultiplicationTable",
					displayName: @"Multiplication Table",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.SpreadsheetSimplifiedMultiplicationTableModule",
					description: @"This demo illustrates how to use the formula calculation engine to automatically create a multiplication table as a set of times tables of up to 10. The cell formatting functionality is used to set the outside borders for each times table and to highlight cells with products. You can specify the number of times tables and the number of columns into which these tables are arranged. Another option allows you to fit the table in a single page. Click the Save As… button to save the table to a file in one of the supported spreadsheet formats.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetSimplifiedMultiplicationTable.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetSimplifiedMultiplicationTable.vb"
					}
				),
				new SimpleModule(demo,
					name: "CellArt",
					displayName: @"Cell Art",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.SpreadsheetCellArtModule",
					description: @"This demo illustrates advanced cell formatting features. You can split an arbitrary image into a set of cells, each of which is the size of a pixel and is colored according to the corresponding pixel of that image. Click the Load Image button to load an image file and click the Save in XLSX button to create a file containing the selected image.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetCellArt.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetCellArt.vb"
					}
				),
				new SimpleModule(demo,
					name: "PictureGallery",
					displayName: @"Picture Gallery",
					group: "Spreadsheet",
					type: "DevExpress.Docs.Demos.SpreadsheetPictureGalleryModule",
					description: @"This demo shows how you can turn a spreadsheet into a picture gallery by changing the background and inserting pictures as floating objects into locations specified by cell ranges.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\SpreadsheetPictureGallery.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\SpreadsheetPictureGallery.vb"
					}
				),
				new SimpleModule(demo,
					name: "RichEditAPI",
					displayName: @"RichEdit API",
					group: "RichEdit",
					type: "DevExpress.Docs.Demos.RichEditAPIModule",
					description: @"This demo introduces objects and methods available via RichEdit API. The TreeView on the right lists the names of actions illustrating API functionality. Click the action and you will see a code snippet and a preview of what the resulting document looks like. The parent node indicates the code file containing this method so you can easily locate it within the demo module.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\RichEditAPIModule.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\RichEditAPIModule.vb"
					}
				),
				new SimpleModule(demo,
					name: "Export",
					displayName: @"Export Files",
					group: "RichEdit",
					type: "DevExpress.Docs.Demos.RichEditExport",
					description: @"In this demo the RichEditDocumentServer loads a document from a file, displays the document in the Print Preview window and saves the document to different formats using the Export API methods.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\RichEditExport.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\RichEditExport.vb"
					}
				),
				new SimpleModule(demo,
					name: "RichEditFindAndReplace",
					displayName: @"Find and Replace",
					group: "RichEdit",
					type: "DevExpress.Docs.Demos.RichEditFindAndReplace",
					description: @"This demo allows you to find and replace characters or text strings in a document.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\RichEditFindAndReplace.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\RichEditFindAndReplace.vb"
					}
				),
				new SimpleModule(demo,
					name: "MailMerge",
					displayName: @"Mail Merge",
					group: "RichEdit",
					type: "DevExpress.Docs.Demos.RichEditMailMerge",
					description: @"This demo performs a mail merge with a main document which is loaded from a file into a RichEditDocumentServer instance. The data source is the Microsoft Access database. Select the addressee by clicking the Contact Name row in the data grid on the left. The merge is previewed in the Print Preview window.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\RichEditMailMerge.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\RichEditMailMerge.vb"
					}
				),
				new SimpleModule(demo,
					name: "PdfFileAttachment",
					displayName: @"PDF File Attachment",
					group: "Pdf",
					type: "DevExpress.Docs.Demos.PdfFileAttachmentDemo",
					description: @"",
					addedIn: KnownDXVersion.V152,
					newUpdatedPriority: 1,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\PdfFileAttachmentDemo.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\PdfFileAttachmentDemo.vb"
					}
				),
				new SimpleModule(demo,
					name: "PdfDocumentCreationAPI",
					displayName: @"PDF Document Creation API",
					group: "Pdf",
					type: "DevExpress.Docs.Demos.PdfDocumentCreationAPI",
					description: @"",
					addedIn: KnownDXVersion.V151,
					updatedIn: KnownDXVersion.V152,
					newUpdatedPriority: 1,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\PdfDocumentCreationAPI.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\PdfDocumentCreationAPI.vb"
					}
				),
				new SimpleModule(demo,
					name: "PdfPasswordProtection",
					displayName: @"PDF Password Protection",
					group: "Pdf",
					type: "DevExpress.Docs.Demos.PdfPasswordProtection",
					description: @"",
					addedIn: KnownDXVersion.V151,
					newUpdatedPriority: 2,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\PdfPasswordProtection.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\PdfPasswordProtection.vb"
					}
				), 
				new SimpleModule(demo,
					name: "PdfSignatureDemo",
					displayName: @"PDF Signature",
					group: "Pdf",
					type: "DevExpress.Docs.Demos.PdfSignatureDemo",
					description: @"",
					addedIn: KnownDXVersion.V151,
					newUpdatedPriority: 3,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\PdfSignatureDemo.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\PdfSignatureDemo.vb"
					}
				), 
				new SimpleModule(demo,
					name: "PdfFormFill",
					displayName: @"PDF Form Filling",
					group: "Pdf",
					type: "DevExpress.Docs.Demos.PdfFormFill",
					description: @"This demo illustrates how to enable the Form Filling feature for the PDF Document Server and programmatically pass values to fill the PDF Form. To see how it works, specify necessary values in the Demo Options and press the Submit button.",
					addedIn: KnownDXVersion.Before142,
					newUpdatedPriority: 2,
					 associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\PdfFormFill.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\PdfFormFill.vb"
					}
				),
				new SimpleModule(demo,
					name: "PdfMergeDocs",
					displayName: @"PDF Page Merging",
					group: "Pdf",
					type: "DevExpress.Docs.Demos.PdfMergeDocs",
					description: @"",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152,
					newUpdatedPriority: 1,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\PdfMergeDocs.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\PdfMergeDocs.vb"
					}
				),
				new SimpleModule(demo,
					name: "PdfDeletePage",
					displayName: @"PDF Page Deletion",
					group: "Pdf",
					type: "DevExpress.Docs.Demos.PdfDeletePage",
					description: @"",
					addedIn: KnownDXVersion.Before142,
					 associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\PdfDeletePage.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\PdfDeletePage.vb"
					}
				),
				new SimpleModule(demo,
					name: "PdfTextExtraction",
					displayName: @"PDF Text Extraction",
					group: "Pdf",
					type: "DevExpress.Docs.Demos.PdfTextExtraction",
					description: @"",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\PdfTextExtraction.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\PdfTextExtraction.vb"
					}
				),
				new SimpleModule(demo,
					name: "PdfTextSearch",
					displayName: @"PDF Text Search",
					group: "Pdf",
					type: "DevExpress.Docs.Demos.PdfTextSearch",
					description: @"",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Document Server\CS\DocsMainDemo\Modules\PdfTextSearch.cs",
						@"\Document Server\VB\DocsMainDemo\Modules\PdfTextSearch.vb"
					}
				),
				new SimpleModule(demo,
					name: "BarCodeVisualization",
					displayName: @"Bar Code Visualization",
					group: "BarCode",
					type: "DevExpress.Docs.Demos.BarCodeVisualization",
					description: @"This demo illustrates how you can programmatically create barcode images and adjust their appearance. The Barcode library implements the majority of industry standard barcode types and allows you to set image options and options specific for each code type.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Compression",
					displayName: @"Compression Examples",
					group: "Compression",
					type: "DevExpress.Docs.Demos.Compression",
					description: @"This demo uses the Data Compression library to zip folders and files with an optional encryption scheme.  Select files or folders with a Ctrl key pressed for multiple selection. To encrypt selected files, type a password and select encryption type in the Options panel. If the Password field is left blank, no encryption is applied. Click the Add to Archive... button to specify a name of the resulting zip file and create a file archive. The status bar indicates progress and allows you to click the Cancel button to stop compression procedure.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Decompression",
					displayName: @"Decompression Examples",
					group: "Compression",
					type: "DevExpress.Docs.Demos.Decompression",
					description: @"This demo uses the Data Compression library to unzip file archives. Select the file, navigate the folder structure of a zipped file archive and view the content of the selected file. If the file is encrypted, you will be prompted for a password.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "UnitConversion",
					displayName: @"Unit Conversion",
					group: "Miscellaneous",
					type: "DevExpress.Docs.Demos.UnitConversion",
					description: @"This demo illustrates how to use our Unit Conversion library to create a simple unit converter. Select a tab and enter value in any available unit of measurement. It is automatically converted to other units of the same physical quantity.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "EncodingDetection",
					displayName: @"Encoding Detection",
					group: "Miscellaneous",
					type: "DevExpress.Docs.Demos.EncodingDetection",
					description: @"This demo illustrates how to use our Document Server to automatically detect character encoding for text files. Load one of our test files or any other text file, and compare how text is displayed without encoding detection and with it.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "NumberToWords",
					displayName: @"Number In Words",
					group: "Miscellaneous",
					type: "DevExpress.Docs.Demos.NumberToWords",
					description: @"This demo illustrates how to use our Document Server library to write numbers in words. Enter a positive integer number into the editor at the top and review how this number (cardinal and ordinal) is automatically written in words in different cultures.",
					addedIn: KnownDXVersion.Before142
				)
			};
		}
	}
}
