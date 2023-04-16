#include "pch.h"
#include "mkl.h"

extern "C" _declspec(dllexport)
double func(int NumSplines, int NumNodes, const double Points[], const double Values[], bool isUniform, double ldrd[], double coeff[], double res[], double integral[]) {
	DFTaskPtr task;
	MKL_INT dorder[] = { 1, 1, 1 };
	double llim[] = {Points[0]};
	double rlim[] = {Points[NumNodes - 1]};
	double PointsStartEnd[] = {Points[0], Points[NumNodes - 1]};
	if (isUniform) {
		dfdNewTask1D(&task, NumNodes, PointsStartEnd, DF_UNIFORM_PARTITION, 1, Values, DF_MATRIX_STORAGE_ROWS);
	}
	else {
		dfdNewTask1D(&task, NumNodes, Points, DF_NON_UNIFORM_PARTITION, 1, Values, DF_MATRIX_STORAGE_ROWS);
	}
	dfdEditPPSpline1D(task, DF_PP_CUBIC, DF_PP_NATURAL, DF_BC_2ND_LEFT_DER | DF_BC_2ND_RIGHT_DER, ldrd, DF_NO_IC, NULL, coeff, DF_NO_HINT);
	dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD);
	dfdInterpolate1D(task, DF_INTERP, DF_METHOD_PP, NumSplines, PointsStartEnd, DF_UNIFORM_PARTITION, 3, dorder, NULL, res, DF_MATRIX_STORAGE_ROWS, NULL);
	dfdIntegrate1D(task, DF_METHOD_PP, 1, llim, DF_UNIFORM_PARTITION, rlim, DF_UNIFORM_PARTITION, NULL, NULL, integral, DF_MATRIX_STORAGE_ROWS);
	dfDeleteTask(&task);
	return 1;
}