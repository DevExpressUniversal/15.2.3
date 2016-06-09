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
namespace DevExpress.XtraReports.Design {
	public static class DesignTool {
		public static bool IsEndUserDesigner(IServiceProvider servProvider) {
			if(servProvider != null) {
				IDesignTool service = servProvider.GetService(typeof(IDesignTool)) as IDesignTool;
				return service != null ? service.IsEndUserDesigner(servProvider) : false;
			}
			return false;
		}
		public static void AddToContainer(IServiceProvider servProvider, IComponent component, string name) {
			if(servProvider != null) {
				IDesignTool service = servProvider.GetService(typeof(IDesignTool)) as IDesignTool;
				if(service != null)
					service.AddToContainer(servProvider, component, name);
			}
		}
		public static void AddToContainer(IServiceProvider servProvider, IComponent component) {
			if(servProvider != null) {
				IDesignTool service = servProvider.GetService(typeof(IDesignTool)) as IDesignTool;
				if(service != null)
					service.AddToContainer(servProvider, component);
			}
		}
		public static void RemoveFromContainer(IServiceProvider servProvider, IComponent component) {
			if(servProvider != null) {
				IDesignTool service = servProvider.GetService(typeof(IDesignTool)) as IDesignTool;
				if(service != null)
					service.RemoveFromContainer(servProvider, component);
			}
		}
		public static void ForceAddToContainer(ISite site, IComponent component, string name) {
			if(site != null) {
				IDesignTool service = site.GetService(typeof(IDesignTool)) as IDesignTool;
				if(service != null)
					service.ForceAddToContainer(site.Container, component, name);
			}
		}
	}
	public interface IDesignTool { 
		bool IsEndUserDesigner(IServiceProvider servProvider);
		void AddToContainer(IServiceProvider servProvider, IComponent component);
		void AddToContainer(IServiceProvider servProvider, IComponent component, string name);
		void ForceAddToContainer(IContainer container, IComponent component, string name);
		void RemoveFromContainer(IServiceProvider servProvider, IComponent component);
	}
}
