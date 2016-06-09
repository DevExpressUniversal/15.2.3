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
using System.Linq;
using DevExpress.Utils.Design;
using System.ComponentModel;
using Microsoft.Windows.Design.Model;
using System.Windows.Input;
using System.Collections;
using System.Windows.Media;
namespace DevExpress.Design.SmartTags {
	public class EnumPropertyLineViewModel : PropertyLineViewModelBase {
		readonly Type enumType;
		public IEnumerable Values {
			get {
				IEnumerable values = Enum.GetValues(enumType);
				if(ProvideValuesCallback != null)
					values = ProvideValuesCallback(values);
				return values;
			}
		}
		ProvideEnumValuesCallback ProvideValuesCallback { get; set; }
		public EnumPropertyLineViewModel(IPropertyLineContext context, string propertyName, Type enumType, Type propertyOwnerType = null, ProvideEnumValuesCallback provideValuesCallback = null)
			: base(context, propertyName, enumType, propertyOwnerType, context.PlatformInfoFactory.ForStandardProperty(enumType.Name)) {
			this.enumType = enumType;
			this.ProvideValuesCallback = provideValuesCallback;
		}
	}
	public delegate IEnumerable ProvideEnumValuesCallback(IEnumerable values);
}
