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
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using System.Collections;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using System.Drawing;
using DevExpress.XtraEditors.Controls;
using System.Reflection;
namespace DevExpress.XtraRichEdit.Design {
	#region RepositoryItemHeightType
	[
	UserRepositoryItem("RegisterHeightType"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemHeightType : RepositoryItemImageComboBox {
		static Hashtable displayNameHT = new Hashtable();
		static RepositoryItemHeightType() {
			RegisterHeightType();
			displayNameHT[HeightUnitType.Exact] = XtraRichEditStringId.Caption_HeightTypeExact;
			displayNameHT[HeightUnitType.Minimum] = XtraRichEditStringId.Caption_HeightTypeMinimum;
		}
		public RepositoryItemHeightType() {
			InitItems();
		}
		#region RegisterHeightType
		public static void RegisterHeightType() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Bitmaps256.RichTextEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(HeightTypeEdit).Name, typeof(HeightTypeEdit), typeof(RepositoryItemHeightType), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#endregion
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(HeightTypeEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		#endregion
		protected void InitItems() {
			BeginUpdate();
			try {
				Items.Clear();
				HeightUnitType[] values = (HeightUnitType[])Enum.GetValues(typeof(HeightUnitType));
				int count = values.Length;
				for (int i = 0; i < count; i++) {
					HeightUnitType currentValue = values[i];
					if (currentValue != HeightUnitType.Auto) {
						Items.Add(new ImageComboBoxItem(GetDisplayName(currentValue), currentValue, -1));
					}
				}
			}
			finally {
				CancelUpdate();
			}
		}
		static string GetDisplayName(HeightUnitType val) {
			return XtraRichEditLocalizer.GetString((XtraRichEditStringId)displayNameHT[val]);
		}
	}
	#endregion
	#region HeightTypeEdit
	[DXToolboxItem(false), ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public class HeightTypeEdit : ImageComboBoxEdit {
		static HeightTypeEdit() {
			RepositoryItemHeightType.RegisterHeightType();
		}
		public HeightTypeEdit() {
			EditValue = String.Empty;
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
		public new HeightUnitType? Value {
			get {
				if (EditValue is HeightUnitType)
					return (HeightUnitType)EditValue;
				return null;
			}
			set {
				HeightUnitType? oldValue = GetHeightTypeFromObject(EditValue);
				if (value == oldValue)
					return;
				EditValue = value;
			}
		}
		public override string EditorTypeName { get { return GetType().Name; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemHeightType Properties { get { return base.Properties as RepositoryItemHeightType; } }
		#endregion
		HeightUnitType? GetHeightTypeFromObject(object value) {
			if (value is HeightUnitType)
				return (HeightUnitType)EditValue;
			return null;
		}
	}
	#endregion
}
