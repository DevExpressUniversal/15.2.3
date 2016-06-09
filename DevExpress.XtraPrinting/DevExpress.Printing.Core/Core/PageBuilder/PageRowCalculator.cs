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
	public class PageRowCalculator : PageRowBuilderBase {
		public float y;
		public float bottomSpan;
		public override bool CanApplyPageBreaks {
			get {
				return false;
			}
			set {
			}
		}
		protected internal override PageRowBuilderBase CreateInternalPageRowBuilder() {
			return this;
		}
		protected override void ResetTopSpan(FillPageResult fillPageResult, DocumentBand docBand) {
		}
		public override void CopyFrom(PageRowBuilderBase source) {
			base.CopyFrom(source);
			if(source is PageRowCalculator)
				this.y = ((PageRowCalculator)source).y;
		}
		protected override PageUpdateData UpdatePageContent(DocumentBand docBand, RectangleF bounds) {
			y = Math.Max(y, (float)(Offset.Y + docBand.SelfHeight));
			y = Math.Min(y, bounds.Height);
			bottomSpan = docBand.BottomSpan;
			return new PageUpdateData(bounds, true);
		}
		protected override void ApplyBottomSpan(float bottomSpan, RectangleF bounds) {
			base.ApplyBottomSpan(bottomSpan, bounds);
			this.bottomSpan += bottomSpan;
		}
		protected override LayoutAdjustment.LayoutAdjuster CreateLayoutAdjuster() {
			return new LayoutAdjustment.LayoutAdjuster(300);
		}
		protected override FillPageResult BuildZOrderMultiColumnInternal(DocumentBand rootBand, RectangleF bounds, MultiColumn mc) {
			ZOderMultiColumnCaclulator builder = new ZOderMultiColumnCaclulator(this);
			builder.CopyFrom(this);
			try {
				return builder.BuildZOrderMultiColumn(rootBand, mc, bounds);
			}
			finally {
				this.CopyFrom(builder);
			}
		}
		protected override bool CanProcessDetail(DocumentBand rootBand, PageBuildInfo pageBuildInfo) {
			if(base.CanProcessDetail(rootBand, pageBuildInfo))
				return true;
			DocumentBand documentBand = rootBand.GetPageFooterBand();
			return documentBand != null && pageBuildInfo.Index == documentBand.Index;
		}
	}
}
