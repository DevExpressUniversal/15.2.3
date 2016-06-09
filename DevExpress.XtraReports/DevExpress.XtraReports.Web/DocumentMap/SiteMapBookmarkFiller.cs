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

using System;
using System.ComponentModel;
using System.Web;
using DevExpress.Web;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Web {
	public class SiteMapBookmarkFiller : BookmarkFiller<SiteMapNode> {
		public static void FillSiteMapWithBookmarks(ASPxSiteMapDataSource dataSource, ASPxDocumentViewer documentViewer) {
			FillSiteMapWithBookmarksCore(dataSource, x => new SiteMapBookmarkFiller(documentViewer, x));
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void FillSiteMapWithBookmarks(ASPxSiteMapDataSource dataSource, ReportViewer reportViewer) {
			FillSiteMapWithBookmarksCore(dataSource, x => new SiteMapBookmarkFiller(reportViewer, x));
		}
		static void FillSiteMapWithBookmarksCore(ASPxSiteMapDataSource dataSource, Func<UnboundSiteMapProvider, SiteMapBookmarkFiller> func) {
			var provider = new UnboundSiteMapProvider("", "");
			dataSource.Provider = provider;
			func(provider).Fill(provider.RootNode);
		}
		readonly UnboundSiteMapProvider provider;
		SiteMapBookmarkFiller(ASPxDocumentViewer documentViewer, UnboundSiteMapProvider provider)
			: base(documentViewer) {
			this.provider = provider;
		}
		SiteMapBookmarkFiller(ReportViewer reportViewer, UnboundSiteMapProvider provider)
			: base(reportViewer) {
			this.provider = provider;
		}
		protected override SiteMapNode FillNode(SiteMapNode parent, BookmarkNode bookmarkNode, string navigationScript) {
			SiteMapNode node = provider.CreateNode("javascript:void(" + navigationScript + ')', bookmarkNode.Text);
			provider.AddSiteMapNode(node, parent);
			return node;
		}
	}
}
