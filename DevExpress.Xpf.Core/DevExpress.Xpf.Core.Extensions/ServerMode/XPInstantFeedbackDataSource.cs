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
	public class XPInstantFeedbackDataSource : XPOSourceBase {
		public static readonly DependencyProperty DefaultSortingProperty;
		public static readonly DependencyProperty DisplayablePropertiesProperty;
		public static readonly DependencyProperty FixedFilterCriteriaProperty;
		public static readonly DependencyProperty FixedFilterStringProperty;
		static XPInstantFeedbackDataSource() {
			Type ownerclass = typeof(XPInstantFeedbackDataSource);
			DefaultSortingProperty = DependencyProperty.Register("DefaultSorting", typeof(string), ownerclass, new PropertyMetadata(null, (d, e) => ((XPInstantFeedbackDataSource)d).OnSettingsChanged()));
			DisplayablePropertiesProperty = DependencyProperty.Register("DisplayableProperties", typeof(string), ownerclass, new PropertyMetadata(null, (d, e) => ((XPInstantFeedbackDataSource)d).OnSettingsChanged()));
			FixedFilterCriteriaProperty = DependencyProperty.Register("FixedFilterCriteria", typeof(CriteriaOperator), ownerclass, new PropertyMetadata(null, (d, e) => ((XPInstantFeedbackDataSource)d).OnSettingsChanged()));
			FixedFilterStringProperty = DependencyProperty.Register("FixedFilterString", typeof(string), ownerclass, new PropertyMetadata(null, (d, e) => ((XPInstantFeedbackDataSource)d).OnSettingsChanged()));
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPInstantFeedbackDataSource,DefaultSorting")]
		public string DefaultSorting {
			get { return (string)GetValue(DefaultSortingProperty); }
			set { SetValue(DefaultSortingProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPInstantFeedbackDataSource,DisplayableProperties")]
		public string DisplayableProperties {
			get { return (string)GetValue(DisplayablePropertiesProperty); }
			set { SetValue(DisplayablePropertiesProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPInstantFeedbackDataSource,FixedFilterCriteria")]
		public CriteriaOperator FixedFilterCriteria {
			get { return (CriteriaOperator)GetValue(FixedFilterCriteriaProperty); }
			set { SetValue(FixedFilterCriteriaProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.XPInstantFeedbackDataSource,FixedFilterString")]
		public string FixedFilterString {
			get { return (string)GetValue(FixedFilterStringProperty); }
			set { SetValue(FixedFilterStringProperty, value); }
		}
		protected override void ClearOldSource() {
			base.ClearOldSource();
			XPInstantFeedbackSource source = Data as XPInstantFeedbackSource;
			if(source == null)
				return;
			source.ResolveSession -= newSource_ResolveSession;
			source.DismissSession -= newSource_DismissSession;
		}
		protected override void RefreshSession() { }
		protected override Session GetSession() { return null; }
		protected override object CreateCollection(Session session) {
			if(ObjectType == null)
				return null;
			XPInstantFeedbackSource newSource = new XPInstantFeedbackSource(ObjectType);
			newSource.ResolveSession += newSource_ResolveSession;
			newSource.DismissSession += newSource_DismissSession;
			newSource.DefaultSorting = DefaultSorting;
			newSource.DisplayableProperties = DisplayableProperties;
			newSource.FixedFilterCriteria = FixedFilterCriteria;
			newSource.FixedFilterString = FixedFilterString;
			return newSource;
		}
		void newSource_DismissSession(object sender, ResolveSessionEventArgs e) {
			if(DismissSession == null)
				return;
			Dispatcher.BeginInvoke(new Action(() => DismissSession(Data, e)));
		}
		void newSource_ResolveSession(object sender, ResolveSessionEventArgs e) {
			if(ResolveSession == null)
				return;
			Dispatcher.BeginInvoke(new Action(() => ResolveSession(Data, e)));
		}
		public event EventHandler<ResolveSessionEventArgs> DismissSession;
		public event EventHandler<ResolveSessionEventArgs> ResolveSession;
	}
}
