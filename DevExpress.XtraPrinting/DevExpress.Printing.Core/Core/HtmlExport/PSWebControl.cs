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

using DevExpress.XtraPrinting.HtmlExport.Controls;
namespace DevExpress.XtraPrinting.Export.Web {
	class PSWebControlMail : PSWebControl {
		public PSWebControlMail(Document document, IImageRepository imageRepository, HtmlExportMode exportMode, bool tableLayout)
			: base(document, imageRepository, exportMode, tableLayout) {
		}
		protected override HtmlBuilderBase GetHtmlBuilder(bool tableLayout) {
			return tableLayout ? (HtmlBuilderBase)new HtmlTableBuilderMail() : (HtmlBuilderBase)new HtmlDivBuilderMail();
		}
	}
	public class PSWebControl : PSWebControlBase {
		DXHtmlTable contentTable;
		bool tableLayout;
		public DXHtmlTable ContentTable {
			get { return contentTable; }
		}
		public PSWebControl(Document document, IImageRepository imageRepository, HtmlExportMode exportMode, bool tableLayout)
			: base(document, imageRepository, exportMode) {
			this.tableLayout = tableLayout;
		}
		protected override void CreatePages() {
			contentTable = CreateHtmlLayoutTable(new HtmlLayoutBuilder(document, htmlExportContext), tableLayout);
			AddChildrenControl(contentTable);
		}
	}
}
