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

using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using System;
using System.Collections.Generic;
using System.Windows.Threading;
namespace DevExpress.Mvvm.UI.ViewInjection {
	public class DocumentGroupStrategy : StrategyBase<DocumentGroup> {
		public static void RegisterStrategy() {
			StrategyManager.Default.RegisterStrategy<DocumentGroup, DocumentGroupStrategy>();
		}
		const string Exception = "ViewInjectionService cannot create view by name or type, because the target control has the ItemContentTemplate/ItemContentTemplateSelector property set.";
		protected DockLayoutManager Manager { get; private set; }
		protected Dictionary<object, DocumentPanel> DocumentPanels { get; private set; }
		protected override void InitializeCore() {
			base.InitializeCore();
			Manager = Target.FindDockLayoutManager();
			Manager.DockItemActivated += OnManagerDockItemActivated;
			Manager.DockItemClosing += OnManagerDockItemClosing;
			DocumentPanels = new Dictionary<object, DocumentPanel>();
			if(Target.ItemContentTemplate == null && Target.ItemContentTemplateSelector == null)
				Target.ItemContentTemplateSelector = ViewSelector;
		}
		protected override void UninitializeCore() {
			Manager.DockItemClosing -= OnManagerDockItemClosing;
			Manager.DockItemActivated -= OnManagerDockItemActivated;
			DocumentPanels = null;
			Manager = null;
			base.UninitializeCore();
		}
		protected override void CheckInjectionProcedure(object viewModel, string viewName, Type viewType) {
			base.CheckInjectionProcedure(viewModel, viewName, viewType);
			if((!string.IsNullOrEmpty(viewName) || viewType != null) && (Target.ItemContentTemplate != null || Target.ItemContentTemplateSelector != ViewSelector))
				throw new InvalidOperationException(Exception);
		}
		protected override void OnInjected(object viewModel) {
			base.OnInjected(viewModel);
			var res = Manager.DockController.AddDocumentPanel(Target);
			res.DataContext = res.Content = res.Caption = viewModel;
			Target.PrepareContainerForItemCore(res);
			DocumentPanels.Add(viewModel, res);
			if(SelectedViewModel == null)
				SelectedViewModel = viewModel;
		}
		protected override void OnRemoved(object viewModel) {
			base.OnRemoved(viewModel);
			Manager.DockController.RemovePanel(DocumentPanels[viewModel]);
			DocumentPanels.Remove(viewModel);
			if(DocumentPanels.Count == 0)
				SelectedViewModel = null;
		}
		protected override void OnSelectedViewModelChanged(object oldValue, object newValue) {
			base.OnSelectedViewModelChanged(oldValue, newValue);
			if(newValue == null || selectionProcedureLocked) return;
			var docPanel = (DocumentPanel)DocumentPanels[newValue];
			Manager.DockController.Activate(docPanel);
		}
		void OnManagerDockItemActivated(object sender, DockItemActivatedEventArgs e) {
			DocumentPanel panel = e.Item as DocumentPanel;
			if(panel == null || !DocumentPanels.ContainsValue(panel)) return;
			selectionProcedureLocked = true;
			SelectedViewModel = DocumentPanels.GetKeyByValue(panel);
			selectionProcedureLocked = false;
		}
		bool selectionProcedureLocked = false;
		void OnManagerDockItemClosing(object sender, ItemCancelEventArgs e) {
			DocumentPanel panel = e.Item as DocumentPanel;
			if(panel == null || !DocumentPanels.ContainsValue(panel)) return;
			if(DockControllerHelper.GetActualClosingBehavior(Manager, panel) == ClosingBehavior.ImmediatelyRemove) {
				e.Cancel = true;
				Remove(DocumentPanels.GetKeyByValue(panel));
			}
		}
	}
}
