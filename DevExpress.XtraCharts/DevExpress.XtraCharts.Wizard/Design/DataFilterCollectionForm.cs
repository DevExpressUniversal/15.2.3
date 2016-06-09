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

namespace DevExpress.XtraCharts.Design {
	public class DataFilterCollectionForm : DevExpress.XtraEditors.XtraForm {
		private System.ComponentModel.Container components = null;
		private DevExpress.Utils.Frames.PropertyGridEx propertyGrid;
		IDataFilterCollectionAccessor collectionAccessor;
		private DevExpress.XtraEditors.RadioGroup rgConjunction;
		private DevExpress.XtraEditors.ListBoxControl lbFilters;
		private DevExpress.XtraEditors.SimpleButton btnAdd;
		private DevExpress.XtraEditors.SimpleButton btnRemove;
		private DevExpress.XtraEditors.SimpleButton btnClose;
		private DevExpress.XtraEditors.LabelControl lblSplitter;
		DataFilterCollectionForm() {
			InitializeComponent();
		}
		public DataFilterCollectionForm(DataFilterCollection dataFilters) : this(new DataFilterCollectionAccessor(dataFilters)) {
		}
		public DataFilterCollectionForm(IDataFilterCollectionAccessor collectionAccessor) : this() {
			this.collectionAccessor = collectionAccessor;
			if (collectionAccessor.Count > 0) {
				for (int i = 0; i < collectionAccessor.Count; i++)
					lbFilters.Items.Add(collectionAccessor[i].ToString());
				lbFilters.SelectedIndex = 0;
			}
			rgConjunction.SelectedIndex = collectionAccessor.ConjunctionMode == ConjunctionTypes.And ? 0 : 1;
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataFilterCollectionForm));
			this.propertyGrid = new DevExpress.Utils.Frames.PropertyGridEx();
			this.rgConjunction = new DevExpress.XtraEditors.RadioGroup();
			this.lbFilters = new DevExpress.XtraEditors.ListBoxControl();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.lblSplitter = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.rgConjunction.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbFilters)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.propertyGrid, "propertyGrid");
			this.propertyGrid.CommandsActiveLinkColor = System.Drawing.SystemColors.ActiveCaption;
			this.propertyGrid.CommandsDisabledLinkColor = System.Drawing.SystemColors.ControlDark;
			this.propertyGrid.CommandsLinkColor = System.Drawing.SystemColors.ActiveCaption;
			this.propertyGrid.DrawFlat = true;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.ToolbarVisible = false;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
			resources.ApplyResources(this.rgConjunction, "rgConjunction");
			this.rgConjunction.Name = "rgConjunction";
			this.rgConjunction.Properties.Columns = 2;
			this.rgConjunction.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgConjunction.Properties.Items"))), resources.GetString("rgConjunction.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgConjunction.Properties.Items2"))), resources.GetString("rgConjunction.Properties.Items3"))});
			this.rgConjunction.SelectedIndexChanged += new System.EventHandler(this.rgConjunction_SelectedIndexChanged);
			resources.ApplyResources(this.lbFilters, "lbFilters");
			this.lbFilters.Name = "lbFilters";
			this.lbFilters.SelectedIndexChanged += new System.EventHandler(this.lbFilters_SelectedIndexChanged);
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			resources.ApplyResources(this.btnRemove, "btnRemove");
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Name = "btnClose";
			resources.ApplyResources(this.lblSplitter, "lblSplitter");
			this.lblSplitter.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.lblSplitter.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.lblSplitter.LineVisible = true;
			this.lblSplitter.Name = "lblSplitter";
			this.CancelButton = this.btnClose;
			resources.ApplyResources(this, "$this");
			this.ControlBox = false;
			this.Controls.Add(this.lblSplitter);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.lbFilters);
			this.Controls.Add(this.rgConjunction);
			this.Controls.Add(this.propertyGrid);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DataFilterCollectionForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Load += new System.EventHandler(this.DataFilterCollectionForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.rgConjunction.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbFilters)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		void EnableControls() {
			btnRemove.Enabled = lbFilters.SelectedIndex >= 0;
		}
		private void DataFilterCollectionForm_Load(object sender, System.EventArgs e) {
			lbFilters.Focus();
		}
		private void lbFilters_SelectedIndexChanged(object sender, System.EventArgs e) {
			propertyGrid.SelectedObject = (lbFilters.SelectedIndex >= 0 && lbFilters.Items.Count > lbFilters.SelectedIndex) ?
				collectionAccessor[lbFilters.SelectedIndex] : null;
			EnableControls();
		}
		private void btnAdd_Click(object sender, System.EventArgs e) {
			DataFilter filter = new DataFilter();
			int index = collectionAccessor.Add(filter);
			if (index >= 0) {
				lbFilters.Items.Add(filter.ToString());
				lbFilters.SelectedIndex = index;
			}
		}
		private void btnRemove_Click(object sender, System.EventArgs e) {
			int selectedIndex = lbFilters.SelectedIndex;
			if (selectedIndex >= 0) {
				collectionAccessor.RemoveAt(selectedIndex);
				lbFilters.Items.RemoveAt(selectedIndex);
				if (lbFilters.Items.Count > 0)
					lbFilters.SelectedIndex = selectedIndex >= lbFilters.Items.Count ?
						lbFilters.Items.Count - 1 : selectedIndex;
			}
		}
		private void propertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e) {
			if (e.ChangedItem.Label == "ColumnName") {
				int selectedIndex = lbFilters.SelectedIndex;
				if (selectedIndex >= 0) {
					lbFilters.Items.RemoveAt(selectedIndex);
					lbFilters.Items.Insert(selectedIndex, collectionAccessor[selectedIndex]);
					lbFilters.SelectedIndex = selectedIndex;
				}
			}
		}
		private void rgConjunction_SelectedIndexChanged(object sender, System.EventArgs e) {
			ConjunctionTypes newConjunctionMode = rgConjunction.SelectedIndex == 0 ? ConjunctionTypes.And : ConjunctionTypes.Or;
			if (newConjunctionMode != collectionAccessor.ConjunctionMode)
				collectionAccessor.ConjunctionMode = newConjunctionMode;
		}
	}
	public interface IDataFilterCollectionAccessor {
		ConjunctionTypes ConjunctionMode { get; set; }
		object this[int index] { get; }
		int Count { get; }
		void RemoveAt(int index);
		int Add(DataFilter item);
	}
	public class DataFilterCollectionAccessor : IDataFilterCollectionAccessor {
		readonly DataFilterCollection collection;
		public DataFilterCollectionAccessor(DataFilterCollection collection) {
			this.collection = collection;
		}
		#region IDataFilterCollectionAccessor Members
		ConjunctionTypes IDataFilterCollectionAccessor.ConjunctionMode {
			get { return collection.ConjunctionMode; }
			set { collection.ConjunctionMode = value; }
		}
		object IDataFilterCollectionAccessor.this[int index] { get { return collection[index]; } }
		int IDataFilterCollectionAccessor.Count { get { return collection.Count; } }
		void IDataFilterCollectionAccessor.RemoveAt(int index) {
			collection.RemoveAt(index);
		}
		int IDataFilterCollectionAccessor.Add(DataFilter item) {
			return collection.Add(item);
		}
		#endregion
	}
}
