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

using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.XtraReports.UI;
using System;
using System.Linq;
using Report = DevExpress.Xpf.Reports.UserDesigner.ReportModel.XtraReportModelBase;
using DevExpress.Diagram.Core;
using System.Windows;
using DevExpress.Xpf.Reports.UserDesigner.Layout.Native;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Reports.UserDesigner.FieldList;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.Mvvm;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Reports.UserDesigner.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.GroupSort.Native {
	abstract class ModifyStrategy {
		readonly Report reportModel;
		protected Report ReportModel { get { return reportModel; } }
		protected XtraReport Report { get { return reportModel.XRObject; } }
		protected ModifyStrategy(Report reportModel) {
			this.reportModel = reportModel;
		}
		internal static string GetValidFieldName(string dataSourceMember, string bindingMember) {
			if (!string.IsNullOrEmpty(dataSourceMember) && bindingMember.StartsWith(dataSourceMember)) {
				return bindingMember.Substring(dataSourceMember.Length + 1);
			}
			if (dataSourceMember == bindingMember)
				return null;
			return bindingMember;
		}
		public static XRGroup GetGroup(XtraReportBase selectedReport, GroupField field) {
			return (field.Band as GroupBand).Return(x => selectedReport.Groups.FindGroupByBand(x), () => null);
		}
		protected static bool BandContainsControls(Band band) {
			return band != null && band.Controls != null && band.Controls.Count > 0;
		}
		protected static MessageResult ShowRemoveWarning(IMessageBoxService message) {
			return message.ShowMessage(ReportLocalizer.GetString(ReportStringId.Msg_GroupSortWarning), ReportLocalizer.GetString(ReportStringId.UD_Title_GroupAndSort), MessageButton.YesNo, MessageIcon.Exclamation);
		}
		protected static void RemoveBands(Report reportModel, params Band[] bands) {
			var items = bands.Where(x => x != null)
			   .Select(x => reportModel.Factory.GetModel(x).DiagramItem).ToArray();
			reportModel.DiagramItem.Diagram.UndoManager().Execute(transaction => transaction.Execute(t => {
				items.ForEach(t.RemoveItem);
			}));
		}
	}
	class GroupSortAddStrategy : ModifyStrategy {
		public GroupSortAddStrategy(Report report) : base(report) { }
		#region add sort
		public GroupField AddSort(string fieldName) {
			var detail = Report.Bands[BandKind.Detail];
			return AddSortCore(fieldName, (DetailBand)detail);
		}
		GroupField AddSortCore(string fieldName, DetailBand detailband) {
			GuardHelper.ArgumentNotNull(fieldName, "bindingData");
			GuardHelper.ArgumentNotNull(detailband, "detailband");
			if (Report.IsDisposed)
				throw new InvalidOperationException();
			var groupField = new GroupField(fieldName);
			detailband.SortFieldsInternal.Add(groupField);
			return groupField;
		}
		#endregion
		#region addGroup
		public GroupField AddGroup(XtraReportBase selectedReportItem, string fieldName) {
			var detail = selectedReportItem.Bands[BandKind.Detail];
			return AddGroupCore(fieldName, (DetailBand)detail);
		}
		GroupField AddGroupCore(string fieldName, DetailBand detailBand) {
			GuardHelper.ArgumentNotNull(fieldName, "bindingData");
			GuardHelper.ArgumentNotNull(detailBand, "detailband");
			if (Report.IsDisposed)
				throw new InvalidOperationException();
			var sortFields = detailBand.SortFields;
			var band = CreateGroupHeader(fieldName);
			var groupField = band.GroupFields[0];
			AddGroupFields(sortFields.ToArray(), band.GroupFields);
			var newBandItem = ReportModel.Factory.GetModel(band, true).DiagramItem;
			var parentItem = detailBand.Report == Report ? ReportModel.DiagramItem : (IDiagramContainer)ReportModel.Factory.GetModel(detailBand.Report).DiagramItem;
			ReportModel.DiagramItem.Diagram.DrawItem(newBandItem, parentItem, new Rect(new Size(0.0, BoundsConverter.ToDouble(band.HeightF, band.Dpi))));
			Tracker.Set(band, x => x.Level, 0);
			return groupField;
		}
		void AddGroupFields(GroupField[] sourceArray, GroupFieldCollection dest) {
			for (int i = 0; i < sourceArray.Length; i++)
				dest.Add(sourceArray[i]);
		}
		GroupHeaderBand CreateGroupHeader(string fieldName) {
			var band = XtraReport.CreateBand(BandKind.GroupHeader) as GroupHeaderBand;
			band.GroupFields.Add(new GroupField(fieldName));
			return band;
		}
		#endregion
	}
	class GroupSortMovementStrategy : ModifyStrategy {
		public GroupSortMovementStrategy(Report reportModel) : base(reportModel) { }
		#region moveDown
		public void MoveDown(IGroupSortItem item, IGroupSortItem child) {
			if (item.ShowHeader && !child.ShowHeader)
				item.GroupField.Band.SortFieldsInternal.Insert(item.GroupField.Band.SortFieldsInternal.Count - 1, child.GroupField);
			else if (!item.ShowHeader && !child.ShowHeader)
				MoveGroupFieldDown(item.GroupField.Band.SortFieldsInternal, item.GroupField);
			else if (!item.ShowHeader && child.ShowHeader)
				child.GroupField.Band.SortFieldsInternal.Insert(0, item.GroupField);
			else if (item.ShowHeader && child.ShowHeader)
				MoveGroupDown(item.GroupField.Band, child.GroupField.Band);
		}
		void MoveGroupFieldDown(GroupFieldCollection owner, GroupField groupField) {
			int index = owner.IndexOf(groupField);
			if (index + 1 < owner.Count) {
				owner.RemoveAt(index);
				owner.Insert(index + 1, groupField);
			}
		}
		void MoveGroupDown(Band band, Band nextBand) {
			GroupField[] groupFields = band.SortFieldsInternal.ToArray();
			for (int i = 0; i < groupFields.Length - 1; i++)
				nextBand.SortFieldsInternal.Insert(i, groupFields[i]);
			IObjectTracker tracker;
			Tracker.GetTracker(band, out tracker);
			band.LevelInternal = nextBand.LevelInternal;
			((TrackablePropertyDescriptor.IObjectTrackerInternal)tracker).RaisePropertyChanged("Level");
		}
		#endregion
		#region moveUp
		public void MoveUp(IGroupSortItem item, IGroupSortItem parent) {
			MoveDown(parent, item);
		}
		#endregion
	}
	class GroupSortRemoveStrategy : ModifyStrategy {
		XtraReportBase selectedReport;
		public GroupSortRemoveStrategy(Report reportModel, XtraReportBase selectedReport) : base(reportModel) {
			this.selectedReport = selectedReport;
		}
		public bool CanRemove(IGroupSortItem item, Action<Action<IMessageBoxService>> doWithMessageBoxService) {
			var canRemove = true;
			var group = GetGroup(selectedReport, item.GroupField);
			if (group != null && (BandContainsControls(group.Header) || BandContainsControls(group.Footer))) {
				doWithMessageBoxService(message => {
					canRemove = ShowRemoveWarning(message) == MessageResult.Yes;
				});
			}
			return canRemove;
		}
		public void Remove(IGroupSortItem groupSortItem, IGroupSortItem nextGroupSortItem) {
			if(groupSortItem.ShowHeader) {
				var group = GetGroup(selectedReport, groupSortItem.GroupField);
				if(groupSortItem.ShowHeader && group != null) {
					var nextBand = nextGroupSortItem.Return(x => x.GroupField.Band, () => groupSortItem.GroupField.Band.Report.Bands[BandKind.Detail]);
					RemoveGroup(group, nextBand);
				}
			} else {
				GroupFieldCollection groupFields = groupSortItem.GroupField.Band.SortFieldsInternal;
				groupFields.Remove(groupSortItem.GroupField);
			}
		}
		void RemoveGroup(XRGroup group, Band nextBand) {
			var nextBandGroupsCollection = nextBand != null ? nextBand.SortFieldsInternal : null;
			var groupFields = group.GroupFields.ToArray();
			if (nextBandGroupsCollection != null) {
				for (int i = 0; i < groupFields.Length - 1; i++) {
					nextBandGroupsCollection.Insert(i, groupFields[i]);
				}
			}
			RemoveGroupCore(group);
		}
		void RemoveGroupCore(XRGroup group) {
			RemoveBands(ReportModel, group.Header, group.Footer);
		}
	}
	class GroupSortFieldModifyStrategy : ModifyStrategy {
		public GroupSortFieldModifyStrategy(Report reportModel) : base(reportModel) { }
		public void ChangeField(GroupField field, string fieldName) {
			var bandModel = ReportModel.Factory.GetModel(field.Band);
			var propertiesModel = (SelectionModel<IDiagramItem>)ReportModel.DiagramItem.Diagram.Controller
				.CreateMultiModel(typeof(SelectionModel<>), typeof(SelectionModel<>), true, new[] { bandModel.DiagramItem });
			field.FieldName = fieldName;
		}
		public void ChangeSortOrder(GroupField field, XRColumnSortOrder sortOrder) {
			field.SortOrder = sortOrder;
		}
	}
	class GroupSortHeaderFooterVisibilityStrategy : ModifyStrategy {
		readonly Action<Action<IMessageBoxService>> doWithMessageBoxService;
		readonly XtraReportBase selectedReport;
		public GroupSortHeaderFooterVisibilityStrategy(Report reportModel, Action<Action<IMessageBoxService>> doWithMessageBoxService, XtraReportBase selectedReport) : base(reportModel) {
			this.doWithMessageBoxService = doWithMessageBoxService;
			this.selectedReport = selectedReport;
		}
		#region header
		public void ShowHeader(IGroupSortItem selectedItem) {
			var field = selectedItem.GroupField;
			var band = field.Band;
			var groupFieldCount = field.Index + 1;
			var level = band.LevelInternal + 1;
			var fields = band.SortFieldsInternal;
			var header = XtraReportBase.CreateBand(BandKind.GroupHeader) as GroupHeaderBand;
			AddGroupFields(fields.ToArray(), header.GroupFields);
			header.Level = level;
			var item = ReportModel.Factory.GetModel(header, true).DiagramItem;
			var reportItem = selectedReport == Report ? ReportModel.DiagramItem : ReportModel.Factory.GetModel(selectedReport).DiagramItem;
			ReportModel.DiagramItem.Diagram.DrawItem(item, (IDiagramContainer)reportItem, new Rect(new Size(0.0, BoundsConverter.ToDouble(header.HeightF, header.Dpi))));
		}
		void AddGroupFields(GroupField[] sourceArray, GroupFieldCollection dest) {
			for (int i = 0; i < sourceArray.Length; i++)
				dest.Add(sourceArray[i]);
		}
		public void HideHeader(IGroupSortItem selectedItem, IGroupSortItem nextItem) {
			var field = selectedItem.GroupField;
			var band = field.Band;
			var group = GetGroup(selectedReport, field);
			if (group != null && CanHideBand(group.Header)) {
				var destSortFields = nextItem.Return(x => x.GroupField.Band.SortFieldsInternal, () => selectedReport.Bands[BandKind.Detail].SortFieldsInternal);
				if (destSortFields != null)
					AddGroupFields(group.GroupFields.ToArray(), destSortFields);
				RemoveBands(ReportModel, group.Header, group.Footer);
			}
		}
		public bool CanHideHeader(IGroupSortItem item) {
			var field = item.GroupField;
			var band = field.Band;
			var group = GetGroup(selectedReport, field);
			return CanHideBand(group.Header);
		}
		#endregion
		#region footer
		public void ChangeFooterVisibility(GroupField field, bool visible) {
			if (visible)
				ShowFooter(field);
			else
				HideFooter(field);
		}
		void ShowFooter(GroupField field) {
			var band = field.Band;
			var level = band.LevelInternal;
			var groupFooter = new GroupFooterBand() { Level = level };
			var item = ReportModel.Factory.GetModel(groupFooter, true).DiagramItem;
			var reportItem = selectedReport == Report ? ReportModel.DiagramItem : ReportModel.Factory.GetModel(selectedReport).DiagramItem;
			ReportModel.DiagramItem.Diagram.DrawItem(item, (IDiagramContainer)reportItem, new Rect(new Size(0.0, BoundsConverter.ToDouble(groupFooter.HeightF, groupFooter.Dpi))));
		}
		void HideFooter(GroupField field) {
			var band = field.Band;
			var group = GetGroup(selectedReport, field);
			if (group != null && CanHideBand(group.Footer)) {
				RemoveBands(ReportModel, group.Footer);
			}
		}
		public bool CanHideFooter(IGroupSortItem item) {
			var field = item.GroupField;
			var band = field.Band;
			var group = GetGroup(selectedReport, field);
			return CanHideBand(group.Footer);
		}
		#endregion
		bool CanHideBand(GroupBand band) {
			bool canRemove = true;
			if (BandContainsControls(band))
				doWithMessageBoxService(message => {
					canRemove = ShowRemoveWarning(message) == MessageResult.Yes;
				});
			return canRemove;
		}
	}
}
