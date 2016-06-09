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
using System.Linq;
using System.Windows;
using DevExpress.XtraReports.UI;
using DevExpress.Mvvm.Native;
using DevExpress.Data.Utils;
namespace DevExpress.Xpf.Reports.UserDesigner.Layout.Native {
	public sealed class SubBandFoundationTracker : IDisposable {
		SubBand subBand;
		readonly XRControlParentTracker parentTracker;
		SubBandsCollectionTracker parentSubBandsTracker;
		readonly Action onFoundationChanged;
		public SubBandFoundationTracker(SubBand subBand, Action onFoundationChanged) {
			this.subBand = subBand;
			parentTracker = new XRControlParentTracker(subBand, OnSubBandParentChanged);
			OnSubBandParentChanged(null);
			this.onFoundationChanged = onFoundationChanged;
		}
		public void Dispose() {
			var band = (Band)parentTracker.ControlParent;
			parentTracker.Dispose();
			if(parentSubBandsTracker != null)
				parentSubBandsTracker.Dispose();
			subBand = null;
		}
		void OnSubBandParentChanged(XRControl oldBandParent) {
			var band = (Band)parentTracker.ControlParent;
			if(parentSubBandsTracker != null)
				parentSubBandsTracker.Dispose();
			parentSubBandsTracker = band == null ? null : new SubBandsCollectionTracker(band, OnParentSubBandsCollectionChanged);
			OnParentSubBandsCollectionChanged();
		}
		void OnParentSubBandsCollectionChanged() {
			subBandFoundation = null;
			subBandFoundationSet = false;
			if(onFoundationChanged != null)
				onFoundationChanged();
		}
		bool subBandFoundationSet;
		Band subBandFoundation;
		public Band SubBandFoundation {
			get {
				if(!subBandFoundationSet) {
					subBandFoundationSet = true;
					subBandFoundation = GetSubBandFoundation(subBand);
				}
				return subBandFoundation;
			}
		}
		static Band GetSubBandFoundation(SubBand subBand) {
			var parent = (Band)subBand.Parent;
			return parent.With(x => x.SubBands.ElementAtOrDefault(subBand.Index - 1))
				?? parent;
		}
	}
}
