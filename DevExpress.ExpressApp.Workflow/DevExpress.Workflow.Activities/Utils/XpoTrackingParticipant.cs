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
using System.Activities.Tracking;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
namespace DevExpress.Workflow.Utils {
	public class XafTrackingParticipant : TrackingParticipantBase {
		protected override void Track(TrackingRecord record, TimeSpan timeout) {
			base.Track(record, timeout);
			string activityId = string.Empty;
			try {
				string recordData = string.Empty;
				WorkflowInstanceRecord workflowInstanceRecord = record as WorkflowInstanceRecord;
				if(workflowInstanceRecord != null) {
					recordData += TrackingParticipantHelper.GetInfo(workflowInstanceRecord);
				}
				ActivityStateRecord activityStateRecord = record as ActivityStateRecord;
				if(activityStateRecord != null) {
					activityId = activityStateRecord.Activity.Id;
					recordData += TrackingParticipantHelper.GetInfo(activityStateRecord);
				}
				CustomTrackingRecord customTrackingRecord = record as CustomTrackingRecord;
				if((customTrackingRecord != null) && (customTrackingRecord.Data.Count > 0)) {
					recordData += TrackingParticipantHelper.GetInfo(customTrackingRecord);
				}
				FaultPropagationRecord faultPropagationRecord = record as FaultPropagationRecord;
				if(faultPropagationRecord != null) {
					recordData += TrackingParticipantHelper.GetInfo(faultPropagationRecord);
				}
				if(!string.IsNullOrEmpty(recordData)) {
					using(IObjectSpace objectSpace = ObjectSpaceProvider.CreateObjectSpace()) {
						ITrackingRecord trackingRecord = (ITrackingRecord)objectSpace.CreateObject(TrackingRecordType);
						trackingRecord.InstanceId = record.InstanceId;
						trackingRecord.DateTime = DateTime.Now;
						trackingRecord.Data = recordData;
						trackingRecord.ActivityId = activityId;
						objectSpace.CommitChanges();
					}
				}
			}
			catch(Exception e) {
				Exception ex = new Exception(e.Message, e);
				ex.Data.Add("record.GetType", record.GetType());
				ex.Data.Add("record.InstanceId", record.InstanceId);
				ex.Data.Add("record.RecordNumber", record.RecordNumber);
				ex.Data.Add("record.EventTime", record.EventTime);
				ex.Data.Add("activityId", activityId);
				DevExpress.Persistent.Base.Tracing.Tracer.LogError(ex);
			}
		}
		public XafTrackingParticipant(IObjectSpaceProvider objectSpaceProvider, Type trackingRecordType) {		   
			Guard.ArgumentNotNull(trackingRecordType, "trackingRecordType");
			this.ObjectSpaceProvider = objectSpaceProvider;
			this.TrackingRecordType = trackingRecordType;
		}
		public IObjectSpaceProvider ObjectSpaceProvider { get; set; }
		public Type TrackingRecordType { get; set; }
	}
	public class TrackingBehavior : IServiceBehavior {
		private IObjectSpaceProvider objectSpaceProvider;
		private Type trackingRecordType;
		public TrackingBehavior(IObjectSpaceProvider objectSpaceProvider, Type trackingRecordType) {
			Guard.ArgumentNotNull(trackingRecordType, "trackingRecordType");
			Guard.ArgumentNotNull(objectSpaceProvider, "objectSpaceProvider");
			this.objectSpaceProvider = objectSpaceProvider;
			this.trackingRecordType = trackingRecordType;
		}
		public virtual void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }
		public virtual void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) {
			WorkflowServiceHost host = serviceHostBase as WorkflowServiceHost;
			if(host != null) {
				string displayName = host.Activity.DisplayName;
				string hostReference = string.Empty;
				XafTrackingParticipant xafTrackingParticipant = new XafTrackingParticipant(objectSpaceProvider, trackingRecordType);
				host.WorkflowExtensions.Add(xafTrackingParticipant);
			}
		}
		public virtual void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }
		public string ProfileName { get; set; }
