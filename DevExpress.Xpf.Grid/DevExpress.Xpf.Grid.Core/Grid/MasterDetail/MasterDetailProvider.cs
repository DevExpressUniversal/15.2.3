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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Xpf.Grid.Native;
namespace DevExpress.Xpf.Grid.Native {
	public class MasterRowScrollInfo {
		public int StartScrollIndex { get; private set; }
		public int DetailStartScrollIndex { get; private set; }
		public bool WholeDetailScrolledOut { get; private set; }
		public MasterRowScrollInfo(int startScrollIndex, int detailStartScrollIndex, bool wholeDetailScrolledOut) {
			this.StartScrollIndex = startScrollIndex;
			this.DetailStartScrollIndex = detailStartScrollIndex;
			this.WholeDetailScrolledOut = wholeDetailScrolledOut;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			MasterRowScrollInfo info = obj as MasterRowScrollInfo;
			return info != null && 
				info.StartScrollIndex == StartScrollIndex && 
				info.DetailStartScrollIndex == DetailStartScrollIndex && 
				info.WholeDetailScrolledOut == WholeDetailScrolledOut;
		}
	}
	public class MasterRowNavigationInfo {
		public int StartVisibleIndex { get; private set; }
		public int DetailStartVisibleIndex { get; private set; }
		public bool IsDetail { get; private set; }
		public MasterRowNavigationInfo(int startVisibleIndex, int detailStartVisibleIndex, bool isDetail) {
			this.StartVisibleIndex = startVisibleIndex;
			this.DetailStartVisibleIndex = detailStartVisibleIndex;
			this.IsDetail = isDetail;
		}
	}
	public abstract class MasterDetailProviderBase {
#if DEBUGTEST
		internal int RecalсDetailScrollInfoCacheCount;
#endif
		protected MasterDetailProviderBase() { }
		public abstract void ChangeMasterRowExpanded(int rowHandle);
		public abstract void UpdateMasterDetailInfo(RowData rowData, bool updateDetailRow);
		public abstract bool IsMasterRowExpanded(int rowHandle, DetailDescriptorBase descriptor = null);
		public abstract NodeContainer GetDetailNodeContainer(int rowHandle);
		public abstract int CalcVisibleDetailRowsCount();
		public abstract int CalcVisibleDetailDataRowCount();
		public abstract void InvalidateDetailScrollInfoCache();
		public abstract int CalcVisibleDetailRowsCountBeforeRow(int scrollIndex);
		public abstract int CalcDetailRowsCountBeforeRow(int visibleIndex);
		public abstract int CalcVisibleDetailRowsCountForRow(int rowHandle);
		public abstract int CalcTotalLevel(int visibleIndex);
		public abstract MasterRowScrollInfo CalcMasterRowScrollInfo(int commonScrollIndex);
		public abstract MasterRowNavigationInfo CalcMasterRowNavigationInfo(int visibleIndex);
		public abstract void SynchronizeDetailTree();
		public abstract DataViewBase FindTargetView(DataViewBase rootView, object originalSource);
		public abstract bool FindViewAndVisibleIndexByScrollIndex(int scrollIndex, int visibleIndex, bool forwardIfServiceRow, out DataViewBase targetView, out int targetVisibleIndex);
		public abstract DataViewBase FindFirstDetailView(int visibleIndex);
		public abstract DataViewBase FindLastInnerDetailView(int visibleIndex);
		public abstract DataControlBase FindVisibleDetailDataControl(int rowHandle);
		public abstract DetailDescriptorBase FindVisibleDetailDescriptor(int rowHandle);
		public abstract DataControlBase FindDetailDataControl(int rowHandle, DataControlDetailDescriptor descriptor);
		public abstract bool SetMasterRowExpanded(int rowHandle, bool expand, DetailDescriptorBase descriptor);
		public abstract void UpdateDetailDataControls(Action<DataControlBase> updateMethod);
		public abstract void UpdateDetailDataControls(Action<DataControlBase> updateOpenDetailMethod, Action<DataControlBase> updateClosedDetailMethod);
		public abstract void UpdateOriginationDataControls(Action<DataControlBase> updateMethod);
		public abstract void UpdateDetailViewIndents(ObservableCollection<DetailIndent> ownerIndents);
		public abstract void ValidateMasterDetailConsistency();
		public abstract void OnDetach();
	}
	public class MasterDetailProvider : MasterDetailProviderBase, IDetailDescriptorOwner {
		readonly TableViewBehavior viewBehavior;
		internal DataViewBase View { get { return viewBehavior.View; } }
		DetailDescriptorBase DetailDescriptor { get; set; }
#if DEBUGTEST
		public
#else
		internal
#endif
 RowDetailInfoBase GetRowDetailInfo(int rowHandle) {
			return GetRowDetailInfoCore(rowHandle, true);
		}
		RowDetailInfoBase GetReadOnlyRowDetailInfo(int rowHandle) {
			if(!View.AllowMasterDetailCore)
				return EmptyRowDetailInfo.Instance;
			return GetRowDetailInfoCore(rowHandle, false) ?? EmptyRowDetailInfo.Instance;
		}
		RowDetailInfoBase GetRowDetailInfoCore(int rowHandle, bool createNewIfNotExist) {
			RowDetailContainer container = View.DataProviderBase.GetRowDetailContainer(rowHandle, () => CreateRowDetailContainer(rowHandle), createNewIfNotExist);
			return container != null ? container.RootDetailInfo : null;
		}
		RowDetailContainer CreateRowDetailContainer(int rowHandle) {
			RowDetailContainer newContainer = new RowDetailContainer(View.DataControl, View.DataProviderBase.GetRowValue(rowHandle));
			DetailInfoWithContent detailInfo = DetailDescriptor.CreateRowDetailInfo(newContainer);
			newContainer.RootDetailInfo = detailInfo;
			return newContainer;
		}
		internal RowDetailInfoBase GetRowDetailInfoForPrinting(int rowHandle) {
			return GetRowDetailInfoCore(rowHandle, false) ?? CreateRowDetailContainer(rowHandle).RootDetailInfo;
		}
		public MasterDetailProvider(TableViewBehavior viewBehavior) {
			this.viewBehavior = viewBehavior;
			DetailDescriptor = View.DataControl.DetailDescriptorCore;
			ValidateMasterDetailConsistency();
			detailScrollInfoCache = new DetailScrollInfoCache(this);
		}
		public override bool SetMasterRowExpanded(int rowHandle, bool expand, DetailDescriptorBase descriptor) {
			RowDetailInfoBase detailInfo = GetRowDetailInfoCore(rowHandle, expand);
			if(detailInfo != null) {
				SetMasterRowExpandedCore(rowHandle, detailInfo, expand, descriptor);
				return true;
			}
			return false;
		}
		protected virtual void SetMasterRowExpandedCore(int rowHandle, RowDetailInfoBase detailInfo, bool expand, DetailDescriptorBase descriptor) {
			if(!View.CommitEditing()) return;
			bool allow = View.DataControl.RaiseMasterRowExpandStateChanging(rowHandle, detailInfo.IsExpanded);
			if(!allow) return;
			View.DataControl.InvalidateDetailScrollInfoCache();
			detailInfo.SetDetailRowExpanded(expand, descriptor);
			View.OnDataReset();
			View.DataControl.RaiseMasterRowExpandStateChanged(rowHandle, detailInfo.IsExpanded);
		}
		public override void ChangeMasterRowExpanded(int rowHandle) {
			RowDetailInfoBase detailInfo = GetRowDetailInfo(rowHandle);
			if(detailInfo.IsExpanded)
				SetMasterRowExpandedCore(rowHandle, detailInfo, false, null);
			else
				SetMasterRowExpandedCore(rowHandle, detailInfo, true, null);
		}
		public override void UpdateMasterDetailInfo(RowData rowData, bool updateDetailRow) {
			int rowHandle = rowData.RowHandle.Value;
			if(updateDetailRow) GetReadOnlyRowDetailInfo(rowHandle).OnUpdateRow(rowData.Row);
			rowData.IsRowExpanded = IsMasterRowExpanded(rowHandle);
			rowData.RowsContainer = GetReadOnlyRowDetailInfo(rowHandle).GetRowsContainerAndUpdateMasterRowData(rowData);
		}
		public override bool IsMasterRowExpanded(int rowHandle, DetailDescriptorBase descriptor = null) {
			return GetReadOnlyRowDetailInfo(rowHandle).IsDetailRowExpanded(descriptor);
		}
		public override NodeContainer GetDetailNodeContainer(int rowHandle) {
			return GetReadOnlyRowDetailInfo(rowHandle).GetNodeContainer();
		}
		#region optimized calculations verification
#if DEBUGTEST
		internal static bool VerifyOptimizedCalculations = false;
		int CalcVisibleDetailRowsCountSlow() {
			return CalcVisibleDetailRowsCountBeforeRow(View.DataProviderBase.VisibleCount);
		}
		MasterRowScrollInfo CalcMasterRowScrollInfoSlow(int commonScrollIndex) {
			int totalScrollIndex = 0;
			for(int i = 0; i < View.DataControl.VisibleRowCount; i++) {
				int localVisibleIndex = View.DataProviderBase.ConvertScrollIndexToVisibleIndex(i, View.AllowFixedGroupsCore);
				int rowHandle = View.DataControl.GetRowHandleByVisibleIndexCore(localVisibleIndex);
				int visibleDetailRowsCount = CalcVisibleDetailRowsCountForRow(rowHandle);
				int totalScrollIndexWithCurrentDetails = totalScrollIndex + visibleDetailRowsCount;
				if(totalScrollIndexWithCurrentDetails >= commonScrollIndex) {
					bool wholeDetailScrolledOut = totalScrollIndexWithCurrentDetails == commonScrollIndex;
					return new MasterRowScrollInfo(i, commonScrollIndex - totalScrollIndex, wholeDetailScrolledOut);
				}
				totalScrollIndex += visibleDetailRowsCount + 1;
			}
			return null;
		}
		int CalcVisibleDetailRowsCountBeforeRowSlow(int scrollIndex) {
			int detailsVisibleRowCount = 0;
			for(int i = 0; i < scrollIndex; i++) {
				object visibleIndexCore = View.DataProviderBase.GetVisibleIndexByScrollIndex(i);
				if(visibleIndexCore is GroupSummaryRowKey) continue;
				int localVisibleIndex = View.DataProviderBase.ConvertScrollIndexToVisibleIndex(i, View.AllowFixedGroupsCore);
				int rowHandle = View.DataControl.GetRowHandleByVisibleIndexCore(localVisibleIndex);
				int visibleDetailRowsCount = CalcVisibleDetailRowsCountForRow(rowHandle);
				detailsVisibleRowCount += visibleDetailRowsCount;
			}
			return detailsVisibleRowCount;
		}
		public DetailScrollInfoCache DetailScrollInfoCache { get { return detailScrollInfoCache; } }
#endif
		#endregion
		DetailScrollInfoCache detailScrollInfoCache;
		public override int CalcVisibleDetailRowsCount() {
			int result = detailScrollInfoCache.CalcScrollDetailRowsCount();
#if DEBUGTEST
			if(VerifyOptimizedCalculations) {
				int resultOld = CalcVisibleDetailRowsCountSlow();
				if(result != resultOld) {
					throw new InvalidOperationException();
				}
			}
#endif
			return result;
		}
		public override int CalcVisibleDetailDataRowCount() {
			return detailScrollInfoCache.CalcVisibleDetailDataRowCount();
		}
		public override MasterRowScrollInfo CalcMasterRowScrollInfo(int commonScrollIndex) {
			MasterRowScrollInfo masterRowScrollInfo = detailScrollInfoCache.CalcMasterRowScrollInfo(commonScrollIndex);
#if DEBUGTEST
			if(VerifyOptimizedCalculations) {
				MasterRowScrollInfo masterRowScrollInfoOld = CalcMasterRowScrollInfoSlow(commonScrollIndex);
				if(!object.Equals(masterRowScrollInfoOld, masterRowScrollInfo)) {
					throw new InvalidOperationException();
				}
			}
#endif
			return masterRowScrollInfo;
		}
		public override MasterRowNavigationInfo CalcMasterRowNavigationInfo(int visibleIndex) {
			return detailScrollInfoCache.CalcMasterRowNavigationInfo(visibleIndex);
		}
		public override void InvalidateDetailScrollInfoCache() {
			detailScrollInfoCache.InvalidateCache();
		}
		public override int CalcVisibleDetailRowsCountBeforeRow(int scrollIndex) {
			int visibleDetailRowsCountBeforeRow = detailScrollInfoCache.CalcScrollDetailRowsCountBeforeRow(scrollIndex);
#if DEBUGTEST
			if(VerifyOptimizedCalculations) {
				int visibleDetailRowsCountBeforeRowOld = CalcVisibleDetailRowsCountBeforeRowSlow(scrollIndex);
				if(visibleDetailRowsCountBeforeRow != visibleDetailRowsCountBeforeRowOld) {
					throw new InvalidOperationException();
				}
			}
#endif
			return visibleDetailRowsCountBeforeRow;
		}
		public override int CalcDetailRowsCountBeforeRow(int visibleIndex) {
			return detailScrollInfoCache.CalcVisibleDetailRowsCountBeforeRow(visibleIndex);
		}
		public override int CalcVisibleDetailRowsCountForRow(int rowHandle) {
			return GetReadOnlyRowDetailInfo(rowHandle).CalcRowsCount();
		}
		public int CalcVisibleDataRowCountForRow(int rowHandle) {
			return GetReadOnlyRowDetailInfo(rowHandle).CalcVisibleDataRowCount();
		}
		public override int CalcTotalLevel(int visibleIndex) {
			return GetReadOnlyRowDetailInfo(View.DataControl.GetRowHandleByVisibleIndexCore(visibleIndex)).CalcTotalLevel();
		}
		public override void SynchronizeDetailTree() {
			DetailDescriptor.SynchronizeDetailTree();
		}
		DataControlBase GetRootDataControl(DataControlBase dataControl) {
			return View.DataControl.GetRootDataControl();
		}
		public override DataViewBase FindTargetView(DataViewBase rootView, object originalSource) {
			RowData rowData = RowData.FindRowData((DependencyObject)originalSource);
			return (rowData != null) ? rowData.View : rootView;
		}
		public override bool FindViewAndVisibleIndexByScrollIndex(int scrollIndex, int visibleIndex, bool forwardIfServiceRow, out DataViewBase targetView, out int targetVisibleIndex) {
			int rowHandle = View.DataControl.GetRowHandleByVisibleIndexCore(visibleIndex);
			RowDetailInfoBase detailInfo = GetReadOnlyRowDetailInfo(rowHandle);
			if(!detailInfo.IsExpanded) {
				targetView = null;
				targetVisibleIndex = -1;
				return false;
			}
			return detailInfo.FindViewAndVisibleIndexByScrollIndex(scrollIndex, forwardIfServiceRow, out targetView, out targetVisibleIndex);
		}
		public override DataViewBase FindFirstDetailView(int visibleIndex) {
			int rowHandle = View.DataControl.GetRowHandleByVisibleIndexCore(visibleIndex);
			RowDetailInfoBase detailInfo = GetReadOnlyRowDetailInfo(rowHandle);
			if(!detailInfo.IsExpanded) {
				return null;
			}
			return detailInfo.FindFirstDetailView();
		}
		public override DataControlBase FindVisibleDetailDataControl(int rowHandle) {
			RowDetailInfoBase detailInfo = GetReadOnlyRowDetailInfo(rowHandle);
			if(!detailInfo.IsExpanded) 
				return null;
			return detailInfo.FindVisibleDetailDataControl();
		}
		public override DetailDescriptorBase FindVisibleDetailDescriptor(int rowHandle) {
			RowDetailInfoBase detailInfo = GetReadOnlyRowDetailInfo(rowHandle);
			if(!detailInfo.IsExpanded)
				return null;
			return detailInfo.FindVisibleDetailDescriptor();
		}
		public override DataControlBase FindDetailDataControl(int rowHandle, DataControlDetailDescriptor descriptor) {
			RowDetailInfoBase detailInfo = GetReadOnlyRowDetailInfo(rowHandle);
			if(!detailInfo.IsExpanded)
				return null;
			return detailInfo.FindDetailDataControl(descriptor);
		}
		public override DataViewBase FindLastInnerDetailView(int visibleIndex) {
			int rowHandle = View.DataControl.GetRowHandleByVisibleIndexCore(visibleIndex);
			RowDetailInfoBase detailInfo = GetReadOnlyRowDetailInfo(rowHandle);
			if(!detailInfo.IsExpanded) {
				return null;
			}
			return detailInfo.FindLastInnerDetailView();
		}
		public override void UpdateDetailDataControls(Action<DataControlBase> updateMethod) {
			DetailDescriptor.UpdateDetailDataControls(updateMethod, null);
		}
		public override void UpdateDetailDataControls(Action<DataControlBase> updateOpenDetailMethod, Action<DataControlBase> updateClosedDetailMethod) {
			DetailDescriptor.UpdateDetailDataControls(updateOpenDetailMethod, updateClosedDetailMethod);
		}
		public override void UpdateOriginationDataControls(Action<DataControlBase> updateMethod) {
			DetailDescriptor.UpdateOriginationDataControls(updateMethod);
		}
		public override void UpdateDetailViewIndents(ObservableCollection<DetailIndent> ownerIndents) {
			DetailDescriptor.UpdateDetailViewIndents(ownerIndents, GetGroupDetailMargin(viewBehavior.TableView.ActualDetailMargin));
		}
		Thickness GetGroupDetailMargin(Thickness actualMargin) {
			return new Thickness(View.DataControl.ActualGroupCountCore * viewBehavior.TableView.LeftGroupAreaIndent + actualMargin.Left, actualMargin.Top, actualMargin.Right, actualMargin.Bottom);
		}
		#region IDetailDescriptorOwner Members
		void IDetailDescriptorOwner.InvalidateTree() {
			View.DataControl.EnumerateThisAndOwnerDataControls(
				dataControl => DataControlOriginationElementHelper.EnumerateDependentElemetsIncludingSource(dataControl, dc => dc,
					dc => dc.MasterDetailProvider.InvalidateDetailScrollInfoCache()
				)
			);
			View.RootView.OnDataReset();
		}
		void IDetailDescriptorOwner.EnumerateOwnerDataControls(Action<DataControlBase> action) {
			if(View.DataControl == null)
				return;
			action(View.DataControl);
			View.DataControl.DataControlOwner.EnumerateOwnerDataControls(action);
		}
		bool IDetailDescriptorOwner.CanAssignTo(DataControlBase dataControl) {
			return dataControl == View.DataControl;
		}
		#endregion
		public override void ValidateMasterDetailConsistency() {
			View.DataProviderBase.ThrowNotSupportedExceptionIfInServerMode();
			View.ThrowNotSupportedInMasterDetailException();
			View.DataControl.ThrowNotSupportedInMasterDetailException();
		}
		public override void OnDetach() {
			View.DataProviderBase.ClearDetailInfo();
			DetailDescriptor.OnDetach();
		}
	}
	public class NullDetailProvider : MasterDetailProviderBase {
		public static NullDetailProvider Instance = new NullDetailProvider();
		NullDetailProvider() { }
		public override void ChangeMasterRowExpanded(int rowHandle) { }
		public override void UpdateMasterDetailInfo(RowData rowData, bool updateDetailRow) { }
		public override bool IsMasterRowExpanded(int rowHandle, DetailDescriptorBase descriptor = null) {
			return false;
		}
		public override bool SetMasterRowExpanded(int rowHandle, bool expand, DetailDescriptorBase descriptor) {
			return false;
		}
		public override NodeContainer GetDetailNodeContainer(int rowHandle) {
			return null;
		}
		public override int CalcVisibleDetailRowsCount() {
			return 0;
		}
		public override int CalcVisibleDetailDataRowCount() {
			return 0;
		}
		public override void InvalidateDetailScrollInfoCache() { }
		public override int CalcVisibleDetailRowsCountBeforeRow(int scrollIndex) {
			return 0;
		}
		public override int CalcDetailRowsCountBeforeRow(int visibleIndex) {
			return 0;
		}
		public override int CalcVisibleDetailRowsCountForRow(int rowHandle) {
			return 0;
		}
		public override int CalcTotalLevel(int visibleIndex) {
			return 0;
		}
		public override MasterRowScrollInfo CalcMasterRowScrollInfo(int commonScrollIndex) {
			return new MasterRowScrollInfo(commonScrollIndex, 0, true);
		}
		public override MasterRowNavigationInfo CalcMasterRowNavigationInfo(int visibleIndex) {
			return new MasterRowNavigationInfo(visibleIndex, 0, false);
		}
		public override void SynchronizeDetailTree() { }
		public override DataViewBase FindTargetView(DataViewBase rootView, object originalSource) {
			return rootView;
		}
		public override bool FindViewAndVisibleIndexByScrollIndex(int scrollIndex, int visibleIndex, bool forwardIfServiceRow, out DataViewBase targetView, out int targetVisibleIndex) {
			targetView = null;
			targetVisibleIndex = -1;
			return false;
		}
		public override DataViewBase FindFirstDetailView(int visibleIndex) {
			return null;
		}
		public override DataViewBase FindLastInnerDetailView(int visibleIndex) {
			return null;
		}
		public override DataControlBase FindVisibleDetailDataControl(int rowHandle) {
			return null;
		}
		public override DetailDescriptorBase FindVisibleDetailDescriptor(int rowHandle) {
			return null;
		}
		public override DataControlBase FindDetailDataControl(int rowHandle, DataControlDetailDescriptor descriptor) {
			return null;
		}
		public override void UpdateDetailDataControls(Action<DataControlBase> updateMethod) { }
		public override void UpdateDetailDataControls(Action<DataControlBase> updateOpenDetailMethod, Action<DataControlBase> updateClosedDetailMethod) { }
		public override void UpdateOriginationDataControls(Action<DataControlBase> updateMethod) { }
		public override void UpdateDetailViewIndents(ObservableCollection<DetailIndent> ownerIndents) { }
		public override void ValidateMasterDetailConsistency() { }
		public override void OnDetach() { }
	}
}
