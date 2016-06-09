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
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using DevExpress.Data;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.Utils.Serializing;
namespace DevExpress.DataAccess.EntityFramework { 
	public sealed class EFStoredProcedureInfo {
		public class EqualityComparer : IEqualityComparer<EFStoredProcedureInfo[]> {
			public static bool Equals(EFStoredProcedureInfo[] x, EFStoredProcedureInfo[] y) {
				if(x == null && y == null)
					return true;
				if(x == null || y == null)
					return false;
				if(x.Length != y.Length)
					return false;
				for(int index = 0; index < x.Length; index++)
					if(!Equals(x[index], y[index]))
						return false;
				return true;
			}
			public static bool Equals(EFStoredProcedureInfo x, EFStoredProcedureInfo y) {
				if(x == null && y == null)
					return true;
				if(x == null || y == null)
					return false;
				if(!string.Equals(x.Name, y.Name))
					return false;
				int count = x.parameters.Count;
				if(y.parameters.Count != count)
					return false;
				for(int i = 0; i < count; i++) {
					EFParameter xParameter = x.parameters[i];
					EFParameter yParameter = y.parameters[i];
					if(!DataSourceParameterBase.BaseEqualityComparer.Equals(xParameter, yParameter))
						return false;
				}
				return true;
			}
			#region IEqualityComparer<StoredProcedureInfo> Members
			bool IEqualityComparer<EFStoredProcedureInfo[]>.Equals(EFStoredProcedureInfo[] x, EFStoredProcedureInfo[] y) { return Equals(x, y); }
			int IEqualityComparer<EFStoredProcedureInfo[]>.GetHashCode(EFStoredProcedureInfo[] obj) { return 0; }
			#endregion
		}
		internal const string storedProcInfoXml = "StoredProcInfo";
		internal const string storedProcInfoNameXml = "StoredProcInfoName";
		readonly EFParameterCollection parameters;
		[XtraSerializableProperty]
		public string Name { get; set; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("DevExpress.DataAccess.UI.Native.EntityFramework.EFStoredProcedureParametersEditor, " + AssemblyInfo.SRAssemblyDataAccessUI, typeof(UITypeEditor))]
		public EFParameterCollection Parameters {
			get {
				return parameters;
			}
		}
		internal EFStoredProcedureInfoCollection ParentCollection { get; set; }
		public EFStoredProcedureInfo() {
			this.parameters  = new EFParameterCollection();
		}
		public EFStoredProcedureInfo(string name)
			: this() {
			Name = name;
		}
		public EFStoredProcedureInfo(string name, IEnumerable<IParameter> parameters)
			: this(name) {
			if(parameters != null)
				this.parameters.AddRange(parameters.Select(EFParameter.FromIParameter));
		}
		public EFStoredProcedureInfo(EFStoredProcedureInfo other) : this(other.Name, other.Parameters) { }
		internal XElement SaveToXml() {
			XElement element = new XElement(storedProcInfoXml, new XAttribute(storedProcInfoNameXml, Name));
			foreach(EFParameter parameter in parameters)
				element.Add(parameter.SaveToXml());
			return element;
		}
		internal void LoadFromXMl(XElement element) {
			Name = element.GetAttributeValue(storedProcInfoNameXml);
			foreach(XElement elementParameter in element.Elements(QuerySerializer.XML_Parameter)) {
				EFParameter parameter = new EFParameter();
				parameter.LoadFromXMl(elementParameter);
				parameters.Add(parameter);
			}
		}
	}
}
