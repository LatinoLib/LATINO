using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Latino.LemmaSharp {
    public interface ILemmatizer : ISerializable {
        string Lemmatize(string sWord);
    }
}
