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

extern alias Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using System.Diagnostics;
using Microsoft.Windows.Design.Metadata;
using Platform::DevExpress.Xpf.Core;
#if SILVERLIGHT
using Platform::System.Windows;
#else
using System.Windows;
#endif
namespace DevExpress.Xpf.Core.Design {
	class DXTabControlParentAdapter : ParentAdapter {
		public override void Parent(ModelItem parent, ModelItem child) {
			if(parent == null) throw new ArgumentNullException("parent");
			if(child == null) throw new ArgumentNullException("child");
			using(ModelEditingScope scope = parent.BeginEdit()) {
				ModelItem newTabItem = child.IsItemOfType(typeof(DXTabItem)) ? child : ModelFactory.CreateItem(parent.Context, typeof(DXTabItem), CreateOptions.InitializeDefaults);
				parent.Content.Collection.Add(newTabItem);
				if(!child.IsItemOfType(typeof(DXTabItem))) {
					scope.Update();
					try {
						ModelItem newParent = RedirectParent(newTabItem, child.ItemType);
						var adapterService = parent.Context.Services.GetRequiredService<AdapterService>();
						var parentAdapter = adapterService.GetAdapter<ParentAdapter>(newParent.ItemType);
						parentAdapter.Parent(newParent, child);
					} catch(Exception ex) {
						throw new InvalidOperationException("Parenting Failed", ex.InnerException);
					}
				}
				scope.Complete();
			}
		}
		public override ModelItem RedirectParent(ModelItem parent, Type childType) {
			if (parent == null) throw new ArgumentNullException("parent");
			if (childType == null) throw new ArgumentNullException("childType");
			if(ModelFactory.ResolveType(parent.Context, new TypeIdentifier(typeof(DXTabItem).FullName)).IsAssignableFrom(childType))
				return parent;
			else {
				if(parent.Content != null && parent.Content.Collection.Count > 0) {
					DXTabControl tabControl = parent.GetCurrentValue() as DXTabControl;
					int index = tabControl == null ? -1 : tabControl.SelectedIndex;
					if(index != -1) {
						ModelItem tabItem = parent.Content.Collection[index];
						return tabItem.IsItemOfType(typeof(DXTabItem)) && !tabItem.Content.IsSet ? tabItem : parent;
					}
				}
				return base.RedirectParent(parent, childType);
			}
		}
		public override void RemoveParent(ModelItem currentParent, ModelItem newParent, ModelItem child) {
			if (currentParent == null) throw new ArgumentNullException("currentParent");
			if (newParent == null) throw new ArgumentNullException("newParent");
			if (child == null) throw new ArgumentNullException("child");
			currentParent.Content.Collection.Remove(child);
		}
	}
}
