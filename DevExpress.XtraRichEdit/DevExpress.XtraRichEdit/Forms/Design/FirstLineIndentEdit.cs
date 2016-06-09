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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Design {
	#region RepositoryItemWeekOfMonth
	[
	UserRepositoryItem("RegisterFirstLineIndent"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemFirstLineIndent : RepositoryItemImageComboBox {
		static Hashtable displayNameHT = new Hashtable();
		static RepositoryItemFirstLineIndent() {
			RegisterFirstLineIndent();
			displayNameHT[ParagraphFirstLineIndent.None] = XtraRichEditStringId.Caption_FirstLineIndentNone;
			displayNameHT[ParagraphFirstLineIndent.Indented] = XtraRichEditStringId.Caption_FirstLineIndentIndented;
			displayNameHT[ParagraphFirstLineIndent.Hanging] = XtraRichEditStringId.Caption_FirstLineIndentHanging;
		}
		public RepositoryItemFirstLineIndent() {
			InitItems();
		}
		#region RegisterWeekFirstLineIndent
		public static void RegisterFirstLineIndent() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Bitmaps256.RichTextEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(FirstLineIndentTypeEdit).Name, typeof(FirstLineIndentTypeEdit), typeof(RepositoryItemFirstLineIndent), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#endregion
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(FirstLineIndentTypeEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		#endregion
		protected void InitItems() {
			BeginUpdate();
			try {
				Items.Clear();
				ParagraphFirstLineIndent[] values = (ParagraphFirstLineIndent[])Enum.GetValues(typeof(ParagraphFirstLineIndent));
				int count = values.Length;
				for (int i = 0; i < count; i++)
					Items.Add(new ImageComboBoxItem(GetDisplayName(values[i]), values[i], -1));
			}
			finally {
				CancelUpdate();
			}
		}
		static string GetDisplayName(ParagraphFirstLineIndent val) {
			return XtraRichEditLocalizer.GetString((XtraRichEditStringId)displayNameHT[val]);
		}
	}
	#endregion
	#region FirstLineIndentTypeEdit
	[DXToolboxItem(false), ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public class FirstLineIndentTypeEdit : ImageComboBoxEdit {
		static FirstLineIndentTypeEdit() {
			RepositoryItemFirstLineIndent.RegisterFirstLineIndent();
		}
		public FirstLineIndentTypeEdit() {
			EditValue = String.Empty;
		}
		ParagraphFirstLineIndent? GetParagraphFirstLineIndentFromObject(object value) {
			if (value is ParagraphFirstLineIndent)
				return (ParagraphFirstLineIndent)EditValue;
			return null;
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
		public new ParagraphFirstLineIndent? Value {
			get {
				if (EditValue is ParagraphFirstLineIndent)
					return (ParagraphFirstLineIndent)EditValue;
				return null;
			}
			set {
				ParagraphFirstLineIndent? oldValue = GetParagraphFirstLineIndentFromObject(EditValue);
				if (value == oldValue)
					return;
				EditValue = value;
			}
		}
		public override string EditorTypeName { get { return GetType().Name; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemFirstLineIndent Properties { get { return base.Properties as RepositoryItemFirstLineIndent; } }
		#endregion
	}
	#endregion
}
