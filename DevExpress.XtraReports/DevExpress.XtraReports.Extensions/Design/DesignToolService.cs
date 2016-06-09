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
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Collections;
namespace DevExpress.XtraReports.Design {
	public interface IEnvironmentService {
		bool IsEndUserDesigner { get; }
	}
	public static class DesignToolHelper {
		public static bool IsEndUserDesigner(IServiceProvider servProvider) {
			IEnvironmentService serv = servProvider.GetService(typeof(IEnvironmentService)) as IEnvironmentService;
			return serv != null && serv.IsEndUserDesigner;
		}
		public static void AddToContainer(IDesignerHost host, IComponent component, string name) {
			ForceAddToContainer(host.Container, component, name);
			ComponentDesigner designer = host.GetDesigner(component) as ComponentDesigner;
			if(designer != null) {
				designer.InitializeNewComponent(new SortedList());
			}
		}
		public static void ForceAddToContainer(IContainer container, IComponent component, string name) {
			if(string.IsNullOrEmpty(name))
				container.Add(component);
			else {
				try {
					container.Add(component, name);
				} catch {
					container.Add(component);
				}
			}
		}
		public static void RemoveFromContainer(IDesignerHost designerHost, IComponent component) {
			designerHost.DestroyComponent(component);
		}
		public static IList GetAssociatedComponents(IComponent component, IDesignerHost designerHost) {
			ArrayList list = new ArrayList();
			GetAssociatedComponents(component, designerHost, list);
			return list;
		}
		static void GetAssociatedComponents(IComponent component, IDesignerHost designerHost, IList list) {
			ComponentDesigner designer = designerHost.GetDesigner(component) as ComponentDesigner;
			if(designer == null)
				return;
			foreach(IComponent childComponent in designer.AssociatedComponents)
				if(childComponent.Site != null) {
					list.Add(childComponent);
					GetAssociatedComponents(childComponent, designerHost, list);
				}
		}
		public static void AddToContainer(IDesignerHost host, IComponent[] components) {
			int count = components.Length;
			for(int i = 0; i < count; i++)
				host.AddToContainer(components[i], true);
		}
		public static void AddToContainer(IDesignerHost host, IComponent c) {
			host.AddToContainer(c, true);
		}
	}
	public class DesignToolService : IDesignTool {
		static IDesignerHost GetDesignerHost(IServiceProvider servProvider) {
			return servProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
		}
		public bool IsEndUserDesigner(IServiceProvider servProvider) {
			return DesignToolHelper.IsEndUserDesigner(servProvider);
		}
		public void AddToContainer(IServiceProvider servProvider, IComponent component) {
			GetDesignerHost(servProvider).AddToContainer(component, true);
		}
		public void AddToContainer(IServiceProvider servProvider, IComponent component, string name) {
			DesignToolHelper.AddToContainer(GetDesignerHost(servProvider), component, name);
		}
		public void ForceAddToContainer(IContainer container, IComponent component, string name) {
			DesignToolHelper.ForceAddToContainer(container, component, name);
		}
		public void RemoveFromContainer(IServiceProvider servProvider, IComponent component) {
			DesignToolHelper.RemoveFromContainer(GetDesignerHost(servProvider), component);
		}
	}
}
