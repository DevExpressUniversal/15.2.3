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
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
namespace DevExpress.XtraPrinting.Native
{
	[ToolboxItem(false)]
	public class ImageContextMenu : ContextMenu
	{
		private IList images;
		private int width;
		private Font Font { get { return System.Windows.Forms.Control.DefaultFont; }
		}
		internal IList Images { 
			get { return images; } 
			set { 
				images = value;
				CreateContent();
			}
		}
		internal event EventHandler ItemClick;
		private void DoItemClick(object sender, System.EventArgs e) {
			if(ItemClick != null) ItemClick(sender, e);
		}
		internal ImageContextMenu(int width) {
			this.width = width;
		}
		protected void CreateContent() {
			MenuItems.Clear();
			if(images == null) return;
			for(int i = 0; i < images.Count; i++) {
				MenuItem item = new MenuItem( i.ToString() );
				item.OwnerDraw = true;
				item.DrawItem += new DrawItemEventHandler(menuItem_DrawItem);
				item.MeasureItem += new MeasureItemEventHandler(menuItem_MeasureItem);
				item.Click += new System.EventHandler(menuItem_Click);
				MenuItems.Add(item);
			}
		}
		private void menuItem_Click(object sender, System.EventArgs e) {
			DoItemClick(sender, e);
		}
		private void menuItem_MeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e) {
			e.ItemWidth = width;
			e.ItemHeight = Font.Height + 4;
		}
		private void menuItem_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e) {
			bool selected = (e.State & DrawItemState.Selected) > 0;
			if(selected) {
				e.DrawBackground();
			} else {
				e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
			}
			Rectangle r = new Rectangle(e.Bounds.X, e.Bounds.Y, (int)(1.5 * e.Bounds.Height), e.Bounds.Height);
			r.Inflate(-2, -2);
			e.Graphics.DrawImage((Image)images[e.Index], r);
			e.Graphics.DrawRectangle(new Pen(SystemBrushes.ControlText), r);
			r.Offset(r.Width + r.Height, 0);
			StringFormat sf = new StringFormat();
			sf.LineAlignment = StringAlignment.Center;
			e.Graphics.DrawString(((MenuItem)sender).Text, Font, new SolidBrush(e.ForeColor), r, sf);
		}
	}
}
