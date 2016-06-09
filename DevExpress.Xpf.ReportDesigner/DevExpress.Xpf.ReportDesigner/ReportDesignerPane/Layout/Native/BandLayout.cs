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
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Layout.Native {
	public sealed class BandLayout : BaseReportElementLayout {
		readonly BandFoundationTracker foundationTracker;
		readonly XRControlRootReportTracker rootReportTracker;
		readonly SubBandsCollectionTracker subBandsCollectionTracker;
		ReportWidthTracker widthTracker;
		Band band;
		public BandLayout(Band band) {
			this.band = band;
			foundationTracker = new BandFoundationTracker(band, RaiseRef1Changed);
			rootReportTracker = new XRControlRootReportTracker(band, OnXRObjectRootReportChanged);
			if(band.BandKind != BandKind.SubBand && band.BandKind != BandKind.TopMargin && band.BandKind != BandKind.BottomMargin) {
				subBandsCollectionTracker = new SubBandsCollectionTracker(band, RaiseRef2Changed);
			}
			band.HeightChanged += OnBandHeightChanged;
			OnXRObjectRootReportChanged();
		}
		void OnXRObjectRootReportChanged() {
			if(widthTracker != null)
				widthTracker.Dispose();
			widthTracker = rootReportTracker.RootReport == null ? null : new ReportWidthTracker(rootReportTracker.RootReport, RaiseParam3Changed);
			RaiseParam3Changed();
		}
		void OnBandHeightChanged(object sender, EventArgs e) {
			RaiseParam4Changed();
		}
		public override XRControl Ref1 { get { return foundationTracker.BandFoundation; } }
		public override XRControl Ref2 {
			get {
				return band.HasSubBands
					? band.SubBands[band.SubBands.Count - 1]
					: null;
			}
		}
		public override double Param1 {
			get { return 0.0; }
			set { }
		}
		public override double Param2 {
			get { return 0.0; }
			set {
				if(value != 0.0)
					throw new NotSupportedException();
			}
		}
		public override double Param3 {
			get { return widthTracker == null ? 0.0 : widthTracker.ReportWidth; }
			set { }
		}
		public override double Param4 {
			get { return BoundsConverter.ToDouble(band.HeightF, band.Dpi); }
			set { band.HeightF = BoundsConverter.ToFloat(value, band.Dpi); }
		}
	}
}
