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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpf.Reports.UserDesigner.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Layout.Native {
	public sealed class SubBandsCollectionTracker : IDisposable {
		Band band;
		readonly Action onCollectionChanged;
		readonly XRControlCollectionChangedWeakEventHandler<SubBandsCollectionTracker> onSubBandsCollectionChanged;
		public SubBandsCollectionTracker(Band band, Action onCollectionChanged) {
			this.band = band;
			this.onSubBandsCollectionChanged = new XRControlCollectionChangedWeakEventHandler<SubBandsCollectionTracker>(this, (tracker, sender, e) => tracker.OnSubBandsCollectionChanged(sender, e));
			band.SubBands.CollectionChanged += this.onSubBandsCollectionChanged.Handler;
			this.onCollectionChanged = onCollectionChanged;
		}
		public void Dispose() {
			band.SubBands.CollectionChanged -= onSubBandsCollectionChanged.Handler;
			band = null;
		}
		void OnSubBandsCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(onCollectionChanged != null)
				onCollectionChanged();
		}
	}
}
