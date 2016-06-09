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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using System.Drawing;
using DevExpress.XtraReports.Native.Presenters;
namespace DevExpress.XtraReports.Native.LayoutView {
	class LayoutViewBandPresenter : RuntimeBandPresenter {
		public LayoutViewBandPresenter(XtraReport report)
			: base(report) {
		}
		public override ArrayList GetPrintableControls(Band band) {
			ArrayList controls = base.GetPrintableControls(band);
			MultiColumn mc;
			XRControl panel = Band.TryGetMultiColumn(band, out mc) ? new MultiColumnBandPanel(band, mc) :
				(XRControl)new BandPanel(band);
			controls.Reverse();
			XRControl[] childControls = (XRControl[])controls.ToArray(typeof(XRControl));
			panel.Controls.AddRange(childControls);
			return new ArrayList(new XRControl[] { panel });
		}
	}
	abstract class BandPanelBase : XRPanel {
		Band band;
		public override XtraReport RootReport { get { return band != null ? band.RootReport : null; } }
		public override Band Band { get { return band; } }
		internal override XRControl PrintableParent { get { return band != null ? band.PrintableParent : null; } }
		protected override BrickOwnerType BrickOwnerType { get { return ((IBrickOwner)RealControl).BrickOwnerType; } }
		protected override DevExpress.XtraPrinting.Native.ControlLayoutRules LayoutRules { get { return ((IBrickOwner)RealControl).LayoutRules; } }
		public BandPanelBase(Band band) {
			this.band = band;
			this.Name = band.Name;
			BackColor = Color.Transparent;
			Padding = PaddingInfo.Empty;
#if ShowBandBorders
			Borders = BorderSide.All;
			BorderColor = LayoutViewAppearance.ContourColor;
			BorderWidth = 1;
#else
			Borders = BorderSide.None;
#endif
		}
		internal override XRControl RealControl {
			get {
				return this.band;
			}
		}
		protected override XRControlCollection CreateChildControls() {
			return new FakedXRControlCollection(this);
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			RootReport.PerformAsLoad(delegate() {
				this.BoundsF = GetBounds(childrenBricks);
			});
			return base.CreateBrick(childrenBricks);
		}
		protected abstract RectangleF GetBounds(VisualBrick[] childrenBricks);
	}
	class BandPanel : BandPanelBase {
		public BandPanel(Band band) : base(band) {
		}
		protected override RectangleF GetBounds(VisualBrick[] childrenBricks) {
			RectangleF bounds = Band.GetClientRectangle();
			if(childrenBricks.Length > 0)
				bounds.Width = Math.Max(bounds.Width, CalcMaxRight(childrenBricks));
			return bounds;
		}
		float CalcMaxRight(VisualBrick[] childrenBricks) {
			float value = -1;
			foreach(VisualBrick brick in childrenBricks)
				value = Math.Max(value, brick.Rect.Right);
			return XRConvert.Convert(value, GraphicsDpi.Document, Band.Dpi);
		}
	}
	class MultiColumnBandPanel : BandPanelBase {
		MultiColumn multiColumn;
		public MultiColumnBandPanel(Band band, MultiColumn multiColumn)
			: base(band) {
			this.multiColumn = multiColumn;
		}
		protected override RectangleF GetBounds(VisualBrick[] childrenBricks) {
			RectangleF bounds = Band.GetClientRectangle();
			bounds.Width = multiColumn.GetUsefulColumnWidth(Band.ClientRectangleF.Width, Band.Dpi);
			return bounds;
		}
		protected override BrickOwnerType BrickOwnerType {
			get {
				return BrickOwnerType.Panel;
			}
		}
	}
	class FakedXRControlCollection : XRControlCollection {
		public FakedXRControlCollection(XRControl owner)
			: base(owner) {
		}
		protected override void OnClear() {
		}
		protected override void OnInsertComplete(int index, object value) {
		}
		protected override void OnRemoveComplete(int index, object value) {
		}
	}
}
