#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
namespace DevExpress.ExpressApp.ReportsV2.Win.Editors {
	public class SortingCollectionEditor : CollectionEditor {
		public class Sorting {
			string property;
			SortingDirection direction;
			ISortingPropertyDescriptorProvider propertyDescriptorProvider;
			public Sorting() {
			}
			public Sorting(ISortingPropertyDescriptorProvider propertyDescriptorProvider, string val, SortingDirection direction) {
				property = val;
				this.propertyDescriptorProvider = propertyDescriptorProvider;
				this.direction = direction;
			}
			[Browsable(false)]
			public ISortingPropertyDescriptorProvider PropertyDescriptorProvider {
				get {
					return propertyDescriptorProvider;
				}
			}
			[DefaultValue(SortingDirection.Ascending)]
			public SortingDirection Direction {
				get {
					return direction;
				}
				set {
					direction = value;
				}
			}
			[Browsable(false)]
			public string Name {
				get {
					return Property == String.Empty || Property == null ? "empty" : Property;
				}
			}
			[Category()]
			[Editor(typeof(PropertyNameEditor), typeof(UITypeEditor))]
			public string Property {
				get {
					return property;
				}
				set {
					property = value;
				}
			}
		}
		public SortingCollectionEditor()
			: base(typeof(object[])) {
		}
		protected override Type CreateCollectionItemType() {
			return typeof(Sorting);
		}
		protected override object CreateInstance(Type itemType) {
			return new Sorting(propertyDescriptorProvider, null, SortingDirection.Ascending);
		}
		protected override bool CanSelectMultipleInstances() {
			return false;
		}
		ISortingPropertyDescriptorProvider propertyDescriptorProvider;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			try {
				Guard.ArgumentNotNull(value, "value");
				Guard.TypeArgumentIs(typeof(SortingCollection), value.GetType(), "value");
				propertyDescriptorProvider = (ISortingPropertyDescriptorProvider)context.Instance;
				return base.EditValue(context, provider, value);
			}
			finally {
				propertyDescriptorProvider = null;
			}
		}
		protected override object[] GetItems(object editValue) {
			List<object> entries = new List<object>();
			foreach(SortProperty p in (SortingCollection)editValue)
				entries.Add(new Sorting(propertyDescriptorProvider, p.PropertyName, p.Direction));
			return entries.ToArray();
		}
		protected override object SetItems(object editValue, object[] value) {
			SortingCollection values = new SortingCollection();
			foreach(Sorting e in value) {
				values.Add(new SortProperty(e.Property, e.Direction));
			}
			return values;
		}
	}
}
