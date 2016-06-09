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
using System.Windows;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.LayoutControl;
namespace DevExpress.Xpf.LayoutControl.Design {
	using LayoutControl = Platform::DevExpress.Xpf.LayoutControl.LayoutControl;
	abstract class ContentControlParentAdapterBase : ParentAdapter {
		public override bool CanParent(ModelItem parent, Type childType) {
			return base.CanParent(parent, childType) && !ControlType.IsAssignableFrom(childType);
		}
		public override void Parent(ModelItem newParent, ModelItem child) {
			child.ResetLayout();
			newParent.Properties["Content"].SetValue(child);
		}
		public override void RemoveParent(ModelItem currentParent, ModelItem newParent, ModelItem child) {
			currentParent.Properties["Content"].ClearValue();
		}
		protected abstract Type ControlType { get; }
	}
	class GroupBoxParentAdapter : ContentControlParentAdapterBase {
		protected override Type ControlType { get { return typeof(GroupBox); } }
	}
	class LayoutGroupParentAdapter : ParentAdapter {
		const string DefaultTabHeader = "Tab";
		public static string GetDefaultGroupHeader(ModelItem layoutGroup) {
			return "LayoutGroup";
		}
		public static void InitializeChild(ModelItem parent, ModelItem child) {
			if (parent.IsTabbedLayoutGroup())
				if (child.IsLayoutGroup())
					child.Properties["Header"].SetValueIfNotSet(DefaultTabHeader);
				else
					child.Properties[new PropertyIdentifier(typeof(LayoutControl), "TabHeader")].SetValueIfNotSet(DefaultTabHeader);
			else
				if (child.IsEmptyLayoutGroup()) {
					child.Properties["Header"].SetValueIfNotSet(GetDefaultGroupHeader(child));
					child.Properties["View"].SetValueIfNotSet(LayoutGroupView.GroupBox);
				}
		}
		public override void Parent(ModelItem newParent, ModelItem child) {
			ModelItem originalChild = null;
			if (WrapChildWithLayoutItem(child.ItemType)) {
				ModelItem layoutItem = ModelFactory.CreateItem(newParent.Context, typeof(LayoutItem),
					CreateOptions.InitializeDefaults, null);
				originalChild = child;
				child = layoutItem;
			}
			InitializeChild(newParent, child);
			newParent.Properties["Children"].Collection.Add(child);
			if (originalChild != null)
				ModelParent.Parent(newParent.Context, child, originalChild);
		}
		public override void RemoveParent(ModelItem currentParent, ModelItem newParent, ModelItem child) {
			currentParent.Properties["Children"].Collection.Remove(child);
		}
		private readonly Type[] SupportedChildTypesForLayoutItem = new Type[] {
			typeof(Platform::System.Windows.Controls.TextBox),
			typeof(Platform::System.Windows.Controls.ComboBox),
			typeof(Platform::System.Windows.Controls.ListBox),
			typeof(Platform::System.Windows.Controls.PasswordBox),
			typeof(Platform::System.Windows.Controls.Slider),
			typeof(Platform::DevExpress.Xpf.Editors.BaseEdit)
		};
		private readonly Type[] UnsupportedChildTypesForLayoutItem = new Type[] { 
			typeof(Platform::DevExpress.Xpf.Editors.CheckEdit)
		};
		private bool WrapChildWithLayoutItem(Type childType) {
			foreach (Type supportedType in SupportedChildTypesForLayoutItem)
				if (supportedType.IsAssignableFrom(childType)) {
					foreach (Type unsupportedType in UnsupportedChildTypesForLayoutItem)
						if (unsupportedType.IsAssignableFrom(childType))
							return false;
					return true;
				}
			return false;
		}
	}
	class LayoutItemParentAdapter : ContentControlParentAdapterBase {
		public override bool CanParent(ModelItem parent, Type childType) {
			return base.CanParent(parent, childType) && childType.IsSubclassOf(typeof(Platform::System.Windows.UIElement));
		}
		protected override Type ControlType { get { return typeof(LayoutItem); } }
	}
	class TileParentAdapter : ContentControlParentAdapterBase {
		protected override Type ControlType { get { return typeof(Tile); } }
	}
	class DisabledParentAdapter : ParentAdapter {
		public override bool CanParent(ModelItem parent, Type childType) {
			return false;
		}
		public override void Parent(ModelItem newParent, ModelItem child) { }
		public override void RemoveParent(ModelItem currentParent, ModelItem newParent, ModelItem child) { }
	}
	class SimplePlacementAdapter : PlacementAdapter {
		public override bool CanSetPosition(PlacementIntent intent, RelativePosition position) {
			return false;
		}
		public override RelativeValueCollection GetPlacement(ModelItem item, params RelativePosition[] positions) {
			return new RelativeValueCollection();
		}
		public override Rect GetPlacementBoundary(ModelItem item) {
			return new Rect();
		}
		public override Rect GetPlacementBoundary(ModelItem item, PlacementIntent intent, params RelativeValue[] positions) {
			return new Rect();
		}
		public override void SetPlacements(ModelItem item, PlacementIntent intent, params RelativeValue[] positions) { }
		public override void SetPlacements(ModelItem item, PlacementIntent intent, RelativeValueCollection placement) { }
	}
}
