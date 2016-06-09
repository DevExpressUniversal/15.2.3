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
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Reports.UserDesigner.Layout;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel.Native.BandModel {
	class InnerBottomBandLogic {
		readonly BandModelBase owner;
		readonly BaseReportElementLayout ownerLayout;
		readonly Action onHeightChanged;
		readonly ResettableLazy<BandModelBase, IDisposable> lazyInnerBottomModel;
		double prevHeight;
		public InnerBottomBandLogic(BandModelBase owner, BaseReportElementLayout ownerLayout, Action onHeightChanged) {
			Guard.ArgumentNotNull(owner, "owner");
			Guard.ArgumentNotNull(ownerLayout, "ownerLayout");
			Guard.ArgumentNotNull(onHeightChanged, "onHeightChanged");
			this.owner = owner;
			this.ownerLayout = ownerLayout;
			this.onHeightChanged = onHeightChanged;
			lazyInnerBottomModel = ResettableLazy.New(
				() => {
					prevHeight = owner.Height;
					return ownerLayout.Ref2.With(x => (BandModelBase)owner.Report.Factory.GetModel(x));
				},
				innerBottomBand => {
					if(owner.Height != prevHeight)
						onHeightChanged();
					return (IDisposable)innerBottomBand.With(x => XRControlModelBoundsTracker.NewBottomTracker(x, onHeightChanged));
				},
				SafeDispose);
		}
		public BandModelBase BandModel {
			get { return lazyInnerBottomModel.Value; }
		}
		public void Reset() {
			lazyInnerBottomModel.Reset();
		}
		static void SafeDispose(IDisposable obj) {
			if(obj != null) {
				obj.Dispose();
			}
		}
	}
}
