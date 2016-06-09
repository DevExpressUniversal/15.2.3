#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Service;
using DevExpress.Data.IO;
namespace DevExpress.DashboardCommon.Server {
	public class DashboardSessionState : IBinarySerializable {
		public DashboardState DashboardState { get; set; }
		public SessionSettings SessionSettings { get; set; }
		public string DataVersion { get; set; }
		public IEnumerable<DashboardParameterInfo> DashboardParameters { get; set; }
		public string DashboardId { get; set; }
		void IBinarySerializable.Serialize(TypedBinaryWriter writer) {
			DashboardState.Serialize(writer);
			int parametersCount = DashboardParameters != null ? DashboardParameters.Count() : 0;
			writer.WriteObject(parametersCount);
			if(parametersCount > 0) {
				foreach(DashboardParameterInfo parameter in DashboardParameters) {
					writer.WriteObject(parameter.Name);
					writer.WriteTypedObject(parameter.Value);
				}
			}
			writer.WriteObject(DataVersion);
			writer.WriteObject(DashboardId);
			writer.WriteObject(SessionSettings.CalculateHiddenTotals);
			writer.WriteObject(SessionSettings.SessionTimeout.TotalSeconds);
		}
		void IBinarySerializable.Deserialize(TypedBinaryReader reader) {
			DashboardState = new DashboardState();
			DashboardState.Deserialize(reader);
			int parametersCount = reader.ReadObject<int>();
			List<DashboardParameterInfo> parameters = new List<DashboardParameterInfo>();
			for(int i = 0; i < parametersCount; i++) {
				DashboardParameterInfo parameter = new DashboardParameterInfo();
				parameter.Name = reader.ReadObject<string>();
				parameter.Value = reader.ReadTypedObject();
				parameters.Add(parameter);
			}
			if(parameters.Count > 0)
				DashboardParameters = parameters;
			DataVersion = reader.ReadObject<string>();
			DashboardId = reader.ReadObject<string>();
			SessionSettings = new SessionSettings();
			SessionSettings.CalculateHiddenTotals = reader.ReadObject<bool>();
			SessionSettings.SessionTimeout = TimeSpan.FromSeconds(reader.ReadObject<double>());
		}
	}
}
