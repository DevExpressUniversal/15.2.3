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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.XtraPivotGrid.Customization.ViewInfo {
	public class CustomizationTreeItemViewInfo : CustomizationItemViewInfo {
		readonly OpenCloseButtonViewInfo button;
		readonly PivotCustomizationTreeItemCaptionViewInfo caption;
		readonly PivotCustomizationTreeItemIconViewInfo icon;
		readonly CustomizationTreeBox tree;
		public CustomizationTreeItemViewInfo(CustomizationTreeBox treeBox, IVisualCustomizationTreeItem node)
			: base(node) {
			this.tree = treeBox;
			this.caption = CreateCaption(node);
			this.button = CreateButton(node, treeBox.LookAndFeel);
			this.icon = CreateIcon(node);
			button.Info.Opened = node.IsExpanded;
		}
		protected virtual PivotCustomizationTreeItemIconViewInfo CreateIcon(IVisualCustomizationTreeItem node) {
			return new PivotCustomizationTreeItemIconViewInfo(PivotGridControl.CustomizationTreeNodeImages, node.ImageIndex);
		}
		protected virtual OpenCloseButtonViewInfo CreateButton(IVisualCustomizationTreeItem node, UserLookAndFeel activeLookAndFeel) {
			return !node.CanExpand ?
				new NullOpenCloseButtonViewInfo() :
				new OpenCloseButtonViewInfo(activeLookAndFeel.Painter.OpenCloseButton);
		}
		protected virtual PivotCustomizationTreeItemCaptionViewInfo CreateCaption(IVisualCustomizationTreeItem node) {
			return new PivotCustomizationTreeItemCaptionViewInfo(tree, node.Caption);
		}
		protected virtual BaseListBoxViewInfo CreateListViewInfo() {
			return new BaseListBoxViewInfo(tree);
		}
		public override void Paint(GraphicsCache cache, Rectangle bounds, bool hotTrack, bool selected, bool focused) {
			Calculate(bounds);
			button.Paint(cache);
			icon.Paint(cache);
			caption.Paint(cache, selected, focused);
		}
		public override bool IsOpenCloseButton(Rectangle bounds, Point point) {
			Calculate(bounds);
			return button.Bounds.Contains(point);
		}
		public override ToolTipControlInfo GetToolTipObjectInfo() {
			return null;
		}
		public int CalcHeight() {
			int res = 0;
			res = Math.Max(res, button.CalcHeight());
			res = Math.Max(res, icon.CalcHeight());
			res = Math.Max(res, caption.CalcHeight());
			return res;
		}
		protected virtual void Calculate(Rectangle bounds) {
			PivotCustomizationTreeRestBounds rest = new PivotCustomizationTreeRestBounds(bounds);
			rest.Indent(Item.Level);
			button.Calculate(rest);
			icon.Calculate(rest);
			caption.Calculate(rest);
		}
		public override void MouseDown(MouseEventArgs e) {
			base.MouseDown(e);
			PivotFieldItem field = (PivotFieldItem)Item.Field;
			if (field == null || (e.Button & MouseButtons.Right) == 0 || tree == null || tree.CustomizationForm == null || !tree.CustomizationForm.Data.OptionsMenu.EnableHeaderMenu)
				return;
			new TreeViewHeader(tree.CustomizationForm.Data.ViewInfo, Item, tree).ShowPopupMenu(e);
		}
		class TreeViewHeader : DevExpress.XtraPivotGrid.Customization.ViewInfo.PivotCustomizationListItemViewInfo.ListHeaderViewInfo {
			Control owner;
			public TreeViewHeader(PivotGridViewInfo viewInfo, IVisualCustomizationTreeItem item, Control owner) : base(null, viewInfo, item) {				
				this.owner = owner;
			}
			protected override Control GetControlOwner() {
				return owner;
			}
		}
	}
	public class PivotCustomizationTreeRestBounds {
		Rectangle bounds;
		public Rectangle Bounds { get { return bounds; } }
		public PivotCustomizationTreeRestBounds(Rectangle bounds) {
			this.bounds = bounds;
		}
		public int GetIndent(Rectangle innerRectangle) {
			return (bounds.Height - innerRectangle.Height) / 2;
		}
		public void Indent(int factor) {
			Reduce(bounds.Height * factor);
		}
		public void Reduce(int x) {
			this.bounds.Offset(x / 2, 0);
			this.bounds.Inflate(-x / 2, 0);
		}
	}
	public class PivotCustomizationTreeBaseViewInfo {
		ObjectInfoArgs info;
		ObjectPainter painter;
		protected PivotCustomizationTreeBaseViewInfo(ObjectPainter painter, ObjectInfoArgs info) {
			this.painter = painter;
			this.info = info;
		}
		public ObjectPainter Painter { get { return painter; } }
		public ObjectInfoArgs Info { get { return info; } }
		public virtual void Paint(GraphicsCache cache) {
			this.info.Cache = cache;
			this.painter.DrawObject(this.info);
		}
		public virtual void Calculate(PivotCustomizationTreeRestBounds calculator) {
			Rectangle bounds = Painter.CalcObjectMinBounds(Info);
			bounds.Offset(calculator.Bounds.X + calculator.GetIndent(bounds), calculator.Bounds.Y + calculator.GetIndent(bounds));
			calculator.Reduce(2 * calculator.GetIndent(bounds) + bounds.Width);
			Bounds = bounds;
		}
		public int CalcHeight() { return Painter.CalcObjectMinBounds(Info).Height; }
		public Rectangle Bounds { get { return info.Bounds; } set { info.Bounds = value; } }
	}
	public class PivotCustomizationTreeItemIconViewInfo : PivotCustomizationTreeBaseViewInfo {
		public PivotCustomizationTreeItemIconViewInfo(object imageList, int index) 
			: base(PivotViewInfo.CreateGlyphPainter(false), PivotViewInfo.CreateGlyphInfoArgs(imageList, index, false)) { }
	}
	public class PivotCustomizationTreeItemCaptionViewInfo {
		readonly string caption;
		readonly CustomizationTreeBox tree;
		Rectangle bounds;
		public PivotCustomizationTreeItemCaptionViewInfo(CustomizationTreeBox tree, string caption) {
			this.tree = tree;
			this.caption = caption;
		}
		protected BaseListBoxViewInfo ListViewInfo { get { return tree.ViewInfo; } }
		protected BaseListBoxItemPainter Painter { get { return tree.ListItemPainter; } }
		ListBoxItemObjectInfoArgs CreateInfoArgs(GraphicsCache cache, bool selected, bool focused) {
			ListBoxItemObjectInfoArgs infoArgs = new ListBoxItemObjectInfoArgs(tree.ViewInfo, cache, this.bounds);
			infoArgs.ItemText = this.caption;
			infoArgs.TextRect = this.bounds;
			if(selected) {
				infoArgs.ItemState = infoArgs.ItemState | DrawItemState.Selected;
				if(focused)
					infoArgs.ItemState = infoArgs.ItemState | DrawItemState.Focus;
			}
			infoArgs.PaintAppearance = ListViewInfo.PaintAppearance;
			return infoArgs;
		}
		public void Paint(GraphicsCache cache, bool selected, bool focused) {
			ListBoxItemObjectInfoArgs infoArgs = CreateInfoArgs(cache, selected, focused);
			Painter.DrawObject(infoArgs);
		}		
		public void Calculate(PivotCustomizationTreeRestBounds calculator) {
			this.bounds = calculator.Bounds;
		}
		public int CalcHeight() {
			ListBoxItemObjectInfoArgs infoArgs = CreateInfoArgs(null, false, false);
			return Painter.CalcObjectMinBounds(infoArgs).Height;
		}
	}
	public class OpenCloseButtonViewInfo : PivotCustomizationTreeBaseViewInfo {
		public OpenCloseButtonViewInfo(ObjectPainter painter)
			: base(painter, new OpenCloseButtonInfoArgs(null)) {
		}
		public new OpenCloseButtonInfoArgs Info { get { return (OpenCloseButtonInfoArgs)base.Info; } }
	}
	public class NullOpenCloseButtonViewInfo : OpenCloseButtonViewInfo {
		public NullOpenCloseButtonViewInfo() : base(new ObjectPainter()) { }
		public override void Paint(GraphicsCache cache) { }
	}
}
