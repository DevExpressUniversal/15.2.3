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
using DevExpress.Utils.Drawing;
using System.Drawing;
using System.Collections.ObjectModel;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Skins;
using DevExpress.Utils;
namespace DevExpress.XtraEditors.Drawing {
	public class ListItemActionInfo : ObjectInfoArgs {
		BaseListBoxViewInfo.ItemInfo info;
		public ListItemActionInfo(BaseListBoxViewInfo.ItemInfo info) {
			this.info = info;
		}
		public int ItemIndex { get { return info.Index; } }
		public object Item { get { return info.Item; } }
		public virtual ObjectPainter Painter { get { return new ListItemActionPainter(); } }
	}
	public class ListItemDeleteActionInfo : ListItemActionInfo {
		public ListItemDeleteActionInfo(BaseListBoxViewInfo.ItemInfo info) : base(info) { }
		public override ObjectPainter Painter { get { return new ListItemDeleteActionPainter(); } }
	}
	public class ListItemDeleteActionPainter : ListItemActionPainter {
		[ThreadStatic]
		static Image defaultDeleteImage;
		static Image DefaultDeleteImage {
			get {
				if(defaultDeleteImage == null) {
					defaultDeleteImage = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Images.removelist.png", typeof(BaseEdit).Assembly);
				}
				return defaultDeleteImage;
			}
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(Point.Empty, GetImage(e).GetImageBounds(0).Size);
		}
		SkinImage GetImage(ObjectInfoArgs e) {
			SkinImage img = new SkinImage(DefaultDeleteImage);
			img.ImageCount = 3;
			img.Layout = SkinImageLayout.Vertical;
			return img;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			int index = 1;
			if(e.State == ObjectState.Hot) index = 0;
			if(e.State == ObjectState.Pressed) index = 2;
			SkinImage image = GetImage(e);
			e.Paint.DrawImage(e.Graphics, image.Image, e.Bounds, image.GetImageBounds(index), null);
		}
	}
	public class ListItemActionPainter : ObjectPainter {
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(0, 0, 10, 10);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			Brush brush = Brushes.Red;
			if(e.State == ObjectState.Hot) brush = Brushes.Green;
			if((e.State & ObjectState.Pressed) == ObjectState.Pressed) brush = Brushes.Yellow;
			e.Cache.FillRectangle(brush, e.Bounds);
		}
	}
	public class ListItemActionCollection : Collection<ListItemActionInfo> {
		public Rectangle Bounds {
			get {
				Rectangle res = Rectangle.Empty;
				if(Count == 0) return Rectangle.Empty;
				for(int n = 0; n < Count; n++) {
					ListItemActionInfo item = this[n];
					if(item.Bounds.IsEmpty) continue;
					if(res.IsEmpty) {
						res = item.Bounds;
					}
					else {
						res.Width = item.Bounds.Right - res.X;
					}
				}
				return res;
			}
		}
	}
}
