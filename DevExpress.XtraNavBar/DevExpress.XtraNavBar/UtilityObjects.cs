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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraNavBar.ViewInfo {
	[ToolboxItem(false)]
	public class NavVScrollBar : DevExpress.XtraEditors.VScrollBar {
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
		}
	}
	public class UpDownButtonObjectInfoArgs : StyleObjectInfoArgs {
		bool isUpButton;
		ObjectPainter buttonPainter;
		public UpDownButtonObjectInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, ObjectState state, ObjectPainter buttonPainter, bool isUpButton) : base(cache, bounds, appearance, state) {
			this.isUpButton = isUpButton;
			this.buttonPainter = buttonPainter;
		}
		public bool IsUpButton {
			get { return isUpButton; } 
		}
		public ObjectPainter ButtonPainter {
			get { return buttonPainter; } 
		}
	}
	public class UpDownButtonObjectPainter : StyleObjectPainter {
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(0, 0, 16, 16);
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			UpDownButtonObjectInfoArgs args = e as UpDownButtonObjectInfoArgs;
			args.ButtonPainter.DrawObject(e);
			Rectangle r = args.ButtonPainter.GetObjectClientRectangle(e);
			DrawArrow(e, r, args.IsUpButton);
		}
		protected virtual void DrawArrow(ObjectInfoArgs e, Rectangle client, bool isUp) {
			AppearanceObject style = GetStyle(e);
			DrawShape(e.Cache, client, style.GetForeColor(), isUp, e.State == ObjectState.Disabled);
		}
		void DrawShape(GraphicsCache cache, Rectangle rect, Color color, bool isUp, bool disabled) {
			int count = rect.Height - 8;
			if(count > 5) count = 5;
			int startY = (rect.Height - count) / 2;
			int middle = rect.Width / 2 - 1;
			Brush darkBrush = cache.GetSolidBrush((disabled ? ControlPaint.LightLight(color) : ControlPaint.Dark(color)));
			int delta = isUp ? 1 : -1, y = 0;
			for(int n = (delta < 0 ? count - 1 : 0) ; (delta < 0 ? n >= 0 : n < count); n += delta) {
				cache.Graphics.FillRectangle(darkBrush, rect.X + middle - n, rect.Y + startY + y, (n * 2) + 1, 1);
				y ++;
			}
		}
	}
}
