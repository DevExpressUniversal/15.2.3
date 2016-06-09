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

using DevExpress.XtraPrinting.Native;
using System;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraPrinting.Native {
	public static class PageCustomBrickHelper {
		public static RectangleF AlignRect(Brick brick, ReadonlyPageData pageData) {
			RectangleF rect = brick.InitialRect;
			if(pageData != null) {
				if(brick.Modifier == BrickModifier.MarginalHeader) {
					return GetAlignedRect(brick, rect, pageData.PageHeaderRect);
				} else if(brick.Modifier == BrickModifier.MarginalFooter) {
					return GetAlignedRect(brick, rect, pageData.PageFooterRect);
				}
			}
			return rect;
		}
		public static ReadonlyPageData GetPageData(Page page, PrintingSystemBase printingSystem) {
			return page != null ? page.PageData :
				printingSystem != null ? printingSystem.PageSettings.Data :
				null;
		}
		static RectangleF GetAlignedRect(Brick brick, RectangleF rect, RectangleF baseRect) {
			baseRect.Location = PointF.Empty;
			IPageBrick pageBrick = (IPageBrick)brick;
			return RectF.Align(rect, baseRect, ForceAligment(pageBrick.Alignment, BrickAlignment.Near), pageBrick.LineAlignment);
		}
		static BrickAlignment ForceAligment(BrickAlignment aligment, BrickAlignment forceAligment) {
			if(aligment == BrickAlignment.None)
				return forceAligment;
			return aligment;
		}
	}
}
