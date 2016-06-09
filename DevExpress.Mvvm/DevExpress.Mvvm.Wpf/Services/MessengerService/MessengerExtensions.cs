#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
namespace DevExpress.Mvvm {
	public static class MessengerExtensions {
		public static void Register<TMessage>(this IMessenger messenger, object recipient, Action<TMessage> action) {
			VerifyMessenger(messenger);
			messenger.Register(recipient, null, false, action);
		}
		public static void Register<TMessage>(this IMessenger messenger, object recipient, bool receiveInheritedMessagesToo, Action<TMessage> action) {
			VerifyMessenger(messenger);
			messenger.Register(recipient, null, receiveInheritedMessagesToo, action);
		}
		public static void Register<TMessage>(this IMessenger messenger, object recipient, object token, Action<TMessage> action) {
			VerifyMessenger(messenger);
			messenger.Register(recipient, token, false, action);
		}
		public static void Send<TMessage>(this IMessenger messenger, TMessage message) {
			VerifyMessenger(messenger);
			messenger.Send(message, null, null);
		}
		public static void Send<TMessage, TTarget>(this IMessenger messenger, TMessage message) {
			VerifyMessenger(messenger);
			messenger.Send(message, typeof(TTarget), null);
		}
		public static void Send<TMessage>(this IMessenger messenger, TMessage message, object token) {
			VerifyMessenger(messenger);
			messenger.Send(message, null, token);
		}
		public static void Unregister<TMessage>(this IMessenger messenger, object recipient) {
			VerifyMessenger(messenger);
			messenger.Unregister<TMessage>(recipient, null, null);
		}
		public static void Unregister<TMessage>(this IMessenger messenger, object recipient, object token) {
			VerifyMessenger(messenger);
			messenger.Unregister<TMessage>(recipient, token, null);
		}
		public static void Unregister<TMessage>(this IMessenger messenger, object recipient, Action<TMessage> action) {
			VerifyMessenger(messenger);
			messenger.Unregister(recipient, null, action);
		}
		static void VerifyMessenger(IMessenger messenger) {
			if(messenger == null)
				throw new ArgumentNullException("messenger");
		}
	}
}
