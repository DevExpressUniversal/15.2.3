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
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing.Design;
namespace DevExpress.XtraGrid.Views.BandedGrid {
	public class BandedViewAppearances : GridViewAppearances {
		public BandedViewAppearances(BaseView view) : base(view) { }
		AppearanceObject bandPanel, bandPanelBackground, headerPanelBackground;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.bandPanel = CreateAppearance("BandPanel");
			this.bandPanelBackground = CreateAppearance("BandPanelBackground");
			this.headerPanelBackground = CreateAppearance("HeaderPanelBackground");
		}
		void ResetBandPanel() { BandPanel.Reset(); }
		bool ShouldSerializeBandPanel() { return BandPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedViewAppearancesBandPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject BandPanel { get { return bandPanel; } }
		void ResetBandPanelBackground() { BandPanelBackground.Reset(); }
		bool ShouldSerializeBandPanelBackground() { return BandPanelBackground.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedViewAppearancesBandPanelBackground"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject BandPanelBackground { get { return bandPanelBackground; } }
		void ResetHeaderPanelBackground() { HeaderPanelBackground.Reset(); }
		bool ShouldSerializeHeaderPanelBackground() { return HeaderPanelBackground.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedViewAppearancesHeaderPanelBackground"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderPanelBackground { get { return headerPanelBackground; } }
	}
	public class BandedViewPrintAppearances : GridViewPrintAppearances {
		public BandedViewPrintAppearances(BaseView view) : base(view) { }
		AppearanceObject bandPanel;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.bandPanel = CreateAppearance("BandPanel");
		}
		void ResetBandPanel() { BandPanel.Reset(); }
		bool ShouldSerializeBandPanel() { return BandPanel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedViewPrintAppearancesBandPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject BandPanel { get { return bandPanel; } }
	}
}
