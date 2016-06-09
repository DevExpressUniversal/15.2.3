ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "数値から符号 (+、-) を除いた絶対値を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には絶対値を求める実数を指定します。"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "利息が満期日に支払われる有価証券に対する未収利息を計算します。.",
		arguments: [
			{
				name: "発行日",
				description: "には、証券の発行日を日付のシリアル値で指定します。"
			},
			{
				name: "受渡日",
				description: "には、証券の満期日を日付のシリアル値で指定します。"
			},
			{
				name: "利率",
				description: "には、証券の年利を指定します。"
			},
			{
				name: "額面",
				description: "には、証券の額面価格を指定します。"
			},
			{
				name: "基準",
				description: "には、利息計算の基礎となる日数の計算方法を数値で指定します。"
			}
		]
	},
	{
		name: "ACOS",
		description: "数値のアークコサインを返します。戻り値の角度は、0 (ゼロ)  ～ PI の範囲のラジアンとなります。アークコサインとは、そのコサインが数値であるような角度のことです。.",
		arguments: [
			{
				name: "数値",
				description: "には求める角度のコサインの値を -1 ～ 1 の範囲で指定します。"
			}
		]
	},
	{
		name: "ACOSH",
		description: "数値の双曲線逆余弦を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には 1 以上の実数を指定します。"
			}
		]
	},
	{
		name: "ACOT",
		description: "数値の逆余接を返します。戻り値の角度は、0 ～ Pi の範囲のラジアンとなります。.",
		arguments: [
			{
				name: "数値",
				description: "には、求める角度の余接の値を指定します。"
			}
		]
	},
	{
		name: "ACOTH",
		description: "数値の逆双曲線余接を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には、求める角度の逆双曲線余接の値を指定します。"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "指定したセルの参照を文字列の形式で返します。.",
		arguments: [
			{
				name: "行番号",
				description: "にはセル参照に使用する行番号を指定します。たとえば、1 行目には 1 を指定します。"
			},
			{
				name: "列番号",
				description: "にはセル参照に使用する列番号を指定します。たとえば、D 列には 4 を指定します。"
			},
			{
				name: "参照の種類",
				description: "にはセル参照の種類を指定します。絶対参照の場合は 1、行が絶対参照で列が相対参照の場合は 2、行が相対参照で列が絶対参照の場合は 3、相対参照の場合は 4 を指定します。"
			},
			{
				name: "参照形式",
				description: "セル参照を A1 形式にするか R1C1 形式にするかを論理値で指定します。参照形式に TRUE を指定する、または省略すると、A1 形式のセル参照が返され、FALSE を指定すると、R1C1 形式のセル参照が返されます。"
			},
			{
				name: "シート名",
				description: "には外部参照として使用するワークシートの名前を文字列で指定します。"
			}
		]
	},
	{
		name: "AND",
		description: "すべての引数が TRUE のとき、TRUE を返します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "論理式1",
				description: "には結果が TRUE または FALSE になる、1 ～ 255 個の論理式を指定できます。引数には論理値、配列、または参照を指定します。 "
			},
			{
				name: "論理式2",
				description: "には結果が TRUE または FALSE になる、1 ～ 255 個の論理式を指定できます。引数には論理値、配列、または参照を指定します。 "
			}
		]
	},
	{
		name: "ARABIC",
		description: "ローマ数字をアラビア数字に変換します。.",
		arguments: [
			{
				name: "文字列",
				description: "には変換するローマ数字を指定します。"
			}
		]
	},
	{
		name: "AREAS",
		description: "参照内の領域の個数を返します。連続したセル範囲、または 1 つのセルが領域とみなされます。.",
		arguments: [
			{
				name: "参照",
				description: "にはセルまたはセル範囲への参照を指定します。複数の領域を指定できます。"
			}
		]
	},
	{
		name: "ASIN",
		description: "数値のアークサインを返します。戻り値の角度は、-PI/2 ～ PI/2 の範囲のラジアンとなります。.",
		arguments: [
			{
				name: "数値",
				description: "には求める角度のサインの値を -1 ～ 1 の範囲で指定します。"
			}
		]
	},
	{
		name: "ASINH",
		description: "数値の双曲線逆正弦を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には 1 以上の実数を指定します。"
			}
		]
	},
	{
		name: "ATAN",
		description: "数値のアークタンジェントを返します。戻り値の角度は、-PI/2 ～ PI/2 の範囲のラジアンとなります。.",
		arguments: [
			{
				name: "数値",
				description: "には求める角度のタンジェントの値を指定します。"
			}
		]
	},
	{
		name: "ATAN2",
		description: "指定された x-y 座標のアークタンジェントを返します。戻り値の角度は、-PI から PI (ただし -PI を除く) の範囲のラジアンとなります。.",
		arguments: [
			{
				name: "x座標",
				description: "には x 座標を指定します。"
			},
			{
				name: "y座標",
				description: "には y 座標を指定します。"
			}
		]
	},
	{
		name: "ATANH",
		description: "数値の双曲線逆正接を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には -1 より大きく 1 より小さい実数を指定します。"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "データ全体の平均値に対するそれぞれのデータの絶対偏差の平均を返します。引数には、数値、数値を含む名前、配列、セル参照を指定できます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には絶対偏差の平均を求めたい引数を、1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には絶対偏差の平均を求めたい引数を、1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "引数の平均値を返します。引数には、数値、数値を含む名前、配列、セル参照を指定できます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には平均を求めたい数値を、1 から 255 個まで指定します。"
			},
			{
				name: "数値2",
				description: "には平均を求めたい数値を、1 から 255 個まで指定します。"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "引数の平均値を返します。引数の文字列および FALSE は 0、TRUE は 1 と見なします。引数には、数値、名前、配列、参照を含むことができます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "値1",
				description: "には平均を算出する値を、1 から 255 個まで指定します。"
			},
			{
				name: "値2",
				description: "には平均を算出する値を、1 から 255 個まで指定します。"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "特定の条件に一致する数値の平均 (算術平均) を計算します。.",
		arguments: [
			{
				name: "範囲",
				description: "には、評価の対象となるセル範囲を指定します。"
			},
			{
				name: "条件",
				description: "には、平均の計算対象となるセルを定義する条件を、数値、式、または文字列で指定します。"
			},
			{
				name: "平均対象範囲",
				description: "には、実際に平均を求めるのに使用されるセルを指定します。省略した場合、範囲内のセルが使用されます。"
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "特定の条件に一致する数値の平均 (算術平均) を計算します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "平均対象範囲",
				description: "には、実際に平均を求めるのに使用されるセルを指定します。"
			},
			{
				name: "条件範囲",
				description: "には、特定の条件による評価の対象となるセル範囲を指定します。"
			},
			{
				name: "条件",
				description: "には、平均を求めるのに使用されるセルを定義する条件を、数値、式、または文字列で指定します。"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "数値を文字列 (baht) に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "変換したい数値を指定します"
			}
		]
	},
	{
		name: "BASE",
		description: "数値を特定の基数 (底) を持つテキスト表現に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "には変換する数値を指定します。"
			},
			{
				name: "基数",
				description: "には数値を変換する基数を指定します。"
			},
			{
				name: "最小長",
				description: "には返される文字列の最小長を指定します。先頭の 0 が省略された場合は追加されません。"
			}
		]
	},
	{
		name: "BESSELI",
		description: "修正ベッセル関数 In(x) を返します。.",
		arguments: [
			{
				name: "x",
				description: "には、関数に代入する値を指定します。"
			},
			{
				name: "n",
				description: "には、ベッセル関数の次数を指定します。"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "ベッセル関数 Jn(x) を返します。.",
		arguments: [
			{
				name: "x",
				description: "には、関数に代入する値を指定します。"
			},
			{
				name: "n",
				description: "には、ベッセル関数の次数を指定します。"
			}
		]
	},
	{
		name: "BESSELK",
		description: "修正ベッセル関数 Kn(x) を返します。.",
		arguments: [
			{
				name: "x",
				description: "には、関数に代入する値を指定します。"
			},
			{
				name: "n",
				description: "には、ベッセル関数の次数を指定します。"
			}
		]
	},
	{
		name: "BESSELY",
		description: "ベッセル関数 Yn(x) を返します。.",
		arguments: [
			{
				name: "x",
				description: "には、関数に代入する値を指定します。"
			},
			{
				name: "n",
				description: "には、ベッセル関数の次数を指定します。"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "β確率分布関数を返します。.",
		arguments: [
			{
				name: "x",
				description: "には区間 A ～ B の範囲で、関数に代入する値を指定します。"
			},
			{
				name: "α",
				description: "には分布に対するパラメーターを指定します。0 より大きい値を指定する必要があります。"
			},
			{
				name: "β",
				description: "には分布に対するパラメーターを指定します。0 より大きい値を指定する必要があります。"
			},
			{
				name: "関数形式",
				description: "には論理値を指定します。TRUE を指定した場合は累積分布関数が返され、FALSE を指定した場合は確率密度関数が返されます。"
			},
			{
				name: "A",
				description: "には x の区間の下限値を指定します。この引数を省略すると A = 0 として計算されます。"
			},
			{
				name: "B",
				description: "には x の区間の上限値を指定します。この引数を省略すると B = 1 として計算されます。"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "累積β確率密度関数の逆関数 (BETA.DIST) を返します。.",
		arguments: [
			{
				name: "確率",
				description: "にはβ確率分布における確率を指定します。"
			},
			{
				name: "α",
				description: "には確率分布のパラメーターを指定します。0 より大きい値を指定する必要があります。"
			},
			{
				name: "β",
				description: "には確率分布のパラメーターを指定します。0 より大きい値を指定する必要があります。"
			},
			{
				name: "A",
				description: "には x の区間の下限値を指定します。この引数を省略すると A = 0 として計算されます。"
			},
			{
				name: "B",
				description: "には x の区間の下限値を指定します。この引数を省略すると B = 1 として計算されます。"
			}
		]
	},
	{
		name: "BETADIST",
		description: "累積β確率密度関数を返します。.",
		arguments: [
			{
				name: "x",
				description: "には区間 A ～ B の範囲で、関数に代入する値を指定します。"
			},
			{
				name: "α",
				description: "には確率分布に対するパラメーターを指定します。0 より大きい値を指定する必要があります。"
			},
			{
				name: "β",
				description: "には確率分布に対するパラメーターを指定します。0 より大きい値を指定する必要があります。"
			},
			{
				name: "A",
				description: "には x の区間の下限値を指定します。この引数を省略すると A = 0 として計算されます。"
			},
			{
				name: "B",
				description: "には x の区間の下限値を指定します。この引数を省略すると B = 1 として計算されます。"
			}
		]
	},
	{
		name: "BETAINV",
		description: "累積β確率密度関数の逆関数を返します。.",
		arguments: [
			{
				name: "確率",
				description: "にはβ確率分布における確率を指定します。"
			},
			{
				name: "α",
				description: "には確率分布のパラメーターを指定します。0 より大きい値を指定する必要があります。"
			},
			{
				name: "β",
				description: "には確率分布のパラメーターを指定します。0 より大きい値を指定する必要があります。"
			},
			{
				name: "A",
				description: "には x の区間の下限値を指定します。この引数を省略すると A = 0 として計算されます。"
			},
			{
				name: "B",
				description: "には x の区間の下限値を指定します。この引数を省略すると B = 1 として計算されます。"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "2 進数を 10 進数に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "変換したい 2 進数を指定します。"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "2 進数を 16 進数に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "変換したい 2 進数を指定します。"
			},
			{
				name: "桁数",
				description: "使用する文字数 (桁数) を指定します。"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "2 進数を 8 進数に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "変換したい 2 進数を指定します。"
			},
			{
				name: "桁数",
				description: "使用する文字数 (桁数) を指定します。"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "二項分布の確率を返します。.",
		arguments: [
			{
				name: "成功数",
				description: "には試行回数中の成功の回数を指定します。"
			},
			{
				name: "試行回数",
				description: "には独立試行の回数を指定します。"
			},
			{
				name: "成功率",
				description: "には試行 1 回あたりの成功率を指定します。"
			},
			{
				name: "関数形式",
				description: "には関数を示す論理値を指定します。TRUE を指定した場合は累積分布関数が返され、FALSE を指定した場合は確率密度関数が返されます。"
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "二項分布を使用した試行結果の確率を返します。.",
		arguments: [
			{
				name: "試行回数",
				description: "には独立試行の回数を指定します。"
			},
			{
				name: "成功率",
				description: "には試行 1 回あたりの成功率を指定します。"
			},
			{
				name: "成功数",
				description: "には試行回数中の成功の回数を指定します。"
			},
			{
				name: "成功数2",
				description: "この関数を指定した場合、成功試行数が成功数と成功数2 の間になる確率を返します。"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "累積二項分布の値が基準値以上になるような最小の値を返します。.",
		arguments: [
			{
				name: "試行回数",
				description: "にはベルヌーイ試行の回数を指定します。"
			},
			{
				name: "成功率",
				description: "には試行 1 回あたりの成功率 (0 ～ 1 の数値) を指定します。"
			},
			{
				name: "α",
				description: "基準値 (0 ～ 1 の数値) を指定します。"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "二項分布の確率を返します。.",
		arguments: [
			{
				name: "成功数",
				description: "には試行回数中の成功の回数を指定します。"
			},
			{
				name: "試行回数",
				description: "には独立試行の回数を指定します。"
			},
			{
				name: "成功率",
				description: "には試行 1 回あたりの成功率を指定します。"
			},
			{
				name: "関数形式",
				description: "には関数の形式を表す論理値を指定します。TRUE を指定した場合、累積分布関数が返されます。FALSE を指定した場合、確率密度関数が返されます。"
			}
		]
	},
	{
		name: "BITAND",
		description: "2 つの数値のビット単位の 'And' を返します。.",
		arguments: [
			{
				name: "数値1",
				description: "には評価する 2 進数の 10 進表現を指定します。"
			},
			{
				name: "数値2",
				description: "には評価する 2 進数の 10 進表現を指定します。"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "左に移動数ビット移動する数値を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には評価する 2 進数の 10 進表現を指定します。"
			},
			{
				name: "移動数",
				description: "には数値を左に移動するビット数を指定します"
			}
		]
	},
	{
		name: "BITOR",
		description: "2 つの数値のビット単位の 'Or' を返します。.",
		arguments: [
			{
				name: "数値1",
				description: "には評価する 2 進数の 10 進表現を指定します。"
			},
			{
				name: "数値2",
				description: "には評価する 2 進数の 10 進表現を指定します。"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "右に移動数ビット移動する数値を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には評価する 2 進数の 10 進表現を指定します。"
			},
			{
				name: "移動数",
				description: "には数値を右に移動するビット数を指定します"
			}
		]
	},
	{
		name: "BITXOR",
		description: "2 つの数値のビット単位の 'Exclusive Or' を返します。.",
		arguments: [
			{
				name: "数値1",
				description: "には評価する 2 進数の 10 進表現を指定します。"
			},
			{
				name: "数値2",
				description: "には評価する 2 進数の 10 進表現を指定します。"
			}
		]
	},
	{
		name: "CEILING",
		description: "指定された基準値の倍数のうち、最も近い値に数値を切り上げます。.",
		arguments: [
			{
				name: "数値",
				description: "には対象となる数値を指定します。"
			},
			{
				name: "基準値",
				description: "には倍数の基準となる数値を指定します。"
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "数値を最も近い整数、または最も近い基準値の倍数に切り上げます。.",
		arguments: [
			{
				name: "数値",
				description: "には対象となる値を指定します。"
			},
			{
				name: "基準値",
				description: "には倍数の基準となる数値を指定します。"
			},
			{
				name: "モード",
				description: "指定され、かつ 0 以外の場合、この関数は 0 とは逆の方向に切り上げます。"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "指定された基準値の倍数のうち、最も近い値に数値を切り上げます。.",
		arguments: [
			{
				name: "数値",
				description: "には対象となる数値を指定します。"
			},
			{
				name: "基準値",
				description: "には倍数の基準となる数値を指定します。"
			}
		]
	},
	{
		name: "CELL",
		description: "シートの読み取り順で、参照の最初のセルの書式設定、位置、内容に関する情報を返します。.",
		arguments: [
			{
				name: "検査の種類",
				description: "には調べるセル情報の種類を表す文字列値を指定します。"
			},
			{
				name: "参照",
				description: "には調べるセルを指定します。"
			}
		]
	},
	{
		name: "CHAR",
		description: "使っているコンピューターの文字セットから、そのコード番号に対応する文字を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には変換する文字を表す 1 ～ 255 の範囲の数値を指定します。"
			}
		]
	},
	{
		name: "CHIDIST",
		description: "カイ 2 乗分布の右側確率の値を返します。.",
		arguments: [
			{
				name: "x",
				description: "には分布を評価する値 (正の数値) を指定します。"
			},
			{
				name: "自由度",
				description: "には自由度 (10^10 を除く 1 ～ 10^10 の間の数値) を指定します。"
			}
		]
	},
	{
		name: "CHIINV",
		description: "カイ 2 乗分布の右側確率の逆関数の値を返します。.",
		arguments: [
			{
				name: "確率",
				description: "にはカイ 2 乗分布における確率 (0 ～ 1 の値) を指定します。"
			},
			{
				name: "自由度",
				description: "には自由度 (10^10 を除く 1 ～ 10^10 の間の数値) を指定します。"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "カイ 2 乗分布の左側確率の値を返します。.",
		arguments: [
			{
				name: "x",
				description: "には分布を評価する値 (正の数値) を指定します。"
			},
			{
				name: "自由度",
				description: "には自由度 (10^10 を除く 1 ～ 10^10 の間の数値) を指定します。"
			},
			{
				name: "関数形式",
				description: "には計算に使用する関数の形式を表す論理値を指定します。TRUE の場合は累積分布関数、FALSE の場合は確率密度関数が返されます。"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "カイ 2 乗分布の右側確率の値を返します。.",
		arguments: [
			{
				name: "x",
				description: "には分布を評価する値 (正の数値) を指定します。"
			},
			{
				name: "自由度",
				description: "には自由度 (10^10 を除く 1 ～ 10^10 の間の数値) を指定します。"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "カイ 2 乗分布の左側確率の逆関数の値を返します。.",
		arguments: [
			{
				name: "確率",
				description: "にはカイ 2 乗分布における確率 (0 ～ 1 の値) を指定します。"
			},
			{
				name: "自由度",
				description: "には自由度 (10^10 を除く 1 ～ 10^10 の間の数値) を指定します。"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "カイ 2 乗分布の右側確率の逆関数の値を返します。.",
		arguments: [
			{
				name: "確率",
				description: "にはカイ 2 乗分布における確率 (0 ～ 1 の値) を指定します。"
			},
			{
				name: "自由度",
				description: "には自由度 (10^10 を除く 1 ～ 10^10 の間の数値) を指定します。"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "統計と自由度に対するカイ 2 乗分布から値を抽出して返します。.",
		arguments: [
			{
				name: "実測値範囲",
				description: "には期待値に対して検定される実測値が入力されている範囲を指定します。"
			},
			{
				name: "期待値範囲",
				description: "には総計に対する行の合計と列の合計の積の割合が入力されている範囲を指定します。"
			}
		]
	},
	{
		name: "CHITEST",
		description: "統計と自由度に対するカイ 2 乗分布から値を抽出して返します。.",
		arguments: [
			{
				name: "実測値範囲",
				description: "には期待値に対して検定される実測値が入力した範囲を指定します。"
			},
			{
				name: "期待値範囲",
				description: "には総計に対する行の合計と列の合計の積の割合が入力されている範囲を指定します。"
			}
		]
	},
	{
		name: "CHOOSE",
		description: "インデックスを使って、引数リストから特定の値または動作を 1 つ選択します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "インデックス",
				description: "には引数リストの何番目の値を選択するかを指定します。インデックスには、1  ～ 254 までの数値、または 1 ～ 254 までの数値を返す数式またはセル参照を指定します。"
			},
			{
				name: "値1",
				description: "には 1 ～ 254 個の引数 (数値、セル参照、名前、数式、関数、文字列) を指定します。ここからインデックスで指定した値が返されます。"
			},
			{
				name: "値2",
				description: "には 1 ～ 254 個の引数 (数値、セル参照、名前、数式、関数、文字列) を指定します。ここからインデックスで指定した値が返されます。"
			}
		]
	},
	{
		name: "CLEAN",
		description: "印刷できない文字を文字列から削除します。.",
		arguments: [
			{
				name: "文字列",
				description: "には印刷できない文字を削除するワークシートの文字データを指定します。"
			}
		]
	},
	{
		name: "CODE",
		description: "文字列の先頭文字を表す数値コードを返します。.",
		arguments: [
			{
				name: "文字列",
				description: "には先頭文字の数値コードを調べたい文字列を指定します。"
			}
		]
	},
	{
		name: "COLUMN",
		description: "参照の列番号を返します。.",
		arguments: [
			{
				name: "参照",
				description: "には列番号を調べるセルまたはセル範囲を指定します。範囲を省略すると、COLUMN 関数が入力されているセルの列番号が返されます。"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "配列または参照の列数を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には列数を計算する配列、配列数式またはセル範囲の参照を指定します。"
			}
		]
	},
	{
		name: "COMBIN",
		description: "すべての項目から指定された個数を選択するときの組み合わせの数を返します。.",
		arguments: [
			{
				name: "総数",
				description: "には抜き取る対象の全体の数を指定します。"
			},
			{
				name: "抜き取り数",
				description: "には抜き取る組み合わせ 1 組に含まれる項目の数を指定します。"
			}
		]
	},
	{
		name: "COMBINA",
		description: "すべての項目から指定された個数を選択するときの組み合わせ (反復あり) の数を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には抜き取る対象の全体の数を指定します。"
			},
			{
				name: "抜き取り数",
				description: "には抜き取る組み合わせ 1 組に含まれる項目の数を指定します。"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "指定した実数係数および虚数係数を複素数に変換します。.",
		arguments: [
			{
				name: "実数",
				description: "には、複素数の実数係数を指定します。"
			},
			{
				name: "虚数",
				description: "には、複素数の虚数係数を指定します。"
			},
			{
				name: "虚数単位",
				description: "には、複素数の虚数単位を表す文字を指定します。"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "複数の文字列を結合して 1 つの文字列にまとめます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "文字列1",
				description: "には 1 つにまとめる 1 ～ 255 個までの文字列を指定できます。引数には文字列、数値、または単一セルの参照を指定します。"
			},
			{
				name: "文字列2",
				description: "には 1 つにまとめる 1 ～ 255 個までの文字列を指定できます。引数には文字列、数値、または単一セルの参照を指定します。"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "正規分布を使用して、母集団の平均に対する信頼区間を求めます。.",
		arguments: [
			{
				name: "α",
				description: "には信頼度を計算するために使用する有意水準 (0 より大きく 1 未満の数値) を指定します。"
			},
			{
				name: "標準偏差",
				description: "にはデータ範囲に対する母集団の標準偏差を指定します。これは、既知であると仮定されます。"
			},
			{
				name: "標本数",
				description: "には標本数を指定します。"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "正規分布を使用して、母集団の平均に対する信頼区間を求めます。.",
		arguments: [
			{
				name: "α",
				description: "には信頼度を計算するために使用する有意水準 (0 より大きく 1 未満の数値) を指定します。"
			},
			{
				name: "標準偏差",
				description: "にはデータ範囲に対する母集団の標準偏差を指定します。これは、既知であると仮定されます。"
			},
			{
				name: "標本数",
				description: "には標本数を指定します。"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "スチューデントの T 分布を使用して、母集団の平均に対する信頼区間を求めます。.",
		arguments: [
			{
				name: "α",
				description: "には信頼度を計算するために使用する有意水準 (0 より大きく 1 未満の数値) を指定します。"
			},
			{
				name: "標準偏差",
				description: "にはデータ範囲に対する母集団の標準偏差を指定します。これは、既知であると仮定されます。"
			},
			{
				name: "標本数",
				description: "には標本数を指定します。"
			}
		]
	},
	{
		name: "CONVERT",
		description: "数値の単位を変換します。.",
		arguments: [
			{
				name: "数値",
				description: "には、変換前の単位で数値を指定します。"
			},
			{
				name: "変換前単位",
				description: "には、変換前の単位を指定します。"
			},
			{
				name: "変換後単位",
				description: "には、変換後の単位を指定します。"
			}
		]
	},
	{
		name: "CORREL",
		description: "2 つの配列の相関係数を返します。.",
		arguments: [
			{
				name: "配列1",
				description: "には値 (数値、名前、配列、数値を含むセル参照) のセル範囲を指定します。"
			},
			{
				name: "配列2",
				description: "には値 (数値、名前、配列、数値を含むセル参照) の 2 番目のセル範囲を指定します。"
			}
		]
	},
	{
		name: "COS",
		description: "角度のコサインを返します。.",
		arguments: [
			{
				name: "数値",
				description: "にはコサインを求める角度をラジアンを単位として指定します。"
			}
		]
	},
	{
		name: "COSH",
		description: "数値の双曲線余弦を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には実数を指定します。"
			}
		]
	},
	{
		name: "COT",
		description: "角度の余接を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には、余接を求める角度をラジアンを単位として指定します。"
			}
		]
	},
	{
		name: "COTH",
		description: "数値の双曲線余接を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には、双曲線余接を求める角度をラジアンを単位として指定します。"
			}
		]
	},
	{
		name: "COUNT",
		description: "範囲内の、数値が含まれるセルの個数を返します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "値1",
				description: "にはデータまたはデータが入力したセルの参照を 1 から 255 個まで指定します。数値データだけがカウントされます。"
			},
			{
				name: "値2",
				description: "にはデータまたはデータが入力したセルの参照を 1 から 255 個まで指定します。数値データだけがカウントされます。"
			}
		]
	},
	{
		name: "COUNTA",
		description: "範囲内の、空白でないセルの個数を返します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "値1",
				description: "にはカウントしたい値およびセルを表す引数を 1 ～ 255 個まで指定します。すべてのデータ型の値が計算の対象となります。"
			},
			{
				name: "値2",
				description: "にはカウントしたい値およびセルを表す引数を 1 ～ 255 個まで指定します。すべてのデータ型の値が計算の対象となります。"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "範囲に含まれる空白セルの個数を返します。.",
		arguments: [
			{
				name: "範囲",
				description: "には空白セルの個数を求めたいセル範囲を指定します。"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "指定された範囲に含まれるセルのうち、検索条件に一致するセルの個数を返します。.",
		arguments: [
			{
				name: "範囲",
				description: "には空白でないセルの個数を求めるセル範囲を指定します。"
			},
			{
				name: "検索条件",
				description: "には計算の対象となるセルを定義する条件を、数値、式、または文字列で指定します。"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "特定の条件に一致するセルの個数を返します。.",
		arguments: [
			{
				name: "検索条件範囲",
				description: "には、特定の条件による評価の対象となるセル範囲を指定します。"
			},
			{
				name: "検索条件",
				description: "には、計算の対象となるセルを定義する条件を、数値、式、または文字列で指定します。"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "利払期間の第 1 日目から受渡日までの日数を計算します。.",
		arguments: [
			{
				name: "受渡日",
				description: "には、証券の受渡日を日付のシリアル値で指定します。"
			},
			{
				name: "満期日",
				description: "には、証券の満期日を日付のシリアル値で指定します。"
			},
			{
				name: "頻度",
				description: "には、年間の利息支払回数を指定します。"
			},
			{
				name: "基準",
				description: "には、利息計算の基礎となる日数の計算方法を数値で指定します。"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "受渡日後の次の利払日を計算します。.",
		arguments: [
			{
				name: "受渡日",
				description: "には、証券の受渡日を日付のシリアル値で指定します。"
			},
			{
				name: "満期日",
				description: "には、証券の満期日を日付のシリアル値で指定します。"
			},
			{
				name: "頻度",
				description: "には、年間の利息支払回数を指定します。"
			},
			{
				name: "基準",
				description: "には、利息計算の基礎となる日数の計算方法を数値で指定します。"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "受渡日と満期日の間の利息支払回数を計算します。.",
		arguments: [
			{
				name: "受渡日",
				description: "には、証券の受渡日を日付のシリアル値で指定します。"
			},
			{
				name: "満期日",
				description: "には、証券の満期日を日付のシリアル値で指定します。"
			},
			{
				name: "頻度",
				description: "には、年間の利息支払回数を指定します。"
			},
			{
				name: "基準",
				description: "には、利息計算の基礎となる日数の計算方法を数値で指定します。"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "受渡日の前の最後の利払日を計算します。.",
		arguments: [
			{
				name: "受渡日",
				description: "には、証券の受渡日を日付のシリアル値で指定します。"
			},
			{
				name: "満期日",
				description: "には、証券の満期日を日付のシリアル値で指定します。"
			},
			{
				name: "頻度",
				description: "には、年間の利息支払回数を指定します。"
			},
			{
				name: "基準",
				description: "には、利息計算の基礎となる日数の計算方法を数値で指定します。"
			}
		]
	},
	{
		name: "COVAR",
		description: "共分散を返します。共分散とは、2 組の対応するデータ間での標準偏差の積の平均値です。.",
		arguments: [
			{
				name: "配列1",
				description: "には整数のデータが入力されている最初のセル範囲を指定します。引数には数値、配列、または数値を含む参照を指定します。"
			},
			{
				name: "配列2",
				description: "には整数のデータが入力されている 2 番目のセル範囲を指定します。引数には数値、配列、または数値を含む参照を指定します。"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "母集団の共分散を返します。共分散とは、2 組の対応するデータ間での標準偏差の積の平均値です。.",
		arguments: [
			{
				name: "配列1",
				description: "には整数のデータが入力されている最初のセル範囲を指定します。引数には数値、配列、または数値を含む参照を指定します。"
			},
			{
				name: "配列2",
				description: "には整数のデータが入力されている 2 番目のセル範囲を指定します。引数には数値、配列、または数値を含む参照を指定します。"
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "標本の共分散を返します。共分散とは、2 組の対応するデータ間での標準偏差の積の平均値です。.",
		arguments: [
			{
				name: "配列1",
				description: "には整数のデータが入力されている最初のセル範囲を指定します。引数には数値、配列、または数値を含む参照を指定します。"
			},
			{
				name: "配列2",
				description: "には整数のデータが入力されている 2 番目のセル範囲を指定します。引数には数値、配列、または数値を含む参照を指定します。"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "累積二項分布の値が基準値以上になるような最小の値を返します。.",
		arguments: [
			{
				name: "試行回数",
				description: "にはベルヌーイ試行の回数を指定します。"
			},
			{
				name: "成功率",
				description: "には試行 1 回あたりの成功率 (0 ～ 1 の数値) を指定します。"
			},
			{
				name: "α",
				description: "基準値 (0 ～ 1 の数値) を指定します。"
			}
		]
	},
	{
		name: "CSC",
		description: "角度の余割を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には、余割を求める角度をラジアンを単位として指定します。"
			}
		]
	},
	{
		name: "CSCH",
		description: "角度の双曲線余割を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には、双曲線余割を求める角度をラジアンを単位として指定します。"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "開始から終了までの貸付期間内で支払われる利息の累計額を計算します。.",
		arguments: [
			{
				name: "利率",
				description: "には、利率を指定します。"
			},
			{
				name: "期間",
				description: "には、支払期間の合計を指定します。"
			},
			{
				name: "現在価値",
				description: "には、現在の貸付け額を指定します。"
			},
			{
				name: "開始期",
				description: "には、計算の対象となる最初の期を指定します。"
			},
			{
				name: "終了期",
				description: "には、計算の対象となる最後の期を指定します。"
			},
			{
				name: "支払期日",
				description: "には、支払時期の指定をします。"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "開始から終了までの貸付期間内で支払われる元金の累計額を計算します。.",
		arguments: [
			{
				name: "利率",
				description: "には、利率の指定をします。"
			},
			{
				name: "期間",
				description: "には、支払期間の合計を指定します。"
			},
			{
				name: "現在価値",
				description: "には、現在の貸付け額を指定します。"
			},
			{
				name: "開始期",
				description: "には、計算の対象となる最初の期を指定します。"
			},
			{
				name: "終了期",
				description: "には、計算の対象となる最後の期を指定します。"
			},
			{
				name: "支払期日",
				description: "には、支払時期の指定をします。"
			}
		]
	},
	{
		name: "DATE",
		description: "指定した日付を表すシリアル値を返します。.",
		arguments: [
			{
				name: "年",
				description: "には Windows 版 Spreadsheet では 1900 から 9999、Macintosh 版 Spreadsheet では 1904 から 9999 までの数値を指定します。"
			},
			{
				name: "月",
				description: "には月を表す数値 (1～12) を指定します。"
			},
			{
				name: "日",
				description: "には日を表す数値 (1～31) を指定します。"
			}
		]
	},
	{
		name: "DATEDIF",
		description: "",
		arguments: [
		]
	},
	{
		name: "DATEVALUE",
		description: "文字列の形式で表された日付を、Spreadsheet の組み込みの日付表示形式でシリアル値に変換して返します。.",
		arguments: [
			{
				name: "日付文字列",
				description: "には日付を表す文字列を、Spreadsheet の組み込みの日付表示形式で指定します。Windows 版を使用する場合は 1900 年 1 月 1 日 ～ 9999 年 12 月 31 日、Macintosh 版を使用する場合は 1904 年 1 月 1 日 ～ 9999 年 12 月 31 日の範囲にある日付を指定します。"
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "データベースの指定された列を検索し、条件を満たすレコードの平均値を返します。.",
		arguments: [
			{
				name: "データベース",
				description: "にはリストまたはデータベースを構成するセル範囲を指定します。データベースはデータを関連付けたリストです。"
			},
			{
				name: "フィールド",
				description: "には二重引用符 (') で囲んだ列ラベルか、リスト内の列の位置を示す番号を指定します。"
			},
			{
				name: "条件",
				description: "には指定した条件が設定されているセル範囲を指定します。範囲は、少なくとも列ラベルとその 1 つ下の検索条件を指定するセルを含む必要があります。"
			}
		]
	},
	{
		name: "DAY",
		description: "シリアル値に対応する日を 1 から 31 までの整数で返します。.",
		arguments: [
			{
				name: "シリアル値",
				description: "には Spreadsheet で日付や時間の計算に使用される日付コードを指定します。"
			}
		]
	},
	{
		name: "DAYS",
		description: "2 つの日付の間の日数を返します。.",
		arguments: [
			{
				name: "終了日",
				description: "開始日と終了日は日数を求める基準となる 2 つの日付です。"
			},
			{
				name: "開始日",
				description: "開始日と終了日は日数を求める基準となる 2 つの日付です。"
			}
		]
	},
	{
		name: "DAYS360",
		description: "1 年を 360 日として、指定した 2 つの日付の間の日数を返します。.",
		arguments: [
			{
				name: "開始日",
				description: "間隔を求めたい 2 つの日付を指定します。"
			},
			{
				name: "終了日",
				description: "間隔を求めたい 2 つの日付を指定します。"
			},
			{
				name: "方式",
				description: "には計算にヨーロッパ方式 (TRUE) と米国 NASD 方式 (FALSE) のどちらを使用するかを、論理値で指定します。"
			}
		]
	},
	{
		name: "DB",
		description: "定率法を使って計算した資産の減価償却を返します。.",
		arguments: [
			{
				name: "取得価額",
				description: "には資産を購入した時点での価格を指定します。"
			},
			{
				name: "残存価額",
				description: "には耐用年数を経た後での資産の価格を指定します。"
			},
			{
				name: "耐用年数",
				description: "には資産を使用できる年数、つまり償却の対象となる資産の寿命年数を指定します。"
			},
			{
				name: "期",
				description: "には減価償却費を計算したい期間を、<耐用年数> と同じ単位で指定します。"
			},
			{
				name: "月",
				description: "には初年度の月数を指定します。省略した場合、12 が指定されたものと見なされます。"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "データベースの指定された列を検索し、条件を満たすレコードの中で数値が入力されているセルの個数を返します。.",
		arguments: [
			{
				name: "データベース",
				description: "にはリストまたはデータベースを構成するセル範囲を指定します。データベースはデータを関連付けたリストです。"
			},
			{
				name: "フィールド",
				description: "には二重引用符 (') で囲んだ列ラベルか、リスト内の列の位置を示す番号を指定します。"
			},
			{
				name: "条件",
				description: "には指定した条件が設定されているセル範囲を指定します。範囲には、列ラベルとその 1 つ下の検索条件を指定するセルを選択します。"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "条件を満たすレコードの中の空白でないセルの個数を返します。.",
		arguments: [
			{
				name: "データベース",
				description: "にはリストまたはデータベースを構成するセル範囲を指定します。データベースはデータを関連付けたリストです。"
			},
			{
				name: "フィールド",
				description: "には二重引用符 (') で囲んだ列ラベルか、リスト内の列の位置を示す番号を指定します。"
			},
			{
				name: "条件",
				description: "には指定した条件が設定されているセル範囲を指定します。範囲には、列ラベルとその 1 つ下の検索条件を指定するセルを選択します。"
			}
		]
	},
	{
		name: "DDB",
		description: "倍率逓減法または指定したその他の方式を使って、計算した資産の減価償却を返します。.",
		arguments: [
			{
				name: "取得価額",
				description: "には資産を購入した時点での価格を指定します。"
			},
			{
				name: "残存価額",
				description: "には耐用年数を経た後での資産の価格を指定します。"
			},
			{
				name: "耐用年数",
				description: "には資産を使用できる年数、つまり償却の対象となる資産の寿命年数を指定します。"
			},
			{
				name: "期",
				description: "には減価償却費を計算したい期間を、<耐用年数> と同じ単位で指定します。"
			},
			{
				name: "率",
				description: "には減価償却費を計算するために使用する償却率を指定します。省略した場合、2 が指定されたものと見なされ、倍率逓減法で計算されます。"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "10 進数を 2 進数に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "変換したい 10 進数を指定します。"
			},
			{
				name: "桁数",
				description: "使用する文字数 (桁数) を指定します。"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "10 進数を 16 進数に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "変換したい 16 進数を指定します。"
			},
			{
				name: "桁数",
				description: "使用する文字数 (桁数) を指定します。"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "10 進数を 8 進数に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "変換したい 8 進数を指定します。"
			},
			{
				name: "桁数",
				description: "使用する文字数 (桁数) を指定します。"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "指定された底の数値のテキスト表現を 10 進数に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "には変換する数値を指定します。"
			},
			{
				name: "基数",
				description: "には変換する数値の基数を指定します。"
			}
		]
	},
	{
		name: "DEGREES",
		description: "ラジアンで表された角度を度に変更します。.",
		arguments: [
			{
				name: "角度",
				description: "には変換したい角度をラジアン単位で指定します。"
			}
		]
	},
	{
		name: "DELTA",
		description: "2 つの数値が等しいかどうかを判別します。.",
		arguments: [
			{
				name: "数値1",
				description: "には、一方の数値を指定します。"
			},
			{
				name: "数値2",
				description: "には、もう一方の数値を指定します。"
			}
		]
	},
	{
		name: "DEVSQ",
		description: "標本の平均値に対する各データの偏差の平方和を返します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には偏差の平方和を求めたい引数、または配列、配列参照を、1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には偏差の平方和を求めたい引数、または配列、配列参照を、1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "DGET",
		description: "データベースの列から指定された条件を満たす 1 つのレコードを抽出します。.",
		arguments: [
			{
				name: "データベース",
				description: "にはリストまたはデータベースを構成するセル範囲を指定します。データベースはデータを関連付けたリストです。"
			},
			{
				name: "フィールド",
				description: "には二重引用符 (') で囲んだ列ラベルか、リスト内の列の位置を示す番号を指定します。"
			},
			{
				name: "条件",
				description: "には指定した条件が設定されているセル範囲を指定します。範囲には、列ラベルとその 1 つ下の検索条件を指定するセルを選択します。"
			}
		]
	},
	{
		name: "DISC",
		description: "証券に対する割引率を計算します。.",
		arguments: [
			{
				name: "受渡日",
				description: "には、証券の受渡日を日付のシリアル値で指定します。"
			},
			{
				name: "満期日",
				description: "には、証券の満期日を日付のシリアル値で指定します。"
			},
			{
				name: "現在価値",
				description: "には、額面価格 $100 に対する証券の価格を指定します。"
			},
			{
				name: "償還価額",
				description: "には、額面価格 $100 に対する証券の償還価額を指定します。"
			},
			{
				name: "基準",
				description: "には、利息計算の基礎となる日数の計算方法を数値で指定します。"
			}
		]
	},
	{
		name: "DMAX",
		description: "データベースの指定された列を検索し、条件を満たすレコードの最大値を返します。.",
		arguments: [
			{
				name: "データベース",
				description: "にはリストまたはデータベースを構成するセル範囲を指定します。データベースはデータを関連付けたリストです。"
			},
			{
				name: "フィールド",
				description: "には二重引用符 (') で囲んだ列ラベルか、リスト内の列の位置を示す番号を指定します。"
			},
			{
				name: "条件",
				description: "には指定した条件が設定されているセル範囲を指定します。範囲には、列ラベルとその 1 つ下の検索条件を指定するセルを選択します。"
			}
		]
	},
	{
		name: "DMIN",
		description: "データベースの指定された列を検索し、条件を満たすレコードの最小値を返します。.",
		arguments: [
			{
				name: "データベース",
				description: "にはリストまたはデータベースを構成するセル範囲を指定します。データベースはデータを関連付けたリストです。"
			},
			{
				name: "フィールド",
				description: "には二重引用符 (') で囲んだ列ラベルか、リスト内の列の位置を示す番号を指定します。"
			},
			{
				name: "条件",
				description: "には指定した条件が設定されているセル範囲を指定します。範囲には、列ラベルとその 1 つ下の検索条件を指定するセルを選択します。"
			}
		]
	},
	{
		name: "DOLLAR",
		description: "数値を四捨五入し、通貨書式を設定した文字列に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "には数値、数値を含むセルの参照、または結果が数値となる数式を指定します。"
			},
			{
				name: "桁数",
				description: "には小数点以下の桁数を指定します。数値は必要に応じて四捨五入されます。桁数を省略すると、2 を指定したと見なされます。"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "分数として表現されているドル単位の価格を、10 進数を使った数値に変換します。.",
		arguments: [
			{
				name: "整数部と分子部",
				description: "には、分数として表現されている数値を指定します。"
			},
			{
				name: "分母",
				description: "には、分数の分母となる整数を指定します。"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "10 進数として表現されているドル単位の価格を、分数を使った数値に変換します。.",
		arguments: [
			{
				name: "小数値",
				description: "には、10 進数として表現された数値を指定します。"
			},
			{
				name: "分母",
				description: "には、分数の分母となる整数を指定します。"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "条件を満たすデータベース レコードの指定したフィールドに入力されている数値の積を返します。.",
		arguments: [
			{
				name: "データベース",
				description: "にはリストまたはデータベースを構成するセル範囲を指定します。データベースはデータを関連付けたリストです。"
			},
			{
				name: "フィールド",
				description: "には二重引用符 (') で囲んだ列ラベルか、リスト内の列の位置を示す番号を指定します。"
			},
			{
				name: "条件",
				description: "には指定した条件が設定されているセル範囲を指定します。範囲には、列ラベルとその 1 つ下の検索条件を指定するセルを選択します。"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "選択したデータベース レコードの標本を基に標準偏差を返します。.",
		arguments: [
			{
				name: "データベース",
				description: "にはリストまたはデータベースを構成するセル範囲を指定します。データベースはデータを関連付けたリストです。"
			},
			{
				name: "フィールド",
				description: "には二重引用符 (') で囲んだ列ラベルか、リスト内の列の位置を示す番号を指定します。"
			},
			{
				name: "条件",
				description: "には指定した条件が設定されているセル範囲を指定します。範囲には、列ラベルとその 1 つ下の検索条件を指定するセルを選択します。"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "条件を満たすレコードを母集団全体と見なして、母集団の標準偏差を返します。.",
		arguments: [
			{
				name: "データベース",
				description: "にはリストまたはデータベースを構成するセル範囲を指定します。データベースはデータを関連付けたリストです。"
			},
			{
				name: "フィールド",
				description: "には二重引用符 (') で囲んだ列ラベルか、リスト内の列の位置を示す番号を指定します。"
			},
			{
				name: "条件",
				description: "には指定した条件が設定されているセル範囲を指定します。範囲には、列ラベルとその 1 つ下の検索条件を指定するセルを選択します。"
			}
		]
	},
	{
		name: "DSUM",
		description: "データベースの指定された列を検索し、条件を満たすレコードの合計を返します。.",
		arguments: [
			{
				name: "データベース",
				description: "にはリストまたはデータベースを構成するセル範囲を指定します。データベースはデータを関連付けたリストです。"
			},
			{
				name: "フィールド",
				description: "には二重引用符 (') で囲んだ列ラベルか、リスト内の列の位置を示す番号を指定します。"
			},
			{
				name: "条件",
				description: "には指定した条件が設定されているセル範囲を指定します。範囲には、列ラベルとその 1 つ下の検索条件を指定するセルを選択します。"
			}
		]
	},
	{
		name: "DVAR",
		description: "条件を満たすデータベース レコードの指定したフィールドに入力した値を母集団の標本とみなして、母集団の分散を返します。.",
		arguments: [
			{
				name: "データベース",
				description: "にはリストまたはデータベースを構成するセル範囲を指定します。データベースはデータを関連付けたリストです。"
			},
			{
				name: "フィールド",
				description: "には二重引用符 (') で囲んだ列ラベルか、リスト内の列の位置を示す番号を指定します。"
			},
			{
				name: "条件",
				description: "には指定した条件が設定されているセル範囲を指定します。範囲には、列ラベルとその 1 つ下の検索条件を指定するセルを選択します。"
			}
		]
	},
	{
		name: "DVARP",
		description: "条件を満たすレコードを母集団全体と見なして、母集団の分散を返します。.",
		arguments: [
			{
				name: "データベース",
				description: "にはリストまたはデータベースを構成するセル範囲を指定します。データベースはデータを関連付けたリストです。"
			},
			{
				name: "フィールド",
				description: "には二重引用符 (') で囲んだ列ラベルか、リスト内の列の位置を示す番号を指定します。"
			},
			{
				name: "条件",
				description: "には指定した条件が設定されているセル範囲を指定します。範囲には、列ラベルとその 1 つ下の検索条件を指定するセルを選択します。"
			}
		]
	},
	{
		name: "EDATE",
		description: "開始日から起算して、指定した月だけ前あるいは後の日付に対応するシリアル値を計算します。.",
		arguments: [
			{
				name: "開始日",
				description: "には、計算の起算日となる日付のシリアル値を指定します。"
			},
			{
				name: "月",
				description: "には、開始日から起算した月数を指定します。"
			}
		]
	},
	{
		name: "EFFECT",
		description: "実質金利の計算をします。.",
		arguments: [
			{
				name: "名目利率",
				description: "名目上の年利を指定します。"
			},
			{
				name: "複利計算期間",
				description: "1 年当たりの複利利息の支払回数を指定します。"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "URL にエンコードされた文字列を返します。.",
		arguments: [
			{
				name: "文字列",
				description: " は URL にエンコードされる文字列です。"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "開始日から起算して、指定した月だけ前あるいは後の月の最終日に対応するシリアル値を計算します。.",
		arguments: [
			{
				name: "開始日",
				description: "には、計算の起算日となる日付のシリアル値を指定します。"
			},
			{
				name: "月",
				description: "には、開始日から起算した月数を指定します。"
			}
		]
	},
	{
		name: "ERF",
		description: "誤差関数の積分値を返します。.",
		arguments: [
			{
				name: "下限",
				description: "には、誤差関数を積分するときの下限値を指定します。"
			},
			{
				name: "上限",
				description: "には、誤差関数を積分するときの上限値を指定します。"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "誤差関数の積分値を返します。.",
		arguments: [
			{
				name: "X",
				description: "には、誤差関数を積分するときの下限値を指定します。"
			}
		]
	},
	{
		name: "ERFC",
		description: "相補誤差関数の積分値を返します。.",
		arguments: [
			{
				name: "x",
				description: "には、誤差関数を積分するときの下限値を指定します。"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "相補誤差関数の積分値を返します。.",
		arguments: [
			{
				name: "X",
				description: "には、誤差関数を積分するときの下限値を指定します。"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "Spreadsheet のエラー値に対応する数値を返します。.",
		arguments: [
			{
				name: "エラー値",
				description: "にはエラー識別番号を調べるエラー値を指定します。実際のエラー値、またはエラー値を含むセルの参照を指定します。"
			}
		]
	},
	{
		name: "EVEN",
		description: "指定した数値をもっとも近い偶数に切り上げた値を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には切り上げたい数値を指定します。"
			}
		]
	},
	{
		name: "EXACT",
		description: "2 つの文字列を比較し、同じものであれば TRUE、異なれば FALSE を返します。EXACT 関数では、英字の大文字と小文字は区別されます。.",
		arguments: [
			{
				name: "文字列1",
				description: "には 1 つめ文字列を指定します。"
			},
			{
				name: "文字列2",
				description: "には 2 つめ文字列を指定します。"
			}
		]
	},
	{
		name: "EXP",
		description: "e を底とする数値のべき乗を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には e を底とするべき乗の指数を指定します。定数 e は自然対数の底で、e = 2.71828182845904 となります。"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "指数分布関数を返します。.",
		arguments: [
			{
				name: "x",
				description: "には関数に代入する負でない値を指定します。"
			},
			{
				name: "λ",
				description: "には正の数値のパラメーターを指定します。"
			},
			{
				name: "関数形式",
				description: "には計算に使用する指数関数の形式を表す論理値を指定します。TRUE の場合は累積分布関数、FALSE の場合は確率密度関数が返されます。"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "指数分布関数を返します。.",
		arguments: [
			{
				name: "x",
				description: "には関数に代入する負でない値を指定します。"
			},
			{
				name: "λ",
				description: "には正の数値のパラメーターを指定します。"
			},
			{
				name: "関数形式",
				description: "には計算に使用する指数関数の形式を表す論理値を指定します。TRUE の場合、戻り値は累積分布関数となり、FALSE の場合は、確率密度関数が返されます。"
			}
		]
	},
	{
		name: "F.DIST",
		description: "(左側) F 確率分布を返します。.",
		arguments: [
			{
				name: "x",
				description: "には関数に代入する負でない数値を指定します。"
			},
			{
				name: "自由度1",
				description: "には分子の自由度 (10^10 を除く 1～ 10^10 の間の数値) を指定します。"
			},
			{
				name: "自由度2",
				description: "には分母の自由度 (10^10 を除く 1～ 10^10 の間の数値) を指定します。"
			},
			{
				name: "関数形式",
				description: "には計算に使用する関数の形式を表す論理値を指定します。TRUE の場合は累積分布関数、FALSE の場合は確率密度関数が返されます。"
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "(右側) F 確率分布を返します。.",
		arguments: [
			{
				name: "x",
				description: "には関数に代入する負でない数値を指定します。"
			},
			{
				name: "自由度1",
				description: "には分子の自由度 (10^10 を除く 1～ 10^10 の間の数値) を指定します。"
			},
			{
				name: "自由度2",
				description: "には分母の自由度 (10^10 を除く 1～ 10^10 の間の数値) を指定します。"
			}
		]
	},
	{
		name: "F.INV",
		description: "(左側) F 確率分布の逆関数を返します。.",
		arguments: [
			{
				name: "確率",
				description: "には F 累積分布における確率 (0～1 の数値) を指定します。"
			},
			{
				name: "自由度1",
				description: "には分子の自由度 (10^10 を除く 1～ 10^10 の間の数値) を指定します。"
			},
			{
				name: "自由度2",
				description: "には分母の自由度 (10^10 を除く 1～ 10^10 の間の数値) を指定します。"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "(右側) F 確率分布の逆関数を返します。.",
		arguments: [
			{
				name: "確率",
				description: "には F 累積分布における確率 (0～1 の数値) を指定します。"
			},
			{
				name: "自由度1",
				description: "には分子の自由度 (10^10 を除く 1～ 10^10 の間の数値) を指定します。"
			},
			{
				name: "自由度2",
				description: "には分母の自由度 (10^10 を除く 1～ 10^10 の間の数値) を指定します。"
			}
		]
	},
	{
		name: "F.TEST",
		description: "F-検定の結果を返します。F-検定により、配列 1 と配列 2 とのデータのばらつきに有意な差が認められない両側確率が返されます。.",
		arguments: [
			{
				name: "配列1",
				description: "には比較対象となる一方のデータ (数値、名前、配列、数値を含むセル参照) を含む配列またはセル範囲を指定します。空白は無視されます。"
			},
			{
				name: "配列2",
				description: "には比較対象となるもう一方のデータ (数値、名前、配列、数値を含むセル参照) を含む配列またはセル範囲を指定します。空白は無視されます。"
			}
		]
	},
	{
		name: "FACT",
		description: "数値の階乗を返します。数値の階乗は、1 ～ 数値の範囲にある整数の積です。.",
		arguments: [
			{
				name: "数値",
				description: "には階乗を求める正の数値を指定します。"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "数値の二重階乗を計算します。.",
		arguments: [
			{
				name: "数値",
				description: "には、二重階乗する数値を指定します。"
			}
		]
	},
	{
		name: "FALSE",
		description: "論理値 FALSE を返します。.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "(右側) F 確率分布を返します。.",
		arguments: [
			{
				name: "x",
				description: "には関数に代入する負でない数値を指定します。"
			},
			{
				name: "自由度1",
				description: "には分子の自由度 (10^10 を除く 1～ 10^10 の間の数値) を指定します。"
			},
			{
				name: "自由度2",
				description: "には分母の自由度 (10^10 を除く 1～ 10^10 の間の数値) を指定します。"
			}
		]
	},
	{
		name: "FIELD",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "FIELDPICTURE",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "FIND",
		description: "文字列が他の文字列内で最初に現れる位置を検索します。大文字と小文字は区別されます。.",
		arguments: [
			{
				name: "検索文字列",
				description: "には検索する文字列を指定します。空文字列 ('') を指定した場合、対象の先頭文字と一致したものと見なされます。ワイルドカード文字は使用できません。"
			},
			{
				name: "対象",
				description: "には検索文字列を含む文字列を指定します。"
			},
			{
				name: "開始位置",
				description: "には検索を開始する位置を指定します。対象の先頭文字から検索を開始するときは 1 を指定します。開始位置を省略すると、1 を指定したと見なされます。"
			}
		]
	},
	{
		name: "FINV",
		description: "F 確率分布の逆関数を返します。つまり、確率 = FDIST(x,...) であるとき、FINV(確率,...) = x となるような x の値を返します。.",
		arguments: [
			{
				name: "確率",
				description: "には F 累積分布における確率 (0～1 の数値) を指定します。"
			},
			{
				name: "自由度1",
				description: "には分子の自由度 (10^10 を除く 1～ 10^10 の間の数値) を指定します。"
			},
			{
				name: "自由度2",
				description: "には分母の自由度 (10^10 を除く 1～ 10^10 の間の数値) を指定します。"
			}
		]
	},
	{
		name: "FISHER",
		description: "フィッシャー変換の結果を返します。.",
		arguments: [
			{
				name: "x",
				description: "には関数に代入する値 (-1 および 1 を除く -1～1 の数値) を指定します。"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "フィッシャー変換の逆関数を返します。y = FISHER(x) であるとき、FISHERINV(y) = x という関係が成り立ちます。.",
		arguments: [
			{
				name: "y",
				description: "には逆変換の対象となる値を指定します。"
			}
		]
	},
	{
		name: "FIXED",
		description: "数値を指定された小数点で四捨五入し、カンマ (,) を使って、または使わずに書式設定した文字列に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "には四捨五入して、文字列に変換したい数値を指定します。"
			},
			{
				name: "桁数",
				description: "には小数点以下の桁数を指定します。桁数を省略すると、2 を指定したと見なされます。"
			},
			{
				name: "桁区切り",
				description: "には計算結果をカンマ ( , ) で桁区切りするかどうかを、論理値 (カンマで桁区切りしない = TRUE、カンマで桁区切りする = FALSE) で指定します。"
			}
		]
	},
	{
		name: "FLOOR",
		description: "指定された基準値の倍数のうち、最も近い値に数値を切り捨てます。.",
		arguments: [
			{
				name: "数値",
				description: "には対象となる数値を指定します。"
			},
			{
				name: "基準値",
				description: "には倍数の基準となる数値を指定します。数値と基準値の正負の符号が同じである必要があります。"
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "数値を最も近い整数、または最も近い基準値の倍数に切り下げます。.",
		arguments: [
			{
				name: "数値",
				description: "には対象となる値を指定します。"
			},
			{
				name: "基準値",
				description: "には倍数の基準となる数値を指定します。"
			},
			{
				name: "モード",
				description: "指定され、かつ 0 以外の場合、この関数は 0 の方向に切り下げます。"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "指定された基準値の倍数のうち、最も近い整数値に数値を切り捨てます。.",
		arguments: [
			{
				name: "数値",
				description: "には対象となる数値を指定します。"
			},
			{
				name: "基準値",
				description: "には倍数の基準となる数値を指定します。"
			}
		]
	},
	{
		name: "FORECAST",
		description: "既知の値を使用し、線形トレンドに沿って将来の値を予測します。.",
		arguments: [
			{
				name: "x",
				description: "には値を予測する対象となるデータ ポイントを数値で指定します。"
			},
			{
				name: "既知のy",
				description: "には独立した配列、またはデータの範囲を指定します。"
			},
			{
				name: "既知のx",
				description: "には独立した配列、または数値データの範囲を指定します。既知のx の値の分散が 0 でない必要があります。"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "数式を文字列として返します。.",
		arguments: [
			{
				name: "参照",
				description: "には数式への参照を指定します。"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "範囲内でのデータの度数分布を、垂直配列で返します。返された配列要素の個数は、区間配列の個数より 1 つだけ多くなります。.",
		arguments: [
			{
				name: "データ配列",
				description: "には度数分布を求めたい値の配列、または参照を指定します。空白セルおよび文字列は無視されます。"
			},
			{
				name: "区間配列",
				description: "にはデータ配列で指定したデータをグループ化するため、値の間隔を配列または参照として指定します。"
			}
		]
	},
	{
		name: "FTEST",
		description: "F-検定の結果を返します。F-検定により、配列 1 と配列 2 とのデータのばらつきに有意な差が認められない両側確率が返されます。.",
		arguments: [
			{
				name: "配列1",
				description: "には比較対象となる一方のデータ (数値、名前、配列、数値を含むセル参照) を含む配列またはセル範囲を指定します。空白は無視されます。"
			},
			{
				name: "配列2",
				description: "には比較対象となるもう一方のデータ (数値、名前、配列、数値を含むセル参照) を含む配列またはセル範囲を指定します。空白は無視されます。"
			}
		]
	},
	{
		name: "FV",
		description: "一定利率の支払いが定期的に行われる場合の、投資の将来価値を返します。.",
		arguments: [
			{
				name: "利率",
				description: "には 1 期間あたりの利率を指定します。たとえば、年率 6% のローンを四半期払いで返済する場合、利率には 6%/4 = 1.5 (%) を指定します。"
			},
			{
				name: "期間",
				description: "には投資期間全体での支払回数の合計を指定します。"
			},
			{
				name: "定期支払額",
				description: "には 1 期間あたりの支払額を指定します。投資期間内に支払額を変更することはできません。"
			},
			{
				name: "現在価値",
				description: "には投資の将来価値、つまり最後の支払いを行った後に残る現金の収支を指定します。将来価値を省略すると、0 を指定したと見なされます。"
			},
			{
				name: "支払期日",
				description: "には支払いがいつ行われるかを表す論理値 (期首払い = 1、期末払い = 0 または省略) を指定します。"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "投資期間内の一連の金利を複利計算することにより、初期投資の元金の将来価値を計算します。.",
		arguments: [
			{
				name: "元金",
				description: "投資の現在価値を指定します。"
			},
			{
				name: "利率配列",
				description: "投資期間内の金利変動を配列として指定します。"
			}
		]
	},
	{
		name: "GAMMA",
		description: "ガンマ関数値を返します。.",
		arguments: [
			{
				name: "x",
				description: "にはガンマを計算する値を指定します。"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "γ分布関数の値を返します。.",
		arguments: [
			{
				name: "x",
				description: "には分布を評価する値 (正の数値) を指定します。"
			},
			{
				name: "α",
				description: "には正の数値の確率分布のパラメーターを指定します。"
			},
			{
				name: "β",
				description: "には正の数値の確率分布のパラメーターを指定します。1 を指定した場合、標準γ分布の値が返されます。"
			},
			{
				name: "関数形式",
				description: "には関数の形式を表す論理値を指定します。TRUE を指定した場合は、累積分布関数が返されます。FALSE を指定した場合または省略した場合は、確率量関数が返されます。"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "γ累積分布の逆関数の値を返します。つまり、確率 = GAMMA.DIST(x,...) であるとき、GAMMA.INV(確率,...) = x となるような x の値を返します。.",
		arguments: [
			{
				name: "確率",
				description: "にはガンマ確率分布における確率 (0 ～ 1 の数値) を指定します。"
			},
			{
				name: "α",
				description: "には確率分布のパラメーター (正の数値) を指定します。"
			},
			{
				name: "β",
				description: "には確率分布のパラメーター (正の数値) を指定します。1 を指定した場合、標準γ分布の値が返されます。"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "γ分布関数の値を返します。.",
		arguments: [
			{
				name: "x",
				description: "には関数に代入する負でない数値を指定します。"
			},
			{
				name: "α",
				description: "には正の数値の確率分布のパラメーターを指定します。"
			},
			{
				name: "β",
				description: "には正の数値の確率分布のパラメーターを指定します。1 を指定した場合、標準γ分布の値が返されます。"
			},
			{
				name: "関数形式",
				description: "関数の形式を表す論理値を指定します。TRUE を指定した場合、累積分布関数が返されます。FALSE を指定した場合、または省略した場合、確率量関数が返されます。"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "γ累積分布の逆関数の値を返します。つまり、確率 = GAMMADIST(x,...) であるとき、GAMMAINV(確率,...) = x となるような x の値を返します。.",
		arguments: [
			{
				name: "確率",
				description: "にはガンマ確率分布における確率 (0 ～ 1 の数値) を指定します。"
			},
			{
				name: "α",
				description: "には確率分布のパラメーター (正の数値) を指定します。"
			},
			{
				name: "β",
				description: "には確率分布のパラメーター (正の数値) を指定します。1 を指定した場合、標準γ分布の値が返されます。"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "γ関数 G(x) の自然対数を返します。.",
		arguments: [
			{
				name: "x",
				description: "には関数に代入する値を正の数値で指定します。"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "γ関数 G(x) の自然対数を返します。.",
		arguments: [
			{
				name: "x",
				description: "には関数に代入する値を正の数値で指定します。"
			}
		]
	},
	{
		name: "GAUSS",
		description: "",
		arguments: [
		]
	},
	{
		name: "GCD",
		description: "指定した数値の最大公約数を計算します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には、最大 255 個までの数値を指定できます。"
			},
			{
				name: "数値2",
				description: "には、最大 255 個までの数値を指定できます。"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "正の数からなる配列またはセル範囲のデータの幾何平均を返します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には幾何平均を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には幾何平均を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "GESTEP",
		description: "しきい値より大きいか小さいかの判定をします。.",
		arguments: [
			{
				name: "数値",
				description: "には、しきい値との大小を比較する数値を指定します。"
			},
			{
				name: "しきい値",
				description: "には、しきい値となる数値を指定します。"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "ピボットテーブルに保存されているデータを取得します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "データフィールド",
				description: "には、データを取得するデータ フィールドの名前を指定します。"
			},
			{
				name: "ピボットテーブル",
				description: "には、取得するデータが含まれているピボットテーブル内のセルまたはセル範囲への参照を指定します。"
			},
			{
				name: "フィールド",
				description: "には、参照するフィールドを指定します。"
			},
			{
				name: "アイテム",
				description: "には、参照するフィールド アイテムを指定します。"
			}
		]
	},
	{
		name: "GROWTH",
		description: "既知のデータ ポイントに対応する指数トレンドの数値を返します。.",
		arguments: [
			{
				name: "既知のy",
				description: "には y = b*m^x となる、既にわかっている y 値の系列 (正の整数の配列または範囲) を指定します。"
			},
			{
				name: "既知のx",
				description: "には y = b*m^x となる、既にわかっている x 値の系列 (既知のy と同じサイズの配列または範囲) を指定します。"
			},
			{
				name: "新しいx",
				description: "には GROWTH 関数を利用して、対応する y の値を計算する新しい x の値を指定します。"
			},
			{
				name: "定数",
				description: "には定数 b を 1 にするかどうかを、論理値で指定します。TRUE に指定すると、b の値も計算され、FALSE を指定するか省略すると、b の値が 1 に設定されます。"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "正の数からなるデータの調和平均を返します。調和平均は、逆数の算術平均 (相加平均) に対する逆数として定義されます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には調和平均を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には調和平均を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "16 進数を 2 進数に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "変換したい 16 進数を指定します。"
			},
			{
				name: "桁数",
				description: "使用する文字数 (桁数) を指定します。"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "16 進数を 10 進数に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "変換したい 16 進数を指定します。"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "16 進数を 8 進数に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "変換したい 16 進数を指定します。"
			},
			{
				name: "桁数",
				description: "使用する文字数 (桁数) を指定します。"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "指定したテーブルまたは配列の先頭行で特定の値を検索し、指定した列と同じ行にある値を返します。.",
		arguments: [
			{
				name: "検索値",
				description: "には範囲の先頭行で検索する値を指定します。検索値には、値、セル参照、または文字列を指定します。"
			},
			{
				name: "範囲",
				description: "には目的のデータが含まれる文字列、数値、または論理値のテーブルを指定します。セル範囲の参照、またはセル範囲名を指定します。"
			},
			{
				name: "行番号",
				description: "には範囲の行番号を指定します。ここで指定された行で一致する値が返されます。範囲の先頭行には 1 を指定します。"
			},
			{
				name: "検索方法",
				description: "には検索値と完全に一致する値だけを検索するか、その近似値を含めて検索するかを、論理値 (近似値を含めて検索 = TRUE または省略、完全一致の値を検索 = FALSE) で指定します。"
			}
		]
	},
	{
		name: "HOUR",
		description: "時刻を 0 (午前 0 時) ～ 23 (午後 11 時) の範囲の整数で返します。.",
		arguments: [
			{
				name: "シリアル値",
				description: "には、Spreadsheet で使用される日付/時刻コードか、または 16:48:00 や 4:48:00 PM のような時刻形式のテキストを指定します。"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "ハード ディスク、ネットワーク サーバー、またはインターネット上に格納されているドキュメントを開くために、ショートカットまたはジャンプを作成します。.",
		arguments: [
			{
				name: "リンク先",
				description: "にはドキュメントを開くためのパスおよびファイル名、ハード ディスクの位置、UNC アドレス、または URL パスを指定します。"
			},
			{
				name: "別名",
				description: "にはセルに表示する文字列または数値を指定します。"
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "超幾何分布を返します。.",
		arguments: [
			{
				name: "標本の成功数",
				description: "には標本内で成功するものの数を指定します。"
			},
			{
				name: "標本数",
				description: "には標本数を指定します。"
			},
			{
				name: "母集団の成功数",
				description: "には母集団内で成功するものの数を指定します。"
			},
			{
				name: "母集団の大きさ",
				description: "には母集団全体の数を指定します。"
			},
			{
				name: "関数形式",
				description: "には論理値を指定します。TRUE を指定した場合は累積分布関数、FALSE を指定した場合は確率密度関数が返されます。"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "超幾何分布を返します。.",
		arguments: [
			{
				name: "標本の成功数",
				description: "には標本内で成功するものの数を指定します。"
			},
			{
				name: "標本数",
				description: "には標本数を指定します。"
			},
			{
				name: "母集団の成功数",
				description: "には母集団内で成功するものの数を指定します。"
			},
			{
				name: "母集団の大きさ",
				description: "には母集団全体の数を指定します。"
			}
		]
	},
	{
		name: "IF",
		description: "論理式の結果 (TRUE か FALSE) に応じて、指定された値を返します。.",
		arguments: [
			{
				name: "論理式",
				description: "には結果が TRUE または FALSE になる値、もしくは数式を指定します。"
			},
			{
				name: "真の場合",
				description: "には論理式の結果が TRUE であった場合に返される値を指定します。省略された場合、TRUE が返されます。最大 7 つまでの IF 関数をネストすることができます。"
			},
			{
				name: "偽の場合",
				description: "には論理式の結果が FALSE であった場合に返される値を指定します。省略された場合、FALSE が返されます。"
			}
		]
	},
	{
		name: "IFERROR",
		description: "式がエラーの場合は、エラーの場合の値を返します。エラーでない場合は、式の値自体を返します。.",
		arguments: [
			{
				name: "値",
				description: "には、任意の値、式、または参照を指定します。"
			},
			{
				name: "エラーの場合の値",
				description: "には、任意の値、式、または参照を指定します。"
			}
		]
	},
	{
		name: "IFNA",
		description: "式が #N/A に解決される場合に指定する値を返します。それ以外の場合は、式の結果を返します。.",
		arguments: [
			{
				name: "値",
				description: "には任意の値または式または参照を返します。"
			},
			{
				name: "NAの場合の値",
				description: "には任意の値または式または参照を返します。"
			}
		]
	},
	{
		name: "IMABS",
		description: "複素数の絶対値を計算します。.",
		arguments: [
			{
				name: "複素数",
				description: "には、計算の対象となる複素数を指定します。"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "複素数の虚部の係数を返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には、計算の対象となる複素数を文字列として指定します。"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "複素数を極形式で表現した場合の偏角θの値をラジアンを単位として計算します。.",
		arguments: [
			{
				name: "複素数",
				description: "には、計算の対象となる複素数を文字列として指定します。"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "複素数の共役複素数を文字列として返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には、計算の対象となる複素数を文字列として指定します。"
			}
		]
	},
	{
		name: "IMCOS",
		description: "複素数のコサインを返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には、コサインを求める複素数を指定します。"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "複素数の双曲線余弦を返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には双曲線余弦を求める複素数を指定します。"
			}
		]
	},
	{
		name: "IMCOT",
		description: "複素数の余接を返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には余接を求める複素数を指定します。"
			}
		]
	},
	{
		name: "IMCSC",
		description: "複素数の余割を返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には余割を求める複素数を指定します。"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "複素数の双曲線余割を返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には双曲線余割を求める複素数を指定します。"
			}
		]
	},
	{
		name: "IMDIV",
		description: "2 つの複素数を割り算しその商を返します。.",
		arguments: [
			{
				name: "複素数1",
				description: "には、割り算の分子となる複素数を指定します。"
			},
			{
				name: "複素数2",
				description: "には、割り算の分母となる複素数を指定します。"
			}
		]
	},
	{
		name: "IMEXP",
		description: "複素数のべき乗を返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には、べき乗を求める複素数を指定します。"
			}
		]
	},
	{
		name: "IMLN",
		description: "複素数の自然対数 (e を底とする対数) を計算します。.",
		arguments: [
			{
				name: "複素数",
				description: "には、計算の対象となる複素数を文字列として指定します。"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "複素数の 10 を底とする対数を返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には、常用対数を求める複素数を指定します。"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "複素数の 2 を底とする対数を返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には、2 を底とする対数を求める複素数を指定します。"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "複素数を底として複素数の整数乗を計算します。.",
		arguments: [
			{
				name: "複素数",
				description: "には、計算の対象となる複素数を文字列として指定します。"
			},
			{
				name: "数値",
				description: "には、複素数を底として何乗するかを整数で指定します。"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "1 ～ 255 個の複素数の積を計算します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "複素数1",
				description: "複素数1   ,複素数2,... 積を求める複素数を 1 ～ 255 個までの範囲で指定します。"
			},
			{
				name: "複素数2",
				description: "複素数1   ,複素数2,... 積を求める複素数を 1 ～ 255 個までの範囲で指定します。"
			}
		]
	},
	{
		name: "IMREAL",
		description: "複素数の実数係数を返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には、実数係数を求める複素数を指定します。"
			}
		]
	},
	{
		name: "IMSEC",
		description: "複素数の正割を返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には正割を求める複素数を指定します。"
			}
		]
	},
	{
		name: "IMSECH",
		description: "複素数の双曲線正割を返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には双曲線正割を求める複素数を指定します。"
			}
		]
	},
	{
		name: "IMSIN",
		description: "複素数のサインを返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には、サインを求める複素数を指定します。"
			}
		]
	},
	{
		name: "IMSINH",
		description: "複素数の双曲線正弦を返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には双曲線正弦を求める複素数を指定します。"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "複素数の平方根を返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には、平方根を求める複素数を指定します。"
			}
		]
	},
	{
		name: "IMSUB",
		description: "2 つの複素数の差を返します。.",
		arguments: [
			{
				name: "複素数1",
				description: "には、複素数2 を減算する元の複素数を指定します。"
			},
			{
				name: "複素数2",
				description: "には、複素数1 から減算する複素数を指定します。"
			}
		]
	},
	{
		name: "IMSUM",
		description: "2 つ以上の複素数の和を計算します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "複素数1",
				description: "には、計算の対象となる複素数を 1 から 255 個までの範囲で指定します。"
			},
			{
				name: "複素数2",
				description: "には、計算の対象となる複素数を 1 から 255 個までの範囲で指定します。"
			}
		]
	},
	{
		name: "IMTAN",
		description: "複素数の正接を返します。.",
		arguments: [
			{
				name: "複素数",
				description: "には正接を求める複素数を指定します。"
			}
		]
	},
	{
		name: "INDEX",
		description: "指定された行と列が交差する位置にある値またはセルの参照を返します。.",
		arguments: [
			{
				name: "配列",
				description: "にはセル範囲または配列定数を指定します。"
			},
			{
				name: "行番号",
				description: "には配列または参照の中にあり、値を返す行を数値で指定します。省略した場合は、必ず列番号を指定する必要があります。"
			},
			{
				name: "列番号",
				description: "には配列または参照の中にあり、値を返す列を数値で指定します。省略した場合は、必ず行番号を指定する必要があります。"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "指定される文字列への参照を返します。.",
		arguments: [
			{
				name: "参照文字列",
				description: "には A1 形式、R1C1 形式の参照、参照として定義されている名前が入力されているセルへの参照、または文字列としてのセルへの参照を指定します。"
			},
			{
				name: "参照形式",
				description: "には参照文字列で指定されたセルに含まれる参照の種類を、論理値 (A1 形式 = TRUE または省略、R1C1 形式 = FALSE) で指定します。"
			}
		]
	},
	{
		name: "INFO",
		description: "使用中のオペレーション システムに関する情報を返します。.",
		arguments: [
			{
				name: "検査の種類",
				description: "には調べたい情報の種類を指定します。"
			}
		]
	},
	{
		name: "INT",
		description: "切り捨てて整数にした数値を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には切り捨てて整数にする実数を指定します。"
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "既知の x と既知の y を通過する線形回帰直線の切片を計算します。.",
		arguments: [
			{
				name: "既知のy",
				description: "には 1 組の従属変数の値を指定します。引数には、数値、名前、配列、または数値を含むセル参照を指定します。"
			},
			{
				name: "既知のx",
				description: "には 1 組の独立変数の値を指定します。引数には、数値、名前、配列、または数値を含むセル参照を指定します。"
			}
		]
	},
	{
		name: "INTRATE",
		description: "全額投資された証券を対象に、その利率を計算します。.",
		arguments: [
			{
				name: "受渡日",
				description: "には、証券の受渡日を日付のシリアル値で指定します。"
			},
			{
				name: "満期日",
				description: "には、証券の満期日を日付のシリアル値で指定します。"
			},
			{
				name: "投資額",
				description: "には、証券への投資額を指定します。"
			},
			{
				name: "償還価額",
				description: "には、満期日における証券の償還価額を指定します。"
			},
			{
				name: "基準",
				description: "には、利息計算の基礎となる日数の計算方法を数値で指定します。"
			}
		]
	},
	{
		name: "IPMT",
		description: "一定利率の支払いが定期的に行われる場合の、投資期間内の指定された期に支払われる金利を返します。.",
		arguments: [
			{
				name: "利率",
				description: "には 1 期間あたりの利率を指定します。たとえば、年率 6% のローンを四半期払いで返済する場合、利率には 6%/4 = 1.5 (%) を指定します。"
			},
			{
				name: "期",
				description: "には金利支払額を求めたい投資期間内の特定の期を、1 から <期間> の範囲内で指定します。"
			},
			{
				name: "期間",
				description: "には投資期間全体での支払回数の合計を指定します。"
			},
			{
				name: "現在価値",
				description: "には投資の現在価値、つまり、将来行われる一連の支払いを現時点で一括払いした場合の合計金額を指定します。"
			},
			{
				name: "将来価値",
				description: "には投資の将来価値、つまり最後の支払いを行った後に残る現金の収支を指定します。将来価値を省略すると、0 を指定したと見なされます。"
			},
			{
				name: "支払期日",
				description: "には支払いがいつ行われるかを表す論理値 (期首払い = 1、期末払い = 0 または省略) を指定します。"
			}
		]
	},
	{
		name: "IRR",
		description: "一連の定期的なキャッシュ フローに対する内部収益率を返します。.",
		arguments: [
			{
				name: "範囲",
				description: "には、内部利益率を計算するための数値が入力されている配列またはセル参照を指定します。"
			},
			{
				name: "推定値",
				description: "には、IRR 関数が計算する内部利益率に近いと推定される数値を指定します。"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "セルの内容が空白の場合に TRUE を返します。.",
		arguments: [
			{
				name: "テストの対象",
				description: "にはテストしたいセル、またはセルを参照する名前を指定します。"
			}
		]
	},
	{
		name: "ISERR",
		description: "セルの内容が #N/A 以外のエラー値 (#VALUE!、#REF!、#DIV/0!、#NUM!、#NAME?、または #NULL!) の場合に TRUE を返します。.",
		arguments: [
			{
				name: "テストの対象",
				description: "にはテストするデータを指定します。引数には、セル、数式、またはセル、数式、値を参照する名前を指定することができます。"
			}
		]
	},
	{
		name: "ISERROR",
		description: "セルの内容がエラー値 (#N/A、#VALUE!、#REF!、#DIV/0!、#NUM!、#NAME?、または #NULL!) の場合に TRUE を返します。.",
		arguments: [
			{
				name: "テストの対象",
				description: "にはテストするデータを指定します。引数には、セル、数式、またはセル、数式、値を参照する名前を指定することができます。"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "引き数に指定した数値が偶数のとき TRUE を返し、奇数のとき FALSE を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には、対象となる数値を指定します。"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "参照が数式を含むセルに対するものかどうかを確認し、TRUE または FALSE を返します。.",
		arguments: [
			{
				name: "参照",
				description: "にはテストするセルへの参照を指定します。参照には、セル参照、数式、またはセルを参照する名前を指定できます。"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "セルの内容が論理値 (TRUE または FALSE) の場合に TRUE を返します。.",
		arguments: [
			{
				name: "テストの対象",
				description: "にはテストするデータを指定します。引数には、セル、数式、またはセル、数式、値を参照する名前を指定することができます。"
			}
		]
	},
	{
		name: "ISNA",
		description: "セルの内容がエラー値 #N/A の場合に TRUE を返します。.",
		arguments: [
			{
				name: "テストの対象",
				description: "にはテストするデータを指定します。引数には、セル、数式、またはセル、数式、値を参照する名前を指定することができます。"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "セルの内容が文字列以外の値 (空白セルも対象) である場合に TRUE を返します。.",
		arguments: [
			{
				name: "テストの対象",
				description: "には テストするデータを指定します。引数には、セル、数式、またはセル、数式、値を参照する名前を指定することができます。"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "セルの内容が数値の場合に TRUE を返します。.",
		arguments: [
			{
				name: "テストの対象",
				description: "にはテストするデータを指定します。引数には、セル、数式、またはセル、数式、値を参照する名前を指定することができます。"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "指定された基準値の倍数のうち、最も近い値に数値を切り上げます。.",
		arguments: [
			{
				name: "数値",
				description: "には対象となる数値を指定します。"
			},
			{
				name: "基準値",
				description: "には倍数の基準となる数値を指定します (オプション)。"
			}
		]
	},
	{
		name: "ISODD",
		description: "引き数に指定した数値が奇数のとき TRUE を返し、偶数のとき FALSE を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には、対象となる数値を指定します。"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "指定された日付のその年における ISO 週番号を返します。.",
		arguments: [
			{
				name: "日付",
				description: "には Spreadsheet で日付や時刻の計算に使用される日付/時刻コードを指定します。"
			}
		]
	},
	{
		name: "ISPMT",
		description: "投資期間内の指定された期に支払われる金利を返します。.",
		arguments: [
			{
				name: "利率",
				description: "には 1 期間あたりの利率を指定します。たとえば、年率 6% のローンを四半期払いで返済する場合、利率には 6%/4 = 1.5 (%) を指定します。"
			},
			{
				name: "期",
				description: "には金利支払額を求めたい投資期間内の特定の期を指定します。"
			},
			{
				name: "期間",
				description: "投資期間全体での支払回数の合計を指定します。"
			},
			{
				name: "現在価値",
				description: "には将来行われる一連の支払いを現時点で一括払いした場合の合計金額を指定します。"
			}
		]
	},
	{
		name: "ISREF",
		description: "セルの内容が参照である場合に TRUE を返します。.",
		arguments: [
			{
				name: "テストの対象",
				description: "にはテストするデータを指定します。引数には、セル、数式、またはセル、数式、値を参照する名前を指定することができます。"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "セルの内容が文字列である場合に TRUE を返します。.",
		arguments: [
			{
				name: "テストの対象",
				description: "にはテストするデータを指定します。引数には、セル、数式、またはセル、数式、値を参照する名前を指定することができます。"
			}
		]
	},
	{
		name: "KURT",
		description: "引数として指定したデータの尖度を返します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には尖度を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には尖度を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "LARGE",
		description: "データの中から、指定した順位番目に大きな値を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には対象となるデータが入力されている配列、またはセル範囲を指定します。"
			},
			{
				name: "順位",
				description: "には抽出する値の、大きい方から数えた順位を数値で指定します。"
			}
		]
	},
	{
		name: "LCM",
		description: "指定した整数の最小公倍数を計算します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には、最大 255 個までの数値を指定できます。"
			},
			{
				name: "数値2",
				description: "には、最大 255 個までの数値を指定できます。"
			}
		]
	},
	{
		name: "LEFT",
		description: "文字列の先頭から指定された数の文字を返します。.",
		arguments: [
			{
				name: "文字列",
				description: "には取り出す文字を含む文字列を指定します。"
			},
			{
				name: "文字数",
				description: "には取り出す文字数を指定します。省略すると、1 を指定したと見なされます。"
			}
		]
	},
	{
		name: "LEN",
		description: "文字列の長さ (文字数) を返します。半角と全角の区別なく、1 文字を 1 として処理します。.",
		arguments: [
			{
				name: "文字列",
				description: "には長さを求めたい文字列を指定します。"
			}
		]
	},
	{
		name: "LINEST",
		description: "最小二乗法を使って直線を当てはめることで、既知のデータ ポイントに対応する線形トレンドを表す補正項を計算します。.",
		arguments: [
			{
				name: "既知のy",
				description: "には y = mx + b となる、既にわかっている y 値の系列を指定します。"
			},
			{
				name: "既知のx",
				description: "には y = mx + b となる、既にわかっている x 値の系列を指定します。"
			},
			{
				name: "定数",
				description: "には定数 b を 0 にするかどうかを表す論理値を指定します。TRUE に指定するか省略すると、b の値も計算されます。FALSE を指定すると、b の値が 0 に設定されます。"
			},
			{
				name: "補正",
				description: "には回帰指数曲線の補正項を追加情報として返すかどうかを論理値で指定します。TRUE を指定すると、回帰指数曲線の補正項が返され、FALSE を指定するか省略すると、m 係数と定数 b のみが返されます。"
			}
		]
	},
	{
		name: "LN",
		description: "数値の自然対数を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には自然対数を求めたい、正の実数値を指定します。"
			}
		]
	},
	{
		name: "LOG",
		description: "指定された数を底とする数値の対数を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には対数を求めたい、正の実数値を指定します。"
			},
			{
				name: "底",
				description: "には対数の底を指定します。底を省略すると、10 を指定したと見なされます。"
			}
		]
	},
	{
		name: "LOG10",
		description: "引数の常用対数を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には常用対数を求めたい、正の実数値を指定します。"
			}
		]
	},
	{
		name: "LOGEST",
		description: "既知のデータ ポイントに対応する指数曲線を表す補正項を計算します。.",
		arguments: [
			{
				name: "既知のy",
				description: "には y = b*m^x となる、既にわかっている y 値の系列を指定します。"
			},
			{
				name: "既知のx",
				description: "には y = b*m^x となる、既にわかっている x 値の系列を指定します。この値は省略可能です。"
			},
			{
				name: "定数",
				description: "には定数 b に 1 を指定するかどうかを表す論理値を指定します。TRUE に指定するか省略すると、b の値も計算されます。FALSE を指定すると、b の値が 1 に設定されます。"
			},
			{
				name: "補正",
				description: "には回帰指数曲線の補正項を追加情報として返すかどうかを、論理値で指定します。TRUE を指定すると、回帰指数曲線の補正項が返され、FALSE を指定するか省略すると、m 係数と定数 b のみが返されます。"
			}
		]
	},
	{
		name: "LOGINV",
		description: "x の対数正規型の累積分布関数の逆関数の値を返します。ln(x) は平均と標準偏差を引数にする正規型分布になります。.",
		arguments: [
			{
				name: "確率",
				description: "には対数正規分布における確率 (0 ～ 1 の数値) を指定します。"
			},
			{
				name: "平均",
				description: "には ln(x) の平均値を指定します。"
			},
			{
				name: "標準偏差",
				description: "には ln(x) の標準偏差 (正の数値) を指定します。"
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "x の対数正規分布の確率を返します。ln(x)は、平均と標準偏差を引数にする正規型分布になります。.",
		arguments: [
			{
				name: "x",
				description: "には正の数値の関数に代入する値を指定します。"
			},
			{
				name: "平均",
				description: "には ln(x) の平均値を指定します。"
			},
			{
				name: "標準偏差",
				description: "には ln(x) の標準偏差 (正の数値) を指定します。"
			},
			{
				name: "関数形式",
				description: "には関数を示す論理値を指定します。TRUE を指定した場合は累積分布関数、FALSE を指定した場合は確率密度関数が返されます。"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "x の対数正規型の累積分布関数の逆関数の値を返します。ln(x) は平均と標準偏差を引数にする正規型分布になります。.",
		arguments: [
			{
				name: "確率",
				description: "には対数正規分布における確率 (0 ～ 1 の数値) を指定します。"
			},
			{
				name: "平均",
				description: "には ln(x) の平均値を指定します。"
			},
			{
				name: "標準偏差",
				description: "には ln(x) の標準偏差 (正の数値) を指定します。"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "x の対数正規分布の確率を返します。ln(x)は、平均と標準偏差を引数にする正規型分布になります。.",
		arguments: [
			{
				name: "x",
				description: "には正の数値の関数に代入する値を指定します。"
			},
			{
				name: "平均",
				description: "には ln(x) の平均値を指定します。"
			},
			{
				name: "標準偏差",
				description: "には ln(x) の標準偏差 (正の数値) を指定します。"
			}
		]
	},
	{
		name: "LOOKUP",
		description: "1 行または 1 列のみのセル範囲、または配列に含まれる値を返します。この関数は旧バージョンとの互換性を維持するためのものです。.",
		arguments: [
			{
				name: "検査値",
				description: "にはベクトルで検索する値を指定します。検査値には、数値、文字列、論理値、または値を参照する名前やセル参照を指定できます。"
			},
			{
				name: "検査範囲",
				description: "には 1 行または 1 列からのみ成り立つ範囲を指定します。検査範囲には、昇順に配置されている文字列、数値、または論理値を指定できます。"
			},
			{
				name: "対応範囲",
				description: "には 1 行または 1 列からのみ成り立つ範囲を指定します。対応範囲は検査範囲と同じサイズあることが必要です。"
			}
		]
	},
	{
		name: "LOWER",
		description: "文字列に含まれる英字をすべて小文字に変換します。.",
		arguments: [
			{
				name: "文字列",
				description: "には小文字に変換する文字列を指定します。それ以外の文字は変換されません。"
			}
		]
	},
	{
		name: "MATCH",
		description: "指定された照合の種類に従って検査範囲内を検索し、検査値と一致する要素の、配列内での相対的な位置を表す数値を返します。.",
		arguments: [
			{
				name: "検査値",
				description: "には、配列、数値、文字列、論理値、またはこれらの値への参照の中で必要な項目を検索するために使用する値を指定します。"
			},
			{
				name: "検査範囲",
				description: "には、検査値が入力されている連続したセル範囲、値の配列、または配列への参照を指定します。"
			},
			{
				name: "照合の種類",
				description: "には 1、0、または -1 の数値のいずれかを指定し、検査値を検索する方法を指定します。"
			}
		]
	},
	{
		name: "MAX",
		description: "引数の最大値を返します。論理値および文字列は無視されます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には最大値を求めたい数値、空白セル、論理値、または文字列番号を、1 ～ 255 個まで指定できます"
			},
			{
				name: "数値2",
				description: "には最大値を求めたい数値、空白セル、論理値、または文字列番号を、1 ～ 255 個まで指定できます"
			}
		]
	},
	{
		name: "MAXA",
		description: "引数の最大値を返します。論理値や文字列も対象となります。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "値1",
				description: "には最大値を求めたい数値、空白セル、論理値、または文字列番号を、1 ～ 255 個まで指定できます。"
			},
			{
				name: "値2",
				description: "には最大値を求めたい数値、空白セル、論理値、または文字列番号を、1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "MDETERM",
		description: "配列の行列式を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には行数と列数が等しい数値配列 (正方行列) を指定します。セル範囲かまたは配列定数のいずれかを指定します。"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "引数リストに含まれる数値のメジアン (中央値) を返します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "にはメジアンを求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "にはメジアンを求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "MID",
		description: "文字列の指定された位置から、指定された数の文字を返します。半角と全角の区別なく、1 文字を 1 として処理します。.",
		arguments: [
			{
				name: "文字列",
				description: "には検索の対象となる文字を含む文字列を指定します。"
			},
			{
				name: "開始位置",
				description: "には抜き出したい文字列の先頭文字の位置を指定します。"
			},
			{
				name: "文字数",
				description: "には文字列から抜き出す文字の数を指定します。"
			}
		]
	},
	{
		name: "MIN",
		description: "引数の最小値を返します。論理値および文字列は無視されます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には最小値を求めたい数値、空白セル、論理値、または文字列番号を、1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には最小値を求めたい数値、空白セル、論理値、または文字列番号を、1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "MINA",
		description: "引数の最小値を返します。論理値や文字列も対象となります。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "値1",
				description: "には最小値を求めたい数値、空白セル、論理値、または文字列番号を、1 ～ 255 個まで指定できます。"
			},
			{
				name: "値2",
				description: "には最小値を求めたい数値、空白セル、論理値、または文字列番号を、1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "MINUTE",
		description: "分を 0 ～ 59 の範囲の整数で返します。.",
		arguments: [
			{
				name: "シリアル値",
				description: "には、Spreadsheet で使用される日付/時刻コードか、または 16:48:00 や 4:48:00 PM のような時刻形式のテキストを指定します。"
			}
		]
	},
	{
		name: "MINVERSE",
		description: "配列の逆行列を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には行数と列数が等しい数値配列 (正方行列) を指定します。セル範囲かまたは配列定数のいずれかを指定します。"
			}
		]
	},
	{
		name: "MIRR",
		description: "投資原価と現金の再投資に対する受取利率 (危険利率) の両方を考慮して、一連の定期的なキャッシュ フローに対する内部収益率を返します。.",
		arguments: [
			{
				name: "範囲",
				description: "には数値を含む配列またはセル参照を指定します。これらの数値は、定期的に発生する一連の支払い (負の値) と収益 (正の値) に対応します。"
			},
			{
				name: "安全利率",
				description: "には投資額 (負のキャッシュ フロー) に対する利率を指定します。"
			},
			{
				name: "危険利率",
				description: "には収益額 (正のキャッシュ フロー) に対する利率を指定します。"
			}
		]
	},
	{
		name: "MMULT",
		description: "2 つの配列の積を返します。計算結果は、行数が配列 1 と同じで、列数が配列 2 と同じ配列になります。.",
		arguments: [
			{
				name: "配列1",
				description: "には行列積を求める最初の配列を指定します。配列 1 の列数は、配列 2 の行数と等しくなければなりません。"
			},
			{
				name: "配列2",
				description: "には行列積を求める最初の配列を指定します。配列 1 の列数は、配列 2 の行数と等しくなければなりません。"
			}
		]
	},
	{
		name: "MOD",
		description: "数値を除算した剰余を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には除算の分子となる数値を指定します。"
			},
			{
				name: "除数",
				description: "には除算の分母となる数値を指定します。"
			}
		]
	},
	{
		name: "MODE",
		description: "配列またはセル範囲として指定されたデータの中で、最も頻繁に出現する値 (最頻値) を返します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には最頻値を求める数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には最頻値を求める数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "最も頻繁に出現する垂直配列、または指定の配列かデータ範囲内で反復的に出現する値を返します。水平配列の場合は、=TRANSPOSE(MODE.MULT(数値1,数値2,...)) を使用します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には、最頻値を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には、最頻値を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "配列またはセル範囲として指定されたデータの中で、最も頻繁に出現する値 (最頻値) を返します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には最頻値を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には最頻値を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "MONTH",
		description: "月を 1 (月) ～ 12 (月) の範囲の整数で返します。.",
		arguments: [
			{
				name: "シリアル値",
				description: "には Spreadsheet で使用される日付/時刻コードを指定します。"
			}
		]
	},
	{
		name: "MROUND",
		description: "指定した値の倍数になるように数値の切り上げあるいは切り捨てを行います。.",
		arguments: [
			{
				name: "数値",
				description: "には、切り上げあるいは切り捨ての対象となる数値を指定します。"
			},
			{
				name: "倍数",
				description: "には、切り上げあるいは切り捨てられた数値が、その倍数となるような数値を指定します。"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "指定された数値の和の階乗と、指定された数値の階乗の積との比を計算します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には、計算の対象となる数値を最大 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には、計算の対象となる数値を最大 255 個まで指定できます。"
			}
		]
	},
	{
		name: "MUNIT",
		description: "指定された次元の単位行列を返します。.",
		arguments: [
			{
				name: "次元",
				description: "には返す単位行列の次元を指定する整数を指定します。"
			}
		]
	},
	{
		name: "N",
		description: "非数値を数値に、日付をシリアル値に、TRUE の場合は 1 に、それ以外の場合は 0 に変換します。.",
		arguments: [
			{
				name: "値",
				description: "には、変換する値を指定します。"
			}
		]
	},
	{
		name: "NA",
		description: "エラー値 #N/A (値が無効) を返します。.",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "負の二項分布の確率関数の値を返します。試行の成功率が一定のとき、成功数で指定した回数の試行が成功する前に、失敗数で指定した回数の試行が失敗する確率です。.",
		arguments: [
			{
				name: "失敗数",
				description: "には試行が失敗する回数を指定します。"
			},
			{
				name: "成功数",
				description: "には分析のしきい値となる、試行が成功する回数を指定します。"
			},
			{
				name: "成功率",
				description: "には試行が成功する確率を 0 ～ 1 までの数値で指定します。"
			},
			{
				name: "関数形式",
				description: "には関数を示す論理値を指定します。TRUE を指定した場合は累積分布関数、FALSE を指定した場合は確率密度関数が返されます。"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "負の二項分布の確率関数の値を返します。試行の成功率が一定のとき、成功数で指定した回数の試行が成功する前に、失敗数で指定した回数の試行が失敗する確率です。.",
		arguments: [
			{
				name: "失敗数",
				description: "には試行が失敗する回数を指定します。"
			},
			{
				name: "成功数",
				description: "には分析のしきい値となる、試行が成功する回数を指定します。"
			},
			{
				name: "成功率",
				description: "には試行が成功する確率を 0 ～ 1 までの数値で指定します。"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "開始日と終了日の間にある週日の日数を計算します。.",
		arguments: [
			{
				name: "開始日",
				description: "には、対象となる期間の初日となる日付のシリアル値を指定します。"
			},
			{
				name: "終了日",
				description: "には、対象となる期間の最終日となる日付のシリアル値を指定します。"
			},
			{
				name: "祭日",
				description: "は省略可能な引数で、国民の祝日などの日数を計算にいれないため対応する日付のシリアル値を指定します。"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "ユーザー設定の週末パラメーターを使用して、開始日と終了日の間にある週日の日数を計算します。.",
		arguments: [
			{
				name: "開始日",
				description: "には、期間の開始日となる日付のシリアル値を指定します。"
			},
			{
				name: "終了日",
				description: "には、期間の終了日となる日付のシリアル値を指定します。"
			},
			{
				name: "週末",
				description: "は週末の開始を指定する番号または文字列です。"
			},
			{
				name: "祭日",
				description: "は省略可能な引数で、国民の祝日などの日数を計算にいれないため対応する日付のシリアル値を指定します。"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "預金などの名目上の年利を計算します。.",
		arguments: [
			{
				name: "実効利率",
				description: "実質年利を指定します。"
			},
			{
				name: "複利計算期間",
				description: "1 年当たりの複利利息の支払回数を指定します。"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "指定した平均と標準偏差に対する正規分布の値を返します。.",
		arguments: [
			{
				name: "x",
				description: "には関数に代入する数値を指定します。"
			},
			{
				name: "平均",
				description: "には分布の算術平均を指定します。"
			},
			{
				name: "標準偏差",
				description: "には分布の標準偏差を正の数値で指定します。"
			},
			{
				name: "関数形式",
				description: "には関数の形式を表す論理値を指定します。TRUE を指定した場合は累積分布関数が返され、FALSE を指定した場合は確率密度関数が返されます。"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "指定した平均と標準偏差に対する正規分布の累積分布関数の逆関数の値を返します。.",
		arguments: [
			{
				name: "確率",
				description: "には正規分布における確率 (0 ～ 1の数値) を指定します。"
			},
			{
				name: "平均",
				description: "には対象となる分布の算術平均を指定します。"
			},
			{
				name: "標準偏差",
				description: "には対象となる分布の標準偏差 (正の数値) を指定します。"
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "標準正規分布を返します。この分布は、平均が 0 で標準偏差が 1 である正規分布に対応します。.",
		arguments: [
			{
				name: "z",
				description: "には関数に代入する値を指定します。"
			},
			{
				name: "関数形式",
				description: "には計算に使用する関数の形式を表す論理値を指定します。TRUE の場合は累積分布関数、FALSE の場合は確率密度関数が返されます。"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "標準正規分布の累積分布関数の逆関数の値を返します。この分布は、平均が 0 で標準偏差が 1 である正規分布に対応します。.",
		arguments: [
			{
				name: "確率",
				description: "には正規分布における確率 (0 ～ 1 の数値) を指定します。"
			}
		]
	},
	{
		name: "NORMDIST",
		description: "指定した平均と標準偏差に対する正規分布関数の値を返します。.",
		arguments: [
			{
				name: "x",
				description: "には関数に代入する数値を指定します。"
			},
			{
				name: "平均",
				description: "には分布の算術平均を指定します。"
			},
			{
				name: "標準偏差",
				description: "には分布の標準偏差を正の数値で指定します。"
			},
			{
				name: "関数形式",
				description: "には関数の形式を表す論理値を指定します。TRUE を指定した場合は、累積分布関数が返され、FALSE を指定した場合は、確率密度関数が返されます。"
			}
		]
	},
	{
		name: "NORMINV",
		description: "指定した平均と標準偏差に対する正規分布の累積分布関数の逆関数の値を返します。.",
		arguments: [
			{
				name: "確率",
				description: "には正規分布における確率 (0 ～ 1の数値) を指定します。"
			},
			{
				name: "平均",
				description: "には対象となる分布の算術平均を指定します。"
			},
			{
				name: "標準偏差",
				description: "には対象となる分布の標準偏差 (正の数値) を指定します。"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "標準正規分布の累積分布関数の値を返します。この分布は、平均が 0 で標準偏差が 1 である正規分布に対応します。.",
		arguments: [
			{
				name: "z",
				description: "には関数に代入する値を指定します。"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "標準正規分布の累積分布関数の逆関数の値を返します。この分布は、平均が 0 で標準偏差が 1 である正規分布に対応します。.",
		arguments: [
			{
				name: "確率",
				description: "には正規分布における確率 (0 ～ 1 の数値) を指定します。"
			}
		]
	},
	{
		name: "NOT",
		description: "引数が FALSE の場合は TRUE、TRUE の場合は FALSE を返します。.",
		arguments: [
			{
				name: "論理式",
				description: "には評価の結果が TRUE または FALSE になる値、または数式を指定します。"
			}
		]
	},
	{
		name: "NOW",
		description: "現在の日付と時刻を表すシリアル値を返します。.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "一定利率の支払いが定期的に行われる場合の、ローンの支払回数を返します。.",
		arguments: [
			{
				name: "利率",
				description: "には 1 期間あたりの利率を指定します。たとえば、年率 6% のローンを四半期払いで返済する場合、利率には 6%/4 = 1.5 (%) を指定します。"
			},
			{
				name: "定期支払額",
				description: "には毎回の支払額を指定します。投資期間内に支払額を変更することはできません。"
			},
			{
				name: "現在価値",
				description: "には投資の現在価値、つまり、将来行われる一連の支払いを現時点で一括払いした場合の合計金額を指定します。"
			},
			{
				name: "将来価値",
				description: "には投資の将来価値、つまり最後の支払いを行った後に残る現金の収支を指定します。将来価値を省略すると、0 を指定したと見なされます。"
			},
			{
				name: "支払期日",
				description: "には支払いがいつ行われるかを表す論理値 (期首払い = 1、期末払い = 0 または省略) を指定します。"
			}
		]
	},
	{
		name: "NPV",
		description: "投資の正味現在価値を、割引率、将来行われる一連の支払い (負の値)、およびその収益 (正の値) を使って算出します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "割引率",
				description: "には投資期間を通じて一定の割引率を指定します。"
			},
			{
				name: "値1",
				description: "には支払額と収益額を表す 1 ～ 254 個の引数を指定します。"
			},
			{
				name: "値2",
				description: "には支払額と収益額を表す 1 ～ 254 個の引数を指定します。"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "文字列をロケールに依存しない方法で数値に変換します。.",
		arguments: [
			{
				name: "文字列",
				description: "には変換する数値を表す文字列を指定します。"
			},
			{
				name: "小数点記号",
				description: "には文字列で小数点記号として使用する文字を指定します。"
			},
			{
				name: "桁区切り記号",
				description: "には文字列でグループ区切り記号として使用する文字を指定します。"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "8 進数を 2 進数に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "には、変換したい 8 進数を指定します。"
			},
			{
				name: "桁数",
				description: "には、2 進表記するときに使用する文字数を (桁数) 指定します。"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "8 進数を 10 進数に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "には、変換したい 8 進数を指定します。"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "8 進数を 16 進数に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "には、変換したい 8 進数を指定します。"
			},
			{
				name: "桁数",
				description: "には、16 進表記するときに使用する文字数 (桁数) を指定します。"
			}
		]
	},
	{
		name: "ODD",
		description: "正の数値を切り上げ、負の数値を切り捨てて、最も近い奇数にします。.",
		arguments: [
			{
				name: "数値",
				description: "には対象となる数値を指定します。"
			}
		]
	},
	{
		name: "OFFSET",
		description: "指定した参照から指定した行数、列数の範囲への参照を返します。.",
		arguments: [
			{
				name: "参照",
				description: "にはオフセットの基準となる参照を指定します。セルまたは隣接するセル範囲を参照する必要があります。"
			},
			{
				name: "行数",
				description: "には基準の左上隅のセルを上方向または下方向へシフトする距離を行数単位で指定します。"
			},
			{
				name: "列数",
				description: "には基準の左上隅のセルを左方向または右方向へシフトする距離を列数単位で指定します。"
			},
			{
				name: "高さ",
				description: "には設定したい高さを行数で指定します。高さを省略すると、基準の参照と同じ行数であると見なされます。"
			},
			{
				name: "幅",
				description: "には設定したい幅を列数で指定します。幅を省略すると、基準の参照と同じ列数であると見なされます。"
			}
		]
	},
	{
		name: "OR",
		description: "いずれかの引数が TRUE のとき、TRUE を返します。引数がすべて FALSE である場合は、FALSE を返します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "論理式1",
				description: "には結果が TRUE または FALSE になる、1 ～ 255 個の論理式を指定します。"
			},
			{
				name: "論理式2",
				description: "には結果が TRUE または FALSE になる、1 ～ 255 個の論理式を指定します。"
			}
		]
	},
	{
		name: "PARAMETER",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "PDURATION",
		description: "投資が指定した価値に達するまでの投資期間を返します。.",
		arguments: [
			{
				name: "利率",
				description: "には投資期間内の利率を示します。"
			},
			{
				name: "現在価値",
				description: "には投資の現在価値を指定します。"
			},
			{
				name: "将来価値",
				description: "には投資の将来価値を指定します。"
			}
		]
	},
	{
		name: "PEARSON",
		description: "ピアソンの積率相関係数 r の値を返します。.",
		arguments: [
			{
				name: "配列1",
				description: "には複数の独立変数の値を指定します。"
			},
			{
				name: "配列2",
				description: "には複数の従属変数の値を指定します。"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "配列に含まれる値の k 番目の百分位を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には相対的な位置を決定する数値データが含まれる配列、または範囲を指定します。"
			},
			{
				name: "率",
				description: "には百分位の値を、0 以上 1 以下の範囲で指定します。"
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "配列に含まれる値の k 番目の百分位を返します。k には、0 より大きく 1 より小さい値を指定します。.",
		arguments: [
			{
				name: "配列",
				description: "には相対的な位置を決定する数値データが含まれる配列、または範囲を指定します。"
			},
			{
				name: "率",
				description: "には百分位の値を、0 以上 1 以下の範囲で指定します。"
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "配列に含まれる値の k 番目の百分位を返します。k には、0 以上 1 以下の値を指定します。.",
		arguments: [
			{
				name: "配列",
				description: "には相対的な位置を決定する数値データが含まれる配列、または範囲を指定します。"
			},
			{
				name: "率",
				description: "には百分位の値を、0 以上 1 以下の範囲で指定します。"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "値 x の配列内での順位を百分率で表した値を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には相対的な位置を決定する数値データが含まれる配列、または範囲を指定します。"
			},
			{
				name: "x",
				description: "には相対的な順位を調べる値を指定します。"
			},
			{
				name: "有効桁数",
				description: "には結果として返される百分率の有効桁数を指定します。省略すると、小数点以下第三位 (0.xxx) まで計算されます。"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "値 x の配列内での順位を百分率 (0 より大きく 1 より小さい) で表した値を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には相対的な位置を決定する数値データが含まれる配列、または範囲を指定します。"
			},
			{
				name: "x",
				description: "には相対的な順位を調べる値を指定します。"
			},
			{
				name: "有効桁数",
				description: "には結果として返される百分率の有効桁数を指定します。省略すると、小数点以下第三位 (0.xxx) まで計算されます。"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "値 x の配列内での順位を百分率 (0 以上 1 以下) で表した値を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には相対的な位置を決定する数値データが含まれる配列、または範囲を指定します。"
			},
			{
				name: "x",
				description: "には相対的な順位を調べる値を指定します。"
			},
			{
				name: "有効桁数",
				description: "には結果として返される百分率の有効桁数を指定します。省略すると、小数点以下第三位 (0.xxx) まで計算されます。"
			}
		]
	},
	{
		name: "PERMUT",
		description: "指定した数の対象から、指定された数だけ抜き取る場合の順列の数を返します。.",
		arguments: [
			{
				name: "標本数",
				description: "には対象の数を整数で指定します。"
			},
			{
				name: "抜き取り数",
				description: "には順列 1 つに含まれる対象の数を整数で指定します。"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "指定した数の対象 (反復あり) から、指定された数だけ抜き取る場合の順列の数を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には対象の数を整数で指定します。"
			},
			{
				name: "抜き取り数",
				description: "には順列 1 つに含まれる対象の数を整数で指定します。"
			}
		]
	},
	{
		name: "PHI",
		description: "標準正規分布の密度関数の値を返します。.",
		arguments: [
			{
				name: "x",
				description: "には標準正規分布の密度を求める数値を指定します。"
			}
		]
	},
	{
		name: "PI",
		description: "円周率π (3.14159265358979) を返します。.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "一定利率の支払いが定期的に行われる場合の、ローンの定期支払額を算出します。.",
		arguments: [
			{
				name: "利率",
				description: "にはローンの 1 期間あたりの利率を指定します。たとえば、年率 6% のローンを四半期払いで返済する場合、利率には 6%/4 = 1.5 (%) を指定します。"
			},
			{
				name: "期間",
				description: "にはローン期間全体での支払回数の合計を指定します。"
			},
			{
				name: "現在価値",
				description: "には投資の現在価値、つまり、将来行われる一連の支払いを現時点で一括払いした場合の合計金額、または元金を指定します。"
			},
			{
				name: "将来価値",
				description: "には投資の将来価値、つまり最後の支払いを行った後に残る現金の収支を指定します。将来価値を省略すると、0 を指定したと見なされます。"
			},
			{
				name: "支払期日",
				description: "には支払いがいつ行われるかを表す論理値 (期首払い = 1、期末払い = 0 または省略) を指定します。"
			}
		]
	},
	{
		name: "POISSON",
		description: "ポワソン分布の値を返します。.",
		arguments: [
			{
				name: "イベント数",
				description: "にはイベント数を指定します。"
			},
			{
				name: "平均",
				description: "には一定の時間内に起こるイベント数の平均値を正の数値で指定します。"
			},
			{
				name: "関数形式",
				description: "には返される確率分布の形式を表す論理値を指定します。TRUE を指定した場合は、累積ポワソン確率が計算され、FALSE を指定した場合は、ポワソン確率が計算されます。"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "ポワソン分布の値を返します。.",
		arguments: [
			{
				name: "イベント数",
				description: "にはイベント数を指定します。"
			},
			{
				name: "平均",
				description: "には一定の時間内に起こるイベント数の平均値を正の数値で指定します。"
			},
			{
				name: "関数形式",
				description: "には返される確率分布の形式を表す論理値を指定します。TRUE を指定した場合は累積ポワソン確率が計算され、FALSE を指定した場合はポワソン確率が計算されます。"
			}
		]
	},
	{
		name: "POWER",
		description: "数値を累乗した値を返します。.",
		arguments: [
			{
				name: "数値",
				description: "にはべき乗の底を指定します。任意の実数を指定できます。"
			},
			{
				name: "指数",
				description: "には <数値> を底とするべき乗の指数を指定します。"
			}
		]
	},
	{
		name: "PPMT",
		description: "一定利率の支払いが定期的に行われる場合の、投資の指定した期に支払われる元金を返します。.",
		arguments: [
			{
				name: "利率",
				description: "には 1 期間あたりの利率を指定します。たとえば、年率 6% のローンを四半期払いで返済する場合、利率には 6%/4 = 1.5 (%) を指定します。"
			},
			{
				name: "期",
				description: "には元金支払額を求めたい投資期間内の特定の期を、1 から <期間> の範囲内で指定します。"
			},
			{
				name: "期間",
				description: "には投資期間全体での支払回数の合計を指定します。"
			},
			{
				name: "現在価値",
				description: "には投資の現在価値、つまり、将来行われる一連の支払いを現時点で一括払いした場合の合計金額を指定します。"
			},
			{
				name: "将来価値",
				description: "には投資の将来価値、つまり最後の支払いを行った後に残る現金の収支を指定します。将来価値を省略すると、0 を指定したと見なされます。"
			},
			{
				name: "支払期日",
				description: "には支払いがいつ行われるかを表す論理値 (期首払い = 1、期末払い = 0 または省略) を指定します。"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "割引証券の額面 $100 に対する価格を計算します。.",
		arguments: [
			{
				name: "受渡日",
				description: "には、証券の受渡日を日付のシリアル値で指定します。"
			},
			{
				name: "満期日",
				description: "には、証券の満期日を日付のシリアル値で指定します。"
			},
			{
				name: "割引率",
				description: "には、証券の割引率を指定します。"
			},
			{
				name: "償還価額",
				description: "には、額面 $100 に対する証券の償還価額を指定します。"
			},
			{
				name: "基準",
				description: "には、利息計算の基礎となる日数の計算方法を数値で指定します。"
			}
		]
	},
	{
		name: "PROB",
		description: "指定した範囲内の値が、上限と下限で指定される範囲に含まれる確率を返します。.",
		arguments: [
			{
				name: "x範囲",
				description: "には確率範囲と対応関係にある数値 x 含む配列またはセル範囲を指定します。"
			},
			{
				name: "確率範囲",
				description: "には x 範囲に含まれるそれぞれの数値に対応する確率 (0 ～ 1 の数値) を指定します。"
			},
			{
				name: "下限",
				description: "には対象となる数値の下限値を指定します。"
			},
			{
				name: "上限",
				description: "には対象となる数値の上限値を指定します (省略可能)。省略すると、x 範囲に含まれる数値が下限の値に等しくなる確率が計算されます。"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "引数の積を返します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には積を求めたい数値、論理値、または数値を表す文字列を、1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には積を求めたい数値、論理値、または数値を表す文字列を、1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "PROPER",
		description: "文字列中の各単語の先頭文字を大文字に変換した結果を返します。.",
		arguments: [
			{
				name: "文字列",
				description: "には二重引用符で囲んだ文字列、文字列を返す数式、または先頭を大文字にしたい文字列が入力されているセルへの参照を指定します。"
			}
		]
	},
	{
		name: "PV",
		description: "投資の現在価値を返します。現在価値とは、将来行われる一連の支払いを、現時点で一括払いした場合の合計金額のことをいいます。.",
		arguments: [
			{
				name: "利率",
				description: "には 1 期間あたりの利率を指定します。たとえば、年率 6% のローンを四半期払いで返済する場合、利率には 6%/4 = 1.5 (%) を指定します。"
			},
			{
				name: "期間",
				description: "には投資期間全体での支払回数の合計を指定します。"
			},
			{
				name: "定期支払額",
				description: "には 1 期間あたりの支払額を指定します。投資期間内に支払額を変更することはできません。"
			},
			{
				name: "将来価値",
				description: "には投資の将来価値、つまり最後の支払いを行った後に残る現金の収支を指定します。"
			},
			{
				name: "支払期日",
				description: "には支払いがいつ行われるかを表す論理値 (期首払い = 1、期末払い = 0 または省略) を指定します。"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "配列に含まれるデータから四分位数を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には対象となる数値データを含む配列、またはセル範囲を指定します。"
			},
			{
				name: "戻り値",
				description: "には戻り値を表す数値 (最小値 = 0、25% = 1、50% = 2、75% = 3、最大値 = 4) を指定します。"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "0 より大きく 1 より小さい百分位値に基づいて、配列に含まれるデータから四分位数を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には対象となる数値データを含む配列、またはセル範囲を指定します。"
			},
			{
				name: "戻り値",
				description: "には戻り値を表す数値 (最小値 = 0、25% = 1、50% = 2、75% = 3、最大値 = 4) を指定します。"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "0 以上 1 以下の百分位値に基づいて、配列に含まれるデータから四分位数を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には対象となる数値データを含む配列、またはセル範囲を指定します。"
			},
			{
				name: "戻り値",
				description: "には戻り値を表す数値 (最小値 = 0、25% = 1、50% = 2、75% = 3、最大値 = 4) を指定します。"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "除算の商の整数部を返します。.",
		arguments: [
			{
				name: "分子",
				description: "には、被除数 (割られる数) を指定します。"
			},
			{
				name: "分母",
				description: "には、除数 (割る数) を指定します。"
			}
		]
	},
	{
		name: "RADIANS",
		description: "度単位で表された角度をラジアンに変換した結果を返します。.",
		arguments: [
			{
				name: "角度",
				description: "には変換したい角度を度単位で指定します。"
			}
		]
	},
	{
		name: "RAND",
		description: "0 以上で 1 より小さい乱数を発生させます。再計算されるたびに、新しい乱数が返されます。.",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "指定された範囲で一様に分布する整数の乱数を返します。.",
		arguments: [
			{
				name: "最小値",
				description: "乱数の最小値を整数で指定します。"
			},
			{
				name: "最大値",
				description: "乱数の最大値を整数で指定します。"
			}
		]
	},
	{
		name: "RANGE",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "RANK",
		description: "順序に従って範囲内の数値を並べ替えたとき、数値が何番目に位置するかを返します。.",
		arguments: [
			{
				name: "数値",
				description: "には順位を調べる数値を指定します。"
			},
			{
				name: "参照",
				description: "には数値を含むセル範囲の参照、または配列を指定します。数値以外の値は無視されます。"
			},
			{
				name: "順序",
				description: "には範囲内の数値を並べ替える方法を表す数値を指定します。順序に 0 を指定するか省略すると、降順で並べ替えられ、0 以外の数値を指定すると、昇順で並べ替えられます。"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "順序に従って範囲内の数値を並べ替えたとき、数値が何番目に位置するかを返します。複数の数値が同じ順位にある場合は、順位の平均を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には順位を調べる数値を指定します。"
			},
			{
				name: "参照",
				description: "には数値を含むセル範囲の参照、または配列を指定します。数値以外の値は無視されます。"
			},
			{
				name: "順序",
				description: "には範囲内の数値を並べ替える方法を表す数値を指定します。順序に 0 を指定するか省略すると、降順で並べ替えられ、0 以外の数値を指定すると、昇順で並べ替えられます。"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "順序に従って範囲内の数値を並べ替えたとき、数値が何番目に位置するかを返します。複数の数値が同じ順位にある場合は、その値の中の最上位を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には順位を調べる数値を指定します。"
			},
			{
				name: "参照",
				description: "には数値を含むセル範囲の参照、または配列を指定します。数値以外の値は無視されます。"
			},
			{
				name: "順序",
				description: "には範囲内の数値を並べ替える方法を表す数値を指定します。順序に 0 を指定するか省略すると、降順で並べ替えられ、0 以外の数値を指定すると、昇順で並べ替えられます。"
			}
		]
	},
	{
		name: "RATE",
		description: "ローンまたは投資の 1 期間あたりの利率を指定します。たとえば、年率 6% のローンを四半期払いで返済する場合、利率には 6%/4 = 1.5 (%) を指定します。.",
		arguments: [
			{
				name: "期間",
				description: "にはローンまたは投資期間全体での支払回数の合計を指定します。"
			},
			{
				name: "定期支払額",
				description: "には毎回の支払額を指定します。ローンまたは投資期間内に支払額を変更することはできません。"
			},
			{
				name: "現在価値",
				description: "には投資の現在価値、つまり、将来行われる一連の支払いを現時点で一括払いした場合の合計金額、または元金を指定します。"
			},
			{
				name: "将来価値",
				description: "には投資の将来価値、つまり最後の支払いを行った後に残る現金の収支を指定します。将来価値を省略すると、0 を指定したと見なされます。"
			},
			{
				name: "支払期日",
				description: "には支払いがいつ行われるかを表す論理値 (期首払い = 1、期末払い = 0 または省略) を指定します。"
			},
			{
				name: "推定値",
				description: "には利率がおよそどれくらいになるかを推定した値を指定します。推定値を省略すると、0.1 (10%) が計算に使用されます。"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "全額投資された証券を対象に、満期日における償還価額を計算します。.",
		arguments: [
			{
				name: "受渡日",
				description: "には、証券の受渡日を日付のシリアル値で指定します。"
			},
			{
				name: "満期日",
				description: "には、証券の満期日を日付のシリアル値で指定します。"
			},
			{
				name: "投資額",
				description: "には、証券への投資額を指定します。"
			},
			{
				name: "割引率",
				description: "には、証券の割引率を指定します。"
			},
			{
				name: "基準",
				description: "には、利息計算の基礎となる日数の計算方法を数値で指定します。"
			}
		]
	},
	{
		name: "REPLACE",
		description: "文字列中の指定した位置の文字列を置き換えた結果を返します。半角と全角の区別なく、1 文字を 1 として処理します。.",
		arguments: [
			{
				name: "文字列",
				description: "には置き換えたい文字列が含まれる文字列を指定します。"
			},
			{
				name: "開始位置",
				description: "には <置換文字列> で置き換えたい文字列の先頭文字の位置を指定します。"
			},
			{
				name: "文字数",
				description: "には <置換文字列> で置き換えたい文字列の文字数を指定します。"
			},
			{
				name: "置換文字列",
				description: "には置き換え後の文字列を指定します。"
			}
		]
	},
	{
		name: "REPT",
		description: "文字列を指定された回数だけ繰り返して表示します。この関数を使用して、セル幅全体に文字列を表示することができます。.",
		arguments: [
			{
				name: "文字列",
				description: "には繰り返す文字列を指定します。"
			},
			{
				name: "繰り返し回数",
				description: "には文字列を繰り返す回数を、正の数値で指定します。"
			}
		]
	},
	{
		name: "RIGHT",
		description: "文字列の末尾から指定された文字数の文字を返します。.",
		arguments: [
			{
				name: "文字列",
				description: "には取り出す文字が含まれる文字列を指定します。"
			},
			{
				name: "文字数",
				description: "には取り出す文字数を指定します。省略すると、1 を指定したと見なされます。"
			}
		]
	},
	{
		name: "ROMAN",
		description: "アラビア数字を、ローマ数字を表す文字列に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "には変換したいアラビア数字を指定します。"
			},
			{
				name: "書式",
				description: "にはローマ数字の書式を数値で指定します。"
			}
		]
	},
	{
		name: "ROUND",
		description: "数値を指定した桁数に四捨五入した値を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には四捨五入の対象となる数値を指定します。"
			},
			{
				name: "桁数",
				description: "には四捨五入する桁数を指定します。桁数に負の数を指定すると、小数点の左側 (整数部分) の指定した桁 (1 の位を 0 とする) に、0 を指定すると、最も近い整数として四捨五入されます。"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "数値を切り捨てます。.",
		arguments: [
			{
				name: "数値",
				description: "には切り捨ての対象となる実数値を指定します。"
			},
			{
				name: "桁数",
				description: "には数値を切り捨てた結果の桁数を指定します。桁数に負の数を指定すると、数値は小数点の左 (整数部分) の指定した桁 (1 の位を 0 とする) に切り捨てられ、 0 を指定するかまたは省略されると、最も近い整数に切り捨てられます。"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "数値を切り上げます。.",
		arguments: [
			{
				name: "数値",
				description: "には切り上げの対象となる実数値を指定します。"
			},
			{
				name: "桁数",
				description: "には数値を切り上げた結果の桁数を指定します。桁数に負の数を指定すると、数値は小数点の左 (整数部分) の指定した桁 (1 の位を 0 とする) に切り上げられ、 0 を指定するかまたは省略されると、最も近い整数に切り上げられます。"
			}
		]
	},
	{
		name: "ROW",
		description: "参照の行番号を返します。.",
		arguments: [
			{
				name: "参照",
				description: "には行番号を調べるセルまたはセル範囲を指定します。範囲を省略すると、ROW 関数が入力されているセルの行番号が返されます。"
			}
		]
	},
	{
		name: "ROWS",
		description: "参照、または配列に含まれる行数を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には行数を求めたい配列、配列数式、またはセル範囲への参照を指定します。"
			}
		]
	},
	{
		name: "RRI",
		description: "投資の成長に対する等価利率を返します。.",
		arguments: [
			{
				name: "期間",
				description: "には投資の期間を指定します。"
			},
			{
				name: "現在価値",
				description: "には投資の現在価値を指定します。"
			},
			{
				name: "将来価値",
				description: "には投資の将来価値を指定します。"
			}
		]
	},
	{
		name: "RSQ",
		description: "指定されたデータ ポイントからピアソンの積率相関係数の 2 乗を返します。.",
		arguments: [
			{
				name: "既知のy",
				description: "にはデータ ポイントの配列、または範囲を指定します。引数には、数値、名前、配列、または数値を含むセル参照を指定します。"
			},
			{
				name: "既知のx",
				description: "にはデータ ポイントの配列、または範囲を指定します。"
			}
		]
	},
	{
		name: "RTD",
		description: "COM オートメーションをサポートするプログラムからリアルタイム データを取り込みます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "プログラムID",
				description: "には、登録された COM オートメーション アドインのプログラムID の名前を指定します。名前は二重引用符で囲みます。"
			},
			{
				name: "サーバー",
				description: "には、アドインを実行するサーバーの名前を指定します。名前は二重引用符で囲みます。アドインをローカルで実行する場合は、空文字列を使用します。"
			},
			{
				name: "トピック1",
				description: "には、データの一部を指定する 1 ～ 38 個の引数を指定します。"
			},
			{
				name: "トピック2",
				description: "には、データの一部を指定する 1 ～ 38 個の引数を指定します。"
			}
		]
	},
	{
		name: "SEARCH",
		description: "文字列が最初に現れる位置の文字番号を返します。大文字、小文字は区別されません。.",
		arguments: [
			{
				name: "検索文字列",
				description: "には検索する文字列を指定します。半角の疑問符 (?) または半角のアスタリスク (*) をワイルドカード文字として使用することができます。"
			},
			{
				name: "対象",
				description: "には検索文字列を含む文字列を指定します。"
			},
			{
				name: "開始位置",
				description: "には検索を開始する位置を、文字列の左から数えた文字数で指定します。省略すると、1 を指定したと見なされます。 "
			}
		]
	},
	{
		name: "SEC",
		description: "角度の正割を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には、正割を求める角度をラジアンを単位として指定します。"
			}
		]
	},
	{
		name: "SECH",
		description: "角度の双曲線正割を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には、双曲線正割を求める角度をラジアンを単位として指定します。"
			}
		]
	},
	{
		name: "SECOND",
		description: "秒を 0 ～ 59 の範囲の整数で返します。.",
		arguments: [
			{
				name: "シリアル値",
				description: "には、Spreadsheet で使用される日付/時刻コードか、または 16:48:23 や 4:48:47 PM のような時刻形式のテキストを指定します。"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "べき級数の和を計算します。.",
		arguments: [
			{
				name: "x",
				description: "には、べき級数に代入する値を指定します。"
			},
			{
				name: "n",
				description: "には、X のべき乗の初期値を指定します。"
			},
			{
				name: "m",
				description: "には、級数の各項ごとに n を増加させる値を指定します。"
			},
			{
				name: "係数",
				description: "には、X のべき乗である各項にかける一連の係数を指定します。"
			}
		]
	},
	{
		name: "SHEET",
		description: "参照されるシートのシート番号を返します。.",
		arguments: [
			{
				name: "値",
				description: "にはシート番号を求めるシートまたは参照の名前を指定します。省略した場合、関数を含むシート番号が返されます。"
			}
		]
	},
	{
		name: "SHEETS",
		description: "参照内のシート数を返します。.",
		arguments: [
			{
				name: "参照",
				description: "には含まれているシート数を求める参照を指定します。省略した場合、関数を含む、ブック内のシート数が返されます。"
			}
		]
	},
	{
		name: "SIGN",
		description: "数値の正負を返します。戻り値は、数値が正の数のときは 1、0 のときは 0、負の数のときは -1 となります。.",
		arguments: [
			{
				name: "数値",
				description: "には実数を指定します。"
			}
		]
	},
	{
		name: "SIN",
		description: "角度のサインを返します。.",
		arguments: [
			{
				name: "数値",
				description: "にはサインを求める角度をラジアンを単位として指定します。角度が度を単位として表されている場合は、PI()/180 を掛けるとラジアンに変換されます。"
			}
		]
	},
	{
		name: "SINH",
		description: "数値の双曲サインを返します。.",
		arguments: [
			{
				name: "数値",
				description: "には実数を指定します。"
			}
		]
	},
	{
		name: "SKEW",
		description: "分布の歪度 (ひずみ) を返します。歪度とは、分布の平均値周辺での両側の非対称度を表す値です。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には歪度を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には歪度を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "人口に基づく分布の歪度 (ひずみ) を返します。歪度とは、分布の平均値周辺での両側の非対称度を表す値です。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には歪度を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定します。"
			},
			{
				name: "数値2",
				description: "には歪度を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定します。"
			}
		]
	},
	{
		name: "SLN",
		description: "資産に対する減価償却を定額法を使って計算し、その結果を返します。.",
		arguments: [
			{
				name: "取得価額",
				description: "には資産を購入した時点での価格を指定します。"
			},
			{
				name: "残存価額",
				description: "には耐用年数を経た後での資産の価格を指定します。"
			},
			{
				name: "耐用年数",
				description: "には資産を使用できる年数 (償却の対象となる資産の寿命年数) を指定します。"
			}
		]
	},
	{
		name: "SLOPE",
		description: "指定されたデータ ポイントから線形回帰直線の傾きを返します。.",
		arguments: [
			{
				name: "既知のy",
				description: "には従属変数の値を含む数値配列、またはセル範囲を指定します。引数には、数値、名前、配列、または数値を含むセル参照を指定します。"
			},
			{
				name: "既知のx",
				description: "には独立変数の値を含む数値配列、あるいはセル範囲を指定します。引数には、数値、名前、配列、または数値を含むセル参照を指定します。"
			}
		]
	},
	{
		name: "SMALL",
		description: "データの中から、指定した順位番目に小さな値を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には対象となるデータが入力されている配列、またはセル範囲を指定します。"
			},
			{
				name: "順位",
				description: "には抽出する値の小さい方から数えた順位を数値で指定します。"
			}
		]
	},
	{
		name: "SQRT",
		description: "数値の正の平方根を返します。.",
		arguments: [
			{
				name: "数値",
				description: "には平方根を求めたい数値を指定します。"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "数値 x πの平方根の値を計算します。.",
		arguments: [
			{
				name: "数値",
				description: "には、πとかけ算する数値を指定します。"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "平均と標準偏差で決定される分布を対象に、正規化された値を返します。.",
		arguments: [
			{
				name: "x",
				description: "には正規化する値を指定します。"
			},
			{
				name: "平均",
				description: "には分布の算術平均を指定します。"
			},
			{
				name: "標準偏差",
				description: "には分布の標準偏差を正の数値で指定します。"
			}
		]
	},
	{
		name: "STDEV",
		description: "標本に基づいて予測した標準偏差を返します。標本内の論理値、および文字列は無視されます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には母集団の標本に対応する数値、または数値を含む参照を、1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には母集団の標本に対応する数値、または数値を含む参照を、1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "引数を母集団全体であると見なして、母集団の標準偏差を返します。論理値、および文字列は無視されます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には母集団に対応する数値、または数値を含む参照を、1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には母集団に対応する数値、または数値を含む参照を、1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "標本に基づいて予測した標準偏差を返します。標本内の論理値、および文字列は無視されます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には母集団の標本に対応する数値、または数値を含む参照を、1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には母集団の標本に対応する数値、または数値を含む参照を、1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "STDEVA",
		description: "論理値や文字列を含む標本に基づいて、予測した標準偏差を返します。文字列および論理値 FALSE は値 0、論理値 TRUE は 1 と見なされます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "値1",
				description: "には母集団の標本に対応する値、名前、または値への参照を、1 ～ 255 個まで指定できます。"
			},
			{
				name: "値2",
				description: "には母集団の標本に対応する値、名前、または値への参照を、1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "STDEVP",
		description: "引数を母集団全体であると見なして、母集団の標準偏差を返します。論理値、および文字列は無視されます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には母集団に対応する数値、または数値を含む参照を、1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には母集団に対応する数値、または数値を含む参照を、1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "論理値や文字列を含む引数を母集団全体と見なして、母集団の標準偏差を返します。文字列および論理値 FALSE は値 0、論理値 TRUE は値 1 と見なされます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "値1",
				description: "には母集団に対応する値、または値を含む名前、配列、参照を、1 ～ 255 個まで指定できます。"
			},
			{
				name: "値2",
				description: "には母集団に対応する値、または値を含む名前、配列、参照を、1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "STEYX",
		description: "回帰において、x に対して予測された値 y の標準誤差を返します。.",
		arguments: [
			{
				name: "既知のy",
				description: "には従属変数の値を含む数値配列またはセル範囲を指定します。引数には、数値、名前、配列、または数値を含むセル参照を指定します。"
			},
			{
				name: "既知のx",
				description: "には独立変数の値を含む数値配列またはセル範囲を指定します。引数には、数値、名前、配列、または数値を含むセル参照を指定します。"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "文字列中の指定した文字を新しい文字で置き換えます。.",
		arguments: [
			{
				name: "文字列",
				description: "には置き換える文字を含む文字列、または目的の文字列が入力されたセル参照を指定します。"
			},
			{
				name: "検索文字列",
				description: "には置き換え前の文字列を指定します。検索文字列と置換文字列の大文字小文字の表記が異なる場合、文字列は置換されません。"
			},
			{
				name: "置換文字列",
				description: "には置き換え後の文字列を指定します。"
			},
			{
				name: "置換対象",
				description: "には文字列に含まれるどの検索文字列を置換文字列に置き換えるかを指定します。省略された場合は、文字列中のすべての検索文字列が置き換えの対象となります。"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "リストまたはデータベースの集計値を返します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "集計方法",
				description: "にはリストの集計に使用する関数を、1 ～ 11 の番号で指定します。"
			},
			{
				name: "参照1",
				description: "には集計するリストの範囲または参照を 1 ～ 254 個まで指定します。"
			}
		]
	},
	{
		name: "SUM",
		description: "セル範囲に含まれる数値をすべて合計します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には合計を求めたい数値を 1 ～ 255 個まで指定できます。論理値および文字列は無視されますが、引数として入力されていれば計算の対象となります。"
			},
			{
				name: "数値2",
				description: "には合計を求めたい数値を 1 ～ 255 個まで指定できます。論理値および文字列は無視されますが、引数として入力されていれば計算の対象となります。"
			}
		]
	},
	{
		name: "SUMIF",
		description: "指定された検索条件に一致するセルの値を合計します。.",
		arguments: [
			{
				name: "範囲",
				description: "には評価の対象となるセル範囲を指定します。"
			},
			{
				name: "検索条件",
				description: "には計算の対象となるセルを定義する条件を数値、式、または文字列で指定します。"
			},
			{
				name: "合計範囲",
				description: "には実際に計算の対象となるセル範囲を指定します。合計範囲を省略すると、範囲内で検索条件を満たすセルが合計されます。"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "特定の条件に一致する数値の合計を求めます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "合計対象範囲",
				description: "には合計対象の実際のセルを指定します。"
			},
			{
				name: "条件範囲",
				description: "には、特定の条件による評価の対象となるセル範囲を指定します。"
			},
			{
				name: "条件",
				description: "には、計算の対象となるセルを定義する条件を数値、式、または文字列で指定します。"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "範囲または配列の対応する要素の積を合計した結果を返します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "配列1",
				description: "には要素の積の合計を求めたい配列を 2 ～ 255 個まで指定できます。引数となる配列は、行数と列数が等しい配列である必要があります。"
			},
			{
				name: "配列2",
				description: "には要素の積の合計を求めたい配列を 2 ～ 255 個まで指定できます。引数となる配列は、行数と列数が等しい配列である必要があります。"
			},
			{
				name: "配列3",
				description: "には要素の積の合計を求めたい配列を 2 ～ 255 個まで指定できます。引数となる配列は、行数と列数が等しい配列である必要があります。"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "引数の 2 乗の和 (平方和) を返します。引数には、数値、数値を含む名前、配列、セル参照を指定できます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には平方和を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には平方和を求めたい数値、または数値を含む名前、配列、セル参照を 1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "2 つの配列で対応する配列要素の平方差を合計します。.",
		arguments: [
			{
				name: "配列1",
				description: "には最初の範囲、または値の配列を指定します。引数には、数値、名前、配列、あるいは数値を含むセル参照を指定します。"
			},
			{
				name: "配列2",
				description: "には 2 番めの範囲、または値の配列を指定します。引数には、数値、名前、配列、あるいは数値を含むセル参照を指定します。"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "2 つの配列の対応する値の積を合計した結果を返します。.",
		arguments: [
			{
				name: "配列1",
				description: "には先頭の範囲、または値の配列を指定します。"
			},
			{
				name: "配列2",
				description: "には 2 番めの範囲、または値の配列を指定します。"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "2 つの配列で対応する配列要素の差を 2 乗し、さらにその合計を返します。.",
		arguments: [
			{
				name: "配列1",
				description: "には対象となる一方の範囲、または数値の配列を指定します。引数には、数値、名前、配列、または数値を含む参照を指定します。"
			},
			{
				name: "配列2",
				description: "には対象となるもう一方の範囲、または数値の配列を指定します。引数には、数値、名前、配列、または数値を含む参照を指定します。"
			}
		]
	},
	{
		name: "SYD",
		description: "資産に対する減価償却を級数法を使って計算し、その結果を返します。.",
		arguments: [
			{
				name: "取得価額",
				description: "には資産を購入した時点での価格を指定します。"
			},
			{
				name: "残存価額",
				description: "には耐用年数を経た後での資産の価格を指定します。"
			},
			{
				name: "耐用年数",
				description: "には資産を使用できる年数、つまり償却の対象となる資産の寿命年数を指定します。"
			},
			{
				name: "期",
				description: "には減価償却費を計算する期を指定します。"
			}
		]
	},
	{
		name: "T",
		description: "値が文字列を参照する場合はその文字列を返し、文字列以外のデータを参照する場合は、空文字列 ('') を返します。.",
		arguments: [
			{
				name: "値",
				description: "には文字列に変換する値を指定します。"
			}
		]
	},
	{
		name: "T.DIST",
		description: "左側のスチューデントの t-分布を返します。.",
		arguments: [
			{
				name: "x",
				description: "には分布を計算する数値を指定します。"
			},
			{
				name: "自由度",
				description: "には自由度を表す整数値を指定します。"
			},
			{
				name: "関数形式",
				description: "には関数の形式を表す論理値を指定します。TRUE を指定した場合は累積分布関数が返され、FALSE を指定した場合は確率密度関数が返されます。"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "両側のスチューデントの t-分布を返します。.",
		arguments: [
			{
				name: "x",
				description: "には分布を計算する数値を指定します。"
			},
			{
				name: "自由度",
				description: "には自由度を表す整数値を指定します。"
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "右側のスチューデントの t-分布を返します。.",
		arguments: [
			{
				name: "x",
				description: "には分布を計算する数値を指定します。"
			},
			{
				name: "自由度",
				description: "には自由度を表す整数値を指定します。"
			}
		]
	},
	{
		name: "T.INV",
		description: "スチューデントの t-分布の左側逆関数を返します。.",
		arguments: [
			{
				name: "確率",
				description: "にはスチューデントの t-分布の両側確率を 0 ～ 1 の数値で 指定します。"
			},
			{
				name: "自由度",
				description: "には分布の自由度を示す正の整数を指定します。"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "スチューデントの t-分布の両側逆関数を返します。.",
		arguments: [
			{
				name: "確率",
				description: "にはスチューデントの t-分布の両側確率を 0 ～ 1 の数値で 指定します。"
			},
			{
				name: "自由度",
				description: "には分布の自由度を示す正の整数を指定します。"
			}
		]
	},
	{
		name: "T.TEST",
		description: "スチューデントの t 検定に関連する確率を返します。.",
		arguments: [
			{
				name: "配列1",
				description: "には一方のデータ配列を指定します。"
			},
			{
				name: "配列2",
				description: "にはもう一方のデータ配列を指定します。"
			},
			{
				name: "検定の指定",
				description: "には片側検定の場合は 1、両側検定の場合は 2 を指定します。"
			},
			{
				name: "検定の種類",
				description: "には実行する t 検定の種類を指定します。対応のある検定の場合は 1、2 標本の等分散が仮定できる場合は 2、2 標本が非等分散の場合は 3 を指定します。"
			}
		]
	},
	{
		name: "TAN",
		description: "角度のタンジェントを返します。.",
		arguments: [
			{
				name: "数値",
				description: "にはタンジェントを求めたい角度をラジアンを単位として指定します。"
			}
		]
	},
	{
		name: "TANH",
		description: "数値の双曲タンジェントを返します。.",
		arguments: [
			{
				name: "数値",
				description: "には実数を指定します。"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "米国財務省短期証券 (TB) の債券相当の利回りを計算します。.",
		arguments: [
			{
				name: "受渡日",
				description: "には、財務省短期証券の受渡日を日付のシリアル値で指定します。"
			},
			{
				name: "満期日",
				description: "には、財務省短期証券の満期日を日付のシリアル値で指定します。"
			},
			{
				name: "割引率",
				description: "には、財務省短期証券の割引率を指定します。"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "米国財務省短期証券 (TB) の額面価格 $100 に対する価格を計算します。.",
		arguments: [
			{
				name: "受渡日",
				description: "には、財務省短期証券の受渡日を日付のシリアル値で指定します。"
			},
			{
				name: "満期日",
				description: "には、財務省短期証券の満期日を日付のシリアル値で指定します。"
			},
			{
				name: "割引率",
				description: "には、財務省短期証券の割引率を指定します。"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "米国財務省短期証券 (TB) の利回りを計算します。.",
		arguments: [
			{
				name: "受渡日",
				description: "には、財務省短期証券の受渡日を日付のシリアル値で指定します。"
			},
			{
				name: "満期日",
				description: "には、財務省短期証券の満期日を日付のシリアル値で指定します。"
			},
			{
				name: "現在価格",
				description: "には、財務省短期証券の額面価格 $100 に対する価格を指定します。"
			}
		]
	},
	{
		name: "TDIST",
		description: "スチューデントの t-分布を返します。.",
		arguments: [
			{
				name: "x",
				description: "には分布を計算する数値を指定します。"
			},
			{
				name: "自由度",
				description: "には自由度を表す整数値を指定します。"
			},
			{
				name: "分布の指定",
				description: "には片側確率を求める場合は 1、両側確率を求める場合は 2 を指定します。"
			}
		]
	},
	{
		name: "TEXT",
		description: "数値に指定した書式を設定し、文字列に変換した結果を返します。.",
		arguments: [
			{
				name: "値",
				description: "には数値、結果が数値となる数式、または数値が入力されているセルへの参照を指定します。"
			},
			{
				name: "表示形式",
				description: "には [セルの書式設定] ダイアログ ボックスの [表示形式] タブの [分類] ボックスに表示されている数値形式を、文字列として指定します。"
			}
		]
	},
	{
		name: "TIME",
		description: "指定した時刻を表すシリアル値 (0:00:00 (午前 12:00:00) から 23:59:59 (午後 11:59:59) までを表す 0 から 0.9999999 の範囲の小数値) を返します。.",
		arguments: [
			{
				name: "時",
				description: "には時を表す 0 から 23 の範囲の数値を指定します。"
			},
			{
				name: "分",
				description: "には分を表す 0 から 59 の範囲の数値を指定します。"
			},
			{
				name: "秒",
				description: "には秒を表す 0 から 59 の範囲の数値を指定します。"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "文字列で表された時刻を、シリアル値 (0 (午前 0 時) から 0.999988426 (午後 11 時 59 分 59 秒) までの数値) に変換します。数式の入力後に、数値を時刻表示形式に設定します。.",
		arguments: [
			{
				name: "時刻文字列",
				description: "には時刻を表す文字列を、Spreadsheet の組み込みの時刻表示形式で指定します。日付の情報は無視されます。"
			}
		]
	},
	{
		name: "TINV",
		description: "スチューデントの t-分布の両側逆関数を返します。.",
		arguments: [
			{
				name: "確率",
				description: "にはスチューデントの t-分布の両側確率を 0 ～ 1 の数値で 指定します。"
			},
			{
				name: "自由度",
				description: "には分布の自由度を示す正の整数を指定します。"
			}
		]
	},
	{
		name: "TODAY",
		description: "現在の日付を表すシリアル値 (Spreadsheet で日付や時刻の計算で使用されるコード) を返します。.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "配列の縦方向と横方向のセル範囲の変換を行います。.",
		arguments: [
			{
				name: "配列",
				description: "には行列変換を行うワークシートのセル範囲または値の配列を指定します。"
			}
		]
	},
	{
		name: "TREND",
		description: "最小二乗法を使用することで、既知のデータ ポイントに対応する線形トレンドの数値を返します。.",
		arguments: [
			{
				name: "既知のy",
				description: "には y = mx + b となる、既にわかっている y 値の範囲または配列を指定します。"
			},
			{
				name: "既知のx",
				description: "には y = mx + b となる、既にわかっている x 値の範囲または配列 (既知のy と同じサイズの配列) を指定します。この値は省略可能です。"
			},
			{
				name: "新しいx",
				description: "には TREND 関数を利用して、対応する y の値を計算する新しい x の値の範囲または配列を指定します。"
			},
			{
				name: "定数",
				description: "には定数 b を 0 にするかどうかを論理値で指定します。TRUE に指定するか省略すると、b の値も計算され、FALSE を指定すると、b の値が 0 に設定されます。"
			}
		]
	},
	{
		name: "TRIM",
		description: "単語間のスペースを 1 つずつ残して、不要なスペースをすべて削除します。.",
		arguments: [
			{
				name: "文字列",
				description: "には余分なスペースを削除する文字列を指定します。"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "データ全体の上限と下限から一定の割合のデータを切り落とし、残りの項の平均値を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には関数の対象となる数値を含む配列、または範囲を指定します。"
			},
			{
				name: "割合",
				description: "には平均値の計算から排除するデータの割合を小数で指定します。"
			}
		]
	},
	{
		name: "TRUE",
		description: "論理値 TRUE を返します。.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "数値の小数部を切り捨てて、整数または指定した桁数に変換します。.",
		arguments: [
			{
				name: "数値",
				description: "には小数部を切り捨てる数値を指定します。"
			},
			{
				name: "桁数",
				description: "には切り捨てを行った後の桁数を指定します。桁数の既定値は 0 (ゼロ) です。"
			}
		]
	},
	{
		name: "TTEST",
		description: "スチューデントの t 検定に関連する確率を返します。.",
		arguments: [
			{
				name: "配列1",
				description: "には一方のデータ配列を指定します。"
			},
			{
				name: "配列2",
				description: "にはもう一方のデータ配列を指定します。"
			},
			{
				name: "検定の指定",
				description: "には片側検定の場合は 1、両側検定の場合は 2 を指定します。"
			},
			{
				name: "検定の種類",
				description: "には実行する t 検定の種類を指定します。対応のある検定の場合は 1、2 標本の等分散が仮定できる場合は 2、2 標本が非等分散の場合は 3 を指定します。"
			}
		]
	},
	{
		name: "TYPE",
		description: "値のデータ タイプを表す数値 (数値 = 1、文字列 = 2、論理値 = 4、エラー値 = 16、配列 = 64) を返します。.",
		arguments: [
			{
				name: "データタイプ",
				description: "数値、文字列、論理値など Spreadsheet で処理できる値であれば、何でも指定できます。"
			}
		]
	},
	{
		name: "UNICODE",
		description: "文字列の最初の文字に対応する番号 (コード ポイント) を返します。.",
		arguments: [
			{
				name: "文字列",
				description: "には Unicode 値を求める文字を指定します。"
			}
		]
	},
	{
		name: "UPPER",
		description: "文字列に含まれる英字をすべて大文字に変換します。.",
		arguments: [
			{
				name: "文字列",
				description: "には大文字に変換する文字列を指定します。文字列には、目的の文字列が入力されたセル参照を指定することもできます。"
			}
		]
	},
	{
		name: "VALUE",
		description: "文字列として入力されている数字を数値に変換します。.",
		arguments: [
			{
				name: "文字列",
				description: "には文字列を半角の二重引用符 (') で囲んで指定するか、または変換する文字列を含むセル参照を指定します。"
			}
		]
	},
	{
		name: "VAR",
		description: "標本に基づいて母集団の分散の推定値 (不偏分散) を返します。標本内の論理値、および文字列は無視されます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には母集団の標本に対応する数値を 1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には母集団の標本に対応する数値を 1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "VAR.P",
		description: "引数を母集団全体と見なし、母集団の分散 (標本分散) を返します。論理値、および文字列は無視されます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には母集団に対応する数値を 1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には母集団に対応する数値を 1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "VAR.S",
		description: "標本に基づいて母集団の分散の推定値 (不偏分散) を返します。標本内の論理値、および文字列は無視されます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には母集団の標本に対応する数値を 1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には母集団の標本に対応する数値を 1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "VARA",
		description: "標本に基づく、分散の予測値を返します。文字列および論理値 FALSE は値 0、論理値 TRUE は値 1 と見なされます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "値1",
				description: "には母集団の標本に対応する値を 1 から 255 個まで指定できます。"
			},
			{
				name: "値2",
				description: "には母集団の標本に対応する値を 1 から 255 個まで指定できます。"
			}
		]
	},
	{
		name: "VARP",
		description: "引数を母集団全体と見なし、母集団の分散 (標本分散) を返します。論理値、および文字列は無視されます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "数値1",
				description: "には母集団に対応する数値を 1 ～ 255 個まで指定できます。"
			},
			{
				name: "数値2",
				description: "には母集団に対応する数値を 1 ～ 255 個まで指定できます。"
			}
		]
	},
	{
		name: "VARPA",
		description: "母集団全体に基づく分散を返します。文字列および論理値 FALSE は値 0、論理値 TRUE は 1 と見なされます。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "値1",
				description: "には母集団に対応する値を 1 から 255 個まで指定できます。"
			},
			{
				name: "値2",
				description: "には母集団に対応する値を 1 から 255 個まで指定できます。"
			}
		]
	},
	{
		name: "VDB",
		description: "倍額定率法または指定された方法を使用して、特定の期における資産の減価償却費を返します。.",
		arguments: [
			{
				name: "取得価額",
				description: "には資産を購入した時点での価格を指定します。"
			},
			{
				name: "残存価額",
				description: "には耐用年数が終了した時点での資産の価格を指定します。"
			},
			{
				name: "耐用年数",
				description: "には償却の対象となる資産の寿命年数を指定します。(耐用年数)"
			},
			{
				name: "開始期",
				description: "には減価償却費の計算の対象となる最初の期を、耐用年数と同じ単位で指定します。"
			},
			{
				name: "終了期",
				description: "には減価償却費の対象となる最後の期を、耐用年数と同じ単位で指定します。"
			},
			{
				name: "率",
				description: "には減価償却率を指定します。率を省略すると、2 を指定したと見なされ、倍額定率法で計算が行われます。"
			},
			{
				name: "切り替えなし",
				description: "には減価償却費が倍率法による計算結果より大きくなったときに、自動的に定額法に切り替えるかどうかを、論理値 (切り替え = FALSE または省略、切り替えなし = TRUE) で指定します。"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "指定された範囲の 1 列目で特定の値を検索し、指定した列と同じ行にある値を返します。テーブルは昇順で並べ替えておく必要があります。.",
		arguments: [
			{
				name: "検索値",
				description: "には範囲の先頭列で検索する値を指定します。検索値には、値、セル参照、または文字列を指定します。"
			},
			{
				name: "範囲",
				description: "には目的のデータが含まれる文字列、数値、または論理値のテーブルを指定します。セル範囲の参照、またはセル範囲名を指定します。"
			},
			{
				name: "列番号",
				description: "は範囲の列番号を指定します。ここで指定された列で一致する値が返されます。範囲の先頭列には 1 を指定します。"
			},
			{
				name: "検索方法",
				description: "には検索値と完全に一致する値だけを検索するか、その近似値を含めて検索するかを、論理値 (近似値を含めて検索 = TRUE または省略、完全一致の値を検索 = FALSE) で指定します。"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "日付に対応する曜日を 1 から 7 までの整数で返します。.",
		arguments: [
			{
				name: "シリアル値",
				description: "は日付を表す数値です。"
			},
			{
				name: "種類",
				description: "には戻り値の種類を表す 1 (日曜 = 1 ～ 土曜 = 7)、2 (月曜 = 1 ～ 日曜 = 7)、3 (月曜 = 0 ～ 日曜 = 6) のいずれかの数字を指定します。"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "日付がその年の第何週目に当たるかを返します。.",
		arguments: [
			{
				name: "シリアル値",
				description: "Spreadsheet で日付や時刻の計算に使用される日付/時刻コードを指定します。"
			},
			{
				name: "週の基準",
				description: "週の始まりを何曜日とするかを 1 か 2 の数値で指定します。"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "ワイブル分布の値を返します。.",
		arguments: [
			{
				name: "x",
				description: "には関数に代入する値を負でない数値で指定します。"
			},
			{
				name: "α",
				description: "には確率分布のパラメーターを正の数値で指定します。"
			},
			{
				name: "β",
				description: "には確率分布のパラメーターを正の数値で指定します。"
			},
			{
				name: "関数形式",
				description: "には論理値を指定します。TRUE を指定した場合は、累積分布関数が返され、FALSE を指定した場合は、確率量関数が返されます。"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "ワイブル分布の値を返します。.",
		arguments: [
			{
				name: "x",
				description: "には関数に代入する値を負でない数値で指定します。"
			},
			{
				name: "α",
				description: "には確率分布のパラメーターを正の数値で指定します。"
			},
			{
				name: "β",
				description: "には確率分布のパラメーターを正の数値で指定します。"
			},
			{
				name: "関数形式",
				description: "には関数を示す論理値を指定します。TRUE を指定した場合は累積分布関数が返され、FALSE を指定した場合は確率量関数が返されます。"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "開始日から起算して日数で指定した日数だけ前あるいは後の日付に対応するシリアル値を計算します。.",
		arguments: [
			{
				name: "開始日",
				description: "対象となる期間の初日となる日付のシリアル値を指定します。"
			},
			{
				name: "日数",
				description: "開始日から起算して、週末あるいは祭日を除く週日の日数を指定します。"
			},
			{
				name: "祭日",
				description: "省略可能な引数で、国民の祝日などの日数を計算にいれないため対応する日付のシリアル値を指定します。"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "ユーザー定義の週末パラメーターを使用して、指定した日数だけ前あるいは後の日付に対応するシリアル値を計算します。.",
		arguments: [
			{
				name: "開始日",
				description: "は期間の開始日となる日付のシリアル値を指定します。"
			},
			{
				name: "日数",
				description: "は開始日から起算して、週末あるいは祭日を除く週日の日数を指定します。"
			},
			{
				name: "週末",
				description: "は週末の開始を指定する週末の番号または文字列です。"
			},
			{
				name: "祭日",
				description: "は省略可能な引数で、国民の祝日などの日数を計算にいれないため対応する日付のシリアル値を指定します。"
			}
		]
	},
	{
		name: "XIRR",
		description: "一連のキャッシュフロー (投資と収益の金額) に基づいて、投資の内部利益率を計算します。.",
		arguments: [
			{
				name: "範囲",
				description: "には、日付で指定されるスケジュールに対応した一連のキャッシュフローを数値配列として指定します。"
			},
			{
				name: "日付",
				description: "には、キャッシュフローに対応する支払日のスケジュールを数値配列として指定します。"
			},
			{
				name: "推定値",
				description: "は、省略可能な引数で、XIRR 関数の計算結果に近いと思われる数値を指定します。"
			}
		]
	},
	{
		name: "XNPV",
		description: "一連のキャッシュフロー (投資と収益の金額) に基づいて、投資の正味現在価値を計算します。.",
		arguments: [
			{
				name: "割引率",
				description: "には、対象となるキャッシュフローに適用する割引率を指定します。"
			},
			{
				name: "キャッシュフロー",
				description: "には、日付で指定されるスケジュールに対応した一連のキャッシュフローを数値配列として指定します。"
			},
			{
				name: "日付",
				description: "には、キャッシュフローに対応する支払日のスケジュールを数値配列として指定します。"
			}
		]
	},
	{
		name: "XOR",
		description: "すべての引数の排他的論理和を返します。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "論理式1",
				description: "には結果が TRUE または FALSE になる、1 ～ 254 の論理式を指定します。引数には論理値、配列、または参照を指定します。"
			},
			{
				name: "論理式2",
				description: "には結果が TRUE または FALSE になる、1 ～ 254 の論理式を指定します。引数には論理値、配列、または参照を指定します。"
			}
		]
	},
	{
		name: "YEAR",
		description: "年を 1900 ～ 9999 の範囲の整数で返します。.",
		arguments: [
			{
				name: "シリアル値",
				description: "には Spreadsheet で使用される日付/時刻コードを指定します。"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "開始日から終了日までの間の日数を、年を単位とする数値で表します。.",
		arguments: [
			{
				name: "開始日",
				description: "には、対象となる期間の初日となる日付のシリアル値を指定します。"
			},
			{
				name: "終了日",
				description: "には、対象となる期間の最終日となる日付のシリアル値を指定します。"
			},
			{
				name: "基準",
				description: "には、日数の計算方法を数値で指定します。"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "米国財務省短期証券 (TB) などの割引債の年利回りを計算します。.",
		arguments: [
			{
				name: "受渡日",
				description: "には、証券の受渡日を日付のシリアル値で指定します。"
			},
			{
				name: "満期日",
				description: "には、証券の満期日を日付のシリアル値で指定します。"
			},
			{
				name: "現在価値",
				description: "には、額面 $100 に対する証券の価格を指定します。"
			},
			{
				name: "償還価額",
				description: "には、額面 $100 に対する証券の償還価額を指定します。"
			},
			{
				name: "基準",
				description: "には、利息計算の基礎となる日数の計算方法を数値で指定します。"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "z 検定の片側確率の P 値を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には x の検定対象となる配列またはデータ範囲を指定します。"
			},
			{
				name: "x",
				description: "には検定する値を指定します。"
			},
			{
				name: "σ",
				description: "には母集団全体に基づく標準偏差を指定します。省略すると、標本に基づく標準偏差が使用されます。"
			}
		]
	},
	{
		name: "ZTEST",
		description: "z 検定の片側確率の P 値を返します。.",
		arguments: [
			{
				name: "配列",
				description: "には x の検定対象となる配列またはデータ範囲を指定します。"
			},
			{
				name: "x",
				description: "には検定する値を指定します。"
			},
			{
				name: "σ",
				description: "には母集団全体に基づく標準偏差を指定します。省略すると、標本に基づく標準偏差が使用されます。"
			}
		]
	}
];