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

using System;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DragDrop;
namespace DevExpress.DashboardWin.Native {
	public class DragAreaDrawingContext : IDisposable {
		public const int SplitterHeight = 2;
		static StringFormat GetStringFormat(AppearanceObject appearance) {
			return appearance.GetStringFormat(TextOptions.DefaultOptionsCenteredWithEllipsis);
		}
		public static Size MeasureString(string str, AppearanceObject appearance, GraphicsCache cache) {
			Size size = appearance.CalcTextSize(cache, GetStringFormat(appearance), str, 0).ToSize();
			size.Width++;
			size.Height++;
			return size;
		}
		public static void DrawString(string str, AppearanceObject appearance, GraphicsCache cache, Rectangle bounds) {
			DrawString(str, appearance, cache, bounds, null);
		}
		public static void DrawString(string str, AppearanceObject appearance, GraphicsCache cache, Rectangle bounds, Color? color) {
			if(color.HasValue)
				appearance.DrawString(cache, str, bounds, cache.GetSolidBrush(color.Value), GetStringFormat(appearance));
			else
				appearance.DrawString(cache, str, bounds, GetStringFormat(appearance));
		}
		readonly DragAreaAppearances appearances;
		DragAreaPainters painters;
		int sectionMinWidth;
		int sectionWidth;
		int dragItemHeight;
		int dragItemBitmapWidth;
		public DragAreaAppearances Appearances { get { return appearances; } }
		public DragAreaPainters Painters { get { return painters; } }
		public int SectionWidth { get { return sectionWidth; } }
		public int DragItemHeight { get { return dragItemHeight; } }
		public int SectionMinWidth { get { return sectionMinWidth; } }
		public DragAreaDrawingContext(UserLookAndFeel lookAndFeel) {
			appearances = new DragAreaAppearances(lookAndFeel);
			Update(lookAndFeel);
		}
		public void Dispose() {
			appearances.Dispose();
			GC.SuppressFinalize(this);
		}
		public void Update(UserLookAndFeel lookAndFeel) {
			appearances.Update(lookAndFeel);
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				painters =  new DragAreaSkinPainters(lookAndFeel);
			else 
				painters = new DragAreaStylePainters();
			GraphicsInfo graphicsInfo = GraphicsInfo.Default;
			try {
				using (GraphicsCache cache = new GraphicsCache(graphicsInfo.AddGraphics(null))) {
					sectionMinWidth = (int)Math.Round(cache.Graphics.DpiX * 1.65);
					sectionWidth = Math.Max(sectionWidth, sectionMinWidth);
					AppearanceObject appearance = appearances.ItemAppearance;
					StyleObjectPainter painter = painters.DragItemPainter;
					StyleObjectInfoArgs args = new StyleObjectInfoArgs(cache, new Rectangle(Point.Empty, MeasureString("X", appearance, cache)), appearance);
					dragItemHeight = painter.CalcBoundsByClientRectangle(args).Height;
					args.Bounds = new Rectangle(0, 0, sectionWidth, dragItemHeight);
					dragItemBitmapWidth = painter.GetObjectClientRectangle(args).Width;
				}
			}
			finally {
				graphicsInfo.ReleaseGraphics();
			}
		}
		public void UpdateSectionWidth(int width) {
			sectionWidth = Math.Max(sectionMinWidth, width - Painters.AreaPainter.HorizontalMargins);
		}
		public Bitmap GetDataItemBitmap(IDataSourceSchema dataSourceSchema, IDragObject dragObject, int width, int height) {
			bool isHierarchy = dragObject.DataItemsCount > 1;
			int actualHeight = height != 0 ? height : dragItemHeight;
			int actualWidth = width != 0 ? width : dragItemBitmapWidth;
			if(isHierarchy) {
				actualWidth += 4;
				actualHeight += 4;
			}
			Bitmap bitmap = new Bitmap(actualWidth, actualHeight);
			using(Graphics gr = Graphics.FromImage(bitmap)) {
				GraphicsInfo graphicsInfo = GraphicsInfo.Default;
				try {
					using(GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(graphicsInfo.AddGraphics(gr)), new XPaint())) {
						DragItem dragItem = new DragItem(dataSourceSchema, null, dragObject.Caption);
						dragItem.Arrange(this, cache, new Rectangle(Point.Empty, new Size(actualWidth, actualHeight)), true, isHierarchy);
						dragItem.Paint(this, cache, true, isHierarchy);
					}
					return bitmap;
				}
				catch {
					bitmap.Dispose();
					throw;
				}
				finally {
					graphicsInfo.ReleaseGraphics();
				}
			}
		}
		public Bitmap GetDragGroupBitmap(DragGroup group) {
			Bitmap bitmap = new Bitmap(group.Bounds.Width, group.Bounds.Height);
			using(Graphics gr = Graphics.FromImage(bitmap)) {
				GraphicsInfo graphicsInfo = GraphicsInfo.Default;
				try {
					using(GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(graphicsInfo.AddGraphics(gr)), new XPaint())) {
						group.ResetState();
						group.Arrange(this, cache, Point.Empty);
						group.Paint(this, cache);
					}
					return bitmap;
				}
				catch {
					bitmap.Dispose();
					throw;
				}
				finally {
					graphicsInfo.ReleaseGraphics();
					group.Select();
					group.Section.Area.Arrange();
				}
			}
		}
	}
}
