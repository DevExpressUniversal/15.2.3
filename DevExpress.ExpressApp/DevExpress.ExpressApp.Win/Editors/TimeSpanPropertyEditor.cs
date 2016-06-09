#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Accessibility;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.ExpressApp.Win.Editors {
	public class TimeSpanPropertyEditor : DXPropertyEditor {
		protected override object CreateControlCore() {
			return new TimeSpanEdit();
		}
		protected override RepositoryItem CreateRepositoryItem() {
			return new RepositoryItemTimeSpanEdit();
		}
		protected override void SetupRepositoryItem(RepositoryItem item) {
			base.SetupRepositoryItem(item);
			((RepositoryItemTimeSpanEdit)item).Init(EditMask, EditMaskType);
		}
		public TimeSpanPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class TimeSpanEdit : TextEdit {
		static TimeSpanEdit() {
			RepositoryItemTimeSpanEdit.Register();
		}
		protected override Object ExtractParsedValue(ConvertEditValueEventArgs e) {
			if(e.Handled) {
				return e.Value;
			}
			else {
				if(e.Value is TimeSpan) {
					return e.Value;
				}
				else if(e.Value is String) {
					try {
						return TimeSpan.Parse((String)e.Value);
					}
					catch {
						return TimeSpan.Zero;
					}
				}
				else if(e.Value is DateTime) {
					return ((DateTime)(e.Value)).TimeOfDay;
				}
				else {
					return TimeSpan.Zero;
				}
			}
		}
		public TimeSpanEdit() {
		}
		public TimeSpan Value {
			get {
				ConvertEditValueEventArgs args = new ConvertEditValueEventArgs();
				args.Value = EditValue;
				return (TimeSpan)ExtractParsedValue(args);
			}
		}
		public override String EditorTypeName {
			get { return RepositoryItemTimeSpanEdit.EditorName; }
		}
	}
	public class RepositoryItemTimeSpanEdit : RepositoryItemTextEdit {
		protected internal const String EditorName = "XafTimeSpanEdit";
		protected internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(
					new EditorClassInfo(EditorName, typeof(TimeSpanEdit), typeof(RepositoryItemTimeSpanEdit), typeof(TextEditViewInfo),
					new TextEditPainter(), true, EditImageIndexes.TextEdit, typeof(TextEditAccessible)));
			}
		}
		public override string GetDisplayText(DevExpress.Utils.FormatInfo format, object editValue) {
			String result = base.GetDisplayText(format, editValue);
			if (editValue != null && Mask.MaskType == MaskType.RegEx && Mask.EditMask == FormattingProvider.GetEditMask(typeof(TimeSpan))) {
				if(editValue is TimeSpan && ((TimeSpan)editValue).Milliseconds != 0) {
					result = result.TrimEnd('0');
				}
			}
			return result;		   
		}
		static RepositoryItemTimeSpanEdit() {
			RepositoryItemTimeSpanEdit.Register();
		}
		public RepositoryItemTimeSpanEdit() {
			Mask.MaskType = MaskType.None;
			if(Mask.MaskType != MaskType.RegEx) {
				Mask.UseMaskAsDisplayFormat = true;
			}
		}
		public void Init(String editMask, EditMaskType maskType) {
			if(!String.IsNullOrEmpty(editMask)) {
				Mask.EditMask = editMask;
				switch(maskType) {
					case EditMaskType.DateTime:
						Mask.MaskType = MaskType.DateTime;
						break;
					default:
						Mask.UseMaskAsDisplayFormat = false;
						Mask.MaskType = MaskType.RegEx;
						break;
				}
			}
		}
		public override String EditorTypeName {
			get { return EditorName; }
		}
	}
}
