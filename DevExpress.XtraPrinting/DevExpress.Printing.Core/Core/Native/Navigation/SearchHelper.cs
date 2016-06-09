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
using System.Drawing;
using System.Collections;
using DevExpress.XtraPrinting.Localization;
using System.Collections.Generic;
namespace DevExpress.XtraPrinting.Native.Navigation {
	public class SearchHelperBase {
		int brickIndex = -1;
		FindState state = FindState.None;
		BrickPagePairCollection brickPagePairs = new BrickPagePairCollection();
		Guid documentID = Guid.Empty;
		BrickPagePairCollection SelectBrickPagePairs(IPrintingSystemContext context, string text, bool matchWholeWord, bool matchCase) {
			if(context == null || context.PrintingSystem == null)
				return new BrickPagePairCollection();
			PageList pages = context.PrintingSystem.Pages;
			TextBrickSelector selector = new TextBrickSelector(text, matchWholeWord, matchCase, context);
			return NavigateHelper.SelectBrickPagePairs(pages, selector, new BrickPagePairComparer(pages));
		}
		void UpdatePages(PrintingDocument document) {
			if(document == null) {
				ResetSearchResults();
				documentID = Guid.Empty;
			} else if(document.ContentIdentity != documentID) {
				ResetSearchResults();
				documentID = document.ContentIdentity;
			}
		}
		protected int GetBrickIndex(SearchDirection searchDirection, bool isSearchFinished) {
			if(isSearchFinished && brickPagePairs.Count > 0) {
				if(searchDirection == SearchDirection.Down && brickIndex == brickPagePairs.Count - 1) return 0;
				else if(searchDirection == SearchDirection.Up && brickIndex == 0) return brickPagePairs.Count - 1;
			}
			return CalcIndex(searchDirection);
		}
		public void ResetSearchResults() {
			state = FindState.None;
		}
		int CalcIndex(SearchDirection searchDirection) {
			int index = brickIndex;
			if(brickPagePairs.Count == 0)
				return -1;
			if(searchDirection == SearchDirection.Up)
				index--;
			else
				index++;
			return Math.Max(0, Math.Min(index, brickPagePairs.Count - 1));
		}
		public bool SearchFinished(SearchDirection searchDirection) {
			return state != FindState.Next ?
				true :
				CalcIndex(searchDirection) == brickIndex;
		}
		public BrickPagePair CircleFindNext(PrintingSystemBase ps, string text, SearchDirection searchDirection, bool matchWholeWord, bool matchCase) {
			FindNextCore(ps, text, searchDirection, matchWholeWord, matchCase);
			brickIndex = GetBrickIndex(searchDirection, SearchFinished(searchDirection));
			if(brickIndex >= 0)
				return brickPagePairs[brickIndex];
			return null;
		}
		public void Reset() {
			brickPagePairs.Clear();
			state = FindState.None;
		}
		void FindNextCore(PrintingSystemBase ps, string text, SearchDirection searchDirection, bool matchWholeWord, bool matchCase) {
			UpdatePages(ps != null ? ps.PrintingDocument : null);
			if(state != FindState.Next) {
				brickPagePairs = SelectBrickPagePairs(ps, text, matchWholeWord, matchCase);
				brickIndex = -1;
			}
			state = FindState.Next;
		}
	}
}
