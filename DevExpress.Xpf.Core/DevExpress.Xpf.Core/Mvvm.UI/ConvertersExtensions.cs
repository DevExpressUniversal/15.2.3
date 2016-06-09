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
using System.Windows.Markup;
namespace DevExpress.Mvvm.UI {
#if !SILVERLIGHT
	public class ReflectionConverterExtension : MarkupExtension {
		class TypeUnsetValue { }
		Type convertBackMethodOwner = typeof(TypeUnsetValue);
		public Type ConvertMethodOwner { get; set; }
		public string ConvertMethod { get; set; }
		public Type ConvertBackMethodOwner {
			get { return convertBackMethodOwner == typeof(TypeUnsetValue) ? ConvertMethodOwner : convertBackMethodOwner; }
			set { convertBackMethodOwner = value; }
		}
		public string ConvertBackMethod { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new ReflectionConverter() { ConvertMethodOwner = ConvertMethodOwner, ConvertMethod = ConvertMethod, ConvertBackMethodOwner = ConvertBackMethodOwner, ConvertBackMethod = ConvertBackMethod };
		}
	}
	public class EnumerableConverterExtension : MarkupExtension {
		public EnumerableConverterExtension() { }
		public EnumerableConverterExtension(IValueConverter itemConverter) {
			ItemConverter = itemConverter;
		}
		public IValueConverter ItemConverter { get; set; }
		public Type TargetItemType { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new EnumerableConverter() { ItemConverter = ItemConverter, TargetItemType = TargetItemType };
		}
	}
#endif
#if !FREE
	public class CriteriaOperatorConverterExtension : MarkupExtension {
		public string Expression { get; set; }
		public CriteriaOperatorConverterExtension() { }
		public CriteriaOperatorConverterExtension(string expression) {
			this.Expression = expression;
		}
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return new CriteriaOperatorConverter() { Expression = Expression };
		}
	}
#endif
	public class TypeCastConverterExtension : MarkupExtension {
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return new TypeCastConverter();
		}
	}
	public class NumericToBooleanConverterExtension : MarkupExtension {
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return new NumericToBooleanConverter();
		}
	}
	public class StringToBooleanConverterExtension : MarkupExtension {
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return new StringToBooleanConverter();
		}
	}
	public class ObjectToBooleanConverterExtension : MarkupExtension {
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return new ObjectToBooleanConverter() { Inverse = this.Inverse };
		}
		public bool Inverse { get; set; }
	}	
	public class BooleanToVisibilityConverterExtension : MarkupExtension {
		public bool Inverse { get; set; }
		public bool HiddenInsteadOfCollapsed { get; set; }
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return new BooleanToVisibilityConverter() { Inverse = this.Inverse, HiddenInsteadOfCollapsed = this.HiddenInsteadOfCollapsed };
		}
	}
	public class NumericToVisibilityConverterExtension : MarkupExtension {
		public bool Inverse { get; set; }
		public bool HiddenInsteadOfCollapsed { get; set; }
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return new NumericToVisibilityConverter() { Inverse = this.Inverse, HiddenInsteadOfCollapsed = this.HiddenInsteadOfCollapsed };
		}
	}
	public class DefaultBooleanToBooleanConverterExtension : MarkupExtension {
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return new DefaultBooleanToBooleanConverter();
		}
	}
	public class BooleanNegationConverterExtension : MarkupExtension {
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return new BooleanNegationConverter();
		}
	}
	public class FormatStringConverterExtension : MarkupExtension {
		public string FormatString { get; set; }
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return new FormatStringConverter() { FormatString = this.FormatString };
		}
	}
	public class BooleanToObjectConverterExtension : MarkupExtension {
		public object TrueValue { get; set; }
		public object FalseValue { get; set; }
		public object NullValue { get; set; }
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return new BooleanToObjectConverter() { TrueValue = this.TrueValue, FalseValue = this.FalseValue, NullValue = this.NullValue };
		}
	}
}
