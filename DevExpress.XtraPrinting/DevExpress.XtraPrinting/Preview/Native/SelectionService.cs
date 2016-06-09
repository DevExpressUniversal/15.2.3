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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.DocumentView;
using DevExpress.DocumentView.Controls;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Enumerators;
using DevExpress.XtraPrinting.Native.Navigation;
namespace DevExpress.XtraPrinting.Preview.Native {
	public class SelectionMessageHandler : WindowTargetBase {
		DevExpress.XtraPrinting.Preview.Native.SelectionService service = null;
		const int CodeEsc = 0x1B;
		const int WM_KEYDOWN = 0x0100;
		public bool CanHandle { get; set; }
		public SelectionMessageHandler(System.Windows.Forms.Control control, DevExpress.XtraPrinting.Preview.Native.SelectionService service)
			: base(control) {
			this.service = service;
			CanHandle = true;
		}
		protected override void HandleMessage(ref Message m, ref bool handled) {
			if(!CanHandle) return;
			service.currentMousePosition = System.Windows.Forms.Control.MousePosition;
			if(m.Msg == Win32.WM_MOUSEMOVE)
				handled = service.HandleMouseMove();
			else if(m.Msg == Win32.WM_LBUTTONDOWN)
				handled = service.HandleLeftMouseDown();
			else if(m.Msg == Win32.WM_LBUTTONUP)
				handled = service.HandleLeftMouseUp();
			else if(m.Msg == Win32.WM_MOUSEWHEEL)
				handled = service.HandleMouseWheel();
			else if(m.Msg == WM_KEYDOWN && m.WParam.ToInt32() == CodeEsc)
				handled = service.HandleKillFocus();
		}
	}
	public class SelectionService {
		#region inner classes
		class PageItem : IPageItem {
			public int Index { get; set; }
			public int OriginalIndex { get; set; }
			public int OriginalPageCount { get; set; }
			public int PageCount { get; set; }
		}
		class PageInfoExporter : TextBrickExporter {
			public IPageItem PageItem { get; set; }
			public override void Draw(IGraphics gr, RectangleF rect, RectangleF parentRect) {
				Text = (Brick as PageInfoTextBrickBase).GetTextInfo(gr.PrintingSystem, PageItem);
				base.Draw(gr, rect, parentRect);
			}
		}
		#endregion
		Point startMousePosition = Point.Empty;
		internal Point currentMousePosition = Point.Empty;
		bool leftButtonDown = false;
		bool mouseMove = false;
		bool canSelect = false;
		bool multiSelect = false;
		Page page;
		List<Tuple<Brick, RectangleF>> selectedBricks = new List<Tuple<Brick, RectangleF>>();
		List<Tuple<Brick, RectangleF>> previousSelectedBricks = new List<Tuple<Brick, RectangleF>>();
		RectangleF selectionClientRectangle = RectangleF.Empty;
		readonly Color selectionColorForBricks = Color.FromArgb(180, SystemColors.Highlight);
		readonly Color selectionColor = Color.FromArgb(100, SystemColors.Highlight);
		const int LengthsSideInsensitiveZone = 10;
		ViewControl control;
		ViewControl ViewControl { get { return control; } }
		PrintControl PrintControl { get { return (PrintControl)ViewControl.Parent; } }
		bool IsSelecting { get { return leftButtonDown && mouseMove; } }
		bool CanSelect {
			get {
				if(!canSelect && (Math.Abs(currentMousePosition.X - startMousePosition.X) > LengthsSideInsensitiveZone || Math.Abs(currentMousePosition.Y - startMousePosition.Y) > LengthsSideInsensitiveZone)) {
					canSelect = true;
				}
				return canSelect;
			}
			set { canSelect = value; }
		}
		Rectangle SelectionScreenRectangle {
			get {
				return Rectangle.Round(RectF.FromPoints(startMousePosition, currentMousePosition));
			}
		}
		public bool HasSelection {
			get {
				return selectedBricks.Count > 0;
			}
		}
		internal SelectionService(ViewControl control) {
			this.control = control;
			control.Paint += control_Paint;
		}
		internal void OnSearchStarted() {
			multiSelect = false;
			page = null;
		}
		public void SelectBrick(Page page, Brick brick) {
			this.page = page;
			selectedBricks.Add(new Tuple<Brick, RectangleF>(brick, brick.Rect));
			UpdateCommands();
			PrintControl.ShowBrickCenter(brick, page);
		}
		public void InvalidateControl() {
			if(PrintControl != null)
				PrintControl.Invalidate(true);
		}
		internal bool HandleLeftMouseDown() {
			if(ViewControl.Cursor == Cursors.Default) {
				InvalidateControl();
				if(!System.Windows.Forms.Control.ModifierKeys.HasFlag(Keys.Shift) && !System.Windows.Forms.Control.ModifierKeys.HasFlag(Keys.Control)) {
					ResetSelectedBricks();
					multiSelect = false;
					page = null;
				} else 
					multiSelect = true;
				leftButtonDown = true;
				startMousePosition = currentMousePosition;
				UpdateCommands();
			}
			return false;
		}
		internal bool HandleMouseMove() {
			if(leftButtonDown && CanSelect) {
				mouseMove = true;
				UpdateSelectedBricks();
				UpdateCommands();
				return true;
			}
			return false;
		}
		internal bool HandleLeftMouseUp() {
			previousSelectedBricks.Clear();
			previousSelectedBricks.AddRange(selectedBricks);
			ResetService();
			return false;
		}
		internal bool HandleMouseWheel() {
			return IsSelecting;
		}
		internal bool HandleKillFocus() {
			ResetService();
			ResetSelectedBricks();
			UpdateCommands();
			return false;
		}
		void UpdateCommands() {
			PrintControl.UpdateCommands(commandSet => {
				commandSet.EnableCommand(selectedBricks.Count > 0, PrintingSystemCommand.Copy);
				commandSet.EnableCommand(selectedBricks.Count > 0, PrintingSystemCommand.PrintSelection);
			});
		}
		void control_Paint(object sender, PaintEventArgs e) {
			if(selectionClientRectangle.IsEmpty)
				return;
			Graphics graph = e.Graphics;
			graph.ExecuteAndKeepState(() => {
				graph.ResetTransform();
				graph.PageUnit = GraphicsUnit.Display;
				using(SolidBrush brush = new SolidBrush(selectionColor)) {
					graph.FillRectangle(brush, selectionClientRectangle);
				}
			});
		}
		void UpdateSelectedBricks() {
			Page page = (Page)FindPage(SelectionScreenRectangle);
			if(this.page == null && page == null)
				return;
			if(this.page == null && page != null)
				this.page = page;
			if(this.page != null && !ReferenceEquals(this.page, page))
				page = this.page;
			List<Tuple<Brick, RectangleF>> oldSelectedBricks = selectedBricks;
			List<Tuple<Brick, RectangleF>> newSelectedBricks = FindBricks(page);
			if(multiSelect) {
				var bricksToAdd = previousSelectedBricks.Where(c => !newSelectedBricks.Any<Tuple<Brick, RectangleF>>(any => { return ReferenceEquals(any.Item1, c.Item1); })).ToList();
				var bricksToDelete = newSelectedBricks.Where(c => previousSelectedBricks.Any<Tuple<Brick, RectangleF>>(any => { return ReferenceEquals(any.Item1, c.Item1); })).ToList();
				bricksToDelete.AddRange(oldSelectedBricks.Where(c => !newSelectedBricks.Any<Tuple<Brick, RectangleF>>(any => { return ReferenceEquals(any.Item1, c.Item1); }) && !previousSelectedBricks.Any<Tuple<Brick, RectangleF>>(any => { return ReferenceEquals(any.Item1, c.Item1); })));
				foreach(var item in bricksToDelete) newSelectedBricks.Remove(item);
				newSelectedBricks.AddRange(bricksToAdd);
				foreach(var item in newSelectedBricks)
					InvalidatePage(item.Item2);
				foreach(var item in bricksToDelete)
					InvalidatePage(RectHelper.InflateRect(item.Item2, 1, BorderSide.All));
			} else {
				foreach(var item in oldSelectedBricks)
					if(!newSelectedBricks.Any<Tuple<Brick, RectangleF>>(any => { return ReferenceEquals(any.Item1, item.Item1); }))
						InvalidatePage(RectHelper.InflateRect(item.Item2, 1, BorderSide.All));
				foreach(var item in newSelectedBricks)
					if(!oldSelectedBricks.Any<Tuple<Brick, RectangleF>>(any => { return ReferenceEquals(any.Item1, item.Item1); })
						|| !previousSelectedBricks.Any<Tuple<Brick, RectangleF>>(any => { return ReferenceEquals(any.Item1, item.Item1); }))
						InvalidatePage(item.Item2);
			}
			ViewControl.InvalidateRect(RectHelper.InflateRect(selectionClientRectangle, 1, BorderSide.All), false);
			selectionClientRectangle = ViewControl.RectangleToClient(SelectionScreenRectangle);
			ViewControl.InvalidateRect(RectHelper.InflateRect(selectionClientRectangle, 1, BorderSide.All), false);
			selectedBricks = newSelectedBricks;
		}
		internal void OnDrawPage(Page page, Graphics gr, PointF position) {
			if(!ReferenceEquals(this.page, page)) return;
			using(SolidBrush brush = new SolidBrush(selectionColorForBricks)) {
				DrawBricks(page, brush, gr, position, selectedBricks.Select<Tuple<Brick, RectangleF>, Brick>(item => { return item.Item1; }).ToList<Brick>());
			}
		}
		void DrawBricks(Page page, SolidBrush brush, Graphics gr, PointF position, List<Brick> bricks) {
			Func<BrickBase, PointF, IIndexedEnumerator> method = (item, itemPosition) => {
				if(item.InnerBrickList.Count > 5 * BrickMapConst.Graduation && item is CompositeBrick) {
					((CompositeBrick)item).ForceBrickMap();
					PointF viewOrigin = new PointF(itemPosition.X + item.InnerBrickListOffset.X, itemPosition.Y + item.InnerBrickListOffset.Y);
					return new MappedIndexedEnumerator(((CompositeBrick)item).BrickMap, item.InnerBrickList) { ClipBounds = gr.ClipBounds, ViewOrigin = viewOrigin };
				}
				return new IndexedEnumerator(item.InnerBrickList);
			};
			new BrickNavigator(page, method) { BrickPosition = position, ClipRect = page.DeflateMinMargins(page.GetRect(position)) }.IterateBricks((brick, brickRect, brickClipRect) => {
				if(bricks.Remove(brick))
					gr.FillRectangle(brush, RectangleF.Intersect(brickRect, brickClipRect));
				return bricks.Count == 0;
			});
		}
		internal void CopyToClipboard() {
			Clipboard.Clear();
			if(selectedBricks.IsEmpty()) return;
			using(PrintingSystemBase ps = new PrintingSystemBase()) {
				ps.Graph.PageUnit = GraphicsUnit.Document;
				using(LinkBase link = new LinkBase(ps)) {
					float left = selectedBricks.Min<Tuple<Brick, RectangleF>>(item => { return item.Item2.Left; });
					float top = selectedBricks.Min<Tuple<Brick, RectangleF>>(item => { return item.Item2.Top; });
					float right = selectedBricks.Max<Tuple<Brick, RectangleF>>(item => { return item.Item2.Right; });
					float bottom = selectedBricks.Max<Tuple<Brick, RectangleF>>(item => { return item.Item2.Bottom; });
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
					PageItem pageItem = page != null ?
						new PageItem { Index = page.Index, OriginalIndex = page.OriginalIndex, OriginalPageCount = page.OriginalPageCount, PageCount = ((IPageItem)page).PageCount } :
						new PageItem { Index = 0, OriginalIndex = 0, OriginalPageCount = 1, PageCount = 1 };
					ps.ExportersFactory.AssignExporter(typeof(PageInfoTextBrick), new PageInfoExporter() { PageItem = pageItem });
					try {
						using(MemoryStream stream = new MemoryStream()) {
							ps.ExportOptions.Image.ExportMode = ImageExportMode.SingleFile;
							ps.ExportToImage(stream, ImageFormat.Bmp);
							Bitmap image = new Bitmap(stream);
							dataObj.SetData(DataFormats.Bitmap, image);
						}
					} finally {
						ps.ExportersFactory.AssignExporter(typeof(PageInfoTextBrick), null);
					}
					Clipboard.SetDataObject(dataObj);
				}
			}
		}
		internal PrintingSystemBase GetFakedPSWithSelection() {
			if(selectedBricks.IsEmpty()) return null;
			PrintingSystemBase ps = new PrintingSystemBase();
			ps.Graph.PageUnit = GraphicsUnit.Document;
			using(LinkBase link = new LinkBase(ps)) {
				link.PaperKind = page.PageData.PaperKind;
				if(link.PaperKind == PaperKind.Custom)
					link.CustomPaperSize = GraphicsUnitConverter.Convert(page.PageData.PageSize.ToSize(), GraphicsDpi.Document, GraphicsDpi.HundredthsOfAnInch);
				link.MinMargins = new Margins(0, 0, 0, 0);
				link.Margins = page.PageData.Margins;
				link.Landscape = page.PageData.Landscape;
				link.PaperName = page.PageData.PaperName;
				CreateDocument(link);
			}
			return ps;
		}
		void CreateDocument(LinkBase link) {
			float topOffset = selectedBricks.Min<Tuple<Brick, RectangleF>>(item => { return item.Item2.Y; });
			float leftOffset = selectedBricks.Min<Tuple<Brick, RectangleF>>(item => { return item.Item2.X; });
			link.CreateDetailArea += (s, args) => {
				foreach(var item in selectedBricks.Reverse<Tuple<Brick, RectangleF>>()) {
					BrickContainerBase brickContainer = new BrickWrapper(item.Item1);
					args.Graph.DrawBrick(brickContainer, RectF.Offset(item.Item2, -leftOffset, -topOffset));
				}
			};
			link.CreateDocument();
		}
		List<Tuple<Brick, RectangleF>> FindBricks(Page page) {
			List<Tuple<Brick, RectangleF>> bricks = new List<Tuple<Brick, RectangleF>>();
			if(page == null)
				return bricks;
			RectangleF pageRect = PrintControl.ViewManager.GetPageRect(page);
			if(pageRect.IsEmpty)
				return bricks;
			Rectangle clientRect = ViewControl.RectangleToClient(SelectionScreenRectangle);
			RectangleF rect = PSUnitConverter.PixelToDoc(clientRect, PrintControl.Zoom, PrintControl.ViewManager.ScrollPos);
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
				new BrickNavigator(page, method) { BrickPosition = PointF.Empty, ClipRect = page.DeflateMinMargins(page.GetRect(PointF.Empty)) }.IterateBricks((brick, brickRect, clipRect) => {
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
		void InvalidatePage(RectangleF rect) {
			if(!rect.IsEmpty) {
				rect = RectHelper.InflateRect(rect, 3, BorderSide.All);
				var rec = (PrintControl.ViewManager.GetPageRect(page));
				rect.Offset(rec.Location);
				rect = PSUnitConverter.DocToPixel(rect, PrintControl.Zoom, PrintControl.ViewManager.ScrollPos);
				ViewControl.InvalidateRect(rect, false);
			}
		}
		public IPage FindPage(Rectangle rect) {
			RectangleF rectDoc = PSUnitConverter.PixelToDoc(ViewControl.RectangleToClient(rect), PrintControl.Zoom, PrintControl.ViewManager.ScrollPos);
			PageEnumerator enumerator = PrintControl.ViewManager.CreatePageEnumerator();
			while(enumerator.MoveNext())
				if(enumerator.CurrentPlace.IntersectsWith(rectDoc))
					return enumerator.Current;
			return null;
		}
		internal void ResetSelectedBricks() {
			foreach(var item in selectedBricks)
				InvalidatePage(RectHelper.InflateRect(item.Item2, 1, BorderSide.All));
			selectedBricks.Clear();
			previousSelectedBricks.Clear();
		}
		void ResetService() {
			ViewControl.InvalidateRect(RectHelper.InflateRect(selectionClientRectangle, 1, BorderSide.All), false);
			selectionClientRectangle = RectangleF.Empty;
			startMousePosition = Point.Empty;
			mouseMove = false;
			multiSelect = false;
			CanSelect = false;
			leftButtonDown = false;
		}
	}
}
