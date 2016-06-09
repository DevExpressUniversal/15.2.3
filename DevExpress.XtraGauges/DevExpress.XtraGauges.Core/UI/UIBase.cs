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
using DevExpress.XtraGauges.Core.Base;
using System.Drawing;
using DevExpress.XtraGauges.Base;
using System.ComponentModel.Design;
using System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraGauges.Core.Base {
	public abstract class BaseViewInfo : BaseObject {
		bool isReadyCore;
		Graphics graphicsCore;
		protected override void OnCreate() {
			this.isReadyCore = false;
		}
		protected override void OnDispose() {
			this.isReadyCore = false;
		}
		public bool IsReady {
			get { return isReadyCore; }
		}
		public void SetDirty() {
			this.isReadyCore = false;
		}
		public void CalcInfo(Graphics g, Rectangle bounds) {
			if(IsReady || IsDisposing) return;
			this.graphicsCore = g;
			CalcViewRects(bounds);
			CalcViewStates();
			this.graphicsCore = null;
			this.isReadyCore = true;
		}
		protected abstract void CalcViewRects(Rectangle bounds);
		protected abstract void CalcViewStates();
		protected Graphics Graphics {
			get { return graphicsCore; }
		}
	}
#if !DXPORTABLE
	public class ComponentTransaction : IDisposable {
		IComponent component = null;
		IComponentChangeService service;
		public ComponentTransaction(IComponent c) {
			this.component = c;
			this.service = (component != null) && (component.Site != null) ? (IComponentChangeService)component.Site.GetService(typeof(IComponentChangeService)) : null;
			if(service != null) {
				try {
					service.OnComponentChanging(component, null);
				}
				catch { } 
			}
		}
		void IDisposable.Dispose() {
			if(service != null) {
				try {
					service.OnComponentChanged(component, null, null, null);
				}
				catch { } 
			}
			component = null;
			service = null;
		}
	}
#endif
}
