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

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class TimeRulerSkinPainter : TimeRulerPainter {
		UserLookAndFeel lookAndFeel;
		public TimeRulerSkinPainter(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
		}
		public override int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetTopBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int HorizontalOverlap {
			get {
				Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
				if (skin.Properties.GetBoolean(SchedulerSkins.OptHeaderRequireHorzOffset))
					return 1;
				else
					return 0;
			}
		}
		public override int VerticalOverlap {
			get {
				Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
				if (skin.Properties.GetBoolean(SchedulerSkins.OptHeaderRequireVertOffset))
					return 1;
				else
					return 0;
			}
		}
		public override int CurrentTimelineHeight {
			get {
				SkinElementInfo el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, SchedulerSkins.SkinCurrentTimeIndicator);
				return ObjectPainter.CalcObjectMinBounds(null, SkinElementPainter.Default, el).Height;
			}
		}
		public override int CurrentTimeAreaHeight { get { return CurrentTimelineHeight; } }
		protected override void DrawBackground(GraphicsCache cache, TimeRulerViewInfo ruler) {
			SkinElementInfo el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, SchedulerSkins.SkinRuler, ruler.ContentBounds);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, el);
		}
		protected override void DrawHeader(GraphicsCache cache, TimeRulerViewInfo ruler) {
			SkinElementInfo el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, SchedulerSkins.SkinRulerHeader, ruler.HeaderBounds);
			el.ImageIndex = ruler.IsFirst ? 0 : 1;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, el);
		}
		protected internal override void DrawSeparatorLineItem(GraphicsCache cache, TimeRulerSeparatorLineItem item) {
			SkinElementInfo el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, item.SkinElementName, item.Bounds);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, el);
		}
		protected internal override void DrawCurrentTimelineItem(GraphicsCache cache, TimeRulerCurrentTimelineBaseItem item) {
			Rectangle bounds = item.Bounds;
			bounds.Y += bounds.Height / 2;
			SkinElementInfo el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, item.SkinElementName, bounds);
			el.ImageIndex = item.ImageIndex;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, el);
		}
		protected internal override int GetFullWidthByContentWidth(GraphicsCache cache, int contentWidth) {
			Rectangle clientBounds = new Rectangle(0, 0, contentWidth, 0);
			SkinElementInfo el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, SchedulerSkins.SkinRuler, clientBounds);
			return ObjectPainter.CalcBoundsByClientRectangle(cache.Graphics, SkinElementPainter.Default, el, clientBounds).Width;
		}
		protected internal override Rectangle CalcClientBounds(GraphicsCache cache, Rectangle bounds) {
			SkinElementInfo el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, SchedulerSkins.SkinRuler, bounds);
			return ObjectPainter.GetObjectClientRectangle(cache.Graphics, SkinElementPainter.Default, el);
		}
	}
}
