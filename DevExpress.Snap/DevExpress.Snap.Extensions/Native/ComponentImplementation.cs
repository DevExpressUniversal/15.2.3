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
using DevExpress.Utils;
using System.ComponentModel;
namespace DevExpress.Snap.Extensions.Native {	
	public class SiteImplementation : ISite {
		readonly IComponent component;
		readonly IServiceProvider serviceProvider;
		public SiteImplementation(IComponent component, IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(component, "component");
			this.component = component;
			this.serviceProvider = serviceProvider; 
		}
		public IComponent Component { get { return component; } }
		public IContainer Container { get { return null; } }
		public bool DesignMode { get { return false; } }
		public string Name { get; set; }
		public object GetService(Type serviceType) {
			return serviceProvider != null ? serviceProvider.GetService(serviceType) : null;
		}
	}
	[DXToolboxItem(false)]
	public class ComponentImplementation : IComponent {
		readonly object customObject;
		readonly SiteImplementation site;
		public ComponentImplementation(object editedObject) : this(editedObject, null) { 
		}
		public ComponentImplementation(object editedObject, IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(editedObject, "editedObject");
			this.customObject = editedObject;
			this.site = new SiteImplementation(this, serviceProvider);
		}
		#region event handlers
		EventHandler disposed;
		public event EventHandler Disposed { add { disposed += value; } remove { disposed = Delegate.Remove(disposed, value) as EventHandler; } }
		protected virtual void RaiseDisposed() {
			if (disposed != null)
				disposed(this, EventArgs.Empty);
		}
		#endregion
		public object CustomObject { get { return customObject; } }
		public ISite Site {
			get { return site; }
			set { throw new NotImplementedException(); }
		}
		public void Dispose() {
			RaiseDisposed();
		}
	}
}
