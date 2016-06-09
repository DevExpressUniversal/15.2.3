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
using System.Text;
namespace DevExpress.Web {
	public delegate void DockManagerEventHandler(object source, DockManagerEventArgs e);
	public delegate void DockManagerCancelEventHandler(object source, DockManagerCancelEventArgs e);
	public class DockManagerEventArgs : EventArgs {
		ASPxDockPanel panel = null;
		ASPxDockZone zone = null;
		public ASPxDockPanel Panel { get { return this.panel; } }
		public ASPxDockZone Zone { get { return this.zone; } }
		public DockManagerEventArgs(ASPxDockPanel panel, ASPxDockZone zone)
			: base() {
			this.panel = panel;
			this.zone = zone;
		}
	}
	public class DockManagerCancelEventArgs : DockManagerEventArgs {
		bool cancel = false;
		public bool Cancel {
			get { return this.cancel; }
			set { this.cancel = value; }
		}
		public DockManagerCancelEventArgs(ASPxDockPanel panel, ASPxDockZone zone)
			: base(panel, zone) {
		}
	}
}
