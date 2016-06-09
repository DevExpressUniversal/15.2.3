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

#if SILVERLIGHT
extern alias Platform;
#endif
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevExpress.Xpf.Core.Design;
#if SILVERLIGHT
using AssemblyInfo = Platform::AssemblyInfo;
using PivotGridControl = Platform.DevExpress.Xpf.PivotGrid.PivotGridControl;
using PivotGridField = Platform.DevExpress.Xpf.PivotGrid.PivotGridField;
using IPivotOLAPDataSource = Platform.DevExpress.XtraPivotGrid.Data.IPivotOLAPDataSource;
using PivotGridWpfData = Platform.DevExpress.Xpf.PivotGrid.Internal.PivotGridWpfData;
using DevExpress.Xpf.Core.Design.AssignDataContextDialog;
using PivotGridData = Platform.DevExpress.XtraPivotGrid.Data.PivotGridData;
using PivotFieldItemBase = Platform.DevExpress.XtraPivotGrid.Data.PivotFieldItemBase;
using ICustomizationTreeItem = Platform.DevExpress.XtraPivotGrid.Customization.ICustomizationTreeItem;
using PivotCustomizationTreeNodeFactoryBase = Platform.DevExpress.XtraPivotGrid.Customization.PivotCustomizationTreeNodeFactoryBase;
using CustomizationFormFields = Platform.DevExpress.XtraPivotGrid.Customization.CustomizationFormFields;
using PivotCustomizationFieldsTreeBase = Platform.DevExpress.XtraPivotGrid.Customization.PivotCustomizationFieldsTreeBase;
using PivotArea = Platform.DevExpress.XtraPivotGrid.PivotArea;
using TreeNodeImageNames = Platform.DevExpress.Xpf.PivotGrid.Internal.TreeNodeImageNames;
using OLAPHierarchy = Platform.DevExpress.PivotGrid.OLAP.OLAPHierarchy;
using IPivotGridDataSource = Platform.DevExpress.XtraPivotGrid.Data.IPivotGridDataSource;
using PivotKPIType = Platform.DevExpress.XtraPivotGrid.PivotKPIType;
using DevExpress.Xpf.Core.Design.CoreUtils;
using Platform::DevExpress.XtraPivotGrid.Data;
using ImageSource = System.Windows.Media.ImageSource;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ImageHelper = DevExpress.Xpf.Core.Native.ImageHelper;
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design.AssignDataContextDialog;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.PivotGrid.OLAP;
#endif
namespace DevExpress.Xpf.PivotGrid.Design {
	public static class OlapFieldsPopulator {
		const string ImagesFolder = "DevExpress.Xpf.PivotGrid.Images.";
		static Dictionary<string, PivotGridDataAsync> dataCache = new Dictionary<string, PivotGridDataAsync>();
		public static Task<IEnumerable<TreeViewItemViewModel>> GetFields(string fieldName, string value, PivotGridWpfData instantData, bool? showDataOnly, Func<bool> checkAccess) {
			return GetDataAsync(value, checkAccess)
				.WithTask(t => RetrieveFields(t, checkAccess))
				.WithFunc(a => PopulateFields(instantData, a, fieldName, showDataOnly));
		}
#if SILVERLIGHT
		public static void UpdateFields(string fieldName, string value, PivotGridWpfData instantData, bool? showDataOnly, TreeViewPropertyLineViewModel fieldNameModel) {
			GetFields(fieldName, value, instantData, showDataOnly, () => instantData.PivotGrid.Dispatcher.CheckAccess())
				.WithFunc(s => CreateFieldsByData(fieldNameModel, s));
		}
#endif
		public static Task<PivotGridDataAsync> GetDataAsync(string olapConnectionString, Func<bool> checkAccess) {
			PivotGridDataAsync data = null;
			if(dataCache.TryGetValue(olapConnectionString, out data)) return TaskHelper.FromResult(data);
			var taskSource = new TaskCompletionSource<PivotGridDataAsync>();
			data = new PivotGridDataAsync();
			data.EventsImplementor = new PivotGridEventsImplementor();
#if !SILVERLIGHT
			data.OLAPDataProvider = OLAPDataProvider.Adomd;
#endif
			data.BeginUpdate();
			data.OLAPConnectionString = olapConnectionString;
			data.EndUpdateAsync(true, r => taskSource.SetAsyncResult(r.Exception, () => checkAccess() ? data : null));
			return taskSource.Task;
		}
		static Task<PivotGridDataAsync> RetrieveFields(PivotGridDataAsync data, Func<bool> checkAccess) {
			if(data == null || data.Fields.Count != 0) return TaskHelper.FromResult(data);
			var taskSource = new TaskCompletionSource<PivotGridDataAsync>();
			data.RetrieveFieldsAsync(PivotArea.FilterArea, false, true, r => {
				if(r.Exception != null) {
					taskSource.SetException(r.Exception);
					return;
				}
				if(!checkAccess()) {
					taskSource.SetResult(null);
					return;
				}
				if(!dataCache.ContainsKey(data.OLAPConnectionString))
					dataCache.Add(data.OLAPConnectionString, data);
				taskSource.SetResult(data);
			});
			return taskSource.Task;
		}
#if SILVERLIGHT
		static void CreateFieldsByData(TreeViewPropertyLineViewModel fieldNameModel, IEnumerable<TreeViewItemViewModel> items) {
			if(items == null) return;
			fieldNameModel.TreeViewItemsViewModel.Items = items;
			if(fieldNameModel.TreeViewItemsViewModel.Items.Count() == 0) {
				fieldNameModel.ReadOnly = false;
				fieldNameModel.ShowSelectButton = false;
			}
		}
#endif
		static IEnumerable<TreeViewItemViewModel> PopulateFields(PivotGridData instantData, PivotGridData newData, string selfFieldName, bool? showDataArea = null) {
			if(newData == null) return null;
			bool isOlap = instantData.IsOLAP || newData.IsOLAP;
			if(!isOlap)
				showDataArea = null;
			List<TreeViewItemViewModel> items = new List<TreeViewItemViewModel>();
			PivotCustomizationFieldsTreeBase nodes = new PivotCustomizationFieldsTreeBase(new CustomizationFormFields(newData), new PivotCustomizationTreeNodeFactoryBase(newData));
			nodes.Update(true);
			CreateNodes(items, nodes.First(), instantData, showDataArea, isOlap, selfFieldName);
			return items;
		}
		static void CreateNodes(List<TreeViewItemViewModel> items, ICustomizationTreeItem nodes, PivotGridData data, bool? showDataArea, bool isOlap, string selfFieldName) {
			if(nodes.Field == null || nodes.Field.Group == null)
				foreach(ICustomizationTreeItem node in nodes.EnumerateChildren())
					CreateNodesByNode(items, node, data, showDataArea, isOlap, selfFieldName);
			else
				foreach(PivotFieldItemBase field in nodes.Field.Group.Fields)
					CreateGroupFieldNode(items, field, data, isOlap, selfFieldName);
		}
		static void CreateGroupFieldNode(List<TreeViewItemViewModel> items, PivotFieldItemBase field, PivotGridData data, bool isOlap, string selfFieldName) {
			TreeViewItemViewModel model = new TreeViewItemViewModel();
			model.DisplayText = field.Caption;
			model.SelectValue = field.FieldName;
			model.CanSelect = CanSelectField(field, data, isOlap, selfFieldName);
			if(isOlap)
				model.Image = ImageHelper.CreateImageFromEmbeddedResource(typeof(PivotGridControl).Assembly, ImagesFolder + TreeNodeImageNames.Dimension + ".png");
			items.Add(model);
		}
		static void CreateNodesByNode(List<TreeViewItemViewModel> items, ICustomizationTreeItem node, PivotGridData data, bool? showDataArea, bool isOlap, string selfFieldName) {
			if(showDataArea != null) {
				if((bool)showDataArea) {
					if(!IsKPIOrMeasureName(node.Name))
						return;
				} else
					if(IsKPIOrMeasureName(node.Name))
						return;
			}
			TreeViewItemViewModel model = new TreeViewItemViewModel();
			model.DisplayText = node.Caption;
			model.SelectValue = node.Field != null ? node.Field.FieldName : null;
			model.CanSelect = node.Field != null && CanSelectField(node.Field, data, isOlap, selfFieldName);
			if(isOlap)
				model.Image = GetImageSourceByICustomizationTreeItem(node);
			items.Add(model);
			items = new List<TreeViewItemViewModel>();
			CreateNodes(items, node, data, null, isOlap, selfFieldName);
			model.Children = items.ToArray();
		}
		static bool CanSelectField(PivotFieldItemBase field, PivotGridData data, bool isOlap, string selfFieldName) {
			return !isOlap || data.Fields[field.FieldName] == null || selfFieldName == field.FieldName;
		}
		static bool IsKPIName(string name) {
			return name == OLAPHierarchy.KPIsHierarchyUniqueName;
		}
		static bool IsMeasureName(string name) {
			return name == OLAPHierarchy.MeasuresHierarchyUniqueName;
		}
		static bool IsKPIOrMeasureName(string name) {
			return IsKPIName(name) || IsMeasureName(name);
		}
		static ImageSource GetImageSourceByICustomizationTreeItem(ICustomizationTreeItem item) {
			string name = null;
			if(IsMeasureName(item.Name))
				name = TreeNodeImageNames.Measure;
			else
				if(IsKPIName(item.Name))
					name = TreeNodeImageNames.KPI;
				else
					if(item.Field != null && item.Field.FieldName.StartsWith(OLAPHierarchy.MeasuresHierarchyUniqueName))
						if(item.Field.KPIType == PivotKPIType.None)
							name = TreeNodeImageNames.Measure;
						else
							name = TreeNodeImageNames.KPI;
					else
						if(item.Field != null)
							if(item.Field.Group != null)
								name = TreeNodeImageNames.Hierarchy;
							else
								name = TreeNodeImageNames.Dimension;
						else
							name = TreeNodeImageNames.FolderClosed;
			if(name == null)
				return null;
			else
				return ImageHelper.CreateImageFromEmbeddedResource(typeof(PivotGridControl).Assembly, ImagesFolder + name + ".png");
		}
	}
}
