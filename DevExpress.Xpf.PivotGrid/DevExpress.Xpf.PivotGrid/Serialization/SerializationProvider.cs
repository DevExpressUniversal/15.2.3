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

using System;
using DevExpress.Xpf.Core.Serialization;
using System.Windows;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Serializing;
#if SL
using DevExpress.Data.Browsing;
#else
using System.ComponentModel;
#endif
namespace DevExpress.Xpf.PivotGrid.Serialization {
	public class PivotSerializationProvider : SerializationProvider {
		protected override void OnClearCollection(XtraItemRoutedEventArgs e) {
			PivotSerializationController controller = GetController(e.Source);
			controller.OnClearCollection(e);
		}
		protected override object OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			IXtraSupportDeserializeCollectionItem supportDeserializeCollectionItem = e.Owner as IXtraSupportDeserializeCollectionItem;
			if(supportDeserializeCollectionItem != null)
				return supportDeserializeCollectionItem.CreateCollectionItem(e.CollectionName, new XtraItemEventArgs(e.RootObject, e.Owner, e.Collection, e.Item));
			PivotSerializationController controller = GetController(e.Source);
			return controller.OnCreateCollectionItem(e);
		}
		protected override object OnFindCollectionItem(XtraFindCollectionItemEventArgs e) {
			PivotSerializationController controller = GetController(e.Source);
			return controller.OnFindCollectionItem(e);
		}
		protected override void OnStartDeserializing(DependencyObject obj, DevExpress.Utils.LayoutAllowEventArgs e) {
			base.OnStartDeserializing(obj, e);
			PivotSerializationController controller = GetController(obj);
			if(controller != null)
				controller.OnStartDeserializing(obj, e);
		}
		protected override void OnEndDeserializing(DependencyObject obj, string restoredVersion) {
			base.OnEndDeserializing(obj, restoredVersion);
			PivotSerializationController controller = GetController(obj);
			if(controller != null)
				controller.OnEndDeserializing(obj, restoredVersion);
		}
		protected override bool OnAllowProperty(AllowPropertyEventArgs e) {
			PivotSerializationController controller = GetController(e.Source);
			if(controller != null)
				if(!controller.OnAllowProperty(e))
					return false;
			return base.OnAllowProperty(e);
		}
		protected override bool OnShouldSerializeProperty(object obj, PropertyDescriptor prop, XtraSerializableProperty xtraSerializableProperty) {
			DependencyPropertyDescriptor dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(prop);
			if(dependencyPropertyDescriptor != null &&
					object.Equals(dependencyPropertyDescriptor.GetValue(obj),
						dependencyPropertyDescriptor.DependencyProperty.GetMetadata(prop.ComponentType).DefaultValue)) {
				return false;
			}
			return base.OnShouldSerializeProperty(obj, prop, xtraSerializableProperty);
		}
		protected override void OnResetProperty(ResetPropertyEventArgs e) {
			if(PivotSerializationController.IsPropertyObsolete(e.Property.Name) || e.Source as FrameworkElement != null && e.Property.Name == "Name")
				return;
			base.OnResetProperty(e);
		}
		PivotSerializationController GetController(object obj) {
			return PivotGrid != null ? PivotGrid.SerializationController : null;
		}
		WeakReference pivotGrid;
		public PivotGridControl PivotGrid {
			get { return pivotGrid.Target as PivotGridControl; }
			set { pivotGrid = new WeakReference(value); }
		}
	}
	public static class SerializationPropertiesNames {
		public const string Fields = "Fields";
		public const string Groups = "Groups";
		public const string GroupFields = "GroupFields";
		public const string SortByConditions = "SortByConditions";
		public const string MRUFilters = "MRUFilters";
		public const string FormatConditions = "FormatConditions";
	}
}
