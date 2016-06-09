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
using System.ComponentModel.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using System.Collections;
using System.Resources;
namespace DevExpress.Web.Design {
	public class ASPxEditDesignerBase : ASPxDataWebControlDesigner {
		private ASPxEditBase editBase = null;
		public ASPxEditBase EditBase {
			get { return editBase; }
		}
		public override void Initialize(IComponent component) {
			this.editBase = (ASPxEditBase)component;
			base.Initialize(component);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new EditDesignerActionListBase(this);
		}
		public override void ShowAbout() {
			EditorsAboutDialogHelper.ShowAbout(Component.Site);
		}
		protected override string GetEmptyDesignTimeHtmlInternal() {
			return "[" + EditBase.ID + "]";
		}
		protected string GetEmptyDesignTimeHtmlInternalBase() {
			return base.GetEmptyDesignTimeHtmlInternal();
		}
	}
	public class ASPxEditDesigner : ASPxEditDesignerBase {
		public const string ValueTypeProprtyName = "ValueType";
		private ASPxEdit edit = null;
		public virtual bool ShowValueTypeProperty {
			get { return false; }
		}		
		public string ValueType {
			get {
				PropertyDescriptor valueTypeProperty = GetEditorValueTypeProprerty();
				return valueTypeProperty != null ? valueTypeProperty.GetValue(Edit).ToString() : string.Empty;
			}
			set {
				PropertyDescriptor valueTypeProperty = GetEditorValueTypeProprerty();
				if(valueTypeProperty != null)
					valueTypeProperty.SetValue(Edit, Type.GetType(value));
			}
		}
		protected ASPxEdit Edit {
			get { return this.edit; }
		}
		protected PropertyDescriptor GetEditorValueTypeProprerty() {
			return TypeDescriptor.GetProperties(Edit.GetType())[ValueTypeProprtyName];
		}
		protected internal bool ShowErrorFrame {
			get { 
				return Edit.ShowErrorFrame; 
			}
			set {
				Edit.ShowErrorFrame = value;
				UpdateDesignTimeHtml();
				TypeDescriptor.Refresh(Component);
			}
		}
		public override void Initialize(IComponent component) {
			this.edit = (ASPxEdit)component;
			base.Initialize(component);
		}
		protected override Control CreateViewControl() {
			Control view = base.CreateViewControl();
			(view as ASPxEdit).ShowErrorFrame = Edit.ShowErrorFrame;
			return view;
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new EditDesignerActionList(this);
		}
	}
	public class EditDesignerActionListBase : ASPxWebControlDesignerActionList {
		public EditDesignerActionListBase(ASPxEditDesignerBase designer)
			: base(designer) {
		}
	}
	public class EditDesignerActionList : EditDesignerActionListBase {
		private ASPxEditDesigner editDesigner = null;
		public enum ValueTypeEnum { Boolean, Int16, Int32, String, Char, Byte, Guid }
		public EditDesignerActionList(ASPxEditDesigner designer)
			: base(designer) {
			this.editDesigner = designer;
		}
		public bool ShowErrorFrame {
			get { return EditDesigner.ShowErrorFrame; }
			set { EditDesigner.ShowErrorFrame = value; }
		}
		public ValueTypeEnum ValueType {
			get { return (ValueTypeEnum)Enum.Parse(typeof(ValueTypeEnum), EditDesigner.ValueType.Replace("System.", string.Empty)); }
			set { EditDesigner.ValueType = string.Format("System.{0}", value); }
		}
		protected ASPxEditDesigner EditDesigner {
			get { return editDesigner; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			if(EditDesigner.ShowValueTypeProperty)
				collection.Add(new DesignerActionPropertyItem(ASPxEditDesigner.ValueTypeProprtyName, ASPxEditDesigner.ValueTypeProprtyName));
			collection.Add(new DesignerActionPropertyItem("ShowErrorFrame", "Preview Error Frame"));
			return collection;
		}
	}
}
