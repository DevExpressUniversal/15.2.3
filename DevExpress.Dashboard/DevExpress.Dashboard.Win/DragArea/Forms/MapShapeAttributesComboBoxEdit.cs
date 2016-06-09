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

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.DashboardWin.Native {
	[UserRepositoryItem("RepositoryItemMapShapeAttributesComboBoxEdit")]
	public class RepositoryItemMapShapeAttributesComboBoxEdit : RepositoryItemImageComboBox {
		static RepositoryItemMapShapeAttributesComboBoxEdit() { RegisterMapShapeAttributesComboBox(); }
		public RepositoryItemMapShapeAttributesComboBoxEdit() {
		}
		public const string MapShapeAttributesComboBoxEditName = "MapShapeAttributesComboBoxEdit";
		public override string EditorTypeName { get { return MapShapeAttributesComboBoxEditName; } }
		public static void RegisterMapShapeAttributesComboBox() {			
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(MapShapeAttributesComboBoxEditName,
			  typeof(MapShapeAttributesComboBoxEdit), typeof(RepositoryItemMapShapeAttributesComboBoxEdit),
			  typeof(ImageComboBoxEditViewInfo), new ButtonEditPainter(), true));
		}
	}
	[DXToolboxItem(false)]
	public partial class MapShapeAttributesComboBoxEdit : ImageComboBoxEdit {
		static MapShapeAttributesComboBoxEdit() { RepositoryItemMapShapeAttributesComboBoxEdit.RegisterMapShapeAttributesComboBox(); }
		MapDashboardItem dashboardItem;
		DashboardDesigner designer;
		bool useNoneValue;
		public override string EditorTypeName { get { return RepositoryItemMapShapeAttributesComboBoxEdit.MapShapeAttributesComboBoxEditName; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemMapShapeAttributesComboBoxEdit Properties {
			get { return base.Properties as RepositoryItemMapShapeAttributesComboBoxEdit; }
		}
		string DefaultValue { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.MapShapeNoneAttribute); } }
		public string SelectedAttribute { get { return AttributeByIndex(SelectedIndex); } }
		public MapShapeAttributesComboBoxEdit()
			: base() {
			Properties.Buttons.Add(new EditorButton(ButtonPredefines.Combo));
			Properties.Buttons.Add(new EditorButton(ButtonPredefines.Search));
			ButtonClick += ShapeAttributesComboBoxEdit_ButtonClick;
		}
		public void Initialize(DashboardDesigner designer, MapDashboardItem dashboardItem, object imageList, string selectedValue, bool useNoneValue) {
			this.designer = designer;
			this.dashboardItem = dashboardItem;
			this.useNoneValue = useNoneValue;
			Properties.SmallImages = imageList;
			BindAttributes(selectedValue);
		}
		void BindAttributes(string selectedValue) {
			List<ImageComboBoxItem> attributes = new List<ImageComboBoxItem>();
			if(useNoneValue)
				attributes.Add(new ImageComboBoxItem(DefaultValue, typeof(object)));
			attributes.AddRange(dashboardItem.MapItemAttributes.Select(attr => new ImageComboBoxItem(attr.Name, DataFieldsBrowserItem.GetImageIndex(attr.Type))).ToList());
			if(attributes.Count > 0) {
				Properties.Items.AddRange(attributes);
				string attribute = selectedValue ?? (useNoneValue ? DefaultValue : null);
				SetSelection(attribute);
			}
		}
		void SetSelection(string attribute) {
			SelectedIndex = IndexOfAttribute(attribute);
			if(SelectedIndex < 0)
				SelectedIndex = 0;
		}
		string AttributeByIndex(int index) {
			int correctIndex = useNoneValue ? index - 1 : index;
			return correctIndex != -1 ? dashboardItem.MapItemAttributes[correctIndex].Name : null;
		}
		int IndexOfAttribute(string attribute) {
			int index = dashboardItem.MapItemAttributes.Select(attr => attr.Name).ToList().IndexOf(attribute);
			return useNoneValue ? index + 1 : index;
		}
		void ShapeAttributesComboBoxEdit_ButtonClick(object sender, ButtonPressedEventArgs e) {
			if(e.Button.Kind == ButtonPredefines.Search)
				PreviewValues();
		}
		void PreviewValues() {
			using(MapAttributePreviewForm form = new MapAttributePreviewForm()) {
				form.LookAndFeel.ParentLookAndFeel = designer.LookAndFeel;
				form.DataSource = PreparePreviewDataSource();
				form.PrepareImages(Properties.SmallImages);
				form.SelectedAttribute = SelectedAttribute;
				DialogResult result = form.ShowDialog();
				if(result == DialogResult.OK)
					SetSelection(form.SelectedAttribute);
			}
		}
		protected DataTable PreparePreviewDataSource() {
			DataTable dataTable = new DataTable();
			dataTable.Locale = CultureInfo.InvariantCulture;
			foreach(DashboardMapItemAttribute attr in dashboardItem.MapItemAttributes)
				dataTable.Columns.Add(attr.Name, attr.Type);
			foreach(MapShapeItem mapItem in dashboardItem.MapItems) {
				dataTable.Rows.Add(dashboardItem.MapItemAttributes
					.Select(attribute => mapItem.Attributes
						.FirstOrDefault(attr => attr.Name == attribute.Name))
						.Select(attr => attr != null ? attr.Value : null)
						.ToArray());
			}
			return dataTable;
		}
	}
}
