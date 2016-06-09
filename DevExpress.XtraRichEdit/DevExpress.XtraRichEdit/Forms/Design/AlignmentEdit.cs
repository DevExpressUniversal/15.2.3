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
using System.Collections;
using System.Drawing;
using System.Reflection;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Design {
	#region RepositoryItemAlignment
	[
	UserRepositoryItem("RegisterAlignmentEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemAlignment : RepositoryItemImageComboBox {
		static Hashtable displayNameHT = new Hashtable();
		static RepositoryItemAlignment() {
			RegisterAlignmentEdit();
			displayNameHT[ParagraphAlignment.Center] = XtraRichEditStringId.Caption_AlignmentCenter;
			displayNameHT[ParagraphAlignment.Justify] = XtraRichEditStringId.Caption_AlignmentJustify;
			displayNameHT[ParagraphAlignment.Left] = XtraRichEditStringId.Caption_AlignmentLeft;
			displayNameHT[ParagraphAlignment.Right] = XtraRichEditStringId.Caption_AlignmentRight;
		}
		public RepositoryItemAlignment() {
			InitItems();
		}
		#region RegisterAlignmentEdit
		public static void RegisterAlignmentEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Bitmaps256.RichTextEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(AlignmentEdit).Name, typeof(AlignmentEdit), typeof(RepositoryItemAlignment), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#endregion
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(AlignmentEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		#endregion
		protected internal virtual void InitItems() {
			BeginUpdate();
			try {
				Items.Clear();
				ParagraphAlignment[] values = (ParagraphAlignment[])Enum.GetValues(typeof(ParagraphAlignment));
				int count = values.Length;
				for (int i = 0; i < count; i++)
					Items.Add(new ImageComboBoxItem(GetDisplayName(values[i]), values[i], -1));
			}
			finally {
				CancelUpdate();
			}
		}
		static string GetDisplayName(ParagraphAlignment val) {
			return XtraRichEditLocalizer.GetString((XtraRichEditStringId)displayNameHT[val]);
		}
	}
	#endregion
	#region AlignmentEdit
#if !DEBUGTEST
	[DXToolboxItem(false), ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
#endif
	public class AlignmentEdit : ImageComboBoxEdit {
		static AlignmentEdit() {
			RepositoryItemAlignment.RegisterAlignmentEdit();
		}
		#region Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object EditValue { get { return base.EditValue; } set { base.EditValue = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ParagraphAlignment? Alignment {
			get {
				if (EditValue is ParagraphAlignment)
					return (ParagraphAlignment)EditValue;
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
		public new RepositoryItemAlignment Properties { get { return base.Properties as RepositoryItemAlignment; } }
		#endregion
	}
	#endregion
}
