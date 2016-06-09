#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Xml.Linq;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon.Native {
	public abstract class ClassXmlSerializer<TClass, TBaseClass> : XmlSerializer<TBaseClass>
		where TBaseClass : class
		where TClass : class, TBaseClass, new() {
		protected ClassXmlSerializer(string name)
			: base(name) {
		}
		public override XElement SaveToXml(TBaseClass obj, object context) {
			XElement element = new XElement(Name);
			SaveToXmlInternal(element, obj, context);
			return element;
		}
		public override TBaseClass LoadFromXml(XElement element, object context) {
			TBaseClass obj = new TClass();
			LoadFromXmlInternal(element, obj, context);
			return obj;
		}
		public override bool Check(TBaseClass obj) {
			return obj.GetType() == typeof(TClass);
		}
		protected abstract void SaveToXmlInternal(XElement element, TBaseClass obj, object context);
		protected abstract void LoadFromXmlInternal(XElement element, TBaseClass obj, object context);
	}
}
