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
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.XtraPrinting.Native.DrillDown;
namespace DevExpress.XtraReports.Web.Native {
	class AspNavigationService : WebNavigationService {
		readonly string reportViewerClientId;
		public AspNavigationService(string reportViewerClientId) {
			this.reportViewerClientId = reportViewerClientId;
		}
		protected override string CreateNavigationScript() {
			if(Url.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
				return Url;
			if(Brick.NavigationPair != BrickPagePair.Empty) {
				string path = string.Join(new string(BookmarkChecker.IndexSeparator, 1), Brick.NavigationPair.BrickIndices);
				return string.Format("javascript:ASPx.RVGotoBM('{0}',{1},'{2}')", reportViewerClientId, Brick.NavigationPageIndex, path);
			}
			if(!string.IsNullOrEmpty(Url) && !string.Equals(Url, SR.BrickEmptyUrl, StringComparison.OrdinalIgnoreCase)) {
				return string.Format("javascript:ASPx.xr_NavigateUrl('{0}','{1}')", DXHttpUtility.UrlEncodeToUnicodeCompatible(Url), Brick.Target);
			}
			var value = Brick.Value;
			if(value is DrillDownKey) {
				var ddk = (DrillDownKey)value;
				return string.Format("javascript:ASPx.xr_NavigateDrillDown('{0}','{1}')", reportViewerClientId, DXHttpUtility.UrlEncodeToUnicodeCompatible(ddk.ToString()));
			}
			return string.Empty;
		}
	}
}
