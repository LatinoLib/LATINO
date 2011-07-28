using System;
namespace Latino.LemmaSharp {
    public interface ILemmatizerModel {
        string Lemmatize(string sWord);
        string ToString();
    }
}
