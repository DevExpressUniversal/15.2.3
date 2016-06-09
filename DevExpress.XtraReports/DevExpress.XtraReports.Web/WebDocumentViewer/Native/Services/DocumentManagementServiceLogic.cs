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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using DevExpress.Printing.Core.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Navigation;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Localization;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.DataContracts;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services {
	public static class DocumentManagementServiceLogic {
		public static string GetFaultMessage(string exceptionMessage) {
			return string.IsNullOrEmpty(exceptionMessage) ? "Internal Server Error" : exceptionMessage;
		}
		public static string GetFaultMessage(Exception ex) {
			return ex == null ? null : GetFaultMessage(ex.Message);
		}
		public static BuildStatusResponse GetColdBuildStatus(Document document, string faultMessage) {
			var pageCount = document == null ? 0 : document.PageCount;
			return string.IsNullOrEmpty(faultMessage)
				? new BuildStatusResponse {
					Completed = true,
					PageCount = pageCount,
					Progress = 100
				}
				: new BuildStatusResponse {
					Completed = false,
					FaultMessage = faultMessage,
					RequestAgain = false
				};
		}
		public static byte[] GetPage(Document document, int pageIndex, int resolution) {
			if(pageIndex < 0 || pageIndex >= document.PageCount) {
				throw new ArgumentOutOfRangeException("pageIndex");
			}
			if(resolution < 0) {
				throw new ArgumentOutOfRangeException("resolution");
			}
			var exportOptions = new ImageExportOptions(ImageFormat.Png) {
				ExportMode = ImageExportMode.SingleFilePageByPage,
				PageRange = (pageIndex + 1).ToString(),
				PageBorderColor = Color.Transparent,
				PageBorderWidth = 0
			};
			exportOptions.Resolution = resolution;
			var ps = document.PrintingSystem;
			using(var stream = new MemoryStream()) {
				ps.ReplaceService<IBrickPublisher>(new DefaultBrickPublisher());
				try {
					ps.ExportToImage(stream, exportOptions);
				} finally {
					ps.RemoveService<IBrickPublisher>();
				}
				return stream.ToArray();
			}
		}
		public static DocumentMapNode FromBookmarkNode(IBookmarkNode bookmark) {
			if(bookmark == null)
				return null;
			var nodes = bookmark.Nodes != null && bookmark.Nodes.Count() > 0
				? bookmark.Nodes.Select(FromBookmarkNode).ToArray()
				: null;
			return new DocumentMapNode() {
				Text = bookmark.Text,
				PageIndex = bookmark.PageIndex,
				Indexes = bookmark.Indices,
				Nodes = nodes
			};
		}
		public static FindTextResponse FindText(Document document, string text, bool matchCase, bool wholeWord) {
			List<FoundText> searchResult = new List<FoundText>();
			if(string.IsNullOrEmpty(text))
				throw new InvalidOperationException(ASPxReportsLocalizer.GetString(ASPxReportsStringId.SearchDialog_EnterText));
			var selector = new TextBrickSelector(text, wholeWord, matchCase, document.PrintingSystem);
			BrickPagePairCollection bpPairs = NavigateHelper.SelectBrickPagePairs(document, selector, new BrickPagePairComparer(document.Pages));
			if(bpPairs == null || bpPairs.Count == 0)
				new FindTextResponse(false);
			for(int i = 0; i < bpPairs.Count; i++) {
				string matchedText = "";
				BrickPagePair pair = bpPairs[i];
				VisualBrick brick = pair.GetBrick(document.Pages) as VisualBrick;
				if(brick != null) {
					string brickText = brick.Text;
					const int CharsCount = 80;
					const int CharCountBeforeMarchedText = 20;
					int startIndex = brick.Text.IndexOf(text, matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
					if(startIndex > CharCountBeforeMarchedText) {
						int firstWhiteSpace = startIndex;
						for(int stepBack = 0; stepBack < CharCountBeforeMarchedText; stepBack++) {
							if(char.IsWhiteSpace(brickText[startIndex - stepBack])) {
								firstWhiteSpace = startIndex - stepBack;
							}
						}
						startIndex = firstWhiteSpace == startIndex ? startIndex - 20 : firstWhiteSpace + 1;
						matchedText = "..." + brickText.Substring(startIndex, Math.Min(CharsCount - 3, brickText.Length - startIndex));
					} else {
						matchedText = brickText.Substring(0, Math.Min(CharsCount, brickText.Length));
					}
				}
				searchResult.Add(new FoundText() { Indexes = pair.Indices, PageIndex = pair.PageIndex, Id = i, Text = matchedText });
			}
			return new FindTextResponse(searchResult.Count != 0) { Matches = searchResult.ToArray() };
		}
		public static BrickMapResponse GetBrickMap(Document document, int pageIndex) {
			var mapBuilder = new MapBuilder();
			var mapNode = mapBuilder.BuildMap(document.PrintingSystem.Pages[pageIndex]);
			var brickMapNode = FromIMapNode(mapNode, document);
			return new BrickMapResponse { ColumnWidthArray = mapBuilder.ColumnWidthArray, Brick = brickMapNode };
		}
		public static ExportedDocument ExportTo(PrintingSystemBase ps, string format, ExportOptions options) {
			using(var stream = new MemoryStream()) {
				var exportingStrategy = new PSExportingStrategy(ps, stream);
				ExportingNamingInfo info = exportingStrategy.GenerateInfo(format, options);
				string documentName = XtraReportAccessor.GetDocumentName(ps);
				return new ExportedDocument(stream.ToArray(), info.ContentType, documentName + "." + info.FileExtension, info.ContentDisposition);
			}
		}
		public static DocumentDataResponse GetDocumentData(Document document, Dictionary<string, bool> drillDownKeys) {
			BookmarkNode rootBookmark = document.RootBookmark;
			var response = new DocumentDataResponse {
				DocumentMap = rootBookmark.Nodes.Count > 0
					? DocumentManagementServiceLogic.FromBookmarkNode(rootBookmark)
					: null,
				DrillDownKeys = drillDownKeys
			};
			return response;
		}
		public static BrickMapNode FromIMapNode(IWebDocumentViewerMapNode node, Document document) {
			var nodes = node.Nodes != null && node.Nodes.Count > 0 ? node.Nodes.Select(x => FromIMapNode(x, document)).ToArray() : null;
			Size size = Size.Round(XRConvert.DocToPixel(node.Bounds.Size));
			Point location = Point.Round(XRConvert.DocToPixel(new PointF(node.Bounds.Location.X, node.Bounds.Location.Y)));
			var nodeContent = (node.Content == null || node.Content.Keys.Count == 0) ? null : node.Content;
			var navigation = ToBrickMapNodeNavigation(node);
			return new BrickMapNode {
				Top = location.Y,
				Left = location.X,
				Width = size.Width,
				Height = size.Height,
				Bricks = nodes,
				Indexes = node.Indexes,
				Col = node.Column,
				Row = node.Row,
				GeneralIndex = node.GeneralBrickIndex,
				Navigation = navigation,
				Content = nodeContent
			};
		}
		static BrickMapNodeNavigation ToBrickMapNodeNavigation(IWebDocumentViewerMapNode node) {
			bool isNotEmpty = false;
			Func<bool> doesNotEmpty = () => { isNotEmpty = true; return true; };
			var navigation = new BrickMapNodeNavigation() {
				DrillDownKey = !string.IsNullOrEmpty(node.DrillDownKey) && doesNotEmpty() ? node.DrillDownKey : null,
				Indexes = !string.IsNullOrEmpty(node.NavigationIndexes) && doesNotEmpty() ? node.NavigationIndexes : null,
				Url = !string.IsNullOrEmpty(node.NavigationUrl) && doesNotEmpty() ? node.NavigationUrl : null,
				Target = !string.IsNullOrEmpty(node.Target) && doesNotEmpty() ? node.Target : null,
				PageIndex = node.NavigationPageIndex >= 0 && doesNotEmpty() ? node.NavigationPageIndex : -1
			};
			return isNotEmpty ? navigation : null;
		}
	}
}
