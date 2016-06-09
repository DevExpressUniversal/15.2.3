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
using System.Windows;
using System.Windows.Input;
#if SILVERLIGHT
using DevExpress.Xpf.Core.Native;
using ManipulationInertiaStartingEventArgs = DevExpress.Xpf.Core.Native.SLManipulationInertiaStartingEventArgs;
using ManipulationDeltaEventArgs = DevExpress.Xpf.Core.Native.SLManipulationDeltaEventArgs;
using ManipulationCompletedEventArgs = DevExpress.Xpf.Core.Native.SLManipulationCompletedEventArgs;
#endif
namespace DevExpress.Xpf.WindowsUI {
	class ManipulationHelper {
		private bool _IsManipulationInertiaEnabled;
		ISupportManipulation Owner;
#if SILVERLIGHT
		SLManipulationHelper helper;
#endif
		public ManipulationHelper(ISupportManipulation owner) {
			FrameworkElement element = owner as FrameworkElement;
			if(element == null) return;
			Owner = owner;
#if SILVERLIGHT
			helper = new SLManipulationHelper(element);
			helper.DisableInertia = true;
			helper.ManipulationInertiaStarting += OnManipulationInertiaStarting;
			helper.ManipulationDelta += OnManipulationDelta;
			helper.ManipulationCompleted += OnManipulationCompleted;
#else
			element.ManipulationInertiaStarting += OnManipulationInertiaStarting;
			element.ManipulationDelta += OnManipulationDelta;
			element.ManipulationCompleted += OnManipulationCompleted;
#endif
		}
		void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e) {
			Owner.OnManipulationCompleted(e);
			e.Handled = true;
		}
		void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e) {
			Owner.OnManipulationDelta(e);
			e.Handled = true;
		}
		void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e) {
			if(IsManipulationInertiaEnabled)
				Owner.OnManipulationInertiaStarting(e);
			e.Handled = true;
		}
		public bool IsManipulationInertiaEnabled {
			get { return _IsManipulationInertiaEnabled; }
			set {
				if(_IsManipulationInertiaEnabled == value) return;
				_IsManipulationInertiaEnabled = value;
#if SILVERLIGHT
					if(helper != null) helper.DisableInertia = !value;
#endif
			}
		}
	}
}
