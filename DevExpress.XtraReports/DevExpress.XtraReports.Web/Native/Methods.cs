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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils.Zip;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Web.Native {
	public static class Methods {
		public static long GetHashCode(string[] items) {
			var adler = new Adler32();
			foreach(string s in items) {
				foreach(char ch in s) {
					adler.Calculate(BitConverter.GetBytes(ch));
				}
			}
			return adler.Checksum;
		}
		public static string GenID(string prefix, string id) {
			return prefix + "_" + id;
		}
		public static void EndResponse(HttpResponse response) {
			HttpContext.Current.ApplicationInstance.CompleteRequest();
			try {
				response.End();
			} catch(ThreadAbortException) {
			}
		}
		public static string GetPageName(HttpRequest request) {
			string[] items = request.CurrentExecutionFilePath.Split('/');
			Debug.Assert(items.Length > 0 && items[items.Length - 1].Length > 0);
			return items[items.Length - 1];
		}
		public static string ToString(Style style) {
			var sb = new StringBuilder();
			CssStyleCollection css = style.GetStyleAttributes(null);
			foreach(string key in css.Keys) {
				sb.Append(key + ':' + css[key] + ';');
			}
			return sb.ToString();
		}
		internal static Size GetPageSize(this PrintingSystemBase printingSystem, int pageIndex, bool clipClientArea) {
			if(printingSystem == null || pageIndex >= printingSystem.Document.PageCount) {
				return Size.Empty;
			}
			var page = (PSPage)printingSystem.Document.Pages[pageIndex];
			SizeF pageSizeF = clipClientArea ? page.ClippedPageSize : page.PageSize;
			return Size.Round(XRConvert.DocToPixel(pageSizeF));
		}
		internal static Size GetPageSize(this XtraReport report, bool clipClientArea) {
			Margins margins = report.Margins;
			Band band = report.Bands[BandKind.TopMargin];
			int top = band != null ? 0 : margins.Top;
			band = report.Bands[BandKind.BottomMargin];
			int bottom = band != null ? 0 : margins.Bottom;
			var pageBounds = XtraReportAccessor.GetPageBounds(report).Size;
			SizeF pageSize = clipClientArea
				? new SizeF(pageBounds.Width - margins.Left - margins.Right, pageBounds.Height - top - bottom)
				: pageBounds;
			return XRConvert.Convert(Size.Round(pageSize), report.Dpi, GraphicsDpi.Pixel);
		}
	}
}
