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
using System.ComponentModel;
using DevExpress.Data.Utils;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services;
using InternalUtils = DevExpress.Web.Mvc.Internal.Utils;
namespace DevExpress.Web.Mvc {
	[ToolboxItem(false)]
	public class MVCxWebDocumentViewer : ASPxWebDocumentViewer {
		internal WebDocumentViewerModel Model { get; set; }
		public MVCxWebDocumentViewer()
			: this(DefaultWebDocumentViewerContainer.Current) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public MVCxWebDocumentViewer(IServiceProvider serviceProvider)
			: this(
			serviceProvider.GetService<IWebDocumentViewerModelGenerator>(),
			serviceProvider.GetService<IWebDocumentViewerHtmlContentGenerator>(),
			serviceProvider.GetService<IJSContentGenerator<WebDocumentViewerModel>>(),
			serviceProvider.GetService<IStoragesCleaner>()) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public MVCxWebDocumentViewer(
			IWebDocumentViewerModelGenerator webDocumentViewerModelGenerator,
			IWebDocumentViewerHtmlContentGenerator htmlContentGenerator,
			IJSContentGenerator<WebDocumentViewerModel> jsContentGenerator,
			IStoragesCleaner storagesCleaner)
			: base(webDocumentViewerModelGenerator, htmlContentGenerator, jsContentGenerator, storagesCleaner) {
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxWebDocumentViewer), InternalUtils.WebDocumentViewerScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientWebDocumentViewer";
		}
		protected override WebDocumentViewerModel GetModel() {
			return Model ?? base.GetModel();
		}
	}
}
