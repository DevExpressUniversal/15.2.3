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
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using System.Collections;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraRichEdit.Localization;
using System.Drawing;
using System.Reflection;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.Office;
using DevExpress.Office.Design.Internal;
using DevExpress.Office.Localization;
namespace DevExpress.XtraRichEdit.Design {
	#region RepositoryItemWidhtType
	[
	UserRepositoryItem("RegisterHeightType"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemWidthType : RepositoryItemImageComboBox {
		static Hashtable displayNameHT = new Hashtable();
		static RepositoryItemWidthType() {
			RegisterWidthType();
			UpdateDisplayNames();
		}
		static void UpdateDisplayNames() {
			displayNameHT[WidthUnitType.FiftiethsOfPercent] = OfficeStringId.Caption_UnitPercent;
			displayNameHT[WidthUnitType.ModelUnits] = UIUnit.UnitCaptionDictionary[defaultUnitType];
		}
		public static void RegisterWidthType() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Bitmaps256.RichTextEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(WidthTypeEdit).Name, typeof(WidthTypeEdit), typeof(RepositoryItemWidthType), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		const DocumentUnit defaultUnitType = DocumentUnit.Inch;
		DocumentUnit unitType;
		public RepositoryItemWidthType() {
			this.unitType = defaultUnitType;
			InitItems();
		}
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(WidthTypeEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		public DocumentUnit UnitType {
			get {
				return unitType;
			}
			set {
				if (unitType == value)
					return;
				unitType = value;
				SetNewDescription();
			}
		}
		#endregion
		void SetNewDescription() {
			ImageComboBoxItem item = GetItemContainingModelUnitValue();
			item.Description = UIUnit.GetTextCaption(unitType);
		}
		ImageComboBoxItem GetItemContainingModelUnitValue() {
			int itemsCount = Items.Count;
			for (int i = 0; i < itemsCount; i++) {
				ImageComboBoxItem curentItem = Items[i];
				if ((WidthUnitType)curentItem.Value == WidthUnitType.ModelUnits)
					return curentItem;
			}
			return null;
		}
		protected void InitItems() {
			BeginUpdate();
			try {
				Items.Clear();
				WidthUnitType[] values = (WidthUnitType[])Enum.GetValues(typeof(WidthUnitType));
				int count = values.Length;
				for (int i = 0; i < count; i++) {
					WidthUnitType currentValue = values[i];
					if (currentValue != WidthUnitType.Auto && currentValue != WidthUnitType.Nil) {
						Items.Add(new ImageComboBoxItem(GetDisplayName(currentValue), currentValue, -1));
					}
				}
			}
			finally {
				CancelUpdate();
			}
		}
		static string GetDisplayName(WidthUnitType val) {
			return OfficeLocalizer.GetString((OfficeStringId)displayNameHT[val]);
		}
	}
	#endregion
	#region WidthTypeEdit
	[DXToolboxItem(false), ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public class WidthTypeEdit : ImageComboBoxEdit {
		static WidthTypeEdit() {
			RepositoryItemWidthType.RegisterWidthType();
		}
		#region Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object EditValue {
			get {
				return base.EditValue;
			}
			set {
				base.EditValue = value;
			}
		}
		public new WidthUnitType? Value {
			get {
				if (EditValue is WidthUnitType)
					return (WidthUnitType)EditValue;
				return null;
			}
			set {
				WidthUnitType? oldValue = GetHeightTypeFromObject(EditValue);
				if (value == oldValue)
					return;
				EditValue = value;
			}
		}
		public override string EditorTypeName { get { return GetType().Name; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemWidthType Properties { get { return base.Properties as RepositoryItemWidthType; } }
		#endregion
		WidthUnitType? GetHeightTypeFromObject(object value) {
			if (value is WidthUnitType)
				return (WidthUnitType)EditValue;
			return null;
		}
	}
	#endregion
}
