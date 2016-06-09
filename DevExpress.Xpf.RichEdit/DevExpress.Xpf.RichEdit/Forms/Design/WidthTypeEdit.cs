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
using DevExpress.Office;
using System.Collections;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.Office.Design.Internal;
using DevExpress.Office.Localization;
namespace DevExpress.Xpf.RichEdit.UI {
	#region WidthTypeEdit
	[DXToolboxBrowsableAttribute(false)]
	public class WidthTypeEdit : ComboBoxEdit {
		static Hashtable displayNameHT = new Hashtable();
		static WidthTypeEdit() {
			UpdateDisplayNames();
		}
		static void UpdateDisplayNames() {
			displayNameHT[WidthUnitType.FiftiethsOfPercent] = OfficeStringId.Caption_UnitPercent;
			displayNameHT[WidthUnitType.ModelUnits] = UIUnit.UnitCaptionDictionary[defaultUnitType];
		}
		const DocumentUnit defaultUnitType = DocumentUnit.Inch;
		DocumentUnit unitType;
		public WidthTypeEdit()
			: base() {
			IsTextEditable = false;
			this.unitType = defaultUnitType;
			this.DisplayMember = "DisplayText";
			Populate();
		}
		#region Properties
		public DocumentUnit UnitType {
			get { return unitType; }
			set {
				if (unitType == value)
					return;
				unitType = value;
				SetNewDescription();
			}
		}
		public WidthUnitType? Value {
			get {
				if (EditValue is WidthTypeEditItem)
					return ((WidthTypeEditItem)EditValue).WidthUnitType;
				return null;
			}
			set {
				WidthUnitType? oldValue = GetWidthTypeFromObject(EditValue);
				if (value == oldValue)
					return;
				EditValue = GetItemContainingUnitValue(value);
			}
		}
		#endregion
		WidthUnitType? GetWidthTypeFromObject(object value) {
			if (value is WidthUnitType)
				return (WidthUnitType)EditValue;
			return null;
		}
		void SetNewDescription() {
			WidthTypeEditItem item = GetItemContainingUnitValue(WidthUnitType.ModelUnits);
			item.DisplayText = UIUnit.GetTextCaption(unitType);
		}
		WidthTypeEditItem GetItemContainingUnitValue(WidthUnitType? unitType) {
			if (unitType == null)
				return null;
			int itemsCount = Items.Count;
			for (int i = 0; i < itemsCount; i++) {
				WidthTypeEditItem currentItem = Items[i] as WidthTypeEditItem;
				if (currentItem.WidthUnitType == unitType)
					return currentItem;
			}
			return null;
		}
		protected virtual void Populate() {
			using (DocumentModel documentModel = new DocumentModel(DevExpress.XtraRichEdit.Internal.RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies())) {
				Items.Clear();
				foreach (WidthUnitType item in DevExpress.Utils.EnumExtensions.GetValues(typeof(WidthUnitType))) {
					if (item != WidthUnitType.Auto && item != WidthUnitType.Nil) {
						WidthTypeEditItem editItem = new WidthTypeEditItem();
						editItem.WidthUnitType = item;
						editItem.DisplayText = GetDisplayName(item);
						Items.Add(editItem);
					}
				}
			}
		}
		static string GetDisplayName(WidthUnitType val) {
			return OfficeLocalizer.GetString((OfficeStringId)displayNameHT[val]);
		}
	}
	#endregion
	#region WidthTypeEditItem
	public class WidthTypeEditItem  {
		WidthUnitType widthUnitType;
		string displayText;
		public WidthUnitType WidthUnitType { get { return widthUnitType; } set { widthUnitType = value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
	}
	#endregion
}
