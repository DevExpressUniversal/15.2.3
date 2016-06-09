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
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Layout {
	#region PrintLayoutViewBoxHitTestCalculator
	public class PrintLayoutViewBoxHitTestCalculator : BoxHitTestCalculator {
		public PrintLayoutViewBoxHitTestCalculator(RichEditHitTestRequest request, RichEditHitTestResult result)
			: base(request, result) {
		}
		public override void ProcessPageAreaCollection(PageAreaCollection collection) {
			if (Request.SearchAnyPieceTable) {
				if (ProcessSpecificPageArea(Result.Page.Header, PageAreaHitTestManager.PageAreaKind.Header, collection))
					return;
				if (ProcessSpecificPageArea(Result.Page.Footer, PageAreaHitTestManager.PageAreaKind.Footer, collection))
					return;
				base.ProcessPageAreaCollection(collection);
			}
			else {
				if (Result.Page.Header != null && Object.ReferenceEquals(Request.PieceTable, Result.Page.Header.PieceTable)) {
					ProcessSpecificPageArea(Result.Page.Header, PageAreaHitTestManager.PageAreaKind.Header, collection);
					return;
				}
				if (Result.Page.Footer != null && Object.ReferenceEquals(Request.PieceTable, Result.Page.Footer.PieceTable)) {
					ProcessSpecificPageArea(Result.Page.Footer, PageAreaHitTestManager.PageAreaKind.Footer, collection);
					return;
				}
				if (Object.ReferenceEquals(Request.PieceTable, Request.PieceTable.DocumentModel.MainPieceTable)) {
					base.ProcessPageAreaCollection(collection);
					return;
				}
				collection.RegisterFailedItemHitTest(this);
			}
		}
		protected internal bool ProcessSpecificPageArea(PageArea area, PageAreaHitTestManager.PageAreaKind areaKind, PageAreaCollection collection) {
			if (area == null)
				return false;
			PageAreaHitTestManager manager = (PageAreaHitTestManager)area.CreateHitTestManager(this);
			manager.AreaKind = areaKind;
			manager.CalcHitTest(false);
			if ((Result.Accuracy & HitTestAccuracy.ExactPageArea) != 0) {
				collection.RegisterSuccessfullItemHitTest(this, area);
				return true;
			}
			else {
				if((Request.Accuracy & HitTestAccuracy.ExactPageArea) != 0) {
					collection.RegisterFailedItemHitTest(this);
					return false;
				}
				Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.PageArea);
				return true;
			}
		}
	}
	#endregion
}
