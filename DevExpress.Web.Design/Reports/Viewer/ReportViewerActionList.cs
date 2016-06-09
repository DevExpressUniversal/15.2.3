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

using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Web.Design.Reports.Converters;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.Design;
namespace DevExpress.Web.Design.Reports.Viewer {
	public class ReportViewerActionList : XRWebControlDesignerActionListBase<ReportViewer> {
		[Editor("DevExpress.XtraReports.Design.ReportNameEditor," + AssemblyInfo.SRAssemblyReportsDesign, typeof(UITypeEditor))]
		[TypeConverter(typeof(ReportTypeNameConverter))]
		public string Report {
			get { return Component.ReportName; }
			set { SetComponentProperty(x => x.ReportName, value); }
		}
		[RefreshProperties(RefreshProperties.All)]
		public bool AutoSize {
			get { return Component.AutoSize; }
			set { SetComponentProperty(x => x.AutoSize, value); }
		}
		public bool PageByPage {
			get { return Component.PageByPage; }
			set { SetComponentProperty(x => x.PageByPage, value); }
		}
		public bool TableLayout {
			get { return Component.TableLayout; }
			set { SetComponentProperty(x => x.TableLayout, value); }
		}
		public bool PrintUsingAdobePlugIn {
			get { return Component.PrintUsingAdobePlugIn; }
			set { SetComponentProperty(x => x.PrintUsingAdobePlugIn, value); }
		}
		public ReportViewerActionList(ReportViewerDesigner designer)
			: base(designer) {
		}
		protected override void BuildActionItems(DesignerActionItemsBuilder builder) {
			builder.AddProperty(() => Report, "Report");
			builder.AddProperty(() => AutoSize, "Auto Size");
			builder.AddProperty(() => PageByPage, "Page By Page View");
			builder.AddProperty(() => PrintUsingAdobePlugIn, "Print Using Adobe Plug-In");
			builder.AddProperty(() => TableLayout, "Table Layout");
		}
	}
}
