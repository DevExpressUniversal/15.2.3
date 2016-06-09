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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraLayout.ViewInfo;
using DevExpress.XtraLayout.Localization;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraLayout.Painting {
	public class SplitterItemPainter : BaseLayoutItemPainter {
		ObjectPainter painterCore;
		protected void UpdatePainter(UserLookAndFeel lookAndFeel) {
			painterCore = SplitterHelper.GetPainter(lookAndFeel);
		}
		public SplitterItemPainter(UserLookAndFeel lookAndFeel) : base() {
			UpdatePainter(lookAndFeel);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			BaseLayoutItemViewInfo vi = e as BaseLayoutItemViewInfo;
			if(vi==null || !vi.Owner.ActualItemVisibility) return;
			SplitterInfoArgs sInfo = new SplitterInfoArgs(false);
			sInfo.Bounds = vi.BoundsRelativeToControl;
			if(sInfo.Bounds.Height > vi.Owner.MaxSize.Height && vi.Owner.MaxSize.Height != 0) sInfo.Bounds = new Rectangle(sInfo.Bounds.Location, new Size(sInfo.Bounds.Width, vi.Owner.MaxSize.Height));
			if(sInfo.Bounds.Width > vi.Owner.MaxSize.Width && vi.Owner.MaxSize.Width != 0) sInfo.Bounds = new Rectangle(sInfo.Bounds.Location, new Size(vi.Owner.MaxSize.Width, sInfo.Bounds.Height));
			sInfo.IsHorizontal = ((SplitterItem)vi.Owner).IsHorizontal;
			sInfo.IsVertical = ((SplitterItem)vi.Owner).IsVertical;
			sInfo.State = vi.Owner.ViewInfo.State;
			sInfo.Cache = e.Cache;
			painterCore.DrawObject(sInfo);
			DrawSelection(vi);
		}
	}
}
