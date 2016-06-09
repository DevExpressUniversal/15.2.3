ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Returnează valoarea absolută a unui număr, un număr fără semnul său.",
		arguments: [
			{
				name: "număr",
				description: "este numărul real pentru care se calculează valoarea absolută"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "Returnează dobânda sporită pentru o asigurare care plătește dobândă la maturitate.",
		arguments: [
			{
				name: "emis",
				description: "este data de emitere a asigurării, exprimată ca număr serial"
			},
			{
				name: "tranzacție",
				description: "este data de maturitate a asigurării, exprimată ca număr serial"
			},
			{
				name: "rată",
				description: "este rata anuală a cuponului de asigurare"
			},
			{
				name: "par",
				description: "este valoarea pară a asigurării"
			},
			{
				name: "bază",
				description: "este tipul de bază de numărare a zilelor care trebuie utilizat"
			}
		]
	},
	{
		name: "ACOS",
		description: "Returnează arccosinusul unui număr, în radiani în intervalul 0 Pi. Arccosinusul este unghiul al cărui cosinus este număr.",
		arguments: [
			{
				name: "număr",
				description: "este cosinusul unghiului și este între -1 și 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Returnează inversa cosinusului hiperbolic al unui număr.",
		arguments: [
			{
				name: "număr",
				description: "este orice număr real egal sau mai mare decât 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Returnează arccotangenta unui număr, exprimată în radiani, din zona de la 0 la Pi.",
		arguments: [
			{
				name: "număr",
				description: "este cotangenta unghiului pe care îl doriți"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Returnează cotangenta hiperbolică inversată a unui număr.",
		arguments: [
			{
				name: "număr",
				description: "este cotangenta hiperbolică a unghiului pe care îl doriți"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "Creează o referință la celulă ca text, având date numerele de rând și coloană.",
		arguments: [
			{
				name: "num_rând",
				description: "este numărul rândului de utilizat în referința la celulă: Row_număr = 1 pentru rândul 1"
			},
			{
				name: "num_coloană",
				description: "este numărul de coloană de utilizat în referința la celulă. De exemplu, Column_număr = 4 pentru coloana D"
			},
			{
				name: "num_abs",
				description: "specifică tipul de referință: absolută = 1; rând absolut/coloană relativă = 2; rând relativ/coloană absolută = 3; relativă = 4"
			},
			{
				name: "a1",
				description: "este o valoare logică și specifică stilul de referință: stil A1 = 1 sau TRUE; stil R1C1 = 0 sau FALSE"
			},
			{
				name: "text_foaie",
				description: "este textul care  specifică numele foii de lucru utilizată ca referință externă"
			}
		]
	},
	{
		name: "AND",
		description: "Verifică dacă toate argumentele sunt TRUE și întoarce TRUE dacă toate argumentele sunt TRUE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logic1",
				description: "sunt de la 1 la 255 de condiții de testat care pot fi TRUE sau FALSE și pot fi valori logice, matrice sau referințe"
			},
			{
				name: "logic2",
				description: "sunt de la 1 la 255 de condiții de testat care pot fi TRUE sau FALSE și pot fi valori logice, matrice sau referințe"
			}
		]
	},
	{
		name: "ARABIC",
		description: "Efectuează conversia unui număr roman în număr arab.",
		arguments: [
			{
				name: "text",
				description: "este numărul roman căruia îi efectuați conversia"
			}
		]
	},
	{
		name: "AREAS",
		description: "Returnează numărul de suprafețe dintr-o referință. O suprafață este o zonă contiguă de celule sau o singură celulă.",
		arguments: [
			{
				name: "referință",
				description: "este o referință la o celulă sau zonă de celule și se poate referi la suprafețe multiple"
			}
		]
	},
	{
		name: "ASIN",
		description: "Returnează arcsinusul unui număr în radiani, în intervalul -Pi/2 Pi/2.",
		arguments: [
			{
				name: "număr",
				description: "este sinusul unghiului și este de la -1 la 1"
			}
		]
	},
	{
		name: "ASINH",
		description: "Returnează inversa sinusului hiperbolic al unui număr.",
		arguments: [
			{
				name: "număr",
				description: "este orice număr real egal sau mai mare decât 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Returnează arctangenta unui număr în radiani, în intervalul -Pi/2  Pi/2.",
		arguments: [
			{
				name: "număr",
				description: "este tangenta unghiului care se calculează"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Returnează arctangenta coordonatelor x și y specificate, în radiani între -Pi și Pi, excluzând -Pi.",
		arguments: [
			{
				name: "num_x",
				description: "este coordonata x a punctului"
			},
			{
				name: "num_y",
				description: "este coordonata y a punctului"
			}
		]
	},
	{
		name: "ATANH",
		description: "Returnează inversa tangentei hiperbolice a unui număr.",
		arguments: [
			{
				name: "număr",
				description: "este orice număr real între -1 și 1 excluzând -1 și 1"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "Returnează media deviațiilor absolute ale punctelor de date față de media lor. Argumentele pot fi numere sau nume, matrice sau referințe care conțin numere.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de argumente pentru care se calculează media deviațiilor absolute"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de argumente pentru care se calculează media deviațiilor absolute"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "Returnează media (aritmetică) a argumentelor sale, care pot fi numere sau nume, matrice sau referințe care conțin numere.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de argumente numerice pentru care se calculează media"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de argumente numerice pentru care se calculează media"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "Returnează media (aritmetică) a argumentelor sale, evaluând textul și valorile FALSE din argumente ca 0; TRUE se evaluează ca 1. Argumentele pot fi numere, nume, matrice sau referințe.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valoare1",
				description: "sunt de la 1 la 255 de argumente pentru care se calculează media"
			},
			{
				name: "valoare2",
				description: "sunt de la 1 la 255 de argumente pentru care se calculează media"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "Găsește media(aritmetică) pentru celulele specificate printr-o condiție sau criteriu date.",
		arguments: [
			{
				name: "zonă",
				description: "este intervalul de celule de evaluat"
			},
			{
				name: "criterii",
				description: " este condiția sau criteriul sub forma unui număr, a unei expresii sau a unui text care definește care celule se vor utiliza pentru a găsi media"
			},
			{
				name: "zonă_medie",
				description: "sunt chiar celulele care se vor utiliza pentru a găsi media. Dacă se omit, se vor utiliza celulele din interval"
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "Găsește media(aritmetică) pentru celulele specificate printr-un set de condiții sau criterii.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "zonă_medie",
				description: "sunt chiar celulele care se vor utiliza pentru a găsi media."
			},
			{
				name: "zonă_criterii",
				description: "este intervalul de celule de evaluat pentru o anumită stare"
			},
			{
				name: "criterii",
				description: "este condiția sau criteriul sub forma unui număr, a unei expresii sau a unui text care definește care celule se vor utiliza pentru a găsi media"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Transformă un număr în text (baht).",
		arguments: [
			{
				name: "număr",
				description: "este numărul de transformat"
			}
		]
	},
	{
		name: "BASE",
		description: "Efectuează conversia unui număr într-o reprezentare text cu baza dată.",
		arguments: [
			{
				name: "număr",
				description: "este numărul căruia îi efectuați conversia"
			},
			{
				name: "bază",
				description: "este baza în care doriți să efectuați conversia numărului"
			},
			{
				name: "lungime_min",
				description: "este lungimea minimă a șirului returnat. Dacă sunt omise, nu se adaugă zerourile inițiale"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Returnează funcția Bessel modificată In(x).",
		arguments: [
			{
				name: "x",
				description: "este valoarea la care se evaluează funcția"
			},
			{
				name: "n",
				description: "este ordinea funcției Bessel"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Returnează funcția Bessel Jn(x).",
		arguments: [
			{
				name: "x",
				description: "este valoarea la care se evaluează funcția"
			},
			{
				name: "n",
				description: "este ordinea funcției Bessel"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Returnează funcția Bessel modificată Kn(x).",
		arguments: [
			{
				name: "x",
				description: "este valoarea la care se evaluează funcția"
			},
			{
				name: "n",
				description: "este ordinea funcției"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Returnează funcția Bessel  Yn(x).",
		arguments: [
			{
				name: "x",
				description: "este valoarea la care se evaluează funcția"
			},
			{
				name: "n",
				description: "este ordinea funcției"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "Returnează funcția de distribuție a probabilității beta.",
		arguments: [
			{
				name: "x",
				description: "este valoarea între A și B în care se evaluează funcția"
			},
			{
				name: "alfa",
				description: "este un parametru al distribuției și trebuie să fie mai mare decât 0"
			},
			{
				name: "beta",
				description: "este un parametru al distribuției și trebuie să fie mai mare decât 0"
			},
			{
				name: "cumulativ",
				description: "este opțional limita inferioară a intervalului x. Dacă este omisă, A = 0"
			},
			{
				name: "A",
				description: "este o valoare logică: pentru funcția de distribuție cumulativă, se utilizează TRUE; pentru funcția de probabilitate a densității, se utilizează FALSE"
			},
			{
				name: "B",
				description: "este opțional limita superioară a intervalului x. Dacă este omisă, B = 1"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Returnează inversa funcției de densitate de probabilitate beta cumulativă (BETA.DIST).",
		arguments: [
			{
				name: "probabilitate",
				description: "este o probabilitate asociată cu distribuția beta"
			},
			{
				name: "alfa",
				description: "este un parametru al distribuției și trebuie să fie mai mare decât 0"
			},
			{
				name: "beta",
				description: "este un parametru al distribuției și trebuie să fie mai mare decât 0"
			},
			{
				name: "A",
				description: "este opțional limita inferioară a intervalului x. Dacă este omisă, A = 0"
			},
			{
				name: "B",
				description: "este opțional limita superioară a intervalului x. Dacă este omisă, B = 1"
			}
		]
	},
	{
		name: "BETADIST",
		description: "Returnează funcția densitate de probabilitate beta cumulativă.",
		arguments: [
			{
				name: "x",
				description: "este valoarea între A și B la care se evaluează funcția"
			},
			{
				name: "alfa",
				description: "este un parametru al distribuției și trebuie să fie mai mare decât 0"
			},
			{
				name: "beta",
				description: "este un parametru al distribuției și trebuie să fie mai mare decât 0"
			},
			{
				name: "A",
				description: "este limita inferioară opțională a intervalului x. Dacă se omite, A = 0"
			},
			{
				name: "B",
				description: "este limita superioară opțională a intervalului x. Dacă se omite, B = 1"
			}
		]
	},
	{
		name: "BETAINV",
		description: "Returnează inversa funcției de densitate de probabilitate beta cumulativă (BETADIST).",
		arguments: [
			{
				name: "probabilitate",
				description: "este o probabilitate asociată cu distribuția beta"
			},
			{
				name: "alfa",
				description: "este un parametru al distribuției și trebuie să fie mai mare decât 0"
			},
			{
				name: "beta",
				description: "este un parametru al distribuției și trebuie să fie mai mare decât 0"
			},
			{
				name: "A",
				description: "este limita inferioară opțională a intervalului x. Dacă se omite, A = 0"
			},
			{
				name: "B",
				description: "este limita superioară opțională a intervalului x. Dacă se omite, B = 1"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "Se efectuează conversia unui număr binar într-un număr zecimal.",
		arguments: [
			{
				name: "număr",
				description: "este numărul binar care va suferi conversia"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "Se efectuează conversia unui număr binar într-un număr hexazecimal.",
		arguments: [
			{
				name: "număr",
				description: "este numărul binar care va suferi conversia"
			},
			{
				name: "locuri",
				description: "ieste numărul de caractere de utilizat"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "Se efectuează conversia unui număr binar într-un octal.",
		arguments: [
			{
				name: "număr",
				description: "este numărul binar care va suferi conversia"
			},
			{
				name: "locuri",
				description: "este numărul de caractere de utilizat"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "Returnează probabilitatea distribuției binomiale pentru o variabilă discretă.",
		arguments: [
			{
				name: "număr_s",
				description: "este numărul de succese din încercări"
			},
			{
				name: "încercări",
				description: "este numărul de încercări independente"
			},
			{
				name: "probabilitate_s",
				description: "este probabilitatea de succes pentru fiecare încercare"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică: pentru funcția distribuție cumulativă, se utilizează TRUE; pentru funcția masă de probabilitate, se utilizează FALSE"
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "Returnează probabilitatea ca un rezultat de încercare să utilizeze o distribuție binomială.",
		arguments: [
			{
				name: "încercări",
				description: "este numărul de încercări independente"
			},
			{
				name: "probabilitate_s",
				description: "este probabilitatea de succes pentru fiecare încercare"
			},
			{
				name: "număr_s",
				description: "este numărul de reușite ale încercărilor"
			},
			{
				name: "număr_s2",
				description: "dacă este furnizată, această funcție returnează probabilitatea ca numărul de încercări reușite să fie între număr_s și număr_s2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Returnează valoarea cea mai mică pentru care distribuția binomială cumulativă este mai mare sau egală cu o valoare criteriu.",
		arguments: [
			{
				name: "încercări",
				description: "este numărul de încercări Bernoulli"
			},
			{
				name: "probabilitate_s",
				description: "este probabilitatea de succes în fiecare încercare, un număr între 0 și 1 inclusiv"
			},
			{
				name: "alfa",
				description: "este valoarea criteriu, un număr între 0 și 1 inclusiv"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "Returnează probabilitatea distribuției binomiale pentru o variabilă discretă.",
		arguments: [
			{
				name: "număr_s",
				description: "este numărul de succese din încercări"
			},
			{
				name: "încercări",
				description: "este numărul de încercări independente"
			},
			{
				name: "probabilitate_s",
				description: "este probabilitatea de succes pentru fiecare încercare"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică: pentru funcția distribuție cumulativă, se utilizează TRUE; pentru funcția de probabilitate de masă, se utilizează FALSE"
			}
		]
	},
	{
		name: "BITAND",
		description: "Returnează un „And” de nivel de bit din două numere.",
		arguments: [
			{
				name: "număr1",
				description: "este reprezentarea zecimalei numărului binar pe care doriți să-l evaluați"
			},
			{
				name: "număr2",
				description: "este reprezentarea zecimalei numărului binar pe care doriți să-l evaluați"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "Returnează un număr deplasat la stânga la volum_deplasare biți.",
		arguments: [
			{
				name: "număr",
				description: "este reprezentarea zecimalei numărului binar pe care doriți să-l evaluați"
			},
			{
				name: "volum_deplasare",
				description: "este numărul de biți la care doriți să deplasați numărul la stânga"
			}
		]
	},
	{
		name: "BITOR",
		description: "Returnează un „Or” de nivel de bit din două numere.",
		arguments: [
			{
				name: "număr1",
				description: "este reprezentarea zecimalei numărului binar pe care doriți să-l evaluați"
			},
			{
				name: "număr2",
				description: "este reprezentarea zecimalei numărului binar pe care doriți să-l evaluați"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "Returnează un număr deplasat la dreapta la volum_deplasare biți.",
		arguments: [
			{
				name: "număr",
				description: "este reprezentarea zecimalei numărului binar pe care doriți să-l evaluați"
			},
			{
				name: "volum_deplasare",
				description: "este numărul de biți la care doriți să deplasați numărul la dreapta"
			}
		]
	},
	{
		name: "BITXOR",
		description: "Returnează un „Exclusive Or” de nivel de bit din două numere.",
		arguments: [
			{
				name: "număr1",
				description: "este reprezentarea zecimalei numărului binar pe care doriți să-l evaluați"
			},
			{
				name: "număr2",
				description: "este reprezentarea zecimalei numărului binar pe care doriți să-l evaluați"
			}
		]
	},
	{
		name: "CEILING",
		description: "Rotunjește prin adaos un număr, la cel mai apropiat multiplu de argument de precizie.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea de rotunjit"
			},
			{
				name: "semnificație",
				description: "este multiplul la care se face rotunjirea"
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "Rotunjește prin adaos un număr, la cel mai apropiat întreg sau la cel mai apropiat multiplu de semnificație.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea de rotunjit"
			},
			{
				name: "semnificație",
				description: "este multiplul la care se face rotunjirea"
			},
			{
				name: "mod",
				description: "atunci când este dat și non-zero, această funcție se va rotunji de la zero"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "Rotunjește prin adaos un număr, la cel mai apropiat întreg sau la cel mai apropiat multiplu de semnificație.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea de rotunjit"
			},
			{
				name: "semnificație",
				description: "este multiplul la care se face rotunjirea"
			}
		]
	},
	{
		name: "CELL",
		description: "Returnează informații privind formatarea, locația sau conținutul primei celule, conform ordinii de citire a foii, într-o referință.",
		arguments: [
			{
				name: "info_tip",
				description: "este o valoare text care specifică tipul de informații dorit."
			},
			{
				name: "referință",
				description: "este celula pentru care se cer informații"
			}
		]
	},
	{
		name: "CHAR",
		description: "Returnează caracterul specificat de numărul codului din setul de caractere al computerului.",
		arguments: [
			{
				name: "număr",
				description: "este un număr între 1 și 255 specificând caracterul"
			}
		]
	},
	{
		name: "CHIDIST",
		description: "Returnează probabilitatea distribuției hi-pătrat unilaterale dreapta.",
		arguments: [
			{
				name: "x",
				description: "este valoarea la care se evaluează distribuția, un număr nenegativ"
			},
			{
				name: "grade_libertate",
				description: "este numărul de grade de libertate, un număr între 1 și 10^10, excluzând 10^10"
			}
		]
	},
	{
		name: "CHIINV",
		description: "Returnează inversa probabilității distribuției hi-pătrat unilaterale dreapta.",
		arguments: [
			{
				name: "probabilitate",
				description: "este o probabilitate asociată cu distribuția hi-pătrat, o valoare între 0 și 1 inclusiv"
			},
			{
				name: "grade_libertate",
				description: "este numărul de grade de libertate, un număr între 1 și 10^10, excluzând 10^10"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "Returnează probabilitatea distribuției Hi-pătrat a cozii din stânga.",
		arguments: [
			{
				name: "x",
				description: "este valoarea în care se evaluează distribuția, un număr nenegativ"
			},
			{
				name: "grade_libertate",
				description: "este numărul de grade de libertate, un număr între 1 și 10^10, excluzând 10^10"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică pentru ca funcția să se întoarcă: funcția de distribuție cumulativă = TRUE; funcția de densitate a probabilității = FALSE"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "Returnează probabilitatea distribuției Hi-pătrat a cozii din dreapta.",
		arguments: [
			{
				name: "x",
				description: "este valoarea în care se evaluează distribuția, un număr nenegativ"
			},
			{
				name: "grade_libertate",
				description: "este numărul de grade de libertate, un număr între 1 și 10^10, excluzând 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "Returnează inversa probabilității distribuției Hi-pătrat la stânga.",
		arguments: [
			{
				name: "probabilitate",
				description: "este o probabilitate asociată cu distribuția Hi-pătrat, o valoare între 0 și 1 inclusiv"
			},
			{
				name: "grade_libertate",
				description: "este numărul de grade de libertate, un număr între 1 și 10^10, excluzând 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "Returnează inversa probabilității distribuției Hi-pătrat la dreapta.",
		arguments: [
			{
				name: "probabilitate",
				description: "este o probabilitate asociată cu distribuția Hi-pătrat, o valoare între 0 și 1 inclusiv"
			},
			{
				name: "grade_libertate",
				description: "este numărul de grade de libertate, un număr între 1 și 10^10, excluzând 10^10"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "Returnează testul de independență: valoarea din distribuția Hi-pătrat pentru statistică și gradele de libertate corespunzătoare.",
		arguments: [
			{
				name: "zonă_actuale",
				description: "este zona de date ce conține observațiile la test față de valorile așteptate"
			},
			{
				name: "zonă_așteptate",
				description: "este zona de date ce conține raportul dintre produsul totalurilor pe rând și totalurilor pe coloane, și totalul general"
			}
		]
	},
	{
		name: "CHITEST",
		description: "Returnează testul de independență: valoarea din distribuția hi-pătrat pentru statistică și gradele de libertate corespunzătoare.",
		arguments: [
			{
				name: "zonă_actuale",
				description: "este zona de date ce conține observațiile la test față de valorile așteptate"
			},
			{
				name: "zonă_așteptate",
				description: "este zona de date ce conține raportul dintre produsul totalurilor pe rând și totalurilor pe coloane, și totalul general"
			}
		]
	},
	{
		name: "CHOOSE",
		description: "Alege o valoare sau acțiune de efectuat dintr-o listă de valori, bazată pe un număr index.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index_num",
				description: "specifică argumentul valoare care se selectează. Index_num trebuie să fie între 1 și 254 sau o formulă sau o referință la un număr între 1 și 254"
			},
			{
				name: "valoare1",
				description: "sunt de la 1 la 254 numere, referințe de celule, nume definite, formule, funcții sau argumente text din care selectează funcția CHOOSE"
			},
			{
				name: "valoare2",
				description: "sunt de la 1 la 254 numere, referințe de celule, nume definite, formule, funcții sau argumente text din care selectează funcția CHOOSE"
			}
		]
	},
	{
		name: "CLEAN",
		description: "Elimină toate caracterele neimprimabile din text.",
		arguments: [
			{
				name: "text",
				description: "este orice informație privind foaia de lucru din care se elimină caracterele neimprimabile"
			}
		]
	},
	{
		name: "CODE",
		description: "Returnează un cod numeric pentru primul caracter dintr-un șir text, în setul de caractere utilizat de computer.",
		arguments: [
			{
				name: "text",
				description: "este textul pentru care rezultă codul primului caracter"
			}
		]
	},
	{
		name: "COLUMN",
		description: "Returnează numărul coloanei pentru o referință.",
		arguments: [
			{
				name: "referință",
				description: "este celula sau zona de celule contigue pentru care se calculează numărul coloanei. Dacă este omisă, se utilizează celula care conține funcția COLOANĂ"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "Returnează numărul de coloane dintr-o matrice sau referință.",
		arguments: [
			{
				name: "matrice",
				description: "este o matrice sau o formulă matrice, sau o referință la o zonă de celule pentru care se calculează numărul de coloane"
			}
		]
	},
	{
		name: "COMBIN",
		description: "Returnează numărul de combinări pentru un număr dat de elemente.",
		arguments: [
			{
				name: "număr",
				description: "este numărul total de elemente"
			},
			{
				name: "număr_ales",
				description: "este numărul de elemente din fiecare combinare"
			}
		]
	},
	{
		name: "COMBINA",
		description: "Returnează numărul de combinări cu repetiții pentru un număr dat de elemente.",
		arguments: [
			{
				name: "număr",
				description: "este numărul total de elemente"
			},
			{
				name: "număr_ales",
				description: "este numărul de elemente din fiecare combinare"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "Se efectuează conversia coeficienților reali și imaginari într-un număr complex.",
		arguments: [
			{
				name: "num_real",
				description: "este coeficientul real al numărului complex"
			},
			{
				name: "num_i",
				description: "este coeficientul imaginar al numărului complex"
			},
			{
				name: "sufix",
				description: "este sufixul componentei imaginare a numărului complex"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "Unește mai multe șiruri text într-un singur șir text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "text1",
				description: "sunt de la 1 la 255 de șiruri text care se unesc într-un singur șir text și care pot fi șiruri text, numere sau referințe la o singură celulă"
			},
			{
				name: "text2",
				description: "sunt de la 1 la 255 de șiruri text care se unesc într-un singur șir text și care pot fi șiruri text, numere sau referințe la o singură celulă"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "Returnează intervalul de încredere pentru o medie a populației, utilizând o distribuție normală.",
		arguments: [
			{
				name: "alfa",
				description: "este nivelul de semnificație utilizat pentru a calcula nivelul de încredere, un număr mai mare decât 0 și mai mic decât 1"
			},
			{
				name: "dev_standard",
				description: "este deviația standard a populației pentru zona de date și se presupune cunoscută. dev_standard trebuie să fie mai mare decât 0"
			},
			{
				name: "dimens",
				description: "este dimensiunea eșantionului"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "Returnează intervalul de încredere pentru o medie a populației, utilizând o distribuție normală.",
		arguments: [
			{
				name: "alfa",
				description: "este nivelul de semnificație utilizat pentru a calcula nivelul de încredere, un număr mai mare decât 0 și mai mic decât 1"
			},
			{
				name: "dev_standard",
				description: "este deviația standard a populației pentru zona de date și se presupune cunoscută. Deviația standard trebuie să fie mai mare decât 0"
			},
			{
				name: "dimensiune",
				description: "este dimensiunea eșantionului"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "Returnează intervalul de încredere pentru o medie a populației, utilizând o distribuție t-student.",
		arguments: [
			{
				name: "alfa",
				description: "este nivelul de semnificație utilizat pentru a calcula nivelul de încredere, un număr mai mare decât 0 și mai mic decât 1"
			},
			{
				name: "dev_standard",
				description: "este deviația standard a populației pentru zona de date și se presupune cunoscută. dev_standard trebuie să fie mai mare decât 0"
			},
			{
				name: "dimensiune",
				description: "este dimensiunea eșantionului"
			}
		]
	},
	{
		name: "CONVERT",
		description: "Se efectuează conversia unui număr de la un sistem de măsurare la altul.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea în din_unități asupra cărora se va efectua conversia"
			},
			{
				name: "din_unit",
				description: "unitățile pentru număr"
			},
			{
				name: "la_unit",
				description: "unitățile pentru rezultat"
			}
		]
	},
	{
		name: "CORREL",
		description: "Returnează coeficientul de corelație dintre două seturi de date.",
		arguments: [
			{
				name: "matrice1",
				description: "este o zonă de celule de valori. Valorile sunt numere, nume, matrice, sau referințe ce conțin numere"
			},
			{
				name: "matrice2",
				description: "este a doua zonă de celule de valori. Valorile sunt numere, nume, matrice, sau referințe ce conțin numere"
			}
		]
	},
	{
		name: "COS",
		description: "Returnează cosinusul unui unghi.",
		arguments: [
			{
				name: "număr",
				description: "este unghiul în radiani pentru care se calculează cosinusul"
			}
		]
	},
	{
		name: "COSH",
		description: "Returnează cosinusul hiperbolic al unui număr.",
		arguments: [
			{
				name: "număr",
				description: "este orice număr real"
			}
		]
	},
	{
		name: "COT",
		description: "Returnează cotangenta unui unghi.",
		arguments: [
			{
				name: "număr",
				description: "este unghiul în radiani pentru care se calculează cotangenta"
			}
		]
	},
	{
		name: "COTH",
		description: "Returnează cotangenta hiperbolică a unui număr.",
		arguments: [
			{
				name: "număr",
				description: "este unghiul în radiani pentru care se calculează cotangenta hiperbolică"
			}
		]
	},
	{
		name: "COUNT",
		description: "Numără câte celule dintr-un interval conțin numere.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valoare1",
				description: "sunt de la 1 la 255 de argumente care conțin sau care se referă la o varietate de tipuri diferite de date, dar numai numerele sunt luate în considerare"
			},
			{
				name: "valoare2",
				description: "sunt de la 1 la 255 de argumente care conțin sau care se referă la o varietate de tipuri diferite de date, dar numai numerele sunt luate în considerare"
			}
		]
	},
	{
		name: "COUNTA",
		description: "Numără celulele dintr-un interval care nu sunt goale.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valoare1",
				description: "sunt de la 1 la 255 de argumente care reprezintă valorile și celulele de numărat. Valorile pot fi orice tip de informație"
			},
			{
				name: "valoare2",
				description: "sunt de la 1 la 255 de argumente care reprezintă valorile și celulele de numărat. Valorile pot fi orice tip de informație"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "Numără celulele goale dintr-o zonă precizată de celule.",
		arguments: [
			{
				name: "zonă",
				description: "este zona din care se numără celulele goale"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "Numără celulele dintr-o zonă care îndeplinesc condiția dată.",
		arguments: [
			{
				name: "zonă",
				description: "este zona de celule din care se numără celulele completate"
			},
			{
				name: "criterii",
				description: "este condiția de forma unui număr, expresie sau text care definește celulele de numărat"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "Contorizează numărul de celule specificat într-un set dat de condiții sau criterii.",
		arguments: [
			{
				name: "zonă_criterii",
				description: "este intervalul de celule de evaluat pentru o anumită stare"
			},
			{
				name: "criterii",
				description: "este condiția sub forma unui număr, a unei expresii sau a unui text care definește care celule se vor contoriza"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "Returnează numărul de zile de la începutul perioadei de cupon la data de lichidare.",
		arguments: [
			{
				name: "tranzacție",
				description: "este data de lichidare a asigurării, exprimată ca număr serial"
			},
			{
				name: "maturitate",
				description: "este data de maturitate a asigurării, exprimată ca număr serial"
			},
			{
				name: "frecvență",
				description: "este numărul de plăți pe cupon pe an"
			},
			{
				name: "bază",
				description: "este tipul de bază de numărare a zilelor care trebuie utilizat"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "Returnează următoarea dată de pe cupon după data de stabilire a asigurării.",
		arguments: [
			{
				name: "tranzacție",
				description: "este data de stabilire a asigurării exprimată ca număr serial"
			},
			{
				name: "maturitate",
				description: "este data de maturitate a asigurării, exprimată ca număr serial"
			},
			{
				name: "frecvență",
				description: "este numărul de plăți pe cupon dintr-un an"
			},
			{
				name: "bază",
				description: "este tipul de bază de numărare a zilelor care trebuie utilizat"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "Returnează numărul de cupoane de plată între data de stabilire a asigurării și data de maturitate.",
		arguments: [
			{
				name: "tranzacție",
				description: "este data de stabilire a asigurării, exprimată ca număr de dată serial"
			},
			{
				name: "maturitate",
				description: "este data de stabilire a asigurării, exprimată ca număr serial"
			},
			{
				name: "frecvență",
				description: "este numărul de plăți pe cupon dintr-un an"
			},
			{
				name: "bază",
				description: "este tipul de bază de numărare a zilelor care trebuie utilizat"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "Returnează data de pe cuponul precedent înainte de data de stabilire a asigurării.",
		arguments: [
			{
				name: "tranzacție",
				description: "este data de stabilire a asigurării, exprimată ca număr serial"
			},
			{
				name: "maturitate",
				description: "este data de maturitate a asigurării, exprimată ca număr serial"
			},
			{
				name: "frecvență",
				description: "este numărul de plăți pe cupon dintr-un an"
			},
			{
				name: "bază",
				description: "este tipul de bază de numărare a zilelor care trebuie utilizat"
			}
		]
	},
	{
		name: "COVAR",
		description: "Returnează covarianța, media produselor deviațiilor pentru fiecare pereche de puncte de date din două seturi de date.",
		arguments: [
			{
				name: "matrice1",
				description: "este prima zonă de celule de întregi și trebuie să fie numere, matrice sau referințe ce conțin numere"
			},
			{
				name: "matrice2",
				description: "este a doua zonă de celule de întregi și trebuie să fie numere, matrice sau referințe ce conțin numere"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "Returnează covarianța populației, media produselor deviațiilor pentru fiecare pereche de puncte din două seturi de date.",
		arguments: [
			{
				name: "matrice1",
				description: "este prima zonă de celule de întregi și trebuie să fie numere, matrice, sau referințe ce conțin numere"
			},
			{
				name: "matrice2",
				description: "este a doua zonă de celule de întregi și trebuie să fie numere, matrice, sau referințe ce conțin numere"
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "Returnează covarianța eșantion, media produselor deviațiilor pentru fiecare pereche de puncte din două seturi de date.",
		arguments: [
			{
				name: "matrice1",
				description: "este prima zonă de celule de întregi și trebuie să fie numere, matrice, sau referințe ce conțin numere"
			},
			{
				name: "matrice2",
				description: "este a doua zonă de celule de întregi și trebuie să fie numere, matrice, sau referințe ce conțin numere"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "Returnează valoarea cea mai mică pentru care distribuția binomială cumulativă este mai mare sau egală cu o valoare criteriu.",
		arguments: [
			{
				name: "încercări",
				description: "este numărul de încercări Bernoulli"
			},
			{
				name: "probabilitate_s",
				description: "este probabilitatea de succes în fiecare încercare, un număr între 0 și 1 inclusiv"
			},
			{
				name: "alfa",
				description: "este valoarea criteriu, un număr între 0 și 1 inclusiv"
			}
		]
	},
	{
		name: "CSC",
		description: "Returnează cosecanta unui unghi.",
		arguments: [
			{
				name: "număr",
				description: "este unghiul în radiani pentru care se calculează cosecanta"
			}
		]
	},
	{
		name: "CSCH",
		description: "Returnează cosecanta hiperbolică a unui unghi.",
		arguments: [
			{
				name: "număr",
				description: "este unghiul în radiani pentru care se calculează cosecanta hiperbolică"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "Returnează dobânda cumulativă plătită între două perioade.",
		arguments: [
			{
				name: "rată",
				description: "este rata dobânzii"
			},
			{
				name: "nper",
				description: "este numărul total de perioade de plată"
			},
			{
				name: "pv",
				description: "este valoarea prezentă"
			},
			{
				name: "per_start",
				description: "este prima perioadă din calcul"
			},
			{
				name: "per_ultima",
				description: "este ultima perioadă din calcul"
			},
			{
				name: "tip",
				description: "este temporizarea plății"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "Returnează suma cumulativă plătită dintr-un împrumut între două date.",
		arguments: [
			{
				name: "rată",
				description: "este rata dobânzii"
			},
			{
				name: "nper",
				description: "este numărul total de perioade de plată"
			},
			{
				name: "pv",
				description: "este valoarea prezentă"
			},
			{
				name: "per_start",
				description: "este prima perioadă din calcul"
			},
			{
				name: "per_ultima",
				description: "este ultima perioadă din calcul"
			},
			{
				name: "tip",
				description: "este temporizarea plății"
			}
		]
	},
	{
		name: "DATE",
		description: "Returnează numărul care reprezintă data în cod dată-oră Spreadsheet.",
		arguments: [
			{
				name: "an",
				description: "este un număr de la 1900 la 9999 în Spreadsheet pentru Windows sau de la 1904 la 9999 în Spreadsheet pentru Macintosh"
			},
			{
				name: "lună",
				description: "este un număr de la 1 la 12 reprezentând luna din an"
			},
			{
				name: "zi",
				description: "este un număr de la 1 la 31 reprezentând ziua din lună"
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
		description: "Transformă o dată în forma de text într-un număr care reprezintă data în cod dată-oră Spreadsheet.",
		arguments: [
			{
				name: "text_dată",
				description: "este textul care reprezintă o dată într-un format de dată Spreadsheet, între 1.1.1900 (Windows) sau 1.1.1904 (Macintosh) și 31.12.9999"
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "Face media valorilor dintr-o coloană dintr-o listă sau bază de date care corespund condițiilor precizate.",
		arguments: [
			{
				name: "bază_de_date",
				description: "este zona de celule care compun lista sau baza de date. O bază de date este o listă de înregistrări înrudite"
			},
			{
				name: "câmp",
				description: "este fie eticheta unei coloane în ghilimele duble, fie un număr care reprezintă poziția coloanei în listă"
			},
			{
				name: "criterii",
				description: "este zona de celule care conține condițiile  precizate. Zona include o etichetă de coloană și o celulă sub etichetă pentru o condiție"
			}
		]
	},
	{
		name: "DAY",
		description: "Returnează ziua din lună, un număr de la 1 la 31.",
		arguments: [
			{
				name: "număr_serie",
				description: "este un număr în cod dată-oră utilizat de Spreadsheet"
			}
		]
	},
	{
		name: "DAYS",
		description: "Returnează numărul de zile dintre cele două date.",
		arguments: [
			{
				name: "dată_sfârșit",
				description: "dată_început și dată_sfârșit sunt cele două date între care doriți să cunoașteți numărul de zile"
			},
			{
				name: "dată_început",
				description: "dată_început și dată_sfârșit sunt cele două date între care doriți să cunoașteți numărul de zile"
			}
		]
	},
	{
		name: "DAYS360",
		description: "Returnează numărul de zile dintre două date bazat pe un an de 360 de zile (12 luni de 30 de zile).",
		arguments: [
			{
				name: "dată_start",
				description: "dată_start și dată_sfârșit sunt cele două date între care se calculează numărul de zile"
			},
			{
				name: "dată_sfârșit",
				description: "dată_start și dată_sfârșit sunt cele două date între care se calculează numărul de zile"
			},
			{
				name: "metodă",
				description: "este o valoare logică și specifică metoda de calcul: S.U.A. (NASD) = FALSE sau omisă; European = TRUE."
			}
		]
	},
	{
		name: "DB",
		description: "Returnează amortizarea unui mijloc fix pentru o perioadă specificată utilizând metoda balanței cu amortizare fixă.",
		arguments: [
			{
				name: "cost",
				description: "este costul inițial al mijlocului fix"
			},
			{
				name: "recuperat",
				description: "este valoarea rămasă la sfârșitul duratei de viață a mijlocului fix"
			},
			{
				name: "viață",
				description: "este numărul de perioade în care se amortizează mijlocul fix (uneori este numită durata de utilizare a mijlocului fix)"
			},
			{
				name: "perioadă",
				description: "este perioada pentru care se calculează amortizarea. Perioada se măsoară în aceleași unități ca viață"
			},
			{
				name: "lună",
				description: "este numărul de luni din primul an. Dacă luna este omisă, se presupune că este 12"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "Numără celulele câmpului (coloanei) care conțin numere. Numărarea se face pentru înregistrările bazei de date care corespund condițiilor specificate.",
		arguments: [
			{
				name: "bază_de_date",
				description: "este zona de celule care compune lista sau baza de date. O bază de date este o listă de date înrudite"
			},
			{
				name: "câmp",
				description: "este fie eticheta unei coloane în ghilimele duble fie un număr care reprezintă poziția coloanei în listă"
			},
			{
				name: "criterii",
				description: "este zona de celule care conține condițiile specificate. Zona include o etichetă de coloană și o celulă sub etichetă pentru o condiție"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "Numără celulele completate în câmp (coloană) pentru înregistrările din baza de date care corespund condițiilor specificate.",
		arguments: [
			{
				name: "bază_de_date",
				description: "este zona de celule care compun lista sau baza de date. O bază de date este o listă de date înrudite"
			},
			{
				name: "câmp",
				description: "este fie eticheta coloanei în ghilimele duble sau un număr care reprezintă poziția coloanei în listă"
			},
			{
				name: "criterii",
				description: "este zona de celule care conține condițiile specificate. Zona include o etichetă de coloană și o celulă sub etichetă pentru o condiție"
			}
		]
	},
	{
		name: "DDB",
		description: "Returnează amortizarea unui mijloc fix pentru o perioadă specificată utilizând metoda balanței amortizare dublă sau altă metodă specificată.",
		arguments: [
			{
				name: "cost",
				description: "este costul inițial al mijlocului fix"
			},
			{
				name: "val_reziduală",
				description: "este valoarea rămasă la sfârșitul duratei de viață a mijlocului fix"
			},
			{
				name: "viață",
				description: "este numărul de perioade în care este amortizat mijlocul fix (uneori numită durata de utilizare)"
			},
			{
				name: "perioadă",
				description: "este perioada pentru care se calculează amortizarea. Perioada trebuie să fie măsurată în aceleași unități de măsură ca viață"
			},
			{
				name: "factor",
				description: "este rata cu care scade balanța. Dacă Factor este omis, se presupune că este 2 (metoda balanței cu dublă amortizare)"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "Se efectuează conversia unui număr zecimal într-un număr binar.",
		arguments: [
			{
				name: "număr",
				description: "este numărul întreg zecimal care va suferi conversia"
			},
			{
				name: "locuri",
				description: "este numărul de caractere de utilizat"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "Se efectuează conversia unui număr zecimal într-un număr hexazecimal.",
		arguments: [
			{
				name: "număr",
				description: "este numărul întreg zecimal care va suferi conversia"
			},
			{
				name: "locuri",
				description: "este numărul de caractere de utilizat"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "Se efectuează conversia unui număr zecimal într-un octal.",
		arguments: [
			{
				name: "număr",
				description: "este numărul întreg zecimal care va suferi conversia"
			},
			{
				name: "locuri",
				description: "este numărul de caractere de utilizat"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Efectuează conversia unei reprezentări text a unui număr într-o bază dată în număr cu zecimale.",
		arguments: [
			{
				name: "număr",
				description: "este numărul căruia îi efectuați conversia"
			},
			{
				name: "bază",
				description: "este baza numărului căruia îi efectuați conversia"
			}
		]
	},
	{
		name: "DEGREES",
		description: "Transformă radiani în grade.",
		arguments: [
			{
				name: "unghi",
				description: "este unghiul de transformat exprimat în radiani"
			}
		]
	},
	{
		name: "DELTA",
		description: "Se testează dacă două numere sunt egale.",
		arguments: [
			{
				name: "număr1",
				description: "este primul număr"
			},
			{
				name: "număr2",
				description: "este al doilea număr"
			}
		]
	},
	{
		name: "DEVSQ",
		description: "Returnează suma pătratelor deviațiilor punctelor de date față de media eșantionului.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de argumente sau o matrice sau o referință la o matrice pentru care se calculează DEVSQ"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de argumente sau o matrice sau o referință la o matrice pentru care se calculează DEVSQ"
			}
		]
	},
	{
		name: "DGET",
		description: "Extrage dintr-o bază de date o singură înregistrare care se potrivește condițiilor specificate.",
		arguments: [
			{
				name: "bază_de_date",
				description: "este zona de celule care compun lista sau baza de date. O bază de date este o listă de date înrudite"
			},
			{
				name: "câmp",
				description: "este fie eticheta unei coloane în ghilimele duble sau un număr care reprezintă poziția coloanei în listă"
			},
			{
				name: "criterii",
				description: "este zona de celule care conțin condițiile specificate. Zona include o etichetă de coloană și o celulă sub etichetă pentru o condiție"
			}
		]
	},
	{
		name: "DISC",
		description: "Returnează rata de reducere pentru o asigurare.",
		arguments: [
			{
				name: "tranzacție",
				description: "este data de lichidare a asigurării, exprimată ca număr serial"
			},
			{
				name: "maturitate",
				description: "este data maturizării asigurării, exprimată ca număr serial"
			},
			{
				name: "pr",
				description: "este prețul asigurării la 100 lei valoare reală"
			},
			{
				name: "rambursare",
				description: "este valoarea de recuperare a asigurării la 100 lei valoare reală"
			},
			{
				name: "bază",
				description: "este tipul de bază de numărare a zilelor care trebuie utilizat"
			}
		]
	},
	{
		name: "DMAX",
		description: "Returnează cel mai mare număr din câmpul (coloana) de înregistrări din baza de date care corespund condițiilor precizate.",
		arguments: [
			{
				name: "bază_de_date",
				description: "este zona de celule care compun lista sau baza de date. O bază de date este o listă de date înrudite"
			},
			{
				name: "câmp",
				description: "este fie eticheta coloanei în ghilimele duble, fie un număr care reprezintă poziția coloanei în listă"
			},
			{
				name: "criterii",
				description: "este zona de celule care conține condițiile precizate. Zona include o etichetă de coloană și o celulă sub etichetă pentru o condiție"
			}
		]
	},
	{
		name: "DMIN",
		description: "Returnează în câmp (coloană) cel mai mic număr din înregistrările din baza de date care corespund condițiilor specificate.",
		arguments: [
			{
				name: "bază_de_date",
				description: "este zona de celule care compun lista sau baza de date. O bază de date este o listă de date înrudite"
			},
			{
				name: "câmp",
				description: "este fie eticheta coloanei în ghilimele duble fie un număr care reprezintă poziția coloanei în listă"
			},
			{
				name: "criterii",
				description: "este zona de celule care conține condițiile specificate. Zona include o etichetă de coloană și o celulă sub etichetă pentru o condiție"
			}
		]
	},
	{
		name: "DOLLAR",
		description: "Transformă un număr în text, utilizând formatul monedă.",
		arguments: [
			{
				name: "număr",
				description: "este un număr, o referință la o celulă care conține un număr, sau o formulă care evaluează numere"
			},
			{
				name: "zecimale",
				description: "este numărul de cifre de la dreapta separatorului zecimal. Numărul este rotunjit dacă este necesar; dacă este omis, zecimale = 2"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "Se efectuează conversia unui preț în dolari, exprimat ca fracție, într-un preț în dolari, exprimat ca număr zecimal.",
		arguments: [
			{
				name: "preț_fracție",
				description: "este un număr exprimat ca fracție"
			},
			{
				name: "fracție",
				description: "este numărul întreg de utilizat ca numitor al fracției"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "Se efectuează conversia unui preț în dolari, exprimat ca număr zecimal, într-un preț în dolari, exprimat ca fracție.",
		arguments: [
			{
				name: "preț_zecimal",
				description: "este un număr zecimal"
			},
			{
				name: "fracție",
				description: "este numărul întreg de utilizat ca numitor al fracției"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "Înmulțește valorile câmpului (coloanei) pentru înregistrările bazei de date care corespund condițiilor specificate.",
		arguments: [
			{
				name: "bază_de_date",
				description: "este zona de celule care compun lista sau baza de date. O bază de date este o listă de date înrudite"
			},
			{
				name: "câmp",
				description: "este fie eticheta coloanei în ghilimele duble fie un număr care reprezintă poziția coloanei în listă"
			},
			{
				name: "criterii",
				description: "este zona de celule care conține condiția specificată. Zona include o etichetă de coloană și o celulă sub etichetă pentru o condiție"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "Estimează deviația standard bazată pe un eșantion din intrările selectate din baza de date.",
		arguments: [
			{
				name: "bază_de_date",
				description: "este zona de celule care compun lista sau baza de date. O bază de date este o listă de date înrudite"
			},
			{
				name: "câmp",
				description: "este fie eticheta unei coloane în ghilimele duble fie un număr care reprezintă poziția coloanei în listă"
			},
			{
				name: "criterii",
				description: "este zona de celule care conține condițiile specificate. Zona include o etichetă de coloană și o celulă sub etichetă pentru o condiție"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "Calculează deviația standard bazată pe întreaga populație a intrărilor selectate din baza de date.",
		arguments: [
			{
				name: "bază_de_date",
				description: "este zona de celule care compun lista sau baza de date. O bază de date este o listă de date înrudite"
			},
			{
				name: "câmp",
				description: "este fie eticheta unei coloane în ghilimele duble fie un număr care reprezintă poziția coloanei în listă"
			},
			{
				name: "criterii",
				description: "este zona de celule care conține condițiile specificate. Zona include o etichetă de coloană și o celulă sub etichetă pentru o condiție"
			}
		]
	},
	{
		name: "DSUM",
		description: "Adună în câmp (coloană) numerele din înregistrările din baza de date care corespund condițiilor specificate.",
		arguments: [
			{
				name: "bază_de_date",
				description: "este zona de celule care compun lista sau baza de date. O bază de date este o listă de date înrudite"
			},
			{
				name: "câmp",
				description: "este fie eticheta unei coloane în ghilimele duble fie un număr care reprezintă poziția coloanei în listă"
			},
			{
				name: "criterii",
				description: "este zona de celule care conține condițiile specificate. Zona include o etichetă de coloană și o celulă sub etichetă pentru o condiție"
			}
		]
	},
	{
		name: "DVAR",
		description: "Estimează varianța bazată pe un eșantion din intrările selectate ale bazei de date.",
		arguments: [
			{
				name: "bază_de_date",
				description: "este zona de celule care compun lista sau baza de date. O bază de date este o listă de date înrudite"
			},
			{
				name: "câmp",
				description: "este fie eticheta coloanei în ghilimele duble fie un număr care reprezintă poziția coloanei în listă"
			},
			{
				name: "criterii",
				description: "este zona de celule care conține condițiile specificate. Zona include o etichetă de coloană și o celulă sub etichetă pentru o condiție"
			}
		]
	},
	{
		name: "DVARP",
		description: "Calculează varianța bazată pe întreaga populație a intrărilor selectate din baza de date.",
		arguments: [
			{
				name: "bază_de_date",
				description: "este zona de celule care compun lista sau baza de date. O bază de date este o listă de date înrudite"
			},
			{
				name: "câmp",
				description: "este fie eticheta unei coloane în ghilimele duble fie un număr care reprezintă poziția coloanei în listă"
			},
			{
				name: "criterii",
				description: "este zona de celule care conține condițiile specificate. Zona include o etichetă de coloană și o celulă sub etichetă pentru o condiție"
			}
		]
	},
	{
		name: "EDATE",
		description: "Returnează numărul serial al datei care este numărul indicat de luni înainte sau după data de început.",
		arguments: [
			{
				name: "dată_start",
				description: "este numărul serial care reprezintă data de început"
			},
			{
				name: "luni",
				description: "este numărul de luni înainte sau după data_de început"
			}
		]
	},
	{
		name: "EFFECT",
		description: "Returnează rata anuală efectivă a dobânzii.",
		arguments: [
			{
				name: "rată_nominală",
				description: "este rata dobânzii nominală"
			},
			{
				name: "n_per",
				description: "este numărul de perioade compuse per an"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "Returnează un șir codificat URL.",
		arguments: [
			{
				name: "text",
				description: "este un șir de codificat URL"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "Returnează numărul serial al ultimei zile din ultima lună înainte sau după un anumit număr de luni specificate.",
		arguments: [
			{
				name: "dată_start",
				description: "este numărul serial care reprezintă data de început"
			},
			{
				name: "luni",
				description: "este numărul de luni înainte sau după data_de început"
			}
		]
	},
	{
		name: "ERF",
		description: "Returnează funcția de eroare.",
		arguments: [
			{
				name: "limită_inf",
				description: "este limita inferioară pentru integrarea ERF"
			},
			{
				name: "limită_sup",
				description: "este limita superioară pentru integrarea ERF"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "Returnează funcția de eroare.",
		arguments: [
			{
				name: "X",
				description: "este o legătură inferioară pentru integrarea ERF.PRECISE"
			}
		]
	},
	{
		name: "ERFC",
		description: "Returnează funcția de eroare complementară.",
		arguments: [
			{
				name: "x",
				description: "este limita inferioară pentru integrarea ERF"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "Returnează funcția complementară de eroare.",
		arguments: [
			{
				name: "X",
				description: "este o legătură inferioară pentru integrarea ERFC.PRECISE"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "Returnează un număr care corespunde unei valori de eroare.",
		arguments: [
			{
				name: "val_er",
				description: "este valoarea de eroare pentru care se cere numărul de identificare și este o valoare de eroare sau o referință la o celulă care conține o valoare de eroare"
			}
		]
	},
	{
		name: "EVEN",
		description: "Rotunjește prin adaos un număr pozitiv și prin lipsă un număr negativ până la cel mai apropiat întreg par.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea de rotunjit"
			}
		]
	},
	{
		name: "EXACT",
		description: "Verifică dacă două șiruri text sunt identice și returnează TRUE sau FALSE. EXACT diferențiază literele mari de literele mici.",
		arguments: [
			{
				name: "text1",
				description: "este primul șir text"
			},
			{
				name: "text2",
				description: "este al doilea șir text"
			}
		]
	},
	{
		name: "EXP",
		description: "Returnează e ridicat la o putere dată.",
		arguments: [
			{
				name: "număr",
				description: "este exponentul aplicat bazei e. Constanta e este egală cu 2,71828182845904, baza logaritmului natural"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "Returnează distribuția exponențială.",
		arguments: [
			{
				name: "x",
				description: "este valoarea funcției, un număr nenegativ"
			},
			{
				name: "lambda",
				description: "este valoarea parametru, un număr pozitiv"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică pentru ceea ce returnează funcția: funcția distribuție cumulativă = TRUE; funcția densitatea probabilității = FALSE"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "Returnează distribuția exponențială.",
		arguments: [
			{
				name: "x",
				description: "este valoarea funcției, un număr nenegativ"
			},
			{
				name: "lambda",
				description: "este valoarea parametru, un număr pozitiv"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică de returnat de către funcție: funcția distribuție cumulativă = TRUE; funcția densitatea probabilității = FALSE"
			}
		]
	},
	{
		name: "F.DIST",
		description: "Returnează distribuția de probabilitate F (coada din stânga) (grad de diversitate) pentru două seturi de date.",
		arguments: [
			{
				name: "x",
				description: "este valoarea în care se evaluează  funcția, un număr nenegativ"
			},
			{
				name: "grade_libertate1",
				description: "este numărătorul gradelor de libertate, un număr între 1 și 10^10, exclusiv 10^10"
			},
			{
				name: "grade_libertate2",
				description: "este numitorul gradelor de libertate, un număr între 1 și 10^10, exclusiv 10^10"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică pentru ca funcția să returneze: funcția de distribuție cumulativă = TRUE; funcția de densitate a probabilității = FALSE"
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "Returnează distribuția de probabilitate F (coada din dreapta) (grad de diversitate) pentru două seturi de date.",
		arguments: [
			{
				name: "x",
				description: "este valoarea în care se evaluează  funcția, un număr nenegativ"
			},
			{
				name: "grade_libertate1",
				description: "este numărătorul gradelor de libertate, un număr între 1 și 10^10, exclusiv 10^10"
			},
			{
				name: "grade_libertate2",
				description: "este numitorul gradelor de libertate, un număr între 1 și 10^10, exclusiv 10^10"
			}
		]
	},
	{
		name: "F.INV",
		description: "Returnează inversa distribuției de probabilitate F (coada din stânga): dacă p = F.DIST(x;...), atunci F.INV(p;....) = x.",
		arguments: [
			{
				name: "probabilitate",
				description: "este o probabilitate asociată cu distribuția cumulativă F, un număr între 0 și 1 inclusiv"
			},
			{
				name: "grade_libertate1",
				description: "este numărătorul gradelor de libertate, un număr între 1 și 10^10, exclusiv 10^10"
			},
			{
				name: "grade_libertate2",
				description: "este numitorul gradelor de libertate, un număr între 1 și 10^10, exclusiv 10^10"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "Returnează inversa distribuției de probabilitate F (coada din dreapta): dacă p = F.DIST.RT(x;...), atunci F.INV.RT(p;....) = x.",
		arguments: [
			{
				name: "probabilitate",
				description: "este o probabilitate asociată cu distribuția cumulativă F, un număr între 0 și 1 inclusiv"
			},
			{
				name: "grade_libertate1",
				description: "este numărătorul gradelor de libertate, un număr între 1 și 10^10, exclusiv 10^10"
			},
			{
				name: "grade_libertate2",
				description: "este numitorul gradelor de libertate, un număr între 1 și 10^10, exclusiv 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Returnează rezultatul unui test F, probabilitatea bilaterală ca varianțele din Matrice1 și Matrice2 să nu fie semnificativ diferite.",
		arguments: [
			{
				name: "matrice1",
				description: "este prima matrice sau zonă de date și pot fi numere sau nume, matrice sau referințe care conțin numere (celulele necompletate se ignoră)"
			},
			{
				name: "matrice2",
				description: "este a doua matrice sau zonă de date și pot fi numere sau nume, matrice sau referințe care conțin numere (celulele necompletate se ignoră)"
			}
		]
	},
	{
		name: "FACT",
		description: "Returnează produsul factorial al unui număr, egal cu 1*2*3*...* număr.",
		arguments: [
			{
				name: "număr",
				description: "este numărul nenegativ pentru care se calculează produsul factorial"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "Returnează factorialul dublu al unui număr.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea pentru care se returnează factorialul dublu"
			}
		]
	},
	{
		name: "FALSE",
		description: "Returnează valoarea logică FALSE.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "Returnează distribuția de probabilitate F (unilaterale dreapta) (grad de diversitate) pentru două seturi de date.",
		arguments: [
			{
				name: "x",
				description: "este valoarea la care se evaluează funcția, un număr nenegativ"
			},
			{
				name: "grade_libertate1",
				description: "este numărătorul gradelor de libertate, un număr între 1 și 10^10, exclusiv 10^10"
			},
			{
				name: "grade_libertate2",
				description: "este numitorul gradelor de libertate, un număr între 1 și 10^10, exclusiv 10^10"
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
		description: "Returnează numărul poziției de început a unui șir text găsit în cadrul altui șir text. FIND diferențiază literele mari de literele mici.",
		arguments: [
			{
				name: "text_de_căutat",
				description: "este textul de găsit. Utilizați ghilimele duble (text gol) pentru a corespunde primului caracter din în_text; metacaracterele nu se admit"
			},
			{
				name: "în_text",
				description: "este textul care conține textul de găsit"
			},
			{
				name: "num_start",
				description: "precizează de la care caracter începe căutarea. Primul caracter din în_text este caracterul numărul 1. Dacă se omite, Start_num = 1"
			}
		]
	},
	{
		name: "FINV",
		description: "Returnează inversa distribuției de probabilitate F (unilaterale dreapta): dacă p = FDIST(x;...), atunci FINV(p;....) = x.",
		arguments: [
			{
				name: "probabilitate",
				description: "este o probabilitate asociată cu distribuția cumulativă F, un număr între 0 și 1 inclusiv"
			},
			{
				name: "grade_libertate1",
				description: "este numărătorul gradelor de libertate, un număr între 1 și 10^10, exclusiv 10^10"
			},
			{
				name: "grade_libertate2",
				description: "este numitorul gradelor de libertate, un număr între 1 și 10^10, exclusiv 10^10"
			}
		]
	},
	{
		name: "FISHER",
		description: "Returnează transformata Fisher.",
		arguments: [
			{
				name: "x",
				description: "este valoarea pentru care se efectuează transformarea, un număr între -1 și 1, exclusiv -1 și 1"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Returnează inversa transformatei Fisher: dacă y = FISHER(x), atunci FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "este valoarea pentru care se efectuează inversa transformatei"
			}
		]
	},
	{
		name: "FIXED",
		description: "Rotunjește un număr la numărul specificat de  zecimale și returnează rezultatul ca text cu sau fără virgule.",
		arguments: [
			{
				name: "număr",
				description: "este numărul de rotunjit și de transformat în text"
			},
			{
				name: "zecimale",
				description: "este numărul de cifre de la dreapta separatorului zecimal. Dacă este omis, zecimale = 2"
			},
			{
				name: "nr_virgule",
				description: "este o valoare logică: nu afișează virgule în textul întors = TRUE; afișează virgule în textul întors = FALSE sau omisă"
			}
		]
	},
	{
		name: "FLOOR",
		description: "Rotunjește prin lipsă un număr, la cel mai apropiat multiplu de semnificație.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea numerică de rotunjit"
			},
			{
				name: "semnificație",
				description: "este multiplul la care se rotunjește. număr și semnif sunt ambele fie pozitive fie negative"
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "Rotunjește prin scădere un număr, la cel mai apropiat întreg sau la cel mai apropiat multiplu de semnificație.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea de rotunjit"
			},
			{
				name: "semnificație",
				description: "este multiplul la care se face rotunjirea"
			},
			{
				name: "mod",
				description: "atunci când este dat și non-zero, această funcție se va rotunji spre zero"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "Rotunjește în jos prin adaos un număr, la cel mai apropiat întreg sau la cel mai apropiat multiplu de precizie.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea numerică de rotunjit"
			},
			{
				name: "semnificație",
				description: "este multiplul la care se face rotunjirea. "
			}
		]
	},
	{
		name: "FORECAST",
		description: "Calculează, sau prezice o valoare viitoare de-a lungul unei tendințe liniare utilizând valori existente.",
		arguments: [
			{
				name: "x",
				description: "este punctul de date pentru care se prezice o valoare și trebuie să fie o valoare numerică"
			},
			{
				name: "y_cunoscute",
				description: "este matricea sau zona de date numerice dependentă"
			},
			{
				name: "x_cunoscute",
				description: "este matricea sau zona de date numerice independentă. Varianța pentru Cunoscute_x nu trebuie să fie zero"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "Returnează o formulă ca șir.",
		arguments: [
			{
				name: "referință",
				description: "este o referință la o formulă"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "Calculează cât de des apar valorile într-o zonă de valori, apoi returnează o matrice verticală de numere având un element mai mult decât matrice_bin.",
		arguments: [
			{
				name: "matrice_date",
				description: "este o matrice de, sau o referință la un set de valori pentru care se numără frecvențele apariției (celulele necompletate și text sunt ignorate)"
			},
			{
				name: "matrice_bin",
				description: "este o matrice de, sau o referință la intervalele în care se grupează valorile din matrice_date"
			}
		]
	},
	{
		name: "FTEST",
		description: "Returnează rezultatul unui test F, probabilitatea bilaterală ca varianțele din Matrice1 și Matrice2 să nu fie semnificativ diferite.",
		arguments: [
			{
				name: "matrice1",
				description: "este prima matrice sau zonă de date și pot fi numere sau nume, matrice sau referințe care conțin numere (celulele necompletate se ignoră)"
			},
			{
				name: "matrice2",
				description: "este a doua matrice sau zonă de date și pot fi numere sau nume, matrice sau referințe care conțin numere (celulele necompletate se ignoră)"
			}
		]
	},
	{
		name: "FV",
		description: "Returnează valoarea viitoare a unei investiții bazate pe plăți constante periodice și o rată constantă a dobânzii.",
		arguments: [
			{
				name: "rată",
				description: "este rata dobânzii pe perioadă. De exemplu, utilizați 6%/4 pentru plăți trimestriale la o dobândă anuală de 6%"
			},
			{
				name: "nper",
				description: "este numărul total de perioade de plată dintr-o investiție"
			},
			{
				name: "pmt",
				description: "este plata făcută pentru fiecare perioadă; nu se modifică pe toată durata investiției"
			},
			{
				name: "pv",
				description: "este valoarea prezentă sau valoarea actuală a unui vărsământ unic corespunzător unei serii de plăți viitoare. Dacă se omite, Pv = 0"
			},
			{
				name: "tip",
				description: "este o valoare care reprezintă scadența plății: plata la începutul perioadei = 1; plata la sfârșitul perioadei = 0 sau se omite"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "Returnează valoarea viitoare a unui principal inițial, după aplicarea unei serii de rate ale dobânzii compuse.",
		arguments: [
			{
				name: "principal",
				description: "este valoarea prezentă"
			},
			{
				name: "plan",
				description: "este un index de rate de dobândă de aplicat"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Returnează valoarea funcției Gamma.",
		arguments: [
			{
				name: "x",
				description: "este valoarea pentru care doriți să calculați Gamma"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "Returnează distribuția gamma.",
		arguments: [
			{
				name: "x",
				description: "este valoarea în care se evaluează distribuția, un număr nenegativ"
			},
			{
				name: "alfa",
				description: "este un parametru pentru distribuție, un număr pozitiv"
			},
			{
				name: "beta",
				description: "este un parametru pentru distribuție, un număr pozitiv. Dacă beta = 1, GAMMA.DIST returnează distribuția gamma standard"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică: returnează funcția distribuție cumulativă = TRUE; returnează funcția masă de probabilitate = FALSE sau se omite"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Returnează inversa distribuției cumulativ gamma: dacă p = GAMMA.DIST(x,...), atunci GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "probabilitate",
				description: "este probabilitatea asociată cu distribuția gamma, un număr între 0 și 1, inclusiv"
			},
			{
				name: "alfa",
				description: "este un  parametru al distribuției, un număr pozitiv"
			},
			{
				name: "beta",
				description: "este un parametru al distribuției, un număr pozitiv. Dacă beta = 1, GAMMA.INV returnează inversa distribuției gamma standard"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "Returnează distribuția gamma.",
		arguments: [
			{
				name: "x",
				description: "este valoarea la care se evaluează distribuția, un număr nenegativ"
			},
			{
				name: "alfa",
				description: "este un parametru pentru distribuție, un număr pozitiv"
			},
			{
				name: "beta",
				description: "este un parametru pentru distribuție, un număr pozitiv. Dacă beta = 1, GAMMADIST returnează distribuția gamma standard"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică: returnează funcția distribuție cumulativă = TRUE; returnează funcția masă de probabilitate = FALSE sau se omite"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Returnează inversa distribuției cumulativ gamma: dacă p = GAMMADIST(x,...), atunci GAMMAINV(p,...) = x.",
		arguments: [
			{
				name: "probabilitate",
				description: "este probabilitatea asociată cu distribuția gamma, un număr între 0 și 1, inclusiv"
			},
			{
				name: "alfa",
				description: "este un parametru al distribuției, un număr pozitiv"
			},
			{
				name: "beta",
				description: "este un parametru al distribuției, un număr pozitiv. Dacă beta = 1, GAMMAINV returnează inversa distribuției gamma standard"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Returnează logaritmul natural al funcției gamma.",
		arguments: [
			{
				name: "x",
				description: "este valoarea pentru care se calculează GAMMALN, un număr pozitiv"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "Returnează logaritmul natural al funcției gamma.",
		arguments: [
			{
				name: "x",
				description: "este valoarea pentru care se calculează GAMMALN.PRECISE, un număr pozitiv"
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
		description: "Returnează cel mai mare divizor comun.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 valori"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 valori"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "Returnează media geometrică a unei matrice sau zone de date numerice pozitive.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere sau nume, matrice sau referințe care conțin numere pentru care se calculează media"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere sau nume, matrice sau referințe care conțin numere pentru care se calculează media"
			}
		]
	},
	{
		name: "GESTEP",
		description: "Se testează dacă numărul este mai mare decât valoarea de prag.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea de testat relativ la prag"
			},
			{
				name: "prag",
				description: "este valoarea de prag"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "Extrage date stocate într-un PivotTable.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "câmp_date",
				description: "este numele câmpului de date din care se extrag date"
			},
			{
				name: "tabel_pivot",
				description: "este o referință la o celulă sau la o zonă de celule din PivotTable care conține datele de regăsit"
			},
			{
				name: "câmp",
				description: "câmpul la care se face referire"
			},
			{
				name: "element",
				description: "elementul de câmp la care se face referire"
			}
		]
	},
	{
		name: "GROWTH",
		description: "Returnează numere cu o tendință de creștere exponențială corespunzător punctelor de date cunoscute.",
		arguments: [
			{
				name: "y_cunoscute",
				description: "este setul de valori-y deja cunoscute în relația y = b*m^x, o matrice sau o zonă de numere pozitive"
			},
			{
				name: "x_cunoscute",
				description: "este un set opțional de valori-x deja cunoscute în relația y = b*m^x, o matrice sau zonă de aceeași dimensiune cu y_cunoscute"
			},
			{
				name: "x_noi",
				description: "sunt valori-x noi pentru care GROWTH returnează valorile-y corespunzătoare"
			},
			{
				name: "const",
				description: "este o valoare logică: constanta b este calculată normal dacă Const = TRUE; b este egal cu 1 dacă Const = FALSE sau se omite"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "Returnează media armonică a unui set de date de numere pozitive: reciproca mediei aritmetice a reciprocelor.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere sau nume, matrice sau referințe care conțin numere pentru care se calculează media armonică"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere sau nume, matrice sau referințe care conțin numere pentru care se calculează media armonică"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "Se efectuează conversia unui număr hexazecimal într-un număr binar.",
		arguments: [
			{
				name: "număr",
				description: "este numărul hexazecimal care va suferi conversia"
			},
			{
				name: "locuri",
				description: "este numărul de caractere de utilizat"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "Se efectuează conversia unui număr hexazecimal într-un număr zecimal.",
		arguments: [
			{
				name: "număr",
				description: "ieste numărul hexazecimal care realizează conversie"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "Se efectuează conversia unui număr hexazecimal într-un octal.",
		arguments: [
			{
				name: "număr",
				description: "este numărul hexazecimal care va suferi conversia"
			},
			{
				name: "locuri",
				description: "este numărul de caractere de utilizat"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "Caută o valoare în rândul de sus al unei tabele sau matrice de valori și returnează valoarea în aceeași coloană dintr-un rând specificat.",
		arguments: [
			{
				name: "căutare_valoare",
				description: "este valoarea de găsit în primul rând al tabelei și este o valoare, o referință, sau un șir text"
			},
			{
				name: "matrice_tabel",
				description: "este o tabelă de text, numere, sau valori logice în care sunt căutate datele. Table_matrice este o referință la o zonă sau un nume de zonă"
			},
			{
				name: "num_index_rând",
				description: "este numărul rândului din Table_matrice din care este întoarsă valoarea care se potrivește. Primul rând de valori din tabelă este rândul 1"
			},
			{
				name: "zonă_căutare",
				description: "este o valoare logică: pentru a găsi cea mai apropiată potrivire în rândul de sus (sortat în ordine ascendentă) = TRUE sau omis; găsirea unei potriviri exacte = FALSE"
			}
		]
	},
	{
		name: "HOUR",
		description: "Returnează ora ca număr de la 0 (12:00 A.M.) la 23 (11:00 P.M.).",
		arguments: [
			{
				name: "număr_serie",
				description: "este un număr în cod dată-oră utilizat de Spreadsheet sau text în format de oră, cum ar fi 16:48:00 sau 4:48:00 PM"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "Creează o comandă rapidă sau un salt care deschide un document stocat pe hard disc, un server de rețea, sau pe Internet.",
		arguments: [
			{
				name: "locație_link",
				description: "este textul care dă calea și numele de fișier pentru documentul care se deschide, o locație pe hard disc, o adresă UNC, sau o cale URL"
			},
			{
				name: "nume_prietenos",
				description: "este textul sau numărul afișat în celulă. Dacă este omis, celula afișează textul Link_location"
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "Returnează distribuția hipergeometrică.",
		arguments: [
			{
				name: "eșantion_s",
				description: "este numărul de  succese din eșantion"
			},
			{
				name: "număr_eșantion",
				description: "este dimensiunea eșantionului"
			},
			{
				name: "populație_s",
				description: "este numărul de succese din populație"
			},
			{
				name: "număr_pop",
				description: "este dimensiunea populației"
			},
			{
				name: "cumulativ",
				description: "este valoarea logică: pentru funcția de distribuție cumulativă, se utilizează TRUE; pentru funcția de densitate a probabilității, se utilizează FALSE"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "Returnează distribuția hipergeometrică.",
		arguments: [
			{
				name: "eșantion_s",
				description: "este numărul de succese din eșantion"
			},
			{
				name: "număr_eșantion",
				description: "este dimensiunea eșantionului"
			},
			{
				name: "populație_s",
				description: "este numărul de succese din populație"
			},
			{
				name: "număr_pop",
				description: "este dimensiunea populației"
			}
		]
	},
	{
		name: "IF",
		description: "Verifică îndeplinirea unei condiții și returnează o valoare dacă o condiție precizată evaluează TRUE și altă valoare dacă evaluează FALSE.",
		arguments: [
			{
				name: "test_logic",
				description: "este orice valoare sau expresie care evaluează TRUE sau FALSE"
			},
			{
				name: "valoare_dacă_adevărat",
				description: "este valoarea întoarsă dacă test_logic este TRUE. Dacă se omite, se returnează TRUE. Se pot imbrica până la șapte funcții IF"
			},
			{
				name: "valoare_dacă_fals",
				description: "este valoarea întoarsă dacă test_logic este FALSE. Dacă se omite, se returnează FALSE"
			}
		]
	},
	{
		name: "IFERROR",
		description: "Returnează valoare_dacă_eroare dacă expresia este o eroare și valoarea expresiei în sine nu este o eroare.",
		arguments: [
			{
				name: "valoare",
				description: "este orice valoare sau expresie sau referință"
			},
			{
				name: "valoare_dacă_eroare",
				description: "este orice valoare sau expresie sau referință"
			}
		]
	},
	{
		name: "IFNA",
		description: "Returnează valoarea pe care o specificați dacă expresia se rezolvă la #N/A, altfel, returnează rezultatul expresiei.",
		arguments: [
			{
				name: "valoare",
				description: "este orice valoare sau expresie sau referință"
			},
			{
				name: "valoare_dacă_na",
				description: "este orice valoare sau expresie sau referință"
			}
		]
	},
	{
		name: "IMABS",
		description: "Returnează valoarea absolută (modulul) a unui număr complex.",
		arguments: [
			{
				name: "număr_i",
				description: "este un număr complex căruia pentru care doriți valoarea absolută"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "Returnează coeficientul imaginar al unui număr complex.",
		arguments: [
			{
				name: "număr_i",
				description: "este numărul complex pentru care doriți coeficientul imaginar"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Returnează argumentul q, un unghi exprimat în radiani.",
		arguments: [
			{
				name: "număr_i",
				description: "este numărul complex pentru care doriți argumentul"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "Returnează conjugata complexă a unui număr complex.",
		arguments: [
			{
				name: "număr_i",
				description: "este un număr complex pentru care doriți conjugata"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Returnează cosinusul unui număr complex.",
		arguments: [
			{
				name: "număr_i",
				description: "este numărul complex pentru care doriți cosinusul"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Returnează cosinusul hiperbolic al unui număr complex.",
		arguments: [
			{
				name: "număr",
				description: "este un număr complex pentru care doriți cosinusul hiperbolic"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Returnează cotangenta unui număr complex.",
		arguments: [
			{
				name: "număr",
				description: "este numărul complex pentru care doriți cotangenta"
			}
		]
	},
	{
		name: "IMCSC",
		description: "Returnează cosecanta unui număr complex.",
		arguments: [
			{
				name: "număr",
				description: "este numărul complex pentru care doriți cosecanta"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Returnează cosecanta hiperbolică a unui număr complex.",
		arguments: [
			{
				name: "număr",
				description: "este numărul complex pentru care doriți cosecanta hiperbolică"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Returnează câtul a două numere complexe.",
		arguments: [
			{
				name: "număr_i1",
				description: "este numărătorul sau de împărțitul complex"
			},
			{
				name: "număr_i2",
				description: "este numitorul sau divizorul complex"
			}
		]
	},
	{
		name: "IMEXP",
		description: "Returnează exponentul unui număr complex.",
		arguments: [
			{
				name: "număr_i",
				description: "este numărul complex pentru care doriți exponentul"
			}
		]
	},
	{
		name: "IMLN",
		description: "Returnează logaritmul natural al unui număr complex.",
		arguments: [
			{
				name: "număr_i",
				description: "este un număr complex pentru care doriți logaritmul natural"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Returnează un logaritm în baza 10 al unui număr complex.",
		arguments: [
			{
				name: "număr_i",
				description: "este un număr complex pentru doriți logaritmul comun"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Returnează un logaritm în baza 2 al unui număr complex.",
		arguments: [
			{
				name: "număr_i",
				description: "este un număr complex pentru care doriți logaritmul în baza 2"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "Returnează un număr complex ridicat la o putere întreagă.",
		arguments: [
			{
				name: "număr_i",
				description: "este un număr complex care trebuie ridicat la putere"
			},
			{
				name: "număr",
				description: "este puterea la care se ridică numărul complex"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "Returnează produsul numerelor complexe de la 1 la 255.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr_i1",
				description: "Inumăr1, Inumăr2,... sunt de la 1 la 255 numere complexe de multiplicat."
			},
			{
				name: "număr_i2",
				description: "Inumăr1, Inumăr2,... sunt de la 1 la 255 numere complexe de multiplicat."
			}
		]
	},
	{
		name: "IMREAL",
		description: "Returnează coeficientul real al unui număr complex.",
		arguments: [
			{
				name: "număr_i",
				description: "este un număr complex pentru care doriți coeficientul real"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Returnează secanta unui număr complex.",
		arguments: [
			{
				name: "număr",
				description: "este numărul complex pentru care doriți secanta"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Returnează secanta hiperbolică a unui număr complex.",
		arguments: [
			{
				name: "număr",
				description: "este numărul complex pentru care doriți secanta hiperbolică"
			}
		]
	},
	{
		name: "IMSIN",
		description: "Returnează sinusul unui număr complex.",
		arguments: [
			{
				name: "număr_i",
				description: "este un număr complex pentru care doriți sinusul"
			}
		]
	},
	{
		name: "IMSINH",
		description: "Returnează sinusul hiperbolic al unui număr complex.",
		arguments: [
			{
				name: "număr",
				description: "este un număr complex pentru care doriți sinusul hiperbolic"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "Returnează rădăcina pătrată a numărului complex.",
		arguments: [
			{
				name: "număr_i",
				description: "este un număr complex pentru care doriți rădăcina pătrată"
			}
		]
	},
	{
		name: "IMSUB",
		description: "Returnează diferența dintre două numere complexe.",
		arguments: [
			{
				name: "număr_i1",
				description: "este numărul complex din care se scade număr_i2"
			},
			{
				name: "număr_i2",
				description: "este numărul complex care se scade din număr_i1"
			}
		]
	},
	{
		name: "IMSUM",
		description: "Returnează suma numerelor complexe.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr_i1",
				description: "sunt de adăugat numere complexe de la 1 la 255"
			},
			{
				name: "număr_i2",
				description: "sunt de adăugat numere complexe de la 1 la 255"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Returnează tangenta unui număr complex.",
		arguments: [
			{
				name: "număr",
				description: "este numărul complex pentru care doriți tangenta"
			}
		]
	},
	{
		name: "INDEX",
		description: "Returnează o valoare sau referința la o celulă de la intersecția unui rând și unei coloane particulare, dintr-o zonă dată.",
		arguments: [
			{
				name: "matrice",
				description: "este o zonă de celule sau o constantă matrice."
			},
			{
				name: "num_rând",
				description: "selectează rândul din Matrice sau din Referință din care returnează o valoare. Dacă se omite, Num_coloană este obligatoriu"
			},
			{
				name: "num_coloană",
				description: "selectează coloana din Matrice sau din Referință din care returnează o valoare. Dacă se omite, Num_rând este obligatoriu"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "Returnează referința specificată de un șir text.",
		arguments: [
			{
				name: "text_ref",
				description: "este o referință la o celulă care conține un stil de referință A1 sau R1C1, un nume definit ca o referință, sau o referință la o celulă ca un șir text"
			},
			{
				name: "a1",
				description: "este o valoare logică și specifică tipul de referință din Ref_text: stil R1C1= FALSE; stil A1 = TRUE sau omis"
			}
		]
	},
	{
		name: "INFO",
		description: "Returnează informațiile privind mediul de operare curent.",
		arguments: [
			{
				name: "tip_text",
				description: "este textul care specifică tipul de informații care se întorc."
			}
		]
	},
	{
		name: "INT",
		description: "Rotunjește un număr prin lipsă la cel mai apropiat întreg.",
		arguments: [
			{
				name: "număr",
				description: "este numărul de rotunjit prin lipsă la un întreg"
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "Calculează punctul în care o linie va intersecta axa y utilizând linia de regresie care se potrivește cel mai bine. Linia este trasată printre valorile x și y cunoscute.",
		arguments: [
			{
				name: "cunoscute_y",
				description: "este setul dependent de observații sau de date și pot fi numere sau nume, matrice, sau referințe care conțin numere"
			},
			{
				name: "cunoscute_x",
				description: "este setul independent de observații sau de date și pot fi numere sau nume, matrice, sau referințe care conțin numere"
			}
		]
	},
	{
		name: "INTRATE",
		description: "Returnează rata dobânzii pentru o asigurare în care s-a investit integral.",
		arguments: [
			{
				name: "tranzacție",
				description: "este data de lichidare a asigurării, exprimată ca număr serial"
			},
			{
				name: "maturitate",
				description: "este data de maturitate a asigurării, exprimată ca număr serial"
			},
			{
				name: "investiție",
				description: "este suma investită în asigurare"
			},
			{
				name: "rambursare",
				description: "este suma care va fi primită la maturitate"
			},
			{
				name: "bază",
				description: "este tipul de bază de numărare a zilelor care trebuie utilizat"
			}
		]
	},
	{
		name: "IPMT",
		description: "Returnează pentru o perioadă dată plata dobânzii unei investiții, bazată pe plăți constante periodice și pe o rată constantă a dobânzii.",
		arguments: [
			{
				name: "rată",
				description: "este rata dobânzii pe o perioadă. De exemplu, utilizați 6%/4 pentru plăți trimestriale la o dobândă anuală de 6%"
			},
			{
				name: "per",
				description: "este perioada pentru care se determină dobânda și este în intervalul de la 1 la Nper"
			},
			{
				name: "nper",
				description: "este numărul total de perioade de plată dintr-o investiție"
			},
			{
				name: "pv",
				description: "este valoarea prezentă sau valoarea actuală a unui vărsământ unic corespunzător unei serii de plăți viitoare"
			},
			{
				name: "fv",
				description: "este valoarea viitoare sau balanța în numerar la care se ajunge după ce ultima plată este făcută. Dacă se omite, Fv = 0"
			},
			{
				name: "tip",
				description: "este o valoare logică reprezentând scadența plății: la sfârșitul perioadei = 0 sau se omite, la începutul perioadei = 1"
			}
		]
	},
	{
		name: "IRR",
		description: "Returnează rata de rentabilitate internă pentru o serie de fluxuri de numerar.",
		arguments: [
			{
				name: "valori",
				description: "este o matrice sau o  referință la celule care conțin numere pentru care calculați rata de rentabilitate internă"
			},
			{
				name: "estim",
				description: "este un număr care reprezintă o aproximare a rezultatului funcției IRR; 0.1 (10 procente) dacă este omis"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "Verifică dacă o referință este către o celulă goală și returnează TRUE sau FALSE.",
		arguments: [
			{
				name: "valoare",
				description: "este celula sau numele care fac referire la celula de testat"
			}
		]
	},
	{
		name: "ISERR",
		description: "Verifică dacă o valoare este o eroare (#VALUE!, #REF!, #DIV/0!, #NUM!, #NAME? sau #NULL!) excluzând #N/A și întoarce TRUE sau FALSE.",
		arguments: [
			{
				name: "valoare",
				description: "este valoarea de testat. Valoare poate face referire la o celulă, o formulă sau un nume care face referire la o celulă, o formulă sau o valoare"
			}
		]
	},
	{
		name: "ISERROR",
		description: "Verifică dacă o valoare este o eroare (#N/A, #VALUE!, #REF!, #DIV/0!, #NUM!, #NAME? sau #NULL!) și întoarce TRUE sau FALSE.",
		arguments: [
			{
				name: "valoare",
				description: "este valoarea de testat. Valoarea poate face referire la o celulă, o formulă sau un nume care face referire la o celulă, o formulă sau o valoare"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "Returnează TRUE dacă numărul este par.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea de testat"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "Verifică dacă o referință este la o celulă ce conține o formulă și returnează TRUE sau FALSE.",
		arguments: [
			{
				name: "referință",
				description: "este o referință la celula pe care doriți să o testați. Referința poate fi o referință la celulă, o formulă sau un nume care se referă la o celulă"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "Verifică dacă o valoare este logică (TRUE sau FALSE) și returnează TRUE sau FALSE.",
		arguments: [
			{
				name: "valoare",
				description: "este valoarea de testat. Valoare face referire la o celulă, o formulă sau un nume care fac referire la o celulă, formulă sau valoare"
			}
		]
	},
	{
		name: "ISNA",
		description: "Verifică dacă o valoare este #N/A și returnează TRUE sau FALSE.",
		arguments: [
			{
				name: "valoare",
				description: "este valoarea de testat. Valoarea poate face referire la o celulă, formulă sau nume care fac referire la o celulă, formulă sau valoare"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "Verifică dacă o valoare este text (celulele necompletate nu conțin text) și returnează TRUE sau FALSE.",
		arguments: [
			{
				name: "valoare",
				description: "este valoarea de testat: o celulă, o formulă sau un nume care fac referire la o celulă, formulă sau valoare"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "Verifică dacă o valoare este număr și returnează TRUE sau FALSE.",
		arguments: [
			{
				name: "valoare",
				description: "este valoarea de testat. Valoare poate face referire la o celulă, o formulă sau un nume care fac referire la o celulă, formulă sau valoare"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "Rotunjește prin adaos un număr, la cel mai apropiat întreg sau la cel mai apropiat multiplu de semnificație.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea de rotunjit"
			},
			{
				name: "semnificație",
				description: "este multiplul opțional la care se face rotunjirea"
			}
		]
	},
	{
		name: "ISODD",
		description: "Returnează TRUE dacă numărul este impar.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea de testat"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "Returnează numărul de săptămână ISO al anului pentru o anumită dată.",
		arguments: [
			{
				name: "dată",
				description: "este codul dată-oră utilizat de Spreadsheet pentru calculul datei și orei"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Returnează dobânda plătită într-o anumită perioadă pentru o investiție.",
		arguments: [
			{
				name: "rată",
				description: "rata dobânzii pe o perioadă. De exemplu, utilizați 6%/4 pentru plăți trimestriale la o dobândă anuală de 6%"
			},
			{
				name: "per",
				description: "perioada pentru care se determină dobânda"
			},
			{
				name: "nper",
				description: "numărul de perioade de plată dintr-o investiție"
			},
			{
				name: "pv",
				description: "valoarea actuală a unui vărsământ unic corespunzător unei serii de plăți viitoare"
			}
		]
	},
	{
		name: "ISREF",
		description: "Verifică dacă o valoare este o referință și returnează TRUE sau FALSE.",
		arguments: [
			{
				name: "valoare",
				description: "este valoarea de testat. Valoare poate face referire la o celulă, o formulă sau un nume care fac referire la o celulă, formulă sau valoare"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "Verifică dacă o valoare este text și returnează TRUE sau FALSE.",
		arguments: [
			{
				name: "valoare",
				description: "este valoarea de testat. Valoare poate face referire la o celulă, o formulă sau un nume care fac referire la o celulă, formulă sau valoare"
			}
		]
	},
	{
		name: "KURT",
		description: "Returnează coeficientul kurtosis pentru un set de date.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere sau de nume, matrice sau referințe care conțin numere pentru care se calculează coeficientul kurtosis"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere sau de nume, matrice sau referințe care conțin numere pentru care se calculează coeficientul kurtosis"
			}
		]
	},
	{
		name: "LARGE",
		description: "Returnează a k-a din cele mai mari valori dintr-un set de date. De exemplu, al cincilea din cele mai mari numere.",
		arguments: [
			{
				name: "matrice",
				description: "este matricea sau zona de date pentru care se determină a k-a valoare ca mărime"
			},
			{
				name: "k",
				description: "este poziția valorii de întors (față de cea mai mare) din matrice sau din zona de celule"
			}
		]
	},
	{
		name: "LCM",
		description: "Returnează ultimul multiplu comun.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 valori pentru care doriți ultimul multiplu comun"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 valori pentru care doriți ultimul multiplu comun"
			}
		]
	},
	{
		name: "LEFT",
		description: "Returnează numărul precizat de caractere de la începutul unui șir text.",
		arguments: [
			{
				name: "text",
				description: "este șirul text care conține caracterele de extras"
			},
			{
				name: "car_num",
				description: "precizează câte caractere să extragă LEFT; 1 dacă se omite"
			}
		]
	},
	{
		name: "LEN",
		description: "Returnează numărul de caractere într-un șir text.",
		arguments: [
			{
				name: "text",
				description: "este textul a cărui lungime se calculează. Spațiile sunt numărate drept caractere"
			}
		]
	},
	{
		name: "LINEST",
		description: "Returnează statistica ce descrie o tendință liniară care se potrivește cel mai bine punctelor de date, prin potrivirea unei linii drepte utilizând metoda celor mai mici pătrate.",
		arguments: [
			{
				name: "y_cunoscute",
				description: "este setul de valori-y deja cunoscute în relația y = mx + b"
			},
			{
				name: "x_cunoscute",
				description: "este un set opțional de valori-x deja cunoscute în relația y = mx + b"
			},
			{
				name: "const",
				description: "este o valoare logică: constanta b este calculată normal dacă Const = TRUE sau se omite; b este egal cu 0 dacă Const = FALSE"
			},
			{
				name: "statistică",
				description: "este o valoare logică: returnează statistica de regresie suplimentară = TRUE; returnează coeficienții m și constanta b = FALSE sau se omite"
			}
		]
	},
	{
		name: "LN",
		description: "Returnează logaritmul natural al unui număr.",
		arguments: [
			{
				name: "număr",
				description: "este numărul real pozitiv pentru care se calculează logaritmul natural"
			}
		]
	},
	{
		name: "LOG",
		description: "Returnează logaritmul unui număr în baza specificată.",
		arguments: [
			{
				name: "număr",
				description: "este numărul real pozitiv pentru care se calculează logaritmul"
			},
			{
				name: "baza",
				description: "este baza logaritmului; 10 dacă este omisă"
			}
		]
	},
	{
		name: "LOG10",
		description: "Returnează logaritmul în baza 10 a unui număr.",
		arguments: [
			{
				name: "număr",
				description: "este numărul real pozitiv pentru care se calculează logaritmul în baza 10"
			}
		]
	},
	{
		name: "LOGEST",
		description: "Returnează o statistică ce descrie o curbă exponențială care corespunde punctelor de date cunoscute.",
		arguments: [
			{
				name: "y_cunoscute",
				description: "este setul de valori-y deja cunoscute în relația y = b*m^x"
			},
			{
				name: "x_cunoscute",
				description: "este un set opțional de valori-x deja cunoscute în relația y = b*m^x"
			},
			{
				name: "const",
				description: "este o valoare logică: constanta b este calculată normal dacă Const = TRUE sau se omite; b este egal cu 1 dacă Const = FALSE"
			},
			{
				name: "statistică",
				description: "este o valoare logică: returnează statistica de regresie suplimentară = TRUE; returnează coeficienții m și constanta b = FALSE sau se omite"
			}
		]
	},
	{
		name: "LOGINV",
		description: "Returnează inversa funcției distribuție cumulativă lognormală a lui x, unde ln(x) este distribuit normal cu parametrii media și dev_standard.",
		arguments: [
			{
				name: "probabilitate",
				description: "este o probabilitate asociată cu distribuția lognormală, un număr între 0 și 1, inclusiv"
			},
			{
				name: "media",
				description: "este media lui ln(x)"
			},
			{
				name: "dev_standard",
				description: "este deviația standard a lui ln(x), un număr pozitiv"
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "Returnează distribuția lognormală a lui x, în care ln(x) este distribuit normal cu parametrii media și dev_standard.",
		arguments: [
			{
				name: "x",
				description: "este valoarea în care se evaluează funcția, un număr pozitiv"
			},
			{
				name: "media",
				description: "este media lui ln(x)"
			},
			{
				name: "dev_standard",
				description: "este deviația standard a ln(x), un număr pozitiv"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică: pentru funcția de distribuție cumulativă, se utilizează TRUE; pentru funcția de densitate a probabilității, se utilizează FALSE"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Returnează inversa funcției distribuție cumulativă lognormală a lui x, unde ln(x) este distribuit normal cu parametrii media și dev_standard.",
		arguments: [
			{
				name: "probabilitate",
				description: "este o probabilitate asociată cu distribuția lognormală, un număr între 0 și 1, inclusiv"
			},
			{
				name: "media",
				description: "este media lui ln(x)"
			},
			{
				name: "dev_standard",
				description: "este deviația standard a lui ln(x), un număr pozitiv"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "Returnează distribuția lognormală cumulativă a lui x, în care ln(x) este distribuit normal cu parametrii media și dev_standard.",
		arguments: [
			{
				name: "x",
				description: "este valoarea în care se evaluează funcția, un număr pozitiv"
			},
			{
				name: "media",
				description: "este media lui ln(x)"
			},
			{
				name: "dev_standard",
				description: "este deviația standard a ln(x), un număr pozitiv"
			}
		]
	},
	{
		name: "LOOKUP",
		description: "Caută o valoare fie într-o zonă de un rând sau de o coloană, fie într-o matrice. Este furnizată pentru compatibilitate cu versiunile anterioare.",
		arguments: [
			{
				name: "căutare_valoare",
				description: "este o valoare pe care funcția LOOKUP o caută în Lookup_vector și este un număr, text, o valoare logică, un nume sau o referință la o valoare"
			},
			{
				name: "căutare_vector",
				description: "este o zonă care conține numai un rând sau o coloană de text, numere sau valori logice, plasate în ordine ascendentă"
			},
			{
				name: "rezultat_vector",
				description: "este o zonă care conține numai un rând sau o coloană, de aceeași dimensiune cu Lookup_vector"
			}
		]
	},
	{
		name: "LOWER",
		description: "Transformă toate literele dintr-un șir text în litere mici.",
		arguments: [
			{
				name: "text",
				description: "este textul de transformat în litere mici. Caracterele din Text care nu sunt litere nu se modifică"
			}
		]
	},
	{
		name: "MATCH",
		description: "Returnează poziția relativă a unui element dintr-o matrice care corespunde unei valori precizate într-o ordine precizată.",
		arguments: [
			{
				name: "căutare_valoare",
				description: "este valoarea utilizată pentru a găsi valoarea din matrice, un număr, text, valoare logică sau o referință la una din acestea"
			},
			{
				name: "căutare_matrice",
				description: "este o zonă contiguă de celule care conțin posibile valori de căutat, o matrice de valori sau o referință la o matrice"
			},
			{
				name: "tip_ret",
				description: "este un număr 1, 0 sau -1 indicând care valoare se returnează."
			}
		]
	},
	{
		name: "MAX",
		description: "Returnează cea mai mare valoare dintr-un set de valori. Ignoră valorile logice și textul.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere, celule goale, valori logice sau numere text pentru care se calculează maximul"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere, celule goale, valori logice sau numere text pentru care se calculează maximul"
			}
		]
	},
	{
		name: "MAXA",
		description: "Returnează cea mai mare valoare dintr-un set de valori. Nu ignoră valorile logice și text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valoare1",
				description: "sunt de la 1 la 255 de numere, celule goale, valori logice sau numere text pentru care se calculează maximul"
			},
			{
				name: "valoare2",
				description: "sunt de la 1 la 255 de numere, celule goale, valori logice sau numere text pentru care se calculează maximul"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Returnează determinantul matriceal al matricei.",
		arguments: [
			{
				name: "matrice",
				description: "este o matrice numerică cu un număr egal de rânduri și coloane, fie o zonă de celule sau o constantă matrice"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Returnează mediana sau numărul din mijlocul unui set de numere date.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere sau de nume, matrice sau referințe care conțin numere, pentru care se află mediana"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere sau de nume, matrice sau referințe care conțin numere, pentru care se află mediana"
			}
		]
	},
	{
		name: "MID",
		description: "Returnează caracterele din mijlocul unui șir text fiind date poziția de început și lungimea.",
		arguments: [
			{
				name: "text",
				description: "este șirul text din care se extrag caracterele"
			},
			{
				name: "num_start",
				description: "este poziția primului caracter de extras. Primul caracter din Text este 1"
			},
			{
				name: "car_num",
				description: "specifică câte caractere se întorc din Text"
			}
		]
	},
	{
		name: "MIN",
		description: "Returnează cel mai mic număr dintr-un set de valori. Ignoră valorile logice și textul.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere, celule goale, valori logice sau numere text pentru care se calculează minimul"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere, celule goale, valori logice sau numere text pentru care se calculează minimul"
			}
		]
	},
	{
		name: "MINA",
		description: "Returnează cea mai mică valoare dintr-un set de valori. Nu ignoră valori logice și text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valoare1",
				description: "sunt de la 1 la 255 de numere, celule goale, valori logice sau numere text pentru care se calculează minimul"
			},
			{
				name: "valoare2",
				description: "sunt de la 1 la 255 de numere, celule goale, valori logice sau numere text pentru care se calculează minimul"
			}
		]
	},
	{
		name: "MINUTE",
		description: "Returnează minutul ca număr de la 0 la 59.",
		arguments: [
			{
				name: "număr_serie",
				description: "este un număr în cod dată-oră utilizat de Spreadsheet sau text în format de oră, cum ar fi 16:48:00 sau 4:48:00 PM"
			}
		]
	},
	{
		name: "MINVERSE",
		description: "Returnează inversa matricei pentru matricea stocată într-o zonă.",
		arguments: [
			{
				name: "matrice",
				description: "este o matrice numerică cu un număr egal de rânduri și de coloane, fie o zonă de celule sau o constantă matrice"
			}
		]
	},
	{
		name: "MIRR",
		description: "Returnează rata de rentabilitate internă pentru o serie de fluxuri periodice de numerar, luând în considerare costul investiției și dobânda din reinvestirea numerarului.",
		arguments: [
			{
				name: "valori",
				description: "este o matrice sau o referință la celule care conțin numere care reprezintă o serie de plăți (negative) și venituri (pozitive) la perioade regulate"
			},
			{
				name: "rată_finan",
				description: "este rata dobânzii plătită în bani utilizați în fluxul de numerar"
			},
			{
				name: "rată_reinvest",
				description: "este rata dobânzii care intră în fluxul de numerar la reinvestirea banilor"
			}
		]
	},
	{
		name: "MMULT",
		description: "Returnează matricea produs a două matrice, o matrice cu același număr de rânduri ca matrice1 și același număr de coloane ca matrice2.",
		arguments: [
			{
				name: "matrice1",
				description: "este prima matrice de numere de înmulțit și numărul său de coloane trebuie să fie egal cu numărul de rânduri al Matrice2"
			},
			{
				name: "matrice2",
				description: "este prima matrice de numere de înmulțit și numărul său de coloane trebuie să fie egal cu numărul de rânduri al Matrice2"
			}
		]
	},
	{
		name: "MOD",
		description: "Returnează restul după ce un număr este împărțit la un împărțitor.",
		arguments: [
			{
				name: "număr",
				description: "este numărul pentru care se calculează restul împărțirii"
			},
			{
				name: "împărțitor",
				description: "este împărțitorul argumentului număr"
			}
		]
	},
	{
		name: "MODE",
		description: "Returnează valoarea cea mai frecventă sau repetitivă dintr-o matrice sau zonă de date.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere, nume, matrice sau referințe care conțin numere pentru care se determină modul"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere, nume, matrice sau referințe care conțin numere pentru care se determină modul"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "Returnează o matrice verticală cu valorile cele mai frecvente sau repetitive dintr-o matrice sau zonă de date. Pentru o matrice orizontală, utilizați =TRANSPOSE(MODE.MULT(număr1,număr2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt numere, nume, matrice sau referințe de la 1 la 255 care conțin numere pentru care se determină modul"
			},
			{
				name: "număr2",
				description: "sunt numere, nume, matrice sau referințe de la 1 la 255 care conțin numere pentru care se determină modul"
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "Returnează valoarea cea mai frecventă sau repetitivă dintr-o matrice sau zonă de date.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere sau de nume, matrice sau referințe care conțin numere pentru care se determină modul"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere sau de nume, matrice sau referințe care conțin numere pentru care se determină modul"
			}
		]
	},
	{
		name: "MONTH",
		description: "Returnează luna, un număr de la 1 (ianuarie) la 12 (decembrie).",
		arguments: [
			{
				name: "număr_serie",
				description: "este un număr în cod dată-oră utilizat de Spreadsheet"
			}
		]
	},
	{
		name: "MROUND",
		description: "Returnează un număr care este rotunjit la valoarea dorită.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea de rotunjit"
			},
			{
				name: "multiplu",
				description: "este multiplul la care se rotunjește numărul"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Returnează setul multinomial de numere.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt valori de la 1 la 255 pentru multinomial"
			},
			{
				name: "număr2",
				description: "sunt valori de la 1 la 255 pentru multinomial"
			}
		]
	},
	{
		name: "MUNIT",
		description: "Returnează matricea de unitate pentru dimensiunea specificată.",
		arguments: [
			{
				name: "dimensiune",
				description: "este un număr întreg ce specifică dimensiunea matricei de unitate pe care doriți să o returnați"
			}
		]
	},
	{
		name: "N",
		description: "Transformă o valoare nenumerică într-un număr, date sau numere seriale, TRUE în 1, orice altceva în 0 (zero).",
		arguments: [
			{
				name: "valoare",
				description: "este valoarea de transformat"
			}
		]
	},
	{
		name: "NA",
		description: "Returnează valoarea de eroare #N/A (valoare indisponibilă).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "Returnează distribuția binomială negativă, probabilitatea că vor exista un număr (număr_f) eșecuri înaintea succesului cu numărul număr_s, cu probabilitatea probabilitate_s a unui succes.",
		arguments: [
			{
				name: "număr_f",
				description: "este numărul de erori"
			},
			{
				name: "număr_s",
				description: "este numărul de succese din punctul de început"
			},
			{
				name: "probabilitate_s",
				description: "este probabilitatea unui succes; un număr între 0 și 1"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică: pentru funcția de distribuție cumulativă, se utilizează TRUE; pentru funcția de probabilitate a masei, se utilizează FALSE"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "Returnează distribuția binomială negativă, probabilitatea că vor exista un număr (număr_f) eșecuri înaintea succesului cu numărul număr_s, cu probabilitatea probabilitate_s a unui succes.",
		arguments: [
			{
				name: "număr_f",
				description: "este numărul de erori"
			},
			{
				name: "număr_s",
				description: "este numărul limită de succese"
			},
			{
				name: "probabilitate_s",
				description: "este probabilitatea unui succes; un număr între 0 și 1"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "Returnează numărul total de zile lucrătoare între două date.",
		arguments: [
			{
				name: "dată_start",
				description: "este numărul de dată serial care reprezintă ziua de început"
			},
			{
				name: "dată_sfârșit",
				description: "este un număr de dată serial care reprezintă ziua de final"
			},
			{
				name: "sărbători",
				description: "este un set opțional de una sau mai multe numere de date seriale care se vor exclude din calendarul de lucru, cum ar fi sărbătorile legale"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "Returnează numărul total de zile lucrătoare între două date cu parametri sfârșit de săptămână particularizați.",
		arguments: [
			{
				name: "dată_start",
				description: "este numărul de dată serial care reprezintă ziua de început"
			},
			{
				name: "dată_sfârșit",
				description: "este un număr de dată serial care reprezintă ziua de final"
			},
			{
				name: "weekend",
				description: "este un număr sau un șir care specifică data sfârșiturilor de săptămână"
			},
			{
				name: "sărbători",
				description: "este un set opțional de una sau mai multe numere de date seriale care se vor exclude din calendarul de lucru, cum ar fi sărbătorile legale"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "Returnează rata anuală a dobânzii nominale.",
		arguments: [
			{
				name: "rată_efectivă",
				description: "este rata dobânzii efective"
			},
			{
				name: "n_per",
				description: "este numărul de perioade compuse dintr-un an"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "Returnează distribuția normală pentru media specificată și deviația standard.",
		arguments: [
			{
				name: "x",
				description: "este valoarea pentru care se face distribuția"
			},
			{
				name: "media",
				description: "este media aritmetică a distribuției"
			},
			{
				name: "dev_standard",
				description: "este deviația standard a densității, un număr pozitiv"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică: pentru funcția distribuție cumulativă, se utilizează TRUE; pentru funcția masă de probabilitate, se utilizează FALSE"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Returnează inversa distribuției cumulativ normale pentru media specificată și deviația standard.",
		arguments: [
			{
				name: "probabilitate",
				description: "este o probabilitate corespunzătoare distribuției normale, un număr între 0 și 1 inclusiv"
			},
			{
				name: "media",
				description: "este media aritmetică a distribuției"
			},
			{
				name: "dev_standard",
				description: "este deviația standard a distribuției, un număr pozitiv"
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "Returnează distribuția normală standard (are o medie de zero și o deviație standard de unu).",
		arguments: [
			{
				name: "z",
				description: "este valoarea pentru care se calculează distribuția"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică pentru ca funcția să returneze: funcția de distribuție cumulativă = TRUE; funcția de densitate a probabilității = FALSE"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Returnează inversa distribuției cumulativ normale standard (are o medie de zero și o deviație standard de unu).",
		arguments: [
			{
				name: "probabilitate",
				description: "este o probabilitate corespunzătoare distribuției normale, un număr între 0 și 1 inclusiv"
			}
		]
	},
	{
		name: "NORMDIST",
		description: "Returnează distribuția cumulativă normală pentru media și deviația standard specificate.",
		arguments: [
			{
				name: "x",
				description: "este valoarea pentru care se face distribuția"
			},
			{
				name: "media",
				description: "este media aritmetică a distribuției"
			},
			{
				name: "dev_standard",
				description: "este deviația standard a distribuției, un număr pozitiv"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică: pentru funcția distribuție cumulativă, se utilizează TRUE; pentru funcția densitatea probabilității, se utilizează FALSE"
			}
		]
	},
	{
		name: "NORMINV",
		description: "Returnează inversa distribuției cumulativ normale pentru media și deviația standard specificate.",
		arguments: [
			{
				name: "probabilitate",
				description: "este o probabilitate corespunzătoare distribuției normale, un număr între 0 și 1 inclusiv"
			},
			{
				name: "media",
				description: "este media aritmetică a distribuției"
			},
			{
				name: "dev_standard",
				description: "este deviația standard a distribuției, un număr pozitiv"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "Returnează distribuția cumulativă normală standard (are o medie de zero și o deviație standard de unu).",
		arguments: [
			{
				name: "z",
				description: "este valoarea pentru care se calculează distribuția"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "Returnează inversa distribuției cumulativ normale standard (are o medie de zero și o deviație standard de unu).",
		arguments: [
			{
				name: "probabilitate",
				description: "este o probabilitate corespunzătoare distribuției normale, un număr între 0 și 1 inclusiv"
			}
		]
	},
	{
		name: "NOT",
		description: "Modifică FALSE în TRUE sau TRUE în FALSE.",
		arguments: [
			{
				name: "logic",
				description: "este o valoare sau o expresie care se evaluează la TRUE sau FALSE"
			}
		]
	},
	{
		name: "NOW",
		description: "Returnează data și ora curente formatate ca dată și oră.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "Returnează numărul de perioade pentru o investiție bazată pe plăți constante periodice și o rată constantă a dobânzii.",
		arguments: [
			{
				name: "rată",
				description: "este rata dobânzii pe perioadă. De exemplu, utilizați 6%/4 pentru plăți trimestriale la o dobândă anuală de 6%"
			},
			{
				name: "pmt",
				description: "este plata făcută pentru fiecare perioadă; nu se modifică pe toată durata investiției"
			},
			{
				name: "pv",
				description: "este valoarea prezentă sau valoarea actuală a unui vărsământ unic corespunzător unei serii de plăți viitoare"
			},
			{
				name: "fv",
				description: "este valoarea viitoare sau balanța în numerar la care se ajunge după ce este făcută ultima plată. Dacă se omite, se utilizează zero"
			},
			{
				name: "tip",
				description: "este o valoare logică: plata la începutul perioadei = 1; plata la sfârșitul perioadei = 0 sau se omite"
			}
		]
	},
	{
		name: "NPV",
		description: "Returnează valoarea netă prezentă a unei investiții bazată pe o rată de actualizare și o serie de plăți viitoare (valori negative) și venituri  (valori pozitive).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rată",
				description: "este rata de actualizare pentru o perioadă"
			},
			{
				name: "valoare1",
				description: "sunt de la 1 la 254 de plăți și venituri, distanțate egal în timp și care au loc la sfârșitul fiecărei perioade"
			},
			{
				name: "valoare2",
				description: "sunt de la 1 la 254 de plăți și venituri, distanțate egal în timp și care au loc la sfârșitul fiecărei perioade"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "Efectuează conversia textului în număr într-o manieră independentă de setările regionale.",
		arguments: [
			{
				name: "text",
				description: "este șirul care reprezintă numărul căruia îi efectuați conversia"
			},
			{
				name: "separator_zecimale",
				description: "este caracterul utilizat ca zecimală în șir"
			},
			{
				name: "separator_grup",
				description: "este caracterul utilizat ca separator de grup în șir"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "Se efectuează conversia unui număr octal într-un număr binar.",
		arguments: [
			{
				name: "număr",
				description: "este numărul octal care va suferi conversia"
			},
			{
				name: "locuri",
				description: "este numărul de caractere de utilizat"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "Se efectuează conversia unui număr octal într-un număr zecimal.",
		arguments: [
			{
				name: "număr",
				description: "este numărul octal care va suferi conversia"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "Se efectuează conversia unui număr octal într-un număr hexazecimal.",
		arguments: [
			{
				name: "număr",
				description: "este numărul octal care va suferi conversia"
			},
			{
				name: "locuri",
				description: "este numărul de caractere de utilizat"
			}
		]
	},
	{
		name: "ODD",
		description: "Rotunjește prin adaos un număr pozitiv și prin lipsă un număr negativ la cel mai apropiat întreg impar.",
		arguments: [
			{
				name: "număr",
				description: "este valoarea de rotunjit"
			}
		]
	},
	{
		name: "OFFSET",
		description: "Returnează o referință la o zonă care este un număr precizat de rânduri și coloane dintr-o referință dată.",
		arguments: [
			{
				name: "referință",
				description: "este referința pe care se bazează decalajul, o referință la o celulă sau zonă de celule adiacente"
			},
			{
				name: "rânduri",
				description: "este numărul de rânduri, în jos sau în sus, la care va face referire celula din stânga sus a rezultatului"
			},
			{
				name: "col",
				description: "este numărul de coloane, la stânga sau la dreapta, la care va face referire celula din stânga sus a rezultatului"
			},
			{
				name: "înalt",
				description: "este înălțimea, în număr de rânduri, a rezultatului, aceeași înălțime cu referință dacă se omite"
			},
			{
				name: "lat",
				description: "este lățimea, în număr de coloane, a rezultatului, aceeași lățime cu referință dacă se omite"
			}
		]
	},
	{
		name: "OR",
		description: "Verifică dacă există argumente TRUE și întoarce TRUE sau FALSE. Returnează FALSE numai dacă toate argumentele sunt FALSE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logic1",
				description: "sunt de la 1 la 255 de condiții de testat care pot fi TRUE sau FALSE"
			},
			{
				name: "logic2",
				description: "sunt de la 1 la 255 de condiții de testat care pot fi TRUE sau FALSE"
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
		description: "Returnează numărul de perioade necesare pentru ca o investiție să atingă o valoare specificată.",
		arguments: [
			{
				name: "rată",
				description: "este rata dobânzii pentru fiecare perioadă."
			},
			{
				name: "pv",
				description: "este valoarea prezentă a investiției"
			},
			{
				name: "fv",
				description: "este valoarea viitoare dorită a investiției"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Returnează coeficientul Pearson de corelație a momentelor produselor, r.",
		arguments: [
			{
				name: "matrice1",
				description: "este un set de valori independente"
			},
			{
				name: "matrice2",
				description: "este un set de valori dependente"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "Returnează a k-a procentilă de valori dintr-o zonă.",
		arguments: [
			{
				name: "matrice",
				description: "este matricea sau zona de date care definește poziția relativă"
			},
			{
				name: "k",
				description: "este valoarea procentilei cuprinsă între 0 și 1, inclusiv"
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "Returnează a k-a procentilă de valori dintr-o zonă, unde k este intervalul 0..1, exclusiv.",
		arguments: [
			{
				name: "matrice",
				description: "este matricea sau zona de date care definește poziția relativă"
			},
			{
				name: "k",
				description: "este valoarea procentilei cuprinsă între 0 și 1, inclusiv"
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "Returnează a k-a procentilă de valori dintr-o zonă, unde k este intervalul 0..1, inclusiv.",
		arguments: [
			{
				name: "matrice",
				description: "este matricea sau zona de date care definește poziția relativă"
			},
			{
				name: "k",
				description: "este valoarea procentilei cuprinsă între 0 și 1, inclusiv"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "Returnează rangul unei valori dintr-un set de date ca procentaj din setul de date.",
		arguments: [
			{
				name: "matrice",
				description: "este matricea sau zona de date cu valori numerice care definesc poziția relativă"
			},
			{
				name: "x",
				description: "este valoarea pentru care se determină rangul"
			},
			{
				name: "semnificație",
				description: "este o valoare opțională care identifică numărul de cifre semnificative pentru procentajul întors, trei cifre dacă este omisă (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "Returnează rangul unei valori dintr-un set de date ca procentaj din setul de date (0..1, exclusiv).",
		arguments: [
			{
				name: "matrice",
				description: "este matricea sau zona de date cu valori numerice care definesc poziția relativă"
			},
			{
				name: "x",
				description: "este valoarea pentru care se determină rangul"
			},
			{
				name: "semnificație",
				description: "este o valoare opțională care identifică numărul de cifre semnificative pentru procentajul întors, trei cifre dacă este omisă (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "Returnează rangul unei valori dintr-un set de date ca procentaj din setul de date (0..1, inclusiv).",
		arguments: [
			{
				name: "matrice",
				description: "este matricea sau zona de date cu valori numerice care definesc poziția relativă"
			},
			{
				name: "x",
				description: "este valoarea pentru care se determină rangul"
			},
			{
				name: "semnificație",
				description: "este o valoare opțională care identifică numărul de cifre semnificative pentru procentajul întors, trei cifre dacă este omisă (0.xxx%)"
			}
		]
	},
	{
		name: "PERMUT",
		description: "Returnează numărul de permutări pentru un număr dat de obiecte care pot fi selectate din totalul de obiecte.",
		arguments: [
			{
				name: "număr",
				description: "este numărul total de obiecte"
			},
			{
				name: "număr_ales",
				description: "este numărul de obiecte din fiecare permutare"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Returnează numărul de permutări pentru un număr dat de obiecte (cu repetiții) care pot fi selectate din totalul de obiecte.",
		arguments: [
			{
				name: "număr",
				description: "este numărul total de obiecte"
			},
			{
				name: "număr_ales",
				description: "este numărul de obiecte din fiecare permutare"
			}
		]
	},
	{
		name: "PHI",
		description: "Returnează valoarea funcției de densitate pentru o distribuție normală standard.",
		arguments: [
			{
				name: "x",
				description: "este numărul pentru care doriți densitatea distribuției normale standard"
			}
		]
	},
	{
		name: "PI",
		description: "Returnează valoarea lui Pi, 3,14159265358979 cu precizie de 15 cifre.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "Calculează plata pentru un împrumut bazat pe plăți constante și o rată constantă a dobânzii.",
		arguments: [
			{
				name: "rată",
				description: "este rata dobânzii pentru împrumut pe o perioadă. De exemplu, utilizați 6%/4 pentru plăți trimestriale la o dobândă anuală de 6%"
			},
			{
				name: "nper",
				description: "este numărul total de plăți pentru împrumut"
			},
			{
				name: "pv",
				description: "este valoarea prezentă: valoarea totală actuală a unei serii de plăți viitoare"
			},
			{
				name: "fv",
				description: "este valoarea viitoare sau balanța în numerar la care se ajunge după ce este făcută ultima plată, 0 (zero) dacă se omite"
			},
			{
				name: "tip",
				description: "este o valoare logică: plata la începutul perioadei = 1; plata la sfârșitul perioadei = 0 sau se omite"
			}
		]
	},
	{
		name: "POISSON",
		description: "Returnează distribuția Poisson.",
		arguments: [
			{
				name: "x",
				description: "este numărul de evenimente"
			},
			{
				name: "media",
				description: "este valoarea numerică așteptată, un număr pozitiv"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică: pentru probabilitatea Poisson cumulativă, se utilizează TRUE; pentru funcția probabilitate de masă Poisson, se utilizează FALSE"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "Returnează distribuția Poisson.",
		arguments: [
			{
				name: "x",
				description: "este numărul de evenimente"
			},
			{
				name: "media",
				description: "este valoarea numerică așteptată, un număr pozitiv"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică: pentru probabilitatea Poisson cumulativă, se utilizează TRUE; pentru funcția masă de probabilitate Poisson, se utilizează FALSE"
			}
		]
	},
	{
		name: "POWER",
		description: "Returnează rezultatul unui număr ridicat la o putere.",
		arguments: [
			{
				name: "număr",
				description: "este numărul bază, orice număr real"
			},
			{
				name: "exponent",
				description: "este exponentul, la care este ridicat numărul bază"
			}
		]
	},
	{
		name: "PPMT",
		description: "Returnează plata efectivă pentru o investiție dată bazată pe plăți constante periodice și o rată constantă a dobânzii.",
		arguments: [
			{
				name: "rată",
				description: "este rata dobânzii pe o perioadă. De exemplu, utilizați 6%/4 pentru plăți trimestriale la o dobândă anuală de 6%"
			},
			{
				name: "per",
				description: "specifică perioada și este în intervalul de la 1 la Nper"
			},
			{
				name: "nper",
				description: "este numărul total de perioade de plată dintr-o investiție"
			},
			{
				name: "pv",
				description: "este valoarea prezentă: valoarea totală a unui vărsământ unic corespunzător unei serii de plăți viitoare"
			},
			{
				name: "fv",
				description: "este valoarea viitoare sau balanța în numerar la care se ajunge după ce este făcută ultima plată"
			},
			{
				name: "tip",
				description: "este o valoare logică: plata la începutul perioadei = 1; plata la sfârșitul perioadei = 0 sau se omite"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "Returnează prețul la 100 lei valoare reală dintr-o asigurare cu reducere.",
		arguments: [
			{
				name: "tranzacție",
				description: "este data de lichidare a asigurării, exprimată ca număr serial"
			},
			{
				name: "maturitate",
				description: "este data de maturitate a asigurării, exprimată ca număr serial"
			},
			{
				name: "reducere",
				description: "este rata de reducere a asigurării"
			},
			{
				name: "rambursare",
				description: "este rata de recuperare a asigurării la 100 lei valoare reală"
			},
			{
				name: "bază",
				description: "este tipul de bază de numărare a zilelor care trebuie utilizat"
			}
		]
	},
	{
		name: "PROB",
		description: "Returnează probabilitatea ca valorile dintr-o zonă să fie între două limite sau egale cu limita inferioară.",
		arguments: [
			{
				name: "zonă_x",
				description: "este zona de valori numerice x cu care sunt asociate probabilitățile"
			},
			{
				name: "zonă_prob",
				description: "este setul de probabilități asociate cu valori din zonă_x, valori între 0 și 1 și excluzând 0"
			},
			{
				name: "limită_inf",
				description: "este limita inferioară a valorii pentru care se calculează probabilitatea"
			},
			{
				name: "limită_sup",
				description: "este opțional limita superioară a valorii. Dacă este omisă, PROB returnează probabilitatea ca valorile din zonă_inf să fie egale cu limită_inf"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "Înmulțește toate numerele date ca argumente.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 până la 255 de numere, valori logice sau reprezentări text ale numerelor de înmulțit"
			},
			{
				name: "număr2",
				description: "sunt de la 1 până la 255 de numere, valori logice sau reprezentări text ale numerelor de înmulțit"
			}
		]
	},
	{
		name: "PROPER",
		description: "Scrie cu majusculă prima literă din fiecare cuvânt al unui șir text și transformă toate celelalte litere în litere mici.",
		arguments: [
			{
				name: "text",
				description: "este text încadrat în ghilimele, o formulă care returnează text sau o referință la o celulă care conține text pentru scriere parțială cu majuscule"
			}
		]
	},
	{
		name: "PV",
		description: "Returnează valoarea prezentă a unei investiții: valoarea totală actuală a unei serii de plăți viitoare.",
		arguments: [
			{
				name: "rată",
				description: "este rata dobânzii pe o perioadă. De exemplu, utilizați 6%/4 pentru plăți trimestriale la o dobândă anuală de 6%"
			},
			{
				name: "nper",
				description: "este numărul total de perioade de plată dintr-o investiție"
			},
			{
				name: "pmt",
				description: "este plata făcută în fiecare perioadă și nu se modifică pe durata investiției"
			},
			{
				name: "fv",
				description: "este valoarea viitoare sau balanța în numerar la care se ajunge după ce este făcută ultima plată"
			},
			{
				name: "tip",
				description: "este o valoare logică: plata la începutul perioadei = 1; plata la sfârșitul perioadei = 0 sa omisă"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "Returnează cuartila unui set de date.",
		arguments: [
			{
				name: "matrice",
				description: "este matricea sau zona de celule de valori numerice pentru care se determină valoarea cuartilei"
			},
			{
				name: "quart",
				description: "este un număr: valoarea minimă = 0; 1-a cuartilă = 1; valoarea mediană = 2; a 3-a cuartilă = 3; valoarea maximă = 4"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "Returnează cuartila unui set de date în baza valorile procentilei din 0..1, exclusiv.",
		arguments: [
			{
				name: "matrice",
				description: "este matricea sau zona de celule de valori numerice pentru care se determină valoarea cuartilei"
			},
			{
				name: "quart",
				description: "este un număr: valoarea minimă = 0; 1-a cuartilă = 1; valoarea mediană = 2; a 3-a cuartilă = 3; valoarea maximă = 4"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "Returnează cuartila unui set de date în baza valorile procentilei din 0..1, inclusiv.",
		arguments: [
			{
				name: "matrice",
				description: "este matricea sau zona de celule de valori numerice pentru care se determină valoarea cuartilei"
			},
			{
				name: "quart",
				description: "este un număr: valoarea minimă = 0; 1-a cuartilă = 1; valoarea mediană = 2; a 3-a cuartilă = 3; valoarea maximă = 4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Returnează porțiunea întreagă a unei împărțiri.",
		arguments: [
			{
				name: "numărător",
				description: "este împărțitorul"
			},
			{
				name: "numitor",
				description: "este deîmpărțitul"
			}
		]
	},
	{
		name: "RADIANS",
		description: "Transformă gradele în radiani.",
		arguments: [
			{
				name: "unghi",
				description: "este unghiul de transformat exprimat în grade"
			}
		]
	},
	{
		name: "RAND",
		description: "Returnează un număr aleator mai mare sau egal cu 0 și mai mic decât 1, distribuit uniform (se modifică la recalculare).",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "Returnează un număr aleatoriu dintre numerele specificate.",
		arguments: [
			{
				name: "inf",
				description: "este cel mai mic număr întreg care va fi returnat de RANDBETWEEN"
			},
			{
				name: "sup",
				description: "este cel mai mare număr întreg care va fi returnat de RANDBETWEEN"
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
		description: "Returnează rangul unui număr dintr-o listă de numere: este dimensiunea relativ la alte valori din listă.",
		arguments: [
			{
				name: "număr",
				description: "este numărul pentru care se găsește rangul"
			},
			{
				name: "ref",
				description: "este o matrice, o referință, o listă de numere. Valorile nenumerice sunt ignorate"
			},
			{
				name: "ordine",
				description: "este un număr: rang în lista sortată descendent = 0 sau omis; rang în lista sortată ascendent = orice valoare diferită de zero"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "Returnează rangul unui număr dintr-o listă de numere: este dimensiunea relativ la alte valori din listă; dacă mai multe valori au același rang, se returnează rangul mediu.",
		arguments: [
			{
				name: "număr",
				description: "este numărul pentru care se găsește rangul"
			},
			{
				name: "ref",
				description: "este o matrice de sau referință la o listă de numere. Valorile nenumerice sunt ignorate"
			},
			{
				name: "ordine",
				description: "este un număr: rang în lista sortată descendent = 0 sau omis; rang în lista sortată ascendent = orice valoare diferită de zero"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "Returnează rangul unui număr dintr-o listă de numere: este dimensiunea relativ la alte valori din listă; dacă mai multe valori au același rang, se returnează rangul maxim al acelui set de valori.",
		arguments: [
			{
				name: "număr",
				description: "este numărul pentru care se găsește rangul"
			},
			{
				name: "ref",
				description: "este o matrice de sau referință la o listă de numere. Valorile nenumerice sunt ignorate"
			},
			{
				name: "ordine",
				description: "este un număr: rang în lista sortată descendent = 0 sau omis; rang în lista sortată ascendent = orice valoare diferită de zero"
			}
		]
	},
	{
		name: "RATE",
		description: "Returnează rata dobânzii pe perioadă pentru un împrumut sau o investiție. De exemplu, utilizați 6%/4 pentru plăți trimestriale la o dobândă anuală de 6%.",
		arguments: [
			{
				name: "nper",
				description: "este numărul total de perioade de plată pentru împrumut sau investiție"
			},
			{
				name: "pmt",
				description: "este plata făcută în fiecare perioadă și nu se modifică pe toată durata împrumutului sau investiției"
			},
			{
				name: "pv",
				description: "este valoarea prezentă: valoarea totală actuală a unei serii de plăți viitoare"
			},
			{
				name: "fv",
				description: "este valoarea viitoare sau balanța în numerar la care se ajunge după ce este făcută ultima plată. Dacă se omite, Fv = 0"
			},
			{
				name: "tip",
				description: "este o valoare logică: plata la începutul perioadei = 1; plata la sfârșitul perioadei = 0 sau se omite"
			},
			{
				name: "estim",
				description: "este propria aproximare a ratei; dacă se omite, estim = 0,1 (10 procente)"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "Returnează suma primită la maturitate pentru o asigurare plătită integral.",
		arguments: [
			{
				name: "tranzacție",
				description: "este data de lichidare a asigurării, exprimată ca număr serial"
			},
			{
				name: "maturitate",
				description: "este data de maturitate a asigurării, exprimată ca număr serial"
			},
			{
				name: "investiție",
				description: "este suma investită în asigurare"
			},
			{
				name: "reducere",
				description: "este rata de reducere"
			},
			{
				name: "bază",
				description: "este  tipul de bază de numărare a zilelor care trebuie utilizat"
			}
		]
	},
	{
		name: "REPLACE",
		description: "Înlocuiește o parte a unui șir text cu un șir text diferit.",
		arguments: [
			{
				name: "text_vechi",
				description: "este textul în care se înlocuiesc unele caractere"
			},
			{
				name: "num_start",
				description: "este poziția caracterului din text_vechi care se înlocuiește cu text_nou"
			},
			{
				name: "car_num",
				description: "este numărul de caractere din text_vechi care se înlocuiește"
			},
			{
				name: "text_nou",
				description: "este textul care va înlocui caracterele din text_vechi"
			}
		]
	},
	{
		name: "REPT",
		description: "Repetă un text de un număr de ori dat. Utilizați REPT pentru a umple o celulă cu un număr de instanțe ale unui șir text.",
		arguments: [
			{
				name: "text",
				description: "este textul de repetat"
			},
			{
				name: "număr_ori",
				description: "este un număr pozitiv care precizează de câte ori se repetă textul"
			}
		]
	},
	{
		name: "RIGHT",
		description: "Returnează numărul precizat de caractere de la sfârșitul unui șir text.",
		arguments: [
			{
				name: "text",
				description: "este șirul text care conține caracterele de extras"
			},
			{
				name: "car_num",
				description: "precizează numărul de caractere de extras, 1 dacă se omite"
			}
		]
	},
	{
		name: "ROMAN",
		description: "Transformă un numeral arab în roman, ca text.",
		arguments: [
			{
				name: "număr",
				description: "este numeralul arab de transformat"
			},
			{
				name: "formă",
				description: "este numărul care precizează tipul de numeral roman dorit."
			}
		]
	},
	{
		name: "ROUND",
		description: "Rotunjește un număr la un număr specificat de cifre.",
		arguments: [
			{
				name: "număr",
				description: "este numărul de rotunjit"
			},
			{
				name: "num_cifre",
				description: "este numărul de cifre la care se face rotunjirea. num_cifre negativ rotunjește la stânga separatorului zecimal; zero la cel mai apropiat întreg"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "Rotunjește un număr prin lipsă, spre zero.",
		arguments: [
			{
				name: "număr",
				description: "este orice număr real care se rotunjește prin lipsă"
			},
			{
				name: "num_cifre",
				description: "este numărul de cifre la care se face rotunjirea. num_cifre negativ rotunjește la stânga separatorului zecimal; zero sau omis, rotunjește la cel mai apropiat întreg"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "Rotunjește un număr prin adaos, dinspre zero.",
		arguments: [
			{
				name: "număr",
				description: "este orice număr real care se rotunjește prin adaos"
			},
			{
				name: "num_cifre",
				description: "este numărul de cifre la care se face rotunjirea. num_cifre negativ rotunjește la stânga separatorului zecimal; zero sau omis, rotunjește la cel mai apropiat întreg"
			}
		]
	},
	{
		name: "ROW",
		description: "Returnează numărul rândului pentru o referință.",
		arguments: [
			{
				name: "referință",
				description: "este celula sau o singură zonă de celule pentru care se calculează numărul rândului; dacă este omisă, returnează celula care conține funcția ROW"
			}
		]
	},
	{
		name: "ROWS",
		description: "Returnează numărul de rânduri dintr-o referință sau matrice.",
		arguments: [
			{
				name: "matrice",
				description: "este o matrice, o formulă matrice, sau o referință la o zonă de celule pentru care se calculează numărul de rânduri"
			}
		]
	},
	{
		name: "RRI",
		description: "Returnează o rată a dobânzii echivalentă pentru creșterea unei investiții.",
		arguments: [
			{
				name: "nper",
				description: "este numărul perioadelor pentru investiție"
			},
			{
				name: "pv",
				description: "este valoarea prezentă a investiției"
			},
			{
				name: "fv",
				description: "este valoarea viitoare a investiției"
			}
		]
	},
	{
		name: "RSQ",
		description: "Returnează pătratul coeficientului Pearson de corelație moment produs printre punctele de date cunoscute.",
		arguments: [
			{
				name: "cunoscute_y",
				description: "este o matrice sau o zonă de puncte de date și pot fi numere sau nume, matrice, sau referința care conțin numere"
			},
			{
				name: "cunoscute_x",
				description: "este o matrice sau zonă de puncte de date și pot fi numere sau nume, matrice, sau referințe care conțin numere"
			}
		]
	},
	{
		name: "RTD",
		description: "Preia date în timp real de la un program care acceptă automatizare COM.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "este numele identificatorului de program pentru o componentă de automatizare COM inclusă la cerere, care este înregistrată. Includeți numele între ghilimele"
			},
			{
				name: "server",
				description: "este numele serverului unde se va  rula programul de completare. Includeți numele între ghilimele. Dacă programul de completare rulează local, utilizați un șir gol"
			},
			{
				name: "subiect1",
				description: "sunt 1 până la 38 parametri care specifică o parte din date"
			},
			{
				name: "subiect2",
				description: "sunt 1 până la 38 parametri care specifică o parte din date"
			}
		]
	},
	{
		name: "SEARCH",
		description: "Returnează numărul caracterului de la care este găsit prima dată un caracter sau un șir text precizat, citind de la stânga la dreapta (nu se diferențiază literele mari și mici).",
		arguments: [
			{
				name: "text_de_căutat",
				description: "este textul de găsit. Utilizați metacaracterele ? și *; utilizați ~? și ~* pentru a găsi caracterele ? și *"
			},
			{
				name: "în_text",
				description: "este textul în care se caută text_de_căutat"
			},
			{
				name: "num_start",
				description: "este numărul caracterului din în_text, numărând de la stânga, de la care se începe căutarea. Dacă este omis, se utilizează 1"
			}
		]
	},
	{
		name: "SEC",
		description: "Returnează secanta unui unghi.",
		arguments: [
			{
				name: "număr",
				description: "este unghiul în radiani pentru care se calculează secanta"
			}
		]
	},
	{
		name: "SECH",
		description: "Returnează secanta hiperbolică a unui unghi.",
		arguments: [
			{
				name: "număr",
				description: "este unghiul în radiani pentru care se calculează secanta hiperbolică"
			}
		]
	},
	{
		name: "SECOND",
		description: "Returnează secunda ca număr în intervalul de la 0 la 59.",
		arguments: [
			{
				name: "număr_serie",
				description: "este un număr în cod dată-oră utilizat de Spreadsheet sau text în format de oră, cum ar fi 16:48:23 sau 4:48:47 PM"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "Returnează suma unor serii de puteri bazate pe formulă.",
		arguments: [
			{
				name: "x",
				description: "este valoarea de intrare a seriilor de puteri"
			},
			{
				name: "n",
				description: "este puterea inițială la care se ridică x"
			},
			{
				name: "m",
				description: "este pasul cu care se mărește n pentru fiecare termen din serie"
			},
			{
				name: "coeficienți",
				description: "este un set de coeficienți prin care fiecare putere succesivă a lui x este multiplicată"
			}
		]
	},
	{
		name: "SHEET",
		description: "Returnează numărul de foaie al foii menționate.",
		arguments: [
			{
				name: "valoare",
				description: "este numele unei foi sau al unei referințe al cărei număr de foaie îl doriți. Dacă este omis, se returnează numărul de foaie ce conține funcția"
			}
		]
	},
	{
		name: "SHEETS",
		description: "Returnează numărul de foi dintr-o referință.",
		arguments: [
			{
				name: "referință",
				description: "este o referință pentru care doriți să aflați numărul de foi. Dacă este omis, se returnează numărul de foi din registrul de lucru"
			}
		]
	},
	{
		name: "SIGN",
		description: "Returnează semnul unui număr: 1 dacă numărul este pozitiv, zero dacă numărul este zero, sau -1 dacă numărul este negativ.",
		arguments: [
			{
				name: "număr",
				description: "este orice număr real"
			}
		]
	},
	{
		name: "SIN",
		description: "Returnează sinusul unui unghi.",
		arguments: [
			{
				name: "număr",
				description: "este unghiul în radiani pentru care se calculează sinusul. Grade * PI()/180 = radiani"
			}
		]
	},
	{
		name: "SINH",
		description: "Returnează sinusul hiperbolic al unui număr.",
		arguments: [
			{
				name: "număr",
				description: "este orice număr real"
			}
		]
	},
	{
		name: "SKEW",
		description: "Returnează panta unei distribuții: o caracterizare a gradului de asimetrie a unei distribuții în jurul mediei sale.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere sau de nume, matrice sau referințe care conțin numere pentru care se calculează panta"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere sau de nume, matrice sau referințe care conțin numere pentru care se calculează panta"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "Returnează panta unei distribuții pe baza unei populații: o caracterizare a gradului de asimetrie a unei distribuții în jurul mediei sale.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 254 de numere sau de nume, matrice sau referințe care conțin numere pentru care se calculează panta de populație"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 254 de numere sau de nume, matrice sau referințe care conțin numere pentru care se calculează panta de populație"
			}
		]
	},
	{
		name: "SLN",
		description: "Returnează amortizarea liniară a unui mijloc fix pe o perioadă.",
		arguments: [
			{
				name: "cost",
				description: "este costul inițial al mijlocului fix"
			},
			{
				name: "recuperat",
				description: "este valoarea rămasă la sfârșitul duratei de viață a mijlocului fix"
			},
			{
				name: "viață",
				description: "este numărul de perioade în care se amortizează mijlocul fix (uneori numită durata de utilizare)"
			}
		]
	},
	{
		name: "SLOPE",
		description: "Returnează panta unei linii de regresie liniară printre punctele de date cunoscute.",
		arguments: [
			{
				name: "cunoscute_y",
				description: "este o matrice sau zonă de celule de puncte de date numerice dependente și pot fi numere sau nume, matrice, sau referințe care conțin numere"
			},
			{
				name: "cunoscute_x",
				description: "este setul de puncte de date independente și pot fi numere sau nume, matrice, sau referințe care conțin numere"
			}
		]
	},
	{
		name: "SMALL",
		description: "Returnează a k-a din cele mai mici valori dintr-un set de date. De exemplu, al cincilea dintre cele mai mici numere.",
		arguments: [
			{
				name: "matrice",
				description: "este o matrice sau zonă de date numerice pentru care se determină cea mai mică a k-a valoare"
			},
			{
				name: "k",
				description: "este poziția valorii de întors (față de cea mai mică) din matrice sau din zonă"
			}
		]
	},
	{
		name: "SQRT",
		description: "Returnează rădăcina pătrată a unui număr.",
		arguments: [
			{
				name: "număr",
				description: "este numărul pentru care se calculează rădăcina pătrată"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "Returnează rădăcina pătrată a (numărului * Pi).",
		arguments: [
			{
				name: "număr",
				description: "este numărul cu care p este multiplicat"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "Returnează o valoare normalizată dintr-o distribuție caracterizată de o medie și o deviație standard.",
		arguments: [
			{
				name: "x",
				description: "este valoarea de normalizat"
			},
			{
				name: "media",
				description: "este media aritmetică a distribuției"
			},
			{
				name: "dev_standard",
				description: "este deviația standard a distribuției, un număr pozitiv"
			}
		]
	},
	{
		name: "STDEV",
		description: "Estimează deviația standard pe baza unui eșantion (ignoră valorile logice și textul din eșantion).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere corespunzătoare unui eșantion de populație și pot fi numere sau referințe care conțin numere"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere corespunzătoare unui eșantion de populație și pot fi numere sau referințe care conțin numere"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "Calculează deviația standard bazată pe întreaga populație dată ca argumente (ignoră valorile logice și text).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere care corespund unei populații și pot fi numere sau referințe care conțin numere"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere care corespund unei populații și pot fi numere sau referințe care conțin numere"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "Estimează deviația standard bazată pe un eșantion (ignoră valorile logice și text din  eșantion).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere corespunzătoare unui eșantion de populație și pot fi numere sau referințe care conțin numere"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere corespunzătoare unui eșantion de populație și pot fi numere sau referințe care conțin numere"
			}
		]
	},
	{
		name: "STDEVA",
		description: "Estimează deviația standard bazat pe un eșantion, incluzând valori logice și text. Textul și valoarea logică FALSE au valoarea 0; valoarea logică TRUE are valoarea 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valoare1",
				description: "sunt de la 1 la 255 de valori care corespund unui eșantion de populație și pot fi valori sau nume sau referințe la valori"
			},
			{
				name: "valoare2",
				description: "sunt de la 1 la 255 de valori care corespund unui eșantion de populație și pot fi valori sau nume sau referințe la valori"
			}
		]
	},
	{
		name: "STDEVP",
		description: "Calculează deviația standard pe baza populației totale dată ca argumente (ignoră valorile logice și textul).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere care corespund unei populații și pot fi numere sau referințe care conțin numere"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere care corespund unei populații și pot fi numere sau referințe care conțin numere"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Calculează deviația standard pe baza întregii populații, incluzând valori logice și text. Textul și valoarea logică FALSE au valoarea 0; valoarea logică TRUE are valoarea 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valoare1",
				description: "sunt de la 1 la 255 de valori care corespund unei populații și pot fi valori, nume, matrice sau referințe ce conțin valori"
			},
			{
				name: "valoare2",
				description: "sunt de la 1 la 255 de valori care corespund unei populații și pot fi valori, nume, matrice sau referințe ce conțin valori"
			}
		]
	},
	{
		name: "STEYX",
		description: "Returnează eroarea standard a valorii y prezise pentru fiecare x dintr-o regresie.",
		arguments: [
			{
				name: "cunoscute_y",
				description: "este o matrice sau o zonă de puncte de date dependente și pot fi numere sau nume, matrice, sau referințe care conțin numere"
			},
			{
				name: "cunoscute_x",
				description: "este o matrice sau zonă de puncte de date independente și pot fi numere sau nume, matrice, sau referințe care conțin numere"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "Înlocuiește textul existent cu text nou într-un șir text.",
		arguments: [
			{
				name: "text",
				description: "este textul sau referința la o celulă care conține textul în care se substituie caracterele"
			},
			{
				name: "text_vechi",
				description: "este textul existent de înlocuit. Dacă literele mari și mici din text_vechi nu se regăsesc în text, SUBSTITUTE nu va înlocui textul"
			},
			{
				name: "text_nou",
				description: "este textul care înlocuiește text_vechi"
			},
			{
				name: "num_instanță",
				description: "specifică ce instanță a text_vechi se înlocuiește. Dacă este omis, fiecare instanță a text_vechi este înlocuită"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Returnează un subtotal într-o listă sau bază de date.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num_funcție",
				description: "este numărul de la 1 la 11 care specifică funcția rezumativă pentru subtotal."
			},
			{
				name: "ref1",
				description: "sunt de la 1 la 254 de zone sau referințe pentru care se calculează subtotalul"
			}
		]
	},
	{
		name: "SUM",
		description: "Adună toate numerele dintr-o zonă de celule.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere de însumat. Valorile logice și textul sunt ignorate în celule, inclusiv dacă sunt tastate ca argumente"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere de însumat. Valorile logice și textul sunt ignorate în celule, inclusiv dacă sunt tastate ca argumente"
			}
		]
	},
	{
		name: "SUMIF",
		description: "Adună celulele specificate de o condiție sau criteriu dat.",
		arguments: [
			{
				name: "zonă",
				description: "este zona de celule care se evaluează"
			},
			{
				name: "criterii",
				description: "este condiția sau criteriul de forma unui număr, expresie, sau text care definește celulele care se adună"
			},
			{
				name: "zonă_sumă",
				description: "sunt celulele actuale de însumat. Dacă este omisă, sunt utilizate celulele din zonă"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "Adaugă celulele specificate dintr-un set de condiții sau criterii.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "zonă_sumă",
				description: "sunt celulele care trebuie adunate."
			},
			{
				name: "zonă_criterii",
				description: "este intervalul de celule de evaluat pentru o anumită stare"
			},
			{
				name: "criterii",
				description: "este condiția sau criteriul sub forma unui număr, a unei expresii sau a unui text care definește care celule se vor adăuga"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "Returnează suma produselor zonelor sau matricelor corespondente.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "matrice1",
				description: "sunt de la 2 la 255 de matrice pentru care se înmulțesc și se adună componente. Toate matricele trebuie să aibă aceleași dimensiuni"
			},
			{
				name: "matrice2",
				description: "sunt de la 2 la 255 de matrice pentru care se înmulțesc și se adună componente. Toate matricele trebuie să aibă aceleași dimensiuni"
			},
			{
				name: "matrice3",
				description: "sunt de la 2 la 255 de matrice pentru care se înmulțesc și se adună componente. Toate matricele trebuie să aibă aceleași dimensiuni"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "Returnează suma pătratelor argumentelor. Argumentele pot fi numere, matrice, nume sau referințe la celule care conțin numere.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de numere, matrice, nume sau referințe la matrice pentru care se calculează suma pătratelor"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de numere, matrice, nume sau referințe la matrice pentru care se calculează suma pătratelor"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "Însumează diferențele pătratelor a două zone sau matrice corespondente.",
		arguments: [
			{
				name: "matrice_x",
				description: "este prima zonă sau matrice de numere și este un număr sau un nume, o matrice sau o referință ce conține numere"
			},
			{
				name: "matrice_y",
				description: "este a doua zonă sau matrice de numere și este un număr sau un nume, o matrice sau o referință ce conține numere"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "Returnează suma totală a sumelor pătratelor numerelor din două zone sau matrice corespondente.",
		arguments: [
			{
				name: "matrice_x",
				description: "este prima zonă sau matrice de numere și este un număr sau un nume, o matrice sau o referință ce conține numere"
			},
			{
				name: "matrice_y",
				description: "este a doua zonă sau matrice de numere și este un număr sau un nume, o matrice sau o referință ce conține numere"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "Însumează pătratele diferențelor dintre două zone sau matrice corespondente.",
		arguments: [
			{
				name: "matrice_x",
				description: "este prima zonă sau matrice de valori și este un număr sau un nume, o matrice sau o referință ce conține numere"
			},
			{
				name: "matrice_y",
				description: "este a doua zonă sau matrice de valori și este un număr sau un nume, o matrice sau o referință ce conține numere"
			}
		]
	},
	{
		name: "SYD",
		description: "Returnează amortizarea în regresie aritmetică a unui mijloc fix pentru o perioadă specificată.",
		arguments: [
			{
				name: "cost",
				description: "este costul inițial al mijlocului fix"
			},
			{
				name: "recuperat",
				description: "este valoarea rămasă la sfârșitul duratei de viață a mijlocului fix"
			},
			{
				name: "viață",
				description: "este numărul de perioade în care se amortizează mijlocul fix (uneori numită durata de utilizare)"
			},
			{
				name: "per",
				description: "este perioada și trebuie să fie măsurată în aceleași unități ca viață"
			}
		]
	},
	{
		name: "T",
		description: "Verifică dacă o valoare este text și, în caz că este, returnează text, iar în caz contrar, returnează ghilimele duble (text gol).",
		arguments: [
			{
				name: "valoare",
				description: "este valoarea de testat"
			}
		]
	},
	{
		name: "T.DIST",
		description: "Returnează distribuția din coada stângă t-student.",
		arguments: [
			{
				name: "x",
				description: "este valoarea numerică la care se evaluează distribuția"
			},
			{
				name: "grade_libertate",
				description: "este un număr întreg care indică numărul de grade de libertate care caracterizează distribuția"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică: pentru funcția de distribuție cumulativă, se utilizează TRUE; pentru funcția de densitate de probabilitate, se utilizează FALSE"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "Returnează distribuția t-student cu două cozi.",
		arguments: [
			{
				name: "x",
				description: "este valoarea numerică la care să evaluați distribuția"
			},
			{
				name: "grade_libertate",
				description: "este un număr întreg care indică numărul de grade de libertate care caracterizează distribuția"
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "Returnează distribuția t-student a cozii din stânga.",
		arguments: [
			{
				name: "x",
				description: "este valoarea numerică la care să evaluați distribuția"
			},
			{
				name: "grade_libertate",
				description: "este un număr întreg care indică numărul de grade de libertate care caracterizează distribuția"
			}
		]
	},
	{
		name: "T.INV",
		description: "Returnează inversa distribuției t Student a cozii din dreapta.",
		arguments: [
			{
				name: "probabilitate",
				description: "este probabilitatea asociată cu distribuția t Student bilaterală, un număr între 0 și 1 inclusiv"
			},
			{
				name: "grade_libertate",
				description: "este un întreg pozitiv indicând numărul de grade de libertate care caracterizează distribuția"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Returnează inversa cu două cozi a distribuției t Student.",
		arguments: [
			{
				name: "probabilitate",
				description: "este probabilitatea asociată cu distribuția t Student bilaterală, un număr între 0 și 1 inclusiv"
			},
			{
				name: "grade_libertate",
				description: "este un întreg pozitiv indicând numărul de grade de libertate care caracterizează distribuția"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Returnează probabilitatea asociată cu un test t Student.",
		arguments: [
			{
				name: "matrice1",
				description: "este primul set de date"
			},
			{
				name: "matrice2",
				description: "este al doilea set de date"
			},
			{
				name: "cozi",
				description: "specifică tipul distribuției întoarse: distribuție unilaterală = 1; distribuție bilaterală = 2"
			},
			{
				name: "tip",
				description: "este felul de test t: pereche = 1, două eșantioane varianță egală (homoscedastică) = 2, două eșantioane varianță inegală = 3"
			}
		]
	},
	{
		name: "TAN",
		description: "Returnează tangenta unui unghi.",
		arguments: [
			{
				name: "număr",
				description: "este unghiul în radiani pentru care se calculează tangenta. Grade * PI()/180 = radiani"
			}
		]
	},
	{
		name: "TANH",
		description: "Returnează tangenta hiperbolică a unui număr.",
		arguments: [
			{
				name: "număr",
				description: "este orice număr real"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "Returnează produsul echivalent cu un bond pentru un certificat de trezorerie.",
		arguments: [
			{
				name: "tranzacție",
				description: "este data de lichidare a certificatului de trezorerie, exprimată ca număr serial"
			},
			{
				name: "maturitate",
				description: "este data de maturitate a certificatului de trezorerie, exprimată ca număr serial"
			},
			{
				name: "reducere",
				description: "este rata de reducere a certificatului de trezorerie"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "Returnează prețul pentru 100 lei valoare reală a unui certificat de trezorerie.",
		arguments: [
			{
				name: "tranzacție",
				description: "este data de lichidare a certificatului de trezorerie, exprimată ca număr serial"
			},
			{
				name: "maturitate",
				description: "este data de maturitate a certificatului de trezorerie, exprimată ca număr serial"
			},
			{
				name: "reducere",
				description: "este rata de reducere a certificatului de trezorerie"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "Returnează produsul pentru un certificat de trezorerie.",
		arguments: [
			{
				name: "tranzacție",
				description: "este data de lichidare a certificatului de trezorerie, exprimată ca număr serial"
			},
			{
				name: "maturitate",
				description: "este data de maturitate a certificatului de trezorerie, exprimată ca număr serial"
			},
			{
				name: "pr",
				description: "este prețul certificatului de trezorerie la 100 lei valoare reală"
			}
		]
	},
	{
		name: "TDIST",
		description: "Returnează distribuția t Student.",
		arguments: [
			{
				name: "x",
				description: "este valoarea numerică la care se evaluează distribuția"
			},
			{
				name: "grade_libertate",
				description: "este un întreg indicând numărul de grade de libertate ce caracterizează distribuția"
			},
			{
				name: "cozi",
				description: "specifică numărul de cozi ale distribuției de returnat: distribuție unilaterală = 1; distribuție bilaterală = 2"
			}
		]
	},
	{
		name: "TEXT",
		description: "Transformă o valoare în text cu un format de număr precizat.",
		arguments: [
			{
				name: "valoare",
				description: "este un număr, o formulă care evaluează o valoare numerică, sau o referință la o celulă care conține o valoare numerică"
			},
			{
				name: "format_text",
				description: "este un format de număr în formă de text din caseta Categorie de pe fila Număr din caseta de dialog Formatare celule (nu General)"
			}
		]
	},
	{
		name: "TIME",
		description: "Transformă ore, minut și secunde date ca numere în numere seriale Spreadsheet, formatate cu un format de oră.",
		arguments: [
			{
				name: "oră",
				description: "este un număr de la 0 la 23 reprezentând ora"
			},
			{
				name: "minut",
				description: "este un număr de la 0 la 59 reprezentând minutul"
			},
			{
				name: "secundă",
				description: "este un număr de la 0 la 59 reprezentând secunda"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "Transformă o oră din format text într-un număr serial Spreadsheet pentru oră, un număr de la 0 (12:00:00 AM) la 0,999988426 (11:59:59 PM). Formatați numărul cu format de oră după introducerea formulei.",
		arguments: [
			{
				name: "text_oră",
				description: "este un șir text care dă o oră în orice format de oră Spreadsheet (informațiile despre dată din șir se ignoră)"
			}
		]
	},
	{
		name: "TINV",
		description: "Returnează inversa distribuției t Student.",
		arguments: [
			{
				name: "probabilitate",
				description: "este probabilitatea asociată cu distribuția t Student bilaterală, un număr între 0 și 1 inclusiv"
			},
			{
				name: "grade_libertate",
				description: "este un întreg pozitiv indicând numărul de grade de libertate care caracterizează distribuția"
			}
		]
	},
	{
		name: "TODAY",
		description: "Returnează data curentă formatată ca dată.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "Transformă o zonă verticală de celule în zonă orizontală sau viceversa.",
		arguments: [
			{
				name: "matrice",
				description: "este o zonă de celule dintr-o foaie de lucru sau o matrice de valori, care se vor transpune"
			}
		]
	},
	{
		name: "TREND",
		description: "Returnează numere într-o tendință liniară care se potrivește punctelor de date cunoscute, utilizând metoda celor mai mici pătrate.",
		arguments: [
			{
				name: "y_cunoscute",
				description: "este o zonă sau o matrice de valori-y deja cunoscute în relația y = mx + b"
			},
			{
				name: "x_cunoscute",
				description: "este o zonă sau o matrice opțională de valori-x deja cunoscute din relația y = mx + b, o matrice de aceeași dimensiune cu y_cunoscute"
			},
			{
				name: "x_noi",
				description: "este o zonă sau o matrice de valori-x noi pentru care TREND returnează valorile-y corespunzătoare"
			},
			{
				name: "const",
				description: "este o valoare logică: constanta b este calculată normal dacă Const = TRUE sau se omite; b este egal cu 0 dacă Const = FALSE"
			}
		]
	},
	{
		name: "TRIM",
		description: "Elimină toate spațiile dintr-un șir text exceptând spațiile simple dintre cuvinte.",
		arguments: [
			{
				name: "text",
				description: "este textul din care se extrag spațiile"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "Returnează media porțiuni interioare a unui set de valori de date.",
		arguments: [
			{
				name: "matrice",
				description: "este zona sau matricea de valori de trunchiat și de mediat"
			},
			{
				name: "procent",
				description: "este numărul fracționar de puncte de date de exclus din partea de sus și din partea de jos a setului de date"
			}
		]
	},
	{
		name: "TRUE",
		description: "Returnează valoarea logică TRUE.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "Trunchiază un număr la un întreg eliminând partea zecimală, sau fracționară a numărului.",
		arguments: [
			{
				name: "număr",
				description: "este numărul de trunchiat"
			},
			{
				name: "num_cifre",
				description: "este un număr care specifică precizia trunchierii, 0 (zero) dacă este omis"
			}
		]
	},
	{
		name: "TTEST",
		description: "Returnează probabilitatea asociată cu un test t Student.",
		arguments: [
			{
				name: "matrice1",
				description: "este primul set de date"
			},
			{
				name: "matrice2",
				description: "este al doilea set de date"
			},
			{
				name: "cozi",
				description: "specifică tipul distribuției de returnat: distribuție unilaterală = 1; distribuție bilaterală = 2"
			},
			{
				name: "tip",
				description: "este tipul de test t: pereche = 1, două eșantioane varianță egală (homoscedastică) = 2, două eșantioane varianță inegală = 3"
			}
		]
	},
	{
		name: "TYPE",
		description: "Returnează un întreg care reprezintă tipul de dată al unei valori: număr = 1; text = 2; valoare logică = 4; valoare de eroare = 16; matrice = 64.",
		arguments: [
			{
				name: "valoare",
				description: "poate să ia orice valoare"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Returnează numărul (punctul de cod) corespunzător primului caracter al textului.",
		arguments: [
			{
				name: "text",
				description: "este caracterul al cărui valoare Unicode o doriți"
			}
		]
	},
	{
		name: "UPPER",
		description: "Transformă un șir text în majuscule.",
		arguments: [
			{
				name: "text",
				description: "este textul de transformat în majuscule, o referință sau un șir text"
			}
		]
	},
	{
		name: "VALUE",
		description: "Transformă un șir text care reprezintă un număr într-un număr.",
		arguments: [
			{
				name: "text",
				description: "este textul în ghilimele sau o referință la o celulă care conține textul de transformat"
			}
		]
	},
	{
		name: "VAR",
		description: "Estimează varianța pe baza unui eșantion (ignoră valorile logice și textul din eșantion).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de argumente numerice corespunzătoare unui eșantion de populație"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de argumente numerice corespunzătoare unui eșantion de populație"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Calculează varianța bazată pe întreaga populație (ignoră  valorile logice și text din populație).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de argumente numerice care corespund unei populații"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de argumente numerice care corespund unei populații"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Estimează varianța bazată pe un eșantion (ignoră valorile logice și text din eșantion).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de argumente numerice corespunzătoare unui eșantion de populație"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de argumente numerice corespunzătoare unui eșantion de populație"
			}
		]
	},
	{
		name: "VARA",
		description: "Estimează varianța pe baza unui eșantion, incluzând valorile logice și text. Textul și valoarea logică FALSE au valoarea 0; valoarea logică TRUE are valoarea 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valoare1",
				description: "sunt de la 1 la 255 de argumente valoare care corespund unui eșantion de populație"
			},
			{
				name: "valoare2",
				description: "sunt de la 1 la 255 de argumente valoare care corespund unui eșantion de populație"
			}
		]
	},
	{
		name: "VARP",
		description: "Calculează varianța pe baza populației totale (ignoră valorile logice și textul din populație).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "număr1",
				description: "sunt de la 1 la 255 de argumente numerice care corespund unei populații"
			},
			{
				name: "număr2",
				description: "sunt de la 1 la 255 de argumente numerice care corespund unei populații"
			}
		]
	},
	{
		name: "VARPA",
		description: "Calculează varianța pe baza întregii populații, incluzând valori logice și text. Textul și valoarea logică FALSE au valoarea 0; valoarea logică TRUE are valoarea 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valoare1",
				description: "sunt de la 1 la 255 de argumente valoare care corespund unei populații"
			},
			{
				name: "valoare2",
				description: "sunt de la 1 la 255 de argumente valoare care corespund unei populații"
			}
		]
	},
	{
		name: "VDB",
		description: "Returnează amortizarea unui mijloc fix pentru orice perioadă specificată, incluzând perioade parțiale, utilizând metoda balanței amortizare dublă sau altă metodă specificată.",
		arguments: [
			{
				name: "cost",
				description: "este costul inițial al mijlocului fix"
			},
			{
				name: "recuperat",
				description: "este valoarea rămasă la sfârșitul duratei de viață a mijlocului fix"
			},
			{
				name: "viață",
				description: "este numărul de perioade în care se amortizează mijlocul fix (uneori este numită durata de utilizare a mijlocului fix)"
			},
			{
				name: "per_start",
				description: "este perioada de început de la care se calculează amortizarea, măsurată în aceleași unități ca viață"
			},
			{
				name: "per_ultima",
				description: "este perioada de sfârșit pentru care se calculează amortizarea, măsurată în aceleași unități ca viață"
			},
			{
				name: "factor",
				description: "este rata la care se deplasează balanța, 2 (balanță cu amortizare dublă) dacă este omisă"
			},
			{
				name: "nr_comutare",
				description: "comută în amortizare liniară când amortizarea este mai mare decât deplasarea balanței = FALSE sau omisă; nu comută = TRUE"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "Caută o valoare în coloana cea mai din stânga a unui tabel, apoi returnează o valoare în același rând dintr-o coloană precizată. Implicit, tabelul trebuie sortat în ordine ascendentă.",
		arguments: [
			{
				name: "căutare_valoare",
				description: "este valoarea de găsit în prima coloană a tabelului și este o valoare, o referință sau un șir text"
			},
			{
				name: "matrice_tabel",
				description: "este un tabel de text, numere sau valori logice, din care se preiau date. Table_matrice este o referință la o zonă sau la nume de zonă"
			},
			{
				name: "num_index_col",
				description: "este numărul de coloană din matrice_tabel din care se returnează valoarea corespondentă. Prima coloană de valori din tabel este coloana 1"
			},
			{
				name: "zonă_căutare",
				description: "este o valoare logică: pentru a găsi cea mai apropiată valoare în prima coloană (sortată în ordine ascendentă) = TRUE sau se omite; găsirea unei potriviri exacte = FALSE"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "Returnează un număr de la 1 la 7 care identifică ziua din săptămână a unei date calendaristice.",
		arguments: [
			{
				name: "număr_serie",
				description: "este un număr care reprezintă o dată"
			},
			{
				name: "tip_returnare",
				description: "este un număr: pentru duminică=1 până la sâmbătă=7, utilizați 1; pentru luni=1 până la duminică=7, utilizați 2; pentru luni=0 până la duminică=6, utilizați 3"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "Returnează numărul de săptămâni dintr-un an.",
		arguments: [
			{
				name: "număr_serie",
				description: "este codul zi-oră utilizat de Spreadsheet pentru calcularea datelor și duratelor"
			},
			{
				name: "tip_returnare",
				description: "este un număr (1 sau 2) care determină tipul de date care se returnează"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Returnează distribuția Weibull.",
		arguments: [
			{
				name: "x",
				description: "este valoarea la care se evaluează funcția, un număr nenegativ"
			},
			{
				name: "alfa",
				description: "este un parametru pentru distribuție, un număr pozitiv"
			},
			{
				name: "beta",
				description: "este un parametru pentru distribuție, un număr pozitiv"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică: pentru funcția distribuție cumulativă, se utilizează TRUE; pentru funcția masă de probabilitate, se utilizează FALSE"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "Returnează distribuția Weibull.",
		arguments: [
			{
				name: "x",
				description: "este valoarea la care se evaluează funcția, un număr nenegativ"
			},
			{
				name: "alfa",
				description: "este un parametru pentru distribuție, un număr pozitiv"
			},
			{
				name: "beta",
				description: "este un parametru pentru distribuție, un număr pozitiv"
			},
			{
				name: "cumulativ",
				description: "este o valoare logică: pentru funcția distribuție cumulativă, se utilizează TRUE; pentru funcția probabilitate de masă, se utilizează FALSE"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "Returnează numărul serial al datei înainte sau după un număr specificat de zile lucrătoare.",
		arguments: [
			{
				name: "dată_start",
				description: "este un număr serial de dată care reprezintă data de început"
			},
			{
				name: "zile",
				description: "este numărul de zile care nu sunt în weekend și nu sunt sărbători înainte sau după data_de început"
			},
			{
				name: "sărbători",
				description: "este un index de început pentru una sau mai multe nume de date seriale care se vor exclude din calendarul de lucru, cum ar fi zilele de sărbătoare legale"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "Returnează numărul serial al datei înainte sau după un număr specificat de zile lucrătoare cu parametri sfârșit de săptămână particularizați.",
		arguments: [
			{
				name: "dată_start",
				description: "este un număr serial de dată care reprezintă data de început"
			},
			{
				name: "zile",
				description: "este numărul de zile care nu sunt în weekend și nu sunt sărbători înainte sau după data_de început"
			},
			{
				name: "weekend",
				description: "indică zilele care cad la sfârșit de săptămână. FIX ME"
			},
			{
				name: "sărbători",
				description: "este un index de început pentru una sau mai multe nume de date seriale care se vor exclude din calendarul de lucru, cum ar fi zilele de sărbătoare legale"
			}
		]
	},
	{
		name: "XIRR",
		description: "Returnează rata internă de întoarcere pentru un calendar de fluxuri monetare.",
		arguments: [
			{
				name: "valori",
				description: "o serie de fluxuri monetare care corespunde unui calendar de plăți în funcție de date"
			},
			{
				name: "date",
				description: "este un calendar de date de plată care corespunde plăților fluxurilor monetare"
			},
			{
				name: "estim",
				description: "este un număr pe care îl ghiciți că se apropie de rezultatul XIRR"
			}
		]
	},
	{
		name: "XNPV",
		description: "Returnează valoarea netă prezentă a fluxurilor monetare.",
		arguments: [
			{
				name: "rată",
				description: "este rata de reducere care se aplică fluxurilor monetare"
			},
			{
				name: "valori",
				description: "este o serie de fluxuri monetare care corespunde unui calendar de plăți în funcție de date"
			},
			{
				name: "date",
				description: "este un calendar de date de plată care corespunde cu plățile fluxului monetar"
			}
		]
	},
	{
		name: "XOR",
		description: "Returnează un „Exclusive Or” logic al tuturor argumentelor.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logic1",
				description: "reprezintă 1 - 254 condiții pe care doriți să le testați care pot fi evaluate cu ADEVĂRAT sau FALS și pot fi valori logice, matrice sau referințe"
			},
			{
				name: "logic2",
				description: "reprezintă 1 - 254 condiții pe care doriți să le testați care pot fi evaluate cu ADEVĂRAT sau FALS și pot fi valori logice, matrice sau referințe"
			}
		]
	},
	{
		name: "YEAR",
		description: "Returnează anul, un întreg în intervalul 1900 - 9999.",
		arguments: [
			{
				name: "număr_serie",
				description: "este un număr în cod dată-oră utilizat de Spreadsheet"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "Returnează fracțiunea de an care reprezintă numărul de zile întregi între data_de început și data_de sfârșit.",
		arguments: [
			{
				name: "dată_start",
				description: "este numărul serial care reprezintă data de început"
			},
			{
				name: "dată_sfârșit",
				description: "este numărul serial care reprezintă data de sfârșit"
			},
			{
				name: "bază",
				description: "este tipul de bază de numărare a zilelor care trebuie utilizat"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "Returnează produsul anual pentru o asigurare cu reducere. De exemplu, certificatele de trezorerie.",
		arguments: [
			{
				name: "tranzacție",
				description: "este data de lichidare a asigurării, exprimată ca număr serial"
			},
			{
				name: "maturitate",
				description: "este data de stabilire a asigurării, exprimată ca număr de dată serial"
			},
			{
				name: "pr",
				description: "este data de maturizare a asigurării, exprimată ca număr serial"
			},
			{
				name: "rambursare",
				description: "este prețul asigurării la 100 lei valoare reală"
			},
			{
				name: "bază",
				description: "este tipul de bază de numărare a zilelor care trebuie utilizat"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Returnează valoarea P unilaterală a unui test z.",
		arguments: [
			{
				name: "matrice",
				description: "este matricea sau zona de date față de care se face testul X"
			},
			{
				name: "x",
				description: "este valoarea de testat"
			},
			{
				name: "sigma",
				description: "este deviația standard a populației (cunoscută). Dacă este omisă, este utilizată deviația standard a eșantionului"
			}
		]
	},
	{
		name: "ZTEST",
		description: "Returnează valoarea P unilaterală a unui test z.",
		arguments: [
			{
				name: "matrice",
				description: "este matricea sau zona de date față de care se face testul X"
			},
			{
				name: "x",
				description: "este valoarea de testat"
			},
			{
				name: "sigma",
				description: "este deviația standard a populației (cunoscută). Dacă este omisă, este utilizată deviația standard a eșantionului"
			}
		]
	}
];