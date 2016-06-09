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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace DevExpress.Web.Design {
	public class ListBoxItemPainter {
		private List<ListBox> fListBoxes = new List<ListBox>();
		private bool fShowIndexButtons;
		private DrawItemState fDrawItemState;
		public event DrawItemEventDelegate DrawItemEvent;
		public ListBoxItemPainter(bool showIndexButtons) {
			fShowIndexButtons = showIndexButtons;
		}
		public ListBox CreateListBox(Font font) {
			ListBox listBox = ControlCreator.CreateListBox();
			listBox.DrawMode = DrawMode.OwnerDrawFixed;
			listBox.Font = font;
			listBox.ItemHeight = font.Height + GetBorderSize().Height * 2;
			listBox.DrawItem += new DrawItemEventHandler(OnListBoxDrawItem);
			fListBoxes.Add(listBox);
			return listBox;
		}
		protected virtual void DrawBackground(ListBox listBox, DrawItemEventArgs drawItemInfo) {
			drawItemInfo.DrawBackground();
		}
		protected virtual void DrawIndexButton(ListBox listBox, DrawItemEventArgs drawItemInfo) {
			Rectangle buttonRectangle = drawItemInfo.Bounds;
			buttonRectangle.Width = GetIndexButtonWidth(listBox, drawItemInfo);
			ControlPaint.DrawButton(drawItemInfo.Graphics, buttonRectangle, ButtonState.Normal);
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			drawItemInfo.Graphics.DrawString(GetIndexButtonText(drawItemInfo),
				listBox.Font, SystemBrushes.ControlText, buttonRectangle, stringFormat);
		}
		protected virtual void DrawText(ListBox listBox, DrawItemEventArgs drawItemInfo) {
			if (drawItemInfo.Index < 0) return;
			Rectangle textRectangle = drawItemInfo.Bounds;
			if (fShowIndexButtons) {
				textRectangle.X += GetIndexButtonWidth(listBox, drawItemInfo) + 1;
				textRectangle.Width -= GetIndexButtonWidth(listBox, drawItemInfo) + 1;
			}
			StringFormat stringFormat = new StringFormat(StringFormatFlags.NoWrap);
			stringFormat.LineAlignment = StringAlignment.Center;
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			FontStyle fontStyle = FontStyle.Regular;
			if ((fDrawItemState & DrawItemState.HotLight) == DrawItemState.HotLight)
				fontStyle = fontStyle | FontStyle.Bold;
			using (Font font = new Font(listBox.Font, fontStyle)) {
				drawItemInfo.Graphics.DrawString(listBox.Items[drawItemInfo.Index].ToString(),
					font, IsSelected(drawItemInfo) ? SystemBrushes.HighlightText : SystemBrushes.ControlText, textRectangle, stringFormat);
			}
		}
		protected virtual void DrawItem(ListBox listBox, DrawItemEventArgs drawItemInfo) {
			if (listBox.Items.Count == 0) return;
			DrawBackground(listBox, drawItemInfo);
			if (fShowIndexButtons) DrawIndexButton(listBox, drawItemInfo);
			DrawText(listBox, drawItemInfo);
		}
		private Size GetBorderSize() {
			return SystemInformation.Border3DSize;
		}
		private string GetIndexButtonText(DrawItemEventArgs drawItemInfo) {
			return drawItemInfo.Index.ToString();
		}
		private int GetIndexButtonWidth(ListBox listBox, DrawItemEventArgs drawItemInfo) {
			string maxIndexButtonText = (listBox.Items.Count - 1).ToString();
			return drawItemInfo.Graphics.MeasureString(maxIndexButtonText, listBox.Font).ToSize().Width + GetBorderSize().Width * 2;
		}
		private void OnDrawItemEvent(DrawListItemEventArgs drawItemEventArgs) {
			if (DrawItemEvent != null)
				DrawItemEvent(this, drawItemEventArgs);
		}
		private bool IsSelected(DrawItemEventArgs drawItemInfo) {
			return (drawItemInfo.State & DrawItemState.Selected) == DrawItemState.Selected;
		}
		private void OnListBoxDrawItem(object sender, DrawItemEventArgs e) {
			ListBox listBox = sender as ListBox;
			if (e.Index != -1) {
				DrawListItemEventArgs args = new DrawListItemEventArgs(listBox.Items[e.Index].ToString(), DrawItemState.None);
				OnDrawItemEvent(args);
				fDrawItemState = args.DrawItemState;
				DrawItem(listBox, e);
			}
		}
	}
	public delegate void DrawItemEventDelegate(object sender, DrawListItemEventArgs e);
	public class DrawListItemEventArgs : EventArgs {
		private DrawItemState fDrawItemState;
		private string fItemText;
		public DrawItemState DrawItemState {
			get { return fDrawItemState; }
			set { fDrawItemState = value; }
		}
		public string ItemText {
			get { return fItemText; }
			set { fItemText = value; }
		}
		public DrawListItemEventArgs(string itemText, DrawItemState drawItemState) {
			fDrawItemState = drawItemState;
			fItemText = itemText;
		}
	}
}
