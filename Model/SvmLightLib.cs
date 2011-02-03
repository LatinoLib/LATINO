/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    SvmLightLib.cs
 *  Desc:	 SVM^light and SVM^multiclass C# wrapper
 *  Created: Aug-2007
 * 
 *  Authors: Miha Grcar 
 * 
 ***************************************************************************/

using System.Runtime.InteropServices;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Class SvmLightLib 
       |
       '-----------------------------------------------------------------------
    */
    internal static class SvmLightLib
    {
#if DEBUG
        const string SVMLIGHTLIB_DLL = "SvmLightLibDebug.dll";
#else
        const string SVMLIGHTLIB_DLL = "SvmLightLib.dll";
#endif
        public delegate void WriteByteCallback(byte b);
        public delegate byte ReadByteCallback();

        // label is 1 or -1 for inductive binary SVM; 1, -1, or 0 (unlabeled) for transductive binary SVM; 
        // a positive integer for multiclass SVM; a real value for SVM regression
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern int NewFeatureVector(int featureCount, int[] features, float[] weights, double label);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void DeleteFeatureVector(int id);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern int GetFeatureVectorFeatureCount(int featureVectorId);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern int GetFeatureVectorFeature(int featureVectorId, int featureIdx);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern float GetFeatureVectorWeight(int featureVectorId, int featureIdx);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern double GetFeatureVectorLabel(int featureVectorId);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void SetFeatureVectorLabel(int featureVectorId, double label);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern int GetFeatureVectorClassifScoreCount(int featureVectorId);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern double GetFeatureVectorClassifScore(int featureVectorId, int classifScoreIdx);

        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void _TrainModel(string args);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern int TrainModel(string args, int featureVectorCount, int[] featureVectors);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void SaveModel(int modelId, string fileName);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern int LoadModel(string fileName);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void SaveModelBin(int modelId, string fileName);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern int LoadModelBin(string fileName);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void SaveModelBinCallback(int modelId, WriteByteCallback callback);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern int LoadModelBinCallback(ReadByteCallback callback);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void _Classify(string args);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void Classify(int modelId, int featureVectorCount, int[] featureVectors);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void DeleteModel(int id);

        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void _TrainMulticlassModel(string args);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern int TrainMulticlassModel(string args, int featureVectorCount, int[] featureVectors);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void SaveMulticlassModel(int modelId, string fileName);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern int LoadMulticlassModel(string fileName);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void SaveMulticlassModelBin(int modelId, string fileName);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern int LoadMulticlassModelBin(string fileName);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void SaveMulticlassModelBinCallback(int modelId, WriteByteCallback callback);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern int LoadMulticlassModelBinCallback(ReadByteCallback callback);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void _MulticlassClassify(string args);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void MulticlassClassify(int modelId, int featureVectorCount, int[] featureVectors);
        [DllImport(SVMLIGHTLIB_DLL)]
        public static extern void DeleteMulticlassModel(int id);
    }
}