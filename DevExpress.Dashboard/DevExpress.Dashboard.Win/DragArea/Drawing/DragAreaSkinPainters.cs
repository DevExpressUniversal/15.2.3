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

using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using System.Collections.Generic;
namespace DevExpress.DashboardWin.Native {
	public class DragAreaSkinPainterBase : DragAreaPainter {
		readonly SkinElement skinElement;
		readonly int horizontalMargins;
		protected SkinElement SkinElement { get { return skinElement; } }
		public override int HorizontalMargins { get { return horizontalMargins; } }
		public DragAreaSkinPainterBase(UserLookAndFeel lookAndFeel, string elementName)  {
			Skin skin = DashboardSkins.GetSkin(lookAndFeel);
			skinElement = skin == null ? null : skin[elementName];
			if(skinElement != null) {
				SkinPaddingEdges contentMargins = skinElement.ContentMargins;
				horizontalMargins = contentMargins.Left + contentMargins.Right;
			}
		}
		protected int GetIntegerProperty(string propertyName) {
			return skinElement == null ? 0 : (int)skinElement.Properties.GetInteger(propertyName);
		}
		protected Color GetColorProperty(string propertyName) {
			return skinElement == null ? Color.Empty : (Color)skinElement.Properties.GetColor(propertyName);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return skinElement == null ? client : SkinElementPainter.Default.CalcBoundsByClientRectangle(new SkinElementInfo(skinElement, client));
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return skinElement == null ? e.Bounds : SkinElementPainter.Default.GetObjectClientRectangle(new SkinElementInfo(skinElement, e.Bounds));
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SkinElementInfo elementInfo = new SkinElementInfo(skinElement, e.Bounds);
			PrepareElementInfo(elementInfo, e);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, elementInfo);
		}
		protected virtual void PrepareElementInfo(SkinElementInfo elementInfo, ObjectInfoArgs e) {
		}
	}
	public class DragAreaSkinPainter : DragAreaSkinPainterBase, IDragAreaPainter {
		public int SectionIndent { get { return GetIntegerProperty("SectionIndent"); } }
		public Color SectionHeaderColor { get { return GetColorProperty("ColorSectionHeader"); } }
		public DragAreaSkinPainter(UserLookAndFeel lookAndFeel) : base(lookAndFeel, DashboardSkins.SkinDragArea) {
		}
	}
	public class DragGroupSkinPainter : DragAreaSkinPainterBase, IDragGroupPainter {
		public int GroupIndent { get { return GetIntegerProperty("GroupIndent"); } }
		public int ItemIndent { get { return GetIntegerProperty("ItemIndent"); } }
		public int ButtonIndent { get { return GetIntegerProperty("ButtonIndent"); } }
		public Color DropIndicatorColor { get { return GetColorProperty("ColorDropIndicator"); } }
		public DragGroupSkinPainter(UserLookAndFeel lookAndFeel) : base(lookAndFeel, DashboardSkins.SkinDragGroup) {
		}
		protected override void PrepareElementInfo(SkinElementInfo elementInfo, ObjectInfoArgs e) {
			base.PrepareElementInfo(elementInfo, e);
			GroupInfoArgs args = e as GroupInfoArgs;
			elementInfo.ImageIndex = args == null ? 0 : (int)args.State;
		}
	}
	public class DragGroupSelectorSkinPainter : DragAreaSkinPainterBase {
		public DragGroupSelectorSkinPainter(UserLookAndFeel lookAndFeel) : base(lookAndFeel, DashboardSkins.SkinDragGroupOptionsButton) {
		}
		protected override void PrepareElementInfo(SkinElementInfo elementInfo, ObjectInfoArgs e) {
			base.PrepareElementInfo(elementInfo, e);
			ElementWithButtonInfoArgs args = e as ElementWithButtonInfoArgs;
			elementInfo.ImageIndex = args == null ? 0 : (int)args.ButtonState;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			DragAreaButtonInfoArgs args = e as DragAreaButtonInfoArgs;
			if (args != null) {
				Image glyph = args.Glyph;
				if (glyph != null) {
					int width = glyph.Width;
					int height = glyph.Height;
					Rectangle bounds = e.Bounds;
					e.Graphics.DrawImage(glyph, bounds.Left + (bounds.Width - width) / 2, bounds.Top + (bounds.Height - height) / 2, width, height);
				}
			}
		}
	}
	public class DragItemSkinPainter : DragAreaSkinPainterBase {
		const int HierarchyItemShift = 4;
		public DragItemSkinPainter(UserLookAndFeel lookAndFeel) : base(lookAndFeel, DashboardSkins.SkinDragItem) {
		}
		protected override void PrepareElementInfo(SkinElementInfo elementInfo, ObjectInfoArgs e) {
			base.PrepareElementInfo(elementInfo, e);
			DragItemInfoArgs args = e as DragItemInfoArgs;
			if(args != null) 
				elementInfo.Bounds = args.CurrentBounds;
			elementInfo.ImageIndex = (int)args.ItemState;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DragItemInfoArgs args = e as DragItemInfoArgs;
			if(args.IsDraggingHierarchy) {
				args.CurrentBounds = new Rectangle(e.Bounds.X + HierarchyItemShift, e.Bounds.Y, e.Bounds.Width - HierarchyItemShift, e.Bounds.Height - HierarchyItemShift);
				base.DrawObject(e);
				args.CurrentBounds = new Rectangle(e.Bounds.X, e.Bounds.Y + HierarchyItemShift, e.Bounds.Width - HierarchyItemShift, e.Bounds.Height - HierarchyItemShift);
			}
			base.DrawObject(e);
			if(args != null) {
				IList<Image> glyphs = args.Glyphs;
				if(glyphs != null && glyphs.Count > 0) {
					Rectangle bounds = args.CurrentBounds;
					int xMargin = (args.NormalGroupHeight - glyphs[0].Height) / 2;
					int yMargins = (bounds.Height - glyphs[0].Height) / 2;
					Point glyphLocation = new Point(bounds.X + xMargin, bounds.Y + yMargins);
					foreach(Image glyph in glyphs) {
						e.Graphics.DrawImage(glyph, new Point(glyphLocation.X, glyphLocation.Y));
						glyphLocation = new Point(glyphLocation.X + glyph.Width + args.StatusIconInnerIndent, glyphLocation.Y);
					}
				}
			}
		}
	}
	public class DragItemOptionsButtonSkinPainter : DragAreaSkinPainterBase, IDragItemOptionsButtonPainter {
		public DragItemOptionsButtonSkinPainter(UserLookAndFeel lookAndFeel) : base(lookAndFeel, DashboardSkins.SkinDragItemOptionsButton) {
		}
		public Rectangle GetActualBounds(Rectangle bounds) {
			Point location = bounds.Location;
			SkinElement skinElement = SkinElement;
			if (skinElement == null)
				return bounds;
			SkinGlyph glyph = skinElement.Glyph;
			SkinImage image = skinElement.Image;
			if (glyph == null || image == null)
				return bounds;
			Rectangle imageBounds = glyph.GetImageBounds(0);
			SkinPaddingEdges margins = skinElement.ContentMargins;
			int imageWidth = imageBounds.Width + margins.Width;
			int imageHeight = imageBounds.Height + margins.Height;
			return new Rectangle(location.X + bounds.Width - imageWidth - 3, location.Y + (bounds.Height - imageHeight) / 2, imageWidth, imageHeight);
		}
		protected override void PrepareElementInfo(SkinElementInfo elementInfo, ObjectInfoArgs e) {
			base.PrepareElementInfo(elementInfo, e);
			ElementWithButtonInfoArgs args = e as ElementWithButtonInfoArgs;
			elementInfo.ImageIndex = args == null ? 0 : (int)args.ButtonState;
		}
	}
	public class SplitterDragSectionSkinPainter : StyleObjectPainter {
		readonly SkinElement skinElement;
		public SplitterDragSectionSkinPainter(UserLookAndFeel lookAndFeel) {
			Skin skin = CommonSkins.GetSkin(lookAndFeel);
			skinElement = skin == null ? null : skin[CommonSkins.SkinLabelLine];
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SkinElementInfo elementInfo = new SkinElementInfo(skinElement, e.Bounds);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, elementInfo);
		}
	}
	public class DragAreaSkinPainters : DragAreaPainters {
		public DragAreaSkinPainters(UserLookAndFeel lookAndFeel) {
			AreaPainter = new DragAreaSkinPainter(lookAndFeel);
			GroupPainter = new DragGroupSkinPainter(lookAndFeel);
			GroupSelectorPainter = new DragGroupSelectorSkinPainter(lookAndFeel);
			DragItemPainter = new DragItemSkinPainter(lookAndFeel);
			DragItemOptionsButtonPainter = new DragItemOptionsButtonSkinPainter(lookAndFeel);
			SplitterPainter = new SplitterDragSectionSkinPainter(lookAndFeel);
		}
	}
}
