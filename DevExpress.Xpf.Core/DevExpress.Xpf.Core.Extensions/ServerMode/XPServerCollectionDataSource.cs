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
namespace DevExpress.Xpf.Core.DataSources {
	public class XPServerCollectionDataSource : XPOSourceBaseWithSession {
		public static readonly DependencyProperty DefaultSortingProperty;
		public static readonly DependencyProperty DisplayablePropertiesProperty;
		public static readonly DependencyProperty AllowEditProperty;
		public static readonly DependencyProperty AllowNewProperty;
		public static readonly DependencyProperty AllowRemoveProperty;
		public static readonly DependencyProperty DeleteObjectOnRemoveProperty;
		public static readonly DependencyProperty FixedFilterCriteriaProperty;
		public static readonly DependencyProperty FixedFilterStringProperty;
		public static readonly DependencyProperty TrackChangesProperty;
		static XPServerCollectionDataSource() {
			Type ownerclass = typeof(XPServerCollectionDataSource);
			DefaultSortingProperty = DependencyProperty.Register("DefaultSorting", typeof(string), ownerclass, new PropertyMetadata(null, (d, e) => ((XPServerCollectionDataSource)d).OnSettingsChanged()));
			DisplayablePropertiesProperty = DependencyProperty.Register("DisplayableProperties", typeof(string), ownerclass, new PropertyMetadata(null, (d, e) => ((XPServerCollectionDataSource)d).OnSettingsChanged()));
			AllowEditProperty = DependencyProperty.Register("AllowEdit", typeof(bool), ownerclass, new PropertyMetadata(false, (d, e) => ((XPServerCollectionDataSource)d).OnSettingsChanged()));
			AllowNewProperty = DependencyProperty.Register("AllowNew", typeof(bool), ownerclass, new PropertyMetadata(false, (d, e) => ((XPServerCollectionDataSource)d).OnSettingsChanged()));
			AllowRemoveProperty = DependencyProperty.Register("AllowRemove", typeof(bool), ownerclass, new PropertyMetadata(false, (d, e) => ((XPServerCollectionDataSource)d).OnSettingsChanged()));
			DeleteObjectOnRemoveProperty = DependencyProperty.Register("DeleteObjectOnRemove", typeof(bool), ownerclass, new PropertyMetadata(false, (d, e) => ((XPServerCollectionDataSource)d).OnSettingsChanged()));
			FixedFilterCriteriaProperty = DependencyProperty.Register("FixedFilterCriteria", typeof(CriteriaOperator), ownerclass, new PropertyMetadata(null, (d, e) => ((XPServerCollectionDataSource)d).OnSettingsChanged()));
			FixedFilterStringProperty = DependencyProperty.Register("FixedFilterString", typeof(string), ownerclass, new PropertyMetadata(null, (d, e) => ((XPServerCollectionDataSource)d).OnSettingsChanged()));
			TrackChangesProperty = DependencyProperty.Register("TrackChanges", typeof(bool), ownerclass, new PropertyMetadata(false, (d, e) => ((XPServerCollectionDataSource)d).OnSettingsChanged()));
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPServerCollectionDataSource,DefaultSorting")]
		public string DefaultSorting {
			get { return (string)GetValue(DefaultSortingProperty); }
			set { SetValue(DefaultSortingProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPServerCollectionDataSource,DisplayableProperties")]
		public string DisplayableProperties {
			get { return (string)GetValue(DisplayablePropertiesProperty); }
			set { SetValue(DisplayablePropertiesProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPServerCollectionDataSource,AllowEdit")]
		public bool AllowEdit {
			get { return (bool)GetValue(AllowEditProperty); }
			set { SetValue(AllowEditProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPServerCollectionDataSource,AllowNew")]
		public bool AllowNew {
			get { return (bool)GetValue(AllowNewProperty); }
			set { SetValue(AllowNewProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPServerCollectionDataSource,AllowRemove")]
		public bool AllowRemove {
			get { return (bool)GetValue(AllowRemoveProperty); }
			set { SetValue(AllowRemoveProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPServerCollectionDataSource,DeleteObjectOnRemove")]
		public bool DeleteObjectOnRemove {
			get { return (bool)GetValue(DeleteObjectOnRemoveProperty); }
			set { SetValue(DeleteObjectOnRemoveProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPServerCollectionDataSource,FixedFilterCriteria")]
		public CriteriaOperator FixedFilterCriteria {
			get { return (CriteriaOperator)GetValue(FixedFilterCriteriaProperty); }
			set { SetValue(FixedFilterCriteriaProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPServerCollectionDataSource,FixedFilterString")]
		public string FixedFilterString {
			get { return (string)GetValue(FixedFilterStringProperty); }
			set { SetValue(FixedFilterStringProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPServerCollectionDataSource,TrackChanges")]
		public bool TrackChanges {
			get { return (bool)GetValue(TrackChangesProperty); }
			set { SetValue(TrackChangesProperty, value); }
		}
		protected override object CreateCollection(Session session) {
			if(ObjectType == null)
				return null;
			XPServerCollectionSource newSource = new XPServerCollectionSource(session, ObjectType);
			newSource.DefaultSorting = DefaultSorting;
			newSource.DisplayableProperties = DisplayableProperties;
			newSource.AllowEdit = AllowEdit;
			newSource.AllowNew = AllowNew;
			newSource.AllowRemove = AllowRemove;
			newSource.DeleteObjectOnRemove = DeleteObjectOnRemove;
			newSource.FixedFilterCriteria = FixedFilterCriteria;
			newSource.FixedFilterString = FixedFilterString;
			newSource.TrackChanges = TrackChanges;
			return newSource;
		}
	}
}
