ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "傳回一數值的絕對值，亦即無正負號的數值.",
		arguments: [
			{
				name: "number",
				description: "為欲求算其絕對值的實數。"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "傳回證券在到期時支付利息的應計利息.",
		arguments: [
			{
				name: "issue",
				description: "是證券的發行日期，以數列日期數字表示"
			},
			{
				name: "settlement",
				description: "是證券的到期日期，以數列日期數字表示"
			},
			{
				name: "rate",
				description: "是證券的年度票息率"
			},
			{
				name: "par",
				description: "是證券的票面價值"
			},
			{
				name: "basis",
				description: "是要使用的日計數基礎類型"
			}
		]
	},
	{
		name: "ACOS",
		description: "傳回一數值的反餘弦值，傳回值以弧度表示，介於 0 和 Pi 之間。反餘弦值是其餘弦值為 Number 的角度。.",
		arguments: [
			{
				name: "number",
				description: "為欲求角度的餘弦值，其值必須介於 1 和 -1 之間。"
			}
		]
	},
	{
		name: "ACOSH",
		description: "傳回一數值的反雙曲線餘弦值.",
		arguments: [
			{
				name: "number",
				description: "為大於或等於 1 的任意實數。"
			}
		]
	},
	{
		name: "ACOT",
		description: "傳回數字的反餘切值，以弧度表示，介於 0 和 Pi 之間。.",
		arguments: [
			{
				name: "number",
				description: "為所要角度的餘切值"
			}
		]
	},
	{
		name: "ACOTH",
		description: "傳回數字的反雙曲餘切值.",
		arguments: [
			{
				name: "number",
				description: "為所要角度的雙曲餘切值"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "根據指定的欄列號碼，傳回代表儲存格位址的字串。.",
		arguments: [
			{
				name: "row_num",
				description: "為儲存格位址的列號，例如 1 即代表第一列。"
			},
			{
				name: "column_num",
				description: "為儲存格位址的欄號，例如 4 即代表第四列。"
			},
			{
				name: "abs_num",
				description: "傳回參照位址的方式。1: 傳回絕對位址; 2: 列位址為絕對，欄為相對; 3: 列位址為相對，欄為絕對; 4: 傳回相對位址。"
			},
			{
				name: "a1",
				description: "選擇用 A1 或 R1C1 格式來表示參照位址。為 1 或 TRUE 時，以 A1 格式表示; 為 0 或 FALSE 時，以 R1C1 格式表示。"
			},
			{
				name: "sheet_text",
				description: "為外部參照的工作表名稱。"
			}
		]
	},
	{
		name: "AND",
		description: "檢查所有的引數是否皆為 TRUE 並傳回 TRUE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "為欲測試之 1 到 255 個 TRUE 或 FALSE 的條件式，並且可為邏輯值、陣列或參照位址。"
			},
			{
				name: "logical2",
				description: "為欲測試之 1 到 255 個 TRUE 或 FALSE 的條件式，並且可為邏輯值、陣列或參照位址。"
			}
		]
	},
	{
		name: "ARABIC",
		description: "將羅馬數字轉換為阿拉伯數字.",
		arguments: [
			{
				name: "text",
				description: "為要轉換的羅馬數字"
			}
		]
	},
	{
		name: "AREAS",
		description: "傳回參照位址中區域的個數。一個區域是一組連續的儲存格範圍或單一儲存格。.",
		arguments: [
			{
				name: "reference",
				description: "是個參照到某單一儲存格或某儲存格範圍的參照位址，這個參照位址可以指向多塊區域。"
			}
		]
	},
	{
		name: "ASIN",
		description: "傳回一數值的反正弦值，傳回值以弧度表示，介於 -Pi/2 和 Pi/2 之間。.",
		arguments: [
			{
				name: "number",
				description: "為欲求角度的正弦值，其值必須介於 1 和 -1 之間。"
			}
		]
	},
	{
		name: "ASINH",
		description: "傳回一數值之反雙曲線正弦值.",
		arguments: [
			{
				name: "number",
				description: "為大於或等於 1 的任意實數。"
			}
		]
	},
	{
		name: "ATAN",
		description: "傳回一數值之反正切值，以弧度表示，介於 -Pi/2 和 Pi/2 之間.",
		arguments: [
			{
				name: "number",
				description: "為欲求算之角度的正切值。"
			}
		]
	},
	{
		name: "ATAN2",
		description: "根據所指定的 X 及 Y 座標值，傳回反正切值。傳回值以弧度表示，介於 -Pi 及 Pi 之間，但不包含 -Pi。.",
		arguments: [
			{
				name: "x_num",
				description: "為 X 軸座標值。"
			},
			{
				name: "y_num",
				description: "為 Y 軸座標值。"
			}
		]
	},
	{
		name: "ATANH",
		description: "傳回一數值的反雙曲線正切值.",
		arguments: [
			{
				name: "number",
				description: "為介於 -1 及 1 之間但不包含 1 及 -1 的任意實數。"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "傳回傳回各資料絕對平均差的平均值 (根據它們的平均)。引數可以是數字或名稱、陣列或是含有數字的參照.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個用來求其計算絕對平均差的引數。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個用來求其計算絕對平均差的引數。"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "傳回其引數的平均值 (算術平均值)，引數可為數字，或是包含數字的名稱、陣列、或參照位址.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個欲求其平均值的數值引數。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個欲求其平均值的數值引數。"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "傳回引數的平均值 (算術平均值)，引數若求得文字及 FALSE 則被視為 0; TRUE 則視為 1。引數可為數字、名稱、陣列或參照.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "是 1 到 255 個欲求其平均值的引數。"
			},
			{
				name: "value2",
				description: "是 1 到 255 個欲求其平均值的引數。"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "找出特定條件或準則所指定儲存格的平均值 (算術平均值).",
		arguments: [
			{
				name: "range",
				description: "是您要評估的儲存格範圍"
			},
			{
				name: "criteria",
				description: "是以數字、運算式或文字為形式的條件或準則，這會定義要使用哪些儲存格以找出平均值"
			},
			{
				name: "average_range",
				description: "是用來找出平均值的實際儲存格。如果被忽略，會使用範圍中的儲存格"
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "找出特定條件或準則集所指定儲存格的平均值 (算術平均值).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "average_range",
				description: "是用來找出平均值的實際儲存格。"
			},
			{
				name: "criteria_range",
				description: "是您要以特定條件評估的儲存格範圍"
			},
			{
				name: "criteria",
				description: "是以數字、運算式或文字為形式的條件或準則，這會定義要用來找出平均值的儲存格"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "將數值轉成文字(泰銖).",
		arguments: [
			{
				name: "number",
				description: "為所要轉換的數值。"
			}
		]
	},
	{
		name: "BASE",
		description: "將數字轉換為含有指定基數 (底數) 的文字表示法.",
		arguments: [
			{
				name: "number",
				description: "為要轉換的數字"
			},
			{
				name: "radix",
				description: "為要把數字轉換成的基底"
			},
			{
				name: "min_length",
				description: "為所傳回之字串的長度下限 (如果沒有加上省略的前置零字元)"
			}
		]
	},
	{
		name: "BESSELI",
		description: "傳回已修改的 Bessel 函數 In(x).",
		arguments: [
			{
				name: "x",
				description: "是要評估函數的值"
			},
			{
				name: "n",
				description: "是 Bessel 函數的冪次"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "傳回 Bessel 函數 Jn(x).",
		arguments: [
			{
				name: "x",
				description: "是要評估函數的值"
			},
			{
				name: "n",
				description: "是 Bessel 函數的冪次"
			}
		]
	},
	{
		name: "BESSELK",
		description: "傳回已修改的 Bessel 函數 Kn(x).",
		arguments: [
			{
				name: "x",
				description: "是要評估函數的值"
			},
			{
				name: "n",
				description: "是函數的冪次"
			}
		]
	},
	{
		name: "BESSELY",
		description: "傳回 Bessel 函數 Yn(x).",
		arguments: [
			{
				name: "x",
				description: "是要評估函數的值"
			},
			{
				name: "n",
				description: "是函數的冪次"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "傳回 beta 機率分配函數.",
		arguments: [
			{
				name: "x",
				description: "為介於 A 和 B 間對此函數求算的值"
			},
			{
				name: "alpha",
				description: "為此種分配的一個參數，其值必須大於 0"
			},
			{
				name: "beta",
				description: "為此種分配的一個參數，其值必須大於 0"
			},
			{
				name: "cumulative",
				description: "為一邏輯值; 當為 TRUE 時，採用累加分配函數; 為 FALSE 時，採用機率密度函數"
			},
			{
				name: "A",
				description: "為 x 區間下限 (選擇性引數)。若省略則 A = 0"
			},
			{
				name: "B",
				description: "是 x 區間上限 (選擇性引數)。若省略則 B = 1。"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "傳回累加 beta 機率密度反函數 (BETA.DIST).",
		arguments: [
			{
				name: "probability",
				description: "為 Beta 分配之機率值"
			},
			{
				name: "alpha",
				description: "為此種分配的一個參數，其值必須大於 0"
			},
			{
				name: "beta",
				description: "為此種分配的一個參數，其值必須大於 0"
			},
			{
				name: "A",
				description: "為 x 區間下限 (選擇性引數)。若省略則 A = 0"
			},
			{
				name: "B",
				description: "是 x 區間上限 (選擇性引數)。若省略則 B = 1。"
			}
		]
	},
	{
		name: "BETADIST",
		description: "傳回累加 beta 機率密度函數.",
		arguments: [
			{
				name: "x",
				description: "為介於 A 和 B 間對此函數求算的值"
			},
			{
				name: "alpha",
				description: "為此種分配的一個參數，其值必須大於 0"
			},
			{
				name: "beta",
				description: "為此種分配的一個參數，其值必須大於 0"
			},
			{
				name: "A",
				description: "為 x 區間下限 (選擇性引數)。若省略則 A = 0"
			},
			{
				name: "B",
				description: "是 x 區間上限 (選擇性引數)。若省略則 B = 1。"
			}
		]
	},
	{
		name: "BETAINV",
		description: "傳回累加 beta 機率密度反函數 (BETADIST).",
		arguments: [
			{
				name: "probability",
				description: "為 Beta 分配之機率值"
			},
			{
				name: "alpha",
				description: "為此種分配的一個參數，其值必須大於 0"
			},
			{
				name: "beta",
				description: "為此種分配的一個參數，其值必須大於 0"
			},
			{
				name: "A",
				description: "為 x 區間下限 (選擇性引數)。若省略則 A = 0"
			},
			{
				name: "B",
				description: "是 x 區間上限 (選擇性引數)。若省略則 B = 1。"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "將二進位數字轉換成十進位.",
		arguments: [
			{
				name: "number",
				description: "是您要轉換的二進位數字"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "將二進位數字轉換成十六進位.",
		arguments: [
			{
				name: "number",
				description: "是您要轉換的二進位數字"
			},
			{
				name: "places",
				description: "是要使用的字元數"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "將二進位數字轉換成八進位.",
		arguments: [
			{
				name: "number",
				description: "是您要轉換的二進位數字"
			},
			{
				name: "places",
				description: "是要使用的字元數"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "傳回在特定次數之二項分配實驗中，實驗成功的機率.",
		arguments: [
			{
				name: "number_s",
				description: "為實驗成功的次數"
			},
			{
				name: "trials",
				description: "為獨立實驗的次數"
			},
			{
				name: "probability_s",
				description: "為每一次實驗的成功機率"
			},
			{
				name: "cumulative",
				description: "為一邏輯值: TRUE 則採用累加分配函數; FALSE 則採用機率質量函數。"
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "傳回使用二項分配之實驗結果的機率.",
		arguments: [
			{
				name: "trials",
				description: "是獨立實驗的數目"
			},
			{
				name: "probability_s",
				description: "是每一次實驗的成功機率"
			},
			{
				name: "number_s",
				description: "是實驗成功次數"
			},
			{
				name: "number_s2",
				description: "如有提供，則此函數所傳回的實驗成功機率，應該介於 number_s 和 number_s2 之間"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "傳回在累加二項分配函數大於或等於臨界值之最小數值.",
		arguments: [
			{
				name: "trials",
				description: "為二項分配之測試個數"
			},
			{
				name: "probability_s",
				description: "每次測試的成功機率，機率值需在 0 和 1 之間 (不含 0 和 1)"
			},
			{
				name: "alpha",
				description: "為臨界值，機率值需在 0 和 1 之間 (不含 0 和 1)。"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "傳回在特定次數之二項分配實驗中，實驗成功的機率.",
		arguments: [
			{
				name: "number_s",
				description: "為實驗成功的次數"
			},
			{
				name: "trials",
				description: "為獨立實驗的次數"
			},
			{
				name: "probability_s",
				description: "為每一次實驗的成功機率"
			},
			{
				name: "cumulative",
				description: "為一邏輯值: TRUE 則採用累加分配函數; FALSE 則採用機率質量函數。"
			}
		]
	},
	{
		name: "BITAND",
		description: "傳回兩個數字的位元 'And'.",
		arguments: [
			{
				name: "number1",
				description: "為要估算之二進位數字的十進位表示法"
			},
			{
				name: "number2",
				description: "為要估算之二進位數字的十進位表示法"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "傳回左移 shift_amount 個位元的數字.",
		arguments: [
			{
				name: "number",
				description: "為要估算的二進位數字的十進位表示法"
			},
			{
				name: "shift_amount",
				description: "為要將數字左移的位元數目"
			}
		]
	},
	{
		name: "BITOR",
		description: "傳回兩個數字的位元 'Or'.",
		arguments: [
			{
				name: "number1",
				description: "為要估算之二進位數字的十進位表示法"
			},
			{
				name: "number2",
				description: "為要估算之二進位數字的十進位表示法"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "傳回右移 shift_amount 個位元的數字.",
		arguments: [
			{
				name: "number",
				description: "為要估算的二進位數字的十進位表示法"
			},
			{
				name: "shift_amount",
				description: "為要將數字右移的位元數目"
			}
		]
	},
	{
		name: "BITXOR",
		description: "傳回兩個數字的位元 'Exclusive Or'.",
		arguments: [
			{
				name: "number1",
				description: "為要估算之二進位數字的十進位表示法"
			},
			{
				name: "number2",
				description: "為要估算之二進位數字的十進位表示法"
			}
		]
	},
	{
		name: "CEILING",
		description: "將指定數值依指定乘算基數無條件捨入.",
		arguments: [
			{
				name: "number",
				description: "為所要進位的值"
			},
			{
				name: "significance",
				description: "為要捨入的數字的倍數"
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "將數字捨入到最接近的整數，或到最接近的指定乘算基數之倍數.",
		arguments: [
			{
				name: "number",
				description: "為要捨入的值"
			},
			{
				name: "significance",
				description: "為要捨入至的倍數"
			},
			{
				name: "mode",
				description: "如果不為零，則此函數會往背離於零的方式捨入"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "傳回指定數值依指定乘算基數無條件的捨入值.",
		arguments: [
			{
				name: "number",
				description: "欲處理之數值"
			},
			{
				name: "significance",
				description: "為用以進行無條件捨位的比較基數。"
			}
		]
	},
	{
		name: "CELL",
		description: "傳回參照第一個儲存格 (依照工作表的讀取順序) 的資訊，如格式、位置及內容等。.",
		arguments: [
			{
				name: "info_type",
				description: "為一文字值，用以指定想要取得的儲存格資訊種類。"
			},
			{
				name: "reference",
				description: "為您想要取得資訊的儲存格"
			}
		]
	},
	{
		name: "CHAR",
		description: "根據您電腦的字元集，傳回代碼所對應的字元。.",
		arguments: [
			{
				name: "number",
				description: "為一介於 1 到 255 之間的數字，用以指定要傳回的字元。"
			}
		]
	},
	{
		name: "CHIDIST",
		description: "傳回右尾卡方分配的機率值.",
		arguments: [
			{
				name: "x",
				description: "用以進行卡方檢定的數值。此值不得為負"
			},
			{
				name: "deg_freedom",
				description: "為自由度。其範圍可為 1 到 10^10 但不包括 10^10。"
			}
		]
	},
	{
		name: "CHIINV",
		description: "傳回卡方分配之右尾機率的反傳值.",
		arguments: [
			{
				name: "probability",
				description: "為卡方分配所使用的機率，此值須在 0 和 1 之間，包括 0 和 1"
			},
			{
				name: "deg_freedom",
				description: "為自由度。其範圍可為 1 到 10^10 但不包括 10^10。"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "傳回左尾卡方分配的機率值.",
		arguments: [
			{
				name: "x",
				description: "用以進行卡方檢定的數值。此值不得為負"
			},
			{
				name: "deg_freedom",
				description: "為自由度。其範圍可為 1 到 10^10，但不包括 10^10"
			},
			{
				name: "cumulative",
				description: "為一邏輯值，用以指定回傳的函數形式: TRUE 時傳回累加分配函數; FALSE 則傳回機率密度函數。"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "傳回右尾卡方分配的機率值.",
		arguments: [
			{
				name: "x",
				description: "用以進行卡方檢定的數值。此值不得為負"
			},
			{
				name: "deg_freedom",
				description: "為自由度。其範圍可為 1 到 10^10，但不包括 10^10。"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "傳回卡方分配之左尾機率的反傳值.",
		arguments: [
			{
				name: "probability",
				description: "為卡方分配所使用的機率，此值須在 0 和 1 之間，且包含 0 和 1"
			},
			{
				name: "deg_freedom",
				description: "為自由度。其範圍可為 1 到 10^10 但不包括 10^10。"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "傳回卡方分配之右尾機率的反傳值.",
		arguments: [
			{
				name: "probability",
				description: "為卡方分配所使用的機率，此值須在 0 和 1 之間，且包含 0 和 1"
			},
			{
				name: "deg_freedom",
				description: "為自由度。其範圍可為 1 到 10^10，但不包括 10^10。"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "傳回獨立性檢定之結果: 依給定的自由度及總計量，傳回卡方獨立性檢定的結果.",
		arguments: [
			{
				name: "actual_range",
				description: "觀察值範圍，用來檢定預估值"
			},
			{
				name: "expected_range",
				description: "為一範圍，其內容為各欄總和乘各列總和後的值，再除以全部值總和的比率。"
			}
		]
	},
	{
		name: "CHITEST",
		description: "傳回獨立性檢定之結果: 依給定的自由度及總計量，傳回卡方獨立性檢定的結果.",
		arguments: [
			{
				name: "actual_range",
				description: "觀察值範圍，用來檢定預估值"
			},
			{
				name: "expected_range",
				description: "為資料範圍，其內容為各欄總和乘各列總和後的值，再除以全部值總和的比率。"
			}
		]
	},
	{
		name: "CHOOSE",
		description: "根據所指定的索引值，傳回引數串列中相對應引數的值，或是執行動作。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index_num",
				description: "為指定所要選取的某個引數值。 Index_num 可以是數值、公式、或是儲存格參照，其值必須介於 1 到 254 之間。"
			},
			{
				name: "value1",
				description: "為 1 到 254 個數值、儲存格參照、名稱、公式、函數、或文字的引數，CHOOSE 函數會依照 Index_num 的值從中取用。"
			},
			{
				name: "value2",
				description: "為 1 到 254 個數值、儲存格參照、名稱、公式、函數、或文字的引數，CHOOSE 函數會依照 Index_num 的值從中取用。"
			}
		]
	},
	{
		name: "CLEAN",
		description: "從文字串中剔除所有無法列印的字元.",
		arguments: [
			{
				name: "text",
				description: "為您要剔除無法列印字元的文字串。"
			}
		]
	},
	{
		name: "CODE",
		description: "文字串中第一個字元的字碼，此一字碼是根據您電腦目前所使用的字元集。.",
		arguments: [
			{
				name: "text",
				description: "為欲求得其第一個字元字碼的文字。"
			}
		]
	},
	{
		name: "COLUMN",
		description: "傳回參照位址之欄號.",
		arguments: [
			{
				name: "reference",
				description: "為欲取其欄號的儲存格或連續儲存格範圍。若省略則使用包含 COLUMN 函數的儲存格。"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "傳回陣列或參照中的欄數。.",
		arguments: [
			{
				name: "array",
				description: "為陣列、陣列公式或是儲存格範圍參照位址，用以傳回所對應的欄數。"
			}
		]
	},
	{
		name: "COMBIN",
		description: "傳回從數值物件中選取特定數量的物件所有可能排列方式的個數。若需了解如何使用此方程式，請參閱 [說明].",
		arguments: [
			{
				name: "number",
				description: "為物件的數目"
			},
			{
				name: "number_chosen",
				description: "為每個組合中要選的物件數目。"
			}
		]
	},
	{
		name: "COMBINA",
		description: "傳回指定項目數量的組合方式數目 (包括重複選取物件).",
		arguments: [
			{
				name: "number",
				description: "為項目的總數"
			},
			{
				name: "number_chosen",
				description: "為每一種組合所含的項目數量"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "將實數係數與虛數係數轉換成複數.",
		arguments: [
			{
				name: "real_num",
				description: "是複數的實數係數"
			},
			{
				name: "i_num",
				description: "是複數的虛數係數"
			},
			{
				name: "suffix",
				description: "是複數虛數元件的字尾"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "將多組字串組合成單一字串.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "text1",
				description: "為1 到 255 個文字串，用以合併為單一字串。它們可以是文字串、數字、或單一儲存格的參照。"
			},
			{
				name: "text2",
				description: "為1 到 255 個文字串，用以合併為單一字串。它們可以是文字串、數字、或單一儲存格的參照。"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "使用常態分配傳回一母體平均數的信賴區間.",
		arguments: [
			{
				name: "alpha",
				description: "為用來推算信賴區間的顯著水準，此值需大於 0 且小於 1"
			},
			{
				name: "standard_dev",
				description: "為此資料的母體標準差，且假定為已知。標準差需大於 0"
			},
			{
				name: "size",
				description: "是樣本大小。"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "傳回使用常態分配一母體平均數的信賴區間.",
		arguments: [
			{
				name: "alpha",
				description: "為用來推算信賴區間的顯著水準，此值需大於 0 且小於 1"
			},
			{
				name: "standard_dev",
				description: "為此資料範圍的母體標準差，且假定為已知。標準差需大於 0"
			},
			{
				name: "size",
				description: "是樣本大小。"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "傳回使用 Student's 式 T 分配一母體平均數的信賴區間.",
		arguments: [
			{
				name: "alpha",
				description: "為用來推算信賴區間的顯著水準，此值需大於 0 且小於 1"
			},
			{
				name: "standard_dev",
				description: "為此資料範圍的母體標準差，且假定為已知。標準差需大於 0"
			},
			{
				name: "size",
				description: "是樣本大小。"
			}
		]
	},
	{
		name: "CONVERT",
		description: "將數字的測量系統轉換為另一個測量系統.",
		arguments: [
			{
				name: "number",
				description: "是 from_units 中要轉換的值"
			},
			{
				name: "from_unit",
				description: "是數字的單位"
			},
			{
				name: "to_unit",
				description: "是結果的單位"
			}
		]
	},
	{
		name: "CORREL",
		description: "傳回兩個資料集的相關係數.",
		arguments: [
			{
				name: "array1",
				description: "為數值的儲存格範圍。此值必須是數字，或是含有數字的名稱、陣列或參照位址"
			},
			{
				name: "array2",
				description: "是第二個數值的儲存格範圍。此值必須是數字，或是含有數字的名稱、陣列或參照位址。"
			}
		]
	},
	{
		name: "COS",
		description: "傳回一角度之餘弦值.",
		arguments: [
			{
				name: "number",
				description: "為欲求算其餘弦值的角度，以弧度表示。"
			}
		]
	},
	{
		name: "COSH",
		description: "傳回一數值之雙曲線餘弦值.",
		arguments: [
			{
				name: "number",
				description: "為任意實數。"
			}
		]
	},
	{
		name: "COT",
		description: "傳回角度的餘切值.",
		arguments: [
			{
				name: "number",
				description: "為要計算餘切值的角度，以弧度表示"
			}
		]
	},
	{
		name: "COTH",
		description: "傳回數字的雙曲餘切值.",
		arguments: [
			{
				name: "number",
				description: "為要計算雙曲餘切值的角度，以弧度表示"
			}
		]
	},
	{
		name: "COUNT",
		description: "計算範圍中包含數字的儲存格數目.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "為 1 到 255 的引數，可以包含或參照到不同類型的資料，但是只會計算數值資料"
			},
			{
				name: "value2",
				description: "為 1 到 255 的引數，可以包含或參照到不同類型的資料，但是只會計算數值資料"
			}
		]
	},
	{
		name: "COUNTA",
		description: "計算範圍中非空白儲存格的數目.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "為 1 到 255 個引數，代表欲計算的值和儲存格。值可為任何類型的資訊。"
			},
			{
				name: "value2",
				description: "為 1 到 255 個引數，代表欲計算的值和儲存格。值可為任何類型的資訊。"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "計算指定範圍內空白儲存格的數目.",
		arguments: [
			{
				name: "range",
				description: "為欲計算空白儲存格數目的範圍。"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "計算一範圍內符合指定條件儲存格的數目.",
		arguments: [
			{
				name: "range",
				description: "為欲計算符合給定條件儲存格數目的範圍"
			},
			{
				name: "criteria",
				description: "為比較的條件，條件可以是可以是數字，表示式或文字，用以指定哪些儲存格會被計算。"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "計算由特定條件或準則集所指定的儲存格數目.",
		arguments: [
			{
				name: "criteria_range",
				description: "是您要以特定條件評估的儲存格範圍"
			},
			{
				name: "criteria",
				description: "是以數字、運算式或文字為形式的條件，這會定義要計算哪些儲存格"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "傳回從票息週期的開始到結帳日期之間的日數.",
		arguments: [
			{
				name: "settlement",
				description: "是證券的結帳日期，以數列日期數字表示"
			},
			{
				name: "maturity",
				description: "是證券的到期日期，以數列日期數字表示"
			},
			{
				name: "frequency",
				description: "是每年票息付款的數字"
			},
			{
				name: "basis",
				description: "是要使用的日計數基礎類型"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "傳回結帳日期之後下一個票息日期.",
		arguments: [
			{
				name: "settlement",
				description: "是證券的結帳日期，以數列日期數字表示"
			},
			{
				name: "maturity",
				description: "是證券的到期日期，以數列日期數字表示"
			},
			{
				name: "frequency",
				description: "是每年票息付款的數字"
			},
			{
				name: "basis",
				description: "是要使用的日計數基礎類型"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "傳回可在結帳日期和到期日期之間支付的票息數字.",
		arguments: [
			{
				name: "settlement",
				description: "是證券的結帳日期，以數列日期數字表示"
			},
			{
				name: "maturity",
				description: "是證券的到期日期，以數列日期數字表示"
			},
			{
				name: "frequency",
				description: "是每年票息付款的數字"
			},
			{
				name: "basis",
				description: "是要使用的日計數基礎類型"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "傳回結帳日期之前的前一個票息日期.",
		arguments: [
			{
				name: "settlement",
				description: "是證券的結帳日期，以數列日期數字表示"
			},
			{
				name: "maturity",
				description: "是證券的到期日期，以數列日期數字表示"
			},
			{
				name: "frequency",
				description: "是每年票息付款的數目"
			},
			{
				name: "basis",
				description: "是要使用的日計數基礎類型"
			}
		]
	},
	{
		name: "COVAR",
		description: "傳回共變數。所謂共變數為兩組資料的各個對應的資料點相減後再全部相乘，然後求平均值.",
		arguments: [
			{
				name: "array1",
				description: "第一組整數資料儲存格範圍，必須是數值、陣列、或含數值的參照"
			},
			{
				name: "array2",
				description: "第二組整數資料儲存格範圍，必須是數值、陣列、或含數值的參照。"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "傳回母體共變數。所謂共變數為兩組資料的各個對應的資料點相減後再全部相乘，然後求平均值.",
		arguments: [
			{
				name: "array1",
				description: "為第一組整數資料儲存格範圍。此範圍必須是數值、陣列或內含數值的參照"
			},
			{
				name: "array2",
				description: "為第二組整數資料儲存格範圍。此範圍必須是數值、陣列或內含數值的參照。"
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "傳回樣本共變數。所謂共變數為兩組資料的各個對應的資料點相減後再全部相乘，然後求平均值.",
		arguments: [
			{
				name: "array1",
				description: "為第一組整數資料儲存格範圍。此範圍必須是數值、陣列或內含數值的參照"
			},
			{
				name: "array2",
				description: "為第二組整數資料儲存格範圍。此範圍必須是數值、陣列或內含數值的參照。"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "傳回在累加二項分配函數大於或等於臨界值之最小數值.",
		arguments: [
			{
				name: "trials",
				description: "為二項分配之測試個數"
			},
			{
				name: "probability_s",
				description: "為每次測試的成功機率，機率值需在 0 和 1 之間 (含 0 和 1)"
			},
			{
				name: "alpha",
				description: "為臨界值，機率值需在 0 和 1 之間 (含 0 和 1)。"
			}
		]
	},
	{
		name: "CSC",
		description: "傳回角度的餘割值.",
		arguments: [
			{
				name: "number",
				description: "為要計算餘割值的角度，以弧度表示"
			}
		]
	},
	{
		name: "CSCH",
		description: "傳回角度的雙曲餘割值.",
		arguments: [
			{
				name: "number",
				description: "為要計算雙曲餘割值的角度，以弧度表示"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "傳回在兩個週期之間所支付的累計利息.",
		arguments: [
			{
				name: "rate",
				description: "是利率"
			},
			{
				name: "nper",
				description: "是付款週期的總數"
			},
			{
				name: "pv",
				description: "是現值"
			},
			{
				name: "start_period",
				description: "是計算中的第一個週期"
			},
			{
				name: "end_period",
				description: "是計算中的最後一個週期"
			},
			{
				name: "type",
				description: "是付款的時機"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "傳回在兩個週期之間的貸款上所支付累計資金.",
		arguments: [
			{
				name: "rate",
				description: "是利率"
			},
			{
				name: "nper",
				description: "是付款週期的總數"
			},
			{
				name: "pv",
				description: "是現值"
			},
			{
				name: "start_period",
				description: "是計算的第一個週期"
			},
			{
				name: "end_period",
				description: "是計算的最後一個週期"
			},
			{
				name: "type",
				description: "是付款的時機"
			}
		]
	},
	{
		name: "DATE",
		description: "在 Spreadsheet 的日期和時間碼中代表日期的時間序列值。.",
		arguments: [
			{
				name: "year",
				description: "係指一個數字，在 Spreadsheet for Windows 中是從 1900 到 9999，在 Spreadsheet for the Macintosh 中是從 1904 到 9999"
			},
			{
				name: "month",
				description: "係指一個從 1 到 12 的數字，代表一年中的月份 "
			},
			{
				name: "day",
				description: "為一個從 1 到 31 的數字，代表這個月份中的日數。"
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
		description: "將文字型態的日期資料轉換為 Spreadsheet 的數字日期型態.",
		arguments: [
			{
				name: "date_text",
				description: "為 Spreadsheet 日期格式的文字串，介於 1900/1/1  到 9999/12/31 之間。"
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "求出清單或資料庫中符合指定條件之欄內的數值平均數.",
		arguments: [
			{
				name: "database",
				description: "為構成清單或資料庫的儲存格範圍。資料庫為相關資料的清單"
			},
			{
				name: "field",
				description: "為雙引號之間的欄標籤，或是代表欄在清單中之位置的數值"
			},
			{
				name: "criteria",
				description: "為包含指定條件的儲存格範圍。範圍涵蓋指定條件下的欄標籤以及標籤下方的儲存格。"
			}
		]
	},
	{
		name: "DAY",
		description: "傳回該月的第幾天，從 1 到 31 的數字。.",
		arguments: [
			{
				name: "serial_number",
				description: "係指 Spreadsheet 所使用的日期和時間碼的數字。"
			}
		]
	},
	{
		name: "DAYS",
		description: "傳回兩個日期之間的間隔天數。.",
		arguments: [
			{
				name: "end_date",
				description: "start_date 和 end_date 是您想要知道間隔天數的兩個日期"
			},
			{
				name: "start_date",
				description: "start_date 和 end_date 是您想要知道間隔日數的兩個日期"
			}
		]
	},
	{
		name: "DAYS360",
		description: "傳回根據一年 360 天的算法 (每個月30天，每年 12 個月) 所求出之兩個日期的相距天數.",
		arguments: [
			{
				name: "start_date",
				description: "start_date 和 end_date 為欲求其相距天數的兩個日期"
			},
			{
				name: "end_date",
				description: "start_date 和 end_date 為欲求其相距天數的兩個日期"
			},
			{
				name: "method",
				description: "為一邏輯值，用以指定日期計算方法: U.S. (NASD) = FALSE 或省略; European = TRUE。"
			}
		]
	},
	{
		name: "DB",
		description: "傳回固定資產在指定期間按定率遞減法計算的折舊.",
		arguments: [
			{
				name: "cost",
				description: "為固定資產的原始成本"
			},
			{
				name: "salvage",
				description: "為固定資產耐用年限終了時之估計殘值"
			},
			{
				name: "life",
				description: "為固定資產之使用期限 (亦稱為資產的耐用年限)"
			},
			{
				name: "period",
				description: "為要計算折舊的期間，必須與 Life 使用相同的衡量單位"
			},
			{
				name: "month",
				description: "指第一年的月份數。若省略此引數則以 12 個月計算。"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "計算資料庫中符合指定條件，且包含數值資料的儲存格數目.",
		arguments: [
			{
				name: "database",
				description: "為資料庫或表格所在的儲存格範圍。資料庫為一組相關資料的集合"
			},
			{
				name: "field",
				description: "為欄位的名稱 (前後須加上雙引號)，或是欄位所在之號碼"
			},
			{
				name: "criteria",
				description: "為包含篩選條件的儲存格範圍。上面一列是欄位的名稱，下面一列是在此欄位篩選所用的條件。"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "計算資料庫中裡符合指定條件，且非空白的儲存格數目.",
		arguments: [
			{
				name: "database",
				description: "為資料庫或表格所在的儲存格範圍。資料庫是指一組相關資料的集合"
			},
			{
				name: "field",
				description: "為欄位的名稱 (前後須加上雙引號)，或是欄位所在之號碼"
			},
			{
				name: "criteria",
				description: "為包含篩選條件的儲存格範圍。上面一列是欄位的名稱，下面一列是在此欄位篩選所用的條件。"
			}
		]
	},
	{
		name: "DDB",
		description: "傳回固定資產在指定期間內按倍率遞減法，或其他指定方法計算所得的折舊.",
		arguments: [
			{
				name: "cost",
				description: "為固定資產的原始成本"
			},
			{
				name: "salvage",
				description: "固定資產耐用年限終了時之估計殘值"
			},
			{
				name: "life",
				description: "為固定資產之使用期限"
			},
			{
				name: "period",
				description: "為所要計算折舊的期間，單位應與使用期限同"
			},
			{
				name: "factor",
				description: "為指定餘額遞減的方法。若省略此引數則以 2 (倍率遞減法) 計算。"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "將十進位數字轉換成二進位.",
		arguments: [
			{
				name: "number",
				description: "是您要轉換的十進位整數"
			},
			{
				name: "places",
				description: "是要使用的字元數"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "將十進位數字轉換成十六進位.",
		arguments: [
			{
				name: "number",
				description: "是您要轉換的十進位整數"
			},
			{
				name: "places",
				description: "是要使用的字元數"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "將十進位數字轉換成八進位.",
		arguments: [
			{
				name: "number",
				description: "是您要轉換的十進位整數"
			},
			{
				name: "places",
				description: "是要使用的字元數"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "將含有指定底數的數字文字表示法，轉換為十進位數字.",
		arguments: [
			{
				name: "number",
				description: "為要轉換的數字"
			},
			{
				name: "radix",
				description: "為要轉換數字之基底"
			}
		]
	},
	{
		name: "DEGREES",
		description: "將弧度轉換成角度.",
		arguments: [
			{
				name: "angle",
				description: "為想要轉換的弧度。"
			}
		]
	},
	{
		name: "DELTA",
		description: "測試兩個數字是否相等.",
		arguments: [
			{
				name: "number1",
				description: "是第一個數字"
			},
			{
				name: "number2",
				description: "是第二個數字"
			}
		]
	},
	{
		name: "DEVSQ",
		description: "傳回樣本平均數和樣本間差異的平方和.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為欲讓 DEVSQ 計算差異平方和的 1 至 255 個引數，或陣列或陣列參照。"
			},
			{
				name: "number2",
				description: "為欲讓 DEVSQ 計算差異平方和的 1 至 255 個引數，或陣列或陣列參照。"
			}
		]
	},
	{
		name: "DGET",
		description: "在資料庫中篩選符合指定條件的一筆記錄.",
		arguments: [
			{
				name: "database",
				description: "為資料庫或表格所在的儲存格範圍。資料庫是指一群相關資料的集合"
			},
			{
				name: "field",
				description: "為欄位的名稱 (前後須加上雙引號)，或是欄位所在之號碼"
			},
			{
				name: "criteria",
				description: "為包含篩選條件的儲存格範圍。上面一列是欄位的名稱，下面一列是在此欄位篩選所用的條件。"
			}
		]
	},
	{
		name: "DISC",
		description: "傳回證券的貼現率.",
		arguments: [
			{
				name: "settlement",
				description: "是證券的結帳日期，以數列日期數字表示"
			},
			{
				name: "maturity",
				description: "是證券的到期日期，以數列日期數字表示"
			},
			{
				name: "pr",
				description: "是每 $100 面額的證券價格"
			},
			{
				name: "redemption",
				description: "是每 $100 面額證券的贖回價值"
			},
			{
				name: "basis",
				description: "是要使用的日計數基礎類型"
			}
		]
	},
	{
		name: "DMAX",
		description: "資料庫中符合指定條件之記錄的欄位 (欄) 內數值最小值.",
		arguments: [
			{
				name: "database",
				description: "為構成清單或資料庫的儲存格範圍。資料庫為相關資料的清單"
			},
			{
				name: "field",
				description: "為雙引號之間的欄標籤，或是代表欄在清單中之位置的數值"
			},
			{
				name: "criteria",
				description: "為包含指定條件的儲存格範圍。範圍涵蓋指定條件下的欄標籤以及標籤下方的儲存格。"
			}
		]
	},
	{
		name: "DMIN",
		description: "資料庫中符合指定條件之記錄的欄位 (欄) 內數值的最小值.",
		arguments: [
			{
				name: "database",
				description: "為構成清單或資料庫的儲存格範圍。資料庫為相關資料的清單"
			},
			{
				name: "field",
				description: "為雙引號之間的欄標籤，或是代表欄在清單中之位置的數值"
			},
			{
				name: "criteria",
				description: "為包含指定條件的儲存格範圍。範圍涵蓋指定條件下的欄標籤以及標籤下方的儲存格。"
			}
		]
	},
	{
		name: "DOLLAR",
		description: "依照貨幣格式，將數值轉成文字.",
		arguments: [
			{
				name: "number",
				description: "為一個數字、包含數值儲存格的參照、或是計算結果為數值的公式"
			},
			{
				name: "decimals",
				description: "為小數點的位數。數值將四捨五入至指定的小數位數，若不設定小數位數，預設值是兩位。"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "將以分數表示的美金價格，轉換成以十進位數字表示的美金價格.",
		arguments: [
			{
				name: "fractional_dollar",
				description: "是以分數表示的數字"
			},
			{
				name: "fraction",
				description: "是要在分數的分母中使用的整數"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "將以十進位數字表示的美金價格，轉換成以分數表示的美金價格.",
		arguments: [
			{
				name: "decimal_dollar",
				description: "是十進位數字"
			},
			{
				name: "fraction",
				description: "是要在分數的分母中使用的整數"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "就資料庫裡所有符合指定條件的記錄，計算指定欄位中數值資料之乘積。.",
		arguments: [
			{
				name: "database",
				description: "為資料庫或清單所在的儲存格範圍。資料庫是指一群相關資料的集合。"
			},
			{
				name: "field",
				description: "為欄位的名稱 (前後須加上雙引號)，或是欄位所在之號碼。"
			},
			{
				name: "criteria",
				description: "為包含篩選條件的儲存格範圍。上面一列是欄位的名稱，下面一列是在此欄位篩選所用的條件。"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "計算資料庫中指定欄位中數值資料之標準差.",
		arguments: [
			{
				name: "database",
				description: "為資料庫或清單所在的儲存格範圍。資料庫為一群相關資料的集合"
			},
			{
				name: "field",
				description: "為欄位的名稱 (前後須加上雙引號)，或是欄位所在之號碼"
			},
			{
				name: "criteria",
				description: "為包含篩選條件的儲存格範圍。上面一列是欄位的名稱，下面一列是在此欄位篩選所用的條件。"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "計算選取資料庫記錄中所選資料的母體標準差.",
		arguments: [
			{
				name: "database",
				description: "資料庫或清單所在的儲存格範圍。資料庫是指一群相關資料的集合"
			},
			{
				name: "field",
				description: "為欄位的名稱 (前後須加上雙引號)，或是欄位所在之號碼"
			},
			{
				name: "criteria",
				description: "為包含篩選條件的儲存格範圍。上面一列是欄位的名稱，下面一列是在此欄位篩選所用的條件。"
			}
		]
	},
	{
		name: "DSUM",
		description: "就資料庫裡所有符合指定條件的記錄中，計算指定欄位中數值資料之總和.",
		arguments: [
			{
				name: "database",
				description: "為資料庫或清單所在的儲存格範圍。資料庫為一群相關資料的集合"
			},
			{
				name: "field",
				description: "為欄位的名稱 (前後須加上雙引號)，或是欄位所在之號碼"
			},
			{
				name: "criteria",
				description: "為包含篩選條件的儲存格範圍。上面一列是欄位的名稱，下面一列是在此欄位篩選所用的條件。"
			}
		]
	},
	{
		name: "DVAR",
		description: "傳回選取資料庫範圍內記錄的樣本變異數.",
		arguments: [
			{
				name: "database",
				description: "為資料庫或清單所在的儲存格範圍。資料庫為一群相關資料的集合"
			},
			{
				name: "field",
				description: "為欄位的名稱 (前後須加上雙引號)，或是欄位所在之號碼"
			},
			{
				name: "criteria",
				description: "為包含篩選條件的儲存格範圍。上面一列是欄位的名稱，下面一列是在此欄位篩選所用的條件。"
			}
		]
	},
	{
		name: "DVARP",
		description: "計算資料庫中選取記錄的母體變異數.",
		arguments: [
			{
				name: "database",
				description: "資料庫或清單所在的儲存格範圍。資料庫是指一群相關資料的集合"
			},
			{
				name: "field",
				description: "為欄位的名稱 (前後須加上雙引號)，或是欄位所在之號碼"
			},
			{
				name: "criteria",
				description: "為包含篩選條件的儲存格範圍。上面一列是欄位的名稱，下面一列是在此欄位篩選所用的條件。"
			}
		]
	},
	{
		name: "EDATE",
		description: "傳回日期的數列數字，這是在開始日期之前或之後所指出的月份數.",
		arguments: [
			{
				name: "start_date",
				description: "是代表開始日期的數列日期數字"
			},
			{
				name: "months",
				description: "是在 start_date 之前或之後的月份數"
			}
		]
	},
	{
		name: "EFFECT",
		description: "傳回實年度利率.",
		arguments: [
			{
				name: "nominal_rate",
				description: "是名義利率"
			},
			{
				name: "npery",
				description: "是每年複利週期的數字"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "傳回 URL 編碼字串.",
		arguments: [
			{
				name: "text",
				description: "為要以 URL 編碼的字串"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "傳回所指定月份數之前或之後的月份最後一天的數列數字.",
		arguments: [
			{
				name: "start_date",
				description: "是代表開始日期的數列日期數字"
			},
			{
				name: "months",
				description: "是在 start_date 之前或之後的月份數"
			}
		]
	},
	{
		name: "ERF",
		description: "傳回錯誤的函數.",
		arguments: [
			{
				name: "lower_limit",
				description: "是整合 ERF 的下界"
			},
			{
				name: "upper_limit",
				description: "是整合 ERF 的上界"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "傳回錯誤函數.",
		arguments: [
			{
				name: "X",
				description: "是整合 ERF.PRECISE 的下界"
			}
		]
	},
	{
		name: "ERFC",
		description: "傳回互補錯誤函數.",
		arguments: [
			{
				name: "x",
				description: "是整合 ERF 的下界"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "傳回互補錯誤函數.",
		arguments: [
			{
				name: "X",
				description: "是整合 ERFC.PRECISE 的下界"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "傳回代表錯誤值的編號.",
		arguments: [
			{
				name: "error_val",
				description: "為錯誤值或含有錯誤值的儲存格參照，藉由此錯誤值找出其代表的錯誤代碼。"
			}
		]
	},
	{
		name: "EVEN",
		description: "無條件捨入 number 的最近偶整數。負數將會以遠離 0 的方式捨入.",
		arguments: [
			{
				name: "number",
				description: "為所要捨入的數值。"
			}
		]
	},
	{
		name: "EXACT",
		description: "檢查兩個文字串是否完全相同並傳回 TRUE 或 FALSE。EXACT 區分大小寫。.",
		arguments: [
			{
				name: "text1",
				description: "是所要檢查的第一個文字串。"
			},
			{
				name: "text2",
				description: "是所要檢查的第二個文字串。"
			}
		]
	},
	{
		name: "EXP",
		description: "傳回指數的 number 乘方值.",
		arguments: [
			{
				name: "number",
				description: "為套用於基數 e 的指數。常數 e 等於 2.71828182845904，也就是自然對數的基數。"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "傳回指數分配函數.",
		arguments: [
			{
				name: "x",
				description: "為函數的值，不可為負數"
			},
			{
				name: "lambda",
				description: "為正數參數值"
			},
			{
				name: "cumulative",
				description: "為一邏輯值，用以指定回傳的函數形式: TRUE 時傳回累加分配函數; FALSE 則傳回機率密度函數。"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "傳回指數分配函數.",
		arguments: [
			{
				name: "x",
				description: "為函數的值，不可為負數"
			},
			{
				name: "lambda",
				description: "為正數參數值"
			},
			{
				name: "cumulative",
				description: "為一邏輯值，用以指定回傳的函數形式: TRUE 時傳回累加分配函數; FALSE 則傳回機率密度函數。"
			}
		]
	},
	{
		name: "F.DIST",
		description: "傳回兩組資料的 (左尾) F 機率分配 (散佈程度).",
		arguments: [
			{
				name: "x",
				description: "用來評估函數的值，需為非負數值"
			},
			{
				name: "deg_freedom1",
				description: "為分子的自由度，需在 1 到 10^10 之間，但不含 10^10"
			},
			{
				name: "deg_freedom2",
				description: "為分母的自由度，需在 1 到 10^10 之間，但不含 10^10。"
			},
			{
				name: "cumulative",
				description: "為一邏輯值，用以指定回傳的函數形式: TRUE 時傳回累加分配函數; FALSE 則傳回機率密度函數。"
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "傳回兩組資料的 (右尾) F 機率分配 (散佈程度).",
		arguments: [
			{
				name: "x",
				description: "用來評估函數的值，需為非負數值"
			},
			{
				name: "deg_freedom1",
				description: "為分子的自由度，需在 1 到 10^10 之間，但不含 10^10"
			},
			{
				name: "deg_freedom2",
				description: "為分母的自由度，需在 1 到 10^10 之間，但不含 10^10。"
			}
		]
	},
	{
		name: "F.INV",
		description: "傳回 (左尾) F 機率分配之反函數值: 若 p = F.DIST(x,...)，則 F.INV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "為一個 F 累加分配的機率值，須在 0 和 1 之間，且含 0 和 1"
			},
			{
				name: "deg_freedom1",
				description: "為分子的自由度，需在 1 到 10^10 之間，但不含 10^10"
			},
			{
				name: "deg_freedom2",
				description: "為分母的自由度，需在 1 到 10^10 之間，但不含 10^10。"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "傳回 (右尾) F 機率分配之反函數值: 若 p = F.DIST.RT(x,...)，則 F.INV.RT(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "為一個 F 累加分配的機率值，須在 0 和 1 之間，且含 0 和 1"
			},
			{
				name: "deg_freedom1",
				description: "為分子的自由度，需在 1 到 10^10 之間，但不含 10^10"
			},
			{
				name: "deg_freedom2",
				description: "為分母的自由度，需在 1 到 10^10 之間，但不含 10^10。"
			}
		]
	},
	{
		name: "F.TEST",
		description: "傳回一個 F 檢定的結果 (雙尾機率值)。用來檢定 Array1 與 Array2 中的變異數是否有顯著不同.",
		arguments: [
			{
				name: "array1",
				description: "為第一個資料陣列或是資料範圍。此範圍可以是數值或內含數值的名稱、陣列、儲存格參照 (若範圍內有空白，該空白將忽略不計)"
			},
			{
				name: "array2",
				description: "為第二個資料陣列或是資料範圍。此範圍可以是數值或內含數值的名稱、陣列、儲存格參照 (若範圍內有空白，該空白將忽略不計)。"
			}
		]
	},
	{
		name: "FACT",
		description: "傳回一數字 的階乘。傳回 1*2*3*....* Number。.",
		arguments: [
			{
				name: "number",
				description: "輸入的乘階數應為非負整數。"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "傳回數字的雙階乘.",
		arguments: [
			{
				name: "number",
				description: "是要傳回雙階乘的值"
			}
		]
	},
	{
		name: "FALSE",
		description: "傳回邏輯值 FALSE。.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "傳回兩組資料的 (右尾) F 機率分配 (散佈程度).",
		arguments: [
			{
				name: "x",
				description: "用來評估函數的值，需為非負數值"
			},
			{
				name: "deg_freedom1",
				description: "為分子的自由度，需在 1 到 10^10 之間但不含 10^10"
			},
			{
				name: "deg_freedom2",
				description: "為分母的自由度，需在 1 到 10^10 之間但不含 10^10。"
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
		description: "某文字串在另一個文字串中的起始位置。FIND 區分大小寫。.",
		arguments: [
			{
				name: "find_text",
				description: "為要搜尋的文字串。可使用雙引號 (空字串) 來找尋符合 Within_text 的第一個字元位置; 不可使用萬用字元。"
			},
			{
				name: "within_text",
				description: "是一個含有欲尋找文字串的搜尋目標文字串。"
			},
			{
				name: "start_num",
				description: "指定從搜尋目標的第幾個字元開始尋找。Within_text 中的第一個字元即位置 1。若省略則 Start_num = 1。"
			}
		]
	},
	{
		name: "FINV",
		description: "傳回 (右尾) F 機率分配之反函數值: 如果 p = FDIST(x,...)，則反函數為 FINV(p,....) = x.",
		arguments: [
			{
				name: "probability",
				description: "為一個 F 累加分配的機率值，須在 0 和 1 之間且含 0 和 1"
			},
			{
				name: "deg_freedom1",
				description: "為分子的自由度，需在 1 到 10^10 之間但不含 10^10"
			},
			{
				name: "deg_freedom2",
				description: "為分母的自由度，需在 1 到 10^10 之間但不含 10^10。"
			}
		]
	},
	{
		name: "FISHER",
		description: "傳回費雪轉換.",
		arguments: [
			{
				name: "x",
				description: "為所要轉換的數值，此值需在 1 到 -1 之間但不含 1 及 -1。"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "傳回費雪轉換的反函數值: 如果 y = FISHER(x) 則反函數為 FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "為您要進行反轉換的數值。"
			}
		]
	},
	{
		name: "FIXED",
		description: "指定小數點的位數，並將數字轉換成文字.",
		arguments: [
			{
				name: "number",
				description: "為要四捨五入的小數位數"
			},
			{
				name: "decimals",
				description: "為小數位數。如果省略，預設值是 2"
			},
			{
				name: "no_commas",
				description: "為是否顯示千分位的邏輯值。若不要顯示千分位，則填入 TRUE; 若要顯示千分位，則填入 FALSE 或不填。"
			}
		]
	},
	{
		name: "FLOOR",
		description: "將一數字依指定乘算基數無條件捨位.",
		arguments: [
			{
				name: "number",
				description: "為所要捨位的數值"
			},
			{
				name: "significance",
				description: "為要捨位的數字的倍數。數值與顯著水準必需同時為正或同時為負。"
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "無條件捨去到最接近的整數，或到最接近的指定乘算基數之倍數.",
		arguments: [
			{
				name: "number",
				description: "為要捨入的數值"
			},
			{
				name: "significance",
				description: "為要捨入的倍數"
			},
			{
				name: "mode",
				description: "如果不是零，則此函數會以趨近於零的方式捨入"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "傳回指定數值依指定乘算基數無條件的捨位值.",
		arguments: [
			{
				name: "number",
				description: "欲處理之數值"
			},
			{
				name: "significance",
				description: "為用以進行無條件捨位的比較基數。"
			}
		]
	},
	{
		name: "FORECAST",
		description: "根據現有資料所產生出的趨勢線來預測未來值.",
		arguments: [
			{
				name: "x",
				description: "為所要預測值的資料點。此值需為數值資料"
			},
			{
				name: "known_y's",
				description: "因相依變數所在的陣列或儲存格範圍"
			},
			{
				name: "known_x's",
				description: "自變數所在的陣列或儲存格範圍。 Known_x's 的變異數不得為 0。"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "傳回以字串呈現的公式.",
		arguments: [
			{
				name: "reference",
				description: "為公式的參照"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "計算範圍內數值出現的區間次數 (即次數分配表)，再將此次數分配表以一垂直的陣列傳出.",
		arguments: [
			{
				name: "data_array",
				description: "為一個數值的陣列，或是一個數值的儲存格範圍之參照位址，用以計算次數分配 (空白及文字資料將略過不計)"
			},
			{
				name: "bins_array",
				description: "為一個區間的陣列或是儲存格參照，用以將 data_arry 區隔為若干群組的區間。"
			}
		]
	},
	{
		name: "FTEST",
		description: "傳回一個 F 檢定的結果 (雙尾機率值)。用來檢定 Array1 與 Array2 中的變異數是否有顯著不同.",
		arguments: [
			{
				name: "array1",
				description: "為第一個資料陣列或是資料範圍，可以是數值或內含數值的名稱、陣列或參照 (若空白則忽略不計)"
			},
			{
				name: "array2",
				description: "為第二個資料陣列或是資料範圍，可以是數值或內含數值的名稱、陣列或參照 (若空白將忽略不計)。"
			}
		]
	},
	{
		name: "FV",
		description: "傳回根據週期、固定支出，以及固定利率的投資未來值.",
		arguments: [
			{
				name: "rate",
				description: "為每一個週期的利率。例如，使用 6%/4 表示 6% 之下的每季付款利率"
			},
			{
				name: "nper",
				description: "為年金的付款總期數"
			},
			{
				name: "pmt",
				description: "係指分期付款; 不得在年金期限內變更"
			},
			{
				name: "pv",
				description: "係指現值或一系列未來付款的目前總額。若省略則 Pv = 0"
			},
			{
				name: "type",
				description: "為代表付款何時到期的數值: 1 表示期初給付; 0 或省略時表示期末給付。"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "在套用一系列複利率之後，傳回初始資金的未來值.",
		arguments: [
			{
				name: "principal",
				description: "是現值"
			},
			{
				name: "schedule",
				description: "是要套用的利率陣列"
			}
		]
	},
	{
		name: "GAMMA",
		description: "傳回伽瑪函數值.",
		arguments: [
			{
				name: "x",
				description: "為要計算伽瑪的值"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "傳回伽瑪分配函數.",
		arguments: [
			{
				name: "x",
				description: "是要用來評估分配的數值，此值不可為負"
			},
			{
				name: "alpha",
				description: "為此種分配的一個正數參數"
			},
			{
				name: "beta",
				description: "為此種分配的一個正數參數。若 beta = 1，則 GAMMADIST 將傳回標準伽瑪分配"
			},
			{
				name: "cumulative",
				description: "為一邏輯值: 當為 TRUE 時，函數傳回累加分配函數; FALSE 或是省略則函數傳回機率質量函數。"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "傳回伽瑪累加分配的反函數值: 若 p = GAMMA.DIST(x,...)，則 GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "為伽瑪分配機率值。此值需在 0 到 1 之間 (包含 0 和 1 )。"
			},
			{
				name: "alpha",
				description: "為此種分配的一個正數參數"
			},
			{
				name: "beta",
				description: "為此種分配的一個正數參數。當 beta=1 時，GAMMA.INV 傳回標準伽瑪的反分配。"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "傳回伽瑪分配函數.",
		arguments: [
			{
				name: "x",
				description: "為用以進行卡方檢定的數值，此值不得為負"
			},
			{
				name: "alpha",
				description: "為此種分配的一個正數參數"
			},
			{
				name: "beta",
				description: "為伽瑪分配的一個正數參數，如果 beta=1 則 GAMMADIST 將傳回標準伽瑪分配"
			},
			{
				name: "cumulative",
				description: "為一邏輯值: 當為 TRUE 時，函數傳回累加分配函數; FALSE 或是省略則函數傳回機率質量函數。"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "傳回伽瑪累加分配的反函數值。如果 p = GAMMADIST(x,...) 則反函數為 GAMMAINV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "為伽瑪分配機率值。此值需在 0 到 1 之間 (包含 0 和 1)"
			},
			{
				name: "alpha",
				description: "為此種分配的一個正數參數"
			},
			{
				name: "beta",
				description: "伽瑪分配的一個參數，需為正數。當 beta=1 時，GAMMADIST 傳回標準伽瑪的反分配。"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "傳回伽瑪函數的自然對數.",
		arguments: [
			{
				name: "x",
				description: "為所要計算 GAMMALN 函數的正數數值。"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "傳回伽瑪函數的自然對數.",
		arguments: [
			{
				name: "x",
				description: "為所要計算 GAMMALN.PRECISE 函數的正數數值。"
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
		description: "傳回最大公因數.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是 1 到 255 的值"
			},
			{
				name: "number2",
				description: "是 1 到 255 的值"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "傳回正數數值資料陣列或資料範圍的幾何平均數.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是您想計算其平均數的 1 到 255 個引數，此為數字或含有數字的名稱、陣列或參照位址。"
			},
			{
				name: "number2",
				description: "是您想計算其平均數的 1 到 255 個引數，此為數字或含有數字的名稱、陣列或參照位址。"
			}
		]
	},
	{
		name: "GESTEP",
		description: "測試數字是否大於閾值.",
		arguments: [
			{
				name: "number",
				description: "是依照步驟測試的值"
			},
			{
				name: "step",
				description: "是閾值"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "從樞紐分析表中抽選資料.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "data_field",
				description: "為欲從中抽選資料的資料欄位名稱"
			},
			{
				name: "pivot_table",
				description: "是包含您要擷取之資料的樞紐分析表中，其儲存格或儲存格範圍的參照"
			},
			{
				name: "field",
				description: "為欲參照之欄位"
			},
			{
				name: "item",
				description: "為欲參照之欄位項目"
			}
		]
	},
	{
		name: "GROWTH",
		description: "傳回數字為符合已知資料點的指數遞增方式.",
		arguments: [
			{
				name: "known_y's",
				description: "是 y = b*m^x 關係中的一組已知 y 值，此為正數之陣列或範圍"
			},
			{
				name: "known_x's",
				description: "是 y = b*m^x 關係中已知的一組選擇性的 x 值，此為大小與 Known_y's 相同之陣列或範圍"
			},
			{
				name: "new_x's",
				description: "是一組要 GROWTH 函數求出對應 y 值的新 x 值"
			},
			{
				name: "const",
				description: "為邏輯值: 若 Const = TRUE，則常數項 b 將依計算而得; 若 Const = FALSE 或省略，常數項 b 將被設定為 1。"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "傳回一組正數資料的調和平均數: 亦即算術平均數的倒數求平均後，再加以倒數.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是您想計算其調和平均數的 1 到 255 個引數，可為數字或含有數字的名稱、陣列或參照位址。"
			},
			{
				name: "number2",
				description: "是您想計算其調和平均數的 1 到 255 個引數，可為數字或含有數字的名稱、陣列或參照位址。"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "將十六進位數字轉換成二進位.",
		arguments: [
			{
				name: "number",
				description: "是您要轉換的十六進位數字"
			},
			{
				name: "places",
				description: "是要使用的字元數"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "將十六進位數字轉換成十進位l.",
		arguments: [
			{
				name: "number",
				description: "是您要轉換的十六進位數字"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "將十六進位數字轉換成八進位.",
		arguments: [
			{
				name: "number",
				description: "是您要轉換的十六進位數字"
			},
			{
				name: "places",
				description: "是要使用的字元數"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "在陣列或表格的第一列尋找指定值，然後傳回指定值所在那一列 (記錄) 中您所要的欄位。.",
		arguments: [
			{
				name: "lookup_value",
				description: "為要在表格頂端列中搜尋的值，此引數可為數值、參照位址或文字串。"
			},
			{
				name: "table_array",
				description: "為欲在其中尋找資料的表格，其資料可為文字、數值、或邏輯值。Table_array 可以是指到一個範圍的參照位址或是範圍名稱。"
			},
			{
				name: "row_index_num",
				description: "傳回數值位於 table_array 中的第幾列的列號數。表格中的第一列的列數為 1 。"
			},
			{
				name: "range_lookup",
				description: "為一邏輯值，當為 TRUE 時，在尋找資料時，將尋找最接近的值; 為 FALSE 時，會尋找完全相同的值。"
			}
		]
	},
	{
		name: "HOUR",
		description: "傳回時間數值所代表的小時數，從 0 (12:00 A.M.) 到 23 (11:00 P.M.)。.",
		arguments: [
			{
				name: "serial_number",
				description: "為 Spreadsheet 所使用的日期時間碼數字，或是時間格式的文字，例如 16:48:00 或 4:48:00 PM。"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "建立一個捷徑，可讓您直接開啟硬碟、網路伺服器、或網際網路上的文件.",
		arguments: [
			{
				name: "link_location",
				description: "為檔案的完整路徑名稱。此路徑可為磁碟機路徑、UNC 位置、URL 位置"
			},
			{
				name: "friendly_name",
				description: "填入您想在儲存格顯示的數字、文字或函數。如省略不填，儲存格將顯示 Link_location 文字。 "
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "傳回超幾何分配.",
		arguments: [
			{
				name: "sample_s",
				description: "為樣本中成功之個數"
			},
			{
				name: "number_sample",
				description: "為樣本之大小"
			},
			{
				name: "population_s",
				description: "為母體中成功之個數"
			},
			{
				name: "number_pop",
				description: "為母體之大小"
			},
			{
				name: "cumulative",
				description: "為一邏輯值; 當為 TRUE 時，採用累加分配函數; 為 FALSE 時，採用機率密度函數。"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "傳回超幾何分配.",
		arguments: [
			{
				name: "sample_s",
				description: "為樣本中成功之個數"
			},
			{
				name: "number_sample",
				description: "為樣本之大小"
			},
			{
				name: "population_s",
				description: "為母體中成功之個數"
			},
			{
				name: "number_pop",
				description: "為母體之大小。"
			}
		]
	},
	{
		name: "IF",
		description: "檢查是否符合某一條件，且若為 TRUE 則傳回某值，若為 FALSE 則傳回另一值.",
		arguments: [
			{
				name: "logical_test",
				description: "為可求得 TRUE 或 FALSE 的任意值或運算式"
			},
			{
				name: "value_if_true",
				description: "為 Logical_test 等於 TRUE 時所傳回的值。若省略則傳回 TRUE。巢狀 IF 函數最多可使用七層"
			},
			{
				name: "value_if_false",
				description: "為 Logical_test 等於 FALSE 時所傳回的值。若省略則傳回 FALSE。"
			}
		]
	},
	{
		name: "IFERROR",
		description: "如果運算式錯誤傳回 value_if_error，否則傳回運算式本身的值.",
		arguments: [
			{
				name: "value",
				description: "是任何值、運算式或參照"
			},
			{
				name: "value_if_error",
				description: "是任何值、運算式或參照"
			}
		]
	},
	{
		name: "IFNA",
		description: "如果運算式解析為 #N/A，則傳回您所指定的值，否則便傳回該運算式的結果.",
		arguments: [
			{
				name: "value",
				description: "為任何值或運算式或參照"
			},
			{
				name: "value_if_na",
				description: "為任何值或運算式或參照"
			}
		]
	},
	{
		name: "IMABS",
		description: "傳回複數的絕對值 (模數).",
		arguments: [
			{
				name: "inumber",
				description: "是您要其絕對值的複數"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "傳回複數的虛係數.",
		arguments: [
			{
				name: "inumber",
				description: "是您要其虛係數的複數"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "傳回引數 q，這是以弧度表示的角度.",
		arguments: [
			{
				name: "inumber",
				description: "是您要其引數的複數"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "傳回複數的共軛複數.",
		arguments: [
			{
				name: "inumber",
				description: "是您要其共軛的複數"
			}
		]
	},
	{
		name: "IMCOS",
		description: "傳回複數的餘弦值.",
		arguments: [
			{
				name: "inumber",
				description: "是您要其餘弦值的複數"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "傳回複數的雙曲餘弦值.",
		arguments: [
			{
				name: "inumber",
				description: "為要求算雙曲餘弦值的複數"
			}
		]
	},
	{
		name: "IMCOT",
		description: "傳回複數的餘切值.",
		arguments: [
			{
				name: "inumber",
				description: "為要計算餘切值的複數"
			}
		]
	},
	{
		name: "IMCSC",
		description: "傳回複數的餘割值.",
		arguments: [
			{
				name: "inumber",
				description: "為要計算餘割值的複數"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "傳回複數的雙曲餘割值.",
		arguments: [
			{
				name: "inumber",
				description: "為要計算雙曲餘割值的複數"
			}
		]
	},
	{
		name: "IMDIV",
		description: "傳回兩個複數的商數.",
		arguments: [
			{
				name: "inumber1",
				description: "是複數分子或被除數"
			},
			{
				name: "inumber2",
				description: "複數分母或除數"
			}
		]
	},
	{
		name: "IMEXP",
		description: "傳回複數的指數.",
		arguments: [
			{
				name: "inumber",
				description: "是您要其指數的複數"
			}
		]
	},
	{
		name: "IMLN",
		description: "傳回複數的自然對數.",
		arguments: [
			{
				name: "inumber",
				description: "是您要其自然對數的複數"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "傳回複數的底數為 10 對數.",
		arguments: [
			{
				name: "inumber",
				description: "是您要求其常用對數的複數"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "傳回複數底數為 2 的對數.",
		arguments: [
			{
				name: "inumber",
				description: "是您要求其底數為 2 的對數之複數"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "傳回遞增至整數冪的複數.",
		arguments: [
			{
				name: "inumber",
				description: "是您要遞增至乘冪的複數"
			},
			{
				name: "number",
				description: "是您要遞增至複數的乘冪"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "傳回 1 到 255 複數的積數.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "Inumber1, Inumber2,... 從 1 到 255 要乘以的複數。"
			},
			{
				name: "inumber2",
				description: "Inumber1, Inumber2,... 從 1 到 255 要乘以的複數。"
			}
		]
	},
	{
		name: "IMREAL",
		description: "傳回複數的實係數.",
		arguments: [
			{
				name: "inumber",
				description: "是您要其實係數的複數"
			}
		]
	},
	{
		name: "IMSEC",
		description: "傳回複數的正割值.",
		arguments: [
			{
				name: "inumber",
				description: "為要計算正割值的複數"
			}
		]
	},
	{
		name: "IMSECH",
		description: "傳回複數的雙曲正割值.",
		arguments: [
			{
				name: "inumber",
				description: "為要計算雙曲正割值的複數"
			}
		]
	},
	{
		name: "IMSIN",
		description: "傳回複數的正弦值.",
		arguments: [
			{
				name: "inumber",
				description: "是您要其正弦值的複數"
			}
		]
	},
	{
		name: "IMSINH",
		description: "傳回複數的雙曲正弦值.",
		arguments: [
			{
				name: "inumber",
				description: "為要求算雙曲正弦值的複數"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "傳回複數的平方根.",
		arguments: [
			{
				name: "inumber",
				description: "是您要其平方根的複數"
			}
		]
	},
	{
		name: "IMSUB",
		description: "傳回兩個複數的差異.",
		arguments: [
			{
				name: "inumber1",
				description: "是要從其中減去 inumber2 的複數"
			},
			{
				name: "inumber2",
				description: "是要從 inumber1 減去的複數"
			}
		]
	},
	{
		name: "IMSUM",
		description: "傳回複數的總和.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "是從 1 到 255 要加入的複數"
			},
			{
				name: "inumber2",
				description: "是從 1 到 255 要加入的複數"
			}
		]
	},
	{
		name: "IMTAN",
		description: "傳回複數的正切值.",
		arguments: [
			{
				name: "inumber",
				description: "為要計算正切值的複數"
			}
		]
	},
	{
		name: "INDEX",
		description: "傳回數值或是位於給定範圍內特定列或欄交集處的儲存格參照.",
		arguments: [
			{
				name: "array",
				description: "為儲存格範圍或是陣列常數"
			},
			{
				name: "row_num",
				description: "從傳回數值的陣列或參照中選取列。若省略則必須指定 Column_num"
			},
			{
				name: "column_num",
				description: "從傳回數值的陣列或參照中選取欄。若省略則必須指定 Row_num"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "一文字串所指定的參照位址.",
		arguments: [
			{
				name: "ref_text",
				description: "是個單一儲存格的參照位址，而此儲存格含有依 A1 或 R1C1 表示法所指定的參照位址、可當作一個參照位址的名稱或是一個儲存格參照位址的文字串"
			},
			{
				name: "a1",
				description: "為一邏輯值，用以區別 Ref_text 所指定的儲存格參照位址是以哪種方式表示: 若為 FALSE 則 Ref_text 被解釋成 R1C1 參照方式表示; 若為 TRUE 或省略則 Ref_text 被解釋成 A1 參照方式表示。"
			}
		]
	},
	{
		name: "INFO",
		description: "傳回目前作業系統環境的相關資訊。.",
		arguments: [
			{
				name: "type_text",
				description: "為文字值，用以指定所欲傳回的資訊類型。"
			}
		]
	},
	{
		name: "INT",
		description: "傳回無條件捨去後的整數值.",
		arguments: [
			{
				name: "number",
				description: "為要無條件捨去的實數。"
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "根據已知的 x 值及 y 值所計算出來的直線迴歸線，求算出某一點 (x 值) 在此線上的之 y 截距 (y 值).",
		arguments: [
			{
				name: "known_y's",
				description: "為所觀測的因變數值組或資料。這些資料可以是數字，或是含有數字的名稱、陣列或參照位址"
			},
			{
				name: "known_x's",
				description: "為所觀測之自變數值組或資料。這些資料可以是數字，或是含有數字的名稱、陣列或參照位址。"
			}
		]
	},
	{
		name: "INTRATE",
		description: "傳回完整投資證券的利率.",
		arguments: [
			{
				name: "settlement",
				description: "是證券的結帳日期，以數列日期數字表示"
			},
			{
				name: "maturity",
				description: "是證券的到期日期，以數列日期數字表示"
			},
			{
				name: "investment",
				description: "是投資證券的數量"
			},
			{
				name: "redemption",
				description: "是在到期時收到的數量"
			},
			{
				name: "basis",
				description: "是要使用的日計數基礎類型"
			}
		]
	},
	{
		name: "IPMT",
		description: "某項投資於付款方式為定期、定額及固定利率時，某一期應付利息之金額.",
		arguments: [
			{
				name: "rate",
				description: "為各期的利率。例如，使用 6%/4 表示 6% 之下的每季付款利率"
			},
			{
				name: "per",
				description: "為求算利息的期次，其值必須介於 1 到 Nper 之間"
			},
			{
				name: "nper",
				description: "為年金的付款總期數"
			},
			{
				name: "pv",
				description: "為未來各期年金現值的總和"
			},
			{
				name: "fv",
				description: "為最後一次付款完成後，所能獲得的現金餘額 (年金終值)。如果省略 Fv 則假設其值為 0"
			},
			{
				name: "type",
				description: "為邏輯值，用以界定各期金額的給付時點: 0 或省略表示各付款期期末給付，1 表示各付款期期初給付。"
			}
		]
	},
	{
		name: "IRR",
		description: "傳回某一連續期間現金流量的內部報酬率。.",
		arguments: [
			{
				name: "values",
				description: "含有用以計算內部報酬率的各期現金流量數值的陣列或參照位址。"
			},
			{
				name: "guess",
				description: "為接近於 IRR 結果的預估值，如果省略，則以 0.1 (10%) 來計算。"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "檢查某參照是否參照到空白儲存格並傳回 TRUE 或 FALSE。.",
		arguments: [
			{
				name: "value",
				description: "為參照到所欲測試之儲存格的儲存格或名稱。"
			}
		]
	},
	{
		name: "ISERR",
		description: "檢查某值是否為 #N/A 以外的錯誤 (#VALUE!、#REF!、#DIV/0!、#NUM!、#NAME? 或 #NULL!)，並傳回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "value",
				description: "為欲測試之值。此值可參照的對象為: 儲存格、公式、或是參照儲存格、公式或值的名稱。"
			}
		]
	},
	{
		name: "ISERROR",
		description: "檢查某值是否為錯誤值 (#N/A、#VALUE!、#REF!、#DIV/0!、#NUM!、#NAME? 或 #NULL!)，並傳回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "value",
				description: "為欲測試之值。此值可參照的對象為: 儲存格、公式、或是參照到儲存格、公式或數值的名稱。"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "如果數字是偶數傳回 TRUE.",
		arguments: [
			{
				name: "number",
				description: "是要測試的值"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "檢查是否為含有公式之儲存格的參照，然後傳回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "reference",
				description: "為要測試儲存格的參照。參照可以是儲存格參照、公式，或是參照到儲存格的名稱"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "檢查某值是否為邏輯值 (TRUE 或 FALSE) 並傳回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "value",
				description: "為所欲測試之值。此值可參照之對象為: 儲存格、公式、或是參照儲存格、公式或值的名稱。"
			}
		]
	},
	{
		name: "ISNA",
		description: "檢查某值是否為 #N/A (值無法使用) 並傳回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "value",
				description: "為欲測試之值。此值可參照的對象為: 儲存格、公式、或是參照到儲存格、公式或數值的名稱。"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "檢查某值是否非文字 (空白儲存格並非文字) 並傳回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "value",
				description: "為所欲測試之值: 可為儲存格、公式、或是參照儲存格、公式或值的名稱。"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "檢查某值是否為數字並傳回 TRUE 或 FALSE。.",
		arguments: [
			{
				name: "value",
				description: "為所欲測試之值。此值可參照之對象為: 儲存格、公式、或是參照儲存格、公式或值的名稱。"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "傳回指定數值依指定乘算基數無條件的捨入值.",
		arguments: [
			{
				name: "number",
				description: "欲處理之數值"
			},
			{
				name: "significance",
				description: "為用以進行無條件捨位的選用比較基數。"
			}
		]
	},
	{
		name: "ISODD",
		description: "如果數字是奇數傳回 TRUE.",
		arguments: [
			{
				name: "number",
				description: "是要測試的值"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "傳回某個指定日期在該年的 ISO 週數目.",
		arguments: [
			{
				name: "date",
				description: "為 Spreadsheet 用來表示日期和時間計算的日期和時間碼"
			}
		]
	},
	{
		name: "ISPMT",
		description: "傳回指定投資期限內的直線式貸款利息.",
		arguments: [
			{
				name: "rate",
				description: "為每期利率。例如，使用 6%/4 表示 6% 之下的每季付款利率"
			},
			{
				name: "per",
				description: "為所要求算利息之期間"
			},
			{
				name: "nper",
				description: "為年金之付款期數"
			},
			{
				name: "pv",
				description: "為一序列未來付款之現值總和。"
			}
		]
	},
	{
		name: "ISREF",
		description: "檢查某值是否為儲存格參照並傳回 TRUE 或 FALSE。.",
		arguments: [
			{
				name: "value",
				description: "為所欲測試之值。此值可參照的對象為: 儲存格、公式、或是參照儲存格、公式或值的名稱。"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "檢查某值是否為文字並傳回 TRUE 或 FALSE。.",
		arguments: [
			{
				name: "value",
				description: "為所欲測試之值。此值可參照之對象為: 儲存格、公式、或是參照儲存格、公式或值的名稱。"
			}
		]
	},
	{
		name: "KURT",
		description: "傳回一個資料組的峰度值 (Kurtosis).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個引數。可為數字，或是含有數字的名稱、陣列或參照位址，用以求算其峰度值。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個引數。可為數字，或是含有數字的名稱、陣列或參照位址，用以求算其峰度值。"
			}
		]
	},
	{
		name: "LARGE",
		description: "傳回資料組中第 k 大的值。例如第五大的數字.",
		arguments: [
			{
				name: "array",
				description: "為要決定其第 k 大的數值之陣列或資料範圍"
			},
			{
				name: "k",
				description: "為在陣列或儲存格範圍中所欲傳回的位置 (從最大起算)。"
			}
		]
	},
	{
		name: "LCM",
		description: "傳回最小公倍數.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是 1 到 255 您要的最小公倍數值"
			},
			{
				name: "number2",
				description: "是 1 到 255 您要的最小公倍數值"
			}
		]
	},
	{
		name: "LEFT",
		description: "從文字串的第一個字元傳回特定長度之間的所有字元。.",
		arguments: [
			{
				name: "text",
				description: "為包含您所要抽選之字元的文字串。"
			},
			{
				name: "num_chars",
				description: "指定您要用 LEFT 來抽選的字元數; 若省略則為 1。"
			}
		]
	},
	{
		name: "LEN",
		description: "傳回文字字串之字元個數.",
		arguments: [
			{
				name: "text",
				description: "為所要計算字元個數的文字串，字串中所有的空白亦當作字元來計算。"
			}
		]
	},
	{
		name: "LINEST",
		description: "使用最小平方法計算最適合於觀測已知資料點的迴歸直線公式，作為描述線性趨勢的統計資料.",
		arguments: [
			{
				name: "known_y's",
				description: "是 y = mx + b 中一組已知的 y 值"
			},
			{
				name: "known_x's",
				description: "是 y = mx +b 中一組已知的 x 值，這是個可省略的引數"
			},
			{
				name: "const",
				description: "為邏輯值: 若 Const = TRUE 或被省略，常數項 b 將依計算而得; 若 Const = FALSE，常數項 b 將被設定為 0"
			},
			{
				name: "stats",
				description: "為邏輯值: 若為 TRUE 則傳回額外的迴歸統計值; 若為 FALSE 或被省略則只傳回係數 m 和常數項 b。"
			}
		]
	},
	{
		name: "LN",
		description: "傳回數字的自然對數.",
		arguments: [
			{
				name: "number",
				description: "為欲求算其自然對數的正實數。"
			}
		]
	},
	{
		name: "LOG",
		description: "依所指定的基底，傳回數字的對數。.",
		arguments: [
			{
				name: "number",
				description: "為所要處理的正實數。"
			},
			{
				name: "base",
				description: "是計算對數時所使用的基底數值。若省略不填時，預設值為 10。"
			}
		]
	},
	{
		name: "LOG10",
		description: "傳回以 10 為底的對數數字.",
		arguments: [
			{
				name: "number",
				description: "為欲求算其以 10 為底之對數數字的正實數。"
			}
		]
	},
	{
		name: "LOGEST",
		description: "計算符合已知資料點的指數曲線，作為描述該曲線的統計資料.",
		arguments: [
			{
				name: "known_y's",
				description: "為一組符合 y = b*m^x 運算關係的已知 y 值"
			},
			{
				name: "known_x's",
				description: "為一組符合 y = b*m^x 運算關係的已知 x 值，為非必要引數"
			},
			{
				name: "const",
				description: "為邏輯值: 若 Const = TRUE 或省略，即可計算得 b; 若 Const = FALSE 則將 b 設定為 1"
			},
			{
				name: "stats",
				description: "為邏輯值: 若為 TRUE 將傳回額外的迴歸統計資料; 若為 FALSE 或省略則只傳回 m 係數和常數項 b。"
			}
		]
	},
	{
		name: "LOGINV",
		description: "傳回 x 的對數常態累加分配函數的反函數。在此 ln(x) 以平均數 Mean 和標準差 Standard_dev 參數進行常態分配.",
		arguments: [
			{
				name: "probability",
				description: "為一個與對數常態分配相關的機率值，機率值須在 0 和 1 之間，含 0 和 1"
			},
			{
				name: "mean",
				description: "為 ln(x) 的平均數"
			},
			{
				name: "standard_dev",
				description: "為 ln(x) 的標準差，此值需為正值。"
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "傳回 x 的對數分配。在此 ln(x) 以平均數 Mean 和標準差 Standard_dev 參數進行常態分配.",
		arguments: [
			{
				name: "x",
				description: "用來評估函數的值，此值需為正"
			},
			{
				name: "mean",
				description: "是 ln(x) 的平均數"
			},
			{
				name: "standard_dev",
				description: "為 ln(x) 的標準差，此值需為正值"
			},
			{
				name: "cumulative",
				description: "為一邏輯值; 當為 TRUE 時，採用累加分配函數; 為 FALSE 時，採用機率密度函數。"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "傳回 x 的對數常態累加分配函數的反函數。在此 ln(x) 為平均數為 Mean，標準差為 Standard_dev，的常態分配的機率變數。 .",
		arguments: [
			{
				name: "probability",
				description: "為一個與對數常態分配相關的機率值，機率值須在 0 和 1 之間，且可包含 0 和 1"
			},
			{
				name: "mean",
				description: "是 ln(x) 的平均數"
			},
			{
				name: "standard_dev",
				description: "是 ln(x) 的標準差，此值需為正值。"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "傳回 x 的累加對數分配。在此 ln(x) 以平均數 Mean 和標準差 Standard_dev 參數進行常態分配.",
		arguments: [
			{
				name: "x",
				description: "用來評估函數的值，此值需為正"
			},
			{
				name: "mean",
				description: "是 ln(x) 的平均數"
			},
			{
				name: "standard_dev",
				description: "是 ln(x) 的標準差，此值需為正值。"
			}
		]
	},
	{
		name: "LOOKUP",
		description: "從單列或單欄範圍，或是陣列中找出一元素值。其目的在於回溯相容性.",
		arguments: [
			{
				name: "lookup_value",
				description: "為 LOOKUP 在 Lookup_vector 中搜尋的值，且可為數字、文字、邏輯值、或是名稱或某值的參照位址"
			},
			{
				name: "lookup_vector",
				description: "為僅包含單列或單欄文字、數字或邏輯值的範圍並以遞增順序排列"
			},
			{
				name: "result_vector",
				description: "為僅包含單列或單欄的範圍，大小與 Lookup_vector 相同"
			}
		]
	},
	{
		name: "LOWER",
		description: "將文字串轉換成小寫。.",
		arguments: [
			{
				name: "text",
				description: "為要轉換成小寫的文字。文字中非字母的字元不會變更。"
			}
		]
	},
	{
		name: "MATCH",
		description: "為符合陣列中項目的相對位置，而該陣列符合指定順序的指定值.",
		arguments: [
			{
				name: "lookup_value",
				description: "是您要在陣列中尋找的值，可為數值、文字或邏輯值、或是上述某一項之參照"
			},
			{
				name: "lookup_array",
				description: "為連續的儲存格範圍，其中含有可能的查詢資料、陣列值、或陣列參照"
			},
			{
				name: "match_type",
				description: "為 1、0 或 -1，用以表示所要傳回的值。"
			}
		]
	},
	{
		name: "MAX",
		description: "傳回引數中的最大值。邏輯值及文字將被略過而不計.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個引數，其內容可為數值、空白儲存格、邏輯值、文字字串。此函數將傳回這些引數的最大值。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個引數，其內容可為數值、空白儲存格、邏輯值、文字字串。此函數將傳回這些引數的最大值。"
			}
		]
	},
	{
		name: "MAXA",
		description: "傳回引數值串列中的最大值。包括邏輯值和文字.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "為欲求其最大值的 1 到 255 個引數，其內容可為數字、空儲存格、邏輯值或文數字。"
			},
			{
				name: "value2",
				description: "為欲求其最大值的 1 到 255 個引數，其內容可為數字、空儲存格、邏輯值或文數字。"
			}
		]
	},
	{
		name: "MDETERM",
		description: "傳回陣列之矩陣行列式.",
		arguments: [
			{
				name: "array",
				description: "為一具有相同行數與列數的數值陣列、儲存格範圍或是常數陣列。"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "傳回引數串列內的中位數或一組給定數字內的中間數字.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個數字、名稱、陣列或參照，其內容包含欲求出中位數的數字。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個數字、名稱、陣列或參照，其內容包含欲求出中位數的數字。"
			}
		]
	},
	{
		name: "MID",
		description: "傳回從文字串中的某個起始位置到指定長度之間的字元.",
		arguments: [
			{
				name: "text",
				description: "為欲從中抽取字元的文字串"
			},
			{
				name: "start_num",
				description: "為您要抽取之第一個字元的位置。Text 中第一個字元的位置為 1"
			},
			{
				name: "num_chars",
				description: "指定要從 Text 中傳回的字元數。"
			}
		]
	},
	{
		name: "MIN",
		description: "傳回引數串列中的最小值 。邏輯值及文字將被略過而不計。 .",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個引數，其內容可為數值、空白儲存格、邏輯值、文字數字。此函數將傳回這些引數的最小值。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個引數，其內容可為數值、空白儲存格、邏輯值、文字數字。此函數將傳回這些引數的最小值。"
			}
		]
	},
	{
		name: "MINA",
		description: "傳回引數串列中的最小值。將不會省略邏輯值及文字.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "為欲求其最小值的 1 到 255 個引數，其內容可為數字、空白儲存格、邏輯值或文數字。"
			},
			{
				name: "value2",
				description: "為欲求其最小值的 1 到 255 個引數，其內容可為數字、空白儲存格、邏輯值或文數字。"
			}
		]
	},
	{
		name: "MINUTE",
		description: "傳回時間值的分鐘部分，從 0 到 59 的數字。.",
		arguments: [
			{
				name: "serial_number",
				description: "為 Spreadsheet 所使用的日期和時間碼數字，或是時間格式的文字，例如 16:48:00 或 4:48:00 PM。"
			}
		]
	},
	{
		name: "MINVERSE",
		description: "傳回儲存於某陣列中之矩陣的反矩陣.",
		arguments: [
			{
				name: "array",
				description: "為一具有相同行數與列數的數值陣列、儲存格範圍、或是常數陣列。"
			}
		]
	},
	{
		name: "MIRR",
		description: "依據所輸入的每期現金流量，考量投資成本及現金再投資利率，傳回各期現金流量內部報酬率。.",
		arguments: [
			{
				name: "values",
				description: "是個含有代表各期支出(負數)及收入(正數)數值的陣列或儲存格參照位址。"
			},
			{
				name: "finance_rate",
				description: "為投入資金的融資利率。"
			},
			{
				name: "reinvest_rate",
				description: "為各期收入淨額的轉投資報酬率。"
			}
		]
	},
	{
		name: "MMULT",
		description: "傳回兩陣列相乘之乘積。傳回的陣列的列數將與 array1 相同，欄數將與 array2 相同.",
		arguments: [
			{
				name: "array1",
				description: "要相乘的第一個陣列。此陣列的欄數需和 array2 的列數相同。"
			},
			{
				name: "array2",
				description: "要相乘的第一個陣列。此陣列的欄數需和 array2 的列數相同。"
			}
		]
	},
	{
		name: "MOD",
		description: "傳回兩數相除後之餘數.",
		arguments: [
			{
				name: "number",
				description: "為您要尋找的餘數"
			},
			{
				name: "divisor",
				description: " 為 Number 之除數。"
			}
		]
	},
	{
		name: "MODE",
		description: "傳回在一陣列或範圍的資料中出現頻率最高的值 (眾數).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個數值，或是含有數字的名稱、陣列或參照位址，用以求算眾數。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個數值，或是含有數字的名稱、陣列或參照位址，用以求算眾數。"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "傳回在一陣列或範圍的資料中出現頻率最高之值的垂直陣列。若為水平陣列，則使用 =TRANSPOSE(MODE.MULT(number1,number2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個數值，或是含有數字的名稱、陣列或參照位址，用以求算眾數。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個數值，或是含有數字的名稱、陣列或參照位址，用以求算眾數。"
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "傳回在一陣列或範圍的資料中出現頻率最高的值.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個數值，或是含有數字的名稱、陣列或參照位址，用以求算眾數。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個數值，或是含有數字的名稱、陣列或參照位址，用以求算眾數。"
			}
		]
	},
	{
		name: "MONTH",
		description: "傳回月份，為 1 (一月) 到 12 (十二月) 的數字。.",
		arguments: [
			{
				name: "serial_number",
				description: "係指 Spreadsheet 所使用的日期和時間碼的數字。"
			}
		]
	},
	{
		name: "MROUND",
		description: "傳回四捨五入為所需倍數的數字.",
		arguments: [
			{
				name: "number",
				description: "是要四捨五入的值"
			},
			{
				name: "multiple",
				description: "是您要四捨五入數字的倍數"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "傳回數字集的多項式.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是 1 到 255 您要多項式的值"
			},
			{
				name: "number2",
				description: "是 1 到 255 您要多項式的值"
			}
		]
	},
	{
		name: "MUNIT",
		description: "傳回指定維度的單位矩陣.",
		arguments: [
			{
				name: "dimension",
				description: "為指定要傳回之單位矩陣維度的整數"
			}
		]
	},
	{
		name: "N",
		description: "將非數字轉換成數字，日期轉換成序列值，TRUE 轉換成 1，其他值轉換成 0 (零).",
		arguments: [
			{
				name: "value",
				description: "為所要轉換的值。"
			}
		]
	},
	{
		name: "NA",
		description: "傳回錯誤值 #N/A (無此值)。.",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "傳回負二項分配的機率值，其代表在單次實驗的成功機率為 Probability_s，在出現第 Number_s-th 成功之前，出現 Number_f 次失敗的這種情況的機率值.",
		arguments: [
			{
				name: "number_f",
				description: "為失敗的次數"
			},
			{
				name: "number_s",
				description: "為成功的次數"
			},
			{
				name: "probability_s",
				description: "是成功的機率，此值需在 0 和 1 之間。"
			},
			{
				name: "cumulative",
				description: "為一邏輯值: TRUE 則採用累加分配函數; FALSE 則採用機率質量函數。"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "傳回負二項分配的機率值，其代表在單次實驗的成功機率為 Probability_s ，在出現第 Number_s-th 成功之前，出現 Number_f  次失敗的這種情況的機率值.",
		arguments: [
			{
				name: "number_f",
				description: "失敗的次數"
			},
			{
				name: "number_s",
				description: "成功的次數"
			},
			{
				name: "probability_s",
				description: "是成功的機率，此值需在 0 和 1 之間。"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "傳回兩個日期之間所有工作日的數目.",
		arguments: [
			{
				name: "start_date",
				description: "是代表開始日期的數列日期數字"
			},
			{
				name: "end_date",
				description: "是代表結束日期的數列日期數字"
			},
			{
				name: "holidays",
				description: "是一或多個數列日期數字的選用集，會從工作行事曆中排除，例如，州假日和聯邦假日以及彈性假日"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "傳回兩個日期之間所有工作日的數目 (含自訂週末參數).",
		arguments: [
			{
				name: "start_date",
				description: "是代表開始日期的數列日期數字"
			},
			{
				name: "end_date",
				description: "是代表結束日期的數列日期數字"
			},
			{
				name: "weekend",
				description: "是指定何時是週末的數字或字串"
			},
			{
				name: "holidays",
				description: "是一或多個數列日期數字的選用集，會從工作行事曆中排除，例如，州假日和聯邦假日以及彈性假日"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "傳回年度名義利率.",
		arguments: [
			{
				name: "effect_rate",
				description: "是實利率"
			},
			{
				name: "npery",
				description: "是每年複利週期的數字"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "傳回指定平均數和標準差下的常態分配.",
		arguments: [
			{
				name: "x",
				description: "為所要求算分配的數值"
			},
			{
				name: "mean",
				description: "為分配的算術平均數"
			},
			{
				name: "standard_dev",
				description: "為分配的標準差，此值需為正值"
			},
			{
				name: "cumulative",
				description: "為一邏輯值: 當為 TRUE 時，採用累加分配函數; 為 FALSE 時，採用機率密度函數。"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "在指定平均數和標準差下，傳回標準常態累加分配的反分配.",
		arguments: [
			{
				name: "probability",
				description: "是相對於常態分配的機率值，此值須在 0 到 1 之間，且可包含 0 及 1"
			},
			{
				name: "mean",
				description: "為分配的算術平均數"
			},
			{
				name: "standard_dev",
				description: "為分配的標準差，此值需為正值。"
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "傳回標準常態分配 (即平均值為 0，標準差為 1).",
		arguments: [
			{
				name: "z",
				description: "為所要求算分配的數值。"
			},
			{
				name: "cumulative",
				description: "為函數傳回的邏輯值; 當為 TRUE 時，採用累加分配函數; 為 FALSE 時，採用機率質量函數。"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "傳回標準常態累加函數的反函數 (即平均數為 0，標準差為 1).",
		arguments: [
			{
				name: "probability",
				description: "是相對於常態分配的機率值，此值須在 0 到 1 之間，且可包含 0 及 1。"
			}
		]
	},
	{
		name: "NORMDIST",
		description: "傳回指定平均數和標準差下的常態累加分配.",
		arguments: [
			{
				name: "x",
				description: "為所要求算分配的數值"
			},
			{
				name: "mean",
				description: "為分配的算術平均數"
			},
			{
				name: "standard_dev",
				description: "為分配的標準差，此值需為正值"
			},
			{
				name: "cumulative",
				description: "為一邏輯值; 當為 TRUE 時，採用累加分配函數; 為 FALSE 時，採用機率密度函數。"
			}
		]
	},
	{
		name: "NORMINV",
		description: "在指定平均數和標準差下，傳回常態累加分配的反分配.",
		arguments: [
			{
				name: "probability",
				description: "是相對於常態分配的機率值，此值須在 0 到 1 之間，且可包含 0 及 1"
			},
			{
				name: "mean",
				description: "為分配的算術平均數"
			},
			{
				name: "standard_dev",
				description: "為分配的標準差，此值需為正值。"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "傳回標準常態累加分配 (即平均值為 0，標準差為 1 的機率分配).",
		arguments: [
			{
				name: "z",
				description: "為所要求算分配的數值。"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "傳回標準常態累加函數的反函數 (即平均數為 0，標準差為 1).",
		arguments: [
			{
				name: "probability",
				description: "是相對於常態分配的機率值，此值須在 0 到 1 之間，且可包含 0 及 1。"
			}
		]
	},
	{
		name: "NOT",
		description: "將 FALSE 變更為 TRUE，或是將  TRUE 變更為 FALSE.",
		arguments: [
			{
				name: "logical",
				description: "為可求得 TRUE 或 FALSE 的值或運算式。"
			}
		]
	},
	{
		name: "NOW",
		description: "傳回格式為日期與時間的目前日期與時間。.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "每期付款金額及固定利率之某項投資的期數.",
		arguments: [
			{
				name: "rate",
				description: "為各期的利率。例如，使用 6%/4 表示 6% 之下的每季付款利率"
			},
			{
				name: "pmt",
				description: "為各期應給付的金額; 其於投資期間均不能改變"
			},
			{
				name: "pv",
				description: "為淨現值，或未來各期年金現值的總和"
			},
			{
				name: "fv",
				description: "是未來的數值，為最後一次付款完成後，所能獲得的現金餘額 (年金終值)。若省略則其值為 0"
			},
			{
				name: "type",
				description: "為一邏輯值: 1 表示期初給付; 0 或省略表示期末給付。"
			}
		]
	},
	{
		name: "NPV",
		description: "在已知貼現率、各期付款值 (負值) 及收入 (正值) 的情況下，求出此投資的淨現值.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rate",
				description: "為一段期間內的貼現率"
			},
			{
				name: "value1",
				description: " 為 1 到 254 個付款或收入的組合。並且此些付款或是收入平均地發生於各期的期末。"
			},
			{
				name: "value2",
				description: " 為 1 到 254 個付款或收入的組合。並且此些付款或是收入平均地發生於各期的期末。"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "以與地區設定無關的方式，將文字轉換為數字.",
		arguments: [
			{
				name: "text",
				description: "為代表要轉換數字的字串"
			},
			{
				name: "decimal_separator",
				description: "為字串中作為小數分隔符號的字元"
			},
			{
				name: "group_separator",
				description: "為字串中作為群組分隔符號的字元"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "將八進位數字轉換成二進位.",
		arguments: [
			{
				name: "number",
				description: "是您要轉換的八進位數字"
			},
			{
				name: "places",
				description: "是要使用的字元數"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "將八進位數字轉換成十進位.",
		arguments: [
			{
				name: "number",
				description: "是您要轉換的八進位數字"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "將八進位數字轉換成十六進位.",
		arguments: [
			{
				name: "number",
				description: "是您要轉換的八進位數字"
			},
			{
				name: "places",
				description: "是要使用的字元數"
			}
		]
	},
	{
		name: "ODD",
		description: "無條件進位正數與負數，該數值最接近的奇數值.",
		arguments: [
			{
				name: "number",
				description: "為所要捨入的數值。"
			}
		]
	},
	{
		name: "OFFSET",
		description: "根據所指定的參照位址取得列數及欄數的範圍。.",
		arguments: [
			{
				name: "reference",
				description: "是個參照位址，它是您用以計算位移結果的起始位置，參照到相鄰選取範圍的一個儲存格或範圍。"
			},
			{
				name: "rows",
				description: "是用以指示左上角儲存格要垂直 (往上或往下) 移動的列數。"
			},
			{
				name: "cols",
				description: "是用以指示左上角儲存格要水平 (往左或往右) 移動的欄數。"
			},
			{
				name: "height",
				description: "是設定傳回的參照位址應包括的儲存格高度 (儲存格範圍的列數) 的數值，若省略則高度同 Reference"
			},
			{
				name: "width",
				description: "是設定傳回的參照位址應包括的儲存格寬度 (儲存格範圍的欄數) 的數值，若省略則寬度同 Reference。"
			}
		]
	},
	{
		name: "OR",
		description: "檢查是否有任一引數為 TRUE 並傳回 TRUE 或 FALSE。當所有的引數皆為 FALSE 時才會傳回 FALSE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "為欲測試之 1 到 255 個條件式，其值可為 TRUE 或 FALSE。"
			},
			{
				name: "logical2",
				description: "為欲測試之 1 到 255 個條件式，其值可為 TRUE 或 FALSE。"
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
		description: "傳回投資達到指定值時所需的週期數.",
		arguments: [
			{
				name: "rate",
				description: "為每期的利率。"
			},
			{
				name: "pv",
				description: "為投資的現值"
			},
			{
				name: "fv",
				description: "為所期望的投資未來值"
			}
		]
	},
	{
		name: "PEARSON",
		description: "傳回皮耳森積差相關係數 r.",
		arguments: [
			{
				name: "array1",
				description: "是一組自變數"
			},
			{
				name: "array2",
				description: "是一組因變數。"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "傳回範圍中位於第 K 個百分比的數值.",
		arguments: [
			{
				name: "array",
				description: "是一個陣列或定義出相對位置的資料範圍，用以求算百分位數"
			},
			{
				name: "k",
				description: "是在 0 到 1 的範圍之內的百分位數，包含 0 及 1。"
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "傳回範圍中位於第 k 個百分比的數值，k 是在 0 到 1 的範圍之內，且不含 0 及 1.",
		arguments: [
			{
				name: "array",
				description: "是一個陣列或定義出相對位置的資料範圍，用以求算百分位數"
			},
			{
				name: "k",
				description: "是在 0 到 1 的範圍之內的百分位數，且包含 0 及 1。"
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "傳回範圍中位於第 k 個百分比的數值，k 是在 0 到 1 的範圍之內，且包含 0 及 1.",
		arguments: [
			{
				name: "array",
				description: "是一個陣列或定義出相對位置的資料範圍，用以求算百分位數"
			},
			{
				name: "k",
				description: "是在 0 到 1 的範圍之內的百分位數，且包含 0 及 1。"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "傳回某數值在一個資料組中的百分比的等級.",
		arguments: [
			{
				name: "array",
				description: "是一個陣列或定義出相對位置的數值資料範圍"
			},
			{
				name: "x",
				description: "為欲求算其等級的值"
			},
			{
				name: "significance",
				description: "為欲求百分比值的小數有效位數 (選擇性引數)，若省略則採用小數 3 位 (0.xxx%)。"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "傳回某數值在一個資料組中的百分比 (0 到 1，不含 0 和 1) 的等級.",
		arguments: [
			{
				name: "array",
				description: "是一個陣列或定義出相對位置的數值資料範圍"
			},
			{
				name: "x",
				description: "為欲求算其等級的值"
			},
			{
				name: "significance",
				description: "為欲求百分比值的小數有效位數 (選擇性引數)，若省略則採用小數 3 位 (0.xxx%)。"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "傳回某數值在一個資料組中的百分比 (0 到 1，含 0 和 1) 的等級.",
		arguments: [
			{
				name: "array",
				description: "是一個陣列或定義出相對位置的數值資料範圍"
			},
			{
				name: "x",
				description: "為欲求算其等級的值"
			},
			{
				name: "significance",
				description: "為欲求百分比值的小數有效位數 (選擇性引數)，若省略則採用小數 3 位 (0.xxx%)。"
			}
		]
	},
	{
		name: "PERMUT",
		description: "傳回從數值物件中選取特定數量的物件時，所有可能排列方式的個數.",
		arguments: [
			{
				name: "number",
				description: "為物體總數"
			},
			{
				name: "number_chosen",
				description: "為每次排列要選取的物件個數。"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "傳回從所有物件選取指定數量的物件時 (包括重複選取物件)，所有可能排列方式的數目.",
		arguments: [
			{
				name: "number",
				description: "為物件的總數"
			},
			{
				name: "number_chosen",
				description: "為每一種排列方式所含的物件數目"
			}
		]
	},
	{
		name: "PHI",
		description: "傳回標準常態分配的密度函數值.",
		arguments: [
			{
				name: "x",
				description: "為要求算標準常態分配密度的數字"
			}
		]
	},
	{
		name: "PI",
		description: "傳回圓週率 Pi 值 (3.14159265358979， 精確到小數 15 位)。.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "每期付款金額及利率固定之下計算年金期付款數額。.",
		arguments: [
			{
				name: "rate",
				description: "為各期的利率。例如，使用 6%/4 表示 6% 之下的每季付款利率。"
			},
			{
				name: "nper",
				description: "為貸款的總付款期數。"
			},
			{
				name: "pv",
				description: "為總淨現值，亦即未來各期年金現值的總和。"
			},
			{
				name: "fv",
				description: "為最後一次付款完成後，所能獲得的現金餘額 (年金終值)，若省略則此值為 0 (零)。"
			},
			{
				name: "type",
				description: "為一邏輯值: 1 表示期初給付; 0 或省略表示期末給付。"
			}
		]
	},
	{
		name: "POISSON",
		description: "傳回波氏分配.",
		arguments: [
			{
				name: "x",
				description: "事件出現的次數"
			},
			{
				name: "mean",
				description: "期望值，需為正數"
			},
			{
				name: "cumulative",
				description: "為一邏輯值; 當為 TRUE 時，傳回波氏分配的累加機率值;當為 FALSE 時，以波氏機率質量函數計算。"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "傳回波氏分配.",
		arguments: [
			{
				name: "x",
				description: "事件出現的次數"
			},
			{
				name: "mean",
				description: "為期望值，需為正數"
			},
			{
				name: "cumulative",
				description: "為一邏輯值; 當為 TRUE 時，傳回波氏分配的累加機率值; 當為 FALSE 時，以波氏機率質量函數計算。"
			}
		]
	},
	{
		name: "POWER",
		description: "傳回數字乘幕的結果.",
		arguments: [
			{
				name: "number",
				description: "是可為任意實數的底數"
			},
			{
				name: "power",
				description: "是指數，即底數所要乘方的次數。"
			}
		]
	},
	{
		name: "PPMT",
		description: "傳回每期付款金額及利率皆為固定之某項投資於某期付款中的本金金額.",
		arguments: [
			{
				name: "rate",
				description: "為各期的利率。例如，使用 6%/4 表示 6% 之下的每季付款利率"
			},
			{
				name: "per",
				description: "為所求的特定期間，其值必須介於 1 到 Nper 之間"
			},
			{
				name: "nper",
				description: "為年金的付款總期數"
			},
			{
				name: "pv",
				description: "為未來各期年金現值的總和"
			},
			{
				name: "fv",
				description: "為最後一次付款完成後，所能獲得的現金餘額 (年金終值)"
			},
			{
				name: "type",
				description: "為一邏輯值: 1 表示各付款期期初給付，0 或省略表示各付款期期末給付。"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "傳回貼現證券每 $100 面額的價格.",
		arguments: [
			{
				name: "settlement",
				description: "是證券的結帳日期，以數列日期數字表示"
			},
			{
				name: "maturity",
				description: "是證券的到期日期，以數列日期數字表示"
			},
			{
				name: "discount",
				description: "是證券的貼現率"
			},
			{
				name: "redemption",
				description: "是每 $100 面額證券的贖回價值"
			},
			{
				name: "basis",
				description: "是要使用的日計數基礎類型"
			}
		]
	},
	{
		name: "PROB",
		description: "傳回落在 lower_limit 和 upper_limit 之間或等於 lower_limit 的機率值.",
		arguments: [
			{
				name: "x_range",
				description: "為 x 值所在的範圍，每一個值都有其相對的機率值"
			},
			{
				name: "prob_range",
				description: "為 x_range 所對應的機率值範圍，各個機率值需介於 0 到 1 之間 (不包含 0 )"
			},
			{
				name: "lower_limit",
				description: "是所求的機率的下限值"
			},
			{
				name: "upper_limit",
				description: "是所求的機率的選擇性上限值。如果省略，PROB 將傳回 X_range 值等於 Lower_limit 的機率值。"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "所有引數數值相乘之乘積。.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個欲相乘之引數，其值為數字、邏輯值或數字的文字格式。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個欲相乘之引數，其值為數字、邏輯值或數字的文字格式。"
			}
		]
	},
	{
		name: "PROPER",
		description: "將文字串轉換成適當的大小寫; 各單字的第一個字母轉換成大寫，其餘所有的字母則都轉換成小寫。.",
		arguments: [
			{
				name: "text",
				description: "文字是以引號括住的文字，傳回文字的公式或是一個意指包含您想要將其部分變為大寫的文字之儲存格的參照。"
			}
		]
	},
	{
		name: "PV",
		description: "傳回某項投資的年金現值: 年金現值為未來各期年金現值的總和.",
		arguments: [
			{
				name: "rate",
				description: "為各期的利率。例如，使用 6%/4 表示 6% 之下的每季付款利率"
			},
			{
				name: "nper",
				description: "為年金的總付款期數"
			},
			{
				name: "pmt",
				description: "為各期所應給付的固定金額且不得在年金期限內變更"
			},
			{
				name: "fv",
				description: "為最後一次付款完成後，所能獲得的現金餘額 (年金終值)"
			},
			{
				name: "type",
				description: "為一邏輯值: 1 表示期初給付; 0 或省略表示期末給付。"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "傳回資料組的四分位數.",
		arguments: [
			{
				name: "array",
				description: "為陣列或內容為數值的儲存格範圍，用以求算其四分位值"
			},
			{
				name: "quart",
				description: "為一數字: 0 = 最小值; 1 = 第一四分位; 2 = 中位數; 3 = 第三四分位; 4 = 最大值。"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "傳回資料組的四分位數 (根據範圍從 0 到 1 的百分位數，不含 0 和 1).",
		arguments: [
			{
				name: "array",
				description: "為陣列或內容為數值的儲存格範圍，用以求算其四分位值"
			},
			{
				name: "quart",
				description: "為一數字: 0 = 最小值; 1 = 第一四分位; 2 = 中位數; 3 = 第三四分位; 4 = 最大值。"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "傳回資料組的四分位數 (根據範圍從 0 到 1 的百分位數，含 0 和 1).",
		arguments: [
			{
				name: "array",
				description: "為陣列或內容為數值的儲存格範圍，用以求算其四分位值"
			},
			{
				name: "quart",
				description: "為一數字: 0 = 最小值; 1 = 第一四分位; 2 = 中位數; 3 = 第三四分位; 4 = 最大值。"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "傳回除法的整數部分.",
		arguments: [
			{
				name: "numerator",
				description: "是被除數"
			},
			{
				name: "denominator",
				description: "是除數"
			}
		]
	},
	{
		name: "RADIANS",
		description: "將角度轉為弧度.",
		arguments: [
			{
				name: "angle",
				description: "為所要轉成弳度的角度值。"
			}
		]
	},
	{
		name: "RAND",
		description: "大於等於 0 且小於 1 的隨機亂數，每當工作表重算時，便會傳回一個新的隨機亂數。.",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "傳回您指定的數字之間的隨機數字.",
		arguments: [
			{
				name: "bottom",
				description: "是將傳回的最小整數 RANDBETWEEN"
			},
			{
				name: "top",
				description: "是將傳回的最大整數 RANDBETWEEN"
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
		description: "傳回某數字在某串列數字中之順序，亦即該數字相對於清單中其他數值的大小.",
		arguments: [
			{
				name: "number",
				description: "為欲找出其順序的數字"
			},
			{
				name: "ref",
				description: "為串列數字所在的陣列或參照範圍。該範圍中之非數值將被忽略"
			},
			{
				name: "order",
				description: "為一數字，用以指定排序方式: 0 或省略代表以遞減方式排序; 其他任何非零數值則表示在清單中以遞增方式排序。"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "傳回某數字在某串列數字中之等級，亦即該數字相對於清單中其他數值的大小; 若有多值有相同等級，將會傳回平均等級.",
		arguments: [
			{
				name: "number",
				description: "為欲找出其等級的數字"
			},
			{
				name: "ref",
				description: "為串列數字所在的陣列或參照範圍。該範圍中之非數值將被忽略"
			},
			{
				name: "order",
				description: "為一數字，用以指定排序方式: 0 或省略代表以遞減方式排序; 其他任何非零數值則表示在清單中以遞增方式排序。"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "傳回某數字在某串列數字中之等級，亦即該數字相對於清單中其他數值的大小; 若有多值有相同等級，將會傳回該組數值中的最高等級.",
		arguments: [
			{
				name: "number",
				description: "為欲找出其等級的數字"
			},
			{
				name: "ref",
				description: "為串列數字所在的陣列或參照範圍。該範圍中之非數值將被忽略"
			},
			{
				name: "order",
				description: "為一數字，用以指定排序方式: 0 或省略代表以遞減方式排序; 其他任何非零數值則表示在清單中以遞增方式排序。"
			}
		]
	},
	{
		name: "RATE",
		description: "貸款或年金每期的利率。例如，使用 6%/4 表示 6% 之下的每季付款利率。.",
		arguments: [
			{
				name: "nper",
				description: "為貸款或年金的總付款期數。"
			},
			{
				name: "pmt",
				description: "為各期所應給付的固定金額且不得在貸款或年金期限內變更"
			},
			{
				name: "pv",
				description: "為淨現值，亦即未來各期年金現值的總和。"
			},
			{
				name: "fv",
				description: "為最後一次付款完成後，所能獲得的現金餘額 (年金終值)。若省略則 Fv = 0"
			},
			{
				name: "type",
				description: "為一邏輯值: 1 表示期初給付; 0 或省略表示期末給付"
			},
			{
				name: "guess",
				description: "為您對期利率的猜測數; 若此值省略則 Guess = 0.1 (10 %)。"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "傳回完整投資證券在到期時收到的數量.",
		arguments: [
			{
				name: "settlement",
				description: "是證券的結帳日期，以數列日期數字表示"
			},
			{
				name: "maturity",
				description: "是證券的到期日期，以數列日期數字表示"
			},
			{
				name: "investment",
				description: "是投資證券的數量"
			},
			{
				name: "discount",
				description: "是證券的貼現率"
			},
			{
				name: "basis",
				description: "是要使用的日計數基礎類型"
			}
		]
	},
	{
		name: "REPLACE",
		description: "將字串中的一部分以其他字串取代。.",
		arguments: [
			{
				name: "old_text",
				description: "其中有部分字元要被取代的文字字串。"
			},
			{
				name: "start_num",
				description: "指出在 old_text 中要以 new_text 取代的開始位置。"
			},
			{
				name: "num_chars",
				description: "為被取代的字串長度。"
			},
			{
				name: "new_text",
				description: "為所要換入的新字串。"
			}
		]
	},
	{
		name: "REPT",
		description: "依指定字串重複幾次顯示。您可以利用 REPT 函數將儲存格以某些文字或字串填滿.",
		arguments: [
			{
				name: "text",
				description: "為所要重複顯示的文字資料"
			},
			{
				name: "number_times",
				description: "為一正數，用以指定所要重複的次數。"
			}
		]
	},
	{
		name: "RIGHT",
		description: "從文字串的最後一個字元傳回特定長度之間的所有字元。.",
		arguments: [
			{
				name: "text",
				description: "為包含您所要抽選之字元的文字串。"
			},
			{
				name: "num_chars",
				description: "指定您要抽選的字元數; 若省略則為 1。"
			}
		]
	},
	{
		name: "ROMAN",
		description: "將阿拉伯數字轉成羅馬字並以文字型態顯示.",
		arguments: [
			{
				name: "number",
				description: "是要轉換的阿拉伯數字"
			},
			{
				name: "form",
				description: "為一數字，用以指定要轉成的羅馬字型態。"
			}
		]
	},
	{
		name: "ROUND",
		description: "依所指定的位數，將數字四捨五入.",
		arguments: [
			{
				name: "number",
				description: "為所要執行四捨五入的數字"
			},
			{
				name: "num_digits",
				description: "為要執行四捨五入計算時所採用的位數。當為負值時，則表示四捨五入到小數點前的指定位數。當為正值，則表示到小數點後的指定位數。"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "將一數字以趨近於零方式捨位.",
		arguments: [
			{
				name: "number",
				description: "為所要捨位之實數"
			},
			{
				name: "num_digits",
				description: "為所要捨位之小數位數。負數代表捨位至小數點左方的位數 (即整數位數); 0 或省略不填，則捨位至最接近的整數。"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "將一數字以背離於零之方式進位.",
		arguments: [
			{
				name: "number",
				description: "為所要捨位之實數"
			},
			{
				name: "num_digits",
				description: "為所要捨位之小數位數。負數代表捨位至小數點左方的位數 (即整數位數); 0 或省略不填，則捨位至最接近的整數。"
			}
		]
	},
	{
		name: "ROW",
		description: "傳回 reference 中之列號.",
		arguments: [
			{
				name: "reference",
				description: "為所要求算列號的單一儲存格或儲存格範圍。如果省略不填，則傳回包含 ROW 函數的儲存格。"
			}
		]
	},
	{
		name: "ROWS",
		description: "傳回陣列或參照位址所含的列數。.",
		arguments: [
			{
				name: "array",
				description: "為陣列、陣列公式或儲存格範圍的參照位址。"
			}
		]
	},
	{
		name: "RRI",
		description: "傳回投資成長的對等利率.",
		arguments: [
			{
				name: "nper",
				description: "為投資的期數"
			},
			{
				name: "pv",
				description: "為投資的現值"
			},
			{
				name: "fv",
				description: "為投資的未來值"
			}
		]
	},
	{
		name: "RSQ",
		description: "傳回以已知資料點來求得皮耳森相關係數的平方值.",
		arguments: [
			{
				name: "known_y's",
				description: "因變數資料點的陣列或輸入範圍。此範圍可以是內含數值的名稱、陣列、儲存格參照"
			},
			{
				name: "known_x's",
				description: "因變數資料點的陣列或輸入範圍。此範圍可以是內含數值的名稱、陣列、儲存格參照。"
			}
		]
	},
	{
		name: "RTD",
		description: "從支援 COM Automation 的程式中取出即時資料.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "是已註冊之 COM Automation 增益集的 ProgID 的名稱。請將執行增益集的伺服器的名稱置於引號內"
			},
			{
				name: "server",
				description: "是要執行增益集的伺服器的名稱。請將此名稱置於引號內。若要在本機執行增益集，請使用空字串"
			},
			{
				name: "string1",
				description: "是個數從 1 到 38 的參數，用來指定一段資料"
			},
			{
				name: "string2",
				description: "是個數從 1 到 38 的參數，用來指定一段資料"
			}
		]
	},
	{
		name: "SEARCH",
		description: "傳回某特定文字串首次出現在另一個文字串的字元位置 (大小寫視為相同)。.",
		arguments: [
			{
				name: "find_text",
				description: "為所要搜尋的文字串。您可以使用 ? 及 * 等萬用字元。若要搜尋 ? 及 * 號，請用 ~? 及 ~* 。"
			},
			{
				name: "within_text",
				description: "為您所要搜尋 find_text 的搜尋範圍字串。"
			},
			{
				name: "start_num",
				description: "指定欲從 within_text 的第若干個字元開始尋找。如果省略不填，預設值為 1。"
			}
		]
	},
	{
		name: "SEC",
		description: "傳回角度的正割值.",
		arguments: [
			{
				name: "number",
				description: "為要計算正割值的角度，以弧度表示"
			}
		]
	},
	{
		name: "SECH",
		description: "傳回角度的雙曲正割值.",
		arguments: [
			{
				name: "number",
				description: "為要計算雙曲正割值的角度，以弧度表示"
			}
		]
	},
	{
		name: "SECOND",
		description: "傳回時間值的秒數部分，從 0 到 59 的數字。.",
		arguments: [
			{
				name: "serial_number",
				description: "為 Spreadsheet 所使用的日期和時間碼數字，或是時間格式的文字，例如 16:48:23 或 4:48:47 PM。"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "傳回依據公式的冪級數總和.",
		arguments: [
			{
				name: "x",
				description: "是冪級數的輸入值"
			},
			{
				name: "n",
				description: "是您要增加 x 的初始乘冪"
			},
			{
				name: "m",
				description: "是在級數中每一個項目增加 n 的步進"
			},
			{
				name: "coefficients",
				description: "是 x 的連續乘冪相乘的係數集"
			}
		]
	},
	{
		name: "SHEET",
		description: "傳回參照工作表的工作表號碼.",
		arguments: [
			{
				name: "value",
				description: "為要取得工作表號碼的工作表名稱或參照。若省略，則會傳回含有該函數的工作表號碼"
			}
		]
	},
	{
		name: "SHEETS",
		description: "傳回參照中的工作表數目.",
		arguments: [
			{
				name: "reference",
				description: "為欲求取所含工作表數目的參照。若省略，則會傳回含有該函數之活頁簿的工作表數目"
			}
		]
	},
	{
		name: "SIGN",
		description: "傳回代表數字正負號的代碼: 1 代表正數，0 代表數字為零，-1 代表負數.",
		arguments: [
			{
				name: "number",
				description: "為任意實數。"
			}
		]
	},
	{
		name: "SIN",
		description: "傳回一角度之正弦值.",
		arguments: [
			{
				name: "number",
				description: "為欲求算其正弦值的角度，以弧度表示。度數 * PI()/180 = 弧度"
			}
		]
	},
	{
		name: "SINH",
		description: "傳回一數值之雙曲線正弦值.",
		arguments: [
			{
				name: "number",
				description: "為任意實數。"
			}
		]
	},
	{
		name: "SKEW",
		description: "傳回一個分配的偏態。所謂偏態是指一個分配的對稱情形.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個引數。可為數值，或是內容為數值的陣列、名稱、儲存格參照，用以求算偏態。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個引數。可為數值，或是內容為數值的陣列、名稱、儲存格參照，用以求算偏態。"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "傳回以某個母體為基礎的分配偏態，所謂偏態是指一個分配的不對稱情形.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 254 個數字或名稱、陣列、或是內容為數值的參照，用以求算偏態"
			},
			{
				name: "number2",
				description: "為 1 到 254 個數字或名稱、陣列、或是內容為數值的參照，用以求算偏態"
			}
		]
	},
	{
		name: "SLN",
		description: "傳回某項固定資產按直線折舊法所計算的每期折舊金額.",
		arguments: [
			{
				name: "cost",
				description: "為固定資產的原始成本"
			},
			{
				name: "salvage",
				description: "為固定資產耐用年限終了時之估計殘值"
			},
			{
				name: "life",
				description: "為固定資產可使用估計期限 (有時稱為資產使用年限)。"
			}
		]
	},
	{
		name: "SLOPE",
		description: "傳回直線迴歸線的斜率.",
		arguments: [
			{
				name: "known_y's",
				description: "因變數所在的陣列或儲存格範圍。此範圍可以是數字，或內含數值的名稱、陣列、儲存格參照"
			},
			{
				name: "known_x's",
				description: "自變數所在的陣列或儲存格範圍。此範圍可以是數字，或內含數值的名稱、陣列、儲存格參照。"
			}
		]
	},
	{
		name: "SMALL",
		description: "傳回資料組中第 k 小的值。例如第五小的數字.",
		arguments: [
			{
				name: "array",
				description: "為所要找出第 k 小值的數字陣列或數值資料範圍"
			},
			{
				name: "k",
				description: "為在陣列或資料範圍中所欲傳回的位置 (從最小起算)。"
			}
		]
	},
	{
		name: "SQRT",
		description: "傳回數字的正平方根.",
		arguments: [
			{
				name: "number",
				description: "為欲求算其平方根的數字。"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "傳回 (number * Pi) 的平方根.",
		arguments: [
			{
				name: "number",
				description: "是 p 要相乘的數字"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "依據平均值及標準差，將數值標準化後傳回.",
		arguments: [
			{
				name: "x",
				description: "為所要標準化的值"
			},
			{
				name: "mean",
				description: "為分配的算術平均數"
			},
			{
				name: "standard_dev",
				description: "為分配的標準差，此值需為正值。"
			}
		]
	},
	{
		name: "STDEV",
		description: "根據樣本，標準差估計值 (樣本中的邏輯值、文字將忽略不計).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個自母體抽樣出來的樣本值，可為數值或含有數值的參照。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個自母體抽樣出來的樣本值，可為數值或含有數值的參照。"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "標準差計算值，根據提供作為引數的整個母體 (輸入值中的邏輯值或文字將忽略不計).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個自母體抽樣出來的樣本值，可為數值或含有數值的參照。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個自母體抽樣出來的樣本值，可為數值或含有數值的參照。"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "根據樣本的標準差估計值 (若傳入的樣本資料中含有邏輯值、文字等，則這些資料將忽略不計).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個自母體抽樣出來的樣本值，可為數值或含有數值的參照。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個自母體抽樣出來的樣本值，可為數值或含有數值的參照。"
			}
		]
	},
	{
		name: "STDEVA",
		description: "根據樣本，傳回標準差估計值。引數可包含邏輯值及文字。其內容若為文字及 FALSE 則被視為 0; TRUE 視為 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "為 1 到 255 個母體樣本資料，可為數值，內容為數值的陣列、名稱、或是數值儲存格的參照。"
			},
			{
				name: "value2",
				description: "為 1 到 255 個母體樣本資料，可為數值，內容為數值的陣列、名稱、或是數值儲存格的參照。"
			}
		]
	},
	{
		name: "STDEVP",
		description: "標準差計算值，根據提供作為引數的整個母體 (邏輯值或文字將忽略不計).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個自母體抽樣出來的樣本值，可為數值或含有數值的參照。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個自母體抽樣出來的樣本值，可為數值或含有數值的參照。"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "根據整個母體，傳回該母體的標準差。引數可包含邏輯值及文字。其內容若為文字及 FALSE 則被視為 0; TRUE 視為 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "為 1 到 255 個母體資料，可為數值，內容為數值的陣列、名稱、或是數值儲存格的參照。"
			},
			{
				name: "value2",
				description: "為 1 到 255 個母體資料，可為數值，內容為數值的陣列、名稱、或是數值儲存格的參照。"
			}
		]
	},
	{
		name: "STEYX",
		description: "依據迴歸線及傳入的 x 值，來算計出 預估的 y 值，傳回 y 值的標準差.",
		arguments: [
			{
				name: "known_y's",
				description: "因變數所在的陣列或儲存格範圍。此範圍可以是數值，或內含數值的名稱、陣列、儲存格參照"
			},
			{
				name: "known_x's",
				description: "自變數所在的陣列或儲存格範圍。此範圍可以是數值，或內含數值的名稱、陣列、儲存格參照。"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "將字串中的部分字串以新字串取代。.",
		arguments: [
			{
				name: "text",
				description: "為文字串或是參考到一文字的儲存格參照，此字串中的一部分文字將以指定字串取代。"
			},
			{
				name: "old_text",
				description: "為要被取代的文字。如果在 Old_text 中找不到此文字，則 SUBSTITUTE 將不會執行取代作業。"
			},
			{
				name: "new_text",
				description: "為將取代 old_text 的新字串。"
			},
			{
				name: "instance_num",
				description: "當文字串中含有數組 old_text 時，此引數係用以指定所要被取代的字串是文字串中的第幾組。如果省略此值，每一組皆會被取代。"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "傳回清單或資料庫內之小計.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "function_num",
				description: "為 1 到 11 的數字，藉以指定小計函數摘要."
			},
			{
				name: "ref1",
				description: "為要計算小計的 1 到 254 個的範圍或參照。"
			}
		]
	},
	{
		name: "SUM",
		description: "傳回儲存格範圍中所有數值的總和.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個所要加總的數值。在所要加總的儲存格中邏輯值及文字將略過不計，而所要加總的引數如有邏輯值及文字亦略過不計。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個所要加總的數值。在所要加總的儲存格中邏輯值及文字將略過不計，而所要加總的引數如有邏輯值及文字亦略過不計。"
			}
		]
	},
	{
		name: "SUMIF",
		description: "計算所有符合指定條件的儲存格的總和.",
		arguments: [
			{
				name: "range",
				description: "所要加總的範圍"
			},
			{
				name: "criteria",
				description: "為要所要加總儲存格的篩選條件 (準則)，可以是數值、表示式或文字串，用來定義那些儲存格要被加總"
			},
			{
				name: "sum_range",
				description: "為將被加總的儲存格。如果省略，則將使用目前範圍內的儲存格。"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "新增由特定條件或準則集所指定的儲存格.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sum_range",
				description: "是要總和的實際儲存格。"
			},
			{
				name: "criteria_range",
				description: "是要以特定條件評估的儲存格範圍"
			},
			{
				name: "criteria",
				description: "是以數字、運算式或文字為形式的條件或準則，這會定義要新增哪些儲存格"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "傳回多個陣列或範圍中的各相對應元素乘積之總和.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "array1",
				description: "為 2 到 255 個陣列，用以求其乘積後再加總這些乘積。所有陣列的大小必須相同。"
			},
			{
				name: "array2",
				description: "為 2 到 255 個陣列，用以求其乘積後再加總這些乘積。所有陣列的大小必須相同。"
			},
			{
				name: "array3",
				description: "為 2 到 255 個陣列，用以求其乘積後再加總這些乘積。所有陣列的大小必須相同。"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "傳回所有引數的平方和。這些引數可以是數字、陣列、名稱，或是包含數字的儲存格參照.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個數字、陣列、名稱或是用以求算平方和的陣列參照"
			},
			{
				name: "number2",
				description: "為 1 到 255 個數字、陣列、名稱或是用以求算平方和的陣列參照"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "將兩個範圍或陣列中的各對應數值相減後再全部加總.",
		arguments: [
			{
				name: "array_x",
				description: "為第一個範圍或陣列值，可以是數字，或是含有數字的名稱、陣列或參照位址"
			},
			{
				name: "array_y",
				description: "為第二個陣列或範圍值，可以是數字，或是含有數字的名稱、陣列或參照位址。"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "傳回兩個陣列中各元素平方和的總和.",
		arguments: [
			{
				name: "array_x",
				description: "為第一個陣列或範圍值，可以是數字，或是含有數字的名稱、陣列或參照位址"
			},
			{
				name: "array_y",
				description: "為第二個陣列或範圍值，可以是數字，或是含有數字的名稱、陣列或參照位址。"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "將兩個範圍或陣列中的各對應數值相減後求平方值再全部加總.",
		arguments: [
			{
				name: "array_x",
				description: "為第一個範圍或陣列值，可以是數字，或是含有數字的名稱、陣列或參照位址"
			},
			{
				name: "array_y",
				description: "為第二個範圍或陣列值，可以是數字，或是含有數字的名稱、陣列或參照位址。"
			}
		]
	},
	{
		name: "SYD",
		description: "按年數合計法計算，傳回某固定資產在某一指定期間的折舊金額.",
		arguments: [
			{
				name: "cost",
				description: "固定資產的原始成本"
			},
			{
				name: "salvage",
				description: "為固定資產耐用年限終了時之估計殘值"
			},
			{
				name: "life",
				description: "為固定資產可使用估計期限 (有時稱為資產使用年限)"
			},
			{
				name: "per",
				description: "為所要計算折舊金額的期數，其採用的衡量單位必須與 Life 引數相同。"
			}
		]
	},
	{
		name: "T",
		description: "檢查某值是否為文字，若為文字則傳回文字，若不是文字則傳回空字串 (空白文字)。.",
		arguments: [
			{
				name: "value",
				description: "為所要測試的值。"
			}
		]
	},
	{
		name: "T.DIST",
		description: "傳回左尾 Student's 式 T 分配值.",
		arguments: [
			{
				name: "x",
				description: "為要用來評估分配的數值"
			},
			{
				name: "deg_freedom",
				description: "為一正整數，表示分配的自由度"
			},
			{
				name: "cumulative",
				description: "為一邏輯值; 當為 TRUE 時，採用累加分配函數; 為 FALSE 時，採用機率密度函數。"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "傳回雙尾 Student's 式 T 分配值.",
		arguments: [
			{
				name: "x",
				description: "為要用來評估分配的數值"
			},
			{
				name: "deg_freedom",
				description: "為一正整數，表示分配的自由度。"
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "傳回右尾 Student's 式 T 分配值.",
		arguments: [
			{
				name: "x",
				description: "為要用來評估分配的數值"
			},
			{
				name: "deg_freedom",
				description: "為一正整數，表示分配的自由度。"
			}
		]
	},
	{
		name: "T.INV",
		description: "傳回 Student's 式 T 分配的左尾反值.",
		arguments: [
			{
				name: "probability",
				description: "為雙尾 Student's 式 T 分配之相關機率值，介於 0 和 1 之間且包含 0 和 1"
			},
			{
				name: "deg_freedom",
				description: "為一正整數，表示分配的自由度。"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "傳回 Student's 式 T 分配的雙尾反值.",
		arguments: [
			{
				name: "probability",
				description: "為雙尾 Student's T 分配之相關機率值，介於 0 和 1 之間且包含 0 和 1"
			},
			{
				name: "deg_freedom",
				description: "為一正整數，表示分配的自由度。"
			}
		]
	},
	{
		name: "T.TEST",
		description: "傳回有關 Student's 式 T 檢定之機率值.",
		arguments: [
			{
				name: "array1",
				description: "為第一資料組"
			},
			{
				name: "array2",
				description: "為第二資料組"
			},
			{
				name: "tails",
				description: "指定分配尾巴的特性: 當為 1 時，代表單尾分配; 當為 2 時，代表雙尾分配"
			},
			{
				name: "type",
				description: "所要執行之 T 檢定種類: 當為 1 時，將執行成偶檢定; 當為 2 時，代表兩樣本具有同一變異數; 當為 3 時，代表兩樣本不具有同一變異數。"
			}
		]
	},
	{
		name: "TAN",
		description: "傳回一角度之正切值.",
		arguments: [
			{
				name: "number",
				description: "為欲求算其正切值的角度，以弧度表示。度數 * PI()/180  = 弧度。"
			}
		]
	},
	{
		name: "TANH",
		description: "傳回一數值之雙曲線正切值.",
		arguments: [
			{
				name: "number",
				description: "為任意實數。"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "傳回國庫券的債券約當收益率.",
		arguments: [
			{
				name: "settlement",
				description: "是國庫券的結帳日期，以數列日期數字表示"
			},
			{
				name: "maturity",
				description: "是國庫券的到期日期，以數列日期數字表示"
			},
			{
				name: "discount",
				description: "是國庫券的貼現率"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "傳回國庫卷每 $100 面額的價格.",
		arguments: [
			{
				name: "settlement",
				description: "是國庫券的結帳日期，以數列日期數字表示"
			},
			{
				name: "maturity",
				description: "是國庫券的到期日期，以數列日期數字表示"
			},
			{
				name: "discount",
				description: "是國庫券的貼現率"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "傳回國庫卷的收益.",
		arguments: [
			{
				name: "settlement",
				description: "是國庫卷的結帳日期，以數列日期數字表示"
			},
			{
				name: "maturity",
				description: "是國庫券的到期日期，以數列日期數字表示"
			},
			{
				name: "pr",
				description: "是每 $100 面額的國庫券價格"
			}
		]
	},
	{
		name: "TDIST",
		description: "傳回 Student's 式之 T 分配.",
		arguments: [
			{
				name: "x",
				description: "是要用來評估分配的數值"
			},
			{
				name: "deg_freedom",
				description: "是用來指出分配自由度的整數值"
			},
			{
				name: "tails",
				description: "指定要傳回的分配尾數的數字: 1 表示傳回單尾分配; 2 表示傳回雙尾分配。"
			}
		]
	},
	{
		name: "TEXT",
		description: "依指定的數值格式，將數字轉成文字.",
		arguments: [
			{
				name: "value",
				description: "為一數字、一個傳回數值的公式、或是數值儲存格的參照"
			},
			{
				name: "format_text",
				description: "為數值顯示格式。您可在 [儲存格格式] 對話方塊中的 [數字] 索引標籤這一頁上找到許多數值格式 (非 [G/通用] 格式)。"
			}
		]
	},
	{
		name: "TIME",
		description: "將代表小時、分、秒的給定數字轉換成 Spreadsheet 時間格式序列值。.",
		arguments: [
			{
				name: "hour",
				description: "為介於 0 到 23 之間的數字，代表小時"
			},
			{
				name: "minute",
				description: "為介於 0 到 59 之間的數字，代表分"
			},
			{
				name: "second",
				description: "為介於 0 到 59 之間的數字，代表秒。"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "將文字時間轉換成 Spreadsheet 時間序列值，亦即從 0 (12:00:00 A.M.) 到 1 (11:59:59 P.M.) 的數字。輸入公式後將數字的格式轉成時間格式.",
		arguments: [
			{
				name: "time_text",
				description: "為依 Spreadsheet 時間格式表達的文字串 (字串中的日期資訊將被忽略)。"
			}
		]
	},
	{
		name: "TINV",
		description: "傳回 Student's 式 T 分配的雙尾反值.",
		arguments: [
			{
				name: "probability",
				description: "為雙尾 Student's 式 T 分配之相關機率值，介於 0 和 1 之間且包含 0 和 1"
			},
			{
				name: "deg_freedom",
				description: "為一正整數，表示分配的自由度。"
			}
		]
	},
	{
		name: "TODAY",
		description: "傳回格式為日期的目前日期。.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "將垂直的儲存格範圍轉為水平範圍或反向操作。.",
		arguments: [
			{
				name: "array",
				description: "是工作表或值之陣列中您所要轉置的儲存格範圍。"
			}
		]
	},
	{
		name: "TREND",
		description: "傳回依據趨勢預測線所求出的值，此趨勢線是用已知資料點以最小平方法求出.",
		arguments: [
			{
				name: "known_y's",
				description: "是一組符合 y = mx + b 運算關係的已知 y 值範圍或陣列"
			},
			{
				name: "known_x's",
				description: "是一組符合 y = mx +b 運算關係的已知 x 值範圍或陣列，此為非必要引數，且陣列大小與 Known_y's 相同"
			},
			{
				name: "new_x's",
				description: "是一組要 TREND 求出對應 y 值的新 x 值範圍或陣列"
			},
			{
				name: "const",
				description: "為邏輯值: Const = TRUE 或省略，常數項 b 將依計算而得; Const = FALSE 則常數項 b 將被設定為 0。"
			}
		]
	},
	{
		name: "TRIM",
		description: "刪除文字字串中多餘的空格 (字與字之間所保留的單一空白將不會被刪除)。.",
		arguments: [
			{
				name: "text",
				description: "為要刪除多餘空白的文字資料。"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "傳回截去某一百分比之外的極端值後，所求得的平均數.",
		arguments: [
			{
				name: "array",
				description: "為所欲消除極端值並求算平均值之資料所在陣列或範圍"
			},
			{
				name: "percent",
				description: "為資料點的百分比值，此百分比所在的資料點將視為極端值而被排除。"
			}
		]
	},
	{
		name: "TRUE",
		description: "傳回邏輯值 TRUE。.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "將一數字的小數、分數部分捨去.",
		arguments: [
			{
				name: "number",
				description: "為所要捨去位數的數字"
			},
			{
				name: "num_digits",
				description: "是對數值執行捨去計算時所採的精確位數。"
			}
		]
	},
	{
		name: "TTEST",
		description: "傳回有關 Student's 式 T 檢定之機率值.",
		arguments: [
			{
				name: "array1",
				description: "為第一資料組"
			},
			{
				name: "array2",
				description: "為第二資料組"
			},
			{
				name: "tails",
				description: "指定分配尾巴的特性: 當為 1 時，代表單尾分配; 當為 2 時，代表雙尾分配"
			},
			{
				name: "type",
				description: "所要執行之 T 檢定種類: 當為 1 時，將執行成偶檢定; 當為 2 時，代表兩樣本具有同一變異數 (同質性); 當為 3 時，代表兩樣本不具有同一變異數。"
			}
		]
	},
	{
		name: "TYPE",
		description: "值的資料型態以整數表示: 值 = 1 代表數字; 2 代表文字; 4 代表邏輯值; 16 代表錯誤值; 64 代表陣列。.",
		arguments: [
			{
				name: "value",
				description: "可為任意值。"
			}
		]
	},
	{
		name: "UNICODE",
		description: "傳回對應到文字第一個字元的數字 (字碼指標).",
		arguments: [
			{
				name: "text",
				description: "為要求算 Unicode 值的字元"
			}
		]
	},
	{
		name: "UPPER",
		description: "將字串轉成大寫。.",
		arguments: [
			{
				name: "text",
				description: "為要轉換大寫的文字資料、儲存格參照。"
			}
		]
	},
	{
		name: "VALUE",
		description: "將文字資料轉換成數字資料.",
		arguments: [
			{
				name: "text",
				description: "為所要轉換的文字資料，它可以是一個左右以引號括起的文字，或是儲存格參照。"
			}
		]
	},
	{
		name: "VAR",
		description: "根據樣本，標準差估計值 (樣本中的邏輯值、文字將忽略不計).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個自母體抽樣出來的數值引數。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個自母體抽樣出來的數值引數。"
			}
		]
	},
	{
		name: "VAR.P",
		description: "根據整個母體，計算變異數 (輸入母體中的邏輯值及文字將忽略不計).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個數值引數，用以代表母體。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個數值引數，用以代表母體。"
			}
		]
	},
	{
		name: "VAR.S",
		description: "根據樣本來估計變異數 (輸入樣本值中的邏輯值及文字將忽略不計).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個數值引數，用以代表母體樣本。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個數值引數，用以代表母體樣本。"
			}
		]
	},
	{
		name: "VARA",
		description: "根據抽樣樣本，傳回變異數估計值。引數可包含邏輯值及文字。若為文字及 FALSE 則被視為 0; TRUE 視為 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "為 1 到 255 個母體樣本資料。"
			},
			{
				name: "value2",
				description: "為 1 到 255 個母體樣本資料。"
			}
		]
	},
	{
		name: "VARP",
		description: "根據整個母體，計算變異數 (母體中的邏輯值及文字將忽略不計).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "為 1 到 255 個數值引數，用以代表母體。"
			},
			{
				name: "number2",
				description: "為 1 到 255 個數值引數，用以代表母體。"
			}
		]
	},
	{
		name: "VARPA",
		description: "根據整個母體，傳回變異數。引數可包含邏輯值及文字。其內容若為文字及 FALSE 則被視為 0; TRUE 視為 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "為 1 到 255 個母體資料。"
			},
			{
				name: "value2",
				description: "為 1 到 255 個母體資料。"
			}
		]
	},
	{
		name: "VDB",
		description: "傳回某項固定資產某個時段間 (包括分期付款) 的折舊數總額，折舊係按倍率遞減法或其他您所指定的遞減速率計算.",
		arguments: [
			{
				name: "cost",
				description: "為固定資產的原始成本"
			},
			{
				name: "salvage",
				description: "為資產耐用年限終了時之價值"
			},
			{
				name: "life",
				description: "為資產可折舊之年數 (有時稱為資產的耐用年限)"
			},
			{
				name: "start_period",
				description: "指定折舊數額的計算是從第幾期，必須與 Life 引數採用相同的衡量單位"
			},
			{
				name: "end_period",
				description: "指定折舊數額的計算是要算到第幾期，必須與 Life 引數採用相同的衡量單位"
			},
			{
				name: "factor",
				description: "用以指定餘額遞減的速率，若省略則為 2 (即採用倍率遞減法)"
			},
			{
				name: "no_switch",
				description: "若為 FALSE 或被省略，則當直線法折舊數額大於遞減餘額法算出的結果時便切換成直線法的折舊數額; 若為 TRUE 則不切換。"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "在一表格的最左欄中尋找含有某特定值的欄位，再傳回同一列中某一指定欄中的值。預設情況下表格必須以遞增順序排序。.",
		arguments: [
			{
				name: "lookup_value",
				description: "是您打算在表格的最左欄中搜尋的值，可以是數值、參照位址或文字串。"
			},
			{
				name: "table_array",
				description: "是要在其中搜尋資料的文字、數字或邏輯值的表格。Table_array 可為儲存格範圍的參照位址或範圍名稱"
			},
			{
				name: "col_index_num",
				description: "是個數值，代表所要傳回的值位於 table_array 中的第幾欄。引數值為 1 代表表格中首欄的值。"
			},
			{
				name: "range_lookup",
				description: "為邏輯值: TRUE 或省略表示找出首欄中最接近的值 (以遞增順序排序); FALSE 表示僅尋找完全符合的數值。"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "傳回介於 1 到 7 的整數，用以識別星期數值。.",
		arguments: [
			{
				name: "serial_number",
				description: "為代表日期的數字。"
			},
			{
				name: "return_type",
				description: "為一數字: 使用 1，代表星期日 = 1 到星期六 = 7; 使用 2，代表星期一 = 1 到星期日 = 7;使用 3，代表星期一 = 0 到星期日 = 6。"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "傳回這一年的週數目.",
		arguments: [
			{
				name: "serial_number",
				description: "是 Spreadsheet 用於日期和時間計算的日期-時間程式碼"
			},
			{
				name: "return_type",
				description: "是判斷傳回值類型的數字 (1 或 2)"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "傳回 Weibull 分配.",
		arguments: [
			{
				name: "x",
				description: "是要評估此函數的數值，為非負數值"
			},
			{
				name: "alpha",
				description: "是該分配的參數值之一，為一正數"
			},
			{
				name: "beta",
				description: "是該分配的一個參數值，為一正數"
			},
			{
				name: "cumulative",
				description: "為一邏輯值: TRUE 時採用累加分配函數; FALSE 則採用機率質量函數。"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "傳回 Weibull 分配.",
		arguments: [
			{
				name: "x",
				description: "是要評估此函數的數值，為非負數值"
			},
			{
				name: "alpha",
				description: "是該分配的參數值之一，為一正數"
			},
			{
				name: "beta",
				description: "是該分配的一個參數值，為一正數"
			},
			{
				name: "cumulative",
				description: "為一邏輯值: TRUE 時採用累加分配函數; FALSE 則採用機率質量函數。"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "傳回指定的工作日數目之前或之後，日期的數列數字.",
		arguments: [
			{
				name: "start_date",
				description: "是代表開始日期的數列日期數字"
			},
			{
				name: "days",
				description: "是在 start_date 之前或之後的非週末和非假日的數目"
			},
			{
				name: "holidays",
				description: "是一或多個數列日期數字的選用陣列，會從工作行事曆中排除，例如，州假日和聯邦假日以及彈性假日"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "傳回指定的工作日數目之前或之後，日期的數列數字 (含自訂週末參數).",
		arguments: [
			{
				name: "start_date",
				description: "是代表開始日期的數列日期數字"
			},
			{
				name: "days",
				description: "是在 start_date 之前或之後的非週末和非假日的數目"
			},
			{
				name: "weekend",
				description: "為指定何時為週末的數字或字串"
			},
			{
				name: "holidays",
				description: "是一或多個數列日期數字的選用陣列，會從工作行事曆中排除，例如，州假日和聯邦假日以及彈性假日"
			}
		]
	},
	{
		name: "XIRR",
		description: "傳回現金流時程的內部收益率.",
		arguments: [
			{
				name: "values",
				description: "是對應到日期中付款時程的現金流數列"
			},
			{
				name: "dates",
				description: "是對應到現金流付款的付款日期排程"
			},
			{
				name: "guess",
				description: "是您猜測最接近 XIRR 結果的數字"
			}
		]
	},
	{
		name: "XNPV",
		description: "傳回現金流時程的淨現值.",
		arguments: [
			{
				name: "rate",
				description: "是套用到現金流的貼現率"
			},
			{
				name: "values",
				description: "是對應到付款時程 (以日計算) 的現金流數列"
			},
			{
				name: "dates",
				description: "是對應到現金流付款的付款日期排程"
			}
		]
	},
	{
		name: "XOR",
		description: "傳回所有引數的邏輯 'Exclusive Or'.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "為要測試的條件，有 1 到 254 個，其結果為 TRUE 或 FALSE，可能為邏輯值、陣列或參照"
			},
			{
				name: "logical2",
				description: "為要測試的條件，有 1 到 254 個，其結果為 TRUE 或 FALSE，可能為邏輯值、陣列或參照"
			}
		]
	},
	{
		name: "YEAR",
		description: "傳回日期的年份部分，為介於 1900 到 9999 之間的整數。.",
		arguments: [
			{
				name: "serial_number",
				description: "係指 Spreadsheet 所使用的日期和時間碼的數字。"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "傳回代表在 start_date 和 end_date 之間所有日期數字的年份分數.",
		arguments: [
			{
				name: "start_date",
				description: "是代表開始日期的數列日期數字"
			},
			{
				name: "end_date",
				description: "是代表結束日期的數列日期數字"
			},
			{
				name: "basis",
				description: "是要使用的日計數基礎類型"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "傳回貼現證券的年收益。例如，國庫債券.",
		arguments: [
			{
				name: "settlement",
				description: "是證券的結帳日期，以數列日期數字表示"
			},
			{
				name: "maturity",
				description: "是證券的到期日期，以數列日期數字表示"
			},
			{
				name: "pr",
				description: "是每 $100 面額證券的價格"
			},
			{
				name: "redemption",
				description: "是每 $100 面額證券的贖回價值"
			},
			{
				name: "basis",
				description: "是要使用的日計數基礎類型"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "傳回 Z 檢定的單尾 P 值.",
		arguments: [
			{
				name: "array",
				description: "為相對欲檢定 X 之陣列或資料範圍"
			},
			{
				name: "x",
				description: "為欲檢定之數值"
			},
			{
				name: "sigma",
				description: "為母體 (已知) 之標準差。若省略則使用樣本標準差。"
			}
		]
	},
	{
		name: "ZTEST",
		description: "傳回 Z 檢定的單尾 P 值.",
		arguments: [
			{
				name: "array",
				description: "為相對欲檢定 X 之陣列或資料範圍"
			},
			{
				name: "x",
				description: "為欲檢定之數值"
			},
			{
				name: "sigma",
				description: "為母體 (已知) 之標準差。若省略則使用樣本標準差。"
			}
		]
	}
];