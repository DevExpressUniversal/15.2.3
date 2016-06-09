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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Dragging;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraDashboardLayout {
	public class ControlPartPainter {
		[System.Security.SecuritySafeCritical]
		public static Bitmap DrawControlPart(Control control, Rectangle bounds, Color backColor) {
			Bitmap targetBitmap = new Bitmap(bounds.Width, bounds.Height);
			Bitmap sourceBitmap = DevExpress.XtraReports.Native.XRControlPaint.GetControlImage(control, DevExpress.XtraReports.UI.WinControlDrawMethod_Utils.UseWMPrintRecursive, DevExpress.XtraReports.UI.WinControlImageType_Utils.Bitmap) as Bitmap;
			if(sourceBitmap != null) {
				using(Graphics source = Graphics.FromImage(sourceBitmap)) {
					using(Graphics target = Graphics.FromImage(targetBitmap)) {
						target.Clear(backColor);
						target.DrawImage(sourceBitmap, new Rectangle(0, 0, bounds.Width, bounds.Height), bounds, GraphicsUnit.Pixel);
					}
				}
				sourceBitmap.Dispose();
			}
			return targetBitmap;
		}
	}
	public class DraggingBehaviour :BaseBehaviour {
		BaseLayoutItem draggingItem = null;
		Bitmap draggingItemImage = null;
		int draggingImageItemHashCode = 0;
		Brush dragHintSolidBrush {
			get {
				if(owner.Owner != null) {
					Skin skin = CommonSkins.GetSkin(owner.Owner.LookAndFeel);
					if(skin != null) {
						return new SolidBrush(skin.Colors["HighlightAlternate"]); ;
					}
				}
				return Brushes.Plum;
			}
		}
		Brush dragHintTransparentBrush {
			get {
				if(owner.Owner != null) {
					Skin skin = CommonSkins.GetSkin(owner.Owner.LookAndFeel);
					if(skin != null) {
						return new SolidBrush(Color.FromArgb(DashboardLayoutSettings.DragIndicatorAlphaLevel, skin.Colors["HighlightAlternate"]));
					}
				}
				return new SolidBrush(Color.FromArgb(DashboardLayoutSettings.DragIndicatorAlphaLevel, Color.Plum));
			}
		}
		Timer expandTimer;
		TimerDrivenDragController dragController;
		public DraggingBehaviour(AdornerWindowHandler handler)
			: base(handler) {
			expandTimer = new Timer();
			expandTimer.Interval = 2;
			expandTimer.Enabled = false;
			expandTimer.Tick += ExpandTimerCallback;
		}
		public override void Dispose() {
			base.Dispose();
			if(expandTimer != null) {
				expandTimer.Tick -= ExpandTimerCallback;
				if(expandTimer.Enabled)
					expandTimer.Stop();
				expandTimer.Dispose();
				expandTimer = null;
			}
		}
		void ExpandTimerCallback(object sender, EventArgs e) {
			expandTimer.Stop();
			if(draggingItem != null) {
				try {
					if(dragController != null) {
						dragController.ExpandTimerTick();
					}
					owner.Invalidate();
				} finally {
					expandTimer.Interval = DashboardLayoutSettings.DropIndicatorUpdateTimeout;
					expandTimer.Start();
				}
			}
		}
		public override bool ProcessEvent(EventType eventType, object sender, MouseEventArgs e, KeyEventArgs key) {
			if((sender as string) == "HookEvent") {
				Point p = e != null ? e.Location : owner.Owner.PointToClient(Cursor.Position);
				Glyph glyphAtPoint = GetGlyphAtPoint(p) as Glyph;
				if(glyphAtPoint != null) {
					bool? result = null;
					result = DoActionWithGroupIfNeed(eventType, e, result);
					if(result.HasValue) return result.Value;
					DragDropDispatcherFactory.Default.ProcessMouseEvent(owner.Owner.DragDropDispatcherClientHelper.ClientDescriptor, new ProcessEventEventArgs(eventType, e));
					return true;
				} else return false;
			} else {
				bool? result = null;
				result = DoActionWithGroupIfNeed(eventType, e, result);
				if(result.HasValue) return result.Value;
				return base.ProcessEvent(eventType, sender, e, key);
			}
		}
		private bool? DoActionWithGroupIfNeed(EventType eventType, MouseEventArgs e, bool? result) {
			if(e != null && e.Button == MouseButtons.Left && (eventType == EventType.MouseDown || eventType == EventType.MouseMove) && owner.State == AdornerWindowHandlerStates.Normal) {
				if(eventType == EventType.MouseDown) {
					Point p = e != null ? e.Location : owner.Owner.PointToClient(Cursor.Position);
					DraggingBitmapGlyph glyphAtPoint = GetGlyphAtPoint(p) as DraggingBitmapGlyph;
					if(glyphAtPoint != null) {
						DragDropDispatcherFactory.Default.ProcessMouseEvent(owner.Owner.DragDropDispatcherClientHelper.ClientDescriptor, new ProcessEventEventArgs(eventType, e));
						result = true;
					}
				}
				if(eventType == EventType.MouseMove) {
					Point p = e != null ? e.Location : owner.Owner.PointToClient(Cursor.Position);
					DraggingBitmapGlyph glyphAtPoint = GetGlyphAtPoint(p) as DraggingBitmapGlyph;
					if(glyphAtPoint != null) {
						DragDropDispatcherFactory.Default.ProcessMouseEvent(owner.Owner.DragDropDispatcherClientHelper.ClientDescriptor, new ProcessEventEventArgs(eventType, e));
						DragDropDispatcherFactory.Default.DragItem = glyphAtPoint.TargetItem;
						result = true;
					}
				}
			}
			return result;
		}
		public void UpdateForDragging(Point p) {
			owner.HotTrackedItem = owner.Owner.CalcHitInfo(p).Item;
			owner.State = AdornerWindowHandlerStates.Dragging;
			Invalidate();
		}
		public override Glyph GetGlyphAtPoint(Point p) {
			foreach(Glyph g in glyphs) {
				if(g.Bounds.Contains(p)) {
					return g;
				}
				if(g is DragHintGlyph && (g as DragHintGlyph).ExpandType == ExpandType.Single) {
					DragHintGlyph dragHint = g as DragHintGlyph;
					if(dragHint.Triangle.Contains(p))
						return dragHint;
				}
			}
			return null;
		}
		public void DoDragging(Point p) {
			DragHintGlyph currentDraggingGlyph = GetGlyphAtPoint(p) as DragHintGlyph;
			if(currentDraggingGlyph == null) return;
			if(!AllowDraggingGlyph(currentDraggingGlyph)) return;
			bool shouldRecreateDragController;
			if(Compare(currentDraggingGlyph, dragController, out shouldRecreateDragController)) {
				if(shouldRecreateDragController) {
					RecreateDragController(currentDraggingGlyph, true);
				} else {
					if(currentDraggingGlyph != null && currentDraggingGlyph.TargetItem != null) currentDraggingGlyph.TargetItem.BeginInit();
					RecreateDragController(currentDraggingGlyph, false);
					if(currentDraggingGlyph != null && currentDraggingGlyph.TargetItem != null) currentDraggingGlyph.TargetItem.EndInit();
				}
			}
			if(currentDraggingGlyph == null) {
				ResetDragController(null);
				owner.Invalidate();
			}
		}
		private bool AllowDraggingGlyph(DragHintGlyph currentDraggingGlyph) {
			if(!(draggingItem is DashboardLayoutControlGroupBase)) return true;
			switch(currentDraggingGlyph.MoveType) {
				case MoveType.Inside:
					if(currentDraggingGlyph.TargetItem is DashboardLayoutControlGroupBase && (currentDraggingGlyph.TargetItem as DashboardLayoutControlGroupBase).AllowDropGroup) return true;
					break;
				case MoveType.Outside:
					DashboardLayoutControlGroupBase parent = currentDraggingGlyph.TargetItem.Parent as DashboardLayoutControlGroupBase;
					if(parent.AllowDropGroup) return true;
					break;
			}
			return false;
		}
		public LayoutItemDragController GetDragController() { return dragController != null ? dragController.GetDragController(draggingItem) : null; }
		public void OnEndDragging() {
			expandTimer.Stop();
			draggingItem.Visibility = LayoutVisibility.Always;
			if(dragController != null) {
				dragController.Drag(draggingItem);
				dragController.Dispose();
				dragController = null;
			}
			draggingItem = null;
			owner.State = AdornerWindowHandlerStates.Normal;
			ResetDraggingImage();
		}
		private void ResetDraggingImage() {
			draggingItemImage.Dispose();
			draggingItemImage = null;
			draggingImageItemHashCode = 0;
		}
		public Image OnStartDragging(BaseLayoutItem draggingItem) {
			this.draggingItem = draggingItem;
			owner.State = AdornerWindowHandlerStates.Dragging;
			if(draggingItem.GetHashCode() != draggingImageItemHashCode) {
				if(draggingItem.Owner != null)
					draggingItemImage = ControlPartPainter.DrawControlPart(draggingItem.Owner.Control, draggingItem.ViewInfo.BoundsRelativeToControl, CommonSkins.GetSkin(owner.Owner.LookAndFeel).Colors["Window"]);
				else {
					Image image = draggingItem.Tag as Image;
					draggingItemImage = new Bitmap(image == null ? XtraDataLayout.IconsHlper.BindingIcon : image);
				}
				draggingImageItemHashCode = draggingItem.GetHashCode();
			}
			if(draggingItem.Parent != null && !draggingItem.IsHidden) draggingItem.Visibility = LayoutVisibility.Never;
			return draggingItemImage;
		}
		protected bool Compare(DragHintGlyph glyph, TimerDrivenDragController dragController, out bool shouldRecreateDragController) {
			shouldRecreateDragController = true;
			if(glyph == null) return true;
			if(dragController == null) return true;
			if(glyph.InsertType != dragController.InsertType) return true;
			if(glyph.TargetItem != dragController.DragToItem) return true;
			if(glyph.MoveType != dragController.MoveType) return true;
			if(glyph.ExpandType != dragController.ExpandType) {
				if(glyph.ExpandType != ExpandType.Single && dragController.ExpandType != ExpandType.Single) shouldRecreateDragController = false;
				return true;
			}
			return false;
		}
		protected void ResetDragController(DragHintGlyph glyph) {
			if(dragController != null) { dragController.Dispose(glyph); dragController = null; }
		}
		private void RecreateDragController(DragHintGlyph glyph, bool shouldInvalidate) {
			ResetDragController(glyph);
			if(glyph != null) {
				expandTimer.Stop();
				expandTimer.Interval = 2;
				dragController = new TimerDrivenDragController(glyph, draggingItem);
				expandTimer.Start();
			}
			if(shouldInvalidate) {
				owner.Invalidate();
			}
		}
		public void OnCancelDragging() {
			if(draggingItem == null) return;
			draggingItem.Visibility = LayoutVisibility.Always;
			draggingItem = null;
			owner.State = AdornerWindowHandlerStates.Normal;
			ResetDraggingImage();
			if(dragController != null) {
				dragController.Dispose();
				dragController = null;
			}
			Invalidate();
		}
		public override void Invalidate() {
			base.Invalidate();
			if(owner.HotTrackedItem != null && owner.State == AdornerWindowHandlerStates.Normal && owner.HotTrackedItem.ViewInfo != null) {
				if(owner.HotTrackedItem.TextVisible) {
					if(owner.HotTrackedItem.Parent != null && owner.HotTrackedItem.Parent.Selected && !owner.HotTrackedItem.Parent.TextVisible) {
						glyphs.Add(new DraggingBitmapGlyph(owner.Owner, owner.HotTrackedItem.Parent.ViewInfo.BoundsRelativeToControl) { TargetItem = owner.HotTrackedItem.Parent });
					} else {
						glyphs.Add(new DraggingGlyph(owner.Owner) {
							Bounds = new Rectangle(owner.HotTrackedItem.ViewInfo.BoundsRelativeToControl.Location, new Size(owner.HotTrackedItem.Bounds.Width, owner.HotTrackedItem.ViewInfo.Padding.Top)),
							TargetItem = owner.HotTrackedItem
						});
					}
				} else  
					if(owner.HotTrackedItem.Parent != null && owner.HotTrackedItem.Parent.Selected && !owner.HotTrackedItem.Parent.TextVisible)
					glyphs.Add(new DraggingBitmapGlyph(owner.Owner, owner.HotTrackedItem.Parent.ViewInfo.BoundsRelativeToControl) { TargetItem = owner.HotTrackedItem.Parent });
				else
					glyphs.Add(new DraggingBitmapGlyph(owner.Owner, owner.HotTrackedItem.ViewInfo.BoundsRelativeToControl) { TargetItem = owner.HotTrackedItem });
			}
			if(owner.HotTrackedItem != null && owner.State == AdornerWindowHandlerStates.Dragging && owner.HotTrackedItem != draggingItem && owner.HotTrackedItem.Visible) {
				if(owner.HotTrackedItem is DashboardLayoutControlGroupBase && ((DashboardLayoutControlGroupBase)owner.HotTrackedItem).ItemCount == 0) {
					FillDragHintForEmptyGroup();
				}
				FillDragHintGlyphForItem();
			}
		}
		private void FillDragHintForEmptyGroup() {
			if(draggingItem is DashboardLayoutControlGroupBase && !((DashboardLayoutControlGroupBase)owner.HotTrackedItem).AllowDropGroup) return;
			DragHintGlyph insideGlyph = new DragHintGlyph(owner.Owner, InsertType.Top, MoveType.Inside, ExpandType.Center, owner.HotTrackedItem);
			glyphs.Add(insideGlyph);
			if(dragController != null) {
				Rectangle dpgRect = dragController.GetDragPlaceGlyphBounds(null);
				if(dragController.MoveType == MoveType.Inside)
					glyphs.Add(new DropPlaceGlyph(owner.Owner, dragHintSolidBrush, dragHintTransparentBrush, dragController.DragHintRectangle, dragController.DragHints, false, insideGlyph.ExpandType == ExpandType.Single) { Bounds = insideGlyph.Bounds });
			}
		}
		private void FillDragHintGlyphForItem() {
			Array insertTypes = Enum.GetValues(typeof(InsertType));
			foreach(InsertType insertType in insertTypes) {
				DragHintGlyph leftDragHint = new DragHintGlyph(owner.Owner, insertType, MoveType.Outside, ExpandType.Left, owner.HotTrackedItem);
				DragHintGlyph centerDragHint = new DragHintGlyph(owner.Owner, insertType, MoveType.Outside, ExpandType.Center, owner.HotTrackedItem);
				DragHintGlyph rightDragHint = new DragHintGlyph(owner.Owner, insertType, MoveType.Outside, ExpandType.Right, owner.HotTrackedItem);
				DragHintGlyph singleDragHint = new DragHintGlyph(owner.Owner, insertType, MoveType.Outside, ExpandType.Single, owner.HotTrackedItem);
				DragHintGlyph[] insertTypeGlyphs = new DragHintGlyph[] { leftDragHint, centerDragHint, rightDragHint, singleDragHint };
				leftDragHint.SetDragHints(insertTypeGlyphs);
				centerDragHint.SetDragHints(insertTypeGlyphs);
				rightDragHint.SetDragHints(insertTypeGlyphs);
				singleDragHint.SetDragHints(insertTypeGlyphs);
				glyphs.Add(leftDragHint);
				glyphs.Add(rightDragHint);
				glyphs.Add(centerDragHint);
				glyphs.Add(singleDragHint);
			}
			if(dragController != null) {
				Rectangle dpgRect = dragController.GetDragPlaceGlyphBounds(null);
				if(dragController.MoveType == MoveType.Outside) {
					if(dpgRect != Rectangle.Empty) {
						glyphs.Add(new DropPlaceGlyph(owner.Owner, dragHintSolidBrush, dragHintTransparentBrush, dragController.DragHintRectangle, dragController.DragHints, dragController.ShowMultipleExpandHints, dragController.ExpandType == ExpandType.Single) { Bounds = dpgRect });
					}
				}
			}
		}
	}
	public class TimerDrivenDragController :IDisposable {
		static Rectangle TranslateRectangle(Rectangle rect, BaseLayoutItem item) {
			rect.Offset(item.ViewInfo.Offset);
			rect.Offset(new Point(-item.Location.X, -item.Location.Y));
			return rect;
		}
		readonly BaseLayoutItem draggingItemCore;
		BaseLayoutItem dragToItemCore;
		ExpandType expandTypeCore;
		InsertType insertTypeCore;
		InsertLocation insertLocationCore;
		LayoutType layoutTypeCore;
		MoveType moveTypeCore;
		Rectangle dragHintBoundsCore;
		Rectangle[] dragHints;
		bool showMultipleExpandTypes = true;
		public bool ShowMultipleExpandHints { get { return showMultipleExpandTypes; } }
		XtraLayout.Utils.Padding padding;
		Dictionary<BaseLayoutItem, DevExpress.XtraLayout.Utils.Padding> affectedItems = new Dictionary<BaseLayoutItem, DevExpress.XtraLayout.Utils.Padding>();
		int dragControllerState;
		List<BaseLayoutItem> before = new List<BaseLayoutItem>();
		List<BaseLayoutItem> after = new List<BaseLayoutItem>();
		public TimerDrivenDragController(DragHintGlyph glyph, BaseLayoutItem draggingItem) {
			draggingItemCore = draggingItem;
			dragToItemCore = glyph.TargetItem;
			expandTypeCore = glyph.ExpandType;
			insertTypeCore = glyph.InsertType;
			moveTypeCore = glyph.MoveType;
			padding = glyph.TargetItem.ViewInfo.Padding;
			layoutTypeCore = glyph.LayoutType;
			insertLocationCore = glyph.InsertLocation;
			dragHints = glyph.DragHints;
			if(dragHints != null) {
				switch(expandTypeCore) {
					case ExpandType.Left:
						dragHintBoundsCore = dragHints[0];
						break;
					case ExpandType.Center:
						dragHintBoundsCore = dragHints[1];
						break;
					case ExpandType.Right:
						dragHintBoundsCore = dragHints[2];
						break;
				}
			}
			InitializeAffectedItems();
			ShowHintTimerTick();
			showHintTimer = new Timer();
			showHintTimer.Interval = ShowHintTimerInterval;
			showHintTimer.Tick += showHintTimer_Tick;
			showHintTimer.Enabled = true;
		}
		void showHintTimer_Tick(object sender, EventArgs e) {
			showHintTimer.Stop();
			if(expandTypeCore == XtraDashboardLayout.ExpandType.Single) {
				dragControllerState = 1;
				if(draggingItemCore.Owner != null) {
					(draggingItemCore.Owner as DashboardLayoutControl).handler.Invalidate();
					return;
				}
				if(dragToItemCore != null && dragToItemCore.Owner != null) {
					(dragToItemCore.Owner as DashboardLayoutControl).handler.Invalidate();
					return;
				}
			}
		}
		bool wasReturnFromGlyph;
		int ShowHintTimerInterval { get { if(expandTypeCore == XtraDashboardLayout.ExpandType.Single && !wasReturnFromGlyph) return DashboardLayoutSettings.DragIndicatorShowTimeout; return 1; } }
		Timer showHintTimer;
		private void InitializeAffectedItems() {
			switch(insertTypeCore) {
				case InsertType.Left:
					if(dragToItemCore.Spacing.Left - dragToItemCore.Spacing.Right == DashboardLayoutSettings.DragDropIndicatorSize) {
						DevExpress.XtraLayout.Utils.Padding spacing = dragToItemCore.Spacing;
						spacing.Left -= DashboardLayoutSettings.DragDropIndicatorSize;
						affectedItems.Add(dragToItemCore, spacing);
						wasReturnFromGlyph = true;
					}
					break;
				case InsertType.Right:
					if(dragToItemCore.Spacing.Right - dragToItemCore.Spacing.Left == DashboardLayoutSettings.DragDropIndicatorSize) {
						DevExpress.XtraLayout.Utils.Padding spacing = dragToItemCore.Spacing;
						spacing.Right -= DashboardLayoutSettings.DragDropIndicatorSize;
						affectedItems.Add(dragToItemCore, spacing);
						wasReturnFromGlyph = true;
					}
					break;
				case InsertType.Top:
					if(dragToItemCore.Spacing.Top - dragToItemCore.Spacing.Bottom == DashboardLayoutSettings.DragDropIndicatorSize) {
						DevExpress.XtraLayout.Utils.Padding spacing = dragToItemCore.Spacing;
						spacing.Top -= DashboardLayoutSettings.DragDropIndicatorSize;
						affectedItems.Add(dragToItemCore, spacing);
						wasReturnFromGlyph = true;
					}
					break;
				case InsertType.Bottom:
					if(dragToItemCore.Spacing.Bottom - dragToItemCore.Spacing.Top == DashboardLayoutSettings.DragDropIndicatorSize) {
						DevExpress.XtraLayout.Utils.Padding spacing = dragToItemCore.Spacing;
						spacing.Bottom -= DashboardLayoutSettings.DragDropIndicatorSize;
						affectedItems.Add(dragToItemCore, spacing);
						wasReturnFromGlyph = true;
					}
					break;
			}
		}
		public InsertType InsertType {
			get { return insertTypeCore; }
			set { insertTypeCore = value; }
		}
		public MoveType MoveType {
			get { return moveTypeCore; }
			set { moveTypeCore = value; }
		}
		public ExpandType ExpandType {
			get { return expandTypeCore; }
			set { expandTypeCore = value; }
		}
		public BaseLayoutItem DragToItem {
			get { return dragToItemCore; }
			set { dragToItemCore = value; }
		}
		protected bool ShouldResetTimer() {
			int count = controllerStatesHistory.Count;
			if(counter == count) return true;
			return false;
		}
		public Rectangle DragHintRectangle { get { return dragHintBoundsCore; } }
		public Rectangle[] DragHints { get { return dragHints; } }
		public void ExpandTimerTick() {
			if(MoveType == XtraLayout.Utils.MoveType.Inside || expandTypeCore == XtraDashboardLayout.ExpandType.Single) return;
			if(dragControllerState > 0) showMultipleExpandTypes = false;
			dragControllerState++;
			if(ShouldResetTimer()) { dragControllerState = 1; controllerStatesHistory.Clear(); RestoreAffectedItems(true); }
		}
		int counter = 0;
		public void ShowHintTimerTick() {
			if(MoveType == XtraLayout.Utils.MoveType.Inside) return;
			LayoutGroup parent = dragToItemCore.Parent;
			if(parent == null) return;
			parent.GetMovedOutsideNeighbors(dragToItemCore, insertLocationCore, InsertLocation.Before, layoutTypeCore, before);
			parent.GetMovedOutsideNeighbors(dragToItemCore, insertLocationCore, InsertLocation.After, layoutTypeCore, after);
			ItemSorter sorter = new ItemSorter(dragToItemCore, LayoutGeometry.InvertLayout(layoutTypeCore));
			if(before.Contains(draggingItemCore)) before.Remove(draggingItemCore);
			if(after.Contains(draggingItemCore)) after.Remove(draggingItemCore);
			before.Sort(sorter);
			after.Sort(sorter);
			showMultipleExpandTypes = before.Where(item => item != draggingItemCore).Count() > 0 && after.Where(item => item != draggingItemCore).Count() > 0;
			if(expandTypeCore == ExpandType.Right && showMultipleExpandTypes)
				before.Clear();
			else if(expandTypeCore == ExpandType.Left & showMultipleExpandTypes)
				after.Clear();
			if(after.Count != 0 && before.Count != 0)
				counter = Math.Max(after.Count, before.Count);
			else counter = after.Count + before.Count;
		}
		List<Size> controllerStatesHistory = new List<Size>();
		public Rectangle GetDragPlaceGlyphBounds(BaseItemCollection items) {
			LayoutRectangle lBounds;
			int controllerState = dragControllerState;
			if(controllerState == 0)
				return Rectangle.Empty;
			else if(controllerState == 1) {
				AddItemToAffectedItems(dragToItemCore);
				lBounds = new LayoutRectangle(dragToItemCore.Bounds, layoutTypeCore);
			} else {
				if(dragToItemCore.Parent != null) {
					BaseItemCollection resultCollection = new BaseItemCollection();
					resultCollection.Add(dragToItemCore);
					for(int counter = 0; counter < controllerState - 1; counter++) {
						if(counter < before.Count) {
							resultCollection.Add(before[counter]);
						}
						if(counter < after.Count) {
							resultCollection.Add(after[counter]);
						}
					}
					foreach(BaseLayoutItem bli in resultCollection) {
						AddItemToAffectedItems(bli);
						if(items != null)
							items.Add(bli);
					}
					lBounds = resultCollection.GetLayoutItemsBounds(layoutTypeCore);
					Size last = controllerStatesHistory.Count == 0 ? Size.Empty : controllerStatesHistory[controllerStatesHistory.Count - 1];
					if(last.Width != controllerState) {
						controllerStatesHistory.Add(new Size(controllerState, resultCollection.Count));
					}
				} else
					return Rectangle.Empty;
			}
			DragHintGlyph.ShrinkRectangle(ref lBounds, insertLocationCore, DashboardLayoutSettings.DragDropIndicatorSize);
			if(layoutTypeCore == LayoutType.Horizontal) {
				lBounds.Top += padding.Bottom;
				lBounds.Height -= padding.Bottom;
			} else {
				lBounds.Top += padding.Left;
				lBounds.Height -= padding.Right;
			}
			return TranslateRectangle(lBounds.Rectangle, dragToItemCore);
		}
		protected void AddItemToAffectedItems(BaseLayoutItem item) {
			if(affectedItems.Keys.Contains(item))
				return;
			affectedItems.Add(item, item.IsDefaultPadding ? DevExpress.XtraLayout.Utils.Padding.Invalid : item.Spacing);
			DevExpress.XtraLayout.Utils.Padding newSpacing = item.Spacing;
			switch(insertTypeCore) {
				case InsertType.Left:
					newSpacing.Left += DashboardLayoutSettings.DragDropIndicatorSize;
					break;
				case InsertType.Right:
					newSpacing.Right += DashboardLayoutSettings.DragDropIndicatorSize;
					break;
				case InsertType.Top:
					newSpacing.Top += DashboardLayoutSettings.DragDropIndicatorSize;
					break;
				case InsertType.Bottom:
					newSpacing.Bottom += DashboardLayoutSettings.DragDropIndicatorSize;
					break;
			}
			item.Spacing = newSpacing;
		}
		public LayoutItemDragController GetDragController(BaseLayoutItem draggingItem) {
			Size rating = Size.Empty;
			if(expandTypeCore == ExpandType.Left) rating.Height = 0;
			if(expandTypeCore == ExpandType.Right) rating.Height = 100;
			if(expandTypeCore == ExpandType.Center) rating.Height = 50;
			BaseItemCollection items = new BaseItemCollection();
			LayoutRectangle lr = new LayoutRectangle(GetDragPlaceGlyphBounds(items), layoutTypeCore);
			LayoutItemDragController dc = new LayoutItemDragController(draggingItem, dragToItemCore, moveTypeCore, insertLocationCore, layoutTypeCore, rating);
			dc.insertToItems = items;
			return dc;
		}
		public void Drag(BaseLayoutItem draggingItem) {
			RestoreAffectedItems(false);
			if(dragToItemCore != null && dragToItemCore.Parent != null) {
				LayoutItemDragController dc = GetDragController(draggingItem);
				dc.Drag();
				if((draggingItem.Owner as DashboardLayoutControl).AllowSelection) {
					if((draggingItem.Owner as DashboardLayoutControl).handler != null) (draggingItem.Owner as DashboardLayoutControl).handler.SelectedItem = draggingItem;
					draggingItem.Selected = true;
				}
			}
		}
		#region IDisposable Members
		public void Dispose() {
			RestoreAffectedItems(false);
		}
		public void Dispose(DragHintGlyph glyph) {
			if(glyph != null) {
				if(InsertType == glyph.InsertType && dragToItemCore == glyph.TargetItem) {
					RestoreAffectedItems(true);
					return;
				}
			}
			showHintTimer.Tick -= showHintTimer_Tick;
			showHintTimer.Dispose();
			RestoreAffectedItems(false);
		}
		private void RestoreAffectedItems(bool skipFirst) {
			bool skipped = false;
			DevExpress.XtraLayout.Utils.Padding skippedSpacing = new DevExpress.XtraLayout.Utils.Padding(0);
			BaseLayoutItem skippedItem = null;
			foreach(BaseLayoutItem bli in affectedItems.Keys) {
				if(!skipped && skipFirst) {
					skipped = true;
					skippedSpacing = affectedItems[bli];
					skippedItem = bli;
					continue;
				}
				bli.Spacing = affectedItems[bli];
			}
			affectedItems.Clear();
			if(skipped) {
				affectedItems.Add(skippedItem, skippedSpacing);
			}
		}
		#endregion
	}
	public class ItemSorter :IComparer<BaseLayoutItem> {
		BaseLayoutItem attractorItem;
		LayoutType layoutType;
		public ItemSorter(BaseLayoutItem attractor, LayoutType lt) {
			attractorItem = attractor;
			layoutType = lt;
		}
		#region IComparer Members
		public int Compare(BaseLayoutItem itemX, BaseLayoutItem itemY) {
			LayoutPoint lpX, lpY, lpAttractor;
			lpX = new LayoutPoint(itemX.Location, layoutType);
			lpY = new LayoutPoint(itemY.Location, layoutType);
			lpAttractor = new LayoutPoint(attractorItem.Location, layoutType);
			int distanceX, distanceY;
			distanceX = Math.Abs(lpAttractor.X - lpX.X);
			distanceY = Math.Abs(lpAttractor.X - lpY.X);
			if(distanceY == distanceX) return 0;
			if(distanceX > distanceY) return 1;
			else return -1;
		}
		#endregion
	}
	public class DashboardNonClientSpaceDragVisualizer :NonClientSpaceDragVisualizer {
		protected internal override DragDropItemCursor DragCursor {
			get {
				return DragDropItemPreviewCursor.Default;
			}
		}
		protected override void UpdateDragCursorLocation(Point pt) {
			DragCursor.Location = pt;
		}
		protected override void UpdateDragCursorSize() {
		}
	}
	public class DashboardLayoutControlDragDropHelper :LayoutControlDragDropHelper {
		public DashboardLayoutControlDragDropHelper(ILayoutControl owner) : base(owner) { }
		protected override void InitNonClientVisualizer() {
			visualizer = new DashboardNonClientSpaceDragVisualizer();
		}
		protected override DraggingVisualizer CreateDraggingVisualizer() {
			return new DashboardLayoutControlDraggingVisualizer(Owner);
		}
		protected override void DisposeNonclientVisualizer() {
		}
		protected new DashboardLayoutControl Owner { get { return base.Owner as DashboardLayoutControl; } }
		protected override LayoutItemDragController CreateLayoutItemDragController(BaseLayoutItem dragItem, Point pt) {
			return base.CreateLayoutItemDragController(dragItem, pt);
		}
		protected override DragDropItemCursor GetDragDropItemCursor() {
			return DragDropItemPreviewCursor.Default;
		}
		protected override void DragCursorSetPos(DragDropItemCursor cursor, Point pt) {
			if(cursor.Visible) {
				Rectangle cursorRect = new Rectangle(pt.X, pt.Y, cursor.CursorWidth, cursor.CursorHeight);
				cursor.Location = Owner.PointToScreen(cursorRect.Location);
				cursor.Size = cursorRect.Size;
				cursor.BringToFront();
			}
		}
		protected override void DragCursorOn(DragDropItemCursor bcursor) {
			DragDropItemPreviewCursor cursor = bcursor as DragDropItemPreviewCursor;
			if(!cursor.Visible && !RDPHelper.IsRemoteSession) {
				cursor.FrameOwner = Owner;
				cursor.DragItemImage = draggingImage;
				cursor.Size = draggingImage.Size;
				cursor.Visible = true;
				cursor.Opacity = 0.8;
			}
		}
		public LayoutItemDragController GetDragController() {
			return Owner.handler.DraggingBehaviour.GetDragController();
		}
		public void UpdateForDragging(Point clientPoint) {
			Owner.handler.DraggingBehaviour.UpdateForDragging(clientPoint);
		}
		public override void DoDragging(Point clientPoint) {
			Owner.handler.DraggingBehaviour.DoDragging(clientPoint);
			base.DoDragging(clientPoint);
		}
		protected override void EndDragging(Point clientPoint) {
			Owner.handler.DraggingBehaviour.OnEndDragging();
		}
		Image draggingImage = null;
		protected void UpdateDraggingImage() {
			BaseLayoutItem draggingItem = DragDropDispatcherFactory.Default.DragItem;
			if(draggingItem != null) {
				draggingImage = Owner.handler.DraggingBehaviour.OnStartDragging(draggingItem);
			}
		}
		public override void OnDragEnter() {
			UpdateDraggingImage();
			base.OnDragEnter();
		}
		public override void DoBeginDrag() {
			UpdateDraggingImage();
			base.DoBeginDrag();
		}
		public override void OnDragLeave() {
			base.OnDragLeave();
		}
		public override void DoDragCancel() {
			Owner.handler.DraggingBehaviour.OnCancelDragging();
			base.DoDragCancel();
		}
	}
	public class DashboardLayoutControlDraggingVisualizer :DraggingVisualizer {
		public DashboardLayoutControlDraggingVisualizer(ILayoutControl owner) : base(owner) { }
		protected override DragFrameWindow CreateDragFrameWindow() {
			return null;
		}
		public override void ShowDragBounds(LayoutItemDragController dragController) {
		}
		public override void HideDragBounds() {
		}
	}
	public class DashboardLayoutControlDragFrameWindow :DragFrameWindow {
		public DashboardLayoutControlDragFrameWindow(ILayoutControl control) : base(control) { }
	}
}
