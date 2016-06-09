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

using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraRichEdit.Forms.Design {
	#region BorderLineStyleListBox
	[DXToolboxItem(false)]
	public class BorderLineStyleListBox : ImageListBoxControl, IBorderLineStyleEditor, ISupportInitialize {
		readonly BorderLineStyleEditorHelper lineStyleEditorHelper;
		DocumentModel documentModel;
		public BorderLineStyleListBox() {
			lineStyleEditorHelper = new BorderLineStyleEditorHelper(this);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BorderLineStyle BorderLineStyle {
			get {
				if (this.SelectedValue == null)
					return BorderLineStyle.None;
				return ((BorderInfo)SelectedValue).Style;
			}
			set {
				if (DocumentModel == null) {
					base.SelectedValue = null;
					return;
				}
				base.SelectedValue = DocumentModel.TableBorderInfoRepository.GetItemByLineStyle(value);
			}
		}
		public DocumentModel DocumentModel {
			get {
				return documentModel;
			}
			set {
				documentModel = value;
				OnDocumentModelChanged();
			}
		}
		protected virtual void OnDocumentModelChanged() {
			if (documentModel == null)
				return;
			PopulateItems();
		}		
		public virtual void BeginInit() {
			lineStyleEditorHelper.OnBeginInit();
		}
		public virtual void EndInit() {
			lineStyleEditorHelper.OnEndInit(true);			
		}
		protected void PopulateItems() {
			lineStyleEditorHelper.PopulateItems(true);
		}
		#region IBorderLineStyleEditor implementation
		void IBorderLineStyleEditor.Clear() {
			Items.Clear();
		}
		int IBorderLineStyleEditor.ItemsCount { get { return Items.Count; } }
		void IBorderLineStyleEditor.AddItem(string description, BorderInfo borderInfo) {
			Items.Add(new BorderLineStyleListBoxItem(description, borderInfo));
		}
		#endregion        
		protected override XtraEditors.ViewInfo.BaseStyleControlViewInfo CreateViewInfo() {
			return new BorderLineStyleImageListBoxViewInfo(this);
		}		
	}
	#endregion
	public class BorderLineStyleListBoxItem : ImageListBoxItem {
		readonly string description;
		public BorderLineStyleListBoxItem(string description, BorderInfo borderInfo) : base(borderInfo) {
			this.description = description;
		}
		public BorderInfo BorderInfo { get { return (BorderInfo)Value; } }
		public override string ToString() {
			return description;
		}
	}
	public class BorderLineStyleImageListBoxViewInfo : ImageListBoxViewInfo {
		public BorderLineStyleImageListBoxViewInfo(BaseImageListBoxControl listBox)
			: base(listBox) {
		}
		public DocumentModel DocumentModel {
			get {
				if (OwnerControl == null)
					return null;
				BorderLineStyleListBox edit = OwnerControl as BorderLineStyleListBox;
				if (edit == null)
					return null;
				return edit.DocumentModel;
			}
		}
		protected override BaseListBoxItemPainter CreateItemPainter() {
			if(IsSkinnedHighlightingEnabled)
				return new BorderLineStyleListBoxSkinItemPainter();
			return new BorderLineStyleListBoxItemPainter();		
		}
		 protected internal override int CalcItemMinHeight() {
			int height = 0;
			if (DocumentModel != null) {
				DocumentModelUnitConverter unitConverter = DocumentModel.UnitConverter;
				height = unitConverter.ModelUnitsToPixels(unitConverter.PointsToModelUnits(20));
			}
			return Math.Max(base.CalcItemMinHeight(), height);
		}
		 protected override ListBoxItemObjectInfoArgs CreateListBoxItemInfoArgs() {
			 return new BorderLineStyleValueListBoxItemObjectInfoArgs(this, null, Rectangle.Empty);
		 }
	}
	public class BorderLineStyleValueListBoxItemObjectInfoArgs : ListBoxItemObjectInfoArgs {
		BorderInfo borderInfo;
		public BorderLineStyleValueListBoxItemObjectInfoArgs(BaseListBoxViewInfo viewInfo, GraphicsCache cache, Rectangle bounds)
			: base(viewInfo, cache, bounds) {
		}
		public BorderInfo BorderInfo { get { return borderInfo; } }
		public override void AssignFromItemInfo(BaseListBoxViewInfo.ItemInfo item) {
			base.AssignFromItemInfo(item);
			ComboBoxItem comboboxItem = item.Item as ComboBoxItem;
			if (comboboxItem != null)
				borderInfo = comboboxItem.Value as BorderInfo;
			else
				this.borderInfo = item.Item as BorderInfo;
		}
	}
	public class BorderLineStyleListBoxSkinItemPainter : ListBoxSkinItemPainter {
		protected override void DrawItemText(ListBoxItemObjectInfoArgs e) {
			BorderLineStyleImageListBoxViewInfo viewInfo = e.ViewInfo as BorderLineStyleImageListBoxViewInfo;
			if (viewInfo == null || viewInfo.DocumentModel == null)
				return;
			BorderLinePainterHelper.DrawBorderLineItem(e, base.DrawItemText, viewInfo.DocumentModel.UnitConverter);
		}
	}
	public class BorderLineStyleListBoxItemPainter : ListBoxItemPainter {
		protected override void DrawItemText(ListBoxItemObjectInfoArgs e) {
			BorderLineStyleImageListBoxViewInfo viewInfo = e.ViewInfo as BorderLineStyleImageListBoxViewInfo;
			if (viewInfo == null || viewInfo.DocumentModel == null)
				return;
			BorderLinePainterHelper.DrawBorderLineItem(e, base.DrawItemText, viewInfo.DocumentModel.UnitConverter);
		}
	}
}
