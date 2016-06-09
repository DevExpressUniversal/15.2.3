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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
namespace DevExpress.Map.Native {
	public class RESTClientCore {
		readonly string uri;
		readonly List<object> parameters;
		readonly Dictionary<string, object> arguments;
		public List<object> Parameters { get { return parameters; } }
		public Dictionary<string, object> Arguments { get { return arguments; } }
		public RESTClientCore(string uri) {
			this.uri = uri;
			this.parameters = new List<object>();
			this.arguments = new Dictionary<string, object>();
		}
		string Format(object value) {
			if(value is int)
				return ((int)value).ToString(CultureInfo.InvariantCulture);
			if(value is float)
				return ((float)value).ToString(CultureInfo.InvariantCulture);
			if(value is double)
				return ((double)value).ToString(CultureInfo.InvariantCulture);
			if(value is bool)
				return ((bool)value) ? "1" : "0";
			if(value is decimal)
				return ((decimal)value).ToString(CultureInfo.InvariantCulture);
			return Convert.ToString(value);
		}
		public Uri CombineUri() {
			string fullUri = uri;
			foreach(var parameter in parameters)
				fullUri += "/" + Format(parameter);
			bool argumentsExists = false;
			foreach(var pair in arguments) {
				if(!argumentsExists) {
					fullUri += "?";
					argumentsExists = true;
				} else
					fullUri += "&";
				fullUri += pair.Key + "=" + Format(pair.Value);
			}
			return new Uri(fullUri);
		}
	}
}
