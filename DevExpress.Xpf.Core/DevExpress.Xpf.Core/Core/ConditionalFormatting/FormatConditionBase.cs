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

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.GridData;
namespace DevExpress.Xpf.Core.ConditionalFormatting.Native {
	public abstract class FormatConditionBaseInfo {
		readonly IColumnInfo unboundColumnInfo;
		public Freezable FormatCore { get; set; }
		public string FieldName { get; set; }
		public string Expression { get; set; }
		public FormatConditionBaseInfo() {
			this.unboundColumnInfo = new ConditionalFormattingColumnInfo(() => ActualExpression);
		}
		public abstract string OwnerPredefinedFormatsPropertyName { get; }
		public void OnFormatNameChanged(DependencyObject condition, string predefinedFormatName, string predefinedFormatsOwnerPath, DependencyProperty formatPropertyForBinding) {
			if(string.IsNullOrEmpty(predefinedFormatName))
				BindingOperations.ClearBinding(condition, formatPropertyForBinding);
			else 
				BindingOperations.SetBinding(condition, formatPropertyForBinding, new Binding(predefinedFormatsOwnerPath + OwnerPredefinedFormatsPropertyName + "[" + predefinedFormatName + "].Format") { RelativeSource = RelativeSource.Self });
		}
		public static object OnCoerceFreezable(object baseValue) {
			return (baseValue as Freezable).With(x => x.GetAsFrozen());
		}
		public static FormatConditionChangeType GetChangeType(DependencyPropertyChangedEventArgs e) {
			return (e.OldValue != null && e.NewValue != null) ? FormatConditionChangeType.AppearanceOnly : FormatConditionChangeType.All;
		}
		public string ActualFieldName { get { return unboundColumnInfo.UnboundExpression != null ? unboundColumnInfo.FieldName : FieldName; } }
		protected virtual string ActualExpression { get { return Expression; } }
		public virtual IEnumerable<IColumnInfo> GetUnboundColumnInfo() {
			if(unboundColumnInfo.UnboundExpression == null)
				return Enumerable.Empty<IColumnInfo>();
			return new IColumnInfo[] { unboundColumnInfo };
		}
		public static bool IsFit(object value) {
			return value is bool ? (bool)value : false;
		}
		public IEnumerable<ConditionalFormatSummaryInfo> CreateSummaryItems() {
			return GetSummaries().Select(x => new ConditionalFormatSummaryInfo(x, ActualFieldName));
		}
		public abstract IEnumerable<ConditionalFormatSummaryType> GetSummaries();
		public abstract Brush CoerceBackground(Brush value, FormatValueProvider provider);
		public abstract DataBarFormatInfo CoerceDataBarFormatInfo(DataBarFormatInfo value, FormatValueProvider provider);
		public virtual TextDecorationCollection CoerceTextDecorations(TextDecorationCollection value, FormatValueProvider provider) {
			return value;
		}
		public virtual Brush CoerceForeground(Brush value, FormatValueProvider provider) {
			return value;
		}
		public virtual double CoerceFontSize(double value, FormatValueProvider provider) {
			return value;
		}
		public virtual FontStyle CoerceFontStyle(FontStyle value, FormatValueProvider provider) {
			return value;
		}
		public virtual FontFamily CoerceFontFamily(FontFamily value, FormatValueProvider provider) {
			return value;
		}
		public virtual FontStretch CoerceFontStretch(FontStretch value, FormatValueProvider provider) {
			return value;
		}
		public virtual FontWeight CoerceFontWeight(FontWeight value, FormatValueProvider provider) {
			return value;
		}
		public abstract ConditionalFormatMask FormatMask { get; }
	}
}
