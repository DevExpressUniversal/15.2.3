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
namespace DevExpress.XtraPrinting.XamlExport {
	public class ResourceMap {
		Dictionary<VisualBrick, string> borderStylesDictionary = new Dictionary<VisualBrick, string>();
		Dictionary<VisualBrick, string> borderDashStylesDictionary = new Dictionary<VisualBrick, string>();
		Dictionary<TextBrick, string> textBlockStylesDictionary = new Dictionary<TextBrick, string>();
		Dictionary<LineBrick, string> lineStylesDictionary = new Dictionary<LineBrick, string>();
		Dictionary<BrickBase, string> imageResourcesDictionary = new Dictionary<BrickBase, string>();
		bool shouldAddCheckBoxTemplates;
		public Dictionary<VisualBrick, string> BorderStylesDictionary { get { return borderStylesDictionary; } }
		public Dictionary<VisualBrick, string> BorderDashStylesDictionary { get { return borderDashStylesDictionary; } }
		public Dictionary<TextBrick, string> TextBlockStylesDictionary { get { return textBlockStylesDictionary; } }
		public Dictionary<LineBrick, string> LineStylesDictionary { get { return lineStylesDictionary; } }
		public Dictionary<BrickBase, string> ImageResourcesDictionary { get { return imageResourcesDictionary; } }
		public bool ShouldAddCheckBoxTemplates { get { return shouldAddCheckBoxTemplates; } set { shouldAddCheckBoxTemplates = value; } }
	}
}
