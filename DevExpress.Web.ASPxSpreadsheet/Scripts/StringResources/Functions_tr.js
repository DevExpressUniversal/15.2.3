ASPxClientSpreadsheet.Functions = [
	{
		name: "ACOS",
		description: "Bir sayının arkkosinüsünü verir, radyan cinsinde ve 0 - Pi aralığındadır. Arkkosinüs, kosinüsü Sayı olan açıdır.",
		arguments: [
			{
				name: "sayı",
				description: "istediğiniz açının kosinüs değeri, -1 ile 1 arasında olmalıdır"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Bir sayının ters hiperbolik kosinüsünü verir.",
		arguments: [
			{
				name: "sayı",
				description: "1 'e eşit veya 1'den büyük herhangi bir gerçek sayı"
			}
		]
	},
	{
		name: "ACOT",
		description: "Bir sayının arkkotanjantını 0 ile Pi aralığındaki radyanlar cinsinden verir.",
		arguments: [
			{
				name: "sayı",
				description: "istediğiniz açının kotanjantı"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Bir sayının ters hiperbolik kotanjant değerini verir.",
		arguments: [
			{
				name: "sayı",
				description: "istenen açının hiperbolik kotanjantı"
			}
		]
	},
	{
		name: "ADRES",
		description: "Bir hücre başvurusunu, belirtilen satır ve sütun numaraları verilmiş halde metin olarak oluşturur.",
		arguments: [
			{
				name: "satır_num",
				description: "hücre başvurusunda kullanılacak satır numarası: satır 1 için, Satır_num = 1 "
			},
			{
				name: "sütun_num",
				description: "hücre başvurusunda kullanılacak sütun numarası. Örneğin, D sütunu için Sütun_num = 4"
			},
			{
				name: "mutlak_num",
				description: "başvuru türünü belirtir: mutlak = 1; mutlak satır/göreceli sütun = 2; göreceli satır/mutlak sütun = 3; göreceli = 4"
			},
			{
				name: "a1",
				description: "başvuru stilini belirten mantıksal değer: A1 stili = 1 ya da DOĞRU; R1C1 stili = 0 ya da YANLIŞ"
			},
			{
				name: "sayfa_metni",
				description: "dış başvuru olarak kullanılacak çalışma sayfasının adını belirten metin"
			}
		]
	},
	{
		name: "AİÇVERİMORANI",
		description: "Para akışı planı için iç dönüş oranını döndürür.",
		arguments: [
			{
				name: "değerler",
				description: "tarihlerdeki ödeme planına karşılık gelen para akışı serisi"
			},
			{
				name: "tarihler",
				description: "para akışı ödemelerine karşılık gelen ödeme günleri planı"
			},
			{
				name: "tahmin",
				description: "XIRR işlevinin sonucuna yakın olarak tahmin edilen sayı"
			}
		]
	},
	{
		name: "ALANSAY",
		description: "Bir başvurudaki alanların sayısını verir. Alan bir bitişik hücreler aralığı ya da sadece bir hücre olabilir.",
		arguments: [
			{
				name: "başvuru",
				description: "bir hücreye veya hücreler aralığına başvurudur ve birden fazla alana işaret edebilir"
			}
		]
	},
	{
		name: "ALTTOPLAM",
		description: "Bir liste veya veritabanından bir alt toplam verir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "işlev_sayısı",
				description: "alt toplamı hesaplamak için kullanılan özet işlevini belirten 1-11 arasındaki sayı."
			},
			{
				name: "başv1",
				description: "alt toplamını almak istediğiniz en az 1 en fazla 254 başvuru veya aralıktır"
			}
		]
	},
	{
		name: "ANA_PARA_ÖDEMESİ",
		description: "Dönemsel sabit ödemeli ve sabit faizli bir yatırım için yapılacak anapara ödemesi tutarını verir.",
		arguments: [
			{
				name: "oran",
				description: "dönem başına düşen faiz oranı. Örneğin, %6 yıllık faiz oranına karşılık üç aylık ödeme için %6/4 kullanın"
			},
			{
				name: "dönem",
				description: "1 ile dönem_sayısı arasında olması gereken dönemi belirtir"
			},
			{
				name: "dönem_sayısı",
				description: "bir yıllık ödeme içerisindeki toplam ödeme dönemi sayısı"
			},
			{
				name: "bd",
				description: "bugünkü değer: gelecekte yapılacak bir dizi ödemenin bugünkü değerini gösteren toplam miktar"
			},
			{
				name: "gd",
				description: "gelecek değer ya da son ödeme yapıldıktan sonra elde edilmek istenen nakit bakiyesi"
			},
			{
				name: "tür",
				description: "mantıksal değer: dönem başındaki ödemeler = 1; dönem sonundaki ödemeler = 0 ya da atlanmış"
			}
		]
	},
	{
		name: "ANBD",
		description: "Para akışı planı için şu anki net değeri döndürür.",
		arguments: [
			{
				name: "oran",
				description: "para akışına uygulanacak indirim oranı"
			},
			{
				name: "değerler",
				description: "tarihlerdeki ödeme planına karşılık gelen para akışı serisi"
			},
			{
				name: "tarihler",
				description: "para akışı ödemelerine karşılık gelen ödeme günleri planı"
			}
		]
	},
	{
		name: "ARA",
		description: "Tek-satırlı ya da tek-sütunlu bir aralıktan ya da bir diziden bir değer verir. Geriye dönük uyumluluk için sağlanmıştır.",
		arguments: [
			{
				name: "aranan_değer",
				description: "ARA'nın ara_vektörü içerisinde arayacağı değerdir ve bir sayı, metin, mantıksal değer, ya da bir değer adı veya bir değer başvurusu olabilir"
			},
			{
				name: "aranan_vektör",
				description: "sadece bir satır veya sütundan oluşan ve artan sıraya yerleştirilmiş olan metin, sayı ya da mantıksal değerler içeren aralık"
			},
			{
				name: "sonuç_vektör",
				description: "sadece bir satır veya sütun içeren aralıktır, Ara_vektörü ile aynı boyuttadır"
			}
		]
	},
	{
		name: "ARAP",
		description: "Bir Roma rakamını Arap rakamına dönüştürür.",
		arguments: [
			{
				name: "metin",
				description: "dönüştürmek istediğiniz Roma rakamı"
			}
		]
	},
	{
		name: "AŞAĞIYUVARLA",
		description: "Bir sayıyı sıfıra yakınsayarak yuvarlar.",
		arguments: [
			{
				name: "sayı",
				description: "aşağı yuvarlamak istediğiniz herhangi bir gerçek sayı"
			},
			{
				name: "sayı_rakamlar",
				description: "sayıyı yuvarlamak istediğiniz rakam sayısı. Negatif sayılar ondalık noktasının soluna; sıfır ya da atlanmış olanlar ise en yakın tamsayıya yuvarlanır"
			}
		]
	},
	{
		name: "ASİN",
		description: "Bir sayının radyan cinsinden -Pi/2 ile Pi/2 aralığındaki arksinüsünü verir.",
		arguments: [
			{
				name: "sayı",
				description: "istediğiniz açının sinüs değeri, -1 ile 1 arasında olmalıdır"
			}
		]
	},
	{
		name: "ASİNH",
		description: "Bir sayının ters hiperbolik sinüsünü verir.",
		arguments: [
			{
				name: "sayı",
				description: "1'e eşit veya 1'den büyük herhangi bir gerçek sayı"
			}
		]
	},
	{
		name: "ATAN",
		description: "Bir sayının radyan cinsinden -Pi/2 ile Pi/2 aralığındaki arktanjantını verir.",
		arguments: [
			{
				name: "sayı",
				description: "istediğiniz açının tanjantı"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Belirtilen x- ve y- koordinatlarının radyan cinsinden -Pi (-Pi hariç) ile Pi arasındaki arktanjantını verir.",
		arguments: [
			{
				name: "x_sayısı",
				description: "noktanın x-koordinatı"
			},
			{
				name: "y_sayısı",
				description: "noktanın y-koordinatı"
			}
		]
	},
	{
		name: "ATANH",
		description: "Bir sayının ters hiperbolik tanjantını verir.",
		arguments: [
			{
				name: "sayı",
				description: "-1 ve 1 arasında (-1 ve 1 hariç) herhangi bir gerçek sayı"
			}
		]
	},
	{
		name: "AY",
		description: "1 (Ocak) ile 12 (Aralık) arasındaki bir sayı ile ifade edilen ayı döndürür.",
		arguments: [
			{
				name: "seri_no",
				description: "Spreadsheet tarafından kullanılan tarih-saat kodundaki sayı"
			}
		]
	},
	{
		name: "AZALANBAKİYE",
		description: "Sabit azalan bakiye yöntemi kullanarak bir varlığın belirtilen dönem içindeki yıpranmasını verir.",
		arguments: [
			{
				name: "maliyet",
				description: "varlığın ilk maliyeti"
			},
			{
				name: "hurda",
				description: "varlığın kullanım ömrü bittikten sonraki hurda değeri"
			},
			{
				name: "ömür",
				description: "varlığın yıpranma dönemi miktarı (bazen varlığın kullanım ömrü olarak da kullanılır)"
			},
			{
				name: "dönem",
				description: "yıpranmayı hesaplamak istediğiniz dönem. Ömür ile aynı birimde olmalıdır"
			},
			{
				name: "ay",
				description: "ilk yılda bulunan ay sayısı. Ay sayısı boş bırakılırsa 12 olduğu varsayılır"
			}
		]
	},
	{
		name: "BAĞ_DEĞ_DOLU_SAY",
		description: "Aralıktaki boş olmayan hücrelerin kaç tane olduğunu sayar.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "değer1",
				description: "Saymak istediğiniz değerleri ve hücreleri temsil eden en az 1 en fazla 255 bağımsız değişkendir. Değerler herhangi bir bilgi türünde olabilir"
			},
			{
				name: "değer2",
				description: "Saymak istediğiniz değerleri ve hücreleri temsil eden en az 1 en fazla 255 bağımsız değişkendir. Değerler herhangi bir bilgi türünde olabilir"
			}
		]
	},
	{
		name: "BAĞ_DEĞ_SAY",
		description: "Aralıktaki sayı içeren hücrelerin kaç tane olduğunu sayar.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "değer1",
				description: "en az 1 en fazla 255 bağımsız değişkendir, her biri farklı türdeki çeşitli verileri taşıyabilir ya da bunlara başvurabilir, fakat yalnızca sayılar dikkate alınır"
			},
			{
				name: "değer2",
				description: "en az 1 en fazla 255 bağımsız değişkendir, her biri farklı türdeki çeşitli verileri taşıyabilir ya da bunlara başvurabilir, fakat yalnızca sayılar dikkate alınır"
			}
		]
	},
	{
		name: "BAHTMETİN",
		description: "Sayıyı (baht) metne dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz sayıdır"
			}
		]
	},
	{
		name: "BASIKLIK",
		description: "Bir veri kümesinin basıklığını verir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "basıklığını bulmak istediğiniz en az 1 en fazla 255 sayı, ad, dizi ya da sayı içeren başvurudur"
			},
			{
				name: "sayı2",
				description: "basıklığını bulmak istediğiniz en az 1 en fazla 255 sayı, ad, dizi ya da sayı içeren başvurudur"
			}
		]
	},
	{
		name: "BD",
		description: "Bir yatırımın bugünkü değerini verir: gelecekte yapılacak bir dizi ödemenin bugünkü değeri.",
		arguments: [
			{
				name: "oran",
				description: "dönem başına faiz oranı. Örneğin, %6 yıllık faiz oranına karşılık üç aylık ödeme için %6/4 kullanın"
			},
			{
				name: "dönem_sayısı",
				description: "bir yıllık ödeme içerisindeki toplam ödeme dönemi sayısı"
			},
			{
				name: "devresel_ödeme",
				description: "her dönem yapılan ödeme, yıllık yatırım dönemi boyunca değişmez"
			},
			{
				name: "gd",
				description: "gelecek değer ya da son ödeme yapıldıktan sonra elde edilmek istenen nakit bakiyesi"
			},
			{
				name: "tür",
				description: "mantıksal değer: dönem başındaki ödemeler = 1; dönem sonundaki ödemeler = 0 ya da atlanmış"
			}
		]
	},
	{
		name: "BESINIR",
		description: "Bir sayının sınır bir değerden büyük olup olmadığını sınar.",
		arguments: [
			{
				name: "sayı",
				description: "sınanacak sayı"
			},
			{
				name: "sınır_değer",
				description: "sınır değer"
			}
		]
	},
	{
		name: "BESSELI",
		description: "In(x) değiştirilmiş Bessel işlevini döndürür.",
		arguments: [
			{
				name: "x",
				description: "işlevin hesaplanacağı değer"
			},
			{
				name: "n",
				description: "Bessel işlevinin derecesi"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Jn(x) Bessel işlevini döndürür.",
		arguments: [
			{
				name: "x",
				description: "işlevin hesaplanacağı değer"
			},
			{
				name: "n",
				description: "Bessel işlevinin derecesi"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Kn(x) değiştirilmiş Bessel işlevini döndürür.",
		arguments: [
			{
				name: "x",
				description: "işlevin hesaplanacağı değer"
			},
			{
				name: "n",
				description: "işlevin derecesi"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Yn(x) Bessel işlevini döndürür.",
		arguments: [
			{
				name: "x",
				description: "işlevin hesaplanacağı değer"
			},
			{
				name: "n",
				description: "işlevin derecesi"
			}
		]
	},
	{
		name: "BETA.DAĞ",
		description: "Beta olasılık dağılımı işlevini verir.",
		arguments: [
			{
				name: "x",
				description: "işlevin değerini bulurken kullanılan A ile B arasındaki değer"
			},
			{
				name: "alfa",
				description: "dağılım parametresi, 0'dan büyük olmalıdır"
			},
			{
				name: "beta",
				description: "dağılım parametresi, 0'dan büyük olmalıdır"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif dağılım işlevi için DOĞRU'yu; olasılık yoğunluk işlevi için YANLIŞ'ı kullanın"
			},
			{
				name: "A",
				description: "x aralığı için isteğe bağlı alt sınır. Atlanırsa, A = 0"
			},
			{
				name: "B",
				description: "x aralığı için isteğe bağlı üst sınır. Atlanırsa, B = 1"
			}
		]
	},
	{
		name: "BETA.TERS",
		description: "Kümülatif beta olasılık yoğunluk işlevinin (BETA.DAĞ) tersini verir.",
		arguments: [
			{
				name: "olasılık",
				description: "beta dağılımıyla ilgili olasılık"
			},
			{
				name: "alfa",
				description: "dağılım parametresi, 0'dan büyük olmalıdır"
			},
			{
				name: "beta",
				description: "dağılım parametresi, 0'dan büyük olmalıdır"
			},
			{
				name: "A",
				description: "x aralığı için isteğe bağlı alt sınır. Atlanırsa, A = 0"
			},
			{
				name: "B",
				description: "x aralığı için isteğe bağlı üst sınır. Atlanırsa, B = 1"
			}
		]
	},
	{
		name: "BETADAĞ",
		description: "Kümülatif beta olasılık yoğunluk işlevini verir.",
		arguments: [
			{
				name: "x",
				description: "işlevin değerini bulurken kullanılan A ile B arasındaki değer"
			},
			{
				name: "alfa",
				description: "dağılım parametresi, 0'dan büyük olmalıdır"
			},
			{
				name: "beta",
				description: "dağılım parametresi, 0'dan büyük olmalıdır"
			},
			{
				name: "A",
				description: "x aralığı için isteğe bağlı alt sınır. Atlanırsa, A = 0"
			},
			{
				name: "B",
				description: "x aralığı için isteğe bağlı üst sınır. Atlanırsa, B = 1"
			}
		]
	},
	{
		name: "BETATERS",
		description: "Kümülatif beta olasılık yoğunluk işlevinin (BETADAĞ) tersini verir.",
		arguments: [
			{
				name: "olasılık",
				description: "beta dağılımıyla ilgili olasılık"
			},
			{
				name: "alfa",
				description: "dağılım parametresi, 0'dan büyük olmalıdır"
			},
			{
				name: "beta",
				description: "dağılım parametresi, 0'dan büyük olmalıdır"
			},
			{
				name: "A",
				description: "x aralığı için isteğe bağlı alt sınır. Atlanırsa, A = 0"
			},
			{
				name: "B",
				description: "x aralığı için isteğe bağlı üst sınır. Atlanırsa, B = 1"
			}
		]
	},
	{
		name: "BİLGİ",
		description: "Geçerli işletim ortamı hakkında bilgi verir.",
		arguments: [
			{
				name: "metin_türü",
				description: "almak istediğiniz bilginin türünü belirten metin."
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "İkilik düzendeki bir sayıyı onluk düzene dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz ikilik düzendeki sayı"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "İkilik düzendeki bir sayıyı onaltılık düzene dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz ikilik düzendeki sayı"
			},
			{
				name: "basamak",
				description: "kullanılacak karakter sayısı"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "İkilik düzendeki bir sayıyı sekizlik düzene dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz ikilik düzendeki sayı"
			},
			{
				name: "basamak",
				description: "kullanılacak karakter sayısı"
			}
		]
	},
	{
		name: "BİNOM.DAĞ",
		description: "Tek terimli binom dağılımı olasılığını verir.",
		arguments: [
			{
				name: "başarı_sayısı",
				description: "denemelerdeki başarı sayısı"
			},
			{
				name: "denemeler",
				description: "bağımsız denemeler sayısı"
			},
			{
				name: "başarı_olasılığı",
				description: "her denemedeki başarı olasılığı"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif dağılım fonksiyonu için DOĞRU'yu; olasılık kütle fonksiyonu için YANLIŞ'ı kullanın"
			}
		]
	},
	{
		name: "BİNOM.DAĞ.ARALIK",
		description: "Binom dağılımını kullanarak bir deneme sonucunun başarı olasılığını döndürür.",
		arguments: [
			{
				name: "denemeler",
				description: "bağımsız denemelerin sayısı"
			},
			{
				name: "başarı_olasılığı",
				description: "her bir denemedeki başarı olasılığı"
			},
			{
				name: "başarı_sayısı",
				description: "denemelerdeki başarı sayısı"
			},
			{
				name: "başarı_sayısı2",
				description: "varsa, bu fonksiyon başarılı denemelerin başarı_sayısı ve başarı_sayısı2 arasında yer alma olasılığını döndürür"
			}
		]
	},
	{
		name: "BİNOM.TERS",
		description: "Kümülatif binom dağılımının ölçüt değerinden küçük veya ona eşit olduğu en küçük değeri verir.",
		arguments: [
			{
				name: "denemeler",
				description: "Bernoulli denemeler sayısı"
			},
			{
				name: "başarı_olasılığı",
				description: "her denemedeki başarı olasılığı, 0 ile 1 arasında (bunlar dahil) bir sayı"
			},
			{
				name: "alfa",
				description: "ölçüt değeri, 0 ile 1 arasında (bunlar dahil) bir sayı"
			}
		]
	},
	{
		name: "BİNOMDAĞ",
		description: "Tek terimli binom dağılımı olasılığını verir.",
		arguments: [
			{
				name: "başarı_sayısı",
				description: "denemelerdeki başarı sayısı"
			},
			{
				name: "denemeler",
				description: "bağımsız denemeler sayısı"
			},
			{
				name: "başarı_olasılığı",
				description: "her denemedeki başarı olasılığı"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif dağılım fonksiyonu için DOĞRU'yu; olasılık kütle fonksiyonu için YANLIŞ'ı kullanın"
			}
		]
	},
	{
		name: "BİRİMMATRİS",
		description: "Belirtilen boyut için birim matris döndürür.",
		arguments: [
			{
				name: "boyut",
				description: "döndürmek istediğiniz birim matrisin boyutunu belirten tamsayı"
			}
		]
	},
	{
		name: "BİRLEŞTİR",
		description: "Birden fazla metin dizesini bir metin dizesi halinde birleştirir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "metin1",
				description: "bir tek metin dizesinde birleştirilecek olan 1-255 arasındaki metin dizeleridir ve metin dizesi, sayı ya da tek hücre başvurusu olabilir"
			},
			{
				name: "metin2",
				description: "bir tek metin dizesinde birleştirilecek olan 1-255 arasındaki metin dizeleridir ve metin dizesi, sayı ya da tek hücre başvurusu olabilir"
			}
		]
	},
	{
		name: "BİTÖZELVEYA",
		description: "İki sayının bit tabanlı bir 'Özel Veya' değerini verir.",
		arguments: [
			{
				name: "sayı1",
				description: "değerlendirmek istediğiniz ikili sayının ondalık gösterimi"
			},
			{
				name: "sayı2",
				description: "değerlendirmek istediğiniz ikili sayının ondalık gösterimi"
			}
		]
	},
	{
		name: "BİTSAĞAKAYDIR",
		description: "shift_amount bitleri tarafından sağa kaydırılan bir sayıyı verir.",
		arguments: [
			{
				name: "sayı",
				description: "değerlendirmek istediğiniz ikilik düzendeki bir sayının ondalık gösterimidir"
			},
			{
				name: "kaydırma_miktarı",
				description: "Sayıyı sağa kaydırmak istediğiniz bitlerin sayısı"
			}
		]
	},
	{
		name: "BİTSOLAKAYDIR",
		description: "shift_amount bitleri tarafından sola kaydırılan bir sayıyı verir.",
		arguments: [
			{
				name: "sayı",
				description: "değerlendirmek istediğiniz ikilik düzendeki bir sayının ondalık gösterimidir"
			},
			{
				name: "kaydırma_miktarı",
				description: "Sayıyı sola kaydırmak istediğiniz bitlerin sayısı"
			}
		]
	},
	{
		name: "BİTVE",
		description: "İki sayının bit tabanlı bir 'Ve' değerini verir.",
		arguments: [
			{
				name: "sayı1",
				description: "değerlendirmek istediğiniz ikili sayının ondalık gösterimi"
			},
			{
				name: "sayı2",
				description: "değerlendirmek istediğiniz ikili sayının ondalık gösterimi"
			}
		]
	},
	{
		name: "BİTVEYA",
		description: "İki sayının bit tabanlı bir 'Veya' değerini verir.",
		arguments: [
			{
				name: "sayı1",
				description: "değerlendirmek istediğiniz ikili sayının ondalık gösterimi"
			},
			{
				name: "sayı2",
				description: "değerlendirmek istediğiniz ikili sayının ondalık gösterimi"
			}
		]
	},
	{
		name: "BÖLÜM",
		description: "Bir bölmenin tamsayı kısmını döndürür.",
		arguments: [
			{
				name: "pay",
				description: "bölünen"
			},
			{
				name: "payda",
				description: "bölen"
			}
		]
	},
	{
		name: "BOŞLUKSAY",
		description: "Belirtilen hücre aralığındaki boş hücreleri sayar.",
		arguments: [
			{
				name: "aralık",
				description: "boş hücrelerin sayısını öğrenmek istediğiniz aralık"
			}
		]
	},
	{
		name: "BUGÜN",
		description: "Bugünkü tarihi, tarih biçiminde verir.",
		arguments: [
		]
	},
	{
		name: "BUL",
		description: "Bir metin dizesini diğer bir metin dizesi içinde bulur ve bulunan dizenin başlama konumu numarasını verir (büyük küçük harf duyarlı).",
		arguments: [
			{
				name: "bul_metin",
				description: "bulmak istediğiniz metin. Metin_içindeki ilk karakteri eşleştirmek için çift tırnak (boş metin) kullanın; eşleştirme karakterleri kullanılamaz"
			},
			{
				name: "metin_içinde",
				description: "bulmak istediğiniz metni içeren metin"
			},
			{
				name: "başlangıç_sayısı",
				description: "aramanın başlayacağı karakteri belirtir. Metin_içindeki ilk karakterin numarası 1'dir. Atlanırsa Başl_num = 1"
			}
		]
	},
	{
		name: "BÜYÜK",
		description: "Bir veri kümesi içindeki en büyük k. değeri verir. Örneğin, beşinci en büyük sayı.",
		arguments: [
			{
				name: "dizi",
				description: "k. en büyük değeri belirlemek için kullanılan veri dizisi veya aralığı"
			},
			{
				name: "k",
				description: "gelecek olan değerin bulunduğu hücre aralığı veya dizideki konumu (en büyük değerden)"
			}
		]
	},
	{
		name: "BÜYÜKHARF",
		description: "Bir metni büyük harfe dönüştürür.",
		arguments: [
			{
				name: "metin",
				description: "büyük harfe dönüştürmek istediğiniz metin, bir başvuru ya da bir metin dizesi"
			}
		]
	},
	{
		name: "BÜYÜME",
		description: "Bilinen veri noktalarıyla eşleşen üstel büyüme trendi içindeki sayıları döndürür.",
		arguments: [
			{
				name: "bilinen_y'ler",
				description: "y=b*m^x denkleminde kullanılan y değerleri kümesi, bir dizi ya da pozitif sayılar aralığı"
			},
			{
				name: "bilinen_x'ler",
				description: "y=b*m^x denkleminde kullanılan isteğe bağlı x değerleri, bir dizi ya da Bilinen_y'lerle aynı boyuttaki aralık"
			},
			{
				name: "yeni_x'ler",
				description: "ilişkili y değerlerini BÜYÜME ile bulacağınız yeni x değerleri"
			},
			{
				name: "sabit",
				description: "mantıksal değer: Sabit = DOĞRU ise sabit b değeri olağan şekilde hesaplanır; Sabit = YANLIŞ ya da atlanmış ise b 1'e eşitlenir"
			}
		]
	},
	{
		name: "ÇARPIKLIK",
		description: "Dağılımın eğriliğini verir: bir dağılımın ortalaması etrafındaki asimetri derecesini belirtir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "eğriliği hesaplamak istediğiniz en az 1 en fazla 255 sayı, ad, dizi ya da sayı içeren başvurudur"
			},
			{
				name: "sayı2",
				description: "eğriliği hesaplamak istediğiniz en az 1 en fazla 255 sayı, ad, dizi ya da sayı içeren başvurudur"
			}
		]
	},
	{
		name: "ÇARPIKLIK.P",
		description: "Popülasyona bağlı olarak dağılımın eğriliğini verir: bir dağılımın ortalaması etrafındaki asimetri derecesini belirtir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "popülasyon eğriliğini hesaplamak istediğiniz en az 1 en fazla 254 sayı, ad, dizi ya da sayı içeren başvurudur"
			},
			{
				name: "sayı2",
				description: "popülasyon eğriliğini hesaplamak istediğiniz en az 1 en fazla 254 sayı, ad, dizi ya da sayı içeren başvurudur"
			}
		]
	},
	{
		name: "ÇARPIM",
		description: "Bağımsız değişken olarak verilen tüm sayıları çarpar.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "çarpımını bulmak istediğiniz en az 1 en fazla 255 sayı, mantıksal değer ya da sayıları temsil eden metindir"
			},
			{
				name: "sayı2",
				description: "çarpımını bulmak istediğiniz en az 1 en fazla 255 sayı, mantıksal değer ya da sayıları temsil eden metindir"
			}
		]
	},
	{
		name: "ÇARPINIM",
		description: "Bir sayının 1*2*3*...*Sayı şeklinde çarpınımını verir.",
		arguments: [
			{
				name: "sayı",
				description: "çarpınımını almak istediğiniz negatif olmayan sayı"
			}
		]
	},
	{
		name: "ÇEVİR",
		description: "Sayıyı bir ölçü biriminden bir diğerine dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüşecek birimdeki değer"
			},
			{
				name: "ilk_birim",
				description: "sayı için birim"
			},
			{
				name: "son_birim",
				description: "sonuç için birim"
			}
		]
	},
	{
		name: "ÇİFT",
		description: "Bir sayıyı, mutlak değerce kendinden büyük en yakın çift tamsayıya yuvarlar.",
		arguments: [
			{
				name: "sayı",
				description: "yuvarlanacak değer"
			}
		]
	},
	{
		name: "ÇİFTAZALANBAKİYE",
		description: "Çift azalan bakiye yöntemi veya belirttiğiniz diğer bir yöntemle, bir varlığın belirtilen dönem içindeki yıpranmasını verir.",
		arguments: [
			{
				name: "maliyet",
				description: "varlığın ilk maliyeti"
			},
			{
				name: "hurda",
				description: "varlığın kullanım ömrü bittikten sonraki hurda değeri"
			},
			{
				name: "ömür",
				description: "varlığın yıpranma dönemi miktarı (bazen varlığın kullanım ömrü olarak da kullanılır)"
			},
			{
				name: "dönem",
				description: "yıpranmayı hesaplamak istediğiniz dönem. Ömür ile aynı birimde olmalıdır"
			},
			{
				name: "faktör",
				description: "bakiyenin azalma oranı. Faktör yoksayılırsa, 2 olarak varsayılır (çift azalan bakiye yöntemi)"
			}
		]
	},
	{
		name: "ÇİFTFAKTÖR",
		description: "Verilen bir sayıdan bire kadar ikişer ikişer azalarak oluşan sayıların çarpımını döndürür.",
		arguments: [
			{
				name: "sayı",
				description: "işlevin uygulanacağı sayı"
			}
		]
	},
	{
		name: "ÇİFTMİ",
		description: "Sayı bir çift sayı ise DOĞRU döndürür.",
		arguments: [
			{
				name: "sayı",
				description: "sınanacak sayı"
			}
		]
	},
	{
		name: "ÇOKEĞERORTALAMA",
		description: "Verili bir koşul veya ölçüt kümesi tarafından belirtilen hücrelerin ortalamasını (aritmetik ortalama) bulur.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "aralık_ortalaması",
				description: "ortalamayı bulmak için kullanılacak asıl hücreler."
			},
			{
				name: "ölçüt_aralığı",
				description: "belirli bir koşula göre değerlendirilmesini istediğiniz hücre aralığı"
			},
			{
				name: "ölçüt",
				description: "ortalamayı bulmak için kullanılacak hücreleri tanımlayan, sayı, ifade veya metin biçimindeki koşul veya ölçüt"
			}
		]
	},
	{
		name: "ÇOKEĞERSAY",
		description: "Verili bir koşul veya ölçüt kümesi tarafından belirtilen hücreleri sayar.",
		arguments: [
			{
				name: "ölçüt_aralığı",
				description: "belirli bir koşula göre değerlendirilmesini istediğiniz hücre aralığı"
			},
			{
				name: "ölçüt",
				description: "sayılacak hücreleri tanımlayan, sayı, ifade veya metin biçimindeki koşul"
			}
		]
	},
	{
		name: "ÇOKETOPLA",
		description: "Verili bir koşul veya ölçüt kümesi tarafından belirtilen hücreleri toplar.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "aralık_toplamı",
				description: "toplanacak asıl hücreler."
			},
			{
				name: "ölçüt_aralığı",
				description: "belirli bir koşula göre değerlendirilmesini istediğiniz hücre aralığı"
			},
			{
				name: "ölçüt",
				description: "toplamı alınacak hücreleri tanımlayan, sayı, ifade veya metin biçimindeki koşul veya ölçüt"
			}
		]
	},
	{
		name: "ÇOKTERİMLİ",
		description: "Bir sayı kümesinin çok terimli değerini döndürür.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "çok terimli değerini istediğiniz en az 1 en çok 255 değer"
			},
			{
				name: "sayı2",
				description: "çok terimli değerini istediğiniz en az 1 en çok 255 değer"
			}
		]
	},
	{
		name: "COS",
		description: "Bir açının kosinüsünü verir.",
		arguments: [
			{
				name: "sayı",
				description: "kosinüs değerini almak istediğiniz radyan cinsinden açı"
			}
		]
	},
	{
		name: "COSH",
		description: "Bir sayının hiperbolik kosinüsünü verir.",
		arguments: [
			{
				name: "sayı",
				description: "herhangi bir gerçek sayı"
			}
		]
	},
	{
		name: "COT",
		description: "Bir açının kotanjant değerini verir.",
		arguments: [
			{
				name: "sayı",
				description: "kotanjantı istenen radyan cinsinden açı"
			}
		]
	},
	{
		name: "COTH",
		description: "Bir sayının hiperbolik kotanjant değerini verir.",
		arguments: [
			{
				name: "sayı",
				description: "hiperbolik kotanjantı istenen radyan cinsinden açı"
			}
		]
	},
	{
		name: "CSC",
		description: "Bir açının kosekant değerini verir.",
		arguments: [
			{
				name: "sayı",
				description: "kosekantı istenen radyan cinsinden açı"
			}
		]
	},
	{
		name: "CSCH",
		description: "Bir açının hiperbolik kosekant değerini verir.",
		arguments: [
			{
				name: "sayı",
				description: "hiperbolik kosekantı istenen radyan cinsinden açı"
			}
		]
	},
	{
		name: "D_İÇ_VERİM_ORANI",
		description: "Yatırım maliyetini ve yatırımın bileşik getirisini dikkate alarak, dönemsel nakit akışları serisi için iç verim oranını verir.",
		arguments: [
			{
				name: "değerler",
				description: "düzenli dönemlerde gerçekleşen bir seri ödeme (negatif) ve geliri (pozitif) temsil eden sayıları içeren hücre başvurusu ya da dizi"
			},
			{
				name: "finansman_faiz_oranı",
				description: "nakit akışında kullanılan paraya ödediğiniz faiz oranı"
			},
			{
				name: "tekraryatırım_oranı",
				description: "yeniden yatırım yaptığınızda elde edeceğiniz faiz oranı"
			}
		]
	},
	{
		name: "DA",
		description: "Bir malın bir dönem için doğrusal yıpranmasını verir.",
		arguments: [
			{
				name: "maliyet",
				description: "malın ilk maliyeti"
			},
			{
				name: "hurda",
				description: "malın kullanım ömrü bittikten sonraki hurda değeri"
			},
			{
				name: "ömür",
				description: "malın yıpranma dönemi sayısı (bazen malın kullanım ömrü olarak da kullanılır)"
			}
		]
	},
	{
		name: "DAB",
		description: "Çift azalan bakiye yöntemini ya da belirttiğiniz başka bir yöntemi kullanarak, kısmi dönemleri de içeren belirli bir dönem için bir malın amortismanını verir.",
		arguments: [
			{
				name: "maliyet",
				description: "malın ilk maliyeti"
			},
			{
				name: "hurda",
				description: "malın kullanım ömrü bittikten sonraki hurda değeri"
			},
			{
				name: "ömür",
				description: "malın yıpranma dönemi miktarı (bazen malın kullanım ömrü olarak da kullanılır)"
			},
			{
				name: "başlangıç_dönemi",
				description: "yıpranmayı hesaplamak istediğiniz başlangıç dönemi, Ömür ile aynı birimde"
			},
			{
				name: "son_dönem",
				description: "yıpranmayı hesaplamak istediğiniz son dönem, Ömür ile aynı birimde"
			},
			{
				name: "faktör",
				description: "bakiyenin azalma oranı, atlanırsa 2 (çift-azalan bakiye)"
			},
			{
				name: "değiştirme",
				description: "yıpranma azalan bakiyeden büyük olduğunda doğrusal yıpranmaya geçiş yap = YANLIŞ ya da atlanmış; geçiş yapma = DOĞRU"
			}
		]
	},
	{
		name: "DAKİKA",
		description: "Bir seri numarasına karşılık gelen, 0-59 arasında bir tamsayı olan dakikayı verir.",
		arguments: [
			{
				name: "seri_no",
				description: "Spreadsheet tarafından kullanılan tarih-saat kodundaki sayı ya da zaman biçimindeki metin, örneğin 16:48:00 ya da 4:48:00"
			}
		]
	},
	{
		name: "DAMGA",
		description: "Bilgisayarınızın karakter kümesindeki kod numarasıyla belirtilen karakteri verir.",
		arguments: [
			{
				name: "sayı",
				description: "1-255 arasında kullanacağınız karakteri belirten sayı"
			}
		]
	},
	{
		name: "DÇARP",
		description: "İki dizinin dizey çarpımını verir, sonuç, dizi1 ile aynı sayıda satıra ve dizi2 ile aynı sayıda sütuna sahip olan bir dizidir.",
		arguments: [
			{
				name: "dizi1",
				description: "çarpmak istediğiniz sayılar dizisidir ve sütun sayısı Dizi2'nin satır sayısıyla aynı olmalıdır"
			},
			{
				name: "dizi2",
				description: "çarpmak istediğiniz sayılar dizisidir ve sütun sayısı Dizi2'nin satır sayısıyla aynı olmalıdır"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "Onluk düzendeki bir sayıyı ikilik düzene dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz onlu tamsayı"
			},
			{
				name: "basamak",
				description: "kullanılacak karakter sayısı"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "Onluk düzendeki bir sayıyı onaltılık düzene dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz onlu tamsayı"
			},
			{
				name: "basamak",
				description: "kullanılacak karakter sayısı"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "Onluk düzendeki bir sayıyı sekizlik düzene dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz onlu tamsayı"
			},
			{
				name: "basamak",
				description: "kullanılacak karakter sayısı"
			}
		]
	},
	{
		name: "DEĞERİND",
		description: "İndirimli bir tahvil için 100 TL başına yüz değerini döndürür.",
		arguments: [
			{
				name: "düzenleme",
				description: "teminatın düzenleme tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "vade",
				description: "teminatın vade tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "indirim",
				description: "teminatın indirim oranı"
			},
			{
				name: "teminat",
				description: "teminatın 100 TL yüz değeri başına kefaret değeri"
			},
			{
				name: "temel",
				description: "kullanılacak gün sayısı türü"
			}
		]
	},
	{
		name: "DEĞİL",
		description: "Bağımsız değişkeninin mantığını tersine çevirir: DOĞRU bir bağımsız değişken için YANLIŞ, YANLIŞ bir bağımsız değişken için DOĞRU verir.",
		arguments: [
			{
				name: "mantıksal",
				description: "DOĞRU veya YANLIŞ olarak değerlendirilebilecek değer veya ifade"
			}
		]
	},
	{
		name: "DEĞİŞTİR",
		description: "Metin dizesinin bir kısmını başka bir metin dizesiyle değiştirir.",
		arguments: [
			{
				name: "eski_metin",
				description: "bazı karakterlerini değiştirmek istediğiniz metin"
			},
			{
				name: "başlangıç_sayısı",
				description: "Yeni_metin ile değiştirmek istediğiniz Eski_metin'deki karakterin konumu"
			},
			{
				name: "sayı_karakterler",
				description: "Eski_metin'de değiştirmek istediğiniz karakter sayısı"
			},
			{
				name: "yeni_metin",
				description: "Eski_metin'deki karakterleri dönüştürecek olan metin"
			}
		]
	},
	{
		name: "DELTA",
		description: "İki sayının eşitliğini sınar.",
		arguments: [
			{
				name: "sayı1",
				description: "ilk sayı"
			},
			{
				name: "sayı2",
				description: "ikinci sayı"
			}
		]
	},
	{
		name: "DERECE",
		description: "Radyanı dereceye çevirir.",
		arguments: [
			{
				name: "açı",
				description: "dönüştürmek istediğiniz radyan cinsinden açı"
			}
		]
	},
	{
		name: "DETERMİNANT",
		description: "Bir dizinin determinantını verir.",
		arguments: [
			{
				name: "dizi",
				description: "eşit sayıda satır ve sütuna sahip olan sayısal dizi, ya da bir hücre aralığı veya bir dizi sabiti"
			}
		]
	},
	{
		name: "DEVRESEL_ÖDEME",
		description: "Sabit ödemeli ve sabit faizli bir borç için yapılacak ödemeyi hesaplar.",
		arguments: [
			{
				name: "oran",
				description: "borç için dönem başına düşen faiz oranı. Örneğin, %6 yıllık faiz oranına karşılık üç aylık ödeme için %6/4 kullanın"
			},
			{
				name: "dönem_sayısı",
				description: "borç için toplam ödeme dönemi sayısı"
			},
			{
				name: "bd",
				description: "bugünkü değer: gelecekte yapılacak bir dizi ödemenin bugünkü değerini gösteren toplam miktar"
			},
			{
				name: "gd",
				description: "gelecek değer ya da son ödeme yapıldıktan sonra elde edilmek istenen nakit bakiyesi, atlanırsa 0 (sıfır)"
			},
			{
				name: "tür",
				description: "mantıksal değer: dönem başındaki ödemeler = 1; dönem sonundaki ödemeler = 0 ya da atlanmış"
			}
		]
	},
	{
		name: "DEVRİK_DÖNÜŞÜM",
		description: "Düşey bir hücreler aralığını yatay bir aralık olarak verir, ya da tam tersi.",
		arguments: [
			{
				name: "dizi",
				description: "devriğini almak istediğiniz çalışma sayfasında bulunan bir hücreler aralığı ya da değerler dizisi"
			}
		]
	},
	{
		name: "DİZEY_TERS",
		description: "Bir dizide saklanan bir dizeyin tersini verir.",
		arguments: [
			{
				name: "dizi",
				description: "eşit sayıda satır ve sütuna sahip olan sayısal dizi, ya da bir hücre aralığı veya bir dizi sabiti"
			}
		]
	},
	{
		name: "DOĞRU",
		description: "Mantıksal DOĞRU'yu verir.",
		arguments: [
		]
	},
	{
		name: "DOLAYLI",
		description: "Bir metin dizesiyle belirtilmiş başvuruyu verir.",
		arguments: [
			{
				name: "başv_metni",
				description: "A1 ya da R1C1-stili başvurusu içeren hücre başvurusu, başvuru olarak tanımlanmış bir ad ya da metin dizesi halindeki bir hücre başvurusu"
			},
			{
				name: "a1",
				description: "Başv_metni'nde geçen başvuru türünü belirten mantıksal değer: R1C1-stili = YANLIŞ; A1-stili = DOĞRU ya da atlanmış"
			}
		]
	},
	{
		name: "DÖRTTEBİRLİK",
		description: "Bir veri kümesinin dörttebirliğini verir.",
		arguments: [
			{
				name: "dizi",
				description: "dörttebir değerini istediğiniz sayısal değerler dizisi veya hücreler aralığı"
			},
			{
				name: "dörttebir",
				description: "sayı: en küçük değer = 0; birinci dörttebir = 1; medyan değeri = 2; üçüncü dörttebir = 3; en büyük değer = 4"
			}
		]
	},
	{
		name: "DÖRTTEBİRLİK.DHL",
		description: "0..1 (bunlar dahil) aralığındaki yüzdebir değerlerini temel alarak veri kümesinin dörttebirliğini verir.",
		arguments: [
			{
				name: "dizi",
				description: "dörttebir değerini istediğiniz sayısal değerler dizisi veya hücreler aralığı"
			},
			{
				name: "dörttebir",
				description: "sayı: en küçük değer = 0; birinci dörtebir = 1; medyan değeri = 2; üçüncü dörtebir = 3; en büyük değer = 4"
			}
		]
	},
	{
		name: "DÖRTTEBİRLİK.HRC",
		description: "0..1 (bunlar hariç) aralığındaki yüzdebir değerlerini temel alarak veri kümesinin dörttebirliğini verir.",
		arguments: [
			{
				name: "dizi",
				description: "dörttebir değerini istediğiniz sayısal değerler dizisi veya hücreler aralığı"
			},
			{
				name: "dörttebir",
				description: "sayı: en küçük değer = 0; birinci dörtebir = 1; medyan değeri = 2; üçüncü dörtebir = 3; en büyük değer = 4"
			}
		]
	},
	{
		name: "DOT",
		description: "En küçük kareler yöntemiyle hesaplanmış olan verilerinize en iyi uyan doğruyu tanımlayan diziyi verir.",
		arguments: [
			{
				name: "bilinen_y'ler",
				description: "y=mx+b denklemindeki y-değerleri kümesi"
			},
			{
				name: "bilinen_x'ler",
				description: "y=mx+b denkleminde kullanılan isteğe bağlı x değerleri"
			},
			{
				name: "sabit",
				description: "mantıksal değer: Sabit = DOĞRU ya da atlanmış ise sabit b değeri olağan şekilde hesaplanır; Sabit = YANLIŞ ise b 0'a eşitlenir"
			},
			{
				name: "konum",
				description: "mantıksal değer: ek regresyon istatistiği = DOĞRU; m-katsayıları ve sabit b değeri = YANLIŞ ya da atlanmış"
			}
		]
	},
	{
		name: "DÜŞEYARA",
		description: "Bir tablonun en sol sütunundaki bir değeri arar ve daha sonra aynı satırda belirttiğiniz sütundan bir değer verir. Varsayılan olarak tablo artan sırada sıralanmalıdır.",
		arguments: [
			{
				name: "aranan_değer",
				description: "tablonun ilk sütununda bulunacak değerdir ve bir değer, bir başvuru ya da bir metin dizesi olabilir"
			},
			{
				name: "tablo_dizisi",
				description: "verinin alınacağı bir metin, sayılar ya da mantıksal değerler tablosu. Tablo_dizisi bir aralığa ya da aralık adına yapılan başvuru olabilir"
			},
			{
				name: "sütun_indis_sayısı",
				description: "uyuşan değerin verileceği tablo_dizisi'ndeki sütun sayısı. Tablodaki ilk değer sütunu sütun1'dir"
			},
			{
				name: "aralık_bak",
				description: "mantıksal değer: ilk sütundaki (artan sırada sıralanmış) en yakın eşleştirmeyi bulmak için = DOĞRU ya da atlanmış; tam bir eşleştirme bulmak için = YANLIŞ"
			}
		]
	},
	{
		name: "EBOŞSA",
		description: "Değer boş bir hücreye başvuruda bulunuyorsa DOĞRU verir.",
		arguments: [
			{
				name: "değer",
				description: "sınamak istediğiniz hücre, ya da hücreye başvuran ad"
			}
		]
	},
	{
		name: "EFORMÜLSE",
		description: "Başvurunun formül içeren bir hücreye yapılıp yapılmadığını denetler ve DOĞRU ya da YANLIŞ değerini döndürür.",
		arguments: [
			{
				name: "başvuru",
				description: "sınamak istediğiniz hücreye yapılan başvuru.  Başvuru bir hücre başvurusu, bir formül ya da hücreye başvuran bir ad olabilir"
			}
		]
	},
	{
		name: "EĞER",
		description: "Belirttiğiniz koşul DOĞRU olarak değerlendiriliyorsa bir değer, YANLIŞ olarak değerlendiriliyorsa başka bir değer verir.",
		arguments: [
			{
				name: "mantıksal_sınama",
				description: "DOĞRU veya YANLIŞ olarak değerlendirilebilecek bir değer veya ifade"
			},
			{
				name: "eğer_doğruysa_değer",
				description: "mantıksal_sınama DOĞRU olduğunda gelen değer. Atlanırsa, DOĞRU gelir. En çok yedi eğer fonksiyonunu iç içe geçirebilirsiniz"
			},
			{
				name: "eğer_yanlışsa_değer",
				description: "mantıksal_sınama YANLIŞ olduğunda gelen değer. Atlanırsa, YANLIŞ gelir"
			}
		]
	},
	{
		name: "EĞERHATA",
		description: "İfade hatalı olursa eğer_hatalıysa_değer, hatalı olmazsa ifadenin kendi değerini döndürür.",
		arguments: [
			{
				name: "değer",
				description: "herhangi bir değer veya ifade veya başvuru"
			},
			{
				name: "eğer_hatalıysa_değer",
				description: "herhangi bir değer veya ifade veya başvuru"
			}
		]
	},
	{
		name: "EĞERORTALAMA",
		description: "Verili bir koşul veya ölçüt tarafından belirtilen hücrelerin ortalamasını (aritmetik ortalama) bulur.",
		arguments: [
			{
				name: "aralık",
				description: "değerlendirilmesini istediğiniz hücre aralığı"
			},
			{
				name: "ölçüt",
				description: "ortalamayı bulmak için kullanılacak hücreleri tanımlayan, sayı, ifade veya metin biçimindeki koşul veya ölçüt"
			},
			{
				name: "aralık_ortalaması",
				description: "ortalamayı bulmak için kullanılacak asıl hücreler. Yoksayılırsa, aralıktaki hücreler kullanılır "
			}
		]
	},
	{
		name: "EĞERSAY",
		description: "Verilen koşula uyan aralık içindeki hücreleri sayar.",
		arguments: [
			{
				name: "aralık",
				description: "boş olmayan hücrelerin sayısını öğrenmek istediğiniz aralık"
			},
			{
				name: "ölçüt",
				description: "hangi hücrelerin sayılacağını tanımlayan sayı, ifade veya metin biçimindeki koşul"
			}
		]
	},
	{
		name: "EĞERYOKSA",
		description: "İfade #N/A olarak çözümlenirse belirttiğiniz değeri döndürür, aksi durumda ifadenin sonucunu döndürür.",
		arguments: [
			{
				name: "değer",
				description: "herhangi bir değer, ifade veya başvuru"
			},
			{
				name: "değer_eğer_yok",
				description: "herhangi bir değer, ifade veya başvuru"
			}
		]
	},
	{
		name: "EĞİLİM",
		description: "Bilinen değerlere en küçük kareler yöntemini uygulayarak değerleri bir doğruya uydurur ve bir doğrusal eğilim boyunca verir.",
		arguments: [
			{
				name: "bilinen_y'ler",
				description: "y = mx+b denklemindeki bilinen y-değerleri aralığı ya da dizisi"
			},
			{
				name: "bilinen_x'ler",
				description: "y = mx+b denkleminde kullanılan isteğe bağlı bilinen x-değerleri aralığı ya da dizisi, Bilinen_y'lerle aynı boyuttaki dizi"
			},
			{
				name: "yeni_x'ler",
				description: "ilişkili y-değerlerini EĞİLİM ile bulacağınız yeni x-değerleri aralığı ya da dizisi"
			},
			{
				name: "sabit",
				description: "mantıksal değer: Sabit = DOĞRU ya da atlanmış ise sabit b değeri olağan şekilde hesaplanır; Sabit = YANLIŞ ise b 0'a eşitlenir"
			}
		]
	},
	{
		name: "EĞİM",
		description: "Verilen veri noktaları boyunca doğrusal regresyon çizgisinin eğimini verir.",
		arguments: [
			{
				name: "bilinen_y'ler",
				description: "sayısal bağımlı veri noktaları dizisi veya hücre aralığıdır ve sayı, ad, dizi ya da sayı içeren başvuru olabilir"
			},
			{
				name: "bilinen_x'ler",
				description: "bağımsız veri noktaları kümesidir ve sayı, ad, dizi, ya da sayı içeren başvuru olabilir"
			}
		]
	},
	{
		name: "EHATA",
		description: "Değerin ,#YOK dışında, hata olup olmadığını denetler (#DEĞER!, #BAŞV!, #SAYI/0!, #SAYI!, #AD?, veya #BOŞ!) ve DOĞRU veya YANLIŞ verir.",
		arguments: [
			{
				name: "değer",
				description: "sınamak istediğiniz değerdir. Bu değer, bir hücreye, bir formüle ya bunlara veya bir değere başvuran bir ada başvuruda bulunabilir"
			}
		]
	},
	{
		name: "EHATALIYSA",
		description: "Değerin hata olup olmadığını denetler (#YOK, #DEĞER!, #BAŞV!, #SAYI/0!, #SAYI!, #AD?, veya #BOŞ!) DOĞRU veya YANLIŞ verir.",
		arguments: [
			{
				name: "değer",
				description: "sınamak istediğiniz değerdir. Bu değer, bir hücreye, bir formüle ya bunlara veya bir değere başvuran bir ada başvuruda bulunabilir"
			}
		]
	},
	{
		name: "ELEMAN",
		description: "Bir dizin numarasını temel alan bir değerler listesinden gerçekleştirmek üzere bir değer ya da eylem seçer.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "dizin_sayısı",
				description: "hangi değer bağımsız değişkeninin seçildiğini belirtir. Dizin_num 1 ile 254 arasında, bir formül ya da 1 ile 254 arasındaki bir sayıya başvuru olmalıdır"
			},
			{
				name: "değer1",
				description: "ELEMAN ile seçilen en az 1 en çok 254 sayı, hücre başvurusu, tanımlanmış ad, formül, işlev ya da metin biçiminde bağımsız değer olabilir"
			},
			{
				name: "değer2",
				description: "ELEMAN ile seçilen en az 1 en çok 254 sayı, hücre başvurusu, tanımlanmış ad, formül, işlev ya da metin biçiminde bağımsız değer olabilir"
			}
		]
	},
	{
		name: "EMANTIKSALSA",
		description: "Bir değerin mantıksal bir değer (DOĞRU veya YANLIŞ) olup olmadığını denetler ve DOĞRU veya YANLIŞ değerini döndürür.",
		arguments: [
			{
				name: "değer",
				description: "sınamak istediğiniz değer. Bu değer, bir hücreye, bir formüle ya bunlara veya bir değere başvuran bir ada başvuruda bulunabilir"
			}
		]
	},
	{
		name: "EMETİNDEĞİLSE",
		description: "Bir değerin metin olup olmadığını denetler (boş hücreler metin değildir) ve metin değilse DOĞRU, metinse YANLIŞ döndürür.",
		arguments: [
			{
				name: "değer",
				description: "sınamak istediğiniz değer: bir hücre; bir formül; ya da bir hücreye, formüle veya değere başvuran bir ad"
			}
		]
	},
	{
		name: "EMETİNSE",
		description: "Bir değerin metin olup olmadığını denetler ve metinse DOĞRU, metin değilse YANLIŞ döndürür.",
		arguments: [
			{
				name: "değer",
				description: "sınamak istediğiniz değer. Bu değer, bir hücreye, bir formüle ya da bunlara veya bir değere başvuran bir ada başvuruda bulunabilir"
			}
		]
	},
	{
		name: "ENÇOK_OLAN",
		description: "Bir veri dizisi ya da aralığında en sık karşılaşılan ya da en çok yinelenen değeri verir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "modunu bulmak istediğiniz en az 1 en fazla 255 sayı, ad, dizi ya da sayı içeren başvurudur"
			},
			{
				name: "sayı2",
				description: "modunu bulmak istediğiniz en az 1 en fazla 255 sayı, ad, dizi ya da sayı içeren başvurudur"
			}
		]
	},
	{
		name: "ENÇOK_OLAN.ÇOK",
		description: "Bir veri dizisi veya aralığında en sık karşılaşılan veya en çok yinelenen değerleri içeren dikey bir dizi verir. Yatay bir dizi için, =DEVRİK_DÖNÜŞÜM(ENÇOK_OLAN.ÇOK(sayı1,sayı2,...)) kullanın.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "modunu bulmak istediğiniz en az 1 en fazla 255 sayı, ad, dizi ya da sayı içeren başvurudur"
			},
			{
				name: "sayı2",
				description: "modunu bulmak istediğiniz en az 1 en fazla 255 sayı, ad, dizi ya da sayı içeren başvurudur"
			}
		]
	},
	{
		name: "ENÇOK_OLAN.TEK",
		description: "Bir veri dizisi ya da aralığında en sık karşılaşılan ya da en çok yinelenen değeri verir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "modunu bulmak istediğiniz en az 1 en fazla 255 sayı, ad, dizi ya da sayı içeren başvurudur"
			},
			{
				name: "sayı2",
				description: "modunu bulmak istediğiniz en az 1 en fazla 255 sayı, ad, dizi ya da sayı içeren başvurudur"
			}
		]
	},
	{
		name: "EREFSE",
		description: "Bir değerin başvuru olup olmadığını denetler ve başvuruysa DOĞRU, değilse YANLIŞ döndürür.",
		arguments: [
			{
				name: "değer",
				description: "sınamak istediğiniz değer. Bu değer, bir hücreye, bir formüle ya da bunlara veya bir değere başvuran bir ada başvuruda bulunabilir"
			}
		]
	},
	{
		name: "ESAYIYSA",
		description: "Bir değerin sayı olup olmadığını denetler ve sayıysa DOĞRU, değilse YANLIŞ döndürür.",
		arguments: [
			{
				name: "değer",
				description: "sınamak istediğiniz değer. Bu değer, bir hücreye, bir formüle ya da bunlara veya bir değere başvuran bir ada başvuruda bulunabilir"
			}
		]
	},
	{
		name: "ETARİHLİ",
		description: "",
		arguments: [
		]
	},
	{
		name: "ETKİN",
		description: "Etkin bileşik faiz oranını döndürür.",
		arguments: [
			{
				name: "nominal_oran",
				description: "nominal faiz oranı"
			},
			{
				name: "dönem_sayısı",
				description: "yıl başına bileşim sayısı"
			}
		]
	},
	{
		name: "ETOPLA",
		description: "Verilen bir koşul ya da ölçüt tarafından belirtilen hücreleri ekler.",
		arguments: [
			{
				name: "aralık",
				description: "değerini bulmak istediğiniz hücreler aralığı"
			},
			{
				name: "ölçüt",
				description: "hangi hücrelerin ekleneceğini tanımlayan sayı, ifade ya da metin formundaki ölçüt veya koşul"
			},
			{
				name: "toplam_aralığı",
				description: "toplanacak gerçek hücreler. Atlanırsa, aralıktaki hücreler kullanılır"
			}
		]
	},
	{
		name: "EYOKSA",
		description: "Değerin #YOK olup olmadığını denetler ve DOĞRU ya da YANLIŞ verir.",
		arguments: [
			{
				name: "değer",
				description: "sınamak istediğiniz değer. Bu değer, bir hücreye, bir formüle ya bunlara veya bir değere başvuran bir ada başvuruda bulunabilir"
			}
		]
	},
	{
		name: "F.DAĞ",
		description: "İki veri kümesi için (sol kuyruklu) F olasılık dağılımını (basıklık derecesi) verir.",
		arguments: [
			{
				name: "x",
				description: "işlevin sonucunu veren değer, negatif olmayan bir sayı"
			},
			{
				name: "serb_derecesi1",
				description: "serbestlik derecesi payında olan sayı, 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			},
			{
				name: "serb_derecesi2",
				description: "serbestlik derecesi paydası, 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			},
			{
				name: "kümülatif",
				description: "işlevin vereceği mantıksal değer: kümülatif dağılım fonksiyonu = DOĞRU; olasılık yoğunluğu fonksiyonu = YANLIŞ"
			}
		]
	},
	{
		name: "F.DAĞ.SAĞK",
		description: "İki veri kümesi için (sağ kuyruklu) F olasılık dağılımını (basıklık derecesi) verir.",
		arguments: [
			{
				name: "x",
				description: "işlevin sonucunu veren değer, negatif olmayan bir sayı"
			},
			{
				name: "serb_derecesi1",
				description: "serbestlik derecesi payında olan sayı, 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			},
			{
				name: "serb_derecesi2",
				description: "serbestlik derecesi paydası, 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			}
		]
	},
	{
		name: "F.TERS",
		description: "(Sol kuyruklu) F olasılık dağılımının tersini verir: p = F.DAĞ(x,...) ise, F.TERS(p,...) = x.",
		arguments: [
			{
				name: "olasılık",
				description: "F kümülatif dağılımı ile ilgili olasılık; 0 ile 1 arasında (bunlar dahil) bir sayı"
			},
			{
				name: "serb_derecesi1",
				description: "serbestlik derecesi payında olan sayı, 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			},
			{
				name: "serb_derecesi2",
				description: "serbestlik derecesi paydası, 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			}
		]
	},
	{
		name: "F.TERS.SAĞK",
		description: "(Sağ kuyruklu) F olasılık dağılımının tersini verir: p = F.DAĞ.SAĞK(x,...) ise, F.TERS.SAĞK(p,...) = x.",
		arguments: [
			{
				name: "olasılık",
				description: "F kümülatif dağılımı ile ilgili olasılık; 0 ile 1 arasında (bunlar dahil) bir sayı"
			},
			{
				name: "serb_derecesi1",
				description: "serbestlik derecesi payında olan sayı, 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			},
			{
				name: "serb_derecesi2",
				description: "serbestlik derecesi paydası, 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Bir F-test sonucu verir; Dizi1 ve Dizi2'nin varyanslarının çok farklı olmadığı iki kuyruklu olasılıktır.",
		arguments: [
			{
				name: "dizi1",
				description: "ilk veri aralığı ya da dizisidir ve sayı, ad, dizi, ya da sayı içeren başvuru olabilir (boşluklar yoksayılır)"
			},
			{
				name: "dizi2",
				description: "ikinci veri aralığı ya da dizisidir ve sayı, ad, dizi, ya da sayı içeren başvuru olabilir (boşluklar yoksayılır)"
			}
		]
	},
	{
		name: "FAİZ_ORANI",
		description: "Bir yıllık borç ya da yatırım için dönem başına düşen faiz oranını verir. Örneğin, %6 yıllık faiz oranına karşılık üç aylık ödeme için %6/4 kullanın.",
		arguments: [
			{
				name: "dönem_sayısı",
				description: "bir yıllık ödeme ya da borç için toplam ödeme dönemi sayısı"
			},
			{
				name: "devresel_ödeme",
				description: "her dönem yapılan ödeme, yıllık ödeme ya da borç dönemi süresince değişemez"
			},
			{
				name: "bd",
				description: "bugünkü değer: gelecekte yapılacak bir dizi ödemenin bugünkü değerini gösteren toplam miktar"
			},
			{
				name: "gd",
				description: "gelecek değer ya da son ödeme yapıldıktan sonra elde edilmek istenen nakit bakiyesi. Atlanırsa, Gd = 0"
			},
			{
				name: "tür",
				description: "mantıksal değer: dönem başındaki ödemeler = 1; dönem sonundaki ödemeler = 0 ya da atlanmış"
			},
			{
				name: "tahmin",
				description: "oran için sizin tahmininiz; atlanırsa, Tahmin = 0,1 (yüzde 10)"
			}
		]
	},
	{
		name: "FAİZORANI",
		description: "Tamamı yatırılan tahvil için faiz oranını döndürür.",
		arguments: [
			{
				name: "düzenleme",
				description: "teminatın düzenleme tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "vade",
				description: "teminatın vade tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "yatırım",
				description: "teminatta yatırılan miktar"
			},
			{
				name: "teminat",
				description: "vadesinde alınacak miktar"
			},
			{
				name: "temel",
				description: "kullanılacak gün sayısı türü"
			}
		]
	},
	{
		name: "FAİZTUTARI",
		description: "Verilen bir dönem için, dönemsel sabit ödemelere ve sabit bir faiz oranını kullanan yatırım faiz ödemesini verir.",
		arguments: [
			{
				name: "oran",
				description: "dönem başına düşen faiz oranı/ Örneğin, %6 yıllık faiz oranına karşılık üç aylık ödeme için %6/4 kullanın"
			},
			{
				name: "dönem",
				description: "faiz tutarını bulmak istediğiniz ve 1 ile dönem sayısı arasında olması gereken dönem"
			},
			{
				name: "dönem_sayısı",
				description: "bir yıllık ödeme içerisindeki toplam ödeme dönemi sayısı"
			},
			{
				name: "bd",
				description: "bugünkü değer veya gelecekte yapılacak bir dizi ödemenin bugünkü toplam değeri"
			},
			{
				name: "gd",
				description: "son ödeme yapıldıktan sonra elde edilmek istenen nakit bakiyesi veya gelecek değer. Atlanırsa, Gd = 0"
			},
			{
				name: "tür",
				description: "son ödeme gününü gösteren mantıksal değer: dönem sonu = 0 ya da atlanmış, dönem başı = 1"
			}
		]
	},
	{
		name: "FDAĞ",
		description: "İki veri kümesi için (sağ kuyruklu) F olasılık dağılımını (basıklık derecesi) verir.",
		arguments: [
			{
				name: "x",
				description: "işlevin sonucunu veren değer, negatif olmayan bir sayı"
			},
			{
				name: "serb_derecesi1",
				description: "serbestlik derecesi payında olan sayı, 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			},
			{
				name: "serb_derecesi2",
				description: "serbestlik derecesi paydası, 1 ile 10^10 arasında (10^10 hariç) bir sayı"
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
		name: "FISHER",
		description: "Fisher dönüşümünü verir.",
		arguments: [
			{
				name: "x",
				description: "dönüşümünü istediğiniz değer, -1 ile 1 arasında (-1 ve 1 hariç) bir sayı"
			}
		]
	},
	{
		name: "FISHERTERS",
		description: "Fisher dönüşümünün tersini verir: y = FISHER(x) ise, FISHERTERS(y) = x.",
		arguments: [
			{
				name: "y",
				description: "dönüşümün tersini almak istediğiniz değer"
			}
		]
	},
	{
		name: "FORMÜLMETNİ",
		description: "Formülü bir dize olarak verir.",
		arguments: [
			{
				name: "başvuru",
				description: "bir formül başvurusu"
			}
		]
	},
	{
		name: "FTERS",
		description: "(Sağ kuyruklu) F olasılık dağılımının tersini verir: p = FDAĞ(x,...) ise, FTERS(p,...) = x.",
		arguments: [
			{
				name: "olasılık",
				description: "F kümülatif dağılımı ile ilgili olasılık; 0 ile 1 arasında (bunlar dahil) bir sayı"
			},
			{
				name: "serb_derecesi1",
				description: "serbestlik derecesi payında olan sayı, 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			},
			{
				name: "serb_derecesi2",
				description: "serbestlik derecesi paydası, 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			}
		]
	},
	{
		name: "FTEST",
		description: "Bir F-test sonucu verir; Dizi1 ve Dizi2'nin varyanslarının çok farklı olmadığı iki kuyruklu olasılıktır.",
		arguments: [
			{
				name: "dizi1",
				description: "ilk veri aralığı ya da dizisidir ve sayı, ad, dizi, ya da sayı içeren başvuru olabilir (boşluklar yoksayılır)"
			},
			{
				name: "dizi2",
				description: "ikinci veri aralığı ya da dizisidir ve sayı, ad, dizi, ya da sayı içeren başvuru olabilir (boşluklar yoksayılır)"
			}
		]
	},
	{
		name: "GAMA",
		description: "Gama işlevi değerini verir.",
		arguments: [
			{
				name: "x",
				description: " Gama değerini hesaplamak istediğiniz değer"
			}
		]
	},
	{
		name: "GAMA.DAĞ",
		description: "Gama dağılımını verir.",
		arguments: [
			{
				name: "x",
				description: "dağılımın değerini bulmak istediğiniz değer, negatif olmayan bir sayı"
			},
			{
				name: "alfa",
				description: "dağılım parametresi, pozitif bir sayı"
			},
			{
				name: "beta",
				description: "dağılım parametresi, pozitif bir sayı. beta = 1 ise, GAMA.DAĞ standart gama dağılımını verir"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif dağılım işlevi = DOĞRU; olasılık kütle işlevi = YANLIŞ ya da atlanmış"
			}
		]
	},
	{
		name: "GAMA.TERS",
		description: "Gama kümülatif dağılımının tersini verir: p = GAMA.DAĞ(x,...) ise, GAMA.TERS(p,...) = x.",
		arguments: [
			{
				name: "olasılık",
				description: "gama dağılımıyla ilgili olasılık, 0 ile 1 arasında (bunlar dahil) bir sayı"
			},
			{
				name: "alfa",
				description: "dağılım parametresi, pozitif bir sayı"
			},
			{
				name: "beta",
				description: "dağılım parametresi, pozitif bir sayı. beta = 1 ise, GAMA.TERS standart gama dağılımının tersini verir"
			}
		]
	},
	{
		name: "GAMADAĞ",
		description: "Gama dağılımını verir.",
		arguments: [
			{
				name: "x",
				description: "dağılımın değerini bulmak istediğiniz değer, negatif olmayan bir sayı"
			},
			{
				name: "alfa",
				description: "dağılım parametresi, pozitif bir sayı"
			},
			{
				name: "beta",
				description: "dağılım parametresi, pozitif bir sayı. beta = 1 ise, GAMADAĞ standart gama dağılımını verir"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif dağılım fonksiyonu = DOĞRU; olasılık kütle fonksiyonu = YANLIŞ ya da atlanmış"
			}
		]
	},
	{
		name: "GAMALN",
		description: "Gama fonksiyonunun doğal logaritmasını verir.",
		arguments: [
			{
				name: "x",
				description: "GAMALN'ini hesaplamak istediğiniz değer, pozitif bir sayı"
			}
		]
	},
	{
		name: "GAMALN.DUYARLI",
		description: "Gama işlevinin doğal logaritmasını döndürür.",
		arguments: [
			{
				name: "x",
				description: "GAMALN.DUYARLI işlevini hesaplamak istediğiniz değer, pozitif bir sayı"
			}
		]
	},
	{
		name: "GAMATERS",
		description: "Gama kümülatif dağılımının tersini verir: p = GAMADAĞ(x,...) ise, GAMATERS(p,...) = x.",
		arguments: [
			{
				name: "olasılık",
				description: "gama dağılımıyla ilgili olasılık, 0 ile 1 arasında (bunlar dahil) bir sayı"
			},
			{
				name: "alfa",
				description: "dağılım parametresi, pozitif bir sayı"
			},
			{
				name: "beta",
				description: "dağılım parametresi, pozitif bir sayı. beta = 1 ise, GAMATERS standart gama dağılımının tersini verir"
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
		name: "GD",
		description: "Bir yatırımın gelecek değerini, dönemsel sabit ödemeler ve sabit faiz oranı kullanarak hesaplar.",
		arguments: [
			{
				name: "oran",
				description: "dönem başına faiz oranı. Örneğin, %6 yıllık faiz oranına karşılık üç aylık ödeme için %6/4 kullanın"
			},
			{
				name: "dönem_sayısı",
				description: "bir yıllık ödeme içerisindeki toplam ödeme dönemi sayısı"
			},
			{
				name: "devresel_ödeme",
				description: "her dönem yapılan ödeme miktarı; yıllık taksit dönemi boyunca değişemez"
			},
			{
				name: "bd",
				description: "bugünkü değer veya gelecekte yapılacak bir dizi ödemenin bugünkü değerini veren küme toplamı. Atlanırsa, Bd = 0"
			},
			{
				name: "tür",
				description: "ödemelerin son gününü gösteren değer: dönem başında = 1; dönem sonunda = 0 ya da atlanmış"
			}
		]
	},
	{
		name: "GDPROGRAM",
		description: "Anaparanın, bir seri bileşik faiz uygulandıktan sonra, gelecekteki değerini verir.",
		arguments: [
			{
				name: "anapara",
				description: "şimdiki değer"
			},
			{
				name: "program",
				description: "uygulanacak faiz oranları serisi"
			}
		]
	},
	{
		name: "GEOORT",
		description: "Bir dizi ya da pozitif sayısal veri aralığının geometrik ortalamasını verir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "ortalamasını bulmak istediğiniz en az 1 en fazla 255 sayı, ad, dizi, ya da sayı içeren başvurudur"
			},
			{
				name: "sayı2",
				description: "ortalamasını bulmak istediğiniz en az 1 en fazla 255 sayı, ad, dizi, ya da sayı içeren başvurudur"
			}
		]
	},
	{
		name: "GERÇEKFAİZV",
		description: "Vadesinde faiz ödeyen bir tahvil için tahakkuk eden vergiyi döndürür.",
		arguments: [
			{
				name: "çıkış",
				description: "teminatın veriliş tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "düzenleme",
				description: "teminatın vade tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "oran",
				description: "teminatın yıllık kupon oranı"
			},
			{
				name: "par",
				description: "teminatın par değeri"
			},
			{
				name: "temel",
				description: "kullanılacak gün sayısı türü"
			}
		]
	},
	{
		name: "GERÇEKLEŞENYATIRIMGETİRİSİ",
		description: "Yatırımın büyümesi için eşdeğer bir faiz oranı verir.",
		arguments: [
			{
				name: "dönem_sayısı",
				description: "yatırım dönemlerinin sayısı"
			},
			{
				name: "bd",
				description: "yatırımın bugünkü değeri"
			},
			{
				name: "gd",
				description: "yatırımın gelecekteki değeri"
			}
		]
	},
	{
		name: "GETİRİ",
		description: "Tamamen yatırılan bir tahvilin vadesindeki getiri miktarını döndürür.",
		arguments: [
			{
				name: "düzenleme",
				description: "teminatın düzenleme tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "vade",
				description: "teminatın vade tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "yatırım",
				description: "teminatta yatırılan miktar"
			},
			{
				name: "indirim",
				description: "teminatın indirim oranı"
			},
			{
				name: "temel",
				description: "kullanılacak gün sayısı türü"
			}
		]
	},
	{
		name: "GÜN",
		description: "1 ile 31 arasındaki, ayın gününü döndürür.",
		arguments: [
			{
				name: "seri_no",
				description: "Microsoft Office tarafından kullanılan tarih-saat kodundaki sayı"
			}
		]
	},
	{
		name: "GÜN360",
		description: "İki tarih arasındaki gün sayısını 360 günlük yılı kullanarak hesaplar (oniki 30 günlük ay).",
		arguments: [
			{
				name: "başlangıç_tarihi",
				description: "başlangıç_tarihi ve son_tarih, iki tarih arasındaki gün sayısını bulmak için kullanılır"
			},
			{
				name: "bitiş_tarihi",
				description: "başlangıç_tarihi ve son_tarih, iki tarih arasındaki gün sayısını bulmak için kullanılır"
			},
			{
				name: "yöntem",
				description: "hesaplama yöntemini belirten mantıksal değer: ABD (NASD) = YANLIŞ ya da atlanmış; Avrupa = DOĞRU."
			}
		]
	},
	{
		name: "GÜNSAY",
		description: "İki tarih arasındaki gün sayısını döndürür.",
		arguments: [
			{
				name: "bitiş_tarihi",
				description: "başlangıç_tarihi ve bitiş_tarihi aralarındaki gün sayısını bilmek istediğiniz iki tarih"
			},
			{
				name: "başlangıç_tarihi",
				description: "başlangıç_tarihi ve bitiş_tarihi aralarındaki gün sayısını bilmek istediğiniz iki tarih"
			}
		]
	},
	{
		name: "GÜVENİLİRLİK.NORM",
		description: "Popülasyon ortalaması için normal bir dağılım kullanarak güvenilirlik aralığını verir.",
		arguments: [
			{
				name: "alfa",
				description: "güvenirlik düzeyini hesaplamak için kullanılan anlamlılık düzeyi; 0'dan büyük 1'den küçük bir sayı"
			},
			{
				name: "standart_sapma",
				description: "veri aralığının bilindiği varsayılan popülasyon standart sapması. Standart_sap 0'dan büyük olmalıdır"
			},
			{
				name: "boyut",
				description: "örnek boyutu"
			}
		]
	},
	{
		name: "GÜVENİLİRLİK.T",
		description: "Popülasyon ortalaması için bir T-dağılımı kullanarak güvenilirlik aralığını verir.",
		arguments: [
			{
				name: "alfa",
				description: "güvenirlik düzeyini hesaplamak için kullanılan anlamlılık düzeyi; 0'dan büyük 1'den küçük bir sayı"
			},
			{
				name: "standart_sapma",
				description: "veri aralığının bilindiği varsayılan popülasyon standart sapması. Standart_sap 0'dan büyük olmalıdır"
			},
			{
				name: "boyut",
				description: "örnek boyutu"
			}
		]
	},
	{
		name: "GÜVENİRLİK",
		description: "Popülasyon ortalaması için normal bir dağılım kullanarak güvenilirlik aralığını verir.",
		arguments: [
			{
				name: "alfa",
				description: "güvenirlik düzeyini hesaplamak için kullanılan anlamlılık düzeyi; 0'dan büyük 1'den küçük bir sayı"
			},
			{
				name: "standart_sapma",
				description: "veri aralığının bilindiği varsayılan popülasyon standart sapması. Standart_sap 0'dan büyük olmalıdır"
			},
			{
				name: "boyut",
				description: "örnek boyutu"
			}
		]
	},
	{
		name: "GZV",
		description: "COM otomasyonunu destekleyen bir programdan gerçek zamanlı veri alır.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "kayıtlı COM otomasyon eklentisinin ProgID adıdır. Adı çift tırnak içine alın"
			},
			{
				name: "sunucu",
				description: "eklentinin üzerinde çalışması gereken sunucunun adıdır. Adı çift tırnak içine alın. Eklenti yerel çalışıyorsa boş dize kullanın"
			},
			{
				name: "konu1",
				description: "bir parça veriyi tanımlayan 1 ile 38 arasında parametrelerdir"
			},
			{
				name: "konu2",
				description: "bir parça veriyi tanımlayan 1 ile 38 arasında parametrelerdir"
			}
		]
	},
	{
		name: "HAFTANINGÜNÜ",
		description: "Verilen tarih gösteren sayıyı kullanarak haftanın gününü tanımlayan 1 ile 7 arasındaki sayı.",
		arguments: [
			{
				name: "seri_no",
				description: "bir tarih gösteren sayı"
			},
			{
				name: "döndür_tür",
				description: "bir sayı: Pazar = 1'den Cumartesi = 7'ye ise, 1'i kullanın; Pazartesi = 1'den Pazar = 7'ye ise, 2'yi kullanın; Pazartesi = 0'dan Pazar = 6'ya ise, 3'ü kullanın"
			}
		]
	},
	{
		name: "HAFTASAY",
		description: "Yıl içinde haftanın numarasını döndürür.",
		arguments: [
			{
				name: "seri_num",
				description: "Spreadsheet tarafından tarih ve zaman hesaplamalarında kullanılan tarih-zaman kodu"
			},
			{
				name: "dönüş_türü",
				description: "döndürülen değerin türünü belirleyen sayı(1 veya 2)"
			}
		]
	},
	{
		name: "HARORT",
		description: "Pozitif sayılardan oluşan bir veri kümesinin harmonik ortalamasını verir: devrik değerlerin aritmetik ortalamasının devrik değeridir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "harmonik ortalamasını bulmak istediğiniz en az 1 en fazla 255 sayı, ad, dizi, ya da sayı içeren başvurudur"
			},
			{
				name: "sayı2",
				description: "harmonik ortalamasını bulmak istediğiniz en az 1 en fazla 255 sayı, ad, dizi, ya da sayı içeren başvurudur"
			}
		]
	},
	{
		name: "HATA.TİPİ",
		description: "Bir hata değerine karşılık gelen bir sayı verir.",
		arguments: [
			{
				name: "hata_değer",
				description: "tanımlayıcı numarasını bulmak istediğiniz hata değeridir ve gerçek bir hata değeri ya da bir hata değeri içeren bir hücreye yapılan başvuru olabilir"
			}
		]
	},
	{
		name: "HATAİŞLEV",
		description: "Hata işlevini döndürür.",
		arguments: [
			{
				name: "alt_limit",
				description: "ERF için alt sınır"
			},
			{
				name: "üst_limit",
				description: "ERF için üst sınır"
			}
		]
	},
	{
		name: "HATAİŞLEV.DUYARLI",
		description: "Hata işlevini döndürür.",
		arguments: [
			{
				name: "X",
				description: "HATAİŞLEV.DUYARLI işlevini tümleştirmek için alt sınırdır"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "Onaltılık düzendeki bir sayıyı ikilik düzene dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz onaltılık düzendeki sayı"
			},
			{
				name: "basamak",
				description: "kullanılacak karakter sayısı"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "Onaltılık düzendeki bir sayıyı onluk düzene çevirir.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz onaltılık düzendeki sayı"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "Onaltılık düzendeki bir sayıyı sekizlik düzene dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz onaltılık düzendeki sayı"
			},
			{
				name: "basamak",
				description: "kullanılacak karakter sayısı"
			}
		]
	},
	{
		name: "HİPERGEOM.DAĞ",
		description: "Hipergeometrik dağılımı verir.",
		arguments: [
			{
				name: "başarı_örnek",
				description: "örnek içindeki başarı sayısı"
			},
			{
				name: "örnek_sayısı",
				description: "örnek boyutu"
			},
			{
				name: "başarı_popülasyon",
				description: "popülasyondaki başarı sayısı"
			},
			{
				name: "pop_sayısı",
				description: "popülasyon boyutu"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif dağılım fonksiyonu için DOĞRU'yu; olasılık yoğunluk fonksiyonu için YANLIŞ'ı kullanın"
			}
		]
	},
	{
		name: "HİPERGEOMDAĞ",
		description: "Hipergeometrik dağılımı verir.",
		arguments: [
			{
				name: "başarı_örnek",
				description: "örnek içindeki başarı sayısı"
			},
			{
				name: "sayı_örnek",
				description: "örnek boyutu"
			},
			{
				name: "başarı_popülasyon",
				description: "popülasyondaki başarı sayısı"
			},
			{
				name: "sayı_pop",
				description: "popülasyon boyutu"
			}
		]
	},
	{
		name: "HTAHDEĞER",
		description: "Hazine tahvili için 100 TL başına yüz değerini döndürür.",
		arguments: [
			{
				name: "düzenleme",
				description: "Hazine tahvilinin düzenleme tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "vade",
				description: "Hazine tahvilinin vade tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "indirim",
				description: "Hazine tahvilinin indirim oranı"
			}
		]
	},
	{
		name: "HTAHEŞ",
		description: "Hazine tahvili için bono eşdeğerini döndürür.",
		arguments: [
			{
				name: "düzenleme",
				description: "Hazine tahvilinin düzenleme tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "vade",
				description: "Hazine tahvilinin vade tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "indirim",
				description: "Hazine tahvilinin indirim oranı"
			}
		]
	},
	{
		name: "HTAHÖDEME",
		description: "Hazine tahvili için getiriyi döndürür.",
		arguments: [
			{
				name: "düzenleme",
				description: "Hazine tahvilinin düzenleme tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "vade",
				description: "Hazine tahvilinin vade tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "ücret",
				description: "Hazine tahvilinin 100 TL başına yüz değeri fiyatı"
			}
		]
	},
	{
		name: "HÜCRE",
		description: "Bir başvurudaki, sayfanın okuma sırasına göre ilk hücrenin biçim, konum ve içeriği hakkında bilgi verir. .",
		arguments: [
			{
				name: "bilgi_türü",
				description: "hangi tür hücre bilgisi istendiğini belirten metin değeri."
			},
			{
				name: "başvuru",
				description: "hakkında bilgi edinmek istediğiniz hücre"
			}
		]
	},
	{
		name: "İÇ_VERİM_ORANI",
		description: "Bir dizi nakit akışı için iç verim oranını verir.",
		arguments: [
			{
				name: "değerler",
				description: "iç verim oranını hesaplamak istediğiniz sayıları içeren dizi veya hücreler başvurusu"
			},
			{
				name: "tahmin",
				description: "tahmininiz, İÇ_VERİM_ORANI sonucuna yakın bir sayıdır; Atlanırsa 0,1 (yüzde 10) kabul edilir"
			}
		]
	},
	{
		name: "İNDİRİM",
		description: "Tahvil için indirim oranını döndürür.",
		arguments: [
			{
				name: "düzenleme",
				description: "teminatın düzenleme tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "vade",
				description: "teminatın vade tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "ücret",
				description: "teminatın 100 TL başına yüz değeri fiyatı"
			},
			{
				name: "teminat",
				description: "teminatın 100 TL yüz değeri başına kefaret değeri"
			},
			{
				name: "temel",
				description: "kullanılacak gün sayısı türü"
			}
		]
	},
	{
		name: "İNDİS",
		description: "Bir tablo ya da aralıktan bir değer ya da değere yapılan başvuruyu verir.",
		arguments: [
			{
				name: "dizi",
				description: "bir hücreler aralığı ya da dizi sabiti."
			},
			{
				name: "satır_sayısı",
				description: "dizi ya da Başvuru'da, almak istediğiniz değeri veren satırı seçer. Atlanırsa, Süt_num gerekir"
			},
			{
				name: "sütun_sayısı",
				description: "dizi ya da Başvuru'da, almak istediğiniz değeri veren sütunu seçer. Atlanırsa, Sat_num gerekir"
			}
		]
	},
	{
		name: "İŞARET",
		description: "Bir sayının işaretini verir: sayı pozitif ise 1, sıfır ise sıfır, negatif ise -1.",
		arguments: [
			{
				name: "sayı",
				description: "herhangi bir gerçek sayı"
			}
		]
	},
	{
		name: "İŞGÜNÜ",
		description: "Belirtilen sayıda işgünü önce veya sonraki bir tarihin seri numarasını döndürür.",
		arguments: [
			{
				name: "başlangıç_tarihi",
				description: "başlangıç tarihini gösteren tarih seri numarası"
			},
			{
				name: "gün_sayısı",
				description: "başlangıç tarihinden önceki veya sonraki tatil veya haftasonu olmayan günler "
			},
			{
				name: "tatiller",
				description: "isteğe göre çalışma takviminden çıkarılacak, bir veya daha fazla tarih seri numarası serisi; örneğin resmi tatiller"
			}
		]
	},
	{
		name: "İŞGÜNÜ.ULUSL",
		description: "Belirtilen sayıda işgünü önce veya sonraki bir tarihin seri numarasını özel hafta sonu parametreleriyle verir.",
		arguments: [
			{
				name: "başlangıç_tarihi",
				description: "başlangıç tarihini gösteren tarih seri numarası"
			},
			{
				name: "gün_sayısı",
				description: "başlangıç tarihinden önceki veya sonraki tatil veya hafta sonu olmayan günler"
			},
			{
				name: "hafta_sonu",
				description: "hafta sonlarının ne zaman olduğunu belirten sayı veya dize"
			},
			{
				name: "tatiller",
				description: "isteğe göre çalışma takviminden çıkarılacak, bir veya daha fazla tarih seri numarası dizisi; örneğin resmi tatiller"
			}
		]
	},
	{
		name: "ISO.TAVAN",
		description: "Bir sayıyı, sıfırdan ıraksayarak, en yakın tamsayı veya anlamlı sayı katına yuvarlar.",
		arguments: [
			{
				name: "sayı",
				description: "yuvarlamak istediğiniz değer"
			},
			{
				name: "anlam",
				description: "kendisine yuvarlamak istediğiniz isteğe bağlı kat"
			}
		]
	},
	{
		name: "ISOHAFTASAY",
		description: "Verilen tarih için yılın ISO hafta numarası sayısını döndürür.",
		arguments: [
			{
				name: "tarih",
				description: "tarih ve saat hesaplama için Spreadsheet tarafından kullanılan tarih-saat kodu"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Borç ödeme faizini verir.",
		arguments: [
			{
				name: "oran",
				description: "dönem başına düşen faiz oranı. Örneğin, %6 yıllık faiz oranına karşılık üç aylık ödeme için %6/4 kullanın"
			},
			{
				name: "dönem",
				description: "faizini bulmak istediğiniz dönem"
			},
			{
				name: "dönem_sayısı",
				description: "bir yıllık ödeme içerisindeki ödeme dönemi sayısı"
			},
			{
				name: "bd",
				description: "gelecekte yapılacak bir dizi ödemenin küme toplamı"
			}
		]
	},
	{
		name: "KAÇINCI",
		description: "Belirli bir sırada belirtilen değerle eşleşen bir öğenin bir dizi içerisindeki göreceli konumunu verir.",
		arguments: [
			{
				name: "aranan_değer",
				description: "dizi içerisinde aradığınız değeri bulmak için kullandığınız değer, sayı, metin, mantıksal değer ya da bunlardan birine yapılan başvuru"
			},
			{
				name: "aranan_dizi",
				description: "aranan değerleri içeren ardışık hücreler aralığı, değerler dizisi ya da dizi başvurusu"
			},
			{
				name: "eşleştir_tür",
				description: "gelen değeri işaret eden 1, 0 ya da -1 sayısı."
			}
		]
	},
	{
		name: "KAREKÖK",
		description: "Bir sayının karekökünü verir.",
		arguments: [
			{
				name: "sayı",
				description: "karekökünü almak istediğiniz sayı"
			}
		]
	},
	{
		name: "KAREKÖKPİ",
		description: "Sayının Pi sayısıyla çarpımının kare kökünü döndürür.",
		arguments: [
			{
				name: "sayı",
				description: "pi sayısıyla çarpılacak sayı"
			}
		]
	},
	{
		name: "KARMAŞIK",
		description: "Gerçel ve sanal katsayıları bir karmaşık sayıya dönüştürür.",
		arguments: [
			{
				name: "gerçel_sayı",
				description: "karmaşık sayının gerçel katsayısı"
			},
			{
				name: "karm_sayı",
				description: "karmaşık sayının sanal katsayısı"
			},
			{
				name: "sonek",
				description: "karmaşık sayının sanal kısmı için sonek"
			}
		]
	},
	{
		name: "KAYDIR",
		description: "Bir hücre ya da hücreler aralığında belirtilen satır ve sütun sayısına karşılık gelen bir aralığa yapılan başvuruyu verir.",
		arguments: [
			{
				name: "başv",
				description: "göreceli konuma temel oluşturmak istediğiniz başvuru, bir hücreye ya da bitişik hücreler aralığına yapılan başvuru"
			},
			{
				name: "satırlar",
				description: "sonucun sol üst hücresinin gösterdiği satır sayısı, aşağı veya yukarı"
			},
			{
				name: "sütunlar",
				description: "sonucun sol-üst hücresinin gösterdiği sütun sayısı, sola veya sağa"
			},
			{
				name: "yükseklik",
				description: "sonucun sahip olmasını istediğiniz yüksekliği (satır sayısı olarak), atlanırsa Başvurunun yüksekliğiyle aynı kabul edilir"
			},
			{
				name: "genişlik",
				description: "sonucun sahip olmasını istediğiniz genişliği (sütun sayısı olarak), atlanırsa Başvurunun genişliğiyle aynı kabul edilir"
			}
		]
	},
	{
		name: "KESMENOKTASI",
		description: "Bilinen x ve y-değerleri üzerindeki en uygun regresyon doğrusunu kullanarak bir doğrunun y-eksenini kestiği noktaları hesaplar.",
		arguments: [
			{
				name: "bilinen_y'ler",
				description: "bağımlı veri ya da gözlemler kümesidir ve sayı, ad, dizi, ya da sayı içeren başvuru olabilir"
			},
			{
				name: "bilinen_x'ler",
				description: "bağımsız veri ya da gözlemler kümesidir ve sayı, ad, dizi, ya da sayı içeren başvuru olabilir"
			}
		]
	},
	{
		name: "KİKARE.DAĞ",
		description: "Kikare dağılımının sol kuyruklu olasılığını verir.",
		arguments: [
			{
				name: "x",
				description: "dağılımın değerini bulmak istediğiniz değer, negatif olmayan bir sayı"
			},
			{
				name: "serb_derecesi",
				description: "serbestlik derecesi sayısı; 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			},
			{
				name: "kümülatif",
				description: "işlevin vereceği mantıksal değer: kümülatif dağılım fonksiyonu = DOĞRU; olasılık yoğunluğu fonksiyonu = YANLIŞ"
			}
		]
	},
	{
		name: "KİKARE.DAĞ.SAĞK",
		description: "Kikare dağılımının sağ kuyruklu olasılığını verir.",
		arguments: [
			{
				name: "x",
				description: "dağılımın değerini bulmak istediğiniz değer, negatif olmayan bir sayı"
			},
			{
				name: "serb_derecesi",
				description: "serbestlik derecesi sayısı; 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			}
		]
	},
	{
		name: "KİKARE.TERS",
		description: "Kikare dağılımının sol kuyruklu olasılığının tersini verir.",
		arguments: [
			{
				name: "olasılık",
				description: "kikare dağılımı ile ilişkili olasılık; 0 ile 1 arasında (bunlar dahil) bir değer"
			},
			{
				name: "serb_derecesi",
				description: "serbestlik derecesi sayısı; 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			}
		]
	},
	{
		name: "KİKARE.TERS.SAĞK",
		description: "Kikare dağılımının sağ kuyruklu olasılığının tersini verir.",
		arguments: [
			{
				name: "olasılık",
				description: "kikare dağılımı ile ilgili olasılık; 0 ile 1 arasında (bunlar dahil) bir değer"
			},
			{
				name: "serb_derecesi",
				description: "serbestlik derecesi sayısı; 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			}
		]
	},
	{
		name: "KİKARE.TEST",
		description: "Bağımsızlık sınaması sonucunu verir: istatistik için kikare dağılımından alınan değer ve uygun serbestlik derecesi.",
		arguments: [
			{
				name: "etkin_erim",
				description: "beklenen değerler karşısında sınanacak gözlemleri içeren veri aralığı"
			},
			{
				name: "beklenen_erim",
				description: "satır toplamları ile sütun toplamları çarpımının büyük toplama oranını içeren veri aralığı"
			}
		]
	},
	{
		name: "KİKAREDAĞ",
		description: "Kikare dağılımının sağ kuyruklu olasılığını verir.",
		arguments: [
			{
				name: "x",
				description: "dağılımın değerini bulmak istediğiniz değer, negatif olmayan bir sayı"
			},
			{
				name: "serb_derecesi",
				description: "serbestlik derecesi sayısı; 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			}
		]
	},
	{
		name: "KİKARETERS",
		description: "Kikare dağılımının sağ kuyruklu olasılığının tersini verir.",
		arguments: [
			{
				name: "olasılık",
				description: "kikare dağılımı ile ilgili olasılık; 0 ile 1 arasında (bunlar dahil) bir değer"
			},
			{
				name: "serb_derecesi",
				description: "serbestlik derecesi sayısı; 1 ile 10^10 arasında (10^10 hariç) bir sayı"
			}
		]
	},
	{
		name: "KİKARETEST",
		description: "Bağımsızlık sınaması sonucunu verir: istatistik için kikare dağılımından alınan değer ve uygun serbestlik derecesi.",
		arguments: [
			{
				name: "etkin_erim",
				description: "beklenen değerler karşısında sınanacak gözlemleri içeren veri aralığı"
			},
			{
				name: "beklenen_erim",
				description: "satır toplamları ile sütun toplamları çarpımının büyük toplama oranını içeren veri aralığı"
			}
		]
	},
	{
		name: "KIRP",
		description: "Bir metin dizesinden sözcükler arasındaki tek boşluklar dışındaki tüm boşlukları kaldırır.",
		arguments: [
			{
				name: "metin",
				description: "boşluklarını kaldırmak istediğiniz metin"
			}
		]
	},
	{
		name: "KIRPORTALAMA",
		description: "Bir veri kümesinin iç kısmının ortalamasını verir.",
		arguments: [
			{
				name: "dizi",
				description: "kırpılıp ortalaması alınacak değerler dizisi veya aralığı"
			},
			{
				name: "yüzde",
				description: "veri kümesinin alt ve üst ucunda bulunan ve hesaplama dışı tutulacak olan veri noktalarının kesirli sayısı"
			}
		]
	},
	{
		name: "KOD",
		description: "Bilgisayarınızın kullandığı karakter kümesinden, metin dizesindeki ilk karakter için sayısal bir kod verir.",
		arguments: [
			{
				name: "metin",
				description: "ilk karakterinin kodunu istediğiniz metin"
			}
		]
	},
	{
		name: "KOMBİNASYON",
		description: "Verilen öğelerin sayısı için kombinasyon sayısını verir.",
		arguments: [
			{
				name: "sayı",
				description: "toplam öğe sayısı"
			},
			{
				name: "sayı_seçilen",
				description: "her kombinasyonda kullanılan öğe sayısı"
			}
		]
	},
	{
		name: "KOMBİNASYONA",
		description: "Verilen sayıda öğe için yinelemelerle birleşimlerin sayısını verir.",
		arguments: [
			{
				name: "sayı",
				description: "öğelerin toplam sayısı"
			},
			{
				name: "sayı_seçilen",
				description: "her birleşimdeki öğe sayısı"
			}
		]
	},
	{
		name: "KÖPRÜ",
		description: "Sabit sürücü, sunucu ağı ya da Internet'te depolanmış olan bir belgeyi açmak için kısayol ya da atlama oluşturur.",
		arguments: [
			{
				name: "bağ_konumu",
				description: "açılması istenen belgeye giden yolu ve dosya adını veren metindir, bir sabit sürücü konumu, UNC adresi, ya da URL yolu"
			},
			{
				name: "yakın_ad",
				description: "hücrede görüntülenen metin ya da sayıdır. Atlandığında, hücre bağ_konumu metnini görüntüler"
			}
		]
	},
	{
		name: "KORELASYON",
		description: "İki veri kümesi arasındaki korelasyon katsayısını verir.",
		arguments: [
			{
				name: "dizi1",
				description: "değer içeren hücre aralığı. Değerler sayı, ad, dizi ya da sayı içeren başvurular olmalıdır"
			},
			{
				name: "dizi2",
				description: "ikinci bir değer içeren hücre aralığı. Değerler sayı, ad, dizi ya da sayı içeren başvurular olmalıdır"
			}
		]
	},
	{
		name: "KOVARYANS",
		description: "Kovaryansı verir; iki veri kümesindeki her veri noktası çifti için sapmaların çarpımlarının ortalaması.",
		arguments: [
			{
				name: "dizi1",
				description: "ilk tamsayılar hücre aralığıdır ve sayı, ad dizi, ya da sayı içeren başvuru olmalıdır"
			},
			{
				name: "dizi2",
				description: "ikinci tamsayılar hücre aralığıdır ve sayı, ad dizi, ya da sayı içeren başvuru olmalıdır"
			}
		]
	},
	{
		name: "KOVARYANS.P",
		description: "Popülasyon kovaryansını verir; iki veri kümesindeki her veri noktası çifti için sapmaların çarpımlarının ortalaması.",
		arguments: [
			{
				name: "dizi1",
				description: "ilk tamsayılar hücre aralığıdır ve sayı, ad dizi, ya da sayı içeren başvuru olmalıdır"
			},
			{
				name: "dizi2",
				description: "ikinci tamsayılar hücre aralığıdır ve sayı, ad dizi, ya da sayı içeren başvuru olmalıdır"
			}
		]
	},
	{
		name: "KOVARYANS.S",
		description: "Örnek kovaryansı verir; iki veri kümesindeki her veri noktası çifti için sapmaların çarpımlarının ortalaması.",
		arguments: [
			{
				name: "dizi1",
				description: "ilk tamsayılar hücre aralığıdır ve sayı, ad dizi, ya da sayı içeren başvuru olmalıdır"
			},
			{
				name: "dizi2",
				description: "ikinci tamsayılar hücre aralığıdır ve sayı, ad dizi, ya da sayı içeren başvuru olmalıdır"
			}
		]
	},
	{
		name: "KRİTİKBİNOM",
		description: "Kümülatif binom dağılımının ölçüt değerinden küçük veya ona eşit olduğu en küçük değeri verir.",
		arguments: [
			{
				name: "denemeler",
				description: "Bernoulli denemeler sayısı"
			},
			{
				name: "başarı_olasılığı",
				description: "her denemedeki başarı olasılığı, 0 ile 1 arasında (bunlar dahil) bir sayı"
			},
			{
				name: "alfa",
				description: "ölçüt değeri, 0 ile 1 arasında (bunlar dahil) bir sayı"
			}
		]
	},
	{
		name: "KÜÇÜK",
		description: "Bir veri kümesinde k. en küçük değeri verir. Örneğin, beşinci en küçük sayı.",
		arguments: [
			{
				name: "dizi",
				description: "k. en küçük değeri bulmak istediğiniz sayısal veri dizisi veya aralığı"
			},
			{
				name: "k",
				description: "gelecek olan değerin bulunduğu hücre aralığı veya dizideki konumu (en küçük değerden)"
			}
		]
	},
	{
		name: "KÜÇÜKHARF",
		description: "Metin dizesindeki tüm büyük harfleri küçük harfe çevirir.",
		arguments: [
			{
				name: "metin",
				description: "küçük harfe dönüştürmek istediğiniz metin. Metindeki harf olmayan karakterler değiştirilmez"
			}
		]
	},
	{
		name: "KUPONGÜNBD",
		description: "Kupon döneminin başlangıcından düzenleme tarihine kadar olan gün sayısını döndürür.",
		arguments: [
			{
				name: "düzenleme",
				description: "teminatın düzenleme tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "vade",
				description: "teminatın vade tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "sıklık",
				description: "yıllık kupon ödemesi sayısı"
			},
			{
				name: "temel",
				description: "kullanılacak gün sayısı türü"
			}
		]
	},
	{
		name: "KUPONGÜNÖKT",
		description: "Düzenleme tarihinden önceki kupon tarihini döndürür.",
		arguments: [
			{
				name: "düzenleme",
				description: "teminatın düzenleme tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "vade",
				description: "teminatın vade tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "sıklık",
				description: "yıllık kupon ödemesi sayısı"
			},
			{
				name: "temel",
				description: "kullanılacak gün sayısı türü"
			}
		]
	},
	{
		name: "KUPONGÜNSKT",
		description: "Düzenleme tarihinden sonraki kupon tarihini döndürür.",
		arguments: [
			{
				name: "düzenleme",
				description: "teminatın düzenleme tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "vade",
				description: "teminatın vade tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "sıklık",
				description: "yıllık kupon ödemesi sayısı"
			},
			{
				name: "temel",
				description: "kullanılacak gün sayısı türü"
			}
		]
	},
	{
		name: "KUPONSAYI",
		description: "Düzenleme ve vade tarihleri arasında ödenebilir kupon sayısını döndürür.",
		arguments: [
			{
				name: "düzenleme",
				description: "teminatın düzenleme tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "vade",
				description: "teminatın vade tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "sıklık",
				description: "yıllık kupon ödemesi sayısı"
			},
			{
				name: "temel",
				description: "kullanılacak gün sayısı türü"
			}
		]
	},
	{
		name: "KUVVET",
		description: "Üssü alınmış sayının sonucunu verir.",
		arguments: [
			{
				name: "sayı",
				description: "taban sayısı, herhangi bir gerçek sayı"
			},
			{
				name: "üs",
				description: "belirli bir tabanda olan sayının üssü"
			}
		]
	},
	{
		name: "KYUVARLA",
		description: "İstenen katsayıya yuvarlanmış bir sayı döndürür.",
		arguments: [
			{
				name: "sayı",
				description: "yuvarlanacak değer"
			},
			{
				name: "katsayı",
				description: "sayıyı yuvarlamak istediğiniz katsayı"
			}
		]
	},
	{
		name: "LİRA",
		description: "Bir sayıyı para biçimi kullanarak metne dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "bir sayı, bir sayı içeren hücreye başvuru ya da işlem sonucu bir sayı olan formül"
			},
			{
				name: "onluklar",
				description: "virgülden sonra sağda ondalık hanesinde kullanılan rakam sayısı. Sayı gerektiği gibi yuvarlanır; atlanırsa, Rakamlar = 2"
			}
		]
	},
	{
		name: "LİRAKES",
		description: "Onluk düzende gösterilen ücreti kesir şekline çevirir.",
		arguments: [
			{
				name: "ondalık_para",
				description: "onluk düzende sayı"
			},
			{
				name: "payda",
				description: "kesrin paydasında kullanılacak tamsayı"
			}
		]
	},
	{
		name: "LİRAON",
		description: "Kesirli olarak gösterilen ücreti onluk düzene çevirir.",
		arguments: [
			{
				name: "kesirli_para",
				description: "kesir cinsinden gösterilen sayı"
			},
			{
				name: "payda",
				description: "kesrin paydasında kullanılacak tamsayı"
			}
		]
	},
	{
		name: "LN",
		description: "Bir sayının doğal logaritmasını verir.",
		arguments: [
			{
				name: "sayı",
				description: "doğal logaritmasını almak istediğiniz pozitif gerçek sayı"
			}
		]
	},
	{
		name: "LOG",
		description: "Bir sayının belirttiğiniz tabandaki logaritmasını alır.",
		arguments: [
			{
				name: "sayı",
				description: "logaritmasını almak istediğiniz pozitif gerçek sayı"
			},
			{
				name: "taban",
				description: "logaritma tabanı; atlanırsa 10"
			}
		]
	},
	{
		name: "LOG10",
		description: "Bir sayının 10 tabanında logaritmasını verir.",
		arguments: [
			{
				name: "sayı",
				description: "10 tabanında logaritmasını almak istediğiniz pozitif gerçek sayı"
			}
		]
	},
	{
		name: "LOGNORM.DAĞ",
		description: "ln(x)'in normal olarak Ortalama ve Standart_sapma parametreleriyle dağıldığı durumlarda x'in lognormal dağılımını verir.",
		arguments: [
			{
				name: "x",
				description: "işlevin sonucunu veren değer, pozitif bir sayı"
			},
			{
				name: "ortalama",
				description: "ln(x)'in ortalaması"
			},
			{
				name: "standart_sapma",
				description: "ln(x)'in standart sapması, pozitif bir sayı"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif dağılım fonksiyonu için DOĞRU'yu; olasılık yoğunluk fonksiyonu için YANLIŞ'ı kullanın"
			}
		]
	},
	{
		name: "LOGNORM.TERS",
		description: "ln(x)'in Ortalama ve Standart_sapma parametreleriyle normal dağıldığı durumlarda x'in kümülatif lognormal dağılım işlevinin tersini verir.",
		arguments: [
			{
				name: "olasılık",
				description: "lognormal dağılımı ile ilgili olasılık, 0 ile 1 arasında (bunlar dahil) bir sayı"
			},
			{
				name: "ortalama",
				description: "ln(x)'in ortalaması"
			},
			{
				name: "standart_sapma",
				description: "ln(x)'in standart sapması, pozitif bir sayı"
			}
		]
	},
	{
		name: "LOGNORMDAĞ",
		description: "ln(x)'in normal olarak Ortalama ve Standart_sapma parametreleriyle dağıldığı durumlarda x'in kümülatif lognormal dağılımını verir.",
		arguments: [
			{
				name: "x",
				description: "işlevin sonucunu veren değer, pozitif bir sayı"
			},
			{
				name: "ortalama",
				description: "ln(x)'in ortalaması"
			},
			{
				name: "standart_sapma",
				description: "ln(x)'in standart sapması, pozitif bir sayı"
			}
		]
	},
	{
		name: "LOGTERS",
		description: "ln(x)'in Ortalama ve Standart_sapma parametreleriyle normal dağıldığı durumlarda x'in kümülatif lognormal dağılım işlevinin tersini verir.",
		arguments: [
			{
				name: "olasılık",
				description: "lognormal dağılımı ile ilgili olasılık, 0 ile 1 arasında (bunlar dahil) bir sayı"
			},
			{
				name: "ortalama",
				description: "ln(x)'in ortalaması"
			},
			{
				name: "standart_sapma",
				description: "ln(x)'in standart sapması, pozitif bir sayı"
			}
		]
	},
	{
		name: "LOT",
		description: "Verilerinize uyması için regresyon çözümlemesi yöntemiyle hesaplanmış olan üstel eğriyi tanımlayan değerler dizisini verir.",
		arguments: [
			{
				name: "bilinen_y'ler",
				description: "y=b*m^x denklemindeki y-değerleri kümesi"
			},
			{
				name: "bilinen_x'ler",
				description: "y=b*m^x denkleminde kullanılan isteğe bağlı x değerleri"
			},
			{
				name: "sabit",
				description: "mantıksal değer: Sabit = DOĞRU ya da atlanmış ise b sabit değeri olağan şekilde hesaplanır; Sabit = YANLIŞ ise b 1'e eşitlenir"
			},
			{
				name: "konum",
				description: "mantıksal değer: ek gerileme istatistiği = DOĞRU; m-katsayıları ve sabit b değeri = YANLIŞ ya da atlanmış"
			}
		]
	},
	{
		name: "M",
		description: "Değer'in başvurduğu metni verir.",
		arguments: [
			{
				name: "değer",
				description: "sınanmak istenen değer: Değer bir metin ya da metin başvurusu ise T Değer verir; Değer metin değilse T çift tırnak (boş metin) verir"
			}
		]
	},
	{
		name: "MAK",
		description: "Bir değerler kümesindeki en büyük değeri verir. Mantıksal değerleri ve metni yoksayar.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "en büyüğünü bulmak istediğiniz en az 1 en fazla 255 sayı, boş hücre, mantıksal değer ya da metin biçiminde sayıdır"
			},
			{
				name: "sayı2",
				description: "en büyüğünü bulmak istediğiniz en az 1 en fazla 255 sayı, boş hücre, mantıksal değer ya da metin biçiminde sayıdır"
			}
		]
	},
	{
		name: "MAKA",
		description: "Bir değerler kümesindeki en büyük değeri verir. Mantıksal değerleri ve metni yoksaymaz.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "değer1",
				description: "en büyüğünü bulmak istediğiniz en az 1 en fazla 255 sayı, boş hücre, mantıksal değer ya da metin biçimindeki sayıdır"
			},
			{
				name: "değer2",
				description: "en büyüğünü bulmak istediğiniz en az 1 en fazla 255 sayı, boş hücre, mantıksal değer ya da metin biçimindeki sayıdır"
			}
		]
	},
	{
		name: "MBUL",
		description: "Özel bir karakter ya da metin dizesinin ilk geçtiği yerin karakter numarasını verir, soldan sağa okuma sırasında (büyük küçük harf duyarlı değil).",
		arguments: [
			{
				name: "bul_metin",
				description: "bulmak istediğiniz metin. ? ve * eşleştirme karakterlerini kullanabilirsiniz; ? ve * karakterlerini bulmak için ~? ve ~* kullanın"
			},
			{
				name: "metin",
				description: "Bul_metin'de kullanılan metin"
			},
			{
				name: "başlangıç_sayısı",
				description: "aramanın başlayacağı Metin_içinde bulunan karakter sayısı (soldan sayıldığında). Atlanırsa, 1 kullanılır"
			}
		]
	},
	{
		name: "METNEÇEVİR",
		description: "Bir değeri belirli bir sayı biçimindeki metne dönüştürür.",
		arguments: [
			{
				name: "değer",
				description: "bir sayı, sayısal değer veren bir formül veya sayısal değer içeren bir hücre başvurusu"
			},
			{
				name: "biçim_metni",
				description: "Hücreleri Biçimlendir iletişim kutusunda bulunan Sayı sekmesindeki Kategori kutusundan alınan metin biçimindeki sayı biçimi (Genel Değil)"
			}
		]
	},
	{
		name: "MİN",
		description: "Bir değerler kümesindeki en küçük değeri verir. Mantıksal değerleri ve metni yoksayar.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "en küçüğünü bulmak istediğiniz en az 1 en fazla 255 sayı, boş hücre, mantıksal değer ya da metin biçiminde sayıdır"
			},
			{
				name: "sayı2",
				description: "en küçüğünü bulmak istediğiniz en az 1 en fazla 255 sayı, boş hücre, mantıksal değer ya da metin biçiminde sayıdır"
			}
		]
	},
	{
		name: "MİNA",
		description: "Bir değerler kümesindeki en küçük değeri verir. Mantıksal değerleri ve metni yoksaymaz.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "değer1",
				description: "en küçüğünü bulmak istediğiniz en az 1 en fazla 255 sayı, boş hücre, mantıksal değer ya da metin biçimindeki sayıdır"
			},
			{
				name: "değer2",
				description: "en küçüğünü bulmak istediğiniz en az 1 en fazla 255 sayı, boş hücre, mantıksal değer ya da metin biçimindeki sayıdır"
			}
		]
	},
	{
		name: "MOD",
		description: "Bir sayının bir bölen tarafından bölünmesinden sonra kalanı verir.",
		arguments: [
			{
				name: "sayı",
				description: "bölme işlemi sonucunda kalanını bulmak istediğiniz sayıdır"
			},
			{
				name: "bölen",
				description: "Sayıyı bölen sayı"
			}
		]
	},
	{
		name: "MUTLAK",
		description: "Bir sayının mutlak değerini verir, işareti olmayan sayı.",
		arguments: [
			{
				name: "sayı",
				description: "mutlak değerini istediğiniz gerçek sayı"
			}
		]
	},
	{
		name: "NBD",
		description: "İndirim oranını, gelecekte yapılacak bir dizi ödemeyi (negatif değerler) ve geliri (pozitif değerler) temel alarak yatırımın bugünkü net değerini verir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "oran",
				description: "bir dönem başına düşen indirim oranıdır"
			},
			{
				name: "değer1",
				description: "zamana eşit olarak yayılmış ve her dönemin sonunda gerçekleşen en az 1 en fazla 254 gelir ve ödeme"
			},
			{
				name: "değer2",
				description: "zamana eşit olarak yayılmış ve her dönemin sonunda gerçekleşen en az 1 en fazla 254 gelir ve ödeme"
			}
		]
	},
	{
		name: "NEGBİNOM.DAĞ",
		description: "Bir başarının negatif binom dağılımını, yani Başarı_sayısı kadar başarıdan önce Başarısızlık_s kadar başarısızlık olması olasılığını Başarı_olasılığı kadar olasılıkla verir.",
		arguments: [
			{
				name: "hata_sayısı",
				description: "başarısızlık sayısı"
			},
			{
				name: "başarı_sayısı",
				description: "başarı eşik sayısı"
			},
			{
				name: "başarı_olasılığı",
				description: "başarı olasılığı; 0 ile 1 arasında bir sayı"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif dağılım fonksiyonu için DOĞRU'yu; olasılık kütle fonksiyonu için YANLIŞ'ı kullanın"
			}
		]
	},
	{
		name: "NEGBİNOMDAĞ",
		description: "Bir başarının negatif binom dağılımını, yani Başarı_sayısı kadar başarıdan önce Başarısızlık_s kadar başarısızlık olması olasılığını Başarı_olasılığı kadar olasılıkla verir.",
		arguments: [
			{
				name: "başarısızlık_s",
				description: "başarısızlık sayısı"
			},
			{
				name: "başarı_sayısı",
				description: "başarı eşik sayısı"
			},
			{
				name: "başarı_olasılığı",
				description: "başarı olasılığı; 0 ile 1 arasında bir sayı"
			}
		]
	},
	{
		name: "NOMİNAL",
		description: "Yıllık nominal faiz oranını döndürür.",
		arguments: [
			{
				name: "etkin_oran",
				description: "etkin faiz oranı"
			},
			{
				name: "dönem_sayısı",
				description: "yıl başına bileşim sayısı"
			}
		]
	},
	{
		name: "NORM.DAĞ",
		description: "Belirtilen ortalama ve standart sapma için normal dağılımı verir.",
		arguments: [
			{
				name: "x",
				description: "dağılımını bulmak istediğiniz değer"
			},
			{
				name: "ortalama",
				description: "dağılımın aritmetik ortalaması"
			},
			{
				name: "standart_sapma",
				description: "dağılımın standart sapması, pozitif bir sayı"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif dağılım işlevi için DOĞRU'yu; olasılık yoğunluk işlevi için YANLIŞ'ı kullanın"
			}
		]
	},
	{
		name: "NORM.S.DAĞ",
		description: "Standart normal dağılımı (ortalaması sıfır, standart sapması bir) verir.",
		arguments: [
			{
				name: "z",
				description: "dağılımını bulmak istediğiniz değer"
			},
			{
				name: "kümülatif",
				description: "fonksiyonun vereceği mantıksal değer: kümülatif dağılım fonksiyonu = DOĞRU; olasılık yoğunluğu fonksiyonu = YANLIŞ"
			}
		]
	},
	{
		name: "NORM.S.TERS",
		description: "Standart normal kümülatif dağılımın (ortalaması sıfır, standart sapması bir) tersini verir.",
		arguments: [
			{
				name: "olasılık",
				description: "normal dağılıma karşılık gelen olasılık, 0 ile 1 arasında (bunlar dahil) bir sayı"
			}
		]
	},
	{
		name: "NORM.TERS",
		description: "Belirtilen ortalama ve standart sapma için normal kümülatif dağılımın tersini verir.",
		arguments: [
			{
				name: "olasılık",
				description: "normal dağılıma karşılık gelen olasılık, 0 ile 1 arasında (bunlar dahil) bir sayı"
			},
			{
				name: "ortalama",
				description: "dağılımın aritmetik ortalaması"
			},
			{
				name: "standart_sapma",
				description: "dağılımın standart sapması, pozitif bir sayı"
			}
		]
	},
	{
		name: "NORMDAĞ",
		description: "Belirtilen ortalama ve standart sapma için normal kümülatif dağılımı verir.",
		arguments: [
			{
				name: "x",
				description: "dağılımını bulmak istediğiniz değer"
			},
			{
				name: "ortalama",
				description: "dağılımın aritmetik ortalaması"
			},
			{
				name: "standart_sapma",
				description: "dağılımın standart sapması, pozitif bir sayı"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif dağılım fonksiyonu için DOĞRU'yu; olasılık yoğunluk fonksiyonu için YANLIŞ'ı kullanın"
			}
		]
	},
	{
		name: "NORMSDAĞ",
		description: "Standart normal kümülatif dağılımı (ortalaması sıfır, standart sapması bir) verir.",
		arguments: [
			{
				name: "z",
				description: "dağılımını bulmak istediğiniz değer"
			}
		]
	},
	{
		name: "NORMSTERS",
		description: "Standart normal kümülatif dağılımın (ortalaması sıfır, standart sapması bir) tersini verir.",
		arguments: [
			{
				name: "olasılık",
				description: "normal dağılıma karşılık gelen olasılık, 0 ile 1 arasında (bunlar dahil) bir sayı"
			}
		]
	},
	{
		name: "NORMTERS",
		description: "Belirtilen ortalama ve standart sapma için normal kümülatif dağılımın tersini verir.",
		arguments: [
			{
				name: "olasılık",
				description: "normal dağılıma karşılık gelen olasılık, 0 ile 1 arasında (bunlar dahil) bir sayı"
			},
			{
				name: "ortalama",
				description: "dağılımın aritmetik ortalaması"
			},
			{
				name: "standart_sapma",
				description: "dağılımın standart sapması, pozitif bir sayı"
			}
		]
	},
	{
		name: "NSAT",
		description: "Bir sayıyı ondalık ya da kesir kısmını kaldırarak bir tamsayıya yuvarlar.",
		arguments: [
			{
				name: "sayı",
				description: "yuvarlamak istediğiniz sayı"
			},
			{
				name: "sayı_rakamlar",
				description: "yuvarlamanın duyarlılığını belirten sayı, atlanırsa 0 (sıfır) kullanılır"
			}
		]
	},
	{
		name: "OBEB",
		description: "En büyük ortak böleni döndürür.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "En az 1 en çok 255 değer"
			},
			{
				name: "sayı2",
				description: "En az 1 en çok 255 değer"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "Sekizlik düzendeki bir sayıyı ikilik düzene dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz sekizlik düzendeki sayı"
			},
			{
				name: "basamak",
				description: "kullanılacak karakter sayısı"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "Sekizlik düzendeki bir sayıyı onluk düzene dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz sekizlik düzendeki sayı"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "Sekizlik düzendeki bir sayıyı onaltılık düzene dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz sekizlik düzendeki sayı"
			},
			{
				name: "basamak",
				description: "kullanılacak karakter sayısı"
			}
		]
	},
	{
		name: "ÖDEMEİND",
		description: "İndirimli bir tahvil için yıllık getiriyi döndürür, örneğin hazine tahvili.",
		arguments: [
			{
				name: "düzenleme",
				description: "teminatın düzenleme tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "vade",
				description: "teminatın vade tarihi, tarih seri numarası cinsinden"
			},
			{
				name: "ücret",
				description: "teminatın 100 TL başına yüz değeri fiyatı"
			},
			{
				name: "teminat",
				description: "teminatın 100 TL yüz değeri başına kefaret değeri"
			},
			{
				name: "temel",
				description: "kullanılacak gün sayısı türü"
			}
		]
	},
	{
		name: "OKEK",
		description: "En küçük ortak çarpanı döndürür.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "en küçük ortak çarpanını istediğiniz en az 1 en çok 255 değer"
			},
			{
				name: "sayı2",
				description: "en küçük ortak çarpanını istediğiniz en az 1 en çok 255 değer"
			}
		]
	},
	{
		name: "OLASILIK",
		description: "Bir aralıktaki değerlerin iki sınır arasında ya da alt sınıra eşit olması olasılığını verir.",
		arguments: [
			{
				name: "x_aralığı",
				description: "olasılık değerleriyle ilgili sayısal x değerleri aralığı"
			},
			{
				name: "olasılık_aralığı",
				description: "X_aralığındaki değerlerle ilgili olasılıklar kümesi, 0 ile 1 arasındaki (0 hariç) değerler"
			},
			{
				name: "alt_sınır",
				description: "olasılık işleminin yapılacağı değerin alt sınırı"
			},
			{
				name: "üst_sınır",
				description: "değerin isteğe bağlı en üst sınırı, OLASILIK X_aralığı değerlerinin Alt_sınır'a eşit olma olasılığını verir"
			}
		]
	},
	{
		name: "ONDALIK",
		description: "Verilen temeldeki bir sayının metin gösterimini ondalık bir sayıya dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz sayı"
			},
			{
				name: "sayıtabanı",
				description: "dönüştürdüğünüz sayının temel Sayı Tabanı"
			}
		]
	},
	{
		name: "ORTALAMA",
		description: "Bağımsız değişkenlerinin (aritmetik) ortalamasını verir, bunlar sayı ya da sayılar içeren ad, dizi veya başvurular olabilir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "ortalamasını bulmak istediğiniz en az 1 en fazla 255 bağımsız sayısal değişkendir"
			},
			{
				name: "sayı2",
				description: "ortalamasını bulmak istediğiniz en az 1 en fazla 255 bağımsız sayısal değişkendir"
			}
		]
	},
	{
		name: "ORTALAMAA",
		description: "Bağımsız değişkenlerinin aritmetik ortalamasını verir, metni ve bağımsız değişkenlerdeki YANLIŞ değerini 0; DOĞRU değerini 1 olarak değerlendirir. Bağımsız değişkenler sayı, ad, dizi ya da başvuru olabilir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "değer1",
				description: "Aritmetik ortalamasını bulmak istediğiniz en az 1 en fazla 255 bağımsız değişkendir"
			},
			{
				name: "değer2",
				description: "Aritmetik ortalamasını bulmak istediğiniz en az 1 en fazla 255 bağımsız değişkendir"
			}
		]
	},
	{
		name: "ORTANCA",
		description: "Verilen sayılar kümesinin ortancasını ya da bu kümenin ortasındaki sayıyı verir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "ortancasını aradığınız en az 1 en fazla 255 sayı, ad, dizi ya da sayı içeren başvurudur"
			},
			{
				name: "sayı2",
				description: "ortancasını aradığınız en az 1 en fazla 255 sayı, ad, dizi ya da sayı içeren başvurudur"
			}
		]
	},
	{
		name: "ORTSAP",
		description: "Veri noktalarının mutlak sapmalarının aritmetik ortalamasını bu noktaların ortalaması aracılığıyla verir. Bağımsız değişkenler sayı, ad, dizi veya sayı içeren başvurular olabilir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "Mutlak sapmalarının ortalamasını elde etmek istediğiniz en az 1 en fazla 255 bağımsız değişkendir"
			},
			{
				name: "sayı2",
				description: "Mutlak sapmalarının ortalamasını elde etmek istediğiniz en az 1 en fazla 255 bağımsız değişkendir"
			}
		]
	},
	{
		name: "ÖZDEŞ",
		description: "İki metin dizesini karşılaştırır ve tamamen aynıysalar DOĞRU, başka durumlarda YANLIŞ verir (büyük küçük harf duyarlı).",
		arguments: [
			{
				name: "metin1",
				description: "ilk metin karakter dizesi"
			},
			{
				name: "metin2",
				description: "ikinci metin karakter dizesi"
			}
		]
	},
	{
		name: "ÖZELVEYA",
		description: "Tüm bağımsız değişkenlerden mantıksal bir 'Özel Veya' değişkenini döndürür.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "mantıksal1",
				description: "DOĞRU veya YANLIŞ olduğunu sınamak istediğiniz en az 1 en fazla 254 koşuldur ve her biri mantıksal değer, dizi ya da başvuru olabilir"
			},
			{
				name: "mantıksal2",
				description: "DOĞRU veya YANLIŞ olduğunu sınamak istediğiniz en az 1 en fazla 254 koşuldur ve her biri mantıksal değer, dizi ya da başvuru olabilir"
			}
		]
	},
	{
		name: "ÖZETVERİAL",
		description: "PivotTable'da depolanmış verileri verir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "veri_alanı",
				description: "verinin alınacağı veri alanının adıdır"
			},
			{
				name: "pivot_table",
				description: "almak istediğiniz verileri içeren PivotTable'daki bir hücreye veya hücreler aralığına yapılan başvurudur"
			},
			{
				name: "alan",
				description: "başvuruda bulunulan alan"
			},
			{
				name: "öğe",
				description: "başvuruda bulunulan alan öğesi"
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
		name: "PARÇAAL",
		description: "Belirttiğiniz konumdan başlamak üzere metinden belirli sayıda karakter verir.",
		arguments: [
			{
				name: "metin",
				description: "çıkartmak istediğiniz karakterlerin bulunduğu metin dizesi"
			},
			{
				name: "başlangıç_sayısı",
				description: "çıkartmak istediğiniz ilk karakterin konumu. Metindeki ilk karakter 1'dir"
			},
			{
				name: "sayı_karakterler",
				description: "Metinden kaç tane karakterin geleceğini belirtir"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Pearson çarpım moment korelasyon katsayısı olan r'yi verir.",
		arguments: [
			{
				name: "dizi1",
				description: "bağımsız değerler kümesi"
			},
			{
				name: "dizi2",
				description: "bağımlı değerler kümesi"
			}
		]
	},
	{
		name: "PERMÜTASYON",
		description: "Tüm nesnelerden seçilebilecek olan verilen sayıda nesne için permütasyon sayısını verir.",
		arguments: [
			{
				name: "sayı",
				description: "toplam nesne sayısı"
			},
			{
				name: "sayı_seçilen",
				description: "her permütasyondaki nesne sayısı"
			}
		]
	},
	{
		name: "PERMÜTASYONA",
		description: "Tüm nesnelerden seçilebilecek olan verilen sayıda (yinelemelerle) nesne için permütasyon sayısını verir.",
		arguments: [
			{
				name: "sayı",
				description: "nesnelerin toplam sayısı"
			},
			{
				name: "sayı_seçilen",
				description: "her permütasyondaki nesne sayısı"
			}
		]
	},
	{
		name: "PHI",
		description: "Standart normal dağılımın yoğunluk fonksiyonunun değerini döndürür.",
		arguments: [
			{
				name: "x",
				description: "standart normal dağılım yoğunluğunu bulmak istediğiniz sayı"
			}
		]
	},
	{
		name: "Pİ",
		description: "Pi değerini verir, 15 rakama kadar yuvarlanmış hali 3,14159265358979'dur.",
		arguments: [
		]
	},
	{
		name: "POISSON",
		description: "Poisson dağılımını verir.",
		arguments: [
			{
				name: "x",
				description: "olay sayısı"
			},
			{
				name: "ortalama",
				description: "beklenen sayısal değer, pozitif bir sayı"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif Poisson olasılığı için DOĞRU'yu; Poisson olasılık kütle fonksiyonu için YANLIŞ'ı kullanın"
			}
		]
	},
	{
		name: "POISSON.DAĞ",
		description: "Poisson dağılımını verir.",
		arguments: [
			{
				name: "x",
				description: "olay sayısı"
			},
			{
				name: "ortalama",
				description: "beklenen sayısal değer, pozitif bir sayı"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif Poisson olasılığı için DOĞRU'yu; Poisson olasılık kütle işlevi için YANLIŞ'ı kullanın"
			}
		]
	},
	{
		name: "PSÜRE",
		description: "Yatırım tarafından belirtilen bir değere ulaşmak için gereken dönem sayısını döndürür.",
		arguments: [
			{
				name: "oran",
				description: "dönem başına düşen faiz oranı."
			},
			{
				name: "bd",
				description: "yatırımın bugünkü değeri"
			},
			{
				name: "gd",
				description: "yatırımın gelecekteki istenen değeri"
			}
		]
	},
	{
		name: "RADYAN",
		description: "Dereceyi radyana dönüştürür.",
		arguments: [
			{
				name: "açı",
				description: "dönüştürmek istediğiniz derece cinsinden açı"
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
		description: "Bir sayı listesindeki bir sayının derecesini verir: listedeki diğer değerlere göreli olarak büyüklüğü.",
		arguments: [
			{
				name: "sayı",
				description: "derecesini bulmak istediğiniz sayı"
			},
			{
				name: "başv",
				description: "bir sayı listesi başvurusu veya dizisi. Sayısal olmayan değerler yoksayılır"
			},
			{
				name: "sıra",
				description: "sayı: azalan sıralı listedeki derece = 0 ya da atlanmış; artan sıralı listedeki derece = sıfır olmayan herhangi bir değer"
			}
		]
	},
	{
		name: "RANK.EŞİT",
		description: "Bir sayı listesindeki bir sayının derecesini verir: listedeki diğer değerlere göreli olarak büyüklüğü; birden fazla değer aynı dereceye sahipse, değer kümesindeki en yüksek derece döndürülür.",
		arguments: [
			{
				name: "sayı",
				description: "derecesini bulmak istediğiniz sayı"
			},
			{
				name: "başv",
				description: "bir sayı listesi başvurusu veya dizisi. Sayısal olmayan değerler yoksayılır"
			},
			{
				name: "sıra",
				description: "sayı: azalan sıralı listedeki derece = 0 ya da atlanmış; artan sıralı listedeki derece = sıfır olmayan herhangi bir değer"
			}
		]
	},
	{
		name: "RANK.ORT",
		description: "Bir sayı listesindeki bir sayının derecesini verir: listedeki diğer değerlere göreli olarak büyüklüğü; birden fazla değer aynı dereceye sahipse, ortalama derece döndürülür.",
		arguments: [
			{
				name: "sayı",
				description: "derecesini bulmak istediğiniz sayı"
			},
			{
				name: "başv",
				description: "bir sayı listesi başvurusu veya dizisi. Sayısal olmayan değerler yoksayılır"
			},
			{
				name: "sıra",
				description: "sayı: azalan sıralı listedeki derece = 0 ya da atlanmış; artan sıralı listedeki derece = sıfır olmayan herhangi bir değer"
			}
		]
	},
	{
		name: "RASTGELEARADA",
		description: "Belirttiğiniz sayılar arasında rastgele bir sayı döndürür.",
		arguments: [
			{
				name: "alt",
				description: "RASTGELEARADA işlevinin döndüreceği en küçük tamsayı"
			},
			{
				name: "üst",
				description: "RASTGELEARADA işlevinin döndüreceği en büyük tamsayı"
			}
		]
	},
	{
		name: "RKARE",
		description: "Verilen veri noktaları boyunca Pearson çarpım moment korelasyon katsayısının karesini verir.",
		arguments: [
			{
				name: "bilinen_y'ler",
				description: "dizi ya da veri noktaları aralığıdır ve sayı, ad, dizi, ya da sayı içeren başvurular olabilir"
			},
			{
				name: "bilinen_x'ler",
				description: "dizi ya da veri noktaları aralığıdır ve sayı, ad, dizi,0 ya da sayı içeren başvurular olabilir"
			}
		]
	},
	{
		name: "ROMEN",
		description: "Arap rakamlarını metin biçimiyle romen rakamlarına dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz sayı"
			},
			{
				name: "form",
				description: "hangi tür romen sayısı istediğinizi belirten sayı."
			}
		]
	},
	{
		name: "S",
		description: "Bir sayıya dönüştürülmüş değeri verir. Sayılar sayılara, Tarihler seri numaralarına, DOĞRU 1'e, bunların dışındaki şeyler de 0 (sıfır)'a dönüştürülür.",
		arguments: [
			{
				name: "değer",
				description: "dönüştürmek istediğiniz değer"
			}
		]
	},
	{
		name: "S_SAYI_ÜRET",
		description: "0 ya da 0'dan büyük ve 1'den küçük bir sayıyı eşit dağılımla rasgele verir (yeniden hesaplama sonucunda değişir).",
		arguments: [
		]
	},
	{
		name: "SAAT",
		description: "Saati verir, bir seri numarasına karşılık gelen 0 (12:00)'dan 23 (11:00)'e kadar bir tamsayı.",
		arguments: [
			{
				name: "seri_no",
				description: "Spreadsheet tarafından kullanılan tarih-saat kodundaki sayı ya da zaman biçiminde metin, örneğin 16:48:00 ya da 4:48:00"
			}
		]
	},
	{
		name: "SAĞDAN",
		description: "Bir metin dizesinin son (en sağdaki) belirtilen sayıdaki karakter ya da karakterlerini verir.",
		arguments: [
			{
				name: "metin",
				description: "çıkarmak istediğiniz karakterleri içeren metin dizesi"
			},
			{
				name: "sayı_karakterler",
				description: "kaç tane karakter çıkarmak istediğinizi belirtir, atlanırsa 1"
			}
		]
	},
	{
		name: "SANAL",
		description: "Bir karmaşık sayının sanal katsayısını döndürür.",
		arguments: [
			{
				name: "karmsayı",
				description: "sanal katsayısı istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANBAĞ_DEĞİŞKEN",
		description: "Bir karmaşık sayının radyan cinsinden argümanını (q) döndürür.",
		arguments: [
			{
				name: "karmsayı",
				description: "argümanı istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANBÖL",
		description: "İki karmaşık sayının bölümünü döndürür.",
		arguments: [
			{
				name: "karmsayı1",
				description: "karmaşık pay veya bölünen"
			},
			{
				name: "karmsayı2",
				description: "karmaşık payda veya bölen"
			}
		]
	},
	{
		name: "SANÇARP",
		description: "En az 1 en çok 255 karmaşık sayının çarpımını döndürür.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "isayı1",
				description: "Isayı1, Isayı2,... çarpımı yapılacak en az 1 en çok 255 sayıdır."
			},
			{
				name: "isayı2",
				description: "Isayı1, Isayı2,... çarpımı yapılacak en az 1 en çok 255 sayıdır."
			}
		]
	},
	{
		name: "SANÇIKAR",
		description: "Karmaşık sayıların toplamını döndürür.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "isayı1",
				description: "eklenecek en az 1 en çok 255 karmaşık sayı"
			},
			{
				name: "isayı2",
				description: "eklenecek en az 1 en çok 255 karmaşık sayı"
			}
		]
	},
	{
		name: "SANCOS",
		description: "Bir karmaşık sayının kosinüs değerini döndürür.",
		arguments: [
			{
				name: "karmsayı",
				description: "kosinüsü istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANCOSH",
		description: "Bir karmaşık sayının hiperbolik kosinüs değerini verir.",
		arguments: [
			{
				name: "karmsayı",
				description: "hiperbolik kosinüsü istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANCOT",
		description: "Bir karmaşık sayının kotanjant değerini verir.",
		arguments: [
			{
				name: "karmsayı",
				description: "kotanjantı istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANCSC",
		description: "Bir karmaşık sayının kosekant değerini verir.",
		arguments: [
			{
				name: "karmsayı",
				description: "kosekantı istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANCSCH",
		description: "Bir karmaşık sayının hiperbolik kosekant değerini verir.",
		arguments: [
			{
				name: "karmsayı",
				description: "hiperbolik kosekantı istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANEŞLENEK",
		description: "Bir karmaşık sayının eşleneğini döndürür.",
		arguments: [
			{
				name: "karmsayı",
				description: "eşleneği istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANGERÇEK",
		description: "Bir karmaşık sayının gerçel katsayısını döndürür.",
		arguments: [
			{
				name: "karmsayı",
				description: "gerçel katsayısı istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANİYE",
		description: "Saniyeyi seri numarasına karşılık gelen 0 ile 59 arasında bir tamsayı cinsinden verir.",
		arguments: [
			{
				name: "seri_no",
				description: "Spreadsheet tarafından kullanılan tarih-saat kodundaki sayı ya da saat biçimindeki metin, örneğin 16:48:23 ya da 4:48:47"
			}
		]
	},
	{
		name: "SANKAREKÖK",
		description: "Bir karmaşık sayının kare kökünü döndürür.",
		arguments: [
			{
				name: "karmsayı",
				description: "kare kökü istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANKUVVET",
		description: "Bir karmaşık sayının tamsayı bir kuvvetini döndürür.",
		arguments: [
			{
				name: "karmsayı",
				description: "kuvveti alınacak karmaşık sayı"
			},
			{
				name: "sayı",
				description: "karmaşık sayının yükseltileceği kuvvet"
			}
		]
	},
	{
		name: "SANLN",
		description: "Bir karmaşık sayının doğal logaritmasını döndürür.",
		arguments: [
			{
				name: "karmsayı",
				description: "doğal logaritması alınacak karmaşık sayı"
			}
		]
	},
	{
		name: "SANLOG10",
		description: "Bir karmaşık sayının 10 tabanında logaritmasını döndürür.",
		arguments: [
			{
				name: "karmsayı",
				description: "ortak logaritması alınacak karmaşık sayı"
			}
		]
	},
	{
		name: "SANLOG2",
		description: "Bir karmaşık sayının 2 tabanında logaritmasını döndürür.",
		arguments: [
			{
				name: "karmsayı",
				description: "2 tabanında logaritması alınacak karmaşık sayı"
			}
		]
	},
	{
		name: "SANMUTLAK",
		description: "Bir karmaşık sayının mutlak değerini (modulus) döndürür.",
		arguments: [
			{
				name: "karmsayı",
				description: "mutlak değeri istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANSEC",
		description: "Bir karmaşık sayının sekant değerini verir.",
		arguments: [
			{
				name: "karmsayı",
				description: "sekantı istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANSECH",
		description: "Bir karmaşık sayının hiperbolik sekant değerini verir.",
		arguments: [
			{
				name: "karmsayı",
				description: "hiperbolik sekantı istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANSIN",
		description: "Bir karmaşık sayının sinüs değerini döndürür.",
		arguments: [
			{
				name: "karmsayı",
				description: "sinüsü istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANSINH",
		description: "Bir karmaşık sayının hiperbolik sinüs değerini verir.",
		arguments: [
			{
				name: "karmsayı",
				description: "hiperbolik sinüsü istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANTAN",
		description: "Bir karmaşık sayının tanjant değerini verir.",
		arguments: [
			{
				name: "karmsayı",
				description: "tanjantı istenen karmaşık sayı"
			}
		]
	},
	{
		name: "SANTOPLA",
		description: "İki karmaşık sayının farkını döndürür.",
		arguments: [
			{
				name: "karmsayı1",
				description: "karmsayı2 değerinin çıkarılacağı karmaşık sayı"
			},
			{
				name: "karmsayı2",
				description: "karmsayı1 değerinden çıkarılacak karmaşık sayı"
			}
		]
	},
	{
		name: "SANÜS",
		description: "Bir karmaşık sayının üssel değerini döndürür.",
		arguments: [
			{
				name: "karmsayı",
				description: "üssel değeri bulunacak karmaşık sayı"
			}
		]
	},
	{
		name: "SAPKARE",
		description: "Veri noktalarının kendi örneklerinin ortalamasından sapmaların kareleri toplamını verir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "SAPKARE'sini hesaplamak istediğiniz En az 1 en fazla 255 bağımsız değişken, ya da bir dizi veya dizi başvurusudur"
			},
			{
				name: "sayı2",
				description: "SAPKARE'sini hesaplamak istediğiniz En az 1 en fazla 255 bağımsız değişken, ya da bir dizi veya dizi başvurusudur"
			}
		]
	},
	{
		name: "SATIR",
		description: "Bir başvurunun satır numarasını verir.",
		arguments: [
			{
				name: "başvuru",
				description: "satır numarasını öğrenmek istediğiniz hücre veya tek bir hücreler aralığı; atlanırsa, SATIR işlevini içeren hücreyi verir"
			}
		]
	},
	{
		name: "SATIRSAY",
		description: "Bir başvuru ya da dizideki satır sayısını verir.",
		arguments: [
			{
				name: "dizi",
				description: "satır sayısını öğrenmek istediğiniz hücreler için bir başvuru, dizi veya dizi formülü"
			}
		]
	},
	{
		name: "SAYFA",
		description: "Başvurulan sayfanın sayfa numarasını döndürür.",
		arguments: [
			{
				name: "değer",
				description: "sayfa numarasını bulmak istediğiniz bir sayfanın veya başvurunun adı.  Atlanırsa, fonksiyonu içeren sayfa numarası döndürülür"
			}
		]
	},
	{
		name: "SAYFALAR",
		description: "Bir başvurudaki sayfa sayısını döndürür.",
		arguments: [
			{
				name: "başvuru",
				description: "içerdiği sayfa sayısını bilmek istediğiniz başvuru.  Atlanırsa, fonksiyonu içeren çalışma kitabındaki sayfa sayısı döndürülür"
			}
		]
	},
	{
		name: "SAYIDEĞERİ",
		description: "Metni yerel bağımsız durumdaki sayıya dönüştürür.",
		arguments: [
			{
				name: "metin",
				description: "dönüştürmek istediğiniz sayıyı temsil eden dize"
			},
			{
				name: "ondalık_ayırıcı",
				description: " dizedeki ondalık ayırıcı olarak kullanılan karakter"
			},
			{
				name: "grup_ayırıcı",
				description: "dizedeki grup ayırıcı olarak kullanılan karakter"
			}
		]
	},
	{
		name: "SAYIDÜZENLE",
		description: "Bir sayıyı belirtilen sayıda ondalıklara yuvarlar ve sonucu virgüllü ya da virgülsüz metin olarak verir.",
		arguments: [
			{
				name: "sayı",
				description: "yuvarlayıp metin türüne döndürmek istediğiniz sayı"
			},
			{
				name: "onluklar",
				description: "ondalık virgülün sağında kullanılan rakam sayısı. Atlanırsa, Ondalıklar = 2"
			},
			{
				name: "virgül_yok",
				description: "bir mantıksal değer: gelen metinde virgülleri görüntüleme = DOĞRU; gelen metinde virgülleri görüntüle = YANLIŞ ya da atlanmış"
			}
		]
	},
	{
		name: "SAYIYAÇEVİR",
		description: "Bir sayıyı gösteren bir metin dizesini bir sayıya dönüştürür.",
		arguments: [
			{
				name: "metin",
				description: "dönüştürmek istediğiniz metni içeren hücre başvurusu veya tırnak işaretiyle kapalı metin"
			}
		]
	},
	{
		name: "SEC",
		description: "Bir açının sekant değerini verir.",
		arguments: [
			{
				name: "sayı",
				description: "sekantı istenen radyan cinsinden açı"
			}
		]
	},
	{
		name: "SECH",
		description: "Bir açının hiperbolik sekant değerini verir.",
		arguments: [
			{
				name: "sayı",
				description: "hiperbolik sekantı istenen radyan cinsinden açı"
			}
		]
	},
	{
		name: "SERİAY",
		description: "Belirtilen sayıda ay önce veya sonraki ayın son gününü belirten seri numarası döndürür.",
		arguments: [
			{
				name: "başlangıç_tarihi",
				description: "başlangıç tarihini gösteren tarih seri numarası"
			},
			{
				name: "ay_sayısı",
				description: "başlangıç tarihinden önceki veya sonraki ay sayısı"
			}
		]
	},
	{
		name: "SERİTARİH",
		description: "Başlangıç tarihinden önceki veya sonraki ay sayısını belirten tarihin seri numarasını döndürür.",
		arguments: [
			{
				name: "başlangıç_tarihi",
				description: "başlangıç tarihini gösteren tarih seri numarası"
			},
			{
				name: "ay_sayısı",
				description: "başlangıç tarihinden önceki veya sonraki ay sayısı"
			}
		]
	},
	{
		name: "SERİTOPLA",
		description: "Formüle dayalı olan kuvvet serisinin toplamını döndürür.",
		arguments: [
			{
				name: "x",
				description: "kuvvet serisi için giriş değeri"
			},
			{
				name: "n",
				description: "x'in yükseltileceği ilk kuvvet"
			},
			{
				name: "m",
				description: "serideki her öğe için n'nin artırma oranı"
			},
			{
				name: "katsayılar",
				description: "x'in ardıl kuvvetlerinin çarpıldığı katsayılar kümesi"
			}
		]
	},
	{
		name: "SIKLIK",
		description: "Bir değerler aralığındaki verilerin hangi sıklıkta yer aldığını hesaplar ve Ara_dizisi'nden bir fazla elemana sahip olan bir düşey sayılar dizisi verir.",
		arguments: [
			{
				name: "veri_dizisi",
				description: "frekansını saymak istediğiniz bir değerler kümesi dizisi veya başvurusu (boşluk ve metin yoksayılır)"
			},
			{
				name: "ara_dizi",
				description: "veri_dizisi'ndeki değerleri gruplayacağınız bir aralıklar dizisi veya başvurusu"
			}
		]
	},
	{
		name: "ŞİMDİ",
		description: "Güncel tarihi ve saati, tarih ve saat biçiminde verir.",
		arguments: [
		]
	},
	{
		name: "SİN",
		description: "Bir açının sinüsünü verir.",
		arguments: [
			{
				name: "sayı",
				description: "sinüsünü almak istediğiniz radyan cinsinden açı. * PI()/180 derece = radyan"
			}
		]
	},
	{
		name: "SİNH",
		description: "Bir sayının hiperbolik sinüsünü verir.",
		arguments: [
			{
				name: "sayı",
				description: "herhangi bir gerçek sayı"
			}
		]
	},
	{
		name: "SOLDAN",
		description: "Bir metin dizesinin ilk (en solundaki) belirtilen sayıdaki karakter ya da karakterlerini verir.",
		arguments: [
			{
				name: "metin",
				description: "çıkartmak istediğiniz karakterleri içeren metin"
			},
			{
				name: "sayı_karakterler",
				description: "SOLDAN fonksiyonunun çıkarmasını istediğiniz karakter sayısını belirtir; atlanırsa 1"
			}
		]
	},
	{
		name: "STANDARTLAŞTIRMA",
		description: "Bir ortalama ve standart sapma tarafından temsil edilen bir dağılımdan normalleştirilen değeri verir.",
		arguments: [
			{
				name: "x",
				description: "normalize etmek istediğiniz değer"
			},
			{
				name: "ortalama",
				description: "dağılımın aritmetik ortalaması"
			},
			{
				name: "standart_sapma",
				description: "dağılımın standart sapması, pozitif bir sayı"
			}
		]
	},
	{
		name: "STDSAPMA",
		description: "Bir örneğe dayanarak standart sapmayı tahmin eder (örnekteki mantıksal değerleri ve metni yoksayar).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "Popülasyondan alınmış bir örneğe karşılık gelen en az 1 en fazla 255 sayıdır; her biri sayı ya da sayı içeren başvuru olabilir"
			},
			{
				name: "sayı2",
				description: "Popülasyondan alınmış bir örneğe karşılık gelen en az 1 en fazla 255 sayıdır; her biri sayı ya da sayı içeren başvuru olabilir"
			}
		]
	},
	{
		name: "STDSAPMA.P",
		description: "Bağımsız değişkenler olarak verilen tüm popülasyonu temel alarak standart sapmayı hesaplar (mantıksal değerleri ve metni yoksayar).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "Popülasyona karşılık gelen en az 1 en fazla 255 sayıdır ve her biri sayı ya da sayı içeren başvuru olabilir"
			},
			{
				name: "sayı2",
				description: "Popülasyona karşılık gelen en az 1 en fazla 255 sayıdır ve her biri sayı ya da sayı içeren başvuru olabilir"
			}
		]
	},
	{
		name: "STDSAPMA.S",
		description: "Bir örneğe dayanarak standart sapmayı tahmin eder (örnekteki mantıksal değerleri ve metni yoksayar).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "popülasyondan alınmış bir örneğe karşılık gelen en az 1 en fazla 255 sayıdır; her biri sayı ya da sayı içeren başvuru olabilir"
			},
			{
				name: "sayı2",
				description: "popülasyondan alınmış bir örneğe karşılık gelen en az 1 en fazla 255 sayıdır; her biri sayı ya da sayı içeren başvuru olabilir"
			}
		]
	},
	{
		name: "STDSAPMAA",
		description: "Mantıksal değerler ve metin içeren bir örneğin standart sapmasını tahmin eder. Metin ve YANLIŞ mantıksal değer 0; DOĞRU mantıksal değer ise 1 değerini alır.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "değer1",
				description: "bir popülasyonun bir örneğine karşılık gelen en az 1 en fazla 255 değerdir ve değer, ad ya da değer başvurusu olabilir"
			},
			{
				name: "değer2",
				description: "bir popülasyonun bir örneğine karşılık gelen en az 1 en fazla 255 değerdir ve değer, ad ya da değer başvurusu olabilir"
			}
		]
	},
	{
		name: "STDSAPMAS",
		description: "Bağımsız değişkenler olarak verilen tüm popülasyonu temel alarak standart sapmayı hesaplar (mantıksal değerleri ve metni yoksayar).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "popülasyona karşılık gelen en az 1 en fazla 255 sayıdır ve her biri sayı ya da sayı içeren başvuru olabilir"
			},
			{
				name: "sayı2",
				description: "popülasyona karşılık gelen en az 1 en fazla 255 sayıdır ve her biri sayı ya da sayı içeren başvuru olabilir"
			}
		]
	},
	{
		name: "STDSAPMASA",
		description: "Mantıksal değerler ve metin içeren tüm bir popülasyon için standart sapmayı hesaplar. Metin ve YANLIŞ mantıksal değer 0; DOĞRU mantıksal değer ise 1 değerini alır.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "değer1",
				description: "bir popülasyona karşılık gelen en az 1 en fazla 255 değerdir ve her biri değer, ad, dizi ya da değer içeren başvuru olabilir"
			},
			{
				name: "değer2",
				description: "bir popülasyona karşılık gelen en az 1 en fazla 255 değerdir ve her biri değer, ad, dizi ya da değer içeren başvuru olabilir"
			}
		]
	},
	{
		name: "STHYX",
		description: "Bir regresyondaki her x değeri için tahmin edilen y değerinin standart hatasını verir.",
		arguments: [
			{
				name: "bilinen_y'ler",
				description: "bağımlı veri noktaları aralığı veya dizisidir ve ve sayı, ad, dizi, ya da sayı içeren başvuru olabilir"
			},
			{
				name: "bilinen_x'ler",
				description: "bağımsız veri noktaları aralığı veya dizisidir ve sayı, ad, dizi, ya da sayı içeren başvuru olabilir"
			}
		]
	},
	{
		name: "SÜTUN",
		description: "Başvurunun sütun sayısını verir.",
		arguments: [
			{
				name: "başvuru",
				description: "sütun sayısını öğrenmek istediğiniz hücre veya bitişik hücreler aralığı. Atlanırsa, SÜTUN işlevini içeren hücre kullanılır"
			}
		]
	},
	{
		name: "SÜTUNSAY",
		description: "Bir dizideki ya da başvurudaki sütun sayısını verir.",
		arguments: [
			{
				name: "dizi",
				description: "sütun sayısını öğrenmek için kullanılan dizi, dizi formülü ya da bir hücre aralığı başvurusu"
			}
		]
	},
	{
		name: "T.DAĞ",
		description: "Sol kuyruklu t-dağılımını verir.",
		arguments: [
			{
				name: "x",
				description: "dağılımın değerini bulmak istediğiniz sayısal değer"
			},
			{
				name: "serb_derecesi",
				description: "dağılımı karakterize eden serbestlik derecesinin sayısını gösteren tam sayı"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif dağılım fonksiyonu için DOĞRU'yu; olasılık yoğunluk fonksiyonu için YANLIŞ'ı kullanın"
			}
		]
	},
	{
		name: "T.DAĞ.2K",
		description: "İki kuyruklu t-dağılımını verir.",
		arguments: [
			{
				name: "x",
				description: "dağılımın değerini bulmak istediğiniz sayısal değer"
			},
			{
				name: "serb_derecesi",
				description: "dağılımı karakterize eden serbestlik derecesinin sayısını gösteren tam sayı"
			}
		]
	},
	{
		name: "T.DAĞ.SAĞK",
		description: "Sağ kuyruklu t-dağılımını verir.",
		arguments: [
			{
				name: "x",
				description: "dağılımın değerini bulmak istediğiniz sayısal değer"
			},
			{
				name: "serb_derecesi",
				description: "dağılımı karakterize eden serbestlik derecesinin sayısını gösteren tam sayı"
			}
		]
	},
	{
		name: "T.TERS",
		description: "T-dağılımının sol kuyruklu tersini verir.",
		arguments: [
			{
				name: "olasılık",
				description: "iki kuyruklu t-dağılımının olasılığı, 0 ile 1 arasındaki (bunlar dahil) bir sayı"
			},
			{
				name: "serb_derecesi",
				description: "dağılımın türünü belirleyen serbestlik derecesinin sayısını gösteren pozitif tamsayı"
			}
		]
	},
	{
		name: "T.TERS.2K",
		description: "T-dağılımının iki kuyruklu tersini verir.",
		arguments: [
			{
				name: "olasılık",
				description: "iki kuyruklu t-dağılımının olasılığı, 0 ile 1 arasındaki (bunlar dahil) bir sayı"
			},
			{
				name: "serb_derecesi",
				description: "dağılımın türünü belirleyen serbestlik derecesinin sayısını gösteren pozitif tamsayı"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Bir t-Test için olasılığı verir.",
		arguments: [
			{
				name: "dizi1",
				description: "ilk veri kümesi"
			},
			{
				name: "dizi2",
				description: "ikinci veri kümesi"
			},
			{
				name: "yazı_say",
				description: "verilecek dağılım kuyruklarının sayısını belirtir: tek kuyruklu dağılım = 1; çift kuyruklu dağılım = 2"
			},
			{
				name: "tür",
				description: "uygulanacak t-test türü: eşli = 1, iki-örnek eşit varyans (homoskedastik) = 2, iki örnek eşit olmayan varyans = 3"
			}
		]
	},
	{
		name: "TABAN",
		description: "Bir sayıyı verilen sayı tabanı (temel) ile bir metin gösterimine dönüştürür.",
		arguments: [
			{
				name: "sayı",
				description: "dönüştürmek istediğiniz sayı"
			},
			{
				name: "sayıtabanı",
				description: "sayıyı dönüştürmek istediğiniz temel Sayı Tabanı"
			},
			{
				name: "min_uzunluk",
				description: "döndürülen dizenin minimum uzunluğu. Atlanırsa baştaki sıfırlar eklenmez"
			}
		]
	},
	{
		name: "TABANAYUVARLA",
		description: "Bir sayıyı, anlamlı en yakın katına, aşağı doğru yuvarlar.",
		arguments: [
			{
				name: "sayı",
				description: "yuvarlamak istediğiniz sayısal değer"
			},
			{
				name: "anlam",
				description: "yuvarlamak istediğiniz kat. Sayı ve Anlamın her ikisi de negatif ya da her ikisi de pozitif olmalıdır"
			}
		]
	},
	{
		name: "TABANAYUVARLA.DUYARLI",
		description: "Bir sayıyı, en yakın tamsayıya veya anlamlı en yakın katına, aşağı doğru yuvarlar.",
		arguments: [
			{
				name: "sayı",
				description: "yuvarlamak istediğiniz sayısal değer"
			},
			{
				name: "anlam",
				description: "yuvarlamak istediğiniz kat. "
			}
		]
	},
	{
		name: "TABANAYUVARLA.MATEMATİK",
		description: "Bir sayıyı, aşağı doğru en yakın tamsayı veya anlamlı sayı katına yuvarlar.",
		arguments: [
			{
				name: "sayı",
				description: "yuvarlamak istediğiniz değer"
			},
			{
				name: "anlam",
				description: "yuvarlamak istediğiniz kat"
			},
			{
				name: "mod",
				description: "verilen ve sıfır olmayan bu işlev sıfıra doğru yuvarlanır"
			}
		]
	},
	{
		name: "TAHMİN",
		description: "Varolan değerleri kullanarak bir gelecek değeri doğrusal bir eğilim boyunca hesaplar ya da tahmin eder.",
		arguments: [
			{
				name: "x",
				description: "bir değer tahmin edilecek veri noktasıdır ve sayısal bir değer olmalıdır"
			},
			{
				name: "bilinen_y'ler",
				description: "bağımlı sayısal veri dizisi ya da aralığı"
			},
			{
				name: "bilinen_x'ler",
				description: "bağımsız sayısal veri dizisi ya da aralığı. Bilinen_x'lerin varyansı sıfır olmalıdır"
			}
		]
	},
	{
		name: "TAKSİT_SAYISI",
		description: "Dönemsel sabit ödemeli ve sabit faizli bir yatırımın dönem sayısını verir.",
		arguments: [
			{
				name: "oran",
				description: "dönem başına düşen faiz oranı. Örneğin, %6 yıllık faiz oranına karşılık üç aylık ödeme için %6/4 kullanın"
			},
			{
				name: "devresel_ödeme",
				description: "her dönem yapılan ödeme miktarı; yıllık taksit dönemi boyunca değişemez"
			},
			{
				name: "bd",
				description: "bugünkü değer veya gelecekte yapılacak bir dizi ödemenin bugünkü toplam değeri"
			},
			{
				name: "gd",
				description: "gelecek değer ya da son ödeme yapıldıktan sonra elde etmek istediğiniz nakit bakiyesi. Atlanırsa sıfır kullanılır"
			},
			{
				name: "tür",
				description: "mantıksal değer: dönem başındaki ödemeler = 1; dönem sonundaki ödemeler = 0 ya da atlanmış"
			}
		]
	},
	{
		name: "TAMİŞGÜNÜ",
		description: "İki tarih arasındaki tüm işgünlerinin sayısını döndürür.",
		arguments: [
			{
				name: "başlangıç_tarihi",
				description: "başlangıç tarihini gösteren tarih seri numarası"
			},
			{
				name: "bitiş_tarihi",
				description: "bitiş tarihini gösteren tarih seri numarası"
			},
			{
				name: "tatiller",
				description: "isteğe göre çalışma takviminden çıkarılacak, bir veya daha fazla tarih seri numarası serisi; örneğin resmi tatiller"
			}
		]
	},
	{
		name: "TAMİŞGÜNÜ.ULUSL",
		description: "İki tarih arasındaki tam işgünlerinin sayısını özel hafta sonu parametreleriyle verir.",
		arguments: [
			{
				name: "başlangıç_tarihi",
				description: "başlangıç tarihini gösteren tarih seri numarası"
			},
			{
				name: "bitiş_tarihi",
				description: "bitiş tarihini gösteren tarih seri numarası"
			},
			{
				name: "hafta_sonu",
				description: "hafta sonunun ne zaman olduğunu belirten sayı veya dize"
			},
			{
				name: "tatiller",
				description: "isteğe göre çalışma takviminden çıkarılacak, bir veya daha fazla tarih seri numarası serisi; örneğin resmi tatiller"
			}
		]
	},
	{
		name: "TAMSAYI",
		description: "Bir sayıyı, sıfırdan ıraksayarak en yakın tam sayıya yuvarlar.",
		arguments: [
			{
				name: "sayı",
				description: "bir tam sayıya aşağıya yuvarlamak istediğiniz gerçek sayı"
			}
		]
	},
	{
		name: "TAN",
		description: "Bir sayının tanjantını verir.",
		arguments: [
			{
				name: "sayı",
				description: "tanjantını almak istediğiniz radyan cinsinden olan açı. * PI()/180 derece = radyan"
			}
		]
	},
	{
		name: "TANH",
		description: "Bir sayının hiperbolik tanjantını verir.",
		arguments: [
			{
				name: "sayı",
				description: "herhangi bir gerçek sayı"
			}
		]
	},
	{
		name: "TARİH",
		description: "Spreadsheet tarih-saat kodundaki tarihi gösteren sayıyı verir.",
		arguments: [
			{
				name: "yıl",
				description: "Windows için Spreadsheet'de 1900 - 9999, Macintosh için Spreadsheet'de 1904 - 9999 arasındaki bir sayı"
			},
			{
				name: "ay",
				description: "yılın ayını gösteren 1 ile 12 arasındaki sayı"
			},
			{
				name: "gün",
				description: "ayın gününü gösteren 1 ile 31 arasındaki sayı"
			}
		]
	},
	{
		name: "TARİHSAYISI",
		description: "Metin formunda bulunan bir tarihi Spreadsheet'deki tarih-saat kodunu gösteren bir sayıya dönüştürür.",
		arguments: [
			{
				name: "tarih_metni",
				description: "Bir tarihi Spreadsheet tarih biçiminde (Windows için 1/1/1900 ile 12/31/9999 arasında, Macintosh için 1/1/1904 ile 12/31/9999 arasında) gösteren metin"
			}
		]
	},
	{
		name: "TAVANAYUVARLA",
		description: "Bir sayıyı, yukarı doğru en yakın anlamlı sayı katına yuvarlar.",
		arguments: [
			{
				name: "sayı",
				description: "yuvarlamak istediğiniz değer"
			},
			{
				name: "anlam",
				description: "yuvarlamak istediğiniz kat"
			}
		]
	},
	{
		name: "TAVANAYUVARLA.DUYARLI",
		description: "Bir sayıyı, sıfırdan ıraksayarak, en yakın tamsayı veya anlamlı sayı katına yuvarlar.",
		arguments: [
			{
				name: "sayı",
				description: "yuvarlamak istediğiniz değer"
			},
			{
				name: "anlam",
				description: "yuvarlamak istediğiniz kat"
			}
		]
	},
	{
		name: "TAVANAYUVARLA.MATEMATİK",
		description: "Bir sayıyı, yukarı doğru en yakın tamsayı veya anlamlı sayı katına yuvarlar.",
		arguments: [
			{
				name: "sayı",
				description: "yuvarlamak istediğiniz değer"
			},
			{
				name: "anlam",
				description: "sayıyı yuvarlamak istediğiniz kat"
			},
			{
				name: "mod",
				description: "belirtildiğinde ve sıfır olmadığında bu işlev sıfırdan ıraksayarak yuvarlar"
			}
		]
	},
	{
		name: "TDAĞ",
		description: "T-dağılımını verir.",
		arguments: [
			{
				name: "x",
				description: "dağılımın değerini bulmak istediğiniz sayısal değer"
			},
			{
				name: "serbestlik_der",
				description: "dağılımı karakterize eden serbestlik derecesinin sayısını gösteren tamsayı"
			},
			{
				name: "yazı_say",
				description: "verilecek dağılım kuyruklarının sayısını belirtir: tek kuyruklu dağılım = 1; çift kuyruklu dağılım = 2"
			}
		]
	},
	{
		name: "TEK",
		description: "Bir sayıyı, mutlak değerce kendinden büyük en yakın tek tamsayıya yuvarlar.",
		arguments: [
			{
				name: "sayı",
				description: "yuvarlanacak değer"
			}
		]
	},
	{
		name: "TEKMİ",
		description: "Sayı bir tek sayı ise DOĞRU döndürür.",
		arguments: [
			{
				name: "sayı",
				description: "sınanacak sayı"
			}
		]
	},
	{
		name: "TEMİZ",
		description: "Metinden yazdırılamayan karakterleri kaldırır.",
		arguments: [
			{
				name: "metin",
				description: "yazılamayan karakterleri kaldırmak istediğiniz çalışma sayfası bilgisi"
			}
		]
	},
	{
		name: "TOPANAPARA",
		description: "İki dönem arasında borca ödenen bileşik anaparayı döndürür.",
		arguments: [
			{
				name: "oran",
				description: "faiz oranı"
			},
			{
				name: "dönem_sayısı",
				description: "ödeme dönemi toplam sayısı"
			},
			{
				name: "değer",
				description: "şimdiki değer"
			},
			{
				name: "başlangıç_dönemi",
				description: "hesaplamadaki ilk dönem"
			},
			{
				name: "bitiş_dönemi",
				description: "hesaplamadaki son dönem"
			},
			{
				name: "tür",
				description: "ödeme zamanı"
			}
		]
	},
	{
		name: "TOPKARE",
		description: "Bağımsız değişkenlerin karelerinin toplamını verir. Bağımsız değişkenler sayı, ad, dizi, ya da sayı içeren hücre başvuruları olabilir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "kareler toplamını bulmak istediğiniz en az 1 en fazla 255 sayı, ad, dizi ya da dizi başvurusudur"
			},
			{
				name: "sayı2",
				description: "kareler toplamını bulmak istediğiniz en az 1 en fazla 255 sayı, ad, dizi ya da dizi başvurusudur"
			}
		]
	},
	{
		name: "TOPLA",
		description: "Tüm sayıları bir hücre aralığına ekler.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "toplanacak en az 1 en fazla 255 sayıdır. Mantıksal değerler ve metin, hücrelerde yoksayılır, ancak bağımsız değişken olarak girilmişlerse eklenirler"
			},
			{
				name: "sayı2",
				description: "toplanacak en az 1 en fazla 255 sayıdır. Mantıksal değerler ve metin, hücrelerde yoksayılır, ancak bağımsız değişken olarak girilmişlerse eklenirler"
			}
		]
	},
	{
		name: "TOPLA.ÇARPIM",
		description: "Verilen aralık ya da dizilerde birbirine karşılık gelen sayısal bileşenleri çarpar ve bu çarpımların toplamını verir.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "dizi1",
				description: "Bileşenlerini önce çarpıp sonra toplamak istediğiniz en az 2 en fazla 255 dizidir. Tüm diziler aynı boyutlara sahip olmalıdır"
			},
			{
				name: "dizi2",
				description: "Bileşenlerini önce çarpıp sonra toplamak istediğiniz en az 2 en fazla 255 dizidir. Tüm diziler aynı boyutlara sahip olmalıdır"
			},
			{
				name: "dizi3",
				description: "Bileşenlerini önce çarpıp sonra toplamak istediğiniz en az 2 en fazla 255 dizidir. Tüm diziler aynı boyutlara sahip olmalıdır"
			}
		]
	},
	{
		name: "TOPÖDENENFAİZ",
		description: "İki dönem arasında ödenen bileşik faizi döndürür.",
		arguments: [
			{
				name: "oran",
				description: "faiz oranı"
			},
			{
				name: "dönem_sayısı",
				description: "ödeme dönemi toplam sayısı"
			},
			{
				name: "değer",
				description: "şimdiki değer"
			},
			{
				name: "başlangıç_dönemi",
				description: "hesaplamadaki ilk dönem"
			},
			{
				name: "bitiş_dönemi",
				description: "hesaplamadaki son dönem"
			},
			{
				name: "tür",
				description: "ödeme zamanı"
			}
		]
	},
	{
		name: "TOPX2AY2",
		description: "Birbirine karşılık gelen iki aralık ya da dizideki sayıların karelerinin toplamlarını hesaplar ve sonra da bu toplamların toplamını verir.",
		arguments: [
			{
				name: "dizi_x",
				description: "ilk sayı dizisi ya da aralığıdır ve sayı, ad, dizi, ya da sayı içeren başvurular olabilir"
			},
			{
				name: "dizi_y",
				description: "ikinci sayı aralığı veya dizisidir ve sayı, ad, dizi, ya da sayı içeren başvuru olabilir"
			}
		]
	},
	{
		name: "TOPX2EY2",
		description: "Birbirine karşılık gelen iki aralık ya da dizideki sayıların kareleri arasındaki farkı hesaplar ve sonra da bu farkların toplamını verir.",
		arguments: [
			{
				name: "dizi_x",
				description: "ilk sayı dizisi ya da aralığıdır ve sayı, ad, dizi, ya da sayı içeren başvurular olabilir"
			},
			{
				name: "dizi_y",
				description: "ikinci sayı aralığı veya dizisidir ve sayı, ad, dizi, ya da sayı içeren başvuru olabilir"
			}
		]
	},
	{
		name: "TOPXEY2",
		description: "Birbirine karşılık gelen iki aralık ya da dizideki değerlerin farklarını hesaplar ve sonra da bu farkların kareleri toplamını verir.",
		arguments: [
			{
				name: "dizi_x",
				description: "ilk değerler aralığı ya da dizisidir ve sayı, ad, dizi, ya da sayı içeren başvuru olabilir"
			},
			{
				name: "dizi_y",
				description: "ikinci değerler aralığı ya da dizisidir ve sayı, ad, dizi, ya da sayı içeren başvuru olabilir"
			}
		]
	},
	{
		name: "TTERS",
		description: "T-dağılımının iki kuyruklu tersini verir.",
		arguments: [
			{
				name: "olasılık",
				description: "iki kuyruklu t-dağılımının olasılığı, 0 ile 1 arasındaki (bunlar dahil) bir sayı"
			},
			{
				name: "serb_derecesi",
				description: "dağılımın türünü belirleyen serbestlik derecesinin sayısını gösteren pozitif tamsayı"
			}
		]
	},
	{
		name: "TTEST",
		description: "Bir t-Test için olasılığı verir.",
		arguments: [
			{
				name: "dizi1",
				description: "ilk veri kümesi"
			},
			{
				name: "dizi2",
				description: "ikinci veri kümesi"
			},
			{
				name: "yazı_say",
				description: "verilecek dağılım kuyruklarının sayısını belirtir: tek kuyruklu dağılım = 1; çift kuyruklu dağılım = 2"
			},
			{
				name: "tür",
				description: "uygulanacak t-test türü: eşli = 1, iki-örnek eşit varyans (homoskedastik) = 2, iki örnek eşit olmayan varyans = 3"
			}
		]
	},
	{
		name: "TÜMHATAİŞLEV",
		description: "Tümleyici hata işlevini döndürür.",
		arguments: [
			{
				name: "x",
				description: "ERF için alt sınır"
			}
		]
	},
	{
		name: "TÜMHATAİŞLEV.DUYARLI",
		description: "Tamamlayıcı hata işlevini döndürür.",
		arguments: [
			{
				name: "X",
				description: "TÜMHATAİŞLEV.DUYARLI işlevini tümleştirmek için alt sınırdır"
			}
		]
	},
	{
		name: "TÜR",
		description: "Değerin veri türünü gösteren sayıyı verir: sayı = 1; metin = 2; mantıksal değer = 4; hata değeri = 16; dizi = 64.",
		arguments: [
			{
				name: "değer",
				description: "herhangi bir değer olabilir"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Metnin ilk karakterine karşılık gelen sayıyı (kod noktası) döndürür.",
		arguments: [
			{
				name: "metin",
				description: "Unicode değerini bulmak istediğiniz karakter"
			}
		]
	},
	{
		name: "URLKODLA",
		description: "Bir URL kodlu dize döndürür.",
		arguments: [
			{
				name: "metin",
				description: "URL kodlu olması gereken bir dize"
			}
		]
	},
	{
		name: "ÜS",
		description: "Verilen bir sayının üssünün e sayısının üssü olarak kullanılması ile oluşan sonucu verir.",
		arguments: [
			{
				name: "sayı",
				description: "e tabanına uygulanan üs. Doğal logaritmanın temeli olan e sabiti 2,71828182845904 değerine eşittir"
			}
		]
	},
	{
		name: "ÜSTEL.DAĞ",
		description: "Üstel dağılımı verir.",
		arguments: [
			{
				name: "x",
				description: "işlevin değeri, negatif olmayan bir sayı"
			},
			{
				name: "lambda",
				description: "parametre değeri, pozitif bir sayı"
			},
			{
				name: "kümülatif",
				description: "işlevin vereceği mantıksal değer: kümülatif dağılım işlevi = DOĞRU; olasılık yoğunluğu işlevi = YANLIŞ"
			}
		]
	},
	{
		name: "ÜSTELDAĞ",
		description: "Üstel dağılımı verir.",
		arguments: [
			{
				name: "x",
				description: "işlevin değeri, negatif olmayan bir sayı"
			},
			{
				name: "lambda",
				description: "parametre değeri, pozitif bir sayı"
			},
			{
				name: "kümülatif",
				description: "işlevin vereceği mantıksal değer: kümülatif dağılım fonksiyonu = DOĞRU; olasılık yoğunluğu fonksiyonu = YANLIŞ"
			}
		]
	},
	{
		name: "UZUNLUK",
		description: "Bir karakter dizesi içerisindeki karakter sayısını verir.",
		arguments: [
			{
				name: "metin",
				description: "karakter sayısını bulmak istediğiniz metin. Boşluklar karakter olarak sayılır"
			}
		]
	},
	{
		name: "VAL",
		description: "Belirttiğiniz koşullara uyan tek bir kaydı veritabanından çıkarır.",
		arguments: [
			{
				name: "veritabanı",
				description: "veritabanını ya da listeyi oluşturan hücreler aralığı. Veritabanı ilgili verilerin listesidir"
			},
			{
				name: "alan",
				description: "çift tırnak işareti içindeki sütun etiketi, ya da sütunun listedeki konumunu gösteren sayı"
			},
			{
				name: "ölçüt",
				description: "belirttiğiniz koşulları içeren hücreler aralığı. Aralık, sütun etiketi ile etiketin altında bulunan ve koşulu taşıyan bir hücreyi içerir"
			}
		]
	},
	{
		name: "VAR",
		description: "Bir örneğe dayanarak varyansı tahmin eder (örnekteki mantıksal değerleri ve metni yoksayar).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "popülasyondan alınmış bir örneğe karşılık gelen en az 1 en fazla 255 sayısal bağımsız değişkendir"
			},
			{
				name: "sayı2",
				description: "popülasyondan alınmış bir örneğe karşılık gelen en az 1 en fazla 255 sayısal bağımsız değişkendir"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Tüm popülasyonun varyansını hesaplar (popülasyondaki mantıksal değerleri ve metni yoksayar).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "Popülasyona karşılık gelen en az 1 en fazla 255 sayısal bağımsız değişkendir"
			},
			{
				name: "sayı2",
				description: "Popülasyona karşılık gelen en az 1 en fazla 255 sayısal bağımsız değişkendir"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Bir örneğe dayanarak varyansı tahmin eder (örnekteki mantıksal değerleri ve metni yoksayar).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "Popülasyondan alınmış bir örneğe karşılık gelen en az 1 en fazla 255 sayısal bağımsız değişkendir"
			},
			{
				name: "sayı2",
				description: "Popülasyondan alınmış bir örneğe karşılık gelen en az 1 en fazla 255 sayısal bağımsız değişkendir"
			}
		]
	},
	{
		name: "VARA",
		description: "Mantıksal değerler ve metin içeren bir örneğin varyansını tahmin eder. Metin ve YANLIŞ mantıksal değer 0; DOĞRU mantıksal değer ise 1 değerini alır.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "değer1",
				description: "bir popülasyonun bir örneğine karşılık gelen en az 1 en fazla 255 değer bağımsız değişkenidir"
			},
			{
				name: "değer2",
				description: "bir popülasyonun bir örneğine karşılık gelen en az 1 en fazla 255 değer bağımsız değişkenidir"
			}
		]
	},
	{
		name: "VARS",
		description: "Tüm popülasyonun varyansını hesaplar (popülasyondaki mantıksal değerleri ve metni yoksayar).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sayı1",
				description: "popülasyona karşılık gelen en az 1 en fazla 255 sayısal bağımsız değişkendir"
			},
			{
				name: "sayı2",
				description: "popülasyona karşılık gelen en az 1 en fazla 255 sayısal bağımsız değişkendir"
			}
		]
	},
	{
		name: "VARSA",
		description: "Mantıksal değerler ve metin içeren bir popülasyon için varyansı hesaplar. Metin ve YANLIŞ mantıksal değer 0; DOĞRU mantıksal değer ise 1 değerini alır.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "değer1",
				description: "bir popülasyona karşılık gelen en az 1 en fazla 255 değer bağımsız değişkenidir"
			},
			{
				name: "değer2",
				description: "bir popülasyona karşılık gelen en az 1 en fazla 255 değer bağımsız değişkenidir"
			}
		]
	},
	{
		name: "VE",
		description: "Tüm bağımsız değişkenlerin DOĞRU olup olmadığını denetler, tümü DOĞRU ise DOĞRU döndürür.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "mantıksal1",
				description: "DOĞRU veya YANLIŞ olduğunu sınamak istediğiniz en az 1 en fazla 255 koşuldur ve her biri mantıksal değer, dizi ya da başvuru olabilir"
			},
			{
				name: "mantıksal2",
				description: "DOĞRU veya YANLIŞ olduğunu sınamak istediğiniz en az 1 en fazla 255 koşuldur ve her biri mantıksal değer, dizi ya da başvuru olabilir"
			}
		]
	},
	{
		name: "VSEÇÇARP",
		description: "Veritabanındaki kayıt alanında (sütun) bulunan ve belirttiğiniz koşullara uyan verileri çarpar.",
		arguments: [
			{
				name: "veritabanı",
				description: "veritabanını ya da listeyi oluşturan hücreler aralığı. Veritabanı ilgili verilerin listesidir"
			},
			{
				name: "alan",
				description: "çift tırnak işareti içindeki sütun etiketi, ya da sütunun listedeki konumunu gösteren sayı"
			},
			{
				name: "ölçüt",
				description: "belirttiğiniz koşulları içeren hücreler aralığı. Aralık, sütun etiketi ile etiketin altında bulunan ve koşulu taşıyan bir hücreyi içerir"
			}
		]
	},
	{
		name: "VSEÇMAK",
		description: "Veritabanındaki kayıt alanında (sütun) bulunan ve belirttiğiniz koşullara uyan en büyük sayıyı verir.",
		arguments: [
			{
				name: "veritabanı",
				description: "veritabanını ya da listeyi oluşturan hücreler aralığı. Veritabanı ilgili verilerin listesidir"
			},
			{
				name: "alan",
				description: "çift tırnak işareti içindeki sütun etiketi, ya da sütunun listedeki konumunu gösteren sayı"
			},
			{
				name: "ölçüt",
				description: "belirttiğiniz koşulları içeren hücreler aralığı. Aralık, sütun etiketi ile etiketin altında bulunan ve koşulu taşıyan bir hücreyi içerir"
			}
		]
	},
	{
		name: "VSEÇMİN",
		description: "Veritabanındaki kayıt alanında (sütun) bulunan ve belirttiğiniz koşullara uyan en küçük sayıyı verir.",
		arguments: [
			{
				name: "veritabanı",
				description: "veritabanını ya da listeyi oluşturan hücreler aralığı. Veritabanı ilgili verilerin listesidir"
			},
			{
				name: "alan",
				description: "çift tırnak işareti içindeki sütun etiketi, ya da sütunun listedeki konumunu gösteren sayı"
			},
			{
				name: "ölçüt",
				description: "belirttiğiniz koşulları içeren hücreler aralığı. Aralık, sütun etiketi ile etiketin altında bulunan ve koşulu taşıyan bir hücreyi içerir"
			}
		]
	},
	{
		name: "VSEÇORT",
		description: "Bir liste ya da veritabanındaki bir sütunda yer alan ve belirttiğiniz koşullara uyan değerlerin ortalamasını verir.",
		arguments: [
			{
				name: "veritabanı",
				description: "veritabanını ya da listeyi oluşturan hücreler aralığı. Veritabanı ilgili verilerin listesidir"
			},
			{
				name: "alan",
				description: "çift tırnak işareti içindeki sütun etiketi, ya da sütunun listedeki konumunu gösteren sayı"
			},
			{
				name: "ölçüt",
				description: "belirttiğiniz koşulları içeren hücreler aralığı. Aralık, sütun etiketi ile etiketin altında bulunan ve koşulu taşıyan bir hücreyi içerir"
			}
		]
	},
	{
		name: "VSEÇSAY",
		description: "Veritabanındaki kayıt alanında (sütun) bulunan sayıları içeren ve belirttiğiniz koşullara uyan hücreleri sayar.",
		arguments: [
			{
				name: "veritabanı",
				description: "veritabanını ya da listeyi oluşturan hücre aralığı. Veritabanı ilgili verilerin listesidir"
			},
			{
				name: "alan",
				description: "çift tırnak işareti içindeki sütun etiketi veya sütunun listedeki konumunu gösteren sayı"
			},
			{
				name: "ölçüt",
				description: "belirttiğiniz koşulları içeren hücre aralığı. Aralık, sütun etiketi ile etiketin altında bulunan ve koşulu taşıyan bir hücreyi içerir"
			}
		]
	},
	{
		name: "VSEÇSAYDOLU",
		description: "Veritabanındaki kayıt alanında (sütun) bulunan ve belirttiğiniz koşullara uyan boş olmayan hücreleri sayar.",
		arguments: [
			{
				name: "veritabanı",
				description: "veritabanını ya da listeyi oluşturan hücreler aralığı. Veritabanı ilgili verilerin listesidir"
			},
			{
				name: "alan",
				description: "çift tırnak işareti içindeki sütun etiketi, ya da sütunun listedeki konumunu gösteren sayı"
			},
			{
				name: "ölçüt",
				description: "belirttiğiniz koşulları içeren hücreler aralığı. Aralık, sütun etiketi ile etiketin altında bulunan ve koşulu taşıyan bir hücreyi içerir"
			}
		]
	},
	{
		name: "VSEÇSTDSAPMA",
		description: "Seçili veritabanı girdilerinden alınan bir örneğin standart sapmasını tahmin eder.",
		arguments: [
			{
				name: "veritabanı",
				description: "veritabanını ya da listeyi oluşturan hücreler aralığı. Veritabanı ilgili verilerin listesidir"
			},
			{
				name: "alan",
				description: "çift tırnak işareti içindeki sütun etiketi, ya da sütunun listedeki konumunu gösteren sayı"
			},
			{
				name: "ölçüt",
				description: "belirttiğiniz koşulları içeren hücreler aralığı. Aralık, sütun etiketi ile etiketin altında bulunan ve koşulu taşıyan bir hücreyi içerir"
			}
		]
	},
	{
		name: "VSEÇSTDSAPMAS",
		description: "Seçili veritabanı girdilerinden oluşan tüm popülasyonun standart sapmasını hesaplar.",
		arguments: [
			{
				name: "veritabanı",
				description: "veritabanını ya da listeyi oluşturan hücreler aralığı. Veritabanı ilgili verilerin listesidir"
			},
			{
				name: "alan",
				description: "çift tırnak işareti içindeki sütun etiketi, ya da sütunun listedeki konumunu gösteren sayı"
			},
			{
				name: "ölçüt",
				description: "belirttiğiniz koşulları içeren hücreler aralığı. Aralık, sütun etiketi ile etiketin altında bulunan ve koşulu taşıyan bir hücreyi içerir"
			}
		]
	},
	{
		name: "VSEÇTOPLA",
		description: "Veritabanındaki kayıt alanında (sütun) bulunan ve belirttiğiniz koşullara uyan sayıları toplar.",
		arguments: [
			{
				name: "veritabanı",
				description: "veritabanını ya da listeyi oluşturan hücreler aralığı. Veritabanı ilgili verilerin listesidir"
			},
			{
				name: "alan",
				description: "çift tırnak işareti içindeki sütun etiketi, ya da sütunun listedeki konumunu gösteren sayı"
			},
			{
				name: "ölçüt",
				description: "belirttiğiniz koşulları içeren hücreler aralığı. Aralık, sütun etiketi ile etiketin altında bulunan ve koşulu taşıyan bir hücreyi içerir"
			}
		]
	},
	{
		name: "VSEÇVAR",
		description: "Seçili veritabanı girdilerinden alınan örneğin varyansını tahmin eder.",
		arguments: [
			{
				name: "veritabanı",
				description: "veritabanını ya da listeyi oluşturan hücreler aralığı. Veritabanı ilgili verilerin listesidir"
			},
			{
				name: "alan",
				description: "çift tırnak işareti içindeki sütun etiketi, ya da sütunun listedeki konumunu gösteren sayı"
			},
			{
				name: "ölçüt",
				description: "belirttiğiniz koşulları içeren hücreler aralığı. Aralık, sütun etiketi ile etiketin altında bulunan ve koşulu taşıyan bir hücreyi içerir"
			}
		]
	},
	{
		name: "VSEÇVARS",
		description: "Seçili veritabanı popülasyonunun varyansını hesaplar.",
		arguments: [
			{
				name: "veritabanı",
				description: "veritabanını ya da listeyi oluşturan hücreler aralığı. Veritabanı ilgili verilerin listesidir"
			},
			{
				name: "alan",
				description: "çift tırnak işareti içindeki sütun etiketi, ya da sütunun listedeki konumunu gösteren sayı"
			},
			{
				name: "ölçüt",
				description: "belirttiğiniz koşulları içeren hücreler aralığı. Aralık, sütun etiketi ile etiketin altında bulunan ve koşulu taşıyan bir hücreyi içerir"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Weibull dağılımını verir.",
		arguments: [
			{
				name: "x",
				description: "işlevin sonucunu veren değer, negatif olmayan bir sayı"
			},
			{
				name: "alfa",
				description: "dağılım parametresi, pozitif bir sayı"
			},
			{
				name: "beta",
				description: "dağılım parametresi, pozitif bir sayı"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif dağılım fonksiyonu için DOĞRU'yu; olasılık kütle fonksiyonu için YANLIŞ'ı kullanın"
			}
		]
	},
	{
		name: "WEIBULL.DAĞ",
		description: "Weibull dağılımını verir.",
		arguments: [
			{
				name: "x",
				description: "işlevin sonucunu veren değer, negatif olmayan bir sayı"
			},
			{
				name: "alfa",
				description: "dağılım parametresi, pozitif bir sayı"
			},
			{
				name: "beta",
				description: "dağılım parametresi, pozitif bir sayı"
			},
			{
				name: "kümülatif",
				description: "mantıksal değer: kümülatif dağılım fonksiyonu için DOĞRU'yu; olasılık kütle fonksiyonu için YANLIŞ'ı kullanın"
			}
		]
	},
	{
		name: "YADA",
		description: "Bağımsız değişkenlerin DOĞRU olup olmadığını denetler ve DOĞRU veya YANLIŞ döndürür. Yalnızca bağımsız değişkenlerin tümü YANLIŞ ise YANLIŞ döndürür.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "mantıksal1",
				description: "sınamak istediğiniz en az 1 en fazla 255 koşuldur, her biri DOĞRU veya YANLIŞ olabilir"
			},
			{
				name: "mantıksal2",
				description: "sınamak istediğiniz en az 1 en fazla 255 koşuldur, her biri DOĞRU veya YANLIŞ olabilir"
			}
		]
	},
	{
		name: "YANLIŞ",
		description: "YANLIŞ mantıksal değerini verir.",
		arguments: [
		]
	},
	{
		name: "YAT",
		description: "Bir malın belirtilen bir dönem için yıpranmasını verir.",
		arguments: [
			{
				name: "maliyet",
				description: "malın ilk maliyeti"
			},
			{
				name: "hurda",
				description: "malın kullanım ömrü bittikten sonraki hurda değeri"
			},
			{
				name: "ömür",
				description: "malın yıpranma dönemi miktarı (bazen malın kullanım ömrü olarak da kullanılır)"
			},
			{
				name: "dönem",
				description: "Ömür birimiyle aynı birimde olması gereken dönem"
			}
		]
	},
	{
		name: "YATAYARA",
		description: "tablonun üst satırındaki değeri ya da değerler dizisini arar ve aynı sütunda belirtilen satırdan değeri verir.",
		arguments: [
			{
				name: "aranan_değer",
				description: "tablonun ilk satırında bulunması gereken değerdir ve bir değer, başvuru veya metin olabilir"
			},
			{
				name: "tablo_dizisi",
				description: "verinin arandığı metin tablosu, sayılar ya da mantıksal değerlerdir. Tablo_dizisi bir aralığa ya da bir aralık adına yapılan bir başvuru olabilir"
			},
			{
				name: "satır_indis_sayısı",
				description: "eşleşen değerin geleceği tablo_dizisi'nde bulunan satır sayısı. Tablodaki ilk veri satırı satır 1'dir"
			},
			{
				name: "aralık_bak",
				description: "mantıksal değer: üst satırdaki (artan sırada sıralanmış) en yakın eşleşmeyi bulmak için = DOĞRU ya da atlanmış; tam eşleşmeyi bulmak için = YANLIŞ"
			}
		]
	},
	{
		name: "YAZIM.DÜZENİ",
		description: "Metin dizesindeki her sözcüğün ilk harfini büyük harfe, diğer tüm harfleri de küçük harfe dönüştürür.",
		arguments: [
			{
				name: "metin",
				description: "tırnak işareti içinde bir metin, metin veren bir formül, ya da kısmen büyük harf yapacağınız metin içeren bir hücreye başvuru"
			}
		]
	},
	{
		name: "YERİNEKOY",
		description: "Metin dizesindeki eski bir metni yenisiyle değiştirir.",
		arguments: [
			{
				name: "metin",
				description: "karakterlerini değiştirmek istediğiniz metni içeren hücre başvurusu veya metin"
			},
			{
				name: "eski_metin",
				description: "değiştirmek istediğiniz metin. Eski_metin'in büyük ve küçük harfleriyle eşleşmezse YERİNEKOY metni değiştirmez"
			},
			{
				name: "yeni_metin",
				description: "Eski_metin ile değiştirmek istediğiniz metin"
			},
			{
				name: "yineleme_sayısı",
				description: "hangi Eski_metin örneğini değiştirmek istediğinizi belirtir. Atlanırsa, Eski_metin'in her örneği değiştirilir"
			}
		]
	},
	{
		name: "YIL",
		description: "1900 - 9999 aralığındaki bir tamsayı ile ifade edilen tarihin yılını döndürür.",
		arguments: [
			{
				name: "seri_no",
				description: "Spreadsheet tarafından kullanılan tarih-saat kodundaki sayı"
			}
		]
	},
	{
		name: "YILORAN",
		description: "Başlangıç ve bitiş tarihleri arasındaki tam gün sayısını gösteren yıl oranını döndürür.",
		arguments: [
			{
				name: "başlangıç_tarihi",
				description: "başlangıç tarihini gösteren tarih seri numarası"
			},
			{
				name: "bitiş_tarihi",
				description: "bitiş tarihini gösteren tarih seri numarası"
			},
			{
				name: "temel",
				description: "kullanılacak gün sayısı türü"
			}
		]
	},
	{
		name: "YİNELE",
		description: "Bir metni verilen sayıda yineler. Hücreyi metin dizesindeki birçok örnekle doldurmak için YİNELE'yi kullanın.",
		arguments: [
			{
				name: "metin",
				description: "yinelemek istediğiniz metin"
			},
			{
				name: "sayı_kere",
				description: "metnin kaç kez yineleneceğini belirten sayı"
			}
		]
	},
	{
		name: "YOKSAY",
		description: "#YOK hata değerini verir (kullanılabilir değer yok).",
		arguments: [
		]
	},
	{
		name: "YUKARIYUVARLA",
		description: "Bir sayıyı sıfırdan ıraksayarak yukarı yuvarlar.",
		arguments: [
			{
				name: "sayı",
				description: "yukarı yuvarlamak istediğiniz herhangi bir gerçek sayı"
			},
			{
				name: "sayı_rakamlar",
				description: "sayıyı yuvarlamak istediğiniz rakam sayısı. Negatif sayılar ondalık noktasının soluna; sıfır ya da atlanmış olanlar ise en yakın tamsayıya yuvarlanır"
			}
		]
	},
	{
		name: "YUVARLA",
		description: "Sayıyı belirli sayıdaki rakama yuvarlar.",
		arguments: [
			{
				name: "sayı",
				description: "yuvarlamak istediğiniz sayı"
			},
			{
				name: "sayı_rakamlar",
				description: "sayıyı yuvarlamak istediğiniz rakam sayısı. Negatif, ondalık noktasının soluna; sıfır ise en yakın tamsayıya yuvarlanır"
			}
		]
	},
	{
		name: "YÜZDEBİRLİK",
		description: "Bir aralık içerisindeki değerlerin k. yüzdebir toplamını verir.",
		arguments: [
			{
				name: "dizi",
				description: "göreceli durumu tanımlayan veri dizisi ya da aralığı"
			},
			{
				name: "k",
				description: "0 ile 1 arasındaki (bunlar dahil) yüzdebirlik değeri"
			}
		]
	},
	{
		name: "YÜZDEBİRLİK.DHL",
		description: "Aralıktaki değerlerin k. yüzdebirliğini verir; k, 0..1 aralığındadır (bunlar dahil).",
		arguments: [
			{
				name: "dizi",
				description: "göreceli durumu tanımlayan veri dizisi ya da aralığı"
			},
			{
				name: "k",
				description: "0 ile 1 arasındaki (bunlar dahil) yüzdebirlik değeri"
			}
		]
	},
	{
		name: "YÜZDEBİRLİK.HRC",
		description: "Aralıktaki değerlerin k. yüzdebirliğini verir; k, 0..1 aralığındadır (bunlar hariç).",
		arguments: [
			{
				name: "dizi",
				description: "göreceli durumu tanımlayan veri dizisi ya da aralığı"
			},
			{
				name: "k",
				description: "0 ile 1 arasındaki (bunlar dahil) yüzdebirlik değeri"
			}
		]
	},
	{
		name: "YÜZDERANK",
		description: "Bir veri kümesindeki bir değerin sırasını, veri kümesinin yüzdesi olarak verir.",
		arguments: [
			{
				name: "dizi",
				description: "göreceli durumu tanımlayan sayısal değerli veri dizisi veya aralığı"
			},
			{
				name: "x",
				description: "derecesini öğrenmek istediğiniz değer"
			},
			{
				name: "anlam",
				description: "dönen yüzde oranı için anlamlı rakam sayısını belirten isteğe bağlı değer, atlanırsa üç rakam kullanılır (0.xxx%)"
			}
		]
	},
	{
		name: "YÜZDERANK.DHL",
		description: "Bir veri kümesindeki değerin derecesini veri kümesinin yüzdesi (0..1, bunlar dahil) olarak verir.",
		arguments: [
			{
				name: "dizi",
				description: "göreceli durumu tanımlayan sayısal değerli veri dizisi veya aralığı"
			},
			{
				name: "x",
				description: "derecesini öğrenmek istediğiniz değer"
			},
			{
				name: "anlam",
				description: "dönen yüzde oranı için anlamlı rakam sayısını belirten isteğe bağlı değer, atlanırsa üç rakam kullanılır (0.xxx%)"
			}
		]
	},
	{
		name: "YÜZDERANK.HRC",
		description: "Bir veri kümesindeki değerin derecesini veri kümesinin yüzdesi (0..1, bunlar hariç) olarak verir.",
		arguments: [
			{
				name: "dizi",
				description: "göreceli durumu tanımlayan sayısal değerli veri dizisi veya aralığı"
			},
			{
				name: "x",
				description: "derecesini öğrenmek istediğiniz değer"
			},
			{
				name: "anlam",
				description: "dönen yüzde oranı için anlamlı rakam sayısını belirten isteğe bağlı değer, atlanırsa üç rakam kullanılır (0.xxx%)"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Bir z-test'in tek kuyruklu P-değerini verir.",
		arguments: [
			{
				name: "dizi",
				description: "X'in sınanacağı veri dizisi veya aralığıdır"
			},
			{
				name: "x",
				description: "sınanacak değerdir"
			},
			{
				name: "sigma",
				description: "popülasyonun (bilinen) standart sapmasıdır. Atlanırsa, örnek standart sapma kullanılır"
			}
		]
	},
	{
		name: "ZAMAN",
		description: "Saat, dakika, saniye olarak girilen sayıları zaman biçimindeki Spreadsheet seri numarasına dönüştürür.",
		arguments: [
			{
				name: "saat",
				description: "0-23 arasında saati gösteren sayı"
			},
			{
				name: "dakika",
				description: "0-59 arasında dakikayı gösteren sayı"
			},
			{
				name: "saniye",
				description: "0-59 arasında saniyeyi gösteren sayı"
			}
		]
	},
	{
		name: "ZAMANSAYISI",
		description: "Bir metin dizesiyle (saat_metni) gösterilen bir saati 0 (00:00:00) ile 0,999988426 (23:59:59) arasındaki Spreadsheet saat seri numarasına çevirir. Formülü girdikten sonra sayıyı saat biçiminde biçimlendirin.",
		arguments: [
			{
				name: "saat_metni",
				description: "saati Spreadsheet saat biçimlerinden herhangi biriyle gösteren metin dizesi (dizedeki tarih bilgisi yoksayılır)"
			}
		]
	},
	{
		name: "ZTEST",
		description: "Bir z-test'in tek kuyruklu P-değerini verir.",
		arguments: [
			{
				name: "dizi",
				description: "X'in sınanacağı veri dizisi veya aralığıdır"
			},
			{
				name: "x",
				description: "sınanacak değerdir"
			},
			{
				name: "sigma",
				description: "popülasyonun (bilinen) standart sapmasıdır. Atlanırsa, örnek standart sapma kullanılır"
			}
		]
	}
];