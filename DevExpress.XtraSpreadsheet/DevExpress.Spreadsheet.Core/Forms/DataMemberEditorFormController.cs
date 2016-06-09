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
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Localization;
using System.ComponentModel;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region DataMemberEditorViewModel
	public class DataMemberEditorViewModel :ViewModelBase {
		#region Fields
		List<string> dataMembers;
		List<string> dataMembersNames;
		ISpreadsheetControl control;
		#endregion
		public DataMemberEditorViewModel(ISpreadsheetControl control, List<string> dataMembers) {
			this.control = control;
			this.dataMembers = dataMembers;
			InitializeDataMembersNames();
		}
		#region Properties
		public List<string> DataMembers {
			get { return dataMembers; }
			set {
				if (dataMembers == value)
					return;
				dataMembers = value;
				OnPropertyChanged("DataMembers");
			}
		}
		public List<string> DataMembersNames {
			get { return dataMembersNames; }
			set {
				if (dataMembersNames == value)
					return;
				dataMembersNames = value;
				OnPropertyChanged("DataMembersNames");
			}
		}
		#endregion
		void InitializeDataMembersNames() {
			dataMembersNames = new List<string>();
			for (int i = 0; i < dataMembers.Count; i++)
				dataMembersNames.Add(MailMergeDefinedNames.DetailLevel + i.ToString());
		}
		public void ApplyChanges() {
			SetDetailDataMemberCommand command = new SetDetailDataMemberCommand(control);
			command.ApplyChanges(dataMembers);
		}
	}
	#endregion
	#region SelectDataMemberViewModel
	public class SelectDataMemberViewModel : ViewModelBase {
		#region Fields
		string dataMember;
		ISpreadsheetControl control;
		#endregion
		public SelectDataMemberViewModel(ISpreadsheetControl control, string dataMember) {
			this.control = control;
			this.dataMember = dataMember;
		}
		#region Properties
		public string DataMember {
			get { return dataMember; }
			set {
				if (dataMember == value)
					return;
				dataMember = value;
				OnPropertyChanged("DataMember");
			}
		}
		#endregion
		public void ApplyChanges() {
			MailMergeSelectDataMemberCommand command = new MailMergeSelectDataMemberCommand(control);
			command.ApplyChanges(dataMember);
		}
	}
	#endregion
}
