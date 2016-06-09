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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.Core.Native {
	public class CanExecuteChangedWeakEventManager : WeakEventManager {
		static CanExecuteChangedWeakEventManager CurrentManager {
			get {
				CanExecuteChangedWeakEventManager currentManager = (CanExecuteChangedWeakEventManager)GetCurrentManager(typeof(CanExecuteChangedWeakEventManager));
				if(currentManager == null) {
					currentManager = new CanExecuteChangedWeakEventManager();
					SetCurrentManager(typeof(CanExecuteChangedWeakEventManager), currentManager);
				}
				return currentManager;
			}
		}
		public static void AddListener(ICommand source, IWeakEventListener listener) {
			CurrentManager.ProtectedAddListener(source, listener);
		}
		public static void RemoveListener(ICommand source, IWeakEventListener listener) {
			CurrentManager.ProtectedRemoveListener(source, listener);
		}
		CanExecuteChangedWeakEventManager() {
		}
		protected override void StartListening(object source) {
			((ICommand)source).CanExecuteChanged += new EventHandler(OnCanExecuteChanged);
		}
		void OnCanExecuteChanged(object sender, EventArgs e) {
			DeliverEvent(sender, e);			
		}
		protected override void StopListening(object source) {
			((ICommand)source).CanExecuteChanged -= new EventHandler(OnCanExecuteChanged);
		}
	}
}
