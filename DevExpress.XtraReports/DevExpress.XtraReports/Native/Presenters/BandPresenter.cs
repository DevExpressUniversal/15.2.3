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
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using DevExpress.XtraPrinting;
using System.Drawing;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.Native.Presenters {
	public interface IBandLogic {
		float GetMaxControlBottom(Band band, bool ignoreAnchoring);
	}
	class BandLogic : IBandLogic {
		public virtual float GetMaxControlBottom(Band band, bool ignoreAnchoring) {
			float bottom = 0;
			foreach(XRControl ctl in band.Controls) {
				if(ctl.BoundsChanging || ignoreAnchoring)
					bottom = Math.Max(bottom, ctl.BottomF);
				else if(ctl.AnchorVertical == VerticalAnchorStyles.Top || ctl.AnchorVertical == VerticalAnchorStyles.None)
					bottom = Math.Max(bottom, ctl.BottomF);
				else if(ctl.AnchorVertical == VerticalAnchorStyles.Bottom)
					bottom = Math.Max(bottom, ctl.HeightF);
				else if(ctl.AnchorVertical == VerticalAnchorStyles.Both)
					bottom = Math.Max(bottom, ctl.TopF);
			}
			foreach(XRCrossBandControl cbControl in band.RootReport.CrossBandControls) {
				bottom = Math.Max(bottom, cbControl.GetMinimumBottom(band));
			}
			return bottom;
		}
	}
	public abstract class BandPresenter {
		protected XtraReport report;
		protected BandPresenter(XtraReport report) {
			this.report = report;
		}
		public abstract ArrayList GetPrintableControls(Band band);
	}
	class DesignBandPresenter : BandPresenter {
		public DesignBandPresenter(XtraReport report) : base(report) { 
		}
		public override ArrayList GetPrintableControls(Band band) {
			ArrayList srcControls = new ArrayList(band.Controls);
			srcControls.Reverse();
			srcControls.AddRange(report.CrossBandControls.GetPrintableControls(band));
			return srcControls;
		}
	}
	class RuntimeBandPresenter : BandPresenter {
		public RuntimeBandPresenter(XtraReport report)
			: base(report) {
		}
		public override ArrayList GetPrintableControls(Band band) {
			ArrayList srcControls = new ArrayList(band.Controls);
			ArrayList strongControls = new ArrayList();
			ArrayList destControls = new ArrayList();
			for(int i = srcControls.Count - 1; i >= 0; i--) {
				XRControl control = (XRControl)srcControls[i];
				if(control.HasUndefinedBounds) {
					srcControls.RemoveAt(i);
					strongControls.Add(control);
				}
			}
			strongControls.Sort(new DevExpress.XtraReports.Native.ControlTopComparer());
			foreach(XRControl control in strongControls) {
				ArrayList aboveControls = SelectAboveControls(srcControls, control.TopF);
				destControls.AddRange(aboveControls);
				destControls.Add(control);
			}
			srcControls.Reverse();
			destControls.AddRange(srcControls);
			System.Diagnostics.Debug.Assert(destControls.Count == band.Controls.Count);
			destControls.AddRange(report.CrossBandControls.GetPrintableControls(band));
			return destControls;
		}
		static ArrayList SelectAboveControls(ArrayList srcControls, float yPos) {
			ArrayList destControls = new ArrayList();
			for(int i = srcControls.Count - 1; i >= 0; i--) {
				XRControl control = (XRControl)srcControls[i];
				if(control.TopF < yPos) {
					srcControls.RemoveAt(i);
					destControls.Add(control);
				}
			}
			return destControls;
		}
	}
}
