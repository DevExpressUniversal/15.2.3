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
using DevExpress.Diagram.Core;
using DevExpress.Utils.Menu;
using DevExpress.XtraToolbox;
using DevExpress.XtraDiagram.Utils;
namespace DevExpress.XtraDiagram.Toolbox {
	public class ToolboxController : IDisposable {
		IToolboxControl toolbox;
		DiagramToolboxControlViewModel viewModel;
		public ToolboxController(IToolboxControl toolbox) {
			this.toolbox = toolbox;
			this.viewModel = CreateViewModel();
			SubscribeEvents(toolbox);
			InitializeToolbox();
			Toolbox.SelectedGroupIndex = 0;
			if(Toolbox is ToolboxControl) {
				ToolboxControl tb = ((ToolboxControl)Toolbox);
				tb.OptionsView.ItemImageSize = new System.Drawing.Size(32, 32);
			}
		}
		protected virtual DiagramToolboxControlViewModel CreateViewModel() {
			return new DiagramToolboxControlViewModel(DiagramToolboxRegistrator.Stencils);
		}
		public void InitializeToolbox() {
			ViewModel.CheckStencils(GetStartCategories());
			LoadGroups();
		}
		protected virtual void SubscribeEvents(IToolboxControl toolbox) {
			toolbox.SelectedGroupChanged += OnToolboxSelectedGroupChanged;
			toolbox.InitializeMenu += OnToolboxInitializeMenu;
			toolbox.SearchTextChanged += OnToolboxSearchTextChanged;
			toolbox.StateChanged += OnToolboxStateChanged;
		}
		protected virtual void UnsubscribeEvents(IToolboxControl toolbox) {
			toolbox.SelectedGroupChanged -= OnToolboxSelectedGroupChanged;
			toolbox.InitializeMenu -= OnToolboxInitializeMenu;
			toolbox.SearchTextChanged -= OnToolboxSearchTextChanged;
			toolbox.StateChanged -= OnToolboxStateChanged;
		}
		private void OnToolboxStateChanged(object sender, ToolboxStateChangedEventArgs e) {
			ToolboxControl tb = Toolbox as ToolboxControl;
			if(tb == null) return;
			if(tb.OptionsMinimizing.State == ToolboxState.Minimized)
				tb.OptionsView.MenuButtonImage = ImageUtils.LoadImage("CompactView_16x16.png");
			else
				tb.OptionsView.MenuButtonImage = null;
		}
		protected virtual void OnToolboxSearchTextChanged(object sender, ToolboxSearchTextChangedEventArgs e) {
			ViewModel.SearchText = e.Text;
			e.Result = CreateItems();
		}
		protected virtual void OnToolboxInitializeMenu(object sender, ToolboxInitializeMenuEventArgs e) {
			LoadMenu(e.Menu, e.IsMinimized);
		}
		protected virtual void OnToolboxSelectedGroupChanged(object sender, ToolboxSelectedGroupChangedEventArgs e) {
			LoadItems(e.Group);
		}
		public DiagramToolboxControlViewModel ViewModel { get { return viewModel; } }
		IEnumerable<string> HiddenShapeId {
			get {
				yield return "NoSymbol";
				yield return "Tools";
				yield return "Heart";
			}
		}
		protected IEnumerable<ToolboxItem> CreateItems() {
			foreach(ShapesItem si in ViewModel.ShapesItemCollection) {
				bool isBeginGroup = ViewModel.ShapesItemCollection.Count > 1;
				foreach(ShapeDescription shape in si.Tools.OfType<ShapeTool>().Select(x => x.Shape)) {
					if(HiddenShapeId.Contains(shape.Id)) continue;
					yield return ToolboxUtils.CreateToolboxItem(si.Name, shape, isBeginGroup);
					isBeginGroup = false;
				}
			}
		}
		public void LoadGroups() {
			Toolbox.Groups.Clear();
			foreach(StencilInfo stencil in ViewModel.CheckedStencils) {
				Toolbox.Groups.Add(new ToolboxGroup(stencil.Name) { Tag = stencil });
			}
		}
		public void LoadItems(IToolboxGroup group) {
			if(group == null) return;
			ViewModel.SelectedStencil = group.Tag as StencilInfo;
			group.Items.BeginUpdate();
			try {
				group.Items.Clear();
				group.Items.AddRange(CreateItems());
			}
			finally {
				group.Items.EndUpdate();
			}
		}
		protected string[] GetStartCategories() {
			return new string[] {
				DiagramToolboxRegistrator.Stencils.ElementAt(0).Id,
				DiagramToolboxRegistrator.Stencils.ElementAt(1).Id
			};
		}
		public IToolboxControl Toolbox { get { return toolbox; } }
		#region Menu
		public DXPopupMenu LoadMenu(DXPopupMenu menu, bool extend) {
			menu.Items.Clear();
			if(!extend) return FillStencils(menu) as DXPopupMenu;
			menu.Items.Add(FillStencils(new DXSubMenuItem("More items")));
			return FillCheckedStencils(menu);
		}
		protected virtual DXSubMenuItem FillStencils(DXSubMenuItem menu) {
			foreach(StencilInfo stencil in ViewModel.Stencils) {
				DXMenuItem item = CreateStencilCheckedItem(stencil);
				if(item == null) continue;
				menu.Items.Add(item);
			}
			return menu;
		}
		protected virtual DXPopupMenu FillCheckedStencils(DXPopupMenu menu) {
			foreach(StencilInfo stencil in ViewModel.CheckedStencils) {
				DXMenuItem item = CreateStencilItem(stencil);
				if(item == null) continue;
				item.BeginGroup = menu.Items.Count == 1;
				menu.Items.Add(item);
			}
			return menu;
		}
		protected virtual DXMenuItem CreateStencilCheckedItem(StencilInfo stencil) {
			if(!stencil.MenuIsVisible) return null;
			DXMenuCheckItem item = new DXMenuCheckItem() { CloseMenuOnClick = false };
			item.Caption = stencil.Name;
			item.Checked = stencil.IsChecked;
			item.Tag = stencil;
			item.CheckedChanged += OnStencilCheckedChanged;
			return item;
		}
		protected virtual DXMenuItem CreateStencilItem(StencilInfo stencil) {
			if(!stencil.MenuIsVisible) return null;
			DXMenuItem item = new DXMenuItem() { CloseMenuOnClick = false };
			item.Caption = stencil.Name;
			item.Tag = stencil;
			item.Click += OnStencilClick;
			return item;
		}
		protected virtual void OnStencilClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			if(item == null) return;
			foreach(IToolboxGroup group in Toolbox.Groups) {
				if(!group.Tag.Equals(item.Tag)) continue;
				Toolbox.SelectedGroup = group;
			}
		}
		protected virtual void OnStencilCheckedChanged(object sender, EventArgs e) {
			DXMenuCheckItem item = sender as DXMenuCheckItem;
			if(item == null) return;
			StencilInfo stencil = item.Tag as StencilInfo;
			if(stencil == null) return;
			stencil.IsChecked = item.Checked;
			LoadGroups();
			Toolbox.SelectedGroupIndex = Toolbox.Groups.Count - 1;
		}
		#endregion
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeEvents(Toolbox);
			}
			this.toolbox = null;
		}
		#endregion
	}
}
