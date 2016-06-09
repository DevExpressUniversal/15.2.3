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
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.XtraPrinting.Native {
	public class PageHeaderFooterRowBuilder : PageHeaderFooterRowBuilderBase {
		public PageHeaderFooterRowBuilder(YPageContentEngine pageContentEngine)
			: base(pageContentEngine) {
		}
		protected override int GetHeaderRowIndex(DocumentBand rootBand) {
			DocumentBand detailContainer = BuildInfoContainer.GetDetailContainer(rootBand, this, RectangleF.Empty);
			DocumentBand detailBand = BuildInfoContainer.GetPrintingDetail(detailContainer);
			return detailBand != null ? detailBand.RowIndex : RenderHistory.GetLastDetailRowIndex(detailContainer != null ? detailContainer : rootBand);
		}
		protected override float GetDocBandHeight(DocumentBand docBand, RectangleF bounds) {
			return BuildHelper.GetDocBandHeight(docBand, bounds, true);
		}
		protected override int GetFooterRowIndex(DocumentBand rootBand) {
			int rowIndex = RenderHistory.GetLastDetailRowIndex(rootBand);
			DocumentBand headerBand = rootBand.GetPageBand(DocumentBandKind.Header);
			if(headerBand != null)
				rowIndex = Math.Max(rowIndex, headerBand.RowIndex);
			return rowIndex;
		}
		protected internal override PageRowBuilderBase CreateInternalPageRowBuilder() {
			return new PageHeaderFooterRowBuilder(PageContentEngine);
		}
	}
}
