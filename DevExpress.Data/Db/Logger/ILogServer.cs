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
namespace DevExpress.Xpo.Logger {
	public interface ILogger {
		int Count { get; }
		int LostMessageCount { get; }
		bool IsServerActive { get; }
		bool Enabled { get; set; }
		int Capacity { get; }		
		void Log(LogMessage message);
		void Log(LogMessage[] messages);
		void ClearLog();
	}
	public class DummyServer : ILogger {
		public int Count {
			get { return 0; }
		}
		public int LostMessageCount {
			get { return 0; }
		}
		public bool IsServerActive {
			get { return false; }
		}
		public bool Enabled {
			get { return false; }
			set { }
		}
		public int Capacity {
			get { return 0; }
		}
		public void Log(LogMessage message) { }
		public void Log(LogMessage[] messages) { }
		public void ClearLog() { }
	}
	namespace Transport {
#if DXPORTABLE
		public interface ILogSource {
			LogMessage GetMessage();
			LogMessage[] GetMessages(int messageAmount);
			LogMessage[] GetCompleteLog();
		}
#else
		using System.ServiceModel;
		[ServiceContract]
		public interface ILogSource {
			[OperationContract]
			LogMessage GetMessage();
			[OperationContract]
			LogMessage[] GetMessages(int messageAmount);
			[OperationContract]
			LogMessage[] GetCompleteLog();
		}
#endif
	}
}
