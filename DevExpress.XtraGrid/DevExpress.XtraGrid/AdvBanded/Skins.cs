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
using DevExpress.Skins;
using DevExpress.Utils;
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
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.BandedGrid.Drawing;
namespace DevExpress.XtraGrid.Skins {
	public class AdvBandedGridSkinElementsPainter : AdvBandedGridElementsPainter {
		public AdvBandedGridSkinElementsPainter(BaseView view) : base(view) { }
		public override bool ShouldInflateRowOnInvalidate() { 
			return GridSkins.GetSkin(View).Properties.GetBoolean(GridSkins.OptHeaderRequireVertOffset);
		}
		public override object GetIndicatorImages(object imageCollection) { 
			SkinElement element = GridSkins.GetSkin(View)[GridSkins.SkinIndicatorImages];
			if(element != null && element.Image != null) return element.Image.GetImages();
			return imageCollection; 
		}
		protected override ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.Skin; } }
		protected override HeaderObjectPainter CreateColumnPainter() { return new SkinHeaderObjectPainter(View); }
		protected override HeaderObjectPainter CreateBandPainter() { return new SkinHeaderObjectPainter(View); }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new SkinIndicatorObjectPainter(View); }
		protected override ObjectPainter CreateOpenCloseButtonPainter() { return new SkinOpenCloseButtonObjectPainter(View); }
		protected override GridFilterPanelPainter CreateFilterPanelPainter() { return new SkinGridFilterPanelPainter(View); }
		protected override GridGroupPanelPainter CreateGroupPanelPainter() { return new SkinGridGroupPanelPainter(View); }
		protected override ObjectPainter CreateColumnSortedShapePainter() { return new SkinSortedShapeObjectPainter(View); }
		protected override ObjectPainter CreateDetailButtonPainter() { return new SkinGridDetailButtonPainter(View); }
		protected override ObjectPainter CreateViewCaptionPainter() { return new SkinGridViewCaptionPainter(View); }
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new SkinGridFilterButtonPainter(View); } 
		protected override ObjectPainter CreateHeaderSmartFilterButtonPainter() { return new GridSmartSkinFilterButtonPainter(View); }
		protected override FooterCellPainter CreateFooterCellPainter() { return new SkinFooterCellPainter(View); }
		protected override FooterPanelPainter CreateFooterPanelPainter() { return new SkinFooterPanelPainter(View); } 
		protected override FooterCellPainter CreateGroupFooterCellPainter() { return new SkinFooterCellPainter(View); }
		protected override FooterPanelPainter CreateGroupFooterPanelPainter() { return new SkinFooterPanelPainter(View); }
		protected override ObjectPainter CreateSpecialTopRowPainter() { return new SkinGridSpecialTopRowIndentPainter(View); }
	}
}
