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

using DevExpress.Skins;
using DevExpress.Utils.Taskbar;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
namespace DevExpress.Utils.Design.Taskbar {
	public partial class JumpListEditorForm : XtraForm {
		ITypeDescriptorContext context;
		IServiceProvider provider;
		object value;
		public JumpListEditorForm(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			this.context = context;
			this.provider = provider;
			this.value = value;
			SkinManager.EnableFormSkins();
			InitializeComponent();
			InitializeCollection(value);
			CalcSplitterPosition(); 
			UpdateButtons();
			PropertyGrid.Site = DesignTimeManager.Component.Site;
			AddedComponents = new List<Component>();
			RemovedComponents = new List<Component>();
		}
		protected List<Component> AddedComponents { get; set; }
		protected List<Component> RemovedComponents { get; set; }
		#region Controls
		protected TreeView TreeView { get { return treeView; } }
		protected SimpleButton BtnAdd { get { return btnAdd; } }
		protected SimpleButton BtnAddTask { get { return btnAddTask; } }
		protected SimpleButton BtnRemove { get { return btnRemove; } }
		protected SimpleButton BtnUpCmd { get { return btnUpCmd; } }
		protected SimpleButton BtnDownCmd { get { return btnDownCmd; } }
		protected PropertyGrid PropertyGrid { get { return propertyGrid; } }
		protected LabelControl LblTasks { get { return lblTasks; } }
		#endregion
		protected virtual ITypeDescriptorContext Context { get { return context; } }
		protected virtual IServiceProvider Provider { get { return provider; } }
		protected virtual TaskbarAssistant TaskbarAssistant { get { return (TaskbarAssistant)Context.Instance; } }
		protected virtual TaskbarAssistantDesignTimeManager DesignTimeManager { get { return (TaskbarAssistantDesignTimeManager)TaskbarAssistant.DesignTimeManager; } }
		protected virtual void InitializeCollection(object list) { }
		protected virtual void OnBtnRemoveClick(object sender, EventArgs e) { }
		protected virtual void OnBtnAddTaskClick(object sender, EventArgs e) { }
		protected virtual void OnBtnUpCmdClick(object sender, EventArgs e) { }
		protected virtual void OnBtnDownCmdClick(object sender, EventArgs e) { }
		public object Result { get { return value; } }
		void CalcSplitterPosition() {
			splitContainerControl.Panel1.MinSize = splitContainerControl.SplitterPosition = 
				btnAddTask.Width + 
				btnAdd.Width + 
				btnUpCmd.Width +
				((btnUpCmd.Location.X) -
				(btnAdd.Location.X + btnAdd.Width)) +
				((btnAdd.Location.X) - 
				(btnAddTask.Location.X + btnAddTask.Width)) +
				(btnUpCmd.Location.X - (treeView.Location.X + treeView.Width)) + 3;
		}
		protected virtual void RefreshCollection() {
			UpdateButtons();
		}
		protected virtual void SetResult(object result) {
			this.value = result;
		}
		protected virtual void OnTreeViewAfterSelect(object sender, TreeViewEventArgs e) {
			propertyGrid.SelectedObject = treeView.SelectedNode.Tag;
			UpdateButtons();
		}
		protected virtual void UpdateButtons() {
			BtnAddTask.Enabled = BtnAdd.Enabled = BtnRemove.Enabled =
				BtnUpCmd.Enabled = BtnDownCmd.Enabled = true;
		}
		protected virtual void OnPropertyGridValueChanged(object s, PropertyValueChangedEventArgs e) {
			RefreshCollection();
		}
		protected virtual void OnBtnOKClick(object sender, EventArgs e) {
			SetResult(value);
			ApplyChanges();
		}
		protected virtual void OnBtnCancelClick(object sender, EventArgs e) {
			CancelChanges();
		}
		void ShowEventsButton(bool value) {
			Type type =  PropertyGrid.GetType();
			MethodInfo mi = type.GetMethod("ShowEventsButton", BindingFlags.NonPublic | BindingFlags.Instance);
			mi.Invoke(PropertyGrid, new object[] { value });
		}
		void OnSelectedObjectsChanged(object sender, EventArgs e) {
			PropertyGrid.PropertyTabs.AddTabType(typeof(System.Windows.Forms.Design.EventsTab), PropertyTabScope.Document);
			ShowEventsButton(true);
		}
		protected override void OnClosing(CancelEventArgs e) {
			PropertyGrid.Site = null;
			base.OnClosing(e);
		}
		void CancelChanges() {
			foreach(Component item in RemovedComponents) TaskbarAssistant.Container.Add(item);
			foreach(Component item in AddedComponents) item.Dispose();
		}
		void ApplyChanges() {
			foreach(Component item in AddedComponents) TaskbarAssistant.Container.Add(item);
			foreach(Component item in RemovedComponents) item.Dispose();
		}
	}
}
