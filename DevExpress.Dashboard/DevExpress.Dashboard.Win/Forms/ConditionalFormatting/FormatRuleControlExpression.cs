#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Windows.Forms;
using DevExpress.DataAccess.UI.Native;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraLayout;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class FormatRuleControlExpression : FormatRuleControlStyleBase, IFormatRuleControlExpressionView {
		FilterCriteriaEditorControl filterEditorControl;
		EventHandler<ShowValueEditorEventArgs> filterEditorControlInitialize;
		public FormatRuleControlExpression()
			: base() {
			InitializeComponent();
		}
		protected override void Initialize(IFormatRuleControlViewInitializationContext initializationContext) {
			base.Initialize(initializationContext);
			IFormatRuleViewExpressionContext context = (IFormatRuleViewExpressionContext)initializationContext;
			ValuePanelGroup.Visibility = XtraLayout.Utils.LayoutVisibility.Never;
			this.filterEditorControl = new FilterCriteriaEditorControl(null, context.FilteredComponent, context.Parameters, context.DefaultItem);
			this.filterEditorControl.Dock = DockStyle.Fill;
			this.filterEditorControl.CriteriaChanged += OnFilterEditorControlCriteriaChanged;
			this.filterEditorControl.BeforeShowValueEditor += OnFilterEditorControlBeforeShowValueEditor;
			LayoutControlItem lciTab = RootGroup.AddItem(MainPanelGroup, XtraLayout.Utils.InsertType.Left);
			lciTab.Control = filterEditorControl;
			lciTab.Padding = new XtraLayout.Utils.Padding(-4, 10, 0, -4);
			lciTab.Spacing = new XtraLayout.Utils.Padding(0);
			lciTab.TextVisible = false;
		}
		void OnFilterEditorControlBeforeShowValueEditor(object sender, ShowValueEditorEventArgs e) {
			if(filterEditorControlInitialize != null)
				filterEditorControlInitialize(this, e);
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			ISupportLookAndFeel lookAndFeelOwner = FindParentLookAndFeelOwner();
			if(lookAndFeelOwner != null)
				LookAndFeel.ParentLookAndFeel = lookAndFeelOwner.LookAndFeel;
		}
		ISupportLookAndFeel FindParentLookAndFeelOwner() {
			Control parent = Parent;
			while(parent != null) {
				ISupportLookAndFeel lookAndFeelOwner = parent as ISupportLookAndFeel;
				if(lookAndFeelOwner != null)
					return lookAndFeelOwner;
				parent = parent.Parent;
			}
			return null;
		}
		void OnFilterEditorControlCriteriaChanged(object sender, FilterCriteriaEditorCriteriaChangedEventArgs e) {
			RaiseStateUpdated();
		}
		#region IFormatRuleControlExpressionView Members
		string IFormatRuleControlExpressionView.Expression {
			get { return filterEditorControl.FilterCriteriaString; }
			set { filterEditorControl.FilterCriteriaString = value; }
		}
		event EventHandler<ShowValueEditorEventArgs> IFormatRuleControlExpressionView.FilterEditorControlInitialize {
			add { filterEditorControlInitialize += value; }
			remove { filterEditorControlInitialize -= value; }
		}
		#endregion
	}
}
