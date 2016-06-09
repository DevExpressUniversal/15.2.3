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

using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Office.Internal {
	public abstract class OfficeControl : ASPxWebControl {
		protected internal const string OfficeControlsScriptResourceName = WebScriptsResourcePath + "OfficeControls.js";
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(InsertTableControl), OfficeControlsScriptResourceName);
		}
	}
	[ToolboxItem(false)]
	public class InsertTableControl : OfficeControl {
		protected internal const string
			InsertTableControlPostfix = "ITC",
			InsertTableControlTablePostfix = "ITCT",
			InsertTableControlCaptionPostfix = "ITCC";
		protected internal const int
			DefaultRowCount = 8,
			DefaultColumnCount = 10;
		public InsertTableControl()
			: this(DefaultRowCount, DefaultColumnCount) { }
		public InsertTableControl(int rowCount, int columnCount)
			: base() {
			RowCount = rowCount;
			ColumnCount = columnCount;
		}
		protected WebControl MainDiv { get; private set; }
		protected WebControl TableDiv { get; private set; }
		protected WebControl CaptionDiv { get; private set; }
		public int RowCount {
			get { return GetIntProperty("RowCount", DefaultRowCount); }
			set { SetIntProperty("RowCount", DefaultRowCount, value); }
		}
		public int ColumnCount {
			get { return GetIntProperty("ColumnCount", DefaultColumnCount); }
			set { SetIntProperty("ColumnCount", DefaultColumnCount, value); }
		}
		public InsertTableControlClientSideEvents ClientSideEvents {
			get { return (InsertTableControlClientSideEvents)base.ClientSideEventsInternal; }
		}
		protected internal InsertTableControlStyles RenderStyles {
			get { return (InsertTableControlStyles)RenderStylesInternal; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			MainDiv = null;
		}
		protected override void CreateControlHierarchy() {
			MainDiv = RenderUtils.CreateDiv();
			MainDiv.ID = InsertTableControlPostfix;
			TableDiv = RenderUtils.CreateDiv();
			TableDiv.ID = InsertTableControlTablePostfix;
			MainDiv.Controls.Add(TableDiv);
			CaptionDiv = RenderUtils.CreateDiv();
			CaptionDiv.ID = InsertTableControlCaptionPostfix;
			MainDiv.Controls.Add(CaptionDiv);
			Controls.Add(MainDiv);
		}
		protected override void PrepareControlHierarchy() {
			GetRootElementStyle().AssignToControl(MainDiv);
			GetCaptionStyle().AssignToControl(CaptionDiv);
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.InsertTableControl";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendFormat("{0}.columnCount = '{1}';\n", localVarName, ColumnCount);
			stb.AppendFormat("{0}.rowCount = '{1}';\n", localVarName, RowCount);
			stb.AppendFormat("{0}.insertTableText = \"{1}\";", localVarName, ASPxperienceLocalizer.GetString(ASPxperienceStringId.InsertTableControl_InsertTable));
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override StylesBase CreateStyles() {
			return new InsertTableControlStyles(this);
		}
		protected internal AppearanceStyleBase GetRootElementStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetRootElementStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetCaptionStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetCaptionStyle());
			return style;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new InsertTableControlClientSideEvents();
		}
	}
	public class InsertTableControlClientSideEvents : ClientSideEvents {
		public InsertTableControlClientSideEvents()
			: base() {
		}
		public string TableInserted {
			get { return GetEventHandler("TableInserted"); }
			set { SetEventHandler("TableInserted", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("TableInserted");
		}
	}
	public class InsertTableControlStyles : StylesBase {
		const string RootElementCssClass = "dxitcControlSys";
		public InsertTableControlStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxitc";
		}
		protected internal AppearanceStyleBase GetRootElementStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CssClass = RootElementCssClass;
			style.CopyFrom(CreateStyleByName("Control"));
			return style;
		}
		protected internal AppearanceStyleBase GetCaptionStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("Caption"));
			return style;
		}
	}
	public class InsertTableTemplate : ITemplate {
		RibbonDropDownButtonItem item;
		string itemName;
		public InsertTableTemplate(RibbonDropDownButtonItem item, string itemName) {
			this.item = item;
			this.itemName = itemName;
		}
		public void InstantiateIn(Control container) {
			InsertTableControl insertTableControl = new InsertTableControl();
			insertTableControl.ID = "ITC";
			insertTableControl.ClientSideEvents.TableInserted = "function(s, e) { s.RaiseRibbonExecCommand('" + item.Ribbon.ClientID + "', '" + itemName + "', e.rowCount, e.columnCount); }";
			container.Controls.Add(insertTableControl);
		}
	}
}
