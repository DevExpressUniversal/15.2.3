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
using System.Text;
using DevExpress.XtraPrinting.Native.Lines;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Xpf.Editors;
#if SILVERLIGHT
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Xpf.Printing.Native.Lines {
	public class LineFactory : LineFactoryBase {
		public override ILine CreateBooleanLine(PropertyDescriptor property, object obj) {
			return new BooleanLine(property, obj);
		}
		public override ILine CreateColorPropertyLine(IStringConverter converter, PropertyDescriptor property, object obj) {
			return new CustomEditorPropertyLine(new PopupColorEdit(), PopupColorEdit.ColorProperty.GetName(), new GdiColorToMediaColorConverter(), null, property, obj);
		}
		public override ILine CreateComboBoxPropertyLine(IStringConverter converter, object[] values, PropertyDescriptor property, object obj) {
			return new ComboBoxPropertyLine(converter, values, property, obj);
		}
		public override ILine CreateEmptyLine() {
			return new EmptyLine();
		}
		public override ILine CreateNumericPropertyLine(IStringConverter converter, PropertyDescriptor property, object obj) {
			return new NumericPropertyLine(converter, property, obj);
		}
		public override ILine CreateSeparatorLine() {
			return CreateEmptyLine();
		}
		public override ILine CreateEditorPropertyLine(IStringConverter converter, PropertyDescriptor property, object obj) {
			return new ButtonEditPropertyLine(converter, property, obj);
		}
		public override ILine CreateDateTimePropertyLine(IStringConverter converter, PropertyDescriptor property, object obj) {
			return new DateTimePropertyLine(converter, property, obj);
		}
	}
}
