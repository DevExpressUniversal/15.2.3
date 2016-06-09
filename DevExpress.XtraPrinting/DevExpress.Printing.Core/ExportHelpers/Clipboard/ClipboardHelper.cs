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
using System.Windows.Forms;
using System.Linq;
using DevExpress.Export;
namespace DevExpress.XtraExport.Helpers {
	public static class ClipboardHelper<TCol, TRow>
	where TCol : IColumn
	where TRow : IRowBase {
		static System.Reflection.Assembly printingAssembly = DevExpress.Data.Utils.AssemblyCache.LoadDXAssembly(AssemblyInfo.SRAssemblyPrinting);
		static System.Reflection.Assembly GetPrintingAssembly(bool throwException) {
			if(throwException && printingAssembly == null)
				throw new Exception(AssemblyInfo.SRAssemblyPrinting + " isn't found.");
			return printingAssembly;
		}
		static Type GetType(string typeName, bool throwException) {
			System.Reflection.Assembly printingAssembly = GetPrintingAssembly(false);
			if(printingAssembly != null)
				return printingAssembly.GetType(typeName);
			return GetTypeOfficially(typeName, throwException);
		}
		static Type GetTypeOfficially(string typeName, bool throwException) {
			return Type.GetType(string.Format("{0}, {1}", typeName, AssemblyInfo.SRAssemblyPrinting), throwException);
		}
		public static void SetClipboardData(IClipboardManager<TCol, TRow> manager, DataObject dataObject) {
			if(manager == null) return;
			manager.SetClipboardData(dataObject);
		}
		public static IClipboardManager<TCol, TRow> GetManager(IClipboardGridView<TCol, TRow> gridView, ClipboardOptions exportOptions) {
			Type type = GetType("DevExpress.XtraExport.Helpers.ClipboardExportManager`2", false);
			if(type == null) return null;
			Type[] genericsArguments = gridView.GetType().GetGenericArguments(); 
			if(genericsArguments.Length != 2) {
				foreach(Type interfaceType in gridView.GetType().GetInterfaces()) {   
					if(interfaceType.Namespace == "DevExpress.XtraExport.Helpers" && interfaceType.Name == "IClipboardGridView`2") {
						genericsArguments = interfaceType.GetGenericArguments();
						break;
					}  
				}
			}
			Type genericType = type.MakeGenericType(genericsArguments);
			IClipboardManager<TCol, TRow> manager = Activator.CreateInstance(genericType, new object[] { gridView, exportOptions }) as IClipboardManager<TCol, TRow>;
			return manager;
		}
	}
}
