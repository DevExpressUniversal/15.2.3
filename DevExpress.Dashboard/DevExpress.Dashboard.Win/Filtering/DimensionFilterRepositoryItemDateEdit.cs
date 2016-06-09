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

using System.ComponentModel;
using DevExpress.DashboardCommon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.DashboardWin.Native {
	[UserRepositoryItem("RegisterDimesionFilterDateEdit")]
	class DataItemFilterRepositoryItemDateEdit : RepositoryItemDateEdit {
		public const string DataItemFilterDateEditName = "DataItemFilterDateEdit";
		static DataItemFilterRepositoryItemDateEdit() { RegisterDimensionFilterDateEdit(); }
		public static void RegisterDimensionFilterDateEdit() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(DataItemFilterDateEditName, typeof(DataItemFilterDateEdit),
				typeof(DataItemFilterRepositoryItemDateEdit), typeof(DateEditViewInfo), new ButtonEditPainter(), false));
		}
		DateTimeGroupInterval? groupInterval;
		public DateTimeGroupInterval? GroupInterval {
			get { return groupInterval; }
			set {
				if(groupInterval != value) {
					groupInterval = value;
					VistaEditTime = DateTimeHelper.GetEditTime(groupInterval);
					VistaCalendarViewStyle = DateTimeHelper.GetCalendarStyle(groupInterval);
				}
			}
		}
		public override string EditorTypeName { get { return DataItemFilterDateEditName; } }
		public DataItemFilterRepositoryItemDateEdit() { }
		public override void Assign(RepositoryItem item) {
			BeginUpdate();
			try {
				base.Assign(item);
				DataItemFilterRepositoryItemDateEdit source = item as DataItemFilterRepositoryItemDateEdit;
				if(source == null) return;
				groupInterval = source.groupInterval;
			}
			finally {
				EndUpdate();
			}
		}
	}
	[DXToolboxItem(false)]
	class DataItemFilterDateEdit : DateEdit {
		static DataItemFilterDateEdit() { DataItemFilterRepositoryItemDateEdit.RegisterDimensionFilterDateEdit(); }
		public override string EditorTypeName { get { return DataItemFilterRepositoryItemDateEdit.DataItemFilterDateEditName; } }
		public DataItemFilterDateEdit() { }
		public new DataItemFilterRepositoryItemDateEdit Properties {
			get { return base.Properties as DataItemFilterRepositoryItemDateEdit; }
		}
	}
}
