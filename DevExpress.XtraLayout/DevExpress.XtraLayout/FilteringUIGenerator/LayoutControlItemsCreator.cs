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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.Utils;
using DevExpress.Utils.Filtering;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraLayout {
	public class LayoutControlItemsCreator : BaseControlCreator {
		LayoutControl layoutControl = null;
		public LayoutControlItemsCreator(LayoutControl layoutControl) {
			this.layoutControl = layoutControl;
		}
		public virtual void CreateLayout(ElementsBindingInfo bi) {
			List<ElementBindingInfo> list = bi.GetAllBindings();
			if(layoutControl.Root != null) layoutControl.Root.StartChange();
			layoutControl.BeginUpdate();
			EnsureExistingPlacementConfiguration();
			for(int i = 0; i < list.Count; i++) {
				ElementBindingInfo bindingInfo = list[i];
				if(bindingInfo.InnerLayoutElementsBindingInfo != null) {
					CreateInnerLayoutGroup(bindingInfo);
					continue;
				}
				else EnsurePlacementRoot();
				Binding binding = null;
				LayoutControlItem lci = GetLayoutItemByBindingInfo(bindingInfo, out binding);
				if(lci != null) {
					Control control = lci.Control;
					if(bindingInfo.EditorType != control.GetType()) {
						Control newControl = CreateBindableControl(bindingInfo);
						lci.BeginInit();
						lci.Control = newControl;
						lci.EndInit();
						try {
							control.Dispose();
						}
						catch {
						}
						finally {
						}
						CheckItemVisibility(lci, bindingInfo);
						continue;
					}
					if(bindingInfo.BoundPropertyName != binding.PropertyName) {
						control.DataBindings.Clear();
						Binding bindingToSet = GetBinding(control, bindingInfo);
						control.DataBindings.Add(bindingToSet);
						CheckItemVisibility(lci, bindingInfo);
						continue;
					}
					CheckItemVisibility(lci, bindingInfo);
				}
				else {
					if(((ILayoutControl)layoutControl).DesignMode && !bindingInfo.Visible) {
					}
					else
						lci = CreateLayoutElement(bindingInfo, bi.ColumnCount);
				}
			}
			layoutControl.EndUpdate();
			if(layoutControl.Root != null) layoutControl.Root.EndChange();
		}
		void CreateInnerLayoutGroup(ElementBindingInfo bindingInfo) {
			EnsurePlacementRoot();
			LayoutGroup parentGroup = placementRoot;
			string parentName = placementRoot.Name;
			placementRoot.Name = "parent" + placementRoot.Name;
			if(!String.IsNullOrEmpty(bindingInfo.GroupName)) {
				DevExpress.XtraDataLayout.AnnotationAttributesGroupName groupNameHelper = new DevExpress.XtraDataLayout.AnnotationAttributesGroupName(bindingInfo.GroupName);
				LayoutGroup group = groupNameHelper.GenerateGroupIfNeeded(placementRoot);
				placementRoot = group;
				string tooltip = AnnotationAttributes.GetColumnDescription(bindingInfo.AnnotationAttributes);
				if(!string.IsNullOrEmpty(tooltip)) {
					group.OptionsToolTip.ToolTip = tooltip;
				}
				string displayName = AnnotationAttributes.GetColumnCaption(bindingInfo.AnnotationAttributes);
				if(!string.IsNullOrEmpty(displayName)) {
					group.Text = displayName;
				}
			}
			else {
				placementRoot = placementRoot.AddGroup();
				placementRoot.Name = "innerAutoGeneratedGroup" + groupCounter;
				placementRoot.Text = bindingInfo.Caption;
				groupCounter++;
			}
			CreateLayout(bindingInfo.InnerLayoutElementsBindingInfo);
			placementRoot = parentGroup;
			placementRoot.Name = parentName;
		}
		protected void CheckItemVisibility(LayoutControlItem lci, ElementBindingInfo elementBi) {
			if(lci.IsHidden && elementBi.Visible) lci.RestoreFromCustomization();
			if((!lci.IsHidden) && (!elementBi.Visible)) lci.HideToCustomization();
		}
		protected virtual LayoutControlItem GetLayoutItemByBindingInfo(ElementBindingInfo info, out Binding outBinding) {
			foreach(BaseLayoutItem item in layoutControl.Items) {
				LayoutControlItem lci = item as LayoutControlItem;
				if(lci != null && lci.Control != null) {
					Control control = lci.Control;
					foreach(Binding binding in control.DataBindings) {
						if(binding.DataSource == DataSource) {
							if(info.DataMember == binding.BindingMemberInfo.BindingMember) {
								outBinding = binding;
								return lci;
							}
						}
					}
				}
			}
			outBinding = null;
			return null;
		}
		Binding GetBinding(Control control, ElementBindingInfo elementBi) {
			return new Binding(elementBi.BoundPropertyName, DataSource, elementBi.DataMember, true, elementBi.DataSourceUpdateMode);
		}
		protected virtual Control CreateBindableControl(ElementBindingInfo elementBi) {
			if(layoutControl == null) return null;
			if(layoutControl.Site != null)
				return CreateBindableControlDesignTime(elementBi, layoutControl.Site.Container);
			else
				return CreateBindableControlRunTime(elementBi);
		}
		protected virtual LayoutControlItem CreateLayoutItem(ElementBindingInfo elementBi, Control control, LayoutControl dataLayout) {
			LayoutControlItem item = dataLayout.CreateLayoutItem(null) as LayoutControlItem;
			item.Owner = dataLayout;
			dataLayout.BeginInit();
			item.Control = control;
			item.AppearanceItemCaption.FontStyleDelta = FontStyle.Bold;
			item.TextLocation = DevExpress.Utils.Locations.Top;
			item.Owner = null;
			item.Text = elementBi.Caption;
			item.Name = "ItemFor" + elementBi.DataMember;
			Helpers.DesignTimeHelper.Default.AddElementToDTSurface(item, dataLayout.Site);
			try {
				if(item.Site != null) item.Site.Name = item.Name;
			}
			catch { }
			dataLayout.EndInit();
			return item;
		}
		protected virtual void PlaceItemIntoLayout(LayoutControlItem item, ElementBindingInfo bindingInfo) {
			if(placementRoot == null) EnsurePlacementRoot();
			PlaceItemIntoLayoutByAnnotationAttributes(item, bindingInfo);
			if(item.MaxSize == Size.Empty) item.StartNewLine = true;
		}
		void PlaceItemIntoLayoutByAnnotationAttributes(LayoutControlItem item, ElementBindingInfo bindingInfo) {
			item.itemAnnotationAttributes = bindingInfo.AnnotationAttributes;
			if(AnnotationAttributes.ShouldHideFieldLabel(bindingInfo.AnnotationAttributes)) 
				item.TextVisible = false;
			if(bindingInfo.GroupName != bindingInfo.AnnotationAttributes.GroupName) item.TextVisible = false;
			string tooltip = AnnotationAttributes.GetColumnDescription(bindingInfo.AnnotationAttributes);
			if(!string.IsNullOrEmpty(tooltip)) {
				item.OptionsToolTip.ToolTip = tooltip;
			}
			if((bindingInfo.AnnotationAttributes == null && bindingInfo.GroupName == null) || placementRoot.LayoutMode == LayoutMode.Flow) {
				placementRoot.AddItem(item);
				return;
			}
			if(!String.IsNullOrEmpty(bindingInfo.GroupName)) {
				DevExpress.XtraDataLayout.AnnotationAttributesGroupName groupNameHelper = new DevExpress.XtraDataLayout.AnnotationAttributesGroupName(bindingInfo.GroupName);
				LayoutGroup group = groupNameHelper.GenerateGroupIfNeeded(placementRoot);
				if(group == null) placementRoot.AddItem(item);
				else group.AddItem(item);
				return;
			}
			placementRoot.AddItem(item);
		}
		int currentGroupCapacity = 0;
		int groupCounter;
		LayoutGroup placementRoot = null;
		protected virtual int GetCapacityThreshold() { return 200; }
		protected virtual LayoutGroup GetPlacementRoot() {
			return placementRoot;
		}
		protected void EnsurePlacementRoot() {
			if(++currentGroupCapacity > GetCapacityThreshold()) {
				currentGroupCapacity = 0;
				placementRoot = null;
			}
			if(placementRoot == null) {
				placementRoot = layoutControl.Root.AddGroup();
				placementRoot.Name = "autoGeneratedGroup" + groupCounter;
				placementRoot.GroupBordersVisible = false;
				placementRoot.AllowDrawBackground = false;
				EnsureCellSize(placementRoot);
				groupCounter++;
			}
		}
		protected void EnsureCellSize(LayoutGroup group) {
			if(group.CellSize.Width == 0 || group.CellSize.Height == 0) {
				placementRoot.CellSize = new Size(100, 20);
			}
		}
		protected void EnsureExistingPlacementConfiguration() {
			foreach(BaseLayoutItem item in layoutControl.Items) {
				LayoutControlItem cItem = item as LayoutControlItem;
				if(placementRoot != null && cItem != null)
					currentGroupCapacity++;
				LayoutGroup group = item as LayoutGroup;
				if(group != null && group.Name.StartsWith("autoGeneratedGroup")) {
					groupCounter++;
					placementRoot = group;
					EnsureCellSize(placementRoot);
					currentGroupCapacity = 0;
				}
			}
		}
		protected virtual void AddItemToHiddenList(LayoutControlItem lci) {
		}
		protected virtual LayoutControlItem CreateLayoutElement(ElementBindingInfo bindingInfo, int CellCount) {
			Control control = CreateBindableControl(bindingInfo);
			LayoutControlItem layoutItem = CreateLayoutItem(bindingInfo, control, layoutControl);
			if(bindingInfo.Visible)
				PlaceItemIntoLayout(layoutItem, bindingInfo);
			else
				AddItemToHiddenList(layoutItem);
			return layoutItem;
		}
		public override void AddItemToGroup(IEditorGeneratorItemWrapper item, IEditorGeneratorGroupWrapper group) {
		}
		public override IEditorGeneratorGroupWrapper CreateGroup(ElementBindingInfo info) {
			return null;
		}
		public override IEditorGeneratorItemWrapper CreateItem(ElementBindingInfo info) {
			return null;
		}
	}
}
