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

using DevExpress.XtraScheduler.Outlook.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.XtraScheduler.OutlookExchange.Native {
	interface IPropertyModificationTracker {
		void Save();
		void FixChanged();
		void RestoreToFixed();
		bool IsModified { get; }
	}
	public class PropertyModificationTracker<T> : IPropertyModificationTracker {
		Dictionary<Type, Type> subTrackers = new Dictionary<Type, Type>();
		T trackedObject;
		Dictionary<string, IPropertyModificationTracker> propertySubTrackerCache = new Dictionary<string, IPropertyModificationTracker>();
		public PropertyModificationTracker(T trackedObject) {
			this.trackedObject = trackedObject;
			SavedPropertiesValues = new Dictionary<string, object>();
			ChangedPropertiesValues = new Dictionary<string, object>();
			RegisterTrackers();
			Save();
		}
		protected Dictionary<string, object> SavedPropertiesValues { get; private set; }
		protected Dictionary<string, object> ChangedPropertiesValues { get; private set; }
		protected virtual void RegisterTrackers() {
		}
		public void Save() {
			PropertyDescriptorCollection properties = GetProperies();
			foreach (PropertyDescriptor property in properties) {
				IPropertyModificationTracker tracker = GetTracker(property);
				string propertyName = property.Name;
				if (tracker != null) {
					SavedPropertiesValues[propertyName] = tracker;
					tracker.Save();
				} else
					SavedPropertiesValues[propertyName] = property.GetValue(this.trackedObject);
			}
			ChangedPropertiesValues.Clear();
		}		
		public void FixChanged() {
			ChangedPropertiesValues.Clear();
			PropertyDescriptorCollection properties = GetProperies();
			foreach (PropertyDescriptor property in properties) {
				string propertyName = property.Name;
				object oldPropertyValue = SavedPropertiesValues[propertyName];
				IPropertyModificationTracker propertyTracker = oldPropertyValue as IPropertyModificationTracker;
				object propertyValue = property.GetValue(this.trackedObject);
				if (propertyTracker != null) {
					propertyTracker.FixChanged();
					if (!propertyTracker.IsModified)
						continue;
					propertyValue = propertyTracker;
				} else {
					if ((propertyValue == null && oldPropertyValue == null) || (propertyValue != null && propertyValue.Equals(oldPropertyValue)))
						continue;
				}
				ChangedPropertiesValues.Add(propertyName, propertyValue);
			}
		}
		public void RestoreToFixed() {
			PropertyDescriptorCollection properties = GetProperies();
			foreach (var propertyValuePair in ChangedPropertiesValues) {
				PropertyDescriptor property = properties[propertyValuePair.Key];
				object propertyValue = propertyValuePair.Value;
				IPropertyModificationTracker subTracker = propertyValue as IPropertyModificationTracker;
				if (subTracker == null)
					property.SetValue(this.trackedObject, propertyValuePair.Value);
				else {
					subTracker.RestoreToFixed();
				}
			}
		}
		IPropertyModificationTracker GetTracker(PropertyDescriptor property) {
			if (this.subTrackers.Count <= 0)
				return null;
			Type propertyType = property.PropertyType;
			if (!this.subTrackers.ContainsKey(propertyType))
				return null;
			string propertyName = property.Name;
			if (!this.propertySubTrackerCache.ContainsKey(propertyName))
				this.propertySubTrackerCache[propertyName] = Activator.CreateInstance(this.subTrackers[propertyType], property.GetValue(this.trackedObject)) as IPropertyModificationTracker;
			return this.propertySubTrackerCache[propertyName];
		}
		protected void RegisterSubTracker(Type propertyType, Type tracker) {
			this.subTrackers.Add(propertyType, tracker);
		}
		PropertyDescriptorCollection GetProperies() {
			return TypeDescriptor.GetProperties(typeof(T));
		}
		#region IPropertyModificationTracker
		bool IPropertyModificationTracker.IsModified { get { return ChangedPropertiesValues.Count > 0; } }
		void IPropertyModificationTracker.Save() {
			Save();
		}
		void IPropertyModificationTracker.FixChanged() {
			FixChanged();
		}
		void IPropertyModificationTracker.RestoreToFixed() {
			RestoreToFixed();
		}
		#endregion
	}
	internal class OutlookAppointmentModificationTracker : PropertyModificationTracker<_AppointmentItem> {
		public OutlookAppointmentModificationTracker(_AppointmentItem apt)
			: base(apt) {
		}
		protected override void RegisterTrackers() {
			base.RegisterTrackers();
			RegisterSubTracker(typeof(_TimeZone), typeof(OutlookTimeZoneModificationTracker));
		}
	}
	internal class OutlookTimeZoneModificationTracker : PropertyModificationTracker<_TimeZone> {
		public OutlookTimeZoneModificationTracker(_TimeZone timeZone)
			: base(timeZone) {
		}
	}
}
