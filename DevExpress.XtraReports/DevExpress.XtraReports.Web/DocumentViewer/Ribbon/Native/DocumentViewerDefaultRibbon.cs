#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using DevExpress.Web;
namespace DevExpress.XtraReports.Web.DocumentViewer.Ribbon.Native {
	public static class DocumentViewerDefaultRibbon {
		public static Collection<RibbonTab> CreateRibbonTabs() {
			var defaultRibbonTabCollection = new Collection<RibbonTab>();
			defaultRibbonTabCollection.Add(CreateHomeTab());
			return defaultRibbonTabCollection;
		}
		static RibbonTab CreateHomeTab() {
			var homeTab = new DocumentViewerHomeRibbonTab();
			homeTab.Groups.Add(CreatePrintGroup());
			homeTab.Groups.Add(CreateExportGroup());
			homeTab.Groups.Add(CreateNavigationGroup());
			homeTab.Groups.Add(CreateReportGroup());
			return homeTab;
		}
		static RibbonGroup CreatePrintGroup() {
			var group = new DocumentViewerPrintRibbonGroup();
			group.Items.Add(new DocumentViewerPrintReportRibbonCommand());
			group.Items.Add(new DocumentViewerPrintPageRibbonCommand());
			return group;
		}
		static RibbonGroup CreateNavigationGroup() {
			var group = new DocumentViewerNavigationRibbonGroup();
			group.Items.Add(new DocumentViewerFirstPageRibbonCommand());
			group.Items.Add(new DocumentViewerPreviousPageRibbonCommand());
			group.Items.Add(new DocumentViewerPageNumbersTemplateRibbonCommand());
			group.Items.Add(new DocumentViewerNextPageRibbonCommand());
			group.Items.Add(new DocumentViewerLastPageRibbonCommand());
			return group;
		}
		static RibbonGroup CreateExportGroup() {
			var group = new DocumentViewerExportRibbonGroup();
			var saveToDiskCommand = new DocumentViewerSaveToDiskDropDownRibbonCommand();
			saveToDiskCommand.CreateDefaultItems();
			group.Items.Add(saveToDiskCommand);
			var saveToWindowCommand = new DocumentViewerSaveToWindowDropDownRibbonCommand();
			saveToWindowCommand.CreateDefaultItems();
			group.Items.Add(saveToWindowCommand);
			return group;
		}
		static RibbonGroup CreateReportGroup() {
			var group = new DocumentViewerReportRibbonGroup();
			group.Items.Add(new DocumentViewerSearchRibbonCommand());
			group.Items.Add(new DocumentViewerParametersPanelToggleCommand());
			group.Items.Add(new DocumentViewerDocumentMapToggleCommand());
			return group;
		}
	}
}
