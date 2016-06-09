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
using System.Activities;
using System.ComponentModel;
using System.Configuration;
using System.Data.Common;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Workflow.Utils;
namespace DevExpress.Workflow.Activities {
	[DevExpress.Utils.ToolboxTabName(ActivitiesAssemblyInfo.DXActivitiesTabName)]
	[ToolboxBitmap(typeof(ObjectSpaceTransactionScope), "Images.CreateObjectSpace.bmp")]
	public class CreateObjectSpace : NativeActivity<IObjectSpace> {
		public const string ConnectionString_ObjectSpaceProviderKey = "ObjectSpaceProvider";
		public const string ConnectionStringName = "ConnectionString";
		protected virtual string GetConnectionString(NativeActivityContext context) {
			return this.ConnectionString.Get(context);
		}
		protected override void Execute(NativeActivityContext context) {
			string connectionString = GetConnectionString(context);
			IObjectSpaceProvider objectSpaceProvider = null;
			if(string.IsNullOrEmpty(connectionString)) {
				objectSpaceProvider = context.GetExtension<IObjectSpaceProvider>();
			}
			if(objectSpaceProvider == null) {
				if(string.IsNullOrEmpty(connectionString)) {
					ConnectionStringSettings appConfigConnectionStringSettings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
					Guard.ArgumentNotNull(appConfigConnectionStringSettings, string.Format("App config '{0}' connection string is not found.", ConnectionStringName));
					connectionString = appConfigConnectionStringSettings.ConnectionString;
				}
				Guard.ArgumentNotNullOrEmpty(connectionString, "connectionString");
				DbConnectionStringBuilder connectionStringBuilder = new DbConnectionStringBuilder();
				connectionStringBuilder.ConnectionString = connectionString;
				string objectSpaceProviderName = "";
				if(connectionStringBuilder.ContainsKey(ConnectionString_ObjectSpaceProviderKey)) {
					objectSpaceProviderName = (string)connectionStringBuilder[ConnectionString_ObjectSpaceProviderKey];
					connectionStringBuilder.Remove(ConnectionString_ObjectSpaceProviderKey);
					try {
						objectSpaceProvider = (IObjectSpaceProvider)ReflectionHelper.CreateObject(objectSpaceProviderName, connectionStringBuilder.ToString());
					}
					catch {
						throw new InvalidOperationException("Unable to create an ObjectSpaceProvider instance. Check the ObjectSpaceProvider name and parameters.");
					}
				}
			}
			IObjectSpace objectSpace = objectSpaceProvider.CreateObjectSpace();
			Guard.ArgumentNotNull(objectSpace, "objectSpaceProvider.CreateObjectSpace()");
			Result.Set(context, objectSpace);
		}
		public CreateObjectSpace() : base() {
#if !DebugTest
			this.Constraints.Add(ConstraintHelper.VerifyParent<NoPersistScope>(this));
#endif
		}
		[DefaultValue(null)]
		public InArgument<string> ConnectionString { get; set; }
	}
}
