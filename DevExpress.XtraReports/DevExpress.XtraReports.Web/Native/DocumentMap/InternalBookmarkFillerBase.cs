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
using DevExpress.Printing.Core.Native;
namespace DevExpress.XtraReports.Web.Native.DocumentMap {
	public abstract class InternalBookmarkFillerBase<T, V> : IBookmarkFiller<T>
		where V : IBookmarkNode {
		static int GetPageNumber(V bookmarkNode) {
			return Math.Max(0, bookmarkNode.PageIndex);
		}
		readonly string viewerClientId;
		protected abstract V RootBookmark { get; }
		protected InternalBookmarkFillerBase(string viewerClientId) {
			this.viewerClientId = viewerClientId;
		}
		public virtual void Fill(T parent) {
			var rootNode = RootBookmark;
			if(rootNode != null) {
				FillNodeInternal(parent, rootNode);
			}
		}
		void IBookmarkFiller<T>.Fill(T parent) {
			Fill(parent);
		}
		protected abstract T FillNode(T parent, V bookmarkNode, string navigationScript);
		void FillNodeInternal(T parent, V bookmarkNode) {
			T child = FillNode(parent, bookmarkNode, GetNavigationScript(bookmarkNode));
			if(child == null)
				return;
			foreach(V item in bookmarkNode.Nodes)
				FillNodeInternal(child, item);
		}
		string GetNavigationScript(V bookmarkNode) {
			var indexes = BookmarkChecker.GetIndicesForPath(bookmarkNode.Indices);
			return string.Format("ASPx.RVGotoBM('{0}',{1},'{2}')", viewerClientId, GetPageNumber(bookmarkNode), indexes);
		}
	}
}
