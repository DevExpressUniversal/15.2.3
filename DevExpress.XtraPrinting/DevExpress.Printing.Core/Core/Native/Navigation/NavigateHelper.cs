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
using System.Collections.Generic;
using DevExpress.DocumentView;
namespace DevExpress.XtraPrinting.Native.Navigation {
	public static class NavigateHelper {
		public static BrickPagePairCollection SelectBrickPagePairs(Document documnent, BrickSelector selector, IComparer sortComparer) {
			return SelectBrickPagePairs(documnent.Pages, selector, sortComparer);
		}
		public static BrickPagePairCollection SelectBrickPagePairs(ICollection<Page> pages, BrickSelector selector, IComparer sortComparer) {
			System.Diagnostics.Debug.Assert(selector != null && pages != null);
			BrickPagePairCollection pairs = new BrickPagePairCollection();
			foreach(Page page in pages) {
				NestedBrickIterator iterator = new NestedBrickIterator(page.InnerBrickList) { Offset = PointF.Empty, ClipRect = page.DeflateMinMargins(page.GetRect(PointF.Empty)) };
				while(iterator.MoveNext()) {
					Brick brick = iterator.CurrentBrick as Brick;
					RectangleF clipRect = RectangleF.Intersect(iterator.CurrentBrickRectangle, iterator.CurrentClipRectangle);
					if(brick == null || !brick.IsVisible || clipRect.IsEmpty)
						continue;
					if(selector.CanSelect(brick, iterator.CurrentBrickRectangle, clipRect))
						pairs.Add(BrickPagePair.Create(iterator.GetCurrentBrickIndices(), page));
				}
			}
			pairs.Sort(sortComparer);
			return pairs;
		}
	}
}
