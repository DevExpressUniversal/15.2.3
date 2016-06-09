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
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;
using System.Configuration;
using System.ComponentModel;
using System.Activities.DurableInstancing;
using DevExpress.Workflow.Utils;
using System.Runtime.DurableInstancing;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
using DevExpress.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Workflow.Activities.Utils;
namespace DevExpress.Workflow.Store {
	public class WorkflowInstanceStoreElement : BehaviorExtensionElement {
		public const string connectionString = "connectionString";
		public const string connectionStringName = "connectionStringName";
		public const string defaultConnectionStringName = "WorkflowStorageConnectionString";
		public const string instanceCompletionAction = "instanceCompletionAction";
		public const string runnableInstancesDetectionPeriod = "runnableInstancesDetectionPeriod";
		public const string instanceStoreType = "instanceStoreType";
		public const string instanceKeyStoreType = "instanceKeyStoreType";
		public const string objectSpaceProviderName = "objectSpaceProviderName";
		[ConfigurationProperty(connectionString, IsRequired=false), StringValidator(MinLength=0)]
		public string ConnectionString {
			get { return (string)base[connectionString]; }
			set { base[connectionString] = value; }
		}
		[ConfigurationProperty(connectionStringName, IsRequired=false), StringValidator(MinLength=0)]
		public string ConnectionStringName {
			get { return (string)base[connectionStringName]; }
			set { base[connectionStringName] = value; }
		}
		[ConfigurationProperty(instanceCompletionAction, IsRequired=false, DefaultValue=InstanceCompletionAction.DeleteAll)]
		public InstanceCompletionAction InstanceCompletionAction {
			get { return (InstanceCompletionAction)base[instanceCompletionAction]; }
			set { base[instanceCompletionAction] = value; }
		}
		[ConfigurationProperty(runnableInstancesDetectionPeriod, IsRequired=false, DefaultValue="00:00:30.0"), TypeConverter(typeof(TimeSpanConverter)), PositiveTimeSpanValidator]
		public TimeSpan RunnableInstancesDetectionPeriod {
			get { return (TimeSpan)base[runnableInstancesDetectionPeriod]; }
			set { base[runnableInstancesDetectionPeriod] = value; }
		}
		[ConfigurationProperty(instanceStoreType, IsRequired=true), TypeConverter(typeof(TypeToStringConverter))]
		public Type InstanceStoreType {
			get { return (Type)base[instanceStoreType]; }
			set { base[instanceStoreType] = value; }
		}
		[ConfigurationProperty(instanceKeyStoreType, IsRequired=true), TypeConverter(typeof(TypeToStringConverter))]
		public Type InstanceKeyStoreType {
			get { return (Type)base[instanceKeyStoreType]; }
			set { base[instanceKeyStoreType] = value; }
		}
		[ConfigurationProperty(objectSpaceProviderName, IsRequired = true)]
		public string ObjectSpaceProviderName {
			get { return (string)base[objectSpaceProviderName]; }
			set { base[objectSpaceProviderName] = value; }
		}
		public override Type BehaviorType {
			get { return typeof(WorkflowInstanceStoreBehavior); }
		}
		protected override object CreateBehavior() {
			string connectionString = null;
			IObjectSpaceProvider objectSpaceProvider = null;
			if(!string.IsNullOrEmpty(ConnectionString)) {
				connectionString = ConnectionString;
			}
			else {
				string connectionStringNameString = string.IsNullOrEmpty(this.ConnectionStringName) ? defaultConnectionStringName : this.ConnectionStringName;
				ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[connectionStringNameString];
				if(settings != null) {
					connectionString = settings.ConnectionString;
				}
			}
			try {
				objectSpaceProvider = (IObjectSpaceProvider)ReflectionHelper.CreateObject(ObjectSpaceProviderName, connectionString);
			}
			catch {
				throw new InvalidOperationException("Unable to create an ObjectSpaceProvider instance. Check the ObjectSpaceProvider name and parameters.");
			}
			WorkflowInstanceStoreBehavior behavior = new WorkflowInstanceStoreBehavior(InstanceStoreType, InstanceKeyStoreType, objectSpaceProvider);
			behavior.WorkflowInstanceStore.InstanceCompletionAction = this.InstanceCompletionAction;
			behavior.WorkflowInstanceStore.RunnableInstancesDetectionPeriod = this.RunnableInstancesDetectionPeriod;
			return behavior;
		}
	}
}
