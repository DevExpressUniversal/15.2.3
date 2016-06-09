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
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraBars;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap;
using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Extensions.Native;
namespace DevExpress.Snap.Extensions.UI {
	public class FilterPopupButtonItem : ControlCommandBarButtonItem<RichEditControl, RichEditCommandId>, IFilterOwner {
		PopupControlContainer container;
		public FilterPopupButtonItem() {
		}
		public FilterPopupButtonItem(BarManager manager)
			: base(manager) {
		}
		public FilterPopupButtonItem(string caption)
			: base(caption) {
		}
		public FilterPopupButtonItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		SnapControl SnapControl { get { return (SnapControl)Control; } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarButtonStyle ButtonStyle { get { return BarButtonStyle.DropDown; } set { } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupControl DropDownControl { get { return container; } set { } }
		protected override RichEditCommandId CommandId { get { return SnapCommandId.FilterFieldPlaceHolder; } }
		#endregion
		protected override Command CreateCommand() {
			return SnapControl != null ? new FilterFieldPlaceHolderCommand(SnapControl) : null;
		}
		protected override void Initialize() {
			base.Initialize();
			this.container = new PopupControlContainer();
			container.CloseOnOuterMouseClick = false;
			container.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			container.Popup += new EventHandler(container_Popup);
		}
		public override bool Enabled {
			get {
				if (SnapControl != null) {
					ListFieldSelectionController controller = new ListFieldSelectionController(SnapControl.DocumentModel);
					SnapFieldInfo fieldInfo = controller.FindDataField();
					if (fieldInfo != null && fieldInfo.ParsedInfo is MergefieldField) {
						IDataAccessService dataAccessService = SnapControl.GetService<IDataAccessService>();
						Type type = dataAccessService != null ? dataAccessService.GetFieldType(fieldInfo) : null;
						if (type != null && type == typeof(DateTime) && fieldInfo.Field.Parent == null)
							return false;
					}
				}
				return base.Enabled;
			}
			set {
				base.Enabled = value;
			}
		}
		void container_Popup(object sender, EventArgs e) {
			container.Controls.Clear();
			ListFieldSelectionController controller = new ListFieldSelectionController(SnapControl.DocumentModel);
			SnapFieldInfo fieldInfo = controller.FindDataField();
			if (fieldInfo == null)
				return;
			IDataAccessService dataAccessService = SnapControl != null ? SnapControl.GetService<IDataAccessService>() : null;
			Type type = dataAccessService != null ? dataAccessService.GetFieldType(fieldInfo) : null;
			if (type == null)
				return;
			MergefieldField parsedInfo = fieldInfo.ParsedInfo as MergefieldField;
			if (parsedInfo == null)
				return;
			string dataFieldName = parsedInfo.DataFieldName;
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if (underlyingType != null)
				type = underlyingType;
			if (type == typeof(DateTime)) {
				container.ShowSizeGrip = false;
				container.AutoSize = true;
				container.FormMinimumSize = new Size(100, 100);
				SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
				Field field = fieldInfo.Field.Parent;
				if (field == null)
					return;
				SNListField snList = calculator.ParseField(fieldInfo.PieceTable, field) as SNListField;
				if (snList == null)
					return;
				IFilterStringAccessService filterStringAccessService = SnapControl.GetService<IFilterStringAccessService>();
				string filterString = filterStringAccessService != null ? filterStringAccessService.GetFilterString(snList) : null;
				CriteriaOperator filterCriteria = FilterStringHelper.GetFilterStringOnlyWithPropertyName(filterString, dataFieldName);
				var values = dataAccessService.GetFilterValues(fieldInfo);
				container.Controls.Add(new DateFilterControlUserControl(this, dataFieldName, values, filterCriteria));
			}
			else {
				container.ShowSizeGrip = true;
				container.AutoSize = false;
				container.FormMinimumSize = new Size(200, 100);
				container.Size = new Size(200, 250);
				FilterPopupUserControl userControl = new FilterPopupUserControl(new FilterItems(this, dataFieldName, dataAccessService.GetFilterItems(fieldInfo))) { Dock = DockStyle.Fill };
				container.Controls.Add(userControl);
			}
		}
		public void ApplyFilter(string dataFieldName, CriteriaOperator filter) {
			new SnapFilterFieldCommand(SnapControl, dataFieldName, filter).Execute();
		}
		public void ApplyFilter(string dataFieldName, string[] values) {
			new SnapFilterFieldCommand(SnapControl, dataFieldName, values).Execute();
		}
	}
	public class FilterItems : IEnumerable<FilterItem> {
		readonly List<FilterItem> filterItems;
		readonly IFilterOwner filterOwner;
		readonly string dataFieldName;
		public FilterItems(IFilterOwner filterOwner, string dataFieldName, IEnumerable<FilterItem> filterItems) {
			this.filterOwner = filterOwner;
			this.dataFieldName = dataFieldName;
			this.filterItems = new List<FilterItem>(filterItems);
		}
		public void ApplyFilter() {
			List<string> values = new List<string>();
			filterItems.ForEach(item => {
				if (item.IsChecked != null && (bool)item.IsChecked) {
					values.Add(item.ToString());
				}
			});
			filterOwner.ApplyFilter(dataFieldName, values.ToArray());
		}
		public void CheckAllItems(bool isChecked) {
			filterItems.ForEach(item => { item.IsChecked = isChecked; });
		}
		public bool? CheckState {
			get {
				int count = 0;
				filterItems.ForEach(item => {
					if (item.IsChecked == true) {
						count++;
					}
				});
				return count == filterItems.Count ? true : count == 0 ? (bool?)false : null;
			}
		}
		IEnumerator<FilterItem> IEnumerable<FilterItem>.GetEnumerator() {
			return filterItems.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return filterItems.GetEnumerator();
		}
	}
}
