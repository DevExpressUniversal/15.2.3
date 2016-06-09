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
using DevExpress.Data.Utils;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions {
	public class XRControlInvalidater {
		readonly static WrappersManager<XRControl, XRControlInvalidater> invalidaters = new WrappersManager<XRControl, XRControlInvalidater>();
		public static void InvalidateStartBandEndBand(XRControl xrControl) {
			XRControlInvalidater invalidater;
			GetInvalidater(xrControl, out invalidater);
			invalidater.InvalidateStartBandEndBand();
		}
		public static void GetInvalidater(XRControl xrControl, out XRControlInvalidater invalidater) {
			invalidater = invalidaters.Wrap(xrControl, _ => new XRControlInvalidater());
		}
		public void InvalidateParent() {
			if(ParentInvalidated != null)
				ParentInvalidated(this, EventArgs.Empty);
		}
		public EventHandler ParentInvalidated;
		public void InvalidateStartBandEndBand() {
			if(StartBandEndBandInvalidated != null)
				StartBandEndBandInvalidated(this, EventArgs.Empty);
		}
		public EventHandler StartBandEndBandInvalidated;
		public void InvalidateDataBindings() {
			if(DataBindingsInvalidated != null)
				DataBindingsInvalidated(this, EventArgs.Empty);
		}
		public EventHandler DataBindingsInvalidated;
	}
	public class XRControlParentInvalidatedWeakEventHandler<T> : WeakEventHandler<T, EventArgs, EventHandler> where T : class {
		static Action<WeakEventHandler<T, EventArgs, EventHandler>, object> onDetachAction = (h, o) => ((XRControlInvalidater)o).ParentInvalidated -= h.Handler;
		static Func<WeakEventHandler<T, EventArgs, EventHandler>, EventHandler> createHandlerFunction = h => h.OnEvent;
		public XRControlParentInvalidatedWeakEventHandler(T owner, Action<T, object, EventArgs> onEventAction)
			: base(owner, onEventAction, onDetachAction, createHandlerFunction) {
		}
	}
	public class XRControlStartBandEndBandInvalidatedWeakEventHandler<T> : WeakEventHandler<T, EventArgs, EventHandler> where T : class {
		static Action<WeakEventHandler<T, EventArgs, EventHandler>, object> onDetachAction = (h, o) => ((XRControlInvalidater)o).StartBandEndBandInvalidated -= h.Handler;
		static Func<WeakEventHandler<T, EventArgs, EventHandler>, EventHandler> createHandlerFunction = h => h.OnEvent;
		public XRControlStartBandEndBandInvalidatedWeakEventHandler(T owner, Action<T, object, EventArgs> onEventAction)
			: base(owner, onEventAction, onDetachAction, createHandlerFunction) {
		}
	}
}
