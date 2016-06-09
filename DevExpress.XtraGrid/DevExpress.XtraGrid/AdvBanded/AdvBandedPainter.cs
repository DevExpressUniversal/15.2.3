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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid.Views.BandedGrid.Drawing;
namespace DevExpress.XtraGrid.Views.BandedGrid.Drawing {
	public class AdvBandedGridPainter : BandedGridPainter {
		public AdvBandedGridPainter(AdvBandedGridView gridView) : base(gridView) {
		}
		public new AdvBandedGridView View { get { return base.View as AdvBandedGridView; } }
		public new AdvBandedGridElementsPainter ElementsPainter { get { return base.ElementsPainter as AdvBandedGridElementsPainter; } }
	}
	public class AdvBandedGridElementsPainter : BandedGridElementsPainter {
		public AdvBandedGridElementsPainter(BaseView view) : base(view) {
		}
	}
	public class AdvBandedGridStyle3DElementsPainter : AdvBandedGridElementsPainter {
		public AdvBandedGridStyle3DElementsPainter(BaseView view) : base(view) {
		}
		protected override ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.Style3D; } }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new Style3DIndicatorObjectPainter(); }
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new GridFilterButtonPainter(EditorButtonHelper.GetPainter(BorderStyles.Style3D)); } 
		protected override FooterCellPainter CreateGroupFooterCellPainter() { return new GridGroupFooterCellPainter(new Border3DSunkenPainter()); }
		protected override FooterPanelPainter CreateGroupFooterPanelPainter() { return new GridGroupFooterPanelPainter(new Style3DButtonObjectPainter()); }
	}
	public class AdvBandedGridUltraFlatElementsPainter : AdvBandedGridElementsPainter {
		public AdvBandedGridUltraFlatElementsPainter(BaseView view) : base(view) {
		}
		protected override ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.UltraFlat; } }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new UltraFlatIndicatorObjectPainter(); }
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new GridFilterButtonPainter(EditorButtonHelper.GetPainter(BorderStyles.UltraFlat)); } 
		protected override FooterCellPainter CreateGroupFooterCellPainter() { return new GridGroupFooterCellPainter(new SimpleBorderPainter()); }
		protected override FooterPanelPainter CreateGroupFooterPanelPainter() { return new GridGroupFooterPanelPainter(new GridUltraFlatButtonPainter()); }
		protected override ObjectPainter CreateSpecialTopRowPainter() { return new GridSpecialTopRowIndentUltraFlatPainter(); }
	}
	public class AdvBandedGridWindowsXPElementsPainter : AdvBandedGridElementsPainter {
		public AdvBandedGridWindowsXPElementsPainter(BaseView view) : base(view) {
		}
		protected override ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.WindowsXP; } }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new XPIndicatorObjectPainter(); }
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new GridFilterButtonPainter(new WindowsXPEditorButtonPainter()); } 
		protected override FooterCellPainter CreateGroupFooterCellPainter() { return new GridGroupFooterCellPainter(new TextFlatBorderPainter()); }
		protected override FooterPanelPainter CreateGroupFooterPanelPainter() { return new GridGroupFooterPanelPainter(new GridWindowsXPButtonPainter()); }
	}
	public class AdvBandedGridMixedXPElementsPainter : AdvBandedGridElementsPainter {
		public AdvBandedGridMixedXPElementsPainter(BaseView view) : base(view) { }
		protected override GridFilterPanelPainter CreateFilterPanelPainter() { return new GridFilterPanelPainter(new WindowsXPEditorButtonPainter(), new WindowsXPCheckObjectPainter()); }
	}
	public class AdvBandedGridOffice2003ElementsPainter : AdvBandedGridElementsPainter {
		public AdvBandedGridOffice2003ElementsPainter(BaseView view) : base(view) {
		}
		protected override ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.Office2003; } }
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new Office2003GridFilterButtonPainter(); } 
		protected override ObjectPainter CreateHeaderSmartFilterButtonPainter() { return new GridSmartOffice2003FilterButtonPainter();	}
		protected override FooterCellPainter CreateGroupFooterCellPainter() { return new Office2003FooterCellPainter(); }
		protected override FooterPanelPainter CreateGroupFooterPanelPainter() { return new GridGroupFooterPanelPainter(new Office2003FooterPanelObjectPainter()); }
		protected override GridFilterPanelPainter CreateFilterPanelPainter() { return new Office2003GridFilterPanelPainter(); }
		protected override GridGroupPanelPainter CreateGroupPanelPainter() { return new Office2003GridGroupPanelPainter(); }
		protected override GridGroupRowPainter CreateGroupRowPainter() { return new Office2003GridGroupRowPainter(this); }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new Office2003IndicatorObjectPainter(); }
		protected override ObjectPainter CreateSpecialTopRowPainter() { return new GridTopNewItemRowIndentOffice2003Painter(); }
	}
}
