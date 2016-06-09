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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Utils;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.Design {
	public class LockService : ILockService {
		#region static
		public static readonly LockService NullLockService = new LockService();
		public static LockService GetInstance(IServiceProvider serviceProvider) {
			return (LockService)ServiceHelper.GetServiceInstance(serviceProvider, typeof(ILockService), NullLockService);
		}
		protected static IComponent GetRealComponent(IComponent component) {
			FakeComponent fakeComponent = component as FakeComponent;
			return fakeComponent == null ? component : fakeComponent.Parent;
		}
		#endregion
		protected LockService() {
		}
		public bool CanChangeComponents(ICollection components) {
			foreach(IComponent component in components) {
				if(!CanChangeComponent(component))
					return false;
			}
			return true;
		}
		public bool CanDeleteComponents(ICollection components) {
			if(components == null) {
				return false;
			}
			foreach(IComponent component in components) {
				if(!CanDeleteComponent(component))
					return false;
			}
			return true;
		}
		public virtual bool CanChangeComponent(IComponent component) {
			return false;
		}
		public virtual bool CanDeleteComponent(IComponent component) {
			return false;
		}
		public virtual bool CanChangeControlParent(XRControl control, XRControl newParent) {
			return false;
		}
		public bool CanChangeBandHeight(Band band, float bottom) {
			if(band != null && !this.CanChangeComponent(band))
				return bottom <= band.HeightF;
			return true;
		}
		public bool CanChangeAnyComponent(ICollection components) {
			foreach (IComponent component in components) {
				if (CanChangeComponent(component))
					return true;
			}
			return false;
		}
		public bool CanDeleteAnyComponent(ICollection components) {
			foreach (IComponent component in components) {
				if (CanDeleteComponent(component))
					return true;
			}
			return false;
		}
	}
	public class VSLockService : LockService {
		IDesignerHost host;
		public VSLockService(IDesignerHost host) {
			if(host == null)
				throw new ArgumentNullException("host");
			this.host = host;
		}
		public override bool CanChangeComponent(IComponent component) {
			XRComponentDesignerBase designer = GetDesigner(component);
			return designer == null ? true : !designer.IsInheritedReadonly;
		}
		public override bool CanDeleteComponent(IComponent component) {
			XRComponentDesignerBase designer = GetDesigner(component);
			return designer == null ? true : !designer.IsInherited;
		}
		public override bool CanChangeControlParent(XRControl control, XRControl newParent) {
			if(control.Parent != newParent)
				return CanChangeComponent(newParent) && CanDeleteComponent(control);
			return CanChangeComponent(control);
		}
		XRComponentDesignerBase GetDesigner(IComponent component) {
			return host.GetDesigner(GetRealComponent(component)) as XRComponentDesignerBase;
		}
	}
	public class EUDLockService : LockService {
		public EUDLockService() {
		}
		public override bool CanChangeComponent(IComponent component) {
			IComponent realComponent = GetRealComponent(component);
			return realComponent is XRControl ? !((XRControl)realComponent).ActualLockedInUserDesigner : true;
		}
		public override bool CanDeleteComponent(IComponent component) {
			XRControl control = GetRealComponent(component) as XRControl;
			if(control != null && !control.IsDisposed) {
				NestedComponentEnumerator en = new NestedComponentEnumerator(control.Controls);
				while(en.MoveNext()) {
					if(en.Current.ActualLockedInUserDesigner)
						return false;
				}
				return !control.ActualLockedInUserDesigner;
			}
			return true;
		}
		public override bool CanChangeControlParent(XRControl control, XRControl newParent) {
			if(control.Parent != newParent)
				return CanChangeComponent(newParent) && CanChangeComponent(control);
			return CanChangeComponent(control);
		}
	}
}
