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
using System.ComponentModel;
using DevExpress.Data;
using System.Windows.Threading;
using DevExpress.Xpf.Core.Native;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Data.Utils;
using DevExpress.Data.Helpers;
#if SL
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Xpf.Core {
	public abstract class DXGridDataController : GridDataController {
		static Action<DXGridDataController, object, ListChangedEventArgs> onChanged = (owner, o, e) => owner.OnListChanged(o, e);
		ListChangedWeakEventHandler<DXGridDataController> listChangedHandler;
		ListChangedWeakEventHandler<DXGridDataController> ListChangedHandler {
			get {
				if(listChangedHandler == null) {
					listChangedHandler = new ListChangedWeakEventHandler<DXGridDataController>(this, onChanged);
				}
				return listChangedHandler;
			}
		}
		protected override bool UseFirstRowTypeWhenPopulatingColumns(Type rowType) {
			return rowType.FullName == ListDataControllerHelper.UseFirstRowTypeWhenPopulatingColumnsTypeName;
		}
		protected override void SubscribeListChanged(Data.Helpers.INotificationProvider provider, object list) {
			if(list is IBindingList)
				(list as IBindingList).ListChanged += ListChangedHandler.Handler;
		}
		protected override void UnsubscribeListChanged(Data.Helpers.INotificationProvider provider, object list) {
			if(list is IBindingList)
				(list as IBindingList).ListChanged -= ListChangedHandler.Handler;
		}
#if !SL //TODO SL
		static bool _DisableThreadingProblemsDetection = false;
		[Obsolete("Threading problems detection disabled")]
		public static bool DisableThreadingProblemsDetection {
			get { return _DisableThreadingProblemsDetection; }
			set { _DisableThreadingProblemsDetection = value; }
		}
		static void ThrowCrossThreadException() {
			throw new InvalidOperationException("Cross thread operation detected. To suppress this exception, set DevExpress.Xpf.Core.DXGridDataController.DisableThreadingProblemsDetection = true");
		}
		protected abstract Dispatcher Dispatcher { get; }
#endif
		#region IWeakEventListener Members
		void OnListChanged(object sender, ListChangedEventArgs e) {
#if !SL
			if(Dispatcher == null || Dispatcher.CheckAccess()) {
#endif
				OnBindingListChanged(sender, e);
#if !SL
			} else {
#pragma warning disable 0618
				if(DisableThreadingProblemsDetection)
#pragma warning restore 0618
					Dispatcher.BeginInvoke(new Action(() => OnBindingListChanged(sender, e)));
				else {
					Dispatcher.BeginInvoke(new Action(ThrowCrossThreadException));
					ThrowCrossThreadException();
				}
			}
#endif
		}
		#endregion
	}
}
