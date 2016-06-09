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
using System.ComponentModel.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Design.Commands;
namespace DevExpress.XtraReports.UserDesigner.Native {
	public class CommandContainer {
		int commandIDID;
		CommandID commandID;
		Guid commandIDGuid = Guid.Empty;
		ReportCommand command = ReportCommand.None;
		public int CommandIDID {
			get { return this.commandIDID; }
			set {
				this.commandIDID = value;
				if(commandIDGuid != Guid.Empty && commandID == null) {
					UpdateCommandID();
				}
			}
		}
		public Guid CommandIDGuid {
			get { return this.commandIDGuid; }
			set {
				this.commandIDGuid = value;
				if(commandIDID != 0 && commandID == null)
					UpdateCommandID();
			}
		}
		public virtual ReportCommand Command {
			get { return command; }
			set {
				this.command = value;
				commandID = CommandIDReportCommandConverter.GetCommandID(command);
				if(commandID != null) {
					commandIDID = commandID.ID;
					commandIDGuid = commandID.Guid;
				} else {
					commandIDID = 0;
					commandIDGuid = Guid.Empty;
				}
			}
		}
		public CommandID CommandID { get { return commandID; } }
		void UpdateCommandID() {
			commandID = new System.ComponentModel.Design.CommandID(commandIDGuid, commandIDID);
			command = CommandIDReportCommandConverter.GetReportCommand(commandID);
		}
	}
}
