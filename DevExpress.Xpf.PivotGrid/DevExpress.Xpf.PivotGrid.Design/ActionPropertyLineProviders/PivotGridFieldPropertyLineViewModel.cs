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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Core.Design.AssignDataContextDialog;
using DevExpress.Xpf.Core.Design.SmartTags;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.PivotGrid.Design {
	public class PivotGridFieldPropertyLineViewModel : PropertyLineWithPopupEditorViewModel {
		PivotGridWpfData instantData;
		string fieldName;
		string connectionString;
		bool? showDataOnly;
		public PivotGridFieldPropertyLineViewModel(PivotGridWpfData instantData, string fieldName, string connectionString, bool? showDataOnly, IPropertyLineContext context, string propertyName)
			: base(context, propertyName, null, context.PlatformInfoFactory.ForStandardProperty("string")) {
			IsReadOnly = true;
			this.instantData = instantData;
			this.fieldName = fieldName;
			this.showDataOnly = showDataOnly;
			this.connectionString = connectionString;
			this.showDataOnly = showDataOnly;
		}
		protected override PropertyLineWithPopupEditorPopupViewModel CreatePopup() {
			return new PivotGridFieldPropertyLinePopupViewModel(this, GetTreeViewItems);
		}
		IEnumerable<TreeViewItemViewModel> GetTreeViewItems() {
			return OlapFieldsPopulator.GetFields(fieldName, connectionString, instantData, showDataOnly, () => true).WaitResult();
		}
	}
	public class PivotGridFieldPropertyLinePopupViewModel : TreeViewPropertyLinePopupViewModel {
		Func<IEnumerable<TreeViewItemViewModel>> getTreeViewItemsCallback;
		public PivotGridFieldPropertyLinePopupViewModel(PivotGridFieldPropertyLineViewModel propertyLine, Func<IEnumerable<TreeViewItemViewModel>> getTreeViewItemsCallback)
			: base(propertyLine) {
			this.getTreeViewItemsCallback = getTreeViewItemsCallback;
			DataProviderCore = new PivotGridFieldPropertyLinePopupViewModelDataProvider(() => PropertyLine.PropertyValueText, getTreeViewItemsCallback);
		}
		protected override void OnSelectedNodeChanged() {
			base.OnSelectedNodeChanged();
			FieldSelectorItemInfo fieldInfo = SelectedNode as FieldSelectorItemInfo;
			if(fieldInfo == null) return;
			PivotGridDesignTimeHelper.SetField(PropertyLine.SelectedItem, fieldInfo.Field, fieldInfo.Name);
		}
	}
	public class PivotGridFieldPropertyLinePopupViewModelDataProvider : TreeViewPropertyLinePopupViewModelDataProvider {
		Func<IEnumerable<TreeViewItemViewModel>> getTreeViewItemsCallback;
		public PivotGridFieldPropertyLinePopupViewModelDataProvider(Func<string> getDefaultSelectedFieldCallback, Func<IEnumerable<TreeViewItemViewModel>> getTreeViewItemsCallback) {
			this.getTreeViewItemsCallback = getTreeViewItemsCallback;
			UpdateAsyncFunc = AsyncHelper.Create<bool, string>(reset => getDefaultSelectedFieldCallback(), UpdateAsync);
		}
		IEnumerable<ManualResetEvent> UpdateAsync(bool reset, string defaultSelectedField, CancellationToken cancellationToken) {
			if(reset) {
				Root = null;
				return null;
			}
			if(cancellationToken.IsCancellationRequested) return null;
			IEnumerable<TreeViewItemViewModel> treeViewItems = getTreeViewItemsCallback();
			FieldSelectorItemInfo defaultItemInfo = null;
			Root = CreateItemInfo(null, treeViewItems, null, defaultSelectedField, ref defaultItemInfo);
			DefaultSelectedItem = defaultItemInfo;
			return null;
		}
		FieldSelectorItemInfo CreateItemInfo(ITreeViewItemViewModel treeViewItem, IEnumerable<ITreeViewItemViewModel> children, FieldSelectorItemInfo parent, string defaultField, ref FieldSelectorItemInfo defaultItemInfo) {
			string name = treeViewItem == null ? string.Empty : treeViewItem.DisplayText;
			ImageSource icon = treeViewItem == null ? null : treeViewItem.Image;
			object field = treeViewItem == null ? null : treeViewItem.SelectValue;
			FieldSelectorItemInfo itemInfo = new FieldSelectorItemInfo(name, icon, field, parent, treeViewItem != null && treeViewItem.CanSelect);
			foreach(TreeViewItemViewModel child in children) {
				FieldSelectorItemInfo childItemInfo = CreateItemInfo(child, child.Children, itemInfo, defaultField, ref defaultItemInfo);
				itemInfo.AddChild(childItemInfo);
				if(defaultItemInfo == null && !string.IsNullOrEmpty(defaultField) && child.SelectValue.With(x => x.ToString()) == defaultField)
					defaultItemInfo = childItemInfo;
			}
			return itemInfo;
		}
	}
	public class FieldSelectorItemInfo : INodeSelectorItem {
		List<FieldSelectorItemInfo> children = new List<FieldSelectorItemInfo>();
		public FieldSelectorItemInfo(string name, ImageSource icon, object field, FieldSelectorItemInfo parent, bool canBeSelected) {
			Name = name;
			Icon = icon;
			Field = field;
			Parent = parent;
			CanBeSelected = canBeSelected;
		}
		public FieldSelectorItemInfo Parent { get; private set; }
		public string Name { get; private set; }
		public ImageSource Icon { get; private set; }
		public object Field { get; private set; }
		public bool CanBeSelected { get; private set; }
		public void AddChild(FieldSelectorItemInfo child) {
			children.Add(child);
		}
		IEnumerable<INodeSelectorItem> IMvvmControlTreeNode<INodeSelectorItem>.GetChildren() { return children; }
		INodeSelectorItem IMvvmControlTreeNode<INodeSelectorItem>.Parent { get { return Parent; } }
	}
}
