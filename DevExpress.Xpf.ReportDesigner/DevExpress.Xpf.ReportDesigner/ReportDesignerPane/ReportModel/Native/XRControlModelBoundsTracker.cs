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
using DevExpress.Utils;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel.Native {
	public static class XRControlModelBoundsTracker {
		sealed class XRControlModelPositionTracker<TXRControlModel> : IXRControlBoundsTracker<TXRControlModel> where TXRControlModel : XRControlModelBase {
			readonly IDisposable positionUnsubscriber;
			public XRControlModelPositionTracker(TXRControlModel model, Action onPositionChanged) {
				Guard.ArgumentNotNull(model, "model");
				this.model = model;
				positionUnsubscriber = model.SubscribePropertyChanged(x => x.Position, onPositionChanged);
			}
			public void Dispose() {
				positionUnsubscriber.Dispose();
			}
			readonly TXRControlModel model;
			public TXRControlModel Model { get { return model; } }
		}
		sealed class XRControlModelHeightTracker<TXRControlModel> : IXRControlBoundsTracker<TXRControlModel> where TXRControlModel : XRControlModelBase {
			readonly IDisposable heightUnsubscriber;
			public XRControlModelHeightTracker(TXRControlModel model, Action onBoundsChanged) {
				Guard.ArgumentNotNull(model, "model");
				this.model = model;
				heightUnsubscriber = model.SubscribePropertyChanged(x => x.Height, onBoundsChanged);
			}
			public void Dispose() {
				heightUnsubscriber.Dispose();
			}
			readonly TXRControlModel model;
			public TXRControlModel Model { get { return model; } }
		}
		sealed class XRControlModelUsableHeightTracker<TXRControlModel> : IXRControlBoundsTracker<TXRControlModel> where TXRControlModel : BandModelBase {
			readonly IDisposable heightUnsubscriber;
			public XRControlModelUsableHeightTracker(TXRControlModel model, Action onBoundsChanged) {
				Guard.ArgumentNotNull(model, "model");
				this.model = model;
				heightUnsubscriber = model.SubscribePropertyChanged(x => x.UsableHeight, onBoundsChanged);
			}
			public void Dispose() {
				heightUnsubscriber.Dispose();
			}
			readonly TXRControlModel model;
			public TXRControlModel Model { get { return model; } }
		}
		sealed class XRControlModelCollapsedTracker<TXRControlModel> : IXRControlBoundsTracker<TXRControlModel> where TXRControlModel : BandModelBase {
			readonly IDisposable unsubscriber;
			public XRControlModelCollapsedTracker(TXRControlModel model, Action onBoundsChanged) {
				Guard.ArgumentNotNull(model, "model");
				this.model = model;
				unsubscriber = model.SubscribePropertyChanged(x => x.Collapsed, onBoundsChanged);
			}
			public void Dispose() {
				unsubscriber.Dispose();
			}
			readonly TXRControlModel model;
			public TXRControlModel Model { get { return model; } }
		}
		sealed class XRControlModelComplexTracker<TXRControlModel> : IXRControlBoundsTracker<TXRControlModel> where TXRControlModel : XRControlModelBase {
			readonly IXRControlBoundsTracker<TXRControlModel>[] trackers;
			public XRControlModelComplexTracker(TXRControlModel model, params IXRControlBoundsTracker<TXRControlModel>[] trackers) {
				Guard.ArgumentNotNull(model, "model");
				this.model = model;
				this.trackers = trackers;
			}
			public void Dispose() {
				foreach(var tracker in trackers)
					tracker.Dispose();
			}
			readonly TXRControlModel model;
			public TXRControlModel Model { get { return model; } }
		}
		sealed class XRControlModelBottomTracker<TXRControlModel> : IXRControlBoundsTracker<TXRControlModel> where TXRControlModel : XRControlModelBase {
			readonly IDisposable bottomUnsubscriber;
			public XRControlModelBottomTracker(TXRControlModel model, Action onBoundsChanged) {
				Guard.ArgumentNotNull(model, "model");
				this.model = model;
				bottomUnsubscriber = model.SubscribePropertyChanged(x => x.Bottom, onBoundsChanged);
			}
			public void Dispose() {
				bottomUnsubscriber.Dispose();
			}
			readonly TXRControlModel model;
			public TXRControlModel Model { get { return model; } }
		}
		sealed class XRControlModelTopTracker<TXRControlModel> : IXRControlBoundsTracker<TXRControlModel> where TXRControlModel : XRControlModelBase {
			readonly IDisposable topUnsubscriber;
			public XRControlModelTopTracker(TXRControlModel model, Action onBoundsChanged) {
				Guard.ArgumentNotNull(model, "model");
				this.model = model;
				topUnsubscriber = model.SubscribePropertyChanged(x => x.Top, onBoundsChanged);
			}
			public void Dispose() {
				topUnsubscriber.Dispose();
			}
			readonly TXRControlModel model;
			public TXRControlModel Model { get { return model; } }
		}
		sealed class XRControlModelTopAbsoluteTracker<TXRControlModel> : IXRControlBoundsTracker<TXRControlModel> where TXRControlModel : XRControlModelBase {
			readonly IDisposable topUnsubscriber;
			public XRControlModelTopAbsoluteTracker(TXRControlModel model, Action onBoundsChanged) {
				Guard.ArgumentNotNull(model, "model");
				this.model = model;
				topUnsubscriber = model.SubscribePropertyChanged(x => x.TopAbsolute, onBoundsChanged);
			}
			public void Dispose() {
				topUnsubscriber.Dispose();
			}
			readonly TXRControlModel model;
			public TXRControlModel Model { get { return model; } }
		}
		sealed class XRControlModelBottomAbsoluteTracker<TXRControlModel> : IXRControlBoundsTracker<TXRControlModel> where TXRControlModel : XRControlModelBase {
			readonly IDisposable bottomUnsubscriber;
			public XRControlModelBottomAbsoluteTracker(TXRControlModel model, Action onBoundsChanged) {
				Guard.ArgumentNotNull(model, "model");
				this.model = model;
				bottomUnsubscriber = model.SubscribePropertyChanged(x => x.BottomAbsolute, onBoundsChanged);
			}
			public void Dispose() {
				bottomUnsubscriber.Dispose();
			}
			readonly TXRControlModel model;
			public TXRControlModel Model { get { return model; } }
		}
		public static IXRControlBoundsTracker<TXRControlModel> NewPositionTracker<TXRControlModel>(TXRControlModel model, Action onPositionChanged) where TXRControlModel : XRControlModelBase {
			return new XRControlModelPositionTracker<TXRControlModel>(model, onPositionChanged);
		}
		public static IXRControlBoundsTracker<TXRControlModel> NewHeightTracker<TXRControlModel>(TXRControlModel model, Action onHeightChanged) where TXRControlModel : XRControlModelBase {
			return new XRControlModelHeightTracker<TXRControlModel>(model, onHeightChanged);
		}
		public static IXRControlBoundsTracker<TXRControlModel> NewUsableHeightTracker<TXRControlModel>(TXRControlModel model, Action onHeightChanged) where TXRControlModel : BandModelBase {
			return new XRControlModelUsableHeightTracker<TXRControlModel>(model, onHeightChanged);
		}
		public static IXRControlBoundsTracker<TXRControlModel> NewCollapsedTracker<TXRControlModel>(TXRControlModel model, Action onCollapsedChanged) where TXRControlModel : BandModelBase {
			return new XRControlModelCollapsedTracker<TXRControlModel>(model, onCollapsedChanged);
		}
		public static IXRControlBoundsTracker<TXRControlModel> NewTopTracker<TXRControlModel>(TXRControlModel model, Action onTopChanged) where TXRControlModel : XRControlModelBase {
			return new XRControlModelTopTracker<TXRControlModel>(model, onTopChanged);
		}
		public static IXRControlBoundsTracker<TXRControlModel> NewBottomTracker<TXRControlModel>(TXRControlModel model, Action onBottomChanged) where TXRControlModel : XRControlModelBase {
			return new XRControlModelBottomTracker<TXRControlModel>(model, onBottomChanged);
		}
		public static IXRControlBoundsTracker<TXRControlModel> NewComplexTracker<TXRControlModel>(TXRControlModel model, params IXRControlBoundsTracker<TXRControlModel>[] trackers) where TXRControlModel : XRControlModelBase {
			return new XRControlModelComplexTracker<TXRControlModel>(model, trackers);
		}
		public static IXRControlBoundsTracker<TXRControlModel> NewPositionAndHeightTracker<TXRControlModel>(TXRControlModel model, Action onPositionOrHeightChanged) where TXRControlModel : XRControlModelBase {
			return NewComplexTracker(model, NewPositionTracker(model, onPositionOrHeightChanged), NewHeightTracker(model, onPositionOrHeightChanged));
		}
		public static IXRControlBoundsTracker<TXRControlModel> NewTopAndBottomTracker<TXRControlModel>(TXRControlModel model, Action onTopOrBottomChanged) where TXRControlModel : XRControlModelBase {
			return NewComplexTracker(model, NewTopTracker(model, onTopOrBottomChanged), NewBottomTracker(model, onTopOrBottomChanged));
		}
		public static IXRControlBoundsTracker<TXRControlModel> NewTopAbsoluteTracker<TXRControlModel>(TXRControlModel model, Action onTopChanged) where TXRControlModel : XRControlModelBase {
			return new XRControlModelTopAbsoluteTracker<TXRControlModel>(model, onTopChanged);
		}
		public static IXRControlBoundsTracker<TXRControlModel> NewBottomAbsoluteTracker<TXRControlModel>(TXRControlModel model, Action onBottomChanged) where TXRControlModel : XRControlModelBase {
			return new XRControlModelBottomAbsoluteTracker<TXRControlModel>(model, onBottomChanged);
		}
		public static IXRControlBoundsTracker<TXRControlModel> NewTopAndBottomAbsoluteTracker<TXRControlModel>(TXRControlModel model, Action onTopOrBottomChanged) where TXRControlModel : XRControlModelBase {
			return NewComplexTracker(model, NewTopAbsoluteTracker(model, onTopOrBottomChanged), NewBottomAbsoluteTracker(model, onTopOrBottomChanged));
		}
	}
}
