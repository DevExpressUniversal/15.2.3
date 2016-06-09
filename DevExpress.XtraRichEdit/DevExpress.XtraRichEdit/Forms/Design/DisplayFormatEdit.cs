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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraEditors.Repository;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Registrator;
using DevExpress.Utils;
using System.Drawing;
using System.Reflection;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using System.Globalization;
using DevExpress.Office.NumberConverters;
namespace DevExpress.XtraRichEdit.Forms.Design {
	#region RepositoryItemDisplayFormat
	[
	UserRepositoryItem("RegisterDisplayFormatEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemDisplayFormat : RepositoryItemImageComboBox {
		static RepositoryItemDisplayFormat() {
			RegisterDisplayFormatEdit();
		}
		public RepositoryItemDisplayFormat() {
			InitItems();
		}
		#region RegisterDisplayFormatEdit
		public static void RegisterDisplayFormatEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Bitmaps256.RichTextEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(DisplayFormatEdit).Name, typeof(DisplayFormatEdit), typeof(RepositoryItemDisplayFormat), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#endregion
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(DisplayFormatEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		#endregion
		protected internal virtual void InitItems() {
			LanguageId languageId = CultureInfoToLanguageIdConverter.GetLanguageId(System.Threading.Thread.CurrentThread.CurrentUICulture);
			BeginUpdate();
			try {
				Items.Clear();
				List<NumberingFormat> values = OrdinalBasedNumberConverter.GetSupportNumberingFormat();
				int count = values.Count;
				for (int i = 0; i < count; i++)
					Items.Add(new ImageComboBoxItem(GetDisplayFormat(values[i], languageId), values[i], -1));
			}
			finally {
				CancelUpdate();
			}
		}
		protected internal string GetDisplayFormat(NumberingFormat format, LanguageId languageId) {
			OrdinalBasedNumberConverter converter = OrdinalBasedNumberConverter.CreateConverter(format, languageId);
			return String.Format("{0},{1},{2}...", converter.ConvertNumber(1), converter.ConvertNumber(2), converter.ConvertNumber(3));
		}
	}
	#endregion
	#region DisplayFormatEdit
#if !DEBUGTEST
	[DXToolboxItem(false), ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
#endif
	public class DisplayFormatEdit : ImageComboBoxEdit {
		static DisplayFormatEdit() {
			RepositoryItemDisplayFormat.RegisterDisplayFormatEdit();
		}
		#region Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object EditValue { get { return base.EditValue; } set { base.EditValue = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public NumberingFormat DisplayFormat { get { return (NumberingFormat)EditValue; } set { EditValue = value; } }
		public override string EditorTypeName { get { return GetType().Name; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemDisplayFormat Properties { get { return base.Properties as RepositoryItemDisplayFormat; } }
		#endregion
	}
	#endregion
}
