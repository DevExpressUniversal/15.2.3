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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Filtering;
using DevExpress.XtraEditors.Filtering;
namespace DevExpress.XtraBars.Navigation {
	public class AccordionControlLayoutCreator : BaseControlCreator {
		AccordionControl accordionCore;
		AccordionControlElement defaultElement;
		public override void BeginCreateLayout() {
			accordionCore.BeginUpdate();
		}
		public override void EndCreateLayout() {
			accordionCore.EndUpdate();
		}
		public override void BestSize() {
			BestSizeCore(accordionCore);
		}
		void BestSizeCore(Control control) {
			foreach(Control item in control.Controls) {
				if(item is AccordionContentContainer) {
					item.Visible = true;
					int minHeight = 10;
					foreach(Control ctrl in item.Controls) {
						ctrl.Visible = true;
						var xtraResizeControl = ctrl as IXtraResizableControl;
						if(xtraResizeControl != null)
							minHeight = Math.Max(minHeight, xtraResizeControl.MinSize.Height);
					}
					item.Height = minHeight + item.Padding.Vertical;
				}
				BestSizeCore(item);
			}
		}
		public AccordionControlLayoutCreator(AccordionControl accordion) {
			accordionCore = accordion;
		}
		public override void AddItemToGroup(IEditorGeneratorItemWrapper item, IEditorGeneratorGroupWrapper group) {
			AccordionControlElement element = item.Item as AccordionControlElement;
			AccordionControlElement layoutGroup = group.Group as AccordionControlElement;
			layoutGroup.Elements.Add(element);
		}
		bool CanShowCaption(ElementBindingInfo info) {
			if(info.AnnotationAttributes == null) return true;
			return !AnnotationAttributes.ShouldHideFieldLabel(info.AnnotationAttributes);
		}
		public override IEditorGeneratorGroupWrapper CreateGroup(ElementBindingInfo info) {
			return new EditorGeneratorWrapper() { Group = CreateGroupCore(info) };
		}
		IContainer Container {
			get {
				return (accordionCore != null) && (accordionCore.Site != null) && accordionCore.Site.DesignMode ?
					accordionCore.Site.Container : null;
			}
		}
		public override IEditorGeneratorItemWrapper CreateItem(ElementBindingInfo info) {
			var control = CreateBindableControl(info, Container);
			AccordionControlElement element = new AccordionControlElement() { Style = ElementStyle.Item, Expanded = true };
			AddToContainer(element);
			element.HeaderVisible = false;
			if(info.AnnotationAttributes != null && !string.IsNullOrEmpty(info.AnnotationAttributes.ShortName))
				element.Text = info.AnnotationAttributes.ShortName;
			else
				element.Text = info.Caption;
			element.ContentContainer = new AccordionContentContainer() { Padding = new Padding(-1) };
			AddToContainer(element.ContentContainer);
			element.ContentContainer.Parent = accordionCore;
			var xtraResizableControl = control as IXtraResizableControl;
			if(xtraResizableControl != null && xtraResizableControl.IsCaptionVisible)
				element.HeaderVisible = true;
			else
				element.HeaderVisible = CanShowCaption(info);
			control.Dock = DockStyle.Fill;
			control.Parent = element.ContentContainer;
			return new EditorGeneratorItemWrapper() { Control = control, Info = info, Item = element };
		}
		protected virtual AccordionControlElement CreateGroupCore(ElementBindingInfo bindingInfo) {
			string caption = AnnotationAttributes.GetColumnCaption(bindingInfo.AnnotationAttributes);
			string tooltip = AnnotationAttributes.GetColumnDescription(bindingInfo.AnnotationAttributes);
			if(!String.IsNullOrEmpty(bindingInfo.GroupName)) {
				AnnotationAttributesGroupName groupNameHelper = new AnnotationAttributesGroupName(bindingInfo.GroupName);
				string key = GetFullKey(groupNameHelper);
				if(groupNameHelper.ChildGroup != null)
					GenerateChildGroup(groupNameHelper, accordionCore.Elements);
				var item = accordionCore.Elements.FirstOrDefault(e => e.Name == key);
				if(item == null)
					item = GetElement(key, accordionCore.Elements);
				if(item != null)
					return item;
				AccordionControlElement group = CreateAccordionControlGroup(groupNameHelper.GroupName, groupNameHelper.GroupName);
				accordionCore.Elements.Add(group);
				return group;
			}
			else {
				CreateDefaultGroup();
			}
			return defaultElement;
		}
		AccordionControlElement CreateAccordionControlGroup(string name, string text, bool headerVisible = true) {
			AccordionControlElement group = new AccordionControlElement() { Style = ElementStyle.Group };
			AddToContainer(group, name);
			group.Name = name;
			group.Text = text;
			group.HeaderVisible = headerVisible;
			return group;
		}
		void AddToContainer(IComponent component, string name = "") {
			if(Container != null)
				Container.Add(component);
			try {
				if(component.Site != null && !string.IsNullOrEmpty(name))
					component.Site.Name = name;
			}
			catch { }
		}
		void CreateDefaultGroup() {
			defaultElement = new AccordionControlElement()
			{
				Style = ElementStyle.Group,
				HeaderVisible = false
			};
			accordionCore.Elements.Add(defaultElement);
			AddToContainer(defaultElement);
		}
		AccordionControlElement GetElement(string key, AccordionControlElementCollection elements) {
			var item = elements.FirstOrDefault(e => e.Name == key);
			if(item != null) return item;
			foreach(var itemCore in elements) {
				var result = GetElement(key, itemCore.Elements);
				if(result != null)
					return result;
			}
			return null;
		}
		string GetFullKey(AnnotationAttributesGroupName groupNameHelper) {
			if(groupNameHelper == null) return string.Empty;
			return groupNameHelper.GroupName + GetFullKey(groupNameHelper.ChildGroup);
		}
		void GenerateChildGroup(AnnotationAttributesGroupName groupName, AccordionControlElementCollection elements, string parentGroupName = "") {
			if(groupName.ChildGroup == null) 
				elements.Add(CreateAccordionControlGroup(parentGroupName + groupName.GroupName, groupName.GroupName));
			else {
				string parentGroups = parentGroupName + groupName.GroupName;
				AccordionControlElement parentGroup = elements.FirstOrDefault(e => e.Name == parentGroups);
				if(parentGroup == null) {
					parentGroup = new AccordionControlElement()
					{
						Style = ElementStyle.Group
					};
					AddToContainer(parentGroup, parentGroups);
					parentGroup.Name = parentGroups;
					parentGroup.Text = groupName.GroupName;
					elements.Add(parentGroup);
				}
				if(groupName.ChildGroup != null)
					GenerateChildGroup(groupName.ChildGroup, parentGroup.Elements, parentGroup.Name);
			}
		}
	}
}
