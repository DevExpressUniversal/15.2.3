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

using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraCharts.Designer.Native {
	public class DropDownButtonFlat : DropDownButton {
		DropDownButtonFlatPainter painter = new DropDownButtonFlatPainter(ObjectState.Disabled, false);
		public ObjectState NotDrawButtonState { get { return painter.NotDrawButtonState; } set { painter.NotDrawButtonState = value; } }
		public bool NotDrawButtonOnNormalState { get { return painter.NotDrawButtonOnNormalState; } set { painter.NotDrawButtonOnNormalState = value; } }
		class DropDownButtonFlatPainter : BaseButtonPainter {
			bool notDrawButtonOnNormalState = false;
			ObjectState notDrawButtonState = ObjectState.Disabled;
			internal ObjectState NotDrawButtonState { get { return notDrawButtonState; } set { notDrawButtonState = value; } }
			internal bool NotDrawButtonOnNormalState { get { return notDrawButtonOnNormalState; } set { notDrawButtonOnNormalState = value; } }
			public DropDownButtonFlatPainter(ObjectState notDrawButtonState, bool notDrawButtonOnNormalState) {
				this.notDrawButtonState = notDrawButtonState;
				this.notDrawButtonOnNormalState = notDrawButtonOnNormalState;
			}
			protected override void DrawContent(ControlGraphicsInfoArgs info) {
				BaseButtonViewInfo vi = info.ViewInfo as BaseButtonViewInfo;
				if (DrawAnimated(info))
					return;
				if (vi.ButtonInfo.FillBackground)
					vi.PaintAppearance.FillRectangle(info.Cache, vi.Bounds);
				vi.BorderPainter.DrawObject(new BorderObjectInfoArgs());
				SkinElement elem = CommonSkins.GetSkin(vi.LookAndFeel)[CommonSkins.SkinLayoutItemBackground];
				SkinElementInfo sinfo = new SkinElementInfo(elem, info.Bounds);
				if (info.ViewInfo.State == ObjectState.Hot)
					sinfo.ImageIndex = 1;
				if (info.ViewInfo.State == ObjectState.Normal)
					sinfo.ImageIndex = 0;
				if (info.ViewInfo.State == (ObjectState.Pressed | ObjectState.Hot))
					sinfo.ImageIndex = 2;
				if (info.ViewInfo.State == ObjectState.Disabled)
					sinfo.ImageIndex = 3;
				if ((notDrawButtonState & info.ViewInfo.State) == 0)
					if ((info.ViewInfo.State == ObjectState.Normal && !notDrawButtonOnNormalState) || info.ViewInfo.State != ObjectState.Normal)
						ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, sinfo);
				if (vi.ButtonInfo.Button.Image != null) {
					int x = ((int)(sinfo.Bounds.Width - vi.ButtonInfo.Button.Image.Width) / 2);
					int y = ((int)(sinfo.Bounds.Height - vi.ButtonInfo.Button.Image.Height) / 2);
					XPaint.Graphics.DrawImage(info.Graphics, vi.ButtonInfo.Button.Image, x, y,
						new Rectangle(Point.Empty, vi.ButtonInfo.Button.Image.Size), info.ViewInfo.State != ObjectState.Disabled);
				}
			}
		}
		public DropDownButtonFlat()
			: base() {
			this.SetStyle(System.Windows.Forms.ControlStyles.Selectable, false);
		}
		protected override BaseControlPainter CreatePainter() { return painter; }
	}
}
