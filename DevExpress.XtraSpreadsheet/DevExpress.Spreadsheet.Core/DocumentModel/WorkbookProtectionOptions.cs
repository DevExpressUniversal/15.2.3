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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class WorkbookProtectionOptions : ICloneable<WorkbookProtectionOptions> , ISupportsCopyFrom<WorkbookProtectionOptions> {
		#region Fields
		bool lockRevisions;
		bool lockStructure;
		bool lockWindows;
		ProtectionCredentials credentials;
		#endregion
		public WorkbookProtectionOptions() {
			this.credentials = new ProtectionCredentials(); 
		}
		#region Properties
		public bool IsLocked { get { return LockRevisions || LockStructure || LockWindows; } }
		public bool LockRevisions { get { return lockRevisions; } set { lockRevisions = value; } }
		public bool LockStructure { get { return lockStructure; } set { lockStructure = value; } }
		public bool LockWindows { get { return lockWindows; } set { lockWindows = value; } }
		public ProtectionCredentials Credentials { get { return credentials; } set { credentials = value; } }
		#endregion
		public WorkbookProtectionOptions Clone() {
			WorkbookProtectionOptions result = new WorkbookProtectionOptions();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(WorkbookProtectionOptions value) {
			this.lockRevisions = value.lockRevisions;
			this.lockStructure = value.lockStructure;
			this.lockWindows = value.lockWindows;
			this.credentials = value.credentials.Clone();
		}
		public bool CheckPassword(string password) {
			return credentials.CheckPassword(password);
		}
	}
}
