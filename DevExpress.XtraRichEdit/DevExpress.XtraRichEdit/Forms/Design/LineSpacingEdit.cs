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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Design {
	#region RepositoryItemLineSpacing
	[
	UserRepositoryItem("RegisterLineSpacingEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemLineSpacing : RepositoryItemImageComboBox {
		static Hashtable displayNameHT = new Hashtable();
		static RepositoryItemLineSpacing() {
			RegisterLineSpacingEdit();
			displayNameHT[ParagraphLineSpacing.Single] = XtraRichEditStringId.Caption_LineSpacingSingle;
			displayNameHT[ParagraphLineSpacing.Sesquialteral] = XtraRichEditStringId.Caption_LineSpacingSesquialteral;
			displayNameHT[ParagraphLineSpacing.Multiple] = XtraRichEditStringId.Caption_LineSpacingMultiple;
			displayNameHT[ParagraphLineSpacing.Exactly] = XtraRichEditStringId.Caption_LineSpacingExactly;
			displayNameHT[ParagraphLineSpacing.Double] = XtraRichEditStringId.Caption_LineSpacingDouble;
			displayNameHT[ParagraphLineSpacing.AtLeast] = XtraRichEditStringId.Caption_LineSpacingAtLeast;
		}
		public RepositoryItemLineSpacing() {
			InitItems();
		}
		#region RegisterLineSpacingEdit
		public static void RegisterLineSpacingEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Bitmaps256.RichTextEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(LineSpacingEdit).Name, typeof(LineSpacingEdit), typeof(RepositoryItemLineSpacing), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#endregion
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(LineSpacingEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		#endregion
		protected void InitItems() {
			BeginUpdate();
			try {
				Items.Clear();
				ParagraphLineSpacing[] values = (ParagraphLineSpacing[])Enum.GetValues(typeof(ParagraphLineSpacing));
				int count = values.Length;
				for (int i = 0; i < count; i++)
					Items.Add(new ImageComboBoxItem(GetDisplayName(values[i]), values[i], -1));
			}
			finally {
				CancelUpdate();
			}
		}
		static string GetDisplayName(ParagraphLineSpacing val) {
			return XtraRichEditLocalizer.GetString((XtraRichEditStringId)displayNameHT[val]);
		}
	}
	#endregion
	[DXToolboxItem(false), ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public class LineSpacingEdit : ImageComboBoxEdit {
		static LineSpacingEdit() {
			RepositoryItemLineSpacing.RegisterLineSpacingEdit();
		}
		#region Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object EditValue { get { return base.EditValue; } set { base.EditValue = value; } }
		public ParagraphLineSpacing? LineSpacing {
			get {
				if (EditValue is ParagraphLineSpacing)
					return (ParagraphLineSpacing)EditValue;
				return null;
			}
			set {
				ParagraphLineSpacing? oldValue = GetParagraphLineSpacingFromObject(EditValue);
				if (value == oldValue)
					return;
				EditValue = value;
			}
		}
		public override string EditorTypeName { get { return GetType().Name; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemLineSpacing Properties { get { return base.Properties as RepositoryItemLineSpacing; } }
		#endregion
		ParagraphLineSpacing? GetParagraphLineSpacingFromObject(object value) {
			if (value is ParagraphLineSpacing)
				return (ParagraphLineSpacing)EditValue;
			return null;
		}
	}
}
