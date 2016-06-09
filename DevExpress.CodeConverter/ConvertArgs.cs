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
using System.ComponentModel;
using System.ComponentModel.Composition;
using DevExpress.CodeParser;
namespace DevExpress.CodeConverter {
	public class ConvertArgs {
		internal ConvertArgs(LanguageElement elementForConverting, ConvertResolver resolver) {
			ElementForConverting = elementForConverting;
			Resolver = resolver;
		}
		public LanguageElement ElementForConverting { get; set; }
		public ICollection<LanguageElement> PrecendingElements { get; set; }
		public LanguageElement NewElement { get; set; }
		public bool NodesAndDetailsHandled { get; set; }
		public ConvertResolver Resolver { get; private set;}
	}
	public interface IConvert {
		string From { get; }
		string To { get; }
		string Mode { get; }
	}
	[MetadataAttribute, AttributeUsage(AttributeTargets.Class)]
	public class ConvertLanguageAttribute : InheritedExportAttribute, IConvert {
		public ConvertLanguageAttribute(string from, string to, string mode = "")
			: base(typeof(ConvertRule)) {
			From = from;
			To = to;
			Mode = mode;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public string Mode { get; private set; }
	}
}
