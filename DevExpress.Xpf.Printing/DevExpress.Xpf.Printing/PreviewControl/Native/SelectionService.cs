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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using DevExpress.DocumentView;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Enumerators;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Printing.Native;
namespace DevExpress.Xpf.Printing.PreviewControl.Native {
	public class SelectionService {
		[DefaultValue(1f)]
		public float Zoom { get; set; }
		const int LengthsSideInsensitiveZone = 10;
		Point selectionRectangleStartPosition = Point.Empty;
		Point selectionRectangleCurrentPosition = Point.Empty;
		Color selectionColorForBricks = Color.FromArgb(180, SystemColors.Highlight);
		bool leftButtonDown = false;
		bool mouseMove = false;
		bool canSelect = false;
		bool multiSelect = false;
		List<Pair<Page, RectangleF>> selectedPages = new List<Pair<Page, RectangleF>>();
		List<Tuple<Brick, RectangleF>> selectedBricks = new List<Tuple<Brick, RectangleF>>();
		List<Tuple<Brick, RectangleF>> previousSelectedBricks = new List<Tuple<Brick, RectangleF>>();
		public Color SelectionColor { 
			get { return selectionColorForBricks; }
			set { selectionColorForBricks = value; }
		}
		bool IsSelecting { get { return leftButtonDown && mouseMove; } }
		internal bool CanSelect {
			get {
				if(!canSelect && leftButtonDown && (Math.Abs(selectionRectangleCurrentPosition.X - selectionRectangleStartPosition.X) > LengthsSideInsensitiveZone 
					|| Math.Abs(selectionRectangleCurrentPosition.Y - selectionRectangleStartPosition.Y) > LengthsSideInsensitiveZone)) {
					canSelect = true;
				}
				return canSelect;
			}
			set { canSelect = value; }
		}
		public bool HasSelection { get { return selectedBricks.Count > 0; } }
		Rectangle SelectionScreenRectangle {
			get {
				return Rectangle.Round(RectF.FromPoints(selectionRectangleStartPosition, selectionRectangleCurrentPosition));
			}
		}
		public SelectionService(IPagesPresenter presenter) {
			this.presenter = presenter;
		}
		internal void CorrectStartPoint(double delta) {
			foreach(Pair<Page, RectangleF> page in selectedPages) {
				page.Second = new RectangleF(new PointF(page.Second.X, page.Second.Y + (float)delta), page.Second.Size);
			}
		}
		internal void SetStartPoint(Point point) {
			this.selectionRectangleStartPosition = point;
		}
		public void OnKillFocus() {
			ResetSelectedBricks();
			ResetService();
			RaiseInvalidate(RectangleF.Empty);
		}
		public bool OnMouseDown(Point cursorLocation, MouseButtons mouseButtons, Keys keys) {
			if(!keys.HasFlag(Keys.Shift) && !keys.HasFlag(Keys.Control)) {
				ResetSelectedBricks();
				multiSelect = false;
			} else
				multiSelect = true;
			leftButtonDown = true;
			selectionRectangleStartPosition = selectionRectangleCurrentPosition  = new Point((int)(cursorLocation.X * ScreenHelper.ScaleX), (int)(cursorLocation.Y * (float)ScreenHelper.ScaleX));
			#region win
			#endregion
			return false;
		}
		public bool OnMouseUp(Point cursorLocation, MouseButtons mouseButtons, Keys keys) {
			previousSelectedBricks.Clear();
			previousSelectedBricks.AddRange(selectedBricks);
			ResetService();
			return false;
		}
		public bool OnMouseMove(Point cursorLocation, MouseButtons mouseButtons, Keys keys, bool isMouseOutsidePresenter) {
			this.selectionRectangleCurrentPosition = new Point((int)(cursorLocation.X * ScreenHelper.ScaleX), (int)(cursorLocation.Y * (float)ScreenHelper.ScaleX));
			if(leftButtonDown && CanSelect) {
				mouseMove = true;
				UpdateSelectedBricks();
				#region win
				#endregion
				return true;
			}
			return false;
		}
		void UpdateSelectedBricks() {
			IEnumerable<Pair<Page, RectangleF>> pages = FindPages(SelectionScreenRectangle);
			if(pages.IsEmpty())
				return;
			pages.ForEach(x => {
				var containedPage = this.selectedPages.FirstOrDefault(page => page.First.Index == x.First.Index);
				if(containedPage != null) {
					this.selectedPages.Remove(containedPage);
				}
				this.selectedPages.Add(x);
			});
			List<Tuple<Brick, RectangleF>> oldSelectedBricks = selectedBricks;
			List<Tuple<Brick, RectangleF>> newSelectedBricks = FindBricks(this.selectedPages);
			if(!newSelectedBricks.IsEmpty()) {
			}
			if(multiSelect) {
				var bricksToAdd = previousSelectedBricks.Where(c => !newSelectedBricks.Any<Tuple<Brick, RectangleF>>(any => { return ReferenceEquals(any.Item1, c.Item1); })).ToList();
				var bricksToDelete = newSelectedBricks.Where(c => previousSelectedBricks.Any<Tuple<Brick, RectangleF>>(any => { return ReferenceEquals(any.Item1, c.Item1); })).ToList();
				bricksToDelete.AddRange(oldSelectedBricks.Where(c => !newSelectedBricks.Any<Tuple<Brick, RectangleF>>(any => { return ReferenceEquals(any.Item1, c.Item1); }) && !previousSelectedBricks.Any<Tuple<Brick, RectangleF>>(any => { return ReferenceEquals(any.Item1, c.Item1); })));
				foreach(var item in bricksToDelete)
					newSelectedBricks.Remove(item);
				newSelectedBricks.AddRange(bricksToAdd);
				ProcessInvalidate(newSelectedBricks, (x) => true);
				ProcessInvalidate(bricksToDelete, (x) => true);
			} else {
				ProcessInvalidate(oldSelectedBricks, (brick) => !newSelectedBricks.Any<Tuple<Brick, RectangleF>>(any => { return ReferenceEquals(any.Item1, brick.Item1); }));
				ProcessInvalidate(newSelectedBricks, (brick) => !oldSelectedBricks.Any<Tuple<Brick, RectangleF>>(any => { return ReferenceEquals(any.Item1, brick.Item1); }) || 
																!previousSelectedBricks.Any<Tuple<Brick, RectangleF>>(any => { return ReferenceEquals(any.Item1, brick.Item1); }));
			}
			selectedBricks = newSelectedBricks;
			RaiseInvalidate(new RectangleF());
		}
		void ProcessInvalidate(List<Tuple<Brick, RectangleF>> bricks, Func<Tuple<Brick, RectangleF>, bool> predicate) {
			#region win
			#endregion
		}
		void RaiseInvalidate(RectangleF rectangleF) {
			if(InvalidatePage != null)
				InvalidatePage(this, EventArgs.Empty);
		}
		public IEnumerable<Pair<Page, RectangleF>> FindPages(Rectangle rect) {
			#region wpf
			var pages = new List<Pair<Page, RectangleF>>();
			foreach(var pageRect in presenter.GetPages()) {
				if(pageRect.Second.IntersectsWith(rect)) {
					var pageREct = CorrectWithDPI(pageRect.Second);
					pages.Add(new Pair<Page, RectangleF>(pageRect.First, pageREct));
				}
			}
			#endregion
			return pages;
		}
		RectangleF CorrectWithDPI(RectangleF rect) {
			return new RectangleF(
				(float)(rect.X * ScreenHelper.ScaleX),
				(float)(rect.Y * ScreenHelper.ScaleX),
				(float)(rect.Width * ScreenHelper.ScaleX),
				(float)(rect.Height * ScreenHelper.ScaleX)
				);
		}
		List<Tuple<Brick, RectangleF>> FindBricks(IEnumerable<Pair<Page, RectangleF>> pages) {
			List<Tuple<Brick, RectangleF>> bricks = new List<Tuple<Brick, RectangleF>>();
			if(pages.IsEmpty())
				return bricks;
			foreach(var page in pages) {
				RectangleF pageRect = PSUnitConverter.PixelToDoc(page.Second, Zoom);
				if(pageRect.IsEmpty)
					return bricks;
				Rectangle clientRect = SelectionScreenRectangle;
				RectangleF rect = PSUnitConverter.PixelToDoc(clientRect, (float)Zoom);
				rect.X -= pageRect.X;
				rect.Y -= pageRect.Y;
				List<Brick> brickList = new List<Brick>();
				using(Region selectionRegion = new Region(rect)) {
					Func<BrickBase, PointF, IIndexedEnumerator> method = (item, itemPosition) => {
						if(item.InnerBrickList.Count > 5 * BrickMapConst.Graduation && item is CompositeBrick) {
							((CompositeBrick)item).ForceBrickMap();
							PointF viewOrigin = new PointF(itemPosition.X + item.InnerBrickListOffset.X, itemPosition.Y + item.InnerBrickListOffset.Y);
							return new ReversedMappedEnumerator(((CompositeBrick)item).BrickMap, item.InnerBrickList) { ClipBounds = rect, ViewOrigin = viewOrigin };
						}
						return new ReversedEnumerator(item.InnerBrickList);
					};
					new BrickNavigator(page.First, method) { BrickPosition = PointF.Empty, ClipRect = page.First.DeflateMinMargins(page.First.GetRect(PointF.Empty)) }.IterateBricks((brick, brickRect, clipRect) => {
						if(RegionIntersectsWithRectangle(selectionRegion, brickRect) && RegionIntersectsWithRectangle(selectionRegion, clipRect)) {
							selectionRegion.Exclude(RectangleF.Intersect(brickRect, clipRect));
							bricks.Add(Tuple.Create<Brick, RectangleF>(brick, RectangleF.Intersect(brickRect, clipRect)));
							bricks.ForEach<Tuple<Brick, RectangleF>>(any => {
								if(brick.InnerBrickList.Contains(any.Item1))
									brickList.Add(any.Item1);
							});
						}
						return false;
					});
				}
				bricks.RemoveAll(item => { return brickList.Contains(item.Item1); });
			}
			return bricks;
		}
		bool RegionIntersectsWithRectangle(Region sourceRegion, RectangleF rectangle) {
			using(Region region = sourceRegion.Clone()) {
				region.Intersect(rectangle);
				using(System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, 0, 0)) {
					RectangleF[] rects = region.GetRegionScans(matrix);
					foreach(RectangleF item in rects)
						if(item.Height > GraphicsUnitConverter.PixelToDoc(1f) && item.Width > GraphicsUnitConverter.PixelToDoc(1f))
							return true;
				}
				return false;
			}
		}
		internal void ResetSelectedBricks() {
			selectedPages = new List<Pair<Page, RectangleF>>();
			var needValidate = !selectedBricks.IsEmpty();
			selectedBricks.Clear();
			previousSelectedBricks.Clear();
			if(needValidate) RaiseInvalidate(new RectangleF());
		}
		void ResetService() {
			selectionRectangleStartPosition = Point.Empty;
			mouseMove = false;
			multiSelect = false;
			CanSelect = false;
			leftButtonDown = false;
		}
		internal void OnDrawPage(Page page, Graphics gr, PointF position) {
			if(selectedPages == null)
				return;
			if(selectedPages.FirstOrDefault(x=> x.First == page) == null)
				return;
			using(SolidBrush brush = new SolidBrush(selectionColorForBricks)) {
				Func<BrickBase, PointF, IIndexedEnumerator> method = (item, itemPosition) => {
					if(item.InnerBrickList.Count > 5 * BrickMapConst.Graduation && item is CompositeBrick) {
						((CompositeBrick)item).ForceBrickMap();
						PointF viewOrigin = new PointF(itemPosition.X + item.InnerBrickListOffset.X, itemPosition.Y + item.InnerBrickListOffset.Y);
						return new MappedIndexedEnumerator(((CompositeBrick)item).BrickMap, item.InnerBrickList) { ClipBounds = gr.ClipBounds, ViewOrigin = viewOrigin };
					}
					return new IndexedEnumerator(item.InnerBrickList);
				};
				List<Brick> bricks = selectedBricks.Select<Tuple<Brick, RectangleF>, Brick>(item => { return item.Item1; }).ToList<Brick>();
				new BrickNavigator(page, method) { BrickPosition = position, ClipRect = page.DeflateMinMargins(page.GetRect(position)) }.IterateBricks((brick, brickRect, brickClipRect) => {
					if(bricks.Remove(brick))
						gr.FillRectangle(brush, RectangleF.Intersect(brickRect, brickClipRect));
					return bricks.Count == 0;
				});
			}
		}
		internal void CopyToClipboard() {
			Clipboard.Clear();
			if(!selectedBricks.IsEmpty())
				using(PrintingSystemBase ps = new PrintingSystemBase()) {
					ps.Graph.PageUnit = GraphicsUnit.Document;
					using(DevExpress.XtraPrinting.LinkBase link = new DevExpress.XtraPrinting.LinkBase(ps)) {
						float left = selectedBricks.Min<Tuple<Brick, RectangleF>>(item => { return item.Item1.Rect.Left; });;
						float top = selectedBricks.Min<Tuple<Brick, RectangleF>>(item => { return item.Item1.Rect.Top; });
						float right = 0;
						float bottom = 0;
						foreach(var page in selectedPages.Select(x => x.First)) {
							var innerBricks = new List<Tuple<Brick, RectangleF>>();
							var enumerator = page.GetEnumerator();
							NestedBrickIterator iterator = new NestedBrickIterator(new[] { page });
							while(iterator.MoveNext()) {
								var selectedBrick = selectedBricks.FirstOrDefault(x => x.Item1 == iterator.CurrentBrick);
								if(selectedBrick != null)
									innerBricks.Add(selectedBrick);
							}
							if(innerBricks.Count == 0)
								continue;
							right = Math.Max(right, innerBricks.Max<Tuple<Brick, RectangleF>>(item => { return item.Item1.Rect.Right; }));
							bottom += innerBricks.Max<Tuple<Brick, RectangleF>>(item => { return item.Item1.Rect.Bottom; });
						}
						link.PaperKind = System.Drawing.Printing.PaperKind.Custom;
						link.CustomPaperSize = Size.Ceiling(GraphicsUnitConverter.Convert(new SizeF(right - left, bottom - top), GraphicsDpi.Document, GraphicsDpi.HundredthsOfAnInch));
						link.MinMargins = new Margins(0, 0, 0, 0);
						link.Margins = new Margins(0, 0, 0, 0);
						CreateDocument(link);
						DataObject dataObj = new DataObject();
						using(MemoryStream stream = new MemoryStream()) {
							ps.ExportOptions.Rtf.ExportMode = RtfExportMode.SingleFile;
							ps.ExportToRtf(stream);
							string s = Encoding.Default.GetString(stream.ToArray());
							dataObj.SetData(DataFormats.Rtf, s);
						}
						using(MemoryStream stream = new MemoryStream()) {
							ps.ExportOptions.Text.TextExportMode = TextExportMode.Text;
							ps.ExportOptions.Text.Encoding = Encoding.Default;
							ps.ExportToText(stream);
							string s = Encoding.Default.GetString(stream.ToArray());
							dataObj.SetData(DataFormats.Text, s);
						}
						using(MemoryStream stream = new MemoryStream()) {
							ps.ExportOptions.Image.ExportMode = ImageExportMode.SingleFile;
							ps.ExportToImage(stream, ImageFormat.Bmp);
							Bitmap image = new Bitmap(stream);
							dataObj.SetData(DataFormats.Bitmap, image);
						}
						Clipboard.SetDataObject(dataObj);
					}
				}
		}
		void CreateDocument(DevExpress.XtraPrinting.LinkBase link) {
			link.CreateDetailArea += (s, args) => {
				float maxBottom = 0;
				foreach(var page in selectedPages.Select(x => x.First)) {
					var innerBricks = new List<Tuple<Brick, RectangleF>>();
					var enumerator = page.GetEnumerator();
					NestedBrickIterator iterator = new NestedBrickIterator(new[] { page });
					while(iterator.MoveNext()) {
						var selectedBrick = selectedBricks.FirstOrDefault(x => x.Item1 == iterator.CurrentBrick);
						if(selectedBrick != null)
							innerBricks.Add(selectedBrick);
					}
					float topOffset = innerBricks.Min<Tuple<Brick, RectangleF>>(item => { return item.Item2.Y; });
					float leftOffset = innerBricks.Min<Tuple<Brick, RectangleF>>(item => { return item.Item2.X; });
					foreach(var item in innerBricks.Reverse<Tuple<Brick, RectangleF>>()) {
						BrickContainerBase brickContainer = new BrickWrapper(item.Item1);
						args.Graph.DrawBrick(brickContainer, RectF.Offset(item.Item2, -leftOffset, -topOffset + maxBottom));
					}
					maxBottom = innerBricks.Max<Tuple<Brick, RectangleF>>(item => { return item.Item2.Bottom; });
				}
			};
			link.CreateDocument();
		}
		internal PrintingSystemBase GetFakedPSWithSelection() {
			if(selectedBricks.IsEmpty())
				return null;
			PrintingSystemBase ps = new PrintingSystemBase();
			ps.Graph.PageUnit = GraphicsUnit.Document;
			var page = selectedPages.FirstOrDefault().First;
			using(DevExpress.XtraPrinting.LinkBase link = new DevExpress.XtraPrinting.LinkBase(ps)) {
				link.PaperKind = page.PageData.PaperKind;
				if(link.PaperKind == System.Drawing.Printing.PaperKind.Custom)
					link.CustomPaperSize = GraphicsUnitConverter.Convert(page.PageData.PageSize.ToSize(), GraphicsDpi.Document, GraphicsDpi.HundredthsOfAnInch);
				link.MinMargins = new Margins(0, 0, 0, 0);
				link.Margins = page.PageData.Margins;
				link.Landscape = page.PageData.Landscape;
				link.PaperName = page.PageData.PaperName;
				CreateDocument(link);
			}
			return ps;
		}
		#region wpf only
		public event EventHandler InvalidatePage;
		readonly IPagesPresenter presenter;
		internal void SelectBrick(Page page, Brick brick) {
			selectedPages.Clear();
			selectedPages.Add(new Pair<Page, RectangleF>(page, RectangleF.Empty));
			selectedBricks.Clear();
			selectedBricks.Add(new Tuple<Brick, RectangleF>(brick, brick.Rect));
		}
		#endregion
	}
}
