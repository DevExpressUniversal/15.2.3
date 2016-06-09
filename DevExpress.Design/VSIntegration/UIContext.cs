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
using DevExpress.Data.Utils;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
namespace DevExpress.Design.VSIntegration {
	public delegate void ActiveChangedEventHandler(object sender, ActiveChangedEventArgs e);
	public class ActiveChangedEventArgs : EventArgs {
		public bool Active { get; private set; }
		public ActiveChangedEventArgs(bool active) {
			this.Active = active;
		}
	}
	public class VSUIContext : IDisposable, IVsSelectionEvents {
		Guid contextType;
		uint eventCookie;
		uint cookie = 0;
		IVsMonitorSelection monitorSelectionService;
		public bool IsActive {
			get {
				int pfActive = 0;
				monitorSelectionService.IsCmdUIContextActive(Cookie, out pfActive);
				return (pfActive == 1);
			}
		}
		uint Cookie {
			get {
				if (cookie == 0)
					monitorSelectionService.GetCmdUIContextCookie(ref contextType, out cookie);
				return cookie;
			}
		}
		public event ActiveChangedEventHandler ActiveChanged;
		public VSUIContext(IServiceProvider services, Guid contextType) {
			monitorSelectionService = services.GetService<IVsMonitorSelection>();
			monitorSelectionService.AdviseSelectionEvents(this, out eventCookie);
			this.contextType = contextType;
		}
		public void Dispose() {
			monitorSelectionService.UnadviseSelectionEvents(eventCookie);
		}
		void RaiseChanged(bool value) {
			if (ActiveChanged != null)
				ActiveChanged(this, new ActiveChangedEventArgs(value));
		}
		int IVsSelectionEvents.OnCmdUIContextChanged(uint dwCmdUICookie, int fActive) {
			if (Cookie == dwCmdUICookie) {
				if (fActive == 1)
					RaiseChanged(true);
				else
					RaiseChanged(false);
			}
			return VSConstants.S_OK;
		}
		int IVsSelectionEvents.OnElementValueChanged(uint elementid, object varValueOld, object varValueNew) {
			return VSConstants.S_OK;
		}
		int IVsSelectionEvents.OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld,
			ISelectionContainer pSCOld, IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew) {
			return VSConstants.S_OK;
		}
	}
	public class VSUIContextDebug : VSUIContext {
		public VSUIContextDebug(IServiceProvider services)
			: base(services, VSConstants.UICONTEXT_Debugging) {
		}
	}
}
