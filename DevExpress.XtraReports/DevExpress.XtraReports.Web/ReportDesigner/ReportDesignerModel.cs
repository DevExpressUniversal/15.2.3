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
using DevExpress.XtraPrinting.WebClientUIControl.DataContracts;
using DevExpress.XtraReports.Web.ClientControls.DataContracts;
using DevExpress.XtraReports.Web.ReportDesigner.DataContracts;
using DevExpress.XtraReports.Web.ReportDesigner.Native.DataContracts;
namespace DevExpress.XtraReports.Web.ReportDesigner {
	public class ReportDesignerModel {
		public string ReportUrl { get; set; }
		public string ReportModelJson { get; set; }
		public Dictionary<string, DataSourceRefInfo[]> DataSourceRefInfo { get; set; }
		public DataSourceInfo[] DataSources { get; set; }
		public string[] DataSourcesData { get; set; }
		public Dictionary<string, string> Subreports { get; set; }
		public MenuAction[] MenuActions { get; set; }
		public string[] MenuItemJSClickActions { get; set; }
		public EnumInfo[] KnownEnums { get; set; }
		public Dictionary<string, string> ReportExtensions { get; set; }
		public WizardDataConnection[] WizardConnections { get; set; }
		public ReportDesignerModelInternals Internals { get; set; }
		public bool IsCustomSqlDisabled { get; set; }
	}
}
