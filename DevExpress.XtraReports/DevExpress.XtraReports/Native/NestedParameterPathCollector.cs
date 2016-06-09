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
using DevExpress.Data;
using DevExpress.XtraReports.Data;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Native {
	public class NestedParameterPathCollector : NestedParameterCollectorBase {
		public event ExceptionEventHandler ExceptionOccurred;
		class CustomDataContainerEnumerator : ParameterContainerEnumerator {
			readonly Stack<string> names = new Stack<string>();
			protected override IEnumerable<IDataContainerBase> EnumerateSubreport(SubreportBase subreport) {
				string name = !string.IsNullOrEmpty(subreport.Name) ? subreport.Name : "undefinedSubreport";
				names.Push(name);
				try {
					foreach(IDataContainerBase item in base.EnumerateSubreport(subreport))
						yield return item;
				} finally {
					names.Pop();
				}
			}
			public string GetPath(string name) {
				names.Push(name);
				try {
					return string.Join(".", names.Reverse<string>());
				} finally {
					names.Pop();
				}
			}
		}
		public virtual IEnumerable<ParameterPath> EnumerateParameters(XtraReport report) {
			Guard.ArgumentNotNull(report, "report");
			parameterSuppliers.Clear();
			CustomDataContainerEnumerator enumerator = new CustomDataContainerEnumerator() { IncludeSubreports = true };
			enumerator.ExceptionOccurred += enumerator_ExceptionOccurred;
			foreach(IDataContainerBase dataContainer in enumerator.EnumerateDataContainers(report)) {
				foreach(Parameter param in GetParameters(dataContainer as IParameterSupplier))
					yield return new ParameterPath(param, enumerator.GetPath(param.Name));
				foreach(Parameter param in GetParameters(dataContainer.DataSource as IParameterSupplier))
					yield return new ParameterPath(param, enumerator.GetPath(param.Name));
			}
		}
		void enumerator_ExceptionOccurred(object sender, ExceptionEventArgs args) {
			if(ExceptionOccurred != null) {
				ExceptionOccurred(this, args);
			} else {
				args.Handled = true;
			}
		}
	}
}
