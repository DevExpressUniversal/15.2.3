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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
namespace DevExpress.DashboardWin.Design {
	public class ImageFileNameEditor : FileNameEditor {
		protected override void InitializeDialog(OpenFileDialog openFileDialog) {
			ImageCodecInfo[] decoders = ImageCodecInfo.GetImageDecoders();
			StringBuilder filterBuilder = new StringBuilder();
			StringBuilder extensionsBuilder = new StringBuilder();
			foreach(ImageCodecInfo info in decoders) {
				filterBuilder.AppendFormat(String.Format("{0} ({1})|{1}|", info.FormatDescription, info.FilenameExtension));
				if(extensionsBuilder.Length > 0)
					extensionsBuilder.Append(";");
				extensionsBuilder.Append(info.FilenameExtension);
			}
			filterBuilder.Append(String.Format("All Image files ({0})|{0}|", extensionsBuilder.ToString()));
			filterBuilder.Append("All files (*.*)|*.*");
			openFileDialog.Filter = filterBuilder.ToString();
			openFileDialog.FilterIndex = decoders.Length + 1;
		}
	}
	public class ImageDataEditor : ImageFileNameEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			try {
				string name = (string)base.EditValue(context, provider, String.Empty);
				return String.IsNullOrEmpty(name) ? value : File.ReadAllBytes(name);
			}
			catch {
				return null;
			}
		}
	}
}
