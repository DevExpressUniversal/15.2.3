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

using DevExpress.Data.Browsing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Entity.Model;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Mvvm.UI.Native.ViewGenerator {
	static class OrientationExtensions {
		public static Orientation OrthogonalValue(this Orientation value) {
			return value == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
		}
	}
	public interface ILayoutElementFactory {
		void CreateGroup(LayoutGroupInfo groupInfo);
		void CreateItem(IEdmPropertyInfo propertyInfo);
	}
	public interface ILayoutElementGenerator {
		void CreateElement(ILayoutElementFactory factory);
	}
	public class LayoutItemInfo : ILayoutElementGenerator {
		readonly IEdmPropertyInfo property;
		public LayoutItemInfo(IEdmPropertyInfo property) {
			this.property = property;
		}
		void ILayoutElementGenerator.CreateElement(ILayoutElementFactory factory) {
			factory.CreateItem(property);
		}
	}
	public class LayoutGroupInfo : ILayoutElementGenerator {
		public LayoutGroupInfo(string name, GroupView? view, Orientation? orientation) {
			Children = new List<ILayoutElementGenerator>();
			Name = name;
			View = view;
			Orientation = orientation;
		}
		public LayoutGroupInfo GetGroupInfo(string path) {
			if(string.IsNullOrEmpty(path))
				return this;
			LayoutGroupInfo result = this;
			string[] descriptions = GetGroupDescriptions(path);
			for(int i = 0; i < descriptions.Length; i++) {
				string name;
				GroupView? view;
				Orientation? orientation;
				GetGroupParameters(descriptions[i], out name, out view, out orientation);
				result = result.FindOrCreateGroupInfo(name, view, orientation);
			}
			return result;
		}
		public Orientation GetOrientation() {
			return Orientation ?? GetDefaultOrientation();
		}
		public GroupView GetView() {
			return View ?? GetDefaultView();
		}
		public static string[] GetGroupDescriptions(string path) {
			return path.Split(new char[] { LayoutGroupInfoConstants.GroupPathSeparator }, StringSplitOptions.RemoveEmptyEntries);
		}
		public List<ILayoutElementGenerator> Children { get; private set; }
		public string Name { get; private set; }
		public Orientation? Orientation { get; private set; }
		public LayoutGroupInfo Parent { get; private set; }
		public GroupView? View { get; private set; }
		protected LayoutGroupInfo CreateGroupInfo(string name, GroupView? view, Orientation? orientation) {
			ConstructorInfo constructor = GetType().GetConstructor(new Type[] { typeof(string), typeof(GroupView?), typeof(Orientation?) });
			return (LayoutGroupInfo)constructor.Invoke(new object[] { name, view, orientation });
		}
		protected LayoutGroupInfo FindGroupInfo(string name) {
			return (LayoutGroupInfo)Children.FirstOrDefault(c => c is LayoutGroupInfo && ((LayoutGroupInfo)c).Name == name);
		}
		protected LayoutGroupInfo FindOrCreateGroupInfo(string name, GroupView? view, Orientation? orientation) {
			LayoutGroupInfo result = FindGroupInfo(name);
			if(result == null) {
				result = CreateGroupInfo(name, view, orientation);
				result.Parent = this;
				Children.Add(result);
			}
			return result;
		}
		protected virtual Orientation GetDefaultOrientation() {
			if(GetView() == GroupView.Group && Parent.GetView() != GroupView.Tabs)
				return Parent.GetOrientation().OrthogonalValue();
			return System.Windows.Controls.Orientation.Vertical;
		}
		protected virtual GroupView GetDefaultView() {
			if(Parent.GetView() == GroupView.Tabs)
				return GroupView.Group;
			return GroupView.GroupBox;
		}
		protected virtual Orientation? GetGroupOrientation(ref string description) {
			Orientation? result = null;
			if(description.Length != 0) {
				char end = description[description.Length - 1];
				if(end == LayoutGroupInfoConstants.HorizontalGroupMark)
					result = System.Windows.Controls.Orientation.Horizontal;
				else
					if(end == LayoutGroupInfoConstants.VerticalGroupMark)
						result = System.Windows.Controls.Orientation.Vertical;
			}
			if(result != null)
				description = description.Substring(0, description.Length - 1);
			return result;
		}
		protected virtual void GetGroupParameters(string description, out string name, out GroupView? view, out Orientation? orientation) {
			description = description.Trim();
			view = GetGroupView(ref description);
			orientation = GetGroupOrientation(ref description);
			name = description;
		}
		protected virtual GroupView? GetGroupView(ref string description) {
			GroupView? result = null;
			if(description.Length >= 2) {
				char start = description[0];
				char end = description[description.Length - 1];
				if(start == LayoutGroupInfoConstants.BorderlessGroupMarkStart && end == LayoutGroupInfoConstants.BorderlessGroupMarkEnd)
					result = GroupView.Group;
				else
					if(start == LayoutGroupInfoConstants.GroupBoxMarkStart && end == LayoutGroupInfoConstants.GroupBoxMarkEnd)
						result = GroupView.GroupBox;
					else
						if(start == LayoutGroupInfoConstants.TabbedGroupMarkStart && end == LayoutGroupInfoConstants.TabbedGroupMarkEnd)
							result = GroupView.Tabs;
			}
			if(result != null)
				description = description.Substring(1, description.Length - 2);
			return result;
		}
		void ILayoutElementGenerator.CreateElement(ILayoutElementFactory factory) {
			factory.CreateGroup(this);
		}
	}
	public interface IGroupGenerator {
		void ApplyGroupInfo(string name, GroupView view, Orientation orientation);
		EditorsGeneratorBase EditorsGenerator { get; }
		IGroupGenerator CreateNestedGroup(string name, GroupView view, Orientation orientation);
		void OnAfterGenerateContent();
	}
}
