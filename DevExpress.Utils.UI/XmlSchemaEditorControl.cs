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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Design;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.Utils.UI {
	[ToolboxItem(false)]
	public partial class XmlSchemaEditorControl : XtraUserControl {
		public event EventHandler Updated;
		string XmlUrl {
			get { return !checkBox1.Checked ? this.tbXmlUrl.Text : String.Empty; }
		}
		string DataClassName {
			get { return checkBox1.Checked ? cbClassName.Text : String.Empty; }
		}
		public object EditValue {
			get { return GetEditValue(); }
		}
		public bool EnableSubmit { 
			get {
				return checkBox1.Checked
				? cbClassName.SelectedItem != null
				: tbXmlUrl.Text.Length > 0;
			} 
		}
		public XmlSchemaEditorControl() {
			InitializeComponent();
		}
		public void Initialize(IDTEService DTEService) {
			FillComboBox(DTEService);
			checkBox1.Enabled = cbClassName.Properties.Items.Count > 0;
		}
		void RaiseUpdated() {
			if(Updated != null)
				Updated(this, new EventArgs());
		}
		void FillComboBox(IDTEService DTEService) {
			if(DTEService != null) {
				string[] dataSetNames = DTEService.GetClassesInfo(typeof(DataSet), null);
				cbClassName.Properties.Items.AddRange(dataSetNames);
			}
			bool hasItems = cbClassName.Properties.Items.Count > 0;
			if(hasItems)
				cbClassName.SelectedIndex = 0;
			cbClassName.Enabled = checkBox1.Checked;
		}
		void cbClassName_SelectedValueChanged(object sender, EventArgs e) {
			RaiseUpdated();
		}
		void tbXmlUrl_TextChanged(object sender, EventArgs e) {
			RaiseUpdated();
		}
		private void checkBox1_CheckedChanged(object sender, System.EventArgs e) {
			cbClassName.Enabled = ((DevExpress.XtraEditors.CheckEdit)sender).Checked;
			tbXmlUrl.Enabled = !cbClassName.Enabled;
			RaiseUpdated();
		}
		private void tbXmlUrl_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "XML files (*.xml)|*.xml|XML Schema files (*.xsd)|*.xsd|All files (*.*)|*.*";
			if(DialogRunner.ShowDialog(dlg) == DialogResult.OK)
				tbXmlUrl.Text = dlg.FileName;
		}
		private object GetEditValue() {
			DataSet ds = CreateDataSet();
			return XmlDataHelper.GetXmlSchema(ds);
		}
		private DataSet CreateDataSet() {
			return string.IsNullOrEmpty(XmlUrl) ?
				string.IsNullOrEmpty(DataClassName) ? null : CreateDataSetByClassName() :
				XmlDataHelper.CreateDataSetByXmlUrl(XmlUrl, true);
		}
		private DataSet CreateDataSetByClassName() {
			try {
				Type t = Type.GetType(DataClassName);
				return t != null ? Activator.CreateInstance(t) as DataSet : null;
			} catch {
				return null;
			}
		}
		private void XmlSchemaEditorControl_Load(object sender, EventArgs e) {
			layoutControl1.MinimumSize = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize;
		}
	}
}
