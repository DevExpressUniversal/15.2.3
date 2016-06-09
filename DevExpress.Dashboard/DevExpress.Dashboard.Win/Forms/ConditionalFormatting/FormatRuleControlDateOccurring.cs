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
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class FormatRuleControlDateOccurring : FormatRuleControlStyleBase, IFormatRuleControlDateOccurringView {
		CheckedComboBoxEdit cbValue;
		public FormatRuleControlDateOccurring()
			: base() {
			InitializeComponent();
		}
		protected override void Initialize(IFormatRuleControlViewInitializationContext initializationContext) {
			base.Initialize(initializationContext);
			AddValueItem();
			cbValue.Tag = FilterDateType.User;
			cbValue.Properties.SetFlags(typeof(FilterDateType));
			cbValue.Properties.Items.Clear();
			EnumManager.Iterate<FilterDateType>((dateType) => {
				cbValue.Properties.Items.Add(dateType, dateType.Localize());
			}, FilterDateType.None, FilterDateType.User, FilterDateType.SpecificDate);
			cbValue.Properties.CustomDisplayText += (s, e) => {
				if(FilterDateType.None.Equals(e.Value))
					e.DisplayText = ((FilterDateType)e.Value).Localize();
				else {
					string str = string.Empty;
					EnumManager.Iterate<FilterDateType>((dateType) => {
						if(((FilterDateType)e.Value & dateType) != 0) {
							str += string.Format("{0}, ", dateType.Localize());
						}
					});
					e.DisplayText = (str.Length > 2) ? str.Substring(0, str.Length - 2) : str;
				}
			};
			cbValue.EditValueChanged += OnDateComboValueChanged;
		}
		void AddValueItem() {
			this.cbValue = new CheckedComboBoxEdit();
			this.cbValue.Name = "cbValue";
			this.cbValue.MinimumSize = new Size(50, 20);
			LayoutControlItem lciValue = ValuePanelGroup.AddItem("Date = ", this.cbValue);
			lciValue.Name = "lciValue";
			lciValue.Padding = new XtraLayout.Utils.Padding(0);
			lciValue.TextVisible = false;
		}
		void OnDateComboValueChanged(object sender, EventArgs e) {
			RaiseStateUpdated();
		}
		#region IFormatRuleControlDateOccurringView Members
		FilterDateType IFormatRuleControlDateOccurringView.DateType {
			get { return (FilterDateType)cbValue.EditValue; }
			set { cbValue.SetEditValue(value); }
		}
		#endregion
	}
}
