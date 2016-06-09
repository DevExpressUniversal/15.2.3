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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.Utils;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Card.Drawing;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Grid.Drawing;
namespace DevExpress.XtraGrid.Skins {
	public class SkinCardElementsPainter : CardElementsPainter {
		public SkinCardElementsPainter(BaseView view) : base(view) { }
		public override bool AllowCustomButtonPainter { get { return false; } }
		protected override ObjectPainter CreateCardCaptionPainter() { return new SkinCardCaptionPainter(View); }
		protected override ObjectPainter CreateButtonsPainter() { return new SkinEditorButtonPainter(View); }
		protected override ObjectPainter CreateCardExpandButtonPainter() { return new SkinCardExpandButtonPainter(View); }
		protected override ObjectPainter CreateViewCaptionPainter() { return new SkinGridViewCaptionPainter(View); }
		protected override ObjectPainter CreateFilterPanelPainter() { return new SkinGridFilterPanelPainter(View); } 
		protected override ObjectPainter CreateCardPainter() { return new SkinCardObjectPainter(View); }
	}
	public class SkinCardExpandButtonPainter : SkinCustomPainter {
		public SkinCardExpandButtonPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			ExplorerBarOpenCloseButtonInfoArgs ee = (ExplorerBarOpenCloseButtonInfoArgs)e;
			SkinElementInfo info = new SkinElementInfo(GridSkins.GetSkin(Provider)[ee.Expanded ? GridSkins.SkinCardCloseButton : GridSkins.SkinCardOpenButton]);
			info.ImageIndex = -1;
			return info;
		}
	}
	public class SkinCardObjectPainter : CardObjectPainter {
		ISkinProvider provider;
		public SkinCardObjectPainter(ISkinProvider provider) { 
			this.provider = provider;
		}
		SkinElementInfo UpdateInfo(ObjectInfoArgs e) {
			CardObjectInfoArgs ee = (CardObjectInfoArgs)e;
			bool selected = (ee.Card.State & (GridRowCellState.Selected | GridRowCellState.Focused)) != 0;
			SkinElementInfo info = new SkinElementInfo(GridSkins.GetSkin(this.provider)[selected ? GridSkins.SkinCardSelected : GridSkins.SkinCard], e.Bounds);
			return info;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			CardObjectInfoArgs ee = (CardObjectInfoArgs)e;
			SkinElementInfo info = UpdateInfo(e);
			info.Bounds = ee.CardClientBounds;
			if(info.Bounds.Height < 1 || ee.ViewInfo.View.GetCardCollapsed(ee.Card.RowHandle)) return;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
	}
	public class SkinCardCaptionPainter : CardCaptionPainter {
		ISkinProvider provider;
		public SkinCardCaptionPainter(ISkinProvider provider) { 
			this.provider = provider;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { 
			return ObjectPainter.GetObjectClientRectangle(e.Graphics, SkinElementPainter.Default, UpdateInfo(e));
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { 
			return ObjectPainter.CalcBoundsByClientRectangle(e.Graphics, SkinElementPainter.Default, UpdateInfo(e), client);
		}
		SkinElementInfo UpdateInfo(ObjectInfoArgs e) {
			CardObjectInfoArgs ee = (CardObjectInfoArgs)e;
			bool selected = ((ee.Card.State & (GridRowCellState.Selected | GridRowCellState.Focused)) != 0);
			bool focused = ee.Card.ViewInfo.View.IsFocusedView;
			Skin skin = GridSkins.GetSkin(this.provider);
			SkinElement element = skin[GridSkins.SkinCardCaption];
			if(selected) {
				element = skin[GridSkins.SkinCardCaptionSelected];
				if(!focused) {
					element = skin[GridSkins.SkinCardCaptionHideSelection];
					if(element == null) element = skin[GridSkins.SkinCardCaption];
				}
			}
			SkinElementInfo info = new SkinElementInfo(element, e.Bounds);
			return info;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			if(e.Bounds.IsEmpty) return;
			Rectangle r = e.Bounds;
			r.Inflate(1, 0);
			SkinElementInfo info = UpdateInfo(e);
			info.Bounds = r;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
	}
}
