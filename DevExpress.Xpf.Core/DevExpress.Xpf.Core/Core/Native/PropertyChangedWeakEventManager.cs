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
namespace DevExpress.Xpf.Core.Native {
	public class PropertyChangedWeakEventManager : WeakEventManager {
		static PropertyChangedWeakEventManager CurrentManager {
			get {
				PropertyChangedWeakEventManager currentManager = (PropertyChangedWeakEventManager)GetCurrentManager(typeof(PropertyChangedWeakEventManager));
				if (currentManager == null) {
					currentManager = new PropertyChangedWeakEventManager();
					SetCurrentManager(typeof(PropertyChangedWeakEventManager), currentManager);
				}
				return currentManager;
			}
		}
		public static void AddListener(INotifyPropertyChanged source, IWeakEventListener listener) {
			CurrentManager.ProtectedAddListener(source, listener);
		}
		public static void RemoveListener(INotifyPropertyChanged source, IWeakEventListener listener) {
			CurrentManager.ProtectedRemoveListener(source, listener);
		}
		PropertyChangedWeakEventManager() {
		}
		void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
			DeliverEvent(sender, e);
		}
		protected override void StartListening(object source) {
			((INotifyPropertyChanged)source).PropertyChanged += OnPropertyChanged;
		}
		protected override void StopListening(object source) {
			((INotifyPropertyChanged)source).PropertyChanged -= OnPropertyChanged;
		}
	}
}
