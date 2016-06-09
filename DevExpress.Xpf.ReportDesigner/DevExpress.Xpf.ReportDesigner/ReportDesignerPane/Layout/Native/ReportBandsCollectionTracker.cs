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
using System.Windows;
using DevExpress.XtraReports.UI;
using DevExpress.Mvvm.Native;
using DevExpress.Data.Utils;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Xpf.Reports.UserDesigner.Layout.Native {
	public sealed class ReportBandsCollectionTracker : IDisposable {
		XtraReportBase report;
		readonly Action onCollectionChanged;
		readonly XRControlCollectionChangedWeakEventHandler<ReportBandsCollectionTracker> onBandsCollectionChanged;
		Dictionary<GroupBand, XRControlPropertyChangedWeakEventHandler<ReportBandsCollectionTracker>> bandPropertyChangedHandlers = 
			new Dictionary<GroupBand, XRControlPropertyChangedWeakEventHandler<ReportBandsCollectionTracker>>();
		public ReportBandsCollectionTracker(XtraReportBase report, Action onCollectionChanged) {
			this.report = report;
			onBandsCollectionChanged = new XRControlCollectionChangedWeakEventHandler<ReportBandsCollectionTracker>(this, (tracker, sender, e) => tracker.OnBandsCollectionChanged(sender, e));
			report.Bands.CollectionChanged += onBandsCollectionChanged.Handler;
			SubscribeToBandPropertiesChanged();
			this.onCollectionChanged = onCollectionChanged;
		}
		void SubscribeToBandPropertiesChanged() {
			bandPropertyChangedHandlers.Clear();
			var groupHeaders = report.Bands.OfType<GroupBand>().ToArray();
			IObjectTracker bandTracker;
			groupHeaders.ForEach(x => {
				Tracker.GetTracker(x, out bandTracker);
				bandPropertyChangedHandlers[x] = new XRControlPropertyChangedWeakEventHandler<ReportBandsCollectionTracker>(this, (tracker, sender, e) => tracker.OnBandPropertyChanged(sender, e));
				bandTracker.ObjectPropertyChanged += bandPropertyChangedHandlers[x].Handler;
			});
		}
		void UnsubscribeFromBandPropertiesChanged() {
			bandPropertyChangedHandlers.ForEach(x => {
				IObjectTracker tracker;
				Tracker.GetTracker(x.Key, out tracker);
				tracker.ObjectPropertyChanged -= x.Value.Handler;
			});
		}
		void OnBandPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "Level" && onCollectionChanged != null)
				onCollectionChanged();
		}
		public void Dispose() {
			report.Bands.CollectionChanged -= onBandsCollectionChanged.Handler;
			bandPropertyChangedHandlers.ForEach(x => {
				IObjectTracker tracker;
				Tracker.GetTracker(x.Key, out tracker);
				tracker.ObjectPropertyChanged -= x.Value.Handler;
			});
			report = null;
		}
		void OnBandsCollectionChanged(object sender, CollectionChangeEventArgs e) {
			UnsubscribeFromBandPropertiesChanged();
			if (onCollectionChanged != null)
				onCollectionChanged();
			SubscribeToBandPropertiesChanged();
		}
		void OnBandHeightChanged(object sender, EventArgs e) {
			if(onCollectionChanged != null)
				onCollectionChanged();
		}
	}
}
