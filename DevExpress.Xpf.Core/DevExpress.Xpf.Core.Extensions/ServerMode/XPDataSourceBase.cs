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

using DevExpress.Xpf.Core.DataSources;
using System;
using System.ComponentModel;
using System.Windows;
using DevExpress.Data.Filtering;
using System.Collections;
using DevExpress.Xpo;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core.DataSources {
	public abstract class DataSourceWithDataBase : SimpleDataSourceBase {
		public static readonly DependencyProperty DataProperty;
		static DataSourceWithDataBase() {
			Type ownerclass = typeof(DataSourceWithDataBase);
			DataProperty = DependencyProperty.Register("Data", typeof(object), ownerclass, new PropertyMetadata(null));
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.DataSourceWithDataBase,Data")]
		public object Data {
			get { return (object)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
		protected override object DataCore {
			get { return Data; }
			set { Data = value; }
		}
		bool initialized = false;
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			initialized = true;
			OnSettingsChanged();
		}
		protected void OnSettingsChanged() {
			if(initialized)
				RefreshDataSource();
		}
		public abstract void RefreshDataSource();
		protected override object UpdateDataCore() {
			return Data;
		}
	}
	public abstract class XPOSourceBase : DataSourceWithDataBase {
		public static readonly DependencyProperty ObjectTypeProperty;
		static XPOSourceBase() {
			Type ownerclass = typeof(XPOSourceBase);
			ObjectTypeProperty = DependencyProperty.Register("ObjectType", typeof(Type), ownerclass, new PropertyMetadata(null, (d, e) => ((XPOSourceBase)d).OnSettingsChanged()));
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPOSourceBase,ObjectType")]
		public Type ObjectType {
			get { return (Type)GetValue(ObjectTypeProperty); }
			set { SetValue(ObjectTypeProperty, value); }
		}
		public override void RefreshDataSource() {
			RefreshSession();
			if(Data != null) {
				IDisposable oldSource = Data as IDisposable;
				ClearOldSource();
				oldSource.Dispose();
				Data = null;
			}
			if(DesignerProperties.GetIsInDesignMode(this)) {
				UpdateData();
				return;
			}
			Data = CreateCollection(GetSession());
		}
		protected virtual void ClearOldSource() { }
		protected abstract void RefreshSession();
		protected abstract Session GetSession();
		protected abstract object CreateCollection(Session session);
		protected override object CreateDesignTimeDataSourceCore() {
			if(ObjectType == null)
				return null;
			return new BaseGridDesignTimeDataSource(ObjectType, DesignData.RowCount, DesignData.UseDistinctValues, null, null, null);
		}
	}
	public abstract class XPOSourceBaseWithSession : XPOSourceBase {
		public static readonly DependencyProperty SessionProperty;
		static XPOSourceBaseWithSession() {
			Type ownerclass = typeof(XPOSourceBaseWithSession);
			SessionProperty = DependencyProperty.Register("Session", typeof(Session), ownerclass, new PropertyMetadata(null, (d, e) => ((XPOSourceBaseWithSession)d).OnSettingsChanged()));
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPOSourceBaseWithSession,Session")]
		public Session Session {
			get { return (Session)GetValue(SessionProperty); }
			set { SetValue(SessionProperty, value); }
		}
		Session internalSession = null;
		protected override void RefreshSession() {
			if(Session == null)
				internalSession = new UnitOfWork();
			else {
				if((internalSession != null) && internalSession != Session)
					internalSession.Dispose();
				internalSession = Session;
			}
		}
		protected override Session GetSession() {
			return internalSession;
		}
	}
	public abstract class XPDataSourceBase : XPOSourceBaseWithSession {
		public static readonly DependencyProperty CaseSensitiveProperty;
		public static readonly DependencyProperty CriteriaProperty;
		public static readonly DependencyProperty SelectDeletedProperty;
		public static readonly DependencyProperty SortingProperty;
		static XPDataSourceBase() {
			Type ownerclass = typeof(XPDataSourceBase);
			CaseSensitiveProperty = DependencyProperty.Register("CaseSensitive", typeof(bool), ownerclass, new PropertyMetadata(false, (d, e) => ((XPDataSourceBase)d).OnSettingsChanged()));
			CriteriaProperty = DependencyProperty.Register("Criteria", typeof(CriteriaOperator), ownerclass, new PropertyMetadata(null, (d, e) => ((XPDataSourceBase)d).OnSettingsChanged()));
			SelectDeletedProperty = DependencyProperty.Register("SelectDeleted", typeof(bool), ownerclass, new PropertyMetadata(false, (d, e) => ((XPDataSourceBase)d).OnSettingsChanged()));
			SortingProperty = DependencyProperty.Register("Sorting", typeof(List<XPViewDataSourceSortProperty>), ownerclass, new PropertyMetadata(null, (d, e) => ((XPDataSourceBase)d).OnSettingsChanged()));
		}
		public XPDataSourceBase() {
			Sorting = new List<XPViewDataSourceSortProperty>();
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPDataSourceBase,CaseSensitive")]
		public bool CaseSensitive {
			get { return (bool)GetValue(CaseSensitiveProperty); }
			set { SetValue(CaseSensitiveProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPDataSourceBase,Criteria")]
		public CriteriaOperator Criteria {
			get { return (CriteriaOperator)GetValue(CriteriaProperty); }
			set { SetValue(CriteriaProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPDataSourceBase,SelectDeleted")]
		public bool SelectDeleted {
			get { return (bool)GetValue(SelectDeletedProperty); }
			set { SetValue(SelectDeletedProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPDataSourceBase,Sorting")]
		public List<XPViewDataSourceSortProperty> Sorting {
			get { return (List<XPViewDataSourceSortProperty>)GetValue(SortingProperty); }
			set { SetValue(SortingProperty, value); }
		}
		protected SortProperty[] GetSorting() {
			List<SortProperty> sorting = new List<SortProperty>();
			Sorting.ForEach(p => sorting.Add(p.ToSortProperty()));
			return sorting.ToArray();
		}
	}
}
