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

using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Web.Native {
	public class HtmlPageWebExportContext : HtmlWebExportContext {
		readonly BookmarkChecker bookmarkChecker;
		public override bool CopyStyleWhenClipping {
			get { return true; }
		}
		public override bool IsPageExport {
			get { return true; }
		}
		public HtmlPageWebExportContext(PrintingSystemBase ps, IScriptContainer scriptContainer, IImageRepository imageRepository)
			: base(ps, scriptContainer, imageRepository) {
			bookmarkChecker = new BookmarkChecker(ps.Document.RootBookmark, ps.Pages);
		}
		public override BrickViewData[] GetData(Brick brick, RectangleF rect, RectangleF clipRect) {
			BrickViewData[] dataContainer = base.GetData(brick, rect, clipRect);
			SetFillers(brick, dataContainer);
			return dataContainer;
		}
		void SetFillers(Brick brick, BrickViewData[] dataContainer) {
			string path = GetBookmarkPath(brick);
			if(string.IsNullOrEmpty(path)) {
				return;
			}
			foreach(BrickViewData data in dataContainer) {
				data.TableCell = new AnchorCell(data.TableCell, path);
			}
		}
		string GetBookmarkPath(Brick brick) {
			string indexes = BrickPagePairHelper.IndicesFromArray(BrickPagePairHelper.GetIndicesByBrick(DrawingPage, brick));
			return string.IsNullOrEmpty(indexes)
				? null
				: bookmarkChecker.GetPresentPath(DrawingPage.Index, indexes);
		}
	}
}
