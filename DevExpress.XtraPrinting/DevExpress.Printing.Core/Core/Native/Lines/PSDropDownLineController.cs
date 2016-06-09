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
using System.ComponentModel;
using DevExpress.XtraPrinting.Native.Lines;
#if SILVERLIGHT
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Xpf.ComponentModel;
#endif
namespace DevExpress.XtraPrinting.Native.Lines {
	public class PSDropDownLineController : PSTextLineController {
		public PSDropDownLineController(PropertyDescriptor property, object obj, string headerText)
			: base(property, obj, headerText) {
		}
		protected override ILine CreateLine(LineFactoryBase lineFactory) {
			if(lineFactory == null)
				return null;
			RuntimeTypeDescriptorContext context = CreateTypeDescriptorContext();
			object[] standardValues = RuntimeTypeDescriptorContext.GetStandardValues(context, property.Converter);
			IStringConverter stringConverter = null;
			if(property.Converter is EnumConverter)
				stringConverter = new LocalizedEnumStringConverter(property.PropertyType);
			else
				stringConverter = CreateTypeStringConverter();
			return lineFactory.CreateComboBoxPropertyLine(stringConverter, standardValues, property, obj);
		}
	}
}
