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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Export;
namespace DevExpress.XtraPrintingLinks
{
	[DefaultProperty("ListView")]
	public class ListViewLinkBase : DevExpress.XtraPrinting.LinkBase 
	{
		#region inner classes
		class BrickFactory {
			const int space = 2;
			ListViewItem item;
			TextBrick textBrick;
			CheckBoxBrick checkBrick;
			ImageBrick imageBrick;
			RectangleF textRect = RectangleF.Empty;
			RectangleF checkRect = RectangleF.Empty;
			RectangleF imageRect = RectangleF.Empty;
			ImageList currentImageList;
			Nullable<PointF> offset = null;
			PointF Offset {
				get {
					if(offset == null) {
						RectangleF rect = item.ListView.GetItemRect(0);
						offset = rect.Location;
					}
					return offset.Value;
				}
			}
			bool CanHaveCheckBox { get { return item.ListView.CheckBoxes; } }
			bool CanHaveImage { get { return currentImageList != null; } }
			bool HaveImage { get { return item.ImageIndex >= 0 && item.ImageIndex < currentImageList.Images.Count; } }
			bool IsLargeImages { get { return item.ListView.View == View.LargeIcon; } }
			Size ImageSize { get { return currentImageList.ImageSize; } }
			public static PanelBrick CreateBrick(ListViewItem item) {
				return (new BrickFactory()).CreateBrickInternal(item);
			}
			private PanelBrick CreateBrickInternal(ListViewItem item) {
				PanelBrick panelBrick = new PanelBrick();
				this.item = item;
				this.currentImageList = IsLargeImages ? item.ListView.LargeImageList : item.ListView.SmallImageList;
				textBrick = new TextBrick();
				textBrick.StringFormat = GetStringFormat();
				textBrick.Text = item.Text;
				textBrick.Sides = BorderSide.None;
				panelBrick.Bricks.Add(textBrick);
				if(CanHaveCheckBox) {
					checkBrick = new CheckBoxBrick();
					checkBrick.Checked = item.Checked;
					checkBrick.Sides = BorderSide.None;
					panelBrick.Bricks.Add(checkBrick);
				}
				if(CanHaveImage && HaveImage) {
					imageBrick = new ImageBrick();
					imageBrick.Image = GetImage();
					imageBrick.Sides = BorderSide.None;
					panelBrick.Bricks.Add(imageBrick);
				}
				CalcLayout();
				textBrick.Rect = textRect;
				if(checkBrick != null) checkBrick.Rect = checkRect;
				if(imageBrick != null) imageBrick.Rect = imageRect;
				panelBrick.Rect = GetItemRect();
				return panelBrick;
			}
			void CalcLayout() {
				if(IsLargeImages)
					CalcLargeIconsLayout();
				else
					CalcSmallIconsLayout();
			}
			void CalcSmallIconsLayout() {
				int height = item.Bounds.Height;
				textRect = GetItemRect();
				textRect.Offset(-textRect.X, 0);
				float x = space;
				if(CanHaveCheckBox) {
					checkRect.Size = GetCheckSize();
					checkRect.X = x;
					checkRect.Y += (height - checkRect.Height) / 2 - 1;
					float OffsetX = checkRect.Right + space;
					x += OffsetX;
					textRect.Width -= OffsetX;
					textRect.Offset(OffsetX, 0);
				}
				if(CanHaveImage) {
					Size imageSize = this.ImageSize;
					imageRect = new RectangleF(x, 0, imageSize.Width, imageSize.Height);
					imageRect.Y += (height - imageRect.Height) / 2;
					x = imageRect.Right + space;
					float OffsetX = imageSize.Width + space;
					textRect.Width -= OffsetX;
					textRect.Offset(OffsetX, 0);
				}
				textRect.Offset(0, -textRect.Y);
			}
			void CalcLargeIconsLayout() {
				RectangleF itemRect = GetItemRect();
				if(CanHaveImage) {
					Size imageSize = this.ImageSize;
					imageRect = new RectangleF((itemRect.Width - imageSize.Width) / 2, 0,
						imageSize.Width, imageSize.Height);
					SizeF checkSize = CanHaveCheckBox ? GetCheckSize() : SizeF.Empty;
					checkRect = new RectangleF(imageRect.Left - checkSize.Width - space,
						imageRect.Bottom - checkSize.Height, checkSize.Width, checkSize.Height);
					textRect = new RectangleF(space, imageRect.Bottom,
						itemRect.Width - space, itemRect.Height - imageRect.Height);
				}
				else {
					SizeF checkSize = CanHaveCheckBox ? GetCheckSize() : SizeF.Empty;
					checkRect = new RectangleF(space, (itemRect.Height - checkSize.Height) / 2,
						checkSize.Width, checkSize.Height);
					int dx = CanHaveCheckBox ? 2 * space : 0;
					textRect = new RectangleF(checkSize.Width + dx, 0,
						itemRect.Width - (checkSize.Width + dx / 2) + 2,
						itemRect.Height + 2);
				}
			}
			RectangleF GetItemRect() {
				RectangleF r = item.Bounds;
				r.Offset(-Offset.X, 0);
				if(r.Width > item.ListView.Width)
					r.Width = item.ListView.Width;
				if(item.ListView.View == View.Details) {
					if(item.ListView.Columns.Count > 0)
						r.Width = item.ListView.Columns[0].Width;
					r.Y -= item.ListView.Items[0].Bounds.Y - 2;
				}
				return r;
			}
			SizeF GetCheckSize() {
				System.Diagnostics.Debug.Assert(checkBrick != null);
				return checkBrick.CheckSize;
			}
			Image GetImage() {
				Image img = null;
				if(CanHaveImage && HaveImage)
					img = currentImageList.Images[item.ImageIndex];
				return img;
			}
			BrickStringFormat GetStringFormat() {
				StringFormat stringFormat;
				if(IsLargeImages) {
					stringFormat = new StringFormat(StringFormatFlags.LineLimit);
					stringFormat.LineAlignment = StringAlignment.Center;
					stringFormat.Alignment = StringAlignment.Center;
				}
				else {
					stringFormat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.LineLimit);
					stringFormat.LineAlignment = StringAlignment.Near;
					stringFormat.Alignment = StringAlignment.Near;
				}
				stringFormat.Trimming = StringTrimming.EllipsisCharacter;
				return new BrickStringFormat(stringFormat);
			}
		}
		#endregion
		private ListView listView;
		private ImageList imageList;
		private int offsetx;
		private BrickGraphics graph;
		public override Type PrintableObjectType {
			get { return typeof(ListView); }
		}
		[
		Category(NativeSR.CatPrinting),
		DefaultValue(null),
		]
		public ListView ListView {
			get { return listView; }
			set { listView = value; }
		}
		public ListViewLinkBase() {
		}
		protected override void BeforeCreate() {
			if(ListView == null)
				throw new NullReferenceException("The ListView property value must not be null");
			base.BeforeCreate();
			ps.Graph.PageUnit = GraphicsUnit.Pixel;
			imageList = ViewEquals(View.SmallIcon,View.List,View.Details) ? listView.SmallImageList :
				(listView.View == View.LargeIcon) ? listView.LargeImageList : null;
			offsetx = (imageList == null) ? 0 : imageList.ImageSize.Height;
			graph = ps.Graph;
		}
		public override void SetDataObject(object data) {
			if(data is ListView)
				listView = data as ListView;
		}
		public override void AddSubreport(PointF offset) {
			if (listView != null) 
				base.AddSubreport(offset);
		}
		protected override void CreateDetailHeader(BrickGraphics gr) {
			if(listView.View != View.Details) return;
			StringFormat sf = new StringFormat(StringFormatFlags.NoWrap);
			sf.LineAlignment = StringAlignment.Near;
			gr.DefaultBrickStyle = new BrickStyle(BorderSide.Left | BorderSide.Top | BorderSide.Right | BorderSide.Right | BorderSide.Bottom, 1, Color.Black,
				SystemColors.Control, SystemColors.ControlText, listView.Font, new BrickStringFormat(sf));
			Rectangle r = Rectangle.Empty;
			r.Y = 1;
			for (int i = 0; i < listView.Columns.Count; i++) {
				r.Width = listView.Columns[i].Width;
				r.Height = listView.Font.Height + 4;
				graph.DrawString(listView.Columns[i].Text, r);
				r.Offset(listView.Columns[i].Width, 0);
			}
		}
		protected override void CreateDetail(BrickGraphics gr) {
			if(listView.View == View.Details) CreateDetails();
			else if( ViewEquals(View.LargeIcon, View.SmallIcon, View.List) ) 
				CreateIcons();
		}
		private bool ViewEquals(params View[] views) {
			foreach(View view in views)
				if(listView.View == view) return true;
			return false;
		}
		private void CreateIcons() {
			graph.DefaultBrickStyle.BackColor = Color.Transparent;
			graph.DefaultBrickStyle.BorderColor = Color.Black;
			graph.DefaultBrickStyle.Sides = BorderSide.None;
			for (int i = 0; i < listView.Items.Count; i++) {
				ListViewItem item = listView.Items[i];
				graph.DefaultBrickStyle.Font = item.Font;
				graph.DefaultBrickStyle.BackColor = item.BackColor;
				graph.DefaultBrickStyle.ForeColor = item.ForeColor;
				PanelBrick brick = BrickFactory.CreateBrick(item);
				RectangleF r = brick.Rect;
				graph.DrawBrick(brick, r);
			}
		}
		private void DrawDetailRow(ListViewItem item) {
			PanelBrick brick = BrickFactory.CreateBrick(item);
			RectangleF r = brick.Rect;
			graph.DrawBrick(brick, r);
			int n = ListView.Columns.Count;
			for (int i = 1; i < n; i++) {
				r.Offset(r.Width, 0);
				r.Width = ListView.Columns[i].Width;
				bool validSubItem = i < item.SubItems.Count;
				if (validSubItem && !item.UseItemStyleForSubItems) {
					ListViewItem.ListViewSubItem subItem = item.SubItems[i];
					graph.DefaultBrickStyle.Font = subItem.Font;
					graph.DefaultBrickStyle.BackColor = subItem.BackColor;
					graph.DefaultBrickStyle.ForeColor = subItem.ForeColor;
				}
				string s = validSubItem ? item.SubItems[i].Text : string.Empty;
				graph.DrawString(s, r);
			}
		}
		private void CreateDetails() {
			graph.DefaultBrickStyle.BackColor = SystemColors.Window;
			graph.DefaultBrickStyle.BorderColor = SystemColors.Control;
			graph.DefaultBrickStyle.Sides = listView.GridLines ? BorderSide.Left | BorderSide.Top | BorderSide.Right | BorderSide.Right | BorderSide.Bottom : BorderSide.None;
			for(int i = 0; i < listView.Items.Count; i++) {
				ListViewItem item = listView.Items[i];
				graph.DefaultBrickStyle.Font = item.Font;
				graph.DefaultBrickStyle.BackColor = item.BackColor;
				graph.DefaultBrickStyle.ForeColor = item.ForeColor;
				DrawDetailRow(item);
			}
		}
	}
}
