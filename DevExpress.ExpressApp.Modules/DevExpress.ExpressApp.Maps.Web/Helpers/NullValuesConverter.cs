#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Reflection;
using System.Web.Script.Serialization;
namespace DevExpress.ExpressApp.Maps.Web.Helpers {
	public class NullValuesConverter : JavaScriptConverter {
		private IEnumerable<Type> supportedTypes;
		public NullValuesConverter() {
			supportedTypes = GetType().Assembly.GetTypes();
		}
		public NullValuesConverter(IEnumerable<Type> supportedTypes) {
			this.supportedTypes = supportedTypes;
		}
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer) {
			throw new NotImplementedException();
		}
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer) {
			var jsonExample = new SortedDictionary<string, object>();
			foreach(var prop in obj.GetType().GetProperties()) {
				bool isIgnored = prop.IsDefined(typeof(ScriptIgnoreAttribute), true);
				var value = prop.GetValue(obj, BindingFlags.Public, null, null, null);
				if(value != null && !isIgnored)
					jsonExample.Add(prop.Name, value);
			}
			foreach(var field in obj.GetType().GetFields()) {
				bool isIgnored = field.IsDefined(typeof(ScriptIgnoreAttribute), true);
				var value = field.GetValue(obj);
				if(value != null && !isIgnored)
					jsonExample.Add(field.Name, value);
			}
			return jsonExample;
		}
		public override IEnumerable<Type> SupportedTypes {
			get { return supportedTypes; }
		}
	}
}
