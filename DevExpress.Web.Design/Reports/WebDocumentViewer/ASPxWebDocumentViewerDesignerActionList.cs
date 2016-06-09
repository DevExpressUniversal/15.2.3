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
namespace DevExpress.Web.Design.Reports.WebDocumentViewer {
	public class ASPxWebDocumentViewerDesignerActionList : XRWebControlDesignerActionListBase<ASPxWebDocumentViewer> {
		readonly ASPxWebDocumentViewerDesigner designer;
		public override bool AutoShow {
			get { return true; }
			set { }
		}
		[TypeConverter(typeof(ColorSchemeConverter))]
		public string ColorScheme {
			get { return Component.ColorScheme; }
			set { SetComponentProperty(x => x.ColorScheme, value); }
		}
		[TypeConverter(typeof(ReportTypeNameConverter))]
		[Editor("DevExpress.XtraReports.Design.ReportNameEditor," + AssemblyInfo.SRAssemblyReportsDesign, typeof(UITypeEditor))]
		public string ReportSourceId {
			get { return Component.ReportSourceId; }
			set {
				SetComponentProperty(x => x.ReportSourceId, value);
			}
		}
		public ASPxWebDocumentViewerDesignerActionList(ASPxWebDocumentViewerDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		public void SwitchToClassicDocumentViewer() {
			ViewersSwitcher.Switch(Component, Designer.DesignerHost);
		}
		protected override void BuildActionItems(DesignerActionItemsBuilder builder) {
			builder.AddProperty(() => ColorScheme, "Color Scheme");
			builder.AddMethod(() => SwitchToClassicDocumentViewer(), "Switch to Classic ASP.NET Document Viewer", "Designer...", includeAsDesignerVerb: true);
			builder.AddProperty(() => ReportSourceId, "Report");
		}
	}
}
