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
using System.Data;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.XtraGrid.Views.Card {
	public class CardCaptionImageEventArgs : EventArgs {
		int rowHandle;
		Image image;
		object imageList;
		int imageIndex;
		public CardCaptionImageEventArgs(int rowHandle, object list) {
			this.rowHandle = rowHandle;
			this.imageList = list;
			this.image = null;
			this.imageIndex = -1;
		}
		public object ImageList { get { return imageList; } set { imageList = value; } }
		public int RowHandle { get { return rowHandle; } }
		public Image Image { get { return image; } set { image = value; } }
		public int ImageIndex { get { return imageIndex; } set { imageIndex = value; } }
		internal Size ImageSize {
			get { 
				if(Image != null) return Image.Size;
				return ImageCollection.IsImageListImageExists(ImageList, ImageIndex) ? ImageCollection.GetImageListSize(ImageList) : Size.Empty;
			}
		}
		internal void DrawImage(GraphicsInfoArgs info, Rectangle bounds) {
			if(ImageSize.IsEmpty) return;
			Rectangle r = bounds;
			r.Size = ImageSize;
			r.X += (bounds.Width - r.Size.Width) / 2;
			r.Y += (bounds.Height - r.Size.Height) / 2;
			if(Image != null)
				info.Cache.Paint.DrawImage(info.Graphics, Image, r.X, r.Y, new Rectangle(Point.Empty, r.Size));
			else {
				ImageCollection.DrawImageListImage(info, ImageList, ImageIndex, r, true);
			}
		}
	}
	public class CardCaptionCustomDrawEventArgs : CustomDrawEventArgs {
		int rowHandle;
		object cardInfo = null;
		public CardCaptionCustomDrawEventArgs(GraphicsCache cache, CardInfo cardInfo, AppearanceObject appearance) : this(cache, cardInfo.CaptionInfo.Bounds, appearance, cardInfo.RowHandle) { 
			this.cardInfo = cardInfo;
		}
		public CardCaptionCustomDrawEventArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, int rowHandle) : base(cache, bounds, appearance) {
			this.rowHandle = rowHandle;
		}
		public string CardCaption {
			get { return ((CardInfo)CardInfo).CaptionInfo.CardCaption; }
			set {
				if(value == null) value = string.Empty;
				((CardInfo)CardInfo).CaptionInfo.CardCaption = value;
			}
		}
		public object CardInfo { get { return cardInfo; } }
		public int RowHandle { get { return rowHandle; } }
	}
	public class FieldHeightEventArgs : EventArgs {
		int rowHandle;
		int fieldHeight;
		GridColumn column;
		public FieldHeightEventArgs(int rowHandle, int fieldHeight, GridColumn column) {
			this.rowHandle = rowHandle;
			this.fieldHeight = fieldHeight;
			this.column = column;
		}
		public int RowHandle { get { return rowHandle; } }
		public GridColumn Column { get { return column; } }
		public int FieldHeight {
			get { return fieldHeight; }
			set {
				if(value < -1) value = -1;
				fieldHeight = value;
			}
		}
	}
	public delegate void FieldHeightEventHandler(object sender, FieldHeightEventArgs e);
	public delegate void CardCaptionCustomDrawEventHandler(object sender, CardCaptionCustomDrawEventArgs e);
	public delegate void CardCaptionImageEventHandler(object sender, CardCaptionImageEventArgs e);
}
