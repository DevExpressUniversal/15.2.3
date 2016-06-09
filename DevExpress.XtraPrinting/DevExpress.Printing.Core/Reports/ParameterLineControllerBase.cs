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
using System.Text;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.XtraReports.Parameters;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native.Lines;
#if SILVERLIGHT
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.XtraReports.Native {
	public abstract class ParameterLineControllerBase : BaseLineController {
		#region static
		public static void Commit(IList<ParameterLineControllerBase> lineControllers) {
			foreach(ParameterLineControllerBase controller in lineControllers)
				controller.propertyDescriptor.Commit();
		}
		public static void Reset(IList<ParameterLineControllerBase> lineControllers) {
			foreach(ParameterLineControllerBase controller in lineControllers)
				controller.propertyDescriptor.Reset();
		}
		#endregion
		ParameterPropertyDescriptor propertyDescriptor;
		Parameter parameter;
		object obj;
		public Parameter Parameter {
			get { return parameter; }
		}
		protected PropertyDescriptor PropertyDescriptor {
			get { return propertyDescriptor; }
		}
		protected object Obj { get { return obj; } }
		public ParameterLineControllerBase(Parameter parameter, object obj) {
			this.parameter = parameter;
			this.propertyDescriptor = new ParameterPropertyDescriptor(parameter);
			this.obj = obj;
		}
		protected override void InitLine() {
			base.InitLine();
			fLine.SetText(string.IsNullOrEmpty(Parameter.Description) ? Parameter.Name : Parameter.Description);
		}
		protected ITypeDescriptorContext CreateContext() {
			return new RuntimeTypeDescriptorContext(propertyDescriptor, obj);
		}
		protected TypeStringConverter CreateStringConverter() {
			return new TypeStringConverter(propertyDescriptor.Converter, CreateContext());
		}
	}
}
