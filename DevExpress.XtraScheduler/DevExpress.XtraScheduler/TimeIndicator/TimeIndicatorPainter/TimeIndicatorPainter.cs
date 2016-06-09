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

using DevExpress.Utils.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class TimeIndicatorPainter : BorderObjectPainter, IViewInfoItemPainter {
		#region Properties
		#endregion
		public virtual void DrawTimeIndicator(GraphicsCache cache, TimeIndicatorViewInfo timeIndicator, ISupportCustomDraw provider) {
			DefaultDrawDelegate defaultDraw = delegate() { DrawTimeIndicatorCore(cache, timeIndicator); };
			if (RaiseTimeIndicatorCustomDrawEvent(cache, timeIndicator, provider, defaultDraw))
				return;
			defaultDraw();
		}
		public void DrawTextItem(GraphicsCache cache, ViewInfoTextItem item) {
		}
		public void DrawVerticalTextItem(GraphicsCache cache, ViewInfoVerticalTextItem item) {
		}
		public void DrawImageItem(GraphicsCache cache, ViewInfoImageItem item) {
		}
		public void DrawHorizontalLineItem(GraphicsCache cache, ViewInfoHorizontalLineItem item) {
		}
		protected virtual bool RaiseTimeIndicatorCustomDrawEvent(GraphicsCache cache, TimeIndicatorViewInfo timeIndicator, ISupportCustomDraw provider, DefaultDrawDelegate defaultDrawDelegate) {
			timeIndicator.Cache = cache;
			try {
				CustomDrawObjectEventArgs args = new CustomDrawObjectEventArgs(timeIndicator, timeIndicator.Bounds, defaultDrawDelegate);
				provider.RaiseCustomDrawTimeIndicator(args);
				return args.Handled;
			} finally {
				timeIndicator.Cache = null;
			}
		}
		protected virtual void DrawTimeIndicatorCore(GraphicsCache cache, TimeIndicatorViewInfo timeIndicator) {
			DrawCurrentTimeItems(cache, timeIndicator.Items);
		}
		protected internal virtual void DrawCurrentTimeItem(GraphicsCache cache, TimeIndicatorBaseItem item) {
			cache.FillRectangle(Color.Black, item.Bounds);
		}
		protected internal virtual void DrawCurrentTimeItems(GraphicsCache cache, ViewInfoItemCollection items) {
			int itemCount = items.Count;
			for (int i = 0; i < itemCount; i++)
				items[i].Draw(cache, this);
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
	}
}
