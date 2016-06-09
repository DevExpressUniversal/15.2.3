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
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Lines;
#if SILVERLIGHT
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.XtraPrinting.Native.Lines {
	public abstract class PSPropertyLineController : BaseLineController {
		string headerText;
		protected PropertyDescriptor property;
		protected object obj;
		#region tests
#if DEBUGTEST
		public string HeaderText { get { return headerText; } }
#endif
		#endregion
		protected PSPropertyLineController(PropertyDescriptor property, object obj, string headerText) {
			this.property = property;
			this.obj = obj;
			this.headerText = headerText;
		}
		protected override void InitLine() {
			fLine.SetText(headerText);
		}
		protected virtual TypeStringConverter CreateTypeStringConverter() {
			return new TypeStringConverter(property.Converter, CreateTypeDescriptorContext());
		}
		protected RuntimeTypeDescriptorContext CreateTypeDescriptorContext() {
			return new RuntimeTypeDescriptorContext(property, obj);
		}
	}
}
