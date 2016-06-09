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

using System.Windows;
namespace DevExpress.Xpf.Printing.Parameters {
	public partial class EditorTemplates : ResourceDictionary {
		static readonly EditorTemplates instance = new EditorTemplates();
		public static Style DescriptionDefaultStyle { get { return (Style)instance["descriptionLabelStyle"]; } }
		public static DataTemplate StringTemplate { get { return (DataTemplate)instance["stringEditorTemplate"]; } }
		public static DataTemplate NumericTemplate { get { return (DataTemplate)instance["numericEditorTemplate"]; } }
		public static DataTemplate NumericFloatTemplate { get { return (DataTemplate)instance["numericFloatEditorTemplate"]; } }
		public static DataTemplate BooleanTemplate { get { return (DataTemplate)instance["booleanEditorTemplate"]; } }
		public static DataTemplate DateTimeTemplate { get { return (DataTemplate)instance["dateTimeEditorTemplate"]; } }
		public static DataTemplate LookUpEditTemplate { get { return (DataTemplate)instance["lookUpEditTemplate"]; } }
		public static DataTemplate GuidTemplate { get { return (DataTemplate)instance["guidTemplate"]; } }
		public static DataTemplate MultiValueTemplate { get { return (DataTemplate)instance["multiValueTemplate"]; } }
		public static DataTemplate MultiValueLookUpTemplate { get { return (DataTemplate)instance["multiValueLookUpTemplate"]; } }
		public EditorTemplates() {
			InitializeComponent();
		}
	}
}
