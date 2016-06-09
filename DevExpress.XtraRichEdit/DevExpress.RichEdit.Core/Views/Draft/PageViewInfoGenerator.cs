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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Internal.DraftLayout {
	#region DraftViewPageViewInfoGenerator
	public class DraftViewPageViewInfoGenerator : PageViewInfoGenerator {
		public DraftViewPageViewInfoGenerator(RichEditView view)
			: base(view) {
		}
		#region Properties
		public override int HorizontalPageGap { get { return 0; } set { } }
		public override int VerticalPageGap { get { return 0; } set { } }
		#endregion
		public override bool CanFitPageToPageRow(Page page, PageViewInfoRow row) {
			return false;
		}
		protected internal override PageGeneratorLayoutManager CreateEmptyClone() {
			return new DraftViewPageViewInfoGenerator(View);
		}
		protected internal override Rectangle CreateInitialPageViewInfoRowBounds(int y) {
			return new Rectangle(0 , y, 0, 0);
		}
		protected internal override int CalculateFirstPageLeftOffset(int totalPagesWidth) {
			return 0; 
		}
	}
	#endregion
}
