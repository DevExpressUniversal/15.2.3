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
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.TabControl {
	[ToolboxItem(false)]
	public class TabControlWithSample : ASPxTabControl {
		protected SampleTab sampleTab = null;
		protected internal const string ScriptName = ASPxSpreadsheet.SpreadsheetTabControlScriptResourceName;
		protected internal const string TabControlEtalonTabsContainerID = "ETC";
		public TabControlWithSample(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) {
			Spreadsheet = spreadsheet;
		}
		protected ASPxSpreadsheet Spreadsheet { get; private set; }
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(TabControlWithSample), ScriptName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientSpreadsheetTabControl";
		}
		protected override TabControlLite CreateTabControl() {
			return DesignMode ? new TabControlLiteDesignMode(this) : (TabControlLite)(new TabControlLiteWithSample(this));
		}
		protected new internal TabControlStyles Styles {
			get { return base.Styles; }
		}
		public SampleTab SampleTabElement {
			get {
				if(sampleTab == null) {
					sampleTab = CreateEtalonTab();
				}
				return sampleTab;
			}
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			base.AddHoverItems(helper);
			helper.AddStyles(GetTabStyles(GetTabHoverCssStyle(SampleTabElement, true)), GetTabElementID(SampleTabElement, true), TabIdPostfixes,
					GetTabImageObjects(GetTabImage(SampleTabElement, true).GetHottrackedScriptObject(Page)), ImageIdPostfixes, GetTabEnabled(SampleTabElement));
			helper.AddStyles(GetTabStyles(GetTabHoverCssStyle(SampleTabElement, false)), GetTabElementID(SampleTabElement, false), TabIdPostfixes,
					GetTabImageObjects(GetTabImage(SampleTabElement, false).GetHottrackedScriptObject(Page)), ImageIdPostfixes, GetTabEnabled(SampleTabElement));
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			base.AddDisabledItems(helper);
			helper.AddStyles(GetTabStyles(GetTabDisabledCssStyle(SampleTabElement, true)), GetTabElementID(SampleTabElement, true), TabIdPostfixes,
			   GetTabImageObjects(GetTabImage(SampleTabElement, true).GetDisabledScriptObject(Page)), ImageIdPostfixes, GetTabEnabled(SampleTabElement));
			helper.AddStyles(GetTabStyles(GetTabDisabledCssStyle(SampleTabElement, false)), GetTabElementID(SampleTabElement, false), TabIdPostfixes,
			   GetTabImageObjects(GetTabImage(SampleTabElement, false).GetDisabledScriptObject(Page)), ImageIdPostfixes, GetTabEnabled(SampleTabElement));
		}
		protected SampleTab CreateEtalonTab() {
			SampleTab sampleTabElement = new SampleTab(this);
			sampleTabElement.Text = "emptyText";
			sampleTabElement.Name = "emptyName";
			sampleTabElement.ClientEnabled = true;
			sampleTabElement.ClientVisible = true;
			sampleTabElement.Enabled = true;
			sampleTabElement.Visible = true;
			return sampleTabElement;
		}
	}
	[ToolboxItem(false)]
	public class TabControlLiteWithSample : TabControlLite {
		public TabControlLiteWithSample(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		protected TabStripWrapperControlLite CreateTabEtalonStripWrapper() {
			return new TabStripWithSampleWrapperControlLite(TabControl);
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			Controls.Add(CreateTabEtalonStripWrapper());
		}
	}
	[ToolboxItem(false)]
	public class TabStripWithSampleWrapperControlLite : TabStripWrapperControlLite {
		public TabStripWithSampleWrapperControlLite(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		protected SampleTabStripControlLite CreateEtalonTabs() {
			return new SampleTabStripControlLite(TabControl);
		}
		protected override void CreateControlHierarchy() {
			ID = TabControlWithSample.TabControlEtalonTabsContainerID;
			AddChild(CreateEtalonTabs());
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AppendDefaultDXClassName(this, SpreadsheetStyles.TabControlSampleStrip);
			RenderUtils.AppendDefaultDXClassName(this, SpreadsheetStyles.TabControlCssClass);
		}
	}
	[ToolboxItem(false)]
	public class SampleTabStripControlLite : TabStripControlLiteBase {
		public SampleTabStripControlLite(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		public new TabControlWithSample TabControl {
			get { return (TabControlWithSample)base.TabControl; }
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			SampleTab tab = TabControl.SampleTabElement;
			AddChild(new TabItemSpacerControlLite(tab));
			CreateTabItemControl(tab, false);
			CreateTabItemControl(tab, true);
			AddChild(new LineBrakeControlLite());
		}
	}
	[ToolboxItem(false)]
	public class SampleTab : Tab {
		protected ASPxTabControlBase spreadsheetTabControl;
		public SampleTab(ASPxTabControlBase spreadsheetTabControl)
			: base() {
			this.spreadsheetTabControl = spreadsheetTabControl;
		}
		public override ASPxTabControlBase TabControl {
			get { return this.spreadsheetTabControl; }
		}
		public override int Index {
			get {
				return -1;
			}
		}
	}
}
