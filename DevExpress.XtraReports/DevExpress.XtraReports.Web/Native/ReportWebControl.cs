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

using System.Web.UI.WebControls;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Web.Native {
	class ReportWebControl : PSWebControl {
		public ReportWebControl(Document document, IImageRepository imageRepository, bool tableLayout)
			: base(document, imageRepository, HtmlExportMode.SingleFile, tableLayout) {
		}
		protected override HtmlExportContext CreateExportContext(PrintingSystemBase printingSystem, IScriptContainer scriptContainer, IImageRepository imageRepository, HtmlExportMode mode) {
			return new HtmlWebExportContext(printingSystem, scriptContainer, imageRepository);
		}
		protected override void CreatePages() {
			base.CreatePages();
			if(ContentTable != null) {
				ContentTable.Style["background-color"] = HtmlConvert.ToHtml(document.PrintingSystem.Graph.PageBackColor);
				ContentTable.CssClass = "page-background-color-holder";
			}
		}
		protected override void AddControlsBeforeCreatePages() {
		}
		protected override void AddControlsAfterCreatePages() {
			Controls.Add(styleControl);
			Controls.Add(scriptControl);
		}
		protected override ScriptBlockControl CreateScriptControl(WebStyleControl styleControl) {
			return new WebScriptBlockControl(styleControl);
		}
		public virtual void ApplyControlSize(WebControl dest) {
			dest.Width = Unit.Empty;
			dest.Height = Unit.Empty;
			if(ContentTable != null) {
				dest.Style["width"] = ContentTable.Style["width"];
				dest.Style["height"] = ContentTable.Style["height"];
			}
		}
	}
}
