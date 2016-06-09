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
using DevExpress.XtraPrinting;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Printing.Native {
	public class DocumentMapBuilder {
		Dictionary<BookmarkInfo, BookmarkNode> bookmarkNodesHT;
		public void Build(Document document) {
			document.Bookmark = document.Name;
			bookmarkNodesHT = new Dictionary<BookmarkInfo, BookmarkNode>();
			foreach(Page page in document.Pages) {
				NestedBrickIterator iterator = new NestedBrickIterator(new[] { page });
				while(iterator.MoveNext()) {
					VisualBrick visualBrick = iterator.CurrentBrick as VisualBrick;
					if(visualBrick == null)
						continue;
					BookmarkInfo bookmarkInfo = visualBrick.BookmarkInfo;
					if(bookmarkInfo != null && bookmarkInfo.HasBookmark) {
						BookmarkNode bookmarkNode = GetBookmarkNode(bookmarkInfo, visualBrick, page);
						if(bookmarkInfo.ParentInfo != null) {
							BookmarkNode parentBookmarkNode = GetBookmarkNode(bookmarkInfo.ParentInfo, null, null);
							parentBookmarkNode.Nodes.Add(bookmarkNode);
						} else {
							document.BookmarkNodes.Add(bookmarkNode);
						}
					}
				}
			}
			bookmarkNodesHT.Clear();
		}
		BookmarkNode GetBookmarkNode(BookmarkInfo bookmarkInfo, VisualBrick brick, Page page) {
			BookmarkNode bookmarkNode;
			if(!bookmarkNodesHT.TryGetValue(bookmarkInfo, out bookmarkNode)) {
				bookmarkNode = new BookmarkNode(bookmarkInfo.Bookmark, brick, page);
				bookmarkNodesHT[bookmarkInfo] = bookmarkNode;
			}
			return bookmarkNode;
		}
	}
}
