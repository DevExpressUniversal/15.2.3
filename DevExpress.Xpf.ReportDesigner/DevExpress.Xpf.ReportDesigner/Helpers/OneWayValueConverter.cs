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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using DevExpress.Utils;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public static class GenericValueConverter {
		public static IValueConverter Create<TSource, TTarget>(Func<TSource, TTarget> convertMethod, Func<TTarget, TSource> convertBackMethod) {
			return new GenericValueConverterImpl<TSource, TTarget>(convertMethod, convertBackMethod);
		}
		sealed class GenericValueConverterImpl<TSource, TTarget> : IValueConverter {
			readonly Func<TSource, TTarget> convertMethod;
			readonly Func<TTarget, TSource> convertBackMethod;
			public GenericValueConverterImpl(Func<TSource, TTarget> convertMethod, Func<TTarget, TSource> convertBackMethod) {
				Guard.ArgumentNotNull(convertMethod, "convertMethod");
				this.convertMethod = convertMethod;
				this.convertBackMethod = convertBackMethod;
			}
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
				return convertMethod((TSource)value);
			}
			object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
				if(convertBackMethod == null)
					throw new NotSupportedException();
				return convertBackMethod((TTarget)value);
			}
		}
	}
}
