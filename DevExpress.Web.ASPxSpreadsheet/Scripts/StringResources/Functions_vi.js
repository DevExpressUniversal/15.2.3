ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Trả về giá trị tuyệt đối một số, giá trị số không dấu.",
		arguments: [
			{
				name: "number",
				description: "là phần số thực cần lấy giá trị tuyệt đối"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "Trả lại lợi nhuận cộng dồn cho thế chấp trả lợi nhuận khi tới hạn.",
		arguments: [
			{
				name: "issue",
				description: "là ngày phát hành thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "settlement",
				description: "là ngày tới hạn thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "rate",
				description: "là tỷ suất cu-pôn hàng năm của thế chấp"
			},
			{
				name: "par",
				description: "là giá trị par của thế chấp"
			},
			{
				name: "basis",
				description: "là loại cơ sở tính ngày được dùng"
			}
		]
	},
	{
		name: "ACOS",
		description: "Trả về ArcCosin của một số, theo radian trong khoảng từ 0 đến Pi. ArcCosin là góc có Cosin bằng Số.",
		arguments: [
			{
				name: "number",
				description: "là Cosin của góc mong muốn và phải từ -1 đến 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Trả về Cosin hi-péc-bôn đảo của một số.",
		arguments: [
			{
				name: "number",
				description: "là số thực bất kỳ lớn hơn hoặc bằng 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Trả về hàm arccotang của con số, theo đơn vị đo góc từ 0 đến Pi.",
		arguments: [
			{
				name: "number",
				description: "là hàm cotang của góc bạn muốn"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Trả về hàm cotang hyperbol ngược của con số.",
		arguments: [
			{
				name: "number",
				description: "là hàm cotang hyperbol của góc bạn muốn"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "Tạo tham chiếu ô dạng văn bản, có số hàng và cột xác định.",
		arguments: [
			{
				name: "row_num",
				description: "là số hiệu hàng trong tham chiếu ô: Số_hiệu_hàng = 1 cho hàng 1"
			},
			{
				name: "column_num",
				description: "là số hiệu cột trong tham chiếu ô.Ví dụ, Số_hiệu_cột = 1 cho cột D"
			},
			{
				name: "abs_num",
				description: "chỉ định loại tham chiếu: tuyệt đối nếu = 1; hàng tuyệt đối/cột tương đối nếu = 2; hàng tương đối/cột tuyệt đối nếu = 3; tương đối nếu = 4"
			},
			{
				name: "a1",
				description: "là giá trị lô-gic thể hiện kiểu tham chiếu: kiểu A1 = 1 hoặc ĐÚNG; kiểu R1C1 = 0 hoặc SAI"
			},
			{
				name: "sheet_text",
				description: "là văn bản chỉ định tên của trang tính sử dụng như tham chiếu ngoài"
			}
		]
	},
	{
		name: "AND",
		description: "Kiểm tra nếu tất cả các tham đối là TRUE, trả về giá trị TRUE nếu tất cả tham đối là TRUE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "từ 1 đến 255 điều kiện cần kiểm tra, có thể là ĐÚNG hoặc SAI, có thể là giá trị lô-gic, mảng, hoặc tham chiếu"
			},
			{
				name: "logical2",
				description: "từ 1 đến 255 điều kiện cần kiểm tra, có thể là ĐÚNG hoặc SAI, có thể là giá trị lô-gic, mảng, hoặc tham chiếu"
			}
		]
	},
	{
		name: "ARABIC",
		description: "Chuyển đổi chữ số La Mã sang chữ số Ả Rập.",
		arguments: [
			{
				name: "text",
				description: "là chữ số La Mã bạn muốn chuyển đổi"
			}
		]
	},
	{
		name: "AREAS",
		description: "Trả về số vùng trong một tham chiếu. Vùng là một khoảng các ô liên tục hoặc một ô đơn.",
		arguments: [
			{
				name: "reference",
				description: "là một tham chiếu tới ô hoặc một khoảng các ô và có thể tham chiếu tới nhiều vùng"
			}
		]
	},
	{
		name: "ASIN",
		description: "Trả về ArcSin của một số theo radian, trong khoảng từ -Pi/2 đến Pi/2.",
		arguments: [
			{
				name: "number",
				description: "là Sin của góc mong muốn và phải từ -1 đến 1"
			}
		]
	},
	{
		name: "ASINH",
		description: "Trả về Sin hi-péc-bôn đảo của một số.",
		arguments: [
			{
				name: "number",
				description: "là số thực bất kỳ lớn hơn hoặc bằng 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Trở về ArcTang của một số theo Radian, trong khoảng từ -Pi/2 đến Pi/2.",
		arguments: [
			{
				name: "number",
				description: "là Tang của góc mong muốn"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Trả về ArcTang của cặp toạ độ x và y, theo radian trong khoảng từ -Pi đến Pi, không tính -Pi.",
		arguments: [
			{
				name: "x_num",
				description: "là toạ độ x của điểm"
			},
			{
				name: "y_num",
				description: "là toạ độ y của điểm"
			}
		]
	},
	{
		name: "ATANH",
		description: "Trả về Tang hi-péc-bôn đảo của một số.",
		arguments: [
			{
				name: "number",
				description: "là số thực bất kỳ trong khoảng từ -1 đến 1 ngoại trừ -1 và 1"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "Trả về trung bình độ lệch tuyệt đối giữa các điểm dữ liệu với giá trị trung bình của chúng. Tham đối có thể là số hoặc tên, mảng, tham chiếu chứa số.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 tham đối cần tính trung bình độ lệch tuyệt đối"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 tham đối cần tính trung bình độ lệch tuyệt đối"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "Trả về giá trị trung bình (trung bình số học) của các tham đối, chúng có thể là các số, tên, mảng hoặc các tham chiếu có chứa số.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 tham đối dạng số bạn cần tính trung bình"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 tham đối dạng số bạn cần tính trung bình"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "Trả về trung bình (trung bình số học) các tham đối của nó, văn bản và SAI được coi là 0; ĐÚNG được coi là 1. Tham đối có thể là số, tên, mảng, hoặc tham chiếu.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "từ 1 đến 255 tham đối cần tính trung bình"
			},
			{
				name: "value2",
				description: "từ 1 đến 255 tham đối cần tính trung bình"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "Tìm trung bình(trung bình số học) cho ô được chỉ ra bởi điều kiện hay chỉ tiêu cho trước.",
		arguments: [
			{
				name: "range",
				description: "là dải các ô muốn đánh giá theo điều kiện cụ thể"
			},
			{
				name: "criteria",
				description: "là điều kiện ở dạng số, biểu thức, hay văn bản xác định ô nào sẽ được dùng để tìm trung bình"
			},
			{
				name: "average_range",
				description: "là các ô thực sự sẽ được dùng để tìm trung bình. Nếu bỏ qua thì các ô trong dải sẽ được dùng "
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "Tìm trung bình(trung bình số học) cho các ô được chỉ ra bởi bởi tập cho trước các điều kiện hay chỉ tiêu.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "average_range",
				description: "là các ô thực sự sẽ được dùng để tìm trung bình"
			},
			{
				name: "criteria_range",
				description: "là dải các ô muốn đánh giá theo điều kiện cụ thể"
			},
			{
				name: "criteria",
				description: "là điều kiện ở dạng số, biểu thức, hay văn bản xác định ô nào sẽ được dùng để tìm trung bình"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Chuyển đổi số sang văn bản (bạt).",
		arguments: [
			{
				name: "number",
				description: "là số cần chuyển đổi"
			}
		]
	},
	{
		name: "BASE",
		description: "Chuyển một con số thành biểu diễn bằng văn bản với cơ số (gốc) cho trước.",
		arguments: [
			{
				name: "number",
				description: "là con số bạn muốn chuyển"
			},
			{
				name: "radix",
				description: "là Cơ số gốc bạn muốn chuyển con số về"
			},
			{
				name: "min_length",
				description: "là chiều dài tối thiểu của xâu trả về. Nếu bỏ qua thì các số 0 ở đâu sẽ không được thêm vào"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Trả về hàm Bessel biến đổi In(x).",
		arguments: [
			{
				name: "x",
				description: "là giá trị cần tính toán hàm"
			},
			{
				name: "n",
				description: "là thứ tự hàm Bessel"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Trả về hàm Bessel Jn(x).",
		arguments: [
			{
				name: "x",
				description: "là giá trị cần tính toán hàm"
			},
			{
				name: "n",
				description: "ilà thứ tự hàm Bessel"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Trả về hàm Bessel biến đổi Kn(x).",
		arguments: [
			{
				name: "x",
				description: "là giá trị cần tính toán hàm"
			},
			{
				name: "n",
				description: "là thứ tự hàm Bessel"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Trả về hàm Bessel Yn(x).",
		arguments: [
			{
				name: "x",
				description: "là giá trị cần tính toán hàm"
			},
			{
				name: "n",
				description: "là thứ tự hàm"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "Trả về hàm phân phối xác suất beta.",
		arguments: [
			{
				name: "x",
				description: "là giá trị từ A đến B để đánh giá hàm"
			},
			{
				name: "alpha",
				description: "là tham số cho phân phối và phải lớn hơn 0"
			},
			{
				name: "beta",
				description: "là tham số cho phân phối và phải lớn hơn 0"
			},
			{
				name: "cumulative",
				description: "là giá trị lôgic: đối với hàm phân phối tích lũy, sử dụng TRUE; đối với hàm mật độ xác suất, sử dụng FALSE"
			},
			{
				name: "A",
				description: "là giới hạn dưới tùy chọn cho khoảng x. Nếu bị bỏ qua, A = 0"
			},
			{
				name: "B",
				description: "là giới hạn trên tùy chọn cho khoảng x. Nếu bị bỏ qua, B = 1"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Trả về nghịch đảo của hàm mật độ phân phối beta tích lũy (BETA.DIST).",
		arguments: [
			{
				name: "probability",
				description: "là xác suất liên quan đến phân phối beta"
			},
			{
				name: "alpha",
				description: "là tham số cho phân phối và phải lớn hơn 0"
			},
			{
				name: "beta",
				description: " là tham số cho phân phối và phải lớn hơn 0"
			},
			{
				name: "A",
				description: "là giới hạn dưới tùy chọn cho khoảng x. Nếu bị bỏ qua, A = 0"
			},
			{
				name: "B",
				description: "là giới hạn trên tùy chọn cho khoảng x. Nếu bị bỏ qua, B = 1"
			}
		]
	},
	{
		name: "BETADIST",
		description: "Trả về hàm trù mật xác suất bê-ta lũy tiến.",
		arguments: [
			{
				name: "x",
				description: "là giá trị định trị của hàm trong khoảng giữa A và B"
			},
			{
				name: "alpha",
				description: "là một tham số dùng để phân bố và phải lớn hơn 0"
			},
			{
				name: "beta",
				description: "là một tham số dùng để phân bố và phải lớn hơn 0"
			},
			{
				name: "A",
				description: "là giới hạn biến thiên dưới tùy chọn của x. Nếu không có, A = 0"
			},
			{
				name: "B",
				description: "là giới hạn biến thiên trên tùy chọn của x. Nếu không có, B = 1"
			}
		]
	},
	{
		name: "BETAINV",
		description: "Trả về giá trị đảo của hàm trù mật xác suất bê-ta lũy tiến (BETADIST).",
		arguments: [
			{
				name: "probability",
				description: "là xác suất gắn với phân bố bêta"
			},
			{
				name: "alpha",
				description: "là một tham biến cho phân bố và phải lớn hơn 0"
			},
			{
				name: "beta",
				description: "là một tham biến cho phân bố và phải lớn hơn 0"
			},
			{
				name: "A",
				description: "là giới hạn biến thiên dưới tùy chọn của x. Nếu không có, A = 0"
			},
			{
				name: "B",
				description: "là giới hạn biến thiên trên tùy chọn của x. Nếu không có, B = 1"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "Chuyển số nhị phân thành thập phân.",
		arguments: [
			{
				name: "number",
				description: "là số nhị phân mà bạn muốn chuyển"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "Chuyển số nhị phân thành thập lục phân.",
		arguments: [
			{
				name: "number",
				description: "là số nhị phân mà bạn muốn chuyển"
			},
			{
				name: "places",
				description: "là số ký tự sử dụng"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "Chuyển số nhị phân thành bát phân.",
		arguments: [
			{
				name: "number",
				description: "là số nhị phân mà bạn muốn chuyển"
			},
			{
				name: "places",
				description: "là số ký tự sử dụng"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "Trả về thời hạn riêng của nhị thức phân bố xác suất.",
		arguments: [
			{
				name: "number_s",
				description: "là số lần thành công trong khi thử"
			},
			{
				name: "trials",
				description: "là số phép thử độc lập"
			},
			{
				name: "probability_s",
				description: "là xác suất thành công của mỗi lần thử"
			},
			{
				name: "cumulative",
				description: "là giá trị lô gíc: dành cho hàm phân bố tích lũy, dùng ĐÚNG; hàm xác suất số đông, dùng SAI"
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "Trả về xác suất của kết quả thử sử dụng phân phối nhị thức.",
		arguments: [
			{
				name: "trials",
				description: "là số lần thử độc lập"
			},
			{
				name: "probability_s",
				description: "là xác suất thành công của mỗi lần thử"
			},
			{
				name: "number_s",
				description: "là số lần thành công của mỗi lần thử"
			},
			{
				name: "number_s2",
				description: "nếu được cung cấp, hàm này trả về xác suất mà số lần thử thành công nằm giữa number_s và number_s2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Trả về giá trị nhỏ nhất có nhị thức phân bố tích lũy lớn hơn hoặc bằng giá trị tiêu chuẩn.",
		arguments: [
			{
				name: "trials",
				description: "là số phép thử Bernoulli"
			},
			{
				name: "probability_s",
				description: "là xác suất thành công của mỗi lần thử, một số từ 0 đến 1"
			},
			{
				name: "alpha",
				description: "là giá trị tiêu chuẩn, một số từ 0 đến 1"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "Trả về thời hạn riêng của nhị thức phân bố xác suất.",
		arguments: [
			{
				name: "number_s",
				description: "là số lần thành công trong khi thử"
			},
			{
				name: "trials",
				description: "là số phép thử độc lập"
			},
			{
				name: "probability_s",
				description: "là xác suất thành công của mỗi lần thử"
			},
			{
				name: "cumulative",
				description: "là giá trị lô-gic: dành cho hàm phân bố tích lũy, dùng ĐÚNG; hàm xác suất số đông, dùng SAI"
			}
		]
	},
	{
		name: "BITAND",
		description: "Trả về 'And' theo bit của hai số.",
		arguments: [
			{
				name: "number1",
				description: "là biểu diễn theo hệ thập phân của hai số nhị phân bạn muốn định trị"
			},
			{
				name: "number2",
				description: "là biểu diễn theo hệ thập phân của số nhị phân bạn muốn định trị"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "Trả về một số được dịch trái bởi các bit shift_amount.",
		arguments: [
			{
				name: "number",
				description: "là biểu thị theo hệ thập phân của số nhị phân bạn muốn đánh giá"
			},
			{
				name: "shift_amount",
				description: "là số lượng bit mà bạn muốn dịch Số sang trái"
			}
		]
	},
	{
		name: "BITOR",
		description: "Trả về 'Or' theo bit của hai số.",
		arguments: [
			{
				name: "number1",
				description: "là biểu diễn theo hệ thập phân của hai số nhị phân bạn muốn định trị"
			},
			{
				name: "number2",
				description: "là biểu diễn theo hệ thập phân của số nhị phân bạn muốn định trị"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "Trả về một số được dịch phải bởi các bit shift_amount .",
		arguments: [
			{
				name: "number",
				description: "là biểu thị theo hệ thập phân của số nhị phân bạn muốn đánh giá"
			},
			{
				name: "shift_amount",
				description: "là số lượng bit mà bạn muốn dịch Số sang phải"
			}
		]
	},
	{
		name: "BITXOR",
		description: "Trả về 'Exclusive Or' theo bit của hai số.",
		arguments: [
			{
				name: "number1",
				description: "là biểu diễn theo hệ thập phân của hai số nhị phân bạn muốn định trị"
			},
			{
				name: "number2",
				description: "là biểu diễn theo hệ thập phân của số nhị phân bạn muốn định trị"
			}
		]
	},
	{
		name: "CEILING",
		description: "Làm tròn số lên, tới số nguyên hoặc bội gần nhất của số có nghĩa.",
		arguments: [
			{
				name: "number",
				description: "là giá trị cần làm tròn"
			},
			{
				name: "significance",
				description: "là số có bội được làm tròn tới"
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "Làm tròn số lên tới số nguyên gần nhất hoặc lên bội số có nghĩa gần nhất.",
		arguments: [
			{
				name: "number",
				description: "là giá trị bạn muốn làm tròn"
			},
			{
				name: "significance",
				description: "là bội số bạn muốn làm tròn lên"
			},
			{
				name: "mode",
				description: "nếu được cung cấp và không bằng không thì hàm này sẽ làm tròn về xa con số không"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "Làm tròn số lên, tới số nguyên hoặc bội gần nhất của số có nghĩa.",
		arguments: [
			{
				name: "number",
				description: "là giá trị cần làm tròn"
			},
			{
				name: "significance",
				description: "là số có bội được làm tròn tới"
			}
		]
	},
	{
		name: "CELL",
		description: "trả lại thông tin về dạng thức, vị trí, hoặc nội dung của ô đầu tiên, tùy theo trật tự đọc trang tính, trong tham chiếu.",
		arguments: [
			{
				name: "info_type",
				description: "là giá trị văn bản chỉ định loại thông tin ô mong muốn."
			},
			{
				name: "reference",
				description: "là ô cần thông tin về nó"
			}
		]
	},
	{
		name: "CHAR",
		description: "Trả về ký tự xác định bởi số hiệu mã từ tập ký tự trên máy tính.",
		arguments: [
			{
				name: "number",
				description: "là số từ 1 đến 255 xác định ký tự mong muốn"
			}
		]
	},
	{
		name: "CHIDIST",
		description: "Trả về xác suất một phía của phân bố chi-bình-phương.",
		arguments: [
			{
				name: "x",
				description: "là giá trị cần tính phân bố, là số âm"
			},
			{
				name: "deg_freedom",
				description: "là số hiệu bậc tự do, là số trong khoảng từ 1 đến 10^10, loại trừ 10^10"
			}
		]
	},
	{
		name: "CHIINV",
		description: "Trả về giá trị đảo xác suất một phía của phân bố chi-bình-phương.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất gắn với chi-bình-phương, là giá trị từ 0 đến 1"
			},
			{
				name: "deg_freedom",
				description: "là số bậc tự do, là số trong khoảng từ 1 đến 10^10, loại trừ 10^10"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "Trả về xác suất phần dư bên trái của phân phối chi-bình phương.",
		arguments: [
			{
				name: "x",
				description: "là giá trị cần đánh giá phân phối, một số không âm"
			},
			{
				name: "deg_freedom",
				description: "là số bậc tự do, một số từ 1 đến 10^10, loại trừ 10^10"
			},
			{
				name: "cumulative",
				description: "là giá trị lôgic cho hàm để trả về: hàm phân phối tích lũy = TRUE; hàm mật độ xác suất = FALSE"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "Trả về xác suất nhánh phải của phân bố chi bình phương.",
		arguments: [
			{
				name: "x",
				description: "là giá trị mà tại đó bạn muốn đánh giá phân bố, một số không âm"
			},
			{
				name: "deg_freedom",
				description: "là số bậc tự do, một số nằm giữa 1 và 10^10, ngoài 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "Trả về nghịch đảo của xác suất phần dư bên trái của phân phối chi-bình phương.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất liên quan đến phân phối chi-bình phương, một giá trị từ 0 đến 1 và bao gồm cả 0 và 1"
			},
			{
				name: "deg_freedom",
				description: "là số bậc tự do, một số từ 1 đến 10^10, loại trừ 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "Trả về nghịch đảo của xác suất phần dư bên phải của phân phối chi-bình phương.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất liên quan đến phân phối chi-bình phương, một giá trị từ 0 đến 1 và bao gồm cả 0 và 1"
			},
			{
				name: "deg_freedom",
				description: "là số bậc tự do, một số từ 1 đến 10^10, loại trừ 10^10"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "Trả về Kiểm định tính độc lập: giá trị từ phân bố thống kê chi-bình-phương và bậc tự do tương ứng.",
		arguments: [
			{
				name: "actual_range",
				description: "là khoảng dữ liệu chứa giá trị quan sát để kiểm chứn lại với các giá trị mong đợi"
			},
			{
				name: "expected_range",
				description: "là khoảng dữ liệu chứa tỷ lệ giữa tích của tổng theo hàng và tổng theo cột với toàn tổng"
			}
		]
	},
	{
		name: "CHITEST",
		description: "Trả về Kiểm định tính độc lập: giá trị từ phân bố thống kê chi-bình-phương và bậc tự do tương ứng.",
		arguments: [
			{
				name: "actual_range",
				description: "là khoảng dữ liệu chứa giá trị quan sát để kiểm chứn lại với các giá trị mong đợi"
			},
			{
				name: "expected_range",
				description: "là khoảng dữ liệu chứa tỷ lệ giữa tích của tổng theo hàng và tổng theo cột với toàn tổng"
			}
		]
	},
	{
		name: "CHOOSE",
		description: "Chọn giá trị hoặc thao tác thực hiện từ danh sách các giá trị, dựa trên số hiệu chỉ mục.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index_num",
				description: "chỉ định tham đối giá trị được chọn. Index_num phải từ 1 đến 254, công thức hoặc tham chiếu tới một số từ 1 đến 254"
			},
			{
				name: "value1",
				description: "từ 1 đến 254 số, tham chiếu ô, tên định nghĩa, công thức, hàm, hoặc tham đối văn bản do hàm CHOOSE chọn"
			},
			{
				name: "value2",
				description: "từ 1 đến 254 số, tham chiếu ô, tên định nghĩa, công thức, hàm, hoặc tham đối văn bản do hàm CHOOSE chọn"
			}
		]
	},
	{
		name: "CLEAN",
		description: "Loại bỏ mọi ký tự không in trong văn bản.",
		arguments: [
			{
				name: "text",
				description: "là bất kỳ trong tin trang tính cần loại bỏ các ký tự không in"
			}
		]
	},
	{
		name: "CODE",
		description: "Trả về mã số của ký tự đầu tiên trong văn bản, theo tập ký tự sử dụng trong máy.",
		arguments: [
			{
				name: "text",
				description: "là văn bản cần mã của ký tự đầu tiên"
			}
		]
	},
	{
		name: "COLUMN",
		description: "Trả về số hiệu cột của tham chiếu.",
		arguments: [
			{
				name: "reference",
				description: "là một ô hoặc một vùng ô liên tiếp bạn cần có số hiệu cột; Nếu không có, sử dụng ô trong hàm CỘT"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "Trả về số cột trong một mảng hoặc tham chiếu.",
		arguments: [
			{
				name: "array",
				description: "là một mảng, công thức mảng, hoặc tham chiếu tới một khoảng các ô cần số cột"
			}
		]
	},
	{
		name: "COMBIN",
		description: "Trả về số tổ hợp của một số phần tử cho trước.",
		arguments: [
			{
				name: "number",
				description: "là tổng số phần tử"
			},
			{
				name: "number_chosen",
				description: "là số phần tử trong mỗi tổ hợp"
			}
		]
	},
	{
		name: "COMBINA",
		description: "Trả về số lượng tổ hợp có lặp lại của một số lượng khoản mục cho trước.",
		arguments: [
			{
				name: "number",
				description: "là tổng số lượng các khoản mục"
			},
			{
				name: "number_chosen",
				description: "là số lượng khoản mục của mỗi tổ hợp"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "Chuyển đổi hệ số thực và ảo thành số phức.",
		arguments: [
			{
				name: "real_num",
				description: "là hệ số thực của số phức"
			},
			{
				name: "i_num",
				description: "là hệ số ảocủa số phức"
			},
			{
				name: "suffix",
				description: "là phần tiếp sau phần ảo của số phức"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "Nối kết vài xâu văn bản vào một xâu văn bản.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "text1",
				description: "từ 1 đến 255 xâu văn bản được nối vào một xâu văn bản và có thể là xâu văn bản, số, hoặc tham chiếu ô đơn"
			},
			{
				name: "text2",
				description: "từ 1 đến 255 xâu văn bản được nối vào một xâu văn bản và có thể là xâu văn bản, số, hoặc tham chiếu ô đơn"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "Trả về khoảng tin cậy của trung bình tổng thể.",
		arguments: [
			{
				name: "alpha",
				description: "là mức quan trọng dùng để tính mức tin cậy, một số lớn hơn 0 và nhỏ hơn 1"
			},
			{
				name: "standard_dev",
				description: "là độ lệch tiêu chuẩn tập toàn bộ của khoảng dữ liệu được coi như đã biết trước. Độ_lệch_tiêu_chuẩn phải lớn hơn 0"
			},
			{
				name: "size",
				description: "là kích cỡ mẫu"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "Trả về khoảng tin cậy cho trung bình của tập hợp, sử dụng phân phối thông thường.",
		arguments: [
			{
				name: "alpha",
				description: "là mức quan trọng dùng để tính mức tin cậy, một số lớn hơn 0 và nhỏ hơn 1"
			},
			{
				name: "standard_dev",
				description: "là độ lệch chuẩn của tập hợp cho vùng dữ liệu và được coi là xác định. Độ lệch chuẩn phải lớn hơn 0"
			},
			{
				name: "size",
				description: "là kích thước mẫu"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "Trả về khoảng tin cậy cho trung bình của tập hợp, sử dụng phân phối Student T.",
		arguments: [
			{
				name: "alpha",
				description: "là mức quan trọng dùng để tính mức tin cậy, một số lớn hơn 0 và nhỏ hơn 1"
			},
			{
				name: "standard_dev",
				description: "là độ lệch chuẩn của tập hợp cho vùng dữ liệu và được coi là xác định. Độ lệch chuẩn phải lớn hơn 0"
			},
			{
				name: "size",
				description: "là kích thước mẫu"
			}
		]
	},
	{
		name: "CONVERT",
		description: "Chuyển đổi một số từ hệ đo này sang hệ khác.",
		arguments: [
			{
				name: "number",
				description: "là giá trị trong from_units cần chuyển đổi"
			},
			{
				name: "from_unit",
				description: "là đơn vị cho số"
			},
			{
				name: "to_unit",
				description: "là đơn vị cho kết quả"
			}
		]
	},
	{
		name: "CORREL",
		description: "Trả về hệ số tương quan giữa hai tập dữ liệu.",
		arguments: [
			{
				name: "array1",
				description: "là khoảng ô các giá trị. Giá trị phải là số, tên, mảng, hoặc tên chứa số"
			},
			{
				name: "array2",
				description: "là khoảng ô các giá trị thứ hai. Giá trị phải là số, tên, mảng, hoặc tên chứa số"
			}
		]
	},
	{
		name: "COS",
		description: "Trả về Cosin của góc.",
		arguments: [
			{
				name: "number",
				description: "là giá trị Cosin của góc theo Radian"
			}
		]
	},
	{
		name: "COSH",
		description: "Trả về Cosin hi-péc-bôn của một số.",
		arguments: [
			{
				name: "number",
				description: "là số thực bất kỳ"
			}
		]
	},
	{
		name: "COT",
		description: "Trả về hàm cotang của một góc.",
		arguments: [
			{
				name: "number",
				description: "là số đo góc theo rađian mà bạn muốn tìm cotang"
			}
		]
	},
	{
		name: "COTH",
		description: "Trả về hàm cotang hyperbol của một con số.",
		arguments: [
			{
				name: "number",
				description: "là số đo góc theo rađian mà bạn muốn tìm cotang hyperbol"
			}
		]
	},
	{
		name: "COUNT",
		description: "Đếm các ô trong phạm vi chứa giá trị số.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "từ 1 đến 255 tham đối chứa hoặc tham chiếu tới các loại dữ liệu khác nhau, nhưng chỉ tính kiểu số"
			},
			{
				name: "value2",
				description: "từ 1 đến 255 tham đối chứa hoặc tham chiếu tới các loại dữ liệu khác nhau, nhưng chỉ tính kiểu số"
			}
		]
	},
	{
		name: "COUNTA",
		description: "Đếm số ô không rỗng trong phạm vi và các giá trị nằm trong danh sách tham đối.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "từ 1 đến 30 tham đối thể hiện giá trị và ô cần đếm. Giá trị có thể là dạng thông tin bất kỳ"
			},
			{
				name: "value2",
				description: "từ 1 đến 30 tham đối thể hiện giá trị và ô cần đếm. Giá trị có thể là dạng thông tin bất kỳ"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "Đếm số ô rỗng trong khoảng các ô xác định.",
		arguments: [
			{
				name: "range",
				description: "là khoảng cần đếm số ô rỗng"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "Đếm số ô trong khoảng thoả mãn điều kiện cho trước.",
		arguments: [
			{
				name: "range",
				description: "là khoảng các ô cần đếm số ô không trắng"
			},
			{
				name: "criteria",
				description: "là điều kiện dưới dạng số, biểu thức, hoặc văn bản xác định các ô nào được đếm"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "Tính số ô được chỉ ra bởi tập cho trước các điều kiện hay chỉ tiêu.",
		arguments: [
			{
				name: "criteria_range",
				description: "là dải các ô muốn đánh giá theo điều kiện cụ thể"
			},
			{
				name: "criteria",
				description: "là điều kiện ở dạng số, biểu thức, hay văn bản xác định ô nào sẽ được tính"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "Trả lại số ngày từ đầu chu kỳ cu-pôn đến ngày thanh khoản.",
		arguments: [
			{
				name: "settlement",
				description: "là ngày thanh khoản thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "maturity",
				description: "là ngày tới hạn thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "frequency",
				description: "là số lần thanh toán cu-pôn hàng năm"
			},
			{
				name: "basis",
				description: "là loại cơ sở tính ngày được dùng"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "Trả lại ngày cu-pôn tiếp theo sau ngày thanh khoản thế chấp.",
		arguments: [
			{
				name: "settlement",
				description: "là ngày thanh khoản thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "maturity",
				description: "là ngày tới hạn thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "frequency",
				description: "là số lần thanh toán cu-pôn hàng năm"
			},
			{
				name: "basis",
				description: "là loại cơ sở tính ngày được dùng"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "Trả lại số cu-pôn trả được giữa ngày thanh khoản thế chấp và ngày tới hạn.",
		arguments: [
			{
				name: "settlement",
				description: "là ngày thanh khoản thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "maturity",
				description: "là ngày tới hạn thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "frequency",
				description: "là số lần thanh toán cu-pôn hàng năm"
			},
			{
				name: "basis",
				description: "là loại cơ sở tính ngày được dùng"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "Trả lại ngày cu-pôn đã qua trước ngày thanh khoản.",
		arguments: [
			{
				name: "settlement",
				description: "là ngày thanh khoản thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "maturity",
				description: "là ngày tới hạn thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "frequency",
				description: "là số lần thanh toán cu-pôn hàng năm"
			},
			{
				name: "basis",
				description: "là loại cơ sở tính ngày được dùng"
			}
		]
	},
	{
		name: "COVAR",
		description: "Trả về hiệp phương sai, là trung bình tích độ lệch của mỗi cặp điểm dữ liệu trong hai tập dữ liệu.",
		arguments: [
			{
				name: "array1",
				description: "là khoảng ô số nguyên thứ nhất và có thể là số, mảng, hoặc tham chiếu chứa số"
			},
			{
				name: "array2",
				description: "là khoảng ô số nguyên thứ hai và có thể là số, mảng, hoặc tham chiếu chứa số"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "Trả về hiệp phương sai mẫu, trung bình của các tích độ lệch cho mỗi cặp điểm dữ liệu trong hai tập hợp dữ liệu.",
		arguments: [
			{
				name: "array1",
				description: "là khoảng ô số nguyên thứ nhất và phải là số, mảng hoặc tham chiếu chứa số"
			},
			{
				name: "array2",
				description: "là khoảng ô số nguyên thứ hai và phải là số, mảng hoặc tham chiếu chứa số"
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "Trả về hiệp phương sai mẫu, trung bình của các tích độ lệch cho mỗi cặp điểm dữ liệu trong hai tập hợp dữ liệu.",
		arguments: [
			{
				name: "array1",
				description: "là khoảng ô số nguyên thứ nhất và phải là số, mảng hoặc tham chiếu chứa số"
			},
			{
				name: "array2",
				description: "là khoảng ô số nguyên thứ hai và phải là số, mảng hoặc tham chiếu chứa số"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "Trả về giá trị nhỏ nhất có nhị thức phân bố tích lũy lớn hơn hoặc bằng giá trị tiêu chuẩn.",
		arguments: [
			{
				name: "trials",
				description: "là số phép thử Bernoulli"
			},
			{
				name: "probability_s",
				description: "là xác suất thành công của mỗi lần thử, một số từ 0 đến 1"
			},
			{
				name: "alpha",
				description: "là giá trị tiêu chuẩn, một số từ 0 đến 1"
			}
		]
	},
	{
		name: "CSC",
		description: "Trả về hàm cosec của một góc.",
		arguments: [
			{
				name: "number",
				description: "là số đo góc theo rađian mà bạn muốn tìm cosec"
			}
		]
	},
	{
		name: "CSCH",
		description: "Trả về hàm cosec hyperbol của một góc.",
		arguments: [
			{
				name: "number",
				description: "là số đo góc theo rađian mà bạn muốn tìm cosec hyperbol"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "Trả lại tiền trả lợi nhuận cộng dồn giữa hai chu kỳ.",
		arguments: [
			{
				name: "rate",
				description: "là tỷ suất lợi nhuận"
			},
			{
				name: "nper",
				description: "là tổng số chu kỳ thanh toán"
			},
			{
				name: "pv",
				description: "là giá trị hiện thời"
			},
			{
				name: "start_period",
				description: "là chu kỳ đầu trong tính toán"
			},
			{
				name: "end_period",
				description: "là chu kỳ cuối trong tính toán"
			},
			{
				name: "type",
				description: "là khoảng thời gian thanh toán"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "Trả lại tiền trả cộng dồn nguyên tắc cho phần vay giữa hai chu kỳ.",
		arguments: [
			{
				name: "rate",
				description: "là tỷ suất lợi nhuận"
			},
			{
				name: "nper",
				description: "là tổng số chu kỳ thanh toán"
			},
			{
				name: "pv",
				description: "là giá trị hiện thời"
			},
			{
				name: "start_period",
				description: "là chu kỳ đầu trong tính toán"
			},
			{
				name: "end_period",
				description: "là chu kỳ cuối trong tính toán"
			},
			{
				name: "type",
				description: "là khoảng thời gian thanh toán"
			}
		]
	},
	{
		name: "DATE",
		description: "Trả về số thể hiện ngày tháng theo mã hóa ngày tháng trong Spreadsheet.",
		arguments: [
			{
				name: "year",
				description: "là một số từ 1900 đến 9999 trong Spreadsheet cho Windows hoặc từ 1904 đến 9999 trong Spreadsheet cho máy Macintosh"
			},
			{
				name: "month",
				description: "là một số từ 1 đến 12 thể hiện tháng trong năm"
			},
			{
				name: "day",
				description: "là một số từ 1 đến 31 thể hiện ngày trong tháng"
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
		description: "Chuyển đổi ngày tháng dưới dạng văn bản sang số thể hiện ngày tháng theo mã hóa ngày tháng trong Spreadsheet.",
		arguments: [
			{
				name: "date_text",
				description: "là văn bản dạng thức ngày tháng thể hiện ngày tháng trong Spreadsheet , giữa 1/1/1900 (Windows) hoặc 1/1/1904 (Macintosh) và 12/31/9999"
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "Trung bình giá trị của cột trong danh sách hoặc trong cơ sở dữ liệu thoả mãn điều kiện chỉ định.",
		arguments: [
			{
				name: "database",
				description: "là vùng ô tạo thành danh sách hoặc cơ sở dữ liệu. Cơ sở dữ liệu là danh sách các dữ liệu có liên quan"
			},
			{
				name: "field",
				description: "Hoặc là nhãn của cột trong cặp dấu nháy kép hoặc là số thứ tự của cột trong danh sách"
			},
			{
				name: "criteria",
				description: "là vùng ô chứa điều kiện chỉ định. Vùng này gồm nhãn cột và một ô bên dưới chỉ điều kiện"
			}
		]
	},
	{
		name: "DAY",
		description: "Trả về ngày trong tháng, một số từ 1 đến 31.",
		arguments: [
			{
				name: "serial_number",
				description: "là một số trong mã ngày-tháng được sử dụng bởi Spreadsheet"
			}
		]
	},
	{
		name: "DAYS",
		description: "Trả về số lượng ngày giữa hai ngày.",
		arguments: [
			{
				name: "end_date",
				description: "Bạn muốn biết có bao nhiêu ngày ở giữa start_date và end_date"
			},
			{
				name: "start_date",
				description: "Bạn muốn biết có bao nhiêu ngày ở giữa start_date và end_date"
			}
		]
	},
	{
		name: "DAYS360",
		description: "Trả về số ngày giữa hai ngày trên cơ sở năm 360-ngày (12 tháng 30-ngày).",
		arguments: [
			{
				name: "start_date",
				description: "ngày_bắt_đầu và ngày_kết_thúc là hai ngày mà bạn muốn biết số ngày có trong khoảng thời gian đó"
			},
			{
				name: "end_date",
				description: "ngày_bắt_đầu và ngày_kết_thúc là hai ngày mà bạn muốn biết số ngày có trong khoảng thời gian đó"
			},
			{
				name: "method",
				description: "là giá trị lô-gic chỉ định phương pháp tính toán: U.S. (NASD) = SAI hoặc không có; European = ĐÚNG."
			}
		]
	},
	{
		name: "DB",
		description: "Trả về khấu hao của tài sản cho một chu kỳ xác định sử dụng phương pháp giảm dần cố định.",
		arguments: [
			{
				name: "cost",
				description: "là chi phí ban đầu của tài sản"
			},
			{
				name: "salvage",
				description: "là giá trị còn lại vào cuối vòng đời của tài sản"
			},
			{
				name: "life",
				description: "là số chu kỳ khấu hao của tài sản (đôi khi được gọi là vòng đời của tài sản)"
			},
			{
				name: "period",
				description: "là chu kỳ cần tính khấu hao. Chu kỳ phải dùng cùng đơn vị tính như Vòng đời"
			},
			{
				name: "month",
				description: "là số tháng trong năm đầu tiên. Nếu không có, tháng được coi bằng 12"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "Đếm số ô chứa giá trị số trong trường (cột) của các bản ghi thuộc cơ sở dữ liệu thoả mãn các điều kiện chỉ định.",
		arguments: [
			{
				name: "database",
				description: "là vùng ô tạo thành danh sách hoặc cơ sở dữ liệu. Cơ sở dữ liệu là danh sách các dữ liệu có liên quan"
			},
			{
				name: "field",
				description: "Hoặc là nhãn của cột trong cặp dấu nháy kép hoặc là số thứ tự của cột trong danh sách"
			},
			{
				name: "criteria",
				description: "là vùng ô chứa điều kiện chỉ định. Vùng này gồm nhãn cột và một ô bên dưới chỉ điều kiện"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "Đếm số ô không trắng trong trường (cột) của các bản ghi trong cơ sở dữ liệu trùng khớp với điều kiện chỉ định.",
		arguments: [
			{
				name: "database",
				description: "là một khoảng các ô tạo thành danh sách hoặc cơ sở dữ liệu. Cơ sở dữ liệu là một danh sách dữ liệu có liên quan"
			},
			{
				name: "field",
				description: "hoặc là nhãn cột trong cặp dấu nháy kép hoặc là số thể hiện thứ tự cột trong danh sách"
			},
			{
				name: "criteria",
				description: "là khoảng các ô chứa điều kiện chỉ định. Khoảng bao gồm nhãn cột và môt ô bên dưới chỉ điều kiện"
			}
		]
	},
	{
		name: "DDB",
		description: "Trả về khấu hao của tài sản cho một chu kỳ xác định dùng phương pháp giảm dần kép hoặc một số phương pháp chỉ định khác.",
		arguments: [
			{
				name: "cost",
				description: "là chi phí ban đầu của tài sản"
			},
			{
				name: "salvage",
				description: "là giá trị còn lại vào cuối vòng đời của tài sản"
			},
			{
				name: "life",
				description: "là số chu kỳ khấu hao của tài sản (đôi khi được gọi là vòng đời của tài sản)"
			},
			{
				name: "period",
				description: "là chu kỳ cần tính khấu hao. Chu kỳ phải dùng cùng đơn vị tính như Vòng đời"
			},
			{
				name: "factor",
				description: "là tốc độ giảm dần. Nếu không có Hệ số, nó được gán bằng 2 (phương pháp giảm dần kép)"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "Chuyển số thập phân thành nhị phân.",
		arguments: [
			{
				name: "number",
				description: "là số nguyên thập phân mà bạn muốn chuyển"
			},
			{
				name: "places",
				description: "là số ký tự sử dụng"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "Chuyển số thập phân thành thập lục phân.",
		arguments: [
			{
				name: "number",
				description: "là số nguyên thập phân mà bạn muốn chuyển"
			},
			{
				name: "places",
				description: "là số ký tự sử dụng"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "Chuyển số thập phân thành bát phân.",
		arguments: [
			{
				name: "number",
				description: "là số nguyên thập phân mà bạn muốn chuyển"
			},
			{
				name: "places",
				description: "là số ký tự sử dụng"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Chuyển dạng biểu diễn bằng văn bản của một con số với cơ số cho trước sang con số ở hệ thập phân.",
		arguments: [
			{
				name: "number",
				description: "là con số bạn muốn chuyển"
			},
			{
				name: "radix",
				description: "là cơ số gốc của con số bạn đang chuyển"
			}
		]
	},
	{
		name: "DEGREES",
		description: "Chuyển đổi từ radian sang độ.",
		arguments: [
			{
				name: "angle",
				description: "là góc tính theo radian cần chuyển đổi"
			}
		]
	},
	{
		name: "DELTA",
		description: "Kiểm tra hai số có bằng nhau không.",
		arguments: [
			{
				name: "number1",
				description: "là số thứ nhất"
			},
			{
				name: "number2",
				description: "là số thứ hai"
			}
		]
	},
	{
		name: "DEVSQ",
		description: "Trả về tổng bình phương độ lệch của các điểm dữ liệu so với trung bình mẫu của chúng.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 đối số, hoặc một mảng hoặc tham chiếu mảng, mà bạn cần DEVSQ tính toán trên đó"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 đối số, hoặc một mảng hoặc tham chiếu mảng, mà bạn cần DEVSQ tính toán trên đó"
			}
		]
	},
	{
		name: "DGET",
		description: "Trích xuất từ cơ sở dữ liệu một bản ghi trùng khớp với các điều kiện chỉ định.",
		arguments: [
			{
				name: "database",
				description: "là vùng ô tạo thành danh sách hoặc cơ sở dữ liệu. Cơ sở dữ liệu là danh sách các dữ liệu có liên quan"
			},
			{
				name: "field",
				description: "Hoặc là nhãn của cột trong cặp dấu nháy kép hoặc là số thứ tự của cột trong danh sách"
			},
			{
				name: "criteria",
				description: "là khoảng các ô chứa điều kiện chỉ định. Khoảng bao gồm nhãn cột và môt ô bên dưới nhãn chỉ điều kiện"
			}
		]
	},
	{
		name: "DISC",
		description: "Trả lại tỷ suất giảm giá của thế chấp.",
		arguments: [
			{
				name: "settlement",
				description: "là ngày thanh khoản thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "maturity",
				description: "là ngày tới hạn thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "pr",
				description: "là giá của thế chấp trên $100 mệnh giá"
			},
			{
				name: "redemption",
				description: "là giá trị chuộc thế chấp trên $100 mệnh giá"
			},
			{
				name: "basis",
				description: "là loại cơ sở tính ngày được dùng"
			}
		]
	},
	{
		name: "DMAX",
		description: "Trả về giá trị lớn nhất trong trường (cột) của các bản ghi thuộc cơ sở dữ liệu thoả mãn điều kiện chỉ định.",
		arguments: [
			{
				name: "database",
				description: "là vùng ô tạo thành danh sách hoặc cơ sở dữ liệu. Cơ sở dữ liệu là danh sách các dữ liệu có liên quan"
			},
			{
				name: "field",
				description: "Hoặc là nhãn của cột trong cặp dấu nháy kép hoặc là số thứ tự của cột trong danh sách"
			},
			{
				name: "criteria",
				description: "là vùng ô chứa điều kiện chỉ định. Vùng này gồm nhãn cột và một ô bên dưới chỉ điều kiện"
			}
		]
	},
	{
		name: "DMIN",
		description: "Trả về giá trị nhỏ nhất trong trường (cột) của các bản ghi thuộc cơ sở dữ liệu thoả mãn điều kiện chỉ định.",
		arguments: [
			{
				name: "database",
				description: "là vùng ô tạo thành danh sách hoặc cơ sở dữ liệu. Cơ sở dữ liệu là danh sách các dữ liệu có liên quan"
			},
			{
				name: "field",
				description: "Hoặc là nhãn của cột trong cặp dấu nháy kép hoặc là số thứ tự của cột trong danh sách"
			},
			{
				name: "criteria",
				description: "là vùng ô chứa điều kiện chỉ định. Vùng này gồm nhãn cột và một ô bên dưới chỉ điều kiện"
			}
		]
	},
	{
		name: "DOLLAR",
		description: "Chuyển đổi số sang văn bản, sử dụng dạng thức tiền tệ.",
		arguments: [
			{
				name: "number",
				description: "là một số, tham chiếu tới ô chứa số, hoặc một công thức cho kết quả là số"
			},
			{
				name: "decimals",
				description: "là số chữ số bên phải dấu thập phân. Số được làm tròn theo yêu cầu; nếu không có, phần thập phân = 2"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "Chuyển đổi giá đô-la, thể hiện như phân số, sang giá đô-la, thể hiện như số thập phân.",
		arguments: [
			{
				name: "fractional_dollar",
				description: "là số thể hiện như phân số"
			},
			{
				name: "fraction",
				description: "là số nguyên dùng trong mẫu số của phân số"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "Chuyển đổi giá đô-la, thể hiện như số thập phân, sang giá đô-la, thể hiện như phân số.",
		arguments: [
			{
				name: "decimal_dollar",
				description: "là số thập phân"
			},
			{
				name: "fraction",
				description: "là số nguyên dùng trong mẫu số của phân số"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "Nhân giá trị trong trường (cột) của các bản ghi trong cơ sở dữ liệu trùng khớp với điều kiện chỉ định.",
		arguments: [
			{
				name: "database",
				description: "là một khoảng các ô tạo thành danh sách hoặc cơ sở dữ liệu. Cơ sở dữ liệu là một danh sách dữ liệu có liên quan"
			},
			{
				name: "field",
				description: "hoặc là nhãn cột trong cặp dấu nháy kép hoặc là số thể hiện thứ tự cột trong danh sách"
			},
			{
				name: "criteria",
				description: "là khoảng các ô chứa điều kiện chỉ định. Khoảng bao gồm nhãn cột và môt ô bên dưới chỉ điều kiện"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "Ước lượng độ lệch tiêu chuẩn dựa trên mẫu từ các mục trong cơ sở dữ liệu.",
		arguments: [
			{
				name: "database",
				description: "là vùng ô tạo thành danh sách hoặc cơ sở dữ liệu. Cơ sở dữ liệu là danh sách các dữ liệu có liên quan"
			},
			{
				name: "field",
				description: "Hoặc là nhãn của cột trong cặp dấu nháy kép hoặc là số thứ tự của cột trong danh sách"
			},
			{
				name: "criteria",
				description: "là vùng ô chứa điều kiện chỉ định. Vùng này gồm nhãn cột và một ô bên dưới chỉ điều kiện"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "Tính toán độ lệch tiêu chuẩn dựa trên tập toàn bộ của các mục trong cơ sở dữ liệu đã chọn.",
		arguments: [
			{
				name: "database",
				description: "là một khoảng các ô tạo thành danh sách hoặc cơ sở dữ liệu. Cơ sở dữ liệu là một danh sách dữ liệu có liên quan"
			},
			{
				name: "field",
				description: "hoặc là nhãn cột trong cặp dấu nháy kép hoặc là số thể hiện thứ tự cột trong danh sách"
			},
			{
				name: "criteria",
				description: "là khoảng các ô chứa điều kiện chỉ định. Khoảng bao gồm nhãn cột và môt ô bên dưới chỉ điều kiện"
			}
		]
	},
	{
		name: "DSUM",
		description: "Thêm số vào trường (cột) của các bản ghi thuộc cơ sở dữ liệu thoả mãn điều kiện chỉ định.",
		arguments: [
			{
				name: "database",
				description: "là vùng ô tạo thành danh sách hoặc cơ sở dữ liệu. Cơ sở dữ liệu là danh sách các dữ liệu có liên quan"
			},
			{
				name: "field",
				description: "Hoặc là nhãn của cột trong cặp dấu nháy kép hoặc là số thứ tự của cột trong danh sách"
			},
			{
				name: "criteria",
				description: "là vùng ô chứa điều kiện chỉ định. Vùng này gồm nhãn cột và một ô bên dưới chỉ điều kiện"
			}
		]
	},
	{
		name: "DVAR",
		description: "Ước lượng phương sai dựa trên mẫu từ các mục của cơ sở dữ liệu lựa chọn.",
		arguments: [
			{
				name: "database",
				description: "là vùng ô tạo thành danh sách hoặc cơ sở dữ liệu. Cơ sở dữ liệu là danh sách các dữ liệu có liên quan"
			},
			{
				name: "field",
				description: "Hoặc là nhãn của cột trong cặp dấu nháy kép hoặc là số thứ tự của cột trong danh sách"
			},
			{
				name: "criteria",
				description: "là vùng ô chứa điều kiện chỉ định. Vùng này gồm nhãn cột và một ô bên dưới chỉ điều kiện"
			}
		]
	},
	{
		name: "DVARP",
		description: "Tính toán dung sai dựa trên tập toàn bộ của các mục trong cơ sở dữ liệu đã chọn.",
		arguments: [
			{
				name: "database",
				description: "là một khoảng các ô tạo thành danh sách hoặc cơ sở dữ liệu. Cơ sở dữ liệu là một danh sách dữ liệu có liên quan"
			},
			{
				name: "field",
				description: "hoặc là nhãn cột trong cặp dấu nháy kép hoặc là số thể hiện thứ tự cột trong danh sách"
			},
			{
				name: "criteria",
				description: "là khoảng các ô chứa điều kiện chỉ định. Khoảng bao gồm nhãn cột và môt ô bên dưới chỉ điều kiện"
			}
		]
	},
	{
		name: "EDATE",
		description: "Trả lại số tuần tự của ngày tháng là số chỉ báo các tháng trước hay sau ngày bắt đầu.",
		arguments: [
			{
				name: "start_date",
				description: "là số tuần tự ngày tháng thể hiện ngày bắt đầu"
			},
			{
				name: "months",
				description: "là số tháng trước hay sau start_date"
			}
		]
	},
	{
		name: "EFFECT",
		description: "Trả lại tỷ suất lợi nhuận có hiệu lực hàng năm.",
		arguments: [
			{
				name: "nominal_rate",
				description: "là tỷ suất lợi nhuận danh nghĩa"
			},
			{
				name: "npery",
				description: "là số chu kỳ tổng gộp hàng năm"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "Trả về chuỗi mã hóa-URL.",
		arguments: [
			{
				name: "text",
				description: "là một chuỗi phải mã hóa URL"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "Trả lại số tuần tự của ngày cuối cùng của tháng trước hay sau số tháng chỉ định.",
		arguments: [
			{
				name: "start_date",
				description: "là số tuần tự ngày tháng thể hiện ngày bắt đầu"
			},
			{
				name: "months",
				description: "là số tháng trước hay sau start_date"
			}
		]
	},
	{
		name: "ERF",
		description: "Trả về hàm lỗi.",
		arguments: [
			{
				name: "lower_limit",
				description: "là biên dưới của tích hợp ERF"
			},
			{
				name: "upper_limit",
				description: "là biên trên của tích hợp ERF"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "Trả về hàm lỗi.",
		arguments: [
			{
				name: "X",
				description: "là cận dưới cho tích phân ERF.PRECISE"
			}
		]
	},
	{
		name: "ERFC",
		description: "Trả về hàm lỗi bổ sung.",
		arguments: [
			{
				name: "x",
				description: "là biên dưới của tích hợp ERF"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "Trả về hàm lỗi bù.",
		arguments: [
			{
				name: "X",
				description: "là cận dưới cho tích phân ERFC.PRECISE"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "Trả về một số khớp với giá trị lỗi.",
		arguments: [
			{
				name: "error_val",
				description: "là giá trị lỗi cần nhận biết bởi số, và có thể là giá trị lỗi thật sự hoặc tham chiếu tới ô chứa giá trị lỗi"
			}
		]
	},
	{
		name: "EVEN",
		description: "Làm tròn số dương lên và số âm xuống số nguyên chẵn gần nhất.",
		arguments: [
			{
				name: "number",
				description: "là giá trị cần làm tròn"
			}
		]
	},
	{
		name: "EXACT",
		description: "Kiểm tra hai xâu văn bản có chính xác giống nhau, và trả về ĐÚNG hoặc SAI. Hàm EXACT có phân biệt chữ hoa, chữ thường.",
		arguments: [
			{
				name: "text1",
				description: "Là xâu văn bản đầu tiên"
			},
			{
				name: "text2",
				description: "Là xâu văn bản thứ hai"
			}
		]
	},
	{
		name: "EXP",
		description: "Trả về giá trị e mũ số chỉ định.",
		arguments: [
			{
				name: "number",
				description: "là số mũ của e. Hằng số e là cơ số lô-ga-rít tự nhiên, bằng 2.71828182845904"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "Trả về phân bố hàm mũ.",
		arguments: [
			{
				name: "x",
				description: "là giá trị của hàm, một số không âm"
			},
			{
				name: "lambda",
				description: "là giá trị của tham biến, một số dương"
			},
			{
				name: "cumulative",
				description: "là giá trị lô gíc cho hàm trả về: = ĐÚNG là hàm phân bố tích lũy; =SAI là hàm mật độ xác suất"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "Trả về phân bố hàm mũ.",
		arguments: [
			{
				name: "x",
				description: "là giá trị của hàm, một số không âm"
			},
			{
				name: "lambda",
				description: "là giá trị của tham biến, một số dương"
			},
			{
				name: "cumulative",
				description: "là giá trị lô-gic cho hàm trả về: = ĐÚNG là hàm phân bố tích lũy; =SAI là hàm mật độ xác suất"
			}
		]
	},
	{
		name: "F.DIST",
		description: "Trả về phân phối xác suất F (phần dư bên trái) (độ đa dạng) cho hai tập dữ liệu.",
		arguments: [
			{
				name: "x",
				description: "là giá trị để đánh giá hàm, một số không âm"
			},
			{
				name: "deg_freedom1",
				description: "là bậc tự do tử số, một số từ 1 đến 10^10, loại trừ 10^10"
			},
			{
				name: "deg_freedom2",
				description: "là bậc tự do mẫu số, một số từ 1 đến 10^10, loại trừ 10^10"
			},
			{
				name: "cumulative",
				description: "là giá trị lôgic cho hàm để trả về: hàm phân phối tích lũy = TRUE; hàm mật độ xác suất = FALSE"
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "Trả về phân phối xác suất F (phần dư bên phải) (độ đa dạng) cho hai tập dữ liệu.",
		arguments: [
			{
				name: "x",
				description: "là giá trị để đánh giá hàm, một số không âm"
			},
			{
				name: "deg_freedom1",
				description: "là bậc tự do tử số, một số từ 1 đến 10^10, loại trừ 10^10"
			},
			{
				name: "deg_freedom2",
				description: "là bậc tự do mẫu số, một số từ 1 đến 10^10, loại trừ 10^10"
			}
		]
	},
	{
		name: "F.INV",
		description: "Trả về nghịch đảo của phân phối xác suất F (phần dư bên trái): nếu p = F.DIST(x,...), thì F.INV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất liên quan đến phân phối tích lũy F, một số từ 0 đến 1 và bao gồm cả 0 và 1"
			},
			{
				name: "deg_freedom1",
				description: "là bậc tự do tử số, một số từ 1 đến 10^10, loại trừ 10^10"
			},
			{
				name: "deg_freedom2",
				description: "là bậc tự do mẫu số, một số từ 1 đến 10^10, loại trừ 10^10"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "Trả về nghịch đảo của phân phối xác suất F (phần dư bên trái): nếu p = F.DIST(x,...), thì F.INV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất liên quan đến phân phối tích lũy F, một số từ 0 đến 1 và bao gồm cả 0 và 1"
			},
			{
				name: "deg_freedom1",
				description: "là bậc tự do tử số, một số từ 1 đến 10^10, loại trừ 10^10"
			},
			{
				name: "deg_freedom2",
				description: "là bậc tự do mẫu số, một số từ 1 đến 10^10, loại trừ 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Trả về kết quả của kiểm-chứng-F, là xác suất hai phía có phương sai trong Mảng1 và Mảng2 không quá khác nhau.",
		arguments: [
			{
				name: "array1",
				description: "là mảng hoặc khoảng dữ liệu đầu tiên và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số (không tính giá trị trắng)"
			},
			{
				name: "array2",
				description: "là mảng hoặc khoảng dữ liệu thứ hai và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số (không tính giá trị trắng)"
			}
		]
	},
	{
		name: "FACT",
		description: "Trả về giai thừa của một số, bằng 1*2*3*...* Số.",
		arguments: [
			{
				name: "number",
				description: "là một số không âm cần tính giai thừa"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "Trả về giai thừa kép của một số.",
		arguments: [
			{
				name: "number",
				description: "là giá trị để trả về giai thừa kép"
			}
		]
	},
	{
		name: "FALSE",
		description: "Trả về giá trị lô-gic SAI.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "Trả về phân bố xác suất F (độ đa dạng) của hai tập dữ liệu.",
		arguments: [
			{
				name: "x",
				description: "là giá trị định trị của hàm, một số không âm"
			},
			{
				name: "deg_freedom1",
				description: "là tử số bậc tự do, một số từ 1 đến 10^10, loại trừ 10^10"
			},
			{
				name: "deg_freedom2",
				description: "là mẫu số bậc tự do, một số từ 1 đến 10^10, loại trừ 10^10"
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
		description: "Trả về vị trí bắt đầu của một xâu văn bản nằm trong xâu văn bản khác. Hàm FIND có phân biệt chữ hoa, chữ thường.",
		arguments: [
			{
				name: "find_text",
				description: "là văn bản cần tìm. Sử dụng cặp dấu nháy kép (văn bản rỗng) để so khớp ký tự đầu tiên trong Văn_bản_chứa; không cho phép ký tự đại diện"
			},
			{
				name: "within_text",
				description: "là văn bản chứa văn bản cần tìm"
			},
			{
				name: "start_num",
				description: "Chỉ định ký tự từ đó bắt đầu tìm. Ký tự đầu tiên trong Văn_bản_chứa có số hiệu là 1. Nếu không có, Số_bắt_đầu = 1"
			}
		]
	},
	{
		name: "FINV",
		description: "Trả về giá trị đảo của phân bố xác suất F: nếu p = FDIST(x,...), thì FINV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất gắn với phân bố tích lũy F, một số từ 0 đến 1"
			},
			{
				name: "deg_freedom1",
				description: "là tử số bậc tự do, một số từ 1 đến 10^10, loại trừ 10^10"
			},
			{
				name: "deg_freedom2",
				description: "là mẫu số bậc tự do, một số từ 1 đến 10^10, loại trừ 10^10"
			}
		]
	},
	{
		name: "FISHER",
		description: "Trả về biến đổi Fi-sơ.",
		arguments: [
			{
				name: "x",
				description: "là giá trị cần biến đổi, một số từ -1 đến 1, loại trừ -1 và 1"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Trả về giá trị đảo của biến đổi Fi-sơ:nếu y = FISHER(x), thì FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "là giá trị cần thực hiện đảo biến đổi"
			}
		]
	},
	{
		name: "FIXED",
		description: "Làm tròn số theo phần thập phân chỉ định và trả về kết quả dạng văn bản có dấu hàng đơn vị hoặc không.",
		arguments: [
			{
				name: "number",
				description: "là số cần làm tròn và chuyển đổi sang dạng văn bản"
			},
			{
				name: "decimals",
				description: "là số chữ số bên phải dấu thập phân. Nếu không có, phần thập phân = 2"
			},
			{
				name: "no_commas",
				description: "là giá trị lô-gic:  = ĐÚNG sẽ không hiển thị dấu ZZ; = FALSE hoặc không có sẽ hiển thị dấu ZZ trong văn bản trả về"
			}
		]
	},
	{
		name: "FLOOR",
		description: " làm tròn number xuống bội số gần nhất của significance.",
		arguments: [
			{
				name: "number",
				description: "is giá trị số bạn muốn round"
			},
			{
				name: "significance",
				description: "is là bội số của cái bạn muốn làm tròn. Number và Signification cả hai phải cùng dương hay cùng âm"
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "Làm tròn số xuống về số nguyên gần nhất hoặc về bội số có nghĩa gần nhất.",
		arguments: [
			{
				name: "number",
				description: "là giá trị bạn muốn làm tròn"
			},
			{
				name: "significance",
				description: "là bội số bạn muốn làm tròn về"
			},
			{
				name: "mode",
				description: "nếu được cung cấp và không bằng không thì hàm này sẽ làm tròn về phía con số không"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "Làm tròn số xuống số nguyên gần nhất hoặc tới bội số có nghĩa gần nhất.",
		arguments: [
			{
				name: "number",
				description: "là giá trị số bạn muốn làm tròn"
			},
			{
				name: "significance",
				description: "là bội số mà bạn muốn làm tròn tới. "
			}
		]
	},
	{
		name: "FORECAST",
		description: "Tính toán, hoặc ước đoán, giá trị tương lai theo hướng tuyến tính sử dụng giá trị đã có.",
		arguments: [
			{
				name: "x",
				description: "là điểm dữ liệu cần ước đoán giá trị và phải là giá trị số"
			},
			{
				name: "known_y's",
				description: "là mảng hoặc khoảng dữ liệu số phụ thuộc"
			},
			{
				name: "known_x's",
				description: "là mảng hoặc khoảng dữ liệu số phụ thuộc. Phương sai của x_đã_biết không thể là không"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "Trả về công thức như là một xâu.",
		arguments: [
			{
				name: "reference",
				description: "là tham chiếu đến công thức"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "Tính số lần xuất hiện của một giá trị trong khoảng các giá trị và trả về một mảng dọc các số có nhiều phần tử hơn Bảng_đựng.",
		arguments: [
			{
				name: "data_array",
				description: "là mảng hoặc tham chiếu tới tập các giá trị cần đếm tần suất (không tính ô trắng và văn bản)"
			},
			{
				name: "bins_array",
				description: "là mảng hoặc tham chiếu tới các khẩu độ cần nhóm giá trị trong Mảng_dữ_liệu"
			}
		]
	},
	{
		name: "FTEST",
		description: "Trả về kết quả của kiểm-chứng-F, là xác suất hai phía có phương sai trong Mảng1 và Mảng2 không quá khác nhau.",
		arguments: [
			{
				name: "array1",
				description: "là mảng hoặc khoảng dữ liệu đầu tiên và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số (không tính giá trị trắng)"
			},
			{
				name: "array2",
				description: "là mảng hoặc khoảng dữ liệu thứ hai và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số (không tính giá trị trắng)"
			}
		]
	},
	{
		name: "FV",
		description: "Trả về giá trị tương lai của khoản đầu tư trên cơ sở các khoản thanh toán, lãi suất không đổi theo chu kỳ.",
		arguments: [
			{
				name: "rate",
				description: "là lãi suất theo chu kỳ. Ví dụ, khoản thanh toán hàng quý là 6% vào tháng tư sẽ dùng 6%/4"
			},
			{
				name: "nper",
				description: "là tổng số chu kỳ thanh toán trong khoản đầu tư"
			},
			{
				name: "pmt",
				description: "là khoản thanh toán cho mỗi chu kỳ và không thay đổi trong suốt thời gian đầu tư"
			},
			{
				name: "pv",
				description: "là giá trị hiện thời, hoặc lượng tiền cả gói có tính đến một chuỗi các khoản thanh toán tương lai. Nếu không có, Pv = 0"
			},
			{
				name: "type",
				description: "là giá trị thể hiện thời điểm thanh toán:=1 nếu là khoản thanh toán đầu chu kỳ; =0 hoặc không có nếu là khoản thanh toán cuối chu kỳ"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "Trả lại giá trị tương lai của vốn ban đầu sau khi áp dụng chuỗi tỷ suất lợi nhuận tổng gộp.",
		arguments: [
			{
				name: "principal",
				description: "là giá trị hiện thời"
			},
			{
				name: "schedule",
				description: "là mảng tỷ suất lợi nhuận để áp dụng"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Trả về giá trị hàm Gamma.",
		arguments: [
			{
				name: "x",
				description: "là giá trị mà bạn muốn tính Gamma"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "Trả về phân phối gamma.",
		arguments: [
			{
				name: "x",
				description: "là giá trị bạn cần đánh giá phân phối, một số không âm"
			},
			{
				name: "alpha",
				description: "là tham số cho phân phối, một số dương"
			},
			{
				name: "beta",
				description: "là tham số cho phân phối, một số dương. Nếu beta = 1, GAMMA.DIST trả về phân phối gamma tiêu chuẩn"
			},
			{
				name: "cumulative",
				description: "là giá trị lôgic: trả về hàm phân phối tích lũy = TRUE; trả về hàm xác suất số đông = FALSE hoặc bị bỏ qua"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Trả về nghịch đảo của hàm tích lũy gamma: nếu p = GAMMA.DIST(x,...), thì GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất liên quan đến phân phối gamma, một số từ 0 đến 1, bao gồm cả 0 và 1"
			},
			{
				name: "alpha",
				description: "là tham số cho phân phối, một số dương"
			},
			{
				name: "beta",
				description: "là tham số cho phân phối, một số dương. Nếu beta = 1, GAMMA.INV trả về nghịch đảo của phân phối gamma chuẩn"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "Trả về phân bố Gamma.",
		arguments: [
			{
				name: "x",
				description: "Là giá trị cần định trị cho phân bố, một số không âm"
			},
			{
				name: "alpha",
				description: "là một tham biến cho phân bố, một số dương"
			},
			{
				name: "beta",
				description: "là một tham biến cho phân bố, một số dương. Nếu beta = 1, GAMMADIST trả về phân bố gamma chuẩn"
			},
			{
				name: "cumulative",
				description: "là giá trị lô-gic: dành cho hàm phân bố tích lũy, dùng ĐÚNG; hàm xác suất số đông, dùng SAI"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Trả về giá trị đảo của phân bố tích lũy gamma: nếu p = GAMMADIST(x,...), thì GAMMAINV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất gắn với phân bố gamma, một số từ 0 đến 1"
			},
			{
				name: "alpha",
				description: "là một tham biến cho phân bố, một số dương"
			},
			{
				name: "beta",
				description: "là một tham biến cho phân bố, một số dương. Nếu beta = 1, GAMMAINV trả về giá trị đảo của phân bố gamma chuẩn"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Trả về lô-ga-rít tự nhiên của hàm gamma.",
		arguments: [
			{
				name: "x",
				description: "là giá trị cần tính GAMMALN, là số dương"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "Trả về loga tự nhiên của hàm gamma.",
		arguments: [
			{
				name: "x",
				description: "là giá trị cho bạn tính GAMMALN.PRECISE, một số dương"
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
		description: "Trả lại ước số chung lớn nhất.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "là các giá trị 1 tới 255"
			},
			{
				name: "number2",
				description: "là các giá trị 1 tới 255"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "Trả về trung bình hình học của một mảng hoặc khoảng dữ liệu số dương.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "là các số từ 1 đến 255 hoặc các tên, các mảng, hoặc các tham chiếu có chứa số cần tính trung bình"
			},
			{
				name: "number2",
				description: "là các số từ 1 đến 255 hoặc các tên, các mảng, hoặc các tham chiếu có chứa số cần tính trung bình"
			}
		]
	},
	{
		name: "GESTEP",
		description: "Kiếm tra số có lớn hơn giá trị ngưỡng không.",
		arguments: [
			{
				name: "number",
				description: "là giá trị cần kiểm tra lại theo bước"
			},
			{
				name: "step",
				description: "là giá trị ngưỡng"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "Trích xuất dữ liệu lưu trong PivotTable.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "data_field",
				description: "là tên của trường dữ liệu cần trích xuất dữ liệu"
			},
			{
				name: "pivot_table",
				description: "là tham chiếu tới ô hoặc khoảng ô trong PivotTable chứa dữ liệu cần trích xuất"
			},
			{
				name: "field",
				description: "trường và mục được tham chiếu tới"
			},
			{
				name: "item",
				description: "trường và mục được tham chiếu tới"
			}
		]
	},
	{
		name: "GROWTH",
		description: "Trả về các số tăng theo hướng hàm mũ phù hợp với các điểm dữ liệu.",
		arguments: [
			{
				name: "known_y's",
				description: "là tập hợp các giá trị y xác định theo quan hệ y = b*m^x, một mảng hoặc khoảng các số dương"
			},
			{
				name: "known_x's",
				description: "là tập hợp tùy chọn các giá trị x xác định theo quan hệ = b*m^x, một mảng hoặc khoảng có cùng kích cỡ như y-xác định"
			},
			{
				name: "new_x's",
				description: "là giá trị x mới cần cho hàm GROWTH trả về giá trị y tương ứng"
			},
			{
				name: "const",
				description: "là giá trị lô gíc: hằng số b được tính toán bình thường nếu Hằng số=ĐÚNG; b đặt bằng 1 nếu Hằng số=SAI hoặc không có"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "Trả về trung bình điều hoà của tập dữ liệu số dương: số nghịch đảo của trung bình nghịch đảo số học.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 số hoặc tên, mảng, hoặc tham chiếu chứa số cần tính trung bình điều hoà"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 số hoặc tên, mảng, hoặc tham chiếu chứa số cần tính trung bình điều hoà"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "Chuyển số thập lục phân thành nhị phân.",
		arguments: [
			{
				name: "number",
				description: "là số thập lục phân mà bạn muốn chuyển"
			},
			{
				name: "places",
				description: "là số ký tự sử dụng"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "Chuyển số thập lục phân thành thập phân.",
		arguments: [
			{
				name: "number",
				description: "là số thập lục phân mà bạn muốn chuyển"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "Chuyển số thập lục phân thành bát phân.",
		arguments: [
			{
				name: "number",
				description: "là số thập lục phân mà bạn muốn chuyển"
			},
			{
				name: "places",
				description: "là số ký tự sử dụng"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "Tìm giá trị trong hàng trên cùng của bảng hoặc mảng các giá trị và trả về giá trị cùng cột từ hàng chỉ định.",
		arguments: [
			{
				name: "lookup_value",
				description: "là giá trị tìm thấy trong hàng đầu tiên của bảng và có thể là giá trị, tham chiếu,hoặc xâu văn bản"
			},
			{
				name: "table_array",
				description: "là một bảng các văn bản, số, hoặc giá trị lô gíc chứa dữ liệu cần tìm. Bảng_mảng có thể là tham chiếu tới khoảng hoặc tên khoảng"
			},
			{
				name: "row_index_num",
				description: "là số hiệu hàng của giá trị trả về phù hợp trong Bảng_mảng. Hàng giá trị đầu tiên trong bảng là hàng 1"
			},
			{
				name: "range_lookup",
				description: "là giá trị lô-gic: =ĐÚNG hoặc không có nếu cần tìm phù hợp nhất trong hàng trên cùng (sắp xếp theo trật tự tăng dần); =SAI nếu cần tìm khớp"
			}
		]
	},
	{
		name: "HOUR",
		description: "Trả về số hiệu của giờ từ 0 (12:00 SA) đến 23 (11:00 CH).",
		arguments: [
			{
				name: "serial_number",
				description: "là một số theo mã ngày tháng của Spreadsheet, hoặc văn bản theo dạng thức thời gian, như 16:48:00 hoặc 4:48:00 CH"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "Tạo Tạo lối tắt hoặc bước nhảy để mở tài liệu lưu trên ổ cứng, máy chủ mạng hoặc trên internet.",
		arguments: [
			{
				name: "link_location",
				description: "là văn bản đưa ra đường dẫn và tên tệp của tài liệu được mở, một vị trí trên đĩa cứng, địa chỉ UNC, hoặc đường dẫn URL"
			},
			{
				name: "friendly_name",
				description: "là văn bản hoặc số được hiển thị trong ô. Nếu không có, sẽ hiển thị văn bản Vị_trí_Liên_kết"
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "Trả về phân phối siêu hình học.",
		arguments: [
			{
				name: "sample_s",
				description: "là số lần thành công trong mẫu"
			},
			{
				name: "number_sample",
				description: "là kích cỡ của mẫu"
			},
			{
				name: "population_s",
				description: "là số lần thành công trong tập hợp"
			},
			{
				name: "number_pop",
				description: "là kích cỡ tập hợp"
			},
			{
				name: "cumulative",
				description: "là giá trị lôgic: đối với hàm phân phối tích lũy, sử dụng TRUE; đối với hàm mật độ xác suất, sử dụng FALSE"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "Trả về phân bố siêu-hình-học.",
		arguments: [
			{
				name: "sample_s",
				description: "là số lần thành công trong mẫu"
			},
			{
				name: "number_sample",
				description: "là kích cỡ của mẫu"
			},
			{
				name: "population_s",
				description: "là số lần thành công trong tập toàn bộ"
			},
			{
				name: "number_pop",
				description: "là kích cỡ tập toàn bộ"
			}
		]
	},
	{
		name: "IF",
		description: "Kiểm tra điều kiện có đáp ứng không và trả về một giá trị nếu TRUE, trả về một giá trị giá trị khác nếu FALSE.",
		arguments: [
			{
				name: "logical_test",
				description: "là bất kỳ giá trị hoặc biểu thức nào có thể được đánh giá ĐÚNG hoặc SAI"
			},
			{
				name: "value_if_true",
				description: "là giá trị được trả về nếu Logical_test là ĐÚNG. Nếu không có, ĐÚNG được trả về. Bạn có thể lồng tới bảy hàm IF"
			},
			{
				name: "value_if_false",
				description: "là giá trị được trả về nếu kiểm tra lô gíc là SAI. Nếu không có, SAI được trả về"
			}
		]
	},
	{
		name: "IFERROR",
		description: "Trả lại value_if_error nếu biểu thức có lỗi và giá trị của biểu thức nếu không.",
		arguments: [
			{
				name: "value",
				description: "là giá trị của biểu thức hay tham chiếu"
			},
			{
				name: "value_if_error",
				description: "là bất kỳ giá trị hay biểu thức hay tham chiếu"
			}
		]
	},
	{
		name: "IFNA",
		description: "Trả về giá trị bạn chỉ định nếu biểu thức cho ra kết quả #N/A, nếu không thì sẽ trả về giá trị của biểu thức.",
		arguments: [
			{
				name: "value",
				description: "là bất kỳ giá trị hoặc biểu thức hoặc tham chiếu nào"
			},
			{
				name: "value_if_na",
				description: "là bất kỳ giá trị hoặc biểu thức hoặc tham chiếu nào"
			}
		]
	},
	{
		name: "IMABS",
		description: "Trả về giá trị tuyệt đối của số phức.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức cần lấy giá trị tuyệt đối"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "Trả về hệ số ảo của số phức.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức cần lấy hệ số ảo"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Trả về tham đối q, góc theo rađian.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức cần lấy tham đối "
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "Trả về số liên hợp của số phức.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức cần lấy số liên hợp"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Trả về côsin của số phức.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức cần lấy côsin"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Trả về hàm cos hyperbol của số phức.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức bạn muốn tìm cos hyperbol"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Trả về hàm cotang của một số phức.",
		arguments: [
			{
				name: "number",
				description: "là số phức mà bạn muốn tìm cotang"
			}
		]
	},
	{
		name: "IMCSC",
		description: "Trả về hàm cosec của một số phức.",
		arguments: [
			{
				name: "number",
				description: "là số phức mà bạn muốn tìm cosec"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Trả về hàm cosec hyperbol của một số phức.",
		arguments: [
			{
				name: "number",
				description: "là số phức mà bạn muốn tìm cosec hyperbol"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Trả về thương số của hai số phức.",
		arguments: [
			{
				name: "inumber1",
				description: "là tử số phức hay số bị chia"
			},
			{
				name: "inumber2",
				description: "là mẫu số phức hay số bị chia"
			}
		]
	},
	{
		name: "IMEXP",
		description: "Trả về số mũ của số phức.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức cần lấy số mũ"
			}
		]
	},
	{
		name: "IMLN",
		description: "Trả về lô-ga-rít tự nhiên của số phức.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức cần lấy lô-ga-rít tự nhiên"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Trả về lô-ga-rít cơ số 10 của số phức.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức cần lấy lô-ga-rít cơ số 10"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Trả về lô-ga-rít cơ số 2 của số phức.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức cần lấy lô-ga-rít cơ số 2"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "Trả về số phức lũy thừa nguyên.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức muốn tính lũy thừa"
			},
			{
				name: "number",
				description: "là lũy thừa của số phức"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "Trả về tích của 1 đến 255 số phức.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "Inumber1, Inumber2,... từ 1 đến 255 số ảo cần nhân."
			},
			{
				name: "inumber2",
				description: "Inumber1, Inumber2,... từ 1 đến 255 số ảo cần nhân."
			}
		]
	},
	{
		name: "IMREAL",
		description: "Trả về hệ số thực của số phức.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức cần lấy hệ số thực"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Trả về hàm sec của một số phức.",
		arguments: [
			{
				name: "number",
				description: "là số phức mà bạn muốn tìm sec"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Trả về hàm sec hyperbol của một số phức.",
		arguments: [
			{
				name: "number",
				description: "là số phức mà bạn muốn tìm sec hyperbol"
			}
		]
	},
	{
		name: "IMSIN",
		description: "Trả về sin của số phức.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức cần lấy sin"
			}
		]
	},
	{
		name: "IMSINH",
		description: "Trả về hàm sin hyperbol của số phức.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức bạn muốn tìm sim hyperbol"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "Trả về căn bậc hai của số phức.",
		arguments: [
			{
				name: "inumber",
				description: "là số phức cần lấy căn bậc hai"
			}
		]
	},
	{
		name: "IMSUB",
		description: "Trả về độ chênh lệch giữa hai số phức.",
		arguments: [
			{
				name: "inumber1",
				description: "là số phức để trừ đi inumber2"
			},
			{
				name: "inumber2",
				description: "ilà số phức để trừ từ inumber1"
			}
		]
	},
	{
		name: "IMSUM",
		description: "Trả về tổng của các số phức.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "từ 1 đến 255 số ảo cần cộng"
			},
			{
				name: "inumber2",
				description: "từ 1 đến 255 số ảo cần cộng"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Trả về hàm tang của một số phức.",
		arguments: [
			{
				name: "number",
				description: "là số phức mà bạn muốn tìm tang"
			}
		]
	},
	{
		name: "INDEX",
		description: "Trả về giá trị hoặc tham chiếu tới ô giao của hàng và cột trong vùng chỉ định.",
		arguments: [
			{
				name: "array",
				description: "là một vùng ô hoặc một hằng số dạng mảng."
			},
			{
				name: "row_num",
				description: "chọn hàng trong Mảng hoặc Tham chiếu trả về giá trị. Nếu không, cần có Số hiệu-cột"
			},
			{
				name: "column_num",
				description: "chọn cột trong Mảng hoặc Tham chiếu trả về giá trị. Nếu không, cần có Số hiệu-hàng"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "Trả về tham chiếu chỉ định bởi xâu văn bản.",
		arguments: [
			{
				name: "ref_text",
				description: "là tham chiếu tới ô chứa một tham chiếu kiểu A1 hoặc R1C1, tên xác định của tham chiếu, hoặc một tham chiếu tới ô là xâu văn bản"
			},
			{
				name: "a1",
				description: "là giá trị lô-gic chỉ định loại tham chiếu trong Văn_bản_tham chiếu: = SAI nếu là kiểu R1C1; =ĐÚNG hoặc không có nếu là kiểu A1"
			}
		]
	},
	{
		name: "INFO",
		description: "Trả lại thông tin về môi trường hệ điều hành hiện thời.",
		arguments: [
			{
				name: "type_text",
				description: "là văn bản chỉ định loại thông tin cần trả về."
			}
		]
	},
	{
		name: "INT",
		description: "Làm tròn số xuống giá trị nguyên gần nhất.",
		arguments: [
			{
				name: "number",
				description: "là phần số thực cần làm tròn số xuống giá trị nguyên"
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "Tính toán giao điểm của đường thẳng với trục y dùng đường hồi quy khớp nhất vẽ qua các giá trị x và giá trị đã biết.",
		arguments: [
			{
				name: "known_y's",
				description: "là tập các giá trị quan sát hoặc dữ liệu phụ thuộc và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số"
			},
			{
				name: "known_x's",
				description: "là tập các giá trị quan sát hoặc dữ liệu độc lập và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số"
			}
		]
	},
	{
		name: "INTRATE",
		description: "Trả lại tỷ suất lợi tức cho thế chấp đầu tư đầy đủ.",
		arguments: [
			{
				name: "settlement",
				description: "là ngày thanh khoản thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "maturity",
				description: "là ngày tới hạn thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "investment",
				description: "là số lượng đầu tư vào thế chấp"
			},
			{
				name: "redemption",
				description: "là số lượng sẽ nhận được lúc tới hạn"
			},
			{
				name: "basis",
				description: "là loại cơ sở tính ngày được dùng"
			}
		]
	},
	{
		name: "IPMT",
		description: "Trả về lãi suất thanh toán cho một chu kỳ định trước của một khoản đầu tư trên cơ sở thanh toán không đổi và lãi suất không đổi theo chu kỳ.",
		arguments: [
			{
				name: "rate",
				description: "là lãi suất theo chu kỳ. Ví dụ, khoản thanh toán hàng quý là 6% vào tháng tư sẽ dùng 6%/4"
			},
			{
				name: "per",
				description: "là chu kỳ cần xác định lãi và phải nằm trong khoảng từ 1 đến SốChukỳ"
			},
			{
				name: "nper",
				description: "là tổng số chu kỳ thanh toán cho một khoản đầu tư"
			},
			{
				name: "pv",
				description: "là giá trị hiện thời, hoặc lượng tiền cả gói có tính đến một chuỗi các khoản thanh toán tương lai"
			},
			{
				name: "fv",
				description: "là giá trị tương lai, hoặc cân đối tiền mặt cần thu được sau khi thực hiện khoản thanh toán cuối cùng. Nếu không có, Fv=0"
			},
			{
				name: "type",
				description: "là giá trị thể hiện thời điểm thanh toán:=0 hoặc không có nếu là khoản thanh toán cuối chu kỳ; =1 nếu là khoản thanh toán đầu chu kỳ"
			}
		]
	},
	{
		name: "IRR",
		description: "Trả về tỷ lệ thu hồi nội tại của một chuỗi các lưu chuyển tiền tệ.",
		arguments: [
			{
				name: "values",
				description: "là một mảng hoặc tham chiếu tới ô chứa các số cần tính tỷ lệ thu hồi nội tại"
			},
			{
				name: "guess",
				description: "là số đoán trước sát với kết quả của tỷ lệ thu hồi nội tại; 0.1 (10 phần trăm) nếu không có"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "Kiểm tra tham chiếu có phải tới một ô rỗng, và trả về ĐÚNG hoặc SAI.",
		arguments: [
			{
				name: "value",
				description: "là ô hoặc tên tham chiếu tới ô cần thử"
			}
		]
	},
	{
		name: "ISERR",
		description: "Kiểm tra xem liệu một giá trị có phải là lỗi không (#VALUE!, #REF!, #DIV/0!, #NUM!, #NAME?, hoặc #NULL!) ngoại trừ #N/A, và trả về TRUE hoặc FALSE.",
		arguments: [
			{
				name: "value",
				description: "là giá trị bạn muốn thử. Giá trị có thể tham chiếu tới một ô, công thức, hoặc tên tham chiếu tới ô, công thức, hoặc giá trị"
			}
		]
	},
	{
		name: "ISERROR",
		description: "Kiểm tra xem liệu một giá trị có phải là lỗi không (#N/A, #VALUE!, #REF!, #DIV/0!, #NUM!, #NAME?, hoặc #NULL!), và trả về TRUE hoặc FALSE.",
		arguments: [
			{
				name: "value",
				description: "là giá trị bạn muốn thử. Giá trị có thể tham chiếu tới một ô, công thức, hoặc tên tham chiếu tới ô, công thức, hoặc giá trị"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "Trả về ĐÚNG nếu là số chẵn.",
		arguments: [
			{
				name: "number",
				description: "là giá trị cần kiểm tra"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "Kiểm tra xem tham chiếu tới một ô có bao gồm công thức không, và trả về TRUE (ĐÚNG) hoặc FALSE (SAI).",
		arguments: [
			{
				name: "reference",
				description: "là tham chiếu tới ô bạn muốn kiểm tra. Tham chiếu có tể là tham chiếu ô, công thức, hoặc tên mà tham chiếu tới ô"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "Kiểm tra giá trị có phải là giá trị lô-gic (ĐÚNG hoặc SAI), và trả về ĐÚNG hoặc SAI.",
		arguments: [
			{
				name: "value",
				description: "là giá trị cần thử. Giá trị có thể tham chiếu tới ô, công thức, hoặc tên tham chiếu tới ô, công thức, hoặc một giá trị"
			}
		]
	},
	{
		name: "ISNA",
		description: "Kiểm tra giá trị có phải là #N/A, trả về ĐÚNG hoặc SAI.",
		arguments: [
			{
				name: "value",
				description: "là giá trị cần kiểm tra. Giá trị có thể tham chiếu tới ô, công thức hoặc là tên tham chiếu tới ô, công thức hoặc là một giá trị"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "Kiểm tra giá trị có phải là văn bản (ô trắng không phải là văn bản), và trả về ĐÚNG hoặc SAI.",
		arguments: [
			{
				name: "value",
				description: "là giá trị cần thử: một ô; công thức; hoặc tên tham chiếu tới ô, công thức, hoặc giá trị"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "Kiểm tra một giá trị có phải là số, và trả về ĐÚNG hoặc SAI.",
		arguments: [
			{
				name: "value",
				description: "là giá trị cần thử. Giá trị có thể tham chiếu tới ô, công thức,hoặc tên tham chiếu tới ô, công thức,hoặc một giá trị"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "Làm tròn số lên đến số nguyên gần nhất hoặc tới bội số có nghĩa gần nhất.",
		arguments: [
			{
				name: "number",
				description: "là giá trị bạn muốn làm tròn"
			},
			{
				name: "significance",
				description: "là bội số tùy chọn mà bạn muốn làm tròn tới"
			}
		]
	},
	{
		name: "ISODD",
		description: "Trả về ĐÚNG nếu là số lẻ.",
		arguments: [
			{
				name: "number",
				description: "là giá trị cần kiểm tra"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "Trả về con số của mã số tuần ISO trong năm với ngày cho trước.",
		arguments: [
			{
				name: "date",
				description: "là mã ngày-giờ sử dụng bởi Spreadsheet để tính toán ngày và giờ"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Trả về lãi phải trả trong chu kỳ xác định của khoản đầu tư.",
		arguments: [
			{
				name: "rate",
				description: "lãi suất theo chu kỳ. Ví dụ, khoản thanh toán hàng quý là 6% vào tháng tư sẽ dùng 6%/4"
			},
			{
				name: "per",
				description: "chu kỳ cần tìm lãi suất"
			},
			{
				name: "nper",
				description: "số chu kỳ thanh toán của khoản đầu tư"
			},
			{
				name: "pv",
				description: "lượng tiền cả gói có tính ngay một chuỗi các khoản thanh toán tương lai"
			}
		]
	},
	{
		name: "ISREF",
		description: "Kiểm tra giá trị có phải là tham chiếu, trả về ĐÚNG hoặc SAI.",
		arguments: [
			{
				name: "value",
				description: "là giá trị cần thử. Giá trị có thể tham chiếu tới ô, công thức, hoặc tên tham chiếu tới ô, công thức, hoặc giá trị"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "Kiểm tra một giá trị có phải là văn bản, và trả về ĐÚNG hoặc SAI.",
		arguments: [
			{
				name: "value",
				description: "là giá trị cần thử. Giá trị có thể tham chiếu tới ô, công thức,hoặc tên tham chiếu tới ô, công thức,hoặc một giá trị"
			}
		]
	},
	{
		name: "KURT",
		description: "Trả về độ nhọn của tập dữ liệu.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 số hoặc tên, mảng, hoặc tham chiếu chứa số cần tính độ nhọn"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 số hoặc tên, mảng, hoặc tham chiếu chứa số cần tính độ nhọn"
			}
		]
	},
	{
		name: "LARGE",
		description: "Trả về giá trị lớn thứ k trong tập dữ liệu. Ví dụ, số lớn thứ năm.",
		arguments: [
			{
				name: "array",
				description: "là mảng hoặc khoảng dữ liệu cần xác định giá trị lớn thứ k"
			},
			{
				name: "k",
				description: "là vị trí (từ giá trị lớn nhất) của một giá trị trong mảng hoặc khoảng ô trả về"
			}
		]
	},
	{
		name: "LCM",
		description: "Trả lại mẫu số chung nhỏ nhất.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "là các giá trị 1 tới 255 mà bạn muốn có mẫu số chung nhỏ nhất"
			},
			{
				name: "number2",
				description: "là các giá trị 1 tới 255 mà bạn muốn có mẫu số chung nhỏ nhất"
			}
		]
	},
	{
		name: "LEFT",
		description: "Trả về số ký tự xác định từ vị trí bắt đầu của xâu văn bản.",
		arguments: [
			{
				name: "text",
				description: "là xâu văn bản chứa các ký tự cần trích xuất"
			},
			{
				name: "num_chars",
				description: "Xác định số ký tự hàm LEFT cần trích xuất; là 1 nếu không có"
			}
		]
	},
	{
		name: "LEN",
		description: "Trả về số lượng ký tự trong xâu văn bản.",
		arguments: [
			{
				name: "text",
				description: "là phần văn bản cần tìm độ dài. Dấu cách được coi như ký tự"
			}
		]
	},
	{
		name: "LINEST",
		description: "Trả về thống kê mô tả xu hướng tuyến tính phù hợp với các điểm dữ liệu, bằng cách khớp đường thẳng dùng phương pháp bình quân nhỏ nhất.",
		arguments: [
			{
				name: "known_y's",
				description: "là tập hợp các giá trị y xác định theo quan hệ y = mx + b"
			},
			{
				name: "known_x's",
				description: "là tập hợp tùy chọn các giá trị x xác định theo quan hệ y = mx + b"
			},
			{
				name: "const",
				description: "là giá trị lô gíc: hằng số b được tính toán bình thường nếu Hằng số=ĐÚNG hoặc không có; b đặt bằng 0 nếu Hằng số=SAI"
			},
			{
				name: "stats",
				description: "là giá trị lô gíc: sẽ trả về thống kê hồi quy bổ sung nếu = ĐÚNG; trả về hệ số m và hằng số b nếu = SAI hoặc không có"
			}
		]
	},
	{
		name: "LN",
		description: "Trả về lô-ga-rít tự nhiên của một số.",
		arguments: [
			{
				name: "number",
				description: "là phần số thực dương cần lấy lô-ga-rít"
			}
		]
	},
	{
		name: "LOG",
		description: "trả về lô-ga-rít của một số theo cơ số chỉ định.",
		arguments: [
			{
				name: "number",
				description: "là phần số thực dương cần tính lô-ga-rít"
			},
			{
				name: "base",
				description: "là cơ số của lô-ga-rít; 10 nếu không có"
			}
		]
	},
	{
		name: "LOG10",
		description: "Trả về lô-ga-rít cơ số 10 của một số.",
		arguments: [
			{
				name: "number",
				description: "là phần số thực dương cần lấy lô-ga-rít cơ số 10"
			}
		]
	},
	{
		name: "LOGEST",
		description: "Trả về thống kê mô tả đường hàm mũ phù hợp với các điểm dữ liệu.",
		arguments: [
			{
				name: "known_y's",
				description: "là tập hợp các giá trị y xác định theo quan hệ y = b*m^x"
			},
			{
				name: "known_x's",
				description: "là tập hợp tùy chọn các giá trị x xác định theo quan hệ y = b*m^x"
			},
			{
				name: "const",
				description: "là giá trị lô gíc: hằng số b được tính toán bình thường nếu Hằng số=ĐÚNG hoặc không có; b đặt bằng 0 nếu Hằng số=SAI"
			},
			{
				name: "stats",
				description: "là giá trị lô gíc: sẽ trả về thống kê hồi quy bổ sung nếu = ĐÚNG; trả về hệ số m và hằng số b nếu = SAI hoặc không có"
			}
		]
	},
	{
		name: "LOGINV",
		description: "Trả về giá trị đảo của hàm phân bố tích lũy loga chuẩn của x, mà ln(x) được phân phối chuẩn với tham số Mean và Standard_dev.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất gắn với phân bố loga chuẩn, một số từ 0 đến 1, kể cả 0 và 1"
			},
			{
				name: "mean",
				description: "là trung bình của ln(x)"
			},
			{
				name: "standard_dev",
				description: "là độ lệch tiêu chuẩn của ln(x), một số dương"
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "Trả về phân phối lôga bình thường của x, trong đó ln(x) được phân phối chuẩn với tham số Trung bình và Độ lệch chuẩn.",
		arguments: [
			{
				name: "x",
				description: "là giá trị để đánh giá hàm, một số dương"
			},
			{
				name: "mean",
				description: "là trung bình của ln(x)"
			},
			{
				name: "standard_dev",
				description: "là độ lệch chuẩn của ln(x), một số dương"
			},
			{
				name: "cumulative",
				description: "là giá trị lôgic: đối với hàm phân phối tích lũy, sử dụng TRUE; đối với hàm mật độ xác suất, sử dụng FALSE"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Trả về giá trị đảo của hàm phân bố lô-ga-rít chuẩn tích lũy, trong đó ln(x) được phân bố chuẩn với tham số Trung_bình và Độ_lệch_tiêu _chuẩn.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất gắn với phân bố lô-ga-rít chuẩn, một số từ 0 đến 1"
			},
			{
				name: "mean",
				description: "là trung bình của ln(x)"
			},
			{
				name: "standard_dev",
				description: "là độ lệch tiêu chuẩn của ln(x), một số dương"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "Trả về phân bố lô-ga-rít chuẩn tích lũy, trong đó ln(x) được phân bố chuẩn với tham số Trung_bình và Độ_lệch_tiêu _chuẩn.",
		arguments: [
			{
				name: "x",
				description: "là giá trị định trị của hàm, một số dương"
			},
			{
				name: "mean",
				description: "là trung bình của ln(x)"
			},
			{
				name: "standard_dev",
				description: "là độ lệch tiêu chuẩn của ln(x), một số dương"
			}
		]
	},
	{
		name: "LOOKUP",
		description: "Tìm trong phạm vi một hàng, một cột, hoặc trong mảng.  Được đưa ra để đảm bảo tương thích ngược.",
		arguments: [
			{
				name: "lookup_value",
				description: "là giá trị hàm LOOKUP tìm trong vector-tìm, có thể là số, văn bản, giá trị lô gíc hoặc tên, tham chiếu tới một giá trị"
			},
			{
				name: "lookup_vector",
				description: "là vùng chỉ chứa một hàng, một cột giá trị văn bản, số, lô gíc theo trật tự tăng dần"
			},
			{
				name: "result_vector",
				description: "là vùng chỉ chứa một hàng hoặc một cột, cùng kích cỡ với vector-tìm"
			}
		]
	},
	{
		name: "LOWER",
		description: "Chuyển đổi mọi chữ cái trong xâu văn bản sang chữ thường.",
		arguments: [
			{
				name: "text",
				description: "là văn bản cần chuyển đổi sang chữ thường. Các ký tự trong Văn bản không phải là chữ cái sẽ không đổi"
			}
		]
	},
	{
		name: "MATCH",
		description: "Trả về vị trí tương đối của một phần tử trong mảng khớp với giá trị cho trước theo trật tự nhất định.",
		arguments: [
			{
				name: "lookup_value",
				description: "là giá trị dùng để tìm giá trị mong muốn trong mảng, có thể là số, văn bản, giá trị lô gíc, hoặc tham chiếu tới các giá trị đó"
			},
			{
				name: "lookup_array",
				description: "là một khoảng các ô liên tục chứa giá trị, mảng các giá trị, hoặc tham chiếu tới mảng tìm kiếm"
			},
			{
				name: "match_type",
				description: "là số 1, 0, hoặc -1 chỉ thị giá trị trả về."
			}
		]
	},
	{
		name: "MAX",
		description: "Trả về giá trị lớn nhất trong tập hợp giá trị. Không tính giá trị lô gíc và văn bản.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 số, ô rỗng, giá trị lô-gic, hoặc văn bản số bạn cần tính giá trị lớn nhất"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 số, ô rỗng, giá trị lô-gic, hoặc văn bản số bạn cần tính giá trị lớn nhất"
			}
		]
	},
	{
		name: "MAXA",
		description: "Trả về giá trị lớn nhất trong tập các giá trị. Không loại trừ giá trị lô gíc và văn bản.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "từ 1 đến 255 số, ô rỗng, giá trị lô gíc, hoặc văn bản số cần giá trị tối đa"
			},
			{
				name: "value2",
				description: "từ 1 đến 255 số, ô rỗng, giá trị lô gíc, hoặc văn bản số cần giá trị tối đa"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Trả về ma trận xác định mảng.",
		arguments: [
			{
				name: "array",
				description: "là một mảng số có cùng số lượng hàng và cột, của một khoảng các ô hoặc một hằng số mảng"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Trả lại số trung bình hoặc số ở khoảng giữa tập hợp các số đã cho.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "là các số từ 1 đến 255 hoặc các tên, các mảng hoặc các tham chiếu có chứa các số mà bạn muốn số trung bình"
			},
			{
				name: "number2",
				description: "là các số từ 1 đến 255 hoặc các tên, các mảng hoặc các tham chiếu có chứa các số mà bạn muốn số trung bình"
			}
		]
	},
	{
		name: "MID",
		description: "Trả về các ký tự ở giữa xâu văn bản, với vị trí bắt đầu và độ dài chỉ định.",
		arguments: [
			{
				name: "text",
				description: "là xâu văn bản cần trích xuất các ký tự"
			},
			{
				name: "start_num",
				description: "là vị trí ký tự đầu tiên cần trích xuất. Vị trí ký tự đầu tiên trong Văn bản là 1"
			},
			{
				name: "num_chars",
				description: "Xác định số lượng ký tự được trả về từ Văn bản"
			}
		]
	},
	{
		name: "MIN",
		description: "Trả về số nhỏ nhất trong tập hợp giá trị. Không tính giá trị lô gíc và văn bản.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 số, ô rỗng, giá trị lô-gic, hoặc văn bản số bạn cần tính giá trị nhỏ nhất"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 số, ô rỗng, giá trị lô-gic, hoặc văn bản số bạn cần tính giá trị nhỏ nhất"
			}
		]
	},
	{
		name: "MINA",
		description: "Trả về giá trị nhỏ nhất trong tập các giá trị. Không loại trừ giá trị lô gíc và văn bản.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "từ 1 đến 255 số, ô rỗng, giá trị lô gíc, hoặc văn bản số cần giá trị tối thiểu"
			},
			{
				name: "value2",
				description: "từ 1 đến 255 số, ô rỗng, giá trị lô gíc, hoặc văn bản số cần giá trị tối thiểu"
			}
		]
	},
	{
		name: "MINUTE",
		description: "Trả về phút, một số từ 0 đến 59.",
		arguments: [
			{
				name: "serial_number",
				description: "là một số theo mã ngày tháng của Spreadsheet hoặc văn bản theo dạng thức thời gian, như 16:48:00 hoặc 4:48:00 CH"
			}
		]
	},
	{
		name: "MINVERSE",
		description: "Trả về ma trận đảo của ma trận trong mảng.",
		arguments: [
			{
				name: "array",
				description: "là một mảng số có cùng số lượng hàng và cột, của một khoảng các ô hoặc một hằng số mảng"
			}
		]
	},
	{
		name: "MIRR",
		description: "Trả về tỷ lệ thu hồi nội tại của một chuỗi các lưu chuyển tiền tệ định kỳ, có tính đến cả chi phí đầu tư và lợi tức của việc tái đầu tư tiền mặt.",
		arguments: [
			{
				name: "values",
				description: "là một mảng hoặc tham chiếu tới ô thể hiện một chuỗi các khoản thanh toán (số âm) và thu nhập (số dương) trong các chu kỳ đều nhau"
			},
			{
				name: "finance_rate",
				description: "là lãi suất phải trả cho khoản tiền sử dụng trong lưu chuyển tiền tệ"
			},
			{
				name: "reinvest_rate",
				description: "là lãi suất nhận được trong lưu chuyển tiền tệ do tái đầu tư chúng"
			}
		]
	},
	{
		name: "MMULT",
		description: "Trả về ma trận tích của hai mảng, là một mảng có cùng số hàng như Mảng1 và số cột như Mảng2.",
		arguments: [
			{
				name: "array1",
				description: "là mảng các số đầu tiên được nhân và phải có số cột bằng với số hàng của Mảng2"
			},
			{
				name: "array2",
				description: "là mảng các số đầu tiên được nhân và phải có số cột bằng với số hàng của Mảng2"
			}
		]
	},
	{
		name: "MOD",
		description: "Trả về phần dư của một số sau khi bị chia.",
		arguments: [
			{
				name: "number",
				description: "là số cần tìm phần dư sau khi thực hiện phép chia"
			},
			{
				name: "divisor",
				description: "là số cần chia cho Số"
			}
		]
	},
	{
		name: "MODE",
		description: "Trả về mức xuất hiện thường xuyên, hoặc mức lặp của một giá trị trong mảng hoặc khoảng dữ liệu.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 số, hoặc tên, mảng, hoặc tham chiếu chứa số cần tính cách thức"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 số, hoặc tên, mảng, hoặc tham chiếu chứa số cần tính cách thức"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "Trả về mảng dọc các giá trị xuất hiện thường xuyên nhất, hoặc lặp lại trong mảng hoặc vùng dữ liệu.  Đối với mảng ngang, sử dụng =TRANSPOSE(MODE.MULT(number1,number2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "là 1 đến 255 số, hoặc tên, mảng hoặc tham chiếu có chứa các số cần lập mẫu"
			},
			{
				name: "number2",
				description: "là 1 đến 255 số, hoặc tên, mảng hoặc tham chiếu có chứa các số cần lập mẫu"
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "Trả về mức xuất hiện thường xuyên, hoặc mức lặp của một giá trị trong mảng hoặc khoảng dữ liệu.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 số, hoặc tên, mảng, hoặc tham chiếu chứa số cần tính cách thức"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 số, hoặc tên, mảng, hoặc tham chiếu chứa số cần tính cách thức"
			}
		]
	},
	{
		name: "MONTH",
		description: "Trả về tháng, một số từ 1 (tháng Giêng) tới 12 (Tháng Chạp).",
		arguments: [
			{
				name: "serial_number",
				description: "là một số trong mã ngày-tháng được sử dụng bởi Spreadsheet"
			}
		]
	},
	{
		name: "MROUND",
		description: "Trả về giá trị đã làm tròn theo bội số.",
		arguments: [
			{
				name: "number",
				description: "là giá trị cần làm tròn"
			},
			{
				name: "multiple",
				description: "là bội số cần làm tròn tới"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Trả lại đa thức từ một tập số.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "là các giá trị 1 tới 255 mà bạn muốn có đa thức"
			},
			{
				name: "number2",
				description: "là các giá trị 1 tới 255 mà bạn muốn có đa thức"
			}
		]
	},
	{
		name: "MUNIT",
		description: "Trả về ma trận đơn vị của chiều được chỉ định.",
		arguments: [
			{
				name: "dimension",
				description: "là số nguyên xác định chiều của ma trận đơn vị bạn muốn trả về"
			}
		]
	},
	{
		name: "N",
		description: "Chuyển đổi giá trị khác số thành số, ngày tháng thành số tuần tự, ĐÚNG thành 1, các giá trị khác thành 0 (không).",
		arguments: [
			{
				name: "value",
				description: "là giá trị cần chuyển đổi"
			}
		]
	},
	{
		name: "NA",
		description: "Trả về giá trị lỗi #N/A (giá trị không áp dụng).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "Trả về xác suất nhị thức âm, xác suất sẽ là Number_f lần thất bại trước lần thành công thứ Number_s, với xác suất thành công Probability_s.",
		arguments: [
			{
				name: "number_f",
				description: "là số lần thất bại"
			},
			{
				name: "number_s",
				description: "là ngưỡng số lần thành công"
			},
			{
				name: "probability_s",
				description: "là xác suất thành công; là số từ 0 đến 1"
			},
			{
				name: "cumulative",
				description: "là giá trị lôgic: đối với hàm phân phối tích lũy, sử dụng TRUE; đối với hàm xác suất số đông, sử dụng FALSE"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "Trả về phân bố nhị thức âm, là xác suất sẽ có Number_f lần thất bại trước thành công thứ Number_s-th, với xác suất của thành công là Probability_s.",
		arguments: [
			{
				name: "number_f",
				description: "là số lần thất bại"
			},
			{
				name: "number_s",
				description: "là số ngưỡng của thành công"
			},
			{
				name: "probability_s",
				description: "là xác suất của thành công; một số từ 0 đến 1"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "Trả lại số ngày làm việc đầy đủ giữa hai mốc ngày.",
		arguments: [
			{
				name: "start_date",
				description: "là số ngày tuần tự thể hiện ngày bắt đầu"
			},
			{
				name: "end_date",
				description: "là số ngày tuần tự thể hiện ngày kết thúc"
			},
			{
				name: "holidays",
				description: "là tập tùy chọn của một hay nhiều số ngày tuần tự để loại khỏi lịch ngày làm việc như ngày lễ của tỉnh hay quốc gia và các ngày nghỉ lễ bù"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "Trả về số ngày làm việc cả ngày giữa hai ngày với tham số ngày cuối tuần tùy chỉnh.",
		arguments: [
			{
				name: "start_date",
				description: "là số ngày tuần tự thể hiện ngày bắt đầu"
			},
			{
				name: "end_date",
				description: "là số ngày tuần tự thể hiện ngày kết thúc"
			},
			{
				name: "weekend",
				description: "là số hoặc chuỗi chỉ định thời điểm diễn ra ngày cuối tuần"
			},
			{
				name: "holidays",
				description: "là tập hợp tùy chọn một hoặc nhiều số ngày tuần tự để loại trừ khỏi lịch làm việc, như ngày lễ tiểu bang và liên bang và ngày nghỉ lễ bù"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "Trả lại tỷ suất lợi nhuận danh nghĩa hàng năm.",
		arguments: [
			{
				name: "effect_rate",
				description: "là tỷ suất lợi nhuận có hiệu lực"
			},
			{
				name: "npery",
				description: "là số chu kỳ tổng gộp hàng năm"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "Trả về phân phối bình thường cho độ lệch chuẩn và trung bình đã chỉ định.",
		arguments: [
			{
				name: "x",
				description: "là giá trị cần tính phân phối"
			},
			{
				name: "mean",
				description: "là giá trị trung bình cộng của phân phối"
			},
			{
				name: "standard_dev",
				description: "là độ lệch chuẩn của phân phối, một số dương"
			},
			{
				name: "cumulative",
				description: "là giá trị lôgic: đối với hàm phân phối tích lũy, sử dụng TRUE; đối với hàm mật độ xác suất, sử dụng FALSE"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Trả về giá trị đảo của phân bố tích lũy chuẩn của trung bình và độ lệch tiêu chuẩn cho trước.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất gắn với phân bố chuẩn, một số từ 0 đến 1"
			},
			{
				name: "mean",
				description: "là trung bình số học của phân bố"
			},
			{
				name: "standard_dev",
				description: "là độ lệch tiêu chuẩn của phân bố, một số dương"
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "Trả về phân phối bình thường chuẩn (có trung bình là không và độ lệch chuẩn là một).",
		arguments: [
			{
				name: "z",
				description: "là giá trị cần tính phân phối"
			},
			{
				name: "cumulative",
				description: "là giá trị lôgic cho hàm để trả về: hàm phân phối tích lũy = TRUE; hàm mật độ xác suất = FALSE"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Trả về giá trị đảo của phân bố tích lũy chuẩn (có trung bình là không và độ lêch tiêu chuẩn là 1).",
		arguments: [
			{
				name: "probability",
				description: "là xác suất gắn với phân bố chuẩn, một số từ 0 đến 1"
			}
		]
	},
	{
		name: "NORMDIST",
		description: "Trả về phân bố tích lũy chuẩn của trung bình và độ lệch tiêu chuẩn cho trước.",
		arguments: [
			{
				name: "x",
				description: "là giá trị cần tính phân bố"
			},
			{
				name: "mean",
				description: "là trung bình số học của phân bố"
			},
			{
				name: "standard_dev",
				description: "là độ lệch tiêu chuẩn của phân bố, một số dương"
			},
			{
				name: "cumulative",
				description: "là giá trị lô-gic: dành cho hàm phân bố tích lũy, dùng ĐÚNG; hàm xác suất số đông, dùng SAI"
			}
		]
	},
	{
		name: "NORMINV",
		description: "Trả về giá trị đảo của phân bố tích lũy chuẩn của trung bình và độ lệch tiêu chuẩn cho trước.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất gắn với phân bố chuẩn, một số từ 0 đến 1, kể cả 0 và 1"
			},
			{
				name: "mean",
				description: "là trung bình số học của phân bố"
			},
			{
				name: "standard_dev",
				description: "là độ lệch tiêu chuẩn của phân bố, một số dương"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "Trả về phân bố tích lũy chuẩn (có trung bình là không và độ lêch tiêu chuẩn là 1).",
		arguments: [
			{
				name: "z",
				description: "là giá trị cần tính phân bố"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "Trả về giá trị đảo của phân bố tích lũy chuẩn (có trung bình là không và độ lêch tiêu chuẩn là 1).",
		arguments: [
			{
				name: "probability",
				description: "là xác suất gắn với phân bố chuẩn, một số từ 0 đến 1"
			}
		]
	},
	{
		name: "NOT",
		description: "Đổi SAI thành ĐÚNG, ĐÚNG thành SAI.",
		arguments: [
			{
				name: "logical",
				description: "là giá trị hoặc biểu thức có thể đánh giá là ĐÚNG hoặc SAI"
			}
		]
	},
	{
		name: "NOW",
		description: "Trả về ngày tháng hiện tại theo dạng thức ngày tháng và thời gian.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "Trả về số chu kỳ của khoản đầu tư trên cơ sở các khoản thanh toán, lãi suất không đổi theo chu kỳ.",
		arguments: [
			{
				name: "rate",
				description: "là lãi suất theo chu kỳ. Ví dụ, khoản thanh toán hàng quý là 6% vào tháng tư sẽ dùng 6%/4"
			},
			{
				name: "pmt",
				description: "là khoản thanh toán cho mỗi chu kỳ và không thay đổi trong suốt thời gian đầu tư"
			},
			{
				name: "pv",
				description: "là giá trị hiện thời, hoặc lượng tiền cả gói có tính đến một chuỗi các khoản thanh toán tương lai"
			},
			{
				name: "fv",
				description: "là giá trị tương lai, hoặc cân đối tiền mặt cần thu được sau khi thực hiện khoản thanh toán cuối cùng. Nếu không có, sẽ bằng 0"
			},
			{
				name: "type",
				description: "là một giá trị logíc: = 1 nếu trả vào đầu chu kỳ ; = 0 hoặc được bỏ qua nếu trả vào cuối chu kỳ"
			}
		]
	},
	{
		name: "NPV",
		description: "Trả lại giá trị thực hiện tại của khoản đầu tư dựa trên tỷ lệ khấu trừ và chuỗi các khoản thanh toán (giá trị âm) và thu nhập (giá trị dương) tương lai.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rate",
				description: "là tỷ lệ khấu trừ qua suốt một chu kỳ"
			},
			{
				name: "value1",
				description: "từ 1 đến 254 khoản thanh toán và thu nhập, theo thời khoảng đều nhau và thực hiện vào cuối mỗi chu kỳ"
			},
			{
				name: "value2",
				description: "từ 1 đến 254 khoản thanh toán và thu nhập, theo thời khoảng đều nhau và thực hiện vào cuối mỗi chu kỳ"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "Chuyển đổi văn bản sang con số độc lập miền địa phương.",
		arguments: [
			{
				name: "text",
				description: "là xâu thể hiện con số bạn muốn chuyển đổi"
			},
			{
				name: "decimal_separator",
				description: "là ký tự sử dụng như là dấu phân cách thập phân trong xâu"
			},
			{
				name: "group_separator",
				description: "là ký tự sử dụng như là dấu phân cách nhóm trong xâu"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "Chuyển số bát phân thành nhị phân.",
		arguments: [
			{
				name: "number",
				description: "là số bát phân mà bạn muốn chuyển"
			},
			{
				name: "places",
				description: "là số ký tự sử dụng"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "Chuyển số bát phân thành thập phân.",
		arguments: [
			{
				name: "number",
				description: "là số bát phân mà bạn muốn chuyển"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "Chuyển số bát phân thành thập lục phân.",
		arguments: [
			{
				name: "number",
				description: "là số bát phân mà bạn muốn chuyển"
			},
			{
				name: "places",
				description: "là số ký tự sử dụng"
			}
		]
	},
	{
		name: "ODD",
		description: "Là tròn số dương lên và số âm xuống số lẻ gần nhất.",
		arguments: [
			{
				name: "number",
				description: "là giá trị cần làm tròn"
			}
		]
	},
	{
		name: "OFFSET",
		description: "Trả về tham chiếu tới khoảng từ số hàng và số cột của một khoảng cho trước.",
		arguments: [
			{
				name: "reference",
				description: "là tham chiếu mà khoảng chênh dựa vào, một tham chiếu tới ô hoặc khoảng các ô lân cận"
			},
			{
				name: "rows",
				description: "là số hàng, trên hoặc dưới, ô trên-trái trong kết quả tham chiếu tới"
			},
			{
				name: "cols",
				description: "là số cột, trái hoặc phải, ô trên-trái trong kết quả tham chiếu tới"
			},
			{
				name: "height",
				description: "là độ cao, tính bằng số hàng, trong kết quả mong muốn, bằng độ cao của Tham chiếu nếu không có"
			},
			{
				name: "width",
				description: "là độ rộng, tính bằng số cột, trong kết quả mong muốn, bằng độ rộng của Tham chiếu nếu không có"
			}
		]
	},
	{
		name: "OR",
		description: "Kiểm tra nếu tất cả các tham đối là SAI, trả về giá trị SAI.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "từ 1 đền 255 điều kiện cần kiểm tra, có thể là ĐÚNG hoặc SAI"
			},
			{
				name: "logical2",
				description: "từ 1 đền 255 điều kiện cần kiểm tra, có thể là ĐÚNG hoặc SAI"
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
		description: "Trả về số lượng chu kỳ để khoản đầu tư đạt đến giá trị được chỉ định.",
		arguments: [
			{
				name: "rate",
				description: "là lãi suất của mỗi chu kỳ."
			},
			{
				name: "pv",
				description: "là giá trị hiện tại của khoản đầu tư"
			},
			{
				name: "fv",
				description: "là giá trị tương lai mong muốn của khoản đầu tư"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Trả về hệ số tương quan mômen tích Pia-sơn, r.",
		arguments: [
			{
				name: "array1",
				description: "là tập các giá trị độc lập"
			},
			{
				name: "array2",
				description: "là tập các giá trị phụ thuộc"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "Trả về phân vị thứ k của các giá trị trong khoảng.",
		arguments: [
			{
				name: "array",
				description: "là mảng hoặc khoảng dữ liệu xác định thế tương quan"
			},
			{
				name: "k",
				description: "là giá trị phân vị trong khoảng từ 0 đến 1"
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "Trả về phân vị thứ k của giá trị trong một khoảng, trong đó k nằm trong khoảng 0..1, bao gồm.",
		arguments: [
			{
				name: "array",
				description: "là cả mảng hoặc vùng dữ liệu xác định vị trí tương đối"
			},
			{
				name: "k",
				description: "là giá trị phân vị từ 0 đến 1, bao gồm cả 0 và 1"
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "Trả về phân vị thứ k của giá trị trong một khoảng, trong đó k nằm trong khoảng 0..1, bao gồm.",
		arguments: [
			{
				name: "array",
				description: "là cả mảng hoặc vùng dữ liệu xác định vị trí tương đối"
			},
			{
				name: "k",
				description: "là giá trị phân vị từ 0 đến 1, bao gồm cả 0 và 1"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "Trả về cấp của giá trị trong tập dữ liệu được gán như số phần trăm của tập dữ liệu.",
		arguments: [
			{
				name: "array",
				description: "là mảng hoặc khoảng dữ liệu với các giá trị số xác định thế tương quan"
			},
			{
				name: "x",
				description: "là giá trị cần biết cấp"
			},
			{
				name: "significance",
				description: "là giá trị tùy chọn chỉ định số chữ số có nghĩa của số phần trăm trả về, là 3 chữ số nếu không có (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "Trả về thứ hạng của một giá trị trong tập hợp dữ liệu dưới dạng phần trăm của tập hợp dữ liệu dưới dạng phần trăm (0..1, bao gồm cả 0 và 1) của tập hợp dữ liệu.",
		arguments: [
			{
				name: "array",
				description: "là mảng hoặc vùng dữ liệu với giá trị số xác định vị trí tương đối"
			},
			{
				name: "x",
				description: "là giá trị cần tính xếp hạng"
			},
			{
				name: "significance",
				description: "là giá trị tùy chọn xác định số chữ số có nghĩa của số phần trăm trả về, ba chữ số nếu bị bỏ qua (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "Trả về thứ hạng của một giá trị trong tập hợp dữ liệu dưới dạng phần trăm của tập hợp dữ liệu dưới dạng phần trăm (0..1, bao gồm cả 0 và 1) của tập hợp dữ liệu.",
		arguments: [
			{
				name: "array",
				description: "là mảng hoặc vùng dữ liệu với giá trị số xác định vị trí tương đối"
			},
			{
				name: "x",
				description: "là giá trị cần tính xếp hạng"
			},
			{
				name: "significance",
				description: "là giá trị tùy chọn xác định số chữ số có nghĩa của số phần trăm trả về, ba chữ số nếu bị bỏ qua (0.xxx%)"
			}
		]
	},
	{
		name: "PERMUT",
		description: "Trả về số hoán vị của số đối tượng được chọn từ tổng số đối tượng.",
		arguments: [
			{
				name: "number",
				description: "là tổng số đối tượng"
			},
			{
				name: "number_chosen",
				description: "là số đối tượng trong mỗi hoán vị"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Trả về số lượng hoán vị của một số lượng đối tượng cho trước (có trùng lặp) mà có thể được chọn từ tổng số đối tượng.",
		arguments: [
			{
				name: "number",
				description: "là tổng số lượng các đối tượng"
			},
			{
				name: "number_chosen",
				description: "là số lượng đối tượng của mỗi hoán vị"
			}
		]
	},
	{
		name: "PHI",
		description: "Trả về hàm mật độ của phân phối chuẩn chuẩn hóa.",
		arguments: [
			{
				name: "x",
				description: "là con số bạn muốn tìm mật độ của phân phối chuẩn chuẩn hóa"
			}
		]
	},
	{
		name: "PI",
		description: "Trả về giá trị số Pi, 3.14159265358979, lấy chính xác tới 15 chữ số.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "Tính toán phần phải thanh toán cho khoản vay trên cơ sở các khoản thanh toán, lãi suất không đổi.",
		arguments: [
			{
				name: "rate",
				description: "là lãi suất theo chu kỳ của khoản vay. Ví dụ, khoản thanh toán hàng quý là 6% vào tháng tư sẽ dùng 6%/4"
			},
			{
				name: "nper",
				description: "là tổng số phần phải thanh toán cho khoản vay"
			},
			{
				name: "pv",
				description: "là giá trị hiện thời: có tính luôn tổng của một chuỗi các khoản thanh toán tương lai"
			},
			{
				name: "fv",
				description: "là giá trị tương lai, hoặc cân đối tiền mặt cần thu được sau khi thực hiện khoản thanh toán cuối cùng. Nếu không có, sẽ bằng 0"
			},
			{
				name: "type",
				description: "là một giá trị logíc: = 1 nếu trả vào đầu chu kỳ ; = 0 hoặc được bỏ qua nếu trả vào cuối chu kỳ"
			}
		]
	},
	{
		name: "POISSON",
		description: "Trả về phân bố Puat-sơn.",
		arguments: [
			{
				name: "x",
				description: "là số sự kiện"
			},
			{
				name: "mean",
				description: "là giá trị số mong muốn, một số dương"
			},
			{
				name: "cumulative",
				description: "là giá trị lô gíc: dành cho xác suất Puat-sơn tích lũy, dùng ĐÚNG; hàm xác suất số đông Puat-sơn, dùng SAI"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "Trả về phân bố Puat-sơn.",
		arguments: [
			{
				name: "x",
				description: "là số sự kiện"
			},
			{
				name: "mean",
				description: "là giá trị số mong muốn, một số dương"
			},
			{
				name: "cumulative",
				description: "là giá trị lô gíc: dành cho xác suất Puat-sơn tích lũy, dùng ĐÚNG; hàm xác suất số đông Puat-sơn, dùng SAI"
			}
		]
	},
	{
		name: "POWER",
		description: "Trả về giá trị hàm mũ của một số.",
		arguments: [
			{
				name: "number",
				description: "là số cơ số, bất kỳ số thực"
			},
			{
				name: "power",
				description: "là số mũ của cơ số"
			}
		]
	},
	{
		name: "PPMT",
		description: "Trả về khoản thanh toán vốn của một khoản đầu tư trên cơ sở thanh toán không đổi và lãi suất không đổi theo chu kỳ.",
		arguments: [
			{
				name: "rate",
				description: "là lãi suất theo chu kỳ. Ví dụ, khoản thanh toán hàng quý là 6% vào tháng tư sẽ dùng 6%/4"
			},
			{
				name: "per",
				description: "Chỉ định chu kỳ và phải nằm trong khoảng từ 1 tới SốChukỳ"
			},
			{
				name: "nper",
				description: "là tổng số chu kỳ thanh toàn cho một khoản đầu tư"
			},
			{
				name: "pv",
				description: "Trả về giá trị hiện thời: có tính luôn tổng của một chuỗi các khoản thanh toán tương lai"
			},
			{
				name: "fv",
				description: "là giá trị tương lai, hoặc cân đối tiền mặt cần thu được sau khi thực hiện khoản thanh toán cuối cùng"
			},
			{
				name: "type",
				description: "là giá trị lô gíc:=1 nếu là khoản thanh toán đầu chu kỳ; =0 hoặc không có nếu là khoản thanh toán cuối chu kỳ"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "Trả lại giá trên $100 mệnh giá của thế chấp giảm giá.",
		arguments: [
			{
				name: "settlement",
				description: "là ngày thanh khoản thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "maturity",
				description: "là ngày tới hạn thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "discount",
				description: "là tỷ suất giảm giá thế chấp"
			},
			{
				name: "redemption",
				description: "là giá trị chuộc thế chấp trên $100 mệnh giá"
			},
			{
				name: "basis",
				description: "là loại cơ sở tính ngày được dùng"
			}
		]
	},
	{
		name: "PROB",
		description: "Trả về xác suất của các giá trị trong khoảng hai giới hạn hoặc bằng giới hạn dưới.",
		arguments: [
			{
				name: "x_range",
				description: "là khoảng giá trị số của x cùng với xác suất của chúng"
			},
			{
				name: "prob_range",
				description: "là tập hợp xác suất gắn với các giá trị trong Khoảng_X, là giá trị từ 0 đến 1 không tính 0"
			},
			{
				name: "lower_limit",
				description: "là giới hạn dưới của giá trị cần lấy xác suất"
			},
			{
				name: "upper_limit",
				description: "là giới hạn trên tùy chọn của giá trị. Nếu không có, hàm PROB trả về xác suất của các giá trị Khoảng_X bằng Giới_hạn_dưới"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "Nhân tất cả các số là tham đối.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 số, giá trị lô-gic, hoặc văn bản thể hiện các số mà bạn muốn nhân"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 số, giá trị lô-gic, hoặc văn bản thể hiện các số mà bạn muốn nhân"
			}
		]
	},
	{
		name: "PROPER",
		description: "Chuyển đổi xâu văn bản sang dạng chữ thích hợp; Chữ cái đầu tiên của từ là chữ hoa, mọi chữ cái khác là chữ thường.",
		arguments: [
			{
				name: "text",
				description: "là văn bản bao trong dấu ngoặc kép, công thức trả về văn bản, hoặc tham chiếu tới ô chứa văn bản viết chữ hoa một phần"
			}
		]
	},
	{
		name: "PV",
		description: "Trả về giá trị hiện thời của khoản đầu tư: có tính luôn tổng của một chuỗi các khoản thanh toán tương lai.",
		arguments: [
			{
				name: "rate",
				description: "là lãi suất theo chu kỳ. Ví dụ, khoản thanh toán hàng quý là 6% vào tháng tư sẽ dùng 6%/4"
			},
			{
				name: "nper",
				description: "là tổng số chu kỳ thanh toán trong khoản đầu tư"
			},
			{
				name: "pmt",
				description: "là khoản thanh toán cho mỗi chu kỳ và không thay đổi trong suốt thời gian đầu tư"
			},
			{
				name: "fv",
				description: "là giá trị tương lai, hoặc cân đối tiền mặt cần thu được sau khi thực hiện khoản thanh toán cuối cùng"
			},
			{
				name: "type",
				description: "là giá trị lô gíc: =1 nếu là khoản thanh toán đầu chu kỳ; =0 hoặc không có nếu là khoản thanh toán cuối chu kỳ"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "Trả về tứ phân vị của tập dữ liệu.",
		arguments: [
			{
				name: "array",
				description: "là mảng hoặc khoảng giá trị số cần xác định giá trị tứ phân vị"
			},
			{
				name: "quart",
				description: "là một số: giá trị tối thiểu = 0; tứ phân vị đầu tiên= 1; giá trị giữa = 2; tứ phân vị thứ ba= 3; giá trị tối đa = 4"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "Trả về tứ phân vị của tập hợp dữ liệu, dựa trên giá trị phần trăm từ 0..1, bao gồm cả 0 và 1.",
		arguments: [
			{
				name: "array",
				description: "là mảng hoặc khoảng ô có giá trị số cần tính giá trị tứ phân vị"
			},
			{
				name: "quart",
				description: "là số: giá trị tối thiểu = 0; tứ phân vị thứ 1 = 1; giá trị trung bình = 2; tứ phân vị thứ 3 = 3; giá trị tối đa = 4"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "Trả về tứ phân vị của tập hợp dữ liệu, dựa trên giá trị phần trăm từ 0..1, bao gồm cả 0 và 1.",
		arguments: [
			{
				name: "array",
				description: "là mảng hoặc khoảng ô có giá trị số cần tính giá trị tứ phân vị"
			},
			{
				name: "quart",
				description: "là số: giá trị tối thiểu = 0; tứ phân vị thứ 1 = 1; giá trị trung bình = 2; tứ phân vị thứ 3 = 3; giá trị tối đa = 4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Trả về phần nguyên của phép chia.",
		arguments: [
			{
				name: "numerator",
				description: "là số bị chia"
			},
			{
				name: "denominator",
				description: "là số chia"
			}
		]
	},
	{
		name: "RADIANS",
		description: "Chuyển đổi độ sang radian.",
		arguments: [
			{
				name: "angle",
				description: "là góc tính theo độ cần chuyển đổi"
			}
		]
	},
	{
		name: "RAND",
		description: "Trả về số ngẫu nhiên phân bổ đều trong khoảng lớn hơn hoặc bằng 0 và nhỏ hơn 1 (thay đổi khi tính lại).",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "Trả lại số ngẫu nhiên giữa các số được chỉ định.",
		arguments: [
			{
				name: "bottom",
				description: "là số nguyên nhỏ nhất mà RANDBETWEEN sẽ trả lại"
			},
			{
				name: "top",
				description: "là số nguyên lớn nhất mà RANDBETWEEN sẽ trả lại"
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
		description: "Trả về cấp của số trong danh sách: là độ lớn tương đối của nó so với các giá trị khác trong danh sách.",
		arguments: [
			{
				name: "number",
				description: "là số cần tìm cấp"
			},
			{
				name: "ref",
				description: "là một mảng, hoặc tham chiếu tới danh sách các số. Không tính các giá trị khác số"
			},
			{
				name: "order",
				description: "là một số: =0 hoặc không có sẽ tính cấp số trong danh sách giảm dần; = giá trị bất kỳ khác không sẽ tính cấp số trong danh sách tăng dần"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "Trả về xếp hạng của số trong danh sách các số: kích cỡ của nó là tương đối với các giá trị khác trong danh sách; nếu có nhiều hơn một giá trị với cùng xếp hạng, sẽ trả về xếp hạng trung bình.",
		arguments: [
			{
				name: "number",
				description: "là số bạn muốn tìm xếp hạng"
			},
			{
				name: "ref",
				description: "là mảng của, hoặc tham chiếu tới, danh sách các số. Bỏ qua các giá trị không phải là số"
			},
			{
				name: "order",
				description: "là một số: xếp hạng trong danh sách sắp xếp giảm dần = 0 hoặc bỏ qua; xếp hạng trong danh sách sắp xếp tăng dần = giá trị khác không bất kỳ"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "Trả về xếp hạng của số trong danh sách các số: kích cỡ của nó là tương đối với các giá trị khác trong danh sách; nếu có nhiều hơn một giá trị với cùng xếp hạng, sẽ trả về xếp hạng trung bình.",
		arguments: [
			{
				name: "number",
				description: "là số bạn muốn tìm xếp hạng"
			},
			{
				name: "ref",
				description: "là mảng của, hoặc tham chiếu tới, danh sách các số. Bỏ qua các giá trị không phải là số"
			},
			{
				name: "order",
				description: "là một số: xếp hạng trong danh sách sắp xếp giảm dần = 0 hoặc bỏ qua; xếp hạng trong danh sách sắp xếp tăng dần = giá trị khác không bất kỳ"
			}
		]
	},
	{
		name: "RATE",
		description: "là lãi suất theo chu kỳ của khoản vay hoặc khoản đầu tư. Ví dụ, khoản thanh toán hàng quý là 6% vào tháng tư sẽ dùng 6%/4.",
		arguments: [
			{
				name: "nper",
				description: "là tổng số chu kỳ thanh toán cho khoản vay hoặc khoản đầu tư"
			},
			{
				name: "pmt",
				description: "là khoản thanh toán cho mỗi chu kỳ và không thay đổi trong suốt thời gian cho vay hoặc đầu tư"
			},
			{
				name: "pv",
				description: "là giá trị hiện thời: có tính luôn tổng của một chuỗi các khoản thanh toán tương lai"
			},
			{
				name: "fv",
				description: "là giá trị tương lai, hoặc cân đối tiền mặt cần thu được sau khi thực hiện khoản thanh toán cuối cùng. Nếu không có, dùng Fv = 0"
			},
			{
				name: "type",
				description: "là giá trị lô gíc: =1 nếu là khoản thanh toán đầu chu kỳ; =0 hoặc không có nếu là khoản thanh toán cuối chu kỳ"
			},
			{
				name: "guess",
				description: "là tỷ lệ được đoán trước; nếu không có, Đoán trước= 0.1 (10 phần trăm)"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "Trả lại số lượng sẽ nhận được lúc tới hạn cho thế chấp đầu tư đầy đủ.",
		arguments: [
			{
				name: "settlement",
				description: "là ngày thanh khoản thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "maturity",
				description: "là ngày tới hạn thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "investment",
				description: "là số lượng đầu tư vào thế chấp"
			},
			{
				name: "discount",
				description: "là tỷ suất giảm giá của thế chấp"
			},
			{
				name: "basis",
				description: "là loại cơ sở tính ngày được dùng"
			}
		]
	},
	{
		name: "REPLACE",
		description: "Thay thế một phần của xâu văn bản bằng xâu văn bản khác.",
		arguments: [
			{
				name: "old_text",
				description: "là văn bản cần thay thế một số ký tự"
			},
			{
				name: "start_num",
				description: "là vị trí của ký tự trong Văn_bản_cũ cần thay thế bằng Văn_bản_mới"
			},
			{
				name: "num_chars",
				description: "là số ký tự trong Văn_bản_cũ cần thay thế"
			},
			{
				name: "new_text",
				description: "là văn bản sẽ thay thế các ký tự trong Văn_bản_cũ"
			}
		]
	},
	{
		name: "REPT",
		description: "Lặp lại văn bản theo số lần chỉ định. Sử dụng hàm REPT để điền xâu văn bản nhiều lần vào ô.",
		arguments: [
			{
				name: "text",
				description: "là văn bản cần lặp lại"
			},
			{
				name: "number_times",
				description: "là một số dương chỉ định số lần lặp lại văn bản"
			}
		]
	},
	{
		name: "RIGHT",
		description: "Trả về số ký tự xác định từ vị trí cuối của xâu văn bản.",
		arguments: [
			{
				name: "text",
				description: "là xâu văn bản chứa các ký tự cần trích xuất"
			},
			{
				name: "num_chars",
				description: "Xác định số ký tự cần trích xuất; là 1 nếu không có"
			}
		]
	},
	{
		name: "ROMAN",
		description: "Chuyển đổi chữ số A-rập sang La mã, như văn bản.",
		arguments: [
			{
				name: "number",
				description: "là chữ số A-rập cần chuyển đổi"
			},
			{
				name: "form",
				description: "là số chỉ định kiểu chữ số La mã mong muốn."
			}
		]
	},
	{
		name: "ROUND",
		description: "Làm tròn số theo số chữ số được chỉ định.",
		arguments: [
			{
				name: "number",
				description: "là số cần làm tròn"
			},
			{
				name: "num_digits",
				description: "là số chữ số cần làm tròn. Nếu là số âm làm tròn phần bên trái dấu thập phân, là số 0 làm tròn đến giá trị nguyên gần nhất"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "Làm tròn số xuống, tiến tới không.",
		arguments: [
			{
				name: "number",
				description: "là số thực cần làm tròn xuống"
			},
			{
				name: "num_digits",
				description: "là số chữ số cần làm tròn. Nếu là số âm làm tròn phần bên trái dấu thập phân, là số 0 hoặc không có làm tròn đến giá trị nguyên gần nhất"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "Làm tròn số lên, xa khỏi không.",
		arguments: [
			{
				name: "number",
				description: "là số thực cần làm tròn lên"
			},
			{
				name: "num_digits",
				description: "là số chữ số cần làm tròn. Nếu là số âm làm tròn phần bên trái dấu thập phân, là số 0 hoặc không có làm tròn đến giá trị nguyên gần nhất"
			}
		]
	},
	{
		name: "ROW",
		description: "Trả về số hiệu hàng của tham chiếu.",
		arguments: [
			{
				name: "reference",
				description: "là một ô hoặc một vùng ô đơn bạn cần có số hiệu hàng; Nếu không có, trả về ô trong hàm HÀNG"
			}
		]
	},
	{
		name: "ROWS",
		description: "Trả về số hàng trong một tham chiếu hoặc mảng.",
		arguments: [
			{
				name: "array",
				description: "là một mảng, công thức mảng, hoặc tham chiếu tới một khoảng các ô cần số hàng"
			}
		]
	},
	{
		name: "RRI",
		description: "Trả về lãi suất tương ứng cho sự tăng trưởng của khoản đầu tư.",
		arguments: [
			{
				name: "nper",
				description: "là số lượng chu kỳ của khoản đầu tư"
			},
			{
				name: "pv",
				description: "là giá trị hiện tại của khoản đầu tư"
			},
			{
				name: "fv",
				description: "là giá trị tương lai của khoản đầu tư"
			}
		]
	},
	{
		name: "RSQ",
		description: "Trả về bình phương hệ số tương quan mômen tích Pia-sơn từ các điểm dữ liệu đã cho.",
		arguments: [
			{
				name: "known_y's",
				description: "là mảng hoặc khoảng các điểm dữ liệu và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số"
			},
			{
				name: "known_x's",
				description: "là mảng hoặc khoảng các điểm dữ liệu và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số"
			}
		]
	},
	{
		name: "RTD",
		description: "Trích xuất dữ liệu thời gian thực từ chương trình hỗ trợ COM tự động.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "là tên của NhậndiệnChươngTrình của bổ trợ COM tự động đã đăng ký. Tên được bao trong dấu ngoặc kép"
			},
			{
				name: "server",
				description: "là tên của máy chủ mà các bổ trợ cần chạy. Tên được bao trong dấu ngoặc kép. Nếu bổ trợ chạy tại chỗ, dùng xâu rỗng"
			},
			{
				name: "topic1",
				description: "từ 1 đến 38 tham số chỉ định một phần của dữ liệu"
			},
			{
				name: "topic2",
				description: "từ 1 đến 38 tham số chỉ định một phần của dữ liệu"
			}
		]
	},
	{
		name: "SEARCH",
		description: "Trả về số ký tự tại vị trí xuất hiện đầu tiên của ký tự hoặc xâu văn bản cho trước, tính từ trái qua phải (không phân biệt chữ hoa, chữ thường).",
		arguments: [
			{
				name: "find_text",
				description: "là văn bản cần tìm. Có thể sử dụng ký tự đại diện ? và *; sử dụng ~? và ~* để tìm ký tự ? và *"
			},
			{
				name: "within_text",
				description: "là văn bản, trong đó cần tìm Văn_bản_tìm"
			},
			{
				name: "start_num",
				description: "là số thứ tự ký tự trong Văn_bản_chứa, tính từ trái sang, cần bắt đầu tìm kiếm. Nếu không có, bắt đầu từ 1"
			}
		]
	},
	{
		name: "SEC",
		description: "Trả về hàm sec của một góc.",
		arguments: [
			{
				name: "number",
				description: "là số đo góc theo rađian mà bạn muốn tìm sec"
			}
		]
	},
	{
		name: "SECH",
		description: "Trả về hàm sec hyperbol của một góc.",
		arguments: [
			{
				name: "number",
				description: "là số đo góc theo rađian mà bạn muốn tìm sec hyperbol"
			}
		]
	},
	{
		name: "SECOND",
		description: "Trả về giây, một số từ 0 đến 59.",
		arguments: [
			{
				name: "serial_number",
				description: "là một số theo mã ngày tháng của Spreadsheet hoặc văn bản theo dạng thức thời gian, như 16:48:23 hoặc 4:48:47 CH"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "Trả về tổng chuỗi lũy thừa theo công thức.",
		arguments: [
			{
				name: "x",
				description: "là giá trị  nhập vào cho chuỗi lũy thừa"
			},
			{
				name: "n",
				description: "là lũy thừa khởi tạo để tăng tới x"
			},
			{
				name: "m",
				description: "là bước tăng cho mỗi phần trong chuỗi"
			},
			{
				name: "coefficients",
				description: "là tập hợp hệ số sẽ được nhân với mỗi lũy thừa của x"
			}
		]
	},
	{
		name: "SHEET",
		description: "Trả về số của trang tính của trang tính được tham chiếu.",
		arguments: [
			{
				name: "value",
				description: "là tên của trang tính hoặc tham chiếu bạn đang muốn tìm số của trang tính. Nếu bỏ qua, sẽ trả về số của trang tính có chứa hàm này"
			}
		]
	},
	{
		name: "SHEETS",
		description: "Trả về số lượng trang tính trong một tham chiếu.",
		arguments: [
			{
				name: "reference",
				description: "là tham chiếu mà bạn muốn biết nó bao gồm bao nhiêu trang tính. Nếu bỏ qua, sẽ trả về số lượng trang tính có trong sổ làm việc"
			}
		]
	},
	{
		name: "SIGN",
		description: "Trả về dấu của số: 1 nếu số dương, 0 nếu là số 0, hoặc -1 nếu số âm.",
		arguments: [
			{
				name: "number",
				description: "là số thực bất kỳ"
			}
		]
	},
	{
		name: "SIN",
		description: "Trả về Sin của góc.",
		arguments: [
			{
				name: "number",
				description: "là giá trị Sin của góc theo Radian. Độ * PI()/180 = Radian"
			}
		]
	},
	{
		name: "SINH",
		description: "Trả về Sin hi-péc-bôn của một số.",
		arguments: [
			{
				name: "number",
				description: "là số thực bất kỳ"
			}
		]
	},
	{
		name: "SKEW",
		description: "Trả về độ xiên của phân bố: đặc trưng mức độ mất đối xứng của phân bố xung quanh trung bình của nó.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 số hoặc tên, mảng, hoặc tham chiếu chứa số cần tính độ xiên"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 số hoặc tên, mảng, hoặc tham chiếu chứa số cần tính độ xiên"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "Trả về độ xiên của phân phối dựa trên một tập mẫu: đặc tính của mức độ bất đối xứng của một phân phối xoay quanh giá trị trung bình.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "là từ 1 đến 254 số lượng tên, mảng, hoặc tham chiếu mà có chứa con số bạn muốn tìm độ xiên của tập mẫu"
			},
			{
				name: "number2",
				description: "là từ 1 đến 254 số lượng tên, mảng, hoặc tham chiếu mà có chứa con số bạn muốn tìm độ xiên của tập mẫu"
			}
		]
	},
	{
		name: "SLN",
		description: "Trả về khấu hao đều của tài sản cho một chu kỳ.",
		arguments: [
			{
				name: "cost",
				description: "là chi phí ban đầu của tài sản"
			},
			{
				name: "salvage",
				description: "là giá trị còn lại ở cuối vòng đời tài sản"
			},
			{
				name: "life",
				description: "là số chu kỳ khấu hao của tài sản (đôi khi được gọi là vòng đời của tài sản)"
			}
		]
	},
	{
		name: "SLOPE",
		description: "Trả về độ dốc của đường hồi quy tuyến tính từ các điểm dữ liệu đã cho.",
		arguments: [
			{
				name: "known_y's",
				description: "là mảng hoặc khoảng ô các điểm dữ liệu số phụ thuộc và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số"
			},
			{
				name: "known_x's",
				description: "là tập hợp các điểm dữ liệu độc lập và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số"
			}
		]
	},
	{
		name: "SMALL",
		description: "Trả về giá trị lớn thứ k trong tập dữ liệu. Ví dụ, số lớn thứ năm.",
		arguments: [
			{
				name: "array",
				description: "là mảng hoặc khoảng dữ liệu số cần xác định giá trị lớn thứ k"
			},
			{
				name: "k",
				description: "là vị trí (từ giá trị nhỏ nhất) của một giá trị trong mảng hoặc khoảng ô trả về"
			}
		]
	},
	{
		name: "SQRT",
		description: "Trả về căn bậc hai của một số.",
		arguments: [
			{
				name: "number",
				description: "là số cần lấy căn bậc hai"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "Trả về căn bậc hai (số * Pi).",
		arguments: [
			{
				name: "number",
				description: "là số nhân với p"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "Trả về giá trị chuẩn hóa từ phân bố đặc trưng bởi trung bình và độ lệch tiêu chuẩn.",
		arguments: [
			{
				name: "x",
				description: "là giá trị cần chuẩn hoá"
			},
			{
				name: "mean",
				description: "là trung bình số học của phân bố"
			},
			{
				name: "standard_dev",
				description: "là độ lệch tiêu chuẩn của phân bố, một số dương"
			}
		]
	},
	{
		name: "STDEV",
		description: "Ước lượng độ lệch tiêu chuẩn dựa trên mẫu (không tính giá trị lô-gic và văn bản trong mẫu).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 số tương ứng với mẫu tập toàn bộ, có thể là số hoặc tham chiếu chứa số"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 số tương ứng với mẫu tập toàn bộ, có thể là số hoặc tham chiếu chứa số"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "Tính toán độ lệch tiêu chuẩn dựa trên tập toàn bộ là các tham đối (không tính giá trị lô gíc và văn bản).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 số tương ứng với tập toàn bộ và có thể là số hoặc tham chiếu chứa số"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 số tương ứng với tập toàn bộ và có thể là số hoặc tham chiếu chứa số"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "Đoán nhận độ lệch tiêu chuẩn dựa trên mẫu (không tính giá trị lô gíc và văn bản trong mẫu).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 số tương ứng với mẫu tập toàn bộ, có thể là số hoặc tham chiếu chứa số"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 số tương ứng với mẫu tập toàn bộ, có thể là số hoặc tham chiếu chứa số"
			}
		]
	},
	{
		name: "STDEVA",
		description: "Ước đoán độ lệch tiêu chuẩn dựa trên mẫu, tính cả giá trị lô gíc và văn bản. Văn bản và giá trị lô gíc SAI có giá trị 0; giá trị lô gíc ĐÚNG có giá trị 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "từ 1 đến 255 giá trị tương ứng với một mẫu của tập toàn bộ và có thể là giá trị, tên hoặc tham chiếu tới giá trị"
			},
			{
				name: "value2",
				description: "từ 1 đến 255 giá trị tương ứng với một mẫu của tập toàn bộ và có thể là giá trị, tên hoặc tham chiếu tới giá trị"
			}
		]
	},
	{
		name: "STDEVP",
		description: "Tính toán độ lệch tiêu chuẩn dựa trên tập toàn bộ là các tham đối (không tính giá trị lô-gic và văn bản).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 số tương ứng với tập toàn bộ và có thể là số hoặc tham chiếu chứa số"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 số tương ứng với tập toàn bộ và có thể là số hoặc tham chiếu chứa số"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Tính toán độ lệch tiêu chuẩn dựa trên tập toàn bộ, tính cả giá trị lô gíc và văn bản. Văn bản và giá trị lô gíc SAI có giá trị 0; giá trị lô gíc ĐÚNG có giá trị 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "từ 1 đến 255 giá trị tương ứng với một tập toàn bộ và có thể là giá trị, tên, mảng, hoặc tham chiếu chứa giá trị"
			},
			{
				name: "value2",
				description: "từ 1 đến 255 giá trị tương ứng với một tập toàn bộ và có thể là giá trị, tên, mảng, hoặc tham chiếu chứa giá trị"
			}
		]
	},
	{
		name: "STEYX",
		description: "Trả về độ sai chuẩn của giá trị y ước đoán cho mỗi giá trị x trong hồi quy.",
		arguments: [
			{
				name: "known_y's",
				description: "là mảng hoặc khoảng các điểm dữ liệu phụ thuộc và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số"
			},
			{
				name: "known_x's",
				description: "là mảng hoặc khoảng các điểm dữ liệu độc lập và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "Thay thế văn bản hiện thời bằng văn bản mới trong xâu văn bản.",
		arguments: [
			{
				name: "text",
				description: "là văn bản hoặc tham chiếu tới ô chứa văn bản cần thay thế các ký tự"
			},
			{
				name: "old_text",
				description: "là văn bản hiện thời cần thay thế. Nếu dạng chữ của Văn_bản_cũ không đúng với dạng chữ của văn bản, hàm SUBSTITUTE sẽ không thay thế văn bản"
			},
			{
				name: "new_text",
				description: "là văn bản thay thế cho Văn_bản_cũ"
			},
			{
				name: "instance_num",
				description: "xác định xuất hiện nào của Văn_bản_cũ sẽ được thay thế. Nếu không có, mọi lần xuất hiện của Văn_bản_cũ sẽ được thay thế"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Trả về tổng con trong danh sách hoặc cơ sở dữ liệu.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "function_num",
				description: "là số từ 1 đến 11 chỉ định hàm tổng hợp cho tổng con"
			},
			{
				name: "ref1",
				description: "từ 1 đến 254 khoảng hoặc tham chiếu cần tính tổng con"
			}
		]
	},
	{
		name: "SUM",
		description: "Cộng tất cả các số trong phạm vi ô.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 số để tính tổng. Không tính giá trị lô-gic và văn bản, trừ phi nó là tham đối"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 số để tính tổng. Không tính giá trị lô-gic và văn bản, trừ phi nó là tham đối"
			}
		]
	},
	{
		name: "SUMIF",
		description: "Cộng các ô chỉ định theo điều kiện hoặc tiêu chí cho trước.",
		arguments: [
			{
				name: "range",
				description: "là khoảng các ô cần đánh giá"
			},
			{
				name: "criteria",
				description: "là điều kiện hoặc tiêu chí dưới dạng số, biểu thức, hoặc văn bản xác định ô nào được thêm vào"
			},
			{
				name: "sum_range",
				description: "là các ô thật sự tính tổng. Nếu không có, sử dụng các ô trong khoảng"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "Thêm ô được chỉ ra bởi tập cho trước các điều kiện hay chỉ tiêu.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sum_range",
				description: "là các ô thực tế cần cộng."
			},
			{
				name: "criteria_range",
				description: "là dải các ô muốn đánh giá theo điều kiện cụ thể"
			},
			{
				name: "criteria",
				description: "là điều kiện ở dạng số, biểu thức, hay văn bản xác định ô nào sẽ được thêm vào"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "Trả về tổng tích của các khoảng hoặc mảng tương ứng.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "array1",
				description: "từ 2 đến 255 mảng cần nhân sau đó cộng các thành phần. Tất cả các mảng phải có cùng kích cỡ"
			},
			{
				name: "array2",
				description: "từ 2 đến 255 mảng cần nhân sau đó cộng các thành phần. Tất cả các mảng phải có cùng kích cỡ"
			},
			{
				name: "array3",
				description: "từ 2 đến 255 mảng cần nhân sau đó cộng các thành phần. Tất cả các mảng phải có cùng kích cỡ"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "Trả về tổng bình phương các tham đối. Tham đối có thể là số, mảng, tên hoặc tham chiếu tới ô chứa số.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 số, mảng, tên hoặc tham chiếu tới mảng cần tính tổng bình phương"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 số, mảng, tên hoặc tham chiếu tới mảng cần tính tổng bình phương"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "Tổng độ lệch giữa bình phương trong hai khoảng hoặc mảng tương ứng.",
		arguments: [
			{
				name: "array_x",
				description: "là khoảng hay mảng số thứ nhất và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số"
			},
			{
				name: "array_y",
				description: "là khoảng hay mảng số thứ hai và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "Trả về tổng cộng của tổng bình phương các số trong hai khoảng hoặc mảng tương ứng.",
		arguments: [
			{
				name: "array_x",
				description: "là khoảng hay mảng số thứ nhất và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số"
			},
			{
				name: "array_y",
				description: "là khoảng hay mảng số thứ hai và có thể là số hoặc tên, mảng, hoặc tham chiếu chứa số"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "Tổng bình phương độ lệch trong hai khoảng hoặc mảng tương ứng.",
		arguments: [
			{
				name: "array_x",
				description: "là khoảng hay mảng giá trị thứ nhất và có thể là số, mảng, hoặc tham chiếu chứa số"
			},
			{
				name: "array_y",
				description: "là khoảng hay mảng giá trị thứ hai và có thể là số, mảng, hoặc tham chiếu chứa số"
			}
		]
	},
	{
		name: "SYD",
		description: "Trả về số khấu hao tổng cả năm của tài sản cho một chu kỳ xác định.",
		arguments: [
			{
				name: "cost",
				description: "là chi phí ban đầu của tài sản"
			},
			{
				name: "salvage",
				description: "là giá trị còn lại ở cuối vòng đời tài sản"
			},
			{
				name: "life",
				description: "là số chu kỳ khấu hao của tài sản (đôi khi được gọi là vòng đời của tài sản)"
			},
			{
				name: "per",
				description: "là chu kỳ và phải dùng cùng đơn vị tính như Vòng đời"
			}
		]
	},
	{
		name: "T",
		description: "Kiểm tra một giá trị có phải là văn bản, và trả về văn bản nếu đúng, hoặc cặp dấu nháy kép (văn bản rỗng) nếu sai.",
		arguments: [
			{
				name: "value",
				description: "là giá trị cần thử"
			}
		]
	},
	{
		name: "T.DIST",
		description: "Trả về phân phối Student-t có phần dư bên trái.",
		arguments: [
			{
				name: "x",
				description: "là giá trị số để đánh giá phân phối"
			},
			{
				name: "deg_freedom",
				description: "là số nguyên cho biết số bậc tự do đặc trưng cho phân phối"
			},
			{
				name: "cumulative",
				description: "là giá trị lôgic: đối với hàm phân phối tích lũy, sử dụng TRUE; đối với hàm mật độ xác suất, sử dụng FALSE"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "Trả về phân bố t Student hai nhánh.",
		arguments: [
			{
				name: "x",
				description: "là giá trị số tại đó đánh giá phân bố"
			},
			{
				name: "deg_freedom",
				description: "là một số nguyên chỉ ra số bậc tự do để phân loại phân bố"
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "Trả về phân bố t Student nhánh phải.",
		arguments: [
			{
				name: "x",
				description: "là giá trị số tại đó đánh giá phân bố"
			},
			{
				name: "deg_freedom",
				description: "là một số nguyên chỉ ra số bậc tự do để phân loại phân bố"
			}
		]
	},
	{
		name: "T.INV",
		description: "Trả về nghịch đảo phần dư bên trái của phân phối Student t.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất liên quan đến phân phối Student t hai phần dư, một số từ 0 đến 1 và bao gồm cả 0 và 1"
			},
			{
				name: "deg_freedom",
				description: "là số nguyên dương cho biết số bậc tự do đặc trưng cho phân phối"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Trả về nghịch đảo phần dư bên trái của phân phối Student t.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất liên quan đến phân phối Student t hai phần dư, một số từ 0 đến 1 và bao gồm cả 0 và 1"
			},
			{
				name: "deg_freedom",
				description: "là số nguyên dương cho biết số bậc tự do đặc trưng cho phân phối"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Trả về xác suất gắn với kiểm định t Student.",
		arguments: [
			{
				name: "array1",
				description: "là tập dữ liệu đầu tiên"
			},
			{
				name: "array2",
				description: "là tập hợp dữ liệu thứ hai"
			},
			{
				name: "tails",
				description: "chỉ định số phần dư của phân bố được trả về: phân bố một phần dư = 1; phân bố hai phần dư = 2"
			},
			{
				name: "type",
				description: "là kiểu của kiểm định t: theo cặp = 1, phương sai ngang nhau cho hai mẫu (phương sai có điều kiện không đổi) = 2, phương sai không ngang nhau cho hai mẫu = 3"
			}
		]
	},
	{
		name: "TAN",
		description: "Trả về Tang của góc.",
		arguments: [
			{
				name: "number",
				description: "là giá trị Tang của góc theo Radian. Độ * PI()/180 = Radian"
			}
		]
	},
	{
		name: "TANH",
		description: "Trả về Tang hi-péc-bôn của một số.",
		arguments: [
			{
				name: "number",
				description: "là số thực bất kỳ"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "Trả lại thu hồi trái phiếu đổi ngang cho trái phiếu kho bạc.",
		arguments: [
			{
				name: "settlement",
				description: "là ngày thanh khoản trái phiếu kho bạc, thể hiện như số ngày tuần tự"
			},
			{
				name: "maturity",
				description: "là ngày tới hạn trái phiếu kho bạc, thể hiện như số ngày tuần tự"
			},
			{
				name: "discount",
				description: "là tỷ suất giảm giá trái phiếu kho bạc"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "Trả lại giá trên $100 mệnh giá cho trái phiếu kho bạc.",
		arguments: [
			{
				name: "settlement",
				description: "là ngày thanh khoản trái phiếu kho bạc, thể hiện như số ngày tuần tự"
			},
			{
				name: "maturity",
				description: "là ngày tới hạn trái phiếu kho bạc, thể hiện như số ngày tuần tự"
			},
			{
				name: "discount",
				description: "là tỷ suất giảm giá trái phiếu kho bạc"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "Trả lại thu hồi cho trái phiếu kho bạc.",
		arguments: [
			{
				name: "settlement",
				description: "là ngày thanh khoản trái phiếu kho bạc, thể hiện như số ngày tuần tự"
			},
			{
				name: "maturity",
				description: "là ngày tới hạn trái phiếu kho bạc, thể hiện như số ngày tuần tự"
			},
			{
				name: "pr",
				description: "là giá trái phiếu kho bạc trên $100 mệnh giá"
			}
		]
	},
	{
		name: "TDIST",
		description: "Trả về phân bố t Student.",
		arguments: [
			{
				name: "x",
				description: "là giá trị số cần tính phân bố"
			},
			{
				name: "deg_freedom",
				description: "là số nguyên chỉ số bậc tự do đặc trưng của phân bố"
			},
			{
				name: "tails",
				description: "chỉ định số phần dư của phân bố được trả về: phân bố một phần dư = 1; phân bố hai phần dư = 2"
			}
		]
	},
	{
		name: "TEXT",
		description: "Chuyển đổi một giá trị sang văn bản theo dạng thức số.",
		arguments: [
			{
				name: "value",
				description: "là một số, một công thức có kết quả số, hoặc một tham chiếu tới ô chứa giá trị số"
			},
			{
				name: "format_text",
				description: "là một số dưới dạng thức văn bản từ hộp Thể loại trên tap Số trong hộp hội thoại Định dạng Ô (không phải là Chung)"
			}
		]
	},
	{
		name: "TIME",
		description: "Chuyển đổi giờ, phút, giây đã cho sang số tuần tự của Spreadsheet, được định dạng theo dạng thức thời gian.",
		arguments: [
			{
				name: "hour",
				description: "là một số từ 0 đến 23 để biểu diễn giờ"
			},
			{
				name: "minute",
				description: "là một số từ 0 đến 59 để biểu diễn phút"
			},
			{
				name: "second",
				description: "là một số từ 0 đến 59 để biểu diễn giây"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "Chuyển đổi văn bản thời gian sang số tuần tự trong Spreadsheet chỉ thời gian, một số từ (12:00:00 SA) đến 0.999988426 (11:59:59 CH). Định dạng số theo dạng thức thời gian sau khi nhập công thức.",
		arguments: [
			{
				name: "time_text",
				description: "là xâu văn bản chỉ thời gian theo bất kỳ dạng thức thời gian nào trong Spreadsheet (bỏ qua thông tin ngày tháng trong xâu)"
			}
		]
	},
	{
		name: "TINV",
		description: "Trả về giá trị đảo của phân bố t Student.",
		arguments: [
			{
				name: "probability",
				description: "là xác suất gắn với phân bố t Student hai phần dư, một số trong khoảng từ 0 đến 1"
			},
			{
				name: "deg_freedom",
				description: "là số nguyên dương chỉ thị số bặc tự do đặc trưng cho phân bố"
			}
		]
	},
	{
		name: "TODAY",
		description: "Trả về ngày hiện thời theo dạng thức ngày tháng.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "Chuyển một dãy dọc các ô sang dãy ngang, hoặc ngược lại,.",
		arguments: [
			{
				name: "array",
				description: "là một dãy ô trên một trang tính hoặc một mảng giá trị mà bạn muốn hoán đổi"
			}
		]
	},
	{
		name: "TREND",
		description: "Trả về các số trong xu hướng tuyến tính phù hợp với các điểm dữ liệu, dùng phương pháp bình phương nhỏ nhất.",
		arguments: [
			{
				name: "known_y's",
				description: "là một khoảng hoặc mảng các giá trị y xác định theo quan hệ y = mx + b"
			},
			{
				name: "known_x's",
				description: "là một khoảng hoặc mảng tùy chọn các giá trị x xác định theo quan hệ y = mx + b, mảng có cùng kích cỡ như y-xác định"
			},
			{
				name: "new_x's",
				description: "là một khoảng hoặc mảng các giá trị x mới cần cho hàm TREND trả về các giá trị y tương ứng"
			},
			{
				name: "const",
				description: "là giá trị lô gíc: hằng số b được tính toán bình thường nếu Hằng số=ĐÚNG hoặc không có; b đặt bằng 0 nếu Hằng số=SAI"
			}
		]
	},
	{
		name: "TRIM",
		description: "Loại bỏ mọi dấu cách trong xâu văn bản ngoại trừ dấu cách đơn giữa các từ.",
		arguments: [
			{
				name: "text",
				description: "là văn bản cần loại bỏ dấu cách"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "Trả về trung bình phần bên trong của tập giá trị dữ liệu.",
		arguments: [
			{
				name: "array",
				description: "là khoảng hoặc mảng các giá trị cần thu gọn và tính trung bình"
			},
			{
				name: "percent",
				description: "là phân đoạn các điểm dữ liệu cần loại trừ khỏi phần trên và dưới của tập dữ liệu"
			}
		]
	},
	{
		name: "TRUE",
		description: "Trả về giá trị lô-gic ĐÚNG.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "Rút ngắn số thành số nguyên bằng cách loại bỏ phần thập phân, hoặc phần phân số của nó.",
		arguments: [
			{
				name: "number",
				description: "là số cần rút ngắn"
			},
			{
				name: "num_digits",
				description: "là số chỉ độ chính xác của việc rút ngắn, bằng 0 (không) nếu không có"
			}
		]
	},
	{
		name: "TTEST",
		description: "Trả về xác suất gắn với kiểm định t Student.",
		arguments: [
			{
				name: "array1",
				description: "là tập dữ liệu đầu tiên"
			},
			{
				name: "array2",
				description: "là tập hợp dữ liệu thứ hai"
			},
			{
				name: "tails",
				description: "chỉ định số phần dư của phân bố được trả về: phân bố một phần dư = 1; phân bố hai phần dư = 2"
			},
			{
				name: "type",
				description: "là kiểu của kiểm định t: theo cặp = 1, phương sai ngang nhau cho hai mẫu (phương sai có điều kiện không đổi) = 2, phương sai không ngang nhau cho hai mẫu = 3"
			}
		]
	},
	{
		name: "TYPE",
		description: "Trả về một số nguyên đại diện loại dữ liệu của một giá trị: số = 1; văn bản = 2; giá trị lô-gic = 4; giá trị lỗi = 16; mảng = 64.",
		arguments: [
			{
				name: "value",
				description: "có thể là bất kỳ giá trị nào"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Trả về con số (điểm mã) tương ứng với ký tự đầu tiên của đoạn văn bản.",
		arguments: [
			{
				name: "text",
				description: "là ký tự bạn muốn tìm giá trị Unicode của nó"
			}
		]
	},
	{
		name: "UPPER",
		description: "Chuyển đổi xâu văn bản sang chữ hoa.",
		arguments: [
			{
				name: "text",
				description: "là văn bản cần đổi sang chữ hoa, một tham chiếu hoặc xâu văn bản"
			}
		]
	},
	{
		name: "VALUE",
		description: "Chuyển đổi xâu văn bản thể hiện số thành số.",
		arguments: [
			{
				name: "text",
				description: "là văn bản nằm trong dấu ngoặc kép hoặc một tham chiếu tới ô chứa văn bản cần chuyển đổi"
			}
		]
	},
	{
		name: "VAR",
		description: "Ước lượng phương sai dựa trên mẫu (không tính giá trị lô-gic và văn bản trong mẫu).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 tham đối dạng số tương ứng với mẫu tập toàn bộ"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 tham đối dạng số tương ứng với mẫu tập toàn bộ"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Tính toán dung sai trên cơ sở tập toàn bộ (không tính các giá trị lô gíc và văn bản trong tập toàn bộ).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 tham đối dạng số tương ứng với tập toàn bộ"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 tham đối dạng số tương ứng với tập toàn bộ"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Ước lượng phương sai dựa trên mẫu (không tính giá trị lô gíc và văn bản trong mẫu).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 tham đối dạng số tương ứng với mẫu tập toàn bộ"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 tham đối dạng số tương ứng với mẫu tập toàn bộ"
			}
		]
	},
	{
		name: "VARA",
		description: "Ước đoán độ lệch tiêu chuẩn dựa trên mẫu, tính cả giá trị lô gíc và văn bản. Văn bản và giá trị lô gíc SAI có giá trị 0; giá trị lô gíc ĐÚNG có giá trị 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "từ 1 đến 255 giá trị tham đối tương ứng với một mẫu của tập toàn bộ"
			},
			{
				name: "value2",
				description: "từ 1 đến 255 giá trị tham đối tương ứng với một mẫu của tập toàn bộ"
			}
		]
	},
	{
		name: "VARP",
		description: "Tính toán dung sai trên cơ sở tập toàn bộ (không tính các giá trị lô-gic và văn bản trong tập toàn bộ).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "từ 1 đến 255 tham đối dạng số tương ứng với tập toàn bộ"
			},
			{
				name: "number2",
				description: "từ 1 đến 255 tham đối dạng số tương ứng với tập toàn bộ"
			}
		]
	},
	{
		name: "VARPA",
		description: "Tính toán phương sai dựa trên tập toàn bộ, tính cả giá trị lô gíc và văn bản. Văn bản và giá trị lô gíc SAI có giá trị 0; giá trị lô gíc ĐÚNG có giá trị 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "từ 1 đến 255 giá trị tham đối tương ứng với một tập toàn bộ"
			},
			{
				name: "value2",
				description: "từ 1 đến 255 giá trị tham đối tương ứng với một tập toàn bộ"
			}
		]
	},
	{
		name: "VDB",
		description: "Trả về khấu hao của tài sản cho bất kỳ chu kỳ được chỉ định, tính cả một phần chu kỳ, sử dụng phương pháp giảm dần kép hoặc một số phương pháp xác định khác.",
		arguments: [
			{
				name: "cost",
				description: "là chi phí ban đầu của tài sản"
			},
			{
				name: "salvage",
				description: "là giá trị còn lại vào cuối vòng đời của tài sản"
			},
			{
				name: "life",
				description: "là số chu kỳ khấu hao của tài sản (đôi khi được gọi là vòng đời của tài sản)"
			},
			{
				name: "start_period",
				description: "là chu kỳ bắt đầu tính khấu hao, và phải dùng cùng đơn vị tính như Vòng đời"
			},
			{
				name: "end_period",
				description: "là chu kỳ cuối cùng tính khấu hao, và phải dùng cùng đơn vị tính như Vòng đời"
			},
			{
				name: "factor",
				description: "là tốc độ giảm dần, nếu không có được gán bằng 2 (phương pháp giảm dần kép)"
			},
			{
				name: "no_switch",
				description: "chuyển sang khấu hao đều khi khấu hao lớn hơn khấu hao giảm dần nếu = SAI hoặc không có; không chuyển nếu = ĐÚNG"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "Tìm giá trị trong cột bên trái nhất của bảng, và trả về giá trị cùng hàng từ cột chỉ định. Mặc định, bảng được sắp xếp theo trật tự tăng dần.",
		arguments: [
			{
				name: "lookup_value",
				description: "là giá trị tìm thấy trong cột đầu tiên của bảng và có thể là giá trị, tham chiếu,hoặc xâu văn bản"
			},
			{
				name: "table_array",
				description: "là một bảng các văn bản, số, hoặc giá trị lô gíc chứa dữ liệu cần truy xuất. Bảng_mảng có thể là tham chiếu tới khoảng hoặc tên khoảng"
			},
			{
				name: "col_index_num",
				description: "là số hiệu cột của giá trị trả về phù hợp trong Bảng_mảng. Cột giá trị đầu tiên trong bảng là cột 1"
			},
			{
				name: "range_lookup",
				description: "là giá trị lô-gic: =ĐÚNG hoặc không có nếu cần tìm phù hợp nhất trong cột đầu tiên (sắp xếp theo trật tự tăng dần); =SAI nếu cần tìm khớp"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "Trả về một số từ 1 đến 7 thể hiện ngày trong tuần.",
		arguments: [
			{
				name: "serial_number",
				description: "là một số biểu diễn ngày tháng"
			},
			{
				name: "return_type",
				description: "là một số: dùng 1, nếu Chủ nhật=1 tới thứ Bảy=7; dùng 2, nếu thứ Hai=1 tới Chủ nhật=7; dùng 3, nếu thứ Hai=0 tới Chủ nhật=6"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "Trả lại số của tuần trong năm.",
		arguments: [
			{
				name: "serial_number",
				description: "là mã ngày-tháng-thời-gian được Spreadsheet dùng để tính toán ngày tháng và thời gian"
			},
			{
				name: "return_type",
				description: "là số (1 hay 2) dùng để xác định loại giá trị trả lại"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Trả về phân bố Weibull.",
		arguments: [
			{
				name: "x",
				description: "là giá trị đánh giá của hàm, một số không âm"
			},
			{
				name: "alpha",
				description: "là tham biến của phân bố, một số dương"
			},
			{
				name: "beta",
				description: "là tham biến của phân bố, một số dương"
			},
			{
				name: "cumulative",
				description: "là giá trị lô gíc: dành cho hàm phân bố tích lũy, dùng ĐÚNG; hàm xác suất số đông, dùng SAI"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "Trả về phân bố Weibull.",
		arguments: [
			{
				name: "x",
				description: "là giá trị đánh giá của hàm, một số không âm"
			},
			{
				name: "alpha",
				description: "là tham biến của phân bố, một số dương"
			},
			{
				name: "beta",
				description: "là tham biến của phân bố, một số dương"
			},
			{
				name: "cumulative",
				description: "là giá trị lô gíc: dành cho hàm phân bố tích lũy, dùng ĐÚNG; hàm xác suất số đông, dùng SAI"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "Trả lại số tuần tự của ngày trước hay sau một số ngày làm việc được chỉ ra.",
		arguments: [
			{
				name: "start_date",
				description: "là số ngày tuần tự thể hiện ngày bắt đầu"
			},
			{
				name: "days",
				description: "là số ngày không phải cuối tuần hay ngày lễ trước hay sau start_date"
			},
			{
				name: "holidays",
				description: "là mảng tùy chọn của một hay nhiều số ngày tuần tự để loại khỏi lịch ngày làm việc như ngày lễ của tỉnh hay quốc gia và các ngày nghỉ lễ bù"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "Trả về số ngày tuần tự trước hoặc sau số ngày làm việc được chỉ định với tham số ngày cuối tuần tùy chỉnh.",
		arguments: [
			{
				name: "start_date",
				description: "là số ngày tuần tự thể hiện ngày bắt đầu"
			},
			{
				name: "days",
				description: "là số ngày không phải cuối tuần và ngày nghỉ trước hoặc sau start_date"
			},
			{
				name: "weekend",
				description: "là số hoặc chuỗi chỉ định thời điểm diễn ra ngày cuối tuần"
			},
			{
				name: "holidays",
				description: "là một mảng tùy chọn một hoặc nhiều số ngày tuần tự để loại trừ khỏi lịch làm việc, như ngày lễ tiểu bang và liên bang và ngày nghỉ lễ bù"
			}
		]
	},
	{
		name: "XIRR",
		description: "Trả về tỷ lệ thu hồi nội bộ của lịch trình lưu chuyển tiền tệ.",
		arguments: [
			{
				name: "values",
				description: "là chuỗi lưu chuyển tiền tệ tương ứng với lịch trình thanh toán theo ngày"
			},
			{
				name: "dates",
				description: "là lịch trình ngày thanh toán tương ứng với thanh toán lưu chuyển tiền tệ"
			},
			{
				name: "guess",
				description: "là số dự đoán sát với kết quả của  XIRR"
			}
		]
	},
	{
		name: "XNPV",
		description: "Trả về giá trị thực hiện tại cho lịch trình lưu chuyển tiền tệ.",
		arguments: [
			{
				name: "rate",
				description: "là tỷ lệ mất giá áp dụng cho lưu chuyển tiền tệ"
			},
			{
				name: "values",
				description: "là chuỗi lưu chuyển tiền tệ tương ứng với lịch trình thanh toán theo ngày"
			},
			{
				name: "dates",
				description: "là lịch trình ngày thanh toán tương ứng với thanh toán lưu chuyển tiền tệ"
			}
		]
	},
	{
		name: "XOR",
		description: "Trả về hàm 'Exclusive Or' lô-gic của tất cả tham đối.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "là từ 1 đến 254 điều kiện bạn muốn kiểm tra mà có thể là TRUE (ĐÚNG) hoặc FALSE (SAI) và có thể là giá trị lô-gic, mảng, hoặc tham chiếu"
			},
			{
				name: "logical2",
				description: "là từ 1 đến 254 điều kiện bạn muốn kiểm tra mà có thể là TRUE (ĐÚNG) hoặc FALSE (SAI) và có thể là giá trị lô-gic, mảng, hoặc tham chiếu"
			}
		]
	},
	{
		name: "YEAR",
		description: "Trả về năm của ngày tháng, một số nguyên trong khoảng 1900 - 9999.",
		arguments: [
			{
				name: "serial_number",
				description: "là một số trong mã ngày-tháng được sử dụng bởi Spreadsheet"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "Trả lại phân số năm thể hiện số ngày nguyên giữa start_date và end_date.",
		arguments: [
			{
				name: "start_date",
				description: "là số tuần tự ngày tháng thể hiện ngày bắt đầu"
			},
			{
				name: "end_date",
				description: "là số tuần tự ngày tháng thể hiện ngày kết thúc"
			},
			{
				name: "basis",
				description: "là loại cơ sở tính ngày được dùng"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "Trả lại thu hồi hàng năm cho thế chấp giảm giá. Ví dụ, trái phiếu kho bạc.",
		arguments: [
			{
				name: "settlement",
				description: "là ngày thanh khoản thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "maturity",
				description: "là ngày tới hạn thế chấp, thể hiện như số ngày tuần tự"
			},
			{
				name: "pr",
				description: "là giá của thế chấp trên $100 mệnh giá"
			},
			{
				name: "redemption",
				description: "là giá trị chuộc thế chấp trên $100 mệnh giá"
			},
			{
				name: "basis",
				description: "là loại cơ sở tính ngày được dùng"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Trả về giá trị P một phần dư của kiểm định z.",
		arguments: [
			{
				name: "array",
				description: "là mảng hoặc khoảng dữ liệu cần kiểm định X"
			},
			{
				name: "x",
				description: "là giá trị cần thử"
			},
			{
				name: "sigma",
				description: "là tập toàn bộ (đã biết) độ lệch tiêu chuẩn. Nếu không có, độ lệch tiêu chuẩn mẫum sẽ được sử dụng"
			}
		]
	},
	{
		name: "ZTEST",
		description: "Trả về giá trị P một phần dư của kiểm định z.",
		arguments: [
			{
				name: "array",
				description: "là mảng hoặc khoảng dữ liệu cần kiểm định X"
			},
			{
				name: "x",
				description: "là giá trị cần thử"
			},
			{
				name: "sigma",
				description: "là tập toàn bộ (đã biết) độ lệch tiêu chuẩn. Nếu không có, độ lệch tiêu chuẩn mẫu sẽ được sử dụng"
			}
		]
	}
];