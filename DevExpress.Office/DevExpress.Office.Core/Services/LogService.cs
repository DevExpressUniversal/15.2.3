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
namespace DevExpress.Office.Services {
	public enum LogCategory {
		Info,
		Warning,
		Error
	}
	public class LogEntry {
		public LogEntry(LogCategory category, string message) {
			Category = category;
			Message = message;
		}
		public LogCategory Category { get; private set; }
		public string Message { get; private set; }
		public override string ToString() {
			return string.Format("{0}: {1}", Category.ToString(), Message);
		}
	}
	public interface ILogService {
		bool IsEmpty { get; }
		IEnumerable<LogEntry> Entries { get; }
		void LogMessage(LogCategory category, string message);
		void Clear();
	}
}
namespace DevExpress.Office.Services.Implementation {
	public class LogService : ILogService {
		readonly List<LogEntry> innerList = new List<LogEntry>();
		#region IImporterLog Members
		public bool IsEmpty {
			get { return innerList.Count == 0; }
		}
		public IEnumerable<LogEntry> Entries {
			get { return innerList; }
		}
		public void LogMessage(LogCategory category, string message) {
			this.innerList.Add(new LogEntry(category, message));
		}
		public void Clear() {
			this.innerList.Clear();
		}
		#endregion
	}
}
