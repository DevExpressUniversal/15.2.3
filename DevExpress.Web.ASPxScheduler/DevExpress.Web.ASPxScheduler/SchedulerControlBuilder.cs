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
using System.Web;
using System.Web.UI;
using System.Web.Util;
using DevExpress.Web;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class SchedulerControlBuilder : ThemableControlBuilder {
		public override object BuildObject() {
			SchedulerControlCreationInterceptor interceptor = SchedulerControlCreationInterceptor.Instance;
			interceptor.ControlCreated += new ASPxSchedulerCreatedEventHandler(OnControlCreated);
			try {
				ASPxScheduler control = (ASPxScheduler)base.BuildObject();
				if (control != null)
					control.EndInit();
				return control;
			}
			finally {
				interceptor.ControlCreated -= new ASPxSchedulerCreatedEventHandler(OnControlCreated);
			}
		}
		protected internal virtual void OnControlCreated(object sender, ASPxSchedulerCreatedEventArgs e) {
			e.Control.BeginInit();
		}
	}
	#region SchedulerControlCreationInterceptor
	public sealed class SchedulerControlCreationInterceptor {
		[ThreadStatic()]
		static SchedulerControlCreationInterceptor instance;
		#region Events
		#region ControlCreated
		ASPxSchedulerCreatedEventHandler onControlCreated;
		public event ASPxSchedulerCreatedEventHandler ControlCreated { add { onControlCreated += value; } remove { onControlCreated -= value; } }
		public void RaiseControlCreated(ASPxScheduler control) {
			if (onControlCreated != null) {
				ASPxSchedulerCreatedEventArgs args = new ASPxSchedulerCreatedEventArgs(control);
				onControlCreated(this, args);
			}
		}
		#endregion
		#endregion
		public static SchedulerControlCreationInterceptor Instance {
			get {
				if (instance == null)
					instance = new SchedulerControlCreationInterceptor();
				return instance;
			}
		}
		internal static SchedulerControlCreationInterceptor InnerInstance { get { return instance; } set { instance = value; } }
	}
	#endregion
}
