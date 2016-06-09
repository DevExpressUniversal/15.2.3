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
namespace DevExpress.Utils.Extensions.Helpers {
	public static class SafeEventRaiseExtensions {
		public static void SafeRaise(this EventHandler eventHandler, object sender, EventArgs e) {
			if(eventHandler == null) return;
			eventHandler.Invoke(sender, e);
		}
		public static void SafeRaise(this EventHandler eventHandler, object sender) {
			if(eventHandler == null) return;
			eventHandler.Invoke(sender, EventArgs.Empty);
		}
		public static void SafeRaise(this PropertyChangedEventHandler eventHandler, object sender, PropertyChangedEventArgs e) {
			if(eventHandler == null) return;
			eventHandler.Invoke(sender, e);
		}
		public static void SafeRaise<T>(this EventHandler<T> eventHandler, object sender, T e) where T : EventArgs {
			if(eventHandler == null) return;
			eventHandler.Invoke(sender, e);
		}
		public static void SafeRaise<THandler, TArgs>(this WeakEventHandler<TArgs, THandler> eventHandler, object sender, TArgs e) where TArgs : EventArgs {
			if(eventHandler == null) return;
			eventHandler.Raise(sender, e);
		}
	}
}
