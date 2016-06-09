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
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.Native.PerfomanceMonitor {
	public interface IPfWriter {
		void WriteEvent(IPfEvent evnt);
		void SaveData();
	}
	public class PfXMLWriter : IPfWriter {
		object syncRoot = new object();
		DataTable table;
		string fileName;
		public PfXMLWriter(string fileName) {
			DXContract.Requires(!String.IsNullOrEmpty(fileName));
			this.fileName = fileName;
			table = new DataTable("PfData");
			table.Locale = CultureInfo.InvariantCulture;
			table.Columns.Add("ThreadId", typeof(int));
			table.Columns.Add("Id", typeof(Guid));
			table.Columns.Add("ParentId", typeof(Guid));
			table.Columns.Add("StepNumber", typeof(int));
			table.Columns.Add("Level", typeof(int));
			table.Columns.Add("Name", typeof(string));
			table.Columns.Add("FullName", typeof(string));
			table.Columns.Add("StartTime", typeof(DateTime));
			table.Columns.Add("Duration", typeof(long));
			table.Columns.Add("GC0Count", typeof(int));
			table.Columns.Add("GC1Count", typeof(int));
			table.Columns.Add("GC2Count", typeof(int));
			table.Columns.Add("GCMemory", typeof(long));
			table.Columns.Add("GC0CountTotal", typeof(int));
			table.Columns.Add("GC1CountTotal", typeof(int));
			table.Columns.Add("GC2CountTotal", typeof(int));
			table.Columns.Add("GCMemoryTotal", typeof(long));
		}
		public void WriteEvent(IPfEvent evnt) {
			lock(syncRoot) {
				DataRow row = table.Rows.Add();
				row["ThreadId"] = evnt.ThreadId;
				row["Id"] = evnt.Id;
				row["ParentId"] = evnt.ParentId.HasValue ? evnt.ParentId : Guid.Empty;
				row["StepNumber"] = evnt.StepNumber;
				row["Level"] = evnt.Level;
				row["Name"] = evnt.Name;
				row["FullName"] = evnt.FullName;
				row["StartTime"] = evnt.StartTime;
				row["Duration"] = evnt.Duration;
				row["GC0Count"] = evnt.GC0Count;
				row["GC1Count"] = evnt.GC1Count;
				row["GC2Count"] = evnt.GC2Count;
				row["GCMemory"] = evnt.GCMemory;
				row["GC0CountTotal"] = evnt.GC0CountTotal;
				row["GC1CountTotal"] = evnt.GC1CountTotal;
				row["GC2CountTotal"] = evnt.GC2CountTotal;
				row["GCMemoryTotal"] = evnt.GCMemoryTotal;
			}
		}
		public void SaveData() {
			lock(syncRoot) {
				table.WriteXml(fileName, XmlWriteMode.WriteSchema);
			}
		}
	}
}
