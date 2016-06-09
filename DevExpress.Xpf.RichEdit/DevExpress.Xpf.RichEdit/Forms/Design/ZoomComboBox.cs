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
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Helpers;
namespace DevExpress.Xpf.RichEdit.UI {
	public class ZoomComboBoxEditSettings : ComboBoxEditSettings {
		static ZoomComboBoxEditSettings() {
			RegisterEditor();
		}
		internal static void RegisterEditor() {
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(ZoomComboBoxEdit), typeof(ZoomComboBoxEditSettings), delegate() { return new ZoomComboBoxEdit(); }, delegate() { return new ZoomComboBoxEditSettings(); });
		}
		public ZoomComboBoxEditSettings() {
			Populate();
		}
		void Populate() {
			Items.Clear();
			Items.Add(500.0);
			Items.Add(200.0);
			Items.Add(150.0);
			Items.Add(100.0);
			Items.Add(75.0);
			Items.Add(50.0);
			Items.Add(25.0);
			Items.Add(10.0);
		}
	}
	[DXToolboxBrowsableAttribute(false)]
	public class ZoomComboBoxEdit : ComboBoxEdit {
		static ZoomComboBoxEdit() {
			ZoomComboBoxEditSettings.RegisterEditor();
		}
		public ZoomComboBoxEdit()
			: base() {
			DefaultStyleKey = typeof(ZoomComboBoxEdit);
		}
		protected override BaseEditSettings CreateEditorSettings() {
			return new ZoomComboBoxEditSettings();
		}
	}
	public class PercentDisplayTextConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (value == null)
				return null;
			try {
				return String.Format(culture, "{0}%", (int)Math.Round(System.Convert.ToDouble(value)));
			}
			catch {
				return value;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
