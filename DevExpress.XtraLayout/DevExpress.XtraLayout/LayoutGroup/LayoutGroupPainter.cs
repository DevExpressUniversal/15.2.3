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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.Utils.Drawing;
using DevExpress.XtraLayout.ViewInfo;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraLayout.Registrator;
using System.Collections;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraLayout.Painting {
	public class LCFlatGroupObjectPainter : FlatGroupObjectPainter {
		public LCFlatGroupObjectPainter(IPanelControlOwner owner) : base(owner) { }
		public override AppearanceDefault DefaultAppearance {
			get {
				return new AppearanceDefault(GetForeColor(), GetBackColor());
			}
		}
		public override AppearanceDefault DefaultAppearanceCaption {
			get {
				return new AppearanceDefault(GetForeColor(), GetBackColor());
			}
		}
		protected virtual Color GetForeColor() {
			if(Owner != null && Owner.GetForeColor() != Color.Empty) return Owner.GetForeColor();
			return SystemColors.ControlText;
		}
		protected virtual Color GetBackColor() {
			return SystemColors.Control;
		}
	}
	public class LayoutGroupPainter : BaseLayoutItemPainter, IPanelControlOwner {
		public LayoutGroupPainter() : base() { }
		protected override ObjectPainter CreateBorderPainter(BaseViewInfo e) {
			return e.Owner.PaintStyle.CreateGroupPainter(this);
		}
		Color IPanelControlOwner.GetForeColor() { return Color.Empty; }
		void IPanelControlOwner.OnCustomDrawCaption(GroupCaptionCustomDrawEventArgs e) {
			OnCustomDrawCaptionCore(e);
		}
		protected virtual void OnCustomDrawCaptionCore(GroupCaptionCustomDrawEventArgs e) { }
		public override void DrawControlsArea(BaseLayoutItemViewInfo e) {
			if(e != null) {
				LayoutGroup group = e.Owner as LayoutGroup;
				if(group.Expanded) {
					if(group.LayoutMode == Utils.LayoutMode.Table && (group.Owner.EnableCustomizationMode || group.Owner.DesignMode)) {
						DrawTable(group, e);
					}
					if(group.Items.Count > 0) {
						if((group.Owner.EnableCustomizationMode || group.Owner.DesignMode)) {
							if(group.LayoutMode == Utils.LayoutMode.Flow && group.CellSize.Width !=0 && group.CellSize.Height !=0) {
								if((group.Owner is LayoutControl) && (group.Owner as LayoutControl).layoutAdornerWindowHandler == null) DrawItems(group, e);
								DrawFlowLines(group, e);
							}
						}
						foreach(BaseLayoutItem item in new ArrayList(group.Items))
							ObjectPainter.DrawObject(e.Cache, item.PaintStyle.GetPainter(item), item.ViewInfo);
					} else {
						e.Cache.FillRectangle(SystemBrushes.ActiveBorder, e.TextAreaRelativeToControl);
						Rectangle rect = group.ViewInfo.PainterBoundsRelativeToControl;
						rect.Width -= 20;
						rect.Height -= 20;
						rect.X += 10;
						rect.Y += 10;
						if(group != null && group.Owner != null && (e.OwnerILayoutControl.EnableCustomizationMode || e.OwnerILayoutControl.DesignMode)) {
							if(group.ParentTabbedGroup == null && group.Owner.AllowPaintEmptyRootGroupText && rect.Height > 30) {
								using(AppearanceObject app = new AppearanceObject()) {
									app.ForeColor = Color.Black;
									app.TextOptions.VAlignment = VertAlignment.Center;
									app.TextOptions.HAlignment = HorzAlignment.Center;
									app.DrawString(e.Cache, LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.EmptyRootGroupText), rect);
								}
							}
						}
					}
				}
			}
		}
		private void DrawTable(LayoutGroup group, BaseLayoutItemViewInfo e) {
			Rectangle groupBoundsToControl = new Rectangle(group.ViewInfo.ClientAreaRelativeToControl.Location, group.ViewInfo.SubLabelSizeIndentions(group.ViewInfo.BoundsRelativeToControl.Size));
			Point firstPoint = groupBoundsToControl.Location;
			Point secondPoint = new Point(groupBoundsToControl.X, groupBoundsToControl.Bottom);
			e.Graphics.DrawLine(GetBorderForTableLinesPen(e), firstPoint, secondPoint);
			foreach(ColumnDefinition col in group.OptionsTableLayoutGroup.ColumnDefinitions) {
				firstPoint.X += (int)col.realSize;
				secondPoint.X += (int)col.realSize;
				e.Graphics.DrawLine(GetBorderForTableLinesPen(e), firstPoint, secondPoint);
			}
			firstPoint = groupBoundsToControl.Location;
			secondPoint = new Point(groupBoundsToControl.Right, groupBoundsToControl.Y);
			e.Graphics.DrawLine(GetBorderForTableLinesPen(e), firstPoint, secondPoint);
			foreach(RowDefinition row in group.OptionsTableLayoutGroup.RowDefinitions) {
				firstPoint.Y += (int)row.realSize;
				secondPoint.Y += (int)row.realSize;
				e.Graphics.DrawLine(GetBorderForTableLinesPen(e), firstPoint, secondPoint);
			}
		}
		Pen GetBorderForTableLinesPen(BaseLayoutItemViewInfo e) {
			Pen result = GetBorderForFlowLinesPen(e);
			result.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			return result;
		}
		private void DrawItems(LayoutGroup group, BaseLayoutItemViewInfo e) {
			FlowHelper.DrawItems(group, e.Cache);
		}
		protected override void DrawBackground(BaseLayoutItemViewInfo e) {
			LayoutGroup group = e.Owner as LayoutGroup;
			LayoutGroupViewInfo vi = e as LayoutGroupViewInfo;
			vi.UpdateAppearance();
			bool drawBackground = (group.GroupBordersVisible || (!group.GroupBordersVisible && group.AllowDrawBackground));
			if(drawBackground) ObjectPainter.DrawObject(e.Cache, GetBorderPainter(e), vi.BorderInfo);
			if(group.GroupBordersVisible) DrawBorder(e);
		}
		private void DrawFlowLines(LayoutGroup group, BaseLayoutItemViewInfo e) {
			Rectangle groupBoundsToControl = group.ViewInfo.BoundsRelativeToControl;
			if(group.Items == null || group.Items.Count == 0) return;
			var firstItem = group.Items[0];
			Rectangle firstBoundsToControl = firstItem.ViewInfo.BoundsRelativeToControl;
			System.Drawing.Drawing2D.GraphicsState save = e.Graphics.Save();
			foreach(BaseLayoutItem item in group.Items) {
				e.Graphics.SetClip(item.ViewInfo.BoundsRelativeToControl, System.Drawing.Drawing2D.CombineMode.Exclude);
			}
			for(int y = firstBoundsToControl.Y; y < groupBoundsToControl.Height + groupBoundsToControl.Y; y += group.CellSize.Height) {
				e.Graphics.DrawLine(GetBorderForFlowLinesPen(e), new Point(firstBoundsToControl.X - 1, y), new Point(groupBoundsToControl.X + groupBoundsToControl.Width - 1, y));
		   }
			for(int x = firstItem.ViewInfo.BoundsRelativeToControl.X; x < groupBoundsToControl.Width + groupBoundsToControl.X; x += group.CellSize.Width) {
				e.Graphics.DrawLine(GetBorderForFlowLinesPen(e), new Point(x, firstBoundsToControl.Y - 1), new Point(x, groupBoundsToControl.Y + groupBoundsToControl.Height - 1));
		   }
			e.Graphics.Restore(save);
		}
	}
}
