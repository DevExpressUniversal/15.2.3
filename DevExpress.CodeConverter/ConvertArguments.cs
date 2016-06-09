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
using System.Linq;
using System.Text;
namespace DevExpress.CodeConverter {
	public class ConvertArguments {
		List<string> modes;
		ConvertResolver resolver;
		public ConvertArguments()
			: this(null, null) {
		}
		public ConvertArguments(ConvertResolver resolver)
			: this(resolver, null) {
		}
		public ConvertArguments(IEnumerable<string> modes)
			: this(null, modes) {
		}
		public ConvertArguments(ConvertResolver resolver, IEnumerable<string> modes) {
			this.resolver = resolver;
			AddModes(modes);
		}
		public void AddModes(IEnumerable<string> modes) {
			if (modes == null)
				return;
			if (this.modes == null)
				this.modes = new List<string>();
			this.modes.AddRange(modes);
		}
		public ConvertResolver Resolver {
			get {
				if (resolver == null)
					resolver = new DefaultConvertResolver(true);
				return resolver;
			}
		}
		public IEnumerable<string> Modes {
			get {
				if (modes == null || modes.Count == 0)
					modes = new List<string>() { string.Empty };
				return modes;
			}
		}
	}
}
