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

using System.Text;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services {
	public class WebDocumentViewerJSContentGenerator : IJSContentGenerator<WebDocumentViewerModel> {
		const bool DebugMode =
#if DEBUG
 true
#else
  false
#endif
;
		readonly ILocalizationInfoProvider localizationInfoProvider;
		public WebDocumentViewerJSContentGenerator(ILocalizationInfoProvider localizationInfoProvider) {
			this.localizationInfoProvider = localizationInfoProvider;
		}
		public void Generate(StringBuilder stb, string localVarName, WebDocumentViewerModel model) {
			var commonModel = CommonModel.Generate();
			new JsAssignmentGenerator(stb, localVarName)
				.AppendAsString("reportId", model.ReportInfo.ReportId)
				.AppendAsString("exportOptions", model.ReportInfo.ExportOptions)
				.AppendContract("parametersInfo", model.ReportInfo.ParametersInfo)
				.AppendContract("pageHeight", model.ReportInfo.PageHeight)
				.AppendContract("pageWidth", model.ReportInfo.PageWidth)
				.AppendContract("menuItems", model.MenuActions)
				.AppendRawArray("menuItemActions", model.MenuItemJSClickActions)
				.AppendAsString("handlerUri", ASPxWebDocumentViewer.DefaultHandlerUri)
				.AppendDictionary("localization", LocalizationInitializer.GetLocalization(localizationInfoProvider))
				.AppendDictionary(CommonModel.InfoDefaultsPropertyName, commonModel.DefaultValues)
				.AppendContract("debugMode", DebugMode);
		}
	}
}
