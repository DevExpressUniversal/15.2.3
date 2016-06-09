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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTab;
namespace DevExpress.ExpressApp.HtmlPropertyEditor.Win {
	[ToolboxItem(false)]
	public class ColorPopupControl : PopupControlContainer, IPopupColorEdit {
		#region Internal classes
		class InternalColorCellsControl : ColorCellsControl {
			private readonly IPopupColorEdit popupColorEdit;
			public InternalColorCellsControl(IPopupColorEdit popupColorEdit)
				: base(null) {
				this.popupColorEdit = popupColorEdit;
			}
			protected override void OnMouseUp(MouseEventArgs e) {
				if((e.Button & MouseButtons.Right) != 0 && Properties.ShowColorDialog) {
					int cellIndex = ((ColorCellsControlViewInfo)ViewInfo).GetCellIndex(new Point(e.X, e.Y));
					if(cellIndex >= ColorCellsControlViewInfo.CellColors.Length) {
						popupColorEdit.ClosePopup();
					}
				}
				base.OnMouseUp(e);
			}
		}
		class InternalPopupColorBuilder : PopupColorBuilder {
			public InternalPopupColorBuilder(IPopupColorEdit owner) : base(owner) { }
			protected override void SetTabPageProperties(int pageIndex, PopupBaseForm shadowForm) {
				if(pageIndex == 0) {
					XtraTabPage tabPage = TabControl.TabPages[pageIndex];
					tabPage.PageVisible = Properties.ShowCustomColors;
					tabPage.Text = GetLocalizedStringCore(StringId.ColorTabCustom);
					ColorCellsControl control = CreateColorCellsControl();
					tabPage.Controls.Add(control);
				}
				else {
					base.SetTabPageProperties(pageIndex, shadowForm);
				}
			}
			private ColorCellsControl CreateColorCellsControl() {
				ColorCellsControl control = new InternalColorCellsControl(Owner);
				control.Properties = Properties;
				control.Dock = DockStyle.Fill;
				control.BorderStyle = BorderStyles.NoBorder;
				if(Owner.LookAndFeel != null)
					control.LookAndFeel.Assign(Owner.LookAndFeel);
				control.Size = control.GetBestSize();
				control.EnterColor += new EnterColorEventHandler(OnEnterColor);
				return control;
			}
		}
		#endregion
		private const string ColorEditEditorTypeName = "ColorEdit";
		private readonly RepositoryItemColorEdit colorEditProperties;
		private readonly PopupColorBuilder popupColorBuilder;
		private readonly object colorChanged = new object();
		private Color resultColor;
		public ColorPopupControl() {
			EditorClassInfo editorClassInfo = EditorRegistrationInfo.Default.Editors[ColorEditEditorTypeName];
			colorEditProperties = (RepositoryItemColorEdit)editorClassInfo.CreateRepositoryItem();
			popupColorBuilder = new InternalPopupColorBuilder(this);
			resultColor = (Color)popupColorBuilder.ResultValue;
			popupColorBuilder.EmbeddedControl.Dock = DockStyle.Fill;
			Controls.Add(popupColorBuilder.EmbeddedControl);
		}
		private void RaiseColorChanged() {
			EventHandler handler = (EventHandler)Events[colorChanged];
			if(handler != null) {
				handler(this, EventArgs.Empty);
			}
		}
		public Color ResultColor {
			get { return resultColor; }
		}
		public event EventHandler ColorChanged {
			add { Events.AddHandler(colorChanged, value); }
			remove { Events.RemoveHandler(colorChanged, value); }
		}
		void IPopupColorEdit.ClosePopup() {
			HidePopup();
		}
		ColorListBox IPopupColorEdit.CreateColorListBox() {
			return new ColorListBox();
		}
		ColorEditTabControlBase IPopupColorEdit.CreateTabControl() {
			return new ColorEditTabControlBase();
		}
		void IPopupColorEdit.OnColorChanged() {
			resultColor = (Color)popupColorBuilder.ResultValue;
			RaiseColorChanged();
		}
		Color IPopupColorEdit.Color {
			get { return ResultColor; }
		}
		object IPopupColorEdit.EditValue {
			get { return ResultColor; }
		}
		bool IPopupColorEdit.IsPopupOpen {
			get {
				return true; 
			}
		}
		RepositoryItemColorEdit IPopupColorEdit.Properties {
			get { return colorEditProperties; }
		}
	}
}
