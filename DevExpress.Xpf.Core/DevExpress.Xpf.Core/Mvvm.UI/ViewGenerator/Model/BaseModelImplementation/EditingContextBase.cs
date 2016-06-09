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
using System.ComponentModel;
using System.Linq;
using System.Collections.Specialized;
using System.Reflection;
using System.Collections;
using DevExpress.Utils;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Mvvm.UI.Native.ViewGenerator.Model {
	public abstract class EditingContextBase : IEditingContext {
		#region ModelServiceBase
		public abstract class ModelServiceBase : IModelService {
			readonly EditingContextBase editingContext;
			public ModelServiceBase(EditingContextBase editingContext) {
				Guard.ArgumentNotNull(editingContext, "editingContext");
				this.editingContext = editingContext;
			}
			IModelSubscribedEvent IModelService.SubscribeToModelChanged(EventHandler handler) { return AddModelChangedHandler(handler); }
			void IModelService.UnsubscribeFromModelChanged(IModelSubscribedEvent e) { RemoveModelChangedHandler(e); }
			protected virtual IModelSubscribedEvent AddModelChangedHandler(EventHandler value) { return null; }
			protected virtual void RemoveModelChangedHandler(IModelSubscribedEvent value) { }
			IModelItem IModelService.Root {
				get { return editingContext.root; }
			}
			void IModelService.RaiseModelChanged() {
				throw new NotImplementedException();
			}
		}
		#endregion
		readonly IModelItem root;
		public IModelItem Root { get { return root; } }
		public EditingContextBase(object root) {
			Guard.ArgumentNotNull(root, "root");
			this.root = CreateModelItem(root, null);
			Services = new ServiceManagerBase();
			Services.Publish<IModelService>(CreateModelService);
		}
		#region IEditingContext
		IModelItem IEditingContext.CreateItem(Type type) {
			return CreateModelItem(Activator.CreateInstance(type), null);
		}
		IModelItem IEditingContext.CreateItem(DXTypeIdentifier typeIdentifier) {
			throw new NotImplementedException();
		}
		IModelItem IEditingContext.CreateStaticMemberItem(Type type, string memberName) {
			throw new NotImplementedException();
		}
		IModelItem IEditingContext.CreateItem(Type type, bool useDefaultInitializer) {
			throw new NotSupportedException();
		}
		IModelItem IEditingContext.CreateItem(DXTypeIdentifier typeIdentifier, bool useDefaultInitializer) {
			throw new NotSupportedException();
		}
		IServiceProvider IEditingContext.Services { get { return Services; } }
		#endregion
		public ServiceManagerBase Services { get; private set; }
		protected abstract IModelService CreateModelService();
		public abstract IModelItem CreateModelItem(object obj, IModelItem parent);
		public abstract IModelPropertyCollection CreateModelPropertyCollection(object element, IModelItem parent);
		public abstract IViewItem CreateViewItem(IModelItem modelItem);
		public abstract IModelEditingScope CreateEditingScope(string description);
		public abstract IModelItemDictionary CreateModelItemDictionary(IDictionary computedValue);
		public abstract IModelItemCollection CreateModelItemCollection(IEnumerable computedValue, IModelItem parent);
		public abstract IModelProperty CreateModelProperty(object obj, PropertyDescriptor property, IModelItem parent);
	}
}
