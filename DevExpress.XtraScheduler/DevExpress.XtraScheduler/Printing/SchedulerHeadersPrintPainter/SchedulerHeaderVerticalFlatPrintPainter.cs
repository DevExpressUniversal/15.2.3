﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	public class SchedulerHeaderVerticalFlatPrintPainter : SchedulerHeaderVerticalFlatPainter {
		public override void Draw(GraphicsCache cache, SchedulerHeader header, ISupportCustomDraw customDrawProvider) {
			base.Draw(cache, header, customDrawProvider);
		}
		protected override BBrushes CreateBorderBrushes(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			return new BBrushes(cache, Color.Black, Color.Black);
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetLeftSideRect(viewInfo.TopBorderBounds, 1);
			cache.FillRectangle(brushes.Dark, bounds);
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetRightSideRect(viewInfo.BottomBorderBounds, 1);
			cache.FillRectangle(brushes.Dark, bounds);
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetTopSideRect(viewInfo.LeftBorderBounds, 1);
			cache.FillRectangle(brushes.Dark, bounds);
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetTopSideRect(viewInfo.RightBorderBounds, 1);
			bounds.Inflate(-1, 0);
			cache.FillRectangle(brushes.LightLight, bounds);
		}
	}
}
