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
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.Controls;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Templates {
	public class XtraBarsCustomizationNodeGenerator : ModelNodesGeneratorBase {
		public const string DefaultCustomizationId = "Default";
		private string nodeId;
		protected override void GenerateNodesCore(ModelNode node) {
			if(!XafBarManager.UseBarsModel) {
				return;
			}
			if(node[DefaultCustomizationId] == null) {
				ITypeInfo templateTypeInfo = XafTypesInfo.Instance.FindTypeInfo(GetTemplateType(node));
				if(templateTypeInfo == null) {
					throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.TemplateTypeInfoMissed, GetTemplateType(node)));
				}
				IModelTemplateXtraBarsCustomization modelTemplateXtraBarsCustomization = GenerateDefaultXtraBarsCustomization(node, templateTypeInfo);
				if(Generated != null) {
					Generated(this, new XtraBarsCustomizationGeneratedEventArgs(modelTemplateXtraBarsCustomization, templateTypeInfo));
				}
			}
			if(node[nodeId] == null) {
				node[DefaultCustomizationId].Clone(nodeId);
			}
		}
		private string GetTemplateType(ModelNode node) {
			return node.Parent.Id;
		}
		private IModelTemplateXtraBarsCustomization GenerateDefaultXtraBarsCustomization(ModelNode node, ITypeInfo typeInfo) {
			IModelTemplateXtraBarsCustomization barCustomizationNode = node.AddNode<IModelTemplateXtraBarsCustomization>(DefaultCustomizationId);
			Object template = typeInfo.CreateInstance();
			if(template is IBarManagerHolder) {
				BarManager manager = ((IBarManagerHolder)template).BarManager;
				if(manager != null) {
					manager.ForceLinkCreate();
					BarManagerModelSynchronizer barManagerModelSynchronizer = new BarManagerModelSynchronizer(manager, barCustomizationNode);
					barManagerModelSynchronizer.WriteCustomization(barCustomizationNode);
					barManagerModelSynchronizer.Dispose();
					barManagerModelSynchronizer = null;
				}
			}
			if(template is IDisposable) {
				((IDisposable)template).Dispose();
			}
			template = null;
			return barCustomizationNode;
		}
		public XtraBarsCustomizationNodeGenerator(string nodeId)
			: base() {
			this.nodeId = nodeId;
		}
		public static event EventHandler<XtraBarsCustomizationGeneratedEventArgs> Generated;
	}
	public class XtraBarsCustomizationGeneratedEventArgs : EventArgs {
		public XtraBarsCustomizationGeneratedEventArgs(IModelTemplateXtraBarsCustomization node, ITypeInfo templateTypeInfo) {
			this.TemplateTypeInfo = templateTypeInfo;
			this.Node = node;
		}
		public IModelTemplateXtraBarsCustomization Node { get; private set; }
		public ITypeInfo TemplateTypeInfo { get; private set; }
	}
	public class XtraBarsModelSynchronizer : ModelSynchronizer<BarManager, IModelTemplateXtraBarsCustomization> {
		internal BarManagerModelSynchronizer modelSynchronizer;
		public XtraBarsModelSynchronizer(BarManager control, IModelTemplateXtraBarsCustomization model)
			: base(control, model) {
			modelSynchronizer = new BarManagerModelSynchronizer(Control, Model);
		}
		protected override void ApplyModelCore() {
			modelSynchronizer.ApplyModel();
		}
		public override void SynchronizeModel() {
			modelSynchronizer.SynchronizeModel();
		}
		public override void Dispose() {
			if(modelSynchronizer != null) {
				modelSynchronizer.Dispose();
				modelSynchronizer = null;
			}
		}
	}
	public class BarManagerModelSynchronizer : ModelSynchronizer<BarManager, IModelTemplateXtraBarsCustomization> {
		private void WriteLinksCustomization(IModelTemplateBarItemBase parent, BarItemLinkCollection links) {
			foreach(BarItemLink link in links) {
				IModelTemplateBarItem linkNode = parent[link.Item.Name];
				if(linkNode == null) {
					if(String.IsNullOrEmpty(link.Item.Name)) {
						string result = String.Format("The validation error has been occured. The Name property of the '{0}' object isn't set. A customized model cannot be saved correctly. Please fix the following Bar item: '{1}'" + Environment.NewLine +
							"Caption: '{2}'" + Environment.NewLine +
							"OwnerItem.Caption: '{3}'",
								link.GetType().FullName,
								link.Bar != null ? link.Bar.BarName : "",
								link.Item.Caption,
								link.OwnerItem != null ? link.OwnerItem.Caption : "");
						throw new WarningException(result);
					}
					linkNode = parent.AddNode<IModelTemplateBarItem>(link.Item.Name);
				}
				linkNode.Index = links.IndexOf(link);
				if(link.UserWidth != 0) {
					linkNode.Width = link.UserWidth;
				}
				linkNode.IsVisible = link.Visible;
				if((link.Item is BarLinksHolder) && !(link.Item is BarLinkContainerExItem)) {
					WriteLinksCustomization(linkNode, ((BarLinksHolder)link.Item).ItemLinks);
				}
			}
		}
		private BarItemLink FindLinkByItemName(BarItemLinkCollection links, string itemName) {
			foreach(BarItemLink link in links) {
				if(link.Item.Name == itemName) {
					return link;
				}
			}
			return null;
		}
		private BarItemLink TryToAddLink(BarLinksHolder barLinksHolder, IModelTemplateBarItem linkDiffs) {
			BarItem desiredItem = null;
			foreach(BarItem item in barLinksHolder.Manager.Items) {
				if(item.Name == linkDiffs.Id) {
					desiredItem = item;
					break;
				}
			}
			if(desiredItem != null) {
				BarItemLink result = barLinksHolder.AddItem(desiredItem);
				if(linkDiffs.Width > -1) {
					result.Width = linkDiffs.Width;
				}
				return result;
			}
			return null;
		}
		private void ApplyCustomization(BarLinksHolder barLinksHolder, IModelTemplateBarItemBase diffs) {
			foreach(IModelTemplateBarItem linkDiffs in diffs) {
				BarItemLink link = FindLinkByItemName(barLinksHolder.ItemLinks, linkDiffs.Id);
				if(linkDiffs.IsDeleted) {
					if(link != null)
						barLinksHolder.ItemLinks.Remove(link);
				}
				else {
					if((link == null) && linkDiffs.IsNew) {
						link = TryToAddLink(barLinksHolder, linkDiffs);
					}
					if(link != null) {
						BarItem item = link.Item;
						int index = (linkDiffs.Index.HasValue && linkDiffs.Index.Value > -1) ? linkDiffs.Index.Value : barLinksHolder.ItemLinks.IndexOf(link);
						if(index != barLinksHolder.ItemLinks.IndexOf(link)) {
							barLinksHolder.ItemLinks.Remove(link);
							link = null; 
							if(index >= barLinksHolder.ItemLinks.Count) {
								link = barLinksHolder.AddItem(item);
							}
							else {
								link = barLinksHolder.ItemLinks.Insert(index, item);
							}
						}
						link.UserWidth = linkDiffs.Width > -1 ? linkDiffs.Width : link.UserWidth;
						link.Visible = linkDiffs.IsVisible;
						if((item is BarLinksHolder) && !(item is BarLinkContainerExItem)) {
							ApplyCustomization((BarLinksHolder)item, linkDiffs);
						}
					}
				}
			}
		}
		private void ApplyCustomization(IModelTemplateXtraBarsCustomization info) {
			Control.ForceInitialize();
			Control.BeginUpdate();
			try {
				Control.LargeIcons = info.LargeIcons;
				foreach(IModelTemplateBar barNode in info) {
					Bar bar = FindBar(barNode.Id);
					if((bar == null) && (barNode.IsNew)) {
						bar = new XafBar(Control);
						bar.BarName = barNode.Id;
						bar.Text = barNode.Id;
						bar.DockStyle = BarDockStyle.Top;
						bar.OptionsBar.AllowDelete = true;
						bar.OptionsBar.AllowRename = true;
					}
					if(bar != null) {
						if(barNode.IsDeleted) {
							Control.Bars.RemoveAt(Control.Bars.IndexOf(bar));
						}
						else {
							bar.Visible = barNode.IsVisible;
							if(barNode.Offset > -1) {
								bar.Offset = barNode.Offset;
							}
							bar.DockStyle = barNode.DockStyle;
							bar.FloatLocation = new System.Drawing.Point(barNode.FloatLocationX, barNode.FloatLocationY);
							ApplyCustomization(bar, barNode);
							bar.DockRow = barNode.DockRow > -1 ? barNode.DockRow : bar.DockRow;
							bar.DockCol = barNode.DockCol > -1 ? barNode.DockCol : bar.DockCol;
						}
					}
				}
				XtraBarAutoHideExtender.Attach(Control);
			}
			finally {
				Control.EndUpdate();
			}
		}
		private Bar FindBar(string barName) {
			foreach(Bar bar in Control.Bars) {
				if(bar.BarName == barName) {
					return bar;
				}
			}
			return null;
		}
		private void SetIsNew(IModelTemplateXtraBarsCustomization customizedModel) {
			IModelTemplateXtraBarsCustomization sourceModel = Model;
			foreach(IModelTemplateBar bar in customizedModel) {
				if(sourceModel[bar.Id] == null || sourceModel[bar.Id].IsNew) {
					bar.IsNew = true;
				}
				foreach(IModelTemplateBarItem barItem in bar) {
					if(sourceModel[bar.Id] == null || sourceModel[bar.Id][barItem.Id] == null || sourceModel[bar.Id][barItem.Id].IsNew) {
						barItem.IsNew = true;
					}
				}
			}
		}
		private void SetIsDeleted(IModelTemplateXtraBarsCustomization customizedModel) {
			IModelTemplateXtraBarsCustomization sourceModel = Model;
			foreach(IModelTemplateBar bar in sourceModel) {
				if(customizedModel[bar.Id] == null) {
					IModelTemplateBar newBar = customizedModel.AddNode<IModelTemplateBar>(bar.Id);
					newBar.IsDeleted = true;
				}
				else {
					bar.IsDeleted = false;
					foreach(IModelTemplateBarItem barItem in bar) {
						if(customizedModel[bar.Id][barItem.Id] == null) {
							IModelTemplateBarItem newBarItem = customizedModel[bar.Id].AddNode<IModelTemplateBarItem>(barItem.Id);
							newBarItem.IsDeleted = true;
						}
						else {
							barItem.IsDeleted = false;
						}
					}
				}
			}
		}
		internal void WriteCustomization(IModelTemplateXtraBarsCustomization target) {
			target.LargeIcons = Control.LargeIcons;
			foreach(Bar bar in Control.Bars) {
				IModelTemplateBar barNode = target[bar.BarName];
				if(barNode == null) {
					barNode = target.AddNode<IModelTemplateBar>(bar.BarName);
				}
				barNode.IsVisible = XtraBarAutoHideExtender.GetIsBarVisibleByUser(bar);
				barNode.DockCol = bar.DockCol;
				barNode.DockRow = bar.DockRow;
				barNode.DockStyle = bar.DockStyle;
				barNode.FloatLocationX = bar.FloatLocation.X;
				barNode.FloatLocationY = bar.FloatLocation.Y;
				barNode.Offset = bar.Offset;
				WriteLinksCustomization(barNode, bar.ItemLinks);
			}
		}
		public BarManagerModelSynchronizer(BarManager control, IModelTemplateXtraBarsCustomization model)
			: base(control, model) {
				Initialize();
		}
		private void Initialize() {
			foreach(Bar bar in Control.Bars) {
				bar.DockChanged += Control_Changed;
			}
		}
		protected override void ApplyModelCore() {
			ApplyCustomization(Model);
		}
		public override void SynchronizeModel() {
			Control.ForceLinkCreate();
			IModelTemplateXtraBarsCustomization customizedModel = (IModelTemplateXtraBarsCustomization)((ModelNode)Model).CreateNode();
			WriteCustomization(customizedModel);
			SetIsNew(customizedModel);
			SetIsDeleted(customizedModel);
			((ModelNode)Model).ApplyDiff((ModelNode)customizedModel);
		}
		public override void Dispose() {
			base.Dispose();
			foreach(Bar bar in Control.Bars) {
				bar.DockChanged -= Control_Changed;
			}
		}
	}
	public class CustomCreateXtraBarAutoHideExtenderEventArgs : EventArgs {
		private XtraBarAutoHideExtender extender;
		public XtraBarAutoHideExtender Extender {
			get { return extender; }
			set { extender = value; }
		}
	}
	public class XtraBarAutoHideExtender {
		private static List<XtraBarAutoHideExtender> extenders = new List<XtraBarAutoHideExtender>();
		private Bar bar;
		private bool isBarVisibleByUser;
		private bool isAutoHiding;
		private static bool canUpdate = true;
		private static void manager_EndCustomization(object sender, EventArgs e) {
			RefreshAttachedExtenders((BarManager)sender);
		}
		private void bar_Disposed(object sender, EventArgs e) {
			extenders.Remove(this);
			if(bar != null) {
				if(bar.Manager != null) {
					bar.Manager.Items.CollectionChanged -= new CollectionChangeEventHandler(Items_CollectionChanged);
					bar.Manager.StartCustomization -= new EventHandler(Manager_StartCustomization);
					bar.Manager.EndCustomization -= new EventHandler(Manager_EndCustomization);
				}
				bar.Disposed -= new EventHandler(bar_Disposed);
				bar.VisibleChanged -= new EventHandler(bar_VisibleChanged);
			}
		}
		private void Items_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(bar == null) {
				return;
			}
			BarItem item = e.Element as BarItem;
			if(item != null &&isBarVisibleByUser) {
				bool isBarContainsLinkToItem = false;
				foreach(BarItemLink link in item.Links) {
					if(link.Bar == bar) {
						isBarContainsLinkToItem = true;
						break;
					}
				}
				if(isBarContainsLinkToItem) {
					UpdateBarVisibility();
				}
			}
		}
		private void bar_VisibleChanged(object sender, EventArgs e) {
			if(bar == null) {
				return;
			}
			if(!isAutoHiding) {
				isBarVisibleByUser = bar.Visible;
			}
		}
		private void Manager_StartCustomization(object sender, EventArgs e) {
			if(bar == null) {
				return;
			}
			bar.Visible = true;
		}
		private void Manager_EndCustomization(object sender, EventArgs e) {
			if(bar == null) {
				return;
			}
			UpdateBarVisibility();
		}
		private static void RefreshAttachedExtenders(BarManager barManager) {
			foreach(Bar bar in barManager.Bars) {
				if(!bar.IsMainMenu && !bar.IsStatusBar) {
					XtraBarAutoHideExtender extender = XtraBarAutoHideExtender.Find(bar);
					if(extender == null) {
						CustomCreateXtraBarAutoHideExtenderEventArgs args = new CustomCreateXtraBarAutoHideExtenderEventArgs();
						args.Extender = new XtraBarAutoHideExtender();
						if(CustomCreateExtender != null) {
							CustomCreateExtender(null, args);
						}
						if(args.Extender != null) {
							args.Extender.Attach(bar);
						}
					}
					else {
						extender.UpdateBarVisibility();
					}
				}
			}
		}
		private static void form_Disposed(object sender, EventArgs e) {
			Form form = (Form)sender;
			form.Disposed -= new EventHandler(form_Disposed);
			form.FormClosing -= new FormClosingEventHandler(form_FormClosing);
			canUpdate = true;
		}
		private static void form_FormClosing(object sender, FormClosingEventArgs e) {
			Form form = (Form)sender;
			IXafDocumentsHostWindow mainForm = form.MdiParent as IXafDocumentsHostWindow;
			Form parentForm = form.MdiParent;
			if(parentForm != null && mainForm != null && mainForm.UIType == UIType.TabbedMDI && mainForm.DocumentManager.View.Documents.Count > 1) {
				canUpdate = false;
			}
		}
		private void UpdateBarVisibility() {
			if(canUpdate) {
				isAutoHiding = true;
				try {
					bar.Visible = isBarVisibleByUser && ContainsVisibleItems(bar);
				}
				finally {
					isAutoHiding = false;
				}
			}
		}
		protected virtual bool ContainsVisibleItems(BarLinksHolder linksHolder) {
			bool isHolderContainsVisibleItems = false;
			foreach(BarItemLink link in linksHolder.ItemLinks) {
				if(link.Item is BarLinksHolder) {
					isHolderContainsVisibleItems |= ContainsVisibleItems((BarLinksHolder)link.Item);
				}
				else {
					isHolderContainsVisibleItems |= (link.Item.Visibility == BarItemVisibility.Always || link.Item.Visibility == BarItemVisibility.OnlyInRuntime);
				}
				if(isHolderContainsVisibleItems) {
					break;
				}
			}
			return isHolderContainsVisibleItems;
		}
		protected virtual void Attach(Bar bar) {
			if(this.bar != null) {
				throw new InvalidOperationException();
			}
			Guard.ArgumentNotNull(bar, "bar");
			this.bar = bar;
			IsBarVisibleByUser = bar.Visible;
			extenders.Add(this);
			bar.Manager.Items.CollectionChanged += new CollectionChangeEventHandler(Items_CollectionChanged);
			bar.Manager.StartCustomization += new EventHandler(Manager_StartCustomization);
			bar.Manager.EndCustomization += new EventHandler(Manager_EndCustomization);
			bar.Disposed += new EventHandler(bar_Disposed);
			bar.VisibleChanged += new EventHandler(bar_VisibleChanged);
			UpdateBarVisibility();
		}
		public Bar Bar {
			get { return bar; }
		}
		public bool IsBarVisibleByUser {
			get { return isBarVisibleByUser; }
			set { isBarVisibleByUser = value; }
		}
		public bool IsAutoHiding {
			get { return isAutoHiding; }
			set { isAutoHiding = value; }
		}
		public static void Attach(BarManager barManager) {
			Form form = barManager.Form as Form;
			if(form != null) {
				form.Disposed += new EventHandler(form_Disposed);
				form.FormClosing += new FormClosingEventHandler(form_FormClosing);
			}
			barManager.EndCustomization -= new EventHandler(manager_EndCustomization);
			barManager.EndCustomization += new EventHandler(manager_EndCustomization);
			RefreshAttachedExtenders(barManager);
		}
		public static XtraBarAutoHideExtender Find(Bar bar) {
			foreach(XtraBarAutoHideExtender extender in extenders) {
				if(extender.bar == bar) {
					return extender;
				}
			}
			return null;
		}
		public static bool GetIsBarVisibleByUser(Bar bar) {
			XtraBarAutoHideExtender extender = XtraBarAutoHideExtender.Find(bar);
			if(extender != null) {
				return extender.IsBarVisibleByUser;
			}
			else {
				return bar.Visible;
			}
		}
		public static event EventHandler<CustomCreateXtraBarAutoHideExtenderEventArgs> CustomCreateExtender;
	}
}
