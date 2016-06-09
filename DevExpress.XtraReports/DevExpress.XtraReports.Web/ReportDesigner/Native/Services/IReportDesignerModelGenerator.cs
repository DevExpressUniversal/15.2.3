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

using System.Collections.Generic;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Native.ClientControls;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native.Services {
	public interface IReportDesignerModelGenerator {
		ReportDesignerModel Generate(XtraReport report, IDictionary<string, object> dataSources, IDictionary<string, string> subreports, IEnumerable<ClientControlsMenuItemModel> menuItems, ReportDesignerModelSettings settings);
	}
	public static class ReportDesignerModelGeneratorExtensions {
#if DEBUGTEST
		public static ReportDesignerModel Generate_TEST(this IReportDesignerModelGenerator reportDesignerModelGenerator, XtraReport report, IDictionary<string, object> dataSources, IDictionary<string, string> subreports, IEnumerable<ClientControlsMenuItemModel> menuItems) {
			var defaultSettings = new ReportDesignerModelSettings(shouldShareReportDataSources: true, tryAddDefaultDataSerializer: true);
			return reportDesignerModelGenerator.Generate(report, dataSources, subreports, menuItems, defaultSettings);
		}
#endif
	}
}
