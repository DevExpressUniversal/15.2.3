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
using System.IO;
using System.CodeDom.Compiler;
using DevExpress.Snap.Core.Native.Data;
namespace DevExpress.Snap.Core.Native.Data.Implementations {
	[GeneratedCode("Suppress FxCop check", "")]
	public class PathParser {
	internal const int _EOF = 0;
	internal const int _Text = 1;
	internal const int _OpenParenthesis = 2;
	internal const int _CloseParenthesis = 3;
	internal const int _OpenBracket = 4;
	internal const int _CloseBracket = 5;
	internal const int _Slash = 6;
	internal const int _Dot = 7;
	internal const int _Comma = 8;
	internal const int maxT = 9;
		const bool T = true;
		const bool x = false;
		const int minErrDist = 2;
		public StringScanner scanner;
		public Errors errors;
		public PathToken t;	
		public PathToken la;   
		int errDist = minErrDist;
		FieldPathInfo result;
	public FieldPathInfo GetResult()
	{
	  return result;
	}
		public PathParser(StringScanner scanner) {
			this.scanner = scanner;
			errors = new Errors();
		}
		void SynErr(int n) {
			if (errDist >= minErrDist)
				errors.SynErr(0, 0, n);
			errDist = 0;
		}
		public void SemErr(string msg) {
			if (errDist >= minErrDist)
				errors.SemErr(0, 0, msg);
			errDist = 0;
		}
		void Get() {
			for (; ; ) {
				t = la;
				la = scanner.Scan();
				if (la.Kind <= maxT) { ++errDist; break; }
				la = t;
			}
		}
		void Expect(int n) {
			if (la.Kind == n)
				Get();
			else { SynErr(n); }
		}
		bool StartOf(int s) {
			return set[s, la.Kind];
		}
		void ExpectWeak(int n, int follow) {
			if (la.Kind == n)
				Get();
			else {
				SynErr(n);
				while (!StartOf(follow))
					Get();
			}
		}
		bool WeakSeparator(int n, int syFol, int repFol) {
			int kind = la.Kind;
			if (kind == n) { Get(); return true; }
			else if (StartOf(repFol)) { return false; }
			else {
				SynErr(n);
				while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
					Get();
					kind = la.Kind;
				}
				return StartOf(syFol);
			}
		}
			void FieldPath() {
		FieldPathInfoCore(out result);
	}
	void FieldPathInfoCore(out FieldPathInfo pathInfo) {
		pathInfo = new FieldPathInfo(); 
		if (la.Kind == 6) {
			AbsolutePathInfo(pathInfo);
		} else if (la.Kind == 1 || la.Kind == 4 || la.Kind == 7) {
			RelativePathInfo(pathInfo, 0);
		} else SynErr(10);
	}
	void AbsolutePathInfo(FieldPathInfo pathInfo) {
		pathInfo.DataMemberInfo = new FieldPathDataMemberInfo();
		Expect(6);
		if (la.Kind == 1) {
			DataSourceName(pathInfo);
			while (la.Kind == 4) {
				Get();
				if (la.Kind == 2) {
					GroupProperties(pathInfo.DataMemberInfo);
				}
				if (StartOf(1)) {
					Filter(pathInfo.DataMemberInfo);
				}
				pathInfo.DataMemberInfo.AddEmptyFilterIfNeeded(); 
				Expect(5);
			}
		} else if (la.Kind == 4) {
			pathInfo.DataSourceInfo = new RootFieldDataSourceInfo(String.Empty); 
			Get();
			if (la.Kind == 2) {
				GroupProperties(pathInfo.DataMemberInfo);
			}
			if (StartOf(1)) {
				Filter(pathInfo.DataMemberInfo);
			}
			pathInfo.DataMemberInfo.AddEmptyFilterIfNeeded(); 
			Expect(5);
			while (la.Kind == 4) {
				Get();
				if (la.Kind == 2) {
					GroupProperties(pathInfo.DataMemberInfo);
				}
				if (StartOf(1)) {
					Filter(pathInfo.DataMemberInfo);
				}
				pathInfo.DataMemberInfo.AddEmptyFilterIfNeeded(); 
				Expect(5);
			}
		} else SynErr(11);
		if (la.Kind == 7) {
			Get();
			RelativePathCore(pathInfo.DataMemberInfo);
		}
	}
	void RelativePathInfo(FieldPathInfo pathInfo, int relativeLevel) {
		if (la.Kind == 7) {
			RelativePrefix();
			RelativePathInfo(pathInfo, relativeLevel + 1);
		} else if (la.Kind == 1 || la.Kind == 4) {
			pathInfo.DataSourceInfo = new RelativeFieldDataSourceInfo(relativeLevel); 
			pathInfo.DataMemberInfo = new FieldPathDataMemberInfo();
			RelativePathCore(pathInfo.DataMemberInfo);
		} else SynErr(12);
	}
	void DataSourceName(FieldPathInfo info) {
		Expect(1);
		info.DataSourceInfo = new RootFieldDataSourceInfo(t.Value); 
	}
	void GroupProperties(FieldPathDataMemberInfo info) {
		GroupProperties groupProperties = new GroupProperties();
		Expect(2);
		if (la.Kind == 1) {
			TemplateHeaderSwitch(groupProperties);
		}
		Expect(8);
		if (la.Kind == 1) {
			TemplateFooterSwitch(groupProperties);
		}
		Expect(8);
		if (la.Kind == 1) {
			TemplateSeparatorSwitch(groupProperties);
		}
		Expect(8);
		GroupFields(groupProperties);
		while (la.Kind == 8) {
			Get();
			GroupFields(groupProperties);
		}
		Expect(3);
		info.AddGroup(groupProperties);
		if (la.Kind == 2) {
			GroupProperties(info);
		}
	}
	void Filter(FieldPathDataMemberInfo info) {
		string text = String.Empty; 
		if (la.Kind == 6) {
			Get();
			BalancedBracketText(ref text);
			info.AddFilter(text, true); 
		} else if (StartOf(2)) {
			SimpleFilterTextStart(ref text);
			if (StartOf(3)) {
				BalancedBracketText(ref text);
			}
			info.AddFilter(text, true); 
		} else SynErr(13);
	}
	void RelativePathCore(FieldPathDataMemberInfo info) {
		if (la.Kind == 1) {
			Get();
			info.AddFieldName(FieldPathService.DecodePath(t.Value)); 
			while (la.Kind == 4) {
				Get();
				if (la.Kind == 2) {
					GroupProperties(info);
				}
				if (StartOf(1)) {
					Filter(info);
				}
				info.AddEmptyFilterIfNeeded(); 
				Expect(5);
			}
		} else if (la.Kind == 4) {
			Get();
			if (la.Kind == 2) {
				GroupProperties(info);
			}
			if (StartOf(1)) {
				Filter(info);
			}
			info.AddEmptyFilterIfNeeded(); 
			Expect(5);
			while (la.Kind == 4) {
				Get();
				if (la.Kind == 2) {
					GroupProperties(info);
				}
				if (StartOf(1)) {
					Filter(info);
				}
				info.AddEmptyFilterIfNeeded(); 
				Expect(5);
			}
		} else SynErr(14);
		if (la.Kind == 7) {
			Get();
			RelativePathCore(info);
		}
	}
	void RelativePrefix() {
		Expect(7);
		Expect(7);
		Expect(6);
	}
	void TemplateHeaderSwitch(GroupProperties groupProperties) {
		Expect(1);
		groupProperties.TemplateHeaderSwitch = t.Value; 
	}
	void TemplateFooterSwitch(GroupProperties groupProperties) {
		Expect(1);
		groupProperties.TemplateFooterSwitch = t.Value; 
	}
	void TemplateSeparatorSwitch(GroupProperties groupProperties) {
		Expect(1);
		groupProperties.TemplateSeparatorSwitch = t.Value; 
	}
	void GroupFields(GroupProperties groupProperties) {
		string fieldName; 
		if (la.Kind == 1) {
			GroupFieldName(out fieldName);
			GroupFieldInfo fieldInfo = groupProperties.AddField(fieldName); 
		} else if (la.Kind == 2) {
			Get();
			GroupFieldInfo fieldInfo = null; 
			if (la.Kind == 1) {
				GroupFieldName(out fieldName);
				fieldInfo = groupProperties.AddField(fieldName); 
			} else if (la.Kind == 3 || la.Kind == 8) {
				fieldInfo = groupProperties.AddField(String.Empty); 
			} else SynErr(15);
			if (la.Kind == 8) {
				Get();
				FieldGroupParameters(fieldInfo);
			}
			Expect(3);
		} else SynErr(16);
	}
	void GroupFieldName(out string fieldName) {
		Expect(1);
		fieldName = t.Value; 
		while (la.Kind == 7) {
			Get();
			Expect(1);
			fieldName += "." + t.Value; 
		}
	}
	void FieldGroupParameters(GroupFieldInfo fieldInfo) {
		SortingParameters(fieldInfo);
		if (la.Kind == 8) {
			Get();
			GroupIntervalParameters(fieldInfo);
		}
	}
	void SortingParameters(GroupFieldInfo fieldInfo) {
		SortDirection(fieldInfo);
	}
	void GroupIntervalParameters(GroupFieldInfo fieldInfo) {
		Expect(1);
		fieldInfo.GroupInterval = FieldPathService.ParseGroupInterval(t.Value); 
	}
	void SortDirection(GroupFieldInfo fieldInfo) {
		Expect(1);
		fieldInfo.SortOrder = FieldPathService.ParseSortOrder(t.Value); 
	}
	void BalancedBracketText(ref string text) {
		if (StartOf(4)) {
			FilterText(ref text);
		} else if (la.Kind == 4) {
			Get();
			text += t.Value; 
			if (StartOf(3)) {
				BalancedBracketText(ref text);
			}
			Expect(5);
			text += t.Value; 
		} else SynErr(17);
		while (StartOf(3)) {
			if (StartOf(4)) {
				FilterText(ref text);
			} else {
				Get();
				text += t.Value; 
				if (StartOf(3)) {
					BalancedBracketText(ref text);
				}
				Expect(5);
				text += t.Value; 
			}
		}
	}
	void SimpleFilterTextStart(ref string text) {
		if (la.Kind == 1) {
			Get();
		} else if (la.Kind == 3) {
			Get();
		} else if (la.Kind == 7) {
			RelativePrefix();
		} else if (la.Kind == 8) {
			Get();
		} else SynErr(18);
		text += t.Value; 
	}
	void FilterText(ref string text) {
		if (StartOf(2)) {
			SimpleFilterTextStart(ref text);
		} else if (la.Kind == 2 || la.Kind == 6) {
			if (la.Kind == 2) {
				Get();
			} else {
				Get();
			}
			text += t.Value; 
		} else SynErr(19);
	}
		public void Parse() {
			la = new PathToken();
			la.Value = "";
			Get();
					FieldPath();
			Expect(0);
		}
		bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x},
		{x,T,x,T, x,x,T,T, T,x,x},
		{x,T,x,T, x,x,x,T, T,x,x},
		{x,T,T,T, T,x,T,T, T,x,x},
		{x,T,T,T, x,x,T,T, T,x,x}
					  };
	} 
	[GeneratedCode("Suppress FxCop check", "")]
	public class Errors {
		public int count = 0;									
		public System.IO.TextWriter errorStream = new StringWriter();   
		public string errMsgFormat = "-- line {0} col {1}: {2}"; 
		public void SynErr(int line, int col, int n) {
			string s;
			switch (n) {
							case 0: s = "EOF expected"; break;
			case 1: s = "Text expected"; break;
			case 2: s = "OpenParenthesis expected"; break;
			case 3: s = "CloseParenthesis expected"; break;
			case 4: s = "OpenBracket expected"; break;
			case 5: s = "CloseBracket expected"; break;
			case 6: s = "Slash expected"; break;
			case 7: s = "Dot expected"; break;
			case 8: s = "Comma expected"; break;
			case 9: s = "??? expected"; break;
			case 10: s = "invalid FieldPathInfoCore"; break;
			case 11: s = "invalid AbsolutePathInfo"; break;
			case 12: s = "invalid RelativePathInfo"; break;
			case 13: s = "invalid Filter"; break;
			case 14: s = "invalid RelativePathCore"; break;
			case 15: s = "invalid GroupFields"; break;
			case 16: s = "invalid GroupFields"; break;
			case 17: s = "invalid BalancedBracketText"; break;
			case 18: s = "invalid SimpleFilterTextStart"; break;
			case 19: s = "invalid FilterText"; break;
				default: s = "error " + n; break;
			}
			errorStream.WriteLine(errMsgFormat, line, col, s);
			count++;
		}
		public void SemErr(int line, int col, string s) {
			errorStream.WriteLine(errMsgFormat, line, col, s);
			count++;
		}
		public void SemErr(string s) {
			errorStream.WriteLine(s);
			count++;
		}
		public void Warning(int line, int col, string s) {
			errorStream.WriteLine(errMsgFormat, line, col, s);
		}
		public void Warning(string s) {
			errorStream.WriteLine(s);
		}
	} 
	public class FatalError : Exception {
		public FatalError(string m) : base(m) { }
	}
}
