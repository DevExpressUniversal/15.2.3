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
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Skins;
using System.Reflection;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
namespace DevExpress.XtraDashboardLayout {
	public class Glyph {
		protected BaseBehaviour owner;
		protected ILayoutControl layoutControl;
		public Rectangle Bounds { get; set; }
		public Glyph(ILayoutControl layoutControl) {
			this.layoutControl = layoutControl;
		}
		public virtual void Paint(Graphics g) {
		}
	}
	public class SimpleGlyph : Glyph {
		public Brush Brush { get; set; }
		public SimpleGlyph(DashboardLayoutControl layoutControl)
			: base(layoutControl) {
		}
		public override void Paint(Graphics g) {
			if(Brush != null)
				g.FillRectangle(Brush, Bounds);
		}
	}
	public class DropPlaceGlyph : SimpleGlyph {
		Rectangle dragHintRect;
		Rectangle[] dragHints;
		bool showExpandTypeZones;
		Brush transparentBrush;
		public DropPlaceGlyph(DashboardLayoutControl layoutControl, Brush solidBrush, Brush transparentBrush, Rectangle dragHintRect, Rectangle[] dragHints, bool showExpandTypeZones,bool isSingle)
			: base(layoutControl) {
			this.Brush = isSingle ? transparentBrush : new SolidBrush(Color.FromArgb(200, (solidBrush as SolidBrush).Color));
			this.dragHints = dragHints;
			this.transparentBrush = transparentBrush;
			this.dragHintRect = dragHintRect;
			this.showExpandTypeZones = showExpandTypeZones;
		}
		public override void Paint(Graphics g) {
			if(showExpandTypeZones) {
				if(dragHints != null) {
					foreach(Rectangle rect in dragHints)
						g.FillRectangle(transparentBrush, rect);
				}
				g.FillRectangle(Brush, dragHintRect);
			} else
				base.Paint(g);
		}
	}
	public class DraggingGlyph : Glyph {
		public BaseLayoutItem TargetItem { get; set; }
		public DraggingGlyph(DashboardLayoutControl layoutControl)
			: base(layoutControl) {
		}
		public override void Paint(Graphics g) {
		}
	}
	public class DraggingBitmapGlyph :DraggingGlyph {
		static Image bitmap = null;
		static Point skinOffset = Point.Empty;
		static Size imageSize = new Size(20, 20);
		public DraggingBitmapGlyph(DashboardLayoutControl layoutControl, Rectangle bounds)
			: base(layoutControl) {
			GetSkinBitmap(layoutControl);
			Bounds = new Rectangle(new Point(skinOffset.X + bounds.X, skinOffset.Y + bounds.Y), imageSize);
		}
		bool GetSkinBitmap(DashboardLayoutControl layoutControl) {
			if(layoutControl != null) {
				Skin skin = DashboardSkins.GetSkin(layoutControl.LookAndFeel);
				if(skin != null) {
					SkinElement elem = skin[DashboardSkins.SkinDashboardDragHintGlyph];
					if(elem != null && elem.Glyph != null) {
						bitmap = elem.Glyph.Image;
						skinOffset = elem.Offset.Offset;
						return true;
					}
				}
				bitmap = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraLayout.DashboardLayout.Behaviours.hand_finger.png"));
			}
			return false;
		}
		public override void Paint(Graphics g) {
				g.DrawImage(bitmap, new Rectangle(Bounds.Location, imageSize));
		}
	}
	public enum ExpandType {
		Left,
		Center,
		Right,
		Single
	};
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay("DragHintGlyph: Bounds={Bounds}, InsertType={insertType},MoveType = {moveType},Brush = {Brush}")]
#endif
	public class DragHintGlyph : SimpleGlyph {
		public static void ShrinkRectangle(ref LayoutRectangle lBounds, InsertLocation insertLocation, int newSize) {
			if(insertLocation == InsertLocation.After) {
				lBounds.X = lBounds.Right - newSize;
				lBounds.Width = newSize;
			} else {
				lBounds.Width = newSize;
			}
		}
		readonly int glyphsPadding = 2;
		ExpandType expandType;
		InsertType insertType;
		MoveType moveType;
		BaseLayoutItem targetItemCore;
		Rectangle[] dragHints;
		InsertLocation insertLocationCore;
		LayoutType layoutTypeCore;
		Triangle triangle; 
		public Rectangle[] DragHints { get { return dragHints; } }
		public InsertLocation InsertLocation { get { return insertLocationCore; } }
		public Triangle Triangle { get { return triangle; } }
		public LayoutType LayoutType { get { return layoutTypeCore; } }
		public InsertType InsertType {
			get { return insertType; }
			set { insertType = value; }
		}
		public MoveType MoveType {
			get { return moveType; }
			set { moveType = value; }
		}
		public ExpandType ExpandType {
			get { return expandType; }
			set { expandType = value; }
		}
		public BaseLayoutItem TargetItem {
			get { return targetItemCore; }
			set { targetItemCore = value; }
		}
		public override void Paint(Graphics g) {
		}
		public DragHintGlyph(DashboardLayoutControl layoutControl, InsertType insertType, MoveType moveType, ExpandType expandType, BaseLayoutItem bli)
			: base(layoutControl) {
			this.expandType = expandType;
			this.insertType = insertType;
			this.moveType = moveType;
			this.targetItemCore = bli;
			if(moveType == XtraLayout.Utils.MoveType.Inside){
				Bounds = GetInsideEmptyGroupRect(bli);
				Brush = Brushes.White;
				layoutTypeCore = XtraLayout.Utils.LayoutType.Horizontal;
				dragHints = new Rectangle[] { Bounds, Bounds, Bounds };
			} else {
				FillForItem(insertType, expandType, bli,(bli is DashboardLayoutControlGroupBase && ((DashboardLayoutControlGroupBase)bli).ItemCount == 0));
			}
		}
		private void FillForItem(InsertType insertType, ExpandType expandType, BaseLayoutItem bli, bool emptyGroup) {
			Rectangle baseRect = bli.ViewInfo.BoundsRelativeToControl;
			int width = baseRect.Width;
			Point start = baseRect.Location;
			Size size;
			Orientation orientation = Orientation.Horizontal;
			size = new Size(width, DashboardLayoutSettings.DragDropIndicatorSize);
			Triangle triangle = new Triangle();
			switch(insertType) {
				case InsertType.Top:
					start = new Point(baseRect.Left + DashboardLayoutSettings.DragDropIndicatorSize, baseRect.Top);
					size = new Size(width - DashboardLayoutSettings.DragDropIndicatorSize * 2, DashboardLayoutSettings.DragDropIndicatorSize);
					orientation = Orientation.Horizontal;
					triangle = new Triangle(baseRect.Location,new Point(baseRect.X + baseRect.Width / 2,baseRect.Y+ baseRect.Height /2),new Point(baseRect.Right,baseRect.Y));
					break;
				case InsertType.Left:
					start = new Point(baseRect.Top , baseRect.Left);
					size.Width = baseRect.Height;
					orientation = Orientation.Vertical;
					triangle = new Triangle(baseRect.Location,new Point(baseRect.X + baseRect.Width / 2,baseRect.Y+ baseRect.Height /2),new Point(baseRect.X, baseRect.Bottom));
					break;
				case InsertType.Right:
					start = new Point(baseRect.Top, baseRect.Right - DashboardLayoutSettings.DragDropIndicatorSize);
					size.Width = baseRect.Height;
					orientation = Orientation.Vertical;
					triangle = new Triangle(new Point(baseRect.Right, baseRect.Y), new Point(baseRect.X + baseRect.Width / 2, baseRect.Y + baseRect.Height / 2), new Point(baseRect.Right, baseRect.Bottom));
					break;
				case InsertType.Bottom:
					size = new Size(width - DashboardLayoutSettings.DragDropIndicatorSize * 2, DashboardLayoutSettings.DragDropIndicatorSize);
					start = new Point(baseRect.Left + DashboardLayoutSettings.DragDropIndicatorSize, baseRect.Bottom - DashboardLayoutSettings.DragDropIndicatorSize);
					orientation = Orientation.Horizontal;
					triangle = new Triangle(new Point(baseRect.X, baseRect.Bottom),new Point(baseRect.X + baseRect.Width / 2,baseRect.Y+ baseRect.Height /2),new Point(baseRect.Right, baseRect.Bottom));
					break;
			}
			width = size.Width / 3;
			switch(expandType) {
				case ExpandType.Left:
					size.Width = width;
					Brush = new SolidBrush(Color.FromArgb(50, Color.Red));
					break;
				case ExpandType.Center:
					start.X += width;
					size.Width = size.Width - 2 * width;
					Brush = new SolidBrush(Color.FromArgb(50, Color.Green));
					break;
				case ExpandType.Right:
					start.X += size.Width - width;
					size.Width = width;
					Brush = new SolidBrush(Color.FromArgb(50, Color.Blue));
					break;
				case ExpandType.Single:
					Brush = new SolidBrush(Color.FromArgb(50, Color.Yellow));
					start = Point.Empty;
					size = Size.Empty;
					this.triangle = triangle;
					break;
			}
			if(orientation == Orientation.Horizontal) {
				Bounds = new Rectangle(start, size);
			} else {
				Bounds = new Rectangle(start.Y, start.X, size.Height, size.Width);
			}
		}
		private static Rectangle GetInsideEmptyGroupRect(BaseLayoutItem bli) {
			Point insidePoint = bli.ViewInfo.ClientAreaRelativeToControl.Location;
			Size insideSize = bli.ViewInfo.BoundsRelativeToControl.Size;
			insideSize.Height -= bli.ViewInfo.Padding.Height + bli.Spacing.Height;
			insideSize.Width -= bli.ViewInfo.Padding.Width + bli.Spacing.Width;
			return new Rectangle(insidePoint, insideSize);
		}
		public void SetDragHints(DragHintGlyph[] glyphs) {
			XtraLayout.Utils.Padding padding = XtraLayout.Utils.Padding.Empty;
			InsertTypeToInsertLocationLayoutTypesConverter.Convert(InsertType, out insertLocationCore, out layoutTypeCore);
			LayoutRectangle dragHintBoundsLeft = new LayoutRectangle(glyphs[0].Bounds, layoutTypeCore);
			LayoutRectangle dragHintBoundsCenter = new LayoutRectangle(glyphs[1].Bounds, layoutTypeCore);
			LayoutRectangle dragHintBoundsRight = new LayoutRectangle(glyphs[2].Bounds, layoutTypeCore);
			if(layoutTypeCore == LayoutType.Horizontal) {
				dragHintBoundsRight.Bottom = TargetItem.ViewInfo.BoundsRelativeToControl.Bottom;
				dragHintBoundsLeft.Top = TargetItem.ViewInfo.BoundsRelativeToControl.Top;
				dragHintBoundsLeft.Top += padding.Bottom;
				dragHintBoundsLeft.Height -= glyphsPadding;
				dragHintBoundsRight.Top += glyphsPadding;
				dragHintBoundsRight.Height -= padding.Bottom;
				dragHintBoundsCenter.Top += glyphsPadding;
				dragHintBoundsCenter.Height -= glyphsPadding;
			} else {
				dragHintBoundsLeft.Top += padding.Left;
				dragHintBoundsLeft.Height -= glyphsPadding;
				dragHintBoundsCenter.Top += glyphsPadding;
				dragHintBoundsCenter.Height -= glyphsPadding;
				dragHintBoundsRight.Top += glyphsPadding;
				dragHintBoundsRight.Height -= padding.Right;
			}
			ShrinkRectangle(ref dragHintBoundsLeft, insertLocationCore, DashboardLayoutSettings.DragDropIndicatorSize);
			ShrinkRectangle(ref dragHintBoundsCenter, insertLocationCore, DashboardLayoutSettings.DragDropIndicatorSize);
			ShrinkRectangle(ref dragHintBoundsRight, insertLocationCore, DashboardLayoutSettings.DragDropIndicatorSize);
			dragHints = new Rectangle[] { dragHintBoundsLeft.Rectangle, dragHintBoundsCenter.Rectangle,dragHintBoundsRight.Rectangle};
		}
	}
	public class CrosshairGlyph : Glyph {
		public Crosshair Crosshair { get; set; }
		public Point ImageLocation { get; set; }
		LayoutControlImageStorage imageStorage;
		public bool Visible { get; set; }
		public LayoutControlImageStorage ImageStorage {
			get {
				if(imageStorage != null) return imageStorage;
				imageStorage = new LayoutControlImageStorage();
				return imageStorage;
			}
		}
		public CrosshairGlyph(DashboardLayoutControl layoutControl, Crosshair crosshair, Point imageLocation,Rectangle glyphBounds)
			: base(layoutControl) {
			Crosshair = crosshair;
			ImageLocation = imageLocation;
			Bounds = glyphBounds;
		}
		public override void Paint(Graphics g) {
			if(!Visible) return;
			switch(Crosshair.CrosshairGroupingType) {
				case CrosshairGroupingTypes.GroupHorizontal:
					g.DrawImage(ImageStorage.CrossHorizontal, ImageLocation);
					break;
				case CrosshairGroupingTypes.GroupVertical:
					g.DrawImage(ImageStorage.CrossVertical, ImageLocation);
					break;
				case CrosshairGroupingTypes.GroupBoth:
					g.DrawImage(ImageStorage.CrossLock, ImageLocation);
					break;
				case CrosshairGroupingTypes.NotSet:
					DrawRealCrosshairType(g);
					break;
			}
		}
		private void DrawRealCrosshairType(Graphics g) {
			switch(Crosshair.realCrosshairTypeCore) {
				case CrosshairGroupingTypes.GroupHorizontal:
					g.DrawImage(ImageStorage.CrossHorizontal, ImageLocation);
					break;
				case CrosshairGroupingTypes.GroupVertical:
					g.DrawImage(ImageStorage.CrossVertical, ImageLocation);
					break;
				case CrosshairGroupingTypes.GroupBoth:
					g.DrawImage(ImageStorage.CrossLock, ImageLocation);
					break;
			}
		}
	}
	public struct Triangle {
		Point A;
		Point B;
		Point C;
		public Point[] Points { get { return new Point[] { A, B, C }; } }
		public Triangle(Point a, Point b, Point c) {
			A = a; B = b; C = c;
		}
		public bool Contains(Point p) {
			int res1 = (A.X - p.X) * (B.Y - A.Y) - (B.X - A.X) * (A.Y - p.Y);
			int res2 = (B.X - p.X) * (C.Y - B.Y) - (C.X - B.X) * (B.Y - p.Y);
			int res3 = (C.X - p.X) * (A.Y - C.Y) - (A.X - C.X) * (C.Y - p.Y);
			if((res1 >= 0 && res2 >= 0 && res3 >= 0) || (res1 <= 0 && res2 <= 0 && res3 <= 0)) {
				return true;
			}
			return false;
		}
		public static Triangle Empty { get {return new Triangle(Point.Empty,Point.Empty,Point.Empty); } }
	}
}
