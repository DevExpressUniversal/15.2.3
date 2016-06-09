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

using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.DataAccess.Native;
using System;
using System.ComponentModel.Design;
using System.IO;
namespace DevExpress.DashboardWin.ServiceModel {
	public interface IDashboardOwnerService : IDisposable {
		Dashboard Dashboard { get; }
		bool IsInternal { get; }
		bool IsDashboardEmpty { get; }
		event EventHandler<DashboardChangedEventArgs> DashboardChanged;
		void CreateDashboard();
		void CreateDashboard(Type type);
		void CreateDashboard(Uri uri, string sourceDirectory);
		void CreateDashboard(Stream stream);
		void CreateDashboard(string filePath);
		void SetDashboard(Dashboard dashboard);
		DashboardItem FindDashboardItemOrGroup(string componentName);
		DashboardItem GetFirstDashboardItemOrGroup();
		void ForceDashboardDesignTime(IDesignerHost designerHost);
	}
	public class DashboardOwnerService : IDashboardOwnerService {
		Dashboard dashboard;
		bool isInternal;
		IDesignerHost designerHost;
		public Dashboard Dashboard { get { return dashboard; } }
		public bool IsInternal { get { return isInternal; } }
		public bool IsDashboardEmpty { get { return dashboard.IsEmpty; } }
		bool InDesignerTransaction { get { return designerHost != null && designerHost.InTransaction; } }
		public event EventHandler<DashboardChangedEventArgs> DashboardChanged;
		public void CreateDashboard() {
			SetDashboard(new Dashboard(), true);
		}
		public void CreateDashboard(Type type) {
			Dashboard dashboard = type != null ? Activator.CreateInstance(type) as Dashboard : null;
			SetDashboard(dashboard, true);
		}
		public void CreateDashboard(Uri uri, string sourceDirectory) {
			Dashboard dashboard = new DashboardLoader().Load(uri, sourceDirectory);
			SetDashboard(dashboard, true);
		}
		public void CreateDashboard(Stream stream) {
			Dashboard dashboard = new Dashboard();
			dashboard.LoadFromXml(stream);
			SetDashboard(dashboard, true);
		}
		public void CreateDashboard(string filePath) {
			Dashboard dashboard = new Dashboard();
			dashboard.LoadFromXml(filePath);
			SetDashboard(dashboard, true);
		}
		public void SetDashboard(Dashboard dashboard) {
			SetDashboard(dashboard, false);
		}
		public DashboardItem FindDashboardItemOrGroup(string componentName) {
			return dashboard.Items[componentName] ?? dashboard.Groups[componentName];
		}
		public DashboardItem GetFirstDashboardItemOrGroup() {
			if(dashboard.Items.Count > 0)
				return dashboard.Items[0];
			if(dashboard.Groups.Count > 0)
				return dashboard.Groups[0];
			return null;
		}
		public void Dispose() {
			UnsubscribeDashboardEvents(dashboard);
			DisposeDashboard(dashboard, isInternal);
			dashboard = null;
		}
		public void ForceDashboardDesignTime(IDesignerHost designerHost) {
			this.designerHost = designerHost;
		}
		void SetDashboard(Dashboard dashboard, bool isInternal) {
			if(dashboard != this.dashboard) {
				Dashboard oldDashboard = this.dashboard;
				bool oldIsInternal = this.isInternal;
				this.dashboard = dashboard;
				this.isInternal = isInternal;
				UnsubscribeDashboardEvents(oldDashboard);				
				SubscribeDashboardEvents(dashboard);
				RaiseDashboardChanged(oldDashboard, dashboard);
				DisposeDashboard(oldDashboard, oldIsInternal);
			}
		}
		void SubscribeDashboardEvents(Dashboard dashboard) {
			if(dashboard != null)
				dashboard.LoadCompleted += OnDashboardLoadCompleted;
		}
		void UnsubscribeDashboardEvents(Dashboard dashboard) {
			if(dashboard != null)
				dashboard.LoadCompleted -= OnDashboardLoadCompleted;
		}
		void DisposeDashboard(Dashboard dashboard, bool isInternal) {
			if(dashboard != null && isInternal)
				dashboard.Dispose();
		}
		void OnDashboardLoadCompleted(object sender, EventArgs e) {
			if (!InDesignerTransaction) 
				RaiseDashboardChanged(dashboard, dashboard);
		}
		void RaiseDashboardChanged(Dashboard oldDashboard, Dashboard newDashboard) {
			if(DashboardChanged != null)
				DashboardChanged(this, new DashboardChangedEventArgs(oldDashboard, newDashboard));
		}
	}
	public class DashboardChangedEventArgs : EventArgs {
		public Dashboard OldDashboard { get; private set; }
		public Dashboard NewDashboard { get; private set; }
		public DashboardChangedEventArgs(Dashboard oldDashboard, Dashboard newDashboard) {
			OldDashboard = oldDashboard;
			NewDashboard = newDashboard;
		}
	}
}
