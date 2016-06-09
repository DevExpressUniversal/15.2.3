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
using System.Text;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using System.ComponentModel;
namespace DevExpress.XtraScheduler.Reporting {
	public interface ISmartSyncOptions : INotifyPropertyChanged {
		SchedulerGroupType GroupType { get; set; }
	}
	#region SmartSyncOptions
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public class SmartSyncOptions : SchedulerNotificationOptions, ISmartSyncOptions {
		SchedulerGroupType groupType;
		public SmartSyncOptions() {
		}
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SmartSyncOptionsGroupType"),
#endif
DefaultValue(SchedulerGroupType.Resource), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public SchedulerGroupType GroupType {
			get { return groupType; }
			set {
				if (groupType == value)
					return;
				SchedulerGroupType oldValue = groupType;
				groupType = value;
				OnChanged(new BaseOptionChangedEventArgs("GroupType", oldValue, value));
			}
		}
		protected internal override void ResetCore() {
			this.groupType = SchedulerGroupType.Resource;
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				SmartSyncOptions syncOptions = options as SmartSyncOptions;
				if (syncOptions == null)
					return;
				this.groupType = syncOptions.GroupType;
			} finally {
				EndUpdate();
			}
		}
	}
	#endregion
}
