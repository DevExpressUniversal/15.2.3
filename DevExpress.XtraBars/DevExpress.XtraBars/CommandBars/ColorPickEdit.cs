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
using System.Drawing;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Office.UI.Internal;
using DevExpress.XtraEditors.Popup;
namespace DevExpress.Office.UI {
	#region RepositoryItemOfficeColorPickEditBase (abstract class)
	public abstract class RepositoryItemOfficeColorPickEditBase : RepositoryItemColorPickEdit, IRepositoryItemColorEdit {
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowCustomColors { get { return true; } set { base.ShowCustomColors = true; } }
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowSystemColors { get { return false; } set { base.ShowSystemColors = false; } }
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowWebColors { get { return false; } set { base.ShowWebColors = false; } }
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HorzAlignment ColorAlignment { get { return HorzAlignment.Center; } set { base.ColorAlignment = HorzAlignment.Center; } }
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorzAlignment DefaultAlignment { get { return HorzAlignment.Center; } }
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color AutomaticColor { get { return Color.Empty; } set { } }
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string AutomaticColorButtonCaption { get { return base.AutomaticColorButtonCaption; } set { base.AutomaticColorButtonCaption = value; } }
		public override BaseEditViewInfo CreateViewInfo() {
			return new OfficeColorEditViewInfo(this);
		}
		protected override object ConvertToEditValue(object val) {
			return (val == null) ? null : base.ConvertToEditValue(val);
		}
		Color IRepositoryItemColorEdit.ConvertToColor(object editValue) {
			return this.ConvertToColor(editValue);
		}
		protected override ColorDialogOptions CreateColorDialogOptions() {
			return new OfficeColorDialogOptions();
		}
	}
	#endregion
	#region OfficeColorDialogOptions
	class OfficeColorDialogOptions : ColorDialogOptions {
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowTransparency { get { return false; } set { } }
	}
	#endregion
	#region RepositoryItemOfficeColorPickEdit
	[
	UserRepositoryItem("RegisterRepositoryItemColorPickEditEx"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemOfficeColorPickEdit : RepositoryItemOfficeColorPickEditBase {
		internal static string InternalEditorTypeName { get { return typeof(OfficeColorPickEdit).Name; } }
		static RepositoryItemOfficeColorPickEdit() { RegisterRepositoryItemColorPickEditEx(); }
		public static void RegisterRepositoryItemColorPickEditEx() {
			EditorClassInfo editorInfo = new EditorClassInfo(InternalEditorTypeName, typeof(OfficeColorPickEdit), typeof(RepositoryItemOfficeColorPickEdit), typeof(OfficeColorEditViewInfo), new ColorEditPainter(), true);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
	}
	#endregion
	#region OfficeColorPickEdit
	[ToolboxItem(false)]
	public class OfficeColorPickEdit : ColorPickEdit {
		static OfficeColorPickEdit() {
			RepositoryItemOfficeColorPickEdit.RegisterRepositoryItemColorPickEditEx();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemOfficeColorPickEdit Properties { get { return ((RepositoryItemColorEdit)base.Properties)as RepositoryItemOfficeColorPickEdit; } }
		public override string EditorTypeName { get { return GetType().Name; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object EditValue { get { return base.EditValue; } set { base.EditValue = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Color? Value { get { return EditValue as Color?; } set { EditValue = value; } }
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Design.Internal {
	public class OfficeColorPickEditPopupColorBuilder : PopupColorBuilderEx {
		public OfficeColorPickEditPopupColorBuilder(IPopupColorEdit owner)
			: base(owner) {
		}
		protected override bool ShouldDrawControlBorder { get { return false; } }
		protected override InnerColorPickControl CreateCustomTabInnerControlInstance() {
			return new OfficeInnerColorPickControl();
		}
	}
	internal class OfficeInnerColorPickControl : InnerColorPickControl {
		protected override void SetSelectedColor(Color color) {
			if (SelectedColor != color)
				base.SetSelectedColor(color);
			else
				OnSelectedColorChanged(new InnerColorPickControlSelectedColorChangedEventArgs(new ColorItem(SelectedColor), new ColorItem(SelectedColor)));
		}
	}
}
