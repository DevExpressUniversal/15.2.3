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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Native;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Xpo.DB;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	[ToolboxItem(false)]
	public partial class ChooseEFDataMemberPageView : WizardViewBase, IChooseEFDataMemberPageView {
		class EFDataMemberRecord {
			readonly DBTable table;
			readonly DBStoredProcedure procedure;
			public string Name {
				get {
					if(DataMember != null)
						return DataMember;
					return PreviewLocalizer.GetString(PreviewStringId.NoneString);
				}
			}
			public int ImageIndex {
				get {
					if(table != null)
						return table.IsView ? 3 : 2;
					if(procedure != null)
						return 1;
					return 0;
				}
			}
			public DBStoredProcedure StoredProcedure {
				get { return procedure; }
			}
			public string DataMember {
				get {
					if(table != null)
						return table.Name;
					if(procedure != null)
						return procedure.Name;
					return null;
				}
			}
			public EFDataMemberRecord(DBTable table) {
				this.table = table;
			}
			public EFDataMemberRecord(DBStoredProcedure procedure) {
				this.procedure = procedure;
			}
			public override string ToString() {
				return Name;
			}
		}
		string dataMember;
		bool storedProcChosen;
		public event EventHandler DataMemberChanged;
		public string DataMember {
			get { return dataMember; }
		}
		public bool StoredProcChosen {
			get { return storedProcChosen; }
		}
		#region IWizardPageView
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseEFDataMember); }
		}
		#endregion
		public ChooseEFDataMemberPageView() {
			InitializeComponent();
			layoutControlContent.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			imageList.Images.Add(ImageHelper.GetImage("None"));
			imageList.Images.Add(ImageHelper.GetImage("MethodBinding"));
			imageList.Images.Add(ImageHelper.GetImage("table"));
			imageList.Images.Add(ImageHelper.GetImage("view"));
		}
		public void Initialize(IEnumerable<DBTable> tables, IEnumerable<DBStoredProcedure> procedures, string dataMember) {
			this.dataMember = dataMember;
			List<EFDataMemberRecord> records = new List<EFDataMemberRecord>();
			if(tables != null)
				records.AddRange(tables.OrderBy(t => t.Name).Select(t => new EFDataMemberRecord(t)));
			if(procedures != null)
				records.AddRange(procedures.OrderBy(p => p.Name).Select(p => new EFDataMemberRecord(p)));
			imageListBoxDataMember.ImageList = imageList;
			imageListBoxDataMember.SelectedValueChanged -= imageListBoxDataMember_SelectedValueChanged;
			imageListBoxDataMember.Items.Clear();
			foreach(EFDataMemberRecord record in records)
				imageListBoxDataMember.Items.Add(record, record.ImageIndex);
			EFDataMemberRecord selectedRecord = records.FirstOrDefault(r => r.DataMember == DataMember);
			imageListBoxDataMember.SelectedValueChanged += imageListBoxDataMember_SelectedValueChanged;
			if(selectedRecord != null) {
				imageListBoxDataMember.SelectedValue = selectedRecord;
				this.storedProcChosen = selectedRecord.StoredProcedure != null;
			}
			else {
				EFDataMemberRecord dataMemberRecord = records.FirstOrDefault();
				if(dataMemberRecord != null) {
					this.dataMember = dataMemberRecord.DataMember;
					this.storedProcChosen = dataMemberRecord.StoredProcedure != null;
				}
			}
			if(DataMemberChanged != null)
				DataMemberChanged(this, EventArgs.Empty);
		}
		void imageListBoxDataMember_SelectedValueChanged(object sender, EventArgs e) {
			EFDataMemberRecord selectedRecord = imageListBoxDataMember.SelectedValue as EFDataMemberRecord;
			if(selectedRecord != null) {
				this.dataMember = selectedRecord.DataMember;
				this.storedProcChosen = selectedRecord.StoredProcedure != null;
			}
			else {
				this.dataMember = null;
				this.storedProcChosen = false;
			}
			if(DataMemberChanged != null)
				DataMemberChanged(this, EventArgs.Empty);
		}
		void ilbDataMember_MouseDoubleClick(object sender, MouseEventArgs e) {
			if(this.imageListBoxDataMember.IndexFromPoint(e.Location) != -1)
				this.MoveForward();
		}
	}
}
