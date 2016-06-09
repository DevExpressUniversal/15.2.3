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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
#else
using System.Windows;
#endif
namespace DevExpress.XtraRichEdit.Internal.PrintLayout {
	#region PrintLayoutViewPageViewInfoGenerator
	public class PrintLayoutViewPageViewInfoGenerator : PageViewInfoGenerator {
		HorizontalAlignment pageHorizontalAlignment = HorizontalAlignment.Center;
		int maxHorizontalPageCount;
		public PrintLayoutViewPageViewInfoGenerator(RichEditView view)
			: base(view) {
		}
		public HorizontalAlignment PageHorizontalAlignment { get { return pageHorizontalAlignment; } set { pageHorizontalAlignment = value; } }
		public int MaxHorizontalPageCount { get { return maxHorizontalPageCount; } set { maxHorizontalPageCount = value; } }
		protected internal override PageGeneratorLayoutManager CreateEmptyClone() {
			return new PrintLayoutViewPageViewInfoGenerator(View);
		}
		protected internal override int CalculateFirstPageLeftOffset(int totalPagesWidth) {
			switch (PageHorizontalAlignment) {
				default:
				case HorizontalAlignment.Center:
					return (ViewPortBounds.Width - totalPagesWidth) / 2;
				case HorizontalAlignment.Left:
					return 0;
				case HorizontalAlignment.Right:
					return (ViewPortBounds.Width - totalPagesWidth);
			}
		}
		public override bool CanFitPageToPageRow(Page page, PageViewInfoRow row) {
			if (MaxHorizontalPageCount <= 0)
				return base.CanFitPageToPageRow(page, row);
			else {
				if (row.Count >= MaxHorizontalPageCount)
					return false;
				else
					return base.CanFitPageToPageRow(page, row);
			}
		}
	}
	#endregion
}
