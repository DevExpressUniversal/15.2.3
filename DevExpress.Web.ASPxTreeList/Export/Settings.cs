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
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web;
using SystemPrinting = System.Drawing.Printing;
namespace DevExpress.Web.ASPxTreeList {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ASPxTreeListPrintSettings : StateManager {
		PageSettings pageSettings;
		public ASPxTreeListPrintSettings() {
			this.pageSettings = null;
		}
		[
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PageSettings PageSettings {
			get {
				if(pageSettings == null)
					pageSettings = new PageSettings();
				return pageSettings;
			}
		}
		[
		NotifyParentProperty(true), DefaultValue(false)]
		public bool ShowTreeButtons {
			get { return GetBoolProperty("PrintTreeButtons", false); }
			set { SetBoolProperty("PrintTreeButtons", false, value); }
		}
		[
		NotifyParentProperty(true), DefaultValue(true)]
		public bool ShowTreeLines {
			get { return GetBoolProperty("PrintTreeLines", true); }
			set { SetBoolProperty("PrintTreeLines", true, value); }
		}
		[
		NotifyParentProperty(true), DefaultValue(false)]
		public bool AutoWidth {
			get { return GetBoolProperty("AutoWidth", false); }
			set { SetBoolProperty("AutoWidth", false, value); }
		}
		[
		NotifyParentProperty(true), DefaultValue(false)]
		public bool ExpandAllNodes {
			get { return GetBoolProperty("ExpandAllNodes", false); }
			set { SetBoolProperty("ExpandAllNodes", false, value); }
		}
		[
		NotifyParentProperty(true), DefaultValue(false)]
		public bool ExportAllPages {
			get { return GetBoolProperty("ExportAllPages", false); }
			set { SetBoolProperty("ExportAllPages", false, value); }
		}
		public virtual void Assign(ASPxTreeListPrintSettings source) {
			ASPxTreeListPrintSettings src = source as ASPxTreeListPrintSettings;
			if(src != null) {
				PageSettings.Assign(src.PageSettings);
				ShowTreeButtons = src.ShowTreeButtons;
				ShowTreeLines = src.ShowTreeLines;
				AutoWidth = src.AutoWidth;
				ExpandAllNodes = src.ExpandAllNodes;
				ExportAllPages = src.ExportAllPages;
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class PageSettings : StateManager {
		PageMargins margins;
		public PageSettings() {
			this.margins = null;
		}
		[
		DefaultValue(false), NotifyParentProperty(true)]
		public bool Landscape {
			get { return GetBoolProperty("Landscape", false); }
			set { SetBoolProperty("Landscape", false, value); }
		}
		[
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PageMargins Margins {
			get {
				if(margins == null)
					margins = new PageMargins();
				return margins;
			}
		}
		[
		DefaultValue(SystemPrinting.PaperKind.Letter), NotifyParentProperty(true)]
		public SystemPrinting.PaperKind PaperKind {
			get { return (SystemPrinting.PaperKind)GetObjectProperty("PaperKind", SystemPrinting.PaperKind.Letter); }
			set { SetObjectProperty("PaperKind", SystemPrinting.PaperKind.Letter, value); }
		}
		[
		DefaultValue(""), NotifyParentProperty(true)]
		public string PaperName {
			get { return GetStringProperty("PaperName", ""); }
			set { SetStringProperty("PaperName", "", value); }
		}
		protected internal void Assign(PageSettings source) {
			PageSettings src = source as PageSettings;
			if(src != null) {
				Landscape = src.Landscape;
				Margins.Assign(src.Margins);
				PaperKind = src.PaperKind;
				PaperName = src.PaperName;
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class PageMargins : StateManager {
		[
		DefaultValue((int)100), NotifyParentProperty(true)]
		public int Left {
			get { return GetIntProperty("Left", 100); }
			set {
				if(value < 0) value = 0;
				SetIntProperty("Left", 100, value);
			}
		}
		[
		DefaultValue((int)100), NotifyParentProperty(true)]
		public int Right {
			get { return GetIntProperty("Right", 100); }
			set {
				if(value < 0) value = 0;
				SetIntProperty("Right", 100, value);
			}
		}
		[
		DefaultValue((int)100), NotifyParentProperty(true)]
		public int Top {
			get { return GetIntProperty("Top", 100); }
			set {
				if(value < 0) value = 0;
				SetIntProperty("Top", 100, value);
			}
		}
		[
		DefaultValue((int)100), NotifyParentProperty(true)]
		public int Bottom {
			get { return GetIntProperty("Bottom", 100); }
			set {
				if(value < 0) value = 0;
				SetIntProperty("Bottom", 100, value);
			}
		}
		protected internal SystemPrinting.Margins ToPrintingMargins() {
			SystemPrinting.Margins margins = new SystemPrinting.Margins();
			margins.Top = Top;
			margins.Bottom = Bottom;
			margins.Left = Left;
			margins.Right = Right;
			return margins;
		}
		protected internal void Assign(PageMargins source) {
			PageMargins src = source as PageMargins;
			if(src != null) {
				Left = src.Left;
				Right = src.Right;
				Top = src.Top;
				Bottom = src.Bottom;
			}
		}
	}
}
