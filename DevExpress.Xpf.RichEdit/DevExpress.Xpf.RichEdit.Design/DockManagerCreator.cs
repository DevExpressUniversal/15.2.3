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
using Platform::DevExpress.Xpf.Core.Design;
using Platform::System.Windows.Data;
using Platform::System.Windows.Controls;
using DevExpress.Xpf.Docking;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.RichEdit;
using Platform::System.Windows;
namespace DevExpress.Xpf.RichEdit.Design {
	public class DockManagerCreator {
		ModelItem root;
		ModelItem dockManager;
		ModelItem masterControl;
		public ModelItem DockManager { get { return dockManager; } }
		public ModelItem MasterControl { get { return masterControl; } }
		public void CreateDockManager(ModelItem primarySelection) {
			this.root = primarySelection.Root;
			try {
				using (ModelEditingScope scope = primarySelection.Root.BeginEdit()) {
					try {
						this.masterControl = primarySelection;
						if (this.masterControl == null)
							return;
						if (this.masterControl.Name == null)
							CreatorHelper.EnsureControlHasName(this.masterControl, root);
						this.dockManager = PrepareDockManager();
						if (this.dockManager == null)
							return;
						AppendLayoutGroupAndLayoutPanel();
					}
					finally {
						scope.Complete();
					}
				}
			}
			finally {
				this.root = null;
			}
		}
		protected internal virtual ModelItem PrepareDockManager() {
			ModelItem dockManager = CreatorHelper.FindItem<DockLayoutManager>(MasterControl);
			if (dockManager == null) {
				dockManager = CreateDockManager(MasterControl.Context, CreateOptions.None);
				CreatorHelper.EnsureControlHasName(dockManager, root);
				WrapMasterControlWithDockManager(dockManager);
			}
			else
				CreatorHelper.EnsureControlHasName(dockManager, root);
			return dockManager;
		}
		protected internal static ModelItem FindDockManager(ModelItem from) {
			return CreatorHelper.FindItem<DockLayoutManager>(from);
		}
		protected internal virtual ModelItem CreateDockManager(EditingContext context, CreateOptions options) {
			return ModelFactory.CreateItem(context, typeof(DockLayoutManager), options);
		}
		protected internal virtual void WrapMasterControlWithDockManager(ModelItem dockManager) {
#if !SL
			CreatorHelper.ReplaceMasterControl(MasterControl, dockManager);
#else
			ModelItem oldParent = MasterControl.Parent;
			RemoveControlFromParent(MasterControl);			
			ModelParent.Parent(oldParent.Context, oldParent, dockManager);			
			ModelParent.Parent(oldParent.Context, dockManager, MasterControl);		
#endif
		}
		void RemoveControlFromParent(ModelItem control) {
			ModelItem parent = MasterControl.Parent;
			if (parent.Content.IsCollection)
				parent.Content.Collection.Remove(control);				
			else
				parent.Content.SetValue(null);
		}
		void AppendLayoutGroupAndLayoutPanel() {
			ModelItem layoutGroup = ModelFactory.CreateItem(DockManager.Context, typeof(LayoutGroup));
			layoutGroup.Properties["Orientation"].SetValue(Orientation.Horizontal);
			dockManager.Properties["LayoutRoot"].SetValue(layoutGroup);
			AppendLayoutPanel(layoutGroup);
			AppendDocumentPanel(layoutGroup);
		}
		private void AppendDocumentPanel(ModelItem layoutGroup) {
			ModelItem documentPanel = ModelFactory.CreateItem(layoutGroup.Context, typeof(DocumentPanel));
			CreatorHelper.EnsureControlHasName(documentPanel, root);
			layoutGroup.Properties["Items"].Collection.Add(documentPanel);
			AppendMasterControl(documentPanel);
		}
		private void AppendMasterControl(ModelItem documentPanel) {
			documentPanel.Properties["Content"].SetValue(MasterControl);
		}
		void AppendLayoutPanel(ModelItem layoutGroup) {
			ModelItem layoutPanel = ModelFactory.CreateItem(layoutGroup.Context, typeof(LayoutPanel));
			CreatorHelper.EnsureControlHasName(layoutPanel, root);
			layoutPanel.Properties["Caption"].SetValue("Main document comments");
			layoutPanel.Properties["MinWidth"].SetValue("350");
			layoutPanel.Properties["MaxWidth"].SetValue("500");
			layoutGroup.Properties["Items"].Collection.Add(layoutPanel);
			AppendRichEditCommentControl(layoutPanel);
		}
		void AppendRichEditCommentControl(ModelItem layoutPanel) {
			ModelItem commentControl = ModelFactory.CreateItem(layoutPanel.Context, typeof(RichEditCommentControl));
			CreatorHelper.EnsureControlHasName(commentControl, root);
			SetRichEditCommentControlProperty(commentControl);
			CreatorHelper.BindItemToControl(MasterControl, commentControl, "RichEditControl");
#if !SL
			CreatorHelper.BindItemToControl(DockManager, commentControl, "DockLayoutManager");
#else
			CreatorHelper.BindItemToControlSL(DockManager, commentControl, "DockLayoutManager");
#endif
			CreatorHelper.BindItemToControl(layoutPanel, commentControl, "LayoutPanel");
			layoutPanel.Properties["Content"].SetValue(commentControl);
		}
		void SetRichEditCommentControlProperty(ModelItem commentControl) {
			commentControl.Properties["HorizontalAlignment"].SetValue(HorizontalAlignment.Stretch);
			commentControl.Properties["VerticalAlignment"].SetValue(VerticalAlignment.Stretch);
			commentControl.Properties["MinWidth"].SetValue("350");
		}
		void BindDockLayoutManagerToCommentControlSL(ModelItem commentControl) {
			ModelProperty dockManagerProperty = commentControl.Properties["DockLayoutManager"];
			if (!dockManagerProperty.IsSet) {
				ModelItem bindingModelItem = ModelFactory.CreateItem(DockManager.Context, typeof(Binding));
				bindingModelItem.Properties["Mode"].SetValue(BindingMode.OneTime);
				RelativeSource source = new RelativeSource(RelativeSourceMode.FindAncestor);
				source.AncestorType = typeof(DockLayoutManager);
				source.AncestorLevel = 1;
				bindingModelItem.Properties["RelativeSource"].SetValue(source);
				dockManagerProperty.SetValue(bindingModelItem);
			}
		}
	}
}
