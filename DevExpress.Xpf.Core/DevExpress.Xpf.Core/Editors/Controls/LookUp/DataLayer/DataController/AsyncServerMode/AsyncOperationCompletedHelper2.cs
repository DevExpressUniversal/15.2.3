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
using System.Collections;
using DevExpress.Data;
using DevExpress.Data.Async;
namespace DevExpress.Xpf.Editors.Helpers {
	public class AsyncOperationCompletedHelper2 {
		OperationCompleted completed;
		AsyncOperationCompletedHelper2(OperationCompleted completed) {
			this.completed = completed;
		}
		static readonly object Token = new object();
		public static DictionaryEntry GetCommandParameter(OperationCompleted completed) {
			return new DictionaryEntry(Token, new AsyncOperationCompletedHelper2(completed));
		}
		public static DictionaryEntry GetCommandParameter(OperationCompleted[] completed) {
			return GetCommandParameter(Combine(completed));
		}
		static OperationCompleted Combine(params OperationCompleted[] delegates) {
			if (delegates == null || delegates.Length == 0) {
				return null;
			}
			Delegate rv = delegates[0];
			for (int i = 1; i < delegates.Length; i++) {
				rv = Delegate.Combine(rv, delegates[i]);
			}
			return (OperationCompleted)rv;
		}
		public static OperationCompleted GetCompletedDelegate(Command command) {
			AsyncOperationCompletedHelper2 helper;
			if (command.TryGetTag(Token, out helper)) {
				return helper.completed;
			}
			else {
				return null;
			}
		}
		public static void AppendCompletedDelegate(Command command, OperationCompleted next) {
			if (next == null)
				return;
			AsyncOperationCompletedHelper2 helper;
			if (command.TryGetTag(Token, out helper)) {
				helper.completed = (OperationCompleted)Delegate.Combine(helper.completed, next);
			}
			else {
				throw new InvalidOperationException(string.Format("'{0}' command did not have AsyncCompletedHelper tag to append next continuation", command));
			}
		}
	}
}
