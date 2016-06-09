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
using System.ComponentModel.Design;
using System.Collections.Generic;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxDateEditDesigner : ASPxButtonEditDesigner {
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new DateEditDesignerActionList(this);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			if(propertyNameToCaptionMap.ContainsKey("Buttons"))
				propertyNameToCaptionMap.Remove("Buttons");
		}
	}
	public class DateEditDesignerActionList : ButtonEditDesignerActionList {
		public DateEditDesignerActionList(ASPxDateEditDesigner designer) 
			: base(designer) {
		}
		protected ASPxDateEdit DateEdit {
			get { return EditDesigner.Component as ASPxDateEdit; }
		}
		public bool EnableFastNavigation {
			get { return DateEdit.CalendarProperties.FastNavProperties.Enabled; }
			set {
				DateEdit.CalendarProperties.FastNavProperties.Enabled = value;
				DateEdit.PropertyChanged("CalendarProperties");
			}
		}
		public EditFormat EditFormat {
			get { return DateEdit.EditFormat; }
			set {
				DateEdit.EditFormat = value;
				DateEdit.PropertyChanged("EditFormat");
			}
		}
		public DateTime Date {
			get { return DateEdit.Date; }
			set {
				DateEdit.Date = value;
				DateEdit.PropertyChanged("Date");
			}
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("EnableFastNavigation", "Enable Fast Calendar Navigation"));
			collection.Add(new DesignerActionPropertyItem("Date", "Date"));
			collection.Add(new DesignerActionPropertyItem("EditFormat", "Format"));
			return collection;
		}
		protected override string GetClearButtonHint() {
			return StringResources.ASPxButtonEditActionList_ClearButtonDisplayModeAllowNullHint;
		}
	}
}
