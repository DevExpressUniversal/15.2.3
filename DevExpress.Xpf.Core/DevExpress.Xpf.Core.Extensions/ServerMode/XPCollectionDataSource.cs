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
	public class XPCollectionDataSource : XPDataSourceBase {
		public static readonly DependencyProperty DisplayablePropertiesProperty;
		public static readonly DependencyProperty BindingBehaviorProperty;
		public static readonly DependencyProperty DeleteObjectOnRemoveProperty;
		public static readonly DependencyProperty LoadingEnabledProperty;
		public static readonly DependencyProperty TopReturnedObjectsProperty;
		public static readonly DependencyProperty SkipReturnedObjectsProperty;
		static XPCollectionDataSource() {
			Type ownerclass = typeof(XPCollectionDataSource);
			DisplayablePropertiesProperty = DependencyProperty.Register("DisplayableProperties", typeof(string), ownerclass, new PropertyMetadata(null, (d, e) => ((XPCollectionDataSource)d).OnSettingsChanged()));
			BindingBehaviorProperty = DependencyProperty.Register("BindingBehavior", typeof(CollectionBindingBehavior), ownerclass, new PropertyMetadata(CollectionBindingBehavior.AllowNone, (d, e) => ((XPCollectionDataSource)d).OnSettingsChanged()));
			DeleteObjectOnRemoveProperty = DependencyProperty.Register("DeleteObjectOnRemove", typeof(bool), ownerclass, new PropertyMetadata(false, (d, e) => ((XPCollectionDataSource)d).OnSettingsChanged()));
			LoadingEnabledProperty = DependencyProperty.Register("LoadingEnabled", typeof(bool), ownerclass, new PropertyMetadata(true, (d, e) => ((XPCollectionDataSource)d).OnSettingsChanged()));
			TopReturnedObjectsProperty = DependencyProperty.Register("TopReturnedObjects", typeof(int), ownerclass, new PropertyMetadata(0, (d, e) => ((XPCollectionDataSource)d).OnSettingsChanged()));
			SkipReturnedObjectsProperty = DependencyProperty.Register("SkipReturnedObjects", typeof(int), ownerclass, new PropertyMetadata(0, (d, e) => ((XPCollectionDataSource)d).OnSettingsChanged()));
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPCollectionDataSource,DisplayableProperties")]
		public string DisplayableProperties {
			get { return (string)GetValue(DisplayablePropertiesProperty); }
			set { SetValue(DisplayablePropertiesProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPCollectionDataSource,BindingBehavior")]
		public CollectionBindingBehavior BindingBehavior {
			get { return (CollectionBindingBehavior)GetValue(BindingBehaviorProperty); }
			set { SetValue(BindingBehaviorProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPCollectionDataSource,DeleteObjectOnRemove")]
		public bool DeleteObjectOnRemove {
			get { return (bool)GetValue(DeleteObjectOnRemoveProperty); }
			set { SetValue(DeleteObjectOnRemoveProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPCollectionDataSource,LoadingEnabled")]
		public bool LoadingEnabled {
			get { return (bool)GetValue(LoadingEnabledProperty); }
			set { SetValue(LoadingEnabledProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPCollectionDataSource,TopReturnedObjects")]
		public int TopReturnedObjects {
			get { return (int)GetValue(TopReturnedObjectsProperty); }
			set { SetValue(TopReturnedObjectsProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPCollectionDataSource,SkipReturnedObjects")]
		public int SkipReturnedObjects {
			get { return (int)GetValue(SkipReturnedObjectsProperty); }
			set { SetValue(SkipReturnedObjectsProperty, value); }
		}
		protected override object CreateCollection(Session session) {
			if(ObjectType == null)
				return null;
			XPCollection newSource = new XPCollection(session, ObjectType);
			newSource.CaseSensitive = CaseSensitive;
			newSource.Criteria = Criteria;
			newSource.SelectDeleted = SelectDeleted;
			newSource.SkipReturnedObjects = SkipReturnedObjects;
			newSource.Sorting.AddRange(GetSorting());
			newSource.DisplayableProperties = DisplayableProperties;
			newSource.BindingBehavior = BindingBehavior;
			newSource.DeleteObjectOnRemove = DeleteObjectOnRemove;
			newSource.LoadingEnabled = LoadingEnabled;
			newSource.TopReturnedObjects = TopReturnedObjects;
			return newSource;
		}
	}
}
