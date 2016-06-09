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
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Printing.Design.Bars;
using System.Windows.Controls;
using System.Windows;
using Platform::DevExpress.Xpf.Printing.Native;
using System.Windows.Data;
using System;
using Microsoft.Windows.Design.Metadata;
using Platform::DevExpress.Xpf.Docking;
using DevExpress.Xpf.Core.Design;
using System.Collections.ObjectModel;
using Platform::DevExpress.Xpf.Core.Native;
using Platform::DevExpress.Mvvm.UI.Interactivity;
using System.Linq;
using Platform::DevExpress.Xpf.Printing;
using Platform::DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Printing.Design.LayoutCreators {
	public abstract class LayoutCreatorBase {
		protected ModelItem PrimarySelection { get; set; }
		protected abstract CommandBarCreator CommandCreator { get; }
		protected abstract string ScopeDescription { get; }
		public void CreateLayout(ModelItem primarySelection) {
			PrimarySelection = primarySelection;
			ClearContent();
			using(ModelEditingScope scope = primarySelection.Root.BeginEdit(ScopeDescription)) {
				try {
					EnsureControlHasName(PrimarySelection);
					CreateResources();
					CreateBars(primarySelection);
				} finally {
					scope.Complete();
				}
			}
		}
		void CreateResources() {
			ModelItem command = ModelFactory.CreateItem(PrimarySelection.Context, typeof(DisabledCommand));
			ResourceManager.AddResource(command, ResourceKeys.DisabledCommand);
			ResourceManager.CreateConverter<ObjectToBooleanConverter>(PrimarySelection, ResourceKeys.ObjectToBooleanConverter);
		}
		void ClearContent() {
			var parent = PrimarySelection.Parent;
			if(parent == PrimarySelection.Root)
				return;
			using(ModelEditingScope scope = PrimarySelection.Root.BeginEdit("Clear content")) {
				try {
					if(parent.ItemType.IsAssignableFrom(typeof(ContentControl))) {
						parent.Properties["Content"].ClearValue();
					} else {
						PrimarySelection.Root.Properties["Content"].ClearValue();
					}
					ModelParent.Parent(PrimarySelection.Context, PrimarySelection.Root, PrimarySelection);
					ClearResources();
				} finally {
					scope.Complete();
				}
			}
		}
		void ClearResources() {
			foreach(var resourceKey in ResourceKeys.ResourceKeyList) {
				ResourceManager.RemoveResource(PrimarySelection.Root, resourceKey);
			}
		}
		protected internal void EnsureControlHasName(ModelItem item, string name = "") {
			if(string.IsNullOrEmpty(item.Name)) {
				ModelItem fakeControl = ModelFactory.CreateItem(item.Context, item.ItemType, CreateOptions.InitializeDefaults);
				item.Name = fakeControl.Name;
			}
			if(string.IsNullOrEmpty(item.Name)) {
				try {
					System.Reflection.MethodInfo method = typeof(ModelFactory).GetMethod("AssignUniqueName");
					if(method != null)
						method.Invoke(null, new object[] { item.Context, PrimarySelection != null ? PrimarySelection : item, item });
				} catch {
				}
			}
			if(string.IsNullOrEmpty(item.Name)) {
				item.Name = item.ItemType.Name + "1";
			}
		}
		protected abstract void CreateBars(ModelItem rootInstance);
	}
	class RibbonLayoutCreator : LayoutCreatorBase {
		CommandBarCreator commandCreator;
		protected override CommandBarCreator CommandCreator {
			get {
				if(commandCreator == null)
					commandCreator = new RibbonCommandCreator();
				return commandCreator;
			}
		}
		protected override string ScopeDescription {
			get { return "Generate Ribbon Layout"; }
		}
		protected override void CreateBars(ModelItem rootInstance) {
			CommandCreator.CreateBars(rootInstance.Root.Content.Value, new BarInfo[] { 
#if !SILVERLIGHT
				BarInfos.FileGroup, 
#endif
				BarInfos.PrintGroup, BarInfos.NavigationGroup, BarInfos.ZoomGroup, BarInfos.ExportGroup, BarInfos.DocumentGroup, BarInfos.WatermarkGroup });
			if(CommandCreator is IStatusBarCommandCreator) {
				((IStatusBarCommandCreator)CommandCreator).CreateStatusBarItems(rootInstance, BarInfos.StatusBarGroup);
			}
			commandCreator = null;
		}
	}
	class BarsLayoutCreator : LayoutCreatorBase {
		CommandBarCreator commandCreator;
		protected override CommandBarCreator CommandCreator {
			get {
				if(commandCreator == null)
					commandCreator = new BarsCommandCreator();
				return commandCreator;
			}
		}
		protected override void CreateBars(ModelItem rootInstance) {
			CommandCreator.CreateBars(PrimarySelection.Root.Content.Value, new BarInfo[] { 
				BarInfos.DocumentGroup, 
#if !SILVERLIGHT
				BarInfos.FileGroup, 
#endif
				BarInfos.PrintGroup, BarInfos.ZoomBarsGroup, BarInfos.NavigationGroup, BarInfos.ExportGroup, BarInfos.WatermarkGroup });
			if(CommandCreator is IStatusBarCommandCreator) {
				((IStatusBarCommandCreator)CommandCreator).CreateStatusBarItems(rootInstance, BarInfos.StatusBarGroup);
			}
			commandCreator = null;
		}
		protected override string ScopeDescription {
			get { return "Generate Bars layout"; }
		}
	}
}
