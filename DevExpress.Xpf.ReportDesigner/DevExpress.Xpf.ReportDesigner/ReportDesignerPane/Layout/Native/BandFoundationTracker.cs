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
	public sealed class BandFoundationTracker : IDisposable {
		Band band;
		readonly XRControlParentTracker parentTracker;
		ReportBandsCollectionTracker parentBandsTracker;
		readonly Action onFoundationChanged;
		public BandFoundationTracker(Band band, Action onFoundationChanged) {
			this.band = band;
			parentTracker = new XRControlParentTracker(band, OnBandParentChanged);
			OnBandParentChanged(null);
			this.onFoundationChanged = onFoundationChanged;
		}
		public void Dispose() {
			parentTracker.Dispose();
			if(parentBandsTracker != null)
				parentBandsTracker.Dispose();
			band = null;
			bandFoundation = null;
		}
		void OnBandParentChanged(XRControl oldBandParent) {
			var report = (XtraReportBase)parentTracker.ControlParent;
			if(parentBandsTracker != null)
				parentBandsTracker.Dispose();
			parentBandsTracker = report == null ? null : new ReportBandsCollectionTracker(report, OnParentBandsCollectionChanged);
			OnParentBandsCollectionChanged();
		}
		void OnParentBandsCollectionChanged() {
			bandFoundation = null;
			bandFoundationSet = false;
			if(onFoundationChanged != null)
				onFoundationChanged();
		}
		bool bandFoundationSet;
		Band bandFoundation;
		public Band BandFoundation {
			get {
				if(!bandFoundationSet) {
					bandFoundation = GetBandFoundation(band);
					bandFoundationSet = true;
				}
				return bandFoundation;
			}
		}
		static Band GetBandFoundation(Band band) {
			if(band.Report == null) return null;
			var foundBandInBandsCollection = false;
			Band foundation = null;
			foreach(var xtraReportBand in band.Report.OrderedBands) {
				if(xtraReportBand == band) {
					foundBandInBandsCollection = true;
					break;
				}
				foundation = xtraReportBand;
			}
			return foundBandInBandsCollection
				? foundation
				: null;
		}
	}
}
