// Includes
#include "stdafx.h"

//------------------------------------------------------------------------
ofstream logfile;
//------------------------------------------------------------------------

int Write_Txt_File_Of_Double(const char *fname, double *massiv, int n_of_records, int len_of_record)
{
	FILE *fp;

	if ((fp = fopen(fname, "w")) == 0) {
		printf("Error: Cannot open file \"%s\" for writing.\n", fname);
		return 1;
	}

	printf("writing %s... ", fname);

	for (int i = 0; i < n_of_records; i++) {
		for (int j = 0; j < len_of_record; j++)
			fprintf(fp, "%25.13e\t", massiv[i*len_of_record + j]);
		fprintf(fp, "\n");
	}
	printf("done\n");

	fclose(fp);
	return 0;
}

//------------------------------------------------------------------------
int Read_Long_From_Txt_File(const char *fname, int *number)
{
	FILE *fp;
	if ((fp = fopen(fname, "r")) == 0) {
		char str[100];
		sprintf(str, "Error: Cannot open file \"%s\" for reading.\n", fname);
		return 1;
	}

	int temp;
	int retcode = fscanf(fp, "%ld", &temp);
	if (retcode != 1) {
		char str[100];
		sprintf(str, "Error reading file \"%s\".\n", fname);
		fclose(fp);
	}
	*number = temp;

	fclose(fp);
	return 0;
}

//------------------------------------------------------------------------
int Read_Bin_File_Of_Double(const char *fname, double *massiv, int n_of_records, int len_of_record)
{
	FILE *fp;
	if ((fp = fopen(fname, "r+b")) == 0) {
		char str[100];
		sprintf(str, "Error: Cannot open file \"%s\" for reading.\n", fname);
		return 1;
	}

	int temp = fread(massiv, sizeof(double)*len_of_record, n_of_records, fp);
	if (temp != n_of_records) {
		char str[100];
		sprintf(str, "Error reading file \"%s\". %ld of %ld records was read.\n", fname, temp, n_of_records);
		fclose(fp);
		return 1;
	}

	fclose(fp);
	return 0;
}

//------------------------------------------------------------------------
int Read_Bin_File_Of_Long(const char *fname, int *massiv, int n_of_records, int len_of_record)
{
	FILE *fp;

	if ((fp = fopen(fname, "r+b")) == 0) {
		char str[100];
		sprintf(str, "Cannot open file %s.\n", fname);
		return 1;
	}

	int temp = fread(massiv, sizeof(int)*len_of_record, n_of_records, fp);
	if (temp != n_of_records) {
		char str[100];
		sprintf(str, "Error reading file \"%s\". %ld of %ld records was read.\n", fname, temp, n_of_records);
		fclose(fp);
		return 1;
	}

	fclose(fp);
	return 0;
}

//------------------------------------------------------------------------
void FromRSFToCSR_Real_1_Sym(int nb, int *ig, int *sz_ia, int *sz_ja)
{
	*sz_ia = nb + 1;
	*sz_ja = ig[nb] + nb;
}

//------------------------------------------------------------------------
void FromRSFToCSR_Real_2_Sym(int nb, int *ig, int *jg, double *di, double *gg,
	MKL_INT *ia, MKL_INT *ja, double *a)
{
	vector<MKL_INT> adr;

	// подсчитываем число элементов в каждой строчке
	adr.resize(nb, 0);

	for (int i = 0; i<nb; i++) {
		adr[i] += 1; // диагональ
		// верхний треугольник
		for (int j = ig[i]; j <= ig[i + 1] - 1; j++) {
			int k = jg[j];
			adr[k]++;
		}
	}

	// ia
	ia[0] = 0;
	for (int i = 0; i<nb; i++)
		ia[i + 1] = ia[i] + adr[i];

	// ja,  a
	for (int i = 0; i<ig[nb] + nb; i++)
		a[i] = 0;

	for (int i = 0; i<nb; i++)
		adr[i] = ia[i]; // в какую позицию заносить значение

	// диагональ
	for (int i = 0; i<nb; i++) {
		ja[adr[i]] = i;
		a[adr[i]] = di[i];
		adr[i]++;
	}

	// верхний треугольник
	for (int i = 0; i<nb; i++) {
		for (int j = ig[i]; j <= ig[i + 1] - 1; j++) {
			int k = jg[j];
			ja[adr[k]] = i;
			a[adr[k]] = gg[j];
			adr[k]++;
		}
	}
}

