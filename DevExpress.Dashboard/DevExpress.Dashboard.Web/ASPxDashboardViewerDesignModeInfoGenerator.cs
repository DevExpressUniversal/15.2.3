#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.IO;
using DevExpress.Web.WebClientUIControl.Internal;
namespace DevExpress.DashboardWeb.Native {
	public static class ASPxDashboardViewerDesignModeInfoGenerator {
		const string DashboardViewerTitle = "Dashboard Viewer";
		public static ClientControlsDesignModeInfo GetDashboardNotSpecifiedInfo() {
			return CreateDesignModeControlInfo(GetHtmlDescription("DashboardNotSpecified"));
		}
		public static ClientControlsDesignModeInfo GetDashboardXmlSpecifiedInfo(string xmlPath) {
			return CreateDesignModeControlInfo(GetHtmlDescription("DashboardXmlSpecified", xmlPath));
		}
		public static ClientControlsDesignModeInfo GetDashboardXmlNotFoundInfo() {
			return CreateDesignModeControlInfo(GetHtmlDescription("DashboardXmlNotFound"));
		}
		public static ClientControlsDesignModeInfo GetDashboardClassSpecifiedInfo(string typeName) {
			return CreateDesignModeControlInfo(GetHtmlDescription("DashboardClassSpecified", typeName));
		}
		public static ClientControlsDesignModeInfo GetDashboardClassNameInvalidInfo() {
			return CreateDesignModeControlInfo(GetHtmlDescription("DashboardClassNameInvalid"));
		}
		static string GetHtmlDescription(string name) {
			return GetHtmlDescription(name, null);
		}
		static string GetHtmlDescription(string name, object parameter) {
			var type = typeof(ASPxDashboardViewer);
			using(var reader = new StreamReader(type.Assembly.GetManifestResourceStream(type, string.Format("Html.{0}.html", name)))) {
				string result = reader.ReadToEnd();
				return parameter != null ? string.Format(result, parameter) : result;
			}
		}
		static ClientControlsDesignModeControlInfo<ASPxDashboardViewerDesignModeImages> CreateDesignModeControlInfo(string htmlDescription) {
			return new ClientControlsDesignModeControlInfo<ASPxDashboardViewerDesignModeImages>(DashboardViewerTitle, htmlDescription);
		}
	}
}
