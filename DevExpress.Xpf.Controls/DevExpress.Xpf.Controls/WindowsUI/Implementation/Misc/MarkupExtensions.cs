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
using System.Windows.Markup;
using DevExpress.Xpf.WindowsUI.Base;
namespace DevExpress.Xpf.WindowsUI {
	[MarkupExtensionReturnType(typeof(Uri))]
	public class RelativeUriExtension : MarkupExtension {
		public RelativeUriExtension() { }
		public RelativeUriExtension(string uriString) {
			UriString = uriString;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new Uri(UriString, UriKind.Relative);
		}
		[ConstructorArgument("uriString")]
		public string UriString { get; set; }
	}
#if SILVERLIGHT
	[MarkupExtensionReturnType(typeof(Type))]
	public class TypeExtension : MarkupExtension {
		private Type _Type;
		private string _TypeName;
		public TypeExtension() {		}
		public TypeExtension(string typeName) {
			if(typeName == null) throw new ArgumentNullException("typeName");
			_TypeName = typeName;
		}
		public TypeExtension(Type type) {
			if(type == null) throw new ArgumentNullException("type");
			_Type = type;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if(_Type == null) {
				IXamlTypeResolver service = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
				if(_TypeName == null || service == null) {
					throw new InvalidOperationException();
				}
				_Type = service.Resolve(this._TypeName);
				if(_Type == null) {
					throw new InvalidOperationException();
				}
			}
			return this._Type;
		}
		[ConstructorArgument("type")]
		public Type Type {
			get { return _Type; }
			set {
				if(value == null) {
					throw new ArgumentNullException("value");
				}
				this._Type = value;
				this._TypeName = null;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string TypeName {
			get {
				return this._TypeName;
			}
			set {
				if(value == null) {
					throw new ArgumentNullException("value");
				}
				this._TypeName = value;
				this._Type = null;
			}
		}
	}
#endif
}
