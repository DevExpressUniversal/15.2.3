#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	internal class HandlerWrapper {
		private EventHandler method;
		public HandlerWrapper(EventHandler method) {
			this.method = method;
		}
		public EventHandler Handler {
			get {
				return new EventHandler(SafeExceute);
			}
		}
		private void SafeExceute(object obj, EventArgs e) {
			try {
				method.Invoke(obj, e);
			}
			catch(Exception ex) {
				if(HandleException != null) {
					HandleException(this, new CustomHandleExceptionEventArgs(ex));
				}
			}
		}
		public event EventHandler<CustomHandleExceptionEventArgs> HandleException;
	}
	internal class HandlerWrapper<A>
		where A : EventArgs {
		private EventHandler<A> method;
		public HandlerWrapper(EventHandler<A> method) {
			this.method = method;
		}
		public EventHandler<A> Handler {
			get {
				return new EventHandler<A>(SafeExceute);
			}
		}
		private void SafeExceute(object obj, A e) {
			try {
				method.Invoke(obj, e);
			}
			catch(Exception ex) {
				if(HandleException != null) {
					HandleException(this, new CustomHandleExceptionEventArgs(ex));
				}
			}
		}
		public event EventHandler<CustomHandleExceptionEventArgs> HandleException;
	}
}
