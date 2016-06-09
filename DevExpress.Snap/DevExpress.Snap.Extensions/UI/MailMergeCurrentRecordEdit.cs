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

using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.Snap.Core.Commands;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Snap.Extensions.Localization;
namespace DevExpress.Snap.Extensions.UI {
	[DXToolboxItem(false)]
	public class MailMergeCurrentRecordEdit : ButtonEdit {
		[Browsable(false)]
		public override string EditorTypeName { get { return RepositoryItemMailMergeCurrentRecordEdit.InternalEditorTypeName; } }
		[Category(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemMailMergeCurrentRecordEdit Properties { get { return base.Properties as RepositoryItemMailMergeCurrentRecordEdit; } }
		protected override bool AcceptsSpace { get { return false; } }
		protected override void OnClickButton(EditorButtonObjectInfoArgs buttonInfo) {
			base.OnClickButton(buttonInfo);
			ProcessButtonClick(buttonInfo.Button);
		}
		internal MailMergeCurrentRecordCommandValue ProcessButtonClick(EditorButton button) {
			MailMergeCurrentRecordCommandValue value = EditValue as MailMergeCurrentRecordCommandValue;
			if(object.ReferenceEquals(value, null))
				return null;
			switch(button.Tag as MailMergeCurrentRecordEditorButtonTag?) {
				case MailMergeCurrentRecordEditorButtonTag.First:
					value.Value = (value.MaxValue > 0) ? 1 : 0;
					break;
				case MailMergeCurrentRecordEditorButtonTag.Prev:
					value--;
					break;
				case MailMergeCurrentRecordEditorButtonTag.Next:
					value++;
					break;
				case MailMergeCurrentRecordEditorButtonTag.Last:
					value.Value = value.MaxValue;
					break;
				default:
					return value;
			}
			EditValue = value;
			UpdateDisplayText();
			return value;
		}
	}
	[UserRepositoryItem("RegisterMailMergeCurrentRecordEdit")]
	[System.Runtime.InteropServices.ComVisible(false)]
	public class RepositoryItemMailMergeCurrentRecordEdit : RepositoryItemButtonEdit {
		const int BUTTON_WIDTH = 6;
		#region Static
		static RepositoryItemMailMergeCurrentRecordEdit() {
			RegisterMailMergeCurrentRecordEdit();
		}
		public static void RegisterMailMergeCurrentRecordEdit() {
			EditorClassInfo editorInfo = new EditorClassInfo(InternalEditorTypeName, typeof(MailMergeCurrentRecordEdit), typeof(RepositoryItemMailMergeCurrentRecordEdit), typeof(DevExpress.XtraEditors.ViewInfo.ButtonEditViewInfo), new ButtonEditPainter(), true) { AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars };
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		internal static string InternalEditorTypeName { get { return typeof(MailMergeCurrentRecordEdit).Name; } }
		#endregion
		#region Properties
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		bool ShouldSerializeButtons() { return false; }
		#endregion
		public override void CreateDefaultButton() {
			Buttons.Add(new EditorButton(MailMergeCurrentRecordEditorButtonTag.Prev, ButtonPredefines.Glyph) {
				IsLeft = true,
				ToolTip = SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.MailMergeCurrentRecordEdit_PreviousButtonTooltip),
				Image = GetImage("Prev"),
				ImageLocation = ImageLocation.MiddleLeft,
				Width = BUTTON_WIDTH
			});
			Buttons.Add(new EditorButton(MailMergeCurrentRecordEditorButtonTag.Next, ButtonPredefines.Glyph) {
				ToolTip = SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.MailMergeCurrentRecordEdit_NextButtonTooltip),
				Image = GetImage("Next"),
				ImageLocation = ImageLocation.MiddleRight,
				Width = BUTTON_WIDTH
			});
			Buttons.Add(new EditorButton(MailMergeCurrentRecordEditorButtonTag.First, ButtonPredefines.Glyph) {
				IsLeft = true,
				ToolTip = SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.MailMergeCurrentRecordEdit_FirstButtonTooltip),
				Image = GetImage("First"),
				ImageLocation = ImageLocation.MiddleLeft,
				Width = BUTTON_WIDTH
			});
			Buttons.Add(new EditorButton(MailMergeCurrentRecordEditorButtonTag.Last, ButtonPredefines.Glyph) {
				ToolTip = SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.MailMergeCurrentRecordEdit_LastButtonTooltip),
				Image = GetImage("Last"),
				ImageLocation = ImageLocation.MiddleRight,
				Width = BUTTON_WIDTH
			});
		}
		private Image GetImage(string imageName) {
			return Image.FromStream(GetType().Assembly.GetManifestResourceStream(string.Format("DevExpress.Snap.Extensions.Images.{0}_16x16.png", imageName)));
		}		
	}
	public enum MailMergeCurrentRecordEditorButtonTag { Prev, Next, First, Last }
}
