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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Layout.Core;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
namespace DevExpress.Xpf.Docking.Design {
	public abstract class ModelUpdate {
		public ModelUpdate(ModelItem root, NotificationEventArgs args, DockLayoutManager manager) {
			Root = root;
			Args = args;
			Manager = manager;
		}
		public ModelItem Root { get; private set; }
		public NotificationEventArgs Args { get; private set; }
		public DockLayoutManager Manager { get; private set; }
		protected EditingContext Context { get { return Root.Context; } }
		public virtual void Execute() { }
		protected ModelItem CreateCopy(EditingContext context, BaseLayoutItem layoutItem) {
			ModelItem modelItem = ModelFactory.CreateItem(context, layoutItem.GetType());
			var properties = TypeDescriptor.GetProperties(layoutItem);
			foreach(PropertyDescriptor pd in properties) {
				if(pd.IsReadOnly) continue;
				if(!pd.ShouldSerializeValue(layoutItem)) continue;
				if(pd.Name == "Items") continue;
				modelItem.Properties[pd.Name].SetValue(pd.GetValue(layoutItem));
			}
			return modelItem;
		}
	}
	public static class ModelUpdateFactory {
		public static ModelUpdate Create(ModelItem root, NotificationEventArgs args, DockLayoutManager manager) {
			LayoutGroup group = args.ActionTarget as LayoutGroup;
			LayoutPanel panel = args.ActionTarget as LayoutPanel;
			DockLayoutManager dockLayoutManager = args.ActionTarget as DockLayoutManager;
			DependencyProperty property = args.Property;
			BaseLayoutItem target = args.ActionTarget as BaseLayoutItem;
			if(target != null && property != null) {
				return new BaseLayoutItemPropertyUpdate(root, args, manager);
			}
			if(dockLayoutManager != null || group != null || panel != null) {
				return new DockLayoutManagerModelUpdate(root, args, manager);
			}
			return null;
		}
	}
	public class DockLayoutManagerModelUpdate : ModelUpdate {
		public DockLayoutManagerModelUpdate(ModelItem root, NotificationEventArgs args, DockLayoutManager manager)
			: base(root, args, manager) {
		}
		public override void Execute() {
			base.Execute();
			DockLayoutManagerModelUpdater updater = new DockLayoutManagerModelUpdater(Root, Manager);
			updater.UpdateModel();
		}
	}
	public class BaseLayoutItemPropertyUpdate : ModelUpdate {
		public BaseLayoutItemPropertyUpdate(ModelItem root, NotificationEventArgs args, DockLayoutManager manager)
			: base(root, args, manager) {
		}
		public DependencyProperty Property { get; set; }
		public override void Execute() {
			BaseLayoutItem item = Args.ActionTarget as BaseLayoutItem;
			Property = Args.Property;
			ModelItem model = ModelServiceHelper.FindModelItem(Context, item);
			if(model != null) {
				object value = item.GetValue(Property);
				model.Properties[new PropertyIdentifier(Property.OwnerType, Property.Name)].SetValue(value);
			}
		}
	}
}
