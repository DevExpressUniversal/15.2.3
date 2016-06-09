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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using System.Collections;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils;
using System.Windows.Controls;
namespace DevExpress.Xpf.RichEdit.UI {
	#region HeightTypeEdit
	[DXToolboxBrowsableAttribute(false)]
	public class HeightTypeEdit : ComboBoxEdit {
		static Hashtable displayNameHT = new Hashtable();
		static HeightTypeEdit() {
			displayNameHT[HeightUnitType.Exact] = XtraRichEditStringId.Caption_HeightTypeExact;
			displayNameHT[HeightUnitType.Minimum] = XtraRichEditStringId.Caption_HeightTypeMinimum;
		}
		public HeightTypeEdit()
			: base() {
			IsTextEditable = false;
			this.DisplayMember = "DisplayText";
			Populate();
		}
		#region Properties
		public HeightUnitType? Value {
			get {
				if (EditValue is HeightTypeEditItem)
					return ((HeightTypeEditItem)EditValue).HeightUnitType;
				return null;
			}
			set {
				HeightUnitType? oldValue = GetHeightTypeFromObject(EditValue);
				if (value == oldValue)
					return;
				EditValue = GetItemContainingUnitValue(value);
			}
		}
		#endregion
		HeightUnitType? GetHeightTypeFromObject(object value) {
			if (value is HeightTypeEditItem)
				return ((HeightTypeEditItem)EditValue).HeightUnitType;
			return null;
		}
		HeightTypeEditItem GetItemContainingUnitValue(HeightUnitType? unitType) {
			if (unitType == null)
				return null;
			int itemsCount = Items.Count;
			for (int i = 0; i < itemsCount; i++) {
				HeightTypeEditItem currentItem = Items[i] as HeightTypeEditItem;
				if (currentItem.HeightUnitType == unitType)
					return currentItem;
			}
			return null;
		}
		protected virtual void Populate() {
			using (DocumentModel documentModel = new DocumentModel(DevExpress.XtraRichEdit.Internal.RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies())) {
				Items.Clear();
				foreach (HeightUnitType item in DevExpress.Utils.EnumExtensions.GetValues(typeof(HeightUnitType))) {
					if (item != HeightUnitType.Auto) {
						HeightTypeEditItem editItem = new HeightTypeEditItem();
						editItem.HeightUnitType = item;
						editItem.DisplayText = GetDisplayName(item);
						Items.Add(editItem);
					}
				}
			}
		}
		static string GetDisplayName(HeightUnitType val) {
			return XtraRichEditLocalizer.GetString((XtraRichEditStringId)displayNameHT[val]);
		}
	}
	#endregion
	#region HeightTypeEditItem
	public class HeightTypeEditItem  {
		HeightUnitType heightUnitType;
		string displayText;
		public HeightUnitType HeightUnitType { get { return heightUnitType; } set { heightUnitType = value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
	}
	#endregion
}
