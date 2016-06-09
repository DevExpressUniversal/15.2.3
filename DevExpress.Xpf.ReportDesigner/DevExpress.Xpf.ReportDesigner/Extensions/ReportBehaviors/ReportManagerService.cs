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

using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Native;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Markup;
namespace DevExpress.Xpf.Reports.UserDesigner.Extensions {
	public class PredefinedReportCollection : ObservableCollection<PredefinedReport> { }
	public class PredefinedReport {
		public Type Type { get; set; }
		public string ReportName { get; set; }
	}
	[ContentProperty("PredefinedReports")]
	public abstract class ReportManagerService : ServiceBase, IReportManagerService {
		FileSystemStore customStore;
		FileSystemStore CustomStore {
			get {
				return customStore ?? (customStore =
					new FileSystemStore(Path.Combine(System.Windows.Forms.Application.UserAppDataPath, Name)));
			}
		}
		PredefinedReportsStore predefinedStore;
		PersistentReportInfoSerializer reportInfoSerializer;
		PersistentReportSerializer reportSerializer;
		string displayName;
		public string DisplayName {
			get { return displayName ?? Name; }
			set { displayName = value; }
		}
		public PredefinedReportCollection PredefinedReports { get; private set; }
		public ReportManagerServiceViewModel ViewModel { get; private set; }
		public event EventHandler ReportsChanged;
		void RaiseReportsChanged() {
			if(ReportsChanged != null)
				ReportsChanged(this, EventArgs.Empty);
		}
		protected override void OnAttached() {
			base.OnAttached();
			RaiseReportsChanged();
		}
		public ReportManagerService() {
			reportInfoSerializer = new PersistentReportInfoSerializer();
			reportSerializer = new PersistentReportSerializer();
			PredefinedReports = new PredefinedReportCollection();
			PredefinedReports.CollectionChanged += PredefinedReports_CollectionChanged;
			predefinedStore = new PredefinedReportsStore(PredefinedReports);
			ViewModel = new ReportManagerServiceViewModel(this);
		}
		void PredefinedReports_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			RaiseReportsChanged();
		}
		public ReportInfo SaveReport(XtraReport report) {
			var id = CustomStore.AddItem(reportSerializer.Save(report), new byte[0]);
			ReportInfo info = new ReportInfo(id, false);
			CustomStore.UpdateItem(id, null, reportInfoSerializer.Save(info));
			RaiseReportsChanged();
			return info;
		}
		public XtraReport GetReport(ReportInfo id) {
			return predefinedStore.GetItem(id.Key)
				?? reportSerializer.Load(CustomStore.GetItem(id.Key));
		}
		public IEnumerable<ReportInfo> GetReports() {
			foreach(var key in predefinedStore.GetKeys()) {
				yield return predefinedStore.GetMetadata(key);
			}
			foreach(var key in CustomStore.GetKeys()) {
				var bytes = CustomStore.GetMetadata(key);
				if (bytes != null) {
					var info = reportInfoSerializer.Load(bytes);
					if(info != null) {
						yield return info;
					}
				}
			}
		}
		public void RemoveReport(ReportInfo id) {
			CustomStore.RemoveItem(id.Key);
			RaiseReportsChanged();
		}
		public void UpdateReport(ReportInfo id, XtraReport report) {
			CustomStore.UpdateItem(id.Key, reportSerializer.Save(report), reportInfoSerializer.Save(id));
			RaiseReportsChanged();
		}
		public void UpdateReportInfo(ReportInfo info) {
			CustomStore.UpdateItem(info.Key, null, reportInfoSerializer.Save(info));
			RaiseReportsChanged();
		}
		public abstract XtraReport GenerateReport(XtraReport initialReport);
		public abstract void AssignDataSource(XtraReport report);
		public virtual bool HasPreview {
			get { return false; }
		}
	}
}
