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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
namespace DevExpress.ExpressApp.Design {
	public partial class DesignErrorControl : UserControl {
		private const string DefaultHeader = "Cannot load the Designer";
		private const string DefaultMessage = "The current and related projects should be built before the Designer can be loaded.\r\nUse the link below to build the projects and reload the Designer.";
		private Icon icon;
		private bool hasCallStack = false;
		private bool callStackVisible = false;
		private void SetHeight(TextBox control) {
			Graphics g = control.CreateGraphics();
			SizeF size = g.MeasureString(control.Text, control.Font, control.Width);
			control.Height = (int)size.Height + 1;
			if(control.MaximumSize.Height != 0 && size.Height > control.MaximumSize.Height) {
				control.ScrollBars = ScrollBars.Vertical;
			}
			else {
				control.ScrollBars = ScrollBars.None;
			}
		}
		private void RecalculateLayout() {
			this.mainPanel.Width = (int)(this.Width * 0.75);
			CalculateCallStackPanelHeight();
			SetHeight(errorMessageTextBox);
			this.mainPanel.Height = headerTextBox.Height + errorMessageTextBox.Height + callStackPanel.Height + controlsPanel.Height;
			this.mainPanel.Left = this.Width / 2 - this.mainPanel.Width / 2;
			this.mainPanel.Top = this.Height / 2 - this.mainPanel.Height / 2;
		}
		private void CalculateCallStackPanelHeight() {
			if(!hasCallStack) {
				callStackPanel.Height = 0;
			}
			else {
				if(callStackVisible) {
					SetHeight(callStackTextBox);
				}
				else {
					callStackTextBox.Height = 0;
				}
				callStackPanel.Height = showCallStackLinkPanel.Height + callStackTextBox.Height;
			}
		}
		private void iconPanel_Paint(object sender, PaintEventArgs e) {
			using(GraphicsCache gcache = new GraphicsCache(e)) {
				gcache.Graphics.DrawIcon(icon, 0, 0);
			}
		}
		private void showCallStackLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			callStackVisible = hasCallStack && !callStackVisible;
			if(callStackVisible) {
				showCallStackLink.Text = @"Hide details";
			}
			else {
				showCallStackLink.Text = @"Details";
			}
			RecalculateLayout();
		}
		private void buildProjectLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			if(BuildProjectClicked != null) {
				BuildProjectClicked(this, new EventArgs());
			}
		}
		private void showGeneratedCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			if(ShowGeneratedCodeClicked != null) {
				ShowGeneratedCodeClicked(this, EventArgs.Empty);
			}
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			RecalculateLayout();
		}
		public DesignErrorControl() {
			InitializeComponent();
			icon = SystemIcons.Warning;
			iconPanel.Width = icon.Width + 10;
			headerTextBox.Text = DefaultHeader;
			errorMessageTextBox.Text = DefaultMessage;
			Dock = DockStyle.Fill;
		}
		public void SetErrorMessage(Exception e, string headerText, bool showBuildLink, string buildProjectLinkText) {
			if(e.Message != string.Empty) {
				errorMessageTextBox.Text = e.Message;
			}
			else {
				errorMessageTextBox.Text = DefaultMessage;
			}
			callStackTextBox.Text = e.StackTrace;
			hasCallStack = callStackTextBox.Text != string.Empty;
			headerTextBox.Text = headerText;
			buildProjectLink.Visible = showBuildLink;
			if(buildProjectLinkText != null) { buildProjectLink.Text = buildProjectLinkText; }
			showGeneratedCodeLink.Visible = e is DevExpress.ExpressApp.Utils.CodeGeneration.CompilerErrorException;
			RecalculateLayout();
		}
		public void SetErrorMessage(Exception e) {
			SetErrorMessage(e, DefaultHeader, true, null);
		}
		public event EventHandler BuildProjectClicked;
		public event EventHandler ShowGeneratedCodeClicked;
	}
}
