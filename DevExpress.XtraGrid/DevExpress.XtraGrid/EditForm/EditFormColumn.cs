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

using System.ComponentModel;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
namespace DevExpress.XtraGrid.EditForm {
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverter))]
	public class EditFormColumn {
		int visibleIndex = 0, globalVisibleIndex;
		int columnSpan = 1, rowSpan = 1;
		bool startNewRow = false;
		internal bool columnSpanChanged = false, rowSpanChanged = false, captionChanged = false;
		EditFormColumnCaptionLocation captionLocation = EditFormColumnCaptionLocation.Default;
		DefaultBoolean visible = DefaultBoolean.Default;
		string caption = "";
		bool useEditorColRowSpan = true;
		[DefaultValue("")]
		public string Caption {
			get { return caption; }
			set {
				if(Caption == value) return;
				caption = value; 
				captionChanged = true;
			}
		}
		[DefaultValue(true)]
		public bool UseEditorColRowSpan {
			get { return useEditorColRowSpan; }
			set { useEditorColRowSpan = value; }
		}
		[DefaultValue(false)]
		public bool StartNewRow {
			get { return startNewRow; }
			set { startNewRow = value; }
		}
		internal void ResetChanged() {
			this.rowSpanChanged = this.columnSpanChanged = this.captionChanged = false;
		}
		public string FieldName { get; internal set; }
		[Browsable(false)]
		public int GlobalVisibleIndex {
			get { return globalVisibleIndex; }
			set {
				if(value < 0) value = -1;
				globalVisibleIndex = value;
			}
		}
		[DefaultValue(0)]
		public int VisibleIndex {
			get { return visibleIndex; }
			set {
				if(value < 0) value = -1;
				visibleIndex = value;
			}
		}
		[DefaultValue(EditFormColumnCaptionLocation.Default)]
		public EditFormColumnCaptionLocation CaptionLocation {
			get { return captionLocation; }
			set { captionLocation = value; }
		}
		[DefaultValue(1)]
		public int ColumnSpan {
			get { return columnSpan; }
			set {
				if(value < 1) value = 1;
				if(ColumnSpan == value) return;
				columnSpan = value;
				columnSpanChanged = true;
			}
		}
		[DefaultValue(1)]
		public int RowSpan {
			get { return rowSpan; }
			set {
				if(value < 1) value = 1;
				if(RowSpan == value) return;
				rowSpan = value;
				rowSpanChanged = true;
			}
		}
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean Visible {
			get { return visible; }
			set { visible = value; }
		}
		[Browsable(false)]
		public bool ColumnVisible { get; set; }
		[Browsable(false)]
		public bool ReadOnly { get; set; }
		[Browsable(false)]
		public XtraEditors.Repository.RepositoryItem RepositoryItem { get; set; }
		public override string ToString() {
			return "";
		}
	}
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
	[Editor("DevExpress.XtraGrid.Design.EditFormColumnCollectionEditor, " + AssemblyInfo.SRAssemblyGridDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class EditFormColumnCollection : DXCollectionBase<EditFormColumn> {
		public override string ToString() {
			return "";
		}
	}
	public class EditFormOwner {
		EditFormColumnCollection columns;
		int editFormColumnCount = 3;
		UserLookAndFeel elementsLookAndFeel;
		public EditFormOwner() {
			this.columns = new EditFormColumnCollection();
		}
		[DefaultValue(3)]
		public int EditFormColumnCount {
			get { return editFormColumnCount; }
			set {
				if(value < 1) value = 1;
				if(value > 20) value = 20;
				editFormColumnCount = value;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditFormColumnCollection Columns { get { return columns; } }
		[Browsable(false)]
		public UserLookAndFeel ElementsLookAndFeel {
			get {
				if(elementsLookAndFeel == null) elementsLookAndFeel = UserLookAndFeel.Default;
				return elementsLookAndFeel;
			}
			set { elementsLookAndFeel = value; }
		}
		[DefaultValue(false)]
		public bool AllowHtmlCaptions { get; set; }
		[DefaultValue(null)]
		public Utils.Menu.IDXMenuManager MenuManager { get; set; }
	}
	public enum EditFormColumnCaptionLocation { Default, Near, Top, None };
}