//------------------------------------------------------------------------
int _tmain(int argc, _TCHAR* argv[])
{
	// Путь к СЛАУ
	string path = "test/";

	logfile.open(path + "pardiso64.log");
	if (!logfile) {
		cerr << "Cannot open pardiso64.log" << endl;
		return 1;
	}

	int i;

	int ig_n_1 = 0;
	int sz_ia = 0;
	int sz_ja = 0;

	double *pr = NULL;
	double *x = NULL;

	// Исходная матрица в разреженном строчно-столбцовом формате
	int nb;
	int *ig = NULL;
	int *jg = NULL;
	double *di = NULL;
	double *gg = NULL;

	// Матрица, сконвертированная в разреженный строчный формат
	MKL_INT *ia = NULL;
	MKL_INT *ja = NULL;
	double *a = NULL;

	// Для PARDISO
	clock_t begin = clock();
	MKL_INT n = 0;
	MKL_INT mtype = 2; // real and symmetric positive definite
	MKL_INT nrhs = 1;
	void *pt[64];
	MKL_INT maxfct = 1;
	MKL_INT	mnum = 1;
	MKL_INT msglvl = 1;
	MKL_INT phase = 13;
	MKL_INT *perm = NULL;
	MKL_INT iparm[64];
	MKL_INT info=-100;

	for (i = 0; i < 64; i++)
		pt[i] = 0;

	iparm[0] = 0; //iparm(2) - iparm(64) заполняются значениями по умолчанию.
	//iparm[0] = 1; // Нужно указать все значения в компонентах iparm(2) - iparm(64).
	//for (i = 1; i < 64; i++)
	//	iparm[i] = 0;

	// Чтение размерности nb
	Read_Long_From_Txt_File((path + "kuslau").c_str(), &nb);

	// ig
	ig = new int[nb + 1];
	Read_Bin_File_Of_Long((path + "ig").c_str(), ig, nb + 1, 1);
	// Если нумерация с 1
	//for (i = 0; i<nb + 1; i++)
	//	ig[i]--;
	ig_n_1 = ig[nb];

	// jg
	jg = new int[ig_n_1];
	Read_Bin_File_Of_Long((path + "jg").c_str(), jg, ig_n_1, 1);
	// Если нумерация с 1
	//for (i = 0; i<ig_n_1; i++)
	//	jg[i]--;

	// di
	di = new double[nb];
	Read_Bin_File_Of_Double((path + "di").c_str(), di, nb, 1);

	// ggl
	gg = new double[ig_n_1];
	Read_Bin_File_Of_Double((path + "gg").c_str(), gg, ig_n_1, 1);

	// pr
	pr = new double[nb];
	Read_Bin_File_Of_Double((path + "pr").c_str(), pr, nb, 1);

	// Конвертирование в CSR
	FromRSFToCSR_Real_1_Sym(nb, ig, &sz_ia, &sz_ja);
	ia = new MKL_INT[sz_ia];
	ja = new MKL_INT[sz_ja];
	a = new double[sz_ja];
	FromRSFToCSR_Real_2_Sym(nb, ig, jg, di, gg, ia, ja, a);

	for (i = 0; i<sz_ia; i++)
		ia[i]++;

	for (i = 0; i<sz_ja; i++)
		ja[i]++;

	if (ig) { delete[] ig; ig = NULL; }
	if (jg) { delete[] jg; jg = NULL; }
	if (di) { delete[] di; di = NULL; }
	if (gg) { delete[] gg; gg = NULL; }

	// Запуск PARDISO
	n = nb;
	x = new double[nb];
	perm = new MKL_INT[nb];

	cout << "pardiso start.." << endl << flush;

	PARDISO(pt, &maxfct, &mnum, &mtype, &phase, &n, a, ia, ja, perm, &nrhs, iparm, &msglvl, pr, x, &info);

	clock_t time = (clock() - begin) / CLOCKS_PER_SEC;

	ofstream fout;
	fout.open((path + "kit").c_str(), ios_base::app);
	fout << "n=" << nb << " pardiso time=" << time << " s" << endl;
	fout.close();

	// Запись решения
	Write_Txt_File_Of_Double((path + "x.txt").c_str(), x, nb, 1);

	// Запись и вывод info
	cout << "info=" << info << endl;
	logfile << "info=" << info << endl;

	if (a)    { delete[] a;    a    = NULL; }
	if (x)    { delete[] x;    x    = NULL; }
	if (pr)   { delete[] pr;   pr   = NULL; }
	if (ia)   { delete[] ia;   ia   = NULL; }
	if (ja)   { delete[] ja;   ja   = NULL; }
	if (perm) { delete[] perm; perm = NULL; }
	
	logfile.close();
	logfile.clear();
	return 0;
}