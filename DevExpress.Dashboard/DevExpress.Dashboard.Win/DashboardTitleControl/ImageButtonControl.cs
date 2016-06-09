#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class ImageButtonControl : SmallImageControl {
		bool isMouseOver = false;
		new ClickableImageButton Image { get { return (ClickableImageButton)base.Image; } }
		public ImageButtonControl(string imageName, Action action, string toolTip) :
			base(imageName) {
			Image.Action = action;
			ToolTip = toolTip;
			ImmediateTooltip = false;
		}
		protected override ResourceColorImage CreateImage(string imageName) {
			return new ClickableImageButton(imageName);
		}
		void SetState(DragAreaButtonState state) {
			if(Image != null) {
				Image.SetOptionsButtonState(state);
				Invalidate();
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(Image == null)
				base.OnPaint(e);
			using(GraphicsCache cache = new GraphicsCache(e))
				Image.Paint(ImageColor, cache);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			SetState(DragAreaButtonState.Normal);
			isMouseOver = false;
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			SetState(DragAreaButtonState.Hot);
			isMouseOver = true;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			SetState(DragAreaButtonState.Selected);
			if(Image != null)
				Image.Execute();
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			SetState(isMouseOver ? DragAreaButtonState.Hot : DragAreaButtonState.Normal);
		}
		internal void ForceExecuteImageButton() {
			Image.Execute();
		}
	}
	public class ClickableImageButton : ImageButton {
		public Action Action { get; set; }
		public ClickableImageButton(string name)
			: base(name) {
		}
		public void Execute() {
			if(Action != null)
				Action();
		}
		protected override float GetStateColor(DragAreaButtonState state) {
			switch(state) {
				case DragAreaButtonState.Disabled:
				return 0.15f;
				case DragAreaButtonState.Hot:
				return 1.0f;
				case DragAreaButtonState.Normal:
				return 0.65f;
				case DragAreaButtonState.Selected:
				return 0.35f;
				default:
				return 0.65f;
			}
		}
	}
}
