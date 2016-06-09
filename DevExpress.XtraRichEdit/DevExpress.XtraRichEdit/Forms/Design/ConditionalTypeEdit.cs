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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Design {
	#region RepositoryItemConditionalType
	[
	UserRepositoryItem("RegisterConditionalTypeEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemConditionalType : RepositoryItemImageComboBox {
		static Hashtable displayNameHT = new Hashtable();
		static RepositoryItemConditionalType() {
			RegisterConditionalTypeEdit();
			displayNameHT[ConditionalTableStyleFormattingTypes.WholeTable] = XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_WholeTable;
			displayNameHT[ConditionalTableStyleFormattingTypes.FirstRow] = XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_FirstRow;
			displayNameHT[ConditionalTableStyleFormattingTypes.LastRow] = XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_LastRow;
			displayNameHT[ConditionalTableStyleFormattingTypes.FirstColumn] = XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_FirstColumn;
			displayNameHT[ConditionalTableStyleFormattingTypes.LastColumn] = XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_LastColumn;
			displayNameHT[ConditionalTableStyleFormattingTypes.OddRowBanding] = XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_OddRowBanding;
			displayNameHT[ConditionalTableStyleFormattingTypes.EvenRowBanding] = XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_EvenRowBanding;
			displayNameHT[ConditionalTableStyleFormattingTypes.OddColumnBanding] = XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_OddColumnBanding;
			displayNameHT[ConditionalTableStyleFormattingTypes.EvenColumnBanding] = XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_EvenColumnBanding;
			displayNameHT[ConditionalTableStyleFormattingTypes.TopLeftCell] = XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_TopLeftCell;
			displayNameHT[ConditionalTableStyleFormattingTypes.TopRightCell] = XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_TopRightCell;
			displayNameHT[ConditionalTableStyleFormattingTypes.BottomLeftCell] = XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_BottomLeftCell;
			displayNameHT[ConditionalTableStyleFormattingTypes.BottomRightCell] = XtraRichEditStringId.Caption_ConditionalTableStyleFormattingTypes_BottomRightCell;
		}
		public RepositoryItemConditionalType() {
			InitItems();
		}
		#region RegisterConditionalTypeEdit
		public static void RegisterConditionalTypeEdit() {
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(ConditionalTypeEdit).Name, typeof(ConditionalTypeEdit), typeof(RepositoryItemConditionalType), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, null);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#endregion
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(ConditionalTypeEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		#endregion
		protected internal virtual void InitItems() {
			BeginUpdate();
			try {
				Items.Clear();
				ConditionalTableStyleFormattingTypes[] values = (ConditionalTableStyleFormattingTypes[])Enum.GetValues(typeof(ConditionalTableStyleFormattingTypes));
				int count = values.Length;
				for (int i = count - 1; i >= 0; i--)
					Items.Add(new ImageComboBoxItem(GetDisplayName(values[i]), values[i], -1));
			}
			finally {
				CancelUpdate();
			}
		}
		static string GetDisplayName(ConditionalTableStyleFormattingTypes val) {
			return XtraRichEditLocalizer.GetString((XtraRichEditStringId)displayNameHT[val]);
		}
	}
	#endregion
	#region ConditionalTypeEdit
#if !DEBUGTEST
	[DXToolboxItem(false), ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
#endif
	public class ConditionalTypeEdit : ImageComboBoxEdit {
		static ConditionalTypeEdit() {
			RepositoryItemConditionalType.RegisterConditionalTypeEdit();
		}
		#region Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object EditValue { get { return base.EditValue; } set { base.EditValue = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ConditionalTableStyleFormattingTypes? ConditionalType {
			get {
				if (EditValue is ConditionalTableStyleFormattingTypes)
					return (ConditionalTableStyleFormattingTypes)EditValue;
				return null;
			}
			set {
				if (value.HasValue)
					EditValue = value.Value;
				else
					EditValue = String.Empty;
			}
		}
		public override string EditorTypeName { get { return GetType().Name; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemConditionalType Properties { get { return base.Properties as RepositoryItemConditionalType; } }
		#endregion
	}
	#endregion
}
