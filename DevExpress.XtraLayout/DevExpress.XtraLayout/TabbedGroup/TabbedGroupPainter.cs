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
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.XtraLayout.Tab;
using System.Drawing;
using DevExpress.XtraLayout.ViewInfo;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraLayout.Registrator;
namespace DevExpress.XtraLayout.Painting {
	public class TabbedGroupPainter : BaseLayoutItemPainter {
		public TabbedGroupPainter(): base() {}
		protected override ObjectPainter CreateBorderPainter(BaseViewInfo e) {
			return new TabObjectPainter();
		}
		public override void DrawObject(ObjectInfoArgs e) {
			if(e != null) {
				BaseLayoutItemViewInfo vi = e as BaseLayoutItemViewInfo;
				if(!vi.Owner.ActualItemVisibility) return;
				if(vi.ClientAreaRelativeToControl.Contains(e.Cache.PaintArgs.ClipRectangle)) {
					DrawBackground(vi);
					DrawControlsArea(vi);
				}
				else {
					if (e.Cache.PaintArgs.ClipRectangle.IntersectsWith(vi.PainterBoundsRelativeToControl))
					{
						vi.UpdateState();
						DrawBackground(vi);
						DrawSelection(vi);
						DrawControlsArea(vi);
					}
				}
			}
		}
		public override void DrawControlsArea(BaseLayoutItemViewInfo e) {
			if(e != null) {
				LayoutGroup activeTab = ((TabbedGroup)e.Owner).SelectedTabPage;
				if(activeTab != null) {
					activeTab.ViewInfo.Cache = e.Cache;
					activeTab.PaintStyle.GetPainter(activeTab).DrawControlsArea(activeTab.ViewInfo);
					activeTab.PaintStyle.GetPainter(activeTab).DrawSelectionInTab(activeTab.ViewInfo);
				}
				else {
					e.Cache.FillRectangle(SystemBrushes.ActiveBorder, e.ClientAreaRelativeToControl);
					Rectangle rect = e.Owner.ViewInfo.PainterBoundsRelativeToControl;
					rect.Width -= 20;
					rect.Height -= 20;
					rect.X += 10;
					rect.Y += 10;
					if(e.Owner != null && e.OwnerILayoutControl != null &&( e.OwnerILayoutControl.EnableCustomizationMode || e.OwnerILayoutControl.DesignMode))
						using(AppearanceObject app = new AppearanceObject()) {
							app.ForeColor = Color.Black;
							app.DrawString(e.Cache, LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.EmptyTabbedGroupText), rect);
						}
				}
			}
		}
		protected override void DrawBackground(BaseLayoutItemViewInfo e) {
			ObjectPainter.DrawObject(e.Cache, GetBorderPainter(e), e.BorderInfo);
			DrawBorder(e);
		}
	}
}
