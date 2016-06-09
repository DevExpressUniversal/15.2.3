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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.Xpf.Core.Native;
using PointF = System.Drawing.PointF;
using RectangleF = System.Drawing.RectangleF;
using SizeF = System.Drawing.SizeF;
using DevExpress.XtraPrinting.Native.Enumerators;
namespace DevExpress.Xpf.Printing.PreviewControl.Native.Models {
	public class PageViewModel : BindableBase, IPage {
		Page page;
		Size pageSize;
		Size visibleSize;
		public PageViewModel(Page page) {
			Page = page;
			PageIndex = page.Index;
		}
		public Size VisibleSize {
			get { return visibleSize; }
			private set { SetProperty(ref visibleSize, value, () => VisibleSize); }
		}
		internal Page Page {
			get {
				return page;
			}
			set {
				SetProperty(ref page, value, () => Page);
				pageSize = GetPageSize();
				VisibleSize = GetPageSize();
			}
		}
		bool IPage.IsLoading {
			get { return false; }
		}
		int pageIndex;
		public int PageIndex {
			get { return pageIndex; }
			private set { pageIndex = value; }
		}
		public System.Windows.Size PageSize {
			get { return pageSize; }
		}
		Size GetPageSize() {
			float width = page.PageSize.Width.DocToDip();
			float height = page.PageSize.Height.DocToDip();
			return new Size(width, height);
		}
		Thickness IPage.Margin { get { return new Thickness(5); } }
		internal Brick GetBrick(Point relativePoint, double zoom) {
			var brickBounds = RectangleF.Empty;
			var position = PSUnitConverter.PixelToDoc(relativePoint.ToWinFormsPoint(), (float)zoom);
			Func<BrickBase, PointF, IIndexedEnumerator> method = (item, itemPosition) => {
				if(item.InnerBrickList.Count > 5 * BrickMapConst.Graduation && item is CompositeBrick) {
					((CompositeBrick)item).ForceBrickMap();
					PointF viewOrigin = new PointF(itemPosition.X + item.InnerBrickListOffset.X, itemPosition.Y + item.InnerBrickListOffset.Y);
					return new ReversedMappedEnumerator(((CompositeBrick)item).BrickMap, item.InnerBrickList) { ClipBounds = new RectangleF(position, new SizeF(1, 1)), ViewOrigin = viewOrigin };
				}
				return new ReversedEnumerator(item.InnerBrickList);
			};
			Tuple<Brick, RectangleF> result = new BrickNavigator(page, method) { BrickPosition = PointF.Empty }.FindBrick(position);
			if(result != null) {
				brickBounds = result.Item2;
				return result.Item1;
			}
			return null;
		}
	}
}
