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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Model {
	public enum ViewsOrder { ListViewDetailView, DetailViewListView }
	[ImageName("ModelEditor_ListView")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelListView")]
#endif
	[ModelNodeValueSource("ImageName", "ModelClass", "DefaultListViewImage")]
	public interface IModelListView : IModelObjectView {
		IModelBandsLayout BandsLayout { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewEditorType"),
#endif
 ModelPersistentName("EditorTypeName")]
		[TypeConverter(typeof(StringToTypeConverterBase))]
		[DataSourceProperty("ModelClass.ListEditorsType")]
		[ModelValueCalculator("ModelClass", "EditorType")]
		[Category("Appearance")]
		Type EditorType { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewDetailView"),
#endif
 ModelValueCalculator("ModelClass", "DefaultDetailView")]
		[ModelPersistentName("DetailViewID")]
		[DataSourceProperty(ModelViewsDomainLogic.DataSourcePropertyPath)]
		[DataSourceCriteria(ModelObjectViewLogic.ModelViewsByClassCriteria)]
		[Category("Appearance")]
		IModelDetailView DetailView { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewMasterDetailView"),
#endif
 ModelValueCalculator("this", "DetailView")]
		[DataSourceProperty(ModelViewsDomainLogic.DataSourcePropertyPath)]
		[DataSourceCriteria(ModelObjectViewLogic.ModelViewsByClassCriteria)]
		[Category("Appearance")]
		IModelDetailView MasterDetailView { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewMasterDetailMode"),
#endif
 ModelValueCalculator("ModelClass", "DefaultListViewMasterDetailMode")]
		[Category("Behavior")]
		MasterDetailMode MasterDetailMode { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewDataAccessMode"),
#endif
 Category("Behavior"), ModelValueCalculator("Application.Options", "DataAccessMode")]
		[RefreshProperties(RefreshProperties.All)]
		CollectionSourceDataAccessMode DataAccessMode { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewUseServerMode"),
#endif
 Category("Behavior")]
		[RefreshProperties(RefreshProperties.All)]
		Boolean UseServerMode { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewIsGroupPanelVisible"),
#endif
 Category("Behavior")]
		[DefaultValue(false)]
		bool IsGroupPanelVisible { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewAutoExpandAllGroups"),
#endif
 DefaultValue(false)]
		[Category("Behavior")]
		bool AutoExpandAllGroups { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewIsFooterVisible"),
#endif
 DefaultValue(false)]
		[Category("Behavior")]
		bool IsFooterVisible { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewGroupSummary"),
#endif
 Category("Behavior")]
		string GroupSummary { get; set; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelListViewColumns")]
#endif
		IModelColumns Columns { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelListViewSplitLayout")]
#endif
		IModelListViewSplitLayout SplitLayout { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelListViewFreezeColumnIndices")]
#endif
		[Category("Behavior")]
		[ModelBrowsable(typeof(NotNewNodeVisibleCalculator))]
		bool FreezeColumnIndices { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewCriteria"),
#endif
 Category("Data")]
		[CriteriaOptions("ModelClass.TypeInfo")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string Criteria { get; set; }
		[DefaultValue("")]
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewFilter"),
#endif
 Category("Data")]
		[CriteriaOptions("ModelClass.TypeInfo")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string Filter { get; set; }
		[Browsable(false)]
		bool FilterEnabled { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewTopReturnedObjects"),
#endif
 Category("Data")]
		[DefaultValue(0)]
		int TopReturnedObjects { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewSorting"),
#endif
 Category("Data")]
		IModelSorting Sorting { get; }
		[  ModelValueCalculator("this", "AllowNew")]
		[Category("Behavior")]
		bool AllowLink { get; set; }
		[ ModelValueCalculator("this", "AllowDelete")]
		[Category("Behavior")]
		bool AllowUnlink { get; set; }
	}
	[ModelNodesGenerator(typeof(ModelListViewColumnsNodesGenerator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelColumns")]
#endif
	public interface IModelColumns : IModelNode, IModelList<IModelColumn> {
		IList<IModelColumn> GetVisibleColumns();
	}
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
	sealed class ApplyDiffValuesMapAttribute : Attribute {
		string sourceValueName;
		string targetValueName;
		public ApplyDiffValuesMapAttribute(string sourceValueName, string targetValueName) {
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNullOrEmpty(sourceValueName, "sourceValueName");
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNullOrEmpty(targetValueName, "targetValueName");
			this.sourceValueName = sourceValueName;
			this.targetValueName = targetValueName;
		}
		public string SourceValueName { get { return sourceValueName; } }
		public string TargetValueName { get { return targetValueName; } }
	}
	[ModelPersistentName("ColumnInfo")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelColumn")]
#endif
	[ApplyDiffValuesMap(ModelListViewColumnsNodesGenerator.GeneratedIndexValueName, ModelValueNames.Index)]
	public interface IModelColumn : IModelMemberViewItem {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelColumnWidth"),
#endif
 Category("Layout")]
		int Width { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelColumnSortOrder"),
#endif
 DefaultValue(DevExpress.Data.ColumnSortOrder.None)]
		[Category("Behavior")]
		DevExpress.Data.ColumnSortOrder SortOrder { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelColumnSortIndex"),
#endif
 DefaultValue(-1)]
		[Category("Layout")]
		int SortIndex { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelColumnGroupIndex"),
#endif
 DefaultValue(-1)]
		[Category("Layout")]
		int GroupIndex { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelColumnGroupInterval"),
#endif
 DefaultValue(GroupInterval.None)]
		[Category("Behavior")]
		GroupInterval GroupInterval { get; set; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelColumnSummary")]
#endif
		IModelColumnSummary Summary { get; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		string FieldName { get; }
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelColumnSummary")]
#endif
	public interface IModelColumnSummary : IModelNode, IModelList<IModelColumnSummaryItem> {
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelColumnSummaryItem")]
#endif
	public interface IModelColumnSummaryItem : IModelNode {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelColumnSummaryItemSummaryType")]
#endif
		SummaryType SummaryType { get; set; }
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelSplitLayout")]
#endif
	public interface IModelSplitLayout : IModelNode {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelSplitLayoutDirection"),
#endif
 Category("Behavior")]
		[DefaultValue(DevExpress.ExpressApp.Layout.FlowDirection.Horizontal)]
		DevExpress.ExpressApp.Layout.FlowDirection Direction { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelSplitLayoutSplitterPosition"),
#endif
 DefaultValue(150)]
		[Category("Layout")]
		int SplitterPosition { get; set; }
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelListViewSplitLayout")]
#endif
	public interface IModelListViewSplitLayout : IModelSplitLayout {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewSplitLayoutViewsOrder"),
#endif
 Category("Behavior")]
		[DefaultValue(ViewsOrder.ListViewDetailView)]
		ViewsOrder ViewsOrder { get; set; }
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelSorting")]
#endif
	public interface IModelSorting : IModelNode, IModelList<IModelSortProperty> {
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelSortProperty")]
#endif
	public interface IModelSortProperty : IModelNode {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelSortPropertyPropertyName")]
#endif
		[Category("Data"), Required]
		string PropertyName { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelSortPropertyDirection"),
#endif
 Category("Behavior")]
		DevExpress.Xpo.DB.SortingDirection Direction { get; set; }
	}
}
