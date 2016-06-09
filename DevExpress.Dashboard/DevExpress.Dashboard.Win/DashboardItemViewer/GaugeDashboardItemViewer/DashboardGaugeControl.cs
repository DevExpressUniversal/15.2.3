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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.Skins;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Win;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class DashboardGaugeControl : GaugeControl, IDashboardGaugeControl, IMouseWheelSupport {
		bool IDashboardGaugeControl.IsDarkBackground {
			get {
				SkinElement element = DashboardSkins.GetSkin(LookAndFeel)[DashboardSkins.SkinGauge];
				if(element != null)
					return element.Properties.GetBoolean("IsBlack", false);
				return false;
			}
		}
		Color IDashboardGaugeControl.DashboardGaugeForeColor {
			get {
				return SkinManager.Default.GetSkin(SkinProductId.Common, LookAndFeel).Colors.GetColor("WindowText", Color.Empty);
			}
		}
		Color IDashboardGaugeControl.DashboardGaugeBackColor {
			get {
				return SkinManager.Default.GetSkin(SkinProductId.Common, LookAndFeel).Colors.GetColor("Window", Color.Empty);
			}
		}
		IEnumerable<IGauge> IDashboardGaugeControl.Gauges { get { return Gauges; } }
		int IDashboardGaugeControl.GaugesCount { get { return Gauges.Count; } }
		public event EventHandler<PaintEventArgs> Painted;
		public IPrintable PrintableCore { get { return GetPrintableCore(); } }
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(Painted != null)
				Painted(this, e);
		}
		void IDashboardGaugeControl.ClearBorder() {
			BorderStyle = BorderStyles.NoBorder;
		}
		void IDashboardGaugeControl.ClearLayout() {
			AutoLayout = false;
			LayoutInterval = 0;
			LayoutPadding = new Thickness(0);
		}
		void IDashboardGaugeControl.ClearGauges() {
			Gauges.Clear();
		}
		Skin IDashboardGaugeControl.GetSkin() {
			return DashboardSkins.GetSkin(LookAndFeel);
		}
		void IDashboardGaugeControl.AddGauge(IGauge gauge, GaugeModel model) {
			if(!Gauges.Contains(gauge))
				Gauges.Add(gauge);
		}
		void IDashboardGaugeControl.RemoveGauge(IGauge gauge) {
			Gauges.Remove(gauge);
		}
		void IDashboardGaugeControl.SetGaugeBounds(IGauge gauge, Rectangle gaugeBounds, Rectangle containerBounds) {
			gauge.Bounds = gaugeBounds;
		}
		protected sealed override void OnMouseWheel(MouseEventArgs e) {
			if(XtraForm.ProcessSmartMouseWheel(this, e)) return;
			OnMouseWheelCore(e);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		void OnMouseWheelCore(MouseEventArgs e) {
			if(!ControlHelper.IsHMouseWheel(e))
				base.OnMouseWheel(e);
		}
	}
}
