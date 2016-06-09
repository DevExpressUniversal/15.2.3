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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Native {
	abstract class FormatRulePresenterBase : IDisposable {
		readonly DataDashboardItem dashboardItem;
		readonly DataItem dataItem;
		readonly IServiceProvider serviceProvider;
		IFormatRuleBaseControlView view;
		IFormatRuleControlChanger changer;
		protected abstract IFormatRuleBaseViewInitializationContext ViewInitializationContext { get; }
		protected IFormatRuleControlChanger Changer {
			get { return changer; }
			set {
				if(value != changer) {
					UnsubscribeChangerEvents();
					changer = value;
					RefreshChanger();
				}
			}
		}
		protected IFormatRuleBaseControlView View {
			get { return view; }
			set {
				if(value != view) {
					UnsubscribeViewEvents();
					view = value;
					RefreshView();
				}
			}
		}
		protected abstract string Caption { get; }
		protected IServiceProvider ServiceProvider { get { return serviceProvider; } }
		protected Dashboard Dashboard { get { return serviceProvider.RequestServiceStrictly<IDashboardOwnerService>().Dashboard; } }
		protected DataDashboardItem DashboardItem { get { return dashboardItem; } }
		protected PivotDashboardItem PivotDashboardItem { get { return dashboardItem as PivotDashboardItem; } }
		protected DataItem DataItem { get { return dataItem; } }
		protected IDashboardDesignerHistoryService HistoryService { get { return serviceProvider.RequestServiceStrictly<IDashboardDesignerHistoryService>(); } }
		protected UserLookAndFeel LookAndFeel { get { return ServiceProvider.RequestServiceStrictly<IDashboardGuiContextService>().LookAndFeel; } }
		protected bool IsDarkSkin { get { return DashboardWinHelper.IsDarkScheme(LookAndFeel); } }
		protected FormatConditionColorScheme Scheme {
			get { return IsDarkSkin ? FormatConditionColorScheme.Dark : FormatConditionColorScheme.Light; }
		}
		protected FormatRulePresenterBase(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");			
			this.serviceProvider = serviceProvider;
			this.dashboardItem = dashboardItem;
			this.dataItem = dataItem;
		}
		public void Initialize(IFormatRuleBaseControlView view, IFormatRuleControlChanger changer) {
			Changer = changer;
			View = view;
		}
		public virtual void Dispose() {
			View = null;
			Changer = null;
		}
		protected abstract bool? InitializeView();
		protected abstract void ApplyView();
		protected abstract void ApplyHistory();
		protected virtual void OnCreated() {
		}
		protected virtual void OnDestroyed() {
		}
		void RefreshChanger() {
			if(changer != null) {
				changer.Refresh(Caption);
				SubscribeChangerEvents();
			}
		}
		void RefreshView() {
			if(view != null) {
				view.Initialize(ViewInitializationContext);
				bool? isApplyable = InitializeView();
				Changer.Enable(isApplyable);
				SubscribeViewEvents();
			}
		}
		void OnStateUpdated(object sender, ViewStateChangedEventArgs e) {
			Changer.Enable(View.IsValid ? (bool?)true : null);
		}
		void OnChanged(object sender, EventArgs e) {
			Changer.Enable(View.IsValid ? (bool?)false : null);
			ApplyView();
			ApplyHistory();
		}
		void OnCreated(object sender, EventArgs e) {
			OnCreated();
		}
		void OnDestroyed(object sender, EventArgs e) {
			OnDestroyed();
		}
		protected virtual void SubscribeViewEvents() {
			if(view != null) {
				view.StateUpdated += OnStateUpdated;
			}
		}
		protected virtual void UnsubscribeViewEvents() {
			if(view != null) {
				view.StateUpdated -= OnStateUpdated;
			}
		}
		void SubscribeChangerEvents() {
			if(changer != null) {
				changer.Changed += OnChanged;
				changer.Created += OnCreated;
				changer.Destroyed += OnDestroyed;
			}
		}
		void UnsubscribeChangerEvents() {
			if(changer != null) {
				changer.Changed -= OnChanged;
				changer.Created -= OnCreated;
				changer.Destroyed -= OnDestroyed;
			}
		}
	}
}
