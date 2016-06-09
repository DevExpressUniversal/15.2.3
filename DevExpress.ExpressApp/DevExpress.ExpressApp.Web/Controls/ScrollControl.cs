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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Web.Templates;
namespace DevExpress.ExpressApp.Web.Controls {
	public class ScrollPositionChangedEventArgs : EventArgs {
		private Point currentScrollPosition;
		public Point CurrentScrollPosition {
			get { return currentScrollPosition; }
			set { currentScrollPosition = value; }
		}
		public ScrollPositionChangedEventArgs(Point currentScrollPosition) {
			this.currentScrollPosition = currentScrollPosition;
		}
	}
	[ToolboxItem(false)]
	public class ScrollControl : WebControl, INamingContainer, ITestable {
		public const string xPositionFieldId = "xPos";
		public const string yPositionFieldId = "yPos";
		private BaseXafPage page;
		private HiddenField currentScrollXPositionHiddenField;
		private HiddenField currentScrollYPositionHiddenField;
		private HiddenField CreateHiddenFieldValueHolder(string holderId) {
			HiddenField valueHolder = new HiddenField();
			valueHolder.ID = holderId;
			valueHolder.ValueChanged += new EventHandler(CurrentScrollPositionChanged);
			return valueHolder;
		}
		private void CurrentScrollPositionChanged(object sender, EventArgs e) {
			OnScrollPositionChanged(this, new ScrollPositionChangedEventArgs(CurrentScrollPosition));
		}
		private void OnScrollPositionChanged(object sender, ScrollPositionChangedEventArgs e) {
			if(ScrollPositionChanged != null) {
				ScrollPositionChanged(sender, e);
			}
		}
		private void Unsubscribe() {
			currentScrollXPositionHiddenField.ValueChanged -= new EventHandler(CurrentScrollPositionChanged);
			currentScrollYPositionHiddenField.ValueChanged -= new EventHandler(CurrentScrollPositionChanged);
		}
		protected override void OnUnload(EventArgs e) {
			OnControlInitialized(this);
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(WebWindow.CurrentRequestWindow != null) {
				string clientScript = String.Format("scrollControlOnLoadCore('{0}', '{1}', '{2}');", currentScrollXPositionHiddenField.ClientID, currentScrollYPositionHiddenField.ClientID, page != null ? page.GetScrollableControlID() : "");
				WebWindow.CurrentRequestWindow.RegisterStartupScript("ScrollControl", clientScript);
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Point CurrentScrollPosition {
			get {
				int x, y;
				int.TryParse(currentScrollXPositionHiddenField.Value, out x);
				int.TryParse(currentScrollYPositionHiddenField.Value, out y);
				return new Point(x, y);
			}
			set {
				currentScrollXPositionHiddenField.Value = value.X.ToString();
				currentScrollYPositionHiddenField.Value = value.Y.ToString();
			}
		}
		public ScrollControl(Page page) {
			this.page = page as BaseXafPage;
			currentScrollXPositionHiddenField = CreateHiddenFieldValueHolder(xPositionFieldId);
			currentScrollYPositionHiddenField = CreateHiddenFieldValueHolder(yPositionFieldId);
			XafUpdatePanel updatePanel = new XafUpdatePanel();
			updatePanel.ID = "UPSPH";
			updatePanel.UpdatePanelForASPxGridListCallback = false;
			updatePanel.Controls.Add(currentScrollXPositionHiddenField);
			updatePanel.Controls.Add(currentScrollYPositionHiddenField);
			Controls.Add(updatePanel);
		}
		public override void Dispose() {
			Unsubscribe();
			base.Dispose();
		}
		public event EventHandler<ScrollPositionChangedEventArgs> ScrollPositionChanged;
		#region ITestable Members
		string ITestable.TestCaption {
			get { return "Scroll Control"; }
		}
		string ITestable.ClientId {
			get { return ID; }
		}
		IJScriptTestControl ITestable.TestControl {
			get {
				return new ScrollControlTestControl();
			}
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public virtual TestControlType TestControlType {
			get {
				return TestControlType.Field;
			}
		}
		#endregion
	}
	public class ScrollControlTestControl : IJScriptTestControl {
		public const string xySeparator = "_";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return "ScrollControlTestControl"; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.GetTextFunctionBody = @"
					return '' + scrollableControl.scrollLeft + '" + xySeparator + @"' + scrollableControl.scrollTop;
				";
				result.SetTextFunctionBody = @"
						var pos = value.split('" + xySeparator + @"');
						scrollXPositionHolder.value = pos[0];
						scrollYPositionHolder.value = pos[1];
				";
				result.ActFunctionBody = @"
						var x = parseInt(scrollXPositionHolder.value);
						var y = parseInt(scrollYPositionHolder.value);
						scrollableControl.scrollLeft = x;
                        scrollableControl.scrollTop = y;
				";
				return result;
			}
		}
		#endregion
	}
}
