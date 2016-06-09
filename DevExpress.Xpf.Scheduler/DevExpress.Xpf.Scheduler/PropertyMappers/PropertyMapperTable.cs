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
using System.Windows;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Xpf.Scheduler.Native {
	#region DependencyPropertyMapperTable
	public class DependencyPropertyMapperTable {
		readonly Dictionary<DependencyProperty, IDependencyPropertyMapper> propertyTable;
		readonly Dictionary<DependencyProperty, PropertyValueInfo> deferredChanges;
		bool isDeferredChanges = false;
		public DependencyPropertyMapperTable() {
			this.propertyTable = new Dictionary<DependencyProperty, IDependencyPropertyMapper>();
			this.deferredChanges = new Dictionary<DependencyProperty, PropertyValueInfo>();
		}
		protected internal Dictionary<DependencyProperty, IDependencyPropertyMapper> PropertyTable { get { return propertyTable; } }
		protected internal Dictionary<DependencyProperty, PropertyValueInfo> DeferredChanges { get { return deferredChanges; } }
		public bool IsDeferredChanges { get { return isDeferredChanges; } }
		public void StartDeferredChanges() {
			isDeferredChanges = true;
		}
		public void CancelDeferredChanges() {
			isDeferredChanges = false;
			ClearDeferredChanges();
		}
		public void CommitDeferredChanges() {
			isDeferredChanges = false;
			foreach(KeyValuePair<DependencyProperty, PropertyValueInfo> item in DeferredChanges) {
				PropertyValueInfo itemValue = item.Value;
				UpdatePropertyMapper(item.Key, itemValue.OldValue, itemValue.NewValue);
			}
			ClearDeferredChanges();
		}
		protected void ClearDeferredChanges() {
			DeferredChanges.Clear();
		}
		public void RegisterPropertyMapper(DependencyProperty property, IDependencyPropertyMapper mapper) {
			if (PropertyTable.ContainsKey(property))
				return;
			PropertyTable[property] = mapper;
		}
		protected void UnregisterPropertyMapper(DependencyProperty property) {
			if (PropertyTable.ContainsKey(property))
				PropertyTable[property].Dispose();
		}
		public void UpdatePropertyMapper(DependencyProperty property, object oldValue, object newValue) {
			XtraSchedulerDebug.Assert(property != null);
			if (IsDeferredChanges) {
				RegisterPropertyChanges(property, oldValue, newValue);
				return;
			}
			IDependencyPropertyMapper mapper = (IDependencyPropertyMapper)PropertyTable[property];
			if (mapper != null)
				mapper.UpdateInnerPropertyValue(oldValue, newValue);
		}
		protected internal void RegisterPropertyChanges(DependencyProperty property, object oldValue, object newValue) {
			if (DeferredChanges.ContainsKey(property)) 
				return;
			PropertyValueInfo info = new PropertyValueInfo(oldValue, newValue);
			DeferredChanges[property] = info;
		}
		public void UnregisterPropertyMappers() {
			foreach (KeyValuePair<DependencyProperty, IDependencyPropertyMapper> propertyInfo in PropertyTable)
				UnregisterPropertyMapper(propertyInfo.Key);
			PropertyTable.Clear();
			DeferredChanges.Clear();
		}
		internal void ActivatePropertyMappers(PropertyMapperSyncDirection syncDirection) {
			foreach (KeyValuePair<DependencyProperty, IDependencyPropertyMapper> propertyInfo in PropertyTable)
				ActivatePropertyMapper(propertyInfo.Key, syncDirection);
		}
		internal void DeactivatePropertyMappers() {
			foreach (KeyValuePair<DependencyProperty, IDependencyPropertyMapper> propertyInfo in PropertyTable)
				DeactivatePropertyMapper(propertyInfo.Key);
		}
		internal void ActivatePropertyMapper(DependencyProperty property, PropertyMapperSyncDirection syncDirection) {
			if (!PropertyTable.ContainsKey(property))
				return;
			PropertyTable[property].Activate(syncDirection);
		}
		internal void DeactivatePropertyMapper(DependencyProperty property) {
			if (!PropertyTable.ContainsKey(property))
				return;
			PropertyTable[property].Deactivate();
		}
	}
	#endregion
	public class PropertyValueInfo {
		public PropertyValueInfo(object oldValue, object newValue) {
			OldValue = oldValue;
			NewValue = newValue;
		}
		public object OldValue { get; set; }
		public object NewValue { get; set; }
	}
}
