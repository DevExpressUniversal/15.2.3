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
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
namespace DevExpress.XtraReports.Design.MouseTargets {
	public interface IMouseTargetService {
		IMouseTarget GetMouseTarget(XRControl control);
		IMouseTarget GetMouseTarget(IDesigner designer);
		SelectionItemBase CreateSelectionItem(XRControl control);
	}
	public class MouseTargetService : IMouseTargetService, IDisposable {
		public static IMouseTarget GetMouseTarget(IServiceProvider servProvider, XRControl control) {
			IMouseTargetService serv = servProvider.GetService(typeof(IMouseTargetService)) as IMouseTargetService;
			return serv != null ? serv.GetMouseTarget(control) : null;
		}
		public static IMouseTarget GetMouseTarget(IServiceProvider servProvider, IDesigner designer) {
			IMouseTargetService serv = servProvider.GetService(typeof(IMouseTargetService)) as IMouseTargetService;
			return serv != null ? serv.GetMouseTarget(designer) : null;
		}
		Dictionary<IComponent, IMouseTarget> dictionary = new Dictionary<IComponent, IMouseTarget>();
		IServiceProvider servProvider;
		public MouseTargetService(IServiceProvider servProvider) {
			this.servProvider = servProvider;
			IComponentChangeService serv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(serv != null)
				serv.ComponentRemoved += new ComponentEventHandler(serv_ComponentRemoved);
		}
		object GetService(Type serviceType) {
			return servProvider.GetService(serviceType);
		}
		void IDisposable.Dispose() {
			IComponentChangeService serv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(serv != null)
				serv.ComponentRemoved -= new ComponentEventHandler(serv_ComponentRemoved);
			if(dictionary != null) {
				foreach(IMouseTarget mouseTarget in dictionary.Values)
					if(mouseTarget is IDisposable)
						((IDisposable)mouseTarget).Dispose();
				dictionary.Clear();
				dictionary = null;
			}
		}
		void serv_ComponentRemoved(object sender, ComponentEventArgs e) {
			IMouseTarget mouseTarget;
			if(dictionary.TryGetValue(e.Component, out mouseTarget)) {
				if(mouseTarget is IDisposable)
					((IDisposable)mouseTarget).Dispose();
				dictionary.Remove(e.Component);
			}
		}
		public virtual SelectionItemBase CreateSelectionItem(XRControl control) {
			System.Diagnostics.Debug.Assert(control != null);
			XRControlDesigner designer = GetDesigner(control) as XRControlDesigner;
			if(designer == null)
				return null;
			if(control is XRCrossBandControl)
				return new CrossBandSelectionItem(control, designer, IsPrimarySelection(control));
			return new ControlSelectionItem(control, designer, IsPrimarySelection(control));
		}
		bool IsPrimarySelection(object comp) {
			ISelectionService serv = GetService(typeof(ISelectionService)) as ISelectionService;
			bool primary = comp.Equals(serv.PrimarySelection) ||
				(comp is WinControlContainer && ((WinControlContainer)comp).WinControl == serv.PrimarySelection);
			return serv.SelectionCount <= 1 ? primary : !primary;
		}
		public virtual IMouseTarget GetMouseTarget(IDesigner designer) {
			IMouseTarget mouseTarget;
			if(!dictionary.TryGetValue(designer.Component, out mouseTarget)) {
				mouseTarget = CreateMouseTarget(designer);
				dictionary.Add(designer.Component, mouseTarget);
			}
			return mouseTarget;
		}
		public virtual IMouseTarget GetMouseTarget(XRControl control) {
			IMouseTarget mouseTarget;
			if(!dictionary.TryGetValue(control, out mouseTarget)) {
				mouseTarget = CreateMouseTarget(GetDesigner(control));
				dictionary.Add(control, mouseTarget);
			}
			return mouseTarget;
		}
		IDesigner GetDesigner(XRControl control) {
			IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host != null)
				return host.GetDesigner(control);
			return null;
		}
		IMouseTarget CreateMouseTarget(IDesigner designer) {
			if(designer != null) {
				MouseTargetAttribute attr = TypeDescriptor.GetAttributes(designer)[typeof(MouseTargetAttribute)] as MouseTargetAttribute;
				if(attr != null)
					return Activator.CreateInstance(attr.TargetType, designer.Component, servProvider) as IMouseTarget;
			}
			return null;
		}
	}
}
