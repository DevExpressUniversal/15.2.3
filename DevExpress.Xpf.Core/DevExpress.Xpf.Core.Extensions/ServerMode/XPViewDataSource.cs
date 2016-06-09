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
using DevExpress.Xpo.DB;
namespace DevExpress.Xpf.Core.DataSources {
	public class XPViewDataSource : XPDataSourceBase {
		public static readonly DependencyProperty GroupCriteriaProperty;
		public static readonly DependencyProperty PropertiesProperty;
		public static readonly DependencyProperty SkipReturnedRecordsProperty;
		public static readonly DependencyProperty TopReturnedRecordsProperty;
		static XPViewDataSource() {
			Type ownerclass = typeof(XPViewDataSource);
			GroupCriteriaProperty = DependencyProperty.Register("GroupCriteria", typeof(CriteriaOperator), ownerclass, new PropertyMetadata(null, (d, e) => ((XPViewDataSource)d).OnSettingsChanged()));
			PropertiesProperty = DependencyProperty.Register("Properties", typeof(List<XPViewDataSourceProperty>), ownerclass, new PropertyMetadata(null, (d, e) => ((XPViewDataSource)d).OnSettingsChanged()));
			SkipReturnedRecordsProperty = DependencyProperty.Register("SkipReturnedRecords", typeof(int), ownerclass, new PropertyMetadata(0, (d, e) => ((XPViewDataSource)d).OnSettingsChanged()));
			TopReturnedRecordsProperty = DependencyProperty.Register("TopReturnedRecords", typeof(int), ownerclass, new PropertyMetadata(0, (d, e) => ((XPViewDataSource)d).OnSettingsChanged()));
		}
		public XPViewDataSource() : base() {
			Properties = new List<XPViewDataSourceProperty>();
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPViewDataSource,GroupCriteria")]
		public CriteriaOperator GroupCriteria {
			get { return (CriteriaOperator)GetValue(GroupCriteriaProperty); }
			set { SetValue(GroupCriteriaProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPViewDataSource,Properties")]
		public List<XPViewDataSourceProperty> Properties {
			get { return (List<XPViewDataSourceProperty>)GetValue(PropertiesProperty); }
			set { SetValue(PropertiesProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPViewDataSource,SkipReturnedRecords")]
		public int SkipReturnedRecords {
			get { return (int)GetValue(SkipReturnedRecordsProperty); }
			set { SetValue(SkipReturnedRecordsProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPViewDataSource,TopReturnedRecords")]
		public int TopReturnedRecords {
			get { return (int)GetValue(TopReturnedRecordsProperty); }
			set { SetValue(TopReturnedRecordsProperty, value); }
		}
		protected override object CreateCollection(Session session) {
			if(ObjectType == null)
				return null;
			XPView newSource = new XPView(session, ObjectType);
			List<ViewProperty> properties = new List<ViewProperty>();
			Properties.ForEach(p => properties.Add(p.ToViewProperty()));
			newSource.Properties.AddRange(properties.ToArray());
			newSource.CaseSensitive = CaseSensitive;
			newSource.Criteria = Criteria;
			newSource.SelectDeleted = SelectDeleted;
			newSource.SkipReturnedRecords = SkipReturnedRecords;
			newSource.Sorting.AddRange(GetSorting());
			newSource.TopReturnedRecords = TopReturnedRecords;
			return newSource;
		}
	}
	public class XPViewDataSourceProperty : DependencyObject {
		public static readonly DependencyProperty FetchProperty;
		public static readonly DependencyProperty GroupProperty;
		public static readonly DependencyProperty NameProperty;
		public static readonly DependencyProperty PropertyProperty;
		public static readonly DependencyProperty SortingProperty;
		[Category(DXDesignTimeControl.DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPViewDataSourceProperty,Fetch")]
		public bool Fetch {
			get { return (bool)GetValue(FetchProperty); }
			set { SetValue(FetchProperty, value); }
		}
		[Category(DXDesignTimeControl.DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPViewDataSourceProperty,Group")]
		public bool Group {
			get { return (bool)GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}
		[Category(DXDesignTimeControl.DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPViewDataSourceProperty,Name")]
		public string Name {
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		[Category(DXDesignTimeControl.DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPViewDataSourceProperty,Property")]
		public CriteriaOperator Property {
			get { return (CriteriaOperator)GetValue(PropertyProperty); }
			set { SetValue(PropertyProperty, value); }
		}
		[Category(DXDesignTimeControl.DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPViewDataSourceProperty,Sorting")]
		public SortDirection Sorting {
			get { return (SortDirection)GetValue(SortingProperty); }
			set { SetValue(SortingProperty, value); }
		}
		static XPViewDataSourceProperty() {
			Type ownerclass = typeof(XPViewDataSourceProperty);
			FetchProperty = DependencyProperty.Register("Fetch", typeof(bool), ownerclass, new PropertyMetadata(true));
			GroupProperty = DependencyProperty.Register("Group", typeof(bool), ownerclass, new PropertyMetadata(false));
			NameProperty = DependencyProperty.Register("Name", typeof(string), ownerclass, new PropertyMetadata(null));
			PropertyProperty = DependencyProperty.Register("Property", typeof(CriteriaOperator), ownerclass, new PropertyMetadata(null));
			SortingProperty = DependencyProperty.Register("Sorting", typeof(SortDirection), ownerclass, new PropertyMetadata(SortDirection.None));
		}
		public ViewProperty ToViewProperty() {
			return new ViewProperty(Name, Sorting, Property ?? CriteriaOperator.Parse("[" + Name + "]"), Group, Fetch);
		}
	}
	public class XPViewDataSourceSortProperty : DependencyObject {
		public static readonly DependencyProperty DirectionProperty;
		public static readonly DependencyProperty PropertyProperty;
		public static readonly DependencyProperty PropertyNameProperty;
		[Category(DXDesignTimeControl.DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPViewDataSourceSortProperty,Direction")]
		public SortingDirection Direction {
			get { return (SortingDirection)GetValue(DirectionProperty); }
			set { SetValue(DirectionProperty, value); }
		}
		[Category(DXDesignTimeControl.DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPViewDataSourceSortProperty,Property")]
		public CriteriaOperator Property {
			get { return (CriteriaOperator)GetValue(PropertyProperty); }
			set { SetValue(PropertyProperty, value); }
		}
		[Category(DXDesignTimeControl.DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPViewDataSourceSortProperty,PropertyName")]
		public string PropertyName {
			get { return (string)GetValue(PropertyNameProperty); }
			set { SetValue(PropertyNameProperty, value); }
		}
		static XPViewDataSourceSortProperty() {
			Type ownerclass = typeof(XPViewDataSourceSortProperty);
			DirectionProperty = DependencyProperty.Register("Direction", typeof(SortingDirection), ownerclass, new PropertyMetadata(SortingDirection.Ascending));
			PropertyProperty = DependencyProperty.Register("Property", typeof(CriteriaOperator), ownerclass, new PropertyMetadata(null));
			PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), ownerclass, new PropertyMetadata(null));
		}
		public SortProperty ToSortProperty() {
			return new SortProperty(Property ?? CriteriaOperator.Parse("[" + PropertyName + "]"), Direction);
		}
	}
}
