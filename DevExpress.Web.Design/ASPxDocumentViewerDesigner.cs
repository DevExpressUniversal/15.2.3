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
using System.ComponentModel.Design;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Web.Design;
using DevExpress.Web.Design.Reports;
using DevExpress.Web.Design.Reports.DocumentViewer;
using DevExpress.XtraPrinting.Design;
namespace DevExpress.XtraReports.Web.Design {
	public class ASPxDocumentViewerDesigner : ASPxWebControlDesigner {
		readonly Wrapper<string> remoteSourceCachedUserName = new Wrapper<string>("");
		ASPxDocumentViewer documentViewer;
		IComponentChangeService componentChangeService;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			documentViewer = (ASPxDocumentViewer)component;
			componentChangeService = GetService<IComponentChangeService>();
			PrintControlDesigner.InitializeLookAndFeelService(GetService<IDesignerHost>(), GetService<ILookAndFeelService>());
		}
		public override void ShowAbout() {
			XtraReportAboutDialogHelper.ShowAbout(Component.Site);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ASPxDocumentViewerDesignerActionList(this, remoteSourceCachedUserName, componentChangeService);
		}
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			XtraReportsAssembliesInsurer.EnsureControlReferences(DesignerHost);
		}
		protected override string GetDesignTimeHtmlInternal() {
			var result = base.GetDesignTimeHtmlInternal();
			var tableIndex = result.IndexOf("<table");
			var styles = result.Substring(0, tableIndex);
			result = result.Substring(tableIndex);
			int pos = result.LastIndexOf("</td>");
			if(pos == -1)
				pos = result.LastIndexOf("</");
			if(pos != -1)
				result = result.Insert(pos, styles);
			return result;
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new DocumentViewerCommonFormDesigner(documentViewer, DesignerHost)));
		}
	}
}
