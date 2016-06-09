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
using System.Linq;
using System.Text;
using DevExpress.Web.ASPxHtmlEditor.Design;
using DevExpress.Web.Design;
namespace DevExpress.Web.ASPxSpreadsheet.Design {
	[CLSCompliant(false)]
	public class SpreadsheetDocumentSelectorFrame : FeatureHtmlEditorDialogFrame {
		public override Type FeatureBrowserFormBase { get { return typeof(SpreadsheetDocumentSelectorForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.Spreadsheet.DocumentSelector.xml" }; } }
	}
	[CLSCompliant(false)]
	public class SpreadsheetDocumentSelectorForm : DocumentSelectorFormBase {
		protected ASPxSpreadsheet Spreadsheet { get { return (ASPxSpreadsheet)SourceObject; } }
		protected override FileManagerDetailsColumnCollection DetailViewColumns { get { return Spreadsheet.SettingsDocumentSelector.FileListSettings.DetailsViewSettings.Columns; } }
		protected override FileManagerToolbarItemCollection ToolbarItems { get { return Spreadsheet.SettingsDocumentSelector.ToolbarSettings.Items; } }
		protected override ItemsEditorOwner AccessRulesOwner { get { return new DocumentSelectorPermissionSettingsItemsOwner(Spreadsheet, Spreadsheet.Site, Spreadsheet.SettingsDocumentSelector.PermissionSettings.AccessRules); } }
		protected override ItemsEditorOwner DetailsViewColumnsOwner { get { return new DetailsViewSettingsColumnsOwner(Spreadsheet, Spreadsheet.Site, DetailViewColumns); } }
		protected override FileManagerToolbarItemsOwner ToolbarItemsOwner { get { return ToolbarItems != null ? new FileManagerToolbarItemsOwner(Spreadsheet, "Toolbar Items", Spreadsheet.Site, ToolbarItems) : null; } }
	}
}
