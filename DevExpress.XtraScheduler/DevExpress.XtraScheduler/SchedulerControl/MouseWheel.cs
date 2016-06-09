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
using System.Windows.Forms;
using DevExpress.Services;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraScheduler {
	public partial class SchedulerControl : IMouseWheelScrollClient, IMouseWheelSupport {
		MouseWheelScrollHelper mouseHelper;
		protected sealed override void OnMouseWheel(MouseEventArgs ev) {
			if (XtraForm.ProcessSmartMouseWheel(this, ev)) return;
			this.mouseHelper.OnMouseWheel(ev);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			this.mouseHelper.OnMouseWheel(e);
		}
		#region IMouseWheelScrollClient Members
		bool IMouseWheelScrollClient.PixelModeHorz { get { return false; } }
		bool IMouseWheelScrollClient.PixelModeVert { get { return false; } }
		void IMouseWheelScrollClient.OnMouseWheel(MouseWheelScrollClientArgs e) {
			int delta = (e.Horizontal) ? e.Distance : e.Distance;
			OfficeMouseWheelEventArgs ea = new OfficeMouseWheelEventArgs(e.Button, e.Clicks, e.X, e.Y, -delta);
			ea.IsHorizontal = e.Horizontal;
			OnMouseWheelCore(ea);
		}		
		#endregion
		protected internal void OnMouseWheelCore(MouseEventArgs e) {
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null)
				svc.OnMouseWheel(e);
			base.OnMouseWheel(e);
		}
	}	
}
