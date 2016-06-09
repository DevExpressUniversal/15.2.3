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

#if SILVERLIGHT
extern alias Platform;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Globalization;
#if !SILVERLIGHT
using DevExpress.Xpf.Editors;
using EnumHelper2 = DevExpress.Utils.EnumExtensions;
#else
using Platform::DevExpress.Xpf.Editors;
using EnumHelper2 = Platform::DevExpress.Utils.EnumExtensions;
#endif
namespace DevExpress.Xpf.Core.Design.Wizards {
	public partial class EditMaskWizard : UserControl {
		internal static Dictionary<MaskType, List<PredefinedMask>> predefinedMasks = new Dictionary<MaskType, List<PredefinedMask>>();
		static EditMaskWizard() {
			foreach(MaskType maskType in EnumHelper2.GetValues(typeof(MaskType)))
				predefinedMasks.Add(maskType, GetPredefinedMasks(maskType));
		}
		static void AddPredefinedMask(List<PredefinedMask> list, string description, string mask, string exampleFormat, params object[] exampleValues) {
			list.Add(new PredefinedMask(description, mask, string.Format(exampleFormat, exampleValues)));
		}
		static List<PredefinedMask> GetPredefinedMasks(MaskType maskType) {
			List<PredefinedMask> result = new List<PredefinedMask>();
			switch (maskType) {
				case MaskType.DateTime:
				case MaskType.DateTimeAdvancingCaret:
					DateTime dt = DateTime.Now;
					AddPredefinedMask(result, "Short date", "d", "{0:d}", dt);
					AddPredefinedMask(result, "Long date", "D", "{0:D}", dt);
					AddPredefinedMask(result, "Short time", "t", "{0:t}", dt);
					AddPredefinedMask(result, "Long time", "T", "{0:T}", dt);
					AddPredefinedMask(result, "Full date/time (short time)", "f", "{0:f}", dt);
					AddPredefinedMask(result, "Full date/time (long time)", "F", "{0:F}", dt);
					AddPredefinedMask(result, "General date/time (short time)", "g", "{0:g}", dt);
					AddPredefinedMask(result, "General date/time (long time)", "G", "{0:G}", dt);
					AddPredefinedMask(result, "Month day", "m", "{0:m}", dt);
					AddPredefinedMask(result, "RFC1123", "r", "{0:r}", dt);
					AddPredefinedMask(result, "Sortable date/time", "s", "Conforms to ISO 8601\n{0:s}", dt);
					AddPredefinedMask(result, "Universal sortable date/time", "u", "{0:u}", dt);
					AddPredefinedMask(result, "Year month", "y", "{0:y}", dt);
					break;
				case MaskType.Numeric:
					double d1 = 123456789.123, d2 = -12.346;
					double p1 = 12.346, p2 = -123.457;
					AddPredefinedMask(result, "Currency", "c", "{0:c}\n{1:c}", d1, d2);
					AddPredefinedMask(result, "Decimal", "d", "{0:d}\n{1:d}", (int)d1, (int)d2);
					AddPredefinedMask(result, "Fixed-point", "f", "{0:f}\n{1:f}", d1, d2);
					AddPredefinedMask(result, "Number", "n", "{0:n}\n{1:n}", d1, d2);
					AddPredefinedMask(result, "Percent (mode 1)", "P", "If Text is '{0:f0}%' then\nEditValue={0:f0}\n{0:f} %\n{1:f} %", p1, p2);
					AddPredefinedMask(result, "Percent (mode 2)", "p", "If Text is '{0:f0}%' then\nEditValue={1:f}\n{0:f} %\n{2:f} %", p1, p1 / 100, p2);
					AddPredefinedMask(result, "Currency (integer)", "c0", "{0:c0}\n{1:c0}", d1, d2);
					AddPredefinedMask(result, "Fixed-point (integer)", "f0", "{0:f0}\n{1:f0}", d1, d2);
					AddPredefinedMask(result, "Number (integer)", "n0", "{0:n0}\n{1:n0}", d1, d2);
					AddPredefinedMask(result, "Percent (mode 1) (integer)", "P0", "If Text is '{0:f0}%' then\nEditValue={0:f0}\n{0:f0} %\n{1:f0} %", p1, p2);
					AddPredefinedMask(result, "Percent (mode 2) (integer)", "p0", "If Text is '{0:f0}%' then\nEditValue={1:f}\n{0:f0} %\n{2:f0} %", p1, p1 / 100, p2);
					AddPredefinedMask(result, "Currency (2 decimal places)", "c2", "{0:c2}\n{1:c2}", d1, d2);
					AddPredefinedMask(result, "Fixed-point (2 decimal places)", "f2", "{0:f2}\n{1:f2}", d1, d2);
					AddPredefinedMask(result, "Number (2 decimal places)", "n2", "{0:n2}\n{1:n2}", d1, d2);
					AddPredefinedMask(result, "Percent (mode 1) (2 decimal places)", "P2", "If Text is '{0:f0}%' then\nEditValue={0:f0}\n{0:f2} %\n{1:f2} %", p1, p2);
					AddPredefinedMask(result, "Percent (mode 2) (2 decimal places)", "p2", "If Text is '{0:f0}%' then\nEditValue={1:f}\n{0:f2} %\n{2:f2} %", p1, p1 / 100, p2);
					AddPredefinedMask(result, "Currency (3 decimal places)", "c3", "{0:c3}\n{1:c3}", d1, d2);
					AddPredefinedMask(result, "Fixed-point (3 decimal places)", "f3", "{0:f3}\n{1:f3}", d1, d2);
					AddPredefinedMask(result, "Number (3 decimal places)", "n3", "{0:n3}\n{1:n3}", d1, d2);
					AddPredefinedMask(result, "Percent (mode 1) (3 decimal places)", "P3", "If Text is '{0:f0}%' then\nEditValue={0:f0}\n{0:f3} %\n{1:f3} %", p1, p2);
					AddPredefinedMask(result, "Percent (mode 2) (3 decimal places)", "p3", "If Text is '{0:f0}%' then\nEditValue={1:f}\n{0:f3} %\n{2:f3} %", p1, p1 / 100, p2);
					break;
				case MaskType.RegEx:
					AddPredefinedMask(result, "Time of day", @"(0?\d|1\d|2[0-3])\:[0-5]\d", "The 24 hour day time:\n15:25\n2:05\n03:57");
					AddPredefinedMask(result, "Time of day with seconds", @"(0?\d|1\d|2[0-3]):[0-5]\d:[0-5]\d", "The 24 hour day time with seconds:\n12:45:10\n3:00:01");
					AddPredefinedMask(result, "Time of day (AM/PM)", @"(0?[1-9]|1[012]):[0-5]\d(AM|PM)", "The 12 hour day time:\n1:35PM\n12:45AM");
					AddPredefinedMask(result, "Time of day with seconds (AM/PM)", @"(0?[1-9]|1[012]):[0-5]\d:[0-5]\d(AM|PM)", "The 12 hour day time with seconds:\n10:03:10AM\n03:00:01PM");
					AddPredefinedMask(result, "Date", @"(0?[1-9]|1[012])/([012]?[1-9]|[123]0|31)/([123][0-9])?[0-9][0-9]", "The MM/dd/yy or MM/dd/yyyy date with year from 1000 to 3999:\n3/12/99\n06/25/1800");
					AddPredefinedMask(result, "Telephone number", @"(\(\d\d\d\) )?\d{1,3}-\d\d-\d\d", "The telephone number with or without city code:\n(345) 234-12-07\n(210) 7-17-81\n26-32-22");
					AddPredefinedMask(result, "Extension", @"\d{0,5}", "15450");
					AddPredefinedMask(result, "Social security", @"\d\d\d-\d\d-\d\d\d\d", "555-55-5555");
					AddPredefinedMask(result, "Short zip code", @"\d\d\d\d\d", "11200");
					AddPredefinedMask(result, "Long zip code", @"\d\d\d\d\d-\d\d\d\d", "11200-0000");
					AddPredefinedMask(result, "E-mail", @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}", "The e-mail address");
					AddPredefinedMask(result, "Decimal number", @"\d+", "Any decimal number");
					AddPredefinedMask(result, "Hexadecimal number", @"[0-9A-Fa-f]+", "Any hexadecimal number");
					AddPredefinedMask(result, "Octal number", @"[0-7]+", "Any octal number");
					AddPredefinedMask(result, "Binary number", @"[01]+", "Any binary number");
					AddPredefinedMask(result, "Yes/No", @"Yes|No", "Yes\nNo");
					AddPredefinedMask(result, "True/False", @"True|False", "True\nFalse");
					AddPredefinedMask(result, "Any symbols", @".+", "Any symbols");
					AddPredefinedMask(result, "Latin letters only", @"[a-zA-Z]+", "Any letters of the latin alphabet");
					AddPredefinedMask(result, "Letters only", @"\p{L}+", "Any letters");
					AddPredefinedMask(result, "Uppercase letters", @"\p{Lu}+", "Any uppercase letters");
					AddPredefinedMask(result, "Lowercase letters", @"\p{Ll}+", "Any lowercase letters");
					break;
				case MaskType.Regular:
					AddPredefinedMask(result, "Phone", @"(\d?\d?\d?) \d\d\d-\d\d\d\d", "(213) 144-1756");
					AddPredefinedMask(result, "Extension", @"\d?\d?\d?\d?\d?", "15023");
					AddPredefinedMask(result, "Social security", @"\d\d\d-\d\d-\d\d\d\d", "555-55-5555");
					AddPredefinedMask(result, "Short zip code", @"\d\d\d\d\d", "90628");
					AddPredefinedMask(result, "Long zip code", @"\d\d\d\d\d-\d?\d?\d?\d?", "90628-0000");
					AddPredefinedMask(result, "Date", @"\d?\d?/\d?\d?/\d\d", "03/24/99");
					AddPredefinedMask(result, "Long Time", @"\d?\d:\d\d:\d\d>[AP]M", "04:15:34PM");
					AddPredefinedMask(result, "Short Time", @"\d?\d:\d\d", "21:45");
					break;
				case MaskType.Simple:
					AddPredefinedMask(result, "Phone", @"(999) 000-0000", "(213) 144-1756");
					AddPredefinedMask(result, "Extension", @"99999", "15023");
					AddPredefinedMask(result, "Social security", @"000-00-0000", "555-55-5555");
					AddPredefinedMask(result, "Short zip code", @"00000", "90628");
					AddPredefinedMask(result, "Long zip code", @"00000-9999", "90628-0000");
					AddPredefinedMask(result, "Date", @"99/99/00", "03/24/99");
					AddPredefinedMask(result, "Long Time", @"90:00:00>LL", "04:15:34PM");
					AddPredefinedMask(result, "Short Time", @"90:00", "21:45");
					break;
			}
			return result;
		}
		public EditMaskWizard() {
			InitializeComponent();
#if SL
			HideMaskCulture();
#endif
			cbMaskAutoComplete.ItemsSource = EnumHelper2.GetValues(typeof(AutoCompleteType));
			List<CultureInfo> cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList<CultureInfo>();
			cultures.Sort((item1, item2) => string.Compare(item1.ToString(), item2.ToString()));
			cbMaskCulture.ItemsSource = cultures;
			DataContextChanged += OnDataContextChanged;
		}
		private void HideMaskCulture() {
			tbMaskCulture.Visibility = System.Windows.Visibility.Collapsed;
			cbMaskCulture.Visibility = System.Windows.Visibility.Collapsed;
			cbMaskBeepOnError.Visibility = System.Windows.Visibility.Collapsed;
		}
		internal void FormResult() {
			EditMaskWizardParams wizardParams = (EditMaskWizardParams)DataContext;
			MaskProperties properties = new MaskProperties();
			properties.Mask = tbMask.Text;
			properties.MaskAllowNullInput = wizardParams.Input.MaskAllowNullInput;
			properties.MaskAutoComplete = (AutoCompleteType)cbMaskAutoComplete.SelectedValue;
			properties.MaskBeepOnError = (bool)cbMaskBeepOnError.IsChecked;
			properties.MaskCulture = (CultureInfo)cbMaskCulture.SelectedValue;
			properties.MaskIgnoreBlank = (bool)cbMaskIgnoreBlank.IsChecked;
			properties.MaskPlaceHolder = string.IsNullOrEmpty(tbMaskPlaceHolder.Text) ? (char)0 : tbMaskPlaceHolder.Text[0];
			properties.MaskSaveLiteral = (bool)cbMaskSaveLiteral.IsChecked;
			properties.MaskShowPlaceHolders = (bool)cbMaskShowPlaceHolders.IsChecked;
			properties.MaskType = (MaskType)cbMaskType.SelectedValue;
			properties.MaskUseAsDisplayFormat = (bool)cbMaskUseAsDisplayFormat.IsChecked;
			wizardParams.Output = properties;
		}
		void lbPredefinedMasks_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (lbPredefinedMasks.SelectedItem != null)
				tbMask.Text = ((PredefinedMask)lbPredefinedMasks.SelectedItem).Mask;
		}
		void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			cbMaskType.ItemsSource = ((EditMaskWizardParams)DataContext).Input.SupportedMaskTypes;
		}
	}
	public class MaskProperties : IMaskProperties {
		public string Mask { get; internal set; }
		public bool MaskAllowNullInput { get; internal set; }
		public AutoCompleteType MaskAutoComplete { get; internal set; }
		public bool MaskBeepOnError { get; internal set; }
		public CultureInfo MaskCulture { get; internal set; }
		public bool MaskIgnoreBlank { get; internal set; }
		public char MaskPlaceHolder { get; internal set; }
		public bool MaskSaveLiteral { get; internal set; }
		public bool MaskShowPlaceHolders { get; internal set; }
		public MaskType MaskType { get; internal set; }
		public bool MaskUseAsDisplayFormat { get; internal set; }
		public MaskType[] SupportedMaskTypes { get; internal set; }
	}
	public class PredefinedMask {
		public PredefinedMask(string description, string mask, string example) {
			Description = description;
			Mask = mask;
			Example = example;
		}
		public string Description { get; private set; }
		public string Example { get; private set; }
		public string Mask { get; private set; }
	}
	public class ContainsEnumValueConverter : IValueConverter {
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (value == null) return false;
			return ((string)parameter).Split(';').Contains(value.ToString());
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class MaskTypeDescriptionConverter : IValueConverter {
		static Dictionary<MaskType, string> maskTypeDescriptions;
		static MaskTypeDescriptionConverter() {
			maskTypeDescriptions = new Dictionary<MaskType, string>();
			maskTypeDescriptions.Add(MaskType.DateTime, "Masks for entering date/time values\n ");
			maskTypeDescriptions.Add(MaskType.DateTimeAdvancingCaret, "Masks for entering date/time values\n(with the caret automatic movement feature)");
			maskTypeDescriptions.Add(MaskType.None, "The mask functionality is disabled\n ");
			maskTypeDescriptions.Add(MaskType.Numeric, "Masks for entering numeric values\n ");
			maskTypeDescriptions.Add(MaskType.RegEx, "Full-functional regular expressions\n ");
			maskTypeDescriptions.Add(MaskType.Regular, "Simplified regular expressions (no-alternation,\nno auto-complete)");
			maskTypeDescriptions.Add(MaskType.Simple, "Masks for entering values of a fixed format\n(e.g. phone numbers)");
		}
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null) return null;
			return maskTypeDescriptions[(MaskType)value];
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class PredefinedMasksSourceConverter : IValueConverter {
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null) return null;
			return EditMaskWizard.predefinedMasks[(MaskType)value];
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class EditMaskWizardParams {
		public IMaskProperties Input { get; set; }
		public IMaskProperties Output { get; set; }
	}
}
