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
namespace DevExpress.Xpf.Core.Design {
	public class PropertyGenerationInfo {
		public const string CommandPropertyTemplateName = "icmd";
		public const string BindablePropertyTemplateName = "vmprop";
		public static PropertyGenerationInfo ForStandardProperty(string typeName) {
			return new PropertyGenerationInfo(BindablePropertyTemplateName, "PropertyType," + typeName, "PropertyName,MyProperty");
		}
		public static PropertyGenerationInfo ForCommandProperty() {
			return new PropertyGenerationInfo(CommandPropertyTemplateName, string.Empty, "PropertyName,MyCommand");
		}
		PropertyGenerationInfo(string newPropertyTemplateName, string newPropertyTemplateProperties, string newPropertyDefaultTemplateProperties) {
			this.NewPropertyTemplateName = newPropertyTemplateName;
			this.newPropertyTemplateProperties = newPropertyTemplateProperties;
			this.newPropertyDefaultTemplateProperties = newPropertyDefaultTemplateProperties;
		}
		string newPropertyTemplateProperties;
		string newPropertyDefaultTemplateProperties;
		public string NewPropertyTemplateName { get; private set; }
		public string GetTemplatePropertiesForNoInteraction(string propertyName) {
			return GetNewPropertyTemplateProperties() + "PropertyName," + propertyName;
		}
		public string GetTemplatePropertiesForInteraction() {
			return GetNewPropertyTemplateProperties() + newPropertyDefaultTemplateProperties;
		}
		string GetNewPropertyTemplateProperties() {
			return string.IsNullOrEmpty(newPropertyTemplateProperties) ? string.Empty : (newPropertyTemplateProperties + ";");
		}
	}
}
