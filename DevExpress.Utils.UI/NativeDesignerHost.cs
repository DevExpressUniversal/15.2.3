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
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel;
namespace DevExpress.Utils.UI {
	public class NativeDesignerHost : IDesignerHost {
		#region IDesignerHost Members
		public IContainer Container {
			get { return null; }
		}
		public void Activate() {
		}
		public event EventHandler Activated {
			add { }
			remove { }
		}
		public virtual IComponent CreateComponent(Type componentClass, string name) {
			return null;
		}
		public IComponent CreateComponent(Type componentClass) {
			return null;
		}
		public DesignerTransaction CreateTransaction(string description) {
			return null;
		}
		public DesignerTransaction CreateTransaction() {
			return null;
		}
		public event EventHandler Deactivated {
			add { }
			remove { }
		}
		public void DestroyComponent(IComponent component) {
		}
		public IDesigner GetDesigner(IComponent component) {
			return null;
		}
		public Type GetType(string typeName) {
			return null;
		}
		public bool InTransaction {
			get { return false; }
		}
		public event EventHandler LoadComplete { 
			add {}
			remove { }
		}
		public bool Loading {
			get { return false; }
		}
		public IComponent RootComponent {
			get { return null; }
		}
		public string RootComponentClassName {
			get { return string.Empty; }
		}
		public event DesignerTransactionCloseEventHandler TransactionClosed {
			add { }
			remove { }
		}
		public event DesignerTransactionCloseEventHandler TransactionClosing {
			add { }
			remove { }
		}
		public string TransactionDescription {
			get { return string.Empty; }
		}
		public event EventHandler TransactionOpened {
			add { }
			remove { }
		}
		public event EventHandler TransactionOpening {
			add {}
			remove {}
		}
		#endregion
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
		}
		public void AddService(Type serviceType, object serviceInstance) {
		}
		public void RemoveService(Type serviceType, bool promote) {
		}
		public void RemoveService(Type serviceType) {
		}
		#endregion
		#region IServiceProvider Members
		public virtual object GetService(Type serviceType) {
			if (serviceType == typeof(IDesignerHost)) {
				return this;
			}
			return null;
		}
		#endregion
	}
}
