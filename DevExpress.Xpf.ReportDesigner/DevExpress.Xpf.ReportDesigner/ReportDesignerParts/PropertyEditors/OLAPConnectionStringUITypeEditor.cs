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

using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.PropertyGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class OLAPConnectionStringUITypeEditor : Control {
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty ShowAdvancedPropertiesProperty;
		static readonly DependencyPropertyKey ConnectionStringBuilderPropertyKey;
		public static readonly DependencyProperty ConnectionStringBuilderProperty;
		static OLAPConnectionStringUITypeEditor() {
			DependencyPropertyRegistrator<OLAPConnectionStringUITypeEditor>.New()
				.Register(d => d.EditValue, out EditValueProperty, null, d => d.OnEditValueChanged(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.Register(d => d.ShowAdvancedProperties, out ShowAdvancedPropertiesProperty, false)
				.RegisterReadOnly(d => d.ConnectionStringBuilder, out ConnectionStringBuilderPropertyKey, out ConnectionStringBuilderProperty, null)
				.OverrideDefaultStyleKey()
			;
		}
		public OLAPConnectionStringUITypeEditor() {
			ConnectionStringBuilder = new OLAPConnectionStringBuilder();
			this.saveCommand = DelegateCommandFactory.Create(Save);
		}
		readonly ICommand saveCommand;
		public ICommand SaveCommand { get { return saveCommand; } }
		public string EditValue {
			get { return (string)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		void OnEditValueChanged() {
			ConnectionStringBuilder = new OLAPConnectionStringBuilder(EditValue);
		}
		public bool ShowAdvancedProperties {
			get { return (bool)GetValue(ShowAdvancedPropertiesProperty); }
			set { SetValue(ShowAdvancedPropertiesProperty, value); }
		}
		public OLAPConnectionStringBuilder ConnectionStringBuilder {
			get { return (OLAPConnectionStringBuilder)GetValue(ConnectionStringBuilderProperty); }
			private set { SetValue(ConnectionStringBuilderPropertyKey, value); }
		}
		void Save() {
			EditValue = ConnectionStringBuilder.FullConnectionString;
		}
	}
}
