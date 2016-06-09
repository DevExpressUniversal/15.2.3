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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using System.Collections.Specialized;
using System.Windows.Data;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualCustomFieldCollection : INotifyPropertyChanged, ISupportCopyFrom<CustomFieldCollection> {
		Dictionary<string, object> items = new Dictionary<string, object>();
		public object this[string propertyName] {
			get {
				if(items.ContainsKey(propertyName))
					return items[propertyName];
				return null;
			}
			set {
				if(!items.ContainsKey(propertyName))
					items.Add(propertyName, value);
				items[propertyName] = value;
				RaisePropertyChanged(propertyName);
			}
		}
		#region INotifyPropertyChanged
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged {
			add {
				onPropertyChanged += value;
			}
			remove {
				onPropertyChanged -= value;
			}
		}
		protected virtual void RaisePropertyChanged(string propertyName) {
			if(onPropertyChanged == null)
				return;
#if SL
			string propertyFullName = String.Format("Item[{0}]", propertyName);
			onPropertyChanged(this, new PropertyChangedEventArgs(propertyFullName));		   
#else
			onPropertyChanged(this, new PropertyChangedEventArgs(Binding.IndexerName)); 
#endif            
		}
		#endregion
		void ISupportCopyFrom<CustomFieldCollection>.CopyFrom(CustomFieldCollection source) {
			CopyFrom(source);
		}
		public bool CopyFrom(CustomFieldCollection source) {
			bool wasChanged = false;
			if (source == null) {
				if (items.Count > 0)
					wasChanged = true;
				items.Clear();
			}
			NotificationCollection<CustomField> fields = source.Fields;
			int count = fields.Count;
			for(int i = 0; i < count; i++) {
				CustomField field = fields[i];
				string fieldName = field.Name;
				if (!Object.Equals(this[fieldName], field.Value)) {
					this[fieldName] = field.Value;
					wasChanged = true;
				}
			}
			return wasChanged;
		}
	}
}
