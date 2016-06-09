#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	internal enum TreeViewDashboardItemType { Standard, Checked }
	[
	DashboardItemType(DashboardItemType.TreeView)
	]
	public class TreeViewDashboardItem : FilterElementDashboardItem {
		const string xmlTreeViewType = "TreeViewType";
		const string xmlAutoExpandNodes = "AutoExpand";
		const TreeViewDashboardItemType DefaultTreeViewType = TreeViewDashboardItemType.Checked;
		const bool DefaultAutoExpandNodes = false;
		TreeViewDashboardItemType treeViewType = DefaultTreeViewType;
		bool autoExpandNodes;
		protected internal override string CaptionPrefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNameTreeViewItem); } }
		[
		DefaultValue(DefaultTreeViewType)
		]
		internal TreeViewDashboardItemType TreeViewType {
			get { return TreeViewDashboardItemType.Checked; }
			set {
				if(treeViewType == value)
					return;
				treeViewType = value;
				OnElementTypeChanged();
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("TreeViewDashboardItemAutoExpandNodes"),
#endif
		DefaultValue(DefaultAutoExpandNodes)
		]
		public bool AutoExpandNodes {
			get { return autoExpandNodes; }
			set {
				if(autoExpandNodes != value) {
					autoExpandNodes = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("TreeViewDashboardItemInteractivityOptions"),
#endif
		Category(CategoryNames.Interactivity),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public FilterableDashboardItemInteractivityOptions InteractivityOptions { get { return InteractivityOptionsBase; } }
		protected internal override bool IsSingleSelection { get { return TreeViewType == TreeViewDashboardItemType.Standard; } }
		protected internal override DashboardItemViewModel CreateViewModel() {
			return new TreeViewDashboardItemViewModel(this);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			XmlHelper.Save(element, xmlTreeViewType, treeViewType, DefaultTreeViewType);
			XmlHelper.Save(element, xmlAutoExpandNodes, autoExpandNodes, DefaultAutoExpandNodes);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			XmlHelper.LoadEnum<TreeViewDashboardItemType>(element, xmlTreeViewType, x => treeViewType = x);
			XmlHelper.Load<bool>(element, xmlAutoExpandNodes, x => autoExpandNodes = x);
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			description.FilterElementType = treeViewType == TreeViewDashboardItemType.Checked ? 
				FilterElementTypeDescription.Checked : 
				FilterElementTypeDescription.Radio;
			return description;
		}
		protected override void AssignFilterElementType(bool multiple) {
			treeViewType = TreeViewDashboardItemType.Checked;
		}
	}
}