#if DebugTest
		public IObjectSpaceProvider DebugTest_ObjectSpaceProvider { get { return objectSpaceProvider;} }
#endif
	}
	[Obsolete("Use the 'TrackingBehaviorElement' instead.")]
	public class XpoTrackingBehaviorElement { }
	public class TrackingBehaviorElement : BehaviorExtensionElement {				
		public static string WorkflowStorageConnectionStringName = "WorkflowStorageConnectionString";
		private const string connectionString = "connectionString";
		private const string profileNameParameter = "profileName";
		private const string objectSpaceProviderName = "objectSpaceProviderName";
		private const string trackingRecordTypeName = "trackingRecordTypeName";
		private ConfigurationPropertyCollection properties;
		protected override object CreateBehavior() {			
			string workflowStorageConnectionString = ConnectionString;
			if(string.IsNullOrEmpty(workflowStorageConnectionString)) {
				workflowStorageConnectionString = ConfigurationManager.ConnectionStrings[WorkflowStorageConnectionStringName].ConnectionString;
			}
			IObjectSpaceProvider objectSpaceProvider = null;
			try {
				objectSpaceProvider = (IObjectSpaceProvider)ReflectionHelper.CreateObject(ObjectSpaceProviderName, workflowStorageConnectionString);
			}
			catch {
				throw new InvalidOperationException("Unable to create an ObjectSpaceProvider instance. Check the ObjectSpaceProvider name and parameters.");
			}			
			TrackingBehavior trackingBehavior = new TrackingBehavior(objectSpaceProvider, TrackingRecordType);
			trackingBehavior.ProfileName = this.ProfileName;
			return trackingBehavior;
		}
		public override Type BehaviorType {
			get { return typeof(TrackingBehavior); }
		}
		[StringValidator(MinLength=0), ConfigurationProperty(profileNameParameter, DefaultValue="", Options=ConfigurationPropertyOptions.IsKey)]
		public string ProfileName {
			get { return (string)base[profileNameParameter]; }
			set { base[profileNameParameter] = value; }
		}
		[ConfigurationProperty(connectionString, IsRequired = false), StringValidator(MinLength = 0)]
		public string ConnectionString {
			get { return (string)base[connectionString]; }
			set { base[connectionString] = value; }
		}
		[ConfigurationProperty(objectSpaceProviderName, IsRequired = true)]
		public string ObjectSpaceProviderName {
			get { return (string)base[objectSpaceProviderName]; }
			set { base[objectSpaceProviderName] = value; }
		}
		[ConfigurationProperty(trackingRecordTypeName, IsRequired = true)]
		public Type TrackingRecordType {
			get { return (Type)base[trackingRecordTypeName]; }
			set { base[trackingRecordTypeName] = value; }
		}
		protected override ConfigurationPropertyCollection Properties {
			get {
				if(this.properties == null) {
					ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
					properties.Add(new ConfigurationProperty(profileNameParameter, typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey));
					properties.Add(new ConfigurationProperty(connectionString, typeof(string), string.Empty, null, new StringValidator(0), ConfigurationPropertyOptions.IsRequired));
					properties.Add(new ConfigurationProperty(objectSpaceProviderName, typeof(string), string.Empty, null, null, ConfigurationPropertyOptions.IsRequired));
					properties.Add(new ConfigurationProperty(trackingRecordTypeName, typeof(string), string.Empty, null, null, ConfigurationPropertyOptions.IsRequired)); 
					this.properties = properties;
				}
				return this.properties;
			}
		}
	}
#region Obsolete 15.1
	[Obsolete("Use the 'XafTrackingParticipant' instead.", true)]
	public class XpoTrackingParticipant : TrackingParticipantBase {
		[Obsolete("Use the 'XafTrackingParticipant' class instead.", true)]
		public XpoTrackingParticipant(IObjectSpaceProvider objectSpaceProvider) { }
	}
	[Obsolete("Use the 'TrackingBehavior' instead.", true)]
	public class XpoTrackingBehavior : TrackingBehavior {
		public XpoTrackingBehavior() : base(null, null) { }
	}
#endregion
}
