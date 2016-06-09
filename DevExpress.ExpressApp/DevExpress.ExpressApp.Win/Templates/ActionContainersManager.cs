#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.ExpressApp.Templates;
namespace DevExpress.ExpressApp.Win.Templates {
	[ToolboxItem(false)] 
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Designer("DevExpress.ExpressApp.Win.Design.ActionContainersManagerDesigner, DevExpress.ExpressApp.Win.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.ComponentModel.Design.IDesigner))]
	[DesignerCategory("Component")]
	public partial class ActionContainersManager : Component {
		private List<IActionContainer> actionContainerComponents;
		private List<IActionContainer> contextMenuContainers;
		private IActionContainer defaultContainer;
		private Control template;
		private View view;
		public ActionContainersManager() {
			actionContainerComponents = new List<IActionContainer>();
			defaultContainer = null;
			contextMenuContainers = new List<IActionContainer>();
			InitializeComponent();
		}
		public ActionContainersManager(IContainer container)
			: this() {
			container.Add(this);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("DevExpress.ExpressApp.Design.ComponentsCollectionEditor, DevExpress.ExpressApp.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		[ReadOnly(false), TypeConverter(typeof(CollectionConverter))]
		public List<IActionContainer> ActionContainerComponents {
			get { return actionContainerComponents; }
			set { actionContainerComponents = value; }
		}
		public ICollection<IActionContainer> GetContainers() {
			List<IActionContainer> actionContainers = new List<IActionContainer>();
			actionContainers.AddRange(actionContainerComponents);
			CustomizeActionContainersEventArgs args = new CustomizeActionContainersEventArgs(actionContainers);
			OnCustomizeActionContainers(args);
			actionContainers = args.ActionContainers;
			return actionContainers;
		}
		public IActionContainer DefaultContainer {
			get { return defaultContainer; }
			set { defaultContainer = value; }
		}
		[Editor("DevExpress.ExpressApp.Design.ComponentsCollectionEditor, DevExpress.ExpressApp.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(UITypeEditor))]
		[CollectionDataSourceProperty("ActionContainerComponents")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ReadOnly(false), TypeConverter(typeof(CollectionConverter))]
		public List<IActionContainer> ContextMenuContainers {
			get { return contextMenuContainers; }
			set { contextMenuContainers = value; }
		}
		public Control Template {
			get { return template; }
			set {
				if(template != null && template is ISupportViewChanged) {
					((ISupportViewChanged)template).ViewChanged -= new EventHandler<TemplateViewChangedEventArgs>(ActionContainersManager_ViewChanged);
				}
				template = value;
				if(template != null && template is ISupportViewChanged) {
					((ISupportViewChanged)template).ViewChanged += new EventHandler<TemplateViewChangedEventArgs>(ActionContainersManager_ViewChanged);
				}
			}
		}
		private void ActionContainersManager_ViewChanged(object sender, TemplateViewChangedEventArgs e) {
			if(view != null) {
				view.ControlsCreated -= new EventHandler(View_ControlsCreated);
			}
			this.view = e.View;
			if(view != null) {
				view.ControlsCreated += new EventHandler(View_ControlsCreated);
				if(view.IsControlCreated) {
					CreateContextMenu(template, view);
				}
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			View view = (View)sender;
			CreateContextMenu(template, view);
		}
		private void CreateContextMenu(Control parent, View context) {
			if(parent is IFrameTemplate) {
				ListView listView = context as ListView;
				if(listView != null && listView.Editor.ContextMenuTemplate != null) {
					CustomizeActionContainersEventArgs args = new CustomizeActionContainersEventArgs(new List<IActionContainer>(ContextMenuContainers));
					OnCustomizeContextMenuActionContainers(args);
					if(args.ActionContainers.Count > 0) {
						listView.Editor.ContextMenuTemplate.CreateActionItems((IFrameTemplate)parent, listView, args.ActionContainers);
					}
				}
			}
		}
		private void OnCustomizeActionContainers(CustomizeActionContainersEventArgs args) {
			if(CustomizeActionContainers != null) {
				CustomizeActionContainers(this, args);
			}
		}
		private void OnCustomizeContextMenuActionContainers(CustomizeActionContainersEventArgs args) {
			if(CustomizeContextMenuActionContainers != null) {
				CustomizeContextMenuActionContainers(this, args);
			}
		}
		public event EventHandler<CustomizeActionContainersEventArgs> CustomizeActionContainers;
		public event EventHandler<CustomizeActionContainersEventArgs> CustomizeContextMenuActionContainers;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
				Template = null;
			}
			base.Dispose(disposing);
		}
	}
	public class CustomizeActionContainersEventArgs : EventArgs {
		private List<IActionContainer> actionContainers;
		public CustomizeActionContainersEventArgs(List<IActionContainer> actionContainers) {
			this.actionContainers = actionContainers;
		}
		public List<IActionContainer> ActionContainers {
			get { return actionContainers; }
		}
	}
	[AttributeUsage(AttributeTargets.Property)]
	public class CollectionDataSourceProperty : Attribute {
		private string propertyName;
		public CollectionDataSourceProperty(string propertyName) {
			this.propertyName = propertyName;
		}
		public string PropertyName {
			get { return propertyName; }
		}
	}
}
