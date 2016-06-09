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
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout;
#if !SL
using System.Drawing;
#endif
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.LayoutEngine;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Internal.SimpleLayout {
	public class SimpleViewPageViewInfoGenerator : PageViewInfoGenerator {
		public SimpleViewPageViewInfoGenerator(SimpleView view)
			: base(view) {
		}
		public override int HorizontalPageGap { get { return 0; } set { } }
		public override int VerticalPageGap { get { return 0; } set { } }
		internal new SimpleView View { get { return (SimpleView)base.View; } }
		public override bool CanFitPageToPageRow(Page page, PageViewInfoRow row) {
			return false;
		}
		protected internal override PageGeneratorLayoutManager CreateEmptyClone() {
			return new SimpleViewPageViewInfoGenerator(View);
		}
		public override void Reset(PageGenerationStrategyType strategy) {
			TotalWidth = long.MinValue;
			VisibleWidth = long.MinValue;
			base.Reset(strategy);
		}
		protected internal override void CalculateWidthParameters() {
			if (TotalWidth == long.MinValue)
				TotalWidth = 100;
			if (VisibleWidth == long.MinValue)
				VisibleWidth = 100;
			long actualTotalWidth = TotalWidth;
			base.CalculateWidthParameters();
			TotalWidth = actualTotalWidth;
		}
		public override ProcessPageResult ProcessPage(Page page, int pageIndex) {
			CalculatePageHeight(page);
			ProcessPageResult result = base.ProcessPage(page, pageIndex);
			TotalWidth = Math.Max(TotalWidth, CalculatePageLogicalTotalWidth(page));
			return result;
		}
		public override int CalculatePageLogicalTotalWidth(Page page) {
			int pageWidth = base.CalculatePageLogicalTotalWidth(page);
			if (View.InternalWordWrap)
				return pageWidth;
			MaxWidthCalculator maxWidthCalculator = new MaxWidthCalculator();
			int maxWidth = maxWidthCalculator.GetMaxWidth(page);
			return Math.Max(pageWidth, maxWidth);
		}
		void CalculatePageHeight(Page page) {
			if (!page.SecondaryFormattingComplete)
				View.Formatter.PerformPageSecondaryFormatting(page);
			int count = page.Comments.Count;
			if (count > 0) {
				int totalCommentsHeight = View.DocumentModel.LayoutUnitConverter.DocumentsToLayoutUnits(6);
				for (int i = 0; i < count; i++) {
					totalCommentsHeight += page.Comments[i].Bounds.Height;
				}
				if (totalCommentsHeight > page.Bounds.Height) {
					int oldHeight = page.Bounds.Height;
					page.Bounds = new Rectangle(page.Bounds.X, page.Bounds.Y, page.Bounds.Width, totalCommentsHeight);
					page.CommentBounds = new Rectangle(page.CommentBounds.X, page.CommentBounds.Y, page.CommentBounds.Width, totalCommentsHeight);
				}
			}
		}
	}
}
