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

using DevExpress.XtraEditors;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class MailMergePreviewForm :DevExpress.XtraBars.Ribbon.RibbonForm {
		private SpreadsheetControl spreadsheetControl1;
		private XtraBars.Ribbon.RibbonControl ribbonControl1;
		private XtraEditors.Repository.RepositoryItemFontEdit repositoryItemFontEdit1;
		private XtraSpreadsheet.Design.RepositoryItemSpreadsheetFontSizeEdit repositoryItemSpreadsheetFontSizeEdit1;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown1;
		private System.ComponentModel.IContainer components;
		private XtraEditors.Repository.RepositoryItemPopupGalleryEdit repositoryItemPopupGalleryEdit1;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown2;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown3;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown4;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown5;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown6;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown7;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown8;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown9;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown10;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown11;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown12;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown13;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown14;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown15;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown16;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown17;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown18;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown19;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown20;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown21;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown22;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown23;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown24;
		private XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
		private SpreadsheetFormulaBarControl spreadsheetFormulaBarControl1;
		private SpreadsheetNameBoxControl spreadsheetNameBoxControl1;
		private SplitContainerControl splitContainerControl1;
		private SplitterControl splitterControl1;
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup1 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem23 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem24 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem25 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem26 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem27 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem28 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem29 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem30 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem31 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem32 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem33 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem34 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem35 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem36 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem37 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem38 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem39 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem40 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem41 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem42 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem43 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem44 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem45 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem46 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem47 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem48 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem49 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem50 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem51 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem52 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem53 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem54 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem55 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem56 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem57 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem58 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem59 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem60 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem61 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem62 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem63 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem64 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem65 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem66 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem67 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem68 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem69 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem70 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem71 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem72 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem73 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem74 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem75 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem76 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem77 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem78 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem79 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem80 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem81 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem82 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem83 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem84 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem85 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem86 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem87 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem88 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem89 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem90 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem91 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem92 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem93 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem94 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem95 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem96 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem97 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem98 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem99 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem100 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem101 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem102 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup23 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem103 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem104 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem105 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup24 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem106 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem107 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem108 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup25 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem109 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem110 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem111 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem112 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem113 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup26 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem114 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem115 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup27 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem116 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem117 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup28 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem118 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem119 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem120 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup29 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem121 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem122 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem123 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup30 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem124 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem125 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup31 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem126 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem127 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem128 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem129 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup32 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem130 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem131 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem132 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem133 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem134 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem135 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem136 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup33 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem137 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem138 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem139 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem140 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem141 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem142 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem143 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem144 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem145 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem146 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem147 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup34 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem148 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem149 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem150 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem151 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem152 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem153 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem154 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem155 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem156 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup35 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem157 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem158 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem159 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem160 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem161 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem162 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem163 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem164 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem165 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup36 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem166 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem167 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem168 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem169 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup37 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem170 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem171 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem172 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem173 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup38 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem174 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem175 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem176 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem177 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem178 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup39 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem179 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem180 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup40 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem181 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem182 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem183 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem184 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraBars.Ribbon.ReduceOperation reduceOperation1 = new DevExpress.XtraBars.Ribbon.ReduceOperation();
			DevExpress.XtraBars.Ribbon.ReduceOperation reduceOperation2 = new DevExpress.XtraBars.Ribbon.ReduceOperation();
			DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup2 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem185 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem186 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem187 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem188 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem189 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem190 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem191 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem192 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem193 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem194 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem195 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup41 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem196 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem197 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem198 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem199 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem200 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem201 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup42 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem202 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem203 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem204 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem205 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem206 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem207 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup43 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem208 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem209 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem210 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem211 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem212 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem213 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem214 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem215 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem216 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem217 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem218 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem219 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup44 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem220 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem221 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem222 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem223 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem224 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem225 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem226 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup45 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem227 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem228 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem229 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem230 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem231 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup46 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem232 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem233 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem234 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup47 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem235 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem236 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem237 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem238 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem239 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup48 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem240 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem241 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem242 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup49 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem243 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem244 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem245 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem246 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup50 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem247 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem248 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem249 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem250 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup51 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem251 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem252 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem253 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem254 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup52 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem255 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem256 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem257 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem258 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup53 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem259 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem260 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem261 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem262 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem263 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem264 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup54 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem265 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup55 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem266 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem267 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup56 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem268 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem269 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup57 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem270 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem271 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup58 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem272 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem273 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem274 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MailMergePreviewForm));
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup59 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem275 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem276 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem277 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup60 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem278 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem279 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem280 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup61 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem281 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem282 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem283 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup62 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem284 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem285 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem286 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup63 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem287 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem288 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem289 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup64 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem290 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem291 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem292 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup65 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem293 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem294 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem295 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem296 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem297 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup66 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem298 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem299 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup67 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem300 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem301 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup68 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem302 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem303 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem304 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup69 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem305 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem306 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem307 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup70 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem308 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem309 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup71 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem310 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem311 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem312 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem313 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup72 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem314 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem315 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem316 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem317 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem318 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem319 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem320 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup73 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem321 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem322 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem323 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem324 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem325 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem326 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem327 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem328 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem329 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem330 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem331 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup74 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem332 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem333 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem334 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem335 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem336 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem337 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem338 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem339 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem340 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup75 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem341 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem342 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem343 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem344 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem345 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem346 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem347 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem348 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem349 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup76 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem350 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem351 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem352 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem353 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup77 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem354 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem355 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem356 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem357 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup78 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem358 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem359 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem360 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem361 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem362 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup79 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem363 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem364 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup80 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem365 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem366 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem367 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem368 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
			this.spreadsheetCommandBarSubItem4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarSubItem5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem43 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem44 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem45 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem46 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem47 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem48 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem49 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem50 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem51 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem52 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem53 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem54 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem55 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonGalleryDropDownItem1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown26 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.spreadsheetCommandBarButtonItem1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.barButtonGroup1 = new DevExpress.XtraBars.BarButtonGroup();
			this.changeFontNameItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeFontNameItem();
			this.repositoryItemFontEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.changeFontSizeItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeFontSizeItem();
			this.repositoryItemSpreadsheetFontSizeEdit2 = new DevExpress.XtraSpreadsheet.Design.RepositoryItemSpreadsheetFontSizeEdit();
			this.spreadsheetControl1 = new DevExpress.XtraSpreadsheet.SpreadsheetControl();
			this.spreadsheetCommandBarButtonItem14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.barButtonGroup2 = new DevExpress.XtraBars.BarButtonGroup();
			this.spreadsheetCommandBarCheckItem1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.barButtonGroup3 = new DevExpress.XtraBars.BarButtonGroup();
			this.spreadsheetCommandBarSubItem1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem23 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem24 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem25 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem26 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem27 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem28 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.changeBorderLineColorItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeBorderLineColorItem();
			this.changeBorderLineStyleItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeBorderLineStyleItem();
			this.commandBarGalleryDropDown25 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.barButtonGroup4 = new DevExpress.XtraBars.BarButtonGroup();
			this.changeCellFillColorItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeCellFillColorItem();
			this.changeFontColorItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeFontColorItem();
			this.barButtonGroup5 = new DevExpress.XtraBars.BarButtonGroup();
			this.spreadsheetCommandBarCheckItem5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.barButtonGroup6 = new DevExpress.XtraBars.BarButtonGroup();
			this.spreadsheetCommandBarCheckItem8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.barButtonGroup7 = new DevExpress.XtraBars.BarButtonGroup();
			this.spreadsheetCommandBarButtonItem29 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem30 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarCheckItem11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarSubItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarCheckItem12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarButtonItem31 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem32 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem33 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.barButtonGroup8 = new DevExpress.XtraBars.BarButtonGroup();
			this.changeNumberFormatItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeNumberFormatItem();
			this.repositoryItemPopupGalleryEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemPopupGalleryEdit();
			this.barButtonGroup9 = new DevExpress.XtraBars.BarButtonGroup();
			this.spreadsheetCommandBarSubItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem34 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem35 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem36 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem37 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem38 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem39 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem40 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.barButtonGroup10 = new DevExpress.XtraBars.BarButtonGroup();
			this.spreadsheetCommandBarButtonItem41 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem42 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonGalleryDropDownItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown27 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown28 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonItem56 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem57 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.galleryFormatAsTableItem1 = new DevExpress.XtraSpreadsheet.UI.GalleryFormatAsTableItem();
			this.commandBarGalleryDropDown29 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.galleryChangeStyleItem1 = new DevExpress.XtraSpreadsheet.UI.GalleryChangeStyleItem();
			this.spreadsheetCommandBarSubItem8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem58 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem59 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem60 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem61 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem62 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem63 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem64 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem65 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem66 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem67 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem68 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem69 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem70 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem71 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem72 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem73 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem74 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem75 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.changeSheetTabColorItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeSheetTabColorItem();
			this.spreadsheetCommandBarButtonItem76 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarCheckItem13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarButtonItem77 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem78 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem79 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem80 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem81 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem82 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem83 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem84 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem85 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem86 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem87 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem88 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem89 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem90 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem91 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem92 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem93 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem94 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem95 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem96 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem97 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem98 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonGalleryDropDownItem4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown30 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown31 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown32 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown33 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown34 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown35 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown36 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonItem99 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem100 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarCheckItem14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarSubItem18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarCheckItem17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.pageSetupPaperKindItem1 = new DevExpress.XtraSpreadsheet.UI.PageSetupPaperKindItem();
			this.spreadsheetCommandBarSubItem19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem101 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem102 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarCheckItem19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarSubItem20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem103 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem104 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem105 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem106 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.functionsFinancialItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsFinancialItem();
			this.functionsLogicalItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsLogicalItem();
			this.functionsTextItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsTextItem();
			this.functionsDateAndTimeItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsDateAndTimeItem();
			this.functionsLookupAndReferenceItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsLookupAndReferenceItem();
			this.functionsMathAndTrigonometryItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsMathAndTrigonometryItem();
			this.spreadsheetCommandBarSubItem23 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.functionsStatisticalItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsStatisticalItem();
			this.functionsEngineeringItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsEngineeringItem();
			this.functionsInformationItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsInformationItem();
			this.functionsCompatibilityItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsCompatibilityItem();
			this.functionsWebItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsWebItem();
			this.spreadsheetCommandBarButtonItem112 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem113 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.definedNameListItem1 = new DevExpress.XtraSpreadsheet.UI.DefinedNameListItem();
			this.spreadsheetCommandBarButtonItem114 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarCheckItem23 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarSubItem24 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarCheckItem24 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem25 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarButtonItem115 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem116 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem117 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem118 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem119 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem120 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem121 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem122 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem123 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem25 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem124 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem125 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem126 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem127 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem128 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem129 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem130 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.galleryChartLayoutItem1 = new DevExpress.XtraSpreadsheet.UI.GalleryChartLayoutItem();
			this.galleryChartStyleItem1 = new DevExpress.XtraSpreadsheet.UI.GalleryChartStyleItem();
			this.spreadsheetCommandBarButtonGalleryDropDownItem11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown37 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarSubItem26 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonGalleryDropDownItem12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown38 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown39 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown40 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown41 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarSubItem27 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonGalleryDropDownItem16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown42 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown43 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarSubItem28 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonGalleryDropDownItem18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown44 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown45 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown46 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown47 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetCommandBarButtonGalleryDropDownItem22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
			this.commandBarGalleryDropDown48 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
			this.renameTableItem1 = new DevExpress.XtraSpreadsheet.UI.RenameTableItem();
			this.repositoryItemTextEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
			this.spreadsheetCommandBarCheckItem26 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem27 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem28 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem29 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem30 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem31 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarCheckItem32 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.galleryTableStylesItem1 = new DevExpress.XtraSpreadsheet.UI.GalleryTableStylesItem();
			this.chartToolsRibbonPageCategory1 = new DevExpress.XtraSpreadsheet.UI.ChartToolsRibbonPageCategory();
			this.chartsDesignRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.ChartsDesignRibbonPage();
			this.chartsLayoutRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.ChartsLayoutRibbonPage();
			this.chartsFormatRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.ChartsFormatRibbonPage();
			this.tableToolsRibbonPageCategory1 = new DevExpress.XtraSpreadsheet.UI.TableToolsRibbonPageCategory();
			this.tableToolsDesignRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.TableToolsDesignRibbonPage();
			this.pictureToolsRibbonPageCategory1 = new DevExpress.XtraSpreadsheet.UI.PictureToolsRibbonPageCategory();
			this.pictureFormatRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.PictureFormatRibbonPage();
			this.drawingToolsRibbonPageCategory1 = new DevExpress.XtraSpreadsheet.UI.DrawingToolsRibbonPageCategory();
			this.drawingFormatRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.DrawingFormatRibbonPage();
			this.fileRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.FileRibbonPage();
			this.homeRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.HomeRibbonPage();
			this.insertRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.InsertRibbonPage();
			this.pageLayoutRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.PageLayoutRibbonPage();
			this.formulasRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.FormulasRibbonPage();
			this.dataRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.DataRibbonPage();
			this.reviewRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.ReviewRibbonPage();
			this.viewRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.ViewRibbonPage();
			this.repositoryItemFontEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.repositoryItemSpreadsheetFontSizeEdit1 = new DevExpress.XtraSpreadsheet.Design.RepositoryItemSpreadsheetFontSizeEdit();
			this.repositoryItemPopupGalleryEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPopupGalleryEdit();
			this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
			this.commandBarGalleryDropDown2 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown1 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown3 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown4 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown5 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown6 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown7 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown8 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown9 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown10 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown11 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown12 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown13 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown14 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown15 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown16 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown17 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown18 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown19 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown20 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown21 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown22 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown23 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.commandBarGalleryDropDown24 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
			this.spreadsheetFormulaBarControl1 = new DevExpress.XtraSpreadsheet.SpreadsheetFormulaBarControl();
			this.spreadsheetNameBoxControl1 = new DevExpress.XtraSpreadsheet.SpreadsheetNameBoxControl();
			this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
			this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
			this.spreadsheetBarController1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetBarController();
			this.spreadsheetCommandBarButtonItem107 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem108 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem109 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem110 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem111 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem131 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarCheckItem33 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
			this.spreadsheetCommandBarButtonItem132 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem133 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem29 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem134 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem135 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarSubItem30 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
			this.spreadsheetCommandBarButtonItem136 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem137 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem138 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem139 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem140 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem141 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem142 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem143 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.spreadsheetCommandBarButtonItem144 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
			this.barStaticItem2 = new DevExpress.XtraBars.BarStaticItem();
			this.drawingFormatArrangeRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.DrawingFormatArrangeRibbonPageGroup();
			this.pictureFormatArrangeRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PictureFormatArrangeRibbonPageGroup();
			this.tablePropertiesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.TablePropertiesRibbonPageGroup();
			this.barStaticItem3 = new DevExpress.XtraBars.BarStaticItem();
			this.tableToolsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.TableToolsRibbonPageGroup();
			this.tableStyleOptionsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.TableStyleOptionsRibbonPageGroup();
			this.tableStylesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.TableStylesRibbonPageGroup();
			this.chartsDesignTypeRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsDesignTypeRibbonPageGroup();
			this.chartsDesignDataRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsDesignDataRibbonPageGroup();
			this.chartsDesignLayoutsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsDesignLayoutsRibbonPageGroup();
			this.chartsDesignStylesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsDesignStylesRibbonPageGroup();
			this.chartsLayoutLabelsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsLayoutLabelsRibbonPageGroup();
			this.chartsLayoutAxesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsLayoutAxesRibbonPageGroup();
			this.chartsLayoutAnalysisRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsLayoutAnalysisRibbonPageGroup();
			this.chartsFormatArrangeRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsFormatArrangeRibbonPageGroup();
			this.commonRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.CommonRibbonPageGroup();
			this.infoRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.InfoRibbonPageGroup();
			this.clipboardRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ClipboardRibbonPageGroup();
			this.fontRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.FontRibbonPageGroup();
			this.barButtonGroup11 = new DevExpress.XtraBars.BarButtonGroup();
			this.barButtonGroup12 = new DevExpress.XtraBars.BarButtonGroup();
			this.barButtonGroup13 = new DevExpress.XtraBars.BarButtonGroup();
			this.barButtonGroup14 = new DevExpress.XtraBars.BarButtonGroup();
			this.alignmentRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.AlignmentRibbonPageGroup();
			this.barButtonGroup15 = new DevExpress.XtraBars.BarButtonGroup();
			this.barButtonGroup16 = new DevExpress.XtraBars.BarButtonGroup();
			this.barButtonGroup17 = new DevExpress.XtraBars.BarButtonGroup();
			this.numberRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.NumberRibbonPageGroup();
			this.barButtonGroup18 = new DevExpress.XtraBars.BarButtonGroup();
			this.barButtonGroup19 = new DevExpress.XtraBars.BarButtonGroup();
			this.barButtonGroup20 = new DevExpress.XtraBars.BarButtonGroup();
			this.stylesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.StylesRibbonPageGroup();
			this.cellsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.CellsRibbonPageGroup();
			this.editingRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.EditingRibbonPageGroup();
			this.tablesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.TablesRibbonPageGroup();
			this.illustrationsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.IllustrationsRibbonPageGroup();
			this.chartsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsRibbonPageGroup();
			this.linksRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.LinksRibbonPageGroup();
			this.symbolsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.SymbolsRibbonPageGroup();
			this.pageSetupRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PageSetupRibbonPageGroup();
			this.pageSetupShowRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PageSetupShowRibbonPageGroup();
			this.pageSetupPrintRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PageSetupPrintRibbonPageGroup();
			this.arrangeRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ArrangeRibbonPageGroup();
			this.functionLibraryRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.FunctionLibraryRibbonPageGroup();
			this.formulaDefinedNamesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.FormulaDefinedNamesRibbonPageGroup();
			this.formulaAuditingRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.FormulaAuditingRibbonPageGroup();
			this.formulaCalculationRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.FormulaCalculationRibbonPageGroup();
			this.sortAndFilterRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.SortAndFilterRibbonPageGroup();
			this.outlineRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.OutlineRibbonPageGroup();
			this.commentsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.CommentsRibbonPageGroup();
			this.changesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChangesRibbonPageGroup();
			this.showRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ShowRibbonPageGroup();
			this.zoomRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ZoomRibbonPageGroup();
			this.windowRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.WindowRibbonPageGroup();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown26)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpreadsheetFontSizeEdit2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown25)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemPopupGalleryEdit2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown27)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown28)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown29)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown30)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown31)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown32)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown33)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown34)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown35)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown36)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown37)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown38)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown39)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown40)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown41)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown42)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown43)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown44)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown45)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown46)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown47)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown48)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpreadsheetFontSizeEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemPopupGalleryEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown11)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown12)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown13)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown14)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown15)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown16)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown17)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown18)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown19)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown20)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown21)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown22)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown23)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown24)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spreadsheetNameBoxControl1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
			this.splitContainerControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spreadsheetBarController1)).BeginInit();
			this.SuspendLayout();
			this.spreadsheetCommandBarSubItem4.CommandName = "ConditionalFormattingCommandGroup";
			this.spreadsheetCommandBarSubItem4.Id = 328;
			this.spreadsheetCommandBarSubItem4.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarSubItem5),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarSubItem6),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem2),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem3),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarSubItem7)});
			this.spreadsheetCommandBarSubItem4.Name = "spreadsheetCommandBarSubItem4";
			this.spreadsheetCommandBarSubItem5.CommandName = "ConditionalFormattingHighlightCellsRuleCommandGroup";
			this.spreadsheetCommandBarSubItem5.Id = 336;
			this.spreadsheetCommandBarSubItem5.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem43),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem44),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem45),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem46),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem47),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem48),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem49)});
			this.spreadsheetCommandBarSubItem5.Name = "spreadsheetCommandBarSubItem5";
			this.spreadsheetCommandBarButtonItem43.CommandName = "ConditionalFormattingGreaterThanRuleCommand";
			this.spreadsheetCommandBarButtonItem43.Id = 329;
			this.spreadsheetCommandBarButtonItem43.Name = "spreadsheetCommandBarButtonItem43";
			this.spreadsheetCommandBarButtonItem43.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem44.CommandName = "ConditionalFormattingLessThanRuleCommand";
			this.spreadsheetCommandBarButtonItem44.Id = 330;
			this.spreadsheetCommandBarButtonItem44.Name = "spreadsheetCommandBarButtonItem44";
			this.spreadsheetCommandBarButtonItem44.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem45.CommandName = "ConditionalFormattingBetweenRuleCommand";
			this.spreadsheetCommandBarButtonItem45.Id = 331;
			this.spreadsheetCommandBarButtonItem45.Name = "spreadsheetCommandBarButtonItem45";
			this.spreadsheetCommandBarButtonItem45.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem46.CommandName = "ConditionalFormattingEqualToRuleCommand";
			this.spreadsheetCommandBarButtonItem46.Id = 332;
			this.spreadsheetCommandBarButtonItem46.Name = "spreadsheetCommandBarButtonItem46";
			this.spreadsheetCommandBarButtonItem46.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem47.CommandName = "ConditionalFormattingTextContainsRuleCommand";
			this.spreadsheetCommandBarButtonItem47.Id = 333;
			this.spreadsheetCommandBarButtonItem47.Name = "spreadsheetCommandBarButtonItem47";
			this.spreadsheetCommandBarButtonItem47.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem48.CommandName = "ConditionalFormattingDateOccurringRuleCommand";
			this.spreadsheetCommandBarButtonItem48.Id = 334;
			this.spreadsheetCommandBarButtonItem48.Name = "spreadsheetCommandBarButtonItem48";
			this.spreadsheetCommandBarButtonItem48.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem49.CommandName = "ConditionalFormattingDuplicateValuesRuleCommand";
			this.spreadsheetCommandBarButtonItem49.Id = 335;
			this.spreadsheetCommandBarButtonItem49.Name = "spreadsheetCommandBarButtonItem49";
			this.spreadsheetCommandBarButtonItem49.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarSubItem6.CommandName = "ConditionalFormattingTopBottomRuleCommandGroup";
			this.spreadsheetCommandBarSubItem6.Id = 343;
			this.spreadsheetCommandBarSubItem6.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem50),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem51),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem52),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem53),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem54),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem55)});
			this.spreadsheetCommandBarSubItem6.Name = "spreadsheetCommandBarSubItem6";
			this.spreadsheetCommandBarButtonItem50.CommandName = "ConditionalFormattingTop10RuleCommand";
			this.spreadsheetCommandBarButtonItem50.Id = 337;
			this.spreadsheetCommandBarButtonItem50.Name = "spreadsheetCommandBarButtonItem50";
			this.spreadsheetCommandBarButtonItem50.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem51.CommandName = "ConditionalFormattingTop10PercentRuleCommand";
			this.spreadsheetCommandBarButtonItem51.Id = 338;
			this.spreadsheetCommandBarButtonItem51.Name = "spreadsheetCommandBarButtonItem51";
			this.spreadsheetCommandBarButtonItem51.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem52.CommandName = "ConditionalFormattingBottom10RuleCommand";
			this.spreadsheetCommandBarButtonItem52.Id = 339;
			this.spreadsheetCommandBarButtonItem52.Name = "spreadsheetCommandBarButtonItem52";
			this.spreadsheetCommandBarButtonItem52.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem53.CommandName = "ConditionalFormattingBottom10PercentRuleCommand";
			this.spreadsheetCommandBarButtonItem53.Id = 340;
			this.spreadsheetCommandBarButtonItem53.Name = "spreadsheetCommandBarButtonItem53";
			this.spreadsheetCommandBarButtonItem53.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem54.CommandName = "ConditionalFormattingAboveAverageRuleCommand";
			this.spreadsheetCommandBarButtonItem54.Id = 341;
			this.spreadsheetCommandBarButtonItem54.Name = "spreadsheetCommandBarButtonItem54";
			this.spreadsheetCommandBarButtonItem54.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem55.CommandName = "ConditionalFormattingBelowAverageRuleCommand";
			this.spreadsheetCommandBarButtonItem55.Id = 342;
			this.spreadsheetCommandBarButtonItem55.Name = "spreadsheetCommandBarButtonItem55";
			this.spreadsheetCommandBarButtonItem55.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonGalleryDropDownItem1.CommandName = "ConditionalFormattingDataBarsCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem1.DropDownControl = this.commandBarGalleryDropDown26;
			this.spreadsheetCommandBarButtonGalleryDropDownItem1.Id = 344;
			this.spreadsheetCommandBarButtonGalleryDropDownItem1.Name = "spreadsheetCommandBarButtonGalleryDropDownItem1";
			this.spreadsheetCommandBarButtonGalleryDropDownItem1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.commandBarGalleryDropDown26.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup1.CommandName = "ConditionalFormattingDataBarsGradientFillCommandGroup";
			spreadsheetCommandGalleryItem1.CommandName = "ConditionalFormattingDataBarGradientBlue";
			spreadsheetCommandGalleryItem2.CommandName = "ConditionalFormattingDataBarGradientGreen";
			spreadsheetCommandGalleryItem3.CommandName = "ConditionalFormattingDataBarGradientRed";
			spreadsheetCommandGalleryItem4.CommandName = "ConditionalFormattingDataBarGradientOrange";
			spreadsheetCommandGalleryItem5.CommandName = "ConditionalFormattingDataBarGradientLightBlue";
			spreadsheetCommandGalleryItem6.CommandName = "ConditionalFormattingDataBarGradientPurple";
			spreadsheetCommandGalleryItemGroup1.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem1,
			spreadsheetCommandGalleryItem2,
			spreadsheetCommandGalleryItem3,
			spreadsheetCommandGalleryItem4,
			spreadsheetCommandGalleryItem5,
			spreadsheetCommandGalleryItem6});
			spreadsheetCommandGalleryItemGroup2.CommandName = "ConditionalFormattingDataBarsSolidFillCommandGroup";
			spreadsheetCommandGalleryItem7.CommandName = "ConditionalFormattingDataBarSolidBlue";
			spreadsheetCommandGalleryItem8.CommandName = "ConditionalFormattingDataBarSolidGreen";
			spreadsheetCommandGalleryItem9.CommandName = "ConditionalFormattingDataBarSolidRed";
			spreadsheetCommandGalleryItem10.CommandName = "ConditionalFormattingDataBarSolidOrange";
			spreadsheetCommandGalleryItem11.CommandName = "ConditionalFormattingDataBarSolidLightBlue";
			spreadsheetCommandGalleryItem12.CommandName = "ConditionalFormattingDataBarSolidPurple";
			spreadsheetCommandGalleryItemGroup2.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem7,
			spreadsheetCommandGalleryItem8,
			spreadsheetCommandGalleryItem9,
			spreadsheetCommandGalleryItem10,
			spreadsheetCommandGalleryItem11,
			spreadsheetCommandGalleryItem12});
			this.commandBarGalleryDropDown26.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup1,
			spreadsheetCommandGalleryItemGroup2});
			this.commandBarGalleryDropDown26.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown26.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown26.Name = "commandBarGalleryDropDown26";
			this.commandBarGalleryDropDown26.Ribbon = this.ribbonControl1;
			this.ribbonControl1.ExpandCollapseItem.Id = 0;
			this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.ribbonControl1.ExpandCollapseItem,
			this.spreadsheetCommandBarButtonItem1,
			this.spreadsheetCommandBarButtonItem2,
			this.spreadsheetCommandBarButtonItem3,
			this.spreadsheetCommandBarButtonItem4,
			this.spreadsheetCommandBarButtonItem5,
			this.spreadsheetCommandBarButtonItem6,
			this.spreadsheetCommandBarButtonItem7,
			this.spreadsheetCommandBarButtonItem8,
			this.spreadsheetCommandBarButtonItem9,
			this.spreadsheetCommandBarButtonItem10,
			this.spreadsheetCommandBarButtonItem11,
			this.spreadsheetCommandBarButtonItem12,
			this.spreadsheetCommandBarButtonItem13,
			this.barButtonGroup1,
			this.changeFontNameItem1,
			this.changeFontSizeItem1,
			this.spreadsheetCommandBarButtonItem14,
			this.spreadsheetCommandBarButtonItem15,
			this.barButtonGroup2,
			this.spreadsheetCommandBarCheckItem1,
			this.spreadsheetCommandBarCheckItem2,
			this.spreadsheetCommandBarCheckItem3,
			this.spreadsheetCommandBarCheckItem4,
			this.barButtonGroup3,
			this.spreadsheetCommandBarSubItem1,
			this.spreadsheetCommandBarButtonItem16,
			this.spreadsheetCommandBarButtonItem17,
			this.spreadsheetCommandBarButtonItem18,
			this.spreadsheetCommandBarButtonItem19,
			this.spreadsheetCommandBarButtonItem20,
			this.spreadsheetCommandBarButtonItem21,
			this.spreadsheetCommandBarButtonItem22,
			this.spreadsheetCommandBarButtonItem23,
			this.spreadsheetCommandBarButtonItem24,
			this.spreadsheetCommandBarButtonItem25,
			this.spreadsheetCommandBarButtonItem26,
			this.spreadsheetCommandBarButtonItem27,
			this.spreadsheetCommandBarButtonItem28,
			this.changeBorderLineColorItem1,
			this.changeBorderLineStyleItem1,
			this.barButtonGroup4,
			this.changeCellFillColorItem1,
			this.changeFontColorItem1,
			this.barButtonGroup5,
			this.spreadsheetCommandBarCheckItem5,
			this.spreadsheetCommandBarCheckItem6,
			this.spreadsheetCommandBarCheckItem7,
			this.barButtonGroup6,
			this.spreadsheetCommandBarCheckItem8,
			this.spreadsheetCommandBarCheckItem9,
			this.spreadsheetCommandBarCheckItem10,
			this.barButtonGroup7,
			this.spreadsheetCommandBarButtonItem29,
			this.spreadsheetCommandBarButtonItem30,
			this.spreadsheetCommandBarCheckItem11,
			this.spreadsheetCommandBarSubItem2,
			this.spreadsheetCommandBarCheckItem12,
			this.spreadsheetCommandBarButtonItem31,
			this.spreadsheetCommandBarButtonItem32,
			this.spreadsheetCommandBarButtonItem33,
			this.barButtonGroup8,
			this.changeNumberFormatItem1,
			this.barButtonGroup9,
			this.spreadsheetCommandBarSubItem3,
			this.spreadsheetCommandBarButtonItem34,
			this.spreadsheetCommandBarButtonItem35,
			this.spreadsheetCommandBarButtonItem36,
			this.spreadsheetCommandBarButtonItem37,
			this.spreadsheetCommandBarButtonItem38,
			this.spreadsheetCommandBarButtonItem39,
			this.spreadsheetCommandBarButtonItem40,
			this.barButtonGroup10,
			this.spreadsheetCommandBarButtonItem41,
			this.spreadsheetCommandBarButtonItem42,
			this.spreadsheetCommandBarSubItem4,
			this.spreadsheetCommandBarButtonItem43,
			this.spreadsheetCommandBarButtonItem44,
			this.spreadsheetCommandBarButtonItem45,
			this.spreadsheetCommandBarButtonItem46,
			this.spreadsheetCommandBarButtonItem47,
			this.spreadsheetCommandBarButtonItem48,
			this.spreadsheetCommandBarButtonItem49,
			this.spreadsheetCommandBarSubItem5,
			this.spreadsheetCommandBarButtonItem50,
			this.spreadsheetCommandBarButtonItem51,
			this.spreadsheetCommandBarButtonItem52,
			this.spreadsheetCommandBarButtonItem53,
			this.spreadsheetCommandBarButtonItem54,
			this.spreadsheetCommandBarButtonItem55,
			this.spreadsheetCommandBarSubItem6,
			this.spreadsheetCommandBarButtonGalleryDropDownItem1,
			this.spreadsheetCommandBarButtonGalleryDropDownItem2,
			this.spreadsheetCommandBarButtonGalleryDropDownItem3,
			this.spreadsheetCommandBarButtonItem56,
			this.spreadsheetCommandBarButtonItem57,
			this.spreadsheetCommandBarSubItem7,
			this.galleryFormatAsTableItem1,
			this.galleryChangeStyleItem1,
			this.spreadsheetCommandBarSubItem8,
			this.spreadsheetCommandBarButtonItem58,
			this.spreadsheetCommandBarButtonItem59,
			this.spreadsheetCommandBarButtonItem60,
			this.spreadsheetCommandBarSubItem9,
			this.spreadsheetCommandBarButtonItem61,
			this.spreadsheetCommandBarButtonItem62,
			this.spreadsheetCommandBarButtonItem63,
			this.spreadsheetCommandBarSubItem10,
			this.spreadsheetCommandBarButtonItem64,
			this.spreadsheetCommandBarButtonItem65,
			this.spreadsheetCommandBarButtonItem66,
			this.spreadsheetCommandBarButtonItem67,
			this.spreadsheetCommandBarButtonItem68,
			this.spreadsheetCommandBarButtonItem69,
			this.spreadsheetCommandBarButtonItem70,
			this.spreadsheetCommandBarButtonItem71,
			this.spreadsheetCommandBarButtonItem72,
			this.spreadsheetCommandBarButtonItem73,
			this.spreadsheetCommandBarButtonItem74,
			this.spreadsheetCommandBarSubItem11,
			this.spreadsheetCommandBarButtonItem75,
			this.changeSheetTabColorItem1,
			this.spreadsheetCommandBarButtonItem76,
			this.spreadsheetCommandBarCheckItem13,
			this.spreadsheetCommandBarButtonItem77,
			this.spreadsheetCommandBarSubItem12,
			this.spreadsheetCommandBarButtonItem78,
			this.spreadsheetCommandBarButtonItem79,
			this.spreadsheetCommandBarButtonItem80,
			this.spreadsheetCommandBarButtonItem81,
			this.spreadsheetCommandBarButtonItem82,
			this.spreadsheetCommandBarSubItem13,
			this.spreadsheetCommandBarButtonItem83,
			this.spreadsheetCommandBarButtonItem84,
			this.spreadsheetCommandBarButtonItem85,
			this.spreadsheetCommandBarButtonItem86,
			this.spreadsheetCommandBarSubItem14,
			this.spreadsheetCommandBarButtonItem87,
			this.spreadsheetCommandBarButtonItem88,
			this.spreadsheetCommandBarButtonItem89,
			this.spreadsheetCommandBarButtonItem90,
			this.spreadsheetCommandBarButtonItem91,
			this.spreadsheetCommandBarButtonItem92,
			this.spreadsheetCommandBarSubItem15,
			this.spreadsheetCommandBarButtonItem93,
			this.spreadsheetCommandBarButtonItem94,
			this.spreadsheetCommandBarSubItem16,
			this.spreadsheetCommandBarButtonItem95,
			this.spreadsheetCommandBarButtonItem96,
			this.spreadsheetCommandBarButtonItem97,
			this.spreadsheetCommandBarButtonItem98,
			this.spreadsheetCommandBarButtonGalleryDropDownItem4,
			this.spreadsheetCommandBarButtonGalleryDropDownItem5,
			this.spreadsheetCommandBarButtonGalleryDropDownItem6,
			this.spreadsheetCommandBarButtonGalleryDropDownItem7,
			this.spreadsheetCommandBarButtonGalleryDropDownItem8,
			this.spreadsheetCommandBarButtonGalleryDropDownItem9,
			this.spreadsheetCommandBarButtonGalleryDropDownItem10,
			this.spreadsheetCommandBarButtonItem99,
			this.spreadsheetCommandBarButtonItem100,
			this.spreadsheetCommandBarSubItem17,
			this.spreadsheetCommandBarCheckItem14,
			this.spreadsheetCommandBarCheckItem15,
			this.spreadsheetCommandBarCheckItem16,
			this.spreadsheetCommandBarSubItem18,
			this.spreadsheetCommandBarCheckItem17,
			this.spreadsheetCommandBarCheckItem18,
			this.pageSetupPaperKindItem1,
			this.spreadsheetCommandBarSubItem19,
			this.spreadsheetCommandBarButtonItem101,
			this.spreadsheetCommandBarButtonItem102,
			this.spreadsheetCommandBarCheckItem19,
			this.spreadsheetCommandBarCheckItem20,
			this.spreadsheetCommandBarCheckItem21,
			this.spreadsheetCommandBarCheckItem22,
			this.spreadsheetCommandBarSubItem20,
			this.spreadsheetCommandBarButtonItem103,
			this.spreadsheetCommandBarButtonItem104,
			this.spreadsheetCommandBarSubItem21,
			this.spreadsheetCommandBarButtonItem105,
			this.spreadsheetCommandBarButtonItem106,
			this.spreadsheetCommandBarSubItem22,
			this.functionsFinancialItem1,
			this.functionsLogicalItem1,
			this.functionsTextItem1,
			this.functionsDateAndTimeItem1,
			this.functionsLookupAndReferenceItem1,
			this.functionsMathAndTrigonometryItem1,
			this.spreadsheetCommandBarSubItem23,
			this.functionsStatisticalItem1,
			this.functionsEngineeringItem1,
			this.functionsInformationItem1,
			this.functionsCompatibilityItem1,
			this.functionsWebItem1,
			this.spreadsheetCommandBarButtonItem112,
			this.spreadsheetCommandBarButtonItem113,
			this.definedNameListItem1,
			this.spreadsheetCommandBarButtonItem114,
			this.spreadsheetCommandBarCheckItem23,
			this.spreadsheetCommandBarSubItem24,
			this.spreadsheetCommandBarCheckItem24,
			this.spreadsheetCommandBarCheckItem25,
			this.spreadsheetCommandBarButtonItem115,
			this.spreadsheetCommandBarButtonItem116,
			this.spreadsheetCommandBarButtonItem117,
			this.spreadsheetCommandBarButtonItem118,
			this.spreadsheetCommandBarButtonItem119,
			this.spreadsheetCommandBarButtonItem120,
			this.spreadsheetCommandBarButtonItem121,
			this.spreadsheetCommandBarButtonItem122,
			this.spreadsheetCommandBarButtonItem123,
			this.spreadsheetCommandBarSubItem25,
			this.spreadsheetCommandBarButtonItem124,
			this.spreadsheetCommandBarButtonItem125,
			this.spreadsheetCommandBarButtonItem126,
			this.spreadsheetCommandBarButtonItem127,
			this.spreadsheetCommandBarButtonItem128,
			this.spreadsheetCommandBarButtonItem129,
			this.spreadsheetCommandBarButtonItem130,
			this.galleryChartLayoutItem1,
			this.galleryChartStyleItem1,
			this.spreadsheetCommandBarButtonGalleryDropDownItem11,
			this.spreadsheetCommandBarSubItem26,
			this.spreadsheetCommandBarButtonGalleryDropDownItem12,
			this.spreadsheetCommandBarButtonGalleryDropDownItem13,
			this.spreadsheetCommandBarButtonGalleryDropDownItem14,
			this.spreadsheetCommandBarButtonGalleryDropDownItem15,
			this.spreadsheetCommandBarSubItem27,
			this.spreadsheetCommandBarButtonGalleryDropDownItem16,
			this.spreadsheetCommandBarButtonGalleryDropDownItem17,
			this.spreadsheetCommandBarSubItem28,
			this.spreadsheetCommandBarButtonGalleryDropDownItem18,
			this.spreadsheetCommandBarButtonGalleryDropDownItem19,
			this.spreadsheetCommandBarButtonGalleryDropDownItem20,
			this.spreadsheetCommandBarButtonGalleryDropDownItem21,
			this.spreadsheetCommandBarButtonGalleryDropDownItem22,
			this.barStaticItem1,
			this.renameTableItem1,
			this.spreadsheetCommandBarCheckItem26,
			this.spreadsheetCommandBarCheckItem27,
			this.spreadsheetCommandBarCheckItem28,
			this.spreadsheetCommandBarCheckItem29,
			this.spreadsheetCommandBarCheckItem30,
			this.spreadsheetCommandBarCheckItem31,
			this.spreadsheetCommandBarCheckItem32,
			this.galleryTableStylesItem1,
			this.spreadsheetCommandBarButtonItem131,
			this.spreadsheetCommandBarCheckItem33,
			this.spreadsheetCommandBarButtonItem132,
			this.spreadsheetCommandBarButtonItem133,
			this.spreadsheetCommandBarSubItem29,
			this.spreadsheetCommandBarButtonItem134,
			this.spreadsheetCommandBarButtonItem135,
			this.spreadsheetCommandBarSubItem30,
			this.spreadsheetCommandBarButtonItem136,
			this.spreadsheetCommandBarButtonItem137,
			this.spreadsheetCommandBarButtonItem138,
			this.spreadsheetCommandBarButtonItem139,
			this.spreadsheetCommandBarButtonItem140,
			this.spreadsheetCommandBarButtonItem141,
			this.spreadsheetCommandBarButtonItem142,
			this.spreadsheetCommandBarButtonItem143,
			this.spreadsheetCommandBarButtonItem144,
			this.barStaticItem2,
			this.barStaticItem3,
			this.barButtonGroup11,
			this.barButtonGroup12,
			this.barButtonGroup13,
			this.barButtonGroup14,
			this.barButtonGroup15,
			this.barButtonGroup16,
			this.barButtonGroup17,
			this.barButtonGroup18,
			this.barButtonGroup19,
			this.barButtonGroup20});
			this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
			this.ribbonControl1.MaxItemId = 530;
			this.ribbonControl1.Name = "ribbonControl1";
			this.ribbonControl1.PageCategories.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageCategory[] {
			this.chartToolsRibbonPageCategory1,
			this.tableToolsRibbonPageCategory1,
			this.pictureToolsRibbonPageCategory1,
			this.drawingToolsRibbonPageCategory1});
			this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
			this.fileRibbonPage1,
			this.homeRibbonPage1,
			this.insertRibbonPage1,
			this.pageLayoutRibbonPage1,
			this.formulasRibbonPage1,
			this.dataRibbonPage1,
			this.reviewRibbonPage1,
			this.viewRibbonPage1});
			this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repositoryItemFontEdit1,
			this.repositoryItemSpreadsheetFontSizeEdit1,
			this.repositoryItemPopupGalleryEdit1,
			this.repositoryItemTextEdit1,
			this.repositoryItemFontEdit2,
			this.repositoryItemSpreadsheetFontSizeEdit2,
			this.repositoryItemPopupGalleryEdit2,
			this.repositoryItemTextEdit2});
			this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013;
			this.ribbonControl1.ShowFullScreenButton = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonControl1.Size = new System.Drawing.Size(916, 144);
			this.spreadsheetCommandBarButtonItem1.CommandName = "FileNew";
			this.spreadsheetCommandBarButtonItem1.Id = 254;
			this.spreadsheetCommandBarButtonItem1.Name = "spreadsheetCommandBarButtonItem1";
			this.spreadsheetCommandBarButtonItem2.CommandName = "FileOpen";
			this.spreadsheetCommandBarButtonItem2.Id = 255;
			this.spreadsheetCommandBarButtonItem2.Name = "spreadsheetCommandBarButtonItem2";
			this.spreadsheetCommandBarButtonItem3.CommandName = "FileSave";
			this.spreadsheetCommandBarButtonItem3.Id = 256;
			this.spreadsheetCommandBarButtonItem3.Name = "spreadsheetCommandBarButtonItem3";
			this.spreadsheetCommandBarButtonItem4.CommandName = "FileSaveAs";
			this.spreadsheetCommandBarButtonItem4.Id = 257;
			this.spreadsheetCommandBarButtonItem4.Name = "spreadsheetCommandBarButtonItem4";
			this.spreadsheetCommandBarButtonItem5.CommandName = "FileQuickPrint";
			this.spreadsheetCommandBarButtonItem5.Id = 258;
			this.spreadsheetCommandBarButtonItem5.Name = "spreadsheetCommandBarButtonItem5";
			this.spreadsheetCommandBarButtonItem6.CommandName = "FilePrint";
			this.spreadsheetCommandBarButtonItem6.Id = 259;
			this.spreadsheetCommandBarButtonItem6.Name = "spreadsheetCommandBarButtonItem6";
			this.spreadsheetCommandBarButtonItem7.CommandName = "FilePrintPreview";
			this.spreadsheetCommandBarButtonItem7.Id = 260;
			this.spreadsheetCommandBarButtonItem7.Name = "spreadsheetCommandBarButtonItem7";
			this.spreadsheetCommandBarButtonItem8.CommandName = "FileUndo";
			this.spreadsheetCommandBarButtonItem8.Id = 261;
			this.spreadsheetCommandBarButtonItem8.Name = "spreadsheetCommandBarButtonItem8";
			this.spreadsheetCommandBarButtonItem9.CommandName = "FileRedo";
			this.spreadsheetCommandBarButtonItem9.Id = 262;
			this.spreadsheetCommandBarButtonItem9.Name = "spreadsheetCommandBarButtonItem9";
			this.spreadsheetCommandBarButtonItem10.CommandName = "PasteSelection";
			this.spreadsheetCommandBarButtonItem10.Id = 273;
			this.spreadsheetCommandBarButtonItem10.Name = "spreadsheetCommandBarButtonItem10";
			this.spreadsheetCommandBarButtonItem10.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem11.CommandName = "CutSelection";
			this.spreadsheetCommandBarButtonItem11.Id = 274;
			this.spreadsheetCommandBarButtonItem11.Name = "spreadsheetCommandBarButtonItem11";
			this.spreadsheetCommandBarButtonItem11.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.spreadsheetCommandBarButtonItem12.CommandName = "CopySelection";
			this.spreadsheetCommandBarButtonItem12.Id = 275;
			this.spreadsheetCommandBarButtonItem12.Name = "spreadsheetCommandBarButtonItem12";
			this.spreadsheetCommandBarButtonItem12.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.spreadsheetCommandBarButtonItem13.CommandName = "ShowPasteSpecialForm";
			this.spreadsheetCommandBarButtonItem13.Id = 276;
			this.spreadsheetCommandBarButtonItem13.Name = "spreadsheetCommandBarButtonItem13";
			this.spreadsheetCommandBarButtonItem13.RibbonStyle = ((DevExpress.XtraBars.Ribbon.RibbonItemStyles)((DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText | DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText)));
			this.barButtonGroup1.Id = 263;
			this.barButtonGroup1.ItemLinks.Add(this.changeFontNameItem1);
			this.barButtonGroup1.ItemLinks.Add(this.changeFontSizeItem1);
			this.barButtonGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem14);
			this.barButtonGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem15);
			this.barButtonGroup1.Name = "barButtonGroup1";
			this.barButtonGroup1.Tag = "{B0CA3FA8-82D6-4BC4-BD31-D9AE56C1D033}";
			this.changeFontNameItem1.Edit = this.repositoryItemFontEdit2;
			this.changeFontNameItem1.Id = 277;
			this.changeFontNameItem1.Name = "changeFontNameItem1";
			this.repositoryItemFontEdit2.AutoHeight = false;
			this.repositoryItemFontEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemFontEdit2.Name = "repositoryItemFontEdit2";
			this.changeFontSizeItem1.Edit = this.repositoryItemSpreadsheetFontSizeEdit2;
			this.changeFontSizeItem1.Id = 278;
			this.changeFontSizeItem1.Name = "changeFontSizeItem1";
			this.repositoryItemSpreadsheetFontSizeEdit2.AutoHeight = false;
			this.repositoryItemSpreadsheetFontSizeEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemSpreadsheetFontSizeEdit2.Control = this.spreadsheetControl1;
			this.repositoryItemSpreadsheetFontSizeEdit2.Name = "repositoryItemSpreadsheetFontSizeEdit2";
			this.spreadsheetControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.spreadsheetControl1.Location = new System.Drawing.Point(0, 169);
			this.spreadsheetControl1.MenuManager = this.ribbonControl1;
			this.spreadsheetControl1.Name = "spreadsheetControl1";
			this.spreadsheetControl1.Size = new System.Drawing.Size(916, 405);
			this.spreadsheetControl1.TabIndex = 0;
			this.spreadsheetControl1.Text = "spreadsheetControl1";
			this.spreadsheetCommandBarButtonItem14.ButtonGroupTag = "{B0CA3FA8-82D6-4BC4-BD31-D9AE56C1D033}";
			this.spreadsheetCommandBarButtonItem14.CommandName = "FormatIncreaseFontSize";
			this.spreadsheetCommandBarButtonItem14.Id = 279;
			this.spreadsheetCommandBarButtonItem14.Name = "spreadsheetCommandBarButtonItem14";
			this.spreadsheetCommandBarButtonItem14.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarButtonItem15.ButtonGroupTag = "{B0CA3FA8-82D6-4BC4-BD31-D9AE56C1D033}";
			this.spreadsheetCommandBarButtonItem15.CommandName = "FormatDecreaseFontSize";
			this.spreadsheetCommandBarButtonItem15.Id = 280;
			this.spreadsheetCommandBarButtonItem15.Name = "spreadsheetCommandBarButtonItem15";
			this.spreadsheetCommandBarButtonItem15.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.barButtonGroup2.Id = 264;
			this.barButtonGroup2.ItemLinks.Add(this.spreadsheetCommandBarCheckItem1);
			this.barButtonGroup2.ItemLinks.Add(this.spreadsheetCommandBarCheckItem2);
			this.barButtonGroup2.ItemLinks.Add(this.spreadsheetCommandBarCheckItem3);
			this.barButtonGroup2.ItemLinks.Add(this.spreadsheetCommandBarCheckItem4);
			this.barButtonGroup2.Name = "barButtonGroup2";
			this.barButtonGroup2.Tag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
			this.spreadsheetCommandBarCheckItem1.ButtonGroupTag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
			this.spreadsheetCommandBarCheckItem1.CommandName = "FormatFontBold";
			this.spreadsheetCommandBarCheckItem1.Id = 281;
			this.spreadsheetCommandBarCheckItem1.Name = "spreadsheetCommandBarCheckItem1";
			this.spreadsheetCommandBarCheckItem1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarCheckItem2.ButtonGroupTag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
			this.spreadsheetCommandBarCheckItem2.CommandName = "FormatFontItalic";
			this.spreadsheetCommandBarCheckItem2.Id = 282;
			this.spreadsheetCommandBarCheckItem2.Name = "spreadsheetCommandBarCheckItem2";
			this.spreadsheetCommandBarCheckItem2.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarCheckItem3.ButtonGroupTag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
			this.spreadsheetCommandBarCheckItem3.CommandName = "FormatFontUnderline";
			this.spreadsheetCommandBarCheckItem3.Id = 283;
			this.spreadsheetCommandBarCheckItem3.Name = "spreadsheetCommandBarCheckItem3";
			this.spreadsheetCommandBarCheckItem3.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarCheckItem4.ButtonGroupTag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
			this.spreadsheetCommandBarCheckItem4.CommandName = "FormatFontStrikeout";
			this.spreadsheetCommandBarCheckItem4.Id = 284;
			this.spreadsheetCommandBarCheckItem4.Name = "spreadsheetCommandBarCheckItem4";
			this.spreadsheetCommandBarCheckItem4.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.barButtonGroup3.Id = 265;
			this.barButtonGroup3.ItemLinks.Add(this.spreadsheetCommandBarSubItem1);
			this.barButtonGroup3.Name = "barButtonGroup3";
			this.barButtonGroup3.Tag = "{DDB05A32-9207-4556-85CB-FE3403A197C7}";
			this.spreadsheetCommandBarSubItem1.ButtonGroupTag = "{DDB05A32-9207-4556-85CB-FE3403A197C7}";
			this.spreadsheetCommandBarSubItem1.CommandName = "FormatBordersCommandGroup";
			this.spreadsheetCommandBarSubItem1.Id = 285;
			this.spreadsheetCommandBarSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem16),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem17),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem18),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem19),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem20),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem21),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem22),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem23),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem24),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem25),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem26),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem27),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem28),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeBorderLineColorItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeBorderLineStyleItem1)});
			this.spreadsheetCommandBarSubItem1.Name = "spreadsheetCommandBarSubItem1";
			this.spreadsheetCommandBarSubItem1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarButtonItem16.CommandName = "FormatBottomBorder";
			this.spreadsheetCommandBarButtonItem16.Id = 286;
			this.spreadsheetCommandBarButtonItem16.Name = "spreadsheetCommandBarButtonItem16";
			this.spreadsheetCommandBarButtonItem17.CommandName = "FormatTopBorder";
			this.spreadsheetCommandBarButtonItem17.Id = 287;
			this.spreadsheetCommandBarButtonItem17.Name = "spreadsheetCommandBarButtonItem17";
			this.spreadsheetCommandBarButtonItem18.CommandName = "FormatLeftBorder";
			this.spreadsheetCommandBarButtonItem18.Id = 288;
			this.spreadsheetCommandBarButtonItem18.Name = "spreadsheetCommandBarButtonItem18";
			this.spreadsheetCommandBarButtonItem19.CommandName = "FormatRightBorder";
			this.spreadsheetCommandBarButtonItem19.Id = 289;
			this.spreadsheetCommandBarButtonItem19.Name = "spreadsheetCommandBarButtonItem19";
			this.spreadsheetCommandBarButtonItem20.CommandName = "FormatNoBorders";
			this.spreadsheetCommandBarButtonItem20.Id = 290;
			this.spreadsheetCommandBarButtonItem20.Name = "spreadsheetCommandBarButtonItem20";
			this.spreadsheetCommandBarButtonItem21.CommandName = "FormatAllBorders";
			this.spreadsheetCommandBarButtonItem21.Id = 291;
			this.spreadsheetCommandBarButtonItem21.Name = "spreadsheetCommandBarButtonItem21";
			this.spreadsheetCommandBarButtonItem22.CommandName = "FormatOutsideBorders";
			this.spreadsheetCommandBarButtonItem22.Id = 292;
			this.spreadsheetCommandBarButtonItem22.Name = "spreadsheetCommandBarButtonItem22";
			this.spreadsheetCommandBarButtonItem23.CommandName = "FormatThickBorder";
			this.spreadsheetCommandBarButtonItem23.Id = 293;
			this.spreadsheetCommandBarButtonItem23.Name = "spreadsheetCommandBarButtonItem23";
			this.spreadsheetCommandBarButtonItem24.CommandName = "FormatBottomDoubleBorder";
			this.spreadsheetCommandBarButtonItem24.Id = 294;
			this.spreadsheetCommandBarButtonItem24.Name = "spreadsheetCommandBarButtonItem24";
			this.spreadsheetCommandBarButtonItem25.CommandName = "FormatBottomThickBorder";
			this.spreadsheetCommandBarButtonItem25.Id = 295;
			this.spreadsheetCommandBarButtonItem25.Name = "spreadsheetCommandBarButtonItem25";
			this.spreadsheetCommandBarButtonItem26.CommandName = "FormatTopAndBottomBorder";
			this.spreadsheetCommandBarButtonItem26.Id = 296;
			this.spreadsheetCommandBarButtonItem26.Name = "spreadsheetCommandBarButtonItem26";
			this.spreadsheetCommandBarButtonItem27.CommandName = "FormatTopAndThickBottomBorder";
			this.spreadsheetCommandBarButtonItem27.Id = 297;
			this.spreadsheetCommandBarButtonItem27.Name = "spreadsheetCommandBarButtonItem27";
			this.spreadsheetCommandBarButtonItem28.CommandName = "FormatTopAndDoubleBottomBorder";
			this.spreadsheetCommandBarButtonItem28.Id = 298;
			this.spreadsheetCommandBarButtonItem28.Name = "spreadsheetCommandBarButtonItem28";
			this.changeBorderLineColorItem1.ActAsDropDown = true;
			this.changeBorderLineColorItem1.Id = 299;
			this.changeBorderLineColorItem1.Name = "changeBorderLineColorItem1";
			this.changeBorderLineStyleItem1.DropDownControl = this.commandBarGalleryDropDown25;
			this.changeBorderLineStyleItem1.Id = 300;
			this.changeBorderLineStyleItem1.Name = "changeBorderLineStyleItem1";
			this.commandBarGalleryDropDown25.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown25.Gallery.ColumnCount = 1;
			this.commandBarGalleryDropDown25.Gallery.DrawImageBackground = false;
			this.commandBarGalleryDropDown25.Gallery.ImageSize = new System.Drawing.Size(65, 46);
			this.commandBarGalleryDropDown25.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
			this.commandBarGalleryDropDown25.Gallery.ItemSize = new System.Drawing.Size(136, 26);
			this.commandBarGalleryDropDown25.Gallery.RowCount = 14;
			this.commandBarGalleryDropDown25.Gallery.ShowGroupCaption = false;
			this.commandBarGalleryDropDown25.Gallery.ShowItemText = true;
			this.commandBarGalleryDropDown25.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown25.Name = "commandBarGalleryDropDown25";
			this.commandBarGalleryDropDown25.Ribbon = this.ribbonControl1;
			this.barButtonGroup4.Id = 266;
			this.barButtonGroup4.ItemLinks.Add(this.changeCellFillColorItem1);
			this.barButtonGroup4.ItemLinks.Add(this.changeFontColorItem1);
			this.barButtonGroup4.Name = "barButtonGroup4";
			this.barButtonGroup4.Tag = "{C2275623-04A3-41E8-8D6A-EB5C7F8541D1}";
			this.changeCellFillColorItem1.Id = 301;
			this.changeCellFillColorItem1.Name = "changeCellFillColorItem1";
			this.changeFontColorItem1.Id = 302;
			this.changeFontColorItem1.Name = "changeFontColorItem1";
			this.barButtonGroup5.Id = 267;
			this.barButtonGroup5.ItemLinks.Add(this.spreadsheetCommandBarCheckItem5);
			this.barButtonGroup5.ItemLinks.Add(this.spreadsheetCommandBarCheckItem6);
			this.barButtonGroup5.ItemLinks.Add(this.spreadsheetCommandBarCheckItem7);
			this.barButtonGroup5.Name = "barButtonGroup5";
			this.barButtonGroup5.Tag = "{03A0322B-12A2-4434-A487-8B5AAF64CCFC}";
			this.spreadsheetCommandBarCheckItem5.ButtonGroupTag = "{03A0322B-12A2-4434-A487-8B5AAF64CCFC}";
			this.spreadsheetCommandBarCheckItem5.CommandName = "FormatAlignmentTop";
			this.spreadsheetCommandBarCheckItem5.Id = 303;
			this.spreadsheetCommandBarCheckItem5.Name = "spreadsheetCommandBarCheckItem5";
			this.spreadsheetCommandBarCheckItem5.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarCheckItem6.ButtonGroupTag = "{03A0322B-12A2-4434-A487-8B5AAF64CCFC}";
			this.spreadsheetCommandBarCheckItem6.CommandName = "FormatAlignmentMiddle";
			this.spreadsheetCommandBarCheckItem6.Id = 304;
			this.spreadsheetCommandBarCheckItem6.Name = "spreadsheetCommandBarCheckItem6";
			this.spreadsheetCommandBarCheckItem6.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarCheckItem7.ButtonGroupTag = "{03A0322B-12A2-4434-A487-8B5AAF64CCFC}";
			this.spreadsheetCommandBarCheckItem7.CommandName = "FormatAlignmentBottom";
			this.spreadsheetCommandBarCheckItem7.Id = 305;
			this.spreadsheetCommandBarCheckItem7.Name = "spreadsheetCommandBarCheckItem7";
			this.spreadsheetCommandBarCheckItem7.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.barButtonGroup6.Id = 268;
			this.barButtonGroup6.ItemLinks.Add(this.spreadsheetCommandBarCheckItem8);
			this.barButtonGroup6.ItemLinks.Add(this.spreadsheetCommandBarCheckItem9);
			this.barButtonGroup6.ItemLinks.Add(this.spreadsheetCommandBarCheckItem10);
			this.barButtonGroup6.Name = "barButtonGroup6";
			this.barButtonGroup6.Tag = "{ECC693B7-EF59-4007-A0DB-A9550214A0F2}";
			this.spreadsheetCommandBarCheckItem8.ButtonGroupTag = "{ECC693B7-EF59-4007-A0DB-A9550214A0F2}";
			this.spreadsheetCommandBarCheckItem8.CommandName = "FormatAlignmentLeft";
			this.spreadsheetCommandBarCheckItem8.Id = 306;
			this.spreadsheetCommandBarCheckItem8.Name = "spreadsheetCommandBarCheckItem8";
			this.spreadsheetCommandBarCheckItem8.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarCheckItem9.ButtonGroupTag = "{ECC693B7-EF59-4007-A0DB-A9550214A0F2}";
			this.spreadsheetCommandBarCheckItem9.CommandName = "FormatAlignmentCenter";
			this.spreadsheetCommandBarCheckItem9.Id = 307;
			this.spreadsheetCommandBarCheckItem9.Name = "spreadsheetCommandBarCheckItem9";
			this.spreadsheetCommandBarCheckItem9.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarCheckItem10.ButtonGroupTag = "{ECC693B7-EF59-4007-A0DB-A9550214A0F2}";
			this.spreadsheetCommandBarCheckItem10.CommandName = "FormatAlignmentRight";
			this.spreadsheetCommandBarCheckItem10.Id = 308;
			this.spreadsheetCommandBarCheckItem10.Name = "spreadsheetCommandBarCheckItem10";
			this.spreadsheetCommandBarCheckItem10.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.barButtonGroup7.Id = 269;
			this.barButtonGroup7.ItemLinks.Add(this.spreadsheetCommandBarButtonItem29);
			this.barButtonGroup7.ItemLinks.Add(this.spreadsheetCommandBarButtonItem30);
			this.barButtonGroup7.Name = "barButtonGroup7";
			this.barButtonGroup7.Tag = "{A5E37DED-106E-44FC-8044-CE3824C08225}";
			this.spreadsheetCommandBarButtonItem29.ButtonGroupTag = "{A5E37DED-106E-44FC-8044-CE3824C08225}";
			this.spreadsheetCommandBarButtonItem29.CommandName = "FormatDecreaseIndent";
			this.spreadsheetCommandBarButtonItem29.Id = 309;
			this.spreadsheetCommandBarButtonItem29.Name = "spreadsheetCommandBarButtonItem29";
			this.spreadsheetCommandBarButtonItem29.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarButtonItem30.ButtonGroupTag = "{A5E37DED-106E-44FC-8044-CE3824C08225}";
			this.spreadsheetCommandBarButtonItem30.CommandName = "FormatIncreaseIndent";
			this.spreadsheetCommandBarButtonItem30.Id = 310;
			this.spreadsheetCommandBarButtonItem30.Name = "spreadsheetCommandBarButtonItem30";
			this.spreadsheetCommandBarButtonItem30.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarCheckItem11.CommandName = "FormatWrapText";
			this.spreadsheetCommandBarCheckItem11.Id = 311;
			this.spreadsheetCommandBarCheckItem11.Name = "spreadsheetCommandBarCheckItem11";
			this.spreadsheetCommandBarCheckItem11.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.spreadsheetCommandBarSubItem2.CommandName = "EditingMergeCellsCommandGroup";
			this.spreadsheetCommandBarSubItem2.Id = 312;
			this.spreadsheetCommandBarSubItem2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem12),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem31),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem32),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem33)});
			this.spreadsheetCommandBarSubItem2.Name = "spreadsheetCommandBarSubItem2";
			this.spreadsheetCommandBarSubItem2.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.spreadsheetCommandBarCheckItem12.CommandName = "EditingMergeAndCenterCells";
			this.spreadsheetCommandBarCheckItem12.Id = 313;
			this.spreadsheetCommandBarCheckItem12.Name = "spreadsheetCommandBarCheckItem12";
			this.spreadsheetCommandBarButtonItem31.CommandName = "EditingMergeCellsAcross";
			this.spreadsheetCommandBarButtonItem31.Id = 314;
			this.spreadsheetCommandBarButtonItem31.Name = "spreadsheetCommandBarButtonItem31";
			this.spreadsheetCommandBarButtonItem32.CommandName = "EditingMergeCells";
			this.spreadsheetCommandBarButtonItem32.Id = 315;
			this.spreadsheetCommandBarButtonItem32.Name = "spreadsheetCommandBarButtonItem32";
			this.spreadsheetCommandBarButtonItem33.CommandName = "EditingUnmergeCells";
			this.spreadsheetCommandBarButtonItem33.Id = 316;
			this.spreadsheetCommandBarButtonItem33.Name = "spreadsheetCommandBarButtonItem33";
			this.barButtonGroup8.Id = 270;
			this.barButtonGroup8.ItemLinks.Add(this.changeNumberFormatItem1);
			this.barButtonGroup8.Name = "barButtonGroup8";
			this.barButtonGroup8.Tag = "{0B3A7A43-3079-4ce0-83A8-3789F5F6DC9F}";
			this.changeNumberFormatItem1.Edit = this.repositoryItemPopupGalleryEdit2;
			this.changeNumberFormatItem1.Id = 317;
			this.changeNumberFormatItem1.Name = "changeNumberFormatItem1";
			this.repositoryItemPopupGalleryEdit2.AutoHeight = false;
			this.repositoryItemPopupGalleryEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemPopupGalleryEdit2.Gallery.AllowFilter = false;
			this.repositoryItemPopupGalleryEdit2.Gallery.AutoFitColumns = false;
			this.repositoryItemPopupGalleryEdit2.Gallery.ColumnCount = 1;
			this.repositoryItemPopupGalleryEdit2.Gallery.FixedImageSize = false;
			spreadsheetCommandGalleryItem13.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem13.CaptionAsValue = true;
			spreadsheetCommandGalleryItem13.CommandName = "FormatNumberGeneral";
			spreadsheetCommandGalleryItem13.IsEmptyHint = true;
			spreadsheetCommandGalleryItem14.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem14.CaptionAsValue = true;
			spreadsheetCommandGalleryItem14.CommandName = "FormatNumberDecimal";
			spreadsheetCommandGalleryItem14.IsEmptyHint = true;
			spreadsheetCommandGalleryItem15.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem15.CaptionAsValue = true;
			spreadsheetCommandGalleryItem15.CommandName = "FormatNumberAccountingCurrency";
			spreadsheetCommandGalleryItem15.IsEmptyHint = true;
			spreadsheetCommandGalleryItem16.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem16.CaptionAsValue = true;
			spreadsheetCommandGalleryItem16.CommandName = "FormatNumberAccountingRegular";
			spreadsheetCommandGalleryItem16.IsEmptyHint = true;
			spreadsheetCommandGalleryItem17.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem17.CaptionAsValue = true;
			spreadsheetCommandGalleryItem17.CommandName = "FormatNumberShortDate";
			spreadsheetCommandGalleryItem17.IsEmptyHint = true;
			spreadsheetCommandGalleryItem18.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem18.CaptionAsValue = true;
			spreadsheetCommandGalleryItem18.CommandName = "FormatNumberLongDate";
			spreadsheetCommandGalleryItem18.IsEmptyHint = true;
			spreadsheetCommandGalleryItem19.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem19.CaptionAsValue = true;
			spreadsheetCommandGalleryItem19.CommandName = "FormatNumberTime";
			spreadsheetCommandGalleryItem19.IsEmptyHint = true;
			spreadsheetCommandGalleryItem20.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem20.CaptionAsValue = true;
			spreadsheetCommandGalleryItem20.CommandName = "FormatNumberPercentage";
			spreadsheetCommandGalleryItem20.IsEmptyHint = true;
			spreadsheetCommandGalleryItem21.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem21.CaptionAsValue = true;
			spreadsheetCommandGalleryItem21.CommandName = "FormatNumberFraction";
			spreadsheetCommandGalleryItem21.IsEmptyHint = true;
			spreadsheetCommandGalleryItem22.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem22.CaptionAsValue = true;
			spreadsheetCommandGalleryItem22.CommandName = "FormatNumberScientific";
			spreadsheetCommandGalleryItem22.IsEmptyHint = true;
			spreadsheetCommandGalleryItem23.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem23.CaptionAsValue = true;
			spreadsheetCommandGalleryItem23.CommandName = "FormatNumberText";
			spreadsheetCommandGalleryItem23.IsEmptyHint = true;
			galleryItemGroup1.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem13,
			spreadsheetCommandGalleryItem14,
			spreadsheetCommandGalleryItem15,
			spreadsheetCommandGalleryItem16,
			spreadsheetCommandGalleryItem17,
			spreadsheetCommandGalleryItem18,
			spreadsheetCommandGalleryItem19,
			spreadsheetCommandGalleryItem20,
			spreadsheetCommandGalleryItem21,
			spreadsheetCommandGalleryItem22,
			spreadsheetCommandGalleryItem23});
			this.repositoryItemPopupGalleryEdit2.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			galleryItemGroup1});
			this.repositoryItemPopupGalleryEdit2.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.repositoryItemPopupGalleryEdit2.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.repositoryItemPopupGalleryEdit2.Gallery.RowCount = 11;
			this.repositoryItemPopupGalleryEdit2.Gallery.ShowGroupCaption = false;
			this.repositoryItemPopupGalleryEdit2.Gallery.ShowItemText = true;
			this.repositoryItemPopupGalleryEdit2.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Hide;
			this.repositoryItemPopupGalleryEdit2.Gallery.StretchItems = true;
			this.repositoryItemPopupGalleryEdit2.Name = "repositoryItemPopupGalleryEdit2";
			this.repositoryItemPopupGalleryEdit2.ShowButtons = false;
			this.repositoryItemPopupGalleryEdit2.ShowPopupCloseButton = false;
			this.repositoryItemPopupGalleryEdit2.ShowSizeGrip = false;
			this.barButtonGroup9.Id = 271;
			this.barButtonGroup9.ItemLinks.Add(this.spreadsheetCommandBarSubItem3);
			this.barButtonGroup9.ItemLinks.Add(this.spreadsheetCommandBarButtonItem39);
			this.barButtonGroup9.ItemLinks.Add(this.spreadsheetCommandBarButtonItem40);
			this.barButtonGroup9.Name = "barButtonGroup9";
			this.barButtonGroup9.Tag = "{508C2CE6-E1C8-4DD1-BA50-6C210FDB31B0}";
			this.spreadsheetCommandBarSubItem3.ButtonGroupTag = "{508C2CE6-E1C8-4DD1-BA50-6C210FDB31B0}";
			this.spreadsheetCommandBarSubItem3.CommandName = "FormatNumberAccountingCommandGroup";
			this.spreadsheetCommandBarSubItem3.Id = 318;
			this.spreadsheetCommandBarSubItem3.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem34),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem35),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem36),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem37),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem38)});
			this.spreadsheetCommandBarSubItem3.Name = "spreadsheetCommandBarSubItem3";
			this.spreadsheetCommandBarSubItem3.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarButtonItem34.CommandName = "FormatNumberAccountingUS";
			this.spreadsheetCommandBarButtonItem34.Id = 319;
			this.spreadsheetCommandBarButtonItem34.Name = "spreadsheetCommandBarButtonItem34";
			this.spreadsheetCommandBarButtonItem35.CommandName = "FormatNumberAccountingUK";
			this.spreadsheetCommandBarButtonItem35.Id = 320;
			this.spreadsheetCommandBarButtonItem35.Name = "spreadsheetCommandBarButtonItem35";
			this.spreadsheetCommandBarButtonItem36.CommandName = "FormatNumberAccountingEuro";
			this.spreadsheetCommandBarButtonItem36.Id = 321;
			this.spreadsheetCommandBarButtonItem36.Name = "spreadsheetCommandBarButtonItem36";
			this.spreadsheetCommandBarButtonItem37.CommandName = "FormatNumberAccountingPRC";
			this.spreadsheetCommandBarButtonItem37.Id = 322;
			this.spreadsheetCommandBarButtonItem37.Name = "spreadsheetCommandBarButtonItem37";
			this.spreadsheetCommandBarButtonItem38.CommandName = "FormatNumberAccountingSwiss";
			this.spreadsheetCommandBarButtonItem38.Id = 323;
			this.spreadsheetCommandBarButtonItem38.Name = "spreadsheetCommandBarButtonItem38";
			this.spreadsheetCommandBarButtonItem39.ButtonGroupTag = "{508C2CE6-E1C8-4DD1-BA50-6C210FDB31B0}";
			this.spreadsheetCommandBarButtonItem39.CommandName = "FormatNumberPercent";
			this.spreadsheetCommandBarButtonItem39.Id = 324;
			this.spreadsheetCommandBarButtonItem39.Name = "spreadsheetCommandBarButtonItem39";
			this.spreadsheetCommandBarButtonItem39.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarButtonItem40.ButtonGroupTag = "{508C2CE6-E1C8-4DD1-BA50-6C210FDB31B0}";
			this.spreadsheetCommandBarButtonItem40.CommandName = "FormatNumberAccounting";
			this.spreadsheetCommandBarButtonItem40.Id = 325;
			this.spreadsheetCommandBarButtonItem40.Name = "spreadsheetCommandBarButtonItem40";
			this.spreadsheetCommandBarButtonItem40.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.barButtonGroup10.Id = 272;
			this.barButtonGroup10.ItemLinks.Add(this.spreadsheetCommandBarButtonItem41);
			this.barButtonGroup10.ItemLinks.Add(this.spreadsheetCommandBarButtonItem42);
			this.barButtonGroup10.Name = "barButtonGroup10";
			this.barButtonGroup10.Tag = "{BBAB348B-BDB2-487A-A883-EFB9982DC698}";
			this.spreadsheetCommandBarButtonItem41.ButtonGroupTag = "{BBAB348B-BDB2-487A-A883-EFB9982DC698}";
			this.spreadsheetCommandBarButtonItem41.CommandName = "FormatNumberIncreaseDecimal";
			this.spreadsheetCommandBarButtonItem41.Id = 326;
			this.spreadsheetCommandBarButtonItem41.Name = "spreadsheetCommandBarButtonItem41";
			this.spreadsheetCommandBarButtonItem41.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarButtonItem42.ButtonGroupTag = "{BBAB348B-BDB2-487A-A883-EFB9982DC698}";
			this.spreadsheetCommandBarButtonItem42.CommandName = "FormatNumberDecreaseDecimal";
			this.spreadsheetCommandBarButtonItem42.Id = 327;
			this.spreadsheetCommandBarButtonItem42.Name = "spreadsheetCommandBarButtonItem42";
			this.spreadsheetCommandBarButtonItem42.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
			this.spreadsheetCommandBarButtonGalleryDropDownItem2.CommandName = "ConditionalFormattingColorScalesCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem2.DropDownControl = this.commandBarGalleryDropDown27;
			this.spreadsheetCommandBarButtonGalleryDropDownItem2.Id = 345;
			this.spreadsheetCommandBarButtonGalleryDropDownItem2.Name = "spreadsheetCommandBarButtonGalleryDropDownItem2";
			this.spreadsheetCommandBarButtonGalleryDropDownItem2.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.commandBarGalleryDropDown27.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup3.CommandName = "ConditionalFormattingColorScalesCommandGroup";
			spreadsheetCommandGalleryItem24.CommandName = "ConditionalFormattingColorScaleGreenYellowRed";
			spreadsheetCommandGalleryItem25.CommandName = "ConditionalFormattingColorScaleRedYellowGreen";
			spreadsheetCommandGalleryItem26.CommandName = "ConditionalFormattingColorScaleGreenWhiteRed";
			spreadsheetCommandGalleryItem27.CommandName = "ConditionalFormattingColorScaleRedWhiteGreen";
			spreadsheetCommandGalleryItem28.CommandName = "ConditionalFormattingColorScaleBlueWhiteRed";
			spreadsheetCommandGalleryItem29.CommandName = "ConditionalFormattingColorScaleRedWhiteBlue";
			spreadsheetCommandGalleryItem30.CommandName = "ConditionalFormattingColorScaleWhiteRed";
			spreadsheetCommandGalleryItem31.CommandName = "ConditionalFormattingColorScaleRedWhite";
			spreadsheetCommandGalleryItem32.CommandName = "ConditionalFormattingColorScaleGreenWhite";
			spreadsheetCommandGalleryItem33.CommandName = "ConditionalFormattingColorScaleWhiteGreen";
			spreadsheetCommandGalleryItem34.CommandName = "ConditionalFormattingColorScaleGreenYellow";
			spreadsheetCommandGalleryItem35.CommandName = "ConditionalFormattingColorScaleYellowGreen";
			spreadsheetCommandGalleryItemGroup3.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem24,
			spreadsheetCommandGalleryItem25,
			spreadsheetCommandGalleryItem26,
			spreadsheetCommandGalleryItem27,
			spreadsheetCommandGalleryItem28,
			spreadsheetCommandGalleryItem29,
			spreadsheetCommandGalleryItem30,
			spreadsheetCommandGalleryItem31,
			spreadsheetCommandGalleryItem32,
			spreadsheetCommandGalleryItem33,
			spreadsheetCommandGalleryItem34,
			spreadsheetCommandGalleryItem35});
			this.commandBarGalleryDropDown27.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup3});
			this.commandBarGalleryDropDown27.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown27.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown27.Name = "commandBarGalleryDropDown27";
			this.commandBarGalleryDropDown27.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem3.CommandName = "ConditionalFormattingIconSetsCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem3.DropDownControl = this.commandBarGalleryDropDown28;
			this.spreadsheetCommandBarButtonGalleryDropDownItem3.Id = 346;
			this.spreadsheetCommandBarButtonGalleryDropDownItem3.Name = "spreadsheetCommandBarButtonGalleryDropDownItem3";
			this.spreadsheetCommandBarButtonGalleryDropDownItem3.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.commandBarGalleryDropDown28.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup4.CommandName = "ConditionalFormattingIconSetsDirectionalCommandGroup";
			spreadsheetCommandGalleryItem36.CommandName = "ConditionalFormattingIconSetArrows3Colored";
			spreadsheetCommandGalleryItem37.CommandName = "ConditionalFormattingIconSetArrows3Grayed";
			spreadsheetCommandGalleryItem38.CommandName = "ConditionalFormattingIconSetArrows4Colored";
			spreadsheetCommandGalleryItem39.CommandName = "ConditionalFormattingIconSetArrows4Grayed";
			spreadsheetCommandGalleryItem40.CommandName = "ConditionalFormattingIconSetArrows5Colored";
			spreadsheetCommandGalleryItem41.CommandName = "ConditionalFormattingIconSetArrows5Grayed";
			spreadsheetCommandGalleryItem42.CommandName = "ConditionalFormattingIconSetTriangles3";
			spreadsheetCommandGalleryItemGroup4.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem36,
			spreadsheetCommandGalleryItem37,
			spreadsheetCommandGalleryItem38,
			spreadsheetCommandGalleryItem39,
			spreadsheetCommandGalleryItem40,
			spreadsheetCommandGalleryItem41,
			spreadsheetCommandGalleryItem42});
			spreadsheetCommandGalleryItemGroup5.CommandName = "ConditionalFormattingIconSetsShapesCommandGroup";
			spreadsheetCommandGalleryItem43.CommandName = "ConditionalFormattingIconSetTrafficLights3";
			spreadsheetCommandGalleryItem44.CommandName = "ConditionalFormattingIconSetTrafficLights3Rimmed";
			spreadsheetCommandGalleryItem45.CommandName = "ConditionalFormattingIconSetTrafficLights4";
			spreadsheetCommandGalleryItem46.CommandName = "ConditionalFormattingIconSetSigns3";
			spreadsheetCommandGalleryItem47.CommandName = "ConditionalFormattingIconSetRedToBlack";
			spreadsheetCommandGalleryItemGroup5.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem43,
			spreadsheetCommandGalleryItem44,
			spreadsheetCommandGalleryItem45,
			spreadsheetCommandGalleryItem46,
			spreadsheetCommandGalleryItem47});
			spreadsheetCommandGalleryItemGroup6.CommandName = "ConditionalFormattingIconSetsIndicatorsCommandGroup";
			spreadsheetCommandGalleryItem48.CommandName = "ConditionalFormattingIconSetSymbols3Circled";
			spreadsheetCommandGalleryItem49.CommandName = "ConditionalFormattingIconSetSymbols3";
			spreadsheetCommandGalleryItem50.CommandName = "ConditionalFormattingIconSetFlags3";
			spreadsheetCommandGalleryItemGroup6.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem48,
			spreadsheetCommandGalleryItem49,
			spreadsheetCommandGalleryItem50});
			spreadsheetCommandGalleryItemGroup7.CommandName = "ConditionalFormattingIconSetsRatingsCommandGroup";
			spreadsheetCommandGalleryItem51.CommandName = "ConditionalFormattingIconSetStars3";
			spreadsheetCommandGalleryItem52.CommandName = "ConditionalFormattingIconSetRatings4";
			spreadsheetCommandGalleryItem53.CommandName = "ConditionalFormattingIconSetRatings5";
			spreadsheetCommandGalleryItem54.CommandName = "ConditionalFormattingIconSetQuarters5";
			spreadsheetCommandGalleryItem55.CommandName = "ConditionalFormattingIconSetBoxes5";
			spreadsheetCommandGalleryItemGroup7.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem51,
			spreadsheetCommandGalleryItem52,
			spreadsheetCommandGalleryItem53,
			spreadsheetCommandGalleryItem54,
			spreadsheetCommandGalleryItem55});
			this.commandBarGalleryDropDown28.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup4,
			spreadsheetCommandGalleryItemGroup5,
			spreadsheetCommandGalleryItemGroup6,
			spreadsheetCommandGalleryItemGroup7});
			this.commandBarGalleryDropDown28.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown28.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown28.Name = "commandBarGalleryDropDown28";
			this.commandBarGalleryDropDown28.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonItem56.CommandName = "ConditionalFormattingRemoveFromSheet";
			this.spreadsheetCommandBarButtonItem56.Id = 347;
			this.spreadsheetCommandBarButtonItem56.Name = "spreadsheetCommandBarButtonItem56";
			this.spreadsheetCommandBarButtonItem57.CommandName = "ConditionalFormattingRemove";
			this.spreadsheetCommandBarButtonItem57.Id = 348;
			this.spreadsheetCommandBarButtonItem57.Name = "spreadsheetCommandBarButtonItem57";
			this.spreadsheetCommandBarSubItem7.CommandName = "ConditionalFormattingRemoveCommandGroup";
			this.spreadsheetCommandBarSubItem7.Id = 349;
			this.spreadsheetCommandBarSubItem7.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem56),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem57)});
			this.spreadsheetCommandBarSubItem7.Name = "spreadsheetCommandBarSubItem7";
			this.galleryFormatAsTableItem1.DropDownControl = this.commandBarGalleryDropDown29;
			this.galleryFormatAsTableItem1.Id = 350;
			this.galleryFormatAsTableItem1.Name = "galleryFormatAsTableItem1";
			this.commandBarGalleryDropDown29.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown29.Gallery.ColumnCount = 7;
			this.commandBarGalleryDropDown29.Gallery.DrawImageBackground = false;
			this.commandBarGalleryDropDown29.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
			this.commandBarGalleryDropDown29.Gallery.ItemSize = new System.Drawing.Size(73, 58);
			this.commandBarGalleryDropDown29.Gallery.RowCount = 10;
			this.commandBarGalleryDropDown29.Name = "commandBarGalleryDropDown29";
			this.commandBarGalleryDropDown29.Ribbon = this.ribbonControl1;
			this.galleryChangeStyleItem1.Gallery.DrawImageBackground = false;
			this.galleryChangeStyleItem1.Gallery.ImageSize = new System.Drawing.Size(65, 46);
			this.galleryChangeStyleItem1.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
			this.galleryChangeStyleItem1.Gallery.ItemSize = new System.Drawing.Size(106, 28);
			this.galleryChangeStyleItem1.Gallery.RowCount = 9;
			this.galleryChangeStyleItem1.Gallery.ShowItemText = true;
			this.galleryChangeStyleItem1.Id = 351;
			this.galleryChangeStyleItem1.Name = "galleryChangeStyleItem1";
			this.spreadsheetCommandBarSubItem8.CommandName = "InsertCellsCommandGroup";
			this.spreadsheetCommandBarSubItem8.Id = 352;
			this.spreadsheetCommandBarSubItem8.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem58),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem59),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem60)});
			this.spreadsheetCommandBarSubItem8.Name = "spreadsheetCommandBarSubItem8";
			this.spreadsheetCommandBarButtonItem58.CommandName = "InsertSheetRows";
			this.spreadsheetCommandBarButtonItem58.Id = 353;
			this.spreadsheetCommandBarButtonItem58.Name = "spreadsheetCommandBarButtonItem58";
			this.spreadsheetCommandBarButtonItem59.CommandName = "InsertSheetColumns";
			this.spreadsheetCommandBarButtonItem59.Id = 354;
			this.spreadsheetCommandBarButtonItem59.Name = "spreadsheetCommandBarButtonItem59";
			this.spreadsheetCommandBarButtonItem60.CommandName = "InsertSheet";
			this.spreadsheetCommandBarButtonItem60.Id = 355;
			this.spreadsheetCommandBarButtonItem60.Name = "spreadsheetCommandBarButtonItem60";
			this.spreadsheetCommandBarSubItem9.CommandName = "RemoveCellsCommandGroup";
			this.spreadsheetCommandBarSubItem9.Id = 356;
			this.spreadsheetCommandBarSubItem9.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem61),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem62),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem63)});
			this.spreadsheetCommandBarSubItem9.Name = "spreadsheetCommandBarSubItem9";
			this.spreadsheetCommandBarButtonItem61.CommandName = "RemoveSheetRows";
			this.spreadsheetCommandBarButtonItem61.Id = 357;
			this.spreadsheetCommandBarButtonItem61.Name = "spreadsheetCommandBarButtonItem61";
			this.spreadsheetCommandBarButtonItem62.CommandName = "RemoveSheetColumns";
			this.spreadsheetCommandBarButtonItem62.Id = 358;
			this.spreadsheetCommandBarButtonItem62.Name = "spreadsheetCommandBarButtonItem62";
			this.spreadsheetCommandBarButtonItem63.CommandName = "RemoveSheet";
			this.spreadsheetCommandBarButtonItem63.Id = 359;
			this.spreadsheetCommandBarButtonItem63.Name = "spreadsheetCommandBarButtonItem63";
			this.spreadsheetCommandBarSubItem10.CommandName = "FormatCommandGroup";
			this.spreadsheetCommandBarSubItem10.Id = 360;
			this.spreadsheetCommandBarSubItem10.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem64),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem65),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem66),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem67),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem68),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarSubItem11),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem75),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeSheetTabColorItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem76),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem13),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem77)});
			this.spreadsheetCommandBarSubItem10.Name = "spreadsheetCommandBarSubItem10";
			this.spreadsheetCommandBarButtonItem64.CommandName = "FormatRowHeight";
			this.spreadsheetCommandBarButtonItem64.Id = 361;
			this.spreadsheetCommandBarButtonItem64.Name = "spreadsheetCommandBarButtonItem64";
			this.spreadsheetCommandBarButtonItem65.CommandName = "FormatAutoFitRowHeight";
			this.spreadsheetCommandBarButtonItem65.Id = 362;
			this.spreadsheetCommandBarButtonItem65.Name = "spreadsheetCommandBarButtonItem65";
			this.spreadsheetCommandBarButtonItem66.CommandName = "FormatColumnWidth";
			this.spreadsheetCommandBarButtonItem66.Id = 363;
			this.spreadsheetCommandBarButtonItem66.Name = "spreadsheetCommandBarButtonItem66";
			this.spreadsheetCommandBarButtonItem67.CommandName = "FormatAutoFitColumnWidth";
			this.spreadsheetCommandBarButtonItem67.Id = 364;
			this.spreadsheetCommandBarButtonItem67.Name = "spreadsheetCommandBarButtonItem67";
			this.spreadsheetCommandBarButtonItem68.CommandName = "FormatDefaultColumnWidth";
			this.spreadsheetCommandBarButtonItem68.Id = 365;
			this.spreadsheetCommandBarButtonItem68.Name = "spreadsheetCommandBarButtonItem68";
			this.spreadsheetCommandBarSubItem11.CommandName = "HideAndUnhideCommandGroup";
			this.spreadsheetCommandBarSubItem11.Id = 372;
			this.spreadsheetCommandBarSubItem11.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem69),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem70),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem71),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem72),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem73),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem74)});
			this.spreadsheetCommandBarSubItem11.Name = "spreadsheetCommandBarSubItem11";
			this.spreadsheetCommandBarButtonItem69.CommandName = "HideRows";
			this.spreadsheetCommandBarButtonItem69.Id = 366;
			this.spreadsheetCommandBarButtonItem69.Name = "spreadsheetCommandBarButtonItem69";
			this.spreadsheetCommandBarButtonItem70.CommandName = "HideColumns";
			this.spreadsheetCommandBarButtonItem70.Id = 367;
			this.spreadsheetCommandBarButtonItem70.Name = "spreadsheetCommandBarButtonItem70";
			this.spreadsheetCommandBarButtonItem71.CommandName = "HideSheet";
			this.spreadsheetCommandBarButtonItem71.Id = 368;
			this.spreadsheetCommandBarButtonItem71.Name = "spreadsheetCommandBarButtonItem71";
			this.spreadsheetCommandBarButtonItem72.CommandName = "UnhideRows";
			this.spreadsheetCommandBarButtonItem72.Id = 369;
			this.spreadsheetCommandBarButtonItem72.Name = "spreadsheetCommandBarButtonItem72";
			this.spreadsheetCommandBarButtonItem73.CommandName = "UnhideColumns";
			this.spreadsheetCommandBarButtonItem73.Id = 370;
			this.spreadsheetCommandBarButtonItem73.Name = "spreadsheetCommandBarButtonItem73";
			this.spreadsheetCommandBarButtonItem74.CommandName = "UnhideSheet";
			this.spreadsheetCommandBarButtonItem74.Id = 371;
			this.spreadsheetCommandBarButtonItem74.Name = "spreadsheetCommandBarButtonItem74";
			this.spreadsheetCommandBarButtonItem75.CommandName = "RenameSheet";
			this.spreadsheetCommandBarButtonItem75.Id = 373;
			this.spreadsheetCommandBarButtonItem75.Name = "spreadsheetCommandBarButtonItem75";
			this.changeSheetTabColorItem1.ActAsDropDown = true;
			this.changeSheetTabColorItem1.Id = 374;
			this.changeSheetTabColorItem1.Name = "changeSheetTabColorItem1";
			this.spreadsheetCommandBarButtonItem76.CommandName = "ReviewProtectSheet";
			this.spreadsheetCommandBarButtonItem76.Id = 375;
			this.spreadsheetCommandBarButtonItem76.Name = "spreadsheetCommandBarButtonItem76";
			this.spreadsheetCommandBarCheckItem13.CommandName = "FormatCellLocked";
			this.spreadsheetCommandBarCheckItem13.Id = 376;
			this.spreadsheetCommandBarCheckItem13.Name = "spreadsheetCommandBarCheckItem13";
			this.spreadsheetCommandBarButtonItem77.CommandName = "FormatCellsContextMenuItem";
			this.spreadsheetCommandBarButtonItem77.Id = 377;
			this.spreadsheetCommandBarButtonItem77.Name = "spreadsheetCommandBarButtonItem77";
			this.spreadsheetCommandBarSubItem12.CommandName = "EditingAutoSumCommandGroup";
			this.spreadsheetCommandBarSubItem12.Id = 378;
			this.spreadsheetCommandBarSubItem12.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem78),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem79),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem80),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem81),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem82)});
			this.spreadsheetCommandBarSubItem12.Name = "spreadsheetCommandBarSubItem12";
			this.spreadsheetCommandBarSubItem12.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.spreadsheetCommandBarButtonItem78.CommandName = "FunctionsInsertSum";
			this.spreadsheetCommandBarButtonItem78.Id = 379;
			this.spreadsheetCommandBarButtonItem78.Name = "spreadsheetCommandBarButtonItem78";
			this.spreadsheetCommandBarButtonItem79.CommandName = "FunctionsInsertAverage";
			this.spreadsheetCommandBarButtonItem79.Id = 380;
			this.spreadsheetCommandBarButtonItem79.Name = "spreadsheetCommandBarButtonItem79";
			this.spreadsheetCommandBarButtonItem80.CommandName = "FunctionsInsertCountNumbers";
			this.spreadsheetCommandBarButtonItem80.Id = 381;
			this.spreadsheetCommandBarButtonItem80.Name = "spreadsheetCommandBarButtonItem80";
			this.spreadsheetCommandBarButtonItem81.CommandName = "FunctionsInsertMax";
			this.spreadsheetCommandBarButtonItem81.Id = 382;
			this.spreadsheetCommandBarButtonItem81.Name = "spreadsheetCommandBarButtonItem81";
			this.spreadsheetCommandBarButtonItem82.CommandName = "FunctionsInsertMin";
			this.spreadsheetCommandBarButtonItem82.Id = 383;
			this.spreadsheetCommandBarButtonItem82.Name = "spreadsheetCommandBarButtonItem82";
			this.spreadsheetCommandBarSubItem13.CommandName = "EditingFillCommandGroup";
			this.spreadsheetCommandBarSubItem13.Id = 384;
			this.spreadsheetCommandBarSubItem13.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem83),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem84),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem85),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem86)});
			this.spreadsheetCommandBarSubItem13.Name = "spreadsheetCommandBarSubItem13";
			this.spreadsheetCommandBarSubItem13.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.spreadsheetCommandBarButtonItem83.CommandName = "EditingFillDown";
			this.spreadsheetCommandBarButtonItem83.Id = 385;
			this.spreadsheetCommandBarButtonItem83.Name = "spreadsheetCommandBarButtonItem83";
			this.spreadsheetCommandBarButtonItem84.CommandName = "EditingFillRight";
			this.spreadsheetCommandBarButtonItem84.Id = 386;
			this.spreadsheetCommandBarButtonItem84.Name = "spreadsheetCommandBarButtonItem84";
			this.spreadsheetCommandBarButtonItem85.CommandName = "EditingFillUp";
			this.spreadsheetCommandBarButtonItem85.Id = 387;
			this.spreadsheetCommandBarButtonItem85.Name = "spreadsheetCommandBarButtonItem85";
			this.spreadsheetCommandBarButtonItem86.CommandName = "EditingFillLeft";
			this.spreadsheetCommandBarButtonItem86.Id = 388;
			this.spreadsheetCommandBarButtonItem86.Name = "spreadsheetCommandBarButtonItem86";
			this.spreadsheetCommandBarSubItem14.CommandName = "FormatClearCommandGroup";
			this.spreadsheetCommandBarSubItem14.Id = 389;
			this.spreadsheetCommandBarSubItem14.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem87),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem88),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem89),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem90),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem91),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem92)});
			this.spreadsheetCommandBarSubItem14.Name = "spreadsheetCommandBarSubItem14";
			this.spreadsheetCommandBarSubItem14.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.spreadsheetCommandBarButtonItem87.CommandName = "FormatClearAll";
			this.spreadsheetCommandBarButtonItem87.Id = 390;
			this.spreadsheetCommandBarButtonItem87.Name = "spreadsheetCommandBarButtonItem87";
			this.spreadsheetCommandBarButtonItem88.CommandName = "FormatClearFormats";
			this.spreadsheetCommandBarButtonItem88.Id = 391;
			this.spreadsheetCommandBarButtonItem88.Name = "spreadsheetCommandBarButtonItem88";
			this.spreadsheetCommandBarButtonItem89.CommandName = "FormatClearContents";
			this.spreadsheetCommandBarButtonItem89.Id = 392;
			this.spreadsheetCommandBarButtonItem89.Name = "spreadsheetCommandBarButtonItem89";
			this.spreadsheetCommandBarButtonItem90.CommandName = "FormatClearComments";
			this.spreadsheetCommandBarButtonItem90.Id = 393;
			this.spreadsheetCommandBarButtonItem90.Name = "spreadsheetCommandBarButtonItem90";
			this.spreadsheetCommandBarButtonItem91.CommandName = "FormatClearHyperlinks";
			this.spreadsheetCommandBarButtonItem91.Id = 394;
			this.spreadsheetCommandBarButtonItem91.Name = "spreadsheetCommandBarButtonItem91";
			this.spreadsheetCommandBarButtonItem92.CommandName = "FormatRemoveHyperlinks";
			this.spreadsheetCommandBarButtonItem92.Id = 395;
			this.spreadsheetCommandBarButtonItem92.Name = "spreadsheetCommandBarButtonItem92";
			this.spreadsheetCommandBarSubItem15.CommandName = "EditingSortAndFilterCommandGroup";
			this.spreadsheetCommandBarSubItem15.Id = 396;
			this.spreadsheetCommandBarSubItem15.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem93),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem94)});
			this.spreadsheetCommandBarSubItem15.Name = "spreadsheetCommandBarSubItem15";
			this.spreadsheetCommandBarButtonItem93.CommandName = "DataSortAscending";
			this.spreadsheetCommandBarButtonItem93.Id = 397;
			this.spreadsheetCommandBarButtonItem93.Name = "spreadsheetCommandBarButtonItem93";
			this.spreadsheetCommandBarButtonItem94.CommandName = "DataSortDescending";
			this.spreadsheetCommandBarButtonItem94.Id = 398;
			this.spreadsheetCommandBarButtonItem94.Name = "spreadsheetCommandBarButtonItem94";
			this.spreadsheetCommandBarSubItem16.CommandName = "EditingFindAndSelectCommandGroup";
			this.spreadsheetCommandBarSubItem16.Id = 399;
			this.spreadsheetCommandBarSubItem16.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem95),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem96)});
			this.spreadsheetCommandBarSubItem16.Name = "spreadsheetCommandBarSubItem16";
			this.spreadsheetCommandBarButtonItem95.CommandName = "EditingFind";
			this.spreadsheetCommandBarButtonItem95.Id = 400;
			this.spreadsheetCommandBarButtonItem95.Name = "spreadsheetCommandBarButtonItem95";
			this.spreadsheetCommandBarButtonItem96.CommandName = "EditingReplace";
			this.spreadsheetCommandBarButtonItem96.Id = 401;
			this.spreadsheetCommandBarButtonItem96.Name = "spreadsheetCommandBarButtonItem96";
			this.spreadsheetCommandBarButtonItem97.CommandName = "InsertTable";
			this.spreadsheetCommandBarButtonItem97.Id = 402;
			this.spreadsheetCommandBarButtonItem97.Name = "spreadsheetCommandBarButtonItem97";
			this.spreadsheetCommandBarButtonItem98.CommandName = "InsertPicture";
			this.spreadsheetCommandBarButtonItem98.Id = 403;
			this.spreadsheetCommandBarButtonItem98.Name = "spreadsheetCommandBarButtonItem98";
			this.spreadsheetCommandBarButtonGalleryDropDownItem4.CommandName = "InsertChartColumnCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem4.DropDownControl = this.commandBarGalleryDropDown30;
			this.spreadsheetCommandBarButtonGalleryDropDownItem4.Id = 404;
			this.spreadsheetCommandBarButtonGalleryDropDownItem4.Name = "spreadsheetCommandBarButtonGalleryDropDownItem4";
			this.commandBarGalleryDropDown30.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup8.CommandName = "InsertChartColumn2DCommandGroup";
			spreadsheetCommandGalleryItem56.CommandName = "InsertChartColumnClustered2D";
			spreadsheetCommandGalleryItem57.CommandName = "InsertChartColumnStacked2D";
			spreadsheetCommandGalleryItem58.CommandName = "InsertChartColumnPercentStacked2D";
			spreadsheetCommandGalleryItemGroup8.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem56,
			spreadsheetCommandGalleryItem57,
			spreadsheetCommandGalleryItem58});
			spreadsheetCommandGalleryItemGroup9.CommandName = "InsertChartColumn3DCommandGroup";
			spreadsheetCommandGalleryItem59.CommandName = "InsertChartColumnClustered3D";
			spreadsheetCommandGalleryItem60.CommandName = "InsertChartColumnStacked3D";
			spreadsheetCommandGalleryItem61.CommandName = "InsertChartColumnPercentStacked3D";
			spreadsheetCommandGalleryItem62.CommandName = "InsertChartColumn3D";
			spreadsheetCommandGalleryItemGroup9.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem59,
			spreadsheetCommandGalleryItem60,
			spreadsheetCommandGalleryItem61,
			spreadsheetCommandGalleryItem62});
			spreadsheetCommandGalleryItemGroup10.CommandName = "InsertChartCylinderCommandGroup";
			spreadsheetCommandGalleryItem63.CommandName = "InsertChartCylinderClustered";
			spreadsheetCommandGalleryItem64.CommandName = "InsertChartCylinderStacked";
			spreadsheetCommandGalleryItem65.CommandName = "InsertChartCylinderPercentStacked";
			spreadsheetCommandGalleryItem66.CommandName = "InsertChartCylinder";
			spreadsheetCommandGalleryItemGroup10.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem63,
			spreadsheetCommandGalleryItem64,
			spreadsheetCommandGalleryItem65,
			spreadsheetCommandGalleryItem66});
			spreadsheetCommandGalleryItemGroup11.CommandName = "InsertChartConeCommandGroup";
			spreadsheetCommandGalleryItem67.CommandName = "InsertChartConeClustered";
			spreadsheetCommandGalleryItem68.CommandName = "InsertChartConeStacked";
			spreadsheetCommandGalleryItem69.CommandName = "InsertChartConePercentStacked";
			spreadsheetCommandGalleryItem70.CommandName = "InsertChartCone";
			spreadsheetCommandGalleryItemGroup11.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem67,
			spreadsheetCommandGalleryItem68,
			spreadsheetCommandGalleryItem69,
			spreadsheetCommandGalleryItem70});
			spreadsheetCommandGalleryItemGroup12.CommandName = "InsertChartPyramidCommandGroup";
			spreadsheetCommandGalleryItem71.CommandName = "InsertChartPyramidClustered";
			spreadsheetCommandGalleryItem72.CommandName = "InsertChartPyramidStacked";
			spreadsheetCommandGalleryItem73.CommandName = "InsertChartPyramidPercentStacked";
			spreadsheetCommandGalleryItem74.CommandName = "InsertChartPyramid";
			spreadsheetCommandGalleryItemGroup12.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem71,
			spreadsheetCommandGalleryItem72,
			spreadsheetCommandGalleryItem73,
			spreadsheetCommandGalleryItem74});
			this.commandBarGalleryDropDown30.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup8,
			spreadsheetCommandGalleryItemGroup9,
			spreadsheetCommandGalleryItemGroup10,
			spreadsheetCommandGalleryItemGroup11,
			spreadsheetCommandGalleryItemGroup12});
			this.commandBarGalleryDropDown30.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown30.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown30.Name = "commandBarGalleryDropDown30";
			this.commandBarGalleryDropDown30.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem5.CommandName = "InsertChartLineCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem5.DropDownControl = this.commandBarGalleryDropDown31;
			this.spreadsheetCommandBarButtonGalleryDropDownItem5.Id = 405;
			this.spreadsheetCommandBarButtonGalleryDropDownItem5.Name = "spreadsheetCommandBarButtonGalleryDropDownItem5";
			this.commandBarGalleryDropDown31.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup13.CommandName = "InsertChartLine2DCommandGroup";
			spreadsheetCommandGalleryItem75.CommandName = "InsertChartLine";
			spreadsheetCommandGalleryItem76.CommandName = "InsertChartStackedLine";
			spreadsheetCommandGalleryItem77.CommandName = "InsertChartPercentStackedLine";
			spreadsheetCommandGalleryItem78.CommandName = "InsertChartLineWithMarkers";
			spreadsheetCommandGalleryItem79.CommandName = "InsertChartStackedLineWithMarkers";
			spreadsheetCommandGalleryItem80.CommandName = "InsertChartPercentStackedLineWithMarkers";
			spreadsheetCommandGalleryItemGroup13.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem75,
			spreadsheetCommandGalleryItem76,
			spreadsheetCommandGalleryItem77,
			spreadsheetCommandGalleryItem78,
			spreadsheetCommandGalleryItem79,
			spreadsheetCommandGalleryItem80});
			spreadsheetCommandGalleryItemGroup14.CommandName = "InsertChartLine3DCommandGroup";
			spreadsheetCommandGalleryItem81.CommandName = "InsertChartLine3D";
			spreadsheetCommandGalleryItemGroup14.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem81});
			this.commandBarGalleryDropDown31.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup13,
			spreadsheetCommandGalleryItemGroup14});
			this.commandBarGalleryDropDown31.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown31.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown31.Name = "commandBarGalleryDropDown31";
			this.commandBarGalleryDropDown31.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem6.CommandName = "InsertChartPieCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem6.DropDownControl = this.commandBarGalleryDropDown32;
			this.spreadsheetCommandBarButtonGalleryDropDownItem6.Id = 406;
			this.spreadsheetCommandBarButtonGalleryDropDownItem6.Name = "spreadsheetCommandBarButtonGalleryDropDownItem6";
			this.commandBarGalleryDropDown32.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup15.CommandName = "InsertChartPie2DCommandGroup";
			spreadsheetCommandGalleryItem82.CommandName = "InsertChartPie2D";
			spreadsheetCommandGalleryItem83.CommandName = "InsertChartPieExploded2D";
			spreadsheetCommandGalleryItemGroup15.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem82,
			spreadsheetCommandGalleryItem83});
			spreadsheetCommandGalleryItemGroup16.CommandName = "InsertChartPie3DCommandGroup";
			spreadsheetCommandGalleryItem84.CommandName = "InsertChartPie3D";
			spreadsheetCommandGalleryItem85.CommandName = "InsertChartPieExploded3D";
			spreadsheetCommandGalleryItemGroup16.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem84,
			spreadsheetCommandGalleryItem85});
			spreadsheetCommandGalleryItemGroup17.CommandName = "InsertChartDoughnut2DCommandGroup";
			spreadsheetCommandGalleryItem86.CommandName = "InsertChartDoughnut2D";
			spreadsheetCommandGalleryItem87.CommandName = "InsertChartDoughnutExploded2D";
			spreadsheetCommandGalleryItemGroup17.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem86,
			spreadsheetCommandGalleryItem87});
			this.commandBarGalleryDropDown32.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup15,
			spreadsheetCommandGalleryItemGroup16,
			spreadsheetCommandGalleryItemGroup17});
			this.commandBarGalleryDropDown32.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown32.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown32.Name = "commandBarGalleryDropDown32";
			this.commandBarGalleryDropDown32.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem7.CommandName = "InsertChartBarCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem7.DropDownControl = this.commandBarGalleryDropDown33;
			this.spreadsheetCommandBarButtonGalleryDropDownItem7.Id = 407;
			this.spreadsheetCommandBarButtonGalleryDropDownItem7.Name = "spreadsheetCommandBarButtonGalleryDropDownItem7";
			this.commandBarGalleryDropDown33.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup18.CommandName = "InsertChartBar2DCommandGroup";
			spreadsheetCommandGalleryItem88.CommandName = "InsertChartBarClustered2D";
			spreadsheetCommandGalleryItem89.CommandName = "InsertChartBarStacked2D";
			spreadsheetCommandGalleryItem90.CommandName = "InsertChartBarPercentStacked2D";
			spreadsheetCommandGalleryItemGroup18.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem88,
			spreadsheetCommandGalleryItem89,
			spreadsheetCommandGalleryItem90});
			spreadsheetCommandGalleryItemGroup19.CommandName = "InsertChartBar3DCommandGroup";
			spreadsheetCommandGalleryItem91.CommandName = "InsertChartBarClustered3D";
			spreadsheetCommandGalleryItem92.CommandName = "InsertChartBarStacked3D";
			spreadsheetCommandGalleryItem93.CommandName = "InsertChartBarPercentStacked3D";
			spreadsheetCommandGalleryItemGroup19.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem91,
			spreadsheetCommandGalleryItem92,
			spreadsheetCommandGalleryItem93});
			spreadsheetCommandGalleryItemGroup20.CommandName = "InsertChartHorizontalCylinderCommandGroup";
			spreadsheetCommandGalleryItem94.CommandName = "InsertChartHorizontalCylinderClustered";
			spreadsheetCommandGalleryItem95.CommandName = "InsertChartHorizontalCylinderStacked";
			spreadsheetCommandGalleryItem96.CommandName = "InsertChartHorizontalCylinderPercentStacked";
			spreadsheetCommandGalleryItemGroup20.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem94,
			spreadsheetCommandGalleryItem95,
			spreadsheetCommandGalleryItem96});
			spreadsheetCommandGalleryItemGroup21.CommandName = "InsertChartHorizontalConeCommandGroup";
			spreadsheetCommandGalleryItem97.CommandName = "InsertChartHorizontalConeClustered";
			spreadsheetCommandGalleryItem98.CommandName = "InsertChartHorizontalConeStacked";
			spreadsheetCommandGalleryItem99.CommandName = "InsertChartHorizontalConePercentStacked";
			spreadsheetCommandGalleryItemGroup21.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem97,
			spreadsheetCommandGalleryItem98,
			spreadsheetCommandGalleryItem99});
			spreadsheetCommandGalleryItemGroup22.CommandName = "InsertChartHorizontalPyramidCommandGroup";
			spreadsheetCommandGalleryItem100.CommandName = "InsertChartHorizontalPyramidClustered";
			spreadsheetCommandGalleryItem101.CommandName = "InsertChartHorizontalPyramidStacked";
			spreadsheetCommandGalleryItem102.CommandName = "InsertChartHorizontalPyramidPercentStacked";
			spreadsheetCommandGalleryItemGroup22.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem100,
			spreadsheetCommandGalleryItem101,
			spreadsheetCommandGalleryItem102});
			this.commandBarGalleryDropDown33.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup18,
			spreadsheetCommandGalleryItemGroup19,
			spreadsheetCommandGalleryItemGroup20,
			spreadsheetCommandGalleryItemGroup21,
			spreadsheetCommandGalleryItemGroup22});
			this.commandBarGalleryDropDown33.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown33.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown33.Name = "commandBarGalleryDropDown33";
			this.commandBarGalleryDropDown33.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem8.CommandName = "InsertChartAreaCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem8.DropDownControl = this.commandBarGalleryDropDown34;
			this.spreadsheetCommandBarButtonGalleryDropDownItem8.Id = 408;
			this.spreadsheetCommandBarButtonGalleryDropDownItem8.Name = "spreadsheetCommandBarButtonGalleryDropDownItem8";
			this.commandBarGalleryDropDown34.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup23.CommandName = "InsertChartArea2DCommandGroup";
			spreadsheetCommandGalleryItem103.CommandName = "InsertChartArea";
			spreadsheetCommandGalleryItem104.CommandName = "InsertChartStackedArea";
			spreadsheetCommandGalleryItem105.CommandName = "InsertChartPercentStackedArea";
			spreadsheetCommandGalleryItemGroup23.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem103,
			spreadsheetCommandGalleryItem104,
			spreadsheetCommandGalleryItem105});
			spreadsheetCommandGalleryItemGroup24.CommandName = "InsertChartArea3DCommandGroup";
			spreadsheetCommandGalleryItem106.CommandName = "InsertChartArea3D";
			spreadsheetCommandGalleryItem107.CommandName = "InsertChartStackedArea3D";
			spreadsheetCommandGalleryItem108.CommandName = "InsertChartPercentStackedArea3D";
			spreadsheetCommandGalleryItemGroup24.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem106,
			spreadsheetCommandGalleryItem107,
			spreadsheetCommandGalleryItem108});
			this.commandBarGalleryDropDown34.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup23,
			spreadsheetCommandGalleryItemGroup24});
			this.commandBarGalleryDropDown34.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown34.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown34.Name = "commandBarGalleryDropDown34";
			this.commandBarGalleryDropDown34.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem9.CommandName = "InsertChartScatterCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem9.DropDownControl = this.commandBarGalleryDropDown35;
			this.spreadsheetCommandBarButtonGalleryDropDownItem9.Id = 409;
			this.spreadsheetCommandBarButtonGalleryDropDownItem9.Name = "spreadsheetCommandBarButtonGalleryDropDownItem9";
			this.commandBarGalleryDropDown35.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup25.CommandName = "InsertChartScatterCommandGroup";
			spreadsheetCommandGalleryItem109.CommandName = "InsertChartScatterMarkers";
			spreadsheetCommandGalleryItem110.CommandName = "InsertChartScatterSmoothLinesAndMarkers";
			spreadsheetCommandGalleryItem111.CommandName = "InsertChartScatterSmoothLines";
			spreadsheetCommandGalleryItem112.CommandName = "InsertChartScatterLinesAndMarkers";
			spreadsheetCommandGalleryItem113.CommandName = "InsertChartScatterLines";
			spreadsheetCommandGalleryItemGroup25.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem109,
			spreadsheetCommandGalleryItem110,
			spreadsheetCommandGalleryItem111,
			spreadsheetCommandGalleryItem112,
			spreadsheetCommandGalleryItem113});
			spreadsheetCommandGalleryItemGroup26.CommandName = "InsertChartBubbleCommandGroup";
			spreadsheetCommandGalleryItem114.CommandName = "InsertChartBubble";
			spreadsheetCommandGalleryItem115.CommandName = "InsertChartBubble3D";
			spreadsheetCommandGalleryItemGroup26.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem114,
			spreadsheetCommandGalleryItem115});
			this.commandBarGalleryDropDown35.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup25,
			spreadsheetCommandGalleryItemGroup26});
			this.commandBarGalleryDropDown35.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown35.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown35.Name = "commandBarGalleryDropDown35";
			this.commandBarGalleryDropDown35.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem10.CommandName = "InsertChartOtherCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem10.DropDownControl = this.commandBarGalleryDropDown36;
			this.spreadsheetCommandBarButtonGalleryDropDownItem10.Id = 410;
			this.spreadsheetCommandBarButtonGalleryDropDownItem10.Name = "spreadsheetCommandBarButtonGalleryDropDownItem10";
			this.commandBarGalleryDropDown36.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup27.CommandName = "InsertChartStockCommandGroup";
			spreadsheetCommandGalleryItem116.CommandName = "InsertChartStockHighLowClose";
			spreadsheetCommandGalleryItem117.CommandName = "InsertChartStockOpenHighLowClose";
			spreadsheetCommandGalleryItemGroup27.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem116,
			spreadsheetCommandGalleryItem117});
			spreadsheetCommandGalleryItemGroup28.CommandName = "InsertChartRadarCommandGroup";
			spreadsheetCommandGalleryItem118.CommandName = "InsertChartRadar";
			spreadsheetCommandGalleryItem119.CommandName = "InsertChartRadarWithMarkers";
			spreadsheetCommandGalleryItem120.CommandName = "InsertChartRadarFilled";
			spreadsheetCommandGalleryItemGroup28.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem118,
			spreadsheetCommandGalleryItem119,
			spreadsheetCommandGalleryItem120});
			this.commandBarGalleryDropDown36.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup27,
			spreadsheetCommandGalleryItemGroup28});
			this.commandBarGalleryDropDown36.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown36.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown36.Name = "commandBarGalleryDropDown36";
			this.commandBarGalleryDropDown36.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonItem99.CommandName = "InsertHyperlink";
			this.spreadsheetCommandBarButtonItem99.Id = 411;
			this.spreadsheetCommandBarButtonItem99.Name = "spreadsheetCommandBarButtonItem99";
			this.spreadsheetCommandBarButtonItem100.CommandName = "InsertSymbol";
			this.spreadsheetCommandBarButtonItem100.Id = 412;
			this.spreadsheetCommandBarButtonItem100.Name = "spreadsheetCommandBarButtonItem100";
			this.spreadsheetCommandBarSubItem17.CommandName = "PageSetupMarginsCommandGroup";
			this.spreadsheetCommandBarSubItem17.Id = 413;
			this.spreadsheetCommandBarSubItem17.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem14),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem15),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem16)});
			this.spreadsheetCommandBarSubItem17.Name = "spreadsheetCommandBarSubItem17";
			this.spreadsheetCommandBarCheckItem14.CaptionDependOnUnits = true;
			this.spreadsheetCommandBarCheckItem14.CommandName = "PageSetupMarginsNormal";
			this.spreadsheetCommandBarCheckItem14.Id = 414;
			this.spreadsheetCommandBarCheckItem14.Name = "spreadsheetCommandBarCheckItem14";
			this.spreadsheetCommandBarCheckItem15.CaptionDependOnUnits = true;
			this.spreadsheetCommandBarCheckItem15.CommandName = "PageSetupMarginsWide";
			this.spreadsheetCommandBarCheckItem15.Id = 415;
			this.spreadsheetCommandBarCheckItem15.Name = "spreadsheetCommandBarCheckItem15";
			this.spreadsheetCommandBarCheckItem16.CaptionDependOnUnits = true;
			this.spreadsheetCommandBarCheckItem16.CommandName = "PageSetupMarginsNarrow";
			this.spreadsheetCommandBarCheckItem16.Id = 416;
			this.spreadsheetCommandBarCheckItem16.Name = "spreadsheetCommandBarCheckItem16";
			this.spreadsheetCommandBarSubItem18.CommandName = "PageSetupOrientationCommandGroup";
			this.spreadsheetCommandBarSubItem18.Id = 417;
			this.spreadsheetCommandBarSubItem18.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem17),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem18)});
			this.spreadsheetCommandBarSubItem18.Name = "spreadsheetCommandBarSubItem18";
			this.spreadsheetCommandBarCheckItem17.CommandName = "PageSetupOrientationPortrait";
			this.spreadsheetCommandBarCheckItem17.Id = 418;
			this.spreadsheetCommandBarCheckItem17.Name = "spreadsheetCommandBarCheckItem17";
			this.spreadsheetCommandBarCheckItem18.CommandName = "PageSetupOrientationLandscape";
			this.spreadsheetCommandBarCheckItem18.Id = 419;
			this.spreadsheetCommandBarCheckItem18.Name = "spreadsheetCommandBarCheckItem18";
			this.pageSetupPaperKindItem1.Id = 420;
			this.pageSetupPaperKindItem1.Name = "pageSetupPaperKindItem1";
			this.spreadsheetCommandBarSubItem19.CommandName = "PageSetupPrintAreaCommandGroup";
			this.spreadsheetCommandBarSubItem19.Id = 421;
			this.spreadsheetCommandBarSubItem19.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem101),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem102)});
			this.spreadsheetCommandBarSubItem19.Name = "spreadsheetCommandBarSubItem19";
			this.spreadsheetCommandBarButtonItem101.CommandName = "PageSetupSetPrintArea";
			this.spreadsheetCommandBarButtonItem101.Id = 422;
			this.spreadsheetCommandBarButtonItem101.Name = "spreadsheetCommandBarButtonItem101";
			this.spreadsheetCommandBarButtonItem102.CommandName = "PageSetupClearPrintArea";
			this.spreadsheetCommandBarButtonItem102.Id = 423;
			this.spreadsheetCommandBarButtonItem102.Name = "spreadsheetCommandBarButtonItem102";
			this.spreadsheetCommandBarCheckItem19.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			this.spreadsheetCommandBarCheckItem19.CommandName = "ViewShowGridlines";
			this.spreadsheetCommandBarCheckItem19.Id = 424;
			this.spreadsheetCommandBarCheckItem19.Name = "spreadsheetCommandBarCheckItem19";
			this.spreadsheetCommandBarCheckItem20.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			this.spreadsheetCommandBarCheckItem20.CommandName = "ViewShowHeadings";
			this.spreadsheetCommandBarCheckItem20.Id = 425;
			this.spreadsheetCommandBarCheckItem20.Name = "spreadsheetCommandBarCheckItem20";
			this.spreadsheetCommandBarCheckItem21.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			this.spreadsheetCommandBarCheckItem21.CommandName = "PageSetupPrintGridlines";
			this.spreadsheetCommandBarCheckItem21.Id = 426;
			this.spreadsheetCommandBarCheckItem21.Name = "spreadsheetCommandBarCheckItem21";
			this.spreadsheetCommandBarCheckItem22.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			this.spreadsheetCommandBarCheckItem22.CommandName = "PageSetupPrintHeadings";
			this.spreadsheetCommandBarCheckItem22.Id = 427;
			this.spreadsheetCommandBarCheckItem22.Name = "spreadsheetCommandBarCheckItem22";
			this.spreadsheetCommandBarSubItem20.CommandName = "ArrangeBringForwardCommandGroup";
			this.spreadsheetCommandBarSubItem20.Id = 428;
			this.spreadsheetCommandBarSubItem20.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem103),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem104)});
			this.spreadsheetCommandBarSubItem20.Name = "spreadsheetCommandBarSubItem20";
			this.spreadsheetCommandBarButtonItem103.CommandName = "ArrangeBringForward";
			this.spreadsheetCommandBarButtonItem103.Id = 429;
			this.spreadsheetCommandBarButtonItem103.Name = "spreadsheetCommandBarButtonItem103";
			this.spreadsheetCommandBarButtonItem104.CommandName = "ArrangeBringToFront";
			this.spreadsheetCommandBarButtonItem104.Id = 430;
			this.spreadsheetCommandBarButtonItem104.Name = "spreadsheetCommandBarButtonItem104";
			this.spreadsheetCommandBarSubItem21.CommandName = "ArrangeSendBackwardCommandGroup";
			this.spreadsheetCommandBarSubItem21.Id = 431;
			this.spreadsheetCommandBarSubItem21.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem105),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem106)});
			this.spreadsheetCommandBarSubItem21.Name = "spreadsheetCommandBarSubItem21";
			this.spreadsheetCommandBarButtonItem105.CommandName = "ArrangeSendBackward";
			this.spreadsheetCommandBarButtonItem105.Id = 432;
			this.spreadsheetCommandBarButtonItem105.Name = "spreadsheetCommandBarButtonItem105";
			this.spreadsheetCommandBarButtonItem106.CommandName = "ArrangeSendToBack";
			this.spreadsheetCommandBarButtonItem106.Id = 433;
			this.spreadsheetCommandBarButtonItem106.Name = "spreadsheetCommandBarButtonItem106";
			this.spreadsheetCommandBarSubItem22.CommandName = "FunctionsAutoSumCommandGroup";
			this.spreadsheetCommandBarSubItem22.Id = 434;
			this.spreadsheetCommandBarSubItem22.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem78),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem79),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem80),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem81),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem82)});
			this.spreadsheetCommandBarSubItem22.Name = "spreadsheetCommandBarSubItem22";
			this.functionsFinancialItem1.Id = 435;
			this.functionsFinancialItem1.Name = "functionsFinancialItem1";
			this.functionsLogicalItem1.Id = 436;
			this.functionsLogicalItem1.Name = "functionsLogicalItem1";
			this.functionsTextItem1.Id = 437;
			this.functionsTextItem1.Name = "functionsTextItem1";
			this.functionsDateAndTimeItem1.Id = 438;
			this.functionsDateAndTimeItem1.Name = "functionsDateAndTimeItem1";
			this.functionsLookupAndReferenceItem1.Id = 439;
			this.functionsLookupAndReferenceItem1.Name = "functionsLookupAndReferenceItem1";
			this.functionsMathAndTrigonometryItem1.Id = 440;
			this.functionsMathAndTrigonometryItem1.Name = "functionsMathAndTrigonometryItem1";
			this.spreadsheetCommandBarSubItem23.CommandName = "FunctionsMoreCommandGroup";
			this.spreadsheetCommandBarSubItem23.Id = 441;
			this.spreadsheetCommandBarSubItem23.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.functionsStatisticalItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.functionsEngineeringItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.functionsInformationItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.functionsCompatibilityItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.functionsWebItem1)});
			this.spreadsheetCommandBarSubItem23.Name = "spreadsheetCommandBarSubItem23";
			this.functionsStatisticalItem1.Id = 442;
			this.functionsStatisticalItem1.Name = "functionsStatisticalItem1";
			this.functionsEngineeringItem1.Id = 443;
			this.functionsEngineeringItem1.Name = "functionsEngineeringItem1";
			this.functionsInformationItem1.Id = 444;
			this.functionsInformationItem1.Name = "functionsInformationItem1";
			this.functionsCompatibilityItem1.Id = 445;
			this.functionsCompatibilityItem1.Name = "functionsCompatibilityItem1";
			this.functionsWebItem1.Id = 446;
			this.functionsWebItem1.Name = "functionsWebItem1";
			this.spreadsheetCommandBarButtonItem112.CommandName = "FormulasShowNameManager";
			this.spreadsheetCommandBarButtonItem112.Id = 447;
			this.spreadsheetCommandBarButtonItem112.Name = "spreadsheetCommandBarButtonItem112";
			this.spreadsheetCommandBarButtonItem113.CommandName = "FormulasDefineNameCommand";
			this.spreadsheetCommandBarButtonItem113.Id = 448;
			this.spreadsheetCommandBarButtonItem113.Name = "spreadsheetCommandBarButtonItem113";
			this.spreadsheetCommandBarButtonItem113.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.definedNameListItem1.Id = 449;
			this.definedNameListItem1.Name = "definedNameListItem1";
			this.definedNameListItem1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.spreadsheetCommandBarButtonItem114.CommandName = "FormulasCreateDefinedNamesFromSelection";
			this.spreadsheetCommandBarButtonItem114.Id = 450;
			this.spreadsheetCommandBarButtonItem114.Name = "spreadsheetCommandBarButtonItem114";
			this.spreadsheetCommandBarButtonItem114.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.spreadsheetCommandBarCheckItem23.CommandName = "ViewShowFormulas";
			this.spreadsheetCommandBarCheckItem23.Id = 451;
			this.spreadsheetCommandBarCheckItem23.Name = "spreadsheetCommandBarCheckItem23";
			this.spreadsheetCommandBarSubItem24.CommandName = "FormulasCalculationOptionsCommandGroup";
			this.spreadsheetCommandBarSubItem24.Id = 452;
			this.spreadsheetCommandBarSubItem24.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem24),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem25)});
			this.spreadsheetCommandBarSubItem24.Name = "spreadsheetCommandBarSubItem24";
			this.spreadsheetCommandBarCheckItem24.CommandName = "FormulasCalculationModeAutomatic";
			this.spreadsheetCommandBarCheckItem24.Id = 453;
			this.spreadsheetCommandBarCheckItem24.Name = "spreadsheetCommandBarCheckItem24";
			this.spreadsheetCommandBarCheckItem25.CommandName = "FormulasCalculationModeManual";
			this.spreadsheetCommandBarCheckItem25.Id = 454;
			this.spreadsheetCommandBarCheckItem25.Name = "spreadsheetCommandBarCheckItem25";
			this.spreadsheetCommandBarButtonItem115.CommandName = "FormulasCalculateNow";
			this.spreadsheetCommandBarButtonItem115.Id = 455;
			this.spreadsheetCommandBarButtonItem115.Name = "spreadsheetCommandBarButtonItem115";
			this.spreadsheetCommandBarButtonItem115.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.spreadsheetCommandBarButtonItem116.CommandName = "FormulasCalculateSheet";
			this.spreadsheetCommandBarButtonItem116.Id = 456;
			this.spreadsheetCommandBarButtonItem116.Name = "spreadsheetCommandBarButtonItem116";
			this.spreadsheetCommandBarButtonItem116.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.spreadsheetCommandBarButtonItem117.CommandName = "ReviewUnprotectSheet";
			this.spreadsheetCommandBarButtonItem117.Id = 457;
			this.spreadsheetCommandBarButtonItem117.Name = "spreadsheetCommandBarButtonItem117";
			this.spreadsheetCommandBarButtonItem118.CommandName = "ReviewProtectWorkbook";
			this.spreadsheetCommandBarButtonItem118.Id = 458;
			this.spreadsheetCommandBarButtonItem118.Name = "spreadsheetCommandBarButtonItem118";
			this.spreadsheetCommandBarButtonItem119.CommandName = "ReviewUnprotectWorkbook";
			this.spreadsheetCommandBarButtonItem119.Id = 459;
			this.spreadsheetCommandBarButtonItem119.Name = "spreadsheetCommandBarButtonItem119";
			this.spreadsheetCommandBarButtonItem120.CommandName = "ReviewShowProtectedRangeManager";
			this.spreadsheetCommandBarButtonItem120.Id = 460;
			this.spreadsheetCommandBarButtonItem120.Name = "spreadsheetCommandBarButtonItem120";
			this.spreadsheetCommandBarButtonItem121.CommandName = "ViewZoomOut";
			this.spreadsheetCommandBarButtonItem121.Id = 461;
			this.spreadsheetCommandBarButtonItem121.Name = "spreadsheetCommandBarButtonItem121";
			this.spreadsheetCommandBarButtonItem122.CommandName = "ViewZoomIn";
			this.spreadsheetCommandBarButtonItem122.Id = 462;
			this.spreadsheetCommandBarButtonItem122.Name = "spreadsheetCommandBarButtonItem122";
			this.spreadsheetCommandBarButtonItem123.CommandName = "ViewZoom100Percent";
			this.spreadsheetCommandBarButtonItem123.Id = 463;
			this.spreadsheetCommandBarButtonItem123.Name = "spreadsheetCommandBarButtonItem123";
			this.spreadsheetCommandBarSubItem25.CommandName = "ViewFreezePanesCommandGroup";
			this.spreadsheetCommandBarSubItem25.Id = 464;
			this.spreadsheetCommandBarSubItem25.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem124),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem125),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem126),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem127)});
			this.spreadsheetCommandBarSubItem25.Name = "spreadsheetCommandBarSubItem25";
			this.spreadsheetCommandBarButtonItem124.CommandName = "ViewFreezePanes";
			this.spreadsheetCommandBarButtonItem124.Id = 465;
			this.spreadsheetCommandBarButtonItem124.Name = "spreadsheetCommandBarButtonItem124";
			this.spreadsheetCommandBarButtonItem125.CommandName = "ViewUnfreezePanes";
			this.spreadsheetCommandBarButtonItem125.Id = 466;
			this.spreadsheetCommandBarButtonItem125.Name = "spreadsheetCommandBarButtonItem125";
			this.spreadsheetCommandBarButtonItem126.CommandName = "ViewFreezeTopRow";
			this.spreadsheetCommandBarButtonItem126.Id = 467;
			this.spreadsheetCommandBarButtonItem126.Name = "spreadsheetCommandBarButtonItem126";
			this.spreadsheetCommandBarButtonItem127.CommandName = "ViewFreezeFirstColumn";
			this.spreadsheetCommandBarButtonItem127.Id = 468;
			this.spreadsheetCommandBarButtonItem127.Name = "spreadsheetCommandBarButtonItem127";
			this.spreadsheetCommandBarButtonItem128.CommandName = "ChartChangeType";
			this.spreadsheetCommandBarButtonItem128.Id = 469;
			this.spreadsheetCommandBarButtonItem128.Name = "spreadsheetCommandBarButtonItem128";
			this.spreadsheetCommandBarButtonItem129.CommandName = "ChartSwitchRowColumn";
			this.spreadsheetCommandBarButtonItem129.Id = 470;
			this.spreadsheetCommandBarButtonItem129.Name = "spreadsheetCommandBarButtonItem129";
			this.spreadsheetCommandBarButtonItem130.CommandName = "ChartSelectData";
			this.spreadsheetCommandBarButtonItem130.Id = 471;
			this.spreadsheetCommandBarButtonItem130.Name = "spreadsheetCommandBarButtonItem130";
			this.galleryChartLayoutItem1.Gallery.ColumnCount = 6;
			this.galleryChartLayoutItem1.Gallery.DrawImageBackground = false;
			this.galleryChartLayoutItem1.Gallery.ImageSize = new System.Drawing.Size(48, 48);
			this.galleryChartLayoutItem1.Gallery.RowCount = 2;
			this.galleryChartLayoutItem1.Id = 472;
			this.galleryChartLayoutItem1.Name = "galleryChartLayoutItem1";
			this.galleryChartStyleItem1.Gallery.ColumnCount = 8;
			this.galleryChartStyleItem1.Gallery.DrawImageBackground = false;
			this.galleryChartStyleItem1.Gallery.ImageSize = new System.Drawing.Size(65, 46);
			this.galleryChartStyleItem1.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
			this.galleryChartStyleItem1.Gallery.ItemSize = new System.Drawing.Size(93, 56);
			this.galleryChartStyleItem1.Gallery.MinimumColumnCount = 4;
			this.galleryChartStyleItem1.Gallery.RowCount = 6;
			this.galleryChartStyleItem1.Id = 473;
			this.galleryChartStyleItem1.Name = "galleryChartStyleItem1";
			this.spreadsheetCommandBarButtonGalleryDropDownItem11.CommandName = "ChartTitleCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem11.DropDownControl = this.commandBarGalleryDropDown37;
			this.spreadsheetCommandBarButtonGalleryDropDownItem11.Id = 474;
			this.spreadsheetCommandBarButtonGalleryDropDownItem11.Name = "spreadsheetCommandBarButtonGalleryDropDownItem11";
			this.commandBarGalleryDropDown37.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown37.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup29.CommandName = "ChartTitleCommandGroup";
			spreadsheetCommandGalleryItem121.CommandName = "ChartTitleNone";
			spreadsheetCommandGalleryItem122.CommandName = "ChartTitleCenteredOverlay";
			spreadsheetCommandGalleryItem123.CommandName = "ChartTitleAbove";
			spreadsheetCommandGalleryItemGroup29.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem121,
			spreadsheetCommandGalleryItem122,
			spreadsheetCommandGalleryItem123});
			this.commandBarGalleryDropDown37.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup29});
			this.commandBarGalleryDropDown37.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown37.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown37.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown37.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown37.Name = "commandBarGalleryDropDown37";
			this.commandBarGalleryDropDown37.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarSubItem26.CommandName = "ChartAxisTitlesCommandGroup";
			this.spreadsheetCommandBarSubItem26.Id = 475;
			this.spreadsheetCommandBarSubItem26.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem12),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem13)});
			this.spreadsheetCommandBarSubItem26.Name = "spreadsheetCommandBarSubItem26";
			this.spreadsheetCommandBarButtonGalleryDropDownItem12.CommandName = "ChartPrimaryHorizontalAxisTitleCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem12.DropDownControl = this.commandBarGalleryDropDown38;
			this.spreadsheetCommandBarButtonGalleryDropDownItem12.Id = 476;
			this.spreadsheetCommandBarButtonGalleryDropDownItem12.Name = "spreadsheetCommandBarButtonGalleryDropDownItem12";
			this.commandBarGalleryDropDown38.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown38.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup30.CommandName = "ChartPrimaryHorizontalAxisTitleCommandGroup";
			spreadsheetCommandGalleryItem124.CommandName = "ChartPrimaryHorizontalAxisTitleNone";
			spreadsheetCommandGalleryItem125.CommandName = "ChartPrimaryHorizontalAxisTitleBelow";
			spreadsheetCommandGalleryItemGroup30.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem124,
			spreadsheetCommandGalleryItem125});
			this.commandBarGalleryDropDown38.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup30});
			this.commandBarGalleryDropDown38.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown38.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown38.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown38.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown38.Name = "commandBarGalleryDropDown38";
			this.commandBarGalleryDropDown38.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem13.CommandName = "ChartPrimaryVerticalAxisTitleCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem13.DropDownControl = this.commandBarGalleryDropDown39;
			this.spreadsheetCommandBarButtonGalleryDropDownItem13.Id = 477;
			this.spreadsheetCommandBarButtonGalleryDropDownItem13.Name = "spreadsheetCommandBarButtonGalleryDropDownItem13";
			this.commandBarGalleryDropDown39.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown39.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup31.CommandName = "ChartPrimaryVerticalAxisTitleCommandGroup";
			spreadsheetCommandGalleryItem126.CommandName = "ChartPrimaryVerticalAxisTitleNone";
			spreadsheetCommandGalleryItem127.CommandName = "ChartPrimaryVerticalAxisTitleRotated";
			spreadsheetCommandGalleryItem128.CommandName = "ChartPrimaryVerticalAxisTitleVertical";
			spreadsheetCommandGalleryItem129.CommandName = "ChartPrimaryVerticalAxisTitleHorizontal";
			spreadsheetCommandGalleryItemGroup31.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem126,
			spreadsheetCommandGalleryItem127,
			spreadsheetCommandGalleryItem128,
			spreadsheetCommandGalleryItem129});
			this.commandBarGalleryDropDown39.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup31});
			this.commandBarGalleryDropDown39.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown39.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown39.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown39.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown39.Name = "commandBarGalleryDropDown39";
			this.commandBarGalleryDropDown39.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem14.CommandName = "ChartLegendCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem14.DropDownControl = this.commandBarGalleryDropDown40;
			this.spreadsheetCommandBarButtonGalleryDropDownItem14.Id = 478;
			this.spreadsheetCommandBarButtonGalleryDropDownItem14.Name = "spreadsheetCommandBarButtonGalleryDropDownItem14";
			this.commandBarGalleryDropDown40.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown40.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup32.CommandName = "ChartLegendCommandGroup";
			spreadsheetCommandGalleryItem130.CommandName = "ChartLegendNone";
			spreadsheetCommandGalleryItem131.CommandName = "ChartLegendAtRight";
			spreadsheetCommandGalleryItem132.CommandName = "ChartLegendAtTop";
			spreadsheetCommandGalleryItem133.CommandName = "ChartLegendAtLeft";
			spreadsheetCommandGalleryItem134.CommandName = "ChartLegendAtBottom";
			spreadsheetCommandGalleryItem135.CommandName = "ChartLegendOverlayAtRight";
			spreadsheetCommandGalleryItem136.CommandName = "ChartLegendOverlayAtLeft";
			spreadsheetCommandGalleryItemGroup32.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem130,
			spreadsheetCommandGalleryItem131,
			spreadsheetCommandGalleryItem132,
			spreadsheetCommandGalleryItem133,
			spreadsheetCommandGalleryItem134,
			spreadsheetCommandGalleryItem135,
			spreadsheetCommandGalleryItem136});
			this.commandBarGalleryDropDown40.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup32});
			this.commandBarGalleryDropDown40.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown40.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown40.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown40.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown40.Name = "commandBarGalleryDropDown40";
			this.commandBarGalleryDropDown40.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem15.CommandName = "ChartDataLabelsCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem15.DropDownControl = this.commandBarGalleryDropDown41;
			this.spreadsheetCommandBarButtonGalleryDropDownItem15.Id = 479;
			this.spreadsheetCommandBarButtonGalleryDropDownItem15.Name = "spreadsheetCommandBarButtonGalleryDropDownItem15";
			this.commandBarGalleryDropDown41.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown41.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup33.CommandName = "ChartDataLabelsCommandGroup";
			spreadsheetCommandGalleryItem137.CommandName = "ChartDataLabelsNone";
			spreadsheetCommandGalleryItem138.CommandName = "ChartDataLabelsDefault";
			spreadsheetCommandGalleryItem139.CommandName = "ChartDataLabelsCenter";
			spreadsheetCommandGalleryItem140.CommandName = "ChartDataLabelsInsideEnd";
			spreadsheetCommandGalleryItem141.CommandName = "ChartDataLabelsInsideBase";
			spreadsheetCommandGalleryItem142.CommandName = "ChartDataLabelsOutsideEnd";
			spreadsheetCommandGalleryItem143.CommandName = "ChartDataLabelsBestFit";
			spreadsheetCommandGalleryItem144.CommandName = "ChartDataLabelsLeft";
			spreadsheetCommandGalleryItem145.CommandName = "ChartDataLabelsRight";
			spreadsheetCommandGalleryItem146.CommandName = "ChartDataLabelsAbove";
			spreadsheetCommandGalleryItem147.CommandName = "ChartDataLabelsBelow";
			spreadsheetCommandGalleryItemGroup33.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem137,
			spreadsheetCommandGalleryItem138,
			spreadsheetCommandGalleryItem139,
			spreadsheetCommandGalleryItem140,
			spreadsheetCommandGalleryItem141,
			spreadsheetCommandGalleryItem142,
			spreadsheetCommandGalleryItem143,
			spreadsheetCommandGalleryItem144,
			spreadsheetCommandGalleryItem145,
			spreadsheetCommandGalleryItem146,
			spreadsheetCommandGalleryItem147});
			this.commandBarGalleryDropDown41.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup33});
			this.commandBarGalleryDropDown41.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown41.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown41.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown41.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown41.Name = "commandBarGalleryDropDown41";
			this.commandBarGalleryDropDown41.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarSubItem27.CommandName = "ChartAxesCommandGroup";
			this.spreadsheetCommandBarSubItem27.Id = 480;
			this.spreadsheetCommandBarSubItem27.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem16),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem17)});
			this.spreadsheetCommandBarSubItem27.Name = "spreadsheetCommandBarSubItem27";
			this.spreadsheetCommandBarButtonGalleryDropDownItem16.CommandName = "ChartPrimaryHorizontalAxisCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem16.DropDownControl = this.commandBarGalleryDropDown42;
			this.spreadsheetCommandBarButtonGalleryDropDownItem16.Id = 481;
			this.spreadsheetCommandBarButtonGalleryDropDownItem16.Name = "spreadsheetCommandBarButtonGalleryDropDownItem16";
			this.commandBarGalleryDropDown42.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown42.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup34.CommandName = "ChartPrimaryHorizontalAxisCommandGroup";
			spreadsheetCommandGalleryItem148.CommandName = "ChartHidePrimaryHorizontalAxis";
			spreadsheetCommandGalleryItem149.CommandName = "ChartPrimaryHorizontalAxisLeftToRight";
			spreadsheetCommandGalleryItem150.CommandName = "ChartPrimaryHorizontalAxisHideLabels";
			spreadsheetCommandGalleryItem151.CommandName = "ChartPrimaryHorizontalAxisRightToLeft";
			spreadsheetCommandGalleryItem152.CommandName = "ChartPrimaryHorizontalAxisDefault";
			spreadsheetCommandGalleryItem153.CommandName = "ChartPrimaryHorizontalAxisScaleThousands";
			spreadsheetCommandGalleryItem154.CommandName = "ChartPrimaryHorizontalAxisScaleMillions";
			spreadsheetCommandGalleryItem155.CommandName = "ChartPrimaryHorizontalAxisScaleBillions";
			spreadsheetCommandGalleryItem156.CommandName = "ChartPrimaryHorizontalAxisScaleLogarithm";
			spreadsheetCommandGalleryItemGroup34.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem148,
			spreadsheetCommandGalleryItem149,
			spreadsheetCommandGalleryItem150,
			spreadsheetCommandGalleryItem151,
			spreadsheetCommandGalleryItem152,
			spreadsheetCommandGalleryItem153,
			spreadsheetCommandGalleryItem154,
			spreadsheetCommandGalleryItem155,
			spreadsheetCommandGalleryItem156});
			this.commandBarGalleryDropDown42.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup34});
			this.commandBarGalleryDropDown42.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown42.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown42.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown42.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown42.Name = "commandBarGalleryDropDown42";
			this.commandBarGalleryDropDown42.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem17.CommandName = "ChartPrimaryVerticalAxisCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem17.DropDownControl = this.commandBarGalleryDropDown43;
			this.spreadsheetCommandBarButtonGalleryDropDownItem17.Id = 482;
			this.spreadsheetCommandBarButtonGalleryDropDownItem17.Name = "spreadsheetCommandBarButtonGalleryDropDownItem17";
			this.commandBarGalleryDropDown43.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown43.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup35.CommandName = "ChartPrimaryVerticalAxisCommandGroup";
			spreadsheetCommandGalleryItem157.CommandName = "ChartHidePrimaryVerticalAxis";
			spreadsheetCommandGalleryItem158.CommandName = "ChartPrimaryVerticalAxisLeftToRight";
			spreadsheetCommandGalleryItem159.CommandName = "ChartPrimaryVerticalAxisHideLabels";
			spreadsheetCommandGalleryItem160.CommandName = "ChartPrimaryVerticalAxisRightToLeft";
			spreadsheetCommandGalleryItem161.CommandName = "ChartPrimaryVerticalAxisDefault";
			spreadsheetCommandGalleryItem162.CommandName = "ChartPrimaryVerticalAxisScaleThousands";
			spreadsheetCommandGalleryItem163.CommandName = "ChartPrimaryVerticalAxisScaleMillions";
			spreadsheetCommandGalleryItem164.CommandName = "ChartPrimaryVerticalAxisScaleBillions";
			spreadsheetCommandGalleryItem165.CommandName = "ChartPrimaryVerticalAxisScaleLogarithm";
			spreadsheetCommandGalleryItemGroup35.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem157,
			spreadsheetCommandGalleryItem158,
			spreadsheetCommandGalleryItem159,
			spreadsheetCommandGalleryItem160,
			spreadsheetCommandGalleryItem161,
			spreadsheetCommandGalleryItem162,
			spreadsheetCommandGalleryItem163,
			spreadsheetCommandGalleryItem164,
			spreadsheetCommandGalleryItem165});
			this.commandBarGalleryDropDown43.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup35});
			this.commandBarGalleryDropDown43.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown43.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown43.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown43.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown43.Name = "commandBarGalleryDropDown43";
			this.commandBarGalleryDropDown43.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarSubItem28.CommandName = "ChartGridlinesCommandGroup";
			this.spreadsheetCommandBarSubItem28.Id = 483;
			this.spreadsheetCommandBarSubItem28.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem18),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem19)});
			this.spreadsheetCommandBarSubItem28.Name = "spreadsheetCommandBarSubItem28";
			this.spreadsheetCommandBarButtonGalleryDropDownItem18.CommandName = "ChartPrimaryHorizontalGridlinesCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem18.DropDownControl = this.commandBarGalleryDropDown44;
			this.spreadsheetCommandBarButtonGalleryDropDownItem18.Id = 484;
			this.spreadsheetCommandBarButtonGalleryDropDownItem18.Name = "spreadsheetCommandBarButtonGalleryDropDownItem18";
			this.commandBarGalleryDropDown44.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown44.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup36.CommandName = "ChartPrimaryHorizontalGridlinesCommandGroup";
			spreadsheetCommandGalleryItem166.CommandName = "ChartPrimaryHorizontalGridlinesNone";
			spreadsheetCommandGalleryItem167.CommandName = "ChartPrimaryHorizontalGridlinesMajor";
			spreadsheetCommandGalleryItem168.CommandName = "ChartPrimaryHorizontalGridlinesMinor";
			spreadsheetCommandGalleryItem169.CommandName = "ChartPrimaryHorizontalGridlinesMajorAndMinor";
			spreadsheetCommandGalleryItemGroup36.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem166,
			spreadsheetCommandGalleryItem167,
			spreadsheetCommandGalleryItem168,
			spreadsheetCommandGalleryItem169});
			this.commandBarGalleryDropDown44.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup36});
			this.commandBarGalleryDropDown44.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown44.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown44.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown44.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown44.Name = "commandBarGalleryDropDown44";
			this.commandBarGalleryDropDown44.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem19.CommandName = "ChartPrimaryVerticalGridlinesCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem19.DropDownControl = this.commandBarGalleryDropDown45;
			this.spreadsheetCommandBarButtonGalleryDropDownItem19.Id = 485;
			this.spreadsheetCommandBarButtonGalleryDropDownItem19.Name = "spreadsheetCommandBarButtonGalleryDropDownItem19";
			this.commandBarGalleryDropDown45.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown45.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup37.CommandName = "ChartPrimaryVerticalGridlinesCommandGroup";
			spreadsheetCommandGalleryItem170.CommandName = "ChartPrimaryVerticalGridlinesNone";
			spreadsheetCommandGalleryItem171.CommandName = "ChartPrimaryVerticalGridlinesMajor";
			spreadsheetCommandGalleryItem172.CommandName = "ChartPrimaryVerticalGridlinesMinor";
			spreadsheetCommandGalleryItem173.CommandName = "ChartPrimaryVerticalGridlinesMajorAndMinor";
			spreadsheetCommandGalleryItemGroup37.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem170,
			spreadsheetCommandGalleryItem171,
			spreadsheetCommandGalleryItem172,
			spreadsheetCommandGalleryItem173});
			this.commandBarGalleryDropDown45.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup37});
			this.commandBarGalleryDropDown45.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown45.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown45.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown45.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown45.Name = "commandBarGalleryDropDown45";
			this.commandBarGalleryDropDown45.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem20.CommandName = "ChartLinesCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem20.DropDownControl = this.commandBarGalleryDropDown46;
			this.spreadsheetCommandBarButtonGalleryDropDownItem20.Id = 486;
			this.spreadsheetCommandBarButtonGalleryDropDownItem20.Name = "spreadsheetCommandBarButtonGalleryDropDownItem20";
			this.commandBarGalleryDropDown46.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown46.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup38.CommandName = "ChartLinesCommandGroup";
			spreadsheetCommandGalleryItem174.CommandName = "ChartLinesNone";
			spreadsheetCommandGalleryItem175.CommandName = "ChartShowDropLines";
			spreadsheetCommandGalleryItem176.CommandName = "ChartShowHighLowLines";
			spreadsheetCommandGalleryItem177.CommandName = "ChartShowDropLinesAndHighLowLines";
			spreadsheetCommandGalleryItem178.CommandName = "ChartShowSeriesLines";
			spreadsheetCommandGalleryItemGroup38.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem174,
			spreadsheetCommandGalleryItem175,
			spreadsheetCommandGalleryItem176,
			spreadsheetCommandGalleryItem177,
			spreadsheetCommandGalleryItem178});
			this.commandBarGalleryDropDown46.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup38});
			this.commandBarGalleryDropDown46.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown46.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown46.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown46.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown46.Name = "commandBarGalleryDropDown46";
			this.commandBarGalleryDropDown46.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem21.CommandName = "ChartUpDownBarsCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem21.DropDownControl = this.commandBarGalleryDropDown47;
			this.spreadsheetCommandBarButtonGalleryDropDownItem21.Id = 487;
			this.spreadsheetCommandBarButtonGalleryDropDownItem21.Name = "spreadsheetCommandBarButtonGalleryDropDownItem21";
			this.commandBarGalleryDropDown47.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown47.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup39.CommandName = "ChartUpDownBarsCommandGroup";
			spreadsheetCommandGalleryItem179.CommandName = "ChartHideUpDownBars";
			spreadsheetCommandGalleryItem180.CommandName = "ChartShowUpDownBars";
			spreadsheetCommandGalleryItemGroup39.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem179,
			spreadsheetCommandGalleryItem180});
			this.commandBarGalleryDropDown47.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup39});
			this.commandBarGalleryDropDown47.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown47.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown47.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown47.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown47.Name = "commandBarGalleryDropDown47";
			this.commandBarGalleryDropDown47.Ribbon = this.ribbonControl1;
			this.spreadsheetCommandBarButtonGalleryDropDownItem22.CommandName = "ChartErrorBarsCommandGroup";
			this.spreadsheetCommandBarButtonGalleryDropDownItem22.DropDownControl = this.commandBarGalleryDropDown48;
			this.spreadsheetCommandBarButtonGalleryDropDownItem22.Id = 488;
			this.spreadsheetCommandBarButtonGalleryDropDownItem22.Name = "spreadsheetCommandBarButtonGalleryDropDownItem22";
			this.commandBarGalleryDropDown48.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown48.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup40.CommandName = "ChartErrorBarsCommandGroup";
			spreadsheetCommandGalleryItem181.CommandName = "ChartErrorBarsNone";
			spreadsheetCommandGalleryItem182.CommandName = "ChartErrorBarsStandardError";
			spreadsheetCommandGalleryItem183.CommandName = "ChartErrorBarsPercentage";
			spreadsheetCommandGalleryItem184.CommandName = "ChartErrorBarsStandardDeviation";
			spreadsheetCommandGalleryItemGroup40.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem181,
			spreadsheetCommandGalleryItem182,
			spreadsheetCommandGalleryItem183,
			spreadsheetCommandGalleryItem184});
			this.commandBarGalleryDropDown48.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup40});
			this.commandBarGalleryDropDown48.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown48.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown48.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown48.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown48.Name = "commandBarGalleryDropDown48";
			this.commandBarGalleryDropDown48.Ribbon = this.ribbonControl1;
			this.barStaticItem1.Caption = "Table Name:";
			this.barStaticItem1.Id = 489;
			this.barStaticItem1.Name = "barStaticItem1";
			this.barStaticItem1.TextAlignment = System.Drawing.StringAlignment.Near;
			this.renameTableItem1.Edit = this.repositoryItemTextEdit2;
			this.renameTableItem1.Id = 490;
			this.renameTableItem1.Name = "renameTableItem1";
			this.repositoryItemTextEdit2.AutoHeight = false;
			this.repositoryItemTextEdit2.Name = "repositoryItemTextEdit2";
			this.spreadsheetCommandBarCheckItem26.CommandName = "TableToolsConvertToRange";
			this.spreadsheetCommandBarCheckItem26.Id = 491;
			this.spreadsheetCommandBarCheckItem26.Name = "spreadsheetCommandBarCheckItem26";
			this.spreadsheetCommandBarCheckItem26.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.spreadsheetCommandBarCheckItem27.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			this.spreadsheetCommandBarCheckItem27.CommandName = "TableToolsToggleHeaderRow";
			this.spreadsheetCommandBarCheckItem27.Id = 492;
			this.spreadsheetCommandBarCheckItem27.Name = "spreadsheetCommandBarCheckItem27";
			this.spreadsheetCommandBarCheckItem28.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			this.spreadsheetCommandBarCheckItem28.CommandName = "TableToolsToggleTotalRow";
			this.spreadsheetCommandBarCheckItem28.Id = 493;
			this.spreadsheetCommandBarCheckItem28.Name = "spreadsheetCommandBarCheckItem28";
			this.spreadsheetCommandBarCheckItem29.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			this.spreadsheetCommandBarCheckItem29.CommandName = "TableToolsToggleBandedColumns";
			this.spreadsheetCommandBarCheckItem29.Id = 494;
			this.spreadsheetCommandBarCheckItem29.Name = "spreadsheetCommandBarCheckItem29";
			this.spreadsheetCommandBarCheckItem30.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			this.spreadsheetCommandBarCheckItem30.CommandName = "TableToolsToggleFirstColumn";
			this.spreadsheetCommandBarCheckItem30.Id = 495;
			this.spreadsheetCommandBarCheckItem30.Name = "spreadsheetCommandBarCheckItem30";
			this.spreadsheetCommandBarCheckItem31.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			this.spreadsheetCommandBarCheckItem31.CommandName = "TableToolsToggleLastColumn";
			this.spreadsheetCommandBarCheckItem31.Id = 496;
			this.spreadsheetCommandBarCheckItem31.Name = "spreadsheetCommandBarCheckItem31";
			this.spreadsheetCommandBarCheckItem32.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			this.spreadsheetCommandBarCheckItem32.CommandName = "TableToolsToggleBandedRows";
			this.spreadsheetCommandBarCheckItem32.Id = 497;
			this.spreadsheetCommandBarCheckItem32.Name = "spreadsheetCommandBarCheckItem32";
			this.galleryTableStylesItem1.Gallery.ColumnCount = 7;
			this.galleryTableStylesItem1.Gallery.DrawImageBackground = false;
			this.galleryTableStylesItem1.Gallery.ImageSize = new System.Drawing.Size(65, 46);
			this.galleryTableStylesItem1.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
			this.galleryTableStylesItem1.Gallery.ItemSize = new System.Drawing.Size(73, 58);
			this.galleryTableStylesItem1.Gallery.RowCount = 10;
			this.galleryTableStylesItem1.Id = 498;
			this.galleryTableStylesItem1.Name = "galleryTableStylesItem1";
			this.chartToolsRibbonPageCategory1.Control = this.spreadsheetControl1;
			this.chartToolsRibbonPageCategory1.Name = "chartToolsRibbonPageCategory1";
			this.chartToolsRibbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
			this.chartsDesignRibbonPage1,
			this.chartsLayoutRibbonPage1,
			this.chartsFormatRibbonPage1});
			this.chartToolsRibbonPageCategory1.Visible = false;
			this.chartsDesignRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.chartsDesignTypeRibbonPageGroup1,
			this.chartsDesignDataRibbonPageGroup1,
			this.chartsDesignLayoutsRibbonPageGroup1,
			this.chartsDesignStylesRibbonPageGroup1});
			this.chartsDesignRibbonPage1.Name = "chartsDesignRibbonPage1";
			this.chartsDesignRibbonPage1.Visible = false;
			this.chartsLayoutRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.chartsLayoutLabelsRibbonPageGroup1,
			this.chartsLayoutAxesRibbonPageGroup1,
			this.chartsLayoutAnalysisRibbonPageGroup1});
			this.chartsLayoutRibbonPage1.Name = "chartsLayoutRibbonPage1";
			this.chartsLayoutRibbonPage1.Visible = false;
			this.chartsFormatRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.chartsFormatArrangeRibbonPageGroup1});
			this.chartsFormatRibbonPage1.Name = "chartsFormatRibbonPage1";
			this.chartsFormatRibbonPage1.Visible = false;
			this.tableToolsRibbonPageCategory1.Control = this.spreadsheetControl1;
			this.tableToolsRibbonPageCategory1.Name = "tableToolsRibbonPageCategory1";
			this.tableToolsRibbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
			this.tableToolsDesignRibbonPage1});
			this.tableToolsRibbonPageCategory1.Visible = false;
			this.tableToolsDesignRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.tablePropertiesRibbonPageGroup1,
			this.tableToolsRibbonPageGroup1,
			this.tableStyleOptionsRibbonPageGroup1,
			this.tableStylesRibbonPageGroup1});
			this.tableToolsDesignRibbonPage1.Name = "tableToolsDesignRibbonPage1";
			this.tableToolsDesignRibbonPage1.Visible = false;
			this.pictureToolsRibbonPageCategory1.Control = this.spreadsheetControl1;
			this.pictureToolsRibbonPageCategory1.Name = "pictureToolsRibbonPageCategory1";
			this.pictureToolsRibbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
			this.pictureFormatRibbonPage1});
			this.pictureToolsRibbonPageCategory1.Visible = false;
			this.pictureFormatRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.pictureFormatArrangeRibbonPageGroup1});
			this.pictureFormatRibbonPage1.Name = "pictureFormatRibbonPage1";
			this.pictureFormatRibbonPage1.Visible = false;
			this.drawingToolsRibbonPageCategory1.Control = this.spreadsheetControl1;
			this.drawingToolsRibbonPageCategory1.Name = "drawingToolsRibbonPageCategory1";
			this.drawingToolsRibbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
			this.drawingFormatRibbonPage1});
			this.drawingToolsRibbonPageCategory1.Visible = false;
			this.drawingFormatRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.drawingFormatArrangeRibbonPageGroup1});
			this.drawingFormatRibbonPage1.Name = "drawingFormatRibbonPage1";
			this.drawingFormatRibbonPage1.Visible = false;
			this.fileRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.commonRibbonPageGroup1,
			this.infoRibbonPageGroup1});
			this.fileRibbonPage1.Name = "fileRibbonPage1";
			this.homeRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.clipboardRibbonPageGroup1,
			this.fontRibbonPageGroup1,
			this.alignmentRibbonPageGroup1,
			this.numberRibbonPageGroup1,
			this.stylesRibbonPageGroup1,
			this.cellsRibbonPageGroup1,
			this.editingRibbonPageGroup1});
			this.homeRibbonPage1.Name = "homeRibbonPage1";
			reduceOperation1.Behavior = DevExpress.XtraBars.Ribbon.ReduceOperationBehavior.UntilAvailable;
			reduceOperation1.ItemLinkIndex = 2;
			reduceOperation1.ItemLinksCount = 0;
			reduceOperation1.Operation = DevExpress.XtraBars.Ribbon.ReduceOperationType.Gallery;
			reduceOperation2.Behavior = DevExpress.XtraBars.Ribbon.ReduceOperationBehavior.UntilAvailable;
			reduceOperation2.Group = this.stylesRibbonPageGroup1;
			reduceOperation2.ItemLinkIndex = 2;
			reduceOperation2.ItemLinksCount = 0;
			reduceOperation2.Operation = DevExpress.XtraBars.Ribbon.ReduceOperationType.Gallery;
			this.homeRibbonPage1.ReduceOperations.Add(reduceOperation1);
			this.homeRibbonPage1.ReduceOperations.Add(reduceOperation2);
			this.insertRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.tablesRibbonPageGroup1,
			this.illustrationsRibbonPageGroup1,
			this.chartsRibbonPageGroup1,
			this.linksRibbonPageGroup1,
			this.symbolsRibbonPageGroup1});
			this.insertRibbonPage1.Name = "insertRibbonPage1";
			this.pageLayoutRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.pageSetupRibbonPageGroup1,
			this.pageSetupShowRibbonPageGroup1,
			this.pageSetupPrintRibbonPageGroup1,
			this.arrangeRibbonPageGroup1});
			this.pageLayoutRibbonPage1.Name = "pageLayoutRibbonPage1";
			this.formulasRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.functionLibraryRibbonPageGroup1,
			this.formulaDefinedNamesRibbonPageGroup1,
			this.formulaAuditingRibbonPageGroup1,
			this.formulaCalculationRibbonPageGroup1});
			this.formulasRibbonPage1.Name = "formulasRibbonPage1";
			this.dataRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.sortAndFilterRibbonPageGroup1,
			this.outlineRibbonPageGroup1});
			this.dataRibbonPage1.Name = "dataRibbonPage1";
			this.reviewRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.commentsRibbonPageGroup1,
			this.changesRibbonPageGroup1});
			this.reviewRibbonPage1.Name = "reviewRibbonPage1";
			this.viewRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.showRibbonPageGroup1,
			this.zoomRibbonPageGroup1,
			this.windowRibbonPageGroup1});
			this.viewRibbonPage1.Name = "viewRibbonPage1";
			this.repositoryItemFontEdit1.AutoHeight = false;
			this.repositoryItemFontEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemFontEdit1.Name = "repositoryItemFontEdit1";
			this.repositoryItemSpreadsheetFontSizeEdit1.AutoHeight = false;
			this.repositoryItemSpreadsheetFontSizeEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemSpreadsheetFontSizeEdit1.Control = this.spreadsheetControl1;
			this.repositoryItemSpreadsheetFontSizeEdit1.Name = "repositoryItemSpreadsheetFontSizeEdit1";
			this.repositoryItemPopupGalleryEdit1.AutoHeight = false;
			this.repositoryItemPopupGalleryEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemPopupGalleryEdit1.Gallery.AllowFilter = false;
			this.repositoryItemPopupGalleryEdit1.Gallery.AutoFitColumns = false;
			this.repositoryItemPopupGalleryEdit1.Gallery.ColumnCount = 1;
			this.repositoryItemPopupGalleryEdit1.Gallery.FixedImageSize = false;
			spreadsheetCommandGalleryItem185.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem185.Caption = "General";
			spreadsheetCommandGalleryItem185.CaptionAsValue = true;
			spreadsheetCommandGalleryItem185.CommandName = "FormatNumberGeneral";
			spreadsheetCommandGalleryItem185.Description = "No specific format.";
			spreadsheetCommandGalleryItem185.IsEmptyHint = true;
			spreadsheetCommandGalleryItem185.Value = "General";
			spreadsheetCommandGalleryItem186.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem186.Caption = "Number";
			spreadsheetCommandGalleryItem186.CaptionAsValue = true;
			spreadsheetCommandGalleryItem186.CommandName = "FormatNumberDecimal";
			spreadsheetCommandGalleryItem186.IsEmptyHint = true;
			spreadsheetCommandGalleryItem186.Value = "Number";
			spreadsheetCommandGalleryItem187.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem187.Caption = "Currency";
			spreadsheetCommandGalleryItem187.CaptionAsValue = true;
			spreadsheetCommandGalleryItem187.CommandName = "FormatNumberAccountingCurrency";
			spreadsheetCommandGalleryItem187.IsEmptyHint = true;
			spreadsheetCommandGalleryItem187.Value = "Currency";
			spreadsheetCommandGalleryItem188.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem188.Caption = "Accounting";
			spreadsheetCommandGalleryItem188.CaptionAsValue = true;
			spreadsheetCommandGalleryItem188.CommandName = "FormatNumberAccountingRegular";
			spreadsheetCommandGalleryItem188.IsEmptyHint = true;
			spreadsheetCommandGalleryItem188.Value = "Accounting";
			spreadsheetCommandGalleryItem189.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem189.Caption = "Short Date";
			spreadsheetCommandGalleryItem189.CaptionAsValue = true;
			spreadsheetCommandGalleryItem189.CommandName = "FormatNumberShortDate";
			spreadsheetCommandGalleryItem189.IsEmptyHint = true;
			spreadsheetCommandGalleryItem189.Value = "Short Date";
			spreadsheetCommandGalleryItem190.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem190.Caption = "Long Date";
			spreadsheetCommandGalleryItem190.CaptionAsValue = true;
			spreadsheetCommandGalleryItem190.CommandName = "FormatNumberLongDate";
			spreadsheetCommandGalleryItem190.IsEmptyHint = true;
			spreadsheetCommandGalleryItem190.Value = "Long Date";
			spreadsheetCommandGalleryItem191.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem191.Caption = "Time";
			spreadsheetCommandGalleryItem191.CaptionAsValue = true;
			spreadsheetCommandGalleryItem191.CommandName = "FormatNumberTime";
			spreadsheetCommandGalleryItem191.IsEmptyHint = true;
			spreadsheetCommandGalleryItem191.Value = "Time";
			spreadsheetCommandGalleryItem192.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem192.Caption = "Percentage";
			spreadsheetCommandGalleryItem192.CaptionAsValue = true;
			spreadsheetCommandGalleryItem192.CommandName = "FormatNumberPercentage";
			spreadsheetCommandGalleryItem192.IsEmptyHint = true;
			spreadsheetCommandGalleryItem192.Value = "Percentage";
			spreadsheetCommandGalleryItem193.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem193.Caption = "Fraction";
			spreadsheetCommandGalleryItem193.CaptionAsValue = true;
			spreadsheetCommandGalleryItem193.CommandName = "FormatNumberFraction";
			spreadsheetCommandGalleryItem193.IsEmptyHint = true;
			spreadsheetCommandGalleryItem193.Value = "Fraction";
			spreadsheetCommandGalleryItem194.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem194.Caption = "Scientific";
			spreadsheetCommandGalleryItem194.CaptionAsValue = true;
			spreadsheetCommandGalleryItem194.CommandName = "FormatNumberScientific";
			spreadsheetCommandGalleryItem194.IsEmptyHint = true;
			spreadsheetCommandGalleryItem194.Value = "Scientific";
			spreadsheetCommandGalleryItem195.AlwaysUpdateDescription = true;
			spreadsheetCommandGalleryItem195.Caption = "Text";
			spreadsheetCommandGalleryItem195.CaptionAsValue = true;
			spreadsheetCommandGalleryItem195.CommandName = "FormatNumberText";
			spreadsheetCommandGalleryItem195.IsEmptyHint = true;
			spreadsheetCommandGalleryItem195.Value = "Text";
			galleryItemGroup2.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem185,
			spreadsheetCommandGalleryItem186,
			spreadsheetCommandGalleryItem187,
			spreadsheetCommandGalleryItem188,
			spreadsheetCommandGalleryItem189,
			spreadsheetCommandGalleryItem190,
			spreadsheetCommandGalleryItem191,
			spreadsheetCommandGalleryItem192,
			spreadsheetCommandGalleryItem193,
			spreadsheetCommandGalleryItem194,
			spreadsheetCommandGalleryItem195});
			this.repositoryItemPopupGalleryEdit1.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			galleryItemGroup2});
			this.repositoryItemPopupGalleryEdit1.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.repositoryItemPopupGalleryEdit1.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.repositoryItemPopupGalleryEdit1.Gallery.RowCount = 11;
			this.repositoryItemPopupGalleryEdit1.Gallery.ShowGroupCaption = false;
			this.repositoryItemPopupGalleryEdit1.Gallery.ShowItemText = true;
			this.repositoryItemPopupGalleryEdit1.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Hide;
			this.repositoryItemPopupGalleryEdit1.Gallery.StretchItems = true;
			this.repositoryItemPopupGalleryEdit1.Name = "repositoryItemPopupGalleryEdit1";
			this.repositoryItemPopupGalleryEdit1.ShowButtons = false;
			this.repositoryItemPopupGalleryEdit1.ShowPopupCloseButton = false;
			this.repositoryItemPopupGalleryEdit1.ShowSizeGrip = false;
			this.repositoryItemTextEdit1.AutoHeight = false;
			this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
			this.commandBarGalleryDropDown2.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup41.CommandName = "ConditionalFormattingDataBarsGradientFillCommandGroup";
			spreadsheetCommandGalleryItem196.Caption = "Blue Data Bar (Gradient)";
			spreadsheetCommandGalleryItem196.CommandName = "ConditionalFormattingDataBarGradientBlue";
			spreadsheetCommandGalleryItem196.Description = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem196.Hint = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem197.Caption = "Green Data Bar (Gradient)";
			spreadsheetCommandGalleryItem197.CommandName = "ConditionalFormattingDataBarGradientGreen";
			spreadsheetCommandGalleryItem197.Description = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem197.Hint = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem198.Caption = "Red Data Bar (Gradient)";
			spreadsheetCommandGalleryItem198.CommandName = "ConditionalFormattingDataBarGradientRed";
			spreadsheetCommandGalleryItem198.Description = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem198.Hint = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem199.Caption = "Orange Data Bar (Gradient)";
			spreadsheetCommandGalleryItem199.CommandName = "ConditionalFormattingDataBarGradientOrange";
			spreadsheetCommandGalleryItem199.Description = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem199.Hint = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem200.Caption = "Light Blue Data Bar (Gradient)";
			spreadsheetCommandGalleryItem200.CommandName = "ConditionalFormattingDataBarGradientLightBlue";
			spreadsheetCommandGalleryItem200.Description = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem200.Hint = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem201.Caption = "Purple Data Bar (Gradient)";
			spreadsheetCommandGalleryItem201.CommandName = "ConditionalFormattingDataBarGradientPurple";
			spreadsheetCommandGalleryItem201.Description = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem201.Hint = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItemGroup41.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem196,
			spreadsheetCommandGalleryItem197,
			spreadsheetCommandGalleryItem198,
			spreadsheetCommandGalleryItem199,
			spreadsheetCommandGalleryItem200,
			spreadsheetCommandGalleryItem201});
			spreadsheetCommandGalleryItemGroup42.CommandName = "ConditionalFormattingDataBarsSolidFillCommandGroup";
			spreadsheetCommandGalleryItem202.Caption = "Blue Data Bar (Solid)";
			spreadsheetCommandGalleryItem202.CommandName = "ConditionalFormattingDataBarSolidBlue";
			spreadsheetCommandGalleryItem202.Description = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem202.Hint = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem203.Caption = "Green Data Bar (Solid)";
			spreadsheetCommandGalleryItem203.CommandName = "ConditionalFormattingDataBarSolidGreen";
			spreadsheetCommandGalleryItem203.Description = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem203.Hint = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem204.Caption = "Red Data Bar (Solid)";
			spreadsheetCommandGalleryItem204.CommandName = "ConditionalFormattingDataBarSolidRed";
			spreadsheetCommandGalleryItem204.Description = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem204.Hint = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem205.Caption = "Orange Data Bar (Solid)";
			spreadsheetCommandGalleryItem205.CommandName = "ConditionalFormattingDataBarSolidOrange";
			spreadsheetCommandGalleryItem205.Description = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem205.Hint = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem206.Caption = "Light Blue Data Bar (Solid)";
			spreadsheetCommandGalleryItem206.CommandName = "ConditionalFormattingDataBarSolidLightBlue";
			spreadsheetCommandGalleryItem206.Description = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem206.Hint = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem207.Caption = "Purple Data Bar (Solid)";
			spreadsheetCommandGalleryItem207.CommandName = "ConditionalFormattingDataBarSolidPurple";
			spreadsheetCommandGalleryItem207.Description = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItem207.Hint = "View a colored data bar in the cell. The length of the data bar represents the va" +
	"lue in the cell. A longer bar represents a higher value.";
			spreadsheetCommandGalleryItemGroup42.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem202,
			spreadsheetCommandGalleryItem203,
			spreadsheetCommandGalleryItem204,
			spreadsheetCommandGalleryItem205,
			spreadsheetCommandGalleryItem206,
			spreadsheetCommandGalleryItem207});
			this.commandBarGalleryDropDown2.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup41,
			spreadsheetCommandGalleryItemGroup42});
			this.commandBarGalleryDropDown2.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown2.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown2.Name = "commandBarGalleryDropDown2";
			this.commandBarGalleryDropDown2.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown1.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown1.Gallery.ColumnCount = 1;
			this.commandBarGalleryDropDown1.Gallery.DrawImageBackground = false;
			this.commandBarGalleryDropDown1.Gallery.ImageSize = new System.Drawing.Size(65, 46);
			this.commandBarGalleryDropDown1.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
			this.commandBarGalleryDropDown1.Gallery.ItemSize = new System.Drawing.Size(136, 26);
			this.commandBarGalleryDropDown1.Gallery.RowCount = 14;
			this.commandBarGalleryDropDown1.Gallery.ShowGroupCaption = false;
			this.commandBarGalleryDropDown1.Gallery.ShowItemText = true;
			this.commandBarGalleryDropDown1.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown1.Name = "commandBarGalleryDropDown1";
			this.commandBarGalleryDropDown1.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown3.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup43.CommandName = "ConditionalFormattingColorScalesCommandGroup";
			spreadsheetCommandGalleryItem208.Caption = "Green - Yellow - Red Color Scale";
			spreadsheetCommandGalleryItem208.CommandName = "ConditionalFormattingColorScaleGreenYellowRed";
			spreadsheetCommandGalleryItem208.Description = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem208.Hint = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem209.Caption = "Red - Yellow - Green Color Scale";
			spreadsheetCommandGalleryItem209.CommandName = "ConditionalFormattingColorScaleRedYellowGreen";
			spreadsheetCommandGalleryItem209.Description = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem209.Hint = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem210.Caption = "Green - White - Red Color Scale";
			spreadsheetCommandGalleryItem210.CommandName = "ConditionalFormattingColorScaleGreenWhiteRed";
			spreadsheetCommandGalleryItem210.Description = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem210.Hint = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem211.Caption = "Red - White - Green Color Scale";
			spreadsheetCommandGalleryItem211.CommandName = "ConditionalFormattingColorScaleRedWhiteGreen";
			spreadsheetCommandGalleryItem211.Description = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem211.Hint = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem212.Caption = "Blue - White - Red Color Scale";
			spreadsheetCommandGalleryItem212.CommandName = "ConditionalFormattingColorScaleBlueWhiteRed";
			spreadsheetCommandGalleryItem212.Description = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem212.Hint = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem213.Caption = "Red - White - Blue Color Scale";
			spreadsheetCommandGalleryItem213.CommandName = "ConditionalFormattingColorScaleRedWhiteBlue";
			spreadsheetCommandGalleryItem213.Description = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem213.Hint = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem214.Caption = "White - Red Color Scale";
			spreadsheetCommandGalleryItem214.CommandName = "ConditionalFormattingColorScaleWhiteRed";
			spreadsheetCommandGalleryItem214.Description = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem214.Hint = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem215.Caption = "Red - White Color Scale";
			spreadsheetCommandGalleryItem215.CommandName = "ConditionalFormattingColorScaleRedWhite";
			spreadsheetCommandGalleryItem215.Description = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem215.Hint = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem216.Caption = "Green - White Color Scale";
			spreadsheetCommandGalleryItem216.CommandName = "ConditionalFormattingColorScaleGreenWhite";
			spreadsheetCommandGalleryItem216.Description = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem216.Hint = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem217.Caption = "White - Green Color Scale";
			spreadsheetCommandGalleryItem217.CommandName = "ConditionalFormattingColorScaleWhiteGreen";
			spreadsheetCommandGalleryItem217.Description = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem217.Hint = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem218.Caption = "Green - Yellow Color Scale";
			spreadsheetCommandGalleryItem218.CommandName = "ConditionalFormattingColorScaleGreenYellow";
			spreadsheetCommandGalleryItem218.Description = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem218.Hint = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem219.Caption = "Yellow - Green Color Scale";
			spreadsheetCommandGalleryItem219.CommandName = "ConditionalFormattingColorScaleYellowGreen";
			spreadsheetCommandGalleryItem219.Description = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItem219.Hint = "Displays a two or three color gradient in a range of cells. The shade of the colo" +
	"r represents the value in the cell.";
			spreadsheetCommandGalleryItemGroup43.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem208,
			spreadsheetCommandGalleryItem209,
			spreadsheetCommandGalleryItem210,
			spreadsheetCommandGalleryItem211,
			spreadsheetCommandGalleryItem212,
			spreadsheetCommandGalleryItem213,
			spreadsheetCommandGalleryItem214,
			spreadsheetCommandGalleryItem215,
			spreadsheetCommandGalleryItem216,
			spreadsheetCommandGalleryItem217,
			spreadsheetCommandGalleryItem218,
			spreadsheetCommandGalleryItem219});
			this.commandBarGalleryDropDown3.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup43});
			this.commandBarGalleryDropDown3.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown3.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown3.Name = "commandBarGalleryDropDown3";
			this.commandBarGalleryDropDown3.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown4.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup44.CommandName = "ConditionalFormattingIconSetsDirectionalCommandGroup";
			spreadsheetCommandGalleryItem220.Caption = "3 Arrows (Colored)";
			spreadsheetCommandGalleryItem220.CommandName = "ConditionalFormattingIconSetArrows3Colored";
			spreadsheetCommandGalleryItem220.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem220.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem221.Caption = "3 Arrows (Gray)";
			spreadsheetCommandGalleryItem221.CommandName = "ConditionalFormattingIconSetArrows3Grayed";
			spreadsheetCommandGalleryItem221.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem221.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem222.Caption = "4 Arrows (Colored)";
			spreadsheetCommandGalleryItem222.CommandName = "ConditionalFormattingIconSetArrows4Colored";
			spreadsheetCommandGalleryItem222.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem222.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem223.Caption = "4 Arrows (Gray)";
			spreadsheetCommandGalleryItem223.CommandName = "ConditionalFormattingIconSetArrows4Grayed";
			spreadsheetCommandGalleryItem223.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem223.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem224.Caption = "5 Arrows (Colored)";
			spreadsheetCommandGalleryItem224.CommandName = "ConditionalFormattingIconSetArrows5Colored";
			spreadsheetCommandGalleryItem224.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem224.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem225.Caption = "5 Arrows (Gray)";
			spreadsheetCommandGalleryItem225.CommandName = "ConditionalFormattingIconSetArrows5Grayed";
			spreadsheetCommandGalleryItem225.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem225.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem226.Caption = "3 Triangles";
			spreadsheetCommandGalleryItem226.CommandName = "ConditionalFormattingIconSetTriangles3";
			spreadsheetCommandGalleryItem226.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem226.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItemGroup44.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem220,
			spreadsheetCommandGalleryItem221,
			spreadsheetCommandGalleryItem222,
			spreadsheetCommandGalleryItem223,
			spreadsheetCommandGalleryItem224,
			spreadsheetCommandGalleryItem225,
			spreadsheetCommandGalleryItem226});
			spreadsheetCommandGalleryItemGroup45.CommandName = "ConditionalFormattingIconSetsShapesCommandGroup";
			spreadsheetCommandGalleryItem227.Caption = "3 Traffic Lights ()";
			spreadsheetCommandGalleryItem227.CommandName = "ConditionalFormattingIconSetTrafficLights3";
			spreadsheetCommandGalleryItem227.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem227.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem228.Caption = "3 Traffic Lights (Rimmed)";
			spreadsheetCommandGalleryItem228.CommandName = "ConditionalFormattingIconSetTrafficLights3Rimmed";
			spreadsheetCommandGalleryItem228.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem228.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem229.Caption = "4 Traffic Lights";
			spreadsheetCommandGalleryItem229.CommandName = "ConditionalFormattingIconSetTrafficLights4";
			spreadsheetCommandGalleryItem229.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem229.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem230.Caption = "3 Signs";
			spreadsheetCommandGalleryItem230.CommandName = "ConditionalFormattingIconSetSigns3";
			spreadsheetCommandGalleryItem230.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem230.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem231.Caption = "Red To Black";
			spreadsheetCommandGalleryItem231.CommandName = "ConditionalFormattingIconSetRedToBlack";
			spreadsheetCommandGalleryItem231.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem231.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItemGroup45.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem227,
			spreadsheetCommandGalleryItem228,
			spreadsheetCommandGalleryItem229,
			spreadsheetCommandGalleryItem230,
			spreadsheetCommandGalleryItem231});
			spreadsheetCommandGalleryItemGroup46.CommandName = "ConditionalFormattingIconSetsIndicatorsCommandGroup";
			spreadsheetCommandGalleryItem232.Caption = "3 Symbols (Circled)";
			spreadsheetCommandGalleryItem232.CommandName = "ConditionalFormattingIconSetSymbols3Circled";
			spreadsheetCommandGalleryItem232.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem232.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem233.Caption = "3 Symbols (Uncircled)";
			spreadsheetCommandGalleryItem233.CommandName = "ConditionalFormattingIconSetSymbols3";
			spreadsheetCommandGalleryItem233.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem233.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem234.Caption = "3 Flags";
			spreadsheetCommandGalleryItem234.CommandName = "ConditionalFormattingIconSetFlags3";
			spreadsheetCommandGalleryItem234.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem234.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItemGroup46.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem232,
			spreadsheetCommandGalleryItem233,
			spreadsheetCommandGalleryItem234});
			spreadsheetCommandGalleryItemGroup47.CommandName = "ConditionalFormattingIconSetsRatingsCommandGroup";
			spreadsheetCommandGalleryItem235.Caption = "3 Stars";
			spreadsheetCommandGalleryItem235.CommandName = "ConditionalFormattingIconSetStars3";
			spreadsheetCommandGalleryItem235.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem235.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem236.Caption = "4 Ratings";
			spreadsheetCommandGalleryItem236.CommandName = "ConditionalFormattingIconSetRatings4";
			spreadsheetCommandGalleryItem236.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem236.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem237.Caption = "5 Ratings";
			spreadsheetCommandGalleryItem237.CommandName = "ConditionalFormattingIconSetRatings5";
			spreadsheetCommandGalleryItem237.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem237.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem238.Caption = "5 Quarters";
			spreadsheetCommandGalleryItem238.CommandName = "ConditionalFormattingIconSetQuarters5";
			spreadsheetCommandGalleryItem238.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem238.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem239.Caption = "5 Boxes";
			spreadsheetCommandGalleryItem239.CommandName = "ConditionalFormattingIconSetBoxes5";
			spreadsheetCommandGalleryItem239.Description = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItem239.Hint = "Display an icon from the above icon set in each cell. Each icon represents a valu" +
	"e in the cell.";
			spreadsheetCommandGalleryItemGroup47.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem235,
			spreadsheetCommandGalleryItem236,
			spreadsheetCommandGalleryItem237,
			spreadsheetCommandGalleryItem238,
			spreadsheetCommandGalleryItem239});
			this.commandBarGalleryDropDown4.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup44,
			spreadsheetCommandGalleryItemGroup45,
			spreadsheetCommandGalleryItemGroup46,
			spreadsheetCommandGalleryItemGroup47});
			this.commandBarGalleryDropDown4.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown4.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown4.Name = "commandBarGalleryDropDown4";
			this.commandBarGalleryDropDown4.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown5.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown5.Gallery.ColumnCount = 7;
			this.commandBarGalleryDropDown5.Gallery.DrawImageBackground = false;
			this.commandBarGalleryDropDown5.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
			this.commandBarGalleryDropDown5.Gallery.ItemSize = new System.Drawing.Size(73, 58);
			this.commandBarGalleryDropDown5.Gallery.RowCount = 10;
			this.commandBarGalleryDropDown5.Name = "commandBarGalleryDropDown5";
			this.commandBarGalleryDropDown5.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown6.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup48.CommandName = "InsertChartColumn2DCommandGroup";
			spreadsheetCommandGalleryItem240.Caption = "Clustered Column";
			spreadsheetCommandGalleryItem240.CommandName = "InsertChartColumnClustered2D";
			spreadsheetCommandGalleryItem240.Description = "Compare values across categories by using vertical rectangles.\r\n\r\nUse it when the" +
	" order of categories is not important or for displaying item counts such as a hi" +
	"stogram.";
			spreadsheetCommandGalleryItem240.Hint = "Compare values across categories by using vertical rectangles.\r\n\r\nUse it when the" +
	" order of categories is not important or for displaying item counts such as a hi" +
	"stogram.";
			spreadsheetCommandGalleryItem241.Caption = "Stacked Column";
			spreadsheetCommandGalleryItem241.CommandName = "InsertChartColumnStacked2D";
			spreadsheetCommandGalleryItem241.Description = "Compare the contribution of each value to a total across categories by using vert" +
	"ical rectangles.\r\n\r\nUse it to emphasize the total across series for one category" +
	".";
			spreadsheetCommandGalleryItem241.Hint = "Compare the contribution of each value to a total across categories by using vert" +
	"ical rectangles.\r\n\r\nUse it to emphasize the total across series for one category" +
	".";
			spreadsheetCommandGalleryItem242.Caption = "100% Stacked Column";
			spreadsheetCommandGalleryItem242.CommandName = "InsertChartColumnPercentStacked2D";
			spreadsheetCommandGalleryItem242.Description = "Compare the percentage that each value contributes to a total across categories b" +
	"y using vertical rectangles.\r\n\r\nUse is to emphasize the proportion of each data " +
	"series.";
			spreadsheetCommandGalleryItem242.Hint = "Compare the percentage that each value contributes to a total across categories b" +
	"y using vertical rectangles.\r\n\r\nUse is to emphasize the proportion of each data " +
	"series.";
			spreadsheetCommandGalleryItemGroup48.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem240,
			spreadsheetCommandGalleryItem241,
			spreadsheetCommandGalleryItem242});
			spreadsheetCommandGalleryItemGroup49.CommandName = "InsertChartColumn3DCommandGroup";
			spreadsheetCommandGalleryItem243.Caption = "3-D Clustered Column";
			spreadsheetCommandGalleryItem243.CommandName = "InsertChartColumnClustered3D";
			spreadsheetCommandGalleryItem243.Description = "Compare values across categories and display clustered columns in 3-D format.";
			spreadsheetCommandGalleryItem243.Hint = "Compare values across categories and display clustered columns in 3-D format.";
			spreadsheetCommandGalleryItem244.Caption = "Stacked Column in 3-D";
			spreadsheetCommandGalleryItem244.CommandName = "InsertChartColumnStacked3D";
			spreadsheetCommandGalleryItem244.Description = "Compare the contribution of each value to a total across categories and display s" +
	"tacked columns in 3-D format.";
			spreadsheetCommandGalleryItem244.Hint = "Compare the contribution of each value to a total across categories and display s" +
	"tacked columns in 3-D format.";
			spreadsheetCommandGalleryItem245.Caption = "100% Stacked Column in 3-D";
			spreadsheetCommandGalleryItem245.CommandName = "InsertChartColumnPercentStacked3D";
			spreadsheetCommandGalleryItem245.Description = "Compare the percentage that each value contributes to a total across categories a" +
	"nd display 100% stacked columns in 3-D format.";
			spreadsheetCommandGalleryItem245.Hint = "Compare the percentage that each value contributes to a total across categories a" +
	"nd display 100% stacked columns in 3-D format.";
			spreadsheetCommandGalleryItem246.Caption = "3-D Column";
			spreadsheetCommandGalleryItem246.CommandName = "InsertChartColumn3D";
			spreadsheetCommandGalleryItem246.Description = "Compare values across categories and across series on three axes.\r\n\r\nUse it when " +
	"the categories and series are equally important.";
			spreadsheetCommandGalleryItem246.Hint = "Compare values across categories and across series on three axes.\r\n\r\nUse it when " +
	"the categories and series are equally important.";
			spreadsheetCommandGalleryItemGroup49.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem243,
			spreadsheetCommandGalleryItem244,
			spreadsheetCommandGalleryItem245,
			spreadsheetCommandGalleryItem246});
			spreadsheetCommandGalleryItemGroup50.CommandName = "InsertChartCylinderCommandGroup";
			spreadsheetCommandGalleryItem247.Caption = "Clustered Cylinder";
			spreadsheetCommandGalleryItem247.CommandName = "InsertChartCylinderClustered";
			spreadsheetCommandGalleryItem247.Description = "Compare values across categories.";
			spreadsheetCommandGalleryItem247.Hint = "Compare values across categories.";
			spreadsheetCommandGalleryItem248.Caption = "Stacked Cylinder";
			spreadsheetCommandGalleryItem248.CommandName = "InsertChartCylinderStacked";
			spreadsheetCommandGalleryItem248.Description = "Compare the contribution of each value to a total across categories.";
			spreadsheetCommandGalleryItem248.Hint = "Compare the contribution of each value to a total across categories.";
			spreadsheetCommandGalleryItem249.Caption = "100% Stacked Cylinder";
			spreadsheetCommandGalleryItem249.CommandName = "InsertChartCylinderPercentStacked";
			spreadsheetCommandGalleryItem249.Description = "Compare the percentage that each value contributes to a total across categories.";
			spreadsheetCommandGalleryItem249.Hint = "Compare the percentage that each value contributes to a total across categories.";
			spreadsheetCommandGalleryItem250.Caption = "3-D Cylinder";
			spreadsheetCommandGalleryItem250.CommandName = "InsertChartCylinder";
			spreadsheetCommandGalleryItem250.Description = "Compare values across categories and across series and display a cylinder chart o" +
	"n three axes.";
			spreadsheetCommandGalleryItem250.Hint = "Compare values across categories and across series and display a cylinder chart o" +
	"n three axes.";
			spreadsheetCommandGalleryItemGroup50.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem247,
			spreadsheetCommandGalleryItem248,
			spreadsheetCommandGalleryItem249,
			spreadsheetCommandGalleryItem250});
			spreadsheetCommandGalleryItemGroup51.CommandName = "InsertChartConeCommandGroup";
			spreadsheetCommandGalleryItem251.Caption = "Clustered Cone";
			spreadsheetCommandGalleryItem251.CommandName = "InsertChartConeClustered";
			spreadsheetCommandGalleryItem251.Description = "Compare values across categories.";
			spreadsheetCommandGalleryItem251.Hint = "Compare values across categories.";
			spreadsheetCommandGalleryItem252.Caption = "Stacked Cone";
			spreadsheetCommandGalleryItem252.CommandName = "InsertChartConeStacked";
			spreadsheetCommandGalleryItem252.Description = "Compare the contribution of each value to a total across categories.";
			spreadsheetCommandGalleryItem252.Hint = "Compare the contribution of each value to a total across categories.";
			spreadsheetCommandGalleryItem253.Caption = "100% Stacked Cone";
			spreadsheetCommandGalleryItem253.CommandName = "InsertChartConePercentStacked";
			spreadsheetCommandGalleryItem253.Description = "Compare the percentage that each value contributes to a total across categories.";
			spreadsheetCommandGalleryItem253.Hint = "Compare the percentage that each value contributes to a total across categories.";
			spreadsheetCommandGalleryItem254.Caption = "3-D Cone";
			spreadsheetCommandGalleryItem254.CommandName = "InsertChartCone";
			spreadsheetCommandGalleryItem254.Description = "Compare values across categories and across series and display a cone chart on th" +
	"ree axes.";
			spreadsheetCommandGalleryItem254.Hint = "Compare values across categories and across series and display a cone chart on th" +
	"ree axes.";
			spreadsheetCommandGalleryItemGroup51.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem251,
			spreadsheetCommandGalleryItem252,
			spreadsheetCommandGalleryItem253,
			spreadsheetCommandGalleryItem254});
			spreadsheetCommandGalleryItemGroup52.CommandName = "InsertChartPyramidCommandGroup";
			spreadsheetCommandGalleryItem255.Caption = "Clustered Pyramid";
			spreadsheetCommandGalleryItem255.CommandName = "InsertChartPyramidClustered";
			spreadsheetCommandGalleryItem255.Description = "Compare values across categories.";
			spreadsheetCommandGalleryItem255.Hint = "Compare values across categories.";
			spreadsheetCommandGalleryItem256.Caption = "Stacked Pyramid";
			spreadsheetCommandGalleryItem256.CommandName = "InsertChartPyramidStacked";
			spreadsheetCommandGalleryItem256.Description = "Compare the contribution of each value to a total across categories.";
			spreadsheetCommandGalleryItem256.Hint = "Compare the contribution of each value to a total across categories.";
			spreadsheetCommandGalleryItem257.Caption = "100% Stacked Pyramid";
			spreadsheetCommandGalleryItem257.CommandName = "InsertChartPyramidPercentStacked";
			spreadsheetCommandGalleryItem257.Description = "Compare the percentage that each value contributes to a total across categories.";
			spreadsheetCommandGalleryItem257.Hint = "Compare the percentage that each value contributes to a total across categories.";
			spreadsheetCommandGalleryItem258.Caption = "3-D Pyramid";
			spreadsheetCommandGalleryItem258.CommandName = "InsertChartPyramid";
			spreadsheetCommandGalleryItem258.Description = "Compare values across categories and across series and display a pyramid chart on" +
	" three axes.";
			spreadsheetCommandGalleryItem258.Hint = "Compare values across categories and across series and display a pyramid chart on" +
	" three axes.";
			spreadsheetCommandGalleryItemGroup52.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem255,
			spreadsheetCommandGalleryItem256,
			spreadsheetCommandGalleryItem257,
			spreadsheetCommandGalleryItem258});
			this.commandBarGalleryDropDown6.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup48,
			spreadsheetCommandGalleryItemGroup49,
			spreadsheetCommandGalleryItemGroup50,
			spreadsheetCommandGalleryItemGroup51,
			spreadsheetCommandGalleryItemGroup52});
			this.commandBarGalleryDropDown6.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown6.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown6.Name = "commandBarGalleryDropDown6";
			this.commandBarGalleryDropDown6.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown7.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup53.CommandName = "InsertChartLine2DCommandGroup";
			spreadsheetCommandGalleryItem259.Caption = "Line";
			spreadsheetCommandGalleryItem259.CommandName = "InsertChartLine";
			spreadsheetCommandGalleryItem259.Description = "Display trend over time (dates, years) or ordered categories.\r\n\r\nUseful when ther" +
	"e are many data points and the order is important.";
			spreadsheetCommandGalleryItem259.Hint = "Display trend over time (dates, years) or ordered categories.\r\n\r\nUseful when ther" +
	"e are many data points and the order is important.";
			spreadsheetCommandGalleryItem260.Caption = "Stacked Line";
			spreadsheetCommandGalleryItem260.CommandName = "InsertChartStackedLine";
			spreadsheetCommandGalleryItem260.Description = "Display the trend of the contribution of each value over time or ordered categori" +
	"es.\r\n\r\nConsider using a stacked area chart instead.";
			spreadsheetCommandGalleryItem260.Hint = "Display the trend of the contribution of each value over time or ordered categori" +
	"es.\r\n\r\nConsider using a stacked area chart instead.";
			spreadsheetCommandGalleryItem261.Caption = "100% Stacked line";
			spreadsheetCommandGalleryItem261.CommandName = "InsertChartPercentStackedLine";
			spreadsheetCommandGalleryItem261.Description = "Display the trend of the percentage each value contributes over time or ordered c" +
	"ategories.\r\n\r\nConsider using 100% stacked area chart instead.";
			spreadsheetCommandGalleryItem261.Hint = "Display the trend of the percentage each value contributes over time or ordered c" +
	"ategories.\r\n\r\nConsider using 100% stacked area chart instead.";
			spreadsheetCommandGalleryItem262.Caption = "Line with Markers";
			spreadsheetCommandGalleryItem262.CommandName = "InsertChartLineWithMarkers";
			spreadsheetCommandGalleryItem262.Description = "Display trend over time (dates, years) or ordered categories.\r\n\r\nUseful when ther" +
	"e are only a few data points.";
			spreadsheetCommandGalleryItem262.Hint = "Display trend over time (dates, years) or ordered categories.\r\n\r\nUseful when ther" +
	"e are only a few data points.";
			spreadsheetCommandGalleryItem263.Caption = "Stacked Line with Markers";
			spreadsheetCommandGalleryItem263.CommandName = "InsertChartStackedLineWithMarkers";
			spreadsheetCommandGalleryItem263.Description = "Display the trend of the contribution of each value over time or ordered categori" +
	"es.\r\n\r\nConsider using a stacked area chart instead.";
			spreadsheetCommandGalleryItem263.Hint = "Display the trend of the contribution of each value over time or ordered categori" +
	"es.\r\n\r\nConsider using a stacked area chart instead.";
			spreadsheetCommandGalleryItem264.Caption = "100% Stacked Line with Markers";
			spreadsheetCommandGalleryItem264.CommandName = "InsertChartPercentStackedLineWithMarkers";
			spreadsheetCommandGalleryItem264.Description = "Display the trend of the percentage each value contributes over time or ordered c" +
	"ategories.\r\n\r\nConsider using 100% stacked area chart instead.";
			spreadsheetCommandGalleryItem264.Hint = "Display the trend of the percentage each value contributes over time or ordered c" +
	"ategories.\r\n\r\nConsider using 100% stacked area chart instead.";
			spreadsheetCommandGalleryItemGroup53.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem259,
			spreadsheetCommandGalleryItem260,
			spreadsheetCommandGalleryItem261,
			spreadsheetCommandGalleryItem262,
			spreadsheetCommandGalleryItem263,
			spreadsheetCommandGalleryItem264});
			spreadsheetCommandGalleryItemGroup54.CommandName = "InsertChartLine3DCommandGroup";
			spreadsheetCommandGalleryItem265.Caption = "3-D Line";
			spreadsheetCommandGalleryItem265.CommandName = "InsertChartLine3D";
			spreadsheetCommandGalleryItem265.Description = "Display each row or column of data as a 3-D ribbon on three axes.";
			spreadsheetCommandGalleryItem265.Hint = "Display each row or column of data as a 3-D ribbon on three axes.";
			spreadsheetCommandGalleryItemGroup54.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem265});
			this.commandBarGalleryDropDown7.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup53,
			spreadsheetCommandGalleryItemGroup54});
			this.commandBarGalleryDropDown7.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown7.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown7.Name = "commandBarGalleryDropDown7";
			this.commandBarGalleryDropDown7.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown8.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup55.CommandName = "InsertChartPie2DCommandGroup";
			spreadsheetCommandGalleryItem266.Caption = "Pie";
			spreadsheetCommandGalleryItem266.CommandName = "InsertChartPie2D";
			spreadsheetCommandGalleryItem266.Description = "Display the contribution of each value to a total.\r\n\r\nUse it when the values can " +
	"be added together or when you have only one data series and all values are posit" +
	"ive.";
			spreadsheetCommandGalleryItem266.Hint = "Display the contribution of each value to a total.\r\n\r\nUse it when the values can " +
	"be added together or when you have only one data series and all values are posit" +
	"ive.";
			spreadsheetCommandGalleryItem267.Caption = "Exploded Pie";
			spreadsheetCommandGalleryItem267.CommandName = "InsertChartPieExploded2D";
			spreadsheetCommandGalleryItem267.Description = "Display the contribution of each value to a total while emphasizing individual va" +
	"lues.\r\n\r\nConsider using a pie chart, and explode individual values instead.";
			spreadsheetCommandGalleryItem267.Hint = "Display the contribution of each value to a total while emphasizing individual va" +
	"lues.\r\n\r\nConsider using a pie chart, and explode individual values instead.";
			spreadsheetCommandGalleryItemGroup55.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem266,
			spreadsheetCommandGalleryItem267});
			spreadsheetCommandGalleryItemGroup56.CommandName = "InsertChartPie3DCommandGroup";
			spreadsheetCommandGalleryItem268.Caption = "Pie in 3-D";
			spreadsheetCommandGalleryItem268.CommandName = "InsertChartPie3D";
			spreadsheetCommandGalleryItem268.Description = "Display the contribution of each value to a total.";
			spreadsheetCommandGalleryItem268.Hint = "Display the contribution of each value to a total.";
			spreadsheetCommandGalleryItem269.Caption = "Exploded pie in 3-D";
			spreadsheetCommandGalleryItem269.CommandName = "InsertChartPieExploded3D";
			spreadsheetCommandGalleryItem269.Description = "Display the contribution of each value to a total while emphasizing individual va" +
	"lues.\r\n\r\nConsider using a 3-D pie chart, and explode individual values instead.";
			spreadsheetCommandGalleryItem269.Hint = "Display the contribution of each value to a total while emphasizing individual va" +
	"lues.\r\n\r\nConsider using a 3-D pie chart, and explode individual values instead.";
			spreadsheetCommandGalleryItemGroup56.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem268,
			spreadsheetCommandGalleryItem269});
			spreadsheetCommandGalleryItemGroup57.CommandName = "InsertChartDoughnut2DCommandGroup";
			spreadsheetCommandGalleryItem270.Caption = "Doughnut";
			spreadsheetCommandGalleryItem270.CommandName = "InsertChartDoughnut2D";
			spreadsheetCommandGalleryItem270.Description = "Display the contribution of each value to a total like a pie chart, but it can co" +
	"ntain multiple series.";
			spreadsheetCommandGalleryItem270.Hint = "Display the contribution of each value to a total like a pie chart, but it can co" +
	"ntain multiple series.";
			spreadsheetCommandGalleryItem271.Caption = "Exploded Doughnut";
			spreadsheetCommandGalleryItem271.CommandName = "InsertChartDoughnutExploded2D";
			spreadsheetCommandGalleryItem271.Description = "Display the contribution of each value to a total while emphasizing individual va" +
	"lues like an exploded pie chart, but it can contain multiple series.";
			spreadsheetCommandGalleryItem271.Hint = "Display the contribution of each value to a total while emphasizing individual va" +
	"lues like an exploded pie chart, but it can contain multiple series.";
			spreadsheetCommandGalleryItemGroup57.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem270,
			spreadsheetCommandGalleryItem271});
			this.commandBarGalleryDropDown8.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup55,
			spreadsheetCommandGalleryItemGroup56,
			spreadsheetCommandGalleryItemGroup57});
			this.commandBarGalleryDropDown8.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown8.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown8.Name = "commandBarGalleryDropDown8";
			this.commandBarGalleryDropDown8.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown9.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup58.CommandName = "InsertChartBar2DCommandGroup";
			spreadsheetCommandGalleryItem272.Caption = "Clustered Bar";
			spreadsheetCommandGalleryItem272.CommandName = "InsertChartBarClustered2D";
			spreadsheetCommandGalleryItem272.Description = "Compare values across categories using horizontal rectangles.\r\n\r\nUse it when the " +
	"values on the chart represent durations or when the category text is very long.";
			spreadsheetCommandGalleryItem272.Hint = "Compare values across categories using horizontal rectangles.\r\n\r\nUse it when the " +
	"values on the chart represent durations or when the category text is very long.";
			spreadsheetCommandGalleryItem273.Caption = "Stacked Bar";
			spreadsheetCommandGalleryItem273.CommandName = "InsertChartBarStacked2D";
			spreadsheetCommandGalleryItem273.Description = "Compare the contribution of each value to a total across categories by using hori" +
	"zontal rectangles.\r\n\r\nUse it when the values on the chart represent durations or" +
	" when the category text is very long.";
			spreadsheetCommandGalleryItem273.Hint = "Compare the contribution of each value to a total across categories by using hori" +
	"zontal rectangles.\r\n\r\nUse it when the values on the chart represent durations or" +
	" when the category text is very long.";
			spreadsheetCommandGalleryItem274.Caption = "100% Stacked Bar";
			spreadsheetCommandGalleryItem274.CommandName = "InsertChartBarPercentStacked2D";
			spreadsheetCommandGalleryItem274.Description = resources.GetString("spreadsheetCommandGalleryItem274.Description");
			spreadsheetCommandGalleryItem274.Hint = resources.GetString("spreadsheetCommandGalleryItem274.Hint");
			spreadsheetCommandGalleryItemGroup58.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem272,
			spreadsheetCommandGalleryItem273,
			spreadsheetCommandGalleryItem274});
			spreadsheetCommandGalleryItemGroup59.CommandName = "InsertChartBar3DCommandGroup";
			spreadsheetCommandGalleryItem275.Caption = "Clustered Bar in 3-D";
			spreadsheetCommandGalleryItem275.CommandName = "InsertChartBarClustered3D";
			spreadsheetCommandGalleryItem275.Description = "Compare values across categories and display clustered bars in 3-D format.";
			spreadsheetCommandGalleryItem275.Hint = "Compare values across categories and display clustered bars in 3-D format.";
			spreadsheetCommandGalleryItem276.Caption = "Stacked Bar in 3-D";
			spreadsheetCommandGalleryItem276.CommandName = "InsertChartBarStacked3D";
			spreadsheetCommandGalleryItem276.Description = "Compare the contribution of each value to a total across categories and display s" +
	"tacked bars in 3-D format.";
			spreadsheetCommandGalleryItem276.Hint = "Compare the contribution of each value to a total across categories and display s" +
	"tacked bars in 3-D format.";
			spreadsheetCommandGalleryItem277.Caption = "100% Stacked Bar in 3-D";
			spreadsheetCommandGalleryItem277.CommandName = "InsertChartBarPercentStacked3D";
			spreadsheetCommandGalleryItem277.Description = "Compare the percentange each value contributes to a total across categories and d" +
	"isplay 100% stacked bars in 3-D format.";
			spreadsheetCommandGalleryItem277.Hint = "Compare the percentange each value contributes to a total across categories and d" +
	"isplay 100% stacked bars in 3-D format.";
			spreadsheetCommandGalleryItemGroup59.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem275,
			spreadsheetCommandGalleryItem276,
			spreadsheetCommandGalleryItem277});
			spreadsheetCommandGalleryItemGroup60.CommandName = "InsertChartHorizontalCylinderCommandGroup";
			spreadsheetCommandGalleryItem278.Caption = "Clustered Horizontal Cylinder";
			spreadsheetCommandGalleryItem278.CommandName = "InsertChartHorizontalCylinderClustered";
			spreadsheetCommandGalleryItem278.Description = "Compare values across categories.";
			spreadsheetCommandGalleryItem278.Hint = "Compare values across categories.";
			spreadsheetCommandGalleryItem279.Caption = "Stacked Horizontal Cylinder";
			spreadsheetCommandGalleryItem279.CommandName = "InsertChartHorizontalCylinderStacked";
			spreadsheetCommandGalleryItem279.Description = "Compare the contribution of each value to a total across categories.";
			spreadsheetCommandGalleryItem279.Hint = "Compare the contribution of each value to a total across categories.";
			spreadsheetCommandGalleryItem280.Caption = "100% Stacked Horizontal Cylinder";
			spreadsheetCommandGalleryItem280.CommandName = "InsertChartHorizontalCylinderPercentStacked";
			spreadsheetCommandGalleryItem280.Description = "Compare the percentange each value contributes to a total across categories.";
			spreadsheetCommandGalleryItem280.Hint = "Compare the percentange each value contributes to a total across categories.";
			spreadsheetCommandGalleryItemGroup60.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem278,
			spreadsheetCommandGalleryItem279,
			spreadsheetCommandGalleryItem280});
			spreadsheetCommandGalleryItemGroup61.CommandName = "InsertChartHorizontalConeCommandGroup";
			spreadsheetCommandGalleryItem281.Caption = "Clustered Horizontal Cone";
			spreadsheetCommandGalleryItem281.CommandName = "InsertChartHorizontalConeClustered";
			spreadsheetCommandGalleryItem281.Description = "Compare values across categories.";
			spreadsheetCommandGalleryItem281.Hint = "Compare values across categories.";
			spreadsheetCommandGalleryItem282.Caption = "Stacked Horizontal Cone";
			spreadsheetCommandGalleryItem282.CommandName = "InsertChartHorizontalConeStacked";
			spreadsheetCommandGalleryItem282.Description = "Compare the contribution of each value to a total across categories.";
			spreadsheetCommandGalleryItem282.Hint = "Compare the contribution of each value to a total across categories.";
			spreadsheetCommandGalleryItem283.Caption = "100% Stacked Horizontal Cone";
			spreadsheetCommandGalleryItem283.CommandName = "InsertChartHorizontalConePercentStacked";
			spreadsheetCommandGalleryItem283.Description = "Compare the percentange each value contributes to a total across categories.";
			spreadsheetCommandGalleryItem283.Hint = "Compare the percentange each value contributes to a total across categories.";
			spreadsheetCommandGalleryItemGroup61.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem281,
			spreadsheetCommandGalleryItem282,
			spreadsheetCommandGalleryItem283});
			spreadsheetCommandGalleryItemGroup62.CommandName = "InsertChartHorizontalPyramidCommandGroup";
			spreadsheetCommandGalleryItem284.Caption = "Clustered Horizontal Pyramid";
			spreadsheetCommandGalleryItem284.CommandName = "InsertChartHorizontalPyramidClustered";
			spreadsheetCommandGalleryItem284.Description = "Compare values across categories.";
			spreadsheetCommandGalleryItem284.Hint = "Compare values across categories.";
			spreadsheetCommandGalleryItem285.Caption = "Stacked Horizontal Pyramid";
			spreadsheetCommandGalleryItem285.CommandName = "InsertChartHorizontalPyramidStacked";
			spreadsheetCommandGalleryItem285.Description = "Compare the contribution of each value to a total across categories.";
			spreadsheetCommandGalleryItem285.Hint = "Compare the contribution of each value to a total across categories.";
			spreadsheetCommandGalleryItem286.Caption = "100% Stacked Horizontal Pyramid";
			spreadsheetCommandGalleryItem286.CommandName = "InsertChartHorizontalPyramidPercentStacked";
			spreadsheetCommandGalleryItem286.Description = "Compare the percentange each value contributes to a total across categories.";
			spreadsheetCommandGalleryItem286.Hint = "Compare the percentange each value contributes to a total across categories.";
			spreadsheetCommandGalleryItemGroup62.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem284,
			spreadsheetCommandGalleryItem285,
			spreadsheetCommandGalleryItem286});
			this.commandBarGalleryDropDown9.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup58,
			spreadsheetCommandGalleryItemGroup59,
			spreadsheetCommandGalleryItemGroup60,
			spreadsheetCommandGalleryItemGroup61,
			spreadsheetCommandGalleryItemGroup62});
			this.commandBarGalleryDropDown9.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown9.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown9.Name = "commandBarGalleryDropDown9";
			this.commandBarGalleryDropDown9.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown10.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup63.CommandName = "InsertChartArea2DCommandGroup";
			spreadsheetCommandGalleryItem287.Caption = "Area";
			spreadsheetCommandGalleryItem287.CommandName = "InsertChartArea";
			spreadsheetCommandGalleryItem287.Description = "Display the trend of values over time or categories.";
			spreadsheetCommandGalleryItem287.Hint = "Display the trend of values over time or categories.";
			spreadsheetCommandGalleryItem288.Caption = "Stacked Area";
			spreadsheetCommandGalleryItem288.CommandName = "InsertChartStackedArea";
			spreadsheetCommandGalleryItem288.Description = "Display the trend of the contribution of each value over time or categories.\r\n\r\nU" +
	"se it to emphasize the trend in the total across series for one category.";
			spreadsheetCommandGalleryItem288.Hint = "Display the trend of the contribution of each value over time or categories.\r\n\r\nU" +
	"se it to emphasize the trend in the total across series for one category.";
			spreadsheetCommandGalleryItem289.Caption = "100% Stacked Area";
			spreadsheetCommandGalleryItem289.CommandName = "InsertChartPercentStackedArea";
			spreadsheetCommandGalleryItem289.Description = "Display the trend of the percentage each value contibutes over time or categories" +
	".\r\n\r\nUse it to emphasize the trend in the proportion of each series.";
			spreadsheetCommandGalleryItem289.Hint = "Display the trend of the percentage each value contibutes over time or categories" +
	".\r\n\r\nUse it to emphasize the trend in the proportion of each series.";
			spreadsheetCommandGalleryItemGroup63.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem287,
			spreadsheetCommandGalleryItem288,
			spreadsheetCommandGalleryItem289});
			spreadsheetCommandGalleryItemGroup64.CommandName = "InsertChartArea3DCommandGroup";
			spreadsheetCommandGalleryItem290.Caption = "3-D Area";
			spreadsheetCommandGalleryItem290.CommandName = "InsertChartArea3D";
			spreadsheetCommandGalleryItem290.Description = "Display the trend of values over time or categories using areas on three axes.";
			spreadsheetCommandGalleryItem290.Hint = "Display the trend of values over time or categories using areas on three axes.";
			spreadsheetCommandGalleryItem291.Caption = "Stacked Area in 3-D";
			spreadsheetCommandGalleryItem291.CommandName = "InsertChartStackedArea3D";
			spreadsheetCommandGalleryItem291.Description = "Display the trend of the contribution of each value over time or categories by us" +
	"ing stacked areas in a 3-D format.";
			spreadsheetCommandGalleryItem291.Hint = "Display the trend of the contribution of each value over time or categories by us" +
	"ing stacked areas in a 3-D format.";
			spreadsheetCommandGalleryItem292.Caption = "100% Stacked Area in 3-D";
			spreadsheetCommandGalleryItem292.CommandName = "InsertChartPercentStackedArea3D";
			spreadsheetCommandGalleryItem292.Description = "Display the trend of the percentage each value contributes over time or categorie" +
	"s by using 100% stacked areas in 3-D format.";
			spreadsheetCommandGalleryItem292.Hint = "Display the trend of the percentage each value contributes over time or categorie" +
	"s by using 100% stacked areas in 3-D format.";
			spreadsheetCommandGalleryItemGroup64.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem290,
			spreadsheetCommandGalleryItem291,
			spreadsheetCommandGalleryItem292});
			this.commandBarGalleryDropDown10.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup63,
			spreadsheetCommandGalleryItemGroup64});
			this.commandBarGalleryDropDown10.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown10.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown10.Name = "commandBarGalleryDropDown10";
			this.commandBarGalleryDropDown10.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown11.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup65.CommandName = "InsertChartScatterCommandGroup";
			spreadsheetCommandGalleryItem293.Caption = "Scatter with only Markers";
			spreadsheetCommandGalleryItem293.CommandName = "InsertChartScatterMarkers";
			spreadsheetCommandGalleryItem293.Description = "Compare pairs of values.\r\n\r\nUse is when the values are not in X-axis order or whe" +
	"n they represent separate measurements.";
			spreadsheetCommandGalleryItem293.Hint = "Compare pairs of values.\r\n\r\nUse is when the values are not in X-axis order or whe" +
	"n they represent separate measurements.";
			spreadsheetCommandGalleryItem294.Caption = "Scatter with Smooth Lines and Markers";
			spreadsheetCommandGalleryItem294.CommandName = "InsertChartScatterSmoothLinesAndMarkers";
			spreadsheetCommandGalleryItem294.Description = "Compare pairs of values.\r\n\r\nUse is when there are a few data points in X-axis ord" +
	"er and the data represents a function.";
			spreadsheetCommandGalleryItem294.Hint = "Compare pairs of values.\r\n\r\nUse is when there are a few data points in X-axis ord" +
	"er and the data represents a function.";
			spreadsheetCommandGalleryItem295.Caption = "Scatter with Smooth Lines";
			spreadsheetCommandGalleryItem295.CommandName = "InsertChartScatterSmoothLines";
			spreadsheetCommandGalleryItem295.Description = "Compare pairs of values.\r\n\r\nUse is when there are many data points in X-axis orde" +
	"r and the data represents a function.";
			spreadsheetCommandGalleryItem295.Hint = "Compare pairs of values.\r\n\r\nUse is when there are many data points in X-axis orde" +
	"r and the data represents a function.";
			spreadsheetCommandGalleryItem296.Caption = "Scatter with Straight Lines and Markers";
			spreadsheetCommandGalleryItem296.CommandName = "InsertChartScatterLinesAndMarkers";
			spreadsheetCommandGalleryItem296.Description = "Compare pairs of values.\r\n\r\nUse is when there are a few data points in X-axis ord" +
	"er and the data represents separate samples.";
			spreadsheetCommandGalleryItem296.Hint = "Compare pairs of values.\r\n\r\nUse is when there are a few data points in X-axis ord" +
	"er and the data represents separate samples.";
			spreadsheetCommandGalleryItem297.Caption = "Scatter with Straight Lines";
			spreadsheetCommandGalleryItem297.CommandName = "InsertChartScatterLines";
			spreadsheetCommandGalleryItem297.Description = "Compare pairs of values.\r\n\r\nUse is when there are many data points in X-axis orde" +
	"r and the data represents separate samples.";
			spreadsheetCommandGalleryItem297.Hint = "Compare pairs of values.\r\n\r\nUse is when there are many data points in X-axis orde" +
	"r and the data represents separate samples.";
			spreadsheetCommandGalleryItemGroup65.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem293,
			spreadsheetCommandGalleryItem294,
			spreadsheetCommandGalleryItem295,
			spreadsheetCommandGalleryItem296,
			spreadsheetCommandGalleryItem297});
			spreadsheetCommandGalleryItemGroup66.CommandName = "InsertChartBubbleCommandGroup";
			spreadsheetCommandGalleryItem298.Caption = "Bubble";
			spreadsheetCommandGalleryItem298.CommandName = "InsertChartBubble";
			spreadsheetCommandGalleryItem298.Description = "Resembles a scatter chart, but compares sets of three values instead of two. The " +
	"third value determines the size of the bubble marker.";
			spreadsheetCommandGalleryItem298.Hint = "Resembles a scatter chart, but compares sets of three values instead of two. The " +
	"third value determines the size of the bubble marker.";
			spreadsheetCommandGalleryItem299.Caption = "Bubble with a 3-D effect";
			spreadsheetCommandGalleryItem299.CommandName = "InsertChartBubble3D";
			spreadsheetCommandGalleryItem299.Description = "Resembles a scatter chart, but compares sets of three values instead of two. The " +
	"third value determines the size of the bubble marker, which is displayed with a " +
	"3-D effect.";
			spreadsheetCommandGalleryItem299.Hint = "Resembles a scatter chart, but compares sets of three values instead of two. The " +
	"third value determines the size of the bubble marker, which is displayed with a " +
	"3-D effect.";
			spreadsheetCommandGalleryItemGroup66.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem298,
			spreadsheetCommandGalleryItem299});
			this.commandBarGalleryDropDown11.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup65,
			spreadsheetCommandGalleryItemGroup66});
			this.commandBarGalleryDropDown11.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown11.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown11.Name = "commandBarGalleryDropDown11";
			this.commandBarGalleryDropDown11.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown12.Gallery.AllowFilter = false;
			spreadsheetCommandGalleryItemGroup67.CommandName = "InsertChartStockCommandGroup";
			spreadsheetCommandGalleryItem300.Caption = "High-Low-Close";
			spreadsheetCommandGalleryItem300.CommandName = "InsertChartStockHighLowClose";
			spreadsheetCommandGalleryItem300.Description = "Requires three series of values in order High, Low and Close.";
			spreadsheetCommandGalleryItem300.Hint = "Requires three series of values in order High, Low and Close.";
			spreadsheetCommandGalleryItem301.Caption = "Open-High-Low-Close";
			spreadsheetCommandGalleryItem301.CommandName = "InsertChartStockOpenHighLowClose";
			spreadsheetCommandGalleryItem301.Description = "Requires four series of values in order Open, High, Low and Close.";
			spreadsheetCommandGalleryItem301.Hint = "Requires four series of values in order Open, High, Low and Close.";
			spreadsheetCommandGalleryItemGroup67.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem300,
			spreadsheetCommandGalleryItem301});
			spreadsheetCommandGalleryItemGroup68.CommandName = "InsertChartRadarCommandGroup";
			spreadsheetCommandGalleryItem302.Caption = "Radar";
			spreadsheetCommandGalleryItem302.CommandName = "InsertChartRadar";
			spreadsheetCommandGalleryItem302.Description = "Display values relative to a center point.\r\n\r\nUse it when the categories are not " +
	"directly comparable.";
			spreadsheetCommandGalleryItem302.Hint = "Display values relative to a center point.\r\n\r\nUse it when the categories are not " +
	"directly comparable.";
			spreadsheetCommandGalleryItem303.Caption = "Radar with Markers";
			spreadsheetCommandGalleryItem303.CommandName = "InsertChartRadarWithMarkers";
			spreadsheetCommandGalleryItem303.Description = "Display values relative to a center point.\r\n\r\nUse it when the categories are not " +
	"directly comparable.";
			spreadsheetCommandGalleryItem303.Hint = "Display values relative to a center point.\r\n\r\nUse it when the categories are not " +
	"directly comparable.";
			spreadsheetCommandGalleryItem304.Caption = "Filled Radar";
			spreadsheetCommandGalleryItem304.CommandName = "InsertChartRadarFilled";
			spreadsheetCommandGalleryItem304.Description = "Display values relative to a center point.\r\n\r\nUse it when the categories are not " +
	"directly comparable and there is only one series.";
			spreadsheetCommandGalleryItem304.Hint = "Display values relative to a center point.\r\n\r\nUse it when the categories are not " +
	"directly comparable and there is only one series.";
			spreadsheetCommandGalleryItemGroup68.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem302,
			spreadsheetCommandGalleryItem303,
			spreadsheetCommandGalleryItem304});
			this.commandBarGalleryDropDown12.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup67,
			spreadsheetCommandGalleryItemGroup68});
			this.commandBarGalleryDropDown12.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown12.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown12.Name = "commandBarGalleryDropDown12";
			this.commandBarGalleryDropDown12.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown13.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown13.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup69.CommandName = "ChartTitleCommandGroup";
			spreadsheetCommandGalleryItem305.Caption = "None";
			spreadsheetCommandGalleryItem305.CommandName = "ChartTitleNone";
			spreadsheetCommandGalleryItem305.Description = "Do not display a chart Title";
			spreadsheetCommandGalleryItem305.Hint = "Do not display a chart Title";
			spreadsheetCommandGalleryItem306.Caption = "Centered Overlay Title";
			spreadsheetCommandGalleryItem306.CommandName = "ChartTitleCenteredOverlay";
			spreadsheetCommandGalleryItem306.Description = "Overlay centered Title on chart without resizing chart";
			spreadsheetCommandGalleryItem306.Hint = "Overlay centered Title on chart without resizing chart";
			spreadsheetCommandGalleryItem307.Caption = "Above Chart";
			spreadsheetCommandGalleryItem307.CommandName = "ChartTitleAbove";
			spreadsheetCommandGalleryItem307.Description = "Display Title at top of chart area and resize chart";
			spreadsheetCommandGalleryItem307.Hint = "Display Title at top of chart area and resize chart";
			spreadsheetCommandGalleryItemGroup69.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem305,
			spreadsheetCommandGalleryItem306,
			spreadsheetCommandGalleryItem307});
			this.commandBarGalleryDropDown13.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup69});
			this.commandBarGalleryDropDown13.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown13.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown13.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown13.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown13.Name = "commandBarGalleryDropDown13";
			this.commandBarGalleryDropDown13.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown14.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown14.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup70.CommandName = "ChartPrimaryHorizontalAxisTitleCommandGroup";
			spreadsheetCommandGalleryItem308.Caption = "None";
			spreadsheetCommandGalleryItem308.CommandName = "ChartPrimaryHorizontalAxisTitleNone";
			spreadsheetCommandGalleryItem308.Description = "Do not display an Axis Title";
			spreadsheetCommandGalleryItem308.Hint = "Do not display an Axis Title";
			spreadsheetCommandGalleryItem309.Caption = "Title Below Axis";
			spreadsheetCommandGalleryItem309.CommandName = "ChartPrimaryHorizontalAxisTitleBelow";
			spreadsheetCommandGalleryItem309.Description = "Display Title below Horizontal Axis and resize chart";
			spreadsheetCommandGalleryItem309.Hint = "Display Title below Horizontal Axis and resize chart";
			spreadsheetCommandGalleryItemGroup70.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem308,
			spreadsheetCommandGalleryItem309});
			this.commandBarGalleryDropDown14.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup70});
			this.commandBarGalleryDropDown14.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown14.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown14.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown14.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown14.Name = "commandBarGalleryDropDown14";
			this.commandBarGalleryDropDown14.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown15.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown15.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup71.CommandName = "ChartPrimaryVerticalAxisTitleCommandGroup";
			spreadsheetCommandGalleryItem310.Caption = "None";
			spreadsheetCommandGalleryItem310.CommandName = "ChartPrimaryVerticalAxisTitleNone";
			spreadsheetCommandGalleryItem310.Description = "Do not display an Axis Title";
			spreadsheetCommandGalleryItem310.Hint = "Do not display an Axis Title";
			spreadsheetCommandGalleryItem311.Caption = "Rotated Title";
			spreadsheetCommandGalleryItem311.CommandName = "ChartPrimaryVerticalAxisTitleRotated";
			spreadsheetCommandGalleryItem311.Description = "Display Rotated Axis Title and resize chart";
			spreadsheetCommandGalleryItem311.Hint = "Display Rotated Axis Title and resize chart";
			spreadsheetCommandGalleryItem312.Caption = "Vertical Title";
			spreadsheetCommandGalleryItem312.CommandName = "ChartPrimaryVerticalAxisTitleVertical";
			spreadsheetCommandGalleryItem312.Description = "Display Axis Title with vertical text and resize chart";
			spreadsheetCommandGalleryItem312.Hint = "Display Axis Title with vertical text and resize chart";
			spreadsheetCommandGalleryItem313.Caption = "Horizontal Title";
			spreadsheetCommandGalleryItem313.CommandName = "ChartPrimaryVerticalAxisTitleHorizontal";
			spreadsheetCommandGalleryItem313.Description = "Display Axis Title horizontally and resize chart";
			spreadsheetCommandGalleryItem313.Hint = "Display Axis Title horizontally and resize chart";
			spreadsheetCommandGalleryItemGroup71.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem310,
			spreadsheetCommandGalleryItem311,
			spreadsheetCommandGalleryItem312,
			spreadsheetCommandGalleryItem313});
			this.commandBarGalleryDropDown15.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup71});
			this.commandBarGalleryDropDown15.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown15.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown15.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown15.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown15.Name = "commandBarGalleryDropDown15";
			this.commandBarGalleryDropDown15.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown16.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown16.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup72.CommandName = "ChartLegendCommandGroup";
			spreadsheetCommandGalleryItem314.Caption = "None";
			spreadsheetCommandGalleryItem314.CommandName = "ChartLegendNone";
			spreadsheetCommandGalleryItem314.Description = "Turn off Legend";
			spreadsheetCommandGalleryItem314.Hint = "Turn off Legend";
			spreadsheetCommandGalleryItem315.Caption = "Show Legend at Right";
			spreadsheetCommandGalleryItem315.CommandName = "ChartLegendAtRight";
			spreadsheetCommandGalleryItem315.Description = "Show Legend and align right";
			spreadsheetCommandGalleryItem315.Hint = "Show Legend and align right";
			spreadsheetCommandGalleryItem316.Caption = "Show Legend at Top";
			spreadsheetCommandGalleryItem316.CommandName = "ChartLegendAtTop";
			spreadsheetCommandGalleryItem316.Description = "Show Legend and align top";
			spreadsheetCommandGalleryItem316.Hint = "Show Legend and align top";
			spreadsheetCommandGalleryItem317.Caption = "Show Legend at Left";
			spreadsheetCommandGalleryItem317.CommandName = "ChartLegendAtLeft";
			spreadsheetCommandGalleryItem317.Description = "Show Legend and align left";
			spreadsheetCommandGalleryItem317.Hint = "Show Legend and align left";
			spreadsheetCommandGalleryItem318.Caption = "Show Legend at Bottom";
			spreadsheetCommandGalleryItem318.CommandName = "ChartLegendAtBottom";
			spreadsheetCommandGalleryItem318.Description = "Show Legend and align bottom";
			spreadsheetCommandGalleryItem318.Hint = "Show Legend and align bottom";
			spreadsheetCommandGalleryItem319.Caption = "Overlay Legend at Right";
			spreadsheetCommandGalleryItem319.CommandName = "ChartLegendOverlayAtRight";
			spreadsheetCommandGalleryItem319.Description = "Show Legend at right of the chart without resizing";
			spreadsheetCommandGalleryItem319.Hint = "Show Legend at right of the chart without resizing";
			spreadsheetCommandGalleryItem320.Caption = "Overlay Legend at Left";
			spreadsheetCommandGalleryItem320.CommandName = "ChartLegendOverlayAtLeft";
			spreadsheetCommandGalleryItem320.Description = "Show Legend at left of the chart without resizing";
			spreadsheetCommandGalleryItem320.Hint = "Show Legend at left of the chart without resizing";
			spreadsheetCommandGalleryItemGroup72.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem314,
			spreadsheetCommandGalleryItem315,
			spreadsheetCommandGalleryItem316,
			spreadsheetCommandGalleryItem317,
			spreadsheetCommandGalleryItem318,
			spreadsheetCommandGalleryItem319,
			spreadsheetCommandGalleryItem320});
			this.commandBarGalleryDropDown16.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup72});
			this.commandBarGalleryDropDown16.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown16.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown16.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown16.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown16.Name = "commandBarGalleryDropDown16";
			this.commandBarGalleryDropDown16.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown17.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown17.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup73.CommandName = "ChartDataLabelsCommandGroup";
			spreadsheetCommandGalleryItem321.Caption = "None";
			spreadsheetCommandGalleryItem321.CommandName = "ChartDataLabelsNone";
			spreadsheetCommandGalleryItem321.Description = "Turn off Data Labels for selection";
			spreadsheetCommandGalleryItem321.Hint = "Turn off Data Labels for selection";
			spreadsheetCommandGalleryItem322.Caption = "Show";
			spreadsheetCommandGalleryItem322.CommandName = "ChartDataLabelsDefault";
			spreadsheetCommandGalleryItem322.Description = "Turn on Data Labels for selection";
			spreadsheetCommandGalleryItem322.Hint = "Turn on Data Labels for selection";
			spreadsheetCommandGalleryItem323.Caption = "Center";
			spreadsheetCommandGalleryItem323.CommandName = "ChartDataLabelsCenter";
			spreadsheetCommandGalleryItem323.Description = "Display Data Labels and position centered on the data point(s)";
			spreadsheetCommandGalleryItem323.Hint = "Display Data Labels and position centered on the data point(s)";
			spreadsheetCommandGalleryItem324.Caption = "Inside End";
			spreadsheetCommandGalleryItem324.CommandName = "ChartDataLabelsInsideEnd";
			spreadsheetCommandGalleryItem324.Description = "Display Data Labels and position inside the end of data point(s)";
			spreadsheetCommandGalleryItem324.Hint = "Display Data Labels and position inside the end of data point(s)";
			spreadsheetCommandGalleryItem325.Caption = "Inside Base";
			spreadsheetCommandGalleryItem325.CommandName = "ChartDataLabelsInsideBase";
			spreadsheetCommandGalleryItem325.Description = "Display Data Labels and position inside the base of data point(s)";
			spreadsheetCommandGalleryItem325.Hint = "Display Data Labels and position inside the base of data point(s)";
			spreadsheetCommandGalleryItem326.Caption = "Outside End";
			spreadsheetCommandGalleryItem326.CommandName = "ChartDataLabelsOutsideEnd";
			spreadsheetCommandGalleryItem326.Description = "Display Data Labels and position outside the end of data point(s)";
			spreadsheetCommandGalleryItem326.Hint = "Display Data Labels and position outside the end of data point(s)";
			spreadsheetCommandGalleryItem327.Caption = "Best Fit";
			spreadsheetCommandGalleryItem327.CommandName = "ChartDataLabelsBestFit";
			spreadsheetCommandGalleryItem327.Description = "Display Data Labels and position with Best Fit";
			spreadsheetCommandGalleryItem327.Hint = "Display Data Labels and position with Best Fit";
			spreadsheetCommandGalleryItem328.Caption = "Left";
			spreadsheetCommandGalleryItem328.CommandName = "ChartDataLabelsLeft";
			spreadsheetCommandGalleryItem328.Description = "Display Data Labels and position left of the data point(s)";
			spreadsheetCommandGalleryItem328.Hint = "Display Data Labels and position left of the data point(s)";
			spreadsheetCommandGalleryItem329.Caption = "Right";
			spreadsheetCommandGalleryItem329.CommandName = "ChartDataLabelsRight";
			spreadsheetCommandGalleryItem329.Description = "Display Data Labels and position right of the data point(s)";
			spreadsheetCommandGalleryItem329.Hint = "Display Data Labels and position right of the data point(s)";
			spreadsheetCommandGalleryItem330.Caption = "Above";
			spreadsheetCommandGalleryItem330.CommandName = "ChartDataLabelsAbove";
			spreadsheetCommandGalleryItem330.Description = "Display Data Labels and position above data point(s)";
			spreadsheetCommandGalleryItem330.Hint = "Display Data Labels and position above data point(s)";
			spreadsheetCommandGalleryItem331.Caption = "Below";
			spreadsheetCommandGalleryItem331.CommandName = "ChartDataLabelsBelow";
			spreadsheetCommandGalleryItem331.Description = "Display Data Labels and position below data point(s)";
			spreadsheetCommandGalleryItem331.Hint = "Display Data Labels and position below data point(s)";
			spreadsheetCommandGalleryItemGroup73.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem321,
			spreadsheetCommandGalleryItem322,
			spreadsheetCommandGalleryItem323,
			spreadsheetCommandGalleryItem324,
			spreadsheetCommandGalleryItem325,
			spreadsheetCommandGalleryItem326,
			spreadsheetCommandGalleryItem327,
			spreadsheetCommandGalleryItem328,
			spreadsheetCommandGalleryItem329,
			spreadsheetCommandGalleryItem330,
			spreadsheetCommandGalleryItem331});
			this.commandBarGalleryDropDown17.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup73});
			this.commandBarGalleryDropDown17.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown17.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown17.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown17.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown17.Name = "commandBarGalleryDropDown17";
			this.commandBarGalleryDropDown17.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown18.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown18.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup74.CommandName = "ChartPrimaryHorizontalAxisCommandGroup";
			spreadsheetCommandGalleryItem332.Caption = "None";
			spreadsheetCommandGalleryItem332.CommandName = "ChartHidePrimaryHorizontalAxis";
			spreadsheetCommandGalleryItem332.Description = "Do not display axis.";
			spreadsheetCommandGalleryItem332.Hint = "Do not display axis.";
			spreadsheetCommandGalleryItem333.Caption = "Show Left to Right Axis";
			spreadsheetCommandGalleryItem333.CommandName = "ChartPrimaryHorizontalAxisLeftToRight";
			spreadsheetCommandGalleryItem333.Description = "Display Axis Left to Right with Labels";
			spreadsheetCommandGalleryItem333.Hint = "Display Axis Left to Right with Labels";
			spreadsheetCommandGalleryItem334.Caption = "Show Axis without Labeling";
			spreadsheetCommandGalleryItem334.CommandName = "ChartPrimaryHorizontalAxisHideLabels";
			spreadsheetCommandGalleryItem334.Description = "Display Axis without labels or tick marks";
			spreadsheetCommandGalleryItem334.Hint = "Display Axis without labels or tick marks";
			spreadsheetCommandGalleryItem335.Caption = "Show Right to Left Axis";
			spreadsheetCommandGalleryItem335.CommandName = "ChartPrimaryHorizontalAxisRightToLeft";
			spreadsheetCommandGalleryItem335.Description = "Display Axis Right to Left with Labels";
			spreadsheetCommandGalleryItem335.Hint = "Display Axis Right to Left with Labels";
			spreadsheetCommandGalleryItem336.Caption = "Show Default Axis";
			spreadsheetCommandGalleryItem336.CommandName = "ChartPrimaryHorizontalAxisDefault";
			spreadsheetCommandGalleryItem336.Description = "Display Axis with default order and labels";
			spreadsheetCommandGalleryItem336.Hint = "Display Axis with default order and labels";
			spreadsheetCommandGalleryItem337.Caption = "Show Axis in Thousands";
			spreadsheetCommandGalleryItem337.CommandName = "ChartPrimaryHorizontalAxisScaleThousands";
			spreadsheetCommandGalleryItem337.Description = "Display Axis with numbers represented in Thousands";
			spreadsheetCommandGalleryItem337.Hint = "Display Axis with numbers represented in Thousands";
			spreadsheetCommandGalleryItem338.Caption = "Show Axis in Millions";
			spreadsheetCommandGalleryItem338.CommandName = "ChartPrimaryHorizontalAxisScaleMillions";
			spreadsheetCommandGalleryItem338.Description = "Display Axis with numbers represented in Millions";
			spreadsheetCommandGalleryItem338.Hint = "Display Axis with numbers represented in Millions";
			spreadsheetCommandGalleryItem339.Caption = "Show Axis in Billions";
			spreadsheetCommandGalleryItem339.CommandName = "ChartPrimaryHorizontalAxisScaleBillions";
			spreadsheetCommandGalleryItem339.Description = "Display Axis with numbers represented in Billions";
			spreadsheetCommandGalleryItem339.Hint = "Display Axis with numbers represented in Billions";
			spreadsheetCommandGalleryItem340.Caption = "Show Axis in Log Scale";
			spreadsheetCommandGalleryItem340.CommandName = "ChartPrimaryHorizontalAxisScaleLogarithm";
			spreadsheetCommandGalleryItem340.Description = "Display Axis using a log 10 base scale";
			spreadsheetCommandGalleryItem340.Hint = "Display Axis using a log 10 base scale";
			spreadsheetCommandGalleryItemGroup74.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem332,
			spreadsheetCommandGalleryItem333,
			spreadsheetCommandGalleryItem334,
			spreadsheetCommandGalleryItem335,
			spreadsheetCommandGalleryItem336,
			spreadsheetCommandGalleryItem337,
			spreadsheetCommandGalleryItem338,
			spreadsheetCommandGalleryItem339,
			spreadsheetCommandGalleryItem340});
			this.commandBarGalleryDropDown18.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup74});
			this.commandBarGalleryDropDown18.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown18.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown18.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown18.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown18.Name = "commandBarGalleryDropDown18";
			this.commandBarGalleryDropDown18.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown19.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown19.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup75.CommandName = "ChartPrimaryVerticalAxisCommandGroup";
			spreadsheetCommandGalleryItem341.Caption = "None";
			spreadsheetCommandGalleryItem341.CommandName = "ChartHidePrimaryVerticalAxis";
			spreadsheetCommandGalleryItem341.Description = "Do not display axis.";
			spreadsheetCommandGalleryItem341.Hint = "Do not display axis.";
			spreadsheetCommandGalleryItem342.Caption = "Show Left to Right Axis";
			spreadsheetCommandGalleryItem342.CommandName = "ChartPrimaryVerticalAxisLeftToRight";
			spreadsheetCommandGalleryItem342.Description = "Display Axis Left to Right with Labels";
			spreadsheetCommandGalleryItem342.Hint = "Display Axis Left to Right with Labels";
			spreadsheetCommandGalleryItem343.Caption = "Show Axis without Labeling";
			spreadsheetCommandGalleryItem343.CommandName = "ChartPrimaryVerticalAxisHideLabels";
			spreadsheetCommandGalleryItem343.Description = "Display Axis without labels or tick marks";
			spreadsheetCommandGalleryItem343.Hint = "Display Axis without labels or tick marks";
			spreadsheetCommandGalleryItem344.Caption = "Show Right to Left Axis";
			spreadsheetCommandGalleryItem344.CommandName = "ChartPrimaryVerticalAxisRightToLeft";
			spreadsheetCommandGalleryItem344.Description = "Display Axis Right to Left with Labels";
			spreadsheetCommandGalleryItem344.Hint = "Display Axis Right to Left with Labels";
			spreadsheetCommandGalleryItem345.Caption = "Show Default Axis";
			spreadsheetCommandGalleryItem345.CommandName = "ChartPrimaryVerticalAxisDefault";
			spreadsheetCommandGalleryItem345.Description = "Display Axis with default order and labels";
			spreadsheetCommandGalleryItem345.Hint = "Display Axis with default order and labels";
			spreadsheetCommandGalleryItem346.Caption = "Show Axis in Thousands";
			spreadsheetCommandGalleryItem346.CommandName = "ChartPrimaryVerticalAxisScaleThousands";
			spreadsheetCommandGalleryItem346.Description = "Display Axis with numbers represented in Thousands";
			spreadsheetCommandGalleryItem346.Hint = "Display Axis with numbers represented in Thousands";
			spreadsheetCommandGalleryItem347.Caption = "Show Axis in Millions";
			spreadsheetCommandGalleryItem347.CommandName = "ChartPrimaryVerticalAxisScaleMillions";
			spreadsheetCommandGalleryItem347.Description = "Display Axis with numbers represented in Millions";
			spreadsheetCommandGalleryItem347.Hint = "Display Axis with numbers represented in Millions";
			spreadsheetCommandGalleryItem348.Caption = "Show Axis in Billions";
			spreadsheetCommandGalleryItem348.CommandName = "ChartPrimaryVerticalAxisScaleBillions";
			spreadsheetCommandGalleryItem348.Description = "Display Axis with numbers represented in Billions";
			spreadsheetCommandGalleryItem348.Hint = "Display Axis with numbers represented in Billions";
			spreadsheetCommandGalleryItem349.Caption = "Show Axis in Log Scale";
			spreadsheetCommandGalleryItem349.CommandName = "ChartPrimaryVerticalAxisScaleLogarithm";
			spreadsheetCommandGalleryItem349.Description = "Display Axis using a log 10 base scale";
			spreadsheetCommandGalleryItem349.Hint = "Display Axis using a log 10 base scale";
			spreadsheetCommandGalleryItemGroup75.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem341,
			spreadsheetCommandGalleryItem342,
			spreadsheetCommandGalleryItem343,
			spreadsheetCommandGalleryItem344,
			spreadsheetCommandGalleryItem345,
			spreadsheetCommandGalleryItem346,
			spreadsheetCommandGalleryItem347,
			spreadsheetCommandGalleryItem348,
			spreadsheetCommandGalleryItem349});
			this.commandBarGalleryDropDown19.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup75});
			this.commandBarGalleryDropDown19.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown19.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown19.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown19.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown19.Name = "commandBarGalleryDropDown19";
			this.commandBarGalleryDropDown19.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown20.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown20.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup76.CommandName = "ChartPrimaryHorizontalGridlinesCommandGroup";
			spreadsheetCommandGalleryItem350.Caption = "None";
			spreadsheetCommandGalleryItem350.CommandName = "ChartPrimaryHorizontalGridlinesNone";
			spreadsheetCommandGalleryItem350.Description = "Do not display Horizontal Gridlines";
			spreadsheetCommandGalleryItem350.Hint = "Do not display Horizontal Gridlines";
			spreadsheetCommandGalleryItem351.Caption = "Major Gridlines";
			spreadsheetCommandGalleryItem351.CommandName = "ChartPrimaryHorizontalGridlinesMajor";
			spreadsheetCommandGalleryItem351.Description = "Display Horizontal Gridlines for Major units";
			spreadsheetCommandGalleryItem351.Hint = "Display Horizontal Gridlines for Major units";
			spreadsheetCommandGalleryItem352.Caption = "Minor Gridlines";
			spreadsheetCommandGalleryItem352.CommandName = "ChartPrimaryHorizontalGridlinesMinor";
			spreadsheetCommandGalleryItem352.Description = "Display Horizontal Gridlines for Minor units";
			spreadsheetCommandGalleryItem352.Hint = "Display Horizontal Gridlines for Minor units";
			spreadsheetCommandGalleryItem353.Caption = "Major & Minor Gridlines";
			spreadsheetCommandGalleryItem353.CommandName = "ChartPrimaryHorizontalGridlinesMajorAndMinor";
			spreadsheetCommandGalleryItem353.Description = "Display Horizontal Gridlines for Major and Minor units";
			spreadsheetCommandGalleryItem353.Hint = "Display Horizontal Gridlines for Major and Minor units";
			spreadsheetCommandGalleryItemGroup76.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem350,
			spreadsheetCommandGalleryItem351,
			spreadsheetCommandGalleryItem352,
			spreadsheetCommandGalleryItem353});
			this.commandBarGalleryDropDown20.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup76});
			this.commandBarGalleryDropDown20.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown20.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown20.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown20.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown20.Name = "commandBarGalleryDropDown20";
			this.commandBarGalleryDropDown20.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown21.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown21.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup77.CommandName = "ChartPrimaryVerticalGridlinesCommandGroup";
			spreadsheetCommandGalleryItem354.Caption = "None";
			spreadsheetCommandGalleryItem354.CommandName = "ChartPrimaryVerticalGridlinesNone";
			spreadsheetCommandGalleryItem354.Description = "Do not display Vertical Gridlines";
			spreadsheetCommandGalleryItem354.Hint = "Do not display Vertical Gridlines";
			spreadsheetCommandGalleryItem355.Caption = "Major Gridlines";
			spreadsheetCommandGalleryItem355.CommandName = "ChartPrimaryVerticalGridlinesMajor";
			spreadsheetCommandGalleryItem355.Description = "Display Vertical Gridlines for Major units";
			spreadsheetCommandGalleryItem355.Hint = "Display Vertical Gridlines for Major units";
			spreadsheetCommandGalleryItem356.Caption = "Minor Gridlines";
			spreadsheetCommandGalleryItem356.CommandName = "ChartPrimaryVerticalGridlinesMinor";
			spreadsheetCommandGalleryItem356.Description = "Display Vertical Gridlines for Minor units";
			spreadsheetCommandGalleryItem356.Hint = "Display Vertical Gridlines for Minor units";
			spreadsheetCommandGalleryItem357.Caption = "Major & Minor Gridlines";
			spreadsheetCommandGalleryItem357.CommandName = "ChartPrimaryVerticalGridlinesMajorAndMinor";
			spreadsheetCommandGalleryItem357.Description = "Display Vertical Gridlines for Major and Minor units";
			spreadsheetCommandGalleryItem357.Hint = "Display Vertical Gridlines for Major and Minor units";
			spreadsheetCommandGalleryItemGroup77.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem354,
			spreadsheetCommandGalleryItem355,
			spreadsheetCommandGalleryItem356,
			spreadsheetCommandGalleryItem357});
			this.commandBarGalleryDropDown21.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup77});
			this.commandBarGalleryDropDown21.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown21.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown21.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown21.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown21.Name = "commandBarGalleryDropDown21";
			this.commandBarGalleryDropDown21.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown22.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown22.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup78.CommandName = "ChartLinesCommandGroup";
			spreadsheetCommandGalleryItem358.Caption = "None";
			spreadsheetCommandGalleryItem358.CommandName = "ChartLinesNone";
			spreadsheetCommandGalleryItem358.Description = "Do not show Drop Lines, High-Low Lines or Series Lines";
			spreadsheetCommandGalleryItem358.Hint = "Do not show Drop Lines, High-Low Lines or Series Lines";
			spreadsheetCommandGalleryItem359.Caption = "Drop Lines";
			spreadsheetCommandGalleryItem359.CommandName = "ChartShowDropLines";
			spreadsheetCommandGalleryItem359.Description = "Show Drop Lines on an Area or Line Chart";
			spreadsheetCommandGalleryItem359.Hint = "Show Drop Lines on an Area or Line Chart";
			spreadsheetCommandGalleryItem360.Caption = "High-Low Lines";
			spreadsheetCommandGalleryItem360.CommandName = "ChartShowHighLowLines";
			spreadsheetCommandGalleryItem360.Description = "Show High-Low Lines on a 2D Line Chart";
			spreadsheetCommandGalleryItem360.Hint = "Show High-Low Lines on a 2D Line Chart";
			spreadsheetCommandGalleryItem361.Caption = "Drop and High-Low Lines";
			spreadsheetCommandGalleryItem361.CommandName = "ChartShowDropLinesAndHighLowLines";
			spreadsheetCommandGalleryItem361.Description = "Show Drop Lines and High-Low Lines on a 2D Line Chart";
			spreadsheetCommandGalleryItem361.Hint = "Show Drop Lines and High-Low Lines on a 2D Line Chart";
			spreadsheetCommandGalleryItem362.Caption = "Series Lines";
			spreadsheetCommandGalleryItem362.CommandName = "ChartShowSeriesLines";
			spreadsheetCommandGalleryItem362.Description = "Show Series Lines on a 2D stacked Bar/Column Pie or Pie or Bar of Pie Chart";
			spreadsheetCommandGalleryItem362.Hint = "Show Series Lines on a 2D stacked Bar/Column Pie or Pie or Bar of Pie Chart";
			spreadsheetCommandGalleryItemGroup78.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem358,
			spreadsheetCommandGalleryItem359,
			spreadsheetCommandGalleryItem360,
			spreadsheetCommandGalleryItem361,
			spreadsheetCommandGalleryItem362});
			this.commandBarGalleryDropDown22.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup78});
			this.commandBarGalleryDropDown22.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown22.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown22.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown22.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown22.Name = "commandBarGalleryDropDown22";
			this.commandBarGalleryDropDown22.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown23.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown23.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup79.CommandName = "ChartUpDownBarsCommandGroup";
			spreadsheetCommandGalleryItem363.Caption = "None";
			spreadsheetCommandGalleryItem363.CommandName = "ChartHideUpDownBars";
			spreadsheetCommandGalleryItem363.Description = "Do not show Up/Down Bars";
			spreadsheetCommandGalleryItem363.Hint = "Do not show Up/Down Bars";
			spreadsheetCommandGalleryItem364.Caption = "Up/Down Bars";
			spreadsheetCommandGalleryItem364.CommandName = "ChartShowUpDownBars";
			spreadsheetCommandGalleryItem364.Description = "Show Up/Down Bars on a Line Chart";
			spreadsheetCommandGalleryItem364.Hint = "Show Up/Down Bars on a Line Chart";
			spreadsheetCommandGalleryItemGroup79.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem363,
			spreadsheetCommandGalleryItem364});
			this.commandBarGalleryDropDown23.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup79});
			this.commandBarGalleryDropDown23.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown23.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown23.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown23.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown23.Name = "commandBarGalleryDropDown23";
			this.commandBarGalleryDropDown23.Ribbon = this.ribbonControl1;
			this.commandBarGalleryDropDown24.Gallery.AllowFilter = false;
			this.commandBarGalleryDropDown24.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			spreadsheetCommandGalleryItemGroup80.CommandName = "ChartErrorBarsCommandGroup";
			spreadsheetCommandGalleryItem365.Caption = "None";
			spreadsheetCommandGalleryItem365.CommandName = "ChartErrorBarsNone";
			spreadsheetCommandGalleryItem365.Description = "Removes the Error Bars for the selected series or all Error Bars if none are sele" +
	"cted";
			spreadsheetCommandGalleryItem365.Hint = "Removes the Error Bars for the selected series or all Error Bars if none are sele" +
	"cted";
			spreadsheetCommandGalleryItem366.Caption = "Error Bars with Standard Error";
			spreadsheetCommandGalleryItem366.CommandName = "ChartErrorBarsStandardError";
			spreadsheetCommandGalleryItem366.Description = "Displays Error Bars for the selected chart series using Standard Error";
			spreadsheetCommandGalleryItem366.Hint = "Displays Error Bars for the selected chart series using Standard Error";
			spreadsheetCommandGalleryItem367.Caption = "Error Bars with Percentage";
			spreadsheetCommandGalleryItem367.CommandName = "ChartErrorBarsPercentage";
			spreadsheetCommandGalleryItem367.Description = "Displays Error Bars for the selected chart series with 5% value";
			spreadsheetCommandGalleryItem367.Hint = "Displays Error Bars for the selected chart series with 5% value";
			spreadsheetCommandGalleryItem368.Caption = "Error Bars with Standard Deviation";
			spreadsheetCommandGalleryItem368.CommandName = "ChartErrorBarsStandardDeviation";
			spreadsheetCommandGalleryItem368.Description = "Displays Error Bars for the selected chart series with 1 standard deviation";
			spreadsheetCommandGalleryItem368.Hint = "Displays Error Bars for the selected chart series with 1 standard deviation";
			spreadsheetCommandGalleryItemGroup80.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			spreadsheetCommandGalleryItem365,
			spreadsheetCommandGalleryItem366,
			spreadsheetCommandGalleryItem367,
			spreadsheetCommandGalleryItem368});
			this.commandBarGalleryDropDown24.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			spreadsheetCommandGalleryItemGroup80});
			this.commandBarGalleryDropDown24.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.commandBarGalleryDropDown24.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.commandBarGalleryDropDown24.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.commandBarGalleryDropDown24.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.commandBarGalleryDropDown24.Name = "commandBarGalleryDropDown24";
			this.commandBarGalleryDropDown24.Ribbon = this.ribbonControl1;
			this.spreadsheetFormulaBarControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.spreadsheetFormulaBarControl1.Location = new System.Drawing.Point(0, 0);
			this.spreadsheetFormulaBarControl1.MinimumSize = new System.Drawing.Size(0, 20);
			this.spreadsheetFormulaBarControl1.Name = "spreadsheetFormulaBarControl1";
			this.spreadsheetFormulaBarControl1.Size = new System.Drawing.Size(766, 20);
			this.spreadsheetFormulaBarControl1.SpreadsheetControl = this.spreadsheetControl1;
			this.spreadsheetFormulaBarControl1.TabIndex = 0;
			this.spreadsheetNameBoxControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.spreadsheetNameBoxControl1.EditValue = "A1";
			this.spreadsheetNameBoxControl1.Location = new System.Drawing.Point(0, 0);
			this.spreadsheetNameBoxControl1.Name = "spreadsheetNameBoxControl1";
			this.spreadsheetNameBoxControl1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.spreadsheetNameBoxControl1.Size = new System.Drawing.Size(145, 20);
			this.spreadsheetNameBoxControl1.SpreadsheetControl = this.spreadsheetControl1;
			this.spreadsheetNameBoxControl1.TabIndex = 0;
			this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitContainerControl1.Location = new System.Drawing.Point(0, 144);
			this.splitContainerControl1.Name = "splitContainerControl1";
			this.splitContainerControl1.Panel1.Controls.Add(this.spreadsheetNameBoxControl1);
			this.splitContainerControl1.Panel2.Controls.Add(this.spreadsheetFormulaBarControl1);
			this.splitContainerControl1.Size = new System.Drawing.Size(916, 20);
			this.splitContainerControl1.SplitterPosition = 145;
			this.splitContainerControl1.TabIndex = 2;
			this.splitterControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitterControl1.Location = new System.Drawing.Point(0, 164);
			this.splitterControl1.MinSize = 20;
			this.splitterControl1.Name = "splitterControl1";
			this.splitterControl1.Size = new System.Drawing.Size(916, 5);
			this.splitterControl1.TabIndex = 1;
			this.splitterControl1.TabStop = false;
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem2);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem3);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem4);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem5);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem6);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem7);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem8);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem9);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem10);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem11);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem12);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem13);
			this.spreadsheetBarController1.BarItems.Add(this.changeFontNameItem1);
			this.spreadsheetBarController1.BarItems.Add(this.changeFontSizeItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem14);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem15);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem2);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem3);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem4);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem16);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem17);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem18);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem19);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem20);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem21);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem22);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem23);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem24);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem25);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem26);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem27);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem28);
			this.spreadsheetBarController1.BarItems.Add(this.changeBorderLineColorItem1);
			this.spreadsheetBarController1.BarItems.Add(this.changeBorderLineStyleItem1);
			this.spreadsheetBarController1.BarItems.Add(this.changeCellFillColorItem1);
			this.spreadsheetBarController1.BarItems.Add(this.changeFontColorItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem5);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem6);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem7);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem8);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem9);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem10);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem29);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem30);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem11);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem2);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem12);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem31);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem32);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem33);
			this.spreadsheetBarController1.BarItems.Add(this.changeNumberFormatItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem3);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem34);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem35);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem36);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem37);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem38);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem39);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem40);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem41);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem42);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem4);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem5);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem43);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem44);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem45);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem46);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem47);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem48);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem49);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem6);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem50);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem51);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem52);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem53);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem54);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem55);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem2);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem3);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem7);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem56);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem57);
			this.spreadsheetBarController1.BarItems.Add(this.galleryFormatAsTableItem1);
			this.spreadsheetBarController1.BarItems.Add(this.galleryChangeStyleItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem8);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem58);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem59);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem60);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem9);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem61);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem62);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem63);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem10);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem64);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem65);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem66);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem67);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem68);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem11);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem69);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem70);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem71);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem72);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem73);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem74);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem75);
			this.spreadsheetBarController1.BarItems.Add(this.changeSheetTabColorItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem76);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem13);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem77);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem12);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem78);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem79);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem80);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem81);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem82);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem13);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem83);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem84);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem85);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem86);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem14);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem87);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem88);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem89);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem90);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem91);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem92);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem15);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem93);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem94);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem16);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem95);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem96);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem97);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem98);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem4);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem5);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem6);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem7);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem8);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem9);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem10);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem99);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem100);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem17);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem14);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem15);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem16);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem18);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem17);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem18);
			this.spreadsheetBarController1.BarItems.Add(this.pageSetupPaperKindItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem19);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem101);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem102);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem19);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem20);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem21);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem22);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem20);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem103);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem104);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem21);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem105);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem106);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem22);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem107);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem108);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem109);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem110);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem111);
			this.spreadsheetBarController1.BarItems.Add(this.functionsFinancialItem1);
			this.spreadsheetBarController1.BarItems.Add(this.functionsLogicalItem1);
			this.spreadsheetBarController1.BarItems.Add(this.functionsTextItem1);
			this.spreadsheetBarController1.BarItems.Add(this.functionsDateAndTimeItem1);
			this.spreadsheetBarController1.BarItems.Add(this.functionsLookupAndReferenceItem1);
			this.spreadsheetBarController1.BarItems.Add(this.functionsMathAndTrigonometryItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem23);
			this.spreadsheetBarController1.BarItems.Add(this.functionsStatisticalItem1);
			this.spreadsheetBarController1.BarItems.Add(this.functionsEngineeringItem1);
			this.spreadsheetBarController1.BarItems.Add(this.functionsInformationItem1);
			this.spreadsheetBarController1.BarItems.Add(this.functionsCompatibilityItem1);
			this.spreadsheetBarController1.BarItems.Add(this.functionsWebItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem112);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem113);
			this.spreadsheetBarController1.BarItems.Add(this.definedNameListItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem114);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem23);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem24);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem24);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem25);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem115);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem116);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem117);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem118);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem119);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem120);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem121);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem122);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem123);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem25);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem124);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem125);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem126);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem127);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem128);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem129);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem130);
			this.spreadsheetBarController1.BarItems.Add(this.galleryChartLayoutItem1);
			this.spreadsheetBarController1.BarItems.Add(this.galleryChartStyleItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem11);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem26);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem12);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem13);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem14);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem15);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem27);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem16);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem17);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem28);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem18);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem19);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem20);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem21);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem22);
			this.spreadsheetBarController1.BarItems.Add(this.barStaticItem1);
			this.spreadsheetBarController1.BarItems.Add(this.renameTableItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem26);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem27);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem28);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem29);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem30);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem31);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem32);
			this.spreadsheetBarController1.BarItems.Add(this.galleryTableStylesItem1);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem131);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem33);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem132);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem133);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem29);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem134);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem135);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem30);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem136);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem137);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem138);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem139);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem140);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem141);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem142);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem143);
			this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem144);
			this.spreadsheetBarController1.BarItems.Add(this.barStaticItem2);
			this.spreadsheetBarController1.BarItems.Add(this.barStaticItem3);
			this.spreadsheetBarController1.Control = this.spreadsheetControl1;
			this.spreadsheetCommandBarButtonItem107.CommandName = "FunctionsInsertSum";
			this.spreadsheetCommandBarButtonItem107.Id = -1;
			this.spreadsheetCommandBarButtonItem107.Name = "spreadsheetCommandBarButtonItem107";
			this.spreadsheetCommandBarButtonItem108.CommandName = "FunctionsInsertAverage";
			this.spreadsheetCommandBarButtonItem108.Id = -1;
			this.spreadsheetCommandBarButtonItem108.Name = "spreadsheetCommandBarButtonItem108";
			this.spreadsheetCommandBarButtonItem109.CommandName = "FunctionsInsertCountNumbers";
			this.spreadsheetCommandBarButtonItem109.Id = -1;
			this.spreadsheetCommandBarButtonItem109.Name = "spreadsheetCommandBarButtonItem109";
			this.spreadsheetCommandBarButtonItem110.CommandName = "FunctionsInsertMax";
			this.spreadsheetCommandBarButtonItem110.Id = -1;
			this.spreadsheetCommandBarButtonItem110.Name = "spreadsheetCommandBarButtonItem110";
			this.spreadsheetCommandBarButtonItem111.CommandName = "FunctionsInsertMin";
			this.spreadsheetCommandBarButtonItem111.Id = -1;
			this.spreadsheetCommandBarButtonItem111.Name = "spreadsheetCommandBarButtonItem111";
			this.spreadsheetCommandBarButtonItem131.CommandName = "FileShowDocumentProperties";
			this.spreadsheetCommandBarButtonItem131.Id = 501;
			this.spreadsheetCommandBarButtonItem131.Name = "spreadsheetCommandBarButtonItem131";
			this.spreadsheetCommandBarCheckItem33.CommandName = "DataFilterToggle";
			this.spreadsheetCommandBarCheckItem33.Id = 502;
			this.spreadsheetCommandBarCheckItem33.Name = "spreadsheetCommandBarCheckItem33";
			this.spreadsheetCommandBarButtonItem132.CommandName = "DataFilterClear";
			this.spreadsheetCommandBarButtonItem132.Id = 503;
			this.spreadsheetCommandBarButtonItem132.Name = "spreadsheetCommandBarButtonItem132";
			this.spreadsheetCommandBarButtonItem133.CommandName = "DataFilterReApply";
			this.spreadsheetCommandBarButtonItem133.Id = 504;
			this.spreadsheetCommandBarButtonItem133.Name = "spreadsheetCommandBarButtonItem133";
			this.spreadsheetCommandBarSubItem29.CommandName = "OutlineGroupCommandGroup";
			this.spreadsheetCommandBarSubItem29.Id = 505;
			this.spreadsheetCommandBarSubItem29.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem134),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem135)});
			this.spreadsheetCommandBarSubItem29.Name = "spreadsheetCommandBarSubItem29";
			this.spreadsheetCommandBarSubItem29.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem134.CommandName = "GroupOutline";
			this.spreadsheetCommandBarButtonItem134.Id = 506;
			this.spreadsheetCommandBarButtonItem134.Name = "spreadsheetCommandBarButtonItem134";
			this.spreadsheetCommandBarButtonItem135.CommandName = "AutoOutline";
			this.spreadsheetCommandBarButtonItem135.Id = 507;
			this.spreadsheetCommandBarButtonItem135.Name = "spreadsheetCommandBarButtonItem135";
			this.spreadsheetCommandBarSubItem30.CommandName = "OutlineUngroupCommandGroup";
			this.spreadsheetCommandBarSubItem30.Id = 508;
			this.spreadsheetCommandBarSubItem30.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem136),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem137)});
			this.spreadsheetCommandBarSubItem30.Name = "spreadsheetCommandBarSubItem30";
			this.spreadsheetCommandBarSubItem30.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem136.CommandName = "UngroupOutline";
			this.spreadsheetCommandBarButtonItem136.Id = 509;
			this.spreadsheetCommandBarButtonItem136.Name = "spreadsheetCommandBarButtonItem136";
			this.spreadsheetCommandBarButtonItem137.CommandName = "ClearOutline";
			this.spreadsheetCommandBarButtonItem137.Id = 510;
			this.spreadsheetCommandBarButtonItem137.Name = "spreadsheetCommandBarButtonItem137";
			this.spreadsheetCommandBarButtonItem138.CommandName = "Subtotal";
			this.spreadsheetCommandBarButtonItem138.Id = 511;
			this.spreadsheetCommandBarButtonItem138.Name = "spreadsheetCommandBarButtonItem138";
			this.spreadsheetCommandBarButtonItem138.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.spreadsheetCommandBarButtonItem139.CommandName = "ShowDetail";
			this.spreadsheetCommandBarButtonItem139.Id = 512;
			this.spreadsheetCommandBarButtonItem139.Name = "spreadsheetCommandBarButtonItem139";
			this.spreadsheetCommandBarButtonItem139.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.spreadsheetCommandBarButtonItem140.CommandName = "HideDetail";
			this.spreadsheetCommandBarButtonItem140.Id = 513;
			this.spreadsheetCommandBarButtonItem140.Name = "spreadsheetCommandBarButtonItem140";
			this.spreadsheetCommandBarButtonItem140.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.spreadsheetCommandBarButtonItem141.CommandName = "ReviewInsertComment";
			this.spreadsheetCommandBarButtonItem141.Id = 514;
			this.spreadsheetCommandBarButtonItem141.Name = "spreadsheetCommandBarButtonItem141";
			this.spreadsheetCommandBarButtonItem142.CommandName = "ReviewEditComment";
			this.spreadsheetCommandBarButtonItem142.Id = 515;
			this.spreadsheetCommandBarButtonItem142.Name = "spreadsheetCommandBarButtonItem142";
			this.spreadsheetCommandBarButtonItem143.CommandName = "ReviewDeleteComment";
			this.spreadsheetCommandBarButtonItem143.Id = 516;
			this.spreadsheetCommandBarButtonItem143.Name = "spreadsheetCommandBarButtonItem143";
			this.spreadsheetCommandBarButtonItem144.CommandName = "ReviewShowHideComment";
			this.spreadsheetCommandBarButtonItem144.Id = 517;
			this.spreadsheetCommandBarButtonItem144.Name = "spreadsheetCommandBarButtonItem144";
			this.spreadsheetCommandBarButtonItem144.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.barStaticItem2.Caption = "Table Name:";
			this.barStaticItem2.Id = 518;
			this.barStaticItem2.Name = "barStaticItem2";
			this.barStaticItem2.TextAlignment = System.Drawing.StringAlignment.Near;
			this.drawingFormatArrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem20);
			this.drawingFormatArrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem21);
			this.drawingFormatArrangeRibbonPageGroup1.Name = "drawingFormatArrangeRibbonPageGroup1";
			this.pictureFormatArrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem20);
			this.pictureFormatArrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem21);
			this.pictureFormatArrangeRibbonPageGroup1.Name = "pictureFormatArrangeRibbonPageGroup1";
			this.tablePropertiesRibbonPageGroup1.ItemLinks.Add(this.barStaticItem3);
			this.tablePropertiesRibbonPageGroup1.ItemLinks.Add(this.renameTableItem1);
			this.tablePropertiesRibbonPageGroup1.Name = "tablePropertiesRibbonPageGroup1";
			this.barStaticItem3.Caption = "Table Name:";
			this.barStaticItem3.Id = 519;
			this.barStaticItem3.Name = "barStaticItem3";
			this.barStaticItem3.TextAlignment = System.Drawing.StringAlignment.Near;
			this.tableToolsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem26);
			this.tableToolsRibbonPageGroup1.Name = "tableToolsRibbonPageGroup1";
			this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem27);
			this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem28);
			this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem29);
			this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem30);
			this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem31);
			this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem32);
			this.tableStyleOptionsRibbonPageGroup1.Name = "tableStyleOptionsRibbonPageGroup1";
			this.tableStylesRibbonPageGroup1.ItemLinks.Add(this.galleryTableStylesItem1);
			this.tableStylesRibbonPageGroup1.Name = "tableStylesRibbonPageGroup1";
			this.chartsDesignTypeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem128);
			this.chartsDesignTypeRibbonPageGroup1.Name = "chartsDesignTypeRibbonPageGroup1";
			this.chartsDesignDataRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem129);
			this.chartsDesignDataRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem130);
			this.chartsDesignDataRibbonPageGroup1.Name = "chartsDesignDataRibbonPageGroup1";
			this.chartsDesignLayoutsRibbonPageGroup1.ItemLinks.Add(this.galleryChartLayoutItem1);
			this.chartsDesignLayoutsRibbonPageGroup1.Name = "chartsDesignLayoutsRibbonPageGroup1";
			this.chartsDesignStylesRibbonPageGroup1.ItemLinks.Add(this.galleryChartStyleItem1);
			this.chartsDesignStylesRibbonPageGroup1.Name = "chartsDesignStylesRibbonPageGroup1";
			this.chartsLayoutLabelsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem11);
			this.chartsLayoutLabelsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem26);
			this.chartsLayoutLabelsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem14);
			this.chartsLayoutLabelsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem15);
			this.chartsLayoutLabelsRibbonPageGroup1.Name = "chartsLayoutLabelsRibbonPageGroup1";
			this.chartsLayoutAxesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem27);
			this.chartsLayoutAxesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem28);
			this.chartsLayoutAxesRibbonPageGroup1.Name = "chartsLayoutAxesRibbonPageGroup1";
			this.chartsLayoutAnalysisRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem20);
			this.chartsLayoutAnalysisRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem21);
			this.chartsLayoutAnalysisRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem22);
			this.chartsLayoutAnalysisRibbonPageGroup1.Name = "chartsLayoutAnalysisRibbonPageGroup1";
			this.chartsFormatArrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem20);
			this.chartsFormatArrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem21);
			this.chartsFormatArrangeRibbonPageGroup1.Name = "chartsFormatArrangeRibbonPageGroup1";
			this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem1);
			this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem2);
			this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem3);
			this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem4);
			this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem5);
			this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem6);
			this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem7);
			this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem8);
			this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem9);
			this.commonRibbonPageGroup1.Name = "commonRibbonPageGroup1";
			this.infoRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem131);
			this.infoRibbonPageGroup1.Name = "infoRibbonPageGroup1";
			this.clipboardRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem10);
			this.clipboardRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem11);
			this.clipboardRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem12);
			this.clipboardRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem13);
			this.clipboardRibbonPageGroup1.Name = "clipboardRibbonPageGroup1";
			this.fontRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup11);
			this.fontRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup12);
			this.fontRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup13);
			this.fontRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup14);
			this.fontRibbonPageGroup1.Name = "fontRibbonPageGroup1";
			this.barButtonGroup11.Id = 520;
			this.barButtonGroup11.ItemLinks.Add(this.changeFontNameItem1);
			this.barButtonGroup11.ItemLinks.Add(this.changeFontSizeItem1);
			this.barButtonGroup11.ItemLinks.Add(this.spreadsheetCommandBarButtonItem14);
			this.barButtonGroup11.ItemLinks.Add(this.spreadsheetCommandBarButtonItem15);
			this.barButtonGroup11.Name = "barButtonGroup11";
			this.barButtonGroup11.Tag = "{B0CA3FA8-82D6-4BC4-BD31-D9AE56C1D033}";
			this.barButtonGroup12.Id = 521;
			this.barButtonGroup12.ItemLinks.Add(this.spreadsheetCommandBarCheckItem1);
			this.barButtonGroup12.ItemLinks.Add(this.spreadsheetCommandBarCheckItem2);
			this.barButtonGroup12.ItemLinks.Add(this.spreadsheetCommandBarCheckItem3);
			this.barButtonGroup12.ItemLinks.Add(this.spreadsheetCommandBarCheckItem4);
			this.barButtonGroup12.Name = "barButtonGroup12";
			this.barButtonGroup12.Tag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
			this.barButtonGroup13.Id = 522;
			this.barButtonGroup13.ItemLinks.Add(this.spreadsheetCommandBarSubItem1);
			this.barButtonGroup13.Name = "barButtonGroup13";
			this.barButtonGroup13.Tag = "{DDB05A32-9207-4556-85CB-FE3403A197C7}";
			this.barButtonGroup14.Id = 523;
			this.barButtonGroup14.ItemLinks.Add(this.changeCellFillColorItem1);
			this.barButtonGroup14.ItemLinks.Add(this.changeFontColorItem1);
			this.barButtonGroup14.Name = "barButtonGroup14";
			this.barButtonGroup14.Tag = "{C2275623-04A3-41E8-8D6A-EB5C7F8541D1}";
			this.alignmentRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup15);
			this.alignmentRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup16);
			this.alignmentRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup17);
			this.alignmentRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem11);
			this.alignmentRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem2);
			this.alignmentRibbonPageGroup1.Name = "alignmentRibbonPageGroup1";
			this.barButtonGroup15.Id = 524;
			this.barButtonGroup15.ItemLinks.Add(this.spreadsheetCommandBarCheckItem5);
			this.barButtonGroup15.ItemLinks.Add(this.spreadsheetCommandBarCheckItem6);
			this.barButtonGroup15.ItemLinks.Add(this.spreadsheetCommandBarCheckItem7);
			this.barButtonGroup15.Name = "barButtonGroup15";
			this.barButtonGroup15.Tag = "{03A0322B-12A2-4434-A487-8B5AAF64CCFC}";
			this.barButtonGroup16.Id = 525;
			this.barButtonGroup16.ItemLinks.Add(this.spreadsheetCommandBarCheckItem8);
			this.barButtonGroup16.ItemLinks.Add(this.spreadsheetCommandBarCheckItem9);
			this.barButtonGroup16.ItemLinks.Add(this.spreadsheetCommandBarCheckItem10);
			this.barButtonGroup16.Name = "barButtonGroup16";
			this.barButtonGroup16.Tag = "{ECC693B7-EF59-4007-A0DB-A9550214A0F2}";
			this.barButtonGroup17.Id = 526;
			this.barButtonGroup17.ItemLinks.Add(this.spreadsheetCommandBarButtonItem29);
			this.barButtonGroup17.ItemLinks.Add(this.spreadsheetCommandBarButtonItem30);
			this.barButtonGroup17.Name = "barButtonGroup17";
			this.barButtonGroup17.Tag = "{A5E37DED-106E-44FC-8044-CE3824C08225}";
			this.numberRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup18);
			this.numberRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup19);
			this.numberRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup20);
			this.numberRibbonPageGroup1.Name = "numberRibbonPageGroup1";
			this.barButtonGroup18.Id = 527;
			this.barButtonGroup18.ItemLinks.Add(this.changeNumberFormatItem1);
			this.barButtonGroup18.Name = "barButtonGroup18";
			this.barButtonGroup18.Tag = "{0B3A7A43-3079-4ce0-83A8-3789F5F6DC9F}";
			this.barButtonGroup19.Id = 528;
			this.barButtonGroup19.ItemLinks.Add(this.spreadsheetCommandBarSubItem3);
			this.barButtonGroup19.ItemLinks.Add(this.spreadsheetCommandBarButtonItem39);
			this.barButtonGroup19.ItemLinks.Add(this.spreadsheetCommandBarButtonItem40);
			this.barButtonGroup19.Name = "barButtonGroup19";
			this.barButtonGroup19.Tag = "{508C2CE6-E1C8-4DD1-BA50-6C210FDB31B0}";
			this.barButtonGroup20.Id = 529;
			this.barButtonGroup20.ItemLinks.Add(this.spreadsheetCommandBarButtonItem41);
			this.barButtonGroup20.ItemLinks.Add(this.spreadsheetCommandBarButtonItem42);
			this.barButtonGroup20.Name = "barButtonGroup20";
			this.barButtonGroup20.Tag = "{BBAB348B-BDB2-487A-A883-EFB9982DC698}";
			this.stylesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem4);
			this.stylesRibbonPageGroup1.ItemLinks.Add(this.galleryFormatAsTableItem1);
			this.stylesRibbonPageGroup1.ItemLinks.Add(this.galleryChangeStyleItem1);
			this.stylesRibbonPageGroup1.Name = "stylesRibbonPageGroup1";
			this.cellsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem8);
			this.cellsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem9);
			this.cellsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem10);
			this.cellsRibbonPageGroup1.Name = "cellsRibbonPageGroup1";
			this.editingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem12);
			this.editingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem13);
			this.editingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem14);
			this.editingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem15);
			this.editingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem16);
			this.editingRibbonPageGroup1.Name = "editingRibbonPageGroup1";
			this.tablesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem97);
			this.tablesRibbonPageGroup1.Name = "tablesRibbonPageGroup1";
			this.illustrationsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem98);
			this.illustrationsRibbonPageGroup1.Name = "illustrationsRibbonPageGroup1";
			this.chartsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem4);
			this.chartsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem5);
			this.chartsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem6);
			this.chartsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem7);
			this.chartsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem8);
			this.chartsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem9);
			this.chartsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem10);
			this.chartsRibbonPageGroup1.Name = "chartsRibbonPageGroup1";
			this.linksRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem99);
			this.linksRibbonPageGroup1.Name = "linksRibbonPageGroup1";
			this.symbolsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem100);
			this.symbolsRibbonPageGroup1.Name = "symbolsRibbonPageGroup1";
			this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem17);
			this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem18);
			this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.pageSetupPaperKindItem1);
			this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem19);
			this.pageSetupRibbonPageGroup1.Name = "pageSetupRibbonPageGroup1";
			this.pageSetupShowRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem19);
			this.pageSetupShowRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem20);
			this.pageSetupShowRibbonPageGroup1.Name = "pageSetupShowRibbonPageGroup1";
			this.pageSetupPrintRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem21);
			this.pageSetupPrintRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem22);
			this.pageSetupPrintRibbonPageGroup1.Name = "pageSetupPrintRibbonPageGroup1";
			this.arrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem20);
			this.arrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem21);
			this.arrangeRibbonPageGroup1.Name = "arrangeRibbonPageGroup1";
			this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem22);
			this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.functionsFinancialItem1);
			this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.functionsLogicalItem1);
			this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.functionsTextItem1);
			this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.functionsDateAndTimeItem1);
			this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.functionsLookupAndReferenceItem1);
			this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.functionsMathAndTrigonometryItem1);
			this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem23);
			this.functionLibraryRibbonPageGroup1.Name = "functionLibraryRibbonPageGroup1";
			this.formulaDefinedNamesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem112);
			this.formulaDefinedNamesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem113);
			this.formulaDefinedNamesRibbonPageGroup1.ItemLinks.Add(this.definedNameListItem1);
			this.formulaDefinedNamesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem114);
			this.formulaDefinedNamesRibbonPageGroup1.Name = "formulaDefinedNamesRibbonPageGroup1";
			this.formulaAuditingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem23);
			this.formulaAuditingRibbonPageGroup1.Name = "formulaAuditingRibbonPageGroup1";
			this.formulaCalculationRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem24);
			this.formulaCalculationRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem115);
			this.formulaCalculationRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem116);
			this.formulaCalculationRibbonPageGroup1.Name = "formulaCalculationRibbonPageGroup1";
			this.sortAndFilterRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem93);
			this.sortAndFilterRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem94);
			this.sortAndFilterRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem33);
			this.sortAndFilterRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem132);
			this.sortAndFilterRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem133);
			this.sortAndFilterRibbonPageGroup1.Name = "sortAndFilterRibbonPageGroup1";
			this.outlineRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem29);
			this.outlineRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem30);
			this.outlineRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem138);
			this.outlineRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem139);
			this.outlineRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem140);
			this.outlineRibbonPageGroup1.Name = "outlineRibbonPageGroup1";
			this.commentsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem141);
			this.commentsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem142);
			this.commentsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem143);
			this.commentsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem144);
			this.commentsRibbonPageGroup1.Name = "commentsRibbonPageGroup1";
			this.changesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem76);
			this.changesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem117);
			this.changesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem118);
			this.changesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem119);
			this.changesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem120);
			this.changesRibbonPageGroup1.Name = "changesRibbonPageGroup1";
			this.showRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem19);
			this.showRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem20);
			this.showRibbonPageGroup1.Name = "showRibbonPageGroup1";
			this.zoomRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem121);
			this.zoomRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem122);
			this.zoomRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem123);
			this.zoomRibbonPageGroup1.Name = "zoomRibbonPageGroup1";
			this.windowRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem25);
			this.windowRibbonPageGroup1.Name = "windowRibbonPageGroup1";
			this.ClientSize = new System.Drawing.Size(916, 574);
			this.Controls.Add(this.spreadsheetControl1);
			this.Controls.Add(this.splitterControl1);
			this.Controls.Add(this.splitContainerControl1);
			this.Controls.Add(this.ribbonControl1);
			this.Name = "MailMergePreviewForm";
			this.Ribbon = this.ribbonControl1;
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Mail Merge Preview";
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown26)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpreadsheetFontSizeEdit2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown25)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemPopupGalleryEdit2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown27)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown28)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown29)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown30)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown31)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown32)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown33)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown34)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown35)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown36)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown37)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown38)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown39)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown40)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown41)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown42)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown43)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown44)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown45)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown46)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown47)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown48)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpreadsheetFontSizeEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemPopupGalleryEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown11)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown12)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown13)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown14)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown15)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown16)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown17)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown18)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown19)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown20)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown21)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown22)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown23)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandBarGalleryDropDown24)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spreadsheetNameBoxControl1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
			this.splitContainerControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spreadsheetBarController1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem1;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem2;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem3;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem4;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem5;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem6;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem7;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem8;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem9;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem10;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem11;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem12;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem13;
		private XtraBars.BarButtonGroup barButtonGroup1;
		private UI.ChangeFontNameItem changeFontNameItem1;
		private XtraEditors.Repository.RepositoryItemFontEdit repositoryItemFontEdit2;
		private UI.ChangeFontSizeItem changeFontSizeItem1;
		private XtraSpreadsheet.Design.RepositoryItemSpreadsheetFontSizeEdit repositoryItemSpreadsheetFontSizeEdit2;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem14;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem15;
		private XtraBars.BarButtonGroup barButtonGroup2;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem1;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem2;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem3;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem4;
		private XtraBars.BarButtonGroup barButtonGroup3;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem1;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem16;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem17;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem18;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem19;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem20;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem21;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem22;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem23;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem24;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem25;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem26;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem27;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem28;
		private UI.ChangeBorderLineColorItem changeBorderLineColorItem1;
		private UI.ChangeBorderLineStyleItem changeBorderLineStyleItem1;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown25;
		private XtraBars.BarButtonGroup barButtonGroup4;
		private UI.ChangeCellFillColorItem changeCellFillColorItem1;
		private UI.ChangeFontColorItem changeFontColorItem1;
		private XtraBars.BarButtonGroup barButtonGroup5;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem5;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem6;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem7;
		private XtraBars.BarButtonGroup barButtonGroup6;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem8;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem9;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem10;
		private XtraBars.BarButtonGroup barButtonGroup7;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem29;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem30;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem11;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem2;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem12;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem31;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem32;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem33;
		private XtraBars.BarButtonGroup barButtonGroup8;
		private UI.ChangeNumberFormatItem changeNumberFormatItem1;
		private XtraEditors.Repository.RepositoryItemPopupGalleryEdit repositoryItemPopupGalleryEdit2;
		private XtraBars.BarButtonGroup barButtonGroup9;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem3;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem34;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem35;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem36;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem37;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem38;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem39;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem40;
		private XtraBars.BarButtonGroup barButtonGroup10;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem41;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem42;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem4;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem5;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem43;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem44;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem45;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem46;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem47;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem48;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem49;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem6;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem50;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem51;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem52;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem53;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem54;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem55;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem1;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown26;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem2;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown27;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem3;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown28;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem7;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem56;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem57;
		private UI.GalleryFormatAsTableItem galleryFormatAsTableItem1;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown29;
		private UI.GalleryChangeStyleItem galleryChangeStyleItem1;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem8;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem58;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem59;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem60;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem9;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem61;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem62;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem63;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem10;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem64;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem65;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem66;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem67;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem68;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem11;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem69;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem70;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem71;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem72;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem73;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem74;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem75;
		private UI.ChangeSheetTabColorItem changeSheetTabColorItem1;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem76;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem13;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem77;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem12;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem78;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem79;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem80;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem81;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem82;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem13;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem83;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem84;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem85;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem86;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem14;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem87;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem88;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem89;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem90;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem91;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem92;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem15;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem93;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem94;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem16;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem95;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem96;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem97;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem98;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem4;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown30;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem5;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown31;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem6;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown32;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem7;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown33;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem8;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown34;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem9;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown35;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem10;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown36;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem99;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem100;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem17;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem14;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem15;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem16;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem18;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem17;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem18;
		private UI.PageSetupPaperKindItem pageSetupPaperKindItem1;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem19;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem101;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem102;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem19;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem20;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem21;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem22;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem20;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem103;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem104;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem21;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem105;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem106;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem22;
		private UI.FunctionsFinancialItem functionsFinancialItem1;
		private UI.FunctionsLogicalItem functionsLogicalItem1;
		private UI.FunctionsTextItem functionsTextItem1;
		private UI.FunctionsDateAndTimeItem functionsDateAndTimeItem1;
		private UI.FunctionsLookupAndReferenceItem functionsLookupAndReferenceItem1;
		private UI.FunctionsMathAndTrigonometryItem functionsMathAndTrigonometryItem1;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem23;
		private UI.FunctionsStatisticalItem functionsStatisticalItem1;
		private UI.FunctionsEngineeringItem functionsEngineeringItem1;
		private UI.FunctionsInformationItem functionsInformationItem1;
		private UI.FunctionsCompatibilityItem functionsCompatibilityItem1;
		private UI.FunctionsWebItem functionsWebItem1;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem112;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem113;
		private UI.DefinedNameListItem definedNameListItem1;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem114;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem23;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem24;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem24;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem25;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem115;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem116;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem117;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem118;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem119;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem120;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem121;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem122;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem123;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem25;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem124;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem125;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem126;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem127;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem128;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem129;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem130;
		private UI.GalleryChartLayoutItem galleryChartLayoutItem1;
		private UI.GalleryChartStyleItem galleryChartStyleItem1;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem11;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown37;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem26;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem12;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown38;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem13;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown39;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem14;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown40;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem15;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown41;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem27;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem16;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown42;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem17;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown43;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem28;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem18;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown44;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem19;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown45;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem20;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown46;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem21;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown47;
		private UI.SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem22;
		private XtraBars.Commands.CommandBarGalleryDropDown commandBarGalleryDropDown48;
		private XtraBars.BarStaticItem barStaticItem1;
		private UI.RenameTableItem renameTableItem1;
		private XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit2;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem26;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem27;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem28;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem29;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem30;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem31;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem32;
		private UI.GalleryTableStylesItem galleryTableStylesItem1;
		private UI.ChartToolsRibbonPageCategory chartToolsRibbonPageCategory1;
		private UI.ChartsDesignRibbonPage chartsDesignRibbonPage1;
		private UI.ChartsLayoutRibbonPage chartsLayoutRibbonPage1;
		private UI.ChartsFormatRibbonPage chartsFormatRibbonPage1;
		private UI.TableToolsRibbonPageCategory tableToolsRibbonPageCategory1;
		private UI.TableToolsDesignRibbonPage tableToolsDesignRibbonPage1;
		private UI.PictureToolsRibbonPageCategory pictureToolsRibbonPageCategory1;
		private UI.PictureFormatRibbonPage pictureFormatRibbonPage1;
		private UI.DrawingToolsRibbonPageCategory drawingToolsRibbonPageCategory1;
		private UI.DrawingFormatRibbonPage drawingFormatRibbonPage1;
		private UI.FileRibbonPage fileRibbonPage1;
		private UI.HomeRibbonPage homeRibbonPage1;
		private UI.InsertRibbonPage insertRibbonPage1;
		private UI.PageLayoutRibbonPage pageLayoutRibbonPage1;
		private UI.FormulasRibbonPage formulasRibbonPage1;
		private UI.DataRibbonPage dataRibbonPage1;
		private UI.ReviewRibbonPage reviewRibbonPage1;
		private UI.ViewRibbonPage viewRibbonPage1;
		private UI.SpreadsheetBarController spreadsheetBarController1;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem107;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem108;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem109;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem110;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem111;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem131;
		private UI.SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem33;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem132;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem133;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem29;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem134;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem135;
		private UI.SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem30;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem136;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem137;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem138;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem139;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem140;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem141;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem142;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem143;
		private UI.SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem144;
		private XtraBars.BarStaticItem barStaticItem2;
		private XtraBars.BarStaticItem barStaticItem3;
		private XtraBars.BarButtonGroup barButtonGroup11;
		private XtraBars.BarButtonGroup barButtonGroup12;
		private XtraBars.BarButtonGroup barButtonGroup13;
		private XtraBars.BarButtonGroup barButtonGroup14;
		private XtraBars.BarButtonGroup barButtonGroup15;
		private XtraBars.BarButtonGroup barButtonGroup16;
		private XtraBars.BarButtonGroup barButtonGroup17;
		private XtraBars.BarButtonGroup barButtonGroup18;
		private XtraBars.BarButtonGroup barButtonGroup19;
		private XtraBars.BarButtonGroup barButtonGroup20;
		private UI.ChartsDesignTypeRibbonPageGroup chartsDesignTypeRibbonPageGroup1;
		private UI.ChartsDesignDataRibbonPageGroup chartsDesignDataRibbonPageGroup1;
		private UI.ChartsDesignLayoutsRibbonPageGroup chartsDesignLayoutsRibbonPageGroup1;
		private UI.ChartsDesignStylesRibbonPageGroup chartsDesignStylesRibbonPageGroup1;
		private UI.ChartsLayoutLabelsRibbonPageGroup chartsLayoutLabelsRibbonPageGroup1;
		private UI.ChartsLayoutAxesRibbonPageGroup chartsLayoutAxesRibbonPageGroup1;
		private UI.ChartsLayoutAnalysisRibbonPageGroup chartsLayoutAnalysisRibbonPageGroup1;
		private UI.ChartsFormatArrangeRibbonPageGroup chartsFormatArrangeRibbonPageGroup1;
		private UI.TablePropertiesRibbonPageGroup tablePropertiesRibbonPageGroup1;
		private UI.TableToolsRibbonPageGroup tableToolsRibbonPageGroup1;
		private UI.TableStyleOptionsRibbonPageGroup tableStyleOptionsRibbonPageGroup1;
		private UI.TableStylesRibbonPageGroup tableStylesRibbonPageGroup1;
		private UI.PictureFormatArrangeRibbonPageGroup pictureFormatArrangeRibbonPageGroup1;
		private UI.DrawingFormatArrangeRibbonPageGroup drawingFormatArrangeRibbonPageGroup1;
		private UI.CommonRibbonPageGroup commonRibbonPageGroup1;
		private UI.InfoRibbonPageGroup infoRibbonPageGroup1;
		private UI.ClipboardRibbonPageGroup clipboardRibbonPageGroup1;
		private UI.FontRibbonPageGroup fontRibbonPageGroup1;
		private UI.AlignmentRibbonPageGroup alignmentRibbonPageGroup1;
		private UI.NumberRibbonPageGroup numberRibbonPageGroup1;
		private UI.StylesRibbonPageGroup stylesRibbonPageGroup1;
		private UI.CellsRibbonPageGroup cellsRibbonPageGroup1;
		private UI.EditingRibbonPageGroup editingRibbonPageGroup1;
		private UI.TablesRibbonPageGroup tablesRibbonPageGroup1;
		private UI.IllustrationsRibbonPageGroup illustrationsRibbonPageGroup1;
		private UI.ChartsRibbonPageGroup chartsRibbonPageGroup1;
		private UI.LinksRibbonPageGroup linksRibbonPageGroup1;
		private UI.SymbolsRibbonPageGroup symbolsRibbonPageGroup1;
		private UI.PageSetupRibbonPageGroup pageSetupRibbonPageGroup1;
		private UI.PageSetupShowRibbonPageGroup pageSetupShowRibbonPageGroup1;
		private UI.PageSetupPrintRibbonPageGroup pageSetupPrintRibbonPageGroup1;
		private UI.ArrangeRibbonPageGroup arrangeRibbonPageGroup1;
		private UI.FunctionLibraryRibbonPageGroup functionLibraryRibbonPageGroup1;
		private UI.FormulaDefinedNamesRibbonPageGroup formulaDefinedNamesRibbonPageGroup1;
		private UI.FormulaAuditingRibbonPageGroup formulaAuditingRibbonPageGroup1;
		private UI.FormulaCalculationRibbonPageGroup formulaCalculationRibbonPageGroup1;
		private UI.SortAndFilterRibbonPageGroup sortAndFilterRibbonPageGroup1;
		private UI.OutlineRibbonPageGroup outlineRibbonPageGroup1;
		private UI.CommentsRibbonPageGroup commentsRibbonPageGroup1;
		private UI.ChangesRibbonPageGroup changesRibbonPageGroup1;
		private UI.ShowRibbonPageGroup showRibbonPageGroup1;
		private UI.ZoomRibbonPageGroup zoomRibbonPageGroup1;
		private UI.WindowRibbonPageGroup windowRibbonPageGroup1;
	}
}
