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
using System.Linq;
using System.Xml.Linq;
using DevExpress.Utils;
namespace DevExpress.DataAccess.Native {
	public class DataComponentHelper {
		const string typeNameAttribure = "TypeName";
		readonly string rootElementName;
		public DataComponentHelper(string rootElementName) {
			this.rootElementName = rootElementName;
		}
		public DataComponentHelper() : this("DataSource") {
		}
		public IDataComponent LoadFromXml(XElement e) {
#if DXPORTABLE
			string typeName = (string)e.Attribute(typeNameAttribure);
#else
			string typeName = e.GetAttributeValue(typeNameAttribure);
#endif
			Type type = Type.GetType(typeName);
			IDataComponent dataComponent = Activator.CreateInstance(type) as IDataComponent;
			if(dataComponent == null)
				throw new InvalidOperationException();
			dataComponent.LoadFromXml(e.Elements().First());
			return dataComponent;
		}
		public XElement SaveToXml(IDataComponent dataComponent) {
			Guard.ArgumentNotNull(dataComponent, "dataComponent");
			XElement rootEl = new XElement(rootElementName);
			rootEl.Add(new XAttribute(typeNameAttribure, dataComponent.GetType().FullName));
			rootEl.Add(dataComponent.SaveToXml());
			return rootEl;
		}
	}
}
